using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using System.Net;
using ERPElectrodata.Objects;
using System.IO;
using System.Data.Entity.SqlServer;
using System.Text;
using System.Web.Script.Serialization;
using ERPElectrodata.AppCode;

namespace ERPElectrodata.Controllers
{
    public class EvaluacionPersonalController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();
        //
        // GET: /EvaluacionPersonal/
        [Authorize]
        public ActionResult Index()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            //bool TieneAcceso = Convert.ToBoolean(dbe.AccesoEvaluacionPersonal(IdPersEnti).FirstOrDefault());

            //if (TieneAcceso)
            //{
                var Responsable = (from pe in dbe.PERSON_ENTITY
                                   join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                   where pe.ID_PERS_ENTI == IdPersEnti
                                   select new
                                   {
                                       NOMBRE = (ce.FIR_NAME == null ? "" : ce.FIR_NAME) + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME),
                                       FOTO = (pe.ID_FOTO == null ? 0 : pe.ID_FOTO)
                                   }).FirstOrDefault();

                int CountPeriodColab = dbe.ListaPeriodosColaboradores(IdPersEnti).Count();
                ViewBag.TieneColaboradores = (CountPeriodColab > 0 ? 1 : 0);

                int ExisteCalificacion = Convert.ToInt32(dbe.ExisteCalificacion(IdPersEnti).Single().Value);

                ViewBag.PendienteConformidad = (ExisteCalificacion > 0 ? 1 : 0);

                ViewBag.FOTO = Responsable.FOTO.ToString() + ".jpg";
                ViewBag.NOMBRE = Responsable.NOMBRE;
                return View();
            //}
            //else
            //{
            //    return RedirectToAction("Error", "EvaluacionPersonal");
            //}

            
        }

        public ActionResult Evaluacion()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string AreaMejora = "", Comentario = "";
            ViewBag.IdPersEnti = IdPersEnti;

            var query = dbe.EvaluadoPlanMejoras.Where(x => x.IdPersEnti == IdPersEnti && x.Estado == true).SingleOrDefault();
            if (query != null)
            {
                AreaMejora = query.AreaMejora == null ? "" : query.AreaMejora.ToString();
                Comentario = query.Comentario == null ? "" : query.Comentario.ToString();
            }
            ViewBag.AreaMejora = AreaMejora;
            ViewBag.Comentario = Comentario;

            return View();
        }

        public ActionResult ListarEvaluacion(int id)
        {
            //int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = dbe.EvaListarObjetivos(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult GrabarEvaluacion(int id)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdEvaluado = Convert.ToInt32(Request.Params["IdEvaluado"]);

            if (IdPersEnti != IdEvaluado) { IdPersEnti = IdEvaluado; }

            string Id = Request.Params["IdEvaluacion"].ToString();
            int IdEva, estado = 0;

            //Validando
            foreach (string strId in Id.Split(','))
            {
                if (!String.IsNullOrEmpty(strId))
                {
                    IdEva = Convert.ToInt32(strId);
                    var EvalCalifica = dbe.EvaluadoCalificacions.Where(x => x.Id == IdEva).SingleOrDefault();
                    if (EvalCalifica.Estado == 0 && id == 1) //Validando plan de mejora
                    {
                        var evaPlan = dbe.EvaluadoPlanMejoras.Where(x => x.IdPersEnti == IdPersEnti && x.Estado == true).SingleOrDefault();
                        if (evaPlan != null)
                        {
                            if (evaPlan.AreaMejora == "")
                            { // || evaPlan.Comentario == ""){
                                return Json("ERROR_PLAN", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json("ERROR_PLAN", JsonRequestBehavior.AllowGet);
                        }
                    }
                    //if (EvalCalifica.Estado == 1 && id == 1)
                    //{
                    //    return Json("ERROR_OBS_JEFE", JsonRequestBehavior.AllowGet);
                    //}
                }
            }

            //Grabar - Finalizar
            foreach (string strIdEva in Id.Split(','))
            {
                if (!String.IsNullOrEmpty(strIdEva))
                {
                    IdEva = Convert.ToInt32(strIdEva);
                    var EvalCalifica = dbe.EvaluadoCalificacions.Where(x => x.Id == IdEva).SingleOrDefault();
                    estado = Convert.ToInt32(EvalCalifica.Estado);
                    if (EvalCalifica.Estado == 0) //Autoevalua
                    {
                        EvalCalifica.IdResultadoEvaluado = Convert.ToInt32(Request.Params["IdAutoEva" + strIdEva]);
                        //EvalCalifica.ObsResultadoEvaluado = Request.Params["txtAutoEva" + strIdEva].ToString();
                        EvalCalifica.Estado = EvalCalifica.Estado + id;
                    }
                    else if (EvalCalifica.Estado == 1) //Evalua Jefe
                    {
                        EvalCalifica.IdResultadoJefe = Convert.ToInt32(Request.Params["IdJefeEva" + strIdEva]);
                        //EvalCalifica.ObsResultadoJefe = Request.Params["txtJefeEva" + strIdEva].ToString();
                        EvalCalifica.Estado = EvalCalifica.Estado + id;
                    }

                    if (id == 2) { EvalCalifica.Estado = EvalCalifica.Estado + 1; } //Confirma RRHH
                    if (id == 3) { EvalCalifica.Estado = EvalCalifica.Estado - 1; } //Rechaza RRHH

                    dbe.Entry(EvalCalifica).State = EntityState.Modified;
                    dbe.SaveChanges();
                }
            }

            //Enviar Correo
            try
            {
                if (id > 0)
                {
                    SendMail mail = new SendMail();
                    mail.EnviarEvaluacionPersonal(IdPersEnti, estado, id);
                }
            }
            catch (Exception ex)
            {

            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult GrabarEvaComentario()
        {
            string id = Request.Params["id"].ToString();
            string texto = Request.Params["texto"].ToString();
            string[] array = new string[2];

            int IdEva = Convert.ToInt32(id.Replace("txtAutoEva", "").Replace("txtJefeEva", "").ToString());

            var EvalCalifica = dbe.EvaluadoCalificacions.Where(x => x.Id == IdEva).SingleOrDefault();

            if (id.Contains("txtAutoEva"))
            {
                EvalCalifica.ObsResultadoEvaluado = texto;
            }
            if (id.Contains("txtJefeEva"))
            {
                EvalCalifica.ObsResultadoJefe = texto;
            }
            dbe.Entry(EvalCalifica).State = EntityState.Modified;
            dbe.SaveChanges();

            return Json(new { Data = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EvaluarPersonal()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string NombreUsuario = Session["NAM_PERS"].ToString();
            List<EvaluarPersonal_Result> query = dbe.EvaluarPersonal(IdPersEnti).ToList();

            query.Add(new EvaluarPersonal_Result { IdEvaluador = IdPersEnti, IdEvaluado = IdPersEnti, Nombre = NombreUsuario.ToUpper(), Cargo = "" });

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult GrabarPlanMejora(EvaluadoPlanMejora objPlan)
        {
            string AreaMejora = objPlan.AreaMejora;
            string Comentario = objPlan.Comentario;
            int IdPersEnti = (int)objPlan.IdPersEnti;


            try
            {
                //AreaMejora = Request.Params["txtAreaMejora"].ToString();
                //Comentario = Request.Params["txtComentario"].ToString();
                dbe.EvaPlanMejora(IdPersEnti, AreaMejora, Comentario);

                //return Json(new { Data = "OK" }, JsonRequestBehavior.AllowGet);
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','');}window.onload=init;</script>");
            }
            catch (Exception ex)
            {
                //return Json(new { Data = "ERROR" }, JsonRequestBehavior.AllowGet);
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
        }

        public ActionResult VistaObjetivos()
        {
            return View();
        }

        public ActionResult ListarObjetivos(int IdCargo, string PalabraClave)
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            List<EvaListarObjetivosRRHH_Result> query = dbe.EvaListarObjetivosRRHH(IdCargo, PalabraClave, 0).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCargo()
        {
            var query = dbe.EvaListarCargo();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Editar(int id)
        {
            List<EvaListarObjetivosRRHH_Result> query = dbe.EvaListarObjetivosRRHH(0, "", id).ToList();

            ViewBag.Id = query.Single().Id;
            ViewBag.Objetivo = query.Single().Nombre;
            ViewBag.Descripcion = query.Single().Descripcion;
            ViewBag.Indicador = query.Single().Indicador;
            ViewBag.Peso = query.Single().Peso;
            ViewBag.Cargo = query.Single().IdCargo;

            return View();
        }

        public ActionResult GrabarEditar(int IdObjetivo, string EditNombre, string EditDescripcion, string EditIndicador, int EditPeso, int EditIdCargo)
        {
            string resp = "OK";

            dbe.EvaEditarObjetivo(IdObjetivo, EditNombre, EditDescripcion, EditIndicador, EditPeso, EditIdCargo);

            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VistaReporte()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.Periodo = "";
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
            }
            return View();
        }

        public ActionResult VistaEstadoEvaluados()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.Periodo = "";
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
            }
            return View();
        }

        public ActionResult ReporteComparativo(int IdCargo, int IdPersEnti)
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            List<EvaReporte_Result> query = dbe.EvaReporte(IdCargo, IdPersEnti).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCalificacionFinal(int id = 0)
        {
            //int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = dbe.EvaCalificacionFinal(id).SingleOrDefault();

            return Json(query.Nota, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerPlanMejora(int id = 0)
        {
            var query = dbe.EvaluadoPlanMejoras.Where(x => x.IdPersEnti == id).SingleOrDefault();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdjuntarArchivo(IEnumerable<HttpPostedFileBase> files)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            //int IdTipoDocumento = Convert.ToInt32(Request.Params["IdTipoDocumento"].ToString());
            //Guardando archivo adjunto
            foreach (var file in files)
            {
                if (file != null)
                {
                    try
                    {
                        var Docs = new EvaDocumento();
                        Docs.NombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);
                        Docs.Extension = Path.GetExtension(file.FileName);
                        Docs.IdPersEnti = IdPersEnti;
                        Docs.FechaCreacion = DateTime.Now;
                        //Docs.IdTipoDocumento = IdTipoDocumento;
                        Docs.IdEvaluacion = 1;
                        Docs.UserIdCreacion = Convert.ToInt32(Session["UserId"]);

                        dbe.EvaDocumentoes.Add(Docs);
                        dbe.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/Talent/EvaluacionPersonal/" + Docs.NombreArchivo + "_" + Convert.ToString(Docs.Id) + Docs.Extension));

                    }
                    catch
                    {
                        return Content("alert('No se pudo cargar el archivo')");
                    }
                }
            }

            return Content("");
        }

        public ActionResult ObtenerArchivos(int id = 0)
        {
            var query = (from doc in dbe.EvaDocumentoes.Where(x => x.IdPersEnti == id && x.IdEvaluacion != 0)
                             //join ta in dbe.TYPE_DOCUMENT on doc.IdTipoDocumento equals ta.ID_TYPE_DOCU
                         select new
                         {
                             ID = doc.Id,
                             LINK = doc.NombreArchivo + "_" + SqlFunctions.StringConvert((double)doc.Id).Trim() + doc.Extension,
                             ARCHIVO = doc.NombreArchivo + doc.Extension
                             //TIPO = ta.NAM_DOCU
                         });

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PlanMejora(int id = 0)
        {
            //int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string AreaMejora = "", Comentario = "";
            ViewBag.IdPersEnti = id;

            EvaluadoPlanMejora obj = new EvaluadoPlanMejora();
            obj.IdPersEnti = id;

            var query = dbe.EvaluadoPlanMejoras.Where(x => x.IdPersEnti == id && x.Estado == true).SingleOrDefault();
            if (query != null)
            {
                //AreaMejora = query.AreaMejora.ToString();
                //Comentario = query.Comentario.ToString();

                obj.AreaMejora = query.AreaMejora == null ? "" : query.AreaMejora.ToString();
                obj.Comentario = query.Comentario == null ? "" : query.Comentario.ToString();
            }
            //ViewBag.AreaMejora = AreaMejora;
            //ViewBag.Comentario = Comentario;

            return View(obj);
        }

        [Authorize]
        public ActionResult VistaFormato()
        {

            ViewBag.IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            return View();
        }

        [Authorize]
        public ActionResult Indicaciones()
        {
            return View();
        }

        [Authorize]
        public ActionResult VistaConformidad()
        {
            //var 

            ViewBag.IdUsuario = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            //var query2 = dbe.EvaluacionConforme(Convert.ToInt32(Session["ID_PERS_ENTI"]), query.Id).FirstOrDefault();
            //        //.Where(x => x.IdPersEnti == Convert.ToInt32(Session["ID_PERS_ENTI"])).FirstOrDefault();
            //ViewBag.IdEvaluacion = query2.IdEvaluacion;
            //ViewBag.Conforme = query2.ConformeEvaluado;

            return View();
        }

        [Authorize]
        public ActionResult VistaDesempeno()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.Periodo = "";
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
            }
            return View();
        }

        [Authorize]
        public ActionResult VistaDesempenoEvaluado()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.Periodo = "";
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
            }
            return View();
        }

        [Authorize]
        public ActionResult VistaDesempenoJefe()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.Periodo = "";
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
            }
            return View();
        }

        [Authorize]
        public ActionResult ReporteHistorico()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            ViewBag.IdPersEnti = IdPersEnti;

            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.Periodo = "";
            ViewBag.estadoPeriodo = 0;
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
                ViewBag.estadoPeriodo = query.IdEstadoPeriodo;
            }

            return View();
        }

        #region Vistas

        [Authorize]
        public ActionResult EvaluacionIndex()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            bool TieneAcceso = Convert.ToBoolean(dbe.AccesoEvaluacionPersonal(IdPersEnti).FirstOrDefault());

            if (TieneAcceso)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Error", "EvaluacionPersonal");
            }
        }

        [Authorize]
        public ActionResult EvaluacionObjetivos(int id = 0)
        {

           int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            
           int TieneEvaluacion = (dbe.Objetivo.Where(x=> x.IdPeriodo == id && x.id_pers_enti == IdPersEnti).Count() > 0 ? 1: 0);
           int IdObjetivo = (TieneEvaluacion == 1 ? Convert.ToInt32(dbe.Objetivo.Where(x => x.IdPeriodo == id && x.id_pers_enti == IdPersEnti).Single().IdObjetivo) : 0);
           int IdEstadoEvaluacion = dbe.Objetivo.Where(x => x.IdObjetivo == IdObjetivo).Select(x => x.IdEstadoObjetivo).SingleOrDefault() ?? 0;

           ViewBag.IdPersEnti = IdPersEnti;
           ViewBag.TieneEvaluacion = TieneEvaluacion;
           ViewBag.IdEvaluacion = IdObjetivo;
           ViewBag.IdEstadoEvaluacion = IdEstadoEvaluacion;
           ViewBag.IdPeriodo = id;

           return View();

            
        }

        [Authorize]
        public ActionResult EvaluacionCompetencias()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            ViewBag.IdPersEnti = IdPersEnti;
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.estadoPeriodo = 0;
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.estadoPeriodo = query.IdEstadoPeriodo;
            }
            return View();
        }

        [Authorize]
        public ActionResult EvaluacionPlanMejora(int id = 0, int id1 = 0)
        {
            var evaluacion = dbe.EvaObtenerPeriodoEvaluacion(id, id1).SingleOrDefault();
            ViewBag.IdPersEntiPlanMejora = id1;
            ViewBag.IdPersEntiPlanJefe = 0;
            ViewBag.IdEstado = 0;
            EvaPlanMejora obj = new EvaPlanMejora();
            if (evaluacion != null)
            {
                ViewBag.IdPersEntiPlanJefe = evaluacion.IdPersEntiJefe;
                ViewBag.IdEstado = evaluacion.IdEstado;
                int IdEvaluacion = Convert.ToInt32(evaluacion.Id);

                var query = dbe.EvaPlanMejoras.Where(x => x.Estado == true && x.IdEvaluacion == IdEvaluacion).SingleOrDefault();
                if (query != null)
                {
                    obj.AreaMejora = query.AreaMejora == null ? "" : query.AreaMejora.ToString();
                    obj.Comentario = query.Comentario == null ? "" : query.Comentario.ToString();
                }
            }
            return View(obj);
        }

        [Authorize]
        public ActionResult EvaluacionFeedBack(int id = 0)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int TieneFeedBackColaborador = (dbe.FeedBackED.Where(x => x.id_pers_enti == IdPersEnti && x.IdPeriodo == id).Count() > 0 ? 1 : 0);
            int IdEvaluacionFeedBack = (TieneFeedBackColaborador == 0 ? 0 : Convert.ToInt32(dbe.FeedBackED.Where(x => x.id_pers_enti == IdPersEnti && x.IdPeriodo == id).Single().IdFeedBack));
            int IdEstadoFeedback = dbe.FeedBackED.Where(x => x.IdFeedBack == IdEvaluacionFeedBack).Select(x => x.IdEstadoFeedBack).SingleOrDefault() ?? 0;

            ViewBag.IdPersEnti = IdPersEnti;
            ViewBag.IdEvaluacionFeedBack = IdEvaluacionFeedBack;
            ViewBag.IdEstadoFeedBack = IdEstadoFeedback;
            ViewBag.IdPeriodo = id;

            return View();
        }


        //[Authorize]
        //public ActionResult RegistroEvaluacion()
        //{
        //    var query = dbe.EvaListarPeriodo().SingleOrDefault();
        //    ViewBag.IdPeriodo = 0;
        //    ViewBag.estadoPeriodo = 0;
        //    var eval = dbe.EvaListarPrimerNombreEvaluacion().SingleOrDefault();
        //    ViewBag.NombreEvaluacion = eval.NombreEvaluacion;
        //    if (query != null)
        //    {
        //        ViewBag.IdPeriodo = query.Id;
        //        ViewBag.Periodo = query.Periodo;
        //        ViewBag.Descripcion = query.Descripcion;
        //        ViewBag.estadoPeriodo = query.IdEstadoPeriodo;
        //    }

        //    return View();
        //}



        [Authorize]
        public ActionResult RegistroCargo()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = "";
            ViewBag.Periodo = "";
            ViewBag.estadoPeriodo = 0;
            var eval = dbe.EvaListarPrimerNombreEvaluacion().SingleOrDefault();
            ViewBag.IdNombreEvaluacion = eval.id;
            ViewBag.NombreEvaluacion = eval.NombreEvaluacion;
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
                ViewBag.estadoPeriodo = query.IdEstadoPeriodo;
            }
            return View();
        }

        //[Authorize]
        //public ActionResult RegistroPeriodo()
        //{
        //    return View();
        //}

        [Authorize]
        public ActionResult RegistroObjetivos()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.Periodo = "";
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
            }
            return View();
        }

        [Authorize]
        public ActionResult RegistroCompetencias()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.Periodo = "";
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
            }
            return View();
        }

        [Authorize]
        public ActionResult EditarPeriodo(int id = 0)
        {
            var obj = dbe.EvaPeriodoes.Find(id);

            ViewBag.IdPeriodo = obj.Id;
            ViewBag.Periodo = obj.Periodo;
            ViewBag.Descripcion = obj.Descripcion;
            return View();
        }

        [Authorize]
        public ActionResult EditarObjetivos(int id = 0)
        {
            var obj = dbe.EvaObjetivos.Find(id);
            var cargo = dbe.CHARTs.Single(x => x.ID_CHAR == obj.IdCargo && x.VIG_CHAR == true);
            var ncargo = dbe.NAME_CHART.Single(x => x.ID_NAM_CHAR == cargo.ID_NAM_CHAR && x.VIG_CHAR == true);
            var neva = dbe.EvaNombreEvaluacions.Single(x => x.Id == obj.IdNombreEvaluacion && x.Estado == true);
            var periodo = dbe.EvaPeriodoes.Single(x => x.Id == obj.IdPeriodo && x.Estado == true);

            ViewBag.IdObjetivo = obj.Id;
            ViewBag.Periodo = periodo.Periodo;
            ViewBag.NombreEvaluacion = neva.Nombre;
            ViewBag.Cargo = Convert.ToString(ncargo.NAM_CHAR);
            ViewBag.Objetivo = obj.Nombre;
            ViewBag.Descripcion = obj.Descripcion;
            ViewBag.Indicador = obj.Indicador;
            ViewBag.Peso = obj.Peso;
            return View();
        }


        [Authorize]
        public ActionResult EditarCompetencias(int id = 0)
        {
            var obj = dbe.EvaCompetencias.Find(id);
            var cargo = dbe.CHARTs.Single(x => x.ID_CHAR == obj.IdCargo && x.VIG_CHAR == true);
            var ncargo = dbe.NAME_CHART.Single(x => x.ID_NAM_CHAR == cargo.ID_NAM_CHAR && x.VIG_CHAR == true);
            var neva = dbe.EvaNombreEvaluacions.Single(x => x.Id == obj.IdNombreEvaluacion && x.Estado == true);
            var periodo = dbe.EvaPeriodoes.Single(x => x.Id == obj.IdPeriodo && x.Estado == true);

            ViewBag.IdCompetencia = obj.Id;
            ViewBag.Periodo = periodo.Periodo;
            ViewBag.NombreEvaluacion = neva.Nombre;
            ViewBag.Cargo = Convert.ToString(ncargo.NAM_CHAR);
            ViewBag.Competencia = obj.Nombre;
            ViewBag.Descripcion = obj.Descripcion;
            ViewBag.Indicador = obj.Indicador;
            ViewBag.Peso = obj.Peso;
            return View();
        }
        #endregion

        [Authorize]
        public ActionResult VistaEvaluacionAnterior()
        {
            return View();
        }

        public ActionResult ListarEvaluacionTerminado(int id, int id1)
        {
            var query = dbe.EvaListarObjetivosTerminado(id, id1);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCalificacionFinalTerminado(int id = 0)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var query = dbe.EvaCalificacionFinalTerminado(id).SingleOrDefault();

            return Json(query.Nota, JsonRequestBehavior.AllowGet);
        }

        #region EvaluacionPersonal
        public ActionResult ListarPersonal()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);


            var query = dbe.EvaListarPersonalEvaluado(IdPersEnti, IdAcco);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ListarPersonalEvaluado(string q, string page)
        //{
        //    int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //    int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
        //    string termino = "";
        //    if (Request.QueryString["q"] == null)
        //    {
        //        termino = "";
        //    }
        //    else
        //    {
        //        termino = Request.QueryString["q"].ToString();
        //    }

        //    var result = dbe.EvaListarPersonalEvaluado(IdPersEnti,IdAcco,termino).ToList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ListarCalificacionObjetivos(int id, int id1)
        {
            var query = dbe.EvaListarCalificacionObjetivos(id, id1);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCalificacionCompetencias(int id, int id1)
        {
            var query = dbe.EvaListarCalificacionCompetencias(id, id1);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarHistoricoCalificacionObjetivos(int id, int id1)
        {
            var query = dbe.EvaListarHistoricoCalificacionObjetivos(id, id1);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarHistoricoCalificacionCompetencias(int id, int id1)
        {
            var query = dbe.EvaListarHistoricoCalificacionCompetencias(id, id1);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerPromedioCalificacion(int id = 0, int id1 = 0)
        {
            try
            {
                var query = dbe.EvaCalificacionPromedio(id, id1).SingleOrDefault();
                return Json(query.Promedio, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ObtenerEstado(int id = 0)
        {
            //var query = dbe.EvaListarEvaluacion(id).SingleOrDefault();
            //try
            //{
            //    return Json(query.IdEstado, JsonRequestBehavior.AllowGet);
            //}
            //catch
            //{
            return Json(0, JsonRequestBehavior.AllowGet);
            //}
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult GrabarEvaluacionPersonal(IEnumerable<HttpPostedFileBase> archivo)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdEvaluado = Convert.ToInt32(Request.Params["IdEvaluado"]);

            int flagObj = 0, flagComp = 0, flagPM = 0, existe = 0;

            int IdEvaluacion = 0, IdEvaObjetivo, IdEvaCompetencia, IdEvaCompetencias, IdEstadoActual = 0, Idestado = 0;

            var usuarioEvaluado = dbe.EvaUsuario(IdEvaluado).SingleOrDefault();


            string Id = Convert.ToString(Request.Params["IdEvaluacion"]);
            string IdCompetencias = Convert.ToString(Request.Params["IdEvaluacionCompetencia"]);
            if (Id == "" || IdCompetencias == "")
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeEvaluacion) top.MensajeEvaluacion('Mensaje','No tiene evaluaciones pendientes');}window.onload=init;</script>");
            }

            var evaluacion = dbe.EvaListarEvaluacion(IdEvaluado).SingleOrDefault();
            IdEvaluacion = Convert.ToInt32(evaluacion.Id);
            IdEstadoActual = Convert.ToInt32(evaluacion.IdEstado);
            string AreaMejora = Request.Params["AreaMejora"].ToString();
            string Comentario = Request.Params["Comentario"].ToString();

            if (IdEstadoActual >= 3)
            {
                //GrabarEstadoRRHH(IdEvaluado,1);
                return Content("");
            }

            //Validaciones - Objetivos
            foreach (string strIdEva in Id.Split(','))
            {
                if (!String.IsNullOrEmpty(strIdEva))
                {
                    if (IdEstadoActual == 1)
                    {
                        if ((Convert.ToString(Request.Params["IdAutoEva" + strIdEva]) == "") || (Convert.ToString(Request.Params["IdAutoEva" + strIdEva]) == "0")
                            || (Convert.ToString(Request.Params["txtAutoEva" + strIdEva]) == ""))
                        {
                            flagObj = 1;
                        }
                    }
                    else if (IdEstadoActual == 2)
                    {
                        if ((Convert.ToString(Request.Params["IdJefeEva" + strIdEva]) == "") || (Convert.ToString(Request.Params["IdJefeEva" + strIdEva]) == "0")
                            || (Convert.ToString(Request.Params["txtJefeEva" + strIdEva]) == ""))
                        {
                            flagObj = 2;
                        }
                    }
                }
            }
            //Validaciones - Competencias
            foreach (string strIdComp in IdCompetencias.Split(','))
            {
                if (!String.IsNullOrEmpty(strIdComp))
                {
                    if (IdEstadoActual == 1)
                    {
                        if ((Convert.ToString(Request.Params["IdAutoCompetencia" + strIdComp]) == "") || (Convert.ToString(Request.Params["IdAutoCompetencia" + strIdComp]) == "0")
                             || (Convert.ToString(Request.Params["txtAutoCompetencia" + strIdComp]) == ""))
                        {
                            flagComp = 2;
                        }

                    }
                    else if (IdEstadoActual == 2)
                    {
                        if ((Convert.ToString(Request.Params["IdJefeCompetencia" + strIdComp]) == "") || (Convert.ToString(Request.Params["IdJefeCompetencia" + strIdComp]) == "0")
                            || (Convert.ToString(Request.Params["txtJefeCompetencia" + strIdComp]) == ""))
                        {
                            flagComp = 2;
                        }
                    }
                }
            }
            //Validando Plan Mejora
            if (IdEstadoActual == 1 && AreaMejora == "")
            {
                flagPM = 1;

            }
            else if (IdEstadoActual == 2 && Comentario == "")
            {
                flagPM = 2;

            }



            if (flagObj != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeEvaluacion) top.MensajeEvaluacion('Mensaje','Debe registrar las observaciones por cada Objetivo');}window.onload=init;</script>");
            }
            if (flagComp != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeEvaluacion) top.MensajeEvaluacion('Mensaje','Debe registrar las observaciones por cada Competencia');}window.onload=init;</script>");
            }
            if (flagPM != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeEvaluacion) top.MensajeEvaluacion('Mensaje','Debe registrar el Plan de Mejora');}window.onload=init;</script>");
            }

            //Modificar Estado
            var EvaId = dbe.EvaEvaluacion.Where(x => x.Id == IdEvaluacion).SingleOrDefault();

            if (EvaId.IdEstado == 1) //Autoevalua
            {
                Idestado = 2;
                EvaId.UsuarioEvaluado = Convert.ToInt32(usuarioEvaluado.UserId);
                EvaId.FechaEvaluado = DateTime.Now;
            }
            else if (EvaId.IdEstado == 2) //Evalua Jefe
            {
                Idestado = 3;
                int usuarioJefe = Convert.ToInt32(Session["UserId"]);
                EvaId.UsuarioJefe = usuarioJefe;
                EvaId.FechaJefe = DateTime.Now;
            }
            EvaId.IdEstado = Idestado;

            dbe.Entry(EvaId).State = EntityState.Modified;
            dbe.SaveChanges();

            //Grabar Objetivos
            foreach (string strIdEva in Id.Split(','))
            {
                if (!String.IsNullOrEmpty(strIdEva))
                {
                    IdEvaObjetivo = Convert.ToInt32(strIdEva);
                    var EvaObjetivos = dbe.EvaCalificacionObjetivoes.Where(x => x.Id == IdEvaObjetivo).SingleOrDefault();

                    if (IdEstadoActual == 1)
                    {
                        EvaObjetivos.IdResultadoEvaluado = Convert.ToInt32(Request.Params["IdAutoEva" + strIdEva]);
                        EvaObjetivos.ObsResultadoEvaluado = Convert.ToString(Request.Params["txtAutoEva" + strIdEva]);

                    }
                    else if (IdEstadoActual == 2)
                    {
                        EvaObjetivos.IdResultadoJefe = Convert.ToInt32(Request.Params["IdJefeEva" + strIdEva]);
                        EvaObjetivos.ObsResultadoJefe = Convert.ToString(Request.Params["txtJefeEva" + strIdEva]);

                    }
                    dbe.Entry(EvaObjetivos).State = EntityState.Modified;
                    dbe.SaveChanges();
                }
            }

            //Grabar Competencias
            foreach (string strIdComp in IdCompetencias.Split(','))
            {

                if (!String.IsNullOrEmpty(strIdComp))
                {
                    IdEvaCompetencia = Convert.ToInt32(strIdComp);
                    var EvaCompetencia = dbe.EvaCalificacionCompetencias.Where(x => x.Id == IdEvaCompetencia).SingleOrDefault();

                    if (IdEstadoActual == 1)
                    {
                        EvaCompetencia.IdResultadoEvaluado = Convert.ToInt32(Request.Params["IdAutoCompetencia" + strIdComp]);

                        EvaCompetencia.ObsResultadoEvaluado = Convert.ToString(Request.Params["txtAutoCompetencia" + strIdComp]);




                    }
                    else if (IdEstadoActual == 2)
                    {
                        EvaCompetencia.IdResultadoJefe = Convert.ToInt32(Request.Params["IdJefeCompetencia" + strIdComp]);

                        EvaCompetencia.ObsResultadoJefe = Convert.ToString(Request.Params["txtJefeCompetencia" + strIdComp]);






                    }
                    dbe.Entry(EvaCompetencia).State = EntityState.Modified;
                    dbe.SaveChanges();
                }
            }

            //Plan de Mejora


            try
            {
                dbe.EvaPlanMejoraEvaluado(IdEvaluacion, AreaMejora, Comentario);

            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeEvaluacion) top.MensajeEvaluacion('ERROR','No se Actualizaron los Datos.\n\n Contacte al Administrador.');}window.onload=init;</script>");
            }

            //Validando Archivos
            if (archivo != null)
            {

                //Adjuntando Archivos
                foreach (var file in archivo)
                {
                    if (file != null)
                    {
                        try
                        {
                            var Docs = new EvaArchivo();
                            Docs.IdEvaluacion = IdEvaluacion;
                            Docs.NombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);
                            Docs.Extension = Path.GetExtension(file.FileName);
                            Docs.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                            Docs.FechaCrea = DateTime.Now;
                            Docs.Estado = true;
                            dbe.EvaArchivos.Add(Docs);
                            dbe.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/EvaluacionPersonal/" + Docs.NombreArchivo + "_" + Convert.ToString(Docs.Id) + Docs.Extension));

                        }
                        catch
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeEvaluacion) top.MensajeEvaluacion('Mensaje','No se pudo cargar el archivo.');}window.onload=init;</script>");

                        }
                    }
                }
            }

            //Enviar Correo
            try
            {
                SendMail mail = new SendMail();
                //mail.EnviarEvaluacionPersonal(IdEvaluado, IdEstadoActual, 0);

            }
            catch (Exception ex)
            {

            }
            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeEvaluacion) top.MensajeEvaluacion('OK','Se guardó correctamente');}window.onload=init;</script>");
        }

        public ActionResult GrabarEstadoRRHH(int id, int id1)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdEvaluacion, IdEstadoActual;
            int idEvaluado = id1;
            //Modificar Estado
            var evaluacion = dbe.EvaListarEvaluacion(idEvaluado).SingleOrDefault();
            IdEvaluacion = Convert.ToInt32(evaluacion.Id);
            var EvaId = dbe.EvaEvaluacion.Where(x => x.Id == IdEvaluacion).SingleOrDefault();
            IdEstadoActual = Convert.ToInt32(EvaId.IdEstado);

            if (id == 1 && IdEstadoActual == 3) { EvaId.IdEstado = 4; } //Confirma RRHH
            if (id == 2 && IdEstadoActual == 3) { EvaId.IdEstado = 2; } //Rechaza RRHH

            dbe.Entry(EvaId).State = EntityState.Modified;
            dbe.SaveChanges();

            //Enviar Correo
            try
            {
                if (id > 0)
                {
                    SendMail mail = new SendMail();
                    mail.EnviarEvaluacionPersonal(idEvaluado, IdEstadoActual, id);
                }
            }
            catch (Exception ex)
            {

            }
            return Json("OK", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdjuntarArchivosEvaluado(IEnumerable<HttpPostedFileBase> files)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var evaluacion = dbe.EvaListarEvaluacion(IdPersEnti).SingleOrDefault();

            if (evaluacion == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneIndex) top.uploadDoneIndex('Mensaje','Usted no tiene una Evaluación Pendiente');}window.onload=init;</script>");
            }

            int IdEvaluacion = Convert.ToInt32(evaluacion.Id);

            //Guardando archivo adjunto
            foreach (var file in files)
            {
                if (file != null)
                {
                    try
                    {
                        var Docs = new EvaArchivo();
                        Docs.IdEvaluacion = IdEvaluacion;
                        Docs.NombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);
                        Docs.Extension = Path.GetExtension(file.FileName);
                        Docs.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                        Docs.FechaCrea = DateTime.Now;
                        Docs.Estado = true;
                        dbe.EvaArchivos.Add(Docs);
                        dbe.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/Talent/EvaluacionPersonal/" + Docs.NombreArchivo + "_" + Convert.ToString(Docs.Id) + Docs.Extension));

                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeEvaluacion) top.MensajeEvaluacion('Mensaje','No se pudo cargar el archivo.');}window.onload=init;</script>");

                    }
                }
            }

            return Content("");
        }

        public ActionResult ObtenerArchivosEvaluado(int id = 0, int id1 = 0)
        {
            var query = dbe.EvaListarArchivos(id, id1).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region RegistroEvaluacion
        public ActionResult ListadoPeriodos()
        {
            var result = dbe.EvaListadoTotalPeriodo().ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTodosEvaluados()
        {
            var query = dbe.EvaListarTodosEvaluados().ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarEvaluadosPeriodo()
        {
            var result = dbe.EvaListarEvaluados().ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerNombreEvaluaciones(int id, string q, string page)
        {
            string termino = "";
            //int id;
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = dbe.EvaObtenerNombreEvaluaciones(id, termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        public ActionResult EvaObtenerEvaluacion(int id = 0)
        {
            var result = dbe.EvaObtenerEvaluacion(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPeriodoCargo(string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = dbe.EvaListarPeriodoCargo(termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarNombreEvaluacion(string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = dbe.EvaListarNombreEvaluacion(termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCargoObjetivos(int id = 0)
        {
            var result = dbe.EvaListarCargoObjetivos(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCargoCompetencias(int id = 0)
        {
            var result = dbe.EvaListarCargoCompetencias(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult CerrarEvaluacion()
        {
            try
            {
                int id = Convert.ToInt32(Request.Params["FormId"].ToString());
                //Cambiando de estado a la evaluacion
                var evaPeriodo = dbe.EvaPeriodoes.Where(x => x.Id == id).SingleOrDefault();
                evaPeriodo.IdEstadoPeriodo = 3;
                dbe.Entry(evaPeriodo).State = EntityState.Modified;
                dbe.SaveChanges();
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        [ValidateInput(false)]
        public ActionResult GrabarPeriodoEvaluados(FormCollection collection, EvaPeriodo evaPeriodo, EvaEvaluado evaEvaluados)
        {

            int flag = 0;
            string estado = Convert.ToString(Request.Params["FormEstadoPeriodo"].ToString());
            if (estado == "1")//Editar
            {
                var query = dbe.EvaListarPeriodo().SingleOrDefault();
                dbe.EvaEliminarEvaluados();

                List<EvaListarTodosEvaluados_Result> evaluados = dbe.EvaListarTodosEvaluados().ToList();
                EvaListarTodosEvaluados_Result[] evaluadosArray = evaluados.ToArray();

                foreach (EvaListarTodosEvaluados_Result eva in evaluadosArray)
                {
                    if (Convert.ToString(collection[eva.Evaluado]) != null)
                    {
                        evaEvaluados.IdPeriodo = query.Id;
                        evaEvaluados.IdPersEnti = eva.IdPersEntiEvaluado;
                        evaEvaluados.IdJefe = eva.IdPersEntiJefe;
                        evaEvaluados.IdCargo = eva.ID_CHAR;
                        evaEvaluados.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                        evaEvaluados.FechaCrea = DateTime.Now;
                        evaEvaluados.Estado = true;
                        dbe.EvaEvaluados.Add(evaEvaluados);
                        dbe.SaveChanges();
                    }

                }
            }
            else //Grabar
            {


                string Periodo = Convert.ToString(Request.Params["FormPeriodo"].ToString());
                string Descripcion = Convert.ToString(Request.Params["FormDescripcion"].ToString());

                //Validaciones
                //Periodo repetido
                int existe = 0;
                var result = (from per in dbe.EvaPeriodoes.Where(x => x.Periodo == Periodo && x.Estado == true).ToList()
                              select new
                              {
                                  id = per.Id,
                                  titulo = per.Periodo
                              }).ToList();

                if (result.Count() > 0)
                {
                    existe = 1;
                }

                if (existe != 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.uploadDoneEva) top.uploadDoneEva('Mensaje','El Nombre del Periodo ya existe.');}window.onload=init;</script>");
                }

                if (Periodo == "" || Descripcion == "")
                {
                    flag = 1;
                }
                if (flag != 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.uploadDoneEva) top.uploadDoneEva('Mensaje','Debe registrar todos los campos del Periodo');}window.onload=init;</script>");
                }

                evaPeriodo.Periodo = Periodo;
                evaPeriodo.Descripcion = Descripcion;
                evaPeriodo.IdEstadoPeriodo = 1;
                evaPeriodo.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                evaPeriodo.FechaCrea = DateTime.Now;
                evaPeriodo.Estado = true;
                dbe.EvaPeriodoes.Add(evaPeriodo);
                dbe.SaveChanges();

                List<EvaListarTodosEvaluados_Result> evaluados = dbe.EvaListarTodosEvaluados().ToList();

                EvaListarTodosEvaluados_Result[] evaluadosArray = evaluados.ToArray();

                string chkImpact = "";
                foreach (EvaListarTodosEvaluados_Result eva in evaluadosArray)
                {

                    if (Convert.ToString(collection[eva.Evaluado]) != null)
                    {
                        chkImpact = chkImpact + eva.IdPersEntiEvaluado + ',';
                        evaEvaluados.IdPeriodo = evaPeriodo.Id;
                        evaEvaluados.IdPersEnti = eva.IdPersEntiEvaluado;
                        evaEvaluados.IdJefe = eva.IdPersEntiJefe;
                        evaEvaluados.IdCargo = eva.ID_CHAR;
                        evaEvaluados.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                        evaEvaluados.FechaCrea = DateTime.Now;
                        evaEvaluados.Estado = true;
                        dbe.EvaEvaluados.Add(evaEvaluados);
                        dbe.SaveChanges();
                    }

                }
            }
            return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.uploadDoneEva) top.uploadDoneEva('OK','Se guardó correctamente');}window.onload=init;</script>");

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult ValidarEvaluacion()
        {
            //Periodo
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            int IdPeriodo = 0;
            if (query != null)
            {
                IdPeriodo = query.Id;
            }

            //Validacion
            List<EvaListarEvaluados_Result> evaluados = dbe.EvaListarEvaluados().ToList();
            EvaListarEvaluados_Result[] evaluadosArray = evaluados.ToArray();

            int cont1 = 0, cont2 = 0;

            foreach (EvaListarEvaluados_Result eva in evaluadosArray)
            {
                cont1 = cont1 + 1;
            }

            string json = Convert.ToString(Request.Params["DataTable"]);
            var serial = new JavaScriptSerializer();
            serial.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic objVal = serial.Deserialize(json, typeof(object));
            foreach (var item in objVal)
            {
                cont2 = cont2 + 1;
            }

            if (cont1 == 0)
            {
                return Json("No hay registros de personal seleccionado para la evaluación.", JsonRequestBehavior.AllowGet);
            }
            if (cont1 != cont2)
            {
                return Json("Seleccione una evaluación para cada personal evaluado.", JsonRequestBehavior.AllowGet);
            }

            //Validando los Pesos
            string dataVal = "";
            int idPersEntiVal = 0, IdCargoVal = 0, IdNombreEvaluacionVal = 0;
            string jsonVal = Convert.ToString(Request.Params["DataTable"]);
            var serializerVal = new JavaScriptSerializer();
            serializerVal.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic validar = serializerVal.Deserialize(jsonVal, typeof(object));


            foreach (var item in validar)
            {
                Decimal contadorObj = 0, contadorComp = 0;
                dataVal = item["name"];
                int c = 0;
                foreach (string strIdEva in dataVal.Split('/'))
                {
                    if (c == 0)
                    {
                        idPersEntiVal = Convert.ToInt32(strIdEva);
                    }
                    else
                    {
                        IdCargoVal = Convert.ToInt32(strIdEva);
                    }
                    c = c + 1;
                }
                IdNombreEvaluacionVal = Convert.ToInt32(item["value"]);

                //Objetivos
                List<EvaObtenerCargoObjetivos_Result> objetivo = dbe.EvaObtenerCargoObjetivos(IdCargoVal, IdNombreEvaluacionVal).ToList();
                // from evaObj in dbe.EvaObjetivos
                //where evaObj.IdCargo == IdCargoVal && evaObj.IdNombreEvaluacion == IdNombreEvaluacionVal
                //&& evaObj.Estado == true && evaObj.IdPeriodo == IdPeriodo
                //select evaObj;
                EvaObtenerCargoObjetivos_Result[] objArray = objetivo.ToArray();

                foreach (EvaObtenerCargoObjetivos_Result calObj in objArray)
                {
                    contadorObj = contadorObj + Convert.ToDecimal(calObj.Peso);
                }

                //Competencias
                List<EvaObtenerCargoCompetencias_Result> competencia = dbe.EvaObtenerCargoCompetencias(IdCargoVal, IdNombreEvaluacionVal).ToList();
                //from evaComp in dbe.EvaCompetencias
                // where evaComp.IdCargo == IdCargoVal
                // && evaComp.IdNombreEvaluacion == IdNombreEvaluacionVal
                // && evaComp.IdPeriodo == IdPeriodo && evaComp.Estado == true
                // select evaComp;
                EvaObtenerCargoCompetencias_Result[] compArray = competencia.ToArray();
                foreach (EvaObtenerCargoCompetencias_Result calComp in compArray)
                {
                    contadorComp = contadorComp + Convert.ToDecimal(calComp.Peso);
                }

                int IdNamChar = Convert.ToInt32(dbe.CHARTs.Single(x => x.ID_CHAR == IdCargoVal).ID_NAM_CHAR);
                string cargo = Convert.ToString(dbe.NAME_CHART.Single(x => x.ID_NAM_CHAR == IdNamChar).NAM_CHAR);


                if (contadorObj != 100)
                {
                    return Json("Verificar que los Pesos de los Objetivos sume 100 para el cargo " + cargo + '.', JsonRequestBehavior.AllowGet);
                }
                if (contadorComp != 100)
                {
                    return Json("Verificar que los Pesos de las Competencias sume 100 para el cargo " + cargo + '.', JsonRequestBehavior.AllowGet);
                }
            }
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [ValidateInput(false)]
        public ActionResult IniciarEvaluacion(EvaEvaluacion evaEvaluacion, EvaCalificacionObjetivo evaObjetivo, EvaCalificacionCompetencia evaCompetencia)
        {
            //Periodo
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            int IdPeriodo = 0;
            if (query != null)
            {
                IdPeriodo = query.Id;
            }
            ///////////////////////////////////////////////////////////////////////////////
            //Creando las Evaluaciones

            //Cambiando de estado a la evaluacion
            var evaPeriodo = dbe.EvaPeriodoes.Where(x => x.Id == IdPeriodo).SingleOrDefault();
            evaPeriodo.IdEstadoPeriodo = 2;
            dbe.Entry(evaPeriodo).State = EntityState.Modified;
            dbe.SaveChanges();

            //Recorriendo el DataTable
            string data = "";
            int idPersEnti = 0, IdCargo = 0, IdNombreEvaluacion = 0;
            string jsonDataTable = Convert.ToString(Request.Params["DataTable"]);
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(jsonDataTable, typeof(object));

            foreach (var item in obj)
            {
                data = item["name"];
                int c = 0;
                foreach (string strIdEva in data.Split('/'))
                {
                    if (c == 0)
                    {
                        idPersEnti = Convert.ToInt32(strIdEva);
                    }
                    else
                    {
                        IdCargo = Convert.ToInt32(strIdEva);
                    }
                    c = c + 1;
                }
                IdNombreEvaluacion = Convert.ToInt32(item["value"]);

                //Evaluacion
                using (var context = new EntityEntities())
                {
                    evaEvaluacion.IdPeriodo = IdPeriodo;
                    evaEvaluacion.IdNombreEvaluacion = IdNombreEvaluacion;
                    evaEvaluacion.IdPersEnti = Convert.ToInt32(idPersEnti);
                    evaEvaluacion.IdEstado = 1;
                    evaEvaluacion.UsuarioCreacion = Convert.ToInt32(Session["UserId"]);
                    evaEvaluacion.FechaCreacion = DateTime.Now;
                    evaEvaluacion.Estado = true;
                    context.EvaEvaluacion.Add(evaEvaluacion);
                    context.SaveChanges();
                }
                //Objetivos
                List<EvaObtenerCargoObjetivos_Result> objetivo = dbe.EvaObtenerCargoObjetivos(IdCargo, IdNombreEvaluacion).ToList();
                //from evaObj in dbe.EvaObjetivos
                //where evaObj.IdCargo == IdCargo && evaObj.IdNombreEvaluacion == IdNombreEvaluacion
                //&& evaObj.IdPeriodo == IdPeriodo && evaObj.Estado == true
                //select evaObj;
                EvaObtenerCargoObjetivos_Result[] objArray = objetivo.ToArray();
                foreach (EvaObtenerCargoObjetivos_Result calObj in objArray)
                {
                    evaObjetivo.IdEvaluacion = evaEvaluacion.Id;
                    evaObjetivo.IdObjetivo = Convert.ToInt32(calObj.Id);
                    evaObjetivo.Estado = true;
                    dbe.EvaCalificacionObjetivoes.Add(evaObjetivo);
                    dbe.SaveChanges();
                }

                //Competencias
                List<EvaObtenerCargoCompetencias_Result> competencia = dbe.EvaObtenerCargoCompetencias(IdCargo, IdNombreEvaluacion).ToList();
                //from evaComp in dbe.EvaCompetencias
                // where evaComp.IdCargo == IdCargo 
                // && evaComp.IdNombreEvaluacion == IdNombreEvaluacion
                // && evaComp.IdPeriodo == IdPeriodo && evaComp.Estado == true
                // select evaComp;
                EvaObtenerCargoCompetencias_Result[] compArray = competencia.ToArray();
                foreach (EvaObtenerCargoCompetencias_Result calComp in compArray)
                {
                    evaCompetencia.IdEvaluacion = evaEvaluacion.Id;
                    evaCompetencia.IdCompetencia = Convert.ToInt32(calComp.Id);
                    evaCompetencia.Estado = true;
                    dbe.EvaCalificacionCompetencias.Add(evaCompetencia);
                    dbe.SaveChanges();
                }
            }

            return Json("OK", JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult GrabarNombreEvaluacion(EvaNombreEvaluacion evaluacion)
        {
            int flag = 0;

            string Nombre = Convert.ToString(Request.Params["FormNombreEva"].ToString());

            //Validaciones
            if (Nombre == "")
            {
                flag = 1;
            }
            if (flag != 0)
            {
                return Json("Mensaje", JsonRequestBehavior.AllowGet);
            }
            evaluacion.Nombre = Nombre;
            evaluacion.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
            evaluacion.FechaCrea = DateTime.Now;
            evaluacion.Estado = true;
            dbe.EvaNombreEvaluacions.Add(evaluacion);
            dbe.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult GrabarObjetivo(EvaObjetivos evaObjetivo)
        {
            int flag = 0, flagNE = 0, flagC = 0;

            string IdPeriodo = Convert.ToString(Request.Params["IdPeriodo"].ToString());
            string IdNombreEva = Convert.ToString(Request.Params["IdNombreEvaluacion"].ToString());
            string IdCargo = Convert.ToString(Request.Params["IdCargo"].ToString());
            string Nombre = Convert.ToString(Request.Params["FormNombreObj"].ToString());
            string Descripcion = Convert.ToString(Request.Params["FormDescripcionObj"].ToString());
            string Indicador = Convert.ToString(Request.Params["FormIndicadorObj"].ToString());
            string Peso = Convert.ToString(Request.Params["FormPesoObj"].ToString());

            //Validaciones
            if (IdNombreEva == "")
            {
                flagNE = 1;
            }
            if (flagNE != 0)
            {
                return Json("MensajeNombreEvaluacion", JsonRequestBehavior.AllowGet);
            }
            if (IdCargo == "")
            {
                flagC = 1;
            }
            if (flagC != 0)
            {
                return Json("MensajeCargo", JsonRequestBehavior.AllowGet);
            }
            if (Nombre == "" || Descripcion == "" || Peso == "")
            {
                flag = 1;
            }
            if (flag != 0)
            {
                return Json("Mensaje", JsonRequestBehavior.AllowGet);
            }

            evaObjetivo.IdPeriodo = Convert.ToInt32(IdPeriodo);
            evaObjetivo.IdNombreEvaluacion = Convert.ToInt32(IdNombreEva);
            evaObjetivo.IdCargo = Convert.ToInt32(IdCargo);
            evaObjetivo.Nombre = Nombre;
            evaObjetivo.Descripcion = Descripcion;
            evaObjetivo.Indicador = Indicador;
            evaObjetivo.Peso = Convert.ToDecimal(Peso);
            evaObjetivo.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
            evaObjetivo.FechaCrea = DateTime.Now;
            evaObjetivo.Estado = true;
            dbe.EvaObjetivos.Add(evaObjetivo);
            dbe.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult GrabarCompetencia(EvaCompetencias evaCompetencia)
        {
            int flag = 0, flagNE = 0, flagC = 0;

            string IdPeriodo = Convert.ToString(Request.Params["IdPeriodo"].ToString());
            string IdNombreEva = Convert.ToString(Request.Params["IdNombreEvaluacion"].ToString());
            string IdCargo = Convert.ToString(Request.Params["IdCargo"].ToString());
            string Nombre = Convert.ToString(Request.Params["FormNombreComp"].ToString());
            string Descripcion = Convert.ToString(Request.Params["FormDescripcionComp"].ToString());
            string Indicador = Convert.ToString(Request.Params["FormIndicadorComp"].ToString());
            string Peso = Convert.ToString(Request.Params["FormPesoComp"].ToString());

            //Validaciones
            if (IdNombreEva == "")
            {
                flagNE = 1;
            }
            if (flagNE != 0)
            {
                return Json("MensajeNombreEvaluacion", JsonRequestBehavior.AllowGet);
            }
            if (IdCargo == "")
            {
                flagC = 1;
            }
            if (flagC != 0)
            {
                return Json("MensajeCargo", JsonRequestBehavior.AllowGet);
            }
            if (Nombre == "" || Descripcion == "" || Peso == "")
            {
                flag = 1;
            }
            if (flag != 0)
            {
                return Json("Mensaje", JsonRequestBehavior.AllowGet);
            }

            evaCompetencia.IdPeriodo = Convert.ToInt32(IdPeriodo);
            evaCompetencia.IdNombreEvaluacion = Convert.ToInt32(IdNombreEva);
            evaCompetencia.IdCargo = Convert.ToInt32(IdCargo);
            evaCompetencia.Nombre = Nombre;
            evaCompetencia.Descripcion = Descripcion;
            evaCompetencia.Indicador = Indicador;
            evaCompetencia.Peso = Convert.ToDecimal(Peso);
            evaCompetencia.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
            evaCompetencia.FechaCrea = DateTime.Now;
            evaCompetencia.Estado = true;
            dbe.EvaCompetencias.Add(evaCompetencia);
            dbe.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult EditarPeriodos()
        {
            int flag = 0;

            int IdPeriodo = Convert.ToInt32(Request.Params["IdPeriodo"].ToString());

            var evaPeriodo = dbe.EvaPeriodoes.Where(x => x.Id == IdPeriodo).SingleOrDefault();

            string periodo = Convert.ToString(Request.Params["FormPeriodo"].ToString());
            string descripcion = Convert.ToString(Request.Params["FormDescripcion"].ToString());

            //Validaciones
            //Periodo repetido
            int existe = 0;
            string periodoActual = Convert.ToString(dbe.EvaPeriodoes.Single(x => x.Id == IdPeriodo).Periodo);
            var result = (from per in dbe.EvaPeriodoes.Where(x => x.Periodo == periodo && x.Estado == true && x.Periodo != periodoActual).ToList()
                          select new
                          {
                              id = per.Id,
                              titulo = per.Periodo
                          }).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                               "if(top.MensajeError) top.MensajeError('Existe','');}window.onload=init;</script>");
            }

            if (periodo == "" || descripcion == "")
            {
                flag = 1;
            }
            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeError) top.MensajeError('Mensaje','');}window.onload=init;</script>");
            }

            evaPeriodo.Periodo = periodo;
            evaPeriodo.Descripcion = descripcion;
            evaPeriodo.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
            evaPeriodo.FechaModifica = DateTime.Now;
            evaPeriodo.Estado = true;

            dbe.Entry(evaPeriodo).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajePeriodo) top.MensajePeriodo('OK','Información Actualizada');}window.onload=init;</script>");

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult EditarObjetivo()
        {
            int flag = 0;

            int IdObjetivo = Convert.ToInt32(Request.Params["IdObjetivo"].ToString());

            var evaObjetivo = dbe.EvaObjetivos.Where(x => x.Id == IdObjetivo).SingleOrDefault();

            string Nombre = Convert.ToString(Request.Params["FormNombreObj"].ToString());
            string Descripcion = Convert.ToString(Request.Params["FormDescripcionObj"].ToString());
            string Indicador = Convert.ToString(Request.Params["FormIndicadorObj"].ToString());
            string Peso = Convert.ToString(Request.Params["FormPesoObj"].ToString());

            //Validaciones
            if (Nombre == "" || Descripcion == "" || Peso == "")
            {
                flag = 1;
            }
            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeObjetivo) top.MensajeObjetivo('Mensaje','Completar la información solicitada');}window.onload=init;</script>");
            }

            evaObjetivo.Nombre = Nombre;
            evaObjetivo.Descripcion = Descripcion;
            evaObjetivo.Indicador = Indicador;
            evaObjetivo.Peso = Convert.ToDecimal(Peso);
            evaObjetivo.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
            evaObjetivo.FechaModifica = DateTime.Now;
            evaObjetivo.Estado = true;

            dbe.Entry(evaObjetivo).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeObjetivo) top.MensajeObjetivo('OK','');}window.onload=init;</script>");

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult EditarCompetencia()
        {
            int flag = 0;

            int IdCompetencia = Convert.ToInt32(Request.Params["IdCompetencia"].ToString());

            var evaCompetencia = dbe.EvaCompetencias.Where(x => x.Id == IdCompetencia).SingleOrDefault();

            string Nombre = Convert.ToString(Request.Params["FormNombreComp"].ToString());
            string Descripcion = Convert.ToString(Request.Params["FormDescripcionComp"].ToString());
            string Indicador = Convert.ToString(Request.Params["FormIndicadorComp"].ToString());
            string Peso = Convert.ToString(Request.Params["FormPesoComp"].ToString());

            //Validaciones
            if (Nombre == "" || Descripcion == "" || Peso == "")
            {
                flag = 1;
            }
            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeCompetencia) top.MensajeCompetencia('Mensaje','Completar la información solicitada');}window.onload=init;</script>");
            }

            evaCompetencia.Nombre = Nombre;
            evaCompetencia.Descripcion = Descripcion;
            evaCompetencia.Indicador = Indicador;
            evaCompetencia.Peso = Convert.ToDecimal(Peso);
            evaCompetencia.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
            evaCompetencia.FechaModifica = DateTime.Now;
            evaCompetencia.Estado = true;

            dbe.Entry(evaCompetencia).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.MensajeCompetencia) top.MensajeCompetencia('OK','');}window.onload=init;</script>");

        }
        public ActionResult EliminarObjetivo()
        {
            try
            {
                int IdObjetivo = Convert.ToInt32(Request.Params["Id"]);
                EvaObjetivos objObjetivo = dbe.EvaObjetivos.Where(x => x.Id == IdObjetivo).SingleOrDefault();

                objObjetivo.Estado = false;
                dbe.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult EliminarCompetencia()
        {
            try
            {
                int IdCompetencia = Convert.ToInt32(Request.Params["Id"]);
                EvaCompetencias objCompetencia = dbe.EvaCompetencias.Where(x => x.Id == IdCompetencia).SingleOrDefault();

                objCompetencia.Estado = false;
                dbe.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        #endregion

        public ActionResult EvaListadoDesempeñoPeriodo()
        {
            //string termino = "";
            //if (Request.QueryString["q"] == null)
            //{
            //    termino = "";
            //}
            //else
            //{
            //    termino = Request.QueryString["q"].ToString();
            //}

            var result = dbe.EvaListadoDesempeñoPeriodo("").ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarDesempeno()
        {
            //string termino = "";
            //if (Request.QueryString["q"] == null)
            //{
            //    termino = "";
            //}
            //else
            //{
            //    termino = Request.QueryString["q"].ToString();
            //}

            int periodoId = Convert.ToInt32(Request.Params["periodoId"]);

            var result = dbe.EvaListadoDesempeño(periodoId).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarDesempenoCargo()
        {
            //string termino = "";
            //if (Request.QueryString["q"] == null)
            //{
            //    termino = "";
            //}
            //else
            //{
            //    termino = Request.QueryString["q"].ToString();
            //}

            int periodoId = Convert.ToInt32(Request.Params["periodoId"]);

            var result = dbe.EvaListarDesempeñoCargo(periodoId, "").ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarDesempenoEvaluado(int id, int id1, string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = dbe.EvaListarDesempeñoEvaluado(id, id1, termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EvaListarHistoricoEvaluado()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int periodoId = Convert.ToInt32(Request.Params["periodoId"]);

            var result = dbe.EvaListarHistoricoEvaluado(periodoId, IdPersEnti, "").ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ListarRptDesempeno(int id = 0)
        {
            var query = dbe.EvaReporteDesempeño(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarRptDesempeñoEvaluado(int id = 0, int id1 = 0)
        {
            var result = dbe.EvaReporteDesempeñoEvaluado(id, id1).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarRptDesempeñoJefe(int id = 0, int id1 = 0)
        {
            var result = dbe.EvaReporteDesempeñoJefe(id, id1).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarRptComparativo(int id = 0, string id1 = "", int id2 = 0)
        {
            int IdPersEnti = (string.IsNullOrEmpty(Request.Params["evaluado"]) ? 0 : Convert.ToInt32(Request.Params["evaluado"].ToString()));
            int periodoId = (string.IsNullOrEmpty(Request.Params["periodo"]) ? 0 : Convert.ToInt32(Request.Params["periodo"].ToString()));

            string cargo = Request.Params["cargo"].ToString();

            var result = dbe.EvaDesempeñoComparativo(periodoId, cargo, IdPersEnti).ToList();
            
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarEstadoEvaluados(int id = 0)
        {
            var result = dbe.EvaReporteEstadoEvaluado(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EvaListarPeriodoObjetivos(int id = 0, int id1 = 0)
        {
            var result = dbe.EvaListarPeriodoObjetivos(id, id1).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EvaListarPeriodoCompetencia(int id = 0, int id1 = 0)
        {
            var result = dbe.EvaListarPeriodoCompetencia(id, id1).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarEvaluaciones()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbe.EvaListarPersonalEvaluadoEstado(IdPersEnti, IdAcco).Where(x => x.IdPersEnti != IdPersEnti).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GuardarConformidad(int id, string tipo)
        {
            try
            {
                if (tipo == "usuario")
                {
                    EvaEvaluacion evaEvaluacion = dbe.EvaEvaluacion.Where(x => x.Id == id).SingleOrDefault();
                    evaEvaluacion.ConformeEvaluado = true;
                    evaEvaluacion.FechaConformeEvaluado = DateTime.Now;

                    Objetivo objetivo = dbe.Objetivo.Where(x => x.IdPeriodo == evaEvaluacion.IdPeriodo && x.IdObjetivo == evaEvaluacion.IdObjetivo).FirstOrDefault();
                    objetivo.IdEstadoObjetivo = 5;

                    FeedBackED feedback = dbe.FeedBackED.Where(x => x.IdPeriodo == evaEvaluacion.IdPeriodo && x.IdFeedBack == evaEvaluacion.IdFeedBack).FirstOrDefault();
                    feedback.IdEstadoFeedBack = 3;

                    dbe.SaveChanges();
                }
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content("ERROR");
            }

        }

        #region Exportar
        public ActionResult ExportarListaDesempenoEvaluado()
        {
            int IdPeriodo = 0, IdDesempenoE = 0;
            IdPeriodo = Convert.ToInt32(Request.Params["cbPeriodo"]);
            string Nombre = "Desempeño";
            if (Convert.ToInt32(Request.Params["IdDesempenoE"]) != 0)
            {
                IdDesempenoE = Convert.ToInt32(Request.Params["IdDesempenoE"]);
                var q = dbe.EvaEscalaDesempeño.Where(x => x.Id == IdDesempenoE && x.Estado == true).SingleOrDefault();
                Nombre = Convert.ToString(q.Nombre);
            }

            List<EvaReporteDesempeñoEvaluado_Result> query = dbe.EvaReporteDesempeñoEvaluado(IdPeriodo, IdDesempenoE).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:13px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".contenido{font-family:Arial, sans-serif;font-size:12px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin:5px 10px;}");
            sb.Append("</style>");
            sb.Append("<big><strong><p align='center' font-weight:'bold'>");
            sb.Append(Nombre);
            sb.Append("</p></strong></big>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='cabecera'>Apellido</td>");
            sb.Append("<th class='cabecera'>Nombre</td>");
            sb.Append("<th class='cabecera'>Evaluador</td>");
            sb.Append("<th class='cabecera'>Cargo del Evaluado</td>");
            sb.Append("<th class='cabecera'>Promedio</td>");
            sb.Append("</tr>");

            foreach (EvaReporteDesempeñoEvaluado_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.Apellido + "</td>");
                sb.Append("<td class='contenido'>" + dr.Nombre + "</td>");
                sb.Append("<td class='contenido'>" + dr.Evaluador + "</td>");
                sb.Append("<td class='contenido'>" + dr.Cargo + "</td>");
                sb.Append("<td class='contenido'>" + dr.Promedio + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=Desempeño" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        public ActionResult ExportarListaDesempenoJefe()
        {
            int IdPeriodo = 0;
            int IdDesempenoJ = 0;

            string Nombre = "Desempeño";
            IdPeriodo = Convert.ToInt32(Request.Params["cbPeriodo"]);
            if (Convert.ToInt32(Request.Params["IdDesempenoJ"]) != 0)
            {
                IdDesempenoJ = Convert.ToInt32(Request.Params["IdDesempenoJ"]);
                var q = dbe.EvaEscalaDesempeño.Where(x => x.Id == IdDesempenoJ && x.Estado == true).SingleOrDefault();
                Nombre = Convert.ToString(q.Nombre);
            }

            List<EvaReporteDesempeñoJefe_Result> query = dbe.EvaReporteDesempeñoJefe(IdPeriodo, IdDesempenoJ).ToList();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style type='text/css'>");
            sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
            sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".cabecera{background:rgb(81, 104, 130);color:white;font-family:Arial, sans-serif;font-size:13px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;}");
            sb.Append(".contenido{font-family:Arial, sans-serif;font-size:12px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:#F7FDFA;margin:5px 10px;}");
            sb.Append(".resultado{font-family:Arial, sans-serif;font-size:12px;font-weight:bold;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;border-top-width:1px;border-bottom-width:1px;background:rgb(178, 186, 196);margin:5px 10px;}");
            sb.Append("</style>");
            sb.Append("<big><strong><p align='center' font-weight:'bold'>");
            sb.Append(Nombre);
            sb.Append("</p></strong></big>");
            sb.Append("<table class='tg'>");
            sb.Append("<tr>");
            sb.Append("<th class='cabecera'>Evaluador</td>");
            sb.Append("<th class='cabecera'>Cantidad</td>");
            sb.Append("<th class='cabecera'>Porcentaje</td>");
            sb.Append("</tr>");

            foreach (EvaReporteDesempeñoJefe_Result dr in query)
            {
                sb.Append("<tr>");
                sb.Append("<td class='contenido'>" + dr.Evaluador + "</td>");
                sb.Append("<td class='contenido'>" + dr.Cantidad + "</td>");
                sb.Append("<td class='contenido'>" + dr.Porcentaje + "%</td>");
                sb.Append("</tr>");
                i = i + dr.Cantidad;
            }
            sb.Append("<tr>");
            sb.Append("<td class='resultado'>" + "TOTAL" + "</td>");
            sb.Append("<td class='resultado'>" + i + "</td>");
            sb.Append("<td class='resultado'>100%</td>");
            sb.Append("</tr>");

            sb.Append("</table>");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=DesempeñoEvaluador" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion

        public ActionResult AgregarObjetivo()
        {
            return View();
        }

        public ActionResult EditarFilaObjetivo()
        {

            int num = int.Parse(Request.Params["num"]);
            string obj = Request.Params["obj"];
            string des = Request.Params["des"];
            bool fi = bool.Parse(Request.Params["fi"]);
            int pes = int.Parse(Request.Params["pes"]);
            string uni = Request.Params["uni"];
            int met = int.Parse(Request.Params["met"]);
            string revision = Request.Params["revision"];

            ViewBag.Numero = num;
            ViewBag.Objetivo = obj;
            ViewBag.Descripcion = des;
            ViewBag.FormulaInvertida = fi;
            ViewBag.Peso = pes;
            ViewBag.Unidad = uni;
            ViewBag.Meta = met;
            ViewBag.Revision = revision.Replace(" ✏️ ", "");

            return View();
        }

        public ActionResult ComboUnidadMedida()
        {
            var result = dbe.CboUnidadMedida().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GuardarIndicadoresObjetivos(List<Dictionary<string, object>> indicadores,int IdPeriodo)
        {

            Objetivo objevaluacion = new Objetivo();

            objevaluacion.id_pers_enti = Convert.ToInt32(Session["ID_PERS_ENTI"]); ;
            objevaluacion.FechaCreacion = DateTime.Now;
            objevaluacion.UsuarioCreador = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            objevaluacion.IdEstadoObjetivo = 1;
            objevaluacion.IdPeriodo = IdPeriodo;

            dbe.Objetivo.Add(objevaluacion);
            dbe.SaveChanges();

            foreach (var indicador in indicadores)
            {
                // Verifica y convierte los valores del diccionario
                int numero = Convert.ToInt32(indicador["Numero"]);
                string objetivo = indicador["Objetivo"]?.ToString();
                string descripcion = indicador["Descripcion"]?.ToString();
                bool formulaInverted = indicador.ContainsKey("FormulaInverted") && (bool)indicador["FormulaInverted"];
                int peso = Convert.ToInt32(indicador["Peso"]);
                string unidad = indicador["Unidad"]?.ToString();
                int unidadmedida = dbe.UnidadMedida.Where(x => x.Nombre == unidad).FirstOrDefault().IdUnidadMedida;
                int meta = Convert.ToInt32(indicador["Meta"]);

                ObjetivoDetalle detalle = new ObjetivoDetalle
                {
                    IdObjetivo = objevaluacion.IdObjetivo,
                    IdEstadoObjetivoDetalle = 1,
                    FechaCreacion = DateTime.Now,
                    UsuarioCreador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                    OrdenIndicador = numero,
                    Objetivo = objetivo,
                    Descripcion = descripcion,
                    FormulaInvertida = formulaInverted,
                    Ponderado = peso,
                    IdUnidadMedida = unidadmedida,
                    Meta = meta
                };

                dbe.ObjetivoDetalle.Add(detalle);
                dbe.SaveChanges();

            }

            return Json(new { success = true, message = "Datos guardados correctamente" });
        }

        [HttpGet]
        public JsonResult ObtenerDatosEvaluacionDetalle(int id)
        {

            var datos = dbe.DatosObjetivoDetalle(id).ToList();

            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditarIndicadoresObjetivos(List<Dictionary<string, object>> indicadores, int IdEvaluacion)
        {

            Objetivo objevaluacion = dbe.Objetivo.Where(x => x.IdObjetivo == IdEvaluacion).FirstOrDefault();

            objevaluacion.UsuarioActualiza = Convert.ToInt32(Session["ID_PERS_ENTI"]); ;
            objevaluacion.FechaActualizacion = DateTime.Now;

            dbe.SaveChanges();

            var Objetivos = dbe.ObjetivoDetalle.Where(x => x.IdObjetivo == IdEvaluacion).ToList();

            foreach (var objetivo in Objetivos)
            {
                dbe.ObjetivoDetalle.Remove(objetivo);
            }
            dbe.SaveChanges();

            foreach (var indicador in indicadores)
            {
                // Verifica y convierte los valores del diccionario
                int numero = Convert.ToInt32(indicador["Numero"]);
                string objetivo = indicador["Objetivo"]?.ToString();
                string descripcion = indicador["Descripcion"]?.ToString();
                bool formulaInverted = indicador.ContainsKey("FormulaInverted") && (bool)indicador["FormulaInverted"];
                int peso = Convert.ToInt32(indicador["Peso"]);
                string unidad = indicador["Unidad"]?.ToString();
                int unidadmedida = dbe.UnidadMedida.Where(x => x.Nombre == unidad).FirstOrDefault().IdUnidadMedida;
                int meta = Convert.ToInt32(indicador["Meta"]);

                ObjetivoDetalle detalle = new ObjetivoDetalle
                {
                    IdObjetivo = objevaluacion.IdObjetivo,
                    IdEstadoObjetivoDetalle = 1,
                    FechaCreacion = DateTime.Now,
                    UsuarioCreador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                    OrdenIndicador = numero,
                    Objetivo = objetivo,
                    Descripcion = descripcion,
                    FormulaInvertida = formulaInverted,
                    Ponderado = peso,
                    IdUnidadMedida = unidadmedida,
                    Meta = meta
                };

                dbe.ObjetivoDetalle.Add(detalle);
                dbe.SaveChanges();

            }

            return Json(new { success = true, message = "Datos guardados correctamente" });


        }

        [HttpPost]
        public ActionResult GuardarFeedBackColaborador(FeedBackED model)
        {
            if (ModelState.IsValid)
            {
                //var feedback = new FeedBackED
                //{
                //    id_pers_enti = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                //    FechaCreacion = DateTime.Now,
                //    UsuarioCreador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                //    IdEstadoFeedBack = 1,
                //    FeedBackColaborador1 = model.FeedBackColaborador1,
                //    FeedBackColaborador2 = model.FeedBackColaborador2,
                //    IdPeriodo = model.IdPeriodo
                //};

                //dbe.FeedBackED.Add(feedback);

                var idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                FeedBackED feedback = dbe.FeedBackED.Where(x => x.IdPeriodo == model.IdPeriodo && x.id_pers_enti == idPersEnti).FirstOrDefault();

                if (feedback != null)
                {
                    feedback.FechaActualizacion = DateTime.Now;
                    feedback.UsuarioActualiza = idPersEnti;
                    feedback.FeedBackColaborador1 = model.FeedBackColaborador1;
                    feedback.FeedBackColaborador2 = model.FeedBackColaborador2;
                }
                else
                {
                    feedback = new FeedBackED
                    {
                        IdPeriodo = model.IdPeriodo,
                        id_pers_enti = idPersEnti,
                        UsuarioCreador = idPersEnti,
                        FechaCreacion = DateTime.Now,
                        FeedBackColaborador1 = model.FeedBackColaborador1,
                        FeedBackColaborador2 = model.FeedBackColaborador2,
                        IdEstadoFeedBack = 1,
                    };

                    dbe.FeedBackED.Add(feedback);
                }

                dbe.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpGet]
        public JsonResult ObtenerDatosFeedBacks(int id)
        {

            var datos = dbe.DatosFeedBacks(id).ToList();

            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditarFeedBackColaborador(FeedBackED model,int IdEvaluacionFeed)
        {
            if (ModelState.IsValid)
            {
                FeedBackED feedback = dbe.FeedBackED.Where(x => x.IdFeedBack == IdEvaluacionFeed).FirstOrDefault();

                feedback.id_pers_enti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                feedback.FechaActualizacion = DateTime.Now;
                feedback.UsuarioActualiza = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                feedback.FeedBackColaborador1 = model.FeedBackColaborador1;
                feedback.FeedBackColaborador2 = model.FeedBackColaborador2;

                dbe.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Invalid data" });
        }

        [Authorize]
        public ActionResult MisColaboradoresIndex()
        {

            return View();
        }
        public ActionResult ListadoSubordinados(int IdPeriodo)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = dbe.ColaboradoresxPeriodo(IdPersEnti, IdPeriodo).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult MisColaboradoresObjetivos(int id = 0, int IdPeriodo = 0)
        {
            int TieneEvaluacion = (dbe.Objetivo.Where(x=> x.id_pers_enti == id && x.IdPeriodo == IdPeriodo).Count() > 0 ? 1 : 0);
            int IdEvaluacion = (TieneEvaluacion == 0 ? 0 : Convert.ToInt32(dbe.Objetivo.Where(x => x.id_pers_enti == id && x.IdPeriodo == IdPeriodo).Single().IdObjetivo));
            int IdEstadoEvaluacion = dbe.Objetivo.Where(x => x.IdObjetivo == IdEvaluacion).Select(x => x.IdEstadoObjetivo).SingleOrDefault() ?? 0;

            ViewBag.IdPersEnti = id;
            ViewBag.TieneEvaluacion = TieneEvaluacion;
            ViewBag.IdEvaluacion = IdEvaluacion;
            ViewBag.IdEstadoEvaluacion = IdEstadoEvaluacion;
            ViewBag.IdPeriodo = IdPeriodo;

            return View();
        }


        [Authorize]
        public ActionResult MisColaboradoresFeedBack(int id = 0, int IdPeriodo = 0)
        {
            int TieneFeedBackColaborador = (dbe.FeedBackED.Where(x=> x.id_pers_enti == id && x.IdPeriodo == IdPeriodo).Count() > 0 ? 1: 0);
            int IdEvaluacionFeedBack = (TieneFeedBackColaborador == 0 ? 0 : Convert.ToInt32(dbe.FeedBackED.Where(x => x.id_pers_enti == id && x.IdPeriodo == IdPeriodo).Single().IdFeedBack) );
            int IdEstadoFeedback = dbe.FeedBackED.Where(x => x.IdFeedBack == IdEvaluacionFeedBack).Select(x => x.IdEstadoFeedBack).SingleOrDefault() ?? 0;

            ViewBag.IdPersEnti = id;
            ViewBag.IdEvaluacionFeedBack = IdEvaluacionFeedBack;
            ViewBag.IdEstadoFeedBack = IdEstadoFeedback;
            ViewBag.IdPeriodo = IdPeriodo;

            return View();
        }

        public ActionResult GuardarRevisionObjetivos(List<Dictionary<string, object>> indicadores, int IdEvaluacion)
        {

            Objetivo objevaluacion = dbe.Objetivo.Where(x => x.IdObjetivo == IdEvaluacion).FirstOrDefault();

            objevaluacion.UsuarioActualiza = Convert.ToInt32(Session["ID_PERS_ENTI"]); ;
            objevaluacion.FechaActualizacion = DateTime.Now;

            var Objetivos = dbe.ObjetivoDetalle.Where(x => x.IdObjetivo == IdEvaluacion).ToList();

            foreach (var objetivo in Objetivos)
            {
                dbe.ObjetivoDetalle.Remove(objetivo);
            }
            dbe.SaveChanges();

            foreach (var indicador in indicadores)
            {
                // Verifica y convierte los valores del diccionario
                int numero = Convert.ToInt32(indicador["Numero"]);
                string objetivo = indicador["Objetivo"]?.ToString();
                string descripcion = indicador["Descripcion"]?.ToString();
                bool formulaInverted = indicador.ContainsKey("FormulaInverted") && (bool)indicador["FormulaInverted"];
                int peso = Convert.ToInt32(indicador["Peso"]);
                string unidad = indicador["Unidad"]?.ToString();
                int unidadmedida = dbe.UnidadMedida.Where(x => x.Nombre == unidad).FirstOrDefault().IdUnidadMedida;
                int meta = Convert.ToInt32(indicador["Meta"]);
                int revision = ((indicador["Revision"]?.ToString()).Replace(" ✏️","") == "APROBADO" ? 2:((indicador["Revision"]?.ToString()).Replace(" ✏️", "") == "DESAPROBADO" ? 3 : ((indicador["Revision"]?.ToString()).Replace(" ✏️ ", "") == "CORREGIDO" ? 4 : 1)));

                ObjetivoDetalle detalle = new ObjetivoDetalle
                {
                    IdObjetivo = objevaluacion.IdObjetivo,
                    IdEstadoObjetivoDetalle = revision,
                    FechaCreacion = DateTime.Now,
                    UsuarioCreador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                    OrdenIndicador = numero,
                    Objetivo = objetivo,
                    Descripcion = descripcion,
                    FormulaInvertida = formulaInverted,
                    Ponderado = peso,
                    IdUnidadMedida = unidadmedida,
                    Meta = meta
                };


                dbe.ObjetivoDetalle.Add(detalle);
                dbe.SaveChanges();

            }

            bool existeEstado3o4 = dbe.ObjetivoDetalle
                        .Any(d => d.IdObjetivo == objevaluacion.IdObjetivo
                               && (d.IdEstadoObjetivoDetalle == 3 || d.IdEstadoObjetivoDetalle == 4));

            if (existeEstado3o4)
            {
                objevaluacion.IdEstadoObjetivo = 3;
            }
            else
            {
                objevaluacion.IdEstadoObjetivo = 2;
            }

            dbe.SaveChanges();

            return Json(new { success = true, message = "Datos guardados correctamente", estadoEvaluacionObjetivo = objevaluacion.IdEstadoObjetivo });


        }

        public ActionResult GuardarCalificacion(List<Dictionary<string, object>> indicadores, int IdEvaluacion, int TotPonderado, string Calificacion)
        {

            Objetivo objevaluacion = dbe.Objetivo.Where(x => x.IdObjetivo == IdEvaluacion).FirstOrDefault();

            objevaluacion.UsuarioActualiza = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            objevaluacion.FechaActualizacion = DateTime.Now;
            objevaluacion.NivelCumplimientoPonderado = TotPonderado;
            objevaluacion.IdDesempeño = Convert.ToInt32(dbe.ObtenerDesempeñoEvaluado(TotPonderado).Single().id);
            objevaluacion.IdEstadoObjetivo = 4;
            objevaluacion.id_pers_enti_evalu = Convert.ToInt32(Session["ID_PERS_ENTI"]);


            EvaEvaluacion evaluacion = dbe.EvaEvaluacion.Where(x => x.IdPeriodo == objevaluacion.IdPeriodo && x.IdPersEnti == objevaluacion.id_pers_enti).FirstOrDefault();
            evaluacion.IdObjetivo = objevaluacion.IdObjetivo;



            var Objetivos = dbe.ObjetivoDetalle.Where(x => x.IdObjetivo == IdEvaluacion).ToList();

            foreach (var indicador in indicadores)
            {
                try
                {
                    // Verifica y convierte los valores del diccionario
                    int numero = Convert.ToInt32(indicador["Numero"]);
                    int valorreal = Convert.ToInt32(indicador["ValorReal"]);
                    int NivCumpl = Convert.ToInt32(indicador["NivelCumplimiento"]);
                    int NivCumplPonderado = Convert.ToInt32(indicador["NivelCumplimientoPonderado"]);

                    // Buscar el objetivo correspondiente en la lista Objetivos
                    var objetivoDetalle = Objetivos.FirstOrDefault(o => o.OrdenIndicador == numero);

                    if (objetivoDetalle != null)
                    {

                        objetivoDetalle.IdEstadoObjetivoDetalle = 5;
                        objetivoDetalle.ValorReal = valorreal;
                        objetivoDetalle.NivelCumplimiento = NivCumpl;
                        objetivoDetalle.NivelCumplimientoPonderado = NivCumplPonderado;
                        objetivoDetalle.FechaActualizacion = DateTime.Now;
                        objetivoDetalle.UsuarioActualiza = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    }

                    dbe.SaveChanges();
                }
                catch (Exception e)
                {
                    return Json(new { success = false }); ;
                }
                
            }

            dbe.SaveChanges();


            return Json(new { success = true, message = "Datos guardados correctamente", estadoEvaluacionObjetivo = objevaluacion.IdEstadoObjetivo });


        }

        [Authorize]
        public ActionResult Error()
        {

            return View();
        }

        

        [Authorize]
        public ActionResult PeriodoyEvaluados()
        {
            return View();
        }

        [Authorize]
        public ActionResult RegistrarPeriodoyEvaluados()
        {
            var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.estadoPeriodo = 0;
            var eval = dbe.EvaListarPrimerNombreEvaluacion().SingleOrDefault();
            ViewBag.NombreEvaluacion = eval.NombreEvaluacion;
            if (query != null)
            {
                ViewBag.IdPeriodo = query.Id;
                ViewBag.Periodo = query.Periodo;
                ViewBag.Descripcion = query.Descripcion;
                ViewBag.estadoPeriodo = query.IdEstadoPeriodo;
            }

            return View();
        }

        [Authorize]
        public ActionResult EditarPeriodoyEvaluados(int id = 0)
        {
            //var query = dbe.EvaListarPeriodo().SingleOrDefault();
            ViewBag.IdPeriodo = 0;
            ViewBag.estadoPeriodo = 0;
            //var eval = dbe.EvaListarPrimerNombreEvaluacion().SingleOrDefault();
            EvaPeriodo periodo = dbe.EvaPeriodoes.Where(x => x.Id == id).FirstOrDefault();

            if (periodo != null)
            {
                ViewBag.IdPeriodo = periodo.Id;
                ViewBag.Periodo = periodo.Periodo;
                ViewBag.Descripcion = periodo.Descripcion;
                ViewBag.estadoPeriodo = periodo.IdEstadoPeriodo;
            }

            return View();
        }

        public ActionResult ListarPersonalED(int id = 0)
        {
            var query = dbe.DatosEditarEvaluados(id).ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarCargosEvaluados()
        {
            var result = dbe.ListarCargosEvaluados().ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult GuardarEditarPeriodoyEvaluados(FormCollection collection, EvaPeriodo evaPeriodo, EvaEvaluado evaEvaluados)
        {


            int flag = 0;
            //string estado = Convert.ToString(Request.Params["FormEstadoPeriodo"].ToString());

            string Periodo = Convert.ToString(Request.Params["FormPeriodo"].ToString());
            string Descripcion = Convert.ToString(Request.Params["FormDescripcion"].ToString());
            int IdPeriodo = Convert.ToInt32(Request.Params["FormIdPeriodo"].ToString());

            List<EvaListarTodosEvaluados_Result> evaluadosConsulta = dbe.EvaListarTodosEvaluados().ToList();
            EvaListarTodosEvaluados_Result[] evaluadosArrayConsulta = evaluadosConsulta.ToArray();

            bool todosNulos = evaluadosArrayConsulta.All(eva => Convert.ToString(collection[eva.Evaluado]) == null);

            if (todosNulos)
            {
                return Content("<script type='text/javascript'> function init() {" +
                               "if(top.uploadDoneEva) top.uploadDoneEva('Mensaje','Debe seleccionar al menos un evaluado.');}window.onload=init;</script>");
            }

            EvaPeriodo periodo = dbe.EvaPeriodoes.Where(x => x.Id == IdPeriodo).FirstOrDefault();

            periodo.Periodo = Periodo;
            periodo.Descripcion = Descripcion;
            periodo.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
            periodo.FechaCrea = DateTime.Now;
            periodo.Estado = true;

            dbe.SaveChanges();

            dbe.EliminarEvaluadosRegistrados(IdPeriodo);

                List<EvaListarTodosEvaluados_Result> evaluados = dbe.EvaListarTodosEvaluados().ToList();
                EvaListarTodosEvaluados_Result[] evaluadosArray = evaluados.ToArray();

                

                foreach (EvaListarTodosEvaluados_Result eva in evaluadosArray)
                {
                    if (Convert.ToString(collection[eva.Evaluado]) != null)
                    {
                        evaEvaluados.IdPeriodo = IdPeriodo;
                        evaEvaluados.IdPersEnti = eva.IdPersEntiEvaluado;
                        evaEvaluados.IdJefe = eva.IdPersEntiJefe;
                        evaEvaluados.IdCargo = eva.ID_CHAR;
                        evaEvaluados.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                        evaEvaluados.FechaCrea = DateTime.Now;
                        evaEvaluados.Estado = true;
                        dbe.EvaEvaluados.Add(evaEvaluados);
                        dbe.SaveChanges();

                        EvaEvaluacion evaluacion = new EvaEvaluacion();
                        evaluacion.IdPeriodo = IdPeriodo;
                        evaluacion.IdPersEnti = eva.IdPersEntiEvaluado;
                        evaluacion.IdNombreEvaluacion = 1;
                        evaluacion.IdEstado = 1;
                        evaluacion.UsuarioCreacion = Convert.ToInt32(Session["UserId"]);
                        evaluacion.FechaCreacion = DateTime.Now;
                        evaluacion.UsuarioEvaluado = eva.IdPersEntiEvaluado;
                        evaluacion.UsuarioJefe = eva.IdPersEntiJefe;
                        if (dbe.Objetivo.Where(x => x.id_pers_enti == eva.IdPersEntiEvaluado && x.IdPeriodo == IdPeriodo).Count() > 0)
                        {
                        evaluacion.IdObjetivo = Convert.ToInt32(dbe.Objetivo.Where(x => x.id_pers_enti == eva.IdPersEntiEvaluado && x.IdPeriodo == IdPeriodo).Single().IdObjetivo);
                        }
                        if (dbe.FeedBackED.Where(x => x.id_pers_enti == eva.IdPersEntiEvaluado && x.IdPeriodo == IdPeriodo).Count() > 0)
                        {
                            evaluacion.IdFeedBack = Convert.ToInt32(dbe.FeedBackED.Where(x => x.id_pers_enti == eva.IdPersEntiEvaluado && x.IdPeriodo == IdPeriodo).Single().IdFeedBack);
                        }
                    
                        dbe.EvaEvaluacion.Add(evaluacion);
                        dbe.SaveChanges();

                }

                }

            return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.uploadDoneEva) top.uploadDoneEva('OK','Se guardó correctamente');}window.onload=init;</script>");

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult GuardarNuevoPeriodoyEvaluados(FormCollection collection, EvaPeriodo evaPeriodo, EvaEvaluado evaEvaluados)
        {
            int flag = 0;

            string Periodo = Convert.ToString(Request.Params["FormPeriodo"].ToString());
            string Descripcion = Convert.ToString(Request.Params["FormDescripcion"].ToString());

            //Validaciones
            //Periodo repetido
            int existe = 0;
            var result = (from per in dbe.EvaPeriodoes.Where(x => x.Periodo == Periodo && x.Estado == true).ToList()
                          select new
                          {
                              id = per.Id,
                              titulo = per.Periodo
                          }).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                               "if(top.uploadDoneEva) top.uploadDoneEva('Mensaje','El Nombre del Periodo ya existe.');}window.onload=init;</script>");
            }

            if (Periodo == "" || Descripcion == "")
            {
                flag = 1;
            }
            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                               "if(top.uploadDoneEva) top.uploadDoneEva('Mensaje','Debe registrar todos los campos del Periodo');}window.onload=init;</script>");
            }

            List<EvaListarTodosEvaluados_Result> evaluados = dbe.EvaListarTodosEvaluados().ToList();
            EvaListarTodosEvaluados_Result[] evaluadosArray = evaluados.ToArray();

            bool todosNulos = evaluadosArray.All(eva => Convert.ToString(collection[eva.Evaluado]) == null);

            if (todosNulos)
            {
                return Content("<script type='text/javascript'> function init() {" +
                               "if(top.uploadDoneEva) top.uploadDoneEva('Mensaje','Debe seleccionar al menos un evaluado.');}window.onload=init;</script>");
            }

            evaPeriodo.Periodo = Periodo;
            evaPeriodo.Descripcion = Descripcion;
            evaPeriodo.IdEstadoPeriodo = 1;
            evaPeriodo.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
            evaPeriodo.FechaCrea = DateTime.Now;
            evaPeriodo.Estado = true;
            dbe.EvaPeriodoes.Add(evaPeriodo);
            dbe.SaveChanges();

            
            
                string chkImpact = "";
                foreach (EvaListarTodosEvaluados_Result eva in evaluadosArray)
                {
                    if (Convert.ToString(collection[eva.Evaluado]) != null)
                    {
                        chkImpact = chkImpact + eva.IdPersEntiEvaluado + ',';
                        evaEvaluados.IdPeriodo = evaPeriodo.Id;
                        evaEvaluados.IdPersEnti = eva.IdPersEntiEvaluado;
                        evaEvaluados.IdJefe = eva.IdPersEntiJefe;
                        evaEvaluados.IdCargo = eva.ID_CHAR;
                        evaEvaluados.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                        evaEvaluados.FechaCrea = DateTime.Now;
                        evaEvaluados.Estado = true;
                        dbe.EvaEvaluados.Add(evaEvaluados);
                        dbe.SaveChanges();

                        EvaEvaluacion evaluacion = new EvaEvaluacion();
                        evaluacion.IdPeriodo = evaPeriodo.Id;
                        evaluacion.IdPersEnti = eva.IdPersEntiEvaluado;
                        evaluacion.IdNombreEvaluacion = 1;
                        evaluacion.IdEstado = 1;
                        evaluacion.UsuarioCreacion = Convert.ToInt32(Session["UserId"]);
                        evaluacion.FechaCreacion = DateTime.Now;
                        evaluacion.UsuarioEvaluado = eva.IdPersEntiEvaluado;
                        evaluacion.UsuarioJefe = eva.IdPersEntiJefe;
                        dbe.EvaEvaluacion.Add(evaluacion);
                        dbe.SaveChanges();

                    }
                }
            

            //int checks

            //string chkImpact = "";
            //foreach (EvaListarTodosEvaluados_Result eva in evaluadosArray)
            //{

            //    if (Convert.ToString(collection[eva.Evaluado]) != null)
            //    {
            //        chkImpact = chkImpact + eva.IdPersEntiEvaluado + ',';
            //        evaEvaluados.IdPeriodo = evaPeriodo.Id;
            //        evaEvaluados.IdPersEnti = eva.IdPersEntiEvaluado;
            //        evaEvaluados.IdJefe = eva.IdPersEntiJefe;
            //        evaEvaluados.IdCargo = eva.ID_CHAR;
            //        evaEvaluados.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
            //        evaEvaluados.FechaCrea = DateTime.Now;
            //        evaEvaluados.Estado = true;
            //        dbe.EvaEvaluados.Add(evaEvaluados);
            //        dbe.SaveChanges();
            //    }

            //}

            return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.uploadDoneEva) top.uploadDoneEva('OK','Se guardó correctamente');}window.onload=init;</script>");

        }


        [Authorize]
        [ValidateInput(false)]
        public ActionResult InicioPeriodoEvaluacion()
        {
            try
            {
                int id = Convert.ToInt32(Request.Params["FormId"].ToString());
                //Cambiando de estado a la evaluacion
                var evaPeriodo = dbe.EvaPeriodoes.Where(x => x.Id == id).SingleOrDefault();
                evaPeriodo.IdEstadoPeriodo = 2;
                dbe.Entry(evaPeriodo).State = EntityState.Modified;
                dbe.SaveChanges();
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListaPeriodosAsignados()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = dbe.ListaPeriodosAsignados(IdPersEnti).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaPeriodosColaboradores()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = dbe.ListaPeriodosColaboradores(IdPersEnti).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult EditarFeedBackJefeDirecto(FeedBackED model)
        {
            if (ModelState.IsValid)
            {
                //FeedBackED feedback = dbe.FeedBackED.Where(x => x.IdFeedBack == IdEvaluacionFeed).FirstOrDefault();

                //feedback.id_pers_enti_evalu = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                //feedback.FechaActualizacion = DateTime.Now;
                //feedback.UsuarioActualiza = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                //feedback.FeedBackJefeDirecto1 = model.FeedBackJefeDirecto1;
                //feedback.FeedBackJefeDirecto2 = model.FeedBackJefeDirecto2;
                //feedback.IdEstadoFeedBack = 2;

                FeedBackED feedback = dbe.FeedBackED.Where(x => x.IdPeriodo == model.IdPeriodo && x.id_pers_enti == model.id_pers_enti).FirstOrDefault();

                if (feedback != null)
                {
                    feedback.id_pers_enti_evalu = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    feedback.FechaActualizacion = DateTime.Now;
                    feedback.UsuarioActualiza = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    feedback.FeedBackJefeDirecto1 = model.FeedBackJefeDirecto1;
                    feedback.FeedBackJefeDirecto2 = model.FeedBackJefeDirecto2;
                    feedback.IdEstadoFeedBack = 2;
                }
                else
                {
                    feedback = new FeedBackED
                    {
                        IdPeriodo = model.IdPeriodo,
                        id_pers_enti = model.id_pers_enti,
                        id_pers_enti_evalu = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                        UsuarioCreador = Convert.ToInt32(Session["ID_PERS_ENTI"]),
                        FechaCreacion = DateTime.Now,
                        FeedBackJefeDirecto1 = model.FeedBackJefeDirecto1,
                        FeedBackJefeDirecto2 = model.FeedBackJefeDirecto2,
                        IdEstadoFeedBack = 2,
                    };

                    dbe.FeedBackED.Add(feedback);
                    dbe.SaveChanges();
                }

                EvaEvaluacion evaluacion = dbe.EvaEvaluacion.Where(x => x.IdPeriodo == feedback.IdPeriodo && x.IdPersEnti == feedback.id_pers_enti).FirstOrDefault();
                evaluacion.IdFeedBack = feedback.IdFeedBack;

                dbe.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Invalid data" });
        }

        public ActionResult ObtenerDesempenoEvaluado(int id = 0)
        {
            var query = dbe.ObtenerDesempeñoEvaluado(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCargosEvaluadosxPeriodo()
        {
            int periodoId = Convert.ToInt32(Request.Params["periodoId"]);

            var result = dbe.ListarCargosxPeriodo(periodoId).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EvaluadoxCargoyPeriodo()
        {
            int periodoId = Convert.ToInt32(Request.Params["periodoId"]);
            string Cargo = Request.Params["Cargo"].ToString();

            var result = dbe.EvaluadoxCargoyPeriodo(periodoId,Cargo).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarEvaluacion()
        {
            try
            {
                int id = Convert.ToInt32(Request.Params["FormId"].ToString());

                dbe.EliminarPeriodoEvaluacion(id);


                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ExisteConformidad(int idperiodo)
        {
            try
            {
                int idpersenti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                var query2 = dbe.EvaluacionConforme(idpersenti, idperiodo).FirstOrDefault();
                //.Where(x => x.IdPersEnti == Convert.ToInt32(Session["ID_PERS_ENTI"])).FirstOrDefault();
                if (query2 != null)
                {
                    int IdEvaluacion = query2.IdEvaluacion;
                    bool Conforme = query2.ConformeEvaluado;

                    // Return JSON object
                    return Json(new { IdEvaluacion, Conforme });
                }
                else
                {
                    return Json(new { error = "No data found" });
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR");
            }

        }


    }

}
