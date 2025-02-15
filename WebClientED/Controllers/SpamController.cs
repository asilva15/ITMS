using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClientED.Models;
using WebClientED.Plugins.Encription;

namespace WebClientED.Controllers
{
    public class SpamController : Controller
    {
        private AESRijndael seg = new AESRijndael();
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        // GET: Spam
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Control()
        {
            try
            {
                string idst = Convert.ToString(Request.Params["idst"]);
                //int id = Convert.ToInt32(seg.Decrypt(idst));
                int id = Convert.ToInt32(idst);
                //ViewData["id"] = System.Web.HttpUtility.UrlEncode(idst);

                int xxx = dbe.SPAM_CONTROL.Where(x => x.ID_SURV_TICK == id).Count();

                if (xxx > 0)
                {
                    SPAM_CONTROL obje = dbe.SPAM_CONTROL.First(x=>x.ID_SURV_TICK == id);
                    return View("ControlOK",obje);
                }

                SURVEY_TICKET st = dbe.SURVEY_TICKET.Single(x => x.ID_SURV_TICK == id);
                TICKET t = dbc.TICKETs.Single(x => x.ID_TICK == st.ID_TICK.Value);
                PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == t.ID_PERS_ENTI);

                SPAM_CONTROL SC = new SPAM_CONTROL();

                SC.ID_SURV_TICK = id;
                SC.ID_PERS_ENTI = pe.ID_PERS_ENTI;
                SC.EMA_SPAM_CONT = pe.EMA_PERS;
                SC.CREATED = DateTime.Now;

                //SPAM_CONTROL SC = new SPAM_CONTROL();

                return View(SC);
            }
            catch
            {
                return Content("Error");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Control(SPAM_CONTROL sc,int id = 0)
        {
            try{
                if( String.IsNullOrEmpty(sc.SUM_SPAM_CONT)){
                    return View(sc);
                }
                else
                {
                    dbe.SPAM_CONTROL.Add(sc);
                    dbe.SaveChanges();

                    return View("ControlOK",sc);
                }
            }
            catch{
                return View(sc);
            }
            
            
        }
    }
}