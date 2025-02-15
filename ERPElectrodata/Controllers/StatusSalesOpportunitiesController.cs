using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;

namespace ERPElectrodata.Controllers
{
    public class StatusSalesOpportunitiesController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public ActionResult ListAll()
        {
            //var query = dbc.STATUS_SALES_OPPORTUNITY.Where(x => x.VIS_CREATE == true);
            textInfo = cultureInfo.TextInfo;

            var result = (from x in dbc.STATUS_SALES_OPPORTUNITY.ToList()
                          select new
                          {
                              x.ID_STAT_SALE_OPPO,
                              NAM_STAT_SALE_OPPO = textInfo.ToUpper(x.NAM_STAT_SALE_OPPO),
                          });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /StatusSalesOpportunities/
        public ActionResult ListAllCreate()
        {
            var query = dbc.STATUS_SALES_OPPORTUNITY.Where(x => x.VIS_CREATE == true);

            var result = (from x in query.ToList()
                          select new { 
                          x.ID_STAT_SALE_OPPO,
                          x.NAM_STAT_SALE_OPPO,
                          });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
