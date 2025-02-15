using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Electrodata.ERPElectrodata.Domain.Services.CatalogoServicioService;
using Electrodata.ERPElectrodata.Domain.Entities;

namespace ERPElectrodata.Controllers
{
    public class CatalogoServicioController : Controller
    {

        CatalogoServicioService _srvCatalogoServicioService = new CatalogoServicioService();

        //
        // GET: /CatalogoServicio/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            var listaCatalogoServicos = _srvCatalogoServicioService.Listar();

            return Json(new { Data = listaCatalogoServicos }, JsonRequestBehavior.AllowGet);
        }

    }
}
