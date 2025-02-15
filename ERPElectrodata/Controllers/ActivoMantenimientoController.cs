using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using ERPElectrodata.Models;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Net;
using System.Threading;
using System.Globalization;
using ERPElectrodata.Object.Ticket;

using System.Drawing;
using System.Reflection;
using System.Text;
using System.Data.OleDb;
using System.Linq.Expressions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Configuration;

namespace ERPElectrodata.Controllers
{
    public class ActivoMantenimientoController : Controller
    {
        //
        // GET: /ActivoMantenimiento/
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        static int estado;

        [Authorize]
        public ActionResult Crear(int id = 0)
        {
            ViewBag.ID_ASSE = id;
            estado = 0;
            return View();
        }

        public ActionResult HistorialMantenimientos(int id = 0)
        {
            ViewBag.ID_ASSE = id;
            return View();
        }

        public ActionResult vistaReporteActivoMant()
        {
            return View();
        }

        public ActionResult Alertas()
        {
            return View();
        }

        public ActionResult vistaReporteHistorialActivoMant()
        {
            return View();
        }

        public ActionResult CargarComponentes(int id = 0)
        {
            int IdActivo = Convert.ToInt32(Request.Params["IdActivo"]);

            var query = dbc.CargarComponentes(IdActivo).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Crear(IEnumerable<HttpPostedFileBase> files, ActivoMantenimiento activoMantenimiento)
        {
            //Valida si hay componentes para registrar su mantenimiento 
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            string idComponente = Convert.ToString(Request.Form["idComponente"]);
            string idsCondicion = Convert.ToString(Request.Form["idsCondicion"]);
            int idActivo = Convert.ToInt32(Request.Form["idActivo"]);
            var UserId = Convert.ToInt32(Session["UserId"]);
            DateTime FechaDesde = Convert.ToDateTime("1/1/0001 12:00:00 AM");
            DateTime FechaHasta = Convert.ToDateTime("1/1/0001 12:00:00 AM");
            int flag = 0;
            if (activoMantenimiento.FechaMantenimiento == null) { flag = 1; }
            if (activoMantenimiento.ProximoMantenimiento == null) { flag = 2; }
            if (Convert.ToString(Request.Form["FechaDesde"]) == "")
            {
                flag = 3;
            }
            else
            {
                FechaDesde = Convert.ToDateTime(Request.Form["FechaDesde"]);
            }
            if (Convert.ToString(Request.Form["FechaHasta"]) == "")
            {
                flag = 4;
            }
            else
            {
                FechaHasta = Convert.ToDateTime(Request.Form["FechaHasta"]);
            }
            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','',0,0);}window.onload=init;</script>");
            }
            TimeSpan DiferenciaTiempo = FechaHasta - FechaDesde;
            double SegundosTotales = DiferenciaTiempo.TotalSeconds;

            if (estado == 0)
            {
                estado = 1;
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeTiempoActividad) top.MensajeTiempoActividad(" + SegundosTotales + ");}window.onload=init;</script>");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (SegundosTotales == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','0','',0,0);}window.onload=init;</script>");
                    }

                    //Datos del Mantenimiento
                    activoMantenimiento.Estado = true;
                    activoMantenimiento.FechaCrea = DateTime.Now;
                    activoMantenimiento.UsuarioCrea = UserId;
                    activoMantenimiento.IdActivo = idActivo;
                    activoMantenimiento.IdProxMant = null;

                    dbc.ActivoMantenimientoes.Add(activoMantenimiento);
                    dbc.SaveChanges();

                    ACTIVITY_LOG activityLog = new ACTIVITY_LOG();
                    //ID_CLIE 
                    //INSERTAR DATOS A LA TABLA ACTIVITY_LOG
                    var IdClie = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == IdAcco && x.ID_PARA == 6).SingleOrDefault().VAL_ACCO_PARA;
                    var IdTypeAct = dbe.TYPE_ACT_LOG.Where(x => x.ID_ACCO == IdAcco && x.DES_ACT == "MANTENIMIENTO PREVENTIVO").SingleOrDefault().ID_TYPE_ACT;
                    var IdSubTypeAct = dbe.SUBTYPE_ACTITY.Where(x => x.ID_TYPE_ACT == IdTypeAct).SingleOrDefault().ID_SUBTYPE_ACT;
                    var SerieActivo = dbc.ASSETs.Where(x => x.ID_ASSE == idActivo).SingleOrDefault().SER_NUMB;
                    activityLog.ID_CLIE = Convert.ToInt32(IdClie);
                    activityLog.ID_TYPE_ACT = Convert.ToInt32(IdTypeAct);
                    activityLog.ID_SUBTYPE_ACT = Convert.ToInt32(IdSubTypeAct);
                    activityLog.COD_SUBTYPE_ACT = activoMantenimiento.IdActivoMant;
                    activityLog.NAM_SUBTYPE_ACT = SerieActivo;
                    activityLog.DATE_INIC = FechaDesde;
                    activityLog.DATE_END = FechaHasta;
                    activityLog.COMENTARIO = "MANTENIMIENTO PREVENTIVO DEL ACTIVO";
                    activityLog.USERID = UserId;
                    activityLog.CREATE_ACT_LOG = DateTime.Now;
                    activityLog.VIG_ACTI_LOG = true;
                    activityLog.TIEMPO_ACT = Convert.ToInt32(SegundosTotales);
                    activityLog.ID_ACCO = IdAcco;

                    dbe.ACTIVITY_LOG.Add(activityLog);
                    dbe.SaveChanges();
                    estado = 1;
                    //Buscar un registro anterior del activo para actualizar el Id del nuevo mantenimiento
                    var queryActualizar = dbc.ActMantActualizarProximoMant(idActivo, activoMantenimiento.IdActivoMant);

                    //Archivos adjuntos
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            try
                            {
                                //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                var ATTA = new ATTACHED();
                                ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                                ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                ATTA.IdActivoMant = activoMantenimiento.IdActivoMant;
                                ATTA.CREATE_ATTA = DateTime.Now;

                                //db.AddToATTACHEDs(ATTA);
                                dbc.ATTACHEDs.Add(ATTA);
                                dbc.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                            }
                            catch
                            {

                            }
                        }
                    }

                    //Guardar el registro de mantenimiento de componentes (en caso hubiera)
                    if (idComponente.Length > 0)
                    {
                        idComponente = idComponente.Substring(0, idComponente.Length - 1);
                        string[] idsComponente = idComponente.Split(',');
                        string msn = "";

                        foreach (string ida in idsComponente)
                        {
                            string[] idCondicion = idsCondicion.Split(',');
                            string comentario = "";
                            foreach (string cond in idCondicion)
                            {
                                string[] idCond = cond.Split('_');
                                if (idCond[0] == ida.ToString())
                                {
                                    comentario = Convert.ToString(idCond[1]);
                                }
                            }
                            var idsearch = Convert.ToInt32(ida);
                            int idActivoMantenimiento = activoMantenimiento.IdActivoMant;

                            ActivoMantenimientoDetalle activoMantDet = new ActivoMantenimientoDetalle();
                            activoMantDet.Estado = true;
                            activoMantDet.IdCompStockDet = Convert.ToInt32(ida);
                            activoMantDet.Comentario = comentario;
                            activoMantDet.IdActivoMant = activoMantenimiento.IdActivoMant;
                            activoMantDet.FechaCrea = DateTime.Now;
                            activoMantDet.UsuarioCrea = UserId;

                            dbc.ActivoMantenimientoDetalles.Add(activoMantDet);
                            dbc.SaveChanges();

                        }

                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('OK','0'," + activityLog.ID_ACTI_LOG + "," + SegundosTotales + ",0);}window.onload=init;</script>");
                    }
                    else
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                            "if(top.uploadDone) top.uploadDone('OK','0'," + activityLog.ID_ACTI_LOG + "," + SegundosTotales + ",0);}window.onload=init;</script>");
                    }
                }
            }

            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }

        public ActionResult CargarActivos()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.CargarActivos(IdAcco).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CargarTipoActivos()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.CargarTipoActivo(IdAcco).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CargarActivosAct( int TipoActivo)
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.CargarActivosAct(IdAcco, TipoActivo).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult reporteHistoricoActivoMantenimiento()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdActivo = Convert.ToInt32(Request.Params["IdActivo"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.VistaReporteHistorialActivoMant(IdActivo,0,IdAcco).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VistaReporteAlertaActivoMantenimiento()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int IdActivo = 0;
            if (Request.Params["IdActivo"] != null) {
                IdActivo = Convert.ToInt32(Request.Params["IdActivo"]);
            } 
            var query = dbc.VistaReporteAlertaActivoMantenimiento(IdActivo,ID_ACCO).ToList();

            var total = query.Count();

            return Json(new { Data = query, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarTiempoActividad()
        {
            int IdActLog = Convert.ToInt32(Request.Params["id"]);
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            string prueba = Request.Params["FechaDesde"];
            DateTime FechaDesde = Convert.ToDateTime("1/1/0001 12:00:00 AM");
            DateTime FechaHasta = Convert.ToDateTime("1/1/0001 12:00:00 AM");
            int flag = 0;
            ACTIVITY_LOG activityLog = dbe.ACTIVITY_LOG.Where(x => x.ID_ACTI_LOG == IdActLog).SingleOrDefault();
            if (Convert.ToString(Request.Form["FechaDesde"]) == "")
            {
                flag = 1;
            }
            else
            {
                FechaDesde = Convert.ToDateTime(Request.Params["FechaDesde"]);
            }
            if (Convert.ToString(Request.Params["FechaHasta"]) == "")
            {
                flag = 2;
            }
            else
            {
                FechaHasta = Convert.ToDateTime(Request.Params["FechaHasta"]);
            }
            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','',0,0);}window.onload=init;</script>");
            }
            TimeSpan DiferenciaTiempo = FechaHasta - FechaDesde;
            double SegundosTotales = DiferenciaTiempo.TotalSeconds;

            if (SegundosTotales == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','',0,0);}window.onload=init;</script>");
            }
            try
            {
                activityLog.TIEMPO_ACT = Convert.ToInt32(SegundosTotales);
                dbe.ACTIVITY_LOG.Attach(activityLog);
                dbc.Entry(activityLog).State = EntityState.Modified;
                dbc.SaveChanges();
            }
            catch
            {
                return Content("Error");
            }

            return Content("Ok");
        }

        public ActionResult ValidarFechas()
        {
            int flag = 0;
            DateTime FechaMantenimiento = Convert.ToDateTime("1/1/0001 12:00:00 AM");
            DateTime ProximoMantenimiento = Convert.ToDateTime("1/1/0001 12:00:00 AM");
            DateTime FechaDesde = Convert.ToDateTime("1/1/0001 12:00:00 AM");
            DateTime FechaHasta = Convert.ToDateTime("1/1/0001 12:00:00 AM");
            if (Convert.ToString(Request.Params["FechaMantenimiento"]) == "")
            {
                flag = 1;
            }
            else
            {
                FechaMantenimiento = Convert.ToDateTime(Request.Params["FechaMantenimiento"]);
            }
            if (Convert.ToString(Request.Params["ProximoMantenimiento"]) == "")
            {
                flag = 2;
            }
            else
            {
                ProximoMantenimiento = Convert.ToDateTime(Request.Params["ProximoMantenimiento"]);
            }
            if (Convert.ToString(Request.Params["FechaDesde"]) == "")
            {
                flag = 3;
            }
            else
            {
                FechaDesde = Convert.ToDateTime(Request.Params["FechaDesde"]);
            }
            if (Convert.ToString(Request.Params["FechaHasta"]) == "")
            {
                flag = 4;
            }
            else
            {
                FechaHasta = Convert.ToDateTime(Request.Params["FechaHasta"]);
            }
            if (flag != 0)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
            else
            {
                TimeSpan DiferenciaTiempo = FechaHasta - FechaDesde;
                double SegundosTotales = DiferenciaTiempo.TotalSeconds;

                return Json(SegundosTotales, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdjuntarArchivosMantenimiento(IEnumerable<HttpPostedFileBase> files)
        {
            var xx = "";
            try
            {
                //adjuntarArchivos = files;
                var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);
                var ID_DOCU_SALE = Convert.ToInt32(Request.Params["ID_DOCU_SALE"]);
                int ID_TYPE_DOCU_ATTA = 0;

                int UserId = Convert.ToInt32(Session["UserId"]);

                var FechaActa = "";
                DateTime DateFechaActa = DateTime.Now;

                if (!String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                {
                    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);

                    if (ID_TYPE_DOCU_ATTA == 1)
                    {
                        FechaActa = Convert.ToString(Request.Params["FechaActa"]);
                        DateFechaActa = Convert.ToDateTime(FechaActa);
                    }
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            string name_atta = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);

                            int repetido = dbc.ATTACHEDs
                                .Where(x => x.DELETE_ATTA == false)
                                .Where(x => x.KEY_ATTA == KEY_ATTA)
                                .Where(x => x.NAM_ATTA == name_atta)
                                .Where(x => x.EXT_ATTA == extension).Count();

                            if (repetido == 0)
                            {
                                var ATTA = new ATTACHED();
                                ATTA.NAM_ATTA = name_atta;
                                ATTA.EXT_ATTA = extension;
                                //ATTA.ID_INCI = ticket.ID_TICK;
                                ATTA.ID_DOCU_SALE = ID_DOCU_SALE;
                                ATTA.CREATE_ATTA = DateTime.Now;
                                ATTA.KEY_ATTA = KEY_ATTA;
                                ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                                ATTA.DELETE_ATTA = false;
                                ATTA.FechaActaConformidad = DateFechaActa;
                                ATTA.UserId = UserId;
                                ATTA.Indicador = 1;
                                dbc.ATTACHEDs.Add(ATTA);
                                dbc.SaveChanges();

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
                    }
                }

                return Content(ID_TYPE_DOCU_ATTAs);
                //return Content(xx);
            }
            catch
            {
                // Convert.ToString(codeID);
                return Content(xx);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverArchivosMantenimiento(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);
                var ID_DOCU_SALE = Convert.ToInt32(Request.Params["ID_DOCU_SALE"]);

                string filenames = Convert.ToString(Request.Params["fileNames"]);

                if (filenames != null)
                {
                    try
                    {
                        string name_atta = Path.GetFileNameWithoutExtension(filenames);

                        var ATTA = dbc.ATTACHEDs
                            .Where(x => x.KEY_ATTA == KEY_ATTA)
                            .Where(x => x.ID_DOCU_SALE == ID_DOCU_SALE)
                            .Where(x => x.ID_DETA_INCI == null)
                            .Where(x => x.DELETE_ATTA == false)
                            .Single(x => x.NAM_ATTA == name_atta);

                        ATTA.DELETE_ATTA = true;

                        try
                        {
                            if (System.IO.File.Exists(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA)))
                            {
                                System.IO.File.Delete(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA));
                            }
                        }
                        catch
                        {

                        }

                        dbc.Entry(ATTA).State = EntityState.Modified;
                        dbc.SaveChanges();
                    }
                    catch
                    {

                    }
                }

                return Content("");
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }
        }

        public ActionResult HistorialMantenimientosListar(int id = 0)
        {
            var query = dbc.ActMantHistorialMantenimientosListar(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
