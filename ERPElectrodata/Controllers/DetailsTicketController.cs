using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Configuration;
using ERPElectrodata.Objects;
using System.Threading;
using System.Globalization;
using ERPElectrodata.Object;
using ERPElectrodata.Object.Ticket;
using System.Web.Security;
using Electrodata.ERPElectrodata.Domain.Services.LeccionAprendidaService;
using Electrodata.ERPElectrodata.Infra.Reposotories.LeccionAprendidaRepositorie;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using System.Text;
using ERPElectrodata.Models.Enum;
using RestSharp;
using System.Threading.Tasks;

namespace ERPElectrodata.Controllers
{
    [Authorize]
    public class DetailsTicketController : Controller
    {

        public LeccionAprendidaService _srvLeccionAprendidaService = new LeccionAprendidaService(new LeccionAprendidaRepositorie());
        int result = (int)Tipo_Peracion.OPERATION_ERROR;
        private CoreEntities db = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private PointTicket pt = new PointTicket();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        public TicketIR tir = new TicketIR();
        /*variables globales gestión del conocimiento*/
        int nivel1 = 0, nivel2 = 0, nivel3 = 0, nivel4 = 0, idTema = 0;
        string nameNivel1 = string.Empty, nameNivel2 = string.Empty, nameNivel3 = string.Empty, nameNivel4 = string.Empty, nameTema = string.Empty;
        bool Flagproblema;
        /*Fin*/

        //
        // GET: /DetailsTicket/ListByIdTick


        #region "gestión Problemas"
        public JsonResult UpdateDetailsTicket(int idTicket = 0)
        {
            var data = db.ComLeccionAprendidas.Where(x => x.ID_TICKET == idTicket && x.SolucionDefinitiva == "").FirstOrDefault();
            Electrodata.ERPElectrodata.Domain.Entities.ComLeccionAprendida objLeccionAprendida = new Electrodata.ERPElectrodata.Domain.Entities.ComLeccionAprendida();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion






        public ActionResult UpdateDatesDetailsTicket(int id)
        {
            DETAILS_TICKET query = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == id);
            ViewBag.COM_DETA_TICK_EDIT = query.COM_DETA_TICK;
            ViewBag.FEC_SCHE_EDIT = query.FEC_SCHE;
            ViewBag.ID_STAT_EDIT = query.ID_STAT;
            ViewBag.ID_TYPE_DETA_TICK_EDIT = query.ID_TYPE_DETA_TICK;
            ViewBag.CREATE_DETA_INCI_EDIT = query.CREATE_DETA_INCI;
            ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
            ViewBag.PortalComent = Convert.ToBoolean(query.PortalComent);
            ViewBag.FechaInicio = query.FROM_TIME;
            ViewBag.FechaFin = query.TO_TIME;
            ViewBag.MinEfectivos = query.MinutosBolsaHoras;
            
            if (Convert.ToInt32(Session["ID_ACCO"]) == 3 && query.ID_STAT == 4)
            {
                ViewBag.FEC_END_REAL_EDIT = query.FEC_END_REAL;
            }
            ViewBag.Habilitado = Convert.ToInt16(Session["SUPERVISOR_SERVICEDESK"]);
            return View(query);
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateDatesDetailsTicket(IEnumerable<HttpPostedFileBase> archivos, DETAILS_TICKET detatick)
        {
            try
            {
                int ID_DETA_TICK = Convert.ToInt32(detatick.ID_DETA_TICK);
                string COM_DETA_TICK = Convert.ToString(Request.Params["COM_DETA_TICK_EDIT"]);
                string STR_STAT_EDIT = Convert.ToString(Request.Params["ID_STAT_EDIT"]);
                int ID_TYPE_DETA_TICK = Convert.ToInt32(Request.Params["ID_TYPE_DETA_TICK_EDIT"]);
                DateTime CREATE_DETA_INCI = Convert.ToDateTime(Request.Params["CREATE_DETA_INCI_EDIT"]);
                int SECOND_CREATE_DETA_INCI = Convert.ToInt32(Request.Params["SECOND_CREATE_DETA_INCI_EDIT"]);
                int MILLISECOND_CREATE_DETA_INCI = Convert.ToInt32(Request.Params["MILLISECOND_CREATE_DETA_INCI_EDIT"]);
                bool PortalComent = Convert.ToBoolean(detatick.PortalComent);
                int ID_TICK = Convert.ToInt32(detatick.ID_TICK);
                int MinutosBolsa = Convert.ToInt32(Request.Params["MinEfectivos"]);
                string KEY_ATTA = null;
                try
                {
                    KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);

                }
                catch
                {

                }

                CREATE_DETA_INCI = CREATE_DETA_INCI.AddSeconds(SECOND_CREATE_DETA_INCI).AddMilliseconds(MILLISECOND_CREATE_DETA_INCI);

                DETAILS_TICKET query = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);

                string FEC_REAL = Convert.ToString(Request.Params["FEC_END_REAL_EDIT"]);
                DateTime FEC_END_REAL;
                if (!string.IsNullOrEmpty(FEC_REAL))
                {
                    FEC_END_REAL = Convert.ToDateTime(Request.Params["FEC_END_REAL_EDIT"]);
                    query.FEC_END_REAL = FEC_END_REAL;
                }

                int ID_STAT_EDIT = 0;
                if (!string.IsNullOrEmpty(STR_STAT_EDIT))
                {
                    ID_STAT_EDIT = Convert.ToInt32(STR_STAT_EDIT);
                }

                if (String.IsNullOrEmpty(COM_DETA_TICK.Trim()))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Ingrese un Comentario');}window.onload=init;</script>");
                }
                try
                {
                    DateTime FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"]);
                    DateTime fechaFin = Convert.ToDateTime(Request.Params["FechaFin"]);
                    ACTIVITY_LOG queryAct = dbe.ACTIVITY_LOG.Single(x => x.DETAILS_TICKETS == ID_DETA_TICK);
                    queryAct.DATE_INIC = FechaInicio;
                    queryAct.DATE_END = fechaFin;
                    queryAct.TIEMPO_ACT = (int)(Convert.ToDateTime(fechaFin).Subtract(Convert.ToDateTime(FechaInicio))).TotalSeconds;
                    dbe.Entry(queryAct).State = EntityState.Modified;
                    dbe.SaveChanges();
                }
                catch
                {
                }
                if (query.UserId == 34)
                {
                    query.COM_DETA_TICK = COM_DETA_TICK;
                    query.CREATE_DETA_INCI = CREATE_DETA_INCI;
                    query.PortalComent = PortalComent;
                }
                else
                {
                    query.COM_DETA_TICK = COM_DETA_TICK;
                    query.CREATE_DETA_INCI = CREATE_DETA_INCI;
                    query.PortalComent = PortalComent;
                    DateTime FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"]);
                    DateTime fechaFin = Convert.ToDateTime(Request.Params["FechaFin"]);
                    query.FROM_TIME = FechaInicio;
                    query.TO_TIME = fechaFin;
                }

                if (ID_STAT_EDIT == 5 && ID_TYPE_DETA_TICK == 3)
                {
                    DateTime FEC_SCHE = Convert.ToDateTime(Request.Params["FEC_SCHE_EDIT"]);
                    int SECOND_FEC_SCHE = Convert.ToInt32(Request.Params["SECOND_FEC_SCHE_EDIT"]);
                    int MILLISECOND_FEC_SCHE = Convert.ToInt32(Request.Params["MILLISECOND_FEC_SCHE_EDIT"]);
                    FEC_SCHE = FEC_SCHE.AddSeconds(SECOND_FEC_SCHE).AddMilliseconds(MILLISECOND_FEC_SCHE);

                    query.FEC_SCHE = FEC_SCHE;
                }

                TICKET ticket = db.TICKETs.Where(x=> x.ID_TICK == query.ID_TICK).FirstOrDefault();
                ticket.MODIFIED_TICK = DateTime.Now;
                ticket.MinutosEfectivos = (ticket.MinutosEfectivos - query.MinutosBolsaHoras) + MinutosBolsa;

                query.MinutosBolsaHoras = MinutosBolsa;

                db.Entry(query).State = EntityState.Modified;
                db.SaveChanges();
                if (KEY_ATTA != null)
                {
                    var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                        .Where(x => x.ID_INCI == null).ToList();
                    if (Attachs.Count() > 0)
                    {
                        foreach (ATTACHED attach in Attachs)
                        {
                            try
                            {
                                var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                                EObj.ID_DETA_INCI = query.ID_DETA_TICK;
                                db.Entry(EObj).State = EntityState.Modified;
                                db.SaveChanges();
                                db.Entry(EObj).State = EntityState.Detached;
                            }
                            catch
                            {

                            }

                        }
                    }
                }

                if (ID_TYPE_DETA_TICK == 3)
                {
                    db.UPDATE_MINUTES_TICKET(ID_TICK);

                }


                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEditDeta) top.uploadDoneEditDeta('OK','La información ha sido actualizada.');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEditDeta) top.uploadDoneEditDeta('ERROR','Contacte al Administrador');}window.onload=init;</script>");

            }
        }

        public ActionResult ListByIdTick(int id)
        {
            int idTicket = 0;
            textInfo = cultureInfo.TextInfo;

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = db.ObtenerDetallesTicket(id, ID_ACCO).ToList();

            if (ID_ACCO == 4)
            {
                var ticket = db.ObtenerTicketIndexEdata(id).ToList();

                ticket.FirstOrDefault().ATTA = AttaTick(id);
                ticket.FirstOrDefault().EXP_TIME = tir.ExpirationTime(id, ObtenerHoraSLA(id));
                ticket.FirstOrDefault().ATTA_TOT = TotAtta(id);
                /*Gestión Probl*/
                var dataPreLeccionAprendida = db.ComLeccionAprendidas.Join(db.CATEGORies, cl => cl.Nivel4, c4 => c4.ID_CATE, (cl, c4) => new
                {
                    idLeccionAprendida = cl.IdLeccioNAprendida,
                    IdTicket = cl.ID_TICKET,
                    Titulo = cl.Titulo,
                    Categoria = c4.NAM_CATE,
                    Descripcion = cl.DescripcionProblema,
                    SolucionAplicada = cl.SolucionAplicada,
                    Impacto = cl.Impactonegocio,
                    SolucionTemporal = cl.SolucionTemporal,
                    SolucionDefinitiva = cl.SolucionDefinitiva,
                    EstadoAprobacion = cl.EstadoAprobacion,
                    IdCuenta = cl.ID_ACCO
                }).Where(l => l.IdCuenta == ID_ACCO);

                var listaLeccion = dataPreLeccionAprendida.Where(x => x.IdTicket == id && x.EstadoAprobacion == "A").ToList();

                if (listaLeccion.Count() != 0)
                    idTicket = Convert.ToInt32(listaLeccion.FirstOrDefault().IdTicket);

                var estadoTicket = db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TICK == idTicket && t.ID_STAT_END == 4).FirstOrDefault();


                return Json(new { data = result, datadeta = ticket, listaLeccionAprendida = estadoTicket != null ? listaLeccion : null }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var ticket = db.ObtenerTicketIndex(id).ToList();

                ticket.FirstOrDefault().ATTA = AttaTick(id);
                ticket.FirstOrDefault().EXP_TIME = tir.ExpirationTime(id, ObtenerHoraSLA(id));
                ticket.FirstOrDefault().ATTA_TOT = TotAtta(id);
                /*Gestión Probl*/
                var dataPreLeccionAprendida = db.ComLeccionAprendidas.Join(db.CATEGORies, cl => cl.Nivel4, c4 => c4.ID_CATE, (cl, c4) => new
                {
                    idLeccionAprendida = cl.IdLeccioNAprendida,
                    IdTicket = cl.ID_TICKET,
                    Titulo = cl.Titulo,
                    Categoria = c4.NAM_CATE,
                    Descripcion = cl.DescripcionProblema,
                    SolucionAplicada = cl.SolucionAplicada,
                    Impacto = cl.Impactonegocio,
                    SolucionTemporal = cl.SolucionTemporal,
                    SolucionDefinitiva = cl.SolucionDefinitiva,
                    EstadoAprobacion = cl.EstadoAprobacion,
                    IdCuenta = cl.ID_ACCO
                }).Where(l => l.IdCuenta == ID_ACCO);

                var listaLeccion = dataPreLeccionAprendida.Where(x => x.IdTicket == id && x.EstadoAprobacion == "A").ToList();

                if (listaLeccion.Count() != 0)
                    idTicket = Convert.ToInt32(listaLeccion.FirstOrDefault().IdTicket);

                var estadoTicket = db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TICK == idTicket && t.ID_STAT_END == 4).FirstOrDefault();
                /*Fin*/


                return Json(new { data = result, datadeta = ticket, listaLeccionAprendida = estadoTicket != null ? listaLeccion : null }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult ObtenerProgresoSLA(int id)
        {

            var progreso = db.ObtenerProgresoSLA(id).FirstOrDefault();

            return Json(progreso, JsonRequestBehavior.AllowGet);
        }

        public string timeusedtick(int ID_TICK)
        {
            int minutos = 0, horas = 0, dias = 0;
            try
            {
                var query = db.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                minutos = Convert.ToInt32(query.MINUTES);
                if (minutos >= 60)
                {
                    horas = minutos / 60;
                    minutos = minutos % 60;
                }
                else
                {
                    horas = 0;

                }
            }
            catch
            {
                horas = 0;
                minutos = 0;

            }
            return horas + " Hours " + minutos + " Minutes";
        }

        public string Ammount(int ID_TICK, int ID_QUEU, decimal AMMO)
        {
            if (ID_QUEU == 26)
            {
                try
                {
                    var xx = db.DETAILS_TICKET
                        .Where(x => x.ID_TICK == ID_TICK)
                        .Where(x => x.ID_STAT_SALE_OPP != null)
                        .OrderByDescending(x => x.ID_DETA_TICK)
                        .First();
                    return Convert.ToString(string.Format("{0:0,0.00}", xx.AMM_SALE_OPP.Value));
                }
                catch
                {
                    return Convert.ToString(string.Format("{0:0,0.00}", AMMO));
                }
            }
            else
            {
                return "0";
            }
        }

        public string Profit(int ID_TICK, int ID_QUEU, decimal AMMO)
        {
            if (ID_QUEU == 26)
            {
                try
                {
                    var xx = db.DETAILS_TICKET
                        .Where(x => x.ID_TICK == ID_TICK)
                        .Where(x => x.ID_STAT_SALE_OPP != null)
                        .OrderByDescending(x => x.ID_DETA_TICK)
                        .First();
                    return Convert.ToString(string.Format("{0:0,0.00}", xx.PRO_SALE_OPP.Value));
                }
                catch
                {
                    return Convert.ToString(string.Format("{0:0,0.00}", 0));
                }
            }
            else
            {
                return "0";
            }
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

        //[Authorize]
        public ActionResult Index(int id = 0)
        {
            //try
            //{

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);


            if (ID_ACCO == 4)
            {
                ViewBag.ID_QUEU_TAREA = Convert.ToInt32(Session["ID_QUEU"]);
                ViewBag.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                var DETA_TICK = new DETAILS_TICKET();
                DETA_TICK.ID_TICK = id;
                string queue = "";
                string reportadoPor = "";
                ViewBag.DATE = String.Format("{0:g}", DateTime.Now);
                ViewBag.UploadFile = "M1DT" + Convert.ToString(DateTime.Now.Ticks);
                int permisoOperaciones = 0;
                var categoria4 = "";
                var categoria5 = "";
                var categoria6 = "";
                //boton para registrar atencion de analista
                int respuesta = Convert.ToInt32(db.HabilitarBtnRegistroAtencion(ID_ACCO, ID_PERS_ENTI, id).FirstOrDefault().Value);
                ViewBag.BotonRegistroAtencion = respuesta;

                var Acco = db.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO);
                var ticket = (from t in db.TICKETs.Where(t => t.ID_TICK == id).ToList()
                              join tt in db.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                              select new
                              {
                                  t.ID_TICK,
                                  t.ID_TYPE_TICK,
                                  t.COD_TICK,
                                  NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                                  t.ID_QUEU,
                                  t.AMM_SALE_OPPO,
                                  t.SUM_TICK,
                                  t.ID_COMP,
                                  t.ID_STAT_END,
                                  t.ID_CATE,
                                  t.ID_TICK_PARENT,
                                  t.IS_PARENT,
                                  t.ID_DOCU_SALE,
                                  t.TITLE_TICK,
                                  t.FechaInicioReal,
                                  t.IdRazon,
                              }).First();
                try
                {
                    var ticketQueue = (from t in db.TICKETs.Where(t => t.ID_TICK == id).ToList()
                                       join qu in db.QUEUEs on t.ID_QUEU equals qu.ID_QUEU
                                       select new
                                       {

                                           t.ID_TICK,
                                           qu.NAM_QUEU_REPO
                                       }).First();
                    queue = ticketQueue.NAM_QUEU_REPO;
                }
                catch (Exception e)
                {

                }

                try
                {
                    var ticketReportadoPor = (from t in db.TICKETs.Where(t => t.ID_TICK == id).ToList()
                                              join ur in db.GrupoReportadors on t.IdGrupoReportador equals ur.IdGrupoReportador
                                              select new
                                              {
                                                  t.ID_TICK,
                                                  ur.Nombre
                                              }).First();
                    reportadoPor = ticketReportadoPor.Nombre;
                }
                catch (Exception e)
                {

                }

                int existeCambio = 0;
                int existeCambioInc = 0;
                int idTicketParent = Convert.ToInt32(ticket.ID_TICK_PARENT);
                if (ticket.IS_PARENT == false)
                {
                    existeCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == idTicketParent).Count();
                }
                try
                {
                    existeCambioInc = db.CHANGE_REQUEST.Where(x => x.IdTicket == id).Count();
                }
                catch
                {
                    existeCambioInc = 0;
                }
                var cate = db.CATEGORies.Single(x => x.ID_CATE == ticket.ID_CATE.Value);
                ViewBag.AplicaGestionActivo = Convert.ToInt32(cate.AplicaGestionActivos);
                ViewBag.time = Acco.TIME_MANUAL;
                ViewBag.NAM_TYPE_TICK = ticket.NAM_TYPE_TICK;
                ViewBag.COD_TICK = ticket.COD_TICK;
                ViewBag.ID_QUEU = ticket.ID_QUEU;
                ViewBag.ID_TICK = id;
                ViewBag.SUM_TICK = ticket.SUM_TICK;
                ViewBag.ID_COMP = ticket.ID_COMP;
                ViewBag.Estado = ticket.ID_STAT_END;
                ViewBag.ExisteCambio = existeCambio;
                ViewBag.ExisteCambioInc = existeCambioInc;
                ViewBag.UserId = Session["UserId"];
                ViewBag.NOM_AREA = queue;
                ViewBag.TITLE_TICK = ticket.TITLE_TICK;
                ViewBag.FechaInicioReal = ticket.FechaInicioReal;
                ViewBag.IdRazon = ticket.IdRazon;
                ViewBag.GrupoReportador = reportadoPor;
                ViewBag.ID_CATE_MOVIL = ticket.ID_CATE;
                ViewBag.TieneTarea = Convert.ToInt32(cate.TieneTarea ?? false);
                ViewBag.Impacto = db.TICKETs.Single(x => x.ID_TICK == id).ImpactoServicio;
                ViewBag.GarantiaProveedor = db.TICKETs.Single(x => x.ID_TICK == id).GarantiaProveedor;

                int cateConTareas = 0;

                ViewBag.CategoriaConTareas = cateConTareas;
                //Tiene Soporte Electrodata
                var soporte = (db.TICKETs.Single(x => x.ID_TICK == id).IdMantED);
                int tieneSoporte = 0;
                //if (soporte != null)
                //{
                int idDocuSale = Convert.ToInt32(db.TICKETs.Where(x => x.ID_TICK == id).Single().IdProyectoSLA);
                if (idDocuSale == 0)
                {

                }
                else
                {
                    var queryOP = db.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == idDocuSale);
                    var queryTipoOP = db.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == queryOP.ID_TYPE_DOCU_SALE.Value);
                    string numOP = queryTipoOP.NAM_TYPE_DOCU_SALE + " " + queryOP.NUM_DOCU_SALE;
                    tieneSoporte = 1;
                    ViewBag.IdDocuSale = idDocuSale;
                    ViewBag.NumDocuSale = numOP;
                }

                //}
                ViewBag.TieneSoporte = tieneSoporte;

                int IdProyectoSLA = Convert.ToInt32(db.TICKETs.Where(x => x.ID_TICK == id).Single().IdProyectoSLA);
                ViewBag.IdProyectoSLA = IdProyectoSLA;

                //Aplica Gestión de Cambios
                int AplicaCambio = 0;
                if (ticket.ID_TYPE_TICK == 1 || ticket.ID_TYPE_TICK == 2)
                {
                    AplicaCambio = 1;
                }
                ViewBag.AplicaCambio = AplicaCambio;
                int IdCambio = 0;
                try
                {

                    if (existeCambio == 0)
                    {
                        if (existeCambioInc == 1)
                        {
                            IdCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == id).SingleOrDefault().id;
                        }
                        else
                        {
                            IdCambio = 0;
                        }
                    }
                    else
                    {
                        IdCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == idTicketParent).SingleOrDefault().id;
                    }
                }
                catch
                {
                    IdCambio = 0;
                }

                ViewBag.IdCambio = IdCambio;
                if (ticket.ID_QUEU.Value == 26)
                {
                    try
                    {
                        var xx = db.DETAILS_TICKET
                            .Where(x => x.ID_TICK == id)
                            .Where(x => x.ID_STAT_SALE_OPP != null)
                            .OrderByDescending(x => x.ID_DETA_TICK)
                            .First();

                        DETA_TICK.AMM_SALE_OPP = xx.AMM_SALE_OPP.Value;
                        DETA_TICK.PRO_SALE_OPP = xx.PRO_SALE_OPP.Value;
                    }
                    catch
                    {
                        DETA_TICK.AMM_SALE_OPP = ticket.AMM_SALE_OPPO;
                        DETA_TICK.PRO_SALE_OPP = 0;
                    }
                }

                ViewBag.Estadito = "Estadito";


                if (Convert.ToInt32((Session["SERVICEDESK"].ToString())) == 1)
                {
                    ViewBag.ACCESO_EDIT = 1;
                }
                if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1)
                {
                    ViewBag.ACCESO_SEND_SURVEY = 1;
                    ViewBag.ACCESO_EDIT_DETAIL = 1;
                }


                var nroleccionAprendida = (from la in db.ComLeccionAprendidas.Where(x => x.ID_TICKET == id).ToList()
                                           select new
                                           {
                                               titulo = la.Titulo,
                                               cuenta = la.DescripcionProblema
                                           }).ToList();


                string Titulo, DescripcionProblema, SolucionAplicada, Impactonegocio, SolucionTemporal, SolucionDefinitiva,
                    Porque2 = "", Porque3 = "", Porque4 = "", Porque5 = "";
                if (nroleccionAprendida.Count() > 0)
                {
                    int idTicket = ticket.ID_TICK;
                    var leccionAprendida = db.ComLeccionAprendidas.Single(t => t.ID_TICKET == idTicket);
                    Titulo = leccionAprendida.Titulo;
                    DescripcionProblema = leccionAprendida.DescripcionProblema;
                    SolucionAplicada = leccionAprendida.SolucionAplicada;
                    Impactonegocio = leccionAprendida.Impactonegocio;
                    SolucionTemporal = leccionAprendida.SolucionTemporal;
                    SolucionDefinitiva = leccionAprendida.SolucionDefinitiva;
                    Porque2 = leccionAprendida.Porque2;
                    Porque3 = leccionAprendida.Porque3;
                    Porque4 = leccionAprendida.Porque4;
                    Porque5 = leccionAprendida.Porque5;
                }
                else
                {
                    Titulo = "";
                    DescripcionProblema = "";
                    SolucionAplicada = "";
                    Impactonegocio = "";
                    SolucionTemporal = "";
                    SolucionDefinitiva = "";
                }

                ViewBag.Titulo = Titulo;
                ViewBag.DescripcionProblema = DescripcionProblema;
                ViewBag.SolucionAplicada = SolucionAplicada;
                ViewBag.Impactonegocio = Impactonegocio;
                ViewBag.SolucionTemporal = SolucionTemporal;
                ViewBag.SolucionDefinitiva = SolucionDefinitiva;
                ViewBag.Porque2 = Porque2;
                ViewBag.Porque3 = Porque3;
                ViewBag.Porque4 = Porque4;
                ViewBag.Porque5 = Porque5;
                //Valida si es ticket proyecto
                if (!(ticket.ID_DOCU_SALE is null))
                    ViewBag.TicketProyecto = 1;
                else
                    ViewBag.TicketProyecto = 0;
                /*Gestión Problemas*/
                var tipoTicket = db.TICKETs.Where(x => x.FlagProblema == true && x.ID_TICK == id).FirstOrDefault();

                if (tipoTicket != null)
                    return View("IndexProblema", DETA_TICK);
                else

                    ViewBag.IdTicket = id;
                var DatosEnvio = db.DatosEnvioCorreoFinal(id).FirstOrDefault();
                ViewBag.Para = DatosEnvio.Para;
                ViewBag.Cc = DatosEnvio.Cc;
                ViewBag.Asunto = DatosEnvio.Asunto;
                return View("IndexEData");

            }
            else
            {
                ViewBag.ID_QUEU_TAREA = Convert.ToInt32(Session["ID_QUEU"]);
                ViewBag.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var DETA_TICK = new DETAILS_TICKET();
                DETA_TICK.ID_TICK = id;
                string queue = "";
                string reportadoPor = "";
                ViewBag.DATE = String.Format("{0:g}", DateTime.Now);
                ViewBag.UploadFile = "M1DT" + Convert.ToString(DateTime.Now.Ticks);
                int permisoOperaciones = 0;
                var categoria4 = "";
                var categoria5 = "";
                var categoria6 = "";
                if (ID_ACCO == 60)
                {
                    ViewBag.PorcentajeTicket = db.ObtenerPorcentajeTicket(id).First();
                }
                else
                {
                    ViewBag.PorcentajeTicket = 0;
                }
                var Acco = db.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO);
                var ticket = (from t in db.TICKETs.Where(t => t.ID_TICK == id).ToList()
                              join tt in db.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                              select new
                              {
                                  t.ID_TICK,
                                  t.ID_TYPE_TICK,
                                  t.COD_TICK,
                                  NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                                  t.ID_QUEU,
                                  t.AMM_SALE_OPPO,
                                  t.SUM_TICK,
                                  t.ID_COMP,
                                  t.ID_STAT_END,
                                  t.ID_CATE,
                                  t.ID_TICK_PARENT,
                                  t.IS_PARENT,
                                  t.ID_DOCU_SALE,
                                  t.TITLE_TICK,
                                  t.FechaInicioReal,
                                  t.IdRazon,
                              }).First();
                try
                {
                    var ticketQueue = (from t in db.TICKETs.Where(t => t.ID_TICK == id).ToList()
                                       join qu in db.QUEUEs on t.ID_QUEU equals qu.ID_QUEU
                                       select new
                                       {

                                           t.ID_TICK,
                                           qu.NAM_QUEU_REPO
                                       }).First();
                    queue = ticketQueue.NAM_QUEU_REPO;
                }
                catch (Exception e)
                {

                }

                try
                {
                    var ticketReportadoPor = (from t in db.TICKETs.Where(t => t.ID_TICK == id).ToList()
                                              join ur in db.GrupoReportadors on t.IdGrupoReportador equals ur.IdGrupoReportador
                                              select new
                                              {
                                                  t.ID_TICK,
                                                  ur.Nombre
                                              }).First();
                    reportadoPor = ticketReportadoPor.Nombre;
                }
                catch (Exception e)
                {

                }

                int existeCambio = 0;
                int existeCambioInc = 0;
                int idTicketParent = Convert.ToInt32(ticket.ID_TICK_PARENT);
                if (ticket.IS_PARENT == false)
                {
                    existeCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == idTicketParent).Count();
                }
                try
                {
                    existeCambioInc = db.CHANGE_REQUEST.Where(x => x.IdTicket == id).Count();
                }
                catch
                {
                    existeCambioInc = 0;
                }
                var cate = db.CATEGORies.Single(x => x.ID_CATE == ticket.ID_CATE.Value);
                ViewBag.AplicaGestionActivo = Convert.ToInt32(cate.AplicaGestionActivos);
                ViewBag.time = Acco.TIME_MANUAL;
                ViewBag.NAM_TYPE_TICK = ticket.NAM_TYPE_TICK;
                ViewBag.COD_TICK = ticket.COD_TICK;
                ViewBag.ID_QUEU = ticket.ID_QUEU;
                ViewBag.ID_TICK = id;
                ViewBag.SUM_TICK = ticket.SUM_TICK;
                ViewBag.ID_COMP = ticket.ID_COMP;
                ViewBag.Estado = ticket.ID_STAT_END;
                ViewBag.ExisteCambio = existeCambio;
                ViewBag.ExisteCambioInc = existeCambioInc;
                ViewBag.UserId = Session["UserId"];
                ViewBag.NOM_AREA = queue;
                ViewBag.TITLE_TICK = ticket.TITLE_TICK;
                ViewBag.FechaInicioReal = ticket.FechaInicioReal;
                ViewBag.IdRazon = ticket.IdRazon;
                ViewBag.GrupoReportador = reportadoPor;
                ViewBag.ID_CATE_MOVIL = ticket.ID_CATE;
                ViewBag.TieneTarea = Convert.ToInt32(cate.TieneTarea ?? false);
                ViewBag.Impacto = db.TICKETs.Single(x => x.ID_TICK == id).ImpactoServicio;
                ViewBag.GarantiaProveedor = db.TICKETs.Single(x => x.ID_TICK == id).GarantiaProveedor;

                int cateConTareas = 0;
                //MINSUR
                if (ID_ACCO == (int)Minsur.MINSUR || ID_ACCO == (int)Minsur.RAURA || ID_ACCO == (int)Minsur.MARCOBRE)
                {
                    CategoriasxIdCateTicket_Result listaCat = db.CategoriasxIdCateTicket(ticket.ID_CATE).First();
                    categoria4 = listaCat.CATEGORIA_4;
                    categoria5 = listaCat.CATEGORIA_5;
                    categoria6 = listaCat.CATEGORIA_6;

                    ViewBag.Cate1 = listaCat.CATEGORIA_1;
                    ViewBag.Cate2 = listaCat.CATEGORIA_2;
                    ViewBag.Cate3 = listaCat.CATEGORIA_3;
                    if (listaCat.CATEGORIA_4 != "")
                    {
                        ViewBag.Cate4 = listaCat.CATEGORIA_4;
                    }
                    else
                    {
                        ViewBag.Cate4 = "-";
                    }

                    ViewBag.Cate5 = listaCat.CATEGORIA_5;
                    ViewBag.Cate6 = listaCat.CATEGORIA_6;
                    ViewBag.ID_CATE_MINSUR = ticket.ID_CATE;

                    //Tareas
                    var tarea = db.CategoriaConTareas.FirstOrDefault(x => x.IdCate == ticket.ID_CATE && x.IdAcco == ID_ACCO);
                    cateConTareas = tarea != null ? 1 : 0;
                }
                ViewBag.CategoriaConTareas = cateConTareas;
                //Tiene Soporte Electrodata
                var soporte = (db.TICKETs.Single(x => x.ID_TICK == id).IdMantED);
                int tieneSoporte = 0;
                if (soporte != null)
                {
                    int idDocuSale = Convert.ToInt32(db.TICKETs.Single(x => x.ID_TICK == id).ID_DOCU_SALE);
                    var queryOP = db.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == idDocuSale);
                    var queryTipoOP = db.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == queryOP.ID_TYPE_DOCU_SALE.Value);
                    string numOP = queryTipoOP.NAM_TYPE_DOCU_SALE + " " + queryOP.NUM_DOCU_SALE;
                    tieneSoporte = 1;
                    ViewBag.IdDocuSale = idDocuSale;
                    ViewBag.NumDocuSale = numOP;
                }
                ViewBag.TieneSoporte = tieneSoporte;
                //Aplica Gestión de Cambios
                int AplicaCambio = 0;
                if (ticket.ID_TYPE_TICK == 1 || ticket.ID_TYPE_TICK == 2)
                {
                    AplicaCambio = 1;
                }
                ViewBag.AplicaCambio = AplicaCambio;
                int IdCambio = 0;
                try
                {
                    //if (existeCambio == 1)
                    //    IdCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == idTicketParent).SingleOrDefault().id;
                    //else
                    //    IdCambio = 0; 
                    //if (existeCambioInc == 1)
                    //    IdCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == id).SingleOrDefault().id;
                    //else
                    //    IdCambio = 0;

                    if (existeCambio == 0)
                    {
                        if (existeCambioInc == 1)
                        {
                            IdCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == id).SingleOrDefault().id;
                        }
                        else
                        {
                            IdCambio = 0;
                        }
                    }
                    else
                    {
                        IdCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == idTicketParent).SingleOrDefault().id;
                    }
                }
                catch
                {
                    IdCambio = 0;
                }

                ViewBag.IdCambio = IdCambio;
                if (ticket.ID_QUEU.Value == 26)
                {
                    try
                    {
                        var xx = db.DETAILS_TICKET
                            .Where(x => x.ID_TICK == id)
                            .Where(x => x.ID_STAT_SALE_OPP != null)
                            .OrderByDescending(x => x.ID_DETA_TICK)
                            .First();

                        DETA_TICK.AMM_SALE_OPP = xx.AMM_SALE_OPP.Value;
                        DETA_TICK.PRO_SALE_OPP = xx.PRO_SALE_OPP.Value;
                    }
                    catch
                    {
                        DETA_TICK.AMM_SALE_OPP = ticket.AMM_SALE_OPPO;
                        DETA_TICK.PRO_SALE_OPP = 0;
                    }
                }

                ViewBag.Estadito = "Estadito";


                if (Convert.ToInt32((Session["SERVICEDESK"].ToString())) == 1)
                {
                    ViewBag.ACCESO_EDIT = 1;
                }
                if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1)
                {
                    ViewBag.ACCESO_SEND_SURVEY = 1;
                    ViewBag.ACCESO_EDIT_DETAIL = 1;
                }

                /*string UserName = Convert.ToString(Session["UserName"]);
                string[] rolesArray = Roles.GetRolesForUser(UserName);

                foreach (string xc in rolesArray)
                {
                    int i = Array.IndexOf(rolesArray, xc);
                    if (xc == "SERVICE DESK")
                    {
                        ViewBag.ACCESO_EDIT = 1;
                    }
                    if (xc == "ADMINISTRADOR")
                    {
                        ViewBag.ACCESO_SEND_SURVEY = 1;
                        ViewBag.ACCESO_EDIT_DETAIL = 1;
                    }

                }*/

                //ViewBag.ACCESO_EDIT = Convert.ToInt32(Session["SUPERVISOR_SERVICEDESK"]);
                //Leccion Aprendida

                var nroleccionAprendida = (from la in db.ComLeccionAprendidas.Where(x => x.ID_TICKET == id).ToList()
                                           select new
                                           {
                                               titulo = la.Titulo,
                                               cuenta = la.DescripcionProblema
                                           }).ToList();


                string Titulo, DescripcionProblema, SolucionAplicada, Impactonegocio, SolucionTemporal, SolucionDefinitiva,
                    Porque2 = "", Porque3 = "", Porque4 = "", Porque5 = "";
                if (nroleccionAprendida.Count() > 0)
                {
                    int idTicket = ticket.ID_TICK;
                    var leccionAprendida = db.ComLeccionAprendidas.Single(t => t.ID_TICKET == idTicket);
                    Titulo = leccionAprendida.Titulo;
                    DescripcionProblema = leccionAprendida.DescripcionProblema;
                    SolucionAplicada = leccionAprendida.SolucionAplicada;
                    Impactonegocio = leccionAprendida.Impactonegocio;
                    SolucionTemporal = leccionAprendida.SolucionTemporal;
                    SolucionDefinitiva = leccionAprendida.SolucionDefinitiva;
                    Porque2 = leccionAprendida.Porque2;
                    Porque3 = leccionAprendida.Porque3;
                    Porque4 = leccionAprendida.Porque4;
                    Porque5 = leccionAprendida.Porque5;
                }
                else
                {
                    Titulo = "";
                    DescripcionProblema = "";
                    SolucionAplicada = "";
                    Impactonegocio = "";
                    SolucionTemporal = "";
                    SolucionDefinitiva = "";
                }

                ViewBag.Titulo = Titulo;
                ViewBag.DescripcionProblema = DescripcionProblema;
                ViewBag.SolucionAplicada = SolucionAplicada;
                ViewBag.Impactonegocio = Impactonegocio;
                ViewBag.SolucionTemporal = SolucionTemporal;
                ViewBag.SolucionDefinitiva = SolucionDefinitiva;
                ViewBag.Porque2 = Porque2;
                ViewBag.Porque3 = Porque3;
                ViewBag.Porque4 = Porque4;
                ViewBag.Porque5 = Porque5;
                //Valida si es ticket proyecto
                if (!(ticket.ID_DOCU_SALE is null))
                    ViewBag.TicketProyecto = 1;
                else
                    ViewBag.TicketProyecto = 0;
                /*Gestión Problemas*/
                var tipoTicket = db.TICKETs.Where(x => x.FlagProblema == true && x.ID_TICK == id).FirstOrDefault();

                if (tipoTicket != null)
                    return View("IndexProblema", DETA_TICK);
                else
                    return View(DETA_TICK);

                /*Fin*/
                //}
                //catch
                //{
                //    return Content("Please Close Conecction");
                //}
            }

        }

        public int TotAtta(int id)
        {
            int atta_tick = db.ATTACHEDs.Where(a => a.ID_INCI == id).Count();

            int atta_all = (from x in db.DETAILS_TICKET.Where(dt => dt.ID_TICK == id)
                            join adt in db.ATTACHEDs on x.ID_DETA_TICK equals adt.ID_DETA_INCI
                            select new
                            {
                                adt.ID_ATTA
                            }).Count();

            return atta_tick + atta_all;
        }

        public ActionResult AttaAllTick(int id)
        {

            try
            {
                var query = (from a in db.ATTACHEDs.Where(a => a.ID_INCI == id)
                             .Where(a => a.DELETE_ATTA == null || a.DELETE_ATTA == false)
                             join tda in db.TYPE_DOCUMENT_ATTACH on a.ID_TYPE_DOCU_ATTA equals tda.ID_TYPE_DOCU_ATTA into ltda
                             from xtda in ltda.DefaultIfEmpty()

                             select new
                             {
                                 NAM_ATTA = a.NAM_ATTA,
                                 ID_ATTA = a.ID_ATTA,
                                 EXT_ATTA = a.EXT_ATTA,
                                 NAM_TYPE_DOCU_ATTA = (xtda == null ? "No Type" : xtda.NAM_TYPE_DOCU_ATTA),
                             });

                var query2 = (from a in db.ATTACHEDs.Where(a => a.DELETE_ATTA == null || a.DELETE_ATTA == false)
                              join dt in db.DETAILS_TICKET.Where(x => x.ID_TICK == id) on a.ID_DETA_INCI equals dt.ID_DETA_TICK
                              join tda in db.TYPE_DOCUMENT_ATTACH on a.ID_TYPE_DOCU_ATTA equals tda.ID_TYPE_DOCU_ATTA into ltda
                              from xtda in ltda.DefaultIfEmpty()
                              select new
                              {
                                  //NAM_ATTA = a.NAM_ATTA,
                                  NAM_ATTA = a.NAM_ATTA,
                                  ID_ATTA = a.ID_ATTA,
                                  EXT_ATTA = a.EXT_ATTA,
                                  NAM_TYPE_DOCU_ATTA = (xtda == null ? "No Type" : xtda.NAM_TYPE_DOCU_ATTA),
                              }).Union(query);
                //var query2 = db.AttaAllTick(id);
                //var p = db.AttaAllTick(id).ToList();
                return Json(new { Data = query2, Count = query2.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "", Count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public string AttaDetaTick(int id)
        {
            string Adjun = "";
            try
            {
                var query = (from a in db.ATTACHEDs.Where(a => a.ID_DETA_INCI == id)
                             .Where(a => a.DELETE_ATTA == null || a.DELETE_ATTA == false)
                             join tda in db.TYPE_DOCUMENT_ATTACH on a.ID_TYPE_DOCU_ATTA equals tda.ID_TYPE_DOCU_ATTA into ltda
                             from xtda in ltda.DefaultIfEmpty()
                             select new
                             {
                                 NAM_ATTA = a.NAM_ATTA,
                                 ID_ATTA = a.ID_ATTA,
                                 EXT_ATTA = a.EXT_ATTA,
                                 NAM_TYPE_DOCU_ATTA = (xtda == null ? "" : xtda.NAM_TYPE_DOCU_ATTA),
                             });

                foreach (var subqu in query)
                {
                    //Adjun = Adjun + "<li><a href='../../Adjuntos/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA +" ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    //Adjun = Adjun + "<li><a href='/DetailsTicket/DescargarArchivo/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    if (subqu.EXT_ATTA.ToLower() == ".pdf" || subqu.EXT_ATTA.ToLower() == ".txt" || subqu.EXT_ATTA.ToLower() == ".png" || subqu.EXT_ATTA.ToLower() == ".jpg")
                    {
                        Adjun = Adjun + "<li><a href='/DetailsTicket/VerDocumento/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' target='_blank'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                    else
                    {
                        Adjun = Adjun + "<li><a href='/DetailsTicket/DescargarArchivo/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        public string AttaTick(int id)
        {
            string Adjun = "";
            try
            {
                var query = db.ObtenerAdjuntosTicket(id).ToList();

                foreach (var subqu in query)
                {
                    //Adjun = Adjun + "<li><a href='../../Adjuntos/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    if (subqu.EXT_ATTA.ToLower() == ".pdf" || subqu.EXT_ATTA.ToLower() == ".txt" || subqu.EXT_ATTA.ToLower() == ".png" || subqu.EXT_ATTA.ToLower() == ".jpg")
                    {
                        Adjun = Adjun + "<li><a href='/DetailsTicket/VerDocumento/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' target='_blank'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                    else
                    {
                        Adjun = Adjun + "<li><a href='/DetailsTicket/DescargarArchivo/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        public String ScheduleDate(int ID_TICK)
        {
            DateTime FechaScheduled;

            try
            {
                FechaScheduled = Convert.ToDateTime(db.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                                .Where(x => x.ID_STAT == 5)
                                .Where(x => x.ID_TYPE_DETA_TICK == 3)
                                .OrderByDescending(x => x.CREATE_DETA_INCI)
                                .Take(1).Single().FEC_SCHE);
            }
            catch
            {
                FechaScheduled = Convert.ToDateTime(db.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_INI_TICK);
            }

            return String.Format("{0:G}", FechaScheduled);
        }

        public String ExpTime(int id_stat_end, int id_prio, int hou_prio, DateTime date, int ID_TICK)
        {
            string stop = "Stopped";
            DateTime Fecha;
            int TimeTrans = 0, time = 0;

            if (id_prio != 5)
            {
                if (id_stat_end == 1 || id_stat_end == 3)
                {
                    try
                    {
                        Fecha = Convert.ToDateTime(db.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                                .Where(x => x.ID_STAT != null)
                                .OrderByDescending(x => x.CREATE_DETA_INCI)
                                .Take(1).Single().CREATE_DETA_INCI);
                    }
                    catch
                    {
                        Fecha = Convert.ToDateTime(db.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_TICK);
                    }

                    TimeTrans = Convert.ToInt32(db.TICKETs.Single(x => x.ID_TICK == ID_TICK).MINUTES);

                    //time = Convert.ToInt32(TimeTrans / 60) + DateTime.Now.Subtract(Fecha).Days * 24 + DateTime.Now.Subtract(Fecha).Hours;

                    int fm = (hou_prio * 60) - (Convert.ToInt32(TimeTrans) + (DateTime.Now.Subtract(Fecha).Days * 24 * 60) + (DateTime.Now.Subtract(Fecha).Hours * 60) + DateTime.Now.Subtract(Fecha).Minutes);

                    //int fm = hou_prio * 60 - TimeTrans;

                    //return Convert.ToString(Convert.ToInt32(hou_prio) - Convert.ToInt32(time))+
                    //    +" : "+
                    //    + " Hours";
                    return Convert.ToString(fm / 60) + ":" + Convert.ToString((fm % 60)) + " HH:mm";
                }
                else if (id_stat_end == 5)
                {
                    try
                    {
                        try
                        {
                            Fecha = Convert.ToDateTime(db.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                                .Where(x => x.ID_STAT == 5)
                                .OrderByDescending(x => x.CREATE_DETA_INCI)
                                .Take(1).Single().FEC_SCHE);

                            if (Fecha == DateTime.MinValue)
                            {
                                Fecha = Convert.ToDateTime(db.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_INI_TICK);
                            }
                        }
                        catch
                        {
                            Fecha = Convert.ToDateTime(db.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_INI_TICK);
                        }

                        TimeTrans = Convert.ToInt32(db.TICKETs.Single(x => x.ID_TICK == ID_TICK).MINUTES);

                        if (Fecha > DateTime.Now)
                        {
                            //time = TimeTrans/60;
                            time = TimeTrans;
                        }
                        else
                        {
                            stop = "Running";
                            //time = Convert.ToInt32(TimeTrans / 60) + DateTime.Now.Subtract(Fecha).Days * 24 + DateTime.Now.Subtract(Fecha).Hours;
                            time = (Convert.ToInt32(TimeTrans) + (DateTime.Now.Subtract(Fecha).Days * 24 * 60) + (DateTime.Now.Subtract(Fecha).Hours * 60) + DateTime.Now.Subtract(Fecha).Minutes);
                        }

                        int fm = (hou_prio * 60) - time;
                        return Convert.ToString(fm / 60) + ":" + Convert.ToString((fm % 60)) + " HH:mm Stop";
                        //return Convert.ToString(Convert.ToInt32(hou_prio) - Convert.ToInt32(time)) + " Hours "+stop;
                    }
                    catch
                    {
                        return stop;
                    }
                }
                else
                {
                    return stop;
                }
            }
            else
            {
                return "Planing";
            }

        }
        //
        // GET: /DetailsTicket/Details/5

        public ActionResult Details(int id = 0)
        {
            DETAILS_TICKET details_ticket = db.DETAILS_TICKET.Find(id);
            if (details_ticket == null)
            {
                return HttpNotFound();
            }
            return View(details_ticket);
        }

        //
        // GET: /DetailsTicket/Create
        [Authorize]
        public ActionResult AdjuntarArchivosComentario(string KEY_ATTA)
        {
            ViewBag.KEY_ATTA = KEY_ATTA;
            return View();
        }



        public ActionResult Create()
        {
            //ViewBag.ID_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE");
            //ViewBag.ID_TYPE_DETA_TICK = new SelectList(db.TYPE_DETAILS_TICKET, "ID_TYPE_DETA_TICK", "NAM_TYPE_DETA_TICK");
            //ViewBag.ID_TICK = new SelectList(db.TICKETs, "ID_TICK", "COD_TICK");
            return View();
        }

        //
        // POST: /DetailsTicket/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(DETAILS_TICKET details_ticket, IEnumerable<HttpPostedFileBase> files)
        {
            string impactoServicio = Request.Form["ImpactoServicio"];

            int UserId = Convert.ToInt32(Session["UserId"]);
            if (UserId == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.perdioSesion();}window.onload=init;</script>");
            }

            //         
            bool chkControlRemoto = Request.Params["ControlRemoto"].ToString() == "false" ? false : true;
            bool chkTipoResolucion = false;

            string estado = string.Empty;

            string KEY_ATTA = string.Empty;

            int ID_ACCOval = Convert.ToInt32(Session["ID_ACCO"]);

            if (impactoServicio != ""  && details_ticket.ID_STAT == 4 && ID_ACCOval == 60)
            {
                db.ActualizarImpactoBNV(details_ticket.ID_TICK, impactoServicio, 1, "Impacto del Servicio: " + impactoServicio, UserId, impactoServicio);
            }

            if(impactoServicio == "" &&  details_ticket.ID_STAT == 4)
            {
                return Content("<script type='text/javascript'> function init() {" +
    "if(top.uploadDone) top.uploadDone('ERROR','Seleccione el impacto del Servicio.');}window.onload=init;</script>");
            }

            

            if (details_ticket.ID_STAT == 0)
            {
                details_ticket.ID_STAT = null;
            }
            /*Registro de Lección aprendida - Gestión de Conocimiento*/
            /*Verificación de tipo de ticket*/

            int id_ticket = Convert.ToInt32(details_ticket.ID_TICK);/*Obtenemos el idTicket*/

            var resultTipo = db.TICKETs.Where(t => t.FlagProblema == true && t.ID_TICK == details_ticket.ID_TICK).FirstOrDefault();/* (validamos si es un ticket problema)*/
            if (db.TICKETs.Single(t => t.ID_TICK == id_ticket).FlagProblema == null)
                chkTipoResolucion = Request.Params["TipoResolucion"].ToString() == "false" ? false : true;
            int idEtapa = 0, idActividad = 0, idSubcategoria = 0;
            bool chkCtrlRemotoAct = false;
            //Valida si es una actividad
            if (details_ticket.ID_TYPE_DETA_TICK == 1)
            {
                //Valida si es un ticket proyecto
                if (!(db.TICKETs.Single(x => x.ID_TICK == id_ticket).ID_DOCU_SALE is null))
                {
                    //Subcategorizacion de proyectos
                    if (Request.Params["cboEtapa"] == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Seleccione la etapa.');}window.onload=init;</script>");
                    }
                    else { idEtapa = Convert.ToInt32(Request.Params["cboEtapa"]); }
                    if (Request.Params["cboActividad"] == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Seleccione la actividad.');}window.onload=init;</script>");
                    }
                    else { idActividad = Convert.ToInt32(Request.Params["cboActividad"]); }
                    if (Request.Params["cboSubcategoria"] == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Seleccione la subcategoría.');}window.onload=init;</script>");
                    }
                    else { idSubcategoria = Convert.ToInt32(Request.Params["cboSubcategoria"]); }
                    //string pryEtapa = Request.Params["cboEtapa"];
                    //string pryActividad = Request.Params["cboActividad"];
                    //string prySubcategoria = Request.Params["cboSubcategoria"];
                    chkCtrlRemotoAct = Request.Params["ControlRemotoAct"].ToString() == "false" ? false : true;
                }
            }

            /*categorías obtenidas*/
            if (details_ticket.ID_STAT == 4) /*Verificamos si se encuentra con el estado resuelto*/
            {
                if (resultTipo != null)/*Verificamos si es un ticket problema*/
                {
                    var objResultLeccionAprendida = db.ComLeccionAprendidas.Where(l => l.ID_TICKET == id_ticket && l.EstadoAprobacion == "A").FirstOrDefault();
                    int CatServiceDiarios = Convert.ToInt32(resultTipo.ID_CATE);
                    int CatClase = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == CatServiceDiarios).FirstOrDefault().ID_CATE_PARE);
                    int CatSubCategoria = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == CatClase).FirstOrDefault().ID_CATE_PARE);
                    int Categoria = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == CatSubCategoria).FirstOrDefault().ID_CATE_PARE);

                    if (details_ticket.Titulo == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un título');}window.onload=init;</script>");
                    }
                    if (details_ticket.DescripcionProblema == null || details_ticket.Porque2 == null
                        || details_ticket.Porque3 == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese por lo menos tres porqués.');}window.onload=init;</script>");
                    }
                    if ((details_ticket.DescripcionProblema).Length <= 3 || (details_ticket.Porque2).Length <= 3
                        || (details_ticket.Porque3).Length <= 3)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese como mínimo 4 caracteres en impacto en el negocio.');}window.onload=init;</script>");
                    }
                    //if (details_ticket.DescripcionProblema == null)
                    //{
                    //    return Content("<script type='text/javascript'> function init() {" +
                    //    "if(top.uploadDone) top.uploadDone('ERROR','Ingrese una descripción del problema    ');}window.onload=init;</script>");
                    //}
                    //if (details_ticket.SolucionAplicada == null)
                    //{
                    //    return Content("<script type='text/javascript'> function init() {" +
                    //    "if(top.uploadDone) top.uploadDone('ERROR','Ingrese una solución aplicada');}window.onload=init;</script>");
                    //}
                    if (details_ticket.Impactonegocio == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un impacto a la lección');}window.onload=init;</script>");
                    }
                    if (details_ticket.SolucionTemporal == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese una solución temporal');}window.onload=init;</script>");
                    }
                    if ((details_ticket.Impactonegocio).Length <= 3)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese como mínimo 4 caracteres en impacto en el negocio.');}window.onload=init;</script>");
                    }
                    if ((details_ticket.SolucionTemporal).Length <= 3)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese como mínimo 4 caracteres en la solución temporal');}window.onload=init;</script>");
                    }
                    KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);

                    Electrodata.ERPElectrodata.Domain.Entities.ComLeccionAprendida objLeccionAprendida = new Electrodata.ERPElectrodata.Domain.Entities.ComLeccionAprendida();

                    if (objResultLeccionAprendida == null)
                    {
                        estado = "Registró";

                        int tipoOperacion = Convert.ToInt32(Tipo_Peracion.OPERATION_REGISTER);

                        objLeccionAprendida.ID_TICKET = details_ticket.ID_TICK;
                        objLeccionAprendida.IdTema = resultTipo.IdTema;
                        objLeccionAprendida.Titulo = details_ticket.Titulo;
                        objLeccionAprendida.Nivel1 = Categoria;
                        objLeccionAprendida.Nvel2 = CatSubCategoria;
                        objLeccionAprendida.Nivel3 = CatClase;
                        objLeccionAprendida.Nivel4 = CatServiceDiarios;
                        objLeccionAprendida.DescripcionProblema = details_ticket.DescripcionProblema;
                        objLeccionAprendida.SolucionAplicada = "";
                        objLeccionAprendida.Impactonegocio = details_ticket.Impactonegocio;
                        objLeccionAprendida.SolucionTemporal = details_ticket.SolucionTemporal;
                        objLeccionAprendida.Porque2 = details_ticket.Porque2;
                        objLeccionAprendida.Porque3 = details_ticket.Porque3;
                        objLeccionAprendida.Porque4 = details_ticket.Porque4;
                        objLeccionAprendida.Porque5 = details_ticket.Porque5;
                        objLeccionAprendida.SolucionDefinitiva = string.IsNullOrEmpty(details_ticket.SolucionDefinitiva) ? "" : details_ticket.SolucionDefinitiva;
                        result = _srvLeccionAprendidaService.OperationLeccionAprendidaBusinessProblemas(objLeccionAprendida, tipoOperacion, KEY_ATTA, Convert.ToInt32(Session["ID_ACCO"]), Session["UserName"].ToString(), Convert.ToInt32(Session["APROBADOR_OPERACIONES"]), Convert.ToInt32(Session["UserId"]));
                    }
                    else
                    {
                        estado = "Modificó";

                        int tipoOperacion = Convert.ToInt32(Tipo_Peracion.OPERATION_MODIFY);

                        objLeccionAprendida.IdLeccioNAprendida = objResultLeccionAprendida.IdLeccioNAprendida;
                        objLeccionAprendida.ID_TICKET = details_ticket.ID_TICK;
                        objLeccionAprendida.IdTema = resultTipo.IdTema;
                        objLeccionAprendida.Titulo = details_ticket.Titulo;
                        objLeccionAprendida.Nivel1 = Categoria;
                        objLeccionAprendida.Nvel2 = CatSubCategoria;
                        objLeccionAprendida.Nivel3 = CatClase;
                        objLeccionAprendida.Nivel4 = CatServiceDiarios;
                        objLeccionAprendida.DescripcionProblema = details_ticket.DescripcionProblema;
                        objLeccionAprendida.Porque2 = details_ticket.Porque2;
                        objLeccionAprendida.Porque3 = details_ticket.Porque3;
                        objLeccionAprendida.Porque4 = details_ticket.Porque4;
                        objLeccionAprendida.Porque5 = details_ticket.Porque5;
                        objLeccionAprendida.SolucionAplicada = "";
                        objLeccionAprendida.Impactonegocio = details_ticket.Impactonegocio;
                        objLeccionAprendida.SolucionTemporal = details_ticket.SolucionTemporal;
                        objLeccionAprendida.SolucionDefinitiva = string.IsNullOrEmpty(details_ticket.SolucionDefinitiva) ? "" : details_ticket.SolucionDefinitiva;
                        result = _srvLeccionAprendidaService.OperationLeccionAprendidaBusinessProblemas(objLeccionAprendida, tipoOperacion, KEY_ATTA, Convert.ToInt32(Session["ID_ACCO"]), Session["UserName"].ToString(), Convert.ToInt32(Session["APROBADOR_OPERACIONES"]), Convert.ToInt32(Session["UserId"]));
                    }

                    if (result == (int)Tipo_Peracion.OPERATION_ERROR)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Contacte al Administrador.');}window.onload=init;</script>");
                    }
                }
            }

            /*Fin*/


            int id = Convert.ToInt32(Request.Form["ID_TICK"]);
            var objTicket = db.TICKETs.Single(t => t.ID_TICK == id);
            string cod_tick = Convert.ToString(objTicket.COD_TICK);
            //string KEY_ATTA = null;
            //PRIMERO VALIDAR SI TIENE HIJOS SIN CLOSE

            try
            {
                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);
            }
            catch
            {

            }

            var AttachsGTI = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                   .Where(x => x.ID_DETA_INCI == null).ToList();
            if (details_ticket.IdRazon == 13)
            {
                if (AttachsGTI == null || !AttachsGTI.Any())
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Debe adjuntar imagen de aprobación.');}window.onload=init;</script>");
                }
                else if (details_ticket.COM_DETA_TICK == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar un comentario.');}window.onload=init;</script>");
                }
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                for (int i = AttachsGTI.Count - 1; i >= 0; i--)
                {
                    var AttachsGTIs = AttachsGTI[i];

                    // Check if the file extension is not an image format
                    if (allowedImageExtensions.Any(ext => AttachsGTIs.EXT_ATTA.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        break;
                    }
                    else
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                           "if(top.uploadDone) top.uploadDone('ERROR','Solo se permiten archivos de imagen.');}window.onload=init;</script>");
                    }
                }

            }

            int CantNoClose = 0;

            if (details_ticket.ID_STAT == 4)
            {
                try
                {
                    var cont = db.TICKETs.Where(t => t.ID_TICK_PARENT == id);

                    CantNoClose = (from t in cont.ToList()
                                   join st in db.STATUS on t.ID_STAT_END equals st.ID_STAT
                                   join cc in db.ConfiguracionCategorias on t.ID_CATE equals cc.IdCate4 into ccGroup
                                   from cc in ccGroup.DefaultIfEmpty()
                                   where cc == null && st.ID_STAT != 4 && st.ID_STAT != 6 && st.ID_STAT != 2 && t.ID_TYPE_FORM != 1
                                   select new
                                   {

                                   }).Count();
                }
                catch
                {

                }

                if (CantNoClose != 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','Tienes tickets Abiertos');}window.onload=init;</script>");
                }
            }
            //var DETA_INCI = new DETAILS_INCIDENT();
            int j;

            if (details_ticket.ID_TYPE_DETA_TICK == 1)
            {
                if (details_ticket.Causa == true)
                {
                    var ListaComentarios = db.DETAILS_TICKET.Where(x => x.ID_TICK == id && x.Causa == true).ToList();
                    if (ListaComentarios.Count() > 0)
                    {
                        details_ticket.Causa = false;
                    }
                }

                if (String.IsNullOrEmpty(details_ticket.COM_DETA_TICK))
                {
                    //return Content("<script>alert('Ingrese Su Comentario');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un Comentario');}window.onload=init;</script>");

                }

                //VALIDANDO DATA FECHA NULA O VACÍA


                if (string.IsNullOrEmpty(Convert.ToString(details_ticket.TO_TIME)) || string.IsNullOrEmpty(Convert.ToString(details_ticket.FROM_TIME)))
                {
                    //return Content("<script>alert('Ingrese Su Comentario');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Ingresar las fechas y horas de su actividad real correctamente.');}window.onload=init;</script>");
                }


                // No inclusion de Fechas Negativas
                int RestaFecha = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;

                if (RestaFecha < 0)
                {
                    //return Content("<script>alert('Ingrese Su Comentario');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Ingresar las fechas y horas de su actividad real correctamente.');}window.onload=init;</script>");
                }


            }
            else if (details_ticket.ID_TYPE_DETA_TICK == 2)
            {
                if (details_ticket.ID_PERS_ENTI == null)
                {
                    //return Content("<script>alert('Seleccione Personal');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Seleccione una persona');}window.onload=init;</script>");
                }

            }
            else if (details_ticket.ID_TYPE_DETA_TICK == 3)
            {
                if (details_ticket.ID_STAT == null)
                {
                    //return Content("<script>alert('Seleccione Status');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Seleccione un Estado');}window.onload=init;</script>");
                }
                else
                {
                    if (details_ticket.ID_STAT == 5 && objTicket.ID_STAT_END == 6)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','No se puede programar un ticket cerrado');}window.onload=init;</script>");
                    }
                    else if (details_ticket.ID_STAT == 5 && details_ticket.FEC_SCHE == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','Seleccione una fecha para programar');}window.onload=init;</script>");
                    }
                    else if (details_ticket.ID_STAT == 5 && details_ticket.FEC_SCHE < DateTime.Now)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','Seleccione una fecha para programar, mayor al dia de hoy');}window.onload=init;</script>");
                    }
                }

            }


            /*Gestión Problemas*/
            if (details_ticket.ID_STAT == 4)
            {
                if (resultTipo == null)
                {
                    if (String.IsNullOrEmpty(details_ticket.COM_DETA_TICK))
                    {
                        //return Content("<script>alert('Ingrese Su Comentario');</script>");
                        return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un Comentario');}window.onload=init;</script>");
                    }
                }
                else
                {
                    details_ticket.COM_DETA_TICK = "<a target='_blank' href='KnowledgeManagement/EditLessonLearned/" + result.ToString() + "'" + "  style='text-decoration:none;'><span class='glyphicon glyphicon-play' aria-hidden='true'></span><span>Se " + estado + " la lección aprendida Nro: " + result.ToString() + " </span></a>";
                    // "El ticket problema fue cerrado de manera satisfactoria, y se realizó la lección aprendida correspondiente N° " + result.ToString();
                }
            }
            details_ticket.COM_DETA_TICK = String.IsNullOrEmpty(details_ticket.COM_DETA_TICK) ? "" : details_ticket.COM_DETA_TICK;
            /*Fin*/
            //---------------------PARA LOS POINTS-------------------------------------

            var statusticket = db.TICKETs.Single(x => x.ID_TICK == details_ticket.ID_TICK);
            int idstat = Convert.ToInt32(statusticket.ID_STAT);
            int idstatend = Convert.ToInt32(statusticket.ID_STAT_END);
            int idtick = Convert.ToInt32(details_ticket.ID_TICK);
            int rejecttick = 0;

            if ((idstatend == 6 || idstatend == 4))
            {
                rejecttick = 1;

            }
            //---------------------------------------------------------------------------

            if (details_ticket.ID_TYPE_DETA_TICK == 3 || details_ticket.ID_TYPE_DETA_TICK == 2)
            {
                details_ticket.SEND_MAIL = false;
            }

            details_ticket.ID_TICK = id;
            details_ticket.UserId = UserId;
            details_ticket.CREATE_DETA_INCI = DateTime.Now;

            var ticket = db.TICKETs.Where(x => x.ID_TICK == id).Single();
            if (details_ticket.ID_STAT == 5)
            {
                ticket.EsperaPorCliente = details_ticket.EsperaPorCliente;
            }

            if (details_ticket.ID_STAT == 4) /*Verificamos si se encuentra con el estado resuelto*/
            {
                ticket.REM_CTRL_TICK = chkControlRemoto;

                if (resultTipo != null)/*Verificamos si es un ticket problema*/
                {
                    /*Gestión de problemas*/
                    if (string.IsNullOrEmpty(details_ticket.SolucionDefinitiva))
                    {
                        details_ticket.ID_STAT = 8;
                        /*Si el campo SolucionDefinitiva está vacío el ticket se registra con estado */
                    }
                    else
                    {
                        if ((details_ticket.SolucionDefinitiva).Length <= 3)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Ingrese como mínimo 4 caracteres en la solución definitiva.');}window.onload=init;</script>");
                        }
                        else
                        {
                            var cambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == id);
                            if (cambio.Count() == 1)
                            {

                                int idCambio = Convert.ToInt32(cambio.SingleOrDefault().id);
                                var estadoCambio = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == idCambio).SingleOrDefault();

                                if (estadoCambio.idTypeRequest != 5 && estadoCambio.idTypeRequest != 3)
                                {
                                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('ERROR','La solicitud del cambio debe estar ejecutada antes de resolver el ticket.');}window.onload=init;</script>");
                                }
                            }
                        }
                    }
                }
                else
                {
                    var cambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == id);
                    if (cambio.Count() == 1)
                    {

                        int idCambio = Convert.ToInt32(cambio.SingleOrDefault().id);
                        var estadoCambio = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == idCambio).SingleOrDefault();

                        if (estadoCambio.idTypeRequest != 5 && estadoCambio.idTypeRequest != 3)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','La solicitud del cambio debe estar ejecutada antes de resolver el ticket.');}window.onload=init;</script>");
                        }
                    }
                }
            }
            /*Fin*/

            //GF - Tickets Hijos
            if ((Convert.ToInt32(Session["ID_ACCO"]) == 1 || Convert.ToInt32(Session["ID_ACCO"]) == 25) && details_ticket.ID_TYPE_DETA_TICK == 3 && details_ticket.ID_STAT == 4)
            {
                var tieneHijosActivos = db.TICKETs.Any(x => x.ID_TICK_PARENT == id && (x.ID_STAT_END == 1 || x.ID_STAT_END == 3 || x.ID_STAT_END == 5));
                if (tieneHijosActivos)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('TieneHijosGF','El ticket cuenta con tickets hijos activos o programados.');}window.onload=init;</script>");
                }
            }

            /*Gestion de tareas de ticket*/
            if (new[] { 56, 57, 58 }.Contains(Convert.ToInt32(Session["ID_ACCO"])) && details_ticket.ID_TYPE_DETA_TICK == 3)
            {
                int[] estadosActivacionTicket = { 1, 3, 5 };
                int[] estadosTareasNoPendientes = { 1, 2 };

                if (details_ticket.ID_STAT == 2)
                {
                    var tareas = from td in db.TareaDetalle where td.Id_Tick == idtick && !(estadosTareasNoPendientes.Contains(td.IdEstado.Value)) select td;
                    foreach (TareaDetalle tarea in tareas)
                    {
                        tarea.IdEstado = 2;
                        tarea.Id_Pers_Enti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        tarea.FechaTareaTerminada = DateTime.Now;
                    }
                    try
                    { db.SaveChanges(); }
                    catch (Exception e)
                    { Console.WriteLine(e); }

                    // Tareas SAP
                    try
                    {
                        int IdPersEntiMod = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        db.CerrarTareas(idtick, IdPersEntiMod);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                if (estadosActivacionTicket.Contains(details_ticket.ID_STAT.Value) && idstatend == 2)
                {
                    var tareas = from td in db.TareaDetalle where td.Id_Tick == idtick select td;
                    foreach (TareaDetalle tarea in tareas)
                    {
                        tarea.IdEstado = 5; tarea.Id_Pers_Enti = null;
                    }
                    try
                    { db.SaveChanges(); }
                    catch (Exception e)
                    { Console.WriteLine(e); }
                }
            }

            //Actualizando el estado del ticket
            if (details_ticket.ID_TYPE_DETA_TICK == 3)
            {
                ticket.ID_STAT_END = details_ticket.ID_STAT;
                if (ticket.ID_STAT_END == 4 && ticket.ID_PERS_ENTI_END == 63132 && ticket.ID_ACCO == 4)
                {
                    List<int> compList = new List<int>(new[] { 67136, 71236, 71251, 71249, 71254, 71260, 71266, 71263, 71241, 71245, 71243, 71269, 71257 });
                    if (compList.Contains(ticket.ID_COMP_END.Value))
                    {
                        ticket.SEND_SURVEY = false;
                    }

                }

            }

            //Validar Motivo Espera
            int ticketProblemaME = Convert.ToInt32(Request.Form["TicketProblemaME"]);
            if (ticketProblemaME == 0 && !new[] { 56, 57, 58, 60 }.Contains(Convert.ToInt32(Session["ID_ACCO"])) && details_ticket.ID_TYPE_DETA_TICK == 3 && details_ticket.ID_STAT == 5)
            {
                string idMotivo = Request.Form["IdMotivoEstadoProgra"];
                if (string.IsNullOrEmpty(idMotivo) && details_ticket.IdMotivoEstado == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Seleccione el motivo de espera.');}window.onload=init;</script>");
                }
                else
                {
                    if (details_ticket.IdMotivoEstado == null)
                        details_ticket.IdMotivoEstado = Convert.ToInt32(idMotivo);

                    ticket.IdMotivoEstado = details_ticket.IdMotivoEstado;
                }
            }

            db.Entry(ticket).State = EntityState.Modified;



            if (details_ticket.ID_STAT == 4)
            {
                if (chkTipoResolucion == true)
                    details_ticket.IdTipoResolucion = 1;
                else
                    details_ticket.IdTipoResolucion = 2;

                objTicket.Solucion = details_ticket.COM_DETA_TICK;
                db.Entry(objTicket).State = EntityState.Modified;
                db.SaveChanges();

                if (Convert.ToInt32(Session["ID_ACCO"]) == 60 && ticket.IS_PARENT == true && ticket.ID_CATE == 23199)
                {
                    var objTicketPadre = db.TICKETs.FirstOrDefault(t => t.ID_TICK == ticket.ID_TICK_PARENT);
                    var comentarioTicketPadre = new DETAILS_TICKET();
                    comentarioTicketPadre.ID_TICK = objTicketPadre.ID_TICK;
                    comentarioTicketPadre.COM_DETA_TICK = details_ticket.COM_DETA_TICK;
                    comentarioTicketPadre.FEC_SCHE = DateTime.Now;
                    comentarioTicketPadre.UserId = details_ticket.UserId;
                    comentarioTicketPadre.CREATE_DETA_INCI = DateTime.Now;
                    comentarioTicketPadre.ID_STAT = 1;
                    comentarioTicketPadre.ID_TYPE_DETA_TICK = 3;
                    comentarioTicketPadre.PortalComent = false;
                    db.DETAILS_TICKET.Add(comentarioTicketPadre);
                    db.SaveChanges();
                    objTicketPadre.ID_STAT = 1;
                    objTicketPadre.IdRazon = 2;
                    db.Entry(objTicketPadre).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            if (Convert.ToInt32(Session["ID_ACCO"]) == 60)
            {
                int ID_RAZON = 0;
                string razon = Request.Params["IdRazon"];
                if (razon == "" || razon == null)
                {
                    ID_RAZON = 0;
                }
                else
                {
                    ID_RAZON = Convert.ToInt32(Request.Params["IdRazon"].ToString());

                }
                //int ID_RAZON = Request.Form["IdRazon"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdRazon"].ToString());
                //if (ID_RAZON == 0) { ticket.IdRazon = null; } else { ticket.IdRazon = ID_RAZON; }
                if (details_ticket.ID_TYPE_DETA_TICK == 3)
                {
                    if (ID_RAZON == 0)
                    {
                        ticket.IdRazon = null;
                    }
                    else
                    {
                        ticket.IdRazon = ID_RAZON;
                    }
                }
                details_ticket.IdRazon = ID_RAZON;
            }

            db.DETAILS_TICKET.Add(details_ticket);
            db.SaveChanges();


            int ID_DETA_TICK = details_ticket.ID_DETA_TICK;
            db.Entry(details_ticket).State = EntityState.Detached;

            if (details_ticket.ID_STAT == 4)
            {
                if (ticket.ID_CATE == 23199 && ticket.ID_QUEU == 105)
                {
                    TicketController ticketController = new TicketController();
                    ticketController.ControllerContext = this.ControllerContext;

                    ticketController.EnviarCorreoSolcucionAccesoUSB();
                }

                //cambiar el estado de informe cuando este se cierre:
                if (ticket.ID_CATE == 13973)
                {
                    int IdInforme = (from ie in db.InformeEDDetalle
                                     join tic in db.TICKETs on (ie.Id == null ? 0 : ie.Id) equals tic.IdInforme into ctxsd
                                     from ctxs in ctxsd.DefaultIfEmpty()
                                     where ctxs.ID_TICK == ticket.ID_TICK
                                     select new
                                     {
                                         IdInforme = ie.Id
                                     }
                                                          ).Single().IdInforme;

                    InformeEDDetalle objInformeDetalle = db.InformeEDDetalle.Where(x => x.Id == IdInforme).FirstOrDefault();

                    objInformeDetalle.IdEstadoInforme = 7;
                    db.SaveChanges();


                }

                //Bolsa de Horas
                if (ticket.ID_ACCO == 4)
                {
                    bool checkConsumoHoras = Request.Params["AplicaConsumoHoras"] != null && Request.Params["AplicaConsumoHoras"].ToString() == "on";

                    int MinutosEfectiv = string.IsNullOrEmpty(Request.Params["MinutosEfectivos"]) ? 0 : Convert.ToInt32(Request.Params["MinutosEfectivos"]);

                    DETAILS_TICKET detailTicketBolsa = db.DETAILS_TICKET.Find(ID_DETA_TICK);
                    detailTicketBolsa.COM_DETA_TICK = detailTicketBolsa.COM_DETA_TICK + " " + (MinutosEfectiv==0?"":"<br/>Ticket Efectivo para Consumo de Horas.");
                    detailTicketBolsa.MinutosBolsaHoras = MinutosEfectiv;
                    db.SaveChanges();

                    if (checkConsumoHoras == true)
                    {
                        if (MinutosEfectiv <= 0)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar la cantidad correcta de minutos efectivos para el descuento de la bolsa de horas.');}window.onload=init;</script>");
                        }
                        else
                        {
                            ticket.ConsumoHoras = checkConsumoHoras;
                            ticket.MinutosEfectivos = (ticket.MinutosEfectivos==null?0: ticket.MinutosEfectivos) + MinutosEfectiv;
                            db.SaveChanges();
                        }
                        
                    }
                    else
                    {
                        ticket.ConsumoHoras = checkConsumoHoras;
                        ticket.MinutosEfectivos = 0;
                        db.SaveChanges();
                    }
                    

                }
            }

            int bandera = 0;
            //string rout="";
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
                            var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                            EObj.ID_DETA_INCI = ID_DETA_TICK;
                            db.Entry(EObj).State = EntityState.Modified;
                            db.SaveChanges();
                            db.Entry(EObj).State = EntityState.Detached;

                            if (ticket.IS_PARENT == true)
                            {
                                var objComentarioTicketHijo = db.DETAILS_TICKET.Where(t => t.ID_TICK == ticket.ID_TICK)
                                    .OrderByDescending(t => t.ID_DETA_TICK)
                                   .FirstOrDefault();
                                var adjuntoComentarioTicketPadre = db.ATTACHEDs.Where(t => t.ID_DETA_INCI == objComentarioTicketHijo.ID_DETA_TICK)
                                   .OrderByDescending(t => t.ID_DETA_INCI)
                                   .FirstOrDefault();

                                var objComentarioTicketPadre = db.DETAILS_TICKET.Where(t => t.ID_TICK == ticket.ID_TICK_PARENT)
                                    .OrderByDescending(t => t.ID_DETA_TICK)
                                   .FirstOrDefault();
                                var objAdjunto = new ATTACHED();

                                objAdjunto.NAM_ATTA = adjuntoComentarioTicketPadre.NAM_ATTA;
                                objAdjunto.EXT_ATTA = adjuntoComentarioTicketPadre.EXT_ATTA;
                                objAdjunto.ID_DETA_INCI = objComentarioTicketPadre.ID_DETA_TICK;
                                objAdjunto.CREATE_ATTA = DateTime.Now;
                                objAdjunto.KEY_ATTA = adjuntoComentarioTicketPadre.KEY_ATTA;
                                objAdjunto.ID_TYPE_DOCU_ATTA = adjuntoComentarioTicketPadre.ID_TYPE_DOCU_ATTA;
                                objAdjunto.DELETE_ATTA = adjuntoComentarioTicketPadre.DELETE_ATTA;
                                db.ATTACHEDs.Add(objAdjunto);
                                db.SaveChanges();
                            }
                        }
                        catch
                        {

                        }

                    }
                    bandera = 1;
                }
            }

            //if (details_ticket.ID_TYPE_DETA_TICK == 3)
            //{
            //    //SendMail mail = new SendMail();
            //    //mail.UpdateStatus(ID_DETA_TICK);
            //}

            //if (details_ticket.ID_TYPE_DETA_TICK == 2)
            //{
            //    //SendMail mail = new SendMail();
            //    //mail.TransferTicket(ID_DETA_TICK);
            //}
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (rejecttick != 1)
            {
                //-------------INSERTA EN PONIT CUANDO SE CREA UN  DETAILS TICKET --------------

                pt.UpdatePointByDetailsTicket(details_ticket, ID_PERS_ENTI, bandera);
            }

            //--------------------------CUANDO ES REABIERTO-----------------------------------------

            if (rejecttick == 1)
            {

                //--------------------------LOG COMMENT-----------------------------------
                if ((details_ticket.ID_TYPE_DETA_TICK == 1 && idstatend == 6))
                {

                    pt.UpdatePointByDetailsTicketReject(details_ticket, ID_PERS_ENTI);
                }

                if ((details_ticket.ID_STAT != 6 || details_ticket.ID_STAT != 4) && (details_ticket.ID_TYPE_DETA_TICK == 2 || details_ticket.ID_TYPE_DETA_TICK == 3))
                {
                    int ID_PERS_ENTI_ASSI = Convert.ToInt32(statusticket.ID_PERS_ENTI_ASSI);
                    pt.UpdatePointByDetailsTicketReject(details_ticket, ID_PERS_ENTI_ASSI);
                }

            }


            /*Se obtiene los códigos de categoría y tema en base al código de ticket*/
            var valores = (from c1 in db.CATEGORies
                           join c2 in db.CATEGORies on c1.ID_CATE_PARE equals c2.ID_CATE
                           join c3 in db.CATEGORies on c2.ID_CATE_PARE equals c3.ID_CATE
                           join c4 in db.CATEGORies on c3.ID_CATE_PARE equals c4.ID_CATE
                           join t in db.TICKETs on c1.ID_CATE equals t.ID_CATE
                           join ct in db.ComTemas on (t.IdTema == null ? 0 : t.IdTema) equals ct.IdTema into ctxsd  /*Agregado para la gestión del conocimiento*/
                           from ctxs in ctxsd.DefaultIfEmpty()
                           where t.ID_TICK == id
                           select new
                           {
                               c1 = c1.ID_CATE,
                               c1nombre = c1.NAM_CATE,
                               c2 = c2.ID_CATE,
                               c2nombre = c2.NAM_CATE,
                               c3 = c3.ID_CATE,
                               c3nombre = c3.NAM_CATE,
                               c4 = c4.ID_CATE,
                               c4nombre = c4.NAM_CATE,
                               idTema = t.IdTema,
                               idTicket = t.ID_TICK,
                               NAME_TEMA = (ctxs == null ? "" : ctxs.Nomtema),  /*Agregado para la gestión del conocimiento*/
                               FlagProblema = t.FlagProblema
                           }).ToList();

            if (valores.Count > 0)
            {
                nivel1 = valores.FirstOrDefault().c4 != null ? Convert.ToInt32(valores.FirstOrDefault().c4) : 0;
                nameNivel1 = valores.FirstOrDefault().c4nombre != null ? valores.FirstOrDefault().c4nombre : string.Empty;
                nivel2 = valores.FirstOrDefault().c3 != null ? Convert.ToInt32(valores.FirstOrDefault().c3) : 0;
                nameNivel2 = valores.FirstOrDefault().c3nombre != null ? valores.FirstOrDefault().c3nombre : string.Empty;
                nivel3 = valores.FirstOrDefault().c2 != null ? Convert.ToInt32(valores.FirstOrDefault().c2) : 0;
                nameNivel3 = valores.FirstOrDefault().c2nombre != null ? valores.FirstOrDefault().c2nombre : string.Empty;
                nivel4 = valores.FirstOrDefault().c1 != null ? Convert.ToInt32(valores.FirstOrDefault().c1) : 0;
                nameNivel4 = valores.FirstOrDefault().c1nombre != null ? valores.FirstOrDefault().c1nombre : string.Empty;
                idTema = valores.FirstOrDefault().idTema != null ? Convert.ToInt32(valores.FirstOrDefault().idTema) : 0;
                nameTema = valores.FirstOrDefault().NAME_TEMA != null ? valores.FirstOrDefault().NAME_TEMA : string.Empty;
                Flagproblema = valores.FirstOrDefault().FlagProblema != null ? Convert.ToBoolean(valores.FirstOrDefault().FlagProblema) : false;
            }
            /*Fin*/

            //Insertando actividades
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //Ticket Proyecto
            var proyecto = db.TicketProyecto(details_ticket.ID_TICK, details_ticket.COM_DETA_TICK, UserId).SingleOrDefault();

            var RegistraActividad = (from ta in dbe.TYPE_ACT_LOG
                                     join sta in dbe.SUBTYPE_ACTITY on ta.ID_TYPE_ACT equals sta.ID_TYPE_ACT
                                     where ta.ID_ACCO == ID_ACCO && ta.VIG_ACT == true && sta.VIG_SUB_TYPE == true
                                     select new
                                     {
                                         ta.ID_TYPE_ACT,
                                         sta.ID_SUBTYPE_ACT
                                     }).Where(x => x.ID_SUBTYPE_ACT == 2 || x.ID_SUBTYPE_ACT == 8 || x.ID_SUBTYPE_ACT == 11 || x.ID_SUBTYPE_ACT == 14 ||
                                         x.ID_SUBTYPE_ACT == 17 || x.ID_SUBTYPE_ACT == 35 || x.ID_SUBTYPE_ACT == 36 || x.ID_SUBTYPE_ACT == 37 || x.ID_SUBTYPE_ACT == 38 || x.ID_SUBTYPE_ACT == 20 ||
                                         x.ID_SUBTYPE_ACT >= 42).FirstOrDefault();

            int ID_TYPE_ACT = 0;
            string NAM_SUBTYPE_ACT = "";
            if (proyecto.Mensaje == "1")
            {
                if (objTicket.ID_CATE == 10144)
                {
                    ID_TYPE_ACT = Convert.ToInt32(dbe.TYPE_ACT_LOG.Single(x => x.DES_ACT == "SOPORTES").ID_TYPE_ACT);

                }
                else if (objTicket.ID_CATE == 13973)
                {
                    ID_TYPE_ACT = Convert.ToInt32(dbe.TYPE_ACT_LOG.Single(x => x.DES_ACT == "INFORMES").ID_TYPE_ACT);
                }
                else
                {
                    ID_TYPE_ACT = 1;
                }
                NAM_SUBTYPE_ACT = proyecto.Op;
                var qType = dbe.TipoServicio.Where(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                if (qType.Count() == 1)
                {
                    ID_TYPE_ACT = Convert.ToInt32(qType.Single().ID_TYPE_ACT);
                }
                ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                objActividad.ID_CLIE = (int)objTicket.ID_COMP;
                objActividad.ID_TYPE_ACT = ID_TYPE_ACT;
                objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                objActividad.COD_SUBTYPE_ACT = id_ticket;
                objActividad.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT;
                objActividad.DATE_INIC = details_ticket.FROM_TIME;
                objActividad.DATE_END = details_ticket.TO_TIME;
                objActividad.COMENTARIO = "";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                objActividad.USERID = UserId;
                objActividad.CREATE_ACT_LOG = DateTime.Now;
                objActividad.VIG_ACTI_LOG = true;
                objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                objActividad.CLOSE_TICKET = details_ticket.ID_STAT == 4 ? true : false;
                objActividad.SCHEDULE_TICKET = details_ticket.ID_STAT == 5 ? true : false;
                objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;
                objActividad.DETAILS_TICKETS = details_ticket.ID_DETA_TICK;
                objActividad.ID_ACCO = ID_ACCO;
                if (details_ticket.ID_TYPE_DETA_TICK == 1)
                {
                    //Valida si es un ticket proyecto
                    if (!(db.TICKETs.Single(x => x.ID_TICK == id_ticket).ID_DOCU_SALE is null))
                    {
                        objActividad.IdEtapa = idEtapa;
                        objActividad.IdActividad = idActividad;
                        objActividad.IdSubcategoria = idSubcategoria;
                        objActividad.ActividadRemota = chkCtrlRemotoAct;
                    }
                }

                dbe.ACTIVITY_LOG.Add(objActividad);
                dbe.SaveChanges();
            }
            else if (RegistraActividad != null)
            {
                ID_TYPE_ACT = RegistraActividad.ID_TYPE_ACT;
                var qType = dbe.TipoServicio.Where(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                if (qType.Count() == 1)
                {
                    ID_TYPE_ACT = Convert.ToInt32(qType.Single().ID_TYPE_ACT);
                }
                NAM_SUBTYPE_ACT = objTicket.COD_TICK;

                ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                objActividad.ID_CLIE = (int)objTicket.ID_COMP;
                objActividad.ID_TYPE_ACT = ID_TYPE_ACT;
                objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                objActividad.COD_SUBTYPE_ACT = id_ticket;
                objActividad.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT;
                objActividad.DATE_INIC = details_ticket.FROM_TIME;
                objActividad.DATE_END = details_ticket.TO_TIME;
                objActividad.COMENTARIO = "";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                objActividad.USERID = UserId;
                objActividad.CREATE_ACT_LOG = DateTime.Now;
                objActividad.VIG_ACTI_LOG = true;
                objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                objActividad.CLOSE_TICKET = details_ticket.ID_STAT == 4 ? true : false;
                objActividad.SCHEDULE_TICKET = details_ticket.ID_STAT == 5 ? true : false;
                objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;
                objActividad.DETAILS_TICKETS = details_ticket.ID_DETA_TICK;
                objActividad.ID_ACCO = ID_ACCO;
                if (details_ticket.ID_TYPE_DETA_TICK == 1)
                {
                    //Valida si es un ticket proyecto
                    if (!(db.TICKETs.Single(x => x.ID_TICK == id_ticket).ID_DOCU_SALE is null))
                    {
                        objActividad.IdEtapa = idEtapa;
                        objActividad.IdActividad = idActividad;
                        objActividad.IdSubcategoria = idSubcategoria;
                        objActividad.ActividadRemota = chkCtrlRemotoAct;
                    }
                }
                dbe.ACTIVITY_LOG.Add(objActividad);
                dbe.SaveChanges();
            }
            //CAMBIO DE ESTADO Y GUARDADO DEL DETAIL DEL PADRE
            if (details_ticket.ID_STAT == 4 || details_ticket.ID_STAT == 2) //SI SE ENVIA RESUELTO O CANCELADO 
            {
                var codparent = db.CierreDeTickets(id).SingleOrDefault(); //REVISAR Y CERRAR EL TICKET PADRE

                //ACTIVIDAD LOG PARA PADRE.

                if (codparent.iddetatick != 0) //SI SE CREA UN DETALLE = SE CIERRA POR TICKETS HIJOS CERRADOS
                { //FALTA ESPECIFICAR SALIDA
                    var proyectPadre = db.TicketProyecto(codparent.tickparent, "PARENT TICKET RESOLVED AUTOMATICALLY BY THE SYSTEM", UserId).SingleOrDefault();//para el padre //PRUEBA COMENTARIO A REVISAR ||details_ticket.COM_DETA_TICK
                    var objParentTicket = db.TICKETs.Single(t => t.ID_TICK == codparent.tickparent);

                    if (proyectPadre.Mensaje == "1")
                    {
                        if (objParentTicket.ID_CATE == 10144)
                        {
                            ID_TYPE_ACT = Convert.ToInt32(dbe.TYPE_ACT_LOG.Single(x => x.DES_ACT == "SOPORTES").ID_TYPE_ACT);

                        }
                        else if (objParentTicket.ID_CATE == 13973)
                        {
                            ID_TYPE_ACT = Convert.ToInt32(dbe.TYPE_ACT_LOG.Single(x => x.DES_ACT == "INFORMES").ID_TYPE_ACT);
                        }
                        else
                        {
                            ID_TYPE_ACT = 1;
                        }

                        NAM_SUBTYPE_ACT = proyectPadre.Op;
                        var qType = dbe.TipoServicio.Where(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                        if (qType.Count() == 1)
                        {
                            ID_TYPE_ACT = Convert.ToInt32(qType.Single().ID_TYPE_ACT);
                        }
                        ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                        objActividad.ID_CLIE = (int)objParentTicket.ID_COMP;
                        objActividad.ID_TYPE_ACT = ID_TYPE_ACT;
                        objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                        objActividad.COD_SUBTYPE_ACT = codparent.tickparent; //nro de ticket
                        objActividad.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT;
                        objActividad.DATE_INIC = details_ticket.FROM_TIME;
                        objActividad.DATE_END = details_ticket.TO_TIME;
                        objActividad.COMENTARIO = "CIERRE AUTOMATICO DE TICKETS . NO EXISTEN TICKETS ABIERTOS";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                        objActividad.USERID = UserId;
                        objActividad.CREATE_ACT_LOG = DateTime.Now;
                        objActividad.VIG_ACTI_LOG = true;
                        objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                        objActividad.CLOSE_TICKET = true;
                        objActividad.SCHEDULE_TICKET = false;
                        objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;
                        objActividad.DETAILS_TICKETS = codparent.iddetatick;
                        objActividad.ID_ACCO = ID_ACCO;
                        dbe.ACTIVITY_LOG.Add(objActividad);
                        dbe.SaveChanges();
                    }
                    else if (RegistraActividad != null)
                    {
                        ID_TYPE_ACT = RegistraActividad.ID_TYPE_ACT;
                        var qType = dbe.TipoServicio.Where(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                        if (qType.Count() == 1)
                        {
                            ID_TYPE_ACT = Convert.ToInt32(qType.Single().ID_TYPE_ACT);
                        }
                        NAM_SUBTYPE_ACT = objParentTicket.COD_TICK;

                        ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                        objActividad.ID_CLIE = (int)objParentTicket.ID_COMP;
                        objActividad.ID_TYPE_ACT = ID_TYPE_ACT;
                        objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                        objActividad.COD_SUBTYPE_ACT = codparent.tickparent;
                        objActividad.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT;
                        objActividad.DATE_INIC = details_ticket.FROM_TIME;
                        objActividad.DATE_END = details_ticket.TO_TIME;
                        objActividad.COMENTARIO = "CIERRE AUTOMATICO DE TICKETS . NO EXISTEN TICKETS ABIERTOS";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                        objActividad.USERID = UserId;
                        objActividad.CREATE_ACT_LOG = DateTime.Now;
                        objActividad.VIG_ACTI_LOG = true;
                        objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                        objActividad.CLOSE_TICKET = true;
                        objActividad.SCHEDULE_TICKET = false;
                        objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;
                        objActividad.DETAILS_TICKETS = codparent.iddetatick;
                        objActividad.ID_ACCO = ID_ACCO;
                        dbe.ACTIVITY_LOG.Add(objActividad);
                        dbe.SaveChanges();
                    }
                }

            }
            // Habilitar envio de correos (SEND MAIL PARA MINSUR SAP) 
            if (details_ticket.ID_TYPE_DETA_TICK == 1)
            {
                if (db.ObtenerColaSAP(ID_DETA_TICK).Single().flagSAP == 1)
                {
                    var dTicket = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
                    dTicket.SEND_MAIL = false;
                    db.Entry(dTicket).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            //Notificaciones de comentario
            if ((Convert.ToInt32(Session["ID_ACCO"]) == 56 || Convert.ToInt32(Session["ID_ACCO"]) == 57 || Convert.ToInt32(Session["ID_ACCO"]) == 58) && details_ticket.COM_DETA_TICK != null)
            {
                var cateComentario = (from t in db.TICKETs
                                      join c in db.CATEGORies on t.ID_CATE equals c.ID_CATE
                                      where t.ID_TICK == details_ticket.ID_TICK
                                      select c).FirstOrDefault();

                if (cateComentario.AplicaNotificacion != null)
                {
                    var usuariosNotifi = db.CategoriaNotificacion.Where(x => x.IdCate == cateComentario.ID_CATE && x.Estado == true).ToList();

                    foreach (var usuarioNoti in usuariosNotifi)
                    {
                        if (Convert.ToInt32(Session["UserId"]) != usuarioNoti.UserId && Convert.ToInt32(Session["UserId"]) != 34)
                        {
                            db.CrearComentarioNotificacion(details_ticket.ID_DETA_TICK, usuarioNoti.UserId, Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["ID_ACCO"]));
                        }
                    }
                }
            }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + cod_tick + "','" + nivel1 + "','" + nivel2 + "','" + nivel3 + "','" + nivel4 + "','" + idTema + "','" + idtick + "','" + nameNivel1 + "','" + nameNivel2 + "','" + nameNivel3 + "','" + nameNivel4 + "','" + nameTema + "','" + details_ticket.ID_STAT + "','" + Flagproblema + "');}window.onload=init;</script>");
        }
        //
        // POST: /DetailsTicket/SendmailCreate/
        public int SendmailCreate(int id)
        {
            try
            {
                string smtp = "smtp.zoho.com";
                //string fromEmail = "esalazar@electrodata.com.pe";
                string fromEmail = "jquisper@electrodata.com.pe";
                string clave = "22904535";//"13579it";
                var port = 465;
                string ssl = "true";

                var ticket = db.TICKETs.Single(t => t.ID_TICK == id);

                var coordinator = db.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                    .Single(x => x.ID_ACCO == ticket.ID_ACCO);

                int IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);

                string mail_coor = "jquisper@electrodata.com.pe";//dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor).EMA_ELEC;

                string mailTo1 = "jquisper@electrodata.com.pe";//dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).EMA_ELEC;

                if (String.IsNullOrEmpty(mailTo1))
                {
                    mailTo1 = fromEmail;
                }

                if (String.IsNullOrEmpty(mail_coor))
                {
                    mail_coor = fromEmail;
                }

                CDO.Message message = new CDO.Message();
                CDO.IConfiguration configuration = message.Configuration;
                ADODB.Fields fields = configuration.Fields;

                ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
                field.Value = smtp;

                field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
                field.Value = port;

                field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
                field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

                field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
                field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

                field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
                field.Value = fromEmail;

                field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
                field.Value = clave;

                field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
                field.Value = ssl;

                fields.Update();

                message.From = fromEmail;
                message.To = mailTo1;
                message.CC = mail_coor;
                message.Subject = Convert.ToString(ticket.COD_TICK) + " Ticket Update Status / Boleto Actualización de Estado ";
                Random rnd = new Random();
                decimal xx = rnd.Next(1, 1000);
                string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
                message.CreateMHTMLBody(strHostName + "Mail/SendMailUpdateStatus/" + Convert.ToString(ticket.ID_TICK) + "?_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone, "Administrator", "123456");
                message.Send();

                return 1;
            }
            catch (Exception e)
            {
                try
                {
                    var acp = new ACCOUNT_PARAMETER();
                    acp.VAL_ACCO_PARA = e.Message;
                    acp.ID_ACCO = null;
                    acp.ID_PARA = null;

                    db.ACCOUNT_PARAMETER.Add(acp);
                    db.SaveChanges();
                    return 0;
                }
                catch
                {
                    return 0;
                }
            }
        }

        //
        // GET: /DetailsTicket/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DETAILS_TICKET details_ticket = db.DETAILS_TICKET.Find(id);
            if (details_ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", details_ticket.ID_CLIE);
            ViewBag.ID_TYPE_DETA_TICK = new SelectList(db.TYPE_DETAILS_TICKET, "ID_TYPE_DETA_TICK", "NAM_TYPE_DETA_TICK", details_ticket.ID_TYPE_DETA_TICK);
            ViewBag.ID_TICK = new SelectList(db.TICKETs, "ID_TICK", "COD_TICK", details_ticket.ID_TICK);
            return View(details_ticket);
        }

        //
        // POST: /DetailsTicket/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DETAILS_TICKET details_ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(details_ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", details_ticket.ID_CLIE);
            ViewBag.ID_TYPE_DETA_TICK = new SelectList(db.TYPE_DETAILS_TICKET, "ID_TYPE_DETA_TICK", "NAM_TYPE_DETA_TICK", details_ticket.ID_TYPE_DETA_TICK);
            ViewBag.ID_TICK = new SelectList(db.TICKETs, "ID_TICK", "COD_TICK", details_ticket.ID_TICK);
            return View(details_ticket);
        }

        [Authorize]
        public ActionResult EditSummary(int id)
        {
            DETAILS_TICKET query = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == id);
            ViewBag.COM_DETA_TICK = query.COM_DETA_TICK;

            return View(query);
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditSummary(int id, int id1 = 0)
        {
            try
            {
                int ID_DETA_TICK = Convert.ToInt32(Request.Params["ID_DETA_TICK"]);
                var COM_DETA_TICK = Convert.ToString(Request.Params["COM_DETA_TICK_E"]);

                DETAILS_TICKET query = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == id);

                if (String.IsNullOrEmpty(COM_DETA_TICK.Trim()))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Ingrese un Comentario');}window.onload=init;</script>");
                }

                query.COM_DETA_TICK = COM_DETA_TICK;

                db.Entry(query).State = EntityState.Modified;
                db.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEditDeta) top.uploadDoneEditDeta('OK','Registro Actualizado');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEditDeta) top.uploadDoneEditDeta('ERROR','Contacte al Administrador');}window.onload=init;</script>");
            }
        }

        //
        // GET: /DetailsTicket/Delete/5

        public ActionResult Delete(int id = 0)
        {
            DETAILS_TICKET details_ticket = db.DETAILS_TICKET.Find(id);
            if (details_ticket == null)
            {
                return HttpNotFound();
            }
            return View(details_ticket);
        }

        //
        // POST: /DetailsTicket/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            DETAILS_TICKET details_ticket = db.DETAILS_TICKET.Find(id);
            db.DETAILS_TICKET.Remove(details_ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult VerDocumento(string id = "")
        {
            try
            {
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
                string filePath = Server.MapPath("~/Adjuntos/" + id);

                if (!System.IO.File.Exists(filePath))
                {
                    throw new FileNotFoundException();
                }

                byte[] byteArray = System.IO.File.ReadAllBytes(filePath);
                string extension = Path.GetExtension(id).ToLower();
                string mimeType;
                string fileName = $"{Path.GetFileNameWithoutExtension(id)}_{new Random().Next(10000, 99999)}{extension}";

                switch (extension)
                {
                    case ".pdf":
                        mimeType = "application/pdf";
                        break;
                    case ".txt":
                        mimeType = "text/plain";
                        break;
                    case ".jpg":
                        mimeType = "image/jpeg";
                        break;
                    case ".png":
                        mimeType = "image/png";
                        break;
                    default:
                        mimeType = "application/octet-stream"; // Tipo genérico para otros archivos
                        fileName = id; // Mantener el nombre original si no es una extensión reconocida
                        break;
                }

                return File(byteArray, mimeType, fileName);
            }
            catch
            {
                string text = "Error: No se pudo descargar el archivo.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));
                return File(stream, "text/plain", "Error.txt");
            }
        }

        public int ObtenerHoraSLA(int idTicket = 0)
        {
            return db.ObtenerHoraSLA(idTicket).Single().TiempoAtencion;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateEdata(DETAILS_TICKET details_ticket, IEnumerable<HttpPostedFileBase> files, IEnumerable<HttpPostedFileBase> archivosSubidos)
        {
            string impactoServicio = Request.Form["ImpactoServicio"];
            string opcionProgramar = Request.Form["opcionesprogramar"];
            bool chkControlRemoto = false;
            bool chkTipoResolucion = false;
            string estado = string.Empty;
            string KEY_ATTA = string.Empty;
            int ID_ACCOval = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            string flagCorreo = "";
            string KEY_ATTA_correo = "";

            if (details_ticket.ID_STAT == 0)
            {
                details_ticket.ID_STAT = null;
            }
            /*Registro de Lección aprendida - Gestión de Conocimiento*/
            /*Verificación de tipo de ticket*/

            int id_ticket = Convert.ToInt32(details_ticket.ID_TICK);/*Obtenemos el idTicket*/
            int IdEstadoAnterior = Convert.ToInt32(db.TICKETs.Where(x => x.ID_TICK == id_ticket).Single().ID_STAT_END);

            var resultTipo = db.TICKETs.Where(t => t.FlagProblema == true && t.ID_TICK == details_ticket.ID_TICK).FirstOrDefault();/* (validamos si es un ticket problema)*/
            if (db.TICKETs.Single(t => t.ID_TICK == id_ticket).FlagProblema == null)
                chkTipoResolucion = Request.Params["TipoResolucion"].ToString() == "false" ? false : true;
            int idEtapa = 0, idActividad = 0, idSubcategoria = 0;
            bool chkCtrlRemotoAct = false;

            // validaciones
            if (UserId == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.perdioSesion();}window.onload=init;</script>");
            }
            if (impactoServicio == "" && details_ticket.ID_STAT == 4)
            {
                return Content("<script type='text/javascript'> function init() {" +
    "if(top.uploadDone) top.uploadDone('ERROR','Seleccione el impacto del Servicio.');}window.onload=init;</script>");
            }

            //if (details_ticket.COM_DETA_TICK == null)
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //            "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar un comentario.');}window.onload=init;</script>");
            //}

            //Valida si es una actividad
            if (details_ticket.ID_TYPE_DETA_TICK == 1 && details_ticket.ID_STAT is null)
            {
                //Valida si es un ticket proyecto
                if (!(db.TICKETs.Single(x => x.ID_TICK == id_ticket).ID_DOCU_SALE is null))
                {
                    //Subcategorizacion de proyectos
                    if (Request.Params["cboEtapa"] == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Seleccione la etapa.');}window.onload=init;</script>");
                    }
                    else { idEtapa = Convert.ToInt32(Request.Params["cboEtapa"]); }
                    if (Request.Params["cboActividad"] == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Seleccione la actividad.');}window.onload=init;</script>");
                    }
                    else { idActividad = Convert.ToInt32(Request.Params["cboActividad"]); }
                    if (Request.Params["cboSubcategoria"] == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Seleccione la subcategoría.');}window.onload=init;</script>");
                    }
                    else { idSubcategoria = Convert.ToInt32(Request.Params["cboSubcategoria"]); }
                    //chkCtrlRemotoAct = Request.Params["ControlRemotoAct"].ToString() == "false" ? false : true;
                }
            }

            /*categorías obtenidas*/
            if (details_ticket.ID_STAT == 4) /*Verificamos si se encuentra con el estado resuelto*/
            {
                if (resultTipo != null)/*Verificamos si es un ticket problema*/
                {
                    var objResultLeccionAprendida = db.ComLeccionAprendidas.Where(l => l.ID_TICKET == id_ticket && l.EstadoAprobacion == "A").FirstOrDefault();
                    int CatServiceDiarios = Convert.ToInt32(resultTipo.ID_CATE);
                    int CatClase = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == CatServiceDiarios).FirstOrDefault().ID_CATE_PARE);
                    int CatSubCategoria = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == CatClase).FirstOrDefault().ID_CATE_PARE);
                    int Categoria = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == CatSubCategoria).FirstOrDefault().ID_CATE_PARE);

                    if (details_ticket.Titulo == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un título');}window.onload=init;</script>");
                    }
                    if (details_ticket.DescripcionProblema == null || details_ticket.Porque2 == null
                        || details_ticket.Porque3 == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese por lo menos tres porqués.');}window.onload=init;</script>");
                    }
                    if ((details_ticket.DescripcionProblema).Length <= 3 || (details_ticket.Porque2).Length <= 3
                        || (details_ticket.Porque3).Length <= 3)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese como mínimo 4 caracteres en impacto en el negocio.');}window.onload=init;</script>");
                    }

                    if (details_ticket.Impactonegocio == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un impacto a la lección');}window.onload=init;</script>");
                    }
                    if (details_ticket.SolucionTemporal == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese una solución temporal');}window.onload=init;</script>");
                    }
                    if ((details_ticket.Impactonegocio).Length <= 3)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese como mínimo 4 caracteres en impacto en el negocio.');}window.onload=init;</script>");
                    }
                    if ((details_ticket.SolucionTemporal).Length <= 3)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ingrese como mínimo 4 caracteres en la solución temporal');}window.onload=init;</script>");
                    }
                    KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);

                    Electrodata.ERPElectrodata.Domain.Entities.ComLeccionAprendida objLeccionAprendida = new Electrodata.ERPElectrodata.Domain.Entities.ComLeccionAprendida();

                    if (objResultLeccionAprendida == null)
                    {
                        estado = "Registró";

                        int tipoOperacion = Convert.ToInt32(Tipo_Peracion.OPERATION_REGISTER);

                        objLeccionAprendida.ID_TICKET = details_ticket.ID_TICK;
                        objLeccionAprendida.IdTema = resultTipo.IdTema;
                        objLeccionAprendida.Titulo = details_ticket.Titulo;
                        objLeccionAprendida.Nivel1 = Categoria;
                        objLeccionAprendida.Nvel2 = CatSubCategoria;
                        objLeccionAprendida.Nivel3 = CatClase;
                        objLeccionAprendida.Nivel4 = CatServiceDiarios;
                        objLeccionAprendida.DescripcionProblema = details_ticket.DescripcionProblema;
                        objLeccionAprendida.SolucionAplicada = "";
                        objLeccionAprendida.Impactonegocio = details_ticket.Impactonegocio;
                        objLeccionAprendida.SolucionTemporal = details_ticket.SolucionTemporal;
                        objLeccionAprendida.Porque2 = details_ticket.Porque2;
                        objLeccionAprendida.Porque3 = details_ticket.Porque3;
                        objLeccionAprendida.Porque4 = details_ticket.Porque4;
                        objLeccionAprendida.Porque5 = details_ticket.Porque5;
                        objLeccionAprendida.SolucionDefinitiva = string.IsNullOrEmpty(details_ticket.SolucionDefinitiva) ? "" : details_ticket.SolucionDefinitiva;
                        result = _srvLeccionAprendidaService.OperationLeccionAprendidaBusinessProblemas(objLeccionAprendida, tipoOperacion, KEY_ATTA, Convert.ToInt32(Session["ID_ACCO"]), Session["UserName"].ToString(), Convert.ToInt32(Session["APROBADOR_OPERACIONES"]), Convert.ToInt32(Session["UserId"]));
                    }
                    else
                    {
                        estado = "Modificó";

                        int tipoOperacion = Convert.ToInt32(Tipo_Peracion.OPERATION_MODIFY);

                        objLeccionAprendida.IdLeccioNAprendida = objResultLeccionAprendida.IdLeccioNAprendida;
                        objLeccionAprendida.ID_TICKET = details_ticket.ID_TICK;
                        objLeccionAprendida.IdTema = resultTipo.IdTema;
                        objLeccionAprendida.Titulo = details_ticket.Titulo;
                        objLeccionAprendida.Nivel1 = Categoria;
                        objLeccionAprendida.Nvel2 = CatSubCategoria;
                        objLeccionAprendida.Nivel3 = CatClase;
                        objLeccionAprendida.Nivel4 = CatServiceDiarios;
                        objLeccionAprendida.DescripcionProblema = details_ticket.DescripcionProblema;
                        objLeccionAprendida.Porque2 = details_ticket.Porque2;
                        objLeccionAprendida.Porque3 = details_ticket.Porque3;
                        objLeccionAprendida.Porque4 = details_ticket.Porque4;
                        objLeccionAprendida.Porque5 = details_ticket.Porque5;
                        objLeccionAprendida.SolucionAplicada = "";
                        objLeccionAprendida.Impactonegocio = details_ticket.Impactonegocio;
                        objLeccionAprendida.SolucionTemporal = details_ticket.SolucionTemporal;
                        objLeccionAprendida.SolucionDefinitiva = string.IsNullOrEmpty(details_ticket.SolucionDefinitiva) ? "" : details_ticket.SolucionDefinitiva;
                        result = _srvLeccionAprendidaService.OperationLeccionAprendidaBusinessProblemas(objLeccionAprendida, tipoOperacion, KEY_ATTA, Convert.ToInt32(Session["ID_ACCO"]), Session["UserName"].ToString(), Convert.ToInt32(Session["APROBADOR_OPERACIONES"]), Convert.ToInt32(Session["UserId"]));
                    }

                    if (result == (int)Tipo_Peracion.OPERATION_ERROR)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Contacte al Administrador.');}window.onload=init;</script>");
                    }
                }
            }

            /*Fin*/


            int id = Convert.ToInt32(Request.Form["ID_TICK"]);
            var objTicket = db.TICKETs.Single(t => t.ID_TICK == id);
            string cod_tick = Convert.ToString(objTicket.COD_TICK);
            //string KEY_ATTA = null;
            //PRIMERO VALIDAR SI TIENE HIJOS SIN CLOSE

            try
            {
                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);
            }
            catch
            {

            }

            var AttachsGTI = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                   .Where(x => x.ID_DETA_INCI == null).ToList();
            if (details_ticket.IdRazon == 13)
            {
                if (AttachsGTI == null || !AttachsGTI.Any())
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Debe adjuntar imagen de aprobación.');}window.onload=init;</script>");
                }

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                for (int i = AttachsGTI.Count - 1; i >= 0; i--)
                {
                    var AttachsGTIs = AttachsGTI[i];

                    // Check if the file extension is not an image format
                    if (allowedImageExtensions.Any(ext => AttachsGTIs.EXT_ATTA.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        break;
                    }
                    else
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                           "if(top.uploadDone) top.uploadDone('ERROR','Solo se permiten archivos de imagen.');}window.onload=init;</script>");
                    }
                }

            }

            int CantNoClose = 0;

            if (details_ticket.ID_STAT == 4)
            {
                try
                {
                    var cont = db.TICKETs.Where(t => t.ID_TICK_PARENT == id);

                    CantNoClose = (from t in cont.ToList()
                                   join st in db.STATUS on t.ID_STAT_END equals st.ID_STAT
                                   join cc in db.ConfiguracionCategorias on t.ID_CATE equals cc.IdCate4 into ccGroup
                                   from cc in ccGroup.DefaultIfEmpty()
                                   where cc == null && st.ID_STAT != 4 && st.ID_STAT != 6 && st.ID_STAT != 2 && t.ID_TYPE_FORM != 1
                                   select new
                                   {

                                   }).Count();
                }
                catch
                {

                }

                if (CantNoClose != 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','Tienes tickets Abiertos');}window.onload=init;</script>");
                }
            }
            //var DETA_INCI = new DETAILS_INCIDENT();
            int j;

            if (details_ticket.ID_TYPE_DETA_TICK == 1 && details_ticket.ID_STAT is null)
            {
                if (details_ticket.Causa == true)
                {
                    var ListaComentarios = db.DETAILS_TICKET.Where(x => x.ID_TICK == id && x.Causa == true).ToList();
                    if (ListaComentarios.Count() > 0)
                    {
                        details_ticket.Causa = false;
                    }
                }

                if (String.IsNullOrEmpty(details_ticket.COM_DETA_TICK))
                {
                    //return Content("<script>alert('Ingrese Su Comentario');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un Comentario');}window.onload=init;</script>");

                }

                //VALIDANDO DATA FECHA NULA O VACÍA


                if (string.IsNullOrEmpty(Convert.ToString(details_ticket.TO_TIME)) || string.IsNullOrEmpty(Convert.ToString(details_ticket.FROM_TIME)))
                {
                    //return Content("<script>alert('Ingrese Su Comentario');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Ingresar las fechas y horas de su actividad real correctamente.');}window.onload=init;</script>");
                }


                // No inclusion de Fechas Negativas
                int RestaFecha = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;

                if (RestaFecha < 0)
                {
                    //return Content("<script>alert('Ingrese Su Comentario');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Ingresar las fechas y horas de su actividad real correctamente.');}window.onload=init;</script>");
                }


            }
            else if (details_ticket.ID_TYPE_DETA_TICK == 2)
            {


                if (details_ticket.ID_PERS_ENTI == null)
                {
                    //return Content("<script>alert('Seleccione Personal');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Seleccione una persona');}window.onload=init;</script>");
                }

            }
            else if (details_ticket.ID_TYPE_DETA_TICK == 3 || (details_ticket.ID_TYPE_DETA_TICK == 1 && details_ticket.ID_STAT != null))
            {
                details_ticket.ID_TYPE_DETA_TICK = 3;
                if (details_ticket.ID_STAT == null)
                {
                    //return Content("<script>alert('Seleccione Status');</script>");
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Seleccione un Estado');}window.onload=init;</script>");
                }
                else
                {
                    if (opcionProgramar == "pausa")
                    {
                        if (details_ticket.ID_STAT == 5 && objTicket.ID_STAT_END == 6)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                            "if(top.uploadDone) top.uploadDone('ERROR','No se puede programar un ticket cerrado');}window.onload=init;</script>");
                        }

                    }
                    else
                    {
                        if (details_ticket.ID_STAT == 5 && objTicket.ID_STAT_END == 6)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                            "if(top.uploadDone) top.uploadDone('ERROR','No se puede programar un ticket cerrado');}window.onload=init;</script>");
                        }
                        else if (details_ticket.ID_STAT == 5 && details_ticket.FEC_SCHE == null)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                            "if(top.uploadDone) top.uploadDone('ERROR','Seleccione una fecha para programar');}window.onload=init;</script>");
                        }
                        else if (details_ticket.ID_STAT == 5 && details_ticket.FEC_SCHE < DateTime.Now)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                            "if(top.uploadDone) top.uploadDone('ERROR','Seleccione una fecha para programar, mayor al dia de hoy');}window.onload=init;</script>");
                        }
                    }

                }

            }


            /*Gestión Problemas*/
            if (details_ticket.ID_STAT == 4)
            {
                if (resultTipo == null)
                {
                    //if(chkTipoResolucion == true) // si es solucion temporal no envia correo
                    //{
                    //    if (String.IsNullOrEmpty(details_ticket.COM_DETA_TICK))
                    //    {


                    //        //details_ticket.COM_DETA_TICK = "Se envió correctamente el correo con los siguientes datos:</br>● Para: " + correos + "</br>● En copia: " + cc + "</br>● Asunto: " + asunto + "</br>";
                    //        //return Content("<script>alert('Ingrese Su Comentario');</script>");
                    //        return Content("<script type='text/javascript'> function init() {" +
                    //    "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un Comentario');}window.onload=init;</script>");
                    //    }
                    //}
                    //else
                    //{
                    string correos, cc, asunto, cuerpo;

                    correos = Request.Params["correos"];
                    cc = Request.Params["cc"];
                    asunto = Request.Params["asunto"];
                    cuerpo = Request.Params["cuerpo"];

                    if (String.IsNullOrEmpty(details_ticket.COM_DETA_TICK))
                    {


                        details_ticket.COM_DETA_TICK = "Se resolvió ticket correctamente. </br> Se envió el correo con los siguientes datos: </br> <b>Para:</b> " + correos + " </br> <b>En copia:</b> " + cc + "</br> <b>Asunto:</b>  " + asunto + "</br> <b>Contenido:</b> </br> " + cuerpo;
                        //return Content("<script>alert('Ingrese Su Comentario');</script>");
                        //    return Content("<script type='text/javascript'> function init() {" +
                        //"if(top.uploadDone) top.uploadDone('ERROR','Ingrese un Comentario');}window.onload=init;</script>");
                    }

                    try
                    {
                        // Configuración de seguridad
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                        // Configuración de Brevo
                        var datosBrevo = db.RemitentesEdata(objTicket.ID_QUEU).SingleOrDefault();
                        if (datosBrevo == null)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','No se pudieron obtener los datos de Brevo');}window.onload=init;</script>");
                        }

                        string apiKey = datosBrevo.ApiKey;
                        string fromEmail = datosBrevo.Remitente;

                        // Configuración del cliente de correo
                        var client = new RestClient("https://api.sendinblue.com/v3/smtp/email");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("accept", "application/json");
                        request.AddHeader("api-key", apiKey);
                        request.AddHeader("Content-Type", "application/json");

                        // Preparar destinatarios
                        var destinatarios = correos
                            .Split(',')
                            .Select(email => new { email = email.Trim() })
                            .ToList();

                        var copias = string.IsNullOrEmpty(cc)
                            ? null
                            : cc.Split(',').Select(email => new { email = email.Trim() }).ToList();

                        // Crear la estructura del correo
                        var emailData = new Dictionary<string, object>
                            {
                                { "sender", new { email = fromEmail, name = "IT Management System" } },
                                { "to", destinatarios },
                                { "cc", copias },
                                { "subject", asunto },
                                { "htmlContent", cuerpo }
                            };

                        // Procesar los adjuntos
                        var attachments = new List<object>();

                        if (archivosSubidos != null)
                        {
                            details_ticket.COM_DETA_TICK = details_ticket.COM_DETA_TICK + "</br></br><b> Cantidad de archivos adjuntos: " + archivosSubidos.Count() + "</b>";
                            foreach (var file in archivosSubidos)
                            {

                                try
                                {
                                    //Obtenemos nombre de archivo
                                    string name_atta = Path.GetFileNameWithoutExtension(file.FileName);
                                    //Obtenemos extensión de archivo
                                    string extension = Path.GetExtension(file.FileName);
                                    KEY_ATTA_correo = "M1DT" + Convert.ToString(DateTime.Now.Ticks);

                                    //Buscamos para identificar si el archivo enviado ya se ha guardado.
                                    int repetido = db.ATTACHEDs
                                        .Where(x => x.DELETE_ATTA == false)
                                        .Where(x => x.KEY_ATTA == KEY_ATTA_correo)
                                        .Where(x => x.NAM_ATTA == name_atta)
                                        .Where(x => x.EXT_ATTA == extension).Count();

                                    //Si el archivo no se ha repetido procedemos a guardarlo.
                                    if (repetido == 0)
                                    {
                                        //Creamos el registro en la base de datos.
                                        var ATTA = new ATTACHED();
                                        ATTA.NAM_ATTA = name_atta;
                                        ATTA.EXT_ATTA = extension;
                                        ATTA.CREATE_ATTA = DateTime.Now;
                                        ATTA.KEY_ATTA = KEY_ATTA_correo;
                                        ATTA.ID_TYPE_DOCU_ATTA = 29;
                                        ATTA.DELETE_ATTA = false;
                                        db.ATTACHEDs.Add(ATTA);
                                        db.SaveChanges();

                                        //Guardamos el archivo en la ruta del servidor y se le añade el id (ID_ATTA) generado
                                        file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                                    }

                                    else
                                    {
                                        return Content("Filename Exist");
                                    }
                                }
                                catch
                                {
                                    return Content("ERROR");
                                }


                                if (file.ContentLength > 0)
                                {
                                    details_ticket.COM_DETA_TICK = details_ticket.COM_DETA_TICK + "</br> -" + file.FileName;
                                    db.SaveChanges();
                                    using (var ms = new MemoryStream())
                                    {
                                        file.InputStream.CopyTo(ms);
                                        string fileBase64 = Convert.ToBase64String(ms.ToArray());

                                        attachments.Add(new
                                        {
                                            name = file.FileName,
                                            content = fileBase64
                                        });
                                    }
                                }
                            }

                            if (attachments.Any())
                            {
                                emailData.Add("attachment", attachments);
                            }
                        }


                        request.AddJsonBody(emailData);

                        // Ejecutar la solicitud
                        IRestResponse response = client.Execute(request);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                        {

                        }
                        else
                        {

                            details_ticket.COM_DETA_TICK = "Se resolvió ticket correctamente. </br> Error al enviar correo. </br> <b>Contenido de correo:<b/>" + cuerpo;
                            flagCorreo = "Error enviando correo.";
                            //Console.WriteLine("Error en la respuesta de Brevo: " + response.Content);
                            //return Content("<script type='text/javascript'> function init() {" +
                            //            "if(top.uploadDone) top.uploadDone('ERROR','Error enviando correo.');}window.onload=init;</script>");
                        }

                    }
                    catch (Exception e)
                    {

                        details_ticket.COM_DETA_TICK = "Se resolvió ticket correctamente. </br> Error al enviar correo. </br> - Contenido de correo:" + cuerpo;

                        flagCorreo = "Error enviando correo.";
                        //Console.WriteLine("Error enviando correo: " + e.Message);
                        //return Content("<script type='text/javascript'> function init() {" +
                        //               "if(top.uploadDone) top.uploadDone('ERROR','Error enviando correo: " + e.Message + "');}window.onload=init;</script>"); ;
                    }
                    //}





                }
                else
                {
                    details_ticket.COM_DETA_TICK = "<a target='_blank' href='KnowledgeManagement/EditLessonLearned/" + result.ToString() + "'" + "  style='text-decoration:none;'><span class='glyphicon glyphicon-play' aria-hidden='true'></span><span>Se " + estado + " la lección aprendida Nro: " + result.ToString() + " </span></a>";
                    // "El ticket problema fue cerrado de manera satisfactoria, y se realizó la lección aprendida correspondiente N° " + result.ToString();
                }
            }
            details_ticket.COM_DETA_TICK = String.IsNullOrEmpty(details_ticket.COM_DETA_TICK) ? "" : details_ticket.COM_DETA_TICK;
            /*Fin*/
            //---------------------PARA LOS POINTS-------------------------------------

            var statusticket = db.TICKETs.Single(x => x.ID_TICK == details_ticket.ID_TICK);
            int idstat = Convert.ToInt32(statusticket.ID_STAT);
            int idstatend = Convert.ToInt32(statusticket.ID_STAT_END);
            int idtick = Convert.ToInt32(details_ticket.ID_TICK);
            int rejecttick = 0;

            if ((idstatend == 6 || idstatend == 4))
            {
                rejecttick = 1;

            }
            //---------------------------------------------------------------------------

            if (details_ticket.ID_TYPE_DETA_TICK == 3 || details_ticket.ID_TYPE_DETA_TICK == 2)
            {
                details_ticket.SEND_MAIL = false;
            }

            details_ticket.ID_TICK = id;
            details_ticket.UserId = UserId;
            details_ticket.CREATE_DETA_INCI = DateTime.Now;

            var ticket = db.TICKETs.Where(x => x.ID_TICK == id).Single();
            if (details_ticket.ID_STAT == 5 && opcionProgramar == "pausa")
            {
                details_ticket.ID_STAT = 9;
            }
            else
            {
                ticket.EsperaPorCliente = details_ticket.EsperaPorCliente;
            }

            if (details_ticket.ID_STAT == 4) /*Verificamos si se encuentra con el estado resuelto*/
            {
                ticket.REM_CTRL_TICK = chkControlRemoto;

                if (resultTipo != null)/*Verificamos si es un ticket problema*/
                {
                    /*Gestión de problemas*/
                    if (string.IsNullOrEmpty(details_ticket.SolucionDefinitiva))
                    {
                        details_ticket.ID_STAT = 8;
                        /*Si el campo SolucionDefinitiva está vacío el ticket se registra con estado */
                    }
                    else
                    {
                        if ((details_ticket.SolucionDefinitiva).Length <= 3)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Ingrese como mínimo 4 caracteres en la solución definitiva.');}window.onload=init;</script>");
                        }
                        else
                        {
                            var cambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == id);
                            if (cambio.Count() == 1)
                            {

                                int idCambio = Convert.ToInt32(cambio.SingleOrDefault().id);
                                var estadoCambio = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == idCambio).SingleOrDefault();

                                if (estadoCambio.idTypeRequest != 5 && estadoCambio.idTypeRequest != 3)
                                {
                                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('ERROR','La solicitud del cambio debe estar ejecutada antes de resolver el ticket.');}window.onload=init;</script>");
                                }
                            }
                        }
                    }
                }
                else
                {
                    var cambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == id);
                    if (cambio.Count() == 1)
                    {

                        int idCambio = Convert.ToInt32(cambio.SingleOrDefault().id);
                        var estadoCambio = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == idCambio).SingleOrDefault();

                        if (estadoCambio.idTypeRequest != 5 && estadoCambio.idTypeRequest != 3)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','La solicitud del cambio debe estar ejecutada antes de resolver el ticket.');}window.onload=init;</script>");
                        }
                    }
                }
            }
            /*Fin*/



            //Actualizando el estado del ticket
            if (details_ticket.ID_TYPE_DETA_TICK == 3)
            {
                ticket.ID_STAT_END = details_ticket.ID_STAT;
                if (ticket.ID_STAT_END == 4 && ticket.ID_PERS_ENTI_END == 63132 && ticket.ID_ACCO == 4)
                {
                    List<int> compList = new List<int>(new[] { 67136, 71236, 71251, 71249, 71254, 71260, 71266, 71263, 71241, 71245, 71243, 71269, 71257 });
                    if (compList.Contains(ticket.ID_COMP_END.Value))
                    {
                        ticket.SEND_SURVEY = false;
                    }

                }

            }

            //Validar Motivo Espera
            int ticketProblemaME = Convert.ToInt32(Request.Form["TicketProblemaME"]);

            if (ticketProblemaME == 0 && details_ticket.ID_TYPE_DETA_TICK == 3 && (details_ticket.ID_STAT == 5 || details_ticket.ID_STAT == 9))
            {
                string idMotivo = Request.Form["IdMotivoEstadoProgra"];
                if (string.IsNullOrEmpty(idMotivo) && details_ticket.IdMotivoEstado == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Seleccione el motivo de espera.');}window.onload=init;</script>");
                }
                else
                {
                    if (details_ticket.IdMotivoEstado == null)
                        details_ticket.IdMotivoEstado = Convert.ToInt32(idMotivo);

                    ticket.IdMotivoEstado = details_ticket.IdMotivoEstado;
                }

            }

            db.Entry(ticket).State = EntityState.Modified;


            if (details_ticket.ID_STAT == 4)
            {
                if (chkTipoResolucion == true)
                    details_ticket.IdTipoResolucion = 1;
                else
                    details_ticket.IdTipoResolucion = 2;

                objTicket.Solucion = details_ticket.COM_DETA_TICK;
                db.Entry(objTicket).State = EntityState.Modified;
                db.SaveChanges();

            }


            db.DETAILS_TICKET.Add(details_ticket);
            db.SaveChanges();

            var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA_correo)
                    .Where(x => x.ID_DETA_INCI == null).ToList();
            if (Attachs.Count() > 0)
            {
                foreach (ATTACHED attach in Attachs)
                {
                    
                        var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                        EObj.ID_DETA_INCI = details_ticket.ID_DETA_TICK;
                        db.Entry(EObj).State = EntityState.Modified;
                        db.SaveChanges();
                        db.Entry(EObj).State = EntityState.Detached;
                    
                }
            }

                        // ACTUALIZAR CORTES DE TIEMPOS DE NIVEL DE SERVICIO
            db.ActualizarTiemposSLA(idtick, IdEstadoAnterior, details_ticket.ID_STAT, details_ticket.FEC_SCHE, UserId);


            int ID_DETA_TICK = details_ticket.ID_DETA_TICK;
            db.Entry(details_ticket).State = EntityState.Detached;

            if (details_ticket.ID_STAT == 4)
            {
                if (ticket.ID_CATE == 23199 && ticket.ID_QUEU == 105)
                {
                    TicketController ticketController = new TicketController();
                    ticketController.ControllerContext = this.ControllerContext;

                    ticketController.EnviarCorreoSolcucionAccesoUSB();
                }

                //cambiar el estado de informe cuando este se cierre:
                if (ticket.ID_CATE == 13973)
                {
                    int IdInforme = (from ie in db.InformeEDDetalle
                                     join tic in db.TICKETs on (ie.Id == null ? 0 : ie.Id) equals tic.IdInforme into ctxsd
                                     from ctxs in ctxsd.DefaultIfEmpty()
                                     where ctxs.ID_TICK == ticket.ID_TICK
                                     select new
                                     {
                                         IdInforme = ie.Id
                                     }
                                                          ).Single().IdInforme;

                    InformeEDDetalle objInformeDetalle = db.InformeEDDetalle.Where(x => x.Id == IdInforme).FirstOrDefault();

                    objInformeDetalle.IdEstadoInforme = 7;
                    db.SaveChanges();


                }

                //Bolsa de Horas
                if (ticket.ID_ACCO == 4)
                {
                    bool checkConsumoHoras = Request.Params["AplicaConsumoHoras"] != null && Request.Params["AplicaConsumoHoras"].ToString() == "on";

                    int MinutosEfectiv = string.IsNullOrEmpty(Request.Params["MinutosEfectivos"]) ? 0 : Convert.ToInt32(Request.Params["MinutosEfectivos"]);

                    DETAILS_TICKET detailTicketBolsa = db.DETAILS_TICKET.Find(ID_DETA_TICK);
                    detailTicketBolsa.COM_DETA_TICK = detailTicketBolsa.COM_DETA_TICK + " " + (MinutosEfectiv == 0 ? "" : "<br/>Ticket Efectivo para Consumo de Horas.");
                    detailTicketBolsa.MinutosBolsaHoras = MinutosEfectiv;
                    db.SaveChanges();

                    if (checkConsumoHoras == true)
                    {
                        if (MinutosEfectiv <= 0)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar la cantidad correcta de minutos efectivos para el descuento de la bolsa de horas.');}window.onload=init;</script>");
                        }
                        else
                        {
                            ticket.ConsumoHoras = checkConsumoHoras;
                            ticket.MinutosEfectivos = (ticket.MinutosEfectivos == null ? 0 : ticket.MinutosEfectivos) + MinutosEfectiv;
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        ticket.ConsumoHoras = checkConsumoHoras;
                        ticket.MinutosEfectivos = 0;
                        db.SaveChanges();
                    }


                }
            }

            int bandera = 0;
            //string rout="";
            if (KEY_ATTA != null)
            {
                var AttachsCorreo = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                    .Where(x => x.ID_DETA_INCI == null).ToList();
                if (AttachsCorreo.Count() > 0)
                {
                    foreach (ATTACHED attach in AttachsCorreo)
                    {
                        try
                        {
                            var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                            EObj.ID_DETA_INCI = ID_DETA_TICK;
                            db.Entry(EObj).State = EntityState.Modified;
                            db.SaveChanges();
                            db.Entry(EObj).State = EntityState.Detached;

                            if (ticket.IS_PARENT == true)
                            {
                                var objComentarioTicketHijo = db.DETAILS_TICKET.Where(t => t.ID_TICK == ticket.ID_TICK)
                                    .OrderByDescending(t => t.ID_DETA_TICK)
                                   .FirstOrDefault();
                                var adjuntoComentarioTicketPadre = db.ATTACHEDs.Where(t => t.ID_DETA_INCI == objComentarioTicketHijo.ID_DETA_TICK)
                                   .OrderByDescending(t => t.ID_DETA_INCI)
                                   .FirstOrDefault();

                                var objComentarioTicketPadre = db.DETAILS_TICKET.Where(t => t.ID_TICK == ticket.ID_TICK_PARENT)
                                    .OrderByDescending(t => t.ID_DETA_TICK)
                                   .FirstOrDefault();
                                var objAdjunto = new ATTACHED();

                                objAdjunto.NAM_ATTA = adjuntoComentarioTicketPadre.NAM_ATTA;
                                objAdjunto.EXT_ATTA = adjuntoComentarioTicketPadre.EXT_ATTA;
                                objAdjunto.ID_DETA_INCI = objComentarioTicketPadre.ID_DETA_TICK;
                                objAdjunto.CREATE_ATTA = DateTime.Now;
                                objAdjunto.KEY_ATTA = adjuntoComentarioTicketPadre.KEY_ATTA;
                                objAdjunto.ID_TYPE_DOCU_ATTA = adjuntoComentarioTicketPadre.ID_TYPE_DOCU_ATTA;
                                objAdjunto.DELETE_ATTA = adjuntoComentarioTicketPadre.DELETE_ATTA;
                                db.ATTACHEDs.Add(objAdjunto);
                                db.SaveChanges();
                            }
                        }
                        catch
                        {

                        }

                    }
                    bandera = 1;
                }
            }


            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (rejecttick != 1)
            {
                //-------------INSERTA EN PONIT CUANDO SE CREA UN  DETAILS TICKET --------------

                pt.UpdatePointByDetailsTicket(details_ticket, ID_PERS_ENTI, bandera);
            }

            //--------------------------CUANDO ES REABIERTO-----------------------------------------

            if (rejecttick == 1)
            {

                //--------------------------LOG COMMENT-----------------------------------
                if ((details_ticket.ID_TYPE_DETA_TICK == 1 && idstatend == 6))
                {

                    pt.UpdatePointByDetailsTicketReject(details_ticket, ID_PERS_ENTI);
                }

                if ((details_ticket.ID_STAT != 6 || details_ticket.ID_STAT != 4) && (details_ticket.ID_TYPE_DETA_TICK == 2 || details_ticket.ID_TYPE_DETA_TICK == 3))
                {
                    int ID_PERS_ENTI_ASSI = Convert.ToInt32(statusticket.ID_PERS_ENTI_ASSI);
                    pt.UpdatePointByDetailsTicketReject(details_ticket, ID_PERS_ENTI_ASSI);
                }

            }


            /*Se obtiene los códigos de categoría y tema en base al código de ticket*/
            var valores = (from c1 in db.CATEGORies
                           join c2 in db.CATEGORies on c1.ID_CATE_PARE equals c2.ID_CATE
                           join c3 in db.CATEGORies on c2.ID_CATE_PARE equals c3.ID_CATE
                           join c4 in db.CATEGORies on c3.ID_CATE_PARE equals c4.ID_CATE
                           join t in db.TICKETs on c1.ID_CATE equals t.ID_CATE
                           join ct in db.ComTemas on (t.IdTema == null ? 0 : t.IdTema) equals ct.IdTema into ctxsd  /*Agregado para la gestión del conocimiento*/
                           from ctxs in ctxsd.DefaultIfEmpty()
                           where t.ID_TICK == id
                           select new
                           {
                               c1 = c1.ID_CATE,
                               c1nombre = c1.NAM_CATE,
                               c2 = c2.ID_CATE,
                               c2nombre = c2.NAM_CATE,
                               c3 = c3.ID_CATE,
                               c3nombre = c3.NAM_CATE,
                               c4 = c4.ID_CATE,
                               c4nombre = c4.NAM_CATE,
                               idTema = t.IdTema,
                               idTicket = t.ID_TICK,
                               NAME_TEMA = (ctxs == null ? "" : ctxs.Nomtema),  /*Agregado para la gestión del conocimiento*/
                               FlagProblema = t.FlagProblema
                           }).ToList();

            if (valores.Count > 0)
            {
                nivel1 = valores.FirstOrDefault().c4 != null ? Convert.ToInt32(valores.FirstOrDefault().c4) : 0;
                nameNivel1 = valores.FirstOrDefault().c4nombre != null ? valores.FirstOrDefault().c4nombre : string.Empty;
                nivel2 = valores.FirstOrDefault().c3 != null ? Convert.ToInt32(valores.FirstOrDefault().c3) : 0;
                nameNivel2 = valores.FirstOrDefault().c3nombre != null ? valores.FirstOrDefault().c3nombre : string.Empty;
                nivel3 = valores.FirstOrDefault().c2 != null ? Convert.ToInt32(valores.FirstOrDefault().c2) : 0;
                nameNivel3 = valores.FirstOrDefault().c2nombre != null ? valores.FirstOrDefault().c2nombre : string.Empty;
                nivel4 = valores.FirstOrDefault().c1 != null ? Convert.ToInt32(valores.FirstOrDefault().c1) : 0;
                nameNivel4 = valores.FirstOrDefault().c1nombre != null ? valores.FirstOrDefault().c1nombre : string.Empty;
                idTema = valores.FirstOrDefault().idTema != null ? Convert.ToInt32(valores.FirstOrDefault().idTema) : 0;
                nameTema = valores.FirstOrDefault().NAME_TEMA != null ? valores.FirstOrDefault().NAME_TEMA : string.Empty;
                Flagproblema = valores.FirstOrDefault().FlagProblema != null ? Convert.ToBoolean(valores.FirstOrDefault().FlagProblema) : false;
            }
            /*Fin*/

            //Insertando actividades
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //Ticket Proyecto
            var proyecto = db.TicketProyecto(details_ticket.ID_TICK, details_ticket.COM_DETA_TICK, UserId).SingleOrDefault();

            var RegistraActividad = (from ta in dbe.TYPE_ACT_LOG
                                     join sta in dbe.SUBTYPE_ACTITY on ta.ID_TYPE_ACT equals sta.ID_TYPE_ACT
                                     where ta.ID_ACCO == ID_ACCO && ta.VIG_ACT == true && sta.VIG_SUB_TYPE == true
                                     select new
                                     {
                                         ta.ID_TYPE_ACT,
                                         sta.ID_SUBTYPE_ACT
                                     }).Where(x => x.ID_SUBTYPE_ACT == 2 || x.ID_SUBTYPE_ACT == 8 || x.ID_SUBTYPE_ACT == 11 || x.ID_SUBTYPE_ACT == 14 ||
                                         x.ID_SUBTYPE_ACT == 17 || x.ID_SUBTYPE_ACT == 35 || x.ID_SUBTYPE_ACT == 36 || x.ID_SUBTYPE_ACT == 37 || x.ID_SUBTYPE_ACT == 38 || x.ID_SUBTYPE_ACT == 20 ||
                                         x.ID_SUBTYPE_ACT >= 42).FirstOrDefault();

            int ID_TYPE_ACT = 0;
            string NAM_SUBTYPE_ACT = "";
            if (proyecto.Mensaje == "1")
            {
                if (objTicket.ID_CATE == 10144)
                {
                    ID_TYPE_ACT = Convert.ToInt32(dbe.TYPE_ACT_LOG.Single(x => x.DES_ACT == "SOPORTES").ID_TYPE_ACT);

                }
                else if (objTicket.ID_CATE == 13973)
                {
                    ID_TYPE_ACT = Convert.ToInt32(dbe.TYPE_ACT_LOG.Single(x => x.DES_ACT == "INFORMES").ID_TYPE_ACT);
                }
                else
                {
                    ID_TYPE_ACT = 1;
                }
                NAM_SUBTYPE_ACT = proyecto.Op;
                var qType = dbe.TipoServicio.Where(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                if (qType.Count() == 1)
                {
                    ID_TYPE_ACT = Convert.ToInt32(qType.Single().ID_TYPE_ACT);
                }
                ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                objActividad.ID_CLIE = (int)objTicket.ID_COMP;
                objActividad.ID_TYPE_ACT = ID_TYPE_ACT;
                objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                objActividad.COD_SUBTYPE_ACT = id_ticket;
                objActividad.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT;
                objActividad.DATE_INIC = details_ticket.FROM_TIME;
                objActividad.DATE_END = details_ticket.TO_TIME;
                objActividad.COMENTARIO = "";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                objActividad.USERID = UserId;
                objActividad.CREATE_ACT_LOG = DateTime.Now;
                objActividad.VIG_ACTI_LOG = true;
                objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                objActividad.CLOSE_TICKET = details_ticket.ID_STAT == 4 ? true : false;
                objActividad.SCHEDULE_TICKET = details_ticket.ID_STAT == 5 ? true : false;
                objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;
                objActividad.DETAILS_TICKETS = details_ticket.ID_DETA_TICK;
                objActividad.ID_ACCO = ID_ACCO;
                if (details_ticket.ID_TYPE_DETA_TICK == 1)
                {
                    //Valida si es un ticket proyecto
                    if (!(db.TICKETs.Single(x => x.ID_TICK == id_ticket).ID_DOCU_SALE is null))
                    {
                        objActividad.IdEtapa = idEtapa;
                        objActividad.IdActividad = idActividad;
                        objActividad.IdSubcategoria = idSubcategoria;
                        objActividad.ActividadRemota = chkCtrlRemotoAct;
                    }
                }

                dbe.ACTIVITY_LOG.Add(objActividad);
                dbe.SaveChanges();
            }
            else if (RegistraActividad != null)
            {
                ID_TYPE_ACT = RegistraActividad.ID_TYPE_ACT;
                var qType = dbe.TipoServicio.Where(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                if (qType.Count() == 1)
                {
                    ID_TYPE_ACT = Convert.ToInt32(qType.Single().ID_TYPE_ACT);
                }
                NAM_SUBTYPE_ACT = objTicket.COD_TICK;

                ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                objActividad.ID_CLIE = (int)objTicket.ID_COMP;
                objActividad.ID_TYPE_ACT = ID_TYPE_ACT;
                objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                objActividad.COD_SUBTYPE_ACT = id_ticket;
                objActividad.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT;
                objActividad.DATE_INIC = details_ticket.FROM_TIME;
                objActividad.DATE_END = details_ticket.TO_TIME;
                objActividad.COMENTARIO = "";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                objActividad.USERID = UserId;
                objActividad.CREATE_ACT_LOG = DateTime.Now;
                objActividad.VIG_ACTI_LOG = true;
                objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                objActividad.CLOSE_TICKET = details_ticket.ID_STAT == 4 ? true : false;
                objActividad.SCHEDULE_TICKET = details_ticket.ID_STAT == 5 ? true : false;
                objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;
                objActividad.DETAILS_TICKETS = details_ticket.ID_DETA_TICK;
                objActividad.ID_ACCO = ID_ACCO;
                if (details_ticket.ID_TYPE_DETA_TICK == 1)
                {
                    //Valida si es un ticket proyecto
                    if (!(db.TICKETs.Single(x => x.ID_TICK == id_ticket).ID_DOCU_SALE is null))
                    {
                        objActividad.IdEtapa = idEtapa;
                        objActividad.IdActividad = idActividad;
                        objActividad.IdSubcategoria = idSubcategoria;
                        objActividad.ActividadRemota = chkCtrlRemotoAct;
                    }
                }
                dbe.ACTIVITY_LOG.Add(objActividad);
                dbe.SaveChanges();
            }
            //CAMBIO DE ESTADO Y GUARDADO DEL DETAIL DEL PADRE
            if (details_ticket.ID_STAT == 4 || details_ticket.ID_STAT == 2) //SI SE ENVIA RESUELTO O CANCELADO 
            {
                var codparent = db.CierreDeTickets(id).SingleOrDefault(); //REVISAR Y CERRAR EL TICKET PADRE

                //ACTIVIDAD LOG PARA PADRE.

                if (codparent.iddetatick != 0) //SI SE CREA UN DETALLE = SE CIERRA POR TICKETS HIJOS CERRADOS
                { //FALTA ESPECIFICAR SALIDA
                    var proyectPadre = db.TicketProyecto(codparent.tickparent, "PARENT TICKET RESOLVED AUTOMATICALLY BY THE SYSTEM", UserId).SingleOrDefault();//para el padre //PRUEBA COMENTARIO A REVISAR ||details_ticket.COM_DETA_TICK
                    var objParentTicket = db.TICKETs.Single(t => t.ID_TICK == codparent.tickparent);

                    if (proyectPadre.Mensaje == "1")
                    {
                        if (objParentTicket.ID_CATE == 10144)
                        {
                            ID_TYPE_ACT = Convert.ToInt32(dbe.TYPE_ACT_LOG.Single(x => x.DES_ACT == "SOPORTES").ID_TYPE_ACT);

                        }
                        else if (objParentTicket.ID_CATE == 13973)
                        {
                            ID_TYPE_ACT = Convert.ToInt32(dbe.TYPE_ACT_LOG.Single(x => x.DES_ACT == "INFORMES").ID_TYPE_ACT);
                        }
                        else
                        {
                            ID_TYPE_ACT = 1;
                        }

                        NAM_SUBTYPE_ACT = proyectPadre.Op;
                        var qType = dbe.TipoServicio.Where(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                        if (qType.Count() == 1)
                        {
                            ID_TYPE_ACT = Convert.ToInt32(qType.Single().ID_TYPE_ACT);
                        }
                        ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                        objActividad.ID_CLIE = (int)objParentTicket.ID_COMP;
                        objActividad.ID_TYPE_ACT = ID_TYPE_ACT;
                        objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                        objActividad.COD_SUBTYPE_ACT = codparent.tickparent; //nro de ticket
                        objActividad.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT;
                        objActividad.DATE_INIC = details_ticket.FROM_TIME;
                        objActividad.DATE_END = details_ticket.TO_TIME;
                        objActividad.COMENTARIO = "CIERRE AUTOMATICO DE TICKETS . NO EXISTEN TICKETS ABIERTOS";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                        objActividad.USERID = UserId;
                        objActividad.CREATE_ACT_LOG = DateTime.Now;
                        objActividad.VIG_ACTI_LOG = true;
                        objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                        objActividad.CLOSE_TICKET = true;
                        objActividad.SCHEDULE_TICKET = false;
                        objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;
                        objActividad.DETAILS_TICKETS = codparent.iddetatick;
                        objActividad.ID_ACCO = ID_ACCO;
                        dbe.ACTIVITY_LOG.Add(objActividad);
                        dbe.SaveChanges();
                    }
                    else if (RegistraActividad != null)
                    {
                        ID_TYPE_ACT = RegistraActividad.ID_TYPE_ACT;
                        var qType = dbe.TipoServicio.Where(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                        if (qType.Count() == 1)
                        {
                            ID_TYPE_ACT = Convert.ToInt32(qType.Single().ID_TYPE_ACT);
                        }
                        NAM_SUBTYPE_ACT = objParentTicket.COD_TICK;

                        ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                        objActividad.ID_CLIE = (int)objParentTicket.ID_COMP;
                        objActividad.ID_TYPE_ACT = ID_TYPE_ACT;
                        objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                        objActividad.COD_SUBTYPE_ACT = codparent.tickparent;
                        objActividad.NAM_SUBTYPE_ACT = NAM_SUBTYPE_ACT;
                        objActividad.DATE_INIC = details_ticket.FROM_TIME;
                        objActividad.DATE_END = details_ticket.TO_TIME;
                        objActividad.COMENTARIO = "CIERRE AUTOMATICO DE TICKETS . NO EXISTEN TICKETS ABIERTOS";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                        objActividad.USERID = UserId;
                        objActividad.CREATE_ACT_LOG = DateTime.Now;
                        objActividad.VIG_ACTI_LOG = true;
                        objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                        objActividad.CLOSE_TICKET = true;
                        objActividad.SCHEDULE_TICKET = false;
                        objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(details_ticket.TO_TIME).Subtract(Convert.ToDateTime(details_ticket.FROM_TIME))).TotalSeconds;
                        objActividad.DETAILS_TICKETS = codparent.iddetatick;
                        objActividad.ID_ACCO = ID_ACCO;
                        dbe.ACTIVITY_LOG.Add(objActividad);
                        dbe.SaveChanges();
                    }
                }

            }
            // Habilitar envio de correos (SEND MAIL PARA MINSUR SAP) 
            if (details_ticket.ID_TYPE_DETA_TICK == 1)
            {
                if (db.ObtenerColaSAP(ID_DETA_TICK).Single().flagSAP == 1)
                {
                    var dTicket = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
                    dTicket.SEND_MAIL = false;
                    db.Entry(dTicket).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            


            if (flagCorreo == "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + cod_tick + "','" + nivel1 + "','" + nivel2 + "','" + nivel3 + "','" + nivel4 + "','" + idTema + "','" + idtick + "','" + nameNivel1 + "','" + nameNivel2 + "','" + nameNivel3 + "','" + nameNivel4 + "','" + nameTema + "','" + details_ticket.ID_STAT + "','" + Flagproblema + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','" + flagCorreo + "');}window.onload=init;</script>");
            }


        }

        [Authorize]
        public JsonResult ListarUltimosTicketsOP(int idTicket, int idDocuSale)
        {
            var ultimostickets = db.UltimosTicketsProyecto(idTicket, idDocuSale).ToList();

            return Json(new { tickets = ultimostickets }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult ResumenSLA(int idTicket)
        {
            //var ListLeccionTickets = _srvLeccionAprendidaService.ListLeccionesAprendidasTickets(idTicket, 1);
            var resumenTiempos = db.ResumendeSLA(idTicket).ToList();
            return Json(new { tiempos = resumenTiempos }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult DetalleResumendeSLA(int idTicket, int idcategoriasla)
        {
            //var ListLeccionTickets = _srvLeccionAprendidaService.ListLeccionesAprendidasTickets(idTicket, 1);
            var resumenTiempos = db.DetalleResumendeSLA(idTicket, idcategoriasla).ToList();
            return Json(new { detalletiempos = resumenTiempos }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegistrarTiempoAtencion(int idTicket)
        {
            try
            {
                int userid = Convert.ToInt32(Session["UserId"]);

                //DETAILS_TICKET nuevodetalle = new DETAILS_TICKET();
                //nuevodetalle.ID_TICK = idTicket;
                //nuevodetalle.ID_TYPE_DETA_TICK = 1;
                //nuevodetalle.UserId = userid;
                //nuevodetalle.COM_DETA_TICK = "Registro de tiempo de atención";
                //nuevodetalle.CREATE_DETA_INCI = DateTime.Now;

                //db.DETAILS_TICKET.Add(nuevodetalle);
                //db.SaveChanges();

                db.RegistrarTiempoAtencion(idTicket, userid);

                return Json(new { status = "OK" });

            }
            catch (Exception ex)
            {
                // Manejo de errores
                return Json(new { status = "ERROR", message = ex.Message });
            }
        }

        [Authorize]
        public ActionResult EnviarCorreoResuelto(int id)
        {
            try
            {

                ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
                ViewBag.IdTicket = id;
                var DatosEnvio = db.DatosEnvioCorreoFinal(id).FirstOrDefault();
                ViewBag.Para = DatosEnvio.Para;
                ViewBag.Cc = DatosEnvio.Cc;
                ViewBag.Asunto = DatosEnvio.Asunto;

                return View();

            }
            catch
            {
                return Content("Please Close Session");
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> EnviarCorreoResueltoAsync(string correos, string cc, string asunto, string cuerpo, int IdTick, IEnumerable<HttpPostedFileBase> archivosSubidos)
        {
            try
            {
                // Configuración de seguridad
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                // Obtener el último detalle del ticket
                int IdDetaTicket = db.DETAILS_TICKET
                    .OrderByDescending(x => x.ID_DETA_TICK)
                    .Select(x => x.ID_DETA_TICK)
                    .FirstOrDefault();

                DETAILS_TICKET objDetalle = db.DETAILS_TICKET
                    .FirstOrDefault(x => x.ID_DETA_TICK == IdDetaTicket);

                if (objDetalle != null)
                {
                    objDetalle.COM_DETA_TICK += "</br></br>Se envió correctamente el correo con los siguientes datos:</br>● Para: " + correos + "</br>● En copia: " + cc + "</br>● Asunto: " + asunto + "</br>";
                    db.SaveChanges();
                }

                // Configuración de Brevo
                var datosBrevo = db.DatosBrevo().SingleOrDefault();
                if (datosBrevo == null)
                {
                    return "No se pudo obtener la configuración de Brevo";
                }

                string apiKey = datosBrevo.ApiKey;
                string fromEmail = datosBrevo.Remitente;

                // Configuración del cliente de correo
                var client = new RestClient("https://api.sendinblue.com/v3/smtp/email");
                var request = new RestRequest(Method.POST);
                request.AddHeader("accept", "application/json");
                request.AddHeader("api-key", apiKey);
                request.AddHeader("Content-Type", "application/json");

                // Preparar destinatarios
                var destinatarios = correos
                    .Split(',')
                    .Select(email => new { email = email.Trim() })
                    .ToList();

                var copias = string.IsNullOrEmpty(cc)
                    ? null
                    : cc.Split(',').Select(email => new { email = email.Trim() }).ToList();

                // Crear la estructura del correo
                var emailData = new Dictionary<string, object>
                {
                    { "sender", new { email = fromEmail, name = "IT Management System" } },
                    { "to", destinatarios },
                    { "cc", copias },
                    { "subject", asunto },
                    { "htmlContent", cuerpo }
                };

                // Procesar los adjuntos
                var attachments = new List<object>();
                foreach (var file in archivosSubidos)
                {
                    if (file.ContentLength > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            string fileBase64 = Convert.ToBase64String(ms.ToArray());

                            attachments.Add(new
                            {
                                name = file.FileName,
                                content = fileBase64
                            });
                        }
                    }
                }

                if (attachments.Any())
                {
                    emailData.Add("attachment", attachments);
                }

                // Agregar el cuerpo del correo al request
                request.AddJsonBody(emailData);

                // Enviar el correo
                var response = await ExecuteAsync(client, request);

                // Validar respuesta
                if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                    response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return "OK";
                }
                else
                {
                    Console.WriteLine("Error en la respuesta de Brevo: " + response.Content);
                    return "ERROR: " + response.Content;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error enviando correo: " + e.Message);
                return "ERROR";
            }
        }


        // Método auxiliar para manejar ExecuteAsync
        private Task<IRestResponse> ExecuteAsync(RestClient client, RestRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();

            client.ExecuteAsync(request, response =>
            {
                if (response.ErrorException != null)
                {
                    taskCompletionSource.SetException(response.ErrorException);
                }
                else
                {
                    taskCompletionSource.SetResult(response);
                }
            });

            return taskCompletionSource.Task;
        }

    }
}