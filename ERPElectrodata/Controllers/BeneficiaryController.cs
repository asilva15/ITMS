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
    public class BeneficiaryController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Beneficiary/

        // POST: /Asset/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, BENEFICIARY bene)
        {
            //int sw = 0;
           
            string Error = "";
            int UserId = Convert.ToInt32(Session["UserId"]);

            string ID_KINDRED = Convert.ToString(Request.Params["ID_KINDRED"].ToString());
            string FIR_NAME = Convert.ToString(Request.Params["FIR_NAME"].ToString()).ToUpper();
            string LAS_NAME = Convert.ToString(Request.Params["LAS_NAME"].ToString()).ToUpper();
            string NUMB_DNI = Convert.ToString(Request.Params["NUMB_DNI"].ToString()).ToUpper();
            string DATE_BIRTH = Convert.ToString(Request.Params["DATE_BIRTH"].ToString());
            int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);

            string KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);

            if (ID_KINDRED.Trim().Length==0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                  "if(top.mensaje) top.mensaje('ERROR','You must select a Kindred');}window.onload=init;</script>");
           }
            else if (FIR_NAME.Trim().Length == 0) {

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','Enter the First Name of Beneficiary');}window.onload=init;</script>");
                
               }
            else if (LAS_NAME.Trim().Length == 0) {

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','Enter the Last Name of Beneficiary');}window.onload=init;</script>");

                 }
            else if (NUMB_DNI.Trim().Length == 0) {

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','Enter the Identification Number of Beneficiary');}window.onload=init;</script>");

               }
            else if (DATE_BIRTH == "") {
                 
                return Content("<script type='text/javascript'> function init() {" +
                 "if(top.mensaje) top.mensaje('ERROR','Enter Date of Birth of Beneficiary');}window.onload=init;</script>");  
            }
            else {    

            try
            {
                //var queryDNI = dbe.ATTACHED_BENEFICIARY.SingleOrDefault(x => x.KEY_ATTA_BENE == KEY_ATTA && x.ID_TYPE_ATTA_BENE == 1 && x.DEL_ATTA_BENE == false);

                //if (queryDNI != null)
                //{
                //    bene.ATT_DNI = true;
                //}
                //else
                //{
                //    bene.ATT_DNI = false;
                //}


                //var queryCE = dbe.ATTACHED_BENEFICIARY.SingleOrDefault(x => x.KEY_ATTA_BENE == KEY_ATTA && x.ID_TYPE_ATTA_BENE == 2 && x.DEL_ATTA_BENE == false);

                //if (queryCE != null)
                //{
                //    bene.ATT_EVID_STUD = true;
                //}
                //else
                //{
                //    bene.ATT_EVID_STUD = false;
                //}

                var query = dbe.ATTACHED_BENEFICIARY.Where(x => x.KEY_ATTA_BENE == KEY_ATTA && x.ID_PERS_ENTI == ID_PERS_ENTI && x.DEL_ATTA_BENE == false).ToList();


                DateTime HB = Convert.ToDateTime(DATE_BIRTH);

                bene.ID_PERS_ENTI = ID_PERS_ENTI;             
                bene.CREATED = DateTime.Now;
                bene.DATE_BRITH = HB;

                dbe.BENEFICIARies.Add(bene);
                dbe.SaveChanges();

                foreach(var x in query)
                {
                    x.ID_BENE = bene.ID_BENE;
                    dbe.ATTACHED_BENEFICIARY.Attach(x);
                    dbe.Entry(x).State = EntityState.Modified;
                    dbe.SaveChanges();
                }

                //if (queryDNI != null)
                //{
                //    queryDNI.ID_BENE = bene.ID_BENE;
                //    dbe.Entry(queryDNI).State = EntityState.Modified;
                //    dbe.SaveChanges();
                //}

                //if (queryCE != null)
                //{
                //    queryCE.ID_BENE = bene.ID_BENE;
                //    dbe.Entry(queryCE).State = EntityState.Modified;
                //    dbe.SaveChanges();

                //}
                                                
            }
            catch (Exception e)
            {
                Error = (e.InnerException == null ? e.Message : e.InnerException.Message);

                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.mensaje) top.mensaje('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
            "if(top.mensaje) top.mensaje('OK','The New Beneficiary has been Saved Successfully');}window.onload=init;</script>");
         }
        }
        public ActionResult EditBeneficiary(int id = 0)
        {

            //Recibira el ID_PERS_ENTI del que depende el beneficiary.           

            ViewBag.ID_PERS_ENTI = id;
            ViewBag.TODAY = String.Format("{0:d}", DateTime.Now);

            ViewBag.UploadFile = "AB" + Convert.ToString(DateTime.Now.Ticks);

            return View();
        }

        public ActionResult AdjuntarDocumenteBeneficiary(string fileUpload)
        {

            //Recibira el ID_PERS_ENTI del que depende el beneficiary.           

            ViewBag.FileUpload = fileUpload;
            

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditBeneficiary(IEnumerable<HttpPostedFileBase> files, BENEFICIARY bene)
        {
            //int sw = 0;

            string Error = "";
            int UserId = Convert.ToInt32(Session["UserId"]);

            string ID_KINDRED = Convert.ToString(Request.Params["ID_KINDRED"].ToString());
            string FIR_NAME = Convert.ToString(Request.Params["New_FIR_NAME"].ToString()).ToUpper();
            string LAS_NAME = Convert.ToString(Request.Params["New_LAS_NAME"].ToString()).ToUpper();
            string NUMB_DNI = Convert.ToString(Request.Params["New_NUMB_DNI"].ToString()).ToUpper();
            string DATE_BIRTH = Convert.ToString(Request.Params["New_DATE_BIRTH"].ToString());
            int ID_BENE = Convert.ToInt32(Request.Form["ID_BENI_EDIT"]);

            //string KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);

            if (ID_KINDRED.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                  "if(top.uploadDone) top.uploadDone('ERROR','You must select a Kindred');}window.onload=init;</script>");
            }
            else if (FIR_NAME.Trim().Length == 0)
            {

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Enter the First Name of Beneficiary');}window.onload=init;</script>");

            }
            else if (LAS_NAME.Trim().Length == 0)
            {

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Enter the Last Name of Beneficiary');}window.onload=init;</script>");

            }
            else if (NUMB_DNI.Trim().Length == 0)
            {

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','Enter the Identification Number of Beneficiary');}window.onload=init;</script>");

            }
            else if (DATE_BIRTH == "")
            {

                return Content("<script type='text/javascript'> function init() {" +
                 "if(top.uploadDone) top.uploadDone('ERROR','Enter Date of Birth of Beneficiary');}window.onload=init;</script>");
            }
            else
            {

                try
                {
                    //var queryDNI = dbe.ATTACHED_BENEFICIARY.FirstOrDefault(x => x.ID_BENE == ID_BENE && x.ID_TYPE_ATTA_BENE == 1 && x.DEL_ATTA_BENE == false);

                    //if (queryDNI != null)
                    //{
                    //    bene.ATT_DNI = true;
                    //}
                    //else
                    //{
                    //    bene.ATT_DNI = false;
                    //}


                    //var queryCE = dbe.ATTACHED_BENEFICIARY.FirstOrDefault(x => x.ID_BENE == ID_BENE && x.ID_TYPE_ATTA_BENE == 2 && x.DEL_ATTA_BENE == false);

                    //if (queryCE != null)
                    //{
                    //    bene.ATT_EVID_STUD = true;
                    //}
                    //else
                    //{
                    //    bene.ATT_EVID_STUD = false;
                    //}

                    //var query = dbe.ATTACHED_BENEFICIARY.Where(x => x.ID_BENE == ID_BENE  && x.DEL_ATTA_BENE == false).ToList();

                    DateTime HB = Convert.ToDateTime(DATE_BIRTH);

                    var b = dbe.BENEFICIARies.Single(x => x.ID_BENE ==ID_BENE);

                    //bene.ID_PERS_ENTI = ID_PERS_ENTI;
                    //bene.CREATED = DateTime.Now;
                    b.ID_KINDRED = bene.ID_KINDRED;
                    b.FIR_NAME = FIR_NAME;
                    b.SEC_NAME = Convert.ToString(Request.Params["New_SEC_NAME"].ToString()).ToUpper();
                    b.LAS_NAME = LAS_NAME;
                    b.MOT_NAME = Convert.ToString(Request.Params["New_MOT_NAME"].ToString()).ToUpper();
                    b.DATE_BRITH = HB;
                    b.NUMB_DNI = NUMB_DNI;
                    b.MODIFIED = DateTime.Now;    
                    dbe.BENEFICIARies.Attach(b);
                    dbe.Entry(b).State = EntityState.Modified;
                    dbe.SaveChanges();

                    //foreach (var x in query)
                    //{
                    //    x.ID_BENE = ID_BENE;
                    //    dbe.ATTACHED_BENEFICIARY.Attach(x);
                    //    dbe.Entry(x).State = EntityState.Modified;
                    //    dbe.SaveChanges();
                    //}


                    //if (queryDNI != null)
                    //{
                    //    queryDNI.ID_BENE = b.ID_BENE;
                    //    dbe.Entry(queryDNI).State = EntityState.Modified;
                    //    dbe.SaveChanges();
                    //}

                    //if (queryCE != null)
                    //{
                    //    queryCE.ID_BENE = b.ID_BENE;
                    //    dbe.Entry(queryCE).State = EntityState.Modified;
                    //    dbe.SaveChanges();

                    //}

                }
                catch (Exception e)
                {
                    Error = (e.InnerException == null ? e.Message : e.InnerException.Message);

                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
                }

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','The Beneficiary has been Saved Successfully');}window.onload=init;</script>");
            }
        }

        public string DeleteBeneficiary(int id = 0)
        {
            try
            {
                BENEFICIARY atta = dbe.BENEFICIARies.Find(id);
                try
                {
                    string arc = atta.FIR_NAME + atta.LAS_NAME + "_" + Convert.ToString(atta.ID_BENE) + ".pdf";
                    dbe.BENEFICIARies.Remove(atta);
                    dbe.SaveChanges();

                    //Eliminando
                    System.IO.File.Delete(Server.MapPath("~/Adjuntos/Talent/BeneficiaryDNI/" + arc));
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

        public ActionResult ListTypeAttaBene()
        {
            var query = (from x in dbe.TYPE_ATTACHED_BENEFICIARY
                         select new
                         {
                             x.ID_TYPE_ATTA_BENE,
                             x.NAM_ATTA_BENE,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

         
       //public ActionResult VieAttaBene(int id)
       // {
       //     return View();
       // }

        public ActionResult ListAttaBene(int id)
        {
            try
            {
                var query = (from ab in dbe.ATTACHED_BENEFICIARY.Where(a => a.ID_BENE == id)
                             .Where(a => a.DEL_ATTA_BENE == false || a.DEL_ATTA_BENE == null)
                             join tab in dbe.TYPE_ATTACHED_BENEFICIARY on ab.ID_TYPE_ATTA_BENE equals tab.ID_TYPE_ATTA_BENE
                             select new
                             {
                                 ID_ATTA = ab.ID_ATTA,
                                 NAM_ATTA = ab.NAM_ATTA,
                                 EXT_ATTA = ab.EXT_ATTA,
                                 NAM_TYPE_ATTA = tab.NAM_ATTA_BENE,
                             }).OrderBy(x=>x.NAM_TYPE_ATTA);   

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "", Count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public string DeleteAttaBene(int id)
        {
            
              ATTACHED_BENEFICIARY atta = dbe.ATTACHED_BENEFICIARY.Find(id);
                try
                {
                    atta.DEL_ATTA_BENE = true;

                    dbe.ATTACHED_BENEFICIARY.Attach(atta);
                    dbe.Entry(atta).State = EntityState.Modified;
                    dbe.SaveChanges();
                

                }
                catch (Exception)
                {
                    return "ERROR";
                }
                return "OK";
            }
           
        


    }
}
