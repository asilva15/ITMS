using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class Site_EntityController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Site_Entity/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var result = (from a in dbe.SITE_ENTITY
                          select new
                          {
                              a.ID_SITE,
                              a.NAM_SITE,
                          }).OrderBy(x => x.NAM_SITE);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(SITE_ENTITY sit_ent)
        {
            int sw = 0;
            string NAM_SITE = Convert.ToString(Request.Form["NAM_SITE"]);

            if (NAM_SITE.Trim().Length == 0) { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            try
            {
                SITE_ENTITY se = new SITE_ENTITY();
                se.NAM_SITE = NAM_SITE;
                dbe.SITE_ENTITY.Add(se);
                dbe.SaveChanges();

                string id = Convert.ToString(se.ID_SITE);

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','Site|" + id + "');}window.onload=init;</script>");
            }
            catch (Exception)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
            }
        }

    }
}
