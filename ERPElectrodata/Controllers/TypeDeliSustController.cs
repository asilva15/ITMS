using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeDeliSustController : Controller
    {

        CoreEntities dbc = new CoreEntities();
        // GET: /TypeDeliSust/

        public ActionResult List()
        {
            var query = (from a in dbc.TYPE_DELI_SUST.Where(x => x.VIG_TYPE_DELI_SUST == true).OrderBy(x => x.NAM_TYPE_DELI_SUST)
                         select new { 
                            a.ID_TYPE_DELI_SUST,
                            a.NAM_TYPE_DELI_SUST,
                         });

            return Json(new { Data = query, Count = query.Count() },JsonRequestBehavior.AllowGet);
        }

    }
}
