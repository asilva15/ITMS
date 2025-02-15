using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class AbsenceController : Controller
    {
        public EntityEntities dbe = new EntityEntities();

        //LIsta de
        public ActionResult List()
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbe.ABSENCEs.ToList();

            var result = (from a in query.OrderByDescending(x=>x.SIN_DATE).Skip(skip).Take(take).ToList()
                          join ta in dbe.TYPE_ABSENCE on a.ID_TYPE_ABSE equals ta.ID_TYPE_ABSE
                          join pe in dbe.PERSON_ENTITY on a.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join cee in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals cee.ID_ENTI
                          join ce in dbe.CLASS_ENTITY on a.UserId equals ce.UserId
                          //join peu in dbe.PERSON_ENTITY on ce.ID_ENTI equals peu.ID_ENTI2
                              select new{
                                  SIN_DATE = String.Format("{0:G}", a.SIN_DATE),
                                  TO_DATE = String.Format("{0:G}", a.TO_DATE),
                                  RET_DATE = String.Format("{0:G}", a.RET_DATE),
                                  EMP_ABSE = cee.FIR_NAME+" "+cee.LAS_NAME,
                                  EMP_ABSE_FOTO = pe.ID_FOTO,
                                  NAM_TYPE_ABSE = ta.NAM_TYPE_ABSE,
                                  NUM_DAYS = a.NUM_DAYS,
                                  SUM = HttpUtility.HtmlDecode(a.SUM)//a.SUM
                              });
            return Json(new { Data= result, Count = query.Count},JsonRequestBehavior.AllowGet);

        }
        //
        // GET: /Absence/

        public ActionResult Index()
        {
            
            return View();
        }

        //
        // GET: /Absence/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Absence/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Absence/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection,ABSENCE newAbsence)
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);

                //if(collection[""])
                if (ModelState.IsValid && UserId != 0)
                {
                    var xx =  newAbsence.TO_DATE.Value.Subtract(newAbsence.SIN_DATE.Value).TotalSeconds;

                    if(Convert.ToInt32(newAbsence.TO_DATE.Value.Subtract(newAbsence.SIN_DATE.Value).TotalHours) < 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','Complete Valid Date','Complete Valid Date');}window.onload=init;</script>");
                    }
                    int days = Convert.ToInt32(newAbsence.TO_DATE.Value.Subtract(newAbsence.SIN_DATE.Value).TotalDays)+1;
                    
                    newAbsence.CREATE_DATE = DateTime.Now;
                    newAbsence.UserId = UserId;
                    newAbsence.NUM_DAYS = days;
                    dbe.ABSENCEs.Add(newAbsence);
                    dbe.SaveChanges();
                }
                else
                {
                    var xx = ModelState;
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','Complete Information','Complete Information');}window.onload=init;</script>");
                }
                // TODO: Add insert logic here
                //ABSENCE cAbsence = new ABSENCE();
                //UpdateModel(cAbsence, collection);
                int ID_ABSE = Convert.ToInt32(newAbsence.ID_ABSE);
                dbe.Entry(newAbsence).State = EntityState.Detached;

                string code = Convert.ToString(dbe.ABSENCEs.Single(t => t.ID_ABSE == ID_ABSE).COD_ABSE);
                

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + code + "','');}window.onload=init;</script>");

            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Absence/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Absence/Edit/5

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
        // GET: /Absence/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Absence/Delete/5

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
