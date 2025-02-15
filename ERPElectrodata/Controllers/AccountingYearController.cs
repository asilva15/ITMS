using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class AccountingYearController : Controller
    {
        public CoreEntities db = new CoreEntities();

        //
        // GET: /AccountingYear/
        public ActionResult ListYearWithMonth()
        {
            var result_year = (from y in db.ACCOUNTING_YEAR
                         select new { 
                            ID_YEAR = y.ID_ACCO_YEAR,
                            NAM_YEAR = y.ID_ACCO_YEAR
                         });

            var result_month = (from x in db.ACCOUNTING_MONTH.Where(x=>x.ID_ACCO_MONT<=12).ToList()
                                    select new{
                                    x.ACCO_MONT,
                                    x.NAM_ACCO_MONT
                                    });

            int val_year = DateTime.Now.Year;
            int val_month = DateTime.Now.Month;

            return Json(new { DataYear = result_year,ValYear = val_year, DataMonth = result_month,ValMonth = val_month}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListIndependiente()
        {
            var query = db.ACCOUNTING_YEAR;
            var result = (from ay in query.ToList()
                          select new
                          {
                              ID_ACCO_YEAR = ay.ID_ACCO_YEAR,
                              DEFAULT = DateTime.Now.Year,
                          });

            int ID_YEAR = 2012;

            var queryM = db.ACCOUNTING_MONTH.Where(am => am.ID_ACCO_YEAR == ID_YEAR);

            var resultM = (from am in queryM.ToList()
                          select new
                          {
                              ACCO_MONT = am.ACCO_MONT,
                              NAM_ACCO_MONT = am.NAM_ACCO_MONT,
                              DEFAULT = DateTime.Now.Month,
                          });

            return Json(new { YEAR = result,MONTH=resultM, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /AccountingYear/
        public ActionResult List()
        {
            var query = db.ACCOUNTING_YEAR;
            var result = (from ay in query.ToList()
                          select new
                          {
                              ID_ACCO_YEAR = ay.ID_ACCO_YEAR,
                              DEFAULT = DefaultMonth(),
                              YearNow = DateTime.Now.Year
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public int DefaultMonth()
        {

            return ((DateTime.Now.Year - 2012) * 12) + DateTime.Now.Month;
        }
        //
        // GET: /AccountingYear/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarAños(string q, string page)
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
            var result = (from y in db.ACCOUNTING_YEAR.ToList()
                          select new
                          {
                              id = y.ID_ACCO_YEAR,
                              text = Convert.ToString(y.ID_ACCO_YEAR)
                          }).Where(x => x.text.Contains(termino));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
