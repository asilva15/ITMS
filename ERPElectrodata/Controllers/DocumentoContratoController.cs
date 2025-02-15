using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;

namespace ERPElectrodata.Controllers
{

    [Authorize]
    public class DocumentoContratoController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        
        //
        // GET: /DocumentoContrato/
        [Authorize]
        public ActionResult Index()
        {
            if ((int)Session["EJECUTIVOCOMERCIAL"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }
        
        [Authorize]
        public ActionResult Crear()
        {
            if ((int)Session["EJECUTIVOCOMERCIAL"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }

        [Authorize]
        public ActionResult Detalle(int id = 0)
        {
            ViewBag.Permitido = (int)Session["SUPERVISOR_EJECUTIVOCOMERCIAL"] == 1 ? 1 : 0;
            ViewBag.IdDocumentoContrato = id;
            return View();
        }

        public ActionResult VerDocumento(int id)
        {
            ViewBag.IdDocumentoContrato = id;
            ViewBag.ID_DOCU_SALE = dbc.DocumentoContrato.Single(x => x.Id == id).IdOP;
            return View();
        }
        
        [Authorize]
        public ActionResult ReporteContratos()
        {
            if ((int)Session["EJECUTIVOCOMERCIAL"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }
        
        public ActionResult EditarDocumento(int id)
        {
            DocumentoContrato objDocContrato = dbc.DocumentoContrato.Where(x => x.Id == id).SingleOrDefault();
            return View(objDocContrato);
        }
        
        [ValidateInput(false)]
        public ActionResult CrearRegistro(DocumentoContrato objDocumento)
        {
            int ID_ACCO, UserId;

            try
            {
                ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                UserId = Convert.ToInt32(Session["UserId"].ToString());

                var idopn = objDocumento.IdOP == null ? 0 : objDocumento.IdOP;

                if (idopn != 0)
                {
                    var DocContrato = dbc.DocumentoContrato.Where(x => x.IdOP == objDocumento.IdOP);

                    if (DocContrato.Count() > 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                    }
                }
                if (objDocumento.EstadoFirmaContrato == null || objDocumento.ConceptoContrato == null || objDocumento.IdCliente == null || objDocumento.IdClienteFinal == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
                }

                objDocumento.FechaCreacion = DateTime.Now;
                objDocumento.IdAcco = ID_ACCO;
                objDocumento.UserIdCreacion = UserId;
                if (objDocumento.IdOP == null) objDocumento.IdOP = 0;
                dbc.DocumentoContrato.Add(objDocumento);
                dbc.SaveChanges();

                int IdDocumentoContrato = objDocumento.Id;

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + IdDocumentoContrato.ToString() + "');}window.onload=init;</script>");
            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
        }
        
                
        [ValidateInput(false)]
        public ActionResult EditarRegistroContrato(DocumentoContrato objDocumento)
        {
            try
            {
                //Registro de la tabla que queremos editar
                DocumentoContrato objDocContrato = dbc.DocumentoContrato.Where(x => x.Id == objDocumento.Id).SingleOrDefault();

                //Validaciones
                //Validar que IdOP no sea nulo
                if (objDocumento.ConceptoContrato == null || objDocumento.IdCliente == null || objDocumento.IdClienteFinal == null
                    )
                {
                    return Content(@"
                        <script type='text/javascript'> 
                            function init() {
                                if(top.uploadDone) top.uploadDone('ERROR','2');
                            }
                            window.onload=init;
                        </script>");
                }

                //Edicion
                objDocContrato.IdCliente = objDocumento.IdCliente;
                objDocContrato.IdClienteFinal = objDocumento.IdClienteFinal;
                objDocContrato.IdOP = objDocumento.IdOP;
                objDocContrato.IdFirmanteContrato = objDocumento.IdFirmanteContrato;
                objDocContrato.ConceptoContrato = objDocumento.ConceptoContrato;
                objDocContrato.NombreDocumentoContrato = objDocumento.NombreDocumentoContrato;
                objDocContrato.EstadoFirmaContrato = objDocumento.EstadoFirmaContrato;
                objDocContrato.TipoMonedaContrato = objDocumento.TipoMonedaContrato;
                objDocContrato.MontoContratoConIgv = objDocumento.MontoContratoConIgv;
                objDocContrato.VisadoLegal = objDocumento.VisadoLegal;
                objDocContrato.PorcentajeMaximo = objDocumento.PorcentajeMaximo;
                objDocContrato.Observaciones = objDocumento.Observaciones;
                objDocContrato.FechaFirmaContrato = objDocumento.FechaFirmaContrato;
                objDocContrato.FechaInicioContrato = objDocumento.FechaInicioContrato;
                objDocContrato.FechaFinContrato = objDocumento.FechaFinContrato;
                objDocContrato.FechaInicioSoporte = objDocumento.FechaInicioSoporte;
                objDocContrato.FechaFinSoporte = objDocumento.FechaFinSoporte;

                //Guardar el registro
                dbc.Entry(objDocContrato).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content(@"
                    <script type='text/javascript'>
                        function init() {
                            if(top.uploadDone) top.uploadDone('OK','0');
                        }
                        window.onload=init;
                    </script>");
            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
        }
        
        public ActionResult Listar()
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            int ConceptoContrato = Convert.ToInt32(Request.Params["ConceptoContrato"].ToString());
            int NumeroOP = Convert.ToInt32(Request.Params["NumeroOP"].ToString());
            int EstadoFirmaContrato = Convert.ToInt32(Request.Params["EstadoFirmaContrato"].ToString());

            int IdCliente = Convert.ToInt32(Request.Params["IdCliente"].ToString());
            int IdClienteFinal = Convert.ToInt32(Request.Params["IdClienteFinal"].ToString());            

            string strFechaInicio = Request.Params["FechaIniContrato"].ToString();
            string strFechaFin = Request.Params["FechaFinContrato"].ToString();
            DateTime FechaIniContrato = Convert.ToDateTime("1900/01/01");
            DateTime FechaFinContrato = Convert.ToDateTime("9999/12/31");
            if (!String.IsNullOrEmpty(strFechaInicio))
            {
                FechaIniContrato = Convert.ToDateTime(strFechaInicio);
            }
            if (!String.IsNullOrEmpty(strFechaFin))
            {
                FechaFinContrato = Convert.ToDateTime(strFechaFin);
            }

            int total = 0;
            List<object> Registros = new List<object>(dbc.DvListarRegistroContrato(ConceptoContrato, NumeroOP, EstadoFirmaContrato, IdCliente, IdClienteFinal, FechaIniContrato, FechaFinContrato));
            //List<object> Registros = new List<object>(dbc.DvListarRegistroContrato(3404, "0", 0, 0, FechaIniContrato, FechaFinContrato, "0"));

            total = Registros.Count();

            var query2 = Registros.Skip(skip).Take(take);

            return Json(new { Grid = query2, Cantidad = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarNumeroOP()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarNumeroOP(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarPorOP(int id)
        {
            //int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.BuscarPorOP(id).ToList().SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosRegistroContrato(int id = 0)
        {

            List<object> Registros = new List<object>(dbc.DvObtenerDatosRegistroContrato(id)).ToList();

            return Json(new { Data = Registros }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ObtenerDatosFianza(int id = 0)
        {

            List<object> Registros = new List<object>(dbc.DvObtenerDatosFianza(id)).ToList();

            return Json(new { Data = Registros }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosPoliza(int id = 0)
        {

            List<object> Registros = new List<object>(dbc.DvObtenerDatosPoliza(id)).ToList();

            return Json(new { Data = Registros }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearEditarFianza(int id = 0)
        {
            ViewBag.IdDocumentoContratoFianza = id;
            int IdDocumentoContrato = Convert.ToInt32(Request.Params["dv"].ToString());
            ViewBag.IdDocumentoContrato = IdDocumentoContrato;
            var query = dbc.Fianza.Where(x => x.Id == id).SingleOrDefault();

            if (query != null)
            {
                ViewBag.TipoFianzaContrato = query.TipoFianza;
                ViewBag.Importe = query.Importe;
                ViewBag.FechaInicio = query.FechaInicio;
                ViewBag.FechaFin = query.FechaFin;
                ViewBag.Descripcion = query.Descripcion;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdjuntarArchivoFianza(int idFianza)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Request;
            if (httpRequest.Files.Count > 0)
            {
                //Guardando archivo adjunto
                foreach (string file in httpRequest.Files)
                {
                    //Se obtiene el objesto archivo
                    var postedFile = httpRequest.Files[file];
                    //Obtenemos nombre de archivo
                    string nombreArchivo = Path.GetFileNameWithoutExtension(postedFile.FileName);
                    //Obtenemos el tamaño de archivo en bytes
                    int size = postedFile.ContentLength;
                    //Obtenemos extensión de archivo
                    string extension = Path.GetExtension(postedFile.FileName);
                    try
                    {
                        var Docs = new FianzaDocumento();
                        Docs.NombreArchivo = Path.GetFileNameWithoutExtension(nombreArchivo);
                        Docs.Extension = Path.GetExtension(extension);
                        Docs.IdFianza = idFianza;
                        Docs.FechaCreacion = DateTime.Now;
                        Docs.UserIdCreacion = Convert.ToInt32(Session["UserId"]);

                        dbc.FianzaDocumento.Add(Docs);
                        dbc.SaveChanges();
                        //if (!Directory.Exists(Server.MapPath("~/Adjuntos/DocumnentControl/RegistrosOP/"))- Directory.CreateDirectory
                        postedFile.SaveAs(Server.MapPath("~/Adjuntos/DocumentControl/RegistrosOP/" + Docs.NombreArchivo + "_" + Convert.ToString(Docs.Id) + Docs.Extension));

                    }
                    catch(Exception e)
                    {
                        return Content("alert('" + e.Message + "')") ;
                    }
                }
            }


            return Content("");
        }

        public ActionResult AdjuntarArchivoPoliza(int idPoliza)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Request;
            if (httpRequest.Files.Count > 0)
            {
                //Guardando archivo adjunto
                foreach (string file in httpRequest.Files)
                {
                    //Se obtiene el objesto archivo
                    var postedFile = httpRequest.Files[file];
                    //Obtenemos nombre de archivo
                    string nombreArchivo = Path.GetFileNameWithoutExtension(postedFile.FileName);
                    //Obtenemos el tamaño de archivo en bytes
                    int size = postedFile.ContentLength;
                    //Obtenemos extensión de archivo
                    string extension = Path.GetExtension(postedFile.FileName);
                    try
                    {
                        var Docs = new PolizaDocumento();
                        Docs.NombreArchivo = Path.GetFileNameWithoutExtension(nombreArchivo);
                        Docs.Extension = Path.GetExtension(extension);
                        Docs.IdPoliza = idPoliza;
                        Docs.FechaCreacion = DateTime.Now;
                        Docs.UserIdCreacion = Convert.ToInt32(Session["UserId"]);

                        dbc.PolizaDocumento.Add(Docs);
                        dbc.SaveChanges();

                        postedFile.SaveAs(Server.MapPath("~/Adjuntos/DocumentControl/RegistrosOP/" + Docs.NombreArchivo + "_" + Convert.ToString(Docs.Id) + Docs.Extension));

                    }
                    catch (Exception e)
                    {
                        return Content("alert('" + e.Message + "')");
                    }
                }
            }


            return Content("");
        }
        public ActionResult CrearEditarPoliza(int id = 0)
        {
            ViewBag.IdDocumentoContratoPoliza = id;
            int IdDocumentoContrato = Convert.ToInt32(Request.Params["dv"].ToString());
            ViewBag.IdDocumentoContrato = IdDocumentoContrato;
            var query = dbc.Poliza.Where(x => x.Id == id).SingleOrDefault();

            if (query != null)
            {
                ViewBag.TipoPolizaContrato = query.TipoPoliza;
                ViewBag.Importe = query.Importe;
                ViewBag.FechaInicio = query.FechaInicio;
                ViewBag.FechaFin = query.FechaFin;
                ViewBag.Descripcion = query.Descripcion;
            }

            return View();
        }

        public ActionResult ListaTipoFianza()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarTipoFianza(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaTipoPoliza()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarTipoPoliza(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarFianza()
        {
            var objFianza = new Fianza();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int IdDocContratoFianza = Convert.ToInt32(Request.Params["IdDocContratoFianza"].ToString());
            int IdDocContrato = Convert.ToInt32(Request.Params["IdDocContrato"].ToString());
            int TipoFianza = 0;
            decimal Importe = 0; 
            DateTime FechaInicio = Convert.ToDateTime("1900/01/01");
            DateTime FechaFin = Convert.ToDateTime("9999/12/31");
            string Descripcion = "";
            try
            {
                TipoFianza = Convert.ToInt32(Request.Params["TipoFianzaContrato"].ToString());
                Importe = Convert.ToDecimal(Request.Params["Importe"].ToString());
                FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"].ToString());
                FechaFin = Convert.ToDateTime(Request.Params["FechaFin"].ToString());
                Descripcion = Request.Params["Descripcion"].ToString();

                if (TipoFianza == null || Importe == null || FechaInicio == null || FechaFin == null)
                {
                    return Json(new { IdFianza = 0, Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { IdFianza = 0, Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            if (IdDocContratoFianza == 0)
            {
                var objDocFianza = dbc.Fianza.Where(x => x.TipoFianza == TipoFianza && x.Importe == Importe && x.Estado == true && x.IdDocumentoContrato == IdDocContrato);
                if (objDocFianza.Count() > 0)
                {
                    return Json(new { IdFianza = 0, Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }
              
                objFianza.IdDocumentoContrato = IdDocContrato;
                objFianza.TipoFianza = TipoFianza;
                objFianza.Importe = Importe;
                objFianza.FechaInicio = FechaInicio;
                objFianza.FechaFin = FechaFin;
                objFianza.Descripcion = Descripcion;
                objFianza.Estado = true;
                objFianza.FechaCreacion = DateTime.Now;
                objFianza.UserIdCreacion = UserId;
                dbc.Fianza.Add(objFianza);
            }
            else
            {
                var objDocFianza = dbc.Fianza.Where(x => x.TipoFianza == TipoFianza && x.Importe == Importe && x.Estado == true && x.Id != IdDocContratoFianza && x.IdDocumentoContrato == IdDocContrato);
                if (objDocFianza.Count() > 0)
                {
                    return Json(new { IdFianza = 0, Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }

                objFianza = dbc.Fianza.Where(x => x.Id == IdDocContratoFianza).SingleOrDefault();
                objFianza.TipoFianza = TipoFianza;
                objFianza.Importe = Importe;
                objFianza.FechaInicio = FechaInicio;
                objFianza.FechaFin = FechaFin;
                objFianza.Descripcion = Descripcion;
                dbc.Entry(objFianza).State = EntityState.Modified;

            }
            dbc.SaveChanges();

            return Json(new { IdFianza=objFianza.Id, Respuesta="OK" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GuardarPoliza()
        {
            var objPoliza = new Poliza();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int IdDocContratoPoliza = Convert.ToInt32(Request.Params["IdDocContratoPoliza"].ToString());
            int IdDocContrato = Convert.ToInt32(Request.Params["IdDocContrato"].ToString());
            int TipoPoliza = 0;
            decimal Importe = 0;
            DateTime FechaInicio = Convert.ToDateTime("1900/01/01");
            DateTime FechaFin = Convert.ToDateTime("9999/12/31");
            string Descripcion = "";
            try
            {
                TipoPoliza = Convert.ToInt32(Request.Params["TipoPolizaContrato"].ToString());
                Importe = Convert.ToDecimal(Request.Params["Importe"].ToString());
                FechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"].ToString());
                FechaFin = Convert.ToDateTime(Request.Params["FechaFin"].ToString());
                Descripcion = Request.Params["Descripcion"].ToString();

                if (TipoPoliza == null || Importe == null || FechaInicio == null || FechaFin == null)
                {
                    return Json(new { IdPoliza = 0, Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { IdPoliza = 0, Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            if (IdDocContratoPoliza == 0)
            {
                var objDocPoliza = dbc.Poliza.Where(x => x.TipoPoliza == TipoPoliza && x.Importe == Importe && x.Estado == true && x.IdDocumentoContrato == IdDocContrato);
                if (objDocPoliza.Count() > 0)
                {
                    return Json(new { IdPoliza = 0, Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }

                objPoliza.IdDocumentoContrato = IdDocContrato;
                objPoliza.TipoPoliza = TipoPoliza;
                objPoliza.Importe = Importe;
                objPoliza.FechaInicio = FechaInicio;
                objPoliza.FechaFin = FechaFin;
                objPoliza.Descripcion = Descripcion;
                objPoliza.Estado = true;
                objPoliza.FechaCreacion = DateTime.Now;
                objPoliza.UserIdCreacion = UserId;
                dbc.Poliza.Add(objPoliza);
            }
            else
            {
                var objDocPoliza = dbc.Poliza.Where(x => x.TipoPoliza == TipoPoliza && x.Importe == Importe && x.Estado == true && x.Id != IdDocContratoPoliza && x.IdDocumentoContrato == IdDocContrato);
                if (objDocPoliza.Count() > 0)
                {
                    Json(new { IdPoliza = 0, Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }

                objPoliza = dbc.Poliza.Where(x => x.Id == IdDocContratoPoliza).SingleOrDefault();
                objPoliza.TipoPoliza = TipoPoliza;
                objPoliza.Importe = Importe;
                objPoliza.FechaInicio = FechaInicio;
                objPoliza.FechaFin = FechaFin;
                objPoliza.Descripcion = Descripcion;
                dbc.Entry(objPoliza).State = EntityState.Modified;
            }
            dbc.SaveChanges();

            return Json(new { IdPoliza = objPoliza.Id, Respuesta = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AdjuntarArchivo(IEnumerable<HttpPostedFileBase> files, int idDocContrato, int idTipoDocumento, string NumDoc)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            foreach (var file in files)
            {
                try
                {
                    var ATTA = new DOCUMENT_CONTROL_ATTACHED();

                    ATTA.NAM_DOCU_CONT_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                    ATTA.EXT_DOCU_CONT_ATTA = Path.GetExtension(file.FileName);
                    ATTA.CREATED = DateTime.Now;
                    ATTA.UserId = UserId;
                    ATTA.IdDocumentoContrato = idDocContrato;
                    ATTA.IdTipoDocumento = idTipoDocumento;
                    ATTA.NumeroDocumento = NumDoc;
                    ATTA.Estado = true;

                    dbc.DOCUMENT_CONTROL_ATTACHED.Add(ATTA);
                    dbc.SaveChanges();

                    file.SaveAs(Server.MapPath("~/Adjuntos/DocumentControl/RegistrosOP/") + ATTA.NAM_DOCU_CONT_ATTA + "_" + Convert.ToString(ATTA.ID_DOCU_CONT_ATTA) + ATTA.EXT_DOCU_CONT_ATTA);

                }
                catch
                {
                    return Content("");
                }
            }

            return Content("");
        }

        public ActionResult ObtenerArchivos(int id = 0)
        {
            var query = dbc.DvObtenerArchivosContrato(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarFianza(int id = 0)
        {
            Fianza objFianza = dbc.Fianza.Where(x => x.Id == id).SingleOrDefault();
            objFianza.Estado = false;
            dbc.Entry(objFianza).State = EntityState.Modified;
            dbc.SaveChanges();

            return Content("");
        }
        public ActionResult EliminarPoliza(int id = 0)
        {
            Poliza objPoliza = dbc.Poliza.Where(x => x.Id == id).SingleOrDefault();
            objPoliza.Estado = false;
            dbc.Entry(objPoliza).State = EntityState.Modified;
            dbc.SaveChanges();

            return Content("");
        }

        public ActionResult EditarDocumentoAdjunto(int id = 0)
        {
            int TipoDocumento = Convert.ToInt32(Request.QueryString["td"]);
            string NumeroDocumento = Request.QueryString["nd"].ToString();

            int UserId = Convert.ToInt32(Session["UserId"]);

            try
            {
                DOCUMENT_CONTROL_ATTACHED objDocumento = dbc.DOCUMENT_CONTROL_ATTACHED.Where(x => x.ID_DOCU_CONT_ATTA == id).SingleOrDefault();
                objDocumento.IdTipoDocumento = TipoDocumento;
                objDocumento.NumeroDocumento = NumeroDocumento;
                objDocumento.UsuarioModifica = UserId;
                objDocumento.FechaModifica = DateTime.Now;
                dbc.Entry(objDocumento).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("OK");
            }
            catch (Exception)
            {
                return Content("ERROR");
            }
        }

        public ActionResult EliminarDocumentoAdjunto(int id = 0)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);

            try
            {
                DOCUMENT_CONTROL_ATTACHED objDocumento = dbc.DOCUMENT_CONTROL_ATTACHED.Where(x => x.ID_DOCU_CONT_ATTA == id).SingleOrDefault();
                objDocumento.Estado = false;
                objDocumento.UsuarioModifica = UserId;
                objDocumento.FechaModifica = DateTime.Now;
                dbc.Entry(objDocumento).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("OK");
            }
            catch (Exception)
            {
                return Content("ERROR");
            }
        }

        public ActionResult VerDoc(string id = "")
        {
            try
            {
                ViewBag.Documento = id;
                return View();
            }
            catch
            {
                return Content("Please Close Session");
            }

        }

        public FileResult DescargarArchivo(string id = "")
        {
            try
            {
                FileStream fileStream = System.IO.File.OpenRead(Server.MapPath("~/Adjuntos/DocumentControl/RegistrosOP/" + id));
                MemoryStream storeStream = new MemoryStream();

                storeStream.SetLength(fileStream.Length);
                fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                byte[] byteArray = storeStream.ToArray();

                storeStream.Flush();
                fileStream.Close();
                storeStream.Close();

                Random r = new Random();
                int aleatorio = r.Next(10000, 99999);

                Response.Clear();

                if ((id.ToLower()).Contains(".pdf"))
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".txt"))
                {
                    Response.ContentType = "text/text";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".txt");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".jpg"))
                {
                    Response.ContentType = "image/jpeg";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".jpg");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".png"))
                {
                    Response.ContentType = "image/png";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".png");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else
                {
                    string filename = "~/Adjuntos/DocumentControl/RegistrosOP/" + id;
                    return File(filename, "application/pdf", id);
                }
            }
            catch
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

                return File(stream, "text/plain", "Error.txt");
            }

        }

        

    }
}
