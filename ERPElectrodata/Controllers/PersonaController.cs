using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using ERPElectrodata.Object;
using ERPElectrodata.Object.Ticket;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.IO;
using ERPElectrodata.Objects;
using System.Web.Security;
using System.Data.Entity.SqlServer;
using System.Text;
namespace ERPElectrodata.Controllers
{
    public class PersonaController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbo = new CoreEntities();
        public static int idCuentaPers = 0;
        public ActionResult Index()
        {
            ViewBag.ID_ACCO = (int)Session["ID_ACCO"];

            return View();
        }

        public ActionResult Crear()
        {
            return View();
        }

        public ActionResult Editar(int id)
        {
            ViewBag.ID_PERS_ENTI = id;
            var pe = dbe.PERSON_ENTITY.Find(id);
            var c = dbe.CHARTs.Find(pe.ID_CHAR);
            var ch = dbe.NAME_CHART.Find(c != null ? c.ID_NAM_CHAR : null);
            var TC = dbe.TYPE_CLIENT.Find(pe.ID_TYPE_CLIE);
            var ar = dbe.AREAs.Find(pe.ID_AREA);
            var cp = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == pe.ID_ENTI2);
            //viewbag
            ViewBag.FIR_NAME = string.IsNullOrEmpty(cp.FIR_NAME) ? " " : cp.FIR_NAME;
            ViewBag.SEC_NAME = string.IsNullOrEmpty(cp.SEC_NAME) ? " " : cp.SEC_NAME;
            ViewBag.LAS_NAME = string.IsNullOrEmpty(cp.LAS_NAME) ? " " : cp.LAS_NAME;
            ViewBag.MOT_NAME = string.IsNullOrEmpty(cp.MOT_NAME) ? " " : cp.MOT_NAME;
            ViewBag.SEX_ENTI = cp.SEX_ENTI;
            ViewBag.CEL_ENTI = cp.CEL_ENTI;
            ViewBag.CEL_PERS = pe.CEL_PERS;
            ViewBag.RPM_PERS = pe.RPM_PERS;
            ViewBag.EMA_PERS = string.IsNullOrEmpty(pe.EMA_PERS) ? " " : pe.EMA_PERS;
            ViewBag.FOT_PERS = string.IsNullOrEmpty(pe.FOT_PERS) ? " " : pe.FOT_PERS;
            ViewBag.ID_CHAR = pe.ID_CHAR;
            if (ch == null)
                ViewBag.NAM_char = "";
            else
                ViewBag.NAM_char = string.IsNullOrEmpty(ch.NAM_CHAR) ? " " : ch.NAM_CHAR;
            ViewBag.CAR_PERS = string.IsNullOrEmpty(pe.CAR_PERS) ? " " : pe.CAR_PERS;
            ViewBag.ID_PERS_ENTI = pe.ID_PERS_ENTI;
            ViewBag.UAD_PERS = string.IsNullOrEmpty(pe.UAD_PERS) ? " " : pe.UAD_PERS;
            ViewBag.ID_TYPE_CLIE = TC.ID_TYPE_CLIE;
            ViewBag.NAM_TYPE_CLIE = string.IsNullOrEmpty(TC.NAM_TYPE_CLIE) ? " " : TC.NAM_TYPE_CLIE;
            ViewBag.ID_AREA = pe.ID_AREA;
            ViewBag.NOM_AREA = string.IsNullOrEmpty(ar.NOM_AREA) ? " " : ar.NOM_AREA;
            return View();
        }

        public ActionResult EditarEstado(int id)
        {
            var ActivosAsignados = (from ca in dbo.CLIENT_ASSET.Where(x => x.ID_PERS_ENTI == id && x.DAT_END == null)
                                    join a in dbo.ASSETs on ca.ID_ASSE equals a.ID_ASSE
                                    select new
                                    {
                                        ca.ID_ASSE,
                                        a.ID_ACCO
                                    }).Where(x => x.ID_ACCO != 0);

            if (ActivosAsignados.Count() > 0)
            {
                return Content("El Usuario tiene equipos " + ActivosAsignados.Count().ToString() + " asignados, no se puede Eliminar.");
            }
            var Persona = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id).SingleOrDefault();
            int IdEnti = Convert.ToInt32(Persona.ID_ENTI2);
            var PersonaDetalle = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == IdEnti).SingleOrDefault();

            Persona.VIG_PERS_ENTI = false;
            PersonaDetalle.VIG_ENTI = false;

            dbe.CLASS_ENTITY.Attach(PersonaDetalle);
            dbe.PERSON_ENTITY.Attach(Persona);
            dbe.Entry(Persona).State = EntityState.Modified;
            dbe.Entry(PersonaDetalle).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("OK");
        }

        public ActionResult Listar()
        {
            int IdCompania = Convert.ToInt32(Request.Params["idCompania"]);
            string Sexo = Convert.ToString(Request.Params["Sexo"]);
            int IdTipoUsuario = Convert.ToInt32(Request.Params["IdTipoUsuario"]);
            string PalabraClave = Convert.ToString(Request.Params["PalabraClave"]);
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var query = dbe.ListarxCompania(IdCompania, Sexo, IdTipoUsuario, PalabraClave, IdCuenta).ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Guardar(int id)
        {
            return View();
        }

        public ActionResult listarCompania(string q, string page)
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
            var result = dbo.ListarCompania(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarUser(string q, string page)
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
            var result = dbe.ListarTipoCliente(termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LisArea(string q, string page)
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = dbe.ListarAreas(IdCuenta, termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarCargo(string q, string page)
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var result = dbe.ListarCargo(IdCuenta, termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarSexo()
        {
            List<Sexo> ListaSexo = new List<Sexo>();
            ListaSexo.Add(new Sexo { id = "M", text = "MASCULINO" });
            ListaSexo.Add(new Sexo { id = "F", text = "FEMENINO" });
            var result = ListaSexo.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // Clases
        public class Sexo
        {
            public string id { get; set; }
            public string text { get; set; }
        }
        public ActionResult ActualizarComp(FormCollection collection, SLADetalle objslad)
        {
            string msjError = string.Empty;
            int ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI".ToString()]);
            //combos
            String vsexo = "";
            try
            {
                vsexo = Request.Params["CbSex"].ToString();
            }
            catch
            {
                vsexo = "";
            }
            String vcargo = "";
            try
            {
                vcargo = Request.Params["CbCargo"].ToString();
            }
            catch
            {
                vcargo = "";
            }
            String vTipUser = "";
            try
            {
                vTipUser = Request.Params["TipUser"].ToString();
            }
            catch
            {
                vTipUser = "";
            }
            String vcbArea = "";
            try
            {
                vcbArea = Request.Params["cbArea"].ToString();
            }
            catch
            {
                vcbArea = "";
            }
            try
            {
                String pNombre = Request.Params["pNombre"].ToString();
                String sNombre = Request.Params["sNombre"].ToString();
                String pApellido = Request.Params["pApellido"].ToString();
                String sApellido = Request.Params["sApellido"].ToString();
                String celularp = Request.Params["celularp"].ToString();
                String celular = Request.Params["celular"].ToString();
                String rpm = Request.Params["rpm"].ToString();
                String extension = Request.Params["exten"].ToString();
                String cPersonal = Request.Params["cPersonal"].ToString();
                String fotocheck = Request.Params["fotoch"].ToString();
                String userA = Request.Params["userA"].ToString();

                // Modificacion PERSON_ENTITY
                PERSON_ENTITY Persona = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI).SingleOrDefault();

                Persona.ID_AREA = Convert.ToInt32(vcbArea);
                Persona.UAD_PERS = userA;
                Persona.CEL_PERS = celularp;
                Persona.RPM_PERS = rpm;
                Persona.FOT_PERS = fotocheck;
                Persona.ID_CHAR = Convert.ToInt32(vcargo);
                Persona.ID_TYPE_CLIE = Convert.ToInt32(vTipUser);
                dbe.Entry(Persona).State = EntityState.Modified;
                dbe.SaveChanges();

                // Modificacion CLASS_ENTITY
                int IdEnti = Convert.ToInt32(Persona.ID_ENTI2);
                CLASS_ENTITY cEntity = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == IdEnti).SingleOrDefault();

                cEntity.FIR_NAME = pNombre;
                cEntity.LAS_NAME = pApellido;
                cEntity.MOT_NAME = sApellido;
                cEntity.SEC_NAME = sNombre;
                cEntity.SEX_ENTI = vsexo;
                cEntity.CEL_ENTI = celular;
                cEntity.RPM_ENTI = rpm;
                cEntity.EXT_ENTI = extension;
                cEntity.EMA_ENTI = cPersonal;

                dbe.Entry(cEntity).State = EntityState.Modified;
                dbe.SaveChanges();

            }

            catch (Exception e)
            {
                string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDoneNR) top.uploadDoneNR('OK','El registro se guardó correctamente');}window.onload=init;</script>");

        }

    }





}