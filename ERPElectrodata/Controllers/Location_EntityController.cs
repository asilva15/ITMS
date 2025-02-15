using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class Location_EntityController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        CoreEntities dbc = new CoreEntities();
        //
        // GET: /Location_Entity/
        public ActionResult Index(int id = 0)
        {
            ViewBag.ID_SITE = id;
            return View();
        }

        public ActionResult List()
        {
            var result = (from a in dbe.LOCATION_ENTITY
                          join b in dbe.SITE_ENTITY on a.ID_SITE equals b.ID_SITE
                          select new
                          {
                              a.ID_LOCA,
                              a.NAM_LOCA,
                              b.NAM_SITE,
                              NAME = b.NAM_SITE + " - " + a.NAM_LOCA,
                          }).OrderBy(x => x.NAME);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(LOCATION_ENTITY loc_ent)
        {
            int sw = 0, ID_SITE = 0;
            string NAM_LOCA = Convert.ToString(Request.Form["NAM_LOCA"]);

            if (Int32.TryParse(Convert.ToString(Request.Form["ID_SITE"]), out ID_SITE) == false) { sw = 1; }
            else if (NAM_LOCA.Trim().Length == 0) { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            try
            {
                LOCATION_ENTITY le = new LOCATION_ENTITY();
                le.ID_SITE = ID_SITE;
                le.NAM_LOCA = NAM_LOCA.ToUpper();
                le.VIG_LOCA = true;
                le.NIVEL = 1;
                dbe.LOCATION_ENTITY.Add(le);
                dbe.SaveChanges();

                string id = Convert.ToString(le.ID_LOCA);

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','Location|" + id + "');}window.onload=init;</script>"); 
            }
            catch (Exception)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
            }           
        }

        public ActionResult IndexLocacion()
        {
            return View();
        }

        public ActionResult CrearLocacion()
        {
            return View();
        }
        public ActionResult EditarLocacion(int id = 0, int id1 = 0)
        {
            LOCATION location = dbc.LOCATIONs.Single(x => x.ID_LOCA == id);
            var a = dbc.ACCOUNTs.Single(x => x.ID_ACCO == id1);
            ViewBag.IdCuenta = a.ID_ACCO;
            ViewBag.Cuenta = a.NAM_ACCO;
            ViewBag.Estado = Convert.ToInt32(location.VIG_LOCA);

            return View(location);
        }

        public ActionResult ListarLocaciones(int id = 0, int id1 = 0)
        {
            var query = dbc.ListarLocaciones(id, id1).ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSite()
        {
            string txt = "";
            int IdCuenta = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (txt == null) { txt = ""; }

            var result = dbc.ListarSite(IdCuenta, txt).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]

        public ActionResult Crear(LOCATION objLocacion)
        {
            int flag = 0, existe = 0;

            String IdCuenta = Convert.ToString(Request.Params["ID_ACCO"]);

            if (String.IsNullOrEmpty(IdCuenta)) { flag = 1; }
            if (String.IsNullOrEmpty(Convert.ToString(objLocacion.ID_SITE))) { flag = 2; }
            if (String.IsNullOrEmpty(objLocacion.NAM_LOCA)) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Por favor, complete los campos obligatorios.');}window.onload=init;</script>");
            }

            int IdSitio = Convert.ToInt32(objLocacion.ID_SITE);
            string Locacion = Convert.ToString(objLocacion.NAM_LOCA);
            var result = dbc.LOCATIONs.Where(x => x.ID_SITE == IdSitio && x.NAM_LOCA == Locacion).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE','La locación ya se encuentra registrada.');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                int IdUser = 0; //CAMBIAR POR '0'
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ha perdido la sesión.');}window.onload=init;</script>");
                }
                objLocacion.VIG_LOCA = true;
                objLocacion.NIVEL = 1;
                dbc.LOCATIONs.Add(objLocacion);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','Locación registrada.');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Contacte al administrador.');}window.onload=init;</script>");
            }
        }


        [HttpPost, ValidateInput(false)]

        public ActionResult Editar(LOCATION objLocacion)
        {
            int flag = 0, existe = 0;

            String IdCuenta = Convert.ToString(Request.Params["ID_ACCO"]);

            if (String.IsNullOrEmpty(IdCuenta)) { flag = 1; }
            if (String.IsNullOrEmpty(Convert.ToString(objLocacion.ID_SITE))) { flag = 2; }
            if (String.IsNullOrEmpty(objLocacion.NAM_LOCA)) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Por favor, complete los campos obligatorios.');}window.onload=init;</script>");
            }

            int IdSitio = Convert.ToInt32(objLocacion.ID_SITE);
            int IdLocacion = Convert.ToInt32(objLocacion.ID_LOCA);
            string Locacion = Convert.ToString(objLocacion.NAM_LOCA);
            string Estado = Convert.ToString(Request.Params["Estado"]);
            var result = dbc.LOCATIONs.Where(x => x.ID_SITE == IdSitio && x.NAM_LOCA == Locacion && x.ID_LOCA != IdLocacion).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE','La locación ya se encuentra registrada.');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                int IdUser = 0; //CAMBIAR POR '0'
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ha perdido la sesión.');}window.onload=init;</script>");
                }

                Convert.ToBoolean(objLocacion.VIG_LOCA);
                dbc.LOCATIONs.Attach(objLocacion);
                dbc.Entry(objLocacion).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','Locación actualizada.');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Contacte al administrador.');}window.onload=init;</script>");
            }
        }

    }
}
