using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Net;
using ERPElectrodata.Objects.CreateMail;
using ERPElectrodata.Object.Plugins;
using System.Net.Mail;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Threading.Tasks;

namespace ERPElectrodata.Controllers
{
    public class InformeEDController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        string reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
        //
        // GET: /InformeED/

        #region Vistas
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

                if (Convert.ToInt32(Session["ADMINISTRADOR_SISTEMA"]) == 1 || Convert.ToInt32(Session["INFORME"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_INFORME"]) == 1)
                {
                    var person = dbc.ResponsablePersona.Where(x => x.IdPersEnti == IdPersEnti).ToList();
                    int IdResponsable = 0;
                    if (person.Count() > 0)
                    {
                        IdResponsable = Convert.ToInt32(person.FirstOrDefault().IdResponsableInforme);
                    }
                    var result = dbc.ObtenerInformesxEstado(0, 0, 0, IdResponsable, "0", 0, 0, "", "", 0).ToList();
                    ViewBag.IdResponsableUsuario = IdResponsable;
                    ViewBag.TotalSinTicket = result.FirstOrDefault().TotalSinTicket;
                    ViewBag.TotalElaboracion = result.FirstOrDefault().TotalElaboracion;
                    ViewBag.TotalAprobacion = result.FirstOrDefault().TotalAprobacion;
                    ViewBag.TotalCorreccion = result.FirstOrDefault().TotalCorreccion;
                    ViewBag.TotalEnvio = result.FirstOrDefault().TotalEnvio;
                    ViewBag.TotalEvidencia = result.FirstOrDefault().TotalEvidencia;
                    ViewBag.TotalConforme = result.FirstOrDefault().TotalConforme;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }

            }
            catch (Exception ex)
            {
                return Content("Por favor inicie sesión." + " " + ex);
            }
        }

        [Authorize]
        public ActionResult EditarSoporteInforme(int id = 0)
        {
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

                ViewBag.ID_DOCU_SALE = id;
                if (Convert.ToInt32(Session["ADMINISTRADOR_SISTEMA"]) == 1 || Convert.ToInt32(Session["INFORME"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_INFORME"]) == 1)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
        }

        [Authorize]
        public ActionResult RegistroInformeED(int id)
        {
            try
            {
                ViewBag.ID_DOCU_SALE = id;
                int idsoporte = 0;

                if (dbc.SoporteEDs.Where(x => x.IdDocuSale == id).Count() == 0)
                {

                }
                else
                {
                    idsoporte = Convert.ToInt32(dbc.SoporteEDs.Where(x => x.IdDocuSale == id && x.Estado == true).FirstOrDefault().IdSoporteED);

                }
             
                var objSoporte = dbc.ObtenerDatosSoporteED(idsoporte).SingleOrDefault();

                if(objSoporte != null)
                {
                    ViewBag.FechaIni = DateTime.ParseExact(objSoporte.InicioSoporte, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    ViewBag.FechaFi = DateTime.ParseExact(objSoporte.FinSoporte, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                }
                else
                {
                    string mensaje = "<div style='background-color: #f2dede; color: #a94442; border: 1px solid #ebccd1; padding: 15px; margin-bottom: 20px;'>Debe registrar un <strong>SOPORTE ELECTRODATA</strong> para crear Informes.</div>";
                    return Content(mensaje);
                }
                return View();
            }
            catch(Exception e)
            {
                return Content("Por favor inicie sesión.");
            }
         }
        [Authorize]
        public ActionResult Calendario()
        {
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }

        [Authorize]
        public ActionResult Detalle(int id)
        {
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
                if (Convert.ToInt32(Session["ADMINISTRADOR_SISTEMA"]) == 1 || Convert.ToInt32(Session["INFORME"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_INFORME"]) == 1)
                {
                    int perfilSupervisor = Convert.ToInt32(Session["SUPERVISOR_INFORME"]);
                    int perfilInforme = Convert.ToInt32(Session["INFORME"]);
                    ViewBag.Id = id;
                    ViewBag.perfilSupervisor = perfilSupervisor;
                    ViewBag.perfilInforme = perfilInforme;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
        }

        [Authorize]
        public ActionResult ReporteInforme()
        {
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
                if (Convert.ToInt32(Session["ADMINISTRADOR_SISTEMA"]) == 1 || Convert.ToInt32(Session["INFORME"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_INFORME"]) == 1)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult GuardarInforme(FormCollection collection, string[] imagenesData, string[] titulosData, string[] comentariosData)
        {
            //Variables
            int idDocuSale = 0;
            int idUser = 0;
            string flag = "";
            string observaciones = "", observacionesInforme = "";
            int idInforme = 0;

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
                idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                idUser = Convert.ToInt32(Session["UserId"].ToString());
                observacionesInforme = Convert.ToString(Request.Params["txtObservacionesInforme"]).Replace("/\n|\r|\n\r/g", "");

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

                //Calculando el Tiempo de Soporte Maximo
                for (int i = 1; i <= NroInformes; i++)
                {
                    string stringFecha = "", stringFechaFin = "";
                    cadenaFecha = "dtinicioenvio" + i;
                    stringFecha = Convert.ToString(Request.Params[cadenaFecha]);

                    cadenaFechafin = "dtfinenvio" + i;
                    stringFechaFin = Convert.ToString(Request.Params[cadenaFechafin]);
                    //fechaInforme = Convert.ToDateTime(Request.Params[cadenaFecha]);
                    if (stringFecha == null)
                    {
                        flag = "Por favor, genere las fechas de los informes.";
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
                    }

                    DateTime fechaInicio = Convert.ToDateTime(stringFecha);
                    DateTime fechaFin = Convert.ToDateTime(stringFechaFin);

                    // Comparar las fechas
                    if (fechaInicio > fechaFin)
                    {
                        flag = "El Informe " + i + " tiene una fecha de fin anterior a la fecha de inicio. Por favor, asegúrate de que las fechas estén correctamente ingresadas.";
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
                    }
                }

                
                //Validacion imagenes:
                for (int i = 0; i < imagenesData.Length; i++)
                {
                    string imagenBase64 = imagenesData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "").Replace("data:image/png;base64,", "");
                    string comentario = comentariosData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "").Replace("[null,\"", "").Replace("[null,", "");
                    string titulo = titulosData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "");

                    string[] imgSeparados = imagenBase64.Split(',');
                    string[] comentariosSeparados = comentario.Split(',');
                    string[] titulosSeparados = titulo.Split(',');

                    var titulos = titulosSeparados.ToList();
                    var comentarios = comentariosSeparados.ToList();
                    var imagenes = imgSeparados.ToList();

                    if(imagenes.Count() > 8)
                    {
                        flag = "Máxima 8 imágenes por informe.";
                        return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
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


                //Cambiando Estado Informe
                var objProyecto = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == idDocuSale);
                objProyecto.EstadoInforme = true;
                dbc.Entry(objProyecto).State = EntityState.Modified;
                dbc.SaveChanges();
                //dbc.CambiarEstadoInforme(idDocuSale, 1);

                bool existe = dbc.InformeED.Any(x =>
                x.IdDocuSale == idDocuSale &&
                x.IdManu == MarcaInforme &&
                x.NroInformes == NroInformes &&
                x.FechaInicio == FechaInicialInforme &&
                x.Frecuencia == frecuenciaInforme &&
                x.Tiempo == tiempoInforme &&
                x.FechaFin == FechaFinalInforme &&
                x.Observaciones == observacionesInforme &&
                x.IdTipoInforme == IdTipoInforme &&
                x.UsuarioCrea == idUser
                );

                if (!existe)
                {
                    // si es informe de tickets
                    if (IdTipoInforme == 1)
                    {
                        // Informes
                        InformeED objInforme = new InformeED();
                        objInforme.IdDocuSale = idDocuSale;
                        //objInforme.IdManu = MarcaInforme;
                        objInforme.IdManu = 1;
                        objInforme.NroInformes = NroInformes;
                        objInforme.FechaInicio = FechaInicialInforme;
                        objInforme.Frecuencia = frecuenciaInforme;
                        objInforme.Tiempo = tiempoInforme;
                        objInforme.FechaFin = FechaFinalInforme;
                        objInforme.Observaciones = observacionesInforme;
                        objInforme.IdTipoInforme = IdTipoInforme;
                        objInforme.FechaCrea = DateTime.Now;
                        objInforme.UsuarioCrea = idUser;

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
                        //FIN

                        objInforme.Estado = true;
                        dbc.InformeED.Add(objInforme);
                        dbc.SaveChanges();
                        idInforme = objInforme.Id;

                        for (int i = 0; i < imagenesData.Length; i++)
                        {
                            string imagenBase64 = imagenesData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "").Replace("data:image/png;base64,", "");
                            string comentario = comentariosData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "").Replace("[null,\"", "").Replace("[null,", "");
                            string titulo = titulosData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "");

                            string[] imgSeparados = imagenBase64.Split(',');
                            string[] comentariosSeparados = comentario.Split(',');
                            string[] titulosSeparados = titulo.Split(',');

                            var titulos = titulosSeparados.ToList();
                            var comentarios = comentariosSeparados.ToList();
                            var imagenes = imgSeparados.ToList();

                                for (int j = 0; j < comentarios.Count; j++)
                                {
                                    if (imagenes[j] == "[]")
                                    {

                                    }
                                    else { 
                                        InformeImagenes imagen = new InformeImagenes();

                                        imagen.src = "data:image/png;base64," + imagenes[j];
                                        imagen.Comentario = comentarios[j];
                                        imagen.TituloImage = titulos[j];
                                        imagen.IdInforme = idInforme;
                                        imagen.IdDocuSale = idDocuSale;

                                        dbc.InformeImagenes.Add(imagen);
                                        dbc.SaveChanges();
                                    }
                                }


                        }

                        // creando marcas
                        var existingAdicionales = dbc.MarcasdeInforme.Where(pa => pa.IdInforme == idInforme).ToList();
                        dbc.MarcasdeInforme.RemoveRange(existingAdicionales);

                        foreach (var FabricantesId in fabricantesDiferentes)
                                {
                                    if (FabricantesId != "")
                                    {
                                    var Marca = new MarcasdeInforme
                                    {
                                        IdDocuSale = idDocuSale,
                                        IdInforme = idInforme,
                                        IdManu = Convert.ToInt32(FabricantesId)

                                    };

                                    dbc.MarcasdeInforme.Add(Marca);
                                    dbc.SaveChanges();
                                    }
                                    else
                                    {

                                    }
                                }
                            
                        
                        //creando fechas informes
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
                            if (objInforme.IdTipoInforme != 1)
                            {
                                objInformeDetalle.IdEstadoInforme = 2;
                            }
                            else
                            {
                                objInformeDetalle.IdEstadoInforme = 1;
                            }

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
                    else
                    {
                        // creando marcas

                        if(fabricantesDiferentes.Count() > 0)
                        {
                            var existingAdicionales = dbc.MarcasdeInforme.Where(pa => pa.IdInforme == idInforme).ToList();

                            dbc.MarcasdeInforme.RemoveRange(existingAdicionales);

                            foreach (var FabricantesId in fabricantesDiferentes)
                            {
                                if(FabricantesId != "")
                                {
                                    

                                    // Informes
                                    InformeED objInforme = new InformeED();
                                    objInforme.IdDocuSale = idDocuSale;
                                    //objInforme.IdManu = MarcaInforme;
                                    objInforme.IdManu = Convert.ToInt32(FabricantesId);
                                    objInforme.NroInformes = NroInformes;
                                    objInforme.FechaInicio = FechaInicialInforme;
                                    objInforme.Frecuencia = frecuenciaInforme;
                                    objInforme.Tiempo = tiempoInforme;
                                    objInforme.FechaFin = FechaFinalInforme;
                                    objInforme.Observaciones = observacionesInforme;
                                    objInforme.IdTipoInforme = IdTipoInforme;
                                    objInforme.FechaCrea = DateTime.Now;
                                    objInforme.UsuarioCrea = idUser;

                                    //INFORMES AUTOMATICOS
                                    objInforme.DestinatarioCorreo = Convert.ToString(txttablaid);
                                    objInforme.DestinatarioenvioFisico = Convert.ToString(txtinformesid);
                                    objInforme.DestinatarioenvioMesaPartes = Convert.ToString(txtgestionid);
                                    objInforme.TipoDiasEnvio = TipoDiasEnvio;
                                    objInforme.Diasmaximoenvio = txtdiashabiles;
                                    objInforme.FrecuenciaPersonalizada = Convert.ToString(frecuenciaInformepersonalizado);
                                    objInforme.TiempoFrecuenciaPersonalizada = txtnumerofrecuenciapersonalizada;
                                    objInforme.DiasRangoIncompleto = diasRestantes;
                                    objInforme.MostrarDiasRestantes = (MostrarDiasRestantes == null ? "SI" : MostrarDiasRestantes);
                                    //FIN

                                    objInforme.Estado = true;
                                    dbc.InformeED.Add(objInforme);
                                    dbc.SaveChanges();

                                    idInforme = objInforme.Id;
                                    //creando fechas informes
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
                                        if (objInforme.IdTipoInforme != 1)
                                        {
                                            objInformeDetalle.IdEstadoInforme = 2;
                                        }
                                        else
                                        {
                                            objInformeDetalle.IdEstadoInforme = 1;
                                        }

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

                                    // creando imagenes 
                                    for (int i = 0; i < imagenesData.Length; i++)
                                    {
                                        string imagenBase64 = imagenesData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "").Replace("data:image/png;base64,", "");
                                        string comentario = comentariosData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "").Replace("[null,\"", "").Replace("[null,", "");
                                        string titulo = titulosData[i].Replace("[\"", "").Replace("\"]", "").Replace("\"", "");

                                        string[] imgSeparados = imagenBase64.Split(',');
                                        string[] comentariosSeparados = comentario.Split(',');
                                        string[] titulosSeparados = titulo.Split(',');

                                        var titulos = titulosSeparados.ToList();
                                        var comentarios = comentariosSeparados.ToList();
                                        var imagenes = imgSeparados.ToList();


                                        for (int j = 0; j < comentarios.Count; j++)
                                        {
                                            if (imagenes[j] == "[]")
                                            {

                                            }
                                            else
                                            {
                                                InformeImagenes imagen = new InformeImagenes();

                                                imagen.src = "data:image/png;base64," + imagenes[j];
                                                imagen.Comentario = comentarios[j];
                                                imagen.TituloImage = titulos[j];
                                                imagen.IdInforme = idInforme;
                                                imagen.IdDocuSale = idDocuSale;

                                                dbc.InformeImagenes.Add(imagen);
                                                dbc.SaveChanges();
                                            }


                                        }



                                    }

                                    //creando marcas
                                    var Marca = new MarcasdeInforme
                                    {
                                        IdDocuSale = idDocuSale,
                                        IdInforme = idInforme,
                                        IdManu = Convert.ToInt32(FabricantesId)// Usa AdicionalId como el ID del formato seleccionado

                                    };

                                    dbc.MarcasdeInforme.Add(Marca);
                                    dbc.SaveChanges();
                                }
                                else
                                {

                                }
                                
                                
                            }

                        }
                    }
                    
                }
                else
                {
                    flag = "-Registro ya existe.";
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Informe','" + flag + "');}window.onload=init;</script>");
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

        public ActionResult ListarInformes()
        {
            int IdCliente = 0, IdClienteFinal = 0, IdMarca = 0, IdResponsable = 0, FlagTicket = 0, Flag = 0, bOP = 0, IdPersEntiAssi = 0;
            string IdEstadoInforme;
            if (int.TryParse(Convert.ToString(Request.Params["IdCliente"]), out IdCliente) == false)
            {
                IdCliente = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdClienteFinal"]), out IdClienteFinal) == false)
            {
                IdClienteFinal = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdMarca"]), out IdMarca) == false)
            {
                IdMarca = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdResponsable"]), out IdResponsable) == false)
            {
                IdResponsable = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["bOP"]), out bOP) == false)
            {
                bOP = 0;
            }

            if (int.TryParse(Convert.ToString(Request.Params["IdPersEntiAssi"]), out IdPersEntiAssi) == false)
            {
                IdPersEntiAssi = 0;
            }


            string fechaInicio = Convert.ToString(Request.Params["fI"]);
            string fechaFin = Convert.ToString(Request.Params["fF"]);

            IdEstadoInforme = Request.Params["IdsEstadoInforme"].ToString();

            FlagTicket = Convert.ToInt32(Request.Params["FlagTicket"]);
            Flag = Convert.ToInt32(Request.Params["Flag"]);
            try
            {
                var result = dbc.ListarInformes(IdCliente, IdClienteFinal, IdMarca, IdResponsable, IdEstadoInforme, FlagTicket, Flag, bOP, fechaInicio, fechaFin, IdPersEntiAssi).ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public ActionResult CambiarEstado(int id = 0)
        {
            int IdAplica = Convert.ToInt32(Request.Params["IdAplica"]);
            dbc.CambiarEstadoInforme(id, IdAplica);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidarInformeAveria(int id = 0)
        {
            var objProyecto = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id);
            objProyecto.AplicaInformeAveria = true;
            dbc.Entry(objProyecto).State = EntityState.Modified;
            dbc.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ValidarInformeAveria2(int id = 0)
        {
            var objProyecto = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id);
            objProyecto.AplicaInformeAveria = false;
            dbc.Entry(objProyecto).State = EntityState.Modified;
            dbc.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ValidarSoporteED(int id = 0)
        {
            var objProyecto = dbc.SoporteEDs.Single(x => x.IdSoporteED == id);
            //objProyecto.AplicaInformeAveria = true;
            dbc.Entry(objProyecto).State = EntityState.Modified;
            dbc.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListadoSoporteInforme(int id = 0)
        {
            var result = dbc.ListadoSoporteInforme(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosInformeED(int id = 0)
        {
            var query = dbc.ObtenerDatosInformeED(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarFechaInforme(int i = 0)
        {
            //Variables
            int IdInformeDetalle = 0;
            int IdTicket = 0;
            int IdUser = 0;
            string Fecha = "";

            try
            {
                // Validaciones 
                IdInformeDetalle = Convert.ToInt32(Request.Params["IdInformeDetalle"]);
                IdTicket = Convert.ToInt32(Request.Params["IdTicket"]);
                Fecha = Convert.ToString(Request.Params["Fecha"]);
                IdUser = Convert.ToInt32(Session["UserId"].ToString());

                var objInformeED = dbc.InformeEDDetalle.Single(x => x.Id == IdInformeDetalle);
                DateTime FechaModificada = Convert.ToDateTime(objInformeED.PeriodoHasta);
                // Valida si el soporte tiene ticket generado
                if (IdTicket == 0)
                {
                    objInformeED.PeriodoHasta = DateTime.ParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objInformeED.FechaInforme = DateTime.ParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objInformeED.FechaModifica = DateTime.Now;
                    objInformeED.UsuarioModifica = IdUser;
                    dbc.Entry(objInformeED).State = EntityState.Modified;
                    dbc.SaveChanges();
                }
                else
                {
                    objInformeED.PeriodoHasta = DateTime.ParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objInformeED.FechaInforme = DateTime.ParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objInformeED.FechaModifica = DateTime.Now;
                    objInformeED.UsuarioModifica = IdUser;
                    dbc.Entry(objInformeED).State = EntityState.Modified;
                    dbc.SaveChanges();

                    //Insertando comentario en el ticket de solicitud
                    var detailsTicket = new DETAILS_TICKET();
                    detailsTicket.ID_TICK = IdTicket;
                    detailsTicket.ID_TYPE_DETA_TICK = 1;
                    detailsTicket.COM_DETA_TICK = "Se realizó la modificación de la fecha de informe a " + Fecha + ".";
                    detailsTicket.UserId = IdUser;
                    detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                    detailsTicket.MINUTES = 0;
                    dbc.DETAILS_TICKET.Add(detailsTicket);
                    dbc.SaveChanges();

                    var objTicket = dbc.TICKETs.Single(x => x.ID_TICK == IdTicket);
                    var descripcion = objTicket.SUM_TICK;
                    objTicket.SUM_TICK = descripcion + "</br>Fecha de Informe (actualizada): " + Fecha + ".";
                    dbc.Entry(objTicket).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                Logs objLogs = new Logs();
                objLogs.IdTipoLog = 2;
                objLogs.IdRegistro = IdInformeDetalle;
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

        [HttpPost]
        public ActionResult GuardarFormatoInforme(FormCollection collection)
        {
            int idInformeDetalle = 0;
            DateTime dtPeriodoDesde, dtPeriodoHasta, dtFechaPlanificadaEnvio, dtFechaMaximaEnvio;
            DateTime FechaDesde = Convert.ToDateTime(null), FechaHasta = Convert.ToDateTime(null), FechaPlanificada = Convert.ToDateTime(null), FechaMaximaEnvio = Convert.ToDateTime(null);
            string idPlantilla = "";

            if (Convert.ToInt32(Request.Params["IdEstado"]) == 1)
            {
                idInformeDetalle = Convert.ToInt32((Request.Params["IdInforme"]).ToString());
                idPlantilla = Convert.ToString(Request.Params["PlantillaOP"]);
                if (idPlantilla == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionSPM) top.MensajeConfirmacionSPM('ERROR','');}window.onload=init;</script>");
                }

                if (DateTime.TryParseExact((Request.Params["dtPeriodoDesde"]).ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtPeriodoDesde))
                {
                    FechaDesde = dtPeriodoDesde;
                }

                if (DateTime.TryParseExact((Request.Params["dtPeriodoHasta"]).ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtPeriodoHasta))
                {

                    FechaHasta = dtPeriodoHasta;

                }

                if (DateTime.TryParseExact((Request.Params["dtFechaPlanificadaEnvio"]).ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFechaPlanificadaEnvio))
                {

                    FechaPlanificada = dtFechaPlanificadaEnvio;

                }

                if (DateTime.TryParseExact((Request.Params["dtFechaMaximaEnvio"]).ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFechaMaximaEnvio))
                {

                    FechaMaximaEnvio = dtFechaMaximaEnvio;

                }

                InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == idInformeDetalle).FirstOrDefault();
                ObjDetalleInforme.PeriodoDesde = FechaDesde;
                ObjDetalleInforme.PeriodoHasta = FechaHasta;
                ObjDetalleInforme.FechaPlanificada = FechaPlanificada;
                ObjDetalleInforme.FechaMaximaEnvio = FechaMaximaEnvio;


                //Validar campos
                //Logo
                var cantidadLogoGuardado = dbc.Logo.Where(x => x.NumDocuSale == ObjDetalleInforme.IdDocuSale).Count();

                if (cantidadLogoGuardado == 0 || FechaDesde == Convert.ToDateTime(null) || FechaHasta == Convert.ToDateTime(null) || FechaPlanificada == Convert.ToDateTime(null) || FechaMaximaEnvio == Convert.ToDateTime(null) || idPlantilla == "" || Request.Form["cboElaboradoPor"] == "" || Request.Form["cboRevisadoPor"] == "" || Request.Form["cboAprobadoPor"] == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionSPM) top.MensajeConfirmacionSPM('ERROR','');}window.onload=init;</script>");
                }

                var listaInformes = dbc.InformeEDDetalle.Where(x => x.IdDocuSale == ObjDetalleInforme.IdDocuSale & x.IdEstadoInforme == 1).ToList();

                foreach (InformeEDDetalle informe in listaInformes)
                {
                    informe.IdEstadoInforme = 2;
                    informe.ElaboradoPor = Convert.ToInt32(Request.Form["cboElaboradoPor"]);
                    informe.AprobadoPor = Convert.ToInt32(Request.Form["cboAprobadoPor"]);
                    informe.RevisadoPor = Convert.ToInt32(Request.Form["cboRevisadoPor"]);
                    dbc.SaveChanges();

                    var informeHistorial = new InformeHistorial
                    {
                        IdInformeDetalle = informe.Id,
                        IdEstadoAnterior = 1,
                        IdEstadoHistorial = 1,
                        IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                        FechaHistorial = DateTime.Now

                    };

                    dbc.InformeHistorial.Add(informeHistorial);
                    dbc.SaveChanges();

                }

                var selectedAdicionales = collection["SelectedAdicionales"].Split(',');

                selectedAdicionales = selectedAdicionales.Where(sa => !string.IsNullOrEmpty(sa)).ToArray();

                int docusale = Convert.ToInt32(ObjDetalleInforme.IdDocuSale);
                if (selectedAdicionales.Length == 1 && string.IsNullOrEmpty(selectedAdicionales[0]))
                {
                    var existingAdicionales = dbc.PlantillaAdicionales
                        .Where(pa => pa.IdDocuSale == ObjDetalleInforme.IdDocuSale)
                        .ToList();

                    dbc.PlantillaAdicionales.RemoveRange(existingAdicionales);
                }
                else
                {
                    var existingAdicionales = dbc.PlantillaAdicionales.Where(pa => pa.IdDocuSale == docusale).ToList();

                    dbc.PlantillaAdicionales.RemoveRange(existingAdicionales);


                    foreach (var AdicionalId in selectedAdicionales)
                    {
                        var Adicional = new PlantillaAdicionales
                        {
                            IdDocuSale = ObjDetalleInforme.IdDocuSale,
                            IdPlantilla = Convert.ToInt32(idPlantilla),
                            IdFormato = Convert.ToInt32(AdicionalId) // Usa AdicionalId como el ID del formato seleccionado
                        };

                        dbc.PlantillaAdicionales.Add(Adicional);
                        dbc.SaveChanges();
                    }
                }
                dbc.SaveChanges();

                dbc.Entry(ObjDetalleInforme).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionSPM) top.MensajeConfirmacionSPM('OK','');}window.onload=init;</script>");

            }
            else if (Convert.ToInt32(Request.Params["IdEstado"]) == 2 || Convert.ToInt32(Request.Params["IdEstado"]) == 4)
            {
                if (DateTime.TryParseExact((Request.Params["dtPeriodoDesde"]).ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtPeriodoDesde))
                {
                    FechaDesde = dtPeriodoDesde;
                }

                if (DateTime.TryParseExact((Request.Params["dtPeriodoHasta"]).ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtPeriodoHasta))
                {

                    FechaHasta = dtPeriodoHasta;

                }

                if (DateTime.TryParseExact((Request.Params["dtFechaPlanificadaEnvio"]).ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFechaPlanificadaEnvio))
                {

                    FechaPlanificada = dtFechaPlanificadaEnvio;

                }

                if (DateTime.TryParseExact((Request.Params["dtFechaMaximaEnvio"]).ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFechaMaximaEnvio))
                {

                    FechaMaximaEnvio = dtFechaMaximaEnvio;

                }


                idInformeDetalle = Convert.ToInt32((Request.Params["IdInforme"]).ToString());
                idPlantilla = Convert.ToString(Request.Params["PlantillaOP"]);

                if (Request.Form["cboElaboradoPor"] == "" || Request.Form["cboRevisadoPor"] == "" || Request.Form["cboAprobadoPor"] == "" || idPlantilla == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionPEM) top.MensajeConfirmacionPEM('ERROR','');}window.onload=init;</script>");
                }

                InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == idInformeDetalle).FirstOrDefault();
                ObjDetalleInforme.PeriodoDesde = FechaDesde;
                ObjDetalleInforme.PeriodoHasta = FechaHasta;
                ObjDetalleInforme.FechaPlanificada = FechaPlanificada;
                ObjDetalleInforme.FechaMaximaEnvio = FechaMaximaEnvio;

                try
                {
                    ObjDetalleInforme.ElaboradoPor = Convert.ToInt32(Request.Form["cboElaboradoPor"]);
                }
                catch (FormatException)
                {
                    ObjDetalleInforme.ElaboradoPor = ObjDetalleInforme.ElaboradoPor;
                }

                try
                {
                    ObjDetalleInforme.RevisadoPor = Convert.ToInt32(Request.Form["cboRevisadoPor"]);
                }
                catch (FormatException)
                {
                    ObjDetalleInforme.RevisadoPor = ObjDetalleInforme.RevisadoPor;
                }
                try
                {
                    ObjDetalleInforme.AprobadoPor = Convert.ToInt32(Request.Form["cboAprobadoPor"]);
                }
                catch (FormatException)
                {
                    ObjDetalleInforme.AprobadoPor = ObjDetalleInforme.AprobadoPor;
                }

                
                
                var selectedAdicionales = collection["SelectedAdicionales"].Split(',');

                selectedAdicionales = selectedAdicionales.Where(sa => !string.IsNullOrEmpty(sa)).ToArray();

                int docusale = Convert.ToInt32(ObjDetalleInforme.IdDocuSale);
                if (selectedAdicionales.Length == 1 && string.IsNullOrEmpty(selectedAdicionales[0]))
                {
                    var existingAdicionales = dbc.PlantillaAdicionales
                        .Where(pa => pa.IdDocuSale == ObjDetalleInforme.IdDocuSale)
                        .ToList();

                    dbc.PlantillaAdicionales.RemoveRange(existingAdicionales);
                }
                else
                {
                    var existingAdicionales = dbc.PlantillaAdicionales.Where(pa => pa.IdDocuSale == docusale).ToList();

                    dbc.PlantillaAdicionales.RemoveRange(existingAdicionales);


                    foreach (var AdicionalId in selectedAdicionales)
                    {
                        var Adicional = new PlantillaAdicionales
                        {
                            IdDocuSale = ObjDetalleInforme.IdDocuSale,
                            IdPlantilla = Convert.ToInt32(idPlantilla),
                            IdFormato = Convert.ToInt32(AdicionalId) // Usa AdicionalId como el ID del formato seleccionado
                        };

                        dbc.PlantillaAdicionales.Add(Adicional);
                        dbc.SaveChanges();
                    }
                }
                dbc.SaveChanges();


                dbc.Entry(ObjDetalleInforme).State = EntityState.Modified;
                dbc.SaveChanges();

                if (!string.IsNullOrEmpty(Request.Form["action"]))
                {
                    string action = Request.Form["action"];

                    if (action == "Corregido")
                    {
                        ObjDetalleInforme.IdEstadoInforme = 3;
                        ObjDetalleInforme.NotificacionAprobacion = 1;
                        dbc.SaveChanges();

                        var comentariosAEliminar = dbc.InformeComentarios.Where(x => x.IdInformeDetalle == idInformeDetalle);
                        dbc.InformeComentarios.RemoveRange(comentariosAEliminar);
                        var ticketPeriodoAEliminar = dbc.TicketPeriodoInforme.Where(x => x.IdInformeDetalle == idInformeDetalle);
                        dbc.TicketPeriodoInforme.RemoveRange(ticketPeriodoAEliminar);

                        dbc.SaveChanges();

                        var informeHistorial = new InformeHistorial
                        {
                            IdInformeDetalle = idInformeDetalle,
                            IdEstadoAnterior = 4,
                            IdEstadoHistorial = 5,
                            IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                            FechaHistorial = DateTime.Now

                        };

                        dbc.InformeHistorial.Add(informeHistorial);
                        dbc.SaveChanges();

                        return Content("<script type='text/javascript'> function init() {" +
                                                  "if(top.MensajeConfirmacionPCM) top.MensajeConfirmacionPCM('CORREGIDO','');}window.onload=init;</script>");
                    }

                    return Content("<script type='text/javascript'> function init() {" +
                                                   "if(top.MensajeConfirmacionPCM) top.MensajeConfirmacionPCM('ACTUALIZAR','');}window.onload=init;</script>");
                }

                return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionPEM) top.MensajeConfirmacionPEM('ACTUALIZAR','');}window.onload=init;</script>");


            }
            
            else if (Convert.ToInt32(Request.Params["IdEstado"]) == 5)
            {
                string correo = Convert.ToString(collection["txtCorreo"]);
                string direccion = Convert.ToString(collection["txtDireccion"]);
                string enlace = Convert.ToString(collection["txtEnlace"]);
                int iddocusale = Convert.ToInt32(collection["NumDocuSale"]);
                idInformeDetalle = Convert.ToInt32((Request.Params["IdInforme"]).ToString());

                InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == idInformeDetalle).FirstOrDefault();
                ObjDetalleInforme.IdEstadoInforme = 6;

                dbc.SaveChanges();

                var listaInformes = dbc.InformeEDDetalle.Where(x => x.IdDocuSale == iddocusale & (x.PeriodoHasta == ObjDetalleInforme.PeriodoHasta || x.PeriodoHasta > ObjDetalleInforme.PeriodoHasta)).ToList();

                foreach (InformeEDDetalle informe in listaInformes)
                {
                    informe.DestinatarioCorreo = correo;
                    informe.DestinatarioenvioFisico = direccion;
                    informe.DestinatarioenvioMesaPartes = enlace;
                    dbc.SaveChanges();
                }


                var informeHistorial = new InformeHistorial
                {
                    IdInformeDetalle = idInformeDetalle,
                    IdEstadoAnterior = 5,
                    IdEstadoHistorial = 6,
                    IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                    FechaHistorial = DateTime.Now

                };

                dbc.InformeHistorial.Add(informeHistorial);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionPENM) top.MensajeConfirmacionPENM('OK','');}window.onload=init;</script>");

            }
            else if (Convert.ToInt32(Request.Params["IdEstado"]) == 6)
            {
                idInformeDetalle = Convert.ToInt32((Request.Params["IdInforme"]).ToString());

                InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == idInformeDetalle).FirstOrDefault();
                ObjDetalleInforme.IdEstadoInforme = 7;
                dbc.SaveChanges();

                var informeHistorial = new InformeHistorial
                {
                    IdInformeDetalle = idInformeDetalle,
                    IdEstadoAnterior = 6,
                    IdEstadoHistorial = 7,
                    IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                    FechaHistorial = DateTime.Now

                };

                dbc.InformeHistorial.Add(informeHistorial);
                dbc.SaveChanges();

                var informeHistorial2 = new InformeHistorial
                {
                    IdInformeDetalle = idInformeDetalle,
                    IdEstadoAnterior = 7,
                    IdEstadoHistorial = 8,
                    IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                    FechaHistorial = DateTime.Now
                };

                dbc.InformeHistorial.Add(informeHistorial2);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionPEVM) top.MensajeConfirmacionPEVM('OK','');}window.onload=init;</script>");

            }

            else
            {

                return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionECM) top.MensajeConfirmacionECM('OK','');}window.onload=init;</script>");
            }

        }


        public ActionResult CronogramaInforme()
        {
            int IdCliente = 0, IdClienteFinal = 0, IdMarca = 0, IdEstado = 0, IdResponsable = 0, IdTipoInforme = 0;
            if (Request.Params["IdCliente"].ToString() != "")
            {
                IdCliente = Convert.ToInt32(Request.Params["IdCliente"].ToString());
            }
            if (Request.Params["IdClienteFinal"].ToString() != "")
            {
                IdClienteFinal = Convert.ToInt32(Request.Params["IdClienteFinal"]);
            }
            if (Request.Params["IdMarca"].ToString() != "")
            {
                IdMarca = Convert.ToInt32(Request.Params["IdMarca"]);
            }
            if (Request.Params["IdEstado"].ToString() != "")
            {
                IdEstado = Convert.ToInt32(Request.Params["IdEstado"]);
            }
            if (Request.Params["IdResponsable"].ToString() != "")
            {
                IdResponsable = Convert.ToInt32(Request.Params["IdResponsable"]);
            }
            if (Request.Params["IdTipoInforme"].ToString() != "")
            {
                IdTipoInforme = Convert.ToInt32(Request.Params["IdTipoInforme"]);
            }
            string OP = Convert.ToString(Request.Params["OP"]);

            var result = dbc.CronogramaInforme(IdCliente, IdClienteFinal, IdMarca, IdEstado, IdResponsable, IdTipoInforme, OP).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoInforme()
        {
            var result = dbc.ListarTipoInforme().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoInformexRresponsable()
        {
            int IdResponsable = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "Id")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                }

                IdResponsable = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<ListarTipoInformexResponsable_Result> query = dbc.ListarTipoInformexResponsable(IdResponsable).ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ListarTipoEntrega()
        {
            var result = dbc.ListarTipoEntrega().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarInformeResponsable()
        {
            var result = dbc.ListarInformeResponsable().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarInformeEstado()
        {
            var result = dbc.ListarInformeEstado().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarInformeEstadoMultiple(string q, string page)
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

            var result = (from s in dbc.ListarInformeEstado()
                          select new
                          {
                              id = s.IdEstado,
                              text = s.Estado
                          }).Where(x => x.text.Contains(termino));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerInformesxEstado()
        {
            int IdCliente = 0, IdClienteFinal = 0, IdMarca = 0, IdResponsable = 0, FlagTicket = 0, bOP = 0, IdPersEntiAssi = 0;
            string IdEstadoInforme;
            if (int.TryParse(Convert.ToString(Request.Params["IdCliente"]), out IdCliente) == false)
            {
                IdCliente = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdClienteFinal"]), out IdClienteFinal) == false)
            {
                IdClienteFinal = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdMarca"]), out IdMarca) == false)
            {
                IdMarca = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdResponsable"]), out IdResponsable) == false)
            {
                IdResponsable = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["IdResponsable"]), out IdResponsable) == false)
            {
                IdResponsable = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["bOP"]), out bOP) == false)
            {
                bOP = 0;
            }

            if (int.TryParse(Convert.ToString(Request.Params["IdPersEntiAssi"]), out IdPersEntiAssi) == false)
            {
                IdPersEntiAssi = 0;
            }

            FlagTicket = Convert.ToInt32(Request.Params["FlagTicket"]);

            string fechaInicio = Convert.ToString(Request.Params["fI"]);
            string fechaFin = Convert.ToString(Request.Params["fF"]);

            IdEstadoInforme = Request.Params["IdsEstadoInforme"].ToString();

            var result = dbc.ObtenerInformesxEstado(IdCliente, IdClienteFinal, IdMarca, IdResponsable, IdEstadoInforme, FlagTicket, bOP, fechaInicio, fechaFin, IdPersEntiAssi).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosInforme(int id = 0)
        {
            var query = dbc.ObtenerDatosInforme(id).ToList();

            ViewBag.ElaboradoPor = query.Single().ElaboradoPor;
            ViewBag.AprobadoPor = query.Single().AprobadoPor;
            ViewBag.RevisadoPor = query.Single().RevisadoPor;

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosPendienteEnvio(int id = 0)
        {
            try
            {
                var query = dbc.ObtenerDatosPendienteEnvio(id).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public ActionResult ObtenerRevisiones(int id = 0)
        {
            var query = dbc.ObtenerRevisiones(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult ObtenerTipoEntrega(int id = 0)
        //{
        //    var query = dbc.ObtenerTipoEntrega(id).ToList();

        //   return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        // }

        //ACTUALIZACIONES

        public ActionResult ListarOP()
        {
            var result = dbc.ComboOP().ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ListarTicketAsignadoA()
        {
            var result = dbc.ListarTicketAsignadoA().ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarInformesporOP()
        {
            int OP = Convert.ToInt32(Request.Params["bOP"]);

            var result = dbc.ListarInformes(0, 0, 0, 0, "0", 0, 0, OP, "", "", 0).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult InformesRelacionados()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                ViewBag.OP = OP;
            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }

        [Authorize]
        public ActionResult AprobarInforme()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IDI = Convert.ToInt32(Request.Params["IDI"]);

                var result = dbc.ObtenerDatosVistaPrevia(IDI).FirstOrDefault();

                ViewBag.OP = OP;
                ViewBag.IdInformeED = IDI;

                return View();


            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }

        }

        [HttpPost]
        public async Task<ActionResult> InformesVistaPrevia(int IdInforme, int Opcion)
        {
            //int IDI = Convert.ToInt32(IdInformeED);
            int IDI = Convert.ToInt32(Request.Params["IdInforme"]);
            var result = await Task.Run(() => dbc.ObtenerDatosVistaPrevia(IDI).FirstOrDefault());

            string FechaInicio = result.FechaInicio.ToString();
            string FechaFin = result.FechaFin.ToString();
            string Cliente = result.Cliente.ToString();
            string ClienteFinal = result.ClienteFinal.ToString();
            string IdDocuSale = result.IdDocuSale.ToString();
            string ID = result.ID.ToString();
            if (Opcion == 0)
            {
                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeAutomaticoPdf = new ReportViewer();
                    rptInformeAutomaticoPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeAutomaticoPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeAutomaticoPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeAutomaticoPdf.ServerReport.ReportPath = "/Formatoinforme/ReporteInforme2";
                    rptInformeAutomaticoPdf.ShowPrintButton = true;
                    rptInformeAutomaticoPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[7];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdPlantilla", ID.ToString());
                    param[6] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeAutomaticoPdf.ServerReport.SetParameters(param);
                    rptInformeAutomaticoPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeAutomaticoPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                    //return File(stream.GetBuffer(), "xml");
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 1)
            {

                try
                {
                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeResolucionPdf = new ReportViewer();
                    rptInformeResolucionPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeResolucionPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeResolucionPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeResolucionPdf.ServerReport.ReportPath = "/FormatoAutomatico";
                    rptInformeResolucionPdf.ShowPrintButton = true;
                    rptInformeResolucionPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[6];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeResolucionPdf.ServerReport.SetParameters(param);
                    rptInformeResolucionPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeResolucionPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 2)
            {

                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeAtencionPdf = new ReportViewer();
                    rptInformeAtencionPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeAtencionPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeAtencionPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeAtencionPdf.ServerReport.ReportPath = "/Formatoinforme/AtencionRequerimeinto";
                    rptInformeAtencionPdf.ShowPrintButton = true;
                    rptInformeAtencionPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[6];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeAtencionPdf.ServerReport.SetParameters(param);
                    rptInformeAtencionPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeAtencionPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                    //return File(stream.GetBuffer(), "xml");
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 3)
            {

                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportInforme1";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[6];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                    //return File(stream.GetBuffer(), "xml");
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 4)
            {

                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportInformeAlertas";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[6];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                    //return File(stream.GetBuffer(), "xml");
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 5)
            {

                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportMEF";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[6];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                    //return File(stream.GetBuffer(), "xml");
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 6)
            {

                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportOEFA";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[6];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                    //return File(stream.GetBuffer(), "xml");
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 7)
            {
                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportSUNAT";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[6];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                    //return File(stream.GetBuffer(), "xml");
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else
            {
                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportCOGATGP";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[6];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdInformeEDDetalle", IDI.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    return Content(pdfBase64, "application/pdf");

                    //return File(stream.GetBuffer(), "xml");
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> DescargarInformesPDF(int IdInforme, int Opcion)
        {
            var result = await Task.Run(() => dbc.ObtenerDatosVistaPrevia(IdInforme).FirstOrDefault());
            string FechaInicio = result.FechaInicio.ToString();
            string FechaFin = result.FechaFin.ToString();
            string Cliente = result.Cliente.ToString();
            string ClienteFinal = result.ClienteFinal.ToString();
            string IdDocuSale = result.IdDocuSale.ToString();
            string ID = result.ID.ToString();

            if (Opcion == 0)
            {
                try
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                    var rptInformeAutomaticoPdf = new ReportViewer();
                    rptInformeAutomaticoPdf.ServerReport.ReportServerCredentials = rvc;
                    rptInformeAutomaticoPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeAutomaticoPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptInformeAutomaticoPdf.ServerReport.ReportPath = "/Formatoinforme/ReporteInforme1";
                    rptInformeAutomaticoPdf.ShowPrintButton = true;
                    rptInformeAutomaticoPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param = new ReportParameter[7];
                    param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param[5] = new ReportParameter("IdPlantilla", ID.ToString());
                    param[6] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());


                    rptInformeAutomaticoPdf.ServerReport.SetParameters(param);
                    rptInformeAutomaticoPdf.ServerReport.Refresh();

                    byte[] pdfBytes = rptInformeAutomaticoPdf.ServerReport.Render("PDF");
                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });
                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 1)
            {

                try
                {
                    var reportServer1 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser1 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass1 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc1 = new CredencialesReporting(reportServerUser1, reportServerPass1, "");

                    // Configura el informe
                    var rptInformeResolucionPdf = new ReportViewer();
                    rptInformeResolucionPdf.ServerReport.ReportServerCredentials = rvc1;
                    rptInformeResolucionPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeResolucionPdf.ServerReport.ReportServerUrl = new Uri(reportServer1);
                    rptInformeResolucionPdf.ServerReport.ReportPath = "/FormatoAutomatico";
                    rptInformeResolucionPdf.ShowPrintButton = true;
                    rptInformeResolucionPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param1 = new ReportParameter[6];
                    param1[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param1[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param1[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param1[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param1[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param1[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeResolucionPdf.ServerReport.SetParameters(param1);
                    rptInformeResolucionPdf.ServerReport.Refresh();

                    byte[] pdfBytes1 = rptInformeResolucionPdf.ServerReport.Render("PDF");
                    string pdfBase64 = Convert.ToBase64String(pdfBytes1);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 2)
            {

                try
                {

                    var reportServer2 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser2 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass2 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc2 = new CredencialesReporting(reportServerUser2, reportServerPass2, "");

                    // Configura el informe
                    var rptInformeAtencionPdf = new ReportViewer();
                    rptInformeAtencionPdf.ServerReport.ReportServerCredentials = rvc2;
                    rptInformeAtencionPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeAtencionPdf.ServerReport.ReportServerUrl = new Uri(reportServer2);
                    rptInformeAtencionPdf.ServerReport.ReportPath = "/Formatoinforme/AtencionRequerimeinto";
                    rptInformeAtencionPdf.ShowPrintButton = true;
                    rptInformeAtencionPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param2 = new ReportParameter[6];
                    param2[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param2[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param2[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param2[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param2[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param2[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeAtencionPdf.ServerReport.SetParameters(param2);
                    rptInformeAtencionPdf.ServerReport.Refresh();

                    byte[] pdfBytes2 = rptInformeAtencionPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes2);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 3)
            {

                try
                {

                    var reportServer3 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser3 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass3 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc3 = new CredencialesReporting(reportServerUser3, reportServerPass3, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc3;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer3);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportInforme1";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param3 = new ReportParameter[6];
                    param3[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param3[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param3[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param3[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param3[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param3[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param3);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes3 = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes3);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 4)
            {

                try
                {

                    var reportServer3 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser3 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass3 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc3 = new CredencialesReporting(reportServerUser3, reportServerPass3, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc3;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer3);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportInformeAlertas";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param3 = new ReportParameter[6];
                    param3[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param3[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param3[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param3[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param3[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param3[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param3);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes3 = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes3);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 5)
            {

                try
                {

                    var reportServer3 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser3 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass3 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc3 = new CredencialesReporting(reportServerUser3, reportServerPass3, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc3;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer3);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportMEF";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param3 = new ReportParameter[6];
                    param3[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param3[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param3[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param3[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param3[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param3[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param3);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes3 = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes3);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 6)
            {

                try
                {

                    var reportServer3 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser3 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass3 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc3 = new CredencialesReporting(reportServerUser3, reportServerPass3, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc3;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer3);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportOEFA";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param3 = new ReportParameter[6];
                    param3[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param3[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param3[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param3[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param3[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param3[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param3);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes3 = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes3);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else if (Opcion == 7)
            {
                try
                {

                    var reportServer3 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser3 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass3 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc3 = new CredencialesReporting(reportServerUser3, reportServerPass3, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc3;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer3);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportSUNAT";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param3 = new ReportParameter[6];
                    param3[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param3[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param3[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param3[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param3[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param3[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param3);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes3 = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes3);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
            else
            {
                try
                {

                    var reportServer3 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser3 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass3 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc3 = new CredencialesReporting(reportServerUser3, reportServerPass3, "");

                    // Configura el informe
                    var rptInformeTablaPdf = new ReportViewer();
                    rptInformeTablaPdf.ServerReport.ReportServerCredentials = rvc3;
                    rptInformeTablaPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeTablaPdf.ServerReport.ReportServerUrl = new Uri(reportServer3);
                    rptInformeTablaPdf.ServerReport.ReportPath = "/TicketReportCOGATGP";
                    rptInformeTablaPdf.ShowPrintButton = true;
                    rptInformeTablaPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param3 = new ReportParameter[6];
                    param3[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param3[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param3[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param3[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param3[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param3[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeTablaPdf.ServerReport.SetParameters(param3);
                    rptInformeTablaPdf.ServerReport.Refresh();

                    byte[] pdfBytes3 = rptInformeTablaPdf.ServerReport.Render("PDF");

                    string pdfBase64 = Convert.ToBase64String(pdfBytes3);

                    // Devolver los datos en formato base64 como JSON
                    return Json(new { pdfData = pdfBase64 });

                }
                catch (Exception e)
                {
                    return Content("Error");
                }
            }
        }



        [HttpPost]
        public ActionResult AprobarInforme(FormCollection collection)
        {

            int idTipoCorreccion = 0;

            if (int.TryParse(collection["tipo-correccion"], out idTipoCorreccion))
            {
                idTipoCorreccion = Convert.ToInt32(collection["tipo-correccion"]);
            }
            else
            {
                idTipoCorreccion = 0;
            }

            string comentario = collection["comentario"];
            var selectedTickets = collection["codigos"].Split(',');
            int IdInforme = Convert.ToInt32(collection["IdInforme"]);

            string idTickValues = string.Join(",", selectedTickets);


            InformeComentarios comentarios = new InformeComentarios();

            comentarios.IdInformeDetalle = IdInforme;
            comentarios.TipoCorreccion = idTipoCorreccion;
            comentarios.Comentario = comentario;
            comentarios.Creador = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            comentarios.IdTick = idTickValues;
            comentarios.FechaCreacion = DateTime.Now;
            comentarios.Vigencia = 1;
            dbc.InformeComentarios.Add(comentarios);
            dbc.SaveChanges();
            // Obtén los valores seleccionados del Kendo MultiSelect


            TicketPeriodoInforme tickets = new TicketPeriodoInforme();
            int idNuevoComentario = comentarios.Id;

            var selectedTicketsList = selectedTickets.ToList();

            if (selectedTickets != null && selectedTicketsList.Count > 0 && !string.IsNullOrEmpty(selectedTickets[0]))
            {
                foreach (var ticket in selectedTickets)
                {
                    tickets.IdInformeDetalle = IdInforme;
                    tickets.TipoCorreccion = idTipoCorreccion;
                    tickets.IdTick = Convert.ToInt32(ticket);
                    tickets.IdComentario = idNuevoComentario;
                    dbc.TicketPeriodoInforme.Add(tickets);
                    dbc.SaveChanges();
                }
            }
            else
            {

                tickets.IdInformeDetalle = IdInforme;
                tickets.TipoCorreccion = idTipoCorreccion;
                tickets.IdComentario = idNuevoComentario;
                dbc.TicketPeriodoInforme.Add(tickets);
                dbc.SaveChanges();
            }



            //var selectedAdicionales = collection["SelectedCodigoTicket"].Split(',');
            return Content("<script type='text/javascript'> function init() {" +
                                          "if(top.MensajeConfirmacionApI) top.MensajeConfirmacionApI('OK','');}window.onload=init;</script>");
        }

        [HttpPost]
        public ActionResult GuardarAprobarInforme()
        {

            int IdInforme = Convert.ToInt32(Request.Params["IDI"]);
            string RdioAprobar = Convert.ToString(Request.Params["estado"]);

            if (RdioAprobar == "aprobar")
            {

                InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == IdInforme).FirstOrDefault();
                ObjDetalleInforme.IdEstadoInforme = 5;
                ObjDetalleInforme.NotificacionAprobado = 1;
                dbc.SaveChanges();

                var informeHistorial = new InformeHistorial
                {
                    IdInformeDetalle = IdInforme,
                    IdEstadoAnterior = 3,
                    IdEstadoHistorial = 3,
                    IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                    FechaHistorial = DateTime.Now

                };

                dbc.InformeHistorial.Add(informeHistorial);
                dbc.SaveChanges();

                return Json(new { mensaje = "Datos procesados con éxito" });
            }
            else
            {
                InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == IdInforme).FirstOrDefault();
                ObjDetalleInforme.IdEstadoInforme = 4;
                ObjDetalleInforme.NotificacionCorreccion = 1;
                dbc.SaveChanges();

                var informeHistorial = new InformeHistorial
                {
                    IdInformeDetalle = IdInforme,
                    IdEstadoAnterior = 3,
                    IdEstadoHistorial = 4,
                    IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                    FechaHistorial = DateTime.Now

                };

                dbc.InformeHistorial.Add(informeHistorial);
                dbc.SaveChanges();

                return Json(new { mensaje = "Datos procesados con éxito" });
            }
        }

        [Authorize]
        public ActionResult CorregirInforme()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IDI = Convert.ToInt32(Request.Params["IDI"]);

                var result = dbc.ObtenerDatosVistaPrevia(IDI).FirstOrDefault();

                ViewBag.OP = OP;
                ViewBag.IDI = IDI;
                ViewBag.InformeResolucion = result.AdicionalResolucion;
                ViewBag.InformeAtencion = result.AdicionalAtencion;
                ViewBag.InformeTabla = result.AdicionalTabla;

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }
        [HttpPost]
        public ActionResult GuardarCorregirInforme()
        {

            int IdInforme = Convert.ToInt32(Request.Params["IDI"]);

            InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == IdInforme).FirstOrDefault();
            ObjDetalleInforme.IdEstadoInforme = 3;
            ObjDetalleInforme.NotificacionAprobacion = 1;
            dbc.SaveChanges();

            var informeHistorial = new InformeHistorial
            {
                IdInformeDetalle = IdInforme,
                IdEstadoAnterior = 4,
                IdEstadoHistorial = 5,
                IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                FechaHistorial = DateTime.Now

            };

            dbc.InformeHistorial.Add(informeHistorial);
            dbc.SaveChanges();

            var comentariosAEliminar = dbc.InformeComentarios.Where(x => x.IdInformeDetalle == IdInforme);
            dbc.InformeComentarios.RemoveRange(comentariosAEliminar);
            var ticketPeriodoAEliminar = dbc.TicketPeriodoInforme.Where(x => x.IdInformeDetalle == IdInforme);
            dbc.TicketPeriodoInforme.RemoveRange(ticketPeriodoAEliminar);

            dbc.SaveChanges();

            return Json(new { mensaje = "Datos procesados con éxito" });


        }

        //[HttpPost]
        //public ActionResult GuardarPendienteEnvio()
        //{
        //    int IdInforme = Convert.ToInt32(Request.Params["IDI"]);

        //    InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == IdInforme).FirstOrDefault();
        //    ObjDetalleInforme.IdEstadoInforme = 6;
        //    dbc.SaveChanges();

        //    return Json(new { mensaje = "Datos procesados con éxito" });

        //}

        [Authorize]
        public ActionResult EditarInforme()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                int IdEstado = Convert.ToInt32(Request.Params["Estado"]);

                int countplantilla = dbc.PlantillaInforme.Where(X => X.Id_Docu_Sale == OP).Count();

                if (countplantilla >0)
                {
                    ViewBag.CrearPlantilla = "hidden";
                    ViewBag.TituloPlantilla = "Plantilla";
                    ViewBag.OcultarEditarPlantilla = "";
                }
                else
                {
                    ViewBag.CrearPlantilla = "";
                    ViewBag.TituloPlantilla = "";
                    ViewBag.OcultarEditarPlantilla = "hidden";
                }

                ViewBag.NumDocuSale = OP;
                ViewBag.IdDetaInforme = IdDetaInforme;
                ViewBag.IdEstado = IdEstado;

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }

        [Authorize]
        public ActionResult Plantilla()
        {
            try
            {
                int NumDocuSale = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                ViewBag.NumDocuSale = NumDocuSale;
                ViewBag.IdDetaInforme = IdDetaInforme;

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }

            return View();
        }

        [Authorize]
        public ActionResult PlantillaEditar()
        {
            try
            {
                int NumDocuSale = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                int IdPlantilla = Convert.ToInt32(Request.Params["IDP"]);
                ViewBag.NumDocuSale = NumDocuSale;
                ViewBag.IdDetaInforme = IdDetaInforme;
                ViewBag.IdPlantilla = IdPlantilla;

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }

            return View();
        }

        [Authorize]
        public ActionResult Logo()
        {
            int NumDocuSale = Convert.ToInt32(Request.Params["OP"]);
            var cantidadLogoGuardado = dbc.Logo.Where(x => x.NumDocuSale == NumDocuSale).Count();
            if (cantidadLogoGuardado >= 1)
            {
                ViewBag.TieneLogo = true;
            }
            else
            {
                ViewBag.TieneLogo = false;
            }

            ViewBag.NumDocuSale = NumDocuSale;
            ViewBag.UploadFile = "M1DS" + Convert.ToString(DateTime.Now.Ticks);
            ViewBag.DATE = String.Format("{0:g}", DateTime.Now);


            return View();
        }

        public ActionResult ComboPlantilla()
        {
            int? docu_sale = Convert.ToInt32(Request.Params["DocuSale"]);
            var result = dbc.ComboPlantilla(docu_sale).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        // Cargar adicionales
        public ActionResult ObtenerAdicionales()
        {
            int docu_sale = Convert.ToInt32(Request.Params["DocuSale"]);

            var result = dbc.CboFormatosAdicionales(docu_sale).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        //carga fabricantes
        public ActionResult ObtenerFabricantesInforme()
        {
            int idinforme = Convert.ToInt32(Request.Params["Id"]);

            var result = dbc.ObtenerFabricantesInforme(idinforme).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        //cargarimagenes
        public ActionResult ObtenerImagenesInforme(int id = 0)
        {

            int idInforme = id;
            var result = dbc.ObtenerImagenesInforme(idInforme).ToList();

            // Aumentar el límite de longitud JSON en la respuesta
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var jsonResult = new ContentResult
            {
                Content = serializer.Serialize(new { data = result }),
                ContentType = "application/json"
            };

            return jsonResult;

        }

        // Cargar adicionales
        public ActionResult ObtenerTipoEntrega()
        {
            int IdDocuSale = Convert.ToInt32(Request.Params["iddocusale"]);
            var result = dbc.ComboTipoEntregaxIdInformeDetalle(IdDocuSale).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        // Cargar tipo envio
        public ActionResult ObtenerTipoEnvio()
        {
            int docu_sale = Convert.ToInt32(Request.Params["DocuSale"]);
            var result = dbc.PlantillaInforme.Where(x => x.Id_Docu_Sale == docu_sale).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        //DATOS NUEVA PLANTILLA

        public ActionResult DatosNuevaPlantilla()
        {
            int IdInforme = Convert.ToInt32(Request.Params["IDI"]);
            var result = dbc.DatosNuevaPlantilla(IdInforme).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DatosEditarPlantilla(int IDP)
        {
            //int IdPlantilla = Convert.ToInt32(Request.Params["IdPlantilla"]);
            var result = dbc.DatosEditarPlantilla(IDP).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VistaPrevia()
        {
            try
            {
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);

                var result = dbc.ObtenerDatosVistaPrevia(IdDetaInforme).FirstOrDefault();

                ViewBag.IdDetaInforme = IdDetaInforme;

                ViewBag.FechaInicio = result.FechaInicio;
                ViewBag.FechaFin = result.FechaFin;
                ViewBag.Cliente = result.Cliente;
                ViewBag.ClienteFinal = result.ClienteFinal;
                ViewBag.IdDocuSale = result.IdDocuSale;
                ViewBag.ID = result.ID;

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }

            return View();
        }

        [HttpPost]
        public ActionResult GuardarLogo(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                int numdocusale = Convert.ToInt32(Request.Params["NumDocuSale"]);
                int existeLogo = dbc.Logo.Where(x => x.NumDocuSale == numdocusale).Count();
                if (files != null)
                {
                    foreach (var filess in files)
                    {


                        if (existeLogo == 1)
                        {
                            Logo LogoExistente = dbc.Logo.Where(x => x.NumDocuSale == numdocusale).FirstOrDefault();

                            string name_atta = Path.GetFileNameWithoutExtension(filess.FileName);
                            string extension = Path.GetExtension(filess.FileName);

                            var logo = new Logo
                            {
                                Nombre = name_atta,
                                Tipo = extension,
                                NumDocuSale = numdocusale,
                                Actualizador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                                FechaActualizacion = DateTime.Now,
                                EstadoLogo = 1

                            };

                            LogoExistente.Nombre = logo.Nombre;
                            LogoExistente.Tipo = logo.Tipo;
                            LogoExistente.NumDocuSale = logo.NumDocuSale;
                            LogoExistente.Actualizador = logo.Actualizador;
                            LogoExistente.FechaActualizacion = logo.FechaActualizacion;
                            LogoExistente.EstadoLogo = logo.EstadoLogo;
                            dbc.SaveChanges();

                            filess.SaveAs(Server.MapPath("~/Content/LogoOP/") + numdocusale + logo.Tipo);

                            return Json(new { success = true });

                        }
                        else
                        {

                            string name_atta = Path.GetFileNameWithoutExtension(filess.FileName);
                            string extension = Path.GetExtension(filess.FileName);

                            var logo = new Logo
                            {
                                Nombre = name_atta,
                                Tipo = extension,
                                NumDocuSale = numdocusale,
                                Creador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                                FechaCreacion = DateTime.Now,
                                EstadoLogo = 1

                            };

                            dbc.Logo.Add(logo);
                            dbc.SaveChanges();


                            filess.SaveAs(Server.MapPath("~/Content/LogoOP/") + numdocusale + logo.Tipo);

                            return Json(new { success = true });
                        }

                    }
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }


        }


        public ActionResult LogodePlantilla()
        {
            int numDocuSale = Convert.ToInt32(Request.Params["NumDocuSale"]);
            var result = dbc.LogodePlantilla(numDocuSale).FirstOrDefault();

            try
            {
                if (result != null && !string.IsNullOrEmpty(result.Datos))
                {
                    string fileName = result.Datos;
                    string filePath = Server.MapPath("~/Content/LogoOP/" + fileName);

                    // Verificar si el archivo existe+
                    if (System.IO.File.Exists(filePath))
                    {
                        return Json(new { Success = true, FilePath = Url.Content("~/Content/LogoOP/" + fileName) }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                // Manejar cualquier error
            }

            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GuardarPlantilla(List<Dictionary<string, object>> orderData, List<Dictionary<string, object>> sublist, int DocuSale, string TituloPlantilla, string TituloInforme)
        {
            int? IdPlantilla = null;
            int? IdInforme = null;

            if (!string.IsNullOrEmpty(Request.Params["IDP"]))
            {
                IdPlantilla = Convert.ToInt32(Request.Params["IDP"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["IDI"]))
            {
                IdInforme = 0;
            }


            using (dbc)
            {
                if (IdPlantilla.HasValue && IdInforme != 0)
                {

                    var existingPlantillaInforme = dbc.PlantillaInforme.FirstOrDefault(p => p.Id == IdPlantilla.Value);
                    if (existingPlantillaInforme != null)
                    {
                        existingPlantillaInforme.TituloPlantilla = TituloPlantilla;
                        existingPlantillaInforme.TituloInforme = TituloInforme;

                    }


                    var existingPlantillaIndex = dbc.PlantillaIndex.Where(pi => pi.Id_Plantilla == IdPlantilla.Value);
                    dbc.PlantillaIndex.RemoveRange(existingPlantillaIndex);
                }
                else
                {

                    var newPlantillaInforme = new PlantillaInforme
                    {
                        Id_Docu_Sale = DocuSale,
                        TituloPlantilla = TituloPlantilla,
                        TituloInforme = TituloInforme,
                        Fecha_Creacion = DateTime.Now,
                        Creador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                        Vigencia = 1
                    };
                    dbc.PlantillaInforme.Add(newPlantillaInforme);


                    dbc.SaveChanges();


                    IdPlantilla = newPlantillaInforme.Id;
                }

                foreach (var itemData in orderData)
                {
                    // Guardar los datos principales en la tabla PlantillaIndex
                    var plantillaIndex = new PlantillaIndex
                    {
                        Id_Plantilla = IdPlantilla.Value,
                        Categoria = Convert.ToString(itemData["OrdenInicial"]),
                        Orden = Convert.ToString(itemData["Order"]),
                        Nombre = Convert.ToString(itemData["Text"]),
                        CheckSeccion = Convert.ToBoolean(itemData["Checked"]),
                        Vigencia = 1
                    };

                    dbc.PlantillaIndex.Add(plantillaIndex);
                }

                foreach (var subItemData in sublist)
                {

                    var plantillaIndex = new PlantillaIndex
                    {
                        Id_Plantilla = IdPlantilla.Value,
                        Categoria = Convert.ToString(subItemData["OrdenInicial"]),
                        Orden = Convert.ToString(subItemData["Order"]),
                        Nombre = Convert.ToString(subItemData["Text"]),
                        CheckSeccion = Convert.ToBoolean(subItemData["Checked"]),
                        Vigencia = 1
                    };

                    dbc.PlantillaIndex.Add(plantillaIndex);
                }


                dbc.SaveChanges();


                return Json(new { success = true });
            }
        }

        public ActionResult PendienteElaboracionModal()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                int IdEstado = Convert.ToInt32(Request.Params["Estado"]);
                ViewBag.NumDocuSale = OP;
                ViewBag.IdDetaInforme = IdDetaInforme;
                ViewBag.IdEstado = IdEstado;

                var result = dbc.ObtenerDatosVistaPrevia(IdDetaInforme).FirstOrDefault();

                ViewBag.FechaInicio = result.FechaInicio;
                ViewBag.FechaFin = result.FechaFin;
                ViewBag.Cliente = result.Cliente;
                ViewBag.ClienteFinal = result.ClienteFinal;
                ViewBag.IdDocuSale = result.IdDocuSale;
                ViewBag.ID = result.ID;


            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }
        public ActionResult PendienteAprobacionModal()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                int IdEstado = Convert.ToInt32(Request.Params["Estado"]);
                ViewBag.NumDocuSale = OP;
                ViewBag.IdDetaInforme = IdDetaInforme;
                ViewBag.IdEstado = IdEstado;

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }
        public ActionResult PendienteCorreccionModal()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                int IdEstado = Convert.ToInt32(Request.Params["Estado"]);
                ViewBag.NumDocuSale = OP;
                ViewBag.IdDetaInforme = IdDetaInforme;
                ViewBag.IdEstado = IdEstado;

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }
        public ActionResult PendienteEnvioModal()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                int IdEstado = Convert.ToInt32(Request.Params["Estado"]);
                ViewBag.NumDocuSale = OP;
                ViewBag.IdDetaInforme = IdDetaInforme;
                ViewBag.IdEstado = IdEstado;

                var result = dbc.ObtenerDatosVistaPrevia(IdDetaInforme).FirstOrDefault();

                ViewBag.FechaInicio = result.FechaInicio;
                ViewBag.FechaFin = result.FechaFin;
                ViewBag.Cliente = result.Cliente;
                ViewBag.ClienteFinal = result.ClienteFinal;
                ViewBag.IdDocuSale = result.IdDocuSale;
                ViewBag.ID = result.ID;
                ViewBag.InformeResolucion = result.AdicionalResolucion;
                ViewBag.InformeAtencion = result.AdicionalAtencion;
                ViewBag.InformeTabla = result.AdicionalTabla;

            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }

        public ActionResult PendienteEvidenciaModal()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                int IdEstado = Convert.ToInt32(Request.Params["Estado"]);
                ViewBag.NumDocuSale = OP;
                ViewBag.IdDetaInforme = IdDetaInforme;
                ViewBag.IdEstado = IdEstado;
            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }

        public ActionResult EnviadoConformeModal()
        {
            try
            {
                int OP = Convert.ToInt32(Request.Params["OP"]);
                int IdDetaInforme = Convert.ToInt32(Request.Params["IDI"]);
                int IdEstado = Convert.ToInt32(Request.Params["Estado"]);
                ViewBag.NumDocuSale = OP;
                ViewBag.IdDetaInforme = IdDetaInforme;
                ViewBag.IdEstado = IdEstado;
            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }

        public ActionResult VerLogo()
        {
            int numDocuSale = Convert.ToInt32(Request.Params["OP"]);
            //var result = dbc.LogodePlantilla(numDocuSale).FirstOrDefault();
            //if (result != null && !string.IsNullOrEmpty(result.Datos))
            //{
            //    string fileName = result.Datos;
            //    string filePath = Server.MapPath("~/Content/LogoOP/" + fileName);
            //    ViewBag.rutaLogo = filePath;
            ViewBag.NumDocuSale = numDocuSale;
            //}
            //else
            //{
            //    ViewBag.rutaLogo = "";
            //}


            return View();
        }
        public ActionResult CargarListaTipoCorreciones()
        {
            List<ComboTipoCorreccion_Result> comboTipo = dbc.ComboTipoCorreccion().ToList();
            return Json(new { Data = comboTipo, Count = comboTipo.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CargarListaTicketsInforme(int IDI)
        {
            // Para probar con tickets reemplazar IDI con 6099
            List<ListadoTicketInforme_Result> listaTickets = dbc.ListadoTicketInforme(IDI).ToList();
            return Json(new { Data = listaTickets, Count = listaTickets.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CargarListaComentariosInforme(int idi)
        {
            try
            {
                List<ListaComentarioInforme_Result> comentarios = dbc.ListaComentarioInforme(idi).ToList();
                string listaComentarios = new JavaScriptSerializer().Serialize(JsonConvert.SerializeObject(comentarios, Formatting.None));
                ViewBag.Comentarios = listaComentarios;
                return PartialView("_ListaComentariosPA", comentarios);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ActionResult CargarContadorTipoCorreccion(int idi)
        {
            List<ContadorTipoCorreccion_Result> contador = dbc.ContadorTipoCorreccion(idi).ToList();
            return Json(new { Data = contador, Count = contador.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MaquinaEstados(int IDI)
        {
            //int IdPlantilla = Convert.ToInt32(Request.Params["IdPlantilla"]);
            var result = dbc.HistorialInformeDetalle(IDI).ToList();

            return Json(new { Maquina = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult SubirEvidenciaModal()
        {
            int IDI = Convert.ToInt32(Request.Params["IDI"]);
            int TipoEntrega = Convert.ToInt32(Request.Params["IDTIPOENTREGA"]);

            ViewBag.IdInformeDetalle = IDI;
            ViewBag.IdTipoEntrega = TipoEntrega;
            ViewBag.UploadFile = "M1DS" + Convert.ToString(DateTime.Now.Ticks);
            ViewBag.DATE = String.Format("{0:g}", DateTime.Now);


            return View();
        }

        [HttpPost]
        public ActionResult GuardarCargoEvidencia()
        {

            int IdInforme = Convert.ToInt32(Request.Params["IDI"]);
            string cargo = Convert.ToString(Request.Params["cargo"]);


            InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == IdInforme).FirstOrDefault();
            ObjDetalleInforme.Cargo = cargo;
            dbc.SaveChanges();

            return Json(new { mensaje = "Datos procesados con éxito" });


        }

        [HttpPost]
        public ActionResult GuardarEvidenciaEnvio(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                int idinformedetalle = Convert.ToInt32(Request.Params["IDI"]);
                int idtipoentrega = Convert.ToInt32(Request.Params["IDTIPOENTREGA"]);
                int existeEvidencia = dbc.InformeEvidenciaEnvio.Where(x => x.IdInformeDetalle == idinformedetalle & x.IdTipoEvidencia == idtipoentrega).Count();
                if (files != null)
                {
                    foreach (var filess in files)
                    {


                        if (existeEvidencia == 1)
                        {
                            InformeEvidenciaEnvio EvidenciaExistente = dbc.InformeEvidenciaEnvio.Where(x => x.IdInformeDetalle == idinformedetalle & x.IdTipoEvidencia == idtipoentrega).FirstOrDefault();

                            string name_atta = Path.GetFileNameWithoutExtension(filess.FileName);
                            string extension = Path.GetExtension(filess.FileName);

                            var evidencia = new InformeEvidenciaEnvio
                            {
                                Nombre = name_atta,
                                Tipo = extension,
                                IdTipoEvidencia = idtipoentrega,
                                IdInformeDetalle = idinformedetalle,
                                Actualizador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                                FechaActualizacion = DateTime.Now,

                            };

                            EvidenciaExistente.Nombre = evidencia.Nombre;
                            EvidenciaExistente.Tipo = evidencia.Tipo;

                            EvidenciaExistente.IdTipoEvidencia = evidencia.IdTipoEvidencia;
                            EvidenciaExistente.IdInformeDetalle = evidencia.IdInformeDetalle;
                            EvidenciaExistente.Actualizador = evidencia.Actualizador;
                            EvidenciaExistente.FechaActualizacion = evidencia.FechaActualizacion;
                            //LogoExistente.EstadoLogo = logo.EstadoLogo;
                            dbc.SaveChanges();

                            filess.SaveAs(Server.MapPath("~/Content/EvidenciaEnvioOP/") + idinformedetalle + idtipoentrega + evidencia.Tipo);

                            return Json(new { success = true });

                        }
                        else
                        {

                            string name_atta = Path.GetFileNameWithoutExtension(filess.FileName);
                            string extension = Path.GetExtension(filess.FileName);

                            var evidencia = new InformeEvidenciaEnvio
                            {
                                Nombre = name_atta,
                                Tipo = extension,
                                IdTipoEvidencia = idtipoentrega,
                                IdInformeDetalle = idinformedetalle,
                                Creador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                                FechaCreacion = DateTime.Now,
                                //EstadoLogo = 1

                            };

                            dbc.InformeEvidenciaEnvio.Add(evidencia);
                            dbc.SaveChanges();


                            filess.SaveAs(Server.MapPath("~/Content/EvidenciaEnvioOP/") + idinformedetalle + idtipoentrega + evidencia.Tipo);

                            return Json(new { success = true });
                        }

                    }
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }


        }

        public ActionResult MostrarImagenEvidencia()
        {
            int IdInformeEDDetalle = Convert.ToInt32(Request.Params["IDI"]);
            int IdTipoEntregaOP = Convert.ToInt32(Request.Params["IDTIPOENTREGA"]);
            var result = dbc.InformeImagenEvidencia(IdInformeEDDetalle, IdTipoEntregaOP).FirstOrDefault();

            try
            {
                if (result != null && !string.IsNullOrEmpty(result.Datos))
                {
                    string fileName = result.Datos;
                    string filePath = Server.MapPath("~/Content/EvidenciaEnvioOP/" + fileName);

                    // Verificar si el archivo existe+
                    if (System.IO.File.Exists(filePath))
                    {
                        return Json(new { Success = true, FilePath = Url.Content("~/Content/EvidenciaEnvioOP/" + fileName) }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                // Manejar cualquier error
            }

            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerEvidencia()
        {
            int IdInformeEDDetalle = Convert.ToInt32(Request.Params["IDI"]);
            int IdTipoEntrega = Convert.ToInt32(Request.Params["IDTIPOENTREGA"]);

            ViewBag.IdInformeDetalle = IdInformeEDDetalle;
            ViewBag.IdTipoEntrega = IdTipoEntrega;


            return View();
        }

        [HttpPost]
        public ActionResult EnvioInforme(int IdInforme)
        {
            var result = dbc.ObtenerDatosVistaPrevia(IdInforme).FirstOrDefault();
            string FechaInicio = result.FechaInicio.ToString();
            string FechaFin = result.FechaFin.ToString();
            string Cliente = result.Cliente.ToString();
            string ClienteFinal = result.ClienteFinal.ToString();
            string IdDocuSale = result.IdDocuSale.ToString();
            string ID = result.ID.ToString();

            //datos para el cuerpo del correo:
            string NombreClienteFinal = result.NombreCliente.ToString();
            string periodo = result.Periodo.ToString();
            string op = result.OP.ToString();
            string producto = result.Producto.ToString();
            string FechaInicioC = result.FechaInicioString.ToString();
            string FechaFinC = result.FechaFinString.ToString();

            byte[] pdfBytes1 = null;
            byte[] pdfBytes2 = null;
            byte[] pdfBytes3 = null;

            try
            {
                string correo = Convert.ToString(Request.Params["correos"]);
                string from = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 4 && x.ID_PARA == 24 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                string pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 4 && x.ID_PARA == 25 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();

                MailMessage mailMessage = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

                // Configurar credenciales y opciones de seguridad
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from, pass);

                // Configurar el mensaje
                mailMessage.From = new MailAddress(from, "IT Management System");
                mailMessage.To.Add(correo);
                mailMessage.Subject = "Informe " + periodo + " de soporte";
                Body body = new Body();
                // Aquí puedes agregar el contenido del correo
                mailMessage.Body = body.EnvioAutomaticoInforme(NombreClienteFinal, periodo, op, producto, FechaInicioC, FechaFinC);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

                var rptInformeAutomaticoPdf = new ReportViewer();
                rptInformeAutomaticoPdf.ServerReport.ReportServerCredentials = rvc;
                rptInformeAutomaticoPdf.ProcessingMode = ProcessingMode.Remote;
                rptInformeAutomaticoPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptInformeAutomaticoPdf.ServerReport.ReportPath = "/Formatoinforme/ReporteInforme";
                rptInformeAutomaticoPdf.ShowPrintButton = true;
                rptInformeAutomaticoPdf.ShowParameterPrompts = false;

                // Define parámetros
                ReportParameter[] param = new ReportParameter[7];
                param[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                param[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                param[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                param[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                param[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                param[5] = new ReportParameter("IdPlantilla", ID.ToString());
                param[6] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                rptInformeAutomaticoPdf.ServerReport.SetParameters(param);
                rptInformeAutomaticoPdf.ServerReport.Refresh();

                byte[] pdfBytes = rptInformeAutomaticoPdf.ServerReport.Render("PDF");
                Attachment pdfAttachment = new Attachment(new MemoryStream(pdfBytes), "Informe.pdf", "application/pdf");
                mailMessage.Attachments.Add(pdfAttachment);

                if (result.AdicionalResolucion == 1)
                {
                    var reportServer1 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser1 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass1 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc1 = new CredencialesReporting(reportServerUser1, reportServerPass1, "");

                    // Configura el informe
                    var rptInformeResolucionPdf = new ReportViewer();
                    rptInformeResolucionPdf.ServerReport.ReportServerCredentials = rvc1;
                    rptInformeResolucionPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeResolucionPdf.ServerReport.ReportServerUrl = new Uri(reportServer1);
                    rptInformeResolucionPdf.ServerReport.ReportPath = "/FormatoAutomatico";
                    rptInformeResolucionPdf.ShowPrintButton = true;
                    rptInformeResolucionPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param1 = new ReportParameter[6];
                    param1[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param1[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param1[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param1[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param1[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param1[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeResolucionPdf.ServerReport.SetParameters(param1);
                    rptInformeResolucionPdf.ServerReport.Refresh();

                    pdfBytes1 = rptInformeResolucionPdf.ServerReport.Render("PDF");

                    Attachment pdfAttachment1 = new Attachment(new MemoryStream(pdfBytes1), "InformeResolucion.pdf", "application/pdf");
                    mailMessage.Attachments.Add(pdfAttachment1);
                }

                if (result.AdicionalAtencion == 1)
                {

                    var reportServer2 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser2 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass2 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc2 = new CredencialesReporting(reportServerUser2, reportServerPass2, "");

                    // Configura el informe
                    var rptInformeAtencionPdf = new ReportViewer();
                    rptInformeAtencionPdf.ServerReport.ReportServerCredentials = rvc2;
                    rptInformeAtencionPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeAtencionPdf.ServerReport.ReportServerUrl = new Uri(reportServer2);
                    rptInformeAtencionPdf.ServerReport.ReportPath = "/Formatoinforme/AtencionRequerimeinto";
                    rptInformeAtencionPdf.ShowPrintButton = true;
                    rptInformeAtencionPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param2 = new ReportParameter[6];
                    param2[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param2[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param2[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param2[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param2[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param2[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeAtencionPdf.ServerReport.SetParameters(param2);
                    rptInformeAtencionPdf.ServerReport.Refresh();

                    pdfBytes2 = rptInformeAtencionPdf.ServerReport.Render("PDF");

                    Attachment pdfAttachment2 = new Attachment(new MemoryStream(pdfBytes2), "InformeAtencion.pdf", "application/pdf");
                    mailMessage.Attachments.Add(pdfAttachment2);

                }

                if (result.AdicionalTabla == 1)
                {

                    var reportServer2 = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser2 = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass2 = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc2 = new CredencialesReporting(reportServerUser2, reportServerPass2, "");

                    // Configura el informe
                    var rptInformeAtencionPdf = new ReportViewer();
                    rptInformeAtencionPdf.ServerReport.ReportServerCredentials = rvc2;
                    rptInformeAtencionPdf.ProcessingMode = ProcessingMode.Remote;
                    rptInformeAtencionPdf.ServerReport.ReportServerUrl = new Uri(reportServer2);
                    rptInformeAtencionPdf.ServerReport.ReportPath = "/Formatoinforme/AtencionRequerimeinto";
                    rptInformeAtencionPdf.ShowPrintButton = true;
                    rptInformeAtencionPdf.ShowParameterPrompts = false;

                    // Define parámetros
                    ReportParameter[] param3 = new ReportParameter[6];
                    param3[0] = new ReportParameter("FechaInicio", FechaInicio.ToString());
                    param3[1] = new ReportParameter("FechaFin", FechaFin.ToString());
                    param3[2] = new ReportParameter("ID_COMP", Cliente.ToString());
                    param3[3] = new ReportParameter("ID_COMP_END", ClienteFinal.ToString());
                    param3[4] = new ReportParameter("IdDocuSale", IdDocuSale.ToString());
                    param3[5] = new ReportParameter("IdInformeEDDetalle", IdInforme.ToString());

                    rptInformeAtencionPdf.ServerReport.SetParameters(param3);
                    rptInformeAtencionPdf.ServerReport.Refresh();

                    pdfBytes3 = rptInformeAtencionPdf.ServerReport.Render("PDF");

                    Attachment pdfAttachment3 = new Attachment(new MemoryStream(pdfBytes3), "InformeTicket.pdf", "application/pdf");
                    mailMessage.Attachments.Add(pdfAttachment3);

                }


                smtp.Send(mailMessage);

                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content("ERROR");
            }
        }

        public ActionResult EdicionInforme()
        {
            //Variables
            int IdInformeDetalle = 0, IdTipoEntrega = 0, IdTicket = 0, IdResponsableInforme = 0, IdEstado = 1, IdDocuSale = 0;
            int IdUser = 0;
            DateTime dtPeriodoDesde, dtPeriodoHasta, dtFechaPlanificadaEnvio, dtFechaMaximaEnvio;

            string Cargo = "", Destinatarios = "";

            try
            {
                IdInformeDetalle = Convert.ToInt32(Request.Params["IdInforme"]);
                IdResponsableInforme = Convert.ToInt32(Request.Params["IdResponsableInforme"]);
                IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                IdTicket = Convert.ToInt32(Request.Params["IdTicket"]);
                IdUser = Convert.ToInt32(Session["UserId"].ToString());

                //Validaciones-Revisiones
                string mensaje = "", revision = "";
                int i2 = 0, i3 = 0, i4 = 0, i5 = 0, i6 = 0, i7 = 0;
                for (int i = 2; i <= 7; i++)
                {
                    if (Convert.ToInt32(Request.Params["rbEstado" + i]) == 1)
                    {
                        if (i == 2) { revision = "Pendiente de Elaboración"; }
                        if (i == 3) { revision = "Pendiente de Aprobación"; }
                        if (i == 4) { revision = "Pendiente de Corrección"; }
                        if (i == 5) { revision = "Pendiente de Envío"; }
                        if (i == 6) { revision = "aprobación administrativa"; }
                        if (i == 7) { revision = "Pendiente de Envidencia"; }
                        //Si es SOC la revisión técnica, aprobación técnica, revisión administrativa y aprobación administrativa NO APLICA.
                        if (IdResponsableInforme == 1 || (IdResponsableInforme == 2 && (i == 2 || i == 7)))
                        {
                            if (String.IsNullOrEmpty(Convert.ToString(Request.Params["dtFecha" + i])))
                            {
                                mensaje = mensaje + "- Seleccione una fecha de " + revision + ".<br/>";
                            }
                            if (String.IsNullOrEmpty(Convert.ToString(Request.Params["txtComentario" + i])))
                            {
                                mensaje = mensaje + "- Seleccione una comentario (" + revision + ").<br/>";
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(Convert.ToString(Request.Params["rbEstado" + i])) || Convert.ToInt32(Request.Params["IdRevision" + i]) != 0)
                    {
                        if (i == 2) { i2 = 1; }
                        if (i == 3) { i3 = 1; }
                        if (i == 4) { i4 = 1; }
                        if (i == 5) { i5 = 1; }
                        if (i == 6) { i6 = 1; }
                        if (i == 7) { i7 = 1; }
                    }
                }

                if (mensaje != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('ALERTA','" + mensaje + "');}window.onload=init;</script>");
                }

                if (IdResponsableInforme == 1 && ((i2 < i3) || (i3 < i4) || (i4 < i5) || (i5 < i6) || (i6 < i7)))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('ALERTA','Debe registrar la revisión anterior antes de pasar a la siguiente.');}window.onload=init;</script>");
                }
                //Si es SOC la revisión técnica, aprobación técnica, revisión administrativa y aprobación administrativa NO APLICA.
                if (IdResponsableInforme == 2 && i2 < i7)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeConfirmacion) top.MensajeConfirmacion('ALERTA','Debe registrar la revisión anterior antes de pasar a la siguiente.');}window.onload=init;</script>");
                }

                if (i2 == 1)
                {
                    IdEstado = 2;
                    if (IdResponsableInforme == 2)
                        IdEstado = 5;
                }
                if (i3 == 1) { IdEstado = 3; }
                if (i4 == 1) { IdEstado = 4; }
                if (i5 == 1) { IdEstado = 5; }
                if (i6 == 1) { IdEstado = 6; }
                if (i7 == 1) { IdEstado = 7; }

                //Edición de Informe
                var objInformeED = dbc.InformeEDDetalle.Single(x => x.Id == IdInformeDetalle);
                // Validaciones 

                if (DateTime.TryParseExact(Convert.ToString(Request.Params["dtPeriodoDesde"]), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtPeriodoDesde))
                {
                    dtPeriodoDesde = (DateTime.ParseExact(Convert.ToString(Request.Params["dtPeriodoDesde"]), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture));
                }
                else
                {
                    dtPeriodoDesde = (DateTime.ParseExact(Convert.ToString(Request.Params["dtPeriodoDesde"]), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                }

                if (DateTime.TryParseExact(Convert.ToString(Request.Params["dtPeriodoHasta"]), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtPeriodoHasta))
                {
                    dtPeriodoHasta = (DateTime.ParseExact(Convert.ToString(Request.Params["dtPeriodoHasta"]), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture));
                }
                else
                {
                    dtPeriodoHasta = (DateTime.ParseExact(Convert.ToString(Request.Params["dtPeriodoHasta"]), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                }
                if (DateTime.TryParseExact(Convert.ToString(Request.Params["dtFechaPlanificadaEnvio"]), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaPlanificadaEnvio))
                {
                    dtFechaPlanificadaEnvio = (DateTime.ParseExact(Convert.ToString(Request.Params["dtFechaPlanificadaEnvio"]), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture));
                }
                else
                {
                    dtFechaPlanificadaEnvio = (DateTime.ParseExact(Convert.ToString(Request.Params["dtFechaPlanificadaEnvio"]), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                }
                if (DateTime.TryParseExact(Convert.ToString(Request.Params["dtFechaMaximaEnvio"]), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFechaMaximaEnvio))
                {
                    dtFechaMaximaEnvio = (DateTime.ParseExact(Convert.ToString(Request.Params["dtFechaMaximaEnvio"]), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture));
                }
                else
                {
                    dtFechaMaximaEnvio = (DateTime.ParseExact(Convert.ToString(Request.Params["dtFechaMaximaEnvio"]), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                }
                

                //else { objInformeED.FechaMaximaEnvio = null; }

                objInformeED.PeriodoDesde = dtPeriodoDesde;
                objInformeED.PeriodoHasta = dtPeriodoHasta;
                objInformeED.FechaPlanificada = dtFechaPlanificadaEnvio;
                objInformeED.FechaMaximaEnvio = dtFechaMaximaEnvio;
                objInformeED.IdEstadoInforme = IdEstado;
                objInformeED.Cargo = Cargo;
                objInformeED.Destinatarios = Destinatarios;
                objInformeED.FechaModifica = DateTime.Now;  
                objInformeED.UsuarioModifica = IdUser;
                dbc.Entry(objInformeED).State = EntityState.Modified;
                dbc.SaveChanges();

                //Tipo Entrega
                try
                {
                    dbc.EliminarTipoEntrega(IdDocuSale);
                }
                catch
                {
                }
                //Seleccion Multiple
                if (!String.IsNullOrEmpty(Request.Params["IdTipoEntrega"]))
                {
                    string te = Convert.ToString(Request.Params["IdTipoEntrega"]);
                    string[] strTipoEntrega = te.Split(',');

                    foreach (string strTEntrega in strTipoEntrega)
                    {
                        OPTipoEntrega objTipoEntrega = new OPTipoEntrega();
                        objTipoEntrega.IdDocuSale = IdDocuSale;
                        objTipoEntrega.IdTipoEntrega = Convert.ToInt32(strTEntrega);
                        dbc.OPTipoEntrega.Add(objTipoEntrega);
                        dbc.SaveChanges();
                    }

                }
                //Revisiones
                int aplicaEstado = 0, IdRevision = 0;
                string fechaRevision = "", comentarios = "";
                for (int i = 2; i <= 7; i++)
                {
                    IdRevision = Convert.ToInt32(Request.Params["IdRevision" + i]);
                    fechaRevision = Convert.ToString(Request.Params["dtFecha" + i]);
                    //Creación
                    if (IdRevision == 0)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(Request.Params["rbEstado" + i])))
                        {
                            aplicaEstado = Convert.ToInt32(Request.Params["rbEstado" + i]);
                            comentarios = Convert.ToString(Request.Params["txtComentario" + i]);

                            InformeEDRevision objRevision = new InformeEDRevision();
                            objRevision.IdInformeEDDetalle = IdInformeDetalle;
                            objRevision.IdEstadoInforme = i;
                            //Si es SOC la revisión técnica, aprobación técnica, revisión administrativa y aprobación administrativa NO APLICA.
                            if (IdResponsableInforme == 2 && (i == 3 || i == 4 || i == 5 || i == 6))
                            {
                                objRevision.Aplica = false;
                                objRevision.Comentarios = "";
                            }
                            else
                            {
                                objRevision.Aplica = Convert.ToBoolean(aplicaEstado);
                                if (aplicaEstado == 1)
                                {
                                    DateTime fechaRevisionDatetime;
                                    if (DateTime.TryParseExact(fechaRevision, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaRevisionDatetime))
                                    {
                                        objRevision.FechaRevision = (DateTime.ParseExact(fechaRevision, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture));
                                    }
                                    else
                                    {
                                        objRevision.FechaRevision = (DateTime.ParseExact(fechaRevision, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                    }
                                    //objRevision.FechaRevision = DateTime.ParseExact(fechaRevision, "dd/MM/yyyy", CultureInfo.InvariantCulture);/* Convert.ToDateTime(fechaRevision);*/
                                    objRevision.Comentarios = comentarios;
                                }
                            }
                            objRevision.FechaCreacion = DateTime.Now;
                            objRevision.UsuarioCreacion = IdUser;
                            objRevision.Estado = true;
                            dbc.InformeEDRevision.Add(objRevision);
                            dbc.SaveChanges();

                            if (IdTicket != 0 && aplicaEstado == 1)
                            {
                                //Si es SOC la revisión técnica, aprobación técnica, revisión administrativa y aprobación administrativa NO APLICA.
                                if (IdResponsableInforme == 1 || (IdResponsableInforme == 2 && (i == 2 || i == 7)))
                                {
                                    //Insertando comentario en el ticket de solicitud
                                    var detailsTicket = new DETAILS_TICKET();

                                    if (i == 7)
                                    {
                                        var objTicket = dbc.TICKETs.Single(x => x.ID_TICK == IdTicket);
                                        objTicket.ID_STAT_END = 4;
                                        objTicket.MODIFIED_TICK = DateTime.Now;
                                        objTicket.FOR_REP = DateTime.Now;
                                        dbc.Entry(objTicket).State = EntityState.Modified;
                                        dbc.SaveChanges();

                                        detailsTicket.ID_STAT = 4;
                                        detailsTicket.ID_TYPE_DETA_TICK = 3;
                                        detailsTicket.IdTipoResolucion = 2;

                                    }
                                    else
                                    {
                                        detailsTicket.ID_TYPE_DETA_TICK = 1;
                                    }
                                    detailsTicket.ID_TICK = IdTicket;
                                    string detalle = "";
                                    if (i == 2) { detalle = "<strong>Elaboración realizada.</strong></br>"; }
                                    if (i == 3) { detalle = "<strong>Revisión Técnica realizada.</strong></br>"; }
                                    if (i == 4) { detalle = "<strong>Aprobación Técnica realizada.</strong></br>"; }
                                    if (i == 5) { detalle = "<strong>Revisión Administrativa realizada.</strong></br>"; }
                                    if (i == 6) { detalle = "<strong>Aprobación Administrativa.</strong></br>"; }
                                    if (i == 7) { detalle = "<strong>Envío realizado.</strong></br>"; }
                                    detailsTicket.COM_DETA_TICK = detalle + comentarios;
                                    detailsTicket.UserId = IdUser;
                                    detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                                    detailsTicket.MINUTES = 0;
                                    dbc.DETAILS_TICKET.Add(detailsTicket);
                                    dbc.SaveChanges();

                                }
                            }
                            if (IdTicket != 0 && aplicaEstado == 0 && i == 7)
                            {
                                //Insertando comentario en el ticket de solicitud
                                var objTicket = dbc.TICKETs.Single(x => x.ID_TICK == IdTicket);
                                objTicket.ID_STAT_END = 4;
                                objTicket.MODIFIED_TICK = DateTime.Now;
                                objTicket.FOR_REP = DateTime.Now;
                                dbc.Entry(objTicket).State = EntityState.Modified;
                                dbc.SaveChanges();

                                var detailsTicket = new DETAILS_TICKET();
                                detailsTicket.ID_STAT = 4;
                                detailsTicket.ID_TYPE_DETA_TICK = 3;
                                detailsTicket.IdTipoResolucion = 2;

                                detailsTicket.ID_TICK = IdTicket;
                                detailsTicket.COM_DETA_TICK = "Informe enviado.";
                                detailsTicket.UserId = IdUser;
                                detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                                detailsTicket.MINUTES = 0;
                                dbc.DETAILS_TICKET.Add(detailsTicket);
                                dbc.SaveChanges();
                            }
                        }
                    }
                    else //Edición
                    {
                        var objRev = dbc.InformeEDRevision.Single(x => x.Id == IdRevision);
                        if (objRev.Aplica == true)
                        {
                            DateTime FechaModificada = Convert.ToDateTime(objRev.FechaRevision);
                            DateTime FechaNueva, FechaModDa ;

                            if (DateTime.TryParseExact(fechaRevision, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaNueva))
                            {
                                FechaNueva = (DateTime.ParseExact(fechaRevision, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture));
                            }
                            else if (DateTime.TryParseExact(fechaRevision, "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaNueva))
                            {
                                FechaNueva = (DateTime.ParseExact(fechaRevision, "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                FechaNueva = (DateTime.ParseExact(fechaRevision, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                            }

                            //if (DateTime.TryParseExact(fechaRevision, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaModDa))
                            //{
                            //    FechaModDa = (DateTime.ParseExact(fechaRevision, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture));
                            //}
                            //else if (DateTime.TryParseExact(fechaRevision, "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaNueva))
                            //{
                            //    FechaNueva = (DateTime.ParseExact(fechaRevision, "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture));
                            //}
                            //else
                            //{
                            //    FechaNueva = (DateTime.ParseExact(fechaRevision, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                            //}
                            //= DateTime.ParseExact(fechaRevision,"dd MM, yyyy", CultureInfo.InvariantCulture);
                            if (FechaModificada != FechaNueva)
                            {
                                objRev.FechaRevision = DateTime.ParseExact(FechaModificada.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                objRev.FechaModifica = DateTime.Now;
                                objRev.UsuarioModifica = IdUser;
                                dbc.Entry(objRev).State = EntityState.Modified;
                                dbc.SaveChanges();

                                Logs objLogs = new Logs();
                                objLogs.IdTipoLog = 3;
                                objLogs.IdRegistro = IdRevision;
                                objLogs.FechaModificada = Convert.ToDateTime(FechaModificada);
                                objLogs.FechaNueva = DateTime.ParseExact(fechaRevision, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                objLogs.FechaCreacion = DateTime.Now;
                                objLogs.UsuarioCreacion = IdUser;
                                objLogs.Estado = true;
                                dbc.Logs.Add(objLogs);
                                dbc.SaveChanges();
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

        public ActionResult ListarEncargadosInforme(int id)
        {
            var result = dbc.CboListarEncargadosInforme(id).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarOPInformeFlag()
        {
            var result = dbc.ListarOPInformeFlag().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        public string AprobacionFormatoInforme()
        {
            int idDocuSale = Convert.ToInt32(Request.Params["id"]);

            DOCUMENT_SALE objDocumentSale = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == idDocuSale).FirstOrDefault();
            objDocumentSale.FlagNotificacionInforme = true;
            dbc.SaveChanges();
            return "Ok";
        }
        public ActionResult ComboFormatoAdicional(int NumDocuSale)
        {
            var result = dbc.CboFormatosAdicionales(NumDocuSale).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListFormatoAdicional(int NumDocuSale)
        {
            var result = dbc.CboFormatosAdicionales(NumDocuSale).ToList();

            return Json(new { Adicionales = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult EnviarAprobar(int IdInforme, string[] SelectedAdicionales)
        {

            InformeEDDetalle ObjDetalleInforme = dbc.InformeEDDetalle.Where(x => x.Id == IdInforme).FirstOrDefault();
            ObjDetalleInforme.IdEstadoInforme = 3;
            ObjDetalleInforme.NotificacionAprobacion = 1;

            var informeHistorial = new InformeHistorial
            {
                IdInformeDetalle = IdInforme,
                IdEstadoAnterior = 2,
                IdEstadoHistorial = 2,
                IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                FechaHistorial = DateTime.Now

            };

            dbc.InformeHistorial.Add(informeHistorial);

            //var selectedAdicionales = collection["SelectedAdicionales"].Split(',');

            int docusale = Convert.ToInt32(ObjDetalleInforme.IdDocuSale);
            int idPlantilla = Convert.ToInt32(dbc.PlantillaInforme.Where(x => x.Id_Docu_Sale == docusale).FirstOrDefault().Id);

            SelectedAdicionales = SelectedAdicionales.Where(sa => !string.IsNullOrEmpty(sa)).ToArray();

            if (SelectedAdicionales.Length == 1 && string.IsNullOrEmpty(SelectedAdicionales[0]))
            {
                var existingAdicionales = dbc.PlantillaAdicionales
                    .Where(pa => pa.IdDocuSale == ObjDetalleInforme.IdDocuSale)
                    .ToList();

                dbc.PlantillaAdicionales.RemoveRange(existingAdicionales);
            }
            else
            {
                var existingAdicionales = dbc.PlantillaAdicionales.Where(pa => pa.IdDocuSale == docusale).ToList();

                dbc.PlantillaAdicionales.RemoveRange(existingAdicionales);


                foreach (var AdicionalId in SelectedAdicionales)
                {
                    var Adicional = new PlantillaAdicionales
                    {
                        IdDocuSale = ObjDetalleInforme.IdDocuSale,
                        IdPlantilla = Convert.ToInt32(idPlantilla),
                        IdFormato = Convert.ToInt32(AdicionalId) // Usa AdicionalId como el ID del formato seleccionado
                    };

                    dbc.PlantillaAdicionales.Add(Adicional);
                }
            }
            dbc.SaveChanges();



            return Json(new { mensaje = "Datos procesados con éxito" });


        }


    }
}
