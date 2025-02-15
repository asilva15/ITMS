using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class BloodGroupController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /BloodGroup/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListBloodGroup()
        {
            var result = (from a in dbe.BLOOD_GROUP
                          select new
                          {
                              a.ID_BLOO_GROU,
                              a.NAM_BLOO_GROU
                          }).OrderBy(x => x.NAM_BLOO_GROU);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
    }
}
