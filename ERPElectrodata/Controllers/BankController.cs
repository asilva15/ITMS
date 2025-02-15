using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class BankController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Bank/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var result = (from a in dbe.BANKs
                          select new
                          {
                              a.ID_BANK,
                              a.NAM_BANK,
                          }).OrderBy(x=>x.NAM_BANK);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
