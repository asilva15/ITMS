using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class QuestionController : Controller
    {
        public CoreEntities db = new CoreEntities();
       
        // GET: /Question/

        public ActionResult Index()
        {
            List<QUESTION> listquestion = new List<QUESTION>();
            //listquestion = db.QUESTIONs.ToList();
            return View(listquestion);
        }

 //------------------CREATE---------------------------------------       
        public ActionResult Create()
        {
            var newquestion = new QUESTION();          

            return View(newquestion);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCreate(QUESTION q)
        {
            
            //db.QUESTIONs.Add(q);
            //db.SaveChanges();

           return Redirect("Index");            
              
        }
             
 //---------------------------EDIT----------------------------------------      
        public ActionResult Edit(int id =0)
        {
            var editquestion = new QUESTION();
            //editquestion = db.QUESTIONs.Single(q => q.ID_QUES == id);

            return View(editquestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEdit(QUESTION q)
        {
            var quest = new QUESTION();
             //quest = db.QUESTIONs.Single(qe => qe.ID_QUES==q.ID_QUES);
                     
            quest.NAM_QUES = q.NAM_QUES;
            //quest.DES_QUES = q.DES_QUES;
            //quest.STA_QUES = q.STA_QUES;
            
            UpdateModel(quest);
            db.SaveChanges();

            return Redirect("Index");

        }
        public ActionResult Delete(int id = 0)
        {
            var quest = new QUESTION();
            //quest = db.QUESTIONs.Single(qe =>qe.ID_QUES==id);
            return View(quest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteQuestion(QUESTION q)
        {
            var questdel = new QUESTION();
            //questdel = db.QUESTIONs.Single(qe => qe.ID_QUES == q.ID_QUES);

            //db.QUESTIONs.Remove(questdel);
            db.SaveChanges();

            return Redirect("Index");

        }
    }
}
