using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Data.Entity;
using ERPElectrodata.Objects;
using System.Data.Entity.Core;
using ERPElectrodata.Object.Talent;
using ERPElectrodata.Helpers;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Core.Objects;
using ERPElectrodata.Object;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
//using System.Drawing;
using ClosedXML.Excel;
namespace ERPElectrodata.Controllers
{
    public class ActivityController : Controller
    {
        //
        // GET: /Activity/
        public EntityEntities dbe = new EntityEntities();
        private PointTicket pt = new PointTicket();
        public CoreEntities db = new CoreEntities();
        #region Vistas Modulo Actividad
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult ReportActivity()
        {
            return View();
        }

        [Authorize]
        public ActionResult NewActivity()
        {
            ViewBag.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            ViewBag.UploadFile = "M1DT" + Convert.ToString(DateTime.Now.Ticks);
            return View();
        }

        [Authorize]
        public ActionResult DetailsActivity()
        {
            ViewBag.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            ViewBag.VALIDATEUSER = ValidateUser();
            return View();
        }
        [Authorize]
        public ActionResult Reportes()
        {
            return View();
        }

        [Authorize]
        public ActionResult ReporteMensual()
        {
            return View();
        }

        [Authorize]
        public ActionResult ReportePorUsuario()
        {
            return View();
        }
        [Authorize]
        public ActionResult ReportePorArea()
        {
            return View();
        }
        [Authorize]
        public ActionResult ReportePorCuentas()
        {
            return View();
        }
        public ActionResult viewEditActivity(int id = 0)
        {
            ACTIVITY_LOG query = dbe.ACTIVITY_LOG.Single(x => x.ID_ACTI_LOG == id);
            using (var context = new EntityEntities())
            {
                string ComentarioDetalle = "";
                ViewBag.TieneDetalleEdita = 0;
                if (query.DETAILS_TICKETS != null)
                {
                    ComentarioDetalle = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == query.DETAILS_TICKETS).COM_DETA_TICK;
                    query.COMENTARIO = ComentarioDetalle;
                    ViewBag.TieneDetalleEdita = 1;
                }
                ViewBag.ID_USR_ACT_EDIT = Convert.ToInt32(Session["UserId"]);
                ViewBag.ID_ACTI_LOG_EDIT = id;
                ViewBag.ID_CLIE_EDIT = query.ID_CLIE;
                ViewBag.NAME_CONTACTO = (query.CONTACTO == null ? "" : query.CONTACTO);
                ViewBag.DATE_INIC_EDIT = (query.DATE_INIC == null ? "" : String.Format("{0:d}", query.DATE_INIC) + " " + String.Format("{0:hh:mm tt}", query.DATE_INIC));
                ViewBag.DATE_END_EDIT = (query.DATE_END == null ? "" : String.Format("{0:d}", query.DATE_END) + " " + String.Format("{0:hh:mm tt}", query.DATE_END));
                ViewBag.ID_TYPE_ACT_EDIT = query.ID_TYPE_ACT;
                ViewBag.ID_SUBTYPE_ACT_EDIT = query.ID_SUBTYPE_ACT;
                ViewBag.COD_SUBTYPE_ACT_EDIT = query.COD_SUBTYPE_ACT;
                ViewBag.NAM_SUBTYPE_ACT = query.NAM_SUBTYPE_ACT;
                ViewBag.COMENTARIO_EDIT = query.COMENTARIO == "" ? ComentarioDetalle : query.COMENTARIO;
                ViewBag.EMAIL_EDIT = query.EMAIL;
                ViewBag.MOTIVO = (query.MOTIVO_EDIT == null ? "" : query.MOTIVO_EDIT);
                ViewBag.TODAY = DateTime.Now;
            }
            return View(query);
        }
        public ActionResult viewDetailsActivity(int id = 0)
        {
            ACTIVITY_LOG query = dbe.ACTIVITY_LOG.Single(x => x.ID_ACTI_LOG == id);
            using (var context = new EntityEntities())
            {
                string ComentarioDetalle = "";
                ViewBag.TieneDetalle = 0;
                if (query.DETAILS_TICKETS != null)
                {
                    ComentarioDetalle = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == query.DETAILS_TICKETS).COM_DETA_TICK;
                    query.COMENTARIO = ComentarioDetalle;
                    ViewBag.TieneDetalle = 1;
                }
                ViewBag.ID_USR_ACT_EDIT = Convert.ToInt32(Session["UserId"]);
                ViewBag.ID_ACTI_LOG_EDIT = id;
                ViewBag.ID_CLIE_EDIT = query.ID_CLIE;
                ViewBag.NAME_CONTACTO = (query.CONTACTO == null ? "" : query.CONTACTO);
                ViewBag.DATE_INIC_EDIT = (query.DATE_INIC == null ? "" : String.Format("{0:d}", query.DATE_INIC) + " " + String.Format("{0:hh:mm tt}", query.DATE_INIC));
                ViewBag.DATE_END_EDIT = (query.DATE_END == null ? "" : String.Format("{0:d}", query.DATE_END) + " " + String.Format("{0:hh:mm tt}", query.DATE_END));
                ViewBag.ID_TYPE_ACT_EDIT = query.ID_TYPE_ACT;
                ViewBag.ID_SUBTYPE_ACT_EDIT = query.ID_SUBTYPE_ACT;
                ViewBag.COD_SUBTYPE_ACT_EDIT = query.COD_SUBTYPE_ACT;
                ViewBag.NAM_SUBTYPE_ACT = query.NAM_SUBTYPE_ACT;
                ViewBag.COMENTARIO_EDIT = query.COMENTARIO == "" ? ComentarioDetalle : query.COMENTARIO;
                ViewBag.EMAIL_EDIT = query.EMAIL;
                ViewBag.MOTIVO = (query.MOTIVO_EDIT == null ? "" : query.MOTIVO_EDIT);
                ViewBag.TODAY = DateTime.Now;
            }
            return View(query);
        }
        public ActionResult viewNewCLient()
        {
            return View();
        }
        public ActionResult viewNewMarca()
        {
            return View();
        }
        #endregion

        #region Listar
        public ActionResult ListTypeActivity()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            using (var context = new EntityEntities())
            {
                int count = 0;
                var query = (from ta in context.TYPE_ACT_LOG.Where(x => x.VIG_ACT == true && x.ID_ACCO == ID_ACCO)
                             join sa in context.SUBTYPE_ACTITY on ta.ID_TYPE_ACT equals sa.ID_TYPE_ACT
                             select new
                             {
                                 ID_TYPE_ACT = ta.ID_TYPE_ACT,
                                 DES_ACT = ta.DES_ACT,
                                 ID_SUBTYPE_ACT = sa.ID_SUBTYPE_ACT,
                                 DES_SUB_TYPE = sa.DES_SUB_TYPE
                             });
                var result = query.ToList();
                count = result.Count();
                return Json(new { Data = result, Count = count }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListTypeActivity_Create()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            using (var context = new EntityEntities())
            {
                int count = 0;
                var query = (from ta in context.TYPE_ACT_LOG.Where(x => x.VIG_ACT == true && x.ID_ACCO == ID_ACCO)
                             join sa in context.SUBTYPE_ACTITY on ta.ID_TYPE_ACT equals sa.ID_TYPE_ACT
                             where ta.ID_TYPE_ACT != 41 && ta.ID_TYPE_ACT != 42 && ta.ID_TYPE_ACT != 1 && ta.DES_ACT != "SOPORTE INTERNO"
                             && ta.DES_ACT != "MANTENIMIENTO PREVENTIVO" && ta.DES_ACT != "SOPORTE EXTERNO"
                             select new
                             {
                                 ID_TYPE_ACT = ta.ID_TYPE_ACT,
                                 DES_ACT = ta.DES_ACT,
                                 ID_SUBTYPE_ACT = sa.ID_SUBTYPE_ACT,
                                 DES_SUB_TYPE = sa.DES_SUB_TYPE
                             });
                var result = query.ToList();
                count = result.Count();
                return Json(new { Data = result, Count = count }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult ListarTipoActividad(string q, string page)
        //{
        //    var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
        //    string termino = "";
        //    if (Request.QueryString["q"] == null)
        //    {
        //        termino = "";
        //    }
        //    else
        //    {
        //        termino = (Request.QueryString["q"].ToString()).ToUpper();
        //    }


        //        var result = (from ta in dbe.TYPE_ACT_LOG.Where(x => x.VIG_ACT == true && x.ID_ACCO == ID_ACCO)
        //                      join sa in dbe.SUBTYPE_ACTITY on ta.ID_TYPE_ACT equals sa.ID_TYPE_ACT
        //                     select new
        //                     {
        //                         id = ta.ID_TYPE_ACT,
        //                         text = (ta.DES_ACT).ToUpper(),
        //                     }).Where(x => x.text.Contains(termino));

        //        return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ListOPActivityClient(int id)
        {
            using (var context = new CoreEntities())
            {
                int count = 0;
                var allId = new int[] { 1, 3, 4, 5 };
                var query = (from ds in context.DOCUMENT_SALE.Where(x => x.ID_COMP_END == id && x.ID_STAT_DOCU_SALE != 5).OrderBy(x => x.DAT_DOCU_SALE)
                             join tds in context.TYPE_DOCUMENT_SALE on ds.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
                             join tk in context.TICKETs on ds.ID_DOCU_SALE equals tk.ID_DOCU_SALE
                             where allId.Contains((int)tk.ID_STAT_END) 
                             select new
                             {
                                 ID_DOCU_SALE = ds.ID_DOCU_SALE,
                                 NUM_DOCU_SALE = ds.NUM_DOCU_SALE.Trim(),
                                 ID_TYPE_DOCU_SALE = ds.ID_TYPE_DOCU_SALE,
                                 NAM_TYPE_DOCU_SALE = tds.NAM_TYPE_DOCU_SALE.Trim(),
                                 OP_TITLE = ds.Titulo != null ? ds.Titulo.Trim() : String.Empty,
                                 NAM_TYPE_OPDOC_SALE = (tds.NAM_TYPE_DOCU_SALE + " " + ds.NUM_DOCU_SALE.Trim()) 
                             });
                var result = query.ToList();
                count = result.Count();
                return Json(new { Data = result, Count = count }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListDetailOP(int id)
        {
            using (var context = new CoreEntities())
            {
                HtmlToText convert = new HtmlToText();
                var query = context.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id).SUM_DOCU_SALE;
                query = convert.Convert(query);
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult ListDetailTicket(int id)
        {
            using (var context = new CoreEntities())
            {
                HtmlToText convert = new HtmlToText();
                var query = context.TICKETs.Single(x => x.ID_TICK == id).SUM_TICK;
                query = convert.Convert(query);
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ListTicketsNotClosed(int id = 0)
        {
            int count = 0;
            using (var context = new CoreEntities())
            {
                var query = context.TICKETs.Where(x => x.ID_COMP == id && x.ID_STAT_END != 6 && x.ID_STAT_END != 2 && x.ID_ACCO != 0 && x.ID_DOCU_SALE == null).ToList();

                var cia = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == id && x.ID_TYPE_ENTI == 1).ToList();

                var result = (from x in query.OrderBy(x => x.ID_TICK)
                              join pe in cia on x.ID_COMP equals pe.ID_ENTI into lcia
                              from xpe in lcia.DefaultIfEmpty()
                              select new
                              {
                                  ID_TICK = x.ID_TICK,
                                  COD_TICK = x.COD_TICK,
                                  CIA = xpe != null ? xpe.COM_NAME : String.Empty
                              });
                var resultfinal = result.ToList();
                count = resultfinal.Count();
                return Json(new { Data = resultfinal, Count = count }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListArea()
        {
            int count = 0;
            using (var context = new EntityEntities())
            {
                var query = (from nc in context.AREA_ACT.Where(x => x.VIG_AREA_ACT == true)
                             select new
                             {
                                 ID_AREA_ACT = nc.ID_AREA_ACT,
                                 ID_DETALLE = nc.ID_DETALLE
                             }).OrderByDescending(x => x.ID_DETALLE);
                var resultado = query.ToList();
                count = resultado.Count();
                return Json(new { Data = resultado, Count = count }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListPersonEmail()
        {
            var qStaff = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9)
                          join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                          join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true)
                                 on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                          join d in dbe.CHARTs on c.ID_CHAR equals d.ID_CHAR
                          join e in dbe.NAME_CHART on d.ID_NAM_CHAR equals e.ID_NAM_CHAR
                          select new
                          {
                              NAME_USER = (b.FIR_NAME + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME)).ToUpper(),
                              ID_PERS_ENTI = a.ID_PERS_ENTI,
                          }).OrderBy(x => x.NAME_USER);

            return Json(new { Data = qStaff, Count = qStaff.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListPersonActivity()
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int ID_ENTI = 0;

                using (var context = new EntityEntities())
                {
                    var CLAS_ENTI = context.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI2;
                    ID_ENTI = Convert.ToInt32(CLAS_ENTI);
                    var query = context.LIST_PERSON_ACTIVITY(ID_PERS_ENTI, ID_ACCO).ToList();
                    return Json(new { Data = query, Count = query.Count(), ID_ENTI = ID_ENTI }, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json(new { Count = 0, ID_ENTI = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ListPersonalActividad(string q, string page)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
                var query = dbe.LIST_PERSON_ACTIVITY(ID_PERS_ENTI, ID_ACCO).ToList();
                var result = (from x in query
                              select new
                              {
                                  id = x.ID_PERS_ENTI,
                                  text = x.PERSON.ToUpper()
                              }).Where(x => x.text.Contains(termino.ToUpper())).OrderBy(x => x.text);
                return Json(result, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult ListPersonalActividadCbx(string q, string page)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var query = dbe.LIST_PERSON_ACTIVITY(ID_PERS_ENTI, ID_ACCO).ToList();
            var result = (from x in query
                          select new
                          {
                              id = x.ID_PERS_ENTI,
                              text = x.PERSON.ToUpper()
                          }).Where(x => x.text.Contains(termino.ToUpper())).OrderBy(x => x.text);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListPersonalActividadMultiSelect(string q, string page)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var query = dbe.LIST_PERSON_ACTIVITY(ID_PERS_ENTI, ID_ACCO).ToList();
            var result = (from x in query
                          select new
                          {
                              opcion = "<option value='" + x.ID_PERS_ENTI + "'>" + x.PERSON.ToUpper() + "</option>",
                              id = x.ID_PERS_ENTI,
                              text = x.PERSON.ToUpper()
                          }).Where(x => x.text.Contains(termino.ToUpper())).OrderBy(x => x.text);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ListPersonalActividadxArea(int id,string q, string page)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = dbe.ActListarPersonalxArea(ID_PERS_ENTI, ID_ACCO, id, termino);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListPersonalActividadxAreaCbx(int id, string q, string page)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = dbe.ActListarPersonalxArea(ID_PERS_ENTI, ID_ACCO, id, termino).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListPersonalActividadxAreaCbx2(string q, string page)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = dbe.ActListarPersonalxAreaCbx(ID_PERS_ENTI, ID_ACCO, 0, termino).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListOutsoursingAll()
        {
            var query1 = dbe.LIST_JOBTITLE_BY_PARENT(3, 4).ToList();

            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9).ToList()
                         join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                         join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true)
                                    on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in query1 on c.ID_CHAR equals d.ID_CHAR
                         select new
                         {
                             a.ID_PERS_ENTI,
                             b.ID_ENTI,
                             NAM_VENDOR_OS = b.FIR_NAME + " " + b.LAS_NAME
                         }).OrderBy(x => x.NAM_VENDOR_OS);



            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListVendorAll()
        {
            var query1 = dbe.LIST_JOBTITLE_BY_PARENT(5, 4).ToList();
            var query2 = dbe.LIST_JOBTITLE_BY_PARENT(3, 4).ToList();

            var list = query1.Union(query2).ToList();

            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9).ToList()
                         join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                         join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true)
                                    on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in list on c.ID_CHAR equals d.ID_CHAR
                         select new
                         {
                             a.ID_PERS_ENTI,
                             b.ID_ENTI,
                             NAM_VENDOR = b.FIR_NAME + " " + b.LAS_NAME
                         }).OrderBy(x => x.NAM_VENDOR);


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarVendedoresActivos()
        {
            var query1 = dbe.LIST_JOBTITLE_BY_PARENT(5, 4).ToList();
            var query2 = dbe.LIST_JOBTITLE_BY_PARENT(3, 4).ToList();

            var list = query1.Union(query2).ToList();

            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9 && x.VIG_PERS_ENTI == true).ToList()
                         join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                         join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true)
                                    on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in list on c.ID_CHAR equals d.ID_CHAR
                         select new
                         {
                             a.ID_PERS_ENTI,
                             b.ID_ENTI,
                             NAM_VENDOR = b.FIR_NAME + " " + b.LAS_NAME
                         }).OrderBy(x => x.NAM_VENDOR);


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListTicketOPActivity(int id = 0)
        {
            int count = 0;
            using (var contexto = new CoreEntities())
            {
                var allId = new int[] { 1, 3, 4, 5 };
                var query = (from ticket in contexto.TICKETs.Where(x => x.ID_DOCU_SALE == id && allId.Contains((int)x.ID_STAT_END))
                             select new
                             {
                                 NAM_TICKET_OP = ticket.COD_TICK,
                                 ID_TICKET_OP = ticket.ID_TICK
                             });
                var resultado = query.ToList();
                count = resultado.Count();
                return Json(new { Data = resultado, Count = count }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Mantenimiento de Actividades
        //public ActionResult CreateNewActivityOriginal(ACTIVITY_LOG newActivity, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
        //    int UserId = 0;
        //    int ID_PERS_ENTI = 0;
        //    int TIEMPO_ACT = 0;
        //    if (int.TryParse(Convert.ToString(Session["UserId"]), out UserId) == false)
        //    {
        //        return Content("<script type='text/javascript'>  function init() {" +
        //                                    "if(top.uploadDoneSession) top.uploadDoneSession('Ha caducado su sesión vuelva a ingresar.');}window.onload=init;</script>");
        //    }
        //    if (int.TryParse(Convert.ToString(Session["ID_PERS_ENTI"]), out ID_PERS_ENTI) == false)
        //    {
        //        return Content("<script type='text/javascript'>  function init() {" +
        //                                    "if(top.uploadDoneSession) top.uploadDoneSession('Ha caducado su sesión vuelva a ingresar.');}window.onload=init;</script>");
        //    }
        //    //int iSubType = Convert.ToInt32(Request.Params["COD_SUBTYPE_ACT"]);//este ya no se usa
        //    //int Cont = Convert.ToInt32(Request.Params["Cont"]);//ya no se usa
        //    //string nam_subtype = Request.Params["NAM_SUBTYPE_ACT"];//ya no se usa

        //    string DES_ASUNTO = Request.Params["DES_ASUNTO"];

        //    //string ListaEnviarMail = string.Empty;//ya no se usa
        //    //string email = string.Empty;//ya no se usa

        //    //string closeTicket = Request.Params["CloseTicketNA"];//no se usa
        //    //string scheduleTicket = Request.Params["SchedulerTicketNA"];//no se usa
        //    //string closeTicketOP = Request.Params["CloseOPNA"];// no se usa

        //    TIEMPO_ACT = Convert.ToInt32(Request.Params["TIEMPO_ACT"]);
        //    DateTime DATE_SCHEDULE;

        //    //bool vacio = false;//no se usa

        //    //if (DateTime.TryParse(Convert.ToString(Request.Params["DATE_SCHEDULE"]), out DATE_SCHEDULE) == false)
        //    //{
        //    //    vacio = true;
        //    //} //no se usa

        //    string KEY_ATTA = null;
        //    try
        //    {
        //        using (var context = new EntityEntities())
        //        {
        //            ACTIVITY_LOG actlog = new ACTIVITY_LOG();
        //            //actlog.ID_CLIE = newActivity.ID_CLIE;//ya no se usa

        //            actlog.ID_TYPE_ACT = newActivity.ID_TYPE_ACT;

        //            //actlog.ID_SUBTYPE_ACT = iSubType;//ya no se usa Representa el id de tipo ticket (1,2,3,4,5)
        //            //actlog.COD_SUBTYPE_ACT = newActivity.ID_SUBTYPE_ACT;//ya no se usa Representa el codigo del ticket
        //            //actlog.NAM_SUBTYPE_ACT = nam_subtype.Trim();//ya no se usa

        //            actlog.DATE_INIC = newActivity.DATE_INIC;
        //            actlog.DATE_END = newActivity.DATE_END;
        //            actlog.COMENTARIO = newActivity.COMENTARIO.Trim();

        //            //actlog.CONTACTO = (newActivity.CONTACTO == null ? "" : newActivity.CONTACTO.Trim());//ya no se usa
        //            //#region Lista de Mail de Destinatarios a Enviar Correo
        //            //List<object> listmail = new List<object>();
        //            //SendMail mail = new SendMail();

        //            //for (int i = 1; i <= Cont; i++)
        //            //{
        //            //    int ID_PERS_ENTI_RECE = Convert.ToInt32(Request.Params["RECEIVER" + i + ""]);
        //            //    if (ID_PERS_ENTI_RECE != 0)
        //            //    {
        //            //        string correo1 = context.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI_RECE).EMA_ELEC;
        //            //        string correo2 = context.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI_RECE).EMA_PERS;

        //            //        if (string.IsNullOrEmpty(correo1))
        //            //        {
        //            //            if (!string.IsNullOrEmpty(correo2))
        //            //            {
        //            //                email = correo2;
        //            //                listmail.Add(email);
        //            //            }
        //            //        }
        //            //        else
        //            //        {
        //            //            email = correo1;
        //            //            listmail.Add(email);
        //            //        }
        //            //    }
        //            //}
        //            //if (listmail.Count > 0 || !string.IsNullOrEmpty(email))
        //            //    ListaEnviarMail = string.Join(",", listmail.ToArray());
        //            //else
        //            //    ListaEnviarMail = "";
        //            //#endregion EnviarMail//ya no se usa
        //            //actlog.EMAIL = ListaEnviarMail;//ya no se usa

        //            actlog.USERID = UserId;
        //            actlog.CREATE_ACT_LOG = DateTime.Now;
        //            actlog.VIG_ACTI_LOG = true;
        //            actlog.ID_PERS_ENTI = ID_PERS_ENTI;
        //            actlog.ID_ACCO = ID_ACCO;

        //            //ya no se usa///////
        //            //if (closeTicket == "true,false")
        //            //    actlog.CLOSE_TICKET = true;
        //            //else
        //            //    actlog.CLOSE_TICKET = false;
        //            //if (scheduleTicket == "true,false")
        //            //    actlog.SCHEDULE_TICKET = true;
        //            //else
        //            //    actlog.SCHEDULE_TICKET = false;
        //            //if (closeTicketOP == "true,false")
        //            //    actlog.CLOSE_TICKET_OP = true;

        //            /////////////////////////

        //            DateTime fec_ini = Convert.ToDateTime(newActivity.DATE_INIC);
        //            DateTime fec_fin = Convert.ToDateTime(newActivity.DATE_END);
        //            TimeSpan diferencia = fec_fin.Subtract(fec_ini);
        //            actlog.TIEMPO_ACT = TIEMPO_ACT;
        //            context.ACTIVITY_LOG.Add(actlog);
        //            try
        //            {
        //                var validateLogAct = context.ACTIVITY_LOG.Where(x => x.USERID == actlog.USERID && x.DATE_INIC == actlog.DATE_INIC && x.DATE_END == actlog.DATE_END).ToList();
        //                if (validateLogAct.Count > 0)
        //                    return Content("<script type='text/javascript'> function init() {" +
        //                                                    "if(top.uploadDone) top.uploadDone('ERROR','El registro en ese rango de horas ya existe.');}window.onload=init;</script>");
        //                else
        //                    context.SaveChanges();
        //                //try
        //                //{
        //                //    mail.SendMailNewActivity(ListaEnviarMail, DES_ASUNTO, UserId, actlog);
        //                //}
        //                //catch (Exception e)
        //                //{
        //                //    string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
        //                //    ACTIVITY_LOG act_log = context.ACTIVITY_LOG.Find(actlog.ID_ACTI_LOG);
        //                //    context.Entry(act_log).State = EntityState.Deleted;
        //                //    context.SaveChanges();
        //                //    return Content("<script type='text/javascript'> function init() {" +
        //                //                    "if(top.uploadDone) top.uploadDone('ERROR','Error al enviar correo, Contáctese con el Administrador.');}window.onload=init;</script>");
        //                //}

        //                //if (actlog.CLOSE_TICKET == true)
        //                //{
        //                //    #region Agregar comentario al Detalle del Ticket Resuelto
        //                //    if ((iSubType == 2 || iSubType == 20) && actlog.COD_SUBTYPE_ACT != null)
        //                //    {
        //                //        System.Text.StringBuilder sbComentario = new System.Text.StringBuilder();
        //                //        sbComentario.Append(actlog.COMENTARIO);
        //                //        using (var contexto = new CoreEntities())
        //                //        {
        //                //            DETAILS_TICKET detTicket = new DETAILS_TICKET();
        //                //            detTicket.ID_TICK = newActivity.ID_SUBTYPE_ACT;
        //                //            detTicket.ID_STAT = 4;
        //                //            detTicket.ID_TYPE_DETA_TICK = 3;
        //                //            detTicket.COM_DETA_TICK = sbComentario.ToString();
        //                //            detTicket.UserId = UserId;
        //                //            detTicket.CREATE_DETA_INCI = DateTime.Now;
        //                //            detTicket.TO_TIME = DateTime.Now.Date;
        //                //            detTicket.FROM_TIME = DateTime.Now.Date;
        //                //            contexto.DETAILS_TICKET.Add(detTicket);
        //                //            contexto.SaveChanges();
        //                //            int ID_PERS_ENTI_Details = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //                //            pt.UpdatePointByDetailsTicket(detTicket, ID_PERS_ENTI, 0);
        //                //            #region Adjuntar archivo al Comentario al Cierre del Ticket

        //                //            try
        //                //            {
        //                //                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);
        //                //            }
        //                //            catch
        //                //            {

        //                //            }
        //                //            if (KEY_ATTA != null)
        //                //            {
        //                //                var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
        //                //                    .Where(x => x.ID_DETA_INCI == null).ToList();
        //                //                if (Attachs.Count() > 0)
        //                //                {
        //                //                    foreach (ATTACHED attach in Attachs)
        //                //                    {
        //                //                        try
        //                //                        {
        //                //                            var query = contexto.DETAILS_TICKET.OrderByDescending(x => x.CREATE_DETA_INCI).Single().ID_DETA_TICK;
        //                //                            var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
        //                //                            EObj.ID_DETA_INCI = query;
        //                //                            db.Entry(EObj).State = EntityState.Modified;
        //                //                            db.SaveChanges();
        //                //                            db.Entry(EObj).State = EntityState.Detached;
        //                //                        }
        //                //                        catch
        //                //                        {

        //                //                        }

        //                //                    }
        //                //                }
        //                //            }
        //                //            #endregion

        //                //            #region Actualizar DetalleTicket en Actividad
        //                //            int ID_ACTI_LOG = actlog.ID_ACTI_LOG;
        //                //            int ID_DETA_TICK = detTicket.ID_DETA_TICK;
        //                //            var detaTicket = contexto.DETAILS_TICKET.Find(ID_DETA_TICK).ID_DETA_TICK;

        //                //            ACTIVITY_LOG _editACT = context.ACTIVITY_LOG.Find(ID_ACTI_LOG);
        //                //            _editACT.DETAILS_TICKETS = detaTicket;
        //                //            context.Entry(_editACT).State = EntityState.Modified;
        //                //            context.SaveChanges();

        //                //            #endregion

        //                //        }
        //                //    }
        //                //    #endregion
        //                //    #region Cargar estado cerrado al Ticket
        //                //    using (var dbc = new CoreEntities())
        //                //    {
        //                //        var query = dbc.TICKETs.Single(x => x.COD_TICK.Contains(actlog.NAM_SUBTYPE_ACT)).ID_TICK;
        //                //        TICKET tick = dbc.TICKETs.Find(query);
        //                //        tick.ID_STAT_END = 4;
        //                //        tick.MODIFIED_TICK = DateTime.Now;
        //                //        tick.FOR_REP = DateTime.Now;
        //                //        dbc.Entry(tick).State = EntityState.Modified;
        //                //        dbc.SaveChanges();
        //                //    }
        //                //    mail.SendMailCloseTicketActivity(DES_ASUNTO, UserId, actlog);
        //                //    #endregion

        //                //}
        //                //else if (actlog.SCHEDULE_TICKET == true)
        //                //{

        //                //    #region Agregar comentario al Detalle del Ticket Programado
        //                //    if (iSubType == 2 && actlog.COD_SUBTYPE_ACT != null)
        //                //    {
        //                //        System.Text.StringBuilder sbComentario = new System.Text.StringBuilder();
        //                //        //string total = string.Format("{0:%h} horas {0:%m} minutos", diferencia);
        //                //        sbComentario.Append(actlog.COMENTARIO);
        //                //        //sbComentario.AppendLine("\n");
        //                //        //sbComentario.AppendLine("<br/>");
        //                //        //sbComentario.Append("Horas Trabajadas: " + total);
        //                //        using (var contexto = new CoreEntities())
        //                //        {
        //                //            DETAILS_TICKET detTicket = new DETAILS_TICKET();
        //                //            detTicket.ID_TICK = newActivity.ID_SUBTYPE_ACT;
        //                //            detTicket.ID_STAT = 5;
        //                //            detTicket.ID_TYPE_DETA_TICK = 3;
        //                //            detTicket.COM_DETA_TICK = sbComentario.ToString();

        //                //            if (vacio == false)
        //                //                detTicket.FEC_SCHE = DATE_SCHEDULE;

        //                //            detTicket.UserId = UserId;
        //                //            detTicket.CREATE_DETA_INCI = DateTime.Now;
        //                //            detTicket.TO_TIME = DateTime.Now.Date;
        //                //            detTicket.FROM_TIME = DateTime.Now.Date;
        //                //            contexto.DETAILS_TICKET.Add(detTicket);
        //                //            contexto.SaveChanges();
        //                //            int ID_PERS_ENTI_Details = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //                //            pt.UpdatePointByDetailsTicket(detTicket, ID_PERS_ENTI, 0);
        //                //            #region Adjuntar archivo al Comentario al Reprogramar el Ticket

        //                //            try
        //                //            {
        //                //                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);
        //                //            }
        //                //            catch
        //                //            {

        //                //            }
        //                //            if (KEY_ATTA != null)
        //                //            {
        //                //                var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
        //                //                    .Where(x => x.ID_DETA_INCI == null).ToList();
        //                //                if (Attachs.Count() > 0)
        //                //                {
        //                //                    foreach (ATTACHED attach in Attachs)
        //                //                    {
        //                //                        try
        //                //                        {
        //                //                            var query = contexto.DETAILS_TICKET.OrderByDescending(x => x.CREATE_DETA_INCI).Single().ID_DETA_TICK;
        //                //                            var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
        //                //                            EObj.ID_DETA_INCI = query;
        //                //                            db.Entry(EObj).State = EntityState.Modified;
        //                //                            db.SaveChanges();
        //                //                            db.Entry(EObj).State = EntityState.Detached;
        //                //                        }
        //                //                        catch
        //                //                        {

        //                //                        }

        //                //                    }
        //                //                }
        //                //            }
        //                //            #endregion

        //                //            #region Actualizar DetalleTicket en Actividad
        //                //            int ID_ACTI_LOG = actlog.ID_ACTI_LOG;
        //                //            int ID_DETA_TICK = detTicket.ID_DETA_TICK;
        //                //            var detaTicket = contexto.DETAILS_TICKET.Find(ID_DETA_TICK).ID_DETA_TICK;

        //                //            ACTIVITY_LOG _editACT = context.ACTIVITY_LOG.Find(ID_ACTI_LOG);
        //                //            _editACT.DETAILS_TICKETS = detaTicket;
        //                //            context.Entry(_editACT).State = EntityState.Modified;
        //                //            context.SaveChanges();

        //                //            #endregion
        //                //        }

        //                //    }
        //                //    #endregion
        //                //    #region Cargar estado programado al Ticket
        //                //    using (var dbc = new CoreEntities())
        //                //    {
        //                //        var query = dbc.TICKETs.Single(x => x.COD_TICK.Contains(actlog.NAM_SUBTYPE_ACT)).ID_TICK;
        //                //        TICKET tick = dbc.TICKETs.Find(query);
        //                //        tick.ID_STAT_END = 5;
        //                //        tick.MODIFIED_TICK = DateTime.Now;
        //                //        tick.FOR_REP = DateTime.Now;
        //                //        dbc.Entry(tick).State = EntityState.Modified;
        //                //        dbc.SaveChanges();
        //                //    }
        //                //    mail.SendMailScheduledTicketActivity(DES_ASUNTO, UserId, actlog, DATE_SCHEDULE);
        //                //    #endregion
        //                //}
        //                //else if (actlog.CLOSE_TICKET_OP == true)
        //                //{
        //                //    #region Agregar comentario al Detalle del Ticket OP Resuelto
        //                //    if (iSubType == 1 && newActivity.ID_TICKET_OP != null)
        //                //    {
        //                //        System.Text.StringBuilder sbComentario = new System.Text.StringBuilder();
        //                //        sbComentario.Append(actlog.COMENTARIO);
        //                //        using (var contexto = new CoreEntities())
        //                //        {
        //                //            DETAILS_TICKET detTicket = new DETAILS_TICKET();
        //                //            detTicket.ID_TICK = newActivity.ID_TICKET_OP;//revisar
        //                //            detTicket.ID_STAT = 4;
        //                //            detTicket.ID_TYPE_DETA_TICK = 3;
        //                //            detTicket.COM_DETA_TICK = sbComentario.ToString();
        //                //            detTicket.UserId = UserId;
        //                //            detTicket.CREATE_DETA_INCI = DateTime.Now;
        //                //            detTicket.TO_TIME = DateTime.Now.Date;
        //                //            detTicket.FROM_TIME = DateTime.Now.Date;
        //                //            contexto.DETAILS_TICKET.Add(detTicket);
        //                //            contexto.SaveChanges();
        //                //            int ID_PERS_ENTI_Details = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //                //            pt.UpdatePointByDetailsTicket(detTicket, ID_PERS_ENTI, 0);
        //                //            #region Actualizar DetalleTicket en Actividad
        //                //            int ID_ACTI_LOG = actlog.ID_ACTI_LOG;
        //                //            int ID_DETA_TICK = detTicket.ID_DETA_TICK;
        //                //            var detaTicket = contexto.DETAILS_TICKET.Find(ID_DETA_TICK).ID_DETA_TICK;

        //                //            ACTIVITY_LOG _editACT = context.ACTIVITY_LOG.Find(ID_ACTI_LOG);
        //                //            _editACT.DETAILS_TICKETS = detaTicket;
        //                //            _editACT.CLOSE_TICKET_OP = true;
        //                //            _editACT.ID_TICKET_OP = newActivity.ID_TICKET_OP;
        //                //            context.Entry(_editACT).State = EntityState.Modified;
        //                //            context.SaveChanges();

        //                //            #endregion

        //                //        }
        //                //        #region Adjuntar archivo al Comentario al ticket OP
        //                //        try
        //                //        {
        //                //            KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);
        //                //        }
        //                //        catch
        //                //        {

        //                //        }
        //                //        if (KEY_ATTA != null)
        //                //        {
        //                //            var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
        //                //                .Where(x => x.ID_DETA_INCI == null).ToList();
        //                //            if (Attachs.Count() > 0)
        //                //            {
        //                //                foreach (ATTACHED attach in Attachs)
        //                //                {
        //                //                    try
        //                //                    {
        //                //                        var query = db.DETAILS_TICKET.OrderByDescending(x => x.CREATE_DETA_INCI).FirstOrDefault().ID_DETA_TICK;
        //                //                        var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
        //                //                        EObj.ID_DETA_INCI = query;
        //                //                        db.Entry(EObj).State = EntityState.Modified;
        //                //                        db.SaveChanges();
        //                //                        db.Entry(EObj).State = EntityState.Detached;
        //                //                    }
        //                //                    catch
        //                //                    {
        //                //                        return Content("<script type='text/javascript'> function init() {" +
        //                //                                                                                "if(top.uploadDone) top.uploadDone('ERROR','Error al Adjuntar archivo.');}window.onload=init;</script>");
        //                //                    }

        //                //                }
        //                //            }
        //                //        }
        //                //        #endregion
        //                //    }
        //                //    #endregion
        //                //    #region Cargar estado cerrado al Ticket
        //                //    using (var dbc = new CoreEntities())
        //                //    {
        //                //        var query = dbc.TICKETs.Single(x => x.ID_TICK == newActivity.ID_TICKET_OP).ID_TICK;
        //                //        TICKET tick = dbc.TICKETs.Find(query);
        //                //        tick.ID_STAT_END = 4;
        //                //        tick.MODIFIED_TICK = DateTime.Now;
        //                //        tick.FOR_REP = DateTime.Now;
        //                //        dbc.Entry(tick).State = EntityState.Modified;
        //                //        dbc.SaveChanges();
        //                //    }
        //                //    mail.SendMailCloseTicketActivity(DES_ASUNTO, UserId, actlog);
        //                //    #endregion
        //                //}
        //                //else
        //                //{
        //                    #region Agregar Comentario a OP
        //                    if (iSubType == 1 && actlog.COD_SUBTYPE_ACT != null)
        //                    {
        //                        System.Text.StringBuilder sbComentario = new System.Text.StringBuilder();
        //                        sbComentario.Append(actlog.COMENTARIO);
        //                        using (var contexto = new CoreEntities())
        //                        {
        //                            DOCUMENT_SALE_ACTIVITY docSaleAct = new DOCUMENT_SALE_ACTIVITY();
        //                            var query = contexto.DOCUMENT_SALE.Find(newActivity.ID_SUBTYPE_ACT);
        //                            docSaleAct.ID_DOCU_SALE = newActivity.ID_SUBTYPE_ACT;
        //                            docSaleAct.ID_STAT_DOCU_SALE = query.ID_STAT_DOCU_SALE;
        //                            docSaleAct.ID_TYPE_ACTI = 1;
        //                            docSaleAct.COM_ACTI_DOCU_SALE = sbComentario.ToString();
        //                            docSaleAct.UserId = UserId;
        //                            docSaleAct.CREATE_ACTI_DOCU_SALE = DateTime.Now;
        //                            contexto.DOCUMENT_SALE_ACTIVITY.Add(docSaleAct);
        //                            contexto.SaveChanges();

        //                            #region Registrar Detalle de Actividad en Ticket
        //                            var DetailsTicket = contexto.TICKETs.Single(x => x.ID_DOCU_SALE == docSaleAct.ID_DOCU_SALE).ID_TICK;
        //                            DETAILS_TICKET detTicketXOP = new DETAILS_TICKET();
        //                            detTicketXOP.ID_TICK = DetailsTicket;
        //                            detTicketXOP.ID_TYPE_DETA_TICK = 1;
        //                            detTicketXOP.COM_DETA_TICK = sbComentario.ToString();
        //                            detTicketXOP.UserId = UserId;
        //                            detTicketXOP.CREATE_DETA_INCI = DateTime.Now;
        //                            detTicketXOP.TO_TIME = DateTime.Now.Date;
        //                            detTicketXOP.FROM_TIME = DateTime.Now.Date;
        //                            contexto.DETAILS_TICKET.Add(detTicketXOP);
        //                            contexto.SaveChanges();
        //                            int ID_PERS_ENTI_Details = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //                            pt.UpdatePointByDetailsTicket(detTicketXOP, ID_PERS_ENTI, 0);
        //                            #endregion
        //                        }
        //                    }
        //                    #endregion

        //                    #region Agregar comentario a Ticket
        //                    if ((iSubType == 2 || iSubType == 8) && actlog.COD_SUBTYPE_ACT != null)
        //                    {
        //                        System.Text.StringBuilder sbComentario = new System.Text.StringBuilder();
        //                        sbComentario.Append(actlog.COMENTARIO);
        //                        using (var contexto = new CoreEntities())
        //                        {
        //                            DETAILS_TICKET detTicket = new DETAILS_TICKET();
        //                            detTicket.ID_TICK = newActivity.ID_SUBTYPE_ACT;
        //                            detTicket.ID_STAT = null;
        //                            detTicket.ID_TYPE_DETA_TICK = 1;
        //                            detTicket.COM_DETA_TICK = sbComentario.ToString();
        //                            detTicket.UserId = UserId;
        //                            detTicket.CREATE_DETA_INCI = DateTime.Now;
        //                            detTicket.TO_TIME = DateTime.Now.Date;
        //                            detTicket.FROM_TIME = DateTime.Now.Date;
        //                            contexto.DETAILS_TICKET.Add(detTicket);
        //                            contexto.SaveChanges();
        //                            int ID_PERS_ENTI_Details = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //                            pt.UpdatePointByDetailsTicket(detTicket, ID_PERS_ENTI, 0);

        //                            #region Actualizar DetalleTicket en Actividad
        //                            int ID_ACTI_LOG = actlog.ID_ACTI_LOG;
        //                            int ID_DETA_TICK = detTicket.ID_DETA_TICK;
        //                            var detaTicket = contexto.DETAILS_TICKET.Find(ID_DETA_TICK).ID_DETA_TICK;

        //                            ACTIVITY_LOG _editACT = context.ACTIVITY_LOG.Find(ID_ACTI_LOG);
        //                            _editACT.DETAILS_TICKETS = ID_DETA_TICK;
        //                            context.Entry(_editACT).State = EntityState.Modified;
        //                            context.SaveChanges();

        //                            #endregion
        //                        }
        //                        #region Adjuntar archivo al Comentario
        //                        try
        //                        {
        //                            KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (KEY_ATTA != null)
        //                        {
        //                            var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
        //                                .Where(x => x.ID_DETA_INCI == null).ToList();
        //                            if (Attachs.Count() > 0)
        //                            {
        //                                foreach (ATTACHED attach in Attachs)
        //                                {
        //                                    try
        //                                    {
        //                                        var query = db.DETAILS_TICKET.OrderByDescending(x => x.CREATE_DETA_INCI).FirstOrDefault().ID_DETA_TICK;
        //                                        var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
        //                                        EObj.ID_DETA_INCI = query;
        //                                        db.Entry(EObj).State = EntityState.Modified;
        //                                        db.SaveChanges();
        //                                        db.Entry(EObj).State = EntityState.Detached;
        //                                    }
        //                                    catch
        //                                    {
        //                                        return Content("<script type='text/javascript'> function init() {" +
        //                                                                                                "if(top.uploadDone) top.uploadDone('ERROR','Error al Adjuntar archivo.');}window.onload=init;</script>");
        //                                    }

        //                                }
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    #endregion
        //                //}
        //            }
        //            catch (DbEntityValidationException dbEx)
        //            {
        //                Exception raise = dbEx;
        //                foreach (var validationErrors in dbEx.EntityValidationErrors)
        //                {
        //                    foreach (var validationError in validationErrors.ValidationErrors)
        //                    {
        //                        string message = string.Format("{0}:{1}",
        //                            validationErrors.Entry.Entity.ToString(),
        //                            validationError.ErrorMessage);
        //                        raise = new InvalidOperationException(message, raise);
        //                    }
        //                }
        //                return Content("<script type='text/javascript'> function init() {" +
        //                                                "if(top.uploadDone) top.uploadDone('ERROR','" + dbEx.ToString() + "');}window.onload=init;</script>");
        //            }
        //            return Content("<script type='text/javascript'>  function init() {" +
        //                                "if(top.uploadDone) top.uploadDone('OK','Su actividad se ha Guardado correctamente.');}window.onload=init;</script>");
        //        }
        //    }
        //    catch (EntityException e)
        //    {
        //        string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
        //        return Content("<script type='text/javascript'> function init() {" +
        //                        "if(top.uploadDone) top.uploadDone('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
        //    }
        //}
        [HttpPost, ValidateInput(false)]
        public ActionResult CreateNewActivity(ACTIVITY_LOG newActivity, IEnumerable<HttpPostedFileBase> files)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = 0;
            int ID_PERS_ENTI = 0;
            int TIEMPO_ACT = 0;
            newActivity.DATE_INIC = DateTime.ParseExact(Request.Params["DATE_INIC"], "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            newActivity.DATE_END = DateTime.ParseExact(Request.Params["DATE_END"], "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            if (int.TryParse(Convert.ToString(Session["UserId"]), out UserId) == false)
            {
                return Content("<script type='text/javascript'>  function init() {" +
                                            "if(top.MostrarMensajeSession) top.MostrarMensajeSession('Ha caducado su sesión vuelva a ingresar.');}window.onload=init;</script>");
            }
            if (int.TryParse(Convert.ToString(Session["ID_PERS_ENTI"]), out ID_PERS_ENTI) == false)
            {
                return Content("<script type='text/javascript'>  function init() {" +
                                            "if(top.MostrarMensajeSession) top.MostrarMensajeSession('Ha caducado su sesión vuelva a ingresar.');}window.onload=init;</script>");
            }

            string DES_ASUNTO = Request.Params["DES_ASUNTO"];

            TIEMPO_ACT = Convert.ToInt32(Request.Params["TIEMPO_ACT"]);

            string KEY_ATTA = null;
            try
            {
                using (var context = new EntityEntities())
                {
                    ACTIVITY_LOG actlog = new ACTIVITY_LOG();

                    actlog.ID_TYPE_ACT = newActivity.ID_TYPE_ACT;

                    actlog.DATE_INIC = newActivity.DATE_INIC;
                    actlog.DATE_END = newActivity.DATE_END;
                    actlog.COMENTARIO = newActivity.COMENTARIO.Trim();
                    actlog.USERID = UserId;
                    actlog.CREATE_ACT_LOG = DateTime.Now;
                    actlog.VIG_ACTI_LOG = true;
                    actlog.ID_PERS_ENTI = ID_PERS_ENTI;
                    actlog.ID_ACCO = ID_ACCO;

                    DateTime fec_ini = Convert.ToDateTime(newActivity.DATE_INIC);
                    DateTime fec_fin = Convert.ToDateTime(newActivity.DATE_END);
                    TimeSpan diferencia = fec_fin.Subtract(fec_ini);
                    actlog.TIEMPO_ACT = TIEMPO_ACT;
                    context.ACTIVITY_LOG.Add(actlog);
                    try
                    {
                        var validateLogAct = context.ACTIVITY_LOG.Where(x => x.USERID == actlog.USERID && x.DATE_INIC == actlog.DATE_INIC && x.DATE_END == actlog.DATE_END).ToList();
                        if (validateLogAct.Count > 0)
                            return Content("<script type='text/javascript'> function init() {" +
                                                            "if(top.MostrarMensaje) top.MostrarMensaje('ERROR','El registro en ese rango de horas ya existe.');}window.onload=init;</script>");
                        else
                            context.SaveChanges();

                        int iSubType = 2;
                        
                        #region Agregar Comentario a OP
                        if (iSubType == 1)
                        {
                            System.Text.StringBuilder sbComentario = new System.Text.StringBuilder();
                            sbComentario.Append(actlog.COMENTARIO);
                            using (var contexto = new CoreEntities())
                            {
                                DOCUMENT_SALE_ACTIVITY docSaleAct = new DOCUMENT_SALE_ACTIVITY();
                                var query = contexto.DOCUMENT_SALE.Find(newActivity.ID_SUBTYPE_ACT);
                                docSaleAct.ID_DOCU_SALE = newActivity.ID_SUBTYPE_ACT;
                                docSaleAct.ID_STAT_DOCU_SALE = query.ID_STAT_DOCU_SALE;
                                docSaleAct.ID_TYPE_ACTI = 1;
                                docSaleAct.COM_ACTI_DOCU_SALE = sbComentario.ToString();
                                docSaleAct.UserId = UserId;
                                docSaleAct.CREATE_ACTI_DOCU_SALE = DateTime.Now;
                                contexto.DOCUMENT_SALE_ACTIVITY.Add(docSaleAct);
                                contexto.SaveChanges();

                                #region Registrar Detalle de Actividad en Ticket
                                var DetailsTicket = contexto.TICKETs.Single(x => x.ID_DOCU_SALE == docSaleAct.ID_DOCU_SALE).ID_TICK;
                                DETAILS_TICKET detTicketXOP = new DETAILS_TICKET();
                                detTicketXOP.ID_TICK = DetailsTicket;
                                detTicketXOP.ID_TYPE_DETA_TICK = 1;
                                detTicketXOP.COM_DETA_TICK = sbComentario.ToString();
                                detTicketXOP.UserId = UserId;
                                detTicketXOP.CREATE_DETA_INCI = DateTime.Now;
                                detTicketXOP.TO_TIME = DateTime.Now.Date;
                                detTicketXOP.FROM_TIME = DateTime.Now.Date;
                                contexto.DETAILS_TICKET.Add(detTicketXOP);
                                contexto.SaveChanges();
                                int ID_PERS_ENTI_Details = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                                pt.UpdatePointByDetailsTicket(detTicketXOP, ID_PERS_ENTI, 0);
                                #endregion
                            }
                        }
                        #endregion

                        #region Agregar comentario a Ticket
                        if ((iSubType == 2 || iSubType == 8))
                        {
                            System.Text.StringBuilder sbComentario = new System.Text.StringBuilder();
                            sbComentario.Append(actlog.COMENTARIO);
                            using (var contexto = new CoreEntities())
                            {
                                DETAILS_TICKET detTicket = new DETAILS_TICKET();
                                detTicket.ID_TICK = newActivity.ID_SUBTYPE_ACT;
                                detTicket.ID_STAT = null;
                                detTicket.ID_TYPE_DETA_TICK = 1;
                                detTicket.COM_DETA_TICK = sbComentario.ToString();
                                detTicket.UserId = UserId;
                                detTicket.CREATE_DETA_INCI = DateTime.Now;
                                detTicket.TO_TIME = DateTime.Now.Date;
                                detTicket.FROM_TIME = DateTime.Now.Date;
                                contexto.DETAILS_TICKET.Add(detTicket);
                                contexto.SaveChanges();
                                int ID_PERS_ENTI_Details = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                                #region Actualizar DetalleTicket en Actividad
                                int ID_ACTI_LOG = actlog.ID_ACTI_LOG;
                                int ID_DETA_TICK = detTicket.ID_DETA_TICK;
                                var detaTicket = contexto.DETAILS_TICKET.Find(ID_DETA_TICK).ID_DETA_TICK;

                                ACTIVITY_LOG _editACT = context.ACTIVITY_LOG.Find(ID_ACTI_LOG);
                                _editACT.DETAILS_TICKETS = ID_DETA_TICK;
                                context.Entry(_editACT).State = EntityState.Modified;
                                context.SaveChanges();

                                #endregion
                            }
                            #region Adjuntar archivo al Comentario
                            try
                            {
                                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);
                            }
                            catch
                            {

                            }
                            if (KEY_ATTA != null)
                            {
                                var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                                    .Where(x => x.ID_DETA_INCI == null).ToList();
                                if (Attachs.Count() > 0)
                                {
                                    foreach (ATTACHED attach in Attachs)
                                    {
                                        try
                                        {
                                            var query = db.DETAILS_TICKET.OrderByDescending(x => x.CREATE_DETA_INCI).FirstOrDefault().ID_DETA_TICK;
                                            var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                                            EObj.ID_DETA_INCI = query;
                                            db.Entry(EObj).State = EntityState.Modified;
                                            db.SaveChanges();
                                            db.Entry(EObj).State = EntityState.Detached;
                                        }
                                        catch
                                        {
                                            return Content("<script type='text/javascript'> function init() {" +
                                                                                                    "if(top.uploadDone) top.uploadDone('ERROR','Error al Adjuntar archivo.');}window.onload=init;</script>");
                                        }

                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        //return Content("<script type = 'text/javascript'> toastr.warning('" + dbEx.ToString() + "', 'ERROR'); </script>");
                        //return Content("<script type='text/javascript'> function init() {" +
                        //                                "if(top.MostrarMensaje) top.MostrarMensaje('ERROR','" + dbEx.ToString() + "');}window.onload=init;</script>");
                        return Content("<script type='text/javascript'> function init() {" +
                                                        "if(top.MostrarMensaje) top.MostrarMensaje('ERROR','Validar los datos ingresados');}window.onload=init;</script>");
                    }
                    return Content("<script type='text/javascript'>  function init() {" +
                                        "if(top.MostrarMensaje) top.MostrarMensaje('OK','Su actividad se ha Guardado correctamente.');}window.onload=init;</script>");
                }
            }
            catch (EntityException e)
            {
                string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MostrarMensaje) top.MostrarMensaje('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }
        }





        public ActionResult CreateClient(string DES_RAZON_SOCIAL, string DES_RUC)
        {
            using (var context = new EntityEntities())
            {
                CLASS_ENTITY ce = new CLASS_ENTITY();
                ce.COM_NAME = DES_RAZON_SOCIAL;
                ce.NUM_TYPE_DI = DES_RUC;
                ce.CREATED = DateTime.Now;
                ce.VIG_ENTI = true;
                ce.ID_TYPE_ENTI = 1;
                ce.ID_TYPE_DI = 4;
                context.CLASS_ENTITY.Add(ce);
                context.SaveChanges();
            }
            return Content("OK");
        }
        public ActionResult CreateMarca(string nom_marca, string det_marca)
        {
            using (var context = new CoreEntities())
            {
                MANUFACTURER ma = new MANUFACTURER();
                ma.NAM_MANU = nom_marca;
                ma.DESC_MANU = det_marca;
                context.MANUFACTURERs.Add(ma);
                context.SaveChanges();
            }
            return Content("OK");
        }
        //public ActionResult ListActivity(int id, string ID_CLIE, string ID_ENTI_FILTER, DateTime SIN_DATE, DateTime TO_DATE, string PALABRA_CLAVE, int URL)
        //{
        //    int count = 0;
        //    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
        //    using (var context = new EntityEntities())
        //    {
        //        int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

        //        if (ID_PERS_ENTI == 25069) { ID_PERS_ENTI = 381; }//Visualizacion de RenzoCobos

        //        var listActivityOperation = context.LIST_ACTIVITY_SUPERVISOR(ID_PERS_ENTI, ID_ACCO);
        //        var query = (from LAO in listActivityOperation
        //                     select new
        //                     {
        //                         ID_ACTI_LOG = LAO.ID_ACTI_LOG,
        //                         NAME_USER = LAO.NAME_USER,
        //                         ID_CLIE = LAO.ID_CLIE,
        //                         COM_NAME = LAO.COM_NAME,
        //                         DES_ACT = LAO.DES_ACT,
        //                         DES_SUB_TYPE = LAO.DES_SUB_TYPE,
        //                         NAM_SUB_TYPE = LAO.NAM_SUBTYPE_ACT,
        //                         DATE_INIC = (LAO.DATE_INIC == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", LAO.DATE_INIC)),
        //                         DATE_END = (LAO.DATE_END == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", LAO.DATE_END)),
        //                         COMENTARIO = LAO.COMENTARIO,
        //                         ID_PERS_ENTI = LAO.ID_PERS_ENTI,
        //                         TIEMPO_ACT = LAO.TIEMPO_ACT,
        //                         LAO.NAM_ACCO,
        //                         LAO.CATEGORIA3,
        //                         LAO.CATEGORIA4
        //                     });

        //        if (URL == 1)
        //        {
        //            query = query.Where(x => x.NAME_USER == PALABRA_CLAVE);
        //        }

        //        if (!string.IsNullOrEmpty(ID_CLIE))
        //        {
        //            query = query.Where(x => x.ID_CLIE == Convert.ToInt32(ID_CLIE));
        //        }
        //        if (!string.IsNullOrEmpty(ID_ENTI_FILTER))
        //        {
        //            query = query.Where(x => x.ID_PERS_ENTI == Convert.ToInt32(ID_ENTI_FILTER));
        //        }
        //        if (SIN_DATE != DateTime.MinValue)
        //        {
        //            query = query.Where(x => Convert.ToDateTime(x.DATE_INIC) >= SIN_DATE);
        //        }
        //        if (TO_DATE.Date.AddHours(23).AddMinutes(59).AddSeconds(59) != DateTime.MinValue)
        //        {
        //            query = query.Where(x => Convert.ToDateTime(x.DATE_INIC) <= TO_DATE.Date.AddHours(23).AddMinutes(59).AddSeconds(59));
        //        }
        //        if (!string.IsNullOrEmpty(PALABRA_CLAVE))
        //        {
        //            query = query.Where(x => x.COM_NAME.ToUpper().Contains(PALABRA_CLAVE.Trim().ToUpper()) || x.NAME_USER.ToUpper().Contains(PALABRA_CLAVE.ToUpper()) || x.DES_ACT.ToUpper().Contains(PALABRA_CLAVE.ToUpper()) || x.NAM_SUB_TYPE.ToUpper().Contains(PALABRA_CLAVE.ToUpper()) || x.COMENTARIO.ToUpper().Contains(PALABRA_CLAVE.ToUpper()));
        //        }
        //        var listresult = query.ToList();
        //        var result = listresult.OrderByDescending(x => x.DATE_INIC + x.COM_NAME).Take(100);
        //        count = result.Count();
        //        return Json(new { Data = result, Count = count }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public ActionResult ListActivityN(string ID_CLIE, string ID_ENTI_FILTER, string SIN_DATE, string TO_DATE, string PALABRA_CLAVE, int URL)
        {
            int count = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new EntityEntities())
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                if (ID_PERS_ENTI == 25069) { ID_PERS_ENTI = 381; }//Visualizacion de RenzoCobos
                #region ConversParam
                int ID_CLI = String.IsNullOrEmpty(ID_CLIE) ? -1 : Convert.ToInt32(ID_CLIE);
                //string ID_USU;
           
                int ID_USU;
                try
                {
                    ID_USU = String.IsNullOrEmpty(ID_ENTI_FILTER) ? -1 : Convert.ToInt32(ID_ENTI_FILTER);
                }
                catch
                {
                    var query = dbe.CLASS_ENTITY.Where(x => x.FIR_NAME + " " + x.LAS_NAME == ID_ENTI_FILTER && x.VIG_ENTI == true);
                    var queryU = (from x in query
                                  join pe in dbe.PERSON_ENTITY on x.ID_ENTI equals pe.ID_ENTI2
                                  select new
                                  {
                                      ID_PERS_ENTI = pe.ID_PERS_ENTI,
                                      ID_ENTI1 = pe.ID_ENTI1
                                  });
                    ID_USU = queryU.Single(x => x.ID_ENTI1 == 9).ID_PERS_ENTI;
                }
                //if (String.IsNullOrEmpty(ID_ENTI_FILTER))
                //    ID_USU = "-1";
                //else
                //    ID_USU = ID_ENTI_FILTER;
                string format = "dd/MM/yyyy";
                DateTime SIN_DATE2;
                DateTime TO_DATE2;
                DateTime? START_DATE;
                DateTime? END_DATE;
                if (SIN_DATE == "" && TO_DATE == "")
                {
                    START_DATE = null;
                    END_DATE = null;
                }
                else
                {
                    //Conversión
                    SIN_DATE2 = DateTime.ParseExact(SIN_DATE, format, CultureInfo.InvariantCulture);
                    TO_DATE2 = DateTime.ParseExact(TO_DATE, format, CultureInfo.InvariantCulture);
                    if (SIN_DATE2 == DateTime.MinValue) { START_DATE = Convert.ToDateTime("01-01-0001").Date; } else { START_DATE = SIN_DATE2; };
                    if (TO_DATE2 == DateTime.MaxValue) { END_DATE = Convert.ToDateTime("01-01-0001").Date; } else { END_DATE = TO_DATE2; };
                }
                #endregion

                var result = context.ListarActividades(ID_PERS_ENTI, ID_ACCO,ID_CLI,ID_USU,PALABRA_CLAVE,START_DATE,END_DATE,URL).ToList();
                
                 count = result.Count();
                return Json(new { Data = result, Count = count },"application/json",Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet,AllowAnonymous]
        public ActionResult EditarActividad(int id)
        {
            using (var context = new EntityEntities())
            {
                var result = context.ActividadxId(id).First();
                ViewBag.IdActividad = id;
                ViewBag.IdTipoActividad = result.ID_TYPE_ACT;
                ViewBag.IdCliente = result.ID_CLIE;
                ViewBag.FIni = result.DATE_INIC;
                ViewBag.FEnd = result.DATE_END;
                ViewBag.Coment = Convert.ToString(result.COMENTARIO).Replace("/<p[^>]*>/g", "");
            }
            
            

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditarActividad(ACTIVITY_LOG oActivity)
        {
            string strfini = Request.Params["DATE_INIC"];
            string strfend = Request.Params["DATE_END"];
            int idAct = Convert.ToInt32(Request.Params["idAct"]); 
            int cliente = Convert.ToInt32(Request.Params["edIDE_CLIE"]);
            //int tActividad = Convert.ToInt32(Request.Params["edID_TYPE_ACT"]);
            DateTime fIni = DateTime.ParseExact(strfini,"dd/MM/yyyy hh:mm tt",CultureInfo.InvariantCulture);
            DateTime fEnd = DateTime.ParseExact(strfend, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string Coment = Convert.ToString(Request.Params["COMENTARIO"]);

            using (var context = new EntityEntities())
            {
                var result = context.EditarActividadTicket(idAct,cliente,oActivity.ID_TYPE_ACT,fIni,fEnd,Coment);
            }
            
            return Content("<script type='text/javascript'> function init() {" +
                                                            "if(top.MostrarMensaje) top.MostrarMensaje('OK','Actividad Editada');}window.onload=init;</script>");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarActividad(int id)
        {
            using (var context = new EntityEntities())
            {
                var result = context.EliminarActividadTicket(id);
            }
            return Content("<script type='text/javascript'> function init() {" +
                                                            "if(top.MostrarMensaje) top.MostrarMensaje('OK','Actividad Editada');}window.onload=init;</script>");
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult EditActivity(ACTIVITY_LOG editActivity)
        {
            int sw = 0;
            string msj = string.Empty;
            DateTime DATE_INIC_EDIT, DATE_END_EDIT;
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_ACTI_LOG_EDIT = Convert.ToInt32(Request.Params["ID_ACTI_LOG_EDIT"]);
            string nam_subtype = Request.Params["NAM_SUBTYPE_ACT"];
            int ID_CLIE_EDIT = 0, ID_TYPE_ACT_EDIT = 0, COD_SUBTYPE_ACT_EDIT = 0, ID_SUBTYPE_ACT_EDIT = 0;
            string NAME_CONTACTO = Request.Params["NAME_CONTACTO"];
            string COMENTARIO_EDIT = Request.Params["COMENTARIO_EDIT"];
            string COMENTARIO = Request.Params["COMENTARIO"];
            string NAM_SUBTYPE_ACT_EDIT = Request.Params["NAM_SUBTYPE_ACT_EDIT"];
            string MOTIVO_EDIT = Request.Params["MOTIVO_EDIT"];
            int TieneDetalle = Convert.ToInt32(Request.Params["hdnTieneDetalleEdita"]);
            int TIEMPO_ACT_EDIT = 0;

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CLIE_EDIT"]), out ID_CLIE_EDIT) == false) { sw = 1; msj = "Se debe ingresar el Cliente."; }
            if (DateTime.TryParse(Convert.ToString(Request.Params["DATE_INIC_EDIT"]), out DATE_INIC_EDIT) == false) { sw = 1; msj = "La fecha de Inicio no puede ser vacia."; }
            if (DateTime.TryParse(Convert.ToString(Request.Params["DATE_END_EDIT"]), out DATE_END_EDIT) == false) { sw = 1; msj = "La fecha de Fin no puede ser vacia."; }
            if (COMENTARIO_EDIT.Trim().Length == 0) { sw = 1; msj = "Se debe ingresar la Descripcion de la actividad."; }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_ACT_EDIT"]), out ID_TYPE_ACT_EDIT) == false) { sw = 1; msj = "Se debe el tipo de Actividad."; }
            if (ID_TYPE_ACT_EDIT != 8)
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["COD_SUBTYPE_ACT_EDIT"]), out COD_SUBTYPE_ACT_EDIT) == false) { sw = 1; msj = "Error al ingresar el subTipo de Actividad"; }
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUBTYPE_ACT_EDIT"]), out ID_SUBTYPE_ACT_EDIT) == false) { sw = 1; msj = "Se debe ingresar la descripcion del Subtipo"; }
            }
            if (MOTIVO_EDIT.Trim().Length == 0) { sw = 1; msj = "Se debe ingresar el motivo."; }
            if (Int32.TryParse(Convert.ToString(Request.Params["TIEMPO_ACT_EDIT"]), out TIEMPO_ACT_EDIT) == false) { }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                               "if(top.uploadDoneEditActivity) top.uploadDoneEditActivity('ERROR','" + msj + "');}window.onload=init;</script>");
            }

            using (var context = new EntityEntities())
            {
                ACTIVITY_LOG _editACT = context.ACTIVITY_LOG.Find(ID_ACTI_LOG_EDIT);

                _editACT.ID_CLIE = ID_CLIE_EDIT;
                _editACT.ID_TYPE_ACT = ID_TYPE_ACT_EDIT;
                _editACT.ID_SUBTYPE_ACT = ID_SUBTYPE_ACT_EDIT;
                _editACT.COD_SUBTYPE_ACT = COD_SUBTYPE_ACT_EDIT;
                _editACT.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT_EDIT;
                _editACT.DATE_INIC = DATE_INIC_EDIT;
                _editACT.DATE_END = DATE_END_EDIT;
                _editACT.COMENTARIO = TieneDetalle == 0 ? COMENTARIO_EDIT : "";
                _editACT.MODIFIED_ACT_LOG = DateTime.Now;
                _editACT.CONTACTO = NAME_CONTACTO;
                _editACT.ID_PERS_ENTI_EDIT = ID_PERS_ENTI;
                _editACT.MOTIVO_EDIT = MOTIVO_EDIT;
                _editACT.TIEMPO_ACT = TIEMPO_ACT_EDIT;
                context.Entry(_editACT).State = EntityState.Modified;
                context.SaveChanges();

                if (TieneDetalle == 1)
                {
                    DETAILS_TICKET objTicketDetalle = db.DETAILS_TICKET.Find(_editACT.DETAILS_TICKETS);
                    objTicketDetalle.COM_DETA_TICK = COMENTARIO;
                    db.Entry(objTicketDetalle).State = EntityState.Modified;
                    db.SaveChanges();
                }

                SendMail mail = new SendMail();
                try
                {
                    mail.SendMailEditActivity(_editACT.EMAIL, _editACT.USERID, _editACT);
                }
                catch (Exception e)
                {
                    string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                    return Content("<script type='text/javascript'> function init() {" +
                                                                "if(top.uploadDone) top.uploadDone('ERROR','Error al enviar correo, Contáctese con el administrador.');}window.onload=init;</script>");
                }



            }

            return Content("<script type='text/javascript'>  function init() {" +
                                                    "if(top.uploadDoneEditActivity) top.uploadDoneEditActivity('OK','Su actividad se ha Actualizado correctamente.');}window.onload=init;</script>");

        }
        public ActionResult RemoveActivity(int id = 0)
        {
            try
            {
                using (var context = new EntityEntities())
                {
                    ACTIVITY_LOG act_log = context.ACTIVITY_LOG.Find(id);
                    act_log.USERID = Convert.ToInt32(Session["UserId"]);
                    act_log.MODIFIED_ACT_LOG = DateTime.Now;
                    act_log.VIG_ACTI_LOG = false;
                    context.Entry(act_log).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return Content("OK");
            }
            catch (Exception)
            {
                return Content("ERROR");
            }
        }
        #endregion

        #region Metodos Auxiliares
        public ActionResult ExportarListActivity()
        {
            int id = Convert.ToInt32(Session["UserId"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string PALABRA_CLAVE = Convert.ToString(Request.Params["PALABRA_CLAVE"]);
            string ID_CLIE = Convert.ToString(Request.Params["ID_CLIE"]);
            string ID_ENTI_FILTER = Convert.ToString(Request.Params["ID_ENTI_FILTER"]);
            DateTime SIN_DATE = Convert.ToDateTime(Request.Params["SIN_DATE"]);
            DateTime TO_DATE = Convert.ToDateTime(Request.Params["TO_DATE"]);
            using (var context = new EntityEntities())
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                var listActivityOperation = context.LIST_ACTIVITY_SUPERVISOR(ID_PERS_ENTI, ID_ACCO);
                var query = (from LAO in listActivityOperation
                             select new
                             {
                                 ID_ACTI_LOG = LAO.ID_ACTI_LOG,
                                 NAME_USER = LAO.NAME_USER,
                                 ID_CLIE = LAO.ID_CLIE,
                                 COM_NAME = LAO.COM_NAME,
                                 DES_ACT = LAO.DES_ACT,
                                 DES_SUB_TYPE = LAO.DES_SUB_TYPE,
                                 NAM_SUB_TYPE = LAO.NAM_SUBTYPE_ACT,
                                 DATE_INIC = (LAO.DATE_INIC == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", Convert.ToDateTime(LAO.DATE_INIC))),
                                 DATE_END = (LAO.DATE_END == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", Convert.ToDateTime(LAO.DATE_END))),
                                 COMENTARIO = StripHtml(LAO.COMENTARIO),
                                 ID_PERS_ENTI = LAO.ID_PERS_ENTI,
                                 TIEMPO_ACT = LAO.TIEMPO_ACT,
                                 CATEGORIA3 = LAO.CATEGORIA3,
                                 CATEGORIA4 = LAO.CATEGORIA4
                             });
                if (!string.IsNullOrEmpty(ID_CLIE))
                {
                    query = query.Where(x => x.ID_CLIE == Convert.ToInt32(ID_CLIE));
                }
                if (!string.IsNullOrEmpty(ID_ENTI_FILTER))
                {
                    query = query.Where(x => x.ID_PERS_ENTI == Convert.ToInt32(ID_ENTI_FILTER));
                }
                if (SIN_DATE != DateTime.MinValue)
                {
                    query = query.Where(x => Convert.ToDateTime(x.DATE_INIC) >= Convert.ToDateTime(SIN_DATE));
                }
                if (TO_DATE.Date.AddHours(23).AddMinutes(59).AddSeconds(59) != DateTime.MinValue)
                {
                    query = query.Where(x => Convert.ToDateTime(x.DATE_INIC) <= TO_DATE.Date.AddHours(23).AddMinutes(59).AddSeconds(59));
                }
                if (!string.IsNullOrEmpty(PALABRA_CLAVE))
                {
                    query = query.Where(x => x.COM_NAME.ToUpper().Contains(PALABRA_CLAVE.Trim().ToUpper()) || x.NAME_USER.ToUpper().Contains(PALABRA_CLAVE.ToUpper()) || x.DES_ACT.ToUpper().Contains(PALABRA_CLAVE.ToUpper()) || x.NAM_SUB_TYPE.ToUpper().Contains(PALABRA_CLAVE.ToUpper()) || x.COMENTARIO.ToUpper().Contains(PALABRA_CLAVE.ToUpper()));
                }
                var result = (from q in query.ToList()
                                  select new
                                  {
                                      CLIENTE = q.COM_NAME,
                                      NAME_USER = q.NAME_USER,
                                      TIPO = q.DES_ACT,
                                      SUBTIPO = q.DES_SUB_TYPE,
                                      DETALLE = q.NAM_SUB_TYPE,
                                      CATEGORIA3 = q.CATEGORIA3,
                                      CATEGORIA4 = q.CATEGORIA4,
                                      FECHAINICIO = q.DATE_INIC,
                                      FECHAFIN = q.DATE_END,
                                      TOTALHORAS = Convert.ToDateTime(q.DATE_END).Subtract(Convert.ToDateTime(q.DATE_INIC)),
                                      COMENTARIO = q.COMENTARIO
                                  }).OrderByDescending(x => x.FECHAINICIO);
                //var result = query.ToList().OrderByDescending(x => x.DATE_END);
                //var result = listresult.OrderByDescending(x => x.FECHAINICIO);
                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = result;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=ListaActividades" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.ContentType = "application/ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                return View();
            }
        }

        private string StripHtml(string source)
        {
            string output;

            //get rid of HTML tags
            output = Regex.Replace(source, "<[^>]*>", string.Empty);

            //get rid of multiple blank lines
            //output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);
            output = output.Replace("&aacute;", "á");
            output = output.Replace("&eacute;", "é");
            output = output.Replace("&iacute;", "í");
            output = output.Replace("&oacute;", "ó");
            output = output.Replace("&uacute;", "ú");
            output = output.Replace("&Aacute;", "Á");
            output = output.Replace("&Eacute;", "É");
            output = output.Replace("&Iacute;", "Í");
            output = output.Replace("&Oacute;", "Ó");
            output = output.Replace("&Uacute;", "Ú");
            output = output.Replace("&ntilde;", "ñ");
            output = output.Replace("&Ntilde;", "Ñ");
            output = output.Replace("&nbsp;", " ");
            output = output.Replace("&lt;", "<");
            output = output.Replace("&gt;", ">");
            output = output.Replace("&amp;", "&");

            return output;
        }

        public bool ValidateUser()
        {
            bool PermisosUser = false;
            using (var context = new EntityEntities())
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                //var query = context.LIST_PERSON_ACTIVITY_SUPERVISOR(ID_PERS_ENTI, ID_ACCO).ToList();
                var query = context.PERS_BY_PERS(ID_PERS_ENTI, ID_ACCO).ToList();
                if (query.Count > 0)
                {
                    PermisosUser = true;
                }
            }
            return PermisosUser;
        }
        #endregion

        #region Reportes

        public ActionResult ListClientEnginerAll(DateTime inic, DateTime fin, int user)
        {
            try
            {
                fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                using (var context = new EntityEntities())
                {
                    if (user != 0)
                    {
                        var Equipo = dbe.PERS_BY_PERS(ID_PERS_ENTI, ID_ACCO).ToList();

                        var ClientEnginer = (from x in context.REPORT_CLIENT_ENGINER(inic, fin, 0, user, ID_ACCO).ToList()
                                             join e in Equipo on x.ID_PERS_ENTI equals e.ID_PERS_ENTI
                                             select new
                                             {
                                                 ID_PERS_ENTI = x.ID_PERS_ENTI,
                                                 ID_CLIE = x.ID_CLIE,
                                                 CLIENTE = x.CLIENTE,
                                                 ID_TYPE_ACT = x.ID_TYPE_ACT,
                                                 TIPO = x.TIPO,
                                                 USER = x.USUARIO,
                                                 HORAS = x.HORAS
                                             });

                        var ClientEnginerImplementacion = from x in context.REPORT_CLIENT_ENGINER(inic, fin, 1, user, ID_ACCO)
                                                          join e in Equipo on x.ID_PERS_ENTI equals e.ID_PERS_ENTI
                                                          select new
                                                          {
                                                              ID_PERS_ENTI = x.ID_PERS_ENTI,
                                                              ID_CLIE = x.ID_CLIE,
                                                              CLIENTE = x.CLIENTE,
                                                              ID_TYPE_ACT = x.ID_TYPE_ACT,
                                                              TIPO = x.TIPO,
                                                              USER = x.USUARIO,
                                                              HORAS = x.HORAS
                                                          };

                        var ClientEnginerSExterno = from x in context.REPORT_CLIENT_ENGINER(inic, fin, 2, user, ID_ACCO)
                                                    join e in Equipo on x.ID_PERS_ENTI equals e.ID_PERS_ENTI
                                                    select new
                                                    {
                                                        ID_PERS_ENTI = x.ID_PERS_ENTI,
                                                        ID_CLIE = x.ID_CLIE,
                                                        CLIENTE = x.CLIENTE,
                                                        ID_TYPE_ACT = x.ID_TYPE_ACT,
                                                        TIPO = x.TIPO,
                                                        USER = x.USUARIO,
                                                        HORAS = x.HORAS
                                                    };
                        var ClientEnginerSInterno = from x in context.REPORT_CLIENT_ENGINER(inic, fin, 3, user, ID_ACCO)
                                                    join e in Equipo on x.ID_PERS_ENTI equals e.ID_PERS_ENTI
                                                    select new
                                                    {
                                                        ID_PERS_ENTI = x.ID_PERS_ENTI,
                                                        ID_CLIE = x.ID_CLIE,
                                                        CLIENTE = x.CLIENTE,
                                                        ID_TYPE_ACT = x.ID_TYPE_ACT,
                                                        TIPO = x.TIPO,
                                                        USER = x.USUARIO,
                                                        HORAS = x.HORAS
                                                    };
                        var ClientEnginerComercial = from x in context.REPORT_CLIENT_ENGINER(inic, fin, 4, user, ID_ACCO)
                                                     join e in Equipo on x.ID_PERS_ENTI equals e.ID_PERS_ENTI
                                                     select new
                                                     {
                                                         ID_PERS_ENTI = x.ID_PERS_ENTI,
                                                         ID_CLIE = x.ID_CLIE,
                                                         CLIENTE = x.CLIENTE,
                                                         ID_TYPE_ACT = x.ID_TYPE_ACT,
                                                         TIPO = x.TIPO,
                                                         USER = x.USUARIO,
                                                         HORAS = x.HORAS
                                                     };
                        var ClientEnginerCapac = from x in context.REPORT_CLIENT_ENGINER(inic, fin, 6, user, ID_ACCO)
                                                 join e in Equipo on x.ID_PERS_ENTI equals e.ID_PERS_ENTI
                                                 select new
                                                 {
                                                     ID_PERS_ENTI = x.ID_PERS_ENTI,
                                                     ID_CLIE = x.ID_CLIE,
                                                     CLIENTE = x.CLIENTE,
                                                     ID_TYPE_ACT = x.ID_TYPE_ACT,
                                                     TIPO = x.TIPO,
                                                     USER = x.USUARIO,
                                                     HORAS = x.HORAS
                                                 };
                        var ClientEnginerOtros = from x in context.REPORT_CLIENT_ENGINER(inic, fin, 8, user, ID_ACCO)
                                                 join e in Equipo on x.ID_PERS_ENTI equals e.ID_PERS_ENTI
                                                 select new
                                                 {
                                                     ID_PERS_ENTI = x.ID_PERS_ENTI,
                                                     ID_CLIE = x.ID_CLIE,
                                                     CLIENTE = x.CLIENTE,
                                                     ID_TYPE_ACT = x.ID_TYPE_ACT,
                                                     TIPO = x.TIPO,
                                                     USER = x.USUARIO,
                                                     HORAS = x.HORAS
                                                 };
                        return Json(new { ClientEnginer = ClientEnginer, ClientEnginerImplementacion = ClientEnginerImplementacion, ClientEnginerSExterno = ClientEnginerSExterno, ClientEnginerSInterno = ClientEnginerSInterno, ClientEnginerComercial = ClientEnginerComercial, ClientEnginerCapac = ClientEnginerCapac, ClientEnginerOtros = ClientEnginerOtros }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var ClientEnginer = (from x in context.REPORT_CLIENT_ENGINER_ALL(inic, fin, 0, ID_ACCO).ToList()
                                             select new
                                             {
                                                 USUARIOS = x.USUARIOS,
                                                 ID_CLIE = x.ID_CLIE,
                                                 CLIENTE = x.CLIENTE,
                                                 HORAS = x.HORAS
                                             });

                        var ClientEnginerImplementacion = from x in context.REPORT_CLIENT_ENGINER_ALL(inic, fin, 1, ID_ACCO)
                                                          select new
                                                          {
                                                              USUARIOS = x.USUARIOS,
                                                              ID_CLIE = x.ID_CLIE,
                                                              CLIENTE = x.CLIENTE,
                                                              HORAS = x.HORAS
                                                          };

                        var ClientEnginerSExterno = from x in context.REPORT_CLIENT_ENGINER_ALL(inic, fin, 2, ID_ACCO)
                                                    select new
                                                    {
                                                        USUARIOS = x.USUARIOS,
                                                        ID_CLIE = x.ID_CLIE,
                                                        CLIENTE = x.CLIENTE,
                                                        HORAS = x.HORAS
                                                    };
                        var ClientEnginerSInterno = from x in context.REPORT_CLIENT_ENGINER_ALL(inic, fin, 3, ID_ACCO)
                                                    select new
                                                    {
                                                        USUARIOS = x.USUARIOS,
                                                        ID_CLIE = x.ID_CLIE,
                                                        CLIENTE = x.CLIENTE,
                                                        HORAS = x.HORAS
                                                    };
                        var ClientEnginerComercial = from x in context.REPORT_CLIENT_ENGINER_ALL(inic, fin, 4, ID_ACCO)
                                                     select new
                                                     {
                                                         USUARIOS = x.USUARIOS,
                                                         ID_CLIE = x.ID_CLIE,
                                                         CLIENTE = x.CLIENTE,
                                                         HORAS = x.HORAS
                                                     };
                        var ClientEnginerCapac = from x in context.REPORT_CLIENT_ENGINER_ALL(inic, fin, 6, ID_ACCO)
                                                 select new
                                                 {
                                                     USUARIOS = x.USUARIOS,
                                                     ID_CLIE = x.ID_CLIE,
                                                     CLIENTE = x.CLIENTE,
                                                     HORAS = x.HORAS
                                                 };
                        var ClientEnginerOtros = from x in context.REPORT_CLIENT_ENGINER_ALL(inic, fin, 8, ID_ACCO)
                                                 select new
                                                 {
                                                     USUARIOS = x.USUARIOS,
                                                     ID_CLIE = x.ID_CLIE,
                                                     CLIENTE = x.CLIENTE,
                                                     HORAS = x.HORAS
                                                 };
                        return Json(new { ClientEnginer = ClientEnginer, ClientEnginerImplementacion = ClientEnginerImplementacion, ClientEnginerSExterno = ClientEnginerSExterno, ClientEnginerSInterno = ClientEnginerSInterno, ClientEnginerComercial = ClientEnginerComercial, ClientEnginerCapac = ClientEnginerCapac, ClientEnginerOtros = ClientEnginerOtros }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListTypeActivityAll(DateTime inic, DateTime fin, string area)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            string[] strAreas = area.Split(',');
            int[] Areas = new int[10]; int contador = 0;

            foreach(string strArea in strAreas){
                Areas[contador] = Convert.ToInt32(strArea);
                contador++;
            }
            if (area == "0") area = "";

            if (ID_PERS_ENTI == 25069) { ID_PERS_ENTI = 381; }//Visualizacion de RenzoCobos

            using (var context = new EntityEntities())
            {
                
                //var Equipo = dbe.PERS_BY_PERS(ID_PERS_ENTI, ID_ACCO).ToList();
                #region ListEnginer
                var ListEnginer = context.ListEnginer(ID_PERS_ENTI, ID_ACCO, inic, fin, area).ToList();
                #endregion

                #region ListEnginerImplementacion y ListEnginerImpAll
                ///////////////////////////Se mantiene el linq por el uso del json//////////////
                var ListEnginerImpAll = context.REPORT_TYPE_ACTIVITY(inic, fin, 1, ID_ACCO).ToList();

                if (area != "0")
                {
                    ListEnginerImpAll = ListEnginerImpAll.Where(x => Areas.Contains((int)x.ID_CHAR_AREA)).ToList();
                }
                ////////////////////////////////////////////////////////////////////////////////
                var ListEnginerImplementacion = context.ListEnginerImplementacion(ID_PERS_ENTI,ID_ACCO,inic,fin,area).ToList();

                #endregion

                #region ListEnginerSExterno

                var ListEnginerSExterno = context.ListEnginerSExterno(ID_PERS_ENTI, ID_ACCO, inic, fin, area).ToList();

                #endregion

                #region ListEnginerSinterno

                var ListEnginerSInterno = context.ListEnginerSInterno(ID_PERS_ENTI, ID_ACCO, inic, fin, area).ToList();
                #endregion

                #region ListEnginerComercial

                var ListEnginerComercial = context.ListEnginerComercial(ID_PERS_ENTI, ID_ACCO, inic, fin, area).ToList();
                #endregion

                #region ListEnginerCapac

                var ListEnginerCapac = context.ListEnginerCapacitacion(ID_PERS_ENTI, ID_ACCO, inic, fin, area).ToList();
                #endregion
                

                #region ListEnginerOtros

                var ListEnginerOtros = context.ListEnginerOtros(ID_PERS_ENTI, ID_ACCO, inic, fin, area).ToList();
                #endregion

                return Json(new
                {
                    ListEnginer = ListEnginer,
                    ListEnginerImplementacion = ListEnginerImplementacion,
                    ListEnginerImpAll = ListEnginerImpAll,
                    ListEnginerSExterno = ListEnginerSExterno,
                    ListEnginerSInterno = ListEnginerSInterno,
                    ListEnginerComercial = ListEnginerComercial,
                    ListEnginerCapac = ListEnginerCapac,
                    ListEnginerOtros = ListEnginerOtros
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListCuentas(DateTime inic, DateTime fin)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            if (ID_PERS_ENTI == 25069) { ID_PERS_ENTI = 381; }//Visualizacion de RenzoCobos

            using (var context = new EntityEntities())
            {
                #region ListCuentas
                var ListCuentas = context.ListCuentas(ID_PERS_ENTI, 0, inic, fin).ToList();
                #endregion

                return Json(new
                {
                    ListCuentas = ListCuentas
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListOpImplementationAll(DateTime inic, DateTime fin, int area)
        {
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            using (var context = new EntityEntities())
            {
                var ListOP = context.REPORT_OP_ACTIVITY_ALL(inic, fin).ToList();

                if (area != 0)
                {
                    ListOP = ListOP.Where(x => x.ID_CHAR_AREA == area).ToList();
                }
                var ListOPAll = (from x in ListOP.ToList()
                                 group x by new { x.NAM_SUBTYPE_ACT } into g
                                 select new
                                 {
                                     OP_ALL = g.Key.NAM_SUBTYPE_ACT,
                                     TOTAL = g.Sum(x => x.TOTAL).ToString()
                                 }).ToList().OrderBy(x => x.OP_ALL);

                var ListInf = (from x in ListOP.ToList().Where(x => x.ID_CHAR_AREA == 7)
                                           group x by new { x.NAM_SUBTYPE_ACT } into g
                                           select new
                                           {
                                               OP_ALL = g.Key.NAM_SUBTYPE_ACT,
                                               TOTAL = g.Sum(x => x.TOTAL).ToString()
                                           }).ToList().OrderBy(x => x.OP_ALL);

                var ListArq = (from x in ListOP.ToList().Where(x => x.ID_CHAR_AREA == 1200 || x.ID_CHAR_AREA == 1203)
                                           group x by new { x.NAM_SUBTYPE_ACT } into g
                                           select new
                                           {
                                               OP_ALL = g.Key.NAM_SUBTYPE_ACT,
                                               TOTAL = g.Sum(x => x.TOTAL).ToString()
                                           }).ToList().OrderBy(x => x.OP_ALL);
                var ListVyS = (from x in ListOP.ToList().Where(x => x.ID_CHAR_AREA == 1236)
                               group x by new { x.NAM_SUBTYPE_ACT } into g
                               select new
                               {
                                   OP_ALL = g.Key.NAM_SUBTYPE_ACT,
                                   TOTAL = g.Sum(x => x.TOTAL).ToString()
                               }).ToList().OrderBy(x => x.OP_ALL);

                var ListNwk = (from x in ListOP.ToList().Where(x => x.ID_CHAR_AREA == 1239)
                               group x by new { x.NAM_SUBTYPE_ACT } into g
                               select new
                               {
                                   OP_ALL = g.Key.NAM_SUBTYPE_ACT,
                                   TOTAL = g.Sum(x => x.TOTAL).ToString()
                               }).ToList().OrderBy(x => x.OP_ALL);
                return Json(new
                {
                    ListOPAll = ListOPAll,
                    ListInf = ListInf,
                    ListArq = ListArq,
                    ListVyS = ListVyS,
                    ListNwk = ListNwk

                }, JsonRequestBehavior.AllowGet);
            }
        }
        #region ListarClientes por Ingeniero
        public ActionResult ListEnginerxClieImp(DateTime inic, DateTime fin, string usr, int tipo)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            using (var context = new EntityEntities())
            {
                var ListEnginerImp = (from x in context.REPORT_TYPE_ACTIVITY(inic, fin, tipo, ID_ACCO).Where(x => x.USUARIO.Contains(usr))
                                      where x.IMPLEMENTACION > 0
                                      select new
                                      {
                                          x.CLIENTE,
                                          x.IMPLEMENTACION
                                      }).ToList();
                return Json(new { ListEnginerImp = ListEnginerImp }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListEnginerxClieSEx(DateTime inic, DateTime fin, string usr, int tipo)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            using (var context = new EntityEntities())
            {
                var ListEnginerExt = (from x in context.REPORT_TYPE_ACTIVITY(inic, fin, tipo, ID_ACCO).Where(x => x.USUARIO.Contains(usr))
                                      where x.SOPORTEEXT > 0
                                      select new
                                      {
                                          x.CLIENTE,
                                          x.SOPORTEEXT
                                      }).ToList();
                return Json(new { ListEnginerExt = ListEnginerExt }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListEnginerxClieSIn(DateTime inic, DateTime fin, string usr, int tipo)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            using (var context = new EntityEntities())
            {
                var ListEnginerIn = (from x in context.REPORT_TYPE_ACTIVITY(inic, fin, tipo, ID_ACCO).Where(x => x.USUARIO.Contains(usr))
                                     where x.SOPORTEINT > 0
                                     select new
                                     {
                                         x.CLIENTE,
                                         x.SOPORTEINT
                                     }).ToList();
                return Json(new { ListEnginerIn = ListEnginerIn }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListEnginerxClieSCOM(DateTime inic, DateTime fin, string usr, int tipo)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            using (var context = new EntityEntities())
            {
                var ListEnginerCOM = (from x in context.REPORT_TYPE_ACTIVITY(inic, fin, tipo, ID_ACCO).Where(x => x.USUARIO.Contains(usr))
                                      where x.COMERCIAL > 0
                                      select new
                                      {
                                          x.CLIENTE,
                                          x.COMERCIAL
                                      }).ToList();
                return Json(new { ListEnginerCOM = ListEnginerCOM }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListEnginerxClieSCAP(DateTime inic, DateTime fin, string usr, int tipo)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            using (var context = new EntityEntities())
            {
                var ListEnginerCAP = (from x in context.REPORT_TYPE_ACTIVITY(inic, fin, tipo, ID_ACCO).Where(x => x.USUARIO.Contains(usr))
                                      where x.CAPACITACION > 0
                                      select new
                                      {
                                          x.CLIENTE,
                                          x.CAPACITACION
                                      }).ToList();
                return Json(new { ListEnginerCAP = ListEnginerCAP }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListEnginerxClieSOTR(DateTime inic, DateTime fin, string usr, int tipo)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            fin = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            using (var context = new EntityEntities())
            {
                var ListEnginerOTR = (from x in context.REPORT_TYPE_ACTIVITY(inic, fin, tipo, ID_ACCO).Where(x => x.USUARIO.Contains(usr))
                                      where x.OTROS > 0
                                      select new
                                      {
                                          x.CLIENTE,
                                          x.OTROS
                                      }).ToList();
                return Json(new { ListEnginerOTR = ListEnginerOTR }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        //Listar Areas de Operaciones
        public ActionResult ListAreasOperaciones()
        {
            using (var contexto = new EntityEntities())
            {
                var query = contexto.CHARTs.Where(x => x.ID_CHAR == 1200 || x.ID_CHAR == 7 || x.ID_CHAR == 1236);
                var result = from x in query.ToList()
                             join c in contexto.NAME_CHART on x.ID_NAM_CHAR equals c.ID_NAM_CHAR
                             select new
                             {
                                 ID_CHART = x.ID_CHAR,
                                 ID_NAM_CHAR = c.NAM_CHAR
                             };

                return Json(new { Data = result.ToList().OrderBy(x => x.ID_NAM_CHAR), Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ListarAreas()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            using (var contexto = new EntityEntities())
            {
                List<OrgObtenerOrganigramaxTipo_Result> query = contexto.OrgObtenerOrganigramaxTipo(ID_PERS_ENTI, 2, 4).ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult ListarAreaxOrganigrama(string q, string page)
        {

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = (from x in dbe.OrgObtenerOrganigramaxTipo(ID_PERS_ENTI, 2, 4).ToList()
                              select new
                              {
                                  id = x.ID_CHAR,
                                  text = x.NAM_CHAR
                              }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text);

           return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ListarAreaxOrganigramaMultiSelect(string q, string page)
        {

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = (from x in dbe.OrgObtenerOrganigramaxTipo(ID_PERS_ENTI, 2, 4).ToList()
                          select new
                          {
                              opcion = "<option value='"+ x.ID_CHAR + "'>"+ x.NAM_CHAR + "</option>",
                              id = x.ID_CHAR,
                              text = x.NAM_CHAR
                          }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarAreaxOrganigramaCbx(string q, string page)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = (from x in dbe.OrgObtenerOrganigramaxTipo(ID_PERS_ENTI, 2, 4).ToList()
                          select new
                          {
                              id = x.ID_CHAR,
                              text = x.NAM_CHAR
                          }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Vistas Reportes
        [Authorize]
        public ActionResult ReportClientEnginer()
        {
            return View();
        }
        [Authorize]
        public ActionResult ReportTimeActivity()
        {
            ViewBag.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            return View();
        }
        [Authorize]
        public ActionResult ReportTypeActivity()
        {
            return View();
        }
        [Authorize]
        public ActionResult ReportOPImplementation()
        {
            return View();
        }
        [Authorize]
        public ActionResult ReportExternalSupport()
        {
            return View();
        }
        [Authorize]
        public ActionResult ReporteActividad()
        {
            return View();
        }

        [Authorize]
        public ActionResult ReporteActividadxIngeniero()
        {
            return View();
        }

        [Authorize]
        public ActionResult RptActividadCliente()
        {
            return View();
        }
        [Authorize]
        public ActionResult RptActividadIngeniero()
        {
            return View();
        }
        #endregion

        public ActionResult ReporteActividadSemanal()
        {
            return View();
        }

        public ActionResult ListarHorasAreaUsuario()
        {
            string FechaInicial = Convert.ToString(Request.Params["FechaInicio"].ToString());
            int IdPersEnti = 0;
            if (Request.Params["Usuario"] != "")
                IdPersEnti = Convert.ToInt32(Request.Params["Usuario"].ToString());
            var result = dbe.ActListarHorasAreaUsuario(FechaInicial, IdPersEnti).ToList();

            return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarPersonalxArea(int id)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            
            var result = dbe.ActListarPersonalxArea(ID_PERS_ENTI, ID_ACCO, id, "");
            var result1 = dbe.ActListarPersonalxArea(ID_PERS_ENTI, ID_ACCO, id, "").ToList();
            if (dbe.ValidarPersonalxUsuario(ID_PERS_ENTI).Single().Cantidad == 0)
            {
                var resultU = result.Where(x => x.id == ID_PERS_ENTI);
                return Json(new { Data = resultU }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ValidarPersonalxUsuario()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var result = dbe.ValidarPersonalxUsuario(ID_PERS_ENTI);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteActividadSemanalxIngeniero()
        {
            return View();
        }
        public ActionResult ReporteTipoActividades()
        {

            ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
            return View();
        }

        public ActionResult RptActividadSemanalxIngeniero()
        {

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            DateTime FechaInicial = Convert.ToDateTime(Request.Params["FechaInicio"].ToString());
            string Areas = Convert.ToString(Request.Params["IdArea"].ToString());

            var resultado = dbe.ActividadxSemana(ID_PERS_ENTI, FechaInicial, Areas);
   
            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarTipoActividad(string q, string page)
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

            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            List<ListarTipoActividad_Result> resultado = dbe.ListarTipoActividad(IdAcco, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoActividadCbx(string q, string page)
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

            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            List<ListarTipoActividad_Result> resultado = dbe.ListarTipoActividad(IdAcco, termino).ToList();
            return Json(new { Data = resultado }, JsonRequestBehavior.AllowGet);

        }
        
        //Tableros
        public ActionResult ConsolidadoActividades()
        {
            return View();
        }
        public ActionResult ConsolidadoActivid()
        {
            return View();
        }



        //VISTA PAR MINSUR,
        public ActionResult DetailsActivityMinsur()
        {
            return View();
        }

        //FORMULARIO ESPECIAL PARA MINSUR,MARCOBRE Y RAURA
        public ActionResult ListCompanyByAccount()
        {

            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);


            var result = dbe.ListarEmpresasMinsur().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarTablaActividadesMinsur()
        {
            int id_coordinador = Convert.ToInt32(Session["COORDINADOR_SERVICEDESK_MINSUR"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);



            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            int sw = 0, Creador = 0, TipoDocumento = 0, ID_PERS_STAT = 0, IdQueu = 0, IdPersEntiAssi = 0;
            string msjError = string.Empty;
            DateTime? FechaInicio = null, FechaFin = null;
            if (int.TryParse(Convert.ToString(Request.Params["ID_ENTI"]), out Creador) == false)
            {
                Creador = 0;
            }
            else if (Convert.ToString(Request.Params["ID_ENTI"]) == "65966")
            {
                Creador = 56;
            }
            else if (Convert.ToString(Request.Params["ID_ENTI"]) == "70889")
            {
                Creador = 57;
            }
            else if (Convert.ToString(Request.Params["ID_ENTI"]) == "70890")
            {
                Creador = 58;
            }


            if (string.IsNullOrEmpty(Convert.ToString(Session["ID_PERS_ENTI"])))
            {
                ID_PERS_ENTI = 0;
            }
            else
            {
                ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            }
            if (int.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out IdQueu) == false)
            {
                IdQueu = 0;
            }

            if (int.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out IdPersEntiAssi) == false)
            {
                IdPersEntiAssi = 0;
            }




            if (Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"], cultures);
            }

            if (Convert.ToString(Request.QueryString["FechaIngresoFin"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoFin"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"], cultures);
            }

            var result = dbe.ListarTablaActividadesMinsur(Creador, ID_PERS_ENTI, IdQueu, IdPersEntiAssi, FechaInicio, FechaFin, id_coordinador).ToList();
            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        // EXPORTAR EXCEL COMPLETO CON TODOS LOS DATOS.
        public ActionResult ExportarExcelMinsurMarcobreRaura()
        {
            int id_coordinador = Convert.ToInt32(Session["COORDINADOR_SERVICEDESK_MINSUR"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int sw = 0, Creador = 0, TipoDocumento = 0, ID_PERS_STAT = 0, IdQueu = 0, IdPersEntiAssi = 0;
            string msjError = string.Empty;
            DateTime? FechaInicio = null, FechaFin = null;
            if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE"]), out Creador) == false)
            {
                Creador = 0;
            }
            else if (Convert.ToString(Request.Params["ID_CLIE"]) == "65966")
            {
                Creador = 56;
            }
            else if (Convert.ToString(Request.Params["ID_CLIE"]) == "70889")
            {
                Creador = 57;
            }
            else if (Convert.ToString(Request.Params["ID_CLIE"]) == "70890")
            {
                Creador = 58;
            }

            if (int.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out IdQueu) == false)
            {
                IdQueu = 0;
            }

            if (int.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out IdPersEntiAssi) == false)
            {
                IdPersEntiAssi = 0;
            }

            if (Convert.ToString(Request.Params["SIN_DATE"]) == "" || Convert.ToString(Request.Params["SIN_DATE"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaInicio = Convert.ToDateTime(Request.Params["SIN_DATE"], cultures);
            }

            if (Convert.ToString(Request.Params["TO_DATE"]) == "" || Convert.ToString(Request.Params["TO_DATE"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaFin = Convert.ToDateTime(Request.Params["TO_DATE"], cultures);
            }

            List<ExportarExcelActividadesMinsurRauraMarcobre_Result> query = dbe.ExportarExcelActividadesMinsurRauraMarcobre(Creador, ID_PERS_ENTI, IdQueu, IdPersEntiAssi, FechaInicio, FechaFin, id_coordinador).ToList();

            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Hoja1");

            // Establece los encabezados de la tabla
            worksheet.Cell(1, 1).Value = "CLIENTE";
            worksheet.Cell(1, 2).Value = "TICKET";
            worksheet.Cell(1, 3).Value = "ESPECIALISTA";
            worksheet.Cell(1, 4).Value = "TIPO DE ACTIVIDAD";
            worksheet.Cell(1, 5).Value = "COMENTARIO DESCRIPCION";
            worksheet.Cell(1, 6).Value = "TIEMPO";
            worksheet.Cell(1, 7).Value = "FECHA INICIO";
            worksheet.Cell(1, 8).Value = "FECHA FIN";

            // Agrega los datos de la tabla
            for (int i = 0; i < query.Count; i++)
            {
                string comentario = query[i].ComentarioDescripcion ?? ""; // Si el valor es nulo, asigna una cadena vacía en su lugar
                string comentarioSinHTML = System.Text.RegularExpressions.Regex.Replace(comentario, "<.*?>", string.Empty);
                worksheet.Cell(i + 2, 5).Value = comentarioSinHTML;
                worksheet.Cell(i + 2, 1).Value = query[i].Cliente;
                worksheet.Cell(i + 2, 2).Value = query[i].Ticket;
                worksheet.Cell(i + 2, 3).Value = query[i].Especialista;
                worksheet.Cell(i + 2, 4).Value = query[i].TipodeActividad;
                worksheet.Cell(i + 2, 5).Value = comentarioSinHTML;
                worksheet.Cell(i + 2, 6).Value = query[i].Tiempo;
                worksheet.Cell(i + 2, 7).Value = query[i].FechaInicio;
                worksheet.Cell(i + 2, 8).Value = query[i].FechaFin;
            }

            // Descarga el archivo en el navegador
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Charset = "";
            Response.AddHeader("content-disposition", "attachment;filename=listadocumentos" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();

            return View();
        }


        // LISTAR AREA MINSUR RAURA Y MARCOBRE
        public ActionResult ListarAreaMinsurMarcobreReura()
        {
            var result = db.ListarAreaMinsurMarcobreRaura().ToList();
            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AssigneByQueue(string q, string page)
        {
            string termino = "";
            int ID_QUEU = 0;
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (NAM_PAR == "ID_QUEU")
            {
                i1 = "0";
                ID_QUEU = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            else
            {
                i1 = "1";
                ID_QUEU = Convert.ToInt32(Request.Params["AreaResp"]);
            }
            //Parametros termino, ID_ACCO, ID_QUEUE

            var query = dbe.AsignadosPorColaMinsurMarcobreRaura(ID_QUEU).ToList();


            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        
        
        //FIN PARA VISTA MINSUR
    }
}

