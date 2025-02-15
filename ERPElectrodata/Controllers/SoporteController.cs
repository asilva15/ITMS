using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class SoporteController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        //
        // GET: /Soporte/
        [Authorize]
        public ActionResult Index()
        {
            bool Acceso = false;
            //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            ViewBag.Permitido = 1;

            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_SERVICEDESK"] == 1)
            {
                Acceso = true;
            }
            if ((int)Session["OPERACIONES"] == 1)
            {
                Acceso = true;
                ViewBag.Permitido = 0;
            }

            //foreach (string rol in rolesArray)
            //{
            //    if (rol == "ADMINISTRADOR" || rol == "ADMIN SERVICE DESK" || rol == "OPERACIONES")
            //    {
            //        Acceso = true; //permitiendo al acceso
            //        if (rol == "OPERACIONES")
            //        {
            //            ViewBag.Permitido = 0;
            //        }
            //    }
            //}
            if (Acceso == true)
            {
                return View();
            }
            else
            {
                return Content("<h3><b>You are not authorized for this section</b></h3>");
            }
        }

        public ActionResult List(int IdComp, string FechaInicio, string FechaFin, string PalabraClave)
        {
            var soporte = dbc.Soportes.ToList();

            var query = (from s in soporte
                         join ce in dbe.CLASS_ENTITY on s.IdCompania equals ce.ID_ENTI
                         select new
                         {
                             IdSoporte = s.Id,
                             s.IdCompania,
                             ce.COM_NAME,
                             CodigoProducto = (s.CodigoProducto == null ? "-" : s.CodigoProducto),
                             Producto = (s.Producto == null ? "-" : s.Producto),
                             Serie = (s.Serie == null ? "-" : s.Serie),
                             FechaFinGarantia = (s.FechaFinGarantia == null ? "-" : s.FechaFinGarantia.ToString()),
                             NroPreventivos = (s.NroPreventivos == null ? "-" : s.NroPreventivos.ToString()),
                             BolsaHoras = (s.BolsaHoras == null ? "-" : s.BolsaHoras.ToString()),
                             Observaciones = (s.Observaciones == null ? "-" : s.Observaciones),
                             Op = (s.OP == null ? "-" : s.OP)
                         }).OrderByDescending(x => x.FechaFinGarantia).Take(200);

            if (IdComp != 0)
            {
                query = query.Where(s => s.IdCompania == IdComp);
            }

            if (FechaInicio != "")
            {
                query = query.Where(s => Convert.ToDateTime(s.FechaFinGarantia == "-" ? "1900-01-01" : s.FechaFinGarantia) >= Convert.ToDateTime(FechaInicio));
            }

            if (FechaFin != "")
            {
                query = query.Where(s => Convert.ToDateTime(s.FechaFinGarantia == "-" ? "1900-01-01" : s.FechaFinGarantia) <= Convert.ToDateTime(FechaFin));
            }

            if (PalabraClave.Trim() != "")
            {
                query = query.Where(s => s.CodigoProducto.ToUpper().Contains(PalabraClave.ToUpper()) || s.Producto.ToUpper().Contains(PalabraClave.ToUpper()) || s.Serie.ToUpper().Contains(PalabraClave.ToUpper()) || s.Observaciones.ToUpper().Contains(PalabraClave.ToUpper()));
            }

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEdit(int id)
        {
            ViewBag.Id = id;

            if (id != 0)
            {
                var sop = dbc.Soportes.Find(id);

                ViewBag.Compania = sop.IdCompania;
                ViewBag.Codigo = sop.CodigoProducto == null ? "-" : sop.CodigoProducto;
                ViewBag.Descripcion = sop.Producto == null ? "-" : sop.Producto;
                ViewBag.Serie = sop.Serie == null ? "-" : sop.Serie;
                ViewBag.FechaFin = sop.FechaFinGarantia  == null ? "" : sop.FechaFinGarantia.ToString();
                ViewBag.Preventivos = sop.NroPreventivos == null ? "0" : sop.NroPreventivos.ToString();
                ViewBag.BolsaHoras = sop.BolsaHoras == null ? "0" : sop.BolsaHoras.ToString();
                ViewBag.Observacion = sop.Observaciones == null ? "-" : sop.Observaciones;
                ViewBag.Op = sop.OP == null ? "-" : sop.OP;
            }
            else
            {
                ViewBag.Preventivos = 0;
                ViewBag.BolsaHoras = 0;
            }
            
            return View();
        }

        public ActionResult Save(int IdSoporte, int IdCompania, string CodigoProd, string ProductoDesc, string SerieProd, string FechaFinProd, int PreventivoProd, int HorasProd, string ObservacionesProd, string Op)
        {
            string resp = "";
            var soporte = new Soporte();

            soporte.Id = IdSoporte;
            soporte.IdCompania = IdCompania;
            soporte.CodigoProducto = CodigoProd;
            soporte.Producto = ProductoDesc;
            soporte.Serie = SerieProd;
            soporte.NroPreventivos = PreventivoProd;
            soporte.BolsaHoras = HorasProd;
            soporte.Observaciones = ObservacionesProd;
            soporte.OP = Op;
            if (!String.IsNullOrEmpty(FechaFinProd))
            {
                soporte.FechaFinGarantia = Convert.ToDateTime(FechaFinProd);
            }

            if (IdSoporte == 0)
            {
                dbc.Soportes.Add(soporte);
            }
            else
            {
                dbc.Soportes.Attach(soporte);
                dbc.Entry(soporte).State = EntityState.Modified;
            }
            dbc.SaveChanges();

            resp = "OK";

            return Json(resp, JsonRequestBehavior.AllowGet);
        }

    }
}
