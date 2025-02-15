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
    public class AttachBeneficiaryController : Controller
    {
        public EntityEntities dbe = new EntityEntities();

        // GET: /AttachBeneficiary/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAttachBeneficiary(IEnumerable<HttpPostedFileBase> files)
        {

            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
            
            try
            {
                var ID_TYPE_ATTA_BENEs = Convert.ToString(Request.Params["ID_TYPE_ATTA_BENE"]);
                var KEY_ATTA_BENE = Convert.ToString(Request.Params["KEY_ATTA_BENE"]);
                int ID_TYPE_ATTA_BENE = 0;

                string EDI = Convert.ToString(Request.Params["EDI"]);

                 var ATTA = new ATTACHED_BENEFICIARY();

                if (!String.IsNullOrEmpty(ID_TYPE_ATTA_BENEs))
                {
                    ID_TYPE_ATTA_BENE = Convert.ToInt32(ID_TYPE_ATTA_BENEs);
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            string name_atta = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName).ToLower();
                            int repetido = -1;

                            if(EDI=="E")
                            {
                                int ID_BENE = Convert.ToInt32(Request.Params["ID_BENE"]);
                                repetido = dbe.ATTACHED_BENEFICIARY
                               .Where(x => x.DEL_ATTA_BENE == false)
                               .Where(x => x.ID_BENE == ID_BENE)                               
                               //.Where(x => x.ID_TYPE_ATTA_BENE == ID_TYPE_ATTA_BENE).Count();
                                .Where(x => x.NAM_ATTA == name_atta)
                                .Where(x => x.EXT_ATTA == extension).Count();

                                ATTA.ID_BENE=ID_BENE;
                            }
                            else
                            {
                                repetido = dbe.ATTACHED_BENEFICIARY
                               .Where(x => x.DEL_ATTA_BENE == false)
                               .Where(x => x.KEY_ATTA_BENE == KEY_ATTA_BENE)
                               .Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                               //.Where(x => x.ID_TYPE_ATTA_BENE == ID_TYPE_ATTA_BENE).Count();
                                .Where(x => x.NAM_ATTA == name_atta)
                                .Where(x => x.EXT_ATTA == extension).Count();
                            }                               

                            if (repetido == 0 && extension==".pdf")
                            {
                                ATTA.NAM_ATTA = name_atta;
                                ATTA.EXT_ATTA = extension;                               
                                ATTA.CREATE_ATTA = DateTime.Now;
                                ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                                ATTA.UserId = UserId;
                                ATTA.ID_TYPE_ATTA_BENE = ID_TYPE_ATTA_BENE;
                                ATTA.KEY_ATTA_BENE = KEY_ATTA_BENE;
                                ATTA.DEL_ATTA_BENE = false;
                                dbe.ATTACHED_BENEFICIARY.Add(ATTA);
                                dbe.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/Talent/BeneficiaryDNI/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                            }

                            else
                            {
                                return Content("Filename Exist");
                            }
                        }
                        catch
                        {
                            return Content("ERROR");
                        }
                    }
                }

                return Content(ID_TYPE_ATTA_BENEs);
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAttachBeneficiary(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var KEY_ATTA_BENE = Convert.ToString(Request.Params["KEY_ATTA_BENE"]);

                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
                int ID_TYPE_ATTA_BENE = Convert.ToInt32(Request.Params["ID_TYPE_ATTA_BENE"]);

                string filenames = Convert.ToString(Request.Params["fileNames"]);

                if (filenames != null)
                {
                    
                    try
                    {
                        string name_atta = Path.GetFileNameWithoutExtension(filenames);
                        string extension = Path.GetExtension(filenames);

                        var ATTA = dbe.ATTACHED_BENEFICIARY
                                  .Where(x => x.KEY_ATTA_BENE == KEY_ATTA_BENE)        
                                  .Where(x=>x.ID_PERS_ENTI==ID_PERS_ENTI)
                                  .Where(x => x.ID_TYPE_ATTA_BENE == ID_TYPE_ATTA_BENE)
                                  .Where(x=>x.EXT_ATTA==extension)
                                  .Single(x => x.NAM_ATTA == name_atta);

                        ATTA.DEL_ATTA_BENE = true;

                        try
                        {
                            
                            if (System.IO.File.Exists(Server.MapPath("~/Adjuntos/Talent/BeneficiaryDNI/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA)))
                            {
                                System.IO.File.Delete(Server.MapPath("~/Adjuntos/Talent/BeneficiaryDNI/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA));
                            }
                        }
                        catch
                        {

                        }

                        dbe.Entry(ATTA).State = EntityState.Modified;                       
                        dbe.SaveChanges();
                        
                    }
                    catch
                    {

                    }
                   
                }

                return Content("");
            }
            catch
            {
                var xx = "";
                return Content(xx);
            }

        } 


    }
}
