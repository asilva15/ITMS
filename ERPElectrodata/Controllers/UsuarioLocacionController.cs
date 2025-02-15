using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class UsuarioLocacionController : Controller
    {
        //
        // GET: /UsuarioLocacion/
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        public ActionResult AsignarLocacion()
        {

            if (Convert.ToInt32(Session["ID_ACCO"].ToString()) == 60 && Convert.ToInt32(Session["COORDINADOR_SERVICEDESK_BNV"]) == 1) {
                List<ListaUsuarioLocacion_Result> lista = new List<ListaUsuarioLocacion_Result>();
                return View(lista);

            }
                else
                {
                    return RedirectToAction("Index", "Error");
                }

        }
        public ActionResult CargarUsuarios()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var listClient = dbc.ListarUsuariosPorCliente(ID_ACCO, 1).ToList();
            return Json(new { data = listClient }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarLocacionVinculado(int idLocacion, int idUsuario)
        {
            //accion: 1 es agregar, 0 es eliminar
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int verificarSiExiste = dbe.UsuarioLocacion.Where(ug => ug.IdUsuario == idUsuario && ug.IdLocacion == idLocacion).Count();
            if (verificarSiExiste == 0)
            {
                dbe.UsuarioLocacion.Add(new UsuarioLocacion { IdLocacion = idLocacion, IdUsuario = idUsuario });
                dbe.SaveChanges();
            }
            List<ListaUsuarioLocacion_Result> listarUsuarios = dbc.ListaUsuarioLocacion(idUsuario).ToList();
            return PartialView("_ListaUsuarioLocacion", listarUsuarios);
        }

        public ActionResult EliminarLocacionVinculado(int idUsuarioLocacion, int idUsuario)
        {
            var usuarioGrupo = dbe.UsuarioLocacion.Where(ug => ug.IdUsuarioLocacion == idUsuarioLocacion).First();
            dbe.UsuarioLocacion.Remove(usuarioGrupo);
            dbe.SaveChanges();
            List<ListaUsuarioLocacion_Result> listarUsuarios = dbc.ListaUsuarioLocacion(idUsuario).ToList();
            return PartialView("_ListaUsuarioLocacion", listarUsuarios);
        }
        public ActionResult CargarLocaciones()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            List<ListarLocacionxCuenta_Result> listaLocaciones = dbc.ListarLocacionxCuenta(ID_ACCO).ToList();
            return Json(new { data = listaLocaciones }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CargarLocacionesVinculadas(int id)
        {
            List<ListaUsuarioLocacion_Result> listarUsuarios = dbc.ListaUsuarioLocacion(id).ToList();
            return PartialView("_ListaUsuarioLocacion", listarUsuarios);
        }
        public ActionResult CargarArregloLocacionesVinculadas(int id)
        {
            List<ListaUsuarioLocacion_Result> listarUsuarios = dbc.ListaUsuarioLocacion(id).ToList();
            return Json(listarUsuarios, JsonRequestBehavior.AllowGet);
        }
    }
}
