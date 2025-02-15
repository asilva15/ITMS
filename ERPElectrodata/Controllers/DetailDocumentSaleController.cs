using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class DetailDocumentSaleController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /DetailDocumentSale/

        public ActionResult ListByCOMP()
        {
            int ID_COMP = -1;
            string ARTI = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_COMP = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "CODE_ARTI")
            {
                ARTI = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }

            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_COMP = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "CODE_ARTI")
            {
                ARTI = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }            

            var query = dbc.DOCUMENT_SALE.Where(x => x.ID_COMP == ID_COMP)
                .Join(dbc.DETAIL_DOCUMENT_SALE, ds => ds.ID_DOCU_SALE, dds => dds.ID_DOCU_SALE, (ds, dds) => new {
                    ID_DETA_DOCU_SALE = dds.ID_DETA_DOCU_SALE,
                    ID_DOCU_SALE = ds.ID_DOCU_SALE,
                    CODE = dds.CODE.Trim(),
                    CODE_ARTI = dds.CODE_ARTI,
                    DESC_DETA = dds.DESC_DETA,
                    dds.SERIE,
                    dds.ID_ITEM_CATE,
                    dds.DAT_INI_SUPP,
                    DAT_END_SUPP = (dds.DAT_END_SUPP == null ? DateTime.Now : dds.DAT_END_SUPP),
                    dds.HOURS,
                    HOURS_MISS = (dds.HOURS == null ? 1 : dds.HOURS) - (dds.ATTENDANCE_HOURS == null ? 0 : dds.ATTENDANCE_HOURS),                    
                });

            DateTime Ahora = DateTime.Now;
            Ahora = ((Ahora.AddHours(Ahora.Hour * -1)).AddMinutes(Ahora.Minute * -1)).AddSeconds((Ahora.Second + 1) * -1);

            var result = (from x in query.Where(x => x.DAT_END_SUPP > Ahora && x.HOURS_MISS > 0).ToList()
                          join a in dbc.ITEM_CATEGORY on x.ID_ITEM_CATE equals a.ID_ITEM_CATE into la
                          from xa in la.DefaultIfEmpty()
                            select new {
                                ID_DETA_DOCU_SALE = x.ID_DETA_DOCU_SALE,
                                ID_DOCU_SALE = x.ID_DOCU_SALE,
                                CODE = x.CODE,
                                //CODE_ARTI = x.CODE_ARTI,
                                TOOLTIP = x.DESC_DETA + (x.CODE_ARTI == null ? "" : ". " + x.CODE_ARTI),
                                DESC_DETA = x.DESC_DETA,
                                SerieBit = (x.SERIE == null ? false : true),
                                CODE_ARTI = (xa == null ? "" : xa.NAM_ITEM_CATE) + " - " + x.CODE + (x.SERIE == null ? "" : " / Serie: " + x.SERIE),
                                ITEM_CATE = (xa == null ? "" : xa.NAM_ITEM_CATE) + " - " + x.CODE +
                                            (x.SERIE == null ? "" : "<div style='width:100%; font-size:.9em;font-family:Arial;'>Serie: " + x.SERIE + "</div>") +
                                            (x.DAT_INI_SUPP != null ? "<div style='width:100%; font-size:.9em;font-family:Arial;'>Warranty from " + String.Format("{0:d}", x.DAT_INI_SUPP) + " to " + String.Format("{0:d}", x.DAT_END_SUPP) + "</div>" :
                                             x.HOURS != null ? "<div style='width:100%; font-size:.9em;font-family:Arial;'>Warranty: " + String.Format("{0:N0}", x.HOURS) + " Hours (Missing hours: " + String.Format("{0:N0}", x.HOURS_MISS) + ")</div>" : ""),
                            });

            if (ARTI.Length > 0) {
                result = result.Where(x => x.CODE_ARTI.ToLower().Contains(ARTI.ToLower()));
            }

            return Json(new {Data = result,Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DetailDocumentSale/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DetailDocumentSale/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DetailDocumentSale/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DetailDocumentSale/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DetailDocumentSale/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DetailDocumentSale/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DetailDocumentSale/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
