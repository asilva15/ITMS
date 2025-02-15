using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeDetailsVacationController : Controller
    {

        EntityEntities dbe = new EntityEntities();
        // GET: /TypeDetailsVacation/

        public ActionResult List()
        {
            var qType = (from a in dbe.TYPE_DETAILS_VACATION.ToList()
                         select new { 
                            a.ID_TYPE_DETA_VACA,
                            a.NAM_TYPE_DETA_VACA,
                         });

            return Json(new { Data = qType, Count = qType.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
