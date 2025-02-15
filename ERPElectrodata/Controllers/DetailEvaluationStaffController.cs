using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;

namespace ERPElectrodata.Controllers
{
    public class DetailEvaluationStaffController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        // GET: /DetailEvaluationStaff/

        public ActionResult Index(int id)
        {   //----Parametros que recibe-----
            int ID_EVAL_STAF = id;

            ViewBag.idevalstaff = ID_EVAL_STAF;

           int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
           // int ID_PERS_ENTI = 234;

            var query = (from es in dbe.EVALUATION_STAFF.Where(ev=>ev.ID_EVAL_STAF==ID_EVAL_STAF).Where(e=>e.ID_PERS_ENTI_SUPE==ID_PERS_ENTI ||
                                    e.ID_PERS_ENTI_UEN==ID_PERS_ENTI||e.ID_PERS_ENTI_MAGE_GENE==ID_PERS_ENTI ||
                                    e.ID_PERS_ENTI_RRHH1==ID_PERS_ENTI || e.ID_PERS_ENTI_RRHH2==ID_PERS_ENTI ||
                                    e.ID_PERS_ENTI_CLIE==ID_PERS_ENTI||e.ID_PERS_ENTI==ID_PERS_ENTI)
                         select new
                         {
                             es.ID_PERS_ENTI,
                         }).ToList();

            //return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            if (query.Count() >= 1)
            {
                return View();
            }

            else
            {
                return Content("You do not have privileges");
            }
        }

        public ActionResult CommentByStaff(int id=0)
        {
            textInfo = cultureInfo.TextInfo;
           int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
           // int ID_PERS_ENTI = 234;

            //-------Estos Valores son lo que debe recibir.-----------

            int ID_EVAL_STAF = id;

            var query = (from des in dbe.DETAIL_EVALUATION_STAFF.Where(x => x.ID_EVAL_STAF == ID_EVAL_STAF).ToList()
                         join es in dbe.EVALUATION_STAFF.Where(e=>e.ID_PERS_ENTI_SUPE==ID_PERS_ENTI ||
                                    e.ID_PERS_ENTI_UEN==ID_PERS_ENTI||e.ID_PERS_ENTI_MAGE_GENE==ID_PERS_ENTI ||
                                    e.ID_PERS_ENTI_RRHH1==ID_PERS_ENTI || e.ID_PERS_ENTI_RRHH2==ID_PERS_ENTI ||
                                    e.ID_PERS_ENTI_CLIE == ID_PERS_ENTI || e.ID_PERS_ENTI == ID_PERS_ENTI) on des.ID_EVAL_STAF equals es.ID_EVAL_STAF
                         join pee in dbe.PERSON_ENTITY on es.ID_PERS_ENTI equals pee.ID_PERS_ENTI
                         join cee in dbe.CLASS_ENTITY on pee.ID_ENTI2 equals cee.ID_ENTI
                         join pe in dbe.PERSON_ENTITY on des.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                         select new
                         {   ID_EVAL_STAF=des.ID_EVAL_STAF,
                             NAM_STAFF = textInfo.ToTitleCase((ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower() + " " + ce.MOT_NAME.ToLower())),
                             CREATED_DETA = String.Format("{0:G}",des.CREATED_DETA),
                             COMMENT = des.COM_DETA_EVAL_STAF,
                             PHOTO = pe.ID_FOTO,
                             NAM_STAFF_EVAL = textInfo.ToTitleCase((cee.FIR_NAME.ToLower() + " " + cee.LAS_NAME.ToLower() + " " + cee.MOT_NAME.ToLower())),
                             CREATED_DETAx = des.CREATED_DETA,
                         }).ToList().OrderByDescending(x=>x.CREATED_DETAx);

            if (query.Count() >= 1)
            {
                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Content("No Comment");
            }
        }

        public ActionResult NewComment(int id=0)
        {
            ViewBag.idevalstaf = id;

            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult SaveCommentEvaluation(DETAIL_EVALUATION_STAFF details_evaluation)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_EVAL_STAF = Convert.ToInt32(Request.Params["ID_EVAL_STAF"]);
            DateTime CREATED_DETA = DateTime.Now;


            if (String.IsNullOrEmpty(details_evaluation.COM_DETA_EVAL_STAF))
            {
             //return Content("<script>alert('Ingrese Su Comentario');</script>");
                return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR','You have not made ​​any comment');}window.onload=init;</script>");
            }
            else { 


            details_evaluation.ID_PERS_ENTI = ID_PERS_ENTI;
            details_evaluation.UserId = UserId;
            details_evaluation.ID_EVAL_STAF=ID_EVAL_STAF;
            details_evaluation.CREATED_DETA=CREATED_DETA;

            dbe.DETAIL_EVALUATION_STAFF.Add(details_evaluation);
            dbe.SaveChanges();
            
          
            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','Saved Succesfully...');}window.onload=init;</script>");
            }
        }



        public IView idevalstaff { get; set; }
    }
}
