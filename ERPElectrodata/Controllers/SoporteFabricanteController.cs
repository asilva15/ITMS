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
    public class SoporteFabricanteController : Controller
    {
        //
        // GET: /SoporteFabricante/
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();

        [Authorize]
        public ActionResult RegistroSoporteFabricante(int id = 0)
        {
            ViewBag.IdDocuSale = id;
            int idsoporte = 0;

            if (dbc.SoporteEDs.Where(x => x.IdDocuSale == id && x.Estado == true).Count() == 0)
            {

            }
            else
            {
                idsoporte = Convert.ToInt32(dbc.SoporteEDs.Where(x => x.IdDocuSale == id && x.Estado == true).FirstOrDefault().IdSoporteED);

            }

            var objSoporte = dbc.ObtenerDatosSoporteED(idsoporte).SingleOrDefault();

            if (objSoporte != null)
            {
                ViewBag.FechaIni = DateTime.ParseExact(objSoporte.InicioSoporte, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                ViewBag.FechaFi = DateTime.ParseExact(objSoporte.FinSoporte, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            }
            else
            {
                string mensaje = "<div style='background-color: #f2dede; color: #a94442; border: 1px solid #ebccd1; padding: 15px; margin-bottom: 20px;'>Debe registrar un <strong>SOPORTE ELECTRODATA</strong> para crear Soporte Fabricante.</div>";
                return Content(mensaje);
            }
            return View();
        }
        [Authorize]
        public ActionResult IndexSoporteFabricante(int id = 0)
        {
            ViewBag.IdDocuSale = id;
            return View();
        }

        [Authorize]
        public ActionResult EditarSoporteFabricante(int id = 0)
        {
            ViewBag.IdSoporteFab = id;
            var query = dbc.SoporteFabricantes.Where(x => x.IdSoporteFAB == id).SingleOrDefault();
            var fab = dbc.MANUFACTURERs.Where(x => x.ID_MANU == query.IdManu).SingleOrDefault();

            int existenProductos = dbc.ProductosProyecto.Where(x => x.IdSoporteFAB == id).Count();

            ViewBag.IdDocuSale = query.IdDocuSale;
            ViewBag.IdFabricante = query.IdManu;
            ViewBag.Fabricante = fab.NAM_MANU;
            ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", query.InicioSoporte);
            ViewBag.FechaFin = String.Format("{0:MM/dd/yyyy}", query.FinSoporte);
            ViewBag.MantPrev = query.MantenimientoPrev;
            ViewBag.Productos = (existenProductos > 0 ? "Nuevo" : query.Productos);
            ViewBag.Observaciones = query.Observaciones;
            ViewBag.RMA = (query.RMA == null ? "" : query.RMA);
            return View();
        }
        [Authorize]
        public JsonResult ListarFabricante(string q, string page)
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

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            var result = dbc.ListarFabricante(termino).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            //return Json(resultado, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult ListarFabricanteOP(int NumDocuSale)
        {
            var result = dbc.ListarFabricanteOP(NumDocuSale).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult ListarFabricantes()
        {
            var result = dbc.ListarFabricante("").ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult GuardarSoporteFabricante()
        {
            //Variables
            int idDocuSale = 0;
            int mantPrev = 0;
            int idUser = 0;
            int contador = 0;
            int ID_MANU = 0;
            string productosconcat = "", codigoproductoconcat = "", nombreproductoconcat = "", observaciones = "", RMA = "";
            DateTime listfechainicio = Convert.ToDateTime(null);
            DateTime listfechafin = Convert.ToDateTime(null);

            try
            {
                idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                idUser = Convert.ToInt32(Session["UserId"].ToString());


                for (int i = 0; i < Request.Params.Count; i++)
                {
                    string clave = Request.Params.GetKey(i);

                    if (clave != null && clave.StartsWith("fabricante"))
                    {

                        int numero = Convert.ToInt32(clave.Replace("fabricante", ""));

                        ID_MANU = Convert.ToInt32(Request.Params[clave]);

                        codigoproductoconcat = Convert.ToString(Request.Params["codigoproducto" + numero]);
                        nombreproductoconcat = Convert.ToString(Request.Params["nombreproducto" + numero]);
                        productosconcat = Convert.ToString(Request.Params["productos" + numero]);
                        listfechainicio = Convert.ToDateTime(Request.Params["fechainicio" + numero]);
                        listfechafin = Convert.ToDateTime(Request.Params["fechafin" + numero]);
                        RMA = Convert.ToString(Request.Params["rma" + numero]);
                        observaciones = Convert.ToString(Request.Params["observaciones" + numero]).Replace("/\n|\r|\n\r/g", "");

                        string[] codigos = codigoproductoconcat.Split('|');
                        string[] nombreproductos = nombreproductoconcat.Split('|');

                        //string numeroStr =Convert.ToInt32(clave.Replace("fabricante",""));
                        var objProyecto = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == idDocuSale);
                        objProyecto.EstadoSoporteFabricante = true;
                        dbc.Entry(objProyecto).State = EntityState.Modified;
                        dbc.SaveChanges();

                        SoporteFabricante objSoporteFAB = new SoporteFabricante();
                        objSoporteFAB.IdDocuSale = idDocuSale;
                        objSoporteFAB.Productos = productosconcat;
                        objSoporteFAB.Observaciones = "";
                        objSoporteFAB.MantenimientoPrev = mantPrev;
                        objSoporteFAB.InicioSoporte = listfechainicio;
                        objSoporteFAB.FinSoporte = listfechafin;
                        objSoporteFAB.MantPeriodico = true;
                        objSoporteFAB.FechaCrea = DateTime.Now;
                        objSoporteFAB.UsuarioCrea = idUser;
                        objSoporteFAB.Estado = true;
                        objSoporteFAB.IdManu = ID_MANU;
                        objSoporteFAB.RMA = RMA;
                        objSoporteFAB.Observaciones = observaciones;
                        dbc.SoporteFabricantes.Add(objSoporteFAB);
                        dbc.SaveChanges();

                        //Agregando los codigos de los productos
                        for (int x = 0; x < codigos.Length; x++)
                        {
                            ProductosProyecto productoproyecto = new ProductosProyecto();
                            productoproyecto.IdDocuSale = idDocuSale;
                            productoproyecto.IdSoporteFAB = objSoporteFAB.IdSoporteFAB;
                            productoproyecto.Codigo = codigos[x];
                            productoproyecto.NombreProducto = nombreproductos[x];
                            productoproyecto.Estado = true;
                            dbc.ProductosProyecto.Add(productoproyecto);
                            dbc.SaveChanges();
                        }


                        dbc.AgregarFechaMantenimientoFab(idDocuSale, objSoporteFAB.IdSoporteFAB, objSoporteFAB.InicioSoporte, objSoporteFAB.FinSoporte, mantPrev, idUser);

                        contador++;

                    }
                }
                if (contador == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.MensajeConfirmacion) top.MensajeConfirmacion('AGREGAR SOPORTES');}window.onload=init;</script>");
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


        [HttpPost, ValidateInput(false)]
        public ActionResult EditarSoporteFabricante(FormCollection collection)
        {

            //Variables
            int idDocuSale = 0, IdSoporteFab = 0;
            int idFabricante = 0;
            int mantPrev = 0;
            int idUser = 0;
            int flag = 0;
            string productos = "";
            string observaciones = "", txtRMAs = "";
            DateTime inicioSoporte = Convert.ToDateTime(null);
            DateTime finSoporte = Convert.ToDateTime(null);
            string cadenaFecha = "", fechaSoporte = "";
            try
            {
                idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                IdSoporteFab = Convert.ToInt32(Request.Params["IdSoporteFab"]);
                productos = (Request.Params["txtProductosFab"] == null ? "" : Convert.ToString(Request.Params["txtProductosFab"]).Replace("/\n|\r|\n\r/g", ""));
                observaciones = Convert.ToString(Request.Params["txtObservacionesFab"]).Replace("/\n|\r|\n\r/g", "");
                txtRMAs = Convert.ToString(Request.Params["txtRMAs"]);

                // Validaciones
                idUser = Convert.ToInt32(Session["UserId"].ToString());
                //Traer el id de fabricante
                if (Request.Params["cbFabricantes"] == null)
                {
                    flag = 1;
                }
                else
                {
                    idFabricante = Convert.ToInt32(Request.Params["cbFabricantes"]);
                }
                if (Request.Params["txtMantPrevFab"] == "")
                {
                    flag = 1;
                }
                else
                {
                    mantPrev = Convert.ToInt32(Request.Params["txtMantPrevFab"]);
                }
                if (Request.Params["dtInicioSoporteFab"] == "")
                {
                    flag = 1;
                }
                else
                {
                    inicioSoporte = Convert.ToDateTime(Request.Params["dtInicioSoporteFab"]);
                }
                if (Request.Params["dtFinSoporteFab"] == "")
                {
                    flag = 1;
                }
                else
                {
                    finSoporte = Convert.ToDateTime(Request.Params["dtFinSoporteFab"]);
                }

                string htmlListaProductos = "";

                if (collection["SelectedProductosText"] == null)
                {
                    htmlListaProductos = productos;
                }
                else
                {
                    string[] selectedProductostext = collection["SelectedProductosText"].Split('|');
                    string[] selectedProductos = collection["SelectedProductos"].Split(',');
                    var productosDiferentes = selectedProductos.ToList();
                    var productosnombresDiferentes = selectedProductostext.ToList();

                    if (productosDiferentes.Count() == 0 && string.IsNullOrEmpty(productosDiferentes[0]) || (productosDiferentes.Count() == 1 && productosDiferentes[0] == ""))
                    {
                        flag = 1;
                    }


                    if (flag == 1)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                                "if(top.MensajeConfirmacion) top.MensajeConfirmacion('1');}window.onload=init;</script>");
                    }
                    // Recorrer la lista de nombres de productos y agregar cada uno como un elemento de lista HTML
                    foreach (var productoNombre in productosnombresDiferentes)
                    {
                        htmlListaProductos += "● " + productoNombre + "</br></br>";
                    }
                }

                SoporteFabricante objSoporteFAB = dbc.SoporteFabricantes.Where(x => x.IdSoporteFAB == IdSoporteFab).FirstOrDefault();
                objSoporteFAB.IdManu = idFabricante;
                objSoporteFAB.Productos = htmlListaProductos;
                objSoporteFAB.Observaciones = observaciones;
                objSoporteFAB.InicioSoporte = inicioSoporte;
                objSoporteFAB.FinSoporte = finSoporte;
                objSoporteFAB.FechaModifica = DateTime.Now;
                objSoporteFAB.UsuarioModifica = idUser;
                objSoporteFAB.Observaciones = observaciones;
                objSoporteFAB.Estado = true;
                objSoporteFAB.RMA = txtRMAs;

                dbc.Entry(objSoporteFAB).State = EntityState.Modified;
                dbc.SaveChanges();

                //List<ObtenerSoporteFabDetalle_Result> listaSoporte = dbc.ObtenerSoporteFabDetalle(IdSoporteFab).ToList();
                ////Registrando las fechas de los mantenimientos
                //foreach (ObtenerSoporteFabDetalle_Result dr in listaSoporte)
                //{
                //    cadenaFecha = "dtSoporte" + dr.IdMant;
                //    fechaSoporte = Convert.ToString(Request.Params[cadenaFecha]);

                //    SoporteFabricanteDetalle objSoporteFabDet = dbc.SoporteFabricanteDetalles.Where(x => x.IdMantFAB == dr.IdMant).FirstOrDefault();
                //    objSoporteFabDet.FechaMantenimiento = Convert.ToDateTime(fechaSoporte);
                //    objSoporteFabDet.FechaModifica = DateTime.Now;
                //    objSoporteFabDet.UsuarioModifica = idUser;
                //    dbc.Entry(objSoporteFabDet).State = EntityState.Modified;
                //    dbc.SaveChanges();
                //}

                //registrandoproductos
                if (collection["SelectedProductosText"] == null)
                {

                }
                else
                {
                    string[] selectedProductostext = collection["SelectedProductosText"].Split('|');
                    string[] selectedProductos = collection["SelectedProductos"].Split(',');
                    var productosDiferentes = selectedProductos.ToList();
                    var productosnombresDiferentes = selectedProductostext.ToList();
                    var existeProductos = dbc.ProductosProyecto.Where(pa => pa.IdSoporteFAB == objSoporteFAB.IdSoporteFAB).ToList();
                    dbc.ProductosProyecto.RemoveRange(existeProductos);

                    int count = 0;

                    foreach (var codigoproducto in productosDiferentes)
                    {
                        if (codigoproducto != "")
                        {
                            var Producto = new ProductosProyecto
                            {
                                IdDocuSale = idDocuSale,
                                IdSoporteFAB = objSoporteFAB.IdSoporteFAB,
                                Codigo = Convert.ToString(codigoproducto),
                                NombreProducto = productosnombresDiferentes[count].ToString(),
                                Estado = true

                            };

                            dbc.ProductosProyecto.Add(Producto);
                            dbc.SaveChanges();
                            count = count + 1;
                        }
                        else
                        {

                        }
                    }

                    dbc.SaveChanges();
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


        public ActionResult CambiarEstadoFabricante(int id = 0)
        {
            int IdAplica = Convert.ToInt32(Request.Params["IdAplica"]);
            dbc.CambiarEstadoSoporteFabricante(id, IdAplica);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarSoporteFabricante(int id = 0)
        {
            var result = dbc.ListarSoporteFabxProyecto(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerSoporteFabDetalle(int id)
        {
            var result = dbc.ObtenerSoporteFabDetalle(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaProductosxFabricante(int ID_DOCU_SALE, int ID_MANU = 0)
        {
            var query = dbc.ListarProductosxFabricanteOP(ID_DOCU_SALE, ID_MANU).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);


        }

        //public ActionResult ListarRMA()
        //{
        //    var query = dbc.ListarRMA().ToList();

        //    return Json(query, JsonRequestBehavior.AllowGet);


        //}

        public ActionResult ObtenerProductosPorSoporteFabricante()
        {
            int idSoporteFab = Convert.ToInt32(Request.Params["Id"]);

            var result = dbc.ObtenerProductosPorSoporteFabricante(idSoporteFab).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}
