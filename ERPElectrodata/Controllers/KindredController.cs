using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class KindredController : Controller
    {
        EntityEntities dbe = new EntityEntities();

        //
        // GET: /Kindred/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() {
            var result = (from k in dbe.KINDREDs
                          select new { 
                            k.ID_KINDRED,
                            k.NAM_KIND,
                          }).OrderBy(x=>x.NAM_KIND);

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
