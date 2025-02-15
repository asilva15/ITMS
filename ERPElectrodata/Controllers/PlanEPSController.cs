using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class PlanEPSController : Controller
    {
        EntityEntities dbe = new EntityEntities();

        //
        // GET: /PlanEPS/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() {
            var result = (from a in dbe.PLAN_EPS
                          select new
                          {
                              a.ID_PLAN_EPS,
                              a.NAM_PLAN,
                          }).OrderBy(x=>x.NAM_PLAN);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
