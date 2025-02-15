using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Data.Entity;
using ERPElectrodata.Object.Talent;
using System.Globalization;
using System.Threading;

namespace ERPElectrodata.Controllers
{
    public class VacationController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        AppLogEntities dba = new AppLogEntities();
        AssistanceEntities dbas = new AssistanceEntities();
        CoreEntities dbc = new CoreEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        private int year = DateTime.Now.Year;

        [Authorize]
        public ActionResult IndexRequestVacation()
        {
            return View();
        }

        public ActionResult ListRequest()
        {
            textInfo = cultureInfo.TextInfo;

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = dbe.VACATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.ID_PERS_ENTI_APPR_1 == ID_PERS_ENTI
                || x.ID_PERS_ENTI_APPR_2 == ID_PERS_ENTI);

            var result = (from x in query.ToList()
                          join pe in dbe.PERSON_ENTITY on x.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          join sv in dbe.STATUS_VACATION on x.ID_VACA_STAT equals sv.ID_VACA_STAT
                          select new { 
                            ID_PERS_ENTI = x.ID_PERS_ENTI,
                            DAT_STAR = String.Format("{0:d}", x.DAT_STAR),
                            DAT_END = String.Format("{0:d}",x.DAT_END),
                            NAME = textInfo.ToTitleCase(ce.FIR_NAME.ToLower())+" "+textInfo.ToTitleCase(ce.LAS_NAME.ToLower()),
                            NAM_VACA_STAT = textInfo.ToTitleCase(sv.NAM_VACA_STAT.ToLower()),
                            COL_VACA_STAT = sv.COL_VACA_STAT,
                          });

            return Json(new { Data = result,Count = query.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestVacation()
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                PROVISION_VACATION provVacation = dbe.PROVISION_VACATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                .Single(x => x.ID_ACCO_YEAR == year);

                VACATION vacation = new VACATION();
                vacation.ID_PERS_ENTI = ID_PERS_ENTI;
                vacation.ID_PROV_VACA = provVacation.ID_PROV_VACA;

                return View(vacation);
            }
            catch
            {
                return Content("Error, PLease contact your Administrator");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestVacation(VACATION vacation,int id = 0)
        {
            try
            {
                int sw = 0;
                string msg = "Vacation request recorded correctly";
                DateTime DAT_STAR, DAT_END;

                if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_STAR"]), out DAT_STAR) == false)
                {
                    sw = sw + 1;
                    msg = "Please enter Date Start";
                }
                if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_END"]), out DAT_END) == false)
                {
                    sw = sw + 1;
                    msg = "Please enter Date End";
                }

                TimeSpan dif = DAT_END - DAT_STAR;

                if (dif.Days < 0)
                {
                    sw = sw + 1;
                    msg = "The end date must be greater than start date";
                }

                if (sw >= 1)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneRequestVacation) top.uploadDoneRequestVacation('ERROR','" + msg + "');}window.onload=init;</script>");
                }
                else
                {
                    var chart = dbe.Obtiene_Nombre_UEN_AREA_CARGO(vacation.ID_PERS_ENTI.Value).First();

                    Organization org = new Organization();
                    var orgx = org.chart_staff(vacation.ID_PERS_ENTI.Value);

                    vacation.NUM_DAYS = dif.Days + 1;
                    vacation.ID_VACA_STAT = 1;
                    vacation.UserId = Convert.ToInt32(Session["UserId"]);
                    vacation.DAT_REGI = DateTime.Now;
                    vacation.ID_PERS_ENTI_APPR_1 = orgx.ID_BOSS;
                    vacation.ID_PERS_ENTI_APPR_2 = orgx.ID_BOSS_UEN;
                    dbe.VACATION.Add(vacation);
                    dbe.SaveChanges();
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneRequestVacation) top.uploadDoneRequestVacation('OK','" + msg + "');}window.onload=init;</script>");
                }
            }
            catch
            {

            }
            

            return Content("");
        }
        // GET: /Vacation/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            int count = 0;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var qVac = dbe.VACATION.Where(x=>x.ID_VACA != 5).OrderByDescending(x=>x.ID_VACA);
            count = qVac.Count();

            var qVaca = (from a in qVac.Skip(skip).Take(take).ToList()
                         join b in dbe.STATUS_VACATION on a.ID_VACA_STAT equals b.ID_VACA_STAT
                         join c in dbe.PERSON_ENTITY on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in dbe.CLASS_ENTITY on c.ID_ENTI2 equals d.ID_ENTI
                         join e in dbe.PERSON_CONTRACT.Where(x=>x.LAS_CONT==true) on a.ID_PERS_ENTI equals e.ID_PERS_ENTI
                         join f in dbe.CHARTs on e.ID_CHAR equals f.ID_CHAR
                         join g in dbe.NAME_CHART on f.ID_NAM_CHAR equals g.ID_NAM_CHAR
                         join h in dbe.CLASS_ENTITY on a.UserId equals h.UserId
                         select new { 
                            a.ID_VACA,
                            a.ID_PERS_ENTI,
                            a.COD_VACA,
                            c.ID_FOTO,
                            a.ID_VACA_STAT,
                            NAM_VACA_STAT = b.NAM_VACA_STAT.ToLower(),
                            NAM_ATTA = (a.NAM_ATTA==null ? "" : a.NAM_ATTA),
                            URL_ATTA = (a.NAM_ATTA == null ? "" : "/Adjuntos/Talent/Vacation/" + a.NAM_ATTA + "_" + Convert.ToString(a.ID_VACA) + ".pdf"),
                            Employee = (d.FIR_NAME + " " + d.LAS_NAME).ToLower(),
                            JobTitle = g.NAM_CHAR.ToLower(),
                            EMA_ELEC = (c.EMA_ELEC == null ? "-" : c.EMA_ELEC.ToLower()),
                            c.CEL_PERS,
                            a.NUM_DAYS,
                            START_DATE = a.DAT_STAR,
                            DAT_STAR = (a.DAT_STAR == null ? "" : string.Format("{0:dddd dd MMMM yyyy}",a.DAT_STAR)),
                            DAT_END = (a.DAT_END == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_END)),
                            DAT_RETU = (a.DAT_RETU == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_RETU)),
                            a.SUMMARY,
                            User = h.FIR_NAME + " " + h.LAS_NAME,
                            DAT_REGI = (a.DAT_REGI == null ? "" : string.Format("{0:G}", a.DAT_REGI)),
                         }).OrderByDescending(x => x.START_DATE);

            return Json(new { Data = qVaca, Count = count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewNewVacation() {
            return View();
        }

        public ActionResult NewVacation(IEnumerable<HttpPostedFileBase> filesVaca, VACATION vac)
        {
            int sw = 0, ID_PERS_ENTI;
            DateTime DAT_STAR, DAT_END, DAT_RETU;
            string msn = "";
            string SUMMARY = Convert.ToString(Request.Form["SUMMARY"]);

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_ENTI"]), out ID_PERS_ENTI) == false) { sw = 1; msn = ResourceLanguaje.Resource.VacationMsn01; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_STAR"]), out DAT_STAR) == false) { sw = 1; msn = ResourceLanguaje.Resource.VacationMsn02; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_END"]), out DAT_END) == false) { sw = 1; msn = ResourceLanguaje.Resource.VacationMsn03; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_RETU"]), out DAT_RETU) == false) { sw = 1; msn = ResourceLanguaje.Resource.VacationMsn04; }
            else if (SUMMARY.Trim().Length == 0) { sw = 1; msn = ResourceLanguaje.Resource.VacationMsn05; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneVaca) top.uploadDoneVaca('ERROR','" + msn + "');}window.onload=init;</script>");
            }
            else
            {
                try
                {
                    int dias = Convert.ToInt32(Request.Form["NumDaysHF"]);
                    vac.NUM_DAYS = dias;
                    vac.ID_VACA_STAT = 2;
                    vac.UserId = Convert.ToInt32(Session["UserId"]);
                    vac.DAT_REGI = DateTime.Now;
                    dbe.VACATION.Add(vac);
                    dbe.SaveChanges();

                    //--------------------Ingresa Registros en Vacation --------------------------

                    try
                    {

                        VACATION_AND_ABSENCE va = new VACATION_AND_ABSENCE();
                        DateTime ds = Convert.ToDateTime(vac.DAT_STAR);
                        int cont = 0;

                        var queryvacabs = (from v in dbe.VACATION.Where(x => x.ID_VACA == vac.ID_VACA && x.ID_VACA_STAT == 2)
                                           join pe in dbe.PERSON_ENTITY on v.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                                           join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                           select new
                                           {
                                               v.ID_VACA,
                                               ce.NUM_TYPE_DI,
                                               v.DAT_STAR,
                                               v.ID_VACA_STAT,
                                               pe.ID_HORA,
                                               v.NUM_DAYS,
                                           }).First();


                        for (int i = 1; i <= queryvacabs.NUM_DAYS; i++)
                        {
                            va.ID_VACA = vac.ID_VACA;
                            va.DAT_VACA_ABSE = ds;
                            va.ID_STAT_VACA = queryvacabs.ID_VACA_STAT;
                            va.NUM_DAY = i;
                            va.ID_HORA = queryvacabs.ID_HORA;
                            va.ID_TYPE_REGI = 6;
                            va.NUM_TYPE_DI = queryvacabs.NUM_TYPE_DI;
                            va.CREATED = DateTime.Now;

                            dbas.VACATION_AND_ABSENCE.Add(va);
                            dbas.SaveChanges();

                            ds = ds.AddDays(1);
                            cont = cont + 1;
                        }

                    }
                    catch (Exception ex)
                    {
                        var exc = new EXCEPTION();
                        exc.DAT_EXCE = DateTime.Now;
                        exc.MESSAGE = ex.InnerException.Message;
                        exc.DES_EXCE = "Creando Registros - TBL:Vacation_and_Absence";
                        dba.EXCEPTIONs.Add(exc);
                        dba.SaveChanges();

                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDoneVaca) top.uploadDoneVaca('ERROR','" + ResourceLanguaje.Resource.MsnErrorITMS + "');}window.onload=init;</script>");
                    }

                    //---------------------------------------------------------------------------------------------------------

                    if (filesVaca != null)
                    {
                        foreach (var file in filesVaca)
                        {
                            string NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            string EXT_ATTA = Path.GetExtension(file.FileName);

                            vac.NAM_ATTA = NAM_ATTA;
                            dbe.VACATION.Attach(vac);
                            dbe.Entry(vac).State = EntityState.Modified;
                            dbe.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Vacation/" + NAM_ATTA + "_" + Convert.ToString(vac.ID_VACA) + EXT_ATTA));
                        }
                    }
                }
                catch (Exception e)
                {
                    var exc = new EXCEPTION();
                    exc.DAT_EXCE = DateTime.Now;
                    exc.MESSAGE = e.InnerException.Message;
                    exc.DES_EXCE = "Registrando Vacaciones - TBL:Vacation";
                    dba.EXCEPTIONs.Add(exc);
                    dba.SaveChanges();

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneVaca) top.uploadDoneVaca('ERROR','" + ResourceLanguaje.Resource.MsnErrorITMS + "');}window.onload=init;</script>");
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneVaca) top.uploadDoneVaca('OK','');}window.onload=init;</script>");
            }
        }

        public ActionResult viewFindVacation() {
            //return Content("<br /><br /><b>Esta opción aun se encuentra en desarrollo.</b><br />Find Vacation estará disponible en las próximas 24 horas.<br />Gracias por su compresión.<br /><br />");
            return View();
        }

        public ActionResult FindResult() {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            string SUMMARY = Convert.ToString(Request.Params["SUMMARY"]);
            
            DateTime DAT_STAR, DAT_END, DAT_RETU;
            int ID_PERS_ENTI, ID_VACA_STAT, NUM_DAYS, Count;

            var vaca = dbe.VACATION.Where(x => x.ID_VACA_STAT != 0);

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == true)
            {
                vaca = vaca.Where(x=>x.ID_PERS_ENTI == ID_PERS_ENTI);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["NUM_DAYS"]), out NUM_DAYS) == true)
            {
                vaca = vaca.Where(x => x.NUM_DAYS == NUM_DAYS);
            }
            if (DateTime.TryParse(Convert.ToString(Request.Params["DAT_STAR"]), out DAT_STAR))
            {
                vaca = vaca.Where(x => x.DAT_STAR >= DAT_STAR);
            }
            if (DateTime.TryParse(Convert.ToString(Request.Params["DAT_END"]), out DAT_END))
            {
                DAT_END = ((DAT_END.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                vaca = vaca.Where(x => x.DAT_STAR <= DAT_END);
            }
            if (DateTime.TryParse(Convert.ToString(Request.Params["DAT_RETU"]), out DAT_RETU))
            {
                vaca = vaca.Where(x => x.DAT_RETU == DAT_RETU);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_VACA_STAT"]), out ID_VACA_STAT) == true)
            {
                vaca = vaca.Where(x => x.ID_VACA_STAT == ID_VACA_STAT);
            }
            if (!String.IsNullOrEmpty(SUMMARY))
            {
                vaca = vaca.Where(x => x.SUMMARY.ToUpper().Contains(SUMMARY.ToUpper()));
            }

            Count = vaca.Count();
            vaca = vaca.OrderByDescending(x => x.ID_VACA).Skip(skip).Take(take);

            var qVaca = (from a in vaca.ToList()
                         join b in dbe.STATUS_VACATION on a.ID_VACA_STAT equals b.ID_VACA_STAT
                         join c in dbe.PERSON_ENTITY on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in dbe.CLASS_ENTITY on c.ID_ENTI2 equals d.ID_ENTI
                         join e in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true) on a.ID_PERS_ENTI equals e.ID_PERS_ENTI
                         join f in dbe.CHARTs on e.ID_CHAR equals f.ID_CHAR
                         join g in dbe.NAME_CHART on f.ID_NAM_CHAR equals g.ID_NAM_CHAR
                         join h in dbe.CLASS_ENTITY on a.UserId equals h.UserId
                         select new
                         {
                             a.ID_VACA,
                             a.ID_PERS_ENTI,
                             a.COD_VACA,
                             c.ID_FOTO,
                             a.ID_VACA_STAT,
                             NAM_VACA_STAT = b.NAM_VACA_STAT.ToLower(),
                             NAM_ATTA = (a.NAM_ATTA == null ? "" : a.NAM_ATTA),
                             URL_ATTA = (a.NAM_ATTA == null ? "" : "/Adjuntos/Talent/Vacation/" + a.NAM_ATTA + "_" + Convert.ToString(a.ID_VACA) + ".pdf"),
                             Employee = (d.FIR_NAME + " " + d.LAS_NAME).ToLower(),
                             JobTitle = g.NAM_CHAR.ToLower(),
                             EMA_ELEC = (c.EMA_ELEC == null ? "-" : c.EMA_ELEC.ToLower()),
                             CEL_PERS = (c.CEL_PERS==null ? "-" : c.CEL_PERS),
                             a.NUM_DAYS,
                             START_DATE = a.DAT_STAR,
                             DAT_STAR = (a.DAT_STAR == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_STAR)),
                             DAT_END = (a.DAT_END == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_END)),
                             DAT_RETU = (a.DAT_RETU == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_RETU)),
                             a.SUMMARY,
                             User = h.FIR_NAME + " " + h.LAS_NAME,
                             DAT_REGI = (a.DAT_REGI == null ? "" : string.Format("{0:G}", a.DAT_REGI)),
                         }).OrderByDescending(x => x.ID_VACA);

            return Json(new { Data = qVaca, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByID_PERS_ENTI(int id = 0)
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            int Count;
            var vaca = dbe.VACATION.Where(x => x.ID_PERS_ENTI == id).OrderByDescending(x=>x.ID_VACA);
            Count = vaca.Count();

            var qVaca = (from a in vaca.Skip(skip).Take(take).ToList()
                         join b in dbe.STATUS_VACATION on a.ID_VACA_STAT equals b.ID_VACA_STAT
                         join h in dbe.CLASS_ENTITY on a.UserId equals h.UserId
                         select new
                         {
                             a.ID_VACA,
                             a.COD_VACA,
                             a.ID_PERS_ENTI,
                             a.ID_VACA_STAT,
                             NAM_VACA_STAT = b.NAM_VACA_STAT.ToLower(),
                             NAM_ATTA = (a.NAM_ATTA == null ? "" : a.NAM_ATTA),
                             URL_ATTA = (a.NAM_ATTA == null ? "" : "/Adjuntos/Talent/Vacation/" + a.NAM_ATTA + "_" + Convert.ToString(a.ID_VACA) + ".pdf"),
                             a.NUM_DAYS,
                             START_DATE = a.DAT_STAR,
                             DAT_STAR = (a.DAT_STAR == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_STAR)),
                             DAT_END = (a.DAT_END == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_END)),
                             DAT_RETU = (a.DAT_RETU == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_RETU)),
                             a.SUMMARY,
                             User = h.FIR_NAME + " " + h.LAS_NAME,
                             DAT_REGI = (a.DAT_REGI == null ? "" : string.Format("{0:G}", a.DAT_REGI)),
                         }).OrderByDescending(x => x.START_DATE);

            return Json(new { Data = qVaca, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        public string Delete(int id = 0)
        {
            try
            {
                VACATION vac = dbe.VACATION.Find(id);
                try
                {
                    vac.ID_VACA_STAT = 5;
                    vac.ID_PERS_ENTI_DELE = Convert.ToInt32(Session["UserId"]);
                    vac.DAT_REGI_DELE = DateTime.Now;
                    dbe.VACATION.Attach(vac);
                    dbe.Entry(vac).State = EntityState.Modified;
                    dbe.SaveChanges();
                }
                catch (Exception e)
                {
                    var exc = new EXCEPTION();
                    exc.MESSAGE = e.InnerException.Message;
                    exc.DAT_EXCE = DateTime.Now;
                    exc.UserId = Convert.ToInt32(Session["UserId"]);
                    exc.DES_EXCE = "Eliminando Vacaciones: ID_VAVA = " + id.ToString();
                    dba.EXCEPTIONs.Add(exc);
                    dba.SaveChanges();
                }
                return "OK";
            }
            catch (Exception)
            {
                return "ERROR";
            }

        }

        public ActionResult ListStatus() {
            var query = (from a in dbe.STATUS_VACATION
                         select new { 
                            a.ID_VACA_STAT,
                            a.NAM_VACA_STAT,
                         }).OrderBy(x=>x.NAM_VACA_STAT);
            return Json(new { Data = query, Count = query.Count() },JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListGraf() {

            var qVac = (from a in dbe.VACATION
                        select new { 
                            a.ID_PERS_ENTI,
                        }).Distinct();

            var qVaca = (from a in qVac.ToList()
                         join b in dbe.PERSON_ENTITY on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                         join c in dbe.CLASS_ENTITY on b.ID_ENTI2 equals c.ID_ENTI
                        select new
                        {
                            a.ID_PERS_ENTI,
                            name = c.FIR_NAME + " " + c.LAS_NAME,
                            desc = "Job Title",
                            values = ListVacationByID_PERS_ENTI(Convert.ToInt32(a.ID_PERS_ENTI))
                        }).Distinct();

            return Json(new { Data = qVaca, Count = qVaca.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListVacationByID_PERS_ENTI(int id = 0)
        {
            var qVac = (from a in dbe.VACATION.Where(x=>x.ID_PERS_ENTI == id)
                        select new
                        {
                            a.DAT_STAR,
                            a.DAT_END,
                            desc = a.SUMMARY,
                            customClass = "ganttRed"
                        }).OrderBy(x=>x.DAT_STAR);

            return Json(new { Data = qVac, Count = qVac.Count() }, JsonRequestBehavior.AllowGet);
        }

        public string Validate()
        {
            int Banner = Convert.ToInt32(Session["BANNER"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //string bannerUrl = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 38 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA==true).VAL_ACCO_PARA;
            //return bannerUrl;

            if (Banner == 1)
            {
                Session["BANNER"] = 0;
                try
                {
                    string bannerUrl = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 38 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA;

                    return bannerUrl;
                }
                catch
                {
                    return "NO";
                }
            }
            else
            {
                return "NO";
            }

        }

        public string ValidateX()
        {
            int Banner = Convert.ToInt32(Session["BANNER"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //string bannerUrl = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 38 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA==true).VAL_ACCO_PARA;
            //return bannerUrl;

            string bannerUrl = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 38 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA;

            return bannerUrl;

        }

        public ActionResult Summary()
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                var query = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                    .Where(x => x.VIG_CONT == true);

                var result = (from c in query.ToList()
                              join pl in dbe.WORK_PERIOD on c.ID_WORK_PERI equals pl.ID_WORK_PERI
                              select new
                              {
                                  STAR_DATE = String.Format("{0:d}", pl.STAR_DATE),
                                  TOTAL_YEAR = pl.TOTAL_YEAR == 0 ? "": Convert.ToString(pl.TOTAL_YEAR)+ (pl.TOTAL_YEAR==1 ? " Year":" Years"),
                                  TOTAL_MONTH = pl.TOTAL_MONTH == 0 ? "" : Convert.ToString(pl.TOTAL_MONTH) + (pl.TOTAL_MONTH == 1 ? " Month" : " Months"),
                                  TOTAL_DAY = pl.TOTAL_DAY == 0 ? "" : Convert.ToString(pl.TOTAL_DAY) + (pl.TOTAL_DAY == 1 ? " Day" : " Days")
                              }).First();

                ViewBag.STAR_DATE = result.STAR_DATE;
                ViewBag.TIMEATCOMPANY = result.TOTAL_YEAR+" "+result.TOTAL_MONTH+" "+result.TOTAL_DAY;

            }
            catch
            {

            }

            return View();

        }

        public ActionResult SummaryData()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = (from pv in dbe.PROVISION_VACATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                         select new
                         {
                             pv.ID_PERS_ENTI,
                             pv.ID_ACCO_YEAR,
                             PENDING = pv.TOTAL_TODAY - pv.USED,
                             TOTAL_TODAY = pv.TOTAL_TODAY,
                             USED = pv.USED
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
