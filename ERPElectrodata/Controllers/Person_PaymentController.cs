using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class Person_PaymentController : Controller
    {

        EntityEntities dbe = new EntityEntities();

        //
        // GET: /Person_Payment/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PanelAccountValid(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult PanelDetailsAccountValid(int id = 0, int id1 = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.ID_TYPE_PAYM = id1;
            return View();
        }

        public ActionResult DataAccountValid(int id = 0, int id1 = 0) 
        {
            var result = (from pp in dbe.PERSON_PAYMENT.Where(x => x.ID_PERS_ENTI == id && x.ID_TYPE_PAYM == id1 && x.DAT_END == null).ToList()
                          join tp in dbe.TYPE_PAYMENT on pp.ID_TYPE_PAYM equals tp.ID_TYPE_PAYM
                          join bk in dbe.BANKs on pp.ID_BANK equals bk.ID_BANK
                          select new { 
                            NAM_BANK = bk.NAM_BANK.ToLower(),
                            tp.NAM_TYPE_PAYM,
                            pp.NUM_ACCO,
                            DAT_STAR = (pp.DAT_STAR == null ? "" : String.Format("{0:d}", pp.DAT_STAR)),
                            DAT_END = (pp.DAT_END == null ? "" : String.Format("{0:d}", pp.DAT_END)),
                          });

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListTypePayment()
        {
            var result = (from tp in dbe.TYPE_PAYMENT
                          select new { 
                            tp.ID_TYPE_PAYM,
                            tp.NAM_TYPE_PAYM,
                          }).OrderBy(x=>x.ID_TYPE_PAYM);

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewPersonAccount(PERSON_PAYMENT pers_paym)
        {
            string NUM_ACCO = Convert.ToString(Request.Form["NUM_ACCO"]);
            DateTime DAT_STAR;
            int sw = 0, ID_TYPE_PAYM_NEW = 0, ID_BANK = 0;
            var PP = new PERSON_PAYMENT();

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_PAYM_NEW"]), out ID_TYPE_PAYM_NEW) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_BANK"]), out ID_BANK) == false) { sw = 1; }
            else if (NUM_ACCO.Trim().Length == 0) { sw = 1; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_STAR"]), out DAT_STAR) == false) { sw = 1; }
            else {                
                PP.ID_BANK = ID_BANK;
                PP.ID_TYPE_PAYM = ID_TYPE_PAYM_NEW;
                PP.NUM_ACCO = NUM_ACCO;
                PP.DAT_STAR = DAT_STAR;
                PP.ID_PERS_ENTI = pers_paym.ID_PERS_ENTI;
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            var ppO = dbe.PERSON_PAYMENT.Where(x => x.ID_PERS_ENTI == PP.ID_PERS_ENTI &&
                                                        x.ID_TYPE_PAYM == ID_TYPE_PAYM_NEW &&
                                                        x.DAT_END == null);

            if (ppO.Count() > 0)
            {
                var OldPP = new PERSON_PAYMENT();
                OldPP = ppO.First();
                OldPP.DAT_END = PP.DAT_STAR;
                dbe.PERSON_PAYMENT.Attach(OldPP);
                dbe.Entry(OldPP).State = EntityState.Modified;
            }

            dbe.PERSON_PAYMENT.Add(PP);
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','NewBankAccount');}window.onload=init;</script>");
        }

        public ActionResult ListOldBankAccount(int id = 0, int id1 = 0)
        {
            var result = (from pp in dbe.PERSON_PAYMENT.Where(x=>x.ID_PERS_ENTI == id &&
                                                              x.ID_TYPE_PAYM == id1 && x.DAT_END != null).ToList()
                          join tp in dbe.TYPE_PAYMENT on pp.ID_TYPE_PAYM equals tp.ID_TYPE_PAYM
                          join bk in dbe.BANKs on pp.ID_BANK equals bk.ID_BANK
                          select new
                          {
                              NAM_BANK = bk.NAM_BANK.ToLower(),
                              tp.NAM_TYPE_PAYM,
                              pp.NUM_ACCO,
                              START_DATE = (pp.DAT_STAR == null ? "" : String.Format("{0:d}", pp.DAT_STAR)),
                              END_DATE = (pp.DAT_END == null ? "" : String.Format("{0:d}", pp.DAT_END)),
                              pp.DAT_STAR
                          }).OrderByDescending(x => x.DAT_STAR);

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PanelAccountsOld(int id = 0, int id1 = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.ID_TYPE_PAYM = id1;
            return View();
        }
    }
}
