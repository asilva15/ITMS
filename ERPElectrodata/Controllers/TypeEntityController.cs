using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeEntityController : Controller
    {
        private EntityEntities dbe = new EntityEntities();
        public ActionResult List() {
            var result = (from t in dbe.TYPE_ENTITY
                          select new { 
                            t.ID_TYPE_ENTI,
                            t.NAM_TYPE_ENTI
                          }).OrderBy(x=>x.NAM_TYPE_ENTI);

            return Json(new { Data = result, Count= result.Count() },JsonRequestBehavior.AllowGet);
        }
    }
}
