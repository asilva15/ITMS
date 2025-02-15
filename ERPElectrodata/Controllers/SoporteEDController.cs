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
    public class SoporteEDController : Controller
    {
        //
        // GET: /SoporteED/
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();

        public ActionResult RegistroSoporteED(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            //if (dbc.PROYECTO_SLA.Where(x => x.IdProyecto == id && x.Estado == true).Count() == 0)
            //    ViewBag.IdSLA = 1;
            //else
            //    ViewBag.IdSLA = dbc.PROYECTO_SLA.Single(x => x.IdProyecto == id && x.Estado == true).IdSLA;
            return View();
        }
        public ActionResult IndexSoporteED(int id = 0)
        {
            ViewBag.IdDocuSale = id;
            return View();
        }
        public ActionResult EditarSoporteED(int id = 0)
        {

            var objSoporte = dbc.ObtenerDatosSoporteED(id).SingleOrDefault();

            ViewBag.IdDocuSale = objSoporte.IdDocuSale;
            ViewBag.IdSoporteED = id;
            ViewBag.MantPeriodico = Convert.ToInt32(objSoporte.MantPeriodico);
            ViewBag.IdFabricante = objSoporte.IdMarca;
            ViewBag.Fabricante = objSoporte.Marca;
            ViewBag.IdSla = objSoporte.IdSla;
            ViewBag.SLA = objSoporte.Sla;
            ViewBag.MantPrev = objSoporte.MantPrev;
            ViewBag.Frecuencia = objSoporte.Frecuencia;
            //ViewBag.FechaInicio = objSoporte.InicioSoporte;
            //ViewBag.FechaFin = objSoporte.FinSoporte;
            ViewBag.FechaInicio = DateTime.ParseExact(objSoporte.InicioSoporte, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            ViewBag.FechaFin = DateTime.ParseExact(objSoporte.FinSoporte, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            ViewBag.TiempoSoporte = objSoporte.Tiempo;
            ViewBag.BolsaHoras = objSoporte.BolsaHoras;
            ViewBag.Observaciones = objSoporte.Observaciones;

            return View();
        }

        public ActionResult EditarInformeED(int id = 0)
        {
            var objInforme = dbc.ObtenerDatosInformeED(id).SingleOrDefault();


            DateTime fechaInicio = DateTime.ParseExact(objInforme.InicioSoporte, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string fechaInicioFormateada = fechaInicio.ToString("yyyy-MM-dd");

            DateTime fechaFin = DateTime.ParseExact(objInforme.FinSoporte, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string fechaFinFormateada = fechaFin.ToString("yyyy-MM-dd");


            var TieneTicket = dbc.InformeTieneTickets(id).Single().ToString();
            //var src = dbc.InformeImagenes.Where(x => x.IdInforme == id).Single().src;

            ViewBag.IdDocuSale = objInforme.IdDocuSale;
            ViewBag.IdInformeED = id;
            ViewBag.IdFabricante = objInforme.IdMarca;
            ViewBag.Fabricante = objInforme.Marca;
            ViewBag.NroInformes = objInforme.NroInformes;
            ViewBag.Frecuencia = objInforme.Frecuencia;
            ViewBag.FechaInicio = fechaInicioFormateada;
            ViewBag.FechaFin = fechaFinFormateada;
            ViewBag.IdTipoInforme = objInforme.IdTipoInforme;
            ViewBag.TiempoSoporte = objInforme.Tiempo;
            ViewBag.Observaciones = objInforme.Observaciones;
            //INFORME AUTOMATICO
            ViewBag.destinoCorreo = objInforme.destinoCorreo;
            ViewBag.destinoFisico = objInforme.destinoFisico;
            ViewBag.destinoMesapartes = objInforme.destinoMesapartes;
            //ViewBag.src = src;

            ViewBag.TiempoPersonalizado = objInforme.TiempoPersonalizado;
            ViewBag.TipoPlazoEnvio = objInforme.TipoPlazoEnvio;
            ViewBag.DiasEnvio = objInforme.DiasEnvio;
            ViewBag.DiasRestantes = objInforme.DiasRangoIncompleto;
            ViewBag.MostrarDiasRestantes = objInforme.MostrarRangosIncompletos;

            ViewBag.TieneTicket = (TieneTicket == "SI" ? "hidden" : "NO");


            if (objInforme.MostrarRangosIncompletos == "" || objInforme.MostrarRangosIncompletos == null)
            {

                ViewBag.MostrarRangosIncompletos = "";
            }
            else
            {

                ViewBag.MostrarRangosIncompletos = objInforme.MostrarRangosIncompletos;
            }


            //FIN

            return View();
        }

        public ActionResult ObtenerDatosSoporteED(int id = 0)
        {
            var query = dbc.ObtenerDatosSoporteED(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSoporteEDMeses(int id = 0, int id1 = 0)
        {
            var query = dbc.ListarFechasSoporteED(id, id1).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarSLA(string q, string page)
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

            List<ListarSLA_Result> resultado = dbc.ListarSLA(termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuardarSoporteElectrodata(FormCollection collection)
        {
            //Variables
            int idDocuSale = 0;
            decimal tiempoSoporte = 0;
            DateTime FechaInicialSoporte;
            DateTime FechaInicialSoporteNP;
            DateTime FechaFinalSoporte;
            DateTime fechaSoporte;
            int idSla = 0;
            int mantPrev = 0;
            int bolsaHoras = 0;
            int idUser = 0;
            string flag = "";
            int IdManu = 0;
            int periodo1 = 0;
            string valoresPeriodo = "";
            int tiempoMaximo = 0;
            int numeroCombo = 0;
            int idSoporteED = 0;
            string observaciones = "", observacionesInforme = "";
            FechaInicialSoporte = Convert.ToDateTime(null);
            FechaInicialSoporteNP = Convert.ToDateTime(null);
            FechaFinalSoporte = Convert.ToDateTime(null);
            fechaSoporte = Convert.ToDateTime(null);

            string FechaInicialSoporteString = Request.Params["dtIniSoporte"].ToString();
            string FechaInicialSoporteNPString = Request.Params["dtIniSoporte"].ToString();
            string FechaFinalSoporteString = Request.Params["dtFinSoporte"].ToString();

            string cadenaFecha = ""/*, fechaSoporte = ""*/;

            try
            {
                idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                idUser = Convert.ToInt32(Session["UserId"].ToString());

                string checkPeriodico = Convert.ToString(Request.Params["chkMantPreiodico"]);
                string frecuencia = Convert.ToString(Request.Params["cbFrecuenciaSoporte"]);

                observaciones = Convert.ToString(Request.Params["txtObservaciones"]).Replace("/\n|\r|\n\r/g", "");

                if (dbc.PROYECTO_SLA.Where(x => x.IdProyecto == idDocuSale && x.TiempoRespuesta == null && x.TiempoAtencion == null && x.TiempoResolucion == null).Count() == 0)
                {

                    SLA slaEstandar = new SLA();
                    slaEstandar.Nombre = "SLA ESTANDAR";
                    slaEstandar.FechaCrea = DateTime.Now;
                    slaEstandar.UsuarioCrea = 34;
                    slaEstandar.Estado = true;
                    slaEstandar.IdCuenta = 4;
                    dbc.SLAs.Add(slaEstandar);
                    dbc.SaveChanges();

                    SLADetalle sladetalleEstandar1 = new SLADetalle();
                    sladetalleEstandar1.IdSLA = slaEstandar.Id;
                    sladetalleEstandar1.IdPrioridad = 1;
                    sladetalleEstandar1.TiempoAtencion = 4;
                    sladetalleEstandar1.TiempoResolucionMinutos = 240;
                    dbc.SLADetalles.Add(sladetalleEstandar1);
                    dbc.SaveChanges();

                    SLADetalle sladetalleEstandar2 = new SLADetalle();
                    sladetalleEstandar2.IdSLA = slaEstandar.Id;
                    sladetalleEstandar2.IdPrioridad = 2;
                    sladetalleEstandar2.TiempoAtencion = 8;
                    sladetalleEstandar2.TiempoResolucionMinutos = 480;
                    dbc.SLADetalles.Add(sladetalleEstandar2);
                    dbc.SaveChanges();

                    SLADetalle sladetalleEstandar3 = new SLADetalle();
                    sladetalleEstandar3.IdSLA = slaEstandar.Id;
                    sladetalleEstandar3.IdPrioridad = 3;
                    sladetalleEstandar3.TiempoAtencion = 24;
                    sladetalleEstandar3.TiempoResolucionMinutos = 1440;
                    dbc.SLADetalles.Add(sladetalleEstandar3);
                    dbc.SaveChanges();

                    SLADetalle sladetalleEstandar4 = new SLADetalle();
                    sladetalleEstandar4.IdSLA = slaEstandar.Id;
                    sladetalleEstandar4.IdPrioridad = 4;
                    sladetalleEstandar4.TiempoAtencion = 48;
                    sladetalleEstandar4.TiempoResolucionMinutos = 2880;
                    dbc.SLADetalles.Add(sladetalleEstandar4);
                    dbc.SaveChanges();

                    SLADetalle sladetalleEstandar5 = new SLADetalle();
                    sladetalleEstandar5.IdSLA = slaEstandar.Id;
                    sladetalleEstandar5.IdPrioridad = 5;
                    sladetalleEstandar5.TiempoAtencion = 0;
                    sladetalleEstandar5.TiempoResolucionMinutos = 0;
                    dbc.SLADetalles.Add(sladetalleEstandar5);
                    dbc.SaveChanges();


                    PROYECTO_SLA psla = new PROYECTO_SLA();

                    psla.IdProyecto = idDocuSale;
                    psla.IdSLA = slaEstandar.Id;
                    psla.Estado = true;
                    psla.UsuarioCrea = 34;
                    psla.FechaCrea = DateTime.Now;

                    dbc.PROYECTO_SLA.Add(psla);
                    dbc.SaveChanges();

                }



                if (checkPeriodico == "on")
                {
                    //Validaciones
                    //if (Request.Params["cbFabricanteED"] == "")
                    //{
                    //    flag = flag + "- Seleccione el fabricante.</br>";
                    //}
                    //else
                    //{
                    //    IdManu = Convert.ToInt32(Request.Params["cbFabricanteED"]);
                    //}
                    //if (Convert.ToString(Request.Params["cbSLA"]) == null) 
                    //if (Request.Params["idCboSLA"] == "")
                    //{
                    //    flag = flag + "- Seleccione el SLA.</br>";
                    //}
                    //else
                    //{
                    //    idSla = Convert.ToInt32(Request.Params["idCboSLA"]);
                    //}
                    //if (dbc.PROYECTO_SLA.Where(x => x.IdProyecto == idDocuSale && x.IdTipoTicket == 1).Count() == 0)
                    //{
                    //    flag = flag + "- Debes agregar por lo menos un SLA de tipo Incidencia.</br>";
                    //}
                    //if (dbc.PROYECTO_SLA.Where(x => x.IdProyecto == idDocuSale && x.IdTipoTicket == 2).Count() == 0)
                    //{
                    //    flag = flag + "- Debes agregar por lo menos un SLA de tipo Requerimiento.</br>";
                    //}
                    if (!string.IsNullOrEmpty(FechaInicialSoporteString))
                    {
                        FechaInicialSoporte = DateTime.ParseExact(FechaInicialSoporteString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        flag = flag + "- Seleccione el inicio del soporte.</br>";
                        //FechaInicialSoporte = Convert.ToDateTime(Request.Params["dtIniSoporte"].ToString());
                    }
                    if (!string.IsNullOrEmpty(FechaFinalSoporteString)) /*FechaFinalSoporteString*/
                    {
                        FechaFinalSoporte = DateTime.ParseExact(FechaFinalSoporteString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //flag = flag + "- Seleccione el fin del soporte.</br>";
                    }
                    else
                    {
                        flag = flag + "- Seleccione el fin del soporte.</br>";
                        //FechaFinalSoporte = Convert.ToDateTime(Request.Params["dtFinSoporte"].ToString());
                    }
                    if (Request.Params["txtTiempoSoporte"] == "")
                    {
                        flag = flag + "- Ingrese el tiempo del soporte.</br>";
                    }
                    else
                    {
                        tiempoSoporte = Convert.ToInt32(Request.Params["txtTiempoSoporte"]);
                    }
                    if (Request.Params["txtMantPreventivo"] == "")
                    {
                        flag = flag + "- Ingrese el nro de mantenimientos.</br>";
                    }
                    else
                    {
                        mantPrev = Convert.ToInt32(Request.Params["txtMantPreventivo"]);
                    }
                    if (Request.Params["txtBolsaHoras"] == "")
                    {
                        flag = flag + "- Ingrese la bolsa de horas</br>";
                    }
                    else
                    {
                        bolsaHoras = Convert.ToInt32(Request.Params["txtBolsaHoras"]);
                    }

                    //finSoporte = Convert.ToDateTime(dbc.CalcularFinSoporte(idDocuSale, tiempoSoporte).SingleOrDefault().FinSoporte);

                    if (flag != "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('SoporteED','" + flag + "');}window.onload=init;</script>");
                    }
                    //Validaciones Fechas
                    for (int i = 1; i <= mantPrev; i++)
                    {
                        string stringFecha = "";
                        cadenaFecha = "dtSoporte" + i;
                        stringFecha = Convert.ToString(Request.Params[cadenaFecha]);
                        //fechaSoporte = DateTime.ParseExact(stringFecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //fechaSoporte = Convert.ToString(Request.Params[cadenaFecha]);

                        if (stringFecha == null)
                        {
                            flag = "Por favor, genere las fechas de los mantenimientos.";
                            return Content("<script type='text/javascript'> function init() {" +
                                               "if(top.MensajeConfirmacion) top.MensajeConfirmacion('SoporteED','" + flag + "');}window.onload=init;</script>");
                        }
                    }

                    //validacion marca:
                    string[] selectedFabricantes = collection["SelectedFabricante"].Split(',');

                    var fabricantesDiferentes = selectedFabricantes.Distinct().ToList();

                    if (fabricantesDiferentes.Count() == 0 && string.IsNullOrEmpty(fabricantesDiferentes[0]) || (fabricantesDiferentes.Count() == 1 && fabricantesDiferentes[0] == ""))
                    {
                        flag = "-Seleccione la marca";
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
                    }

                    if (flag != "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
                    }


                    //Cambiando Estado Soporte ED
                    var objProyecto = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == idDocuSale);
                    objProyecto.EstadoSoporteED = true;
                    dbc.Entry(objProyecto).State = EntityState.Modified;
                    dbc.SaveChanges();
                    //dbc.CambiarEstadoSoporteED(idDocuSale, 1);


                    bool existe = dbc.SoporteEDs.Any(x => x.IdDocuSale == idDocuSale && x.IdManu == IdManu && x.MantenimientoPrev == mantPrev && x.FinSoporte == FechaFinalSoporte && x.BolsaHoras == bolsaHoras && x.Observaciones == observaciones && x.TiempoSoporte == tiempoSoporte && x.UsuarioCrea == idUser && x.Frecuencia == frecuencia);

                    if (!existe)
                    {
                        if (fabricantesDiferentes.Count() > 0)
                        {
                            var existingAdicionales = dbc.MarcasdeSoporteED.Where(pa => pa.IdSoporteED == idSoporteED).ToList();

                            dbc.MarcasdeSoporteED.RemoveRange(existingAdicionales);

                            foreach (var FabricantesId in fabricantesDiferentes)
                            {
                                if (FabricantesId != "")
                                {
                                    SoporteED objSoporteED = new SoporteED();
                                    objSoporteED.IdDocuSale = idDocuSale;
                                    objSoporteED.MantPeriodico = true;
                                    objSoporteED.IdManu = Convert.ToInt32(FabricantesId);
                                    objSoporteED.IdSLA = 1;
                                    objSoporteED.MantenimientoPrev = mantPrev;
                                    objSoporteED.InicioSoporte = FechaInicialSoporte;
                                    objSoporteED.FinSoporte = FechaFinalSoporte;
                                    objSoporteED.BolsaHoras = bolsaHoras;
                                    objSoporteED.Observaciones = observaciones;
                                    //if (frecuencia == "D")
                                    //{
                                    //    tiempoSoporte = Convert.ToDecimal(tiempoSoporte / 30);
                                    //}
                                    objSoporteED.TiempoSoporte = tiempoSoporte;
                                    objSoporteED.FechaCrea = DateTime.Now;
                                    objSoporteED.UsuarioCrea = idUser;
                                    objSoporteED.Estado = true;
                                    objSoporteED.Frecuencia = frecuencia;
                                    dbc.SoporteEDs.Add(objSoporteED);
                                    dbc.SaveChanges();

                                    //Registrando las fechas de los soportes
                                    for (int i = 1; i <= mantPrev; i++)
                                    {
                                        cadenaFecha = "dtSoporte" + i;
                                        fechaSoporte = DateTime.ParseExact(Convert.ToString(Request.Params[cadenaFecha]), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        //fechaSoporte = ;

                                        SoporteEDDetalle objSoporteEDet = new SoporteEDDetalle();
                                        objSoporteEDet.IdSoporteED = objSoporteED.IdSoporteED;
                                        objSoporteEDet.IdDocuSale = idDocuSale;
                                        objSoporteEDet.FechaMantenimiento = Convert.ToDateTime(fechaSoporte);
                                        objSoporteEDet.FechaCrea = DateTime.Now;
                                        objSoporteEDet.UsuarioCrea = idUser;
                                        objSoporteEDet.Estado = true;
                                        dbc.SoporteEDDetalles.Add(objSoporteEDet);
                                        dbc.SaveChanges();
                                    }

                                    //creando marcas
                                    var Marca = new MarcasdeSoporteED
                                    {
                                        IdDocuSale = idDocuSale,
                                        IdSoporteED = objSoporteED.IdSoporteED,
                                        IdManu = Convert.ToInt32(FabricantesId)// Usa AdicionalId como el ID del formato seleccionado

                                    };

                                    dbc.MarcasdeSoporteED.Add(Marca);
                                    dbc.SaveChanges();
                                }
                                else
                                {

                                }


                            }

                        }
                    }

                    //dbc.AgregarFechaMantenimientoEDFecIni(idDocuSale, objSoporteED.IdSoporteED, FechaInicialSoporte, objSoporteED.FinSoporte, mantPrev, idUser);

                }
                else
                {
                    //if (Request.Params["idCboSLA"] == "")
                    //{
                    //    flag = flag + "- Seleccione el SLA.</br>";
                    //}
                    //else
                    //{
                    //    idSla = Convert.ToInt32(Request.Params["idCboSLA"]);
                    //}
                    if (Request.Params["txtBolsaHoras"] == "")
                    {
                        flag = flag + "- Ingrese la bolsa de horas</br>";
                    }
                    else
                    {
                        bolsaHoras = Convert.ToInt32(Request.Params["txtBolsaHoras"]);
                    }
                    if (!string.IsNullOrEmpty(FechaInicialSoporteNPString))
                    {
                        FechaInicialSoporteNP = DateTime.ParseExact(FechaInicialSoporteNPString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        flag = flag + "- Seleccione la fecha de inicio.</br>";
                        //FechaInicialSoporteNP = Convert.ToDateTime(Request.Params["dtIniSoporte"]);
                    }

                    if (!string.IsNullOrEmpty(FechaFinalSoporteString)) /*FechaFinalSoporteString*/
                    {
                        FechaFinalSoporte = DateTime.ParseExact(FechaFinalSoporteString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //flag = flag + "- Seleccione el fin del soporte.</br>";
                    }
                    else
                    {
                        flag = flag + "- Seleccione el fin del soporte.</br>";
                    }
                    if (Request.Params["txtTiempoSoporte"] == "")
                    {
                        flag = flag + "- Ingrese el tiempo del soporte.</br>";
                    }
                    else
                    {
                        tiempoSoporte = Convert.ToInt32(Request.Params["txtTiempoSoporte"]);
                    }
                    //if (Request.Params["txtMantPreventivo"] == "")
                    //{
                    //    flag = flag + "- Ingrese el nro de mantenimientos.</br>";
                    //}
                    //else
                    //{
                    //    mantPrev = Convert.ToInt32(Request.Params["txtMantPreventivo"]);
                    //}
                    //if (Request.Params["cbFecha1"] == "" || Request.Params["cbFecha1"] == null)
                    //{
                    //    flag = flag + "- Seleccione las fechas de los mantenimientos.</br>";
                    //}
                    //else
                    //{
                    //    periodo1 = Convert.ToInt32(Request.Params["cbFecha1"]);
                    //}


                    //validacion marca:
                    string[] selectedFabricantes = collection["SelectedFabricante"].Split(',');

                    var fabricantesDiferentes = selectedFabricantes.Distinct().ToList();

                    if (fabricantesDiferentes.Count() == 0 && string.IsNullOrEmpty(fabricantesDiferentes[0]) || (fabricantesDiferentes.Count() == 1 && fabricantesDiferentes[0] == ""))
                    {
                        flag = "-Seleccione la marca";
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
                    }

                    if (flag != "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('SoporteED','" + flag + "');}window.onload=init;</script>");
                    }

                    //Cambiando Estado Soporte ED
                    var objProyecto = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == idDocuSale);
                    objProyecto.EstadoSoporteED = true;
                    dbc.Entry(objProyecto).State = EntityState.Modified;
                    dbc.SaveChanges();

                    numeroCombo = Convert.ToInt32(Request.Params["NroCombo"]);

                    //Validando los combos
                    //for (int i = 1; i <= numeroCombo; i++)
                    //{
                    //    if (Convert.ToString(Request.Params["cbFecha" + i]) == null)
                    //    {
                    //        flag = "Por favor, seleccione los tiempos para los mantenimientos.";
                    //        return Content("<script type='text/javascript'> function init() {" +
                    //                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('SoporteED','" + flag + "');}window.onload=init;</script>");
                    //    }
                    //}


                    // Calcular el tiempo del soporte

                    //string periodo = "cbFecha" + numeroCombo;
                    //int tiempoTotalSoporte = Convert.ToInt32(Request.Params[periodo]);

                    ////Calculando el Tiempo de Soporte Maximo
                    //for (int i = 1; i <= numeroCombo; i++)
                    //{
                    //    valoresPeriodo = "cbFecha" + i;
                    //    tiempoMaximo = Convert.ToInt32(Request.Params[valoresPeriodo]);
                    //    if (tiempoMaximo > tiempoTotalSoporte)
                    //    {
                    //        tiempoTotalSoporte = tiempoMaximo;
                    //    }
                    //}
                    // tiempoSoporte = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO_PARA == idPeriodo).SingleOrDefault().VAL_ACCO_PARA);
                    // //Fin de soporte
                    //FechaFinalSoporte = Convert.ToDateTime(dbc.CalcularFinSoporte(idDocuSale, tiempoSoporte).SingleOrDefault().FinSoporte);
                    //FechaFinalSoporte = FechaInicialSoporteNP.AddMonths(tiempoTotalSoporte);
                    // Verificar si el registro ya existe
                    bool existe = dbc.SoporteEDs.Any(x => x.IdDocuSale == idDocuSale && x.IdManu == IdManu && x.IdSLA == idSla && x.FinSoporte == FechaFinalSoporte && x.BolsaHoras == bolsaHoras && x.Observaciones == observaciones && x.TiempoSoporte == tiempoSoporte && x.UsuarioCrea == idUser);

                    if (!existe)
                    {
                        if (fabricantesDiferentes.Count() > 0)
                        {
                            var existingAdicionales = dbc.MarcasdeSoporteED.Where(pa => pa.IdSoporteED == idSoporteED).ToList();

                            dbc.MarcasdeSoporteED.RemoveRange(existingAdicionales);

                            foreach (var FabricantesId in fabricantesDiferentes)
                            {
                                if (FabricantesId != "")
                                {
                                    SoporteED objSoporteED = new SoporteED();
                                    objSoporteED.IdDocuSale = idDocuSale;
                                    objSoporteED.MantPeriodico = true;
                                    objSoporteED.IdManu = Convert.ToInt32(FabricantesId);
                                    objSoporteED.IdSLA = 1;
                                    objSoporteED.MantenimientoPrev = mantPrev;
                                    objSoporteED.InicioSoporte = FechaInicialSoporteNP;
                                    objSoporteED.FinSoporte = FechaFinalSoporte;
                                    objSoporteED.BolsaHoras = bolsaHoras;
                                    objSoporteED.Observaciones = observaciones;
                                    //if (frecuencia == "D")
                                    //{
                                    //    tiempoSoporte = Convert.ToDecimal(tiempoSoporte / 30);
                                    //}
                                    objSoporteED.TiempoSoporte = tiempoSoporte;
                                    //objSoporteED.TiempoSoporte = tiempoTotalSoporte;
                                    objSoporteED.FechaCrea = DateTime.Now;
                                    objSoporteED.UsuarioCrea = idUser;
                                    objSoporteED.Estado = true;
                                    objSoporteED.Frecuencia = frecuencia;
                                    dbc.SoporteEDs.Add(objSoporteED);
                                    dbc.SaveChanges();

                                    //Registrando las fechas de los soportes
                                    for (int i = 1; i <= mantPrev; i++)
                                    {
                                        cadenaFecha = "dtSoporte" + i;
                                        fechaSoporte = DateTime.ParseExact(Convert.ToString(Request.Params[cadenaFecha]), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        //fechaSoporte = ;

                                        SoporteEDDetalle objSoporteEDet = new SoporteEDDetalle();
                                        objSoporteEDet.IdSoporteED = objSoporteED.IdSoporteED;
                                        objSoporteEDet.IdDocuSale = idDocuSale;
                                        objSoporteEDet.FechaMantenimiento = Convert.ToDateTime(fechaSoporte);
                                        objSoporteEDet.FechaCrea = DateTime.Now;
                                        objSoporteEDet.UsuarioCrea = idUser;
                                        objSoporteEDet.Estado = true;
                                        dbc.SoporteEDDetalles.Add(objSoporteEDet);
                                        dbc.SaveChanges();
                                    }

                                    //creando marcas
                                    var Marca = new MarcasdeSoporteED
                                    {
                                        IdDocuSale = idDocuSale,
                                        IdSoporteED = objSoporteED.IdSoporteED,
                                        IdManu = Convert.ToInt32(FabricantesId)// Usa AdicionalId como el ID del formato seleccionado

                                    };

                                    dbc.MarcasdeSoporteED.Add(Marca);
                                    dbc.SaveChanges();
                                }
                                else
                                {

                                }


                            }

                        }
                    }


                }

                return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('OK','');}window.onload=init;</script>");
            }
            catch (Exception e)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('ERROR','');}window.onload=init;</script>");
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditarSoporteElectrodata(FormCollection collection)
        {
            //Variables
            decimal tiempoSoporte = 0;
            DateTime FechaInicialSoporte;
            DateTime FechaInicialSoporteNP;
            DateTime FechaFinalSoporte;
            DateTime fechaSoporte;
            int idSla = 0;
            int mantPrev = 0;
            int bolsaHoras = 0;
            int idUser = 0;
            string flag = "";
            int IdManu = 0;
            string observaciones = "";
            FechaInicialSoporte = Convert.ToDateTime(null);
            FechaInicialSoporteNP = Convert.ToDateTime(null);
            FechaFinalSoporte = Convert.ToDateTime(null);
            fechaSoporte = Convert.ToDateTime(null);

            string FechaInicialSoporteString = Request.Params["dtIniSoporteED"].ToString();
            string FechaInicialSoporteNPString = Request.Params["dtIniSoporteED"].ToString();
            string FechaFinalSoporteString = Request.Params["dtFinSoporteED"].ToString();

            string cadenaFecha = ""/*, fechaSoporte = ""*/;

            try
            {
                idUser = Convert.ToInt32(Session["UserId"].ToString());

                int IdSoporteED = Convert.ToInt32(Request.Params["IdSoporteED"]);
                string checkPeriodico = Convert.ToString(Request.Params["MantPeriodico"]);
                string frecuencia = Convert.ToString(Request.Params["cbFrecuenciaSoporteED"]);

                observaciones = Convert.ToString(Request.Params["txtObservacionesED"]).Replace("/\n|\r|\n\r/g", "");

                if (checkPeriodico == "1")
                {
                    //Validaciones
                    if (Convert.ToString(Request.Params["cbFabricanteElectrodata"]) == null)
                    {
                        flag = flag + "- Seleccione el fabricante.</br>";
                    }
                    else
                    {
                        var fabricante = Request.Params["cbFabricanteElectrodata"];
                        if (int.TryParse(fabricante, out int result))
                        {
                            IdManu = result;
                        }
                        else
                        {
                            var query = dbc.MANUFACTURERs.Where(x => x.NAM_MANU == fabricante).FirstOrDefault();
                            IdManu = query?.ID_MANU ?? 0; // Si query es nulo, asigna 0 como valor predeterminado
                        }

                    }
                    if (Convert.ToString(Request.Params["idCboSLAED"]) == null)
                    {
                        flag = flag + "- Seleccione el SLA.</br>";
                    }
                    else
                    {
                        idSla = Convert.ToInt32(Request.Params["idCboSLAED"]);
                    }
                    if (!string.IsNullOrEmpty(FechaInicialSoporteString))
                    {
                        FechaInicialSoporte = DateTime.ParseExact(FechaInicialSoporteString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //flag = flag + "- Seleccione el inicio del soporte.</br>";
                    }
                    else
                    {
                        flag = flag + "- Seleccione el inicio del soporte.</br>";
                    }
                    if (!string.IsNullOrEmpty(FechaFinalSoporteString))
                    {
                        FechaFinalSoporte = DateTime.ParseExact(FechaFinalSoporteString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //flag = flag + "- Seleccione el fin del soporte.</br>";
                    }
                    else
                    {
                        flag = flag + "- Seleccione el fin del soporte.</br>";
                        //FechaFinalSoporte = Convert.ToDateTime(Request.Params["dtFinSoporteED"].ToString());
                    }
                    if (Request.Params["txtTiempoSoporteED"] == "")
                    {
                        flag = flag + "- Ingrese el tiempo del soporte.</br>";
                    }
                    else
                    {
                        tiempoSoporte = Convert.ToInt32(Request.Params["txtTiempoSoporteED"]);
                    }
                    if (Request.Params["txtBolsaHorasED"] == "")
                    {
                        flag = flag + "- Ingrese la bolsa de horas</br>";
                    }
                    else
                    {
                        bolsaHoras = Convert.ToInt32(Request.Params["txtBolsaHorasED"]);
                    }
                    if (flag != "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('SoporteED','" + flag + "');}window.onload=init;</script>");
                    }
                    //Validaciones Fechas
                    for (int i = 1; i <= mantPrev; i++)
                    {
                        cadenaFecha = "dtSoporte" + i;
                        fechaSoporte = DateTime.ParseExact(Convert.ToString(Request.Params[cadenaFecha]), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (fechaSoporte == null)
                        {
                            flag = "Por favor, genere las fechas de los mantenimientos.";
                            return Content("<script type='text/javascript'> function init() {" +
                                               "if(top.MensajeConfirmacion) top.MensajeConfirmacion('SoporteED','" + flag + "');}window.onload=init;</script>");
                        }
                    }

                    List<ObtenerSoporteEDDetalle_Result> listaSoporte = dbc.ObtenerSoporteEDDetalle(IdSoporteED).ToList();

                    foreach (ObtenerSoporteEDDetalle_Result dr in listaSoporte)
                    {
                        cadenaFecha = "dtSoporte" + dr.IdMant;
                        fechaSoporte = DateTime.ParseExact(Convert.ToString(Request.Params[cadenaFecha]), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (fechaSoporte == null)
                        {
                            flag = "Por favor, seleccione las fechas de los mantenimientos.";
                            return Content("<script type='text/javascript'> function init() {" +
                                               "if(top.MensajeConfirmacion) top.MensajeConfirmacion('SoporteED','" + flag + "');}window.onload=init;</script>");
                        }
                    }

                    SoporteED objSoporteED = dbc.SoporteEDs.Where(x => x.IdSoporteED == IdSoporteED).FirstOrDefault();
                    objSoporteED.IdManu = IdManu;
                    objSoporteED.IdSLA = idSla;
                    objSoporteED.InicioSoporte = FechaInicialSoporte;
                    objSoporteED.FinSoporte = FechaFinalSoporte;
                    objSoporteED.BolsaHoras = bolsaHoras;
                    objSoporteED.Observaciones = observaciones;
                    //if (frecuencia == "D")
                    //{
                    //    tiempoSoporte = Convert.ToDecimal(tiempoSoporte / 30);
                    //}
                    objSoporteED.TiempoSoporte = tiempoSoporte;
                    objSoporteED.Frecuencia = frecuencia;
                    objSoporteED.FechaModifica = DateTime.Now;
                    objSoporteED.UsuarioModifica = idUser;
                    dbc.Entry(objSoporteED).State = EntityState.Modified;
                    dbc.SaveChanges();

                    //Registrando las fechas de los mantenimientos
                    foreach (ObtenerSoporteEDDetalle_Result dr in listaSoporte)
                    {
                        cadenaFecha = "dtSoporte" + dr.IdMant;
                        fechaSoporte = DateTime.ParseExact(Convert.ToString(Request.Params[cadenaFecha]), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        SoporteEDDetalle objSoporteEDet = dbc.SoporteEDDetalles.Where(x => x.IdMantED == dr.IdMant).FirstOrDefault();
                        objSoporteEDet.FechaMantenimiento = Convert.ToDateTime(fechaSoporte);
                        objSoporteEDet.FechaModifica = DateTime.Now;
                        objSoporteEDet.UsuarioModifica = idUser;
                        dbc.Entry(objSoporteEDet).State = EntityState.Modified;
                        dbc.SaveChanges();
                    }

                }
                else
                {
                    // Validaciones
                    if (Convert.ToString(Request.Params["cbFabricanteElectrodata"]) == null)
                    {
                        flag = flag + "- Seleccione el fabricante.</br>";
                    }
                    else
                    {
                        var fabricante = Request.Params["cbFabricanteElectrodata"];
                        if (int.TryParse(fabricante, out int result))
                        {
                            IdManu = result;
                        }
                        else
                        {
                            var query = dbc.MANUFACTURERs.Where(x => x.NAM_MANU == fabricante).FirstOrDefault();
                            IdManu = query?.ID_MANU ?? 0; // Si query es nulo, asigna 0 como valor predeterminado
                        }

                    }

                    if (Convert.ToString(Request.Params["idCboSLAED"]) == null)
                    {
                        flag = flag + "- Seleccione el SLA.</br>";
                    }
                    else
                    {
                        idSla = Convert.ToInt32(Request.Params["idCboSLAED"]);
                    }

                    if (Request.Params["txtBolsaHorasED"] == "")
                    {
                        flag = flag + "- Ingrese la bolsa de horas</br>";
                    }
                    else
                    {
                        bolsaHoras = Convert.ToInt32(Request.Params["txtBolsaHorasED"]);
                    }
                    if (!string.IsNullOrEmpty(FechaInicialSoporteNPString))
                    {
                        FechaInicialSoporteNP = DateTime.ParseExact(FechaInicialSoporteNPString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        flag = flag + "- Seleccione la fecha de inicio.</br>";
                        //FechaInicialSoporteNP = Convert.ToDateTime(Request.Params["dtIniSoporteED"]);
                    }
                    if (Request.Params["dtFinSoporteED"] == "")
                    {
                        flag = flag + "- Seleccione la fecha de fin.</br>";
                        //FechaInicialSoporteNP = Convert.ToDateTime(Request.Params["dtIniSoporteED"]);

                    }
                    else
                    {
                        FechaFinalSoporte = DateTime.ParseExact(FechaFinalSoporteString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    if (flag != "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('SoporteED','" + flag + "');}window.onload=init;</script>");
                    }

                    SoporteED objSoporteED = dbc.SoporteEDs.Where(x => x.IdSoporteED == IdSoporteED).FirstOrDefault();
                    objSoporteED.IdManu = IdManu;
                    objSoporteED.IdSLA = idSla;
                    objSoporteED.BolsaHoras = bolsaHoras;
                    objSoporteED.Observaciones = observaciones;
                    objSoporteED.FechaModifica = DateTime.Now;
                    objSoporteED.UsuarioModifica = idUser;
                    objSoporteED.InicioSoporte = FechaInicialSoporteNP;
                    objSoporteED.FinSoporte = FechaFinalSoporte;
                    objSoporteED.Frecuencia = frecuencia;
                    objSoporteED.TiempoSoporte = tiempoSoporte;
                    dbc.Entry(objSoporteED).State = EntityState.Modified;
                    dbc.SaveChanges();

                }

                return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('OK','');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('ERROR','');}window.onload=init;</script>");
            }
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult EditarInformeED(FormCollection collection, string[] imagenesData, string[] titulosData, string[] comentariosData)
        {
            //Variables
            int idUser = 0;
            string flag = "";
            string observacionesInforme = "";

            int idDocuSale = 0;

            int MarcaInforme = 0, IdTipoInforme = 0;
            DateTime FechaInicialInforme = Convert.ToDateTime(null);
            DateTime FechaFinalInforme = Convert.ToDateTime(null);
            int tiempoInforme = 0;
            int NroInformes = 0, txtnumerofrecuenciapersonalizada = 0, txtdiashabiles = 0, diasRestantes = 0;
            string cadenaFecha = "", cadenaFechafin = "", cadenaFechamaximo = "";
            DateTime fechaInforme = Convert.ToDateTime(null);
            DateTime fechaInformefin = Convert.ToDateTime(null);
            DateTime fechaInformeplanificada = Convert.ToDateTime(null);
            DateTime fechaInformemaximo = Convert.ToDateTime(null);

            string txtinformesid = "", txtgestionid = "", txttablaid = "";

            try
            {
                idUser = Convert.ToInt32(Session["UserId"].ToString());
                int IdInformeED = Convert.ToInt32(Request.Params["IdInformeED"]);
                string frecuencia = Convert.ToString(Request.Params["cbFrecuenciaInforme"]);
                observacionesInforme = Convert.ToString(Request.Params["txtObservacionesInforme"]).Replace("/\n|\r|\n\r/g", "");

                idDocuSale = Convert.ToInt32(Request.Params["IdDocuSaleED"]);

                //if (Convert.ToString(Request.Params["cbFabricanteInforme"]) == null)
                //{
                //    flag = "- Seleccione el fabricante.</br>";
                //}
                //else
                //{
                //    MarcaInforme = Convert.ToInt32(Request.Params["cbFabricanteInforme"]);
                //}

                if (Convert.ToString(Request.Params["cbTipoInforme"]) == null || Convert.ToString(Request.Params["cbTipoInforme"]) == "")
                {
                    flag = flag + "- Seleccione el Tipo de Informe.</br>";
                }
                else
                {
                    IdTipoInforme = Convert.ToInt32(Request.Params["cbTipoInforme"]);
                }

                if (Request.Params["FInicio"] == "")
                {
                    flag = flag + "- Seleccione la fecha de inicio.</br>";
                }
                else
                {
                    FechaInicialInforme = Convert.ToDateTime(Request.Params["FInicio"].ToString());
                }
                if (Request.Params["FFin"] == "")
                {
                    flag = flag + "- Seleccione la fecha de fin.</br>";
                }
                else
                {
                    FechaFinalInforme = Convert.ToDateTime(Request.Params["FFin"].ToString());
                }
                if (Request.Params["txtTiempoInforme"] == "")
                {
                    flag = flag + "- Ingrese el tiempo.</br>";
                }
                else
                {
                    tiempoInforme = Convert.ToInt32(Request.Params["txtTiempoInforme"]);
                }
                if (Request.Params["txtNroInformes"] == "")
                {
                    flag = flag + "- Ingrese el nro de informes.</br>";
                }
                else
                {
                    NroInformes = Convert.ToInt32(Request.Params["txtNroInformes"]);
                }

                //INFORME AUTOMATICO
                if (string.IsNullOrEmpty(Convert.ToString(Request.Params["txtinformesid"])))
                {
                    txtinformesid = "";
                }
                else
                {
                    txtinformesid = Convert.ToString(Request.Params["txtinformesid"]);
                }



                if (string.IsNullOrEmpty(Convert.ToString(Request.Params["txtgestionid"])))
                {
                    txtgestionid = "";
                }
                else
                {
                    txtgestionid = Convert.ToString(Request.Params["txtgestionid"]);
                }


                if (string.IsNullOrEmpty(Convert.ToString(Request.Params["txttablaid"])))
                {
                    txttablaid = "";
                }
                else
                {
                    txttablaid = Convert.ToString(Request.Params["txttablaid"]);
                }

                if (txtinformesid == "" && txtgestionid == "" && txttablaid == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Envio','');}window.onload=init;</script>");
                }


                if (Request.Params["txtdiashabiles"] == "")
                {
                    flag = flag + "- Ingrese el nro de dias habiles.</br>";
                }
                else
                {
                    txtdiashabiles = Convert.ToInt32(Request.Params["txtdiashabiles"]);
                }

                if (Request.Params["txtNroInformes"] == "")
                {
                    flag = flag + "- Ingrese el nro de informes.</br>";
                }
                else
                {
                    NroInformes = Convert.ToInt32(Request.Params["txtNroInformes"]);
                }

                if (Request.Params["diasRestantes"] == "")
                {
                    diasRestantes = 0;
                }
                else
                {
                    diasRestantes = Convert.ToInt32(collection["diasRestantes"]);
                }

                string frecuenciaInforme = Convert.ToString(Request.Params["cborangoenvio"]);

                string frecuenciaInformepersonalizado = "";
                if (frecuenciaInforme == "P")
                {
                    frecuenciaInformepersonalizado = Convert.ToString(Request.Params["cbFrecuenciapersonalizada"]);

                    if (Request.Params["txtnumerofrecuenciapersonalizada"] == "")
                    {
                        flag = flag + "- Ingrese el numerode dias habiles.</br>";
                    }
                    else
                    {
                        txtnumerofrecuenciapersonalizada = Convert.ToInt32(Request.Params["txtnumerofrecuenciapersonalizada"]);
                    }
                }
                else
                {
                    // Asignar valores en blanco
                    frecuenciaInformepersonalizado = "";
                    txtnumerofrecuenciapersonalizada = 0;
                }

                string TipoDiasEnvio = Convert.ToString(Request.Params["cbdiashabiles"]);
                string MostrarDiasRestantes = Convert.ToString(Request.Params["opcion"]);

                //FIN




                if (flag != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
                }




                // Informes
                InformeED objInforme = dbc.InformeED.Where(x => x.Id == IdInformeED).FirstOrDefault();
                objInforme.IdDocuSale = idDocuSale;
                objInforme.IdManu = 1;
                //objInforme.IdManu = 1;
                objInforme.NroInformes = NroInformes;
                objInforme.Frecuencia = frecuenciaInforme;
                objInforme.Tiempo = tiempoInforme;
                if (FechaInicialInforme == null || FechaInicialInforme == DateTime.MinValue)
                {
                    objInforme.FechaInicio = DateTime.Now;
                }
                else
                {
                    objInforme.FechaInicio = FechaInicialInforme;
                }

                if (FechaFinalInforme == null || FechaFinalInforme == DateTime.MinValue)
                {
                    objInforme.FechaFin = DateTime.Now;
                }
                else
                {
                    objInforme.FechaFin = FechaFinalInforme;
                }

                objInforme.Observaciones = observacionesInforme;
                objInforme.IdTipoInforme = IdTipoInforme;
                objInforme.UsuarioModifica = idUser;

                //INFORMES AUTOMATICOS
                objInforme.DestinatarioCorreo = Convert.ToString(txttablaid);
                objInforme.DestinatarioenvioFisico = Convert.ToString(txtinformesid);
                objInforme.DestinatarioenvioMesaPartes = Convert.ToString(txtgestionid);
                objInforme.TipoDiasEnvio = TipoDiasEnvio;
                objInforme.Diasmaximoenvio = txtdiashabiles;
                objInforme.FrecuenciaPersonalizada = Convert.ToString(frecuenciaInformepersonalizado);
                objInforme.TiempoFrecuenciaPersonalizada = txtnumerofrecuenciapersonalizada;
                objInforme.DiasRangoIncompleto = diasRestantes;
                objInforme.MostrarDiasRestantes = (MostrarDiasRestantes == null ? "" : MostrarDiasRestantes);
                objInforme.FechaModifica = DateTime.Now;


                //dbc.Entry(objInforme).State = EntityState.Modified;
                dbc.SaveChanges();

                //Registrando las fechas de los informes

                var listaInforme = dbc.InformeEDDetalle.Where(x => x.IdInformeED == IdInformeED).ToList();

                if (listaInforme.Count() > 0)
                {
                    if (dbc.InformeTieneTickets(IdInformeED).Single().ToString() == "SI")
                    {
                        List<ObtenerInformeEDDetalle_Result> listaInformeTicket = dbc.ObtenerInformeEDDetalle(IdInformeED).ToList();

                        foreach (ObtenerInformeEDDetalle_Result dr in listaInformeTicket)
                        {
                            cadenaFecha = "dtinicioenvio" + dr.IdMant;
                            fechaInforme = Convert.ToDateTime(Request.Params[cadenaFecha]);
                            if (fechaInforme == null)
                            {
                                flag = "Por favor, seleccione las fechas de los mantenimientos.";
                                return Content("<script type='text/javascript'> function init() {" +
                                                   "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
                            }
                        }

                        foreach (ObtenerInformeEDDetalle_Result dr in listaInformeTicket)
                        {
                            cadenaFecha = "dtinicioenvio" + dr.IdMant;
                            fechaInforme = Convert.ToDateTime(Request.Params[cadenaFecha]);
                            //datos agregados informe automatico
                            cadenaFechafin = "dtfinenvio" + dr.IdMant;
                            fechaInformefin = Convert.ToDateTime(Request.Params[cadenaFechafin]);


                            fechaInformeplanificada = fechaInformefin.AddDays(1); // Aumentar un día a la fecha


                            cadenaFechamaximo = "dtmaximoenvio" + dr.IdMant;
                            fechaInformemaximo = Convert.ToDateTime(Request.Params[cadenaFechamaximo]);


                            InformeEDDetalle objInformeDetalle = dbc.InformeEDDetalle.Where(x => x.Id == dr.IdMant).FirstOrDefault();
                            objInformeDetalle.FechaInforme = Convert.ToDateTime(fechaInforme);
                            objInformeDetalle.FechaModifica = DateTime.Now;
                            objInformeDetalle.UsuarioModifica = idUser;

                            //datos agregados informe automatico
                            objInformeDetalle.PeriodoDesde = fechaInforme;
                            objInformeDetalle.PeriodoHasta = fechaInformefin;
                            objInformeDetalle.FechaPlanificada = fechaInformeplanificada;
                            objInformeDetalle.FechaMaximaEnvio = fechaInformemaximo;


                            objInformeDetalle.DestinatarioCorreo = Convert.ToString(txttablaid);
                            objInformeDetalle.DestinatarioenvioFisico = Convert.ToString(txtinformesid);
                            objInformeDetalle.DestinatarioenvioMesaPartes = Convert.ToString(txtgestionid);


                            dbc.Entry(objInformeDetalle).State = EntityState.Modified;
                            dbc.SaveChanges();
                        }
                    }
                    else
                    {
                        dbc.InformeEDDetalle.RemoveRange(listaInforme);

                        foreach (var key in Request.Params.AllKeys)
                        {
                            if (key.Contains("dtinicioenvio"))
                            {
                                DateTime fechaInicio = Convert.ToDateTime(Request.Params[key]);

                                InformeEDDetalle objInformeDetalle = new InformeEDDetalle();
                                objInformeDetalle.IdDocuSale = idDocuSale;
                                objInformeDetalle.IdInformeED = IdInformeED;
                                if (objInforme.IdTipoInforme != 1)
                                {
                                    objInformeDetalle.IdEstadoInforme = 2;
                                }
                                else
                                {
                                    objInformeDetalle.IdEstadoInforme = 1;
                                }
                                //objInformeDetalle.IdEstadoInforme = 1;
                                objInformeDetalle.FechaCrea = DateTime.Now;
                                objInformeDetalle.UsuarioCrea = idUser;
                                objInformeDetalle.Estado = true;

                                objInformeDetalle.FechaInforme = fechaInicio;
                                objInformeDetalle.PeriodoDesde = fechaInicio;

                                // Buscar la clave correspondiente para dtfinenvio
                                string claveFin = key.Replace("dtinicioenvio", "dtfinenvio");
                                if (Request.Params.AllKeys.Contains(claveFin))
                                {
                                    DateTime fechaFin = Convert.ToDateTime(Request.Params[claveFin]);
                                    objInformeDetalle.PeriodoHasta = fechaFin;
                                    objInformeDetalle.FechaPlanificada = fechaFin.AddDays(1);
                                }

                                // Buscar la clave correspondiente para dtmaximoenvio
                                string claveMaximo = key.Replace("dtinicioenvio", "dtmaximoenvio");
                                if (Request.Params.AllKeys.Contains(claveMaximo))
                                {
                                    DateTime fechaMaximo = Convert.ToDateTime(Request.Params[claveMaximo]);
                                    objInformeDetalle.FechaMaximaEnvio = fechaMaximo;
                                }

                                objInformeDetalle.DestinatarioCorreo = Convert.ToString(txttablaid);
                                objInformeDetalle.DestinatarioenvioFisico = Convert.ToString(txtinformesid);
                                objInformeDetalle.DestinatarioenvioMesaPartes = Convert.ToString(txtgestionid);

                                dbc.InformeEDDetalle.Add(objInformeDetalle);
                                dbc.SaveChanges();
                            }
                        }
                    }



                }
                else
                {
                    //var existingAdicionales = dbc.MarcasdeInforme.Where(pa => pa.IdInforme == IdInformeED).ToList();

                    dbc.InformeEDDetalle.RemoveRange(listaInforme);


                    for (int i = 1; i <= NroInformes; i++)
                    {
                        cadenaFecha = "dtinicioenvio" + i;
                        fechaInforme = Convert.ToDateTime(Request.Params[cadenaFecha]);


                        //datos agregados informe automatico
                        cadenaFechafin = "dtfinenvio" + i;
                        fechaInformefin = Convert.ToDateTime(Request.Params[cadenaFechafin]);


                        fechaInformeplanificada = fechaInformefin.AddDays(1); // Aumentar un día a la fecha


                        cadenaFechamaximo = "dtmaximoenvio" + i;
                        fechaInformemaximo = Convert.ToDateTime(Request.Params[cadenaFechamaximo]);
                        //fin 

                        InformeEDDetalle objInformeDetalle = new InformeEDDetalle();
                        objInformeDetalle.IdDocuSale = idDocuSale;
                        objInformeDetalle.IdInformeED = objInforme.Id;
                        objInformeDetalle.FechaInforme = Convert.ToDateTime(fechaInforme);
                        objInformeDetalle.IdEstadoInforme = 1;
                        objInformeDetalle.FechaCrea = DateTime.Now;

                        //datos agregados informe automatico
                        objInformeDetalle.PeriodoDesde = fechaInforme;
                        objInformeDetalle.PeriodoHasta = fechaInformefin;
                        objInformeDetalle.FechaPlanificada = fechaInformeplanificada;
                        objInformeDetalle.FechaMaximaEnvio = fechaInformemaximo;


                        objInformeDetalle.DestinatarioCorreo = Convert.ToString(txttablaid);
                        objInformeDetalle.DestinatarioenvioFisico = Convert.ToString(txtinformesid);
                        objInformeDetalle.DestinatarioenvioMesaPartes = Convert.ToString(txtgestionid);



                        //fin 

                        objInformeDetalle.UsuarioCrea = idUser;
                        objInformeDetalle.Estado = true;
                        dbc.InformeEDDetalle.Add(objInformeDetalle);
                        dbc.SaveChanges();
                    }

                }


                string[] selectedFabricantes = collection["SelectedFabricante"].Split(',');

                var fabricantesDiferentes = selectedFabricantes.Distinct().ToList();

                if (fabricantesDiferentes.Count() == 1 && string.IsNullOrEmpty(fabricantesDiferentes[0]))
                {
                    var existingAdicionales = dbc.MarcasdeInforme.Where(pa => pa.IdInforme == IdInformeED).ToList();

                    dbc.MarcasdeInforme.RemoveRange(existingAdicionales);
                }
                else
                {
                    var existingAdicionales = dbc.MarcasdeInforme.Where(pa => pa.IdInforme == IdInformeED).ToList();

                    dbc.MarcasdeInforme.RemoveRange(existingAdicionales);


                    foreach (var FabricantesId in fabricantesDiferentes)
                    {
                        var Marca = new MarcasdeInforme
                        {
                            IdDocuSale = idDocuSale,
                            IdInforme = IdInformeED,
                            IdManu = Convert.ToInt32(FabricantesId)

                        };

                        dbc.MarcasdeInforme.Add(Marca);
                        dbc.SaveChanges();
                    }

                }


                if (imagenesData.Length == 0)
                {
                    var existingImagenes = dbc.InformeImagenes.Where(pa => pa.IdInforme == IdInformeED).ToList();

                    dbc.InformeImagenes.RemoveRange(existingImagenes);
                    dbc.SaveChanges();
                }
                else
                {
                    var existingImagenes = dbc.InformeImagenes.Where(pa => pa.IdInforme == IdInformeED).ToList();

                    dbc.InformeImagenes.RemoveRange(existingImagenes);


                    for (int i = 0; i < imagenesData.Length; i++)
                    {
                        string imagenBase64 = imagenesData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "").Replace("data:image/png;base64,", "");
                        string comentario = comentariosData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "").Replace("[null,\"", "").Replace("[null,", "");
                        string titulo = titulosData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "");

                        string[] imgSeparados = imagenBase64.Split(',');
                        string[] comentariosSeparados = comentario.Split(',');
                        string[] titulosSeparados = titulo.Split(',');

                        var titulos = titulosSeparados.ToList();
                        var comentarios = comentariosSeparados.Distinct().ToList();
                        var imagenes = imgSeparados.ToList();

                        for (int j = 0; j < titulos.Count; j++)
                        {
                            // Crear una nueva instancia de InformeImagenes para cada elemento de titulos e imagenes
                            InformeImagenes imagen = new InformeImagenes();

                            imagen.src = "data:image/png;base64," + imagenes[j];
                            imagen.Comentario = comentarios[j];
                            imagen.TituloImage = titulos[j];
                            imagen.IdInforme = IdInformeED;
                            imagen.IdDocuSale = idDocuSale;

                            dbc.InformeImagenes.Add(imagen);
                        }


                    }

                    dbc.SaveChanges();

                }

                return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('OK','');}window.onload=init;</script>");
            }
            catch (Exception e)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('ERROR','');}window.onload=init;</script>");
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditarSoporteElectrodataOld()
        {
            //Variables
            int idDocuSale = 0, IdSoporteED = 0;
            int tiempoSoporte = 0;
            int idSla = 0;
            int mantPrev = 0;
            int bolsaHoras = 0;
            int idUser = 0;
            int flag = 0;
            int IdManu = 0;
            int periodo1 = 0;
            string valoresPeriodo = "";
            int tiempoMaximo = 0;
            int numeroCombo = 0;
            string observaciones = "";
            DateTime finSoporte = Convert.ToDateTime(null);
            try
            {
                // Validaciones
                idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                IdSoporteED = Convert.ToInt32(Request.Params["IdSoporteED"]);
                idUser = Convert.ToInt32(Session["UserId"].ToString());
                string checkPeriodico = Convert.ToString(Request.Params["chkMantPeriodicoED"]);
                observaciones = Convert.ToString(Request.Params["txtObservacionesED"]).Replace("/\n|\r|\n\r/g", "");

                if (checkPeriodico == "on")
                {

                    if (Request.Params["cbFabricanteElectrodata"] == null)
                    {
                        flag = 1;
                    }
                    else
                    {
                        IdManu = Convert.ToInt32(Request.Params["cbFabricanteElectrodata"]);
                    }
                    if (Request.Params["cbSLAED"] == null)
                    {
                        flag = 1;
                    }
                    else
                    {
                        idSla = Convert.ToInt32(Request.Params["cbSLAED"]);
                    }
                    if (Request.Params["txtTiempoSoporteED"] == "")
                    {
                        flag = 1;
                    }
                    else
                    {
                        tiempoSoporte = Convert.ToInt32(Request.Params["txtTiempoSoporteED"]);
                    }
                    if (Request.Params["txtMantPreventivoED"] == "")
                    {
                        flag = 1;
                    }
                    else
                    {
                        mantPrev = Convert.ToInt32(Request.Params["txtMantPreventivoED"]);
                    }
                    if (Request.Params["txtBolsaHorasED"] == "")
                    {
                        flag = 1;
                    }
                    else
                    {
                        bolsaHoras = Convert.ToInt32(Request.Params["txtBolsaHorasED"]);
                    }

                    if (flag == 1)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                               "if(top.MensajeConfirmacion) top.MensajeConfirmacion('1');}window.onload=init;</script>");
                    }


                    finSoporte = Convert.ToDateTime(dbc.CalcularFinSoporte(idDocuSale, tiempoSoporte).SingleOrDefault().FinSoporte);

                    //Cambiando Estado Soporte ED
                    dbc.CambiarEstadoSoporteED(idDocuSale, 1);

                    SoporteED objSoporteED = dbc.SoporteEDs.Where(x => x.IdSoporteED == IdSoporteED).FirstOrDefault();
                    objSoporteED.MantPeriodico = true;
                    objSoporteED.IdManu = IdManu;
                    objSoporteED.IdSLA = idSla;
                    objSoporteED.MantenimientoPrev = mantPrev;
                    objSoporteED.FinSoporte = finSoporte;
                    objSoporteED.BolsaHoras = bolsaHoras;
                    objSoporteED.Observaciones = observaciones;
                    objSoporteED.TiempoSoporte = tiempoSoporte;
                    objSoporteED.FechaModifica = DateTime.Now;
                    objSoporteED.UsuarioModifica = idUser;
                    objSoporteED.Estado = true;

                    dbc.Entry(objSoporteED).State = EntityState.Modified;
                    dbc.SaveChanges();

                    //Eliminando el detalle del Soporte Fabricante
                    dbc.SoporteDetalleEliminar(IdSoporteED, 1);
                    //Agregando el detalle
                    dbc.AgregarFechaMantenimientoED(idDocuSale, IdSoporteED, finSoporte, mantPrev, idUser);
                }
                else
                {
                    if (Request.Params["cbFabricanteElectrodata"] == null)
                    {
                        flag = 1;
                    }
                    else
                    {
                        IdManu = Convert.ToInt32(Request.Params["cbFabricanteElectrodata"]);
                    }
                    if (Request.Params["cbSLAED"] == null)
                    {
                        flag = 1;
                    }
                    else
                    {
                        idSla = Convert.ToInt32(Request.Params["cbSLAED"]);
                    }

                    if (Request.Params["txtBolsaHorasED"] == "")
                    {
                        flag = 1;
                    }
                    else
                    {
                        bolsaHoras = Convert.ToInt32(Request.Params["txtBolsaHorasED"]);
                    }
                    if (Request.Params["cbFechaED1"] == "" || Request.Params["cbFechaED1"] == null)
                    {
                        flag = 1;
                    }
                    else
                    {
                        periodo1 = Convert.ToInt32(Request.Params["cbFechaED1"]);
                    }
                    if (flag == 1)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('1');}window.onload=init;</script>");
                    }

                    numeroCombo = Convert.ToInt32(Request.Params["NroCombo"]);

                    //Validando los combos
                    for (int i = 1; i <= numeroCombo; i++)
                    {
                        if (Convert.ToString(Request.Params["cbFechaED" + i]) == "null")
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('1');}window.onload=init;</script>");
                        }
                    }

                    // Calcular el tiempo del soporte
                    string periodo = "cbFechaED" + numeroCombo;
                    tiempoSoporte = Convert.ToInt32(Request.Params[periodo]);
                    //Calculando el Tiempo de Soporte Maximo
                    for (int i = 1; i <= numeroCombo; i++)
                    {
                        valoresPeriodo = "cbFechaED" + i;
                        tiempoMaximo = Convert.ToInt32(Request.Params[valoresPeriodo]);
                        if (tiempoMaximo > tiempoSoporte)
                        {
                            tiempoSoporte = tiempoMaximo;
                        }
                    }
                    //tiempoSoporte = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO_PARA == idPeriodo).SingleOrDefault().VAL_ACCO_PARA);
                    // Fin de soporte
                    finSoporte = Convert.ToDateTime(dbc.CalcularFinSoporte(idDocuSale, tiempoSoporte).SingleOrDefault().FinSoporte);


                    SoporteED objSoporteED = dbc.SoporteEDs.Where(x => x.IdSoporteED == IdSoporteED).FirstOrDefault();
                    objSoporteED.MantPeriodico = false;
                    objSoporteED.IdManu = IdManu;
                    objSoporteED.IdSLA = idSla;
                    objSoporteED.MantenimientoPrev = numeroCombo;
                    objSoporteED.FinSoporte = finSoporte;
                    objSoporteED.BolsaHoras = bolsaHoras;
                    objSoporteED.Observaciones = observaciones;
                    objSoporteED.TiempoSoporte = tiempoSoporte;
                    objSoporteED.FechaModifica = DateTime.Now;
                    objSoporteED.UsuarioModifica = idUser;
                    objSoporteED.Estado = true;
                    dbc.Entry(objSoporteED).State = EntityState.Modified;
                    dbc.SaveChanges();

                    //Eliminando el detalle del Soporte Fabricante
                    dbc.SoporteDetalleEliminar(IdSoporteED, 1);


                    for (int i = 1; i <= numeroCombo; i++)
                    {

                        string valorPeriodo = "cbFechaED" + i;
                        int tiempoSop = Convert.ToInt32(Request.Params[valorPeriodo]);
                        // Fin de soporte 
                        DateTime fechaMantenimiento = Convert.ToDateTime(dbc.CalcularFinSoporte(idDocuSale, tiempoSop).SingleOrDefault().FinSoporte);
                        SoporteEDDetalle objSoporteEDet = new SoporteEDDetalle();
                        objSoporteEDet.IdSoporteED = objSoporteED.IdSoporteED;
                        objSoporteEDet.IdDocuSale = idDocuSale;
                        objSoporteEDet.FechaMantenimiento = fechaMantenimiento;
                        objSoporteEDet.FechaCrea = DateTime.Now;
                        objSoporteEDet.UsuarioCrea = idUser;
                        objSoporteEDet.Estado = true;
                        dbc.SoporteEDDetalles.Add(objSoporteEDet);
                        dbc.SaveChanges();
                    }


                }
                return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('OK');}window.onload=init;</script>");

            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                                            "if(top.MensajeConfirmacion) top.MensajeConfirmacion('ERROR');}window.onload=init;</script>");

            }
        }


        public ActionResult CalcularFinSoporte(int id = 0)
        {
            int tiempoSoporte = 0;
            //var query = dbc.CalcularFinSoporte(0, 0).ToList();

            if (Request.Params["TiempoSoporte"] == "")
            {
                var query = dbc.CalcularFinSoporte(0, 0).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                tiempoSoporte = Convert.ToInt32(Request.Params["TiempoSoporte"]);
                var query = dbc.CalcularFinSoporte(id, tiempoSoporte).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CambiarEstadoED(int id = 0)
        {
            int IdAplica = Convert.ToInt32(Request.Params["IdAplica"]);
            dbc.CambiarEstadoSoporteED(id, IdAplica);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSoporteED(int id = 0)
        {
            var result = dbc.ListarSoporteEDxProyecto(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerarFechasInforme()
        {
            DateTime fechaInicio = Convert.ToDateTime(Request.Params["fI"]);
            DateTime fechaFin = Convert.ToDateTime(Request.Params["fF"]);
            string rango = Convert.ToString(Request.Params["rangoEnvio"]);
            string opcionInformeAdicional = Convert.ToString(Request.Params["opcionInformeAdicional"]);

            if (opcionInformeAdicional == "")
            {
                opcionInformeAdicional = "SI";
            }

            string plazoEnvio = Convert.ToString(Request.Params["plazoEnvio"]);
            int cantidadDias = 0;
            if (Request.Params["cantidadDias"] != "")
            {
                cantidadDias = Convert.ToInt32(Request.Params["cantidadDias"]);
            }

            var result = dbc.GenerarFechas(fechaInicio, fechaFin, rango, opcionInformeAdicional, plazoEnvio, cantidadDias).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerarFechasMantenimiento()
        {
            DateTime FechaInicial = Convert.ToDateTime(Request.Params["FechaInicial"].ToString());
            DateTime FechaFinal = Convert.ToDateTime(Request.Params["FechaFinal"].ToString());
            int NroInformes = Convert.ToInt32(Request.Params["NroInformes"]);
            var result = dbc.GenerarFechasMantenimiento(FechaInicial, FechaFinal, NroInformes).ToList();
            result.ForEach(x => x.Fecha = DateTime.ParseExact(x.Fecha, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerSoporteEDDetalle(int id)
        {
            var result = dbc.ObtenerSoporteEDDetalle(id).ToList();
            result.ForEach(x => x.FechaMantenimiento = DateTime.ParseExact(x.FechaMantenimiento, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerInformeEDDetalle(int id = 0)
        {

            int idInforme = id;

            try
            {
                var result = dbc.ObtenerInformeEDDetalle(idInforme).ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                var result = dbc.ObtenerInformeEDDetalle(idInforme).ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }


        }

        #region "Edicion Fecha de Mantenimiento"
        public ActionResult ListaSoportes(int id = 0)
        {
            ViewBag.IdDocuSale = id;
            ViewBag.NumOP = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id).NUM_DOCU_SALE;
            int permisoOperaciones = 0;
            if (Convert.ToInt32(Session["OPERACIONES"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_OPERACIONES"]) == 1)
            {
                permisoOperaciones = 1;
            }
            //Permiso para edicion de fecha de mantenimiento(Soporte EData) solo para operaciones
            ViewBag.Operaciones = permisoOperaciones;
            return View();
        }

        public ActionResult datosOP(int id = 0)
        {
            var queryOP = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id);
            var queryTipoOP = dbc.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == queryOP.ID_TYPE_DOCU_SALE.Value);
            string numOP = queryTipoOP.NAM_TYPE_DOCU_SALE + " " + queryOP.NUM_DOCU_SALE;
            return Content(numOP);
        }

        public ActionResult EditarFechaMantenimiento(int id = 0)
        {
            ViewBag.IdMantED = id;
            var query = dbc.DatosFechaMantenimiento(id).SingleOrDefault();
            ViewBag.FechaMantenimiento = String.Format("{0:MM/dd/yyyy}", query.FechaMantenimiento);
            return View();
        }

        public ActionResult DatosFechaMantenimiento(int id = 0)
        {
            var query = dbc.DatosFechaMantenimiento(id).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSoportes(int id = 0)
        {
            var query = dbc.ListarSoportes(id).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditarFechaMant(int id = 0)
        {
            //Variables
            int IdUser = 0;
            int flag = 0;
            DateTime fechaMantenimiento = Convert.ToDateTime(null);
            try
            {
                // Validaciones
                IdUser = Convert.ToInt32(Session["UserId"].ToString());
                if (Request.QueryString["FechaMantenimiento"] != "")
                {
                    fechaMantenimiento = Convert.ToDateTime(Request.QueryString["FechaMantenimiento"]);
                }
                else
                {
                    flag = 1;
                }
                if (flag == 1)
                {
                    return Content("1");
                }

                SoporteEDDetalle objSoporteED = dbc.SoporteEDDetalles.Where(x => x.IdMantED == id).FirstOrDefault();
                objSoporteED.FechaMantenimiento = fechaMantenimiento;
                objSoporteED.FechaModifica = DateTime.Now;
                objSoporteED.UsuarioModifica = IdUser;
                objSoporteED.Estado = true;
                dbc.Entry(objSoporteED).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }
        #endregion

        public ActionResult CalculoFinSoporte(int id = 0)
        {
            int tiempoSoporte = Convert.ToInt32(Request.Params["TiempoSoporte"]);
            DateTime FechaInicial = Convert.ToDateTime(Request.Params["TiempoInicial"].ToString());
            string frecuencia = Convert.ToString(Request.Params["Frecuencia"]);
            if (frecuencia == "M")
            {
                FechaInicial = FechaInicial.AddMonths(tiempoSoporte);
            }
            else
            {
                FechaInicial = FechaInicial.AddDays(tiempoSoporte);
            }
            return Json(FechaInicial.ToString("dd/MM/yyyy"), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarSoporteInforme(int id = 0)
        {
            var result = dbc.ListarSoporteInforme(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarFechaSoporte(int i = 0)
        {
            //Variables
            int IdSoporteDetalle = 0;
            int IdTicket = 0;
            int IdUser = 0;
            string Fecha = "";

            try
            {
                // Validaciones 
                IdSoporteDetalle = Convert.ToInt32(Request.Params["IdSoporteDetalle"]);
                IdTicket = Convert.ToInt32(Request.Params["IdTicket"]);
                Fecha = Convert.ToString(Request.Params["Fecha"]);
                IdUser = Convert.ToInt32(Session["UserId"].ToString());

                var objSoporteED = dbc.SoporteEDDetalles.Single(x => x.IdMantED == IdSoporteDetalle);
                DateTime FechaModificada = Convert.ToDateTime(objSoporteED.FechaMantenimiento);
                // Valida si el soporte tiene ticket generado
                if (IdTicket == 0)
                {
                    objSoporteED.FechaMantenimiento = DateTime.ParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objSoporteED.FechaModifica = DateTime.Now;
                    objSoporteED.UsuarioModifica = IdUser;
                    dbc.Entry(objSoporteED).State = EntityState.Modified;
                    dbc.SaveChanges();
                }
                else
                {
                    //Insertando comentario en el ticket de solicitud
                    var detailsTicket = new DETAILS_TICKET();
                    detailsTicket.ID_TICK = IdTicket;
                    detailsTicket.ID_TYPE_DETA_TICK = 1;
                    detailsTicket.COM_DETA_TICK = "Se realizó la modificación de la fecha de soporte a " + Fecha + ".";
                    detailsTicket.UserId = IdUser;
                    detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                    detailsTicket.MINUTES = 0;
                    dbc.DETAILS_TICKET.Add(detailsTicket);
                    dbc.SaveChanges();

                    var objTicket = dbc.TICKETs.Single(x => x.ID_TICK == IdTicket);
                    var descripcion = objTicket.SUM_TICK;
                    objTicket.SUM_TICK = descripcion + "</br>Fecha de soporte (actualizado): " + Fecha + ".";
                    dbc.Entry(objTicket).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                Logs objLogs = new Logs();
                objLogs.IdTipoLog = 1;
                objLogs.IdRegistro = IdSoporteDetalle;
                objLogs.FechaModificada = FechaModificada;
                objLogs.FechaNueva = DateTime.ParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                objLogs.FechaCreacion = DateTime.Now;
                objLogs.UsuarioCreacion = IdUser;
                objLogs.Estado = true;
                dbc.Logs.Add(objLogs);
                dbc.SaveChanges();

                return Content("OK");
            }
            catch (Exception e)
            {
                return Content("ERROR");
            }
        }

        public ActionResult ObtenerRangosyDias()
        {

            DateTime fechaInicio = Convert.ToDateTime(null);
            DateTime fechaFin = Convert.ToDateTime(null);


            if (Request.Params["fI"] == "")
            {
                fechaFin = Convert.ToDateTime("1900-01-01");
            }
            else
            {
                fechaInicio = Convert.ToDateTime(Request.Params["fI"]);
            }


            if (Request.Params["fF"] == "")
            {
                fechaFin = Convert.ToDateTime("1900-01-01");
            }
            else
            {
                fechaFin = Convert.ToDateTime(Request.Params["fF"]);
            }



            string rango = Convert.ToString(Request.Params["rangoEnvio"]);
            if (rango == null)
            {
                rango = "M";
            }
            string opcion = Convert.ToString(Request.Params["opcionInformeAdicional"]);
            if (opcion == "")
            {
                opcion = "SI";
            }

            var result = (from c in dbc.CalculoNumeroInformesyDias(fechaInicio, fechaFin, rango, opcion)
                          select new
                          {
                              NumeroRangos = c.NumeroRangos,
                              DiasRestantes = c.DiasRestantes
                          }).FirstOrDefault();

            //dbc.CalculoNumeroInformesyDias(fechaInicio, fechaFin, rango, opcion).ToList();


            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        //carga fabricantes
        public ActionResult ObtenerFabricantesSoporte()
        {
            int idsoporte = Convert.ToInt32(Request.Params["Id"]);

            var result = dbc.ObtenerFabricantesSoporte(idsoporte).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ObtenerDetalleBolsaHoras(int IdTick)
        {
            var result = dbc.DetalleBolsahoras(IdTick).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TipoEventoElectrodata()
        {
            var result = dbc.SlaTipoEvento().ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerNombreSLA(int tipoEventoId, int idDocuSale)
        {

            string nombreSLA = dbc.ObtenerNombreSLA(tipoEventoId, idDocuSale).FirstOrDefault().ToString();

            return Json(nombreSLA, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerPrioridadNuevo()
        {

            var query = dbc.ObtenerPrioridadNuevo().ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerPrioridadEstandar(int tipoEventoId)
        {

            var query = dbc.ObtenerPrioridadEstandar(tipoEventoId).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerPrioridadEditar(int IdSla)
        {

            var query = dbc.ObtenerPrioridadEditar(IdSla).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public string CrearSlaProyecto()
        {

            string msjError = string.Empty;
            string nombreSLA = "", cuenta = "", estado = "";
            int IdDocuSale = Convert.ToInt32(Request.Params["idDocuSale"]);
            int IdTipoTicket = Convert.ToInt32((Request.Params["idTipoTicket"].ToString() == "" ? 0 : Convert.ToInt32(Request.Params["idTipoTicket"])));


            //Validaciones
            if (Request.Params["cbCuentaSLA"] == "" || Request.Params["cbCuentaSLA"] == null)
                msjError = msjError + "- Seleccione la cuenta.<br/>";
            else
                cuenta = Request.Params["cbCuentaSLA"];

            if (IdTipoTicket == 0)
                msjError = msjError + "Seleccione un tipo de evento";
            if (Request.Params["txtNombreSLA"] == "" || Request.Params["txtNombreSLA"] == null)
                msjError = msjError + "- Ingrese el nombre.<br/>";
            else
                nombreSLA = Request.Params["txtNombreSLA"];
            if (Request.Params["cbEstadoSLA"] == "" || Request.Params["cbEstadoSLA"] == null)
                msjError = msjError + "- Seleccione el estado.<br/>";
            else
                estado = Request.Params["cbEstadoSLA"];

            //Validaciones Array Prioridad
            string arrPrioridad = Request.Params["arrayPrioridad"];
            string[] arrPrio = arrPrioridad.Split(',');
            //return null;
            int cont = 0;
            try
            {
                #region validaciones

                int idCuenta = Convert.ToInt32(cuenta);
                var val = (from v in dbc.SLAs.Where(x => x.IdCuenta == idCuenta && x.Nombre == nombreSLA) select new { IDSLA = v.Id, NOMBRE = v.Nombre }).ToList();

                if (val.Count() > 0)
                {
                    msjError = msjError + "- El nombre del SLA ya se encuentra registrado.<br/>";
                }
                int sw = 0;
                int flag = 0;
                int flagchecktiempos = 0;
                int flagtiemporesolucion = 0;
                foreach (string items in arrPrio)
                {
                    string[] item;
                    item = items.Split('|');
                    if (Convert.ToString(item[0]) == "true")
                    {
                        if ((Convert.ToString(item[2]) == "" && Convert.ToString(item[5]) == "true")
                            || (Convert.ToString(item[3]) == "" && Convert.ToString(item[6]) == "true")
                            || Convert.ToString(item[4]) == "" && Convert.ToString(item[7]) == "true")
                        {
                            sw = sw + 1;
                        }

                        flag = flag + 1;
                    }

                    if (Convert.ToString(item[5]) == "false" && Convert.ToString(item[6]) == "false" && Convert.ToString(item[7]) == "false")
                    {
                        flagchecktiempos = flagchecktiempos + 1;
                    }

                    if (Convert.ToString(item[7]) == "false")
                    {
                        flagtiemporesolucion = flagtiemporesolucion + 1;
                    }

                }
                if (flagchecktiempos != 0)
                {
                    msjError = msjError + "- Debe seleccionar por lo menos un tiempo de nivel de servicio.<br/>";
                }
                if (flag == 0)
                {
                    msjError = msjError + "- Debe seleccionar por lo menos una prioridad.<br/>";
                }

                if (flagtiemporesolucion != 0)
                {
                    msjError = msjError + "- El tiempo de resolución es obligatorio.<br/>";
                }

                if (sw != 0)
                {
                    msjError = msjError + "- Debe registrar el tiempo de las prioridades seleccionadas.<br/>";
                }


                #endregion
                if (msjError != string.Empty)
                {
                    return msjError;
                }
                else
                {
                    int IdUser = 0;
                    try
                    {
                        IdUser = Convert.ToInt32(Session["UserId"].ToString());
                    }
                    catch
                    {
                        return "ERROR";
                    }
                    SLA objsla = new SLA();
                    objsla.IdCuenta = idCuenta;
                    objsla.Nombre = nombreSLA;
                    objsla.FechaCrea = DateTime.Now;
                    objsla.UsuarioCrea = IdUser;
                    if (IdTipoTicket != 0)
                    {
                        objsla.ID_TYPE_TICK = IdTipoTicket;
                    }

                    objsla.Estado = true;
                    dbc.SLAs.Add(objsla);
                    dbc.SaveChanges();

                    int Idsla = objsla.Id;

                    string checktiempos = arrPrio.FirstOrDefault();
                    string[] tiempos;
                    tiempos = checktiempos.Split('|');

                    PROYECTO_SLA objproyectosla = new PROYECTO_SLA();
                    objproyectosla.IdProyecto = IdDocuSale;
                    objproyectosla.IdSLA = Idsla;
                    objproyectosla.FechaCrea = DateTime.Now;
                    objproyectosla.UsuarioCrea = IdUser;
                    objproyectosla.Estado = true;
                    objproyectosla.TiempoRespuesta = Convert.ToBoolean(tiempos[5]);
                    objproyectosla.TiempoAtencion = Convert.ToBoolean(tiempos[6]);
                    objproyectosla.TiempoResolucion = Convert.ToBoolean(tiempos[7]);
                    dbc.PROYECTO_SLA.Add(objproyectosla);
                    dbc.SaveChanges();


                    //RECORRIDO DE LA LISTA

                    #region Detalle de Solicitud

                    foreach (var items in arrPrio)
                    {
                        string[] item;
                        item = items.Split('|');
                        if (Convert.ToString(item[0]) == "true")
                        {
                            int Respuesta = (Convert.ToString(item[2]) == "" ? 0 : Convert.ToInt32(item[2]));
                            int Atencion = (Convert.ToString(item[3]) == "" ? 0 : Convert.ToInt32(item[3]));
                            int Resolucion = (Convert.ToString(item[4]) == "" ? 0 : Convert.ToInt32(item[4]));

                            SLADetalle objslad = new SLADetalle();
                            objslad.IdSLA = Idsla;
                            objslad.IdPrioridad = Convert.ToInt32(item[1]);
                            objslad.TiempoAtencion = Resolucion / 60;

                            objslad.TiempoRespuestaMinutos = Respuesta;
                            objslad.TiempoAtencionMinutos = Atencion;
                            objslad.TiempoResolucionMinutos = Resolucion;

                            objslad.Estado = true;
                            dbc.SLADetalles.Add(objslad);
                            dbc.SaveChanges();

                        }
                    }



                    SLADetalle objslaplanificada = new SLADetalle();
                    objslaplanificada.IdSLA = Idsla;
                    objslaplanificada.IdPrioridad = 5;
                    objslaplanificada.TiempoAtencion = 0;

                    objslaplanificada.TiempoRespuestaMinutos = 0;
                    objslaplanificada.TiempoAtencionMinutos = 0;
                    objslaplanificada.TiempoResolucionMinutos = 0;

                    objslaplanificada.Estado = true;
                    dbc.SLADetalles.Add(objslaplanificada);
                    dbc.SaveChanges();

                    // SIEMPRE REGISTRAR PLANIFICADA PARA QUE NO SE CAIGAN LOS TICKETS AUTOMATICOS
                    #endregion

                }
                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        public string ValidarEdicionSLA()
        {
            try
            {
                int idSLA = 0, idDocuSale = 0;
                if (Request.Params["IdSLA"] != "") idSLA = Convert.ToInt32(Request.Params["IdSLA"]);
                if (Request.Params["IdDocuSale"] != "") idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                int idUsuario = Convert.ToInt32(Session["UserId"]);
                var mensaje = dbc.ValidarEdicionSLA(idSLA, idDocuSale).Single().Mensaje;
                return mensaje;
            }
            catch
            {
                return "Error";
            }
        }
        public string EditarSLAPry()
        {

            string msjError = string.Empty;
            string nombreSLA = "", cuenta = "", estado = "";
            int idTipoTicket = 0;
            int IdDocuSale = Convert.ToInt32(Request.Params["idDocuSale"]);

            //Validaciones
            if (Request.Params["cbCuentaSLA"] == "" || Request.Params["cbCuentaSLA"] == null)
                msjError = msjError + "- Seleccione la cuenta.<br/>";
            else
                cuenta = Request.Params["cbCuentaSLA"];

            //if (Request.Params["txtNombreSLA"] == "" || Request.Params["txtNombreSLA"] == null)
            //    msjError = msjError + "- Ingrese el nombre.<br/>";
            //else
            nombreSLA = Request.Params["txtNombreSLA"];

            if (Request.Params["cbEstadoSLA"] == "" || Request.Params["cbEstadoSLA"] == null)
                msjError = msjError + "- Seleccione el estado.<br/>";
            else
                estado = Request.Params["cbEstadoSLA"];

            if (Request.Params["idTipoTicket"] == "" || Request.Params["idTipoTicket"] == null)
                msjError = msjError + "- Seleccione un tipo de evento.<br/>";
            else
                idTipoTicket = Convert.ToInt32(Request.Params["idTipoTicket"]);
            //Validaciones Array Prioridad
            string arrPrioridad = Request.Params["arrayPrioridad"];
            string[] arrPrio = arrPrioridad.Split(',');
            int IdSLA = Convert.ToInt32(Request.Params["IdSLA"]);
            try
            {
                #region validaciones

                int idCuenta = Convert.ToInt32(cuenta);
                var val = (from v in dbc.SLAs.Where(x => x.IdCuenta == idCuenta && x.Nombre == nombreSLA && x.Id != IdSLA) select new { IDSLA = v.Id, NOMBRE = v.Nombre }).ToList();

                //if (val.Count() > 0)
                //{
                //    msjError = msjError + "- El nombre del SLA ya se encuentra registrado.<br/>";
                //}
                int sw = 0;
                int flag = 0;
                int flagtiempos = 0;
                int flagtiemporesolucion = 0;
                foreach (string items in arrPrio)
                {
                    string[] item;
                    item = items.Split('|');
                    if (Convert.ToString(item[0]) == "true")
                    {
                        if ((Convert.ToString(item[2]) == "" && Convert.ToString(item[5]) == "true")
                            || (Convert.ToString(item[3]) == "" && Convert.ToString(item[6]) == "true")
                            || Convert.ToString(item[4]) == "" && Convert.ToString(item[7]) == "true")
                        {
                            sw = sw + 1;
                        }

                        flag = flag + 1;
                    }

                    if (Convert.ToString(item[5]) == "false" && Convert.ToString(item[6]) == "false" && Convert.ToString(item[7]) == "false")
                    {
                        flagtiempos = flagtiempos + 1;
                    }

                    if (Convert.ToString(item[7]) == "false")
                    {
                        flagtiemporesolucion = flagtiemporesolucion + 1;
                    }
                }
                if (flagtiempos != 0)
                {
                    msjError = msjError + "- Debe seleccionar por lo menos un tiempo.<br/>";
                }
                if (flag == 0)
                {
                    msjError = msjError + "- Debe seleccionar por lo menos una prioridad.<br/>";
                }

                if (flagtiemporesolucion != 0)
                {
                    msjError = msjError + "- El tiempo de resolución es obligatorio.<br/>";
                }

                if (sw != 0)
                {
                    msjError = msjError + "- Debe registrar el tiempo de las prioridades seleccionadas.<br/>";
                }

                #endregion
                if (msjError != string.Empty)
                {
                    return msjError;
                }
                else
                {
                    int IdUser = 0;
                    try
                    {
                        IdUser = Convert.ToInt32(Session["UserId"].ToString());
                    }
                    catch
                    {
                        return "ERROR";
                    }

                    SLA objsla = dbc.SLAs.Find(IdSLA);
                    objsla.IdCuenta = idCuenta;
                    objsla.Nombre = nombreSLA;
                    objsla.FechaModifica = DateTime.Now;
                    objsla.UsuarioModifica = IdUser;

                    if (idTipoTicket != 0)
                    {
                        objsla.ID_TYPE_TICK = idTipoTicket;
                    }

                    if (estado == "1")
                    {
                        objsla.Estado = true;
                    }
                    else
                    {
                        objsla.Estado = false;
                    }

                    dbc.Entry(objsla).State = EntityState.Modified;
                    dbc.SaveChanges();

                    string checktiempos = arrPrio.FirstOrDefault();
                    string[] tiempos;
                    tiempos = checktiempos.Split('|');

                    PROYECTO_SLA objproyectosla = dbc.PROYECTO_SLA.Where(x => x.IdProyecto == IdDocuSale && x.IdSLA == IdSLA).FirstOrDefault();
                    objproyectosla.IdProyecto = IdDocuSale;
                    objproyectosla.IdSLA = IdSLA;
                    objproyectosla.FechaCrea = DateTime.Now;
                    objproyectosla.UsuarioCrea = IdUser;
                    objproyectosla.Estado = true;
                    objproyectosla.TiempoRespuesta = Convert.ToBoolean(tiempos[5]);
                    objproyectosla.TiempoAtencion = Convert.ToBoolean(tiempos[6]);
                    objproyectosla.TiempoResolucion = Convert.ToBoolean(tiempos[7]);
                    //dbc.PROYECTO_SLA.Add(objproyectosla);
                    dbc.Entry(objproyectosla).State = EntityState.Modified;
                    dbc.SaveChanges();


                    dbc.EditarSLA(IdSLA, "", 0, DateTime.Now, objsla.Estado);

                    //RECORRIDO DE LA LISTA

                    #region Detalle de Solicitud

                    foreach (var items in arrPrio)
                    {
                        string[] item;
                        item = items.Split('|');
                        if (Convert.ToString(item[0]) == "true")
                        {
                            int Respuesta = (Convert.ToString(item[2]) == "" ? 0 : Convert.ToInt32(item[2]));
                            int Atencion = (Convert.ToString(item[3]) == "" ? 0 : Convert.ToInt32(item[3]));
                            int Resolucion = (Convert.ToString(item[4]) == "" ? 0 : Convert.ToInt32(item[4]));

                            SLADetalle objslad = new SLADetalle();
                            objslad.IdSLA = IdSLA;
                            objslad.IdPrioridad = Convert.ToInt32(item[1]);
                            objslad.TiempoAtencion = Resolucion / 60;

                            objslad.TiempoRespuestaMinutos = Respuesta;
                            objslad.TiempoAtencionMinutos = Atencion;
                            objslad.TiempoResolucionMinutos = Resolucion;

                            objslad.Estado = true;
                            dbc.SLADetalles.Add(objslad);
                            dbc.SaveChanges();

                        }
                    }

                    SLADetalle objslaplanificada = new SLADetalle();
                    objslaplanificada.IdSLA = IdSLA;
                    objslaplanificada.IdPrioridad = 5;
                    objslaplanificada.TiempoAtencion = 0;

                    objslaplanificada.TiempoRespuestaMinutos = 0;
                    objslaplanificada.TiempoAtencionMinutos = 0;
                    objslaplanificada.TiempoResolucionMinutos = 0;

                    objslaplanificada.Estado = true;
                    dbc.SLADetalles.Add(objslaplanificada);
                    dbc.SaveChanges();

                    // SIEMPRE REGISTRAR PLANIFICADA PARA QUE NO SE CAIGAN LOS TICKETS AUTOMATICOS
                    #endregion
                }
                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        public ActionResult ListarSLAxProyecto()
        {
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);

            var result = dbc.ListarSLAxProyecto(IdDocuSale).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCboSLAxOP(int id = 0, int idComp = 0, string subCuenta = "", int idtipeticket = 0)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);


            var result = dbc.ListarCboSLAxProyecto(id, idAcco, idComp, subCuenta, idtipeticket).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
