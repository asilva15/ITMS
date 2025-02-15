using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class StatusFeedbackController : Controller
    {
        public CoreEntities db = new CoreEntities();

        // GET: /StatusFeedback/
        public ActionResult Index()
        {
            List<STATUS_FEEDBACK> liststafeed = new List<STATUS_FEEDBACK>();
            liststafeed = db.STATUS_FEEDBACK.ToList();
            return View(liststafeed);
        }

        //------------------CREATE---------------------------------------       
        public ActionResult Create()
        {
            var newstafeed = new STATUS_FEEDBACK();

            return View(newstafeed);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCreate(STATUS_FEEDBACK sf)
        {

            db.STATUS_FEEDBACK.Add(sf);
            db.SaveChanges();

            return Redirect("Index");

        }

        //---------------------------EDIT----------------------------------------      
        public ActionResult Edit(int id = 0)
        {
            var editstafeed = new STATUS_FEEDBACK();
            editstafeed = db.STATUS_FEEDBACK.Single(sf => sf.ID_STAT_FEED== id);

            return View(editstafeed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEdit(STATUS_FEEDBACK sf)
        {
            var stafeed = new STATUS_FEEDBACK();
            stafeed = db.STATUS_FEEDBACK.Single(stf => stf.ID_STAT_FEED == sf.ID_STAT_FEED);

            stafeed.NAM_STAT_FEED = sf.NAM_STAT_FEED;
            stafeed.DES_STAT_FEED = sf.DES_STAT_FEED;


            UpdateModel(stafeed);
            db.SaveChanges();

            return Redirect("Index");

        }
    }
}
