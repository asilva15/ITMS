using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using ERPElectrodata.AppCode;
using System.Text;
using ERPElectrodata.Object;
using System.Globalization;
using System.Threading;

using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net.Mime;

namespace ERPElectrodata.Controllers
{
    public class EncuestaConfiguracionController : Controller
    {
        //
        // GET: /ComponenteStockCabecera/ 

        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        private AppLogEntities dbl = new AppLogEntities();

        private string IpServer = "";
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        //private Encryption seg = new Encryption();
        //private AESRijndael seg = new AESRijndael();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexEncuestaConfiguracion()
        {
            return View();
        }

        public ActionResult Reportes()
        {
            return View();
        }

        public ActionResult Graficos()
        {
            return View();
        }

        public ActionResult GraficosMensuales()
        {
            return View();
        }

        public ActionResult Editar(int Id = 0)
        {
            EncuestaConfiguracion objEncuestaConfiguracion = dbe.EncuestaConfiguracions.Where(EC => EC.IdEncuestaConfiguracion == Id).FirstOrDefault();
            var ec = dbe.EncuestaConfiguracions.Where(EC => EC.IdEncuestaConfiguracion == Id).SingleOrDefault();
            var ac = dbc.ACCOUNTs.Where(AC => AC.ID_ACCO == ec.IdAcco).SingleOrDefault();
            ViewBag.Cuenta = ac.NAM_ACCO;
            //cantidadActual = Convert.ToInt32(objComponenteStockCabecera.CantidadTotal); 
            //cantidadDisponible = Convert.ToInt32(objComponenteStockCabecera.CantidadDisponible); 
            return View(objEncuestaConfiguracion);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Editar(EncuestaConfiguracion objEncuestaConfiguracion)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int nroEncuestas = 0;
            Boolean envioDiario = Convert.ToBoolean(objEncuestaConfiguracion.EnvioDiario);
            Boolean envioAutomatico = Convert.ToBoolean(objEncuestaConfiguracion.EnvioAutomatico);

            //Validar campos en blanco
            int flag = 0;
            if (envioDiario == false)
            {
                if (Request.Params["NroEncuestas"] == "")
                {
                    flag = 1;
                }
                else
                {
                    nroEncuestas = Convert.ToInt32(Request.Params["NroEncuestas"]);
                }
            }
            else
            {
                objEncuestaConfiguracion.NroEncuestas = 0;
            }

            if (envioAutomatico == false)
            {
                if (Request.Params["NroEncuestas"] == "0")
                {
                    flag = 1;
                }
                else
                {
                    nroEncuestas = Convert.ToInt32(Request.Params["NroEncuestas"]);
                }
            }
            else
            {
                nroEncuestas = 0;
            }
            

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {

                //int IdAcco = 0;
                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
                }
                
                objEncuestaConfiguracion.NroEncuestas = nroEncuestas;
                objEncuestaConfiguracion.FechaModificacion = DateTime.Now;
                objEncuestaConfiguracion.UsuarioModificacion = IdUser;
                dbe.EncuestaConfiguracions.Attach(objEncuestaConfiguracion);
                dbe.Entry(objEncuestaConfiguracion).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + "" + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }


        public ActionResult Crear(EncuestaConfiguracion objEncuestaConfiguracion)
        {
            Boolean check = Convert.ToBoolean(objEncuestaConfiguracion.EnvioAutomatico);
            Boolean envioDiario = Convert.ToBoolean(objEncuestaConfiguracion.EnvioDiario);
            int idCuenta = 0;
            //Validar campos en blanco 
            int flag = 0;

            if (envioDiario == false)
            {
                if (Convert.ToString(objEncuestaConfiguracion.NroEncuestas) == "")
                {
                    flag = 1;
                }
            }
            else
            {
                objEncuestaConfiguracion.NroEncuestas = 0;
            }
            if (Convert.ToString(objEncuestaConfiguracion.IdAcco) == "")
            {
                flag = 2;
            }
            else
            {
                
            }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
                }

                //Validar que la cuenta no exista como un registro de configuracion  
                idCuenta = Convert.ToInt32(objEncuestaConfiguracion.IdAcco);
                var queryCuenta = dbe.EncuestaConfiguracions.Where(EC => EC.IdAcco == idCuenta && EC.Estado == true);
                int count = queryCuenta.Count();
                if (queryCuenta.Count() == 0)
                {
                    objEncuestaConfiguracion.Estado = true;
                    objEncuestaConfiguracion.EncuestasEnviadas = 0;
                    objEncuestaConfiguracion.FechaCreacion = DateTime.Now;
                    objEncuestaConfiguracion.UsuarioCreacion = IdUser;
                    dbe.EncuestaConfiguracions.Add(objEncuestaConfiguracion);
                    dbe.SaveChanges();

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + "" + "');}window.onload=init;</script>");
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('DOB','0','" + "" + "');}window.onload=init;</script>");
                }
                
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult EncuestaConfiguracionListar(int i = 0)
        {
            int IdAcco = 0;
            int NroEncuestas = 0;
            //Boolean EnvioAutomatico = Convert.ToBoolean(Request.Params["EnvioAutomatico"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            string acc = Request.Params["IdAcco"];
            string nro = Request.Params["NroEncuestas"];
            string env = Request.Params["EnvioAutomatico"];
            var query = dbe.EncuestaConfiguracionListar(0, 0).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerarEncuesta()
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            string tipo = Request.QueryString["tipo"];
            Tickets tic = new Tickets();
            //Si el tipo es diferente de 0, valida si la ultima fecha de envio, de lo contrario omite la validacion
            if (tipo != "0")
            {
                //Validar la fecha última de envío
                var ultimoEnvio = dbe.ValidarFechaUltimaDeEnvio(id).SingleOrDefault();
                var queryEncuestaConfiguracion = dbe.EncuestaConfiguracions.Where(x => x.IdEncuestaConfiguracion == id).SingleOrDefault();

                if (Convert.ToInt32(ultimoEnvio) <= 30 && ultimoEnvio != null)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = dbe.EncuestaConfiguracionEnvioManual(id);
                    string retorna = tic.GeneraEncuesta(id);
                    return Json(retorna, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                var query = dbe.EncuestaConfiguracionEnvioManual(id);
                string retorna = tic.GeneraEncuesta(id);
                return Json(retorna, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult encuestaResumen()
        {
            int IdAcco = 0;
            DateTime FechaInicio = DateTime.Now;
            DateTime FechaFin = DateTime.Now;
            try
            {
                IdAcco = Convert.ToInt32(Request.Params["IdAcco"].ToString());
                FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"].ToString());
                FechaFin = Convert.ToDateTime(Request.Params["FechaFin"].ToString());
            }
            catch
            {
                
            }

            var result = dbe.GraficoResumen(IdAcco,FechaInicio,FechaFin).ToList();

            return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult encuestaPregunta13()
        {
            int IdAcco = 0;
            DateTime FechaInicio = DateTime.Now;
            DateTime FechaFin = DateTime.Now; 
            try
            {
                IdAcco = Convert.ToInt32(Request.Params["IdAcco"].ToString());
                FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"].ToString());
                FechaFin = Convert.ToDateTime(Request.Params["FechaFin"].ToString());
            }
            catch
            {
                
            }
            var result = dbe.GraficoPregunta(IdAcco, FechaInicio, FechaFin, 13).ToList();

            return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult encuestaPregunta14()
        {
            int IdAcco = 0;
            DateTime FechaInicio = DateTime.Now;
            DateTime FechaFin = DateTime.Now;
            try
            { 
                IdAcco = Convert.ToInt32(Request.Params["IdAcco"].ToString());
                FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"].ToString());
                FechaFin = Convert.ToDateTime(Request.Params["FechaFin"].ToString());
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + "Debe completar los campos porfavor" + "');}window.onload=init;</script>");
            }
            var result = dbe.GraficoPregunta(IdAcco, FechaInicio, FechaFin, 14).ToList();

            return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult encuestaPregunta15()
        {
            int IdAcco = 0;
            DateTime FechaInicio = DateTime.Now;
            DateTime FechaFin = DateTime.Now;
            try
            {
                 IdAcco = Convert.ToInt32(Request.Params["IdAcco"].ToString());
                 FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"].ToString());
                 FechaFin = Convert.ToDateTime(Request.Params["FechaFin"].ToString());
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + "Debe completar los campos porfavor" + "');}window.onload=init;</script>");
            }

            var result = dbe.GraficoPregunta(IdAcco, FechaInicio, FechaFin, 15).ToList();

            return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult encuestaResumenMensual()
        {
            int IdAcco = 0;
            int Mes = 0;
            int Anio = 0;
            var ACCO = dbc.ACCOUNTs.Where(a => a.ID_ACCO == IdAcco).SingleOrDefault();
            try
            {
                IdAcco = Convert.ToInt32(Request.Params["IdAcco"].ToString());
                Mes = Convert.ToInt32(Request.Params["Mes"].ToString());
                Anio = Convert.ToInt32(Request.Params["Anio"].ToString());
            }
            catch
            {

            }

            var result = dbe.GraficoResumenMensual(IdAcco, Mes, Anio).ToList();

            return Json(new { pie = result, agree = 330 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult encuestaPreguntaMensual()
        {
            int IdAcco = 0;
            int Mes = 0;
            int Anio = 0;
            int Pregunta = 0;
            try
            {
                IdAcco = Convert.ToInt32(Request.Params["IdAcco"].ToString());
                Mes = Convert.ToInt32(Request.Params["Mes"].ToString());
                Anio = Convert.ToInt32(Request.Params["Anio"].ToString());
                Pregunta = Convert.ToInt32(Request.Params["Pregunta"].ToString());
            }
            catch
            {

            }

            var result = dbe.GraficoPreguntaMensual(IdAcco, Mes, Anio,Pregunta).ToList();

            return Json(new { pie = result, agree = 330 }, JsonRequestBehavior.AllowGet);
        }

    }
}
