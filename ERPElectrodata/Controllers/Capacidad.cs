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
using System.Text;

namespace ERPElectrodata.Controllers
{
    public class CapacidadController : Controller
    {
        //
        // GET: /PerfilActivos/
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();

        public ActionResult Index()
        {
            ViewBag.IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.NamAcco = Convert.ToString(Session["NAM_ACCO"]);
            return View();
        }
        public ActionResult ListarGestionCapacidad(int id = 0)
        {
            var query = dbc.ListarAccountQueue(id).ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarCapacidad() {

            int IdAccoQueue = Convert.ToInt32(Request.Params["IdAccoQueue"]);
            int cantTicket = Convert.ToInt32(Request.Params["CantTicket"]);
            int cantProyecto = 0;
            int IdAcco = Convert.ToInt32(Request.Params["IdAcco"]);
            string proyecto = Request.Params["CantProyecto"];
            if(IdAcco == 4)
                cantProyecto = Convert.ToInt32(Request.Params["CantProyecto"]);
            Boolean estado = Convert.ToBoolean(Request.Params["Estado"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            var query = dbc.GuardarCapacidad(IdAccoQueue, cantTicket, cantProyecto, IdAcco, UserId,estado).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CuentaPorDefecto()
        {
            var query = dbc.ListarCuentas(Convert.ToString(Session["NAM_ACCO"])).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidarCantidadTicket_PersAsignado(int id = 0)
        {
            String mensaje = "";
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ValidarCantidadTicket_PersAsignado(id, IdAcco).SingleOrDefault();

            mensaje = Convert.ToString(query.Mensaje);

            return Content(mensaje);
        }

    }
}
