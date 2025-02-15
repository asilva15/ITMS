using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using System.IO;
using System.Text;

namespace ERPElectrodata.Controllers
{
    
    [Authorize]
    public class DocumentoVentaController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        //
        // GET: /DocumentoVenta/
        [Authorize]
        public ActionResult Index()
        {
            if ((int)Session["EJECUTIVOCOMERCIAL"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
            //if ((int)Session["EJECUTIVOCOMERCIAL"] == 1 || (int)Session["EJECUTIVOCOMERCIAL_OUTSOURCING"] == 1)
            //{
            //    return View();
            //}
            //return Content("Necesitas permisos para acceder a esta sección.");//prueba_outsourcing
        }

        [Authorize]
        public ActionResult Crear()
        {
            if ((int)Session["EJECUTIVOCOMERCIAL"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
            /*if ((int)Session["EJECUTIVOCOMERCIAL"] == 1 || (int)Session["EJECUTIVOCOMERCIAL_OUTSOURCING"] == 1)
            {
                return View();
            }
            return Content("Necesitas permisos para acceder a esta sección.");*/
        }

        [Authorize]
        public ActionResult Detalle(int id = 0)
        {
            ViewBag.Permitido = (int)Session["SUPERVISOR_EJECUTIVOCOMERCIAL"] == 1 ? 1 : 0;
            ViewBag.IdDocumentoVenta = id;
            return View();
        }

        public ActionResult VerDocumento(int id)
        {
            ViewBag.IdDocumentoVenta = id;
            ViewBag.ID_DOCU_SALE = dbc.DocumentoVentas.Single(x => x.Id == id).IdOP;
            return View();
        }

        [Authorize]
        public ActionResult ReporteSoluciones()
        {
            if ((int)Session["EJECUTIVOCOMERCIAL"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
            /*if ((int)Session["EJECUTIVOCOMERCIAL"] == 1 || (int)Session["EJECUTIVOCOMERCIAL_OUTSOURCING"] == 1)
            {
                return View();
            }
            return Content("Necesitas permisos para acceder a esta sección.");*/
        }

        public ActionResult EditarDocumento(int id)
        {
            DocumentoVenta objDocVenta = dbc.DocumentoVentas.Where(x => x.Id == id).SingleOrDefault();
            //int var = dbc.DOCUMENT_SALE.Where(y => y.ID_DOCU_SALE == id).Select(s => new { EstadoTipo = s.EstadoTipoProyecto });
            int idop = objDocVenta.IdOP.Value;
            DOCUMENT_SALE oCUMENT_SALE = dbc.DOCUMENT_SALE.Where(y => y.ID_DOCU_SALE == idop).SingleOrDefault();
            ViewBag.EstadoTipo = oCUMENT_SALE.EstadoTipoProyecto.Value;
            /*var objDocVenta = dbc.DocumentoVentas.Join(dbc.DOCUMENT_SALE,
                dv => dv.IdOP,
                ds => ds.ID_DOCU_SALE,
                (dv,ds)=> new { DocumentoVentas = dv, EstadoTipo = ds.EstadoTipoProyecto}).Where(dsAndDv => dsAndDv.DocumentoVentas.IdOP == id);*/
            return View(objDocVenta);
        }

        [ValidateInput(false)]
        public ActionResult CrearRegistro(DocumentoVenta objDocumento)
        {
            int ID_ACCO, UserId;

            try
            {
                ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                UserId = Convert.ToInt32(Session["UserId"].ToString());

                var DocVenta = dbc.DocumentoVentas.Where(x => x.IdOP == objDocumento.IdOP);

                if (DocVenta.Count() > 0 )
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                }
                if (objDocumento.IdOP == null || objDocumento.IdTipoOP == null || objDocumento.IdCliente == null || objDocumento.IdClienteFinal == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
                }

                objDocumento.FechaCreacion = DateTime.Now;
                objDocumento.IdAcco = ID_ACCO;
                objDocumento.UserIdCreacion = UserId;
                dbc.DocumentoVentas.Add(objDocumento);
                dbc.SaveChanges();

                int IdDocumentoVenta = objDocumento.Id;

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + IdDocumentoVenta.ToString() + "');}window.onload=init;</script>");
            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
        }

        [ValidateInput(false)]
        public ActionResult EditarRegistro(DocumentoVenta objDocumento)
        {
            try
            {
                DocumentoVenta objDocVenta = dbc.DocumentoVentas.Where(x => x.Id == objDocumento.Id).SingleOrDefault();

                if (objDocumento.IdOP == null || objDocumento.IdTipoOP == null || objDocumento.IdCliente == null || objDocumento.IdClienteFinal == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
                }
                objDocVenta.IdCliente = objDocumento.IdCliente;
                objDocVenta.IdClienteFinal = objDocumento.IdClienteFinal;
                objDocVenta.IdOP = objDocumento.IdOP;
                objDocVenta.IdTipoOP = objDocumento.IdTipoOP;
                objDocVenta.Servicio = objDocumento.Servicio;
                objDocVenta.TipoCambio = objDocumento.TipoCambio;
                objDocVenta.MontoDolares = objDocumento.MontoDolares;
                objDocVenta.MontoSoles = objDocumento.MontoSoles;
                objDocVenta.Descripcion = objDocumento.Descripcion;
                objDocVenta.FechaInicioContrato = objDocumento.FechaInicioContrato;
                objDocVenta.FechaFinContrato = objDocumento.FechaFinContrato;
                objDocVenta.FechaInicioSoporte = objDocumento.FechaInicioSoporte;
                objDocVenta.FechaFinSoporte = objDocumento.FechaFinSoporte;

                dbc.Entry(objDocVenta).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0');}window.onload=init;</script>");
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

            int IdSolucionTI = Convert.ToInt32(Request.Params["IdSolucionTI"].ToString());
            int IdTipoOP = Convert.ToInt32(Request.Params["IdTipoOP"].ToString());
            int IdMarca = Convert.ToInt32(Request.Params["IdMarca"].ToString());
            int IdCliente = Convert.ToInt32(Request.Params["IdCliente"].ToString());
            int IdClienteFinal = Convert.ToInt32(Request.Params["IdClienteFinal"].ToString());
            string PalabraClave = Request.Params["PalabraClave"].ToString();
            int TipoProyecto = Convert.ToInt32(Request.Params["TipoProyecto"].ToString());
            string MontoMinimoSoles = Convert.ToString(Request.Params["MontoMinimoSoles"]);
            string MontoMaximoSoles = Convert.ToString(Request.Params["MontoMaximoSoles"]);
            string MontoMinimoDolares = Convert.ToString(Request.Params["MontoMinimoDolares"]);
            string MontoMaximoDolares = Convert.ToString(Request.Params["MontoMaximoDolares"]);
            string NroDocumento = Convert.ToString(Request.Params["NroDocumento"]);


            string strFechaInicio = Request.Params["FechaIniContrato"].ToString();
            string strFechaFin = Request.Params["FechaFinContrato"].ToString();
            DateTime FechaIniContrato = Convert.ToDateTime("1900/01/01");
            DateTime FechaFinContrato = Convert.ToDateTime("9999/12/31");

            int MinimoSoles = 0;
            int MaximoSoles = 999999999;
            int MinimoDolares = 0;
            int MaximoDolares = 999999999;

            if (!String.IsNullOrEmpty(strFechaInicio))
            {
                FechaIniContrato = Convert.ToDateTime(strFechaInicio);
            }
            if (!String.IsNullOrEmpty(strFechaFin))
            {
                FechaFinContrato = Convert.ToDateTime(strFechaFin);
            }
            if (!String.IsNullOrEmpty(MontoMinimoSoles))
            {
                MinimoSoles = Convert.ToInt32(MontoMinimoSoles);
            }
            if (!String.IsNullOrEmpty(MontoMaximoSoles))
            {
                MaximoSoles = Convert.ToInt32(MontoMaximoSoles);
            }
            if (!String.IsNullOrEmpty(MontoMinimoDolares))
            {
                MinimoDolares = Convert.ToInt32(MontoMinimoDolares);
            }
            if (!String.IsNullOrEmpty(MontoMaximoDolares))
            {
                MaximoDolares = Convert.ToInt32(MontoMaximoDolares);
            }

            int total = 0;

            List<object> Registros = new List<object>(dbc.DvListarRegistro(IdSolucionTI, IdTipoOP, IdMarca, IdCliente, IdClienteFinal, FechaIniContrato, FechaFinContrato, PalabraClave, TipoProyecto, MinimoSoles, MaximoSoles, MinimoDolares,MaximoDolares,NroDocumento));

            //List<object> Registros = new List<object>(dbc.DvListarRegistro(IdSolucionTI, IdTipoOP, IdMarca, IdCliente, IdClienteFinal, FechaIniContrato, FechaFinContrato, PalabraClave, TipoProyecto, MinimoSoles, MaximoSoles, MinimoDolares,MaximoDolares));
            total = Registros.Count();

            var query2 = Registros.Skip(skip).Take(take);

            return Json(new { Grid = query2, Cantidad = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosRegistro(int id = 0)
        {

            List<object> Registros = new List<object>(dbc.DvObtenerDatosRegistro(id)).ToList();

            return Json(new { Data = Registros }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosMarca(int id = 0)
        {

            List<object> Registros = new List<object>(dbc.DvObtenerDatosMarca(id)).ToList();

            return Json(new { Data = Registros }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearEditarMarca(int id = 0)
        {
            ViewBag.IdDocumentoVentaMarca = id;
            int IdDocumentoVenta = Convert.ToInt32(Request.Params["dv"].ToString());
            ViewBag.IdDocumentoVenta = IdDocumentoVenta;
            var query = dbc.DocumentoVentaMarcas.Where(x => x.Id == id).SingleOrDefault();

            if (query != null)
            {
                ViewBag.IdMarca = query.IdMarca;
                ViewBag.TipoCambio = query.TipoCambio;
                ViewBag.MontoDolares = query.MontoDolares;
                ViewBag.MontoSoles = query.MontoSoles;
                ViewBag.Descripcion = query.Descripcion;
                ViewBag.IdSolucionTI = query.IdSolucionTI;
            }

            return View();
        }

        public ActionResult Guardar()
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int IdDocVentaMarca = Convert.ToInt32(Request.Params["IdDocVentaMarca"].ToString());
            int IdDocVenta = Convert.ToInt32(Request.Params["IdDocVenta"].ToString());
            int IdMarca = 0;
            int IdSolucionTI = 0;
            decimal TipoCambio = 0, MontoDolares=0, MontoSoles=0;
            string Descripcion = "";
            try
            {
                IdMarca = Convert.ToInt32(Request.Params["Marca"].ToString());
                IdSolucionTI = Convert.ToInt32(Request.Params["IdSolucionTI"].ToString());
                TipoCambio = Convert.ToDecimal(Request.Params["TipoCambio"].ToString());
                MontoDolares = Convert.ToDecimal(Request.Params["MontoDol"].ToString());
                MontoSoles = Convert.ToDecimal(Request.Params["MontoSol"].ToString());
                Descripcion = Request.Params["Descripcion"].ToString();

                if (IdMarca == null || IdSolucionTI == null || TipoCambio == null || MontoDolares ==null || MontoSoles == null)
                {
                    return Content("ERROR_EXISTE");
                }
            }
            catch
            {
                return Content("ERROR_EXISTE");
            }
            if (IdDocVentaMarca == 0)
            {
                var objDocMarca = dbc.DocumentoVentaMarcas.Where(x => x.IdMarca == IdMarca && x.IdSolucionTI == IdSolucionTI && x.Estado == true && x.IdDocumentoVenta == IdDocVenta);
                if (objDocMarca.Count() > 0)
                {
                    return Content("ERROR_EXISTE");
                }

                var objMarca = new DocumentoVentaMarca();
                objMarca.IdSolucionTI = IdSolucionTI;
                objMarca.IdMarca = IdMarca;
                objMarca.IdDocumentoVenta = IdDocVenta;
                objMarca.TipoCambio = TipoCambio;
                objMarca.MontoDolares = MontoDolares;
                objMarca.MontoSoles = MontoSoles;
                objMarca.Descripcion = Descripcion;
                objMarca.Estado = true;
                objMarca.FechaCreacion = DateTime.Now;
                objMarca.UserIdCreacion = UserId;
                dbc.DocumentoVentaMarcas.Add(objMarca);
            }
            else
            {
                var objDocMarca = dbc.DocumentoVentaMarcas.Where(x => x.IdMarca == IdMarca && x.IdSolucionTI == IdSolucionTI && x.Estado == true && x.Id != IdDocVentaMarca && x.IdDocumentoVenta == IdDocVenta);
                if (objDocMarca.Count() > 0)
                {
                    return Content("ERROR_EXISTE");
                }

                DocumentoVentaMarca objMarca = dbc.DocumentoVentaMarcas.Where(x => x.Id == IdDocVentaMarca).SingleOrDefault();
                objMarca.IdSolucionTI = IdSolucionTI;
                objMarca.IdMarca = IdMarca;
                objMarca.TipoCambio = TipoCambio;
                objMarca.MontoDolares = MontoDolares;
                objMarca.MontoSoles = MontoSoles;
                objMarca.Descripcion = Descripcion;
                dbc.Entry(objMarca).State = EntityState.Modified;
            }
            dbc.SaveChanges();

            return Content("OK");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AdjuntarArchivo(IEnumerable<HttpPostedFileBase> files, int idDocVenta, int idTipoDocumento, string NumDoc)
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
                    ATTA.IdDocumentoVenta = idDocVenta;
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
            var query = dbc.DvObtenerArchivos(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarMarca(int id = 0)
        {
            DocumentoVentaMarca objMarca = dbc.DocumentoVentaMarcas.Where(x => x.Id == id).SingleOrDefault();
            objMarca.Estado = false;
            dbc.Entry(objMarca).State = EntityState.Modified;
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
            }catch(Exception){
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
            }catch(Exception){
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
