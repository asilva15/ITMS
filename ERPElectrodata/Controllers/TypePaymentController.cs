using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypePaymentController : Controller
    {
        private EntityEntities dbe = new EntityEntities();

        // GET: /TypePayment/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List() {
            var query = (from a in dbe.TYPE_PAYMENT.Where(a=>a.VIG_TYPE_PAYM == true)
                         select a).OrderBy(a=>a.ORD_LIST);
            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
    }
}
