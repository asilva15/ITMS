using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using WebMatrix.WebData;
using System.Web.Security;
using System.IO;

namespace ERPElectrodata.Controllers
{
    public class TransactionController : Controller
    {
        private CoreEntities db = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();

        //
        // GET: /Transaction/
        public ActionResult Index()
        {
            var transactions = db.TRANSACTIONs.Include(t => t.CATEGORY_TRANSACTION).Include(t => t.ACCOUNTING_MONTH);
            return View(transactions.ToList());
        }

        // GET: /Transaction/Find
        [Authorize]
        public ActionResult Find()
        {
            return View();
        }

        // GET: /Transaction/FindResult
        public ActionResult FindResult()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            int ID_CATE_TRAN, ID_TYPE_TRAN, YEA_TRAN, ID_ACCO_MONT, UserId, Count;

            DateTime START_DATE, END_DATE;
            Decimal AMO_TRANS, TOT_TRAN;

            string DES_TRANS = Convert.ToString(Request.Params["DES_TRANS"]);

            var transac = (from t in db.TRANSACTIONs
                           join c in db.CATEGORY_TRANSACTION on t.ID_CATE_TRAN equals c.ID_CATE_TRAN
                           join tt in db.TYPE_TRANSACTION on c.ID_TYPE_TRAN equals tt.ID_TYPE_TRAN
                           join am in db.ACCOUNTING_MONTH on t.ID_ACCO_MONT equals am.ID_ACCO_MONT
                           join ac in db.ACCOUNTs on t.ID_ACCO equals ac.ID_ACCO
                           where t.ID_ACCO == ID_ACCO
                           select new { 
                               t.ID_ACCO,
                               t.ID_TRAN,
                               t.ID_CATE_TRAN,
                               t.ID_ACCO_MONT,
                               t.DES_TRANS,
                               t.DAT_TRAN,
                               t.CREATE_TRAN,
                               t.MODIFIED_TRAN,
                               t.TYP_CHAN,
                               t.UserId,
                               t.AMO_TRANS,
                               t.IGV_TRAN,
                               TOT_TRAN = t.AMO_TRANS + t.IGV_TRAN,
                               c.ID_TYPE_TRAN,
                               c.NAM_CATE_TRAN,
                               tt.NAM_TYPE_TRAN,
                               tt.CSS_TYPE_TRAN,
                               ac.MON_ACCO,
                               ac.SIM_MONE_ACCO,
                               am.ID_ACCO_YEAR,
                               am.NAM_ACCO_MONT,
                           });

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE_TRAN"]), out ID_CATE_TRAN) == true)
            {
                transac = transac.Where(t => t.ID_CATE_TRAN == ID_CATE_TRAN);
            }
            else if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_TRAN"]), out ID_TYPE_TRAN) == true)
            {
                transac = transac.Where(t => t.ID_CATE_TRAN == ID_TYPE_TRAN);
            }
          
            if (Int32.TryParse(Convert.ToString(Request.Params["YEA_TRAN"]), out YEA_TRAN) == true)
            {
                transac = transac.Where(t => t.ID_ACCO_YEAR == YEA_TRAN);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_ACCO_MONT"]), out ID_ACCO_MONT) == true)
            {
                transac = transac.Where(t => t.ID_ACCO_MONT == ID_ACCO_MONT);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["UserId"]), out UserId) == true)
            {
                transac = transac.Where(t => t.UserId == UserId);
            }
            if (!String.IsNullOrEmpty(DES_TRANS))
            {
                transac = transac.Where(t => t.DES_TRANS.ToUpper().Contains(DES_TRANS.ToUpper()));
            }
            if (Decimal.TryParse(Convert.ToString(Request.Params["AMO_TRANS"]), out AMO_TRANS))
            {
                transac = transac.Where(t => t.AMO_TRANS == AMO_TRANS);
            }
            if (Decimal.TryParse(Convert.ToString(Request.Params["TOT_TRAN"]), out TOT_TRAN))
            {
                transac = transac.Where(t => t.TOT_TRAN == TOT_TRAN);
            }
            if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE"]), out START_DATE))
            {
                transac = transac.Where(t => t.DAT_TRAN >= START_DATE);
            }
            if (DateTime.TryParse(Convert.ToString(Request.Params["END_DATE"]), out END_DATE))
            {
                END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                transac = transac.Where(t => t.DAT_TRAN <= END_DATE);
            }

            Count = transac.Count();
            transac = transac.OrderByDescending(t => t.DAT_TRAN).ThenByDescending(t => t.DAT_TRAN).Skip(skip).Take(take);

            var listuser = (from ce in dbe.CLASS_ENTITY
                            select new
                            {
                                usuario = ce.FIR_NAME + " " + ce.LAS_NAME,
                                ce.UserId,
                            }).ToList();

            var result = (from t in transac.ToList()
                          join lu in listuser on t.UserId equals lu.UserId
                          select new
                          {     
                               t.ID_TRAN,
                               t.CSS_TYPE_TRAN,
                               t.SIM_MONE_ACCO,
                               NAM_TYPE_TRAN = t.NAM_TYPE_TRAN.ToLower(),
                               NAM_CATE_TRAN = t.NAM_CATE_TRAN.ToLower(),
                               NAM_ACCO_MONT = t.NAM_ACCO_MONT.ToLower(),
                               Anio = t.ID_ACCO_YEAR,
                               Mes = t.NAM_ACCO_MONT.ToLower(),
                               AMO_TRANS = Convert.ToString(String.Format("{0:N2}",t.AMO_TRANS)),
                               IGV_TRAN = Convert.ToString(String.Format("{0:N2}", t.IGV_TRAN)),
                               TOT_TRAN = Convert.ToString(String.Format("{0:N2}", t.TOT_TRAN)),
                               t.DES_TRANS,
                               DAT_TRAN = String.Format("{0:G}", t.DAT_TRAN),
                               CREATE_TRAN = String.Format("{0:G}", t.CREATE_TRAN),
                               MODIFIED_TRAN = String.Format("{0:G}", t.MODIFIED_TRAN),
                               TYP_CHAN = Convert.ToString(String.Format("{0:N3}", t.TYP_CHAN)),
                               usuario = lu.usuario.ToLower()
                          });

            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Transaction/Details/5
        public ActionResult Details(int id = 0)
        {
            TRANSACTION transaction = db.TRANSACTIONs.Single(x=>x.ID_TRAN == id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            int ID_TYPE_TRAN = db.CATEGORY_TRANSACTION.Single(x => x.ID_CATE_TRAN == transaction.ID_CATE_TRAN).ID_TYPE_TRAN.Value;
            int Anio = db.ACCOUNTING_MONTH.Single(x => x.ID_ACCO_MONT == transaction.ID_ACCO_MONT).ID_ACCO_YEAR.Value;
            //string mon = db.ACCOUNTs.Single(x => x.ID_ACCO == transaction.ID_ACCO).MON_ACCO;

            ViewBag.ID_TRAN = id;
            ViewBag.ID_TYPE_TRAN = ID_TYPE_TRAN;
            ViewBag.Anio = Anio;
            ViewData["MONEY"] = db.ACCOUNT_PARAMETER.Single(a => a.ID_ACCO == transaction.ID_ACCO && a.ID_PARA == 16).VAL_ACCO_PARA;
            ViewBag.TTTransac = transaction.AMO_TRANS + transaction.IGV_TRAN;
            ViewBag.DAT_TRAN = String.Format("{0:g}", transaction.DAT_TRAN);
            return View(transaction);
        }

        public ActionResult ListAdjuntos(int id = 0)
        {
            var result = (from a in db.ATTACHEDs.Where(x => x.ID_TRANSACTION == id).ToList()
                          select new
                          {
                              fileName = "<a href='/Adjuntos/" + a.NAM_ATTA + "_" + Convert.ToString(a.ID_ATTA) + a.EXT_ATTA + " ' TARGET='_BLANK' title='Download File'>" + a.NAM_ATTA + "_" + Convert.ToString(a.ID_ATTA) + a.EXT_ATTA + "</a>",
                              a.ID_ATTA
                          });

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProfitTresMeses()
        {
            //string[] rolesArray = Roles.GetRolesForUser(WebSecurity.CurrentUserName);
            //string[] IN_ROLE = { "ADMINISTRADOR", "FINANCIAL", "SYSTEMADMINISTRATOR" };
            int index = 0;

            //foreach (string xc in IN_ROLE)
            //{
            //    int i = Array.IndexOf(rolesArray, xc);
            //    if (i == 0)
            //        index = index + 1;
            //}
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                index = index + 1;
            }

            int ID_USER = Convert.ToInt32(Session["UserId"].ToString());
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string IGV = Convert.ToString(Request.Params["igv"].ToString());
            bool bigv = false;

            if (!String.IsNullOrEmpty(IGV))
            {
                if (IGV == "conIGV")
                {
                    bigv = true;
                }
            }

            try
            {
                if (index > 0)
                {
                    var query = db.PROFIT_3_MESES(ID_ACCO, bigv).ToList();

                    return Json(new { result = query }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Content("ERROR");
                }

            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult Comparison(int id)
        {
            int ID_USER = Convert.ToInt32(Session["UserId"].ToString());

            //string[] rolesArray = Roles.GetRolesForUser(WebSecurity.CurrentUserName);
            //string[] IN_ROLE = { "ADMINISTRADOR", "FINANCIAL", "SYSTEMADMINISTRATOR" };
            int index = 0;

            //foreach (string xc in IN_ROLE)
            //{
            //    int i = Array.IndexOf(rolesArray, xc);
            //    if (i == 0)
            //        index = index + 1;
            //}
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                index = index + 1;
            }

            try
            {
                if (index>0)
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                    string money = db.ACCOUNTs.Single(a => a.ID_ACCO == ID_ACCO).SIM_MONE_ACCO;

                    int ID_ACCO_MONT = db.ACCOUNTING_MONTH.Single(am => am.ACCO_MONT == DateTime.Now.Month
                            && am.ID_ACCO_YEAR == DateTime.Now.Year).ID_ACCO_MONT - 2;

                    var query = db.TRANSACTIONs.Where(t => t.ID_ACCO_MONT == ID_ACCO_MONT && t.ID_ACCO == ID_ACCO)
                        .Join(db.CATEGORY_TRANSACTION, t => t.ID_CATE_TRAN, ct => ct.ID_CATE_TRAN,
                            (t, ct) => new { t.AMO_TRANS, ct.NAM_CATE_TRAN, ct.ID_CATE_TRAN, ct.ID_TYPE_TRAN })
                        .Where(t => t.ID_TYPE_TRAN == id);

                    var result = (from t in query.ToList()
                                  group t by new { t.ID_CATE_TRAN, t.NAM_CATE_TRAN } into g
                                  select new
                                  {
                                      name = g.Key.NAM_CATE_TRAN.ToLower(),
                                      total = g.Sum(ct => ct.AMO_TRANS),
                                  }).OrderByDescending(t => t.total);

                    return Json(new { result = result, money = money }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Content("ERROR");
                }
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult ComparisonTable(int id)
        {
            int ID_USER = Convert.ToInt32(Session["UserId"].ToString());
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            //string[] rolesArray = Roles.GetRolesForUser(WebSecurity.CurrentUserName);
            //string[] IN_ROLE = { "ADMINISTRADOR", "FINANCIAL", "SYSTEMADMINISTRATOR" };
            int index = 0;

            //foreach (string xc in IN_ROLE)
            //{
            //    int i = Array.IndexOf(rolesArray, xc);
            //    if (i == 0)
            //        index = index + 1;
            //}
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                index = index + 1;
            }

            try
            {
                if (index>0)
                {
                    var query = db.COMPARISON(id, ID_ACCO).ToList();

                    return Json(new { result = query }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Content("ERROR");
                }

            }
            catch
            {
                return Content("ERROR");
            }
        }
        //
        // GET: /Transaction/Create
        [Authorize]
        public ActionResult Create()
        {
            var cTran = new TRANSACTION();
                        
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            cTran.ID_ACCO = ID_ACCO;
            cTran.DAT_TRAN = DateTime.Now;

            ViewData["MONEY"] = db.ACCOUNT_PARAMETER.Single(a => a.ID_ACCO == ID_ACCO && a.ID_PARA == 16).VAL_ACCO_PARA;
            ViewData["SIM_MONE_ACCO"] = db.ACCOUNT_PARAMETER.Single(a => a.ID_ACCO == ID_ACCO && a.ID_PARA == 15).VAL_ACCO_PARA;
            

            //string[] rolesArray = Roles.GetRolesForUser(WebSecurity.CurrentUserName);
            //string[] IN_ROLE = { "ADMINISTRADOR", "FINANCIAL", "SYSTEMADMINISTRATOR" };
            int index = 0;
            //foreach (string xc in IN_ROLE)
            //{
            //    int i = Array.IndexOf(rolesArray, xc);
            //    if (i == 0)
            //        index = index + 1;
            //}
            //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            //foreach (string rol in rolesArray)
            //{
            //    if (rol == "ADMINISTRADOR" || rol == "FINANCIAL" || rol == "SYSTEMADMINISTRATOR")
            //    {
            //        index = index + 1;
            //    }
            //}
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                index = index + 1;
            }

            if (index > 0)
            {
                cTran.TYP_CHAN = 1;
                if (ViewData["SIM_MONE_ACCO"] == "$.") {
                    cTran.TYP_CHAN = Convert.ToDecimal(2.6);
                }
                return View(cTran);
            }
            else
            {
                return Content("You Haven't Acces");
            }
        }

        //
        // POST: /Transaction/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, TRANSACTION transaction)
        {            
            DateTime DAT_TRAN;
            Decimal AMO_TRANS, IGV_TRAN, TOT_TRAN;

            int sw = 0;
            if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_TRAN"]), out DAT_TRAN) == false) { sw = 1; }
            else if (decimal.TryParse(Convert.ToString(Request.Form["AMO_TRANS"]), out AMO_TRANS) == false) { sw = 1; }
            else if (decimal.TryParse(Convert.ToString(Request.Form["IGV_TRAN"]), out IGV_TRAN) == false) { sw = 1; }
            else if (decimal.TryParse(Convert.ToString(Request.Form["TOT_TRAN"]), out TOT_TRAN) == false) { sw = 1; }

            string desc = Convert.ToString(Request.Form["DES_TRANS"]);
            desc = desc.Replace("&nbsp;", "");
            desc = desc.Replace("<br />", "");
            if (desc.Trim().Length == 0) { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            transaction.ID_ACCO = ID_ACCO;
            transaction.CREATE_TRAN = DateTime.Now;
            transaction.UserId = Convert.ToInt32(Session["UserId"].ToString());
            db.TRANSACTIONs.Add(transaction);
            db.SaveChanges();

            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var ATTA = new ATTACHED();
                        ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ATTA.ID_TRANSACTION = transaction.ID_TRAN;
                        ATTA.CREATE_ATTA = DateTime.Now;

                        db.ATTACHEDs.Add(ATTA);
                        db.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                    }
                }
            }
            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('OK','');}window.onload=init;</script>");

        }

        //
        // GET: /Transaction/Edit/5
        public ActionResult Edit(int id = 0)
        {
            TRANSACTION transaction = db.TRANSACTIONs.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_CATE_TRAN = new SelectList(db.CATEGORY_TRANSACTION, "ID_CATE_TRAN", "NAM_CATE_TRAN", transaction.ID_CATE_TRAN);
            ViewBag.ID_ACCO_MONT = new SelectList(db.ACCOUNTING_MONTH, "ID_ACCO_MONT", "NAM_ACCO_MONT", transaction.ID_ACCO_MONT);
            return View(transaction);
        }

        //
        // POST: /Transaction/Edit/5
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(IEnumerable<HttpPostedFileBase> files, TRANSACTION transaction)
        {
            DateTime DAT_TRAN;
            Decimal AMO_TRANS, IGV_TRAN, TOT_TRAN;
            int ID_CATE_TRAN, YEA_TRAN, ID_ACCO_MONT;

            int sw = 0;            
            if (int.TryParse(Convert.ToString(Request.Form["ID_CATE_TRAN"]), out ID_CATE_TRAN) == false) { sw = 1; }
            else if (int.TryParse(Convert.ToString(Request.Form["YEA_TRAN"]), out YEA_TRAN) == false) { sw = 1; }
            else if (int.TryParse(Convert.ToString(Request.Form["ID_ACCO_MONT"]), out ID_ACCO_MONT) == false) { sw = 1; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_TRAN"]), out DAT_TRAN) == false) { sw = 1; }
            else if (decimal.TryParse(Convert.ToString(Request.Form["AMO_TRANS"]), out AMO_TRANS) == false) { sw = 1; }
            else if (decimal.TryParse(Convert.ToString(Request.Form["IGV_TRAN"]), out IGV_TRAN) == false) { sw = 1; }
            else if (decimal.TryParse(Convert.ToString(Request.Form["TOT_TRAN"]), out TOT_TRAN) == false) { sw = 1; }

            string desc = Convert.ToString(Request.Form["DES_TRANS"]);
            desc = desc.Replace("&nbsp;", "");
            desc = desc.Replace("<br />", "");
            if (desc.Trim().Length == 0) { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            try
            {   //Guardando los cambios
                TRANSACTION trans = new TRANSACTION();
                trans = db.TRANSACTIONs.Single(x => x.ID_TRAN == transaction.ID_TRAN);
                trans.DAT_TRAN = transaction.DAT_TRAN;
                trans.ID_CATE_TRAN = transaction.ID_CATE_TRAN;
                trans.ID_ACCO_MONT = transaction.ID_ACCO_MONT;
                trans.AMO_TRANS = transaction.AMO_TRANS;
                trans.IGV_TRAN = transaction.IGV_TRAN;
                trans.DES_TRANS = transaction.DES_TRANS;
                
                db.Entry(trans).State = EntityState.Modified;
                db.SaveChanges();

                //Guardando los adjuntos
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_TRANSACTION = transaction.ID_TRAN;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            db.ATTACHEDs.Add(ATTA);
                            db.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {
                        }
                    }
                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
            }
        }


        public string DeleteAttach(int id = 0) {
            try
            {
                ATTACHED atta = db.ATTACHEDs.Find(id);
                try
                {
                    string arc = atta.NAM_ATTA + "_" + Convert.ToString(atta.ID_ATTA) + atta.EXT_ATTA;
                    db.ATTACHEDs.Remove(atta);
                    db.SaveChanges(); 

                    //Eliminando
                    System.IO.File.Delete(Server.MapPath("~/Adjuntos/" + arc));               
                }
                catch (Exception)
                {
                    return "ERROR";
                }   
                return "OK";
            }
            catch (Exception)
            {
                return "ERROR";
            }

        }
        //
        // GET: /Transaction/Delete/5
        public ActionResult Delete(int id = 0)
        {
            TRANSACTION transaction = db.TRANSACTIONs.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        //
        // POST: /Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            TRANSACTION transaction = db.TRANSACTIONs.Find(id);
            db.TRANSACTIONs.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}