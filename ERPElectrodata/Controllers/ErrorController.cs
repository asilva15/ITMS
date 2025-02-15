using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.AppCode;

namespace ERPElectrodata.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            Sesiones objSesion = new Sesiones();

            ViewBag.Nombre = objSesion.SesionNombre;
            ViewBag.Foto = objSesion.SesionFoto;
            return View();
        }

    }
}
