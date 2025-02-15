using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class StatusAssetController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /StatusAsset/
        public ActionResult Index()
        {
            return View();
        }        

        public ActionResult List()
        {
            var result = (from sa in dbc.STATUS_ASSET.ToList()
                          select new
                          {
                              sa.ID_STAT_ASSE,
                              sa.NAM_STAT_ASSE
                          }).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarEstadosBNV()
        {
            int idGrupo;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "NAM_STAT_ASSE")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarEstadosActBNV(idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
