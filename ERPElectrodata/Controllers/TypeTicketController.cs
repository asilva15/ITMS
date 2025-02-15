using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeTicketController : Controller
    {
        private CoreEntities db = new CoreEntities();

        public ActionResult List()
        {

            if ((int)Session["ID_ACCO"] == 4)
            {
                var result = (from tt in db.TYPE_TICKET
                              where tt.VIS_TYPE_TICK == true && tt.VIG_TYPE_TICK == true
                              select new
                              {
                                  tt.ID_TYPE_TICK,
                                  tt.NAM_TYPE_TICK,
                              }).ToList().OrderBy(tt => tt.ID_TYPE_TICK);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else if ((int)Session["ID_ACCO"] == 26)
            {
                var result = (from tt in db.TYPE_TICKET
                              where tt.VIS_TYPE_TICK == true && tt.VIG_TYPE_TICK == true && tt.ID_TYPE_TICK != 6
                              select new
                              {
                                  tt.ID_TYPE_TICK,
                                  tt.NAM_TYPE_TICK,
                              }).ToList().OrderBy(tt => tt.ID_TYPE_TICK);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else if ((int)Session["ID_ACCO"] == 60)
            {
                //var result = (from tt in db.TYPE_TICKET
                //              where tt.VIS_TYPE_TICK == true && tt.VIG_TYPE_TICK == true
                //              select new
                //              {
                //                  tt.ID_TYPE_TICK,
                //                  tt.NAM_TYPE_TICK,
                //              }).ToList().Where(t => t.ID_TYPE_TICK == 1 || t.ID_TYPE_TICK == 2).OrderBy(tt => tt.ID_TYPE_TICK);
                //return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

                /* REVISADO LS*/
                var result = (from tt in db.TYPE_TICKET
                              where tt.VIS_TYPE_TICK == true && tt.VIG_TYPE_TICK == true
                              select new
                              {
                                  tt.ID_TYPE_TICK,
                                  tt.NAM_TYPE_TICK,
                              }).ToList().Where(t => t.ID_TYPE_TICK == 1 || t.ID_TYPE_TICK == 2 || t.ID_TYPE_TICK == 4).OrderBy(tt => tt.ID_TYPE_TICK);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from tt in db.TYPE_TICKET
                              where tt.VIS_TYPE_TICK == true && tt.VIG_TYPE_TICK == true && tt.ID_TYPE_TICK != 4 && tt.ID_TYPE_TICK != 6
                              select new
                              {
                                  tt.ID_TYPE_TICK,
                                  tt.NAM_TYPE_TICK,
                              }).ToList().OrderBy(tt => tt.ID_TYPE_TICK);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
        }
        
        //Lista TYPO de tiket filtrado por empresa
        public ActionResult ListarCboTypeTicketxEmpresa()
        {
            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);

          
                var result = db.ListarCboTypeTicketxEmpresa(idcuenta).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ListarTipoCreacion(string q, string page, int id)
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

            var result = db.ListarTipoTicket(termino).ToList();
            if(id!=4 && id != 26)
            {
                result = result.Where(x => x.id != 4).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoEdicion(int id)
        {
            //int id= Convert.ToInt32(Request.Params["IdCuenta"]);
            if (id == 4 || id==26)
            {
                var result = (from tt in db.TYPE_TICKET
                              where tt.VIS_TYPE_TICK == true && tt.VIG_TYPE_TICK == true
                              select new
                              {
                                  tt.ID_TYPE_TICK,
                                  tt.NAM_TYPE_TICK,
                              }).ToList().OrderBy(tt => tt.ID_TYPE_TICK);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from tt in db.TYPE_TICKET
                              where tt.VIS_TYPE_TICK == true && tt.VIG_TYPE_TICK == true && tt.ID_TYPE_TICK != 4
                              select new
                              {
                                  tt.ID_TYPE_TICK,
                                  tt.NAM_TYPE_TICK,
                              }).ToList().OrderBy(tt => tt.ID_TYPE_TICK);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListarTipoTicket(string q, string page)
        {
            string termino = "";

            //if (Request.QueryString["q"] == null)
            //{
            //    termino = "";
            //}
            //else
            //{
            //    termino = Request.QueryString["q"].ToString();
            //}

            var result = db.ListarTipoTicket(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        //
        // GET: /TypeTicket/

        public ActionResult Index()
        {
            return View(db.TYPE_TICKET.ToList());
        }

        //
        // GET: /TypeTicket/Details/5

        public ActionResult Details(int id = 0)
        {
            TYPE_TICKET type_ticket = db.TYPE_TICKET.Find(id);
            if (type_ticket == null)
            {
                return HttpNotFound();
            }
            return View(type_ticket);
        }
        
        //
        // GET: /TypeTicket/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TypeTicket/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TYPE_TICKET type_ticket)
        {
            if (ModelState.IsValid)
            {
                db.TYPE_TICKET.Add(type_ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(type_ticket);
        }

        //
        // GET: /TypeTicket/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TYPE_TICKET type_ticket = db.TYPE_TICKET.Find(id);
            if (type_ticket == null)
            {
                return HttpNotFound();
            }
            return View(type_ticket);
        }

        //
        // POST: /TypeTicket/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TYPE_TICKET type_ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(type_ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(type_ticket);
        }

        //
        // GET: /TypeTicket/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TYPE_TICKET type_ticket = db.TYPE_TICKET.Find(id);
            if (type_ticket == null)
            {
                return HttpNotFound();
            }
            return View(type_ticket);
        }

        //
        // POST: /TypeTicket/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            TYPE_TICKET type_ticket = db.TYPE_TICKET.Find(id);
            db.TYPE_TICKET.Remove(type_ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult ListTipoTicketSLA()
        {

                var result = (from tt in db.TYPE_TICKET
                              where tt.VIS_TYPE_TICK == true && tt.VIG_TYPE_TICK == true
                              select new
                              {
                                  tt.ID_TYPE_TICK,
                                  tt.NAM_TYPE_TICK,
                              }).ToList().Where(t => t.ID_TYPE_TICK == 1 || t.ID_TYPE_TICK == 2).OrderBy(tt => tt.ID_TYPE_TICK);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            
        }

    }
}