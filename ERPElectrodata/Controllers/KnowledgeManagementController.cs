using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Globalization;
using System.Configuration;
using ERPElectrodata.Objects;
using WebMatrix.WebData;
using System.Web.Security;
using System.Threading;
using ERPElectrodata.Object;
using ERPElectrodata.Object.Ticket;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Net;
using Electrodata.ERPElectrodata.Domain.Services.TemaService;
using Electrodata.ERPElectrodata.Domain.Entities;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using Electrodata.ERPElectrodata.Infra.Reposotories.TemaRepositorie;
using Electrodata.ERPElectrodata.Domain.Services.LeccionAprendidaService;
using Electrodata.ERPElectrodata.Infra.Reposotories.LeccionAprendidaRepositorie;

namespace ERPElectrodata.Controllers
{
    public class KnowledgeManagementController : Controller
    {
        #region "Variables Globales"
        public CoreEntities db = new CoreEntities();
        public TemaService _srvTemaService = new TemaService(new TemaRepositorie());
        public LeccionAprendidaService _srvLeccionAprendidaService = new LeccionAprendidaService(new LeccionAprendidaRepositorie());
        int result = (int)Tipo_Peracion.OPERATION_ERROR;
        string FechaCreacionInicio = string.Empty;
        string FechaCreacionFin = string.Empty;
        string FechaBajaInicio = string.Empty;
        string FechaBajaFin = string.Empty;
        string FechaBaja = string.Empty;
        #endregion

        #region "Lecciones Aprendidas"
        [Authorize]
        public ActionResult NewLessonLearned()
        {
            string vista = string.Empty;
            if (Convert.ToInt32(Session["APROBADOR_OPERACIONES"]) == 1)
                vista = "NewLessonLearnedApprover";
            else
                vista = "NewLessonLearned";
            ViewBag.TypeOperation = Convert.ToInt32(Tipo_Peracion.OPERATION_REGISTER);
            ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            return View(vista);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveLessonLearned(Electrodata.ERPElectrodata.Domain.Entities.ComLeccionAprendida objLeccionAprendida, int TypeOperation, string KEY_ATTA)
        {
            //int tipoOperacion = Convert.ToInt32(Request.Params["idTypeOperacion"].ToString());
            int tipoOperacion = 0;
            string KeyAtta = string.Empty;

            if (!String.IsNullOrEmpty(KEY_ATTA)) KeyAtta = KEY_ATTA;
            if (!Int32.TryParse(Convert.ToString(TypeOperation), out tipoOperacion)) tipoOperacion = TypeOperation;
            try
            {
                result = _srvLeccionAprendidaService.OperationLeccionAprendidaBusiness(objLeccionAprendida, tipoOperacion, KeyAtta, Convert.ToInt32(Session["ID_ACCO"]), Session["UserName"].ToString(), Convert.ToInt32(Session["APROBADOR_OPERACIONES"]), Convert.ToInt32(Session["UserId"]));
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult TrayLessonsLearned()
        {

            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            return View();
        }

        [Authorize]
        public JsonResult BuscarLeccionesAprendidas(LessonLearned objLessonLearned, string palabraClave)
        {

            //int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            //int take = Convert.ToInt32(Request.Params["take"].ToString());

            if (objLessonLearned.FechaCreacionInicio.ToString() == "1/1/0001 12:00:00 AM") objLessonLearned.fechaI_ = "";
            else objLessonLearned.fechaI_ = DateTime.Parse(objLessonLearned.FechaCreacionInicio.ToString()).ToString("yyyyMMdd");

            if (objLessonLearned.FechaCreacionFin.ToString() == "1/1/0001 12:00:00 AM") objLessonLearned.fechaF_ = "";
            else objLessonLearned.fechaF_ = DateTime.Parse(objLessonLearned.FechaCreacionFin.ToString()).ToString("yyyyMMdd");

            

            var listaLeccionesAprendidas = _srvLeccionAprendidaService.SearchInitialLessonLearned(
                                               objLessonLearned.Nivel1, objLessonLearned.Nvel2, objLessonLearned.Nivel3, objLessonLearned.Nivel4, objLessonLearned.IdTema, objLessonLearned.fechaF_,
                                               objLessonLearned.fechaI_, objLessonLearned.NroRevisiones, objLessonLearned.PalabraClave, objLessonLearned.EstadoAprobacion, Convert.ToInt32(objLessonLearned.Puntuacion), objLessonLearned.TipoTicket).Select(x => new
                                               {
                                                   IdLeccioNAprendida = x.IdLeccioNAprendida,
                                                   Usuario = x.Usuario,
                                                   fechaCreacion = String.Format("{0:d}", x.fechaCreacion) + " " + String.Format("{0:HH:mm}", x.fechaCreacion),
                                                   fechaActualizacion = String.Format("{0:d}", x.fechaActualizacion) + " " + String.Format("{0:HH:mm}", x.fechaActualizacion),
                                                   Aprobador = x.Aprobador,
                                                   Ruta = x.Ruta,
                                                   Titulo = x.Titulo,
                                                   nombreNivel4 = x.nombreNivel4,
                                                   nombreTipoTicket = x.nombreTipoTicket,
                                                   NroRevisiones = x.NroRevisiones,
                                                   EstadoAprobacion = x.EstadoAprobacion,
                                                   DescripcionEstadoAprobacion=x.DescripcionEstadoAprobacion,
                                                   //== "P" ? "Pendiente" : x.EstadoAprobacion == "A"?"Aprobado":"Desaprobado",
                                                   ColorEstado = x.ColorEstado,
                                                   DescripcionProblema = StripHtml(x.DescripcionProblema),
                                                   Porque2 = StripHtml(x.Porque2),
                                                   Porque3 = StripHtml(x.Porque3),
                                                   Porque4 = StripHtml(x.Porque4),
                                                   Porque5 = StripHtml(x.Porque5),
                                                   
                                               });
            var Count = 0;
            Count = listaLeccionesAprendidas.Count();
            //listaLeccionesAprendidas = listaLeccionesAprendidas.Skip(skip).Take(take);
            
            

            return Json(listaLeccionesAprendidas, JsonRequestBehavior.AllowGet);
            //return Json(new { Data = listaLeccionesAprendidas, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BuscarLeccionesAprendidas2(LessonLearned objLessonLearned, string palabraClave)
        {

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            if (objLessonLearned.FechaCreacionInicio.ToString() == "1/1/0001 12:00:00 AM") objLessonLearned.fechaI_ = "";
            else objLessonLearned.fechaI_ = DateTime.Parse(objLessonLearned.FechaCreacionInicio.ToString()).ToString("yyyyMMdd");

            if (objLessonLearned.FechaCreacionFin.ToString() == "1/1/0001 12:00:00 AM") objLessonLearned.fechaF_ = "";
            else objLessonLearned.fechaF_ = DateTime.Parse(objLessonLearned.FechaCreacionFin.ToString()).ToString("yyyyMMdd");



            var listaLeccionesAprendidas = _srvLeccionAprendidaService.SearchInitialLessonLearned(
                                               objLessonLearned.Nivel1, objLessonLearned.Nvel2, objLessonLearned.Nivel3, objLessonLearned.Nivel4, objLessonLearned.IdTema, objLessonLearned.fechaF_,
                                               objLessonLearned.fechaI_, objLessonLearned.NroRevisiones, objLessonLearned.PalabraClave, objLessonLearned.EstadoAprobacion, Convert.ToInt32(objLessonLearned.Puntuacion), objLessonLearned.TipoTicket).Select(x => new
                                               {
                                                   IdLeccioNAprendida = x.IdLeccioNAprendida,
                                                   Usuario = x.Usuario,
                                                   fechaCreacion = String.Format("{0:d}", x.fechaCreacion) + " " + String.Format("{0:HH:mm}", x.fechaCreacion),
                                                   fechaActualizacion = String.Format("{0:d}", x.fechaActualizacion) + " " + String.Format("{0:HH:mm}", x.fechaActualizacion),
                                                   Aprobador = x.Aprobador,
                                                   Ruta = x.Ruta,
                                                   Titulo = x.Titulo,
                                                   nombreNivel4 = x.nombreNivel4,
                                                   nombreTipoTicket = x.nombreTipoTicket,
                                                   NroRevisiones = x.NroRevisiones,
                                                   EstadoAprobacion = x.EstadoAprobacion,
                                                   DescripcionEstadoAprobacion = x.DescripcionEstadoAprobacion,
                                                   //== "P" ? "Pendiente" : x.EstadoAprobacion == "A"?"Aprobado":"Desaprobado",
                                                   ColorEstado = x.ColorEstado,
                                                   DescripcionProblema = StripHtml(x.DescripcionProblema),
                                                   Porque2 = StripHtml(x.Porque2),
                                                   Porque3 = StripHtml(x.Porque3),
                                                   Porque4 = StripHtml(x.Porque4),
                                                   Porque5 = StripHtml(x.Porque5),

                                               });
            var Count = 0;
            Count = listaLeccionesAprendidas.Count();
            listaLeccionesAprendidas = listaLeccionesAprendidas.Skip(skip).Take(take);



            //return Json(listaLeccionesAprendidas, JsonRequestBehavior.AllowGet);
            return Json(new { Data = listaLeccionesAprendidas, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        [ActionName("LoadLessonsLearned")]
        public ActionResult LoadLessonsLearned(LessonLearned objLessonLearned, string palabraClave)
        {
            if (objLessonLearned.FechaCreacionInicio.ToString() == "1/1/0001 12:00:00 AM")
                objLessonLearned.fechaI_ = "";
            else
                objLessonLearned.fechaI_ = DateTime.Parse(objLessonLearned.FechaCreacionInicio.ToString()).ToString("yyyyMMdd");

            if (objLessonLearned.FechaCreacionFin.ToString() == "1/1/0001 12:00:00 AM")
                objLessonLearned.fechaF_ = "";
            else
                objLessonLearned.fechaF_ = DateTime.Parse(objLessonLearned.FechaCreacionFin.ToString()).ToString("yyyyMMdd");

            var listaLeccionesAprendidas = _srvLeccionAprendidaService.SearchInitialLessonLearned(
                                            objLessonLearned.Nivel1, objLessonLearned.Nvel2, objLessonLearned.Nivel3, objLessonLearned.Nivel4, objLessonLearned.IdTema, objLessonLearned.fechaF_,
                                            objLessonLearned.fechaI_, objLessonLearned.NroRevisiones, objLessonLearned.PalabraClave, objLessonLearned.EstadoAprobacion, Convert.ToInt32(objLessonLearned.Puntuacion), objLessonLearned.TipoTicket);

            ViewBag.data = listaLeccionesAprendidas;
            return Json(listaLeccionesAprendidas, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ListTemaLessonLearned(int ID_ACCO, int ID_CATE)
        {
            ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var ListTemaporLessonLearned = _srvTemaService.ListTemaLessonLearned(ID_ACCO, ID_CATE);
            return Json(new { Data = ListTemaporLessonLearned, Count = ListTemaporLessonLearned.Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLeccionesAprendidasUsuario()
        {
            var result = db.ComLeccionesAprendidasUsuario().ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Lección Aprendida Ticket Problema"
        public ActionResult GenerarLeccionTicketProblema(int id)
        {
            ViewBag.IdTicket = id;
            ViewBag.TypeOperation = Convert.ToInt32(Tipo_Peracion.OPERATION_REGISTER);
            ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            //  ViewBag.ID_COMP = id;
            return View();

        }


        public ActionResult GuardarLeccionTicketProblema(Electrodata.ERPElectrodata.Domain.Entities.ComLeccionAprendida objLeccionAprendida, int TypeOperation, string KEY_ATTA)
        {
            int tipoOperacion = 0;
            string KeyAtta = string.Empty;

            if (!String.IsNullOrEmpty(KEY_ATTA)) KeyAtta = KEY_ATTA;
            if (!Int32.TryParse(Convert.ToString(TypeOperation), out tipoOperacion)) tipoOperacion = TypeOperation;
            try
            {
                result = _srvLeccionAprendidaService.OperationLeccionAprendidaBusiness(objLeccionAprendida, tipoOperacion, KeyAtta, Convert.ToInt32(Session["ID_ACCO"]), Session["UserName"].ToString(), Convert.ToInt32(Session["APROBADOR_OPERACIONES"]), Convert.ToInt32(Session["UserId"]));
            }
            catch (Exception ex)
            {
                throw;
            }

            /*Actualizamos el estado del ticket Problema*/
            int IdTicket = Convert.ToInt32( objLeccionAprendida.ID_TICKET);
            var objTicket = db.TICKETs.Where(x => x.ID_TICK == IdTicket).FirstOrDefault();
            objTicket.ID_STAT_END = 4;
            objTicket.SUM_TICK = "El ticket fue cerrado satisfactoriamente";
            db.Entry(objTicket).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            db.Entry(objTicket).State = System.Data.Entity.EntityState.Detached;
            /*Fin*/
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region "Temas"

        [Authorize]
        public ActionResult TrayThemes()
        {
            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var cInc = new Electrodata.ERPElectrodata.Domain.Entities.ComTema();
            cInc.DateEnd = DateTime.Now;
            ViewBag.DATE = String.Format("{0:g}", cInc.DateEnd);
            //ViewBag.token = Request.Cookies["__RequestVerificationToken"].Value;
            return View();
        }

        [HttpGet]
        [Authorize]
        [ActionName("_NewTheme")]
        public ActionResult _NewTheme()
        {

            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var cInc = new Electrodata.ERPElectrodata.Domain.Entities.ComTema();
            cInc.DateEnd = DateTime.Now;
            ViewBag.DATE = String.Format("{0:g}", cInc.DateEnd);
            return PartialView();
        }

        [HttpGet]
        [Authorize]
        [ActionName("_ModifyTheme")]
        public ActionResult _ModifyTheme(int iTheme, int IdCuentaCatTema)
        {

            var ObjectTheme = LoadObjectTheme(iTheme, IdCuentaCatTema);
            ViewBag.IdCuentaCatTema = IdCuentaCatTema;
            ViewBag.DATE = ObjectTheme.DateEnd;
            var ticket = (from t in db.ComCuentaCategoryTemas.Where(x => x.IdCuentaCategoryTema == IdCuentaCatTema).ToList()
                          join c4 in db.CATEGORies on t.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in db.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in db.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE

                          select new
                          {
                              ID_CATE_N4 = c4.ID_CATE,
                              ID_CATE_N3 = c3.ID_CATE,
                              ID_CATE_N2 = c2.ID_CATE,
                              ID_CATE_N1 = c1.ID_CATE,
                              CATE_4 = c4.NAM_CATE.ToLower(),
                              CATE_3 = c3.NAM_CATE.ToLower(),
                              CATE_2 = c2.NAM_CATE.ToLower(),
                              CATE_1 = c1.NAM_CATE.ToLower()
                          }).Single();

            ViewBag.CATE_4 = ticket.ID_CATE_N4;
            ViewBag.CATE_3 = ticket.ID_CATE_N3;
            ViewBag.CATE_2 = ticket.ID_CATE_N2;
            ViewBag.CATE_1 = ticket.ID_CATE_N1;
            return PartialView(ObjectTheme);
        }


        [HttpGet]
        [Authorize]
        [ActionName("_NewThemeTicket")]
        public ActionResult _NewThemeTicket(int idCategoria)
        {
            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var cInc = new Electrodata.ERPElectrodata.Domain.Entities.ComTema();
            cInc.DateEnd = DateTime.Now;
            ViewBag.DATE = String.Format("{0:g}", cInc.DateEnd);
            ViewBag.idcategoria = idCategoria;
            return PartialView();
        }

        [HttpGet]
        [Authorize]
        [ActionName("_ViewTopic")]
        public ActionResult _ViewTopic(int iTheme, int IdCuentaCatTema)
        {
            var ObjectTheme = LoadObjectTheme(iTheme, IdCuentaCatTema);
            ViewBag.IdCuentaCatTema = IdCuentaCatTema;
            var ticket = (from t in db.ComCuentaCategoryTemas.Where(x => x.IdCuentaCategoryTema == IdCuentaCatTema).ToList()
                          join c4 in db.CATEGORies on t.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in db.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in db.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE

                          select new
                          {
                              ID_CATE_N4 = c4.ID_CATE,
                              ID_CATE_N3 = c3.ID_CATE,
                              ID_CATE_N2 = c2.ID_CATE,
                              ID_CATE_N1 = c1.ID_CATE,
                              CATE_4 = c4.NAM_CATE,
                              CATE_3 = c3.NAM_CATE,
                              CATE_2 = c2.NAM_CATE,
                              CATE_1 = c1.NAM_CATE
                          }).Single();
            
            //objModelTheme.idCategoria = Convert.ToInt32(ticket.CATE_4);
            ViewBag.CATE_4 = ticket.CATE_4;
            ViewBag.CATE_3 = ticket.CATE_3;
            ViewBag.CATE_2 = ticket.CATE_2;
            ViewBag.CATE_1 = ticket.CATE_1;
            return PartialView(ObjectTheme);
        }

        private ThemeModel LoadObjectTheme(int iTheme, int IdCuentaCatTema)
        {
            ThemeModel objModelTheme = new ThemeModel();

            var objNegocyTheme = _srvTemaService.ObjectTheme(iTheme);
            objModelTheme.IdTema = objNegocyTheme.IdTema;
            objModelTheme.Nomtema = objNegocyTheme.Nomtema;
            objModelTheme.idCategoria = Convert.ToInt32(_srvTemaService.ThemeCategoryAccountList(IdCuentaCatTema).ID_CATE);

            
            //objModelTheme.idCategoria = Convert.ToInt32(_srvTemaService.ThemeCategoryAccountList(IdCuentaCatTema).ID_CATE);
            objModelTheme.DateEnd = Convert.ToDateTime(objNegocyTheme.DateEnd);
            return objModelTheme;
        }

        [HttpGet]
        [Authorize]
        [ActionName("SearchInitialThemes")]
        public ActionResult SearchInitialThemes(ThemeModel objThemeModel)
        {
            var listThemes = ListTopics(objThemeModel);
            return Json(listThemes, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        [ActionName("SearchThemesByParametres")]
        public ActionResult SearchThemesByParametres(ThemeModel objThemeModel)
        {
            var listThemes = ListTopics(objThemeModel);
            return Json(listThemes, JsonRequestBehavior.AllowGet);
        }

        private List<ComFindTopics_Result> ListTopics(ThemeModel objThemeModel)
        {
            FechaCreacionInicio = objThemeModel.FechaCreacionInicio.ToString() == "1/1/0001 12:00:00 AM" ? string.Empty : DateTime.Parse(objThemeModel.FechaCreacionInicio.ToString()).ToString("yyyy-MM-dd");
            FechaCreacionFin = objThemeModel.FechaCreacionFin.ToString() == "1/1/0001 12:00:00 AM" ? string.Empty : DateTime.Parse(objThemeModel.FechaCreacionFin.ToString()).ToString("yyyy-MM-dd");
            FechaBajaInicio = objThemeModel.FechaBajaInicio.ToString() == "1/1/0001 12:00:00 AM" ? string.Empty : DateTime.Parse(objThemeModel.FechaBajaInicio.ToString()).ToString("yyyy-MM-dd");
            FechaBajaFin = objThemeModel.FechaBajaFin.ToString() == "1/1/0001 12:00:00 AM" ? string.Empty : DateTime.Parse(objThemeModel.FechaBajaFin.ToString()).ToString("yyyy-MM-dd");

            var listThemes = _srvTemaService.SearchInitialThemes(objThemeModel.Nomtema == null ? string.Empty : objThemeModel.Nomtema, Convert.ToInt32(Session["ID_ACCO"].ToString()), objThemeModel.idCategoria, FechaCreacionInicio, FechaCreacionFin,
                                                                 objThemeModel.Vigtema, FechaBajaInicio, FechaBajaFin);
            return listThemes;
        }

        [HttpPost]
        [Authorize]
        [ActionName("SaveTheme")]
        public ActionResult SaveTheme(ThemeModel objThemeModel, int TypeOperation)
        {
            var listaTemas = (dynamic)null;
            FechaBaja = objThemeModel.DateEnd.ToString() == "1/1/0001 12:00:00 AM" ? string.Empty : DateTime.Parse(objThemeModel.DateEnd.ToString()).ToString("dd-MM-yyyy");
            result = _srvTemaService.OperationThemeBusiness(objThemeModel.IdTema, Convert.ToInt32(Session["ID_ACCO"].ToString()), objThemeModel.IdCuentaCatTema, objThemeModel.idCategoria, objThemeModel.Nomtema,
                                                            true, Session["UserName"].ToString(), DateTime.Parse(DateTime.Now.ToString()).ToString("dd-MM-yyyy"),
                                                            FechaBaja, Convert.ToInt32(Session["ID_ACCO"].ToString()),
                                                            DateTime.Parse(DateTime.Now.ToString()).ToString("dd-MM-yyyy"), Session["UserName"].ToString(),
                                                            TypeOperation);

            if (result != (int)Tipo_Peracion.OPERATION_ERROR)
            {
                objThemeModel.Nomtema = string.Empty;
                objThemeModel.IdTema = result;
                objThemeModel.idCategoria = 0;
                listaTemas = ListTopics(objThemeModel);
            }
            return Json(new { listaTemas = listaTemas != null ? listaTemas : "", idTema = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  "Aprobador"

        public JsonResult CombosEdicion(int id = 0)
        {
            var result = _srvLeccionAprendidaService.FirstLessonLearned(id);
            var listArchivos = _srvLeccionAprendidaService.listaAdjuntos(id); //Non se usa , retirar
            return Json(new { obj = result, archivos = listArchivos }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LessonLearnedActivity(int id = 0)
        {
            var listActividades = _srvLeccionAprendidaService.listaActividades(id, Convert.ToInt32(Session["ID_ACCO"]));
            return Json(listActividades, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LessonLearnedArchivos(int id = 0)
        {
            var listArchivos = _srvLeccionAprendidaService.listaAdjuntos(id);
            return Json(new { Data = listArchivos, Count = listArchivos.Count() }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult TrayApprover()
        {
            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            return View();
        }

        [Authorize]
        public ActionResult EditLessonLearned(int id = 0)
        {
            string vista = string.Empty;
            if (Convert.ToInt32(Session["APROBADOR_OPERACIONES"]) == 1)
                vista = "NewLessonLearnedApprover";
            else
                vista = "NewLessonLearned";

            var obj = _srvLeccionAprendidaService.FirstLessonLearned(id);

            LessonLearned objLessonLearned = new LessonLearned()
            {
                Titulo = obj.Titulo,
                Nivel1 = Convert.ToInt32(obj.Nivel1),
                Nvel2 = Convert.ToInt32(obj.Nvel2),
                Nivel3 = Convert.ToInt32(obj.Nivel3),
                Nivel4 = Convert.ToInt32(obj.Nivel4),
                NombreNivel1 = obj.nombreNivel1,
                NombreNivel2 = obj.nombreNivel2,
                NombreNivel3 = obj.nombreNivel3,
                NombreNivel4 = obj.nombreNivel4,
                IdTema = Convert.ToInt32(obj.IdTema),
                NombreTema = obj.nombreTema,
                DescripcionProblema = StripHtml(obj.DescripcionProblema),
                SolucionAplicada = obj.SolucionAplicada,
                Impactonegocio = StripHtml(obj.Impactonegocio),
                SolucionTemporal = StripHtml(obj.SolucionTemporal),
                SolucionDefinitiva = StripHtml(obj.SolucionDefinitiva),
                Porque2 = StripHtml(obj.Porque2),
                Porque3 = StripHtml(obj.Porque3),
                Porque4 = StripHtml(obj.Porque4),
                Porque5 = StripHtml(obj.Porque5)
            };
            //
            //ViewBag.Nivel1 = obj.nombreNivel1;
            //ViewBag.Nvel2 = obj.nombreNivel2;
            ViewBag.valorPerfil = Convert.ToInt32(Session["APROBADOR_OPERACIONES"]);
            ViewBag.idLeccionAprendida = id;//3 significa que la llamada es de la bandeja del aprobador
            ViewBag.CombosEdicion = 3;//3 significa que la llamada es de la bandeja del aprobador
            ViewBag.TypeOperation = 2;//2 es update
            ViewBag.UploadFile = obj.KEY_ATTA.ToString();// "M1CT" + Convert.ToString(DateTime.Now.Ticks);
            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            return View(vista, objLessonLearned);
        }
        private string StripHtml(string source)
        {
            string output;

            //get rid of HTML tags
            output = Regex.Replace(source, "<[^>]*>", string.Empty);

            //get rid of multiple blank lines
            //output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);
            output = output.Replace("&aacute;", "á");
            output = output.Replace("&eacute;", "é");
            output = output.Replace("&iacute;", "í");
            output = output.Replace("&oacute;", "ó");
            output = output.Replace("&uacute;", "ú");
            output = output.Replace("&Aacute;", "Á");
            output = output.Replace("&Eacute;", "É");
            output = output.Replace("&Iacute;", "Í");
            output = output.Replace("&Oacute;", "Ó");
            output = output.Replace("&Uacute;", "Ú");
            output = output.Replace("&ntilde;", "ñ");
            output = output.Replace("&Ntilde;", "Ñ");
            output = output.Replace("&nbsp;", " ");
            output = output.Replace("&lt;", "<");
            output = output.Replace("&gt;", ">");
            output = output.Replace("&amp;", "&");

            return output;
        }
        [HttpGet]
        [Authorize]
        [ActionName("_DetalleAprobador")]
        public ActionResult _DetalleAprobador(int id)
        {
            var detalleLeccionAprendida = _srvLeccionAprendidaService.DetalleModalLeccionAprendida(id);
            ViewBag.Aprobacion = detalleLeccionAprendida.PorcentajeAprobacion == null ? 0 : detalleLeccionAprendida.PorcentajeAprobacion;
            ViewBag.UltimaPuntuacion = detalleLeccionAprendida.PuntuacionActual == null ? "0" : detalleLeccionAprendida.PuntuacionActual; ;
            ViewBag.Revisiones = detalleLeccionAprendida.Revisiones == null ? 0 : detalleLeccionAprendida.Revisiones; ;
            ViewBag.Categoria = detalleLeccionAprendida.Categoria == null ? "0" : detalleLeccionAprendida.Categoria; ;
            return PartialView();
        }

        public JsonResult RelojAprobador(int id)
        {
            var detalleLeccionAprendida = _srvLeccionAprendidaService.DetalleModalLeccionAprendida(id);

            return Json(detalleLeccionAprendida, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _AdjuntarArchivo(string UploadFile)
        {
            ViewBag.UploadFile = UploadFile;
            return View();
        }
        #endregion

        #region "Lecciones aprendidas en Tickets"idTicket
        [Authorize]
        public JsonResult ListLeccionesAprendidasTickets(int idTicket)
        {
            var ListLeccionTickets = _srvLeccionAprendidaService.ListLeccionesAprendidasTickets(idTicket, 1);
            var VinculacionTickets = _srvLeccionAprendidaService.ListLeccionesAprendidasTickets(idTicket, 0);
            return Json(new { lecciones = ListLeccionTickets, tickets = VinculacionTickets }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Reportes"

        [Authorize]
        public ActionResult ReportLessonLearned()
        {
            return View();
        }


        public ActionResult ViewLessonsLearnedByUserReport()
        {
            return View();
        }

        public ActionResult ViewLessonsByCategoryReport()
        {
            ViewBag.idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            return View();
        }

        public ActionResult ViewLessonsMensual()
        {
            return View();
        }

        public ActionResult ListarLeccionesAprendidasMensuales()
        {
            int ANIO = 0, MES = 0;

            ANIO = Convert.ToInt32(Request.Params["ANIO"].ToString());
            MES = Convert.ToInt32(Request.Params["MONTH"].ToString());

            var result = db.ComReporteMensualLeccionesAprendidas(ANIO, MES).ToList();

            return Json(new { line = result}, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
