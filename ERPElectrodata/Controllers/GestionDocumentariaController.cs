using ERPElectrodata.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

using System.Text;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Controllers
{
    public class GestionDocumentariaController : Controller
    {
        #region "variables Globales"
        Sesiones objSesiones = new Sesiones();
        //GestionDocumentosService _serviceGestionDocumentosService = new GestionDocumentosService(new GestionDocumentosRepositorie());
        //int result = (int)Tipo_Peracion.OPERATION_ERROR;
        int result = 0;
        public CoreEntities dbc = new CoreEntities();
        static int IdPersona = 0;
        static int IdMarca = 0;
        static int IdTipoDocumento = 0;
        static String Descripcion = "";
        static int SesionIdCuenta = 0;
        #endregion

        #region  "Nueva Gestión Documentaria"

        public ActionResult GestionDocumentos(){

            ViewBag.idCuenta = objSesiones.SesionIdCuenta;
            ViewBag.UploadFile = objSesiones.KeyAdjunto;
            return View();
        }

          [HttpPost, ValidateInput(false)]
        public ActionResult GuardarGestionDocumenatria(GestionDocumento objGestionDocumento, string keyAta)
        {
            string KeyAtta = string.Empty;
            var data = (dynamic)null;
            objGestionDocumento.Creado = DateTime.Now;
            objGestionDocumento.UsuarioCreacion = Convert.ToInt32(Session["UserId"].ToString());
            if (!String.IsNullOrEmpty(keyAta)) KeyAtta = keyAta;
            try
            {
                //result = _serviceGestionDocumentosService.GuardarGestionDocumenatria(objGestionDocumento, keyAta, objSesiones.SesionIdCuenta, objSesiones.SesionIdUsuario);
                //data= _serviceGestionDocumentosService.ListarArchivos(0, null, 0, null, objSesiones.SesionIdCuenta);

                using (var dbContextTransaction = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        dbc.GestionDocumentoes.Add(objGestionDocumento);
                        dbc.SaveChanges();
                        int id = Convert.ToInt32(objGestionDocumento.Id);

                        if (keyAta != null)
                        {
                            var Attachs = dbc.Adjuntoes.Where(x => x.KeyAdjunto == keyAta).Where(x => x.IdGestionDocumento == null).ToList();
                            if (Attachs.Count() > 0)
                            {
                                foreach (Adjunto attach in Attachs)
                                {
                                    var EObj = dbc.Adjuntoes.Single(x => x.Id == attach.Id);
                                    EObj.IdGestionDocumento = id;
                                    dbc.Entry(EObj).State = System.Data.Entity.EntityState.Modified;
                                    dbc.SaveChanges();
                                    //db.Entry(EObj).State = System.Data.Entity.EntityState.Detached;
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                        result = id;
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }

                data = result;
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region   "Lista de Gestión documentaria"

          public JsonResult ListarArchivos(GestionDocumento objGestionDocumento)
         {
            IdPersona = Convert.ToInt32(objGestionDocumento.IdPersona);
            IdMarca = Convert.ToInt32(objGestionDocumento.IdMarca);
            IdTipoDocumento = Convert.ToInt32(objGestionDocumento.IdTipoDocumento);
            Descripcion = objGestionDocumento.Descripcion;
            SesionIdCuenta = objSesiones.SesionIdCuenta;
            var data = dbc.ProcListarGestionDocumentos(Convert.ToInt32(objGestionDocumento.IdPersona), objGestionDocumento.IdMarca, Convert.ToInt32(objGestionDocumento.IdTipoDocumento), objGestionDocumento.Descripcion, objSesiones.SesionIdCuenta).ToList();

              return Json(data, JsonRequestBehavior.AllowGet);
          }


          public ActionResult ObtenerTipos(int idCuenta=0)
          {
              //var data = _serviceGestionDocumentosService.ObtenerTipos(idCuenta);
              var data = dbc.ProcObtenerTipos(idCuenta).ToList();

              return Json(new { Data = data, Count = data.Count() }, JsonRequestBehavior.AllowGet);
          }

        public ActionResult ExportarCategorias(GestionDocumento objGestionDocumento)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            List<ProcListarGestionDocumentos_Result> query = dbc.ProcListarGestionDocumentos(IdPersona, IdMarca, IdTipoDocumento, Descripcion, SesionIdCuenta).ToList();
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
            sb.Append("<th class='cabecera'>Usuario</td>");
            sb.Append("<th class='cabecera'>Marca</td>");
            sb.Append("<th class='cabecera'>Tipo de Documento</td>");
            sb.Append("<th class='cabecera'>Documento</td>");
            sb.Append("<th class='cabecera'>Vigencia</td>");
            sb.Append("<th class='cabecera'>Fecha de Inicio</td>");
            sb.Append("<th class='cabecera'>Fecha de Fin</td>");
            sb.Append("<th class='cabecera'>Fecha de Creación</td>");
            sb.Append("</tr>");

            foreach (ProcListarGestionDocumentos_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.Usuario + "</td>");
                sb.Append("<td class='contenido'>" + dr.Marca + "</td>");
                sb.Append("<td class='contenido'>" + dr.TipoDocumento + "</td>");
                sb.Append("<td class='contenido'>" + dr.Documento + "</td>");
                sb.Append("<td class='contenido'>" + dr.Vigencia + "</td>");
                sb.Append("<td class='contenido'>" + dr.FechaInicioVigencia + "</td>");
                sb.Append("<td class='contenido'>" + dr.FechaFinVigencia + "</td>");
                sb.Append("<td class='contenido'>" + dr.Creado + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=GestionDocumentaria" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        #endregion
    }
}
