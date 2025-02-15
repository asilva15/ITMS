using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Web.Security;
using Microsoft.Reporting.WebForms;  
using System.IO;
using System.Web.Services.Protocols;
using System.Configuration;
  
namespace ERPElectrodata.Controllers
{
    public class AssistanceController : Controller
    {
        public AssistanceEntities dba = new AssistanceEntities();
        public EntityEntities dbe = new EntityEntities();
        string reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();

        //public ActionResult  FilePDF()
        //{
        //    ServerReport sr = new ServerReport();
        //    sr.ReportServerUrl = new Uri(reportServer);
        //    sr.ReportPath = "/Attendance/MONTHLY_QUEUE";
            
        //    ReportParameter[] param = new ReportParameter[3];
        //    param[0] = new ReportParameter("ID_YEAR", "2014");
        //    param[1] = new ReportParameter("ID_MONTH", "3");
        //    param[2] = new ReportParameter("ID_QUEUE", "30");

        //    sr.SetParameters(param);
        //    sr.Refresh();

        //    byte[] renderedBytes;

        //    renderedBytes = sr.Render("pdf");

        //    MemoryStream memStream = new MemoryStream(renderedBytes);
        //    MemoryStream ms = new MemoryStream();

        //    var zz = File(ms.GetBuffer(), "pdf");
        //    //FileStream stream = File.Create("report.PDF", result.Length);
        //    //Console.WriteLine("File created.");
        //    //stream.Write(bytes, 0, result.Length);
        //    //Console.WriteLine("Result written to the file.");
           
        //    //stream.Close();
        //    Response.Clear();
        //    Response.ContentType = "pdf";//mimeType;
        //    Response.AddHeader("content-disposition", "filename=Attendance.jpeg"); //attachment;
        //    Response.BinaryWrite(renderedBytes);
        //    Response.End();

        //    return File(ms.GetBuffer(), "pdf");
        //}

        public ActionResult AttendanceByQueue()
        {
            return View();
        }
        public ActionResult StaffByLocation()
        {
            var query = dbe.TA_STAFF_LOCATION(4).ToList();

            return Json(new { Data = query },JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffByUEN()
        {
            var query = dbe.LIST_QUANTITY_STAFF_BY_YEAR(2014).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Equipo(int id = 0)
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int ID_ENTI = 0;

                var CLAS_ENTI = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI);

                ID_ENTI = Convert.ToInt32(CLAS_ENTI.ID_ENTI2.Value);

                var query = dbe.PERS_BY_PERS(ID_PERS_ENTI, ID_ACCO).ToList();

                return Json(new { Data = query, Count = query.Count(), ID_ENTI = ID_ENTI }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Count = 0, ID_ENTI = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TAL_ASSI_GRAP(int id = 0,int id1 = 0, int id2 = 0)
        {
            var query = dba.TAL_ASSI_GRAP(id,id1,id2);
            return Json(new { data=query}, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Assistance/

        public ActionResult Index()
        {
            ViewBag.ACCESO_NEWREQ = 0;
            //string UserName = Convert.ToString(Session["UserName"]);
            //string[] rolesArray = Roles.GetRolesForUser(UserName);

            if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
            {
                ViewBag.VerWork = "1";
            }
            else
            {
                ViewBag.VerWork = "0";
            }

            if ((int)Session["RRHH"] == 1)
            {
                ViewBag.VerWork = "1";
                ViewBag.ACCESO_SUBMENU = 1;
            }

            return View();
        }

        //
        // GET: /Assistance/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Assistance/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Assistance/Create

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
        // GET: /Assistance/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Assistance/Edit/5

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
        // GET: /Assistance/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Assistance/Delete/5

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

        [Authorize]
        public ActionResult Report_Attendance()
        {
            return View();
        }
        public ActionResult ReportAtteByPerson()
        {
            return View();
        }
        public ActionResult ReportAtteByDay()
        {
            return View();
        }

        public ActionResult ReportAtteByArea()
        {
            return View();
        }

        public ActionResult ReportAtteByCIA()
        {
            return View();
        }
        public ActionResult ReportAtteTardies()
        {
            return View();
        }
        public ActionResult ReportAtteHoursWorked()
        {
            return View();
        }
        public ActionResult ListByArea()
        {
            var result = (from lba in dbe.LIST_BY_AREA()
                          select new
                          {
                              lba.ID_CHAR,
                              lba.NAM_CHAR,

                          }).ToList();
            return Json(new { Data = result, Count = result.Count }, JsonRequestBehavior.AllowGet);

        }
    }
}
