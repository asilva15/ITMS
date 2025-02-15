using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class CoordinadorController : Controller
    {
        //
        // GET: /Coordinador/
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();
        public ActionResult Crear()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var listaCoordinadores = dbc.ListarUsuariosPorCliente(ID_ACCO,2).Where(x=>x.Coordinador==1).ToList();
            return View(listaCoordinadores);
        }
        public ActionResult ActualizarPagina()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var listaCoordinadores = dbc.ListarUsuariosPorCliente(ID_ACCO, 2).Where(x => x.Coordinador == 1).ToList();
            return View("Crear",listaCoordinadores);
        }
        public ActionResult AgregarUsuario()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var listaUsuarios = new List<ListaUsuariosCoordinadores_Result>();
            return View(listaUsuarios);
        }

        public ActionResult CargarUsuarios()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var listClient = dbc.ListarUsuariosPorCliente(ID_ACCO, 1).ToList();
            return Json(new { data= listClient },JsonRequestBehavior.AllowGet);
        }

        public ActionResult CargarUsuarioVincular()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var listClient = dbc.ListarUsuariosPorCliente(ID_ACCO, 1).ToList();
            return Json(new { data = listClient }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CargarCoordinadores()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var listCoordinadores = dbc.ListarUsuariosPorCliente(ID_ACCO, 1).Where(user => user.Coordinador == 1).ToList();
            return Json(new { data = listCoordinadores }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ModificarCoordinador(int id, int tipoAccion)
        {
            //Eliminar es 0, guardar es 1
            if(tipoAccion == 1)
            {
                
                var rsptaAdd=dbc.ModificarCoordinador(tipoAccion, id);
            }
            else
            {
                var rsptaDelet = dbc.ModificarCoordinador(tipoAccion, id);
            }
            return new EmptyResult();
        }

        public ActionResult CargarListaUsuariosVinculados(int idCoordinador)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var listarUsuarios = dbe.ListaUsuariosCoordinadores(idCoordinador, 1, ID_ACCO).ToList();
            return PartialView("_ListaUsuariosVinculados", listarUsuarios);
        }

        public ActionResult GuardarEliminarUsuarioVinculado(int idCoordinador, int idUsuario,int accion)
        {
            //accion: 1 es agregar, 0 es eliminar
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var datosCoordinador = dbe.PERSON_ENTITY.Where(t => t.ID_PERS_ENTI == idCoordinador).First();
            if (accion == 1)
            {
                dbe.Usuario_Coordinador.Add(new Usuario_Coordinador { ID_PERS_ENTI = idUsuario, ID_PERS_ENTI_Coordinador = idCoordinador });
            }
            else
            {
                var usuario = dbe.Usuario_Coordinador.Where(t => t.ID_PERS_ENTI == idUsuario && t.ID_PERS_ENTI_Coordinador == idCoordinador).First();
                dbe.Usuario_Coordinador.Remove(usuario);
            }
            dbe.SaveChanges();
            var listarUsuarios = dbe.ListaUsuariosCoordinadores(idCoordinador, 1, ID_ACCO).ToList();
            return PartialView("_ListaUsuariosVinculados", listarUsuarios);
        }
    }
}
