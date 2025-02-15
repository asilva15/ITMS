using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace ERPElectrodata.Controllers
{
    public class ProgramaController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        public static int IdPersEnti = 0;
        public static int IdProveedor = 0;
        public static int Criticidad = 0;
        //
        // GET: /Programa/

        public ActionResult Index()
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
            ViewBag.IdPrograma = Id;
            return View();
        }

        public ActionResult Editar(int Id = 0)
        {
            Programa objPrograma = dbc.Programas.Where(x => x.Id == Id).SingleOrDefault();

            return View(objPrograma);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Editar(Programa objPrograma)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);

            if (ModelState.IsValid)
            {
                objPrograma.UsuarioModifica = UserId;
                objPrograma.FechaModifica = DateTime.Now;
                dbc.Programas.Attach(objPrograma);
                dbc.Entry(objPrograma).State = EntityState.Modified;
                dbc.SaveChanges();
            }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        public ActionResult EditarLicencia(int Id = 0)
        {

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Crear(Programa objPrograma)
        {
            int flag = 0;
            int IdAcco = 0;
            int IdUser = 0;
            try
            {
                IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());
                IdUser = Convert.ToInt32(Session["UserId"].ToString());
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDonePrograma) top.uploadDonePrograma('ERROR','2','0');}window.onload=init;</script>");
            }

            if (String.IsNullOrEmpty(objPrograma.Codigo)) { flag = 1; }
            //if (String.IsNullOrEmpty(objPrograma.CodigoIdentifica)) { flag = 2; }
            if (String.IsNullOrEmpty(objPrograma.Nombre)) { flag = 2; }
            //if (String.IsNullOrEmpty(objPrograma.Serie)) { flag = 4; }
            //if (String.IsNullOrEmpty(objPrograma.Solped)) { flag = 5; }
            //if (objPrograma.IdProveedor == null) { flag = 6; }
            if (objPrograma.Criticidad== null) { flag = 3; }

            var queryPrograma = dbc.Programas.Where(Pr => (Pr.Nombre == objPrograma.Nombre || Pr.Codigo == objPrograma.Codigo) && Pr.IdAcco== IdAcco).Count();
            if (queryPrograma != 0) {
                return Content("<script>" +
                    "top.uploadDonePrograma('REPETIDO1','" + "wda" + "','"+objPrograma.Id.ToString()+"');" +
                    "</script>");

            }
            if (flag != 0) {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDonePrograma) top.uploadDonePrograma('ERROR1','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {

                //int CantidadLicencias = (int)objPrograma.LicenciaDispo;

                objPrograma.IdAcco = IdAcco;
                objPrograma.FechaCrea = DateTime.Now;
                objPrograma.UsuarioCrea = IdUser;

                dbc.Programas.Add(objPrograma);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDonePrograma) top.uploadDonePrograma('OK1','0','" + objPrograma.Id.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDonePrograma) top.uploadDonePrograma('ERROR1','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult Listar()
        {
            //textInfo = cultureInfo.TextInfo;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            int IdPersEnti = Convert.ToInt32(Request.Params["IdPersEnti"].ToString());
            int IdProveedor = Convert.ToInt32(Request.Params["IdProveedor"].ToString());
            int Criticidad = Convert.ToInt32(Request.Params["Criticidad"].ToString());
            //int IdTipoLicencia = Convert.ToInt32(Request.Params["IdTipoLicencia"].ToString());
            //string Clave = Request.Params["Clave"].ToString();

            var query = dbc.swListar(IdAcco, IdProveedor, Criticidad, IdPersEnti,"").ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatos(int id = 0)
        {
            ViewBag.IdPrograma = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.swObtenerDetalle(IdAcco, id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosLicencia(int id = 0)
        {
            ViewBag.IdPrograma = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.swObtenerProgramaLicencia(IdAcco, id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerActivoPrograma(int id = 0)
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.swActivoProgramaLicencia(IdAcco, id);

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProgramaLicencia()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdAsse = Convert.ToInt32(Request.Params["IdAsse"]);

            var query = dbc.swListarPrograma(IdAcco, IdAsse).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarOP()
        {
            var query = dbc.ListarCentroCosto();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarActivoPrograma()
        {
            try
            {
                int IdActivo = Convert.ToInt32(Request.QueryString["ac"]);
                int IdPrograma = Convert.ToInt32(Request.QueryString["pr"]);
                string FechaVence = (Request.QueryString["fv"]);
                int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
                int UserId = Convert.ToInt32(Session["UserId"]);

                var query = dbc.swGuardarActivoProgLicencia(IdActivo, IdPrograma, UserId).SingleOrDefault();
                if (query != null)
                {
                    return Content("OK");
                }
                else
                {
                    return Content("ERROR");
                }
            }
            catch
            {
                return Content("ERROR");
            }
        }


        public ActionResult EliminarActivoPrograma()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            int Id = Convert.ToInt32(Request.QueryString["id"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            var query = dbc.ProcEliminarActivoProgramaLicencia(Id);
            if (Convert.ToInt32(query) == 2)
                return Content("OK");
            else
                return Content("ERROR");
        }

        public ActionResult ListarProgramas()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ProListarProgramas(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDetallePrograma(int id = 0)
        {
            ViewBag.IdPrograma = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.ProDetallePrograma(id, IdAcco);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLicencias(int id = 0)
        {
            ViewBag.IdPrograma = id;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ProListarLicencias(ID_ACCO, id).ToList();
            return Json(new { Data = query, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult eliminarPrograma()
        {
            int Id = Convert.ToInt32(Request.QueryString["id"]);
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.ProActualizarActivoPrograma(IdAcco,Id);
            if (Convert.ToInt32(query) == 1)
                return Content("OK");
            else
                return Content("ERROR");
        }

        public ActionResult ExportarListaProgramas()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                List<swListar_Result> query = dbc.swListar(ID_ACCO, IdProveedor, Criticidad, IdPersEnti, "").ToList();

                StringBuilder sb = new StringBuilder();
                int i = 0;
                sb.Append("<meta charset='utf-8'>");
                sb.Append("<style type='text/css'>");
                sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
                sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin:5px 10px;}");
                sb.Append("</style>");
                sb.Append("<table class='tg'>");
                sb.Append("<tr>");
                sb.Append("<th class='cabecera'>Programa</td>");
                sb.Append("<th class='cabecera'>Codigo</td>");
                sb.Append("<th class='cabecera'>Serie</td>");
                sb.Append("<th class='cabecera'>Proveedor</td>");
                sb.Append("<th class='cabecera'>Version</td>");
                sb.Append("<th class='cabecera'>Fecha de Compra</td>");
                sb.Append("<th class='cabecera'>Criticidad</td>");
                sb.Append("</tr>");

                foreach (swListar_Result dr in query)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.Nombre + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Codigo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Serie + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Proveedor + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Version + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaCompra + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Criticidad + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment;filename=Programas" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();

            return View();
        }

    }
}
