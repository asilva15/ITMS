using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.Models;
using WebClient.Plugins.Encription;

namespace WebClient.Controllers
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
                string idst = Convert.ToString(Request.Params["idts"]);
                int id = Convert.ToInt32(seg.Decrypt(idst));
                ViewData["id"] = System.Web.HttpUtility.UrlEncode(idst);

                SURVEY_TICKET st = dbe.SURVEY_TICKET.Single(x=>x.ID_SURV_TICK == id);
                TICKET t = dbc.TICKETs.Single(x=>x.ID_TICK == st.ID_TICK.Value);
                PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x=>x.ID_PERS_ENTI == t.ID_PERS_ENTI);

                SPAM_CONTROL SC = new SPAM_CONTROL();

                SC.ID_SURV_TICK = id;
                SC.ID_PERS_ENTI = pe.ID_PERS_ENTI;
                SC.EMA_SPAM_CONT = pe.EMA_PERS;


                return View(SC);
            }
            catch
            {
                return View();
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Control(int id=0)
        {
            return View();
        }
    }
}