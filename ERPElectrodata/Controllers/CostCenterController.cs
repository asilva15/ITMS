using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class CostCenterController : Controller
    {
        //
        // GET: /CostCenter/
        public CoreEntities dbc = new CoreEntities();

        public ActionResult List()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            string COD = Convert.ToString(Request.Params["filter[filters][0][value]"]);

            var query = dbc.COST_CENTER.Where(cc => cc.VIG_COST_CENT == true && cc.ID_ACCO == ID_ACCO);

            if (!String.IsNullOrEmpty(COD))
            {
                query = query.Where(cc => cc.COD_COST_CENT.Contains(COD.ToUpper()));
            }

            var result = (from cc in query.ToList()
                          select new
                          {
                              cc.ID_COST_CENT,
                              cc.COD_COST_CENT,
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: /Manufacturer/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(COST_CENTER cc)
        {
            string COD_COST_CENT = Convert.ToString(Request.Form["COD_COST_CENT"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (COD_COST_CENT.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneCostCenter('ERROR','0');}window.onload=init;</script>");
            }
            cc.COD_COST_CENT = cc.COD_COST_CENT.ToUpper();
            int ctd = dbc.COST_CENTER.Where(x => x.COD_COST_CENT == cc.COD_COST_CENT && x.ID_ACCO == ID_ACCO ).Count();
            if (ctd > 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneCostCenter('ERROR','2');}window.onload=init;</script>");
            }

            try
            {                
                cc.ID_ACCO = ID_ACCO;
                cc.VIG_COST_CENT = true;
                dbc.COST_CENTER.Add(cc);
                dbc.SaveChanges();
                int id = Convert.ToInt32(cc.ID_COST_CENT);

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDoneCostCenter('OK','" + id.ToString() + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneCostCenter('ERROR','1');}window.onload=init;</script>");
            }
        }

    }
}
