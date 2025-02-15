using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class NavigatorController : Controller
    {
        public ActionResult IncidentRequest()
        {
            try
            {
                //if(Convert.ToInt32(Session["MAI_CLIENT_ITO"]) == 1)
                //{
                //    return View("IncidentRequestITO");
                //}
                //else
                //{
                    return View();
                //}
            }
            catch
            {
                return Content("Please Restar Session");
            }
            
        }

        public ActionResult AssetConfiguration()
        {
            return View();
        }
        

        //
        // GET: /Navigator/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Navigator/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Navigator/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Navigator/Create

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
        // GET: /Navigator/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Navigator/Edit/5

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
        // GET: /Navigator/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Navigator/Delete/5

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
