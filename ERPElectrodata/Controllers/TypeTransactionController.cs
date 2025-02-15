using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeTransactionController : Controller
    {

        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /TypeTransaction/
        public ActionResult List()
        {
            var query = dbc.TYPE_TRANSACTION;
            var result = (from tt in query.ToList()
                          select new
                          {
                              ID_TYPE_TRAN = tt.ID_TYPE_TRAN,
                              NAM_TYPE_TRAN = tt.NAM_TYPE_TRAN,
                              DEFAULT = DefaultCategory(tt.ID_TYPE_TRAN),
                          });
            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public int DefaultCategory(int id)
        {
            var query = dbc.CATEGORY_TRANSACTION.Where(c => c.ID_TYPE_TRAN == id).OrderBy(c => c.NAM_CATE_TRAN).First();
            return Convert.ToInt32(query.ID_CATE_TRAN);
        }

        //
        // GET: /TypeTransaction/

        public ActionResult Index()
        {
            return View();
        }

    }
}
