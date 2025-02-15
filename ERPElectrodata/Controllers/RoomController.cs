using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class RoomController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /Room/
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Date = String.Format("{0:d}", DateTime.Now);
            ViewBag.DateTime = String.Format("{0:g}", DateTime.Now);
            return View();
        }

        public ActionResult ListRoom()
        {
            var query = dbe.ROOMs.Where(x=>x.VIG_ROOM == true);

            var result = (from x in query.ToList()
                          select new {
                              NAM_ROOM = x.NAM_ROOM,
                              ID_ROOM = x.ID_ROOM
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Room/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Room/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Room/Create

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
        // GET: /Room/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Room/Edit/5

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
        // GET: /Room/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Room/Delete/5

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
