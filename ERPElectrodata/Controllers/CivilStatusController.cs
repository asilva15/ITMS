using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class CivilStatusController : Controller
    {
        EntityEntities dbe = new EntityEntities();

        //
        // GET: /CivilStatus/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListCivilStatus() {
            var result = (from a in dbe.CIVIL_STATUS
                          select new { 
                            a.ID_CIVI_STAT,
                            a.NAM_CIVI_STAT
                          }).OrderBy(x => x.NAM_CIVI_STAT);
            return Json(new { Data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }
    }
}
