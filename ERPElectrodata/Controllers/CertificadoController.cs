using ERPElectrodata.Models;
using ERPElectrodata.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using System.Text;
using ClosedXML.Excel;

namespace ERPElectrodata.Controllers
{
    [Authorize]
    public class CertificadoController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();


        #region Vistas

        //
        // GET: /Certificado/

        public ActionResult ProgramacionCertificado()
        {
            int flagRRHH = 0;
            int flagOperaciones = 0;

            try
            {
                //if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1 || (int)Session["ADMINISTRADOR"] == 1 
                //    || (int)Session["APROBAR_SELECCION_PERSONAL"] == 1 || (int)Session["OPERACIONES"] == 1 || (int)Session["RRHH"] == 1) {
                //    if ((int)Session["RRHH"] == 1)
                //    {
                //        flagRRHH = 1;
                //    }
                //    if ((int)Session["APROBAR_SELECCION_PERSONAL"] == 1 || (int)Session["OPERACIONES"] == 1)
                //    {
                //        flagOperaciones = 1;
                //    }
                //    ViewBag.RRHH = flagRRHH;
                //    ViewBag.OPERACIONES = flagOperaciones;
                //    return View();
                //}
                if ((int)Session["RRHH"] == 1 || (int)Session["SUPERVISOR_RRHH"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_COMPLETA"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_PARCIAL"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_ORGANIGRAMA"] == 1)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }             
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult Reprogramacion(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult Editar(int id)
        {
                int flagRRHH = 0;
            if ((int)Session["RRHH"] == 1 || (int)Session["SUPERVISOR_RRHH"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_COMPLETA"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_PARCIAL"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_ORGANIGRAMA"] == 1)
            {
                flagRRHH = 1;
            }
            ViewBag.RRHH = flagRRHH;
            ViewBag.Id = id;
            return View();
        }
        public ActionResult Eliminar(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult RendirExamen(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult CargarCertificado(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult CalendarioCertificaciones()
        {
            return View();
        }

        public ActionResult MisProgramaciones()
        {                        
            try
            {
                if(Session["UserId"] == null)
                {
                    return RedirectToAction("Index", "Error");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult CalendarioMisCertificaciones()
        {
            return View();
        }

        public ActionResult ImportarExcel()
        {
            return View();
        }

        public ActionResult MisCertificaciones()
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return RedirectToAction("Index", "Error");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult VerDocumento(string id = "")
        {
            //ViewBag.Flag = id;
            ViewBag.Documento = id;
            return View();
        }
        #endregion

        public ActionResult ObtenerDatosCertificado(int id)
        {
            var result = dbe.ObtenerDatosCertificado(id).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarMisProgramaciones(int id)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            var result = dbe.MisProgramaciones(IdPersEnti, id).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaMotivoCertificado()
        {
            var query = dbe.ListaMotivoCertificado().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCertificadosPendientes()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            int IdMarca = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            var query = dbe.ListarCertificadosPendientes(IdPersEnti, IdMarca).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCertificados(int id)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            var result = dbe.ListarCertificados(IdPersEnti, id).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarMisCertificados()
        {
            int IdMarca = 0;
            DateTime? FechaInicio = null, FechaFin = null;
            if (int.TryParse(Convert.ToString(Request.Params["IdMarca"]), out IdMarca) == false)
            {
                IdMarca = 0;
            }

            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "" || Convert.ToString(Request.QueryString["FechaInicio"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"]);
            }

            if (Convert.ToString(Request.QueryString["FechaFin"]) == "" || Convert.ToString(Request.QueryString["FechaFin"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.Params["FechaFin"]);
            }
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            int UserId = (int)Session["UserId"];
            var result = dbe.MisCertificados(IdPersEnti, IdMarca,FechaInicio,FechaFin, UserId).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarReprogramaciones(int id)
        {
            var result = dbe.ListarReprogramaciones(id).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult RegistrarProgramacion(Certificado objCertificado)
        {
            try
            {
                int IdUsuario = Convert.ToInt32(Session["UserId"].ToString());
                string mensaje = "";

                if (objCertificado.IdGerencia == null) { mensaje = "- Seleccione la gerencia."; }
                if (objCertificado.IdArea == null) { mensaje = mensaje + "<br>- Seleccione el área."; }
                if (objCertificado.IdPersEnti == null) { mensaje = mensaje + "<br>- Seleccione el usuario."; }
                if (objCertificado.Monto == null || objCertificado.Monto == 0) 
                { mensaje = mensaje + "<br>- Ingrese un monto."; }
                if (objCertificado.IdMarca == null) { mensaje = mensaje + "<br>- Seleccione la marca."; }
                if (string.IsNullOrEmpty(objCertificado.Nombre)) { mensaje = mensaje + "<br>- Ingrese el nombre del Certificado."; }
                if (string.IsNullOrEmpty(objCertificado.NombreExamen)) { mensaje = mensaje + "<br>- Ingrese el nombre del examen."; }
                if (objCertificado.MotivoId == null) { 
                    mensaje = mensaje + "<br>- Seleccione el Motivo de la Certificación.";
                }
        

                if (Convert.ToString(objCertificado.FechaProgramada)=="") { mensaje = mensaje + "<br>- Ingrese una fecha programada."; }

                if (mensaje != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MostrarMensaje) top.MostrarMensaje('VACIO','"+ mensaje + "');}window.onload=init;</script>");
                }
                //var JefeResponsable = new ResponsablexArea_Result();
                int? IdPersEntiarea = null;
                var nombrejefe = "";
                var emailjefe = "";
       




                var jefeInmediato = (from Colaborador in dbe.PERSON_CONTRACT
                                     join C in dbe.CHARTs on Colaborador.ID_CHAR equals C.ID_CHAR
                                     join ContratoJefe in dbe.PERSON_CONTRACT on C.ID_CHAR_PARE equals ContratoJefe.ID_CHAR
                                     join Jefe in dbe.PERSON_ENTITY on ContratoJefe.ID_PERS_ENTI equals Jefe.ID_PERS_ENTI
                                     join ClaseJefe in dbe.CLASS_ENTITY on Jefe.ID_ENTI2 equals ClaseJefe.ID_ENTI
                                     where Colaborador.ID_PERS_ENTI == objCertificado.IdPersEnti
                                         && Colaborador.VIG_CONT == true
                                         && Jefe.ID_PERS_STAT == 1
                                     select new
                                     {
                                         nombrejefe = ClaseJefe.LAS_NAME + " " + ClaseJefe.MOT_NAME,
                                         EmailJefe = Jefe.EMA_ELEC,
                                         ID_PERS_ENTI = Jefe.ID_PERS_ENTI
                                     }).FirstOrDefault();


                if (jefeInmediato == null)
                {
                    var JefeResponsable = dbe.ResponsablexArea(objCertificado.IdArea).FirstOrDefault();

                    if (jefeInmediato != null)
                    {
                        IdPersEntiarea = JefeResponsable.ID_PERS_ENTI;
                        nombrejefe = JefeResponsable.Nombre;
                        emailjefe = JefeResponsable.Email;
                    }
                }
                if (jefeInmediato != null)
                {
                    IdPersEntiarea = jefeInmediato.ID_PERS_ENTI;
                    nombrejefe = jefeInmediato.nombrejefe;
                    emailjefe = jefeInmediato.EmailJefe;
                }




                DateTime fec_ini = Convert.ToDateTime(objCertificado.FechaProgramada);
                objCertificado.FechaProgramadaOrigin = objCertificado.FechaProgramada;
                objCertificado.IdEstado =1;
                objCertificado.FechaCreacion = DateTime.Now;
                objCertificado.UsuarioCreacion = IdUsuario;
                objCertificado.JefeInmediato = IdPersEntiarea;

                objCertificado.Estado = true;

                dbe.Certificado.Add(objCertificado);
                dbe.SaveChanges();

                SendMail mail = new SendMail();
                mail.EnviarProgramacionCertificado((int)objCertificado.Id, (int)objCertificado.IdGerencia, (int)objCertificado.IdArea, (int)objCertificado.IdPersEnti, objCertificado.Nombre, (DateTime)objCertificado.FechaProgramada, (int)objCertificado.IdEstado, (int)objCertificado.IdArea, (string)emailjefe);

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MostrarMensaje) top.MostrarMensaje('OK','');}window.onload=init;</script>");

            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MostrarMensaje) top.MostrarMensaje('ERROR','');}window.onload=init;</script>");
            }
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult ReprogramarCertificacion(Reprogramacion objReprogramacion)
        {
            try
            {
                int IdUsuario = Convert.ToInt32(Session["UserId"].ToString());
                int flag = 0;

                if (Convert.ToString(objReprogramacion.Motivo) == "") { flag = 1; }
                if (Convert.ToString(Request.Params["txtFechaProgramada"]) == "") { flag = 2; }

                if (flag != 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeReprogramacion) top.MensajeReprogramacion('VACIO');}window.onload=init;</script>");
                }

                int IdCertificado = Convert.ToInt32(Request.Params["Id"]);

                DateTime fechaProgramada = Convert.ToDateTime(objReprogramacion.FechaProgramada);
                objReprogramacion.IdCertificado = IdCertificado;
                objReprogramacion.FechaProgramada = Convert.ToDateTime(Request.Params["txtFechaProgramada"]);
                objReprogramacion.FechaCreacion = DateTime.Now;
                objReprogramacion.UsuarioCreacion = IdUsuario;
                objReprogramacion.Estado = true;

                dbe.Reprogramacion.Add(objReprogramacion);
                dbe.SaveChanges();


                Certificado objCertificado = dbe.Certificado.Where(x => x.Id == IdCertificado).SingleOrDefault();

                objCertificado.FechaProgramada = Convert.ToDateTime(Request.Params["txtFechaProgramada"]);
                objCertificado.FechaModifica = DateTime.Now;
                objCertificado.UsuarioModifica = IdUsuario;
                dbe.Certificado.Attach(objCertificado);
                dbe.Entry(objCertificado).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeReprogramacion) top.MensajeReprogramacion('OK');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeReprogramacion) top.MensajeReprogramacion('ERROR');}window.onload=init;</script>");
            }
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult EditarCertificado(int id = 0)
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                Certificado objCertificado = dbe.Certificado.Where(x => x.Id == id).SingleOrDefault();

                objCertificado.Nombre = Convert.ToString(Request.Params["txtNombre"]);
                objCertificado.Monto = Convert.ToDecimal(Request.Params["txtMonto"]);
                objCertificado.FechaModifica = DateTime.Now;
                objCertificado.UsuarioModifica = UserId;
                dbe.Certificado.Attach(objCertificado);
                dbe.Entry(objCertificado).State = EntityState.Modified;
                dbe.SaveChanges();
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeEditar) top.MensajeEditar('OK');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeEditar) top.MensajeEditar('ERROR');}window.onload=init;</script>");
            }

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ExamenRendido(int id = 0, string examenRendido = "si")
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                Certificado objCertificado = dbe.Certificado.Where(x => x.Id == id).SingleOrDefault();

                string mensajeExamen = "RENDIDO";

                if (examenRendido == "no")
                {
                    objCertificado.IdEstado = 5;
                    mensajeExamen = "NO_RENDIDO";
                }
                else
                {
                    objCertificado.IdEstado = 2;
                }
                objCertificado.FechaModifica = DateTime.Now;
                objCertificado.UsuarioModifica = UserId;
                dbe.Certificado.Attach(objCertificado);
                dbe.Entry(objCertificado).State = EntityState.Modified;
                dbe.SaveChanges();
                return Content($"<script type='text/javascript'> function init() {{" +
                       $"if(top.MensajeExamen) top.MensajeExamen('{mensajeExamen}');}}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeExamen) top.MensajeExamen('ERROR');}window.onload=init;</script>");
            }

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EliminarProgramacion(int id = 0)
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                Certificado objCertificado = dbe.Certificado.Where(x => x.Id == id).SingleOrDefault();

                objCertificado.FechaModifica = DateTime.Now;
                objCertificado.UsuarioModifica = UserId;
                objCertificado.Estado = false;
                dbe.Certificado.Attach(objCertificado);
                dbe.Entry(objCertificado).State = EntityState.Modified;
                dbe.SaveChanges();
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeEliminar) top.MensajeEliminar('OK');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeEliminar) top.MensajeEliminar('ERROR');}window.onload=init;</script>");
            }

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CargarCertificacion(IEnumerable<HttpPostedFileBase> files, PERSON_DOCUMENTS objDocumentos)
        {
            try
            {
                string mensaje = "";
                int IdUsuario = Convert.ToInt32(Session["UserId"].ToString());
                int Vigencia = Convert.ToInt32(Request.Params["Vigencia"]);
                int aprobacion = Convert.ToInt32(Request.Params["rbAprobacion"]);

                if (Vigencia == 1)
                {
                    if (Convert.ToString(Request.Params["txtFechaInicio"]) == "") { mensaje = "- Seleccione la fecha inicio."; }
                    if (Convert.ToString(Request.Params["txtFechaFin"]) == "") { mensaje = mensaje + "<br>- Seleccione la fecha final."; }
                }

                if (aprobacion == 1)
                {
                    foreach (var file in files)
                    {
                        if (file == null)
                        {
                            mensaje = mensaje + "<br>- Seleccione un archivo";
                        }
                    }
                }
                if (mensaje != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeCompletado) top.MensajeCompletado('VACIO','"+ mensaje +"');}window.onload=init;</script>");
                }

                int IdCertificado = Convert.ToInt32(Request.Params["Id"]);

                Certificado objCertificado = dbe.Certificado.Where(x => x.Id == IdCertificado).SingleOrDefault();
                if (aprobacion == 1)
                {
                    objCertificado.IdEstado = 3;
                }
                else
                {
                    objCertificado.IdEstado = 4;
                }
                objCertificado.FechaModifica = DateTime.Now;
                objCertificado.UsuarioModifica = IdUsuario;
                dbe.Certificado.Attach(objCertificado);
                dbe.Entry(objCertificado).State = EntityState.Modified;
                dbe.SaveChanges();

                //PERSON_DOCUMENTS
                if (aprobacion == 1)
                {
                    int IdPersEnti = Convert.ToInt32(objCertificado.IdPersEnti);
                    PERSON_CONTRACT objContract = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == IdPersEnti && x.VIG_CONT == true && x.LAS_CONT == true).FirstOrDefault();
                    objDocumentos.ID_TYPE_DOCU = 6;
                    objDocumentos.ID_PERS_ENTI = IdPersEnti;
                    objDocumentos.CREATE_ATTA = DateTime.Now;
                    objDocumentos.UserId = IdUsuario;
                    objDocumentos.ID_PERS_CONT = Convert.ToInt32(objContract.ID_PERS_CONT);
                    objDocumentos.IdCuenta = IdUsuario;
                    objDocumentos.Vigencia = Convert.ToBoolean(Vigencia);
                    if (Vigencia == 1)
                    {
                        DateTime fechaInicio;
                        DateTime fechaFin;

                        if (!DateTime.TryParseExact(Request.Params["txtFechaInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fechaInicio))
                        {
                            return Content("<script>alert('Formato de fecha de inicio incorrecto.');</script>");
                        }

                        if (!DateTime.TryParseExact(Request.Params["txtFechaFin"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fechaFin))
                        {
                            return Content("<script>alert('Formato de fecha de fin incorrecto.');</script>");
                        }

                        if (fechaFin < fechaInicio)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeCompletado) top.MensajeCompletado('VACIO','La fecha de fin no puede ser anterior a la fecha de inicio.');}window.onload=init;</script>");


                            


                        }

                        objDocumentos.FechaInicioVigencia = fechaInicio;
                        objDocumentos.FechaFinVigencia = fechaFin;
                    }
                    objDocumentos.Estado = true;
                    objDocumentos.IdMarca = Convert.ToInt32(objCertificado.IdMarca);
                    objDocumentos.IdCertificacion = IdCertificado;

                    int? IdPersEntiarea = null;
                    var nombrejefe = "";
                    var emailjefe = "";

                    var jefeInmediato = (from Colaborador in dbe.PERSON_CONTRACT
                                         join C in dbe.CHARTs on Colaborador.ID_CHAR equals C.ID_CHAR
                                         join ContratoJefe in dbe.PERSON_CONTRACT on C.ID_CHAR_PARE equals ContratoJefe.ID_CHAR
                                         join Jefe in dbe.PERSON_ENTITY on ContratoJefe.ID_PERS_ENTI equals Jefe.ID_PERS_ENTI
                                         join ClaseJefe in dbe.CLASS_ENTITY on Jefe.ID_ENTI2 equals ClaseJefe.ID_ENTI
                                         where Colaborador.ID_PERS_ENTI == objCertificado.IdPersEnti
                                             && Colaborador.VIG_CONT == true
                                             && Jefe.ID_PERS_STAT == 1
                                         select new
                                         {
                                             nombrejefe = ClaseJefe.LAS_NAME + " " + ClaseJefe.MOT_NAME,
                                             EmailJefe = Jefe.EMA_ELEC,
                                             ID_PERS_ENTI = Jefe.ID_PERS_ENTI
                                         }).FirstOrDefault();


                    if (jefeInmediato == null)
                    {
                        var JefeResponsable = dbe.ResponsablexArea(objCertificado.IdArea).FirstOrDefault();

                        if (jefeInmediato != null)
                        {
                            IdPersEntiarea = JefeResponsable.ID_PERS_ENTI;
                            nombrejefe = JefeResponsable.Nombre;
                            emailjefe = JefeResponsable.Email;
                        }
                    }
                    if (jefeInmediato != null)
                    {
                        IdPersEntiarea = jefeInmediato.ID_PERS_ENTI;
                        nombrejefe = jefeInmediato.nombrejefe;
                        emailjefe = jefeInmediato.EmailJefe;
                    }

                    //Archivo
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            try
                            {
                                String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                                        RegexOptions.None, TimeSpan.FromSeconds(1.5));
                                var NAM_ATTA = doc;
                                var EXT_ATTA = Path.GetExtension(file.FileName);

                                objDocumentos.NAM_ATTA = NAM_ATTA;
                                objDocumentos.EXT_ATTA = EXT_ATTA;
                                dbe.PERSON_DOCUMENTS.Add(objDocumentos);
                                dbe.SaveChanges();

                                /*DESCOMENTAR CUANDO PASE*/
                                file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Documents/" + NAM_ATTA + "_" + Convert.ToString(objDocumentos.ID_PERS_DOCU) + EXT_ATTA));

                                SendMail mail = new SendMail();
                                mail.EnviarProgramacionCertificado((int)objCertificado.Id, (int)objCertificado.IdGerencia, (int)objCertificado.IdArea, (int)objCertificado.IdPersEnti, objCertificado.Nombre, (DateTime)objCertificado.FechaProgramada, (int)objCertificado.IdEstado, (int)objCertificado.IdArea, (string)emailjefe);
                            }
                            catch
                            {
                                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeCompletado) top.MensajeCompletado('ERROR','');}window.onload=init;</script>");
                            }
                        }
                    }
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeCompletado) top.MensajeCompletado('OK','" + aprobacion + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeCompletado) top.MensajeCompletado('ERROR','');}window.onload=init;</script>");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ImportarExcel(HttpPostedFileBase excelFile, ExcelCertificado objExcel)
        {
            try
            {
                int IdUsuario = Convert.ToInt32(Session["UserId"].ToString());

                if (excelFile.FileName.EndsWith("xls") || excelFile.FileName.EndsWith("xlsx"))
                {
                    //Guardar archivo
                    String doc = Regex.Replace(Path.GetFileNameWithoutExtension(excelFile.FileName), @"[^\w\.@-]", "",
                                                        RegexOptions.None, TimeSpan.FromSeconds(1.5));
                    var NAM_ATTA = doc;
                    var EXT_ATTA = Path.GetExtension(excelFile.FileName);

                    objExcel.Nombre = NAM_ATTA;
                    objExcel.Extension = EXT_ATTA;
                    objExcel.FechaCreacion = DateTime.Now;
                    objExcel.UsuarioCreacion = IdUsuario;
                    objExcel.Estado = true;
                    dbe.ExcelCertificado.Add(objExcel);
                    dbe.SaveChanges();

                    string path = Server.MapPath("~/Adjuntos/Talent/Documents/" + NAM_ATTA + "_" + Convert.ToString(objExcel.Id) + EXT_ATTA);
                    excelFile.SaveAs(path);

                    //Carga
                    Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(path);
                    Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange;

                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        string gerencia = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 1]).Text;
                        string area = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 2]).Text;
                        string persona = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 3]).Text;
                        string marca = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 4]).Text;
                        string certificado = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 5]).Text;
                        string fecha = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 6]).Text;
                        DateTime fechaDatetime = Convert.ToDateTime(fecha);

                        string mensaje = dbe.InsertarCertificadoExcel(gerencia, area, persona, marca, certificado, fechaDatetime, IdUsuario).SingleOrDefault().Mensaje;
                    }

                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MostrarMensajeExcel) top.MostrarMensajeExcel('OK');}window.onload=init;</script>");
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MostrarMensajeExcel) top.MostrarMensajeExcel('FORMATO');}window.onload=init;</script>");

                }

            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MostrarMensajeExcel) top.MostrarMensajeExcel('ERROR');}window.onload=init;</script>");
            }
        }


        public FileResult DescargarArchivo(string id = "")
        {
            try
            {
                string ruta = "";

                ruta = "~/Adjuntos/Talent/Documents/";

                FileStream fileStream = System.IO.File.OpenRead(Server.MapPath(ruta + id));
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

                //Response.ContentType = "application/octet-stream";
                if ((id.ToLower()).Contains(".pdf"))
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".txt"))
                {
                    Response.ContentType = "text/text";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".txt");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".jpg"))
                {
                    Response.ContentType = "image/jpeg";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".jpg");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".png"))
                {
                    Response.ContentType = "image/png";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".png");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else
                {
                    string filename = ruta + id;
                    return File(filename, "application/pdf", id);
                }

            }
            catch
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

                return File(stream, "text/plain", "Error.txt");
            }

        }

        public ActionResult DescargarReporteGlobalCertificado()
        {
            try
            {
                var query = dbe.Database.SqlQuery<ReporteGlobalCertificado_Result>("Certificado.ReporteGlobalCertificado").ToList();

                // Crear el archivo Excel usando ClosedXML
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var worksheet = wb.Worksheets.Add("ReporteGlobalCertificado");


                    worksheet.Cell(1, 1).Value = "Estado";
                    worksheet.Cell(1, 2).Value = "Gerencia";
                    worksheet.Cell(1, 3).Value = "Área";
                    worksheet.Cell(1, 4).Value = "Persona";
                    worksheet.Cell(1, 5).Value = "DNI";
                    worksheet.Cell(1, 6).Value = "Cargo";
                    worksheet.Cell(1, 7).Value = "Correo";
                    worksheet.Cell(1, 8).Value = "Jefe";
                    worksheet.Cell(1, 9).Value = "Certificado";
                    worksheet.Cell(1, 10).Value = "Examen";
                    worksheet.Cell(1, 11).Value = "Motivo Certificado";
                    worksheet.Cell(1, 12).Value = "Marca";
                    worksheet.Cell(1, 13).Value = "Fecha Programada";
                    worksheet.Cell(1, 14).Value = "Monto";
                    worksheet.Cell(1, 15).Value = "Fecha Inicio Vigencia";
                    worksheet.Cell(1, 16).Value = "Fecha Fin Vigencia";
                    worksheet.Cell(1, 17).Value = "Usuario Creador";
                    worksheet.Cell(1, 18).Value = "Fecha Creación";
                    worksheet.Cell(1, 19).Value = "Motivo Reprogramación";
                    worksheet.Cell(1, 20).Value = "Fecha Reprogramación";
                    worksheet.Cell(1, 21).Value = "Usuario Modificador";



                    int row = 2;
                    foreach (var item in query)
                    {
                        worksheet.Cell(row, 1).Value = item.Estado;
                        worksheet.Cell(row, 2).Value = item.Gerencia;
                        worksheet.Cell(row, 3).Value = item.Area;
                        worksheet.Cell(row, 4).Value = item.Persona;
                        worksheet.Cell(row, 5).Value = item.DNI;
                        worksheet.Cell(row, 6).Value = item.CARGO;
                        worksheet.Cell(row, 7).Value = item.EMAIL;
                        worksheet.Cell(row, 8).Value = item.JEFE;
                        worksheet.Cell(row, 9).Value = item.Certificado;
                        worksheet.Cell(row, 10).Value = item.Examen;
                        worksheet.Cell(row, 11).Value = item.MotivoCertificado;
                        worksheet.Cell(row, 12).Value = item.Marca;
                        worksheet.Cell(row, 13).Value = item.FechaProgramada;
                        worksheet.Cell(row, 14).Value = item.Monto;
                        worksheet.Cell(row, 15).Value = item.Fecha_Inicio_Certificado;
                        worksheet.Cell(row, 16).Value = item.Fecha_Fin_Certificado;
                        worksheet.Cell(row, 17).Value = item.Usuario_Creador;
                        worksheet.Cell(row, 18).Value = item.FechaCreacion;
                        worksheet.Cell(row, 19).Value = item.MotivoReprogramacion;
                        worksheet.Cell(row, 20).Value = item.NuevaFechaProgramada;
                        worksheet.Cell(row, 21).Value = item.UsuarioModificador;

                        row++;
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Flush();
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteGlobalCertificado.xlsx");
                    }
                    //var range = worksheet.Range(1, 1, row - 1, 21);
                    //var table = range.CreateTable();
                    //table.Theme = XLTableTheme.TableStyleMedium9;

                    //worksheet.Columns().AdjustToContents();

                    //using (MemoryStream stream = new MemoryStream())
                    //{
                    //    wb.SaveAs(stream);
                    //    stream.Flush();
                    //    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteGlobalCertificado.xlsx");
                    //}
                }
            }

            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error, contáctese con el Administrador: " + ex.Message);
            }
        }


        public ActionResult DetalleCertificado(int id)
        {
            try
            {
                int flagRRHH = 0;

                if ((int)Session["RRHH"] == 1 || (int)Session["SUPERVISOR_RRHH"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_COMPLETA"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_PARCIAL"] == 1 || (int)Session["CERTIFICADOS_VISUALIZACION_ORGANIGRAMA"] == 1)
                {
                    flagRRHH = 1;
                }
                ViewBag.RRHH = flagRRHH;


                var certificadoDetalle = dbe.Database.SqlQuery<ObtenerDetalleCertificado_Result>(
                    "Certificado.ObtenerDetalleCertificado {0}", id).FirstOrDefault();

                if (certificadoDetalle == null)
                {
                    return HttpNotFound();
                }

                return View(certificadoDetalle);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error Contacte al Administardor: " + ex.Message);
            }
        }




        public ActionResult ObtenerListadoGerenciasCertificado()
        {
            var query = dbe.ListaGerenciasCertificado().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerListadoAreasxGerenciaCertificado()
        {

            //int IdGerencia = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);

            int ID_GER = 0;
            string CTE = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_CHAR")
            {
                ID_GER = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "title")
            {
                CTE = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }

            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_CHAR")
            {
                ID_GER = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "title")
            {
                CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }

            var query = dbe.AreasxGerenciaCertificado(ID_GER, CTE).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerListadoPersonalxAreaCertificado()
        {
            int IdArea = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);

            var query = dbe.ListaEmpleadosxAreaCertificado(IdArea).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
