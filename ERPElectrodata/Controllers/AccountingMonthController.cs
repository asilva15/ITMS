using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class AccountingMonthController : Controller
    {
        public CoreEntities db = new CoreEntities();

        public ActionResult ListIndependiente()
        {
            int ID_YEAR = 2012;

            var query = db.ACCOUNTING_MONTH.Where(am => am.ID_ACCO_YEAR == ID_YEAR);

            var result = (from am in query.ToList()
                          select new
                          {
                              ACCO_MONT = am.ACCO_MONT,
                              NAM_ACCO_MONT = am.NAM_ACCO_MONT,
                              DEFAULT = DateTime.Now.Month,
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /AccountingMonth/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            int ID_YEAR = 0;
            try
            {
                ID_YEAR = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            }
            catch
            {

            }
            var query = db.ACCOUNTING_MONTH.Where(am => am.ID_ACCO_YEAR == ID_YEAR);
            var ma = query.Where(x=>x.ACCO_MONT == DateTime.Now.Month);
            int mesact = 1;
            if (ma.Count() > 0) {
                mesact = ma.First().ID_ACCO_MONT;
            }

            var result = (from am in query.ToList()
                          select new
                          {
                              ID_ACCO_MONTH = am.ID_ACCO_MONT,
                              NAM_ACCO_MONT = am.NAM_ACCO_MONT,
                              DEFAULT = DateTime.Now.Month,
                              MonthNow = mesact
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarMeses(string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = (from m in db.ACCOUNTING_MONTH.Where(y => y.ID_ACCO_YEAR == DateTime.Now.Year)
                          select new
                          {
                              id = m.ACCO_MONT,
                              text = m.NAM_ACCO_MONT
                          }).Where(x => x.text.Contains(termino));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
