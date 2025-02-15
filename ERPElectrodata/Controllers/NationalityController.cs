using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class NationalityController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Nationality/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListNationality()
        {
            var result = (from a in dbe.NATIONALITies
                          select new
                          {
                              a.ID_NATI,
                              a.NAM_NATI
                          }).OrderBy(x => x.NAM_NATI);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
