using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class DocumentControlTypeController : Controller
    {
        ELECTRODATAEntities dbed = new ELECTRODATAEntities();
        //
        // GET: /DocumentControlType/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fil()
        {

            try
            {
                string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);
                int skip = Convert.ToInt32(Request.Params["skip"].ToString());
                int take = Convert.ToInt32(Request.Params["take"].ToString());

                

                if (field == "NUMDOC")
                {
                    string NUMDOC = Convert.ToString(Request.Params["filter[filters][0][value]"]);

                    var query = dbed.MOVCABE1FIL.Where(x => x.NUMDOC.Contains(NUMDOC.ToUpper()));

                    var result = (from fil in query.OrderBy(x => x.NUMDOC).Skip(skip).Take(take).ToList()
                                  select new
                                  {
                                      NUMDOC = fil.NUMDOC.Trim(),
                                      NOMANE = fil.NOMANE.Trim(),
                                  });

                    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch{
                return Json(new { Data = "" }, JsonRequestBehavior.AllowGet);
            }

            //string cod = "2014-10";

            
        }

    }
}
