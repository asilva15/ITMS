using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;

namespace ERPElectrodata.Controllers
{
    public class RoomSchedulerController : Controller
    {

        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /RoomScheduler/
        public ActionResult ListByIdRoom(int id)
        {
            int YEAR = DateTime.Now.Year;
            int MONTH = DateTime.Now.Month;

            try
            {
                YEAR = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
                MONTH = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
            }
            catch
            {

            }
            
            var query = dbe.SCHEDULERs
                .Join(dbe.ROOM_SCHEDULER.Where(x=>x.ID_ROOM == id), s => s.ID_SCHE, rs => rs.ID_SCHE, (s, rs) => new { 
                    s.ID_SCHE,s.STA_DATE,s.END_DATE,s.TIT_SCHE,s.ALL_DAY,s.ID_REPE,s.REP_EVER,s.ID_REPE_END,s.REPE_END_VALU
                });

            //Eventos que no se repiten
            var no_repeat = (from x in query.Where(x => x.ID_REPE == 1)
                             .Where(x=>x.STA_DATE.Value.Year == YEAR && x.STA_DATE.Value.Month == MONTH || (x.END_DATE.Value.Year == YEAR && x.END_DATE.Value.Month == MONTH ))
                             .ToList()
                             join rr in dbe.REPEATs on x.ID_REPE equals rr.ID_REPE
                             join re in dbe.REPEAT_END on x.ID_REPE_END equals re.ID_REPE_END
                             select new { 
                              start = String.Format("{0:g}", x.STA_DATE),
                              end = String.Format("{0:g}", x.END_DATE),
                              title = x.TIT_SCHE,
                              taskId = x.ID_SCHE,
                              id = x.ID_SCHE,
                              isAllDay = x.ALL_DAY,
                              recurrenceRule = ""
                             });

            //Eventos que se repiten y tienen fecha final
            var result = (from x in query.ToList()
                          join rr in dbe.REPEATs on x.ID_REPE equals rr.ID_REPE
                          join re in dbe.REPEAT_END on x.ID_REPE_END equals re.ID_REPE_END
                          //where x.ID_REPE != 1
                          select new
                          {
                              start = String.Format("{0:g}", x.STA_DATE),
                              end = String.Format("{0:g}", x.END_DATE),
                              title = x.TIT_SCHE,
                              taskId = x.ID_SCHE,
                              id = x.ID_SCHE,
                              isAllDay = x.ALL_DAY,
                              recurrenceRule = rr.NAM_KENDO+";INTERVAL="+Convert.ToString(x.REP_EVER)+
                                (re.ID_REPE_END == 1 ? "" : re.ID_REPE_END == 2 ? ";" + re.NAM_KENDO + "=" + Convert.ToString(x.REPE_END_VALU) : ";" + re.NAM_KENDO + "=" + String.Format("{0:yyyyMMddTHHmmssZ}", DateTime.ParseExact(x.REPE_END_VALU,"yyyyMMdd",CultureInfo.InvariantCulture)))
                          });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /RoomScheduler/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /RoomScheduler/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /RoomScheduler/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /RoomScheduler/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /RoomScheduler/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /RoomScheduler/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /RoomScheduler/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
