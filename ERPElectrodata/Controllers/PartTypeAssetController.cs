using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class PartTypeAssetController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /PartTypeAsset/List
        public ActionResult List(int id = 0)
        {
            var result = (from p in dbc.PART_TYPE_ASSET
                          where p.ID_TYPE_ASSE == id && p.VIS_PART_TYPE_ASSE == true
                          select new {
                            p.ID_PART_TYPE_ASSE,
                            p.NAM_PART_TYPE_ASSE
                          });

            return Json(new {Data = result,Count = result.Count() },JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /PartTypeAsset/ParameterConfigurationByID_PTA
        public ActionResult ParameterConfigurationByID_PTA(int id = 0, int id1 = 0)
        {
            var query = dbc.CONFIGURATIONs.Where(x => x.ID_ASSE == id);

            var result = (from p in dbc.PART_TYPE_ASSET
                          where p.ID_PART_TYPE_ASSE == id1
                          join pc in dbc.PARAMETER_CONFIGURATION on p.ID_PARA_CONF equals pc.ID_PARA_CONF
                          join c in query on pc.ID_PARA_CONF equals c.ID_PARA_CONF into lc
                          from cx in lc.DefaultIfEmpty()
                          select new
                          {
                              p.ID_PART_TYPE_ASSE,
                              p.NAM_PART_TYPE_ASSE,
                              pc.HAS_VALUE,
                              pc.UND_PARA_CONF,
                              ID_CONF = (cx.ID_CONF == null ? 0 : cx.ID_CONF),
                              VAL_CONF = (cx.VAL_CONF == null ? String.Empty : cx.VAL_CONF)
                          });

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /PartTypeAsset/

        public ActionResult Index()
        {
            return View();
        }

    }
}
