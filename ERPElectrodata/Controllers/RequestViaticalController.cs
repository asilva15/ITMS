using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class RequestViaticalController : Controller
    {

        CoreEntities dbc = new CoreEntities();
        // GET: /RequestViatical/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() {

            var query = (from a in dbc.TYPE_VIATICAL.Where(x => x.VIG_TYPE_VIAT == true)
                         select new
                         {
                             a.ID_TYPE_VIAT,
                             a.NAM_TYPE_VIAT,
                             AMOUNT = 0,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListDetailViatical(int id = 0) {
            var query = (from a in dbc.REQUEST_VIATICAL.Where(x => x.ID_REQU_EXPE == id)
                         join b in dbc.TYPE_VIATICAL on a.ID_TYPE_VIAT equals b.ID_TYPE_VIAT
                         select new { 
                            a.AMOUNT,
                            b.NAM_TYPE_VIAT
                         });
            return Json(new { Data = query, Count = query.Count()},JsonRequestBehavior.AllowGet);
        }

    }
}
