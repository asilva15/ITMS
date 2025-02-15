using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using System.Globalization;

namespace ERPElectrodata.Controllers
{
    public class SchedulerController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        
        // GET: /Scheduler/
        public ActionResult Index()
        {
            return View();
        }
        
        // GET: /Scheduler/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
       
        // GET: /Scheduler/Create
        public ActionResult CreateRoom(int id)
        {

            string start = Convert.ToString(Request.Params["start"]);
            string end = Convert.ToString(Request.Params["end"]);
            //var sche = new SCHEDULER();

            ViewBag.start = start;
            ViewBag.end = end;

            return View();
        }
      
        // POST: /Scheduler/Create
        [HttpPost,ValidateInput(false)]
        public ActionResult CreateRoom(FormCollection collection, SCHEDULER scheduler)
        {

            int ID_ROOM = 0, ID_REQU = 0,VAL_DATA = 0;
            DateTime START_DATE, END_DATE, REPE_END_VALU_DATE;
            try
            {
                if (!String.IsNullOrEmpty(Request.Params["ID_REQU"]))
                {
                    if (Int32.TryParse(Convert.ToString(Request.Params["ID_ROOM"]), out ID_ROOM) == true)
                    {
                        VAL_DATA = 1;
                    }
                }
                else
                {
                    VAL_DATA = 0;
                }

                if (!String.IsNullOrEmpty(Request.Params["ID_REQU"]))
                {
                    if (Int32.TryParse(Convert.ToString(Request.Params["ID_REQU"]), out ID_REQU) == true)
                    {
                        VAL_DATA = 1;
                    }
                }
                else
                {
                    VAL_DATA = 0;
                }

                if(String.IsNullOrEmpty(scheduler.TIT_SCHE))
                {
                    VAL_DATA = 0;
                }

                if (DateTime.TryParse(Convert.ToString(scheduler.STA_DATE), out START_DATE))
                {
                    if (START_DATE == DateTime.MinValue)
                    {
                        VAL_DATA = 0;
                    }
                }

                if (DateTime.TryParse(Convert.ToString(scheduler.END_DATE), out END_DATE))
                {
                    if (END_DATE == DateTime.MinValue)
                    {
                        VAL_DATA = 0;
                    }
                }

                if (scheduler.ID_REPE_END == 3)
                {
                    if (DateTime.TryParse(Convert.ToString(scheduler.REPE_END_VALU), out REPE_END_VALU_DATE))
                    {
                        scheduler.REPE_END_VALU = String.Format("{0:yyyyMMdd}", REPE_END_VALU_DATE);
                    }
                    else
                    {
                        VAL_DATA = 0;
                    }
                }
                // TODO: Add insert logic here

                if (VAL_DATA == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                try
                {
                    
                    
                    dbe.SCHEDULERs.Add(scheduler);
                    dbe.SaveChanges();

                    var ROOM_SCHE = new ROOM_SCHEDULER();
                    ROOM_SCHE.ID_SCHE = scheduler.ID_SCHE;
                    ROOM_SCHE.ID_ROOM = ID_ROOM;
                    ROOM_SCHE.ID_PERS_ENTI = ID_REQU;
                    dbe.ROOM_SCHEDULER.Add(ROOM_SCHE);
                    dbe.SaveChanges();
                }
                catch
                {

                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
        }
        
        // GET: /Scheduler/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
       
        // POST: /Scheduler/Edit/5
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
      
        // GET: /Scheduler/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
        
        // POST: /Scheduler/Delete/5
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

        [ValidateInput(false)]
        public ActionResult viewCreateEvent(){
            string acc = Convert.ToString(Request.Params["acc"]);
            string start = Convert.ToString(Request.Params["start"]);
            string end = Convert.ToString(Request.Params["end"]);
            string proc = Convert.ToString(Request.Params["proc"]);
            string idper = Convert.ToString(Request.Params["idper"]);            

            ViewBag.acc = acc;
            ViewBag.start = start;
            ViewBag.end = end;
            ViewBag.ID_PERS_ENTI = idper;
            ViewBag.proc = proc;
            ViewBag.ID_SCHE = 0;
            ViewBag.title = "";
            ViewBag.description = "";
            ViewBag.titleAccion = ResourceLanguaje.Resource.AddEvent;
            ViewBag.titleButton = ResourceLanguaje.Resource.BtnCreate;
            if (acc == "edit")
            {
                var sche = new SCHEDULER();

                string id = Convert.ToString(Request.Params["id"]);
                string title = Convert.ToString(Request.Params["title"]);
                string desc = Convert.ToString(Request.Params["desc"]);
                ViewBag.title = title;
                ViewBag.description = desc;
                ViewBag.ID_SCHE = id;
                sche.DES_SCHE = desc;
                ViewBag.titleAccion = ResourceLanguaje.Resource.EditEvent;
                ViewBag.titleButton = ResourceLanguaje.Resource.BtnSave;

                sche = dbe.SCHEDULERs.Find(Convert.ToInt32(id));
                ViewBag.ID_REPE_END = sche.ID_REPE_END;
                ViewBag.REPE_END_VALUE = sche.REPE_END_VALU;
                if (sche.ID_REPE_END != 1)
                {
                    if (sche.ID_REPE_END == 3)
                    {
                        ViewBag.REPE_END_VALUE = DateTime.ParseExact(sche.REPE_END_VALU, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                }
                return View(sche);
            }
            else { return View(); }            
        }

        // POST: /Scheduler/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult CreateEvent(FormCollection collection, SCHEDULER scheduler)
        {
            string acc = Convert.ToString(Request.Form["accHF"]);

            int VAL_DATA = 1;
            DateTime START_DATE, END_DATE, REPE_END_VALU_DATE;
            try
            {
                if (String.IsNullOrEmpty(scheduler.TIT_SCHE))
                {
                    VAL_DATA = 0;
                }

                if (DateTime.TryParse(Convert.ToString(scheduler.STA_DATE), out START_DATE))
                {
                    if (START_DATE == DateTime.MinValue)
                    {
                        VAL_DATA = 0;
                    }
                }
                else { VAL_DATA = 0; }

                if (DateTime.TryParse(Convert.ToString(scheduler.END_DATE), out END_DATE))
                {
                    if (END_DATE == DateTime.MinValue)
                    {
                        VAL_DATA = 0;
                    }
                }
                else { VAL_DATA = 0; }

                if (scheduler.ID_REPE_END == 3)
                {
                    if (DateTime.TryParse(Convert.ToString(scheduler.REPE_END_VALU), out REPE_END_VALU_DATE))
                    {
                        scheduler.REPE_END_VALU = String.Format("{0:yyyyMMdd}", REPE_END_VALU_DATE);
                    }
                    else
                    {
                        VAL_DATA = 0;
                    }
                }
                
                // TODO: Add insert logic here
                if (VAL_DATA == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                try
                {
                    if (acc == "new")
                    {
                        int UserId = Convert.ToInt32(Session["UserId"]);
                        scheduler.CREATE_DATE = DateTime.Now;
                        scheduler.UserId = UserId;
                        dbe.SCHEDULERs.Add(scheduler);
                        dbe.SaveChanges();

                        //var respchar = new RESPONSIBLE_CHART_SCHEDULER();
                        //respchar.ID_RESP_CHAR = 1;
                        //respchar.ID_SCHE = scheduler.ID_SCHE;
                        //respchar.DAT_REGI = DateTime.Now;
                        //dbe.RESPONSIBLE_CHART_SCHEDULER.Add(respchar);
                        //dbe.SaveChanges();
                        int idper = Convert.ToInt32(Request.Form["ID_PERS_ENTI_HF"]);

                        var schcha = new SCHEDULER_CHART();
                        schcha.ID_SCHE = scheduler.ID_SCHE;
                        schcha.ID_CHAR = 5;
                        schcha.ID_PERS_ENTI = idper;
                        dbe.SCHEDULER_CHART.Add(schcha);
                        dbe.SaveChanges();

                    }
                    else { //Editar
                        var sch = dbe.SCHEDULERs.Single(x=>x.ID_SCHE == scheduler.ID_SCHE);
                        var hsc = new HISTORY_SCHEDULER();

                        hsc.CREATE_DATE_HIST = DateTime.Now;
                        hsc.ID_USER_HIST = Convert.ToInt32(Session["UserId"]);
                        hsc.ID_SCHE = scheduler.ID_SCHE;
                        hsc.STA_DATE = sch.STA_DATE;
                        hsc.END_DATE = sch.END_DATE;
                        hsc.ALL_DAY = sch.ALL_DAY;
                        hsc.TIT_SCHE = sch.TIT_SCHE;
                        hsc.ID_REPE = sch.ID_REPE;
                        hsc.REP_EVER = sch.REP_EVER;                        
                        hsc.REP_ON = sch.REP_ON;
                        hsc.ID_REPE_END = sch.ID_REPE_END;
                        hsc.REPE_END_VALU = sch.REPE_END_VALU;
                        hsc.END_SCHE = sch.END_SCHE;
                        hsc.DES_SCHE = sch.DES_SCHE;
                        hsc.UserId = sch.UserId;
                        hsc.CREATE_DATE = sch.CREATE_DATE;
                        hsc.DELETE_DATE = sch.DELETE_DATE;
                        dbe.HISTORY_SCHEDULER.Add(hsc);
                        dbe.SaveChanges();

                        using(EntityEntities db = new EntityEntities())
                        {
                            SCHEDULER Nsch = db.SCHEDULERs.Find(scheduler.ID_SCHE);

                            Nsch.CREATE_DATE = DateTime.Now;
                            Nsch.ID_SCHE = scheduler.ID_SCHE;
                            Nsch.STA_DATE = scheduler.STA_DATE;
                            Nsch.END_DATE = scheduler.END_DATE;
                            Nsch.ALL_DAY = scheduler.ALL_DAY;
                            Nsch.TIT_SCHE = scheduler.TIT_SCHE;
                            Nsch.ID_REPE = scheduler.ID_REPE;
                            Nsch.REP_EVER = scheduler.REP_EVER;                        
                            Nsch.REP_ON = scheduler.REP_ON;
                            Nsch.ID_REPE_END = scheduler.ID_REPE_END;
                            Nsch.REPE_END_VALU = scheduler.REPE_END_VALU;
                            Nsch.END_SCHE = scheduler.END_SCHE;
                            Nsch.DES_SCHE = scheduler.DES_SCHE;
                            Nsch.UserId = Convert.ToInt32(Session["UserId"]);
                            Nsch.CREATE_DATE = scheduler.CREATE_DATE;
                            Nsch.DELETE_DATE = scheduler.DELETE_DATE;
                            Nsch.MODIFIED_DATE = DateTime.Now;

                            db.SCHEDULERs.Attach(Nsch);
                            db.Entry(Nsch).State = EntityState.Modified;
                            db.SaveChanges();
                        }                        
                    }
                }
                catch(Exception e)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
        }

        public ActionResult viewSchedulerPersonal(int id = 0,string acceso = "",int id1 = -1) {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.ID_AREA_SALE = id1;
            ViewBag.acceso = acceso;
            ViewBag.Date = String.Format("{0:d}", DateTime.Now);
            //ViewBag.DateTime = String.Format("{0:g}", DateTime.Now);

            ViewBag.TimeStar = String.Format("{0:d}", DateTime.Now) + " 08:00 AM";
            ViewBag.TimeEnd = String.Format("{0:d}", DateTime.Now.AddDays(1)) + " 08:00 AM";

            return View();
        }

    }
}
