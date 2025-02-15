using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{    
    public class DetailsVacationController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        AppLogEntities dba = new AppLogEntities();
        AssistanceEntities dbas = new AssistanceEntities();

        // GET: /DetailsVacation/
        public ActionResult Index(int id=0)
        {
            var vac = dbe.VACATION.Single(x => x.ID_VACA == id);
            ViewBag.ID_VACA = id;
            ViewBag.COD_VACA = vac.COD_VACA;
            return View();
        }

        public ActionResult ListByIdVaca(int id = 0) {

            var qVaca = (from a in dbe.VACATION.Where(x=>x.ID_VACA == id).ToList()
                         join b in dbe.STATUS_VACATION on a.ID_VACA_STAT equals b.ID_VACA_STAT
                         join c in dbe.CLASS_ENTITY on a.UserId equals c.UserId
                         join d in dbe.PERSON_ENTITY on a.ID_PERS_ENTI equals d.ID_PERS_ENTI
                         join e in dbe.CLASS_ENTITY on d.ID_ENTI2 equals e.ID_ENTI
                         join f in dbe.PERSON_ENTITY on c.ID_ENTI equals f.ID_ENTI2
                         select new
                         {
                             a.ID_VACA,
                             a.COD_VACA,
                             a.ID_PERS_ENTI,
                             a.ID_VACA_STAT,
                             NAM_ATTA = AttaDetaVaca(Convert.ToInt32(a.ID_VACA),(a.NAM_ATTA==null ? "" : a.NAM_ATTA)),
                             a.NUM_DAYS,
                             NAM_VACA_STAT = b.NAM_VACA_STAT.ToLower(),
                             START_DATE = a.DAT_STAR,
                             DAT_STAR = (a.DAT_STAR == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_STAR)),
                             DAT_END = (a.DAT_END == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_END)),
                             DAT_RETU = (a.DAT_RETU == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", a.DAT_RETU)),
                             a.SUMMARY,
                             User = (c.FIR_NAME + " " + c.LAS_NAME).ToLower(),
                             ID_FOTO = (f.ID_FOTO==null ? 0 : f.ID_FOTO),
                             DAT_REGI = (a.DAT_REGI == null ? "" : string.Format("{0:G}", a.DAT_REGI)),
                             Employee = (e.FIR_NAME + " " + e.LAS_NAME).ToLower(),
                             Email = (d.EMA_ELEC == null ? "" : d.EMA_ELEC).ToLower(),
                             Cell = (d.CEL_PERS == null ? "-" : (d.CEL_PERS.Trim().Length == 0 ? "-" : d.CEL_PERS.Trim())),
                         }).OrderByDescending(x => x.START_DATE);

            var qComm = (from a in dbe.DETAILS_VACATION.Where(x => x.ID_VACA == id).ToList()
                         join b in dbe.TYPE_DETAILS_VACATION on a.ID_TYPE_DETA_VACA equals b.ID_TYPE_DETA_VACA
                         join c in dbe.CLASS_ENTITY on a.UserId equals c.UserId
                         join d in dbe.PERSON_ENTITY on c.ID_ENTI equals d.ID_ENTI2
                         select new
                         {
                             a.ID_DETA_VACA,
                             a.ID_VACA,
                             a.ID_TYPE_DETA_VACA,
                             a.UserId,
                             a.COM_DETA_VACA,
                             DAT_REGI = (a.DAT_REGI==null?"":string.Format("{0:G}",a.DAT_REGI)),
                             NAM_TYPE_DETA_VACA = (b.NAM_TYPE_DETA_VACA==null?"":b.NAM_TYPE_DETA_VACA.ToLower()),
                             User = (c.FIR_NAME + " " + c.LAS_NAME).ToLower(),
                             ATTA = AttaCommentVaca(Convert.ToInt32(a.ID_DETA_VACA)),
                             d.ID_FOTO,
                             REGITER = a.DAT_REGI,
                         }).OrderByDescending(x=>x.REGITER);

            return Json(new { Data = qVaca, Count = qVaca.Count(), Details = qComm }, JsonRequestBehavior.AllowGet);        
        }

        public string AttaDetaVaca(int id = 0, string nam = "")
        {
            if (nam.Length > 0)
            {
                nam = "<li><a href='/Adjuntos/Talent/Vacation/" + nam + "_" + id.ToString() + ".pdf' TARGET='_BLANK'>" + nam + "</a></li>";
                return nam;
            }
            else {
                return String.Empty;            
            }
        }

        public string AttaCommentVaca(int id)
        {
            string Adjun = "";
            try
            {
                var query = dbe.ATTACHED_VACATION.Where(a => a.ID_DETA_VACA == id);
                foreach (ATTACHED_VACATION subqu in query)
                {
                    Adjun = Adjun + "<li><a href='/Adjuntos/Talent/Vacation/Details/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA_VACA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_ATTA + "</a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files)
        {
            int sw = 0, ID_TYPE_DETA_VACA;
            var detv = new DETAILS_VACATION();
            string msn = "";
            string COM_DETA_VACA = Convert.ToString(Request.Params["COM_DETA_VACA"].ToString());

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_DETA_VACA"]), out ID_TYPE_DETA_VACA) == false) { sw = 1; msn = ResourceLanguaje.Resource.InformationMissingMsn; }
            else if (COM_DETA_VACA.Trim().Length == 0) { sw = 1; msn = ResourceLanguaje.Resource.VacationMsn07; }
            else
            {
                int ID_VACA = Convert.ToInt32(Request.Form["ID_VACA"]);
                detv.ID_TYPE_DETA_VACA = ID_TYPE_DETA_VACA;
                detv.COM_DETA_VACA = COM_DETA_VACA;
                detv.DAT_REGI = DateTime.Now;
                detv.UserId = Convert.ToInt32(Session["UserId"]);
                detv.ID_VACA = ID_VACA;
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneVaca) top.uploadDoneVaca('ERROR','" + msn + "');}window.onload=init;</script>");
            }
            else
            {
                try
                {
                    dbe.DETAILS_VACATION.Add(detv);
                    dbe.SaveChanges();

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            try
                            {
                                var ATTA = new ATTACHED_VACATION();
                                ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                                ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                ATTA.ID_DETA_VACA = detv.ID_DETA_VACA;
                                ATTA.CREATE_ATTA = DateTime.Now;

                                dbe.ATTACHED_VACATION.Add(ATTA);
                                dbe.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Vacation/Details/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_VACA) + ATTA.EXT_ATTA);
                            }
                            catch
                            {

                            }
                        }
                    }
                    if (detv.ID_TYPE_DETA_VACA == 2)
                    {
                        var vac = dbe.VACATION.Find(detv.ID_VACA);
                        vac.ID_VACA_STAT = 5;
                        vac.ID_PERS_ENTI_DELE = Convert.ToInt32(Session["UserId"]);
                        vac.DAT_REGI_DELE = DateTime.Now;
                        dbe.VACATION.Attach(vac);
                        dbe.Entry(vac).State = EntityState.Modified;
                        dbe.SaveChanges();

                        //--------------------Cambia de Estado a los Registros de Vacation_And_Absence---------------

                        var query = (from va in dbas.VACATION_AND_ABSENCE.Where(x => x.ID_VACA == vac.ID_VACA) select new { va.ID_VACA_ABSE, }).ToList();

                        foreach (var i in query)
                        {
                            int ID_VACA_ABSE = Convert.ToInt32(i.ID_VACA_ABSE);
                            var vaa = dbas.VACATION_AND_ABSENCE.Find(ID_VACA_ABSE);
                            vaa.ID_STAT_VACA = vac.ID_VACA_STAT;
                            vaa.MODIFIED = DateTime.Now;

                            dbas.VACATION_AND_ABSENCE.Attach(vaa);
                            dbas.Entry(vaa).State = EntityState.Modified;
                            dbas.SaveChanges();
                        }

                        //-----------------Cuando se quiera eleminar varios registros-------------------------------
                        // dbas.VACATION_AND_ABSENCE.RemoveRange(dbas.VACATION_AND_ABSENCE.Where(x => x.ID_VACA == vac.ID_VACA)).ToList();

                    }

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneVaca) top.uploadDoneVaca('OK','');}window.onload=init;</script>");
                }
                catch (Exception e)
                {
                    var exc = new EXCEPTION();
                    exc.DES_EXCE = "Error al registrar el detalle de vacaciones: ID_VACA=" + Convert.ToString(detv.ID_VACA);
                    exc.MESSAGE = e.InnerException.Message;
                    exc.DAT_EXCE = DateTime.Now;
                    exc.UserId = Convert.ToInt32(Session["UserId"]);
                    dba.EXCEPTIONs.Add(exc);
                    dba.SaveChanges();

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneVaca) top.uploadDoneVaca('ERROR','" + e.InnerException.Message + "');}window.onload=init;</script>");
                }

            }
        }
    }
}
