using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.IO.Compression;
using Ionic.Zip;
using ERPElectrodata.Objects;

namespace ERPElectrodata.Controllers
{
    public class DocumentExpenseController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        AppLogEntities dba = new AppLogEntities();
        //
        // GET: /DocumentExpense/

        public ActionResult Index()
        {
            return View();
        }

        ///BETSY
        //---Editar
        public ActionResult EditarDetalleDocument(int id = 0)
        {
            ViewBag.ID_REQU_EXPE_2 = id;
            ViewBag.ID_TIPO_DOCUMENTO = Convert.ToInt32(dbc.ObtenerIdTipoDocumentExpense(id).FirstOrDefault());
            ViewBag.ID_CODIGO_ARTICULO = Convert.ToInt32(dbc.ObtenerIdCodigoArtDocumentExpense(id).FirstOrDefault());//
            ViewBag.ID_TIPO_IMPUESTO = Convert.ToInt32(dbc.ObtenerIdTipoImpuestoDocumentExpense(id).FirstOrDefault());
            ViewBag.IGV = Convert.ToString(dbc.ObtenerIGVDocumentExpense(id).FirstOrDefault());
            ViewBag.FECHA = dbc.ObtenerFechaDocumentExpense(id).FirstOrDefault();
            ViewBag.MONTO = Convert.ToString(dbc.ObtenerMontoDocumentExpense(id).FirstOrDefault());
            ViewBag.SolicitanteDocExpense = Convert.ToString(dbc.ObtenerRegistadorDocumentExpense(id).FirstOrDefault());
            if (ViewBag.ID_TIPO_IMPUESTO == 4)
            {
                ViewBag.TotalIGV = Math.Round(0.00, 2);
                ViewBag.MontoTotal = ViewBag.MONTO;
            }
            else
            {
                ViewBag.TotalIGV = Math.Round((Convert.ToDecimal(ViewBag.MONTO) * Convert.ToDecimal(ViewBag.IGV.Replace("%", ""))) / 100, 2);
                ViewBag.MontoTotal = Convert.ToDecimal(ViewBag.MONTO) + ((Convert.ToDecimal(ViewBag.MONTO) * Convert.ToDecimal(ViewBag.IGV.Replace("%", ""))) / 100);
            }
            //ViewBag.TotalIGV = ;
            //ViewBag.TotalConIGV = ;

            ViewBag.DOC_DATE_MAX_EDIT = String.Format("{0:g}", DateTime.Today);
            return View();
        }

        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarDocument(IEnumerable<HttpPostedFileBase> files, DOCUMENT_EXPENSE de)
        {
            //int id_docu = de.ID_DOCU_EXPE;
            string serie = Convert.ToString(Request.Params["txtSerie"]);
            int id_tipo_deli_sust = 0;

            int numero = 0;
            if (Request.Params["txtNumero"] == "")
            {
                numero = -1;
            }
            else
            {
                try
                {
                    numero = Convert.ToInt32(Request.Params["txtNumero"]);
                }
                catch (Exception e)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneMsnEDDocument) top.uploadDoneMsnEDDocument('ERROR','El campo número acepta maxímo 10 carácteres');}window.onload=init;</script>");
                }
            }

            string ruc = Convert.ToString(Request.Params["txtProveedorRuc"]) + "P";

            string proveedor = Convert.ToString(Request.Params["txtProveedorNom"]);

            string comentario = Convert.ToString(Request.Params["txtComentario"]);

            int ID = Convert.ToInt32(Request.Params["TXT_ID_DOCUMENT"]);//

            string razon = Convert.ToString(Request.Params["txtRazon"]);

            string destino = Convert.ToString(Request.Params["txtDestino"]);

            int id_tipo = Convert.ToInt32(de.ID_TYPE_DOCU_EXPE);

            int id_cod_articulo;
            if (Request.Params["CodigoArticulo"] == "")
            {
                id_cod_articulo = -1;
            }
            else
            {
                id_cod_articulo = Convert.ToInt32(Request.Params["CodigoArticulo"]);
            }

            //int id_tipo_impuesto = Convert.ToInt32(Request.Params["TIPO_IMPUESTO"]);
            int id_tipo_impuesto;
            if (Request.Params["TIPO_IMPUESTO"] == "")
            {
                id_tipo_impuesto = -1;
            }
            else
            {
                if(Convert.ToString(Request.Params["IGV"]) == "10%" || Convert.ToString(Request.Params["IGV"]) == "18%")
                {
                    id_tipo_impuesto = 1;
                }
                else
                {
                    id_tipo_impuesto = 4;
                }
            }

            string IGV = Convert.ToString(Request.Params["IGV"]);

            string tipoMoneda = Convert.ToString(dbc.ObtenerTipoMonedaDocumentExpense(ID).FirstOrDefault());
            //string moneda = "";
            if (tipoMoneda == "MN")
            {
                //COIN = (b.COIN == "MN" ? "S/." : "US$"),
                tipoMoneda = "MN";
            }
            else
            {
                tipoMoneda = "ME";
            }

            decimal monto = Convert.ToDecimal(Request.Params["txtMonto"]);

            DateTime fecha = Convert.ToDateTime(de.DOC_DATE);
            //int id_movil = Convert.ToInt32(dbc.ActualizarDatosDocumentExpense11(ID, serie, numero, ruc, comentario, id_tipo, id_cod_articulo, id_tipo_impuesto, IGV, tipoMoneda, monto, razon, destino).FirstOrDefault());
            try
            {
                dbc.ActualizarDatosDocumentExpense(ID, serie, numero, ruc, comentario, id_tipo, id_cod_articulo, id_tipo_impuesto, IGV, tipoMoneda, monto, fecha, razon, destino);

                int cantdetallemovilidad = dbc.DETAIL_MOBILITY.Where(x => x.ID_DETA_MOVI == ID).Count();

                if (cantdetallemovilidad > 0)
                {
                    int ID_EXPE = Convert.ToInt32((from dm in dbc.DETAIL_MOBILITY.Where(x => x.ID_DETA_MOVI == ID)
                                                   select new
                                                   {
                                                       dm.ID_DOCU_EXPE

                                                   }).FirstOrDefault().ID_DOCU_EXPE);

                    ID = ID_EXPE;
                }

                var query = (from dex in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == ID)
                             select new
                             {
                                 dex.ID_REQU_EXPE

                             }).FirstOrDefault();


                DOCUMENT_EXPENSE qREQ = dbc.DOCUMENT_EXPENSE.Single(x => x.ID_DOCU_EXPE == ID);

                qREQ.PlacaVehiculo = Convert.ToString(Request.Params["PlacaVehiculo"]);
                qREQ.PlanillaMovilidad = Convert.ToString(Request.Params["PlanillaMovilidad"]);

                dbc.SaveChanges();

                if (files != null)
                {
                    int id_detail_movi = 0;
                    dbc.EliminarAttachedDocumentExpense(ID);

                    //int id_docu_expe = Convert.ToInt32(dbc.DETAIL_MOBILITY.Where(x => x.ID_DETA_MOVI == ID).Single().ID_DOCU_EXPE);

                    if (cantdetallemovilidad > 0)
                    {
                        id_detail_movi = Convert.ToInt32(Request.Params["TXT_ID_DOCUMENT"]);
                        de = dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == ID).FirstOrDefault();
                        id_tipo_deli_sust = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == query.ID_REQU_EXPE).Single().ID_TYPE_DELI_SUST);

                    }
                    else
                    {
                        de = dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == ID).FirstOrDefault();
                        int id_request = Convert.ToInt32(dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == ID).Single().ID_REQU_EXPE);
                        id_tipo_deli_sust = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id_request).Single().ID_TYPE_DELI_SUST);
                    }

                    string ruta = "";

                    if (id_tipo_deli_sust == 1)
                    {
                        ruta = "CajaChica";
                    }
                    else if (id_tipo_deli_sust == 2)
                    {
                        ruta = "Viaticos";
                    }
                    else
                    {
                        ruta = "Reembolso";
                    }
                    //string ruta = "Viaticos";

                    foreach (var file in files)
                    {
                        try
                        {
                            var ATTA = new ATTACHED();

                            ATTA.NAM_ATTA = de.DOC_SERI + "-" + de.DOC_NUMB;

                            if (de.ID_TYPE_DOCU_EXPE == 4)
                            {
                                ATTA.NAM_ATTA = "MOV-" + dbc.ListarDetalleViatico(de.ID_REQU_EXPE).Single().Solicitante.Replace(" ", "-");
                            }
                            else if (de.ID_TYPE_DOCU_EXPE == 8)
                            {
                                ATTA.NAM_ATTA = "REINTEGRO-" + de.CodigoOperacion;
                            }

                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_DOCU_EXPE = de.ID_DOCU_EXPE;
                            ATTA.CREATE_ATTA = DateTime.Now;
                            ATTA.ID_DETA_MOVI = id_detail_movi;
                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();
                            file.SaveAs(Server.MapPath("~/Adjuntos/" + ruta + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);

                            if (de.DocumentoObservado == true)
                            {
                                
                                de.DocumentoObservado = false;
                                dbc.SaveChanges();

                                SendMail smail = new SendMail();
                                smail.DocumentoActualizado(de);
                            }

                            

                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneMsn) top.uploadDoneMsn('Update PettyCash');}window.onload=init;</script>");
        }

        public ActionResult ObtenerDatosDocument(int id)
        {
            //  ViewBag.idCuenta = 4;
            var result = dbc.ObtenerDatosDocumentExpense(id).ToList();
            
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        //---Eliminar
        public ActionResult EliminarDetalleDocument(int id)//vista
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ObservarDetalleDocument(int id = 0)
        {
            ViewBag.ID_DOCU_EXPE = id;
            ViewBag.ID_TIPO_DOCUMENTO = Convert.ToInt32(dbc.ObtenerIdTipoDocumentExpense(id).FirstOrDefault());
            ViewBag.ID_CODIGO_ARTICULO = Convert.ToInt32(dbc.ObtenerIdCodigoArtDocumentExpense(id).FirstOrDefault());//
            ViewBag.ID_TIPO_IMPUESTO = Convert.ToInt32(dbc.ObtenerIdTipoImpuestoDocumentExpense(id).FirstOrDefault());
            ViewBag.IGV = Convert.ToString(dbc.ObtenerIGVDocumentExpense(id).FirstOrDefault());
            ViewBag.FECHA = dbc.ObtenerFechaDocumentExpense(id).FirstOrDefault();
            ViewBag.MONTO = Convert.ToString(dbc.ObtenerMontoDocumentExpense(id).FirstOrDefault());
            ViewBag.SolicitanteDocExpense = Convert.ToString(dbc.ObtenerRegistadorDocumentExpense(id).FirstOrDefault());


            ViewBag.DOC_DATE_MAX_EDIT = String.Format("{0:g}", DateTime.Today);
            return View();
        }

        public ActionResult DetalleObservado(int id = 0)
        {
            ViewBag.ID_DOCU_EXPE = id;

            int countmovi = dbc.DETAIL_MOBILITY.Where(x => x.ID_DETA_MOVI == id).Count();

            if (countmovi > 0)
            {
                id = Convert.ToInt32(dbc.DETAIL_MOBILITY.Where(x => x.ID_DETA_MOVI == id).Single().ID_DOCU_EXPE);
            }
            ViewBag.Observacion = Convert.ToString(dbc.DOCUMENT_EXPENSE.Find(id).Observacion);


            ViewBag.DOC_DATE_MAX_EDIT = String.Format("{0:g}", DateTime.Today);
            return View();
        }

        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ObservarDocument()
        {
            int id_docu_expe = 0;
            DOCUMENT_EXPENSE de;

            string Observacion = Convert.ToString(Request.Params["txtObservacion"]);
            id_docu_expe = Convert.ToInt32(Request.Params["ID_DOCU_EXPE"]);

            int countdetamovil = dbc.DETAIL_MOBILITY.Where(x => x.ID_DETA_MOVI == id_docu_expe).Count();

            if (countdetamovil > 0)
            {
                id_docu_expe = Convert.ToInt32(dbc.DETAIL_MOBILITY.Where(x => x.ID_DETA_MOVI == id_docu_expe).Single().ID_DOCU_EXPE);
                de = dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == id_docu_expe).FirstOrDefault();
            }
            else
            {
                de = dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == id_docu_expe).FirstOrDefault();
            }

            de.DocumentoObservado = true;
            de.Observacion = Observacion;

            dbc.SaveChanges();

            try
            {
                REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == de.ID_REQU_EXPE).FirstOrDefault();
                if (re.ID_TYPE_DELI_SUST != 3)
                {
                    SendMail smail = new SendMail();
                    smail.DocumentoObservado(de);
                }
                else
                {
                    re.SEND_MAIL = false;
                    dbc.SaveChanges();
                }
                

            }
            catch (Exception)
            {
            }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneMsn) top.uploadDoneMsn('Update PettyCash');}window.onload=init;</script>");
        }


        //llena combobox de tipo doc
        public ActionResult ObtenerTipoDocument()
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

            var result = dbc.ListarTipoDocumentExpense(termino).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCodCuentaDocument()
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

            //var result = dbc.ObtenerTipoDocumentExpense().ToList();
            var result = dbc.ListarCodigoDocumentExpense(termino).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerNomImpuestoDocument()
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

            var result = dbc.ListarImpuestoDocumentExpense(termino).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult EliminarDocumentExpense2(int id = 0)
        {
            try
            {
                dbc.EliminarDocumentExpense(id);

                //return Content("<script type='text/javascript'> function init() {" +
                //"if(top.MensajeEliminar) top.MensajeEliminar('OK');}window.onload=init;</script>");
                //var query = (from dex in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == id)
                //             select new
                //             {
                //                 dex.ID_REQU_EXPE

                //             });

                DOCUMENT_EXPENSE qREQ = dbc.DOCUMENT_EXPENSE.Single(x => x.ID_DOCU_EXPE == id);

                //GenerarReciboEgreso(qREQ);

                return Content("<script type='text/javascript'> function init() {" +
               "if(top.uploadDoneMsn) top.uploadDoneMsn('OK Delete');}window.onload=init;</script>");
            }
            catch(Exception e)
            {
                //return Content("<script type='text/javascript'> function init() {" +
                //"if(top.MensajeEliminar) top.MensajeEliminar('ERROR');}window.onload=init;</script>");
                return Content("<script type='text/javascript'> function init() {" +
             "if(top.uploadDoneMsn) top.uploadDoneMsn('OK Delete');}window.onload=init;</script>");
            }

        }

        //public ActionResult ObtenerSolicitante(string selectedValue)
        //{
        //    ViewBag.SolicitanteDocExpense = "Elvis";
        //    // Hacer algo con el valor seleccionado, como consultarlo en la base de datos
        //    //var result = dbc.ListarCodigoDocumentExpense(selectedValue);

        //    //// Devolver una respuesta que será manejada por la solicitud AJAX
        //    //return Json(result, JsonRequestBehavior.AllowGet);
        //    return EditarDetalleDocument();
        //}


        public ActionResult ListDocumentExpenseByID_DELI_SUST()
        {
            int ID_DELI_SUST = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());

            var qUser = (from a in dbe.CLASS_ENTITY.Where(x => x.COM_NAME == null)
                         join b in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9) on a.ID_ENTI equals b.ID_ENTI2
                         select new
                         {
                             USER = a.FIR_NAME + " " + a.LAS_NAME,
                             b.ID_PERS_ENTI
                         });

            var query = (from a in dbc.REQUEST_EXPENSE.Where(x => x.ID_DELI_SUST == ID_DELI_SUST && x.ID_STAT_REQU_EXPE != 2).ToList()
                         join b in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_STAT_DOCU_EXPE == 3 &&
                                                                    x.ID_TYPE_DOCU_EXPE != 5 &&
                                                                    x.ID_TYPE_DOCU_EXPE != 6) on a.ID_REQU_EXPE equals b.ID_REQU_EXPE
                         join c in dbc.TYPE_DOCUMENT_EXPENSE on b.ID_TYPE_DOCU_EXPE equals c.ID_TYPE_DOCU_EXPE
                         join d in dbc.DETAIL_MOBILITY.Where(x => x.ID_STAT_DETA_MOBI != 2) on b.ID_DOCU_EXPE equals d.ID_DOCU_EXPE into ld
                         from xd in ld.DefaultIfEmpty()
                         join e in qUser on a.ID_PERS_ENTI_REQU equals e.ID_PERS_ENTI
                         //join d in dbc.DOCUMENT_SALE on a.ID_DOCU_SALE equals c.ID_DOCU_SALE
                         select new
                         {
                             a.COD_REQU_EXPE,
                             DOC_NUMB = b.DOC_SERI + "-" + b.DOC_NUMB,
                             SUPPLIER = (xd != null ? xd.REASON + "/" + xd.DESTINATION : b.SUPPLIER),
                             //DOC_DATE = String.Format("{0:d}" ,b.DOC_DATE),
                             b.DOC_DATE,
                             c.NAM_DOCU,
                             AMOUNT = (xd == null ? b.AMOUNT : xd.AMOUNT),
                             COIN = (b.COIN == "MN" ? "S/." : "US$"),
                             a.ID_DELI_SUST,
                             b.ID_DOCU_EXPE,
                             USER = TextCapitalize(e.USER)
                         });

            ViewBag.IDIOMA = ResourceLanguaje.Resource.Idioma;

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListTypeDocumentExpense()
        {
            var query = (from a in dbc.TYPE_DOCUMENT_EXPENSE.Where(x => x.VIG_DOCU == true)
                         select new
                         {
                             a.ID_TYPE_DOCU_EXPE,
                             a.NAM_DOCU
                         }).OrderBy(x => x.NAM_DOCU);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListCuentaCDocumentExpense()
        {
            var query = (from a in dbc.CUENTA_CONTABLE.OrderBy(x => x.ID)
                         select new
                         {
                             a.COD_S,
                             a.DESCRIPCION
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListUsuarioGlosa()
        {
            var query = dbc.UsuarioGlosa().ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListIGVDocumentExpense()
        {
            var query = (from a in dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 1078)
                         select new
                         {
                             a.VAL_ACCO_PARA
                         }).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListTipoImpuestoDocumentExpense()
        {
            var query = (from a in dbc.TIPO_IMPUESTO.OrderBy(x => x.ID)
                         select new
                         {
                             a.ID,
                             a.DESCRIPCION
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult ListTipoAcquiDocumentExpense()
        //{
        //    var query = (from a in dbc.TIPO_DESC_ACQ.OrderBy(x => x.ID)
        //                 select new
        //                 {
        //                     a.ID,
        //                     a.DESCRIPCION
        //                 });

        //    return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        //}
        [Authorize]
        public ActionResult viewNewPettyCash(string id2, int id = 0, string id1 = "")
        {
            //var query = dbc.ListarTipoDocumentExpense(id);

            ViewBag.ID_REQU_EXPE = id;

            ViewBag.Currency = id1;
            //Session["Soli"]= id2;
            ViewBag.Soli = id2.Replace('.', ' ');
            ViewBag.DOC_DATE_MAX = String.Format("{0:g}", DateTime.Today);
            return View();
        }

        [Authorize]
        public ActionResult viewNewViatical()
        {
            ViewBag.DOC_DATE_MAX = String.Format("{0:g}", DateTime.Today);
            return View();
        }

        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateDocument(IEnumerable<HttpPostedFileBase> files, DOCUMENT_EXPENSE de)
        {
            int sw = 0;
            string Error = "";
            decimal AMOUNT = 0;
            decimal MONTOCENTIMOS = 0;
            int movix = 0;

            if (de.ID_TYPE_DOCU_EXPE == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn02; }
            else if (de.DOC_DATE == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn03; }

            else if (de.ID_TYPE_DOCU_EXPE != 4 && de.ID_TYPE_DOCU_EXPE != 8)
            {
                if (de.COD_ART == null) { sw = 1; Error = "Usted debe de ingresar el Codigo del Articulo"; }
                else if ((de.COD_ART.ToString() == "65930021" || de.COD_ART.ToString() == "65930002" || de.COD_ART.ToString() == "63430016") & (de.PlacaVehiculo == null ? "" : de.PlacaVehiculo.Trim()).Length == 0) { sw = 1; Error = "Usted debe de ingresar la placa del vehículo."; }
                else if ((de.DOC_SERI == null ? "" : de.DOC_SERI.Trim()).Length == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn04; }
                else if ((de.DOC_NUMB == null ? "" : de.DOC_NUMB.Trim()).Length == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn05; }
                else if (de.PROVEEDOR == null || de.PROVEEDOR.Length != 11) { sw = 1; Error = "El RUC debe contener 11 caractéres sin P."; }
                else if ((de.SUPPLIER == null ? "" : de.SUPPLIER.Trim()).Length == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn06; }
                else if (Decimal.TryParse(Convert.ToString(Request.Form["AMOUNT"]), out AMOUNT) == false) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn08; }
                else if (AMOUNT == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn09; }
                else if (Convert.ToString(Request.Form["CheckCentimo"]) == "on" && Decimal.TryParse(Convert.ToString(Request.Form["MontoCentimos"]), out MONTOCENTIMOS) == false) { sw = 1; Error = "Ingrese el monto de los céntimos inefactos."; }
                else if (Convert.ToString(Request.Form["CheckCentimo"]) == "on" &&  MONTOCENTIMOS == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn09; }
                else if (Convert.ToInt32(Request.Form["TIPO_IMPUESTO"]) == 0) { sw = 1; Error = "Usted debe de ingresar el Tipo de Impuesto"; }
                else if (files == null) {sw = 1; Error = "Usted debe adjuntar el comprobante."; }
                else
                {
                    de.AMOUNT = AMOUNT;
                    de.SUPPLIER = de.SUPPLIER.Replace("'", "");
                    de.PROVEEDOR = de.PROVEEDOR + "P";
                    de.DOC_NUMB = de.DOC_NUMB.TrimStart('0');
                    de.COMENTARIO = de.GLOSA;

                    if (Convert.ToInt32(Request.Form["TIPO_IMPUESTO"]) == 1)
                    { //Si se aplica IGV

                        if (de.IGV == "10%" || de.IGV == "18%")
                        {
                            de.TIPO_DACQ = 1;
                            if (de.IGV == "10%")
                            {
                                de.CodigoImpuesto = "G1";
                            }
                            else
                            {
                                de.CodigoImpuesto = "G";
                            }
                        }
                        else
                        {
                            sw = 1; Error = "Usted debe seleccionar el tipo de I.G.V";
                        }
                    }
                    else if (Convert.ToInt32(Request.Form["TIPO_IMPUESTO"]) == 4)
                    {
                        if (de.IGV == "No aplica" || de.IGV == "Exonerado")
                        {
                            de.TIPO_DACQ = 4;
                            if (de.IGV == "No aplica")
                            {
                                de.CodigoImpuesto = "I";
                            }
                            else
                            {
                                de.CodigoImpuesto = "E";
                            }
                            //de.IGV = null;
                            de.IGV = "0%";

                            
                        }
                        else
                        {
                            sw = 1; Error = "Usted debe seleccionar el tipo de I.G.V";
                        }
                    }
                }
            }
            else if(de.ID_TYPE_DOCU_EXPE == 8)
            {
                
                de.COD_ART = 0; de.TIPO_IMPUESTO = 4; de.TIPO_DACQ = 0; de.PROVEEDOR = ""; de.IGV = "0%"; de.REASON = "";de.COMENTARIO = de.GLOSA;

                if ((de.Banco == null ? "" : de.Banco.Trim()).Length == 0) { sw = 1; Error = "Ingrese el nombre del Banco"; }
                else if ((de.CodigoOperacion == null ? "" : de.CodigoOperacion.Trim()).Length == 0) { sw = 1; Error = "Ingrese el código de operación."; }
                else if (Decimal.TryParse(Convert.ToString(Request.Form["AMOUNT"]), out AMOUNT) == false) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn08; }
                else if (AMOUNT == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn09; }
                else if (files == null) { sw = 1; Error = "Usted debe adjuntar el comprobante."; }
                else
                {
                    de.AMOUNT = AMOUNT;
                }
            }
            else
            {
                string DESTINATION = Convert.ToString(Request.Form["DESTINATION"].ToString());
                
                de.TIPO_IMPUESTO = 4; de.TIPO_DACQ = 0; de.PROVEEDOR = ""; de.IGV = "0%"; de.COMENTARIO = de.GLOSA;

                if ((de.REASON == null ? "" : de.REASON.Trim()).Length == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn10; }
                else if ((DESTINATION == null ? "" : DESTINATION.Trim()).Length == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn11; }

                else if ((de.PlanillaMovilidad == null ? "" : de.PlanillaMovilidad.Trim()).Length == 0) { sw = 1; Error = "Ingrese la Planilla de Movilidad"; }
                else if (Decimal.TryParse(Convert.ToString(Request.Form["AMOUNT"]), out AMOUNT) == false) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn08; }
                else if (Convert.ToString(Request.Form["usuarioGlosa"]) == "") { sw = 1; Error = "Seleccione un usuario"; }
                else if (AMOUNT == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn09; }
                else if (files == null) { sw = 1; Error = "Usted debe adjuntar el comprobante."; }
                else
                {
                    de.AMOUNT = AMOUNT;
                    de.REASON = de.REASON.Replace("'", "");
                    DESTINATION = DESTINATION.Replace("'", "");
                }
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneMsn) top.uploadDoneMsn('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                try
                {
                    de.ID_ENTI_COMP = 9; //ID_ENTI de Electrodata
                    de.ID_PERS_ENTI_REGI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    de.ID_REQU_EXPE = Convert.ToInt32(Request.Form["NEW_ID_REQU_EXPE"]);
                    de.COIN = Convert.ToString(Request.Form["NEW_COIN"]);
                    de.ID_STAT_DOCU_EXPE = 1; //Por aprobar
                    de.DAT_REGI = DateTime.Now;
                    de.DAT_REQU_APPR = de.DAT_REGI;
                    

                    if (de.ID_TYPE_DOCU_EXPE != 4) //Documento de Pago (DdP) Mobilidad.
                    {
                  
                        dbc.DOCUMENT_EXPENSE.Add(de);
                        dbc.SaveChanges();

                       
                    }
                    else
                    {
                        
                        var query = (from a in dbc.DETAIL_MOBILITY.Where(x => x.DAT_EXPE == de.DOC_DATE &&
                                         x.ID_STAT_DETA_MOBI != 2)
                                     join b in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_STAT_DOCU_EXPE != 2) on a.ID_DOCU_EXPE equals b.ID_DOCU_EXPE
                                     where b.ID_PERS_ENTI_REGI == de.ID_PERS_ENTI_REGI && b.ID_TYPE_DOCU_EXPE == 4 &&
                                        b.ID_REQU_EXPE == de.ID_REQU_EXPE
                                     select new
                                     {
                                         a.DAT_EXPE,
                                         a.AMOUNT,
                                         a.ID_DOCU_EXPE,
                                     });

                        var qMob = (from a in query
                                    group a by new { a.DAT_EXPE } into g
                                    select new
                                    {
                                        g.Key.DAT_EXPE,
                                        SUMA = g.Sum(x => x.AMOUNT)
                                    });

                            
                                DETAIL_MOBILITY dm = new DETAIL_MOBILITY();
                                var q1 = (from a in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_REQU_EXPE == de.ID_REQU_EXPE
                                                    && x.ID_TYPE_DOCU_EXPE == 4 && x.ID_STAT_DOCU_EXPE != 2)//&& x.ID_DOCU_EXPE == de.ID_DOCU_EXPE)
                                          select new
                                          {
                                              a.ID_DOCU_EXPE
                                          });

                                    de.PROVEEDOR = Convert.ToString(Request.Form["usuarioGlosa"]);
                                    dbc.DOCUMENT_EXPENSE.Add(de);
                                    dbc.SaveChanges();
                                    dm.ID_DOCU_EXPE = de.ID_DOCU_EXPE;
                               
                                string DESTINO = Convert.ToString(Request.Form["DESTINATION"].ToString());

                                dm.DAT_EXPE = de.DOC_DATE;
                                dm.REASON = de.REASON;
                                dm.DESTINATION = DESTINO;
                                dm.AMOUNT = de.AMOUNT;
                                dm.DAT_REGI = de.DAT_REGI;
                                dm.ID_STAT_DETA_MOBI = 1; //Por aprobar

                                dbc.DETAIL_MOBILITY.Add(dm);
                                dbc.SaveChanges();

                                var qDMov = dbc.DETAIL_MOBILITY.Where(x => x.ID_DOCU_EXPE == dm.ID_DOCU_EXPE
                                    && x.ID_STAT_DETA_MOBI != 2);
                                var qTTDMov = (from a in qDMov
                                               group a by new { a.ID_DOCU_EXPE } into g
                                               select new
                                               {
                                                   g.Key.ID_DOCU_EXPE,
                                                   SUMA = g.Sum(x => x.AMOUNT)
                                               });

                                DOCUMENT_EXPENSE uDE = dbc.DOCUMENT_EXPENSE.Find(dm.ID_DOCU_EXPE);
                                uDE.AMOUNT = qTTDMov.First().SUMA;
                                dbc.DOCUMENT_EXPENSE.Attach(uDE);
                                dbc.Entry(uDE).State = EntityState.Modified;
                                dbc.SaveChanges();

                                movix = dm.ID_DETA_MOVI;
                                de.ID_DOCU_EXPE = uDE.ID_DOCU_EXPE;


                    }
                    if (files != null)
                    {

                        int type_request_expense = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == de.ID_REQU_EXPE).Single().ID_TYPE_DELI_SUST);

                        string ruta = "";

                        if (type_request_expense == 1)
                        {
                           ruta = "CajaChica";
                        }
                        else if (type_request_expense == 2)
                        {
                            ruta = "Viaticos";
                        }
                        else
                        {
                            ruta = "Reembolso";
                        }
                        
                        foreach (var file in files)
                        {
                            try
                            {
                                var ATTA = new ATTACHED();

                                ATTA.NAM_ATTA = de.DOC_SERI + "-" + de.DOC_NUMB;

                                if (de.ID_TYPE_DOCU_EXPE == 4)
                                {
                                    string codusuario = de.PROVEEDOR.ToString();
                                    string NombreUsuario = dbc.UsuarioGlosa().Where(x => x.CODUSUARIO == codusuario).Single().NOMBRE.ToString();
                                    ATTA.NAM_ATTA = "MOV-" + NombreUsuario;
                                }else if (de.ID_TYPE_DOCU_EXPE == 8)
                                {
                                    ATTA.NAM_ATTA = "REINTEGRO-" + de.CodigoOperacion;
                                }
                                
                                ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                ATTA.ID_DOCU_EXPE = de.ID_DOCU_EXPE;
                                ATTA.CREATE_ATTA = DateTime.Now;
                                ATTA.ID_DETA_MOVI = movix;
                                dbc.ATTACHEDs.Add(ATTA);
                                dbc.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/" + ruta + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);

                                if (Convert.ToString(Request.Form["CheckCentimo"]) == "on")
                                {
                                    de.AMOUNT = MONTOCENTIMOS;
                                    de.TIPO_IMPUESTO = 4;
                                    de.TIPO_DACQ = 4;
                                    de.IGV = "0%";
                                    de.ID_DOCU_EXPE_PARE = de.ID_DOCU_EXPE;
                                    de.Item = "02";
                                    dbc.DOCUMENT_EXPENSE.Add(de);
                                    dbc.SaveChanges();
                                }
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }

                    
                }
                catch (Exception e)
                {
                    var exc = new EXCEPTION();
                    exc.DAT_EXCE = DateTime.Now;
                    exc.MESSAGE = (e.InnerException == null ? e.Message : e.InnerException.Message);
                    exc.DES_EXCE = "Modulo Caja Chica: error al registrar el documento de rendición:";
                    dba.EXCEPTIONs.Add(exc);
                    dba.SaveChanges();

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneMsn) top.uploadDoneMsn('ERROR','Error DB - AppLog');}window.onload=init;</script>");
                }
            }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneMsn) top.uploadDoneMsn('OK PettyCash');}window.onload=init;</script>");
        }

        public ActionResult viewDocumentWithoutApproval()
        {
            return View();
        }

        public ActionResult viewDocumentsPettyCash(int id = 0)
        {
            ViewBag.ID_REQU_EXPE = id;
            return View();
        }

        public ActionResult viewDocuViaticalWithoutApproval(int id = 0)
        {
            ViewBag.ID_DELI_SUST = id;
            return View();
        }

        public ActionResult ListDocument(int id = 0)
        {
            Boolean resp = false;
            int ID_PERS_ENTI_RESP = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string ID_PERS_ENTIs = Convert.ToString(ID_PERS_ENTI);

            try
            {
                // Muestra a los responsables de Caja Chica: Melisa, Juan, Ronald, Luis
                ID_PERS_ENTI_RESP = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 37) //ID_PARA=37=>RESPONSABLE PETTY CASH
                    .Where(x => x.ID_ACCO == ID_ACCO)
                    .Where(x => x.VAL_ACCO_PARA == ID_PERS_ENTIs).Count();

                if (ID_PERS_ENTI_RESP > 0) //Si es que se ha logueado un reposnsable de Caja chica. 
                {
                    ID_PERS_ENTI_RESP = ID_PERS_ENTI;
                    resp = true;
                }
            }
            catch
            {

            }
            ViewBag.resp = resp;

            var result = dbc.ListarDetalleViaticoDocumentos2(ID_PERS_ENTI, id).ToList();
            // var result = dbc.ListarDetalleViaticoDocumentos12(ID_PERS_ENTI, id).ToList();


            int tt = result.Count();

            var query = (from a in result.OrderByDescending(x => x.DAT_ORD).ThenByDescending(x => x.DAT_ORD_REG).ToList()
                         join b in dbc.REQUEST_EXPENSE on a.ID_REQU_EXPE equals b.ID_REQU_EXPE
                         select new
                         {
                             SUPPLIER = (a.SUPPLIER == null ? "" : a.SUPPLIER),
                             NUMBER = (a.DOC_SERI == null ? "" : a.DOC_SERI) + "-" + (a.DOC_NUMB == null ? "" : a.DOC_NUMB),
                             DATE = (a.ID_TYPE_DOCU_EXPE != 4 ? String.Format("{0:d}", a.DOC_DATE) : String.Format("{0:d}", a.DET_DATE)),
                             TYPE = a.NAM_TYPE_DOCU,
                             COIN = (a.COIN == "MN" ? "S/." : "US$"),
                             MONEDA = (a.COIN == "MN" ? ResourceLanguaje.Resource.PeruvianNuevoSol : ResourceLanguaje.Resource.DollarAmerican),
                             //AMOUNT = String.Format("{0:N2}", a.AMOUNT),
                             AMOUNT = /*String.Concat(*/String.Format("{0:N2}", a.AMOUNT)/*, a.IGV != null ? " <span style='background-color: #f4ff00;'>(IGV " + a.IGV + ")" + "</span>" : "")*/,
                             a.ID_TYPE_DOCU_EXPE,
                             a.ID_DOCU_EXPE,
                             DET_AMOUN = (a.DET_AMOUN == null ? "" : String.Format("{0:N2}", a.DET_AMOUN)),
                             MOBILITY = (a.ID_TYPE_DOCU_EXPE != 4 ? "" : a.DET_REAS + " / " + a.DET_DEST),
                             REINTEGRO = (a.BANCO +" - "+ a.COD_OPERACION),
                             DET_DATE = (a.DET_DATE == null ? "" : String.Format("{0:d}", a.DET_DATE)),
                             a.ID_DETA_MOVI,
                             a.DET_REAS,
                             a.DET_DEST,
                             a.ID_REQU_EXPE,
                             swDelete = (a.ID_STAT_REQU_EXPE == 3 && a.ID_PERS_ENTI_REQU == ID_PERS_ENTI ?
                                        CountApproval(a.ID_REQU_EXPE.Value) : a.swDelete),
                             a.TotalAcco,
                             a.ID_ATTA,
                             a.NAM_ATTA,
                             a.EXT_ATTA,
                             URL = a.NAM_ATTA + '_' + a.ID_ATTA + a.EXT_ATTA,
                             a.ID_STAT_REQU_EXPE,
                             //IGV_CAL = a.IGV_CALCULADO
                             IGV_CAL = a.IGV_CALCULADO,
                             PORCENTAJEIGV = (a.IGV != null ? a.IGV : "0%"),
                             swObservar = ((ID_PERS_ENTI == b.ID_PERS_ENTI_ASSI || resp) && a.ID_STAT_REQU_EXPE == 6 ? true : false),
                             swObservado = a.DocumentoObservado
                         }).ToList();


            return Json(new { data = query.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MontoTotal(int id = 0)
        {
            var qryIni = dbc.ListarDetalleViaticoDocumentos2(1007,id).ToList();
            var qTotal = String.Format("{0:N2}", qryIni.Sum(x => x.AMOUNT));
            string coin = "", total = "";
            if (dbc.DOCUMENT_EXPENSE.Where(x => x.ID_REQU_EXPE == id && x.ID_STAT_DOCU_EXPE != 2).ToList().Count() != 0)
            {
                coin = dbc.DOCUMENT_EXPENSE.First(x => x.ID_REQU_EXPE == id && x.ID_STAT_DOCU_EXPE != 2).COIN;
                if (coin == "MN")
                    total = "S/. " + qTotal;
                else
                    total = "US$ " + qTotal;
            }
            return Json(new { dTotal = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TotalIGV(int id = 0)
        {

            var qryIni = dbc.ListarDetalleViaticoDocumentos2(1007, id).ToList();
            var qTotal = qryIni.Sum(x => x.AMOUNT);

            decimal? qTotalIGV = qryIni.Sum(x => Convert.ToDecimal(x.IGV_CALCULADO));

            decimal? totalDecimal = qTotalIGV - qTotal;

            string coin = "";
            string total = "";

            if (dbc.DOCUMENT_EXPENSE.Where(x => x.ID_REQU_EXPE == id && x.ID_STAT_DOCU_EXPE != 2).ToList().Count() != 0)
            {
                coin = dbc.DOCUMENT_EXPENSE.First(x => x.ID_REQU_EXPE == id && x.ID_STAT_DOCU_EXPE != 2).COIN;
                if (coin == "MN")
                    total = "S/. " + Math.Round(totalDecimal.Value, 2);
                else
                    total = "US$ " + Math.Round(totalDecimal.Value, 2);
            }

            return Json(new { dTotal = total }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult MontoTotalIGV(int id = 0)
        {
            var qryIni = dbc.ListarDetalleViaticoDocumentos2(1007, id).ToList();

            var qTotal = qryIni.Sum(x => Convert.ToDecimal(x.IGV_CALCULADO));
          
            string coin = "", total = "";
            if (dbc.DOCUMENT_EXPENSE.Where(x => x.ID_REQU_EXPE == id && x.ID_STAT_DOCU_EXPE != 2).ToList().Count() != 0)
            {
                coin = dbc.DOCUMENT_EXPENSE.First(x => x.ID_REQU_EXPE == id && x.ID_STAT_DOCU_EXPE != 2).COIN;
                if (coin == "MN")
                    total = "S/. " + qTotal;
                else
                    total = "US$ " + qTotal;
            }
            return Json(new { dTotal = total }, JsonRequestBehavior.AllowGet);
        }


        public bool CountApproval(int id = 0)
        {
            bool ctd = false;
            var query = dbc.REQUEST_EXPENSE_ACTIVITY.Where(x => x.ID_REQU_EXPE == id && x.ID_STAT_REQU_EXPE == 2);
            if (query.Count() > 0) { ctd = true; }
            return ctd;
        }
        public string Delete(int id = 0)
        {
            try
            {
                DOCUMENT_EXPENSE de = dbc.DOCUMENT_EXPENSE.Find(id);
                if (de.ID_STAT_DOCU_EXPE == 1)
                {
                    de.ID_STAT_DOCU_EXPE = 2;
                    de.DAT_REMO = DateTime.Now;

                    dbc.DOCUMENT_EXPENSE.Attach(de);
                    dbc.Entry(de).State = EntityState.Modified;
                    dbc.SaveChanges();
                    return "OK";

                    //La data lo actualiza el trigger AUDOCUMENT_EXPENSE de la tabla  Document_Expense. 
                }
                else
                {
                    return "ChangeStatus";
                }
            }
            catch (Exception e)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = (e.InnerException == null ? e.Message : e.InnerException.Message);
                exc.DES_EXCE = "Modulo Caja Chica: error al cambia a estado removed";
                dba.EXCEPTIONs.Add(exc);
                dba.SaveChanges();

                return "Error";
            }
        }
        public string DeleteMobility(int id = 0)
        {
            try
            {
                DETAIL_MOBILITY dm = dbc.DETAIL_MOBILITY.Find(id);
                if (dm.ID_STAT_DETA_MOBI == 1)
                {
                    dm.ID_STAT_DETA_MOBI = 2;
                    dm.DAT_REMO = DateTime.Now;

                    dbc.DETAIL_MOBILITY.Attach(dm);
                    dbc.Entry(dm).State = EntityState.Modified;
                    dbc.SaveChanges();

                    var qMob = (from a in dbc.DETAIL_MOBILITY.Where(x => x.ID_DOCU_EXPE == dm.ID_DOCU_EXPE && x.ID_STAT_DETA_MOBI != 2)
                                group a by new { a.ID_DOCU_EXPE } into g
                                select new
                                {
                                    g.Key.ID_DOCU_EXPE,
                                    SUMA = g.Sum(x => x.AMOUNT)
                                });

                    DOCUMENT_EXPENSE de = dbc.DOCUMENT_EXPENSE.Find(dm.ID_DOCU_EXPE);
                    if (qMob.Count() > 0)
                    {
                        de.AMOUNT = qMob.First().SUMA;
                    }
                    else
                    {
                        de.ID_STAT_DOCU_EXPE = 2;
                        de.DAT_REMO = dm.DAT_REMO;
                    }
                    dbc.DOCUMENT_EXPENSE.Attach(de);
                    dbc.Entry(de).State = EntityState.Modified;
                    dbc.SaveChanges();

                    return "OK";
                }
                else
                {
                    return "ChangeStatus";
                }
            }
            catch (Exception e)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = (e.InnerException == null ? e.Message : e.InnerException.Message);
                exc.DES_EXCE = "Modulo Caja Chica: error al cambia a estado removed de la mobilidad";
                dba.EXCEPTIONs.Add(exc);
                dba.SaveChanges();

                return "Error";
            }
        }

        public ActionResult viewPettyCash()
        {
            return View();
        }

        public ActionResult viewViatical()
        {
            return View();
        }

        public string TextCapitalize(string txt = "")
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(txt.ToLower());
        }

        public ActionResult VerDocumento(string id = "",int idRequest = 0)
        {
            try
            {
                ViewBag.Documento = id;
                ViewBag.IdRequest = idRequest;
                return View();
            }
            catch
            {
                return Content("Please Close Session");
            }

        }

        public ActionResult MontoRecepcion()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            var query = dbc.MontoRecepcion(id).ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DescargarArchivo(string id = "", int idRequest = 0)
        {
            try
            {
                int ID_TYPE_DELI_SUST = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == idRequest).Single().ID_TYPE_DELI_SUST);
                FileStream fileStream = null;
                if (ID_TYPE_DELI_SUST == 1)
                {
                    fileStream = System.IO.File.OpenRead(Server.MapPath("~/Adjuntos/CajaChica/" + id));
                }
                else if(ID_TYPE_DELI_SUST == 2)
                {
                    fileStream = System.IO.File.OpenRead(Server.MapPath("~/Adjuntos/Viaticos/" + id));
                }
                else
                {
                    fileStream = System.IO.File.OpenRead(Server.MapPath("~/Adjuntos/Reembolso/" + id));
                }
                
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

                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                Response.BinaryWrite(byteArray);
                Response.End();
                return File(storeStream.GetBuffer(), "xml");
            }
            catch(Exception e)
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));
                return File(stream, "text/plain", "Error.txt");
            }
        }
        public FileResult DescargarArchivoAll(int id = 0)
        {
            //byte[] data = null;
            //string filename = null;

            var idatta = (from c in dbc.ATTACHEDs
                          join b in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_STAT_DOCU_EXPE != 2) on c.ID_DOCU_EXPE equals b.ID_DOCU_EXPE
                          join a in dbc.REQUEST_EXPENSE on b.ID_REQU_EXPE equals a.ID_REQU_EXPE
                          where a.ID_REQU_EXPE == id && b.ID_STAT_DOCU_EXPE != 2
                          select new
                          {
                              c.NAM_ATTA,
                              c.EXT_ATTA,
                              c.ID_ATTA
                          }).ToList();

            var Att = idatta.Count();
            try
            {
                //using (ZipFile zip = new ZipFile())

                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    foreach (var nombre in idatta)
                    {
                        var nombre1 = nombre.NAM_ATTA + "_" + nombre.ID_ATTA + nombre.EXT_ATTA;
                        int id_type_deli = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).Single().ID_TYPE_DELI_SUST);

                        var archivo1 = Server.MapPath("~/Adjuntos/Viaticos/" + nombre1);

                        if (id_type_deli == 1)
                        {
                            archivo1 = Server.MapPath("~/Adjuntos/CajaChica/" + nombre1);
                        }
                        else if (id_type_deli == 3)
                        {
                            archivo1 = Server.MapPath("~/Adjuntos/Reembolso/" + nombre1);
                        }

                        var archivo1_nombre = Path.GetFileName(archivo1);

                        var archivo1_arregloBytes = System.IO.File.ReadAllBytes(archivo1);


                        //var archivo1 = Server.MapPath("~/Adjuntos/Viaticos/" + nombre1);
                        //var archivo1_nombre = Path.GetFileName(archivo1);
                        //var archivo1_arregloBytes = System.IO.File.ReadAllBytes(archivo1);

                        zip.AddEntry(archivo1_nombre, archivo1_arregloBytes);
                    }

                    var nombreDelZip = "Document_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";

                    using (MemoryStream output = new MemoryStream())
                    {
                        zip.Save(output);
                        return File(output.ToArray(), "application/zip", nombreDelZip);
                    }

                }
            }
            catch
            {
                string textx = "Error.";
                var streamx = new MemoryStream(Encoding.ASCII.GetBytes(textx));

                return File(streamx, "text/plain", "Error.txt");
            }

        }

        public ActionResult ObtenerNombreProveedor(string proveedor)
        {
         
            string nombreProveedor = dbc.ObtenerNombreDelProveedor(proveedor).FirstOrDefault();

            return Json(nombreProveedor, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AgregarCodigoArticulo()
        {
            return View();
        }
        public ActionResult CrearCuentaContable(CUENTA_CONTABLE cc)
        {
            return null;
        }
    }
}
