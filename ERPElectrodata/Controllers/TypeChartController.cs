using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{    
    public class TypeChartController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /TypeChart/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() {
            var query = (from a in dbe.TYPE_CHART
                         select new { 
                            a.ID_TYPE_CHAR,
                            a.NAM_TYPE
                         });
            return Json(new { Data = query, Count = query.Count() },JsonRequestBehavior.AllowGet);
        }

    }
}
