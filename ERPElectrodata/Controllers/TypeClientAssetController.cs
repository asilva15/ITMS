using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeClientAssetController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /TypeClientAsset/List

        public ActionResult List()
        {
            var result = (from tca in dbc.TYPE_CLIENT_ASSET.Where(x => x.VIS_TYPE_ASSE_CLIE == true)
                          select new { 
                            tca.ID_TYPE_ASSE_CLIEN,
                            tca.NAM_TYPE_ASSE_CLIE
                          });//dbc.TYPE_CLIENT_ASSET.Where(tca => tca.VIS_TYPE_ASSE_CLIE == true);

            return Json(new { Data = result,Count= result.Count()},JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            return View();
        }

    }
}
