using ERPElectrodata.Models;
using ERPElectrodata.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace ERPElectrodata.Controllers
{
    public class SeleccionSolicitudPersonalController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        #region Vistas

        [Authorize]
        public ActionResult CrearSolicitud()
        {
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }

        public ActionResult EditarSolicitud(int? id = 0)
        {
            var Remuneracion = (from r in dbe.SolicitudPersonal where r.Id == id select r).First().Remuneracion;
            ViewBag.Remuneracion = Remuneracion;
            ViewBag.Id = id;
            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarSolicitud(SolicitudPersonal p)
        {
            var Solicitud = dbe.SolicitudPersonal.Where(i => i.Id == p.Id).Single();
            Solicitud.Remuneracion = p.Remuneracion;
            dbe.SolicitudPersonal.Attach(Solicitud);
            dbe.Entry(Solicitud).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        public ActionResult IngresarDuracion(int? id = 0)
        {
            //var Beneficio = (from r in dbe.SolicitudPersonal where r.Id == id select r).First().Beneficio;
            var s = dbe.SolicitudPersonal.Find(id);
            ViewBag.DuracionContrato = s.DuracionContrato;
            ViewBag.Id = id;
            return View(s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IngresarDuracion(SolicitudPersonal p)
        {
            var Solicitud = dbe.SolicitudPersonal.Where(i => i.Id == p.Id).Single();
            Solicitud.DuracionContrato = p.DuracionContrato;
            dbe.SolicitudPersonal.Attach(Solicitud);
            dbe.Entry(Solicitud).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        public ActionResult EditarProyecto(int? id = 0)
        {
            var query1 = (from s in dbe.SolicitudPersonal
                         select s).ToList();
            var query2 = (from c in dbc.DOCUMENT_SALE
                           select c).ToList();
            var query3 = (from c in dbe.CLASS_ENTITY
                          select c).ToList();
            var Cliente = (from sp in query1
                           join ds in query2 
                        on sp.IdProyecto equals ds.ID_DOCU_SALE
                            join ca in query3
                       on ds.ID_COMP equals ca.ID_ENTI
                           where sp.Id == id
                          select ca).FirstOrDefault().ID_ENTI;
            var Proyecto = (from r in dbe.SolicitudPersonal
                                 where r.Id == id
                                select r).First().IdProyecto;
            ViewBag.IdCliente = Cliente;
            ViewBag.Id = id;
            ViewBag.IdProyecto= Proyecto;
            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProyecto(SolicitudPersonal p)
        {
            var Solicitud = dbe.SolicitudPersonal.Where(i => i.Id == p.Id).Single();
            Solicitud.IdProyecto = p.IdProyecto;
            dbe.SolicitudPersonal.Attach(Solicitud);
            dbe.Entry(Solicitud).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        public ActionResult AgregarProyecto(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public ActionResult AgregarProyecto(SolicitudPersonal solicitudPersonal)
        {
            try
            {
                string msg = "";
                if (solicitudPersonal.IdProyecto == 0 || solicitudPersonal.IdProyecto == null)
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneAgregarOP) top.uploadDoneAgregarOP('Seleccione el proyecto.');}window.onload=init;</script>");
                //msg += "Seleccione el proyecto. <br/>";

                var solicitud = dbe.SolicitudPersonal.Where(i => i.Id == solicitudPersonal.Id).Single();

                solicitud.IdProyecto = solicitudPersonal.IdProyecto;
                solicitud.FechaFinProyecto = solicitudPersonal.FechaFinProyecto;
                solicitud.Presupuesto = solicitudPersonal.Presupuesto;
                dbe.SolicitudPersonal.Attach(solicitud);
                dbe.Entry(solicitud).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneAgregarOP) top.uploadDoneAgregarOP('OK');}window.onload=init;</script>");
            }
            catch (Exception e)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneAgregarOP) top.uploadDoneAgregarOP('ERROR');}window.onload=init;</script>");
            }
        }

        public ActionResult ListarProyectoComboBox(int? id = 0)
        {
            var idCliente = id;
            var query = dbe.ListarProyectoVigente(idCliente, "").ToList();
             return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult ListarSolicitudPersonal()
        {
            int flag = 0, aprobadoradm = 0;
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1 || (int)Session["ADMINISTRADOR"] == 1 || (int)Session["APROBAR_SELECCION_PERSONAL"] == 1
                   || (int)Session["RRHH"] == 1)
                {
                    flag = 1;
                }


                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                if (IdPersEnti == 11783 || IdPersEnti == 1008)
                {
                    aprobadoradm = 1;
                }
            }
            catch
            {


            }

            ViewBag.FlagAprobador = flag;
            ViewBag.APROBADORADM = aprobadoradm;
            return View();
        }

        [Authorize]
        public ActionResult DetalleSolicitudPersonal(int id = 0)
        {

            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Id = id;
            var IdUser = Convert.ToInt32(Session["UserId"].ToString());
            var IdUsuario = (from ex in dbe.Usuarios where ex.UserId == IdUser select ex).First().Id;
            var VerificarPermiso = (from u in dbe.UsuarioPerfils
                                    where u.IdUsuario == IdUsuario
                                    orderby u.Perfil.Perfil1
                                    select new
                                    {
                                        per = u.Perfil

                                    }).ToList();
            foreach (var xc in VerificarPermiso)
            {
                string eva = xc.per.Perfil1;
                if (eva == "ADMINISTRADOR_SISTEMA" || IdUser == 208 || IdUser == 35)
                {
                    ViewBag.ACCESOO = 1;
                    break;
                }
                if (eva == "OPERACIONES")
                {
                    ViewBag.ACCESOO = 2;
                    break;
                }
                if (eva == "RRHH" || eva == "SUPERVISOR_RRHH")
                {
                    ViewBag.ACCESOO = 3;
                    break;
                }
            }
            var IdSolicitud = (from ex in dbe.SolicitudPersonal where ex.Id == id select ex).First().IdEstadoSolicitud;
            var UsuarioSolicita = (from ex in dbe.SolicitudPersonal where ex.Id == id select ex).First().UsuarioCrea;
            if (Convert.ToInt32(Session["ID_PERS_ENTI"].ToString()) == UsuarioSolicita)
            {
                ViewBag.Solicitante = 1;

            }
            ViewBag.Estado = IdSolicitud;

            return View();
        }

        public ActionResult RechazarSolicitudPersonal(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        [Authorize]
        public ActionResult ListarSolicitudesHistoricas()
        {
            return View();
        }

        #endregion

        public ActionResult RenovarSolicitud(int id = 0)
        {
            var renovacionResult = dbe.ObtenerRenovacion(id).FirstOrDefault();
            return View(renovacionResult);
        }

        public ActionResult ObtenerDatosSolicitud(int id = 0)
        {
            var query = dbe.ObtenerDetalleSolicitud(id).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarMotivoVacante()
        {
            var query = dbe.ListarMotivoVacante().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoContrato()
        {
            var query = dbe.ListarTipoContrato().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSolicitantePersonal()
        {
            var query = dbe.ListarSolicitantePersonal().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAreaSolicitantePersonal()
        {
            var query = dbe.ListarAreaSolicitantePersonal().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProyectoVigente()
        {
            int ID_CLI = 0;
            string txt = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_CLI = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "PROYECTO")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }

            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_CLI = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "PROYECTO")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }

            var query = dbe.ListarProyectoVigente(ID_CLI, txt).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPersonalEdataPorArea()
        {
            string txt = "";
            int IdArea = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);

            txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (txt == null) { txt = ""; }

            var query = dbe.ListarPersonalEdtaArea(IdArea, txt).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }




        [Authorize]
        public ActionResult AprobarSolicitud()
        {

            string IdSolicitudes = Convert.ToString(Request.Params["IdSolicitud"]);
            IdSolicitudes = IdSolicitudes.Replace("select-all,", "");
            try
            {
                foreach (string strIdSolicitud in IdSolicitudes.Split(','))
                {
                    if (strIdSolicitud == "")
                    {
                        return Content("VACIO");
                    }
                    else
                    {
                        int Id = Convert.ToInt32(strIdSolicitud);
                        //var estado =dbe.SolicitudPersonal.Where(x => x.Id == Id).FirstOrDefault();
                        //if(estado.IdEstadoSolicitud == 1)
                        //{
                        int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

                        var solicitud = dbe.SolicitudPersonal.Where(x => x.Id == Id).SingleOrDefault();
                        if (IdUsuario == 11783 || IdUsuario == 1008)
                        {
                            solicitud.IdEstadoSolicitud = 6;

                        }
                        else
                        {
                            solicitud.IdEstadoSolicitud = 2;
                        }

                        solicitud.FechaModifica = DateTime.Now;
                        solicitud.UsuarioModifica = IdUsuario;
                        dbe.Entry(solicitud).State = EntityState.Modified;
                        solicitud.IdAprobacion = IdUsuario;
                        dbe.SaveChanges();

                        try
                        {
                            var soli = dbe.SolicitudPersonal.Where(x => x.Id == Id).FirstOrDefault();
                            int GTH = 0;
                            var IdGth = (from pe in dbe.PERSON_ENTITY
                                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                         join us in dbe.Usuarios on ce.UserId equals us.UserId
                                         where pe.ID_PERS_ENTI == soli.UsuarioCrea
                                         select us).FirstOrDefault().Id;

                            var VerificarPermiso = (from u in dbe.UsuarioPerfils
                                                    where u.IdUsuario == IdGth
                                                    orderby u.Perfil.Perfil1
                                                    select new
                                                    {
                                                        per = u.Perfil

                                                    }).ToList();
                            if (VerificarPermiso.Count() != 0)
                            {
                                foreach (var xc in VerificarPermiso)
                                {
                                    string eva = xc.per.Perfil1;

                                    if (eva == "RRHH" || eva == "SUPERVISOR_RRHH")
                                    {
                                        GTH = 1;
                                        break;
                                    }
                                }
                            }


                            SendMail mail = new SendMail();
                            mail.EnviarSolicitudPersonal((int)soli.UsuarioCrea, (int)soli.IdGerencia, (int)soli.IdEstadoSolicitud, soli.Id, GTH, 0, null);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RechazarSolicitudPersonal()
        {
            try
            {


                string IdSolicitudes = Convert.ToString(Request.Params["IdSolicitudesRechazo"]);
                IdSolicitudes = IdSolicitudes.Replace("select-all,", "");

                foreach (string strIdSolicitud in IdSolicitudes.Split(','))
                {
                    if (strIdSolicitud == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MostrarMensajeRechazado) top.MostrarMensajeRechazado('VACIO','');}window.onload=init;</script>");
                    }
                    else
                    {
                        int Id = Convert.ToInt32(strIdSolicitud);
                        int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

                        var objSolicitudRechazado = new SolicitudRechazado();
                        objSolicitudRechazado.IdSolicitudPersonal = Id;
                        objSolicitudRechazado.IdMotivoRechazo = Convert.ToInt32(Request.Params["cbMotivo" + strIdSolicitud]);
                        objSolicitudRechazado.FechaCrea = DateTime.Now;
                        objSolicitudRechazado.UsuarioCrea = IdUsuario;
                        objSolicitudRechazado.Estado = true;

                        dbe.SolicitudRechazado.Add(objSolicitudRechazado);
                        dbe.SaveChanges();

                        //Rechazar
                        var solicitud = dbe.SolicitudPersonal.Where(x => x.Id == Id).SingleOrDefault();
                        solicitud.IdEstadoSolicitud = 4;
                        solicitud.FechaModifica = DateTime.Now;
                        solicitud.UsuarioModifica = IdUsuario;
                        dbe.Entry(solicitud).State = EntityState.Modified;
                        dbe.SaveChanges();
                        try
                        {
                            var soli = dbe.SolicitudPersonal.Where(x => x.Id == Id).FirstOrDefault();
                            int GTH = 0;
                            var IdGth = (from pe in dbe.PERSON_ENTITY
                                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                         join us in dbe.Usuarios on ce.UserId equals us.UserId
                                         where pe.ID_PERS_ENTI == soli.UsuarioCrea
                                         select us).FirstOrDefault().Id;

                            var VerificarPermiso = (from u in dbe.UsuarioPerfils
                                                    where u.IdUsuario == IdGth
                                                    orderby u.Perfil.Perfil1
                                                    select new
                                                    {
                                                        per = u.Perfil

                                                    }).ToList();
                            if (VerificarPermiso.Count() != 0)
                            {
                                foreach (var xc in VerificarPermiso)
                                {
                                    string eva = xc.per.Perfil1;

                                    if (eva == "RRHH" || eva == "SUPERVISOR_RRHH")
                                    {
                                        GTH = 1;
                                        break;
                                    }
                                }
                            }
                            SendMail mail = new SendMail();
                            mail.EnviarSolicitudPersonal((int)soli.UsuarioCrea, (int)soli.IdGerencia, (int)soli.IdEstadoSolicitud, soli.Id, GTH, 0, null);
                        }
                        catch (Exception ex)
                        {


                        }
                    }

                }


                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MostrarMensajeRechazado) top.MostrarMensajeRechazado('OK','Las solicitudes fueron rechazadas.');}window.onload=init;</script>");
            }
            catch
            {

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MostrarMensajeRechazado) top.MostrarMensajeRechazado('ERROR','');}window.onload=init;</script>");
            }
        }

        public ActionResult ListarSolicitudesRechazar()
        {
            string id = Convert.ToString(Request.QueryString["IdSolicitudes"]);
            string IdSolicitudes = id.Replace("select-all,", "");
            var result = dbe.ListarSolicitudesARechazar(IdSolicitudes).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarMotivoRechazo(string q, string page)
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

            var result = dbe.ListarMotivoRechazo(termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


          public ActionResult VerDocumento(string id = "")
           {
               //ViewBag.Flag = id;
               ViewBag.Documento = id;
               return View();
           }
       
        public FileResult DescargarArchivo(string id = "")
        {
            try
            {
                string ruta = "";

                ruta = "~/Adjuntos/Talent/Candidatos/";

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



        public ActionResult DescargarArchivoSolicitud(int id)
        {
            try
            {
                string nombreArchivo = id.ToString() + ".docx";
                string rutaArchivo = Path.Combine(Server.MapPath("~/Adjuntos/Talent/SolicitudPersonal/"), nombreArchivo);

                if (System.IO.File.Exists(rutaArchivo))
                {
                    return File(rutaArchivo, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", nombreArchivo);
                }
                else
                {
                    ViewBag.ErrorMessage = "El archivo no existe.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al descargar el archivo: " + ex.Message;
                return View(); 
            }
        }


        public ActionResult VerificarExistenciaArchivo(int id)
        {
            try
            {
                string nombreArchivo = id.ToString() + ".docx";
                string rutaArchivo = Path.Combine(Server.MapPath("~/Adjuntos/Talent/SolicitudPersonal/"), nombreArchivo);

                bool existe = System.IO.File.Exists(rutaArchivo);

                return Json(new { existe = existe }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al verificar la existencia del archivo: " + ex.Message });
            }
        }




        [HttpPost, ValidateInput(false)]
        public ActionResult CrearSolicitud(IEnumerable<HttpPostedFileBase> archivos, ASSET asset, SolicitudPersonal objSolicitudPersonal, int[] valor = null)
        {
            try
            {
                int flag = 0, AprobadoJefe = 0, GTH = 0;
                int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
                var IdUser = Convert.ToInt32(Session["UserId"].ToString());
                var Id = (from ex in dbe.Usuarios where ex.UserId == IdUser select ex).First().Id;
                var VerificarPermiso = (from u in dbe.UsuarioPerfils
                                        where u.IdUsuario == Id
                                        orderby u.Perfil.Perfil1
                                        select new
                                        {
                                            per = u.Perfil

                                        }).ToList();
                foreach (var xc in VerificarPermiso)
                {
                    string eva = xc.per.Perfil1;


                    if (eva == "APROBAR_SELECCION_PERSONAL" || IdUser == 35 || eva == "RRHH")
                    {
                        AprobadoJefe = 1;
                        break;
                    }
                }

                if (VerificarPermiso.Count() != 0)
                {
                    foreach (var xc in VerificarPermiso)
                    {
                        string eva = xc.per.Perfil1;

                        if (eva == "RRHH" || eva == "SUPERVISOR_RRHH")
                        {
                            GTH = 1;
                            break;
                        }
                    }
                }


                bool nuevoPuesto = Request.Form["nuevo"] == "nuevo";

                if (nuevoPuesto)
                {
                    string cargoManual = Request.Form["cargoManual"];

                    if (!string.IsNullOrEmpty(cargoManual))
                    {
                        objSolicitudPersonal.Cargo = cargoManual;
                    }
                    else
                    {
                        flag = 3;
                    }
                }


                if (objSolicitudPersonal.IdMotivoVacante == null) { flag = 1; }
                if (objSolicitudPersonal.IdArea <= 0 || objSolicitudPersonal.IdArea == null) { flag = 3; }
                if (objSolicitudPersonal.IdTipoContrato == null) { flag = 5; }


                if (String.IsNullOrEmpty(objSolicitudPersonal.Justificacion)) { flag = 7; }
                if (objSolicitudPersonal.CantidadVacante == 0) { flag = 8; }
                if (String.IsNullOrEmpty(objSolicitudPersonal.Requisitos)) { flag = 9; }

                if (objSolicitudPersonal.IdMotivoVacante == 1 || objSolicitudPersonal.IdMotivoVacante == 4)
                {
                    if (objSolicitudPersonal.Cargo == "" || objSolicitudPersonal.Cargo == null) { flag = 2; }
                    if (objSolicitudPersonal.IdJefeInmediato <= 0 || objSolicitudPersonal.IdJefeInmediato == null) { flag = 6; }
                }
                else if (objSolicitudPersonal.IdMotivoVacante == 2)
                {
                    if (objSolicitudPersonal.IdPersonaReemplaza <= 0 || objSolicitudPersonal.IdPersonaReemplaza == null) { flag = 4; }
                }
                else if (objSolicitudPersonal.IdMotivoVacante == 3)
                {
                    if (objSolicitudPersonal.Cargo == "" || objSolicitudPersonal.Cargo == null) { flag = 2; }
                    if (objSolicitudPersonal.IdJefeInmediato <= 0 || objSolicitudPersonal.IdJefeInmediato == null) { flag = 6; }
                    if (objSolicitudPersonal.IdProyecto == 0) { flag = 1; }
                    string p = Convert.ToString(objSolicitudPersonal.Presupuesto);
                    if (String.IsNullOrEmpty(p) || p == "0")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MostrarMensaje) top.MostrarMensaje('MENSAJE','Ingrese un presupuesto.');}window.onload=init;</script>");
                    }
                }

                if (flag != 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MostrarMensaje) top.MostrarMensaje('VACIO','');}window.onload=init;</script>");
                }

                DateTime fec_ini = Convert.ToDateTime(objSolicitudPersonal.FechaInicioTrabajo);
                DateTime fec_fin = Convert.ToDateTime(objSolicitudPersonal.FechaFinTrabajo);
                //TimeSpan diferencia = fec_fin.Subtract(fec_ini);
                if (AprobadoJefe == 1)
                {
                    objSolicitudPersonal.IdEstadoSolicitud = 2;
                }
                else
                {
                    objSolicitudPersonal.IdEstadoSolicitud = 1;
                }

                if (Convert.ToString(Request.Params["FechaInicioTrabajo"].ToString()) == "")
                {
                    objSolicitudPersonal.FechaInicioTrabajo = null;
                }
                else
                {
                    objSolicitudPersonal.FechaInicioTrabajo = DateTime.ParseExact(Convert.ToString(Request.Params["FechaInicioTrabajo"].ToString()), "dd/MM/yyyy", null);
                }

                if (Convert.ToString(Request.Params["FechaFinTrabajo"].ToString()) == "")
                {
                    objSolicitudPersonal.FechaFinTrabajo = null;
                }
                else
                {
                    objSolicitudPersonal.FechaFinTrabajo = DateTime.ParseExact(Convert.ToString(Request.Params["FechaFinTrabajo"].ToString()), "dd/MM/yyyy", null);
                }

                if (Convert.ToString(Request.Params["FechaFinProyecto"].ToString()) == "")
                {
                    objSolicitudPersonal.FechaFinProyecto = null;
                }
                else
                {
                    objSolicitudPersonal.FechaFinProyecto = DateTime.ParseExact(Convert.ToString(Request.Params["FechaFinProyecto"].ToString()), "dd/MM/yyyy", null);
                }

                if (Convert.ToString(Request.Params["Presupuesto"].ToString()) == "")
                {
                    objSolicitudPersonal.Presupuesto = 0;
                }
                else
                {
                    objSolicitudPersonal.Presupuesto = Convert.ToDecimal(Request.Params["Presupuesto"].ToString());
                }

                objSolicitudPersonal.FechaCrea = DateTime.Now;
                objSolicitudPersonal.UsuarioCrea = IdUsuario;
                objSolicitudPersonal.Estado = true;
                objSolicitudPersonal.Remuneracion = 0;
                objSolicitudPersonal.Modalidad = Request.Params["Opcion"].ToString();
                string ubicacion = Request.Params["Ubicacion"];
                if (ubicacion != null)
                {
                    objSolicitudPersonal.UbicacionModalidad = ubicacion;
                }



                dbe.SolicitudPersonal.Add(objSolicitudPersonal);
                dbe.SaveChanges();

                //Registro de Otros Requerimientos

                List<string> otrosRequerimientos = new List<string>();

                if (Request.Form["sctrSuperficie"] != null)
                {
                    otrosRequerimientos.Add(Request.Form["sctrSuperficie"]);
                }

                if (Request.Form["sctrSocavon"] != null)
                {
                    otrosRequerimientos.Add(Request.Form["sctrSocavon"]);
                }

                if (Request.Form["tRegistro"] != null)
                {
                    otrosRequerimientos.Add(Request.Form["tRegistro"]);
                }

                if (Request.Form["elVidaLey"] != null)
                {
                    otrosRequerimientos.Add(Request.Form["elVidaLey"]);
                }

                var ultimoId = dbe.SolicitudPersonal.OrderByDescending(s => s.Id).Select(s => s.Id).FirstOrDefault();

                foreach (var item in otrosRequerimientos)
                {
                    RequisitoAdicionalSolicitud requisitos = new RequisitoAdicionalSolicitud();

                    requisitos.IdSolicitudPersonal = ultimoId;
                    requisitos.UsuarioCrea = IdUsuario;
                    requisitos.FechaCrea = DateTime.Now;
                    requisitos.Requisito = item;

                    dbe.RequisitoAdicionalSolicitud.Add(requisitos);
                }

                dbe.SaveChanges();



                var idATTA = dbe.SolicitudPersonal
        .OrderByDescending(s => s.Id)
        .Select(s => s.Id)
        .FirstOrDefault();

                if (archivos != null)
                {

                    string rutaCarpeta = Server.MapPath("~/Adjuntos/Talent/SolicitudPersonal/");

                    foreach (var file in archivos)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);


                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_SOLICITUD_PERSONAL = idATTA;
                            //ATTA.ID_ASSE = asset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;
                            if (valor != null)
                            {
                                foreach (int v in valor)
                                {
                                    //
                                    var soli = dbe.SolicitudPersonal.OrderByDescending(x => x.Id).FirstOrDefault();
                                    ATTA.ID_SOLICITUD_PERSONAL = soli.Id;
                                }
                            }


                            var idarchivo = dbe.SolicitudPersonal
                                    .OrderByDescending(s => s.Id)
                                    .Select(s => s.Id)
                                    .FirstOrDefault();

                            string extension = Path.GetExtension(file.FileName);
                            string nombreArchivo = idarchivo.ToString() + extension;
                            string rutaCompletaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);
                            file.SaveAs(rutaCompletaArchivo);


                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            //                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/SeleccionPersonal/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);




                        }
                        catch
                        {

                        }
                    }
                }

                //


                List<string> concatSoftware = new List<string>();
                string otroSoftware = null;

                if (Request.Form["cSIDIGE"] != null)
                {
                    concatSoftware.Add(Request.Form["cSIDIGE"]);
                }

                if (Request.Form["cBITRIX"] != null)
                {
                    concatSoftware.Add(Request.Form["cBITRIX"]);
                }

                if (Request.Form["cITMS"] != null)
                {
                    concatSoftware.Add(Request.Form["cITMS"]);
                }

                if (Request.Form["cOtro"] != null)
                {
                    concatSoftware.Add(Request.Form["cOtro"]);

                    if (!string.IsNullOrEmpty(Request.Params["Especificar"]))
                    {
                        otroSoftware = Request.Params["Especificar"].ToString();
                    }

                }


                foreach (var item in concatSoftware)
                {
                    RecursosAdicionales adicionales = new RecursosAdicionales();

                    adicionales.IdSolicitudPersonal = ultimoId;
                    adicionales.UsuarioCrea = IdUsuario;
                    adicionales.FechaCrea = DateTime.Now;
                    adicionales.TipoRecurso = "Software";
                    adicionales.Recurso = item;

                    if (item == "Otro")
                    {
                        adicionales.Descripcion = otroSoftware;
                    }

                    dbe.RecursosAdicionales.Add(adicionales);
                }

                dbe.SaveChanges();


                List<string> concatRed = new List<string>();

                if (Request.Form["CuentaRed"] != null)
                {
                    concatRed.Add(Request.Form["CuentaRed"]);
                }

                if (Request.Form["Email"] != null)
                {
                    concatRed.Add(Request.Form["Email"]);
                }

                foreach (var item in concatRed)
                {
                    RecursosAdicionales adicionales = new RecursosAdicionales();

                    adicionales.IdSolicitudPersonal = ultimoId;
                    adicionales.UsuarioCrea = IdUsuario;
                    adicionales.FechaCrea = DateTime.Now;
                    adicionales.TipoRecurso = "Red";
                    adicionales.Recurso = item;

                    dbe.RecursosAdicionales.Add(adicionales);
                }

                dbe.SaveChanges();


                if (!string.IsNullOrEmpty(Request.Params["Carpeta"]))
                {
                RecursosAdicionales carpeta = new RecursosAdicionales();
                carpeta.IdSolicitudPersonal = ultimoId;
                carpeta.UsuarioCrea = IdUsuario;
                carpeta.Descripcion = Request.Params["Carpeta"].ToString();
                carpeta.FechaCrea = DateTime.Now;
                carpeta.TipoRecurso = "Carpeta";
                carpeta.TipoAcceso = Request.Form["permisos"];
                    dbe.RecursosAdicionales.Add(carpeta);
                    dbe.SaveChanges();
                }

               try
                {
                    var soli = dbe.SolicitudPersonal.OrderByDescending(x => x.Id).FirstOrDefault();
                    SendMail mail = new SendMail();
                    mail.EnviarSolicitudPersonal((int)soli.UsuarioCrea, (int)soli.IdGerencia, (int)soli.IdEstadoSolicitud, soli.Id, GTH, 0, null);
                }
                catch (Exception ex)
                {


                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MostrarMensaje) top.MostrarMensaje('OK','La solicitud fue registrada correctamente.');}window.onload=init;</script>");

            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MostrarMensaje) top.MostrarMensaje('ERROR','');}window.onload=init;</script>");
            }
        }


        public ActionResult ObtenerListadoGerencias()
        {
            var query = dbe.ListaGerencias().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerListadoAreasxGerencia()
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

            var query = dbe.AreasxGerencia(ID_GER, CTE).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerListadoPersonalxArea()
        {
            int IdArea = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);

            var query = dbe.ListaEmpleadosxArea(IdArea).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarEstados()
        {
            var query = dbe.ListarEstadoSolicitud().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ListarCargoSolicitud()
        {
            int IdArea = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            var query = dbe.ListarCargoSeleccion(IdArea).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListadoSolicitudPersonal()
        {
            int sw = 0, Error = 0, Estado = 0, IdGerencia = 0, Area = 0, Solicitante = 0;
            string msjError = string.Empty;
            DateTime? FechaInicio = null, FechaFin = null;
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (int.TryParse(Convert.ToString(Request.Params["IdEstado"]), out Estado) == false)
            {
                Estado = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdGerencia"]), out IdGerencia) == false)
            {
                IdGerencia = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdArea"]), out Area) == false)
            {
                Area = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdSolicitante"]), out Solicitante) == false)
            {
                Solicitante = 0;
            }

            if (Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"]);
            }

            if (Convert.ToString(Request.QueryString["FechaIngresoFin"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoFin"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"]);
            }
            //Separar logica de unidad minera
            //Obtener el CHAR del usuario actual
            var charUsuario = dbe.ValidaUsuario(IdPersEnti).ToList()[0].ID_CHAR_PERS;
            int charActual = (int)charUsuario;
            List<int> listaChart = new List<int>();
            bool recorrioChart = false;
            while(recorrioChart == false)
            {
                int valorActual = charActual;
                int nuevoValor = obtenerChartParent(charActual);
                listaChart.Add(nuevoValor);
                charActual = nuevoValor;
                if(charActual == -1)
                {
                    recorrioChart = true;
                }
            }
            //Nodo de unidad minera es 3
            if (listaChart.Contains(3))
            {
                //es unidad minera
                //Validar si puede aprobar, en caso que no, devuelve vacio
                List<AprobadorUM> resultadoValidacion = dbe.AprobadorUM.Where(x => x.ID_PERS_ENTI == IdPersEnti).ToList();
                if(resultadoValidacion.Count != 0)
                {
                    //Habilitado para aprobar
                    if (resultadoValidacion[0].puedeAprobar == 1)
                    {
                        //Validar si vera todos los subniveles o solo asignados
                        if (resultadoValidacion[0].TodosSubNiveles == 1)
                        {
                            var resultado = dbe.ListarSolicitudPersonalPendiente(Estado, IdGerencia, Area, Solicitante, FechaInicio, FechaFin, IdPersEnti).ToList();
                            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var result = dbe.ListarSolicitudPersonalPendienteUM(Estado, IdGerencia, Area, Solicitante, FechaInicio, FechaFin, IdPersEnti).ToList();
                            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //var result = new ListarSolicitudPersonalPendiente_Result();
                        var result = dbe.ListarSolicitudPersonalPendiente(Estado, IdGerencia, Area, Solicitante, FechaInicio, FechaFin, IdPersEnti).ToList();
                        return Json(new { data = result }, JsonRequestBehavior.AllowGet); ;
                    }
                   
                    
                }
                else
                {
                    //var result = new ListarSolicitudPersonalPendiente_Result();
                    var result = dbe.ListarSolicitudPersonalPendiente(Estado, IdGerencia, Area, Solicitante, FechaInicio, FechaFin, IdPersEnti).ToList();
                    return Json(new { data = result }, JsonRequestBehavior.AllowGet); ;
                }
                
            }
            else
            {
                var result = dbe.ListarSolicitudPersonalPendiente(Estado, IdGerencia, Area, Solicitante, FechaInicio, FechaFin, IdPersEnti).ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            

            

        }
        public int obtenerChartParent(int chart)
        {
            try
            {
                var chartVal = dbe.CHARTs.Where(x => x.ID_CHAR == chart).ToList()[0].ID_CHAR_PARE.Value;
                return chartVal;
            }
            catch (Exception e)
            {
                return -1;
            }
            
        }

        public ActionResult ListadoSolicitudHistorica()
        {
            int sw = 0, Error = 0, Estado = 0, IdGerencia = 0, Area = 0, Solicitante = 0;
            string msjError = string.Empty;
            DateTime? FechaInicio = null, FechaFin = null;
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (int.TryParse(Convert.ToString(Request.Params["IdEstado"]), out Estado) == false)
            {
                Estado = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdGerencia"]), out IdGerencia) == false)
            {
                IdGerencia = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdArea"]), out Area) == false)
            {
                Area = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdSolicitante"]), out Solicitante) == false)
            {
                Solicitante = 0;
            }

            if (Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"]);
            }

            if (Convert.ToString(Request.QueryString["FechaIngresoFin"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoFin"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"]);
            }
            var result = dbe.ListarSolicitudHistoricas(Estado, IdGerencia, Area, Solicitante, FechaInicio, FechaFin, IdPersEnti).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }
        
        //cd
        public FileResult DescargarFormato(string archivo = "")
        {
            try
            {
                //var dataRepository = new Servicio.Costo();
                //var newRegistro = dataRepository.ListarValoresParametro("C:\DATA");
                //string rutaaAdjuntos = Convert.ToString(newRegistro.FirstOrDefault().Mensaje);


                string rutaAdjuntos = Server.MapPath("~/Adjuntos/Talent/SolicitudPersonal/");


                archivo = "FORMATO DE PERFIL DE PUESTO";

                int num = (new Random().Next(10000, 99999));
                var Response = HttpContext.Response;

                //Obtener Ruta de Archivo y bytearray
                string path = rutaAdjuntos + archivo + ".docx";
                string contentType = "";

                //if (System.IO.Directory.Exists(path))
                //{

                //}
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);

                MemoryStream ms = new MemoryStream(fileBytes, 0, 0, true, true);
                Response.AddHeader("content-disposition", "attachment;filename=" + archivo + ".docx");
                Response.Buffer = true;
                Response.Clear();
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                Response.OutputStream.Flush();
                Response.End();

                contentType = "application/vnd.openxmlformats";

                return new FileStreamResult(Response.OutputStream, contentType);
            }
            catch(Exception e)
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));
                return File(stream, "text/plain", "Error.txt");
            }
        }
        //

    }
}