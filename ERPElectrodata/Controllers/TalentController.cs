using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Web.Script.Serialization;

using System.Web.Helpers;
using ERPElectrodata.Objects;
using System.Globalization;
using System.Threading;

using WebMatrix.WebData;
using System.Configuration;
using System.Text.RegularExpressions;
using ERPElectrodata.Object.Plugins;
using ERPElectrodata.Object.Talent;

using Excel = Microsoft.Office.Interop.Excel;
using ClosedXML.Excel;

using System.Web.UI.WebControls;
using System.Text;
namespace ERPElectrodata.Controllers
{
    [Authorize]
    public class TalentController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        static int IdPersEnti = 0;
        static int ID_TYPE_DOCU = 0;
        static int ID_PERS_ENTI = 0;
        static int IdInstituto = 0;
        static int IdMarca = 0;
        static int IdGradoInstruccion = 0;
        static string NAM_ATTA;
        static string strFechaInicioVen = "";
        static string strFechaFinVen = "";
        //
        // GET: /Talent/
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                Session["MAIN"] = "mp4";

                ViewBag.Editar = 0;

                if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                {
                    ViewBag.VerWork = "1";
                }
                else
                {
                    ViewBag.VerWork = "0";
                }

                string ID_PERS_ENTI = Convert.ToString(Session["ID_PERS_ENTI"]);
                int ID_PE = Convert.ToInt32(ID_PERS_ENTI);
                int ID_CHAR = 0, ID_NAM_CHAR = 0, ID_PERS_CONT = 0;

                //Variable que permite visualizar los botones Edit y Contract
                ViewBag.Supervisor = 0;
                Session["Supervisor"] = ViewBag.Supervisor;

                //Variable que permite visualizar link para el registro de vacaciones
                ViewBag.AccessVacation = 0; //sin acceso
                ViewBag.AccessLoadBallots = 0; //sin acceso a cargar Boletas de Pago

                //Personal Administrador del Talent: RRHH
                int ctd = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 17 && x.VAL_ACCO_PARA == ID_PERS_ENTI && x.VIG_ACCO_PARA == true).Count();

                //x.ID_PARA == 18: CIA SUPPLY(Compania con el personal)
                int ID_ENTI = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true).First().VAL_ACCO_PARA);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                if (ctd == 0)
                {
                    //Obteniendo el Cargo del Usuario
                    var query1 = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PE && x.VIG_CONT == true && x.LAS_CONT == true);
                    if (query1.Count() > 0)
                    {
                        ID_CHAR = (query1.First().ID_CHAR.HasValue == true ? query1.First().ID_CHAR.Value : 0);
                        if (ID_CHAR != 0)
                        {
                            var query2 = dbe.CHARTs.Single(x => x.ID_CHAR == ID_CHAR);
                            ID_NAM_CHAR = query2.ID_NAM_CHAR.Value;
                        }
                        ID_PERS_CONT = query1.First().ID_PERS_CONT;
                    }

                    //Determinando si el usuario es un Gerente
                    if (ID_NAM_CHAR > 0)
                    {
                        var query3 = dbe.NAME_CHART.Single(x => x.ID_NAM_CHAR == ID_NAM_CHAR);
                        bool MN = (query3.MANAGEMENT == null ? false : Convert.ToBoolean(query3.MANAGEMENT));
                        if (MN) { ctd = 1; ViewBag.Supervisor = 1; }
                    }

                    //Determinando si el usuario tiene personal a cargo
                    if (ctd == 0)
                    {
                        //Obteniendo todos los cargos del personal segun el organigrama                 
                        var qPC_CH = dbe.PERSON_CONTRACT_CHART.Where(x => x.ID_PERS_CONT == ID_PERS_CONT);

                        var qr4 = (from a in dbe.CHARTs.ToList()// .Where(x => x.ID_CHAR_PARE == ID_CHAR)
                                   join qpc in qPC_CH on a.ID_CHAR_PARE equals qpc.ID_CHAR
                                   join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR into lb
                                   from xb in lb.DefaultIfEmpty()
                                   join c in dbe.CHARTs on a.ID_CHAR equals c.ID_CHAR_PARE into lc
                                   from xc in lc.DefaultIfEmpty()
                                   join d in dbe.NAME_CHART on (xc == null ? 0 : xc.ID_NAM_CHAR) equals d.ID_NAM_CHAR into ld
                                   from xd in ld.DefaultIfEmpty()
                                   select new
                                   {
                                       ID_CHAR = (xb.ID_TYPE_CHAR == 3 ? a.ID_CHAR : (xc == null ? 0 : xc.ID_CHAR)),
                                       ID_TYPE_CHAR = (xb.ID_TYPE_CHAR == 3 ? 3 : (xd == null ? 0 : xd.ID_TYPE_CHAR)),
                                   }).Where(x => x.ID_TYPE_CHAR == 3);

                        var query5 = dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true).ToList()
                                    .Join(qr4, x => x.ID_CHAR, q4 => q4.ID_CHAR, (x, q4) => new
                                    {
                                        x.ID_CHAR,
                                        x.ID_PERS_ENTI,
                                    });

                        //Si tiene personal a su cargo
                        if (query5.Count() > 0)
                        {
                            ctd = 1;
                            var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_ENTI).ToList()
                                         join q5 in query5 on pe.ID_PERS_ENTI equals q5.ID_PERS_ENTI
                                         join ae in dbe.ACCOUNT_ENTITY.Where(x => x.ID_ACCO == ID_ACCO) on
                                                 pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                                         select new
                                         {
                                             pe.ID_PERS_STAT,
                                             ae.VIS_TALE,
                                             pe.ID_ACCO_PERT
                                         });

                            query = query.Where(x => x.VIS_TALE == true);

                            if (ID_ACCO != 4)
                            {
                                query = query.Where(x => x.ID_ACCO_PERT == ID_ACCO);
                            }

                            ViewBag.ID_PERS_ENTI = ID_PERS_ENTI;
                            ViewBag.TPlanilla = query.Where(x => x.ID_PERS_STAT == 1).Count();
                            ViewBag.TPracticante = query.Where(x => x.ID_PERS_STAT == 2).Count();
                            ViewBag.TCesado = query.Where(x => x.ID_PERS_STAT == 3).Count();
                            ViewBag.TDisponible = query.Where(x => x.ID_PERS_STAT == 4).Count();

                            Session["Supervisor"] = ViewBag.Supervisor;

                            //Acceso al formulario de vacaciones                        
                            //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
                            //foreach (string rol in rolesArray)
                            //{
                            //    if (rol == "RRHH" || rol == "ADMINISTRADOR")
                            //    {
                            //        ViewBag.AccessVacation = 1; //permitiendo al acceso
                            //        ViewBag.AccessLoadBallots = 1; //permitiendo al acceso
                            //    }
                            //}
                            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["RRHH"] == 1)
                            {
                                ViewBag.Editar = 1;
                                ViewBag.AccessVacation = 1; //permitiendo al acceso
                                ViewBag.AccessLoadBallots = 1; //permitiendo al acceso
                            }

                            return View();
                        }
                    }
                }
                else { ViewBag.Supervisor = 1; }

                Session["Supervisor"] = ViewBag.Supervisor;

                if (ctd == 0)
                {
                    var query = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PE).ToList()
                                .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                                {
                                    ce.SEX_ENTI,
                                    x.ID_FOTO
                                }).First();

                    ViewBag.Manager = 0;
                    ViewBag.FOTO = query.ID_FOTO.ToString() + ".jpg";
                    ViewBag.ID_PERS_ENTI = ID_PERS_ENTI;
                    ViewBag.TitleProfile = ResourceLanguaje.Resource.Profile;

                    return View("Index1");
                }
                else
                {

                    var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_ENTI)
                                 join ae in dbe.ACCOUNT_ENTITY.Where(x => x.ID_ACCO == ID_ACCO) on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                                 select new
                                 {
                                     pe.ID_PERS_STAT,
                                     ae.VIS_TALE,
                                     pe.ID_ACCO_PERT
                                 }).Where(ae => ae.VIS_TALE == true);

                    if (ID_ACCO != 4)
                    {
                        query = query.Where(x => x.ID_ACCO_PERT == ID_ACCO);
                    }

                    ViewBag.ID_PERS_ENTI = ID_PERS_ENTI;
                    ViewBag.TPlanilla = query.Where(x => x.ID_PERS_STAT == 1).Count();
                    ViewBag.TPracticante = query.Where(x => x.ID_PERS_STAT == 2).Count();
                    ViewBag.TCesado = query.Where(x => x.ID_PERS_STAT == 3).Count();
                    ViewBag.TDisponible = query.Where(x => x.ID_PERS_STAT == 4).Count();

                    //Acceso al formulario de vacaciones                
                    //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
                    //foreach (string rol in rolesArray)
                    //{
                    //    if (rol == "RRHH" || rol == "ADMINISTRADOR")
                    //    {
                    //        ViewBag.AccessVacation = 1; //permitiendo al acceso a vacaciones
                    //        ViewBag.AccessLoadBallots = 1; //permitiendo al acceso a cargar Boletas
                    //    }
                    //}
                    if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["RRHH"] == 1)
                    {
                        ViewBag.Editar = 1;
                        ViewBag.AccessVacation = 1; //permitiendo al acceso
                        ViewBag.AccessLoadBallots = 1; //permitiendo al acceso
                    }

                    return View();
                }
            }
            catch
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




        #region "Listar Directorio"
        public ActionResult DirectorioTelefono()
        {
            ViewBag.IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            return View();
        }

        public ActionResult FindResulto(string area, string locacion)
        {

            try
            {
                // ... Tu lógica para obtener datos ...



                string Codigo = "";
                //string Usuario = Convert.ToString(Request.Params["ID_ENTI"].ToString());
                string Marca = "";
                string Sitio = "";
                string Estado = "";
                //string TipoActivo = Convert.ToString(Request.Params["ID_TYPE_ASSE"].ToString());
                string NombreEquipo = "";
                string Serie = "";
                //string SOLPED = Convert.ToString(Request.Params["SOLPED"].ToString());
                string TiposActivo = Request.Params["valTypeAsset"].ToString();
                string Usuarios = "";
                string Contratos = "";

                string NumeroFactura = "";
                string GuiaRemision = "";
                string OrdenCompra = "";



                string ModeloComercial = Convert.ToString(Request.Params["ID_COMM_MODE"] == null ? "0" : Request.Params["ID_COMM_MODE"]);
                string ModeloFabrica = Convert.ToString(Request.Params["ID_MANU_MODE"] == null ? "0" : Request.Params["ID_MANU_MODE"]);
                string Locacion = Convert.ToString(Request.Params["ID_LOCA"] == null ? "0" : Request.Params["ID_LOCA"]);
                string Condicion = "";
                string Mac = Convert.ToString(Request.Params["MAC_ADRR"] == null ? "" : Request.Params["MAC_ADRR"]);

                if (String.IsNullOrEmpty(Marca)) Marca = "0";
                if (String.IsNullOrEmpty(Sitio)) Sitio = "0";
                if (String.IsNullOrEmpty(Estado)) Estado = "0";
                if (String.IsNullOrEmpty(ModeloComercial)) ModeloComercial = "0";
                if (String.IsNullOrEmpty(ModeloFabrica)) ModeloFabrica = "0";
                if (String.IsNullOrEmpty(Locacion)) Locacion = "0";
                if (String.IsNullOrEmpty(Condicion)) Condicion = "0";

                int IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());


                //Selección múltiple
                if (String.IsNullOrEmpty(TiposActivo)) TiposActivo = "0";
                TiposActivo = TiposActivo.Substring(0, TiposActivo.Length - 1);
                if (String.IsNullOrEmpty(Usuarios)) Usuarios = "0";
                Usuarios = Usuarios.Substring(0, Usuarios.Length - 1);

                string grupos = "";
                string usuariosResp = "";
                string subTipo = "";
                // MINSUR / MARCOBRE / RAURA
                if (Convert.ToInt32(Session["ID_ACCO"]) == 56 || Convert.ToInt32(Session["ID_ACCO"]) == 57 || Convert.ToInt32(Session["ID_ACCO"]) == 58)
                {
                    grupos = Request.Params["IdGrupo"].ToString();
                    usuariosResp = Request.Params["UsuariosResp"].ToString();
                    subTipo = Request.Params["IdSubTipoActivo"].ToString();
                    if (String.IsNullOrEmpty(grupos)) grupos = "";
                    if (String.IsNullOrEmpty(subTipo)) subTipo = "";

                    if (String.IsNullOrEmpty(usuariosResp)) usuariosResp = "0";
                    usuariosResp = usuariosResp.Substring(0, usuariosResp.Length - 1);
                }
                if (String.IsNullOrEmpty(Contratos)) Contratos = "0";
                Contratos = Contratos.Substring(0, Contratos.Length - 1);


                var query = dbc.ActBuscar(Codigo, Usuarios, Contratos, TiposActivo, Marca, ModeloComercial, ModeloFabrica, Sitio, Locacion, Estado, Condicion, Serie, Mac, NombreEquipo, NumeroFactura, GuiaRemision, OrdenCompra, IdAcco, usuariosResp, grupos, subTipo)
     .Where(item => (string.IsNullOrEmpty(area) || item.Area == area)
    && (string.IsNullOrEmpty(locacion) || item.Locacion == locacion)
    && (item.TipoActivo == "CHIP" && item.Estado == "ASIGNADO"))
     .OrderBy(item => item.Marca)
     .ToList();

                var total = query.Count();



                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue; // Establece el límite a un valor grande.
                var serializedData = serializer.Serialize(new { Data = query, Count = total });

                return Content(serializedData, "application/json");
            }
            catch (Exception ex)
            {
                // Maneja errores según tus necesidades.
                return Content("Error: " + ex.Message, "text/plain");
            }



        }

        #endregion

        #region "FUNCIONAL"

        //CREACION DEL REPORTE TIKET

        [Authorize]
        public ActionResult ReporteDocUsuarios()
        {
            return View();
        }
        public ActionResult ListarTablaDocumentos_User()
        {
            int sw = 0, Creador = 0, TipoDocumento = 0, ID_PERS_STAT = 0;
            string msjError = string.Empty;
            DateTime? FechaInicio = null, FechaFin = null;
            if (int.TryParse(Convert.ToString(Request.Params["ID_ENTI"]), out Creador) == false)
            {
                Creador = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["ID_TYPE_DOCU"]), out TipoDocumento) == false)
            {
                TipoDocumento = 0;
            }

            if (Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"], cultures);
            }

            if (Convert.ToString(Request.QueryString["FechaIngresoFin"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoFin"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"], cultures);
            }


            if (int.TryParse(Convert.ToString(Request.Params["ID_PERS_STAT"]), out ID_PERS_STAT) == false)
            {
                ID_PERS_STAT = 0;
            }
            var result = dbe.ListarTablaDocumentos(Creador, TipoDocumento, FechaInicio, FechaFin, ID_PERS_STAT).ToList();

            //var result = dbe.ListarTablaDocumentos().ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult ListarTablaDocumentos()
        //{

        //    var Registros = dbe.ListarTablaDocumentos().ToList();

        //    return Json(new { Data = Registros }, JsonRequestBehavior.AllowGet);
        //}



        //CREACION DEL LLENADO DEL EXEL POR DOCUMENTOPS----


        public ActionResult ExportarListadocumento_Trabajadores()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int Creador = 0, TipoDocumento = 0, ID_PERS_STAT = 0, IdUsuario = 0;
            string msjError = string.Empty;
            DateTime? FechaInicio = null, FechaFin = null;



            if (int.TryParse(Convert.ToString(Request.Params["ID_ENTI"]), out Creador) == false)
            {
                Creador = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["ID_TYPE_DOCU"]), out TipoDocumento) == false)
            {
                TipoDocumento = 0;
            }

            if (Convert.ToString(Request.Params["FechaIngresoInicio"]) == "" || Convert.ToString(Request.Params["FechaIngresoInicio"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"], cultures);
            }

            if (Convert.ToString(Request.Params["FechaIngresoFin"]) == "" || Convert.ToString(Request.Params["FechaIngresoInicio"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"], cultures);
            }

            if (int.TryParse(Convert.ToString(Request.Params["ID_PERS_STAT"]), out ID_PERS_STAT) == false)
            {
                ID_PERS_STAT = 0;

            }


            List<ListarExportarExcelDocumentos_Result> query = dbe.ListarExportarExcelDocumentos(Creador, TipoDocumento, FechaInicio, FechaFin, ID_PERS_STAT).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:100px 80px;border-style:solid red 2px;border-width:1px;overflow:hidden;word-break:normal;border-color:#0000;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".cabecera{background:rgb(255, 153, 255);color:black;font-family:Arial, sans-serif;font-size:12px;font-weight:normal;padding:40px 100px;border:red solid 2px;border-width:1px;overflow:hidden;word-break:normal;border-color:#000000;border-top-width:2px;border-bottom-width:1px;}");
            sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#000000;border-top-width:2px;border-bottom-width:2px;background:#F7FDFA;margin:5px 10px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='cabecera'>NOMBRE</td>");
            sb.Append("<th class='cabecera'>NOMBRE ÁREA</td>");
            sb.Append("<th class='cabecera'>PUESTO</td>");
            sb.Append("<th class='cabecera'>FECHA DE INGRESO</td>");
            sb.Append("<th class='cabecera'>FICHA DE INGRESO (PDF)</td>");
            sb.Append("<th class='cabecera'>DNI o CE</td>");
            sb.Append("<th class='cabecera'>ANTECEDENTES PENALES</td>");
            sb.Append("<th class='cabecera'>ANTECEDENTES POLICIALES</td>");
            sb.Append("<th class='cabecera'>CERTIADULTO</td>");
            sb.Append("<th class='cabecera'>CERTIFICADO DOMICILIARIO</td>");
            sb.Append("<th class='cabecera'>RECIBO DE SERVICIOS DE LUZ, AGUA O TELÉFONO</td>");
            sb.Append("<th class='cabecera'>FOTO (PDF)</td>");
            sb.Append("<th class='cabecera'>DNI DE CÓNYUGE O CONVIVIENTE</td>");
            sb.Append("<th class='cabecera'>PARTIDA DE MATRIMONIO</td>");
            sb.Append("<th class='cabecera'>UNIÓN DE HECHO (EN CASO DE CONVIVIENTE)</td>");
            sb.Append("<th class='cabecera'>DNI DE HIJOS (MENORES DE 18 AÑOS)</td>");
            sb.Append("<th class='cabecera'>DNI DE HIJOS (MAYORES DE 18 AÑOS Y HASTA 24 AÑOS) SOLO EN EL CASO SE ENCUENTREN ESTUDIANDO ACTUALMENTE Y ADJUNTAR EL CERTIFICADO DE ESTUDIOS Y/O PAGOS REALIZADOS DEL CICLO ACTUAL</td>");
            sb.Append("<th class='cabecera'>CURRICULUM VITAE</td>");
            sb.Append("<th class='cabecera'>CURRICULUM VITAE  EDATA</td>");
            sb.Append("<th class='cabecera'>CONSTANCIA DE ESTUDIOS (Si actualmente tienes estudios técnicos o universitarios)</td>");
            sb.Append("<th class='cabecera'>CONSTANCIA DE EGRESADO (ESTUDIANTE UNIVERSITARIO)</td>");
            sb.Append("<th class='cabecera'>TÉCNICO TITULADO</td>");
            sb.Append("<th class='cabecera'>BACHILLER</td>");
            sb.Append("<th class='cabecera'>TÍTULO UNIVERSITARIO</td>");
            sb.Append("<th class='cabecera'>COLEGIATURA</td>");
            sb.Append("<th class='cabecera'>MAESTRÍA</td>");
            sb.Append("<th class='cabecera'>CERTIFICACIONES</td>");
            sb.Append("<th class='cabecera'>CURSOS</td>");
            sb.Append("<th class='cabecera'>DIPLOMADOS O ESPECIALIZACIONES</td>");
            sb.Append("<th class='cabecera'>CONSTANCIA DE TRABAJO</td>");
            sb.Append("<th class='cabecera'>REPORTE DE RENTAS DE 5TA SUNAT- OTROS EMPLEADORES</td>");
            sb.Append("<th class='cabecera'>CARGO DE ENTREGA DE RENTAS DE 5TA CATEGORÍA</td>");
            sb.Append("<th class='cabecera'>DECLARACIÓN JURADA DE 5TA CATEGORÍA</td>");
            sb.Append("<th class='cabecera'>DECLARACIÓN JURADA DE VIDA LEY</td>");
            sb.Append("<th class='cabecera'>LICENCIA DE MANEJO</td>");
            sb.Append("<th class='cabecera'>PASAPORTE</td>");
            sb.Append("<th class='cabecera'>CERTIFICACIONES TRADUCIDAS</td>");


            // no concuerdan datos

            sb.Append("<th class='cabecera'>CARTA DE OFERTA</td>");
            sb.Append("<th class='cabecera'>INFORMACIÓN DERECHOHABIENTES</td>");
            sb.Append("<th class='cabecera'>CONSTANCIA DE GRADO ACADÉMICO</td>");
            sb.Append("<th class='cabecera'>FORMATO EPS</td>");

            sb.Append("</tr>");

            foreach (ListarExportarExcelDocumentos_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.NombreColaborador + "</td>");
                sb.Append("<td class='contenido'>" + dr.NombreArea + "</td>");
                sb.Append("<td class='contenido'>" + dr.CargoEntrega5taCategoria + "</td>");
                sb.Append("<td class='contenido'>" + dr.Inicio + "</td>");
                sb.Append("<td class='contenido'>" + dr.FichaIngreso + "</td>");
                sb.Append("<td class='contenido'>" + dr.DNI + "</td>");
                sb.Append("<td class='contenido'>" + dr.AntecedentePenales + "</td>");
                sb.Append("<td class='contenido'>" + dr.AntecedentePoliciales + "</td>");
                sb.Append("<td class='contenido'>" + dr.Certiadulto + "</td>");
                sb.Append("<td class='contenido'>" + dr.CertificadoDomiciliario + "</td>");
                sb.Append("<td class='contenido'>" + dr.ReciboServicios + "</td>");
                sb.Append("<td class='contenido'>" + dr.Foto + "</td>");
                sb.Append("<td class='contenido'>" + dr.DNIConyugue + "</td>");
                sb.Append("<td class='contenido'>" + dr.PartidaMatrimonio + "</td>");
                sb.Append("<td class='contenido'>" + dr.EscrituraPublicaUnion + "</td>");
                sb.Append("<td class='contenido'>" + dr.DNIHijos18Menores + "</td>");
                sb.Append("<td class='contenido'>" + dr.DNIhijosDe18 + "</td>");
                sb.Append("<td class='contenido'>" + dr.CV + "</td>");
                sb.Append("<td class='contenido'>" + dr.CurriculumEdata + "</td>");
                sb.Append("<td class='contenido'>" + dr.EstudianteUniversitario + "</td>");
                sb.Append("<td class='contenido'>" + dr.ConstanciaEgresado + "</td>");
                sb.Append("<td class='contenido'>" + dr.TecnicoTitulado + "</td>");
                sb.Append("<td class='contenido'>" + dr.Bachiller + "</td>");
                sb.Append("<td class='contenido'>" + dr.TituloUniversitario + "</td>");
                sb.Append("<td class='contenido'>" + dr.Colegiatura + "</td>");
                sb.Append("<td class='contenido'>" + dr.Maestria + "</td>");
                sb.Append("<td class='contenido'>" + dr.Certificaciones + "</td>");
                sb.Append("<td class='contenido'>" + dr.Cursos + "</td>");
                sb.Append("<td class='contenido'>" + dr.DiplomadosEspecializaciones + "</td>");
                sb.Append("<td class='contenido'>" + dr.ConstanciaTrabajo + "</td>");
                sb.Append("<td class='contenido'>" + dr.ReporteSunat + "</td>");
                sb.Append("<td class='contenido'>" + dr.CargoEntrega5taCategoria + "</td>");
                sb.Append("<td class='contenido'>" + dr.DeclaracionJuradaCategoria + "</td>");
                sb.Append("<td class='contenido'>" + dr.DeclaraciondeVidaLey + "</td>");
                sb.Append("<td class='contenido'>" + dr.LicenciaManejo + "</td>");
                sb.Append("<td class='contenido'>" + dr.Pasaporte + "</td>");
                sb.Append("<td class='contenido'>" + dr.CertificacionesTraducidas + "</td>");

                //------
                sb.Append("<td class='contenido'>" + dr.CartaOferta + "</td>");
                sb.Append("<td class='contenido'>" + dr.InformacionDerechohabientes + "</td>");
                sb.Append("<td class='contenido'>" + dr.ConstanciaGradoAcademico + "</td>");
                sb.Append("<td class='contenido'>" + dr.FormatoEPS + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=listadocumentos" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }


        // ----FIN DE LA CREACION----

        public ActionResult CboListarEstadoPersona()
        {
            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);


            var result = dbe.CboListarEstadoPersona().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ListarCBOPersonalxDOC()
        {
            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);


            var result = dbe.ListarCBOPersonalxDOC().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ListarCBOTypexDOC()
        {
            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);


            var result = dbe.ListarCBOTypexDOC().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }



        // FIN DE CREACION DE REPORTE TIKET

        public ActionResult IndexNew()
        {
            try
            {
                Session["MAIN"] = "mp4";

                ViewBag.Editar = 0;

                if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                {
                    ViewBag.VerWork = "1";
                }
                else
                {
                    ViewBag.VerWork = "0";
                }

                string ID_PERS_ENTI = Convert.ToString(Session["ID_PERS_ENTI"]);
                int ID_PE = Convert.ToInt32(ID_PERS_ENTI);
                int ID_CHAR = 0, ID_NAM_CHAR = 0, ID_PERS_CONT = 0;

                //Variable que permite visualizar los botones Edit y Contract
                ViewBag.Supervisor = 0;
                Session["Supervisor"] = ViewBag.Supervisor;

                //Variable que permite visualizar link para el registro de vacaciones
                ViewBag.AccessVacation = 0; //sin acceso
                ViewBag.AccessLoadBallots = 0; //sin acceso a cargar Boletas de Pago

                //Personal Administrador del Talent: RRHH
                var q = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 17 && x.VAL_ACCO_PARA == ID_PERS_ENTI && x.VIG_ACCO_PARA == true);
                int ctd = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 17 && x.VAL_ACCO_PARA == ID_PERS_ENTI && x.VIG_ACCO_PARA == true).Count();

                //x.ID_PARA == 18: CIA SUPPLY(Compania con el personal)
                int ID_ENTI = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true).First().VAL_ACCO_PARA);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                if (ctd == 0)
                {
                    //Obteniendo el Cargo del Usuario
                    var query1 = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PE && x.VIG_CONT == true && x.LAS_CONT == true);
                    if (query1.Count() > 0)
                    {
                        ID_CHAR = (query1.First().ID_CHAR.HasValue == true ? query1.First().ID_CHAR.Value : 0);
                        if (ID_CHAR != 0)
                        {
                            var query2 = dbe.CHARTs.Single(x => x.ID_CHAR == ID_CHAR);
                            ID_NAM_CHAR = query2.ID_NAM_CHAR.Value;
                        }
                        ID_PERS_CONT = query1.First().ID_PERS_CONT;
                    }

                    //Determinando si el usuario es un Gerente
                    if (ID_NAM_CHAR > 0)
                    {
                        var query3 = dbe.NAME_CHART.Single(x => x.ID_NAM_CHAR == ID_NAM_CHAR);
                        bool MN = (query3.MANAGEMENT == null ? false : Convert.ToBoolean(query3.MANAGEMENT));
                        if (MN) { ctd = 1; ViewBag.Supervisor = 1; }
                    }

                    //Determinando si el usuario tiene personal a cargo
                    if (ctd == 0)
                    {
                        //Obteniendo todos los cargos del personal segun el organigrama                 
                        var qPC_CH = dbe.PERSON_CONTRACT_CHART.Where(x => x.ID_PERS_CONT == ID_PERS_CONT);

                        var qr4 = (from a in dbe.CHARTs.ToList()// .Where(x => x.ID_CHAR_PARE == ID_CHAR)
                                   join qpc in qPC_CH on a.ID_CHAR_PARE equals qpc.ID_CHAR
                                   join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR into lb
                                   from xb in lb.DefaultIfEmpty()
                                   join c in dbe.CHARTs on a.ID_CHAR equals c.ID_CHAR_PARE into lc
                                   from xc in lc.DefaultIfEmpty()
                                   join d in dbe.NAME_CHART on (xc == null ? 0 : xc.ID_NAM_CHAR) equals d.ID_NAM_CHAR into ld
                                   from xd in ld.DefaultIfEmpty()
                                   select new
                                   {
                                       ID_CHAR = (xb.ID_TYPE_CHAR == 3 ? a.ID_CHAR : (xc == null ? 0 : xc.ID_CHAR)),
                                       ID_TYPE_CHAR = (xb.ID_TYPE_CHAR == 3 ? 3 : (xd == null ? 0 : xd.ID_TYPE_CHAR)),
                                   }).Where(x => x.ID_TYPE_CHAR == 3);

                        var query5 = dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true).ToList()
                                    .Join(qr4, x => x.ID_CHAR, q4 => q4.ID_CHAR, (x, q4) => new
                                    {
                                        x.ID_CHAR,
                                        x.ID_PERS_ENTI,
                                    });

                        //Si tiene personal a su cargo
                        if (query5.Count() > 0)
                        {
                            ctd = 1;
                            var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_ENTI).ToList()
                                         join q5 in query5 on pe.ID_PERS_ENTI equals q5.ID_PERS_ENTI
                                         join ae in dbe.ACCOUNT_ENTITY.Where(x => x.ID_ACCO == ID_ACCO) on
                                                 pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                                         select new
                                         {
                                             pe.ID_PERS_STAT,
                                             ae.VIS_TALE,
                                             pe.ID_ACCO_PERT
                                         });

                            query = query.Where(x => x.VIS_TALE == true);

                            if (ID_ACCO != 4)
                            {
                                query = query.Where(x => x.ID_ACCO_PERT == ID_ACCO);
                            }

                            ViewBag.ID_PERS_ENTI = ID_PERS_ENTI;
                            ViewBag.TPlanilla = query.Where(x => x.ID_PERS_STAT == 1).Count();
                            ViewBag.TPracticante = query.Where(x => x.ID_PERS_STAT == 2).Count();
                            ViewBag.TCesado = query.Where(x => x.ID_PERS_STAT == 3).Count();
                            ViewBag.TDisponible = query.Where(x => x.ID_PERS_STAT == 4).Count();

                            Session["Supervisor"] = ViewBag.Supervisor;

                            //Acceso al formulario de vacaciones                        
                            //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
                            //foreach (string rol in rolesArray)
                            //{
                            //    if (rol == "RRHH" || rol == "ADMINISTRADOR")
                            //    {
                            //        ViewBag.AccessVacation = 1; //permitiendo al acceso
                            //        ViewBag.AccessLoadBallots = 1; //permitiendo al acceso
                            //    }
                            //}
                            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["RRHH"] == 1)
                            {
                                ViewBag.Editar = 1;
                                ViewBag.AccessVacation = 1; //permitiendo al acceso
                                ViewBag.AccessLoadBallots = 1; //permitiendo al acceso
                            }

                            return View();
                        }
                    }
                }
                else { ViewBag.Supervisor = 1; }

                Session["Supervisor"] = ViewBag.Supervisor;

                if (ctd == 0)
                {
                    var query = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PE).ToList()
                                .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                                {
                                    ce.SEX_ENTI,
                                    x.ID_FOTO
                                }).First();

                    ViewBag.Manager = 0;
                    ViewBag.FOTO = query.ID_FOTO.ToString() + ".jpg";
                    ViewBag.ID_PERS_ENTI = ID_PERS_ENTI;
                    ViewBag.TitleProfile = ResourceLanguaje.Resource.Profile;

                    return View("Index1New");
                }
                else
                {

                    var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_ENTI)
                                 join ae in dbe.ACCOUNT_ENTITY.Where(x => x.ID_ACCO == ID_ACCO) on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                                 select new
                                 {
                                     pe.ID_PERS_STAT,
                                     ae.VIS_TALE,
                                     pe.ID_ACCO_PERT
                                 }).Where(ae => ae.VIS_TALE == true);

                    if (ID_ACCO != 4)
                    {
                        query = query.Where(x => x.ID_ACCO_PERT == ID_ACCO);
                    }

                    ViewBag.ID_PERS_ENTI = ID_PERS_ENTI;
                    ViewBag.TPlanilla = query.Where(x => x.ID_PERS_STAT == 1).Count();
                    ViewBag.TPracticante = query.Where(x => x.ID_PERS_STAT == 2).Count();
                    ViewBag.TCesado = query.Where(x => x.ID_PERS_STAT == 3).Count();
                    ViewBag.TDisponible = query.Where(x => x.ID_PERS_STAT == 4).Count();

                    //Acceso al formulario de vacaciones                
                    //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
                    //foreach (string rol in rolesArray)
                    //{
                    //    if (rol == "RRHH" || rol == "ADMINISTRADOR")
                    //    {
                    //        ViewBag.AccessVacation = 1; //permitiendo al acceso a vacaciones
                    //        ViewBag.AccessLoadBallots = 1; //permitiendo al acceso a cargar Boletas
                    //    }
                    //}
                    if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["RRHH"] == 1)
                    {
                        ViewBag.Editar = 1;
                        ViewBag.AccessVacation = 1; //permitiendo al acceso
                        ViewBag.AccessLoadBallots = 1; //permitiendo al acceso
                    }

                    return View();
                }
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

                return RedirectToAction("IndexNew", "Talent");
            }
        }

        public ActionResult DataUser(int id = 0)
        {
            int ID_PERS_ENTI = id;

            var query = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                {
                    x.EMA_PERS,
                    x.EMA_ELEC,
                    x.EXT_PERS,
                    x.TEL_PERS,
                    x.CEL_PERS,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_STAT,
                    x.ID_CARG_AREA,
                    User = ce.FIR_NAME + " " + ce.SEC_NAME + " " + ce.LAS_NAME,
                    ce.MOT_NAME,
                    ce.EMA_ENTI,
                })
                .Join(dbe.CARGO_AREA, x => x.ID_CARG_AREA, ca => ca.ID_CARG_AREA, (x, ca) => new
                {
                    x.User,
                    x.EMA_ENTI,
                    x.EMA_PERS,
                    x.EMA_ELEC,
                    x.EXT_PERS,
                    x.TEL_PERS,
                    x.CEL_PERS,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_STAT,
                    x.ID_CARG_AREA,
                    x.MOT_NAME,
                    ca.ID_CARG,
                    ca.ID_AREA,
                })
                .Join(dbe.CARGOes, x => x.ID_CARG, c => c.ID_CARG, (x, c) => new
                {
                    x.User,
                    x.EMA_ENTI,
                    x.EMA_PERS,
                    x.EMA_ELEC,
                    x.EXT_PERS,
                    x.TEL_PERS,
                    x.CEL_PERS,
                    x.ID_PERS_ENTI,
                    x.ID_AREA,
                    x.ID_PERS_STAT,
                    x.MOT_NAME,
                    c.NAM_CARG,
                })
                .Join(dbe.AREAs, x => x.ID_AREA, a => a.ID_AREA, (x, a) => new
                {
                    x.User,
                    x.NAM_CARG,
                    x.EMA_PERS,
                    x.EXT_PERS,
                    x.TEL_PERS,
                    x.CEL_PERS,
                    x.ID_PERS_ENTI,
                    x.ID_AREA,
                    x.ID_PERS_STAT,
                    x.EMA_ELEC,
                    x.EMA_ENTI,
                    x.MOT_NAME,
                    a.NOM_AREA
                })
                .Join(dbe.PERSON_STATUS, x => x.ID_PERS_STAT, s => s.ID_PERS_STAT, (x, s) => new
                {
                    x.User,
                    x.NAM_CARG,
                    x.EMA_PERS,
                    x.EXT_PERS,
                    x.TEL_PERS,
                    x.CEL_PERS,
                    x.ID_PERS_ENTI,
                    x.ID_AREA,
                    x.ID_PERS_STAT,
                    x.NOM_AREA,
                    x.EMA_ELEC,
                    x.EMA_ENTI,
                    x.MOT_NAME,
                    s.NAM_STAT
                });

            var query1 = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.LAS_CONT == true)
                .Join(dbe.CONTRACT_CONDITION, x => x.ID_COND_CONT, c => c.ID_COND_CONT, (x, c) => new
                {
                    c.CONDITION,
                    x.STAR_DATE,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_CONT,
                    x.ID_WORK_PERI,
                })
                .Join(dbe.WORK_PERIOD, x => x.ID_WORK_PERI, wp => wp.ID_WORK_PERI, (x, wp) => new
                {
                    x.CONDITION,
                    x.STAR_DATE,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_CONT,
                    x.ID_WORK_PERI,
                    wp.CESS_DATE,
                    STAR_PERI = wp.STAR_DATE,
                });

            var query2 = dbe.PERSON_LOCATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.END_DATE == null)
                .Join(dbe.LOCATION_ENTITY, x => x.ID_LOCA, l => l.ID_LOCA, (x, l) => new
                {
                    l.NAM_LOCA,
                    l.ID_SITE,
                    STAR_LOCA = x.STAR_DATE,
                    END_LOCA = x.END_DATE,
                    ID_PERS_ENTI
                })
                .Join(dbe.SITE_ENTITY, x => x.ID_SITE, s => s.ID_SITE, (x, s) => new
                {
                    x.NAM_LOCA,
                    x.ID_SITE,
                    x.STAR_LOCA,
                    x.END_LOCA,
                    x.ID_PERS_ENTI,
                    s.NAM_SITE,
                });

            var result = (from q in query.ToList()
                          join q1 in query1 on q.ID_PERS_ENTI equals (q1 == null ? 0 : q1.ID_PERS_ENTI) into lq1
                          from xq1 in lq1.DefaultIfEmpty()
                          join q2 in query2 on q.ID_PERS_ENTI equals (q2 == null ? 0 : q2.ID_PERS_ENTI) into lq2
                          from xq2 in lq2.DefaultIfEmpty()
                          select new
                          {
                              User = q.User + (q.MOT_NAME == null ? "" :
                                                  (q.MOT_NAME.Trim().Length == 0 ? "" : " " + q.MOT_NAME.Substring(0, 1).ToUpper() + ".")),
                              CAR_PERS = (q.NAM_CARG == null ? "-" : q.NAM_CARG.ToLower()),
                              Ubicacion = (xq2 == null ? "-" : (xq2.NAM_SITE + " / " + xq2.NAM_LOCA).ToLower()),
                              EMA_PERS = (q.EMA_PERS == null ? "-" : (q.EMA_PERS.Trim().Length == 0 ? "-" : q.EMA_PERS.ToLower())),
                              EMA_ELEC = (q.EMA_ELEC == null ? "-" : (q.EMA_ELEC.Trim().Length == 0 ? "-" : q.EMA_ELEC.ToLower())),
                              EMA_ENTI = (q.EMA_ENTI == null ? "-" : (q.EMA_ENTI.Trim().Length == 0 ? "-" : q.EMA_ENTI.ToLower())),
                              NAM_STAT = q.NAM_STAT.ToLower(),
                              NOM_AREA = q.NOM_AREA.ToLower(),
                              CEL_PERS = (q.CEL_PERS == null ? "-" : q.CEL_PERS),
                              EXT_PERS = (q.EXT_PERS == null ? "-" : q.EXT_PERS),
                              STAR_PERI = (xq1 == null ? "-" : String.Format("{0:M/d/yyyy}", xq1.STAR_PERI)),
                              CONDITION = (xq1 == null ? "-" : xq1.CONDITION.ToLower()),
                              CESS_PER = (xq1 == null ? "-" : (xq1.CESS_DATE == null ? "-" : String.Format("{0:M/d/yyyy}", xq1.CESS_DATE))),
                              Tiempo = TimeWork(ID_PERS_ENTI),
                              USB = USB(Convert.ToInt32(q.ID_AREA))
                          });

            var resultDocu = (from pd in dbe.PERSON_DOCUMENTS.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                              join d in dbe.TYPE_DOCUMENT on pd.ID_TYPE_DOCU equals d.ID_TYPE_DOCU
                              where d.REGISTER == -1
                              select new
                              {
                                  pd.NAM_ATTA,
                                  pd.EXT_ATTA,
                                  pd.ID_PERS_DOCU,
                                  d.NAM_DOCU,
                                  d.ID_TYPE_DOCU,
                                  d.REGISTER,
                              }).OrderBy(x => x.ID_TYPE_DOCU);

            return Json(new { data = result, data1 = resultDocu, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public string TimeWork(int ID_PERS_ENTI/*,DateTime date1, DateTime date2*/)
        {
            string tempo = "";
            ////DateTime date2 = DateTime.Today;
            //int daysDiff = ((TimeSpan)(date2 - date1)).Days;
            //int anio = 0, mes = 0;
            //while(daysDiff > 360){
            //    anio = anio + 1;
            //    daysDiff = daysDiff - 360;
            //}
            //while (daysDiff > 30)
            //{
            //    mes = mes + 1;
            //    daysDiff = daysDiff - 30;
            //}
            //if (anio > 0) { tempo = anio.ToString() + " Año" + (anio == 1 ? " " : "s "); }
            //if (mes > 0) { tempo = tempo + mes.ToString() + " Mes" + (mes == 1 ? " " : "es "); }
            //tempo = tempo + daysDiff.ToString() + " Día" + (daysDiff == 1 ? " " : "s");
            try
            {
                var query = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                   .Where(x => x.VIG_CONT == true);

                var result = (from c in query.ToList()
                              join pl in dbe.WORK_PERIOD on c.ID_WORK_PERI equals pl.ID_WORK_PERI
                              select new
                              {
                                  STAR_DATE = String.Format("{0:d}", pl.STAR_DATE),
                                  TOTAL_YEAR = pl.TOTAL_YEAR == 0 ? "" : Convert.ToString(pl.TOTAL_YEAR) + (pl.TOTAL_YEAR == 1 ? " Year" : " Years"),
                                  TOTAL_MONTH = pl.TOTAL_MONTH == 0 ? "" : Convert.ToString(pl.TOTAL_MONTH) + (pl.TOTAL_MONTH == 1 ? " Month" : " Months"),
                                  TOTAL_DAY = pl.TOTAL_DAY == 0 ? "" : Convert.ToString(pl.TOTAL_DAY) + (pl.TOTAL_DAY == 1 ? " Day" : " Days")
                              }).First();

                //ViewBag.STAR_DATE = result.STAR_DATE;
                tempo = result.TOTAL_YEAR + " " + result.TOTAL_MONTH + " " + result.TOTAL_DAY;
            }
            catch
            {

            }


            return tempo;
        }

        public string USB(int id)
        {
            string usb = "";
            try
            {
                var query = dbe.AREAs.Where(x => x.ID_AREA == id).First();
                if (query.NIV_AREA == 2)
                {
                    usb = query.NOM_AREA;
                }
                else if (query.NIV_AREA == 3)
                {
                    var query1 = dbe.AREAs.Where(x => x.ID_AREA == query.ID_AREA_PARENT).First();
                    usb = query1.NOM_AREA;
                }
                else if (query.NIV_AREA == 4)
                {
                    var query1 = dbe.AREAs.Where(x => x.ID_AREA == query.ID_AREA_PARENT)
                                .Join(dbe.AREAs, x => x.ID_AREA_PARENT, y => y.ID_AREA, (x, y) => new
                                {
                                    y.NOM_AREA
                                }).First();

                    usb = query1.NOM_AREA;
                }
            }
            catch { }
            if (usb.Trim().Length == 0) { usb = "-"; }
            return usb.ToLower();
        }

        public ActionResult TreeInvoice(int id = 0)
        {
            int ID_PERS_ENTI = id;
            var query1 = dbe.PERSON_INVOICE.Where(c => c.ID_PERS_ENTI == ID_PERS_ENTI);
            var query2 = dbc.ACCOUNTING_MONTH;
            int anio = DateTime.Now.Year;
            int mes = DateTime.Now.Month;
            try
            {
                int ID_PARA = Convert.ToInt32(Request.Params["ID_PARA"]);
                if (ID_PARA == 0) //Carga los años
                {
                    var query3 = (from q1 in query1.ToList()
                                  join q2 in query2 on q1.ID_ACCO_MONT equals q2.ID_ACCO_MONT
                                  select new
                                  {
                                      ID_PARA = q2.ID_ACCO_YEAR,
                                      NAME_PARA = Convert.ToString(q2.ID_ACCO_YEAR),
                                      HAS_VALUE = true,
                                      VAL_PARA = Convert.ToString(q2.ID_ACCO_YEAR),
                                      expanded = (anio - 1 <= q2.ID_ACCO_YEAR ? true : false)
                                  }).Distinct().ToList();

                    return Json(query3.OrderByDescending(x => x.ID_PARA), JsonRequestBehavior.AllowGet);
                }
                else if (ID_PARA < 0) //Carga los documentos
                {
                    ID_PARA = ID_PARA * -1;
                    var query3 = (from q1 in query1.ToList()
                                  join q2 in query2 on q1.ID_ACCO_MONT equals q2.ID_ACCO_MONT
                                  where q2.ID_ACCO_MONT == ID_PARA
                                  select new
                                  {
                                      ID_PARA = q2.ID_ACCO_MONT,
                                      NAME_PARA = Convert.ToString(q1.NAM_ATTA),
                                      HAS_VALUE = false,
                                      SIGNED = q1.SIGNED,
                                      NAM_ATTA = q1.NAM_ATTA + q1.EXT_ATTA,
                                      NAM_FILE = q1.NAM_ATTA + "_" + Convert.ToString(q1.ID_INVO) + q1.EXT_ATTA,
                                      ID_FILE = q1.ID_INVO,
                                      ANIO_MES = Convert.ToInt32(Convert.ToString(q2.ID_ACCO_YEAR) +
                                                (q2.ACCO_MONT < 10 ? "0" : "") + Convert.ToString(q2.ACCO_MONT)),
                                  }).Distinct().OrderBy(x => x.SIGNED).ToList();
                    return Json(query3, JsonRequestBehavior.AllowGet);
                }
                else
                { //Carga los meses
                    var query3 = (from q1 in query1.ToList()
                                  join q2 in query2 on q1.ID_ACCO_MONT equals q2.ID_ACCO_MONT
                                  where q2.ID_ACCO_YEAR == ID_PARA
                                  select new
                                  {
                                      ID_PARA = q2.ID_ACCO_MONT * -1,
                                      NAME_PARA = Convert.ToString(q2.NAM_ACCO_MONT),
                                      HAS_VALUE = true,
                                      expanded = ExpandedMes(anio, mes, Convert.ToInt32(q2.ID_ACCO_YEAR), Convert.ToInt32(q2.ACCO_MONT), ID_PERS_ENTI, q2.ID_ACCO_MONT),
                                      q2.ACCO_MONT
                                  }).Distinct().ToList();
                    return Json(query3.OrderByDescending(x => x.ACCO_MONT), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                var query3 = (from q1 in query1.ToList()
                              join q2 in query2 on q1.ID_ACCO_MONT equals q2.ID_ACCO_MONT
                              select new
                              {
                                  ID_PARA = q2.ID_ACCO_YEAR,
                                  NAME_PARA = Convert.ToString(q2.ID_ACCO_YEAR),
                                  HAS_VALUE = true,
                                  VAL_PARA = Convert.ToString(q2.ID_ACCO_YEAR),
                                  expanded = (anio - 1 <= q2.ID_ACCO_YEAR ? true : false)
                              }).Distinct().ToList();

                return Json(query3.OrderBy(x => x.ID_PARA), JsonRequestBehavior.AllowGet);
            }
        }

        public bool ExpandedMes(int anioA, int mesA, int anio, int mes, int ID_PERS_ENTI, int ID_ACCO_MONT)
        {
            bool exp = false;
            if (anioA - 1 <= anio)
            {
                //Revisamos
                if (anioA == anio)
                {
                    int ctd = dbe.PERSON_INVOICE.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.ID_ACCO_MONT == ID_ACCO_MONT && x.SIGNED == true).Count();
                    if (ctd == 0) { exp = true; }
                }
                else if (mesA <= mes)
                {
                    int ctd = dbe.PERSON_INVOICE.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.ID_ACCO_MONT == ID_ACCO_MONT && x.SIGNED == true).Count();
                    if (ctd == 0) { exp = true; }
                }
            }
            return exp;
        }

        public int CountDocsByTypeDocs(int id = 0, int id1 = 0)
        {

            int con = 0;
            con = dbe.DOCUMENTS.Where(x => x.ID_TYPE_DOCU == id).Count();
            if (con == 0)
            {
                con = dbe.PERSON_DOCUMENTS.Where(x => x.ID_TYPE_DOCU == id && x.ID_PERS_ENTI == id1).Count();
            }
            return con;
        }

        public int CountDocumentsByIdTypeDoc(int id = 0)
        {
            int con = 0;
            con = dbe.DOCUMENTS.Where(x => x.ID_TYPE_DOCU == id).Count();
            return con;
        }

        public ActionResult TreeDocs(int id = 0)
        {
            try
            {
                int ID_PARA = Convert.ToInt32(Request.Params["ID_PARA"]);
                if (ID_PARA == 0) //Carga los tipos de documentos
                {
                    var query = (from q in dbe.TYPE_DOCUMENT.ToList()
                                 where q.REGISTER == 0 && q.VIG_DOCU == true
                                 select new
                                 {
                                     ID_PARA = q.ID_TYPE_DOCU,
                                     NAME_PARA = q.NAM_DOCU,
                                     HAS_VALUE = (q.ID_TYPE_DOCU == 8 ? true : (CountDocsByTypeDocs(q.ID_TYPE_DOCU, id) > 0 ? true : false)),
                                     VAL_PARA = q.NAM_DOCU,
                                     expanded = false,
                                     ATTACH = false,
                                     q.ORDEN
                                 });
                    var qq = query.Count();
                    return Json(query.OrderBy(x => x.ORDEN), JsonRequestBehavior.AllowGet);
                }
                else
                { //Carga los documentos
                    if (ID_PARA == 8) //Caso Job Description
                    {
                        var query = (from a in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == id && x.LAS_CONT == true).ToList()
                                     join b in dbe.CHARTs on a.ID_CHAR equals b.ID_CHAR
                                     join c in dbe.NAME_CHART on b.ID_NAM_CHAR equals c.ID_NAM_CHAR
                                     select new
                                     {
                                         ID_PARA = id,
                                         NAME_PARA = "Job Description - " + CapitalizeCadena(c.NAM_CHAR),
                                         NAM_ATTA = "Job Description - " + CapitalizeCadena(c.NAM_CHAR),
                                         HAS_VALUE = false,
                                         expanded = false,
                                         ATTACH = true,
                                         ID_TYPE_DOCU = 8,
                                         NAM_FILE = "/Talent/VerDocumento/2/" + "Job Description - " + CapitalizeCadena(c.NAM_CHAR) + ".pdf",
                                         //"Job Description - " + CapitalizeCadena(c.NAM_CHAR) + ".pdf",
                                         STAR_DATE = "",
                                         END_DATE = "",
                                         CONT = 0,
                                     });


                        return Json(query, JsonRequestBehavior.AllowGet);
                    }
                    else if (CountDocumentsByIdTypeDoc(ID_PARA) > 0)
                    { //Caso Template y Reglamento Interno

                        var query = (from d in dbe.DOCUMENTS.Where(x => x.ID_TYPE_DOCU == ID_PARA)
                                     select new
                                     {
                                         ID_PARA = d.ID_DOCU,
                                         NAME_PARA = d.NAM_FILE,
                                         NAM_ATTA = d.NAM_FILE,
                                         HAS_VALUE = false,
                                         expanded = false,
                                         ATTACH = true,
                                         ID_TYPE_DOCU = d.ID_TYPE_DOCU,
                                         NAM_FILE = (d.EXT_FILE).ToLower() == ".pdf" || (d.EXT_FILE).ToLower() == ".png" ||
                                            (d.EXT_FILE).ToLower() == ".jpg" || (d.EXT_FILE).ToLower() == ".txt"
                                            ? "/Talent/VerDocumento/1/" + d.NAM_FILE + d.EXT_FILE : "/Talent/DescargarArchivo/1/" + d.NAM_FILE + d.EXT_FILE,
                                         //d.NAM_FILE + d.EXT_FILE,
                                         STAR_DATE = "",
                                         END_DATE = "",
                                         CONT = 1,
                                         ICONO = (d.EXT_FILE == ".doc" || d.EXT_FILE == ".docx" ? "doc.png" : "pdf.png"),
                                     });

                        return Json(query, JsonRequestBehavior.AllowGet);
                    }
                    else //Resto de casos
                    {
                        var query = (from q in dbe.PERSON_DOCUMENTS.ToList()
                                     where q.ID_TYPE_DOCU == ID_PARA && q.ID_PERS_ENTI == id
                                     join c in dbe.PERSON_CONTRACT on q.ID_PERS_CONT equals c.ID_PERS_CONT into lc
                                     from xc in lc.DefaultIfEmpty()
                                     select new
                                     {
                                         ID_PARA = q.ID_PERS_DOCU,
                                         NAME_PARA = q.NAM_ATTA,
                                         NAM_ATTA = q.NAM_ATTA + q.EXT_ATTA,
                                         HAS_VALUE = false,
                                         expanded = false,
                                         ATTACH = true,
                                         ID_TYPE_DOCU = q.ID_TYPE_DOCU,
                                         NAM_FILE = (q.EXT_ATTA).ToLower() == ".pdf" || (q.EXT_ATTA).ToLower() == ".png" ||
                                                (q.EXT_ATTA).ToLower() == ".jpg" || (q.EXT_ATTA).ToLower() == ".txt"
                                                ? "/Talent/VerDocumento/1/" + q.NAM_ATTA + "_" + Convert.ToString(q.ID_PERS_DOCU) + q.EXT_ATTA : "/Talent/DescargarArchivo/1/" + q.NAM_ATTA + "_" + Convert.ToString(q.ID_PERS_DOCU) + q.EXT_ATTA,
                                         //q.NAM_ATTA + "_" + Convert.ToString(q.ID_PERS_DOCU) + q.EXT_ATTA,
                                         STAR_DATE = (xc == null ? "" : String.Format("{0:M/d/yyyy}", xc.STAR_DATE)),
                                         END_DATE = (xc == null ? "" : String.Format("{0:M/d/yyyy}", xc.END_DATE)),
                                         ORD_DATE = (xc == null ? Convert.ToDateTime("01-01-90") : xc.STAR_DATE),
                                         CONT = 0,
                                     });
                        return Json(query.OrderByDescending(x => x.ORD_DATE).OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception)
            {
                var query = (from q in dbe.TYPE_DOCUMENT.ToList()
                             where q.REGISTER == 0 && q.VIG_DOCU == true
                             select new
                             {
                                 ID_PARA = q.ID_TYPE_DOCU,
                                 NAME_PARA = q.NAM_DOCU,
                                 HAS_VALUE = (CountDocsByTypeDocs(q.ID_TYPE_DOCU, id) > 0 ? true : false),
                                 VAL_PARA = q.NAM_DOCU,
                                 expanded = false,
                                 ATTACH = false,
                             });

                return Json(query.OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EliminarDocumento(int typeDocument)
        {
            try
            {
                var IdPersonEntity = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                dbe.EliminarDocumentoGTH(IdPersonEntity, typeDocument);

                return RedirectToAction("TreeAchi", new { id = IdPersonEntity });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error al intentar eliminar el documento: " + ex.Message;
                return View("Error"); 
            }
        }


        public ActionResult TreeAchi(int id = 0)
        {
            try
            {
                int ID_PARA = Convert.ToInt32(Request.Params["ID_PARA"]);
                if (ID_PARA == 0) //Carga los tipos de documentos
                {
                    var query = (from q in dbe.TYPE_DOCUMENT.ToList()
                                 where q.REGISTER == 1 && q.VIG_DOCU == true && q.ID_TYPE_DOCU != 1 && q.ID_TYPE_DOCU != 12 && q.ID_TYPE_DOCU != 64
                                 select new
                                 {
                                     ID_PARA = q.ID_TYPE_DOCU,
                                     NAME_PARA = q.NAM_DOCU,
                                     HAS_VALUE = (CountDocsByTypeDocs(q.ID_TYPE_DOCU, id) > 0 ? true : false),
                                     VAL_PARA = q.NAM_DOCU,
                                     expanded = false,
                                     ATTACH = false,
                                     GRUPO = q.GRUPO_DOCU,
                                     q.ORDEN
                                 }).ToList();
                    return Json(query.OrderBy(x => x.ORDEN), JsonRequestBehavior.AllowGet);
                }
                else
                { //Carga los documentos
                    var query = (from q in dbe.PERSON_DOCUMENTS.ToList()
                                 where q.ID_TYPE_DOCU == ID_PARA && q.ID_PERS_ENTI == id && q.ID_TYPE_DOCU != 1 && q.ID_TYPE_DOCU != 12 && q.ID_TYPE_DOCU != 64
                                 select new
                                 {
                                     ID_PARA = q.ID_PERS_DOCU,
                                     NAME_PARA = q.NAM_ATTA,
                                     NAM_ATTA = q.NAM_ATTA + q.EXT_ATTA,
                                     HAS_VALUE = false,
                                     expanded = false,
                                     ATTACH = true,
                                     CREATE_ATTA = q.CREATE_ATTA,
                                     ID_TYPE_DOCU = q.ID_TYPE_DOCU,
                                     Descripcion = q.Descripcion == null ? "" : q.Descripcion,
                                     NAM_FILE = (q.EXT_ATTA).ToLower() == ".pdf" || (q.EXT_ATTA).ToLower() == ".png" ||
                                                (q.EXT_ATTA).ToLower() == ".jpg" || (q.EXT_ATTA).ToLower() == ".txt"
                                                ? "/Talent/VerDocumento/1/" + q.NAM_ATTA + "_" + Convert.ToString(q.ID_PERS_DOCU) + q.EXT_ATTA : "/Talent/DescargarArchivo/1/" + (q.NAM_ATTA + "_" + Convert.ToString(q.ID_PERS_DOCU) + q.EXT_ATTA),
                                     //q.NAM_ATTA + "_" + Convert.ToString(q.ID_PERS_DOCU) + q.EXT_ATTA,
                                     DELETE = DeleteFile(Convert.ToInt32(q.ID_PERS_ENTI))
                                 });
                    return Json(query.OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                var query = (from q in dbe.TYPE_DOCUMENT.ToList()
                             where q.REGISTER == 1 && q.VIG_DOCU == true && q.ID_TYPE_DOCU != 1 && q.ID_TYPE_DOCU != 12 && q.ID_TYPE_DOCU != 64
                             select new
                             {
                                 ID_PARA = q.ID_TYPE_DOCU,
                                 NAME_PARA = q.NAM_DOCU,
                                 HAS_VALUE = (CountDocsByTypeDocs(q.ID_TYPE_DOCU, id) > 0 ? true : false),
                                 VAL_PARA = q.NAM_DOCU,
                                 expanded = false,
                                 ATTACH = false,
                                 q.ORDEN
                             });

                return Json(query.OrderBy(x => x.ORDEN), JsonRequestBehavior.AllowGet);
            }
        }

        public bool DeleteFile(int ID_PERS_ENTI)
        {
            int val = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int pe = Convert.ToInt32(Session["RRHH_Admin"]);
            try
            {
                if (val == ID_PERS_ENTI)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        public string CapitalizeCadena(string cadena)
        {
            cadena = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cadena.ToLower());
            return cadena;
        }

        public ActionResult ListByStatus(int id = 0)
        {
            int ID_PERS_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int Count = 0, sw = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            ViewBag.ID_PERS_STAT = ID_PERS_STAT;
            //x.ID_PARA == 18: CIA SUPPLY(Compania con el personal)
            //int ID_ENTI = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA);
            var queryCIA = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO);
            int ID_ENTI = Convert.ToInt32(queryCIA.VAL_ACCO_PARA);

            var Person = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_STAT == ID_PERS_STAT)
                            .Where(x => x.ID_ENTI1 == ID_ENTI)
                            .Join(dbe.ACCOUNT_ENTITY, pe => pe.ID_PERS_ENTI, ae => ae.ID_PERS_ENTI, (pe, ae) => new
                            {
                                pe.ID_PERS_ENTI,
                                ae.ID_ACCO,
                                pe.ID_ENTI1,
                                pe.ID_ENTI2,
                                pe.ID_ACCO_PERT,
                                pe.EMA_PERS,
                                pe.EMA_ELEC,
                                pe.CEL_PERS,
                                pe.TEL_PERS,
                                ae.VIS_TALE,
                                pe.ID_FOTO,
                            })
                            .Where(x => x.ID_ACCO == ID_ACCO)
                            .Where(x => x.VIS_TALE == true)
                            .Join(dbe.CLASS_ENTITY, pe => pe.ID_ENTI2, ce => ce.ID_ENTI, (pe, ce) => new
                            {
                                pe.ID_PERS_ENTI,
                                pe.ID_ACCO,
                                pe.ID_ENTI1,
                                pe.ID_ENTI2,
                                ce.FIR_NAME,
                                ce.LAS_NAME,
                                pe.ID_ACCO_PERT,
                                pe.EMA_PERS,
                                pe.EMA_ELEC,
                                pe.CEL_PERS,
                                pe.TEL_PERS,
                                pe.ID_FOTO,
                                ce.ID_ENTI,
                                ce.SEX_ENTI,
                            });

            Person.Count();
            int ind = id;
            if (ind == 0) //Solo personal a su cargo
            {
                //Obteniendo el Cargo del Usuario
                int ID_PERS_CONT = 0;
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                var query1 = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.VIG_CONT == true && x.LAS_CONT == true);
                if (query1.Count() > 0)
                {
                    ID_PERS_CONT = query1.First().ID_PERS_CONT;

                    var qPC_CH = (from a in dbe.PERSON_CONTRACT_CHART
                                  where a.ID_PERS_CONT == ID_PERS_CONT
                                  select new
                                  {
                                      a.ID_CHAR,
                                  });

                    var query4 = (from a in dbe.CHARTs
                                  join qpc in qPC_CH on a.ID_CHAR_PARE equals qpc.ID_CHAR
                                  join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR into lb
                                  from xb in lb.DefaultIfEmpty()
                                  join c in dbe.CHARTs on a.ID_CHAR equals c.ID_CHAR_PARE into lc
                                  from xc in lc.DefaultIfEmpty()
                                  join d in dbe.NAME_CHART on (xc == null ? 0 : xc.ID_NAM_CHAR) equals d.ID_NAM_CHAR into ld
                                  from xd in ld.DefaultIfEmpty()
                                  select new
                                  {
                                      ID_CHAR = (xb.ID_TYPE_CHAR == 3 ? a.ID_CHAR : (xc == null ? 0 : xc.ID_CHAR)),
                                      ID_TYPE_CHAR = (xb.ID_TYPE_CHAR == 3 ? 3 : (xd == null ? 0 : xd.ID_TYPE_CHAR)),
                                  }).Where(x => x.ID_TYPE_CHAR == 3);

                    var query5 = dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true)
                                .Join(query4, x => x.ID_CHAR, q4 => q4.ID_CHAR, (x, q4) => new
                                {
                                    x.ID_PERS_ENTI,
                                });

                    if (query5.Count() > 0)
                    {
                        Person = (from a in Person
                                  join q5 in query5 on a.ID_PERS_ENTI equals q5.ID_PERS_ENTI
                                  //where (from b in query5 select b.ID_PERS_ENTI).Contains(a.ID_PERS_ENTI)
                                  select a);
                    }
                }
            }

            if (ID_ACCO != 4)
            {
                Person = Person.Where(x => x.ID_ACCO_PERT == ID_ACCO);
            }

            Count = Person.Count();

            var Cargos = dbe.PERSON_CONTRACT.Where(pc => pc.LAS_CONT == true)
                .Join(dbe.CHARTs, pc => pc.ID_CHAR, ch => ch.ID_CHAR, (pc, ch) => new
                {
                    ch.ID_CHAR,
                    ch.ID_CHAR_PARE,
                    ch.ID_NAM_CHAR,
                    pc.ID_PERS_ENTI,
                    pc.ID_COND_CONT,
                    pc.END_DATE,
                    pc.STAR_DATE,
                    pc.ID_WORK_PERI,
                })
                .Join(dbe.CONTRACT_CONDITION, x => x.ID_COND_CONT, cc => cc.ID_COND_CONT, (x, cc) => new
                {
                    x.ID_CHAR,
                    x.ID_CHAR_PARE,
                    x.ID_NAM_CHAR,
                    x.ID_PERS_ENTI,
                    x.ID_COND_CONT,
                    x.END_DATE,
                    x.STAR_DATE,
                    x.ID_WORK_PERI,
                    cc.CONDITION
                }).ToList();

            var site_loca = dbc.SITEs
                .Join(dbc.LOCATIONs, s => s.ID_SITE, l => l.ID_SITE, (s, l) => new
                {
                    s.NAM_SITE,
                    l.NAM_LOCA,
                    l.ID_LOCA,
                    UBIC = s.NAM_SITE + " - " + l.NAM_LOCA
                }).ToList();

            var temp = Person.ToList();

            var result = (from x in Person.ToList().OrderBy(x => x.FIR_NAME).ThenBy(x => x.LAS_NAME).Skip(skip).Take(take)

                          join pc in Cargos on x.ID_PERS_ENTI equals pc.ID_PERS_ENTI into lpc
                          from xpc in lpc.DefaultIfEmpty()

                          join wp in dbe.WORK_PERIOD on xpc == null ? null : xpc.ID_WORK_PERI equals wp.ID_WORK_PERI into lwp
                          from xwp in lwp.DefaultIfEmpty()

                          join cha in dbe.NAME_CHART on xpc == null ? null : xpc.ID_NAM_CHAR equals cha.ID_NAM_CHAR into lcha
                          from xcha in lcha.DefaultIfEmpty()

                          join pl in dbe.PERSON_LOCATION.Where(x => x.END_DATE == null) on x.ID_PERS_ENTI equals pl.ID_PERS_ENTI into lpl
                          from xpl in lpl.DefaultIfEmpty()

                          join l in site_loca on xpl == null ? null : xpl.ID_LOCA equals l.ID_LOCA into ll
                          from xl in ll.DefaultIfEmpty()

                          select new
                          {
                              usuario = x.FIR_NAME.ToUpper() + " " + x.LAS_NAME.ToUpper(),
                              CAR_PERS = xcha == null ? "-" : xcha.NAM_CHAR,
                              ID_ENTI = x.ID_ENTI,
                              ID_CIA = x.ID_ENTI1,
                              FOTO = x.ID_FOTO == null ? null : x.ID_FOTO,
                              EMA_PERS = x.EMA_PERS == null ? "-" : x.EMA_PERS.ToLower(),
                              EMA_ELEC = x.EMA_ELEC == null ? "-" : x.EMA_ELEC.Trim().Length == 0 ? "-" : x.EMA_ELEC.ToLower(),
                              CEL_PERS = x.CEL_PERS == null ? "-" : x.CEL_PERS,
                              TEL_PERS = x.TEL_PERS == null ? "-" : x.TEL_PERS,
                              NOM_AREA = FindNodo(Convert.ToInt32((xpc == null ? 0 : xpc.ID_CHAR_PARE)), 2),
                              SITIO = xl == null ? "-" : xl.UBIC.ToLower(),
                              USB = FindNodo(Convert.ToInt32((xpc == null ? 0 : xpc.ID_CHAR_PARE)), 1),
                              STAR_CONT = xwp == null ? "-" : String.Format("{0:d}", xwp.STAR_DATE),
                              END_CONT = xpc == null ? "-" : String.Format("{0:d}", xpc.END_DATE),
                              CESS_DATE = xwp == null ? "-" : String.Format("{0:d}", xwp.CESS_DATE),
                              CONDITION = xpc == null ? "-" : xpc.CONDITION.ToLower(),
                              ID_PERS_ENTI = x.ID_PERS_ENTI,
                              ID_PERS_STAT = ID_PERS_STAT,
                          });

            var q = result.ToList();
            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        public string FindNodo(int idchpare = 0, int type = 0)
        {

            var query = (from a in dbe.CHARTs.Where(x => x.ID_CHAR == idchpare)
                         join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR

                         join c in dbe.CHARTs on a.ID_CHAR_PARE equals c.ID_CHAR into lc
                         from xc in lc.DefaultIfEmpty()
                         join d in dbe.NAME_CHART on (xc == null ? 0 : xc.ID_NAM_CHAR) equals d.ID_NAM_CHAR into ld
                         from xd in ld.DefaultIfEmpty()

                         join e in dbe.CHARTs on (xc == null ? 0 : xc.ID_CHAR_PARE) equals e.ID_CHAR into le
                         from xe in le.DefaultIfEmpty()
                         join f in dbe.NAME_CHART on (xe == null ? 0 : xe.ID_NAM_CHAR) equals f.ID_NAM_CHAR into lf
                         from xf in lf.DefaultIfEmpty()

                         join g in dbe.CHARTs on (xe == null ? 0 : xe.ID_CHAR_PARE) equals g.ID_CHAR into lg
                         from xg in lg.DefaultIfEmpty()
                         join h in dbe.NAME_CHART on (xg == null ? 0 : xg.ID_NAM_CHAR) equals h.ID_NAM_CHAR into lh
                         from xh in lh.DefaultIfEmpty()

                         select new
                         {
                             NAM_CHAR = (b.ID_TYPE_CHAR == type ? b.NAM_CHAR :
                                        (xd == null ? "-" : xd.ID_TYPE_CHAR == type ? xd.NAM_CHAR :
                                        (xf == null ? "-" : xf.ID_TYPE_CHAR == type ? xf.NAM_CHAR :
                                        (xh == null ? "-" : xh.ID_TYPE_CHAR == type ? xh.NAM_CHAR : "-"
                                        )))),
                         });
            if (query.Count() > 0)
            {
                return query.First().NAM_CHAR.ToLower();
            }
            return "-";
        }

        public string NameNivSuperior(int idchpare = 0, int type = 0)
        {
            var query = dbe.CHARTs.Where(x => x.ID_CHAR == idchpare)
                        .Join(dbe.NAME_CHART, x => x.ID_NAM_CHAR, n => n.ID_NAM_CHAR, (x, n) => new
                        {
                            x.ID_CHAR_PARE,
                            n.NAM_CHAR,
                            n.ID_TYPE_CHAR
                        });

            if (query.Count() > 0)
            {
                var q1 = query.Single();
                if (q1.ID_TYPE_CHAR == type)
                {
                    return q1.NAM_CHAR.ToLower();
                }
                else
                {
                    return NameNivSuperior(Convert.ToInt32(q1.ID_CHAR_PARE), type);
                }
            }
            return "-";
        }

        public ActionResult MyProfile()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ToList()
                        .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                        {
                            ce.SEX_ENTI,
                            x.ID_FOTO
                        }).First();
            string idpe = ID_PERS_ENTI.ToString();
            int mg = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 17 && x.VAL_ACCO_PARA == idpe).Count();

            ViewBag.TitleProfile = ResourceLanguaje.Resource.MyProfile;
            ViewBag.FOTO = query.ID_FOTO.ToString() + ".jpg";
            ViewBag.ID_PERS_ENTI = ID_PERS_ENTI;
            ViewBag.Layout = "Nulo";
            ViewBag.Manager = mg;
            return View("Index1");
        }

        [Authorize]
        public ActionResult Profile(int id = 0)
        {
            var query = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id).ToList()
                        .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                        {
                            ce.SEX_ENTI,
                            x.ID_FOTO
                        }).First();
            ViewBag.TitleProfile = ResourceLanguaje.Resource.Profile;
            if (Convert.ToInt32(Session["ID_PERS_ENTI"]) == id || Convert.ToInt32(Session["ADMINISTRADOR_SISTEMA"]) == 1 || Convert.ToInt32(Session["RRHH"]) == 1)
            {
                ViewBag.TitleProfile = ResourceLanguaje.Resource.MyProfile;
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }

            string idpe = id.ToString();
            int mg = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 17 && x.VAL_ACCO_PARA == idpe).Count();

            ViewBag.Manager = mg;
            ViewBag.FOTO = query.ID_FOTO.ToString() + ".jpg";
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.Layout = "";
            return View("Index1");
        }

        [Authorize]
        public ActionResult Edit(int id = 0, int id1 = 0)
        {
            string ID_PE = Convert.ToString(Session["ID_PERS_ENTI"]);
            if (AccesoContracEdit(ID_PE))
            {
                var query = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id);

                ViewBag.Supervisor = id1;
                ViewBag.FOTO = query.ID_FOTO.ToString() + ".jpg";
                ViewBag.ID_PERS_ENTI = id;
                ViewBag.ID_PERS_STAT = query.ID_PERS_STAT;
                ViewBag.My_ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                return View();
            }
            else
            {
                return Content("<div style=\"width:100%; position: relative;height:100%; text-align:center; \">" +
                "<div style=\"position: absolute; top: 50%; left: 50%; height: 100px; width: 300px; " +
                "margin: -50px 0 0 -150px; vertical-align:middle; color:#B21E1E; font-size:2em;\">ACCESO RESTRINGIDO</div></div>");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(IEnumerable<HttpPostedFileBase> filesPhoto, CLASS_ENTITY classentity)
        {
            string FIR_NAME = Convert.ToString(Request.Form["FIR_NAME"]);
            string LAS_NAME = Convert.ToString(Request.Form["LAS_NAME"]);
            string SEC_NAME = Convert.ToString(Request.Form["SEC_NAME"]);
            string THI_NAME = Convert.ToString(Request.Form["THI_NAME"]);


            DateTime? BIRTHDAY;

            //CLASS_ENTITY ce = new CLASS_ENTITY();
            var ce = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == classentity.ID_ENTI);

            int sw = 0; //ID_CARG_AREA = 0, ID_AREA = 0, ID_LOCA = 0;
            if (FIR_NAME.Trim().Length == 0) { sw = 1; }
            else if (LAS_NAME.Trim().Length == 0) { sw = 1; }
            else if (Convert.ToString(Request.Params["BIRTHDAY"]) == "") { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            else
            {
                int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);
                WebImage img = WebImage.GetImageFromRequest("filesPhoto");
                if (img != null)
                {   //Redimensionando la imagen
                    //int alt = img.Height;
                    //int anc = img.Width;
                    //decimal por = 0;
                    //if (alt > anc && alt > 84)
                    //{
                    //    por = 84 / Convert.ToDecimal(alt);
                    //    alt = 84;
                    //    anc = Convert.ToInt32(Convert.ToDecimal(anc) * por);
                    //}
                    //if (anc > alt && anc > 84)
                    //{
                    //    por = 84 / Convert.ToDecimal(anc);
                    //    anc = 84;
                    //    alt = Convert.ToInt32(Convert.ToDecimal(alt) * por);
                    //}
                    //img.Resize(anc, alt);
                    img.Save(Server.MapPath("~/Content/Fotos/" + ID_PERS_ENTI.ToString() + ".jpg"));
                }

                try
                {
                    //string S_BIRTHDAY = Convert.ToString(Request.Params["BIRTHDAY"]);
                    BIRTHDAY = DateTime.ParseExact(Convert.ToString(Request.Params["BIRTHDAY"].ToString()), "d/M/yyyy hh:mm tt", null);
                    ce.FIR_NAME = FIR_NAME.ToUpper();
                    ce.LAS_NAME = LAS_NAME.ToUpper();
                    ce.SEC_NAME = (classentity.SEC_NAME == null ? null : classentity.SEC_NAME.ToUpper());
                    ce.THI_NAME = (classentity.THI_NAME == null ? null : classentity.THI_NAME.ToUpper());

                    ce.MOT_NAME = (classentity.MOT_NAME == null ? null : classentity.MOT_NAME.ToUpper());
                    ce.ADDRESS = (classentity.ADDRESS == null ? null : classentity.ADDRESS.ToUpper());
                    ce.SEX_ENTI = classentity.SEX_ENTI.ToUpper();
                    ce.BIRTHDAY = (BIRTHDAY == null ? null : BIRTHDAY);
                    ce.TEL_ENTI = (classentity.TEL_ENTI == null ? null : classentity.TEL_ENTI);
                    ce.CEL_ENTI = (classentity.CEL_ENTI == null ? null : classentity.CEL_ENTI);
                    ce.EMA_ENTI = (classentity.EMA_ENTI == null ? null : classentity.EMA_ENTI.ToUpper());
                    ce.NUM_TYPE_DI = (classentity.NUM_TYPE_DI == null ? null : classentity.NUM_TYPE_DI);
                    ce.EXPIRE_DATE_DNI = (classentity.EXPIRE_DATE_DNI == null ? null : classentity.EXPIRE_DATE_DNI);
                    ce.EMISSION_DATE_DNI = (classentity.EMISSION_DATE_DNI == null ? null : classentity.EMISSION_DATE_DNI);
                    ce.CONTACT_NAME = (classentity.CONTACT_NAME == null ? null : classentity.CONTACT_NAME.ToUpper());
                    ce.CONTACT_PHONE = (classentity.CONTACT_PHONE == null ? null : classentity.CONTACT_PHONE);
                    ce.ID_CIVI_STAT = (classentity.ID_CIVI_STAT == null ? null : classentity.ID_CIVI_STAT);
                    ce.ID_NATI = (classentity.ID_NATI == null ? null : classentity.ID_NATI);
                    ce.ID_TYPE_DI = (classentity.ID_TYPE_DI == null ? null : classentity.ID_TYPE_DI);
                    ce.ID_DEGR_INST = (classentity.ID_DEGR_INST == null ? null : classentity.ID_DEGR_INST);
                    ce.ID_BLOO_GROU = (classentity.ID_BLOO_GROU == null ? null : classentity.ID_BLOO_GROU);
                    ce.IdProfesion = classentity.IdProfesion;
                    ce.IdUniversidad = classentity.IdUniversidad;

                    dbe.CLASS_ENTITY.Attach(ce);
                    dbe.Entry(ce).State = EntityState.Modified;
                    dbe.SaveChanges();

                    PERSON_ENTITY pe = new PERSON_ENTITY();
                    pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI);

                    pe.EMA_ELEC = Convert.ToString(Request.Form["EMA_ELEC"]).ToUpper();
                    pe.EMA_PERS = Convert.ToString(Request.Form["EMA_PERS"]).ToUpper();
                    pe.CEL_PERS = Convert.ToString(Request.Form["CEL_PERS"]);
                    pe.TEL_PERS = Convert.ToString(Request.Form["TEL_PERS"]);
                    pe.EXT_PERS = Convert.ToString(Request.Form["EXT_PERS"]);

                    pe.RPM_PERS = Convert.ToString(Request.Params["RPM_PERS_ENTI"]);
                    pe.ID_SUB_CIA = Convert.ToInt32(Request.Params["NAM_CIA"]);

                    if (img != null) { pe.ID_FOTO = ID_PERS_ENTI; }
                    else if (pe.ID_FOTO < 1)
                    {
                        if (classentity.SEX_ENTI == "M") { pe.ID_FOTO = 0; }
                        else { pe.ID_FOTO = -1; }
                    }

                    dbe.PERSON_ENTITY.Attach(pe);
                    dbe.Entry(pe).State = EntityState.Modified;
                    dbe.SaveChanges();

                    //PERSON_LOCATION pl = new PERSON_LOCATION();
                    //pl = dbe.PERSON_LOCATION.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.END_DATE == null);
                    //if (pl.ID_LOCA != ID_LOCA) {
                    //    pl.END_DATE = DateTime.Now;
                    //    pl.VIG_LOCA = false;
                    //    dbe.PERSON_LOCATION.Attach(pl);
                    //    dbe.Entry(pl).State = EntityState.Modified;
                    //    dbe.SaveChanges();

                    //    var plNew = new PERSON_LOCATION();
                    //    plNew.ID_PERS_ENTI = ID_PERS_ENTI;
                    //    plNew.ID_LOCA = ID_LOCA;
                    //    plNew.STAR_DATE = DateTime.Now;
                    //    plNew.VIG_LOCA = true;
                    //    dbe.PERSON_LOCATION.Add(plNew);
                    //    dbe.SaveChanges();
                    //}

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('OK','Se actualizó el perfil');}window.onload=init;</script>");
                }
                catch (Exception)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensaje) top.mensaje('ERROR','Ha ocurrido un error, contacte al administrador');}window.onload=init;</script>");
                }
            }
        }

        public ActionResult AttachInvoice(int id = 0, int id1 = 0)
        {

            var qPer = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id)
                        .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, c => c.ID_ENTI, (x, c) => new
                        {
                            Employee = c.FIR_NAME.Substring(0, 1).ToUpper() + c.FIR_NAME.Substring(1, c.FIR_NAME.Length - 1).ToLower() +
                            " " + c.LAS_NAME.Substring(0, 1).ToUpper() + c.LAS_NAME.Substring(1, c.LAS_NAME.Length - 1).ToLower(),
                        });

            if (qPer.Count() > 0)
            {
                ViewBag.Employee = qPer.First().Employee;
            }
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.Profile = id1;
            return View();
        }

        public ActionResult AttachInvoiceUser(int id = 0)
        {
            var qPer = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id)
                        .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, c => c.ID_ENTI, (x, c) => new
                        {
                            Employee = c.FIR_NAME.Substring(0, 1).ToUpper() + c.FIR_NAME.Substring(1, c.FIR_NAME.Length - 1).ToLower() +
                            " " + c.SEC_NAME.Substring(0, 1).ToUpper() + c.SEC_NAME.Substring(1, c.SEC_NAME.Length - 1).ToLower() +
                            " " + c.LAS_NAME.Substring(0, 1).ToUpper() + c.LAS_NAME.Substring(1, c.LAS_NAME.Length - 1).ToLower() +
                            " " + c.MOT_NAME.Substring(0, 1).ToUpper() + c.MOT_NAME.Substring(1, c.MOT_NAME.Length - 1).ToLower(),
                        });

            if (qPer.Count() > 0)
            {
                ViewBag.Employee = qPer.First().Employee;
            }
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AttachInvoiceUser(IEnumerable<HttpPostedFileBase> filesInvoice, ACCOUNTING_MONTH acco_mont)
        {
            int sw = 0, ID_PERS_ENTI, ID_ACCO_YEAR = 0,
                ID_ACCO_MONT = 0, ID_TYPE_PAYM = 0;
            string reques = "request1", error = "0";

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_PAYM"]), out ID_TYPE_PAYM) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_ACCO_YEAR"]), out ID_ACCO_YEAR) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_ACCO_MONT"]), out ID_ACCO_MONT) == false) { sw = 1; }
            else if (filesInvoice == null) { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + error + "');}window.onload=init;</script>");
            }

            ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);

            string NAM_PERS = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, c => c.ID_ENTI, (x, c) => new
                {
                    NAM = c.FIR_NAME + " " + c.SEC_NAME + " " + c.LAS_NAME + " " + c.MOT_NAME
                }).First().NAM;

            //Guardando archivo adjunto
            foreach (var file in filesInvoice)
            {
                if (file != null)
                {
                    try
                    {
                        string nam_file = "";
                        if (ID_TYPE_PAYM == 1) { nam_file = "Payment Ballot"; }
                        else if (ID_TYPE_PAYM == 3) { nam_file = "5th Category Rent Certificate"; }
                        else if (ID_TYPE_PAYM == 4) { nam_file = "Certificate Utilities"; }

                        var ATTA = new PERSON_INVOICE();
                        ATTA.ID_TYPE_PAYM = ID_TYPE_PAYM;
                        ATTA.NAM_ATTA = nam_file + " (Signed)";
                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                        ATTA.CREATE_ATTA = DateTime.Now;
                        ATTA.ID_ACCO_MONT = ID_ACCO_MONT;
                        ATTA.SIGNED = true;
                        ATTA.UserId = Convert.ToInt32(Session["UserId"]);

                        dbe.PERSON_INVOICE.Add(ATTA);
                        dbe.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Invoice/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_INVO) + ATTA.EXT_ATTA));

                        ViewBag.ANIO_MES = ViewBag.ANIO_MES + 1;
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                    }
                }
            }
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','AttachInvoice');}window.onload=init;</script>");

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AttachInvoice(IEnumerable<HttpPostedFileBase> filesInvoice, ACCOUNTING_MONTH acco_mont)
        {
            int sw = 0, HD_SIGNED = 0, SIGNED = 1, //SIGNED = 0, 
                ID_PERS_ENTI, ID_ACCO_YEAR = 0,
                ID_ACCO_MONT = 0, ID_TYPE_PAYM = 0;
            string reques = "request1", error = "0";

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_PAYM"]), out ID_TYPE_PAYM) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Params["ID_ACCO_YEAR"]), out ID_ACCO_YEAR) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Params["ID_ACCO_MONT"]), out ID_ACCO_MONT) == false) { sw = 1; }
            else if (filesInvoice == null) { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensajeCarga) top.mensajeCarga('ERROR','" + error + "');}window.onload=init;</script>");
            }

            HD_SIGNED = Convert.ToInt32(Request.Params["HD_SIGNED"]);
            if (HD_SIGNED == 2)
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_EMP"]), out ID_PERS_ENTI) == false) { sw = 1; error = "2"; }
                if (sw == 1)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.mensajeCarga) top.mensajeCarga('ERROR','" + error + "');}window.onload=init;</script>");
                }
                HD_SIGNED = 1;//El estado siempre sea Signed(Firmado).
                reques = "request2";
            }
            else { ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]); }

            if (HD_SIGNED == 1) { SIGNED = HD_SIGNED; }
            else { SIGNED = Convert.ToInt32(Request.Params["SIGNED"]); }

            string NAM_PERS = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, c => c.ID_ENTI, (x, c) => new
                {
                    NAM = c.FIR_NAME + " " + c.LAS_NAME
                }).First().NAM;

            //Guardando archivo adjunto
            foreach (var file in filesInvoice)
            {
                if (file != null)
                {
                    try
                    {
                        string nam_file = "";
                        if (ID_TYPE_PAYM == 1) { nam_file = "Payment Ballot"; }
                        else if (ID_TYPE_PAYM == 3) { nam_file = "5th Category Rent Certificate"; }
                        else if (ID_TYPE_PAYM == 4) { nam_file = "Certificate Utilities"; }
                        else if (ID_TYPE_PAYM == 5) { nam_file = "Gratification"; }
                        else if (ID_TYPE_PAYM == 6) { nam_file = "Payment Ballot NetCare"; }

                        var ATTA = new PERSON_INVOICE();
                        ATTA.ID_TYPE_PAYM = ID_TYPE_PAYM;
                        ATTA.NAM_ATTA = nam_file + (SIGNED == 0 ? " (Unsigned)" : " (Signed)");
                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                        ATTA.CREATE_ATTA = DateTime.Now;
                        ATTA.ID_ACCO_MONT = ID_ACCO_MONT;
                        ATTA.SIGNED = (SIGNED == 0 ? false : true);
                        ATTA.UserId = Convert.ToInt32(Session["UserId"]);

                        dbe.PERSON_INVOICE.Add(ATTA);
                        dbe.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Invoice/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_INVO) + ATTA.EXT_ATTA));

                        //if (ATTA.SIGNED == false)
                        //{
                        //    SendMail mail = new SendMail();
                        //    string _html = mail.NotificationPaymentBallot(ATTA.ID_INVO);
                        //}

                        ViewBag.ANIO_MES = ViewBag.ANIO_MES + 1;
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.mensajeActualizacion) top.mensajeActualizacion('ERROR','1');}window.onload=init;</script>");
                    }
                }
            }
            if (reques == "request1")
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.mensajecarga) top.mensajecarga('ok');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.mensajeCarga) top.mensajeCarga('OK');}window.onload=init;</script>");
            }

        }

        public ActionResult ListIssuesUser(int id = 0)
        {
            var resultDocu = (from pd in dbe.PERSON_DOCUMENTS.Where(x => x.ID_PERS_ENTI == id)
                              join d in dbe.TYPE_DOCUMENT on pd.ID_TYPE_DOCU equals d.ID_TYPE_DOCU
                              where d.REGISTER == -1
                              select new
                              {
                                  Ruta = (pd.EXT_ATTA).ToLower() == ".pdf" || (pd.EXT_ATTA).ToLower() == ".png" ||
                                            (pd.EXT_ATTA).ToLower() == ".jpg" || (pd.EXT_ATTA).ToLower() == ".txt"
                                            ? "/Talent/VerDocumento/1/" : "/Talent/DescargarArchivo/1/",
                                  pd.NAM_ATTA,
                                  pd.EXT_ATTA,
                                  pd.ID_PERS_DOCU,
                                  d.NAM_DOCU,
                                  d.ID_TYPE_DOCU,
                                  d.REGISTER,
                              }).OrderBy(x => x.ID_TYPE_DOCU);

            return Json(new { data = resultDocu, Count = resultDocu.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AttachDocs(int id = 0, int id1 = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.ID_PERS_STAT = id1;
            return View();
        }

        public ActionResult ListTypeDocument(int id = 0)
        {
            if (id == 0)
            {
                var resultDocu = (from d in dbe.TYPE_DOCUMENT
                                  where d.USER_DOWNLOAD == true
                                      && d.NAM_DOCU != "Carta de Oferta"
                                      && d.NAM_DOCU != "Pasaporte"
                                      && d.NAM_DOCU != "Formato EPS"
                                  select new
                                  {
                                      d.NAM_DOCU,
                                      d.ID_TYPE_DOCU,
                                      d.REGISTER,
                                      d.ORDEN
                                  }).OrderBy(x => x.NAM_DOCU);

                return Json(new { Data = resultDocu, Count = resultDocu.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var resultDocu = (from o in dbe.OPTION_STATUS_TYPEDOCU
                                  join d in dbe.TYPE_DOCUMENT on o.ID_TYPE_DOCU equals d.ID_TYPE_DOCU
                                  where o.ID_PERS_STAT == id
                                      && o.VIG_OPTI == true
                                      && d.NAM_DOCU != "Carta de Oferta"
                                      && d.NAM_DOCU != "Pasaporte"
                                      && d.NAM_DOCU != "Formato EPS"
                                  select new
                                  {
                                      d.NAM_DOCU,
                                      d.ID_TYPE_DOCU,
                                      d.REGISTER,
                                      d.ORDEN
                                  }).OrderBy(x => x.NAM_DOCU);

                return Json(new { Data = resultDocu, Count = resultDocu.Count() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AttachDocs(IEnumerable<HttpPostedFileBase> filesDocs, PERSON_DOCUMENTS perdocs)
        {
            int sw = 0, ID_PERS_ENTI = 0, ID_TYPE_DOCU = 0, ID_COND_CONT = 0, ID_WORK_PERI = 0, Error = 0, ID_CARG_AREA = 0, ID_PERS_STAT;
            DateTime STAR_DATE, END_DATE;

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_DOCU"]), out ID_TYPE_DOCU) == false) { sw = 1; }
            else if (filesDocs == null) { sw = 1; Error = 4; }
            int taVigencia = Convert.ToInt32(Request.Form["hdnVigencia"].ToString());
            try
            {
                ID_CARG_AREA = Convert.ToInt32(Request.Form["ID_CARG_AREA"]);
                perdocs.Vigencia = Convert.ToBoolean(taVigencia);
            }
            catch { }

            var cont = new PERSON_CONTRACT();
            if (ID_TYPE_DOCU == 2)
            {//Caso Contrato
                if (Int32.TryParse(Convert.ToString(Request.Form["ID_COND_CONT"]), out ID_COND_CONT) == false) { sw = 1; }
                else if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE"]), out STAR_DATE) == false) { sw = 1; }
                else if (DateTime.TryParse(Convert.ToString(Request.Form["END_DATE"]), out END_DATE) == false) { sw = 1; }
                else if (STAR_DATE >= END_DATE) { sw = 1; Error = 2; }
                else if (filesDocs == null) { sw = 1; Error = 3; }
                else if (ID_CARG_AREA == 0) { sw = 1; Error = 5; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_STAT"]), out ID_PERS_STAT) == false) { sw = 1; }
                else
                {
                    cont.STAR_DATE = STAR_DATE;
                    cont.END_DATE = END_DATE;
                    cont.ID_COND_CONT = ID_COND_CONT;
                    cont.ID_PERS_STAT = ID_PERS_STAT;
                    cont.ID_CARG_AREA = ID_CARG_AREA;
                }
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','" + Error + "');}window.onload=init;</script>");
            }

            ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);
            //Para el caso que e cargue el contrato laboral
            if (ID_TYPE_DOCU == 2)
            {
                var contrac = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.VIG_CONT == true);
                if (contrac.Count() > 0)
                {
                    var conAnt = contrac.First();
                    ID_WORK_PERI = Convert.ToInt32(conAnt.ID_WORK_PERI);
                    conAnt.VIG_CONT = false;
                    conAnt.LAS_CONT = false;
                    dbe.PERSON_CONTRACT.Attach(conAnt);
                    dbe.Entry(conAnt).State = EntityState.Modified;
                    dbe.SaveChanges();
                }
                else
                {
                    var wp = new WORK_PERIOD();
                    wp.STAR_DATE = cont.STAR_DATE;
                    dbe.WORK_PERIOD.Add(wp);
                    dbe.SaveChanges();

                    ID_WORK_PERI = Convert.ToInt32(wp.ID_WORK_PERI);
                }

                cont.ID_PERS_ENTI = ID_PERS_ENTI;
                cont.VIG_CONT = true;
                cont.LAS_CONT = true;
                cont.ID_WORK_PERI = ID_WORK_PERI;

                dbe.PERSON_CONTRACT.Add(cont);
                dbe.SaveChanges();

                //enviando correo
                SendEmailContract(ID_PERS_ENTI, 1);
            }

            var pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI);

            if (cont.ID_PERS_CONT == 0 && pe.ID_PERS_STAT != 4 && pe.ID_PERS_STAT != 2)
            {
                var contrac = dbe.PERSON_CONTRACT.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.VIG_CONT == true && x.LAS_CONT == true);
                cont.ID_PERS_CONT = contrac.ID_PERS_CONT;
            }


            //Guardando archivo adjunto
            foreach (var file in filesDocs)
            {
                if (file != null)
                {
                    try
                    {
                        //var ATTA = new PERSON_DOCUMENTS();
                        String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                                RegexOptions.None, TimeSpan.FromSeconds(2.5));
                        perdocs.NAM_ATTA = doc;
                        perdocs.EXT_ATTA = Path.GetExtension(file.FileName);
                        perdocs.ID_PERS_ENTI = ID_PERS_ENTI;
                        perdocs.CREATE_ATTA = DateTime.Now;
                        perdocs.ID_TYPE_DOCU = ID_TYPE_DOCU;
                        perdocs.ID_PERS_CONT = cont.ID_PERS_CONT;
                        perdocs.UserId = Convert.ToInt32(Session["UserId"]);
                        perdocs.Estado = true;

                        dbe.PERSON_DOCUMENTS.Add(perdocs);
                        dbe.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Documents/" + perdocs.NAM_ATTA + "_" + Convert.ToString(perdocs.ID_PERS_DOCU) + perdocs.EXT_ATTA));

                        if (ID_TYPE_DOCU == 4) //Enviando correo Issues
                        {
                            SendEmailIssues(perdocs.ID_PERS_DOCU, perdocs.NAM_ATTA + "_" + Convert.ToString(perdocs.ID_PERS_DOCU) + perdocs.EXT_ATTA);
                        }
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.mensaje) top.mensaje('ERROR',6);}window.onload=init;</script>");
                    }
                }
            }


            int IdCertificacion = Convert.ToInt32(perdocs.IdCertificacion);
            if (IdCertificacion != 0)
            {
                Certificado objCertificado = dbe.Certificado.Where(x => x.Id == IdCertificacion).SingleOrDefault();
                objCertificado.IdEstado = 3;
                objCertificado.FechaModifica = DateTime.Now;
                objCertificado.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
                dbe.Certificado.Attach(objCertificado);
                dbe.Entry(objCertificado).State = EntityState.Modified;
                dbe.SaveChanges();
            }

            //int IdInstituto = Convert.ToInt32(perdocs.IdInstituto);
            //if (IdInstituto != 0)
            //{
            //    Institucion objInstitucion = dbe.Institucion.Where(x => x.CodIns == IdInstituto).SingleOrDefault();
            //    objInstitucion.FechaModifica = DateTime.Now;
            //    objInstitucion.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
            //    dbe.Institucion.Attach(objInstitucion);
            //    dbe.Entry(objInstitucion).State = EntityState.Modified;
            //    dbe.SaveChanges();
            //}

            int TypeReg = dbe.TYPE_DOCUMENT.Single(x => x.ID_TYPE_DOCU == ID_TYPE_DOCU).REGISTER.Value;
            string msnUpdate = "AttachDocs";
            if (TypeReg == 1) { msnUpdate = "AttachAchie"; }
            else if (TypeReg == -1) { msnUpdate = "AttachIssue"; }

            return Content("<script type='text/javascript'> function init() {" +
            "if(top.mensaje) top.mensaje('OK','Se cargaron los documentos con éxito.');}window.onload=init;</script>");
        }

        private void SendEmailIssues(int id = 0, string arc = "")
        {
            //id = ID_PERS_DOCU
            SendMail mail = new SendMail();
            mail.MailIssuesEmployee(id, arc);
        }

        public string DeleteAttachInvoice(int id = 0)
        {
            try
            {
                PERSON_INVOICE atta = dbe.PERSON_INVOICE.Find(id);
                try
                {
                    string arc = atta.NAM_ATTA + "_" + Convert.ToString(atta.ID_INVO) + atta.EXT_ATTA;
                    dbe.PERSON_INVOICE.Remove(atta);
                    dbe.SaveChanges();

                    //Eliminando
                    System.IO.File.Delete(Server.MapPath("~/Adjuntos/Talent/Invoice/" + arc));
                }
                catch (Exception)
                {
                    return "ERROR";
                }
                return "OK";
            }
            catch (Exception)
            {
                return "ERROR";
            }

        }

        public string DeleteAttachDocs(int id = 0)
        {
            try
            {
                PERSON_DOCUMENTS atta = dbe.PERSON_DOCUMENTS.Find(id);
                try
                {
                    string arc = atta.NAM_ATTA + "_" + Convert.ToString(atta.ID_PERS_DOCU) + atta.EXT_ATTA;
                    dbe.PERSON_DOCUMENTS.Remove(atta);
                    dbe.SaveChanges();

                    //Eliminando
                    System.IO.File.Delete(Server.MapPath("~/Adjuntos/Talent/Documents/" + arc));
                }
                catch (Exception)
                {
                    return "ERROR";
                }
                return "OK";
            }
            catch (Exception)
            {
                return "ERROR";
            }

        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult NewProfile()
        {
            ViewBag.TODAY = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
            ViewBag.anio = DateTime.Today.Year;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewProfile(IEnumerable<HttpPostedFileBase> filesPhoto, IEnumerable<HttpPostedFileBase> filesDocs, CLASS_ENTITY cl_enti)
        {
            try
            {
                string S_FIR_NAME = string.Empty;
                string S_SEC_NAME = string.Empty;
                string S_THI_NAME = string.Empty;
                string S_LAS_NAME = string.Empty;
                string S_EMA_ENTI = string.Empty;
                string S_MOT_NAME = string.Empty;
                string S_BIRTHDAY = string.Empty;

                S_FIR_NAME = Convert.ToString(Request.Params["FIR_NAME"].ToString()).ToUpper();
                S_SEC_NAME = Convert.ToString(Request.Params["SEC_NAME"].ToString()).ToUpper();
                S_THI_NAME = Convert.ToString(Request.Params["THI_NAME"].ToString()).ToUpper();
                S_LAS_NAME = Convert.ToString(Request.Params["LAS_NAME"].ToString()).ToUpper();
                S_EMA_ENTI = Convert.ToString(Request.Params["EMA_ENTI"].ToString()).ToUpper();
                S_MOT_NAME = Convert.ToString(Request.Params["MOT_NAME"].ToString()).ToUpper();
                //string S_EMA_PERS = Convert.ToString(Request.Params["EMA_PERS"].ToString()).ToUpper();


                string S_EMA_ELEC = Convert.ToString(Request.Params["EMA_ELEC"].ToString()).ToUpper();

                var DniDuplicado = (from ce in dbe.CLASS_ENTITY
                                    where ce.NUM_TYPE_DI == cl_enti.NUM_TYPE_DI
                                    select new
                                    {
                                        ce.ID_ENTI,
                                    });

                int sw = 0, Error = 0, ID_AREA = 0, ID_PERS_STAT = 0,
                    ID_LOCA = 0, ID_TYPE_DI, ID_COND_CONT, ID_CHAR = 0, ID_SUB_CIA = 0;
                string msjError = string.Empty;

                DateTime STAR_DATE, END_DATE, BIRTHDAY;

                //var fecha = String.Format("{0:M/d/yyyy}", BIRTHDAY);
                var cont = new PERSON_CONTRACT();

                ID_CHAR = Convert.ToInt32(Request.Form["ID_CHAR"]);
                //ID_AREA = Convert.ToInt32(Request.Params["ID_AREA"]);
                if (S_FIR_NAME.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Primer Nombre."; }
                else if (S_LAS_NAME.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Apellido Paterno."; }
                else if (S_MOT_NAME.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Apellido Materno."; }
                else if (Convert.ToString(Request.Params["BIRTHDAY"].ToString()) == "") { sw = 1; msjError = "Se debe ingresar la fecha de nacimiento."; }
                else if (S_EMA_ENTI.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Email Personal."; }
                else if (cl_enti.NUM_TYPE_DI == null) { sw = 1; msjError = "Ingrese el número de documento de identidad."; }
                else if (DniDuplicado.Count() >= 1) { sw = 1; msjError = "El número de documento de identidad ya esta registrado."; }
                else if (cl_enti.IdProfesion == null) { sw = 1; msjError = "Se debe ingresar la Carrera Profesional."; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_AREA"]), out ID_AREA) == false) { sw = 1; msjError = "Se debe ingresar el Area."; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_DI"]), out ID_TYPE_DI) == false) { sw = 1; msjError = "Se debe ingresar el Primer Nombre."; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_STAT"]), out ID_PERS_STAT) == false) { sw = 1; msjError = "Se debe ingresar el Status."; }
                else if (S_EMA_ELEC.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Email de Electrodata."; }
                if (ID_PERS_STAT == 1 || ID_PERS_STAT == 2)
                {

                    if (Convert.ToString(Request.Params["STAR_DATE"].ToString()) == "") { sw = 1; msjError = "Se debe ingresar la fecha de Inicio del Contrato."; }
                    else if (Convert.ToString(Request.Params["END_DATE"].ToString()) == "") { sw = 1; msjError = "Se debe ingresar la fecha de Fin del Contrato."; }
                    else if (DateTime.ParseExact(Convert.ToString(Request.Params["STAR_DATE"].ToString()), "d/M/yyyy hh:mm tt", null) > DateTime.ParseExact(Convert.ToString(Request.Params["END_DATE"].ToString()), "d/M/yyyy hh:mm tt", null)) { sw = 1; Error = 2; msjError = "La fecha de Inicio de Contrato no puede ser Mayor a la de Fin de Contrato."; }
                    else if (Int32.TryParse(Convert.ToString(Request.Form["ID_COND_CONT"]), out ID_COND_CONT) == false) { sw = 1; msjError = "Se debe ingresar la condición de Contrato."; }
                    else if (ID_CHAR == 0) { sw = 1; msjError = "Se debe ingresar el puesto."; }
                    else if (Int32.TryParse(Convert.ToString(Request.Form["ID_LOCA"]), out ID_LOCA) == false) { sw = 1; msjError = "Se debe ingresar el Lugar de Trabajo."; }
                    else if (Int32.TryParse(Convert.ToString(Request.Form["PLANILLA"]), out ID_SUB_CIA) == false) { sw = 1; msjError = "Se debe ingresar la Compañia."; }
                    // else if (filesDocs == null) { sw = 1; Error = 3; msjError = "Se debe Cargar el Documento del Contrato."; }
                    else
                    {

                        cont.STAR_DATE = DateTime.ParseExact(Convert.ToString(Request.Params["STAR_DATE"].ToString()), "d/M/yyyy hh:mm tt", null);
                        cont.END_DATE = DateTime.ParseExact(Convert.ToString(Request.Params["END_DATE"].ToString()), "d/M/yyyy hh:mm tt", null);
                        cont.ID_COND_CONT = ID_COND_CONT;
                    }
                }
                if (sw == 1)
                {
                    return Content("<script type='text/javascript'>function init() {" +
                        "if(top.mensaje){ top.mensaje('ERROR','" + msjError + "');}}window.onload=init;</script>");
                }
                //string S_EMA_ENTI = Convert.ToString(Request.Params["EMA_ENTI"].ToString()).ToUpper();
                int UserId = Convert.ToInt32(Session["UserId"]);
                cl_enti.FIR_NAME = S_FIR_NAME;
                cl_enti.SEC_NAME = S_SEC_NAME;
                cl_enti.THI_NAME = S_THI_NAME;
                cl_enti.LAS_NAME = S_LAS_NAME;
                cl_enti.MOT_NAME = (cl_enti.MOT_NAME == null ? null : cl_enti.MOT_NAME.ToUpper());
                cl_enti.EMA_ENTI = S_EMA_ENTI;
                cl_enti.CREATED = DateTime.Now;
                //BIRTHDAY = DateTime.ParseExact(Convert.ToString(Request.Params["BIRTHDAY"].ToString()), "d/M/yyyy", null);
                cl_enti.BIRTHDAY = DateTime.ParseExact(Convert.ToString(Request.Params["BIRTHDAY"].ToString()), "d/M/yyyy", null);
                cl_enti.VIG_ENTI = true;
                cl_enti.ID_TYPE_ENTI = 2;
                cl_enti.ADDRESS = (cl_enti.ADDRESS == null ? null : cl_enti.ADDRESS.ToUpper());
                cl_enti.ID_USER = UserId;

                dbe.CLASS_ENTITY.Add(cl_enti);
                dbe.SaveChanges();

                string S_CEL_PERS = Convert.ToString(Request.Params["CEL_PERS"].ToString());
                string S_EXT_PERS = Convert.ToString(Request.Params["EXT_PERS"].ToString());
                string S_TEL_RPM = Convert.ToString(Request.Params["TEL_RPM"].ToString());


                string S_EMA_PERS = Convert.ToString(Request.Params["EMA_PERS"].ToString()).ToUpper();


                var pers_enti = new PERSON_ENTITY();
                var S_ID_ENTI1 = Convert.ToString(Request.Form["ID_ENTI1"].ToString());

                int ID_ACCO_PERT = dbc.ACCOUNT_PARAMETER.Single(x => x.VAL_ACCO_PARA == S_ID_ENTI1
                    && x.ID_PARA == 6 && x.VIG_ACCO_PARA == true).ID_ACCO.Value;


                //------------------------------------------------------------------
                try { ID_SUB_CIA = Convert.ToInt32(Request.Params["PLANILLA"]); }
                catch { ID_SUB_CIA = 0; }

                pers_enti.ID_ACCO_PERT = ID_ACCO_PERT;
                pers_enti.ID_ENTI1 = Convert.ToInt32(S_ID_ENTI1);
                pers_enti.ID_ENTI2 = cl_enti.ID_ENTI;
                pers_enti.ID_AREA = ID_AREA;
                pers_enti.ID_PRIO = 4;

                pers_enti.ID_SUB_CIA = ID_SUB_CIA;


                pers_enti.ID_TYPE_CLIE = 2;
                pers_enti.CEL_PERS = S_CEL_PERS;
                pers_enti.RPM_PERS = S_TEL_RPM;
                pers_enti.EXT_PERS = S_EXT_PERS;
                pers_enti.EMA_ELEC = S_EMA_ELEC;
                pers_enti.EMA_PERS = S_EMA_PERS;
                pers_enti.CREATED = DateTime.Now;
                pers_enti.VIG_PERS_ENTI = true;
                pers_enti.ID_PERS_STAT = ID_PERS_STAT;
                pers_enti.ID_FOTO = (cl_enti.SEX_ENTI == "M" ? 0 : -1);
                pers_enti.EPS = (Convert.ToInt32(Request.Form["EPS_HF"].ToString()) == 1 ? true : false);
                //pers_enti.NroColegiatura = Request.Form["NRO_COLEGIATURA"];
                dbe.PERSON_ENTITY.Add(pers_enti);
                dbe.SaveChanges();

                //--------------------------FOT_PERS -->RELATIVO--------------------

                var fotpers = (from pe in dbe.PERSON_ENTITY
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                               join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                               where (pe.ID_ENTI1 == 9 || pe.ID_ENTI1 == 4550 || pe.ID_ENTI1 == 3975)
                               where pc.VIG_CONT == true
                               where ae.ID_ACCO == 4 && ae.VIG_ACCO_ENTI == true
                               select new
                               {
                                   pe.ID_PERS_ENTI,
                               });

                string FOT_PERS = Convert.ToString(pers_enti.ID_PERS_ENTI) + Convert.ToString(fotpers.Count().ToString("D4"));

                int id_hora = 0;
                if (ID_LOCA == 81) { id_hora = 1; }
                else if (ID_LOCA == 82) { id_hora = 2; }
                else { id_hora = 0; }

                pers_enti.FOT_PERS = FOT_PERS;
                pers_enti.ID_HORA = id_hora;


                //---------------------CANASTA NAVIDEÑA----------------------------------

                int CanNav = Convert.ToInt32(Request.Params["CanNav"]);
                var chribask = new CHRISTMAN_BASKET();
                if (CanNav == 1)
                {
                    chribask.ID_PERS_ENTI = pers_enti.ID_PERS_ENTI;
                    chribask.YEA_CHRI_BASK = DateTime.Today.Year;
                    chribask.CREATED = DateTime.Now;
                    chribask.VIG_CHRI_BASK = true;

                    dbe.CHRISTMAN_BASKET.Add(chribask);
                    dbe.SaveChanges();

                }

                //----------------------------EPS--------------------------------------------

                int epsanual = Convert.ToInt32(Request.Params["EPSANUAL"]);
                var epsyearly = new EPS_YEARLY();
                if (epsanual == 1)
                {
                    epsyearly.ID_PERS_ENTI = pers_enti.ID_PERS_ENTI;
                    epsyearly.YEA_EPS_YEAR = DateTime.Today.Year;
                    epsyearly.CREATED = DateTime.Now;
                    epsyearly.VIG_EPS_YEAR = true;

                    dbe.EPS_YEARLY.Add(epsyearly);
                    dbe.SaveChanges();

                }

                //-------------------------------------------------------------------

                int ID_PERS_ENTI = pers_enti.ID_PERS_ENTI;
                if (filesPhoto != null)
                {

                    WebImage img = WebImage.GetImageFromRequest("filesPhoto");
                    if (img != null)
                    {   //Redimensionando la imagen
                        int alt = img.Height;
                        int anc = img.Width;
                        decimal por = 0;
                        if (alt > anc && alt > 84)
                        {
                            por = 84 / Convert.ToDecimal(alt);
                            alt = 84;
                            anc = Convert.ToInt32(Convert.ToDecimal(anc) * por);
                        }
                        if (anc > alt && anc > 84)
                        {
                            por = 84 / Convert.ToDecimal(anc);
                            anc = 84;
                            alt = Convert.ToInt32(Convert.ToDecimal(alt) * por);
                        }
                        img.Resize(anc, alt);
                        img.Save(Server.MapPath("~/Content/Fotos/" + ID_PERS_ENTI.ToString() + ".jpg"));
                    }

                    pers_enti.ID_FOTO = ID_PERS_ENTI;
                }

                //Guardando la Actualización

                dbe.PERSON_ENTITY.Attach(pers_enti);
                dbe.Entry(pers_enti).State = EntityState.Modified;
                dbe.SaveChanges();

                var acco_enti = new ACCOUNT_ENTITY();

                acco_enti.ID_PERS_ENTI = ID_PERS_ENTI;
                acco_enti.ID_ACCO = 4;
                acco_enti.VIG_ACCO_ENTI = true;
                acco_enti.DEF_ACCO = true;
                acco_enti.VIS_REQU = true;
                acco_enti.VIS_ASSI = false;
                acco_enti.VIS_TALE = true;
                if (ID_PERS_STAT == 1 || ID_PERS_STAT == 2) //EMPLOYEES y TEMPORAL STAFF
                {
                    acco_enti.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                }
                dbe.ACCOUNT_ENTITY.Add(acco_enti);
                dbe.SaveChanges();

                if (ID_PERS_STAT == 1 || ID_PERS_STAT == 2)
                {
                    //Registrando Banco y Cta. de Salary y CTS
                    int ID_BANK_SALA = 0, ID_BANK_CTS = 0, ID_TYPE_PENS = 0;
                    DateTime STAR_DATE_SALA, STAR_DATE_CTS, STAR_DATE_PENS, STAR_DATE_EPS;
                    if (Int32.TryParse(Convert.ToString(Request.Form["ID_BANK_SALA"]), out ID_BANK_SALA) == true)
                    {
                        var pp = new PERSON_PAYMENT();
                        pp.ID_TYPE_PAYM = 1; //SALARY
                        pp.ID_PERS_ENTI = ID_PERS_ENTI;
                        pp.ID_BANK = ID_BANK_SALA;
                        pp.NUM_ACCO = Convert.ToString(Request.Form["NUM_ACCO_SALA"].ToString());
                        if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE_SALA"]), out STAR_DATE_SALA) == true) { pp.DAT_STAR = STAR_DATE_SALA; };
                        dbe.PERSON_PAYMENT.Add(pp);
                        dbe.SaveChanges();
                    }
                    if (Int32.TryParse(Convert.ToString(Request.Form["ID_BANK_CTS"]), out ID_BANK_CTS) == true)
                    {
                        var pp = new PERSON_PAYMENT();
                        pp.ID_TYPE_PAYM = 2; //CTS
                        pp.ID_PERS_ENTI = ID_PERS_ENTI;
                        pp.ID_BANK = ID_BANK_CTS;
                        pp.NUM_ACCO = Convert.ToString(Request.Form["NUM_ACCO_CTS"].ToString());
                        if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE_CTS"]), out STAR_DATE_CTS) == true) { pp.DAT_STAR = STAR_DATE_CTS; };
                        dbe.PERSON_PAYMENT.Add(pp);
                        dbe.SaveChanges();
                    }
                    //Registrando Pension: AFP o ONP
                    if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_PENS"]), out ID_TYPE_PENS) == true)
                    {
                        var p = new PENSION();
                        p.ID_PERS_ENTI = ID_PERS_ENTI;
                        p.ID_TYPE_PENS = ID_TYPE_PENS;
                        p.CUSPP = Convert.ToString(Request.Form["CUSPP"].ToString()).ToUpper();
                        if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE_PENS"]), out STAR_DATE_PENS) == true) { p.START_DATE = STAR_DATE_PENS; };
                        dbe.PENSIONs.Add(p);
                        dbe.SaveChanges();
                    }
                    //Registrando EPS
                    if (pers_enti.EPS == true)
                    {
                        var eps = new EPSALUD();

                        int ID_PLAN_EPS, NUM_FAMI = 0;
                        eps.ID_PERS_ENTI = ID_PERS_ENTI;
                        if (Int32.TryParse(Convert.ToString(Request.Form["ID_PLAN_EPS"]), out ID_PLAN_EPS) == true) { eps.ID_PLAN_EPS = ID_PLAN_EPS; }
                        if (Int32.TryParse(Convert.ToString(Request.Form["NUM_FAMI"]), out NUM_FAMI) == true) { eps.NUMBER_FAMI = NUM_FAMI; }
                        if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE_EPS"]), out STAR_DATE_EPS) == true) { eps.STAR_DATE = STAR_DATE_EPS; };
                        eps.LAST_EPS = true;
                        dbe.EPSALUDs.Add(eps);
                        dbe.SaveChanges();
                    }

                    //Registrando Location
                    var pl = new PERSON_LOCATION();
                    pl.ID_LOCA = ID_LOCA;
                    pl.ID_PERS_ENTI = ID_PERS_ENTI;
                    pl.STAR_DATE = DateTime.Now;
                    pl.VIG_LOCA = true;
                    dbe.PERSON_LOCATION.Add(pl);
                    dbe.SaveChanges();
                    int ID_PERS_LOCA = Convert.ToInt32(pl.ID_PERS_LOCA);

                    //Registrando nuevo periodo laboral
                    var wp = new WORK_PERIOD();
                    wp.STAR_DATE = cont.STAR_DATE;
                    dbe.WORK_PERIOD.Add(wp);
                    dbe.SaveChanges();
                    int ID_WORK_PERI = Convert.ToInt32(wp.ID_WORK_PERI);

                    //Registrando el contrato
                    cont.ID_PERS_ENTI = ID_PERS_ENTI;
                    cont.ID_WORK_PERI = ID_WORK_PERI;
                    cont.VIG_CONT = true;
                    cont.LAS_CONT = true;
                    cont.ID_PERS_LOCA = ID_PERS_LOCA;
                    cont.ID_CHAR = ID_CHAR;
                    cont.ID_PERS_STAT = ID_PERS_STAT;
                    dbe.PERSON_CONTRACT.Add(cont);
                    dbe.SaveChanges();

                    //Registrado PERSON_CONTRACT_CHART
                    int ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                    var pcch = new PERSON_CONTRACT_CHART();
                    pcch.ID_PERS_CONT = ID_PERS_CONT;
                    pcch.ID_CHAR = ID_CHAR;
                    pcch.IS_CONTRACT = true;
                    pcch.VIG_CONT_CHAR = true;
                    dbe.PERSON_CONTRACT_CHART.Add(pcch);
                    dbe.SaveChanges();

                    //enviando correo
                    //SendEmailContract(ID_PERS_ENTI, 1);


                    //Guardando archivo adjunto del contrato

                    if (filesDocs != null)
                    {
                        foreach (var file in filesDocs)
                        {
                            var ATTA = new PERSON_DOCUMENTS();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                            ATTA.CREATE_ATTA = DateTime.Now;
                            ATTA.ID_TYPE_DOCU = 2;
                            ATTA.ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                            ATTA.UserId = Convert.ToInt32(Session["UserId"]);

                            dbe.PERSON_DOCUMENTS.Add(ATTA);
                            dbe.SaveChanges();
                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Documents/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_PERS_DOCU) + ATTA.EXT_ATTA));
                        }
                    }
                }
                #region Creación de Usuario ITMS
                string usernameITMS = string.Empty;
                string passwordITMS = string.Empty;

                usernameITMS = QuitAccents(S_FIR_NAME.Trim().Substring(0, 1).ToLower() + S_LAS_NAME.Trim().ToLower());
                //passwordITMS = ConfigurationManager.AppSettings["clave"].ToString() + DateTime.Now.Year;
                gth objectGth = new gth();
                passwordITMS = objectGth.GenerarContrasenia(); //ConfigurationManager.AppSettings["clave"].ToString() + cl_enti.NUM_TYPE_DI.ToString();

                var query = dbe.RH_VALIDATE_ACCOUNT_ITMS(usernameITMS).ToList();
                if (query.Count() == 0)
                {
                    //Creación en tabla UserProfile
                    WebSecurity.CreateUserAndAccount(usernameITMS, passwordITMS);
                    //WebSecurity.Login(usernameITMS, passwordITMS);
                }
                else
                {
                    if (S_MOT_NAME != null || S_MOT_NAME == "")
                    {
                        usernameITMS = usernameITMS + S_MOT_NAME.Trim().Substring(0, 1).ToLower();
                        var user = dbe.Usuarios.Where(x => x.Usuario1 == usernameITMS).ToList();
                        if (user.Count() == 0)
                        {
                            WebSecurity.CreateUserAndAccount(usernameITMS.ToLower(), passwordITMS);
                        }
                        else
                        {
                            usernameITMS = S_FIR_NAME.Trim().ToLower() + "." + S_LAS_NAME.Trim().ToLower();
                            WebSecurity.CreateUserAndAccount(usernameITMS.ToLower(), passwordITMS);
                        }
                        //WebSecurity.Login(usernameITMS, passwordITMS);
                    }
                    else
                    {
                        usernameITMS = S_FIR_NAME.Trim().ToLower() + "." + S_LAS_NAME.Trim().ToLower();
                        WebSecurity.CreateUserAndAccount(usernameITMS.ToLower(), passwordITMS);
                        //WebSecurity.Login(usernameITMS, passwordITMS);
                    }
                }

                #region Actualizacion de  idUser de la tabla UserProfile a la tabla CLASS_ENTITY
                var cargarUP = dbe.RH_VALIDATE_USERPROFILE(usernameITMS).First();
                UserId = cargarUP.UserId;
                cl_enti.UserId = UserId;
                dbe.CLASS_ENTITY.Attach(cl_enti);
                dbe.Entry(cl_enti).State = EntityState.Modified;
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

                //Registrando en PERSON_STATUS_CHANGE
                var psc = new PERSON_STATUS_CHANGE();
                psc.ID_PERS_ENTI = ID_PERS_ENTI;
                psc.ID_PERS_STAT = ID_PERS_STAT;
                psc.ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                psc.STAR_DATE = DateTime.Now;
                psc.UserId = Convert.ToInt32(Session["UserId"]);
                dbe.PERSON_STATUS_CHANGE.Add(psc);
                dbe.SaveChanges();

                string LugarTrabajo = "", Cargo = "";
                try
                {
                    Cargo = (from c in dbe.CHARTs.Where(x => x.ID_CHAR == ID_CHAR)
                             join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                             select new
                             {
                                 nc.NAM_CHAR
                             }).SingleOrDefault().NAM_CHAR;

                    LugarTrabajo = (from l in dbc.LOCATIONs.Where(x => x.ID_LOCA == ID_LOCA)
                                    join s in dbc.SITEs on l.ID_SITE equals s.ID_SITE
                                    select new
                                    {
                                        Lugar = s.NAM_SITE + " " + l.NAM_LOCA
                                    }).SingleOrDefault().Lugar;
                }
                catch { }

                #region Enviar Correo de Confirmacion de Creacion de Usuario ITMS
                SendMail mail = new SendMail();
                mail.CreacionUsuarioEData(ConfigurationManager.AppSettings["Mailrrhh"].ToString(), S_FIR_NAME.Trim() + " " + S_LAS_NAME.Trim() + " " + S_MOT_NAME, usernameITMS, passwordITMS, S_EMA_ENTI.Trim(), Cargo, LugarTrabajo);
                #endregion

                //return Content("<script type='text/javascript'> function init() {" +
                //      "if(top.uploadDone) top.uploadDone('OK','" + ID_PERS_ENTI.ToString() + "');}window.onload=init;</script>");

                return Content("<script type='text/javascript'>function init() {" +
                    "if(top.mensaje){ top.mensaje('OK'," + ID_PERS_ENTI.ToString() + ");}}window.onload=init;</script>");
            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'>function init() {" +
                    "if(top.mensaje){ top.mensaje('ERROR','Ha ocurrido un error, contacte al administrador');}}window.onload=init;</script>");
                //return Content("<script type='text/javascript'>function init() {" +
                //        "if(top.uploadDoneProfile){ top.uploadDoneProfile('ERROR','" + "0" + "','" + ex.Message + "');}}window.onload=init;</script>");
            }
        }

        [Authorize]
        public ActionResult Find(int id = 0)
        {
            int flagAdminTalent = 0;
            int ID_ACCO = 0;
            int ID_CIA = 0;
            string ID_PERS_ENTI = string.Empty;
            ID_PERS_ENTI = Convert.ToString(Session["ID_PERS_ENTI"]);
            ViewBag.ID_PERS_ENTI = ID_PERS_ENTI;
            // Usuarios Administradores de RRHH
            flagAdminTalent = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 17 && x.VAL_ACCO_PARA == ID_PERS_ENTI && x.VIG_ACCO_PARA == true).Count();
            if (flagAdminTalent != 0)
                ViewBag.Supervisor = 1;
            else
                ViewBag.Supervisor = id;
            ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ID_CIA = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 6 && x.ID_ACCO == ID_ACCO).VAL_ACCO_PARA);
            ViewBag.ID_CIA = ID_CIA;
            return View();
        }

        public ActionResult FindResult()
        {
            string S_FIR_NAME = Convert.ToString(Request.Params["FIR_NAME"].ToString());
            string S_LAS_NAME = Convert.ToString(Request.Params["LAS_NAME"].ToString());
            string S_CEL_PERS = Convert.ToString(Request.Params["CEL_ENTI"].ToString());
            string S_EXT_PERS = Convert.ToString(Request.Params["EXT_ENTI"].ToString());
            string S_EMA_ENTI = Convert.ToString(Request.Params["EMA_ENTI"].ToString());
            string S_EMA_ELEC = Convert.ToString(Request.Params["EMA_ELEC"].ToString());
            string S_EMA_PERS = Convert.ToString(Request.Params["EMA_PERS"].ToString());
            string S_ID_TYPE_CHAR = Convert.ToString(Request.Params["ID_TYPE_CHAR"].ToString());

            string S_ID_LOCA = null, S_ID_PERS_STAT = null, S_ID_COND_CONT = null;

            int ID_LOCA, ID_PERS_STAT, ID_COND_CONT, ID_CHAR, ID_TYPE_CHAR, ID_NAM_CHAR;
            DateTime START_DATE, END_DATE;

            //try { S_ID_CARG = Convert.ToString(Request.Params["ID_CARG"].ToString()); }
            //catch { }

            //try { S_ID_AREA = Convert.ToString(Request.Params["ID_AREA"].ToString()); }
            //catch { }

            try { S_ID_LOCA = Convert.ToString(Request.Params["ID_LOCA"].ToString()); }
            catch { }

            try { S_ID_PERS_STAT = Convert.ToString(Request.Params["ID_PERS_STAT1"].ToString()); }
            catch { }

            try { S_ID_COND_CONT = Convert.ToString(Request.Params["ID_COND_CONT1"].ToString()); }
            catch { }

            //try { S_ID_TYPE_CHAR = Convert.ToString(Request.Params["ID_TYPE_CHAR"].ToString()); }
            //catch { }

            //try { S_ID_CHAR = Convert.ToString(Request.Params["ID_CHAR"].ToString()); }
            //catch { }

            //try { S_ID_NAM_CHAR = Convert.ToString(Request.Params["ID_NAM_CHAR"].ToString()); }
            //catch { }

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int Count = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //x.ID_PARA == 18: CIA SUPPLY(Compania con el personal)
            //int ID_ENTI = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA);
            var queryCIA = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 6 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true);
            int ID_ENTI = Convert.ToInt32(queryCIA.VAL_ACCO_PARA);

            var cargo_area = dbe.PERSON_CONTRACT.Where(pc => pc.LAS_CONT == true)
                .Join(dbe.CHARTs, pc => pc.ID_CHAR, ch => ch.ID_CHAR, (pc, ch) => new
                {
                    pc.ID_PERS_ENTI,
                    pc.ID_COND_CONT,
                    pc.END_DATE,
                    pc.STAR_DATE,
                    pc.ID_WORK_PERI,
                    pc.ID_CHAR,
                    ch.ID_CHAR_PARE,
                    ch.ID_NAM_CHAR,
                })
                .Join(dbe.NAME_CHART, x => x.ID_NAM_CHAR, nc => nc.ID_NAM_CHAR, (x, nc) => new
                {
                    x.ID_PERS_ENTI,
                    x.ID_COND_CONT,
                    x.END_DATE,
                    x.STAR_DATE,
                    x.ID_WORK_PERI,
                    x.ID_CHAR,
                    x.ID_CHAR_PARE,
                    x.ID_NAM_CHAR,
                    nc.NAM_CHAR,
                })
                .Join(dbe.CONTRACT_CONDITION, x => x.ID_COND_CONT, cc => cc.ID_COND_CONT, (x, cc) => new
                {
                    x.ID_PERS_ENTI,
                    x.ID_COND_CONT,
                    x.END_DATE,
                    x.STAR_DATE,
                    x.ID_WORK_PERI,
                    x.ID_CHAR,
                    x.ID_CHAR_PARE,
                    x.ID_NAM_CHAR,
                    x.NAM_CHAR,
                    cc.CONDITION
                }).ToList();

            var site_loca = dbc.SITEs
                .Join(dbc.LOCATIONs, s => s.ID_SITE, l => l.ID_SITE, (s, l) => new
                {
                    s.NAM_SITE,
                    l.NAM_LOCA,
                    l.ID_LOCA,
                    UBIC = s.NAM_SITE + " - " + l.NAM_LOCA
                }).ToList();

            var query = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_ENTI1 == ID_ENTI).ToList()
                         join ps in dbe.PERSON_STATUS on pe.ID_PERS_STAT equals ps.ID_PERS_STAT
                         join ae in dbe.ACCOUNT_ENTITY.Where(x => x.ID_ACCO == ID_ACCO).Where(x => x.VIS_TALE == true)
                         on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI

                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI

                         join ca in cargo_area on pe.ID_PERS_ENTI equals ca.ID_PERS_ENTI into lca
                         from xpc in lca.DefaultIfEmpty()

                         join pl in dbe.PERSON_LOCATION.Where(x => x.END_DATE == null) on pe.ID_PERS_ENTI equals pl.ID_PERS_ENTI into lpl
                         from xpl in lpl.DefaultIfEmpty()

                         join sl in site_loca on xpl == null ? null : xpl.ID_LOCA equals sl.ID_LOCA into lsl
                         from xsl in lsl.DefaultIfEmpty()

                         select new
                         {
                             NAM_STAT = ps.NAM_STAT.ToLower(),
                             pe.ID_PERS_ENTI,
                             ae.ID_ACCO,
                             pe.ID_ENTI1,
                             pe.ID_ENTI2,
                             FIR_NAME = ce.FIR_NAME.ToLower(),
                             LAS_NAME = ce.LAS_NAME.ToLower(),
                             pe.ID_ACCO_PERT,
                             EMA_PERS = pe.EMA_PERS == null ? "-" : pe.EMA_PERS.ToLower(),
                             EMA_ELEC = pe.EMA_ELEC == null ? "-" : pe.EMA_ELEC.ToLower(),
                             pe.CEL_PERS,
                             pe.TEL_PERS,
                             pe.ID_FOTO,
                             ID_ENTI = ce.ID_ENTI,
                             pe.EXT_PERS,
                             EMA_ENTI = ce.EMA_ENTI,
                             pe.ID_PERS_STAT,
                             ID_WORK_PERI = (xpc == null ? 0 : xpc.ID_WORK_PERI),
                             CONDITION = (xpc == null ? "" : xpc.CONDITION.ToLower()),
                             ID_CHAR = (xpc == null ? 0 : xpc.ID_CHAR),
                             ID_CHAR_PARE = (xpc == null ? 0 : xpc.ID_CHAR_PARE),
                             NAM_CHAR = (xpc == null ? "-" : xpc.NAM_CHAR.ToLower()),
                             ID_COND_CONT = (xpc == null ? 0 : xpc.ID_COND_CONT),
                             STAR_DATE = (xpc == null ? null : xpc.STAR_DATE),
                             END_DATE = (xpc == null ? null : xpc.END_DATE),
                             FOTO = pe.ID_FOTO,
                             ID_LOCA = (xsl == null ? 0 : xsl.ID_LOCA),
                             NAM_SITE = (xsl == null ? "" : xsl.NAM_SITE),
                             NAM_LOCA = (xsl == null ? "" : xsl.NAM_LOCA),
                             UBIC = (xsl == null ? "" : xsl.UBIC),
                             usuario = ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower(),
                         });

            Count = query.Count();

            if (ID_ACCO != 4)
            {
                query = query.Where(x => x.ID_ACCO_PERT == ID_ACCO);
            }

            if (!String.IsNullOrEmpty(S_FIR_NAME))
            {
                query = query.Where(a => a.FIR_NAME.ToUpper().Contains(S_FIR_NAME.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_LAS_NAME))
            {
                query = query.Where(a => a.LAS_NAME.ToUpper().Contains(S_LAS_NAME.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_CEL_PERS))
            {
                query = query.Where(a => a.CEL_PERS.Contains(S_CEL_PERS));
            }
            if (!String.IsNullOrEmpty(S_EXT_PERS))
            {
                query = query.Where(a => a.EXT_PERS.Contains(S_EXT_PERS));
            }
            if (!String.IsNullOrEmpty(S_EMA_ENTI))
            {
                query = query.Where(a => a.EMA_ENTI.ToUpper().Contains(S_EMA_ENTI.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_EMA_ELEC))
            {
                query = query.Where(a => a.EMA_ELEC.ToUpper().Contains(S_EMA_ELEC.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_EMA_PERS))
            {
                query = query.Where(a => a.EMA_PERS.ToUpper().Contains(S_EMA_PERS.ToUpper()));
            }
            if (Int32.TryParse(S_ID_TYPE_CHAR, out ID_TYPE_CHAR))
            {
                if (ID_TYPE_CHAR == 0)
                {
                    string S_ID_CHAR = Convert.ToString(Request.Params["ID_CHART"].ToString());
                    if (Int32.TryParse(S_ID_CHAR, out ID_CHAR))
                    {
                        if (ID_CHAR != 0)
                        {
                            query = query.Where(a => a.ID_CHAR == ID_CHAR);
                        }
                    }
                }
                else
                {
                    string S_ID_NAM_CHAR = Convert.ToString(Request.Params["ID_NAM_CHAR"].ToString());
                    if (Int32.TryParse(S_ID_NAM_CHAR, out ID_NAM_CHAR))
                    {
                        if (ID_NAM_CHAR != 0)
                        {
                            var recJT = dbe.JOBTITLE_BY_ID_NAM_CHAR(ID_NAM_CHAR).ToList();

                            query = (from a in query
                                     join b in recJT on a.ID_CHAR equals b.ID_CHAR
                                     select a);
                        }
                    }
                }
            }

            //if (Int32.TryParse(S_ID_AREA, out ID_AREA))
            //{

            //    try
            //    {
            //        var area = dbe.AREAs.Where(x => x.ID_AREA_PARENT == ID_AREA).ToList();

            //        if (area.Count()==0)
            //        {
            //            query = query.Where(a => a.ID_AREA == ID_AREA);
            //        }
            //        else
            //        {
            //            int[] array = new int[area.Count()];
            //            int h = 0;
            //            foreach (AREA ar in area)
            //            {
            //                array[h] = Convert.ToInt32(ar.ID_AREA);
            //                h = h + 1;
            //            }
            //            query = query.Where(a => array.Contains((int)a.ID_AREA));
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}

            if (Int32.TryParse(S_ID_PERS_STAT, out ID_PERS_STAT))
            {
                query = query.Where(a => a.ID_PERS_STAT == ID_PERS_STAT);
            }

            if (Int32.TryParse(S_ID_LOCA, out ID_LOCA))
            {
                query = query.Where(x => x.ID_LOCA == ID_LOCA);
            }
            string fec1 = Convert.ToString(Request.Params["START_DATE1"]);
            string fec2 = Convert.ToString(Request.Params["END_DATE1"]);

            //if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE1"]), out START_DATE))
            //{
            //    query = query.Where(t => t.STAR_DATE >= START_DATE);
            //}

            if (Convert.ToString(Request.Params["START_DATE1"]) != "")
            {
                START_DATE = DateTime.ParseExact(fec1, "d/M/yyyy hh:mm tt", null);
                query = query.Where(t => t.STAR_DATE >= START_DATE);
            }

            if (Convert.ToString(Request.Params["END_DATE1"]) != "")
            {
                END_DATE = DateTime.ParseExact(fec2, "d/M/yyyy hh:mm tt", null);
                END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                query = query.Where(t => t.END_DATE <= END_DATE);
            }

            if (Int32.TryParse(S_ID_COND_CONT, out ID_COND_CONT))
            {
                query = query.Where(t => t.ID_COND_CONT == ID_COND_CONT);
            }

            Count = query.Count();

            var bar = (from g in query.ToList()
                       group g by new { g.ID_PERS_STAT, g.NAM_STAT } into g
                       select new
                       {
                           ID_PERS_STAT = g.Key.ID_PERS_STAT,
                           name = g.Key.NAM_STAT,
                           cantidad = g.Count()
                       });

            var pie = (from g in query.ToList()
                       join cc in dbe.CONTRACT_CONDITION on g.ID_COND_CONT equals cc.ID_COND_CONT
                       group cc by new { cc.ID_COND_CONT, cc.CONDITION } into g
                       select new
                       {
                           name = g.Key.CONDITION,
                           y = g.Count()
                       });

            var result = (from q in query.OrderBy(x => x.usuario).Skip(skip).Take(take)

                              //join a in dbe.AREAs on q.ID_AREA equals a.ID_AREA into la
                              //from xa in la.DefaultIfEmpty()

                              //join cg in dbe.CARGOes on q.ID_CARG equals cg.ID_CARG into lcg
                              //from xcg in lcg.DefaultIfEmpty()

                          join wp in dbe.WORK_PERIOD on q.ID_WORK_PERI equals wp.ID_WORK_PERI into lwp
                          from xwp in lwp.DefaultIfEmpty()

                          select new
                          {
                              usuario = q.usuario,
                              NAM_STAT = q.NAM_STAT.ToLower(),
                              CAR_PERS = q.NAM_CHAR,//(xcg == null ? "-" : xcg.NAM_CARG.ToLower()),
                              ID_CIA = q.ID_ENTI1,
                              ID_ENTI = q.ID_ENTI,
                              FOTO = q.FOTO,
                              EMA_PERS = q.EMA_PERS == null ? "-" : q.EMA_PERS.ToLower(),
                              EMA_ELEC = q.EMA_ELEC == null ? "-" : q.EMA_ELEC.Trim().Length == 0 ? "-" : q.EMA_ELEC.ToLower(),
                              CEL_PERS = q.CEL_PERS == null ? "-" : q.CEL_PERS,
                              TEL_PERS = q.TEL_PERS == null ? "-" : q.TEL_PERS,
                              EXT_PERS = q.EXT_PERS == null ? "-" : q.EXT_PERS,
                              NOM_AREA = FindNodo(Convert.ToInt32((q.ID_CHAR_PARE == null ? 0 : q.ID_CHAR_PARE)), 2),
                              SITIO = q.UBIC == null ? "-" : q.UBIC.ToLower(),
                              USB = FindNodo(Convert.ToInt32((q.ID_CHAR_PARE == null ? 0 : q.ID_CHAR_PARE)), 1),
                              STAR_CONT = q.STAR_DATE == null ? "-" : String.Format("{0:d/M/yyyy}", q.STAR_DATE),
                              END_CONT = q.END_DATE == null ? "-" : String.Format("{0:d/M/yyyy}", q.END_DATE),
                              CESS_DATE = xwp == null ? "-" : String.Format("{0:d/M/yyyy}", xwp.CESS_DATE),
                              CONDITION = q.CONDITION == null ? "-" : q.CONDITION.ToLower(),
                              ID_PERS_ENTI = q.ID_PERS_ENTI,
                              ID_PERS_STAT = q.ID_PERS_STAT,
                          }).ToList();

            return Json(new { Data = result, Count = Count, Barras = bar, Pie = pie }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult FindResultGraph()
        {
            textInfo = cultureInfo.TextInfo;

            string S_FIR_NAME = Convert.ToString(Request.Params["FIR_NAME"].ToString());
            string S_LAS_NAME = Convert.ToString(Request.Params["LAS_NAME"].ToString());
            string S_CEL_PERS = Convert.ToString(Request.Params["CEL_ENTI"].ToString());
            string S_EXT_PERS = Convert.ToString(Request.Params["EXT_ENTI"].ToString());
            string S_EMA_ENTI = Convert.ToString(Request.Params["EMA_ENTI"].ToString());
            string S_EMA_ELEC = Convert.ToString(Request.Params["EMA_ELEC"].ToString());
            string S_EMA_PERS = Convert.ToString(Request.Params["EMA_PERS"].ToString());

            string S_ID_CARG = null, S_ID_AREA = null, S_ID_LOCA = null, S_ID_PERS_STAT = null, S_ID_COND_CONT = null;

            int ID_CARG, ID_AREA, ID_LOCA, ID_PERS_STAT, ID_COND_CONT;
            DateTime START_DATE, END_DATE;

            try { S_ID_CARG = Convert.ToString(Request.Params["ID_CARG"].ToString()); }
            catch { }

            try { S_ID_AREA = Convert.ToString(Request.Params["ID_AREA"].ToString()); }
            catch { }

            try { S_ID_LOCA = Convert.ToString(Request.Params["ID_LOCA"].ToString()); }
            catch { }

            try { S_ID_PERS_STAT = Convert.ToString(Request.Params["ID_PERS_STAT1"].ToString()); }
            catch { }

            try { S_ID_COND_CONT = Convert.ToString(Request.Params["ID_COND_CONT1"].ToString()); }
            catch { }

            //int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            //int take = Convert.ToInt32(Request.Params["take"].ToString());
            int Count = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //x.ID_PARA == 18: CIA SUPPLY(Compania con el personal)
            //int ID_ENTI = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA);
            var queryCIA = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO);
            int ID_ENTI = Convert.ToInt32(queryCIA.VAL_ACCO_PARA);

            var cargo_area = dbe.PERSON_CONTRACT.Where(pc => pc.LAS_CONT == true)
                .Join(dbe.CARGO_AREA, pc => pc.ID_CARG_AREA, ca => ca.ID_CARG_AREA, (pc, ca) => new
                {
                    ca.ID_AREA,
                    ca.ID_CARG,
                    pc.ID_PERS_ENTI,
                    pc.ID_COND_CONT,
                    pc.END_DATE,
                    pc.STAR_DATE,
                    pc.ID_WORK_PERI,
                })
                .Join(dbe.CONTRACT_CONDITION, x => x.ID_COND_CONT, cc => cc.ID_COND_CONT, (x, cc) => new
                {
                    x.ID_AREA,
                    x.ID_CARG,
                    x.ID_PERS_ENTI,
                    x.ID_COND_CONT,
                    x.END_DATE,
                    x.STAR_DATE,
                    x.ID_WORK_PERI,
                    cc.CONDITION
                }).ToList();

            var site_loca = dbc.SITEs
                .Join(dbc.LOCATIONs, s => s.ID_SITE, l => l.ID_SITE, (s, l) => new
                {
                    s.NAM_SITE,
                    l.NAM_LOCA,
                    l.ID_LOCA,
                    UBIC = s.NAM_SITE + " - " + l.NAM_LOCA
                }).ToList();


            var query = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_ENTI1 == ID_ENTI).ToList()
                         join ps in dbe.PERSON_STATUS on pe.ID_PERS_STAT equals ps.ID_PERS_STAT
                         join ae in dbe.ACCOUNT_ENTITY.Where(x => x.ID_ACCO == ID_ACCO).Where(x => x.VIS_TALE == true)
                         on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI

                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI

                         join ca in cargo_area on pe.ID_PERS_ENTI equals ca.ID_PERS_ENTI into lca
                         from xpc in lca.DefaultIfEmpty()

                         join pl in dbe.PERSON_LOCATION.Where(x => x.END_DATE == null) on pe.ID_PERS_ENTI equals pl.ID_PERS_ENTI into lpl
                         from xpl in lpl.DefaultIfEmpty()

                         join sl in site_loca on xpl == null ? null : xpl.ID_LOCA equals sl.ID_LOCA into lsl
                         from xsl in lsl.DefaultIfEmpty()

                         select new
                         {
                             NAM_STAT = ps.NAM_STAT,
                             pe.ID_PERS_ENTI,
                             ae.ID_ACCO,
                             pe.ID_ENTI2,
                             FIR_NAME = ce.FIR_NAME,
                             LAS_NAME = ce.LAS_NAME,
                             pe.ID_ACCO_PERT,
                             pe.EMA_PERS,
                             pe.EMA_ELEC,
                             pe.CEL_PERS,
                             pe.TEL_PERS,
                             pe.ID_FOTO,
                             ID_ENTI = ce.ID_ENTI,
                             pe.EXT_PERS,
                             EMA_ENTI = ce.EMA_ENTI,
                             pe.ID_PERS_STAT,
                             ID_WORK_PERI = (xpc == null ? 0 : xpc.ID_WORK_PERI),
                             CONDITION = (xpc == null ? "" : xpc.CONDITION),
                             ID_CARG = (xpc == null ? 0 : xpc.ID_CARG),
                             ID_AREA = (xpc == null ? 0 : xpc.ID_AREA),
                             ID_COND_CONT = (xpc == null ? 0 : xpc.ID_COND_CONT),
                             STAR_DATE = (xpc == null ? null : xpc.STAR_DATE),
                             END_DATE = (xpc == null ? null : xpc.END_DATE),
                             FOTO = pe.ID_FOTO,
                             ID_LOCA = (xsl == null ? 0 : xsl.ID_LOCA),
                             NAM_SITE = (xsl == null ? "" : xsl.NAM_SITE),
                             NAM_LOCA = (xsl == null ? "" : xsl.NAM_LOCA),
                             UBIC = (xsl == null ? "" : xsl.UBIC),
                             usuario = ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower(),
                         });

            Count = query.Count();

            if (ID_ACCO != 4)
            {
                query = query.Where(x => x.ID_ACCO_PERT == ID_ACCO);
            }

            if (!String.IsNullOrEmpty(S_FIR_NAME))
            {
                query = query.Where(a => a.FIR_NAME.ToUpper().Contains(S_FIR_NAME.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_LAS_NAME))
            {
                query = query.Where(a => a.LAS_NAME.ToUpper().Contains(S_LAS_NAME.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_CEL_PERS))
            {
                query = query.Where(a => a.CEL_PERS.Contains(S_CEL_PERS));
            }
            if (!String.IsNullOrEmpty(S_EXT_PERS))
            {
                query = query.Where(a => a.EXT_PERS.Contains(S_EXT_PERS));
            }
            if (!String.IsNullOrEmpty(S_EMA_ENTI))
            {
                query = query.Where(a => a.EMA_ENTI.ToUpper().Contains(S_EMA_ENTI.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_EMA_ELEC))
            {
                query = query.Where(a => a.EMA_ELEC.ToUpper().Contains(S_EMA_ELEC.ToUpper()));
            }
            if (!String.IsNullOrEmpty(S_EMA_PERS))
            {
                query = query.Where(a => a.EMA_PERS.ToUpper().Contains(S_EMA_PERS.ToUpper()));
            }

            if (Int32.TryParse(S_ID_CARG, out ID_CARG))
            {
                query = query.Where(a => a.ID_CARG == ID_CARG);
            }

            if (Int32.TryParse(S_ID_AREA, out ID_AREA))
            {

                try
                {
                    var area = dbe.AREAs.Where(x => x.ID_AREA_PARENT == ID_AREA).ToList();

                    if (area.Count() == 0)
                    {
                        query = query.Where(a => a.ID_AREA == ID_AREA);
                    }
                    else
                    {
                        int[] array = new int[area.Count()];
                        int h = 0;
                        foreach (AREA ar in area)
                        {
                            array[h] = Convert.ToInt32(ar.ID_AREA);
                            h = h + 1;
                        }
                        query = query.Where(a => array.Contains((int)a.ID_AREA));
                    }
                }
                catch
                {
                }
            }

            if (Int32.TryParse(S_ID_PERS_STAT, out ID_PERS_STAT))
            {
                query = query.Where(a => a.ID_PERS_STAT == ID_PERS_STAT);
            }

            if (Int32.TryParse(S_ID_LOCA, out ID_LOCA))
            {
                query = query.Where(x => x.ID_LOCA == ID_LOCA);
            }
            string fec1 = Convert.ToString(Request.Params["START_DATE1"]);
            string fec2 = Convert.ToString(Request.Params["END_DATE1"]);

            if (Convert.ToString(Request.Params["START_DATE1"]) != "")
            {
                START_DATE = DateTime.ParseExact(fec1, "d/M/yyyy hh:mm tt", null);
                query = query.Where(t => t.STAR_DATE >= DateTime.ParseExact(fec1, "d/M/yyyy hh:mm tt", null));
            }

            if (Convert.ToString(Request.Params["END_DATE1"]) != "")
            {
                END_DATE = DateTime.ParseExact(fec2, "d/M/yyyy hh:mm tt", null);
                END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                query = query.Where(t => t.END_DATE <= END_DATE);
            }

            if (Int32.TryParse(S_ID_COND_CONT, out ID_COND_CONT))
            {
                query = query.Where(t => t.ID_COND_CONT == ID_COND_CONT);
            }

            Count = query.Count();


            var bar = (from g in query.ToList()
                       group g by new { g.ID_PERS_STAT, g.NAM_STAT } into g
                       select new
                       {
                           ID_PERS_STAT = g.Key.ID_PERS_STAT,
                           name = textInfo.ToTitleCase(g.Key.NAM_STAT.ToLower()),
                           cantidad = g.Count()
                       }).OrderByDescending(x => x.cantidad);

            var pie = (from g in query.ToList()
                       join cc in dbe.CONTRACT_CONDITION on g.ID_COND_CONT equals cc.ID_COND_CONT
                       group cc by new { cc.ID_COND_CONT, cc.CONDITION } into g
                       select new
                       {
                           name = textInfo.ToTitleCase(g.Key.CONDITION.ToLower()),
                           y = g.Count()
                       });

            return Json(new { Data = bar, Data1 = pie }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeStatus(int id, int id1, int id2, string proc = "")
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.ID_PERS_STAT = id1;
            ViewBag.ID_ENTI = id2;
            ViewBag.Procedencia = proc;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ChangeStatus(IEnumerable<HttpPostedFileBase> filesDocs, IEnumerable<HttpPostedFileBase> fileAttaCess, PERSON_STATUS PerStat)
        {
            int sw = 0, Error = 0, ID_PERS_STAT = 0, ID_COND_CONT = 0, ID_REAS_END = 0,
                ID_CARG_AREA = 0, ID_WORK_PERI, ID_LOCA = 0, ID_CHAR = 0, ID_PERS_ENTI_TEMP = 0;
            DateTime STAR_DATE, END_DATE, CESS_DATE = DateTime.Now;
            decimal GROSS_SALARY = 0;
            int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);
            int ID_USER = Convert.ToInt32(Session["UserId"]);
            var cont = new PERSON_CONTRACT();

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_STAT"]), out ID_PERS_STAT) == false) { sw = 1; }
            if (ID_PERS_STAT == 1 || ID_PERS_STAT == 2)
            {
                ID_CHAR = Convert.ToInt32(Request.Form["ID_CHAR"]);

                if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE"]), out STAR_DATE) == false) { sw = 1; }
                else if (DateTime.TryParse(Convert.ToString(Request.Form["END_DATE"]), out END_DATE) == false) { sw = 1; }
                else if (STAR_DATE > END_DATE) { sw = 1; Error = 2; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_COND_CONT"]), out ID_COND_CONT) == false) { sw = 1; }
                else if (ID_CHAR == 0) { sw = 1; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_LOCATION"]), out ID_LOCA) == false) { sw = 1; }
                //else if (Decimal.TryParse(Convert.ToString(Request.Form["GROSS_SALARY"]), out GROSS_SALARY) == false) { sw = 1; }
                //else if (filesDocs == null) { sw = 1; Error = 3; }
                else
                {
                    cont.STAR_DATE = STAR_DATE;
                    cont.END_DATE = END_DATE;
                    cont.ID_COND_CONT = ID_COND_CONT;
                    cont.GROSS_SALARY = GROSS_SALARY;
                    cont.ID_CARG_AREA = ID_CARG_AREA;
                    cont.ID_PERS_STAT = ID_PERS_STAT;
                    cont.ID_CHAR = ID_CHAR;
                }
            }
            else if (ID_PERS_STAT == 3)
            {
                if (DateTime.TryParse(Convert.ToString(Request.Form["CESS_DATE"]), out CESS_DATE) == false) { sw = 1; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_REAS_END"]), out ID_REAS_END) == false) { sw = 1; }
                else
                {
                    //Verificando si hay personal que le reportan
                    //int ID_PERS = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);
                    //var qReport = (from a in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS
                    //                         && x.VIG_CONT == true && x.LAS_CONT == true)
                    //               join b in dbe.CHARTs on a.ID_CHAR equals b.ID_CHAR_PARE
                    //               join c in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true)
                    //                         on b.ID_CHAR equals c.ID_CHAR
                    //               select new
                    //               {
                    //                   c.ID_PERS_ENTI,
                    //               });
                    //if (qReport.Count() > 0)
                    //{
                    //    if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_ENTI_TEMP"]), out ID_PERS_ENTI_TEMP) == false) { sw = 1; }
                    //}
                    //Verifica si tiene activos asignados
                    int activosAsignados = Convert.ToInt32(dbe.ValidaActivosAsignados(ID_PERS_ENTI).SingleOrDefault().CantidadActivos);
                    if (activosAsignados != 0)
                    {
                        sw = 1;
                        Error = 4;
                    }
                }
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensajeActualizacionEstado) top.mensajeActualizacionEstado('ERROR','" + Error + "');}window.onload=init;</script>");
                //return Content("<script type='text/javascript'> function init() {" +
                //    "if(top.uploadDone) top.uploadDone('ERROR','" + Error + "','validacion');}window.onload=init;</script>");
            }
            else
            {
                try
                {
                    var pscLast = dbe.PERSON_STATUS_CHANGE.First(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.END_DATE == null);
                    pscLast.END_DATE = DateTime.Now;
                    dbe.PERSON_STATUS_CHANGE.Attach(pscLast);
                    dbe.Entry(pscLast).State = EntityState.Modified;
                    dbe.SaveChanges();
                }
                catch { }

                var pscNew = new PERSON_STATUS_CHANGE();
                pscNew.ID_PERS_ENTI = ID_PERS_ENTI;
                pscNew.ID_PERS_STAT = ID_PERS_STAT;
                pscNew.STAR_DATE = DateTime.Now;
                pscNew.UserId = ID_USER;
                pscNew.ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                dbe.PERSON_STATUS_CHANGE.Add(pscNew);
                dbe.SaveChanges();

                Boolean flag = true;
                if (ID_PERS_STAT == 3) { flag = false; }
                //Registrando el cambio de estado
                var pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI);
                pe.ID_PERS_STAT = ID_PERS_STAT;
                pe.VIG_PERS_ENTI = flag;
                dbe.PERSON_ENTITY.Attach(pe);
                dbe.Entry(pe).State = EntityState.Modified;
                dbe.SaveChanges();

                //Realizando cambio de estado
                int IdEnti = Convert.ToInt32(dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI).SingleOrDefault().ID_ENTI2);
                var class_entity = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == IdEnti);
                class_entity.VIG_ENTI = flag;
                dbe.CLASS_ENTITY.Attach(class_entity);
                dbe.Entry(class_entity).State = EntityState.Modified;
                dbe.SaveChanges();

                //Registrando contrato si cambia a EMPLOYEES or TEMPORAL STAFF
                if (ID_PERS_STAT == 1 || ID_PERS_STAT == 2)
                {
                    //var contrac = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.VIG_CONT == true);
                    var contrac = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI);
                    if (contrac.Count() > 0)
                    {
                        //var conLast = contrac.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI);
                        var conLast = contrac.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI).OrderByDescending(x => x.ID_PERS_CONT).First();
                        if (conLast.VIG_CONT == false)
                        {
                            var wp = new WORK_PERIOD();
                            wp.STAR_DATE = cont.STAR_DATE;
                            dbe.WORK_PERIOD.Add(wp);
                            dbe.SaveChanges();

                            ID_WORK_PERI = Convert.ToInt32(wp.ID_WORK_PERI);
                        }
                        else { ID_WORK_PERI = Convert.ToInt32(conLast.ID_WORK_PERI); }

                        conLast.VIG_CONT = false;
                        conLast.LAS_CONT = false;
                        dbe.PERSON_CONTRACT.Attach(conLast);
                        dbe.Entry(conLast).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                    else
                    {
                        var wp = new WORK_PERIOD();
                        wp.STAR_DATE = cont.STAR_DATE;
                        dbe.WORK_PERIOD.Add(wp);
                        dbe.SaveChanges();

                        ID_WORK_PERI = Convert.ToInt32(wp.ID_WORK_PERI);
                    }

                    //Registrando el Sitio y la Ubicacion
                    var pl = dbe.PERSON_LOCATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.END_DATE == null);
                    if (pl.Count() > 0)
                    {
                        var plFirst = pl.First();
                        plFirst.END_DATE = CESS_DATE;
                        plFirst.VIG_LOCA = false;

                        dbe.PERSON_LOCATION.Attach(plFirst);
                        dbe.Entry(plFirst).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }

                    var NewPL = new PERSON_LOCATION();
                    NewPL.STAR_DATE = cont.STAR_DATE;
                    NewPL.ID_PERS_ENTI = ID_PERS_ENTI;
                    NewPL.ID_LOCA = ID_LOCA;
                    NewPL.VIG_LOCA = true;
                    dbe.PERSON_LOCATION.Add(NewPL);
                    dbe.SaveChanges();

                    //Registrado PERSON_CONTRACT
                    cont.ID_PERS_LOCA = NewPL.ID_PERS_LOCA;
                    cont.ID_PERS_ENTI = ID_PERS_ENTI;
                    cont.VIG_CONT = true;
                    cont.LAS_CONT = true;
                    cont.ID_WORK_PERI = ID_WORK_PERI;
                    dbe.PERSON_CONTRACT.Add(cont);
                    dbe.SaveChanges();

                    //Registrado PERSON_CONTRACT_CHART
                    int ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                    var pcch = new PERSON_CONTRACT_CHART();
                    pcch.ID_PERS_CONT = ID_PERS_CONT;
                    pcch.ID_CHAR = ID_CHAR;
                    pcch.IS_CONTRACT = true;
                    pcch.VIG_CONT_CHAR = true;
                    dbe.PERSON_CONTRACT_CHART.Add(pcch);
                    dbe.SaveChanges();

                    //enviando correo
                    SendEmailContract(ID_PERS_ENTI, 1);

                    //Guardando archivo adjunto del contrato
                    if (filesDocs != null)
                    {
                        foreach (var file in filesDocs)
                        {
                            try
                            {
                                var ATTA = new PERSON_DOCUMENTS();
                                ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                                ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                                ATTA.CREATE_ATTA = DateTime.Now;
                                ATTA.ID_TYPE_DOCU = 2;
                                ATTA.ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                                ATTA.UserId = ID_USER;

                                dbe.PERSON_DOCUMENTS.Add(ATTA);
                                dbe.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Documents/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_PERS_DOCU) + ATTA.EXT_ATTA));
                            }
                            catch
                            {
                            }
                        }
                    }
                    var pscUpt = dbe.PERSON_STATUS_CHANGE.Single(x => x.ID_PERS_STAT_CHAN == pscNew.ID_PERS_STAT_CHAN);
                    pscUpt.ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                    dbe.PERSON_STATUS_CHANGE.Attach(pscUpt);
                    dbe.Entry(pscUpt).State = EntityState.Modified;
                    dbe.SaveChanges();
                    // Habilitando acceso al ITMS
                    var usuario = dbe.Usuarios.Single(x => x.UserId == class_entity.UserId);
                    usuario.Estado = true;
                    usuario.FechaUltimoAcceso = DateTime.Now;
                    dbe.Usuarios.Attach(usuario);
                    dbe.Entry(usuario).State = EntityState.Modified;
                    dbe.SaveChanges();

                }//Actualizando el contrato si cambia a CEASED STAFF
                else if (ID_PERS_STAT == 3)
                {
                    var contrac = dbe.PERSON_CONTRACT.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.VIG_CONT == true);
                    var wp = dbe.WORK_PERIOD.Single(x => x.ID_WORK_PERI == contrac.ID_WORK_PERI);
                    wp.CESS_DATE = CESS_DATE;
                    wp.ID_REAS_END = ID_REAS_END;

                    //Guardando archivo adjunto del motivo del cese
                    if (fileAttaCess != null)
                    {
                        foreach (var file in fileAttaCess)
                        {
                            try
                            {
                                String NAM_ATTA = "";
                                NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);

                                file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Documents/" + NAM_ATTA + "_" + Convert.ToString(wp.ID_WORK_PERI) + ".pdf"));
                                wp.NAM_ATTA = NAM_ATTA;
                            }
                            catch
                            {
                            }
                        }
                    }
                    dbe.WORK_PERIOD.Attach(wp);
                    dbe.Entry(wp).State = EntityState.Modified;
                    dbe.SaveChanges();

                    //Organization Chart por Contrato
                    var pc_chart = dbe.PERSON_CONTRACT_CHART.Single(x => x.ID_PERS_CONT == contrac.ID_PERS_CONT);
                    pc_chart.VIG_CONT_CHAR = false;
                    pc_chart.DAT_END = CESS_DATE;
                    dbe.PERSON_CONTRACT_CHART.Attach(pc_chart);
                    dbe.Entry(pc_chart).State = EntityState.Modified;
                    dbe.SaveChanges();

                    //Actualizando Contrato
                    var bi = dbe.EMAIL_BOSS_INMSUP(contrac.ID_PERS_ENTI).ToList();
                    if (bi.Count() > 0) //Registrando el cargo y jefe inmediato superior
                    {
                        contrac.ID_INME_BOSS_LAST = bi.First().ID_PERS_ENTI;
                        contrac.ID_CHAR_LAST = bi.First().ID_CHAR;
                    }
                    contrac.VIG_CONT = false;
                    contrac.LAS_CONT = false;
                    contrac.ID_PERS_STAT = 3;
                    dbe.PERSON_CONTRACT.Attach(contrac);
                    dbe.Entry(contrac).State = EntityState.Modified;
                    dbe.SaveChanges();

                    var pscUpt = dbe.PERSON_STATUS_CHANGE.Single(x => x.ID_PERS_STAT_CHAN == pscNew.ID_PERS_STAT_CHAN);
                    pscUpt.ID_PERS_CONT = Convert.ToInt32(contrac.ID_PERS_CONT);
                    pscUpt.ID_PERS_ENTI_TEMP = ID_PERS_ENTI_TEMP;

                    dbe.PERSON_STATUS_CHANGE.Attach(pscUpt);
                    dbe.Entry(pscUpt).State = EntityState.Modified;
                    dbe.SaveChanges();

                    // Restringuiendo acceso al ITMS
                    var usuario = dbe.Usuarios.Single(x => x.UserId == class_entity.UserId);
                    usuario.Estado = false;
                    dbe.Usuarios.Attach(usuario);
                    dbe.Entry(usuario).State = EntityState.Modified;
                    dbe.SaveChanges();

                    //Cambio cargo al personal temporal
                    if (ID_PERS_ENTI_TEMP != 0)
                    {
                        int ID_PERS_CONT_TEMP = dbe.PERSON_CONTRACT.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI_TEMP &&
                                            x.LAS_CONT == true && x.VIG_CONT == true).ID_PERS_CONT;

                        var pcCH = new PERSON_CONTRACT_CHART();
                        pcCH.ID_CHAR = pc_chart.ID_CHAR;
                        pcCH.ID_PERS_CONT = ID_PERS_CONT_TEMP;
                        pcCH.VIG_CONT_CHAR = true;
                        pcCH.IS_CONTRACT = false;
                        pcCH.DAT_STAR = DateTime.Today;

                        dbe.PERSON_CONTRACT_CHART.Add(pcCH);
                        dbe.SaveChanges();
                    }

                    //enviando correo por Cese de Personal
                    //SendEmailContract(ID_PERS_ENTI, 3);
                }

                //Envio credenciales
                int ID_PERS_STAT_INI = Convert.ToInt32(Request.Form["ID_PERS_STAT_INI"]);
                if (ID_PERS_STAT_INI == 3)
                {
                    gth objGth = new gth();
                    string contra = objGth.GenerarContrasenia();
                    Criptografia cripto = new Criptografia();
                    string contraEncriptada = cripto.Encriptar(contra);

                    dbe.ResetearContrasenaReingreso(contraEncriptada, class_entity.UserId, ID_USER);

                    string nomUser = class_entity.FIR_NAME.ToUpper() + " " + class_entity.LAS_NAME.ToUpper();
                    //string mailTo = ConfigurationManager.AppSettings["Mailrrhh"].ToString();
                    var mailTo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_ACCO_PARA == 4179 && x.VIG_ACCO_PARA == true)?.VAL_ACCO_PARA;
                    string cuenta = dbe.Usuarios.Single(x => x.UserId == class_entity.UserId).Usuario1;

                    SendMail mail = new SendMail();
                    string envio = mail.ResetearContrasenaReingreso(mailTo, contra, nomUser, cuenta);
                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensajeActualizacionEstado) top.mensajeActualizacionEstado('OK','0');}window.onload=init;</script>");
                //return Content("<script type='text/javascript'> function init() {" +
                //    "if(top.uploadDone) top.uploadDone('OK','0');}window.onload=init;</script>");
            }
        }

        [Authorize]
        public ActionResult Reports()
        {
            return View();
        }

        public ActionResult ReportEmployeesMonthly()
        {
            int day = DateTime.Today.Day - 1;
            int mes = DateTime.Today.Month;
            int anio = DateTime.Today.Year;
            DateTime FA = DateTime.Today;
            FA = FA.AddDays(-day);
            FA = FA.AddMonths(-5);

            int min = (FA.Year * 100) + FA.Month;
            int max = (anio * 100) + mes;

            var result = (from q in dbc.ACCOUNTING_MONTH.ToList()
                          .Where(x => min <= (x.ID_ACCO_YEAR * 100) + x.ACCO_MONT
                                && (x.ID_ACCO_YEAR * 100) + x.ACCO_MONT <= max).ToList()
                          select new
                          {
                              Anio = q.ID_ACCO_YEAR,
                              MesO = q.ACCO_MONT,
                              Mes = q.NAM_ACCO_MONT.ToLower(),
                          });

            var query = dbe.PERSON_CONTRACT.Where(x => x.END_DATE >= FA);
            int m6 = query.Where(x => x.STAR_DATE.Value.Month - (x.STAR_DATE.Value.Year < anio ? 12 : 0) <= mes &&
                                        x.END_DATE.Value.Month + (x.END_DATE.Value.Year > anio ? 12 : 0) >= mes).Count();

            mes = mes - 1;
            if (mes == 0) { mes = 12; anio = anio - 1; }
            int m5 = query.Where(x => x.STAR_DATE.Value.Month - (x.STAR_DATE.Value.Year < anio ? 12 : 0) <= mes &&
                                        x.END_DATE.Value.Month + (x.END_DATE.Value.Year > anio ? 12 : 0) >= mes).Count();

            mes = mes - 1;
            if (mes == 0) { mes = 12; anio = anio - 1; }
            int m4 = query.Where(x => x.STAR_DATE.Value.Month - (x.STAR_DATE.Value.Year < anio ? 12 : 0) <= mes &&
                                        x.END_DATE.Value.Month + (x.END_DATE.Value.Year > anio ? 12 : 0) >= mes).Count();

            mes = mes - 1;
            if (mes == 0) { mes = 12; anio = anio - 1; }
            int m3 = query.Where(x => x.STAR_DATE.Value.Month - (x.STAR_DATE.Value.Year < anio ? 12 : 0) <= mes &&
                                        x.END_DATE.Value.Month + (x.END_DATE.Value.Year > anio ? 12 : 0) >= mes).Count();

            mes = mes - 1;
            if (mes == 0) { mes = 12; anio = anio - 1; }
            int m2 = query.Where(x => x.STAR_DATE.Value.Month - (x.STAR_DATE.Value.Year < anio ? 12 : 0) <= mes &&
                                        x.END_DATE.Value.Month + (x.END_DATE.Value.Year > anio ? 12 : 0) >= mes).Count();

            mes = mes - 1;
            if (mes == 0) { mes = 12; anio = anio - 1; }
            int m1 = query.Where(x => x.STAR_DATE.Value.Month - (x.STAR_DATE.Value.Year < anio ? 12 : 0) <= mes &&
                                        x.END_DATE.Value.Month + (x.END_DATE.Value.Year > anio ? 12 : 0) >= mes).Count();

            int UserId = Convert.ToInt32(Session["UserId"]);
            var result1 = (from q in dbe.CLASS_ENTITY.Where(x => x.UserId == UserId).ToList()
                           select new
                           {
                               NAM_SERI = "Employees",
                               MES1 = m1,
                               MES2 = m2,
                               MES3 = m3,
                               MES4 = m4,
                               MES5 = m5,
                               MES6 = m6,
                               COL_SERI = "#B21E1E",
                           });

            int L1 = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 21).VAL_ACCO_PARA);
            int L2 = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 22).VAL_ACCO_PARA);

            return Json(new { Data = result1, Legend = result, L1 = L1, L2 = L2 }, JsonRequestBehavior.AllowGet);
        }

        //Expiration Contract
        private void SendEmailContract(int id = 0, int id1 = 1)
        {
            //id1 = 1:New Contract (Nuevo Contrato)
            //id1 = 2:Ceased Employee (Empleado Cesado)
            SendMail mail = new SendMail();
            mail.ManttoContract(id, id1);
        }

        public ActionResult ListCIA()
        {
            var query = dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null);

            //x.ID_PARA == 18 COMPANIA PROPIETARIA
            var result = (from a in dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true).ToList()
                          join b in query on Convert.ToInt32(a.VAL_ACCO_PARA) equals b.ID_ENTI
                          select new
                          {
                              ID_ENTI1 = b.ID_ENTI,
                              b.COM_NAME,
                          }).OrderBy(x => x.ID_ENTI1).ToList();

            var xx = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 18 && x.VIG_ACCO_PARA == true).ToList();

            //var result = 

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DataUserEdit(int id = 0)
        {
            int ID_PERS_ENTI = id;

            var qPersonal = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI1, ce => ce.ID_ENTI, (x, ce) => new
                {
                    x.EXT_PERS,
                    x.TEL_PERS,
                    x.CEL_PERS,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_STAT,
                    x.ID_CARG_AREA,
                    x.ID_ENTI2,
                    x.EMA_ELEC,
                    x.EMA_PERS,
                    x.NroColegiatura,
                    ce.COM_NAME,
                })
                .Join(dbe.PERSON_STATUS, x => x.ID_PERS_STAT, s => s.ID_PERS_STAT, (x, s) => new
                {
                    x.EXT_PERS,
                    x.TEL_PERS,
                    x.CEL_PERS,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_STAT,
                    x.ID_CARG_AREA,
                    x.COM_NAME,
                    x.ID_ENTI2,
                    x.EMA_ELEC,
                    x.EMA_PERS,
                    s.NAM_STAT,
                    x.NroColegiatura,
                });

            var qContract = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.LAS_CONT == true)
                .Join(dbe.CONTRACT_CONDITION, x => x.ID_COND_CONT, c => c.ID_COND_CONT, (x, c) => new
                {
                    x.STAR_DATE,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_CONT,
                    x.ID_WORK_PERI,
                    x.ID_CARG_AREA,
                    x.GROSS_SALARY,
                    x.ID_CHAR,
                    c.CONDITION,
                })
                .Join(dbe.WORK_PERIOD, x => x.ID_WORK_PERI, wp => wp.ID_WORK_PERI, (x, wp) => new
                {
                    x.STAR_DATE,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_CONT,
                    x.ID_WORK_PERI,
                    x.ID_CARG_AREA,
                    x.GROSS_SALARY,
                    x.ID_CHAR,
                    x.CONDITION,
                    wp.CESS_DATE,
                    STAR_PERI = wp.STAR_DATE,
                })
                .Join(dbe.CHARTs, x => x.ID_CHAR, ca => ca.ID_CHAR, (x, ca) => new
                {
                    x.STAR_DATE,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_CONT,
                    x.ID_WORK_PERI,
                    x.ID_CHAR,
                    x.GROSS_SALARY,
                    x.CONDITION,
                    x.CESS_DATE,
                    x.STAR_PERI,
                    ca.ID_CHAR_PARE,
                    ca.ID_NAM_CHAR,
                })
                .Join(dbe.NAME_CHART, x => x.ID_NAM_CHAR, nc => nc.ID_NAM_CHAR, (x, nc) => new
                {
                    x.STAR_DATE,
                    x.ID_PERS_ENTI,
                    x.ID_PERS_CONT,
                    x.ID_WORK_PERI,
                    x.ID_CHAR,
                    x.GROSS_SALARY,
                    x.CONDITION,
                    x.CESS_DATE,
                    x.STAR_PERI,
                    x.ID_CHAR_PARE,
                    x.ID_NAM_CHAR,
                    nc.NAM_CHAR,
                });

            var qLocaCore = dbc.SITEs.Where(x => x.ID_ACCO == 4)
                .Join(dbc.LOCATIONs, x => x.ID_SITE, l => l.ID_SITE, (x, l) => new
                {
                    x.ID_SITE,
                    x.NAM_SITE,
                    l.ID_LOCA,
                    l.NAM_LOCA,
                });

            var result = (from q in qPersonal.ToList()
                          join cp in dbe.CLASS_ENTITY on q.ID_ENTI2 equals cp.ID_ENTI

                          join tdi in dbe.TYPE_DOCUMENTIDENT on cp.ID_TYPE_DI equals tdi.ID_TYPE_DI into ltdi
                          from xtdi in ltdi.DefaultIfEmpty()
                          join cs in dbe.CIVIL_STATUS on cp.ID_CIVI_STAT equals cs.ID_CIVI_STAT into lcs
                          from xcs in lcs.DefaultIfEmpty()
                          join di in dbe.DEGREE_INSTRUCTION on cp.ID_DEGR_INST equals di.ID_DEGR_INST into ldi
                          from xdi in ldi.DefaultIfEmpty()
                          join bg in dbe.BLOOD_GROUP on cp.ID_BLOO_GROU equals bg.ID_BLOO_GROU into lbg
                          from xbg in lbg.DefaultIfEmpty()
                          join n in dbe.NATIONALITies on cp.ID_NATI equals n.ID_NATI into ln
                          from xn in ln.DefaultIfEmpty()

                          join q1 in qContract on q.ID_PERS_ENTI equals (q1 == null ? 0 : q1.ID_PERS_ENTI) into lq1
                          from xq1 in lq1.DefaultIfEmpty()
                          join pl in dbe.PERSON_LOCATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.END_DATE == null)
                                    on q.ID_PERS_ENTI equals (pl == null ? 0 : pl.ID_PERS_ENTI) into lpl
                          from xpl in lpl.DefaultIfEmpty()
                          join q3 in qLocaCore on (xpl == null ? 0 : xpl.ID_LOCA) equals q3.ID_LOCA into lq3
                          from xq3 in lq3.DefaultIfEmpty()

                          join eps in dbe.EPSALUDs.Where(x => x.LAST_EPS == true) on q.ID_PERS_ENTI equals eps.ID_PERS_ENTI into leps
                          from xeps in leps.DefaultIfEmpty()
                          join peps in dbe.PLAN_EPS on (xeps == null ? 0 : xeps.ID_PLAN_EPS) equals peps.ID_PLAN_EPS into lpeps
                          from xpeps in lpeps.DefaultIfEmpty()

                          join p in dbe.PENSIONs.Where(x => x.END_DATE == null) on q.ID_PERS_ENTI equals p.ID_PERS_ENTI into lp
                          from xp in lp.DefaultIfEmpty()
                          join tp in dbe.TYPE_PENSION on (xp == null ? 0 : xp.ID_TYPE_PENS) equals tp.ID_TYPE_PENS into ltp
                          from xtp in ltp.DefaultIfEmpty()

                          join pp in dbe.PERSON_PAYMENT.Where(x => x.DAT_END == null && x.ID_TYPE_PAYM == 1) on q.ID_PERS_ENTI equals pp.ID_PERS_ENTI into lpp
                          from xpp in lpp.DefaultIfEmpty()
                          join b in dbe.BANKs on (xpp == null ? 0 : xpp.ID_BANK) equals b.ID_BANK into lb
                          from xb in lb.DefaultIfEmpty()

                          join cts in dbe.PERSON_PAYMENT.Where(x => x.DAT_END == null && x.ID_TYPE_PAYM == 2) on q.ID_PERS_ENTI equals cts.ID_PERS_ENTI into lcts
                          from xcts in lcts.DefaultIfEmpty()
                          join bcts in dbe.BANKs on (xcts == null ? 0 : xcts.ID_BANK) equals bcts.ID_BANK into lbcts
                          from xbcts in lbcts.DefaultIfEmpty()

                          join cpr in dbe.Profesions on (cp.IdProfesion == null ? 0 : cp.IdProfesion) equals cpr.Id into cprl
                          from cprle in cprl.DefaultIfEmpty()

                          join uni in dbe.Universidad on (cp.IdUniversidad == null ? 0 : cp.IdUniversidad) equals uni.Id into unis
                          from unise in unis.DefaultIfEmpty()
                          select new
                          {
                              COM_NAME = q.COM_NAME.ToLower(),
                              User = cp.FIR_NAME + " " + cp.SEC_NAME + " " + cp.LAS_NAME + (cp.MOT_NAME == null ? "" :
                                        (cp.MOT_NAME.Trim().Length == 0 ? "" : " " + cp.MOT_NAME.Substring(0, 1).ToUpper() + ".")),
                              Sex = (cp.SEX_ENTI == null ? "-" : (cp.SEX_ENTI == "M" ? @ResourceLanguaje.Resource.Male : @ResourceLanguaje.Resource.Female)).ToLower(),
                              NUM_TYPE_DI = (cp.NUM_TYPE_DI == null ? "-" : cp.NUM_TYPE_DI),
                              TYPE_DI = (xtdi == null ? "-" : xtdi.NAM_TYPE_DI),
                              EMISSION_DATE_DNI = (cp.EMISSION_DATE_DNI == null ? "-" : String.Format("{0:d}", cp.EMISSION_DATE_DNI)),
                              EXPIRE_DATE_DNI = (cp.EXPIRE_DATE_DNI == null ? "-" : String.Format("{0:d}", cp.EXPIRE_DATE_DNI)),
                              TEL_ENTI = (cp.TEL_ENTI == null ? "-" : cp.TEL_ENTI),
                              CEL_ENTI = (cp.CEL_ENTI == null ? "-" : cp.CEL_ENTI),
                              EMA_ENTI = (cp.EMA_ENTI == null ? "-" : cp.EMA_ENTI.ToLower()),
                              EMA_ELEC = (q.EMA_ELEC == null ? "-" : q.EMA_ELEC.Trim().Length == 0 ? "-" : q.EMA_ELEC.ToLower()),
                              EMA_PERS = (q.EMA_PERS == null ? "-" : q.EMA_PERS.Trim().Length == 0 ? "-" : q.EMA_PERS.ToLower()),
                              NRO_COLEGIATURA = (q.NroColegiatura == "" ? "-" : (q.NroColegiatura == null) ? "-" : q.NroColegiatura),

                              ADDRESS = (cp.ADDRESS == null ? "-" : cp.ADDRESS.ToLower()),
                              CONTACT_NAME = (cp.CONTACT_NAME == null ? "-" : cp.CONTACT_NAME.ToLower()),
                              CONTACT_PHONE = (cp.CONTACT_PHONE == null ? "-" : cp.CONTACT_PHONE),
                              BIRTHDAY = (cp.BIRTHDAY == null ? "-" : String.Format("{0:dd/MM/yyyy}", cp.BIRTHDAY)),
                              CIVI_STAT = (xcs == null ? "-" : (xcs.NAM_CIVI_STAT == null ? "-" : xcs.NAM_CIVI_STAT.ToLower())),
                              DEGR_INST = (xdi == null ? "-" : (xdi.NAM_DEGR_INST == null ? "-" : xdi.NAM_DEGR_INST.ToLower())),
                              BLOO_GROU = (xbg == null ? "-" : (xbg.NAM_BLOO_GROU == null ? "-" : xbg.NAM_BLOO_GROU.ToLower())),
                              NATI = (xn == null ? "-" : (xn.NAM_NATI == null ? "-" : xn.NAM_NATI.ToLower())),

                              CAR_PERS = (xq1 == null ? "-" : (xq1.ID_CHAR == null ? "-" : xq1.NAM_CHAR.ToLower())),
                              Ubicacion = (xq3 == null ? "-" : (xq3.NAM_SITE + " / " + xq3.NAM_LOCA).ToLower()),
                              Site = (xq3 == null ? "-" : xq3.NAM_SITE.ToLower()),
                              Location = (xq3 == null ? "-" : xq3.NAM_LOCA.ToLower()),
                              NAM_STAT = q.NAM_STAT.ToLower(),
                              NOM_AREA = (xq1 == null ? "-" : FindNodo(Convert.ToInt32((xq1.ID_CHAR_PARE == null ? 0 : xq1.ID_CHAR_PARE)), 2)),
                              TEL_PERS = (q.TEL_PERS == null ? "-" : q.TEL_PERS),
                              CEL_PERS = (q.CEL_PERS == null ? "-" : q.CEL_PERS),
                              EXT_PERS = (q.EXT_PERS == null ? "-" : q.EXT_PERS),
                              STAR_PERI = (xq1 == null ? "-" : String.Format("{0:dd/MM/yyyy}", xq1.STAR_PERI)),
                              CONDITION = (xq1 == null ? "-" : xq1.CONDITION.ToLower()),
                              CESS_PER = (xq1 == null ? "-" : (xq1.CESS_DATE == null ? "-" : String.Format("{0:dd/MM/yyyy}", xq1.CESS_DATE))),
                              Tiempo = TimeWork(ID_PERS_ENTI),
                              USB = (xq1 == null ? "-" : FindNodo(Convert.ToInt32((xq1.ID_CHAR_PARE == null ? 0 : xq1.ID_CHAR_PARE)), 1)),
                              //USB = USB(Convert.ToInt32((xq1==null ? 0 : xq1.ID_AREA))),
                              GROSS_SALARY = (xq1 == null ? "-" : String.Format("{0:0.00}", xq1.GROSS_SALARY)),
                              SalaryBank = (xb == null ? "-" : xb.NAM_BANK.ToLower()),
                              SalaryNumAcco = (xpp == null ? "-" : xpp.NUM_ACCO),
                              SalaryAOD = (xpp == null ? "-" : (xpp.DAT_STAR == null ? "-" : String.Format("{0:d}", xpp.DAT_STAR))),
                              CTSBank = (xbcts == null ? "-" : xbcts.NAM_BANK.ToLower()),
                              CTSNumAcco = (xcts == null ? "-" : xcts.NUM_ACCO),
                              CTSAOD = (xcts == null ? "-" : (xcts.DAT_STAR == null ? "-" : String.Format("{0:d}", xcts.DAT_STAR))),
                              AFP_ONP = (xtp == null ? "-" : xtp.NAM_PENS.ToLower()),
                              CUSPP = (xp == null ? "-" : xp.CUSPP),
                              PENDateStar = (xp == null ? "-" : (xp.START_DATE == null ? "-" : String.Format("{0:d}", xp.START_DATE))),
                              EPSPlan = (xpeps == null ? "-" : xpeps.NAM_PLAN.ToLower()),
                              NUM_FAMI = (xeps == null ? "-" : String.Format("{0:0}", xeps.NUMBER_FAMI)),
                              EPSDateStar = (xeps == null ? "-" : (xeps.STAR_DATE == null ? "-" : String.Format("{0:d}", xeps.STAR_DATE))),
                              Profesion = (cprle == null ? "-" : cprle.Nombre),
                              Universidad = (unise == null ? "" : unise.Nombre)
                          });

            return Json(new { data = result, Count = qPersonal.Count() }, JsonRequestBehavior.AllowGet);
        }

        public string Edad(DateTime brith)
        {
            DateTime currentDate = DateTime.Now;

            int ageInYears = 0;
            int ageInMonths = 0;
            int ageInDays = 0;

            ageInDays = currentDate.Day - brith.Day;
            ageInMonths = currentDate.Month - brith.Month;
            ageInYears = currentDate.Year - brith.Year;

            if (ageInDays < 0)
            {
                ageInDays += DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                ageInMonths = ageInMonths--;

                if (ageInMonths < 0)
                {
                    ageInMonths += 12;
                    ageInYears--;
                }
            }
            if (ageInMonths < 0)
            {
                ageInMonths += 12;
                ageInYears--;
            }

            return ageInYears.ToString() + " Years " + ageInMonths.ToString() + " Months";
        }

        public ActionResult ListBeneficiaries(int id = 0, int id1 = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.sw = id1;
            return View();
        }

        public ActionResult ListAchievements(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult ListInvoice(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;

            //Determinando si el usuario puede eliminar 
            ViewBag.OPT_DELETE = 0;
            //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            //foreach (string xc in rolesArray)
            //{
            //    if (xc == "RRHH" || xc == "ADMIN RRHH")
            //    { ViewBag.OPT_DELETE = 1; }
            //}
            if ((int)Session["RRHH"] == 1)
            {
                ViewBag.OPT_DELETE = 1;
            }

            ViewBag.ANIO_MES = Convert.ToInt32(DateTime.Now.Year.ToString() + (DateTime.Now.Month < 10 ? "0" : "") + DateTime.Now.Month.ToString());
            if (ViewBag.OPT_DELETE == 0)
            {// si no es falso entonces tiene el rol de RRHH or ADMIN RRHH, por lo tanto puede descargar el archivo

                var query2 = dbc.ACCOUNTING_MONTH;
                var qPBUnsigned = (from a in dbe.PERSON_INVOICE.Where(x => x.ID_PERS_ENTI == id && x.SIGNED == false && x.ID_TYPE_PAYM == 1).ToList()
                                   where !(from c in dbe.PERSON_INVOICE.Where(x => x.ID_PERS_ENTI == id && x.SIGNED == true && x.ID_TYPE_PAYM == 1)
                                           select c.ID_ACCO_MONT).Contains(a.ID_ACCO_MONT)
                                   join b in query2 on a.ID_ACCO_MONT equals b.ID_ACCO_MONT
                                   select new
                                   {
                                       AnioMes = Convert.ToInt32(Convert.ToString(b.ID_ACCO_YEAR) +
                                                (b.ACCO_MONT < 10 ? "0" : "") + Convert.ToString(b.ACCO_MONT)),
                                   }).OrderBy(x => x.AnioMes);

                if (qPBUnsigned.Count() > 0)
                {   //Año y mes del ultima boleta sin firmar, por lo tanto no puede descargar las siguiente sin antes subir esta ultima.
                    ViewBag.ANIO_MES = qPBUnsigned.First().AnioMes;
                }
            }

            return View();
        }

        public ActionResult ListDocuments(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult ListaBeneficiarios(int id = 0)
        {
            DateTime fi = Convert.ToDateTime("01-01-1950");

            var resultBeneficiries =
                        (from b in dbe.BENEFICIARies.Where(x => x.ID_PERS_ENTI == id).ToList()
                         join k in dbe.KINDREDs on b.ID_KINDRED equals k.ID_KINDRED
                         select new
                         {
                             b.ID_BENE,
                             b.ID_KINDRED,
                             Beneficiari = b.FIR_NAME.ToLower() + " " + b.LAS_NAME.ToLower(),
                             BENI_FIR_NAME = b.FIR_NAME,
                             BENI_LAS_NAME = b.LAS_NAME,

                             BENI_SEC_NAME = b.SEC_NAME,
                             BENI_MOT_NAME = b.MOT_NAME,

                             BRITH = (b.DATE_BRITH == null ? "-" : String.Format("{0:d}", b.DATE_BRITH)),
                             EDAD = b.DATE_BRITH.HasValue == true ? Edad(b.DATE_BRITH.Value) : "-",
                             b.NUMB_DNI,
                             NAM_KIND = k.NAM_KIND.ToLower(),
                             DATE_BRITH = (b.DATE_BRITH.HasValue == true ? b.DATE_BRITH : fi),
                             b.ATT_DNI,
                             b.ATT_EVID_STUD,
                             NAM_FILE_DNI = (b.ATT_DNI == true ? b.FIR_NAME + b.LAS_NAME + "_" + Convert.ToString(b.ID_BENE) + ".pdf" : ""),
                             NAM_FILE_CE = (b.ATT_EVID_STUD == true ? "EvidenceOfStudies_" + Convert.ToString(b.ID_BENE) + ".pdf" : ""),
                         }).OrderBy(x => x.DATE_BRITH);

            return Json(new { data = resultBeneficiries, Count = resultBeneficiries.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewBeneficiary(int id = 0)
        {

            //Recibira el ID_PERS_ENTI del que depende el beneficiary.           

            ViewBag.ID_PERS_ENTI = id;
            ViewBag.TODAY = String.Format("{0:d}", DateTime.Now);

            ViewBag.UploadFile = "AB" + Convert.ToString(DateTime.Now.Ticks);

            return View();
        }



        public bool AccesoContracEdit(string ID_PERS_ENTI = "")
        {
            //Personal Administrador del Talent: RRHH
            if (ID_PERS_ENTI != "")
            {
                int ctd = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 17 && x.VAL_ACCO_PARA == ID_PERS_ENTI && x.VIG_ACCO_PARA == true).Count();
                if (ctd == 0)
                {
                    //Obteniendo el Area y Cargo del Usuario
                    int ID_CARG_AREA = 0, ID_CARG = 0;
                    int ID_PE = Convert.ToInt32(ID_PERS_ENTI);
                    var query1 = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PE && x.VIG_CONT == true && x.LAS_CONT == true);
                    if (query1.Count() > 0)
                    {
                        ID_CARG_AREA = (query1.Single().ID_CARG_AREA.HasValue == true ? query1.Single().ID_CARG_AREA.Value : 0);
                        if (ID_CARG_AREA != 0)
                        {
                            var query2 = dbe.CARGO_AREA.Single(x => x.ID_CARG_AREA == ID_CARG_AREA);
                            ID_CARG = query2.ID_CARG.Value;
                        }
                    }

                    //Determinando si el usuario es un Gerente
                    if (ID_CARG > 0)
                    {
                        var query3 = dbe.CARGOes.Single(x => x.ID_CARG == ID_CARG);
                        bool MN = (query3.MANAGEMENT == null ? false : Convert.ToBoolean(query3.MANAGEMENT));
                        if (MN) { return true; }
                    }
                }
                else { return true; }
            }

            return false;
        }

        [Authorize]
        public ActionResult Contract(int id = 0)
        {
            string ID_PE = Convert.ToString(Session["ID_PERS_ENTI"]);
            if (AccesoContracEdit(ID_PE))
            {
                var query = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id);

                ViewBag.ID_PERS_CONT = 0;
                var qPC = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == id && x.LAS_CONT == true);
                if (qPC.Count() > 0)
                {
                    ViewBag.ID_PERS_CONT = qPC.First().ID_PERS_CONT;
                }

                ViewBag.FOTO = query.ID_FOTO.ToString() + ".jpg";
                ViewBag.ID_PERS_ENTI = id;
                return View();
            }
            else
            {
                return Content("<div style=\"width:100%; position: relative;height:100%; text-align:center; \">" +
                "<div style=\"position: absolute; top: 50%; left: 50%; height: 100px; width: 300px; " +
                "margin: -50px 0 0 -150px; vertical-align:middle; color:#B21E1E; font-size:2em;\">ACCESO RESTRINGIDO</div></div>");
            }
        }

        public ActionResult TabProfile(int id = 0, string id1 = "")
        {
            IdPersEnti = id;
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.FOTO = id1;
            return View();
        }

        public ActionResult TabProfileOld(int id = 0, string id1 = "")
        {
            IdPersEnti = id;
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.FOTO = id1;
            return View();
        }

        public ActionResult TabDocuments(int id = 0)
        {
            int ID_PERS_STAT = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id).ID_PERS_STAT.Value;
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.ID_PERS_STAT = ID_PERS_STAT;
            return View();
        }

        public ActionResult TabPaymentBallots(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult TabBeneficiaries(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult TabVacation(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult TabEditProfile(int id = 0, string id1 = "")
        {
            CLASS_ENTITY ce = new CLASS_ENTITY();
            var query = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id);

            ce = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == query.ID_ENTI2);

            ViewBag.EMA_PERS = query.EMA_PERS;
            ViewBag.EMA_ELEC = query.EMA_ELEC;
            ViewBag.TEL_PERS = query.TEL_PERS;
            ViewBag.EXT_PERS = query.EXT_PERS;
            ViewBag.CEL_PERS = query.CEL_PERS;

            ViewBag.ID_SUB_CIA = query.ID_SUB_CIA;
            ViewBag.RPM_PERS_ENTI = query.RPM_PERS;

            ViewBag.SEX_ENTI = ce.SEX_ENTI;
            ViewBag.BIRTHDAY = (ce.BIRTHDAY == null ? "" : String.Format("{0:d/M/yyyy}", ce.BIRTHDAY) + " " + String.Format("{0:hh:mm tt}", ce.BIRTHDAY));
            ViewBag.EXPIRE_DATE_DNI = (ce.EXPIRE_DATE_DNI == null ? "" : String.Format("{0:d}", ce.EXPIRE_DATE_DNI) + " " + String.Format("{0:hh:mm tt}", ce.EXPIRE_DATE_DNI));
            ViewBag.EMISSION_DATE_DNI = (ce.EMISSION_DATE_DNI == null ? "" : String.Format("{0:d}", ce.EMISSION_DATE_DNI) + " " + String.Format("{0:hh:mm tt}", ce.EMISSION_DATE_DNI));
            ViewBag.TODAY = String.Format("{0:d}", DateTime.Now) + " 12:00 AM";
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.FOTO = id1;
            return View(ce);
        }

        public ActionResult TabEditDocuments(int id = 0, int id1 = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.ID_PERS_STAT = id1;
            return View();
        }

        public ActionResult TabEditPaymentBallots(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult TabEditBeneficiaries(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult TabEditAccounts(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult TabEditPension(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult TabActivities(int id = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }
        public ActionResult Training()
        {

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = (from a in dbe.PERSON_TRAINING.Where(x => x.ID_PERS_REQU == ID_PERS_ENTI ||
                                    x.ID_PERS_SUPE == ID_PERS_ENTI || x.ID_PERS_OPER == ID_PERS_ENTI)
                         select new
                         {
                             a.STATUS
                         });

            ViewBag.TProcessing = query.Where(x => x.STATUS == 0).Count(); ;
            ViewBag.TByApproving = query.Where(x => x.STATUS == 1).Count(); ; ;
            ViewBag.TApprovals = query.Where(x => x.STATUS == 2).Count(); ; ;
            ViewBag.TResults = query.Where(x => x.STATUS == 3).Count(); ; ;
            return View();
        }

        [Authorize]
        public ActionResult ViewDownload(string id = "")
        {
            ViewBag.Folder = id;
            return View();
        }

        [Authorize]
        public ActionResult DownLoadFile(string id = "")
        {
            try
            {
                //open file from the disk (file path is the path to the file to be opened)
                int idf = Convert.ToInt32(Session["ID_PERS_DOCU"]);
                Session["ID_PERS_DOCU"] = "";
                string filename = "";
                if (id == "Documents")
                {
                    var pd = dbe.PERSON_DOCUMENTS.Single(x => x.ID_PERS_DOCU == idf);
                    filename = pd.NAM_ATTA + "_" + Convert.ToString(pd.ID_PERS_DOCU) + ".pdf";
                }
                else if (id == "Invoice")
                {
                    var pd = dbe.PERSON_INVOICE.Single(x => x.ID_INVO == idf);
                    filename = pd.NAM_ATTA + "_" + Convert.ToString(pd.ID_INVO) + ".pdf";
                }

                FileStream fileStream = System.IO.File.OpenRead(Server.MapPath("~/Adjuntos/Talent/" + id + "/" + filename));
                MemoryStream storeStream = new MemoryStream();

                storeStream.SetLength(fileStream.Length);
                fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                byte[] byteArray = storeStream.ToArray();

                storeStream.Flush();
                fileStream.Close();
                storeStream.Close();

                Random r = new Random();
                int aleatorio = r.Next(10000, 99999);
                aleatorio = aleatorio + idf;

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                Response.BinaryWrite(byteArray);
                Response.End();

                return File(storeStream.GetBuffer(), "pdf");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "alert('" + ResourceLanguaje.Resource.DownloadFileMsn + "');}window.onload=init;</script>");
            }

        }

        [Authorize]
        public ActionResult ViewFindAchievements()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = (int)Session["UserId"];
            if (ID_ACCO == 4)//ELECTRODATA
            {
                string perfilLogros = (string)dbe.PerfilLogros(UserId).Single().PERFIL;
                if (perfilLogros == "")
                {
                    return RedirectToAction("Index", "Error");
                }
                else
                {
                    ViewBag.Perfil = perfilLogros;
                }
            }
            ViewBag.swith = 0;
            return View();
        }
        //[Authorize]
        //public ActionResult ViewFindAchievements()
        //{
        //    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
        //    int UserId = (int)Session["UserId"];
        //    if (ID_ACCO == 4)//ELECTRODATA
        //    {
        //        int permisoLogros = (int)dbe.PermisoLogros(UserId).Single().Cantidad;
        //        if (permisoLogros == 0)
        //        {
        //            return Content("Necesitas permisos para acceder a esta sección.");
        //        }
        //    }
        //    ViewBag.swith = 0;
        //    return View();
        //}
        public ActionResult FindAchieResult()
        {
            NAM_ATTA = Convert.ToString(Request.Params["NAM_ATTA"].ToString());

            String documento = Request.Params["ID_TYPE_DOCU"].ToString();
            if (Request.Params["ID_TYPE_DOCU"].ToString() == "")
            {
                ID_TYPE_DOCU = 0;
            }
            else
            {
                ID_TYPE_DOCU = Convert.ToInt32(Request.Params["ID_TYPE_DOCU"]);
            }

            if (Request.Params["ID_PERS_ENTI"].ToString() == "")
            {
                ID_PERS_ENTI = 0;
            }
            else
            {
                ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
            }

            if (Request.Params["IdInstitucion"].ToString() == "")//nova
            {
                IdInstituto = 0;
            }
            else
            {
                IdInstituto = Convert.ToInt32(Request.Params["IdInstituto"]);
            }

            if (Request.Params["IdMarca"].ToString() == "")
            {
                IdMarca = 0;
            }
            else
            {
                IdMarca = Convert.ToInt32(Request.Params["IdMarca"]);
            }
            if (Request.Params["ID_DEGR_INST"].ToString() == "")
            {
                IdGradoInstruccion = 0;
            }
            else
            {
                IdGradoInstruccion = Convert.ToInt32(Request.Params["ID_DEGR_INST"]);
            }

            strFechaInicioVen = Request.Params["FechaIniVencimiento"].ToString();
            strFechaFinVen = Request.Params["FechaFinVencimiento"].ToString();
            DateTime FechaIniVencimiento = Convert.ToDateTime("1900/01/01");
            DateTime FechaFinVencimiento = Convert.ToDateTime("9999/12/31");
            if (!String.IsNullOrEmpty(strFechaInicioVen))
            {
                FechaIniVencimiento = Convert.ToDateTime(strFechaInicioVen);
            }
            if (!String.IsNullOrEmpty(strFechaFinVen))
            {
                FechaFinVencimiento = Convert.ToDateTime(strFechaFinVen).AddDays(1);
            }

            List<docPersonasDocumentos_Result> query = dbe.docPersonasDocumentos(ID_TYPE_DOCU, ID_PERS_ENTI, IdMarca, NAM_ATTA, FechaIniVencimiento, FechaFinVencimiento, IdGradoInstruccion, IdInstituto).ToList();

            var query3 = (from q2 in query
                          select new
                          {
                              q2.ID_PERS_ENTI,
                          }).Distinct();

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query2 = (from q3 in query3.Skip(skip).Take(take).ToList()
                          join q2 in query on q3.ID_PERS_ENTI equals q2.ID_PERS_ENTI
                          select q2);

            return Json(new { Data = query2, Count = query3.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindAchieResultD()
        {
            NAM_ATTA = Convert.ToString(Request.Params["NAM_ATTA"].ToString());

            String documento = Request.Params["ID_TYPE_DOCU"].ToString();
            if (Request.Params["ID_TYPE_DOCU"].ToString() == "")
            {
                ID_TYPE_DOCU = 0;
            }
            else
            {
                ID_TYPE_DOCU = Convert.ToInt32(Request.Params["ID_TYPE_DOCU"]);
            }

            if (Request.Params["ID_PERS_ENTI"].ToString() == "")
            {
                ID_PERS_ENTI = 0;
            }
            else
            {
                ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
            }

            if (Request.Params["IdInstitucion"].ToString() == "")
            {
                IdInstituto = 0;
            }
            else
            {
                IdInstituto = Convert.ToInt32(Request.Params["IdInstituto"]);
            }

            if (Request.Params["IdMarca"].ToString() == "")
            {
                IdMarca = 0;
            }
            else
            {
                IdMarca = Convert.ToInt32(Request.Params["IdMarca"]);
            }
            if (Request.Params["ID_DEGR_INST"].ToString() == "")
            {
                IdGradoInstruccion = 0;
            }
            else
            {
                IdGradoInstruccion = Convert.ToInt32(Request.Params["ID_DEGR_INST"]);
            }
            DateTime FechaIniVencimiento = Convert.ToDateTime("1900/01/01");
            DateTime FechaFinVencimiento = Convert.ToDateTime("9999/12/31");
            if (!String.IsNullOrEmpty(strFechaInicioVen))
            {
                FechaIniVencimiento = Convert.ToDateTime(strFechaInicioVen);
            }
            if (!String.IsNullOrEmpty(strFechaFinVen))
            {
                FechaFinVencimiento = Convert.ToDateTime(strFechaFinVen).AddDays(1);
            }

            List<docPersonasDocumentos_Result> query = dbe.docPersonasDocumentos(ID_TYPE_DOCU, ID_PERS_ENTI, IdMarca, NAM_ATTA, FechaIniVencimiento, FechaFinVencimiento, IdGradoInstruccion, IdInstituto).ToList();



            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ViewChart()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var qAP = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 6 && x.ID_ACCO == ID_ACCO);

            if (qAP.Count() > 0)
            {
                ViewBag.ID_CIA = Convert.ToInt32(qAP.Single().VAL_ACCO_PARA);
            }
            return View();
        }

        [Authorize]
        public ActionResult LoadPaymentBallots()
        {
            if (Convert.ToInt32(Session["RRHH"]) == 1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }

        }

        public ActionResult TabEditQueue(int id = 0)
        {
            ViewBag.ID_PERS_ENTI_QUEU = id;
            var query = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id);
            ViewBag.ID_ACCO_PERT = query.ID_ACCO_PERT;
            ViewBag.ID_QUEU = query.ID_QUEU;
            return View();
        }
        public ActionResult ListAccounts()
        {
            var query = (from a in dbc.ACCOUNTs
                         select new
                         {
                             ID_ACCO = a.ID_ACCO,
                             NAM_ACCO = a.NAM_ACCO,
                         });

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListQueues()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);

                var query = (from aq in dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO).ToList()
                             join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                             select new
                             {
                                 ID_QUEU = aq.ID_QUEU,
                                 NAM_QUEU = q.NAM_QUEU,
                                 DES_QUEU = CapitalizeCadena(q.DES_QUEU),
                             });

                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize, HttpPost]
        public ActionResult UpdateQueue()
        {
            string PERS_ENTI = Convert.ToString(Request.Params["ID_PERS_ENTI"]);
            string ACCO_PERS = Convert.ToString(Request.Params["ID_ACCO"]);
            string QUEU = Convert.ToString(Request.Params["ID_QUEU"]);

            if (!String.IsNullOrEmpty(QUEU) && !String.IsNullOrEmpty(PERS_ENTI) && !String.IsNullOrEmpty(ACCO_PERS))
            {
                int ID_QUEU = Convert.ToInt32(Request.Params["ID_QUEU"]);
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
                int ID_ACCO_PERS = Convert.ToInt32(Request.Params["ID_ACCO"]);
                PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI);

                pe.ID_QUEU = ID_QUEU;
                pe.ID_ACCO_PERT = ID_ACCO_PERS;
                dbe.PERSON_ENTITY.Attach(pe);
                dbe.Entry(pe).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('OK','Se creó el registro.');}window.onload=init;</script>");
                //return Content("<script type='text/javascript'> function init() {" +
                // "if(top.PopUpQueue) top.PopUpQueue('OK','0');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','Ha ocurrido un error, contacte al administrador.');}window.onload=init;</script>");
                //return Content("<script type='text/javascript'> function init() {" +
                //"if(top.PopUpQueue) top.PopUpQueue('ERROR','0');}window.onload=init;</script>");
            }
        }

        public ActionResult ListAccountEntity(int id)
        {
            var account = dbc.ACCOUNTs.ToList();

            var query = (from ae in dbe.ACCOUNT_ENTITY.Where(x => x.ID_PERS_ENTI == id && x.VIG_ACCO_ENTI == true).ToList()
                         join a in account on ae.ID_ACCO equals a.ID_ACCO
                         select new
                         {
                             ae.ID_ACCO_ENTI,
                             ae.ID_ACCO,
                             a.NAM_ACCO,
                             ae.VIG_ACCO_ENTI,
                             ae.DEF_ACCO,
                             ae.VIS_ASSI,
                             ae.VIS_REQU,
                             ae.VIS_TALE,

                         }).OrderBy(x => x.NAM_ACCO);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SaveAccountEntity()
        {
            string vr = Convert.ToString(Request.Params["vr"]);
            string va = Convert.ToString(Request.Params["va"]);
            string vt = Convert.ToString(Request.Params["vt"]);
            string idaccoenti = Convert.ToString(Request.Params["idaccoenti"]);

            if (!String.IsNullOrEmpty(vr) && !String.IsNullOrEmpty(va) && !String.IsNullOrEmpty(vt) && !String.IsNullOrEmpty(idaccoenti))
            {
                int ID_ACCO_ENTI = Convert.ToInt32(idaccoenti);
                bool visreqe = Convert.ToBoolean(vr);
                bool visass = Convert.ToBoolean(va);
                bool vistale = Convert.ToBoolean(vt);

                ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);
                ae.VIS_REQU = visreqe;
                ae.VIS_ASSI = visass;
                ae.VIS_TALE = vistale;

                dbe.ACCOUNT_ENTITY.Attach(ae);
                dbe.Entry(ae).State = EntityState.Modified;
                dbe.SaveChanges();
                return Content("OK");
            }
            else
            {
                return Content("ERROR");
            }
        }

        public ActionResult RemoveAccountEntity()
        {

            string idaccoenti = Convert.ToString(Request.Params["idaccoenti"]);

            if (!String.IsNullOrEmpty(idaccoenti))
            {
                int ID_ACCO_ENTI = Convert.ToInt32(idaccoenti);

                ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);
                ae.VIG_ACCO_ENTI = false;

                dbe.ACCOUNT_ENTITY.Attach(ae);
                dbe.Entry(ae).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("OK");
            }
            else
            {
                return Content("ERROR");
            }
        }

        public ActionResult DefaultAccountEntity()
        {

            string idaccoenti = Convert.ToString(Request.Params["idaccoenti"]);
            string idpersenti = Convert.ToString(Request.Params["idpersenti"]);

            if (!String.IsNullOrEmpty(idaccoenti) && !String.IsNullOrEmpty(idpersenti))
            {
                int ID_ACCO_ENTI = Convert.ToInt32(idaccoenti);
                int ID_PERS_ENTI = Convert.ToInt32(idpersenti);

                var aee = dbe.ACCOUNT_ENTITY.Where(x => x.DEF_ACCO == true && x.VIG_ACCO_ENTI == true && x.ID_PERS_ENTI == ID_PERS_ENTI);
                if (aee.Count() > 0)
                {
                    foreach (var accenti in aee)
                    {
                        accenti.DEF_ACCO = false;

                        dbe.ACCOUNT_ENTITY.Attach(accenti);
                        dbe.Entry(accenti).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);
                ae.DEF_ACCO = true;

                dbe.ACCOUNT_ENTITY.Attach(ae);
                dbe.Entry(ae).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("OK");
            }
            else
            {
                return Content("ERROR");
            }
        }
        public ActionResult SaveNewAccountEntity()
        {
            string vr = Convert.ToString(Request.Params["vr"]);
            string va = Convert.ToString(Request.Params["va"]);
            string vt = Convert.ToString(Request.Params["vt"]);
            string idacco = Convert.ToString(Request.Params["idacco"]);
            string idpersenti = Convert.ToString(Request.Params["idpersenti"]);

            if (!String.IsNullOrEmpty(vr) && !String.IsNullOrEmpty(va) && !String.IsNullOrEmpty(vt) && !String.IsNullOrEmpty(idacco) && !String.IsNullOrEmpty(idpersenti))
            {
                int ID_ACCO = Convert.ToInt32(idacco);
                int ID_PERS_ENTI = Convert.ToInt32(idpersenti);

                bool visreqe = Convert.ToBoolean(vr);
                bool visass = Convert.ToBoolean(va);
                bool vistale = Convert.ToBoolean(vt);

                var query = dbe.ACCOUNT_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.ID_ACCO == ID_ACCO);

                if (query.Count() == 1)
                {
                    int ID_ACCO_ENTI = Convert.ToInt32(query.First().ID_ACCO_ENTI);

                    ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);
                    ae.VIG_ACCO_ENTI = true;
                    ae.DEF_ACCO = false;
                    ae.VIS_REQU = visreqe;
                    ae.VIS_ASSI = visass;
                    ae.VIS_TALE = vistale;

                    dbe.ACCOUNT_ENTITY.Attach(ae);
                    dbe.Entry(ae).State = EntityState.Modified;
                    dbe.SaveChanges();

                    return Content("OK");
                }
                else
                {
                    ACCOUNT_ENTITY ae = new ACCOUNT_ENTITY();
                    ae.ID_ACCO = ID_ACCO;
                    ae.ID_PERS_ENTI = ID_PERS_ENTI;
                    ae.VIG_ACCO_ENTI = true;
                    ae.VIS_REQU = visreqe;
                    ae.VIS_ASSI = visass;
                    ae.VIS_TALE = vistale;

                    dbe.ACCOUNT_ENTITY.Add(ae);
                    dbe.SaveChanges();

                    return Content("OK");
                }
            }
            else
            {
                return Content("ERROR");
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


        public ActionResult ExportarListaDocumentos()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            string strFechaInicioVen = Request.Params["FechaIniVencimiento1"].ToString();
            string strFechaFinVen = Request.Params["FechaFinVencimiento1"].ToString();
            DateTime FechaIniVencimiento = Convert.ToDateTime("1900/01/01");
            DateTime FechaFinVencimiento = Convert.ToDateTime("9999/12/31");

            if (!String.IsNullOrEmpty(strFechaInicioVen))
            {
                FechaIniVencimiento = Convert.ToDateTime(strFechaInicioVen);
            }
            if (!String.IsNullOrEmpty(strFechaFinVen))
            {
                FechaFinVencimiento = Convert.ToDateTime(strFechaFinVen).AddDays(1);
            }

            List<docPersonasDocumentos_Result> query = dbe.docPersonasDocumentos(ID_TYPE_DOCU, ID_PERS_ENTI, IdMarca, NAM_ATTA, FechaIniVencimiento, FechaFinVencimiento, IdGradoInstruccion, IdInstituto).ToList();

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
            sb.Append("<th class='cabecera'>Carrera Profesional</td>");
            sb.Append("<th class='cabecera'>Marca</td>");
            sb.Append("<th class='cabecera'>Tipo de Documento</td>");
            sb.Append("<th class='cabecera'>Documento</td>");
            sb.Append("<th class='cabecera'>Vigencia</td>");
            sb.Append("<th class='cabecera'>Fecha de Inicio</td>");
            sb.Append("<th class='cabecera'>Fecha de Fin</td>");
            sb.Append("<th class='cabecera'>Fecha de Creación</td>");
            sb.Append("<th class='cabecera'>Intitucion</td>");
            //sb.Append("<th class='cabecera'>Descripción</td>");
            sb.Append("</tr>");

            foreach (docPersonasDocumentos_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.USUARIO + "</td>");
                sb.Append("<td class='contenido'>" + dr.Profesion + "</td>");
                sb.Append("<td class='contenido'>" + dr.Marca + "</td>");
                sb.Append("<td class='contenido'>" + dr.NAM_DOCU + "</td>");
                sb.Append("<td class='contenido'>" + dr.NAM_ATTA + "</td>");
                sb.Append("<td class='contenido'>" + dr.Vigencia + "</td>");
                sb.Append("<td class='contenido'>" + dr.FechaInicio + "</td>");
                sb.Append("<td class='contenido'>" + dr.FechaFin + "</td>");
                sb.Append("<td class='contenido'>" + dr.FechaCreacion + "</td>");
                sb.Append("<td class='contenido'>" + dr.Institucion + "<td/>");
                //sb.Append("<td class='contenido'>" + dr.Descripción + "<td/>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=Documentacion" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult VerDocumento(string id = "", string id1 = "")
        {
            ViewBag.Flag = id;
            ViewBag.Documento = id1;
            return View();
        }
        public FileResult DescargarArchivo(string id = "", string id1 = "")
        {
            try
            {
                string ruta = "";
                if (id == "1")
                {
                    ruta = "~/Adjuntos/Talent/Documents/";
                }
                else if (id == "2")
                {
                    ruta = "~/Adjuntos/Talent/JobDescription/";
                }
                FileStream fileStream = System.IO.File.OpenRead(Server.MapPath(ruta + id1));
                MemoryStream storeStream = new MemoryStream();

                storeStream.SetLength(fileStream.Length);
                fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                byte[] byteArray = storeStream.ToArray();

                storeStream.Flush();
                fileStream.Close();
                storeStream.Close();

                Random r = new Random();
                int aleatorio = r.Next(10000, 99999);

                Response.Clear();

                //Response.ContentType = "application/octet-stream";
                if ((id1.ToLower()).Contains(".pdf"))
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".txt"))
                {
                    Response.ContentType = "text/text";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".txt");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".jpg"))
                {
                    Response.ContentType = "image/jpeg";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".jpg");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".png"))
                {
                    Response.ContentType = "image/png";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".png");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else
                {
                    string filename = ruta + id1;
                    return File(filename, "application/pdf", id1);
                }

            }
            catch
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

                return File(stream, "text/plain", "Error.txt");
            }

        }

        public ActionResult ReporteGradoInstruccion()
        {
            return View();
        }

        //LISTAR TIPOS DE DOCUMENTOS....




        public ActionResult ExportarListadocumentoTrabajadores()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int Creador = 0, TipoDocumento = 0, ID_PERS_STAT = 0, IdUsuario = 0;
            string msjError = string.Empty;
            DateTime? FechaInicio = null, FechaFin = null;



            if (int.TryParse(Convert.ToString(Request.Params["ID_ENTI"]), out Creador) == false)
            {
                Creador = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["ID_TYPE_DOCU"]), out TipoDocumento) == false)
            {
                TipoDocumento = 0;
            }

            if (Convert.ToString(Request.Params["FechaIngresoInicio"]) == "" || Convert.ToString(Request.Params["FechaIngresoInicio"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"], cultures);
            }

            if (Convert.ToString(Request.Params["FechaIngresoFin"]) == "" || Convert.ToString(Request.Params["FechaIngresoInicio"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"], cultures);
            }

            if (int.TryParse(Convert.ToString(Request.Params["ID_PERS_STAT"]), out ID_PERS_STAT) == false)
            {
                ID_PERS_STAT = 0;

            }


            List<ListarExportarExcelDocumentos_Result> query = dbe.ListarExportarExcelDocumentos(Creador, TipoDocumento, FechaInicio, FechaFin, ID_PERS_STAT).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:100px 80px;border-style:solid red 2px;border-width:1px;overflow:hidden;word-break:normal;border-color:#0000;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".cabecera{background:rgb(255, 153, 255);color:black;font-family:Arial, sans-serif;font-size:12px;font-weight:normal;padding:40px 100px;border:red solid 2px;border-width:1px;overflow:hidden;word-break:normal;border-color:#000000;border-top-width:2px;border-bottom-width:1px;}");
            sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#000000;border-top-width:2px;border-bottom-width:2px;background:#F7FDFA;margin:5px 10px;}");
            sb.Append("</style>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='cabecera'>NOMBRE</td>");
            sb.Append("<th class='cabecera'>NOMBRE ÁREA</td>");
            sb.Append("<th class='cabecera'>PUESTO</td>");
            sb.Append("<th class='cabecera'>FECHA DE INGRESO</td>");
            sb.Append("<th class='cabecera'>FICHA DE INGRESO (PDF)</td>");
            sb.Append("<th class='cabecera'>DNI o CE</td>");
            sb.Append("<th class='cabecera'>ANTECEDENTES PENALES</td>");
            sb.Append("<th class='cabecera'>ANTECEDENTES POLICIALES</td>");
            sb.Append("<th class='cabecera'>CERTIADULTO</td>");
            sb.Append("<th class='cabecera'>CERTIFICADO DOMICILIARIO</td>");
            sb.Append("<th class='cabecera'>RECIBO DE SERVICIOS DE LUZ, AGUA O TELÉFONO</td>");
            sb.Append("<th class='cabecera'>FOTO (PDF)</td>");
            sb.Append("<th class='cabecera'>DNI DE CÓNYUGE O CONVIVIENTE</td>");
            sb.Append("<th class='cabecera'>PARTIDA DE MATRIMONIO</td>");
            sb.Append("<th class='cabecera'>UNIÓN DE HECHO (EN CASO DE CONVIVIENTE)</td>");
            sb.Append("<th class='cabecera'>DNI DE HIJOS (MENORES DE 18 AÑOS)</td>");
            sb.Append("<th class='cabecera'>DNI DE HIJOS (MAYORES DE 18 AÑOS Y HASTA 24 AÑOS) SOLO EN EL CASO SE ENCUENTREN ESTUDIANDO ACTUALMENTE Y ADJUNTAR EL CERTIFICADO DE ESTUDIOS Y/O PAGOS REALIZADOS DEL CICLO ACTUAL</td>");
            sb.Append("<th class='cabecera'>CURRICULUM VITAE</td>");
            sb.Append("<th class='cabecera'>CURRICULUM VITAE  EDATA</td>");
            sb.Append("<th class='cabecera'>CONSTANCIA DE ESTUDIOS (Si actualmente tienes estudios técnicos o universitarios)</td>");
            sb.Append("<th class='cabecera'>CONSTANCIA DE EGRESADO (ESTUDIANTE UNIVERSITARIO)</td>");
            sb.Append("<th class='cabecera'>TÉCNICO TITULADO</td>");
            sb.Append("<th class='cabecera'>BACHILLER</td>");
            sb.Append("<th class='cabecera'>TÍTULO UNIVERSITARIO</td>");
            sb.Append("<th class='cabecera'>COLEGIATURA</td>");
            sb.Append("<th class='cabecera'>MAESTRÍA</td>");
            sb.Append("<th class='cabecera'>CERTIFICACIONES</td>");
            sb.Append("<th class='cabecera'>CURSOS</td>");
            sb.Append("<th class='cabecera'>DIPLOMADOS O ESPECIALIZACIONES</td>");
            sb.Append("<th class='cabecera'>CONSTANCIA DE TRABAJO</td>");
            sb.Append("<th class='cabecera'>REPORTE DE RENTAS DE 5TA SUNAT- OTROS EMPLEADORES</td>");
            sb.Append("<th class='cabecera'>CARGO DE ENTREGA DE RENTAS DE 5TA CATEGORÍA</td>");
            sb.Append("<th class='cabecera'>DECLARACIÓN JURADA DE 5TA CATEGORÍA</td>");
            sb.Append("<th class='cabecera'>DECLARACIÓN JURADA DE VIDA LEY</td>");
            sb.Append("<th class='cabecera'>LICENCIA DE MANEJO</td>");
            sb.Append("<th class='cabecera'>PASAPORTE</td>");
            sb.Append("<th class='cabecera'>CERTIFICACIONES TRADUCIDAS</td>");


            // no concuerdan datos

            sb.Append("<th class='cabecera'>CARTA DE OFERTA</td>");
            sb.Append("<th class='cabecera'>INFORMACIÓN DERECHOHABIENTES</td>");
            sb.Append("<th class='cabecera'>CONSTANCIA DE GRADO ACADÉMICO</td>");
            sb.Append("<th class='cabecera'>FORMATO EPS</td>");

            sb.Append("</tr>");

            foreach (ListarExportarExcelDocumentos_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.NombreColaborador + "</td>");
                sb.Append("<td class='contenido'>" + dr.NombreArea + "</td>");
                sb.Append("<td class='contenido'>" + dr.CargoEntrega5taCategoria + "</td>");
                sb.Append("<td class='contenido'>" + dr.Inicio + "</td>");
                sb.Append("<td class='contenido'>" + dr.FichaIngreso + "</td>");
                sb.Append("<td class='contenido'>" + dr.DNI + "</td>");
                sb.Append("<td class='contenido'>" + dr.AntecedentePenales + "</td>");
                sb.Append("<td class='contenido'>" + dr.AntecedentePoliciales + "</td>");
                sb.Append("<td class='contenido'>" + dr.Certiadulto + "</td>");
                sb.Append("<td class='contenido'>" + dr.CertificadoDomiciliario + "</td>");
                sb.Append("<td class='contenido'>" + dr.ReciboServicios + "</td>");
                sb.Append("<td class='contenido'>" + dr.Foto + "</td>");
                sb.Append("<td class='contenido'>" + dr.DNIConyugue + "</td>");
                sb.Append("<td class='contenido'>" + dr.PartidaMatrimonio + "</td>");
                sb.Append("<td class='contenido'>" + dr.EscrituraPublicaUnion + "</td>");
                sb.Append("<td class='contenido'>" + dr.DNIHijos18Menores + "</td>");
                sb.Append("<td class='contenido'>" + dr.DNIhijosDe18 + "</td>");
                sb.Append("<td class='contenido'>" + dr.CV + "</td>");
                sb.Append("<td class='contenido'>" + dr.CurriculumEdata + "</td>");
                sb.Append("<td class='contenido'>" + dr.EstudianteUniversitario + "</td>");
                sb.Append("<td class='contenido'>" + dr.ConstanciaEgresado + "</td>");
                sb.Append("<td class='contenido'>" + dr.TecnicoTitulado + "</td>");
                sb.Append("<td class='contenido'>" + dr.Bachiller + "</td>");
                sb.Append("<td class='contenido'>" + dr.TituloUniversitario + "</td>");
                sb.Append("<td class='contenido'>" + dr.Colegiatura + "</td>");
                sb.Append("<td class='contenido'>" + dr.Maestria + "</td>");
                sb.Append("<td class='contenido'>" + dr.Certificaciones + "</td>");
                sb.Append("<td class='contenido'>" + dr.Cursos + "</td>");
                sb.Append("<td class='contenido'>" + dr.DiplomadosEspecializaciones + "</td>");
                sb.Append("<td class='contenido'>" + dr.ConstanciaTrabajo + "</td>");
                sb.Append("<td class='contenido'>" + dr.ReporteSunat + "</td>");
                sb.Append("<td class='contenido'>" + dr.CargoEntrega5taCategoria + "</td>");
                sb.Append("<td class='contenido'>" + dr.DeclaracionJuradaCategoria + "</td>");
                sb.Append("<td class='contenido'>" + dr.DeclaraciondeVidaLey + "</td>");
                sb.Append("<td class='contenido'>" + dr.LicenciaManejo + "</td>");
                sb.Append("<td class='contenido'>" + dr.Pasaporte + "</td>");
                sb.Append("<td class='contenido'>" + dr.CertificacionesTraducidas + "</td>");

                //------
                sb.Append("<td class='contenido'>" + dr.CartaOferta + "</td>");
                sb.Append("<td class='contenido'>" + dr.InformacionDerechohabientes + "</td>");
                sb.Append("<td class='contenido'>" + dr.ConstanciaGradoAcademico + "</td>");
                sb.Append("<td class='contenido'>" + dr.FormatoEPS + "</td>");
                sb.Append("</tr>");


            }
            sb.Append("</table>");
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=listadocumentos" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }




        public ActionResult ListarEstadoColaborador()
        {
            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);


            var result = dbe.ListarEstadoColaborador().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ListarPersonalxDocumentos()
        {
            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);


            var result = dbe.ListarPersonalxDocumentos().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ListarTipoDocumentos()
        {
            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);


            var result = dbe.ListarTipoDocumentosTalento().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }


        //public ActionResult ListarTablaDocumentos_User()
        //{
        //    int sw = 0, Creador = 0, TipoDocumento = 0, ID_PERS_STAT = 0;
        //    string msjError = string.Empty;
        //    DateTime? FechaInicio = null, FechaFin = null;
        //    if (int.TryParse(Convert.ToString(Request.Params["ID_ENTI"]), out Creador) == false)
        //    {
        //        Creador = 0;
        //    }
        //    if (int.TryParse(Convert.ToString(Request.Params["ID_TYPE_DOCU"]), out TipoDocumento) == false)
        //    {
        //        TipoDocumento = 0;
        //    }

        //    if (Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == null)
        //    {
        //        FechaInicio = null;
        //    }
        //    else
        //    {
        //        CultureInfo cultures = new CultureInfo("es-ES");
        //        FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"], cultures);
        //    }

        //    if (Convert.ToString(Request.QueryString["FechaIngresoFin"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoFin"]) == null)
        //    {
        //        FechaFin = null;
        //    }
        //    else
        //    {
        //        CultureInfo cultures = new CultureInfo("es-ES");
        //        FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"], cultures);
        //    }


        //    if (int.TryParse(Convert.ToString(Request.Params["ID_PERS_STAT"]), out ID_PERS_STAT) == false)
        //    {
        //        ID_PERS_STAT = 0;
        //    }
        //    var result = dbe.ListarTablaDocumentos(Creador, TipoDocumento, FechaInicio, FechaFin, ID_PERS_STAT).ToList();

        //    //var result = dbe.ListarTablaDocumentos().ToList();

        //    return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        //}


        //FIN DE LISTAR TIPO DE DOCUMENTOS

        public ActionResult DescargarPlantillaExcel()
        {
            string rutaArchivo = Server.MapPath("~/Adjuntos/Talent/Plantilla ingresos.xlsx");

            return File(rutaArchivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla ingresos.xlsx");
        }

        [Authorize]
        public ActionResult NewGroupProfile()
        {
            ViewBag.TODAY = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
            ViewBag.anio = DateTime.Today.Year;
            return View();
        }

        [HttpPost]
        public ActionResult NewGroupProfile(HttpPostedFileBase excelFile)
        {
            ViewBag.TODAY = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
            ViewBag.anio = DateTime.Today.Year;

            if (excelFile != null && excelFile.ContentLength > 0)
            {
                var listPersonEntity = new List<PERSON_ENTITY>();
                var listPersonContract = new List<PERSON_CONTRACT>();
                var myModelList = new List<CLASS_ENTITY>();
                var listAccount = new List<Account_Entity_Excel>();
                var listDatosExcel = new List<Datos_Excel>();

                try
                {
                    using (var excelWorkbook = new XLWorkbook(excelFile.InputStream))
                    {
                        var excelWorksheet = excelWorkbook.Worksheet(1);

                        if (excelWorksheet.Name != "Colaboradores")
                        {
                            ViewBag.MsgError = "Ha ocurrido un error, contacte al administrador";
                            return View();
                        }

                        var rowCount = excelWorksheet.RowsUsed().Count();

                        for (int i = 2; i <= rowCount; i++)
                        {
                            var rowData = new CLASS_ENTITY()
                            {
                                FIR_NAME = excelWorksheet.Cell(i, 1).Value != null ? excelWorksheet.Cell(i, 1).Value.ToString().Trim() : string.Empty,
                                SEC_NAME = excelWorksheet.Cell(i, 2).Value != null ? excelWorksheet.Cell(i, 2).Value.ToString().Trim() : string.Empty,
                                THI_NAME = excelWorksheet.Cell(i, 3).Value != null ? excelWorksheet.Cell(i, 3).Value.ToString().Trim() : string.Empty,
                                LAS_NAME = excelWorksheet.Cell(i, 4).Value != null ? excelWorksheet.Cell(i, 4).Value.ToString().Trim() : string.Empty,
                                MOT_NAME = excelWorksheet.Cell(i, 5).Value != null ? excelWorksheet.Cell(i, 5).Value.ToString().Trim() : string.Empty,
                                SEX_ENTI = excelWorksheet.Cell(i, 6).Value != null ? GetSexEnti(excelWorksheet.Cell(i, 6).Value.ToString()) : string.Empty,
                                ID_CIVI_STAT = excelWorksheet.Cell(i, 7).Value != null ? GetIdCiviStat(excelWorksheet.Cell(i, 7).Value.ToString()) : null,
                                ADDRESS = excelWorksheet.Cell(i, 9).Value != null ? excelWorksheet.Cell(i, 9).Value.ToString().Trim() : string.Empty,
                                TEL_ENTI = excelWorksheet.Cell(i, 10).Value != null ? excelWorksheet.Cell(i, 10).Value.ToString().Trim() : string.Empty,
                                CEL_ENTI = excelWorksheet.Cell(i, 11).Value != null ? excelWorksheet.Cell(i, 11).Value.ToString().Trim() : string.Empty,
                                EMA_ENTI = excelWorksheet.Cell(i, 12).Value != null ? excelWorksheet.Cell(i, 12).Value.ToString().Trim() : string.Empty,
                                ID_NATI = excelWorksheet.Cell(i, 14).Value != null ? GetNati(excelWorksheet.Cell(i, 14).Value.ToString()) : null,
                                ID_DEGR_INST = excelWorksheet.Cell(i, 15).Value != null ? GetDegrInst(excelWorksheet.Cell(i, 15).Value.ToString()) : null,
                                ID_TYPE_DI = excelWorksheet.Cell(i, 16).Value != null ? GetTypeDi(excelWorksheet.Cell(i, 16).Value.ToString()) : null,
                                CONTACT_NAME = excelWorksheet.Cell(i, 18).Value != null ? excelWorksheet.Cell(i, 18).Value.ToString().Trim() : string.Empty,
                                CONTACT_PHONE = excelWorksheet.Cell(i, 19).Value != null ? excelWorksheet.Cell(i, 19).Value.ToString().Trim() : string.Empty,
                                ID_BLOO_GROU = excelWorksheet.Cell(i, 20).Value != null ? GetBlooGrou(excelWorksheet.Cell(i, 20).Value.ToString()) : null,
                                IdProfesion = excelWorksheet.Cell(i, 21).Value != null ? GetProfesion(excelWorksheet.Cell(i, 21).Value.ToString()) : null,
                                IdUniversidad = excelWorksheet.Cell(i, 22).Value != null ? GetUniversidad(excelWorksheet.Cell(i, 22).Value.ToString()) : null,
                            };

                            if (excelWorksheet.Cell(i, 17).Value != null && int.TryParse(excelWorksheet.Cell(i, 17).Value.ToString().Trim(), out int _)) rowData.NUM_TYPE_DI = excelWorksheet.Cell(i, 17).Value.ToString().Trim();

                            var personEntity = new PERSON_ENTITY()
                            {
                                EMA_PERS = excelWorksheet.Cell(i, 13).Value != null ? excelWorksheet.Cell(i, 13).Value.ToString().Trim() : string.Empty,
                                ID_PERS_STAT = excelWorksheet.Cell(i, 23).Value != null ? GetEstado(excelWorksheet.Cell(i, 23).Value.ToString()) : 1,
                                ID_LOCA = excelWorksheet.Cell(i, 30).Value != null ? GetLocation(excelWorksheet.Cell(i, 30).Value.ToString()) : null,
                                ID_SUB_CIA = excelWorksheet.Cell(i, 31).Value != null ? GetIdSubCia(excelWorksheet.Cell(i, 31).Value.ToString()) : null,
                                CEL_PERS = excelWorksheet.Cell(i, 32).Value != null ? excelWorksheet.Cell(i, 32).Value.ToString().Trim() : string.Empty,
                                EXT_PERS = excelWorksheet.Cell(i, 33).Value != null ? excelWorksheet.Cell(i, 33).Value.ToString().Trim() : string.Empty,
                                RPM_PERS = excelWorksheet.Cell(i, 34).Value != null ? excelWorksheet.Cell(i, 34).Value.ToString().Trim() : string.Empty,
                                EMA_ELEC = excelWorksheet.Cell(i, 35).Value != null ? excelWorksheet.Cell(i, 35).Value.ToString().Trim() : string.Empty,
                                ID_AREA = excelWorksheet.Cell(i, 36).Value != null ? GetArea(excelWorksheet.Cell(i, 36).Value.ToString()) : null,
                            };

                            var personContract = new PERSON_CONTRACT();

                            if (excelWorksheet.Cell(i, 25).Value != null) personContract.ID_COND_CONT = GetCondCont(excelWorksheet.Cell(i, 25).Value.ToString());
                            if (excelWorksheet.Cell(i, 28).Value != null && decimal.TryParse(excelWorksheet.Cell(i, 28).Value.ToString().Trim(), out decimal _)) personContract.GROSS_SALARY = Convert.ToDecimal(excelWorksheet.Cell(i, 28).Value);

                            var datosExcel = new Datos_Excel();

                            datosExcel.BIRTHDAY = string.Empty;
                            if (excelWorksheet.Cell(i, 8).Value != null && excelWorksheet.Cell(i, 8).Value is DateTime dateValueBir)
                            {
                                datosExcel.BIRTHDAY = dateValueBir.ToString("d/M/yyyy", CultureInfo.InvariantCulture);
                            }

                            datosExcel.STAR_DATE = string.Empty;
                            if (excelWorksheet.Cell(i, 26).Value != null && excelWorksheet.Cell(i, 26).Value is DateTime dateValueStaDat)
                            {
                                datosExcel.STAR_DATE = dateValueStaDat.ToString("d/M/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                            }

                            datosExcel.END_DATE = string.Empty;
                            if (excelWorksheet.Cell(i, 27).Value != null && excelWorksheet.Cell(i, 27).Value is DateTime dateValueEndDat)
                            {
                                datosExcel.END_DATE = dateValueEndDat.ToString("d/M/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                            }

                            datosExcel.ID_CHAR = "0";
                            datosExcel.NAM_CHAR = string.Empty;
                            if (excelWorksheet.Cell(i, 29).Value != null && int.TryParse(excelWorksheet.Cell(i, 29).Value.ToString().Trim(), out int _))
                            {
                                var cargo = GetCargo(excelWorksheet.Cell(i, 29).Value.ToString().Trim());
                                if (cargo != "")
                                {
                                    datosExcel.ID_CHAR = excelWorksheet.Cell(i, 29).Value.ToString().Trim();
                                    datosExcel.NAM_CHAR = cargo;
                                }
                            }

                            var accountEntity = new Account_Entity_Excel();
                            if (excelWorksheet.Cell(i, 38).Value != null) accountEntity.IDQUEU = GetIdQueu(excelWorksheet.Cell(i, 38).Value.ToString());
                            if (excelWorksheet.Cell(i, 39).Value != null) accountEntity.VISREQU = GetBoolAccount(excelWorksheet.Cell(i, 39).Value.ToString());
                            if (excelWorksheet.Cell(i, 40).Value != null) accountEntity.VISASSI = GetBoolAccount(excelWorksheet.Cell(i, 40).Value.ToString());

                            listAccount.Add(accountEntity);
                            listPersonEntity.Add(personEntity);
                            listPersonContract.Add(personContract);
                            listDatosExcel.Add(datosExcel);
                            myModelList.Add(rowData);
                        }
                    }

                    ViewBag.AccountEntity = listAccount;
                    ViewBag.PersonEntity = listPersonEntity;
                    ViewBag.PersonContract = listPersonContract;
                    ViewBag.DatosExcel = listDatosExcel;

                    return View("NewGroupProfile", myModelList);
                }
                catch (Exception ex)
                {

                    ViewBag.MsgError = "error2" + ex.ToString();
                    return View();
                }
            }
            else
            {
                ViewBag.MsgError = "Seleccione un Excel";
                return View();
            }

        }

        private string GetCargo(string text)
        {
            int id = Int32.Parse(text);

            var result = dbe.CHARTs
                .Where(c => c.ID_CHAR == id)
                .Join(
                    dbe.NAME_CHART,
                    chart => chart.ID_NAM_CHAR,
                    nameChart => nameChart.ID_NAM_CHAR,
                    (chart, nameChart) => new { Chart = chart, NameChart = nameChart }
                ).Select(joinResult => new
                {
                    Chart = joinResult.Chart,
                    NameChart = joinResult.NameChart
                })
                .FirstOrDefault();

            if (result == null) return "";
            return result.NameChart.NAM_CHAR;
        }

        private bool GetBoolAccount(string text)
        {
            return text == "SI";
        }

        private Nullable<int> GetIdQueu(string text)
        {
            //return dbc.QUEUEs.FirstOrDefault(q => q.DES_QUEU == text).ID_QUEU;
            int id = (from aq in dbc.ACCOUNT_QUEUE
                      join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                      where aq.ID_ACCO == 4 && q.DES_QUEU == text
                      select q.ID_QUEU).FirstOrDefault();
            if (id == 0) return null;

            return id;
        }

        private int GetListAccounts(string text)
        {
            return dbc.ACCOUNTs.FirstOrDefault(q => q.NAM_ACCO == text).ID_ACCO;
        }

        private string GetSexEnti(string text)
        {
            if (text == "MALE")
            {
                return "M";
            }
            else if (text == "FEMALE")
            {
                return "F";
            }
            else
            {
                return string.Empty;
            }
        }

        private Nullable<int> GetIdCiviStat(string text)
        {
            switch (text)
            {
                case "SOLTERO(A)":
                    return 1;
                case "CASADO(A)":
                    return 2;
                case "VIUDO(A)":
                    return 3;
                case "DIVORCIADO(A)":
                    return 4;
                case "CONVIVIENTE":
                    return 5;
                default:
                    return null;
            }
        }

        private Nullable<int> GetIdSubCia(string text)
        {
            if (text == "NET CARE SAC")
            {
                return 4550;
            }
            else if (text == "GRUPO ELECTRODATA SAC")
            {
                return 9;
            }
            else
            {
                return null;
            }
        }

        private Nullable<int> GetEstado(string text)
        {
            int id = dbe.PERSON_STATUS.FirstOrDefault(q => q.NAM_STAT == text)?.ID_PERS_STAT ?? 0;
            if (id == 0) return 1;

            return id;
        }

        private Nullable<int> GetTypeDi(string text)
        {
            int id = dbe.TYPE_DOCUMENTIDENT.FirstOrDefault(q => q.NAM_TYPE_DI == text)?.ID_TYPE_DI ?? 0;
            if (id == 0) return null;

            return id;
        }

        private Nullable<int> GetNati(string text)
        {
            int id = dbe.NATIONALITies.FirstOrDefault(q => q.NAM_NATI == text)?.ID_NATI ?? 0;
            if (id == 0) return null;

            return id;
        }

        private Nullable<int> GetDegrInst(string text)
        {
            int id = dbe.DEGREE_INSTRUCTION.FirstOrDefault(q => q.NAM_DEGR_INST == text)?.ID_DEGR_INST ?? 0;
            if (id == 0) return null;

            return id;
        }

        private Nullable<int> GetBlooGrou(string text)
        {
            int id = dbe.BLOOD_GROUP.FirstOrDefault(q => q.NAM_BLOO_GROU == text)?.ID_BLOO_GROU ?? 0;
            if (id == 0) return null;

            return id;
        }

        private Nullable<int> GetProfesion(string text)
        {
            int id = dbe.Profesions.FirstOrDefault(q => q.Nombre == text)?.Id ?? 0;
            if (id == 0) return null;

            return id;
        }

        private Nullable<int> GetUniversidad(string text)
        {
            int id = dbe.Universidad.FirstOrDefault(q => q.Nombre == text)?.Id ?? 0;
            if (id == 0) return null;

            return id;
        }

        private int GetCondCont(string text)
        {
            int resul = 3;

            switch (text)
            {
                case "PLANILLA":
                    resul = 1;
                    break;
                case "RECIBO POR HONORARIOS":
                    resul = 2;
                    break;
                case "PRACTICANTE":
                    resul = 3;
                    break;
            }

            return resul;
        }

        private Nullable<int> GetLocation(string text)
        {
            int id = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var query = dbc.SITEs.Where(s => s.ID_ACCO == ID_ACCO);

            var result = (from s in query.ToList()
                          join l in dbc.LOCATIONs on s.ID_SITE equals l.ID_SITE
                          select new
                          {
                              l.ID_LOCA,
                              s.ID_SITE,
                              l.NAM_LOCA,
                              s.NAM_SITE
                          });

            foreach (var item in result)
            {
                if ((item.NAM_SITE + " - " + item.NAM_LOCA) == text) id = item.ID_LOCA;
            }
            if (id == 0) return null;

            return id;
        }

        private Nullable<int> GetArea(string text)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbe.AREAs.Where(a => a.ID_ACCO == ID_ACCO);

            var result = (from a in query.ToList()
                          select new
                          {
                              a.ID_AREA,
                              NAM_AREA = a.NOM_AREA
                          }).ToList();

            int id = result.FirstOrDefault(q => q.NAM_AREA == text)?.ID_AREA ?? 0;
            if (id == 0) return null;

            return id;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewProfiles(IEnumerable<HttpPostedFileBase> filesPhoto, IEnumerable<HttpPostedFileBase> filesDocs, CLASS_ENTITY cl_enti)
        {
            try
            {
                string S_FIR_NAME = string.Empty;
                string S_SEC_NAME = string.Empty;
                string S_THI_NAME = string.Empty;
                string S_LAS_NAME = string.Empty;
                string S_EMA_ENTI = string.Empty;
                string S_MOT_NAME = string.Empty;
                string S_BIRTHDAY = string.Empty;

                S_FIR_NAME = Convert.ToString(Request.Params["FIR_NAME"].ToString()).ToUpper();
                S_SEC_NAME = Convert.ToString(Request.Params["SEC_NAME"].ToString()).ToUpper();
                S_THI_NAME = Convert.ToString(Request.Params["THI_NAME"].ToString()).ToUpper();
                S_LAS_NAME = Convert.ToString(Request.Params["LAS_NAME"].ToString()).ToUpper();
                S_EMA_ENTI = Convert.ToString(Request.Params["EMA_ENTI"].ToString()).ToUpper();
                S_MOT_NAME = Convert.ToString(Request.Params["MOT_NAME"].ToString()).ToUpper();
                //string S_EMA_PERS = Convert.ToString(Request.Params["EMA_PERS"].ToString()).ToUpper();


                string S_EMA_ELEC = Convert.ToString(Request.Params["EMA_ELEC"].ToString()).ToUpper();

                var DniDuplicado = (from ce in dbe.CLASS_ENTITY
                                    where ce.NUM_TYPE_DI == cl_enti.NUM_TYPE_DI
                                    select new
                                    {
                                        ce.ID_ENTI,
                                    });

                int sw = 0, Error = 0, ID_AREA = 0, ID_PERS_STAT = 0,
                    ID_LOCA = 0, ID_TYPE_DI, ID_COND_CONT, ID_CHAR = 0, ID_SUB_CIA = 0;
                string msjError = string.Empty;

                DateTime STAR_DATE, END_DATE, BIRTHDAY;

                //var fecha = String.Format("{0:M/d/yyyy}", BIRTHDAY);
                var cont = new PERSON_CONTRACT();

                ID_CHAR = Convert.ToInt32(Request.Form["ID_CHAR"]);
                //ID_AREA = Convert.ToInt32(Request.Params["ID_AREA"]);
                if (S_FIR_NAME.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Primer Nombre."; }
                else if (S_LAS_NAME.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Apellido Paterno."; }
                else if (S_MOT_NAME.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Apellido Materno."; }
                else if (Convert.ToString(Request.Params["BIRTHDAY"].ToString()) == "") { sw = 1; msjError = "Se debe ingresar la fecha de nacimiento."; }
                else if (S_EMA_ENTI.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Email Personal."; }
                else if (cl_enti.NUM_TYPE_DI == null) { sw = 1; msjError = "Ingrese el número de documento de identidad."; }
                else if (DniDuplicado.Count() >= 1) { sw = 1; msjError = "El número de documento de identidad ya esta registrado."; }
                else if (cl_enti.IdProfesion == null) { sw = 1; msjError = "Se debe ingresar la Carrera Profesional."; }
                //else if (Int32.TryParse(Convert.ToString(Request.Form["ID_AREA"]), out ID_AREA) == false) { sw = 1; msjError = "Se debe ingresar el Area."; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_DI"]), out ID_TYPE_DI) == false) { sw = 1; msjError = "Se debe ingresar el Primer Nombre."; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_PERS_STAT"]), out ID_PERS_STAT) == false) { sw = 1; msjError = "Se debe ingresar el Status."; }
                else if (S_EMA_ELEC.Trim().Length == 0) { sw = 1; msjError = "Se debe ingresar el Email de Electrodata."; }
                if (ID_PERS_STAT == 1 || ID_PERS_STAT == 2)
                {

                    if (Convert.ToString(Request.Params["STAR_DATE"].ToString()) == "") { sw = 1; msjError = "Se debe ingresar la fecha de Inicio del Contrato."; }
                    else if (Convert.ToString(Request.Params["END_DATE"].ToString()) == "") { sw = 1; msjError = "Se debe ingresar la fecha de Fin del Contrato."; }
                    else if (DateTime.ParseExact(Convert.ToString(Request.Params["STAR_DATE"].ToString()), "d/M/yyyy hh:mm tt", null) > DateTime.ParseExact(Convert.ToString(Request.Params["END_DATE"].ToString()), "d/M/yyyy hh:mm tt", null)) { sw = 1; Error = 2; msjError = "La fecha de Inicio de Contrato no puede ser Mayor a la de Fin de Contrato."; }
                    else if (Int32.TryParse(Convert.ToString(Request.Form["ID_COND_CONT"]), out ID_COND_CONT) == false) { sw = 1; msjError = "Se debe ingresar la condición de Contrato."; }
                    else if (ID_CHAR == 0) { sw = 1; msjError = "Se debe ingresar el puesto."; }
                    else if (Int32.TryParse(Convert.ToString(Request.Form["ID_LOCA"]), out ID_LOCA) == false) { sw = 1; msjError = "Se debe ingresar el Lugar de Trabajo."; }
                    else if (Int32.TryParse(Convert.ToString(Request.Form["PLANILLA"]), out ID_SUB_CIA) == false) { sw = 1; msjError = "Se debe ingresar la Compañia."; }
                    // else if (filesDocs == null) { sw = 1; Error = 3; msjError = "Se debe Cargar el Documento del Contrato."; }
                    else
                    {

                        cont.STAR_DATE = DateTime.ParseExact(Convert.ToString(Request.Params["STAR_DATE"].ToString()), "d/M/yyyy hh:mm tt", null);
                        cont.END_DATE = DateTime.ParseExact(Convert.ToString(Request.Params["END_DATE"].ToString()), "d/M/yyyy hh:mm tt", null);
                        cont.ID_COND_CONT = ID_COND_CONT;
                    }
                }
                if (sw == 1)
                {
                    return Json(new { success = false, message = msjError });
                    //return Content("<script type='text/javascript'>function init() {" +
                    //    "if(top.mensaje){ top.mensaje('ERROR','" + msjError + "');}}window.onload=init;</script>");
                }
                //string S_EMA_ENTI = Convert.ToString(Request.Params["EMA_ENTI"].ToString()).ToUpper();
                int UserId = Convert.ToInt32(Session["UserId"]);
                cl_enti.FIR_NAME = S_FIR_NAME;
                cl_enti.SEC_NAME = S_SEC_NAME;
                cl_enti.THI_NAME = S_THI_NAME;
                cl_enti.LAS_NAME = S_LAS_NAME;
                cl_enti.MOT_NAME = (cl_enti.MOT_NAME == null ? null : cl_enti.MOT_NAME.ToUpper());
                cl_enti.EMA_ENTI = S_EMA_ENTI;
                cl_enti.CREATED = DateTime.Now;
                //BIRTHDAY = DateTime.ParseExact(Convert.ToString(Request.Params["BIRTHDAY"].ToString()), "d/M/yyyy", null);
                cl_enti.BIRTHDAY = DateTime.ParseExact(Convert.ToString(Request.Params["BIRTHDAY"].ToString()), "d/M/yyyy", null);
                cl_enti.VIG_ENTI = true;
                cl_enti.ID_TYPE_ENTI = 2;
                cl_enti.ADDRESS = (cl_enti.ADDRESS == null ? null : cl_enti.ADDRESS.ToUpper());
                cl_enti.ID_USER = UserId;
                dbe.CLASS_ENTITY.Add(cl_enti);
                dbe.SaveChanges();

                string S_CEL_PERS = Convert.ToString(Request.Params["CEL_PERS"].ToString());
                string S_EXT_PERS = Convert.ToString(Request.Params["EXT_PERS"].ToString());
                string S_TEL_RPM = Convert.ToString(Request.Params["TEL_RPM"].ToString());


                string S_EMA_PERS = Convert.ToString(Request.Params["EMA_PERS"].ToString()).ToUpper();


                var pers_enti = new PERSON_ENTITY();
                var S_ID_ENTI1 = Convert.ToString(Request.Form["ID_ENTI1"].ToString());

                int ID_ACCO_PERT = dbc.ACCOUNT_PARAMETER.Single(x => x.VAL_ACCO_PARA == S_ID_ENTI1
                    && x.ID_PARA == 6 && x.VIG_ACCO_PARA == true).ID_ACCO.Value;


                //------------------------------------------------------------------
                try { ID_SUB_CIA = Convert.ToInt32(Request.Params["PLANILLA"]); }
                catch { ID_SUB_CIA = 0; }

                pers_enti.ID_ACCO_PERT = ID_ACCO_PERT;
                pers_enti.ID_ENTI1 = Convert.ToInt32(S_ID_ENTI1);
                pers_enti.ID_ENTI2 = cl_enti.ID_ENTI;
                pers_enti.ID_AREA = ID_AREA;
                pers_enti.ID_PRIO = 4;

                pers_enti.ID_SUB_CIA = ID_SUB_CIA;


                pers_enti.ID_TYPE_CLIE = 2;
                pers_enti.CEL_PERS = S_CEL_PERS;
                pers_enti.RPM_PERS = S_TEL_RPM;
                pers_enti.EXT_PERS = S_EXT_PERS;
                pers_enti.EMA_ELEC = S_EMA_ELEC;
                pers_enti.EMA_PERS = S_EMA_PERS;
                pers_enti.CREATED = DateTime.Now;
                pers_enti.VIG_PERS_ENTI = true;
                pers_enti.ID_PERS_STAT = ID_PERS_STAT;
                pers_enti.ID_FOTO = (cl_enti.SEX_ENTI == "M" ? 0 : -1);
                pers_enti.EPS = (Convert.ToInt32(Request.Form["EPS_HF"].ToString()) == 1 ? true : false);
                //pers_enti.NroColegiatura = Request.Form["NRO_COLEGIATURA"];
                dbe.PERSON_ENTITY.Add(pers_enti);
                dbe.SaveChanges();

                //--------------------------FOT_PERS -->RELATIVO--------------------

                var fotpers = (from pe in dbe.PERSON_ENTITY
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                               join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                               where (pe.ID_ENTI1 == 9 || pe.ID_ENTI1 == 4550 || pe.ID_ENTI1 == 3975)
                               where pc.VIG_CONT == true
                               where ae.ID_ACCO == 4 && ae.VIG_ACCO_ENTI == true
                               select new
                               {
                                   pe.ID_PERS_ENTI,
                               });

                string FOT_PERS = Convert.ToString(pers_enti.ID_PERS_ENTI) + Convert.ToString(fotpers.Count().ToString("D4"));

                int id_hora = 0;
                if (ID_LOCA == 81) { id_hora = 1; }
                else if (ID_LOCA == 82) { id_hora = 2; }
                else { id_hora = 0; }

                pers_enti.FOT_PERS = FOT_PERS;
                pers_enti.ID_HORA = id_hora;


                //---------------------CANASTA NAVIDEÑA----------------------------------

                int CanNav = Convert.ToInt32(Request.Params["CanNav"]);
                var chribask = new CHRISTMAN_BASKET();
                if (CanNav == 1)
                {
                    chribask.ID_PERS_ENTI = pers_enti.ID_PERS_ENTI;
                    chribask.YEA_CHRI_BASK = DateTime.Today.Year;
                    chribask.CREATED = DateTime.Now;
                    chribask.VIG_CHRI_BASK = true;

                    dbe.CHRISTMAN_BASKET.Add(chribask);
                    dbe.SaveChanges();

                }

                //----------------------------EPS--------------------------------------------

                int epsanual = Convert.ToInt32(Request.Params["EPSANUAL"]);
                var epsyearly = new EPS_YEARLY();
                if (epsanual == 1)
                {
                    epsyearly.ID_PERS_ENTI = pers_enti.ID_PERS_ENTI;
                    epsyearly.YEA_EPS_YEAR = DateTime.Today.Year;
                    epsyearly.CREATED = DateTime.Now;
                    epsyearly.VIG_EPS_YEAR = true;

                    dbe.EPS_YEARLY.Add(epsyearly);
                    dbe.SaveChanges();

                }

                //-------------------------------------------------------------------

                int ID_PERS_ENTI = pers_enti.ID_PERS_ENTI;
                if (filesPhoto != null)
                {

                    WebImage img = WebImage.GetImageFromRequest("filesPhoto");
                    if (img != null)
                    {   //Redimensionando la imagen
                        int alt = img.Height;
                        int anc = img.Width;
                        decimal por = 0;
                        if (alt > anc && alt > 84)
                        {
                            por = 84 / Convert.ToDecimal(alt);
                            alt = 84;
                            anc = Convert.ToInt32(Convert.ToDecimal(anc) * por);
                        }
                        if (anc > alt && anc > 84)
                        {
                            por = 84 / Convert.ToDecimal(anc);
                            anc = 84;
                            alt = Convert.ToInt32(Convert.ToDecimal(alt) * por);
                        }
                        img.Resize(anc, alt);
                        img.Save(Server.MapPath("~/Content/Fotos/" + ID_PERS_ENTI.ToString() + ".jpg"));
                    }

                    pers_enti.ID_FOTO = ID_PERS_ENTI;
                }

                //Guardando la Actualización

                dbe.PERSON_ENTITY.Attach(pers_enti);
                dbe.Entry(pers_enti).State = EntityState.Modified;
                dbe.SaveChanges();

                var acco_enti = new ACCOUNT_ENTITY();

                acco_enti.ID_PERS_ENTI = ID_PERS_ENTI;
                acco_enti.ID_ACCO = 4;
                acco_enti.VIG_ACCO_ENTI = true;
                acco_enti.DEF_ACCO = true;
                acco_enti.VIS_REQU = true;
                acco_enti.VIS_ASSI = false;
                acco_enti.VIS_TALE = true;
                if (ID_PERS_STAT == 1 || ID_PERS_STAT == 2) //EMPLOYEES y TEMPORAL STAFF
                {
                    acco_enti.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                }
                dbe.ACCOUNT_ENTITY.Add(acco_enti);
                dbe.SaveChanges();

                if (ID_PERS_STAT == 1 || ID_PERS_STAT == 2)
                {
                    //Registrando Banco y Cta. de Salary y CTS
                    int ID_BANK_SALA = 0, ID_BANK_CTS = 0, ID_TYPE_PENS = 0;
                    DateTime STAR_DATE_SALA, STAR_DATE_CTS, STAR_DATE_PENS, STAR_DATE_EPS;
                    if (Int32.TryParse(Convert.ToString(Request.Form["ID_BANK_SALA"]), out ID_BANK_SALA) == true)
                    {
                        var pp = new PERSON_PAYMENT();
                        pp.ID_TYPE_PAYM = 1; //SALARY
                        pp.ID_PERS_ENTI = ID_PERS_ENTI;
                        pp.ID_BANK = ID_BANK_SALA;
                        pp.NUM_ACCO = Convert.ToString(Request.Form["NUM_ACCO_SALA"].ToString());
                        if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE_SALA"]), out STAR_DATE_SALA) == true) { pp.DAT_STAR = STAR_DATE_SALA; };
                        dbe.PERSON_PAYMENT.Add(pp);
                        dbe.SaveChanges();
                    }
                    if (Int32.TryParse(Convert.ToString(Request.Form["ID_BANK_CTS"]), out ID_BANK_CTS) == true)
                    {
                        var pp = new PERSON_PAYMENT();
                        pp.ID_TYPE_PAYM = 2; //CTS
                        pp.ID_PERS_ENTI = ID_PERS_ENTI;
                        pp.ID_BANK = ID_BANK_CTS;
                        pp.NUM_ACCO = Convert.ToString(Request.Form["NUM_ACCO_CTS"].ToString());
                        if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE_CTS"]), out STAR_DATE_CTS) == true) { pp.DAT_STAR = STAR_DATE_CTS; };
                        dbe.PERSON_PAYMENT.Add(pp);
                        dbe.SaveChanges();
                    }
                    //Registrando Pension: AFP o ONP
                    if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_PENS"]), out ID_TYPE_PENS) == true)
                    {
                        var p = new PENSION();
                        p.ID_PERS_ENTI = ID_PERS_ENTI;
                        p.ID_TYPE_PENS = ID_TYPE_PENS;
                        p.CUSPP = Convert.ToString(Request.Form["CUSPP"].ToString()).ToUpper();
                        if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE_PENS"]), out STAR_DATE_PENS) == true) { p.START_DATE = STAR_DATE_PENS; };
                        dbe.PENSIONs.Add(p);
                        dbe.SaveChanges();
                    }
                    //Registrando EPS
                    if (pers_enti.EPS == true)
                    {
                        var eps = new EPSALUD();

                        int ID_PLAN_EPS, NUM_FAMI = 0;
                        eps.ID_PERS_ENTI = ID_PERS_ENTI;
                        if (Int32.TryParse(Convert.ToString(Request.Form["ID_PLAN_EPS"]), out ID_PLAN_EPS) == true) { eps.ID_PLAN_EPS = ID_PLAN_EPS; }
                        if (Int32.TryParse(Convert.ToString(Request.Form["NUM_FAMI"]), out NUM_FAMI) == true) { eps.NUMBER_FAMI = NUM_FAMI; }
                        if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE_EPS"]), out STAR_DATE_EPS) == true) { eps.STAR_DATE = STAR_DATE_EPS; };
                        eps.LAST_EPS = true;
                        dbe.EPSALUDs.Add(eps);
                        dbe.SaveChanges();
                    }

                    //Registrando Location
                    var pl = new PERSON_LOCATION();
                    pl.ID_LOCA = ID_LOCA;
                    pl.ID_PERS_ENTI = ID_PERS_ENTI;
                    pl.STAR_DATE = DateTime.Now;
                    pl.VIG_LOCA = true;
                    dbe.PERSON_LOCATION.Add(pl);
                    dbe.SaveChanges();
                    int ID_PERS_LOCA = Convert.ToInt32(pl.ID_PERS_LOCA);

                    //Registrando nuevo periodo laboral
                    var wp = new WORK_PERIOD();
                    wp.STAR_DATE = cont.STAR_DATE;
                    dbe.WORK_PERIOD.Add(wp);
                    dbe.SaveChanges();
                    int ID_WORK_PERI = Convert.ToInt32(wp.ID_WORK_PERI);

                    //Registrando el contrato
                    cont.ID_PERS_ENTI = ID_PERS_ENTI;
                    cont.ID_WORK_PERI = ID_WORK_PERI;
                    cont.VIG_CONT = true;
                    cont.LAS_CONT = true;
                    cont.ID_PERS_LOCA = ID_PERS_LOCA;
                    cont.ID_CHAR = ID_CHAR;
                    cont.ID_PERS_STAT = ID_PERS_STAT;
                    dbe.PERSON_CONTRACT.Add(cont);
                    dbe.SaveChanges();

                    //Registrado PERSON_CONTRACT_CHART
                    int ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                    var pcch = new PERSON_CONTRACT_CHART();
                    pcch.ID_PERS_CONT = ID_PERS_CONT;
                    pcch.ID_CHAR = ID_CHAR;
                    pcch.IS_CONTRACT = true;
                    pcch.VIG_CONT_CHAR = true;
                    dbe.PERSON_CONTRACT_CHART.Add(pcch);
                    dbe.SaveChanges();

                    //enviando correo
                    //SendEmailContract(ID_PERS_ENTI, 1);


                    //Guardando archivo adjunto del contrato

                    if (filesDocs != null)
                    {
                        foreach (var file in filesDocs)
                        {
                            var ATTA = new PERSON_DOCUMENTS();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                            ATTA.CREATE_ATTA = DateTime.Now;
                            ATTA.ID_TYPE_DOCU = 2;
                            ATTA.ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                            ATTA.UserId = Convert.ToInt32(Session["UserId"]);

                            dbe.PERSON_DOCUMENTS.Add(ATTA);
                            dbe.SaveChanges();
                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Documents/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_PERS_DOCU) + ATTA.EXT_ATTA));
                        }
                    }
                }
                #region Creación de Usuario ITMS
                string usernameITMS = string.Empty;
                string passwordITMS = string.Empty;

                usernameITMS = QuitAccents(S_FIR_NAME.Trim().Substring(0, 1).ToLower() + S_LAS_NAME.Trim().ToLower());
                //passwordITMS = ConfigurationManager.AppSettings["clave"].ToString() + DateTime.Now.Year;
                gth objectGth = new gth();
                passwordITMS = objectGth.GenerarContrasenia(); //ConfigurationManager.AppSettings["clave"].ToString() + cl_enti.NUM_TYPE_DI.ToString();

                var query = dbe.RH_VALIDATE_ACCOUNT_ITMS(usernameITMS).ToList();
                if (query.Count() == 0)
                {
                    //Creación en tabla UserProfile
                    WebSecurity.CreateUserAndAccount(usernameITMS, passwordITMS);
                    //WebSecurity.Login(usernameITMS, passwordITMS);
                }
                else
                {
                    if (S_MOT_NAME != null || S_MOT_NAME == "")
                    {
                        usernameITMS = usernameITMS + S_MOT_NAME.Trim().Substring(0, 1).ToLower();
                        var user = dbe.Usuarios.Where(x => x.Usuario1 == usernameITMS).ToList();
                        if (user.Count() == 0)
                        {
                            WebSecurity.CreateUserAndAccount(usernameITMS.ToLower(), passwordITMS);
                        }
                        else
                        {
                            usernameITMS = S_FIR_NAME.Trim().ToLower() + "." + S_LAS_NAME.Trim().ToLower();
                            WebSecurity.CreateUserAndAccount(usernameITMS.ToLower(), passwordITMS);
                        }
                        //WebSecurity.Login(usernameITMS, passwordITMS);
                    }
                    else
                    {
                        usernameITMS = S_FIR_NAME.Trim().ToLower() + "." + S_LAS_NAME.Trim().ToLower();
                        WebSecurity.CreateUserAndAccount(usernameITMS.ToLower(), passwordITMS);
                        //WebSecurity.Login(usernameITMS, passwordITMS);
                    }
                }

                #region Actualizacion de  idUser de la tabla UserProfile a la tabla CLASS_ENTITY
                var cargarUP = dbe.RH_VALIDATE_USERPROFILE(usernameITMS).First();
                UserId = cargarUP.UserId;
                cl_enti.UserId = UserId;
                dbe.CLASS_ENTITY.Attach(cl_enti);
                dbe.Entry(cl_enti).State = EntityState.Modified;
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

                //Registrando en PERSON_STATUS_CHANGE
                var psc = new PERSON_STATUS_CHANGE();
                psc.ID_PERS_ENTI = ID_PERS_ENTI;
                psc.ID_PERS_STAT = ID_PERS_STAT;
                psc.ID_PERS_CONT = Convert.ToInt32(cont.ID_PERS_CONT);
                psc.STAR_DATE = DateTime.Now;
                psc.UserId = Convert.ToInt32(Session["UserId"]);
                dbe.PERSON_STATUS_CHANGE.Add(psc);
                dbe.SaveChanges();

                string LugarTrabajo = "", Cargo = "";
                try
                {
                    Cargo = (from c in dbe.CHARTs.Where(x => x.ID_CHAR == ID_CHAR)
                             join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                             select new
                             {
                                 nc.NAM_CHAR
                             }).SingleOrDefault().NAM_CHAR;

                    LugarTrabajo = (from l in dbc.LOCATIONs.Where(x => x.ID_LOCA == ID_LOCA)
                                    join s in dbc.SITEs on l.ID_SITE equals s.ID_SITE
                                    select new
                                    {
                                        Lugar = s.NAM_SITE + " " + l.NAM_LOCA
                                    }).SingleOrDefault().Lugar;
                }
                catch { }

                // New Account Entity
                string S_REQU = string.Empty;
                string S_ASSI = string.Empty;
                string S_QUEU = string.Empty;
                S_REQU = Convert.ToString(Request.Params["REQU"]);
                S_ASSI = Convert.ToString(Request.Params["ASSI"]);
                //if (String.IsNullOrEmpty(S_TALE)) S_TALE = "false";
                S_QUEU = Convert.ToString(Request.Params["ID_QUEU"]);
                UpdateQueueExcel(pers_enti.ID_PERS_ENTI.ToString(), S_QUEU);
                SaveAccountEntityExcel(S_REQU, S_ASSI, acco_enti.ID_ACCO_ENTI.ToString());

                #region Enviar Correo de Confirmacion de Creacion de Usuario ITMS
                SendMail mail = new SendMail();
                mail.CreacionUsuarioEData(ConfigurationManager.AppSettings["Mailrrhh"].ToString(), S_FIR_NAME.Trim() + " " + S_LAS_NAME.Trim() + " " + S_MOT_NAME, usernameITMS, passwordITMS, S_EMA_ENTI.Trim(), Cargo, LugarTrabajo);
                #endregion

                return Json(new { success = true, message = "Se ha creado correctamente el usuario." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ha ocurrido un error, contacte al administrador" });
            }
        }

        public void SaveAccountEntityExcel(string vr, string va, string idacco)
        {
            if (!String.IsNullOrEmpty(idacco))
            {
                int ID_ACCO_ENTI = Convert.ToInt32(idacco);

                ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);

                if (!String.IsNullOrEmpty(vr))
                {
                    bool visreqe = Convert.ToBoolean(vr);
                    ae.VIS_REQU = visreqe;
                }

                if (!String.IsNullOrEmpty(va))
                {
                    bool visass = Convert.ToBoolean(va);
                    ae.VIS_ASSI = visass;
                }

                dbe.ACCOUNT_ENTITY.Attach(ae);
                dbe.Entry(ae).State = EntityState.Modified;
                dbe.SaveChanges();
            }
        }

        public void UpdateQueueExcel(string PERS_ENTI, string QUEU)
        {
            if (!String.IsNullOrEmpty(QUEU) && !String.IsNullOrEmpty(PERS_ENTI))
            {
                int ID_QUEU = Convert.ToInt32(QUEU);
                int ID_PERS_ENTI = Convert.ToInt32(PERS_ENTI);
                PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI);

                pe.ID_QUEU = ID_QUEU;
                dbe.PERSON_ENTITY.Attach(pe);
                dbe.Entry(pe).State = EntityState.Modified;
                dbe.SaveChanges();
            }
        }

        public ActionResult DeshabilitarUsuario(int idPersenti = 0)
        {
            try
            {

                var query = dbe.DeshabilitacionUsuarios(idPersenti).Single();
                return Json(new { success = true, message = query.Mensaje, id = query.Id }); ;

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ha ocurrido un error, contacte al administrador" });
            }

        }

        public ActionResult ListarUniversidades()
        {
            var result = dbe.Universidad.Where(x => x.Estado == true)
                            .Select(x => new { x.Id, x.Nombre })
                            .ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearUniversidad(int index = 0, int cantidad = 0)
        {
            ViewBag.Cantidad = cantidad;
            ViewBag.Index = index;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearUniversidad(Universidad universidad)
        {
            int id;
            string nombre = Convert.ToString(Request.Form["Nombre"]).Trim();
            if (nombre.Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneUniversidad) top.uploadDoneUniversidad('ERROR','1');}window.onload=init;</script>");
            }

            int userId = Convert.ToInt32(Session["UserId"].ToString());
            int ctd = dbe.Universidad.Where(x => x.Nombre.ToUpper() == nombre.ToUpper()).Count();
            if (ctd > 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneUniversidad) top.uploadDoneUniversidad('ERROR','3');}window.onload=init;</script>");
            }

            try
            {
                universidad.Nombre = universidad.Nombre.Trim();
                universidad.Estado = true;
                universidad.UsuarioCrea = userId;
                universidad.FechaCrea = DateTime.Now;
                dbe.Universidad.Add(universidad);
                dbe.SaveChanges();
                id = Convert.ToInt32(universidad.Id);

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneUniversidad) top.uploadDoneUniversidad('OK','" + id.ToString() + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneUniversidad) top.uploadDoneUniversidad('ERROR','2');}window.onload=init;</script>");
            }

        }

    }

    public struct Account_Entity_Excel
    {
        public int IDACCO;
        public bool VISREQU;
        public bool VISASSI;
        public bool VISTALE;
        public Nullable<int> IDQUEU;
    }

    public struct Datos_Excel
    {
        public string BIRTHDAY;
        public string STAR_DATE;
        public string END_DATE;
        public string ID_CHAR;
        public string NAM_CHAR;
    }

    #endregion


}
