using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class BuyModeController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public ActionResult List()
        {
            var query = dbc.BUY_MODE.Where(bm => bm.VIG_BUY_MODE == true);

            var result = (from bm in query.ToList()
                          select new
                          {
                              bm.ID_BUY_MODE,
                              bm.NAM_BUY_MODE

                          }).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /BuyMode/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarModosCompraBNV()
        {
            int idGrupo;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "NAM_BUY_MODE")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarModoCompraBNV(idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
