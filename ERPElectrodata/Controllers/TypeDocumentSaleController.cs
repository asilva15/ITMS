using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeDocumentSaleController : Controller
    {
        public CoreEntities dbc = new CoreEntities();

        //
        // GET: /TypeDocumentSale/
        public ActionResult ListAll()
        {
            var query = dbc.TYPE_DOCUMENT_SALE.ToList();

            var result = (from x in query
                          select new {
                              ID_TYPE_DOCU_SALE = x.ID_TYPE_DOCU_SALE,
                              NAM_TYPE_DOCU_SALE = x.NAM_TYPE_DOCU_SALE,
                              COD_TYPE_DOCU_SALE = x.COD_TYPE_DOCU_SALE
                          });
            return Json(new {Data = result,Count = query.Count() },JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /TypeDocumentSale/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /TypeDocumentSale/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TypeDocumentSale/Create

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
        // GET: /TypeDocumentSale/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /TypeDocumentSale/Edit/5

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
        // GET: /TypeDocumentSale/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /TypeDocumentSale/Delete/5

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

        public ActionResult ListOPxTipo()
        {
            int IdTipoOp;
            string FIELD;
            string texto = "";

            int EstadoTipo=0;

            if (Request.Params["EstadoTipo"] != null)
            {
                FIELD = Convert.ToString(Request.Params["idTypeDocuSale"]);
                EstadoTipo = Convert.ToInt32(Request.Params["EstadoTipo"].ToString()); //1c
                                                                                       //IdTipoOp = Convert.ToInt32(Request.Params["IdTypeOp"]);
                if (FIELD == "ID_TYPE_DOCU_SALE" && EstadoTipo != 0)
                {
                    IdTipoOp = Convert.ToInt32(Request.Params["IdTypeOp"]);
                    texto = Convert.ToString(Request.Params["texto"]);
                    if (texto == null) { texto = ""; }
                }
                else
                {
                    IdTipoOp = Convert.ToInt32(Request.Params["texto"]);
                }
            }
            else {
                FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

                if (FIELD == "ID_TYPE_DOCU_SALE")
                {
                    IdTipoOp = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
                    texto = Convert.ToString(Request.Params["filter[filters][1][value]"]);
                    if (texto == null) { texto = ""; }
                }
                else
                {
                    IdTipoOp = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                }
            }
            IQueryable<DOCUMENT_SALE> query;
            if (EstadoTipo != 0) {

                query = dbc.DOCUMENT_SALE.Where(x => x.EstadoTipoProyecto == EstadoTipo && x.ID_TYPE_DOCU_SALE == IdTipoOp && x.NUM_DOCU_SALE.Contains(texto));
            } else {

                query = dbc.DOCUMENT_SALE.Where(x => x.ID_TYPE_DOCU_SALE == IdTipoOp && x.NUM_DOCU_SALE.Contains(texto));
            }

            var result = (from x in query.ToList()
                          select new
                          {
                              x.ID_DOCU_SALE,
                              x.NUM_DOCU_SALE
                          }).OrderByDescending(x => x.ID_DOCU_SALE);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarTipoOP(string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = (from x in dbc.TYPE_DOCUMENT_SALE.ToList()
                          select new
                          {
                              id = x.ID_TYPE_DOCU_SALE,
                              text = x.COD_TYPE_DOCU_SALE
                          });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
