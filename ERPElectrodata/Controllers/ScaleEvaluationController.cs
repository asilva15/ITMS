using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ScaleEvaluationController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /ScaleEvaluation/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllActive()
        {
            var result = (from x in dbe.SCALE_EVALUATION.ToList()
                          select new {
                              COL_SCAL_EVAL = x.COL_SCAL_EVAL,
                              ACR = x.ACR,
                              VAL = (x.VAL_SUP == null ? "" : Convert.ToString(x.VAL_SUP)+" - ") + (x.VAL_INF == null ? "" :Convert.ToString(x.VAL_INF))
                          });
            return Json(new {Data = result,Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
