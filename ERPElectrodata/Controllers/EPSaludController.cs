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
    public class EPSaludController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /EPSalud/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PanelEPSalud(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult ListEPSalud(int id = 0) {
            var result = (from eps in dbe.EPSALUDs.ToList()
                          join tp in dbe.PLAN_EPS on eps.ID_PLAN_EPS equals tp.ID_PLAN_EPS
                          where eps.ID_PERS_ENTI == id
                          select new
                          {
                              eps.NUMBER_FAMI,
                              S_DATE = eps.STAR_DATE,
                              STAR_DATE = (eps.STAR_DATE == null ? "-" : string.Format("{0:d}",eps.STAR_DATE)),
                              END_DATE = (eps.END_DATE == null ? "-" : string.Format("{0:d}", eps.END_DATE)),
                              tp.NAM_PLAN,
                          }).OrderByDescending(x=>x.S_DATE);

            return Json(new { data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListPlanEPS()
        {
            var result = (from pe in dbe.PLAN_EPS
                          select new
                          {
                              pe.ID_PLAN_EPS,
                              pe.NAM_PLAN,
                          }).OrderBy(x => x.NAM_PLAN);

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewEPSalud(EPSALUD eps)
        {
            DateTime START_DATE_EPS;
            int sw = 0, ID_PLAN_EPS = 0,NUMBER_FAMI;
            var NewEPS = new EPSALUD();

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_PLAN_EPS"]), out ID_PLAN_EPS) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["NUMBER_FAMI"]), out NUMBER_FAMI) == false) { sw = 1; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["START_DATE_EPS"]), out START_DATE_EPS) == false) { sw = 1; }
            else {
                
                NewEPS.ID_PLAN_EPS = ID_PLAN_EPS;
                NewEPS.STAR_DATE = START_DATE_EPS;
                NewEPS.NUMBER_FAMI = NUMBER_FAMI;
                NewEPS.LAST_EPS = true;            
            }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI_EPS"]);
            NewEPS.ID_PERS_ENTI = ID_PERS_ENTI;

            //var OldEPS = dbe.EPSALUDs.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.END_DATE == null);
            var OldEPS = dbe.EPSALUDs.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.END_DATE == null).FirstOrDefault();

            if (OldEPS != null)
            {
                OldEPS.END_DATE = NewEPS.STAR_DATE;
                OldEPS.LAST_EPS = false;
                dbe.EPSALUDs.Attach(OldEPS);
                dbe.Entry(OldEPS).State = EntityState.Modified;
            }

            dbe.EPSALUDs.Add(NewEPS);
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','NewEPSalud');}window.onload=init;</script>");
        }

    }
}
