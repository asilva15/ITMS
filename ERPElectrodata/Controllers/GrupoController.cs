using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Web.Helpers;
using ERPElectrodata.AppCode;
using ERPElectrodata.Helpers;
using System.Data.Entity;
using System.Text;

using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;

using ERPElectrodata.Object;


namespace ERPElectrodata.Controllers
{
    public class GrupoController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Grupo/
        //public static int Idacco = 0;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexMantGrupo()
        {
            return View();
        }

        public ActionResult CrearGrupo()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult CreaNuevoGrupo(FormCollection collection, GRUPOPLANTILLA objgrupo)
        {
            string msjError = string.Empty;
            String cuenta = "";

            try
            {
                cuenta = Request.Params["cbCuentaCrea"].ToString();
            }
            catch
            {
                cuenta = "";
            }

            String Descgrupo = Request.Params["TextareaGrupo"].ToString();

            try
            {
                String Nomgrupo = Request.Params["GrupoCrea"].ToString();


                #region validaciones
                if (cuenta == "" || Nomgrupo == "")
                {
                    msjError = "Debe completar los datos";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                int IdCuenta = Convert.ToInt32(cuenta);
                var val = (from gp in dbc.GRUPOPLANTILLA.Where(x => x.IdCuenta == IdCuenta && x.NOMBRE == Nomgrupo) select new { IDGRUPO = gp.ID_GRUPO, NOMBRE = gp.NOMBRE }).ToList();

                if (val.Count() > 0)
                {
                    msjError = "El nombre del GRUPO ya se encuentra registrada.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                #endregion
                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    msjError = "No se encuentra registro de usuario";
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                objgrupo.IdCuenta = IdCuenta;
                objgrupo.NOMBRE = Nomgrupo;
                objgrupo.DESCRIPCION = Descgrupo;
                objgrupo.FECHACREACION = DateTime.Now;
                objgrupo.USUARIOCREACION = IdUser;
                objgrupo.ESTADO = true;
                dbc.GRUPOPLANTILLA.Add(objgrupo);
                dbc.SaveChanges();

            }
            catch (Exception e)
            {
                string Error = e.Message;
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDoneNR) top.uploadDoneNR('OK','El registro se guardó correctamente');}window.onload=init;</script>");


        }



        public ActionResult EditarGrupo(int Id = 0)
        {
            var gp = (from a in dbc.GRUPOPLANTILLA.Where(x => x.ID_GRUPO == Id).ToList()
                      join b in dbc.ACCOUNTs on a.IdCuenta equals b.ID_ACCO
                      select new { Nombre = a.NOMBRE, Estado = a.ESTADO, Id_Cuenta = a.IdCuenta, Cuenta = b.NAM_ACCO, Descripcion = a.DESCRIPCION }).Single();
            ViewBag.IdGrupo = Id;
            ViewBag.IdCuenta = gp.Id_Cuenta;
            ViewBag.Cuenta = gp.Cuenta;
            ViewBag.Nombre = gp.Nombre.ToString();
            ViewBag.Descrip = gp.Descripcion.ToString();
            ViewBag.Estado = Convert.ToInt32(gp.Estado);
            return View();

        }

        public ActionResult ListarGrupoTodo()
        {
            int IdCuenta = 0;
            String cuenta = Request.Params["IdCuenta"].ToString();
            if (cuenta != "") { IdCuenta = Convert.ToInt32(cuenta); }
            var result = dbc.ListarBuscarGrupo(IdCuenta).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }
        
        public ActionResult ListarCboGrupoCuentaEstado(int ID_ACCO = 0) //Para listar de Tickets
        {
             List<ListarGrupoCuentaEstado_Result> query = dbc.ListarGrupoCuentaEstado(ID_ACCO, "", "1").ToList();
             var result = (from c in query.ToList() select new { IDGRUPO = c.ID_GRUPO, NOMBRE = c.Nombre });

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EdicionGrupo(FormCollection collection) //GRUPOPLANTILLA objGrupo
        {
        
            string msjError = string.Empty;
            String cuenta = "";
            
            int Idgrupo = Convert.ToInt32(Request.Params["IDGrupo"]);
        
            GRUPOPLANTILLA objgroup = dbc.GRUPOPLANTILLA.Where(x => x.ID_GRUPO == Idgrupo).SingleOrDefault();

            try
            {

                cuenta = Convert.ToString(objgroup.IdCuenta); //Request.Params["cbCuentaE"].ToString();
            }
            catch
            {
                cuenta = "";
            }

            String Descgrupo = Request.Params["TextareaGrupoEdita"].ToString();

            try
            {
                String Nomgrupo = Request.Params["GrupoE"].ToString();

                #region validaciones
                if (cuenta == "" || Nomgrupo == "")
                {
                    msjError = "Debe completar los datos";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                int IdCuenta = Convert.ToInt32(cuenta);
                var val = (from gp in dbc.GRUPOPLANTILLA.Where(x => x.IdCuenta == IdCuenta && x.NOMBRE == Nomgrupo && x.ID_GRUPO != Idgrupo) select new { Idgrupo = gp.ID_GRUPO, NomGrupo = gp.NOMBRE }).ToList();

                if (val.Count() > 0)
                {
                    msjError = "El nombre del GRUPO ya se encuentra registrada.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }


                #endregion

                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    msjError = "No se encuentra registro de usuario";
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                string Estado = Convert.ToString(Request.Params["EstadoE"]);

                GRUPOPLANTILLA objgrupo = dbc.GRUPOPLANTILLA.Find(Idgrupo);
                objgrupo.IdCuenta = IdCuenta;
                objgrupo.NOMBRE = Nomgrupo;
                objgrupo.DESCRIPCION = Descgrupo;
                objgrupo.FECHAMODIFICA = DateTime.Now;
                objgrupo.USUARIOMODIFICA = IdUser;
                if (Estado == "on")
                {
                    objgrupo.ESTADO = true;
                }
                else
                {
                    objgrupo.ESTADO = false;
                }

                dbc.Entry(objgrupo).State = EntityState.Modified;
                dbc.SaveChanges();

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
