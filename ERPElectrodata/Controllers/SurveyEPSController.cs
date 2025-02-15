using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class SurveyEPSController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /SurveyEPS/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            try{
            
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            DETAIL_SURVEY_EPS dseps = dbe.DETAIL_SURVEY_EPS.Single(x=>x.ID_PERS_ENTI == ID_PERS_ENTI);

            int ID_CIA_EPS = 0;
            string ID_CIA_EPSs = Convert.ToString(dseps.ID_CIA_EPS);

            try
            {
                if (dseps.ID_CIA_EPS != null)
                {
                    var ciaEps = dbe.CIA_EPS.Single(x => x.ID_CIA_EPS == dseps.ID_CIA_EPS.Value);

                    return Content("Ud. Ha seleccionado: "+ciaEps.NAM_CIA_EPS);
                }
            }
            catch
            {

            }

            return View(dseps);
            }
            catch{
                return Content("Contacte con su Administrador");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DETAIL_SURVEY_EPS dseps, int id = 0)
        {
            try
            {
                int ID_DETA_SURV_EPS = Convert.ToInt32(dseps.ID_DETA_SURV_EPS);

                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                var objEdit = dbe.DETAIL_SURVEY_EPS
                    .Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                    .Single(x => x.ID_DETA_SURV_EPS == ID_DETA_SURV_EPS);

                objEdit.ID_CIA_EPS = dseps.ID_CIA_EPS;
                objEdit.CREATED = DateTime.Now;
                dbe.Entry(objEdit).State = EntityState.Modified;
                dbe.SaveChanges();

                var ciaEps = dbe.CIA_EPS.Single(x => x.ID_CIA_EPS == objEdit.ID_CIA_EPS.Value);

                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('OK','"+ciaEps.NAM_CIA_EPS+"');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            
        }

    }
}
