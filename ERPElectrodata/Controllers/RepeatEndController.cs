using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class RepeatEndController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /RepeatEnd/

        public ActionResult ListVig()
        {

            var query = dbe.REPEAT_END.Where(x=>x.VIG_REPE_END == true);

            var result = (from re in query.ToList()
                          select new {
                              ID_REPE_END = re.ID_REPE_END,
                              NAM_REPE_END = re.NAM_REPE_END
                          });

            return Json(new { Data = result,Count = query.Count()}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /RepeatEnd/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /RepeatEnd/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /RepeatEnd/Create

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
        // GET: /RepeatEnd/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /RepeatEnd/Edit/5

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
        // GET: /RepeatEnd/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /RepeatEnd/Delete/5

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
