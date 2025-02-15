using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class QuestionPersonEntityController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /QuestionPersonEntity/Index

        public ActionResult Index(int id = 0)
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
                ViewBag.id = ID_PERS_ENTI;
                return View();
            }
            catch
            {
                return Content("Please Close Session");
            }
            
        }
        public ActionResult QuestionBySurvey(int id = 0)
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

                //var query = dbe.QUESTION_GROUP.Where(x => x.ID_SURVEY == id)
                //.Join(dbe-, x => x.ID_QUES_GROU, y => y.ID_QUES_GROU, (x, y) => new
                //{
                //    NAM_QUES_GROU = x.NAM_QUES_GROU,
                //    NAM_QUES = y.NAM_QUES,
                //    ID_QUES_TYPE = y.ID_QUES_TYPE,
                //    ID_QUES = y.ID_QUES
                //});
                var query = dbe.QUESTION_PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI);

                var result = (from x in query.ToList()
                              join q in dbe.QUESTIONs on x.ID_QUES equals q.ID_QUES
                              select new
                              {
                                  //NAM_QUES_GROU = x.NAM_QUES_GROU,
                                  ID_QUES_PERS_ENTI = x.ID_QUES_PERS_ENTI,
                                  NAM_QUES = q.NAM_QUES,
                                  ID_QUES_TYPE = q.ID_QUES_TYPE,
                                  ID_QUES = x.ID_QUES
                              });

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "0" },JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult Create()
        {
            int band = 0;
            //string fin = "OK";

            foreach (string var in Request.Params)
            {
                try
                {

                    string ID_QUES_PERS_ENTIs = Convert.ToString(var);
                    int ID_QUES_PERS_ENTI =0; //Convert.ToInt32(var);
                    Boolean verifica = false;

                    verifica = Int32.TryParse(ID_QUES_PERS_ENTIs, out ID_QUES_PERS_ENTI);

                    if(verifica){
                        string value = Convert.ToString(Request.Params[Convert.ToString(ID_QUES_PERS_ENTI)]);
                        var edit = dbe.QUESTION_PERSON_ENTITY.Single(x => x.ID_QUES_PERS_ENTI == ID_QUES_PERS_ENTI);
                        edit.VAL_QUES_PERS_ENTI = value;

                        var ques = dbe.QUESTIONs.Single(x => x.ID_QUES == edit.ID_QUES.Value);

                        if(ques.ID_QUES_TYPE == 1 && value == "0"){
                            band = band + 1;
                            break;
                        }

                        dbe.Entry(edit).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                    else{
                        break;
                    }

                }
                catch (Exception e)
                {

                    band = band + 1;
                }
            }

            if (band == 0)
            {
                return Content("OK");
            }
            else
            {
                return Content("Todos Los Campos Son Requeridos");
            }

            //return Content(fin);
        }

    }
}
