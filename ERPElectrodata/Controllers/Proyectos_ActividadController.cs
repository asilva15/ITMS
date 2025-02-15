using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class Proyectos_ActividadController : Controller
    {
        //
        // GET: /Proyectos_Actividad/

        CoreEntities dbc = new CoreEntities();
        [Authorize]
        public ActionResult Index()
        {
            if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                return View();
            else
                return RedirectToAction("Index", "Error");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuevaActividad(int idEtapa, string actividad, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new CoreEntities())
            {
                try
                {
                    //Comprueba si existe el tipo de servicio
                    if (context.Actividad.Where(x => x.IdEtapa == idEtapa && x.Nombre == actividad).Count() > 0) return Content("DOBLE");

                    //Crea etapa
                    context.NuevaActividad(idEtapa, actividad, estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }
            }
        }

        [HttpGet]
        public ActionResult ListarActividad()
        {
            using (var context = new CoreEntities())
            {
                var result = context.ListarActividad(0).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarActividad(int id, int idEtapa, string actividad, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new CoreEntities())
            {
                try
                {
                    //Validar si el registro ya existe
                    var qActividad = context.Actividad.Where(x => x.IdEtapa == idEtapa && x.Nombre == actividad && x.Id != id);
                    if (qActividad.Count() > 0) return Content("DOBLE");

                    context.EditarActividad(id, idEtapa, actividad, estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpGet]
        public ActionResult DetalleActividad(int id)
        {
            using (var context = new CoreEntities())
            {
                var result = context.ListarActividad(id).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CboListarActividad()
        {
            try
            {
                List<CboListarActividad_Result> query = dbc.CboListarActividad().ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
