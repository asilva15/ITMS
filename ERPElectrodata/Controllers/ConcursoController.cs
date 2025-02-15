using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ConcursoController : Controller
    {
        //
        // GET: /Concurso/
        private EntityEntities dbe = new EntityEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registrar(int id)
        {
            Concurso objConcurso = new Concurso();
            objConcurso.Campo1 = id;
            objConcurso.FechaCreacion = DateTime.Now;
            dbe.Concursoes.Add(objConcurso);
            dbe.SaveChanges();
            return Json(new { Data = id }, JsonRequestBehavior.AllowGet);
        }
    }
}
