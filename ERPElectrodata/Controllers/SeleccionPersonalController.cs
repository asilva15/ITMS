using ERPElectrodata.Models;
using ERPElectrodata.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text;


namespace ERPElectrodata.Controllers
{
    public class SeleccionPersonalController : Controller
    {
        //
        // GET: /SeleccionPersonal/
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities db = new CoreEntities();
        public ActionResult Registro()
        {
            return View();

        }

        public ActionResult CrearCandidato(int id = 0)
        {
            //EnviarCorreoSolicitud(5644); de prueba
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
                if (eva == "ADMINISTRADOR_SISTEMA")
                {
                    ViewBag.ACCESO = 1;
                }
                if (eva == "OPERACIONES")
                {
                    ViewBag.ACCESO = 2;
                }
                if (eva == "RRHH" || eva == "SUPERVISOR_RRHH")
                {
                    ViewBag.ACCESO = 3;
                }
            }
            ViewBag.Id = id;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearCandidato(IEnumerable<HttpPostedFileBase> files, Candidato objCandidato)
        {
            int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            int flag = 0;

            if (objCandidato.IdProfesion <= 0 || objCandidato.IdProfesion == null) { flag = 1; }
            //if (String.IsNullOrEmpty(objCandidato.CurriculumVitae)) { flag = 2; }
            if (String.IsNullOrEmpty(objCandidato.Nombres)) { flag = 3; }
            if (String.IsNullOrEmpty(objCandidato.Apellidos)) { flag = 4; }
            if (objCandidato.PretencionSalarial == 0 || objCandidato.PretencionSalarial == null) { flag = 5; }
            if (String.IsNullOrEmpty(objCandidato.Estudios)) { flag = 6; }
            //if (String.IsNullOrEmpty(objCandidato.ComentarioJefe)) { flag = 7; }



            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            //objCandidato.CurriculumVitae = Path.GetFileNameWithoutExtension(files.FileName);
            //objCandidato.CurriculumVitae = Path.GetExtension(files.FileName);
            objCandidato.IdEstadoCandidato = 1;
            objCandidato.IdSolicitudPersonal = Convert.ToInt32(Request.Params["id"]);
            objCandidato.FechaCrea = DateTime.Now;
            objCandidato.UsuarioCrea = IdUsuario;
            objCandidato.Estado = true;

            foreach (var file in files)
            {
                if (file != null)
                {
                    try
                    {
                        //var ATTA = new PERSON_DOCUMENTS();
                        String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                                RegexOptions.None, TimeSpan.FromSeconds(1.5));
                        var NAM_ATTA = doc;
                        var EXT_ATTA = Path.GetExtension(file.FileName);

                        file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Candidatos/" + NAM_ATTA + "_" + EXT_ATTA));

                        objCandidato.CurriculumVitae = "/SeleccionSolicitudPersonal/VerDocumento/" + NAM_ATTA + "_" + EXT_ATTA;
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                    }
                }
            }

            dbe.Candidato.Add(objCandidato);
            dbe.SaveChanges();
            //Si los combos son null
            var nulo = false;

            if (Request.Params["ResponsabilidadPuntualidad"] == null)
            {
                nulo = true;
            }
            else
            {

                if (Request.Params["ResponsabilidadPuntualidad"].ToString() != "")
                {
                    var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                    objPuntajeCaracteristicas.IdCandidato = objCandidato.Id;
                    objPuntajeCaracteristicas.IdCaracteristicas = 1;
                    objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["ResponsabilidadPuntualidad"].ToString());
                    objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                    objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    objPuntajeCaracteristicas.FechaModifica = null;
                    objPuntajeCaracteristicas.UsuarioModifica = null;
                    objPuntajeCaracteristicas.Estado = true;
                    dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                    dbe.SaveChanges();
                }

            }
            if (Request.Params["Presentacion"] == null)
            {
                nulo = true;
            }
            else
            {

                if (Request.Params["Presentacion"].ToString() != "")
                {
                    var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                    objPuntajeCaracteristicas.IdCandidato = objCandidato.Id;
                    objPuntajeCaracteristicas.IdCaracteristicas = 2;
                    objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["Presentacion"].ToString());
                    objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                    objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    objPuntajeCaracteristicas.FechaModifica = null;
                    objPuntajeCaracteristicas.UsuarioModifica = null;
                    objPuntajeCaracteristicas.Estado = true;
                    dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                    dbe.SaveChanges();
                }

            }

            if (Request.Params["Personalidad"] == null)
            {
                nulo = true;
            }
            else
            {

                if (Request.Params["Personalidad"].ToString() != "")
                {
                    var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                    objPuntajeCaracteristicas.IdCandidato = objCandidato.Id;
                    objPuntajeCaracteristicas.IdCaracteristicas = 3;
                    objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["Personalidad"].ToString());
                    objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                    objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    objPuntajeCaracteristicas.FechaModifica = null;
                    objPuntajeCaracteristicas.UsuarioModifica = null;
                    objPuntajeCaracteristicas.Estado = true;
                    dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                    dbe.SaveChanges();
                }

            }

            if (Request.Params["HabilidadExpresion"] == null)
            {
                nulo = true;
            }
            else
            {

                if (Request.Params["HabilidadExpresion"].ToString() != "")
                {
                    var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                    objPuntajeCaracteristicas.IdCandidato = objCandidato.Id;
                    objPuntajeCaracteristicas.IdCaracteristicas = 5;
                    objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["HabilidadExpresion"].ToString());
                    objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                    objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    objPuntajeCaracteristicas.FechaModifica = null;
                    objPuntajeCaracteristicas.UsuarioModifica = null;
                    objPuntajeCaracteristicas.Estado = true;
                    dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                    dbe.SaveChanges();
                }

            }

            if (Request.Params["Referencias"] == null)
            {
                nulo = true;
            }
            else
            {

                if (Request.Params["Referencias"].ToString() != "")
                {
                    var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                    objPuntajeCaracteristicas.IdCandidato = objCandidato.Id;
                    objPuntajeCaracteristicas.IdCaracteristicas = 4;
                    objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["Referencias"].ToString());
                    objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                    objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    objPuntajeCaracteristicas.FechaModifica = null;
                    objPuntajeCaracteristicas.UsuarioModifica = null;
                    objPuntajeCaracteristicas.Estado = true;
                    dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                    dbe.SaveChanges();
                }

            }

            //files.SaveAs(Server.MapPath("~/Adjuntos/Asset/"));

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','0','" + objCandidato.Id.ToString() + "');}window.onload=init;</script>");

        }

        public ActionResult EditarCandidato(int id = 0)
        {
            var IdUser = Convert.ToInt32(Session["UserId"].ToString());
            var IdUsuario = (from ex in dbe.Usuarios where ex.UserId == IdUser select ex).First().Id;
            var VerificarPermiso = (from u in dbe.UsuarioPerfils
                                    where u.IdUsuario == IdUsuario
                                    orderby u.Perfil.Perfil1
                                    select new
                                    {
                                        per = u.Perfil

                                    }).ToList();
            var IdSolicitud = (from ex in dbe.Candidato where ex.Id == id select ex).First().IdSolicitudPersonal;
            var UsuarioSolicita = (from ex in dbe.SolicitudPersonal where ex.Id == IdSolicitud select ex).First().UsuarioCrea;
            if (Convert.ToInt32(Session["ID_PERS_ENTI"].ToString()) == UsuarioSolicita)
            {
                ViewBag.ACCESO = 2;

            }
            foreach (var xc in VerificarPermiso)
            {
                string eva = xc.per.Perfil1;
                if (eva == "ADMINISTRADOR_SISTEMA" || IdUser == 208 || IdUser == 35)
                {
                    ViewBag.ACCESO = 1;
                }
                if (eva == "OPERACIONES")
                {
                    ViewBag.ACCESO = 2;
                }
                if (eva == "RRHH")
                {
                    ViewBag.ACCESO = 3;
                }
            }

             var UsuarioSolicitante = (from ex in dbe.SolicitudPersonal where ex.Id == IdSolicitud select ex).First().UsuarioCrea;
             if (Convert.ToInt32(Session["ID_PERS_ENTI"].ToString()) == UsuarioSolicita)
             {
                 ViewBag.Solicitante1 = 1;
             }


            ViewBag.Id = id;



            var c = dbe.Candidato.Find(id);
            var p = dbe.Profesions.Find(c.IdProfesion);
            int[] idgth = { 1, 2, 3, 4, 5 };
            int[] idjefe = { 6, 7, 8 };
            var pgth = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == id && idgth.Contains((int)x.IdCaracteristicas)).Sum(o => o.Puntaje);
            var pgjefe = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == id && idjefe.Contains((int)x.IdCaracteristicas)).Sum(o => o.Puntaje);
            ViewBag.Curriculum = c.CurriculumVitae;
            ViewBag.Nombres = c.Nombres;
            ViewBag.Apellidos = c.Apellidos;
            ViewBag.Profesion = p.Id;
            ViewBag.PretencionSalarial = c.PretencionSalarial;
            ViewBag.Estudios = c.Estudios;
            ViewBag.Observacion = c.Observacion;
            ViewBag.ComentarioJefe = c.ComentarioJefe;
            var pPuntualidad = dbe.CaracteristicasCandidato.Where(x => x.IdCaracteristicas == 1 && x.IdCandidato == id).FirstOrDefault();
            var pPresentacion = dbe.CaracteristicasCandidato.Where(x => x.IdCaracteristicas == 2 && x.IdCandidato == id).FirstOrDefault();
            var pPersonalidad = dbe.CaracteristicasCandidato.Where(x => x.IdCaracteristicas == 3 && x.IdCandidato == id).FirstOrDefault();
            var pReferencia = dbe.CaracteristicasCandidato.Where(x => x.IdCaracteristicas == 4 && x.IdCandidato == id).FirstOrDefault();
            var pHabilidad = dbe.CaracteristicasCandidato.Where(x => x.IdCaracteristicas == 5 && x.IdCandidato == id).FirstOrDefault();
            var pExperiencia = dbe.CaracteristicasCandidato.Where(x => x.IdCaracteristicas == 6 && x.IdCandidato == id).FirstOrDefault();
            var pConocimiento = dbe.CaracteristicasCandidato.Where(x => x.IdCaracteristicas == 7 && x.IdCandidato == id).FirstOrDefault();
            var pHabilidades = dbe.CaracteristicasCandidato.Where(x => x.IdCaracteristicas == 8 && x.IdCandidato == id).FirstOrDefault();
            if (pPuntualidad == null)
            {
                ViewBag.pPuntualidad = 0;
            }
            else
            {
                ViewBag.pPuntualidad = pPuntualidad.Puntaje;
            }
            if (pPresentacion == null)
            {
                ViewBag.pPresentacion = 0;
            }
            else
            {
                ViewBag.pPresentacion = pPresentacion.Puntaje;
            }
            if (pPersonalidad == null)
            {
                ViewBag.pPersonalidad = 0;
            }
            else
            {
                ViewBag.pPersonalidad = pPersonalidad.Puntaje;
            }
            if (pReferencia == null)
            {
                ViewBag.pReferencia = 0;
            }
            else
            {
                ViewBag.pReferencia = pReferencia.Puntaje;
            }
            if (pHabilidad == null)
            {
                ViewBag.pHabilidad = 0;
            }
            else
            {
                ViewBag.pHabilidad = pHabilidad.Puntaje;
            }
            if (pExperiencia == null)
            {
                ViewBag.pExperiencia = 0;
            }
            else
            {
                ViewBag.pExperiencia = pExperiencia.Puntaje;
            }
            if (pConocimiento == null)
            {
                ViewBag.pConocimiento = 0;
            }
            else
            {
                ViewBag.pConocimiento = pConocimiento.Puntaje;
            }
            if (pHabilidades == null)
            {
                ViewBag.pHabilidades = 0;
            }
            else
            {
                ViewBag.pHabilidades = pHabilidades.Puntaje;
            }
            if (pgth == null)
            {
                ViewBag.PuntajeGTH = 0;
                ViewBag.SumGTH = 0;
            }
            else
            {
                ViewBag.SumGTH = pgth;
                ViewBag.PuntajeGTH = Convert.ToDecimal(pgth / 5.0);
            }
            if (pgjefe == null)
            {
                ViewBag.PuntajeJefe = 0;
                ViewBag.SumJefe = 0;
            }
            else
            {
                ViewBag.SumJefe = pgjefe;
                ViewBag.PuntajeJefe = Math.Round(Convert.ToDecimal(pgjefe / 3.0), 2);
            }
            if (pgjefe == null && pgth == null)
            {
                ViewBag.SumTotal = 0;
                ViewBag.PuntajeTotal = 0;
            }
            else
            {
                ViewBag.SumTotal = (pgjefe + pgth) / 2.0;
                ViewBag.PuntajeTotal = Math.Round((ViewBag.PuntajeGTH + ViewBag.PuntajeJefe) / 2, 2);
            }

            return View(c);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditarCandidato(IEnumerable<HttpPostedFileBase> files, Candidato objCandidt)
        {
            //Validar campos en blanco
            int flag = 0;

            if (Request.Params["ID_PROFESION1"].ToString() == null) { flag = 1; }
            else { objCandidt.IdProfesion = Convert.ToInt32(Request.Params["ID_PROFESION1"]); }

            int Profesion = Convert.ToInt32(Request.Params["ID_PROFESION1"]);



            if (Convert.ToString(objCandidt.Nombres) == null) { flag = 2; }
            if (Convert.ToString(objCandidt.Apellidos) == null) { flag = 3; }
            if (Convert.ToString(objCandidt.PretencionSalarial) == null) { flag = 4; }
            if (Convert.ToString(objCandidt.Estudios) == null) { flag = 5; }
            
            if (Convert.ToString(Profesion) == null) { flag = 7; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }


            if (ModelState.IsValid)
            {
                Convert.ToBoolean(objCandidt.Estado);

                objCandidt.IdSolicitudPersonal = Convert.ToInt32(Request.Params["Id"]);

                var objCandidato = dbe.Candidato.Find(objCandidt.Id);
                objCandidato.Id = objCandidt.Id;
                objCandidato.IdProfesion = objCandidt.IdProfesion;

                objCandidato.Nombres = objCandidt.Nombres;
                objCandidato.Apellidos = objCandidt.Apellidos;
                objCandidato.PretencionSalarial = objCandidt.PretencionSalarial;
                objCandidato.Estudios = objCandidt.Estudios;
                objCandidato.Observacion = objCandidt.Observacion;
                objCandidato.ComentarioJefe = objCandidt.ComentarioJefe;
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        try
                        {
                            //var ATTA = new PERSON_DOCUMENTS();
                            String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                                    RegexOptions.None, TimeSpan.FromSeconds(1.5));
                            var NAM_ATTA = doc;
                            var EXT_ATTA = Path.GetExtension(file.FileName);

                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Candidatos/" + NAM_ATTA + "_" + EXT_ATTA));

                            objCandidato.CurriculumVitae = "/SeleccionSolicitudPersonal/VerDocumento/" + NAM_ATTA + "_" + EXT_ATTA;
                        }
                        catch
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                        }
                    }
                }
                dbe.Candidato.Attach(objCandidato);
                dbe.Entry(objCandidato).State = EntityState.Modified;
                dbe.SaveChanges();
                //Si los combos son null
                var nulo = false;

                if (Request.Params["ResponsabilidadPuntualidad1"] == null)
                {
                    nulo = true;
                }
                else
                {
                    var existe = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 1).Count();
                    if (existe == 0)
                    {
                        if (Request.Params["ResponsabilidadPuntualidad1"].ToString() != "")
                        {
                            var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                            objPuntajeCaracteristicas.IdCandidato = objCandidt.Id;
                            objPuntajeCaracteristicas.IdCaracteristicas = 1;
                            objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["ResponsabilidadPuntualidad1"].ToString());
                            objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                            objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                            objPuntajeCaracteristicas.FechaModifica = null;
                            objPuntajeCaracteristicas.UsuarioModifica = null;
                            objPuntajeCaracteristicas.Estado = true;
                            dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                            dbe.SaveChanges();
                        }
                    }
                    else
                    {
                        var idcc = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 1).FirstOrDefault();

                        var objPuntajeCaracteristica = dbe.CaracteristicasCandidato.Find(idcc.Id);
                        objPuntajeCaracteristica.Id = idcc.Id;
                        objPuntajeCaracteristica.IdCandidato = objCandidt.Id;
                        objPuntajeCaracteristica.IdCaracteristicas = objPuntajeCaracteristica.IdCaracteristicas;
                        objPuntajeCaracteristica.Puntaje = Convert.ToInt32(Request.Params["ResponsabilidadPuntualidad1"].ToString());
                        objPuntajeCaracteristica.FechaCrea = objPuntajeCaracteristica.FechaCrea;
                        objPuntajeCaracteristica.UsuarioCrea = objPuntajeCaracteristica.UsuarioCrea;
                        objPuntajeCaracteristica.FechaModifica = DateTime.Now;
                        objPuntajeCaracteristica.UsuarioModifica = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        objPuntajeCaracteristica.Estado = objPuntajeCaracteristica.Estado;
                        dbe.Candidato.Attach(objCandidato);
                        dbe.Entry(objCandidato).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                if (Request.Params["Presentacion1"] == null)
                {
                    nulo = true;
                }
                else
                {
                    var existe1 = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 2).Count();
                    if (existe1 == 0)
                    {
                        if (Request.Params["Presentacion1"].ToString() != "")
                        {
                            var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                            objPuntajeCaracteristicas.IdCandidato = objCandidt.Id;
                            objPuntajeCaracteristicas.IdCaracteristicas = 2;
                            objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["Presentacion1"].ToString());
                            objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                            objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                            objPuntajeCaracteristicas.FechaModifica = null;
                            objPuntajeCaracteristicas.UsuarioModifica = null;
                            objPuntajeCaracteristicas.Estado = true;
                            dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                            dbe.SaveChanges();
                        }
                    }
                    else
                    {
                        var idcc = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 2).FirstOrDefault();

                        var objPuntajeCaracteristica = dbe.CaracteristicasCandidato.Find(idcc.Id);
                        objPuntajeCaracteristica.Id = idcc.Id;
                        objPuntajeCaracteristica.IdCandidato = objCandidt.Id;
                        objPuntajeCaracteristica.IdCaracteristicas = objPuntajeCaracteristica.IdCaracteristicas;
                        objPuntajeCaracteristica.Puntaje = Convert.ToInt32(Request.Params["Presentacion1"].ToString());
                        objPuntajeCaracteristica.FechaCrea = objPuntajeCaracteristica.FechaCrea;
                        objPuntajeCaracteristica.UsuarioCrea = objPuntajeCaracteristica.UsuarioCrea;
                        objPuntajeCaracteristica.FechaModifica = DateTime.Now;
                        objPuntajeCaracteristica.UsuarioModifica = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        objPuntajeCaracteristica.Estado = objPuntajeCaracteristica.Estado;
                        dbe.Candidato.Attach(objCandidato);
                        dbe.Entry(objCandidato).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                if (Request.Params["Personalidad1"] == null)
                {
                    nulo = true;
                }
                else
                {
                    var existe2 = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 3).Count();
                    if (existe2 == 0)
                    {
                        if (Request.Params["Personalidad1"].ToString() != "")
                        {
                            var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                            objPuntajeCaracteristicas.IdCandidato = objCandidt.Id;
                            objPuntajeCaracteristicas.IdCaracteristicas = 3;
                            objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["Personalidad1"].ToString());
                            objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                            objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                            objPuntajeCaracteristicas.FechaModifica = null;
                            objPuntajeCaracteristicas.UsuarioModifica = null;
                            objPuntajeCaracteristicas.Estado = true;
                            dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                            dbe.SaveChanges();
                        }
                    }
                    else
                    {
                        var idcc = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 3).FirstOrDefault();

                        var objPuntajeCaracteristica = dbe.CaracteristicasCandidato.Find(idcc.Id);
                        objPuntajeCaracteristica.Id = idcc.Id;
                        objPuntajeCaracteristica.IdCandidato = objCandidt.Id;
                        objPuntajeCaracteristica.IdCaracteristicas = objPuntajeCaracteristica.IdCaracteristicas;
                        objPuntajeCaracteristica.Puntaje = Convert.ToInt32(Request.Params["Personalidad1"].ToString());
                        objPuntajeCaracteristica.FechaCrea = objPuntajeCaracteristica.FechaCrea;
                        objPuntajeCaracteristica.UsuarioCrea = objPuntajeCaracteristica.UsuarioCrea;
                        objPuntajeCaracteristica.FechaModifica = DateTime.Now;
                        objPuntajeCaracteristica.UsuarioModifica = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        objPuntajeCaracteristica.Estado = objPuntajeCaracteristica.Estado;
                        dbe.Candidato.Attach(objCandidato);
                        dbe.Entry(objCandidato).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                if (Request.Params["HabilidadExpresion1"] == null)
                {
                    nulo = true;
                }
                else
                {
                    var existe3 = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 5).Count();
                    if (existe3 == 0)
                    {
                        if (Request.Params["HabilidadExpresion1"].ToString() != "")
                        {
                            var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                            objPuntajeCaracteristicas.IdCandidato = objCandidt.Id;
                            objPuntajeCaracteristicas.IdCaracteristicas = 5;
                            objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["HabilidadExpresion1"].ToString());
                            objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                            objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                            objPuntajeCaracteristicas.FechaModifica = null;
                            objPuntajeCaracteristicas.UsuarioModifica = null;
                            objPuntajeCaracteristicas.Estado = true;
                            dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                            dbe.SaveChanges();
                        }
                    }
                    else
                    {
                        var idcc = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 5).FirstOrDefault();

                        var objPuntajeCaracteristica = dbe.CaracteristicasCandidato.Find(idcc.Id);
                        objPuntajeCaracteristica.Id = idcc.Id;
                        objPuntajeCaracteristica.IdCandidato = objCandidt.Id;
                        objPuntajeCaracteristica.IdCaracteristicas = objPuntajeCaracteristica.IdCaracteristicas;
                        objPuntajeCaracteristica.Puntaje = Convert.ToInt32(Request.Params["HabilidadExpresion1"].ToString());
                        objPuntajeCaracteristica.FechaCrea = objPuntajeCaracteristica.FechaCrea;
                        objPuntajeCaracteristica.UsuarioCrea = objPuntajeCaracteristica.UsuarioCrea;
                        objPuntajeCaracteristica.FechaModifica = DateTime.Now;
                        objPuntajeCaracteristica.UsuarioModifica = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        objPuntajeCaracteristica.Estado = objCandidato.Estado;
                        dbe.Candidato.Attach(objCandidato);
                        dbe.Entry(objCandidato).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                if (Request.Params["Referencias1"] == null)
                {
                    nulo = true;
                }
                else
                {
                    var existe4 = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 4).Count();
                    if (existe4 == 0)
                    {
                        if (Request.Params["Referencias1"].ToString() != "")
                        {
                            var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                            objPuntajeCaracteristicas.IdCandidato = objCandidt.Id;
                            objPuntajeCaracteristicas.IdCaracteristicas = 4;
                            objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["Referencias1"].ToString());
                            objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                            objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                            objPuntajeCaracteristicas.FechaModifica = null;
                            objPuntajeCaracteristicas.UsuarioModifica = null;
                            objPuntajeCaracteristicas.Estado = true;
                            dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                            dbe.SaveChanges();
                        }
                    }
                    else
                    {
                        var idcc = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 4).FirstOrDefault();

                        var objPuntajeCaracteristica = dbe.CaracteristicasCandidato.Find(idcc.Id);
                        objPuntajeCaracteristica.Id = idcc.Id;
                        objPuntajeCaracteristica.IdCandidato = objCandidt.Id;
                        objPuntajeCaracteristica.IdCaracteristicas = objPuntajeCaracteristica.IdCaracteristicas;
                        objPuntajeCaracteristica.Puntaje = Convert.ToInt32(Request.Params["Referencias1"].ToString());
                        objPuntajeCaracteristica.FechaCrea = objPuntajeCaracteristica.FechaCrea;
                        objPuntajeCaracteristica.UsuarioCrea = objPuntajeCaracteristica.UsuarioCrea;
                        objPuntajeCaracteristica.FechaModifica = DateTime.Now;
                        objPuntajeCaracteristica.UsuarioModifica = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        objPuntajeCaracteristica.Estado = objCandidato.Estado;
                        dbe.Candidato.Attach(objCandidato);
                        dbe.Entry(objCandidato).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                if (Request.Params["ExperienciaLaboral"] == null)
                {
                    nulo = true;
                }
                else
                {
                    var existe5 = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 6).Count();
                    if (existe5 == 0)
                    {
                        if (Request.Params["ExperienciaLaboral"].ToString() != "")
                        {
                            var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                            objPuntajeCaracteristicas.IdCandidato = objCandidt.Id;
                            objPuntajeCaracteristicas.IdCaracteristicas = 6;
                            objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["ExperienciaLaboral"].ToString());
                            objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                            objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                            objPuntajeCaracteristicas.FechaModifica = null;
                            objPuntajeCaracteristicas.UsuarioModifica = null;
                            objPuntajeCaracteristicas.Estado = true;
                            dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                            dbe.SaveChanges();
                        }
                    }
                    else
                    {
                        var idcc = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 6).FirstOrDefault();

                        var objPuntajeCaracteristica = dbe.CaracteristicasCandidato.Find(idcc.Id);
                        objPuntajeCaracteristica.Id = idcc.Id;
                        objPuntajeCaracteristica.IdCandidato = objCandidt.Id;
                        objPuntajeCaracteristica.IdCaracteristicas = objPuntajeCaracteristica.IdCaracteristicas;
                        objPuntajeCaracteristica.Puntaje = Convert.ToInt32(Request.Params["ExperienciaLaboral"].ToString());
                        objPuntajeCaracteristica.FechaCrea = objPuntajeCaracteristica.FechaCrea;
                        objPuntajeCaracteristica.UsuarioCrea = objPuntajeCaracteristica.UsuarioCrea;
                        objPuntajeCaracteristica.FechaModifica = DateTime.Now;
                        objPuntajeCaracteristica.UsuarioModifica = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        objPuntajeCaracteristica.Estado = objCandidato.Estado;
                        dbe.Candidato.Attach(objCandidato);
                        dbe.Entry(objCandidato).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                if (Request.Params["Conocimientos"] == null)
                {
                    nulo = true;
                }
                else
                {
                    var existe6 = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 7).Count();
                    if (existe6 == 0)
                    {
                        if (Request.Params["Conocimientos"].ToString() != "")
                        {
                            var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                            objPuntajeCaracteristicas.IdCandidato = objCandidt.Id;
                            objPuntajeCaracteristicas.IdCaracteristicas = 7;
                            objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["Conocimientos"].ToString());
                            objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                            objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                            objPuntajeCaracteristicas.FechaModifica = null;
                            objPuntajeCaracteristicas.UsuarioModifica = null;
                            objPuntajeCaracteristicas.Estado = true;
                            dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                            dbe.SaveChanges();
                        }
                    }
                    else
                    {
                        var idcc = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 7).FirstOrDefault();

                        var objPuntajeCaracteristica = dbe.CaracteristicasCandidato.Find(idcc.Id);
                        objPuntajeCaracteristica.Id = idcc.Id;
                        objPuntajeCaracteristica.IdCandidato = objCandidt.Id;
                        objPuntajeCaracteristica.IdCaracteristicas = objPuntajeCaracteristica.IdCaracteristicas;
                        objPuntajeCaracteristica.Puntaje = Convert.ToInt32(Request.Params["Conocimientos"].ToString());
                        objPuntajeCaracteristica.FechaCrea = objPuntajeCaracteristica.FechaCrea;
                        objPuntajeCaracteristica.UsuarioCrea = objPuntajeCaracteristica.UsuarioCrea;
                        objPuntajeCaracteristica.FechaModifica = DateTime.Now;
                        objPuntajeCaracteristica.UsuarioModifica = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        objPuntajeCaracteristica.Estado = objCandidato.Estado;
                        dbe.Candidato.Attach(objCandidato);
                        dbe.Entry(objCandidato).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                if (Request.Params["HabilidadesPotencial"] == null)
                {
                    nulo = true;
                }
                else
                {
                    var existe7 = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 8).Count();
                    if (existe7 == 0)
                    {
                        if (Request.Params["HabilidadesPotencial"].ToString() != "")
                        {
                            var objPuntajeCaracteristicas = new CaracteristicasCandidato();
                            objPuntajeCaracteristicas.IdCandidato = objCandidt.Id;
                            objPuntajeCaracteristicas.IdCaracteristicas = 8;
                            objPuntajeCaracteristicas.Puntaje = Convert.ToInt32(Request.Params["HabilidadesPotencial"].ToString());
                            objPuntajeCaracteristicas.FechaCrea = DateTime.Now;
                            objPuntajeCaracteristicas.UsuarioCrea = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                            objPuntajeCaracteristicas.FechaModifica = null;
                            objPuntajeCaracteristicas.UsuarioModifica = null;
                            objPuntajeCaracteristicas.Estado = true;
                            dbe.CaracteristicasCandidato.Add(objPuntajeCaracteristicas);
                            dbe.SaveChanges();
                        }
                    }
                    else
                    {
                        var idcc = dbe.CaracteristicasCandidato.Where(x => x.IdCandidato == objCandidt.Id && x.IdCaracteristicas == 8).FirstOrDefault();

                        var objPuntajeCaracteristica = dbe.CaracteristicasCandidato.Find(idcc.Id);
                        objPuntajeCaracteristica.Id = idcc.Id;
                        objPuntajeCaracteristica.IdCandidato = objCandidt.Id;
                        objPuntajeCaracteristica.IdCaracteristicas = objPuntajeCaracteristica.IdCaracteristicas;
                        objPuntajeCaracteristica.Puntaje = Convert.ToInt32(Request.Params["HabilidadesPotencial"].ToString());
                        objPuntajeCaracteristica.FechaCrea = objPuntajeCaracteristica.FechaCrea;
                        objPuntajeCaracteristica.UsuarioCrea = objPuntajeCaracteristica.UsuarioCrea;
                        objPuntajeCaracteristica.FechaModifica = DateTime.Now;
                        objPuntajeCaracteristica.UsuarioModifica = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                        objPuntajeCaracteristica.Estado = objCandidato.Estado;
                        dbe.Candidato.Attach(objCandidato);
                        dbe.Entry(objCandidato).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objCandidt.Id.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult EliminarCandidato(int? id = 0)
        {

            ViewBag.Id = id;
            return View();


        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EliminarCandidato(int id = 0)
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                Candidato objCddt = dbe.Candidato.Where(x => x.Id == id).SingleOrDefault();

                objCddt.FechaModifica = DateTime.Now;
                objCddt.UsuarioModifica = UserId;
                objCddt.Estado = false;
                dbe.Candidato.Attach(objCddt);
                dbe.Entry(objCddt).State = EntityState.Modified;
                dbe.SaveChanges();
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','0','" + objCddt.Id.ToString() + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult EnviarCorreoCvCargado(int? id = 0)
        {

            ViewBag.Id = id;
            return View();


        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EnviarCorreoCvCargado(int id = 0)
        {
            try
            {
                var correo = Request.Params["inpCorreo"];
                if (correo == "")
                {
                    correo = null;
                }

                var soli = dbe.SolicitudPersonal.Where(x => x.Id == id).FirstOrDefault();

                var candidatos = dbe.Candidato.Where(o => o.IdSolicitudPersonal == soli.Id).Count();

                

                if (candidatos == 0)
                {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone3) top.uploadDone3('NoCandidatos','0','" + soli.Id.ToString() + "');}window.onload=init;</script>");
                }
                else
                {

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
                    mail.EnviarSolicitudPersonal((int)soli.UsuarioCrea, (int)soli.IdGerencia, 6, soli.Id, GTH, 1, correo);
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone3) top.uploadDone3('OK','0','" + soli.Id.ToString() + "');}window.onload=init;</script>");
                }
                
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult RechazarSolicitud(int? id = 0)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RechazarSolicitud(int id = 0)
        {
            try
            {
                int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

                var objSolicitudRechazado = new SolicitudRechazado();
                objSolicitudRechazado.IdSolicitudPersonal = id;
                objSolicitudRechazado.IdMotivoRechazo = Convert.ToInt32(Request.Params["cbMotivo"]);
                objSolicitudRechazado.FechaCrea = DateTime.Now;
                objSolicitudRechazado.UsuarioCrea = IdUsuario;
                objSolicitudRechazado.Estado = true;

                var solicitud = dbe.SolicitudPersonal.Where(x => x.Id == id).SingleOrDefault();
                solicitud.IdEstadoSolicitud = 4;
                solicitud.FechaModifica = DateTime.Now;
                solicitud.UsuarioModifica = IdUsuario;
                dbe.Entry(solicitud).State = EntityState.Modified;
                solicitud.IdAprobacion = IdUsuario;
                dbe.SaveChanges();

                try
                {
                    var soli = dbe.SolicitudPersonal.Where(x => x.Id == id).FirstOrDefault();
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
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone6) top.uploadDone6('ConfirmoRechazoSolicitud','Se aprobo la solicitud correctamente.');}window.onload=init;</script>");

            }
            catch (Exception)
            {

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone6) top.uploadDone6('ERROR','');}window.onload=init;</script>");
            }
                     
        }
        
        public ActionResult AprobarSolicitud(int? id = 0)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AprobarSolicitud(int id = 0)
        {


            try
            {
                int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

                var solicitud = dbe.SolicitudPersonal.Where(x => x.Id == id).SingleOrDefault();
                solicitud.IdEstadoSolicitud = 6;
                solicitud.FechaModifica = DateTime.Now;
                solicitud.UsuarioModifica = IdUsuario;
                dbe.Entry(solicitud).State = EntityState.Modified;
                solicitud.IdAprobacion = IdUsuario;
                dbe.SaveChanges();



                try
                {
                    var soli = dbe.SolicitudPersonal.Where(x => x.Id == id).FirstOrDefault();
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
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone7) top.uploadDone7('ConfirmoAproboSolicitud','Se aprobo la solicitud correctamente.');}window.onload=init;</script>");

            }
            catch (Exception)
            {

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone7) top.uploadDone7('ERROR','');}window.onload=init;</script>");
            }

        
        }



        [HttpPost]
        public ActionResult EnviarCorreoSolicitud(int idSolicitud)
        {
            try
            {
                var detalleSolicitud = dbe.EnviarConfirmacionSolicitud(idSolicitud).FirstOrDefault();

                if (detalleSolicitud != null)
                {
                    StringBuilder htmlBody = new StringBuilder();

                    htmlBody.Append("Estimados:<br><br>");
                    htmlBody.Append("Buen día, se notifica el siguiente ingreso:<br><br>");

                    htmlBody.Append("<table style='border-collapse: collapse; width: 70%;'>");

                    // Fila 1: Candidatos
                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Cargo</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.Cargo);
                    htmlBody.Append("</tr>");


                    // Fila 2: Candidatos
                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Candidatos</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.candidatos);
                    htmlBody.Append("</tr>");

                    // Fila 3: Área
                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Área</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.Area);
                    htmlBody.Append("</tr>");

                    // Fila 4: Fecha Inicio Trabajo
                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Fecha Inicio Trabajo</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.FechaInicioTrabajo);
                    htmlBody.Append("</tr>");

                    // Fila 5: Jefe Inmediato
                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Jefe Inmediato</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.JefeInmediato);
                    htmlBody.Append("</tr>");

                    htmlBody.Append("</table>");

                    htmlBody.Append("<br>Por favor, su apoyo con lo siguiente:<br>");



                    htmlBody.Append("<br><table style='border-collapse: collapse; width: 70%;'>");


                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Recursos Informaticos</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.RecursoInformatico);
                    htmlBody.Append("</tr>");


                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Recursos de Comunicación</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.RecursoComunicacion);
                    htmlBody.Append("</tr>");
                    

                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Recursos de Red</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.Red);
                    htmlBody.Append("</tr>");


                    htmlBody.Append("<tr>");
                    htmlBody.Append("<td style='background-color: #3e5060; color: white; font-weight: bold; width: 50%; border: 1px solid #ddd; padding: 8px;'>Carpeta Accesible</td>");
                    htmlBody.AppendFormat("<td style='border: 1px solid #ddd; padding: 8px;'>{0}</td>", detalleSolicitud.Carpeta);
                    htmlBody.Append("</tr>");

                    htmlBody.Append("</table>");

                    htmlBody.Append("<br>Ante cualquier duda o consulta, nos comentan. <br>Gracias.");

                    

                    var correo = db.ACCOUNT_PARAMETER
                        .Where(account => (account.ID_PARA == 24) && account.ID_ACCO == 4)
                        .Select(account => account.VAL_ACCO_PARA)
                        .ToList().FirstOrDefault();

                    var contraseña = db.ACCOUNT_PARAMETER
                        .Where(account => (account.ID_PARA == 25) && account.ID_ACCO == 4)
                        .Select(account => account.VAL_ACCO_PARA)
                        .ToList().FirstOrDefault();


                    using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                    {
                        client.Port = 587;
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(correo, contraseña);

                        using (MailMessage message = new MailMessage())
                        {
                            message.From = new MailAddress(correo);
                            message.Subject = "Nuevo Ingreso - Cargo: " + detalleSolicitud.Cargo;
                            message.Body = htmlBody.ToString();
                            message.IsBodyHtml = true;
                            message.To.Add("servicedesk@electrodata.com.pe");

                            client.Send(message);
                        }
                    }

                    return Json(new { success = true, message = "Correos electrónicos enviados correctamente" });
                }
                else
                {
                    return Json(new { success = false, message = "No se encontraron detalles de solicitud para el ID proporcionado" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al enviar los correos electrónicos: " + ex.Message });
            }
        }






        public ActionResult  ReferenciaCandidato(int id = 0)
        {
            var VerBoton = 0;
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
                if (eva == "ADMINISTRADOR_SISTEMA" || eva == "RRHH" || IdUser == 208 || IdUser == 35)
                {
                    VerBoton = 1;
                }
            }
            ViewBag.VerBoton = VerBoton;
            ViewBag.Id = id;
            return View();
        }

        public ActionResult CrearReferenciaCandidato(int id = 0)
        {
            var VerBoton = 0;
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
                if (eva == "ADMINISTRADOR_SISTEMA" || eva == "RRHH" || IdUser == 208 || IdUser == 35)
                {
                    VerBoton = 1;
                }
            }
            ViewBag.VerBoton = VerBoton;
            ViewBag.Id = id;
            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CrearReferenciaCandidato(Referencia objReferencia)
        {
            int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            int flag = 0;

            if (String.IsNullOrEmpty(objReferencia.Empresa)) { flag = 1; }
            if (String.IsNullOrEmpty(objReferencia.NombreApellidoEmpleador)) { flag = 2; }
            if (String.IsNullOrEmpty(objReferencia.CargoEmpleador)) { flag = 3; }
            if (String.IsNullOrEmpty(objReferencia.RelacionLaboral)) { flag = 4; }
            if (String.IsNullOrEmpty(objReferencia.CargoCandidato)) { flag = 5; }
            if (String.IsNullOrEmpty(objReferencia.TiempoTrabajadoCandidato)) { flag = 6; }
            if (String.IsNullOrEmpty(objReferencia.MotivoRetiroCandidato)) { flag = 7; }
            if (String.IsNullOrEmpty(objReferencia.CompetenciaSignificativa)) { flag = 8; }
            if (String.IsNullOrEmpty(objReferencia.CompetenciaFortalecer)) { flag = 9; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }


            objReferencia.IdCandidato = Convert.ToInt32(Request.Params["Id"]);
            objReferencia.FechaCrea = DateTime.Now;
            objReferencia.UsuarioCrea = IdUsuario;
            objReferencia.Estado = true;


            if (ModelState.IsValid)
            {
                dbe.Referencia.Add(objReferencia);
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objReferencia.Id.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }

        }


        public ActionResult EditarReferenciaCandidato()
        {
            int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            int Id = Convert.ToInt32(Request.Params["Id"].ToString());
            int flag = 0;

            string Empresa = Convert.ToString(Request.Params["Empresa"]);
            string NombreApellidoEmpleador = Convert.ToString(Request.Params["NombreApellidoEmpleador"]);
            string CargoEmpleador = Convert.ToString(Request.Params["CargoEmpleador"]);
            string RelacionLaboral = Convert.ToString(Request.Params["RelacionLaboral"]);
            string CargoCandidato = Convert.ToString(Request.Params["CargoCandidato"]);
            string TiempoTrabajadoCandidato = Convert.ToString(Request.Params["TiempoTrabajadoCandidato"]);
            string MotivoRetiroCandidato = Convert.ToString(Request.Params["MotivoRetiroCandidato"]);
            string CompetenciaSignificativa = Convert.ToString(Request.Params["CompetenciaSignificativa"]);
            string CompetenciaFortalecer = Convert.ToString(Request.Params["CompetenciaFortalecer"]);
            string VolveriaContratar = Convert.ToString(Request.Params["VolveriaContratar"]);

            if (String.IsNullOrEmpty(Empresa)) { flag = 1; }
            if (String.IsNullOrEmpty(NombreApellidoEmpleador)) { flag = 2; }
            if (String.IsNullOrEmpty(CargoEmpleador)) { flag = 3; }
            if (String.IsNullOrEmpty(RelacionLaboral)) { flag = 4; }
            if (String.IsNullOrEmpty(CargoCandidato)) { flag = 5; }
            if (String.IsNullOrEmpty(TiempoTrabajadoCandidato)) { flag = 6; }
            if (String.IsNullOrEmpty(MotivoRetiroCandidato)) { flag = 7; }
            if (String.IsNullOrEmpty(CompetenciaSignificativa)) { flag = 8; }
            if (String.IsNullOrEmpty(CompetenciaFortalecer)) { flag = 9; }

            if (flag != 0)
            {
                return Content("ERROR");
            }


            var objReferencia = dbe.Referencia.Where(x => x.Id == Id).SingleOrDefault();
            objReferencia.Empresa = Empresa;
            objReferencia.NombreApellidoEmpleador = NombreApellidoEmpleador;
            objReferencia.CargoEmpleador = CargoEmpleador;
            objReferencia.RelacionLaboral = RelacionLaboral;
            objReferencia.CargoCandidato = CargoCandidato;
            objReferencia.TiempoTrabajadoCandidato = TiempoTrabajadoCandidato;
            objReferencia.MotivoRetiroCandidato = MotivoRetiroCandidato;
            objReferencia.VolveriaContratar = Convert.ToBoolean(VolveriaContratar);
            objReferencia.CompetenciaSignificativa = CompetenciaSignificativa;
            objReferencia.CompetenciaFortalecer = CompetenciaFortalecer;
            objReferencia.FechaModifica = DateTime.Now;
            objReferencia.UsuarioModifica = IdUsuario;
            objReferencia.Estado = true;


            if (ModelState.IsValid)
            {
                dbe.Entry(objReferencia).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("OK");
            }
            else
            {
                return Content("ERROR");
            }

        }

        public ActionResult ListarReferencia(int id = 0)
        {

            var query = dbe.ListarReferencia(id).ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarReferencia(int id = 0)
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                Referencia objRfrnc = dbe.Referencia.Where(x => x.Id == id).SingleOrDefault();
                objRfrnc.FechaModifica = DateTime.Now;
                objRfrnc.UsuarioModifica = UserId;
                objRfrnc.Estado = false;
                dbe.Referencia.Attach(objRfrnc);
                dbe.Entry(objRfrnc).State = EntityState.Modified;
                dbe.SaveChanges();
                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AprobarCandidatos(FormCollection collection)
        {
            int IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            int IdSolicitud = Convert.ToInt32(Request.Params["IdSolicitud"]);
            try
            {
                List<ListarCandidatoTodo_Result> candidatos = dbe.ListarCandidatoTodo(IdSolicitud).ToList();
                ListarCandidatoTodo_Result[] candidatosArray = candidatos.ToArray();
                var CantAprobado = 0;
                //Validaciones
                if (candidatos.Count() == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone5) top.uploadDone5('CREARCANDIDATO','');}window.onload=init;</script>");
                }
                foreach (ListarCandidatoTodo_Result cand in candidatosArray)
                {
                    int IdCandidato = cand.IdCandidato;

                    if (Convert.ToString(Request.Params["Aprobar" + IdCandidato]) == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone5) top.uploadDone5('VALIDAR','');}window.onload=init;</script>");
                    }
                    if (cand.PuntajeGTH == 0.0 || cand.PuntajeJefe == 0.0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone5) top.uploadDone5('CALIFICAR','');}window.onload=init;</script>");
                    }
                    if (IdUsuario == 11783 || IdUsuario == 1007)
                    {
                        var cant = dbe.SolicitudPersonal.Where(xx => xx.Id == IdSolicitud).FirstOrDefault();

                        if (Convert.ToString(Request.Params["Aprobar" + IdCandidato]) == "1")
                        {
                            CantAprobado++;
                        }
                        //if (cand.PuntajeGTH == 0.0 || cand.PuntajeGTH == 0.0)
                        //{
                        //    return Content("<script type='text/javascript'> function init() {" +
                        //"if(top.uploadDone1) top.uploadDone1('CALIFICAR','');}window.onload=init;</script>");
                        //}
                        if (cant.CantidadVacante < CantAprobado)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone5) top.uploadDone5('VALIDARCANTIDAD','');}window.onload=init;</script>");
                        }

                    }

                }

                //Guardar
                foreach (ListarCandidatoTodo_Result cand in candidatosArray)
                {
                    int IdCandidato = cand.IdCandidato;
                    string estado = Convert.ToString(Request.Params["Aprobar" + IdCandidato]);

                    var candidato = dbe.Candidato.Where(x => x.Id == IdCandidato).SingleOrDefault();
                    if (Convert.ToString(Request.Params["Aprobar" + IdCandidato]) == "1")
                    {
                        if (IdUsuario == 11783 || IdUsuario == 1007)
                        {
                            candidato.IdEstadoCandidato = 4;
                        }
                        else
                        {
                            candidato.IdEstadoCandidato = 2;
                        }

                    }
                    else
                    {
                        candidato.IdEstadoCandidato = 3;
                    }
                    candidato.FechaModifica = DateTime.Now;
                    candidato.UsuarioModifica = IdUsuario;
                    dbe.Entry(candidato).State = EntityState.Modified;
                    dbe.SaveChanges();
                }


                //Rechazar
                var solicitud = dbe.SolicitudPersonal.Where(x => x.Id == IdSolicitud).SingleOrDefault();
                if (IdUsuario == 11783 || IdUsuario == 1007)
                {
                    solicitud.IdEstadoSolicitud = 5;
                }
                else
                {
                    solicitud.IdEstadoSolicitud = 3;
                }

                solicitud.FechaModifica = DateTime.Now;
                solicitud.UsuarioModifica = IdUsuario;
                dbe.Entry(solicitud).State = EntityState.Modified;
                dbe.SaveChanges();

                try
                {
                    var soli = dbe.SolicitudPersonal.Where(x => x.Id == IdSolicitud).FirstOrDefault();
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
                    EnviarCorreoSolicitud(IdSolicitud);
                    SendMail mail = new SendMail();
                    mail.EnviarSolicitudPersonal((int)soli.UsuarioCrea, (int)soli.IdGerencia, (int)soli.IdEstadoSolicitud, soli.Id, GTH, 0,null);
                  
                       
                    

                }
                catch (Exception ex)
                {


                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone5) top.uploadDone5('OK','Se realizó la aprobación de los candidatos');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone5) top.uploadDone5('ERROR','');}window.onload=init;</script>");
            }
        }
        public ActionResult ListarCandidatoTodo(int id = 0)
        {

            var query = dbe.ListarCandidatoTodo(id).ToList();

            var total = query.Count();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerReferencia(int id = 0)
        {
            var query = dbe.ObtenerReferencia(id).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
