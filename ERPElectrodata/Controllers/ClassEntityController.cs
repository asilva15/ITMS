using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ClassEntityController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();

        public ActionResult CreateSupply()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSupply(CLASS_ENTITY ce)
        {
            
            string ID_SIDIGE = Convert.ToString(Request.Params["ID_SIDIGE"]);
            string NUM_TYPE_DI = Convert.ToString(Request.Params["NUM_TYPE_DI"]);
            string COM_NAME = Convert.ToString(Request.Params["COM_NAME"]);
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            if (!String.IsNullOrEmpty(ID_SIDIGE))
            {
                ce.ID_SIDIGE = ID_SIDIGE;
            }

            if (String.IsNullOrEmpty(NUM_TYPE_DI))
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.doneCreateSupply) top.doneCreateSupply('ERROR','Please enter RUC');}window.onload=init;</script>");
            }
            else
            {
                int cant = dbe.CLASS_ENTITY.Where(x => x.NUM_TYPE_DI.ToLower().Contains(NUM_TYPE_DI.ToLower())).Count();
                if (cant > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.doneCreateSupply) top.doneCreateSupply('ERROR','This RUC is already registered in the database');}window.onload=init;</script>");
                }
            }

            if (String.IsNullOrEmpty(COM_NAME))
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.doneCreateSupply) top.doneCreateSupply('ERROR','Please Enter Commercial Name');}window.onload=init;</script>");
            }


            try
            {
                ce.VIG_ENTI = true;
                ce.CREATED = DateTime.Now;
                ce.ID_TYPE_ENTI = 1;
                ce.ID_TYPE_DI = 4;
                //ce.UserId = UserId;

                dbe.CLASS_ENTITY.Add(ce);
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.doneCreateSupply) top.doneCreateSupply('OK','"+ce.ID_ENTI+"','"+ce.COM_NAME+"');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.doneCreateSupply) top.doneCreateSupply('ERROR','Contact Your Administrator','');}window.onload=init;</script>");

            }

            //return View();
        }

        public ActionResult ListAllCompany()
        {
            string cont = null;
            try
            {
                cont = Convert.ToString(Request.Params["filter[filters][0][value]"]).ToUpper();
            }
            catch { }

            var query = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1);

            if (!String.IsNullOrEmpty(cont))
            {
                query = query.Where(x=>x.COM_NAME.ToUpper().Contains(cont) || x.NUM_TYPE_DI.ToUpper().Contains(cont));
            }

            var result = (from x in query.ToList()
                          join td in dbe.TYPE_DOCUMENTIDENT on x.ID_TYPE_DI equals td.ID_TYPE_DI
                          select new
                          {
                              ID_ENTI = x.ID_ENTI,
                              COM_NAME = (x.COM_NAME == null ? "" : x.COM_NAME.ToUpper()),
                              NAM_TYPE_DI = td.NAM_TYPE_DI,
                              NUM_TYPE_DI = (x.NUM_TYPE_DI == null ? "" : x.NUM_TYPE_DI),
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CIAOP()
        {
            var CIA_OP = new int[] { 9, 1185, 3975 };

            var query = dbe.CLASS_ENTITY.Where(t => CIA_OP.Contains((int)t.ID_ENTI));

            var result = (from e in query.ToList()
                          join tdi in dbe.TYPE_DOCUMENTIDENT on e.ID_TYPE_DI equals tdi.ID_TYPE_DI
                          select new
                          {
                              COM_NAME = e.COM_NAME,
                              ID_ENTI = e.ID_ENTI,
                              NAM_TYPE_DI = tdi.NAM_TYPE_DI,
                              NUM_TYPE_DI = e.NUM_TYPE_DI
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /ClassEntity/
        public ActionResult ListPerson()
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbe.CLASS_ENTITY.Where(x=>x.ID_TYPE_ENTI == 2).ToList();

            var result = (from x in query.OrderBy(x=>x.LAS_NAME).Skip(skip).Take(take)
                          select new {
                              ID_ENTI = x.ID_ENTI,
                              FIR_NAME = x.FIR_NAME,
                              LAS_NAME = x.LAS_NAME,
                              MOT_NAME = x.MOT_NAME,
                              SEC_NAME = x.SEC_NAME,
                              THI_NAME = x.THI_NAME,
                              CEL_ENTI = x.CEL_ENTI == null ? "" : x.CEL_ENTI,
                              EMA_ENTI = x.EMA_ENTI == null ? "":x.EMA_ENTI,
                              SEX_ENTI = x.SEX_ENTI,
                          });

            return Json(new {Data = result,Count = query.Count() },JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarCompania(string q, string page)
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

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            var resultado = dbc.ListarCompania(termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Admin(int id)
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /ClassEntity/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ClassEntity/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ClassEntity/Create

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
        // GET: /ClassEntity/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ClassEntity/Edit/5

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
        // GET: /ClassEntity/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ClassEntity/Delete/5

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
