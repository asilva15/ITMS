using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using System.Web.Security;
using System.IO;
using System.Text;
using ERPElectrodata.Objects;

namespace ERPElectrodata.Controllers
{
    public class DeliverySustainController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        AppLogEntities dba = new AppLogEntities();

        //
        // GET: /DeliverySustain/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            Boolean resp = false;
            int ID_PERS_ENTI_RESP = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string ID_PERS_ENTIs = Convert.ToString(ID_PERS_ENTI);
            try
            {
                // Muestra a los responsables de Caja Chica: Melisa, Juan, Ronald, Luis
                ID_PERS_ENTI_RESP = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 37) //ID_PARA=37=>RESPONSABLE PETTY CASH
                    .Where(x => x.ID_ACCO == ID_ACCO)
                    .Where(x => x.VAL_ACCO_PARA == ID_PERS_ENTIs).Count();

                if (ID_PERS_ENTI_RESP > 0) //Si es que se ha logueado un reposnsable de Caja chica. 
                {
                    ID_PERS_ENTI_RESP = ID_PERS_ENTI;
                    resp = true;
                }

                REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(id);

                var amount = (from de in dbc.MontoRecepcion(id) select new { de.AMOUNT });

                decimal TotalRequest = Convert.ToDecimal(re.AMOUNT);

                decimal TotalRecepcion = amount.Sum(x => Convert.ToDecimal(x.AMOUNT));

                ViewBag.MontoADevolver = TotalRecepcion - TotalRequest;
                ViewBag.Estado = re.ID_STAT_REQU_EXPE;



            }
            catch
            {

            }

            ViewBag.resp = resp;
            ViewBag.id = id;
            ViewBag.xRendir = EsAdministrador();
            ViewBag.PerfilEditarMonto = Convert.ToString(dbe.PerfilEditarMonto(ID_ACCO,ID_PERS_ENTI,Session["Cargo"].ToString(),id).Single());

            return View();
        }

        public ActionResult ListarDetalleViatico()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            bool resp = Convert.ToBoolean(Request.Params["resp"]);
            int ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
            var qDetalle = dbc.ListarDetalleViatico(id).ToList();
            var qDet = (from a in qDetalle
                        select new
                        {
                            SwxRendir = ((resp || EsAdministrador()) && (a.ID_STAT_REQU_EXPE == 1 || a.ID_STAT_REQU_EXPE == 5) ? true : false ),
                            SwRendir = ((ID_PERS_ENTI == (a.ID_PERS_ENTI_ASSI == 0 ? -1 : a.ID_PERS_ENTI_ASSI) || resp) && a.ID_STAT_REQU_EXPE == 6 ? true : false),
                            SwPrint = (a.ID_TYPE_DELI_SUST == 2 && (a.ID_STAT_REQU_EXPE == 6 || a.ID_STAT_REQU_EXPE == 7) && (ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || resp))
                            
                        });

            var querydetalle = qDet.FirstOrDefault();
            return Json(new { data = qDetalle, data2 = qDet}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListDeliverySustainType(int id = 0)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = (from a in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI && x.ID_TYPE_DELI_SUST == id).ToList()
                         select new
                         {
                             a.DOC_NUMB,
                             a.DOC_DATE,
                             DATE_DOC = (a.DOC_DATE == null ? "-" : string.Format("{0:d}", a.DOC_DATE)),
                             COIN = (a.COIN == "MN" ? "S/." : "US$"),
                             a.DOC_AMOU,
                             a.DOC_REMA,
                             //a.AMO_REGI,
                             a.DAT_REGI,
                             DOC_STAT = (a.DOC_STAT == "P" ? "Por Rendir" : "Rendido"),
                             NULO = "",
                             a.ID_DELI_SUST,
                         }).OrderByDescending(x => x.DOC_DATE);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListDeliverySustainByTypeByStat(int id = 1, string stat = "P")
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = (from a in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI
                                    && x.ID_TYPE_DELI_SUST == id && x.DOC_STAT == stat).ToList()
                         join b in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals b.ID_TYPE_DELI_SUST
                         select new {
                            DOC_NUMB = a.DOC_NUMB.Trim(),
                            a.DOC_DATE,
                            DATE_DOC = (a.DOC_DATE == null ? "-" :  string.Format("{0:d}",a.DOC_DATE)),
                            COIN = (a.COIN == "MN" ? "S/." : "US$"),
                            CURRENCY = a.COIN,
                            AMOUNT = (a.DOC_AMOU == null ? "-" : string.Format("{0:N2}", a.DOC_AMOU)),
                            REMAIN = (a.DOC_REMA == null ? "-" : string.Format("{0:N2}", a.DOC_REMA)),
                            //AMOUNT_REGI = (a.AMO_REGI == null ? "-" : string.Format("{0:N2}", a.AMO_REGI)),
                            a.DOC_AMOU,
                            a.DOC_REMA,
                            //a.AMO_REGI,
                            a.DAT_REGI,
                            DOC_STAT = (a.DOC_STAT == "P" ? "Por Rendir" : "Rendido"),
                            NULO = "",
                            a.ID_DELI_SUST,
                            b.ID_TYPE_DELI_SUST,
                            b.NAM_TYPE_DELI_SUST,
                            //dataText = a.DOC_NUMB.Trim() + (a.COIN == "MN" ? " (S/. " : " (US$ ") + string.Format("{0:N2}", a.DOC_AMOU)+
                            //         " / " + string.Format("{0:N2}", a.AMO_REGI) + ")",
                            //ID_DOCU_SALE = (a.ID_DOCU_SALE == null ? 0 : a.ID_DOCU_SALE),
                            //REASON = (a.REASON == null ? "" : a.REASON),
                            //PLACE = (a.PLACE == null ? "" : a.PLACE),
                         }).OrderByDescending(x=>x.DOC_DATE);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult viewNewSustain() {
            return View();
        }

        [Authorize]
        public ActionResult viewNewRequest() {
            return View();
        }

        public ActionResult ListPettyCashAvailable(string mon = "", decimal tt = 0) { 

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);         

            var data = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI && x.COIN == mon && x.ID_STAT_DELI_SUST == 1 && x.ID_TYPE_DELI_SUST==2).ToList()      // Lista las cajas chicas con estado 1:Register
                         join tds in dbc.TYPE_DELI_SUST on ds.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                         join sds in dbc.STATUS_DELI_SUST on ds.ID_STAT_DELI_SUST equals sds.ID_STAT_DELI_SUST
                         select new
                         { 
                             ds.ID_DELI_SUST,
                             ds.DOC_NUMB,
                             ds.DOC_AMOU,
                             AMO_AVAI = GetTotalAvailable(ds.ID_DELI_SUST),
                             ds.DOC_DATE,
                             tds.NAM_TYPE_DELI_SUST,
                             sds.NAM_STAT_DELI_SUST,
                            
                         });

                var query= (from a in data.Where(x=>x.AMO_AVAI>=tt)
                            select new
                          {
                              a.ID_DELI_SUST,
                              DOC_NUMB = a.DOC_NUMB.Trim(),
                              DOC_AMOU = (a.DOC_AMOU == null ? "0.00" : String.Format("{0:N2}", a.DOC_AMOU)),
                              AMO_AVAI = (a.AMO_AVAI == 0 ? "0.00" : String.Format("{0:N2}", a.AMO_AVAI)),
                              DAT_REG = (a.DOC_DATE == null ? "" : String.Format("{0:d}", a.DOC_DATE)),
                              NAME_TYPE = a.NAM_TYPE_DELI_SUST,
                              NAM_STAT_DELI_SUST = a.NAM_STAT_DELI_SUST,
                              DOC_NUMB_AND_NAME_TYPE = a.DOC_NUMB.Trim() + " " + a.NAM_TYPE_DELI_SUST,
                          });
            
            return Json(new { Data = query, Count = query.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListPettyCashPrevious()
        {

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var data = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI &&  x.ID_TYPE_DELI_SUST==1 &&   // Cuando hay una caja chica cerrada(ID_STAT_DELI_SUST=3) y no ha sido asignada ha otra 
                                                               x.ID_STAT_DELI_SUST == 3 && x.AMO_AVAI_ASSI == false).ToList()
                        join tds in dbc.TYPE_DELI_SUST on ds.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                        join sds in dbc.STATUS_DELI_SUST on ds.ID_STAT_DELI_SUST equals sds.ID_STAT_DELI_SUST
                        select new
                        {
                            ds.ID_DELI_SUST,
                            ds.DOC_NUMB,
                            ds.DOC_AMOU,
                            AMO_AVAI = GetTotalAvailable(ds.ID_DELI_SUST),
                            ds.DOC_DATE,
                            tds.NAM_TYPE_DELI_SUST,
                            sds.NAM_STAT_DELI_SUST,
                        });

            var query = (from a in data
                         select new
                         {
                             a.ID_DELI_SUST,
                             DOC_NUMB = a.DOC_NUMB.Trim(),
                             DOC_AMOU = (a.DOC_AMOU == null ? "0.00" : String.Format("{0:N2}", a.DOC_AMOU)),
                             AMO_AVAI = (a.AMO_AVAI == 0 ? "0.00" : String.Format("{0:N2}", a.AMO_AVAI)),
                             DAT_REG = (a.DOC_DATE == null ? "" : String.Format("{0:d}", a.DOC_DATE)),
                             NAME_TYPE = a.NAM_TYPE_DELI_SUST,
                             NAM_STAT_DELI_SUST=a.NAM_STAT_DELI_SUST,
                             DOC_NUMB_AND_NAME_TYPE=a.DOC_NUMB.Trim()+" "+a.NAM_TYPE_DELI_SUST,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult viewRequestReport() {
            return View();
        }

        public ActionResult ListByID_PERS_ENTI_ASSI() {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var qDS = (from a in dbc.REQUEST_EXPENSE.Where(x => x.ID_DELI_SUST != null && x.ID_STAT_REQU_EXPE != 2 &&
                           (x.ID_TYPE_DELI_SUST == 1 && x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI) ||
                           (x.ID_TYPE_DELI_SUST == 2 && (x.ID_PERS_ENTI_REQU == ID_PERS_ENTI ||
                                                         x.ID_PERS_ENTI_APPR == ID_PERS_ENTI ||
                                                         x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI ||
                                                         x.ID_PERS_ENTI_APPR_VIAT == ID_PERS_ENTI)))
                       select new { 
                            a.ID_DELI_SUST
                       });

            var query = (from a in dbc.DELIVERY_SUSTAIN.ToList()
                         where (from a1 in qDS select a1.ID_DELI_SUST).Contains(a.ID_DELI_SUST)
                         join b in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals b.ID_TYPE_DELI_SUST
                         join c in dbc.DELIVERY_SUSTAIN on (a.ID_PREV == null ? 0 : a.ID_PREV) equals c.ID_DELI_SUST into lc
                         from xc in lc.DefaultIfEmpty()
                         select new {
                             DOC_NUMB = a.DOC_NUMB.Trim(),
                             a.COIN,
                             DOC_AMOU = (a.COIN == "MN" ? "S/. " : "US$ ") + String.Format("{0:N2}",a.DOC_AMOU),
                             a.DOC_REMA,
                             a.ID_DELI_SUST,
                             DAT_REG = (a.DAT_REGI == null ? "" : String.Format("{0:d}",a.DAT_REGI)),
                             AMO_AVAI = (a.COIN == "MN" ? "S/. " : "US$ ") + String.Format("{0:N2}", a.AMO_AVAI),
                             AMO_PREV = (a.COIN == "MN" ? "S/. " : "US$ ") + 
                                        (xc == null ? "0.00" : String.Format("{0:N2}", xc.AMO_AVAI)),
                             b.NAM_TYPE_DELI_SUST
                         });

            return Json(new { Data = query, Count = query.Count() },JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult viewClosure() {
            var ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = dbc.PETTY_CASH_ASSIGNED.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI && x.VIG_PETT_CASH_ASSI == true);
            ViewBag.VisibleNew = 0;
            if (query.Count() > 0) { // Para que pueda ver el boton New Delivery(Caja Chica)
                ViewBag.VisibleNew = 1;
            }
            return View();
        }

        public ActionResult ListPettyCash() {
            
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);            
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            //Listado de personas
            var qUser = (from a in dbe.CLASS_ENTITY.Where(x=>x.COM_NAME == null) 
                         join b in dbe.PERSON_ENTITY.Where(x=>x.ID_ENTI1 == 9) on a.ID_ENTI equals b.ID_ENTI2                         
                         select new {
                            USER = a.FIR_NAME + " " + a.LAS_NAME,
                            b.ID_PERS_ENTI
                         });

            bool VIS_APPR = false,resp = false;
            string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            foreach (string rol in rolesArray) {
            if (rol == "ACCOUNTANT" || rol == "ADMINISTRADOR") {
                    VIS_APPR = true; //permitiendo al acceso
	            }
            }

            try
            {
                string ID_PERS_ENTIs = Convert.ToString(ID_PERS_ENTI);
                int ID_PERS_ENTI_RESP = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 37)
                    .Where(x => x.ID_ACCO == ID_ACCO).Where(x => x.VAL_ACCO_PARA == ID_PERS_ENTIs).Count();

                if (ID_PERS_ENTI_RESP > 0)
                {
                    ID_PERS_ENTI_RESP = ID_PERS_ENTI;
                    resp = true;
                }
            }
            catch
            {

            }

            var query = (from a in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI || VIS_APPR == true).ToList()
                        join b in qUser on a.ID_PERS_ENTI_ASSI equals b.ID_PERS_ENTI
                        join c in dbc.STATUS_DELI_SUST on a.ID_STAT_DELI_SUST equals c.ID_STAT_DELI_SUST
                        join tds in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST                        
                        select new
                        {
                            a.ID_DELI_SUST,
                            a.DOC_NUMB,                           
                            a.DOC_DATE,
                            a.DOC_AMOU,                            
                            a.AMO_AVAI_PREV,
                            a.ID_TYPE_DELI_SUST,
                            a.DOC_REMA,
                            a.AMO_AVAI,
                            a.ID_PERS_ENTI_ASSI,                           
                            a.COIN,
                            a.ID_STAT_DELI_SUST,                             
                            b.USER,
                            c.NAM_STAT_DELI_SUST,
                            tds.NAM_TYPE_DELI_SUST,                                                     
                        });

            var data = (from q in query.OrderByDescending(x => x.DOC_DATE).Skip(skip).Take(take)
                        select new
                        {
                            q.ID_DELI_SUST,
                            q.DOC_NUMB,
                            q.DOC_DATE,
                            q.DOC_AMOU,
                            q.AMO_AVAI_PREV,
                            q.ID_TYPE_DELI_SUST,
                            q.DOC_REMA,
                            q.AMO_AVAI,
                            q.ID_PERS_ENTI_ASSI,
                            q.NAM_STAT_DELI_SUST,
                            q.COIN,
                            q.NAM_TYPE_DELI_SUST,                           
                            q.USER,                         
                            q.ID_STAT_DELI_SUST,                            
                        });

            var result = (from d in data.ToList()
                          select new
                          {
                           d.ID_DELI_SUST,
                           d.DOC_NUMB,
                           d.DOC_DATE,
                           d.DOC_AMOU,
                           SUBSTANT = d.DOC_AMOU + (d.AMO_AVAI_PREV == null ? 0 : d.AMO_AVAI_PREV) - ((d.ID_TYPE_DELI_SUST == 2) ? d.DOC_AMOU-GetAmountRemain(d.ID_DELI_SUST) : GetTotalRemaining(d.ID_DELI_SUST)),
                           UNSUBSTA = Unsubstantiated(d.ID_DELI_SUST),
                           AMO_AVAI = (d.ID_TYPE_DELI_SUST == 2) ? d.DOC_AMOU - GetAmountAvailable(d.ID_DELI_SUST) : GetTotalAvailable(d.ID_DELI_SUST),
                           COIN = (d.COIN == "MN" ? "S/." : "US$"),
                           d.USER,
                           NAM_STAT = TextCapitalize(d.NAM_STAT_DELI_SUST),
                           PREVIUS = (d.AMO_AVAI_PREV == null ? 0 : d.AMO_AVAI_PREV),
                           VIS_APPR = VIS_APPR,
                           d.ID_STAT_DELI_SUST,
                           btnPrint = (d.ID_STAT_DELI_SUST == 2 || d.ID_STAT_DELI_SUST == 3) &&
                                      ((d.ID_PERS_ENTI_ASSI == ID_PERS_ENTI) || resp),
                           NAME_TYPE = d.NAM_TYPE_DELI_SUST,
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public decimal Unsubstantiated(int id = 0) {
            try
            {
                var query = (from a in dbc.REQUEST_EXPENSE.Where(x => x.ID_DELI_SUST == id &&
                                 (x.ID_STAT_REQU_EXPE == 5 || x.ID_STAT_REQU_EXPE == 6)) //5:Pendiente, 6:Validado. 
                             group a by new { a.ID_DELI_SUST } into g
                             select new
                             {
                                 g.Key.ID_DELI_SUST,
                                 SUMA = g.Sum(x => x.AMOUNT)
                             }).Single();

                return Convert.ToDecimal(query.SUMA);
            }
            catch (Exception)
            {

                return 0;
            }
            
        }
        // Aprueba haber recibido la caja chica.  
        public string ReceptionRequest(int id = 0)
        {
            try
            {
                var row = dbc.DELIVERY_SUSTAIN.Find(id);
                row.ID_STAT_DELI_SUST = 2;  //2:Validado
                dbc.DELIVERY_SUSTAIN.Attach(row);
                dbc.Entry(row).State = EntityState.Modified;
                dbc.SaveChanges();

                DateTime fec = DateTime.Now;
                var dsa = dbc.DELIVERY_SUSTAIN_ACTIVITY.Single(x => x.ID_DELI_SUST == id && x.ID_STAT_DELI_SUST == 1);
                dsa.DAT_END = fec;
                dbc.DELIVERY_SUSTAIN_ACTIVITY.Attach(dsa);
                dbc.Entry(dsa).State = EntityState.Modified;
                dbc.SaveChanges();

                var ndsa = new DELIVERY_SUSTAIN_ACTIVITY();  //Agrega un nuevo detalle de caja chica. 
                ndsa.ID_DELI_SUST = id;
                ndsa.DAT_REGI = fec;
                ndsa.ID_STAT_DELI_SUST = 2;
                ndsa.UserId = Convert.ToInt32(Session["UserId"]);
                dbc.DELIVERY_SUSTAIN_ACTIVITY.Add(ndsa);
                dbc.SaveChanges();
            }
            catch (Exception)
            {
                return "Error";
            }
            return "OK";
        }

        public string ReceptionApprove(int id = 0)
        {
            try
            {
                var row = dbc.DELIVERY_SUSTAIN.Find(id);
                row.ID_STAT_DELI_SUST = 3; // Estado Cerrado

                var dscerrar = (from ds in dbc.DELIVERY_SUSTAIN.Where(x=>x.ID_DELI_SUST==id).ToList()
                                select new
                                {
                                    ds.ID_DELI_SUST,
                                    AVAI=GetTotalAvailable(ds.ID_DELI_SUST),
                                    REMA=GetTotalRemaining(ds.ID_DELI_SUST),
                                }).First();

                row.AMO_AVAI = dscerrar.AVAI;
                row.DOC_REMA = dscerrar.REMA;
                row.AMO_AVAI_ASSI = false;

                dbc.DELIVERY_SUSTAIN.Attach(row);
                dbc.Entry(row).State = EntityState.Modified;
                dbc.SaveChanges();

                DateTime fec = DateTime.Now;
                var dsa = dbc.DELIVERY_SUSTAIN_ACTIVITY.Single(x => x.ID_DELI_SUST == id && x.ID_STAT_DELI_SUST == 2);
                dsa.DAT_END = fec;
                dbc.DELIVERY_SUSTAIN_ACTIVITY.Attach(dsa);
                dbc.Entry(dsa).State = EntityState.Modified;
                dbc.SaveChanges();

                var ndsa = new DELIVERY_SUSTAIN_ACTIVITY();
                ndsa.ID_DELI_SUST = id;
                ndsa.DAT_REGI = fec;
                ndsa.ID_STAT_DELI_SUST = 3;
                ndsa.UserId = Convert.ToInt32(Session["UserId"]);
                dbc.DELIVERY_SUSTAIN_ACTIVITY.Add(ndsa);
                dbc.SaveChanges();

                return "OK";
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        public ActionResult ListActivitiesByID_DELI_SUST() {
            int ID_DELI_SUST = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());

            var qUser = (from a in dbe.CLASS_ENTITY.Where(x=>x.COM_NAME == null)
                         join b in dbe.PERSON_ENTITY.Where(x=>x.ID_ENTI1 == 9) on a.ID_ENTI equals b.ID_ENTI2
                         select new {
                            USER = a.FIR_NAME + " " + a.LAS_NAME,
                            b.ID_PERS_ENTI,
                            a.UserId
                         });

            var query = (from a in dbc.DELIVERY_SUSTAIN_ACTIVITY.Where(x => x.ID_DELI_SUST == ID_DELI_SUST).ToList()
                         join b in dbc.STATUS_DELI_SUST on a.ID_STAT_DELI_SUST equals b.ID_STAT_DELI_SUST
                         join c in qUser on a.UserId equals c.UserId
                         select new { 
                            a.DAT_REGI,
                            a.DAT_END,
                            NAM_STAT_DELI_SUST = TextCapitalize(b.NAM_STAT_DELI_SUST),
                            USER = TextCapitalize(c.USER)
                         });

            return Json(new { Data = query},JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewNewDelivery() {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //CREACIÓN DE UNA NUEVA CAJA CHICA.
        public ActionResult Create(DELIVERY_SUSTAIN ds) {  
            int sw = 0, ID_PREV=0;            
            string Error = "";
            if (ds.DOC_DATE == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn03; }           
            else if (ds.DOC_AMOU == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn09; }
            else if (ds.DOC_AMOU == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn09; }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDonePettyCash) top.uploadDonePettyCash('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }
            else {
                try
                {
                    int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    //Obtenido el saldo anterior
                    ds.ID_PERS_ENTI_ASSI = ID_PERS_ENTI;
                    ds.ID_TYPE_DELI_SUST = 1;//Petty Cash
                    ds.NAM_ANEX = Convert.ToString(Session["NAM_PERS"]);
                    ds.DOC_REMA = ds.DOC_AMOU; //Monto restante igual al inicial.
                    ds.AMO_AVAI = ds.DOC_AMOU; // Monto disponible igual al inicial. 
                    ds.DOC_STAT = "P";
                    ds.ID_STAT_DELI_SUST = 1;// Registrado
                    ds.AMO_AVAI_ASSI = false;
                    ds.ID_PREV = 0;// Si no hay cajas chicas 
                    ds.DAT_REGI = DateTime.Now;                    

                    try { 
                        ID_PREV = Convert.ToInt32(Request.Params["ID_PREV"]); 
                    }catch {
                        ID_PREV = 0;
                    }

                    var qDeli = dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == ID_PREV); //Esta es la nueva Caja chica 
                   
                    if (qDeli.Count() > 0) {   // Aqui se asigna el monto anterior a la nueva caja chica. 
                        ds.DOC_REMA = ds.DOC_REMA + qDeli.First().AMO_AVAI; // Lo restante ahora es el monto mas lo sobrante lo de la caja anterior
                        ds.AMO_AVAI = ds.AMO_AVAI + qDeli.First().AMO_AVAI;// Lo disponible ahora es el monto mas lo sobrante lo de la caja anterior
                        ds.ID_PREV = qDeli.First().ID_DELI_SUST;         // Asignamos el ID_DELI_SUST anterior.                         
                        ds.AMO_AVAI_PREV=qDeli.First().AMO_AVAI;

                        DELIVERY_SUSTAIN U_DS = qDeli.First(); 
                        U_DS.AMO_AVAI_ASSI = true;              // Ya se asigno a la nueva caja chica

                        dbc.DELIVERY_SUSTAIN.Attach(U_DS);
                        dbc.Entry(U_DS).State = EntityState.Modified;
                        dbc.SaveChanges();
                    }
                    dbc.DELIVERY_SUSTAIN.Add(ds);
                    dbc.SaveChanges();

                    DELIVERY_SUSTAIN_ACTIVITY dsa = new DELIVERY_SUSTAIN_ACTIVITY();  // Registramos la actividad de la caja chica.
                    dsa.ID_DELI_SUST = ds.ID_DELI_SUST;
                    dsa.ID_STAT_DELI_SUST = 1; 
                    dsa.DAT_REGI = DateTime.Now;
                    dsa.UserId = Convert.ToInt32(Session["UserId"]);
                    dbc.DELIVERY_SUSTAIN_ACTIVITY.Add(dsa);
                    dbc.SaveChanges();

                }
                catch (Exception e)
                {
                    var exc = new EXCEPTION();
                    exc.DAT_EXCE = DateTime.Now;
                    exc.MESSAGE = (e.InnerException == null ? e.Message : e.InnerException.Message);
                    exc.DES_EXCE = "Modulo Caja Chica: error al registrar un nuevo Delivery Sustain(Delivery)";
                    dba.EXCEPTIONs.Add(exc);
                    dba.SaveChanges();

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDonePettyCash) top.uploadDonePettyCash('ERROR','Error DB - AppLog');}window.onload=init;</script>");
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDonePettyCash) top.uploadDonePettyCash('OK','"+ResourceLanguaje.Resource.NewPettyCashRegisted + "');}window.onload=init;</script>");
            }
        }

        public ActionResult viewPaymentViatical()
        {
            string mon = Request.QueryString["mon"];
            decimal tt = Convert.ToDecimal(Request.QueryString["tt"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int idt = Convert.ToInt32(Request.QueryString["idt"]);

            int idstat = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).Single().ID_STAT_REQU_EXPE);

            decimal reembolso = Convert.ToDecimal(dbc.MontoRecepcion(id).Sum(m => m.AMOUNT));

            decimal ttreembolso = reembolso - tt;


            ViewBag.Moneda = mon;
            ViewBag.Total = tt;
            ViewBag.ID_REQU_EXPE = id;
            ViewBag.ID_TYPE_REQUEST = idt;
            ViewBag.ID_STAT_REQUEST = idstat;
            ViewBag.TotalReembolso = ttreembolso;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult PaymentViatical(IEnumerable<HttpPostedFileBase> files, DELIVERY_SUSTAIN ds) {

            int sw = 0;
            string Error = "";
            int ID_REQU_EXPE = Convert.ToInt32(Request.Form["HF_ID_REQU_EXPE"]);
            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(ID_REQU_EXPE);
            int ID_PERS_ENTI_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (ds.NUM_CHEC == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn05; }
            else if (ds.DOC_DATE == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn03; }
            else if (ds.NumeroPRC == null && re.ID_TYPE_DELI_SUST==2 && re.ID_STAT_REQU_EXPE != 9) { sw = 1; Error = "Ingresa el número PRC. Campo Obligatorio."; }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.toastDeliverySustain) top.toastDeliverySustain('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }
            

            
            decimal DOC_AMOU = Convert.ToDecimal(Request.Form["HF_DOC_AMOU"]);
            string CURRE = Convert.ToString(Request.Form["HF_COIN"]);

            ds.COIN = CURRE;
            ds.DOC_AMOU = DOC_AMOU;
            ds.DOC_REMA = DOC_AMOU;
            ds.AMO_AVAI = 0;
            ds.DAT_REGI = DateTime.Now;
            ds.ID_PERS_ENTI_ASSI = ID_PERS_ENTI_ASSI;
            ds.ID_TYPE_DELI_SUST = 2;//Tipo Viatico
            if (re.ID_TYPE_DELI_SUST == 1)
            {
                ds.ID_TYPE_DELI_SUST = 1;
            }
            if (re.ID_TYPE_DELI_SUST == 3)
            {
                ds.ID_TYPE_DELI_SUST = 3;
            }

            ds.ID_STAT_DELI_SUST = 1;//Estado Asignado
            ds.ID_PREV = 0;
            ds.AMO_AVAI_ASSI = true;
            dbc.DELIVERY_SUSTAIN.Add(ds);

                if (files != null)
                {
                    string ruta = "Depositos";

                    foreach (var file in files)
                    {
                        try
                        {
                        string tiposolicitud = dbc.TYPE_DELI_SUST.Where(x => x.ID_TYPE_DELI_SUST == re.ID_TYPE_DELI_SUST).Single().NAM_TYPE_DELI_SUST;

                        if (re.ID_STAT_REQU_EXPE == 9 && re.ID_TYPE_DELI_SUST != 3)
                        {
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = "DevolucionalUsuario" + "_" + re.ID_REQU_EXPE + "-" + ds.NUM_CHEC;
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_REQUE_EXPE = re.ID_REQU_EXPE;
                            ATTA.CREATE_ATTA = DateTime.Now;
                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/" + ruta + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);

                            re.SEND_MAIL = false;
                            re.ID_STAT_REQU_EXPE = 7;
                            dbc.SaveChanges();
                            REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                            rea.ID_REQU_EXPE = ID_REQU_EXPE;
                            rea.UserId = Convert.ToInt32(Session["UserId"]);
                            rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                            rea.DAT_STAR = DateTime.Now;
                            dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                            dbc.SaveChanges();

                            DELIVERY_SUSTAIN_ACTIVITY dsa = new DELIVERY_SUSTAIN_ACTIVITY();
                            dsa.ID_DELI_SUST = ds.ID_DELI_SUST;
                            dsa.ID_STAT_DELI_SUST = 1;
                            dsa.DAT_REGI = DateTime.Now;
                            dsa.UserId = Convert.ToInt32(Session["UserId"]);
                            dbc.DELIVERY_SUSTAIN_ACTIVITY.Add(dsa);
                            dbc.SaveChanges();
                        }
                        else
                        {


                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = tiposolicitud + "_" + ds.ID_DELI_SUST + "-" + ds.NUM_CHEC;
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_DELI_SUST = ds.ID_DELI_SUST;
                            ATTA.CREATE_ATTA = DateTime.Now;
                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/" + ruta + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);

                            re.ID_DELI_SUST = ds.ID_DELI_SUST;
                            re.SEND_MAIL = false;
                            re.ID_STAT_REQU_EXPE = 5;
                            if (re.ID_TYPE_DELI_SUST == 1)
                            {
                                re.ID_STAT_REQU_EXPE = 7;
                            }


                            if (re.ID_TYPE_DELI_SUST == 3)
                            {
                                re.ID_STAT_REQU_EXPE = dbc.STATUS_REQUEST_EXPENSE.Where(x => x.NAM_STAT_REQU_EXPE == "Reembolsado").Single().ID_STAT_REQU_EXPE;

                            }

                            re.ID_PERS_ENTI_ASSI = ID_PERS_ENTI_ASSI;


                            dbc.REQUEST_EXPENSE.Attach(re);
                            dbc.Entry(re).State = EntityState.Modified;
                            dbc.SaveChanges();

                            REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                            rea.ID_REQU_EXPE = ID_REQU_EXPE;
                            rea.UserId = Convert.ToInt32(Session["UserId"]);
                            rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                            rea.DAT_STAR = DateTime.Now;
                            dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                            dbc.SaveChanges();

                            DELIVERY_SUSTAIN_ACTIVITY dsa = new DELIVERY_SUSTAIN_ACTIVITY();
                            dsa.ID_DELI_SUST = ds.ID_DELI_SUST;
                            dsa.ID_STAT_DELI_SUST = 1;
                            dsa.DAT_REGI = DateTime.Now;
                            dsa.UserId = Convert.ToInt32(Session["UserId"]);
                            dbc.DELIVERY_SUSTAIN_ACTIVITY.Add(dsa);
                            dbc.SaveChanges();
                        }
                    }
                        catch (Exception e)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                       "if(top.toastDeliverySustain) top.toastDeliverySustain('ERROR','" + "Archivo inválido" + "');}window.onload=init;</script>");
                        }
                    }
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                       "if(top.toastDeliverySustain) top.toastDeliverySustain('ERROR','" + "Subir archivo pdf de depósito." + "');}window.onload=init;</script>");
                }
            

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.toastDeliverySustain) top.toastDeliverySustain('OK','" + "Pago Realizado" + "');}window.onload=init;</script>");            
        }

        public string TextCapitalize(string txt = "")
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(txt.ToLower());
        }

       
        public decimal GetAmountAvailable(int id = 0)
        {
            try
            {
                var MPC = dbc.DELIVERY_SUSTAIN.Single(x => x.ID_DELI_SUST == id); 
               
               var DocuExpe=(from de in dbc.DOCUMENT_EXPENSE.Where(x=>x.ID_STAT_DOCU_EXPE==3)
                             join tde in dbc.TYPE_DOCUMENT_EXPENSE.Where(x=>x.AFF_AVAI!=false) on de.ID_TYPE_DOCU_EXPE equals tde.ID_TYPE_DOCU_EXPE
                              select new
                              {
                                  de.ID_DOCU_EXPE,
                                  de.ID_REQU_EXPE,                                 
                                  de.ID_TYPE_DOCU_EXPE,
                                  de.AMOUNT,
                              });

               var data = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id)
                           join re in dbc.REQUEST_EXPENSE on ds.ID_DELI_SUST equals re.ID_DELI_SUST
                           join sre in dbc.STATUS_REQUEST_EXPENSE on re.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                           join de in DocuExpe on re.ID_REQU_EXPE equals de.ID_REQU_EXPE into ld
                           from lde in ld.DefaultIfEmpty()
                           select new
                           {
                               re.ID_DELI_SUST,
                               MONTO = ds.DOC_AMOU,
                               MONTOAVAI = (lde.ID_DOCU_EXPE == null && sre.AFF_PETT_CASH_AVAI == true) ? re.AMOUNT :
                                         (lde.ID_DOCU_EXPE != null && sre.AFF_PETT_CASH_AVAI == true && lde.ID_TYPE_DOCU_EXPE != 5) ? lde.AMOUNT:0
                           });

                if (data.Count() > 0) // Si Hay requerimientos
                {
                    var query = (from a in data
                                 group a by new { a.ID_DELI_SUST } into g
                                 select new
                                 {
                                     g.Key.ID_DELI_SUST,
                                     SUMA = g.Sum(x => x.MONTOAVAI)
                                 }).Single();

                    return (Convert.ToDecimal(query.SUMA));
                }else
                {
                    return 0; 
                }
                
            }
            catch (Exception)
            {
               return 0;
            }
        }

        public decimal GetAmountRemain(int id = 0)
        {
            try
            {
                var MPC = dbc.DELIVERY_SUSTAIN.Single(x => x.ID_DELI_SUST == id); //MPC:Monto Petty Cash

                var DocuExpe = (from de in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_STAT_DOCU_EXPE == 3)
                                join tde in dbc.TYPE_DOCUMENT_EXPENSE.Where(x => x.AFF_AVAI != false && x.ID_TYPE_DOCU_EXPE!=5) on de.ID_TYPE_DOCU_EXPE equals tde.ID_TYPE_DOCU_EXPE
                                select new
                                {
                                    de.ID_DOCU_EXPE,
                                    de.ID_REQU_EXPE,
                                    de.ID_TYPE_DOCU_EXPE,
                                    de.AMOUNT,
                                });

                var data = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id)
                            join re in dbc.REQUEST_EXPENSE on ds.ID_DELI_SUST equals re.ID_DELI_SUST
                            join sre in dbc.STATUS_REQUEST_EXPENSE on re.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                            join de in DocuExpe on re.ID_REQU_EXPE equals de.ID_REQU_EXPE                           
                            select new
                            {
                                re.ID_DELI_SUST,
                                MONTO = ds.DOC_AMOU,
                                MONTOREMA = (sre.AFF_PETT_CASH_REMA == true) ? de.AMOUNT :0                                         

                            });
                if (data.Count() > 0)
                {
                    var query = (from a in data
                                 group a by new { a.ID_DELI_SUST } into g
                                 select new
                                 {
                                     g.Key.ID_DELI_SUST,
                                     SUMA = g.Sum(x => x.MONTOREMA)
                                 }).Single();
                    return (Convert.ToDecimal(query.SUMA));

                } else
                    {
                      return  0; 
                    }
            }
            catch (Exception)
            {               
                return 0;
            }
        }
        public decimal GetTotalAvailable(int id = 0)
        {
            try
            {
                 var query = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id).ToList()
                                 select new
                                 {
                                     AVAI = ds.DOC_AMOU + ds.AMO_AVAI_PREV - GetAmountAvailable(id),
                                 }).FirstOrDefault();

                    return Convert.ToDecimal(query.AVAI);                
            }
            catch
            {
                return 0;
            }
        }


        public bool CountApproval(int id = 0)
        {
            try
            {
                int ID_TYPE_DELI = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).Single().ID_TYPE_DELI_SUST);

                bool ctd = false;
                var query = dbc.REQUEST_EXPENSE_ACTIVITY.Where(x => x.ID_REQU_EXPE == id && x.ID_STAT_REQU_EXPE == 2);

                var detallequery = query.FirstOrDefault();
                if (query.Count() > 0) { ctd = true; }

                if (ID_TYPE_DELI == 3 || ID_TYPE_DELI == 1)
                {
                    ctd = true;
                }

                return ctd;
            }
            catch
            {
                return false;
            }
        }

        public bool EsAdministrador()
        {
            try
            {
                bool ctd;
                
                if (Session["Cargo"].ToString() == "ADMINISTRADOR DE CONTRATOS" || Session["Cargo"].ToString() == "Administrador de Contratos") { ctd = true; }

                else {
                    ctd = false;
                }

                return ctd;
            }
            catch
            {
                return false;
            }
        }

        public decimal GetTotalRemaining(int id = 0)
        { 
            try
            {
               
                    var query = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id).ToList()
                                 select new
                                 {
                                     REMA = ds.DOC_AMOU+ ds.AMO_AVAI_PREV - GetAmountRemain(id),
                                 }).FirstOrDefault();

                    return Convert.ToDecimal(query.REMA);
                
            }
            catch
            {
                return 0;
            }
        }

        public ActionResult VerDocumento(string id = "",int id_request = 0)
        {
            try
            {
                REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Where(x=> x.ID_REQU_EXPE == id_request).FirstOrDefault();

                if (re.ID_TYPE_DELI_SUST == 3)
                {

                    int ID_STAT_REQU_EXPE = dbc.STATUS_REQUEST_EXPENSE.Where(x => x.NAM_STAT_REQU_EXPE == "Reembolsado").Single().ID_STAT_REQU_EXPE;
                    if (re.ID_STAT_REQU_EXPE.Value == ID_STAT_REQU_EXPE)
                    {
                        re.ID_STAT_REQU_EXPE = 7;
                        dbc.REQUEST_EXPENSE.Attach(re);
                        dbc.Entry(re).State = EntityState.Modified;
                        dbc.SaveChanges();

                        REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                        rea.UserId = Convert.ToInt32(Session["UserId"]);
                        rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                        rea.DAT_STAR = DateTime.Now;
                        rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                        dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                        dbc.SaveChanges();

                        try
                        {
                            SendMail smail = new SendMail();
                            smail.RequestAccountability(re);

                            PERSON_ENTITY_NOTIFICATION penReq = new PERSON_ENTITY_NOTIFICATION();
                            penReq.CREATED = DateTime.Now;
                            penReq.ID_PERS_NOTI = 4;
                            penReq.ID_PERS_ENTI = re.ID_PERS_ENTI_REQU;
                            penReq.UserId = rea.UserId;
                            dbe.PERSON_ENTITY_NOTIFICATION.Add(penReq);
                            dbe.SaveChanges();

                            PERSON_ENTITY_NOTIFICATION penApp = new PERSON_ENTITY_NOTIFICATION();
                            penApp.CREATED = DateTime.Now;
                            penApp.ID_PERS_NOTI = 4;
                            penApp.ID_PERS_ENTI = re.ID_PERS_ENTI_APPR;
                            penApp.UserId = rea.UserId;
                            dbe.PERSON_ENTITY_NOTIFICATION.Add(penApp);
                            dbe.SaveChanges();

                            PERSON_ENTITY_NOTIFICATION penAss = new PERSON_ENTITY_NOTIFICATION();
                            penAss.CREATED = DateTime.Now;
                            penAss.ID_PERS_NOTI = 4;
                            penAss.ID_PERS_ENTI = re.ID_PERS_ENTI_ASSI;
                            penAss.UserId = rea.UserId;
                            dbe.PERSON_ENTITY_NOTIFICATION.Add(penAss);
                            dbe.SaveChanges();
                        }
                        catch (Exception)
                        {
                        }
                    }

                }


                ViewBag.Documento = id;
                
                return View();
            }
            catch
            {
                return Content("Please Close Session");
            }

        }

        public FileResult DescargarArchivo(string id = "")
        {
            try
            {
                FileStream fileStream = System.IO.File.OpenRead(Server.MapPath("~/Adjuntos/Depositos/" + id));
                MemoryStream storeStream = new MemoryStream();

                storeStream.SetLength(fileStream.Length);
                fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                byte[] byteArray = storeStream.ToArray();

                storeStream.Flush();
                fileStream.Close();
                storeStream.Close();

                Random r = new Random();
                int aleatorio = r.Next(10000, 99999);

                Response.Clear();

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                Response.BinaryWrite(byteArray);
                Response.End();
                return File(storeStream.GetBuffer(), "xml");
            }
            catch
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));
                return File(stream, "text/plain", "Error.txt");
            }
        }

    }
}
