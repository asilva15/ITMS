using ERPElectrodata.AppCode;
using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Controllers
{
    public class SupervisionController : Controller
    {
        //
        // GET: /Supervision/
        private EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inicio()
        {
            return View();
        }

        public ActionResult ObtenerIncidentePrioridad()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktTicketxPrioridadTop20(Cuenta).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaPrioridad = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerUsuarioIncidente()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktUsuarioTickets(1, Cuenta).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { UsuarioIncidente = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerUsuarioRequerimiento()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktUsuarioTickets(2, Cuenta).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { UsuarioRequerimiento = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarListaPrioridad()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;

            List<tktTicketxPrioridadTop20_Result> query = dbc.tktTicketxPrioridadTop20(Cuenta).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#fff;background-color:#26ADE4;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='tg-031e'>PRIORIDAD</td>");
            sb.Append("<th class='tg-031e'>CODIGO</td>");
            sb.Append("<th class='tg-031e'>ASIGNADO A</td>");
            sb.Append("<th class='tg-031e'>COMENTARIOS</td>");
            sb.Append("<th class='tg-031e'>CAUSAS</td>");
            sb.Append("<th class='tg-031e'>ESTADO_ACTUAL</td>");
            sb.Append("</tr>");

            foreach (tktTicketxPrioridadTop20_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='tg-031e'>" + dr.PRIORIDAD + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CODIGO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ASIGNADO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.COMENTARIO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CAUSA + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ESTADO_ACTUAL + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=IncidentePrioridad" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult Reportes()
        {
            return View();
        }

        public ActionResult RptPrioridad()
        {
            return View();
        }

        public ActionResult RptCantidad()
        {
            return View();
        }

        public ActionResult RptTiempo()
        {
            return View();
        }

        public ActionResult RptTiempoRequerimiento()
        {
            return View();
        }

        public ActionResult RptSLA()
        {
            return View();
        }

        public ActionResult RptSLAReq()
        {
            return View();
        }

        public ActionResult RptCargaLaboral()
        {
            return View();
        }

        public ActionResult RptActividad()
        {
            return View();
        }

        public ActionResult ObtenerTiempoIncidente(int idTipo)
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktTicketTiempoTranscurrido(Cuenta, 0, 0, idTipo).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaTiempo = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarListaTiempo()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;

            int TipoTicket = Convert.ToInt32(Request.Params["TipoTicket"].ToString());

            List<tktTicketTiempoTranscurrido_Result> query = dbc.tktTicketTiempoTranscurrido(Cuenta, 0, 0, TipoTicket).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#fff;background-color:#26ADE4;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='tg-031e'>TIEMPO (Dias)</td>");
            sb.Append("<th class='tg-031e'>CODIGO</td>");
            sb.Append("<th class='tg-031e'>ASIGNADO A</td>");
            sb.Append("<th class='tg-031e'>COMENTARIOS</td>");
            sb.Append("<th class='tg-031e'>CAUSAS</td>");
            sb.Append("<th class='tg-031e'>ESTADO</td>");
            sb.Append("</tr>");

            foreach (tktTicketTiempoTranscurrido_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='tg-031e'>" + dr.TIEMPO_TRANSCURRIDO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CODIGO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ASIGNADO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.COMENTARIO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CAUSA + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ESTADO + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=IncidenteTiempoTranscurrido" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ObtenerSLAIncidente(int idTipo)
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktTicketSLA(Cuenta, 0, 0, idTipo).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaTiempo = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarListaSLA()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;

            int TipoTicket = Convert.ToInt32(Request.Params["TipoTicket"].ToString());

            List<tktTicketSLA_Result> query = dbc.tktTicketSLA(Cuenta, 0, 0, TipoTicket).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#fff;background-color:#26ADE4;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='tg-031e'>TIEMPO (Hrs)</td>");
            sb.Append("<th class='tg-031e'>CODIGO</td>");
            sb.Append("<th class='tg-031e'>ASIGNADO A</td>");
            sb.Append("<th class='tg-031e'>COMENTARIOS</td>");
            sb.Append("<th class='tg-031e'>CAUSAS</td>");
            sb.Append("<th class='tg-031e'>ESTADO</td>");
            sb.Append("</tr>");

            foreach (tktTicketSLA_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='tg-031e'>" + dr.TIEMPO_RESTANTE + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CODIGO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ASIGNADO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.COMENTARIO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CAUSA + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ESTADO + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=IncidenteSLA" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ObtenerCargaLaboralInci()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktCargaLaboralIncidente(Cuenta, 0).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaInci = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCargaLaboralReq()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktCargaLaboralRequerimiento(Cuenta, 0).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaReq = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteSolicitante(int id = 0, int id1 = 0)
        {
            ViewBag.Solicitante = id;
            ViewBag.Tipo = id1;
            return View();
        }

        public ActionResult ObtenerTicketxSolicitante(int id = 0, int id1 = 0)
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktTicketxSolicitante(Cuenta, id, id1).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaSolicitante = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteAsignado(int id = 0, int id1 = 0)
        {
            ViewBag.Asignado = id;
            ViewBag.Tipo = id1;
            return View();
        }

        public ActionResult ObtenerTicketxAnalista(int id = 0, int id1 = 0)
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktTicketxAnalista(Cuenta, id, id1).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaSolicitante = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarListaSolicitante()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;

            int Solicitante = Convert.ToInt32(Request.Params["Solicitante"].ToString());
            int TipoTicket = Convert.ToInt32(Request.Params["Tipo"].ToString());

            List<tktTicketxSolicitante_Result> query = dbc.tktTicketxSolicitante(Cuenta, Solicitante, TipoTicket).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#fff;background-color:#26ADE4;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='tg-031e'>PRIORIDAD</td>");
            sb.Append("<th class='tg-031e'>ESTADO</td>");
            sb.Append("<th class='tg-031e'>CODIGO</td>");
            sb.Append("<th class='tg-031e'>ASIGNADO A</td>");
            sb.Append("<th class='tg-031e'>COMENTARIOS</td>");
            sb.Append("<th class='tg-031e'>CAUSAS</td>");
            sb.Append("<th class='tg-031e'>SOLUCION</td>");
            sb.Append("</tr>");

            foreach (tktTicketxSolicitante_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='tg-031e'>" + dr.PRIORIDAD + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ESTADO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CODIGO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ASIGNADO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.COMENTARIO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CAUSA + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.SOLUCION + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=TicketSolicitante" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ExportarListaAsignado()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;

            int Asignado = Convert.ToInt32(Request.Params["Asignado"].ToString());
            int TipoTicket = Convert.ToInt32(Request.Params["Tipo"].ToString());

            List<tktTicketxSolicitante_Result> query = dbc.tktTicketxAnalista(Cuenta, Asignado, TipoTicket).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#fff;background-color:#26ADE4;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='tg-031e'>PRIORIDAD</td>");
            sb.Append("<th class='tg-031e'>ESTADO</td>");
            sb.Append("<th class='tg-031e'>CODIGO</td>");
            sb.Append("<th class='tg-031e'>ASIGNADO A</td>");
            sb.Append("<th class='tg-031e'>COMENTARIOS</td>");
            sb.Append("<th class='tg-031e'>CAUSAS</td>");
            sb.Append("<th class='tg-031e'>ESTADO_ACTUAL</td>");
            sb.Append("</tr>");

            foreach (tktTicketxSolicitante_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='tg-031e'>" + dr.PRIORIDAD + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ESTADO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CODIGO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ASIGNADO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.COMENTARIO + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CAUSA + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.ESTADO_ACTUAL + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=TicketSolicitante" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult RptTicketxActivo(int id = 0, int id1 = 0)
        {
            return View();
        }

        public ActionResult ObtenerTicketxActivo(int id = 0, int id1 = 0)
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktTicketxActivo(1,0,Cuenta).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaTicket = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteTicketActivoDetalle(int id = 0)
        {
            ViewBag.IdTipoActivo = id;
            return View();
        }

        public ActionResult ObtenerTicketActivoDetalle(int id = 0)
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.tktTicketxActivoDetalle(1, id, Cuenta).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaTicketActivo = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarListaTicketActivo()
        {
            Sesiones Sesion = new Sesiones();
            int Cuenta = Sesion.SesionIdCuenta;

            int TipoActivo = Convert.ToInt32(Request.Params["TipoActivo"].ToString());

            List<tktTicketxActivoDetalle_Result> query = dbc.tktTicketxActivoDetalle(1, TipoActivo, Cuenta).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#fff;background-color:#26ADE4;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='tg-031e'>ESTADO</td>");
            sb.Append("<th class='tg-031e'>CODIGO TICKET</td>");
            sb.Append("<th class='tg-031e'>SERVICIO</td>");
            sb.Append("<th class='tg-031e'>INCIDENTE</td>");
            sb.Append("<th class='tg-031e'>TIPO ACTIVO</td>");
            sb.Append("<th class='tg-031e'>ASIGNADO A</td>");
            sb.Append("<th class='tg-031e'>COMENTARIO</td>");
            sb.Append("</tr>");

            foreach (tktTicketxActivoDetalle_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='tg-031e'>" + dr.Estado + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.CodigoTicket + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.Categoria3 + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.Categoria4 + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.TipoActivo + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.Asignado + "</td>");
                sb.Append("<td class='tg-031e'>" + dr.Comentario + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=TicketActivoDetalle" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult RptReporteTicket()
        {            
            return View();
        }

        //public ActionResult ListaTicket()
        //{
        //    int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

        //    List<RptReporteTickets_Result> resultado = dbc.RptReporteTickets(IdCuenta, DateTime.Now.AddDays(-150), DateTime.Now).ToList();

        //    return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        //}
    }
}
