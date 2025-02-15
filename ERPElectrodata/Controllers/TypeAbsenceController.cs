using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeAbsenceController : Controller
    {

        public EntityEntities dbe = new EntityEntities();

        public ActionResult List()
        {
            var query = (from x in dbe.TYPE_ABSENCE.ToList()
                         select new
                         {
                             ID_TYPE_ABSE = x.ID_TYPE_ABSE,
                             NAM_TYPE_ABSE = x.NAM_TYPE_ABSE
                         });

            return Json(new { Data = query, Count = query.Count()}, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /TypeAbsence/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /TypeAbsence/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /TypeAbsence/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TypeAbsence/Create

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
        // GET: /TypeAbsence/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /TypeAbsence/Edit/5

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
        // GET: /TypeAbsence/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /TypeAbsence/Delete/5

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
