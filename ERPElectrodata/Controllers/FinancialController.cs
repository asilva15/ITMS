using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
//using DotNetOpenAuth.AspNet;
//using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class FinancialController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /Financial/
        [Authorize]
        public ActionResult Index()
        {

            try
            {
                int index = 0;

                if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
                {
                    index = index + 1;
                }

                if (index > 0)
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    Session["MAIN"] = "mp5";
                    var ACCO = dbc.ACCOUNTs.Single(a => a.ID_ACCO == ID_ACCO);
                    var PARA = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO);
                    ViewBag.LAT_ACCO = PARA.Single(x => x.ID_PARA == 1).VAL_ACCO_PARA;
                    ViewBag.LON_ACCO = PARA.Single(x => x.ID_PARA == 2).VAL_ACCO_PARA;
                    ViewBag.STAFF = PARA.Single(x => x.ID_PARA == 3).VAL_ACCO_PARA;
                    ViewBag.SITES = PARA.Single(x => x.ID_PARA == 4).VAL_ACCO_PARA;
                    ViewBag.SERVICE = PARA.Single(x => x.ID_PARA == 5).VAL_ACCO_PARA;
                    ViewBag.NAM_ACCO = ACCO.NAM_ACCO;
                    ViewBag.SIM_MONE_ACCO = ACCO.SIM_MONE_ACCO;
                    ViewBag.MONEY_NAME = PARA.Single(x => x.ID_PARA == 16).VAL_ACCO_PARA;

                    var query = dbc.TRANSACTIONs.Where(t => t.ID_ACCO == ID_ACCO)
                            .Join(dbc.CATEGORY_TRANSACTION, t => t.ID_CATE_TRAN, ct => ct.ID_CATE_TRAN, (t, ct) => new { t.AMO_TRANS, ct.ID_TYPE_TRAN })
                            .Join(dbc.TYPE_TRANSACTION, x => x.ID_TYPE_TRAN, tt => tt.ID_TYPE_TRAN, (x, tt) => new { x.AMO_TRANS, tt.FAC_TYPE_TRAN, x.ID_TYPE_TRAN });

                    /*var Personas = db.ACCOUNT_CLIENT.Where(ac => ac.ID_ACCO == ID_ACCO)
                        .Join(db.CLIENTs, ac => ac.ID_CLIE, c => c.ID_CLIE, (ac, c) => new { c.ID_CLIE,c.IS_OWN}).Where(x=>x.IS_OWN == true);*/

                    var fact1 = query.Sum(t => (t.AMO_TRANS * t.FAC_TYPE_TRAN));
                    var fact2 = query.Sum(t => ((t.ID_TYPE_TRAN == 1 ? t.AMO_TRANS : 0)));

                    ViewBag.PROFIT = Math.Round(Convert.ToDecimal((fact1 / fact2) * 100), 0);

                    ViewBag.Revenues = Math.Round(Convert.ToDecimal(query.Where(x => x.ID_TYPE_TRAN == 1).Sum(x => x.AMO_TRANS)), 2);
                    ViewBag.Expenses = Math.Round(Convert.ToDecimal(query.Where(x => x.ID_TYPE_TRAN == 2).Sum(x => x.AMO_TRANS)), 2);

                    return View();
                }
                else
                {
                    //return Content("Message: You have not privileges");
                    Session["MAIN"] = "mp5";
                    return View("Index1");
                }
            }
            catch
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("Index");
            }
        }

        public ActionResult Index1()
        {
            return View();
        }

        //
        // GET: /Financial/
        public ActionResult PopUp(int id = 0)
        {
            //var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var ACCO = dbc.ACCOUNTs.Single(a => a.ID_ACCO == id);

            var ts = Convert.ToDateTime(ACCO.DATE_END).Subtract(Convert.ToDateTime(ACCO.DATE_STAR));
            var ts1 = DateTime.Now.Subtract(Convert.ToDateTime(ACCO.DATE_STAR));

            var porcentaje = ts1.TotalDays / ts.TotalDays;

            var Users = dbc.ACCOUNT_CLIENT.Where(ac => ac.ID_ACCO == id)
                    .Join(dbc.CLIENTs, ac => ac.ID_CLIE, c => c.ID_CLIE, (ac, c) => new { c.ID_CLIE, c.IS_OWN }).Where(x => x.IS_OWN == false);

            var Equipment = dbc.ASSETs.Where(a => a.ID_ACCO == id)
                    .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (a, ca) => new { ca.DAT_END, ca.ID_COND })
                    .Where(x => x.DAT_END == null)
                    .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new { c.ID_STAT_ASSE })
                    .Where(x => x.ID_STAT_ASSE == 1);

            var transa = dbc.TRANSACTIONs.Where(t => t.ID_ACCO == id)
                    .Join(dbc.CATEGORY_TRANSACTION, t => t.ID_CATE_TRAN, ct => ct.ID_CATE_TRAN, (t, ct) => new { t.AMO_TRANS, ct.ID_TYPE_TRAN, t.IGV_TRAN })
                    .Join(dbc.TYPE_TRANSACTION, x => x.ID_TYPE_TRAN, tt => tt.ID_TYPE_TRAN, (x, tt) => new { x.AMO_TRANS, x.IGV_TRAN, tt.FAC_TYPE_TRAN, x.ID_TYPE_TRAN });

            ViewBag.Users = Users.Count();
            ViewBag.Equipment = Equipment.Count();
            ViewBag.Porcentaje = Math.Round(porcentaje * 100, 2);
            ViewBag.NAM_ACCO = ACCO.NAM_ACCO;
            ViewBag.SIM_ACCO = ACCO.SIM_MONE_ACCO;
            ViewBag.DES_ACCO = ACCO.DES_ACCO;

            ViewBag.RevenuesSin = String.Format("{0:0,0.00}", Convert.ToDecimal(transa.Where(x => x.ID_TYPE_TRAN == 1).Sum(x => x.AMO_TRANS)));
            ViewBag.RevenuesCon = String.Format("{0:0,0.00}", Convert.ToDecimal(transa.Where(x => x.ID_TYPE_TRAN == 1).Sum(x => x.AMO_TRANS + x.IGV_TRAN)));

            ViewBag.ExpensesSin = String.Format("{0:0,0.00}", Convert.ToDecimal(transa.Where(x => x.ID_TYPE_TRAN == 2).Sum(x => x.AMO_TRANS)), 2);
            ViewBag.ExpensesCon = String.Format("{0:0,0.00}", Convert.ToDecimal(transa.Where(x => x.ID_TYPE_TRAN == 2).Sum(x => x.AMO_TRANS + x.IGV_TRAN)));

            return View();
        }

        //
        // GET: /Financial/
        public ActionResult PROFIT_RATE_ALL()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbc.PROFIT_RATE_ALL(ID_ACCO).ToList();
            return Json(new { area = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
