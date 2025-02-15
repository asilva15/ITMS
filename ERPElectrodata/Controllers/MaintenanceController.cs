using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ERPElectrodata;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class MaintenanceController : Controller
    {
        private CoreEntities dbc = new CoreEntities();

        //
        // GET: /Maintenance/
        [Authorize]
        public ActionResult Index()
        {
            if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1 || (int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_SERVICEDESK"] == 1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult IndexMantenimiento()
        {
            if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1 || (int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_SERVICEDESK"] == 1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult MantenimientoDatos()
        {
            int idGrupo = ObtenerIdGrupoUsuarioBNV();
            if (idGrupo != 0)
            {
                ViewBag.IdGrupo = idGrupo;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult MantTipoActivoBNV(int id = 0)
        {
            ViewBag.IdGrupo = id;
            return View();
        }

        public ActionResult MantMarcaBNV(int id = 0)
        {
            ViewBag.IdGrupo = id;
            return View();
        }

        public ActionResult MantModeloComercialBNV(int id = 0)
        {
            ViewBag.IdGrupo = id;
            return View();
        }

        public ActionResult MantLocacionBNV(int id = 0)
        {
            ViewBag.IdGrupo = id;
            return View();
        }

        public ActionResult MantExcepUsuarioBNV(int id = 0)
        {
            ViewBag.IdGrupo = id;
            return View();
        }

        public ActionResult MantContratoBNV(int id = 0)
        {
            ViewBag.IdGrupo = id;
            return View();
        }

        public ActionResult MantOperadorBNV(int id = 0)
        {
            ViewBag.IdGrupo = id;
            return View();
        }

        public ActionResult CreateOperador(int id = 0, int mant = 0)
        {
            ViewBag.IdGrupo = id;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateOperador(OperadorActivo operador)
        {
            string nombreOpera = Convert.ToString(Request.Form["Nombre"]);
            if (nombreOpera.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneOperador('ERROR','0');}window.onload=init;</script>");
            }
            operador.Nombre = operador.Nombre.ToUpper();

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            if (ID_ACCO == 60)
            {
                int idGrupoAct = Convert.ToInt32(Request.Form["idGrupoAct"]);
                if (idGrupoAct != 0)
                {
                    int id;
                    try
                    {
                        int ctd = dbc.OperadorActivo.Where(x => x.Nombre == operador.Nombre && x.IdGrupo == idGrupoAct).Count();
                        if (ctd > 0)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDoneOperador('ERROR','2');}window.onload=init;</script>");
                        }

                        operador.IdGrupo = idGrupoAct;
                        operador.Estado = true;
                        operador.UsuarioCreacion = Convert.ToInt32(Session["UserId"].ToString());
                        operador.FechaCreacion = DateTime.Now;

                        dbc.OperadorActivo.Add(operador);
                        dbc.SaveChanges();

                        id = Convert.ToInt32(operador.Id);

                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDoneOperador('OK','" + id.ToString() + "');}window.onload=init;</script>");

                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDoneOperador('ERROR','1');}window.onload=init;</script>");
                    }
                }

                return Content("");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneOperador('ERROR','1');}window.onload=init;</script>");
            }

        }

        public ActionResult ListadoOperadoresBNV(int IdGrupo = 0, int IdOperador = 0)
        {
            var response = dbc.ListadoOperadores(IdGrupo, IdOperador).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoOperadorBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var opera = dbc.OperadorActivo.FirstOrDefault(x => x.Id == id);
                if (opera != null)
                {
                    opera.Estado = Convert.ToBoolean(idEstado);
                    opera.UsuarioModifica = Convert.ToInt32(Session["UserId"].ToString());
                    opera.FechaModifica = DateTime.Now;
                    dbc.OperadorActivo.Attach(opera);
                    dbc.Entry(opera).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult MantPlanBNV(int id = 0)
        {
            ViewBag.IdGrupo = id;
            return View();
        }

        public ActionResult CreatePlan(int id = 0, int idGrupo = 0, int mant = 0)
        {
            ViewBag.IdOperador = id;
            ViewBag.IdGrupo = idGrupo;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreatePlan(PlanActivo plan)
        {
            string nombrePlan = Convert.ToString(Request.Form["Nombre"]);
            if (nombrePlan.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDonePlan('ERROR','0');}window.onload=init;</script>");
            }

            int IdOperador_HF = Convert.ToInt32(Request.Form["IdOperador_HF"]);

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            if (ID_ACCO == 60)
            {
                int idGrupoAct = Convert.ToInt32(Request.Form["idGrupoAct"]);
                if (idGrupoAct != 0)
                {
                    int id;
                    try
                    {
                        int ctd = dbc.PlanActivo.Where(x => x.Nombre.ToUpper() == plan.Nombre.ToUpper() && x.IdOperador == IdOperador_HF && x.IdGrupo == idGrupoAct).Count();
                        if (ctd > 0)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDonePlan('ERROR','2','');}window.onload=init;</script>");
                        }

                        plan.Estado = true;
                        plan.IdGrupo = idGrupoAct;
                        plan.IdOperador = IdOperador_HF;
                        plan.UsuarioCreacion = Convert.ToInt32(Session["UserId"].ToString());
                        plan.FechaCreacion = DateTime.Now;

                        dbc.PlanActivo.Add(plan);
                        dbc.SaveChanges();

                        id = Convert.ToInt32(plan.Id);

                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDonePlan('OK','" + id.ToString() + "','" + nombrePlan + "');}window.onload=init;</script>");
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDonePlan('ERROR','1');}window.onload=init;</script>");
                    }
                }

                return Content("");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDonePlan('ERROR','1');}window.onload=init;</script>");
            }

        }

        public ActionResult ListadoPlanesBNV(int IdGrupo = 0, int IdOperador = 0, int IdPlan = 0)
        {
            var response = dbc.ListadoPlanes(IdGrupo, IdOperador, IdPlan).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoPlanBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var plan = dbc.PlanActivo.FirstOrDefault(x => x.Id == id);
                if (plan != null)
                {
                    plan.Estado = Convert.ToBoolean(idEstado);
                    plan.UsuarioModifica = Convert.ToInt32(Session["UserId"].ToString());
                    plan.FechaModifica = DateTime.Now;
                    dbc.PlanActivo.Attach(plan);
                    dbc.Entry(plan).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public int ObtenerIdGrupoUsuarioBNV()
        {
            var grupos = new Dictionary<string, string>
                        {
                            { "SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO", "MICROINFORMATICO" },
                            { "SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA", "INFRAESTRUCTURA" },
                            { "SUPERVISOR_ACTIVOS_BNV_MOVIL", "MOVIL" }
                        };

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string nombreGrupo = grupos.FirstOrDefault(g => Convert.ToInt32(Session[g.Key]) == 1).Value;
            var grupoActivo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.VAL_ACCO_PARA == nombreGrupo && x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO && x.ID_PARA == 1070);

            if (grupoActivo != null)
                return grupoActivo.ID_ACCO_PARA;
            else
                return 0;
        }

        public ActionResult ListadoAplicacionesBNV()
        {
            var response = dbc.AplicacionMovil.Select(x => new {
                                    x.Id,
                                    Nombre = x.Nombre,
                                    Estado = x.Estado
                                }).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MantAplicacionBNV()
        {
            return View();
        }

        public ActionResult CreateAplicacion(int id = 0, int mant = 0)
        {
            ViewBag.IdGrupo = id;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateAplicacion(AplicacionMovil aplicacion)
        {
            string nombreApp = Convert.ToString(Request.Form["Nombre"]);
            if (nombreApp.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneAplicacion('ERROR','0','');}window.onload=init;</script>");
            }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            if (ID_ACCO == 60)
            {
                try
                {
                    int id;
                    int ctd = dbc.AplicacionMovil.Where(x => x.Nombre == aplicacion.Nombre).Count();
                    if (ctd > 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDoneAplicacion('ERROR','2','');}window.onload=init;</script>");
                    }

                    aplicacion.Estado = true;
                    aplicacion.UsuarioCrea = Convert.ToInt32(Session["UserId"].ToString());
                    aplicacion.FechaCrea = DateTime.Now;

                    dbc.AplicacionMovil.Add(aplicacion);
                    dbc.SaveChanges();

                    id = Convert.ToInt32(aplicacion.Id);

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneAplicacion('OK','" + id.ToString() + "','" + aplicacion.Nombre + "');}window.onload=init;</script>");

                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneAplicacion('ERROR','1','');}window.onload=init;</script>");
                }

                return Content("");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneAplicacion('ERROR','1','');}window.onload=init;</script>");
            }

        }

        public ActionResult ModificarEstadoAplicacionBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var app = dbc.AplicacionMovil.FirstOrDefault(x => x.Id == id);
                if (app != null)
                {
                    app.Estado = Convert.ToBoolean(idEstado);
                    app.UsuarioActualiza = Convert.ToInt32(Session["UserId"].ToString());
                    app.FechaActualiza = DateTime.Now;
                    dbc.AplicacionMovil.Attach(app);
                    dbc.Entry(app).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

    }
}
