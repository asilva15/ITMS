using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class Actividad_TipoServicioController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        //
        // GET: /Actividad_TipoServicio/

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
                //Datos del tipo de servicio
                using (var context = new EntityEntities())
                {
                    var tipoServicio = context.ListarTipoServicio(id).Single();
                    ViewBag.Id = tipoServicio.Id;
                    ViewBag.IdUnidadNegocio = tipoServicio.IdUnidadNegocio;
                    ViewBag.IdMacroservicio = tipoServicio.IdMacroservicio;
                    ViewBag.IdServicio = tipoServicio.IdServicio;
                    ViewBag.IdIncidenteReq = tipoServicio.IdIncidenteReq;
                    ViewBag.TipoServicio = tipoServicio.TipoServicio;
                    ViewBag.ID_TYPE_ACT = tipoServicio.ID_TYPE_ACT;
                    ViewBag.ChkEstado = tipoServicio.ChkEstado;
                    ViewBag.ChkGrafico = tipoServicio.ChkGrafico;
                }
                return View();
            }
            else {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpGet]
        public ActionResult ListarTipoServicio()
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListarTipoServicio(0).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuevoTipoServicio(int incidenteReq, int type_act, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    if (context.TipoServicio.Where(x => x.IdCate == incidenteReq).Count() > 0) return Content("CATE");
                    //Comprueba si existe el tipo de servicio
                    if (context.TipoServicio.Where(x => x.ID_TYPE_ACT == type_act && x.IdCate == incidenteReq).Count() > 0) return Content("DOBLE");

                    //Crea el Perfil/Rol
                    context.NuevoTipoServicio(incidenteReq, type_act, false,estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTipoServicio(int id, int incidenteReq, int type_act, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    if (context.TipoServicio.Where(x => x.IdCate == incidenteReq && x.Id != id).Count() > 0) return Content("CATE");
                    //Validar si el registro ya existe
                    var qTipoServicio = context.TipoServicio.Where(x => x.ID_TYPE_ACT == type_act && x.IdCate == incidenteReq && x.Id != id);
                    if (qTipoServicio.Count() > 0) return Content("DOBLE");

                    context.ActualizarTipoServicio(id, incidenteReq, type_act, false, estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }
    }
}
