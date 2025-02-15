using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class PriorityController : Controller
    {
        private CoreEntities db = new CoreEntities();

        //
        // GET: /Priority/
        public ActionResult List()
        {
            var result = (from p in db.PRIORITies.ToList()
                          select new
                          {
                              p.NAM_PRIO,
                              p.ID_PRIO,
                              p.HOU_PRIO
                          });

            return Json(new { Data = result, Count=result.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarMenor(int id)
        {
            var result = (from p in db.PRIORITies.ToList()
                          select new
                          {
                              p.NAM_PRIO,
                              p.ID_PRIO,
                              p.HOU_PRIO
                          });//}).Where(x=>x.ID_PRIO <= id);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPrioridadTicket(string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = db.ListarPrioridadTicket(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarPrioridadTicketTR()
        {
            //string termino = "";

            //if (Request.QueryString["q"] == null)
            //{
            //    termino = "";
            //}
            //else
            //{
            //    termino = Request.QueryString["q"].ToString();
            //}
            var result = db.ListarPrioridadTicket("").ToList();
            
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Priority/

        public ActionResult Index()
        {
            return View(db.PRIORITies.ToList());
        }

        //
        // GET: /Priority/Details/5

        public ActionResult Details(int id = 0)
        {
            PRIORITY priority = db.PRIORITies.Find(id);
            if (priority == null)
            {
                return HttpNotFound();
            }
            return View(priority);
        }

        //
        // GET: /Priority/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Priority/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PRIORITY priority)
        {
            if (ModelState.IsValid)
            {
                db.PRIORITies.Add(priority);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(priority);
        }

        //
        // GET: /Priority/Edit/5

        public ActionResult Edit(int id = 0)
        {
            PRIORITY priority = db.PRIORITies.Find(id);
            if (priority == null)
            {
                return HttpNotFound();
            }
            return View(priority);
        }

        //
        // POST: /Priority/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PRIORITY priority)
        {
            if (ModelState.IsValid)
            {
                db.Entry(priority).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(priority);
        }

        //
        // GET: /Priority/Delete/5

        public ActionResult Delete(int id = 0)
        {
            PRIORITY priority = db.PRIORITies.Find(id);
            if (priority == null)
            {
                return HttpNotFound();
            }
            return View(priority);
        }

        //
        // POST: /Priority/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            PRIORITY priority = db.PRIORITies.Find(id);
            db.PRIORITies.Remove(priority);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}