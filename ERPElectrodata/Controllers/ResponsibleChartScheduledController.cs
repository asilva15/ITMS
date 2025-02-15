using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class ResponsibleChartScheduledController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        private AppLogEntities dba = new AppLogEntities();
        // GET: /ResponsibleChartScheduled/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /ResponsibleChartScheduled/ListByIdChar/5
        //public ActionResult ListByIdChar(int id = 0)
        //{
        //    int YEAR = DateTime.Now.Year;
        //    int MONTH = DateTime.Now.Month;

        //    try
        //    {
        //        YEAR = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
        //        MONTH = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
        //    }
        //    catch
        //    {

        //    }

        //    var query = dbe.RESPONSIBLE_CHART.Where(x => x.ID_CHAR == id)
        //        .Join(dbe.RESPONSIBLE_CHART_SCHEDULER, rc => rc.ID_RESP_CHAR, rcs => rcs.ID_RESP_CHAR, (rc, rcs) => new
        //        {
        //            rcs.ID_SCHE,
        //        })
        //        .Join(dbe.SCHEDULERs, x => x.ID_SCHE, s => s.ID_SCHE, (x, s) => new {
        //            s.ID_SCHE,
        //            s.STA_DATE,
        //            s.END_DATE,
        //            s.TIT_SCHE,
        //            s.ALL_DAY,
        //            s.ID_REPE,
        //            s.REP_EVER,
        //            s.ID_REPE_END,
        //            s.REPE_END_VALU,
        //            s.DES_SCHE,
        //            s.ID_RECURRENCE,
        //            s.EXP_RECURRENCE,
        //        });

        //    ////Eventos que no se repiten
        //    //var no_repeat = (from x in query.Where(x => x.ID_REPE == 1)
        //    //                 .Where(x => x.STA_DATE.Value.Year == YEAR && x.STA_DATE.Value.Month == MONTH || (x.END_DATE.Value.Year == YEAR && x.END_DATE.Value.Month == MONTH))
        //    //                 .ToList()
        //    //                 join rr in dbe.REPEATs on x.ID_REPE equals rr.ID_REPE
        //    //                 join re in dbe.REPEAT_END on x.ID_REPE_END equals re.ID_REPE_END
        //    //                 select new
        //    //                 {
        //    //                     start = String.Format("{0:g}", x.STA_DATE),
        //    //                     end = String.Format("{0:g}", x.END_DATE),
        //    //                     title = x.TIT_SCHE,
        //    //                     taskId = x.ID_SCHE,
        //    //                     id = x.ID_SCHE,
        //    //                     isAllDay = x.ALL_DAY,
        //    //                     recurrenceRule = ""
        //    //                 });

        //    //Eventos que se repiten y tienen fecha final
        //    var result = (from x in query.ToList()
        //                  join rr in dbe.REPEATs on x.ID_REPE equals rr.ID_REPE
        //                  join re in dbe.REPEAT_END on x.ID_REPE_END equals re.ID_REPE_END
        //                  //where x.ID_REPE != 1
        //                  select new
        //                  {
        //                      start = String.Format("{0:g}", x.STA_DATE),
        //                      end = String.Format("{0:g}", x.END_DATE),
        //                      title = x.TIT_SCHE,
        //                      taskId = x.ID_SCHE,
        //                      id = x.ID_SCHE,
        //                      description = x.DES_SCHE,
        //                      isAllDay = x.ALL_DAY,
        //                      recurrenceRule = (rr.NAM_KENDO == null ? "" :
        //                        rr.NAM_KENDO + ";INTERVAL=" + Convert.ToString(x.REP_EVER) +
        //                        (re.ID_REPE_END == 1 ? "" : 
        //                            re.ID_REPE_END == 2 ? ";" + re.NAM_KENDO + "=" + Convert.ToString(x.REPE_END_VALU) : 
        //                                ";" + re.NAM_KENDO + "=" + String.Format("{0:yyyyMMddTHHmmssZ}", DateTime.ParseExact(x.REPE_END_VALU, "yyyyMMdd", CultureInfo.InvariantCulture)))),
        //                      recurrenceId = x.ID_RECURRENCE,
        //                      recurrenceException = (x.EXP_RECURRENCE == null ? x.EXP_RECURRENCE : String.Format("{0:yyyyMMddTHHmmssZ}", DateTime.ParseExact(x.EXP_RECURRENCE, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture))),
        //                  });

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ListByID_PERS_ENTI(int id1 = 0, int id2 = -1)
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

            var query = dbe.SCHEDULER_CHART.Where(x => x.ID_PERS_ENTI == id1 || x.ID_PERS_ENTI == id2)
                .Join(dbe.SCHEDULERs.Where(x=>x.DELETE_DATE == null), x => x.ID_SCHE, s => s.ID_SCHE, (x, s) => new
                {
                    s.ID_SCHE,
                    s.STA_DATE,
                    s.END_DATE,
                    s.TIT_SCHE,
                    s.ALL_DAY,
                    s.ID_REPE,
                    s.REP_EVER,
                    s.ID_REPE_END,
                    s.REPE_END_VALU,
                    s.DES_SCHE,
                    s.ID_RECURRENCE,
                    s.EXP_RECURRENCE,
                    x.ID_CHAR,
                    x.ID_PERS_ENTI,
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
                              description = (x.DES_SCHE == null ? "" : x.DES_SCHE),
                              isAllDay = x.ALL_DAY,
                              recurrenceRule = (rr.NAM_KENDO == null ? "" :
                                rr.NAM_KENDO + ";INTERVAL=" + Convert.ToString(x.REP_EVER) +
                                (re.ID_REPE_END == 1 ? "" :
                                    re.ID_REPE_END == 2 ? ";" + re.NAM_KENDO + "=" + Convert.ToString(x.REPE_END_VALU) :
                                        ";" + re.NAM_KENDO + "=" + String.Format("{0:yyyyMMddTHHmmssZ}", DateTime.ParseExact(x.REPE_END_VALU, "yyyyMMdd", CultureInfo.InvariantCulture)))),
                              //recurrenceId = x.ID_RECURRENCE,
                              //recurrenceException = (x.EXP_RECURRENCE == null ? x.EXP_RECURRENCE : String.Format("{0:yyyyMMddTHHmmssZ}", DateTime.ParseExact(x.EXP_RECURRENCE, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture))),
                              //AccEdit = true,
                              roomId = (x.ID_PERS_ENTI == 0 ? 1 : 2),
                          });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string AccEdit(int id = 0) {
            string acceso = "0";

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            if (ID_PERS_ENTI == id) { //Usuario logeado es seleccionado
                acceso = "1";
            }
            else {
                //Determinando si el usuario logeado es responsable del Area
                var rc = dbe.RESPONSIBLE_CHART.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.VIG_RESP_CHAR == true);
                if (rc.Count() > 0) {
                    acceso = "1";
                }            
            }
            //else if (id == 0)
            //{ //Se seleccionó el area
            //    //Determinando si el usuario es responsable del Area
            //    var rc = dbe.RESPONSIBLE_CHART.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.VIG_RESP_CHAR == true);
            //    if (rc.Count() > 0)
            //    {
            //        acceso = "1";
            //    }
            //}
            return acceso;
        }

        public string AccDelete(int id = 0)
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                SCHEDULER sch = dbe.SCHEDULERs.Find(id);
                sch.DELETE_DATE = DateTime.Now;
                sch.ID_PERS_ENTI_DELE = ID_PERS_ENTI;
                dbe.SCHEDULERs.Attach(sch);
                dbe.Entry(sch).State = EntityState.Modified;
                dbe.SaveChanges();
            }
            catch (Exception e)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = e.InnerException == null ? e.Message : e.InnerException.Message;
                exc.DES_EXCE = "Error al eliminar evento en el scheduled de ventas. ID_SCHE = " + Convert.ToString(id);
                dba.EXCEPTIONs.Add(exc);
                dba.SaveChanges();
            }

            return "1";
        }
    }
}
