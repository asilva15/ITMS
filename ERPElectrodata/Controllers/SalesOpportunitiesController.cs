using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using System.Globalization;
using System.Threading;

namespace ERPElectrodata.Controllers
{
    [Authorize]
    public class SalesOpportunitiesController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public ActionResult PreSaleReport()
        {
            return View();
        }

        public ActionResult PreSaleReportData()
        {
            try
            {
                DateTime since_date = Convert.ToDateTime(Request.Params["SIN_DATE"]);
                DateTime to_date = Convert.ToDateTime(Request.Params["TO_DATE"]);

                textInfo = cultureInfo.TextInfo;

                var status = (from x in dbc.STATUS_SALES_OPPORTUNITY.ToList()
                              select new
                              {
                                  ID_STAT_SALE_OPPO = x.ID_STAT_SALE_OPPO,
                                  NAM_STAT_SALE_OPPO = textInfo.ToTitleCase(x.NAM_STAT_SALE_OPPO.ToLower()),
                                  color = x.COL_STAT_SALE_OPPO
                              });

                var data = dbc.SO_PRES_REP(since_date, to_date).ToList();

                var vendors = (from d in data
                               group d by new { d.ID_PERS_ENTI, d.EMPLOYEE } into g
                               select new
                               {
                                   id = g.Key.ID_PERS_ENTI,
                                   name = g.Key.EMPLOYEE,
                                   count = g.Sum(x => x.ID_STAT_SALE_OPPO == 8 ? x.QTY:0)
                               }).OrderByDescending(x=>x.count).ThenBy(x=>x.name);

                var groups = (from x in dbc.STATUS_SALES_OPPORTUNITY.ToList()
                              group x by new { x.ID_GROU, x.NAM_GROU, x.COL_GROU } into g
                              select new
                              {
                                  id = g.Key.ID_GROU,
                                  name = g.Key.NAM_GROU,
                                  color = g.Key.COL_GROU
                              });

                var datagroups = (from d in data
                                  group d by new { d.ID_GROUP, d.ID_PERS_ENTI } into g
                                  select new
                                  {
                                      id = g.Key.ID_GROUP,
                                      ID_PERS_ENTI = g.Key.ID_PERS_ENTI,
                                      qty = g.Sum(x => x.QTY),
                                      ammount = g.Sum(x => x.AMMOUNT)
                                  });

                return Json(new { status = status, data = data, vendors = vendors, groups = groups, datagroups = datagroups }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Content("Error");
            }
        }
        //
        // GET: /SalesOpportunities/
        public ActionResult Details(int id)
        {
            try
            {
                textInfo = cultureInfo.TextInfo;
                var person = dbe.PERSON_ENTITY.Where(x => x.ID_ENTI2 != null)
                    .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, y => y.ID_ENTI, (x, y) => new { x.ID_ENTI1, y.FIR_NAME, y.LAS_NAME, y.UserId, x.ID_PERS_ENTI,x.ID_FOTO });

                var cia = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1);

                var query = dbc.SALES_OPPORTUNITY.Where(x => x.ID_SALE_OPPO == id);

                var zz = query.ToList();

                var result = (from x in query.ToList().OrderBy(x => x.CREATE_DATE)
                              join s in dbc.STATUS_SALES_OPPORTUNITY on x.ID_STAT_SALE_OPPO_END equals s.ID_STAT_SALE_OPPO
                              join m in dbc.MANUFACTURERs on x.ID_MANU equals m.ID_MANU
                              join p in person.ToList() on x.ID_PERS_ENTI equals p.ID_PERS_ENTI
                              where x.ID_COMP == p.ID_ENTI1
                              join o in person.ToList() on x.UserId equals o.UserId
                              join c in cia.ToList() on x.ID_COMP equals c.ID_ENTI
                              select new
                              {
                                  CREATE_DATE = String.Format("{0:d}", x.CREATE_DATE) + " " + String.Format("{0:HH:mm}", x.CREATE_DATE),
                                  MODIFIED_DATE = String.Format("{0:d}", x.MODIFIED_DATE) + " " + String.Format("{0:HH:mm}", x.MODIFIED_DATE),
                                  CLO_DATE = String.Format("{0:d}", x.CLO_DATE),
                                  NAM_STAT_SALE_OPPO = textInfo.ToTitleCase(s.NAM_STAT_SALE_OPPO.ToLower()),
                                  TIT_SALE_OPPO = x.TIT_SALE_OPPO,
                                  COD_SALE_OPPO = x.COD_SALE_OPPO,
                                  AMO_SALE_OPPO = x.AMO_SALE_OPPO.Value.ToString("N2"),
                                  MAR_SALE_OPPO = x.MAR_SALE_OPPO,
                                  OWNER = textInfo.ToTitleCase(o.FIR_NAME.ToLower() + " " + o.LAS_NAME.ToLower()),
                                  CONTACT = textInfo.ToTitleCase(p.FIR_NAME.ToLower() + " " + p.LAS_NAME.ToLower()),
                                  CIA = textInfo.ToTitleCase(c.COM_NAME.ToLower()),
                                  NAM_MANU = textInfo.ToTitleCase(m.NAM_MANU.ToLower()),
                                  ID_FOTO = Convert.ToString(o.ID_FOTO) + ".jpg",
                                  SUM_SALE_OPPO = HttpUtility.HtmlDecode(x.SUM_SALE_OPPO),
                              });

                var xy = result.First();

                ViewBag.CREATE_DATE = xy.CREATE_DATE;
                ViewBag.MODIFIED_DATE = xy.MODIFIED_DATE;
                ViewBag.CLO_DATE = xy.CLO_DATE;
                ViewBag.NAM_STAT_SALE_OPPO = xy.NAM_STAT_SALE_OPPO;
                ViewBag.TIT_SALE_OPPO = xy.TIT_SALE_OPPO;
                ViewBag.COD_SALE_OPPO = xy.COD_SALE_OPPO;
                ViewBag.AMO_SALE_OPPO = xy.AMO_SALE_OPPO;
                ViewBag.MAR_SALE_OPPO = xy.MAR_SALE_OPPO;
                ViewBag.OWNER = xy.OWNER;
                ViewBag.CONTACT = xy.CONTACT;
                ViewBag.CIA = xy.CIA;
                ViewBag.NAM_MANU = xy.NAM_MANU;
                ViewBag.ID_FOTO = xy.ID_FOTO;
                ViewBag.SUM_SALE_OPPO = xy.SUM_SALE_OPPO;

                ViewBag.DataS = xy;

                return View(result);
            }
            catch
            {
                return Content("Please Close Session");
            }
        }

        public ActionResult List()
        {
            try
            {
                int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);

                textInfo = cultureInfo.TextInfo;

                var person = dbe.PERSON_ENTITY.Where(x => x.ID_ENTI2 != null)
                    .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, y => y.ID_ENTI, (x, y) => new { x.ID_ENTI1,y.FIR_NAME, y.LAS_NAME,y.UserId,x.ID_PERS_ENTI });

                var cia = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1);

                var query = dbc.SALES_OPPORTUNITY.Where(x => x.ID_STAT_SALE_OPPO_END == ID_STAT);

                var result = (from x in query.ToList().OrderBy(x=>x.CREATE_DATE)
                              join s in dbc.STATUS_SALES_OPPORTUNITY on x.ID_STAT_SALE_OPPO_END equals s.ID_STAT_SALE_OPPO
                              join m in dbc.MANUFACTURERs on x.ID_MANU equals m.ID_MANU
                              join p in person.ToList() on x.ID_PERS_ENTI equals p.ID_PERS_ENTI
                              where x.ID_COMP == p.ID_ENTI1
                              join o in person.ToList() on x.UserId equals o.UserId
                              join c in cia.ToList() on x.ID_COMP equals c.ID_ENTI
                              select new {
                                  ID_SALE_OPPO = x.ID_SALE_OPPO,
                                  CREATE_DATE = String.Format("{0:d}", x.CREATE_DATE) + " " + String.Format("{0:HH:mm}", x.CREATE_DATE),
                                  MODIFIED_DATE = String.Format("{0:d}", x.MODIFIED_DATE) + " " + String.Format("{0:HH:mm}", x.MODIFIED_DATE),
                                  CLO_DATE = String.Format("{0:d}", x.CLO_DATE),
                                  NAM_STAT_SALE_OPPO = textInfo.ToTitleCase(s.NAM_STAT_SALE_OPPO.ToLower()),
                                  TIT_SALE_OPPO = x.TIT_SALE_OPPO,
                                  COD_SALE_OPPO = x.COD_SALE_OPPO,
                                  AMO_SALE_OPPO = x.AMO_SALE_OPPO.Value.ToString("N2"),
                                  MAR_SALE_OPPO = x.MAR_SALE_OPPO,
                                  OWNER = textInfo.ToTitleCase(o.FIR_NAME.ToLower() + " " + o.LAS_NAME.ToLower()),
                                  CONTACT = textInfo.ToTitleCase(p.FIR_NAME.ToLower() + " " + p.LAS_NAME.ToLower()),
                                  CIA = c.COM_NAME,
                                  NAM_MANU = textInfo.ToTitleCase(m.NAM_MANU.ToLower()),
                              });
                return Json(new { Data = result,Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "" }, JsonRequestBehavior.AllowGet);
            }

            
            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SALES_OPPORTUNITY saleOppo,IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                int UserId = 0;

                UserId = Convert.ToInt32(Session["UserId"]);
                if (UserId == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadSalesOp) top.uploadSalesOp('ERROR','" + "Please, Close Your Session" + "');}window.onload=init;</script>");
                }

                if (ModelState.IsValid)
                {
                    saleOppo.CREATE_DATE = DateTime.Now;
                    saleOppo.MODIFIED_DATE = DateTime.Now;
                    saleOppo.ID_STAT_SALE_OPPO_END = saleOppo.ID_STAT_SALE_OPPO.Value;
                    saleOppo.UserId = UserId;
                    dbc.SALES_OPPORTUNITY.Add(saleOppo);
                    dbc.SaveChanges();

                    int ID_SALE_OPPO = Convert.ToInt32(saleOppo.ID_SALE_OPPO);
                    dbc.Entry(saleOppo).State = EntityState.Detached;

                    string code = Convert.ToString(dbc.SALES_OPPORTUNITY.Single(t => t.ID_SALE_OPPO == ID_SALE_OPPO).COD_SALE_OPPO);

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadSalesOp) top.uploadSalesOp('OK','" + code + "');}window.onload=init;</script>");

                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadSalesOp) top.uploadSalesOp('ERROR','" + "Completed Missing Information" + "');}window.onload=init;</script>");
                }
            }
            catch(Exception e)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadSalesOp) top.uploadSalesOp('ERRROR','" + "Error Cantact Support" + "');}window.onload=init;</script>");
            }

        }

    }
}
