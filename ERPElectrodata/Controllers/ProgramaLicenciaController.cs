using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Globalization;

using System.Data.Entity.Core.Objects;

using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace ERPElectrodata.Controllers
{
    public class ProgramaLicenciaController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        public static int idLicenciaVencida;
        public static String estado = "";
        public static int IdPrograma = 0;
        public static int IdTipoLicencia = 0;

        public static DateTime? FechaCompra = null;
        public static String Serie = "";
        //
        // GET: /ProgramaLicencia/

        public ActionResult Crear()
        {
            //Mediante el UserId, validamos si el usuario tiene asignado el IdPerfil 37 --> 'Permiso Licencias'
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var queryUsuario = dbe.Usuarios.Where(Us => Us.UserId == UserId).SingleOrDefault();
            int idUsuario = queryUsuario.Id;
            var queryPerfil = dbe.UsuarioPerfils.Where(Up => Up.IdUsuario == idUsuario && Up.IdPerfil == 37).Count();
            if (queryPerfil == 1)
            {
                return View();
            }
            else
            {
                //Si no tiene permisos, retornamos al index
                return RedirectToAction("Index");
            }
        }

        public ActionResult LicenciasPorVencer()
        {
            return View();
        }

        //Ver al detalle cada registro
        public ActionResult Detalle(int programa, int proveedor, string version)
        {
            var IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var model = dbc.ListarLicenciasUnificadas(IdAcco, programa, proveedor, version).FirstOrDefault();
            return View(model);
        }





        public ActionResult ListarUsuarioLicencia()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdAsse = Convert.ToInt32(Request.Params["IdAsse"]);

            var query = dbc.ObtenerDatosUsuariosConLicenciaVigente().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CrearLicencia(int IdPrograma = 0, int IdProveedor = 0, string Version = "")
        {
            Version = HttpUtility.UrlDecode(Version);

            var modeloParaVista = new ProgramaLicencia
            {
                IdPrograma = IdPrograma,
                IdProveedor = IdProveedor,
                VersionLic = Version
            };

            return View(modeloParaVista);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearLicencia(ProgramaLicencia objProgramaLicencia)
        {
            int flag = 0;
            int validacion = 0;
            int Cantidad = 0;

            string validacionValue = Request.Form["Validacion"];

            if (!string.IsNullOrEmpty(validacionValue) && validacionValue.Equals("on"))
            {
                validacion = 1;
            }
            else
            {
                validacion = 0;
            }


            if (Request.Form["Cantidad"] == "" || Request.Form["Cantidad"].Equals("0")) { flag = 6; } else { Cantidad = Convert.ToInt32(Request.Form["Cantidad"]); }

            if (objProgramaLicencia.IdTipoLicencia == 0 || objProgramaLicencia.IdTipoLicencia == null) { flag = 2; };

            if (Convert.ToString(objProgramaLicencia.TipoLicencia) == "" || objProgramaLicencia.TipoLicencia == null) { flag = 3; };

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                       "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
            }

            int asignacionUsuario = 0;

            int idPersonEntity = 0;
            int UserId = Convert.ToInt32(Session["UserId"]);

            ObjectParameter result = new ObjectParameter("Result", typeof(int));

            dbc.InsertarProgramaLicencia(
                objProgramaLicencia.IdPrograma,
                objProgramaLicencia.IdModoInstalacion,
                objProgramaLicencia.Costo,
                objProgramaLicencia.TipoLicencia,
                objProgramaLicencia.IdProveedor,
                objProgramaLicencia.Solped,
                objProgramaLicencia.Observacion,
                objProgramaLicencia.CodigoAdicional,
                objProgramaLicencia.CodigoActivacion,
                objProgramaLicencia.VersionLic,
                objProgramaLicencia.IdTipoLicencia,
                UserId,
                asignacionUsuario,
                Cantidad,
                idPersonEntity,
                objProgramaLicencia.FechaCompra,
                Convert.ToInt32(objProgramaLicencia.AreaSolicitante),
                validacion,
                objProgramaLicencia.FechaActivacion,
                result,
                objProgramaLicencia.TipoMoneda
                );

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }



        //Editar un registro de la tabla ProgramaLicencia

        public ActionResult Editar(int Id = 0)
        {
            ProgramaLicencia objProgramaLicencia = dbc.ProgramaLicencia.Where(p => p.Id == Id && p.Activo == false).SingleOrDefault();
            return View(objProgramaLicencia);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Editar(ProgramaLicencia objProgramaLicencia)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);

            int flag = 0;

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                var query = dbc.EditarProgramaLicencia(objProgramaLicencia.Id, objProgramaLicencia.FechaCompra,UserId, objProgramaLicencia.CodigoActivacion, objProgramaLicencia.CodigoAdicional);
            }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }


        public ActionResult EditarLicencias(int Id = 0)
        {
            ProgramaLicencia objProgramaLicencia = dbc.ProgramaLicencia.Where(p => p.Id == Id && p.Activo == false).SingleOrDefault();
            return View(objProgramaLicencia);
        }


        public ActionResult ListarTipoLicencia()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ObtenerPeriodoLicencia().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NuevoPeriodoLicencias()
        {
            return View();
        }


        // POST: /Manufacturer/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult NuevoPeriodoLicencia(IEnumerable<HttpPostedFileBase> files, ObtenerPeriodoLicencia_Result objPeriodo)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (String.IsNullOrEmpty(objPeriodo.NombrePeriodo) || objPeriodo.DiasPeriodo == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneProveedor) top.uploadDoneProveedor('ERROR','0');}window.onload=init;</script>");
            }

            //var queryProveedor = dbc.Proveedors.Where(Pr => (Pr.Nombre == objProveedor.Nombre) && Pr.IdAcco == ID_ACCO).Count();
            //if (queryProveedor != 0)
            //{
            //    return Content("<script>" +
            //        "top.uploadDoneProveedor('ERROR','1');" +
            //        "</script>");

            //}
            //objPeriodo.Estado = true;
            //objProveedor.IdAcco = ID_ACCO;
            //objProveedor.Ruc = objProveedor.Ruc;
            //objProveedor.Direccion = objProveedor.Direccion;
            //objProveedor.UserIdCreacion = UserId;
            //objProveedor.FechaCreacion = DateTime.Now;
            //string DescripcionProv = Convert.ToString(Request.Form["DescripcionProv"]);
            //objProveedor.Descripcion = DescripcionProv;

            try
            {
                //dbc.Proveedors.Add(objProveedor);
                //dbc.SaveChanges();
                //int id = Convert.ToInt32(objProveedor.Id);
                dbc.InsertarPeriodoLicencia(objPeriodo.NombrePeriodo, objPeriodo.DiasPeriodo, objPeriodo.Descripcion);

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneProveedor) top.uploadDoneProveedor('OK','" + 1 + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneProveedor) top.uploadDoneProveedor('ERROR','2');}window.onload=init;</script>");
            }
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult Crear(ProgramaLicencia objProgramaLicencia)
        {
            int flag = 0;
            if (objProgramaLicencia.IdTipoLicencia == null) { flag = 1; }
            if (objProgramaLicencia.IdPrograma <= 0) { flag = 3; }
            if (String.IsNullOrEmpty(objProgramaLicencia.VersionLic)) { flag = 4; }
            if (objProgramaLicencia.IdProveedor <= 0) { flag = 7; }



            bool isvalidacion = Request.Form["Validacion"] == "on";
            bool isByUser = Request.Form["IS_BY_USER"] == "on";
            int validacion = isvalidacion ? 1 : 0;
            int IdUsuarioAsignado = isByUser ? 1 : 0;

            int CantidadLicencias = 0;

            if (isByUser == false)
            {
                if (Request.Form["LicenciasDisponibles"] == "" || Request.Form["LicenciasDisponibles"].Equals("0")) { flag = 6; } else { CantidadLicencias = Convert.ToInt32(Request.Form["LicenciasDisponibles"]); }
            }

            int IdPersEntiAcco = 0;
            if (isByUser == true)
            {
                if (Request.Form["id_pers_enti"] == "" || Request.Form["id_pers_enti"].Equals("0")) { flag = 6; } else { IdPersEntiAcco = Convert.ToInt32(Request.Form["id_pers_enti"]); }
            }


            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone1) top.uploadDone1('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }


            if (ModelState.IsValid)
            {
                int IdUser = 0;
               
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());



                    if (IdUsuarioAsignado == 0)
                    {

                        ObjectParameter result = new ObjectParameter("Result", typeof(int));

                        dbc.InsertarProgramaLicencia(
                objProgramaLicencia.IdPrograma,
                objProgramaLicencia.IdModoInstalacion,
                objProgramaLicencia.Costo,
                objProgramaLicencia.TipoLicencia,
                objProgramaLicencia.IdProveedor,
                objProgramaLicencia.Solped,
                objProgramaLicencia.Observacion,
                objProgramaLicencia.CodigoAdicional,
                objProgramaLicencia.CodigoActivacion,
                objProgramaLicencia.VersionLic,
                objProgramaLicencia.IdTipoLicencia,
                IdUser,
                IdUsuarioAsignado,
                CantidadLicencias,
                IdPersEntiAcco,
                objProgramaLicencia.FechaCompra,
                Convert.ToInt32(objProgramaLicencia.AreaSolicitante),
                validacion,
                objProgramaLicencia.FechaActivacion,
                result,
                objProgramaLicencia.TipoMoneda
                );

                    }
                    else
                    {
                        CantidadLicencias = 1;
                        ObjectParameter result = new ObjectParameter("Result", typeof(int));

                        dbc.InsertarProgramaLicencia(
                objProgramaLicencia.IdPrograma,
                objProgramaLicencia.IdModoInstalacion,
                objProgramaLicencia.Costo,
                objProgramaLicencia.TipoLicencia,
                objProgramaLicencia.IdProveedor,
                objProgramaLicencia.Solped,
                objProgramaLicencia.Observacion,
                objProgramaLicencia.CodigoAdicional,
                objProgramaLicencia.CodigoActivacion,
                objProgramaLicencia.VersionLic,
                objProgramaLicencia.IdTipoLicencia,
                IdUser,
                IdUsuarioAsignado,
                CantidadLicencias,
                IdPersEntiAcco,
                objProgramaLicencia.FechaCompra,
                Convert.ToInt32(objProgramaLicencia.AreaSolicitante),
                validacion,
                objProgramaLicencia.FechaActivacion,
                result,
                objProgramaLicencia.TipoMoneda
                );
                        if (result != null)
                        {
                            int val = (int)result.Value;
                            if (val == 1)
                            {
                                return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.uploadDone1) top.uploadDone1('LicxUsu','2','0');}window.onload=init;</script>");
                            }
                        }

                    }

                    string versionCodificada = HttpUtility.UrlEncode(objProgramaLicencia.VersionLic);

                    return Content("<script type='text/javascript'> function init() {" +
              "if(top.uploadDone1) top.uploadDone1('OK2','0','" + objProgramaLicencia.IdPrograma + "','" + objProgramaLicencia.IdProveedor + "','" + versionCodificada + "');}window.onload=init;</script>");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone1) top.uploadDone1('ERROR','2','0');}window.onload=init;</script>");
                }
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone1) top.uploadDone1('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult newObtenerLicencias(int Programa = 0, int Proveedor = 0)
        {
            var IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarLicenciasUnificadas(IdAcco, Programa, Proveedor, "0").ToList();
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaPrioridad = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }



        //Obtener Licencias
        public ActionResult ObtenerListaLicencias(int i = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            estado = Request.Params["Estado"].ToString();
            IdPrograma = Convert.ToInt32(Request.Params["IdPrograma"].ToString());
            IdTipoLicencia = Convert.ToInt32(Request.Params["IdTipoLicencia"].ToString());
            var strFC = Request.Params["FechaCompra"].ToString();

            if (!String.IsNullOrEmpty(strFC)) { FechaCompra = Convert.ToDateTime(strFC); }

            //Serie = Request.Params["Serie"].ToString();
            var query = dbc.ProcListarProgramaLicencias(ID_ACCO, estado, 0, IdPrograma, IdTipoLicencia).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { ListaPrioridad = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerListaLicenciasVistaMS(int i = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            IdPrograma = Convert.ToInt32(Request.Params["IdPrograma"].ToString());
            var query = dbc.ProListarLicenciasVistaMS(ID_ACCO, IdPrograma).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLicenciasDisponibles(int idProveedor = 0, int idPrograma = 0, string version = "")
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ProcListarLicenciasDisponibles(ID_ACCO, idPrograma, idProveedor, version).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Index()
        {
            //Mediante el UserId, validamos si el usuario tiene asignado el IdPerfil 37 --> 'Permiso Licencias'
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var queryUsuario = dbe.Usuarios.Where(Us => Us.UserId == UserId).SingleOrDefault();
            int idUsuario = queryUsuario.Id;
            var queryPerfil = dbe.UsuarioPerfils.Where(Up => Up.IdUsuario == idUsuario && Up.IdPerfil == 37).Count();
            //
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int[] numbers = new int[0];

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var programaLicencia = dbc.ProgramaLicencia;
            int mesActual = DateTime.Now.Month;
            int anioActual = DateTime.Now.Year;
            var query = dbc.ProcListarLicenciasPorCaducar(ID_ACCO, mesActual, anioActual).Count();
            ViewBag.CantidadLicencias = query;
            try
            {
                if (queryPerfil == 1)
                {
                    ViewBag.Perfil = 37;
                }
                else
                {
                    ViewBag.Perfil = 0;
                }
                return View();

                //ViewBag.TRed = (from pl in dbc.ProgramaLicencias
                //                join pr in dbc.Programas on pl.IdPrograma equals pr.Id
                //                join ap in dbc.ACCOUNT_PARAMETER on pl.IdTipoLicencia equals ap.ID_ACCO_PARA
                //                where pr.IdAcco == ID_ACCO
                //                && pl.IdTipoLicencia == 222
                //                select new
                //                {
                //                    pl.Id
                //                }).ToList().Count();

                //ViewBag.TStandalone = programaLicencia.Where(p => p.IdTipoLicencia == 221).Count();

                //ViewBag.TStandalone = (from pl in dbc.ProgramaLicencias
                //                join pr in dbc.Programas on pl.IdPrograma equals pr.Id
                //                join ap in dbc.ACCOUNT_PARAMETER on pl.IdTipoLicencia equals ap.ID_ACCO_PARA
                //                where pr.IdAcco == ID_ACCO
                //                && pl.IdTipoLicencia == 221
                //                select new
                //                {
                //                    pl.Id
                //                }).ToList().Count();

                //ViewBag.TDisponible = (from pl in dbc.ProgramaLicencias
                //                       join pr in dbc.Programas on pl.IdPrograma equals pr.Id
                //                       join ap in dbc.ACCOUNT_PARAMETER on pl.IdTipoLicencia equals ap.ID_ACCO_PARA
                //                       where pr.IdAcco == ID_ACCO
                //                       && pl.Activo == true
                //                       select new
                //                       {
                //                           pl.Id
                //                       }).ToList().Count();

                //ViewBag.TNoDisponible = (from pl in dbc.ProgramaLicencias
                //                       join pr in dbc.Programas on pl.IdPrograma equals pr.Id
                //                       join ap in dbc.ACCOUNT_PARAMETER on pl.IdTipoLicencia equals ap.ID_ACCO_PARA
                //                       where pr.IdAcco == ID_ACCO
                //                       && pl.Activo == false
                //                       select new
                //                       {
                //                           pl.Id
                //                       }).ToList().Count();
                //ViewBag.TimeServer = String.Format("{0:MMM d yyyy HH:mm:ss}", DateTime.Now);
                //ViewBag.Message = "";

            }
            catch (Exception e)
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("Index");
            }
        }

        public ActionResult ObtenerDatos(int id = 0)
        {
            ViewBag.Id = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ProcListarProgramaLicencias(IdAcco, "", id, 0, 0).ToList();

            if (Convert.ToInt32(Session["SUPERVISOR_PERMISO_LICENCIAS"]) != 1)
            {
                query.ForEach(x => x.CodigoActivacion = "xxxxxxxxxxxxxxxxxxxxxx");
                query.ForEach(x => x.CodigoAdicional = "xxxxxxxxxxxxxxxxxxxxxx");

            }
            var result = query.SingleOrDefault();

            int cantidad = query.Count();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HabilitarLicencia(int id = 0)
        {
            ViewBag.Id = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            //DateTime fechaActivacion = DateTime.Now;
            //DateTime fechaVencimiento = DateTime.Now.AddYears(1);
            var query = dbc.ProcHabilitarLicencia(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeshabilitarLicencia(int id = 0)
        {
            ViewBag.Id = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            DateTime fechaActivacion = DateTime.Now;
            DateTime fechaVencimiento = DateTime.Now.AddYears(1);
            var query = dbc.ProcDeshabilitarLicencia(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLicenciasPorVencer()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int mesActual = DateTime.Now.Month;
            int anioActual = DateTime.Now.Year;
            var query = dbc.ProcListarLicenciasPorCaducar(ID_ACCO, mesActual, anioActual).ToList();

            var total = query.Count();

            return Json(new { Data = query, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListadoLicenciasPorVencer()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int mesActual = DateTime.Now.Month;
            int anioActual = DateTime.Now.Year;
            var query = dbc.ProcListarLicenciasPorCaducar(ID_ACCO, mesActual, anioActual).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarCantidadLicenciasPorVencer()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int mesActual = DateTime.Now.Month;
            int anioActual = DateTime.Now.Year;
            var query = dbc.ProcListarLicenciasPorCaducar(ID_ACCO, mesActual, anioActual).Count();

            ViewBag.CantidadLicencias = query;

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAnioBusqueda()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ProcListarAnioBusqueda(IdCuenta);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarMeswBusqueda()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ProcListarMesBusqueda(IdCuenta, DateTime.Now.Year);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        //Culminar actualizar licencia
        public ActionResult ActualizarLicencia(int id = 0, int idVencido = 0)
        {
            ViewBag.Id = id;
            int IdPrograma = Convert.ToInt32(Request.Params["IdPrograma"].ToString());
            int IdLicencia = Convert.ToInt32(Request.Params["IdLicencia"].ToString());
            ViewBag.Id = IdPrograma;
            var queryLicDisponible = dbc.ProgramaLicencia.Where(Pl => Pl.IdPrograma == IdPrograma && Pl.Activo == false).FirstOrDefault();

            if (queryLicDisponible != null)
            {
                ViewBag.IdPrograma = queryLicDisponible.IdPrograma;
                ViewBag.IdTipoLicencia = queryLicDisponible.IdTipoLicencia;
                ViewBag.Codigo = queryLicDisponible.Codigo;
                ViewBag.Version = queryLicDisponible.VersionLic;
                ViewBag.CodigoActivacion = queryLicDisponible.CodigoActivacion;
                ViewBag.Serie = queryLicDisponible.Serie;
                ViewBag.IdLicDisponible = queryLicDisponible.Id;
                ViewBag.IdLicVencida = IdLicencia;
                idLicenciaVencida = IdLicencia;
            }
            return View();
        }

        public ActionResult CancelarAsignacionActivoLicencia(int id = 0)
        {
            ViewBag.Id = id;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            DateTime fechaActivacion = DateTime.Now;
            DateTime fechaVencimiento = DateTime.Now.AddYears(1);
            var query = dbc.ProcCancelarAsignacionActivoLicencia(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult eliminarLicencia(int id = 0)
        {
            ViewBag.Id = id;
            var query = dbc.ProcEliminarLicencia(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarListaLicencias()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (Convert.ToInt32(Session["PERMISO_LICENCIAS"]) == 1)
            {
                List<ProcListarProgramaLicencias_Result> query = dbc.ProcListarProgramaLicencias(ID_ACCO, estado, 0, IdPrograma, IdTipoLicencia).ToList();

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
                sb.Append("<th class='cabecera'>Tipo de Licencia</td>");
                sb.Append("<th class='cabecera'>Versión</td>");
                sb.Append("<th class='cabecera'>Serie</td>");
                sb.Append("<th class='cabecera'>Estado</td>");
                sb.Append("<th class='cabecera'>Fecha de Activación</td>");
                sb.Append("<th class='cabecera'>Fecha de Vencimiento</td>");
                sb.Append("</tr>");

                foreach (ProcListarProgramaLicencias_Result dr in query)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.Nombre + "</td>");
                    sb.Append("<td class='contenido'>" + dr.VAL_ACCO_PARA + "</td>");
                    sb.Append("<td class='contenido'>" + dr.VersionLic + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Serie + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Activo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaActivacion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaVencimiento + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment;filename=Licencias" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            else
            {
                List<ProListarLicenciasVistaMS_Result> query = dbc.ProListarLicenciasVistaMS(ID_ACCO, IdPrograma).ToList();

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
                sb.Append("<th class='cabecera'>#</td>");
                sb.Append("<th class='cabecera'>Programa</td>");
                sb.Append("<th class='cabecera'>Cantidad Total</td>");
                sb.Append("<th class='cabecera'>En Uso</td>");
                sb.Append("<th class='cabecera'>Disponible</td>");
                sb.Append("<th class='cabecera'>Habilitado</td>");
                sb.Append("</tr>");

                foreach (ProListarLicenciasVistaMS_Result dr in query)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.ORDEN + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Nombre + "</td>");
                    sb.Append("<td class='contenido'>" + dr.CantidadTotal + "</td>");
                    sb.Append("<td class='contenido'>" + dr.EnUso + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Disponible + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Habilitado + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment;filename=Licencias" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            return View();
        }

        public ActionResult ListarUsuarioAsociado()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbe.PL_UsuariosPorCuenta(ID_ACCO).ToList();
            return Json(new { Data = query, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarActivosxUsuario(int ID_PERS_ENTI)
        {
            var query = dbc.ObtenerActivosxUsuario(ID_PERS_ENTI).ToList();
            return Json(new { Data = query, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Renovar(string idsSeleccionados)
        {
            int cantidad = 0;
            cantidad = (idsSeleccionados.Split(',')).Length;
            ViewData["idsSeleccionados"] = idsSeleccionados;
            ViewData["cantidad"] = cantidad;
            return View();
        }


        public ActionResult RenovarLicencia(string idsProgramaLicencia,int registro = 0)
        {
            var IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int flag = 0;
            int IdProgramaLicencia = 0;
            int Cant;//Cant = 1 -> Registro || Cant 0 = -> Renovacion || Cant = 2 -> Renovacion Masiva
            int UsuarioModifica = Convert.ToInt32(Session["UserId"]);
            DateTime? FechaActivacion = null;
            int IdTipoLicencia = 0;
            string Solped = null;
            string AreaSolicitante = null;
            var Costo = Convert.ToDecimal(0);
            DateTime? FechaCompra = null;
            string TipoMoneda = null;
            int IdProveedor = 0;
            string TipoLicencia = null;
            bool RenovacionMasiva = Convert.ToBoolean(Request.Params["RenovacionMasiva"]);


            if (RenovacionMasiva == true) {

                if (string.IsNullOrEmpty(Request.Params["IdProveedor"])) { flag = 7; } else { IdProveedor = Convert.ToInt32(Request.Params["IdProveedor"]); }

                if (!string.IsNullOrEmpty(Request.Params["FechaComprav"]) && DateTime.TryParseExact(Request.Params["FechaComprav"], "M/d/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                { FechaCompra = result; }
                else { flag = 1; }

                if (string.IsNullOrEmpty(Request.Params["IdTipoLicencia"])) { flag = 5; } else { IdTipoLicencia = Convert.ToInt32(Request.Params["IdTipoLicencia"]); }

                if (string.IsNullOrEmpty(Request.Params["Costo"])) { flag = 2; } else { Costo = Convert.ToDecimal(Request.Params["Costo"]); }

                if (string.IsNullOrEmpty(Request.Form["TipoMoneda"])) { flag = 7; } else { TipoMoneda = Convert.ToString(Request.Params["TipoMoneda"]); }

                if (flag != 0)
                {
                    return Json(new { error = true, mensaje = "Registrar Informacion" });
                }

                Cant = 2;

                var codigos = idsProgramaLicencia.Split(',');

                int contador = 0;

                foreach (var codigo in codigos)
                {
                    IdProgramaLicencia = int.Parse(codigo.Trim());

                    if (contador == 0)
                    {
                        dbc.RenovarLicenciaPrograma(IdProgramaLicencia, FechaCompra, UsuarioModifica, Cant, FechaActivacion, IdTipoLicencia, Solped, AreaSolicitante, Costo, IdAcco, TipoMoneda, IdProveedor, TipoLicencia);

                        contador++;
                    }
                }

                return Json(new { error = false, mensaje = "Se renovó correctamente la licencia" });
            }
            else
            {

                if (!string.IsNullOrEmpty(Request.Params["FechaComprav"]) && DateTime.TryParseExact(Request.Params["FechaComprav"], "M/d/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {FechaCompra = result;}else{flag = 1;}

                if (string.IsNullOrEmpty(Request.Params["Costo"])){flag = 2;}else{Costo = Convert.ToDecimal(Request.Params["Costo"]);}


                if (string.IsNullOrEmpty(Request.Form["TipoMoneda"])) { flag = 7; } else { TipoMoneda = Convert.ToString(Request.Params["TipoMoneda"]); }

                if (flag != 0) { return Json(new { error = true, mensaje = "Registrar Informacion" }); }


                //Renovacion

                var codigos = idsProgramaLicencia.Split(',');

                //For renovacion
                Cant = 0;

                foreach (var codigo in codigos)
                {
                    IdProgramaLicencia = int.Parse(codigo.Trim());
                    dbc.RenovarLicenciaPrograma(IdProgramaLicencia, FechaCompra, UsuarioModifica, Cant, FechaActivacion, IdTipoLicencia, Solped, AreaSolicitante, Costo, IdAcco, TipoMoneda, IdProveedor, TipoLicencia);
                }


                //Registro

                if (registro > 0)
                {

                    if (string.IsNullOrEmpty(Request.Params["TipoLicencia"])){ flag = 3; } else {
                        TipoLicencia = Convert.ToString(Request.Params["TipoLicencia"]);

                        if (TipoLicencia == "Unitario")
                        {
                            FechaActivacion = null;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(Request.Params["FechaActivacion"]) && DateTime.TryParseExact(Request.Params["FechaActivacion"], "M/d/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultw))
                            { FechaActivacion = resultw; }
                            else { flag = 4; }
                        }

                    };

                    if (string.IsNullOrEmpty(Request.Params["IdTipoLicencia"])) { flag = 5; } else { IdTipoLicencia = Convert.ToInt32(Request.Params["IdTipoLicencia"]); }

                    if (string.IsNullOrEmpty(Request.Params["Solped"])) { flag = 6; } else { Solped = Convert.ToString(Request.Params["Solped"]); }

                    if (flag != 0) { return Json(new { error = true, mensaje = "Registrar Informacion" }); }

                    Cant = 1;

                    for (int i = 0; i < registro; i++)
                    {
                        dbc.RenovarLicenciaPrograma(IdProgramaLicencia, FechaCompra, UsuarioModifica, Cant, FechaActivacion, IdTipoLicencia, Solped,AreaSolicitante, Costo, IdAcco, TipoMoneda, IdProveedor, TipoLicencia);
                    }

                }

                return Json(new { error = false, mensaje = "Se renovó correctamente la licencia" });


            }

        }


        /*
         public ActionResult RenovarLicencia(int id = 0)
        {
            try
            {
                var Cantidad = 0;
                try
                {
                    if (Request.Params["LicenciasDisponibles"] != null)
                    {
                        int.TryParse(Request.Params["LicenciasDisponibles"], out Cantidad);
                    }
                }
                catch
                {
                    Cantidad = 0;
                }
                var fechaCompra = DateTime.ParseExact(Request.Params["FechaComprav"], "M/d/yyyy h:mm tt", CultureInfo.InvariantCulture);
                var query = dbc.RenovarLicencia(id, fechaCompra, Cantidad);
                var x = query;


                return Json(new { error = x }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.DenyGet);
            }
        }
         
         */

    }
}
