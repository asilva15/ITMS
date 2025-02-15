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
    public class DocumentControlDetailController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        //
        // GET: /DocumentControlDetail/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /DocumentControlDetail/ByDC
        public ActionResult ByDC(int id = 0)
        {
            try
            {
                var query = dbc.DOCUMENT_CONTROL_DETAIL.Where(x=>x.ID_DOCU_CONT == id);

                var result = (from dcd in query.ToList()
                              select new {
                                  CODART = dcd.CODART,
                                  DESART = dcd.DESART,
                                  PRECOS = dcd.PRECOS,
                                  CANTOT = dcd.CANTOT,
                              });

                return Json(new {Data=result,Count=query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "", Count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Work(int id = 0)
        {
            try
            {
                textInfo = cultureInfo.TextInfo;

                int skip = Convert.ToInt32(Request.Params["skip"].ToString());
                int take = Convert.ToInt32(Request.Params["take"].ToString());
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var listuser = (from lu in dbe.ACCOUNT_ENTITY
                                join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                where lu.ID_ACCO == ID_ACCO
                                select new
                                {
                                    lu.ID_PERS_ENTI,
                                    Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                    ce.UserId,
                                    ID_FOTO = pe.ID_FOTO,
                                }).ToList();

                var query = dbc.DOCUMENT_CONTROL_WORK.Where(x => x.ID_DOCU_CONT == id);

                var result = (from dcw in query.OrderByDescending(x=>x.CREATED).ToList()
                              join pec in listuser on dcw.UserId equals pec.UserId
                              join pe in listuser on dcw.ID_PERS_ENTI equals pe.ID_PERS_ENTI into lpe
                              from xpe in lpe.DefaultIfEmpty()
                              join dca in dbc.DOCUMENT_CONTROL_ACTION on dcw.ID_DOCU_CONT_ACTI equals dca.ID_DOCU_CONT_ACTIV
                              select new {
                                  FROM_PERSON = textInfo.ToTitleCase(pec.Client.ToLower()),
                                  TO_PERSON = xpe == null ? String.Empty : textInfo.ToTitleCase(xpe.Client.ToLower()),
                                CREATED = String.Format("{0:d}", dcw.CREATED) + " " + String.Format("{0:HH:mm}", dcw.CREATED),
                                ACTION = dca.NAM_DOCU_CONT_ACTI,
                                FROM_FOTO = pec.ID_FOTO,
                              });

                return Json(new { Data = result,Count= query.Count()},JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "No Result", Count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Acces(int id = 0)
        {
            textInfo = cultureInfo.TextInfo;

            var query = dbc.DOCUMENT_CONTROL_RECEIVER.Where(x=>x.ID_DOCU_CONT == id).ToList();

            var result = (from dcr in query
                          join pe in dbe.PERSON_ENTITY on dcr.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          join c in dbe.CHARTs on dcr.ID_CHAR equals c.ID_CHAR into lc
                          from xc in lc.DefaultIfEmpty()
                          join nc in dbe.NAME_CHART on (xc != null ? xc.ID_NAM_CHAR : null) equals nc.ID_NAM_CHAR into lnc
                          from xnc in lnc.DefaultIfEmpty()
                          select new {
                              PERSON = textInfo.ToTitleCase(ce.FIR_NAME.ToLower()) + " " + textInfo.ToTitleCase(ce.LAS_NAME.ToLower()),
                              AREA = (xnc != null ? xnc.NAM_CHAR_ENGL : String.Empty),
                              CREATED = String.Format("{0:d}", dcr.CREATED) + " " + String.Format("{0:HH:mm}", dcr.CREATED),
                              FROM_FOTO = pe.ID_FOTO
                          });

            return Json(new { Data = result },JsonRequestBehavior.AllowGet);
        }
    }
}
