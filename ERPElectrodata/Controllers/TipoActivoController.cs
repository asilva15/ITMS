using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

namespace ERPElectrodata.Controllers
{
    public class TipoActivoController : Controller
    {


        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();

        public static int idTipoActivo = 0;
        public static int ID_ACCO =0;
        //
        // GET: /TipoActivo/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Crear()
        {
            return View();
        }

        public ActionResult CrearCuentaTA()
        {
            return View();
        }
        
        


        [HttpPost, ValidateInput(false)]
        public ActionResult CrearCuentaTA(ACCOUNT_TYPE_ASSET objACCOUNT_TYPE_ASSET)
        {
            int ID_TYPE_ASSET = Convert.ToInt32(Session["ID_TYPE_ASSET"]);
            //Validar campos en blanco
            int flag = 0,existe=0;

            if (Convert.ToString(objACCOUNT_TYPE_ASSET.ID_TYPE_ASSE) == "") { flag = 1; }
            if (Convert.ToString(objACCOUNT_TYPE_ASSET.ID_ACCO) == "") { flag = 2; }

            if (objACCOUNT_TYPE_ASSET.MONTH_LIFE == null) { flag = 3; }

            int Cuenta=objACCOUNT_TYPE_ASSET.ID_ACCO;
            int Tipo = objACCOUNT_TYPE_ASSET.ID_TYPE_ASSE;

            

            var result = (from at in dbc.ACCOUNT_TYPE_ASSET.Where(x => x.ID_ACCO == Cuenta
                            && x.ID_TYPE_ASSE == Tipo).ToList()
                          select new
                          {
                              id = at.ID,
                              cuenta = at.ID_ACCO
                          }).ToList();
            
            if (result.Count() >0)
            {
                existe = 1;
            }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }
            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {

                
                //Generar los componentes de acuerdo al campo ingresado 'stock'
                
                dbc.ACCOUNT_TYPE_ASSET.Add(objACCOUNT_TYPE_ASSET);
                    dbc.SaveChanges();
                

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objACCOUNT_TYPE_ASSET.ID_TYPE_ASSE.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }


        public ActionResult ListarCuentaTipoActivo()
        {
            var query = dbc.CTAListarCuentaTipoActivo().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Crear(TYPE_ASSET objComponente)
        {
            int ID_TYPE_ASSET = Convert.ToInt32(Session["ID_TYPE_ASSET"]);
            //Validar campos en blanco
            int flag = 0;

            if (String.IsNullOrEmpty(objComponente.NAM_TYPE_ASSE)) { flag = 2; }
            if (String.IsNullOrEmpty(objComponente.DESC_TYPE_ASSE)) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {


                //Generar los componentes de acuerdo al campo ingresado 'stock'

                objComponente.IN_GRAP = true;
                Convert.ToString(objComponente.COLOR);
                objComponente.INDICE = 6;
                dbc.TYPE_ASSET.Add(objComponente);
                dbc.SaveChanges();


                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objComponente.ID_TYPE_ASSE.ToString() + "');}window.onload=init;</script>");
                
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        //Lista y Búsqueda de componentes
        public ActionResult BuscarListarTipoActivo(int i = 0)
        {
            // int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ID_ACCO = Convert.ToInt32(Request.Params["ID_ACCO"].ToString());

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            idTipoActivo = Convert.ToInt32(Request.Params["ID_TYPE_ASSE"].ToString());
            int meses = Convert.ToInt32(Request.Params["MONTH_LIFE"].ToString());

            var query = dbc.CuentaListarBuscarTipoActivo(ID_ACCO, idTipoActivo, meses).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Editar(int id = 0)
        {
            ViewBag.ID_TYPE_ASSE = id;
            var pe = dbc.TYPE_ASSET.Find(id);
            
            ViewBag.NAM_TYPE_ASSE = pe.NAM_TYPE_ASSE;
            ViewBag.DESC_TYPE_ASSE = pe.DESC_TYPE_ASSE;
            ViewBag.COLOR = pe.COLOR;

            return View(pe);
        }

        public ActionResult RequesterByAcco()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.TYPE_ASSET.Where(ae => ae.ID_TYPE_ASSE == ID_ACCO).ToList();

            var result = (from x in query
                          
                          select new
                          {
                              CLIE = x.ID_TYPE_ASSE,
                              x.NAM_TYPE_ASSE,
                              x.DESC_TYPE_ASSE,
                          }).ToList().OrderBy(x => x.CLIE);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditarTipoActivo(TYPE_ASSET objTipoActivo)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_TYPE_ASSET = Convert.ToInt32(Session["ID_TYPE_ASSET"]);
            //Validar campos en blanco
            int flag = 0;

            if (String.IsNullOrEmpty(objTipoActivo.NAM_TYPE_ASSE)) { flag = 1; }
            if (String.IsNullOrEmpty(objTipoActivo.DESC_TYPE_ASSE)) { flag = 2; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }


            if (ModelState.IsValid)
            {
                objTipoActivo.IN_GRAP = true;
                //objTipoActivo.COLOR = "#00000";
                Convert.ToString(objTipoActivo.COLOR);
                objTipoActivo.INDICE = 6;
                dbc.TYPE_ASSET.Attach(objTipoActivo);
                dbc.Entry(objTipoActivo).State = EntityState.Modified;
                dbc.SaveChanges();
            
            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult EditarCuenta(int id = 0, int id1 = 0)
        {
            
                
                //var pe = dbc.ACCOUNT_TYPE_ASSET.Find(id,idAcco);

                var at = dbc.TYPE_ASSET.Find(id);
                var a = dbc.ACCOUNTs.Find(id1);
                var f = dbc.ACCOUNT_TYPE_ASSET.Single(x => x.ID_TYPE_ASSE == at.ID_TYPE_ASSE && x.ID_ACCO == a.ID_ACCO);

                //var cp = dbc.ACCOUNTs.Single(x => x.ID_ACCO == pe.ID_ACCO);
                //var ta = dbc.TYPE_ASSET.Single(x => x.ID_TYPE_ASSE == pe.ID_TYPE_ASSE);
                ViewBag.ID_TYPE_ASSE = at.ID_TYPE_ASSE;
                ViewBag.ID_ACCO = a.ID_ACCO;
                ViewBag.NAM_ACCO = a.NAM_ACCO;
                ViewBag.NAM_TYPE_ASSE = at.NAM_TYPE_ASSE;
                ViewBag.MONTH_LIFE = f.MONTH_LIFE;
                ViewBag.ID = f.ID;
                return View(f);
            
        }

        //public ActionResult CrearCuentaTA()
        //{
        //    try
        //    {
        //        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

        //        var cInc = new TICKET();
        //        cInc.FEC_TICK = DateTime.Now;
        //        ViewBag.DATE = String.Format("{0:g}", cInc.FEC_TICK);
        //        ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
        //        ViewBag.ACCESO_NEWREQ = 0;
        //        ViewBag.ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
        //        ViewBag.ID_PERS_ENTI_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

        //        string UserName = Convert.ToString(Session["UserName"]);
        //        string[] rolesArray = Roles.GetRolesForUser(UserName);
        //        Roles.GetRolesForUser(UserName);


        //        foreach (string xc in rolesArray)
        //        {
        //            int i = Array.IndexOf(rolesArray, xc);
        //            if (xc == "SERVICE DESK" || xc == "SYSTEMADMINISTRATOR" || xc == "ADMINISTRADOR")
        //            {
        //                ViewBag.ACCESO_NEWREQ = 1;
        //            }
        //        }

        //        bool cia = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).VIS_COMP.Value;
        //        if (cia)
        //        {
        //            return View("CrearCuentaTA");
        //        }
        //        else
        //        {
        //            return View();
        //        }
        //    }
        //    catch
        //    {
        //        return Content("Please Close Session");
        //    }
        //}
    

        [HttpPost, ValidateInput(false)]
        public ActionResult EditarCuentaTA(ACCOUNT_TYPE_ASSET objAccountType)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            int Month_Life = Convert.ToInt32(Request.Params["MONTH_LIFE_ACC"].ToString());

            objAccountType.MONTH_LIFE = Month_Life;

            int flag = 0;
            if (Convert.ToString(objAccountType.ID_TYPE_ASSE) == "") { flag = 1; }
            if (Convert.ToString(objAccountType.ID_ACCO) == "") { flag = 2; }

            if (objAccountType.MONTH_LIFE == null) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneEditarCuenta('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                dbc.ACCOUNT_TYPE_ASSET.Attach(objAccountType);
                dbc.Entry(objAccountType).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneEditarCuenta('OK','0','" + objAccountType.ID.ToString() + "');}window.onload=init;</script>");

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneEditarCuenta('ERROR','1','0');}window.onload=init;</script>");
            }
        }
    
        public ActionResult ListarTipoActivoCuenta()
        {
            string txt = "";
            int ID_ACCO = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (txt == null) { txt = ""; }
    
            var query = dbc.ListarTipoActivoCuenta(ID_ACCO,txt).ToList();
    
            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
    
        public ActionResult ListarCuenta()
        {
            string txt = "";
            int ID_ACCO = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
    
            txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (txt == null) { txt = ""; }
    
            var query = dbc.ListarCuentaTipoActivo(ID_ACCO, txt).ToList();
    
            return Json(new { Data = query,Count= query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarTipoActivo(string q, string page)
        {
            string termino = "";
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idTipoAsignacion = Convert.ToInt32(Request.Params["IdTipoAsignacion"]);

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var resultado = dbc.ListarTipoActivo(idTipoAsignacion, termino, idAcco).ToList();
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

    }
}
