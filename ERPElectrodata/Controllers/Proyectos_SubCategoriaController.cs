using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class Proyectos_SubCategoriaController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        //
        // GET: /Proyectos_SubCategoria/
        [Authorize]
        public ActionResult Index()
        {
            if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                return View();
            else
                return RedirectToAction("Index", "Error");
        }
        [Authorize]
        public ActionResult Editar(int id)
        {

            if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
            {
                //Datos de la subcategoría
                using (var context = new CoreEntities())
                {
                    var subCategoria = context.ListarSubcategoria(id).Single();
                    ViewBag.Id = subCategoria.IdSubcategoria;
                    ViewBag.IdEtapa = subCategoria.IdEtapa;
                    ViewBag.IdActividad = subCategoria.IdActividad;
                    ViewBag.Subcategoria = subCategoria.Subcategoria;
                    ViewBag.ChkEstado = subCategoria.EstadoSubcategoria;
                    ViewBag.ChkGrafico = subCategoria.GraficoSubcategoria;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuevaSubcategoria(int idEtapa,int idActividad, string subcategoria, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new CoreEntities())
            {
                int idetapa = Convert.ToInt32(Request.Params["idEtapa"]);
                try
                {
                    //Comprueba si existe el tipo de servicio
                    if (context.SubCategoria.Where(x => x.IdEtapa == idEtapa && x.IdActividad == idActividad && x.Nombre == subcategoria).Count() > 0) return Content("DOBLE");

                    //Crea etapa
                    context.NuevaSubcategoria(idEtapa,idActividad, subcategoria, false , estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }
            }
        }

        [HttpGet]
        public ActionResult ListarSubcategoria()
        {
            using (var context = new CoreEntities())
            {
                var result = context.ListarSubcategoria(0).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarSubcategoria(int id,int idEtapa,int idActividad, string subcategoria, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new CoreEntities())
            {
                try
                {
                    //Validar si el registro ya existe
                    var qSubcategoria = context.SubCategoria.Where(x => x.IdActividad == idActividad && x.Nombre == subcategoria && x.Id != id);
                    if (qSubcategoria.Count() > 0) return Content("DOBLE");

                    context.EditarSubcategoria(id,idEtapa, idActividad, subcategoria, false, estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpGet]
        public ActionResult DetalleSubcategoria(int id)
        {
            using (var context = new CoreEntities())
            {
                var result = context.ListarSubcategoria(id).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CboListarSubcategoria()
        {
            try
            {
                List<CboListarSubcategoria_Result> query = dbc.CboListarSubcategoria().ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
