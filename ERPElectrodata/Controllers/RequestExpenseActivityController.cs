using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;

namespace ERPElectrodata.Controllers
{
    public class RequestExpenseActivityController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        // GET: /RequestExpenseActivity/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListActivity(int id = 0) {

            CultureInfo csi = new CultureInfo("Es-Es");
            var qUser = (from a in dbe.CLASS_ENTITY.Where(x=> x.UserId != null)
                         select new {
                            User = ((a.FIR_NAME == null ? "" : a.FIR_NAME) + " " + 
                                    (a.LAS_NAME == null ? "" : a.LAS_NAME)).ToLower(),
                            a.UserId
                         });
                         
            var query = (from a in dbc.REQUEST_EXPENSE_ACTIVITY.Where(x=>x.ID_REQU_EXPE == id).ToList()
                         join b in dbc.STATUS_REQUEST_EXPENSE on a.ID_STAT_REQU_EXPE equals b.ID_STAT_REQU_EXPE
                         join c in qUser on a.UserId equals c.UserId
                         select new {
                             DAT_STAR = (a.DAT_STAR == null ? "" : csi.DateTimeFormat.GetDayName(Convert.ToDateTime(a.DAT_STAR).DayOfWeek) + " " + String.Format("{0:MMM dd yyyy}", a.DAT_STAR).ToLower()),
                             DAT_HH = (a.DAT_STAR == null ? "" : String.Format("{0:hh:mm tt}", a.DAT_STAR).ToLower()),
                             NAM_STAT_REQU_EXPE = ProperCase(b.NAM_STAT_REQU_EXPE),
                             User = ProperCase(c.User),
                             a.ID_REQU_EXPE_ACTI,
                             REASON_REJECT = (a.REASON_REJECT == null ? "" : a.REASON_REJECT),
                             a.ID_STAT_REQU_EXPE,
                         });
            var qds = query.ToList();
            return Json(new{Data = query, Count = query.Count()},JsonRequestBehavior.AllowGet);
        }

        public static string ProperCase(string s)
        {
            s = s.ToLower();
            string sProper = "";

            char[] seps = new char[] { ' ' };            
            foreach (string ss in s.Split(seps))
            {
                if (ss != "") {
                    sProper += char.ToUpper(ss[0]);
                    sProper += (ss.Substring(1, ss.Length - 1) + ' ');                
                }                
            }

            return sProper;
        }
    }
}
