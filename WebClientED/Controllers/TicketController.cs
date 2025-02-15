using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClientED.Models;
using System.Globalization;
using System.Threading;
using WebClient.Plugins.Ticket;
using WebClientED.Plugins.Encription;
using System.Net;
using System.Data.Entity;

namespace WebClient.Controllers
{
    public class TicketController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        AppLogEntities dbl = new AppLogEntities();
        public TicketIR tir = new TicketIR();

        private AESRijndael seg = new AESRijndael();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        
        // GET: Ticket
        public ActionResult Index(int id = 0)
        {
            string idt = Convert.ToString(Request.Params["idt"].ToString());
            ViewData["id"] = idt;
            try
            {
                //var ID_TICK = Convert.ToInt32(seg.Decrypt(idt));
                var ID_TICK = Convert.ToInt32(idt);
                //var query = dbc.TICKETs.Single(x=>x.ID_TICK == ID_TICK);

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                //string idst = ;

                string ruta = Server.MapPath("~/Images/");

                if (System.IO.File.Exists(ruta + "\\logo-s-" + Convert.ToString(ticket.ID_ACCO.Value) + ".png"))
                {
                    ViewBag.LogoLayout = "logo-s-" + Convert.ToString(ticket.ID_ACCO.Value) + ".png";
                }
                else
                {
                    ViewBag.LogoLayout = "logo-s-4.png";
                }
            }
            catch
            {

            }

            return View();
        }

        public ActionResult DatosRespuesta(int id = 0)
        {
            textInfo = cultureInfo.TextInfo;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var idt = dbc.TICKETs.Where(p => p.ID_TICK == id);
            var result = (from tt in idt.ToList()
                         join ep in dbe.PERSON_ENTITY on tt.ID_PERS_ENTI equals ep.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on ep.ID_ENTI2 equals ce.ID_ENTI
                        select new
                        {
                          ID_TICK = tt.ID_TICK,
                          COD_TICK = tt.COD_TICK,
                          User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower()))
                          //ID_PERS_ENTI = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower()))
                        });

            return Json(new { Data = result}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RespuestaNegativa(DETAILS_TICKET details_ticket,int id = 0, int d =0)
        {

            ViewData["id"] = id;

            var dt = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).First();
            var dtc = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).Count();
            var dt1 = (dynamic)null;

            if (dt.ID_STAT == 4 && dtc == 1)
            {
                details_ticket = new DETAILS_TICKET();
                details_ticket.ID_TICK = dt.ID_TICK;
                details_ticket.ID_STAT = 1;
                details_ticket.ID_CLIE = dt.ID_CLIE;
                details_ticket.ID_QUEU = dt.ID_QUEU;
                details_ticket.ID_TICK_PARENT = dt.ID_TICK_PARENT;
                details_ticket.ID_TYPE_DETA_TICK = dt.ID_TYPE_DETA_TICK;
                details_ticket.COM_DETA_TICK = "Usuario: Aun hay temas pendiendes por regularizar";
                details_ticket.UserId = 34; //Usuario administrador
                details_ticket.CREATE_DETA_INCI = DateTime.Now;
                details_ticket.ID_PERS_ENTI = dt.ID_PERS_ENTI;
                details_ticket.IND = dt.IND;
                details_ticket.ID_STAT_SALE_OPP = dt.ID_STAT_SALE_OPP;
                details_ticket.AMM_SALE_OPP = dt.AMM_SALE_OPP;
                details_ticket.PRO_SALE_OPP = dt.PRO_SALE_OPP;
                details_ticket.FROM_TIME = DateTime.Now;
                details_ticket.TO_TIME = DateTime.Now;
                details_ticket.SEND_MAIL = false;
                details_ticket.GET_DATE = dt.GET_DATE;
                details_ticket.ID_ENTI_CREATED = dt.ID_ENTI_CREATED;
                details_ticket.ID_ENTI_TRAN = dt.ID_ENTI_TRAN;
                details_ticket.ID_REAS_SCHE = dt.ID_REAS_SCHE;
                details_ticket.FEC_END_REAL = dt.FEC_END_REAL;
                details_ticket.EsperaPorCliente = false;
                details_ticket.Causa = false;

                dbc.DETAILS_TICKET.Add(details_ticket);
                var tk = dbc.TICKETs.Where(p => p.ID_TICK == id).First();
                tk.ID_STAT_END = 1;
                dbc.SaveChanges();
            }
            else
            {
                dt1 = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).Skip(1).Take(1).First();
                if (dt.ID_STAT == 4 && dt1.ID_STAT == 4 || dt.ID_STAT == 1 && dt1.ID_STAT == 4)
                {
                    return RedirectToAction("TicketRespondido" + "/" + id, "Ticket");
                }
                else
                {
                    details_ticket = new DETAILS_TICKET();
                    details_ticket.ID_TICK = dt.ID_TICK;
                    details_ticket.ID_STAT = 1;
                    details_ticket.ID_CLIE = dt.ID_CLIE;
                    details_ticket.ID_QUEU = dt.ID_QUEU;
                    details_ticket.ID_TICK_PARENT = dt.ID_TICK_PARENT;
                    details_ticket.ID_TYPE_DETA_TICK = dt.ID_TYPE_DETA_TICK;
                    details_ticket.COM_DETA_TICK = "Usuario: Aun hay temas pendiendes por regularizar";
                    details_ticket.UserId = 34;//Usuario Administrador
                    details_ticket.CREATE_DETA_INCI = DateTime.Now;
                    details_ticket.ID_PERS_ENTI = dt.ID_PERS_ENTI;
                    details_ticket.IND = dt.IND;
                    details_ticket.ID_STAT_SALE_OPP = dt.ID_STAT_SALE_OPP;
                    details_ticket.AMM_SALE_OPP = dt.AMM_SALE_OPP;
                    details_ticket.PRO_SALE_OPP = dt.PRO_SALE_OPP;
                    details_ticket.FROM_TIME = DateTime.Now;
                    details_ticket.TO_TIME = DateTime.Now;
                    details_ticket.SEND_MAIL = false;
                    details_ticket.GET_DATE = dt.GET_DATE;
                    details_ticket.ID_ENTI_CREATED = dt.ID_ENTI_CREATED;
                    details_ticket.ID_ENTI_TRAN = dt.ID_ENTI_TRAN;
                    details_ticket.ID_REAS_SCHE = dt.ID_REAS_SCHE;
                    details_ticket.FEC_END_REAL = dt.FEC_END_REAL;
                    details_ticket.EsperaPorCliente = false;
                    details_ticket.Causa = false;

                    dbc.DETAILS_TICKET.Add(details_ticket);
                    var tk = dbc.TICKETs.Where(p => p.ID_TICK == id).First();
                    tk.ID_STAT_END = 1;
                    dbc.SaveChanges();
                }
                return View();
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RespuestaNegativa(DETAILS_TICKET details_ticket, int id = 0)
        {
            ViewData["id"] = id;
            var dt = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).First();

            dt.COM_DETA_TICK = details_ticket.COM_DETA_TICK;

            dbc.SaveChanges();

            return RedirectToAction("GraciasPorResponder" + "/" + id, "Ticket");
        }
        
        [HttpGet]
        public ActionResult RespuestaPositiva(DETAILS_TICKET details_ticket,int id = 0, int d = 0)
        {
            ViewData["id"] = id;
            var dt = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).First();
            var dtc = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).Count();
            var dt1 = (dynamic)null;

            if(dt.ID_STAT == 4 && dtc == 1)
            {
                details_ticket = new DETAILS_TICKET();
                details_ticket.ID_TICK = dt.ID_TICK;
                details_ticket.ID_STAT = dt.ID_STAT;
                details_ticket.ID_CLIE = dt.ID_CLIE;
                details_ticket.ID_QUEU = dt.ID_QUEU;
                details_ticket.ID_TICK_PARENT = dt.ID_TICK_PARENT;
                details_ticket.ID_TYPE_DETA_TICK = dt.ID_TYPE_DETA_TICK;
                details_ticket.COM_DETA_TICK = "Usuario: No hay recomendacion de mejora";
                details_ticket.UserId = 34; //Usuario Administrador
                details_ticket.CREATE_DETA_INCI = DateTime.Now;
                details_ticket.ID_PERS_ENTI = dt.ID_PERS_ENTI;
                details_ticket.IND = dt.IND;
                details_ticket.ID_STAT_SALE_OPP = dt.ID_STAT_SALE_OPP;
                details_ticket.AMM_SALE_OPP = dt.AMM_SALE_OPP;
                details_ticket.PRO_SALE_OPP = dt.PRO_SALE_OPP;
                details_ticket.FROM_TIME = DateTime.Now;
                details_ticket.TO_TIME = DateTime.Now;
                details_ticket.SEND_MAIL = false;
                details_ticket.ID_ENTI_CREATED = dt.ID_ENTI_CREATED;
                details_ticket.ID_ENTI_TRAN = dt.ID_ENTI_TRAN;
                details_ticket.EsperaPorCliente = false;
                details_ticket.Causa = false;

                dbc.DETAILS_TICKET.Add(details_ticket);
                dbc.SaveChanges();
            }
              
            else
            {
                dt1 = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).Skip(1).Take(1).First();
                if (dt.ID_STAT == 4 && dt1.ID_STAT == 4 || dt.ID_STAT == 1 && dt1.ID_STAT == 4)
                {
                    return RedirectToAction("TicketRespondido" + "/" + id, "Ticket");
                }
                else
                {
                    details_ticket = new DETAILS_TICKET();
                    details_ticket.ID_TICK = dt.ID_TICK;
                    details_ticket.ID_STAT = dt.ID_STAT;
                    details_ticket.ID_CLIE = dt.ID_CLIE;
                    details_ticket.ID_QUEU = dt.ID_QUEU;
                    details_ticket.ID_TICK_PARENT = dt.ID_TICK_PARENT;
                    details_ticket.ID_TYPE_DETA_TICK = dt.ID_TYPE_DETA_TICK;
                    details_ticket.COM_DETA_TICK = "No hay recomendacion de mejora";
                    details_ticket.UserId = 34; //Usuario Administrador
                    details_ticket.CREATE_DETA_INCI = DateTime.Now;
                    details_ticket.ID_PERS_ENTI = dt.ID_PERS_ENTI;
                    details_ticket.IND = dt.IND;
                    details_ticket.ID_STAT_SALE_OPP = dt.ID_STAT_SALE_OPP;
                    details_ticket.AMM_SALE_OPP = dt.AMM_SALE_OPP;
                    details_ticket.PRO_SALE_OPP = dt.PRO_SALE_OPP;
                    details_ticket.FROM_TIME = DateTime.Now;
                    details_ticket.TO_TIME = DateTime.Now;
                    details_ticket.SEND_MAIL = false;
                    details_ticket.ID_ENTI_CREATED = dt.ID_ENTI_CREATED;
                    details_ticket.ID_ENTI_TRAN = dt.ID_ENTI_TRAN;
                    details_ticket.EsperaPorCliente = false;
                    details_ticket.Causa = false;

                    dbc.DETAILS_TICKET.Add(details_ticket);
                    dbc.SaveChanges();
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RespuestaPositiva(DETAILS_TICKET details_ticket, int id = 0)
        {
            ViewData["id"] = id;
            var dt = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).First();

            dt.COM_DETA_TICK = details_ticket.COM_DETA_TICK;

            dbc.SaveChanges();

            return RedirectToAction("GraciasPorResponder" + "/" + id, "Ticket");
        }

        public ActionResult TicketRespondido(DETAILS_TICKET details_ticket, int id = 0)
        {
            ViewData["id"] = id;
            var dt = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).First();

            return View();
        }

        public ActionResult GraciasPorResponder(DETAILS_TICKET details_ticket, int id = 0)
        {
            ViewData["id"] = id;
            var dt = dbc.DETAILS_TICKET.Where(p => p.ID_TICK == id).OrderByDescending(o => o.ID_DETA_TICK).First();

            return View();
        }

        public ActionResult Unique(int id = 0)
        {
            try
            {
                string idt = Convert.ToString(Request.Params["idt"].ToString());
                //id = Convert.ToInt32(seg.Decrypt(idt.Replace(" ","+")));
                id = Convert.ToInt32(idt.Replace(" ", "+"));

                textInfo = cultureInfo.TextInfo;

                var query = dbc.TICKETs.Where(x => x.ID_TICK == id);
                var ticket = query.First();

                var listuser = (from lu in dbe.ACCOUNT_ENTITY
                                join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                where lu.ID_ACCO == ticket.ID_ACCO//&& ce.UserId != null
                                select new
                                {
                                    lu.ID_PERS_ENTI,
                                    Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                    ce.UserId,
                                    pe.ID_FOTO,
                                }).ToList();

                var listCIA = (from pe in dbe.PERSON_ENTITY
                               join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                               select new
                               {
                                   pe.ID_PERS_ENTI,
                                   CIA = ce.COM_NAME,
                               }).ToList();

                var result = (from t in query.ToList()
                              join tt in dbc.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                              join ts in dbc.STATUS on t.ID_STAT_END equals ts.ID_STAT
                              join tp in dbc.PRIORITies on t.ID_PRIO equals tp.ID_PRIO
                              join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                              join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                              join scat in dbc.CATEGORies on cl.ID_CATE_PARE equals scat.ID_CATE
                              join cat in dbc.CATEGORies on scat.ID_CATE_PARE equals cat.ID_CATE
                              join tso   in dbc.SOURCEs on t.ID_SOUR equals tso.ID_SOUR
                              join cb in listuser on t.UserId equals cb.UserId
                              join at in listuser on t.ID_PERS_ENTI_ASSI equals at.ID_PERS_ENTI
                              join cia in listCIA on t.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                              select new {
                                  AEU = textInfo.ToTitleCase(ReturnRequ(t.ID_PERS_ENTI_END.Value)),
                                  NAM_TYPE_TICK = textInfo.ToTitleCase(tt.NAM_TYPE_TICK.ToLower()),
                                  COD_TICK = t.COD_TICK,
                                  NAM_STAT = textInfo.ToTitleCase(ts.NAM_STAT.ToLower()),
                                  NAM_PRIO = textInfo.ToTitleCase(tp.NAM_PRIO.ToLower()),
                                  SLA = (tp.HOU_PRIO == 0 ? "Planning" : Convert.ToString(tp.HOU_PRIO)+" Hours"),
                                  EXP_TIME = tir.ExpirationTime(t.ID_TICK,tp.HOU_PRIO.Value),
                                  NAM_CATE = textInfo.ToTitleCase(cat.NAM_CATE.ToLower()),
                                  NAM_SUB_CATE = textInfo.ToTitleCase(scat.NAM_CATE.ToLower()),
                                  NAM_CLAS =textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SUB_CLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_SOUR = textInfo.ToTitleCase(tso.NAM_SOUR.ToLower()),
                                  CREATED = String.Format("{0:d}", t.CREATE_TICK) + " " + String.Format("{0:HH:mm}", t.CREATE_TICK),
                                  MODIFIED = String.Format("{0:d}", t.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", t.MODIFIED_TICK),
                                  ATTA = TotAtta(Convert.ToInt32(t.ID_TICK)),
                                  RDP = (t.REM_CTRL_TICK == true ? "Yes" : "No"),
                                  SUM_TICK = t.SUM_TICK,
                                  PHOTO = cb.ID_FOTO,
                                  CREA_BY = textInfo.ToTitleCase(cb.Client.ToLower()),
                                  ASSI_TO = textInfo.ToTitleCase(at.Client.ToLower()),
                                  CIA = textInfo.ToTitleCase(cia.CIA.ToLower()),
                              });

               

                var querydt = dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id).ToList();

                var resultdt = (from di in querydt.OrderByDescending(di => di.ID_DETA_TICK)
                              join tdt in dbc.TYPE_DETAILS_TICKET on di.ID_TYPE_DETA_TICK equals tdt.ID_TYPE_DETA_TICK
                              join p in listuser on di.UserId equals p.UserId

                              join s in dbc.STATUS on di.ID_STAT equals s.ID_STAT into ls
                              from x in ls.DefaultIfEmpty()

                              join q in dbc.QUEUEs on di.ID_QUEU equals q.ID_QUEU into lq
                              from y in lq.DefaultIfEmpty()

                              join a in listuser on di.ID_PERS_ENTI equals a.ID_PERS_ENTI into la
                              from z in la.DefaultIfEmpty()

                              //join sso in dbc.STATUS_SALES_OPPORTUNITY on di.ID_STAT_SALE_OPP equals sso.ID_STAT_SALE_OPPO into lsso
                              //from w in lsso.DefaultIfEmpty()

                              //join t in db.TICKETs on di.ID_TICK_PARENT equals t.ID_TICK into lt
                              //from k in lt.DefaultIfEmpty()

                              select new
                              {
                                  PERS = textInfo.ToTitleCase(p.Client.ToLower()),
                                  COM_DETA_INCI = di.COM_DETA_TICK,
                                  ID_TYPE_DETA_TICK = di.ID_TYPE_DETA_TICK,
                                  NAM_TYPE_DETA_TICK = textInfo.ToTitleCase(tdt.NAM_TYPE_DETA_TICK.ToLower()),
                                  NAM_STAT = (x == null ? String.Empty : " | "+textInfo.ToTitleCase(x.NAM_STAT.ToLower())),
                                  NAM_QUEU = (y == null ? String.Empty : y.NAM_QUEU),
                                  //COD_PARENT = (k == null ? String.Empty : k.COD_TICK),
                                  ASSI = (z == null ? String.Empty : " | " +  textInfo.ToTitleCase(z.Client.ToLower())),
                                  CREATE_DETA_INCI = String.Format("{0:d}", di.CREATE_DETA_INCI),
                                  ADJ = 1,//AttaDetaTick(di.ID_DETA_TICK),
                                  FEC_SCHE = String.Format("{0:d}", di.FEC_SCHE),
                                  ID_PHOT = "1",//p.ID_CLIE,
                                  PHOTO = p.ID_FOTO,
                                  //NAM_STAT_SALE_OPPO = (w == null ? String.Empty : w.NAM_STAT_SALE_OPPO),
                                  //AMM_SALE_OPP = string.Format("{0:0,0.00}", di.AMM_SALE_OPP == null ? 0 : di.AMM_SALE_OPP),
                                  //PRO_SALE_OPP = string.Format("{0:0,0.00}", di.PRO_SALE_OPP == null ? 0 : di.PRO_SALE_OPP),
                                  INEX = di.ID_DETA_TICK

                              }).ToList();

                return Json(new {Data=result,Details=resultdt }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = ex.Message;
                exc.DES_EXCE = "WebClientED - TicketController - Unique";
                dbl.EXCEPTION.Add(exc);
                dbl.SaveChanges();
                return Content("ERROR");
            }
            
        }

        public int TotAtta(int id)
        {
            int atta_tick = dbc.ATTACHEDs.Where(a => a.ID_INCI == id).Count();

            int atta_all = (from x in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id)
                            join adt in dbc.ATTACHEDs on x.ID_DETA_TICK equals adt.ID_DETA_INCI
                            select new
                            {
                                adt.ID_ATTA
                            }).Count();

            return atta_tick + atta_all;
        }

        public string ReturnRequ(int idend = 0)
        {
            var listClient = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == idend)
                              join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                              select new
                              {
                                  Client = ce.FIR_NAME + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME),
                              });

            return listClient.First().Client.ToLower();
        }
    }
}