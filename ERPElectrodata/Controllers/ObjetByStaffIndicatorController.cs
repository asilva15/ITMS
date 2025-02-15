using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ObjetByStaffIndicatorController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /ObjetByStaffIndicator/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListIndicatorByObjetive(int id=0)
        {
            var query = dbe.OBJECT_BY_STAFF_INDICATOR.Where(x => x.ID_OBJE_STAF == id);

            var result = (from x in query.ToList()
                          join oi in dbe.OBJETIVE_INDICATOR on x.ID_OBJE_INDI equals oi.ID_OBJE_INDI
                          select new {
                              NAM_OBJE_INDI = oi.NAM_OBJE_INDI,
                              VAL_OBJE_STAF_INDI = x.VAL_OBJE_STAF_INDI,
                          });

            return Json(new {Data = result,Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
