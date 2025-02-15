using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ERPElectrodata.Models;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class DocumentSaleActivityController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /DocumentSaleActivity/

        [Authorize]
        public ActionResult Index(int id = 0)
        {
            var q = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id)
                .Join(dbc.TYPE_DOCUMENT_SALE, x => x.ID_TYPE_DOCU_SALE, td => td.ID_TYPE_DOCU_SALE, (x, td) => new
                {
                    x.ID_STAT_DOCU_SALE,
                    x.NUM_DOCU_SALE,
                    td.COD_TYPE_DOCU_SALE,
                });

            if (q.Count() > 0) {
                ViewBag.Status = q.First().ID_STAT_DOCU_SALE.Value;
                ViewBag.Titulo = q.First().COD_TYPE_DOCU_SALE + " " + q.First().NUM_DOCU_SALE;
                ViewBag.ID_DOCU_SALE = id;
                ViewBag.ID_STAT_DOCU_SALE = q.First().ID_STAT_DOCU_SALE.Value;            
            }

            return View();
        }

        public ActionResult ListByIdDocuSale(int id = 0)
        {

            var qContact = (from a in dbe.CLASS_ENTITY
                            join b in dbe.PERSON_ENTITY on a.ID_ENTI equals b.ID_ENTI2
                            join c in dbe.CLASS_ENTITY on b.ID_ENTI1 equals c.ID_ENTI
                            select new
                            {
                                ID_CTTS = a.ID_ENTI,
                                USER_PERS = a.FIR_NAME + " " + (a.LAS_NAME == null ? "" : a.LAS_NAME),
                                TEL_PERS = (b.TEL_PERS == null ? "-" : (b.TEL_PERS.Trim().Length == 0 ? "-" : b.TEL_PERS)),
                                EMA_PERS = (b.EMA_PERS == null ? "-" : (b.EMA_PERS.Trim().Length == 0 ? "-" : b.EMA_PERS)).ToLower(),
                                CAR_PERS = (b.CAR_PERS == null ? "-" : (b.CAR_PERS.Trim().Length == 0 ? "-" : b.CAR_PERS)).ToLower(),
                                EXT_PERS = (b.EXT_PERS == null ? "-" : (b.EXT_PERS.Trim().Length == 0 ? "-" : b.EXT_PERS)),
                                ID_COMP = c.ID_ENTI,
                                COM_NAME = (c.COM_NAME == null ? "-" : c.COM_NAME),
                                ADDRESS = (c.ADDRESS == null ? "-" : (c.ADDRESS.Trim().Length == 0 ? "-" : c.ADDRESS)).ToLower(),
                                TEL_ENTI = (c.TEL_ENTI == null ? "-" : (c.TEL_ENTI.Trim().Length == 0 ? "-" : c.TEL_ENTI)),
                                EXT_ENTI = (c.EXT_ENTI == null ? "-" : (c.EXT_ENTI.Trim().Length == 0 ? "-" : c.EXT_ENTI)),
                            });

            var qClient = (from c in dbe.CLASS_ENTITY
                           select new
                           {
                               ID_COMP = c.ID_ENTI,
                               COM_NAME = (c.COM_NAME == null ? "-" : c.COM_NAME),
                               ADDRESS = (c.ADDRESS == null ? "-" : (c.ADDRESS.Trim().Length == 0 ? "-" : c.ADDRESS)).ToLower(),
                               TEL_ENTI = (c.TEL_ENTI == null ? "-" : (c.TEL_ENTI.Trim().Length == 0 ? "-" : c.TEL_ENTI)),
                               EXT_ENTI = (c.EXT_ENTI == null ? "-" : (c.EXT_ENTI.Trim().Length == 0 ? "-" : c.EXT_ENTI)),
                           });

            var qVendor = (from a in dbe.PERSON_ENTITY
                           join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                           select new
                           {
                               a.ID_PERS_ENTI,
                               USER = (b.FIR_NAME + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME)).ToLower(),
                               TEL_PERS = (a.TEL_PERS == null ? "-" : (a.TEL_PERS.Trim().Length == 0 ? "-" : a.TEL_PERS)),
                               EMA_PERS = (a.EMA_PERS == null ? "-" : (a.EMA_PERS.Trim().Length == 0 ? "-" : a.EMA_PERS)).ToLower(),
                               EMA_ELEC = (a.EMA_ELEC == null ? "-" : (a.EMA_ELEC.Trim().Length == 0 ? "-" : a.EMA_ELEC)).ToLower(),
                               EXT_PERS = (a.EXT_PERS == null ? "-" : (a.EXT_PERS.Trim().Length == 0 ? "-" : a.EXT_PERS)),
                               b.UserId,
                               ID_FOTO = (a.ID_FOTO == null ? 0 : a.ID_FOTO),
                           });

            var query = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).ToList()
                         join e in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals e.ID_TYPE_DOCU_SALE
                         join f in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals f.ID_STAT_DOCU_SALE
                         join b in qContact on a.ID_CTTS equals b.ID_CTTS
                         join c in qClient on a.ID_COMP_END equals c.ID_COMP
                         join d in qVendor on a.ID_PERS_ENTI_VEND equals d.ID_PERS_ENTI
                         join g in qVendor on a.UserId equals g.UserId
                         join h in qClient on a.ID_COMP equals h.ID_COMP
                         join i in qClient on a.ID_ENTI equals i.ID_COMP
                         join t in dbc.TICKETs on a.ID_DOCU_SALE equals t.ID_DOCU_SALE into lt
                         from xt in lt.DefaultIfEmpty()
                         join j in qVendor on a.UserId equals j.UserId into lj
                         from xj in lj.DefaultIfEmpty()
                         select new
                         {
                             a.ID_DOCU_SALE,
                             a.NUM_DOCU_SALE,
                             a.OS,
                             a.SUM_DOCU_SALE,
                             e.COD_TYPE_DOCU_SALE,
                             NAM_STAT_DOCU_SALE = f.NAM_STAT_DOCU_SALE.ToLower(),
                             DAT_DOCU_SALE = (a.DAT_DOCU_SALE == null ? "" : string.Format("{0:d}", a.DAT_DOCU_SALE)),
                             EXP_DATE = (a.EXP_DATE == null ? "" : string.Format("{0:d}", a.EXP_DATE)),
                             UserRegDate = (a.DAT_REGISTER == null ? "" : string.Format("{0:d}", a.DAT_REGISTER)),
                             UserReg = g.USER,
                             UserRegTel = g.TEL_PERS,
                             UserRegEma = g.EMA_PERS,
                             UserRegExt = g.EXT_PERS,
                             ClieName = h.COM_NAME.ToLower(),
                             ClieAddr = h.ADDRESS,
                             ClieTele = h.TEL_ENTI,
                             ClieExte = h.EXT_ENTI,
                             EndClieName = c.COM_NAME.ToLower(),
                             EndClieAddr = c.ADDRESS,
                             EndClieTele = c.TEL_ENTI,
                             EndClieExte = c.EXT_ENTI,
                             ContUser = b.USER_PERS,
                             ContTele = b.TEL_PERS,
                             ContExte = b.EXT_PERS,
                             ContJobT = b.CAR_PERS,
                             Vendor = d.USER,
                             VendorEma = d.EMA_ELEC,
                             VendorTel = d.TEL_PERS,
                             VendorExt = d.EXT_PERS,
                             NAM_COMP = i.COM_NAME.ToLower(),
                             ID_TICK = (xt == null ? 0 : xt.ID_TICK),
                             COD_TICK = (xt == null ? "-" : xt.COD_TICK),
                             NameUser = (xj==null?"":xj.USER),
                             UserFoto = (xj==null?0:xj.ID_FOTO)
                         });

            var qActivity = (from a in dbc.DOCUMENT_SALE_ACTIVITY.Where(x => x.ID_DOCU_SALE == id).ToList()
                             join b in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals b.ID_STAT_DOCU_SALE
                             join c in dbc.TYPE_ACTIVITY on a.ID_TYPE_ACTI equals c.ID_TYPE_ACTI
                             join d in qVendor on a.UserId equals d.UserId
                             select new
                             {
                                 a.COM_ACTI_DOCU_SALE,
                                 a.ID_TYPE_ACTI,
                                 NAM_STAT_DOCU_SALE = b.NAM_STAT_DOCU_SALE.ToLower(),
                                 NAM_TYPE_ACTI = c.NAM_TYPE_ACTI.ToLower(),
                                 User = d.USER,
                                 PHOTO = d.ID_FOTO,
                                 CREATED = (a.CREATE_ACTI_DOCU_SALE == null ? "" : String.Format("{0:d}", a.CREATE_ACTI_DOCU_SALE) + " " + String.Format("{0:t}", a.CREATE_ACTI_DOCU_SALE)),
                                 ADJ = AttaActiDocuSale(Convert.ToInt32(a.ID_ACTI_DOCU_SALE), Convert.ToInt32(a.ID_TYPE_ACTI), Convert.ToInt32(a.ID_DOCU_SALE)),
                                 ASSI = AssignOP(Convert.ToInt32(a.ID_DOCU_SALE)),
                                 a.CREATE_ACTI_DOCU_SALE,
                             }).OrderByDescending(x => x.CREATE_ACTI_DOCU_SALE);

            return Json(new { Data = query, Count = query.Count(), Data1 = qActivity }, JsonRequestBehavior.AllowGet);
        }

        public string AttaActiDocuSale(int id, int idtype, int idds)
        {
            string Adjun = "";
            try
            {
                if (idtype == 4) //Obteniendo el act de conformidad del knowledge
                {
                    var query = dbc.KNOWLEDGEs.Where(a => a.ID_DOCU_SALE == idds);
                    foreach (KNOWLEDGE subqu in query)
                    {
                        Adjun = Adjun + "<li><a href='../../Adjuntos/Knowledge/" + subqu.NAM_ATTA + "_" + subqu.ID_KNOW + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                }
                else
                {
                    var query = dbc.ATTACHEDs.Where(a => a.ID_ACTI_DOCU_SALE == id);
                    foreach (ATTACHED subqu in query)
                    {
                        Adjun = Adjun + "<li><a href='../../Adjuntos/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        public string AssignOP(int idds)
        {

            var qAssign = (from a in dbe.PERSON_ENTITY
                           join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                           select new
                           {
                               a.ID_PERS_ENTI,
                               USER = (b.FIR_NAME + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME)).ToLower(),
                           });

            var query = (from a in dbc.TICKETs.Where(x => x.ID_DOCU_SALE == idds && x.IS_PARENT == true).ToList()
                         join b in qAssign on a.ID_PERS_ENTI_ASSI equals b.ID_PERS_ENTI
                         select new
                         {
                             b.USER,
                         });

            string usuario = "";
            if (query.Count() > 0)
            {
                usuario = query.First().USER;
            }

            return usuario;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateActivity(IEnumerable<HttpPostedFileBase> files, DOCUMENT_SALE_ACTIVITY dsa)
        {
            int TA = 0, ID_ACCO_MONT;
            DateTime DATE_ATTA;
            string msnError = "", msnErrorTitle = ResourceLanguaje.Resource.InformationMissing;

            if (dsa.ID_TYPE_ACTI.HasValue == false) { msnError = ResourceLanguaje.Resource.Message3; }
            else if (dsa.ID_TYPE_ACTI.Value == 4) //Caso registrando el acta de conformidad
            {
                if (DateTime.TryParse(Convert.ToString(Request.Form["DATE_ATTA"]), out DATE_ATTA) == false) { msnError = ResourceLanguaje.Resource.SaleMsn07; }
                else if (String.IsNullOrEmpty(dsa.COM_ACTI_DOCU_SALE)) { msnError = ResourceLanguaje.Resource.Message1; }
                else if (files == null) { msnError = ResourceLanguaje.Resource.AttachMsn; }
            }
            else if (String.IsNullOrEmpty(dsa.COM_ACTI_DOCU_SALE)) { msnError = ResourceLanguaje.Resource.Message1; }

            if (msnError.Length > 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + msnErrorTitle + "','" + msnError + "');}window.onload=init;</script>");
            }

            //Registrando
            int ID_DOCU_SALE = Convert.ToInt32(Request.Form["ID_DOCU_SALE_HF"]);
            TA = dsa.ID_TYPE_ACTI.Value;

            if (TA == 1 || TA == 4 || TA == 5 || TA == 6)
            { //Log Comment & Resolved
                dsa.ID_DOCU_SALE = ID_DOCU_SALE;
                dsa.UserId = Convert.ToInt32(Session["UserId"]);
                dsa.CREATE_ACTI_DOCU_SALE = DateTime.Now;
                dbc.DOCUMENT_SALE_ACTIVITY.Add(dsa);
                dbc.SaveChanges();

                if (files != null && TA != 4)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_ACTI_DOCU_SALE = dsa.ID_ACTI_DOCU_SALE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                if (TA == 5 || TA == 6)
                { //Actualizando el Document Sale a Resolved o Closed
                    var ds = dbc.DOCUMENT_SALE.Find(ID_DOCU_SALE);
                    ds.ID_STAT_DOCU_SALE = (TA == 5 ? 3 : 4);
                    dbc.DOCUMENT_SALE.Attach(ds);
                    dbc.Entry(ds).State = EntityState.Modified;
                    dbc.SaveChanges();
                }
            }
            if (TA == 4)
            { //Registrando el acta de conformidad
                DATE_ATTA = Convert.ToDateTime(Request.Form["DATE_ATTA"]);

                var ds = dbc.DOCUMENT_SALE.Find(ID_DOCU_SALE);
                string cia = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ds.ID_COMP).COM_NAME;
                var qAM = dbc.ACCOUNTING_MONTH.Where(x => x.ACCO_MONT == DATE_ATTA.Month && x.ID_ACCO_YEAR == DATE_ATTA.Year);
                if (qAM.Count() > 0)
                {

                    ID_ACCO_MONT = qAM.First().ID_ACCO_MONT;
                    var qKnow = dbc.KNOWLEDGEs.Where(x => x.ID_ACCO_MONT == ID_ACCO_MONT && x.ID_KNOW_CATE == 17);
                    string NAM_ATTA = DATE_ATTA.Month.ToString() + "." + (qKnow.Count() + 1).ToString() + " act of conformity";

                    try
                    {   //registrando el acta de conformidad en el Knowledge
                        var NewKnow = new KNOWLEDGE();
                        NewKnow.ID_AREA = 63;
                        NewKnow.ID_KNOW_CATE = 17;
                        NewKnow.NAM_KNOW = NAM_ATTA;
                        NewKnow.DESC_KNOW = cia.ToUpper();
                        NewKnow.NAM_ATTA = NAM_ATTA;
                        NewKnow.EXT_ATTA = ".pdf";
                        NewKnow.DATE_ATTA = DATE_ATTA;
                        NewKnow.CREATED_KNOW = DateTime.Now;
                        NewKnow.UserId = Convert.ToInt32(Session["UserId"]);
                        NewKnow.VIG_KNOW = true;
                        NewKnow.ID_ACCO_MONT = ID_ACCO_MONT;
                        NewKnow.ID_DOCU_SALE = ID_DOCU_SALE;
                        dbc.KNOWLEDGEs.Add(NewKnow);
                        dbc.SaveChanges();

                        try
                        {
                            foreach (var file in files)
                            {
                                if (file != null)
                                {
                                    file.SaveAs(Server.MapPath("~/Adjuntos/Knowledge/" + NewKnow.NAM_ATTA + "_" + Convert.ToString(NewKnow.ID_KNOW) + ".pdf"));
                                }
                            }
                            //Actualizando el estado del Document Sale a Closed
                            ds.ID_STAT_DOCU_SALE = 4;
                            dbc.DOCUMENT_SALE.Attach(ds);
                            dbc.Entry(ds).State = EntityState.Modified;
                            dbc.SaveChanges();
                        }
                        catch
                        {
                            KNOWLEDGE know = dbc.KNOWLEDGEs.Find(NewKnow.ID_KNOW);
                            dbc.KNOWLEDGEs.Remove(know);
                            dbc.SaveChanges();
                        }
                        string proc = Convert.ToString(Request.Form["procAC_HF"]);

                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('OK_ACTCONF','" + ResourceLanguaje.Resource.InformationSaved + "','" + ResourceLanguaje.Resource.SaleMsn05 + "');}window.onload=init;</script>");
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','" + ResourceLanguaje.Resource.BasedDrawback + "','" + ResourceLanguaje.Resource.BasedDrawbackMsn + "');}window.onload=init;</script>");
                    }
                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + ResourceLanguaje.Resource.InformationMissing + "','" + ResourceLanguaje.Resource.SalesMsn04 + "');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + ResourceLanguaje.Resource.InformationSaved + "','" + @ResourceLanguaje.Resource.Message4 + "');}window.onload=init;</script>");
        }

        public ActionResult ListOPsNotClosed() {
            var qCOMP = (from a in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null).OrderBy(x => x.COM_NAME)
                         select new { 
                            a.COM_NAME,
                            a.ID_ENTI
                         });

            var query = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE != 4).ToList()
                         join b in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals b.ID_TYPE_DOCU_SALE
                         join c in qCOMP on a.ID_COMP equals c.ID_ENTI
                         join d in qCOMP on a.ID_COMP_END equals d.ID_ENTI
                         select new { 
                            a.ID_DOCU_SALE,
                            a.NUM_DOCU_SALE,
                            b.NAM_TYPE_DOCU_SALE,
                            CIA_1 = c.COM_NAME,
                            CIA_2 = d.COM_NAME,
                            COM_NAME = c.COM_NAME.ToLower(),
                            COM_NAME_END = d.COM_NAME.ToLower(),
                            TextDS = b.NAM_TYPE_DOCU_SALE + ": " + a.NUM_DOCU_SALE.Trim(),
                            dataTextDS = b.NAM_TYPE_DOCU_SALE + ": " + a.NUM_DOCU_SALE.Trim() + " (" + c.COM_NAME + " / " + d.COM_NAME + ")",
                         });

            return Json(new { Data = query, Count = query.Count()},JsonRequestBehavior.AllowGet);
        }
    }
}
