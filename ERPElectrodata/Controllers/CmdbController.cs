using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class CmdbController : Controller
    {
        //
        // GET: /Cmdb/
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        [Authorize]
        public ActionResult Index()
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                return View();
            }
        }

        #region "Elementos de la Configuración"

        [Authorize]
        public ActionResult Dashboard()
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult Reporte(int id = 0)
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                ViewBag.Id = id;

                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                ViewBag.IdPersEnti = IdPersEnti;
                DateTime fecha = new DateTime(DateTime.Now.Year, 1, 1);
                ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);
                ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
                return View();
            }
        }

        [Authorize]
        public ActionResult DetalleTicket(int id = 0)
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                ViewBag.IdTicket = id;
                return View();
            }
        }

        [Authorize]
        public ActionResult DetallePersonas(int id = 0, int id1=0)
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                var query = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id).ToList()
                        .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                        {
                            ce.SEX_ENTI,
                            x.ID_FOTO
                        }).First();

                ViewBag.IdPersona = id;
                ViewBag.IdCuenta = id1;
                ViewBag.FOTO = query.ID_FOTO.ToString() + ".jpg";
                return View();
            }
        }

        [Authorize]
        public ActionResult DetalleActivos(int id = 0, int id1 = 0)
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                ViewBag.IdActivo = id;
                ViewBag.IdCuenta = id1;
                return View();
            }
        }


        [Authorize]
        public ActionResult ReporteTicket()
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                return View();
            }
        }
        [Authorize]
        public ActionResult ReporteActivos()
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                return View();
            }
        }
        [Authorize]
        public ActionResult ReportePersonas()
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                return View();
            }
        }

        #endregion

        #region "Vistas Servicios"

        public ActionResult ServicioImplementacion()
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ServicioOutsourcing(int id = 0)
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                ViewBag.IdCuentaArea = id;
                return View();
            }
        }

        public ActionResult ServicioServiceDesk()
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ServicioNOC()
        {
            if ((int)Session["CMDB"] == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                return View();
            }
        }

        #endregion

        #region "Reportes Implementacion"

        public ActionResult RptImplementacionOP()
        {
            return View();
        }

        public ActionResult RptImplementacionActivo()
        {
            return View();
        }

        public ActionResult RptImplementacionPersona()
        {
            return View();
        }

        public ActionResult RptImplementacionCambio()
        {
            return View();
        }

        public ActionResult RptImplementacionConocimiento()
        {
            return View();
        }

        #endregion

        #region "Reportes Outsourcing"

        public ActionResult RptOutsourcingTicket(int id = 0)
        {
            ViewBag.IdCuentaArea = id;
            return View();
        }

        public ActionResult RptOutsourcingActivo(int id = 0)
        {
            ViewBag.IdCuentaArea = id;
            return View();
        }

        public ActionResult RptOutsourcingPersona(int id = 0)
        {
            ViewBag.IdCuentaArea = id;
            return View();
        }

        public ActionResult RptOutsourcingCambio(int id = 0)
        {
            ViewBag.IdCuentaArea = id;
            return View();
        }

        public ActionResult RptOutsourcingConocimiento(int id = 0)
        {
            ViewBag.IdCuentaArea = id;
            return View();
        }

        #endregion

        #region "Reportes Service Desk"

        public ActionResult RptServiceDeskTicket()
        {
            return View();
        }

        public ActionResult RptServiceDeskActivo()
        {
            return View();
        }

        public ActionResult RptServiceDeskPersona()
        {
            return View();
        }

        public ActionResult RptServiceDeskCambio()
        {
            return View();
        }

        public ActionResult RptServiceDeskConocimiento()
        {
            return View();
        }

        #endregion

        #region "Reportes NOC"

        public ActionResult RptNocTicket()
        {
            return View();
        }

        public ActionResult RptNocActivo()
        {
            return View();
        }

        public ActionResult RptNocPersona()
        {
            return View();
        }

        public ActionResult RptNocCambio()
        {
            return View();
        }

        public ActionResult RptNocConocimiento()
        {
            return View();
        }

        #endregion

        #region "Métodos"

        public ActionResult ObtenerAreaPorCuenta(int id = 0)
        {
            var consulta = (from a in dbc.ACCOUNTs
                            join aq in dbc.ACCOUNT_QUEUE on a.ID_ACCO equals aq.ID_ACCO
                            join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                          select new
                          {
                              a.ACR_ACCO,
                              q.NAM_QUEU,
                              q.DES_QUEU,
                              aq.ID_ACCO_QUEUE,
                              q.ID_QUEU,
                              a.ID_ACCO,
                              aq.VIG_ACCO_QUEU
                          }).Where(x => x.ID_ACCO == id && x.VIG_ACCO_QUEU == true).OrderBy(x=>x.NAM_QUEU);

            return Json(new { Data = consulta}, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult ObtenerEstructuraCmdb()
        {
            List<ObtenerEstructura_Result> resultado = dbc.ObtenerEstructura().ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTickets(int id = 0, int id1 = 0, int id2 = 0)
        {
            DateTime FechaInicio, FechaFin;
            int IdCuenta = Convert.ToInt32(Request.QueryString["IdCuenta"]);
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }
            var result = dbc.ListarTickets(FechaInicio, FechaFin, IdCuenta, id, id1, id2).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPersonas(int id = 0, int id1 = 0, int id2 = 0)
        {
            
            int IdCuenta = Convert.ToInt32(Request.QueryString["IdCuenta"]);
            
            var result = dbc.ListarPersonas(IdCuenta, id, id1, id2).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarActivos(int id = 0, int id1 = 0, int id2 = 0)
        {
            DateTime FechaInicio, FechaFin;
            int IdCuenta = Convert.ToInt32(Request.QueryString["IdCuenta"]);
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }
            var result = dbc.ListarActivos(FechaInicio, FechaFin, IdCuenta, id, id1, id2).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        //Tickets
        public ActionResult TicketDetalle(int id = 0)
        {
            var result = dbc.TicketDetalle(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TicketDetalleOp(int id = 0)
        {
            var result = dbc.TicketDetalleOp(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TicketDetalleViaticos(int id = 0)
        {
            var result = dbc.TicketDetalleViaticos(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TicketDetalleCambios(int id = 0)
        {
            var result = dbc.TicketDetalleCambios(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        
        //Activos
        public ActionResult ObtenerDatosActivo(int id = 0, int id1 = 0)
        {
            id1 = 4;
            var query = dbc.ActObtenerDetalle(id, id1).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActivoDetalleComponente(int id = 0, int id1 = 0, int id2 = 0)
        {
            var result = dbc.ActivoDetalleComponente(id,id1,id2).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        //Persona
        public ActionResult PersonaDetalleTicket(int id = 0)
        {
            var result = dbc.PersonaDetalleTicket(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonaDetalleOp(int id = 0)
        {
            var result = dbc.PersonaDetalleOp(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonaDetalleActivos(int id = 0, int id1 = 0)
        {
            var result = dbc.PersonaDetalleActivos(id, id1).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonaDetalleViaticos(int id = 0)
        {
            var result = dbc.PersonaDetalleViaticos(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonaDetalleCambios(int id = 0)
        {
            var result = dbc.PersonaDetalleCambios(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
  
        public ActionResult AssetTicket(int id = 0, int id1=0)
        {
            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = (from df in dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == id)
                         join t in dbc.TICKETs.Where(x => x.ID_STAT_END != 2) on df.ID_TICK equals t.ID_TICK
                         join tf in dbc.TYPE_FORMAT on t.ID_TYPE_FORM equals tf.ID_TYPE_FORM
                         join dt in dbc.DETAILS_TICKET.Where(x => x.ID_TYPE_DETA_TICK == 7) on t.ID_TICK equals dt.ID_TICK into dtx
                         from x in dtx.DefaultIfEmpty()
                         //join atf in db.ATTACHED_TICKET_FORMAT on x.ID_DETA_TICK equals atf.ID_DETA_TICK into atx
                         //from ax in atx.DefaultIfEmpty()
                         select new
                         {
                             ID_TICK = t.ID_TICK,
                             COD_TICK = t.COD_TICK,
                             CREATE_TICK = t.CREATE_TICK,
                             NAM_TYPE_FORM = tf.NAM_TYPE_FORM,
                             ID_PERS_ENTI = t.ID_PERS_ENTI,
                             ID_DETA_TICK = x.ID_DETA_TICK == null ? 0 : x.ID_DETA_TICK
                         }).ToList();

            var result = (from q in query
                          join pe in dbe.PERSON_ENTITY on q.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          select new
                          {
                              q.ID_TICK,
                              q.COD_TICK,
                              CREATE_TICK = String.Format("{0:d}", q.CREATE_TICK),
                              q.NAM_TYPE_FORM,
                              ce.FIR_NAME,
                              ce.LAS_NAME,
                              ADJUNTOS = AdjuntosDetailsTicket(q.ID_DETA_TICK, q.ID_TICK, id1)
                          }).Where(x => x.ADJUNTOS != "").OrderByDescending(x => x.ID_TICK);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public string AdjuntosDetailsTicket(int id, int IdTicket, int IdCuenta)
        {
            int id_TF = dbc.TICKETs.Single(x => x.ID_TICK == IdTicket).ID_TYPE_FORM.Value;

            string ruta = "Delivery";
            if (id_TF == 2) { ruta = "Reception"; }
            string Adjun = "";
            try
            {
                var query = dbc.ATTACHED_TICKET_FORMAT.Where(a => a.ID_DETA_TICK == id);
                foreach (ATTACHED_TICKET_FORMAT atta in query)
                {
                    Adjun = Adjun + "<li><a href='/Adjuntos/" + ruta + "/" + IdCuenta.ToString() + "/" + atta.NAM_ATTA + "_" + atta.ID_ATTA_TICK_FORM + atta.EXT_ATTA + " ' TARGET='_BLANK'>" + atta.NAM_ATTA + atta.EXT_ATTA + "<img src='/Content/Images/pdf.png' style='width:14px; height:14px; border:none;'></a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
