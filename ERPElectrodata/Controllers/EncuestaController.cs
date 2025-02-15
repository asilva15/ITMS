using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class EncuestaController : Controller
    {
        public CoreEntities db = new CoreEntities();
        //
        // GET: /Encuesta/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Encuesta/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Encuesta/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Encuesta/Create

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
        // GET: /Encuesta/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Encuesta/Edit/5

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
        // GET: /Encuesta/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Encuesta/Delete/5

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

        public ActionResult EncuestaPortalUsuario(int IdDetalleTicket)
        {
            string flag = "", mensaje = "", codigoTicket="", usuarioAfectado="";
            int IdEstadoEncuesta = 0;
            var objEncuesta = db.ValidacionEncuestaPortalUsuario(IdDetalleTicket).FirstOrDefault();

            codigoTicket = Convert.ToString(objEncuesta.CodigoTicket);
            usuarioAfectado = Convert.ToString(objEncuesta.UsuarioAfectado);
            flag = Convert.ToString(objEncuesta.Id);
            mensaje = objEncuesta.Mensaje;
            IdEstadoEncuesta = Convert.ToInt32(objEncuesta.IdEstadoEncuesta);

            if (IdEstadoEncuesta == 0)
            {
                flag = "0";
                mensaje = "No se encontró una encuesta para este ticket.";
            }

            ViewBag.IdDetalleTicket = IdDetalleTicket;
            ViewBag.CodigoTicket = codigoTicket;
            ViewBag.UsuarioAfectado = usuarioAfectado;
            ViewBag.Flag = flag;
            ViewBag.Mensaje = mensaje;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEncuesta()
        {
            try
            {
                int IdDetalleTicket = Convert.ToInt32(Request.Params["IdDetalleTicket"]);
                string chkOpcion = Convert.ToString(Request.Params["chkOpcion"]);
                string Comentario = Convert.ToString(Request.Params["txtComentario"]);

                if (chkOpcion == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.Mensaje) top.Mensaje('VACIO');}window.onload=init;</script>");
                }
                //Validación si responde SI
                if (chkOpcion == "SI")
                {
                    var objPortal = db.EncuestaPortalUsuario.Single(x => x.IdDetalleTicket == IdDetalleTicket);
                    objPortal.IdEstadoEncuesta = 2;
                    objPortal.FechaRespuesta = DateTime.Now;
                    objPortal.Comentario = Comentario;
                    db.Entry(objPortal).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (chkOpcion == "NO")
                {
                    var objPortal = db.EncuestaPortalUsuario.Single(x => x.IdDetalleTicket == IdDetalleTicket);
                    objPortal.IdEstadoEncuesta = 3;
                    objPortal.FechaRespuesta = DateTime.Now;
                    objPortal.Comentario = Comentario;
                    db.Entry(objPortal).State = EntityState.Modified;
                    db.SaveChanges();

                    var ticket = db.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == IdDetalleTicket);
                    int IdTicket = Convert.ToInt32(ticket.ID_TICK);

                    var dt = new DETAILS_TICKET();
                    dt.ID_TICK = IdTicket;
                    dt.ID_STAT = 1;
                    dt.ID_TYPE_DETA_TICK = 3;
                    dt.COM_DETA_TICK = "Ticket abierto en la Encuesta de Satisfacción por usuario afectado.</br>"+ 
                       ( Comentario.Trim() == "" ? "" : "Comentario del usuario: "+ Comentario );
                    dt.UserId = 34;
                    dt.CREATE_DETA_INCI = DateTime.Now;
                    dt.SEND_MAIL = false;
                    dt.PortalComent = true;
                    db.DETAILS_TICKET.Add(dt);
                    db.SaveChanges();

                    var tick = db.TICKETs.Single(x => x.ID_TICK == IdTicket);
                    tick.ID_STAT_END = 1;
                    db.TICKETs.Attach(tick);
                    db.Entry(tick).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.Mensaje) top.Mensaje('OK');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.Mensaje) top.Mensaje('ERROR');}window.onload=init;</script>");
            }
        }


    }
}
