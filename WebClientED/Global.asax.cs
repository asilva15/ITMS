using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebClientED.Plugins;
using WebClientED.Models;
using WebClientED.Plugins.SurveyTicket;
using System.Data.Entity;

namespace WebClientED
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["Temporal"] = new System.Threading.Timer(new System.Threading.TimerCallback(GeneraEncuesta), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0,0,3, 0, 0));
            Application["ResultsSurveyTicket"] = new System.Threading.Timer(new System.Threading.TimerCallback(ResultsSurveyTicket), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 0, 1, 0, 0));

        }

        public void ResultsSurveyTicket(object state)
        {

            try
            {
                var fecha = DateTime.Now;
                //if (fecha.Hour == 17)
                //{
                   var qsurvey = (from s in dbe.SURVEYs.Where(x => x.VIG_TYPE_SURV == true)
                                  join st in dbe.SURVEY_TICKET.Where(x => x.ID_SURV_STAT == 3 && x.SEN_REPO == false) on s.ID_SURV equals st.ID_SURV //ID_SURV_STAT==3 Encuesta respondida
                                   select new
                                   {
                                       st.ID_SURV_TICK,
                                       st.ID_TICK,
                                   }).ToList();

                    var qticket = (from qs in qsurvey
                                   join t in dbc.TICKETs on qs.ID_TICK equals t.ID_TICK
                                   select new
                                   {
                                       qs.ID_SURV_TICK,
                                       t.ID_TICK,
                                       t.COD_TICK,
                                       t.ID_PERS_ENTI,
                                       t.ID_ACCO,
                                   }).ToList();

                    var query = (from qt in qticket
                                 join pe in dbe.PERSON_ENTITY on qt.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                                 join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                 select new
                                 {
                                     ID_PERS_ENTI = pe.ID_PERS_ENTI,
                                     ID_TICK = qt.ID_TICK,
                                     ID_SURV_TICK = qt.ID_SURV_TICK,
                                     ID_ACCO = qt.ID_ACCO,
                                     //EMA_ELEC = pe.EMA_ELEC == null ? "lsempertegui@electrodata.com.pe" : pe.EMA_ELEC,
                                     EMA_PERS = pe.EMA_PERS,
                                     FIR_NAME = ce.FIR_NAME.ToLower() + " " + (String.IsNullOrEmpty(ce.LAS_NAME) ? "" : ce.LAS_NAME.ToLower()),
                                     SEX_ENTI = String.IsNullOrEmpty(ce.SEX_ENTI) ? String.Empty : ce.SEX_ENTI,
                                     DESC_FILE = qt.COD_TICK == null ? "Resultado Encuesta" + Convert.ToString(qt.ID_TICK) : qt.COD_TICK,
                                     FIR_NAME1 = ce.FIR_NAME.ToLower(),
                                 }).ToList();

                    foreach (var pers in query)
                    {
                        try
                        {
                            int ID_PERS_ENTI = pers.ID_PERS_ENTI;
                            string DESC_FILE = pers.DESC_FILE;
                            int ID_TICK = pers.ID_TICK;
                            try
                            {
                                //var q = dbe.PERSON_ENTITY_NOTIFICATION
                                //    .Where(x => x.ID_PERS_ENTI == pers.ID_PERS_ENTI && x.ID_PERS_NOTI == 6)
                                //    .Where(x => x.CREATED.Value.Day == fecha.Day)
                                //    .Where(x => x.CREATED.Value.Month == fecha.Month)
                                //    .Where(x => x.CREATED.Value.Year == fecha.Year)
                                //    .Count();
                                var q = 0;

                                if (q == 0)
                                {
                                    ReportSurveyTick rpt = new ReportSurveyTick();
                                    string RST = rpt.SurveyTick(ID_TICK, DESC_FILE, pers.SEX_ENTI, pers.FIR_NAME1, pers.EMA_PERS,pers.ID_ACCO.Value);

                                    if (RST == "OK")
                                    {
                                        PERSON_ENTITY_NOTIFICATION noti_atte_report = new PERSON_ENTITY_NOTIFICATION();
                                        noti_atte_report.ID_PERS_ENTI = pers.ID_PERS_ENTI;
                                        noti_atte_report.ID_PERS_NOTI = 6;
                                        noti_atte_report.CREATED = DateTime.Now;
                                        noti_atte_report.UserId = 29;
                                        dbe.PERSON_ENTITY_NOTIFICATION.Add(noti_atte_report);
                                        dbe.SaveChanges();

                                        SURVEY_TICKET ust = dbe.SURVEY_TICKET.Find(pers.ID_SURV_TICK);
                                        ust.SEN_REPO = true;
                                        dbe.SURVEY_TICKET.Attach(ust);
                                        dbe.Entry(ust).State = EntityState.Modified;
                                        dbe.SaveChanges();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var exc = new EXCEPTION();
                                exc.DAT_EXCE = DateTime.Now;
                                exc.MESSAGE = ex.Message;
                                exc.DES_EXCE = "Global asax - ResultsSurveyTicket - 1";
                                dbl.EXCEPTION.Add(exc);
                                dbl.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            var exc = new EXCEPTION();
                            exc.DAT_EXCE = DateTime.Now;
                            exc.MESSAGE = ex.Message;
                            exc.DES_EXCE = "Global asax - ResultsSurveyTicket - 2";
                            dbl.EXCEPTION.Add(exc);
                            dbl.SaveChanges();
                        }
                    }
                //}
            }
            catch (Exception ex)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = ex.Message;
                exc.DES_EXCE = "Global asax - ResultsSurveyTicket - 3";
                dbl.EXCEPTION.Add(exc);
                dbl.SaveChanges();
            }
        }

        public void GeneraEncuesta(object state)
        {

            try
            {
                //var query = 
                Tickets wt = new Tickets();
                //wt.GeneraEncuesta();
                //wt.EnviarEncuesta();
            }
            catch (Exception ex)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = ex.Message;
                exc.DES_EXCE = "Global asax - GeneraEncuesta";
                dbl.EXCEPTION.Add(exc);
                dbl.SaveChanges();
            }
        }
    }
}
