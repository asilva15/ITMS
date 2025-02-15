using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClientED.Models;
using System.Data.Entity;
using WebClientED.Plugins.Encription;
using System.Configuration;
using System.IO;

namespace WebClient.Controllers
{
    public class SurveyController : Controller
    {
        private EntityEntities dbe = new EntityEntities();
        private CoreEntities dbc = new CoreEntities();
        //private Encryption seg = new Encryption();
        private AESRijndael seg = new AESRijndael();
        private string IpServer = ConfigurationManager.AppSettings["IpServer"].ToString();
        //private AppLogEntities db = new AppLogEntities();
        int idCuenta = 0;
        
        // GET: Survey
        public ActionResult Index(int id = 0)
        {
            try
            {
                //String s = seg.Encrypt("2914");
                string ids = Convert.ToString(Request.Params["idst"].ToString());
                ViewData["id"] = System.Web.HttpUtility.UrlEncode(ids);
                //string xxx = System.Web.HttpUtility.UrlEncode(ids);
                //string idst = seg.Decrypt(ids);
                //int ID_SURV_TICK = Convert.ToInt32(idst);
                int ID_SURV_TICK = Convert.ToInt32(ids);

                var query = dbe.SURVEY_TICKET.Single(x => x.ID_SURV_TICK == ID_SURV_TICK);
                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == query.ID_TICK.Value);
                idCuenta = ticket.ID_ACCO.Value;
                string ruta = Server.MapPath("~/Images/");

                if (System.IO.File.Exists(ruta + "\\logo-s-"+Convert.ToString(ticket.ID_ACCO.Value)+".png"))
                {
                    ViewData["logo"] = "logo-s-" + Convert.ToString(ticket.ID_ACCO.Value) + ".png";
                }
                else
                {
                    ViewData["logo"] = "logo-s-4.png";
                }

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

            string ids = "0";
            if (!String.IsNullOrEmpty(idxs))
            {
                //ids = seg.Decrypt(idxs);
                ids = Convert.ToString(id);
            }

            id = Convert.ToInt32(idxs);

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
                                  cuenta = tick.ID_ACCO,
                                  summary = tick.SUM_TICK,
                                  id_xt= System.Web.HttpUtility.UrlEncode(seg.Encrypt(tick.ID_TICK.ToString())),
                              });

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Question(int id = 0,int id1 = 0)
        {
            //string ids = seg.Decrypt(Convert.ToString(Request.Params["idst"]));
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
            int c = result.Count();
            var q = result.ToList();
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
                        //Si la cuenta es Golfieds
                        var query = dbe.SURVEY_TICKET.Single(x => x.ID_SURV_TICK == id);
                        int idCuenta = Convert.ToInt32(dbc.TICKETs.Single(x => x.ID_TICK == query.ID_TICK.Value).ID_ACCO);
                        if (idCuenta == 1)
                        {
                            string puntos = Convert.ToString(Request.Params["puntos"].ToString());
                            if(puntos == "")
                                return RedirectToAction("Index", new { idst = ids });
                            else
                                qte.VAL_QUES_TICK = puntos;
                        }
                        else
                        {
                            qte.VAL_QUES_TICK = val;
                        }
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

        public ActionResult Rpta2(int id = 0)
        {
            string idxs = Convert.ToString(Request.Params["idst"].ToString());
            string valor = Convert.ToString(Request.Params["valor"].ToString());
            //idxs = idxs.Replace(" ", "+");

            id = Convert.ToInt32(idxs);

            var st = dbe.SURVEY_TICKET.Single(x => x.ID_SURV_TICK == id);
            var qt = dbe.QUESTION_TICKET.Single(x => x.ID_SURV_TICK == id);
            if (st.ID_SURV_STAT.Value == 3)
            {
                ViewData["id"] = "ERROR";
                return View();
            }
            else
            {
                st.ID_SURV_STAT = 3;
                dbe.SURVEY_TICKET.Attach(st);
                dbe.Entry(st).State = EntityState.Modified;
                dbe.SaveChanges();

                qt.VAL_QUES_TICK = valor;
                dbe.QUESTION_TICKET.Attach(qt);
                dbe.Entry(qt).State = EntityState.Modified;
                dbe.SaveChanges();
                ViewData["id"] = "OK";
                return View();
            }
        }

        public ActionResult Rptas(int id = 0)
        {
            string idxs = Convert.ToString(Request.Params["idst"].ToString());
            string valor = Convert.ToString(Request.Params["valor"].ToString());
            //idxs = idxs.Replace(" ", "+");

            id = Convert.ToInt32(idxs);

            var st = dbe.SURVEY_TICKET.Single(x => x.ID_SURV_TICK == id);
            var qt = dbe.QUESTION_TICKET.Single(x => x.ID_SURV_TICK == id);
            var ticket = dbc.TICKETs.Single(x => x.ID_TICK == st.ID_TICK.Value);
            string ruta = Server.MapPath("~/Images/");

            if (System.IO.File.Exists(ruta + "\\logo-s-" + Convert.ToString(ticket.ID_ACCO.Value) + ".png"))
            {
                ViewData["logo"] = "logo-s-" + Convert.ToString(ticket.ID_ACCO.Value) + ".png";
            }
            else
            {
                ViewData["logo"] = "logo-s-4.png";
            }

            if (st.ID_SURV_STAT.Value == 3)
            {
                ViewData["id"] = "ERROR";
                return View();
            }
            else
            {
                st.ID_SURV_STAT = 3;
                dbe.SURVEY_TICKET.Attach(st);
                dbe.Entry(st).State = EntityState.Modified;
                dbe.SaveChanges();

                qt.VAL_QUES_TICK = valor;
                dbe.QUESTION_TICKET.Attach(qt);
                dbe.Entry(qt).State = EntityState.Modified;
                dbe.SaveChanges();
                ViewData["id"] = "OK";
                return View();
            }
        }

    }
}