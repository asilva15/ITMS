using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;

namespace ERPElectrodata.Controllers
{
    public class ObjectByStaffController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        //private TextInfo textInfo;

        // GET: /ObjectByStaff/

        public ActionResult Index(int id=0)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            //id = 114;
            
            var query = dbe.EVALUATION_STAFF.Single(x => x.ID_EVAL_STAF == id);  

            if (Convert.ToInt32(query.ID_PERS_ENTI) == ID_PERS_ENTI)
            {
                ViewBag.ID_EVAL_STAF = id;
                return View();
            }
            else
            {
                return Content("<b>You are not permissions to enter objectives in this evaluation</b>");
                
            }
        }

        public ActionResult ListFrecuecyMonitoring()
        {
            var query = (from x in dbe.FREQUENCY_MONITORING
                         select new { 
                         x.ID_FREQ_MONI,
                         x.NAM_FREQ_MONI,
                         });
            
            
            return Json(new {Data=query,Count=query.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListMeasure()
        {
            var query = (from x in dbe.MEASUREs
                         select new
                         {   
                             x.ID_MEAS,
                             x.NAM_MEAS,                             
                         });


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult SaveObjectivesByStaff(OBJETIVE objective)
        {
            //Este parametro recibira. 
            int ID_EVAL_STAF = Convert.ToInt32(Request.Params["ID_EVAL_STAF"]);
           // int ID_EVAL_STAF = 114;
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = dbe.EVALUATION_STAFF.Single(x => x.ID_EVAL_STAF == ID_EVAL_STAF);
            int ID_EVAL = Convert.ToInt32(query.ID_EVAL);
            string Error = "";

            OBJECT_BY_STAFF objectivesbystaff = new OBJECT_BY_STAFF();

            if (Convert.ToInt32(query.ID_PERS_ENTI) == ID_PERS_ENTI) 
            {               
                    if (String.IsNullOrEmpty(objective.NAM_OBJE))
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','You must introduce your objective');}window.onload=init;</script>");
                    }
                    else if (objective.ID_MEAS==null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','You must select a measure');}window.onload=init;</script>");
                    }
                    else if (objective.APP_WHEN == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','You must enter a date of application');}window.onload=init;</script>");
                    }
                    else if (objective.ID_FREQ_MONI == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','You must enter a monitoring frequency');}window.onload=init;</script>");
                    }
                    else
                    {
                    
                     try
                      {
                        //Guardamos el Objetivo
                        objective.ID_TYPE_OBJE = 2;
                        objective.ID_EVAL = ID_EVAL; //Este valor se le tiene que pasar de la vista del GPARS
                        objective.ID_STAT_OBJE = 1;
                        objective.CREATED = DateTime.Now;

                        dbe.OBJETIVES.Add(objective);
                        dbe.SaveChanges();

                        //Guardamos el OBJETIVE_BY_STAFF por cada objetivo creado. 
                        objectivesbystaff.ID_EVAL_STAF = ID_EVAL_STAF;
                        objectivesbystaff.ID_PERS_ENTI = ID_PERS_ENTI;
                        objectivesbystaff.ID_OBJE = objective.ID_OBJE;
                        objectivesbystaff.CREATED = DateTime.Now;
                        objectivesbystaff.STA_DATE = DateTime.Now;
                        objectivesbystaff.END_DATE = objective.APP_WHEN;
                        objectivesbystaff.ID_STAT_OBJE = 3;
                        objectivesbystaff.VIG_OBJE_STAF = true;

                        dbe.OBJECT_BY_STAFF.Add(objectivesbystaff);
                        dbe.SaveChanges();
                    }
                
                catch (Exception e)
                {
                    Error = (e.InnerException == null ? e.Message : e.InnerException.Message);

                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
                }
             }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','Your Objective has been successfully saved');}window.onload=init;</script>");
            }
            else
            { 
               return Content("<script type='text/javascript'> function init() {" +
               "if(top.uploadDone) top.uploadDone('ERRORPERMI','You are not permissions to enter objectives in this evaluation');}window.onload=init;</script>");
               
            }
        }

        public ActionResult Measure(OBJETIVE objective)
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveMeasure(MEASURE measure)
        {           

            if (!String.IsNullOrEmpty(measure.NAM_MEAS))
            {
                string NAM_MEAS = measure.NAM_MEAS.ToUpper();

                var query = dbe.MEASUREs.Where(x=>x.NAM_MEAS==NAM_MEAS);
                int Count = query.Count();

                if(Count==0)
                {
                    measure.NAM_MEAS = NAM_MEAS;
                    dbe.MEASUREs.Add(measure);
                    dbe.SaveChanges();
                    return Content("<script type='text/javascript'> function init() {" +
                              "if(top.NameMeasure) top.NameMeasure('OK');}window.onload=init;</script>");
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.NameMeasure) top.NameMeasure('ERRORCOUNT');}window.onload=init;</script>");

                }           
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                               "if(top.NameMeasure) top.NameMeasure('ERROR');}window.onload=init;</script>");
            }

            
        }
    }
}
