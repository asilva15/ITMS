using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class InstitucionController : Controller
    {
        //
        // GET: /Institucion/

        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var result = (from x in dbe.Institucion.ToList()
                          select new
                          {
                              x.IdInstitucion,
                              x.Nombre
                          }).OrderBy(x => x.Nombre);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarInstitucion()
        {
            var result = dbe.Institucion.Where(x => x.Estado == true).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

    }
}
