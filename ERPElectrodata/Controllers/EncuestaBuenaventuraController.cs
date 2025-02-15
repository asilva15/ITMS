using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class EncuestaBuenaventuraController : Controller
    {
        public CoreEntities db = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /EncuestaBuenaventura/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Editar(int idPregunta)
        {
            ViewBag.IdPregunta = idPregunta;
            return View();
        }

        public ActionResult EncuestaUsuario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearPregunta(string pregunta, string preguntaespecificada, string categoria, string respuestas)
        {
            try
            {
                int idCategoria = 0;
                int usuario = (int)(Session["UserId"]);

                var result = (from cat in db.CategoriaPreguntaEncuesta.Where(x => x.EstadoCategoria == true && x.NombreCategoria == categoria).ToList()
                              select new
                              {
                                  cat.IdCategoria
                              }).ToList();

                if(result.Count == 0)
                {
                    var objCategoria = new CategoriaPreguntaEncuesta();
                    objCategoria.NombreCategoria = categoria;
                    objCategoria.FechaRegistro = DateTime.Now;
                    objCategoria.FechaActualizacion = DateTime.Now;
                    objCategoria.EstadoCategoria = true;
                    objCategoria.UsuarioActualizacion = usuario;
                    objCategoria.UsuarioRegistra = usuario;
                    db.CategoriaPreguntaEncuesta.Add(objCategoria);
                    db.SaveChanges();
                    idCategoria = Convert.ToInt32(objCategoria.IdCategoria);
                }else
                {
                    idCategoria = result.First().IdCategoria;
                }

                var objPregunta = new PreguntaEncuesta();
                objPregunta.Pregunta = pregunta;
                objPregunta.FechaRegistro = DateTime.Now;
                objPregunta.FechaActualizacion = DateTime.Now;
                objPregunta.UsuarioRegistra = usuario;
                objPregunta.EstadoPregunta = true;
                objPregunta.IdCategoria = idCategoria;
                objPregunta.PreguntaEspecificar = preguntaespecificada;
                db.PreguntaEncuesta.Add(objPregunta);
                db.SaveChanges();
                int idPregunta = Convert.ToInt32(objPregunta.IdPregunta);

                JArray respuestasArray = JArray.Parse(respuestas);

                foreach (JToken respuestaToken in respuestasArray)
                {
                    string respuestaTexto = respuestaToken["texto"]?.ToString();
                    bool respuestaTextoEspecifico = respuestaToken["marcada"]?.ToObject<bool>() ?? false;


                    var objRespuesta = new RespuestasEncuesta();
                    objRespuesta.IdPregunta = idPregunta;
                    objRespuesta.Respuesta = respuestaTexto;
                    objRespuesta.FlagEspecificar = respuestaTextoEspecifico;
                    objRespuesta.FechaResgistro = DateTime.Now;
                    objRespuesta.FechaActualizacion = DateTime.Now;
                    objRespuesta.UsuarioRegistra = usuario;
                    objRespuesta.EstadoRespuesta = true;
                    db.RespuestasEncuesta.Add(objRespuesta);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public ActionResult ListarPreguntas()
        {
            var currentYear = DateTime.Now.Year;
            var result = (from preguntas in db.PreguntaEncuesta.Where(x => x.EstadoPregunta == true).ToList()
                          where preguntas.FechaRegistro.Year == currentYear
                          select new
                          {
                              IdPreunnta = preguntas.IdPregunta,
                              PREGUNTA = preguntas.Pregunta,
                              AnioPregunta = currentYear
                          });
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerCategorias()
        {
            var result = (from categoria in db.CategoriaPreguntaEncuesta.Where(x => x.EstadoCategoria == true).ToList()
                          select new
                          {
                              categoria.IdCategoria,
                              categoria.NombreCategoria
                          });
            result = result.OrderBy(x => x.IdCategoria).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerDetallePregunta(int IdPregunta)
        {
            try
            {
                var result = (from pregunta in db.PreguntaEncuesta
                              join respuesta in db.RespuestasEncuesta on pregunta.IdPregunta equals respuesta.IdPregunta
                              join categoria in db.CategoriaPreguntaEncuesta on pregunta.IdCategoria equals categoria.IdCategoria
                              where pregunta.EstadoPregunta == true && pregunta.IdPregunta == IdPregunta && respuesta.EstadoRespuesta == true
                              select new
                              {
                                  pregunta.IdPregunta,
                                  pregunta.Pregunta,
                                  respuesta.IdRespuesta,
                                  respuesta.Respuesta,
                                  respuesta.FlagEspecificar,
                                  pregunta.IdCategoria,
                                  categoria.NombreCategoria 
                              }).ToList();

                result = result.OrderBy(x => x.IdRespuesta).ToList();
                var dd = Json(new { data = result }, JsonRequestBehavior.AllowGet);
                return dd;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ActionResult ObtenerListadoPreguntayRespuestas()
        {
            var currentYear = DateTime.Now.Year;
            var result = (from pregunta in db.PreguntaEncuesta.Where(x => x.EstadoPregunta == true).ToList()
                          join respuesta in db.RespuestasEncuesta on pregunta.IdPregunta equals respuesta.IdPregunta
                          where respuesta.EstadoRespuesta == true && pregunta.FechaRegistro.Year == currentYear
                          select new
                          {
                              pregunta.IdPregunta,
                              pregunta.Pregunta,
                              pregunta.PreguntaEspecificar,
                              respuesta.IdRespuesta,
                              respuesta.Respuesta,
                              respuesta.FlagEspecificar
                          });
            result = result.OrderBy(x => x.IdRespuesta).ToList();
            var dd = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            return dd;
        }

        [HttpPost]
        public ActionResult GuardarCambios(int IdPregunta, string pregunta, string respuestas, string categoria)
        {
            int idCategoria = 0;
            int usuario = (int)(Session["UserId"]);

            var result = (from cat in db.CategoriaPreguntaEncuesta.Where(x => x.EstadoCategoria == true && x.NombreCategoria == categoria).ToList()
                          select new
                          {
                              cat.IdCategoria
                          }).ToList();

            if (result.Count == 0)
            {
                var objCategoria = new CategoriaPreguntaEncuesta();
                objCategoria.NombreCategoria = categoria;
                objCategoria.FechaRegistro = DateTime.Now;
                objCategoria.FechaActualizacion = DateTime.Now;
                objCategoria.EstadoCategoria = true;
                objCategoria.UsuarioActualizacion = usuario;
                objCategoria.UsuarioRegistra = usuario;
                db.CategoriaPreguntaEncuesta.Add(objCategoria);
                db.SaveChanges();
                idCategoria = Convert.ToInt32(objCategoria.IdCategoria);
            }
            else
            {
                idCategoria = result.First().IdCategoria;
            }

            JArray respuestasArray = JArray.Parse(respuestas);
            try
            {
                var Pregunta = new PreguntaEncuesta();
                Pregunta = db.PreguntaEncuesta.Find(IdPregunta);
                Pregunta.Pregunta = pregunta;
                Pregunta.FechaActualizacion = DateTime.Now;
                Pregunta.UsuarioActualizacion = (int)Session["UserId"];
                Pregunta.IdCategoria = idCategoria;
                db.PreguntaEncuesta.Attach(Pregunta);
                db.Entry(Pregunta).State = EntityState.Modified;
                db.SaveChanges();

                foreach (JToken respuestaToken in respuestasArray)
                {
                    int IdRespuesta = (int)respuestaToken["IdRespuesta"];
                    string Respuesta = respuestaToken["Respuesta"]?.ToString();

                    var RespuestaM = db.RespuestasEncuesta.Find(IdRespuesta);
                    RespuestaM.Respuesta = Respuesta;
                    RespuestaM.FechaActualizacion = DateTime.Now;
                    RespuestaM.UsuarioActualizacion = (int)Session["UserId"];
                    db.RespuestasEncuesta.Attach(RespuestaM);
                    db.Entry(RespuestaM).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch
            {
            }

            return null;
        }

        [HttpPost]
        public ActionResult EliminarPregunta(int IdPregunta)
        {
            try
            {
                var Pregunta = new PreguntaEncuesta();
                Pregunta = db.PreguntaEncuesta.Find(IdPregunta);
                Pregunta.EstadoPregunta = false;
                Pregunta.FechaActualizacion = DateTime.Now;
                Pregunta.UsuarioActualizacion = (int)Session["UserId"];
                db.PreguntaEncuesta.Attach(Pregunta);
                db.Entry(Pregunta).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                //return Content("<script type='text/javascript'> function init() {" +
                //        "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0');}window.onload=init;</script>");
            }
            //return Content("<script type='text/javascript'> function init() {" +
            //            "if(top.uploadDoneForm) top.uploadDoneForm('OK','0');}window.onload=init;</script>");
            return null;
        }
    }
}
