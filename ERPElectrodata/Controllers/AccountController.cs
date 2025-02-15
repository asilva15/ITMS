using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using ERPElectrodata.Filters;
using ERPElectrodata.Models;
using System.Globalization;
using System.Data.Entity;
using System.Threading;
using ERPElectrodata.Object.Plugins;
using ERPElectrodata.Object.Enumerador;
using System.Xml;
using System.Text;
using db = ERPElectrodata.Models;
using ent = ERPElectrodata.Object;
using System.Collections;
using ERPElectrodata.Objects;
using ERPElectrodata.Object.Talent;
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;

namespace ERPElectrodata.Controllers
{

    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        ent.IdiomaDetalle dbIdiomaDetalle = new ent.IdiomaDetalle();
        ent.Idioma dbIdioma = new ent.Idioma();
        public static ArrayList arrNombres = new ArrayList();
        public static ArrayList arrIds = new ArrayList();
        public static ArrayList arrLlaves = new ArrayList();
        public static ArrayList arrTextos = new ArrayList();
        public static ArrayList arrResultado = new ArrayList();

        public ActionResult ListUserProfile()
        {
            var query = dbe.MA_LIST_USER_PROFILE().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LocationAll()
        {

            var query = dbc.PARAMETERs.Where(p => p.ID_PARA == 1 || p.ID_PARA == 2)
                .Join(dbc.ACCOUNT_PARAMETER, p => p.ID_PARA, ap => ap.ID_PARA, (p, ap) => new { p.ID_PARA, ap.VAL_ACCO_PARA, ap.ID_ACCO });

            var result = (from x in query
                          group x by x.ID_ACCO into xg
                          select new
                          {
                              ID_ACCO = xg.Select(g => g.ID_ACCO).FirstOrDefault(),
                              COOR = xg.Select(g => g.VAL_ACCO_PARA),
                          });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AccountsByUser()
        {
            //var qac = dbc.ACCOUNT_CLIENT.ToArray();
            if (Session["UserId"] == null) return null;

            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            var query = dbe.CLASS_ENTITY.Where(x => x.UserId == UserId)
                .Join(dbe.PERSON_ENTITY, ce => ce.ID_ENTI, pe => pe.ID_ENTI2, (ce, pe) => new { pe.ID_PERS_ENTI })
                .Join(dbe.ACCOUNT_ENTITY, ae => ae.ID_PERS_ENTI, pe => pe.ID_PERS_ENTI, (ae, pe) => new
                {
                    ID_PERS_ENTI = ae.ID_PERS_ENTI,
                    VIG_ACCO_ENTI = pe.VIG_ACCO_ENTI,
                    DEF_ACCO = pe.DEF_ACCO,
                    ID_ACCO = pe.ID_ACCO
                })
                .Where(x => x.VIG_ACCO_ENTI == true);

            var account = dbc.ACCOUNTs;

            var result = (from ce in query.ToList()
                              //join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                              //join ac in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ac.ID_PERS_ENTI
                          join a in account.ToList() on ce.ID_ACCO equals a.ID_ACCO
                          //where ac.VIG_ACCO_ENTI == true
                          select new
                          {
                              ce.DEF_ACCO,
                              ce.ID_PERS_ENTI,
                              a.NAM_ACCO,
                              a.ID_ACCO
                          }).OrderBy(x => x.NAM_ACCO);
            var qrr = result.ToList();
            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCuentasxUsuario(string q, string page)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = dbc.ListarCuentaxUsuario(UserId, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCuentasxUsuarioTR(string q, string page)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = dbc.ListarCuentaxUsuario(UserId, "").ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Session["MAIN"] = "mp1";
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Error = "";
            return View();
        }

        [AllowAnonymous]
        public ActionResult RecordarContrasena(string returnUrl)
        {
            Session["MAIN"] = "mp1";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RecordarContrasenaCodigo(FormCollection frmC)
        {
            string correo = Request.Params["correo"];

            try
            {
                var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.EMA_ELEC == correo)
                             join ce in dbe.CLASS_ENTITY.Where(y => y.UserId!= null) on pe.ID_ENTI2 equals ce.ID_ENTI
                             join ue in dbe.Usuarios on ce.UserId equals ue.UserId
                             select new
                             {
                                 FIR_NAME = ce.FIR_NAME,
                                 LAS_NAME = ce.LAS_NAME,
                                 NUM_TYPE_DI = ce.NUM_TYPE_DI,
                                 UserId = ce.UserId,
                                 USUARIO = ue.Usuario1,
                                 CORREO1 = pe.EMA_PERS,
                                 CORREO2 = pe.EMA_ELEC
                             }).Single();

                var usuario = query.USUARIO;
                String nombre = query.FIR_NAME.ToUpper();
                String apellido = query.LAS_NAME.ToUpper();
                String correo1 = query.CORREO1 ?? "";
                String correo2 = query.CORREO2 ?? "";
                int ctacorreo = (correo1 != "") ? 1 : ((correo2 != "") ? 2 : 0);

                if (ModelState.IsValid)
                {
                    int IdUser = 34;

                    string token = GeneraRandomToken();

                    dbe.AlmacenarCodigoClave(token, query.UserId);

                    SendMail email = new SendMail();//Para enviar el correo
                    if (ctacorreo == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('ERROR','Ha ocurrido un error, contacte al administrador.');}window.onload=init;</script>");
                    }
                    else if (correo1 == correo2)
                    {
                        email.EnviarCodigoReestablecimiento(correo1, $"{nombre} {apellido}", usuario, token);
                    }
                    else
                    {
                        email.EnviarCodigoReestablecimiento(correo1, $"{nombre} {apellido}", usuario, token);
                        email.EnviarCodigoReestablecimiento(correo2, $"{nombre} {apellido}", usuario, token);
                    }
                    TempData["Mensaje"] = "OK";
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('OK','Se ha enviado un codigo al correo brindado.');}window.onload=init;</script>");
                }
                else
                {
                    TempData["Mensaje"] = "Ha ocurrido un error, contacte al administrador.";
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','Ha ocurrido un error, contacte al administrador.');}window.onload=init;</script>");
                }
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = "El correo no se encuentra asociado a un usuario, por favor comunicarse con RRHH.";
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','El correo no se encuentra asociado a un usuario, por favor comunicarse con RRHH.');}window.onload=init;</script>");
            }
        }

        [HttpPost]
        public ActionResult CambiarContrasena()
        {
            string correo = Request.Params["correo"];
            string codigo = Request.Params["codigo"];
            Criptografia cripto = new Criptografia(); // Para encriptar password

            try
            {
                var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.EMA_ELEC == correo)
                             join ce in dbe.CLASS_ENTITY.Where(y => y.UserId != null) on pe.ID_ENTI2 equals ce.ID_ENTI
                             join ue in dbe.Usuarios on ce.UserId equals ue.UserId
                             select new
                             {
                                 FIR_NAME = ce.FIR_NAME,
                                 LAS_NAME = ce.LAS_NAME,
                                 NUM_TYPE_DI = ce.NUM_TYPE_DI,
                                 UserId = ce.UserId,
                                 USUARIO = ue.Usuario1,
                                 CORREO1 = pe.EMA_PERS,
                                 CORREO2 = pe.EMA_ELEC
                             }).Single();

                var usuario = query.USUARIO;
                String nombre = query.FIR_NAME.ToUpper();
                String apellido = query.LAS_NAME.ToUpper();
                string correo1 = query.CORREO1 ?? "";
                string correo2 = query.CORREO2 ?? "";
                int ctacorreo = (correo1 != "") ? 1 : ((correo2 != "") ? 2 : 0);


                if(!ModelState.IsValid)
                {
                    TempData["Mensaje"] = "Ha ocurrido un error, contacte al administrador.";
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','Ha ocurrido un error, contacte al administrador.');}window.onload=init;</script>");
                }

                int IdUser = 34;

                //VALIDAR CODIGO 
                ObjectResult<int?> verficaCodigoBD = dbe.ObtenerUsuarioCodigo(codigo, query.UserId);
                int? verficaCodigo = verficaCodigoBD.FirstOrDefault();

                if (verficaCodigo != 1)
                {
                    TempData["Mensaje"] = "Ha ocurrido un error, el codigo no es válido.";
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','El codigo no es válido.');}window.onload=init;</script>");
                }


                        gth objGth = new gth();
                        String CLAVE = objGth.GenerarContrasenia();

                        String xxx = cripto.Encriptar(CLAVE);

                SendMail email = new SendMail();//Para enviar el correo

                dbe.RestablecerContrasena(xxx, query.UserId, query.UserId);

                dbe.ValidarContrasena(query.UserId, xxx, query.UserId);

                if (ctacorreo == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','Ha ocurrido un error, contacte al administrador.');}window.onload=init;</script>");
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

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.mensaje) top.mensaje('OK','Se ha enviado el codigo al correo brindado.');}window.onload=init;</script>");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = "El correo no se encuentra asociado a un usuario, por favor comunicarse con RRHH.";
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','El correo no se encuentra asociado a un usuario, por favor comunicarse con RRHH.');}window.onload=init;</script>");
            }
        }

        private string GeneraRandomToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            Criptografia cripto = new Criptografia();
            bool Flag = false;

            string Usuario = model.UserName;
            string Password = model.Password;

            string xxx = cripto.Desencriptar("jjzP5dECqi83y5dl3gpORQ==");
            string xxy = cripto.Encriptar("ed20233");
            string xyz = cripto.Desencriptar("jrY2dqtdSgdvYcwNQhtlMQ==");
            if (ModelState.IsValid)
            {
                LoginValidar_Result obj = new LoginValidar_Result();
                obj = dbe.LoginValidar(Usuario, cripto.Encriptar(Password)).SingleOrDefault();
                //string v = cripto.Desencriptar("MpeQW8J5WBFyYZ5pXFpMmw==");
                //string dataj = obj.Usuario;

                if (obj != null)
                {
                    Flag = AsignarPermisos(Usuario, obj);
                    //listasIdioma(arrNombres, arrIds, arrLlaves, arrTextos); //aqui se envian los arrays vacios y se instancia la funcion
                    returnUrl = String.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                }
                else
                {
                    var usu = dbe.Usuarios.Where(x => x.Usuario1 == Usuario).SingleOrDefault();
                    if (usu != null)
                    {
                        if (usu.Bloqueado == true)
                        {
                            ModelState.AddModelError("", "Su usuario ha sido bloqueado luego de 5 intentos fallidos. Por favor, póngase en contacto con el administrador.");
                            ViewBag.Error = "Su usuario ha sido bloqueado luego de 5 intentos fallidos. Por favor, póngase en contacto con el administrador.";
                            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('ERROR','El inicio de sesión está bloqueado. Haga clic en Restablecer password para restablecer su cuenta');}window.onload=init;</script>");
                        }
                        else
                        {
                            ModelState.AddModelError("", "El nombre de usuario o la contraseña especificados son incorrectos.");
                            ViewBag.Error = "El nombre de usuario o la contraseña especificados son incorrectos.";
                            return Content("<script type='text/javascript'> function init() {" +
                                "if(top.mensaje) top.mensaje('ERROR','La contraseña es incorrecta. Vuelve a intentarlo o haga clic en Restablecer password en caso no la recuerde');}window.onload=init;</script>");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "El nombre de usuario o la contraseña especificados son incorrectos.");
                        ViewBag.Error = "El nombre de usuario o la contraseña especificados son incorrectos.";
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('ERROR','La contraseña es incorrecta. Vuelve a intentarlo o haga clic en Restablecer password en caso no la recuerde');}window.onload=init;</script>");
                    }
                    //}
                }
            }
            if (Convert.ToInt32(Session["USUARIO_EXTERNO"]) == 1)
            {
                return RedirectToAction("IndexUsuarioExterno", "Home");///inicio de 1 capa como cliente externo
            }
            else
            {
                if (Flag)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);

                    //Usuario OT
                    if (Convert.ToInt32(Session["ID_ACCO"]) == 55 && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1 && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('OK_HB_OT','0');}window.onload=init;</script>");
                    }


                    if (returnUrl == "/Home/Index" || returnUrl == "/")
                    {
                        ViewBag.ReturnUrl = returnUrl;
                        //return RedirectToAction("Principal", "Home");
                        return Content("<script type='text/javascript'> function init() {" +
                "if(top.mensaje) top.mensaje('OK','0');}window.onload=init;</script>");
                    }

                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.mensaje) top.mensaje('OK','0');}window.onload=init;</script>");
                }
                else
                {
                    ViewBag.Error = "The Username or Password specified are incorrect";
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('ERROR','Usuario sin acceso. Por favor, póngase en contacto con el administrador.');}window.onload=init;</script>");
                }
            }
        }
        public void listasIdioma(ArrayList arrNombres, ArrayList arrIds, ArrayList arrLlaves, ArrayList arrTextos)
        {
            XmlDocument xDoc = new XmlDocument();
            String file_path = System.AppDomain.CurrentDomain.BaseDirectory + "idiomas.xml";
            xDoc.Load(file_path);
            //se obtienen los datos del xml document
            XmlNodeList xIdiomas = xDoc.GetElementsByTagName("idioma");
            XmlNodeList xNombres = ((XmlElement)xIdiomas[0]).GetElementsByTagName("nombre");
            XmlNodeList xIds = ((XmlElement)xIdiomas[0]).GetElementsByTagName("id");
            XmlNodeList xLlaves = ((XmlElement)xIdiomas[0]).GetElementsByTagName("llave");
            XmlNodeList xTextos = ((XmlElement)xIdiomas[0]).GetElementsByTagName("texto");
            //se rellena los datos en el array
            foreach (XmlElement nodo in xNombres)
            {
                arrNombres.Add(nodo.InnerText);
            }
            foreach (XmlElement nodo in xIds)
            {
                arrIds.Add(nodo.InnerText);
            }
            foreach (XmlElement nodo in xLlaves)
            {
                arrLlaves.Add(nodo.InnerText);
            }
            foreach (XmlElement nodo in xTextos)
            {
                arrTextos.Add(nodo.InnerText);
            }
        }
        public bool AsignarPermisos(string Usuario, LoginValidar_Result objUsuario)
        {
            try
            {
                textInfo = cultureInfo.TextInfo;

                Session["NAM_PERS"] = textInfo.ToTitleCase(objUsuario.FIR_NAME.ToLower()) + " " + textInfo.ToTitleCase(objUsuario.LAS_NAME.ToLower());
                Session["ID_CLAS_ENTI"] = objUsuario.ID_ENTI;
                Session["ID_PERS_ENTI"] = objUsuario.ID_PERS_ENTI;
                Session["ID_FOTO"] = Convert.ToString(objUsuario.ID_FOTO) + ".jpg";
                Session["ID_AREA"] = objUsuario.ID_AREA;
                Session["ID_ACCO"] = objUsuario.ID_ACCO;
                Session["NAM_ACCO"] = objUsuario.NAM_ACCO;
                Session["ID_QUEU"] = objUsuario.ID_QUEU;
                Session["VIS_ALL_QUEU"] = objUsuario.VIS_ALL_QUEU;
                Session["VIS_ALL_CHAN"] = objUsuario.VIS_ALL_CHAN;
                Session["BANNER"] = objUsuario.VAL_ACCO_PARA;

                Session["UserName"] = Usuario;
                Session["UserId"] = objUsuario.UserId;
                Session["Cargo"] = objUsuario.Cargo;
                Session["FechaActualizacion"] = objUsuario.FechaActualizacion;

                Session["POINT"] = 0;

                Session["ADMINISTRADOR_SISTEMA"] = 0;
                Session["ADMINISTRADOR"] = 0;
                Session["SUPERVISOR_SERVICEDESK"] = 0;
                Session["SUPERVISOR_OPERACIONES"] = 0;
                Session["SUPERVISOR_PMO"] = 0;
                Session["SUPERVISOR_SOPORTE"] = 0;
                Session["SUPERVISOR_INVENTARIO"] = 0;
                Session["SUPERVISOR_DOCUMENTOS"] = 0;
                Session["SUPERVISOR_CONTROLDOCUMENTARIO"] = 0;
                Session["SUPERVISOR_EJECUTIVOCOMERCIAL"] = 0;
                Session["SUPERVISOR_RRHH"] = 0;
                Session["SUPERVISOR_FINANZAS"] = 0;
                Session["SERVICEDESK"] = 0;
                Session["OPERACIONES"] = 0;
                Session["PROJECTMANAGER"] = 0;
                Session["SOPORTE"] = 0;
                Session["INVENTARIO"] = 0;
                Session["CONTROLDOCUMENTARIO"] = 0;
                Session["EJECUTIVOCOMERCIAL"] = 0;
                Session["EJECUTIVOCOMERCIAL_OUTSOURCING"] = 0;
                Session["RRHH"] = 0;
                Session["FINANZAS"] = 0;
                Session["USUARIO_PMO"] = 0;
                Session["AUDITOR_TICKET"] = 0;
                Session["AUDITOR_ACTIVIDADES"] = 0;
                Session["AUDITOR_PMO"] = 0;
                Session["AUDITOR_ACTIVO"] = 0;
                Session["AUDITOR_INVENTARIO"] = 0;
                Session["AUDITOR_DOCUMENTOS"] = 0;
                Session["AUDITOR_CONTROLDOCUMENTOS"] = 0;
                Session["AUDITOR_EJECUTIVOCOMERCIAL"] = 0;
                Session["AUDITOR_TALENTO"] = 0;
                Session["AUDITOR_FINANZAS"] = 0;
                Session["APROBADOR_OPERACIONES"] = 0; /*GESTION DEL CONOCIMIENTO GCR161203*/
                Session["SISTEMA_INTEGRADO_GESTION"] = 0;
                Session["CMDB"] = 0;
                Session["PERMISO_LICENCIAS"] = 0;
                Session["SUPERVISOR_OUTSOURCING"] = 0;
                Session["RENOVACIONES"] = 0;
                Session["SUPERVISOR_PMO_OUTSOURCING"] = 0;
                Session["PROJECTMANAGER_OUTSOURCING"] = 0;
                Session["USUARIO_EXTERNO"] = 0;
                Session["PMO_GERENCIA"] = 0;
                Session["PMO_GERENCIA_ITO"] = 0;
                Session["ADMINPORTAL"] = 0;
                Session["APROBAR_SELECCION_PERSONAL"] = 0;
                Session["USUARIO_EXTERNO_TICKET"] = 0;
                Session["PMO_SOPORTE"] = 0;
                Session["RENOVACIONES_OP"] = 0;
                Session["SUPERVISOR_INFORME"] = 0;
                Session["INFORME"] = 0;
                Session["COORDINADOR_SERVICEDESK_BNV"] = 0;
                Session["EJECUTOR_TAREAS_MINSUR_MARCOBRE_RAURA"] = 0;
                Session["GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA"] = 0;
                Session["SUPERVISOR_GESTOR_ACTIVOS"] = 0;
                Session["GESTOR_ACTIVOS_MINSUR_MARCOBRE_RAURA"] = 0;
                Session["SUPERVISOR_PMO_LISTADO"] = 0;

                Session["GESTOR_DOCUMENTOS_ACTIVOS"] = 0;
                Session["USUARIO_NOTIFICACIONES"] = 0;
                Session["GESTOR_ACTIVOS_BNV_MICROINFORMATICO"] = 0;
                Session["SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO"] = 0;
                Session["GESTOR_ACTIVOS_BNV_INFRAESTRUCTURA"] = 0;
                Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"] = 0;
                Session["SUPERVISOR_PERMISO_LICENCIAS"] = 0;
                Session["GESTOR_ACTIVOS_BNV_MOVIL"] = 0;
                Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"] = 0;

                //CUENTAS Y PERMISOS
                Session["SUPERVISOR_SERVICEDESK_ELECTRODATA"] = 0;
                Session["SUPERVISOR_SERVICEDESK_CUENTA"] = 0;
                Session["GESTOR_ACTIVOS_HUDBAY_OT"] = 0;
                Session["GESTOR_ACTIVOS_HUDBAY_IT"] = 0;

                //CuentaAdministradorCategoria
                Session["ADMINISTRADOR_CATEGORIA"] = 0;

                Session["CERTIFICADOS_VISUALIZACION_COMPLETA"] = 0;
                Session["CERTIFICADOS_VISUALIZACION_PARCIAL"] = 0;
                Session["CERTIFICADOS_VISUALIZACION_ORGANIGRAMA"] = 0;

                List<LoginObtenerPermisos_Result> PermisosUsuario = dbe.LoginObtenerPermisos(Usuario).ToList();

                foreach (LoginObtenerPermisos_Result PerfilUsuario in PermisosUsuario)
                {
                    switch (PerfilUsuario.IdPerfil)
                    {
                        case (int)Perfiles.ROL.ADMINISTRADOR_SISTEMA:
                            Session["ADMINISTRADOR_SISTEMA"] = 1;
                            Session["ADMINISTRADOR"] = 1;
                            Session["SUPERVISOR_SERVICEDESK"] = 1;
                            Session["SUPERVISOR_OPERACIONES"] = 1;
                            Session["SUPERVISOR_PMO"] = 1;
                            Session["SUPERVISOR_SOPORTE"] = 1;
                            Session["SUPERVISOR_INVENTARIO"] = 1;
                            Session["SUPERVISOR_DOCUMENTOS"] = 1;
                            Session["SUPERVISOR_CONTROLDOCUMENTARIO"] = 1;
                            Session["SUPERVISOR_EJECUTIVOCOMERCIAL"] = 1;
                            Session["SUPERVISOR_RRHH"] = 1;
                            Session["SUPERVISOR_FINANZAS"] = 1;
                            Session["SERVICEDESK"] = 1;
                            Session["OPERACIONES"] = 1;
                            Session["PROJECTMANAGER"] = 1;
                            Session["SOPORTE"] = 1;
                            Session["INVENTARIO"] = 1;
                            Session["CONTROLDOCUMENTARIO"] = 1;
                            Session["EJECUTIVOCOMERCIAL"] = 1;
                            Session["EJECUTIVOCOMERCIAL_OUTSOURCING"] = 1;
                            Session["RRHH"] = 1;
                            Session["FINANZAS"] = 1;
                            Session["CMDB"] = 1;
                            Session["SISTEMA_INTEGRADO_GESTION"] = 1;
                            Session["PERMISO_LICENCIAS"] = 1;
                            Session["SUPERVISOR_OUTSOURCING"] = 1;
                            Session["APROBADOR_OPERACIONES"] = 1;
                            Session["RENOVACIONES"] = 1;
                            Session["SUPERVISOR_PMO_OUTSOURCING"] = 1;
                            Session["PROJECTMANAGER_OUTSOURCING"] = 1;
                            Session["PMO_GERENCIA"] = 1;
                            Session["PMO_GERENCIA_ITO"] = 1;
                            Session["ADMINPORTAL"] = 1;
                            Session["APROBAR_SELECCION_PERSONAL"] = 1;
                            Session["PMO_SOPORTE"] = 1;
                            Session["SUPERVISOR_INFORME"] = 1;
                            Session["INFORME"] = 1;
                            Session["COORDINADOR_SERVICEDESK_MINSUR"] = 1;
                            
                            Session["COORDINADOR_SERVICEDESK_BNV"] = 1;
                            Session["SUPERVISOR_GESTOR_ACTIVOS"] = 1;
                            Session["EJECUTOR_TAREAS_MINSUR_MARCOBRE_RAURA"] = 1;
                            Session["GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA"] = 1;
                            Session["GESTOR_ACTIVOS_MINSUR_MARCOBRE_RAURA"] = 1;
                            Session["SUPERVISOR_PMO_LISTADO"] = 1;
                            Session["GESTOR_DOCUMENTOS_ACTIVOS"] = 1;
                            Session["SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO"] = 1;
                            Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"] = 1;
                            Session["SUPERVISOR_PERMISO_LICENCIAS"] = 1;
                            Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"] = 1;

                            break;

                        case (int)Perfiles.ROL.ADMINISTRADOR:
                            Session["ADMINISTRADOR"] = 1;
                            Session["AUDITOR_TICKET"] = 1;
                            Session["AUDITOR_ACTIVIDADES"] = 1;
                            Session["AUDITOR_PMO"] = 1;
                            Session["AUDITOR_ACTIVO"] = 1;
                            Session["AUDITOR_INVENTARIO"] = 1;
                            Session["AUDITOR_DOCUMENTOS"] = 1;
                            Session["AUDITOR_CONTROLDOCUMENTOS"] = 1;
                            Session["AUDITOR_EJECUTIVOCOMERCIAL"] = 1;
                            Session["AUDITOR_TALENTO"] = 1;
                            Session["AUDITOR_FINANZAS"] = 1;
                            Session["CMDB"] = 1;
                            Session["APROBADOR_OPERACIONES"] = 1;
                            Session["APROBAR_SELECCION_PERSONAL"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_SERVICEDESK:
                            Session["SUPERVISOR_SERVICEDESK"] = 1;
                            Session["SERVICEDESK"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_OPERACIONES:
                            Session["SUPERVISOR_OPERACIONES"] = 1;
                            Session["OPERACIONES"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_PMO:
                            Session["SUPERVISOR_PMO"] = 1;
                            Session["PROJECTMANAGER"] = 1;
                            Session["USUARIO_PMO"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_SOPORTE:
                            Session["SUPERVISOR_SOPORTE"] = 1;
                            Session["SOPORTE"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_INVENTARIO:
                            Session["SUPERVISOR_INVENTARIO"] = 1;
                            Session["INVENTARIO"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_DOCUMENTOS:
                            Session["SUPERVISOR_DOCUMENTOS"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_CONTROLDOCUMENTARIO:
                            Session["SUPERVISOR_CONTROLDOCUMENTARIO"] = 1;
                            Session["CONTROLDOCUMENTARIO"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_EJECUTIVOCOMERCIAL:
                            Session["SUPERVISOR_EJECUTIVOCOMERCIAL"] = 1;
                            Session["EJECUTIVOCOMERCIAL"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_RRHH:
                            Session["SUPERVISOR_RRHH"] = 1;
                            Session["RRHH"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_FINANZAS:
                            Session["SUPERVISOR_FINANZAS"] = 1;
                            Session["FINANZAS"] = 1;
                            break;

                        case (int)Perfiles.ROL.SERVICEDESK:
                            Session["SERVICEDESK"] = 1;
                            break;

                        case (int)Perfiles.ROL.OPERACIONES:
                            Session["OPERACIONES"] = 1;
                            break;

                        case (int)Perfiles.ROL.PROJECTMANAGER:
                            Session["PROJECTMANAGER"] = 1;
                            break;

                        case (int)Perfiles.ROL.SOPORTE:
                            Session["SOPORTE"] = 1;
                            break;

                        case (int)Perfiles.ROL.INVENTARIO:
                            Session["INVENTARIO"] = 1;
                            break;

                        case (int)Perfiles.ROL.CONTROLDOCUMENTARIO:
                            Session["CONTROLDOCUMENTARIO"] = 1;
                            break;

                        case (int)Perfiles.ROL.EJECUTIVOCOMERCIAL:
                            Session["EJECUTIVOCOMERCIAL"] = 1;
                            break;
                        case (int)Perfiles.ROL.EJECUTIVOCOMERCIAL_OUTSOURCING:
                            Session["EJECUTIVOCOMERCIAL_OUTSOURCING"] = 1;
                            break;
                        case (int)Perfiles.ROL.RRHH:
                            Session["RRHH"] = 1;
                            break;

                        case (int)Perfiles.ROL.FINANZAS:
                            Session["FINANZAS"] = 1;
                            break;

                        case (int)Perfiles.ROL.USUARIO_PMO:
                            Session["USUARIO_PMO"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_TICKET:
                            Session["AUDITOR_TICKET"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_ACTIVIDADES:
                            Session["AUDITOR_ACTIVIDADES"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_PMO:
                            Session["AUDITOR_PMO"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_ACTIVO:
                            Session["AUDITOR_ACTIVO"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_INVENTARIO:
                            Session["AUDITOR_INVENTARIO"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_DOCUMENTOS:
                            Session["AUDITOR_DOCUMENTOS"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_CONTROLDOCUMENTOS:
                            Session["AUDITOR_CONTROLDOCUMENTOS"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_EJECUTIVOCOMERCIAL:
                            Session["AUDITOR_EJECUTIVOCOMERCIAL"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_TALENTO:
                            Session["AUDITOR_TALENTO"] = 1;
                            break;

                        case (int)Perfiles.ROL.AUDITOR_FINANZAS:
                            Session["AUDITOR_FINANZAS"] = 1;
                            break;

                        /*GESTION DEL CONOCIMIENTO GCR161203*/
                        case (int)Perfiles.ROL.APROBADOR_OPERACIONES:
                            Session["APROBADOR_OPERACIONES"] = 1;
                            break;
                        /*Fin*/

                        case (int)Perfiles.ROL.SISTEMA_INTEGRADO_GESTION:
                            Session["SISTEMA_INTEGRADO_GESTION"] = 1;
                            Session["CMDB"] = 1;
                            break;

                        case (int)Perfiles.ROL.CMDB:
                            Session["CMDB"] = 1;
                            break;

                        case (int)Perfiles.ROL.PERMISO_LICENCIAS:
                            Session["PERMISO_LICENCIAS"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_OUTSOURCING:
                            Session["SUPERVISOR_OUTSOURCING"] = 1;
                            break;

                        case (int)Perfiles.ROL.RENOVACIONES:
                            Session["RENOVACIONES"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_PMO_OUTSOURCING:
                            Session["SUPERVISOR_PMO_OUTSOURCING"] = 1;
                            Session["PROJECTMANAGER_OUTSOURCING"] = 1;
                            break;
                        case (int)Perfiles.ROL.PROJECTMANAGER_OUTSOURCING:
                            Session["PROJECTMANAGER_OUTSOURCING"] = 1;
                            break;

                        case (int)Perfiles.ROL.USUARIO_EXTERNO:
                            Session["USUARIO_EXTERNO"] = 1;
                            break;
                        case (int)Perfiles.ROL.PMO_GERENCIA:
                            Session["PMO_GERENCIA"] = 1;
                            break;
                        case (int)Perfiles.ROL.PMO_GERENCIA_ITO:
                            Session["PMO_GERENCIA_ITO"] = 1;
                            break;
                        case (int)Perfiles.ROL.ADMINPORTAL:
                            Session["ADMINPORTAL"] = 1;
                            break;
                        case (int)Perfiles.ROL.APROBAR_SELECCION_PERSONAL:
                            Session["APROBAR_SELECCION_PERSONAL"] = 1;
                            break;
                        case (int)Perfiles.ROL.USUARIO_EXTERNO_TICKET:
                            Session["USUARIO_EXTERNO_TICKET"] = 1;
                            break;
                        case (int)Perfiles.ROL.PMO_SOPORTE:
                            Session["PMO_SOPORTE"] = 1;
                            break;
                        case (int)Perfiles.ROL.DESCARGAR_LOGROS:
                            Session["DESCARGAR_LOGROS"] = 1;
                            Session["VISUALIZA_LOGROS"] = 1;
                            break;
                        case (int)Perfiles.ROL.VISUALIZA_LOGROS:
                            Session["VISUALIZA_LOGROS"] = 1;
                            break;
                        case (int)Perfiles.ROL.RENOVACIONES_OP:
                            Session["RENOVACIONES_OP"] = 1;
                            break;
                        case (int)Perfiles.ROL.SUPERVISOR_INFORME:
                            Session["SUPERVISOR_INFORME"] = 1;
                            Session["INFORME"] = 1;
                            break;
                        case (int)Perfiles.ROL.INFORME:
                            Session["INFORME"] = 1;
                            break;
                        case (int)Perfiles.ROL.COORDINADOR_SERVICEDESK_BNV:
                            Session["COORDINADOR_SERVICEDESK_BNV"] = 1;
                            break;
                        case (int)Perfiles.ROL.USUARIO_EXTERNO_TICKETS_ACTIVOS:
                            Session["USUARIO_EXTERNO_TICKETS_ACTIVOS"] = 1;
                            break;
                        case (int)Perfiles.ROL.COORDINADOR_SERVICEDESK_MINSUR:
                            Session["COORDINADOR_SERVICEDESK_MINSUR"] = 1;
                            break;
                        case (int)Perfiles.ROL.EJECUTOR_TAREAS_MINSUR_MARCOBRE_RAURA:
                            Session["EJECUTOR_TAREAS_MINSUR_MARCOBRE_RAURA"] = 1;
                            break;
                        case (int)Perfiles.ROL.GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA:
                            Session["GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA"] = 1;
                            break;
                        case (int)Perfiles.ROL.SUPERVISOR_GESTOR_ACTIVOS:
                            Session["SUPERVISOR_GESTOR_ACTIVOS"] = 1;
                            break;
                        case (int)Perfiles.ROL.GESTOR_ACTIVOS_MINSUR_MARCOBRE_RAURA:
                            Session["GESTOR_ACTIVOS_MINSUR_MARCOBRE_RAURA"] = 1;
                            break;
                        case (int)Perfiles.ROL.SUPERVISOR_PMO_LISTADO:
                            Session["SUPERVISOR_PMO_LISTADO"] = 1;
                            break;
                        case (int)Perfiles.ROL.GESTOR_DOCUMENTOS_ACTIVOS:
                            Session["GESTOR_DOCUMENTOS_ACTIVOS"] = 1;
                            break;
                        case (int)Perfiles.ROL.USUARIO_NOTIFICACIONES:
                            Session["USUARIO_NOTIFICACIONES"] = 1;
                            break;
                        case (int)Perfiles.ROL.GESTOR_ACTIVOS_BNV_MICROINFORMATICO:
                            Session["GESTOR_ACTIVOS_BNV_MICROINFORMATICO"] = 1;
                            break;
                        case (int)Perfiles.ROL.SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO:
                            Session["SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO"] = 1;
                            break;
                        case (int)Perfiles.ROL.GESTOR_ACTIVOS_BNV_INFRAESTRUCTURA:
                            Session["GESTOR_ACTIVOS_BNV_INFRAESTRUCTURA"] = 1;
                            break;
                        case (int)Perfiles.ROL.SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA:
                            Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"] = 1;
                            break;
                        case (int)Perfiles.ROL.SUPERVISOR_PERMISO_LICENCIAS:
                            Session["SUPERVISOR_PERMISO_LICENCIAS"] = 1;
                            break;
                        case (int)Perfiles.ROL.GESTOR_ACTIVOS_BNV_MOVIL:
                            Session["GESTOR_ACTIVOS_BNV_MOVIL"] = 1;
                            break;
                        case (int)Perfiles.ROL.SUPERVISOR_ACTIVOS_BNV_MOVIL:
                            Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"] = 1;
                            break;

                        case (int)Perfiles.ROL.SUPERVISOR_SERVICEDESK_ELECTRODATA:
                            Session["SUPERVISOR_SERVICEDESK_ELECTRODATA"] = 1;
                            break;
                        case (int)Perfiles.ROL.SUPERVISOR_SERVICEDESK_CUENTA:
                            Session["SUPERVISOR_SERVICEDESK_CUENTA"] = 1;
                            break;
                        case (int)Perfiles.ROL.GESTOR_ACTIVOS_HUDBAY_OT:
                            Session["GESTOR_ACTIVOS_HUDBAY_OT"] = 1;
                            break;
                        case (int)Perfiles.ROL.GESTOR_ACTIVOS_HUDBAY_IT:
                            Session["GESTOR_ACTIVOS_HUDBAY_IT"] = 1;
                            break;
                        case (int)Perfiles.ROL.ADMINISTRADOR_CATEGORIA:
                            Session["ADMINISTRADOR_CATEGORIA"] = 1;
                            break;
                        case (int)Perfiles.ROL.CERTIFICADOS_VISUALIZACION_COMPLETA:
                            Session["CERTIFICADOS_VISUALIZACION_COMPLETA"] = 1;
                            break;
                        case (int)Perfiles.ROL.CERTIFICADOS_VISUALIZACION_PARCIAL:
                            Session["CERTIFICADOS_VISUALIZACION_PARCIAL"] = 1;
                            break;
                        case (int)Perfiles.ROL.CERTIFICADOS_VISUALIZACION_ORGANIGRAMA:
                            Session["CERTIFICADOS_VISUALIZACION_ORGANIGRAMA"] = 1;
                            break;
                        default:
                            break;
                    }
                }
                //Session["IdIdioma"] = 1; // Id del idioma por defecto
                //Session["Idioma"] = "Español"; // Nombre del idioma por defecto
                var objUsuarioIdioma = dbe.Usuarios.Where(x => x.UserId == objUsuario.UserId).FirstOrDefault();

                var IdIdioma = objUsuarioIdioma.IdIdioma;
                if (IdIdioma == null)
                    IdIdioma = 1;
                Session["IdIdioma"] = IdIdioma;
                Session["Idioma"] = dbe.Idioma.Find(Session["IdIdioma"]).Nombre;

                //return RedirectToLocal("/");

                //Actualizar Idiomas
                XmlDocument xtemp = new XmlDocument();
                try
                {
                    xtemp.Load(System.AppDomain.CurrentDomain.BaseDirectory + "idiomas.xml");
                }
                catch (Exception)
                {
                    xtemp = null;
                }
                if (xtemp == null)
                {
                    List<db.ListaIdiomaDetalleXML_Result> lista = dbIdiomaDetalle.ListaIdiomaDetalleXML();
                    String file_path = System.AppDomain.CurrentDomain.BaseDirectory + "idiomas.xml";
                    XmlTextWriter writer;
                    writer = new XmlTextWriter(file_path, Encoding.UTF8);
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("idioma");
                    for (int i = 0; i < lista.Count(); i++)
                    {
                        writer.WriteElementString("id", Convert.ToString(lista.ToArray()[i].Id));
                        writer.WriteElementString("nombre", Convert.ToString(lista.ToArray()[i].Nombre));
                        writer.WriteElementString("llave", Convert.ToString(lista.ToArray()[i].Llave));
                        writer.WriteElementString("texto", Convert.ToString(lista.ToArray()[i].Texto));
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }

                return true;

            }
            catch (Exception ex)
            {
                // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
                ModelState.AddModelError("", "The username or password specified are incorrect");
                ViewBag.Error = "The Username or Password specified are incorrect";
                //return View();
                return false;
            }
        }

        public ActionResult cambioIdioma()
        {
            //Id de cambio de idioma
            int idIdioma = Convert.ToInt32(Request.Params["IdIdioma"]);
            Session["IdIdioma"] = idIdioma;
            //String llave = Convert.ToString(Request.Params["llave"]);
            arrResultado.Clear();
            
            for (int i = 0; i < arrIds.Count; i++)
            {
                if (Convert.ToInt32(arrIds.ToArray()[i]) == idIdioma)
                {
                    arrResultado.Add(Convert.ToString(arrTextos.ToArray()[i]) + "|" + Convert.ToString(arrLlaves.ToArray()[i]));
                }
            }
            return Json(new { Data = arrResultado }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult cambioIdiomaTexto()
        {
            //Id de cambio de idioma
            int idIdioma = Convert.ToInt32(Request.Params["IdIdioma"]);
            Session["IdIdioma"] = idIdioma;
            //String llave = Convert.ToString(Request.Params["llave"]);
            arrResultado.Clear();

            for (int i = 0; i < arrIds.Count; i++)//sw
            {
                if (Convert.ToInt32(arrIds.ToArray()[i]) == idIdioma)
                {
                    //obtiene los textos requeridos identificados por el array en curso
                    arrResultado.Add(Convert.ToString(arrTextos.ToArray()[i]) + "|" + Convert.ToString(arrLlaves.ToArray()[i]));//aqui pasa 1 id por palabras en 5+ idiomas * no va diccionario x duplicaion de bd
                }
            }
            return Json(new { Data = arrResultado }, JsonRequestBehavior.AllowGet);//pasa los textos requeridos segun idioma
        
        }

        public ActionResult cambioIdiomaxLlave()
        {
            //Id de cambio de idioma
            int idIdioma = Convert.ToInt32(Session["IdIdioma"]);

            string llave = Convert.ToString(Request.Params["llaves"]);
            string[] llaves = llave.Split('|');

            var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
                        select new
                        {
                            cadena = id.Texto + "|" + id.Llave
                        };

            arrResultado.Clear();
            var xxx = query.ToList();

            foreach (var cadena in xxx)
            {
                arrResultado.Add(cadena.cadena);
            }

            //for (int i = 0; i < arrIds.Count; i++)
            //{
            //    if (Convert.ToInt32(arrIds.ToArray()[i]) == idIdioma)
            //    {
            //        arrResultado.Add(Convert.ToString(arrTextos.ToArray()[i]) + "|" + Convert.ToString(arrLlaves.ToArray()[i]));
            //    }
            //}
            return Json(new { Data = arrResultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult cambioIdiomaxLlave2(string llave)
        {
            //Id de cambio de idioma
            int idIdioma = Convert.ToInt32(Session["IdIdioma"]);

            //llave = Convert.ToString(Request.Params["llaves"]);
            string[] llaves = llave.Split('|');

            var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
                        select new
                        {
                            //cadena = id.Texto + "|" + id.Llave
                            llave = id.Llave,
                            texto = id.Texto
                        };
            ViewBag.Asignado = "XXXX";
            //arrResultado.Clear();
            var xxx = query.ToList();

            //foreach (var cadena in xxx)
            //{
            //    arrResultado.Add(cadena.cadena);
            //}

            //for (int i = 0; i < arrIds.Count; i++)
            //{
            //    if (Convert.ToInt32(arrIds.ToArray()[i]) == idIdioma)
            //    {
            //        arrResultado.Add(Convert.ToString(arrTextos.ToArray()[i]) + "|" + Convert.ToString(arrLlaves.ToArray()[i]));
            //    }
            //}
            return Json(xxx, JsonRequestBehavior.AllowGet);
        }
        //ORIGINAL
        //public ActionResult cambioIdioma()
        //{
        //    //Id de cambio de idioma
        //    int idIdioma = Convert.ToInt32(Request.Params["IdIdioma"]);
        //    Session["IdIdioma"] = idIdioma;
        //    String llave = Convert.ToString(Request.Params["llave"]);

        //    for (int i = 0; i < arrIds.Count; i++)
        //    {
        //        if (Convert.ToString(arrLlaves.ToArray()[i]) == llave && Convert.ToInt32(arrIds.ToArray()[i]) == idIdioma)
        //        {
        //            arrResultado.Add(Convert.ToString(arrTextos.ToArray()[i]) + "|" + Convert.ToString(arrLlaves.ToArray()[i]));
        //        }
        //    }
        //    return Json(new { Data = arrResultado }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult cambioSessionIdioma()
        //{
        //    int idIdioma = Convert.ToInt32(Request.Params["cbxIdioma"]);
        //    int idUsuario = Convert.ToInt32(Session["IdUsuario"]);
        //    IEnumerable<db.CambioIdiomadeUsuario_Result> lista = dbIdioma.CambioIdiomadeUsuario(idUsuario, idIdioma);
        //    IEnumerable<db.CambioIdiomadeUsuario_Result> resultado = Mapper.Map<IEnumerable<db.CambioIdiomadeUsuario_Result>, IEnumerable<db.CambioIdiomadeUsuario_Result>>(lista);
        //    //Cambiar el idIdioma de la sesion
        //    Session["IdIdioma"] = idIdioma;
        //    //Session["Idioma"] = resultado.SingleOrDefault().Nombre;

        //    return Content(Convert.ToString(Session["IdIdioma"]));
        //}


        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public Boolean UsuarioActivo()
        {
            int usuario = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            if(usuario == 0)
            {
                return false;
            }
            return true;
        }

        //
        // GET: /Account/Register

        //[Authorize]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Intento de registrar al usuario
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Desasociar la cuenta solo si el usuario que ha iniciado sesión es el propietario
            if (ownerAccount == User.Identity.Name)
            {
                // Usar una transacción para evitar que el usuario elimine su última credencial de inicio de sesión
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        [HttpGet]
        [Authorize]
        public ActionResult Manage()
        {
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            var antiguaContrasena = Request.Params["OldPassword"];
            var nuevaContrasena = Request.Params["NewPassword"];
            var confirmaContrasena = Request.Params["ConfirmPassword"];
            try
            {
                if (nuevaContrasena != confirmaContrasena && nuevaContrasena != "")
                {
                    return Content("<script type = 'text/javascript'> function init(){" +
                        "if(top.uploadDoneNuevo) top.uploadDoneNuevo('Error', 'Las contraseñas no coinciden', '0');}}window.onload=init;</script>");
                }
                else
                {
                    string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$";
                    bool cumple = Regex.IsMatch(nuevaContrasena, pattern);
                    
                    if (cumple)
                    {
                        Criptografia cripto1 = new Criptografia();
                        int UserId = (int)Session["UserId"];

                        ObjectResult<int?> resultadoObjeto = dbe.ValidarContrasena(UserId, cripto1.Encriptar(nuevaContrasena), UserId);
                        int? resultadoNullable = resultadoObjeto.SingleOrDefault();
                        int flagHistorial = resultadoNullable ?? 0;

                        if (flagHistorial != 1)
                        {
                            var mensaje = "La contraseña ha sido utilizada anteriormente.";
                            return Content("<script type='text/javascript'>function init() {" +
                                        "if(top.uploadDoneNuevo){ top.uploadDoneNuevo('ERROR','" + mensaje + "', '0');}}window.onload=init;</script>");
                        }

                        
                        string Usuario1 = Session["UserName"].ToString();
                        int flag1 = (int)dbe.LoginCambioPassword(Usuario1, cripto1.Encriptar(nuevaContrasena), cripto1.Encriptar(antiguaContrasena)).SingleOrDefault();

                    }
                    else
                    {
                        var mensaje = "";
                        if (!Regex.IsMatch(nuevaContrasena, @"(?=.*[a-z])"))
                        {
                            mensaje = "La contraseña debe contener al menos una letra minúscula.";
                        }

                        // Check for at least one uppercase letter
                        else if (!Regex.IsMatch(nuevaContrasena, @"(?=.*[A-Z])"))
                        {
                            mensaje = "La contraseña debe contener al menos una letra mayúscula.";
                        }

                        // Check for at least one digit
                        else if (!Regex.IsMatch(nuevaContrasena, @"(?=.*\d)"))
                        {
                            mensaje = "La contraseña debe contener al menos un dígito.";
                        }

                        // Check for at least one special character
                        else if (!Regex.IsMatch(nuevaContrasena, @"(?=.*[^\da-zA-Z])"))
                        {
                            mensaje = "La contraseña debe contener al menos un carácter especial.";
                        }

                        // Check for a minimum length of 12 characters
                        else if (nuevaContrasena.Length < 12)
                        {
                            mensaje = "La contraseña debe tener una longitud mínima de 12 caracteres.";
                        }

                        return Content("<script type='text/javascript'>function init() {" +
                                        "if(top.uploadDoneNuevo){ top.uploadDoneNuevo('ERROR','" + mensaje + "', '0');}}window.onload=init;</script>");
                    }
                }
            }catch (Exception e)
            {
                return Content("<script type='text/javascript'>function init() {" +
                                        "if(top.uploadDoneNuevo){ top.uploadDoneNuevo('ERROR','" + e.ToString() + "', '0');}}window.onload=init;</script>");
            }


            return Content("<script type='text/javascript'>function init() {" +
                                "if(top.uploadDoneNuevo){ top.uploadDoneNuevo('OK','Se cambio la clave correctamente', '" + "0" + "');}}window.onload=init;</script>");
            //return RedirectToLocal("/Account/Manage/" + flag.ToString());
        }

        public ActionResult ManageClient(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "La contraseña se ha cambiado."
                : message == ManageMessageId.SetPasswordSuccess ? "Su contraseña se ha establecido."
                : message == ManageMessageId.RemoveLoginSuccess ? "El inicio de sesión externo se ha quitado."
                : "";
            //ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageClient(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword iniciará una excepción en lugar de devolver false en determinados escenarios de error.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "La contraseña actual es incorrecta o la nueva contraseña no es válida.");
                    }
                }
            }
            else
            {
                // El usuario no dispone de contraseña local, por lo que debe quitar todos los errores de validación generados por un
                // campo OldPassword vacío
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View();
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // Si el usuario actual ha iniciado sesión, agregue la cuenta nueva
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // El usuario es nuevo, solicitar nombres de pertenencia deseados
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insertar un nuevo usuario en la base de datos
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Comprobar si el usuario ya existe
                    if (user == null)
                    {
                        // Insertar el nombre en la tabla de perfiles
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "El nombre de usuario ya existe. Escriba un nombre de usuario diferente.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Aplicaciones auxiliares
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Vaya a http://go.microsoft.com/fwlink/?LinkID=177550 para
            // obtener una lista completa de códigos de estado.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El nombre de usuario ya existe. Escriba un nombre de usuario diferente.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Ya existe un nombre de usuario para esa dirección de correo electrónico. Escriba una dirección de correo electrónico diferente.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La contraseña especificada no es válida. Escriba un valor de contraseña válido.";

                case MembershipCreateStatus.InvalidEmail:
                    return "La dirección de correo electrónico especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "La respuesta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "La pregunta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidUserName:
                    return "El nombre de usuario especificado no es válido. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.ProviderError:
                    return "El proveedor de autenticación devolvió un error. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";

                case MembershipCreateStatus.UserRejected:
                    return "La solicitud de creación de usuario se ha cancelado. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";

                default:
                    return "Error desconocido. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";
            }
        }
        #endregion

        public ActionResult ViewAuthorization(int id, int id1, string fd = "")
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.Folder = fd;
            Session["ID_PERS_DOCU"] = id1;
            return View();
        }

        public ActionResult ValidateCredentials(LoginModel model, string returnUrl)
        {
            Criptografia cripto = new Criptografia();
            LoginValidar_Result obj = new LoginValidar_Result();
            obj = dbe.LoginValidar(model.UserName, cripto.Encriptar(model.Password)).SingleOrDefault();

            if (model.UserName == null || model.Password == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','0');}window.onload=init;</script>");
            }
            else if (ModelState.IsValid && obj != null)
            {

                //else if (ModelState.IsValid && Membership.ValidateUser(model.UserName, model.Password))
                //{

                int UserId = (int)obj.UserId; //WebSecurity.GetUserId(model.UserName);
                int UserIdLogin = (int)Session["UserId"];
                int ctd = 0;
                bool sw = false;

                if ((int)Session["RRHH"] == 1)
                {
                    ctd = 1;
                }

                int ID_PERS_ENTI = dbe.CLASS_ENTITY.Where(x => x.UserId == UserId)
                                    .Join(dbe.PERSON_ENTITY, x => x.ID_ENTI, pe => pe.ID_ENTI2, (x, pe) => new
                                    {
                                        pe.ID_PERS_ENTI,
                                    }).First().ID_PERS_ENTI;

                //string S_ID_PERS_ENTI = Convert.ToString(ID_PERS_ENTI);

                ////Determinando si el usuario es Admin del Talent(Puede descargar el documento)
                //int ctd = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 17 &&
                //            x.VAL_ACCO_PARA == S_ID_PERS_ENTI && x.VIG_ACCO_PARA == true).Count();

                //if (ctd == 0)
                //{
                //    //Determinando si el usuario es un Gerente (Puede descargar el documento)
                //    var query1 = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.VIG_CONT == true && x.LAS_CONT == true)
                //                 .Join(dbe.CARGO_AREA, x => x.ID_CARG_AREA, ca => ca.ID_CARG_AREA, (x, ca) => new
                //                 {
                //                     ca.ID_CARG,
                //                 })
                //                 .Join(dbe.CARGOes, x => x.ID_CARG, cg => cg.ID_CARG, (x, cg) => new
                //                 {
                //                     cg.MANAGEMENT,
                //                 });
                //    if (query1.Count() > 0)
                //    {
                //        if (query1.First().MANAGEMENT.HasValue)
                //        {
                //            sw = query1.First().MANAGEMENT.Value;
                //        }
                //    }
                //}
                //else { sw = true; }

                //if (sw == false)
                //{
                //    int ID_PERS_ENTI_FORM = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);
                //    if (ID_PERS_ENTI_FORM == ID_PERS_ENTI) { UserIdLogin = UserId; }
                //}

                if (ctd == 1 || UserId == UserIdLogin)
                {
                    string folder = Convert.ToString(Request.Form["Folder"]);

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('OK','OpenPDF','" + folder + "');}window.onload=init;</script>");
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.mensaje('ERROR','6');}window.onload=init;</script>");
                }
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.mensaje('ERROR','5');}window.onload=init;</script>");
            }
        }

        [AllowAnonymous]
        public ActionResult DisplayInitial(string returnUrl)
        {
            Session["MAIN"] = "mp1";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult DisplayInitial02(string returnUrl)
        {
            Session["MAIN"] = "mp1";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            if (lang == "en")
            {
                lang = "en-US";
            }
            else if (lang == "es")
            {
                lang = "es-PE";
            }
            Session["Culture"] = new CultureInfo(lang);
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public String DateTimeServer()
        {
            string mon = String.Format("{0:MMMM}", DateTime.Now);
            string ndia = String.Format("{0:dddd}", DateTime.Now);
            string dia = DateTime.Now.Day.ToString();
            string hh = DateTime.Now.Hour.ToString();
            string mn = DateTime.Now.Minute.ToString();
            string sc = DateTime.Now.Second.ToString();

            string fec = "";
            if (ResourceLanguaje.Resource.Idioma == "ESP")
            {
                fec = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
            }
            else { fec = String.Format("{0:M/d/yyyy}", DateTime.Now); }

            return dia + "|" + hh + "|" + mn + "|" + sc + "|" + mon + "|" + ndia + "|" + fec;
        }

        public ActionResult ListAccount()
        {
            var account = (from a in dbc.ACCOUNTs.Where(X => X.DES_ACCO != "X").ToList()
                           select new
                           {
                               a.ID_ACCO,
                               a.NAM_ACCO,
                               a.ACR_ACCO,
                               ENVIO = 0
                           }).OrderBy(x => x.NAM_ACCO);

            return Json(new { Data = account }, JsonRequestBehavior.AllowGet);
        }

        //Mantenimiento
        public ActionResult ListaTipoCuenta()
        {
            int ID_ACCO;

            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (FIELD == "ID_ACCO_TYPE")
            {
                ID_ACCO = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }
            else
            {
                ID_ACCO = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
            }

            var query = dbc.ACCOUNTs.Where(a => a.ID_ACCO_TYPE == ID_ACCO);

            var result = (from x in query.ToList()
                          orderby x.NAM_ACCO
                          select new
                          {
                              x.ID_ACCO,
                              x.NAM_ACCO
                          }).OrderBy(x => x.NAM_ACCO);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexCuenta()
        {
            return View();
        }

        public ActionResult ListaTipoCuentaCombo()
        {
            var query = (from at in dbc.ACCOUNT_TYPE select new { at.ID_ACCO_TYPE, at.NAM_ACCO_TYPE }).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreaCuenta()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreaCuenta(ACCOUNT objACCOUNT)
        {
            int flag = 0, existe = 0;

            String NombreTipoCuenta = Convert.ToString(Request.Params["ID_ACCOUNT_TYPE"]);

            if (String.IsNullOrEmpty(objACCOUNT.NAM_ACCO)) { flag = 1; }
            if (String.IsNullOrEmpty(objACCOUNT.ACR_ACCO)) { flag = 2; }
            if (String.IsNullOrEmpty(NombreTipoCuenta)) { flag = 3; }
            if (String.IsNullOrEmpty(objACCOUNT.DES_ACCO)) { flag = 4; }
            if (String.IsNullOrEmpty(objACCOUNT.SIT_ACCO)) { flag = 5; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            objACCOUNT.VIS_COMP = true;

            objACCOUNT.ID_ACCO_TYPE = Convert.ToInt32(NombreTipoCuenta.ToString());

            String NombreCuenta = objACCOUNT.NAM_ACCO;

            var result = (from at in dbc.ACCOUNTs.Where(x => x.NAM_ACCO == NombreCuenta && x.ID_ACCO_TYPE == objACCOUNT.ID_ACCO_TYPE).ToList()
                          select new { id = at.ID_ACCO_TYPE, marca = at.NAM_ACCO }).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                dbc.ACCOUNTs.Add(objACCOUNT);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objACCOUNT.ID_ACCO_TYPE.ToString() + "');}window.onload=init;</script>");
            }

            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }

        }

        public ActionResult ListarBuscarCuentaTodo(int i = 0)
        {
            String Nombre = Request.Params["NAM_ACCO"].ToString();
            int Tipo = 0;
            if (Request.Params["ID_ACCO_TYPE"] != null)
            {
                Tipo = Convert.ToInt32(Request.Params["ID_ACCO_TYPE"].ToString());
            }

            String Visibilidad = Convert.ToString(Request.Params["VIS_COMP"]);

            var query = dbc.ListarBuscarCuenta(Nombre, Tipo, Visibilidad).ToList();

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query2 = query.Skip(skip).Take(take);
            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditaCuenta(int Id = 0)
        {
            var c = dbc.ACCOUNTs.Find(Id);
            var t = dbc.ACCOUNT_TYPE.Find(c.ID_ACCO_TYPE);
            ViewBag.Nombre = c.NAM_ACCO;
            ViewBag.Acronimo = c.ACR_ACCO;
            //ViewBag.Tipo = t.NAM_ACCO_TYPE;
            ViewBag.IdTipo = t.ID_ACCO_TYPE;
            ViewBag.Descripcion = c.DES_ACCO;
            ViewBag.Ubicacion = c.SIT_ACCO;

            return View(c);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Editacuenta(ACCOUNT objACCO)
        {
            //Validar campos en blanco
            int flag = 0, existe = 0;

            if (Request.Params["ID_ACCOUNT_TYPE"].ToString() == null) { flag = 1; }
            else
            {
                objACCO.ID_ACCO_TYPE = Convert.ToInt32(Request.Params["ID_ACCOUNT_TYPE"]);
            }
            if (Convert.ToString(objACCO.NAM_ACCO) == null) { flag = 2; }
            if (Convert.ToString(objACCO.ACR_ACCO) == null) { flag = 3; }
            if (Convert.ToString(objACCO.DES_ACCO) == null) { flag = 4; }
            if (Convert.ToString(objACCO.SIT_ACCO) == null) { flag = 5; }
            if (Convert.ToString(objACCO.ID_ACCO_TYPE) == null) { flag = 6; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            int Codigo = objACCO.ID_ACCO;
            String NombreCuenta = objACCO.NAM_ACCO;
            var result = (from at in dbc.ACCOUNTs.Where(x => x.NAM_ACCO == NombreCuenta && x.ID_ACCO_TYPE == objACCO.ID_ACCO_TYPE
                                                                  && x.ID_ACCO != Codigo).ToList()
                          select new { id = at.ID_ACCO_TYPE, marca = at.NAM_ACCO }).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                ACCOUNT objRegistro = dbc.ACCOUNTs.Find(objACCO.ID_ACCO);
                objRegistro.NAM_ACCO = objACCO.NAM_ACCO;
                objRegistro.ACR_ACCO = objACCO.ACR_ACCO;
                objRegistro.ID_ACCO_TYPE = objACCO.ID_ACCO_TYPE;
                objRegistro.DES_ACCO = objACCO.DES_ACCO;
                objRegistro.SIT_ACCO = objACCO.SIT_ACCO;
                objRegistro.VIS_COMP = objACCO.VIS_COMP;
                objRegistro.ACR_ACCO = objACCO.ACR_ACCO;

                dbc.Entry(objRegistro).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objACCO.ID_ACCO.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }
    }
}
