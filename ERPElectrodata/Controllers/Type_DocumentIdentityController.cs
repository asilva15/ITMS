using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class Type_DocumentIdentityController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Type_DocumentIdentity/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() {

            var result = (from q in dbe.TYPE_DOCUMENTIDENT
                          select new { 
                            q.ID_TYPE_DI,
                            q.NAM_TYPE_DI
                          }).ToList();

            return Json(new { Data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

    }
}
