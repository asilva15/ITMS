using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using ERPElectrodata.Models.Enum;

namespace ERPElectrodata.Controllers
{
    public class SourceTicketController : Controller
    {
        private CoreEntities db = new CoreEntities();

        public ActionResult List()
        {
            int id_acco = Convert.ToInt32(Session["ID_ACCO"]);
            var result = (from s in db.SOURCEs
                          select new
                          {
                              s.ID_SOUR,
                              s.NAM_SOUR,
                              s.VIG_SOUR
                          }).Where(x => x.VIG_SOUR == true);
            if(id_acco != 60)
            {
                result = result.Where(x => x.ID_SOUR != 11);
            }
            if(id_acco == (int)Minsur.MINSUR || id_acco == (int)Minsur.MARCOBRE || id_acco == (int)Minsur.RAURA)
            {
                result = result.Where(x => x.ID_SOUR != 6 && x.ID_SOUR != 10);
            }
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /SourceTicket/

        public ActionResult Index()
        {
            return View(db.SOURCEs.ToList());
        }

        //
        // GET: /SourceTicket/Details/5

        public ActionResult Details(int id = 0)
        {
            SOURCE source = db.SOURCEs.Find(id);
            if (source == null)
            {
                return HttpNotFound();
            }
            return View(source);
        }

        //
        // GET: /SourceTicket/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SourceTicket/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SOURCE source)
        {
            if (ModelState.IsValid)
            {
                db.SOURCEs.Add(source);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(source);
        }

        //
        // GET: /SourceTicket/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SOURCE source = db.SOURCEs.Find(id);
            if (source == null)
            {
                return HttpNotFound();
            }
            return View(source);
        }

        //
        // POST: /SourceTicket/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SOURCE source)
        {
            if (ModelState.IsValid)
            {
                db.Entry(source).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(source);
        }

        //
        // GET: /SourceTicket/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SOURCE source = db.SOURCEs.Find(id);
            if (source == null)
            {
                return HttpNotFound();
            }
            return View(source);
        }

        //
        // POST: /SourceTicket/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SOURCE source = db.SOURCEs.Find(id);
            db.SOURCEs.Remove(source);
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