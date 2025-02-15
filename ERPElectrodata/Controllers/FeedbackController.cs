using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class FeedbackController : Controller
    {
        public CoreEntities db = new CoreEntities();

        public ActionResult Index(int id = 0)
        {

            ViewBag.idtick = id;

            return View(ViewBag);
        }

        public ActionResult ShowFeedback(int id = 0)
        {
            //var query = (from q in db.QUESTIONs
            //             join df in db.DETAILS_FEEDBACK on q.ID_QUES equals df.ID_QUES
            //             join f in db.FEEDBACKs on df.ID_FEED equals f.ID_FEED
            //             where f.ID_TICK == id
            //             select new
            //             {
            //                 ID = df.ID_DETA_FEED,
            //                 NAME = q.NAM_QUES,
            //                 IDFEED = f.ID_FEED,

            //             }).ToList();

            return Json(new {  }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFeedback()
        {
            int contador = Convert.ToInt32(Request.Params["contador"]);
            var updetafeed = new DETAILS_FEEDBACK();
            var updatesamary = new FEEDBACK();

            for (int i = 0; i < contador; i++)
            {
                int iddetafeed = Convert.ToInt32(Request.Params["iddetafeed" + i]);
                int valquest = Convert.ToInt32(Request.Params["valquest" + i]);

                updetafeed = db.DETAILS_FEEDBACK.Single(df => df.ID_DETA_FEED == iddetafeed);
                updetafeed.VAL_QUES = valquest;
                UpdateModel(updetafeed);
                db.SaveChanges();

            }

            string mensaje = Convert.ToString(Request.Params["mensaje"]);
            int idfeed = Convert.ToInt32(Request.Params["idfeed"]);

            updatesamary = db.FEEDBACKs.Single(fb=>fb.ID_FEED==idfeed);
            updatesamary.SUM_FEED = mensaje;
            UpdateModel(updatesamary);
            db.SaveChanges();

            return View();
        }


    }
}
