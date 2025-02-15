using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class ProfesionController : Controller
    {
        //
        // GET: /Profesion/
        EntityEntities dbe = new EntityEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            var resultado = (from p in dbe.Profesions.Where(x => x.Estado == true)
                          select new
                          {
                              p.Id,
                              p.Nombre
                          }).OrderBy(x => x.Nombre);

            return Json(new { Data = resultado, Count = resultado.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Crear()
        {
            return View();
        }

        public ActionResult Editar(int id = 0)
        {
            Profesion objProfesion = dbe.Profesions.Find(id);

            return View(objProfesion);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CrearProfesion(Profesion objProfesion)
        {
            try
            {

                objProfesion.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                objProfesion.FechaCrea = DateTime.Now;
                objProfesion.Estado = true;

                if (String.IsNullOrEmpty(objProfesion.Nombre))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensajeCrearProfesion) top.mensajeCrearProfesion('ERROR',0);}window.onload=init;</script>");
                }

                dbe.Profesions.Add(objProfesion);
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensajeCrearProfesion) top.mensajeCrearProfesion('OK'," + objProfesion.Id.ToString() + ");}window.onload=init;</script>");
            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensajeCrearProfesion) top.mensajeCrearProfesion('ERROR',0);}window.onload=init;</script>");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProfesion(Profesion objProfesion)
        {
            try
            {
                objProfesion.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
                objProfesion.FechaModifica = DateTime.Now;

                if (String.IsNullOrEmpty(objProfesion.Nombre))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR',0);}window.onload=init;</script>");
                }

                dbe.Entry(objProfesion).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('OK'," + objProfesion.Id.ToString() + ");}window.onload=init;</script>");
            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR',0);}window.onload=init;</script>");
            }
        }

        public ActionResult ListarProfesiones()
        {
            var query = dbe.ListarProfesiones().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
