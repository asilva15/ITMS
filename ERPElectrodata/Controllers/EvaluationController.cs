using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class EvaluationController : Controller
    {
        //
        // GET: /Evaluation/

        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var qEval = (from a in dbe.EVALUATIONs.ToList()
                         join b in dbe.STATUS_EVALUATION on a.ID_STAT_EVAL equals b.ID_STAT_EVAL
                         join c in dbe.CLASS_ENTITY on a.UserId equals c.UserId
                         select new
                         {
                             a.ID_EVAL,
                             a.COD_EVAL,
                             a.NAM_EVAL,
                             a.DES_EVAL,
                             DAT_START = (a.DAT_START == null ? "" : String.Format("{0:d}", a.DAT_START)),
                             DAT_END = (a.DAT_END == null ? "" : String.Format("{0:d}", a.DAT_END)),
                             NAM_STAT_EVAL = b.NAM_STAT_EVAL.ToLower(),
                             CREATED = (a.CREATED == null ? "" : String.Format("{0:d}", a.CREATED)),
                             USER = (c.FIR_NAME + " " + c.LAS_NAME).ToLower(),
                         });

            return Json(new { Data = qEval, Count = qEval.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id = 0) {
            var qEVAL = dbe.EVALUATIONs.Single(x => x.ID_EVAL == id);
            
            ViewBag.ID_EVAL = id;
            ViewBag.COD_EVAL = qEVAL.COD_EVAL;
            return View();
        }

        public ActionResult ListDetailsEvaluation(int id = 0) {

            var qEval = (from a in dbe.EVALUATIONs.Where(x=>x.ID_EVAL == id).ToList()
                         join b in dbe.STATUS_EVALUATION on a.ID_STAT_EVAL equals b.ID_STAT_EVAL
                         join c in dbe.CLASS_ENTITY on a.UserId equals c.UserId
                         join d in dbe.PERSON_ENTITY.Where(x=>x.ID_ENTI1 == 9) on c.ID_ENTI equals d.ID_ENTI2
                         select new
                         {
                             a.ID_EVAL,
                             a.COD_EVAL,
                             a.NAM_EVAL,
                             a.DES_EVAL,
                             DAT_START = (a.DAT_START == null ? "" : String.Format("{0:d}", a.DAT_START)),
                             DAT_END = (a.DAT_END == null ? "" : String.Format("{0:d}", a.DAT_END)),
                             NAM_STAT_EVAL = b.NAM_STAT_EVAL.ToLower(),
                             CREATED = (a.CREATED == null ? "" : String.Format("{0:d}", a.CREATED)),
                             USER = (c.FIR_NAME + " " + c.LAS_NAME).ToLower(),
                             d.ID_FOTO,
                         });

            var qObje = (from a in dbe.OBJETIVES.Where(x => x.ID_EVAL == id).ToList()
                         join b in dbe.STATUS_OBJETIVES on a.ID_STAT_OBJE equals b.ID_STAT_OBJE
                         join c in dbe.FREQUENCY_MONITORING on a.ID_FREQ_MONI equals c.ID_FREQ_MONI
                         join d in dbe.TYPE_OBJECTIVE on a.ID_TYPE_OBJE equals d.ID_TYPE_OBJE
                         select new { 
                             a.ID_EVAL,
                             a.ID_OBJE,
                             a.NAM_OBJE,
                             a.DES_OBJE,                             
                             a.ID_TYPE_OBJE,
                             d.NAM_TYPE_OBJE,
                             a.MEASURED,                             
                             APP_WHEN = (a.APP_WHEN == null ? "-" : String.Format("{0:d}",a.APP_WHEN)),
                             b.NAM_STAT_OBJE,
                             c.NAM_FREQ_MONI,
                         });

            return Json(new { Data = qEval, Count = qEval.Count(), Data1 = qObje }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListStatusObjective() {
            var qSTATOBJ = dbe.STATUS_OBJETIVES.Where(x=>x.VIG_STAT_OBJE == true).ToList();

            return Json(new { Data = qSTATOBJ, Count = qSTATOBJ.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListTypeObjective()
        {
            var qTypeObje = dbe.TYPE_OBJECTIVE.Where(x=>x.VIG_TYPE_OBJE == true).ToList();

            return Json(new { Data = qTypeObje, Count = qTypeObje.Count() }, JsonRequestBehavior.AllowGet);
        }        

        public ActionResult ListFreqMonitoring()
        {
            var qFreqMoni = dbe.FREQUENCY_MONITORING.ToList();

            return Json(new { Data = qFreqMoni, Count = qFreqMoni.Count() }, JsonRequestBehavior.AllowGet);
        }
        

    }
}
