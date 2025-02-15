using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace ERPElectrodata.Controllers
{
    public class CuentaPermisosController : Controller
    {
        //
        // GET: /CuentaPermisos/
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;

        //cuentas y permisos
        public string CapitalizeCadena(string cadena)
        {
            cadena = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cadena.ToLower());
            return cadena;
        }
        public ActionResult Index()
        {
            if ((int)Session["SUPERVISOR_SERVICEDESK_ELECTRODATA"] == 1 || (int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_SERVICEDESK_CUENTA"] == 1)
            {
                return View();
            }
           
            else
            {
                return RedirectToAction("Index", "Error");
            }
            //return View();
        }

        public ActionResult EditarColaUsuario()
        {
            if ((int)Session["SUPERVISOR_SERVICEDESK_ELECTRODATA"] == 1 || (int)Session["ADMINISTRADOR"] == 1)
            {
                ViewBag.ID_ACCO = (int)Session["ID_ACCO"];
                return View();
            }else if ((int)Session["SUPERVISOR_SERVICEDESK_CUENTA"] == 1)
            {
                return RedirectToAction("EditarColaUsuarioCuenta", "CuentaPermisos");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult EditarColaUsuarioCuenta()
        {

            ViewBag.ID_ACCO = (int)Session["ID_ACCO"];
            return View();
            
        }

        // SUPERVISOR_CUENTA_ELECTRODATA
        public ActionResult UsuarioPorCuenta_SPELC(string options, string page)
        {

            string termino = "";
            if (options != null)
            {


                if (options != "{\"data\":{}}" && options != "{\"data\":{\"filter\":{\"filters\":[],\"logic\":\"and\"}}}" && options != "{\"data\":{\"sort\":[{\"field\":\"ASSI\",\"dir\":\"asc\"}]}}" && options != "{\"data\":{\"filter\":{\"filters\":[],\"logic\":\"and\"},\"sort\":[{\"field\":\"ASSI\",\"dir\":\"asc\"}]}}")
                {
                    JObject optionObject = JObject.Parse(options);
                    termino = (string)optionObject["data"]["filter"]["filters"].FirstOrDefault()["value"];

                }
                else { termino = ""; }
            }
            else
            {
                termino = (Request.Params["filter[filters][0][value]"] == null ? "" : Request.Params["filter[filters][0][value]"]);
            }
            int ID_PERS_ENTI_SPELCT = (int)Session["ID_PERS_ENTI"];


            var query = dbe.UsuariosPorCuenta(termino, 0).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        // SUPERVISOR_CUENTA_ELECTRODATA
        public ActionResult ListaCuentaPorUsuario_SPELC()
        {
            int ID_PERS_ENTI = 0;
            string filtro = "";

            if (Convert.ToString(Request.Params["filter[filters][0][field]"]) == "ID_PERS_ENTI")
            {
                ID_PERS_ENTI = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
                filtro = "";
            }
            else
            {
                ID_PERS_ENTI = 0;
                filtro = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            }

            var account = dbc.ACCOUNTs.ToList();

            if (filtro == null)
            {
                filtro = "";
            }

            var query = dbe.CuentaPorUsuario(filtro, 0, ID_PERS_ENTI).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        //SUPERVISOR_CUENTA_ELECTRODATA
        public ActionResult ListaCuentasEditar_SPELC(int id)
        {
            
            int ID_PERS_ENTI = id;

            string filtro = "";

            if (Convert.ToString(Request.Params["filter[filters][0][value]"]) == null)
            {
                filtro = "";
            }
            else
            {
                filtro = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            }


            var query = dbe.EditarCuentasPorUsuario(filtro, 0, ID_PERS_ENTI).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        //SUPERVISOR_CUENTA
        [Authorize, HttpPost]
        public ActionResult ActualizarCuenta()
        {
            string PERS_ENTI = Convert.ToString(Request.Params["ID_PERS_ENTI"]);
            string ACCO_PERS = Convert.ToString(Request.Params["ID_ACCO"]);
            string QUEU = Convert.ToString(Request.Params["ID_QUEU"]);

            if (!String.IsNullOrEmpty(QUEU) && !String.IsNullOrEmpty(PERS_ENTI) && !String.IsNullOrEmpty(ACCO_PERS))
            {
                int ID_QUEU = Convert.ToInt32(Request.Params["ID_QUEU"]);
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
                int ID_ACCO_PERS = Convert.ToInt32(Request.Params["ID_ACCO"]);
                PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI);

                pe.ID_QUEU = ID_QUEU;
                pe.ID_ACCO_PERT = ID_ACCO_PERS;
                dbe.PERSON_ENTITY.Attach(pe);
                dbe.Entry(pe).State = EntityState.Modified;
                dbe.SaveChanges();

                dbe.ActualizarCuentaPorDefecto(ID_PERS_ENTI, ID_ACCO_PERS);

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('OK','Se creó el registro.');}window.onload=init;</script>");

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.mensaje) top.mensaje('ERROR','Ha ocurrido un error, contacte al administrador.');}window.onload=init;</script>");
                //return Content("<script type='text/javascript'> function init() {" +
                //"if(top.PopUpQueue) top.PopUpQueue('ERROR','0');}window.onload=init;</script>");
            }
        }


        //SUPERVISOR_CUENTA
        [HttpPost]
        public ActionResult GuardarRestriccionesCuenta()
        {
            string vr = Convert.ToString(Request.Params["vr"]);
            string va = Convert.ToString(Request.Params["va"]);
            string idaccoenti = Convert.ToString(Request.Params["idaccoenti"]);

            if (!String.IsNullOrEmpty(vr) && !String.IsNullOrEmpty(va) && !String.IsNullOrEmpty(idaccoenti))
            {
                int ID_ACCO_ENTI = Convert.ToInt32(idaccoenti);
                bool visreqe = Convert.ToBoolean(vr);
                bool visass = Convert.ToBoolean(va);
                //bool vistale = Convert.ToBoolean(vt);

                ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);
                ae.VIS_REQU = visreqe;
                ae.VIS_ASSI = visass;

                dbe.ACCOUNT_ENTITY.Attach(ae);
                dbe.Entry(ae).State = EntityState.Modified;
                dbe.SaveChanges();
                return Content("OK");
            }
            else
            {
                return Content("ERROR");
            }
        }

        //SUPERVISOR_CUENTA
        public ActionResult GuardarNuevaCuenta()
        {
            string vr = Convert.ToString(Request.Params["vr"]);
            string va = Convert.ToString(Request.Params["va"]);
            string idacco = Convert.ToString(Request.Params["idacco"]);
            string idpersenti = Convert.ToString(Request.Params["idpersenti"]);

            if (!String.IsNullOrEmpty(vr) && !String.IsNullOrEmpty(va) && !String.IsNullOrEmpty(idacco) && !String.IsNullOrEmpty(idpersenti))
            {
                int ID_ACCO = Convert.ToInt32(idacco);
                int ID_PERS_ENTI = Convert.ToInt32(idpersenti);

                bool visreqe = Convert.ToBoolean(vr);
                bool visass = Convert.ToBoolean(va);

                var query = dbe.ACCOUNT_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.ID_ACCO == ID_ACCO);

                if (query.Count() == 1)
                {
                    int ID_ACCO_ENTI = Convert.ToInt32(query.First().ID_ACCO_ENTI);

                    ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);
                    ae.VIG_ACCO_ENTI = true;
                    ae.DEF_ACCO = false;
                    ae.VIS_REQU = visreqe;
                    ae.VIS_ASSI = visass;

                    dbe.ACCOUNT_ENTITY.Attach(ae);
                    dbe.Entry(ae).State = EntityState.Modified;
                    dbe.SaveChanges();

                    return Content("OK");
                }
                else
                {
                    ACCOUNT_ENTITY ae = new ACCOUNT_ENTITY();
                    ae.ID_ACCO = ID_ACCO;
                    ae.ID_PERS_ENTI = ID_PERS_ENTI;
                    ae.VIG_ACCO_ENTI = true;
                    ae.VIS_REQU = visreqe;
                    ae.VIS_ASSI = visass;

                    dbe.ACCOUNT_ENTITY.Add(ae);
                    dbe.SaveChanges();

                    return Content("OK");
                }
            }
            else
            {
                return Content("ERROR");
            }
        }
        // SUPERVISOR_CUENTA
        public ActionResult UsuarioPorCuenta(string options, string page)
        {
            
            string termino = "";
            if (options!=null) { 
                
                
             if(options != "{\"data\":{}}" && options != "{\"data\":{\"filter\":{\"filters\":[],\"logic\":\"and\"}}}" && options != "{\"data\":{\"sort\":[{\"field\":\"ASSI\",\"dir\":\"asc\"}]}}" && options != "{\"data\":{\"filter\":{\"filters\":[],\"logic\":\"and\"},\"sort\":[{\"field\":\"ASSI\",\"dir\":\"asc\"}]}}")
                {
                JObject optionObject = JObject.Parse(options);
                termino = (string)optionObject["data"]["filter"]["filters"].FirstOrDefault()["value"];

                }
                else { termino = ""; }
            }
            else
            {
                    termino = (Request.Params["filter[filters][0][value]"] == null? "": Request.Params["filter[filters][0][value]"]);
            }
            int ID_PERS_ENTI_SPELCT = (int)Session["ID_PERS_ENTI"];


            var query = dbe.UsuariosPorCuenta(termino,ID_PERS_ENTI_SPELCT).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        //SUPERVISOR CUENTA
        public ActionResult ListaCuentaPorUsuario()
        {
            int ID_PERS_ENTI = 0;
            int ID_PERS_ENTI_SUP = (int)Session["ID_PERS_ENTI"];
            string filtro = "";

            if (Convert.ToString(Request.Params["filter[filters][0][field]"]) == "ID_PERS_ENTI")
            {
                ID_PERS_ENTI = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
                filtro = "";
            }else
            {
                ID_PERS_ENTI = 0;
                filtro = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            }

            var account = dbc.ACCOUNTs.ToList();

            if(filtro == null)
            {
                filtro = "";
            }

            var query = dbe.CuentaPorUsuario(filtro, ID_PERS_ENTI_SUP, ID_PERS_ENTI).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        //SUPERVISOR_CUENTA
        public ActionResult ListaCuentasSupervisor()
        {
            int ID_PERS_ENTI_SUP = (int)Session["ID_PERS_ENTI"];
            string filtro = "";

            if (Convert.ToString(Request.Params["filter[filters][0][value]"]) == null)
            {
                filtro = "";
            }
            else
            {
                filtro = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            }

            var account = dbc.ACCOUNTs.ToList();

            if (filtro == null)
            {
                filtro = "";
            }

            var query = dbe.CuentaPorUsuario(filtro, ID_PERS_ENTI_SUP, 0).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        //SUPERVISOR_CUENTA
        public ActionResult ListaCuentasEditar(int id)
        {
            int ID_PERS_ENTI_SUP = (int)Session["ID_PERS_ENTI"];
            int ID_PERS_ENTI = id;

            string filtro = "";

            if (Convert.ToString(Request.Params["filter[filters][0][value]"]) == null)
            {
                filtro = "";
            }
            else
            {
                filtro = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            }


            var query = dbe.EditarCuentasPorUsuario(filtro, ID_PERS_ENTI_SUP, ID_PERS_ENTI).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        //SUPERVISOR_CUENTA
        public ActionResult ListaGrupoPorCuenta()
        {
            try
            {
                int ID_ACCO = 0;
                string filtro = "";

                if (Convert.ToString(Request.Params["filter[filters][0][field]"]) == "ID_ACCO")
                {
                    ID_ACCO = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
                    filtro = "";
                }
                else
                {
                    ID_ACCO = 0;
                    filtro = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                }
                var result = (from q in dbe.GrupoPorCuenta(filtro,ID_ACCO)
                              select new
                              {
                                  ID_QUEU = q.ID_QUEU,
                                  NAM_QUEU = q.NAM_QUEU,
                                  DES_QUEU = CapitalizeCadena(q.DES_QUEU)
                              }).ToList();

                

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "" }, JsonRequestBehavior.AllowGet);
            }
        }


       
        //SUPERVISOR_CUENTA
        public ActionResult CuentaColaUsuario(int idpersenti)
        {
            try
            {
                var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == idpersenti)
                             select new
                             { 
                             ID_ACCO_PERT = pe.ID_ACCO_PERT,
                             ID_QUEU = pe.ID_QUEU
                             
                             }).ToList();


                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        //SUPERVISOR_CUENTA
        public ActionResult EliminarCuenta()
        {

            string idaccoenti = Convert.ToString(Request.Params["idaccoenti"]);

            if (!String.IsNullOrEmpty(idaccoenti))
            {
                int ID_ACCO_ENTI = Convert.ToInt32(idaccoenti);

                ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);
                ae.VIG_ACCO_ENTI = false;

                dbe.ACCOUNT_ENTITY.Attach(ae);
                dbe.Entry(ae).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("OK");
            }
            else
            {
                return Content("ERROR");
            }
        }

        //SUPERVISOR_CUENTA
        public ActionResult CuentaPorDefecto()
        {

            string idaccoenti = Convert.ToString(Request.Params["idaccoenti"]);
            string idpersenti = Convert.ToString(Request.Params["idpersenti"]);

            if (!String.IsNullOrEmpty(idaccoenti) && !String.IsNullOrEmpty(idpersenti))
            {
                int ID_ACCO_ENTI = Convert.ToInt32(idaccoenti);
                int ID_PERS_ENTI = Convert.ToInt32(idpersenti);

                var aee = dbe.ACCOUNT_ENTITY.Where(x => x.DEF_ACCO == true && x.VIG_ACCO_ENTI == true && x.ID_PERS_ENTI == ID_PERS_ENTI);
                if (aee.Count() > 0)
                {
                    foreach (var accenti in aee)
                    {
                        accenti.DEF_ACCO = false;

                        dbe.ACCOUNT_ENTITY.Attach(accenti);
                        dbe.Entry(accenti).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                }

                ACCOUNT_ENTITY ae = dbe.ACCOUNT_ENTITY.Single(x => x.ID_ACCO_ENTI == ID_ACCO_ENTI);
                ae.DEF_ACCO = true;

                dbe.ACCOUNT_ENTITY.Attach(ae);
                dbe.Entry(ae).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("OK");
            }
            else
            {
                return Content("ERROR");
            }
        }


    }
}
