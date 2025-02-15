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

namespace ERPElectrodata.Controllers
{
    public class ContactoController : Controller
    {
        //
        // GET: /Contacto/
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();


        public ActionResult RegistroContacto(int id)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }
        public ActionResult EditarContacto(int id)
        {
            ViewBag.IdEnti = id;
            return View();
        }
        public ActionResult VerContacto(int id)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }
        public ActionResult ContactoRenovacion(int id)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }
        public ActionResult NuevoContactoRenovacion(int id)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }
        //[HttpPost, ValidateInput(false)]
        public ActionResult GuardarContacto(int i = 0)
        {
            //Variables
            int IdDocuSale = 0;
            int IdAccoPara = 0;
            int IdPersEnti = 0;
            int IdUser = 0;
            
            try
            {
                // Validaciones 
                IdAccoPara = Convert.ToInt32(Request.Params["IdAccoPara"]);
                IdPersEnti = Convert.ToInt32(Request.Params["IdPersEnti"]);
                IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                IdUser = Convert.ToInt32(Session["UserId"].ToString());

                // Valida que el contacto no tenga relacion con el proyecto 
                var query = dbc.Contactoes.Where(x => x.IdDocuSale == IdDocuSale && x.IdPersEnti == IdPersEnti && x.Estado == true && x.IdAccoPara == IdAccoPara);
                if (query.Count() >= 1)
                {
                    return Content("EXISTE");
                }

                Contacto objContacto = new Contacto();
                objContacto.IdDocuSale = IdDocuSale;
                objContacto.IdPersEnti = IdPersEnti;
                objContacto.IdAccoPara = IdAccoPara;
                objContacto.FechaCrea = DateTime.Now;
                objContacto.UsuarioCrea = IdUser;
                objContacto.Estado = true;
                dbc.Contactoes.Add(objContacto);
                dbc.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult CargarCompania(int i = 0)
        {
            //Variables
            int IdDocuSale = 0;

            try
            {
                // Validaciones
                IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                var query = dbc.CargarCompania(IdDocuSale).SingleOrDefault();

                return Content(query.Compania);
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult AgregarNuevoContacto()
        {
            // NUEVO REGISTRO 
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
            string nom = Convert.ToString(Request.Params["FirName"]);
            string ape = Convert.ToString(Request.Params["LasName"]);
            string sex = Convert.ToString(Request.Params["SexEnti"]);
            string cel = Convert.ToString(Request.Params["CelPers"]);
            string rpm = Convert.ToString(Request.Params["RpmPers"]);
            string ext = Convert.ToString(Request.Params["ExtPers"]);
            string ema = Convert.ToString(Request.Params["EmaPers"]);
            string car = Convert.ToString(Request.Params["CarPers"]);
            string Observacion = Convert.ToString(Request.Params["Observacion"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int IdComp = 0;
            int ID_TICL = 2; //Tipo de Usuario
            var queryDocuSale = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == IdDocuSale).SingleOrDefault();
            var compania = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == queryDocuSale.ID_COMP.Value).SingleOrDefault();
            IdComp = compania.ID_ENTI;
            string ida = "0";

            int flag = 0;

            var PERS_ENTI = new PERSON_ENTITY();

            // Valida que los campos estén completos
            if (nom.Trim().Length == 0) { flag = 1; }
            else if (ape.Trim().Length == 0) { flag = 1; }
            else if (ema.Trim().Length == 0) { flag = 1; }
         
            if (flag == 1)
            {
                return Content("1"); //Campos vacíos
            }
            else
            {

                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int ID_USER = Convert.ToInt32(Session["UserId"]);
                
                int cant = dbe.CLASS_ENTITY.Where(x => x.FIR_NAME == nom.Trim() && x.LAS_NAME == ape.Trim())
                       .Join(dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == IdComp), x => x.ID_ENTI, pe => pe.ID_ENTI2, (x, pe) => new
                       {
                           x.ID_ENTI,
                           pe.ID_PERS_ENTI
                       })
                       .Join(dbe.ACCOUNT_ENTITY, x => x.ID_PERS_ENTI, ae => ae.ID_PERS_ENTI, (x, ae) => new
                       {
                           x.ID_ENTI,
                           x.ID_PERS_ENTI,
                           ae.ID_ACCO,
                           ae.VIG_ACCO_ENTI
                       }).Where(x => x.ID_ACCO == ID_ACCO && x.VIG_ACCO_ENTI == true).Count();

                if (cant > 0)
                {
                    return Content("2");//Ya existe el contacto
                }

                int idNew;
                try
                {
                    var cla = new CLASS_ENTITY();
                    cla.FIR_NAME = nom.Trim().ToUpper();
                    cla.LAS_NAME = ape.Trim().ToUpper();
                    cla.TEL_ENTI = cel;
                    cla.EMA_ENTI = ema.Trim().ToUpper();
                    cla.RPM_ENTI = rpm;
                    cla.EXT_ENTI = ext;
                    cla.SEX_ENTI = sex;
                    cla.VIG_ENTI = true;
                    cla.ID_TYPE_ENTI = 2;
                    cla.ID_TYPE_DI = 1;
                    cla.CREATED = DateTime.Now;
                    cla.ID_USER = ID_USER;
                    cla.Observaciones = Observacion;

                    dbe.CLASS_ENTITY.Add(cla);
                    dbe.SaveChanges();

                    int id = Convert.ToInt32(cla.ID_ENTI);
                    int ID_PRIO = Convert.ToInt32(dbe.TYPE_CLIENT.Single(x => x.ID_TYPE_CLIE == ID_TICL).ID_PRIO);

                    try
                    {
                        PERS_ENTI.ID_ENTI2 = id;
                        PERS_ENTI.ID_QUEU = null;
                        PERS_ENTI.ID_PRIO = ID_PRIO;
                        PERS_ENTI.ID_TYPE_CLIE = ID_TICL;
                        PERS_ENTI.CAR_PERS = car.Trim().ToUpper();
                        PERS_ENTI.CEL_PERS = cel;
                        PERS_ENTI.RPM_PERS = rpm;
                        PERS_ENTI.EXT_PERS = ext;
                        PERS_ENTI.EMA_PERS = ema;
                        PERS_ENTI.ID_ENTI1 = IdComp;
                        PERS_ENTI.CREATED = DateTime.Now;
                        PERS_ENTI.ID_USER = ID_USER;
                        PERS_ENTI.VIG_PERS_ENTI = true;
                        if (ID_ACCO == 4)
                        {
                            PERS_ENTI.ID_AREA = 44;
                        }
                        else
                        {
                            PERS_ENTI.ID_AREA = Convert.ToInt32(ida);
                        }

                        dbe.PERSON_ENTITY.Add(PERS_ENTI);
                        dbe.SaveChanges();

                        idNew = Convert.ToInt32(PERS_ENTI.ID_PERS_ENTI);

                        var ACCO_ENTI = new ACCOUNT_ENTITY();
                        ACCO_ENTI.ID_PERS_ENTI = idNew;
                        ACCO_ENTI.ID_ACCO = ID_ACCO;
                        ACCO_ENTI.VIG_ACCO_ENTI = true;
                        ACCO_ENTI.DEF_ACCO = true;
                        ACCO_ENTI.VIS_REQU = true;
                        ACCO_ENTI.VIS_ASSI = ID_ACCO == 4 ? false : true;
                        ACCO_ENTI.VIS_TALE = false;

                        dbe.ACCOUNT_ENTITY.Add(ACCO_ENTI);
                        dbe.SaveChanges();
                    }
                    catch (Exception)
                    {
                        return Content("1");
                    }
                    //string NewId = Convert.ToString(idNew);   
                    return Content("OK");
                }
                catch
                {
                    return Content("1");
                }
            }
        }

        public ActionResult ObtenerDatosContacto(int id = 0)
        {
            var query = dbc.ObtenerDatosContacto(id).ToList();
            return Json(new { Data= query}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditarPersona()
        {
            // NUEVO REGISTRO 
            int IdCompania = Convert.ToInt32(Request.Params["IdCompania"]);
            int IdEnti = Convert.ToInt32(Request.Params["IdEnti"]);
            int IdPersEnti = Convert.ToInt32(Request.Params["IdPersEnti"]);
            string nom = Convert.ToString(Request.Params["txtFirName"]);
            string ape = Convert.ToString(Request.Params["txtLasName"]);
            string sex = Convert.ToString(Request.Params["cbSexo"]);
            string cel = Convert.ToString(Request.Params["txtCelPers"]);
            string rpm = Convert.ToString(Request.Params["txtRpmPers"]);
            string ext = Convert.ToString(Request.Params["txtExtPers"]);
            string ema = Convert.ToString(Request.Params["txtEmaPers"]);
            string car = Convert.ToString(Request.Params["txtCarPers"]);
            string Observacion = Convert.ToString(Request.Params["txtObservacion"]);
            int flag = 0;

            // Valida que los campos estén completos
            if (nom.Trim().Length == 0) { flag = 1; }
            else if (ape.Trim().Length == 0) { flag = 1; }
            else if (ema.Trim().Length == 0) { flag = 1; }

            if (flag == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeEditarContacto) top.MensajeEditarContacto('Vacio');}window.onload=init;</script>");
            }
            else
            {
                int cant = dbe.CLASS_ENTITY.Where(x => x.FIR_NAME == nom.Trim() && x.LAS_NAME == ape.Trim() && x.ID_ENTI!=IdEnti)
                       .Join(dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == IdCompania), x => x.ID_ENTI, pe => pe.ID_ENTI2, (x, pe) => new
                       {
                           x.ID_ENTI,
                           pe.ID_PERS_ENTI
                       }).Count();
                if (cant > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeEditarContacto) top.MensajeEditarContacto('Existe');}window.onload=init;</script>");
                }

                try
                {
                    CLASS_ENTITY cla = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == IdEnti);
                    cla.FIR_NAME = nom.Trim().ToUpper();
                    cla.LAS_NAME = ape.Trim().ToUpper();
                    cla.TEL_ENTI = cel;
                    cla.EMA_ENTI = ema.Trim().ToUpper();
                    cla.RPM_ENTI = rpm;
                    cla.EXT_ENTI = ext;
                    cla.SEX_ENTI = sex;
                    cla.Observaciones = Observacion;
                    dbe.CLASS_ENTITY.Attach(cla);
                    dbe.Entry(cla).State = EntityState.Modified;
                    dbe.SaveChanges();

                    try
                    {
                        PERSON_ENTITY PERS_ENTI = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti).SingleOrDefault();
                        PERS_ENTI.CAR_PERS = car.Trim().ToUpper();
                        PERS_ENTI.CEL_PERS = cel;
                        PERS_ENTI.RPM_PERS = rpm;
                        PERS_ENTI.EXT_PERS = ext;
                        PERS_ENTI.EMA_PERS = ema;
                        dbe.Entry(PERS_ENTI).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeEditarContacto) top.MensajeEditarContacto('" + e.Message + "');}window.onload=init;</script>");
                    }

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeEditarContacto) top.MensajeEditarContacto('OK');}window.onload=init;</script>");
                }
                catch (Exception e)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeEditarContacto) top.MensajeEditarContacto('"+e.Message+"');}window.onload=init;</script>");
                }
            }
        }

        public ActionResult ListarContactosAgregados(int id = 0)
        {
            //int idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"].ToString());
            var query = dbc.ContactosAgregados(id).ToList();
            
            return Json(new { data = query}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarContacto()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                int idContacto = Convert.ToInt32(Request.Params["IdContacto"]);
                Contacto objContacto = dbc.Contactoes.Where(x => x.IdContacto == idContacto).SingleOrDefault();
                objContacto.FechaModifica = DateTime.Now;
                objContacto.UsuarioModifica = UserId;
                objContacto.Estado = false;
                dbc.Contactoes.Attach(objContacto);
                dbc.Entry(objContacto).State = EntityState.Modified;
                dbc.SaveChanges();
                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public JsonResult ListarContactos(string q, string page)
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

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);

            List<ListarContactos_Result> resultado = dbc.ListarContactos(IdDocuSale,termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarComboContactosOP(int id, string q, string page)
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
            List<ListarContactosOP_Result> resultado = dbc.ListarContactosOP(id, termino).ToList();
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarContactosOP(int id = 0)
        {
            var query = dbc.ListarContactosOP(id,"");
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarTipoContacto()
        {
            int idparametro = Convert.ToInt32(Request.QueryString["idparametro"]);
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = (from x in dbc.ACCOUNT_PARAMETER
                          where x.ID_PARA == idparametro &&
                          x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO
                          select new
                          {
                              id = x.ID_ACCO_PARA,
                              text = x.VAL_ACCO_PARA
                          }).OrderBy(x => x.text);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarContactosOPRenovacion()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);

            List<ListarContactos_Result> resultado = dbc.ListarContactos(IdDocuSale, "").ToList();

            return Json( new { Data = resultado, Count = resultado.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarYAgregar(int i = 0)
        {
            //Variables
            int IdDocuSale = 0;
            int IdAccoPara = 0;
            int IdPersEnti = 0;
            string celular = "";
            string correo = "";
            int IdUser = 0;

            try
            {
                // Validaciones 
                IdAccoPara = Convert.ToInt32(Request.Params["IdAccoPara"]);
                IdPersEnti = Convert.ToInt32(Request.Params["IdPersEnti"]);
                IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                celular = Request.Params["Celular"];
                correo = Request.Params["Correo"];
                IdUser = Convert.ToInt32(Session["UserId"].ToString());
                //Editar correo y celular
                CLASS_ENTITY cla = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == IdPersEnti);
                cla.CEL_ENTI = celular;
                cla.EMA_ENTI = correo.ToUpper();
                dbe.CLASS_ENTITY.Attach(cla);
                dbe.Entry(cla).State = EntityState.Modified;
                dbe.SaveChanges();
                PERSON_ENTITY PERS_ENTI = dbe.PERSON_ENTITY.Where(x => x.ID_ENTI2 == IdPersEnti).SingleOrDefault();
                PERS_ENTI.CEL_PERS = celular;
                PERS_ENTI.EMA_PERS = correo.ToUpper();
                dbe.Entry(PERS_ENTI).State = EntityState.Modified;
                dbe.SaveChanges();
                // Valida que el contacto no tenga relacion con el proyecto 
                var query = dbc.Contactoes.Where(x => x.IdDocuSale == IdDocuSale && x.IdPersEnti == IdPersEnti && x.Estado == true && x.IdAccoPara == IdAccoPara);
                if (query.Count() >= 1)
                {
                    return Content("EXISTE");
                }

                Contacto objContacto = new Contacto();
                objContacto.IdDocuSale = IdDocuSale;
                objContacto.IdPersEnti = IdPersEnti;
                objContacto.IdAccoPara = IdAccoPara;
                objContacto.FechaCrea = DateTime.Now;
                objContacto.UsuarioCrea = IdUser;
                objContacto.Estado = true;
                dbc.Contactoes.Add(objContacto);
                dbc.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }
    }
}
