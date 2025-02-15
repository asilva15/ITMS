using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class PensionController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Pension/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListTypePension() {
            var result = (from a in dbe.TYPE_PENSION
                          select new { 
                            a.ID_TYPE_PENS,
                            a.NAM_PENS,
                          });
            return Json(new { Data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult PanelPensionOld(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult DataPensionOld(int id = 0)
        {
            var result = (from p in dbe.PENSIONs.ToList()
                          join t in dbe.TYPE_PENSION on p.ID_TYPE_PENS equals t.ID_TYPE_PENS
                          where p.ID_PERS_ENTI == id
                          select new
                          {
                              CUSPP = (p.CUSPP == null ? "-" : p.CUSPP),
                              p.ID_PENS,
                              p.ID_TYPE_PENS,
                              t.NAM_PENS,
                              START_DATE = (p.START_DATE == null ? "-" : string.Format("{0:d}", p.START_DATE)),
                              END_DATE = (p.END_DATE == null ? "-" : string.Format("{0:d}", p.END_DATE)),
                              START = p.START_DATE,
                          }).OrderByDescending(x=>x.START);

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult NewPension(PENSION pens) {
            string CUSPP = Convert.ToString(Request.Form["CUSPP"]);
            DateTime START_DATE;
            int sw = 0, ID_TYPE_PENS = 0;

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_PENS"]), out ID_TYPE_PENS) == false) { sw = 1; }
            else if (CUSPP.Trim().Length == 0) { sw = 1; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["START_DATE"]), out START_DATE) == false) { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            //var OldPP = dbe.PENSIONs.Single(x => x.ID_PERS_ENTI == pens.ID_PERS_ENTI && x.END_DATE == null);
            var OldPP = dbe.PENSIONs.Where(x => x.ID_PERS_ENTI == pens.ID_PERS_ENTI && x.END_DATE == null).FirstOrDefault();

            if (OldPP != null)
            {
                OldPP.END_DATE = pens.START_DATE;
                dbe.PENSIONs.Attach(OldPP);
                dbe.Entry(OldPP).State = EntityState.Modified;
            }

            dbe.PENSIONs.Add(pens);
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','NewPension');}window.onload=init;</script>");    
        }
    }
}
