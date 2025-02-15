using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using ERPElectrodata.Objects;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Web.Security;

namespace ERPElectrodata.Controllers
{
    public class SalesController : Controller
    {

        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();
        
        // GET: /Sales/
        [Authorize]
        public ActionResult Index()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int sw = 0;

            //Todos los DOCUMENT_SALE que tiene ticket 
            var query = (from a in dbc.DOCUMENT_SALE
                         join b in dbc.TICKETs on a.ID_DOCU_SALE equals b.ID_DOCU_SALE into lb
                         from xb in lb.DefaultIfEmpty()
                         select new
                         {
                             a.ID_STAT_DOCU_SALE,
                             xb.ID_PERS_ENTI_ASSI,
                         });

            var qDS = dbc.DOCUMENT_SALE;

            var qTK = dbc.TICKETs.Where(x => x.ID_DOCU_SALE != null)
                        .Join(dbc.DOCUMENT_SALE, x => x.ID_DOCU_SALE, ds => ds.ID_DOCU_SALE, (x, ds) => new
                        {
                            x.ID_PERS_ENTI_ASSI,
                            ds.ID_STAT_DOCU_SALE,
                        });

            ViewBag.POpen = 0;
            ViewBag.TTOpen = 0;
            //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            //foreach (string rol in rolesArray)
            //{
            //    if (rol == "ASSIGNED OPs" || rol == "ADMINISTRADOR")
            //    {
            //        sw = 1;
            //    }
            //}

            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_PMO"]  == 1)
            {
                sw = 1;
            }

            if (sw == 1)
            {
                ViewBag.POpen = qDS.Where(x => x.ID_STAT_DOCU_SALE == 1).Count();
                ViewBag.PAssigned = qDS.Where(x => x.ID_STAT_DOCU_SALE == 2).Count();
                ViewBag.PResolved = qDS.Where(x => x.ID_STAT_DOCU_SALE == 3).Count();
                ViewBag.PClosed = qDS.Where(x => x.ID_STAT_DOCU_SALE == 4).Count();

                ViewBag.TTOpen = ViewBag.POpen;
                ViewBag.TTAssigned = ViewBag.PAssigned;
                ViewBag.TTResolved = ViewBag.PResolved;
                ViewBag.TTClosed = ViewBag.PClosed;
            }
            else
            {
                var listIPE = dbe.LIST_STAFF_BY_BOSS(ID_PERS_ENTI).ToList();

                var filTK = (from a in qTK.ToList()
                             where (from b in listIPE select b.ID_PERS_ENTI).Contains(Convert.ToInt32(a.ID_PERS_ENTI_ASSI))
                             select new
                             {
                                 a.ID_PERS_ENTI_ASSI,
                                 a.ID_STAT_DOCU_SALE,
                             });
                ViewBag.PAssigned = filTK.Where(x => x.ID_STAT_DOCU_SALE == 2).Count();
                ViewBag.PResolved = filTK.Where(x => x.ID_STAT_DOCU_SALE == 3).Count();
                ViewBag.PClosed = filTK.Where(x => x.ID_STAT_DOCU_SALE == 4).Count();

                ViewBag.TTAssigned = qDS.Where(x => x.ID_STAT_DOCU_SALE == 2).Count();
                ViewBag.TTResolved = qDS.Where(x => x.ID_STAT_DOCU_SALE == 3).Count();
                ViewBag.TTClosed = qDS.Where(x => x.ID_STAT_DOCU_SALE == 4).Count();
            }

            return View();
        }

        public ActionResult List()
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());

            var qCIA = dbe.PERSON_ENTITY
                        .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                        {
                            x.ID_PERS_ENTI,
                            x.ID_ENTI2,
                            x.ID_ENTI1,
                            ClientContact = (ce.FIR_NAME == null ? "" : ce.FIR_NAME) + " " + ce.SEC_NAME  + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME) + " " + ce.MOT_NAME,
                            ClientEmail = (x.EMA_PERS == null ? "-" : x.EMA_PERS),
                            ClientJobTitle = (x.CAR_PERS == null ? "-" : x.CAR_PERS).ToLower(),
                            ClientTel = (x.TEL_PERS == null ? "-" : x.TEL_PERS),
                            ClientAnexo = (x.EXT_PERS == null ? "-" : x.EXT_PERS),
                        })
                        .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI1, ce => ce.ID_ENTI, (x, ce) => new
                        {
                            ID_CTTS = x.ID_ENTI2,
                            ID_COMP = x.ID_ENTI1,
                            x.ID_PERS_ENTI,
                            x.ClientContact,
                            x.ClientEmail,
                            x.ClientJobTitle,
                            x.ClientTel,
                            x.ClientAnexo,
                            CIA = ce.COM_NAME,
                            CIA_ADDR = (ce.ADDRESS == null ? "-" : ce.ADDRESS),
                            CIA_TELE = (ce.TEL_ENTI == null ? "-" : ce.TEL_ENTI),
                            CIA_RUC = (ce.NUM_TYPE_DI == null ? "" : ce.NUM_TYPE_DI),
                        });


            if (ID_STAT == 0){ ID_STAT = 1; }

            int tt = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT).Count();

            var qCIA1 = dbe.CLASS_ENTITY.Where(x=>x.COM_NAME != null);

            int sw = 0;
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_PMO"] == 1)
            {
                sw = 1;
            }

            if (sw == 1)
            {   //Tiene el rol y puede ver todas las OPs
                var query = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT)
                                .OrderByDescending(x => x.DAT_DOCU_SALE).Skip(skip).Take(take).ToList()
                             join tds in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
                             join b in qCIA1 on a.ID_COMP equals b.ID_ENTI
                             join c in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals c.ID_STAT_DOCU_SALE
                             join d in qCIA on a.ID_PERS_ENTI_VEND equals d.ID_PERS_ENTI
                             join e in qCIA on a.ID_CTTS equals e.ID_CTTS
                             join f in dbc.KNOWLEDGEs on a.ID_DOCU_SALE equals f.ID_DOCU_SALE into lf
                             from xf in lf.DefaultIfEmpty()
                             join g in qCIA1 on a.ID_ENTI equals g.ID_ENTI
                             select new
                             {
                                 a.ID_DOCU_SALE,
                                 OS = (a.OS == null ? "" : a.OS),
                                 EXP_DATE = a.EXP_DATE == null ? "-" : String.Format("{0:d}", a.EXP_DATE),
                                 CIA = b.COM_NAME.ToLower(),
                                 CIA_ADDR = (b.ADDRESS == null ? "-" : b.ADDRESS.ToLower()),
                                 CIA_TELE = (b.TEL_ENTI == null ? "-" : b.TEL_ENTI),
                                 CIA_RUC = (b.NUM_TYPE_DI == null ? "-" : b.NUM_TYPE_DI),
                                 NAM_CLIE_CONT = e.ClientContact.ToLower(),
                                 CLI_EMAI = e.ClientEmail,
                                 CLI_JOB_TITL = e.ClientJobTitle,
                                 CLI_TELE = e.ClientTel,
                                 CLI_PHON_EXTE = e.ClientAnexo,
                                 tds.COD_TYPE_DOCU_SALE,
                                 a.NUM_DOCU_SALE,
                                 DAT_DOCU_SALE = a.DAT_DOCU_SALE == null ? "-" : String.Format("{0:d}", a.DAT_DOCU_SALE),
                                 a.TIPO_CAMB,
                                 a.SUM_DOCU_SALE,
                                 NAM_STAT_DOCU_SALE = c.NAM_STAT_DOCU_SALE.ToLower(),
                                 VENDOR = d.ClientContact.ToLower(),
                                 a.ID_STAT_DOCU_SALE,
                                 NAM_FILE = (xf == null ? "" : (xf.NAM_ATTA == null ? "-" : xf.NAM_ATTA.ToLower())),
                                 NAM_ATTA = (xf == null ? "" : (xf.NAM_ATTA == null ? "-" : xf.NAM_ATTA.ToLower() + "_" + xf.ID_KNOW.ToString() + ".pdf")),
                                 DATE_ATTA = (xf == null ? "" : (xf.DATE_ATTA == null ? "-" : String.Format("{0:d}", xf.DATE_ATTA))),
                                 COM_NAME = g.COM_NAME.ToLower(),
                             }).OrderByDescending(x => x.NUM_DOCU_SALE);

                return Json(new { Data = query, Count = tt }, JsonRequestBehavior.AllowGet);
            }
            else {
                if (ID_STAT == 1) //no tiene el rol por lo tanto no puede ver las OPs Open
                {
                    var query = (from a in dbc.DOCUMENT_SALE.Where(x=>x.ID_DOCU_SALE == 0) select a);
                    return Json(new { Data = query, Count = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {//no tiene el rol pero puede ver las asignadas a el y a su personal
                    int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    var listIPE = dbe.LIST_STAFF_BY_BOSS(ID_PERS_ENTI).ToList();

                    var qry = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT).ToList()
                               join t in dbc.TICKETs on a.ID_DOCU_SALE equals t.ID_DOCU_SALE
                               where (from lb in listIPE select lb.ID_PERS_ENTI).Contains(Convert.ToInt32(t.ID_PERS_ENTI_ASSI))
                               select a);

                    tt = qry.Count();

                    var query = (from a in qry.Skip(skip).Take(take).ToList()
                                 join tds in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
                                 join b in qCIA1 on a.ID_COMP equals b.ID_ENTI
                                 join c in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals c.ID_STAT_DOCU_SALE
                                 join d in qCIA on a.ID_PERS_ENTI_VEND equals d.ID_PERS_ENTI
                                 join e in qCIA on a.ID_CTTS equals e.ID_CTTS
                                 join f in dbc.KNOWLEDGEs on a.ID_DOCU_SALE equals f.ID_DOCU_SALE into lf
                                 from xf in lf.DefaultIfEmpty()
                                 join g in qCIA1 on a.ID_ENTI equals g.ID_ENTI
                                 select new
                                 {
                                     a.ID_DOCU_SALE,
                                     OS = (a.OS == null ? "" : a.OS),
                                     EXP_DATE = a.EXP_DATE == null ? "-" : String.Format("{0:d}", a.EXP_DATE),
                                     CIA = b.COM_NAME.ToLower(),
                                     CIA_ADDR = (b.ADDRESS == null ? "-" : b.ADDRESS.ToLower()),
                                     CIA_TELE = (b.TEL_ENTI == null ? "-" : b.TEL_ENTI),
                                     CIA_RUC = (b.NUM_TYPE_DI == null ? "-" : b.NUM_TYPE_DI),
                                     NAM_CLIE_CONT = e.ClientContact.ToLower(),
                                     CLI_EMAI = e.ClientEmail,
                                     CLI_JOB_TITL = e.ClientJobTitle,
                                     CLI_TELE = e.ClientTel,
                                     CLI_PHON_EXTE = e.ClientAnexo,
                                     tds.COD_TYPE_DOCU_SALE,
                                     a.NUM_DOCU_SALE,
                                     DAT_DOCU_SALE = a.DAT_DOCU_SALE == null ? "-" : String.Format("{0:d}", a.DAT_DOCU_SALE),
                                     a.TIPO_CAMB,
                                     a.SUM_DOCU_SALE,
                                     NAM_STAT_DOCU_SALE = c.NAM_STAT_DOCU_SALE.ToLower(),
                                     VENDOR = d.ClientContact.ToLower(),
                                     a.ID_STAT_DOCU_SALE,
                                     NAM_FILE = (xf == null ? "" : (xf.NAM_ATTA == null ? "-" : xf.NAM_ATTA.ToLower())),
                                     NAM_ATTA = (xf == null ? "" : (xf.NAM_ATTA == null ? "-" : xf.NAM_ATTA.ToLower() + "_" + xf.ID_KNOW.ToString() + ".pdf")),
                                     DATE_ATTA = (xf == null ? "" : (xf.DATE_ATTA == null ? "-" : String.Format("{0:d}", xf.DATE_ATTA))),
                                     COM_NAME = g.COM_NAME.ToLower(),
                                 }).OrderByDescending(x => x.NUM_DOCU_SALE);

                    return Json(new { Data = query, Count = tt }, JsonRequestBehavior.AllowGet);                  
                }          
            }
        }

        public ActionResult ViewDetailArticleService(int id = 0,string proc = "")
        {
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.Procedencia = proc;
            return View();
        }

        public ActionResult ListDetailsOP(int id = 0)
        {
            var query = (from a in dbc.DETAIL_DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).ToList()
                         join b in dbc.DOCUMENT_SALE on a.ID_DOCU_SALE equals b.ID_DOCU_SALE
                         join c in dbc.ITEM_CATEGORY on a.ID_ITEM_CATE equals c.ID_ITEM_CATE into lc
                         from xc in lc.DefaultIfEmpty()
                         select new
                         {
                             a.ID_DETA_DOCU_SALE,
                             a.ID_ARTI,
                             a.QUANTITY,
                             a.CODE,
                             b.ID_DOCU_SALE,
                             b.ID_STAT_DOCU_SALE,
                             DESC_DETA = (xc == null ? "" : xc.NAM_ITEM_CATE + " - ") + a.DESC_DETA +
                                         (a.CODE_ARTI == null ? "" : "<br />" + a.CODE_ARTI) +
                                         (a.SERIE == null ? "" : "<br /><span style='font-size:.9em;font-family:Arial;color:#00528e;'>Serie: " + a.SERIE + "</span>") + 
                                         (a.DAT_INI_SUPP != null ? "<br /><span style='font-size:.9em;font-family:Arial;color:#00528e;'>Warranty from " + String.Format("{0:d}", a.DAT_INI_SUPP) + " to " + String.Format("{0:d}", a.DAT_END_SUPP) + "</span>" :
                                         a.HOURS != null ? "<br /><span style='font-size:.9em;font-family:Arial;color:#00528e;'>Warranty: " + String.Format("{0:N0}", a.HOURS) + " Hours</span>" : ""),
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewSale() {
            return View();
        }

        public ActionResult ListStatusDocuSale() {
            var query = (from a in dbc.STATUS_DOCUMENT_SALE
                         select a).OrderBy(x => x.NAM_STAT_DOCU_SALE);

            return Json(new { Data = query, Count = query.Count() },JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListClients() {
            var query = (from a in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null && x.VIG_ENTI == true)
                         select new
                         {
                             a.COM_NAME,
                             a.ID_ENTI,
                         }).OrderBy(x=>x.COM_NAME);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarClientes(string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = (from a in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null)
                         select new
                         {
                             id = a.ID_ENTI,
                             text = a.COM_NAME,
                         }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarEstadosPMO(string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = (from a in dbc.STATUS_DOCUMENT_SALE
                          select new
                          {
                              id = a.ID_STAT_DOCU_SALE,
                              text = a.NAM_STAT_DOCU_SALE,
                          }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListContactByID_ENTI1() {

            int ID_ENTI = 0;
            try { ID_ENTI = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]); }
            catch { ID_ENTI = 0; }            

            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_ENTI)
                         join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                         join c in dbe.CARGOes on a.ID_CARG equals c.ID_CARG into lc
                         from xc in lc.DefaultIfEmpty()
                         select new { 
                            a.ID_PERS_ENTI,
                            b.ID_ENTI,
                            NAM_CONTACT = b.FIR_NAME + " " + b.SEC_NAME + " " + b.LAS_NAME + " " + b.MOT_NAME,
                            NAM_CARG = (xc == null ? "-" : xc.NAM_CARG).ToLower(),
                         }).Where(x=> x.NAM_CONTACT!=null && x.NAM_CONTACT!="").OrderBy(x => x.NAM_CONTACT);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListVendor()
        {
            var query1 = dbe.LIST_JOBTITLE_BY_PARENT(5,4).ToList();

            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9).ToList()
                         join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                         join c in dbe.PERSON_CONTRACT.Where(x=>x.VIG_CONT == true && x.LAS_CONT == true)
                                    on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in query1 on c.ID_CHAR equals d.ID_CHAR
                         select new
                         {
                             a.ID_PERS_ENTI,
                             b.ID_ENTI,
                             NAM_VENDOR = b.FIR_NAME + " " + b.SEC_NAME + " " + b.LAS_NAME + " " + b.MOT_NAME
                         }).OrderBy(x => x.NAM_VENDOR);


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAddDetail() { 

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(DOCUMENT_SALE ds)
        {
            DateTime DAT_DOCU_SALE;
            int sw = 0, ID_STAT_DOCU_SALE = 0, ID_PERS_ENTI = 0, ID_PERS_ENTI_VEND = 0, 
                    ID_PERS_ENTI_ASSI = 0, ID_QUEU = 0, ID_ENTI = 0, ID_CATE = 0;

            string S_NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"].ToString()).ToUpper();
            string S_MONEY = Convert.ToString(Request.Params["MONEY"]);

            var dv = new DOCUMENT_SALE();
            var ticket = new TICKET();

            if (S_NUM_DOCU_SALE.Trim().Length == 0) { sw = 1; }
            else if (S_MONEY.Trim().Length == 0) { sw = 1; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_DOCU_SALE"]), out DAT_DOCU_SALE) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_STAT_DOCU_SALE"]), out ID_STAT_DOCU_SALE) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_ENTI"]), out ID_ENTI) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_ENTI"]), out ID_PERS_ENTI) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_ENTI_VEND"]), out ID_PERS_ENTI_VEND) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_QUEU"]), out ID_QUEU) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_CATE"]), out ID_CATE) == false) { sw = 1; }
            else if (Convert.ToInt32(Request.Form["ctdHD"]) <= 0) { sw = 1; }//Validando que tenga detalle
            else {
                dv.DAT_DOCU_SALE = DAT_DOCU_SALE;
                ticket.ID_QUEU = ID_QUEU;
                ticket.ID_PERS_ENTI_ASSI = ID_PERS_ENTI_ASSI;
            }

            //Validar que el codigo no exista en la BD
            var f_DV = dbc.DOCUMENT_SALE.Where(x => x.NUM_DOCU_SALE == S_NUM_DOCU_SALE);
            if (f_DV.Count() > 0) {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");            
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            try
            {
                dv.NUM_DOCU_SALE = S_NUM_DOCU_SALE;
                dv.ID_TYPE_DOCU_SALE = 1;
                dv.ID_STAT_DOCU_SALE = ID_STAT_DOCU_SALE;
                dv.ID_PERS_ENTI_VEND = ID_PERS_ENTI_VEND;
                dv.MONEY = S_MONEY;
                dv.UserId = Convert.ToInt32(Session["UserId"]);
                dv.DAT_REGISTER = DateTime.Now;               

                dbc.DOCUMENT_SALE.Add(dv);
                dbc.SaveChanges();
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
            }

            int ID_DOCU_SALE = dv.ID_DOCU_SALE;

            string ListIdsHD = Convert.ToString(Request.Params["ListIdsHD"]);
            ListIdsHD = ListIdsHD.Substring(0, ListIdsHD.Length - 1);
            string[] lista = ListIdsHD.Split(',');
            foreach (string id in lista) {

                string codeArt = Convert.ToString(Request.Form["codeArtHD" + id.ToString()]);
                string codePro = Convert.ToString(Request.Form["codeProHD" + id.ToString()]);
                string desc = Convert.ToString(Request.Form["descHD" + id.ToString()]);
                decimal qty = Convert.ToDecimal(Request.Form["qtyHD" + id.ToString()]);
                decimal upr = Convert.ToDecimal(Request.Form["uprHD" + id.ToString()]);
                int idmanu = Convert.ToInt32(Request.Form["idManuHD" + id.ToString()]);
                int idmm = Convert.ToInt32(Request.Form["id_Manu_ModeHD" + id.ToString()]);
                int idcm = Convert.ToInt32(Request.Form["id_Comm_ModeHD" + id.ToString()]);

                try
                {
                    var ddv = new DETAIL_DOCUMENT_SALE();
                    ddv.ID_DOCU_SALE = ID_DOCU_SALE;
                    ddv.CODE_ARTI = codeArt;
                    ddv.CODE = codePro;                    
                    ddv.DESC_DETA = desc;
                    ddv.QUANTITY = qty;
                    ddv.VTA_ME = upr;
                    if (idmanu != 0) { ddv.ID_MANU = idmanu; }
                    if (idmm != 0) { ddv.ID_MANU_MODE = idmm; }
                    if (idcm != 0) { ddv.ID_COMM_MODE = idcm; }

                    dbc.DETAIL_DOCUMENT_SALE.Add(ddv);
                    dbc.SaveChanges();  
                }
                catch
                {
                    DOCUMENT_SALE v_dv = dbc.DOCUMENT_SALE.Find(ID_DOCU_SALE);
                    dbc.DOCUMENT_SALE.Remove(v_dv);
                    dbc.SaveChanges();

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                }
              
            }
            string usuario = Convert.ToString(Session["NAM_PERS"]).ToUpper();
            
            try
            {
                int ID_CHAR = 0;
                try {
                    ID_CHAR = dbe.PERSON_CONTRACT.Single(x => x.LAS_CONT == true && x.ID_PERS_CONT == ticket.ID_PERS_ENTI_ASSI).ID_CHAR.Value;
                }
                catch{ ID_CHAR = 0; }

                ticket.ID_DOCU_SALE = ID_DOCU_SALE;
                ticket.ID_ACCO = 4;
                ticket.ID_TYPE_TICK = 2;
                ticket.ID_PERS_ENTI = ID_PERS_ENTI;
                ticket.ID_PERS_ENTI_END = ID_PERS_ENTI;
                ticket.ID_COMP = ID_ENTI;
                ticket.ID_AREA = ID_CHAR;
                ticket.ID_PRIO = 5;
                ticket.ID_STAT = 1;
                ticket.ID_STAT_END = 1;
                ticket.ID_SOUR = 4;
                ticket.FEC_TICK = dv.DAT_REGISTER;
                ticket.SUM_TICK = "TICKET CREATED BY " + usuario + " TO REGISTER SALE";
                ticket.REM_CTRL_TICK = false;
                ticket.UserId = dv.UserId;
                ticket.CREATE_TICK = DateTime.Now;
                ticket.MODIFIED_TICK = DateTime.Now;
                ticket.IS_PARENT = false;
                ticket.ID_CATE = ID_CATE;
                ticket.DAT_EXPI_TICK = dv.EXP_DATE;

                dbc.TICKETs.Add(ticket);
                dbc.SaveChanges();

                int id = Convert.ToInt32(ticket.ID_TICK);

                //SendMail mail = new SendMail();
                //string xx = mail.NewTicketByNewSale(id);

            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','0');}window.onload=init;</script>");
        }

        public ActionResult FindSale() {
            return View();
        }

        public ActionResult FindOPSidige()
        {
            return View();
        }

        public ActionResult ResultFindOPSidige()
        {
            try
            {
                int ID_ENTI = Convert.ToInt32(Request.Params["ID_ENTI"]);
                int ID_TYPE_DOCU_SALE = Convert.ToInt32(Request.Params["ID_TYPE_DOCU_SALE"]);
                string NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"]);

                ((IObjectContextAdapter)dbc).ObjectContext.CommandTimeout = 600;
                var query = dbc.LIST_OS(ID_ENTI, ID_TYPE_DOCU_SALE, NUM_DOCU_SALE).ToList();
                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                //return Json(new { Data = "", Count = 0 }, JsonRequestBehavior.AllowGet);
                return Content(e.InnerException.Message);
            }
        }

        public ActionResult CreateOPFromSIDIGE()
        {
            return Content("OK");
        }

        public ActionResult ListVendorAll()
        {
            var query1 = dbe.LIST_JOBTITLE_BY_PARENT(5, 4).ToList();

            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9).ToList()
                         join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                         join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true)
                                    on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in query1 on c.ID_CHAR equals d.ID_CHAR
                         select new
                         {
                             a.ID_PERS_ENTI,
                             b.ID_ENTI,
                             NAM_VENDOR = b.FIR_NAME + " " + b.SEC_NAME + " " + b.LAS_NAME + " " + b.MOT_NAME
                         }).OrderBy(x => x.NAM_VENDOR);


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindSaleResult() {
            string S_NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"].ToString()).Trim();
            string S_OS = Convert.ToString(Request.Params["OS"].ToString()).Trim();
            string S_KEYWORD = Convert.ToString(Request.Params["KEYWORD"].ToString()).Trim();

            string S_ID_STAT_DOCU_SALE = null, S_ID_ENTI = null, S_ID_PERS_ENTI = null, S_ID_PERS_ENTI_VEND = null,
                    S_ID_QUEU = null, S_ID_PERS_ENTI_ASSI = null, S_ID_CATE = null, S_ID_TYPE_DOCU_SALE = null, S_ID_COMP_END = null;

            int ID_STAT_DOCU_SALE, ID_TYPE_DOCU_SALE, ID_ENTI, ID_PERS_ENTI, ID_PERS_ENTI_VEND, ID_QUEU, ID_PERS_ENTI_ASSI, ID_CATE, tt = 0, ID_COMP_END;

            try { S_ID_STAT_DOCU_SALE = Convert.ToString(Request.Params["ID_STAT_DOCU_SALE"].ToString()); }
            catch { }

            try { S_ID_TYPE_DOCU_SALE = Convert.ToString(Request.Params["ID_TYPE_DOCU_SALE"].ToString()); }
            catch { }

            try { S_ID_ENTI = Convert.ToString(Request.Params["ID_ENTI"].ToString()); }
            catch { }

            try { S_ID_PERS_ENTI = Convert.ToString(Request.Params["ID_PERS_ENTI"].ToString()); }
            catch { }

            try { S_ID_PERS_ENTI_VEND = Convert.ToString(Request.Params["ID_PERS_ENTI_VEND"].ToString()); }
            catch { }

            try { S_ID_QUEU = Convert.ToString(Request.Params["ID_QUEU_FIND"].ToString()); }
            catch { }

            try { S_ID_PERS_ENTI_ASSI = Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI_FIND"].ToString()); }
            catch { }

            try { S_ID_CATE = Convert.ToString(Request.Params["ID_CATE_FIND"].ToString()); }
            catch { }

            try { S_ID_COMP_END = Convert.ToString(Request.Params["ID_COMP_END"].ToString()); }
            catch { }

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var qClient = (from a in dbe.PERSON_ENTITY
                           join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                           join c in dbe.CLASS_ENTITY on a.ID_ENTI1 equals c.ID_ENTI
                           select new { 
                                a.ID_PERS_ENTI,
                                a.ID_ENTI1,
                                a.ID_ENTI2,
                                CONTACT = b.FIR_NAME + " " + b.SEC_NAME + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME) + " " + b.MOT_NAME,
                                a.EMA_PERS,
                                a.TEL_PERS,
                                a.EXT_PERS,
                                a.CAR_PERS,
                                c.COM_NAME,
                                RUC = (c.NUM_TYPE_DI == null ? "" : c.NUM_TYPE_DI),
                                c.ADDRESS,
                                c.TEL_ENTI,
                           });


            if (Int32.TryParse(S_ID_PERS_ENTI, out ID_PERS_ENTI)){
                qClient = qClient.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI);
            }
            else if (Int32.TryParse(S_ID_ENTI, out ID_ENTI)){
                qClient = qClient.Where(x => x.ID_ENTI1 == ID_ENTI);
            }

            var qDocuSale = (from a in dbc.DOCUMENT_SALE
                             join b in dbc.TICKETs on a.ID_DOCU_SALE equals b.ID_DOCU_SALE into lb
                             from xb in lb.DefaultIfEmpty()
                             select new { 
                                a.ID_DOCU_SALE,
                                a.ID_TYPE_DOCU_SALE,
                                a.ID_STAT_DOCU_SALE,
                                a.NUM_DOCU_SALE,
                                a.DAT_DOCU_SALE,
                                a.SUM_DOCU_SALE,
                                a.ID_PERS_ENTI_VEND,
                                a.UserId,
                                a.DAT_REGISTER,
                                a.ID_COMP,
                                a.ID_COMP_END,
                                a.ID_CTTS,
                                a.OS,
                                a.ID_ENTI,
                                a.EXP_DATE,
                                xb.ID_PERS_ENTI_END,
                                xb.ID_PERS_ENTI_ASSI,
                                xb.ID_QUEU,
                                xb.ID_CATE,
                             });

            if (!String.IsNullOrEmpty(S_NUM_DOCU_SALE))
            {
                qDocuSale = qDocuSale.Where(x => x.NUM_DOCU_SALE.ToUpper().Contains(S_NUM_DOCU_SALE.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_OS))
            {
                qDocuSale = qDocuSale.Where(x => x.OS.ToUpper().Contains(S_OS.ToUpper()));
            }
            if (Int32.TryParse(S_ID_TYPE_DOCU_SALE, out ID_TYPE_DOCU_SALE))
            {
                qDocuSale = qDocuSale.Where(x => x.ID_TYPE_DOCU_SALE == ID_TYPE_DOCU_SALE);
            }
            if (Int32.TryParse(S_ID_STAT_DOCU_SALE, out ID_STAT_DOCU_SALE))
            {
                qDocuSale = qDocuSale.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE);
            }
            if (Int32.TryParse(S_ID_PERS_ENTI, out ID_PERS_ENTI))
            {
                qDocuSale = qDocuSale.Where(x => x.ID_CTTS == ID_PERS_ENTI);
            }
            if (Int32.TryParse(S_ID_COMP_END, out ID_COMP_END))
            {
                qDocuSale = qDocuSale.Where(x => x.ID_COMP_END == ID_COMP_END);
            }
            if (Int32.TryParse(S_ID_PERS_ENTI_VEND, out ID_PERS_ENTI_VEND))
            {
                qDocuSale = qDocuSale.Where(x => x.ID_PERS_ENTI_VEND == ID_PERS_ENTI_VEND);
            }
            if (Int32.TryParse(S_ID_QUEU, out ID_QUEU))
            {
                qDocuSale = qDocuSale.Where(x => x.ID_QUEU == ID_QUEU);
            }
            if (Int32.TryParse(S_ID_PERS_ENTI_ASSI, out ID_PERS_ENTI_ASSI))
            {
                qDocuSale = qDocuSale.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
            }
            if (Int32.TryParse(S_ID_CATE, out ID_CATE))
            {
                qDocuSale = qDocuSale.Where(x => x.ID_CATE == ID_CATE);
            }
            if (!String.IsNullOrEmpty(S_KEYWORD))
            {
                var qDetail = (from a in dbc.DETAIL_DOCUMENT_SALE.Where(x => x.DESC_DETA.ToUpper().Contains(S_KEYWORD.ToUpper()))
                               select new { 
                                    a.ID_DOCU_SALE
                               }).Distinct();

                qDocuSale = (from a in qDocuSale
                             join b in qDetail on a.ID_DOCU_SALE equals b.ID_DOCU_SALE
                             select a);
            }

            var qUnion = (from a in qDocuSale.ToList()
                          join b in qClient on a.ID_CTTS equals b.ID_ENTI2
                         join c in dbe.PERSON_ENTITY on a.ID_PERS_ENTI_VEND equals c.ID_PERS_ENTI
                         join d in dbe.CLASS_ENTITY on c.ID_ENTI2 equals d.ID_ENTI
                         join e in dbe.PERSON_ENTITY on (a.ID_PERS_ENTI_ASSI == null ? 0 : a.ID_PERS_ENTI_ASSI) equals e.ID_PERS_ENTI into le
                         from xe in le.DefaultIfEmpty()
                         join f in dbe.CLASS_ENTITY on (xe == null ? 0 : xe.ID_ENTI2) equals f.ID_ENTI into lf
                         from xf in lf.DefaultIfEmpty()
                         join g in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals g.ID_TYPE_DOCU_SALE
                         join h in dbc.KNOWLEDGEs on a.ID_DOCU_SALE equals h.ID_DOCU_SALE into lh
                         from xh in lh.DefaultIfEmpty()
                         join i in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals i.ID_STAT_DOCU_SALE
                         join j in dbe.CLASS_ENTITY on a.ID_ENTI equals j.ID_ENTI
                         select new
                         {
                             a.ID_DOCU_SALE,
                             COD_TYPE_DOCU_SALE =(g.COD_TYPE_DOCU_SALE == null ? "" : g.COD_TYPE_DOCU_SALE),
                             i.COLOR,
                             NUM_DOCU_SALE = (a.NUM_DOCU_SALE == null ? "" : a.NUM_DOCU_SALE),
                             DAT_DOCU_SALE = (a.DAT_DOCU_SALE == null ? "-" : String.Format("{0:d}", a.DAT_DOCU_SALE)),
                             EXP_DATE = (a.EXP_DATE == null ? "-" : String.Format("{0:d}", a.EXP_DATE)),
                             VENDOR = d.FIR_NAME + " " + d.SEC_NAME + " " + (d.LAS_NAME == null ? "" : d.LAS_NAME) + " " + d.MOT_NAME,
                             a.ID_STAT_DOCU_SALE,
                             a.SUM_DOCU_SALE,
                             NAM_CLIE_CONT = (b == null ? "-" : (b.CONTACT == null ? "-" : b.CONTACT)).ToLower(),
                             CLI_EMAI = (b == null ? "-" : (b.EMA_PERS == null ? "-" : b.EMA_PERS)),
                             CLI_TELE = (b == null ? "-" : (b.TEL_PERS == null ? "-" : b.TEL_PERS)),
                             CLI_PHON_EXTE = (b == null ? "-" : (b.EXT_PERS == null ? "-" : b.EXT_PERS)),
                             CLI_JOB_TITL = (b == null ? "-" : (b.CAR_PERS == null ? "-" : b.CAR_PERS.ToLower())),
                             CIA = (b == null ? "-" : b.COM_NAME.ToLower()),
                             CIA_RUC = (b == null ? "-" : b.RUC),                             
                             CIA_ADDR = (b == null ? "-" : (b.ADDRESS == null ? "-" : b.ADDRESS)),
                             CIA_TELE = (b == null ? "-" : (b.TEL_ENTI == null ? "-" : b.TEL_ENTI)),                             
                             ASSIGNED = (xf == null ? "-" : xf.FIR_NAME + " " + xf.SEC_NAME + " " + xf.LAS_NAME + " " + xf.MOT_NAME),
                             NAM_FILE = (xh == null ? "" : (xh.NAM_ATTA == null ? "-" : xh.NAM_ATTA.ToLower())),
                             NAM_ATTA = (xh == null ? "" : (xh.NAM_ATTA == null ? "-" : xh.NAM_ATTA.ToLower() + "_" + xh.ID_KNOW.ToString() + ".pdf")),
                             DATE_ATTA = (xh == null ? "" : (xh.DATE_ATTA == null ? "-" : String.Format("{0:d}", xh.DATE_ATTA))),
                             a.OS,
                             COM_NAME = j.COM_NAME.ToLower(),
                         });

            tt = qUnion.Count();

            var query = (from a in qUnion.OrderByDescending(x => x.NUM_DOCU_SALE).Skip(skip).Take(take).ToList()
                         join b in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals b.ID_STAT_DOCU_SALE                         
                         select new
                         {
                             a.ID_DOCU_SALE,
                             a.ID_STAT_DOCU_SALE,
                             a.NUM_DOCU_SALE,
                             a.COD_TYPE_DOCU_SALE,
                             a.COLOR,
                             a.DAT_DOCU_SALE,
                             a.NAM_CLIE_CONT,
                             a.CLI_EMAI,
                             a.CLI_TELE,
                             a.CLI_PHON_EXTE,
                             a.CIA,
                             a.CIA_RUC,
                             a.CLI_JOB_TITL,
                             a.CIA_ADDR,
                             a.CIA_TELE,
                             a.OS,
                             a.COM_NAME,
                             a.EXP_DATE,
                             a.SUM_DOCU_SALE,
                             VENDOR = a.VENDOR.ToLower(),
                             ASSIGNED = a.ASSIGNED.ToLower(),
                             NAM_STAT_DOCU_SALE = b.NAM_STAT_DOCU_SALE.ToLower(),
                             a.NAM_FILE,
                             a.NAM_ATTA,
                             a.DATE_ATTA,
                         });

            //Resultados para el Grafico
            var qGraf = (from a in qUnion.ToList()
                         join b in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals b.ID_STAT_DOCU_SALE
                         select new
                         {
                            a.COLOR,
                            NAM_STAT_DOCU_SALE = b.NAM_STAT_DOCU_SALE.ToLower(),
                            a.ID_STAT_DOCU_SALE,
                            a.DAT_DOCU_SALE,
                            a.NUM_DOCU_SALE,
                         });

            string from = "", to = "";
            if (qGraf.Count() > 0) {
                from = String.Format("{0:d}", qGraf.OrderByDescending(d => d.DAT_DOCU_SALE).First().DAT_DOCU_SALE);
                to = String.Format("{0:d}", qGraf.OrderBy(t => t.DAT_DOCU_SALE).First().DAT_DOCU_SALE);
            }            

            int TTClosed = qGraf.Where(x => x.ID_STAT_DOCU_SALE == 4).Count();

            qGraf = qGraf.Where(x => x.ID_STAT_DOCU_SALE != 4).OrderByDescending(x => x.NUM_DOCU_SALE);

            var resultPie = (from q in qGraf
                             group q by new { q.COLOR, q.NAM_STAT_DOCU_SALE } into g
                             select new
                             {
                                 name = g.Key.NAM_STAT_DOCU_SALE.Substring(0,1).ToUpper() + 
                                        g.Key.NAM_STAT_DOCU_SALE.Substring(1,g.Key.NAM_STAT_DOCU_SALE.Length -1).ToLower(),
                                 color = g.Key.COLOR,
                                 y = g.Count(),
                             }).ToList();

            return Json(new { Data = query, Count = tt, Pie = resultPie, From = from , To = to, TTClosed = TTClosed}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListPersonED()
        {

            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9).ToList()
                         join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                         join c in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true)
                                    on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         select new
                         {
                             a.ID_PERS_ENTI,
                             b.ID_ENTI,
                             NAM_PERSON = b.FIR_NAME + " " + b.SEC_NAME + " " + b.LAS_NAME + " " + b.MOT_NAME
                         }).OrderBy(x => x.NAM_PERSON);


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAssignee(int id = 0,string id1 = "") {
            ViewBag.ID_DOCU_SALE = id;
            DOCUMENT_SALE ds = dbc.DOCUMENT_SALE.Find(id);

            ViewBag.ID_PERS_ENTI = dbe.PERSON_ENTITY.Single(x => x.ID_ENTI1 == ds.ID_COMP && x.ID_ENTI2 == ds.ID_CTTS).ID_PERS_ENTI;
            ViewBag.ID_COMP = ds.ID_COMP;
            ViewBag.Proc = id1;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AssignedTo()
        {
            string SUM_DOCU_SALE = Request.Form["SUM_DOCU_SALE"], msnError = "", msnErrorTitle = ResourceLanguaje.Resource.InformationMissing;
            int ID_QUEU = 0, ID_CATE = 0, ID_PERS_ENTI_ASSI = 0;
            int sw = 0;

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_QUEU"]), out ID_QUEU) == false) { msnError = ResourceLanguaje.Resource.Message5; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == false) { msnError = ResourceLanguaje.Resource.Message6; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_CATE"]), out ID_CATE) == false) { msnError = ResourceLanguaje.Resource.Message7; }
            else if (String.IsNullOrEmpty(SUM_DOCU_SALE)) { msnError = ResourceLanguaje.Resource.Message8; }
            if (msnError.Length > 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + msnErrorTitle + "','" + msnError + "');}window.onload=init;</script>");
            }

            int ID_DOCU_SALE = Convert.ToInt32(Request.Form["ID_DOCU_SALE_HF"]);
            int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI_HF"]);
            int ID_COMP = Convert.ToInt32(Request.Form["ID_COMP_HF"]);

            DOCUMENT_SALE ds = dbc.DOCUMENT_SALE.Find(ID_DOCU_SALE);
            ds.ID_STAT_DOCU_SALE = 2;
            dbc.DOCUMENT_SALE.Attach(ds);
            dbc.Entry(ds).State = EntityState.Modified;
            dbc.SaveChanges();

            string usuario = Convert.ToString(Session["NAM_PERS"]).ToUpper();

            try
            {
                var ticket = new TICKET();
                int ID_CHAR = 0;
                try
                {
                    ID_CHAR = dbe.PERSON_CONTRACT.Single(x => x.LAS_CONT == true && x.ID_PERS_CONT == ID_PERS_ENTI_ASSI).ID_CHAR.Value;
                }
                catch { ID_CHAR = 0; }

                ticket.ID_QUEU = ID_QUEU;
                ticket.ID_DOCU_SALE = ID_DOCU_SALE;
                ticket.ID_ACCO = 4;
                ticket.ID_TYPE_TICK = 3;
                ticket.ID_PERS_ENTI = ID_PERS_ENTI;
                ticket.ID_PERS_ENTI_END = ID_PERS_ENTI;
                ticket.ID_PERS_ENTI_ASSI = ID_PERS_ENTI_ASSI;
                ticket.ID_COMP = ID_COMP;
                ticket.ID_AREA = ID_CHAR;
                ticket.ID_PRIO = 5;
                ticket.ID_STAT = 5;
                ticket.ID_STAT_END = 5;
                ticket.ID_SOUR = 4;
                ticket.FEC_TICK = DateTime.Now;
                ticket.SUM_TICK = SUM_DOCU_SALE;
                ticket.REM_CTRL_TICK = false;
                ticket.UserId = Convert.ToInt32(Session["UserId"]);
                ticket.CREATE_TICK = DateTime.Now;
                ticket.FEC_INI_TICK = ds.EXP_DATE;
                ticket.MODIFIED_TICK = DateTime.Now;
                ticket.IS_PARENT = true;
                ticket.ID_CATE = ID_CATE;
                ticket.SEND_MAIL = false;

                dbc.TICKETs.Add(ticket);
                dbc.SaveChanges();

                int id = Convert.ToInt32(ticket.ID_TICK);
                dbc.Entry(ticket).State = EntityState.Detached;

                var detTick = new DETAILS_TICKET();
                detTick.ID_TICK = id;
                detTick.ID_STAT = 5;
                detTick.ID_TYPE_DETA_TICK = 3;
                detTick.COM_DETA_TICK = SUM_DOCU_SALE;
                detTick.FEC_SCHE = ds.EXP_DATE;
                detTick.UserId = Convert.ToInt32(Session["UserId"]);
                detTick.CREATE_DETA_INCI = DateTime.Now;
                dbc.DETAILS_TICKET.Add(detTick);
                dbc.SaveChanges();

                string code = Convert.ToString(dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK);
                string proc = Convert.ToString(Request.Form["Proc_HF"]);

                //SendMail mail = new SendMail();
                //string xx = mail.NewTicket(id);

                var dsa = new DOCUMENT_SALE_ACTIVITY();
                dsa.ID_DOCU_SALE = ID_DOCU_SALE;
                dsa.ID_STAT_DOCU_SALE = ds.ID_STAT_DOCU_SALE;
                dsa.ID_TYPE_ACTI = 2;
                dsa.UserId = ticket.UserId;
                dsa.CREATE_ACTI_DOCU_SALE = ticket.CREATE_TICK;
                dsa.COM_ACTI_DOCU_SALE = ticket.SUM_TICK;

                dbc.DOCUMENT_SALE_ACTIVITY.Add(dsa);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK_ASSI','" + proc + "','" + code + "');}window.onload=init;</script>");

            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
            }

        }

        public ActionResult UpdateBarStatusOP() {
            var status = dbc.STATUS_DOCUMENT_SALE;            

            var result = (from s in status.ToList()
                          select new
                          {
                              s.ID_STAT_DOCU_SALE,
                              s.NAM_STAT_DOCU_SALE,
                              OPs = OpsByStatus(s.ID_STAT_DOCU_SALE),
                              Total = TTOpsByStatus(s.ID_STAT_DOCU_SALE),
                          }).ToList();

            return Json(new { Data = result}, JsonRequestBehavior.AllowGet);
        }

        public int OpsByStatus(int ID_STAT_DOCU_SALE) {
            if (ID_STAT_DOCU_SALE == 0 || ID_STAT_DOCU_SALE == 1)
            {
                return dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE).Count();
            }
            else {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                int tt = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE)
                             join b in dbc.TICKETs.Where(x=>x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI) on 
                                        a.ID_DOCU_SALE equals b.ID_DOCU_SALE
                             select new
                             {
                                 a.ID_DOCU_SALE,
                             }).Count();
                return tt;
            }
           
        }

        public int TTOpsByStatus(int ID_STAT_DOCU_SALE)
        {
            return dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE).Count();
        }

        public ActionResult ListTicketsByOP(int id = 0) {

            var qUsers = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9)
                          join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                          select new { 
                            a.ID_PERS_ENTI,
                            USER = b.FIR_NAME + " " + b.SEC_NAME + " " + b.LAS_NAME + " " + b.MOT_NAME,
                            b.UserId,
                          });

            int idtick = dbc.TICKETs.Single(x => x.ID_DOCU_SALE == id).ID_TICK;

            var query = (from a in dbc.TICKETs.Where(x => x.ID_DOCU_SALE == id || x.ID_TICK_PARENT == idtick).ToList()
                         join b in dbc.TYPE_TICKET on a.ID_TYPE_TICK equals b.ID_TYPE_TICK
                         join c in dbc.STATUS on a.ID_STAT_END equals c.ID_STAT
                         join d in dbc.CATEGORies on a.ID_CATE equals d.ID_CATE
                         join e in dbc.CATEGORies on d.ID_CATE_PARE equals e.ID_CATE
                         join f in dbc.CATEGORies on e.ID_CATE_PARE equals f.ID_CATE
                         join g in dbc.CATEGORies on f.ID_CATE_PARE equals g.ID_CATE
                         join h in qUsers on a.ID_PERS_ENTI_ASSI equals h.ID_PERS_ENTI
                         join i in qUsers on a.UserId equals i.UserId
                         select new {
                             a.ID_TICK,
                             a.COD_TICK,
                             NAM_TYPE_TICK = b.NAM_TYPE_TICK.ToLower(),
                             NAM_SUBCLASS = d.NAM_CATE.ToLower(),
                             NAM_CLASS = e.NAM_CATE.ToLower(),
                             NAM_SUBCATE = f.NAM_CATE.ToLower(),
                             NAM_CATE = g.NAM_CATE.ToLower(),
                             CREATE_TICK = (a.CREATE_TICK == null ? "-" : String.Format("{0:d}",a.CREATE_TICK)),
                             MODIFIED_TICK = (a.MODIFIED_TICK == null ? "-" : String.Format("{0:d}", a.MODIFIED_TICK)),
                             NAM_ASSI = h.USER.ToLower(),
                             NAM_REQU = i.USER.ToLower(),
                             NAM_STAT = c.NAM_STAT.ToLower(),
                             a.SUM_TICK,
                         });

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewListTicket(int id = 0) {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult viewWarranty(int id = 0, int id1 = 0) {
            ViewBag.ID_DETA_DOCU_SALE = id;
            ViewBag.ID_DOCU_SALE = id1;
            return View();        
        }

        public ActionResult viewSerie(int id = 0, int id1 = 0)
        {
            ViewBag.ID_DETA_DOCU_SALE = id;
            ViewBag.ID_DOCU_SALE = id1;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveWarranty() {
            int ID_DETA_DOCU_SALE = 0, sw = 0, Hours;
            DateTime START_DATE, END_DATE;
            string opc = "", msn = "";
            
            ID_DETA_DOCU_SALE = Convert.ToInt32(Request.Form["ID_DETA_DOCU_SALE"]);
            var det = dbc.DETAIL_DOCUMENT_SALE.Find(ID_DETA_DOCU_SALE);

            opc = Convert.ToString(Request.Form["OPT1"]);            
            if (opc == "on")
            {
                if (DateTime.TryParse(Convert.ToString(Request.Form["START_DATE"]), out START_DATE) == false) { sw = 1; msn = ResourceLanguaje.Resource.Message10; }
                else if (DateTime.TryParse(Convert.ToString(Request.Form["END_DATE"]), out END_DATE) == false) { sw = 1; msn = ResourceLanguaje.Resource.Message11; }
                else {
                    det.DAT_INI_SUPP = START_DATE;
                    det.DAT_END_SUPP = END_DATE;
                    det.HOURS = null;
                }
            }
            else {
                opc = Convert.ToString(Request.Form["OPT2"]);
                if (opc == "on")
                {
                    if (Int32.TryParse(Convert.ToString(Request.Form["Hours"]), out Hours) == false) { sw = 1; msn = ResourceLanguaje.Resource.Message12; }
                    else
                    {
                        det.HOURS = Hours;
                        det.DAT_INI_SUPP = null;
                        det.DAT_END_SUPP = null;
                    }
                }
                else {
                    det.HOURS = null;
                    det.DAT_INI_SUPP = null;
                    det.DAT_END_SUPP = null;                
                }

            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + @ResourceLanguaje.Resource.InformationMissing + "','" + msn + "');}window.onload=init;</script>");
            }
            dbc.DETAIL_DOCUMENT_SALE.Attach(det);
            dbc.Entry(det).State = EntityState.Modified;
            dbc.SaveChanges();

            string ID_DOCU_SALE;
            ID_DOCU_SALE = Convert.ToString(Request.Form["ID_DOCU_SALE"]);

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK_WARR'," + ID_DOCU_SALE + ");}window.onload=init;</script>");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveSerie()
        {
            int ID_DETA_DOCU_SALE = 0, sw = 0;            
            string opc = "";
            string Serie = "";

            ID_DETA_DOCU_SALE = Convert.ToInt32(Request.Form["ID_DETA_DOCU_SALE"]);
            Serie = Convert.ToString(Request.Form["Serie"]);
            var det = dbc.DETAIL_DOCUMENT_SALE.Find(ID_DETA_DOCU_SALE);

            opc = Convert.ToString(Request.Form["OPT4"]);
            if (opc == "on")
            {
                det.SERIE = null;
            }
            else
            {
                if (Serie.Trim().Length == 0) { sw = 1; }
                else { det.SERIE = Serie; }
            }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + @ResourceLanguaje.Resource.InformationMissing  + "','" + ResourceLanguaje.Resource.Message9 + "');}window.onload=init;</script>");
            }            
            dbc.DETAIL_DOCUMENT_SALE.Attach(det);
            dbc.Entry(det).State = EntityState.Modified;
            dbc.SaveChanges();

            string ID_DOCU_SALE;
            ID_DOCU_SALE = Convert.ToString(Request.Form["ID_DOCU_SALE"]);

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK_WARR'," + ID_DOCU_SALE + ");}window.onload=init;</script>");
        }

        public ActionResult ViewActConformity(int id = 0, string id1 = "")
        {            
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.Proc = id1;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AttachActConformity(IEnumerable<HttpPostedFileBase> files) {
            int sw = 0, ID_DOCU_SALE = 0, ID_ACCO_MONT;
            DateTime DATE_ATTA;
            string code = "0";
            if (DateTime.TryParse(Convert.ToString(Request.Form["DATE_ATTA"]), out DATE_ATTA) == false) { sw = 1; }
            else if (files == null) { sw = 1; code = "3"; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + code + "');}window.onload=init;</script>");
            }
            ID_DOCU_SALE = Convert.ToInt32(Request.Form["ID_DOCU_SALE_HF"]);

            var ds = dbc.DOCUMENT_SALE.Find(ID_DOCU_SALE);
            string cia = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ds.ID_COMP).COM_NAME;
            var qAM = dbc.ACCOUNTING_MONTH.Where(x => x.ACCO_MONT == DATE_ATTA.Month && x.ID_ACCO_YEAR == DATE_ATTA.Year);
            if (qAM.Count() > 0) {
                ID_ACCO_MONT = qAM.First().ID_ACCO_MONT;
                var qKnow = dbc.KNOWLEDGEs.Where(x => x.ID_ACCO_MONT == ID_ACCO_MONT && x.ID_KNOW_CATE == 17);
                string NAM_ATTA = DATE_ATTA.Month.ToString() + "." + (qKnow.Count() + 1).ToString() + " act of conformity";

                try
                {
                    var NewKnow = new KNOWLEDGE();
                    NewKnow.ID_AREA = 63;
                    NewKnow.ID_KNOW_CATE = 17;
                    NewKnow.NAM_KNOW = NAM_ATTA;
                    NewKnow.DESC_KNOW = cia.ToUpper();
                    NewKnow.NAM_ATTA = NAM_ATTA;
                    NewKnow.EXT_ATTA = ".pdf";
                    NewKnow.DATE_ATTA = DATE_ATTA;
                    NewKnow.CREATED_KNOW = DateTime.Now;
                    NewKnow.UserId = Convert.ToInt32(Session["UserId"]);
                    NewKnow.VIG_KNOW = true;
                    NewKnow.ID_ACCO_MONT = ID_ACCO_MONT;
                    NewKnow.ID_DOCU_SALE = ID_DOCU_SALE;
                    dbc.KNOWLEDGEs.Add(NewKnow);
                    dbc.SaveChanges();

                    try
                    {
                        foreach (var file in files)
                        {
                            if (file != null)
                            {
                                file.SaveAs(Server.MapPath("~/Adjuntos/Knowledge/" + NewKnow.NAM_ATTA + "_" + Convert.ToString(NewKnow.ID_KNOW) + ".pdf"));
                            }
                        }
                        ds.ID_STAT_DOCU_SALE = 4;
                        dbc.DOCUMENT_SALE.Attach(ds);
                        dbc.Entry(ds).State = EntityState.Modified;
                        dbc.SaveChanges();
                    }
                    catch
                    {
                        KNOWLEDGE know = dbc.KNOWLEDGEs.Find(NewKnow.ID_KNOW);
                        dbc.KNOWLEDGEs.Remove(know);
                        dbc.SaveChanges();
                    }
                    string proc = Convert.ToString(Request.Form["procAC_HF"]);

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK_ACTCONF','','" + proc + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");                    
                }
            }
            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','4');}window.onload=init;</script>");
        }

        public ActionResult OPFromSIDIGE()
        {
            var OP = new DOCUMENT_SALE();

            OP.ID_COMP = Convert.ToInt32(Request.Params["ID_ENTI"]);
            OP.ID_TYPE_DOCU_SALE = Convert.ToInt32(Request.Params["ID_TYPE_DOCU_SALE"]);
            OP.NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"]);
            OP.OS = Convert.ToString(Request.Params["OS"]);

            return View(OP);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult OPFromSIDIGE(DOCUMENT_SALE OP, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                int ID_COMP_END = 0;
                int ID_DOCU_SALE = 0;
                int ID_USER = 0;
                DateTime EXP_DATE = DateTime.MinValue;

                int ID_ENTI = Convert.ToInt32(Request.Params["ID_COMP"]);
                int ID_TYPE_DOCU_SALE = Convert.ToInt32(Request.Params["ID_TYPE_DOCU_SALE"]);
                string NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"]);
                string OS = Convert.ToString(Request.Params["OS"]);

                try
                {
                    int UserId = Convert.ToInt32(Session["UserId"]);
                    if (UserId == 0) {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" + 
                                        ResourceLanguaje.Resource.SessionTerminated + "');}window.onload=init;</script>");
                        
                    }
                    ID_USER = UserId;
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" +
                                        ResourceLanguaje.Resource.SessionTerminated + "');}window.onload=init;</script>");
                }
                
                try
                {
                    ID_COMP_END = Convert.ToInt32(Request.Params["ID_COMP_END"]);

                    if (DateTime.TryParse(Convert.ToString(Request.Params["EXP_DATE"]), out EXP_DATE))
                    {
                        if (EXP_DATE == DateTime.MinValue)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" +
                                        ResourceLanguaje.Resource.InformationMissing + "');}window.onload=init;</script>");
                        }
                    }

                    if (ID_COMP_END == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" +
                                        ResourceLanguaje.Resource.InformationMissing + "');}window.onload=init;</script>");
                    }

                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" +
                                        ResourceLanguaje.Resource.InformationMissing + "');}window.onload=init;</script>");
                }

                ((IObjectContextAdapter)dbc).ObjectContext.CommandTimeout = 720;
                ID_DOCU_SALE = dbc.OP_SIDIGE_TO_ITMS(ID_ENTI, ID_TYPE_DOCU_SALE, NUM_DOCU_SALE, ID_COMP_END, EXP_DATE, OS, ID_USER).First().ID_DOCU_SALE;

                var tds = dbc.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == ID_TYPE_DOCU_SALE);

                try
                {
                    var ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == ID_DOCU_SALE);

                    ds.SUM_DOCU_SALE = OP.SUM_DOCU_SALE;
                    ds.DAT_REGISTER = DateTime.Now;
                    dbc.Entry(ds).State = EntityState.Modified;
                    dbc.SaveChanges();

                    

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            try
                            {
                                var ATTA = new ATTACHED();
                                ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                                ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                ATTA.ID_DOCU_SALE = ID_DOCU_SALE;
                                ATTA.CREATE_ATTA = DateTime.Now;

                                dbc.ATTACHEDs.Add(ATTA);
                                dbc.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/Sales/OP/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                            }
                            catch
                            {

                            }
                        }
                    }

                }
                catch
                {

                }

                try
                {
                    var email = new SendMail();
                    email.Document_Sale(ID_DOCU_SALE);
                }
                catch
                {

                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" +tds.NAM_TYPE_DOCU_SALE+" "+ NUM_DOCU_SALE + "');}window.onload=init;</script>");
            }
            catch (Exception e)
            {
                int ID_USER = 0;

                try
                {
                    ID_USER = Convert.ToInt32(Session["UserId"]);
                    var log = new EXCEPTION();

                    log.DAT_EXCE = DateTime.Now;
                    log.MESSAGE = e.Message;
                    log.UserId = ID_USER;
                    log.DES_EXCE = e.Message;

                    dbl.EXCEPTIONs.Add(log);
                    dbl.SaveChanges();
                }
                catch
                {

                }
                

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','"+ResourceLanguaje.Resource.ContactYourSystemAdministrator+"');}window.onload=init;</script>");
            }
        }

        public ActionResult Index1() {            

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = dbe.ACCESS_SALES_BY_ID_PERS_ENTI(ID_PERS_ENTI, 4).ToList();

            int acceso = query.First().ACCESS_SALE;
            if (acceso == 0) {
                return Content("Message: You have not privileges");
            }

            Session["MAIN"] = "mp8";
            return View();
        }

        public ActionResult LIST_ORGANIZATION_BY_ID_PERS_ENTI()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = dbe.LIST_ORGANIZATION_BY_ID_PERS_ENTI(ID_PERS_ENTI, 4).ToList();
            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewSalesDashboard() {
            return View();
        }

        public ActionResult viewPerformanceTotal() {

            string url = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 29 && x.ID_ACCO == 4).VAL_ACCO_PARA;
            ViewBag.URL_PERFORMANCE_TOTAL = url;

            return View();
        }

        public ActionResult viewDetailsSales(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.URL_DASHBOARD_SALES = "#";
            var url = dbe.URL_EXTERNAL.Where(x => x.ID_PERS_ENTI == id && x.NAM_URL == "DASHBOARD SALES" && x.VIG_URL == true);
            if (url.Count() > 0) {
                ViewBag.URL_DASHBOARD_SALES = (url.First().URL == null ? "#" : url.First().URL);    
            }
            return View();
        }
        
        public ActionResult Person_Sales(int id = 0) {
            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id).ToList()
                         join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                         join c in dbe.PERSON_CONTRACT.Where(x=>x.LAS_CONT == true) on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in dbe.CHARTs on c.ID_CHAR equals d.ID_CHAR
                         join e in dbe.NAME_CHART on d.ID_NAM_CHAR equals e.ID_NAM_CHAR
                         select new
                         {
                             PERSON = (b.FIR_NAME + " " + b.SEC_NAME + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME) + " " + b.MOT_NAME).ToLower(),
                             EMA_ELEC = (a.EMA_ELEC == null ? "-" : (a.EMA_ELEC.Trim().Length == 0 ? "-" : a.EMA_ELEC.Trim())).ToLower(),
                             TEL_PERS = (a.TEL_PERS == null ? "-" : (a.TEL_PERS.Trim().Length == 0 ? "-" : a.TEL_PERS.Trim())),
                             CEL_PERS = (a.CEL_PERS == null ? "-" : (a.CEL_PERS.Trim().Length == 0 ? "-" : a.CEL_PERS.Trim())),
                             EXT_PERS = (a.EXT_PERS == null ? "-" : (a.EXT_PERS.Trim().Length == 0 ? "-" : a.EXT_PERS.Trim())),
                             TEL_PERS_HOME = (b.TEL_ENTI == null ? "-" : (b.TEL_ENTI.Trim().Length == 0 ? "-" : b.TEL_ENTI.Trim())),
                             ADDRESS = (b.ADDRESS == null ? "-" : b.ADDRESS),
                             ID_PHOTO = Convert.ToString(a.ID_FOTO) + ".jpg",
                             JOBTITLE = e.NAM_CHAR.ToLower(),
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewChartScheduled() {
            ViewBag.Date = String.Format("{0:d}", DateTime.Now);
            ViewBag.DateTime = String.Format("{0:g}", DateTime.Now);

            ViewBag.TimeStar = String.Format("{0:d}", DateTime.Now) + " 08:00 AM";
            ViewBag.TimeEnd = String.Format("{0:d}", DateTime.Now.AddDays(1)) + " 08:00 AM";

            //ViewBag.Acceso = "0";
            //int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            //var query = dbe.RESPONSIBLE_CHART.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.ID_CHAR == 5);
            //if (query.Count() > 0) { ViewBag.Acceso = "1"; }

            return View();
        }

        public int ListOpsMensaje()
        {
            int ID_STAT = 1; //Open             
            int tt = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT).Count();
            int sw = 0;
            string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            foreach (string rol in rolesArray)
            {
                if (rol == "ASSIGNED OPs" || rol == "ADMINISTRADOR")
                {
                    sw = 1;
                }
            }

            return dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT).Count();

            //if (sw == 1)
            //{   //Tiene el rol y puede ver todas las OPs              
            //    return tt;
            //}
            //else
            //{
            //    if (ID_STAT == 1) //no tiene el rol por lo tanto no puede ver las OPs Open
            //    {
            //        var query = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == 0) select a);
            //        return 0;
            //    }
            //    else
            //    {//no tiene el rol pero puede ver las asignadas a el y a su personal
            //        int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            //        var listIPE = dbe.LIST_STAFF_BY_BOSS(ID_PERS_ENTI).ToList();

            //        var qry = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT).ToList()
            //                   join t in dbc.TICKETs on a.ID_DOCU_SALE equals t.ID_DOCU_SALE
            //                   where (from lb in listIPE select lb.ID_PERS_ENTI).Contains(Convert.ToInt32(t.ID_PERS_ENTI_ASSI))
            //                   select a.ID_DOCU_SALE);

            //        tt = qry.Count();
            //        return tt;
            //    }
            //}
        }
    }
}
