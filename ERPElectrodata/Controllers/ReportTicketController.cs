using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ReportTicketController : Controller
    {
        public CoreEntities dbc = new CoreEntities();

        [Authorize]
        public ActionResult WorkLoad()
        {
            
            return View();
        }
        public ActionResult ResultWorkLoad()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var SIN_DATEs = Convert.ToString(Request.Params["SIN_DATE"]);
                var TO_DATEs = Convert.ToString(Request.Params["TO_DATE"]);
                var ID_QUEUs = Convert.ToString(Request.Params["ID_QUEU"]);

                DateTime SIN_DATE = Convert.ToDateTime(SIN_DATEs);
                DateTime TO_DATE = Convert.ToDateTime(TO_DATEs);
                int ID_QUEU = Convert.ToInt32(ID_QUEUs);

                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["rbSubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                var query = dbc.WORK_LOAD_STATUS(SIN_DATE, TO_DATE, ID_ACCO, ID_QUEU, SubCuenta).ToList();
                return Json(new { Data  = query}, JsonRequestBehavior.AllowGet);
            }
            catch{
                return Json(new { Data = "Error" }, JsonRequestBehavior.AllowGet);
            }
            
        }

        //
        // GET: /ReportTicket/
        public ActionResult WorkLoadByArea()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var SIN_DATEs = Convert.ToString(Request.Params["SIN_DATE_WA"]);
                var TO_DATEs = Convert.ToString(Request.Params["TO_DATE_WA"]);

                DateTime SIN_DATE = Convert.ToDateTime(SIN_DATEs);
                DateTime TO_DATE = Convert.ToDateTime(TO_DATEs);

                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["rbSubCuentaWA"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                var query = dbc.WORK_LOAD_AREA(SIN_DATE, TO_DATE, ID_ACCO, SubCuenta).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /ReportTicket/
        public ActionResult WorkLoadManufacturer()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var SIN_DATEs = Convert.ToString(Request.Params["SIN_DATE_WM"]);
                var TO_DATEs = Convert.ToString(Request.Params["TO_DATE_WM"]);

                DateTime SIN_DATE = Convert.ToDateTime(SIN_DATEs);
                DateTime TO_DATE = Convert.ToDateTime(TO_DATEs);

                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["rbSubCuentaWM"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                var query = dbc.WORK_LOAD_MANU(SIN_DATE, TO_DATE, ID_ACCO, SubCuenta).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /ReportTicket/
        public ActionResult WorkLoadBySupProject()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var SIN_DATEs = Convert.ToString(Request.Params["SIN_DATE_WSP"]);
                var TO_DATEs = Convert.ToString(Request.Params["TO_DATE_WSP"]);

                DateTime SIN_DATE = Convert.ToDateTime(SIN_DATEs);
                DateTime TO_DATE = Convert.ToDateTime(TO_DATEs);

                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["rbSubCuentaWSP"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                var query = dbc.WORK_LOAD_SUP_PROJE(SIN_DATE, TO_DATE, ID_ACCO, SubCuenta).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /ReportTicket/
        public ActionResult WorkLoadCIA()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var SIN_DATEs = Convert.ToString(Request.Params["SIN_DATE_WC"]);
                var TO_DATEs = Convert.ToString(Request.Params["TO_DATE_WC"]);

                DateTime SIN_DATE = Convert.ToDateTime(SIN_DATEs);
                DateTime TO_DATE = Convert.ToDateTime(TO_DATEs);

                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["rbSubCuentaWC"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                var query = dbc.WORK_LOAD_CIA(SIN_DATE, TO_DATE, ID_ACCO, SubCuenta).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        //
        // GET: /ReportTicket/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /ReportTicket/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ReportTicket/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ReportTicket/Create

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
        // GET: /ReportTicket/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ReportTicket/Edit/5

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
        // GET: /ReportTicket/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ReportTicket/Delete/5

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
