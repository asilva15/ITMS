using ERPElectrodata.Models;
using ERPElectrodata.Object.Talent;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Mvc;
using ERPElectrodata.Objects;
using ERPElectrodata.Object.Plugins;

using WebMatrix.WebData;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace ERPElectrodata.Controllers
{
    public class SeguridadController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Seguridad/

        public ActionResult Index()
        {
            bool Acceso = false;
            if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
            {
                Acceso = true; //permitiendo al acceso
            }
            
            if (Acceso == true)
            {
                return View();
            }
            else
            {
                return Content("<h3><b>Necesitas autorización para esta sección.</b></h3>");
            }
        }

        public ActionResult Usuarios()
        {
            return View();
        }

        public ActionResult Perfiles()
        {
            return View();
        }

        public ActionResult Permisos()
        {
            return View();
        }

        public ActionResult Eventos()
        {
            return View();
        }

        public ActionResult RestablecerContrasena()
        {
            return View();
        }

        public ActionResult UsuarioExterno()
        {
            return View();
        }

        public ActionResult RestableceContrasena(int Id = 0)
        {
            SendMail email = new SendMail();//Para enviar el correo
            Criptografia cripto = new Criptografia(); // Para encriptar password

            var query = (from ce in dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == Id)
                         join ue in dbe.Usuarios on ce.UserId equals ue.UserId
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         select new
                         {
                             FIR_NAME=ce.FIR_NAME,
                             LAS_NAME=ce.LAS_NAME,
                             NUM_TYPE_DI=ce.NUM_TYPE_DI,
                             UserId = ce.UserId,
                             USUARIO = ue.Usuario1,
                             CORREO1 = pe.EMA_PERS,
                             CORREO2 = pe.EMA_ELEC
                         }).Single();

            var usuario = query.USUARIO;
            String nombre = query.FIR_NAME.ToUpper();
            String apellido = query.LAS_NAME.ToUpper();
            String correo1;
            String correo2;

            int ctacorreo = 0;
            if (query.CORREO1 != null)
            {
                correo1 = query.CORREO1; 
                ctacorreo = 1;
            }
            else 
            {
                correo1 = "";
            }
            if (query.CORREO2 != null)
            {
                correo2 = query.CORREO2;
                ctacorreo = 2;
            }
            else 
            {
                correo2 = "";
            }

            //Generando nueva clave  
            char c = nombre[0];
            char b = apellido[0];
            String DNI = query.NUM_TYPE_DI.ToString();
            String ver = nombre.Substring(0, 1);
            String ver1 = apellido.Substring(0, 1);

            gth objGth = new gth();
            String CLAVE = objGth.GenerarContrasenia(); //ver.Trim().ToUpper() + ver1.Trim().ToUpper() + DNI.Trim();
            String xxx = cripto.Encriptar(CLAVE);

            if (ModelState.IsValid)
            {
                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("ERROR_1");
                }

                dbe.RestablecerContrasena(xxx, query.UserId, IdUser);

                if(ctacorreo==0)
                {
                    return Content("EXISTE");
                }
                else if (correo1 == correo2)
                {
                    email.EnviaRestaurarContrasena(correo1, (nombre + " " + apellido), usuario, CLAVE);
                }
                else 
                {
                    email.EnviaRestaurarContrasena(correo1, (nombre + " " + apellido), usuario, CLAVE);
                    email.EnviaRestaurarContrasena(correo2, (nombre + " " + apellido), usuario, CLAVE);
                }
                return Content("OK");
            }
            else
            {
                return Content("ERROR_2");
            }
        }

        public ActionResult prueba()
        {
            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','OK','OK');}window.onload=init;</script>");
        }

        public ActionResult ListarUsuarioPerfil(int id = 0)
        {
            try
            {
                int ID_ENTI = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id).ID_ENTI2.Value;
                int UserId = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ID_ENTI).UserId.Value;

                var query = (from u in dbe.Usuarios.Where(x => x.UserId == UserId)
                             join up in dbe.UsuarioPerfils on u.Id equals up.IdUsuario
                             join p in dbe.Perfils on up.IdPerfil equals p.Id
                             select new
                             {
                                 IdUsuarioPerfil = up.Id,
                                 NombrePerfil = p.Perfil1,
                                 up.Estado
                             }).Where(x => x.Estado != false).OrderBy(x => x.NombrePerfil);

                Organization org = new Organization();
                var Organ = org.chart_staff(id);

                return Json(new { Data = query, Orga = Organ }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "", Count = 0 }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult EliminarUsuarioPerfil()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);

                //int RoleId = Convert.ToInt32(Request.Params["RoleId"]);
                //int UserId_Affected = Convert.ToInt32(Request.Params["UserId"]);

                int IdUsuarioPerfil = Convert.ToInt32(Request.Params["Id"]);
                //int ID_PERS_ENTI_Affected = Convert.ToInt32(Request.Params["idpersenti"]);

                //webpages_UsersInRoles ur = dbe.webpages_UsersInRoles.Single(x => x.UserId == UserId_Affected && x.RoleId == RoleId);

                UsuarioPerfil objUsuarioPerfil = dbe.UsuarioPerfils.Where(x => x.Id == IdUsuarioPerfil).SingleOrDefault();

                //dbe.webpages_UsersInRoles.Remove(ur);
                objUsuarioPerfil.Estado = false;
                dbe.SaveChanges();

                LOG_TABLE_ROLES ltr = new LOG_TABLE_ROLES();

                ltr.ID_PERS_ENTI_Affected = objUsuarioPerfil.IdUsuario;
                ltr.UserId = UserId;
                //ltr.UserId_Affected = UserId_Affected;
                ltr.ID_ACTI_TABL_ROLE = 3;
                ltr.RoleId_Affected = objUsuarioPerfil.IdPerfil;
                ltr.CREATED = DateTime.Now;

                dbe.LOG_TABLE_ROLES.Add(ltr);
                dbe.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult ListarPerfil()
        {
            var query = (from p in dbe.Perfils
                         select new
                         {
                             ROLE_ID = p.Id,
                             ROLE_NAME = p.Perfil1,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPerfilesxCuenta()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                List<RolesxCuenta_Result> query = dbe.RolesxCuenta(ID_ACCO).ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GuardarUsuarioPerfil()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["idpersenti"]);
                int IdPerfil = Convert.ToInt32(Request.Params["idrole"]);

                var queryPE = from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                              join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                              join u in dbe.Usuarios on ce.UserId equals u.UserId
                              select new
                              {
                                  IdUsuario = u.Id
                              };

                int Usuario = queryPE.SingleOrDefault().IdUsuario;
                
                //int UserId_Affected = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ID_ENTI).UserId.Value;

                var query = dbe.UsuarioPerfils.Where(x => x.IdUsuario == Usuario && x.IdPerfil == IdPerfil).ToList();
                //var query = dbe.webpages_UsersInRoles.Where(x => x.UserId == UserId_Affected && x.RoleId == ID_ROLE);

                if (query.Count() == 0)
                {

                    UsuarioPerfil objUP = new UsuarioPerfil();

                    objUP.IdUsuario = queryPE.SingleOrDefault().IdUsuario;
                    objUP.IdPerfil = IdPerfil;
                    objUP.FechaCrea = DateTime.Now;
                    objUP.Estado = true;

                    dbe.UsuarioPerfils.Add(objUP);
                    dbe.SaveChanges();

                    LOG_TABLE_ROLES ltr = new LOG_TABLE_ROLES();

                    ltr.ID_PERS_ENTI_Affected = ID_PERS_ENTI;
                    ltr.UserId = UserId;
                    //ltr.UserId_Affected = UserId_Affected;
                    ltr.ID_ACTI_TABL_ROLE = 1;
                    ltr.RoleId_Affected = IdPerfil;
                    ltr.CREATED = DateTime.Now;

                    dbe.LOG_TABLE_ROLES.Add(ltr);
                    dbe.SaveChanges();

                    return Content("OK");
                }
                else
                {
                    List<UsuarioPerfil> queryUP = dbe.UsuarioPerfils.Where(x => x.IdUsuario == Usuario && x.IdPerfil == IdPerfil && x.Estado == false).ToList();

                    if (queryUP.Count() == 0)
                    {
                        return Content("EXISTE");
                    }
                    else
                    {
                        UsuarioPerfil objUP = queryUP.FirstOrDefault();
                        objUP.FechaModifica = DateTime.Now;
                        objUP.Estado = true;
                        dbe.SaveChanges();
                        return Content("OK");
                    }
                }
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult IndexEvento()
        {
            return View();
        }

        public ActionResult EventoAuditable()
        {
            return View();
        }

        public ActionResult ObtenerCuentaCargo(int id = 0)
        {
            try
            {
                int IdAccoType = 0;
                IdAccoType = Convert.ToInt32(dbc.ACCOUNTs.Single(x => x.ID_ACCO == id).ID_ACCO_TYPE);
                return Json(IdAccoType, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RegistroUsuarioExterno()
        {

            int usuarioLogin = Convert.ToInt32(Session["UserId"]);

            String sexo= "", cuenta = "", colaCuenta = "", cbCargo = "", area = "", cbTipoUsuario = "";
            try { sexo = Request.Params["cbSexo"].ToString(); }
            catch { sexo = ""; }
            try { cuenta = Request.Params["cbCuenta"].ToString(); }
            catch { cuenta = ""; }
            try{colaCuenta = Request.Params["cbColaCuenta"].ToString();}
            catch { colaCuenta = ""; }
            try { cbCargo = Request.Params["cbCargo"].ToString(); }
            catch { cbCargo = ""; }
            try { area = Request.Params["cbArea"].ToString(); }
            catch { area = ""; }
            try { cbTipoUsuario = Request.Params["cbTipoUsuario"].ToString(); }
            catch { cbTipoUsuario = ""; }

            String pNombre = Request.Params["txtPrimerNombre"].ToString();
            String aPaterno = Request.Params["txtApePaterno"].ToString();
            String aMaterno = Request.Params["txtApeMaterno"].ToString();
            String celular = Request.Params["txtCelular"].ToString();
            String correo = Request.Params["txtCorreo"].ToString();
            String cargo = Request.Params["txtCargo"].ToString();


            if (pNombre == "" || aPaterno == "" || sexo == "" || correo == "" || cbTipoUsuario == "" || colaCuenta == "" || area == "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.Mensaje) top.Mensaje('ERROR','" + "Debe completar los datos obligatorios." + "');}window.onload=init;</script>");
            }


            int IdCuenta = Convert.ToInt32(cuenta);
            int flag = 0;
            if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == IdCuenta).ID_ACCO_TYPE == 1)
            {
                if (cbCargo == "")
                {
                    flag = 1;
                }

            }
            else
            {
                if (cargo == "")
                {
                    flag = 1;
                }
            }
            if (flag==1){
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.Mensaje) top.Mensaje('ERROR','" + "Registre el cargo." + "');}window.onload=init;</script>");
            }

            try
            {

                var cla = new CLASS_ENTITY();
                cla.FIR_NAME = pNombre.Trim().ToUpper();
                cla.LAS_NAME = aPaterno.Trim().ToUpper();
                cla.MOT_NAME = aMaterno.Trim().ToUpper();
                cla.SEX_ENTI = sexo;
                cla.VIG_ENTI = true;
                cla.ID_TYPE_ENTI = 2;
                cla.ID_TYPE_DI = 1;
                cla.CREATED = DateTime.Now;
                cla.ID_USER = usuarioLogin;

                dbe.CLASS_ENTITY.Add(cla);
                dbe.SaveChanges();

                int IdEnti = Convert.ToInt32(cla.ID_ENTI);
                int cola = Convert.ToInt32(colaCuenta);
                int IdTipoUsuario = Convert.ToInt32(cbTipoUsuario);
                int ID_PRIO = Convert.ToInt32(dbe.TYPE_CLIENT.Single(x => x.ID_TYPE_CLIE == IdTipoUsuario).ID_PRIO);
                int IdCompania = 0;
                try
                {
                    //Obteniendo compañía
                    var query1 = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 6 && x.ID_ACCO == IdCuenta && x.VIG_ACCO_PARA == true).FirstOrDefault();
                    IdCompania = Convert.ToInt32(query1.VAL_ACCO_PARA);
                }
                catch
                {
                    IdCompania = 0;
                }
                try
                {


                    var PERS_ENTI = new PERSON_ENTITY();
                    PERS_ENTI.ID_ENTI1 = IdCompania;
                    PERS_ENTI.ID_ENTI2 = IdEnti;
                    PERS_ENTI.ID_QUEU = cola;
                    PERS_ENTI.ID_PRIO = ID_PRIO;
                    PERS_ENTI.ID_TYPE_CLIE = IdTipoUsuario;
                    PERS_ENTI.FOT_PERS = "0";
                    if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == IdCuenta).ID_ACCO_TYPE == 1)
                        PERS_ENTI.ID_CHAR = Convert.ToInt32(cbCargo);
                    else
                        PERS_ENTI.CAR_PERS = cargo.Trim().ToUpper();
                    PERS_ENTI.CEL_PERS = celular;
                    PERS_ENTI.EMA_ELEC = correo;
                    PERS_ENTI.EMA_PERS = correo;
                    PERS_ENTI.CREATED = DateTime.Now;
                    PERS_ENTI.ID_USER = usuarioLogin;
                    PERS_ENTI.VIG_PERS_ENTI = true;
                    PERS_ENTI.ID_AREA = Convert.ToInt32(area);                    

                    dbe.PERSON_ENTITY.Add(PERS_ENTI);
                    dbe.SaveChanges();

                    int IdPersEnti = Convert.ToInt32(PERS_ENTI.ID_PERS_ENTI);

                    var ACCO_ENTI = new ACCOUNT_ENTITY();
                    ACCO_ENTI.ID_PERS_ENTI = IdPersEnti;
                    ACCO_ENTI.ID_ACCO = IdCuenta;
                    ACCO_ENTI.VIG_ACCO_ENTI = true;
                    ACCO_ENTI.DEF_ACCO = true;
                    ACCO_ENTI.VIS_REQU = true;
                    ACCO_ENTI.VIS_ASSI = IdCuenta == 4 ? false : true;
                    ACCO_ENTI.VIS_TALE = false;

                    dbe.ACCOUNT_ENTITY.Add(ACCO_ENTI);
                    dbe.SaveChanges();

                    #region Creación de Usuario ITMS
                    string usernameITMS = string.Empty;
                    string passwordITMS = string.Empty;

                    usernameITMS = QuitAccents(pNombre.Trim().Substring(0, 1).ToLower() + aPaterno.Trim().ToLower());
                    //passwordITMS = ConfigurationManager.AppSettings["clave"].ToString() + DateTime.Now.Year;
                    passwordITMS = "ITMS" + Convert.ToString(DateTime.Now.Year);

                    var query = dbe.RH_VALIDATE_ACCOUNT_ITMS(usernameITMS).ToList();
                    if (query.Count() == 0)
                    {
                        //Creación en tabla UserProfile
                        WebSecurity.CreateUserAndAccount(usernameITMS, passwordITMS);
                        //WebSecurity.Login(usernameITMS, passwordITMS);
                    }
                    else
                    {
                        if (aMaterno != null && aMaterno != "")
                        {
                            usernameITMS = usernameITMS + aMaterno.Trim().Substring(0, 1).ToLower();
                            WebSecurity.CreateUserAndAccount(usernameITMS.ToLower(), passwordITMS);
                            //WebSecurity.Login(usernameITMS, passwordITMS);
                        }
                        else
                        {
                            usernameITMS = pNombre.Trim().ToLower() + "." + aPaterno.Trim().ToLower();
                            WebSecurity.CreateUserAndAccount(usernameITMS.ToLower(), passwordITMS);
                            //WebSecurity.Login(usernameITMS, passwordITMS);
                        }
                    }

                    #region Actualizacion de  idUser de la tabla UserProfile a la tabla CLASS_ENTITY
                    var cargarUP = dbe.RH_VALIDATE_USERPROFILE(usernameITMS).First();
                    int UserId = cargarUP.UserId;
                    cla.UserId = UserId;
                    dbe.CLASS_ENTITY.Attach(cla);
                    dbe.Entry(cla).State = EntityState.Modified;
                    dbe.SaveChanges();

                        //Insertamos en la tabla Usuario
                        Criptografia cripto = new Criptografia();
                        Usuario objUsuario = new Usuario();
                        objUsuario.Usuario1 = usernameITMS;
                        objUsuario.Password = cripto.Encriptar(passwordITMS);
                        objUsuario.IntentosFallidos = 0;
                        objUsuario.Bloqueado = false;
                        objUsuario.FechaCrea = DateTime.Now;
                        objUsuario.Estado = true;
                        objUsuario.UsuarioCrea = 34;
                        objUsuario.CodigoIdioma = 0;
                        objUsuario.UserId = cargarUP.UserId;
                        dbe.Usuarios.Add(objUsuario);
                        dbe.SaveChanges();
                        #endregion
                    #endregion

                        //Enviar Correo
                        try
                        {
                            #region Enviar Correo de Confirmacion de Creacion de Usuario ITMS
                            SendMail mail = new SendMail();
                            String nombreCuenta = Convert.ToString(dbc.ACCOUNTs.Single(x => x.ID_ACCO == IdCuenta).NAM_ACCO);

                            String NombreCompleto = pNombre.Trim().ToUpper() + " " + aPaterno.Trim().ToUpper() + " " + aMaterno.Trim().ToUpper();
                            mail.CreacionUsuarioExterno(NombreCompleto, usernameITMS, passwordITMS, correo.Trim(), nombreCuenta);
                            #endregion


                        }
                        catch (Exception ex)
                        {

                        }
                }
                catch (Exception)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.Mensaje) top.Mensaje('ERROR','Contacte al administrador.');}window.onload=init;</script>");
                }
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('OK','El usuario se registró correctamente');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.Mensaje) top.Mensaje('ERROR','Contacte al administrador.');}window.onload=init;</script>");
            }
            
        }

        public string QuitAccents(string inputString)
        {
            Regex a = new Regex("[á|à|ä|â]", RegexOptions.Compiled);
            Regex e = new Regex("[é|è|ë|ê]", RegexOptions.Compiled);
            Regex i = new Regex("[í|ì|ï|î]", RegexOptions.Compiled);
            Regex o = new Regex("[ó|ò|ö|ô]", RegexOptions.Compiled);
            Regex u = new Regex("[ú|ù|ü|û]", RegexOptions.Compiled);
            Regex n = new Regex("[ñ|Ñ]", RegexOptions.Compiled);
            inputString = a.Replace(inputString, "a");
            inputString = e.Replace(inputString, "e");
            inputString = i.Replace(inputString, "i");
            inputString = o.Replace(inputString, "o");
            inputString = u.Replace(inputString, "u");
            inputString = n.Replace(inputString, "n");
            return inputString;
        }


    }
}
