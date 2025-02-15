using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using ERPElectrodata.Object;
using ERPElectrodata.Object.Ticket;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.IO;
using ERPElectrodata.Objects;
using System.Web.Security;
using System.Data.Entity.SqlServer;

namespace ERPElectrodata.Controllers
{
    public class FacturacionController : Controller
    {
        //
        // GET: /Facturacion/
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarMantenimientosPorFacturar(int id = 0)
        {
            var query = dbc.ListarMantenimientosPorFacturar(id).ToList();

            var total = query.Count();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult actualizarEstadoFacturacion()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                int IdFacturacion = Convert.ToInt32(Request.Params["IdFacturacion"]);
                int estado = Convert.ToInt32(Request.Params["Estado"]);
                Facturacion objFacturacion = dbc.Facturacions.Where(x => x.Id == IdFacturacion).SingleOrDefault();
                objFacturacion.FechaModifica = DateTime.Now;
                objFacturacion.UsuarioModifica = UserId;
                if (estado == 0)
                {
                    objFacturacion.EstadoFacturado = false;
                    objFacturacion.FechaFacturacion = null;
                }
                else
                {
                    objFacturacion.EstadoFacturado = true;
                    objFacturacion.FechaFacturacion = DateTime.Now;
                }
                dbc.Facturacions.Attach(objFacturacion);
                dbc.Entry(objFacturacion).State = EntityState.Modified;
                dbc.SaveChanges();
                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }
        

    }
}
