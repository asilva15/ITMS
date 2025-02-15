using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class SidigeController : Controller
    {
        //
        // GET: /Sidige/
        private ELECTRODATAEntities dbed = new ELECTRODATAEntities();

        public ActionResult ListSupplier()
        {
            try
            {
                string name_ruc = (Convert.ToString(Request.Params["filter[filters][0][value]"])).ToLower();

                var query = dbed.ANEXOes.Where(x => x.XTIPIDE1.Contains("RUC"))
                    .Where(x => x.XTIPANE == "PRO")
                    .Where(x => x.IDEANE1 != null)
                    .Where(x => x.NOMANE.ToLower().Contains(name_ruc) || x.IDEANE1.ToLower().Contains(name_ruc));

                var result = (from x in query
                              select new
                              {
                                  CODANE = x.CODANE,
                                  NOMANE = x.NOMANE,
                                  IDEANE1 = x.IDEANE1,
                                  DIRANE = x.DIRANE,
                                  TELANE = x.TELANE,
                              });

                return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data= "Error Parametro"}, JsonRequestBehavior.AllowGet);
            }
            
        }
        
        public ActionResult Index()
        {
            return View();
        }

    }
}
