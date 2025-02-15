using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

using ERPElectrodata.Models;
using System.Data.Entity;
using ERPElectrodata.AppCode;
using System.Text;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
namespace ERPElectrodata.Controllers
{
    public class InformController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        //
        // GET: /Inform/
        [Authorize]

        public ActionResult IndexPlantilla()
        {
            return View();
        }

        public ActionResult CrearPlantilla()
        {
            return View();
        }
        public ActionResult CrearFormato()
        {
            return View();
        }



        public ActionResult EditarPlantilla(int id = 0)
        {
            ViewBag.Id = id;
            //var pe = dbc.ACCOUNT_TYPE_ASSET.Find(id,idAcco);

            var inf = dbc.InformePlantillas.Find(id);
            ViewBag.Titulo = inf.Titulo;

            return View(inf);
        }

        public ActionResult FormatoInformesAuto()
        {
            ViewData["Message"] = "Welcome to Web Form 2";
            return View();
        }
        public ActionResult InformeAutomatico(int id = 0)
        {
            ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);

            ViewBag.Id = id;

            var inf = dbc.InformePlantillas.Find(id);

            return View(inf);
        }
        public ActionResult ListarInformePlantillasolo()
        {

            List<object> Registros = new List<object>(dbc.InformePlantillaListar()).ToList();

            return Json(new { data = Registros }, JsonRequestBehavior.AllowGet);


        }


        public ActionResult listarplantilla2()
        {



            var result = dbc.InformePlantillaListar().ToList();

            //var result = dbe.ListarTablaDocumentos().ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ListarInformePlantilla(int i = 0)
        {
            // int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.InformePlantillaListar().ToList();
            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Crear(FormCollection collection, InformePlantilla objInfPlantilla, InformePlantillaCategoria objInfCategoria, InformePlantillaGrafico objInfGrafico)
        {
            int sw = 0, existe = 0;
            string msjError = string.Empty;

            if (String.IsNullOrEmpty(objInfPlantilla.Titulo)) { sw = 1; msjError = "Ingrese un Titulo"; }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
            }

            #region validaciones
            String titulo = objInfPlantilla.Titulo;
            var result = (from inf in dbc.InformePlantillas.Where(x => x.Titulo == titulo && x.Estado == true).ToList()
                          select new
                          {
                              id = inf.Id,
                              titulo = inf.Titulo
                          }).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + "El Nombre de la Plantilla ya existe." + "','0');}window.onload=init;</script>");
            }


            IQueryable<InformeCategoria> iCat = from c in dbc.InformeCategorias
                                                where c.Estado == true
                                                select c;
            InformeCategoria[] iCatArray = iCat.ToArray();

            string chkCategoria = "";
            foreach (InformeCategoria infCat in iCatArray)
            {
                if (Convert.ToString(collection[infCat.Nombre]) != null)
                {
                    if (Convert.ToString(collection["Descripcion" + infCat.Id]) != null)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(collection["Descripcion" + infCat.Id])))
                        {
                            sw = 1; msjError = "Se debe completar la Descripción de " + infCat.Nombre;
                        }
                        chkCategoria = Convert.ToString(collection[infCat.Nombre]);
                        if (sw == 1)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                        }
                    }
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
                }

                objInfPlantilla.UsuarioIdCrea = IdUser;
                objInfPlantilla.FechaCrea = DateTime.Now;
                objInfPlantilla.Estado = true;
                dbc.InformePlantillas.Add(objInfPlantilla);
                dbc.SaveChanges();

                #region CheckCategorias
                IQueryable<InformeCategoria> informeCat = from c in dbc.InformeCategorias where c.Estado == true select c;
                InformeCategoria[] informeCatArray = informeCat.ToArray();
                foreach (InformeCategoria infCat in informeCatArray)
                {
                    if (Convert.ToString(collection[infCat.Nombre]) != null)
                    {
                        objInfCategoria.IdPlantilla = objInfPlantilla.Id;
                        objInfCategoria.IdCategoria = infCat.Id;
                        objInfCategoria.Descripcion = Convert.ToString(collection["Descripcion" + infCat.Id]);
                        objInfCategoria.Estado = true;
                        dbc.InformePlantillaCategorias.Add(objInfCategoria);
                        dbc.SaveChanges();
                    }
                }
                #endregion

                #region CheckGraficos

                IQueryable<InformeGrafico> informe = from i in dbc.InformeGraficos
                                                     where i.Estado == true
                                                     select i;
                //from i in dbc.InformeGraficos join cg in dbc.InformeCategoriaGraficos on i.Id equals cg.IdGrafico
                //                                     where i.Estado == true
                //                                     select i;
                InformeGrafico[] informeArray = informe.ToArray();


                foreach (InformeGrafico iGrafico in informeArray)
                {
                    if (Convert.ToString(collection[iGrafico.Nombre]) != null)
                    {
                        objInfGrafico.IdPlantilla = objInfPlantilla.Id;
                        var id = dbc.InformeCategoriaGraficos.Single(x => x.IdGrafico == iGrafico.Id);
                        objInfGrafico.IdCategoriaGraficos = id.Id;
                        objInfGrafico.Estado = true;
                        dbc.InformePlantillaGraficos.Add(objInfGrafico);
                        dbc.SaveChanges();
                    }
                }
                #endregion
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objInfPlantilla.Id.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }





        public ActionResult ListarInformePlantillaTodo(int i = 0)
        {
            // int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.InformeGraficosCategoriasListado().ToList();

            var query3 = (from q2 in query
                          select new
                          {
                              q2.IdCategoria,
                          }).Distinct();

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query2 = (from q3 in query3.Skip(skip).Take(take).ToList()
                          join q2 in query on q3.IdCategoria equals q2.IdCategoria
                          select q2);


            return Json(new { Data = query2, Cantidad = query3.Count() }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult ObtenerDatosCategoria(int id = 0)
        {
            using (var context = new CoreEntities())
            {
                //Thread.Sleep(3000);
                var result = dbc.InformePlantillaCategoriasListar(id).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ObtenerDatosGraficos(int id = 0)
        {
            using (var context = new CoreEntities())
            {
                //Thread.Sleep(3000);
                var result = dbc.InformePlantillaGraficosListar(id).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Editar(FormCollection collection, InformePlantillaCategoria objInfCategoria, InformePlantillaGrafico objInfGrafico)
        {
            //Validar repetido
            int existe = 0;
            string titulo = Convert.ToString(Request.Params["Titulo"].ToString());
            int IdTitulo = Convert.ToInt32(Request.Params["Id"].ToString());

            var objInfPlantilla = dbc.InformePlantillas.Where(x => x.Id == IdTitulo).SingleOrDefault();

            string tituloActual = Convert.ToString(dbc.InformePlantillas.Single(x => x.Id == IdTitulo).Titulo);
            var result = (from inf in dbc.InformePlantillas.Where(x => x.Titulo == titulo && x.Estado == true && x.Titulo != tituloActual).ToList()
                          select new
                          {
                              titulo = inf.Titulo
                          }).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + "El Nombre de la Plantilla ya existe." + "','0');}window.onload=init;</script>");
            }

            int sw = 0;
            string msjError = string.Empty;

            if (String.IsNullOrEmpty(titulo)) { sw = 1; msjError = "Ingrese un Titulo"; }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
            }

            #region validaciones
            IQueryable<InformeCategoria> iCat = from c in dbc.InformeCategorias
                                                where c.Estado == true
                                                select c;
            InformeCategoria[] iCatArray = iCat.ToArray();

            string chkCategoria = "";
            foreach (InformeCategoria infCat in iCatArray)
            {
                if (Convert.ToString(collection[infCat.Nombre]) != null)
                {
                    if (Convert.ToString(collection["Descripcion" + infCat.Id]) != null)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(collection["Descripcion" + infCat.Id])))
                        {
                            sw = 1; msjError = "Se debe completar la Descripción de " + infCat.Nombre;
                        }
                        chkCategoria = Convert.ToString(collection[infCat.Nombre]);
                        if (sw == 1)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                        }
                    }
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
                }
                objInfPlantilla.Titulo = titulo;
                Convert.ToInt32(objInfPlantilla.UsuarioIdCrea);
                Convert.ToDateTime(objInfPlantilla.FechaCrea);

                objInfPlantilla.UsuarioIdModifica = IdUser;
                objInfPlantilla.FechaModifica = DateTime.Now;
                objInfPlantilla.Estado = true;
                //dbc.InformePlantillas.Attach(objInfPlantilla);
                dbc.Entry(objInfPlantilla).State = EntityState.Modified;
                dbc.SaveChanges();

                dbc.InformeCategoriaGraficoEliminar(objInfPlantilla.Id);

                #region CheckCategorias
                IQueryable<InformeCategoria> informeCat = from c in dbc.InformeCategorias where c.Estado == true select c;
                InformeCategoria[] informeCatArray = informeCat.ToArray();
                foreach (InformeCategoria infCat in informeCatArray)
                {
                    if (Convert.ToString(collection[infCat.Nombre]) != null)
                    {
                        objInfCategoria.IdPlantilla = objInfPlantilla.Id;
                        objInfCategoria.IdCategoria = infCat.Id;
                        objInfCategoria.Descripcion = Convert.ToString(collection["Descripcion" + infCat.Id]);
                        objInfCategoria.Estado = true;
                        dbc.InformePlantillaCategorias.Add(objInfCategoria);
                        dbc.SaveChanges();
                    }
                }
                #endregion

                #region CheckGraficos

                IQueryable<InformeGrafico> informe = from i in dbc.InformeGraficos
                                                     where i.Estado == true
                                                     select i;
                //from i in dbc.InformeGraficos join cg in dbc.InformeCategoriaGraficos on i.Id equals cg.IdGrafico
                //                                     where i.Estado == true
                //                                     select i;
                InformeGrafico[] informeArray = informe.ToArray();


                foreach (InformeGrafico iGrafico in informeArray)
                {
                    if (Convert.ToString(collection[iGrafico.Nombre]) != null)
                    {
                        dbc.InformePlantillaListar();
                        objInfGrafico.IdPlantilla = objInfPlantilla.Id;
                        var id = dbc.InformeCategoriaGraficos.Single(x => x.IdGrafico == iGrafico.Id);
                        objInfGrafico.IdCategoriaGraficos = id.Id;
                        objInfGrafico.Estado = true;
                        dbc.InformePlantillaGraficos.Add(objInfGrafico);
                        dbc.SaveChanges();
                    }
                }
                #endregion
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objInfPlantilla.Id.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult Index()
        {
            try
            {
                //int ITO = 0;

                //ITO = Convert.ToInt32(Session["MAI_CLIENT_ITO"].ToString());

                //if (ITO == 0)
                //{
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                ViewBag.Anio = DateTime.Now.Year;
                ViewBag.Mes = DateTime.Now.Month;
                Session["MAIN"] = "mp6";
                ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
                DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);

                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int admin = 0;
                var cola = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti).FirstOrDefault();
                if (cola != null)
                {
                    if (cola.ID_QUEU == 9)
                    {
                        admin = 1;
                    }
                }
                ViewBag.Admin = admin;
                return View(ID_ACCO);
                //}
                //else
                //{
                //    ViewBag.Anio = DateTime.Now.Year;
                //    ViewBag.Mes = DateTime.Now.Month;
                //    return View("ClientITO");
                //}

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

                //return Content("Session Decline, Sign Out And Login, PLease");
            }
        }

        //
        // GET: /Inform/INF_RANKING_TICKETS
        public ActionResult INF_RANKING_TICKETS()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                {
                    ID_CLIE_REP = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                {
                    ID_CLIE_FIN = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                {
                    IdTipoTicket = 0;
                }

                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
                var result = dbc.tktInformeFechasRankingTickets(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta);

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Inform/VTicketReport
        public ActionResult VTicketReport()
        {
            return View();
        }

        public ActionResult VTicketReportDetail()
        {
            return View();
        }


        //
        // GET: /Inform/VTicketReport
        public ActionResult VTicketReportSLA()
        {
            return View();
        }

        //
        // GET: /Inform/INF_TYPE_TICK
        public ActionResult INF_TYPE_TICK()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                int ID_COMP = 0, ID_COMP_END = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0;

                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_COMP) == false)
                {
                    ID_COMP = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_COMP_END) == false)
                {
                    ID_COMP_END = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }

                var result = dbc.tktInformeTicketPorNivel(FechaInicio, FechaFin, ID_ACCO, ID_COMP, ID_COMP_END, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, 0, SubCuenta);
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Inform/INF_TYPE_TICK
        public ActionResult INF_BY_TYPE_TICK(int id = 0, int id1 = 0, int id2 = 0)
        {
            try
            {
                string tt = Convert.ToString(id1);
                //string tit = null, color = null;

                DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                int ID_COMP = 0, ID_COMP_END = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0;
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_COMP) == false)
                {
                    ID_COMP = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_COMP_END) == false)
                {
                    ID_COMP_END = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var result_x = dbc.tktInformeTicketPorNivelDetalle(FechaInicio, FechaFin, ID_ACCO, ID_COMP, ID_COMP_END, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, id1, id, id2, SubCuenta);

                return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Inform/LEGEND_KSA
        public ActionResult LEGEND_KSA(int id, int MONTH, int YEAR)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            //int MONTH = Convert.ToInt32(DateTime.Now.Month);
            //int YEAR = Convert.ToInt32(DateTime.Now.Year);
            var query = dbc.TARGETs.Where(t => t.ID_ACCO == ID_ACCO)
                .Join(dbc.ACCOUNTING_MONTH, t => t.ID_ACCO_MONT, am => am.ID_ACCO_MONT, (t, am) => new { am.ACCO_MONT, t.ID_CATE, t.ID_TARGET, t.ID_MANA_COMM, am.ID_ACCO_YEAR })
                .Where(t => t.ACCO_MONT == MONTH && t.ID_MANA_COMM == id && t.ID_ACCO_YEAR == YEAR);

            var resultCant = (from t in query.ToList()
                              join ct in dbc.CATEGORies on t.ID_CATE equals ct.ID_CATE
                              group ct by new { ct.ID_CATE, ct.NAM_CATE } into g
                              select new
                              {
                                  NAM_CATE = g.Key.NAM_CATE,
                                  ID_CATE = g.Key.ID_CATE
                              });

            var result = (from t in query.ToList()
                          join ct in dbc.CATEGORies on t.ID_CATE equals ct.ID_CATE
                          join ts in dbc.TARGET_SEGMENT on t.ID_TARGET equals ts.ID_TARGET
                          join s in dbc.SEGMENTs on ts.ID_SEGM equals s.ID_SEGM
                          orderby ct.NAM_CATE
                          select new
                          {
                              ID_CATE = ct.ID_CATE,
                              VAL_TARG_SEGM = Convert.ToString(ts.VAL_SUP_TARG_SEGM) + (ts.VAL_INF_TARG_SEGM == ts.VAL_SUP_TARG_SEGM ? "" : " - " + Convert.ToString(ts.VAL_INF_TARG_SEGM)),
                              COLOR = s.COLOR
                          });

            return Json(new { legend = result, Cantlegend = resultCant, }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult INF_SLA(int a = 0, int m = 0)
        {
            int ID_CLIE_REP = 0, ID_CLIE_FIN = 0;
            if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
            {
                ID_CLIE_REP = 0;
            }

            if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
            {
                ID_CLIE_FIN = 0;
            }
            DateTime DatMax = DateTime.Now;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (a != 0 && m != 0)
            {
                DatMax = Convert.ToDateTime(m.ToString() + "/1/" + a.ToString());
            }
            DatMax = DatMax.AddMonths(1).AddDays(-1);

            //var query = dbc.INF_SLA(ID_ACCO, DatMax, ID_CLIE_REP, ID_CLIE_FIN).ToList();
            //var serie = dbc.INF_SLA_COLU(ID_ACCO, DatMax, ID_CLIE_REP).ToList();
            var query = dbc.tktInformeSLA(ID_ACCO, DatMax, ID_CLIE_REP, ID_CLIE_FIN).ToList();
            var serie = dbc.tktInformeSLAColumna(ID_ACCO, DatMax, ID_CLIE_REP, ID_CLIE_FIN).ToList();

            return Json(new { sla = query, total = query.Count(), serie = serie }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult INF_KSA(int a = 0, int m = 0)
        {
            int ID_CLIE_REP = 0, ID_CLIE_FIN = 0;
            if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
            {
                ID_CLIE_REP = 0;
            }

            if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
            {
                ID_CLIE_FIN = 0;
            }

            DateTime DatMax = DateTime.Now;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (a != 0 && m != 0)
            {
                DatMax = Convert.ToDateTime(m.ToString() + "/1/" + a.ToString());
            }
            DatMax = DatMax.AddMonths(1).AddDays(-1);

            //var query = dbc.INF_KSA(ID_ACCO, DatMax, ID_CLIE_REP).ToList();
            //var serie = dbc.INF_KSA_COLU(ID_ACCO, DatMax, ID_CLIE_REP).ToList();
            var query = dbc.tktInformeKSA(ID_ACCO, DatMax, ID_CLIE_REP, ID_CLIE_FIN).ToList();
            var serie = dbc.tktInformeKSAColumna(ID_ACCO, DatMax, ID_CLIE_REP, ID_CLIE_FIN).ToList();
            return Json(new { fcr = query, total = query.Count(), serie = serie }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult INF_BACKLOG()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbc.INF_BACKLOG(ID_ACCO).ToList();

            return Json(new { backlog = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult INF_AVG_TIME_RESO()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                //int ITO = Convert.ToInt32(Session["MAI_CLIENT_ITO"].ToString());
                int ID_COMP = 0, ID_COMP_FIN = 0;
                int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                {
                    ID_CLIE_REP = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                {
                    ID_CLIE_FIN = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                {
                    IdTipoTicket = 0;
                }
                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }

                DateTime tiempoInicial = DateTime.Now.AddMonths(-13);

                var query = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.CREATE_TICK > tiempoInicial);

                var result2 = (from t in query.ToList()
                               group t by new { Anio = t.FEC_TICK.Value.Year, MesO = t.FEC_TICK.Value.Month, Mes = String.Format("{0:MMMM}", t.FEC_TICK), } into g
                               select new
                               {
                                   Anio = g.Key.Anio,
                                   Mes = g.Key.Mes,
                                   MesO = g.Key.MesO
                               }).OrderByDescending(t => t.Anio).ThenByDescending(t => t.MesO).Take(12).OrderBy(t => t.Anio).ThenBy(t => t.MesO);

                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["dtFechaFin"]);
                var result = dbc.tktInformeFechasResolucionMediaTiempo(FechaFin, ID_ACCO, ID_COMP, ID_COMP_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdSubClase, IdClase, IdTipoTicket, SubCuenta).ToList();

                return Json(new { pie = result, legend = result2 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult INF_CRR()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                //int ITO = Convert.ToInt32(Session["MAI_CLIENT_ITO"].ToString());
                int ID_COMP = 0, ID_COMP_FIN = 0;
                int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                var ACCO = dbc.ACCOUNTs.Single(a => a.ID_ACCO == ID_ACCO);
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                {
                    ID_CLIE_REP = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                {
                    ID_CLIE_FIN = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                {
                    IdTipoTicket = 0;
                }
                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }

                DateTime tiempoInicial = DateTime.Now.AddMonths(-13);

                var query = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.CREATE_TICK > tiempoInicial);

                if (ID_CLIE_REP != 0)
                {
                    query = dbc.TICKETs.Where(t => t.ID_COMP == ID_CLIE_REP);
                    ID_COMP = ID_CLIE_REP;
                }
                if (ID_CLIE_FIN != 0)
                {
                    query = dbc.TICKETs.Where(t => t.ID_COMP_END == ID_CLIE_FIN);
                    ID_COMP_FIN = ID_CLIE_FIN;
                }
                var result2 = (from t in query.ToList()
                               group t by new { Anio = t.FEC_TICK.Value.Year, MesO = t.FEC_TICK.Value.Month, Mes = String.Format("{0:MMMM}", t.FEC_TICK), } into g
                               select new
                               {
                                   Anio = g.Key.Anio,
                                   Mes = g.Key.Mes,
                                   MesO = g.Key.MesO
                               }).OrderByDescending(t => t.Anio).ThenByDescending(t => t.MesO).Take(11).OrderBy(t => t.Anio).ThenBy(t => t.MesO);

                //var result = dbc.INF_CRR(ID_ACCO, ID_COMP).ToList();

                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["dtFechaFin"]);
                var result = dbc.tktInformeFechasTasaResolucionLlamadas(FechaFin, ID_ACCO, ID_COMP, ID_COMP_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).ToList();

                return Json(new { line = result, legend = result2, agree = ACCO.CAL_ACCO }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TICK_BY_CLAS()
        {
            int MONTH = Convert.ToInt32(Request.Params["mes"].ToString());
            int YEAR = Convert.ToInt32(Request.Params["anio"].ToString());
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbc.TICK_BY_CLAS(ID_ACCO, YEAR, MONTH);

            return Json(new { pie = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TOLEVEL2()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                {
                    ID_CLIE_REP = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                {
                    ID_CLIE_FIN = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                {
                    IdTipoTicket = 0;
                }
                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }

                DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                var query = dbc.tktInformeFechasNivel2(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).ToList();


                var result = (from t in query.ToList()
                                  //join u in db.CLIENTs on t.ID_ASSI equals u.ID_CLIE
                              join q in dbc.QUEUEs on t.ID_QUEU equals q.ID_QUEU
                              group q by new { q.LEV_QUEU } into g
                              select new
                              {
                                  LEV_QUEU = g.Key.LEV_QUEU,
                                  name = "Level " + Convert.ToString(g.Key.LEV_QUEU),
                                  y = g.Count()
                              }).OrderBy(l => l.LEV_QUEU);

                return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TICKET_STATUS()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                {
                    ID_CLIE_REP = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                {
                    ID_CLIE_FIN = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                {
                    IdTipoTicket = 0;
                }
                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                //var query = dbc.tktListarInformeTicket(MONTH,YEAR,ID_ACCO,ID_CLIE_REP,ID_CLIE_FIN).ToList();
                DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
                var query = dbc.tktListadoFechasInformeTicket(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).ToList();

                var result = (from t in query.ToList()
                              join s in dbc.STATUS on t.ID_STAT_END equals s.ID_STAT
                              group s by new { s.ID_STAT, s.NAM_STAT, s.COL_INDV_STAT, s.ORD_STAT } into g
                              select new
                              {
                                  ORD_STAT = g.Key.ORD_STAT,
                                  ID_STAT = g.Key.ID_STAT,
                                  name = g.Key.NAM_STAT.ToLower(),
                                  y = g.Count(),
                                  color = g.Key.COL_INDV_STAT,
                                  url = "#/IndexPlantilla/"// + Convert.ToString(t.ID_STAT_END) + "/1"
                              }).OrderBy(r => r.ORD_STAT);
                //var idjfsijf = result.ToList();

                return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult INF_WORKLOAD()
                {
                    try
                    {
                        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

                        int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                            IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0, diaCorte = 0;

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                        {
                            ID_CLIE_REP = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                        {
                            ID_CLIE_FIN = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                        {
                            IdAreaResponsable = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                        {
                            IdAsignadoA = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                        {
                            IdCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                        {
                            IdSubCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                        {
                            IdClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                        {
                            IdSubClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                        {
                            IdTipoTicket = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["DiaCorte"]), out diaCorte) == false)
                        {
                            diaCorte = 0;
                        }
                        int SubCuenta = 0;
                        try
                        {
                            SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                        }
                        catch
                        {
                            SubCuenta = 0;
                        }
                        int ID_COMP = 0, ID_COMP_FIN = 0;

                        ID_COMP = ID_CLIE_REP;
                        ID_COMP_FIN = ID_CLIE_FIN;

                        var color = (from s in dbc.STATUS.ToList()
                                     orderby s.ORD_STAT
                                     group s by new { s.COL_GRAP_STAT } into g
                                     select new
                                     {
                                         g.Key.COL_GRAP_STAT
                                     });

                        //var query = dbc.INF_WORKLOAD(ID_ACCO, ID_QUEU, ID_COMP, ID_COMP_FIN).ToList();
                        DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["dtFechaInicio"]);
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["dtFechaFin"]);

                        var query = dbc.tktInformeFechasTrabajo(FechaInicio, FechaFin, ID_ACCO, ID_COMP, ID_COMP_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, diaCorte, SubCuenta).ToList();

                        return Json(new { workload = query, color = color }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult TICKET_PRIORITY()
                {
                    try
                    {
                        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int ID_CLIE_REP = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                            IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                        {
                            ID_CLIE_REP = 0;
                        }
                        int ID_CLIE_FIN = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                        {
                            ID_CLIE_FIN = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                        {
                            IdAreaResponsable = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                        {
                            IdAsignadoA = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                        {
                            IdCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                        {
                            IdSubCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                        {
                            IdClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                        {
                            IdSubClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                        {
                            IdTipoTicket = 0;
                        }
                        int SubCuenta = 0;
                        try
                        {
                            SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                        }
                        catch
                        {
                            SubCuenta = 0;
                        }
                        //var query = dbc.tktListarInformeTicket(MONTH, YEAR, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN).ToList();
                        DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
                        var query = dbc.tktListadoFechasInformeTicket(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).Where(x => x.ID_STAT_END != 2).ToList();

                        var result = (from t in query.ToList()
                                      join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                                      group p by new { p.ID_PRIO, p.NAM_PRIO, p.COL_PRIO } into g
                                      select new
                                      {
                                          ID_PRIO = g.Key.ID_PRIO,
                                          name = g.Key.NAM_PRIO.ToLower(),
                                          y = g.Count(),
                                          color = g.Key.COL_PRIO
                                      }).OrderBy(r => r.ID_PRIO);

                        return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult ViewReportChangeSalary()
                {
                    return View();
                }

                public ActionResult ViewReportMemorandum()
                {
                    return View();
                }

                public ActionResult ListCompanyEDyMEF()
                {
                    using (var contexto = new EntityEntities())
                    {
                        var query = contexto.CLASS_ENTITY.Where(x => x.ID_ENTI == 4185 || x.ID_ENTI == 4237 || x.ID_ENTI == 9 || x.ID_ENTI == 31884 && x.ID_TYPE_ENTI == 1);

                        var result = (from x in query.ToList()
                                      join td in contexto.TYPE_DOCUMENTIDENT on x.ID_TYPE_DI equals td.ID_TYPE_DI
                                      select new
                                      {
                                          ID_ENTI = x.ID_ENTI,
                                          COM_NAME = (x.COM_NAME == null ? "" : x.COM_NAME.ToUpper()),
                                          NAM_TYPE_DI = td.NAM_TYPE_DI,
                                          NUM_TYPE_DI = (x.NUM_TYPE_DI == null ? "" : x.NUM_TYPE_DI),
                                      });

                        return Json(new { Data = result.ToList(), Count = query.Count() }, JsonRequestBehavior.AllowGet);
                    }
                }
                public ActionResult ViewReportTicket()
                {
                    return View();
                }
                public ActionResult statusTicketIndicator()
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    DateTime Finic = Convert.ToDateTime(Request.Params["Inic"]);
                    DateTime Ffin = Convert.ToDateTime(Request.Params["Fin"]);

                    int ID_CLIE_REP = 0;
                    if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                    {
                        ID_CLIE_REP = 0;
                    }
                    int SubCuenta = 0;
                    try
                    {
                        SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                    }
                    catch
                    {
                        SubCuenta = 0;
                    }

                    var result = dbc.tktIndicadorEstado(Finic, Ffin, ID_ACCO, ID_CLIE_REP, SubCuenta).ToList();

                    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

                    //if (ID_CLIE_REP > 0)
                    //{
                    //    var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_TICK == 1)
                    //            .Where(t => t.FEC_TICK >= Finic && t.FEC_TICK <= Ffin)
                    //            .Where(t => t.ID_COMP == ID_CLIE_REP)
                    //            .GroupBy(t => t.ID_STAT_END)
                    //            .Select(t => new { ID_STAT_END = t.Key, count = t.Count() });

                    //    var result = (from t in tick.ToList()
                    //                  join tt in dbc.STATUS_TICKET on t.ID_STAT_END equals tt.ID_STAT_TICK
                    //                  select new
                    //                  {
                    //                      name = tt.NAM_STAT_TICK.Substring(0, 1) + tt.NAM_STAT_TICK.Substring(1, tt.NAM_STAT_TICK.Length - 1).ToLower(),
                    //                      y = t.count,
                    //                      color = tt.COL_GRAP_STAT_TICK,
                    //                      url = "/Inform/IND_BY_STATUS_TICK/0/" + Convert.ToString(t.ID_STAT_END) + "/1"
                    //                  });
                    //    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_TICK == 1)
                    //        .Where(t => t.FEC_TICK >= Finic && t.FEC_TICK <= Ffin)
                    //        .GroupBy(t => t.ID_STAT_END)
                    //        .Select(t => new { ID_STAT_END = t.Key, count = t.Count() });

                    //    var result = (from t in tick.ToList()
                    //                  join tt in dbc.STATUS_TICKET on t.ID_STAT_END equals tt.ID_STAT_TICK
                    //                  where tt.VIG_STAT_TICK == true
                    //                  select new
                    //                  {
                    //                      name = tt.NAM_STAT_TICK.Substring(0, 1) + tt.NAM_STAT_TICK.Substring(1, tt.NAM_STAT_TICK.Length - 1).ToLower(),
                    //                      y = t.count,
                    //                      color = tt.COL_GRAP_STAT_TICK,
                    //                      url = "/Inform/IND_BY_STATUS_TICK/0/" + Convert.ToString(t.ID_STAT_END) + "/1"
                    //                  });

                    //    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                    //}
                }
                public ActionResult IND_BY_STATUS_TICK(int id = 0, int id1 = 0, int id2 = 0)
                {
                    string tt = Convert.ToString(id1);
                    string tit = null, color = null;

                    DateTime Finic = Convert.ToDateTime(Request.Params["Inic"].ToString());
                    DateTime Ffin = Convert.ToDateTime(Request.Params["Fin"].ToString());

                    Ffin = Ffin.AddDays(1);

                    try
                    {
                        tit = dbc.CATEGORies.Single(c => c.ID_CATE == id).NAM_CATE.ToLower();
                        color = dbc.STATUS.Single(ttx => ttx.ID_STAT == id1).COL_GRAP_STAT;
                    }
                    catch
                    {
                        tit = "";
                        color = dbc.STATUS.Single(ttx => ttx.ID_STAT == id1).COL_GRAP_STAT;
                    }

                    int ID_CLIE_REP = 0;
                    if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                    {
                        ID_CLIE_REP = 0;
                    }

                    int SubCuenta = 0;
                    try
                    {
                        SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                    }
                    catch
                    {
                        SubCuenta = 0;
                    }

                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                    var listCate = (from c1 in dbc.CATEGORies.Where(c1 => c1.ID_CATE_PARE == null).ToList()
                                    join c2 in dbc.CATEGORies on c1.ID_CATE equals c2.ID_CATE_PARE
                                    join c3 in dbc.CATEGORies on c2.ID_CATE equals c3.ID_CATE_PARE
                                    join c4 in dbc.CATEGORies on c3.ID_CATE equals c4.ID_CATE_PARE
                                    select new
                                    {
                                        ID_CATE_1 = c1.ID_CATE,
                                        ID_CATE_2 = c2.ID_CATE,
                                        ID_CATE_3 = c3.ID_CATE,
                                        ID_CATE_4 = c4.ID_CATE,
                                    }).ToList();

                    var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_STAT_END == id1 && t.ID_TYPE_TICK == 1)
                                                        .Where(t => t.FEC_TICK >= Finic && t.FEC_TICK <= Ffin);

                    if (ID_CLIE_REP > 0)
                    {
                        tick = tick.Where(x => x.ID_COMP == ID_CLIE_REP)
                            .Where(x => x.ID_DOCU_SALE == null);
                    }
                    if (SubCuenta == 1)
                    {
                        tick = tick.Where(x => x.SubCuenta == "INTERNO");
                    }
                    else
                    if (SubCuenta == 2)
                    {
                        tick = tick.Where(x => x.SubCuenta == "EXTERNO");
                    }

                    var tick_cate = (from t in tick.ToList()
                                     join lc in listCate on t.ID_CATE equals lc.ID_CATE_4
                                     select new
                                     {
                                         lc.ID_CATE_1,
                                         lc.ID_CATE_2,
                                         lc.ID_CATE_3,
                                         lc.ID_CATE_4
                                     });

                    if (id2 == 1)
                    {
                        var result = tick_cate.GroupBy(lc => lc.ID_CATE_1).Select(t => new { ID_CATE = t.Key, COUNT = t.Count() }).ToList();
                        var result_x = (from x in result
                                        join c in dbc.CATEGORies on x.ID_CATE equals c.ID_CATE
                                        select new
                                        {
                                            name = c.NAM_CATE.ToLower(),
                                            y = x.COUNT,
                                            url = "/Inform/IND_BY_STATUS_TICK/" + Convert.ToString(x.ID_CATE) + "/" + tt + "/2/",
                                            title = "Category",//@ResourceLanguaje.Resource.TicketsByCategory,
                                            color = color,
                                        }).OrderByDescending(x => x.y);

                        return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);
                    }

                    if (id2 == 2)
                    {
                        var result = tick_cate.Where(x => x.ID_CATE_1 == id).GroupBy(lc => lc.ID_CATE_2).Select(t => new { ID_CATE = t.Key, COUNT = t.Count() }).ToList();
                        var result_x = (from x in result
                                        join c in dbc.CATEGORies.ToList() on x.ID_CATE equals c.ID_CATE
                                        select new
                                        {
                                            name = c.NAM_CATE.ToLower(),
                                            y = x.COUNT,
                                            url = "/Inform/IND_BY_STATUS_TICK/" + Convert.ToString(x.ID_CATE) + "/" + tt + "/3",
                                            title = "SubCategory",//@ResourceLanguaje.Resource.TicketsByCategory+": "+tit ,
                                            color = color,
                                        }).OrderByDescending(x => x.y);

                        return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);
                    }
                    if (id2 == 3)
                    {
                        var result = tick_cate.Where(x => x.ID_CATE_2 == id).GroupBy(lc => lc.ID_CATE_3).Select(t => new { ID_CATE = t.Key, COUNT = t.Count() }).ToList();
                        var result_x = (from x in result
                                        join c in dbc.CATEGORies.ToList() on x.ID_CATE equals c.ID_CATE
                                        select new
                                        {
                                            name = c.NAM_CATE.ToLower(),
                                            y = x.COUNT,
                                            url = "/Inform/IND_BY_STATUS_TICK/" + Convert.ToString(x.ID_CATE) + "/" + tt + "/4",
                                            title = "Class",//,@ResourceLanguaje.Resource.TicketsByCategory + ": " + tit,
                                            color = color,
                                        }).OrderByDescending(x => x.y);

                        return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);
                    }
                    if (id2 == 4)
                    {
                        var result = tick_cate.Where(x => x.ID_CATE_3 == id).GroupBy(lc => lc.ID_CATE_4).Select(t => new { ID_CATE = t.Key, COUNT = t.Count() }).ToList();
                        var result_x = (from x in result
                                        join c in dbc.CATEGORies.ToList() on x.ID_CATE equals c.ID_CATE
                                        select new
                                        {
                                            name = c.NAM_CATE.ToLower(),
                                            y = x.COUNT,
                                            url = "/Inform/IND_BY_STATUS_TICK/" + Convert.ToString(x.ID_CATE) + "/" + tt + "/0",
                                            title = "SubClass",//@ResourceLanguaje.Resource.TicketsByCategory + ": " + tit,
                                            color = color,
                                        }).OrderByDescending(x => x.y);

                        return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }
                public ActionResult priorityTicketIndicator()
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    DateTime Finic = Convert.ToDateTime(Request.Params["Inic"]);
                    DateTime Ffin = Convert.ToDateTime(Request.Params["Fin"]);

                    int ID_CLIE_REP = 0;
                    if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                    {
                        ID_CLIE_REP = 0;
                    }
                    int SubCuenta = 0;
                    try
                    {
                        SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                    }
                    catch
                    {
                        SubCuenta = 0;
                    }
                    var result = dbc.tktIndicadorPrioridad(Finic, Ffin, ID_ACCO, ID_CLIE_REP, SubCuenta).ToList();

                    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

                    //if (ID_CLIE_REP > 0)
                    //{
                    //    var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_TICK == 1)
                    //            .Where(t => t.FEC_TICK >= Finic && t.FEC_TICK <= Ffin)
                    //            .Where(t => t.ID_COMP == ID_CLIE_REP)
                    //            .GroupBy(t => t.ID_PRIO)
                    //            .Select(t => new { ID_PRIO = t.Key, count = t.Count() });
                    //    var result = (from t in tick.ToList()
                    //                  join tt in dbc.PRIORITies on t.ID_PRIO equals tt.ID_PRIO
                    //                  select new
                    //                  {
                    //                      name = tt.DES_PRIO.Substring(0, 1) + tt.DES_PRIO.Substring(1, tt.DES_PRIO.Length - 1).ToLower(),
                    //                      y = t.count,
                    //                      color = tt.COL_PRIO,
                    //                      url = "/Inform/IND_BY_PRIORITY_TICK/0/" + Convert.ToString(t.ID_PRIO) + "/1"
                    //                  });
                    //    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_TICK == 1)
                    //        .Where(t => t.FEC_TICK >= Finic && t.FEC_TICK <= Ffin)
                    //        .GroupBy(t => t.ID_PRIO)
                    //            .Select(t => new { ID_PRIO = t.Key, count = t.Count() });

                    //    var result = (from t in tick.ToList()
                    //                  join tt in dbc.PRIORITies on t.ID_PRIO equals tt.ID_PRIO
                    //                  select new
                    //                  {
                    //                      name = tt.DES_PRIO.Substring(0, 1) + tt.DES_PRIO.Substring(1, tt.DES_PRIO.Length - 1).ToLower(),
                    //                      y = t.count,
                    //                      color = tt.COL_PRIO,
                    //                      url = "/Inform/IND_BY_PRIORITY_TICK/0/" + Convert.ToString(t.ID_PRIO) + "/1"
                    //                  });
                    //    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                    //}
                }
                public ActionResult IND_BY_PRIORITY_TICK(int id = 0, int id1 = 0, int id2 = 0)
                {
                    string tt = Convert.ToString(id1);
                    string tit = null, color = null;

                    DateTime Finic = Convert.ToDateTime(Request.Params["Inic"].ToString());
                    DateTime Ffin = Convert.ToDateTime(Request.Params["Fin"].ToString());

                    Ffin = Ffin.AddDays(1);

                    try
                    {
                        tit = dbc.CATEGORies.Single(c => c.ID_CATE == id).NAM_CATE.ToLower();
                        color = dbc.PRIORITies.Single(ttx => ttx.ID_PRIO == id1).COL_PRIO;
                    }
                    catch
                    {
                        tit = "";
                        color = dbc.PRIORITies.Single(ttx => ttx.ID_PRIO == id1).COL_PRIO;
                    }

                    int ID_CLIE_REP = 0;
                    if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                    {
                        ID_CLIE_REP = 0;
                    }
                    int SubCuenta = 0;
                    try
                    {
                        SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                    }
                    catch
                    {
                        SubCuenta = 0;
                    }


                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                    var listCate = (from c1 in dbc.CATEGORies.Where(c1 => c1.ID_CATE_PARE == null).ToList()
                                    join c2 in dbc.CATEGORies on c1.ID_CATE equals c2.ID_CATE_PARE
                                    join c3 in dbc.CATEGORies on c2.ID_CATE equals c3.ID_CATE_PARE
                                    join c4 in dbc.CATEGORies on c3.ID_CATE equals c4.ID_CATE_PARE
                                    select new
                                    {
                                        ID_CATE_1 = c1.ID_CATE,
                                        ID_CATE_2 = c2.ID_CATE,
                                        ID_CATE_3 = c3.ID_CATE,
                                        ID_CATE_4 = c4.ID_CATE,
                                    }).ToList();

                    var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_PRIO == id1 && t.ID_TYPE_TICK == 1)
                                                        .Where(t => t.FEC_TICK >= Finic && t.FEC_TICK <= Ffin);

                    if (ID_CLIE_REP > 0)
                    {
                        tick = tick.Where(x => x.ID_COMP == ID_CLIE_REP)
                            .Where(x => x.ID_DOCU_SALE == null);
                    }
                    if (SubCuenta == 1)
                    {
                        tick = tick.Where(x => x.SubCuenta == "INTERNO");
                    }
                    else
                    if (SubCuenta == 2)
                    {
                        tick = tick.Where(x => x.SubCuenta == "EXTERNO");
                    }
                    var tick_cate = (from t in tick.ToList()
                                     join lc in listCate on t.ID_CATE equals lc.ID_CATE_4
                                     select new
                                     {
                                         lc.ID_CATE_1,
                                         lc.ID_CATE_2,
                                         lc.ID_CATE_3,
                                         lc.ID_CATE_4
                                     });

                    if (id2 == 1)
                    {
                        var result = tick_cate.GroupBy(lc => lc.ID_CATE_1).Select(t => new { ID_CATE = t.Key, COUNT = t.Count() }).ToList();
                        var result_x = (from x in result
                                        join c in dbc.CATEGORies on x.ID_CATE equals c.ID_CATE
                                        select new
                                        {
                                            name = c.NAM_CATE.ToLower(),
                                            y = x.COUNT,
                                            url = "/Inform/IND_BY_PRIORITY_TICK/" + Convert.ToString(x.ID_CATE) + "/" + tt + "/2/",
                                            title = "Category",//@ResourceLanguaje.Resource.TicketsByCategory,
                                            color = color,
                                        }).OrderByDescending(x => x.y);

                        return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);
                    }

                    if (id2 == 2)
                    {
                        var result = tick_cate.Where(x => x.ID_CATE_1 == id).GroupBy(lc => lc.ID_CATE_2).Select(t => new { ID_CATE = t.Key, COUNT = t.Count() }).ToList();
                        var result_x = (from x in result
                                        join c in dbc.CATEGORies.ToList() on x.ID_CATE equals c.ID_CATE
                                        select new
                                        {
                                            name = c.NAM_CATE.ToLower(),
                                            y = x.COUNT,
                                            url = "/Inform/IND_BY_PRIORITY_TICK/" + Convert.ToString(x.ID_CATE) + "/" + tt + "/3",
                                            title = "SubCategory",//@ResourceLanguaje.Resource.TicketsByCategory+": "+tit ,
                                            color = color,
                                        }).OrderByDescending(x => x.y);

                        return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);
                    }
                    if (id2 == 3)
                    {
                        var result = tick_cate.Where(x => x.ID_CATE_2 == id).GroupBy(lc => lc.ID_CATE_3).Select(t => new { ID_CATE = t.Key, COUNT = t.Count() }).ToList();
                        var result_x = (from x in result
                                        join c in dbc.CATEGORies.ToList() on x.ID_CATE equals c.ID_CATE
                                        select new
                                        {
                                            name = c.NAM_CATE.ToLower(),
                                            y = x.COUNT,
                                            url = "/Inform/IND_BY_PRIORITY_TICK/" + Convert.ToString(x.ID_CATE) + "/" + tt + "/4",
                                            title = "Class",//,@ResourceLanguaje.Resource.TicketsByCategory + ": " + tit,
                                            color = color,
                                        }).OrderByDescending(x => x.y);

                        return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);
                    }
                    if (id2 == 4)
                    {
                        var result = tick_cate.Where(x => x.ID_CATE_3 == id).GroupBy(lc => lc.ID_CATE_4).Select(t => new { ID_CATE = t.Key, COUNT = t.Count() }).ToList();
                        var result_x = (from x in result
                                        join c in dbc.CATEGORies.ToList() on x.ID_CATE equals c.ID_CATE
                                        select new
                                        {
                                            name = c.NAM_CATE.ToLower(),
                                            y = x.COUNT,
                                            url = "/Inform/IND_BY_PRIORITY_TICK/" + Convert.ToString(x.ID_CATE) + "/" + tt + "/0",
                                            title = "SubClass",//@ResourceLanguaje.Resource.TicketsByCategory + ": " + tit,
                                            color = color,
                                        }).OrderByDescending(x => x.y);

                        return Json(new { Data = result_x }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }
                public ActionResult gradeResponseSurvey()
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    DateTime Finic = Convert.ToDateTime(Request.Params["Inic"]);
                    DateTime Ffin = Convert.ToDateTime(Request.Params["Fin"]);

                    Ffin = Ffin.AddDays(1);

                    int ID_CLIE_REP = 0;
                    if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                    {
                        ID_CLIE_REP = 0;
                    }
                    int SubCuenta = 0;
                    try
                    {
                        SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                    }
                    catch
                    {
                        SubCuenta = 0;
                    }

                    using (var context = new EntityEntities())
                    {
                        if (ID_CLIE_REP > 0)
                        {
                            var tick = (from t in dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO)
                               .Where(t => t.ID_COMP == ID_CLIE_REP)
                                        select new
                                        {
                                            ID_TICK = t.ID_TICK,
                                            SubCuenta = (t.SubCuenta == null ? "" : t.SubCuenta)
                                        }).ToList();
                            if (SubCuenta == 1)
                            {
                                tick = tick.Where(x => x.SubCuenta == "INTERNO").ToList();
                            }
                            else if (SubCuenta == 2)
                            {
                                tick = tick.Where(x => x.SubCuenta == "EXTERNO").ToList();
                            }

                            var qticket = context.QUESTION_TICKET.Where(x => x.VAL_QUES_TICK != null && (x.ID_QUES == 13 || x.ID_QUES == 14 || x.ID_QUES == 15))
                                .Where(qt => qt.CREATED >= Finic && qt.CREATED <= Ffin).ToList();
                            var survey = (from t in tick
                                          join st in context.SURVEY_TICKET on t.ID_TICK equals st.ID_TICK
                                          join qt in qticket.ToList() on st.ID_SURV_TICK equals qt.ID_SURV_TICK
                                          group qt by new { qt.VAL_QUES_TICK } into g
                                          select new
                                          {
                                              VAL_QUES_TICK = g.Key,
                                              color = (g.Key.VAL_QUES_TICK == "1" ? "#3366CC" : g.Key.VAL_QUES_TICK == "2" ? "#DC3912" : g.Key.VAL_QUES_TICK == "3" ?
                                              "FF9900" : g.Key.VAL_QUES_TICK == "4" ? "#109618" : g.Key.VAL_QUES_TICK == "5" ? "#990099" : ""),
                                              count = g.Count()
                                          });

                            var result = (from t in survey.ToList()
                                          select new
                                          {
                                              name = t.VAL_QUES_TICK.VAL_QUES_TICK,
                                              y = t.count,
                                              color = t.color
                                          });

                            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO).ToList();
                            if (SubCuenta == 1)
                            {
                                tick = tick.Where(x => x.SubCuenta == "INTERNO").ToList();
                            }
                            else if (SubCuenta == 2)
                            {
                                tick = tick.Where(x => x.SubCuenta == "EXTERNO").ToList();
                            }

                            var qticket = context.QUESTION_TICKET.Where(x => x.VAL_QUES_TICK != null && (x.ID_QUES == 13 || x.ID_QUES == 14 || x.ID_QUES == 15))
                                            .Where(qt => qt.CREATED >= Finic && qt.CREATED <= Ffin).ToList();

                            var survey = (from t in tick
                                          join st in context.SURVEY_TICKET on t.ID_TICK equals st.ID_TICK
                                          join qt in qticket.ToList() on st.ID_SURV_TICK equals qt.ID_SURV_TICK
                                          group qt by new { qt.VAL_QUES_TICK } into g
                                          select new
                                          {
                                              VAL_QUES_TICK = g.Key,
                                              color = (g.Key.VAL_QUES_TICK == "1" ? "#3366CC" : g.Key.VAL_QUES_TICK == "2" ? "#DC3912" : g.Key.VAL_QUES_TICK == "3" ?
                                              "FF9900" : g.Key.VAL_QUES_TICK == "4" ? "#109618" : g.Key.VAL_QUES_TICK == "5" ? "#990099" : ""),
                                              count = g.Count()
                                          });

                            var result = (from t in survey.ToList()
                                          select new
                                          {
                                              name = t.VAL_QUES_TICK.VAL_QUES_TICK,
                                              y = t.count,
                                              color = t.color
                                          });

                            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                public ActionResult ResponseSurvey()
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    DateTime Finic = Convert.ToDateTime(Request.Params["Inic"]);
                    DateTime Ffin = Convert.ToDateTime(Request.Params["Fin"]);

                    Ffin = Ffin.AddDays(1);
                    int ID_CLIE_REP = 0;
                    if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                    {
                        ID_CLIE_REP = 0;
                    }
                    int SubCuenta = 0;
                    try
                    {
                        SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                    }
                    catch
                    {
                        SubCuenta = 0;
                    }

                    using (var context = new EntityEntities())
                    {
                        if (ID_CLIE_REP > 0)
                        {
                            var tick = (from t in dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO)
                               .Where(t => t.ID_COMP == ID_CLIE_REP)
                                        select new
                                        {
                                            ID_TICK = t.ID_TICK,
                                            SubCuenta = (t.SubCuenta == null ? "" : t.SubCuenta)
                                        }).ToList();

                            if (SubCuenta == 1)
                            {
                                tick = tick.Where(x => x.SubCuenta == "INTERNO").ToList();
                            }
                            else if (SubCuenta == 2)
                            {
                                tick = tick.Where(x => x.SubCuenta == "EXTERNO").ToList();
                            }

                            var reply = context.QUESTION_TICKET.Where(x => x.VAL_QUES_TICK != null)
                                                 .Where(qt => qt.CREATED >= Finic && qt.CREATED <= Ffin).ToList();

                            var replyFinal = (from t in tick
                                              join st in context.SURVEY_TICKET on t.ID_TICK equals st.ID_TICK
                                              join qt in reply on st.ID_SURV_TICK equals qt.ID_SURV_TICK
                                              select new
                                              {
                                                  qt.ID_QUES_TICK
                                              }).Count();


                            var noreply = context.QUESTION_TICKET.Where(x => x.VAL_QUES_TICK == null)
                                                 .Where(qt => qt.CREATED >= Finic && qt.CREATED <= Ffin).ToList();

                            var noreplyFinal = (from t in tick
                                                join st in context.SURVEY_TICKET on t.ID_TICK equals st.ID_TICK
                                                join qt in noreply on st.ID_SURV_TICK equals qt.ID_SURV_TICK
                                                select new
                                                {
                                                    qt.ID_QUES_TICK
                                                }).Count();

                            return Json(new { reply = replyFinal, noreply = noreplyFinal }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var tick = (from t in dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO)
                                        select new
                                        {
                                            ID_TICK = t.ID_TICK,
                                            SubCuenta = (t.SubCuenta == null ? "" : t.SubCuenta)
                                        }).ToList();

                            if (SubCuenta == 1)
                            {
                                tick = tick.Where(x => x.SubCuenta == "INTERNO").ToList();
                            }
                            else if (SubCuenta == 2)
                            {
                                tick = tick.Where(x => x.SubCuenta == "EXTERNO").ToList();
                            }

                            var reply = context.QUESTION_TICKET.Where(x => x.VAL_QUES_TICK != null)
                                                 .Where(qt => qt.CREATED >= Finic && qt.CREATED <= Ffin).ToList();

                            var replyFinal = (from t in tick.ToList()
                                              join st in context.SURVEY_TICKET on t.ID_TICK equals st.ID_TICK
                                              join qt in reply on st.ID_SURV_TICK equals qt.ID_SURV_TICK
                                              select new
                                              {
                                                  qt.ID_QUES_TICK
                                              }).Count();
                            var noreply = context.QUESTION_TICKET.Where(x => x.VAL_QUES_TICK == null)
                                                 .Where(qt => qt.CREATED >= Finic && qt.CREATED <= Ffin).ToList();

                            var noreplyFinal = (from t in tick.ToList()
                                                join st in context.SURVEY_TICKET on t.ID_TICK equals st.ID_TICK
                                                join qt in noreply on st.ID_SURV_TICK equals qt.ID_SURV_TICK
                                                select new
                                                {
                                                    qt.ID_QUES_TICK
                                                }).Count();

                            return Json(new { reply = replyFinal, noreply = noreplyFinal }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                public ActionResult TicketNivelEscalamiento()
                {
                    try
                    {
                        int idCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int idClienteInicial = 0, idClienteFinal = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out idClienteInicial) == false)
                        {
                            idClienteInicial = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out idClienteFinal) == false)
                        {
                            idClienteFinal = 0;
                        }
                        DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                        var resultado = dbc.InfTicketEscalamiento(idCuenta, idClienteInicial, idClienteFinal, FechaInicio, FechaFin).ToList();

                        return Json(new { pie = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult TicketTipo()
                {
                    try
                    {
                        int idCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int idClienteInicial = 0, idClienteFinal = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out idClienteInicial) == false)
                        {
                            idClienteInicial = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out idClienteFinal) == false)
                        {
                            idClienteFinal = 0;
                        }
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);



                        var resultado = dbc.InfTipoTicket(FechaFin, idCuenta, idClienteInicial, idClienteFinal).ToList();
                        return Json(new { pie = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult TicketPrioridadSLA()
                {
                    try
                    {
                        int idCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int idClienteInicial = 0, idClienteFinal = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out idClienteInicial) == false)
                        {
                            idClienteInicial = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out idClienteFinal) == false)
                        {
                            idClienteFinal = 0;
                        }
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                        var resultado = dbc.InfPrioridadSLA(FechaFin, idCuenta, idClienteInicial, idClienteFinal).ToList();

                        return Json(new { pie = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                #region "Detalle - Informe Mensual"
                public ActionResult Detalle()
                {

                    return View();
                }
                public ActionResult ListarDetalle()
                {
                    int
                        clieRep = 0,
                        clieFin = 0,
                        areaResponsable = 0,
                        asignadoA = 0,
                        categoria = 0,
                        subCategoria = 0,
                        clase = 0,
                        subClase = 0,
                        tipoTicket = 0,
                        idAcco = Convert.ToInt32(Session["ID_ACCO"]);

                    String idReporte = Request.Params["idReporte"];
                    String tipoReporte = Request.Params["tipoReporte"];
                    if (int.TryParse(Convert.ToString(Request.Params["areaResponsable"]), out areaResponsable) == false)
                    {
                        areaResponsable = 0;
                    }
                    if (int.TryParse(Convert.ToString(Request.Params["categoria"]), out categoria) == false)
                    {
                        categoria = 0;
                    }
                    if (int.TryParse(Convert.ToString(Request.Params["subCategoria"]), out subCategoria) == false)
                    {
                        subCategoria = 0;
                    }
                    if (int.TryParse(Convert.ToString(Request.Params["clase"]), out clase) == false)
                    {
                        clase = 0;
                    }
                    if (int.TryParse(Convert.ToString(Request.Params["subClase"]), out subClase) == false)
                    {
                        subClase = 0;
                    }
                    if (int.TryParse(Convert.ToString(Request.Params["tipoTicket"]), out tipoTicket) == false)
                    {
                        tipoTicket = 0;
                    }
                    DateTime fechaFin = Convert.ToDateTime(Request.Params["fechaFin"]);
                    if (tipoReporte == "estado")
                    {
                        DateTime fechaInicio = Convert.ToDateTime(Request.Params["fechaInicio"]);
                        if (int.TryParse(Convert.ToString(Request.Params["asignadoA"]), out asignadoA) == false)
                        {
                            asignadoA = 0;
                        }
                        List<tktListadoFechasInformeTicket_Detalle_Result> resultado = dbc.tktListadoFechasInformeTicket_Detalle(fechaInicio, fechaFin, idAcco, clieRep, clieFin, areaResponsable, asignadoA, categoria, subCategoria, clase, subClase, tipoTicket, Convert.ToInt32(idReporte)).ToList();
                        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        if (tipoReporte == "prioridad")
                    {
                        DateTime fechaInicio = Convert.ToDateTime(Request.Params["fechaInicio"]);
                        if (int.TryParse(Convert.ToString(Request.Params["asignadoA"]), out asignadoA) == false)
                        {
                            asignadoA = 0;
                        }
                        List<tktListadoFechasInformeTicketxPrioridad_Detalle_Result> resultado = dbc.tktListadoFechasInformeTicketxPrioridad_Detalle(fechaInicio, fechaFin, idAcco, clieRep, clieFin, areaResponsable, asignadoA, categoria, subCategoria, clase, subClase, tipoTicket, Convert.ToInt32(idReporte)).ToList();
                        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    else
                            if (tipoReporte == "tolevel2")
                    {
                        DateTime fechaInicio = Convert.ToDateTime(Request.Params["fechaInicio"]);
                        if (int.TryParse(Convert.ToString(Request.Params["asignadoA"]), out asignadoA) == false)
                        {
                            asignadoA = 0;
                        }
                        List<tktInformeFechasNivel2_Detalle_Result> resultado = dbc.tktInformeFechasNivel2_Detalle(fechaInicio, fechaFin, idAcco, clieRep, clieFin, areaResponsable, asignadoA, categoria, subCategoria, clase, subClase, tipoTicket, Convert.ToInt32(idReporte)).ToList();
                        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    else
                                if (tipoReporte == "SLA")
                    {
                        DateTime fechaInicio = Convert.ToDateTime(Request.Params["fechaInicio"]);
                        List<tktInformeFechasRankingTickets_Detalle_Result> resultado = dbc.tktInformeFechasRankingTickets_Detalle(fechaInicio, fechaFin, idAcco, clieRep, clieFin, areaResponsable, Convert.ToString(Request.Params["asignadoA"]), categoria, subCategoria, clase, subClase, tipoTicket, idReporte).ToList();
                        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    else
                                    if (tipoReporte == "personal_estado")
                    {
                        DateTime fechaInicio = Convert.ToDateTime(Request.Params["fechaInicio"]);
                        List<tktInformeFechasRankingTicketsxPersonal_Estado_Detalle_Result> resultado = dbc.tktInformeFechasRankingTicketsxPersonal_Estado_Detalle(fechaInicio, fechaFin, idAcco, clieRep, clieFin, areaResponsable, Convert.ToString(Request.Params["asignadoA"]), categoria, subCategoria, clase, subClase, tipoTicket, idReporte).ToList();
                        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    else
                                        if (tipoReporte == "workload")
                    {
                        String mes = Request.Params["fechaInicio"];
                        List<tktInformeFechasTrabajo_Detalle_Result> resultado = dbc.tktInformeFechasTrabajo_Detalle(mes, fechaFin, idAcco, areaResponsable, clieRep, clieFin, areaResponsable, asignadoA, categoria, subCategoria, clase, subClase, tipoTicket, idReporte).ToList();
                        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    else
                                            if (tipoReporte == "llamadas")
                    {
                        String mes = Request.Params["fechaInicio"];
                        List<tktInformeFechasTasaResolucionLlamadas_Detalle_Result> resultado = dbc.tktInformeFechasTasaResolucionLlamadas_Detalle(mes, fechaFin, idAcco, clieRep, clieFin, areaResponsable, asignadoA, categoria, subCategoria, clase, subClase, tipoTicket).ToList();
                        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        String mes = Request.Params["fechaInicio"];
                        List<tktInformeFechasResolucionMediaTiempo_Detalle_Result> resultado = dbc.tktInformeFechasResolucionMediaTiempo_Detalle(mes, fechaFin, idAcco, clieRep, clieFin, areaResponsable, asignadoA, categoria, subCategoria, clase, subClase, tipoTicket).ToList();
                        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult ObtenerDatosReporte()
                {

                    String idReporte = Request.Params["idReporte"];
                    String tipoReporte = Request.Params["tipoReporte"];
                    String resultado = "";
                    if (tipoReporte == "estado")
                    {
                        int idReport = Convert.ToInt32(idReporte);
                        resultado = "Tickets por Estado - " + dbc.STATUS.Single(x => x.ID_STAT == idReport).NAM_STAT;
                    }
                    else
                        if (tipoReporte == "prioridad")
                    {
                        int idReport = Convert.ToInt32(idReporte);
                        resultado = "Tickets por Prioridad - " + dbc.PRIORITies.Single(x => x.ID_PRIO == idReport).NAM_PRIO;
                    }
                    else
                            if (tipoReporte == "tolevel2")
                    {
                        int idReport = Convert.ToInt32(idReporte);
                        resultado = "Escalamiento al nivel 2 Level " + dbc.QUEUEs.Single(x => x.ID_QUEU == idReport).LEV_QUEU;
                    }
                    else
                                if (tipoReporte == "SLA")
                    {
                        resultado = "Tickets por acuerdo de nivel de servicio - " + idReporte;
                    }
                    else
                                    if (tipoReporte == "personal_estado")
                    {
                        resultado = "Tickets por Personal/Estado - " + idReporte;
                    }
                    if (tipoReporte == "workload")
                    {
                        resultado = "Trabajo - " + idReporte;
                    }
                    if (tipoReporte == "llamadas")
                    {
                        resultado = "Tasa de resolución de llamadas - " + idReporte;
                    }
                    if (tipoReporte == "resolucion")
                    {
                        resultado = "Resolución media del tiempo - " + idReporte;
                    }

                    return Content(resultado);
                }
                #endregion

                public ActionResult TICKET_P1()
                {
                    try
                    {
                        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                            IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                        {
                            ID_CLIE_REP = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                        {
                            ID_CLIE_FIN = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                        {
                            IdAreaResponsable = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                        {
                            IdAsignadoA = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                        {
                            IdCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                        {
                            IdSubCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                        {
                            IdClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                        {
                            IdSubClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                        {
                            IdTipoTicket = 0;
                        }
                        int SubCuenta = 0;
                        try
                        {
                            SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                        }
                        catch
                        {
                            SubCuenta = 0;
                        }

                        DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                        List<tktInci1ResolvedP123_Result> Solicitudes = dbc.tktInci1ResolvedP123(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).ToList();

                        int resolved = Solicitudes.Where(x => x.ID_PRIO == 1).Where(t => t.ID_STAT_END == 4 || t.ID_STAT_END == 6).Count();
                        int noResolved = Solicitudes.Where(x => x.ID_PRIO == 1).Where(t => t.ID_STAT_END != 4 && t.ID_STAT_END != 6 && t.ID_STAT_END != 2).Count();
                        Ratio obj = new Ratio();
                        obj.Resolved = resolved;
                        obj.NoResolved = noResolved;

                        return Json(obj, JsonRequestBehavior.AllowGet);

                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult TICKET_P2()
                {
                    try
                    {
                        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                            IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                        {
                            ID_CLIE_REP = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                        {
                            ID_CLIE_FIN = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                        {
                            IdAreaResponsable = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                        {
                            IdAsignadoA = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                        {
                            IdCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                        {
                            IdSubCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                        {
                            IdClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                        {
                            IdSubClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                        {
                            IdTipoTicket = 0;
                        }

                        int SubCuenta = 0;
                        try
                        {
                            SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                        }
                        catch
                        {
                            SubCuenta = 0;
                        }
                        DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                        List<tktInci1ResolvedP123_Result> Solicitudes = dbc.tktInci1ResolvedP123(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).ToList();

                        int resolved = Solicitudes.Where(t => t.ID_STAT_END == 4 || t.ID_STAT_END == 6).Count();
                        int noResolved = Solicitudes.Where(x => x.ID_PRIO == 2).Where(t => t.ID_STAT_END != 4 && t.ID_STAT_END != 6 && t.ID_STAT_END != 2).Count();
                        Ratio obj = new Ratio();
                        obj.Resolved = resolved;
                        obj.NoResolved = noResolved;

                        return Json(obj, JsonRequestBehavior.AllowGet);

                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult TICKET_P3()
                {
                    try
                    {
                        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                            IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                        {
                            ID_CLIE_REP = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                        {
                            ID_CLIE_FIN = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                        {
                            IdAreaResponsable = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                        {
                            IdAsignadoA = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                        {
                            IdCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                        {
                            IdSubCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                        {
                            IdClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                        {
                            IdSubClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                        {
                            IdTipoTicket = 0;
                        }
                        int SubCuenta = 0;
                        try
                        {
                            SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                        }
                        catch
                        {
                            SubCuenta = 0;
                        }

                        DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                        List<tktInci1ResolvedP123_Result> Solicitudes = dbc.tktInci1ResolvedP123(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).ToList();

                        int resolved = Solicitudes.Where(x => x.ID_PRIO == 3).Where(t => t.ID_STAT_END == 4 || t.ID_STAT_END == 6).Count();
                        int noResolved = Solicitudes.Where(x => x.ID_PRIO == 3).Where(t => t.ID_STAT_END != 4 && t.ID_STAT_END != 6 && t.ID_STAT_END != 2).Count();
                        Ratio obj = new Ratio();
                        obj.Resolved = resolved;
                        obj.NoResolved = noResolved;

                        return Json(obj, JsonRequestBehavior.AllowGet);

                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult TICKET_L1()
                {
                    try
                    {
                        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                            IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                        {
                            ID_CLIE_REP = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                        {
                            ID_CLIE_FIN = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                        {
                            IdAreaResponsable = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                        {
                            IdAsignadoA = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                        {
                            IdCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                        {
                            IdSubCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                        {
                            IdClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                        {
                            IdSubClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                        {
                            IdTipoTicket = 0;
                        }
                        int SubCuenta = 0;
                        try
                        {
                            SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                        }
                        catch
                        {
                            SubCuenta = 0;
                        }

                        DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                        List<tktListadoFechasInformeTickets_Result> Solicitudes = dbc.tktListadoFechasInformeTickets(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).ToList();

                        int resolved = Solicitudes.Where(x => x.ID_TYPE_TICK == 1).Where(o => o.ID_QUEU == 5).Where(t => t.ID_STAT_END == 4 || t.ID_STAT_END == 6).Count();
                        int total = Solicitudes.Where(x => x.ID_TYPE_TICK == 1).Where(t => t.ID_STAT_END != 2).Count();
                        int noResolved = total - resolved;
                        Ratio obj = new Ratio();
                        obj.Resolved = resolved;
                        obj.NoResolved = noResolved;

                        return Json(obj, JsonRequestBehavior.AllowGet);

                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }

                public ActionResult TICKET_CLOSED()
                {
                    try
                    {
                        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                        int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                            IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                        {
                            ID_CLIE_REP = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                        {
                            ID_CLIE_FIN = 0;
                        }

                        if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                        {
                            IdAreaResponsable = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                        {
                            IdAsignadoA = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                        {
                            IdCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                        {
                            IdSubCategoria = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                        {
                            IdClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                        {
                            IdSubClase = 0;
                        }
                        if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                        {
                            IdTipoTicket = 0;
                        }

                        int SubCuenta = 0;
                        try
                        {
                            SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                        }
                        catch
                        {
                            SubCuenta = 0;
                        }
                        DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                        DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                        List<tktListadoFechasInformeTickets_Result> Solicitudes = dbc.tktListadoFechasInformeTickets(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).ToList();

                        int resolved = Solicitudes.Where(t => t.ID_STAT_END == 4 || t.ID_STAT_END == 6).Count();
                        int total = Solicitudes.Where(t => t.ID_STAT_END != 2).Count();
                        int noResolved = total - resolved;
                        Ratio obj = new Ratio();
                        obj.Resolved = resolved;
                        obj.NoResolved = noResolved;

                        return Json(obj, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                }



            
        public class Ratio
        {
            public int Resolved { get; set; }
            public int NoResolved { get; set; }
        }

        public ActionResult Soli_InSLA()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
                int ID_CLIE_REP = 0, ID_CLIE_FIN = 0, IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;
                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_REP"]), out ID_CLIE_REP) == false)
                {
                    ID_CLIE_REP = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["ID_CLIE_FIN"]), out ID_CLIE_FIN) == false)
                {
                    ID_CLIE_FIN = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                {
                    IdTipoTicket = 0;
                }
                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }

                DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);

                int inSLA = dbc.tktListaFechasInSLA(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).Count();
                int total = dbc.tktListadoFechasInformeTickets(FechaInicio, FechaFin, ID_ACCO, ID_CLIE_REP, ID_CLIE_FIN, IdAreaResponsable, IdAsignadoA, IdCategoria, IdSubCategoria, IdClase, IdSubClase, IdTipoTicket, SubCuenta).Count();

                int outSLA = total - inSLA;

                Ratio obj = new Ratio();
                obj.Resolved = inSLA;
                obj.NoResolved = outSLA;

                return Json(obj, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }


        //codigo para desabilitar el codigo
        public ActionResult DesabilitarEstadoPlantilla(int id = 0)
        {

            bool estado = false;
            var result = (from inf in dbc.InformePlantillas.Where(x => x.Id == id && x.Estado == true).ToList()
                          select new
                          {
                              titulo = inf.Titulo
                          }).ToList();


            var objInfPlantilla = dbc.InformePlantillas.Where(x => x.Id == id && x.Estado == true).ToList();


            if (objInfPlantilla.Count() > 0)
            {
                estado = false;

            }
            else
            {

                estado = true;
            }


            var results = dbc.DesactivarEstadoPlantilla(id, estado);
            //var query = dbc.InformePlantillaListar().ToList();
            //var result = dbe.ListarTablaDocumentos().ToList();

            return Json(new { data = results }, JsonRequestBehavior.AllowGet);

        }

    }
}
