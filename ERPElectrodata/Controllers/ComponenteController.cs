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
    public class ComponenteController : Controller
    {
        //
        // GET: /Componente/

        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        public static int cantidadActual = 0;
        public static int idTipoActivo = 0;
        public static String nombreActivo = "";

        public static int ID_ACCO = 0;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CrearComp()
        {
            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CrearCom(Componente objComp)
        {
            int ID_TYPE_ASSET = Convert.ToInt32(Session["ID_TYPE_ASSET"]);
            //Validar campos en blanco
            int flag = 0;

            if (Convert.ToString(objComp.IdTipoActivo) == "") { flag = 1; }
            if (String.IsNullOrEmpty(objComp.Nombre)) { flag = 2; }
            if (Convert.ToString(objComp.IdAcco) == "") { flag = 3; }
            String.IsNullOrEmpty(objComp.Descripcion);


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

                objComp.UsuarioCrea = IdUser;
                objComp.Estado = true;
                objComp.Activo = false;
                objComp.FechaCrea = DateTime.Now;

                var id = dbc.ACCOUNT_TYPE_ASSET.Single(x => x.ID_ACCO == objComp.IdAcco && x.ID_TYPE_ASSE == objComp.IdTipoActivo);
                objComp.IdCuentaTipoActivo = id.ID;
                dbc.Componentes.Add(objComp);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objComp.IdTipoActivo.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
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
            var queryNombreLicencia = dbc.TYPE_ASSET.Where(Ta => Ta.ID_TYPE_ASSE == Id).FirstOrDefault();
            var queryStock = dbc.Componentes.Where(Co => Co.IdTipoActivo == Id && Co.IdAcco == ID_ACCO  && Co.Activo == false && Co.Estado == true).Count();
            var queryEnUso = dbc.Componentes.Where(Co => Co.IdTipoActivo == Id && Co.IdAcco == ID_ACCO && Co.Activo == true && Co.Estado == true).Count();
            ViewBag.Id = Id;
            idTipoActivo = Id;
            ViewBag.NombreLicencia = queryNombreLicencia.NAM_TYPE_ASSE;
            ViewBag.Stock = queryStock;
            ViewBag.EnUso = queryEnUso;
            return View();
        }

        public ActionResult DetalleComponente(String id = "")
        {
            nombreActivo = id;
            ViewBag.NombreActivo = nombreActivo;
            return View();
        }

        public ActionResult Editar(int Id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var cantidadDisponible = dbc.Componentes.Where(Co => Co.IdTipoActivo == Id && Co.IdAcco == ID_ACCO && Co.Activo == false && Co.Estado == true).Count();
            var cantidadEnUso = dbc.Componentes.Where(Co => Co.IdTipoActivo == Id && Co.IdAcco == ID_ACCO && Co.Activo == true && Co.Estado == true).Count();
            ViewBag.Cantidad = (cantidadDisponible);
            cantidadActual = (cantidadDisponible);
            Componente objComponente = dbc.Componentes.Where(Co => Co.IdTipoActivo == Id).First();
            return View(objComponente);
        }

        public ActionResult Actualizar(int Id = 0)
        {
            Componente objComponente = dbc.Componentes.Where(Co => Co.IdComponente == Id).SingleOrDefault();
            return View(objComponente);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Actualizar(Componente objComponente)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);

            if (ModelState.IsValid)
            {
                objComponente.Estado = true;
                objComponente.Activo = false;
                objComponente.UsuarioModifica = UserId;
                objComponente.FechaModifica = DateTime.Now;
                dbc.Componentes.Attach(objComponente);
                dbc.Entry(objComponente).State = EntityState.Modified;
                dbc.SaveChanges();
            }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Crear(Componente objComponente)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //Validar campos en blanco
            int flag = 0;
            if (objComponente.IdTipoActivo == null) { flag = 1; }
            if (String.IsNullOrEmpty(objComponente.Nombre)) { flag = 2; }
            int stock = 0;
            if (Request.Form["stock"] == "" || Request.Form["stock"].Equals("0")) { flag = 6; } else { stock = Convert.ToInt32(Request.Form["stock"]); }

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
                for (int i = 1; i <= stock; i++)
                {
                    objComponente.UsuarioCrea = IdUser;
                    objComponente.Estado = true;
                    objComponente.IdAcco = ID_ACCO;
                    objComponente.Activo = false;
                    objComponente.FechaCrea = DateTime.Now;
                    dbc.Componentes.Add(objComponente);
                    dbc.SaveChanges();
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','"+objComponente.IdTipoActivo.ToString()+"');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult Guardar(Componente objComponente)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int IdUser = Convert.ToInt32(Session["UserId"].ToString());
            int nuevaCantidad = Convert.ToInt32(Request.Params["stock"].ToString());
            //Verifica si el stock ingresado es menor que el stock actual 
            if (nuevaCantidad < cantidadActual)
            {
                if (ModelState.IsValid)
                {
                    //Calcula la cantidad de componentes que se deben eliminar
                    int cantidadEliminar = cantidadActual - nuevaCantidad;
                    for(int i = 1;i <= cantidadEliminar; i++)
                    {
                        var query = dbc.CompEliminarComponentes(ID_ACCO,idTipoActivo);
                        //int count = query.ToString().Count();
                    }
                }
            }
            else
            {
                int cantidadAInsertar = nuevaCantidad - cantidadActual;
                //Agregar la (nuevaCantidad - cantidadActual) de componentes
                for (int i = 1; i <= cantidadAInsertar; i++)
                {
                    //Si agregar, validar que se agregue en el mismo grupo
                    objComponente.UsuarioCrea = IdUser;
                    objComponente.Estado = true;
                    objComponente.IdAcco = ID_ACCO;
                    objComponente.Activo = false;
                    objComponente.FechaCrea = DateTime.Now;
                    dbc.Componentes.Add(objComponente);
                    dbc.SaveChanges();
                }
            }
            int UserId = Convert.ToInt32(Session["UserId"]);

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        //Lista y Búsqueda de componentes
        public ActionResult BuscarListarComponentes(int i = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            idTipoActivo = Convert.ToInt32(Request.Params["IdTipoActivo"].ToString());
            var query = dbc.CompListarBuscarComponentes(ID_ACCO, idTipoActivo);

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult eliminar(int id = 0)
        {
            ViewBag.Id = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.CompEliminarComponente(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoActivo()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ActCompListarTipoActivo(ID_ACCO).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarEstadoActivo()
        {
            var query = dbc.ListarEstadoActivo().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCondicionActivo()
        {

            int IdEstado = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            string Condicion = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            if (Condicion == null)
                Condicion = "";
            var query = dbc.ListarCondicionAsset(IdEstado,Condicion).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarComponentes(int id = 0)
        {
            //int IdTipoActivo = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            var objCuentaActivoComponente = dbc.ACCOUNT_TYPE_ASSET.Where(AT => AT.ID == id).SingleOrDefault();
            int idTipoActivo = Convert.ToInt32(objCuentaActivoComponente.ID_TYPE_ASSE);
            string Componente = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            if (Componente == null)
                Componente = "";

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ActCompListarComponentes(ID_ACCO, idTipoActivo,Componente).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarComponentesPorTipoActivo(int id = 0)
        {
            //string TipoActivo = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            int IdTipoActivo = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            String TipoComponente = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            if (TipoComponente == null)
                TipoComponente = "";
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ActCompListarComponentesPorTipoActivo(ID_ACCO, IdTipoActivo).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSubTipoComponentes(int id = 0)
        {
            int IdComponente = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            string Componente = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            if (Componente == null)
                Componente = "";
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ActCompListarStockComponente(ID_ACCO, IdComponente,Componente).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerListaComponentes(int id = 0)
        {
            ViewBag.Id = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.CompDetalleComponente(nombreActivo, idTipoActivo,IdAcco);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerListaCantidadesComponentes(int id = 0)
        {
            ViewBag.Id = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.CompListarCantidadesComponentes(IdAcco, idTipoActivo);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ObtenerDetalleComponente(int id = 0)
        //{
        //    ViewBag.Id = id;
        //    int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

        //    var query = dbc.ProcListarProgramaLicencias(IdAcco, "", id, 0, 0);

        //    return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ListarActivoComponentes(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ActListarActivosComponentes(ID_ACCO, id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarNuevoComponente()
        {
            int IdActivo = 0;
            int IdComponente = 0;
            int IdStock = 0;
            String Comentario = "";
            int UserId = 0;
            try
            {
                IdActivo = Convert.ToInt32(Request.QueryString["ta"]);
                IdComponente = Convert.ToInt32(Request.QueryString["cp"]);
                IdStock = Convert.ToInt32(Request.QueryString["IdStock"]);
                Comentario = Request.QueryString["comen"];
                UserId = Convert.ToInt32(Session["UserId"]);
                var query = dbc.CompActualizarActivoComponente(IdStock, UserId, IdActivo,Comentario);
                if(Convert.ToInt32(query)==3)
                    return Content("OK");
                else
                    return Content("ERROR");
            }
            catch {
                return Content("ERROR");
            }
        }

        public ActionResult EditarComponente()
        {
            int IdActivoComponente = Convert.ToInt32(Request.QueryString["id"]);
            int IdComponente = Convert.ToInt32(Request.QueryString["cp"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            ActivoComponente objActivoComponente = dbc.ActivoComponentes.Where(x => x.IdActivoComponente == IdActivoComponente).SingleOrDefault();

            objActivoComponente.IdComponente = IdComponente;
            objActivoComponente.Estado = true;
            objActivoComponente.FechaModifica = DateTime.Now;
            objActivoComponente.UsuarioModifica = UserId;

            dbc.Entry(objActivoComponente).State = EntityState.Modified;
            dbc.SaveChanges();

            return Content("OK");
        }

        public ActionResult EliminarComponente()
        {
            int IdActivoComponente = Convert.ToInt32(Request.QueryString["id"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            var query = dbc.CompEliminarActivoComponente(IdActivoComponente);
            if (Convert.ToInt32(query) == 3)
                return Content("OK");
            else
                return Content("ERROR");
        }

        public ActionResult actualizarLicenciaPorVencer()
        {
            int IdActivoComponente = Convert.ToInt32(Request.QueryString["id"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            var query = dbc.ProcActualizarLicenciaPorVencer(IdActivoComponente,UserId);
            if (Convert.ToInt32(query) == 3)
                return Content("OK");
            else
                return Content("ERROR");
        }

        public ActionResult ExportarListaComponentes()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            
            List<CompListarBuscarComponentes_Result> query = dbc.CompListarBuscarComponentes(ID_ACCO,idTipoActivo).ToList();

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
            sb.Append("<th class='cabecera'>Cantidad Total</td>");
            sb.Append("<th class='cabecera'>En Uso</td>");
            sb.Append("<th class='cabecera'>Disponible</td>");
            sb.Append("</tr>");

            foreach (CompListarBuscarComponentes_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.Cantidad + "</td>");
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
            Response.AddHeader("Content-Disposition", "attachment;filename=Componentes" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ExportarListaDetalleComponentes()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            List<CompListarCantidadesComponentes_Result> query = dbc.CompListarCantidadesComponentes(ID_ACCO, idTipoActivo).ToList();

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
            sb.Append("<th class='cabecera'>Tipo de Activo</td>");
            sb.Append("<th class='cabecera'>Componente</td>");
            sb.Append("<th class='cabecera'>Cantidad Total</td>");
            sb.Append("<th class='cabecera'>Disponible</td>");
            sb.Append("<th class='cabecera'>En Uso</td>");
            sb.Append("</tr>");

            foreach (CompListarCantidadesComponentes_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.Nombre + "</td>");
                sb.Append("<td class='contenido'>" + dr.CantidadTotal + "</td>");
                sb.Append("<td class='contenido'>" + dr.Disponible + "</td>");
                sb.Append("<td class='contenido'>" + dr.EnUso + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=DetalleComponentes" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ListarComponente()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string p = Request.QueryString["filter[filters][0][value]"];
            int IdTipoActivo = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            string IdComponente = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            if (IdComponente == null)
                IdComponente = "";
            var query = dbc.CompListarComponente(IdTipoActivo,ID_ACCO, IdComponente).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarComponentesRegistrados()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int IdTipoActivo = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            string txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            if (txt == null)
                txt = "";
            var query = dbc.CompListarComponente(IdTipoActivo, ID_ACCO, txt).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditarComp(int id = 0)
        {

            //var pe = dbc.ACCOUNT_TYPE_ASSET.Find(id,idAcco);

            var c = dbc.Componentes.Find(id);
            var a = dbc.ACCOUNTs.Single(x => x.ID_ACCO == c.IdAcco);
            var ta = dbc.TYPE_ASSET.Single(x => x.ID_TYPE_ASSE == c.IdTipoActivo);

            //var cp = dbc.ACCOUNTs.Single(x => x.ID_ACCO == pe.ID_ACCO);
            //var ta = dbc.TYPE_ASSET.Single(x => x.ID_TYPE_ASSE == pe.ID_TYPE_ASSE);
            ViewBag.NAM_ACCO = a.NAM_ACCO;
            ViewBag.NAM_TYPE_ASSE = ta.NAM_TYPE_ASSE;
            ViewBag.Nombre = c.Nombre;
            ViewBag.Descripcion = c.Descripcion;

            return View(c);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditarCom(Componente objComp)
        {
            int ID_TYPE_ASSET = Convert.ToInt32(Session["ID_TYPE_ASSET"]);
            //Validar campos en blanco
            int flag = 0;

            if (Convert.ToString(objComp.IdTipoActivo) == "") { flag = 1; }
            if (String.IsNullOrEmpty(objComp.Nombre)) { flag = 2; }
            if (Convert.ToString(objComp.IdAcco) == "") { flag = 3; }
            String.IsNullOrEmpty(objComp.Descripcion);

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

                Convert.ToInt32(objComp.UsuarioCrea);
                Convert.ToDateTime(objComp.FechaCrea);
                Convert.ToBoolean(objComp.Estado);

                objComp.Activo = false;
                objComp.UsuarioModifica = IdUser;
                objComp.FechaModifica = DateTime.Now;

                var id = dbc.ACCOUNT_TYPE_ASSET.Single(x => x.ID_ACCO == objComp.IdAcco && x.ID_TYPE_ASSE == objComp.IdTipoActivo);
                objComp.IdCuentaTipoActivo = id.ID;

                dbc.Componentes.Attach(objComp);
                dbc.Entry(objComp).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objComp.IdComponente.ToString() + "');}window.onload=init;</script>");

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        //Lista y Búsqueda de componentes
        public ActionResult BuscarListarComponentesTodo(int i = 0)
        {
            // int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ID_ACCO = Convert.ToInt32(Request.Params["ID_ACCO"].ToString());

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            idTipoActivo = Convert.ToInt32(Request.Params["ID_TYPE_ASSE"].ToString());
            string Nombre = Convert.ToString(Request.Params["NOMBRE"].ToString());
            string Descripcion = Convert.ToString(Request.Params["DESCRIPCION"].ToString());

           
            if (Descripcion == "")
            {
                 var query = dbc.CompListarComponentes(ID_ACCO, idTipoActivo, Nombre, Descripcion, 1).ToList();
                 var total = query.Count();

                 var query2 = query.Skip(skip).Take(take);

                 return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var queryD = dbc.CompListarComponentes(ID_ACCO, idTipoActivo, Nombre, Descripcion, 2).ToList();
                var total = queryD.Count();

                var query2 = queryD.Skip(skip).Take(take);

                return Json(new { Data = query2, Cantidad = queryD.Count() }, JsonRequestBehavior.AllowGet);
            }

            
        }
    }
}
