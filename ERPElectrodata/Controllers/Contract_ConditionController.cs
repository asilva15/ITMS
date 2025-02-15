using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class Contract_ConditionController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Contract_Condition/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() {
             var result = (from a in dbe.CONTRACT_CONDITION
                          select new
                          {
                              a.ID_COND_CONT,
                              NAM_COND = a.CONDITION
                          }).OrderBy(x => x.NAM_COND).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);        
        }

    }
}
