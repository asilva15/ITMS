using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class GrupoUsuariosController : Controller
    {
        //
        // GET: /GrupoUsuarios/
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public ActionResult Index()
        {

            if (Convert.ToInt32(Session["ID_ACCO"].ToString()) == 60 && Convert.ToInt32(Session["COORDINADOR_SERVICEDESK_BNV"]) == 1)
            {
                List<ListaUsuariosGrupos_Result> listarUsuarios = new List<ListaUsuariosGrupos_Result>();
                return View(listarUsuarios);
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
            return Json(new { data = listClient, Count = listClient.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CargarGrupos()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]); 
            //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            List<ListarGruposPorCuenta_Result> listaGrupos = dbc.ListarGruposPorCuenta(ID_ACCO).ToList();
            List<ListarGruposPorCuenta_Result> listaFiltradaGrupos = new List<ListarGruposPorCuenta_Result>();
            //foreach (ListarGruposPorCuenta_Result item in listaGrupos)
            //{
                
            //    if (item.IdGrupo != ID_QUEU)
            //    {
            //        listaFiltradaGrupos.Add(item);
            //    }
            //}
            return Json(new { data = listaGrupos, Count = listaGrupos.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GuardarEliminarUsuarioVinculado(int idGrupo, int idUsuario, int accion)
        {
            //accion: 1 es agregar, 0 es eliminar
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);


                if (accion == 1)
                {
                    int verificarSiExiste = dbe.UsuarioGrupo.Where(ug => ug.IdUsuario == idUsuario && ug.IdGrupo == idGrupo).Count();
                    if (verificarSiExiste == 0)
                    {
                        dbe.UsuarioGrupo.Add(new UsuarioGrupo { IdGrupo = idGrupo, IdUsuario = idUsuario });
                        dbe.SaveChanges();
                    }
                }
                else
                {
                    var usuarioGrupo = dbe.UsuarioGrupo.Where(ug => ug.IdUsuario == idUsuario && ug.IdGrupo == idGrupo).First();
                    dbe.UsuarioGrupo.Remove(usuarioGrupo);
                    dbe.SaveChanges();
                }
            List<ListaUsuariosGrupos_Result> listarUsuarios = dbc.ListaUsuariosGrupos(idUsuario).ToList();

            return PartialView("_ListaUsuariosGrupos", listarUsuarios);

        }

        public ActionResult CargarListaGruposVinculados(int idUsuario)
        {
            ViewBag.idUser = idUsuario;
            List<ListaUsuariosGrupos_Result> listarUsuarios = dbc.ListaUsuariosGrupos(idUsuario).ToList();
            return PartialView("_ListaUsuariosGrupos", listarUsuarios);
        }
        public ActionResult ActualizarGrupoDefault(int idGrupo, int idUsuario)
        {
            PERSON_ENTITY usuario = dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == idUsuario).FirstOrDefault();
            string nombreGrupo = dbc.QUEUEs.Where(q => q.ID_QUEU == idGrupo).FirstOrDefault().NAM_QUEU_REPO;

            usuario.ID_QUEU = idGrupo;
            //idGrupo=Convert.ToInt32( dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == idUsuario).Single().ID_QUEU);
            //var nombreGrupo = dbc.QUEUEs.Where(q => q.ID_QUEU == idGrupo).Single().NAM_QUEU_REPO;
            //var nombreGrupo = dbc.QUEUEs.FirstOrDefault(q => q.ID_QUEU == idGrupo).NAM_QUEU_REPO;
            dbe.SaveChanges();
            return Json(nombreGrupo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CargarArregloGruposVinculados(int idUsuario)
        {
            ViewBag.idUser = idUsuario;
            List<ListaUsuariosGrupos_Result> listarUsuarios = dbc.ListaUsuariosGrupos(idUsuario).ToList();
            return Json(listarUsuarios, JsonRequestBehavior.AllowGet);
        }


    }
}
