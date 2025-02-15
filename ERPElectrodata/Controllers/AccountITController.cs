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
    public class AccountITController : Controller
    {
        private CoreEntities db = new CoreEntities();

        public ActionResult ChangeAccount(int id)
        {
            try{
                var ACCO = db.ACCOUNTs.Single(a => a.ID_ACCO == id);
                Session["ID_ACCO"] = ACCO.ID_ACCO;
                Session["NAM_ACCO"] = ACCO.NAM_ACCO;

            return Content("OK");
            }
            catch{
                return Content("ERROR");
            }
        }
        //
        // GET: /AccountIT/

        public ActionResult Index()
        {
            return View(db.ACCOUNTs.ToList());
        }

        //
        // GET: /AccountIT/Details/5

        public ActionResult Details(int id = 0)
        {
            ACCOUNT account = db.ACCOUNTs.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        //
        // GET: /AccountIT/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AccountIT/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ACCOUNT account)
        {
            if (ModelState.IsValid)
            {
                db.ACCOUNTs.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        //
        // GET: /AccountIT/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ACCOUNT account = db.ACCOUNTs.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        //
        // POST: /AccountIT/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ACCOUNT account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        //
        // GET: /AccountIT/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ACCOUNT account = db.ACCOUNTs.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        //
        // POST: /AccountIT/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ACCOUNT account = db.ACCOUNTs.Find(id);
            db.ACCOUNTs.Remove(account);
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