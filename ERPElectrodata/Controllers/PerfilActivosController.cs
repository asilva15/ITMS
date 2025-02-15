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
    public class PerfilActivosController : Controller
    {
        //
        // GET: /PerfilActivos/
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPerfilActivo() {
            ViewBag.Administrador = Convert.ToInt32(Session["ADMINISTRADOR"].ToString());
            return View();
        }

        public ActionResult NuevoPerfil()
        {
            return View();
        }

        public ActionResult EditarPerfil(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }

        public JsonResult ListChart(string q, string page)
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            List<ListChart_Result> resultado = dbe.ListChart(termino,IdAcco).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarPerfil(int i = 0)
        {
            //Variables
            int IdCargo = 0;
            string cbTipActPro = "";
            int IdActivoPrograma = 0;
            int IdUser = 0;

            try
            {
                // Validaciones 
                IdCargo = Convert.ToInt32(Request.Params["cbCargo"]);
           cbTipActPro = Convert.ToString(Request.Params["cbTipActPro"]);
           IdUser = Convert.ToInt32(Session["UserId"].ToString());
           string[] idsTipActPro = cbTipActPro.Split(',');

            foreach (string idTActPro in idsTipActPro)
            {
                string[] id = idTActPro.Split('|');
                if (!id[0].Equals("undefined") && (id[0] != ""))
                {
                    string TipoAsignacion = id[0];
                    string ActivoPrograma = id[1];
                    int idActivoPrograma = Convert.ToInt32(ActivoPrograma);
                    int idTipoAsignacion = Convert.ToInt32(TipoAsignacion);
                    var tipoAsignacion = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO_PARA == idTipoAsignacion);
                    var validaActivo = dbc.PerfilActivos.Where(x => x.IdChar == IdCargo && (x.IdTipoActivo == idActivoPrograma || x.IdPrograma == idActivoPrograma)).Count();
                    if (Convert.ToInt32(validaActivo) == 0)
                    {
                        PerfilActivo objPerfilActivo = new PerfilActivo();
                        objPerfilActivo.IdChar = IdCargo;
                        objPerfilActivo.IdAccoPara = idTipoAsignacion;
                        if (tipoAsignacion.VAL_ACCO_PARA == "ACTIVO")
                            objPerfilActivo.IdTipoActivo = idActivoPrograma;
                        else
                            objPerfilActivo.IdPrograma = idActivoPrograma;
                        objPerfilActivo.EstadoRegistro = true;
                        objPerfilActivo.Estado = true;
                        objPerfilActivo.FechaCrea = DateTime.Now;
                        objPerfilActivo.UsuarioCrea = IdUser;
                        dbc.PerfilActivos.Add(objPerfilActivo);
                        dbc.SaveChanges();
                    }
                }
            }
            return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ValidaPerfil()
        {
            int IdCargo = 0;
            string cbTipoAsignacion = "";
            string cbActivoPrograma = "";
            int IdUser = 0;

            //try
            //{
            // Validaciones 
            IdCargo = Convert.ToInt32(Request.Params["cbCargo"]);
            int resultado = 0;
            cbTipoAsignacion = Convert.ToString(Request.Params["cbTipoAsignacion"]);
            cbActivoPrograma = Convert.ToString(Request.Params["cbActivoPrograma"]);
            IdUser = Convert.ToInt32(Session["UserId"].ToString());
            string[] idsTipAct = cbTipoAsignacion.Split(',');
            string[] idsActProg = cbActivoPrograma.Split(',');
            foreach (string idTAsig in idsTipAct)
            {
                foreach (string idActPro in idsActProg)
                {
                    if (!idTAsig.Equals("undefined") && !idActPro.Equals("undefined") && (idTAsig != "" && idActPro != "") || (idTAsig != "null" && idActPro != "null"))
                    {
                        resultado += 1;
                    }
                }
            }
            if (resultado == 0)
                return Content("ERROR");
            else
                return Content("OK");
        }

        public ActionResult ListarPerfilActivos() {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarPerfilActivos(IdAcco).ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarPerfilActivo()
        {
            int IdPerfilActivo = Convert.ToInt32(Request.Params["IdPerfilActivo"]);
            try
            {
                PerfilActivo pa = dbc.PerfilActivos.Where(x => x.Id == IdPerfilActivo).FirstOrDefault();
                pa.Estado = false;

                dbc.Entry(pa).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ActualizarPerfilActivo() {
            try
            {
                int IdPerfil = Convert.ToInt32(Request.Params["IdPerfilActivo"]);
                int IdAccoPara = Convert.ToInt32(Request.Params["IdAccoPara"]);
                int IdActivoPrograma = Convert.ToInt32(Request.Params["IdActivoPrograma"]);
                var tipoAsignacion = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO_PARA == IdAccoPara);
                int IdCargo = Convert.ToInt32(dbc.PerfilActivos.Single(x => x.Id == IdPerfil).IdChar);
                var query = dbc.PerfilActivos.Where(x => x.IdChar == IdCargo && x.IdAccoPara == IdAccoPara && x.Estado == true && x.Id != IdPerfil && (x.IdTipoActivo == IdActivoPrograma || x.IdPrograma == IdActivoPrograma)).Count(); ;
                PerfilActivo pa = dbc.PerfilActivos.Where(x => x.Id == IdPerfil).FirstOrDefault();
                if (query == 1)
                {
                    return Content("ERROR");
                }
                else
                {
                    pa.IdAccoPara = IdAccoPara;
                    if (tipoAsignacion.VAL_ACCO_PARA == "ACTIVO")
                        pa.IdTipoActivo = IdActivoPrograma;
                    else
                        pa.IdPrograma = IdActivoPrograma;
                    dbc.Entry(pa).State = EntityState.Modified;
                    dbc.SaveChanges();
                    return Content("OK");
                }
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ObtenerDatosPerfil(int id = 0)
        {
            var query = dbc.ObtenerDatosPerfilActivo(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
