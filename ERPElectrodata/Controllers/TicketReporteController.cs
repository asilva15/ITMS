using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TicketReporteController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /TicketReporte/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Seguimiento()
        {
            return View();
        }

        public ActionResult RptTicketSeguimiento()
        {
            DateTime fecha = (DateTime.Now).AddDays(-7);
            ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);
            ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
            return View();
        }

        public ActionResult Atenciones()
        {
            int anio = 0, mes = 0;
            anio = DateTime.Now.Year;
            mes = DateTime.Now.Month;

            var query=dbc.ACCOUNTING_MONTH.Where(x=> x.ID_ACCO_YEAR==anio && x.ACCO_MONT==mes).SingleOrDefault();

            ViewBag.AnioActual = anio;
            ViewBag.MesActual = query.ID_ACCO_MONT;
            return View();
        }
        public ActionResult RptTicketPrioridad()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.Anio = DateTime.Now.Year;
            ViewBag.Mes = DateTime.Now.Month;
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);
            ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
            ViewBag.IdCuenta = IdCuenta;

            var prioridad = dbc.PRIORITies.FirstOrDefault();
            ViewBag.IdPrioridad = prioridad.ID_PRIO;
            ViewBag.Prioridad = prioridad.NAM_PRIO;
            return View();
        }

        public ActionResult Ranking()
        {
            return View();
        }

        public ActionResult GestionDiaria()
        {
            return View();
        }

        public ActionResult GestionAnalista()
        {
            return View();
        }

        public ActionResult TicketsCreaCierraMes()
        {
            int anio = 0, mes = 0;
            anio = DateTime.Now.Year;
            mes = DateTime.Now.Month;

            var query = dbc.ACCOUNTING_MONTH.Where(x => x.ID_ACCO_YEAR == anio && x.ACCO_MONT == mes).SingleOrDefault();

            ViewBag.AnioActual = anio;
            ViewBag.MesActual = query.ID_ACCO_MONT;
            return View();
        }

        public ActionResult ListarTicketsConsolidado(int id = 0, int id1 = 0, string id2 = "")
        {
            DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            string IdEstado = Convert.ToString(Request.Params["IdEstado"]);
            string IdAsignado = Convert.ToString(Request.Params["IdAsignado"]);
            string cbCuenta = id2;

            if (IdEstado == "null" || IdEstado == "") { IdEstado = "0"; }
            if (IdAsignado == "null" || IdAsignado == "") { IdAsignado = "0"; }
            if (cbCuenta == "null" || cbCuenta == "") { cbCuenta = "0"; }

            int Cuenta1 = 0, Cuenta2 = 0, Cuenta3 = 0, Cuenta4 = 0, Cuenta5 = 0;
            int cont = 1;
            foreach (string strIdAr in cbCuenta.Split(','))
            {
                if (!String.IsNullOrEmpty(strIdAr))
                {
                    if (cont == 1) { Cuenta1 = Convert.ToInt32(strIdAr); }
                    if (cont == 2) { Cuenta2 = Convert.ToInt32(strIdAr); }
                    if (cont == 3) { Cuenta3 = Convert.ToInt32(strIdAr); }
                    if (cont == 4) { Cuenta4 = Convert.ToInt32(strIdAr); }
                    if (cont == 5) { Cuenta5 = Convert.ToInt32(strIdAr); }
                    cont++;
                }
            }

            var result = dbc.tktListarTicketsConsolidado(Cuenta1, Cuenta2, Cuenta3, Cuenta4, Cuenta5,
                id, id1, Convert.ToInt32(IdEstado), Convert.ToInt32(IdAsignado), FechaInicio, FechaFin).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaEstadoTicketActivo()
        {
            var query = (from st in dbc.STATUS.Where(x => x.ID_STAT == 1 || x.ID_STAT == 5) select new { st.ID_STAT, st.NAM_STAT }).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaEstadoTicket()
        {
            var query = (from st in dbc.STATUS.Where(x => x.ID_STAT == 1 || x.ID_STAT == 4 || x.ID_STAT == 5 || x.ID_STAT ==6) select new { st.ID_STAT, st.NAM_STAT }).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
    }
}
