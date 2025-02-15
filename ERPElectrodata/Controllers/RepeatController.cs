using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class RepeatController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /Repeat/
        public ActionResult ListVig()
        {
            var query = dbe.REPEATs.Where(x => x.VIG_REPE == true);
            var result = (from x in query.ToList()
                          select new { 
                            x.ID_REPE,
                            x.NAM_REPE
                          });

            return Json(new {Data=result,Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Repeat/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Repeat/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Repeat/Create

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
        // GET: /Repeat/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Repeat/Edit/5

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
        // GET: /Repeat/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Repeat/Delete/5

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
