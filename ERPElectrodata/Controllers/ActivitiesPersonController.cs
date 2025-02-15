using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ActivitiesPersonController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(ACTIVITY_PERSON act_pers)
        {
            //string HouStarS = Convert.ToString(Request.Params["HOU_STAR"]);
            //string HourEnd = Convert.ToString(Request.Params["HOU_END"]);
            //TimeSpan HouStar ;

            //if (!String.IsNullOrEmpty(HouStarS))
            //{
            //    if (TimeSpan.TryParse(HouStarS, out HouStar))
            //    {
            //        act_pers.HOU_STAR = HouStar;
            //    }
            //}

            //if (!String.IsNullOrEmpty(HourEnd))
            //{

            //}

            if (ModelState.IsValid)
            {
                //DateTime xx = new

                if (act_pers.HOU_STAR >= act_pers.HOU_END)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('ERROR','" + "CODE" + "');}window.onload=init;</script>");
                }

                act_pers.CREATED = DateTime.Now;
                dbe.ACTIVITY_PERSON.Add(act_pers);
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('OK','" + "CODE" + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('ERROR','" + "CODE" + "');}window.onload=init;</script>");
            }
            
            //return View();
        }

        //
        // GET: /ActivitiesPerson/
        public ActionResult ListByPerson(int id = 0)
        {
            DateTime fecha = DateTime.Now;

            try
            {
                string fechaS = Convert.ToString(Request.Params["dtp_dia"]);
                fecha = Convert.ToDateTime(fechaS);
            }
            catch
            {
                return Json(new { Data = 0, Count = 0 }, JsonRequestBehavior.AllowGet);
            }

            DateTime fechalimit = fecha.AddDays(1);

            var query = dbe.ACTIVITY_PERSON.Where(x => x.ID_PERS_ENTI == id)
                .Where(x => x.CREATED >= fecha)
                .Where(x => x.CREATED < fechalimit);

            var query_t = query.ToList()
                .Select(x => Convert.ToInt32(x.ID_TICK));

            var tickets = (from t in dbc.TICKETs.Where(x=>query_t.Contains((int)x.ID_TICK))
                         select new
                         {
                             t.ID_TICK,
                             COD_TICK = t.COD_TICK
                         }).ToList();

            var result = (from ap in query.ToList()
                          join t in tickets on ap.ID_TICK equals t.ID_TICK into lt
                          from xt in lt.DefaultIfEmpty()
                          select new {
                              HOUR = String.Format("{0:hh\\:mm}", ap.HOU_STAR.Value) + " - " + String.Format("{0:hh\\:mm}", ap.HOU_END.Value),
                              //HOU_END = String.Format("{0:t}", ap.HOU_END),
                              DES_ACTI_PERS =  ap.DES_ACTI_PERS,
                              CLI_ACTI_PERS = ap.CLI_ACTI_PERS,
                              COD_TICK = (xt != null ? xt.COD_TICK : String.Empty)
                          });

            return Json(new { Data = result, Count = query.Count()}, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /ActivitiesPerson/ReportWeekly
        public ActionResult ReportWeekly(int id=0)
        {
            DateTime date_x = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            try
            {
                string fechaS = Convert.ToString(Request.Params["dtp_dia"]);
                date_x = Convert.ToDateTime(fechaS);
            }
            catch
            {
                return Json(new { Data = 0, Count = 0 }, JsonRequestBehavior.AllowGet);
            }

           

            List<DateTime> dias = new List<DateTime>();
            int i=0;
            //var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            //var ms = cal.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Sunday);

            if (Convert.ToInt32(date_x.DayOfWeek) == 1)
            {
                date_x = date_x.AddDays(-1);
            }
            else
            {
                do
                {
                    dias.Add(date_x);
                    i++;
                    date_x = date_x.AddDays(-1);
                }
                while (Convert.ToInt32(date_x.DayOfWeek) != 1);
            }

            DateTime fechalimit = date_x.AddDays(7);

            var query = (from q in dbe.ACTIVITY_PERSON.Where(x => x.ID_PERS_ENTI == id)
                         .Where(x=>x.CREATED >= date_x)
                         .Where(x => x.CREATED < fechalimit).ToList()
                         //join d in dias on q.CREATED.Value.DayOfYear equals d.DayOfYear
                         group q by q.CREATED.Value.Month into g
                         select new {
                             WorkHours = g.Sum(d => DiferenceHours((TimeSpan)d.HOU_STAR, (TimeSpan)d.HOU_END))
                         });

            var TotalHours = 48;

            var Horas = query.Sum(x=>x.WorkHours);

            var HorasWork = (TotalHours - Horas) <= 0 ? 0 : (TotalHours - Horas);

            var Overtime = ((TotalHours - Horas) >= 0 ? 0 : Math.Abs(TotalHours - Horas));

            return Json(new { WorkHours = Horas, HorasWork = HorasWork, Overtime = Overtime, TotalHours = TotalHours }, JsonRequestBehavior.AllowGet);
        }

        public double DiferenceHours(TimeSpan since,TimeSpan hasta)
        {
            var timer = (hasta - since).TotalMinutes / 60;
            return Convert.ToDouble(timer);
        }
    }
}
