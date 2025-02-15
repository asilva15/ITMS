using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class RoleController : Controller
    {

        [HttpGet]
        public ActionResult MantenimientoRol()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListaUsuarioRol()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AgregarMenu(int id = 0)
        {
            ViewBag.IdRol = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarMenu(int idRol, List<int> menus)
        {
            int Aco = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    if(menus!= null)
                    {
                        foreach(int id in menus){
                            context.AgregarMenu(idRol, id,Aco);
                        }

                    }
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }
                
            }
            
        }

        [HttpGet]
        public ActionResult MenuAcciones(int idPerfilMenu)
        {
            ViewBag.IdPerfilMenu = idPerfilMenu;
            return View();
        }

        [HttpGet]
        public ActionResult AsignarRol()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListaRoles()
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListaRoles().ToList();
                return Json(new { Data = result}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListaAcciones(int idPerfilMenu)
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListaAcciones(idPerfilMenu).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListaMenusxRol(int id)
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListaMenusxRol(id).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearRol(string Nombre,bool Estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    //Comprueba si el nombre de Perfil/Rol ya existe y esta activo
                    if (context.Perfils.Where(x => x.Perfil1 == Nombre).Count() > 0) return Content("DOBLE");
                    
                    //Crea el Perfil/Rol
                    context.CrearRol(Nombre, Estado, UserId);
                    return Content("OK");
                }
                catch(Exception)
                {
                    return Content("ERROR");
                }
            }
        }

        [HttpGet]
        public ActionResult EditarRol(int id)
        {
            using (var context = new EntityEntities())
            {
                var rol = context.Perfils.Where(x => x.Id == id).First();
                ViewBag.IdRol = rol.Id;
                ViewBag.NombreRol = rol.Perfil1;
                ViewBag.EstadoRol = rol.Estado;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarRol(int id,string Nombre,bool Estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    //Comprueba si el nombre de Perfil/Rol que queremos usar ya existe
                    var toEdit = context.Perfils.Where(x => x.Perfil1 == Nombre && x.Id != id);
                    if (toEdit.Count() > 0) return Content("DOBLE");

                    context.EditarRol(id, Nombre, Estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpGet]
        public ActionResult EliminarRol(int id)
        {
            ViewBag.IdRolE = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarRol(int id,int x)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    context.EliminarRol(id, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearMenu(int IdMenu, string Nombre,string Link, bool Estado)
        {
            int Aco = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    //Comprueba si el nombre de Menú ya existe
                    if (context.Menu.Where(x => x.Nombre == Nombre).Count() > 0) return Content("DOBLE");

                    //Crea el Menú
                    context.CrearMenu(IdMenu, Nombre, Link, Estado,Aco);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }
            }
        }

        [HttpGet]
        public ActionResult ListaMenusNoRol(int idRol)
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListaMenusNoRol(idRol).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult EditarMenu(int id)
        {
            using (var context = new EntityEntities())
            {
                var menu = context.Menu.Where(x => x.Id == id).First();
                ViewBag.IdMenu = menu.Id;
                ViewBag.NombreMenu = menu.Nombre;
                ViewBag.EstadoMenu = menu.Estado;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarMenu(int id, string Nombre, bool Estado)
        {
            int Aco = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    //Comprueba si el nombre de Perfil/Rol que queremos usar ya existe
                    var toEdit = context.Menu.Where(x => x.Nombre == Nombre && x.Id != id);
                    if (toEdit.Count() > 0) return Content("DOBLE");

                    context.EditarMenu(id, Nombre, Estado,Aco);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpGet]
        public ActionResult EliminarMenu(int idPerfilMenu)
        {
            ViewBag.idPerfilMenu = idPerfilMenu;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarMenuRol(int idPerfilMenu)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    context.EliminarMenu(idPerfilMenu, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearAccion(int idPerfilMenu, int idAccion, bool Estado)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    //Comprueba si el nombre de Acción ya existe
                    if (context.PerfilMenuAccion.Where(x => x.IdPerfilMenu == idPerfilMenu && x.IdAccion == idAccion && x.Estado == true).Count() > 0) return Content("DOBLE");

                    //Crea el Menú
                    context.CrearAccion(idPerfilMenu, idAccion, Estado, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarAccion(int id)
        {
            int Aco = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    context.EliminarAccion(id,Aco);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        [HttpGet]
        public ActionResult DuplicarRol(int IdRol)
        {
            ViewBag.IdRol = IdRol;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DuplicarRolMenuAccion(int IdRol)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    context.DuplicarRol(IdRol, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
        }

        //[HttpGet]
        //public ActionResult ListaUsrRol()
        //{
        //    using (var context = new EntityEntities())
        //    {
        //        var result = context.ListaUsuarioRol().ToList();
        //        return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public ActionResult ListaComboUsuarios()
        {
            using (var context = new EntityEntities())
            {
                var result = context.comboListaUsr().ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListaComboRoles()
        {
            using (var context = new EntityEntities())
            {
                var result = context.comboListaRol().ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListaComboAcciones()
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListaAccion().ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult BuscarUsuarioRol(int userid, int rolid)
        {
            using (var context = new EntityEntities())
            {
                var result = context.BuscaUsrRol(userid,rolid).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListaMenusAccionesRol(int idRol)
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListaMenusAccionesxRol(idRol).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListaRolesActivosxUsr(int idUsr)
        {
            using (var context = new EntityEntities())
            {
                var result = context.ListaRolesActivosxUsr(idUsr).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarRoles(int idUsr, List<int> roles)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    context.EliminarRolesUsuario(idUsr, UserId);
                    if (roles != null)
                    {
                        foreach (int id in roles)
                        {
                            context.ActualizarRoles(idUsr, id, UserId);
                        }
                    }
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DuplicarRoles(int idUser1, int idUser2)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            using (var context = new EntityEntities())
            {
                try
                {
                    context.EliminarRolesUsuario(idUser2, UserId);
                    context.DuplicarRoles(idUser1, idUser2, UserId);
                    return Content("OK");
                }
                catch (Exception)
                {
                    return Content("ERROR");
                }

            }
        }

    }
}
