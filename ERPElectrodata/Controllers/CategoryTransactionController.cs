using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ManagementSystemIT.Controllers
{
    public class CategoryTransactionController : Controller
    {
        public CoreEntities db = new CoreEntities();
        //
        // GET: /CategoryTransaction/
        public ActionResult List()
        {
            int IdTypeTran = 1;
            try{
                IdTypeTran = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            }
            catch{
                IdTypeTran =1;
            }

            var query = db.CATEGORY_TRANSACTION.Where(ct => ct.ID_TYPE_TRAN == IdTypeTran);
            
            var result = (from ct in query.ToList()
                          orderby ct.NAM_CATE_TRAN
                             select new {
                             NAM_CATE_TRAN = ct.NAM_CATE_TRAN,
                             ID_CATE_TRAN = ct.ID_CATE_TRAN,
                             //DEFAULT = (ct.ID_CATE_TRAN == 1 ? 1:2)
                             });

            return Json(new{Data=result,total=query.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
