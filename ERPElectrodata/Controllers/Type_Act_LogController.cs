using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class Type_Act_LogController : Controller
    {
        //
        // GET: /Type_Act_Log/
        EntityEntities dbe = new EntityEntities();
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
        public ActionResult Nuevo(string tipoActividad, int subtipo, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            int idCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    //Comprueba si existe el tipo de servicio
                    if (context.TYPE_ACT_LOG.Where(x => x.DES_ACT == tipoActividad && x.ID_ACCO == idCuenta).Count() > 0) return Content("DOBLE");

                    //Crea etapa
                    context.NuevoTipoActividad(tipoActividad, subtipo,estado,idCuenta);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }
            }
        }

        [HttpGet]
        public ActionResult Listar()
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListarTipoActividadLog(0).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, string tipoActividad, int subtipo, bool estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            int idCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    //Validar si el registro ya existe
                    var qEtapa = context.TYPE_ACT_LOG.Where(x => x.DES_ACT == tipoActividad && x.ID_ACCO == idCuenta && x.ID_TYPE_ACT != id);
                    if (qEtapa.Count() > 0) return Content("DOBLE");

                    context.ActualizarTipoActividad(id, tipoActividad, subtipo,estado, idCuenta);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpGet]
        public ActionResult Detalle(int id)
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListarTipoActividadLog(id).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CboListarTypeActLog()
        {
            try
            {
                List<CboListarTipoActividad_Result> query = dbe.CboListarTipoActividad(Convert.ToInt32(Session["ID_ACCO"])).ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
