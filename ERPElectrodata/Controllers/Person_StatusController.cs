using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class Person_StatusController : Controller
    {
        EntityEntities dbe = new EntityEntities();

        //
        // GET: /Person_Status/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAll() {
            var query = dbe.PERSON_STATUS;
            var result = (from a in query.ToList()
                          select new
                          {
                              a.ID_PERS_STAT,
                              a.NAM_STAT
                          }).OrderBy(x=>x.NAM_STAT).ToList();
            return Json(new { Data = result, Count = query.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByVisible()
        {
            var query = dbe.PERSON_STATUS.Where(x => x.VIS_STAT == true);
            var result = (from a in query.ToList()
                          select new
                          {
                              a.ID_PERS_STAT,
                              a.NAM_STAT
                          }).OrderBy(x => x.NAM_STAT).ToList();
            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListOption(int id)
        {
            var query = dbe.OPTION_STATUS.Where(x=>x.ID_PERS_STAT == id);

            var result = (from a in query
                          join ps in dbe.PERSON_STATUS on a.ID_PERS_STAT_OPTI equals ps.ID_PERS_STAT
                          select new
                          {
                              ID_PERS_STAT = a.ID_PERS_STAT_OPTI,
                              ps.NAM_STAT
                          }).OrderBy(x => x.NAM_STAT).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListForContract()
        {
            var result = (from ps in dbe.PERSON_STATUS
                          where ps.ID_PERS_STAT == 1 || ps.ID_PERS_STAT == 2
                          select new
                          {
                              ps.ID_PERS_STAT,
                              ps.NAM_STAT
                          }).OrderBy(x => x.NAM_STAT).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListReasonEndPeriod()
        {

            try
            {
                var query = (from a in dbe.REASON_END_PERIOD
                             select new
                             {
                                 a.ID_REAS_END,
                                 a.NAM_REAS,
                                 a.VIG_REAS,
                             });

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = 0, Count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
