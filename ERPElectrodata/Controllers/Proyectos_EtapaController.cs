using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class Proyectos_EtapaController : Controller
    {
        //
        // GET: /Proyectos_Etapa/

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
        public ActionResult NuevaEtapa(string etapa, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new CoreEntities())
            {
                try
                {
                    //Comprueba si existe el tipo de servicio
                    if (context.Etapa.Where(x => x.Nombre == etapa).Count() > 0) return Content("DOBLE");

                    //Crea etapa
                    context.NuevaEtapa(etapa, estado, UserId);
                    return Content("OK");
                }
                catch (Exception e)
                {
                    return Content("ERROR");
                }
            }
        }

        [HttpGet]
        public ActionResult ListarEtapa()
        {
            using (var context = new CoreEntities())
            {
                var result = context.ListarEtapa(0).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEtapa(int id, string etapa, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new CoreEntities())
            {
                try
                {
                    //Validar si el registro ya existe
                    var qEtapa = context.Etapa.Where(x => x.Nombre == etapa && x.Id != id);
                    if (qEtapa.Count() > 0) return Content("DOBLE");

                    context.EditarEtapa(id, etapa, estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpGet]
        public ActionResult DetalleEtapa(int id)
        {
            using (var context = new CoreEntities())
            {
                var result = context.ListarEtapa(id).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CboListarEtapa()
        {
            try
            {
                List<CboListarEtapa_Result> query = dbc.CboListarEtapa().ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
