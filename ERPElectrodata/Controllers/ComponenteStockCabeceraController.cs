using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using ERPElectrodata.AppCode;
using System.Text;


using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Controllers
{
    public class ComponenteStockCabeceraController : Controller
    {
        //
        // GET: /ComponenteStockCabecera/

        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        static int idTipoActivo = 0;
        static int idComponente = 0;
        static int idSubTipoComponente = 0;
        static int cantidadActual = 0;
        static int cantidadDisponible = 0;
        static int idStockCabecera = 0;
        static int idEstado = 0;

        [HttpPost, ValidateInput(false)]
        public ActionResult Mantenimiento(ActivoMantenimiento activoMantenimiento)
        {
            //Valida si hay componentes para registrar su mantenimiento
            string idComponente = Convert.ToString(Request.Form["idComponente"]);
            int flag = 0;
            if (activoMantenimiento.FechaMantenimiento == null) { flag = 1; }
            if (activoMantenimiento.ProximoMantenimiento == null) { flag = 2; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
            }


            //if (idComponente.Length > 0)
            //{
            //    idComponente = idComponente.Substring(0, idComponente.Length - 1);
            //    string[] idsComponente = idComponente.Split(',');
            //    string msn = "";

            //    foreach (string ida in idsComponente)
            //    {
            //        var idsearch = Convert.ToInt32(ida);
            //        var query = dbc.ASSETs
            //            .Where(a => a.ID_ASSE == idsearch)
            //            .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
            //            {
            //                x.ID_ASSE,
            //                ca.ID_COND,
            //                ca.DAT_END
            //            }).Where(x => x.DAT_END == null && x.ID_COND != 9 && x.ID_COND != 3);

            //        int cant = query.Count();
            //        if (cant > 0)
            //        {
            //            msn = msn + idsearch.ToString() + ",";
            //        }

            //    }
            //}
            //else
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //        "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
            //}
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
        }

        public ActionResult Index(int Id = 0)
        {
            ViewBag.Id = Id;
            if (Id != 0)
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var objSubTipoCompoente = dbc.SubTipoComponentes.Where(SU => SU.IdSubtComp == Id).FirstOrDefault();
                ViewBag.TipoActivo = Convert.ToInt32(objSubTipoCompoente.IdTipoActivo);
                ViewBag.Componente = Convert.ToInt32(objSubTipoCompoente.IdComponente);
            }
            return View();
        }
        public ActionResult VistaAgrupada()
        {
            return View();
        }

        public ActionResult Crear()
        {
            if (Convert.ToInt32(Session["ID_ACCO"]) == 4)
            {
                if (Convert.ToInt32(Session["SOPORTE"]) == 1)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Detalle(int Id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.Id = Id;
            var queryComponenteStock = dbc.ComponenteStockCabecera.Where(Comp => Comp.IdCompStockCab == Id).FirstOrDefault();
            //ViewBag.Stock = Convert.ToInt32(queryComponenteStock.CantidadDisponible);
            //ViewBag.EnUso = Convert.ToInt32(queryComponenteStock.CantidadEnUso);
            ViewBag.Nombre = queryComponenteStock.Nombre;
            //ViewBag.Solped = queryComponenteStock.Solped;
            //ViewBag.FechaAdquisicion = queryComponenteStock.FechaAdquisicion;
            return View();
        }

        public ActionResult Editar(int Id = 0)
        {
            ComponenteStockCabecera objComponenteStockCabecera = dbc.ComponenteStockCabecera.Where(Comp => Comp.IdCompStockCab == Id).FirstOrDefault();
            ViewBag.CantidadActual = Convert.ToInt32(objComponenteStockCabecera.CantidadTotal);
            cantidadActual = Convert.ToInt32(objComponenteStockCabecera.CantidadTotal);
            cantidadDisponible = Convert.ToInt32(objComponenteStockCabecera.CantidadDisponible);
            return View(objComponenteStockCabecera);
        }

        public ActionResult Condicion(int id = 0)
        {
            ViewBag.IdCompStockDet = id;
            var objComponenteStockDetalle = dbc.ComponenteStockDetalles.Where(CO => CO.IdCompStockDet == id).SingleOrDefault();
            int ID_COND = Convert.ToInt32(objComponenteStockDetalle.IdCond);
            var objCondicion = dbc.CONDITIONs.Where(CON => CON.ID_COND == ID_COND).SingleOrDefault();
            ViewBag.IdCondicion = Convert.ToInt32(objComponenteStockDetalle.IdCond);
            idEstado = Convert.ToInt32(objCondicion.ID_STAT_ASSE);
            ViewBag.IdEstado = Convert.ToInt32(objCondicion.ID_STAT_ASSE);
            //var objCondicion = dbc.CONDITIONs.Where(SA => SA.ID_COND == IdCondicion).SingleOrDefault();
            //ViewBag.IdEstadoActivo = objCondicion.ID_STAT_ASSE;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Crear(ComponenteStockCabecera objComponenteStockCabecera)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int stock = 0;
            int idSubTipoComponente = 0;
            String Solped = Convert.ToString(objComponenteStockCabecera.Solped);
            String FechaAdquisicion = Convert.ToString(objComponenteStockCabecera.FechaAdquisicion);
            int IdLocacion = Convert.ToInt32(objComponenteStockCabecera.IdLocacion);
            objComponenteStockCabecera.IdCond = 9;
            //Validar campos en blanco
            int flag = 0;
            if (objComponenteStockCabecera.IdTipoActivo == null) { flag = 1; }
            if (objComponenteStockCabecera.IdComponente == null)
            {
                flag = 2;
            }
            else
            {
                stock = Convert.ToInt32(objComponenteStockCabecera.CantidadTotal);
            }
            if (objComponenteStockCabecera.IdSubTipoComponente == null)
            {
                flag = 3;
            }
            else
            {
                  idSubTipoComponente = Convert.ToInt32(objComponenteStockCabecera.IdSubTipoComponente);
            }
            if (objComponenteStockCabecera.CantidadTotal == null) { flag = 4; }
            if (String.IsNullOrEmpty(objComponenteStockCabecera.Nombre)) { flag = 5; }
            //if (String.IsNullOrEmpty(objComponenteStockCabecera.Solped)) { flag = 6; }
            //if (objComponenteStockCabecera.IdLocacion == null) { flag = 7; }

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

                //Generar los componentes de acuerdo al campo ingresado 'stock'
                //objComponenteStockCabecera.IdTipoActivo = id
                objComponenteStockCabecera.IdAcco = ID_ACCO;
                objComponenteStockCabecera.Estado = true;
                objComponenteStockCabecera.CantidadDisponible = objComponenteStockCabecera.CantidadTotal;
                objComponenteStockCabecera.CantidadEnUso = 0;
                objComponenteStockCabecera.FechaCrea = DateTime.Now;
                objComponenteStockCabecera.UsuarioCrea = IdUser;
                dbc.ComponenteStockCabecera.Add(objComponenteStockCabecera);
                dbc.SaveChanges();
                for (int i = 1; i <= stock; i++)
                {
                    ComponenteStockDetalle objComponenteStockDetalle = new ComponenteStockDetalle();
                    objComponenteStockDetalle.IdCompStockCab = objComponenteStockCabecera.IdCompStockCab;
                    objComponenteStockDetalle.IdLocacion = objComponenteStockCabecera.IdLocacion;
                    objComponenteStockDetalle.IdCond = objComponenteStockCabecera.IdCond;
                    objComponenteStockDetalle.FechaCrea = DateTime.Now;
                    objComponenteStockDetalle.UsuarioCrea = IdUser;
                    objComponenteStockDetalle.Estado = true;
                    objComponenteStockDetalle.Activo = false;
                    objComponenteStockDetalle.IdCompStockDet = 1;
                    dbc.ComponenteStockDetalles.Add(objComponenteStockDetalle);
                    dbc.SaveChanges();
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objComponenteStockCabecera.IdCompStockCab.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Editar(ComponenteStockCabecera objComponenteStockCabecera)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int stock = 0;
            //Validar campos en blanco
            int flag = 0;
            if (Request.Params["stock"] == "")
            {
                flag = 1;
            }
            else
            {
                stock = Convert.ToInt32(Request.Params["stock"]) - cantidadActual;
            }
            if (String.IsNullOrEmpty(objComponenteStockCabecera.Nombre)) { flag = 2; }

            if (stock<0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
            }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
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
                        "if(top.uploadDone) top.uploadDone('ERROR','3');}window.onload=init;</script>");
                }

                //Generar los componentes de acuerdo al campo ingresado 'stock'
                objComponenteStockCabecera.CantidadTotal = cantidadActual + stock;
                objComponenteStockCabecera.CantidadDisponible = cantidadDisponible + stock;
                objComponenteStockCabecera.FechaModifica = DateTime.Now;
                objComponenteStockCabecera.UsuarioModifica = IdUser;
                objComponenteStockCabecera.IdCond = 1;
                dbc.ComponenteStockCabecera.Attach(objComponenteStockCabecera);
                dbc.Entry(objComponenteStockCabecera).State = EntityState.Modified;
                dbc.SaveChanges();
                int cab = Convert.ToInt32(objComponenteStockCabecera.IdCompStockCab);
                var locacion = dbc.ComponenteStockDetalles.Where(CO => CO.IdCompStockCab == cab).First();
                int IdLocacion = Convert.ToInt32(locacion.IdLocacion); 
                for (int i = 1; i <= stock; i++)
                {
                    ComponenteStockDetalle objComponenteStockDetalle = new ComponenteStockDetalle();
                    objComponenteStockDetalle.IdCompStockCab = objComponenteStockCabecera.IdCompStockCab;
                    objComponenteStockDetalle.FechaCrea = DateTime.Now;
                    objComponenteStockDetalle.UsuarioCrea = IdUser;
                    objComponenteStockDetalle.Estado = true;
                    objComponenteStockDetalle.Activo = false;
                    objComponenteStockDetalle.IdCond = 9;
                    if (IdLocacion == 0)
                    {
                        objComponenteStockDetalle.IdLocacion = null;
                    }
                    else
                    {
                        objComponenteStockDetalle.IdLocacion = IdLocacion;
                    }
                    objComponenteStockDetalle.IdCompStockDet = 1;
                    dbc.ComponenteStockDetalles.Add(objComponenteStockDetalle);
                    dbc.SaveChanges();
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + objComponenteStockCabecera.IdTipoActivo.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','4');}window.onload=init;</script>");
            }
        }

        public ActionResult ActualizarCondicionComponente()
        {
            int idComponenteStockDet = Convert.ToInt32(Request.Params["id"].ToString());
            int idCondicion = Convert.ToInt32(Request.Params["cond"].ToString());
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            ComponenteStockDetalle objComponenteStockDet = dbc.ComponenteStockDetalles.Where(x => x.IdCompStockDet == idComponenteStockDet).SingleOrDefault();

            try
            {
                objComponenteStockDet.FechaModifica = DateTime.Now;
                objComponenteStockDet.UsuarioModifica = UserId;
                objComponenteStockDet.IdCond = idCondicion;
                dbc.ComponenteStockDetalles.Attach(objComponenteStockDet);
                dbc.Entry(objComponenteStockDet).State = EntityState.Modified;
                dbc.SaveChanges();
            }
            catch
            {
                return Content("Error");
            }

            return Content("Ok");
        }

        public ActionResult ListarStockComponentes(int i = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            idTipoActivo = Convert.ToInt32(Request.Params["IdTipoActivo"].ToString());
            idComponente = Convert.ToInt32(Request.Params["IdComponente"].ToString()); 
            idSubTipoComponente = Convert.ToInt32(Request.Params["IdSubTipoComponente"].ToString());
            var query = dbc.CompListarStockComponentes(ID_ACCO, idTipoActivo, idComponente, idSubTipoComponente).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CompListarVistaAgrupada(int i = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            idTipoActivo = Convert.ToInt32(Request.Params["IdTipoActivo"].ToString());
            idComponente = Convert.ToInt32(Request.Params["IdComponente"].ToString());
            idSubTipoComponente = Convert.ToInt32(Request.Params["IdSubTipoComponente"].ToString());
            var query = dbc.CompListarVistaAgrupada(ID_ACCO,idTipoActivo,idComponente,idSubTipoComponente).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerStockComponenteDetalle(int id = 0)
        {
            ViewBag.Id = id;
            idStockCabecera = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.CompStockComponenteDetalle(IdAcco, id);

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDetalleStockCabecera(int id = 0)
        {
            ViewBag.Id = id;

            var query = dbc.CompDetalleStockCabecera(id).ToList();
            int cantidad = query.Count();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCondicionComponentes()
        {
            var query = dbc.ListarCondicionComponentes(idEstado).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarStockComponentes()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            List<CompListarStockComponentes_Result> query = dbc.CompListarStockComponentes(ID_ACCO, idTipoActivo, idComponente, idSubTipoComponente).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style type='text/css'>");
            sb.Append(".tg{border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:20px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin-top:50px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='cabecera'>Tipo de Activo</td>");
            sb.Append("<th class='cabecera'>Componente</td>");
            sb.Append("<th class='cabecera'>Subtipo de Componente</td>");
            sb.Append("<th class='cabecera'>Nombre</td>");
            sb.Append("<th class='cabecera'>Cantidad Total</td>");
            sb.Append("<th class='cabecera'>En Uso</td>");
            sb.Append("<th class='cabecera'>Disponible</td>");
            sb.Append("</tr>");

            foreach (CompListarStockComponentes_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.Componente + "</td>");
                sb.Append("<td class='contenido'>" + dr.SubTipoComponente + "</td>");
                sb.Append("<td class='contenido'>" + dr.NombreStockCabecera + "</td>");
                sb.Append("<td class='contenido'>" + dr.CantidadTotal + "</td>");
                sb.Append("<td class='contenido'>" + dr.EnUso + "</td>");
                sb.Append("<td class='contenido'>" + dr.Disponible + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=StockComponentes" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ExportarStockDetalleComponentes()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            List<CompStockComponenteDetalle_Result> query = dbc.CompStockComponenteDetalle(ID_ACCO, idStockCabecera).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style type='text/css'>");
            sb.Append(".tg{border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:20px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin-top:50px;margin:5px 10px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='cabecera'>#</td>");
            sb.Append("<th class='cabecera'>Componente</td>");
            sb.Append("<th class='cabecera'>Subtipo de Componente</td>");
            sb.Append("<th class='cabecera'>Nombre</td>");
            sb.Append("<th class='cabecera'>Locación</td>");
            sb.Append("<th class='cabecera'>Estado</td>");
            sb.Append("<th class='cabecera'>Condición</td>");
            sb.Append("</tr>");

            foreach (CompStockComponenteDetalle_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.ORDEN + "</td>");
                sb.Append("<td class='contenido'>" + dr.Componente + "</td>");
                sb.Append("<td class='contenido'>" + dr.SubTipoComponente + "</td>");
                sb.Append("<td class='contenido'>" + dr.NombreStockCabecera + "</td>");
                sb.Append("<td class='contenido'>" + dr.Locacion + "</td>");
                sb.Append("<td class='contenido'>" + dr.Estado + "</td>");
                sb.Append("<td class='contenido'>" + dr.Condicion + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=DetalleStockComponentes" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ExportarVistaAgrupada()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            List<CompListarVistaAgrupada_Result> query = dbc.CompListarVistaAgrupada(ID_ACCO, idTipoActivo, idComponente, idSubTipoComponente).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style type='text/css'>");
            sb.Append(".tg{border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:20px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin-top:50px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='cabecera'>Tipo de Activo</td>");
            sb.Append("<th class='cabecera'>Componente</td>");
            sb.Append("<th class='cabecera'>Subtipo de Componente</td>");
            sb.Append("<th class='cabecera'>Cantidad Total</td>");
            sb.Append("<th class='cabecera'>En Uso</td>");
            sb.Append("<th class='cabecera'>Disponible</td>");
            sb.Append("</tr>");

            foreach (CompListarVistaAgrupada_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.NAM_TYPE_ASSE + "</td>");
                sb.Append("<td class='contenido'>" + dr.Nombre + "</td>");
                sb.Append("<td class='contenido'>" + dr.NomSubtComp + "</td>");
                sb.Append("<td class='contenido'>" + dr.Total + "</td>");
                sb.Append("<td class='contenido'>" + dr.EnUso + "</td>");
                sb.Append("<td class='contenido'>" + dr.Disponible + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=VistaAgrupada" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

    }
}
