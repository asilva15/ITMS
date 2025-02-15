using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.IO;
using System.Data;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class TrainingController : Controller
    {

        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Training/
        public ActionResult Index(int id = 0)
        {
            var query = (from a in dbe.PERSON_TRAINING.Where(x => x.ID_PERS_REQU == id ||
                                    x.ID_PERS_SUPE == id || x.ID_PERS_OPER == id)
                         select new
                         {
                             a.STATUS
                         });

            ViewBag.ID_PERS_ENTI = id;
            ViewBag.TProcessing = query.Where(x => x.STATUS == 0).Count(); ;
            ViewBag.TByApproving = query.Where(x => x.STATUS == 1).Count(); ; ;
            ViewBag.TApprovals = query.Where(x => x.STATUS == 2).Count(); ; ;
            ViewBag.TResults = query.Where(x => x.STATUS == 3).Count(); ; ;
            return View(); 
        }

        public ActionResult ListByStatus(int id = 0)
        {
            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            
            var result = (from a in dbe.PERSON_TRAINING.Where(x => x.ID_PERS_REQU == id ||
                                                              x.ID_PERS_SUPE == id ||
                                                              x.ID_PERS_OPER == id)
                          .Where(x=>x.STATUS == ID_STAT)
                          .OrderBy(x => x.DAT_REQU).Skip(skip).Take(take).ToList()
                          join b in dbe.PERSON_ENTITY on a.ID_PERS_REQU equals b.ID_PERS_ENTI
                          join ce1 in dbe.CLASS_ENTITY on b.ID_ENTI2 equals ce1.ID_ENTI
                          join c in dbe.PERSON_ENTITY on (a.ID_PERS_SUPE==null ? 0 : a.ID_PERS_SUPE) equals c.ID_PERS_ENTI into lc
                          from xc in lc.DefaultIfEmpty()
                          join ce2 in dbe.CLASS_ENTITY on (xc == null ? 0 : xc.ID_ENTI2) equals ce2.ID_ENTI into lce2
                          from xce2 in lce2.DefaultIfEmpty()
                          join d in dbe.PERSON_ENTITY on a.ID_PERS_OPER equals d.ID_PERS_ENTI into ld
                          from xd in ld.DefaultIfEmpty()
                          join ce3 in dbe.CLASS_ENTITY on (xd == null ? 0 : xd.ID_ENTI2) equals ce3.ID_ENTI into lce3
                          from xce3 in lce3.DefaultIfEmpty()
                          join e in dbe.TYPE_TRAINING on a.ID_TYPE_TRAI equals e.ID_TYPE_TRAI
                          join f in dbe.MANUFACTURER_TRAINING on a.ID_TRAI_MANU equals f.ID_TRAI_MANU

                          join ct in dbe.PERSON_CONTRACT.Where(x=>x.VIG_CONT == true && x.LAS_CONT == true) on 
                                (a.ID_PERS_OPER == null ? 0 :a.ID_PERS_OPER) equals ct.ID_PERS_ENTI into lct
                          from xct in lct.DefaultIfEmpty()
                          join ca in dbe.CARGO_AREA on (xct == null ? 0 : xct.ID_CARG_AREA) equals ca.ID_CARG_AREA into lca
                          from xca in lca.DefaultIfEmpty()
                          join cg in dbe.CARGOes on (xca == null ? 0 : xca.ID_CARG) equals cg.ID_CARG into lcg
                          from xcg in lcg.DefaultIfEmpty()

                          select new {
                              a.CODE_TRAI,
                              a.ID_PERS_TRAI,
                              Request = CapitalizeCadena(ce1.FIR_NAME + " " + ce1.LAS_NAME),
                              a.NAM_REVIEW,
                              DAT_REVIEW = (a.DAT_REVIEW == null ? "-" : string.Format("{0:d}",a.DAT_REVIEW)),
                              a.MONEY,
                              AMOUNT = string.Format("{0:0.00}",a.AMOUNT),
                              ID_STAT = a.STATUS,
                              VIS_BT_APPR_SUP = (a.STATUS == 1 && a.ID_PERS_SUPE == id && a.DAT_APPR_SUPE == null ? true : false),
                              VIS_BT_APPR_OPE = (a.STATUS == 1 && a.ID_PERS_OPER == id && a.DAT_APPR_SUPE != null && a.DAT_APPR_OPER == null ? true : false),
                              VIS_APPR_OPE = (a.STATUS == 1 && a.DAT_APPR_SUPE != null && a.DAT_APPR_OPER == null ? true : false),
                              CARG_OPER = CapitalizeCadena(xcg == null ? "" : xcg.NAM_CARG),
                              Status = (a.STATUS == 0 ? ResourceLanguaje.Resource.Processing : 
                                       (a.STATUS == 1 ? ResourceLanguaje.Resource.ByApproving : 
                                       (a.STATUS == 2 ? ResourceLanguaje.Resource.Approvals : ResourceLanguaje.Resource.Results))),
                              DAT_REQU = (a.DAT_REQU == null ? "-" : string.Format("{0:g}", a.DAT_REQU)),
                              DATE_REQ = a.DAT_REQU,
                              Supervisor = CapitalizeCadena((xce2 == null ? "-" : xce2.FIR_NAME + " " + xce2.LAS_NAME)),
                              DAT_APPR_SUPE = (a.DAT_APPR_SUPE == null ? "-" : string.Format("{0:g}", a.DAT_APPR_SUPE)),
                              GGOperaciones = CapitalizeCadena((xce3 == null ? "-" : xce3.FIR_NAME + " " + xce3.LAS_NAME)),
                              DAT_APPR_OPER = (a.DAT_APPR_OPER == null ? "-" : string.Format("{0:g}", a.DAT_APPR_OPER)),
                              STAT_PAYM = (a.STAT_PAYM == true ? "Si" : "No"),
                              DAT_PAYM = (a.DAT_PAYM == null ? "-" : string.Format("{0:d}", a.DAT_PAYM)),
                              NAM_MANU = CapitalizeCadena(f.NAM_MANU.ToLower()),
                              NAM_TRAI = CapitalizeCadena(e.NAM_TRAI.ToLower()),
                              NAM_ATTA = (a.NAM_ATTA == null ? "-" : a.NAM_ATTA),
                              NAM_FILE = (a.NAM_ATTA == null ? "-" : a.NAM_ATTA + "_" + Convert.ToString(a.ID_PERS_TRAI) + ".pdf"),
                              SUM_TRAI = (a.SUM_TRAI == null ? "" : a.SUM_TRAI),
                          }).OrderByDescending(x => x.DATE_REQ);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewNewTraining(int id = 0) {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public int ID_Supervisor(int idCaAr = 0) {

            var query = dbe.CARGO_AREA.Where(x => x.ID_CARG_AREA == idCaAr);
            int idPaCaAr = 0, idArea = 0, idCaArSup = 0, idSup = 0;

            if (query.Count() > 0) {
                var query1 = query.First();
                idPaCaAr = (query1.ID_CARG_PARE.HasValue == true ? query1.ID_CARG_PARE.Value : 0 );
                idArea = (query1.ID_AREA.HasValue == true ? query1.ID_AREA.Value : 0);

                var query2 = dbe.CARGO_AREA.Where(x => x.ID_AREA == idArea && x.ID_CARG == idPaCaAr);
                if (query2.Count() > 0)
                {
                    idCaArSup = query2.First().ID_CARG_AREA;

                    var query3 = dbe.PERSON_CONTRACT.Where(x => x.ID_CARG_AREA == idCaArSup && x.LAS_CONT == true && x.VIG_CONT == true);
                    if (query3.Count() > 0)
                    {
                        idSup = query3.First().ID_PERS_ENTI.Value;
                    }
                }
                else { 
                    
                }
            }

            return idSup;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewTraining(IEnumerable<HttpPostedFileBase> filesTrai,PERSON_TRAINING ptra)
        {
            int sw = 0, ID_TYPE_TRAI, ID_TRAI_MANU = 0, ID_CARG_AREA = 0, ID_PERS_SUPE = 0;
            DateTime DAT_REVIEW;
            Decimal AMOUNT = 0;

            string code = "";
            string NAM_REVIEW = Convert.ToString(Request.Form["NAM_REVIEW"]);
            string MONEY = Convert.ToString(Request.Form["MONEY"]);
            PERSON_TRAINING pt = new PERSON_TRAINING();

            if (NAM_REVIEW.Trim().Length == 0) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_TRAI"]), out ID_TYPE_TRAI) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_TRAI_MANU"]), out ID_TRAI_MANU) == false) { sw = 1; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["DAT_REVIEW"]), out DAT_REVIEW) == false) { sw = 1; }
            else if (MONEY != "US$" && MONEY != "S/.") { sw = 1; }
            else if (Decimal.TryParse(Convert.ToString(Request.Form["AMOUNT"]), out AMOUNT) == false) { sw = 1; }
            else {
                int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);

                pt.STATUS = 0;

                var cst1 = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.LAS_CONT == true);
                if (cst1.Count() > 0) {
                    ID_CARG_AREA = cst1.First().ID_CARG_AREA.Value;

                    var qCar = dbe.CARGO_AREA.Where(x => x.ID_CARG_AREA == ID_CARG_AREA)
                               .Join(dbe.CARGOes,x=>x.ID_CARG,ca=>ca.ID_CARG,(x,ca)=>new{
                                ca.NAM_CARG,
                                ca.MANAGEMENT,
                               });
                    //En caso que usuario es Gerente
                    if (qCar.First().MANAGEMENT == true)
                    {
                        pt.ID_PERS_SUPE = ID_PERS_ENTI;
                        pt.DAT_APPR_SUPE = DateTime.Now;
                        pt.ID_PERS_OPER = ID_PERS_ENTI;
                        pt.DAT_APPR_OPER = DateTime.Now;
                        pt.STATUS = 2;
                    }
                    else {
                        //Obteniendo el supervisor del usuario
                        pt.ID_PERS_SUPE = ID_Supervisor(ID_CARG_AREA);
                        
                        if (pt.ID_PERS_SUPE == 0) //en caso que el usuario es supervisor.
                        {
                            pt.STATUS = 1;
                            pt.ID_PERS_SUPE = ID_PERS_ENTI;
                            pt.DAT_APPR_SUPE = DateTime.Now;
                            int idpoer = GET_ID_PERS_OPER(ID_PERS_ENTI);
                            if (idpoer != 0) {
                                pt.ID_PERS_OPER = idpoer;
                            }
                        }                    
                    }
                }                

                pt.ID_PERS_REQU = ID_PERS_ENTI;
                pt.ID_CARG_AREA = ID_CARG_AREA;                
                pt.ID_TYPE_TRAI = ID_TYPE_TRAI;
                pt.NAM_REVIEW = NAM_REVIEW;
                pt.DAT_REVIEW = DAT_REVIEW;
                pt.ID_TRAI_MANU = ID_TRAI_MANU;
                pt.MONEY = MONEY;
                pt.AMOUNT = AMOUNT;                
                pt.DAT_REQU = DateTime.Now;
                pt.STAT_PAYM = false;
                pt.SUM_TRAI = ptra.SUM_TRAI;
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            else {
                try
                {
                    dbe.PERSON_TRAINING.Add(pt);
                    dbe.SaveChanges();
                    dbe.Entry(pt).State = EntityState.Detached;

                    int ID_PERS_TRAI = Convert.ToInt32(pt.ID_PERS_TRAI);                    
                    code = dbe.PERSON_TRAINING.Single(x => x.ID_PERS_TRAI == ID_PERS_TRAI).CODE_TRAI;
                    
                    if (filesTrai != null) {
                        foreach (var file in filesTrai)
                        {
                            string NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            string EXT_ATTA = Path.GetExtension(file.FileName);

                            pt.NAM_ATTA = NAM_ATTA;
                            dbe.PERSON_TRAINING.Attach(pt);
                            dbe.Entry(pt).State = EntityState.Modified;
                            dbe.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Training/" + NAM_ATTA + "_" + Convert.ToString(ID_PERS_TRAI) + EXT_ATTA));                        
                        }
                    }
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','Training','" + code + "');}window.onload=init;</script>");
            }        
        }

        public int GET_ID_PERS_OPER(int id = 0) {
            int ID_PERS_OPER = 0;
            //Obteniendo el ID del Management
            var query1 = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == id && x.VIG_CONT == true && x.LAS_CONT == true);
            if (query1.Count() > 0)
            {
                int ID_CARG_AREA = query1.Single().ID_CARG_AREA.Value;
                var ID_AREA = dbe.CARGO_AREA.Single(x => x.ID_CARG_AREA == ID_CARG_AREA).ID_AREA;
                var query2 = dbe.CARGO_AREA.Where(x => x.ID_AREA == ID_AREA);
                var crg = (from a in dbe.CARGOes.Where(x => x.MANAGEMENT == true)
                           where (from b in query2 select b.ID_CARG).Contains(a.ID_CARG) ||
                                  (from b in query2 select b.ID_CARG_PARE).Contains(a.ID_CARG)
                           select a);
                if (crg.Count() > 0)
                {
                    int ID_CARG = crg.First().ID_CARG;
                    int ID_CARG_AREA_MN = dbe.CARGO_AREA.Single(x => x.ID_CARG == ID_CARG).ID_CARG_AREA;
                    int ID_PERS_ENTI_MN = dbe.PERSON_CONTRACT.Single(x => x.ID_CARG_AREA == ID_CARG_AREA_MN
                        && x.VIG_CONT == true && x.LAS_CONT == true).ID_PERS_ENTI.Value;

                    ID_PERS_OPER = ID_PERS_ENTI_MN;
                }
            }
            return ID_PERS_OPER;
        }

        public ActionResult ListTypeTraining() {

            var result = (from tt in dbe.TYPE_TRAINING.ToList()
                          select new { 
                            tt.ID_TYPE_TRAI,
                            NAM_TRAI = CapitalizeCadena(tt.NAM_TRAI),
                          });
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListManuTraining()
        {

            var result = (from tm in dbe.MANUFACTURER_TRAINING.ToList()
                          select new
                          {
                              tm.ID_TRAI_MANU,
                              NAM_MANU = CapitalizeCadena(tm.NAM_MANU),
                          });
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public string RequestApproval(int id = 0)
        {
            try
            {
                PERSON_TRAINING pt = dbe.PERSON_TRAINING.Find(id);
                try
                {
                    pt.DAT_REQU_APPR = DateTime.Now;
                    pt.STATUS = 1;
                    dbe.PERSON_TRAINING.Attach(pt);
                    dbe.Entry(pt).State = EntityState.Modified;
                    dbe.SaveChanges();
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

        public string ApprovalSupervisor(int id = 0) {
            try
            {
                PERSON_TRAINING pt = dbe.PERSON_TRAINING.Find(id);
                try
                {
                    //Obteniendo el ID del Management
                    int idpoer = GET_ID_PERS_OPER(Convert.ToInt32( pt.ID_PERS_SUPE));
                    if (idpoer != 0)
                    {
                        pt.ID_PERS_OPER = idpoer;
                    }

                    pt.DAT_APPR_SUPE = DateTime.Now;
                    dbe.PERSON_TRAINING.Attach(pt);
                    dbe.Entry(pt).State = EntityState.Modified;
                    dbe.SaveChanges();
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

        public string ApprovalManagement(int id = 0) {
            try
            {
                PERSON_TRAINING pt = dbe.PERSON_TRAINING.Find(id);
                try
                {
                    pt.STATUS = 2;
                    pt.DAT_APPR_OPER = DateTime.Now;
                    dbe.PERSON_TRAINING.Attach(pt);
                    dbe.Entry(pt).State = EntityState.Modified;
                    dbe.SaveChanges();
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

        public ActionResult RejectRequest(int id = 0) {
            ViewBag.ID_PERS_TRAI = id;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RejectTraining(PERSON_TRAINING ptra)
        {
            int sw = 0;
            int ID_PERS_TRAI = Convert.ToInt32(Request.Form["ID_PERS_TRAI"]);
            string REA_REJECT = Convert.ToString(Request.Form["REA_REJECT"]);

            if (REA_REJECT.Trim().Length == 0) { sw = 1; }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            PERSON_TRAINING pt = dbe.PERSON_TRAINING.Find(ID_PERS_TRAI);
            pt.REA_REJECT = REA_REJECT;
            pt.STATUS = 0;
            pt.DAT_REJECT = DateTime.Now;
            pt.ID_PERS_REJ = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            dbe.PERSON_TRAINING.Attach(pt);
            dbe.Entry(pt).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','Reject');}window.onload=init;</script>");
        }

        public string CapitalizeCadena(string cadena)
        {
            cadena = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cadena.ToLower());
            return cadena;
        }

    }
}
