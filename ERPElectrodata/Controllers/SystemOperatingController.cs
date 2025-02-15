using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class SystemOperatingController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /SystemOperating/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListByTypeAsset()
        {
            int ID_TYPE_ASSE;
            string txt = "";
            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (FIELD == "ID_TYPE_ASSE")
            {
                ID_TYPE_ASSE = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
                if (txt == null) { txt = ""; }
            }
            else
            {
                ID_TYPE_ASSE = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
            }

            var query = (from a in dbc.SYSTEM_OPER_TYPE_ASSE.Where(x => x.ID_TYPE_ASSE == ID_TYPE_ASSE)
                         join b in dbc.SYSTEM_OPERATING on a.ID_SYST_OPER equals b.ID_SYST_OPER
                         select new { 
                            a.ID_SYST_OPER_TYPE_ASSE,
                            a.ID_TYPE_ASSE,
                            a.ID_SYST_OPER,
                            b.NAM_SYST_OPER,
                         }).OrderBy(x=>x.NAM_SYST_OPER);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create(int id = 0) {
            ViewBag.ID_TYPE_ASSE = id;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(SYSTEM_OPER_TYPE_ASSE sota) {
            string NAM_SYST_OPER = Convert.ToString(Request.Form["NAM_SYST_OPER"]);

            if (NAM_SYST_OPER.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneOS('ERROR','0');}window.onload=init;</script>");
            }

            int ID_TYPE_ASSE = Convert.ToInt32(Request.Form["ID_TYPE_ASSE_HF"]);
            NAM_SYST_OPER = NAM_SYST_OPER.ToUpper();
            string DESC_SYST_OPER = Convert.ToString(Request.Form["DESC_SYST_OPER"]);

            var so = dbc.SYSTEM_OPERATING.Where(x => x.NAM_SYST_OPER.ToUpper() == NAM_SYST_OPER);
            int id = 0;
            if (so.Count() == 0)
            {
                var sop = new SYSTEM_OPERATING();
                sop.NAM_SYST_OPER = NAM_SYST_OPER;
                sop.DESC_SYST_OPER = DESC_SYST_OPER;
                dbc.SYSTEM_OPERATING.Add(sop);
                dbc.SaveChanges();

                var sotype = new SYSTEM_OPER_TYPE_ASSE();
                sotype.ID_TYPE_ASSE = ID_TYPE_ASSE;
                sotype.ID_SYST_OPER = sop.ID_SYST_OPER;
                sotype.VIG_SYST_OPER_TYPE_ASSE = true;
                dbc.SYSTEM_OPER_TYPE_ASSE.Add(sotype);
                dbc.SaveChanges();

                id = Convert.ToInt32(sotype.ID_SYST_OPER_TYPE_ASSE);
            }
            else {
                var q = (from a in dbc.SYSTEM_OPER_TYPE_ASSE.Where(x => x.ID_TYPE_ASSE == ID_TYPE_ASSE)
                         join b in dbc.SYSTEM_OPERATING.Where(x => x.NAM_SYST_OPER.ToUpper() == NAM_SYST_OPER) on a.ID_SYST_OPER equals b.ID_SYST_OPER
                         select a);

                if (q.Count() > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneOS('ERROR','1');}window.onload=init;</script>");
                }
                else
                {
                    var sotype = new SYSTEM_OPER_TYPE_ASSE();
                    sotype.ID_TYPE_ASSE = ID_TYPE_ASSE;
                    sotype.ID_SYST_OPER = so.First().ID_SYST_OPER;
                    sotype.VIG_SYST_OPER_TYPE_ASSE = true;
                    dbc.SYSTEM_OPER_TYPE_ASSE.Add(sotype);
                    dbc.SaveChanges();

                    id = Convert.ToInt32(sotype.ID_SYST_OPER_TYPE_ASSE);
                }            
            }

            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDoneOS('OK','" + id.ToString() + "');}window.onload=init;</script>");
        }

    }
}
