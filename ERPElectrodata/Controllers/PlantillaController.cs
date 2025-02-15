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
    public class PlantillaController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexMantPlantilla() 
        {

            return View();
        }

        public ActionResult CrearPlantilla()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearNuevaPlantilla(FormCollection collection, PLANTILLA objplantilla)
        {
            string msjError = string.Empty;
            String cuenta = "";
            String grupo = "";
            String tipoplan = "";
            String Nomplantilla = ""; 
            String descrip = "";

            try
            {
                cuenta = Request.Params["cbCuentaCrea"].ToString();
                grupo = Request.Params["cbGrupoCrea"].ToString();
                tipoplan = Request.Params["cbTipoPlantillaCrea"].ToString();
                Nomplantilla = Request.Params["PlantillaCrea"].ToString();
                descrip = Request.Params["DESCRIPCION"].ToString();//objplantilla.DESCRIPCION; 
            }
            catch
            {
                cuenta = "";
                grupo = "";
                tipoplan = "";
                Nomplantilla = "";
                descrip = "";
            }

            try
            {
                //String Nomplantilla = Request.Params["PlantillaCrea"].ToString();

                #region validaciones
                if (cuenta == "" || grupo == "" || tipoplan == "" || Nomplantilla == "" || descrip == "")
                {
                    msjError = "Debe completar los datos";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                int IdCuenta = Convert.ToInt32(cuenta);
                int Idgrupo = Convert.ToInt32(grupo);
                int Idtipoplan = Convert.ToInt32(tipoplan);

                var val = (from a in dbc.PLANTILLA.Where(x => x.NOMBRE == Nomplantilla && x.ID_GRUPO == Idgrupo)
                            join b in dbc.GRUPOPLANTILLA on a.ID_GRUPO equals b.ID_GRUPO
                            join c in dbc.ACCOUNTs on b.IdCuenta equals c.ID_ACCO
                            select new { IDPLANTILLA = a.ID_PLAN, NOMBRE = a.NOMBRE, IdCuenta =  c.ID_ACCO, Idgrupo = b.ID_GRUPO}).ToList();


                if (val.Count() > 0)
                {
                        msjError = "El nombre de la PLANTILLA ya se encuentra registrada.";
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


                objplantilla.ID_GRUPO = Idgrupo;
                objplantilla.ID_TIPO_PLAN = Idtipoplan;
                objplantilla.NOMBRE = Nomplantilla;
                objplantilla.DESCRIPCION = descrip;
                objplantilla.USUARIOCREACION = IdUser;
                objplantilla.FECHACREACION = DateTime.Now;
                objplantilla.ESTADO = true;
                dbc.PLANTILLA.Add(objplantilla);
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

        public ActionResult EditarPlantilla(int id)
        {

            PLANTILLA objPlantilla = dbc.PLANTILLA.Single(x => x.ID_PLAN == id);

            var data = (from a in dbc.PLANTILLA.Where(x => x.ID_PLAN == id).ToList()
                        join b in dbc.GRUPOPLANTILLA on a.ID_GRUPO equals b.ID_GRUPO
                        join c in dbc.TIPOPLANTILLA on a.ID_TIPO_PLAN equals c.ID
                        join d in dbc.ACCOUNTs on b.IdCuenta equals d.ID_ACCO
                        select new { Id_Cuenta = b.IdCuenta, Cuenta = d.NAM_ACCO, Idgrupo = a.ID_GRUPO, Grupo = b.NOMBRE, 
                            Idtipoplan = a.ID_TIPO_PLAN, TipoPlan = c.NOMBRE, Nombre = a.NOMBRE, Descripcion = a.DESCRIPCION, 
                            Estado = a.ESTADO, EstadoTipPlan=c.ESTADO }).Single();


                ViewBag.IdPlantilla = id;
                ViewBag.IdGrupo = data.Idgrupo;
                ViewBag.NomGrupo = data.Grupo;
                ViewBag.IdCuenta = data.Id_Cuenta;
                ViewBag.Cuenta = data.Cuenta;
                ViewBag.IdTipoPlan = data.Idtipoplan;
                ViewBag.TipoPlan = data.TipoPlan;
                ViewBag.Plantilla = data.Nombre;
                ViewBag.Descripcion = data.Descripcion;

                ViewBag.Estado = Convert.ToInt32(data.Estado);



            return View(objPlantilla);


            /* if (objPlantilla == null) { return HttpNotFound(); }*/
            /*  if (data.EstadoTipPlan == false)
  {
      return JavaScript(activMsj());
      //TempData["msgtp"] = "<script>'Tipo de Plantilla Inactiva'</script>";
  }*/

        }
        public ActionResult ListarPlantillaTodo()
        {
            int IdCuenta = 0;
            int IdGrupo = 0;
            int IdTipoPlan = 0;
            String cuenta = Request.Params["IdCuenta"].ToString();
            String grupo = Request.Params["IdGrupo"].ToString();
            String TipoPlan = Request.Params["TipoPlantilla"].ToString();

            if (cuenta != "") { IdCuenta = Convert.ToInt32(cuenta); }
            if (grupo != "") { IdGrupo = Convert.ToInt32(grupo); }
            if (TipoPlan != "") { IdTipoPlan = Convert.ToInt32(TipoPlan); }

            var result = dbc.ListarBuscarPlantilla(IdCuenta, IdGrupo, IdTipoPlan).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarPlantillaCombo()
        {
            var query = (from at in dbc.PLANTILLA select new { at.ID_PLAN, at.NOMBRE }).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarGruposCuenta() //plantilla y ticket
        {
            string txt = "";
            int id = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (txt == null) { txt = ""; }

            var query = dbc.ListarGrupoCuenta(id, txt).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

        }


 /*       
        public ActionResult ListarGrupoCombo() //Editar Ticket 
        {
      
            //var query = (from gp in dbc.GRUPOPLANTILLA select new { gp.ID_GRUPO, gp.NOMBRE }).ToList();
            //return Json(new { Data = query }, JsonRequestBehavior.AllowGet);

        }
  */      

        public ActionResult ListarTipoPlantillaCombo(int estd = 0) //no habia nada puse en int
        {

            var query = (from tp in dbc.TIPOPLANTILLA.Where(x =>x.ESTADO == true) select new { tp.ID, tp.NOMBRE }).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            /*
            var query = (from tp in dbc.TIPOPLANTILLA select new { tp.ID, tp.NOMBRE }).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            */
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EdicionPlantilla(FormCollection collection, PLANTILLA objplant)
        
        {
            string msjError = string.Empty;
            String cuenta = "";
            String plantilla = "";
            String tipoplan = "";
            String grupo = "";
            String Descplan = "";

            int Idplan = Convert.ToInt32(Request.Params["IDPlantilla"]);
            int Idgrupo = Convert.ToInt32(Request.Params["GrupoE"]);

            if (Idgrupo ==0) {
                msjError = "Debe completar los datos";
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
            }

            GRUPOPLANTILLA objgroup = dbc.GRUPOPLANTILLA.Where(x => x.ID_GRUPO == Idgrupo).Single();
             
            int IdCuenta = Convert.ToInt32(objgroup.IdCuenta); //Convert.ToInt32(Request.Params["cbCuentaE"]);                    
            int Idtipoplan = Convert.ToInt32(Request.Params["cbTipoPlantillaE"]);

            /*
            int IdCuenta = Convert.ToInt32(Request.Params["cbCuentaE"].ToString());
            int Idplan = Convert.ToInt32(Request.Params["PlantillaE"].ToString());
            int Idgrupo = Convert.ToInt32(Request.Params["GrupoE"].ToString());
            int Idtipoplan = Convert.ToInt32(Request.Params["cbTipoPlantillaE"].ToString());
            */                      
           
            try
            {
                cuenta = Convert.ToString(objgroup.IdCuenta);//Request.Params["cbCuentaE"].ToString(); // 
                plantilla = Request.Params["PlantillaE"].ToString();
                tipoplan = Request.Params["cbTipoPlantillaE"].ToString();
                grupo = Request.Params["GrupoE"].ToString();
                Descplan = Request.Params["DESCRIPCION"].ToString();//Descplan = Request.Params["TextareaPlanEdita"].ToString();
            }
            catch
            {
                cuenta = "";
                plantilla = "";
                tipoplan = "";
                grupo = "";
                Descplan = "";

            }
;
             //String Descplan =  Request.Params["TextareaPlanEdita"].ToString();

            try
            {
                String Nomplan = Request.Params["PlantillaE"].ToString();

              
                if (cuenta == "" || plantilla == "" || grupo == "" || tipoplan == "" || Descplan == "" /*|| Descplan == null*/)
                {
                    msjError = "Debe completar los datos";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                var val = (from gp in dbc.PLANTILLA.Where(x => x.ID_PLAN != Idplan && x.NOMBRE == Nomplan && x.ID_GRUPO == Idgrupo && x.ID_TIPO_PLAN == Idtipoplan)
                           join b in dbc.GRUPOPLANTILLA on gp.ID_GRUPO equals b.ID_GRUPO
                           join c in dbc.ACCOUNTs on b.IdCuenta equals c.ID_ACCO
                           select new { idplantilla = gp.ID_PLAN, Idgrupo = gp.ID_GRUPO, idtipoplant= gp.ID_TIPO_PLAN, idcuenta = b.IdCuenta, Nomplan = gp.NOMBRE , Descplan = gp.DESCRIPCION}).ToList();


                if (val.Count() > 0)
                {
                    msjError = "El nombre de la PLANTILLA ya se encuentra registrada.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }               

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

                PLANTILLA objplantilla = dbc.PLANTILLA.Find(Idplan);
                objplantilla.NOMBRE = Nomplan;
                objplantilla.ID_GRUPO = Idgrupo;
                objplantilla.ID_TIPO_PLAN = Idtipoplan;
                objplantilla.DESCRIPCION = Descplan;
                objplantilla.FECHAMODIFICA = DateTime.Now;
                objplantilla.USUARIOMODIFICA = IdUser;
                if (Estado == "on")
                {
                    objplantilla.ESTADO = true;
                }
                else
                {
                    objplantilla.ESTADO = false;
                }

                dbc.Entry(objplantilla).State = EntityState.Modified;
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

        public ActionResult ListarPlantillaGrupo(int ID_TIPOPLAN = 0) //Lista para Ticket //int ID_ACCO = 0
        {            
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            string NOMBRE = "";

            int ID_GRUPO = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            NOMBRE = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (NOMBRE == null) { NOMBRE = ""; }

            if(ID_TIPOPLAN == 1)
            {
                var result = dbc.ListarPlantillaGrupoCuenta(ID_ACCO, ID_GRUPO, NOMBRE, ID_TIPOPLAN, "1", "1").ToList(); //Creacion
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }

            else if (ID_TIPOPLAN == 2)
            {
                var result = dbc.ListarPlantillaGrupoCuenta(ID_ACCO, ID_GRUPO, NOMBRE, ID_TIPOPLAN, "1", "1").ToList(); //Comentario
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            return View();

            /*
            string NOMBRE = "";

            int ID_GRUPO = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            NOMBRE = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (NOMBRE == null) { NOMBRE = ""; }

            var result = dbc.ListarPlantillaGrupoCuenta(ID_ACCO, ID_GRUPO, NOMBRE, 1,"1","1").ToList(); //Creacion
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
           */
        }

        [Authorize]
        public ActionResult ObtenerDescripcionPlantilla()
        {           
            int plan = 0;
            int grupo = 0;
            String descripcion = "";

            grupo = Convert.ToInt32(Request.Params["IdGrupo"]);
            plan = Convert.ToInt32(Request.Params["IdPlan"]);

            var result = dbc.ObtenerPlantilla(plan, grupo).Single();
            return Content(result);

            /*
                   int plan = 0;
                   int grupo = 0;
                   //String descrip = 0;

                   grupo = Convert.ToInt32(Request.Params["IdGrupo"]);
                   plan = Convert.ToInt32(Request.Params["IdPlan"]);

                 var result = (from a in dbc.PLANTILLA.Where(x => x.ID_PLAN == plan && x.ID_GRUPO== grupo).ToList()
                                         select new { Descripcion = a.DESCRIPCION }).Single();
            
            //var result = dbc.ObtenerPlantilla(plan, grupo).ToList();
            //return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            */
        }

        [Authorize]
        public ActionResult Principal()
        {
            if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1 || Convert.ToInt32((Session["SUPERVISOR_SERVICEDESK"].ToString())) == 1)
            {
                return View();
            }
            return RedirectToAction("Principal", "Home");
        }
        [Authorize]
        public ActionResult Nuevo()
        {
            if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1 || Convert.ToInt32((Session["SUPERVISOR_SERVICEDESK"].ToString())) == 1)
            {
                return View();
            }
            return RedirectToAction("Principal", "Home");
        }
        [Authorize]
        public ActionResult Editar(int id)
        {
            if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1 || Convert.ToInt32((Session["SUPERVISOR_SERVICEDESK"].ToString())) == 1)
            {
                ViewBag.Id = id;
                return View();
            }
            return RedirectToAction("Principal", "Home");
        }


        [HttpGet]
        public ActionResult ObtenerPlantilla(int IdPlantilla)
        {
            var response = dbc.PlantillaMinsur.Find(IdPlantilla);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ListarPlantillas(int? IdGrupo, int? IdTipoTicket, int? IdPrioridad)
        {
            try
            {
                var response = dbc.LeerPlantillasMinsur2(IdGrupo, IdTipoTicket, IdPrioridad, Convert.ToInt32(Session["ID_ACCO"]));
                return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LeerPlantillasMinsur2_Result response = new LeerPlantillasMinsur2_Result();
                return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CrearPlantilla(string nombrePlantilla, PlantillaMinsur plantilla)
        {
            try
            {
                dbc.CrearPlantilla(nombrePlantilla, Convert.ToInt32(Session["UserId"]), plantilla.TITLE_TICK, plantilla.ID_CATE_N1, plantilla.ID_CATE_N2, plantilla.ID_CATE, plantilla.ID_TYPE_TICK, plantilla.ID_PRIO, plantilla.ID_STAT, plantilla.ID_QUEU, plantilla.SUM_TICK, plantilla.FEC_INI_TICK, plantilla.REM_CTRL_TICK, plantilla.IS_PARENT, plantilla.ID_TICK_PARENT);
                return Json(new { code = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = "ERROR" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditarPlantilla(PlantillaMinsur plantilla)
        {
            try
            {
                var plantillaObj = dbc.PlantillaMinsur.Find(plantilla.Id);

                if (plantillaObj.IdGrupal == null)
                {
                    plantillaObj.Nombre = plantilla.Nombre;
                    plantillaObj.FechaModifica = DateTime.Now;
                    plantillaObj.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
                    plantillaObj.Estado = true;

                    plantillaObj.TITLE_TICK = plantilla.TITLE_TICK;
                    plantillaObj.ID_TYPE_TICK = plantilla.ID_TYPE_TICK;
                    plantillaObj.ID_PRIO = plantilla.ID_PRIO;
                    plantillaObj.ID_STAT = plantilla.ID_STAT;
                    plantillaObj.FEC_INI_TICK = plantilla.FEC_INI_TICK;
                    plantillaObj.ID_QUEU = plantilla.ID_QUEU;
                    plantillaObj.ID_CATE_N1 = plantilla.ID_CATE_N1;
                    plantillaObj.ID_CATE_N2 = plantilla.ID_CATE_N2;
                    plantillaObj.ID_CATE = plantilla.ID_CATE;
                    plantillaObj.REM_CTRL_TICK = plantilla.REM_CTRL_TICK;
                    plantillaObj.IS_PARENT = plantilla.IS_PARENT;
                    plantillaObj.ID_TICK_PARENT = plantilla.ID_TICK_PARENT;
                    plantillaObj.SUM_TICK = plantilla.SUM_TICK;

                    dbc.Entry(plantillaObj).State = EntityState.Modified;
                    dbc.SaveChanges();
                }
                else
                {
                    dbc.EditarPlantilla(plantilla.Nombre, plantillaObj.IdGrupal, Convert.ToInt32(Session["UserId"]), plantilla.TITLE_TICK, plantilla.ID_CATE_N1, plantilla.ID_CATE_N2, plantilla.ID_CATE, plantilla.ID_TYPE_TICK, plantilla.ID_PRIO, plantilla.ID_STAT, plantilla.ID_QUEU, plantilla.SUM_TICK, plantilla.FEC_INI_TICK, plantilla.REM_CTRL_TICK, plantilla.IS_PARENT, plantilla.ID_TICK_PARENT);
                }

                
                return Json(new { code = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = "ERROR" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult EliminarPlantilla(int Id)
        {
            try
            {
                var plantilla = dbc.PlantillaMinsur.Find(Id);

                plantilla.FechaModifica = DateTime.Now;
                plantilla.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
                plantilla.Estado = false;
                dbc.Entry(plantilla).State = EntityState.Modified;
                dbc.SaveChanges();
                return Json(new { code = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = "ERROR" }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult ObtenerCategorias(int? idCat)
        {
            var listaCategorias = dbc.ObtenerCadenaCategoria(idCat).ToList();

            return Json(listaCategorias, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarGrupos()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var response = dbc.ListarGrupoPlantillaMinsur(ID_ACCO).ToList();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPlantillasGrupo()
        {
            var response = dbc.ListarPlantillas(Convert.ToInt32(Session["ID_ACCO"])).ToList();
            return Json(new { Data = response }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarGruposUsuario()
        {
            if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1 || Convert.ToInt32((Session["SUPERVISOR_SERVICEDESK"].ToString())) == 1)
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var resultA = (from aq in dbc.ACCOUNT_QUEUE
                              join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                              where aq.ID_ACCO == ID_ACCO
                              select new
                              {
                                  ID_QUEU = q.ID_QUEU,
                                  DES_QUEU = (q.NAM_QUEU_REPO == null ? "" : q.NAM_QUEU_REPO.ToUpper())
                              }).OrderBy(x => x.DES_QUEU);
                return Json(new { Data = resultA }, JsonRequestBehavior.AllowGet);
            }

            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

            var result = (from q in dbc.QUEUEs
                          where q.ID_QUEU == ID_QUEU
                          select new
                          {
                              ID_QUEU = q.ID_QUEU,
                              DES_QUEU = (q.NAM_QUEU_REPO == null ? "" : q.NAM_QUEU_REPO.ToUpper())
                          }).OrderBy(x => x.DES_QUEU);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

    }
}
