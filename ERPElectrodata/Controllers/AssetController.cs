using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Net;
using System.Threading;
using System.Globalization;
using ERPElectrodata.Object.Ticket;
using OfficeOpenXml;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Data.OleDb;
using System.Linq.Expressions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using app = ERPElectrodata.AppCode;
//using LinqToExcel;


using System.Configuration;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

//using ExcelImport.Models;

namespace ERPElectrodata.Controllers
{
    public class AssetController : Controller
    {
        OleDbConnection Econ;
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString);
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        public int ctd = 1;
        public TicketIR tir = new TicketIR();
        public int IdTipoActivo = 0;
        public int IdComponente = 0;
        public int IdSubTipoComponente = 0;
        public static int IdTipoActivoPro = 0;
        public static int IdPrograma = 0;
        public static int IdEstado = 0;
        public static int IdCondicion = 0;
        static int ID_PERS_ENTI = 0;
        static int Id_Enti = 0;
        static int ID_AREA = 0;
        static int IdLocacion = 0;

        #region "Vistas"

        [Authorize]
        public ActionResult Index()
        {
            try
            {
                Session["MAIN"] = "mp2";

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                
                int surplus = 0;
                int spare = 0;
                int assigned = 0;
                int unassigned = 0;

                if (ID_ACCO == 60)
                {
                    int idGrupoActivo = ObtenerIdGrupoUsuarioBNV();

                    if (idGrupoActivo != 0)
                    {
                        string grupoActivo = ObtenerNomGrupoUsuarioBNV(idGrupoActivo);
                        
                        ViewBag.UMinera = ObtenerUMineraUsuarioBNV(Convert.ToInt32(Session["ID_PERS_ENTI"]), idGrupoActivo);
                        ViewBag.IdGrupo = idGrupoActivo;
                        ViewBag.Grupo = grupoActivo;
                    }
                    else
                    {
                        return RedirectToAction("Index", "Error");
                    }
                }
                else if (ID_ACCO == 55)
                {
                    if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 1)
                    {
                        int idGrupoIT = ObtenerIdGrupo("IT");
                        ViewBag.IdGrupoIT = idGrupoIT;
                    }
                    else if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
                    {
                        return RedirectToAction("IndexOT", "Asset");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Error");
                    }
                }
                else
                {
                    var query = dbc.ASSETs.Where(a => a.ID_ACCO == ID_ACCO)
                        .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (a, ca) => new { ca.DAT_END, a.ID_ASSE, ca.ID_COND })
                        .Where(x => x.DAT_END == null)
                        .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new { c.ID_STAT_ASSE, x.ID_ASSE });

                    surplus = query.Where(x => x.ID_STAT_ASSE == 3).Count();
                    spare = query.Where(x => x.ID_STAT_ASSE == 2).Count();
                    assigned = query.Where(x => x.ID_STAT_ASSE == 1).Count();
                    unassigned = query.Where(x => x.ID_STAT_ASSE == 4).Count();
                }

                ViewBag.Surplus = surplus;
                ViewBag.Spare = spare;
                ViewBag.Assigned = assigned;
                ViewBag.Unassigned = unassigned;

                return View();
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

        //
        // GET: /Asset/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            ASSET asset = dbc.ASSETs.Single(a => a.ID_ASSE == id);

            var clie_asset = new CLIENT_ASSET();
            clie_asset.ID_ASSE = id;
            //if (asset == null)
            //{
            //    return HttpNotFound();
            //}
            ViewBag.ID_ASSE = id;
            ViewBag.ID_TYPE_ASSE = asset.ID_TYPE_ASSE;
            return View(clie_asset);
        }

        public ActionResult CargaMasivaArchivos()
        {
            int cuenta = Convert.ToInt32(Session["ID_ACCO"]);

            if ((cuenta == 56 || cuenta == 57 || cuenta == 58) && Convert.ToInt32(Session["GESTOR_ACTIVOS_MINSUR_MARCOBRE_RAURA"]) == 1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }
        //[HttpPost]
        public ActionResult CargaMasiva(/*HttpPostedFileBase file*/)
        {
            //string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            //string filepath = "/folder/" + filename;
            //file.SaveAs(Path.Combine(Server.MapPath("folder"), filename));
            //InsertExceldata(filepath, filename);
            return View();
        }

        [Authorize]
        public ActionResult Detalle(int id = 0)
        {
            ASSET Activo = dbc.ASSETs.Single(a => a.ID_ASSE == id);

            var clie_asset = new CLIENT_ASSET();
            clie_asset.ID_ASSE = id;

            ViewBag.ID_ASSE = id;
            ViewBag.ID_TYPE_ASSE = Activo.ID_TYPE_ASSE;

            int idGrupo = 0;
            string grupo = "";
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (idAcco == 60)
            {
                int idGrupoActivo = ObtenerIdGrupoUsuarioBNV();

                if (idGrupoActivo != 0)
                {
                    var uMinera = dbe.CLASS_ENTITY.FirstOrDefault(x => x.ID_ENTI == Activo.UMinera).COM_NAME;
                    grupo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(Activo.IdGrupo));

                    ViewBag.UMinera = uMinera.ToUpper();
                    idGrupo = Convert.ToInt32(Activo.IdGrupo);
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            else if (idAcco == 55)
            {
                grupo = ObtenerNombreGrupo(Convert.ToInt32(Activo.IdGrupo));

                if (grupo == "IT" && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 0)
                    return RedirectToAction("Index", "Error");

                if (grupo == "OT" && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 0)
                    return RedirectToAction("Index", "Error");

                idGrupo = Convert.ToInt32(Activo.IdGrupo);
            }

            int permiso = Convert.ToInt32(Session["PERMISO_LICENCIAS"]);
            ViewBag.Permiso = permiso;
            ViewBag.IdGrupo = idGrupo;
            ViewBag.Grupo = grupo;
            return View(clie_asset);
        }

        #endregion

        #region "Creacion Activo"



        #endregion

        #region "Bandeja Activos"

        #endregion

        #region "Detalle Activos Vista"

        public ActionResult Editar(int id = 0)
        {
            ASSET objActivo = dbc.ASSETs.Single(a => a.ID_ASSE == id);         
            ViewBag.FechaFinContrato = objActivo.FechaFinContrato;
            ViewBag.FechaAdquisicion = objActivo.ACQ_DATE;
            ViewBag.IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            if (Convert.ToInt32(Session["ID_ACCO"]) == 1)
            {
                CLIENT_ASSET clientA = dbc.CLIENT_ASSET.Single(c => c.ID_ASSE == id && c.DAT_END == null);
                CONDITION co = dbc.CONDITIONs.Single(c => c.ID_COND == clientA.ID_COND);
                STATUS_ASSET sa = dbc.STATUS_ASSET.Single(st => st.ID_STAT_ASSE == co.ID_STAT_ASSE);
                ViewBag.Estado = sa.ID_STAT_ASSE;
                bool PorContratista;
                if (clientA.PorContratista == null || clientA.PorContratista == false)
                {
                    PorContratista = false;
                    ViewBag.DatosContratista = "";
                }
                else
                {
                    PorContratista = true;
                    ViewBag.DatosContratista = clientA.UsuarioContratista + ',' + clientA.EmpresaContratista + ',' + clientA.AreaContratista;
                }
                ViewBag.PorContratista = PorContratista;
            }
            if (objActivo == null)
            {
                return HttpNotFound();
            }
            return View(objActivo);
        }


        public ActionResult EditarHudbay(int id = 0)
        {
            ASSET objActivo = dbc.ASSETs.Single(a => a.ID_ASSE == id);
            if (objActivo == null)
            {
                return HttpNotFound();
            }

            string grupo = ObtenerNombreGrupo(Convert.ToInt32(objActivo.IdGrupo));

            if (grupo == "OT")
                return View("EditarHudbayOT", objActivo);
            else
                return View(objActivo);
        }

        public ActionResult Actividades(int id = 0)
        {
            ViewBag.ID_ASSE = id;
            return View();
        }

        public ActionResult Formatos(int id = 0)
        {
            ViewBag.ID_ASSE = id;
            return View();
        }

        public ActionResult LicenciaIndex()
        {
            return View();
        }

        public ActionResult Componentes(int id = 0)
        {
            int idTipoActivo = 0;
            int idAcco = 0;
            int idCuentaTipoActivo = 0;
            ViewBag.ID_ASSE = id;
            var objActivo = dbc.ASSETs.Where(AS => AS.ID_ASSE == id).SingleOrDefault();
            idTipoActivo = Convert.ToInt32(objActivo.ID_TYPE_ASSE);
            idAcco = Convert.ToInt32(objActivo.ID_ACCO);
            var objCuentaTipoActivo = dbc.ACCOUNT_TYPE_ASSET.Where(AT => AT.ID_TYPE_ASSE == idTipoActivo && AT.ID_ACCO == idAcco).SingleOrDefault();
            idCuentaTipoActivo = Convert.ToInt32(objCuentaTipoActivo.ID);
            ViewBag.IdCuentaTipoActivo = idCuentaTipoActivo;
            string crea = Convert.ToString(Request.Params["crea"]);
            ViewBag.Crea = crea;
            return View();
        }

        public ActionResult Condicion(int id = 0, int idGrupo = 0)
        {
            ViewBag.ID_ASSE = id;
            var objClientAsset = dbc.CLIENT_ASSET.Where(x => x.ID_ASSE == id && x.DAT_END == null)
            .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new { x.ID_CLIE_ASSE, c.ID_STAT_ASSE, x.ID_COND, x.ID_PERS_ENTI }).SingleOrDefault();

            ViewBag.IdCondicion = objClientAsset.ID_COND;
            ViewBag.IdEstadoActivo = objClientAsset.ID_STAT_ASSE;
            ViewBag.IdClienteActivo = objClientAsset.ID_CLIE_ASSE;
            ViewBag.IdGrupo = idGrupo;
            ViewBag.IdAsignado = objClientAsset.ID_PERS_ENTI;
            return View();
        }

        public ActionResult RecepcionCondicion(int id = 0)
        {
            int IdCompStockDet = 0;
            var objActivoComponente = dbc.ActivoComponentes.Where(AC => AC.IdActivoComponente == id).SingleOrDefault();
            IdCompStockDet = Convert.ToInt32(objActivoComponente.IdCompStockDet);
            ViewBag.IdCompStockDet = IdCompStockDet;
            ViewBag.IdActivoComponente = id;
            //var objComponenteStockDetalle = dbc.ComponenteStockDetalles.Where(CO => CO.IdCompStockDet == IdCompStockDet).SingleOrDefault();
            //IdCondicion = Convert.ToInt32(objComponenteStockDetalle.IdCompStockDet); 
            //ViewBag.IdCondicion = IdCondicion;
            return View();
        }

        public ActionResult ActualizarCondicionComponente()
        {
            int idComponenteStockDet = Convert.ToInt32(Request.Params["id"].ToString());
            int idCondicion = Convert.ToInt32(Request.Params["cond"].ToString());
            int idActivoComponente = Convert.ToInt32(Request.Params["idActivoComponente"].ToString());
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            int idComponenteStockCab = 0;

            ComponenteStockDetalle objComponenteStockDet = dbc.ComponenteStockDetalles.Where(x => x.IdCompStockDet == idComponenteStockDet).SingleOrDefault();
            ActivoComponente objActivoComponente = dbc.ActivoComponentes.Where(AC => AC.IdActivoComponente == idActivoComponente).SingleOrDefault();
            var objComponenteStockDetalle = dbc.ComponenteStockDetalles.Where(CO => CO.IdCompStockDet == idComponenteStockDet).SingleOrDefault();
            idComponenteStockCab = Convert.ToInt32(objComponenteStockDetalle.IdCompStockCab);
            ComponenteStockCabecera objComponenteStockCab = dbc.ComponenteStockCabecera.Where(CAB => CAB.IdCompStockCab == idComponenteStockCab).SingleOrDefault();
            try
            {
                objComponenteStockDet.FechaModifica = DateTime.Now;
                objComponenteStockDet.UsuarioModifica = UserId;
                objComponenteStockDet.IdCond = idCondicion;
                objComponenteStockDet.Activo = false;
                dbc.ComponenteStockDetalles.Attach(objComponenteStockDet);
                dbc.Entry(objComponenteStockDet).State = EntityState.Modified;
                dbc.SaveChanges();

                objActivoComponente.Estado = false;
                dbc.ActivoComponentes.Attach(objActivoComponente);
                dbc.Entry(objActivoComponente).State = EntityState.Modified;
                dbc.SaveChanges();

                objComponenteStockCab.CantidadEnUso = objComponenteStockCab.CantidadEnUso - 1;
                objComponenteStockCab.CantidadDisponible = objComponenteStockCab.CantidadDisponible + 1;
                dbc.ComponenteStockCabecera.Attach(objComponenteStockCab);
                dbc.Entry(objComponenteStockCab).State = EntityState.Modified;
                dbc.SaveChanges();

            }
            catch
            {
                return Content("Error");
            }

            return Content("Ok");
        }

        public ActionResult HistorialActivos(int id = 0)
        {
            ViewBag.IdActivo = id;
            return View();
        }
        public ActionResult ListarCondicionComponentesRecepcion()
        {
            var query = dbc.ListarCondicionComponentesRecepcion().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Detalle Activos Procedimientos"

        public ActionResult ObtenerDatosActivo(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ActObtenerDetalle2(id, ID_ACCO).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarCondicion()
        {
            int idClienteActivo = Convert.ToInt32(Request.Params["id"].ToString());
            int idCondicion = Convert.ToInt32(Request.Params["cond"].ToString());
            string Comentario = Request.Params["txt"].ToString();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            CLIENT_ASSET objClientAsset = dbc.CLIENT_ASSET.Where(x => x.ID_CLIE_ASSE == idClienteActivo).SingleOrDefault();

            int condicionAnterior = (int)objClientAsset.ID_COND;
            try
            {
                objClientAsset.DAT_END = DateTime.Now;

                dbc.CLIENT_ASSET.Attach(objClientAsset);
                dbc.Entry(objClientAsset).State = EntityState.Modified;
                dbc.SaveChanges();

                objClientAsset.ID_COND = idCondicion;
                objClientAsset.DAT_STAR = DateTime.Now;
                objClientAsset.DAT_END = null;
                objClientAsset.ID_TYPE_CLIE_ASSE = 3;
                objClientAsset.SUM_CLIE_ASSE = Comentario;
                objClientAsset.CREATE_DATE = DateTime.Now;
                objClientAsset.UserId = UserId;

                dbc.CLIENT_ASSET.Add(objClientAsset);
                dbc.SaveChanges();
            }
            catch
            {
                return Content("Error");
            }

            return Content("Ok");
        }

        #endregion

        //
        // GET: /Asset/ListByIdPersEnty
        public ActionResult ListByIdPersEnty(int id = 0)
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.CLIENT_ASSET.Where(ca => ca.ID_PERS_ENTI == id && ca.DAT_END == null)
                .Join(dbc.ASSETs, ca => ca.ID_ASSE, a => a.ID_ASSE, (ca, a) => new
                {
                    ca.CREATE_DATE,
                    ca.ID_ASSE,
                    a.ID_ACCO,
                    a.ID_TYPE_ASSE,
                    a.COD_ASSE,
                    a.ID_MANU,
                    ca.ID_COND
                })
                .Where(x => x.ID_ACCO == ID_ACCO)
                .OrderBy(x => x.CREATE_DATE);

            var result = (from x in query.ToList().Skip(skip).Take(take)
                          join ta in dbc.TYPE_ASSET on x.ID_TYPE_ASSE equals ta.ID_TYPE_ASSE
                          join m in dbc.MANUFACTURERs on x.ID_MANU equals m.ID_MANU
                          join c in dbc.CONDITIONs on x.ID_COND equals c.ID_COND
                          join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                          select new
                          {
                              COD_ASSE = (x.COD_ASSE == null ? "Without Code" : x.COD_ASSE),
                              NAM_TYPE_ASSE = ta.NAM_TYPE_ASSE.ToLower(),
                              NAM_MANU = m.NAM_MANU.ToLower(),
                              NAM_COND = c.NAM_COND.ToLower(),
                              NAM_STAT_ASSE = s.NAM_STAT_ASSE.ToLower(),
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportAsset()
        {
            return View();
        }

        public ActionResult ListByText()
        {
            //int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            //int take = Convert.ToInt32(Request.Params["take"].ToString());
            //string text = Request.Params["filter[filters][0][value]"].ToString();
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = Convert.ToString(Request.Params["txt"]);
            if (txt == null) { txt = ""; }

            var query1 = dbc.ASSETs.Where(x => x.ID_ACCO == ID_ACCO)
                .Join(dbc.CLIENT_ASSET, x => x.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    ca.DAT_END,
                    ca.ID_COND
                })
                .Where(x => x.DAT_END == null)
                .Join(dbc.TYPE_ASSET, x => x.ID_TYPE_ASSE, ta => ta.ID_TYPE_ASSE, (x, ta) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    ta.NAM_TYPE_ASSE
                })
                .Join(dbc.MANUFACTURERs, x => x.ID_MANU, m => m.ID_MANU, (x, m) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.NAM_TYPE_ASSE,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    m.NAM_MANU
                })
                .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.NAM_TYPE_ASSE,
                    x.NAM_MANU,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    c.ID_STAT_ASSE,
                    c.NAM_COND
                })
                .Join(dbc.STATUS_ASSET, x => x.ID_STAT_ASSE, sa => sa.ID_STAT_ASSE, (x, sa) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.NAM_TYPE_ASSE,
                    x.NAM_MANU,
                    x.ID_STAT_ASSE,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    x.NAM_COND,
                    sa.NAM_STAT_ASSE
                })

                .Where(x => (x.ID_STAT_ASSE != 1 && x.ID_STAT_ASSE != 3) && //ID_COND = 9: UNASSIGNED-OPERATIVE,  ID_COND = 3: SPARE-AVAILABLE, 
                    (x.NAM_TYPE_ASSE.ToUpper().Contains(txt.ToUpper()) || x.COD_ASSE.ToUpper().Contains(txt.ToUpper()) || x.SER_NUMB.ToUpper().Contains(txt.ToUpper()) ||
                    x.NAM_MANU.ToUpper().Contains(txt.ToUpper()) || /*x.NAM_COMM_MODE.Contains(txt) ||*/ (x.NAM_STAT_ASSE + " - " + x.NAM_COND).ToUpper().Contains(txt.ToUpper())));

            ///===================
            ///
            var awof = dbc.ASSETs.Where(x => x.ID_ACCO == ID_ACCO)
                .Join(dbc.DETAIL_TICKET_FORMAT, a => a.ID_ASSE, dtf => dtf.ID_ASSE, (a, dtf) => new
                {
                    ID_ASSE = a.ID_ASSE,
                    dtf.ID_DETA_TICK_DELI_RECE
                }).GroupBy(x => x.ID_ASSE, p => p.ID_DETA_TICK_DELI_RECE, (key, g) => new { ID_ASSE = key, Cant = g.Count() });

            int cant = awof.Count();

            var res2 = (from x in dbc.ASSETs.Where(x => x.ID_ACCO == ID_ACCO).ToList()
                        join ca in dbc.CLIENT_ASSET.Where(x => x.DAT_END == null) on x.ID_ASSE equals ca.ID_ASSE
                        join ta in dbc.TYPE_ASSET on x.ID_TYPE_ASSE equals ta.ID_TYPE_ASSE
                        join m in dbc.MANUFACTURERs on x.ID_MANU equals m.ID_MANU
                        join c in dbc.CONDITIONs on ca.ID_COND equals c.ID_COND
                        join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                        //join cm in db.COMMERCIAL_MODEL on x.ID_COMM_MODE equals cm.ID_COMM_MODE
                        join y in awof on x.ID_ASSE equals y.ID_ASSE into ly
                        from xly in ly.DefaultIfEmpty()
                        select new
                        {
                            ID_ASSE = x.ID_ASSE,
                            ID_ASSE_TEMP = (xly == null ? 0 : xly.ID_ASSE),
                            COD_ASSE = (x.COD_ASSE == null ? "-" : x.COD_ASSE),
                            SER_NUMB = (x.SER_NUMB == null ? "-" : x.SER_NUMB),
                            ID_TYPE_ASSE = (int)x.ID_TYPE_ASSE,
                            ID_MANU = (int)x.ID_MANU,
                            ID_COND = (int)ca.ID_COND,
                            NAM_TYPE_ASSE = ta.NAM_TYPE_ASSE,
                            NAM_MANU = (m.NAM_MANU == null ? "-" : m.NAM_MANU),
                            NAM_COMM_MODE = "",//(cm.NAM_COMM_MODE == null ? "-" : cm.NAM_COMM_MODE),
                            COND = s.NAM_STAT_ASSE + " - " + c.NAM_COND,
                            NAM_STAT_ASSE = s.NAM_STAT_ASSE,
                            NAM_EQUI = (x.NAM_EQUI == null ? "-" : x.NAM_EQUI),
                            NAM_ASSE = ta.NAM_TYPE_ASSE + " : " + x.COD_ASSE,

                        }).Where(x => x.ID_ASSE_TEMP == 0)
                        .Where(x => (x.NAM_TYPE_ASSE.ToUpper().Contains(txt.ToUpper()) || x.COD_ASSE.ToUpper().Contains(txt.ToUpper()) || x.SER_NUMB.ToUpper().Contains(txt.ToUpper()) ||
                    x.NAM_MANU.ToUpper().Contains(txt.ToUpper()) || /*x.NAM_COMM_MODE.Contains(txt) ||*/ (x.COND.ToUpper()).Contains(txt.ToUpper())));

            //var epd = res2.OrderBy(x=>x.COD_ASSE).ToList();
            //var listr = res2.Where(x => x.COD_ASSE.ToUpper().Contains(txt.ToUpper())).ToList();
            //int res2c = res2.Count();
            //////===============


            var result = (from x in query1.OrderBy(x => x.NAM_TYPE_ASSE).ToList()
                          select new
                          {
                              ID_ASSE = x.ID_ASSE,
                              ID_ASSE_TEMP = 0,
                              COD_ASSE = (x.COD_ASSE == null ? "-" : x.COD_ASSE),
                              SER_NUMB = (x.SER_NUMB == null ? "-" : x.SER_NUMB),
                              ID_TYPE_ASSE = (int)x.ID_TYPE_ASSE,
                              ID_MANU = (int)x.ID_MANU,
                              ID_COND = (int)x.ID_COND,
                              NAM_TYPE_ASSE = x.NAM_TYPE_ASSE,
                              NAM_MANU = (x.NAM_MANU == null ? "-" : x.NAM_MANU),
                              NAM_COMM_MODE = "",//(x.NAM_COMM_MODE == null ? "-" : x.NAM_COMM_MODE),
                              COND = x.NAM_STAT_ASSE + " - " + x.NAM_COND,
                              NAM_STAT_ASSE = x.NAM_STAT_ASSE,
                              NAM_EQUI = (x.NAM_EQUI == null ? "-" : x.NAM_EQUI),
                              NAM_ASSE = x.NAM_TYPE_ASSE + " : " + x.COD_ASSE,
                          });

            //int resultc = result.Count();

            var all = result.Concat(res2).Distinct();

            return Json(new { Data = all, Count = query1.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAllAssetByAcco()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            if (txt == null) { txt = ""; }

            var query1 = dbc.ASSETs.Where(x => x.ID_ACCO == ID_ACCO)
                .Join(dbc.TYPE_ASSET, x => x.ID_TYPE_ASSE, ta => ta.ID_TYPE_ASSE, (x, ta) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    ta.NAM_TYPE_ASSE,
                    x.IdGrupo
                });

            if (ID_ACCO == 55)
            {
                int idGrupoIT = ObtenerIdGrupo("IT");
                query1 = query1.Where(x => x.IdGrupo == idGrupoIT);
            }

            if (txt != "")
            {
                query1 = query1.Where(x => (x.NAM_TYPE_ASSE.Contains(txt) || x.COD_ASSE.Contains(txt) || x.SER_NUMB.Contains(txt)));
                var result = (from x in query1.OrderBy(x => x.NAM_TYPE_ASSE).ToList()
                              select new
                              {
                                  x.ID_ASSE,
                                  COD_ASSE = (x.COD_ASSE == null ? "-" : x.COD_ASSE),
                                  SER_NUMB = (x.SER_NUMB == null ? "-" : x.SER_NUMB),
                                  x.NAM_TYPE_ASSE,
                                  NAM_ASSE = x.NAM_TYPE_ASSE + " : " + x.COD_ASSE,
                              });

                return Json(new { Data = result, Count = query1.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from x in query1.OrderBy(x => x.NAM_TYPE_ASSE).Skip(0).Take(99).ToList()
                              select new
                              {
                                  x.ID_ASSE,
                                  COD_ASSE = (x.COD_ASSE == null ? "-" : x.COD_ASSE),
                                  SER_NUMB = (x.SER_NUMB == null ? "-" : x.SER_NUMB),
                                  x.NAM_TYPE_ASSE,
                                  NAM_ASSE = x.NAM_TYPE_ASSE + " : " + x.COD_ASSE,
                              });

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }

        }

        public List<object> ListarActivos(int IdTipoActivo, int IdMarca, int IdUsuario, string PalabraClave, int ID_STAT_ASSE, int skip, int take, ref int total)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query1 = dbc.ASSETs.Where(a => a.ID_ACCO == ID_ACCO)
                .Join(dbc.TYPE_ASSET, x => x.ID_TYPE_ASSE, ta => ta.ID_TYPE_ASSE, (x, ta) => new
                {
                    x.ID_ASSE,
                    x.CREATE_ASSE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_COST_CENT,
                    x.ID_MANU_MODE,
                    x.COD_ASSE,
                    x.COST,
                    x.NAM_EQUI,
                    x.ID_BUY_MODE,
                    x.LOTE,
                    x.ACQ_DATE,
                    x.Contrato,
                    ta.NAM_TYPE_ASSE,
                    ta.COLOR,
                    ta.INDICE,
                    x.MAC_ADRR
                })
                .Join(dbc.CLIENT_ASSET, x => x.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
                {
                    x.COD_ASSE,
                    x.ACQ_DATE,
                    x.ID_ASSE,
                    x.CREATE_ASSE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_COST_CENT,
                    x.NAM_TYPE_ASSE,
                    x.COLOR,
                    x.INDICE,
                    x.ID_MANU_MODE,
                    x.COST,
                    x.ID_BUY_MODE,
                    x.NAM_EQUI,
                    x.LOTE,
                    x.MAC_ADRR,
                    x.Contrato,
                    ca.DAT_END,
                    ca.ID_COND,
                    ca.ID_PERS_ENTI,
                    ca.CREATE_DATE,
                    ca.ID_LOCA,
                    ca.UserId,
                    ca.ID_AREA,
                })
                .Where(x => x.DAT_END == null)
                .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new
                {
                    x.COD_ASSE,
                    x.ID_ASSE,
                    x.CREATE_ASSE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_COST_CENT,
                    x.NAM_TYPE_ASSE,
                    x.COLOR,
                    x.INDICE,
                    x.DAT_END,
                    x.ID_COND,
                    x.ID_PERS_ENTI,
                    x.CREATE_DATE,
                    x.ID_LOCA,
                    x.UserId,
                    x.COST,
                    x.ID_BUY_MODE,
                    x.ID_MANU_MODE,
                    x.NAM_EQUI,
                    x.LOTE,
                    x.ACQ_DATE,
                    x.Contrato,
                    c.ID_STAT_ASSE,
                    c.NAM_COND,
                    x.ID_AREA,
                    x.MAC_ADRR
                }).Where(x => x.ID_STAT_ASSE == ID_STAT_ASSE);

            if (IdTipoActivo != 0)
            {
                query1 = query1.Where(x => x.ID_TYPE_ASSE == IdTipoActivo);
            }
            if (IdMarca != 0)
            {
                query1 = query1.Where(x => x.ID_MANU == IdMarca);
            }
            if (!String.IsNullOrEmpty(PalabraClave))
            {
                query1 = query1.Where(x => x.NAM_EQUI.Contains(PalabraClave) || x.COD_ASSE.Contains(PalabraClave) || x.SER_NUMB.Contains(PalabraClave) || x.MAC_ADRR.Contains(PalabraClave));
            }

            var query2 = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_SOUR == 4)
                        .Join(dbc.TYPE_TICKET, t => t.ID_TYPE_TICK, tt => tt.ID_TYPE_TICK, (t, tt) => new
                        {
                            t.ID_TICK,
                            t.COD_TICK,
                            t.ID_TYPE_TICK,
                            t.ID_ASSE,
                            tt.NAM_TYPE_TICK
                        });

            var query3 = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO)
                         .Join(dbe.PERSON_ENTITY, x => x.ID_PERS_ENTI, pe => pe.ID_PERS_ENTI, (x, pe) => new
                         {
                             x.ID_PERS_ENTI,
                             pe.ID_ENTI2
                         })
                         .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_ENTI2,
                             ce.ID_ENTI,
                             ce.FIR_NAME,
                             ce.LAS_NAME,
                             ce.UserId
                         });

            var query5 = query3;

            if (IdUsuario != 0)
            {
                query5 = query5.Where(x => x.ID_PERS_ENTI == IdUsuario);
            }

            int Cuenta = ID_ACCO;

            if (ID_ACCO == 2)
            {
                Cuenta = 1;
            }
            var query4 = dbe.AREAs.Where(a => a.ID_ACCO == Cuenta && a.ID_AREA_PARENT != null).ToList();

            var result = (from a in query1.OrderBy(x => x.INDICE).ThenBy(x => x.NAM_TYPE_ASSE).ToList()
                          join pe in query5 on a.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join m in dbc.MANUFACTURERs on a.ID_MANU equals m.ID_MANU
                          join cm in dbc.COMMERCIAL_MODEL on a.ID_COMM_MODE equals cm.ID_COMM_MODE into lcm
                          from xcm in lcm.DefaultIfEmpty()
                          join mm in dbc.MANUFACTURER_MODEL on a.ID_MANU_MODE equals mm.ID_MANU_MODE into lmm
                          from xmm in lmm.DefaultIfEmpty()
                          join t in query2 on a.ID_ASSE equals t.ID_ASSE into lt
                          from x in lt.DefaultIfEmpty()
                          join l in dbc.LOCATIONs on a.ID_LOCA equals l.ID_LOCA
                          join s in dbc.SITEs on l.ID_SITE equals s.ID_SITE
                          join uce in query3 on a.UserId equals uce.UserId
                          join cc in dbc.COST_CENTER on a.ID_COST_CENT equals cc.ID_COST_CENT into lcc
                          from z in lcc.DefaultIfEmpty()
                          join bm in dbc.BUY_MODE on a.ID_BUY_MODE equals bm.ID_BUY_MODE into lbm
                          from xbm in lbm.DefaultIfEmpty()
                          join ar in query4 on a.ID_AREA equals ar.ID_AREA into lar
                          from xar in lar.DefaultIfEmpty()
                          select new
                          {
                              COD_ASSE = (a.COD_ASSE == null ? "-" : a.COD_ASSE),
                              a.ID_ASSE,
                              ACQ_DATE = (a.ACQ_DATE == null ? "-" : String.Format("{0:G}", a.ACQ_DATE)),
                              NAM_COND = a.NAM_COND.ToLower(),
                              USER = pe.FIR_NAME.ToLower() + ' ' + pe.LAS_NAME.ToLower(),
                              REGISTER = String.Format("{0:G}", a.CREATE_ASSE),
                              UPDATE = String.Format("{0:G}", a.CREATE_DATE),
                              NAM_TYPE_ASSE = a.NAM_TYPE_ASSE.ToLower(),
                              NAM_MANU = m.NAM_MANU,
                              NAM_COMM_MODE = (xcm == null ? String.Empty : xcm.NAM_COMM_MODE),
                              NAM_MANU_MODE = (xmm == null ? String.Empty : xmm.NAM_MANU_MODE),
                              SER_NUMB = (a.SER_NUMB == null ? "-" : a.SER_NUMB),
                              SOLPED = (a.SOLPED == null ? String.Empty : a.SOLPED),
                              COD_TICK = (x == null ? String.Empty : x.COD_TICK),
                              TYPE_TICK = (x == null ? String.Empty : x.NAM_TYPE_TICK.ToLower()),
                              NAM_LOCA = l.NAM_LOCA.ToLower(),
                              NAM_SITE = s.NAM_SITE.ToLower(),
                              a.COLOR,
                              CREATE_BY = uce.FIR_NAME.ToLower() + ' ' + uce.LAS_NAME.ToLower(),
                              CECO = (z == null ? "-" : z.COD_COST_CENT),
                              COST = (a.COST == null ? "-" : Convert.ToString(String.Format("{0:N2}", a.COST))),
                              NAM_BUY_MODE = (xbm == null ? string.Empty : xbm.NAM_BUY_MODE),
                              NAM_EQUI = (a.NAM_EQUI == null ? "-" : a.NAM_EQUI),
                              NAM_AREA = (xar == null ? "-" : xar.NOM_AREA),
                              LOT = (a.LOTE == null ? String.Empty : Convert.ToString(a.LOTE)),
                              CONTRATO = (a.Contrato == null ? "-" : a.Contrato),
                              a.ID_STAT_ASSE
                          }).Skip(skip).Take(take).OrderByDescending(x => x.CREATE_BY).ToList();

            List<object> Resultado = new List<object>(result);

            return Resultado;
        }

        public ActionResult ListByStatus(int IdTipoActivo, int IdMarca, int IdUsuario, string PalabraClave)
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int Estado = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.ActListar(IdTipoActivo.ToString(), IdMarca.ToString(), IdUsuario.ToString(), PalabraClave, Estado, IdAcco).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Grid = query2, Cantidad = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PrevisualizarDatos()
        {
            var previsualizacionDatos = new List<DatosIngresadosActivo>();

            try
            {
                HttpPostedFileBase file = Request.Files["file"];

                if (file != null && file.ContentLength > 0)
                {
                    if (!IsExcelFile(file.FileName))
                    {
                        return Json(new { success = false, message = "Por favor, subir un documento Excel.",  });
                    }

                    using (XLWorkbook workbook = new XLWorkbook(file.InputStream))
                    {
                        IXLWorksheet worksheet = workbook.Worksheet(1);

                        int lastRow = worksheet.LastRowUsed().RowNumber();
                        int batchSize = 1000;
                        

                        for (int startRow = 2; startRow <= lastRow; startRow += batchSize)
                        {
                            
                            int endRow = Math.Min(startRow + batchSize - 1, lastRow);

                            for (int row = startRow; row <= endRow; row++)
                            {

                                DatosIngresadosActivo activo = new DatosIngresadosActivo()
                                {
                                    Hostname = worksheet.Cell(row, 1).Value.ToString(),
                                    Serie = worksheet.Cell(row, 2).Value.ToString(),
                                    TipoActivo = worksheet.Cell(row, 3).Value.ToString(),
                                    SubTipoActivo = worksheet.Cell(row, 4).Value.ToString(),
                                    CodigoActivo = worksheet.Cell(row, 5).Value.ToString(),
                                    Grupo = worksheet.Cell(row, 6).Value.ToString(),
                                    Marca = worksheet.Cell(row, 7).Value.ToString(),
                                    ModeloComercial = worksheet.Cell(row, 8).Value.ToString(),
                                    ModeloFabrica = worksheet.Cell(row, 9).Value.ToString(),
                                    Arrendedor = worksheet.Cell(row, 10).Value.ToString(),
                                    Solped = worksheet.Cell(row, 11).Value.ToString(),
                                    CentroCosto = worksheet.Cell(row, 12).Value.ToString(),
                                    ModoCompra = worksheet.Cell(row, 13).Value.ToString(),
                                    Costo = double.TryParse(worksheet.Cell(row, 14).Value.ToString(), out double costValue) ? Convert.ToDecimal(costValue) : 0,
                                    FechaFinContrato = worksheet.Cell(row, 15).GetDateTime().ToString(),
                                    Sitio = worksheet.Cell(row, 16).Value.ToString(),
                                    Locacion = worksheet.Cell(row, 17).Value.ToString(),
                                    FechaAdquisicion = worksheet.Cell(row, 18).GetDateTime().ToString(),
                                    Comentarios = worksheet.Cell(row, 19).Value.ToString(),
                                    Condicion = worksheet.Cell(row, 20).Value.ToString(),
                                    Estado = worksheet.Cell(row, 21).Value.ToString(),
                                    UsuarioResponsable = worksheet.Cell(row, 22).Value.ToString(),
                                    UsuarioAsignado = worksheet.Cell(row, 23).Value.ToString(),
                                    IpLocal = worksheet.Cell(row, 24).Value.ToString(),
                                    IpPublica = worksheet.Cell(row, 25).Value.ToString(),
                                    Contrato = worksheet.Cell(row, 26).Value.ToString(),
                                    MacFisica = worksheet.Cell(row, 27).Value.ToString(),
                                    MacInalambrica = worksheet.Cell(row, 28).Value.ToString(),
                                    MacBluetooth = worksheet.Cell(row, 29).Value.ToString(),
                                };

                                DataCompleta(worksheet, row, activo);

                                previsualizacionDatos.Add(activo);
                            }
                        }

                        return Json(new { success = true, message = "Datos cargados", previsualizacionDatos });

                    }
                }

                return Json(new { success = false, message = "Error con la información", previsualizacionDatos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error con la información", previsualizacionDatos });
            }
        }

        private bool IsExcelFile(string fileName)
        {
            string[] allowedExtensions = { ".xlsx", ".xls" };
            string fileExtension = Path.GetExtension(fileName.ToLower());
            return allowedExtensions.Contains(fileExtension);
        }

        public void DataCompleta(IXLWorksheet worksheet, int row, DatosIngresadosActivo activo)
        {

            int[] ColumnaObligatoria = new int[] { 2, 3, 6, 7, 13, 17, 16, 20, 21 }; 

            foreach (var Columna in ColumnaObligatoria)
            {
                if (worksheet.Cell(row, Columna).IsEmpty())
                {
                    activo.Valido = false;
                    activo.Observacion = "Falta información en la columna " + (Columna + 2) + ".";
                    break;
                }
                else
                {
                    activo.Valido = true;
                    activo.Observacion = "";
                }
            }

            string NumeroSerie = worksheet.Cell(row, 2).Value.ToString().Trim().ToUpper();

            var validarActivo = dbc.ASSETs.Where(x => x.SER_NUMB == NumeroSerie).Count();
            
            if (validarActivo > 0)
            {
                activo.Valido = false;
                activo.Observacion = "El activo ya existe.";
            }
        }

        public string ObtenerMensajesDeFaltantes(Dictionary<int, List<string>> diccionario)
        {
            StringBuilder mensaje = new StringBuilder();

            foreach (var kvp in diccionario)
            {
                int fila = kvp.Key;
                List<string> columnasFaltantes = new List<string>();

                // Verificar si la fila tiene valores asociados
                if (kvp.Value != null && kvp.Value.Count > 0)
                {
                    mensaje.Append($"A la fila {fila} le faltan las columnas: ");
                    mensaje.Append(string.Join(", ", kvp.Value));
                    mensaje.AppendLine();
                }
                else
                {
                    mensaje.AppendLine($"La fila {fila} no tiene valores asociados en el diccionario.");
                }
            }

            return mensaje.ToString();
        }


        [HttpPost]
        public ActionResult ImportarDatosActivo()
        {
            int cuenta = (int)Session["ID_ACCO"];
            string mensaje = "";
            try
            {
                HttpPostedFileBase file = Request.Files["file"];

                if (file != null && file.ContentLength > 0)
                {
                    using (XLWorkbook workbook = new XLWorkbook(file.InputStream))
                    {
                        IXLWorksheet worksheet = workbook.Worksheet(1);

                        int lastRow = worksheet.LastRowUsed().RowNumber();
                        int batchSize = 1000;

                        for (int startRow = 2; startRow <= lastRow; startRow += batchSize)
                        {
                            int endRow = Math.Min(startRow + batchSize - 1, lastRow);

                            for (int row = startRow; row <= endRow; row++)
                            {
                                DatosIngresadosActivo ActivoTemporal = new DatosIngresadosActivo();

                                string valorTipoActivo = worksheet.Cell(row, 3).Value.ToString();
                                string valorCodigoActivo = worksheet.Cell(row, 5).Value.ToString();
                                string valorNumeroSerie = worksheet.Cell(row, 2).Value.ToString();
                                string valorMarca = worksheet.Cell(row, 7).Value.ToString();
                                string valorLocacion = worksheet.Cell(row, 17).Value.ToString();
                                string valorModoCompra = worksheet.Cell(row, 13).Value.ToString();


                                if (!string.IsNullOrEmpty(valorTipoActivo) && !string.IsNullOrEmpty(valorCodigoActivo) && !string.IsNullOrEmpty(valorNumeroSerie) && !string.IsNullOrEmpty(valorMarca) && !string.IsNullOrEmpty(valorLocacion) && !string.IsNullOrEmpty(valorModoCompra))
                                {
                                    //SUBTIPO
                                    string namTypeAsse = worksheet.Cell(row, 3).Value.ToString();
                                    var tipoActivo = dbc.TYPE_ASSET.FirstOrDefault(x => x.NAM_TYPE_ASSE == namTypeAsse)?.ID_TYPE_ASSE;

                                    if (tipoActivo == 63 || tipoActivo == 3 || tipoActivo == 6)
                                    {
                                        if (string.IsNullOrEmpty(worksheet.Cell(row, 4).Value.ToString()))
                                        {
                                            ActivoTemporal.Valido = false;
                                            ActivoTemporal.Observacion = "";
                                        }
                                    }

                                    //TIPO DE COMPRA
                                    string namBuyMode = worksheet.Cell(row, 13).Value.ToString();
                                    var tipoCompra = dbc.BUY_MODE.FirstOrDefault(x => x.NAM_BUY_MODE == namBuyMode && x.VIG_BUY_MODE == true);

                                    //MARCA
                                    string namManu = worksheet.Cell(row, 7).Value.ToString();
                                    var Marca = dbc.MANUFACTURERs.FirstOrDefault(x => x.NAM_MANU == namManu);

                                    if (Marca == null && !string.IsNullOrEmpty(namManu))
                                    {
                                        MANUFACTURER mANUFACTURER = new MANUFACTURER();
                                        mANUFACTURER.NAM_MANU = namManu;
                                        mANUFACTURER.DESC_MANU = "";
                                        dbc.MANUFACTURERs.Add(mANUFACTURER);
                                        dbc.SaveChanges();
                                        Marca = mANUFACTURER;
                                    }

                                    //NOMBRE COMERCIAL
                                    string namComercial = worksheet.Cell(row, 8).Value.ToString();
                                    var Comercial = dbc.COMMERCIAL_MODEL.FirstOrDefault(x => x.NAM_COMM_MODE == namComercial && x.Estado == true && x.ID_MANU == (Marca.ID_MANU));

                                    //ESTADO
                                    string Estado = worksheet.Cell(row, 21).Value.ToString();
                                    var EstadoActivo = dbc.STATUS_ASSET.FirstOrDefault(x => x.NAM_STAT_ASSE == Estado).ID_STAT_ASSE;

                                    //CONDICION
                                    string Condicion = worksheet.Cell(row, 20).Value.ToString();
                                    var CondicionActivo = dbc.CONDITIONs.FirstOrDefault(x => x.ID_STAT_ASSE == EstadoActivo && x.NAM_COND == Condicion).ID_COND;

                                    if (!string.IsNullOrEmpty(namComercial) && Comercial == null)
                                    {
                                        COMMERCIAL_MODEL comercial = new COMMERCIAL_MODEL();
                                        comercial.NAM_COMM_MODE = namComercial;
                                        comercial.DESC_COMM_MODE = "";
                                        comercial.Estado = true;
                                        comercial.FechaCreacion = DateTime.Now;
                                        comercial.ID_MANU = Marca?.ID_MANU;

                                        dbc.COMMERCIAL_MODEL.Add(comercial);
                                        dbc.SaveChanges();

                                        Comercial = comercial;
                                    }


                                    //CONTRATO
                                    string namContrato = worksheet.Cell(row, 26).Value.ToString();
                                    var Contrato = dbc.Contratoes.FirstOrDefault(x => x.Nombre == namContrato && x.Estado == true && x.ID_ACCO == cuenta);

                                    if (Contrato == null && !string.IsNullOrEmpty(namContrato))
                                    {
                                        Contrato con = new Contrato();
                                        con.Nombre = namContrato;
                                        con.Descripcion = "";
                                        con.Estado = true;
                                        con.FechaCrea = DateTime.Now;
                                        con.ID_ACCO = cuenta;
                                        con.UsuarioCrea = 34;

                                        dbc.Contratoes.Add(con);
                                        dbc.SaveChanges();

                                        Contrato = con;
                                    }

                                    //GRUPO
                                    string nomGrupo = worksheet.Cell(row, 6).Value.ToString();
                                    var Grupo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_PARA == 1070 && x.VAL_ACCO_PARA == nomGrupo && x.VIG_ACCO_PARA == true && x.ID_ACCO == cuenta);

                                    //ARRENDADOR
                                    string nomArrendador = worksheet.Cell(row, 10).Value.ToString();
                                    var Arredandor = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_PARA == 1071 && x.VAL_ACCO_PARA == nomArrendador && x.VIG_ACCO_PARA == true && x.ID_ACCO == cuenta);

                                    //TIPO DE COMPRA
                                    var TipoCompraValido = tipoCompra?.ID_BUY_MODE;


                                    var numeroDeSerie = worksheet.Cell(row, 2).Value.ToString();
                                    var codigoActivo = worksheet.Cell(row, 5).Value.ToString();

                                    var validarActivo = dbc.ASSETs.Where(x => x.COD_ASSE == codigoActivo.ToUpper() && x.SER_NUMB == numeroDeSerie).Count();

                                    if (validarActivo > 0)
                                    {
                                        ActivoTemporal.Valido = false;

                                    }
                                    else
                                    {
                                        ASSET activo = new ASSET
                                        {
                                            ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]),
                                            COD_ASSE = worksheet.Cell(row, 5).Value.ToString(),
                                            ID_TYPE_ASSE = tipoActivo,
                                            ID_BUY_MODE = tipoCompra?.ID_BUY_MODE,
                                            SER_NUMB = worksheet.Cell(row, 2).Value.ToString(),
                                            SUM_ASSE = "Carga Masiva",
                                            ID_MANU = Marca?.ID_MANU,
                                            ID_COMM_MODE = Comercial?.ID_COMM_MODE,
                                            CREATE_ASSE = DateTime.Now,
                                            CREATE_DATE = DateTime.Now,
                                            UserId = 34,
                                            ACQ_DATE = !string.IsNullOrEmpty(worksheet.Cell(row, 18).Value.ToString()) ? worksheet.Cell(row, 18).GetDateTime().Date : (DateTime?)null,
                                            Contrato = worksheet.Cell(row, 8).Value.ToString(),
                                            IdContrato = Contrato?.Id,
                                            FechaFinContrato = !string.IsNullOrEmpty(worksheet.Cell(row, 15).Value.ToString()) ? worksheet.Cell(row, 15).GetDateTime().Date : (DateTime?)null,
                                            IdGrupo = Grupo?.ID_ACCO_PARA,
                                            IdArrendador = Arredandor?.ID_ACCO_PARA
                                        };
                                        dbc.ASSETs.Add(activo);
                                        dbc.SaveChanges();

                                        var AsseId = dbc.ASSETs.OrderByDescending(a => a.ID_ASSE).Select(a => a.ID_ASSE).FirstOrDefault();

                                        string nomSite = worksheet.Cell(row, 16).Value.ToString();
                                        var Site = dbc.SITEs.FirstOrDefault(x => x.ID_ACCO == cuenta && x.NAM_SITE == nomSite);

                                        string nomLocacion = worksheet.Cell(row, 17).Value.ToString();
                                        var Locacion = dbc.LOCATIONs.FirstOrDefault(x => x.NAM_LOCA == nomLocacion && x.ID_SITE == Site.ID_SITE && x.VIG_LOCA == true);

                                        if (Locacion == null)
                                        {
                                            return Json(new { success = false, message = "No se encuentra la locación en la cuenta." });
                                        }


                                        int IdPersEnti = 1007;

                                        if (EstadoActivo == 1 || EstadoActivo == 5)
                                        {
                                            string Usuario = worksheet.Cell(row, 23).Value.ToString().ToUpper().Trim();

                                            var UsuarioAsignado = (from pe in dbe.PERSON_ENTITY
                                                                   join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                                                   select new
                                                                   {
                                                                       pe.ID_PERS_ENTI,
                                                                       Client = ce.FIR_NAME + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME) + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME),
                                                                   }).FirstOrDefault(u => u.Client == Usuario);

                                            if (UsuarioAsignado != null)
                                            {
                                                IdPersEnti = UsuarioAsignado.ID_PERS_ENTI;
                                            }
                                        }

                                        CLIENT_ASSET clienteAsset = new CLIENT_ASSET
                                        {
                                            ID_AREA = 0,
                                            ID_ASSE = AsseId,
                                            ID_COND = CondicionActivo,
                                            ID_LOCA = Locacion?.ID_LOCA,
                                            ID_PERS_ENTI = IdPersEnti,
                                            DAT_STAR = DateTime.Now,
                                            CREATE_DATE = DateTime.Now,
                                            UserId = 34,
                                            SUM_CLIE_ASSE = "Cargado Masivamente",
                                            ID_TYPE_CLIE_ASSE = 6
                                        };

                                        dbc.CLIENT_ASSET.Add(clienteAsset);
                                        
                                        dbc.SaveChanges();
                                    }
                                }
                                else
                                {
                                    // Manejar el caso donde los valores son nulos o vacíos
                                }

                            }
                            dbc.SaveChanges();
                        }

                        if (mensaje != "")
                        {
                            return Json(new { success = false, message = mensaje, });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Datos importados correctamente", });
                        }
                    }
                }
                return Json(new { success = false, message = "No se seleccionó ningún archivo" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al importar datos: " + ex.Message });
            }
        }

        public ActionResult ContarxEstados(int IdTipoActivo, int IdMarca, int IdUsuario, string PalabraClave)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ASSETs.Where(a => a.ID_ACCO == ID_ACCO)
                .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (a, ca) =>
                    new { ca.DAT_END, a.ID_ASSE, ca.ID_COND, a.ID_TYPE_ASSE, a.ID_MANU, a.NAM_EQUI, a.COD_ASSE, a.SER_NUMB, a.MAC_ADRR, ca.ID_PERS_ENTI, a.SUM_ASSE })
                .Where(x => x.DAT_END == null)
                .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new { c.ID_STAT_ASSE, x.ID_ASSE, x.ID_TYPE_ASSE, x.ID_MANU, x.NAM_EQUI, x.COD_ASSE, x.SER_NUMB, x.MAC_ADRR, x.ID_PERS_ENTI, x.SUM_ASSE });

            if (IdTipoActivo != 0)
            {
                query = query.Where(x => x.ID_TYPE_ASSE == IdTipoActivo);
            }
            if (IdMarca != 0)
            {
                query = query.Where(x => x.ID_MANU == IdMarca);
            }
            if (!String.IsNullOrEmpty(PalabraClave))
            {
                query = query.Where(x => x.NAM_EQUI.Contains(PalabraClave) || x.COD_ASSE.Contains(PalabraClave) || x.SER_NUMB.Contains(PalabraClave) || x.MAC_ADRR.Contains(PalabraClave) || x.SUM_ASSE.Contains(PalabraClave));
            }
            if (IdUsuario != 0)
            {
                query = query.Where(x => x.ID_PERS_ENTI == IdUsuario);
            }

            int Surplus = query.Where(x => x.ID_STAT_ASSE == 3).Count();
            int Spare = query.Where(x => x.ID_STAT_ASSE == 2).Count();
            int Assigned = query.Where(x => x.ID_STAT_ASSE == 1).Count();
            int Unassigned = query.Where(x => x.ID_STAT_ASSE == 4).Count();

            return Json(new { Asignado = Assigned, NoAsignado = Unassigned, Repuesto = Spare, Inoperativo = Surplus }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult find()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.IdAcco = ID_ACCO;

            if (ID_ACCO == 60)
            {
                int idGrupoActivo = ObtenerIdGrupoUsuarioBNV();

                if (idGrupoActivo != 0)
                {
                    string grupoActivo = ObtenerNomGrupoUsuarioBNV(idGrupoActivo);

                    int supervisor = 0;
                    if (Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"]) == 1)
                        supervisor = 1;

                    var easset = new ASSET();
                    easset.IdGrupo = idGrupoActivo;
                    easset.UMinera = ObtenerUMineraUsuarioBNV(Convert.ToInt32(Session["ID_PERS_ENTI"]), idGrupoActivo);

                    ViewBag.Grupo = grupoActivo;
                    ViewBag.Supervisor = supervisor;
                    return View("FindBNV", easset);
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            else if (ID_ACCO == 55)
            {
                if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 1)
                {
                    int idGrupoIT = ObtenerIdGrupo("IT");
                    ViewBag.IdGrupoIT = idGrupoIT;
                }
                else if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
                {
                    return RedirectToAction("FindOT", "Asset");
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }

            return View();
        }

        //carga avanzada
        public ActionResult FindResult()
        {
            string Codigo = Request.Params["COD_ASSE"].ToString();
            //string Usuario = Convert.ToString(Request.Params["ID_ENTI"].ToString());
            string Marca = Request.Params["ID_MANU"].ToString();
            string Sitio = Request.Params["ID_SITE"].ToString();
            string Estado = Request.Params["ID_STAT_ASSE"].ToString();
            //string TipoActivo = Convert.ToString(Request.Params["ID_TYPE_ASSE"].ToString());
            string NombreEquipo = Request.Params["NAM_EQUI"].ToString();
            string Serie = Request.Params["SER_NUMB"].ToString();
            //string SOLPED = Convert.ToString(Request.Params["SOLPED"].ToString());
            string TiposActivo = Request.Params["valTypeAsset"].ToString();
            string Usuarios = Request.Params["Usuarios"].ToString();
            string Contratos = Request.Params["Contratos"].ToString();

            string NumeroFactura = "";
            string GuiaRemision = "";
            string OrdenCompra = "";



            if (Convert.ToInt32(Session["ID_ACCO"]) == 4)
            {
                NumeroFactura = Request.Params["NumeroFactura"].ToString();
                GuiaRemision = Request.Params["GuiaRemision"].ToString();

            }

            if (Convert.ToInt32(Session["ID_ACCO"]) == 55)
            {
                NumeroFactura = Request.Params["NumeroFactura"].ToString();
                GuiaRemision = Request.Params["GuiaRemision"].ToString();
                OrdenCompra = Request.Params["OrdenCompra"].ToString();
            }

            string ModeloComercial = Convert.ToString(Request.Params["ID_COMM_MODE"] == null ? "0" : Request.Params["ID_COMM_MODE"]);
            string ModeloFabrica = Convert.ToString(Request.Params["ID_MANU_MODE"] == null ? "0" : Request.Params["ID_MANU_MODE"]);
            string Locacion = Convert.ToString(Request.Params["ID_LOCA"] == null ? "0" : Request.Params["ID_LOCA"]);
            string Condicion = Convert.ToString(Request.Params["ID_COND"] == null ? "0" : Request.Params["ID_COND"]);
            string Mac = Convert.ToString(Request.Params["MAC_ADRR"] == null ? "" : Request.Params["MAC_ADRR"]);

            if (String.IsNullOrEmpty(Marca)) Marca = "0";
            if (String.IsNullOrEmpty(Sitio)) Sitio = "0";
            if (String.IsNullOrEmpty(Estado)) Estado = "0";
            if (String.IsNullOrEmpty(ModeloComercial)) ModeloComercial = "0";
            if (String.IsNullOrEmpty(ModeloFabrica)) ModeloFabrica = "0";
            if (String.IsNullOrEmpty(Locacion)) Locacion = "0";
            if (String.IsNullOrEmpty(Condicion)) Condicion = "0";

            int IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

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

            if (Convert.ToInt32(Session["ID_ACCO"]) == 55)
            {
                grupos = Request.Params["IdGrupoIT"].ToString();
                if (String.IsNullOrEmpty(grupos)) grupos = "";
            }

            if (String.IsNullOrEmpty(Contratos)) Contratos = "0";
            Contratos = Contratos.Substring(0, Contratos.Length - 1);

            var query = dbc.ActBuscar(Codigo, Usuarios, Contratos, TiposActivo, Marca, ModeloComercial, ModeloFabrica, Sitio, Locacion, Estado, Condicion, Serie, Mac, NombreEquipo, NumeroFactura, GuiaRemision, OrdenCompra, IdAcco, usuariosResp, grupos, subTipo).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export()
        {
            string COD_ASSE = Convert.ToString(Request.Params["COD_ASSE"].ToString());
            string S_ID_PERS_ENTI = Convert.ToString(Request.Params["ID_ENTI"].ToString());
            string S_ID_MANU = Convert.ToString(Request.Params["ID_MANU"].ToString());
            string S_ID_SITE = Convert.ToString(Request.Params["ID_SITE"].ToString());
            string S_ID_STAT_ASSE = Convert.ToString(Request.Params["ID_STAT_ASSE"].ToString());
            string S_ID_TYPE_ASSE = Convert.ToString(Request.Params["ID_TYPE_ASSE"].ToString());
            string NAM_EQUI = Convert.ToString(Request.Params["NAM_EQUI"].ToString());
            string SER_NUMB = Convert.ToString(Request.Params["SER_NUMB"].ToString());

            string TiposActivo = Convert.ToString(Request.Params["ExpTypeAsset"].ToString());
            string Usuarios = Convert.ToString(Request.Params["Usuarios"].ToString());
            string Contratos = Convert.ToString(Request.Params["Contratos"].ToString());

            //Selección múltiple
            TiposActivo = TiposActivo.Substring(0, TiposActivo.Length - 1);
            if (String.IsNullOrEmpty(Usuarios)) Usuarios = "0,";
            Usuarios = Usuarios.Substring(0, Usuarios.Length - 1);
            if (String.IsNullOrEmpty(Contratos)) Contratos = "0,";
            Contratos = Contratos.Substring(0, Contratos.Length - 1);

            string[] TipoActivo = TiposActivo.Split(',');
            string[] Usuario = Usuarios.Split(',');
            string[] Contrato = Contratos.Split(',');

            int[] intTipoActivo = new int[100];
            int[] intUsuario = new int[100];
            int[] intContrato = new int[100];

            int cont = 0;
            foreach (string tipo in TipoActivo)
            {
                intTipoActivo[cont] = Convert.ToInt32(tipo);
                cont++;
            }
            cont = 0;
            foreach (string strUsuario in Usuario)
            {
                intUsuario[cont] = Convert.ToInt32(strUsuario);
                cont++;
            }
            cont = 0;
            foreach (string strContrato in Contrato)
            {
                intContrato[cont] = Convert.ToInt32(strContrato);
                cont++;
            }

            string S_ID_COMM_MODE = null, S_ID_MANU_MODE = null, S_ID_LOCA = null, S_ID_COND = null;

            int ID_PERS_ENTI, ID_MANU, ID_SITE, ID_STAT_ASSE, ID_TYPE_ASSE, ID_COMM_MODE, ID_MANU_MODE, ID_LOCA, ID_COND;

            try { S_ID_COMM_MODE = Convert.ToString(Request.Params["ID_COMM_MODE"].ToString()); }
            catch { }

            try { S_ID_MANU_MODE = Convert.ToString(Request.Params["ID_MANU_MODE"].ToString()); }
            catch { }

            try { S_ID_LOCA = Convert.ToString(Request.Params["ID_LOCA"].ToString()); }
            catch { }

            try { S_ID_COND = Convert.ToString(Request.Params["ID_COND"].ToString()); }
            catch { }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbc.ASSETs.Where(a => a.ID_ACCO == ID_ACCO)
                .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (a, ca) => new
                {
                    a.ID_ASSE,
                    a.CREATE_ASSE,
                    a.ID_TYPE_ASSE,
                    a.ID_MANU,
                    a.ID_COMM_MODE,
                    a.SER_NUMB,
                    a.SOLPED,
                    a.LOTE,
                    a.ID_TICK,
                    a.COD_ASSE,
                    a.NAM_EQUI,
                    a.ID_MANU_MODE,
                    a.ID_COST_CENT,
                    a.ID_BUY_MODE,
                    a.COST,
                    a.IdContrato,
                    ca.DAT_END,
                    ca.CREATE_DATE,
                    ca.ID_COND,
                    ca.ID_LOCA,
                    ca.UserId,
                    ca.ID_PERS_ENTI
                })
                .Where(x => x.DAT_END == null)
                .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new
                {
                    x.ID_ASSE,
                    x.ID_PERS_ENTI,
                    x.CREATE_ASSE,
                    x.CREATE_DATE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_LOCA,
                    x.UserId,
                    x.COD_ASSE,
                    x.NAM_EQUI,
                    x.ID_MANU_MODE,
                    x.ID_COST_CENT,
                    x.LOTE,
                    x.ID_BUY_MODE,
                    x.COST,
                    x.IdContrato,
                    c.ID_STAT_ASSE,
                    c.NAM_COND,
                    c.ID_COND
                })
                .Join(dbc.TYPE_ASSET, x => x.ID_TYPE_ASSE, ta => ta.ID_TYPE_ASSE, (x, ta) => new
                {
                    x.ID_ASSE,
                    x.ID_STAT_ASSE,
                    x.NAM_COND,
                    x.ID_PERS_ENTI,
                    x.CREATE_ASSE,
                    x.CREATE_DATE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_LOCA,
                    x.UserId,
                    x.COD_ASSE,
                    x.NAM_EQUI,
                    x.ID_COND,
                    x.ID_MANU_MODE,
                    x.ID_COST_CENT,
                    x.LOTE,
                    x.ID_BUY_MODE,
                    x.COST,
                    x.IdContrato,
                    ta.NAM_TYPE_ASSE,
                    ta.COLOR,
                    ta.INDICE
                })
                .Join(dbc.LOCATIONs, x => x.ID_LOCA, l => l.ID_LOCA, (x, l) => new
                {
                    x.ID_ASSE,
                    x.ID_STAT_ASSE,
                    x.NAM_COND,
                    x.ID_PERS_ENTI,
                    x.CREATE_ASSE,
                    x.CREATE_DATE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_LOCA,
                    x.NAM_TYPE_ASSE,
                    x.COLOR,
                    x.INDICE,
                    x.UserId,
                    x.COD_ASSE,
                    x.NAM_EQUI,
                    x.ID_COND,
                    x.ID_MANU_MODE,
                    x.ID_COST_CENT,
                    x.LOTE,
                    x.ID_BUY_MODE,
                    x.COST,
                    x.IdContrato,
                    l.NAM_LOCA,
                    l.ID_SITE
                });

            if (!String.IsNullOrEmpty(COD_ASSE))
            {
                query = query.Where(a => a.COD_ASSE.ToUpper().Contains(COD_ASSE.ToUpper()));
            }

            if (Usuario.Length > 1)
            {
                //query = query.Where(a => a.ID_PERS_ENTI == ID_PERS_ENTI);
                query = query.Where(a => intUsuario.Contains(a.ID_PERS_ENTI.Value));
            }

            if (Int32.TryParse(S_ID_MANU, out ID_MANU))
            {
                query = query.Where(a => a.ID_MANU == ID_MANU);
            }

            if (Int32.TryParse(S_ID_SITE, out ID_SITE))
            {
                query = query.Where(a => a.ID_SITE == ID_SITE);
            }

            if (Int32.TryParse(S_ID_STAT_ASSE, out ID_STAT_ASSE))
            {
                query = query.Where(a => a.ID_STAT_ASSE == ID_STAT_ASSE);
            }

            //if (Int32.TryParse(S_ID_TYPE_ASSE, out ID_TYPE_ASSE))
            //{
            //    query = query.Where(a => a.ID_TYPE_ASSE == ID_TYPE_ASSE);
            //}
            if (TipoActivo.Length > 1)
            {
                query = query.Where(a => intTipoActivo.Contains(a.ID_TYPE_ASSE.Value));
            }

            if (Int32.TryParse(S_ID_COMM_MODE, out ID_COMM_MODE))
            {
                query = query.Where(a => a.ID_COMM_MODE == ID_COMM_MODE);
            }

            if (Int32.TryParse(S_ID_MANU_MODE, out ID_MANU_MODE))
            {
                query = query.Where(a => a.ID_MANU_MODE == ID_MANU_MODE);
            }

            if (Int32.TryParse(S_ID_LOCA, out ID_LOCA))
            {
                query = query.Where(a => a.ID_LOCA == ID_LOCA);
            }

            if (Int32.TryParse(S_ID_COND, out ID_COND))
            {
                query = query.Where(a => a.ID_COND == ID_COND);
            }

            if (!String.IsNullOrEmpty(NAM_EQUI))
            {
                query = query.Where(a => a.NAM_EQUI.ToUpper().Contains(NAM_EQUI.ToUpper()));
            }

            if (!String.IsNullOrEmpty(SER_NUMB))
            {
                query = query.Where(a => a.SER_NUMB.ToUpper().Contains(SER_NUMB.ToUpper()));
            }

            if (Contrato.Length > 1)
            {
                query = query.Where(a => intContrato.Contains(a.IdContrato.Value));
            }

            //if (!String.IsNullOrEmpty(SOLPED))
            //{
            //    query = query.Where(a => a.SOLPED.ToUpper().Contains(SOLPED.ToUpper()));
            //}

            var query2 = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO)
                         .Join(dbe.PERSON_ENTITY, x => x.ID_PERS_ENTI, pe => pe.ID_PERS_ENTI, (x, pe) => new
                         {
                             x.ID_PERS_ENTI,
                             pe.ID_ENTI2,
                             pe.ID_AREA
                         })
                         .Join(dbe.AREAs, x => x.ID_AREA, a => a.ID_AREA, (x, a) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_ENTI2,
                             x.ID_AREA,
                             a.NOM_AREA
                         })
                         .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_ENTI2,
                             x.ID_AREA,
                             x.NOM_AREA,
                             ce.ID_ENTI,
                             ce.FIR_NAME,
                             ce.LAS_NAME,
                             ce.UserId
                         });

            var query3 = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_SOUR == 4)
                        .Join(dbc.TYPE_TICKET, t => t.ID_TYPE_TICK, tt => tt.ID_TYPE_TICK, (t, tt) => new
                        {
                            t.ID_TICK,
                            t.COD_TICK,
                            t.ID_TYPE_TICK,
                            t.ID_ASSE,
                            tt.NAM_TYPE_TICK
                        });

            var result = (from a in query.OrderBy(a => a.INDICE).ThenBy(a => a.NAM_TYPE_ASSE).ToList()
                          join c in query2 on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                          join m in dbc.MANUFACTURERs on a.ID_MANU equals m.ID_MANU
                          join cm in dbc.COMMERCIAL_MODEL on a.ID_COMM_MODE equals cm.ID_COMM_MODE into acm
                          from x2 in acm.DefaultIfEmpty()
                          join mm in dbc.MANUFACTURER_MODEL on a.ID_MANU_MODE equals mm.ID_MANU_MODE into amm
                          from x3 in amm.DefaultIfEmpty()
                          join bm in dbc.BUY_MODE on a.ID_BUY_MODE equals bm.ID_BUY_MODE into abm
                          from x4 in abm.DefaultIfEmpty()
                          join t in query3 on a.ID_TICK equals t.ID_TICK into lt
                          from x in lt.DefaultIfEmpty()
                          join tt in dbc.TYPE_TICKET on (x == null ? 0 : x.ID_TICK) equals tt.ID_TYPE_TICK into ltt
                          from y in ltt.DefaultIfEmpty()
                          join s in dbc.SITEs on a.ID_SITE equals s.ID_SITE
                          join ucl in query2 on a.UserId equals ucl.UserId
                          join cc in dbc.COST_CENTER on a.ID_COST_CENT equals cc.ID_COST_CENT into lcc
                          from z in lcc.DefaultIfEmpty()
                          join contr in dbc.Contratoes on a.IdContrato equals contr.Id into lcont
                          from xcont in lcont.DefaultIfEmpty()
                          select new
                          {
                              CODIGO_ACTIVO = (a.COD_ASSE == null ? String.Empty : a.COD_ASSE),
                              NUMERO_SERIE = (a.SER_NUMB == null ? String.Empty : a.SER_NUMB),
                              NOMBRE_EQUIPO = (a.NAM_EQUI == null ? String.Empty : a.NAM_EQUI),
                              COSTO = (a.COST == null ? String.Empty : Convert.ToString(a.COST)),
                              LOTE = (a.LOTE == null ? String.Empty : Convert.ToString(a.LOTE)),
                              AREA = (c.NOM_AREA == null ? String.Empty : c.NOM_AREA),
                              CONDICION = a.NAM_COND.ToLower(),
                              USUARIO = c.FIR_NAME.ToLower() + ' ' + c.LAS_NAME.ToLower(),
                              REGISTRO = String.Format("{0:G}", a.CREATE_ASSE),
                              UPDATE = String.Format("{0:G}", a.CREATE_DATE),
                              TIPO_ACTIVO = a.NAM_TYPE_ASSE.ToLower(),
                              MARCA = m.NAM_MANU,
                              SOLPED = (a.SOLPED == null ? String.Empty : a.SOLPED),
                              LOCACION = a.NAM_LOCA.ToLower(),
                              SITE = s.NAM_SITE.ToLower(),
                              CREADO_POR = ucl.FIR_NAME.ToLower() + ' ' + ucl.LAS_NAME.ToLower(),
                              CECO = (z == null ? "-" : z.COD_COST_CENT),
                              MODELO_COMERCIAL = (x2 == null ? String.Empty : x2.NAM_COMM_MODE),
                              MODELO_FABRICA = (x3 == null ? String.Empty : x3.NAM_MANU_MODE),
                              MODO_ADQUISICION = (x4 == null ? String.Empty : x4.NAM_BUY_MODE),
                              CONTRATO = (xcont.Nombre == null ? "-" : xcont.Nombre)
                          }).ToList();

            var grid = new GridView();
            grid.DataSource = result.ToList();
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=BusquedaActivo" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();

        }

        public ActionResult FindResultGraph()
        {

            string COD_ASSE = Convert.ToString(Request.Params["COD_ASSE"].ToString());
            string S_ID_PERS_ENTI = Convert.ToString(Request.Params["ID_ENTI"].ToString());
            string S_ID_MANU = Convert.ToString(Request.Params["ID_MANU"].ToString());
            string S_ID_SITE = Convert.ToString(Request.Params["ID_SITE"].ToString());
            string S_ID_STAT_ASSE = Convert.ToString(Request.Params["ID_STAT_ASSE"].ToString());
            string S_ID_TYPE_ASSE = Convert.ToString(Request.Params["ID_TYPE_ASSE"].ToString());
            string NAM_EQUI = Convert.ToString(Request.Params["NAM_EQUI"].ToString());
            string SER_NUMB = Convert.ToString(Request.Params["SER_NUMB"].ToString());
            //string SOLPED = Convert.ToString(Request.Params["SOLPED"].ToString());
            //string CECO = Convert.ToString(Request.Params["CECO"].ToString());

            string S_ID_COMM_MODE = null, S_ID_MANU_MODE = null, S_ID_LOCA = null, S_ID_COND = null;

            int ID_PERS_ENTI, ID_MANU, ID_SITE, ID_STAT_ASSE, ID_TYPE_ASSE, ID_COMM_MODE, ID_MANU_MODE, ID_LOCA, ID_COND;

            try { S_ID_COMM_MODE = Convert.ToString(Request.Params["ID_COMM_MODE"].ToString()); }
            catch { }

            try { S_ID_MANU_MODE = Convert.ToString(Request.Params["ID_MANU_MODE"].ToString()); }
            catch { }

            try { S_ID_LOCA = Convert.ToString(Request.Params["ID_LOCA"].ToString()); }
            catch { }

            try { S_ID_COND = Convert.ToString(Request.Params["ID_COND"].ToString()); }
            catch { }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            //int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            //int take = Convert.ToInt32(Request.Params["take"].ToString());


            var query = dbc.ASSETs.Where(a => a.ID_ACCO == ID_ACCO)
                .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (a, ca) => new
                {

                    a.ID_ASSE,
                    a.CREATE_ASSE,
                    a.ID_TYPE_ASSE,
                    a.ID_MANU,
                    a.ID_COMM_MODE,
                    a.SER_NUMB,
                    a.SOLPED,
                    a.ID_TICK,
                    a.COD_ASSE,
                    a.NAM_EQUI,
                    a.ID_MANU_MODE,
                    a.ID_COST_CENT,
                    ca.ID_COND,
                    ca.ID_PERS_ENTI,
                    ca.DAT_END,
                    ca.CREATE_DATE,
                    ca.ID_LOCA,
                    ca.UserId
                })
                .Where(x => x.DAT_END == null)
                .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new
                {
                    x.ID_ASSE,
                    x.ID_PERS_ENTI,
                    x.CREATE_ASSE,
                    x.CREATE_DATE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_LOCA,
                    x.UserId,
                    x.COD_ASSE,
                    x.NAM_EQUI,
                    x.ID_MANU_MODE,
                    x.ID_COST_CENT,
                    c.ID_STAT_ASSE,
                    c.NAM_COND,
                    c.ID_COND
                })
                .Join(dbc.TYPE_ASSET, x => x.ID_TYPE_ASSE, ta => ta.ID_TYPE_ASSE, (x, ta) => new
                {
                    x.ID_ASSE,
                    x.ID_STAT_ASSE,
                    x.NAM_COND,
                    x.ID_PERS_ENTI,
                    x.CREATE_ASSE,
                    x.CREATE_DATE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_LOCA,
                    x.UserId,
                    x.COD_ASSE,
                    x.NAM_EQUI,
                    x.ID_COND,
                    x.ID_MANU_MODE,
                    x.ID_COST_CENT,
                    ta.NAM_TYPE_ASSE,
                    ta.COLOR,
                    ta.INDICE
                })
                .Join(dbc.LOCATIONs, x => x.ID_LOCA, l => l.ID_LOCA, (x, l) => new
                {
                    x.ID_ASSE,
                    x.ID_STAT_ASSE,
                    x.NAM_COND,
                    x.ID_PERS_ENTI,
                    x.CREATE_ASSE,
                    x.CREATE_DATE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_LOCA,
                    x.NAM_TYPE_ASSE,
                    x.COLOR,
                    x.INDICE,
                    x.UserId,
                    x.COD_ASSE,
                    x.NAM_EQUI,
                    x.ID_COND,
                    x.ID_MANU_MODE,
                    x.ID_COST_CENT,
                    l.ID_SITE,
                    l.NAM_LOCA
                });

            if (!String.IsNullOrEmpty(COD_ASSE))
            {
                query = query.Where(a => a.COD_ASSE.ToUpper().Contains(COD_ASSE.ToUpper()));
            }

            if (Int32.TryParse(S_ID_PERS_ENTI, out ID_PERS_ENTI))
            {
                query = query.Where(a => a.ID_PERS_ENTI == ID_PERS_ENTI);
            }

            if (Int32.TryParse(S_ID_MANU, out ID_MANU))
            {
                query = query.Where(a => a.ID_MANU == ID_MANU);
            }

            if (Int32.TryParse(S_ID_SITE, out ID_SITE))
            {
                query = query.Where(a => a.ID_SITE == ID_SITE);
            }

            if (Int32.TryParse(S_ID_COMM_MODE, out ID_COMM_MODE))
            {
                query = query.Where(a => a.ID_COMM_MODE == ID_COMM_MODE);
            }

            if (Int32.TryParse(S_ID_MANU_MODE, out ID_MANU_MODE))
            {
                query = query.Where(a => a.ID_MANU_MODE == ID_MANU_MODE);
            }

            if (Int32.TryParse(S_ID_LOCA, out ID_LOCA))
            {
                query = query.Where(a => a.ID_LOCA == ID_LOCA);
            }

            if (!String.IsNullOrEmpty(NAM_EQUI))
            {
                query = query.Where(a => a.NAM_EQUI.ToUpper().Contains(NAM_EQUI.ToUpper()));
            }

            if (!String.IsNullOrEmpty(SER_NUMB))
            {
                query = query.Where(a => a.SER_NUMB.ToUpper().Contains(SER_NUMB.ToUpper()));
            }

            //if (!String.IsNullOrEmpty(SOLPED))
            //{
            //    query = query.Where(a => a.SOLPED.ToUpper().Contains(SOLPED.ToUpper()));
            //}

            var query1 = query;
            var query2 = query;

            if (Int32.TryParse(S_ID_STAT_ASSE, out ID_STAT_ASSE))
            {
                query1 = query1.Where(a => a.ID_STAT_ASSE == ID_STAT_ASSE);
            }

            if (Int32.TryParse(S_ID_COND, out ID_COND))
            {
                query1 = query1.Where(a => a.ID_COND == ID_COND);
            }

            var EqbyType = (from x in query1.ToList()
                            group x by new { x.ID_TYPE_ASSE, x.NAM_TYPE_ASSE } into g
                            select new
                            {
                                name = g.Key.NAM_TYPE_ASSE.ToLower(),
                                cantidad = g.Count()
                            }).OrderByDescending(r => r.cantidad).Take(12);

            if (Int32.TryParse(S_ID_TYPE_ASSE, out ID_TYPE_ASSE))
            {
                query2 = query2.Where(a => a.ID_TYPE_ASSE == ID_TYPE_ASSE);
            }

            var EqbyStatus = (from x in query2.ToList()
                              join sa in dbc.STATUS_ASSET on x.ID_STAT_ASSE equals sa.ID_STAT_ASSE
                              group sa by new { sa.ID_STAT_ASSE, sa.NAM_STAT_ASSE } into g
                              select new
                              {
                                  name = g.Key.NAM_STAT_ASSE.Substring(0, 1).ToUpper() + g.Key.NAM_STAT_ASSE.Substring(1, g.Key.NAM_STAT_ASSE.Length - 1).ToLower(),
                                  y = g.Count()
                              }).OrderByDescending(r => r.y);

            return Json(new { EqbyType = EqbyType, EqbyStatus = EqbyStatus }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteGrafico(int idTipoActivo)
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbc.ASSETs.Where(a => a.ID_ACCO == ID_ACCO)
                .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (a, ca) => new
                {

                    a.ID_ASSE,
                    a.CREATE_ASSE,
                    a.ID_TYPE_ASSE,
                    a.ID_MANU,
                    a.ID_COMM_MODE,
                    a.SER_NUMB,
                    a.SOLPED,
                    a.ID_TICK,
                    a.COD_ASSE,
                    a.NAM_EQUI,
                    a.ID_MANU_MODE,
                    a.ID_COST_CENT,
                    ca.ID_COND,
                    ca.ID_PERS_ENTI,
                    ca.DAT_END,
                    ca.CREATE_DATE,
                    ca.ID_LOCA,
                    ca.UserId
                })
                .Where(x => x.DAT_END == null)
                .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new
                {
                    x.ID_ASSE,
                    x.ID_PERS_ENTI,
                    x.CREATE_ASSE,
                    x.CREATE_DATE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_LOCA,
                    x.UserId,
                    x.COD_ASSE,
                    x.NAM_EQUI,
                    x.ID_MANU_MODE,
                    x.ID_COST_CENT,
                    c.ID_STAT_ASSE,
                    c.NAM_COND,
                    c.ID_COND
                })
                .Join(dbc.TYPE_ASSET, x => x.ID_TYPE_ASSE, ta => ta.ID_TYPE_ASSE, (x, ta) => new
                {
                    x.ID_ASSE,
                    x.ID_STAT_ASSE,
                    x.NAM_COND,
                    x.ID_PERS_ENTI,
                    x.CREATE_ASSE,
                    x.CREATE_DATE,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COMM_MODE,
                    x.SER_NUMB,
                    x.SOLPED,
                    x.ID_TICK,
                    x.ID_LOCA,
                    x.UserId,
                    x.COD_ASSE,
                    x.NAM_EQUI,
                    x.ID_COND,
                    x.ID_MANU_MODE,
                    x.ID_COST_CENT,
                    ta.NAM_TYPE_ASSE,
                    ta.COLOR,
                    ta.INDICE
                });
            //.Join(dbc.LOCATIONs, x => x.ID_LOCA, l => l.ID_LOCA, (x, l) => new
            //{
            //    x.ID_ASSE,
            //    x.ID_STAT_ASSE,
            //    x.NAM_COND,
            //    x.ID_PERS_ENTI,
            //    x.CREATE_ASSE,
            //    x.CREATE_DATE,
            //    x.ID_TYPE_ASSE,
            //    x.ID_MANU,
            //    x.ID_COMM_MODE,
            //    x.SER_NUMB,
            //    x.SOLPED,
            //    x.ID_TICK,
            //    x.ID_LOCA,
            //    x.NAM_TYPE_ASSE,
            //    x.COLOR,
            //    x.INDICE,
            //    x.UserId,
            //    x.COD_ASSE,
            //    x.NAM_EQUI,
            //    x.ID_COND,
            //    x.ID_MANU_MODE,
            //    x.ID_COST_CENT,
            //    l.ID_SITE,
            //    l.NAM_LOCA
            //});

            var query1 = query;
            var query2 = query;

            var EqbyType = (from x in query1.ToList()
                            group x by new { x.ID_TYPE_ASSE, x.NAM_TYPE_ASSE } into g
                            select new
                            {
                                name = g.Key.NAM_TYPE_ASSE.ToLower(),
                                cantidad = g.Count()
                            }).OrderByDescending(r => r.cantidad).Take(12);

            if (idTipoActivo != 0)
            {
                query2 = query2.Where(x => x.ID_TYPE_ASSE == idTipoActivo);
            }

            var EqbyStatus = (from x in query2.ToList()
                              join sa in dbc.STATUS_ASSET on x.ID_STAT_ASSE equals sa.ID_STAT_ASSE
                              group sa by new { sa.ID_STAT_ASSE, sa.NAM_STAT_ASSE } into g
                              select new
                              {
                                  name = g.Key.NAM_STAT_ASSE.Substring(0, 1).ToUpper() + g.Key.NAM_STAT_ASSE.Substring(1, g.Key.NAM_STAT_ASSE.Length - 1).ToLower(),
                                  y = g.Count()
                              }).OrderByDescending(r => r.y);

            return Json(new { EqbyType = EqbyType, EqbyStatus = EqbyStatus }, JsonRequestBehavior.AllowGet);
        }

        public string ValidarIdsAsset(string ids)
        {

            string[] ids_asset = ids.Split(',');
            string msn = "";

            var query2 = (from id_asset in ids_asset select id_asset);

            foreach (string dtide in query2)
            {
                var idsearch = Convert.ToInt32(dtide);
                var query = dbc.ASSETs
                    .Where(a => a.ID_ASSE == idsearch)
                    .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
                    {
                        x.ID_ASSE,
                        ca.ID_COND
                    })
                    .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new
                    {
                        x.ID_ASSE,
                        x.ID_COND,
                        c.ID_STAT_ASSE
                    })
                    .Join(dbc.STATUS_ASSET, x => x.ID_STAT_ASSE, s => s.ID_STAT_ASSE, (x, s) => new
                    {
                        x.ID_ASSE,
                        x.ID_COND,
                        x.ID_STAT_ASSE,
                        s.NAM_STAT_ASSE
                    }).Where(x => x.NAM_STAT_ASSE != "UNASSIGNED");

                int cant = query.Count();
                if (cant > 0)
                {
                    msn = msn + idsearch.ToString() + ",";
                }
            }
            if (msn.Length > 0) { return msn; }
            else { return "OK"; }
            ////dt is the DataTable you're working with.
            //var tblasset = from row in dtDatos.AsEnumerable()
            //               group row["Id"] by row["Id"] into codasset
            //               select codasset;

            //var query = from g in tblasset
            //            select new
            //            {
            //                idstbl = g.Key
            //            };
            ////cart.Entries.Select(c => c.ProductId).Contains(p.ProductId)           
        }

        // GET: /Asset/Create
        [Authorize]
        public ActionResult Create()
        {
            int perfil = Convert.ToInt32(Session["SOPORTE"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (ID_ACCO == 4)
            {
                if (Convert.ToInt32(Session["SOPORTE"]) == 1)
                {
                    var easset = new ASSET();
                    easset.CREATE_ASSE = DateTime.Now;
                    ViewBag.MON_ACCO = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).MON_ACCO.ToString();
                    return View(easset);
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                    //return Content("Necesitas permisos para acceder a esta sección."); 
                }
            }
            else if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
            {
                if (Convert.ToInt32(Session["GESTOR_ACTIVOS_MINSUR_MARCOBRE_RAURA"]) == 1)
                {
                    var easset = new ASSET();
                    easset.CREATE_ASSE = DateTime.Now;
                    ViewBag.MON_ACCO = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).MON_ACCO.ToString();
                    return View(easset);
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                    //return Content("Necesitas permisos para acceder a esta sección."); 
                }
            }
            else if (ID_ACCO == 55)
            {
                if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 1)
                {
                    int idGrupoIT = ObtenerIdGrupo("IT");
                    ViewBag.IdGrupoIT = idGrupoIT;
                    return View("CreateHudbay");
                }
                else if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
                {
                    return RedirectToAction("CreateOT", "Asset");
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            else if (ID_ACCO == 60)
            {
                int idGrupoActivo = ObtenerIdGrupoUsuarioBNV();

                if (idGrupoActivo != 0)
                {
                    //Grupo MICROINFORMATICO solo tiene acceso el SUPERVISOR
                    var can = GruposBNV().Where(x => Convert.ToInt32(Session[x.Key]) == 1).ToList().Count();
                    if (can == 1 && Convert.ToInt32(Session["GESTOR_ACTIVOS_BNV_MICROINFORMATICO"]) == 1)
                        return RedirectToAction("Index", "Error");

                    return View("CreateBNV");
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            else
            {
                var easset = new ASSET();
                easset.CREATE_ASSE = DateTime.Now;
                ViewBag.MON_ACCO = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).MON_ACCO.ToString();
                return View(easset);
            }
        }

        // POST: /Asset/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> archivos, ASSET asset)
        {
            int ID_ACCO = 0;
            int ID_USER = 0;
            try
            {
                ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                ID_USER = Convert.ToInt32(Session["UserId"].ToString());
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
            }


            int ID_PER_ENTI, ID_COND = 0, ID_LOCA = 0, ID_QUEU, ID_PRIO = 4, ID_ASSI = 0, ID_TYPE_ASSE, ID_MANU, ID_BUY_MODE, ID_COST_CENT = 0;
            DateTime ACQ_DATE;
            string COD_ASSE = Convert.ToString(Request.Form["COD_ASSE"]);
            string SER_NUMB = Convert.ToString(Request.Form["SER_NUMB"]);
            string SUM_ASSE = Convert.ToString(Request.Form["SUM_ASSE"]);

            int sw = 0;
            if (COD_ASSE.Trim().Length == 0) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_ASSE"]), out ID_TYPE_ASSE) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_MANU"]), out ID_MANU) == false) { sw = 1; }
            else if (SER_NUMB.Trim().Length == 0) { sw = 1; }
            //else if (Int32.TryParse(Convert.ToString(Request.Form["ID_COND"]), out ID_COND) == false) { sw = 1; }
            //else if (DateTime.TryParse(Convert.ToString(Request.Form["ACQ_DATE"]), out ACQ_DATE) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_LOCA"]), out ID_LOCA) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_BUY_MODE"]), out ID_BUY_MODE) == false) { sw = 1; }
            else
            {
                if (Convert.ToInt32(Session["ID_ACCO"]) == 1) //Aplica solo para GF
                {
                    if (Int32.TryParse(Convert.ToString(Request.Form["ID_COST_CENT"]), out ID_COST_CENT) == false) { sw = 1; }
                }
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            SUM_ASSE = SUM_ASSE.Replace("&nbsp;", "");
            SUM_ASSE = SUM_ASSE.Replace("<br />", "");
            if (SUM_ASSE.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            try
            {
                ID_QUEU = Convert.ToInt32(Session["ID_QUEU"].ToString());
                ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                ID_QUEU = 0;
            }

            try
            {
                int cantAsset = dbc.ASSETs.Where(x => (x.COD_ASSE.ToLower() == COD_ASSE.ToLower() || x.SER_NUMB == SER_NUMB) && x.ID_ACCO == ID_ACCO).Count();
                if (cantAsset > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','3');}window.onload=init;</script>");
                }
            }
            catch
            {

            }

            if (ModelState.IsValid)
            {
                //
                var query = dbc.PARAMETERs.Where(x => x.NAM_PARA == "RESPONSIBLE ASSET")
                    .Join(dbc.ACCOUNT_PARAMETER, x => x.ID_PARA, p => p.ID_PARA, (x, p) => new
                    {
                        p.VAL_ACCO_PARA,
                        p.ID_ACCO
                    }).Where(x => x.ID_ACCO == ID_ACCO).First();

                ID_PER_ENTI = Convert.ToInt32(query.VAL_ACCO_PARA);
                ID_PRIO = Convert.ToInt32(dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_PER_ENTI).ID_PRIO);

                asset.CREATE_DATE = DateTime.Now;
                asset.ID_ACCO = ID_ACCO;
                asset.UserId = ID_USER;

                if ((ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58) && (asset.ID_TYPE_ASSE == 3 || asset.ID_TYPE_ASSE == 6 || asset.ID_TYPE_ASSE == 63))
                {
                    asset.IdSubTipoActivo = Convert.ToInt32(Request.Form["SubTipoActivo"]);
                }
                if ((ID_ACCO == 1) && (asset.ID_TYPE_ASSE == 6 || asset.ID_TYPE_ASSE == 3 || asset.ID_TYPE_ASSE == 41990
                        || asset.ID_TYPE_ASSE == 18 || asset.ID_TYPE_ASSE == 1223))
                {
                    asset.CapacidadRam = Convert.ToInt32(Request.Form["CapacidadRam"]);
                    asset.CapacidadDisco = Convert.ToInt32(Request.Form["CapacidadDisco"]);
                    asset.TipoDisco = Convert.ToInt32(Request.Form["TipoDisco"]);
                }
                dbc.ASSETs.Add(asset);
                dbc.SaveChanges();

                var CLIENTASSET = new CLIENT_ASSET();
                int ID_AREA = 0;

                try
                {   //Obteniendo el area por defecto para los nuevos activos : 31 'AREA BY DEFAULT ASSET'
                    ID_AREA = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 31)
                                    .VAL_ACCO_PARA);
                    CLIENTASSET.ID_AREA = ID_AREA;
                }
                catch { }

                CLIENTASSET.ID_PERS_ENTI = ID_PER_ENTI;
                CLIENTASSET.ID_ASSE = asset.ID_ASSE;
                CLIENTASSET.ID_AREA = ID_AREA;
                CLIENTASSET.ID_COND = 9;
                CLIENTASSET.ID_LOCA = ID_LOCA;
                CLIENTASSET.DAT_STAR = DateTime.Now;
                CLIENTASSET.CREATE_DATE = DateTime.Now;
                CLIENTASSET.UserId = ID_USER;
                CLIENTASSET.ID_TYPE_CLIE_ASSE = 6;

                dbc.CLIENT_ASSET.Add(CLIENTASSET);
                dbc.SaveChanges();

                if (archivos != null)
                {
                    foreach (var file in archivos)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_ASSE = asset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            //db.AddToATTACHEDs(ATTA);
                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                string usuario = Convert.ToString(Session["NAM_PERS"]).ToUpper();

                var ticket = new TICKET();
                string Code = null;
                try
                {
                    ticket.ID_ACCO = ID_ACCO;
                    ticket.ID_TYPE_TICK = 2;
                    ticket.ID_PERS_ENTI = ID_PER_ENTI;
                    ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
                    ticket.ID_PERS_ENTI_ASSI = ID_ASSI;
                    ticket.ID_AREA = CLIENTASSET.ID_AREA;
                    ticket.ID_QUEU = ID_QUEU;
                    ticket.ID_PRIO = ID_PRIO;
                    ticket.ID_STAT = 1;
                    ticket.ID_STAT_END = 4;
                    ticket.ID_ASSE = asset.ID_ASSE;
                    ticket.ID_SOUR = 4;
                    ticket.FEC_TICK = asset.CREATE_ASSE;
                    ticket.SUM_TICK = "TICKET CREATED BY " + usuario + " TO REGISTER TECHNOLOGICAL DEVICE IN THE INVENTORY";
                    ticket.REM_CTRL_TICK = false;
                    ticket.UserId = ID_USER;
                    ticket.CREATE_TICK = DateTime.Now;
                    ticket.FEC_INI_TICK = ticket.CREATE_TICK;
                    ticket.MODIFIED_TICK = DateTime.Now;
                    ticket.IS_PARENT = false;
                    ticket.ID_CATE = 181;
                    ticket.ID_TYPE_FORM = 3;
                    ticket.IdSLA = 1; //Asignación SLA ESTANDAR

                    dbc.TICKETs.Add(ticket);
                    dbc.SaveChanges();

                    int id = Convert.ToInt32(ticket.ID_TICK);

                    dbc.Entry(ticket).State = EntityState.Detached;

                    Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

                    var asseForm = dbc.ASSETs.Single(x => x.ID_ASSE == asset.ID_ASSE);
                    asseForm.ID_TICK = id;
                    dbc.ASSETs.Attach(asseForm);
                    dbc.Entry(asseForm).State = EntityState.Modified;
                    dbc.SaveChanges();

                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                }
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + Code + "', '" + asset.ID_ASSE.ToString() + "');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }

        // POST: /Asset/CreateHudbay
        [HttpPost, ValidateInput(false)]
        public ActionResult CreateHudbay(IEnumerable<HttpPostedFileBase> archivos, ASSET asset)
        {
            int ID_ACCO = 0;
            int ID_USER = 0;
            try
            {
                ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                ID_USER = Convert.ToInt32(Session["UserId"].ToString());
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
            }


            int ID_PER_ENTI, ID_COND = 0, ID_LOCA = 0, ID_QUEU, ID_PRIO = 4, ID_ASSI = 0, ID_TYPE_ASSE, ID_MANU, ID_BUY_MODE, ID_COST_CENT = 0;
            DateTime ACQ_DATE;
            string mensajeE = "";
            string COD_ASSE = Convert.ToString(Request.Form["COD_ASSE"]);
            string SER_NUMB = Convert.ToString(Request.Form["SER_NUMB"]);
            string SUM_ASSE = Convert.ToString(Request.Form["SUM_ASSE"]);
            string NAM_EQUI = Convert.ToString(Request.Form["NAM_EQUI"]);
            //Validaciones
            if (Request.Form["ID_TYPE_ASSE"] == "" || Request.Form["ID_TYPE_ASSE"] == null)
                mensajeE = mensajeE + "Seleccione el tipo de activo. <br/>";
            if (COD_ASSE == "")
                mensajeE = mensajeE + "Ingrese el código de activo. <br/>";
            if (SER_NUMB == "")
                mensajeE = mensajeE + "Ingrese el número de serie. <br/>";
            if (NAM_EQUI == "")
                mensajeE = mensajeE + "Ingrese el nombre del equipo. <br/>";
            if (Request.Form["ID_MANU"] == "" || Request.Form["ID_MANU"] == null)
                mensajeE = mensajeE + "Seleccione la marca. <br/>";
            if (Request.Form["ID_LOCA"] == "" || Request.Form["ID_LOCA"] == null)
                mensajeE = mensajeE + "Seleccione la locación. <br/>";
            else
                ID_LOCA = Convert.ToInt32(Request.Form["ID_LOCA"]);

            try
            {
                int cantAsset = dbc.ASSETs.Where(x => (x.COD_ASSE.ToLower() == COD_ASSE.ToLower() || x.SER_NUMB == SER_NUMB) && x.ID_ACCO == ID_ACCO).Count();
                if (cantAsset > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','3');}window.onload=init;</script>");
                }
            }
            catch
            {

            }

            if (mensajeE != "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','" + mensajeE + "',0);}window.onload=init;</script>");
            }

            SUM_ASSE = SUM_ASSE.Replace("&nbsp;", "");
            SUM_ASSE = SUM_ASSE.Replace("<br />", "");
            //if (SUM_ASSE.Trim().Length == 0)
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            //}

            try
            {
                ID_QUEU = Convert.ToInt32(Session["ID_QUEU"].ToString());
                ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                ID_QUEU = 0;
            }

            try
            {
                int cantAsset = dbc.ASSETs.Where(x => (x.COD_ASSE.ToLower() == COD_ASSE.ToLower() || x.SER_NUMB == SER_NUMB) && x.ID_ACCO == ID_ACCO).Count();
                if (cantAsset > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','3',0);}window.onload=init;</script>");
                }
            }
            catch
            {

            }

            if (ModelState.IsValid)
            {
                var query = dbc.PARAMETERs.Where(x => x.NAM_PARA == "RESPONSIBLE ASSET")
                    .Join(dbc.ACCOUNT_PARAMETER, x => x.ID_PARA, p => p.ID_PARA, (x, p) => new
                    {
                        p.VAL_ACCO_PARA,
                        p.ID_ACCO
                    }).Where(x => x.ID_ACCO == ID_ACCO).First();

                ID_PER_ENTI = Convert.ToInt32(query.VAL_ACCO_PARA);
                ID_PRIO = Convert.ToInt32(dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_PER_ENTI).ID_PRIO);
                int idGrupoIT = Convert.ToInt32(Request.Form["IdGrupoIT"]);

                asset.CREATE_DATE = DateTime.Now;
                asset.CREATE_ASSE = DateTime.Now;
                asset.ID_ACCO = ID_ACCO;
                asset.UserId = ID_USER;
                asset.IdGrupo = idGrupoIT;
                dbc.ASSETs.Add(asset);
                dbc.SaveChanges();

                var CLIENTASSET = new CLIENT_ASSET();
                int ID_AREA = 0;

                try
                {   //Obteniendo el area por defecto para los nuevos activos : 31 'AREA BY DEFAULT ASSET'
                    ID_AREA = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 31)
                                    .VAL_ACCO_PARA);
                    CLIENTASSET.ID_AREA = ID_AREA;
                }
                catch { }

                CLIENTASSET.ID_PERS_ENTI = ID_PER_ENTI;
                CLIENTASSET.ID_ASSE = asset.ID_ASSE;
                CLIENTASSET.ID_AREA = ID_AREA;
                CLIENTASSET.ID_COND = 9;
                CLIENTASSET.ID_LOCA = ID_LOCA;
                CLIENTASSET.DAT_STAR = DateTime.Now;
                CLIENTASSET.CREATE_DATE = DateTime.Now;
                CLIENTASSET.UserId = ID_USER;
                CLIENTASSET.ID_TYPE_CLIE_ASSE = 6;

                dbc.CLIENT_ASSET.Add(CLIENTASSET);
                dbc.SaveChanges();

                if (archivos != null)
                {
                    foreach (var file in archivos)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_ASSE = asset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            //db.AddToATTACHEDs(ATTA);
                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                string usuario = Convert.ToString(Session["NAM_PERS"]).ToUpper();

                //var ticket = new TICKET();
                string Code = null;
                try
                {
                    var ticket = dbc.InsertarTicketActivoHudbay(ID_ACCO, ID_PER_ENTI, ID_ASSI, ID_AREA, ID_QUEU, ID_PRIO,
                    asset.ID_ASSE, asset.CREATE_ASSE, "TICKET CREATED BY " + usuario + " TO REGISTER TECHNOLOGICAL DEVICE IN THE INVENTORY",
                    ID_USER, idGrupoIT).Single();

                    int id = Convert.ToInt32(ticket.ID_TICK);

                    Code = ticket.COD_TICK;

                    var asseForm = dbc.ASSETs.Single(x => x.ID_ASSE == asset.ID_ASSE);
                    asseForm.ID_TICK = id;
                    dbc.ASSETs.Attach(asseForm);
                    dbc.Entry(asseForm).State = EntityState.Modified;
                    dbc.SaveChanges();

                }
                catch (Exception e)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + Code + "', '" + asset.ID_ASSE.ToString() + "');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }

        // GET: /Asset/Edit/5
        public ActionResult Edit(int id = 0)
        {
            ASSET asset = dbc.ASSETs.Single(a => a.ID_ASSE == id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            ViewBag.Adjuntos = Adjuntos(id);
            return View(asset);
        }

        public ActionResult EditPartial(int id = 0)
        {
            ASSET asset = dbc.ASSETs.Single(a => a.ID_ASSE == id);
            if (asset == null)
            {
                return HttpNotFound();
            }

            CLIENT_ASSET client_asset = dbc.CLIENT_ASSET.Single(b => b.ID_ASSE == id && b.DAT_END == null);

            ViewBag.ACQ_DATE = String.Format("{0:g}", asset.ACQ_DATE);
            ViewBag.LOCACION = client_asset.ID_LOCA;
            return PartialView(asset);
        }

        public string Adjuntos(int id)
        {
            string Adjun = "";
            try
            {
                var query = dbc.ATTACHEDs.Where(a => a.ID_ASSE == id);
                foreach (ATTACHED subqu in query)
                {
                    Adjun = Adjun + "<li><a href='/Adjuntos/Asset/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        //
        // POST: /Asset/Edit/5
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(IEnumerable<HttpPostedFileBase> files, ASSET asset)
        {
            if (ModelState.IsValid)
            {
                var objActivo = dbc.ASSETs.Where(x => x.SER_NUMB == asset.SER_NUMB);
                    
                if (objActivo == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR');}window.onload=init;</script>");
                }

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                int cantAsset = dbc.ASSETs.Where(x => (x.COD_ASSE.ToLower() == asset.COD_ASSE.ToLower() || x.SER_NUMB == asset.SER_NUMB) && x.ID_ACCO == ID_ACCO && x.ID_ASSE != asset.ID_ASSE).Count();
                if (cantAsset > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','El código de activo o la serie ingresada ya existen. Por favor validar.');}window.onload=init;</script>");
                }

                if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                {
                    asset.ACQ_DATE = (string.IsNullOrEmpty(Request.Form["FechaAdquisicion"])) ? (DateTime?)null : Convert.ToDateTime(Request.Form["FechaAdquisicion"]);
                }

                asset.MODIFIED_DATE = DateTime.Now;
                dbc.ASSETs.Attach(asset);
                //db.ObjectStateManager.ChangeObjectState(asset, EntityState.Modified);                
                dbc.Entry(asset).State = EntityState.Modified;
                dbc.SaveChanges();

                var objClientAsset = dbc.CLIENT_ASSET.Where(x => x.ID_ASSE == asset.ID_ASSE && x.DAT_END == null).FirstOrDefault();

                if (objClientAsset != null)
                {
                    objClientAsset.UserId = Convert.ToInt32(Session["UserId"]);
                    objClientAsset.DAT_END = DateTime.Now;
                    objClientAsset.CREATE_DATE = DateTime.Now;
                    objClientAsset.ID_TYPE_CLIE_ASSE = 5;
                    objClientAsset.SUM_CLIE_ASSE = asset.SUM_ASSE;
                    dbc.CLIENT_ASSET.Add(objClientAsset);
                    dbc.SaveChanges();
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_CLIE_ASSE = objClientAsset.ID_CLIE_ASSE;
                            ATTA.ID_ASSE = objClientAsset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR');}window.onload=init;</script>");
        }

        [HttpPost]
        public void EditContratista(int idAsset, bool PorContratista, string DatosContratista = "")
        {
            CLIENT_ASSET objClientAsset = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == idAsset && x.DAT_END == null);
            CLIENT_ASSET objClientAssetNuevo = dbc.CLIENT_ASSET.OrderByDescending(x => x.CREATE_DATE).First(x => x.ID_ASSE == idAsset);
            if (PorContratista == false)
            {
                objClientAsset.PorContratista = null;
                objClientAssetNuevo.PorContratista = null;
                objClientAsset.UsuarioContratista = null;
                objClientAsset.EmpresaContratista = null;
                objClientAsset.AreaContratista = null;
                objClientAssetNuevo.UsuarioContratista = null;
                objClientAssetNuevo.EmpresaContratista = null;
                objClientAssetNuevo.AreaContratista = null;
            }
            else
            {   objClientAsset.PorContratista = true;
                objClientAssetNuevo.PorContratista = true;
                objClientAsset.UsuarioContratista = DatosContratista.Split(',')[0];
                objClientAsset.EmpresaContratista = DatosContratista.Split(',')[1];
                objClientAsset.AreaContratista = DatosContratista.Split(',')[2];
                objClientAssetNuevo.UsuarioContratista = DatosContratista.Split(',')[0];
                objClientAssetNuevo.EmpresaContratista = DatosContratista.Split(',')[1];
                objClientAssetNuevo.AreaContratista = DatosContratista.Split(',')[2];
            }
            dbc.SaveChanges();
        }
        
        //
        // POST: /Asset/EditHudbay/5
        [HttpPost, ValidateInput(false)]
        public ActionResult EditHudbay(IEnumerable<HttpPostedFileBase> files, ASSET asset)
        {
            if (ModelState.IsValid)
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                string mensajeE = "";
                var objActivo = dbc.ASSETs.Where(x => x.SER_NUMB == asset.SER_NUMB);

                if (objActivo == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR');}window.onload=init;</script>");
                }
                int cantAsset = dbc.ASSETs.Where(x => (x.COD_ASSE.ToLower() == asset.COD_ASSE.ToLower() || x.SER_NUMB == asset.SER_NUMB) && x.ID_ACCO == ID_ACCO && x.ID_ASSE != asset.ID_ASSE).Count();
                if (cantAsset > 0)
                {
                    mensajeE = mensajeE + "El código de activo o la serie ingresada ya existen. Por favor validar.";
                }

                if (asset.ID_TYPE_ASSE == 0 || asset.ID_TYPE_ASSE == null)
                    mensajeE = mensajeE + "Seleccione el tipo de activo. <br/>";
                if (asset.COD_ASSE == "" || asset.COD_ASSE == null)
                    mensajeE = mensajeE + "Ingrese el código de activo. <br/>";
                if (asset.SER_NUMB == "" || asset.SER_NUMB == null)
                    mensajeE = mensajeE + "Ingrese el número de serie. <br/>";
                if (asset.NAM_EQUI == "" || asset.NAM_EQUI == null)
                    mensajeE = mensajeE + "Ingrese el nombre del equipo. <br/>";

                if (mensajeE != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','" + mensajeE + "');}window.onload=init;</script>");
                }

                asset.MODIFIED_DATE = DateTime.Now;
                dbc.ASSETs.Attach(asset);
                //db.ObjectStateManager.ChangeObjectState(asset, EntityState.Modified);                
                dbc.Entry(asset).State = EntityState.Modified;
                dbc.SaveChanges();

                var objClientAsset = dbc.CLIENT_ASSET.Where(x => x.ID_ASSE == asset.ID_ASSE && x.DAT_END == null).FirstOrDefault();

                if (objClientAsset != null)
                {
                    objClientAsset.UserId = Convert.ToInt32(Session["UserId"]);
                    objClientAsset.DAT_END = DateTime.Now;
                    objClientAsset.CREATE_DATE = DateTime.Now;
                    objClientAsset.ID_TYPE_CLIE_ASSE = 5;
                    objClientAsset.SUM_CLIE_ASSE = asset.SUM_ASSE;
                    dbc.CLIENT_ASSET.Add(objClientAsset);
                    dbc.SaveChanges();
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_CLIE_ASSE = objClientAsset.ID_CLIE_ASSE;
                            ATTA.ID_ASSE = objClientAsset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR');}window.onload=init;</script>");
        }

        public ActionResult IndexByIdJson(int id = 0)
        {
            var query1 = dbc.ASSETs.Where(a => a.ID_ASSE == id);
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query2 = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO)
                         .Join(dbe.PERSON_ENTITY, x => x.ID_PERS_ENTI, pe => pe.ID_PERS_ENTI, (x, pe) => new
                         {
                             x.ID_PERS_ENTI,
                             pe.ID_ENTI2,
                             pe.ID_AREA,
                             pe.ID_FOTO,
                         })
                         .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_ENTI2,
                             x.ID_AREA,
                             x.ID_FOTO,
                             ce.ID_ENTI,
                             ce.FIR_NAME,
                             ce.LAS_NAME,
                             ce.UserId
                         })
                         .Join(dbe.AREAs, x => x.ID_AREA, a => a.ID_AREA, (x, a) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_ENTI2,
                             x.ID_AREA,
                             x.ID_ENTI,
                             x.FIR_NAME,
                             x.LAS_NAME,
                             x.UserId,
                             x.ID_FOTO,
                             a.NOM_AREA
                         });

            var result = (from a in query1.ToList()
                          join ca in dbc.CLIENT_ASSET on a.ID_ASSE equals ca.ID_ASSE
                          where ca.DAT_END == null
                          join c in query2 on ca.ID_PERS_ENTI equals c.ID_PERS_ENTI
                          join ta in dbc.TYPE_ASSET on a.ID_TYPE_ASSE equals ta.ID_TYPE_ASSE
                          join m in dbc.MANUFACTURERs on a.ID_MANU equals m.ID_MANU
                          join cm in dbc.COMMERCIAL_MODEL on a.ID_COMM_MODE equals cm.ID_COMM_MODE into lcm
                          from xcm in lcm.DefaultIfEmpty()
                          join mm in dbc.MANUFACTURER_MODEL on a.ID_MANU_MODE equals mm.ID_MANU_MODE into lmm
                          from x in lmm.DefaultIfEmpty()
                          join l in dbc.LOCATIONs on ca.ID_LOCA equals l.ID_LOCA
                          join s in dbc.SITEs on l.ID_SITE equals s.ID_SITE
                          join cd in dbc.CONDITIONs on ca.ID_COND equals cd.ID_COND
                          join st in dbc.STATUS_ASSET on cd.ID_STAT_ASSE equals st.ID_STAT_ASSE
                          join u in query2 on a.UserId equals u.UserId
                          join bm in dbc.BUY_MODE on a.ID_BUY_MODE equals bm.ID_BUY_MODE into lbm
                          from bmx in lbm.DefaultIfEmpty()
                          join cc in dbc.COST_CENTER on a.ID_COST_CENT equals cc.ID_COST_CENT into lcc
                          from z in lcc.DefaultIfEmpty()
                          join sota in dbc.SYSTEM_OPER_TYPE_ASSE on a.ID_SYST_OPER_TYPE_ASSE equals sota.ID_SYST_OPER_TYPE_ASSE into lsota
                          from xsota in lsota.DefaultIfEmpty()
                          join so in dbc.SYSTEM_OPERATING on (xsota == null ? 0 : xsota.ID_SYST_OPER) equals so.ID_SYST_OPER into lso
                          from xso in lso.DefaultIfEmpty()
                              //join ram in dbc.SUBTYPE_ASSET on a.ID_SUBTYPE1 equals ram.ID_SUBT_ASSE into lram
                              //from xram in lram.DefaultIfEmpty()
                              //join proc in dbc.SUBTYPE_ASSET on a.ID_SUBTYPE2 equals proc.ID_SUBT_ASSE into lproc
                              //from xproc in lproc.DefaultIfEmpty()
                              //join hd in dbc.SUBTYPE_ASSET on a.ID_SUBTYPE3 equals hd.ID_SUBT_ASSE into lhd
                              //from xhd in lhd.DefaultIfEmpty()
                              //join par in db.ACCOUNT_PARAMETER on a.idCompaniaTelefono equals par.ID_ACCO_PARA into lpar
                              //from xpar in lpar.DefaultIfEmpty()
                          join cont in dbc.Contratoes on a.IdContrato equals cont.Id into lcont
                          from xcont in lcont.DefaultIfEmpty()
                          select new
                          {
                              COD_ASSE = (a.COD_ASSE == null ? "-" : a.COD_ASSE),
                              ACQ_DATE = (a.ACQ_DATE == null ? "-" : String.Format("{0:MM/dd/yyyy}", a.ACQ_DATE)),
                              ASSI = c.FIR_NAME.ToLower() + " " + c.LAS_NAME.ToLower(),
                              NAM_TYPE_ASSE = ta.NAM_TYPE_ASSE.ToLower(),
                              NAM_AREA = (c == null ? "-" : c.NOM_AREA.ToLower()),
                              NAM_MANU = m.NAM_MANU.ToLower(),
                              NAM_COMM_MODE = (xcm == null ? "-" : xcm.NAM_COMM_MODE.ToLower()),
                              NAM_MANU_MODE = (x == null ? "-" : x.NAM_MANU_MODE),
                              SER_NUMB = (a.SER_NUMB == null ? "-" : a.SER_NUMB),
                              SOLPED = (a.SOLPED == null ? "-" : a.SOLPED),
                              NAM_LOCA = l.NAM_LOCA.ToLower(),
                              NAM_SITE = s.NAM_SITE.ToLower(),
                              NAM_EQUI = (a.NAM_EQUI == null ? "-" : a.NAM_EQUI),
                              NAM_COND = cd.NAM_COND.ToLower(),
                              NAM_STAT_ASSE = st.NAM_STAT_ASSE.ToLower(),
                              SUMM = (a.SUM_ASSE == null ? "" : a.SUM_ASSE),
                              CREATE = String.Format("{0:G}", a.CREATE_ASSE),
                              UPDATE = String.Format("{0:G}", ca.CREATE_DATE),
                              USER = u.FIR_NAME.ToLower() + " " + u.LAS_NAME.ToLower(),
                              CECO = (z == null ? "-" : z.COD_COST_CENT),
                              NAM_BUY_MODE = (bmx == null ? "-" : bmx.NAM_BUY_MODE.ToLower()),
                              COST = (a.COST == null ? "-" : String.Format("{0:0,0.00}", a.COST)),
                              LOT = (a.LOTE == null ? "-" : Convert.ToString(a.LOTE)),
                              ADJUNTOS = Adjuntos(id),
                              FOTO = Convert.ToString(u.ID_FOTO) + ".jpg",
                              MAC_ADRR = (a.MAC_ADRR == null ? "-" : a.MAC_ADRR),
                              SO = (xso == null ? "-" : xso.NAM_SYST_OPER),
                              //NAM_RAM = (xram == null ? "-" : xram.NAM_SUBT_ASSE),
                              //NAM_PROC = (xproc == null ? "-" : xproc.NAM_SUBT_ASSE),
                              //NAM_HD = (xhd == null ? "-" : xhd.NAM_SUBT_ASSE),
                              VALORACTIVO = (a.ValorActivo == null ? 0 : a.ValorActivo),
                              RIESGOINTR = (a.RiesgoIntrinseco == null ? 0 : a.RiesgoIntrinseco),
                              FEC_INI_PRES = (a.DAT_STAR_LEAS == null ? "-" : String.Format("{0:G}", a.DAT_STAR_LEAS)),
                              FEC_END_PRES = (a.DAT_END_LEAS == null ? "-" : String.Format("{0:G}", a.DAT_END_LEAS)),
                              GARANTIA = (a.fechaFinGarantia == null ? "-" : String.Format("{0:G}", a.fechaFinGarantia)),
                              IpLocal = (a.IpLocal == null ? "-" : a.IpLocal),
                              IpPublica = (a.IpPublica == null ? "-" : a.IpPublica),
                              Contrato = (a.IdContrato == null ? "-" : xcont.Nombre)
                          });

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TreeConfiguration(int id)
        {
            var query = dbc.CONFIGURATIONs.Where(c => c.VIG_CONF == true && c.ID_ASSE == id);

            try
            {
                int ID_CONF = Convert.ToInt32(Request.Params["ID_CONF"]);

                if (ID_CONF == 0)
                {
                    query = query.Where(c => c.ID_CONF_PARE == null);
                }
                else
                {
                    query = query.Where(c => c.ID_CONF_PARE == ID_CONF);
                }
            }
            catch
            {
                query = query.Where(c => c.ID_CONF_PARE == null);
            }

            var result = (from c in query.ToList()
                          join pc in dbc.PARAMETER_CONFIGURATION on c.ID_PARA_CONF equals pc.ID_PARA_CONF
                          select new
                          {
                              id = c.ID_CONF,
                              c.ID_CONF,
                              pc.NAM_PARA_CONF,
                              HAS_VALUE = (pc.HAS_VALUE == true ? false : true),
                              VAL_CONF = c.VAL_CONF + (pc.UND_PARA_CONF == null ? "" : " " + pc.UND_PARA_CONF),
                              expanded = pc.expanded,
                          }).OrderBy(x => x.NAM_PARA_CONF);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /Asset/Delete/5
        public ActionResult Delete(int id = 0)
        {
            ASSET asset = dbc.ASSETs.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        // POST: /Asset/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ASSET asset = dbc.ASSETs.Find(id);
            dbc.ASSETs.Remove(asset);
            dbc.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            dbc.Dispose();
            base.Dispose(disposing);
        }

        [Authorize]
        public ActionResult viewAssetReport()
        {
            return View();
        }

        public ActionResult viewAssetsByUserReport()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.IdAcco = ID_ACCO;
            return View();
        }

        public ActionResult viewHistoryAssetReport()
        {
            if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 0)
                return RedirectToAction("Index", "Error");

            return View();
        }

        public ActionResult ReportAssetPrograma()
        {
            return View();
        }

        public ActionResult viewAssetByProgram()
        {
            return View();
        }

        public ActionResult viewAssetByComponent()
        {
            return View();
        }

        public ActionResult viewAssetByReportHistorical()
        {
            return View();
        }

        public ActionResult viewAssetByArea()
        {
            return View();
        }
        public ActionResult viewAssetByMovil()
        {
            return View();
        }
        /**/
        public ActionResult viewRptTransferenciaActivos_CambioGuardia()
        {
            return View();
        }

        public ActionResult ListAssetByAcco()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var qA = (from a in dbc.ASSETs.Where(X => X.ID_ACCO == ID_ACCO)
                      join b in dbc.TYPE_ASSET on a.ID_TYPE_ASSE equals b.ID_TYPE_ASSE
                      select new
                      {
                          a.ID_ASSE,
                          a.COD_ASSE,
                          b.NAM_TYPE_ASSE,
                      });
            return Json(new { Data = qA, Count = qA.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssetTicket(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = (from df in dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == id)
                         join t in dbc.TICKETs.Where(x => x.ID_STAT_END != 2) on df.ID_TICK equals t.ID_TICK
                         join tf in dbc.TYPE_FORMAT on t.ID_TYPE_FORM equals tf.ID_TYPE_FORM
                         join dt in dbc.DETAILS_TICKET.Where(x => x.ID_TYPE_DETA_TICK == 7) on t.ID_TICK equals dt.ID_TICK into dtx
                         from x in dtx.DefaultIfEmpty()
                             //join atf in db.ATTACHED_TICKET_FORMAT on x.ID_DETA_TICK equals atf.ID_DETA_TICK into atx
                             //from ax in atx.DefaultIfEmpty()
                         select new
                         {
                             ID_TICK = t.ID_TICK,
                             COD_TICK = t.COD_TICK,
                             CREATE_TICK = t.CREATE_TICK,
                             NAM_TYPE_FORM = tf.NAM_TYPE_FORM,
                             ID_PERS_ENTI = t.ID_PERS_ENTI,
                             ID_DETA_TICK = x.ID_DETA_TICK == null ? 0 : x.ID_DETA_TICK
                         }).ToList();

            var result = (from q in query
                          join pe in dbe.PERSON_ENTITY on q.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          select new
                          {
                              q.ID_TICK,
                              q.COD_TICK,
                              CREATE_TICK = String.Format("{0:d}", q.CREATE_TICK),
                              q.NAM_TYPE_FORM,
                              ce.FIR_NAME,
                              ce.LAS_NAME,
                              ADJUNTOS = AdjuntosDetailsTicket(q.ID_DETA_TICK, q.ID_TICK)
                          }).Where(x => x.ADJUNTOS != "").OrderByDescending(x => x.ID_TICK);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public string AdjuntosDetailsTicket(int id, int IdTicket)
        {
            int id_TF = dbc.TICKETs.Single(x => x.ID_TICK == IdTicket).ID_TYPE_FORM.Value;

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string ruta = "Delivery";
            if (id_TF == 2) { ruta = "Reception"; }
            string Adjun = "";
            try
            {
                if (id == 0)
                {
                    Adjun = "Sin archivos adjuntos";
                }
                else
                {
                    var query = dbc.ATTACHED_TICKET_FORMAT.Where(a => a.ID_DETA_TICK == id);
                    foreach (ATTACHED_TICKET_FORMAT atta in query)
                    {
                        Adjun = Adjun + "<li><a href='/Adjuntos/" + ruta + "/" + ID_ACCO.ToString() + "/" + atta.NAM_ATTA + "_" + atta.ID_ATTA_TICK_FORM + atta.EXT_ATTA + " ' TARGET='_BLANK'>" + atta.NAM_ATTA + atta.EXT_ATTA + "<img src='/Content/Images/pdf.png' style='width:14px; height:14px; border:none;'></a></li>";
                    }
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        [Authorize]
        public ActionResult Reportes()
        {
            if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 0)
                return RedirectToAction("Index", "Error");

            return View();
        }

        public ActionResult Ticket()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (ID_ACCO == 60)
            {
                int idGrupoActivo = ObtenerIdGrupoUsuarioBNV();

                if (idGrupoActivo != 0)
                {
                    ViewBag.IdGrupo = idGrupoActivo;
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            else if (ID_ACCO == 55)
            {
                if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 1)
                {
                    int idGrupoIT = ObtenerIdGrupo("IT");
                    ViewBag.IdGrupoIT = idGrupoIT;
                }
                else if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
                {
                    return RedirectToAction("TicketOT", "Asset");
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }

            ViewBag.Id_Acco = ID_ACCO;
            return View();
        }

        public ActionResult ExportaPrograma()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = (from a in dbc.ASSETs.Where(x => x.ID_ACCO == ID_ACCO)
                          join ta in dbc.TYPE_ASSET on a.ID_TYPE_ASSE equals ta.ID_TYPE_ASSE
                          join ap in dbc.ActivoProgramas on a.ID_ASSE equals ap.IdActivo
                          join p in dbc.Programas on ap.IdPrograma equals p.Id
                          select new
                          {
                              CODGIO_ACTIVO = a.COD_ASSE,
                              TIPO_ACTIVO = ta.NAM_TYPE_ASSE,
                              NOMBRE_EQUIPO = a.NAM_EQUI,
                              NUMERO_SERIE = a.SER_NUMB,
                              PROGRAMA = p.Nombre
                          });

            var grid = new GridView();
            grid.DataSource = result.ToList();
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ActivoSoftware" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        // GET: /Ticket/ListByStatus Cargar Grilla
        public ActionResult ListByStatusTicket(int IdResponsable, int IdUsuarioAfectado, string PalabraClave, int IdGrupo)
        {
            textInfo = cultureInfo.TextInfo;
            ctd = 1;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME),
                                ce.UserId,
                            }).ToList();

            var listCIA = (from pe in dbe.PERSON_ENTITY
                           join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               pe.ID_PERS_ENTI,
                               COMPANY = ce.COM_NAME,
                           }).ToList();

            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO);

            if (ID_ACCO == 60 || ID_ACCO == 55)
            {
                tick = tick.Where(x => x.IdGrupoActivo == IdGrupo);
            }

            //Diferenciando activos - soporte
            tick = tick.Where(x => x.ID_TYPE_FORM != null);

            int Count = 0;
            int[] numbers = new int[0];
            if (ID_STAT == 0)
            {
                tick = tick.Where(t => (t.ID_STAT_END == 1 || t.ID_STAT_END == 3 || t.ID_STAT_END == 5) && t.ID_TYPE_FORM == 1);
            }
            else if (ID_STAT == 1)
            {
                tick = tick.Where(i => (i.ID_STAT_END == 4 || i.ID_STAT_END == 6) && i.ID_TYPE_FORM == 1);
            }
            else if (ID_STAT == 2)
            {
                tick = tick.Where(i => (i.ID_STAT_END == 1 || i.ID_STAT_END == 3 || i.ID_STAT_END == 5) && i.ID_TYPE_FORM == 2);
            }
            else if (ID_STAT == 3)
            {
                tick = tick.Where(i => (i.ID_STAT_END == 4 || i.ID_STAT_END == 6) && i.ID_TYPE_FORM == 2);
            }

            if (IdResponsable != 0)
            {
                tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == IdResponsable);
            }
            if (IdUsuarioAfectado != 0)
            {
                tick = tick.Where(i => i.ID_PERS_ENTI == IdUsuarioAfectado);
            }
            if (PalabraClave != "")
            {
                tick = tick.Where(i => i.SUM_TICK.ToLower().Contains(PalabraClave.ToLower()));
            }

            Count = tick.Count();
            tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);

            var result = (from i in tick.ToList()
                          join so in dbc.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in dbc.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in dbc.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in dbc.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in dbc.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in dbc.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in dbc.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE
                          join pr in dbc.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listuser on i.UserId equals cr.UserId
                          join ac in dbc.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listuser on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                          join ds in dbc.DOCUMENT_SALE on (i.ID_DOCU_SALE == null ? 0 : i.ID_DOCU_SALE) equals ds.ID_DOCU_SALE into lds
                          from xds in lds.DefaultIfEmpty()
                          join tds in dbc.TYPE_DOCUMENT_SALE on (xds == null ? 0 : xds.ID_TYPE_DOCU_SALE) equals tds.ID_TYPE_DOCU_SALE into ltds
                          from xtds in ltds.DefaultIfEmpty()
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              SUM_INCI_PLAIN = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT,
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR,
                              NAM_SUBCLAS = c4.NAM_CATE,
                              NAM_CLAS = c3.NAM_CATE,
                              NAM_SUBC = c2.NAM_CATE,
                              NAM_CATE = c1.ABR_CATE,
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK,
                              URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                              REQU = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                              CREATE_INCI = String.Format("{0:d}", i.CREATE_TICK) + " " + String.Format("{0:HH:mm}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:d}", i.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO,
                              HOU_PRIO = ObtenerHoraSLA(i.ID_TICK) != 0 ? Convert.ToString(ObtenerHoraSLA(i.ID_TICK)) + "h" : "",
                              //HOU_PRIO = pr.HOU_PRIO != 0 ? pr.HOU_PRIO + "h" : "",
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client,
                              ACCO = ac.NAM_ACCO,
                              ASSI = asi.Client,
                              EXP_TIME = TiempoTranscurrido(i.CREATE_TICK),//ExpTime(i.ID_TICK),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none"),
                              COUNTSON = 0,
                              CIA = textInfo.ToTitleCase(cia.COMPANY.ToLower()).Substring(0, (cia.COMPANY.Length > 48 ? 48 : cia.COMPANY.Length)) +
                                    (cia.COMPANY.Length > 48 ? "..." : ""),
                              CIA_TOOL = textInfo.ToTitleCase(cia.COMPANY.ToLower()),
                              ID_ACCO,
                              VIS_COMP = ac.VIS_COMP,
                              i.ID_STAT_END,
                              NUM_OP = (xds == null ? "" : xds.NUM_DOCU_SALE),
                              COD_TYPE_DOCU_SALE = (xtds == null ? "" : xtds.COD_TYPE_DOCU_SALE),
                              ID_DOCU_SALE = (xds == null ? 0 : xds.ID_DOCU_SALE),
                              //DATE_INI = String.Format("{0:MMM d yyyy HH:mm:ss}", Convert.ToDateTime(i.CREATE_TICK).AddHours(Convert.ToInt32(pr.HOU_PRIO))),
                              Seque = contador(),
                              DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_TICK), Convert.ToDateTime(i.FEC_INI_TICK), Convert.ToInt32(ObtenerHoraSLA(i.ID_TICK))),
                              DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : "",
                              HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : ""
                          });
            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        public string TiempoTranscurrido(DateTime? FechaCreacion)
        {
            int fm = DateTime.Now.Subtract(Convert.ToDateTime(FechaCreacion)).Minutes;

            int day = fm / 1440;
            int hour = (fm - day * 1440) / 60;
            int minutes = fm - day * 1440 - hour * 60;

            return day.ToString() + " dia(s) | " + hour.ToString() + " : " + minutes.ToString();
        }

        public string ReturnRequ(int idend = 0)
        {

            var listClient = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == idend)
                              join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                              select new
                              {
                                  Client = ce.FIR_NAME + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME) + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME),
                              });

            return listClient.First().Client;
        }

        public int contador()
        {
            return ctd++;
        }

        public string DateMaxima(int IdTick, DateTime DatIni, int HH)
        {
            string FecMax = "";
            Double MinTransc = 0;

            var qDet = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == IdTick //&& x.ID_STAT == 5
                );

            if (qDet.Where(x => x.ID_STAT == 5).Where(x => x.ID_TYPE_DETA_TICK == 3).Count() > 0)
            {
                MinTransc = qDet.AsEnumerable().Sum(x => x.MINUTES).Value;
                qDet = qDet.Where(x => x.ID_TYPE_DETA_TICK == 3).OrderByDescending(x => x.ID_DETA_TICK);

                DateTime fecSche = qDet.Where(x => x.ID_STAT == 5).First().FEC_SCHE.Value;
                Double hh = Convert.ToDouble(HH) - (MinTransc / 60);
                FecMax = String.Format("{0:MMM d yyyy HH:mm:ss}", fecSche.AddHours(hh));
            }
            else
            {
                FecMax = String.Format("{0:MMM d yyyy HH:mm:ss}", DatIni.AddHours(HH));
            }
            return FecMax;
        }

        public string ScheduleDate(int ID_TICK, DateTime FEC_INI_TICK)
        {
            try
            {
                var query = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                    .Where(x => x.ID_STAT == 5);

                if (query.Count() != 0)
                {
                    var xyz = query.OrderByDescending(x => x.ID_DETA_TICK).First().FEC_SCHE;
                    return String.Format("{0:d}", Convert.ToDateTime(xyz.Value));
                }
                else
                {
                    return String.Format("{0:d}", Convert.ToDateTime(FEC_INI_TICK));
                }
                //.OrderByDescending(x => x.ID_DETA_TICK)
                //.Where(x => x.ID_STAT == 5).First().FEC_SCHE;
            }
            catch
            {
                return "Stopped";
            }

        }

        public string ScheduleTime(int ID_TICK, DateTime FEC_INI_TICK)
        {
            try
            {
                var query = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                    .Where(x => x.ID_STAT == 5);

                if (query.Count() != 0)
                {
                    var xyz = query.OrderByDescending(x => x.ID_DETA_TICK).First().FEC_SCHE;
                    return String.Format("{0:HH:mm}", Convert.ToDateTime(xyz.Value));
                }
                else
                {
                    return String.Format("{0:HH:mm}", Convert.ToDateTime(FEC_INI_TICK));

                }
            }
            catch
            {
                return "Stopped";
            }

        }

        public ActionResult UpdateBarStatus(int id = 0)
        {
            int idResponsable = Convert.ToInt32(Request.QueryString["IdResponsable"]);
            int idUsuarioAfectado = Convert.ToInt32(Request.QueryString["IdUsuarioAfectado"]);
            string PalabraClave = Convert.ToString(Request.QueryString["PalabraClave"]);
            int idGrupo = Convert.ToInt32(Request.QueryString["IdGrupo"]);

            var TipoFormato = dbc.TYPE_FORMAT.Where(x => x.ID_TYPE_FORM != 3);

            var result = (from tf in TipoFormato.ToList()
                          select new
                          {
                              tf.ID_TYPE_FORM,
                              tf.NAM_TYPE_FORM,
                              TicketsP = TicketsByStatusx(tf.ID_TYPE_FORM, 0, idResponsable, idUsuarioAfectado, PalabraClave, idGrupo),
                              TotalP = TotTicketsByStatusx(tf.ID_TYPE_FORM, 0, idGrupo),
                              TicketsC = TicketsByStatusx(tf.ID_TYPE_FORM, 1, idResponsable, idUsuarioAfectado, PalabraClave, idGrupo),
                              TotalC = TotTicketsByStatusx(tf.ID_TYPE_FORM, 1, idGrupo)
                          }).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public int TicketsByStatusx(int tipoFormato, int estado, int idResponsable, int idUsuarioAfectado, string PalabraClave, int idGrupo)
        {
            int[] numbers = new int[0];
            int iq = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            bool VIS_ALL_QUEU = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

            var cant = dbc.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_TYPE_FORM == tipoFormato);

            if (ID_ACCO == 60 || ID_ACCO == 55)
            {
                cant = cant.Where(x => x.IdGrupoActivo == idGrupo);
            }

            //Pendientes
            if (estado == 0)
            {
                cant = cant.Where(x => x.ID_STAT_END == 1 || x.ID_STAT_END == 3 || x.ID_STAT_END == 5);
            }
            //Cerrados
            if (estado == 1)
            {
                cant = cant.Where(x => x.ID_STAT_END == 4 || x.ID_STAT_END == 6);
            }

            if (idResponsable != 0)
            {
                cant = cant.Where(i => i.ID_PERS_ENTI_ASSI == idResponsable);
            }
            if (idUsuarioAfectado != 0)
            {
                cant = cant.Where(i => i.ID_PERS_ENTI == idUsuarioAfectado);
            }
            if (PalabraClave != "")
            {
                cant = cant.Where(i => i.SUM_TICK.ToLower().Contains(PalabraClave.ToLower()));
            }

            return cant.Count();
        }

        public int TotTicketsByStatusx(int tipoFormato, int estado, int idGrupo)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var cant = dbc.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_TYPE_FORM == tipoFormato);

            if (ID_ACCO == 60 || ID_ACCO == 55)
            {
                cant = cant.Where(x => x.IdGrupoActivo == idGrupo);
            }

            if (estado == 0)
            {
                cant = cant.Where(x => x.ID_STAT_END == 1 || x.ID_STAT_END == 3);
            }
            if (estado == 1)
            {
                cant = cant.Where(x => x.ID_STAT_END == 4 || x.ID_STAT_END == 6);
            }

            return cant.Count();
        }

        public ActionResult reportePorPrograma()
        {
            try
            {
                int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
                int UserId = Convert.ToInt32(Session["UserId"]);

                int skip = Convert.ToInt32(Request.Params["skip"].ToString());
                int take = Convert.ToInt32(Request.Params["take"].ToString());

                IdTipoActivoPro = Convert.ToInt32(Request.Params["IdTipoActivo"].ToString());
                IdPrograma = Convert.ToInt32(Request.Params["IdPrograma"].ToString());
                IdCondicion = Convert.ToInt32(Request.Params["IdCondicion"].ToString());
                IdEstado = Convert.ToInt32(Request.Params["IdEstado"].ToString());

                var query = dbc.ActViewAssetByProgram(IdAcco, IdPrograma, IdTipoActivoPro, IdEstado, IdCondicion).ToList();

                var total = query.Count();

                var query2 = query.Skip(skip).Take(take);

                return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Content("<script>alert('ERROR');</script>");
            }
        }

        public ActionResult ReporteActivoPorPrograma()
        {
            try
            {
                int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
                int UserId = Convert.ToInt32(Session["UserId"]);

                IdTipoActivoPro = Convert.ToInt32(Request.Params["IdTipoActivo"].ToString());
                IdPrograma = Convert.ToInt32(Request.Params["IdPrograma"].ToString());
                IdCondicion = Convert.ToInt32(Request.Params["IdCondicion"].ToString());
                IdEstado = Convert.ToInt32(Request.Params["IdEstado"].ToString());

                var query = dbc.ActViewAssetByProgram(IdAcco, IdPrograma, IdTipoActivoPro, IdEstado, IdCondicion).ToList();

                return Json(new { data = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Content("<script>alert('ERROR');</script>");
            }
        }
        public ActionResult reportePorComponente()
        {
            try
            {
                int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
                int UserId = Convert.ToInt32(Session["UserId"]);

                int skip = Convert.ToInt32(Request.Params["skip"].ToString());
                int take = Convert.ToInt32(Request.Params["take"].ToString());

                IdTipoActivo = Convert.ToInt32(Request.Params["IdTipoActivo"].ToString());
                IdComponente = Convert.ToInt32(Request.Params["IdComponente"].ToString());
                IdSubTipoComponente = Convert.ToInt32(Request.Params["IdSubTipoComponente"].ToString());
                IdCondicion = Convert.ToInt32(Request.Params["IdCondicion"].ToString());
                IdEstado = Convert.ToInt32(Request.Params["IdEstado"].ToString());
                IdLocacion = Convert.ToInt32(Request.Params["IdLocacion"].ToString());

                var query = dbc.ActViewAssetByComponent(IdAcco, IdTipoActivo, IdComponente, IdSubTipoComponente, IdEstado, IdCondicion, IdLocacion).ToList();

                var total = query.Count();

                var query2 = query.Skip(skip).Take(take);

                return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Content("<script>alert('ERROR');</script>");
            }
        }

        public ActionResult ReporteActivoPorComponente()
        {
            try
            {
                int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
                int UserId = Convert.ToInt32(Session["UserId"]);

                IdTipoActivo = Convert.ToInt32(Request.Params["IdTipoActivo"].ToString());
                IdComponente = Convert.ToInt32(Request.Params["IdComponente"].ToString());
                IdSubTipoComponente = Convert.ToInt32(Request.Params["IdSubTipoComponente"].ToString());
                IdCondicion = Convert.ToInt32(Request.Params["IdCondicion"].ToString());
                IdEstado = Convert.ToInt32(Request.Params["IdEstado"].ToString());
                IdLocacion = Convert.ToInt32(Request.Params["IdLocacion"].ToString());

                var query = dbc.ActViewAssetByComponent(IdAcco, IdTipoActivo, IdComponente, IdSubTipoComponente, IdEstado, IdCondicion, IdLocacion).ToList();

                return Json(new { data = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Content("<script>alert('ERROR');</script>");
            }
        }
        public ActionResult reporteHistoricoActivos()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
            //int Estado = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.ActViewAssetByReportHistorial(ID_PERS_ENTI).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteActivosHistorico()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);

            var query = dbc.ActViewAssetByReportHistorial(ID_PERS_ENTI).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult reporteActivoPorArea()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ID_AREA = Convert.ToInt32(Request.Params["ID_AREA"]);
            //int Estado = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.ActViewAssetByArea(IdAcco, ID_AREA).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportePorArea()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ID_AREA = Convert.ToInt32(Request.Params["ID_AREA"]);

            var query = dbc.ActViewAssetByArea(IdAcco, ID_AREA)
                            .Take(5000)
                            .ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DescargarExcelTicket(string area)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                int IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());

                int areaId = string.IsNullOrEmpty(area) ? 0 : Convert.ToInt32(area);

                var query = dbc.ActViewAssetByArea(IdAcco, areaId).ToList();
                var selectedData = query.Select(item => new
                {
                    Area = item.Area,
                    Usuario = item.Usuario,
                    TipoActivo = item.TipoActivo,
                    CodigoActivo = item.CodigoActivo,
                    NombreActivo = item.NombreActivo,
                    SerieActivo = item.SerieActivo,
                    EstadoActivo = item.EstadoActivo,
                    Condicion = item.Condicion,
                    CreadoPor = item.CreadoPor,
                    FechaCreacion = item.FechaCreacion,
                    FechaAsignacion = item.FechaAsignacion,
                    FechaRecepcion = item.FechaRecepcion
                }).ToList();

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("<meta charset='utf-8'>");
                stringBuilder.AppendLine("<style type='text/css'>");
                stringBuilder.AppendLine(".cabecera {background: #2a4389;color: white;font-family: Arial, sans-serif;font-size: 12px;font-weight: normal;border-width: thin;padding: 10px;border: solid;overflow: hidden;word-break: normal;border-color: #b6bbc3;}");
                stringBuilder.AppendLine(".fecha {background: #b6bbc3;color: #000000;font-size: 14px;font-weight: bold;}");
                stringBuilder.AppendLine(".contenido {font-family: Arial, sans-serif;font-size: 14px;padding: 10px;border-style: solid;overflow: hidden;border-width: thin;word-break: normal;border-color: #b6bbc3;}");
                stringBuilder.AppendLine("</style>");

                stringBuilder.AppendLine("<table>");

                stringBuilder.AppendLine("<tr>");
                stringBuilder.AppendLine("<th class='cabecera'>Area</th>");
                stringBuilder.AppendLine("<th class='cabecera'>Usuario Final</th>");
                stringBuilder.AppendLine("<th class='cabecera'>TipoActivo</th>");
                stringBuilder.AppendLine("<th class='cabecera'>CodigoActivo</th>");
                stringBuilder.AppendLine("<th class='cabecera'>NombreActivo</th>");
                stringBuilder.AppendLine("<th class='cabecera'>SerieActivo</th>");
                stringBuilder.AppendLine("<th class='cabecera'>EstadoActivo</th>");
                stringBuilder.AppendLine("<th class='cabecera'>Condicion</th>");
                stringBuilder.AppendLine("<th class='cabecera'>CreadoPor</th>");
                stringBuilder.AppendLine("<th class='cabecera'>FechaCreacion</th>");
                stringBuilder.AppendLine("<th class='cabecera'>FechaAsignacion</th>");
                stringBuilder.AppendLine("<th class='cabecera'>FechaRecepcion</th>");
                stringBuilder.AppendLine("</tr>");

                foreach (var item in selectedData)
                {
                    stringBuilder.AppendLine("<tr>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.Area}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.Usuario}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.TipoActivo}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.CodigoActivo}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.NombreActivo}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.SerieActivo}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.EstadoActivo}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.Condicion}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.CreadoPor}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.FechaCreacion}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.FechaAsignacion}</td>");
                    stringBuilder.AppendLine($"<td class='contenido'>{item.FechaRecepcion}</td>");
                    stringBuilder.AppendLine("</tr>");
                }

                stringBuilder.AppendLine("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", $"attachment;filename=ExcelTicket_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls");
                Response.Write(stringBuilder.ToString());
                Response.Flush();
                Response.End();

                return View();
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message, "text/plain");
            }
        }

        private void AppendTableCell(StringBuilder stringBuilder, object value)
        {
            stringBuilder.AppendLine($"<td class='contenido'>{value}</td>");
        }

        private string GetCurrentDateTime()
        {
            var now = DateTime.Now;
            return now.ToString("yyyyMMddHHmmss");
        }


        public ActionResult ListarAreas()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ArListarAreas(IdAcco).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult ListarAreasCuenta()
        //{
        //    int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
        //    var query = dbc.ArListarAreasCuenta(IdAcco).ToList();

        //    return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ListarActivossss()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int IdTipoActivo = 0;
            string pe = "";
            try
            {
                IdTipoActivo = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
                pe = Request.QueryString["filter[filters][1][value]"];
            }
            catch
            { }
            var query = dbc.ListarActivos1(IdTipoActivo, ID_ACCO, pe).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerLocacion(int id = 0)
        {
            //Locacion
            var locacion = dbc.CLIENT_ASSET.Where(x => x.ID_ASSE == id && x.DAT_END == null).SingleOrDefault();

            return Json(locacion.ID_LOCA, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ActualizarUbicacion()
        {
            int IdAsse = Convert.ToInt32(Request.QueryString["IdAsse"]);
            string IdLoghjcacion = Convert.ToString(Request.QueryString["IdLoca"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            if (Convert.ToString(Request.QueryString["IdLoca"]) == "")
            {
                return Content("Seleccione una locación.");
            }
            else
            {
                int IdLocacion = Convert.ToInt32(Request.QueryString["IdLoca"]);
                dbc.ActActualizarUbicacion(IdAsse, IdLocacion, UserId);
                return Content("El Registro ha sido actualizado.");
            }
        }

        public ActionResult ExportarReporteActivoPorArea()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            List<ActViewAssetByArea_Result> query = dbc.ActViewAssetByArea(ID_ACCO, ID_AREA).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            if (ID_ACCO == 60 || ID_ACCO == 61 || ID_ACCO == 62 || ID_ACCO == 63 || ID_ACCO == 64 || ID_ACCO == 65 ||
                ID_ACCO == 66 || ID_ACCO == 67 || ID_ACCO == 68 || ID_ACCO == 68 || ID_ACCO == 69 || ID_ACCO == 70 ||
                ID_ACCO == 71 || ID_ACCO == 72)
            {
                sb.Append("<meta charset='utf-8'>");
                sb.Append("<style type='text/css'>");
                sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
                sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin:5px 10px;}");
                sb.Append("</style>");
                sb.Append("<table class='tg'>");
                sb.Append("<tr>");
                sb.Append("<th class='cabecera'>Área</td>");
                sb.Append("<th class='cabecera'>Unidad Negocio</td>");
                sb.Append("<th class='cabecera'>Asignado a</td>");
                sb.Append("<th class='cabecera'>Tipo de Activo</td>");
                sb.Append("<th class='cabecera'>Código </td>");
                sb.Append("<th class='cabecera'>Activo</td>");
                sb.Append("<th class='cabecera'>Serie</td>");
                sb.Append("<th class='cabecera'>Estado</td>");
                sb.Append("<th class='cabecera'>Condición</td>");
                sb.Append("<th class='cabecera'>Creado Por</td>");
                sb.Append("<th class='cabecera'>Fecha de Creación</td>");
                sb.Append("<th class='cabecera'>Fecha de Asignación</td>");
                sb.Append("</tr>");

                foreach (ActViewAssetByArea_Result dr in query)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.Area + "</td>");
                    sb.Append("<td class='contenido'>" + dr.UnidadNegocio + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Usuario + "</td>");
                    sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.CodigoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.NombreActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.SerieActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.EstadoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Condicion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.CreadoPor + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaCreacion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaAsignacion + "</td>");
                    sb.Append("</tr>");
                }
            }
            else
            {
                sb.Append("<meta charset='utf-8'>");
                sb.Append("<style type='text/css'>");
                sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
                sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin:5px 10px;}");
                sb.Append("</style>");
                sb.Append("<table class='tg'>");
                sb.Append("<tr>");
                sb.Append("<th class='cabecera'>Área</td>");
                sb.Append("<th class='cabecera'>Asignado a</td>");
                sb.Append("<th class='cabecera'>Tipo de Activo</td>");
                sb.Append("<th class='cabecera'>Código </td>");
                sb.Append("<th class='cabecera'>Activo</td>");
                sb.Append("<th class='cabecera'>Serie</td>");
                sb.Append("<th class='cabecera'>Estado</td>");
                sb.Append("<th class='cabecera'>Condición</td>");
                sb.Append("<th class='cabecera'>Creado Por</td>");
                sb.Append("<th class='cabecera'>Fecha de Creación</td>");
                sb.Append("<th class='cabecera'>Fecha de Asignación</td>");
                sb.Append("</tr>");

                foreach (ActViewAssetByArea_Result dr in query)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.Area + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Usuario + "</td>");
                    sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.CodigoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.NombreActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.SerieActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.EstadoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Condicion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.CreadoPor + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaCreacion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaAsignacion + "</td>");
                    sb.Append("</tr>");
                }
            }


            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=ReportePorArea" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ExportarReporteHistoricoActivos()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            List<ActViewAssetByReportHistorial_Result> query = dbc.ActViewAssetByReportHistorial(ID_PERS_ENTI).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;

            if (ID_ACCO == 60 || ID_ACCO == 61 || ID_ACCO == 62 || ID_ACCO == 63 || ID_ACCO == 64 || ID_ACCO == 65 ||
                ID_ACCO == 66 || ID_ACCO == 67 || ID_ACCO == 68 || ID_ACCO == 68 || ID_ACCO == 69 || ID_ACCO == 70 ||
                ID_ACCO == 71 || ID_ACCO == 72)
            {
                sb.Append("<meta charset='utf-8'>");
                sb.Append("<style type='text/css'>");
                sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
                sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin:5px 10px;}");
                sb.Append("</style>");
                sb.Append("<table class='tg'>");
                sb.Append("<tr>");
                sb.Append("<th class='cabecera'>Estado</td>");
                sb.Append("<th class='cabecera'>Asignado a</td>");
                sb.Append("<th class='cabecera'>Tipo de Activo</td>");
                sb.Append("<th class='cabecera'>Tipo Movimiento</td>");
                sb.Append("<th class='cabecera'>Código de Activo</td>");
                sb.Append("<th class='cabecera'>Activo</td>");
                sb.Append("<th class='cabecera'>Serie</td>");
                sb.Append("<th class='cabecera'>Fecha de Asignación</td>");
                sb.Append("<th class='cabecera'>Fecha de Recepción</td>");
                sb.Append("<th class='cabecera'>Fecha de Creación</td>");
                sb.Append("<th class='cabecera'>Unidad Negocio</td>");
                sb.Append("<th class='cabecera'>Área</td>");
                sb.Append("</tr>");

                foreach (ActViewAssetByReportHistorial_Result dr in query)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.Estado + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Usuario + "</td>");
                    sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.TipoMovimiento + "</td>");
                    sb.Append("<td class='contenido'>" + dr.CodigoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.NombreActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.SerieActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaAsignacion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaRecepcion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaCreacion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.UnidadNegocio + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Area + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

            }
            else
            {
                sb.Append("<meta charset='utf-8'>");
                sb.Append("<style type='text/css'>");
                sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
                sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin:5px 10px;}");
                sb.Append("</style>");
                sb.Append("<table class='tg'>");
                sb.Append("<tr>");
                sb.Append("<th class='cabecera'>Estado</td>");
                sb.Append("<th class='cabecera'>Asignado a</td>");
                sb.Append("<th class='cabecera'>Tipo de Activo</td>");
                sb.Append("<th class='cabecera'>Código de Activo</td>");
                sb.Append("<th class='cabecera'>Activo</td>");
                sb.Append("<th class='cabecera'>Serie</td>");
                sb.Append("<th class='cabecera'>Fecha de Asignación</td>");
                sb.Append("<th class='cabecera'>Fecha de Recepción</td>");
                sb.Append("<th class='cabecera'>Fecha de Creación</td>");
                sb.Append("</tr>");

                foreach (ActViewAssetByReportHistorial_Result dr in query)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.Estado + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Usuario + "</td>");
                    sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.CodigoActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.NombreActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.SerieActivo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaAsignacion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaRecepcion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaCreacion + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

            }



            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=ReporteHistorico" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ExportarListaActivoComponente()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (String.IsNullOrEmpty(Request.Params["IdTipoActivo"])) { IdTipoActivo = 0; }
            else { IdTipoActivo = Convert.ToInt32(Request.Params["IdTipoActivo"].ToString()); }

            if (String.IsNullOrEmpty(Request.Params["IdComponente"])) { IdComponente = 0; }
            else { IdComponente = Convert.ToInt32(Request.Params["IdComponente"].ToString()); }

            if (String.IsNullOrEmpty(Request.Params["IdSubTipoComponente"])) { IdSubTipoComponente = 0; }
            else { IdSubTipoComponente = Convert.ToInt32(Request.Params["IdSubTipoComponente"].ToString()); }

            if (String.IsNullOrEmpty(Request.Params["IdCondicion"])) { IdCondicion = 0; }
            else { IdCondicion = Convert.ToInt32(Request.Params["IdCondicion"].ToString()); }

            if (String.IsNullOrEmpty(Request.Params["IdEstado"])) { IdEstado = 0; }
            else { IdEstado = Convert.ToInt32(Request.Params["IdEstado"].ToString()); }

            if (String.IsNullOrEmpty(Request.Params["IdLocacion"])) { IdLocacion = 0; }
            else { IdLocacion = Convert.ToInt32(Request.Params["IdLocacion"].ToString()); }

            List<ActViewAssetByComponent_Result> query = dbc.ActViewAssetByComponent(ID_ACCO, IdTipoActivo, IdComponente, IdSubTipoComponente, IdEstado, IdCondicion, IdLocacion).ToList();

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
            sb.Append("<th class='cabecera'>Asignado a</td>");
            sb.Append("<th class='cabecera'>Tipo de Activo</td>");
            sb.Append("<th class='cabecera'>Código de Activo</td>");
            sb.Append("<th class='cabecera'>Nombre de Activo</td>");
            sb.Append("<th class='cabecera'>Serie de Activo</td>");
            sb.Append("<th class='cabecera'>Componente</td>");
            sb.Append("<th class='cabecera'>Subtipo de Componente</td>");
            sb.Append("<th class='cabecera'>Comentario</td>");
            sb.Append("<th class='cabecera'>Estado</td>");
            sb.Append("<th class='cabecera'>Condición</td>");
            sb.Append("<th class='cabecera'>Locación</td>");
            sb.Append("<th class='cabecera'>Creado por</td>");
            sb.Append("<th class='cabecera'>Fecha de Asignación Componente</td>");
            sb.Append("<th class='cabecera'>Fecha de Asignación Activo</td>");
            sb.Append("</tr>");

            foreach (ActViewAssetByComponent_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.UsuarioAsignado + "</td>");
                sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                sb.Append("<td class='contenido'>&nbsp;" + dr.CodigoActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.NombreActivo + "</td>");
                sb.Append("<td class='contenido'>&nbsp;" + dr.SerieActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.Componente + "</td>");
                sb.Append("<td class='contenido'>" + dr.SubTipoComponente + "</td>");
                sb.Append("<td class='contenido'>" + dr.ComentarioAsignacion + "</td>");
                sb.Append("<td class='contenido'>" + dr.Estado + "</td>");
                sb.Append("<td class='contenido'>" + dr.Condicion + "</td>");
                sb.Append("<td class='contenido'>" + dr.Locacion + "</td>");
                sb.Append("<td class='contenido'>" + dr.UsuarioCrea + "</td>");
                sb.Append("<td class='contenido'>" + dr.FechaAsignacion + "</td>");
                sb.Append("<td class='contenido'>" + dr.FechaAsignacionActivo + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=ActivoComponente" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ExportarListaActivoPrograma()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            List<ActViewAssetByProgram_Result> query = dbc.ActViewAssetByProgram(ID_ACCO, IdPrograma, IdTipoActivoPro, IdEstado, IdCondicion).ToList();

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
            sb.Append("<th class='cabecera'>Asignado a</td>");
            sb.Append("<th class='cabecera'>Tipo de Activo</td>");
            sb.Append("<th class='cabecera'>Código de Activo</td>");
            sb.Append("<th class='cabecera'>Activo</td>");
            sb.Append("<th class='cabecera'>Serie de Activo</td>");
            sb.Append("<th class='cabecera'>Código de Programa</td>");
            sb.Append("<th class='cabecera'>Programa</td>");
            sb.Append("<th class='cabecera'>Serie</td>");
            sb.Append("<th class='cabecera'>Proveedor</td>");
            sb.Append("<th class='cabecera'>Estado</td>");
            sb.Append("<th class='cabecera'>Condición</td>");
            sb.Append("<th class='cabecera'>Creado por</td>");
            sb.Append("<th class='cabecera'>Fecha de Asignación</td>");
            sb.Append("</tr>");

            foreach (ActViewAssetByProgram_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.UsuarioAsignado + "</td>");
                sb.Append("<td class='contenido'>" + dr.TipoActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.CodigoActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.NombreActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.SerieActivo + "</td>");
                sb.Append("<td class='contenido'>" + dr.CodigoPrograma + "</td>");
                sb.Append("<td class='contenido'>" + dr.NombrePrograma + "</td>");
                sb.Append("<td class='contenido'>" + dr.SeriePrograma + "</td>");
                sb.Append("<td class='contenido'>" + dr.Proveedor + "</td>");
                sb.Append("<td class='contenido'>" + dr.Estado + "</td>");
                sb.Append("<td class='contenido'>" + dr.Condicion + "</td>");
                sb.Append("<td class='contenido'>" + dr.UsuarioCrea + "</td>");
                sb.Append("<td class='contenido'>" + dr.FechaAsignacion + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=ActivoPrograma" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        //[HttpPost]
        //public JsonResult UploadExcel(ASSET users, HttpPostedFileBase FileUpload)
        //{

        //    List<string> data = new List<string>();
        //    if (FileUpload != null)
        //    {
        //        // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
        //        if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //        {


        //            string filename = FileUpload.FileName;
        //            string targetpath = Server.MapPath("~/Doc/");
        //            FileUpload.SaveAs(targetpath + filename);
        //            string pathToExcelFile = targetpath + filename;
        //            var connectionString = "";
        //            if (filename.EndsWith(".xls"))
        //            {
        //                connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
        //            }
        //            else if (filename.EndsWith(".xlsx"))
        //            {
        //                connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
        //            }

        //            var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
        //            var ds = new DataSet();

        //            adapter.Fill(ds, "ExcelTable");

        //            DataTable dtable = ds.Tables["ExcelTable"];

        //            string sheetName = "Sheet1";

        //            var excelFile = new ExcelQueryFactory(pathToExcelFile);
        //            var artistAlbums = from a in excelFile.Worksheet<ASSET>(sheetName) select a;

        //            foreach (var a in artistAlbums)
        //            {
        //                try
        //                {
        //                    if (a.NAM_EQUI != "")
        //                    {
        //                        ASSET AST = new ASSET();
        //                        //TU.Name = a.Name;
        //                        //TU.Address = a.Address;
        //                        //TU.ContactNo = a.ContactNo;
        //                        AST.NAM_EQUI = a.NAM_EQUI;
        //                        AST.TYPE_ASSET = a.TYPE_ASSET;
        //                        //MARCA
        //                        AST.COMMERCIAL_MODEL = a.COMMERCIAL_MODEL;
        //                        AST.MANUFACTURER_MODEL = a.MANUFACTURER_MODEL;
        //                        AST.SER_NUMB = a.SER_NUMB;
        //                        AST.MAC_ADRR = a.MAC_ADRR;
        //                        AST.NAM_EQUI = a.NAM_EQUI;
        //                        AST.ACQ_DATE = a.ACQ_DATE;
        //                        AST.IdContrato = a.IdContrato;
        //                        //LOCACION
        //                        AST.SOLPED = a.SOLPED;
        //                        AST.LOTE = a.LOTE;
        //                        AST.ID_BUY_MODE = a.ID_BUY_MODE;
        //                        AST.ID_COST_CENT = a.ID_COST_CENT;
        //                        AST.COST = a.COST;
        //                        AST.IpLocal = a.IpLocal;
        //                        AST.IpPublica = a.IpPublica;
        //                        AST.NumeroFactura = a.NumeroFactura;
        //                        AST.ValorActivo = a.ValorActivo;
        //                        AST.RiesgoIntrinseco = a.RiesgoIntrinseco;
        //                        AST.Mac_Address = a.Mac_Address;
        //                        AST.SUM_ASSE = a.SUM_ASSE;
        //                        dbc.ASSETs.Add(AST);
        //                        dbc.SaveChanges();

        //                    }
        //                    else
        //                    {
        //                        data.Add("<ul>");
        //                        if (a.NAM_EQUI == "" || a.NAM_EQUI == null) data.Add("<li> name is required</li>");
        //                        if (a.SER_NUMB == "" || a.SER_NUMB == null) data.Add("<li> Address is required</li>");
        //                        if (a.COD_ASSE == "" || a.COD_ASSE == null) data.Add("<li>ContactNo is required</li>");

        //                        data.Add("</ul>");
        //                        data.ToArray();
        //                        return Json(data, JsonRequestBehavior.AllowGet);
        //                    }
        //                }

        //                catch (DbEntityValidationException ex)
        //                {
        //                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
        //                    {

        //                        foreach (var validationError in entityValidationErrors.ValidationErrors)
        //                        {

        //                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

        //                        }

        //                    }
        //                }
        //            }
        //            //deleting excel file from folder  
        //            if ((System.IO.File.Exists(pathToExcelFile)))
        //            {
        //                System.IO.File.Delete(pathToExcelFile);
        //            }
        //            return Json("success", JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            //alert message for invalid file format  
        //            data.Add("<ul>");
        //            data.Add("<li>Only Excel file format is allowed</li>");
        //            data.Add("</ul>");
        //            data.ToArray();
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        data.Add("<ul>");
        //        if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
        //        data.Add("</ul>");
        //        data.ToArray();
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //private void ExcelConn(string filepath)
        //{
        //    string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", filepath);
        //    Econ = new OleDbConnection(constr);
        //}

        //private void InsertExceldata(string filepath, string filename)
        //{
        //    string fullpath = Server.MapPath("/folder") + filename;
        //    ExcelConn(fullpath);
        //    string query = string.Format("Select * from [{0}]", "Sheet1$");
        //    OleDbCommand Ecom = new OleDbCommand(query, Econ);
        //    Econ.Open();

        //    DataSet ds = new DataSet();
        //    OleDbDataAdapter oda = new OleDbDataAdapter(query, Econ);
        //    Econ.Close();
        //    oda.Fill(ds);

        //    DataTable dt = ds.Tables[0];

        //    SqlBulkCopy objbulk = new SqlBulkCopy(con);
        //    objbulk.DestinationTableName = "TYPE_ASSET";
        //    objbulk.ColumnMappings.Add("NAM_TYPE_ASSE", "NAM_TYPE_ASSE");
        //    objbulk.ColumnMappings.Add("DESC_TYPE_ASSE", "DESC_TYPE_ASSE");
        //    objbulk.ColumnMappings.Add("IN_GRAP", "IN_GRAP");
        //    objbulk.ColumnMappings.Add("COLOR", "COLOR");
        //    objbulk.ColumnMappings.Add("INDICE", "INDICE");
        //    con.Open();
        //    objbulk.WriteToServer(dt);
        //    con.Close();
        //}

        //[HttpPost]
        //public JsonResult UploadExcel(TYPE_ASSET tipoActivo, HttpPostedFileBase FileUpload)
        //{

        //    List<string> data = new List<string>();
        //    if (FileUpload != null)
        //    {
        //        // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
        //        if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //        {


        //            string filename = FileUpload.FileName;
        //            string targetpath = Server.MapPath("~/Doc/");
        //            FileUpload.SaveAs(targetpath + filename);
        //            string pathToExcelFile = targetpath + filename;
        //            var connectionString = "";
        //            if (filename.EndsWith(".xls"))
        //            {
        //                connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
        //            }
        //            else if (filename.EndsWith(".xlsx"))
        //            {
        //                connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
        //            }

        //            var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
        //            var ds = new DataSet();

        //            adapter.Fill(ds, "ExcelTable");

        //            DataTable dtable = ds.Tables["ExcelTable"];

        //            string sheetName = "Sheet1";

        //            var excelFile = new ExcelQueryFactory(pathToExcelFile);
        //            var artistAlbums = from a in excelFile.Worksheet<TYPE_ASSET>(sheetName) select a; 

        //            foreach (var a in artistAlbums)
        //            {
        //                try
        //                {
        //                    if (a.Name != "" && a.Address != "" && a.ContactNo != "")
        //                    {
        //                        TYPE_ASSET ta = new TYPE_ASSET();
        //                        ta.NAM_TYPE_ASSE = a.NAM_TYPE_ASSE;
        //                        ta.DESC_TYPE_ASSE = a.DESC_TYPE_ASSE;
        //                        ta.IN_GRAP = a.IN_GRAP;
        //                        ta.COLOR = a.COLOR;
        //                        ta.INDICE = a.INDICE;
        //                        dbc.TYPE_ASSET.Add(ta);
        //                        dbc.SaveChanges();

        //                    }
        //                    else
        //                    {
        //                        data.Add("<ul>");
        //                        if (a.NAM_TYPE_ASSE == "" || a.Name == null) data.Add("<li>Se requiere el nombre</li>");

        //                        data.Add("</ul>");
        //                        data.ToArray();
        //                        return Json(data, JsonRequestBehavior.AllowGet);
        //                    }
        //                }

        //                catch (DbEntityValidationException ex)
        //                {
        //                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
        //                    {

        //                        foreach (var validationError in entityValidationErrors.ValidationErrors)
        //                        {

        //                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

        //                        }

        //                    }
        //                }
        //            }
        //            //deleting excel file from folder  
        //            if ((System.IO.File.Exists(pathToExcelFile)))
        //            {
        //                System.IO.File.Delete(pathToExcelFile);
        //            }
        //            return Json("success", JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            //alert message for invalid file format  
        //            data.Add("<ul>");
        //            data.Add("<li>Only Excel file format is allowed</li>");
        //            data.Add("</ul>");
        //            data.ToArray();
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        data.Add("<ul>");
        //        if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
        //        data.Add("</ul>");
        //        data.ToArray();
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult ListarHistorialActivos(int id = 0)
        {
            var result = dbc.ActListarHistorialActivos(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarResponsables()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            int Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 6 && x.ID_ACCO == ID_ACCO).Single().VAL_ACCO_PARA);

            var queryPE = dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == Compania).ToList();

            var result = (from pe in queryPE
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          join ae in dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO && ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList()
                            on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                          select new
                          {
                              CLIE = p.FIR_NAME + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              pe.ID_AREA
                          }).ToList().OrderBy(x => x.CLIE);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResponsablexArea(int id)
        {
            string termino = "";
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdAreaResponsable = id;

            List<PersonaPorAreaResponsable_Result> resultado = dbe.PersonaPorAreaResponsable(IdCuenta, IdAreaResponsable, termino).ToList();

            return Json(new { Data = resultado, Count = resultado.Count() }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult UpdateComentAsset(int id)
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                TICKET query = dbc.TICKETs.Single(x => x.ID_TICK == id);
                ViewBag.FEC_TICK = query.FEC_TICK;
                ViewBag.FEC_INI_TICK_EDIT = query.FEC_INI_TICK;
                ViewBag.FlagProblema = Convert.ToInt32(query.FlagProblema);
                ViewBag.Habilitado = Convert.ToInt16(Session["SERVICEDESK"]);
                return View(query);
            }
            catch
            {
                return Content("Please Close Session");
            }

        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateComentAsset(IEnumerable<HttpPostedFileBase> files, TICKET ticket, int id)
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                DateTime FEC_INI_TICK = Convert.ToDateTime(Request.Params["FEC_INI_TICK_EDIT"]);
                int ID_TICK = id;
                string SUM_TICK = Convert.ToString(ticket.SUM_TICK);

                if (String.IsNullOrEmpty(SUM_TICK))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Ingrese Comentario');}window.onload=init;</script>");
                }

                TICKET query = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                query.SUM_TICK = SUM_TICK;


                dbc.Entry(query).State = EntityState.Modified;
                dbc.SaveChanges();

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                    RegexOptions.None, TimeSpan.FromSeconds(1.5));
                            var ATTA = new ATTACHED_TICKET_FORMAT();
                            ATTA.NAM_ATTA = doc;
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_TICK = id;
                            ATTA.CREATE_DATE = DateTime.Now;

                            ////db.AddToATTACHEDs(ATTA);
                            dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Delivery/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('OK','La información ha sido actualizada.');}window.onload=init;</script>");


            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Contácte al Administrador');}window.onload=init;</script>");

            }
        }

        [Authorize]
        public ActionResult UpdateComentDetailsTicketAsset(int id)
        {
            DETAILS_TICKET query = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == id);
            ViewBag.COM_DETA_TICK_EDIT = query.COM_DETA_TICK;
            ViewBag.FEC_SCHE_EDIT = query.FEC_SCHE;
            ViewBag.ID_STAT_EDIT = query.ID_STAT;
            ViewBag.ID_TYPE_DETA_TICK_EDIT = query.ID_TYPE_DETA_TICK;
            ViewBag.CREATE_DETA_INCI_EDIT = query.CREATE_DETA_INCI;
            ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
            ViewBag.PortalComent = Convert.ToBoolean(query.PortalComent);
            if (Convert.ToInt32(Session["ID_ACCO"]) == 3 && query.ID_STAT == 4)
            {
                ViewBag.FEC_END_REAL_EDIT = query.FEC_END_REAL;
            }
            ViewBag.Habilitado = Convert.ToInt16(Session["SUPERVISOR_SERVICEDESK"]);
            ViewBag.IdTicket = Convert.ToString(query.ID_TICK);
            return View(query);
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateComentDetailsTicketAsset(IEnumerable<HttpPostedFileBase> files, DETAILS_TICKET detatick)
        {
            try
            {
                var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int ID_DETA_TICK = Convert.ToInt32(detatick.ID_DETA_TICK);
                string COM_DETA_TICK = Convert.ToString(Request.Params["COM_DETA_TICK_EDIT"]);
                int ID_TYPE_DETA_TICK = Convert.ToInt32(Request.Params["ID_TYPE_DETA_TICK_EDIT"]);
                int ID_TICK = Convert.ToInt32(detatick.ID_TICK);

                DETAILS_TICKET query = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);

                if (String.IsNullOrEmpty(COM_DETA_TICK.Trim()))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEditDeta) top.uploadDoneEditDeta('ERROR','Ingrese un Comentario');}window.onload=init;</script>");
                }

                query.COM_DETA_TICK = COM_DETA_TICK;

                dbc.Entry(query).State = EntityState.Modified;
                dbc.SaveChanges();
                if (files != null)
                {
                    int id_TF = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).ID_TYPE_FORM.Value;
                    string ruta = "Delivery";
                    if (id_TF == 2) { ruta = "Reception"; }
                    foreach (var file in files)
                    {
                        try
                        {
                            var ATTA = new ATTACHED_TICKET_FORMAT();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_DETA_TICK = ID_DETA_TICK;
                            ATTA.CREATE_DATE = DateTime.Now;

                            dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/" + ruta + "/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEditDeta) top.uploadDoneEditDeta('OK','La información ha sido actualizada.');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEditDeta) top.uploadDoneEditDeta('ERROR','Contacte al Administrador');}window.onload=init;</script>");

            }
        }

        public int ObtenerHoraSLA(int idTicket = 0)
        {
            return dbc.ObtenerHoraSLA(idTicket).Single().TiempoAtencion;
        }

        public ActionResult ListarTipoServidor()
        {
            var result = dbc.ListarTipoServidor().ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewHistoryAssetReportGral()
        {
            return View();
        }

        [Authorize]
        public ActionResult RepositorioDocumentoActivos()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //int UserId = (int)Session["UserId"];
            //if (ID_ACCO != 56 || ID_ACCO != 57 || ID_ACCO != 58)
            //{
            //   return RedirectToAction("Index", "Error");
            //}
            if (ID_ACCO == 60)
            {
                int idGrupoActivo = ObtenerIdGrupoUsuarioBNV();

                if (idGrupoActivo == 0)
                    return RedirectToAction("Index", "Error");

                ViewBag.IdGrupo = idGrupoActivo;
                return View("RepositorioDocumentoActivosBNV");
            }
            return View();
        }

        public ActionResult BuscarArchivoActivos()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_TYPE_FORM, ID_PERS_ENTI;

            if (Request.Params["ID_TYPE_FORM"] == "") { ID_TYPE_FORM = 0; }
            else { ID_TYPE_FORM = Convert.ToInt32(Request.Params["ID_TYPE_FORM"]); }

            if (Request.Params["ID_PERS_ENTI"] == "") { ID_PERS_ENTI = 0; }
            else { ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]); }

            string SER_NUMB = Convert.ToString(Request.Params["SER_NUMB"] == null ? "" : Request.Params["SER_NUMB"]);

            List<BuscarArchivoActivos_Result> query = dbc.BuscarArchivoActivos(ID_ACCO, ID_TYPE_FORM, ID_PERS_ENTI, SER_NUMB).ToList();
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


        public ActionResult BuscarArchivoActivosD()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_TYPE_FORM, ID_PERS_ENTI;

            if (Request.Params["ID_TYPE_FORM"] == "") { ID_TYPE_FORM = 0; }
            else { ID_TYPE_FORM = Convert.ToInt32(Request.Params["ID_TYPE_FORM"]); }

            if (Request.Params["ID_PERS_ENTI"] == "") { ID_PERS_ENTI = 0; }
            else { ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]); }

            string SER_NUMB = Convert.ToString(Request.Params["SER_NUMB"] == null ? "" : Request.Params["SER_NUMB"]);

            List<BuscarArchivoActivos_Result> query = dbc.BuscarArchivoActivos(ID_ACCO, ID_TYPE_FORM, ID_PERS_ENTI, SER_NUMB).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarUMineraBNV()
        {
            int idGrupo, id, idPers = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "COM_NAME")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            string grupo = ObtenerNomGrupoUsuarioBNV(idGrupo);

            if ((grupo == "MICROINFORMATICO" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO"]) == 1) || (grupo == "INFRAESTRUCTURA" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"]) == 1) || (grupo == "MOVIL" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"]) == 1))
                id = 1;
            else
                id = 2;

            var result = dbc.ListarUMinerasBNV(id, idPers, idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAllUMineraBNV()
        {
            var result = dbc.ListarUMinera().ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProyectosBNV()
        {
            int idGrupo;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "Nombre")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarContratosBNV(idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateBNV(IEnumerable<HttpPostedFileBase> archivos, ASSET asset)
        {
            int ID_ACCO = 0;
            int ID_USER = 0;
            try
            {
                ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                ID_USER = Convert.ToInt32(Session["UserId"].ToString());
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','2');}window.onload=init;</script>");
            }

            if (asset.IdGrupo == null)
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Seleccione el grupo.',0);}window.onload=init;</script>");

            int ID_PER_ENTI, ID_LOCA = 0, ID_QUEU, ID_PRIO = 4, ID_ASSI = 0;
            int idDaaS = dbc.Contratoes.FirstOrDefault(x => x.ID_ACCO == 60 && x.Estado == true && x.Nombre == "DaaS").Id;
            int idAlqui = dbc.Contratoes.FirstOrDefault(x => x.ID_ACCO == 60 && x.Estado == true && x.Nombre == "Alquiler").Id;
            string nomGrupo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(asset.IdGrupo));
            string mensajeE = "";
            string SER_NUMB = Convert.ToString(Request.Form["SER_NUMB"]);
            string SUM_ASSE = Convert.ToString(Request.Form["SUM_ASSE"]);
            string NAM_EQUI = Convert.ToString(Request.Form["NAM_EQUI"]);
            SUM_ASSE = SUM_ASSE.Replace("&nbsp;", "");
            SUM_ASSE = SUM_ASSE.Replace("<br />", "");

            //Validaciones
            int cantAsset = dbc.ASSETs.Where(x => x.SER_NUMB == SER_NUMB && x.IdGrupo == asset.IdGrupo && x.ID_ACCO == ID_ACCO).Count();
            if (cantAsset > 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','3');}window.onload=init;</script>");
            }

            if (asset.UMinera == null)
                mensajeE += "Seleccione la unidad minera. <br/>";
            if (asset.ID_TYPE_ASSE == null)
                mensajeE += "Seleccione el tipo de activo. <br/>";
            if (SER_NUMB == "")
                mensajeE += "Ingrese el número de serie. <br/>";
            if (asset.ID_MANU == null)
                mensajeE += "Seleccione la marca. <br/>";
            if (asset.ID_COMM_MODE == null)
                mensajeE += "Seleccione el modelo comercial. <br/>";

            // Validaciones MICROINFORMATICO
            if (nomGrupo == "MICROINFORMATICO")
            {
                if (asset.IdContrato == null)
                    mensajeE += "Seleccione el proyecto. <br/>";
                if (asset.IdContrato == idDaaS && (asset.ACQ_DATE == null || asset.FechaFin == null))
                    mensajeE += "Seleccione las fechas. <br/>";
                if (asset.IdContrato == idAlqui && asset.ACQ_DATE == null)
                    mensajeE += "Seleccione la fecha de inicio. <br/>";
            }
            // Validaciones INFRAESTRUCTURA
            else if (nomGrupo == "INFRAESTRUCTURA")
            {
                if (asset.ID_BUY_MODE == null)
                    mensajeE += "Seleccione el modo de compra. <br/>";
                if (asset.IdContrato == null)
                    mensajeE += "Seleccione el contrato. <br/>";

                ID_LOCA = (Request.Form["Locacion"] != "" && Request.Form["Locacion"] != null) ? Convert.ToInt32(Request.Form["Locacion"]) : 0;
            }
            // Validaciones MOVIL
            else if (nomGrupo == "MOVIL")
            {
                if (asset.IdContrato == null)
                {
                    mensajeE += "Seleccione el contrato. <br/>";
                }

                var antivirus = Request.Params["AntivirusMovil"];
                asset.Antivirus = antivirus == "on";
            }

            if (mensajeE != "")
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','" + mensajeE + "',0);}window.onload=init;</script>");

            try
            {
                ID_QUEU = Convert.ToInt32(Session["ID_QUEU"].ToString());
                ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                ID_QUEU = 0;
            }

            if (ModelState.IsValid)
            {
                var query = dbc.PARAMETERs.Where(x => x.NAM_PARA == "RESPONSIBLE ASSET")
                    .Join(dbc.ACCOUNT_PARAMETER, x => x.ID_PARA, p => p.ID_PARA, (x, p) => new
                    {
                        p.VAL_ACCO_PARA,
                        p.ID_ACCO
                    }).Where(x => x.ID_ACCO == ID_ACCO).First();

                ID_PER_ENTI = Convert.ToInt32(query.VAL_ACCO_PARA);
                ID_PRIO = Convert.ToInt32(dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_PER_ENTI).ID_PRIO);

                asset.CREATE_DATE = DateTime.Now;
                asset.CREATE_ASSE = DateTime.Now;
                asset.ID_ACCO = ID_ACCO;
                asset.UserId = ID_USER;
                dbc.ASSETs.Add(asset);
                dbc.SaveChanges();

                //Agregar aplicaciones movil
                if (nomGrupo == "MOVIL")
                {
                    string aplicacionesSelect = Request.Params["aplicacionesMovil"];

                    if (!string.IsNullOrEmpty(aplicacionesSelect))
                    {
                        var listaAplicaciones = aplicacionesSelect.Split(',').ToList();

                        foreach (var idAplicacion in listaAplicaciones)
                        {
                            int idApp = Convert.ToInt32(idAplicacion);

                            var nuevaAplicacion = new AplicacionMovilActivo
                            {
                                IdActivo = asset.ID_ASSE,
                                IdAplicacion = idApp,
                                Estado = true,
                                FechaCrea = DateTime.Now,
                                UsuarioCrea = ID_USER
                            };

                            dbc.AplicacionMovilActivo.Add(nuevaAplicacion);
                        }
                        dbc.SaveChanges();
                    }
                }

                var CLIENTASSET = new CLIENT_ASSET();
                int ID_AREA = 0;

                try
                {   //Obteniendo el area por defecto para los nuevos activos : 31 'AREA BY DEFAULT ASSET'
                    ID_AREA = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 31)
                                    .VAL_ACCO_PARA);
                    CLIENTASSET.ID_AREA = ID_AREA;
                }
                catch { }

                CLIENTASSET.ID_PERS_ENTI = ID_PER_ENTI;
                CLIENTASSET.ID_ASSE = asset.ID_ASSE;
                CLIENTASSET.ID_AREA = ID_AREA;
                CLIENTASSET.ID_COND = 9;
                CLIENTASSET.ID_LOCA = ID_LOCA;
                CLIENTASSET.DAT_STAR = DateTime.Now;
                CLIENTASSET.CREATE_DATE = DateTime.Now;
                CLIENTASSET.UserId = ID_USER;
                CLIENTASSET.ID_TYPE_CLIE_ASSE = 6;
                CLIENTASSET.UMinera = asset.UMinera;
                CLIENTASSET.SUM_CLIE_ASSE = asset.SUM_ASSE;

                dbc.CLIENT_ASSET.Add(CLIENTASSET);
                dbc.SaveChanges();

                if (archivos != null)
                {
                    foreach (var file in archivos)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_ASSE = asset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            //db.AddToATTACHEDs(ATTA);
                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                string usuario = Convert.ToString(Session["NAM_PERS"]).ToUpper();

                string Code = null;
                try
                {
                    int? idGrupo = asset.IdGrupo;
                    var ticket = dbc.InsertarTicketActBNV(ID_ACCO, ID_PER_ENTI, ID_ASSI, ID_AREA, ID_QUEU, ID_PRIO,
                    asset.ID_ASSE, asset.CREATE_ASSE, "TICKET CREATED BY " + usuario + " TO REGISTER TECHNOLOGICAL DEVICE IN THE INVENTORY",
                    ID_USER, idGrupo).Single();

                    int id = Convert.ToInt32(ticket.ID_TICK);

                    Code = ticket.COD_TICK;

                    var asseForm = dbc.ASSETs.Single(x => x.ID_ASSE == asset.ID_ASSE);
                    asseForm.ID_TICK = id;
                    dbc.ASSETs.Attach(asseForm);
                    dbc.Entry(asseForm).State = EntityState.Modified;
                    dbc.SaveChanges();

                }
                catch (Exception e)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + Code + "', '" + asset.ID_ASSE.ToString() + "');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }

        public ActionResult ListByStatusBNV(int IdGrupo, int IdTipoActivo, int IdMarca, int IdUsuario, int IdUMinera, string PalabraClave)
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int Estado = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            string grupo = ObtenerNomGrupoUsuarioBNV(IdGrupo);
            int id, idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if ((grupo == "MICROINFORMATICO" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO"]) == 1) || (grupo == "INFRAESTRUCTURA" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"]) == 1) || (grupo == "MOVIL" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"]) == 1))
                id = 1;
            else
                id = 2;

            var query = dbc.ListadoActivosxGrupoBNV(IdGrupo, IdTipoActivo.ToString(), IdMarca.ToString(), IdUsuario.ToString(), PalabraClave, IdUMinera.ToString(), Estado, IdAcco, idPersEnti, id).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Grid = query2, Cantidad = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContarxEstadosBNV(int IdGrupo, int IdTipoActivo, int IdMarca, int IdUsuario, int IdUMinera, string PalabraClave)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string grupo = ObtenerNomGrupoUsuarioBNV(IdGrupo);
            int id, idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if ((grupo == "MICROINFORMATICO" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO"]) == 1) || (grupo == "INFRAESTRUCTURA" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"]) == 1) || (grupo == "MOVIL" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"]) == 1))
                id = 1;
            else
                id = 2;

            var query = dbc.ListadoActivosxGrupoBNV(IdGrupo, IdTipoActivo.ToString(), IdMarca.ToString(), IdUsuario.ToString(), PalabraClave, IdUMinera.ToString(), 0, ID_ACCO, idPersEnti, id).ToList();

            int Surplus = query.Where(x => x.Estado == "BAJA").Count();
            int Spare = query.Where(x => x.Estado == "REPUESTO").Count();
            int Assigned = query.Where(x => x.Estado == "ASIGNADO").Count();
            int Unassigned = query.Where(x => x.Estado == "NO ASIGNADO").Count();

            return Json(new { Asignado = Assigned, NoAsignado = Unassigned, Repuesto = Spare, Inoperativo = Surplus }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindResultBNV()
        {
            string IdGrupo = Request.Params["IdGrupo"].ToString();
            string UMinera = Request.Params["UMinera"].ToString();
            string Codigo = Request.Params["COD_ASSE"].ToString();
            string Marca = Request.Params["ID_MANU"].ToString();
            //string Estado = Request.Params["ID_STAT_ASSE"].ToString();
            string NombreEquipo = Request.Params["NAM_EQUI"].ToString();
            string Serie = Request.Params["SER_NUMB"].ToString();
            string TiposActivo = Request.Params["valTypeAsset"].ToString();
            string Usuarios = Request.Params["Usuarios"].ToString();
            string Contratos = Request.Params["Contratos"].ToString();
            string Estados = Request.Params["Estados"].ToString();
            string ModeloComercial = Convert.ToString(Request.Params["ID_COMM_MODE"] == null ? "0" : Request.Params["ID_COMM_MODE"]);
            string Condicion = Convert.ToString(Request.Params["ID_COND"] == null ? "0" : Request.Params["ID_COND"]);

            if (String.IsNullOrEmpty(IdGrupo)) IdGrupo = "0";
            if (String.IsNullOrEmpty(UMinera)) UMinera = "0";
            if (String.IsNullOrEmpty(Marca)) Marca = "0";
            //if (String.IsNullOrEmpty(Estado)) Estado = "0";
            if (String.IsNullOrEmpty(ModeloComercial)) ModeloComercial = "0";
            if (String.IsNullOrEmpty(Condicion)) Condicion = "0";

            int IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            //Selección múltiple
            if (String.IsNullOrEmpty(TiposActivo)) TiposActivo = "0";
            TiposActivo = TiposActivo.Substring(0, TiposActivo.Length - 1);
            if (String.IsNullOrEmpty(Usuarios)) Usuarios = "0";
            Usuarios = Usuarios.Substring(0, Usuarios.Length - 1);
            if (String.IsNullOrEmpty(Contratos)) Contratos = "0";
            Contratos = Contratos.Substring(0, Contratos.Length - 1);
            if (String.IsNullOrEmpty(Estados)) Estados = "0";
            Estados = Estados.Substring(0, Estados.Length - 1);

            if (Usuarios == "0") Usuarios = "";
            if (Contratos == "0") Contratos = "";
            if (Estados == "0") Estados = "";

            string grupo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(IdGrupo));
            int id, idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (grupo == "MICROINFORMATICO" || (grupo == "INFRAESTRUCTURA" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"]) == 1) || (grupo == "MOVIL" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"]) == 1))
                id = 1;
            else
                id = 2;

            var query = dbc.BuscarActivosBNV(UMinera, IdGrupo, Codigo, Usuarios, Contratos, TiposActivo, Marca, ModeloComercial, Estados, Condicion, Serie, NombreEquipo, IdAcco,idPersEnti,id).ToList();

            var total = query.Count();
            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportBNV()
        {
            string IdGrupo = Request.Params["IdGrupo"].ToString();
            if (String.IsNullOrEmpty(IdGrupo)) IdGrupo = "0";

            if (IdGrupo == "0")
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone();}window.onload=init;</script>");

            string UMinera = Request.Params["UMinera"].ToString();
            string Codigo = Request.Params["COD_ASSE"].ToString();
            string Marca = Request.Params["ID_MANU"].ToString();
            //string Estado = Request.Params["ID_STAT_ASSE"].ToString();
            string NombreEquipo = Request.Params["NAM_EQUI"].ToString();
            string Serie = Request.Params["SER_NUMB"].ToString();
            string Usuarios = Request.Params["Usuarios"].ToString();
            string Contratos = Request.Params["Contratos"].ToString();
            string Estados = Request.Params["Estados"].ToString();
            string TiposActivo = Convert.ToString(Request.Params["ExpTypeAsset"].ToString());
            string ModeloComercial = Convert.ToString(Request.Params["ID_COMM_MODE"] == null ? "0" : Request.Params["ID_COMM_MODE"]);
            string Condicion = Convert.ToString(Request.Params["ID_COND"] == null ? "0" : Request.Params["ID_COND"]);

            if (String.IsNullOrEmpty(UMinera)) UMinera = "0";
            if (String.IsNullOrEmpty(Marca)) Marca = "0";
            //if (String.IsNullOrEmpty(Estado)) Estado = "0";
            if (String.IsNullOrEmpty(ModeloComercial)) ModeloComercial = "0";
            if (String.IsNullOrEmpty(Condicion)) Condicion = "0";

            int IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());

            //Selección múltiple
            if (String.IsNullOrEmpty(TiposActivo)) TiposActivo = "0";
            TiposActivo = TiposActivo.Substring(0, TiposActivo.Length - 1);
            if (String.IsNullOrEmpty(Usuarios)) Usuarios = "0";
            Usuarios = Usuarios.Substring(0, Usuarios.Length - 1);
            if (String.IsNullOrEmpty(Contratos)) Contratos = "0";
            Contratos = Contratos.Substring(0, Contratos.Length - 1);
            if (String.IsNullOrEmpty(Estados)) Estados = "0";
            Estados = Estados.Substring(0, Estados.Length - 1);

            if (Usuarios == "0") Usuarios = "";
            if (Contratos == "0") Contratos = "";
            if (Estados == "0") Estados = "";

            string grupoActivo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(IdGrupo));
            int id, idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (grupoActivo == "MICROINFORMATICO" || (grupoActivo == "INFRAESTRUCTURA" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"]) == 1) || (grupoActivo == "MOVIL" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"]) == 1))
                id = 1;
            else
                id = 2;

            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Detalle");

            var query = dbc.BuscarActivosBNV(UMinera, IdGrupo, Codigo, Usuarios, Contratos, TiposActivo, Marca, ModeloComercial, Estados, Condicion, Serie, NombreEquipo, IdAcco, idPersEnti, id)
                .OrderBy(x => x.UnidadMinera)
                .ToList();

            if (grupoActivo == "MICROINFORMATICO")
            {
                worksheet.Cell(1, 1).Value = "GRUPO";
                worksheet.Cell(1, 2).Value = "UNIDAD MINERA";
                worksheet.Cell(1, 3).Value = "SERIE";
                worksheet.Cell(1, 4).Value = "NOMBRE";
                worksheet.Cell(1, 5).Value = "TIPO ACTIVO";
                worksheet.Cell(1, 6).Value = "MARCA";
                worksheet.Cell(1, 7).Value = "MODELO";
                worksheet.Cell(1, 8).Value = "SITIO";
                worksheet.Cell(1, 9).Value = "LOCACION";
                worksheet.Cell(1, 10).Value = "USUARIO ASIGNADO";
                worksheet.Cell(1, 11).Value = "AREA";
                worksheet.Cell(1, 12).Value = "ESTADO";
                worksheet.Cell(1, 13).Value = "CONDICION";
                worksheet.Cell(1, 14).Value = "PROYECTO";
                worksheet.Cell(1, 15).Value = "TARIFA";

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                    worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
                }

                for (int i = 0; i < query.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = query[i].Grupo;
                    worksheet.Cell(i + 2, 2).Value = query[i].UnidadMinera;
                    worksheet.Cell(i + 2, 3).Value = query[i].Serie;
                    worksheet.Cell(i + 2, 4).Value = query[i].NombreActivo;
                    worksheet.Cell(i + 2, 5).Value = query[i].TipoActivo;
                    worksheet.Cell(i + 2, 6).Value = query[i].Marca;
                    worksheet.Cell(i + 2, 7).Value = query[i].ModeloComercial;
                    worksheet.Cell(i + 2, 8).Value = query[i].Sitio;
                    worksheet.Cell(i + 2, 9).Value = query[i].Locacion;
                    worksheet.Cell(i + 2, 10).Value = query[i].UsuarioAsignado;
                    worksheet.Cell(i + 2, 11).Value = query[i].Area;
                    worksheet.Cell(i + 2, 12).Value = query[i].Estado;
                    worksheet.Cell(i + 2, 13).Value = query[i].Condicion;
                    worksheet.Cell(i + 2, 14).Value = query[i].Contrato;
                    worksheet.Cell(i + 2, 15).Value = query[i].Costo;
                }

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }

            }
            else if (grupoActivo == "INFRAESTRUCTURA")
            {
                worksheet.Cell(1, 1).Value = "GRUPO";
                worksheet.Cell(1, 2).Value = "UNIDAD MINERA";
                worksheet.Cell(1, 3).Value = "SITIO";
                worksheet.Cell(1, 4).Value = "LOCACION";
                worksheet.Cell(1, 5).Value = "USUARIO ASIGNADO";
                worksheet.Cell(1, 6).Value = "AREA";
                worksheet.Cell(1, 7).Value = "TIPO ACTIVO";
                worksheet.Cell(1, 8).Value = "MARCA";
                worksheet.Cell(1, 9).Value = "MODELO COMERCIAL";
                worksheet.Cell(1, 10).Value = "SERIE";
                worksheet.Cell(1, 11).Value = "NOMBRE ACTIVO";
                worksheet.Cell(1, 12).Value = "IP LOCAL";
                worksheet.Cell(1, 13).Value = "ESTADO";
                worksheet.Cell(1, 14).Value = "CONDICION";
                worksheet.Cell(1, 15).Value = "COMENTARIOS";
                worksheet.Cell(1, 16).Value = "MODO COMPRA";
                worksheet.Cell(1, 17).Value = "CONTRATO";
                worksheet.Cell(1, 18).Value = "MAC FISICA";

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                    worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
                }

                for (int i = 0; i < query.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = query[i].Grupo;
                    worksheet.Cell(i + 2, 2).Value = query[i].UnidadMinera;
                    worksheet.Cell(i + 2, 3).Value = query[i].Sitio;
                    worksheet.Cell(i + 2, 4).Value = query[i].Locacion;
                    worksheet.Cell(i + 2, 5).Value = query[i].UsuarioAsignado;
                    worksheet.Cell(i + 2, 6).Value = query[i].Area;
                    worksheet.Cell(i + 2, 7).Value = query[i].TipoActivo;
                    worksheet.Cell(i + 2, 8).Value = query[i].Marca;
                    worksheet.Cell(i + 2, 9).Value = query[i].ModeloComercial;
                    worksheet.Cell(i + 2, 10).Value = query[i].Serie;
                    worksheet.Cell(i + 2, 11).Value = query[i].NombreActivo;
                    worksheet.Cell(i + 2, 12).Value = query[i].IpLocal;
                    worksheet.Cell(i + 2, 13).Value = query[i].Estado;
                    worksheet.Cell(i + 2, 14).Value = query[i].Condicion;
                    worksheet.Cell(i + 2, 15).Value = (query[i].Resumen != null ? Regex.Replace(query[i].Resumen, "<.*?>", " ") : string.Empty);
                    worksheet.Cell(i + 2, 16).Value = query[i].ModoCompra;
                    worksheet.Cell(i + 2, 17).Value = query[i].Contrato;
                    worksheet.Cell(i + 2, 18).Value = query[i].MacFisica;
                }

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }

            }
            else if (grupoActivo == "MOVIL")
            {
                worksheet.Cell(1, 1).Value = "GRUPO";
                worksheet.Cell(1, 2).Value = "UNIDAD MINERA";
                worksheet.Cell(1, 3).Value = "SITIO";
                worksheet.Cell(1, 4).Value = "LOCACION";
                worksheet.Cell(1, 5).Value = "USUARIO ASIGNADO";
                worksheet.Cell(1, 6).Value = "AREA";
                worksheet.Cell(1, 7).Value = "TIPO ACTIVO";
                worksheet.Cell(1, 8).Value = "MARCA";
                worksheet.Cell(1, 9).Value = "MODELO COMERCIAL";
                worksheet.Cell(1, 10).Value = "SERIE";
                worksheet.Cell(1, 11).Value = "IMEI";
                worksheet.Cell(1, 12).Value = "ANEXO";
                worksheet.Cell(1, 13).Value = "ESTADO";
                worksheet.Cell(1, 14).Value = "CONDICION";
                worksheet.Cell(1, 15).Value = "COMENTARIOS";
                worksheet.Cell(1, 16).Value = "MODALIDAD";
                worksheet.Cell(1, 17).Value = "CONTRATO";
                worksheet.Cell(1, 18).Value = "FECHA INICIO";
                worksheet.Cell(1, 19).Value = "FECHA RENOVACION";
                worksheet.Cell(1, 20).Value = "OPERADOR";
                worksheet.Cell(1, 21).Value = "PLAN";
                worksheet.Cell(1, 22).Value = "MONTO MENSUAL";
                worksheet.Cell(1, 23).Value = "MONTO TOTAL";
                worksheet.Cell(1, 24).Value = "ANTIVIRUS";
                worksheet.Cell(1, 25).Value = "APLICACIONES";

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                    worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
                }

                for (int i = 0; i < query.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = query[i].Grupo;
                    worksheet.Cell(i + 2, 2).Value = query[i].UnidadMinera;
                    worksheet.Cell(i + 2, 3).Value = query[i].Sitio;
                    worksheet.Cell(i + 2, 4).Value = query[i].Locacion;
                    worksheet.Cell(i + 2, 5).Value = query[i].UsuarioAsignado;
                    worksheet.Cell(i + 2, 6).Value = query[i].Area;
                    worksheet.Cell(i + 2, 7).Value = query[i].TipoActivo;
                    worksheet.Cell(i + 2, 8).Value = query[i].Marca;
                    worksheet.Cell(i + 2, 9).Value = query[i].ModeloComercial;
                    worksheet.Cell(i + 2, 10).Value = query[i].Serie;
                    worksheet.Cell(i + 2, 11).Value = query[i].NombreActivo;
                    worksheet.Cell(i + 2, 12).Value = query[i].IpLocal;
                    worksheet.Cell(i + 2, 13).Value = query[i].Estado;
                    worksheet.Cell(i + 2, 14).Value = query[i].Condicion;
                    worksheet.Cell(i + 2, 15).Value = (query[i].Resumen != null ? Regex.Replace(query[i].Resumen, "<.*?>", " ") : string.Empty);
                    worksheet.Cell(i + 2, 16).Value = query[i].Modalidad;
                    worksheet.Cell(i + 2, 17).Value = query[i].Contrato;
                    worksheet.Cell(i + 2, 18).Value = query[i].ContratoInicio;
                    worksheet.Cell(i + 2, 19).Value = query[i].ContratoFin;
                    worksheet.Cell(i + 2, 20).Value = query[i].Operador;
                    worksheet.Cell(i + 2, 21).Value = query[i].Plan;
                    worksheet.Cell(i + 2, 22).Value = query[i].Costo;
                    worksheet.Cell(i + 2, 23).Value = query[i].MontoTotal;
                    worksheet.Cell(i + 2, 24).Value = query[i].Antivirus;
                    worksheet.Cell(i + 2, 25).Value = query[i].Aplicaciones;
                }

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }

            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=BusquedaActivo" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                memoryStream.WriteTo(Response.OutputStream);
            }
            Response.End();

            return View();
        }

        public ActionResult DescargarReporteGlobalBNV()
        {
            string IdGrupo = Request.Params["IdGrupo"].ToString();
            if (String.IsNullOrEmpty(IdGrupo)) IdGrupo = "0";

            string UMinera = "0";
            string Codigo = "";
            string Marca = "0";
            string Estado = "";
            string NombreEquipo = "";
            string Serie = "";
            string Usuarios = "";
            string Contratos = "";
            string TiposActivo = "";
            string ModeloComercial = "0";
            string Condicion = "0";

            int IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string grupoActivo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(IdGrupo));
            int id, idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (grupoActivo == "MICROINFORMATICO" || (grupoActivo == "INFRAESTRUCTURA" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA"]) == 1) || (grupoActivo == "MOVIL" && Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MOVIL"]) == 1))
                id = 1;
            else
                id = 2;

            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Detalle");

            var query = dbc.BuscarActivosBNV(UMinera, IdGrupo, Codigo, Usuarios, Contratos, TiposActivo, Marca, ModeloComercial, Estado, Condicion, Serie, NombreEquipo, IdAcco, idPersEnti, id)
                .OrderBy(x => x.UnidadMinera)
                .ToList();

            if (grupoActivo == "MICROINFORMATICO")
            {
                worksheet.Cell(1, 1).Value = "GRUPO";
                worksheet.Cell(1, 2).Value = "UNIDAD MINERA";
                worksheet.Cell(1, 3).Value = "SERIE";
                worksheet.Cell(1, 4).Value = "NOMBRE";
                worksheet.Cell(1, 5).Value = "TIPO ACTIVO";
                worksheet.Cell(1, 6).Value = "MARCA";
                worksheet.Cell(1, 7).Value = "MODELO";
                worksheet.Cell(1, 8).Value = "SITIO";
                worksheet.Cell(1, 9).Value = "LOCACION";
                worksheet.Cell(1, 10).Value = "USUARIO ASIGNADO";
                worksheet.Cell(1, 11).Value = "AREA";
                worksheet.Cell(1, 12).Value = "ESTADO";
                worksheet.Cell(1, 13).Value = "CONDICION";
                worksheet.Cell(1, 14).Value = "PROYECTO";
                worksheet.Cell(1, 15).Value = "TARIFA";

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                    worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
                }

                for (int i = 0; i < query.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = query[i].Grupo;
                    worksheet.Cell(i + 2, 2).Value = query[i].UnidadMinera;
                    worksheet.Cell(i + 2, 3).Value = query[i].Serie;
                    worksheet.Cell(i + 2, 4).Value = query[i].NombreActivo;
                    worksheet.Cell(i + 2, 5).Value = query[i].TipoActivo;
                    worksheet.Cell(i + 2, 6).Value = query[i].Marca;
                    worksheet.Cell(i + 2, 7).Value = query[i].ModeloComercial;
                    worksheet.Cell(i + 2, 8).Value = query[i].Sitio;
                    worksheet.Cell(i + 2, 9).Value = query[i].Locacion;
                    worksheet.Cell(i + 2, 10).Value = query[i].UsuarioAsignado;
                    worksheet.Cell(i + 2, 11).Value = query[i].Area;
                    worksheet.Cell(i + 2, 12).Value = query[i].Estado;
                    worksheet.Cell(i + 2, 13).Value = query[i].Condicion;
                    worksheet.Cell(i + 2, 14).Value = query[i].Contrato;
                    worksheet.Cell(i + 2, 15).Value = query[i].Costo;
                }

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }

            }
            else if (grupoActivo == "INFRAESTRUCTURA")
            {
                worksheet.Cell(1, 1).Value = "GRUPO";
                worksheet.Cell(1, 2).Value = "UNIDAD MINERA";
                worksheet.Cell(1, 3).Value = "SITIO";
                worksheet.Cell(1, 4).Value = "LOCACION";
                worksheet.Cell(1, 5).Value = "USUARIO ASIGNADO";
                worksheet.Cell(1, 6).Value = "AREA";
                worksheet.Cell(1, 7).Value = "TIPO ACTIVO";
                worksheet.Cell(1, 8).Value = "MARCA";
                worksheet.Cell(1, 9).Value = "MODELO COMERCIAL";
                worksheet.Cell(1, 10).Value = "SERIE";
                worksheet.Cell(1, 11).Value = "NOMBRE ACTIVO";
                worksheet.Cell(1, 12).Value = "IP LOCAL";
                worksheet.Cell(1, 13).Value = "ESTADO";
                worksheet.Cell(1, 14).Value = "CONDICION";
                worksheet.Cell(1, 15).Value = "COMENTARIOS";
                worksheet.Cell(1, 16).Value = "MODO COMPRA";
                worksheet.Cell(1, 17).Value = "CONTRATO";
                worksheet.Cell(1, 18).Value = "MAC FISICA";

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                    worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
                }

                for (int i = 0; i < query.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = query[i].Grupo;
                    worksheet.Cell(i + 2, 2).Value = query[i].UnidadMinera;
                    worksheet.Cell(i + 2, 3).Value = query[i].Sitio;
                    worksheet.Cell(i + 2, 4).Value = query[i].Locacion;
                    worksheet.Cell(i + 2, 5).Value = query[i].UsuarioAsignado;
                    worksheet.Cell(i + 2, 6).Value = query[i].Area;
                    worksheet.Cell(i + 2, 7).Value = query[i].TipoActivo;
                    worksheet.Cell(i + 2, 8).Value = query[i].Marca;
                    worksheet.Cell(i + 2, 9).Value = query[i].ModeloComercial;
                    worksheet.Cell(i + 2, 10).Value = query[i].Serie;
                    worksheet.Cell(i + 2, 11).Value = query[i].NombreActivo;
                    worksheet.Cell(i + 2, 12).Value = query[i].IpLocal;
                    worksheet.Cell(i + 2, 13).Value = query[i].Estado;
                    worksheet.Cell(i + 2, 14).Value = query[i].Condicion;
                    worksheet.Cell(i + 2, 15).Value = (query[i].Resumen != null ? Regex.Replace(query[i].Resumen, "<.*?>", " ") : string.Empty);
                    worksheet.Cell(i + 2, 16).Value = query[i].ModoCompra;
                    worksheet.Cell(i + 2, 17).Value = query[i].Contrato;
                    worksheet.Cell(i + 2, 18).Value = query[i].MacFisica;
                }

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }

            }
            else if (grupoActivo == "MOVIL")
            {
                worksheet.Cell(1, 1).Value = "GRUPO";
                worksheet.Cell(1, 2).Value = "UNIDAD MINERA";
                worksheet.Cell(1, 3).Value = "SITIO";
                worksheet.Cell(1, 4).Value = "LOCACION";
                worksheet.Cell(1, 5).Value = "USUARIO ASIGNADO";
                worksheet.Cell(1, 6).Value = "AREA";
                worksheet.Cell(1, 7).Value = "TIPO ACTIVO";
                worksheet.Cell(1, 8).Value = "MARCA";
                worksheet.Cell(1, 9).Value = "MODELO COMERCIAL";
                worksheet.Cell(1, 10).Value = "SERIE";
                worksheet.Cell(1, 11).Value = "IMEI";
                worksheet.Cell(1, 12).Value = "ANEXO";
                worksheet.Cell(1, 13).Value = "ESTADO";
                worksheet.Cell(1, 14).Value = "CONDICION";
                worksheet.Cell(1, 15).Value = "COMENTARIOS";
                worksheet.Cell(1, 16).Value = "MODALIDAD";
                worksheet.Cell(1, 17).Value = "CONTRATO";
                worksheet.Cell(1, 18).Value = "FECHA INICIO";
                worksheet.Cell(1, 19).Value = "FECHA RENOVACION";
                worksheet.Cell(1, 20).Value = "OPERADOR";
                worksheet.Cell(1, 21).Value = "PLAN";
                worksheet.Cell(1, 22).Value = "MONTO MENSUAL";
                worksheet.Cell(1, 23).Value = "MONTO TOTAL";
                worksheet.Cell(1, 24).Value = "ANTIVIRUS";
                worksheet.Cell(1, 25).Value = "APLICACIONES";

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                    worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
                }

                for (int i = 0; i < query.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = query[i].Grupo;
                    worksheet.Cell(i + 2, 2).Value = query[i].UnidadMinera;
                    worksheet.Cell(i + 2, 3).Value = query[i].Sitio;
                    worksheet.Cell(i + 2, 4).Value = query[i].Locacion;
                    worksheet.Cell(i + 2, 5).Value = query[i].UsuarioAsignado;
                    worksheet.Cell(i + 2, 6).Value = query[i].Area;
                    worksheet.Cell(i + 2, 7).Value = query[i].TipoActivo;
                    worksheet.Cell(i + 2, 8).Value = query[i].Marca;
                    worksheet.Cell(i + 2, 9).Value = query[i].ModeloComercial;
                    worksheet.Cell(i + 2, 10).Value = query[i].Serie;
                    worksheet.Cell(i + 2, 11).Value = query[i].NombreActivo;
                    worksheet.Cell(i + 2, 12).Value = query[i].IpLocal;
                    worksheet.Cell(i + 2, 13).Value = query[i].Estado;
                    worksheet.Cell(i + 2, 14).Value = query[i].Condicion;
                    worksheet.Cell(i + 2, 15).Value = (query[i].Resumen != null ? Regex.Replace(query[i].Resumen, "<.*?>", " ") : string.Empty);
                    worksheet.Cell(i + 2, 16).Value = query[i].Modalidad;
                    worksheet.Cell(i + 2, 17).Value = query[i].Contrato;
                    worksheet.Cell(i + 2, 18).Value = query[i].ContratoInicio;
                    worksheet.Cell(i + 2, 19).Value = query[i].ContratoFin;
                    worksheet.Cell(i + 2, 20).Value = query[i].Operador;
                    worksheet.Cell(i + 2, 21).Value = query[i].Plan;
                    worksheet.Cell(i + 2, 22).Value = query[i].Costo;
                    worksheet.Cell(i + 2, 23).Value = query[i].MontoTotal;
                    worksheet.Cell(i + 2, 24).Value = query[i].Antivirus;
                    worksheet.Cell(i + 2, 25).Value = query[i].Aplicaciones;
                }

                for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }

            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                var bytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(bytes);
                return Content(base64String, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        public ActionResult DescargarReporteCelularBNV()
        {
            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Detalle");

            var query = dbc.ExportarReporteActivoCelular(3103).ToList();

            worksheet.Cell(1, 1).Value = "GRUPO";
            worksheet.Cell(1, 2).Value = "UNIDAD MINERA";
            worksheet.Cell(1, 3).Value = "SERIE";
            worksheet.Cell(1, 4).Value = "NOMBRE";
            worksheet.Cell(1, 5).Value = "TIPO ACTIVO";
            worksheet.Cell(1, 6).Value = "MARCA";
            worksheet.Cell(1, 7).Value = "MODELO";
            worksheet.Cell(1, 8).Value = "SITIO";
            worksheet.Cell(1, 9).Value = "LOCACION";
            worksheet.Cell(1, 10).Value = "USUARIO ASIGNADO";
            worksheet.Cell(1, 11).Value = "AREA";
            worksheet.Cell(1, 12).Value = "CELULAR";
            worksheet.Cell(1, 13).Value = "NUMERO";
            worksheet.Cell(1, 14).Value = "ESTADO";
            worksheet.Cell(1, 15).Value = "CONDICION";
            worksheet.Cell(1, 16).Value = "PROYECTO";
            worksheet.Cell(1, 17).Value = "TARIFA";

            for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
            {
                worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
            }

            for (int i = 0; i < query.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = query[i].Grupo;
                worksheet.Cell(i + 2, 2).Value = query[i].UnidadMinera;
                worksheet.Cell(i + 2, 3).Value = query[i].Serie;
                worksheet.Cell(i + 2, 4).Value = query[i].NombreActivo;
                worksheet.Cell(i + 2, 5).Value = query[i].TipoActivo;
                worksheet.Cell(i + 2, 6).Value = query[i].Marca;
                worksheet.Cell(i + 2, 7).Value = query[i].ModeloComercial;
                worksheet.Cell(i + 2, 8).Value = query[i].Sitio;
                worksheet.Cell(i + 2, 9).Value = query[i].Locacion;
                worksheet.Cell(i + 2, 10).Value = query[i].UsuarioAsignado;
                worksheet.Cell(i + 2, 11).Value = query[i].Area;
                worksheet.Cell(i + 2, 12).Value = query[i].LineaMovil != null ? "SI" : "";
                worksheet.Cell(i + 2, 13).Value = query[i].LineaMovil;
                worksheet.Cell(i + 2, 14).Value = query[i].Estado;
                worksheet.Cell(i + 2, 15).Value = query[i].Condicion;
                worksheet.Cell(i + 2, 16).Value = query[i].Contrato;
                worksheet.Cell(i + 2, 17).Value = query[i].Costo;
            }

            for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
            {
                worksheet.Column(i).AdjustToContents();
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                var bytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(bytes);
                return Content(base64String, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        public ActionResult UMinera(int id = 0)
        {
            ViewBag.ID_ASSE = id;
            int idSede = 0;
            var asset = dbc.ASSETs.Single(a => a.ID_ASSE == id);
            var clieAsse = dbc.CLIENT_ASSET.FirstOrDefault(x => x.ID_ASSE == id && x.DAT_END == null);
            var loca = dbc.LOCATIONs.FirstOrDefault(x => x.ID_LOCA == clieAsse.ID_LOCA);
            var estado = dbc.CONDITIONs.FirstOrDefault(x => x.ID_COND == clieAsse.ID_COND);
            string grupo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(asset.IdGrupo));

            if (loca != null)
                idSede = Convert.ToInt32(loca.ID_SITE);

            ViewBag.IdGrupo = asset.IdGrupo;
            ViewBag.Grupo = grupo;
            ViewBag.IdSede = idSede;
            ViewBag.IdUMinera = asset.UMinera;
            ViewBag.IdLoca = clieAsse.ID_LOCA;
            ViewBag.IdEstadoAct = estado.ID_STAT_ASSE;
            return View();
        }

        public ActionResult ActualizarUMinera()
        {
            try
            {
                int idActivo = Convert.ToInt32(Request.Params["id"].ToString());
                int idUMinera = Convert.ToInt32(Request.Params["uMinera"].ToString());
                int UserId = Convert.ToInt32(Session["UserId"].ToString());
                int idLoca = (Request.Params["Locacion"] != "" && Request.Params["Locacion"] != null) ? Convert.ToInt32(Request.Params["Locacion"]) : 0;

                dbc.ActualizarUMineraLocacionBNV(idActivo, idUMinera, UserId, idLoca);
            }
            catch
            {
                return Content("Error");
            }

            return Content("Ok");
        }

        public ActionResult EditarBNV(int id = 0)
        {
            ASSET objActivo = dbc.ASSETs.Single(a => a.ID_ASSE == id);
            if (objActivo == null)
            {
                return HttpNotFound();
            }

            string grupo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(objActivo.IdGrupo));
            ViewBag.Grupo = grupo;

            string nomContra = "";
            var contra = dbc.Contratoes.FirstOrDefault(x => x.Id == objActivo.IdContrato);

            if (contra != null)
                nomContra = contra.Nombre;

            if (grupo == "MOVIL")
            {
                var aplicacionesInstaladas = (from aa in dbc.AplicacionMovilActivo
                             join a in dbc.AplicacionMovil on aa.IdAplicacion equals a.Id
                             where aa.Estado == true && aa.IdActivo == id
                             select new
                             {
                                 id = aa.IdAplicacion,
                                 text = a.Nombre
                             }).ToList();

                ViewBag.AplicacionesInstaladas = aplicacionesInstaladas;
            }

            string tipoAct = dbc.TYPE_ASSET.FirstOrDefault(x => x.ID_TYPE_ASSE == objActivo.ID_TYPE_ASSE).NAM_TYPE_ASSE;
            ViewBag.TipoAct = tipoAct;
            ViewBag.Proyecto = nomContra;
            
            return View(objActivo);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditBNV(IEnumerable<HttpPostedFileBase> files, ASSET asset)
        {
            if (ModelState.IsValid)
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int userId = Convert.ToInt32(Session["UserId"]);
                string mensajeE = "";
                var objActivo = dbc.ASSETs.Where(x => x.ID_ASSE == asset.ID_ASSE);
                int idDaaS = dbc.Contratoes.FirstOrDefault(x => x.ID_ACCO == 60 && x.Estado == true && x.Nombre == "DaaS").Id;
                int idAlqui = dbc.Contratoes.FirstOrDefault(x => x.ID_ACCO == 60 && x.Estado == true && x.Nombre == "Alquiler").Id;

                if (asset.IdGrupo == null)
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','Seleccione el grupo.',0);}window.onload=init;</script>");

                if (objActivo == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR');}window.onload=init;</script>");
                }

                int cantAsset = dbc.ASSETs.Where(x => x.SER_NUMB == asset.SER_NUMB && x.IdGrupo == asset.IdGrupo && x.ID_ACCO == ID_ACCO && x.ID_ASSE != asset.ID_ASSE).Count();
                if (cantAsset > 0)
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('INFO','La serie ingresada ya existe. Por favor validar.',0);}window.onload=init;</script>");

                string nomGrupo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(asset.IdGrupo));

                if (asset.ID_TYPE_ASSE == 0 || asset.ID_TYPE_ASSE == null)
                    mensajeE += "Seleccione el tipo de activo. <br/>";
                if (asset.SER_NUMB == "" || asset.SER_NUMB == null)
                    mensajeE += "Ingrese el número de serie. <br/>";
                if (asset.ID_MANU == 0 || asset.ID_MANU == null)
                    mensajeE += "Seleccione la marca del activo. <br/>";
                if (asset.ID_COMM_MODE == 0 || asset.ID_COMM_MODE == null)
                    mensajeE += "Seleccione el modelo del activo. <br/>";

                if (nomGrupo == "MICROINFORMATICO")
                {
                    if (asset.IdContrato == 0 || asset.IdContrato == null)
                        mensajeE += "Ingrese el proyecto del equipo. <br/>";
                    if (asset.IdContrato == idDaaS && (asset.ACQ_DATE == null || asset.FechaFin == null))
                        mensajeE += "Seleccione las fechas. <br/>";
                    if (asset.IdContrato == idAlqui && asset.ACQ_DATE == null)
                        mensajeE += "Seleccione la fecha de inicio. <br/>";
                }
                else if (nomGrupo == "INFRAESTRUCTURA")
                {
                    if (asset.ID_BUY_MODE == 0 || asset.ID_BUY_MODE == null)
                        mensajeE += "Seleccione el modo de compra. <br/>";
                }
                else if (nomGrupo == "MOVIL")
                {
                    if (asset.IdContrato == null)
                    {
                        mensajeE += "Seleccione el contrato. <br/>";
                    }
                }

                if (mensajeE != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','" + mensajeE + "');}window.onload=init;</script>");
                }

                if (nomGrupo == "MOVIL")
                {
                    var antivirus = Request.Params["AntivirusMovil"];
                    asset.Antivirus = antivirus == "on";
                }

                asset.MODIFIED_DATE = DateTime.Now;
                dbc.ASSETs.Attach(asset);
                //db.ObjectStateManager.ChangeObjectState(asset, EntityState.Modified);                
                dbc.Entry(asset).State = EntityState.Modified;
                dbc.SaveChanges();

                //Actualizar aplicaciones movil
                if (nomGrupo == "MOVIL")
                {
                    var aplicacionesSelect = Request.Params["aplicacionesMovil"];
                    dbc.ActualizaAplicacionMovilActivo(asset.ID_ASSE, aplicacionesSelect, userId);
                }

                var objClientAsset = dbc.CLIENT_ASSET.Where(x => x.ID_ASSE == asset.ID_ASSE && x.DAT_END == null).FirstOrDefault();

                if (objClientAsset != null)
                {
                    objClientAsset.UserId = userId;
                    objClientAsset.DAT_STAR = DateTime.Now;
                    objClientAsset.DAT_END = DateTime.Now;
                    objClientAsset.CREATE_DATE = DateTime.Now;
                    objClientAsset.ID_TYPE_CLIE_ASSE = 5;
                    objClientAsset.SUM_CLIE_ASSE = asset.SUM_ASSE;
                    dbc.CLIENT_ASSET.Add(objClientAsset);
                    dbc.SaveChanges();
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_CLIE_ASSE = objClientAsset.ID_CLIE_ASSE;
                            ATTA.ID_ASSE = objClientAsset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR');}window.onload=init;</script>");
        }

        public ActionResult ListarLocacionBNV(int idGrupo = 0)
        {
            var IdUMinera = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            var result = dbc.ListarLocacionesBNV(idGrupo, IdUMinera).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSedesBNV(int idGrupo = 0)
        {
            var IdUMinera = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            var result = dbc.ListarSedesBNV(idGrupo, IdUMinera).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLocacionUsuarioBNV()
        {
            var IdPers = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            int idGrupo = ObtenerIdGrupoUsuarioBNV();
            int idUMinera = Convert.ToInt32(dbe.PERSON_ENTITY.FirstOrDefault(x => x.ID_PERS_ENTI == IdPers).ID_ENTI1);
            var result = dbc.ListarLocacionesBNV(idGrupo, idUMinera).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarLocacionBNV()
        {
            int id = 0;

            try
            {
                string nomLoca = Request.Params["NomLoca"].ToString();
                int uMinera = Convert.ToInt32(Request.Params["idUMinera"].ToString());
                int idSede = Convert.ToInt32(Request.Params["idSede"].ToString());
                int UserId = Convert.ToInt32(Session["UserId"].ToString());
                int idGrupoAct = Convert.ToInt32(Request.Params["idGrupo"].ToString());

                var loca = dbc.LOCATIONs.FirstOrDefault(x => x.NAM_LOCA.ToUpper() == nomLoca.ToUpper() && x.ID_SITE == idSede);
                if (loca != null)
                {
                    id = loca.ID_LOCA;
                }
                else
                {
                    LOCATION newLoca = new LOCATION();
                    newLoca.NAM_LOCA = nomLoca.ToUpper();
                    newLoca.ID_SITE = idSede;
                    newLoca.VIG_LOCA = false;
                    dbc.LOCATIONs.Add(newLoca);
                    dbc.SaveChanges();
                    id = Convert.ToInt32(newLoca.ID_LOCA);
                }

                int exis = dbc.LocacionxGrupo.Where(x => x.IdLocacion == id && x.IdGrupoActivo == idGrupoAct && x.UMinera == uMinera).Count();
                if (exis == 0)
                {
                    LocacionxGrupo locaGru = new LocacionxGrupo();
                    locaGru.IdLocacion = id;
                    locaGru.IdGrupoActivo = idGrupoAct;
                    locaGru.UMinera = uMinera;
                    locaGru.UsuarioCreacion = UserId;
                    locaGru.FechaCreacion = DateTime.Now;
                    locaGru.Estado = true;
                    dbc.LocacionxGrupo.Add(locaGru);
                    dbc.SaveChanges();
                }
            }
            catch
            {
                return Content("ERROR");
            }

            return Content(id.ToString());
        }

        public ActionResult GuardarSedeBNV()
        {
            int id = 0;

            try
            {
                string nomSede = Request.Params["NomSede"].ToString();
                int uMinera = Convert.ToInt32(Request.Params["idUMinera"].ToString());
                int UserId = Convert.ToInt32(Session["UserId"].ToString());
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int idGrupoAct = Convert.ToInt32(Request.Params["idGrupo"].ToString());

                var sede = dbc.SITEs.FirstOrDefault(x => x.NAM_SITE.ToUpper() == nomSede.ToUpper() && x.ID_ACCO == ID_ACCO);
                if (sede != null)
                {
                    id = sede.ID_SITE;
                }
                else
                {
                    SITE newSite = new SITE();
                    newSite.NAM_SITE = nomSede.ToUpper();
                    newSite.ID_ACCO = ID_ACCO;
                    dbc.SITEs.Add(newSite);
                    dbc.SaveChanges();
                    id = Convert.ToInt32(newSite.ID_SITE);
                }

                int exis = dbc.SedexGrupo.Where(x => x.IdSede == id && x.IdGrupoActivo == idGrupoAct && x.UMinera == uMinera).Count();
                if (exis == 0)
                {
                    SedexGrupo sedeGru = new SedexGrupo();
                    sedeGru.IdSede = id;
                    sedeGru.IdGrupoActivo = idGrupoAct;
                    sedeGru.UMinera = uMinera;
                    sedeGru.UsuarioCreacion = UserId;
                    sedeGru.FechaCreacion = DateTime.Now;
                    sedeGru.Estado = true;
                    dbc.SedexGrupo.Add(sedeGru);
                    dbc.SaveChanges();
                }
            }
            catch
            {
                return Content("ERROR");
            }

            return Content(id.ToString());
        }

        public ActionResult ListarAfectadosxUMinBNV()
        {
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);
            string txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            int idUMinera;

            if (field == "CLIE")
                idUMinera = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
            else
                idUMinera = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);


            var query = (from pe in dbe.PERSON_ENTITY
                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                         where pe.ID_ENTI1 == idUMinera && pe.VIG_PERS_ENTI == true && ce.VIG_ENTI == true
                         select new
                         {
                             CLIE = ((ce.FIR_NAME ?? "") + " " + (ce.SEC_NAME ?? "") + " " + (ce.LAS_NAME ?? "") + " " + (ce.MOT_NAME ?? "")).Trim().ToUpper(),
                             pe.ID_PERS_ENTI,
                             pe.ID_PRIO,
                             ID_AREA = pe.ID_AREA ?? 0
                         });

            if (field == "CLIE")
                query = query.Where(x => x.CLIE.Contains(txt));

            var result = query.OrderBy(x => x.CLIE).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAfectadosxGrupoBNV(int id = 0, int idUMin = 0, int idGrupo = 0)
        {
            if (id == 1) //Todos
            {
                var listaAsignados = dbe.ListarUsuarioAfectadoBuenaventura("").ToList();
                return Json(new { Data = listaAsignados, Count = listaAsignados.Count() }, JsonRequestBehavior.AllowGet);
            }
            else if (id == 3) //Filtro por excepcion de usuarios
            {
                var usuarios = dbe.ListarUsuarioFiltroExcepcion(idGrupo, "").ToList();
                return Json(new { Data = usuarios, Count = usuarios.Count() }, JsonRequestBehavior.AllowGet);
            }
            else if (id == 4) //Filtro por usuarios cesados
            {
                var usuarios = dbe.ListarUsuariosCesadosConActivos(idGrupo, "").ToList();
                return Json(new { Data = usuarios, Count = usuarios.Count() }, JsonRequestBehavior.AllowGet);
            }
            else //Filtro por Unidad Minera
            {
                var query = (from pe in dbe.PERSON_ENTITY
                             join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                             where pe.ID_ENTI1 == idUMin && pe.VIG_PERS_ENTI == true && ce.VIG_ENTI == true
                             select new
                             {
                                 CLIE = ((ce.FIR_NAME ?? "") + " " + (ce.SEC_NAME ?? "") + " " + (ce.LAS_NAME ?? "") + " " + (ce.MOT_NAME ?? "")).Trim().ToUpper(),
                                 pe.ID_PERS_ENTI,
                                 pe.ID_PRIO,
                                 ID_AREA = pe.ID_AREA ?? 0
                             });

                var result = query.OrderBy(x => x.CLIE).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListadoExcepUsuariosBNV(int IdGrupo = 0, int IdUsuario = 0)
        {
            var response = dbe.ListadoUsuarioFiltroExcepcion(IdGrupo, IdUsuario).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearExcepUsuarioBNV(int id = 0, int idUsuario = 0)
        {
            try
            {
                int exis = dbe.UsuariosFiltroExcepcion.Where(x => x.IdGrupo == id && x.IdPersEnti == idUsuario).Count();
                if (exis == 0)
                {
                    var usuario = new UsuariosFiltroExcepcion();
                    usuario.IdPersEnti = idUsuario;
                    usuario.IdGrupo = id;
                    usuario.UsuarioCreacion = Convert.ToInt32(Session["UserId"].ToString()); ;
                    usuario.FechaCreacion = DateTime.Now;
                    usuario.Estado = true;
                    dbe.UsuariosFiltroExcepcion.Add(usuario);
                    dbe.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ModificarEstadoExcepUsuarioBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var usuario = dbe.UsuariosFiltroExcepcion.FirstOrDefault(x => x.Id == id);
                if (usuario != null)
                {
                    usuario.Estado = Convert.ToBoolean(idEstado);
                    usuario.UsuarioModifica = Convert.ToInt32(Session["UserId"].ToString());
                    usuario.FechaModifica = DateTime.Now;
                    dbe.UsuariosFiltroExcepcion.Attach(usuario);
                    dbe.Entry(usuario).State = EntityState.Modified;
                    dbe.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult vFlujoCorte()
        {
            int idGrupo = ObtenerIdGrupoUsuarioBNV();
            ViewBag.IdGrupo = idGrupo;
            return View();
        }

        public ActionResult DescargarCorteFlujo()
        {
            string fec = Request.Form["FechaRF"].ToString();
            string uMin = Request.Form["UMineraRF"].ToString();
            int IdGrupo = Convert.ToInt32(Request.Params["IdGrupo"]);

            DateTime fecha;
            int uMinera;

            if (!DateTime.TryParseExact(fec, "MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha)
                || !int.TryParse(uMin, out uMinera))
            {
                return Json(new { Estado = "ERROR", Msg = "Complete la información." }, JsonRequestBehavior.AllowGet);
            }

            int mes = fecha.Month;
            int año = fecha.Year;
            string nomUMin = dbc.ObtenerUMinera(uMinera).FirstOrDefault();
            string nomMes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes);
            string nameFile = "Inventario_" + nomUMin + "_" + nomMes + "_" + año + ".xlsx";

            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Detalle");

            var query = dbc.ReporteInventarioEnvioCorreo(IdGrupo, uMinera, mes, año).ToList();

            worksheet.Cell(1, 1).Value = "Cantidad";
            worksheet.Cell(1, 2).Value = "Serie";
            worksheet.Cell(1, 3).Value = "Nombre de Equipo";
            worksheet.Cell(1, 4).Value = "Fabricante";
            worksheet.Cell(1, 5).Value = "Tipo";
            worksheet.Cell(1, 6).Value = "Modelo";
            worksheet.Cell(1, 7).Value = "Organización";
            worksheet.Cell(1, 8).Value = "Mes Act.";
            worksheet.Cell(1, 9).Value = "Área";
            worksheet.Cell(1, 10).Value = "Usuario";
            worksheet.Cell(1, 11).Value = "Estado";
            worksheet.Cell(1, 12).Value = "Ubicación Anterior";
            worksheet.Cell(1, 13).Value = "Nueva Ubicación";
            worksheet.Cell(1, 14).Value = "Proyecto";
            worksheet.Cell(1, 15).Value = "Vcto.";
            worksheet.Cell(1, 16).Value = "Tarifa Nueva";
            worksheet.Cell(1, 17).Value = "Comentario";

            for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
            {
                worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
            }

            for (int i = 0; i < query.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = query[i].Cantidad;
                worksheet.Cell(i + 2, 2).Value = query[i].NumeroSerie;
                worksheet.Cell(i + 2, 3).Value = query[i].NombreActivo;
                worksheet.Cell(i + 2, 4).Value = query[i].Fabricante;
                worksheet.Cell(i + 2, 5).Value = query[i].TipoActivo;
                worksheet.Cell(i + 2, 6).Value = query[i].Modelo;
                worksheet.Cell(i + 2, 7).Value = query[i].Organizacion;
                worksheet.Cell(i + 2, 8).Value = query[i].FechaActivacion;
                worksheet.Cell(i + 2, 9).Value = query[i].Area;
                worksheet.Cell(i + 2, 10).Value = query[i].UsuarioAsignado;
                worksheet.Cell(i + 2, 11).Value = query[i].EstadoActivo;
                worksheet.Cell(i + 2, 12).Value = query[i].UbicacionAnte;
                worksheet.Cell(i + 2, 13).Value = query[i].UbicacionActual;
                worksheet.Cell(i + 2, 14).Value = query[i].Contrato;
                worksheet.Cell(i + 2, 15).Value = query[i].FechaVencimiento;
                worksheet.Cell(i + 2, 16).Value = query[i].Tarifa;
                worksheet.Cell(i + 2, 17).Value = query[i].Comentario;
            }

            for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
            {
                if (worksheet.Column(i) == worksheet.Column(2))
                {
                    worksheet.Column(i).Width = 30;
                }
                else
                {
                    worksheet.Column(i).AdjustToContents();
                }
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);

                memoryStream.WriteTo(Response.OutputStream);
            }
            Response.End();

            return Json(new { Estado = "OK", Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        public Dictionary<string, string> GruposBNV()
        {
            return new Dictionary<string, string>
                        {
                            { "GESTOR_ACTIVOS_BNV_MICROINFORMATICO", "MICROINFORMATICO" },
                            { "SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO", "MICROINFORMATICO" },
                            { "GESTOR_ACTIVOS_BNV_INFRAESTRUCTURA", "INFRAESTRUCTURA" },
                            { "SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA", "INFRAESTRUCTURA" },
                            { "GESTOR_ACTIVOS_BNV_MOVIL", "MOVIL" },
                            { "SUPERVISOR_ACTIVOS_BNV_MOVIL", "MOVIL" }
                        };
        }

        public ActionResult ListarGruposBNV(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var grupoUser = GruposBNV().Where(x => Convert.ToInt32(Session[x.Key]) == 1).ToList();
            List<dynamic> result = new List<dynamic>();

            foreach (var g in grupoUser)
            {
                int idG = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.VAL_ACCO_PARA == g.Value && x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO && x.ID_PARA == 1070).ID_ACCO_PARA;

                if (!result.Any(item => item.Id == idG))
                    result.Add(new { Id = idG, Nombre = g.Value });
            }

            if (id == 1) // Filtro Microinformatico (solo si es supervisor)
            {
                if (Convert.ToInt32(Session["SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO"]) == 0)
                    result = result.Where(item => item.Id != 3103).ToList();
            }

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarGruposSuperBNV()
        {
            var grupos = new Dictionary<string, string>
                        {
                            { "SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO", "MICROINFORMATICO" },
                            { "SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA", "INFRAESTRUCTURA" },
                            { "SUPERVISOR_ACTIVOS_BNV_MOVIL", "MOVIL" }
                        };

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var grupoUser = grupos.Where(x => Convert.ToInt32(Session[x.Key]) == 1).ToList();
            List<dynamic> result = new List<dynamic>();

            foreach (var g in grupoUser)
            {
                int id = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.VAL_ACCO_PARA == g.Value && x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO && x.ID_PARA == 1070).ID_ACCO_PARA;

                if (!result.Any(item => item.Id == id))
                    result.Add(new { Id = id, Nombre = g.Value });
            }

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public int? ObtenerUMineraUsuarioBNV(int idPers, int idGrupo)
        {
            var usuario = dbc.UMineraxUsuario.FirstOrDefault(x => x.IdPers == idPers && x.IdGrupo == idGrupo && x.Estado == true);

            if (usuario != null)
                return usuario.UMinera;
            else
                return null;
        }

        public int ObtenerIdGrupoUsuarioBNV()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string nombreGrupo = GruposBNV().FirstOrDefault(g => Convert.ToInt32(Session[g.Key]) == 1).Value;
            var grupoActivo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.VAL_ACCO_PARA == nombreGrupo && x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO && x.ID_PARA == 1070);

            if (grupoActivo != null)
                return grupoActivo.ID_ACCO_PARA;
            else
                return 0;
        }

        public string ObtenerNomGrupoUsuarioBNV(int id)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var grupo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_ACCO_PARA == id);

            if (grupo != null)
                return grupo.VAL_ACCO_PARA;
            else
                return "";
        }

        public ActionResult CargaActivos()
        {
            int cuenta = Convert.ToInt32(Session["ID_ACCO"]);

            if (cuenta == 60)
            {
                int idGrupo = ObtenerIdGrupoUsuarioBNV();

                if (idGrupo != 0)
                {
                    var can = GruposBNV().Where(x => Convert.ToInt32(Session[x.Key]) == 1).ToList().Count();
                    if (can == 1 && Convert.ToInt32(Session["GESTOR_ACTIVOS_BNV_MICROINFORMATICO"]) == 1)
                        return RedirectToAction("Index", "Error");

                    return View("CargaActivosBNV");
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult DescargarPlantillaBNV(int idGrupo)
        {
            string rutaArchivo, nombre;

            if (idGrupo == 3103)
                nombre = "CargaMasivaMicroinformatico.xlsx";
            else if (idGrupo == 4171)
                nombre = "CargaMasivaInfraestructura.xlsx";
            else
                nombre = "CargaMasivaMovil.xlsx";

            rutaArchivo = Server.MapPath("~/Adjuntos/" + nombre);

            return File(rutaArchivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombre);
        }

        [HttpPost]
        public ActionResult ValidarCargaMasivaBNV(int idGrupo, int idUMinera)
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string grupo = ObtenerNomGrupoUsuarioBNV(idGrupo);
                string msg = "OK";

                if (file != null && file.ContentLength > 0)
                {
                    using (XLWorkbook workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        int lastRow = worksheet.LastRowUsed().RowNumber();
                        int batchSize = 1000;
                        int endRow = Math.Min(batchSize + 1, lastRow);

                        var previsualizacionDatos = new List<CargaMasiva>();

                        if (grupo == "MICROINFORMATICO")
                        {
                            for (int row = 2; row <= endRow; row++)
                            {
                                var activo = new CargaMasiva()
                                {
                                    Serie = worksheet.Cell(row, 1).Value.ToString().Trim(),
                                    NombreActivo = worksheet.Cell(row, 2).Value.ToString().Trim(),
                                    TipoActivo = worksheet.Cell(row, 3).Value.ToString().Trim(),
                                    Marca = worksheet.Cell(row, 4).Value.ToString().Trim(),
                                    Modelo = worksheet.Cell(row, 5).Value.ToString().Trim(),
                                    Estado = worksheet.Cell(row, 6).Value.ToString().Trim(),
                                    Condicion = worksheet.Cell(row, 7).Value.ToString().Trim(),
                                    UsuarioAsignado = worksheet.Cell(row, 8).Value.ToString().Trim(),
                                    Sitio = worksheet.Cell(row, 9).Value.ToString().Trim(),
                                    Locacion = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                    Ubicacion = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                    Contrato = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                    FechaInicio = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                    FechaFin = worksheet.Cell(row, 14).Value.ToString().Trim(),
                                    Tarifa = worksheet.Cell(row, 15).Value.ToString().Trim(),
                                    Comentario = worksheet.Cell(row, 16).Value.ToString().Trim(),
                                };

                                activo.Errores = dbc.ValidarDatosCargaMasiva(idGrupo, idUMinera, activo.Serie, activo.TipoActivo, activo.Marca, activo.Modelo, activo.Estado, activo.Condicion, activo.UsuarioAsignado, activo.Sitio, activo.Locacion, activo.Contrato, activo.FechaInicio, activo.FechaFin, "", "", "", "", "").FirstOrDefault();

                                if (activo.Errores != "")
                                {
                                    activo.EstadoValidacion = false;
                                    msg = "ERROR";
                                }
                                else
                                {
                                    activo.EstadoValidacion = true;
                                }

                                previsualizacionDatos.Add(activo);
                            }
                        }
                        else if (grupo == "INFRAESTRUCTURA")
                        {
                            for (int row = 2; row <= endRow; row++)
                            {
                                var activo = new CargaMasiva()
                                {
                                    Serie = worksheet.Cell(row, 1).Value.ToString().Trim(),
                                    NombreActivo = worksheet.Cell(row, 2).Value.ToString().Trim(),
                                    TipoActivo = worksheet.Cell(row, 3).Value.ToString().Trim(),
                                    Marca = worksheet.Cell(row, 4).Value.ToString().Trim(),
                                    Modelo = worksheet.Cell(row, 5).Value.ToString().Trim(),
                                    Estado = worksheet.Cell(row, 6).Value.ToString().Trim(),
                                    Condicion = worksheet.Cell(row, 7).Value.ToString().Trim(),
                                    UsuarioAsignado = worksheet.Cell(row, 8).Value.ToString().Trim(),
                                    Sitio = worksheet.Cell(row, 9).Value.ToString().Trim(),
                                    Locacion = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                    ModoCompra = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                    Contrato = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                    Comentario = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                    IpLocal = worksheet.Cell(row, 14).Value.ToString().Trim(),
                                    MacFisica = worksheet.Cell(row, 15).Value.ToString().Trim(),
                                    Procesador1 = worksheet.Cell(row, 16).Value.ToString().Trim(),
                                    Procesador2 = worksheet.Cell(row, 17).Value.ToString().Trim(),
                                    CantidadHD = worksheet.Cell(row, 18).Value.ToString().Trim(),
                                    CapacidadHD = worksheet.Cell(row, 19).Value.ToString().Trim(),
                                };

                                activo.Errores = dbc.ValidarDatosCargaMasiva(idGrupo, idUMinera, activo.Serie, activo.TipoActivo, activo.Marca, activo.Modelo, activo.Estado, activo.Condicion, activo.UsuarioAsignado, activo.Sitio, activo.Locacion, activo.Contrato, activo.FechaInicio, activo.FechaFin, activo.ModoCompra, "", "", "", "").FirstOrDefault();

                                if (activo.Errores != "")
                                {
                                    activo.EstadoValidacion = false;
                                    msg = "ERROR";
                                }
                                else
                                {
                                    activo.EstadoValidacion = true;
                                }

                                previsualizacionDatos.Add(activo);
                            }
                        }
                        else if (grupo == "MOVIL")
                        {
                            for (int row = 2; row <= endRow; row++)
                            {
                                var activo = new CargaMasiva()
                                {
                                    Serie = worksheet.Cell(row, 1).Value.ToString().Trim(),
                                    NombreActivo = worksheet.Cell(row, 2).Value.ToString().Trim(),
                                    TipoActivo = worksheet.Cell(row, 3).Value.ToString().Trim(),
                                    Marca = worksheet.Cell(row, 4).Value.ToString().Trim(),
                                    Modelo = worksheet.Cell(row, 5).Value.ToString().Trim(),
                                    Estado = worksheet.Cell(row, 6).Value.ToString().Trim(),
                                    Condicion = worksheet.Cell(row, 7).Value.ToString().Trim(),
                                    UsuarioAsignado = worksheet.Cell(row, 8).Value.ToString().Trim(),
                                    Sitio = worksheet.Cell(row, 9).Value.ToString().Trim(),
                                    Locacion = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                    Contrato = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                    FechaInicio = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                    FechaFin = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                    Modalidad = worksheet.Cell(row, 14).Value.ToString().Trim(),
                                    Operador = worksheet.Cell(row, 15).Value.ToString().Trim(),
                                    Plan = worksheet.Cell(row, 16).Value.ToString().Trim(),
                                    Tarifa = worksheet.Cell(row, 17).Value.ToString().Trim(),
                                    MontoTotal = worksheet.Cell(row, 18).Value.ToString().Trim(),
                                    IpLocal = worksheet.Cell(row, 19).Value.ToString().Trim(),
                                    Comentario = worksheet.Cell(row, 20).Value.ToString().Trim(),
                                    Antivirus = worksheet.Cell(row, 21).Value.ToString().Trim(),
                                    Aplicaciones = worksheet.Cell(row, 22).Value.ToString().Trim()
                                };

                                activo.Errores = dbc.ValidarDatosCargaMasiva(idGrupo, idUMinera, activo.Serie, activo.TipoActivo, activo.Marca, activo.Modelo, activo.Estado, activo.Condicion, activo.UsuarioAsignado, activo.Sitio, activo.Locacion, activo.Contrato, activo.FechaInicio, activo.FechaFin, "", activo.Modalidad, activo.Operador, activo.Plan, activo.Aplicaciones).FirstOrDefault();

                                if (activo.Errores != "")
                                {
                                    activo.EstadoValidacion = false;
                                    msg = "ERROR";
                                }
                                else
                                {
                                    activo.EstadoValidacion = true;
                                }

                                previsualizacionDatos.Add(activo);
                            }
                        }

                        return Json(new { success = true, message = msg, previsualizacionDatos });

                    }
                }

                return Json(new { success = false, message = "Error con la información" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error con la información" });
            }
        }

        [HttpPost]
        public ActionResult InsertarCargaMasivaBNV(int idGrupo, int idUMinera)
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string grupo = ObtenerNomGrupoUsuarioBNV(idGrupo);
                string msg = "OK";

                int ID_USER = Convert.ToInt32(Session["UserId"].ToString());
                int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"].ToString());
                int ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
                string usuario = Convert.ToString(Session["NAM_PERS"]).ToUpper();

                if (file != null && file.ContentLength > 0)
                {
                    using (XLWorkbook workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        int lastRow = worksheet.LastRowUsed().RowNumber();
                        int batchSize = 1000;
                        int endRow = Math.Min(batchSize + 1, lastRow);

                        if (grupo == "MICROINFORMATICO")
                        {
                            for (int row = 2; row <= endRow; row++)
                            {
                                var activo = new CargaMasiva()
                                {
                                    Serie = worksheet.Cell(row, 1).Value.ToString().Trim(),
                                    NombreActivo = worksheet.Cell(row, 2).Value.ToString().Trim(),
                                    TipoActivo = worksheet.Cell(row, 3).Value.ToString().Trim(),
                                    Marca = worksheet.Cell(row, 4).Value.ToString().Trim(),
                                    Modelo = worksheet.Cell(row, 5).Value.ToString().Trim(),
                                    Estado = worksheet.Cell(row, 6).Value.ToString().Trim(),
                                    Condicion = worksheet.Cell(row, 7).Value.ToString().Trim(),
                                    UsuarioAsignado = worksheet.Cell(row, 8).Value.ToString().Trim(),
                                    Sitio = worksheet.Cell(row, 9).Value.ToString().Trim(),
                                    Locacion = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                    Ubicacion = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                    Contrato = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                    FechaInicio = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                    FechaFin = worksheet.Cell(row, 14).Value.ToString().Trim(),
                                    Tarifa = worksheet.Cell(row, 15).Value.ToString().Trim(),
                                    Comentario = worksheet.Cell(row, 16).Value.ToString().Trim(),
                                };

                                msg = dbc.InsertarActivoCargaMasiva(idGrupo, idUMinera, ID_USER, usuario, ID_ASSI, ID_QUEU, activo.Serie, activo.NombreActivo, activo.TipoActivo, activo.Marca, activo.Modelo, activo.Estado, activo.Condicion, activo.UsuarioAsignado, activo.Sitio, activo.Locacion, activo.Contrato, activo.FechaInicio, activo.FechaFin, activo.Tarifa, activo.Comentario, activo.Ubicacion, "", "", "", "", "", "", "", "", "", "", "", "", "").FirstOrDefault();

                            }
                        }
                        else if (grupo == "INFRAESTRUCTURA")
                        {
                            for (int row = 2; row <= endRow; row++)
                            {
                                var activo = new CargaMasiva()
                                {
                                    Serie = worksheet.Cell(row, 1).Value.ToString().Trim(),
                                    NombreActivo = worksheet.Cell(row, 2).Value.ToString().Trim(),
                                    TipoActivo = worksheet.Cell(row, 3).Value.ToString().Trim(),
                                    Marca = worksheet.Cell(row, 4).Value.ToString().Trim(),
                                    Modelo = worksheet.Cell(row, 5).Value.ToString().Trim(),
                                    Estado = worksheet.Cell(row, 6).Value.ToString().Trim(),
                                    Condicion = worksheet.Cell(row, 7).Value.ToString().Trim(),
                                    UsuarioAsignado = worksheet.Cell(row, 8).Value.ToString().Trim(),
                                    Sitio = worksheet.Cell(row, 9).Value.ToString().Trim(),
                                    Locacion = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                    ModoCompra = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                    Contrato = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                    Comentario = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                    IpLocal = worksheet.Cell(row, 14).Value.ToString().Trim(),
                                    MacFisica = worksheet.Cell(row, 15).Value.ToString().Trim(),
                                    Procesador1 = worksheet.Cell(row, 16).Value.ToString().Trim(),
                                    Procesador2 = worksheet.Cell(row, 17).Value.ToString().Trim(),
                                    CantidadHD = worksheet.Cell(row, 18).Value.ToString().Trim(),
                                    CapacidadHD = worksheet.Cell(row, 19).Value.ToString().Trim(),
                                };

                                msg = dbc.InsertarActivoCargaMasiva(idGrupo, idUMinera, ID_USER, usuario, ID_ASSI, ID_QUEU, activo.Serie, activo.NombreActivo, activo.TipoActivo, activo.Marca, activo.Modelo, activo.Estado, activo.Condicion, activo.UsuarioAsignado, activo.Sitio, activo.Locacion, activo.Contrato, "", "", "", activo.Comentario, "", activo.ModoCompra, activo.IpLocal, activo.MacFisica, activo.Procesador1, activo.Procesador2, activo.CantidadHD, activo.CapacidadHD, "", "", "", "", "", "").FirstOrDefault();

                            }
                        }
                        else if (grupo == "MOVIL")
                        {
                            for (int row = 2; row <= endRow; row++)
                            {
                                var activo = new CargaMasiva()
                                {
                                    Serie = worksheet.Cell(row, 1).Value.ToString().Trim(),
                                    NombreActivo = worksheet.Cell(row, 2).Value.ToString().Trim(),
                                    TipoActivo = worksheet.Cell(row, 3).Value.ToString().Trim(),
                                    Marca = worksheet.Cell(row, 4).Value.ToString().Trim(),
                                    Modelo = worksheet.Cell(row, 5).Value.ToString().Trim(),
                                    Estado = worksheet.Cell(row, 6).Value.ToString().Trim(),
                                    Condicion = worksheet.Cell(row, 7).Value.ToString().Trim(),
                                    UsuarioAsignado = worksheet.Cell(row, 8).Value.ToString().Trim(),
                                    Sitio = worksheet.Cell(row, 9).Value.ToString().Trim(),
                                    Locacion = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                    Contrato = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                    FechaInicio = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                    FechaFin = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                    Modalidad = worksheet.Cell(row, 14).Value.ToString().Trim(),
                                    Operador = worksheet.Cell(row, 15).Value.ToString().Trim(),
                                    Plan = worksheet.Cell(row, 16).Value.ToString().Trim(),
                                    Tarifa = worksheet.Cell(row, 17).Value.ToString().Trim(),
                                    MontoTotal = worksheet.Cell(row, 18).Value.ToString().Trim(),
                                    IpLocal = worksheet.Cell(row, 19).Value.ToString().Trim(),
                                    Comentario = worksheet.Cell(row, 20).Value.ToString().Trim(),
                                    Antivirus = worksheet.Cell(row, 21).Value.ToString().Trim(),
                                    Aplicaciones = worksheet.Cell(row, 22).Value.ToString().Trim()
                                };

                                msg = dbc.InsertarActivoCargaMasiva(idGrupo, idUMinera, ID_USER, usuario, ID_ASSI, ID_QUEU, activo.Serie, activo.NombreActivo, activo.TipoActivo, activo.Marca, activo.Modelo, activo.Estado, activo.Condicion, activo.UsuarioAsignado, activo.Sitio, activo.Locacion, activo.Contrato, activo.FechaInicio, activo.FechaFin, activo.Tarifa, activo.Comentario, "", "", activo.IpLocal, "", "", "", "", "", activo.Modalidad, activo.Operador, activo.Plan, activo.MontoTotal, activo.Antivirus, activo.Aplicaciones).FirstOrDefault();
                            }
                        }

                        return Json(new { success = true, message = msg });

                    }
                }

                return Json(new { success = false, message = "Error al guardar la información" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar la información" });
            }
        }

        public ActionResult ListarModalidadBNV()
        {
            int idGrupo;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "Nombre")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarModalidadActBNV(idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarOperadorBNV()
        {
            int idGrupo;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "Nombre")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarOperadorActBNV(idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAllOperadorBNV()
        {
            var result = (from x in dbc.OperadorActivo
                          select new
                          {
                              x.Id,
                              x.Nombre,
                              x.Estado
                          }).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPlanBNV(int idGrupo = 0)
        {
            int IdOperador;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "Nombre")
            {
                IdOperador = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                IdOperador = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarPlanActBNV(idGrupo, IdOperador, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlocNotasBNV(int id = 0)
        {
            var objClientAsset = dbc.CLIENT_ASSET.FirstOrDefault(x => x.ID_ASSE == id && x.DAT_END == null);
            ViewBag.ID_ASSE = id;
            ViewBag.ID_CLIE_ASSE = objClientAsset.ID_CLIE_ASSE;
            return View();
        }

        public ActionResult CargarBlocNotas(int id = 0)
        {
            var result = dbc.ObtenerBlocNotasBNV(id).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GuardarBlocNotas()
        {
            int idAsse = Convert.ToInt32(Request.Params["idAsse"].ToString());
            int idClieAsse = Convert.ToInt32(Request.Params["idClieAsse"].ToString());
            string txt = Request.Params["txt"].ToString();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            try
            {
                BlocNotasAct bloc = new BlocNotasAct();

                bloc.IdAsse = idAsse;
                bloc.IdClieAsse = idClieAsse;
                bloc.Comentario = txt;
                bloc.UsuarioCreacion = UserId;
                bloc.FechaCreacion = DateTime.Now;
                bloc.Estado = true;

                dbc.BlocNotasAct.Add(bloc);
                dbc.SaveChanges();
            }
            catch
            {
                return Content("Error");
            }

            return Content("Ok");
        }

        public ActionResult ListarResponsablexGrupo()
        {
            int idGrupo;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "Nombre")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbe.ListarResponsablesxGrupo(idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public int ObtenerIdGrupo(string nomGrupo)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var grupo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_ACCO == idAcco && x.VAL_ACCO_PARA == nomGrupo && x.ID_PARA == 1070 && x.VIG_ACCO_PARA == true);

            if (grupo != null)
                return grupo.ID_ACCO_PARA;
            else
                return 0;
        }

        public string ObtenerNombreGrupo(int idGrupo)
        {
            var grupo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_ACCO_PARA == idGrupo);

            if (grupo != null)
                return grupo.VAL_ACCO_PARA;
            else
                return "";
        }

        public ActionResult CreateOT()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            if (idAcco == 55 && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
            {
                int idGrupoOT = ObtenerIdGrupo("OT");
                ViewBag.IdGrupoOT = idGrupoOT;
                return View("CreateHudbayOT");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateHudbayOT(IEnumerable<HttpPostedFileBase> archivos, ASSET asset)
        {
            int idPersEnti, idLoca, userId = 0, idQueu = 0, idPrio = 4, idPersAssi = 0, idArea = 0;
            string mensajeVal = "";
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            idLoca = (Request.Form["Locacion"] != "" && Request.Form["Locacion"] != null) ? Convert.ToInt32(Request.Form["Locacion"]) : 0;
            asset.SER_NUMB = asset.SER_NUMB?.Trim() ?? string.Empty;
            asset.NAM_EQUI = asset.NAM_EQUI?.Trim() ?? string.Empty;
            asset.COD_ASSE = asset.COD_ASSE?.Trim() ?? string.Empty;

            //validaiones campos obligatorios
            if (string.IsNullOrEmpty(asset.SER_NUMB))
                mensajeVal += "Ingrese el número de serie. <br/>";
            if (string.IsNullOrEmpty(asset.NAM_EQUI))
                mensajeVal += "Ingrese el nombre del equipo. <br/>";
            if (asset.ID_TYPE_ASSE == null)
                mensajeVal += "Seleccione el tipo de activo. <br/>";
            if (asset.ID_MANU == null)
                mensajeVal += "Seleccione la marca. <br/>";
            if (idLoca == 0)
                mensajeVal += "Seleccione la locación. <br/>";

            //grupo OT
            int idGrupoOT = Convert.ToInt32(Request.Form["IdGrupoOT"]);

            //validación serie repetida
            int existeSerie = dbc.ASSETs.Where(x => x.SER_NUMB == asset.SER_NUMB && x.ID_ACCO == idAcco && x.IdGrupo == idGrupoOT).Count();
            if (existeSerie > 0)
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('ERROR','3');}window.onload=init;</script>");

            if (mensajeVal != "")
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('INFO','" + mensajeVal + "',0);}window.onload=init;</script>");

            try
            {
                idPersAssi = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                userId = Convert.ToInt32(Session["UserId"]);
                idQueu = Convert.ToInt32(Session["ID_QUEU"]);
                idArea = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == idAcco && x.ID_PARA == 31).VAL_ACCO_PARA);
            }
            catch
            {

            }

            if (ModelState.IsValid)
            {
                //insertar asset
                asset.CREATE_DATE = DateTime.Now;
                asset.CREATE_ASSE = DateTime.Now;
                asset.ID_ACCO = idAcco;
                asset.UserId = userId;
                asset.IdGrupo = idGrupoOT;
                dbc.ASSETs.Add(asset);
                dbc.SaveChanges();

                var query = dbc.PARAMETERs.Where(x => x.NAM_PARA == "RESPONSIBLE ASSET")
                                .Join(dbc.ACCOUNT_PARAMETER, x => x.ID_PARA, p => p.ID_PARA, (x, p) => new
                                {
                                    p.VAL_ACCO_PARA,
                                    p.ID_ACCO
                                }).Where(x => x.ID_ACCO == idAcco).First();

                idPersEnti = Convert.ToInt32(query.VAL_ACCO_PARA);
                idPrio = Convert.ToInt32(dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == idPersEnti).ID_PRIO);

                //insertar client asset
                var clientAsset = new CLIENT_ASSET();
                clientAsset.ID_PERS_ENTI = idPersEnti;
                clientAsset.ID_ASSE = asset.ID_ASSE;
                clientAsset.ID_AREA = idArea;
                clientAsset.ID_COND = 9;
                clientAsset.ID_LOCA = idLoca;
                clientAsset.DAT_STAR = DateTime.Now;
                clientAsset.CREATE_DATE = DateTime.Now;
                clientAsset.UserId = userId;
                clientAsset.ID_TYPE_CLIE_ASSE = 6;
                dbc.CLIENT_ASSET.Add(clientAsset);
                dbc.SaveChanges();

                if (archivos != null)
                {
                    foreach (var file in archivos)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_ASSE = asset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            //db.AddToATTACHEDs(ATTA);
                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }

                string usuario = Convert.ToString(Session["NAM_PERS"]).ToUpper();

                //crear ticket de creación de asset
                string Code = null;
                try
                {
                    var ticket = dbc.InsertarTicketActivoHudbay(idAcco, idPersEnti, idPersAssi, idArea, idQueu, idPrio,
                    asset.ID_ASSE, asset.CREATE_ASSE, "TICKET CREATED BY " + usuario + " TO REGISTER TECHNOLOGICAL DEVICE IN THE INVENTORY",
                    userId, idGrupoOT).Single();

                    int id = Convert.ToInt32(ticket.ID_TICK);

                    Code = ticket.COD_TICK;

                    var asseForm = dbc.ASSETs.Single(x => x.ID_ASSE == asset.ID_ASSE);
                    asseForm.ID_TICK = id;
                    dbc.ASSETs.Attach(asseForm);
                    dbc.Entry(asseForm).State = EntityState.Modified;
                    dbc.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + Code + "', '" + asset.ID_ASSE.ToString() + "');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }

        public ActionResult IndexOT()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            if (idAcco == 55 && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
            {
                int idGrupoOT = ObtenerIdGrupo("OT");
                ViewBag.IdGrupoOT = idGrupoOT;
                return View("IndexHudbayOT");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult ListaActivosPorGrupo()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["IdGrupo"]);
            int idTipoActivo = Convert.ToInt32(Request.Params["IdTipoActivo"]);
            int idMarca = Convert.ToInt32(Request.Params["IdMarca"]);
            int idUsuario = Convert.ToInt32(Request.Params["IdUsuario"]);
            string palabraClave = Request.Params["PalabraClave"];
            int idEstado = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.ListaActivosPorGrupo(idGrupo, idEstado, idTipoActivo, idMarca, idUsuario, palabraClave, idAcco).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Grid = query2, Cantidad = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CantidadActivosPorGrupo()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["IdGrupo"]);
            int idTipoActivo = Convert.ToInt32(Request.Params["IdTipoActivo"]);
            int idMarca = Convert.ToInt32(Request.Params["IdMarca"]);
            int idUsuario = Convert.ToInt32(Request.Params["IdUsuario"]);
            string palabraClave = Request.Params["PalabraClave"];

            var query = dbc.CantidadActivosPorGrupo(idGrupo, idTipoActivo, idMarca, idUsuario, palabraClave, idAcco).FirstOrDefault();

            int Assigned = query.Asignados.GetValueOrDefault();
            int Unassigned = query.NoAsignados.GetValueOrDefault();
            int Spare = query.Repuestos.GetValueOrDefault();
            int Surplus = query.Inoperativos.GetValueOrDefault();

            return Json(new { Asignado = Assigned, NoAsignado = Unassigned, Repuesto = Spare, Inoperativo = Surplus }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditHudbayOT(IEnumerable<HttpPostedFileBase> files, ASSET asset)
        {
            if (ModelState.IsValid)
            {
                int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
                string mensajeE = "";
                var activo = dbc.ASSETs.Where(x => x.SER_NUMB == asset.SER_NUMB);

                asset.SER_NUMB = asset.SER_NUMB?.Trim() ?? string.Empty;
                asset.NAM_EQUI = asset.NAM_EQUI?.Trim() ?? string.Empty;
                asset.COD_ASSE = asset.COD_ASSE?.Trim() ?? string.Empty;

                if (activo == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR');}window.onload=init;</script>");
                }

                int cantAsset = dbc.ASSETs.Where(x => x.SER_NUMB == asset.SER_NUMB && x.IdGrupo == asset.IdGrupo && x.ID_ACCO == idAcco && x.ID_ASSE != asset.ID_ASSE).Count();
                if (cantAsset > 0)
                {
                    mensajeE += "La serie ingresada ya existe. Por favor validar. <br/>";
                }

                if (string.IsNullOrEmpty(asset.SER_NUMB))
                    mensajeE += "Ingrese el número de serie. <br/>";
                if (string.IsNullOrEmpty(asset.NAM_EQUI))
                    mensajeE += "Ingrese el nombre del equipo. <br/>";
                if (asset.ID_TYPE_ASSE == 0 || asset.ID_TYPE_ASSE == null)
                    mensajeE += "Seleccione el tipo de activo. <br/>";
                if (asset.ID_MANU == 0 || asset.ID_MANU == null)
                    mensajeE += "Seleccione la marca. <br/>";

                if (mensajeE != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','" + mensajeE + "');}window.onload=init;</script>");
                }

                asset.MODIFIED_DATE = DateTime.Now;
                dbc.ASSETs.Attach(asset);               
                dbc.Entry(asset).State = EntityState.Modified;
                dbc.SaveChanges();

                var objClientAsset = dbc.CLIENT_ASSET.Where(x => x.ID_ASSE == asset.ID_ASSE && x.DAT_END == null).FirstOrDefault();

                if (objClientAsset != null)
                {
                    objClientAsset.UserId = Convert.ToInt32(Session["UserId"]);
                    objClientAsset.DAT_END = DateTime.Now;
                    objClientAsset.CREATE_DATE = DateTime.Now;
                    objClientAsset.ID_TYPE_CLIE_ASSE = 5;
                    objClientAsset.SUM_CLIE_ASSE = asset.SUM_ASSE;
                    dbc.CLIENT_ASSET.Add(objClientAsset);
                    dbc.SaveChanges();
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_CLIE_ASSE = objClientAsset.ID_CLIE_ASSE;
                            ATTA.ID_ASSE = objClientAsset.ID_ASSE;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR');}window.onload=init;</script>");
        }

        public ActionResult TicketOT()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            if (idAcco == 55 && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
            {
                int idGrupoOT = ObtenerIdGrupo("OT");
                ViewBag.IdGrupoOT = idGrupoOT;
                return View("TicketHudbayOT");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult ListarResponsableHBOT()
        {
            var result = dbe.ListarResponsablesHBOT().ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindOT()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            if (idAcco == 55 && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
            {
                int idGrupoOT = ObtenerIdGrupo("OT");
                ViewBag.IdGrupoOT = idGrupoOT;
                return View("FindHudbayOT");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult FindResultOT()
        {
            string IdGrupo = Request.Params["IdGrupoOT"].ToString();
            string Codigo = Request.Params["COD_ASSE"].ToString();
            string Serie = Request.Params["SER_NUMB"].ToString();
            string NombreEquipo = Request.Params["NAM_EQUI"].ToString();
            string MacFisica = Request.Params["MAC_ADRR"].ToString();
            string Marca = Request.Params["ID_MANU"].ToString();
            string ModeloComercial = Convert.ToString(Request.Params["ID_COMM_MODE"] == null ? "0" : Request.Params["ID_COMM_MODE"]);
            string Sitio = Request.Params["ID_SITE"].ToString();
            string Locacion = Convert.ToString(Request.Params["ID_LOCA"] == null ? "0" : Request.Params["ID_LOCA"]);
            string Estado = Request.Params["ID_STAT_ASSE"].ToString();
            string Condicion = Convert.ToString(Request.Params["ID_COND"] == null ? "0" : Request.Params["ID_COND"]);

            string Usuarios = Request.Params["Usuarios"].ToString();
            string TiposActivo = Request.Params["valTypeAsset"].ToString();
            string Contratos = Request.Params["Contratos"].ToString();

            if (String.IsNullOrEmpty(IdGrupo)) IdGrupo = "0";
            if (String.IsNullOrEmpty(Marca)) Marca = "0";
            if (String.IsNullOrEmpty(ModeloComercial)) ModeloComercial = "0";
            if (String.IsNullOrEmpty(Sitio)) Sitio = "0";
            if (String.IsNullOrEmpty(Locacion)) Locacion = "0";
            if (String.IsNullOrEmpty(Estado)) Estado = "0";
            if (String.IsNullOrEmpty(Condicion)) Condicion = "0";

            string IdAcco = Session["ID_ACCO"].ToString();
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            //Selección múltiple
            if (String.IsNullOrEmpty(TiposActivo)) TiposActivo = "0";
            TiposActivo = TiposActivo.Substring(0, TiposActivo.Length - 1);
            if (String.IsNullOrEmpty(Usuarios)) Usuarios = "0";
            Usuarios = Usuarios.Substring(0, Usuarios.Length - 1);
            if (String.IsNullOrEmpty(Contratos)) Contratos = "0";
            Contratos = Contratos.Substring(0, Contratos.Length - 1);

            var query = dbc.BuscarActivoGrupoOT(IdGrupo, IdAcco, Codigo, Serie, NombreEquipo, MacFisica, Marca, ModeloComercial, Usuarios, TiposActivo, Contratos, Sitio, Locacion, Estado, Condicion).ToList();

            var total = query.Count();
            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportHudbayOT()
        {
            string IdGrupo = Request.Params["IdGrupoOT"].ToString();
            string Codigo = Request.Params["COD_ASSE"].ToString();
            string Serie = Request.Params["SER_NUMB"].ToString();
            string NombreEquipo = Request.Params["NAM_EQUI"].ToString();
            string MacFisica = Request.Params["MAC_ADRR"].ToString();
            string Marca = Request.Params["ID_MANU"].ToString();
            string ModeloComercial = Convert.ToString(Request.Params["ID_COMM_MODE"] == null ? "0" : Request.Params["ID_COMM_MODE"]);
            string Sitio = Request.Params["ID_SITE"].ToString();
            string Locacion = Convert.ToString(Request.Params["ID_LOCA"] == null ? "0" : Request.Params["ID_LOCA"]);
            string Estado = Request.Params["ID_STAT_ASSE"].ToString();
            string Condicion = Convert.ToString(Request.Params["ID_COND"] == null ? "0" : Request.Params["ID_COND"]);

            string Usuarios = Request.Params["Usuarios"].ToString();
            string TiposActivo = Request.Params["ExpTypeAsset"].ToString();
            string Contratos = Request.Params["Contratos"].ToString();

            if (String.IsNullOrEmpty(IdGrupo)) IdGrupo = "0";
            if (String.IsNullOrEmpty(Marca)) Marca = "0";
            if (String.IsNullOrEmpty(ModeloComercial)) ModeloComercial = "0";
            if (String.IsNullOrEmpty(Sitio)) Sitio = "0";
            if (String.IsNullOrEmpty(Locacion)) Locacion = "0";
            if (String.IsNullOrEmpty(Estado)) Estado = "0";
            if (String.IsNullOrEmpty(Condicion)) Condicion = "0";

            string IdAcco = Session["ID_ACCO"].ToString();

            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Detalle");

            var query = dbc.BuscarActivoGrupoOT(IdGrupo, IdAcco, Codigo, Serie, NombreEquipo, MacFisica, Marca, ModeloComercial, Usuarios, TiposActivo, Contratos, Sitio, Locacion, Estado, Condicion).ToList();

            worksheet.Cell(1, 1).Value = "CÓDIGO ACTIVO";
            worksheet.Cell(1, 2).Value = "SERIE";
            worksheet.Cell(1, 3).Value = "NOMBRE ACTIVO";
            worksheet.Cell(1, 4).Value = "TIPO ACTIVO";
            worksheet.Cell(1, 5).Value = "MARCA";
            worksheet.Cell(1, 6).Value = "MODELO COMERCIAL";
            worksheet.Cell(1, 7).Value = "LOTE";
            worksheet.Cell(1, 8).Value = "SITIO";
            worksheet.Cell(1, 9).Value = "LOCACIÓN";
            worksheet.Cell(1, 10).Value = "CONDICIÓN";
            worksheet.Cell(1, 11).Value = "ESTADO";
            worksheet.Cell(1, 12).Value = "USUARIO ASIGNADO";
            worksheet.Cell(1, 13).Value = "IP LOCAL";
            worksheet.Cell(1, 14).Value = "IP PÚBLICA";
            worksheet.Cell(1, 15).Value = "CONTRATO";
            worksheet.Cell(1, 16).Value = "MAC FÍSICA";
            worksheet.Cell(1, 17).Value = "MAC INALÁMBRICA";
            worksheet.Cell(1, 18).Value = "MAC BLUETOOTH";

            for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
            {
                worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D3765");
                worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
            }

            for (int i = 0; i < query.Count(); i++)
            {
                worksheet.Cell(i + 2, 1).Value = query[i].CodigoActivo;
                worksheet.Cell(i + 2, 2).Value = query[i].Serie;
                worksheet.Cell(i + 2, 3).Value = query[i].NombreActivo;
                worksheet.Cell(i + 2, 4).Value = query[i].TipoActivo;
                worksheet.Cell(i + 2, 5).Value = query[i].Marca;
                worksheet.Cell(i + 2, 6).Value = query[i].ModeloComercial;
                worksheet.Cell(i + 2, 7).Value = query[i].Lote;
                worksheet.Cell(i + 2, 8).Value = query[i].Sitio;
                worksheet.Cell(i + 2, 9).Value = query[i].Locacion;
                worksheet.Cell(i + 2, 10).Value = query[i].Condicion;
                worksheet.Cell(i + 2, 11).Value = query[i].Estado;
                worksheet.Cell(i + 2, 12).Value = query[i].UsuarioAsignado;
                worksheet.Cell(i + 2, 13).Value = query[i].IpLocal;
                worksheet.Cell(i + 2, 14).Value = query[i].IpPublica;
                worksheet.Cell(i + 2, 15).Value = query[i].Contrato;
                worksheet.Cell(i + 2, 16).Value = query[i].MacFisica;
                worksheet.Cell(i + 2, 17).Value = query[i].MacInalambrica;
                worksheet.Cell(i + 2, 18).Value = query[i].MacBluetooth;
            }

            for (int i = 1; i <= worksheet.ColumnsUsed().Count(); i++)
            {
                worksheet.Column(i).AdjustToContents();
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=BusquedaActivo" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                memoryStream.WriteTo(Response.OutputStream);
            }
            Response.End();

            return View();
        }

        public ActionResult DescargarPlantillaMinsur()
        {
            string rutaArchivo = Server.MapPath("~/Adjuntos/CargaMasivaActivosMinsur.xlsx");

            return File(rutaArchivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CargaMasivaActivosMinsur.xlsx");
        }

        [HttpPost]
        public ActionResult ValidarCargaMasivaMinsur()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string msg = "OK";
                int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

                if (file != null && file.ContentLength > 0)
                {
                    using (XLWorkbook workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        int lastRow = worksheet.LastRowUsed().RowNumber();
                        int batchSize = 1000;
                        int endRow = Math.Min(batchSize + 1, lastRow);

                        var previsualizacionDatos = new List<CargaMasiva>();

                        for (int row = 2; row <= endRow; row++)
                        {
                            var activo = new CargaMasiva()
                            {
                                NombreActivo = worksheet.Cell(row, 1).Value.ToString().Trim(),
                                Serie = worksheet.Cell(row, 2).Value.ToString().Trim(),
                                TipoActivo = worksheet.Cell(row, 3).Value.ToString().Trim(),
                                SubTipoActivo = worksheet.Cell(row, 4).Value.ToString().Trim(),
                                Codigo = worksheet.Cell(row, 5).Value.ToString().Trim(),
                                Grupo = worksheet.Cell(row, 6).Value.ToString().Trim(),
                                Marca = worksheet.Cell(row, 7).Value.ToString().Trim(),
                                Modelo = worksheet.Cell(row, 8).Value.ToString().Trim(),
                                ModeloFabrica = worksheet.Cell(row, 9).Value.ToString().Trim(),
                                Arrendador = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                Solped = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                CentroCosto = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                ModoCompra = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                Tarifa = worksheet.Cell(row, 14).Value.ToString().Trim(), //Costo
                                FechaFin = worksheet.Cell(row, 15).Value.ToString().Trim(), //Fecha fin contrato
                                Sitio = worksheet.Cell(row, 16).Value.ToString().Trim(),
                                Locacion = worksheet.Cell(row, 17).Value.ToString().Trim(),
                                FechaInicio = worksheet.Cell(row, 18).Value.ToString().Trim(), //Fecha adquisicion
                                Condicion = worksheet.Cell(row, 19).Value.ToString().Trim(),
                                Estado = worksheet.Cell(row, 20).Value.ToString().Trim(),
                                UsuarioResponsable = worksheet.Cell(row, 21).Value.ToString().Trim(),
                                UsuarioAsignado = worksheet.Cell(row, 22).Value.ToString().Trim(),
                                IpLocal = worksheet.Cell(row, 23).Value.ToString().Trim(),
                                IpPublica = worksheet.Cell(row, 24).Value.ToString().Trim(),
                                Contrato = worksheet.Cell(row, 25).Value.ToString().Trim(),
                                MacFisica = worksheet.Cell(row, 26).Value.ToString().Trim(),
                                MacInalambrica = worksheet.Cell(row, 27).Value.ToString().Trim(),
                                MacBluetooth = worksheet.Cell(row, 28).Value.ToString().Trim(),
                                Lote = worksheet.Cell(row, 29).Value.ToString().Trim(),
                                TipoUsuario = worksheet.Cell(row, 30).Value.ToString().Trim()
                            };

                            activo.Errores = dbc.ValidarDatosCargaMasivaMS(idAcco, activo.TipoActivo, activo.SubTipoActivo, activo.Serie, activo.Codigo, activo.Grupo, activo.Marca,activo.Modelo, 
                                activo.Sitio, activo.Locacion, activo.Estado, activo.Condicion, activo.UsuarioAsignado, activo.UsuarioResponsable, activo.ModoCompra).FirstOrDefault();

                            if (activo.Errores != "")
                            {
                                activo.EstadoValidacion = false;
                                msg = "ERROR";
                            }
                            else
                            {
                                activo.EstadoValidacion = true;
                            }

                            previsualizacionDatos.Add(activo);
                        }

                        return Json(new { success = true, message = msg, previsualizacionDatos });

                    }
                }

                return Json(new { success = false, message = "Error con la información" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error con la información" });
            }
        }

        [HttpPost]
        public ActionResult InsertarCargaMasivaMinsur()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string msg = "OK";

                int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
                int ID_USER = Convert.ToInt32(Session["UserId"].ToString());
                int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"].ToString());
                int ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
                string usuario = Convert.ToString(Session["NAM_PERS"]).ToUpper();

                if (file != null && file.ContentLength > 0)
                {
                    using (XLWorkbook workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        int lastRow = worksheet.LastRowUsed().RowNumber();
                        int batchSize = 1000;
                        int endRow = Math.Min(batchSize + 1, lastRow);

                        for (int row = 2; row <= endRow; row++)
                        {
                            var activo = new CargaMasiva()
                            {
                                NombreActivo = worksheet.Cell(row, 1).Value.ToString().Trim(),
                                Serie = worksheet.Cell(row, 2).Value.ToString().Trim(),
                                TipoActivo = worksheet.Cell(row, 3).Value.ToString().Trim(),
                                SubTipoActivo = worksheet.Cell(row, 4).Value.ToString().Trim(),
                                Codigo = worksheet.Cell(row, 5).Value.ToString().Trim(),
                                Grupo = worksheet.Cell(row, 6).Value.ToString().Trim(),
                                Marca = worksheet.Cell(row, 7).Value.ToString().Trim(),
                                Modelo = worksheet.Cell(row, 8).Value.ToString().Trim(),
                                ModeloFabrica = worksheet.Cell(row, 9).Value.ToString().Trim(),
                                Arrendador = worksheet.Cell(row, 10).Value.ToString().Trim(),
                                Solped = worksheet.Cell(row, 11).Value.ToString().Trim(),
                                CentroCosto = worksheet.Cell(row, 12).Value.ToString().Trim(),
                                ModoCompra = worksheet.Cell(row, 13).Value.ToString().Trim(),
                                Tarifa = worksheet.Cell(row, 14).Value.ToString().Trim(), //Costo
                                FechaFin = worksheet.Cell(row, 15).Value.ToString().Trim(), //Fecha fin contrato
                                Sitio = worksheet.Cell(row, 16).Value.ToString().Trim(),
                                Locacion = worksheet.Cell(row, 17).Value.ToString().Trim(),
                                FechaInicio = worksheet.Cell(row, 18).Value.ToString().Trim(), //Fecha adquisicion
                                Condicion = worksheet.Cell(row, 19).Value.ToString().Trim(),
                                Estado = worksheet.Cell(row, 20).Value.ToString().Trim(),
                                UsuarioResponsable = worksheet.Cell(row, 21).Value.ToString().Trim(),
                                UsuarioAsignado = worksheet.Cell(row, 22).Value.ToString().Trim(),
                                IpLocal = worksheet.Cell(row, 23).Value.ToString().Trim(),
                                IpPublica = worksheet.Cell(row, 24).Value.ToString().Trim(),
                                Contrato = worksheet.Cell(row, 25).Value.ToString().Trim(),
                                MacFisica = worksheet.Cell(row, 26).Value.ToString().Trim(),
                                MacInalambrica = worksheet.Cell(row, 27).Value.ToString().Trim(),
                                MacBluetooth = worksheet.Cell(row, 28).Value.ToString().Trim(),
                                Lote = worksheet.Cell(row, 29).Value.ToString().Trim(),
                                TipoUsuario = worksheet.Cell(row, 30).Value.ToString().Trim()
                            };

                            var datos = dbc.ObtenerDatosCargaMasivaMS(idAcco, ID_USER, activo.TipoActivo, activo.SubTipoActivo, activo.Grupo, activo.Marca, activo.Modelo, activo.ModeloFabrica,
                                activo.Arrendador, activo.CentroCosto, activo.ModoCompra, activo.Sitio, activo.Locacion, activo.Estado, activo.Condicion, activo.UsuarioResponsable,
                                activo.UsuarioAsignado, activo.Contrato, activo.TipoUsuario).FirstOrDefault();

                            msg = dbc.InsertarActivoCargaMasivaMS(idAcco, ID_USER, usuario, ID_ASSI, ID_QUEU, activo.Serie, datos.IdTipoActivo, datos.IdSubTipoActivo, activo.Codigo, activo.NombreActivo,
                                datos.IdGrupo, datos.IdMarca, datos.IdModeloComercial, datos.IdModeloFabrica, datos.IdArrendador, activo.Solped, datos.IdCentroCosto, datos.IdModoCompra,
                                activo.Tarifa, activo.FechaFin, datos.IdLocacion, activo.FechaInicio, datos.IdEstado, datos.IdCondicion, datos.IdPersResponsable, datos.IdPersAsignado,
                                activo.IpLocal, activo.IpPublica, datos.IdContrato, activo.MacFisica, activo.MacInalambrica, activo.MacBluetooth, activo.Lote, datos.IdTipoUsuario).FirstOrDefault();

                        }

                        return Json(new { success = true, message = msg });

                    }
                }

                return Json(new { success = false, message = "Error al guardar la información" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar la información" });
            }
        }

        public ActionResult ListarAplicacionesMovil()
        {
            var result = (from x in dbc.AplicacionMovil
                          where x.Estado == true
                          select new
                          {
                              id = x.Id,
                              text = x.Nombre
                          });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarDocumentosAdjuntosBNV(int IdGrupo, int? IdUMinera, int? IdPersEnti, string NumSerie, int? IdTipoFormato)
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (IdUMinera == null) IdUMinera = 0;
            if (IdPersEnti == null) IdPersEnti = 0;
            if (IdTipoFormato == null) IdTipoFormato = 0;

            var query = dbc.ListarDocumentosAdjuntosBNV(IdAcco, IdGrupo, IdUMinera, IdPersEnti, NumSerie, IdTipoFormato).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarDocumentosAdjuntosBNV()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdGrupo = !string.IsNullOrEmpty(Request.Params["IdGrupo"]) ? Convert.ToInt32(Request.Params["IdGrupo"]) : 0;
            int IdUMinera = !string.IsNullOrEmpty(Request.Params["UMinera"]) ? Convert.ToInt32(Request.Params["UMinera"]) : 0;
            int IdPersEnti = !string.IsNullOrEmpty(Request.Params["ID_PERS_ENTI"]) ? Convert.ToInt32(Request.Params["ID_PERS_ENTI"]) : 0;
            int IdTipoFormato = !string.IsNullOrEmpty(Request.Params["ID_TYPE_FORM"]) ? Convert.ToInt32(Request.Params["ID_TYPE_FORM"]) : 0;
            string NumSerie = Convert.ToString(Request.Params["SER_NUMB"]);

            DataTable dataTable = ObtenerDocumentosAdjuntosBNV(IdAcco, IdGrupo, IdUMinera, IdPersEnti, NumSerie, IdTipoFormato);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ReporteDocumentosAdjuntos");

                ws.Column(6).Style.Numberformat.Format = "dd/MM/yyyy";
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 40;
                ws.Column(6).Width = 15;
                ws.Column(7).Width = 40;
                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);

                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=ReporteDocumentosAdjuntos.xlsx");

                ms.WriteTo(HttpContext.Response.OutputStream);
                HttpContext.Response.End();
            }

            return View();
        }

        public DataTable ObtenerDocumentosAdjuntosBNV(int IdAcco, int IdGrupo, int IdUMinera, int IdPersEnti, string NumSerie, int IdTipoFormato)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Buenaventura.ExportarDocumentosAdjuntosBNV", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdAcco", IdAcco);
                    cmd.Parameters.AddWithValue("@IdGrupo", IdGrupo);
                    cmd.Parameters.AddWithValue("@IdUMinera", IdUMinera);
                    cmd.Parameters.AddWithValue("@IdPersEnti", IdPersEnti);
                    cmd.Parameters.AddWithValue("@NumSerie", NumSerie);
                    cmd.Parameters.AddWithValue("@IdTipoFormato", IdTipoFormato);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

    }

    public class CargaMasiva
    {
        public string Serie { get; set; }
        public string Codigo { get; set; }
        public string NombreActivo { get; set; }
        public string Marca { get; set; }
        public string TipoActivo { get; set; }
        public string SubTipoActivo { get; set; }
        public string Modelo { get; set; }
        public string ModeloFabrica { get; set; }
        public string UsuarioAsignado { get; set; }
        public string UsuarioResponsable { get; set; }
        public string Estado { get; set; }
        public string Condicion { get; set; }
        public string Sitio { get; set; }
        public string Locacion { get; set; }
        public string Ubicacion { get; set; }
        public string Contrato { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string Tarifa { get; set; }
        public string Comentario { get; set; }
        public string ModoCompra { get; set; }
        public string IpLocal { get; set; }
        public string IpPublica { get; set; }
        public string MacFisica { get; set; }
        public string MacInalambrica { get; set; }
        public string MacBluetooth { get; set; }
        public string Procesador1 { get; set; }
        public string Procesador2 { get; set; }
        public string CantidadHD { get; set; }
        public string CapacidadHD { get; set; }
        public string Modalidad { get; set; }
        public string Operador { get; set; }
        public string Plan { get; set; }
        public string MontoTotal { get; set; }
        public string Grupo { get; set; }
        public string Arrendador { get; set; }
        public string CentroCosto { get; set; }
        public string Solped { get; set; }
        public string Lote { get; set; }
        public string TipoUsuario { get; set; }
        public string Antivirus { get; set; }
        public string Aplicaciones { get; set; }
        public string Errores { get; set; }
        public bool EstadoValidacion { get; set; }

    }
}