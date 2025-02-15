using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Web.Helpers;
using ERPElectrodata.AppCode;
using ERPElectrodata.Helpers;
using System.Data.Entity;
using System.Text;

using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using ClosedXML.Excel;

namespace ERPElectrodata.Controllers
{
    public class ChangeRequestController : Controller
    {
        public CoreEntities db = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        CHANGE_REQUEST model = new CHANGE_REQUEST();
        static bool ListarTodos = false;
        static int idSession = 0;
        static int approved = 0;
        #region vistas
        public ActionResult Index()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            ViewBag.IdCuenta = IdCuenta;
            return View();
        }

        public ActionResult Details(int id)
        {
            return View();
        }


        

        public ActionResult Create()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string para =Convert.ToString(db.GestionCambioITMinsur(ID_PERS_ENTI).FirstOrDefault());
            if (IdCuenta == 4)
            {
                ViewBag.IdCuenta = IdCuenta;
                return View();
            }
            else if (IdCuenta == 60)
            {
                return RedirectToAction("CreateBNV", "ChangeRequest");
            }
            else if (IdCuenta == 56 && para=="1")
            {
                var idClasi = (from ct in db.CHANGE_TYPE
                               where ct.IdCuenta == 56 && ct.status == true && ct.nombre == "Menor"
                               select ct.id).FirstOrDefault();
                int ClasificacionCambio = Convert.ToInt32(idClasi);
                ViewBag.Prioridad= Convert.ToInt32(ClasificacionCambio);

                return RedirectToAction("CreateMINSUR", "ChangeRequest");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        #region (ERP)
        public ActionResult CreateBNV()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string para = Convert.ToString(db.GestionCambioITMinsur(ID_PERS_ENTI).FirstOrDefault());
            if (IdCuenta == 60)
            {
                ViewBag.IdCuentaBNV = IdCuenta;
                return View();
            }
            else if (IdCuenta == 4)
            {
                return RedirectToAction("Create", "ChangeRequest");
            }
            else if (IdCuenta == 56 && para == "1")
            {
                var idClasi = (from ct in db.CHANGE_TYPE
                               where ct.IdCuenta == 56 && ct.status == true && ct.nombre == "Menor"
                               select ct.id).FirstOrDefault();
                int ClasificacionCambio = Convert.ToInt32(idClasi);
                ViewBag.Prioridad = Convert.ToInt32(ClasificacionCambio);
                return RedirectToAction("CreateMINSUR", "ChangeRequest");
            }
            else
            {
                return RedirectToAction("Index", "Error");

            }

            // int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);


        }

        public ActionResult CreateMINSUR()
        {


            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string para = Convert.ToString(db.GestionCambioITMinsur(ID_PERS_ENTI).FirstOrDefault());
            var idClasi = (from ct in db.CHANGE_TYPE
                           where ct.IdCuenta == 56 && ct.status == true && ct.nombre == "Menor"
                           select ct.id).FirstOrDefault();
            int ClasificacionCambio = Convert.ToInt32(idClasi);

            if (IdCuenta == 56 && para == "1")
            {
                ViewBag.Prioridad = Convert.ToInt32(ClasificacionCambio);

                ViewBag.IdCuentaMS = IdCuenta;
                return View();
            }
            else if (IdCuenta == 60)
            {
                return RedirectToAction("CreateBNV", "ChangeRequest");
            }
            else if (IdCuenta == 4)
            {
                return RedirectToAction("Create", "ChangeRequest");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
            
        }

        public ActionResult ResumenGestionCambioBNV(int id)
        {
            var query = db.CHANGE_APPROVED.Single(x => x.idChangeRequest == id);

            var ListarTodos = "0";
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1)
            {
                ListarTodos = "1";
            }

            ViewBag.IdApproved = query.idTypeRequest;
            ViewBag.IdSession = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            ViewBag.SwApproval = ListarTodos;
            ViewBag.Approved = query.approver;
            ViewBag.IdGestionCambio = id;
            return View();
        }

        public ActionResult viewDetailChangeBNV()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int MostrarOpciones = 0, UsuarioResponsable = 0;
            try
            {
                var IdAprobacion = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == id).SingleOrDefault();

                if (IdAprobacion.idTypeRequest == 2 || IdAprobacion.idTypeRequest == 7)
                {
                    MostrarOpciones = 1;
                }

                if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1)
                {
                    UsuarioResponsable = 1;
                }
            }
            catch
            {
                MostrarOpciones = 0;
            }

            ViewBag.ID_CHANGE_REQUEST = id;
            ViewBag.IdPersEnti = IdPersEnti;
            ViewBag.MostrarOpciones = MostrarOpciones;
            ViewBag.Admin = UsuarioResponsable;

            return View();
        }

        public ActionResult viewDetailChangeMinsur()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int MostrarOpciones = 0, UsuarioResponsable = 0;
            try
            {
                var IdAprobacion = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == id).SingleOrDefault();

                if (IdAprobacion.idTypeRequest == 2 || IdAprobacion.idTypeRequest == 7)
                {
                    MostrarOpciones = 1;
                }

                if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1)
                {
                    UsuarioResponsable = 1;
                }
            }
            catch
            {
                MostrarOpciones = 0;
            }

            ViewBag.ID_CHANGE_REQUEST = id;
            ViewBag.IdPersEnti = IdPersEnti;
            ViewBag.MostrarOpciones = MostrarOpciones;
            ViewBag.Admin = UsuarioResponsable;

            return View();
        }

        public ActionResult GuardarComentario()
        {
            int IdDetalleCambio = 0;
            IdDetalleCambio = Convert.ToInt32(Request.Params["IdDetalleCambio"]);
            int IdCambio = Convert.ToInt32(Request.Params["IdCambio"]);
            string coment = Request.Params["coment"];
            int UserId = Convert.ToInt32(Session["UserId"]);
            db.sp_Insert_Coment(IdCambio, coment, UserId);
            return null;
        }

        public ActionResult GestionCambiosDetalleBNV(int id)
        {
            var query = db.GestionCambiosDetalleBNV(id);
            var cr = query.ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GestionCommentBNV(int id)
        {
            var query = db.sp_List_Coment(id).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GestionAttaBNV(int id)
        {
            var query = db.sp_List_Atta_Change(id).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }



        [Authorize]
        public ActionResult RegistrarActividad()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdDetalleCambio = Convert.ToInt32(Request.Params["IdDetalleCambio"]);
            int IdCambio = Convert.ToInt32(Request.Params["IdCambio"]);



            ObjectParameter OutputParam = new ObjectParameter("idReturn", typeof(int));
            db.GestionCambioObtenerOrderBNV(IdCambio, IdDetalleCambio, OutputParam);
            IdDetalleCambio = Convert.ToInt32(OutputParam.Value);
            string mensaje = "Actividad realizada incorrectamente";

            //Registrar Actividad Realizada
            CHANGE_DETAIL CD = db.CHANGE_DETAIL.Where(x => x.ID == IdDetalleCambio).FirstOrDefault();
            CD.ActividadRealizada = false;
            CD.FechaActividadRealizada = DateTime.Now;
            db.Entry(CD).State = EntityState.Modified;
            db.SaveChanges();

            db.GestionCambioRegistrarActividadTicket(IdCambio);

            int cambioDetalle = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio).Count();
            CHANGE_APPROVED catoPlan = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == @IdCambio).FirstOrDefault();

            //Envio de correo a los involucrados de las actividades
            ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
            var listado = db.GestionCambioListarActividades(IdCambio, IdCuenta).Where(x => x.Estado == 0);
            foreach (var query in listado)
            {
                var detail = db.CHANGE_DETAIL.Where(x => x.ID == query.IdDetalleCambio && (x.EnvioCorreo == true || x.EnvioCorreo != null)).SingleOrDefault();
                if (detail != null)
                {
                    detail.EnvioCorreo = false;
                    db.Entry(detail).State = EntityState.Modified;
                    db.SaveChanges();
                    mail.sendMailUsersRequestBNV(detail, catoPlan, 3);
                }
            }

            if (cambioDetalle == 1)
            {
                //Enviar correo a Usuario para informar que el plan se ejecuto
                //int idRequester = Convert.ToInt32(db.CHANGE_REQUEST.Single(x => x.id == IdCambio).ID_PERS_ENTI_ASSI);
                var idRequester = (from cr in db.CHANGE_REQUEST.Where(x => x.id == IdCambio)
                                   join t in db.TICKETs on cr.IdTicket equals t.ID_TICK
                                   select new
                                   {
                                       t.ID_PERS_ENTI
                                   }).FirstOrDefault();
                ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                smail.sendMailGestionCambiosBNV(Convert.ToInt32(idRequester.ID_PERS_ENTI), catoPlan, "E");
                mensaje = "La solicitud del cambio se ha ejecutado";
            }
            else
            {
                int ultimaActividad = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio
                                        && (x.ActividadRealizada == null || x.ActividadRealizada == true) && x.STATUS == false).Count();
                if (ultimaActividad == 0)
                {
                    //Enviar correo a Usuario para informar que el plan se ejecuto
                    int idRequester = Convert.ToInt32(db.CHANGE_REQUEST.Single(x => x.id == IdCambio).ID_PERS_ENTI);
                    ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                    smail.sendMailGestionCambios(idRequester, catoPlan, "E");
                    mensaje = "La solicitud del cambio se ha ejecutado";
                }
            }

            return Content(mensaje);
        }

        // POST: /ChangeRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateBNV(FormCollection collection)
        {
            //Variables
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            int ClasificacionCambio = Convert.ToInt32(Request.Params["cbClasificacionCambio"]);
            int TipoCambio = Convert.ToInt32(Request.Params["cbTipoCambio"]);
            int Prioridad = Convert.ToInt32(Request.Params["cbPrioridad"]);
            int cbActividad = 9;


            int IdPersEntiAssi = Convert.ToInt32(Request.Params["cbUsuarioSolicitante"]);
            // int IdGrupoCambio = Convert.ToInt32(Request.Params["cbGrupo"]);
            // int IdSedeCambio = Convert.ToInt32(Request.Params["cbSede"]);


            int contador = Convert.ToInt32(Request.Params["Contador"]);
            string ImpactoDescripcion = Convert.ToString(Request.Params["txtImpactoDescripcion"]);

            if (ClasificacionCambio == 0 || Prioridad == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
            }

            var tipoDescripcion = db.CHANGE_TYPE.Where(x => x.id == ClasificacionCambio && x.status == true).SingleOrDefault();
            string txtTipoDescripcion = "";
            if (tipoDescripcion.descripcion == true)
            {
                txtTipoDescripcion = Convert.ToString(Request.Params["txtTipoDescripcion"]);
                if (txtTipoDescripcion == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
                var TipoCambioUrgente = db.CHANGE_TYPE_DETAIL.Where(x => x.idChangeType == ClasificacionCambio && x.status == true).FirstOrDefault();
                TipoCambio = Convert.ToInt32(TipoCambioUrgente.id);
            }
            else
            {
                if (TipoCambio == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
            }

            //Preguntas
            string txtPregunta1 = Convert.ToString(Request.Params["txtPregunta1"]);
            string txtPregunta2 = Convert.ToString(Request.Params["txtPregunta2"]);
            string txtPregunta3 = Convert.ToString(Request.Params["txtPregunta3"]);
            string txtPregunta4 = Convert.ToString(Request.Params["txtPregunta4"]);

            if (txtPregunta2 == "" || txtPregunta1 == "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Por favor responda todas las preguntas','2');}window.onload=init;</script>");
            }

            //Impacto
            IQueryable<CHANGE_IMPACT> impact = from ci in db.CHANGE_IMPACT
                                               where ci.IdCuenta == IdCuenta && ci.status == true
                                               select ci;

            CHANGE_IMPACT[] impactArray = impact.ToArray();


            string chkImpact = "";
            foreach (CHANGE_IMPACT imp in impactArray)
            {
                if (Convert.ToString(collection[imp.detail]) != null)
                {
                    chkImpact = chkImpact + imp.id + ",";
                }

            }


            int code;
            try
            {
                //Registro de Aprobación de Usuario
                //var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).ToList();
                //var JefeInmediato = IdPersEnti;
                // var ID_PERS_ENTI_APPR = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);

                var changeRequest = new CHANGE_REQUEST();
                changeRequest.idChangeType = ClasificacionCambio;
                changeRequest.idChangeTypeDetail = TipoCambio;
                changeRequest.isImpactCheck = chkImpact;
                changeRequest.otherImpact = ImpactoDescripcion;
                changeRequest.idPriority = Prioridad;
                changeRequest.urgentDetail = txtTipoDescripcion;
                changeRequest.otherChange = "";
                changeRequest.question1 = txtPregunta1;
                changeRequest.question2 = txtPregunta2;
                changeRequest.question3 = txtPregunta3;
                changeRequest.question4 = txtPregunta4;
                changeRequest.ID_PERS_ENTI = IdPersEntiAssi; //changeRequest.ID_PERS_ENTI = IdPersEnti; 
                changeRequest.createDate = DateTime.Now;
                changeRequest.IdCuenta = IdCuenta;
                changeRequest.ID_PERS_ENTI_Modified = IdPersEnti;
                changeRequest.ID_PERS_ENTI_ASSI = IdPersEntiAssi;
                //changeRequest.idGroup = IdGrupoCambio;
                //  changeRequest.idAttentioSite = IdSedeCambio;

                db.CHANGE_REQUEST.Add(changeRequest);
                db.SaveChanges();

                 code = Convert.ToInt32(db.GestionCambioObtenerID().FirstOrDefault());

                var approved = new CHANGE_APPROVED();
                approved.idChangeRequest = changeRequest.id;
                approved.idTypeRequest = 2;
                //approved.approver = ID_PERS_ENTI_APPR;
                approved.approver = 79581;
                approved.createDate = DateTime.Now;
                approved.modifiedDate = DateTime.Now;
                approved.activeJustified = false;
                approved.activeToplan = 0; //2
                db.CHANGE_APPROVED.Add(approved);
                db.SaveChanges();
                #region Detalle de Solicitud

                var cambioDetalle = new CHANGE_DETAIL();

                int IdActividad = 0, IdResponsable = 0;
                string DescripcionActividad = "";

                cbActividad = 9;
                for (int i = 0; i <= contador; i++)
                {
                    
                        IdActividad = cbActividad;
                        //IdActividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                        if (i == 0 || i == 2 || i == 3)
                        {
                            IdResponsable = 79581;
                        }
                        else if (i == 1 || i == 4)
                        {
                            IdResponsable = 79579;
                        }
                        else
                        {
                            IdResponsable = 77094;
                        }
                        //IdResponsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                        //DescripcionActividad = Convert.ToString(Request.Params["Descripcion" + i]);

                        if (i == 0) { DescripcionActividad = "Adjuntar RFC y Aprobaciones"; }
                        else if (i == 1) { DescripcionActividad = "Revisar Información"; }  
                        else if (i == 2) { DescripcionActividad = "Atención/ Ejecución de Requerimientos/Solicitar Conformidad"; } 
                        else if (i == 3) { DescripcionActividad = "Adjuntar Conformidad"; } 
                        else if (i == 4) { DescripcionActividad = "Revisar Informe de Atención"; }  
                        else if (i == 5) { DescripcionActividad = "Cerrar Ticket"; }

                        cambioDetalle.ID_TYPE_TASK = IdActividad;
                        cambioDetalle.ID_CHANGE_REQUEST = changeRequest.id;
                        cambioDetalle.ID_PERSON_ENTI = IdResponsable;
                        cambioDetalle.DETAIL = DescripcionActividad;
                        cambioDetalle.DATE_INIC = DateTime.Now;
                        cambioDetalle.DATE_END = DateTime.Now;
                        cambioDetalle.STATUS = true;
                        cambioDetalle.FechaInicioProgramada = DateTime.Now;
                        cambioDetalle.FechaFinProgramada = DateTime.Now;
                        //ADD LAST 05/05/2023
                        cambioDetalle.ActividadValidada = true;
                        cambioDetalle.FechaActividadValidada = DateTime.UtcNow;

                        db.CHANGE_DETAIL.Add(cambioDetalle);
                        db.SaveChanges();
                        cbActividad = cbActividad + 1;
                    
                }

                #endregion


                ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();

                    //(ERP)
                    mail.sendMailGestionCambiosBNV(IdPersEntiAssi, approved, "N");
                    

            }
            catch (Exception e)
            {
                string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MensajeConfirmacion) top.MensajeConfirmacion('No se registró la solicitud del cambio, error al enviar el correo. Contacte al administrador.','2');}window.onload=init;</script>");
        
            }

            return Content("<script type='text/javascript'> function init() {" +
                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('"+ Convert.ToString(code) + "','1');}window.onload=init;</script>");
            //int id = 1;
            //return Content("<script type='text/javascript'> function init() {" +
            //               "if(top.uploadDone) top.uploadDoneBnv('OK','" + code + "-" + id + "');}window.onload=init;</script>");
        }


        // POST: /ChangeRequest/Create/MINSUR
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateMINSUR(FormCollection collection)
        {
            //Variables
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            //int ClasificacionCambio = 37;
            var idClasi  = (from ct in db.CHANGE_TYPE
                                  where ct.IdCuenta == 56 && ct.status == true && ct.nombre == "Menor"
                                  select ct.id).FirstOrDefault();
            int ClasificacionCambio = Convert.ToInt32(idClasi);
            int TipoCambio = Convert.ToInt32(Request.Params["cbTipoCambio"]);
            var idPrio = (from cp in db.CHANGE_PRIORITY
                      where cp.IdCuenta == 56 && cp.status == true && cp.nombre == "Baja"
                      select cp.id).FirstOrDefault();
            int Prioridad = Convert.ToInt32(idPrio);

            //int Prioridad = 52;
            int changeRequestID = 0;

            int contador = Convert.ToInt32(Request.Params["Contador"]);
            string ImpactoDescripcion = Convert.ToString(Request.Params["txtImpactoDescripcion"]);

            if (ClasificacionCambio == 0 || Prioridad == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
            }

            var tipoDescripcion = db.CHANGE_TYPE.Where(x => x.id == ClasificacionCambio && x.status == true).SingleOrDefault();
            string txtTipoDescripcion = "";
            if (tipoDescripcion.descripcion == true)
            {
                txtTipoDescripcion = Convert.ToString(Request.Params["txtTipoDescripcion"]);
                if (txtTipoDescripcion == "")
                {
                    //return Content("<script type='text/javascript'> function init() {" +
                    //    "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
                var TipoCambioUrgente = db.CHANGE_TYPE_DETAIL.Where(x => x.idChangeType == ClasificacionCambio && x.status == true).FirstOrDefault();
                TipoCambio = Convert.ToInt32(TipoCambioUrgente.id);
            }
            else
            {
                if (TipoCambio == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
            }

            //Preguntas
            string txtPregunta1 = Convert.ToString(Request.Params["txtPregunta1"]);
            string txtPregunta2 = Convert.ToString(Request.Params["txtPregunta2"]);
            string txtPregunta3 = Convert.ToString(Request.Params["txtPregunta3"]);
            string txtPregunta4 = Convert.ToString(Request.Params["txtPregunta4"]);

            //if (txtPregunta1 == "" || txtPregunta2 == "" || txtPregunta3 == "" || txtPregunta4 == "")
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //             "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Por favor responda todas las preguntas','2');}window.onload=init;</script>");
            //}

            //Impacto
            IQueryable<CHANGE_IMPACT> impact = from ci in db.CHANGE_IMPACT
                                               where ci.IdCuenta == IdCuenta && ci.status == true
                                               select ci;

            CHANGE_IMPACT[] impactArray = impact.ToArray();


            string chkImpact = "";
            foreach (CHANGE_IMPACT imp in impactArray)
            {
                if (Convert.ToString(collection[imp.detail]) != null)
                {
                    chkImpact = chkImpact + imp.id + ",";
                }

            }

            try
            {
                //Registro de Aprobación de Usuario
                
                int ID_PERS_ENTI_APPR = Convert.ToInt32(Request.Params["cbUsuarioAprobador"]);

                var changeRequest = new CHANGE_REQUEST();
                changeRequest.idChangeType = ClasificacionCambio;
                changeRequest.idChangeTypeDetail = TipoCambio;
                changeRequest.isImpactCheck = chkImpact;
                changeRequest.otherImpact = ImpactoDescripcion;
                changeRequest.idPriority = Prioridad;
                changeRequest.urgentDetail = txtTipoDescripcion;
                changeRequest.otherChange = "";
                changeRequest.question1 = txtPregunta1;
                changeRequest.question2 = txtPregunta2;
                changeRequest.question3 = txtPregunta3;
                changeRequest.question4 = txtPregunta4;
                changeRequest.ID_PERS_ENTI = IdPersEnti;
                changeRequest.createDate = DateTime.Now;
                changeRequest.IdCuenta = IdCuenta;
                db.CHANGE_REQUEST.Add(changeRequest);
                db.SaveChanges();

                changeRequestID = Convert.ToInt32(changeRequest.id);

                var approved = new CHANGE_APPROVED();
                approved.idChangeRequest = changeRequest.id;
                approved.idTypeRequest = 8;
                approved.approver = ID_PERS_ENTI_APPR;
                approved.createDate = DateTime.Now;
                approved.activeJustified = false;
                approved.activeToplan = 0;
                db.CHANGE_APPROVED.Add(approved);
                db.SaveChanges();

                //var approved = new CHANGE_APPROVED();
                //approved.idChangeRequest = changeRequest.id;
                //approved.idTypeRequest = 8;
                //approved.approver = ID_PERS_ENTI_APPR;
                //approved.createDate = DateTime.Now;
                //approved.activeJustified = true;
                //approved.activeToplan = 2;
                //db.CHANGE_APPROVED.Add(approved);
                //db.SaveChanges();
                #region Detalle de Solicitud

                var cambioDetalle = new CHANGE_DETAIL();

                int IdActividad = 0, IdResponsable = 0;
                string DescripcionActividad = "";


                        IdActividad = 99;
                        IdResponsable = Convert.ToInt32(Request.Params["cbUsuarioEjecutor"]);
                        DescripcionActividad = "";
                        cambioDetalle.ActividadValidada = true;
                        cambioDetalle.FechaActividadValidada = DateTime.Now;
                        cambioDetalle.ID_TYPE_TASK = IdActividad;
                        cambioDetalle.ID_CHANGE_REQUEST = changeRequest.id;
                        cambioDetalle.ID_PERSON_ENTI = IdResponsable;
                        cambioDetalle.DETAIL = DescripcionActividad;
                        cambioDetalle.DATE_INIC = DateTime.Now;
                        cambioDetalle.DATE_END = DateTime.Now;
                        cambioDetalle.STATUS = true;
                        cambioDetalle.FechaInicioProgramada = DateTime.Now;
                    cambioDetalle.FechaFinProgramada = DateTime.Now;
                db.CHANGE_DETAIL.Add(cambioDetalle);
                        db.SaveChanges();

                #endregion


                ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                mail.sendMailGestionCambios(IdPersEnti, approved, "SM");
                //Envio de correo a los involucrados de las actividades
               // var query = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == changeRequest.id);
                //foreach (var detail in query)
                //{
                //    mail.sendMailUsersRequest(detail, approved, 1);
                //}
            }
            catch (Exception e)
            {
                string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MensajeConfirmacion) top.MensajeConfirmacion('No se registró la solicitud del cambio, error al enviar el correo. Contacte al administrador.','2');}window.onload=init;</script>");
            }

            //return Content("<script type='text/javascript'> function init() {" +
            //            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('El Comité del SIG y/o tu Gerencia respectiva se comunicarán con Ud. para desarrollar el plan de acción sobre el cambio.','1');}window.onload=init;</script>");
            return Content("<script type='text/javascript'> function init() {" +
                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('" + Convert.ToString(changeRequestID) + "','1');}window.onload=init;</script>");

        }


        public ActionResult ResumenGestionCambioMinsur(int id)
        {
            var query = db.CHANGE_APPROVED.Single(x => x.idChangeRequest == id);

            var ListarTodos = "0";
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1)
            {
                ListarTodos = "1";
            }

            ViewBag.IdApproved = query.idTypeRequest;
            ViewBag.IdSession = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            ViewBag.SwApproval = ListarTodos;
            ViewBag.Approved = query.approver;
            ViewBag.IdGestionCambio = id;
            return View();
        }



        public ActionResult ListarCambioUserAsignado(string q, string page, int id)
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

            var result = db.GestionCambioListarResponsable(id, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCambioEjecutorMinsur(string q, string page, int id)
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

            var result = db.GestionCambioListarEjecutorMinsur(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCambioAprobadorMinsur(string q, string page, int id)
        {
            string termino = "";
            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = db.GestionCambioAprobadoresMinsur(id).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarGrupoCambio(string q, string page, int id)
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

            var result = db.GestionCambioListarGrupo(id, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarGrupoCambioSede(string q, string page, int id)
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

            var result = db.GestionCambioListarGrupoSede(termino, id).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListBNV(int idChangeType, int idTipoCambio, int idApproved, int idRequested, string keyword)
        {
            if (keyword.Equals(""))
            {
                keyword = "0";
            }
            if (idApproved == 1)
            {
                idApproved = 2;
            }
            int code = Convert.ToInt32(keyword);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            using (var context = new CoreEntities())
            {
                int ID_PERS_ENTI = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
                bool listarAprobadosxJefes = false;
                ListarTodos = false;
                #region datos Usuario y Jefe

                //(ERP)
                var idEntiCuenta = (from d in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                    select d.ID_ENTI1)
                                    ;


                var qUserEdata = context.GestionCambioObtenerUsuarios().ToList();


                int idJefe = (from a in context.CHANGE_APPROVED
                              join b in context.CHANGE_REQUEST on a.idChangeRequest equals b.id
                              where b.ID_PERS_ENTI == ID_PERS_ENTI
                              select a.approver).FirstOrDefault();

                var qJefeEdata = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == idEntiCuenta.FirstOrDefault())//idEntiCuenta) //Listado de Personas
                                  join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                                  select new
                                  {
                                      ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      Jefe = b.FIR_NAME + " " + b.LAS_NAME,
                                      UsuarioJefe = b.FIR_NAME.Substring(0, 1) + ". " + b.LAS_NAME
                                  }).ToList();
                #endregion

                #region Permisos de Visualizacion de solicitudes
                int esJefe = (from a in context.CHANGE_APPROVED.Where(x => x.approver == ID_PERS_ENTI)
                              select a.approver).FirstOrDefault();
                if (esJefe > 0)
                {
                    listarAprobadosxJefes = true;
                }

                var queu_ID_MDS = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                   select a.ID_QUEU).FirstOrDefault();
                if ((int)Session["ADMINISTRADOR"] == 1 || queu_ID_MDS == 100)
                {
                    ListarTodos = true;
                }
                #endregion

                #region JefesAprobadores
                if (listarAprobadosxJefes && !ListarTodos) ///this
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO)
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  //join cd in context.CHANGE_DETAIL on cr.id equals cd.ID_CHANGE_REQUEST
                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      //estado de solicitud
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      //colorApproved = (ca.idTypeRequest == 1 ? "#2b5797" : ca.idTypeRequest == 2 ? "#7cbb00" : ca.idTypeRequest == 3 ? "#b21e1e" : ca.idTypeRequest == 4 ? "#ffbb00" : ca.idTypeRequest == 5 ? "#990099" : ca.idTypeRequest == 6 ? "#0099C6" : ca.idTypeRequest == 7 ? "#7cb5ec" : ""),
                                      //LetraApproved = (ca.idTypeRequest == 1 ? "S" : ca.idTypeRequest == 2 ? "A" : ca.idTypeRequest == 3 ? "R" : ca.idTypeRequest == 4 ? "P" : ca.idTypeRequest == 5 ? "E" : ca.idTypeRequest == 6 ? "NE" : ca.idTypeRequest == 7 ? "NE" : ""),
                                      TypeApproved = ctr.type,
                                      detailChange = cr.question2 == null ? "" : cr.question2,
                                      idchangeApproved = ca.id,
                                      //detalle
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      detpriority = pr.detail,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden,
                                      IdResponsable = 79581,//cd.ID_PERSON_ENTI,
                                      ID_PERS_ENTI_SOL = cr.ID_PERS_ENTI_ASSI
                                  }).Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.approved == ID_PERS_ENTI || x.IdResponsable == ID_PERS_ENTI).Distinct();

                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI_SOL == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (code > 0)
                    {
                        result = result.Where(x => x.idChangeRequest == code);
                    }

                    int tt = result.Count();
                    int fil = tt - skip;

                    var queryFinal = (from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                      join b in qUserEdata on a.ID_PERS_ENTI_SOL equals b.ID_PERS_ENTI
                                      join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      select new
                                      {
                                          idSession = ID_PERS_ENTI,
                                          idChangeRequest = a.idChangeRequest,
                                          idChangeType = a.idChangeType,
                                          ChangeType = a.ChangeType,
                                          ChangeTypeDetail = a.ChangeTypeDetail,
                                          ID_PERS_ENTI = a.ID_PERS_ENTI,
                                          approved = a.approved,
                                          idApproved = a.idApproved,
                                          priority = a.priority,
                                          registro = a.registro,
                                          colorApproved = a.colorApproved,
                                          LetraApproved = a.LetraApproved,
                                          TypeApproved = a.TypeApproved,
                                          User = b.User,
                                          Jefe = c.Jefe,
                                          detailChange = a.detailChange == null ? "" : a.detailChange,
                                          DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                          MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                          DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                          TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                          SwApproval = ListarTodos,
                                          idchangeApproved = a.idchangeApproved,
                                          question1 = a.question1,
                                          question3 = a.question3,
                                          question4 = a.question4,
                                          question5 = a.question5,
                                          question6 = a.question6,
                                          question7 = a.question7,
                                          otherChange = a.otherChange,
                                          urgentDetail = a.urgentDetail,
                                          otherImpact = a.otherImpact,
                                          isImpactCheck = a.isImpactCheck,
                                          detpriority = a.detpriority,
                                          modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                          activeJustified = a.activeJustified,
                                          justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                          changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                          activeToplan = a.activeToplan,
                                          idJefe = a.idJefe,
                                          dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                          porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                      }).Distinct();
                    var quer = queryFinal.ToList();
                    if (queryFinal.Count() >= 1)
                    {
                        idSession = Convert.ToInt32(queryFinal.ToList().First().idSession);
                        approved = Convert.ToInt32(queryFinal.ToList().First().approved);
                    }
                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);



                }
                #endregion JefesAprobadores
                #region Administradores y SIG
                else if (ListarTodos)
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO).ToList()
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      TypeApproved = ctr.type,
                                      detailChange = cr.question2 == null ? "" : cr.question2,
                                      //detailChange = (cr.Activo == false ? cr.question2 : Convert.ToString(cr.question2.Substring(0, 61) + tk.COD_TICK)),
                                      idchangeApproved = ca.id,
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      detpriority = pr.detail,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden,
                                      ID_PERS_ENTI_SOL = cr.ID_PERS_ENTI
                                  });

                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI_SOL == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (code > 0)
                    {
                        result = result.Where(x => x.idChangeRequest == code);
                    }
                    int tt = result.Count();
                    var qJefeEdata1 = db.GestionCambioObtenerJefesBNV().ToList();

                    var queryFinal = from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                     join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                     join c in qJefeEdata1 on a.approved equals c.ID_PERS_ENTI
                                     select new
                                     {
                                         idSession = ID_PERS_ENTI,
                                         idChangeRequest = a.idChangeRequest,
                                         idChangeType = a.idChangeType,
                                         ChangeType = a.ChangeType,
                                         ChangeTypeDetail = a.ChangeTypeDetail,
                                         ID_PERS_ENTI = a.ID_PERS_ENTI,
                                         approved = a.approved,
                                         idApproved = a.idApproved,
                                         priority = a.priority,
                                         registro = a.registro,
                                         colorApproved = a.colorApproved,
                                         LetraApproved = a.LetraApproved,
                                         TypeApproved = a.TypeApproved,
                                         User = b.User,
                                         Jefe = c.Jefe,
                                         detailChange = a.detailChange,
                                         DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                         MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                         DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                         TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                         SwApproval = ListarTodos,
                                         idchangeApproved = a.idchangeApproved,
                                         question1 = a.question1,
                                         question3 = a.question3,
                                         question4 = a.question4,
                                         question5 = a.question5,
                                         question6 = a.question6,
                                         question7 = a.question7,
                                         otherChange = a.otherChange,
                                         urgentDetail = a.urgentDetail,
                                         otherImpact = a.otherImpact,
                                         isImpactCheck = a.isImpactCheck,
                                         detpriority = a.detpriority,
                                         modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                         activeJustified = a.activeJustified,
                                         justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                         changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                         activeToplan = a.activeToplan,
                                         idJefe = a.idJefe,
                                         dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                         porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                     };
                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region usuarios
                else
                {
                    var qJefeEdata1 = db.GestionCambioObtenerJefesBNV().ToList();

                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO)
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  join cd in context.CHANGE_DETAIL on cr.id equals cd.ID_CHANGE_REQUEST
                                  join tk in context.TICKETs on cr.IdTicket equals tk.ID_TICK into x
                                  from tk in x.DefaultIfEmpty()

                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      detpriority = pr.detail,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      TypeApproved = ctr.type,
                                      detailChange = (cr.Activo == null ? cr.question2 : cr.question2.Substring(0, 61) + tk.COD_TICK),
                                      //detailChange = (cr.Activo == false ? cr.question2 : Convert.ToString(cr.question2.Split('<'))),
                                      idchangeApproved = ca.id,
                                      //detalle
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden,
                                      IdResponsable = cd.ID_PERSON_ENTI,
                                      ID_PERS_ENTI_SOL = cr.ID_PERS_ENTI
                                  }).Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.IdResponsable == ID_PERS_ENTI).Distinct();
                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI_SOL == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (code > 0)
                    {
                        result = result.Where(x => x.idChangeRequest == code);
                    }
                    int tt = result.Count();
                    int fil = tt - skip;

                    var queryFinal = (from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                      join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                      join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      //join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      select new
                                      {
                                          idSession = ID_PERS_ENTI,
                                          idChangeRequest = a.idChangeRequest,
                                          idChangeType = a.idChangeType,
                                          ChangeType = a.ChangeType,
                                          ChangeTypeDetail = a.ChangeTypeDetail,
                                          ID_PERS_ENTI = a.ID_PERS_ENTI,
                                          approved = a.approved,
                                          idApproved = a.idApproved,
                                          priority = a.priority,
                                          registro = a.registro,
                                          colorApproved = a.colorApproved,
                                          LetraApproved = a.LetraApproved,
                                          TypeApproved = a.TypeApproved,
                                          User = b.User,
                                          Jefe = c.Jefe,
                                          detailChange = a.detailChange == null ? "" : a.detailChange,
                                          DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                          MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                          DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                          TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                          SwApproval = listarAprobadosxJefes,
                                          idchangeApproved = a.idchangeApproved,
                                          //
                                          question1 = a.question1,
                                          question3 = a.question3,
                                          question4 = a.question4,
                                          question5 = a.question5,
                                          question6 = a.question6,
                                          question7 = a.question7,
                                          otherChange = a.otherChange,
                                          urgentDetail = a.urgentDetail,
                                          otherImpact = a.otherImpact,
                                          isImpactCheck = a.isImpactCheck,
                                          detpriority = a.detpriority,
                                          modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                          activeJustified = a.activeJustified,
                                          justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                          changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                          activeToplan = a.activeToplan,
                                          idJefe = a.idJefe,
                                          dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                          porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                      }).Distinct();
                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
        }
        #endregion


        public ActionResult Reportes()
        {
            return View();
        }

        public ActionResult VerTodosNotificaciones()
        {
            return View();
        }

        public ActionResult viewDetailtoPlan()
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            ViewBag.ID_CHANGE_APPROV = id;
            return View();
        }
        public ActionResult viewDetailChange()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int MostrarOpciones = 0, UsuarioResponsable = 0;
            try
            {
                var IdAprobacion = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == id).SingleOrDefault();

                if (IdAprobacion.idTypeRequest == 2 || IdAprobacion.idTypeRequest == 7)
                {
                    MostrarOpciones = 1;
                }

                if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1)
                {
                    UsuarioResponsable = 1;
                }
            }
            catch
            {
                MostrarOpciones = 0;
            }

            ViewBag.ID_CHANGE_REQUEST = id;
            ViewBag.IdPersEnti = IdPersEnti;
            ViewBag.MostrarOpciones = MostrarOpciones;
            ViewBag.Admin = UsuarioResponsable;

            return View();
        }



        public ActionResult viewDetailActivity()
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            ViewBag.ID_CHANGE_REQUEST_TOREJECT = id;
            return View();
        }
        public ActionResult Consolidado()
        {
            return View();
        }
        public ActionResult RegistrarGestionCambio(int id)
        {

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = db.ObtenerCategoriaTicket(id, IdCuenta).SingleOrDefault();
            int IdTipoGestionCambio = 0;
            string TipoGestionCambio = "";
            if (query != null)
            {
                IdTipoGestionCambio = Convert.ToInt32(query.IdTipoGestionCambio);
                TipoGestionCambio = query.TipoGestionCambio;
            }
            ViewBag.IdTipoCambioCategoria = IdTipoGestionCambio;
            ViewBag.TipoCambioCategoria = TipoGestionCambio;
            ViewBag.IdCuenta = IdCuenta;
            ViewBag.IdTicket = id;
            return View();
        }

        public ActionResult RegistrarGestionCambioBNV(int id)
        {

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = db.ObtenerCategoriaTicket(id, IdCuenta).SingleOrDefault();
            int IdTipoGestionCambio = 0;
            string TipoGestionCambio = "";
            if (query != null)
            {
                IdTipoGestionCambio = Convert.ToInt32(query.IdTipoGestionCambio);
                TipoGestionCambio = query.TipoGestionCambio;
            }
            ViewBag.IdTipoCambioCategoria = IdTipoGestionCambio;
            ViewBag.TipoCambioCategoria = TipoGestionCambio;
            ViewBag.IdCuenta = IdCuenta;
            ViewBag.IdTicket = id;
            return View();
        }

        public ActionResult RegistrarGestionActivos(int id)
        {

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = db.ObtenerCategoriaTicket(id, IdCuenta).SingleOrDefault();
            int IdTipoGestionCambio = 0;
            string TipoGestionCambio = "";
            if (query != null)
            {
                IdTipoGestionCambio = Convert.ToInt32(query.IdTipoGestionCambio);
                TipoGestionCambio = query.TipoGestionCambio;
            }
            ViewBag.IdTipoCambioCategoria = IdTipoGestionCambio;
            ViewBag.TipoCambioCategoria = TipoGestionCambio;
            ViewBag.IdCuenta = IdCuenta;
            ViewBag.IdTicket = id;
            return View();
        }

        public ActionResult ResumenGestionCambio(int id)
        {
            var query = db.CHANGE_APPROVED.Single(x => x.idChangeRequest == id);

            var ListarTodos = "0";
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1)
            {
                ListarTodos = "1";
            }

            ViewBag.IdApproved = query.idTypeRequest;
            ViewBag.IdSession = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            ViewBag.SwApproval = ListarTodos;
            ViewBag.Approved = query.approver;
            ViewBag.IdGestionCambio = id;
            return View();
        }
        public ActionResult EditarDetalleCambio(int id)
        {
            ViewBag.IdDetalleCambio = id;
            var detalle = db.CHANGE_DETAIL.Where(x => x.ID == id && x.STATUS == true).SingleOrDefault();
            ViewBag.FechaInicioReal = String.Format("{0:MM/dd/yyyy}", detalle.FechaInicioProgramada);
            ViewBag.FechaFinReal = String.Format("{0:MM/dd/yyyy}", detalle.FechaFinProgramada);
            return View();
        }

        public ActionResult EditarActividades(int id)
        {

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            //(ERP)
            //if (IdCuenta)
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            int Acceso = 0;

            var cambio = db.CHANGE_REQUEST.Where(x => x.id == id).SingleOrDefault();
            var aprovador = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == id).SingleOrDefault();
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1 ||
                cambio.ID_PERS_ENTI == IdPersEnti || aprovador.approver == IdPersEnti)
            {
                Acceso = 1; //permitiendo al acceso
            }

            var person = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti).FirstOrDefault();
            var usuario = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == person.ID_ENTI2).FirstOrDefault();
            //var person = 73123;
            //    //(from d in db.CHANGE_REQUEST
            //    //           where d.id==id
            //    //           select d.ID_PERS_ENTI_ASSI
            //    //         ).FirstOrDefault();
            // var usuario = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == person).FirstOrDefault();

            ViewBag.IdPersEnti = IdPersEnti;
            ViewBag.Usuario = Convert.ToString(usuario.FIR_NAME + ' ' + usuario.LAS_NAME);
            ViewBag.IdCuenta = IdCuenta;
            ViewBag.IdCambio = id;
            ViewBag.Acceso = Acceso;
            return View();
        }
        #endregion

        #region Metodos

        public ActionResult ListarCambioClasificacionTipo(string q, string page, int id)
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

            var result = db.GestionCambioListarClasificacionTipo(id, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCambioPrioridad(string q, string page, int id, int id1)
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

            var result = db.GestionCambioListarPrioridad(id, id1, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCambioTareas(string q, string page, int id)
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

            var result = db.GestionCambioListarTareas(id, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCambioResponsable(string q, string page, int id)
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

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

                var result = db.GestionCambioListarResponsable(id, termino).ToList();
            

            return Json(result, JsonRequestBehavior.AllowGet);
        }

       

        public ActionResult ListarCambioEstados(string q, string page)
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

            var result = db.GestionCambioListarEstados(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerImpactoCambio(int id, int id1)
        {
            var result = db.GestionCambioObtenerImpacto(id, id1).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerTipoDescripcion(int id)
        {
            var result = db.CHANGE_TYPE.Where(x => x.id == id).SingleOrDefault();
            try
            {
                return Json(result.descripcion, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ObtenerOtroImpacto(int id)
        {
            var result = db.CHANGE_IMPACT.Where(x => x.id == id).SingleOrDefault();
            try
            {
                return Json(result.isChecked, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListarActividadesCambio(int id)
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            //var result = db.GestionCambioListarActividades(id, IdCuenta).ToList();
            var result = db.GestionCambioListarActividades(id, IdCuenta).ToList(); //CAMBIAR
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SolucionDefinitivaActividades(int id)
        {
            var result = db.GestionCambioSolucionDefinitiva(id).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarGestionCambio(FormCollection collection)
        {

            //Variables
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            int ClasificacionCambio = Convert.ToInt32(Request.Params["cbClasificacionCambio"]);
            int IdTicket = Convert.ToInt32(Request.Params["IdTicket"]);
            int TipoCambio = Convert.ToInt32(Request.Params["cbTipoCambio"]);
            int Prioridad = Convert.ToInt32(Request.Params["cbPrioridad"]);

            int contador = Convert.ToInt32(Request.Params["Contador"]);
            string ImpactoDescripcion = Convert.ToString(Request.Params["txtImpactoDescripcion"]);

            if (ClasificacionCambio == 0 || Prioridad == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
            }

            var tipoDescripcion = db.CHANGE_TYPE.Where(x => x.id == ClasificacionCambio && x.status == true).SingleOrDefault();
            string txtTipoDescripcion = "";
            if (tipoDescripcion.descripcion == true)
            {
                txtTipoDescripcion = Convert.ToString(Request.Params["txtTipoDescripcion"]);
                if (txtTipoDescripcion == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
                var TipoCambioUrgente = db.CHANGE_TYPE_DETAIL.Where(x => x.idChangeType == ClasificacionCambio && x.status == true).FirstOrDefault();
                TipoCambio = Convert.ToInt32(TipoCambioUrgente.id);
            }
            else
            {
                if (TipoCambio == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
            }

            //Preguntas
            string txtPregunta1 = Convert.ToString(Request.Params["txtPregunta1"]);
            string txtPregunta2 = Convert.ToString(Request.Params["txtPregunta2"]);
            string txtPregunta3 = Convert.ToString(Request.Params["txtPregunta3"]);
            string txtPregunta4 = Convert.ToString(Request.Params["txtPregunta4"]);

            if (txtPregunta1 == "" || txtPregunta2 == "" || txtPregunta3 == "" || txtPregunta4 == "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Por favor responda todas las preguntas','2');}window.onload=init;</script>");
            }

            //Impacto
            IQueryable<CHANGE_IMPACT> impact = from ci in db.CHANGE_IMPACT
                                               where ci.IdCuenta == IdCuenta && ci.status == true
                                               select ci;

            CHANGE_IMPACT[] impactArray = impact.ToArray();


            string chkImpact = "";
            foreach (CHANGE_IMPACT imp in impactArray)
            {
                if (Convert.ToString(collection[imp.detail]) != null)
                {
                    chkImpact = chkImpact + imp.id + ",";
                }

            }

            //Validación - Detalle del Cambio
            int Actividad = 0, Responsable = 0;
            string FechaInicio = "", FechaFin = "", descripcion = "";
            int flag = 0;
            for (int i = 0; i <= contador; i++)
            {
                if (Convert.ToString(collection["Descripcion" + i]) != null)
                {
                    flag = flag + 1;
                    Actividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                    Responsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                    FechaInicio = Convert.ToString(Request.Params["dtFechaInicio" + i]);
                    FechaFin = Convert.ToString(Request.Params["dtFechaFin" + i]);
                    descripcion = Convert.ToString(Request.Params["Descripcion" + i]);

                    if (Actividad == 0 || Responsable == 0 || FechaInicio == "" || FechaFin == "" || descripcion == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registre los datos del Detalle del Cambio.','2');}window.onload=init;</script>");
                    }

                }
            }
            if (flag == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registre el Detalle del Cambio.','2');}window.onload=init;</script>");
            }

            try
            {
                //Registro de Aprobación de Usuario
                var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).ToList();
                var ID_PERS_ENTI_APPR = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);

                var changeRequest = new CHANGE_REQUEST();
                changeRequest.idChangeType = ClasificacionCambio;
                changeRequest.idChangeTypeDetail = TipoCambio;
                changeRequest.isImpactCheck = chkImpact;
                changeRequest.otherImpact = ImpactoDescripcion;
                changeRequest.idPriority = Prioridad;
                changeRequest.urgentDetail = txtTipoDescripcion;
                changeRequest.otherChange = "";
                changeRequest.question1 = txtPregunta1;
                changeRequest.question2 = txtPregunta2;
                changeRequest.question3 = txtPregunta3;
                changeRequest.question4 = txtPregunta4;
                changeRequest.ID_PERS_ENTI = IdPersEnti;
                changeRequest.createDate = DateTime.Now;
                changeRequest.IdCuenta = IdCuenta;
                changeRequest.IdTicket = IdTicket;
                db.CHANGE_REQUEST.Add(changeRequest);
                db.SaveChanges();


                var approved = new CHANGE_APPROVED();
                approved.idChangeRequest = changeRequest.id;
                approved.idTypeRequest = 1;
                approved.approver = ID_PERS_ENTI_APPR;
                approved.createDate = DateTime.Now;
                approved.activeJustified = false;
                approved.activeToplan = 0;
                db.CHANGE_APPROVED.Add(approved);
                db.SaveChanges();
                #region Detalle de Solicitud

                var cambioDetalle = new CHANGE_DETAIL();

                int IdActividad = 0, IdResponsable = 0;
                string DescripcionActividad = "";

                for (int i = 0; i <= contador; i++)
                {
                    if (Convert.ToString(collection["Descripcion" + i]) != null)
                    {
                        IdActividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                        IdResponsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                        DescripcionActividad = Convert.ToString(Request.Params["Descripcion" + i]);

                        cambioDetalle.ID_TYPE_TASK = IdActividad;
                        cambioDetalle.ID_CHANGE_REQUEST = changeRequest.id;
                        cambioDetalle.ID_PERSON_ENTI = IdResponsable;
                        cambioDetalle.DETAIL = DescripcionActividad;
                        cambioDetalle.DATE_INIC = Convert.ToDateTime(Request.Params["dtFechaInicio" + i]);
                        cambioDetalle.DATE_END = Convert.ToDateTime(Request.Params["dtFechaFin" + i]);
                        cambioDetalle.STATUS = true;
                        cambioDetalle.FechaInicioProgramada = Convert.ToDateTime(Request.Params["dtFechaInicio" + i]);
                        cambioDetalle.FechaFinProgramada = Convert.ToDateTime(Request.Params["dtFechaFin" + i]);
                        db.CHANGE_DETAIL.Add(cambioDetalle);
                        db.SaveChanges();
                    }
                }

                #endregion

                var detailsTicket = new DETAILS_TICKET();
                detailsTicket.ID_TICK = IdTicket;
                detailsTicket.ID_TYPE_DETA_TICK = 1;
                detailsTicket.COM_DETA_TICK = "Se registra una solicitud de cambio.";
                detailsTicket.UserId = 34;
                detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                detailsTicket.MINUTES = 0;
                db.DETAILS_TICKET.Add(detailsTicket);
                db.SaveChanges();


                ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                mail.sendMailGestionCambios(IdPersEnti, approved, "S");
                //Envio de correo a los involucrados de las actividades
                var query = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == changeRequest.id);
                foreach (var detail in query)
                {
                    mail.sendMailUsersRequest(detail, approved, 1);
                }
            }
            catch (Exception e)
            {
                string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MensajeConfirmacion) top.MensajeConfirmacion('No se registró la solicitud del cambio, error al enviar el correo. Contacte al administrador.','1');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('El Comité del SIG y/o tu Gerencia respectiva se comunicarán con Ud. para desarrollar el plan de acción sobre el cambio.','1');}window.onload=init;</script>");
        }

        public ActionResult GuardarGestionCambioBNV(FormCollection collection)
        {

            //Variables
            int salida = 1;
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            int ClasificacionCambio = Convert.ToInt32(Request.Params["cbClasificacionCambio"]);
            int IdTicket = Convert.ToInt32(Request.Params["IdTicket"]);
            int TipoCambio = Convert.ToInt32(Request.Params["cbTipoCambio"]);
            int Prioridad = Convert.ToInt32(Request.Params["cbPrioridad"]);

            int contador = Convert.ToInt32(Request.Params["Contador"]);
            string ImpactoDescripcion = Convert.ToString(Request.Params["txtImpactoDescripcion"]);

            if (ClasificacionCambio == 0 || Prioridad == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
            }

            var tipoDescripcion = db.CHANGE_TYPE.Where(x => x.id == ClasificacionCambio && x.status == true).SingleOrDefault();
            string txtTipoDescripcion = "";
            if (tipoDescripcion.descripcion == true)
            {
                txtTipoDescripcion = Convert.ToString(Request.Params["txtTipoDescripcion"]);
                if (txtTipoDescripcion == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
                var TipoCambioUrgente = db.CHANGE_TYPE_DETAIL.Where(x => x.idChangeType == ClasificacionCambio && x.status == true).FirstOrDefault();
                TipoCambio = Convert.ToInt32(TipoCambioUrgente.id);
            }
            else
            {
                if (TipoCambio == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
            }

            //Preguntas
            string txtPregunta1 = Convert.ToString(Request.Params["txtPregunta1"]);
            string txtPregunta2 = Convert.ToString(Request.Params["txtPregunta2"]);
            string txtPregunta3 = Convert.ToString(Request.Params["txtPregunta3"]);
            string txtPregunta4 = Convert.ToString(Request.Params["txtPregunta4"]);

            //if (txtPregunta1 == "" || txtPregunta2 == "" || txtPregunta3 == "" || txtPregunta4 == "")
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //             "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Por favor responda todas las preguntas','2');}window.onload=init;</script>");
            //}

            //Impacto
            IQueryable<CHANGE_IMPACT> impact = from ci in db.CHANGE_IMPACT
                                               where ci.IdCuenta == IdCuenta && ci.status == true
                                               select ci;

            CHANGE_IMPACT[] impactArray = impact.ToArray();


            string chkImpact = "";
            foreach (CHANGE_IMPACT imp in impactArray)
            {
                if (Convert.ToString(collection[imp.detail]) != null)
                {
                    chkImpact = chkImpact + imp.id + ",";
                }

            }

            //Validación - Detalle del Cambio
            int Actividad = 0, Responsable = 0;
            string FechaInicio = "", FechaFin = "", descripcion = "";
            int flag = 0;
            //Add
            int cbActividad = 9;
            for (int i = 0; i <= contador; i++)
            {
                if (Convert.ToString(collection["Descripcion" + i]) != null)
                {
                    flag = flag + 1;
                    Actividad = cbActividad;
                    if (i == 0 || i == 2 || i == 3)
                    {
                        Responsable = 79581;
                    }
                    else if (i == 1 || i == 4)
                    {
                        Responsable = 79579;
                    }
                    else
                    {
                        Responsable = 77094;
                    }
                    //Actividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                    // Responsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                    FechaInicio = Convert.ToString(Request.Params["dtFechaInicio" + i]);
                    FechaFin = Convert.ToString(Request.Params["dtFechaFin" + i]);
                    descripcion = Convert.ToString(Request.Params["Descripcion" + i]);

                    if (Actividad == 0 || Responsable == 0 || FechaInicio == "" || FechaFin == "" || descripcion == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registre los datos del Detalle del Cambio.','2');}window.onload=init;</script>");
                    }

                }
                cbActividad = cbActividad + 1;
            }
            if (flag == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registre el Detalle del Cambio.','2');}window.onload=init;</script>");
            }

            try
            {
                var JefeInmediato = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti)
                                     select pe.EMA_ELEC).Single();
                var ID_PERS_ENTI_APPR = IdPersEnti;
                //Registro de Aprobación de Usuario
                if (ClasificacionCambio == 8)
                {
                    //  var   mailJefe = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).Single();
                    //  var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).ToList();
                    ////  var  JefeInmediato = mailJefe.EMA_ELEC;
                    //  // JefeInmediato = "erivera@electrodata.com.pe";
                    //  var ID_PERS_ENTI_APPR = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);
                };



                var changeRequest = new CHANGE_REQUEST();
                changeRequest.idChangeType = ClasificacionCambio;
                changeRequest.idChangeTypeDetail = TipoCambio;
                changeRequest.isImpactCheck = chkImpact;
                changeRequest.otherImpact = ImpactoDescripcion;
                changeRequest.idPriority = Prioridad;
                changeRequest.urgentDetail = txtTipoDescripcion;
                changeRequest.otherChange = "";
                changeRequest.question1 = txtPregunta1;
                changeRequest.question2 = txtPregunta2;
                changeRequest.question3 = txtPregunta3;
                changeRequest.question4 = txtPregunta4;
                changeRequest.ID_PERS_ENTI = IdPersEnti;
                changeRequest.createDate = DateTime.Now;
                changeRequest.IdCuenta = IdCuenta;
                changeRequest.IdTicket = IdTicket;
                db.CHANGE_REQUEST.Add(changeRequest);
                db.SaveChanges();


                var approved = new CHANGE_APPROVED();
                approved.idChangeRequest = changeRequest.id;
                approved.idTypeRequest = 2;
                approved.approver = ID_PERS_ENTI_APPR;
                approved.createDate = DateTime.Now;
                approved.activeJustified = false;
                approved.activeToplan = 2;
                db.CHANGE_APPROVED.Add(approved);
                db.SaveChanges();
                #region Detalle de Solicitud

                var cambioDetalle = new CHANGE_DETAIL();

                int IdActividad = 0, IdResponsable = 0;
                string DescripcionActividad = "";
                cbActividad = 9;
                for (int i = 0; i <= contador; i++)
                {
                    if (Convert.ToString(collection["Descripcion" + i]) != null)
                    {
                        IdActividad = cbActividad;
                        //IdActividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                        if (i == 0 || i == 2 || i == 3)
                        {
                            IdResponsable = IdPersEnti;
                        }
                        else if (i == 1 || i == 4)
                        {
                            IdResponsable = 79579;
                        }
                        else
                        {
                            IdResponsable = 77094;
                        }
                        //IdResponsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                        DescripcionActividad = Convert.ToString(Request.Params["Descripcion" + i]);

                        cambioDetalle.ID_TYPE_TASK = IdActividad;
                        cambioDetalle.ID_CHANGE_REQUEST = changeRequest.id;
                        cambioDetalle.ID_PERSON_ENTI = IdResponsable;
                        cambioDetalle.DETAIL = DescripcionActividad;
                        cambioDetalle.DATE_INIC = Convert.ToDateTime(Request.Params["dtFechaInicio" + i]);
                        cambioDetalle.DATE_END = Convert.ToDateTime(Request.Params["dtFechaFin" + i]);
                        cambioDetalle.STATUS = true;
                        cambioDetalle.FechaInicioProgramada = Convert.ToDateTime(Request.Params["dtFechaInicio" + i]);
                        cambioDetalle.FechaFinProgramada = Convert.ToDateTime(Request.Params["dtFechaFin" + i]);
                        //ADD LAST 05/05/2023
                        cambioDetalle.ActividadValidada = true;
                        cambioDetalle.FechaActividadValidada = DateTime.UtcNow;

                        db.CHANGE_DETAIL.Add(cambioDetalle);
                        db.SaveChanges();
                        cbActividad = cbActividad + 1;
                    }
                }

                #endregion

                var detailsTicket = new DETAILS_TICKET();
                detailsTicket.ID_TICK = IdTicket;
                detailsTicket.ID_TYPE_DETA_TICK = 1;
                detailsTicket.COM_DETA_TICK = "Se registra una solicitud de cambio.";
                detailsTicket.UserId = 34;
                detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                detailsTicket.MINUTES = 0;
                db.DETAILS_TICKET.Add(detailsTicket);
                db.SaveChanges();

                var idRequester = (from cr in db.CHANGE_REQUEST
                                   join t in db.TICKETs.Where(x => x.ID_TICK == IdTicket) on cr.IdTicket equals t.ID_TICK
                                   select new
                                   {
                                       t.ID_PERS_ENTI
                                   }).FirstOrDefault();

                ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                salida = mail.sendMailGestionCambiosBNV(Convert.ToInt32(idRequester.ID_PERS_ENTI), approved, "S");
                //Envio de correo a los involucrados de las actividades
                var query = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == changeRequest.id);
                foreach (var detail in query)
                {
                    mail.sendMailUsersRequestBNV(detail, approved, 1);
                }
                if (salida == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                                   "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Se registró la solicitud del cambio, error al enviar un correo. Contacte al administrador.','1');}window.onload=init;</script>");

                }


            }
            catch (Exception e)
            {
                string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                if (salida == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                                   "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Se registró la solicitud del cambio, error al enviar un correo. Contacte al administrador.','1');}window.onload=init;</script>");

                }
            }
             //Actualizar estado de reafirmación de cambios
             
            //if(salida==1)
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Se registró la solicitud para desarrollar el plan de acción sobre el cambio.','1');}window.onload=init;</script>");

        }

        public ActionResult EditarFechasCambio()
        {

            if (Convert.ToString(Request.Params["dtFechaInicioReal"]).ToString() == "" || Convert.ToString(Request.Params["dtFechaFinReal"]).ToString() == "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                                  "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios.','2');}window.onload=init;</script>");
            }

            int IdDetalleActividad = Convert.ToInt32(Request.Params["IdDetalleCambio"]);
            CHANGE_DETAIL CD = db.CHANGE_DETAIL.Where(x => x.ID == IdDetalleActividad).FirstOrDefault();

            CD.FechaInicioProgramada = Convert.ToDateTime(Request.Params["dtFechaInicioReal"]);
            CD.FechaFinProgramada = Convert.ToDateTime(Request.Params["dtFechaFinReal"]);

            db.Entry(CD).State = EntityState.Modified;
            db.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                         "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registro guardado correctamente.','1');}window.onload=init;</script>");
        }

        [Authorize]
        public ActionResult RegistrarActividadCambio()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdDetalleCambio = Convert.ToInt32(Request.Params["IdDetalleCambio"]);
            int IdCambio = Convert.ToInt32(Request.Params["IdCambio"]);
            string mensaje = "Actividad Realizada";
            db.sp_Email_False(IdDetalleCambio);
            //Registrar Actividad Realizada
            CHANGE_DETAIL CD = db.CHANGE_DETAIL.Where(x => x.ID == IdDetalleCambio).FirstOrDefault();
            CD.ActividadRealizada = true;
            CD.FechaActividadRealizada = DateTime.Now;
            db.Entry(CD).State = EntityState.Modified;
            db.SaveChanges();

            db.GestionCambioRegistrarActividadTicket(IdCambio);

            int cambioDetalle = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio).Count();
            CHANGE_APPROVED catoPlan = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == @IdCambio).FirstOrDefault();

            //Envio de correo a los involucrados de las actividades
            ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
            var listado = db.GestionCambioListarActividades(IdCambio, IdCuenta).Where(x => x.Estado == 1);
            if (IdCuenta == 60)
            {
                foreach (var query in listado)
                {
                    var detail = db.CHANGE_DETAIL.Where(x => x.ID == query.IdDetalleCambio && (x.EnvioCorreo == false || x.EnvioCorreo == null)).SingleOrDefault();
                    if (detail != null)
                    {
                        detail.EnvioCorreo = true;
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                        mail.sendMailUsersRequestBNV(detail, catoPlan, 3);
                    }
                }

                if (cambioDetalle == 1)
                {
                    //Enviar correo a Usuario para informar que el plan se ejecuto
                    //int idRequester = Convert.ToInt32(db.CHANGE_REQUEST.Single(x => x.id == IdCambio).ID_PERS_ENTI_ASSI);
                    var idRequester = (from cr in db.CHANGE_REQUEST.Where(x => x.id == IdCambio)
                                       join t in db.TICKETs on cr.IdTicket equals t.ID_TICK
                                       select new
                                       {
                                           t.ID_PERS_ENTI
                                       }).FirstOrDefault();
                    ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                    smail.sendMailGestionCambiosBNV(Convert.ToInt32(idRequester.ID_PERS_ENTI), catoPlan, "E");
                    mensaje = "La solicitud del cambio se ha ejecutado";
                }
                else
                {
                    int ultimaActividad = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio
                                            && (x.ActividadRealizada == null || x.ActividadRealizada == false) && x.STATUS == true).Count();
                    if (ultimaActividad == 0)
                    {
                        //Enviar correo a Usuario para informar que el plan se ejecuto
                        //int idRequester = Convert.ToInt32(db.CHANGE_REQUEST.Single(x => x.id == IdCambio).ID_PERS_ENTI_ASSI);

                        var idRequester = (from cr in db.CHANGE_REQUEST.Where(x => x.id == IdCambio)
                                           select new
                                           {
                                               cr.ID_PERS_ENTI
                                           }).FirstOrDefault();

                        ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                        smail.sendMailGestionCambiosBNV(Convert.ToInt32(idRequester.ID_PERS_ENTI), catoPlan, "E");
                        mensaje = "La solicitud del cambio se ha ejecutado";

                    }
                }
            }
            else
            {
                foreach (var query in listado)
                {
                    var detail = db.CHANGE_DETAIL.Where(x => x.ID == query.IdDetalleCambio && (x.EnvioCorreo == false || x.EnvioCorreo == null)).SingleOrDefault();
                    if (detail != null)
                    {
                        detail.EnvioCorreo = true;
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                        mail.sendMailUsersRequest(detail, catoPlan, 3);
                    }
                }

                if (cambioDetalle == 1)
                {
                    int ID_ACCO_MNS = Convert.ToInt32(Session["ID_ACCO"]);
                    //Enviar correo a Usuario para informar que el plan se ejecuto
                    int idRequester = Convert.ToInt32(db.CHANGE_REQUEST.Single(x => x.id == IdCambio).ID_PERS_ENTI);
                    ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();


                    if (ID_ACCO_MNS == 56)
                    {
                        smail.sendMailGestionCambios(idRequester, catoPlan, "EM");
                    }
                    else
                    {
                        smail.sendMailGestionCambios(idRequester, catoPlan, "E");

                    }
                    mensaje = "La solicitud del cambio se ha ejecutado";
                }
                else
                {
                    int ultimaActividad = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio
                                            && (x.ActividadRealizada == null || x.ActividadRealizada == false) && x.STATUS == true).Count();
                    if (ultimaActividad == 0)
                    {
                        //Enviar correo a Usuario para informar que el plan se ejecuto
                        int idRequester = Convert.ToInt32(db.CHANGE_REQUEST.Single(x => x.id == IdCambio).ID_PERS_ENTI);
                        ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                        smail.sendMailGestionCambios(idRequester, catoPlan, "E");
                        mensaje = "La solicitud del cambio se ha ejecutado";

                    }
                }

            }

            return Content(mensaje);
        }


        [Authorize]
        public ActionResult RechazarActividad()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdDetalleCambio = Convert.ToInt32(Request.Params["IdDetalleCambio"]);
            int IdCambio = Convert.ToInt32(Request.Params["IdCambio"]);

            CHANGE_DETAIL CDD = db.CHANGE_DETAIL.Where(x => x.ID == (IdDetalleCambio)).FirstOrDefault();
            CDD.EnvioCorreo = false;
            db.Entry(CDD).State = EntityState.Modified;
            db.SaveChanges();

            ObjectParameter OutputParam = new ObjectParameter("idReturn", typeof(int));
            db.GestionCambioObtenerOrderBNV(IdCambio, IdDetalleCambio, OutputParam);
            IdDetalleCambio = Convert.ToInt32(OutputParam.Value);
            string mensaje = "Actividad realizada incorrectamente";

            
            //Registrar Actividad Realizada
            CHANGE_DETAIL CD = db.CHANGE_DETAIL.Where(x => x.ID == IdDetalleCambio).FirstOrDefault();
            CD.ActividadRealizada = false;
            CD.FechaActividadRealizada = DateTime.Now;
            db.Entry(CD).State = EntityState.Modified;
            db.SaveChanges();

            db.GestionCambioRegistrarActividadTicket(IdCambio);

            int cambioDetalle = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio).Count();
            CHANGE_APPROVED catoPlan = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == @IdCambio).FirstOrDefault();

            //Envio de correo a los involucrados de las actividades
            ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
            var listado = db.GestionCambioListarActividades(IdCambio, IdCuenta).Where(x => x.Estado == 0);
            //foreach (var query in listado)
            //{
            var detail = db.CHANGE_DETAIL.Where(x => x.ID == IdDetalleCambio).SingleOrDefault();
            if (detail != null)
            {
                detail.EnvioCorreo = false;
                db.Entry(detail).State = EntityState.Modified;
                db.SaveChanges();
                mail.sendMailUsersRequestBNV(detail, catoPlan, 3);
            }
            //}

            
                mensaje = "La actividad del cambio se ha ejecutado";
           

            return Content(mensaje);
        }
        public ActionResult AgregarActividadCambio()
        {
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            int IdCambio = Convert.ToInt32(Request.Params["IdCambio"]);
            //Validaciones
            string Actividad = "", Responsable = "", FechaInicio = "", FechaFin = "", descripcion = "";
            Actividad = Convert.ToString(Request.Params["cbActividad"]);
            Responsable = Convert.ToString(Request.Params["cbResponsable"]);
            FechaInicio = Convert.ToString(Request.Params["dtFechaInicio"]).ToString();
            FechaFin = Convert.ToString(Request.Params["dtFechaFin"]).ToString();
            descripcion = Convert.ToString(Request.Params["Descripcion"]);


            if (Actividad == "null" || Responsable == "null" || FechaInicio == "" || FechaFin == "" || descripcion == "")
            {
                return Content("Debe ingresar todos los campos.");
            }

            //Registrar Actividad Realizada
            var cambioDetalle = new CHANGE_DETAIL();
            cambioDetalle.ID_TYPE_TASK = Convert.ToInt32(Actividad);
            cambioDetalle.ID_CHANGE_REQUEST = IdCambio;
            cambioDetalle.ID_PERSON_ENTI = Convert.ToInt32(Responsable);
            cambioDetalle.DETAIL = descripcion;
            cambioDetalle.DATE_INIC = Convert.ToDateTime(Request.Params["dtFechaInicio"]);
            cambioDetalle.DATE_END = Convert.ToDateTime(Request.Params["dtFechaFin"]);
            cambioDetalle.STATUS = true;
            cambioDetalle.FechaInicioProgramada = Convert.ToDateTime(Request.Params["dtFechaInicio"]);
            cambioDetalle.FechaFinProgramada = Convert.ToDateTime(Request.Params["dtFechaFin"]);
            cambioDetalle.ActividadValidada = true;
            cambioDetalle.FechaActividadValidada = DateTime.Now;
            db.CHANGE_DETAIL.Add(cambioDetalle);
            db.SaveChanges();


            int cambio = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio).Count();
            CHANGE_APPROVED catoPlan = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == @IdCambio).FirstOrDefault();
            int correo = 0;
            if (catoPlan.idTypeRequest == 8)
            {
                correo = 1;
            }

            if (cambio == 1)
            {
                catoPlan.idTypeRequest = 8;
                catoPlan.modifiedDate = DateTime.Now;
                db.Entry(catoPlan).State = EntityState.Modified;
                db.SaveChanges();

                //Enviar correo a Usuario para informar que el plan se valido
                if (correo == 0)
                {
                    ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                    mail.sendMailGestionCambios(IdPersEnti, catoPlan, "V");
                }
            }
            else
            {
                int ultimaActividad = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio
                                        && (x.ActividadValidada == null || x.ActividadValidada == false) && x.STATUS == true).Count();
                if (ultimaActividad == 0)
                {
                    catoPlan.idTypeRequest = 8;
                    catoPlan.modifiedDate = DateTime.Now;
                    db.Entry(catoPlan).State = EntityState.Modified;
                    db.SaveChanges();

                    //Enviar correo a Usuario para informar que el plan se valido
                    if (correo == 0)
                    {
                        ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                        mail.sendMailGestionCambios(IdPersEnti, catoPlan, "V");
                    }

                }
            }

            return Content("OK");
        }
        [Authorize]
        public ActionResult ValidarActividadCambio()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            int IdDetalleCambio = Convert.ToInt32(Request.Params["IdDetalleCambio"]);
            int IdCambio = Convert.ToInt32(Request.Params["IdCambio"]);
            //var idPersEntiSol = (from d in db.CHANGE_REQUEST
            //                     where d.id == IdCambio
            //                     select d.ID_PERS_ENTI_ASSI).SingleOrDefault();

            var idPersEntiSol = (from cr in db.CHANGE_REQUEST.Where(x => x.id == IdCambio)
                                 join t in db.TICKETs on cr.IdTicket equals t.ID_TICK
                                 select new
                                 {
                                     t.ID_PERS_ENTI
                                 }).FirstOrDefault();

            //Validaciones
            string Actividad = "", Responsable = "", FechaInicio = "", FechaFin = "", descripcion = "";
            Actividad = Convert.ToString(Request.Params["cbActividad"]);
            Responsable = Convert.ToString(Request.Params["cbResponsable"]);
            FechaInicio = Convert.ToString(Request.Params["dtFechaInicio"]).ToString();
            FechaFin = Convert.ToString(Request.Params["dtFechaFin"]).ToString();
            descripcion = Convert.ToString(Request.Params["Descripcion"]);


            if (Actividad == "null" || Responsable == "null" || FechaInicio == "" || FechaFin == "" || descripcion == "")
            {
                return Content("Debe ingresar todos los campos.");
            }

            //Registrar Actividad Realizada
            CHANGE_DETAIL CD = db.CHANGE_DETAIL.Where(x => x.ID == IdDetalleCambio).FirstOrDefault();
            CD.ID_TYPE_TASK = Convert.ToInt32(Actividad);
            CD.ID_PERSON_ENTI = Convert.ToInt32(Responsable);
            CD.FechaInicioProgramada = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", Request.Params["dtFechaInicio"]));
            CD.FechaFinProgramada = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", Request.Params["dtFechaFin"]));
            CD.DETAIL = descripcion;
            CD.ActividadValidada = true;
            CD.FechaActividadValidada = DateTime.Now;
            db.Entry(CD).State = EntityState.Modified;
            db.SaveChanges();

            int cambioDetalle = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio).Count();
            CHANGE_APPROVED catoPlan = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == @IdCambio).FirstOrDefault();
            int correo = 0;
            if (catoPlan.idTypeRequest == 8)
            {
                correo = 1;
            }

            if (cambioDetalle == 1)
            {
                catoPlan.idTypeRequest = 8;
                catoPlan.modifiedDate = DateTime.Now;
                db.Entry(catoPlan).State = EntityState.Modified;
                db.SaveChanges();
                //Enviar correo a Usuario para informar que el plan se valido
                if (correo == 0)
                {
                    ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                    if (ID_ACCO == 60)
                    {
                        mail.sendMailGestionCambiosBNV(Convert.ToInt32(idPersEntiSol.ID_PERS_ENTI), catoPlan, "V");
                        //mail.sendMailGestionCambiosBNV(IdPersEnti, catoPlan, "V");
                    }
                    else
                    {
                        mail.sendMailGestionCambios(IdPersEnti, catoPlan, "V");
                    }

                }
            }
            else
            {
                int ultimaActividad = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio
                                        && (x.ActividadValidada == null || x.ActividadValidada == false) && x.STATUS == true).Count();
                if (ultimaActividad == 0)
                {
                    catoPlan.idTypeRequest = 8;
                    catoPlan.modifiedDate = DateTime.Now;
                    db.Entry(catoPlan).State = EntityState.Modified;
                    db.SaveChanges();

                    //Enviar correo a Usuario para informar que el plan se valido
                    if (correo == 0)
                    {
                            ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                        if (ID_ACCO == 60)
                        {
                            mail.sendMailGestionCambiosBNV(Convert.ToInt32(idPersEntiSol.ID_PERS_ENTI), catoPlan, "V");
                            //  mail.sendMailGestionCambiosBNV(IdPersEnti, catoPlan, "V");
                            //   return Content("<script type='text/javascript'> function init() {" +
                            //"if(top.MensajeConfirmacion) top.MensajeConfirmacion('Actividades aprobadas para desarrollar el plan de acción sobre el cambio.','1');}window.onload=init;</script>");

                            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Actividades aprobadas para desarrollar el para desarrollar el plan de acción sobre el cambio.','1');}window.onload=init;</script>");

                        }
                        else
                        {
                            mail.sendMailGestionCambios(IdPersEnti, catoPlan, "V");
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('El Comité del SIG y/o tu Gerencia respectiva se comunicarán con Ud. para desarrollar el plan de acción sobre el cambio.','1');}window.onload=init;</script>");

                        }

                    }

                }
            }

            return Content("OK");

        }

        [Authorize]
        public ActionResult ValidarActividadEliminada()
        {
            int IdCambio = Convert.ToInt32(Request.Params["IdCambio"]);
            int cambioDetalle = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio && x.STATUS == true).Count();

            if (cambioDetalle == 1)
            {
                return Content("ERROR");
                //catoPlan.idTypeRequest = 8;
                //catoPlan.modifiedDate = DateTime.Now;
                //db.Entry(catoPlan).State = EntityState.Modified;
                //db.SaveChanges();
                ////Enviar correo a Usuario para informar que el plan se valido
                //if (correo == 0)
                //{
                //    ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                //    mail.sendMailGestionCambios(IdPersEnti, catoPlan, "V");
                //}
            }
            else
            {
                return Content("OK");
            }
        }

        [Authorize]
        public ActionResult EliminarActividadCambio()
        {
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            int IdDetalleCambio = Convert.ToInt32(Request.Params["IdDetalleCambio"]);
            int IdCambio = Convert.ToInt32(Request.Params["IdCambio"]);
            string Motivo = Convert.ToString(Request.Params["txtMotivo"]);

            //Registrar Actividad Realizada
            CHANGE_DETAIL CD = db.CHANGE_DETAIL.Where(x => x.ID == IdDetalleCambio).FirstOrDefault();
            CD.Motivo = Motivo;
            CD.FechaEliminacion = DateTime.Now;
            CD.STATUS = false;
            db.Entry(CD).State = EntityState.Modified;
            db.SaveChanges();


            CHANGE_APPROVED catoPlan = db.CHANGE_APPROVED.Where(x => x.idChangeRequest == @IdCambio).FirstOrDefault();
            int correo = 0;
            if (catoPlan.idTypeRequest == 8)
            {
                correo = 1;
            }


            int ultimaActividad = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == IdCambio
                                    && (x.ActividadValidada == null || x.ActividadValidada == false) && x.STATUS == true).Count();
            if (ultimaActividad == 0)
            {
                catoPlan.idTypeRequest = 8;
                catoPlan.modifiedDate = DateTime.Now;
                db.Entry(catoPlan).State = EntityState.Modified;
                db.SaveChanges();

                //Enviar correo a Usuario para informar que el plan se valido
                if (correo == 0)
                {
                    ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                    mail.sendMailGestionCambios(IdPersEnti, catoPlan, "V");
                }

            }


            return Content("OK");
        }

        //
        // POST: /ChangeRequest/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            //Variables
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            int ClasificacionCambio = Convert.ToInt32(Request.Params["cbClasificacionCambio"]);
            int TipoCambio = Convert.ToInt32(Request.Params["cbTipoCambio"]);
            int Prioridad = Convert.ToInt32(Request.Params["cbPrioridad"]);


            int contador = Convert.ToInt32(Request.Params["Contador"]);
            string ImpactoDescripcion = Convert.ToString(Request.Params["txtImpactoDescripcion"]);

            if (ClasificacionCambio == 0 || Prioridad == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
            }

            var tipoDescripcion = db.CHANGE_TYPE.Where(x => x.id == ClasificacionCambio && x.status == true).SingleOrDefault();
            string txtTipoDescripcion = "";
            if (tipoDescripcion.descripcion == true)
            {
                txtTipoDescripcion = Convert.ToString(Request.Params["txtTipoDescripcion"]);
                if (txtTipoDescripcion == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
                var TipoCambioUrgente = db.CHANGE_TYPE_DETAIL.Where(x => x.idChangeType == ClasificacionCambio && x.status == true).FirstOrDefault();
                TipoCambio = Convert.ToInt32(TipoCambioUrgente.id);
            }
            else
            {
                if (TipoCambio == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
            }

            //Preguntas
            string txtPregunta1 = Convert.ToString(Request.Params["txtPregunta1"]);
            string txtPregunta2 = Convert.ToString(Request.Params["txtPregunta2"]);
            string txtPregunta3 = Convert.ToString(Request.Params["txtPregunta3"]);
            string txtPregunta4 = Convert.ToString(Request.Params["txtPregunta4"]);

            if (txtPregunta1 == "" || txtPregunta2 == "" || txtPregunta3 == "" || txtPregunta4 == "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Por favor responda todas las preguntas','2');}window.onload=init;</script>");
            }

            //Impacto
            IQueryable<CHANGE_IMPACT> impact = from ci in db.CHANGE_IMPACT
                                               where ci.IdCuenta == IdCuenta && ci.status == true
                                               select ci;

            CHANGE_IMPACT[] impactArray = impact.ToArray();


            string chkImpact = "";
            foreach (CHANGE_IMPACT imp in impactArray)
            {
                if (Convert.ToString(collection[imp.detail]) != null)
                {
                    chkImpact = chkImpact + imp.id + ",";
                }

            }

            //Validación - Detalle del Cambio
            int Actividad = 0, Responsable = 0;
            string FechaInicio = "";
            string FechaFin = "";
            int flag = 0;
            for (int i = 0; i <= contador; i++)
            {
                if (Convert.ToString(collection["Descripcion" + i]) != null)
                {
                    flag = flag + 1;
                    Actividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                    Responsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                    FechaInicio = Convert.ToString(Request.Params["dtFechaInicio" + i]);
                    FechaFin = Convert.ToString(Request.Params["dtFechaFin" + i]);

                    if (Actividad == 0 || Responsable == 0 || FechaInicio == "" || FechaFin == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registre los datos del Detalle del Cambio.','2');}window.onload=init;</script>");
                    }

                }
            }
            if (flag == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registre el Detalle del Cambio.','2');}window.onload=init;</script>");
            }

            try
            {
                //Registro de Aprobación de Usuario
                var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).ToList();
                var ID_PERS_ENTI_APPR = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);

                var changeRequest = new CHANGE_REQUEST();
                changeRequest.idChangeType = ClasificacionCambio;
                changeRequest.idChangeTypeDetail = TipoCambio;
                changeRequest.isImpactCheck = chkImpact;
                changeRequest.otherImpact = ImpactoDescripcion;
                changeRequest.idPriority = Prioridad;
                changeRequest.urgentDetail = txtTipoDescripcion;
                changeRequest.otherChange = "";
                changeRequest.question1 = txtPregunta1;
                changeRequest.question2 = txtPregunta2;
                changeRequest.question3 = txtPregunta3;
                changeRequest.question4 = txtPregunta4;
                changeRequest.ID_PERS_ENTI = IdPersEnti;
                changeRequest.createDate = DateTime.Now;
                changeRequest.IdCuenta = IdCuenta;
                db.CHANGE_REQUEST.Add(changeRequest);
                db.SaveChanges();


                var approved = new CHANGE_APPROVED();
                approved.idChangeRequest = changeRequest.id;
                approved.idTypeRequest = 1;
                approved.approver = ID_PERS_ENTI_APPR;
                approved.createDate = DateTime.Now;
                approved.activeJustified = false;
                approved.activeToplan = 0;
                db.CHANGE_APPROVED.Add(approved);
                db.SaveChanges();
                #region Detalle de Solicitud

                var cambioDetalle = new CHANGE_DETAIL();

                int IdActividad = 0, IdResponsable = 0;
                string DescripcionActividad = "";

                for (int i = 0; i <= contador; i++)
                {
                    if (Convert.ToString(collection["Descripcion" + i]) != null)
                    {
                        IdActividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                        IdResponsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                        DescripcionActividad = Convert.ToString(Request.Params["Descripcion" + i]);

                        cambioDetalle.ID_TYPE_TASK = IdActividad;
                        cambioDetalle.ID_CHANGE_REQUEST = changeRequest.id;
                        cambioDetalle.ID_PERSON_ENTI = IdResponsable;
                        cambioDetalle.DETAIL = DescripcionActividad;
                        cambioDetalle.DATE_INIC = Convert.ToDateTime(Request.Params["dtFechaInicio" + i]);
                        cambioDetalle.DATE_END = Convert.ToDateTime(Request.Params["dtFechaFin" + i]);
                        cambioDetalle.STATUS = true;
                        cambioDetalle.FechaInicioProgramada = Convert.ToDateTime(Request.Params["dtFechaInicio" + i]);
                        cambioDetalle.FechaFinProgramada = Convert.ToDateTime(Request.Params["dtFechaFin" + i]);
                        db.CHANGE_DETAIL.Add(cambioDetalle);
                        db.SaveChanges();
                    }
                }

                #endregion


                ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                mail.sendMailGestionCambios(IdPersEnti, approved, "S");
                //Envio de correo a los involucrados de las actividades
                var query = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == changeRequest.id);
                foreach (var detail in query)
                {
                    mail.sendMailUsersRequest(detail, approved, 1);
                }
            }
            catch (Exception e)
            {
                string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MensajeConfirmacion) top.MensajeConfirmacion('No se registró la solicitud del cambio, error al enviar el correo. Contacte al administrador.','1');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('El Comité del SIG y/o tu Gerencia respectiva se comunicarán con Ud. para desarrollar el plan de acción sobre el cambio.','1');}window.onload=init;</script>");


        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuardarArchivo(IEnumerable<HttpPostedFileBase> files)
        {

            int adjunto = 0;
            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        var NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        var EXT_ATTA = Path.GetExtension(file.FileName);
                        var id = Convert.ToInt32(Request.Params["idGestion"]);
                        int userId = Convert.ToInt32(Session["UserId"]);
                        var adjunt = db.sp_InsertAttaChange(NAM_ATTA, EXT_ATTA, id, userId);
                        adjunto = Convert.ToInt32(adjunt.Single().Value);
                        file.SaveAs(Server.MapPath("~/Adjuntos/ChangeRequest/") + NAM_ATTA + "_" + adjunto.ToString() + EXT_ATTA);
                    }
                    catch
                    {

                    }
                }
            }
            //Se registro correctamente
            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone();}window.onload=init;</script>");

        }

        public ActionResult List(int idChangeType, int idTipoCambio, int idApproved, int idRequested, string keyword)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            using (var context = new CoreEntities())
            {
                int ID_PERS_ENTI = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
                bool listarAprobadosxJefes = false;
                ListarTodos = false;
                #region datos Usuario y Jefe

                //(ERP)
                int idEntiCuenta = 9;

                var qUserEdata = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == idEntiCuenta) //Listado de Personas
                                  join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                                  select new
                                  {
                                      ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      //ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      User = b.FIR_NAME + " " + b.LAS_NAME,
                                      Usua = b.FIR_NAME.Substring(0, 1) + ". " + b.LAS_NAME
                                  });


                int idJefe = (from a in context.CHANGE_APPROVED
                              join b in context.CHANGE_REQUEST on a.idChangeRequest equals b.id
                              where b.ID_PERS_ENTI == ID_PERS_ENTI
                              select a.approver).FirstOrDefault();

                var qJefeEdata = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == idEntiCuenta) //Listado de Personas
                                  join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                                  select new
                                  {
                                      ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      Jefe = b.FIR_NAME + " " + b.LAS_NAME,
                                      UsuarioJefe = b.FIR_NAME.Substring(0, 1) + ". " + b.LAS_NAME
                                  }).ToList();
                #endregion
                #region Permisos de Visualizacion de solicitudes
                int esJefe = (from a in context.CHANGE_APPROVED.Where(x => x.approver == ID_PERS_ENTI)
                              select a.approver).FirstOrDefault();
                if (esJefe > 0)
                {
                    listarAprobadosxJefes = true;
                }
                if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1)
                {
                    ListarTodos = true;
                }
                #endregion

                #region JefesAprobadores
                if (listarAprobadosxJefes && !ListarTodos)
                {
                    var result = (from ca in context.CHANGE_APPROVED
                                  join cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO) on ca.idChangeRequest equals cr.id
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  join cd in context.CHANGE_DETAIL on cr.id equals cd.ID_CHANGE_REQUEST
                                  join tk in context.TICKETs on cr.IdTicket equals tk.ID_TICK into x
                                  from tk in x.DefaultIfEmpty()
                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      //estado de solicitud
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      //colorApproved = (ca.idTypeRequest == 1 ? "#2b5797" : ca.idTypeRequest == 2 ? "#7cbb00" : ca.idTypeRequest == 3 ? "#b21e1e" : ca.idTypeRequest == 4 ? "#ffbb00" : ca.idTypeRequest == 5 ? "#990099" : ca.idTypeRequest == 6 ? "#0099C6" : ca.idTypeRequest == 7 ? "#7cb5ec" : ""),
                                      //LetraApproved = (ca.idTypeRequest == 1 ? "S" : ca.idTypeRequest == 2 ? "A" : ca.idTypeRequest == 3 ? "R" : ca.idTypeRequest == 4 ? "P" : ca.idTypeRequest == 5 ? "E" : ca.idTypeRequest == 6 ? "NE" : ca.idTypeRequest == 7 ? "NE" : ""),
                                      TypeApproved = ctr.type,
                                      detailChange = cr.question2 == null ? "" : cr.question2,
                                      idchangeApproved = ca.id,
                                      //detalle
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      detpriority = pr.detail,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden,
                                      IdResponsable = cd.ID_PERSON_ENTI
                                  }).Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.approved == ID_PERS_ENTI || x.IdResponsable == ID_PERS_ENTI).Distinct();

                  

                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (!String.IsNullOrEmpty(keyword))
                    {
                        result = result.Where(x => x.detailChange.ToLower().Contains(keyword.ToLower()));
                    }

                    int tt = result.Count();
                    int fil = tt - skip;

                    var queryFinal = (from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                      join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                      join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      select new
                                      {
                                          idSession = ID_PERS_ENTI,
                                          idChangeRequest = a.idChangeRequest,
                                          idChangeType = a.idChangeType,
                                          ChangeType = a.ChangeType,
                                          ChangeTypeDetail = a.ChangeTypeDetail,
                                          ID_PERS_ENTI = a.ID_PERS_ENTI,
                                          approved = a.approved,
                                          idApproved = a.idApproved,
                                          priority = a.priority,
                                          registro = a.registro,
                                          colorApproved = a.colorApproved,
                                          LetraApproved = a.LetraApproved,
                                          TypeApproved = a.TypeApproved,
                                          User = b.User,
                                          Jefe = c.Jefe,
                                          detailChange = a.detailChange == null ? "" : a.detailChange,
                                          DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                          MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                          DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                          TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                          SwApproval = ListarTodos,
                                          idchangeApproved = a.idchangeApproved,
                                          question1 = a.question1,
                                          question3 = a.question3,
                                          question4 = a.question4,
                                          question5 = a.question5,
                                          question6 = a.question6,
                                          question7 = a.question7,
                                          otherChange = a.otherChange,
                                          urgentDetail = a.urgentDetail,
                                          otherImpact = a.otherImpact,
                                          isImpactCheck = a.isImpactCheck,
                                          detpriority = a.detpriority,
                                          modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                          activeJustified = a.activeJustified,
                                          justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                          changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                          activeToplan = a.activeToplan,
                                          idJefe = a.idJefe,
                                          dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                          porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                      }).Distinct();
                    if (queryFinal.Count() >= 1)
                    {
                        idSession = Convert.ToInt32(queryFinal.ToList().First().idSession);
                        approved = Convert.ToInt32(queryFinal.ToList().First().approved);
                    }
                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                #endregion JefesAprobadores
                #region Administradores y SIG
                else if (ListarTodos)
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO).ToList()
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  //join tk in context.TICKETs on cr.IdTicket equals tk.ID_TICK into x
                                  //from tk in x.DefaultIfEmpty()
                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      TypeApproved = ctr.type,
                                      detailChange = cr.question2 == null ? "" : cr.question2,
                                      //detailChange = (cr.Activo == false ? cr.question2 : Convert.ToString(cr.question2.Substring(0, 61) + tk.COD_TICK)),
                                      idchangeApproved = ca.id,
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      detpriority = pr.detail,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden
                                  });

                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (!String.IsNullOrEmpty(keyword))
                    {
                        result = result.Where(x => x.detailChange.ToLower().Contains(keyword.ToLower()));
                    }

                    int tt = result.Count();
                    var queryFinal = from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                     join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                     join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                     select new
                                     {
                                         idSession = ID_PERS_ENTI,
                                         idChangeRequest = a.idChangeRequest,
                                         idChangeType = a.idChangeType,
                                         ChangeType = a.ChangeType,
                                         ChangeTypeDetail = a.ChangeTypeDetail,
                                         ID_PERS_ENTI = a.ID_PERS_ENTI,
                                         approved = a.approved,
                                         idApproved = a.idApproved,
                                         priority = a.priority,
                                         registro = a.registro,
                                         colorApproved = a.colorApproved,
                                         LetraApproved = a.LetraApproved,
                                         TypeApproved = a.TypeApproved,
                                         User = b.User,
                                         Jefe = c.Jefe,
                                         detailChange = a.detailChange,
                                         DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                         MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                         DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                         TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                         SwApproval = ListarTodos,
                                         idchangeApproved = a.idchangeApproved,
                                         question1 = a.question1,
                                         question3 = a.question3,
                                         question4 = a.question4,
                                         question5 = a.question5,
                                         question6 = a.question6,
                                         question7 = a.question7,
                                         otherChange = a.otherChange,
                                         urgentDetail = a.urgentDetail,
                                         otherImpact = a.otherImpact,
                                         isImpactCheck = a.isImpactCheck,
                                         detpriority = a.detpriority,
                                         modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                         activeJustified = a.activeJustified,
                                         justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                         changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                         activeToplan = a.activeToplan,
                                         idJefe = a.idJefe,
                                         dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                         porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                     };
                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region usuarios
                else
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO)
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  join cd in context.CHANGE_DETAIL on cr.id equals cd.ID_CHANGE_REQUEST
                                  join tk in context.TICKETs on cr.IdTicket equals tk.ID_TICK into x
                                  from tk in x.DefaultIfEmpty()

                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      detpriority = pr.detail,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      TypeApproved = ctr.type,
                                      detailChange = (cr.Activo == null ? cr.question2 : cr.question2.Substring(0, 61) + tk.COD_TICK),
                                      //detailChange = (cr.Activo == false ? cr.question2 : Convert.ToString(cr.question2.Split('<'))),
                                      idchangeApproved = ca.id,
                                      //detalle
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden,
                                      IdResponsable = cd.ID_PERSON_ENTI
                                  }).Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.IdResponsable == ID_PERS_ENTI).Distinct();

                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (!String.IsNullOrEmpty(keyword))
                    {
                        result = result.Where(x => x.detailChange.ToLower().Contains(keyword.ToLower()));
                    }
                    int tt = result.Count();
                    int fil = tt - skip;
                    var queryFinal = (from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                      join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                      join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      select new
                                      {
                                          idSession = ID_PERS_ENTI,
                                          idChangeRequest = a.idChangeRequest,
                                          idChangeType = a.idChangeType,
                                          ChangeType = a.ChangeType,
                                          ChangeTypeDetail = a.ChangeTypeDetail,
                                          ID_PERS_ENTI = a.ID_PERS_ENTI,
                                          approved = a.approved,
                                          idApproved = a.idApproved,
                                          priority = a.priority,
                                          registro = a.registro,
                                          colorApproved = a.colorApproved,
                                          LetraApproved = a.LetraApproved,
                                          TypeApproved = a.TypeApproved,
                                          User = b.User,
                                          Jefe = c.Jefe,
                                          detailChange = a.detailChange == null ? "" : a.detailChange,
                                          DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                          MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                          DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                          TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                          SwApproval = listarAprobadosxJefes,
                                          idchangeApproved = a.idchangeApproved,
                                          //
                                          question1 = a.question1,
                                          question3 = a.question3,
                                          question4 = a.question4,
                                          question5 = a.question5,
                                          question6 = a.question6,
                                          question7 = a.question7,
                                          otherChange = a.otherChange,
                                          urgentDetail = a.urgentDetail,
                                          otherImpact = a.otherImpact,
                                          isImpactCheck = a.isImpactCheck,
                                          detpriority = a.detpriority,
                                          modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                          activeJustified = a.activeJustified,
                                          justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                          changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                          activeToplan = a.activeToplan,
                                          idJefe = a.idJefe,
                                          dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                          porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                      }).Distinct();
                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
        }

        public ActionResult ListMINSUR(int idChangeType, int idTipoCambio, int idApproved, int idRequested, string keyword)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            using (var context = new CoreEntities())
            {
                int ID_PERS_ENTI = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
                bool listarAprobadosxJefes = false;
                ListarTodos = false;
                #region datos Usuario y Jefe

                //(ERP)
                int idEntiCuenta = 9;

                var qUserEdata = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 65966 || x.ID_ENTI1==9) //Listado de Personas
                                  join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                                  select new
                                  {
                                      ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      //ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      User = b.FIR_NAME + " " + b.LAS_NAME,
                                      Usua = b.FIR_NAME.Substring(0, 1) + ". " + b.LAS_NAME
                                  });


                int idJefe = (from a in context.CHANGE_APPROVED
                              join b in context.CHANGE_REQUEST on a.idChangeRequest equals b.id
                              where b.ID_PERS_ENTI == ID_PERS_ENTI
                              select a.approver).FirstOrDefault();

                var qJefeEdata = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 65966) //Listado de Personas
                                  join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                                  select new
                                  {
                                      ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      Jefe = b.FIR_NAME + " " + b.LAS_NAME,
                                      UsuarioJefe = b.FIR_NAME.Substring(0, 1) + ". " + b.LAS_NAME
                                  }).ToList();

                var qjf = qJefeEdata.ToList();
                #endregion
                #region Permisos de Visualizacion de solicitudes
                int esJefe = (from a in context.CHANGE_APPROVED.Where(x => x.approver == ID_PERS_ENTI)
                              select a.approver).FirstOrDefault();
                if (esJefe > 0)
                {
                    listarAprobadosxJefes = true;
                }
                if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SISTEMA_INTEGRADO_GESTION"] == 1)
                {
                    ListarTodos = true;
                }
                #endregion

                #region JefesAprobadores
                if (listarAprobadosxJefes && !ListarTodos)
                {
                    var result = (from ca in context.CHANGE_APPROVED
                                  join cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO) on ca.idChangeRequest equals cr.id
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  join cd in context.CHANGE_DETAIL on cr.id equals cd.ID_CHANGE_REQUEST
                                  join tk in context.TICKETs on cr.IdTicket equals tk.ID_TICK into x
                                  from tk in x.DefaultIfEmpty()
                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      //estado de solicitud
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      //colorApproved = (ca.idTypeRequest == 1 ? "#2b5797" : ca.idTypeRequest == 2 ? "#7cbb00" : ca.idTypeRequest == 3 ? "#b21e1e" : ca.idTypeRequest == 4 ? "#ffbb00" : ca.idTypeRequest == 5 ? "#990099" : ca.idTypeRequest == 6 ? "#0099C6" : ca.idTypeRequest == 7 ? "#7cb5ec" : ""),
                                      //LetraApproved = (ca.idTypeRequest == 1 ? "S" : ca.idTypeRequest == 2 ? "A" : ca.idTypeRequest == 3 ? "R" : ca.idTypeRequest == 4 ? "P" : ca.idTypeRequest == 5 ? "E" : ca.idTypeRequest == 6 ? "NE" : ca.idTypeRequest == 7 ? "NE" : ""),
                                      TypeApproved = ctr.type,
                                      detailChange = cr.question2 == null ? "" : cr.question2,
                                      idchangeApproved = ca.id,
                                      //detalle
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      detpriority = pr.detail,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden,
                                      IdResponsable = cd.ID_PERSON_ENTI
                                  }).Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.approved == ID_PERS_ENTI || x.IdResponsable == ID_PERS_ENTI).Distinct();



                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (!String.IsNullOrEmpty(keyword))
                    {
                        result = result.Where(x => x.detailChange.ToLower().Contains(keyword.ToLower()));
                    }

                    int tt = result.Count();
                    int fil = tt - skip;

                    var queryFinal = (from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                      join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                      join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      select new
                                      {
                                          idSession = ID_PERS_ENTI,
                                          idChangeRequest = a.idChangeRequest,
                                          idChangeType = a.idChangeType,
                                          ChangeType = a.ChangeType,
                                          ChangeTypeDetail = a.ChangeTypeDetail,
                                          ID_PERS_ENTI = a.ID_PERS_ENTI,
                                          approved = a.approved,
                                          idApproved = a.idApproved,
                                          priority = a.priority,
                                          registro = a.registro,
                                          colorApproved = a.colorApproved,
                                          LetraApproved = a.LetraApproved,
                                          TypeApproved = a.TypeApproved,
                                          User = b.User,
                                          Jefe = c.Jefe,
                                          detailChange = a.detailChange == null ? "" : a.detailChange,
                                          DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                          MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                          DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                          TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                          SwApproval = ListarTodos,
                                          idchangeApproved = a.idchangeApproved,
                                          question1 = a.question1,
                                          question3 = a.question3,
                                          question4 = a.question4,
                                          question5 = a.question5,
                                          question6 = a.question6,
                                          question7 = a.question7,
                                          otherChange = a.otherChange,
                                          urgentDetail = a.urgentDetail,
                                          otherImpact = a.otherImpact,
                                          isImpactCheck = a.isImpactCheck,
                                          detpriority = a.detpriority,
                                          modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                          activeJustified = a.activeJustified,
                                          justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                          changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                          activeToplan = a.activeToplan,
                                          idJefe = a.idJefe,
                                          dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                          porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                      }).Distinct();
                    if (queryFinal.Count() >= 1)
                    {
                        idSession = Convert.ToInt32(queryFinal.ToList().First().idSession);
                        approved = Convert.ToInt32(queryFinal.ToList().First().approved);
                    }
                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                #endregion JefesAprobadores
                #region Administradores y SIG
                else if (ListarTodos)
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO).ToList()
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  //join tk in context.TICKETs on cr.IdTicket equals tk.ID_TICK into x
                                  //from tk in x.DefaultIfEmpty()
                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      TypeApproved = ctr.type,
                                      detailChange = cr.question2 == null ? "" : cr.question2,
                                      //detailChange = (cr.Activo == false ? cr.question2 : Convert.ToString(cr.question2.Substring(0, 61) + tk.COD_TICK)),
                                      idchangeApproved = ca.id,
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      detpriority = pr.detail,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden
                                  });

                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (!String.IsNullOrEmpty(keyword))
                    {
                        result = result.Where(x => x.detailChange.ToLower().Contains(keyword.ToLower()));
                    }

                    int tt = result.Count();
                    var queryFinal = from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                     join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                     join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                     select new
                                     {
                                         idSession = ID_PERS_ENTI,
                                         idChangeRequest = a.idChangeRequest,
                                         idChangeType = a.idChangeType,
                                         ChangeType = a.ChangeType,
                                         ChangeTypeDetail = a.ChangeTypeDetail,
                                         ID_PERS_ENTI = a.ID_PERS_ENTI,
                                         approved = a.approved,
                                         idApproved = a.idApproved,
                                         priority = a.priority,
                                         registro = a.registro,
                                         colorApproved = a.colorApproved,
                                         LetraApproved = a.LetraApproved,
                                         TypeApproved = a.TypeApproved,
                                         User = b.User,
                                         Jefe = c.Jefe,
                                         detailChange = a.detailChange,
                                         DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                         MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                         DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                         TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                         SwApproval = ListarTodos,
                                         idchangeApproved = a.idchangeApproved,
                                         question1 = a.question1,
                                         question3 = a.question3,
                                         question4 = a.question4,
                                         question5 = a.question5,
                                         question6 = a.question6,
                                         question7 = a.question7,
                                         otherChange = a.otherChange,
                                         urgentDetail = a.urgentDetail,
                                         otherImpact = a.otherImpact,
                                         isImpactCheck = a.isImpactCheck,
                                         detpriority = a.detpriority,
                                         modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                         activeJustified = a.activeJustified,
                                         justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                         changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                         activeToplan = a.activeToplan,
                                         idJefe = a.idJefe,
                                         dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                         porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                     };
                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region usuarios
                else
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO)
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  join cd in context.CHANGE_DETAIL on cr.id equals cd.ID_CHANGE_REQUEST
                                  join tk in context.TICKETs on cr.IdTicket equals tk.ID_TICK into x
                                  from tk in x.DefaultIfEmpty()

                                  select new
                                  {
                                      idSession = ID_PERS_ENTI,
                                      idChangeRequest = cr.id,
                                      idChangeType = cr.idChangeType,
                                      idChangeTypeDetail = cr.idChangeTypeDetail,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      approved = ca.approver,
                                      idApproved = ca.idTypeRequest,
                                      priority = pr.nombre,
                                      detpriority = pr.detail,
                                      registro = cr.createDate,
                                      colorApproved = ctr.Color,
                                      LetraApproved = ctr.Abreviatura,
                                      TypeApproved = ctr.type,
                                      detailChange = (cr.Activo == null ? cr.question2 : cr.question2.Substring(0, 61) + tk.COD_TICK),
                                      //detailChange = (cr.Activo == false ? cr.question2 : Convert.ToString(cr.question2.Split('<'))),
                                      idchangeApproved = ca.id,
                                      //detalle
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      question3 = cr.question3 == null ? "" : cr.question3,
                                      question4 = cr.question4 == null ? "" : cr.question4,
                                      question5 = cr.question5 == null ? "" : cr.question5,
                                      question6 = cr.question6 == null ? "" : cr.question6,
                                      question7 = cr.question7 == null ? "" : cr.question7,
                                      otherChange = cr.otherChange == null ? "" : cr.otherChange,
                                      urgentDetail = cr.urgentDetail == null ? "" : cr.urgentDetail,
                                      otherImpact = cr.otherImpact == null ? "" : cr.otherImpact,
                                      isImpactCheck = cr.isImpactCheck == null ? "" : cr.isImpactCheck,
                                      modifiedDate = ca.modifiedDate,
                                      activeJustified = ca.activeJustified,
                                      justificationChange = ca.justificationChange,
                                      changeDate = ca.changeDate,
                                      activeToplan = ca.activeToplan,
                                      idJefe = ca.approvedby == null ? 0 : ca.approvedby,
                                      dateActiveToplan = ca.dateActiveToplan,
                                      orden = ctr.Orden,
                                      IdResponsable = cd.ID_PERSON_ENTI
                                  }).Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.IdResponsable == ID_PERS_ENTI).Distinct();

                    if (idChangeType > 0)
                    {
                        result = result.Where(x => x.idChangeType == idChangeType);
                    }
                    if (idTipoCambio > 0)
                    {
                        result = result.Where(x => x.idChangeTypeDetail == idTipoCambio);
                    }
                    if (idRequested > 0)
                    {
                        result = result.Where(x => x.ID_PERS_ENTI == idRequested);
                    }
                    if (idApproved > 0)
                    {
                        result = result.Where(x => x.idApproved == idApproved);
                    }
                    if (!String.IsNullOrEmpty(keyword))
                    {
                        result = result.Where(x => x.detailChange.ToLower().Contains(keyword.ToLower()));
                    }
                    int tt = result.Count();
                    int fil = tt - skip;
                    var queryFinal = (from a in result.ToList().OrderBy(x => x.orden)
                                   .ThenByDescending(x => x.registro).Skip(skip).Take(take)
                                      join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                      join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      select new
                                      {
                                          idSession = ID_PERS_ENTI,
                                          idChangeRequest = a.idChangeRequest,
                                          idChangeType = a.idChangeType,
                                          ChangeType = a.ChangeType,
                                          ChangeTypeDetail = a.ChangeTypeDetail,
                                          ID_PERS_ENTI = a.ID_PERS_ENTI,
                                          approved = a.approved,
                                          idApproved = a.idApproved,
                                          priority = a.priority,
                                          registro = a.registro,
                                          colorApproved = a.colorApproved,
                                          LetraApproved = a.LetraApproved,
                                          TypeApproved = a.TypeApproved,
                                          User = b.User,
                                          Jefe = c.Jefe,
                                          detailChange = a.detailChange == null ? "" : a.detailChange,
                                          DAY_NAME = (a.registro == null ? "" : String.Format("{0:dddd}", a.registro).ToLower()),
                                          MONTH_YEAR = (a.registro == null ? "" : String.Format("{0:MMM dd yyyy}", a.registro).ToLower()),
                                          DAT_LONG = (a.registro == null ? "" : String.Format("{0:dddd MMM dd yyyy}", a.registro).ToLower()),
                                          TIME = (a.registro == null ? "" : String.Format("{0:hh:mm tt}", a.registro).ToUpper()),
                                          SwApproval = listarAprobadosxJefes,
                                          idchangeApproved = a.idchangeApproved,
                                          //
                                          question1 = a.question1,
                                          question3 = a.question3,
                                          question4 = a.question4,
                                          question5 = a.question5,
                                          question6 = a.question6,
                                          question7 = a.question7,
                                          otherChange = a.otherChange,
                                          urgentDetail = a.urgentDetail,
                                          otherImpact = a.otherImpact,
                                          isImpactCheck = a.isImpactCheck,
                                          detpriority = a.detpriority,
                                          modifiedDate = (a.modifiedDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.modifiedDate).ToLower()),
                                          activeJustified = a.activeJustified,
                                          justificationChange = a.justificationChange == null ? "" : a.justificationChange,
                                          changeDate = a.changeDate == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.changeDate),
                                          activeToplan = a.activeToplan,
                                          idJefe = a.idJefe,
                                          dateActiveToplan = a.dateActiveToplan == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", a.dateActiveToplan),
                                          porcentajeAvance = db.GestionCambioPorcentajeAvance(a.idChangeRequest).SingleOrDefault().PorcentajeAvance
                                      }).Distinct();
                    var ass = queryFinal.ToList();

                    return Json(new { Data = queryFinal.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
        }


        public ActionResult GestionCambiosDetalle(int id)
        {
            var query = db.GestionCambiosDetalle(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public ActionResult RejectApproved(int id = 0)
        {
            try
            {
                int ID_CUENTA= Convert.ToInt32(Convert.ToString(Session["ID_ACCO"]));
                using (var context = new CoreEntities())
                {
                    int ID_PERS_ENTI = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
                    CHANGE_APPROVED ca = context.CHANGE_APPROVED.Find(id);
                    CHANGE_REQUEST req = context.CHANGE_REQUEST.Find(ca.idChangeRequest);
                    
                    if (ID_CUENTA==60)
                    {
                        
                            int idRequest = Convert.ToInt32(context.CHANGE_APPROVED.Single(x => x.id == id).idChangeRequest);
                        db.GestionCambiosAnularBNV(idRequest, ID_PERS_ENTI);
                       // db.GestionCambioRechazado(idRequest, ID_PERS_ENTI);
                            ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                            int idRequester = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == idRequest).ID_PERS_ENTI);

                            smail.sendMailGestionCambiosBNV(idRequester, ca, "AN");
                      
                    }
                    else
                    {
                        if (ca.idTypeRequest == 8)
                        {
                            int idRequest = Convert.ToInt32(context.CHANGE_APPROVED.Single(x => x.id == id).idChangeRequest);
                            //ca.idTypeRequest = 3;
                            //ca.modifiedDate = DateTime.Now;
                            //ca.approvedby = ID_PERS_ENTI;
                            //context.Entry(ca).State = EntityState.Modified;
                            //context.SaveChanges();

                            if (req.Activo == true)
                            {
                                var tic = context.TICKETs.Single(x => x.ID_TICK_PARENT == req.IdTicket && x.IS_PARENT == false);
                                int IdTicket = tic.ID_TICK;
                                //Agregar el comentario al ticket
                                var detailsTicket = new DETAILS_TICKET();
                                detailsTicket.ID_TICK = IdTicket;
                                detailsTicket.ID_TYPE_DETA_TICK = 3;
                                detailsTicket.COM_DETA_TICK = "Se ha rechazado el cambio y se procede a cerrar el ticket.";
                                detailsTicket.UserId = 34;
                                detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                                detailsTicket.MINUTES = 0;
                                detailsTicket.ID_STAT = 4;
                                db.DETAILS_TICKET.Add(detailsTicket);
                                db.SaveChanges();
                                //Se cierra el ticket requerimiento
                                TICKET ticket = context.TICKETs.Find(IdTicket);
                                ticket.ID_STAT = 6;
                                ticket.ID_STAT_END = 6;
                                context.Entry(ticket).State = EntityState.Modified;
                                context.SaveChanges();

                                var detailsTicket2 = new DETAILS_TICKET();
                                detailsTicket2.ID_TICK = req.IdTicket;
                                detailsTicket2.ID_TYPE_DETA_TICK = 1;
                                detailsTicket2.COM_DETA_TICK = "Se ha rechazado la solicitud de cambio relacionado al ticket.";
                                detailsTicket2.UserId = 34;
                                detailsTicket2.CREATE_DETA_INCI = DateTime.Now;
                                detailsTicket2.MINUTES = 0;
                                detailsTicket2.ID_STAT = 4;
                                db.DETAILS_TICKET.Add(detailsTicket2);
                                db.SaveChanges();
                            }

                            db.GestionCambioRechazado(idRequest, ID_PERS_ENTI);
                            ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                            int idRequester = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == idRequest).ID_PERS_ENTI);
                            if (ID_CUENTA == 56)
                            {
                                smail.sendMailGestionCambios(idRequester, ca, "RM");

                            }
                            else {
                                smail.sendMailGestionCambios(idRequester, ca, "R");

                            }


                            //Comentario de Ticket
                            int IdTicketC = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == idRequest).IdTicket);
                        if (IdTicketC != 0)
                        {
                            var detailsTicket = new DETAILS_TICKET();
                            detailsTicket.ID_TICK = IdTicketC;
                            detailsTicket.ID_TYPE_DETA_TICK = 1;
                            detailsTicket.COM_DETA_TICK = "La solicitud del cambio fue rechazada.";
                            detailsTicket.UserId = 34;
                            detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                            detailsTicket.MINUTES = 0;
                            db.DETAILS_TICKET.Add(detailsTicket);
                            db.SaveChanges();
                        }
                    }
                        else
                        {
                            return Content("Advertencia");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR");
            }
            return Content("OK");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectApprovedBNV(int id = 0)
        {
            try
            {
                db.GestionCambiosCancelarBNV(id);
            }
            catch
            {
                return Content("ERROR");
            }
            return Content("OK");
        }

        public ActionResult UpdateActivityToApproved(int id)
        {
            try
            {
                using (var context = new CoreEntities())
                {
                    int ID_PERS_ENTI = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));

                    CHANGE_REQUEST changeRequest = context.CHANGE_REQUEST.Find(id);
                    var idChange_Approved = context.CHANGE_APPROVED.Where(x => x.idChangeRequest == id).FirstOrDefault().id;

                    CHANGE_APPROVED ca = context.CHANGE_APPROVED.Find(idChange_Approved);
                    ca.idTypeRequest = 1;
                    ca.modifiedDate = DateTime.Now;
                    context.Entry(ca).State = EntityState.Modified;
                    context.SaveChanges();


                    ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                    mail.sendMailGestionCambios(ID_PERS_ENTI, ca, "S");

                    //Envio de correo a los involucrados de las actividades
                    var query = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == changeRequest.id && x.STATUS == true);
                    foreach (var detail in query)
                    {
                        mail.sendMailUsersRequest(detail, ca, 1);
                    }

                    //Comentario de Ticket
                    int IdTicket = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == changeRequest.id).IdTicket);
                    if (IdTicket != 0)
                    {
                        var detailsTicket = new DETAILS_TICKET();
                        detailsTicket.ID_TICK = IdTicket;
                        detailsTicket.ID_TYPE_DETA_TICK = 1;
                        detailsTicket.COM_DETA_TICK = "Se registra una solicitud de cambio.";
                        detailsTicket.UserId = 34;
                        detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                        detailsTicket.MINUTES = 0;
                        db.DETAILS_TICKET.Add(detailsTicket);
                        db.SaveChanges();
                    }

                }
            }
            catch
            {
                return Content("ERROR");
            }
            return Content("OK");
        }

		
		public ActionResult Approved(int id = 0)
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                using (var context = new CoreEntities())
                {
                    int ID_PERS_ENTI = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
                    CHANGE_APPROVED ca = context.CHANGE_APPROVED.Find(id);
                    CHANGE_REQUEST req = context.CHANGE_REQUEST.Find(ca.idChangeRequest);
                    if (ca.idTypeRequest == 8)
                    {
                        ca.idTypeRequest = 2;
                        ca.modifiedDate = DateTime.Now;
                        ca.approvedby = ID_PERS_ENTI;
                        context.Entry(ca).State = EntityState.Modified;
                        context.SaveChanges();
                        //Si es un cambio asociado a activos o perfil de activos, 
                        //se cierra el ticket requerimiento, adicionando un comentario
                        if (req.Activo == true)
                        {
                            var tic = context.TICKETs.Single(x => x.ID_TICK_PARENT == req.IdTicket && x.IS_PARENT == false);
                            int IdTicket = tic.ID_TICK;
                            //Agregar el comentario al ticket
                            var detailsTicket = new DETAILS_TICKET();
                            detailsTicket.ID_TICK = IdTicket;
                            detailsTicket.ID_TYPE_DETA_TICK = 3;
                            detailsTicket.COM_DETA_TICK = "Se ha aprobado el cambio y se procede a cerrar el ticket.";
                            detailsTicket.UserId = 34;
                            detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                            detailsTicket.MINUTES = 0;
                            detailsTicket.ID_STAT = 4;
                            db.DETAILS_TICKET.Add(detailsTicket);
                            db.SaveChanges();
                            //Se cierra el ticket requerimiento
                            TICKET ticket = context.TICKETs.Find(IdTicket);
                            ticket.ID_STAT = 6;
                            ticket.ID_STAT_END = 6;
                            context.Entry(ticket).State = EntityState.Modified;
                            context.SaveChanges();

                            var detailsTicket2 = new DETAILS_TICKET();
                            detailsTicket2.ID_TICK = req.IdTicket;
                            detailsTicket2.ID_TYPE_DETA_TICK = 1;
                            detailsTicket2.COM_DETA_TICK = "Se ha aprobado la solicitud de cambio relacionado al ticket.";
                            detailsTicket2.UserId = 34;
                            detailsTicket2.CREATE_DETA_INCI = DateTime.Now;
                            detailsTicket2.MINUTES = 0;
                            detailsTicket2.ID_STAT = 4;
                            db.DETAILS_TICKET.Add(detailsTicket2);
                            db.SaveChanges();
                        }
                        //envio de correo al Usuario
                        ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                        int idRequest = Convert.ToInt32(context.CHANGE_APPROVED.Single(x => x.id == id).idChangeRequest);
                        int idRequester = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == idRequest).ID_PERS_ENTI);

                        if (ID_ACCO == 60)
                        {
                            //idRequester = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == idRequest).ID_PERS_ENTI_ASSI);
                            var idRequesterBNV = (from cr in db.CHANGE_REQUEST.Where(x => x.id == idRequest)
                                                  join t in db.TICKETs on cr.IdTicket equals t.ID_TICK
                                                  select new
                                                  {
                                                      t.ID_PERS_ENTI
                                                  }).FirstOrDefault();
                            smail.sendMailGestionCambiosBNV(Convert.ToInt32(idRequesterBNV.ID_PERS_ENTI), ca, "A");

                        }
                        else if (ID_ACCO == 56)
                        {
                            //idRequester = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == idRequest).ID_PERS_ENTI_ASSI);
                            var idEjecutorMNS = (from cr in db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == idRequest)
                                                select new
                                                  {
                                                      cr.ID_PERSON_ENTI
                                                  }).FirstOrDefault();
                            smail.sendMailGestionCambios(Convert.ToInt32(idEjecutorMNS.ID_PERSON_ENTI), ca, "O");

                        }
                        else
                        {
                            smail.sendMailGestionCambios(idRequester, ca, "A");
                        }

                        //Envio de correo a los involucrados de las actividades
                        var query = context.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == ca.idChangeRequest && x.STATUS == true);

                        if (ID_ACCO == 60)
                        {
                            foreach (var detail in query)
                            {
                                smail.sendMailUsersRequestBNV(detail, ca, 2);
                            }
                        }
                        else if (ID_ACCO==4)
                        {
                            foreach (var detail in query)
                            {
                                smail.sendMailUsersRequest(detail, ca, 2);
                            }
                        }

                        //Ticket
                        int IdTicketCambio = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == idRequest).IdTicket);
                        if (IdTicketCambio != 0)
                        {
                            var detailsTicket = new DETAILS_TICKET();
                            detailsTicket.ID_TICK = IdTicketCambio;
                            detailsTicket.ID_TYPE_DETA_TICK = 1;
                            detailsTicket.COM_DETA_TICK = "La solicitud del cambio fue aprobada.";
                            detailsTicket.UserId = 34;
                            detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                            detailsTicket.MINUTES = 0;
                            db.DETAILS_TICKET.Add(detailsTicket);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        return Content("Advertencia");
                    }
                }
            }
            catch
            {
                return Content("ERROR");
            }
            return Content("OK");
        }
        public ActionResult toPlanRequest()
        {
            try
            {
                using (var context = new CoreEntities())
                {
                    int id = Convert.ToInt32(Request.Params["id"]);
                    int idChangeRequest = Convert.ToInt32(Request.Params["statusPlanAccion"]);

                    CHANGE_APPROVED catoPlan = context.CHANGE_APPROVED.Find(id);
                    if (catoPlan.idTypeRequest == 2)
                    {
                        //ac.justificationChange = justificacion;                          
                        catoPlan.activeToplan = 2;
                        catoPlan.idTypeRequest = idChangeRequest;
                        catoPlan.dateActiveToplan = DateTime.Now;
                        context.Entry(catoPlan).State = EntityState.Modified;
                        context.SaveChanges();
                        //Enviar correo a Usuario para informar que el plan se ejecuto
                        int idRequest = Convert.ToInt32(context.CHANGE_APPROVED.Single(x => x.id == id).idChangeRequest);
                        int idRequester = Convert.ToInt32(context.CHANGE_REQUEST.Single(x => x.id == idRequest).ID_PERS_ENTI);
                        //ERPElectrodata.Objects.SendMail smail = new ERPElectrodata.Objects.SendMail();
                        //smail.sendMailtoPlanRequest(idRequester,catoPlan);
                    }
                    else
                    {
                        return Content("Advertencia");
                    }
                }
            }
            catch
            {
                return Content("ERROR");
            }
            return Content("OK");
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult AutorizarCambio(IEnumerable<HttpPostedFileBase> filesInvoice, CHANGE_APPROVED ca)
        {
            try
            {
                using (var context = new CoreEntities())
                {
                    int sw = 0;
                    int id = 0;
                    int ID_PERS_ENTI = 0;
                    int ID_CHANGE_APPROV = 0;
                    string Error = string.Empty;

                    if (Int32.TryParse(Convert.ToString(Session["ID_PERS_ENTI"]), out ID_PERS_ENTI) == false) { sw = 1; Error = "Error al cargar usuario, actualizar la pagina."; }
                    else if (Int32.TryParse((Request.Form["HF_ID_CHANGE_APPROV"]), out ID_CHANGE_APPROV) == false) { sw = 1; Error = "Error al cargar ID_CHANGE_APPROV, actualizar pagina."; }
                    else if (filesInvoice == null) { sw = 1; Error = "No cargo archivo correctamente, por favor volver a intentarlo"; }
                    if (sw == 1)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                              "if(top.uploadDoneNR) top.uploadDoneMsn('ERROR','" + Error + "');}window.onload=init;</script>");
                    }
                    CHANGE_APPROVED ac = context.CHANGE_APPROVED.Find(ID_CHANGE_APPROV);
                    if (ac.idTypeRequest == 2 && ac.activeJustified == false)
                    {
                        ac.justificationChange = ca.justificationChange;
                        ac.changeDate = ca.changeDate;
                        ac.activeJustified = true;
                        ac.activeToplan = 1;
                        ac.dateActiveJustified = DateTime.Now;
                        context.Entry(ac).State = EntityState.Modified;
                        context.SaveChanges();
                        //Enviar correo a Jefes

                        var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(ID_PERS_ENTI).ToList();
                        var ID_PERS_ENTI_APPR = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);
                        string CorreoJefe = JefeInmediato.First().EMA_ELEC;
                        ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                        mail.sendMailSummaryChange(ID_PERS_ENTI, ac, CorreoJefe);

                        foreach (var file in filesInvoice)
                        {
                            if (file != null)
                            {
                                try
                                {
                                    var ATTA = new CHANGE_DETAIL_PLAN_ATTACH();

                                    ATTA.ID_CHANGE_APPROVED = ID_CHANGE_APPROV;
                                    ATTA.NAM_ATTA = "Informe_Plan_Accion";
                                    ATTA.EXT_ATTA = System.IO.Path.GetExtension(file.FileName);
                                    ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                                    ATTA.CREATE_ATTA = DateTime.Now;
                                    context.CHANGE_DETAIL_PLAN_ATTACH.Add(ATTA);
                                    context.SaveChanges();

                                    file.SaveAs(Server.MapPath("~/Adjuntos/ChangeRequest/ToPlan/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_CHANGE_APPROVED) + System.IO.Path.GetExtension(file.FileName)));
                                }
                                catch (Exception ex)
                                {
                                    return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDoneMsn) top.uploadDoneMsn('ERROR','Error al cargar archivo, detalle de error: " + ex.Message + "')');}window.onload=init;</script>");
                                }
                            }
                        }
                    }
                    else
                    {
                        return Content("<script type='text/javascript'> function init() {" +
               "if(top.uploadDoneMsn) top.uploadDoneMsn('Error','Ya se registro el Resumen del cambio.');}window.onload=init;</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'> function init() {" +
               "if(top.uploadDoneMsn) top.uploadDoneMsn('Error','Error al registrar el resumen del cambio, detalle de error: " + ex.Message + "');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
               "if(top.uploadDoneMsn) top.uploadDoneMsn('OK toPlan','Se ha registrado el resumen del cambio.');}window.onload=init;</script>");
        }
        public ActionResult GuardarGestionActivo(FormCollection collection)
        {

            //Variables
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
            int ClasificacionCambio = Convert.ToInt32(Request.Params["cbClasificacionCambio"]);
            int IdTicket = Convert.ToInt32(Request.Params["IdTicket"]);
            int TipoCambio = Convert.ToInt32(Request.Params["cbTipoCambio"]);
            int Prioridad = Convert.ToInt32(Request.Params["cbPrioridad"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            var ticketReq = new TICKET();

            int contador = Convert.ToInt32(Request.Params["Contador"]);
            string ImpactoDescripcion = Convert.ToString(Request.Params["txtImpactoDescripcion"]);

            if (ClasificacionCambio == 0 || Prioridad == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
            }

            var tipoDescripcion = db.CHANGE_TYPE.Where(x => x.id == ClasificacionCambio && x.status == true).SingleOrDefault();
            string txtTipoDescripcion = "";
            if (tipoDescripcion.descripcion == true)
            {
                txtTipoDescripcion = Convert.ToString(Request.Params["txtTipoDescripcion"]);
                if (txtTipoDescripcion == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
                var TipoCambioUrgente = db.CHANGE_TYPE_DETAIL.Where(x => x.idChangeType == ClasificacionCambio && x.status == true).FirstOrDefault();
                TipoCambio = Convert.ToInt32(TipoCambioUrgente.id);
            }
            else
            {
                if (TipoCambio == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
            }

            //Preguntas
            //string txtPregunta1 = Convert.ToString(Request.Params["txtPregunta1"]);
            //string txtPregunta2 = Convert.ToString(Request.Params["txtPregunta2"]);
            //string txtPregunta3 = Convert.ToString(Request.Params["txtPregunta3"]);
            //string txtPregunta4 = Convert.ToString(Request.Params["txtPregunta4"]);

            //if (txtPregunta1 == "" || txtPregunta2 == "" || txtPregunta3 == "" || txtPregunta4 == "")
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //             "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Por favor responda todas las preguntas','2');}window.onload=init;</script>");
            //}

            //Impacto
            IQueryable<CHANGE_IMPACT> impact = from ci in db.CHANGE_IMPACT
                                               where ci.IdCuenta == IdCuenta && ci.status == true
                                               select ci;

            CHANGE_IMPACT[] impactArray = impact.ToArray();


            string chkImpact = "";
            foreach (CHANGE_IMPACT imp in impactArray)
            {
                if (Convert.ToString(collection[imp.detail]) != null)
                {
                    chkImpact = chkImpact + imp.id + ",";
                }

            }

            //Validación - Detalle del Cambio
            int Actividad = 0, Responsable = 0;
            string FechaInicio = "";
            string FechaFin = "";
            int flag = 0;
            for (int i = 0; i <= contador; i++)
            {
                if (Convert.ToString(collection["Descripcion" + i]) != null)
                {
                    flag = flag + 1;
                    Actividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                    Responsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                    FechaInicio = Convert.ToString(Request.Params["dtFechaInicio" + i]);
                    FechaFin = Convert.ToString(Request.Params["dtFechaFin" + i]);

                    if (Actividad == 0 || Responsable == 0 || FechaInicio == "" || FechaFin == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registre los datos del Detalle del Cambio.','2');}window.onload=init;</script>");
                    }

                }
            }
            if (flag == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Registre el Detalle del Cambio.','2');}window.onload=init;</script>");
            }

            try
            {
                //(ERP)
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                //Registro de Aprobación de Usuario
                var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).ToList();
                var ID_PERS_ENTI_APPR = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);

                var ticketInc = db.TICKETs.Single(x => x.ID_TICK == IdTicket);
                // Se cierra el ticket incidente
                ticketInc.ID_STAT_END = 6;
                ticketInc.IS_PARENT = true;
                db.Entry(ticketInc).State = EntityState.Modified;
                db.SaveChanges();

                //Datos del ticket incidente
                //Una vez hecha la solicitud del cambio, se Genera el ticket requerimiento
                ticketReq.ID_ACCO = IdCuenta;
                ticketReq.ID_TYPE_TICK = 2;
                ticketReq.ID_AREA = ticketInc.ID_AREA;
                ticketReq.ID_QUEU = ticketInc.ID_QUEU;
                ticketReq.ID_PRIO = 5;
                ticketReq.ID_STAT = 1;
                ticketReq.ID_STAT_END = 1;
                ticketReq.ID_SOUR = 4;
                ticketReq.FEC_TICK = DateTime.Now;
                ticketReq.FEC_INI_TICK = DateTime.Now;
                ticketReq.SUM_TICK = "Se registró una solicitud de cambios, con referencia al ticket <a href='/DetailsTicket/Index/" + ticketInc.ID_TICK + "' target='_blank'>" + ticketInc.COD_TICK + "</a>";
                ticketReq.UserId = UserId;
                ticketReq.CREATE_TICK = DateTime.Now;
                ticketReq.MODIFIED_TICK = DateTime.Now;
                ticketReq.ID_TICK_PARENT = IdTicket;
                ticketReq.IS_PARENT = false;
                ticketReq.ID_COMP = ticketInc.ID_COMP;
                ticketReq.ID_COMP_END = ticketInc.ID_COMP_END;
                ticketReq.ID_DOCU_SALE = ticketInc.ID_DOCU_SALE;
                ticketReq.ID_DETA_DOCU_SALE = ticketInc.ID_DETA_DOCU_SALE;
                ticketReq.ID_CATE = ticketInc.ID_CATE;
                ticketReq.ID_PERS_ENTI = ticketInc.ID_PERS_ENTI;
                ticketReq.ID_PERS_ENTI_END = ticketInc.ID_PERS_ENTI_END;
                ticketReq.ID_PERS_ENTI_ASSI = ticketInc.ID_PERS_ENTI_ASSI;
                ticketReq.ID_PERS_ENTI_ASSI_STAR = ticketInc.ID_PERS_ENTI_ASSI_STAR;
                ticketReq.ID_ASSE = ticketInc.ID_ASSE;
                ticketReq.ID_TYPE_FORM = ticketInc.ID_TYPE_FORM;
                ticketReq.ID_LOCA = ticketInc.ID_LOCA;
                ticketReq.DAT_EXPI_TICK = DateTime.Now;
                ticketReq.SEND_MAIL = false;
                db.TICKETs.Add(ticketReq);
                db.SaveChanges();

                var changeRequest = new CHANGE_REQUEST();
                changeRequest.idChangeType = ClasificacionCambio;
                changeRequest.idChangeTypeDetail = TipoCambio;
                changeRequest.isImpactCheck = chkImpact;
                changeRequest.otherImpact = ImpactoDescripcion;
                changeRequest.idPriority = Prioridad;
                changeRequest.urgentDetail = txtTipoDescripcion;
                changeRequest.otherChange = "";
                //changeRequest.question1 = txtPregunta1;
                changeRequest.question2 = "Se registra una solicitud de cambio con referencia al ticket " + ticketInc.COD_TICK;
                //changeRequest.question3 = txtPregunta3;
                //changeRequest.question4 = txtPregunta4;
                changeRequest.IdTicketReq = ticketReq.ID_TICK;
                changeRequest.ID_PERS_ENTI = IdPersEnti;
                changeRequest.createDate = DateTime.Now;
                changeRequest.IdCuenta = IdCuenta;
                changeRequest.IdTicket = IdTicket;
                changeRequest.Activo = true;
                db.CHANGE_REQUEST.Add(changeRequest);
                db.SaveChanges();


                var approved = new CHANGE_APPROVED();
                approved.idChangeRequest = changeRequest.id;
                approved.idTypeRequest = 1;
                approved.approver = ID_PERS_ENTI_APPR;
                approved.createDate = DateTime.Now;
                approved.activeJustified = false;
                approved.activeToplan = 0;
                approved.VisNoti = true;
                db.CHANGE_APPROVED.Add(approved);
                db.SaveChanges();
                #region Detalle de Solicitud

                var cambioDetalle = new CHANGE_DETAIL();

                int IdActividad = 0, IdResponsable = 0;
                string DescripcionActividad = "";

                for (int i = 0; i <= contador; i++)
                {
                    if (Convert.ToString(collection["Descripcion" + i]) != null)
                    {
                        IdActividad = Convert.ToInt32(Request.Params["cbActividad" + i]);
                        IdResponsable = Convert.ToInt32(Request.Params["cbResponsable" + i]);
                        DescripcionActividad = Convert.ToString(Request.Params["Descripcion" + i]);

                        cambioDetalle.ID_TYPE_TASK = IdActividad;
                        cambioDetalle.ID_CHANGE_REQUEST = changeRequest.id;
                        cambioDetalle.ID_PERSON_ENTI = IdResponsable;
                        cambioDetalle.DETAIL = DescripcionActividad;
                        cambioDetalle.DATE_INIC = Convert.ToDateTime(Request.Params["dtFechaInicio" + i]);
                        cambioDetalle.DATE_END = Convert.ToDateTime(Request.Params["dtFechaFin" + i]);
                        cambioDetalle.STATUS = true;
                        cambioDetalle.VisNoti = true;
                        cambioDetalle.FechaInicioProgramada = Convert.ToDateTime(Request.Params["dtFechaInicio" + i]);
                        cambioDetalle.FechaFinProgramada = Convert.ToDateTime(Request.Params["dtFechaFin" + i]);
                        db.CHANGE_DETAIL.Add(cambioDetalle);
                        db.SaveChanges();
                    }
                }

                #endregion

                var detailsTicket = new DETAILS_TICKET();
                detailsTicket.ID_TICK = IdTicket;
                detailsTicket.ID_TYPE_DETA_TICK = 1;
                detailsTicket.COM_DETA_TICK = "Se registra una Solicitud de Cambios, y se genera el <a href='/DetailsTicket/Index/" + ticketReq.ID_TICK + "' target='_blank'>ticket requerimiento.</a>";
                detailsTicket.UserId = 34;
                detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                detailsTicket.MINUTES = 0;
                db.DETAILS_TICKET.Add(detailsTicket);
                db.SaveChanges();


                ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                mail.sendMailGestionCambios(IdPersEnti, approved, "S");
                //Envio de correo a los involucrados de las actividades
                var query = db.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == changeRequest.id);
                if (ID_ACCO > 59 && ID_ACCO < 73)
                {
                    foreach (var detail in query)
                    {
                        mail.sendMailUsersRequestBNV(detail, approved, 1);
                    }
                }
                else
                {
                    foreach (var detail in query)
                    {
                        mail.sendMailUsersRequest(detail, approved, 1);
                    }
                }

            }
            catch (Exception e)
            {
                string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MensajeConfirmacion) top.MensajeConfirmacion('No se registró la solicitud del cambio, error al enviar el correo. Contacte al administrador.','1',0);}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('El Comité del SIG y/o tu Gerencia respectiva se comunicarán con Ud. para desarrollar el plan de acción sobre el cambio.','1'," + ticketReq.ID_TICK + ");}window.onload=init;</script>");
        }

        public ActionResult ListarNotificaciones()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            List<ListarNotificacionesPActivos_Result> resultado = db.ListarNotificacionesPActivos(IdPersEnti).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarVistaNotificaciones()
        {
            int id = Convert.ToInt32(Request.Params["Id"]);
            try
            {
                CHANGE_APPROVED CA = db.CHANGE_APPROVED.Single(x => x.idChangeRequest == id);
                CA.VisNoti = false;
                db.Entry(CA).State = EntityState.Modified;
                db.SaveChanges();
                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        #endregion
        #region listas
        public ActionResult ListChangeType()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new CoreEntities())
            {
                var result = (from ch in context.CHANGE_TYPE.Where(x => x.IdCuenta == ID_ACCO && x.status == true).ToList()
                              select new
                              {
                                  id = ch.id,
                                  details = ch.nombre + "-" + ch.details,
                                  descripcion = ch.descripcion
                              }).ToList();

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListApproved()
        {
            using (var context = new CoreEntities())
            {
                var result = (from ap in context.CHANGE_TYPE_REQUEST.Where(x => x.estado == true)
                              select new
                              {
                                  id = ap.id,
                                  detail = ap.type
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListChangeDetails()
        {
            int ID_LIST_CHANGE = 0;
            string txt = "";
            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);
            if (FIELD == "id")
            {
                ID_LIST_CHANGE = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
                if (txt == null) { txt = ""; }
            }
            else
            {
                ID_LIST_CHANGE = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
            }

            using (var context = new CoreEntities())
            {
                var result = (from det in context.CHANGE_TYPE_DETAIL.Where(x => x.idChangeType == ID_LIST_CHANGE)
                              select new
                              {
                                  id = det.id,
                                  detail = det.detail
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListPriority()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new CoreEntities())
            {
                var result = (from pr in context.CHANGE_PRIORITY.Where(x => x.IdCuenta == ID_ACCO).ToList()
                              select new
                              {
                                  id = pr.id,
                                  detail = pr.nombre + "-" + pr.detail
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListImpact()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new CoreEntities())
            {

                var result = (from ci in context.CHANGE_IMPACT.Where(x => x.IdCuenta == ID_ACCO && x.status == true).ToList()
                              select new
                              {
                                  idImpact = ci.id,
                                  detail = ci.detail,
                                  ischecked = ci.isChecked
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListToPlan()
        {
            using (var context = new CoreEntities())
            {
                var result = (from ap in context.CHANGE_TYPE_REQUEST.Where(x => x.estado == true)
                              where ap.id == 5 || ap.id == 6
                              select new
                              {
                                  id = ap.id,
                                  detail = ap.type
                              }).ToList();

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListTaskType()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new CoreEntities())
            {
                var result = (from ctt in context.CHANGE_TYPE_TASK.Where(x => x.STATUS == true && x.IdCuenta == ID_ACCO)
                              select new
                              {
                                  id = ctt.ID,
                                  detail = ctt.TYPE_TASK
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListStaff()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var qcuenta = (from cuenta in db.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6)
                           select new
                           {
                               idcuenta = cuenta.VAL_ACCO_PARA
                           }).Single();

            int account = Convert.ToInt32(qcuenta.idcuenta);

            var qStaff = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == account && x.VIG_PERS_ENTI == true)
                          join b in dbe.CLASS_ENTITY.Where(x => x.FIR_NAME != null && x.LAS_NAME != null) on a.ID_ENTI2 equals b.ID_ENTI
                          //join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true)
                          //       on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                          //join d in dbe.CHARTs on c.ID_CHAR equals d.ID_CHAR
                          //join e in dbe.NAME_CHART on d.ID_NAM_CHAR equals e.ID_NAM_CHAR
                          select new
                          {
                              User = (b.FIR_NAME + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME)).ToLower(),
                              UserUpper = (b.FIR_NAME + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME)).ToUpper(),
                              //JobTitle = e.NAM_CHAR.ToLower(),
                              ID_PHOT = a.ID_FOTO,
                              a.ID_PERS_ENTI,
                          }).OrderBy(x => x.User);

            return Json(new { Data = qStaff }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DetailImpact(String id)
        {
            using (var context = new CoreEntities())
            {
                try
                {
                    var query = context.CH_ListaImpacto(id).ToList()[0].valores;
                    if (query == null)
                    {
                        query = "";
                    }
                    return Json(new { Data = query.ToString() }, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(new { Data = "" }, JsonRequestBehavior.AllowGet);
                }
            }

        }


        public ActionResult DetailChange()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var qcuenta = (from cuenta in db.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6)
                           select new
                           {
                               idcuenta = cuenta.VAL_ACCO_PARA
                           }).Single();

            int account = Convert.ToInt32(qcuenta.idcuenta);

            using (var context = new CoreEntities())
            {
                int id = Convert.ToInt32(Request.Params["id"]);
                var qUserEdata = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == account) //Listado de Personas
                                  join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                                  select new
                                  {
                                      ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      User = b.FIR_NAME + " " + b.LAS_NAME,
                                  }).ToList();

                var query = (from cd in context.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == id && x.STATUS == true)
                             join ctt in context.CHANGE_TYPE_TASK on cd.ID_TYPE_TASK equals ctt.ID
                             //join usr in qUserEdata on cd.ID_PERSON_ENTI equals usr.ID_PERS_ENTI                             
                             select new
                             {
                                 TYPE_TASK = ctt.TYPE_TASK,
                                 ID_PERS_ENTI = cd.ID_PERSON_ENTI,
                                 //REQUESTER = usr.User,
                                 DETAIL_TASK = cd.DETAIL,
                                 DATE_INIC = cd.DATE_INIC,
                                 DATE_END = cd.DATE_END
                             }).ToList();

                var queryFinal = (from q in query
                                  join usr in qUserEdata on q.ID_PERS_ENTI equals usr.ID_PERS_ENTI
                                  select new
                                  {
                                      TYPE_TASK = q.TYPE_TASK,
                                      REQUESTER = usr.User,
                                      DETAIL_TASK = q.DETAIL_TASK,
                                      DATE_INIC = (q.DATE_INIC == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", q.DATE_INIC).ToLower()),
                                      DATE_END = (q.DATE_END == null ? "" : string.Format("{0:MM/dd/yy H:mm:ss}", q.DATE_END).ToLower()),
                                  }).ToList();

                return Json(new { Data = queryFinal, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListDetailActivity(int id)
        {
            using (var context = new CoreEntities())
            {

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var qcuenta = (from cuenta in db.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6)
                               select new
                               {
                                   idcuenta = cuenta.VAL_ACCO_PARA
                               }).Single();

                int account = Convert.ToInt32(qcuenta.idcuenta);

                var qUserEdata = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == account) //Listado de Personas
                                  join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                                  select new
                                  {
                                      ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      User = b.FIR_NAME + " " + b.LAS_NAME,
                                  }).ToList();

                var query = (from cd in context.CHANGE_DETAIL.Where(x => x.ID_CHANGE_REQUEST == id)
                             join ctt in context.CHANGE_TYPE_TASK on cd.ID_TYPE_TASK equals ctt.ID
                             select new
                             {
                                 ID = cd.ID,
                                 ID_TYPE_TASK = cd.ID_TYPE_TASK,
                                 TYPE_TASK = ctt.TYPE_TASK,
                                 ID_PERS_ENTI = cd.ID_PERSON_ENTI,
                                 DETAIL_TASK = cd.DETAIL,
                                 DATE_INIC = cd.FechaInicioProgramada,
                                 DATE_END = cd.FechaFinProgramada,
                                 STATUS = cd.STATUS
                             }).ToList();

                var queryFinal = (from q in query
                                  join usr in qUserEdata on q.ID_PERS_ENTI equals usr.ID_PERS_ENTI
                                  where q.STATUS == true
                                  select new
                                  {
                                      ID = q.ID,
                                      ID_TYPE_TASK = q.ID_TYPE_TASK,
                                      ID_PERS_ENTI = q.ID_PERS_ENTI,
                                      DETAIL = q.DETAIL_TASK,
                                      DATE_INIC = (q.DATE_INIC == null ? "" : string.Format("{0:MM/dd/yyyy}", q.DATE_INIC).ToLower()),
                                      DATE_END = (q.DATE_END == null ? "" : string.Format("{0:MM/dd/yyyy}", q.DATE_END).ToLower()),
                                      Actividad = q.TYPE_TASK,
                                      Responsable = usr.User,
                                  }).ToList();
                return this.Jsonp(queryFinal);
            }
        }
        public JsonResult UpdateDetailActivity(int id)
        {
            var models = this.DeserializeObject<IEnumerable<CHANGE_DETAIL_UPDATE>>("models");
            if (models != null)
            {
                using (var context = new CoreEntities())
                {
                    int estado = Convert.ToInt32(db.CHANGE_APPROVED.Where(x => x.idChangeRequest == id).SingleOrDefault().idTypeRequest);
                    if (estado == 3)
                    {
                        CHANGE_DETAIL UpdCD = context.CHANGE_DETAIL.Find(models.FirstOrDefault().ID);
                        UpdCD.ID_TYPE_TASK = int.Parse(models.FirstOrDefault().Actividad);
                        UpdCD.ID_CHANGE_REQUEST = id;
                        UpdCD.ID_PERSON_ENTI = models.FirstOrDefault().Responsable;
                        UpdCD.DETAIL = models.FirstOrDefault().DETAIL;
                        UpdCD.FechaInicioProgramada = models.FirstOrDefault().DATE_INIC;
                        UpdCD.FechaFinProgramada = models.FirstOrDefault().DATE_END;
                        UpdCD.STATUS = true;
                        context.Entry(UpdCD).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                }
            }
            return this.Jsonp(models);
        }
        public JsonResult CreateDetailActivity(int id)
        {
            var models = this.DeserializeObject<IEnumerable<CHANGE_DETAIL_CREATE>>("models");
            if (models != null)
            {
                using (var context = new CoreEntities())
                {
                    //var ID_TYPE_TASK = context.CHANGE_TYPE_TASK.Where(x => x.TYPE_TASK.ToLower().Contains(models.FirstOrDefault().Actividad.ToLower())).FirstOrDefault().ID;

                    int estado = Convert.ToInt32(db.CHANGE_APPROVED.Where(x => x.idChangeRequest == id).SingleOrDefault().idTypeRequest);
                    if (estado == 3)
                    {
                        CHANGE_DETAIL detail = new CHANGE_DETAIL();
                        detail.ID_TYPE_TASK = int.Parse(models.FirstOrDefault().Actividad);
                        detail.ID_CHANGE_REQUEST = id;
                        detail.ID_PERSON_ENTI = models.FirstOrDefault().Responsable;
                        detail.DETAIL = models.FirstOrDefault().DETAIL;
                        detail.DATE_INIC = models.FirstOrDefault().DATE_INIC;
                        detail.DATE_END = models.FirstOrDefault().DATE_END;
                        detail.FechaInicioProgramada = models.FirstOrDefault().DATE_INIC;
                        detail.FechaFinProgramada = models.FirstOrDefault().DATE_END;
                        detail.STATUS = true;
                        context.CHANGE_DETAIL.Add(detail);
                        context.SaveChanges();
                    }
                }
            }
            return this.Jsonp(models);
        }
        public JsonResult DeleteDetailActivity(int id)
        {
            var models = this.DeserializeObject<IEnumerable<CHANGE_DETAIL_DELETE>>("models");
            if (models != null)
            {
                using (var context = new CoreEntities())
                {
                    CHANGE_DETAIL UpdCD = context.CHANGE_DETAIL.Find(models.FirstOrDefault().ID);
                    UpdCD.STATUS = false;
                    context.Entry(UpdCD).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            return this.Jsonp(models);
        }
        #endregion
        #region Graficos
        public ActionResult ClasificacionCambio(DateTime inic, DateTime fin, int idRequested)
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            using (var context = new CoreEntities())
            {
                var query = (from cr in context.CHANGE_REQUEST.Where(x => x.createDate >= inic && x.createDate <= fin && x.IdCuenta == ID_ACCO)
                             select new
                             {
                                 id = cr.id,
                                 ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                 idChangeType = cr.idChangeType
                             });
                if (idRequested > 0)
                {
                    query = query.Where(x => x.ID_PERS_ENTI == idRequested);
                }

                var result = (from cr in query.ToList()
                              join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                              group ct by new { ct.nombre, ct.id } into g
                              select new
                              {
                                  color = (g.Key.id == 1 ? "#3366CC" : g.Key.id == 2 ? "#DC3912" : g.Key.id == 3 ? "FF9900" : ""),
                                  nombre = g.Key.nombre,
                                  Count = g.Count()
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CambioMenor(DateTime inic, DateTime fin, int idRequested)
        {
            using (var context = new CoreEntities())
            {
                var query = (from cr in context.CHANGE_REQUEST.Where(x => x.createDate >= inic && x.createDate <= fin)
                             where cr.idChangeType == 1
                             select new
                             {
                                 id = cr.id,
                                 ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                 idChangeDetail = cr.idChangeTypeDetail
                             });
                if (idRequested > 0)
                {
                    query = query.Where(x => x.ID_PERS_ENTI == idRequested);
                }
                var result = (from cr in query.ToList()
                              join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeDetail equals ctd.id
                              group ctd by new { ctd.detail, ctd.id } into g
                              select new
                              {
                                  color = (g.Key.id == 1 ? "#3366CC" : g.Key.id == 2 ? "#DC3912" : g.Key.id == 3 ? "FF9900" :
                                  g.Key.id == 4 ? "#109618" : g.Key.id == 5 ? "#990099" : g.Key.id == 18 ? "#0099C6" : g.Key.id == 15 ? "#DD4481" : g.Key.id == 16 ? "#F7A35C" : ""),
                                  nombre = g.Key.detail,
                                  Count = g.Count()
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CambioMayor(DateTime inic, DateTime fin, int idRequested)
        {

            using (var context = new CoreEntities())
            {
                var query = (from cr in context.CHANGE_REQUEST.Where(x => x.createDate >= inic && x.createDate <= fin)
                             where cr.idChangeType == 2
                             select new
                             {
                                 id = cr.id,
                                 ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                 idChangeDetail = cr.idChangeTypeDetail

                             });
                if (idRequested > 0)
                {
                    query = query.Where(x => x.ID_PERS_ENTI == idRequested);
                }
                var result = (from cr in query.ToList()
                              join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeDetail equals ctd.id
                              group ctd by new { ctd.detail, ctd.id } into g
                              select new
                              {
                                  color = (g.Key.id == 1 ? "#3366CC" : g.Key.id == 2 ? "#DC3912" : g.Key.id == 3 ? "FF9900" :
                                  g.Key.id == 4 ? "#109618" : g.Key.id == 5 ? "#990099" : g.Key.id == 18 ? "#0099C6" : g.Key.id == 15 ? "#DD4481" : g.Key.id == 16 ? "#F7A35C" : ""),
                                  nombre = g.Key.detail,
                                  Count = g.Count()
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Impacto(DateTime inic, DateTime fin, int idRequested)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new CoreEntities())
            {
                var queryImpact = context.CH_ImpactoporSolicitud(inic, fin, idRequested, ID_ACCO).ToList();

                var result = (from cr in queryImpact
                              group cr by new { cr.detail, cr.idImpacto } into g
                              select new
                              {
                                  color = (g.Key.idImpacto == "1" ? "#3366CC" : g.Key.idImpacto == "2" ? "#DC3912" : g.Key.idImpacto == "3" ? "FF9900" : g.Key.idImpacto == "4" ? "#109618" : g.Key.idImpacto == "5" ? "#990099" : ""),
                                  nombre = g.Key.detail,
                                  Count = g.Count()
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Prioridad(DateTime inic, DateTime fin, int idRequested)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new CoreEntities())
            {
                var query = (from cr in context.CHANGE_REQUEST.Where(x => x.createDate >= inic && x.createDate <= fin && x.IdCuenta == ID_ACCO)
                             select new
                             {
                                 id = cr.id,
                                 ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                 idPriority = cr.idPriority

                             });
                if (idRequested > 0)
                {
                    query = query.Where(x => x.ID_PERS_ENTI == idRequested);
                }
                var result = (from cr in query.ToList()
                              join cp in context.CHANGE_PRIORITY on cr.idPriority equals cp.id
                              group cp by new { cp.nombre, cp.id } into g
                              select new
                              {
                                  color = "",
                                  nombre = g.Key.nombre,
                                  Count = g.Count()
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Autorización(DateTime inic, DateTime fin, int idRequested)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new CoreEntities())
            {
                var query = (from ca in context.CHANGE_APPROVED.Where(x => x.createDate >= inic && x.createDate <= fin)
                             join cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO) on ca.idChangeRequest equals cr.id
                             select new
                             {
                                 id = ca.id,
                                 ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                 idTypeRequest = ca.idTypeRequest

                             });
                if (idRequested > 0)
                {
                    query = query.Where(x => x.ID_PERS_ENTI == idRequested);
                }
                var result = (from cr in query.ToList()
                              join cp in context.CHANGE_TYPE_REQUEST on cr.idTypeRequest equals cp.id
                              group cp by new { cp.type, cp.id } into g
                              select new
                              {
                                  color = (g.Key.id == 1 ? "#3366CC" : g.Key.id == 2 ? "#DC3912" : g.Key.id == 3 ? "FF9900" :
                                             g.Key.id == 4 ? "#7cb5ec" : g.Key.id == 5 ? "#90ed7d" : g.Key.id == 6 ? "#CCC" : ""),
                                  nombre = g.Key.type,
                                  Count = g.Count()
                              }).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ObtenerNotificaciones()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = db.NotificacionesResponsableCambio(IdPersEnti).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public int CantidadNotificaciones()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = db.NotificacionesResponsableCambio(IdPersEnti).ToList();


            return query.Count();
        }
        #endregion
        #region Reportes
        public ActionResult ExportarListaDetalleCambio()
        {
            int ID_CHANGE_REQUEST = Convert.ToInt32(Request.Params["id"]);
            List<GestionCambiosListar_Result> query = db.GestionCambiosListar(ID_CHANGE_REQUEST).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin:5px 10px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='cabecera'>Tipo de Tarea</td>");
            sb.Append("<th class='cabecera'>Responsable</td>");
            sb.Append("<th class='cabecera'>Descripcion</td>");
            sb.Append("<th class='cabecera'>Fecha Inicio</td>");
            sb.Append("<th class='cabecera'>Fecha Fin</td>");
            sb.Append("<th class='cabecera'>Observaciones</td>");
            sb.Append("</tr>");

            foreach (GestionCambiosListar_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.TYPE_TASK + "</td>");
                sb.Append("<td class='contenido'>" + dr.Responsable + "</td>");
                sb.Append("<td class='contenido'>" + dr.DETAIL + "</td>");
                sb.Append("<td class='contenido'>" + dr.DATE_INIC + "</td>");
                sb.Append("<td class='contenido'>" + dr.DATE_END + "</td>");
                sb.Append("<td class='contenido'>" + dr.OBSERVACIONES + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=DetalleCambio" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion

        [Authorize]
        public ActionResult VReporteCambiosBNV()
        {
            return View();
        }

        [Authorize]
        public ActionResult DescargarExcelGCambios(string FROM_DATE = "", string TO_DATE = "")
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (!DateTime.TryParseExact(FROM_DATE, "yyyy/M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicio)
                || !DateTime.TryParseExact(TO_DATE, "yyyy/M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFin))
            {
                return Json(new { success = false, message = "Formato de fecha no válido" });
            }

            List<ReporteGCambios> resultados = ListRegistrosCambiosBNV(fechaInicio, fechaFin);

            if (resultados != null && resultados.Count >= 1)
            {
                try
                {
                    string rutaPlantilla = Server.MapPath("~/Adjuntos/Reporte Gestion Cambios.xlsx");
                    string nombreCopia = $"ReporteGCambios_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                    string tempFilePath = Path.Combine(Server.MapPath("~/Adjuntos"), nombreCopia);

                    System.IO.File.Copy(rutaPlantilla, tempFilePath, true);

                    using (var workbook = new XLWorkbook(tempFilePath))
                    {
                        var worksheet = workbook.Worksheet(1);

                        worksheet.Cell(2, 2).Value = FROM_DATE;
                        worksheet.Cell(2, 4).Value = TO_DATE;
                        int row = 5;

                        foreach (var resultado in resultados)
                        {
                            worksheet.Cell(row, 1).Value = resultado.IdChangeRequest;
                            worksheet.Cell(row, 2).Value = GetName(resultado.AutorCrea);

                            Dictionary<string, object> detailChange = GetDetailChange(resultado.IdChangeRequest);
                            worksheet.Cell(row, 3).Value = detailChange["Responsable"];
                            worksheet.Cell(row, 4).Value = detailChange["Grupo"];
                            worksheet.Cell(row, 5).Value = resultado.User;

                            Dictionary<string, object> person = GetPerson(resultado.IdPersSol);
                            worksheet.Cell(row, 6).Value = person["Cargo"];

                            int? idLoca = person["IdLoca"] as int?;
                            worksheet.Cell(row, 7).Value = GetSite(idLoca);
                            worksheet.Cell(row, 8).Value = GetLocation(idLoca);

                            worksheet.Cell(row, 9).Value = resultado.CreateDate;
                            worksheet.Cell(row, 10).Value = resultado.CreateDate;
                            worksheet.Cell(row, 11).Value = resultado.CreateDate;

                            DateTime? fechaCierre = detailChange["FechaCierre"] as DateTime?;
                            worksheet.Cell(row, 12).Value = fechaCierre;
                            worksheet.Cell(row, 13).Value = fechaCierre;
                            worksheet.Cell(row, 14).Value = fechaCierre;

                            var diasPendientes = fechaCierre.HasValue ? Math.Abs(((TimeSpan)(fechaCierre?.Date - resultado.CreateDate?.Date)).Days) : Math.Abs(((TimeSpan)(DateTime.Now.Date - resultado.CreateDate?.Date)).Days);
                            worksheet.Cell(row, 15).Value = diasPendientes;
                            worksheet.Cell(row, 16).Value = GetRango(diasPendientes);

                            worksheet.Cell(row, 17).Value = resultado.ChangeType + " - " + resultado.ChangeTypeDetail;
                            worksheet.Cell(row, 18).Value = resultado.Estado;
                            worksheet.Cell(row, 19).Value = resultado.Asunto;
                            worksheet.Cell(row, 20).Value = resultado.Descripcion;
                            worksheet.Cell(row, 21).Value = GetComentarioSol(resultado.IdChangeRequest);

                            var rangoCeldas = worksheet.Range(worksheet.Cell(row, 1), worksheet.Cell(row, 21));
                            rangoCeldas.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            rangoCeldas.Style.Border.TopBorderColor = XLColor.SkyBlue;
                            rangoCeldas.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            rangoCeldas.Style.Border.LeftBorderColor = XLColor.SkyBlue;
                            rangoCeldas.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            rangoCeldas.Style.Border.RightBorderColor = XLColor.SkyBlue;
                            rangoCeldas.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            rangoCeldas.Style.Border.BottomBorderColor = XLColor.SkyBlue;
                            rangoCeldas.Style.Alignment.WrapText = false;

                            row++;

                        }
                        workbook.Save();
                    }

                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment; filename={nombreCopia}");

                    using (var stream = new FileStream(tempFilePath, FileMode.Open))
                    {
                        stream.CopyTo(Response.OutputStream);
                    }

                    Response.End();

                    System.IO.File.Delete(tempFilePath);

                    return Json(new { success = true, message = "Se descargó el excel" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Ha ocurrido un error, contacte al administrador" });
                }

            }
            else
            {
                return Json(new { success = false });
            }
        }

        private List<ReporteGCambios> ListRegistrosCambiosBNV(DateTime fechaInicio, DateTime fechaFin)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            List<ReporteGCambios> list = new List<ReporteGCambios>();

            using (var context = new CoreEntities())
            {
                int ID_PERS_ENTI = Convert.ToInt32(Convert.ToString(Session["ID_PERS_ENTI"]));
                bool listarAprobadosxJefes = false;
                ListarTodos = false;
                #region datos Usuario y Jefe

                //(ERP)
                var idEntiCuenta = (from d in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                    select d.ID_ENTI1);


                var qUserEdata = context.GestionCambioObtenerUsuarios().ToList();


                int idJefe = (from a in context.CHANGE_APPROVED
                              join b in context.CHANGE_REQUEST on a.idChangeRequest equals b.id
                              where b.ID_PERS_ENTI == ID_PERS_ENTI
                              select a.approver).FirstOrDefault();

                var qJefeEdata = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == idEntiCuenta.FirstOrDefault())//idEntiCuenta) //Listado de Personas
                                  join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                                  select new
                                  {
                                      ID_PERS_ENTI = a.ID_PERS_ENTI,
                                      Jefe = b.FIR_NAME + " " + b.LAS_NAME,
                                      UsuarioJefe = b.FIR_NAME.Substring(0, 1) + ". " + b.LAS_NAME
                                  }).ToList();
                #endregion

                #region Permisos de Visualizacion de solicitudes
                int esJefe = (from a in context.CHANGE_APPROVED.Where(x => x.approver == ID_PERS_ENTI)
                              select a.approver).FirstOrDefault();
                if (esJefe > 0)
                {
                    listarAprobadosxJefes = true;
                }

                var queu_ID_MDS = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                   select a.ID_QUEU).FirstOrDefault();
                if ((int)Session["ADMINISTRADOR"] == 1 || queu_ID_MDS == 100)
                {
                    ListarTodos = true;
                }
                #endregion

                #region JefesAprobadores
                if (listarAprobadosxJefes && !ListarTodos) ///this
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO && x.createDate >= fechaInicio && x.createDate <= fechaFin)
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  select new
                                  {
                                      idChangeRequest = cr.id,
                                      autorCrea = cr.ID_PERS_ENTI_Modified,
                                      IdPersEntiSol = cr.ID_PERS_ENTI,
                                      registro = cr.createDate,
                                      TypeApproved = ctr.type,
                                      detailChange = cr.question2 == null ? "" : cr.question2,
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,

                                      approved = ca.approver,
                                      orden = ctr.Orden,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      IdResponsable = 79581,//cd.ID_PERSON_ENTI,
                                      ID_PERS_ENTI_SOL = cr.ID_PERS_ENTI_ASSI
                                  }).Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.approved == ID_PERS_ENTI || x.IdResponsable == ID_PERS_ENTI).Distinct();

                    var queryFinal = (from a in result.ToList().OrderBy(x => x.idChangeRequest)
                                      join b in qUserEdata on a.ID_PERS_ENTI_SOL equals b.ID_PERS_ENTI
                                      join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      select new ReporteGCambios
                                      {
                                          IdChangeRequest = a.idChangeRequest,
                                          AutorCrea = a.autorCrea,
                                          User = b.User,
                                          IdPersSol = a.IdPersEntiSol,
                                          CreateDate = a.registro,
                                          Estado = a.TypeApproved,
                                          Asunto = a.detailChange,
                                          Descripcion = a.question1,
                                          ChangeType = a.ChangeType,
                                          ChangeTypeDetail = a.ChangeTypeDetail
                                      }).Distinct();

                    list = queryFinal.ToList();
                }
                #endregion JefesAprobadores
                #region Administradores y SIG
                else if (ListarTodos)
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO && x.createDate >= fechaInicio && x.createDate <= fechaFin).ToList()
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  select new
                                  {
                                      idChangeRequest = cr.id,
                                      autorCrea = cr.ID_PERS_ENTI_Modified,
                                      IdPersEntiSol = cr.ID_PERS_ENTI,
                                      registro = cr.createDate,
                                      TypeApproved = ctr.type,
                                      detailChange = cr.question2 == null ? "" : cr.question2,
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,

                                      approved = ca.approver,
                                      orden = ctr.Orden
                                  });

                    var qJefeEdata1 = db.GestionCambioObtenerJefesBNV().ToList();

                    var queryFinal = from a in result.ToList().OrderBy(x => x.idChangeRequest)
                                     join b in qUserEdata on a.IdPersEntiSol equals b.ID_PERS_ENTI
                                     join c in qJefeEdata1 on a.approved equals c.ID_PERS_ENTI
                                     select new ReporteGCambios
                                     {
                                         IdChangeRequest = a.idChangeRequest,
                                         AutorCrea = a.autorCrea,
                                         User = b.User,
                                         IdPersSol = a.IdPersEntiSol,
                                         CreateDate = a.registro,
                                         Estado = a.TypeApproved,
                                         Asunto = a.detailChange,
                                         Descripcion = a.question1,
                                         ChangeType = a.ChangeType,
                                         ChangeTypeDetail = a.ChangeTypeDetail
                                     };

                    list = queryFinal.ToList();
                }
                #endregion
                #region usuarios
                else
                {
                    var result = (from cr in context.CHANGE_REQUEST.Where(x => x.IdCuenta == ID_ACCO && x.createDate >= fechaInicio && x.createDate <= fechaFin)
                                  join ca in context.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
                                  join ct in context.CHANGE_TYPE on cr.idChangeType equals ct.id
                                  join ctd in context.CHANGE_TYPE_DETAIL on cr.idChangeTypeDetail equals ctd.id
                                  join pr in context.CHANGE_PRIORITY on cr.idPriority equals pr.id
                                  join ctr in context.CHANGE_TYPE_REQUEST on ca.idTypeRequest equals ctr.id
                                  join cd in context.CHANGE_DETAIL on cr.id equals cd.ID_CHANGE_REQUEST
                                  join tk in context.TICKETs on cr.IdTicket equals tk.ID_TICK into x
                                  from tk in x.DefaultIfEmpty()

                                  select new
                                  {
                                      idChangeRequest = cr.id,
                                      autorCrea = cr.ID_PERS_ENTI_Modified,
                                      IdPersEntiSol = cr.ID_PERS_ENTI,
                                      registro = cr.createDate,
                                      TypeApproved = ctr.type,
                                      detailChange = (cr.Activo == null ? cr.question2 : cr.question2.Substring(0, 61) + tk.COD_TICK),
                                      question1 = cr.question1 == null ? "" : cr.question1,
                                      ChangeType = ct.nombre,
                                      ChangeTypeDetail = ctd.detail,
                                      orden = ctr.Orden,

                                      approved = ca.approver,
                                      ID_PERS_ENTI = cr.ID_PERS_ENTI,
                                      IdResponsable = cd.ID_PERSON_ENTI
                                  }).Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.IdResponsable == ID_PERS_ENTI).Distinct();

                    var queryFinal = (from a in result.ToList().OrderBy(x => x.idChangeRequest)
                                      join b in qUserEdata on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                                      join c in qJefeEdata on a.approved equals c.ID_PERS_ENTI
                                      select new ReporteGCambios
                                      {
                                          IdChangeRequest = a.idChangeRequest,
                                          AutorCrea = a.autorCrea,
                                          User = b.User,
                                          IdPersSol = a.IdPersEntiSol,
                                          CreateDate = a.registro,
                                          Estado = a.TypeApproved,
                                          Asunto = a.detailChange,
                                          Descripcion = a.question1,
                                          ChangeType = a.ChangeType,
                                          ChangeTypeDetail = a.ChangeTypeDetail
                                      }).Distinct();

                    list = queryFinal.ToList();
                }
                #endregion

                return list;
            }
        }

        private string GetName(int? id_pers)
        {
            var name = (from p in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id_pers)
                        join c in dbe.CLASS_ENTITY on p.ID_ENTI2 equals c.ID_ENTI
                        select (c.FIR_NAME + " " + c.LAS_NAME + " " + c.MOT_NAME)).FirstOrDefault();

            if (name == null) return "";

            TextInfo textInfo = new CultureInfo("es-ES", false).TextInfo;
            return textInfo.ToTitleCase(name.ToLower());
        }

        private Dictionary<string, object> GetDetailChange(int id_change)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            using (var context = new CoreEntities())
            {
                var result = context.CHANGE_DETAIL
                    .Where(x => x.ID_CHANGE_REQUEST == id_change && x.ActividadRealizada == null)
                    .OrderBy(x => x.ID)
                    .FirstOrDefault();

                if (result == null)
                {
                    result = context.CHANGE_DETAIL
                        .Where(x => x.ID_CHANGE_REQUEST == id_change && x.ActividadRealizada == true)
                        .OrderByDescending(x => x.ID)
                        .FirstOrDefault();
                }

                if (result != null)
                {
                    dict.Add("Responsable", GetName(result.ID_PERSON_ENTI));
                    dict.Add("FechaCierre", result.FechaActividadRealizada);
                    dict.Add("Grupo", GetGroup(result.ID_PERSON_ENTI));
                }

                return dict;
            }
        }

        private string GetRango(int dias)
        {
            if (dias <= 30)
            {
                return "Menor a 30 días";
            }
            else if (dias > 30 && dias <= 60)
            {
                return "31 a 60 días";
            }
            else if (dias > 60 && dias <= 90)
            {
                return "61 a 90 días";
            }
            else if (dias > 90 && dias <= 120)
            {
                return "91 a 120 días";
            }
            else
            {
                return "Mayor a 120 días";
            }
        }

        private string GetLocation(int? id_loca)
        {
            if (id_loca == null) return null;

            using (var context = new CoreEntities())
            {
                var location = context.LOCATIONs.Where(x => x.ID_LOCA == id_loca).SingleOrDefault();

                if (location != null) return location.NAM_LOCA;
            }

            return null;
        }

        private Dictionary<string, object> GetPerson(int? id_pers)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var result = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id_pers).FirstOrDefault();

            if (result != null)
            {
                dict.Add("Cargo", result.CAR_PERS);
                dict.Add("IdLoca", result.ID_LOCA);
            }

            return dict;
        }

        private string GetSite(int? id_loca)
        {
            if (id_loca == null) return null;

            using (var context = new CoreEntities())
            {
                var site = (from l in context.LOCATIONs
                            join s in context.SITEs on l.ID_SITE equals s.ID_SITE
                            where l.ID_LOCA == id_loca
                            select (s.NAM_SITE)).SingleOrDefault();

                if (site != null) return site;
            }

            return null;
        }

        private string GetGroup(int? id_pers)
        {
            var id_queue = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id_pers).Select(x => x.ID_QUEU).FirstOrDefault();

            using (var context = new CoreEntities())
            {
                var grupo = context.QUEUEs.Where(x => x.ID_QUEU == id_queue).SingleOrDefault();

                if (grupo != null) return grupo.NAM_QUEU_REPO;
            }

            return null;
        }

        private string GetComentarioSol(int id_change_req)
        {
            using (var context = new CoreEntities())
            {
                var result = context.CHANGE_REQUEST_COMMENT
                    .Where(x => x.ID_CHA == id_change_req)
                    .OrderByDescending(x => x.ID_DETA_CHAN)
                    .FirstOrDefault();

                if (result != null) return result.COM_DETA_CHA;

                return "";
            }
        }
    }
    public partial class CHANGE_DETAIL_DELETE
    {
        public string Actividad { get; set; }
        public Nullable<System.DateTime> DATE_INIC { get; set; }
        public Nullable<System.DateTime> DATE_END { get; set; }
        public string DETAIL { get; set; }
        public int ID { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<int> ID_TYPE_TASK { get; set; }
        public string Responsable { get; set; }
    }
    public partial class CHANGE_DETAIL_UPDATE
    {
        public string Actividad { get; set; }
        public Nullable<System.DateTime> DATE_INIC { get; set; }
        public Nullable<System.DateTime> DATE_END { get; set; }
        public string DETAIL { get; set; }
        public int ID { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<int> ID_TYPE_TASK { get; set; }
        public int Responsable { get; set; }
    }
    public partial class CHANGE_DETAIL_CREATE
    {
        public string Actividad { get; set; }
        public Nullable<System.DateTime> DATE_INIC { get; set; }
        public Nullable<System.DateTime> DATE_END { get; set; }
        public string DETAIL { get; set; }
        public int Responsable { get; set; }
    }

    public struct ReporteGCambios
    {
        public int IdChangeRequest;
        public int? AutorCrea;
        public string User;
        public int? IdPersSol;
        public DateTime? CreateDate;
        public string Estado;
        public string Asunto;
        public string Descripcion;
        public string ChangeType;
        public string ChangeTypeDetail;
    }

}
