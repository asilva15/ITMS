using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.Models;
using System.Data.Entity;
using WebClient.Plugins.Encription;
using System.Configuration;

namespace WebClient.Controllers
{
    public class SurveyController : Controller
    {
        private EntityEntities dbe = new EntityEntities();
        private CoreEntities dbc = new CoreEntities();
        //private Encryption seg = new Encryption();
        private AESRijndael seg = new AESRijndael();
        private string IpServer = ConfigurationManager.AppSettings["IpServer"].ToString();
        // GET: Survey
        public ActionResult Index(int id = 0)
        {
            try
            {
                string ids = Convert.ToString(Request.Params["idst"].ToString());
                ViewData["id"] = System.Web.HttpUtility.UrlEncode(ids);
                return View();
            }
            catch
            {
                return View();
            }
            
        }

        //id = {ID_SURV_TICK}
        public ActionResult Grupo(int id = 0)
        {
            string idxs = Convert.ToString(Request.Params["idst"].ToString());
            //idxs = idxs.Replace(" ", "+");

            //string ids = seg.Decrypt(idxs);
            string ids = idxs;

            id = Convert.ToInt32(ids);

            var st = dbe.SURVEY_TICKET.Single(x=>x.ID_SURV_TICK == id);

            if (st.ID_SURV_STAT.Value == 3)
            {

                var result = (from x in dbe.SURVEY_TICKET.Where(x => x.ID_SURV_TICK == id)
                              select new { 
                                work = 0,
                                msg = "Gracias por tu Respuesta",
                              });

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = dbe.QUESTION_GROUP.Where(x => x.ID_SURVEY == st.ID_SURV.Value);
                var tick = dbc.TICKETs.Single(x => x.ID_TICK == st.ID_TICK.Value);

                var result = (from x in query.ToList()
                              select new
                              {
                                  nombre = x.NAM_QUES_GROU,
                                  id = x.ID_QUES_GROU,
                                  idst = id,
                                  work=1,
                                  ticket = tick.COD_TICK,
                                  id_xt= System.Web.HttpUtility.UrlEncode(seg.Encrypt(tick.ID_TICK.ToString())),
                              });

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }

            
        }

        public ActionResult Question(int id = 0,int id1 = 0)
        {
        //    string ids = seg.Decrypt(Convert.ToString(Request.Params["idst"]));
            string ids = Convert.ToString(Request.Params["idst"]);

            id1 = Convert.ToInt32(ids);

            var query = dbe.QUESTIONs.Where(x => x.ID_QUES_GROU == id);

            var result = (from x in query.ToList()
                          join qt in dbe.QUESTION_TICKET.Where(x=>x.ID_SURV_TICK == id1) on x.ID_QUES equals qt.ID_QUES
                          select new
                          {
                              nombre = x.NAM_QUES,
                              id = x.ID_QUES,
                              Type = x.ID_QUES_TYPE,
                              idqt = qt.ID_QUES_TICK
                          });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id)
        {
            string ids = System.Web.HttpUtility.UrlDecode(Convert.ToString(Request.Params["idst"].ToString()));
            try
            {
                
                //id = Convert.ToInt32(seg.Decrypt(ids));
                id = Convert.ToInt32(ids);

                foreach (string cadena in Request.Form)
                {
                    int val_id;

                    if (Int32.TryParse(cadena, out val_id))
                    {
                        string val = Convert.ToString(Request.Form[cadena].ToString());
                        QUESTION_TICKET qte = dbe.QUESTION_TICKET.Single(x => x.ID_QUES_TICK == val_id && x.ID_SURV_TICK == id);

                        if (String.IsNullOrEmpty(val))
                        {
                            return RedirectToAction("Index", new { idst = ids });
                        }
                        else
                        {
                            int ID_QUES_TYPE = dbe.QUESTIONs.Single(x => x.ID_QUES == qte.ID_QUES.Value).ID_QUES_TYPE.Value;
                            switch (ID_QUES_TYPE)
                            {
                                case 1:
                                    if (Convert.ToInt32(val) == 0)
                                        return RedirectToAction("Index", new { idst = ids });
                                    break;
                                default:
                                    break;
                            }

                        }

                        dbe.Entry(qte).State = EntityState.Modified;
                        qte.VAL_QUES_TICK = val;
                        qte.MODIFIED = DateTime.Now;
                        dbe.SaveChanges();
                    }
                }

                //Inserta Actividad
                SURVEY_TICKET_ACTIVITY sta = new SURVEY_TICKET_ACTIVITY();
                sta.ID_SURV_TICK = id;
                sta.ID_SURV_STAT = 3;
                sta.CREATED = DateTime.Now;
                dbe.SURVEY_TICKET_ACTIVITY.Add(sta);
                dbe.SaveChanges();

                SURVEY_TICKET st = dbe.SURVEY_TICKET.Single(x=>x.ID_SURV_TICK == id);
                dbe.Entry(st).State = EntityState.Modified;
                st.ID_SURV_STAT = 3;
                dbe.SaveChanges();

                return RedirectToAction("Index", new { idst = ids });
            }
            catch
            {
                return RedirectToAction("Index", new { idst = ids });
            }
        }
    }
}