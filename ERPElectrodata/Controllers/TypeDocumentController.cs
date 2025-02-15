using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeDocumentController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /TypeDocument/

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListTypeDocumentsAchievements() {
            var result = (from a in dbe.TYPE_DOCUMENT.Where(x => x.REGISTER == 1)
                          select new { 
                            a.ID_TYPE_DOCU,
                            a.NAM_DOCU,
                          }).OrderBy(x=>x.NAM_DOCU);

            return Json(new { Data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

    }
}
