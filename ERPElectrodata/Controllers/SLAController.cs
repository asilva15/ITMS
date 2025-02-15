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
    public class SLAController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        private CoreEntities db = new CoreEntities();

        //
        // GET: /SLA/


        //public ActionResult List()
        //{
        //    var result = (from p in db.PRIORITies.ToList()
        //                  select new
        //                  {
        //                      p.NAM_PRIO,
        //                      p.ID_PRIO,
        //                      p.HOU_PRIO
        //                  });

        //    return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult List()
        {
            int idcuenta = Convert.ToInt32(Session["ID_ACCO"]);

            int idtiket = Convert.ToInt32(Session["ID_TYPE_TICK"]);

            var result = (from s in db.SLAs.ToList()
                          select new
                          {
                              s.Id,
                              s.Nombre,
                              s.IdCuenta

                          }

                          ).Where(x => x.IdCuenta == idcuenta); ;
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerCompaniaxCuenta()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            if (IdAcco != 0)
            {
                String IdEnti = "";
                var query = dbc.ObtenerCompaniaxCuenta(IdAcco).ToList();
                if (query.Count() == 0)
                    IdEnti = "";
                else
                    IdEnti = Convert.ToString(query.SingleOrDefault().IdEnti);
                return Content(IdEnti);
            }
            else
            {
                return Content("");
            }
        }


        public ActionResult ListaSLACategory(int IdTypeTicket, int IdACCO)
        {
            var result = dbc.ListSLACategory(IdTypeTicket, IdACCO).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        // LISTAR CASCADA FILTRTADO POR TIPO DE TIKET Y EMPRESA
        public ActionResult ListarCboSLAxTypeTick()
        {
            int ID_TYPE_TICK = 0;
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_TYPE_TICK")
            {
                ID_TYPE_TICK = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "NAM_TYPE_TICK")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }


            var result = dbc.ListarCboSLAxTypeTick(idAcco, ID_TYPE_TICK).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarSLATicketCategory(int idCat1, int idCat2, int idCat3, int idTypeTicket)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = dbc.UltimoSLACategory(idCat1, idCat2, idCat3, idTypeTicket).ToList();

            if (result.Count() == 0)
            {
                var results = dbc.ListSlaStandarBNV(idTypeTicket, ID_ACCO).ToList();
                return Json(new { Data = results, Count = results.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ListarSLATicket(int idCate)
        {
            int ID_TYPE_TICK = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_TYPE_TICK")
            {
                ID_TYPE_TICK = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }

            var result = dbc.SLATicket(idCate).ToList();

            if (idCate != 0 && (result == null || result.Count == 0))
            {
                var results = dbc.ListSlaStandarBNV(ID_TYPE_TICK, ID_ACCO).ToList();
                return Json(new { Data = results, Count = results.Count() }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }











        public ActionResult Index()
        {

            return View(db.SLAs.ToList());
        }

        public ActionResult IndexMantSLA()
        {
            return View();
        }

        public ActionResult CreaNuevoSLA()
        {
            return View();
        }

        public ActionResult EditarSLA(int Id = 0)
        {
            var pt = (from a in dbc.SLAs.Where(x => x.Id == Id).ToList()
                      join b in dbc.ACCOUNTs on a.IdCuenta equals b.ID_ACCO
                      select new { NOMBRE = a.Nombre, Estado = a.Estado, IdCuenta = a.IdCuenta, Cuenta = b.NAM_ACCO }).Single();
            ViewBag.IDSLA = Id;
            ViewBag.IdCuenta = pt.IdCuenta;
            ViewBag.Cuenta = pt.Cuenta;
            ViewBag.Nombre = pt.NOMBRE.ToString();
            ViewBag.Estado = Convert.ToInt32(pt.Estado);
            return View();
        }



        public ActionResult ListarSLATodo()
        {
            int IdCuenta = 0;
            String cuenta = Request.Params["IdCuenta"].ToString();
            String Nombre = Request.Params["Nombre"].ToString();
            String EstadoSLA = Convert.ToString(Request.Params["EstadoSLA"]);
            if (cuenta != "") { IdCuenta = Convert.ToInt32(cuenta); }
            var result = dbc.ListarBuscarSLA(IdCuenta, Nombre, EstadoSLA).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSLAsCombo()
        {
            var query = (from pr in dbc.SLAs select new { ID_SLA = pr.Id, NOM_SLA = pr.Nombre }).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCrearPrioridadSLA()
        {
            var query = (from pr in dbc.PRIORITies select new { ID_PRIO = pr.ID_PRIO, NAM_PRIO = pr.NAM_PRIO, TIEMPOA = 0, Color = pr.COL_PRIO }).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarEditarPrioridadSLA(int id = 0, int id1 = 0)
        {
            var query = dbc.MostrarPrioridadSLA(id, id1).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearNuevoSLA(FormCollection collection, SLA objsla, SLADetalle objslad)
        {

            string msjError = string.Empty;
            int sw = 0;
            String cuenta = "";
            try
            {
                cuenta = Request.Params["cbCuentaCrea"].ToString();
            }
            catch
            {
                cuenta = "";
            }

            try
            {
                String Nomsla = Request.Params["NombreCrea"].ToString();

                #region validaciones
                if (cuenta == "" || Nomsla == "")
                {
                    msjError = "Debe completar los datos";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                int IdCuenta = Convert.ToInt32(cuenta);
                var val = (from v in dbc.SLAs.Where(x => x.IdCuenta == IdCuenta && x.Nombre == Nomsla) select new { IDSLA = v.Id, NOMBRE = v.Nombre }).ToList();

                if (val.Count() > 0)
                {
                    msjError = "El nombre del SLA ya se encuentra registrada.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                //Validando tabla
                var query = (from pr in dbc.PRIORITies select new { ID_PRIO = pr.ID_PRIO, NAM_PRIO = pr.NAM_PRIO }).ToList();
                int flag = 0;
                foreach (var items in query)
                {
                    if (Convert.ToString(Request.Params["Aplica" + items.ID_PRIO]) == "on")
                    {
                        if (Convert.ToString(Request.Params["Tiempo" + items.ID_PRIO]) == "")
                        {
                            sw = sw + 1;
                        }
                        flag = flag + 1;
                    }
                }
                if (flag == 0)
                {
                    msjError = "Debe seleccionar por lo menos una prioridad.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                if (sw != 0)
                {
                    msjError = "Debe registrar el tiempo de las prioridades seleccionadas.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                if (IdCuenta == 4 && (Request.Params["Aplica5"] != "on" || Request.Params["Tiempo5"] == ""))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + "Debe agregar la prioridad <span style=\"font-weight:bold\">Planificado</span>" + "');}window.onload=init;</script>");
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

                objsla.IdCuenta = IdCuenta;
                objsla.Nombre = Nomsla;
                objsla.FechaCrea = DateTime.Now;
                objsla.UsuarioCrea = IdUser;
                objsla.Estado = true;
                dbc.SLAs.Add(objsla);
                dbc.SaveChanges();

                int Idsla = objsla.Id;
                //RECORRIDO DE LA LISTA 

                #region Detalle de Solicitud

                foreach (var items in query)
                {
                    if (Convert.ToString(Request.Params["Aplica" + items.ID_PRIO]) == "on")
                    {
                        if (Convert.ToString(Request.Params["Tiempo" + items.ID_PRIO]) != "")
                        {
                            objslad.IdSLA = Idsla;
                            objslad.IdPrioridad = items.ID_PRIO;
                            objslad.TiempoAtencion = Convert.ToDecimal(Request.Params["Tiempo" + items.ID_PRIO]);
                            objslad.Estado = true;
                            dbc.SLADetalles.Add(objslad);
                            dbc.SaveChanges();
                        }
                    }
                }
                #endregion

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




        public ActionResult AEUByAcco2()
        {
            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string NAM_PERS = "";
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, i2;
            int ID_COMP = 0;

            if (NAM_PAR == "IdSLA")
            {
                int IdSLA = Convert.ToInt32(Request.QueryString[("filter[filters][0][value]")]);
                ID_COMP = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdSLA).ID_ENTI1.Value;
                NAM_PERS = Convert.ToString(Request.QueryString[("filter[filters][1][value]")]);
            }
            else
            {
                int ID_PERS_ENTI = Convert.ToInt32(Request.QueryString[("filter[filters][1][value]")]);
                ID_COMP = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI1.Value;
                NAM_PERS = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            //NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            //if (NAM_PAR == "ID_PERS_ENTI")
            //{
            //    ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            //}
            //else if (NAM_PAR == "CLIE")
            //{
            //    va = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            //}

            //NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            //if (NAM_PAR == "ID_PERS_ENTI")
            //{
            //    ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            //}
            //else if (NAM_PAR == "CLIE")
            //{
            //    CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            //}

            //var query = dbe.ACCOUNT_ENTITY.Where(ae => ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList();

            if (String.IsNullOrEmpty(NAM_PERS))
            {
                NAM_PERS = "";
            }

            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_COMP).ToList()
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              //CLIE = (p.FIR_NAME + " " + (p.LAS_NAME == null ? "" : p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME)).ToUpper(),
                              CLIE = (p.FIR_NAME + " " + (p.SEC_NAME == null ? "" : p.SEC_NAME) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME)).ToUpper(),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              ID_AREA = (pe.ID_AREA == null ? 0 : pe.ID_AREA)
                          }).ToList().OrderBy(x => x.CLIE).Where(x => x.CLIE.Contains(NAM_PERS.ToUpper()));
            var r = result.ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }













        public ActionResult EdicionSLA(FormCollection collection, SLADetalle objslad)
        {

            string msjError = string.Empty;
            int sw = 0;
            int Idsla = Convert.ToInt32(Request.Params["IDSLA"].ToString());

            String cuenta = "";
            try
            {
                cuenta = Request.Params["cbCuentaE"].ToString();
            }
            catch
            {
                cuenta = "";
            }

            try
            {
                String Nomsla = Request.Params["Nom"].ToString();

                #region validaciones
                if (cuenta == "" || Nomsla == "")
                {
                    msjError = "Debe completar los datos";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                int IdCuenta = Convert.ToInt32(cuenta);
                var val = (from v in dbc.SLAs.Where(x => x.IdCuenta == IdCuenta && x.Nombre == Nomsla && x.Id != Idsla) select new { IDSLA = v.Id, NOMBRE = v.Nombre }).ToList();

                if (val.Count() > 0)
                {
                    msjError = "El nombre del SLA ya se encuentra registrada.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                //Validando tabla
                var query = (from pr in dbc.PRIORITies select new { ID_PRIO = pr.ID_PRIO, NAM_PRIO = pr.NAM_PRIO }).ToList();
                int flag = 0;
                foreach (var items in query)
                {
                    if (Convert.ToString(Request.Params["Aplica" + items.ID_PRIO]) == "on")
                    {
                        if (Convert.ToString(Request.Params["Tiempo" + items.ID_PRIO]) == "")
                        {
                            sw = sw + 1;
                        }
                        flag = flag + 1;
                    }
                }

                if (flag == 0)
                {
                    msjError = "Debe seleccionar por lo menos una prioridad.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                if (sw != 0)
                {
                    msjError = "Debe registrar el tiempo de las prioridades seleccionadas.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                if (IdCuenta == 4 && (Request.Params["Aplica5"] != "on" || Request.Params["Tiempo5"] == ""))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + "Debe agregar la prioridad <span style=\"font-weight:bold\">Planificado</span>" + "');}window.onload=init;</script>");
                }
                #endregion

                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    msjError = "No se encuentra registro de usuario.";
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                //dbc.EditarSLA(Idsla,Nomsla,IdUser,DateTime.Now,objsla.Estado);
                string Estado = Convert.ToString(Request.Params["Estado"]);

                SLA objsla = dbc.SLAs.Find(Idsla);
                objsla.IdCuenta = IdCuenta;
                objsla.Nombre = Nomsla;
                objsla.FechaModifica = DateTime.Now;
                objsla.UsuarioModifica = IdUser;
                if (Estado == "on")
                {
                    objsla.Estado = true;
                }
                else
                {
                    objsla.Estado = false;
                }

                dbc.Entry(objsla).State = EntityState.Modified;
                dbc.SaveChanges();
                dbc.EditarSLA(Idsla, "", 0, DateTime.Now, objsla.Estado);

                //RECORRIDO DE LA LISTA
                #region Detalle de Solicitud
                foreach (var items in query)
                {
                    if (Convert.ToString(Request.Params["Aplica" + items.ID_PRIO]) == "on")
                    {
                        if (Convert.ToString(Request.Params["Tiempo" + items.ID_PRIO]) != "")
                        {
                            objslad.IdSLA = Idsla;
                            objslad.IdPrioridad = items.ID_PRIO;
                            objslad.TiempoAtencion = Convert.ToDecimal(Request.Params["Tiempo" + items.ID_PRIO]);
                            objslad.Estado = true;
                            dbc.SLADetalles.Add(objslad);
                            dbc.SaveChanges();
                        }
                    }
                }
                #endregion
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

        public ActionResult ObtenerPrioridadPorSLA()
        {
            int idSla = Convert.ToInt32(Request.Params["IdSLA"]);
            var query = dbc.ObtenerPrioridadPorSLA(idSla).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ListarCboPrioridadxSLA(int idSLA)
        //{
        //    var result = dbc.ListarCboPrioridadxSLA(idSLA).ToList();
        //    return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        //}


        //PRUEBA DE YECSON
        public ActionResult ListarCboRequerimientosxSLA()
        {
            int idSLA = 0;
            string txt = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "Id")
            {
                idSLA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "Nombre")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "Id")
            {
                idSLA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "Nombre")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }
            var result = dbc.ListarCboPrioridadxSLA(idSLA).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }








        public ActionResult ListarSLAPorCuenta()
        {
            List<ListarBuscarSLA_Result> resultado = dbc.ListarBuscarSLA(4, "", "1").ToList();

            return Json(new { Data = resultado, Count = resultado.Count() }, JsonRequestBehavior.AllowGet);
        }

        public string ActualizarSLA()
        {
            try
            {
                int idProyecto = 0, idSLA = 0;
                if (Request.Params["IdProyecto"] != "") idProyecto = Convert.ToInt32(Request.Params["IdProyecto"]);
                if (Request.Params["IdSLA"] != "") idSLA = Convert.ToInt32(Request.Params["IdSLA"]);
                int idUsuario = Convert.ToInt32(Session["UserId"]);
                dbc.ActualizarSLA(idProyecto, idSLA, idUsuario);
                return "Actualizado";
            }
            catch
            {
                return "Error";
            }
        }

        public string CrearSLA()
        {

            string msjError = string.Empty;
            string nombreSLA = "", cuenta = "", estado = "";
            //Validaciones
            if (Request.Params["cbCuentaSLA"] == "" || Request.Params["cbCuentaSLA"] == null)
                msjError = msjError + "- Seleccione la cuenta.<br/>";
            else
                cuenta = Request.Params["cbCuentaSLA"];
            if (Request.Params["txtNombreSLA"] == "" || Request.Params["txtNombreSLA"] == null)
                msjError = msjError + "- Ingrese el nombre.<br/>";
            else
                nombreSLA = Request.Params["txtNombreSLA"];
            if (Request.Params["cbEstadoSLA"] == "" || Request.Params["cbEstadoSLA"] == null)
                msjError = msjError + "- Seleccione el estado.<br/>";
            else
                estado = Request.Params["cbEstadoSLA"];
            //Validaciones Array Prioridad
            string arrPrioridad = Request.Params["arrayPrioridad"];
            string[] arrPrio = arrPrioridad.Split(',');
            int cont = 0;
            try
            {
                #region validaciones

                int idCuenta = Convert.ToInt32(cuenta);
                var val = (from v in dbc.SLAs.Where(x => x.IdCuenta == idCuenta && x.Nombre == nombreSLA) select new { IDSLA = v.Id, NOMBRE = v.Nombre }).ToList();

                if (val.Count() > 0)
                {
                    msjError = msjError + "- El nombre del SLA ya se encuentra registrado.<br/>";
                }
                int sw = 0;
                int flag = 0;
                foreach (string items in arrPrio)
                {
                    string[] item;
                    item = items.Split('|');
                    if (Convert.ToString(item[0]) == "true")
                    {
                        if (Convert.ToString(item[2]) == "")
                        {
                            sw = sw + 1;
                        }
                        flag = flag + 1;
                    }
                }
                if (flag == 0)
                {
                    msjError = msjError + "- Debe seleccionar por lo menos una prioridad.<br/>";
                }
                if (sw != 0)
                {
                    msjError = msjError + "- Debe registrar el tiempo de las prioridades seleccionadas.<br/>";
                }
                string prio5 = arrPrio[4].Split('|')[0];
                string tiempo5 = arrPrio[4].Split('|')[2];
                if (idCuenta == 4 && (prio5 != "true" || tiempo5 == ""))
                {
                    msjError = msjError + "- Debe agregar la prioridad <span style=\"font-weight:bold\">Planificado</span>";
                }

                #endregion
                if (msjError != string.Empty)
                {
                    return msjError;
                }
                else
                {
                    int IdUser = 0;
                    try
                    {
                        IdUser = Convert.ToInt32(Session["UserId"].ToString());
                    }
                    catch
                    {
                        return "ERROR";
                    }
                    SLA objsla = new SLA();
                    objsla.IdCuenta = idCuenta;
                    objsla.Nombre = nombreSLA;
                    objsla.FechaCrea = DateTime.Now;
                    objsla.UsuarioCrea = IdUser;
                    objsla.Estado = true;
                    dbc.SLAs.Add(objsla);
                    dbc.SaveChanges();

                    int Idsla = objsla.Id;
                    //RECORRIDO DE LA LISTA

                    #region Detalle de Solicitud

                    foreach (var items in arrPrio)
                    {
                        string[] item;
                        item = items.Split('|');
                        if (Convert.ToString(item[0]) == "true")
                        {
                            if (Convert.ToString(item[2]) != "")
                            {
                                SLADetalle objslad = new SLADetalle();
                                objslad.IdSLA = Idsla;
                                objslad.IdPrioridad = Convert.ToInt32(item[1]);
                                objslad.TiempoAtencion = Convert.ToDecimal(item[2]);
                                objslad.Estado = true;
                                dbc.SLADetalles.Add(objslad);
                                dbc.SaveChanges();
                            }
                        }
                    }
                    #endregion

                }
                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        public ActionResult ObtenerSLA(int id = 0)
        {
            var query = dbc.ObtenerDetalleSLA(id).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public string EditarSLAPry()
        {

            string msjError = string.Empty;
            string nombreSLA = "", cuenta = "", estado = "";
            //Validaciones
            if (Request.Params["cbCuentaSLA"] == "" || Request.Params["cbCuentaSLA"] == null)
                msjError = msjError + "- Seleccione la cuenta.<br/>";
            else
                cuenta = Request.Params["cbCuentaSLA"];
            if (Request.Params["txtNombreSLA"] == "" || Request.Params["txtNombreSLA"] == null)
                msjError = msjError + "- Ingrese el nombre.<br/>";
            else
                nombreSLA = Request.Params["txtNombreSLA"];
            if (Request.Params["cbEstadoSLA"] == "" || Request.Params["cbEstadoSLA"] == null)
                msjError = msjError + "- Seleccione el estado.<br/>";
            else
                estado = Request.Params["cbEstadoSLA"];
            //Validaciones Array Prioridad
            string arrPrioridad = Request.Params["arrayPrioridad"];
            string[] arrPrio = arrPrioridad.Split(',');
            int IdSLA = Convert.ToInt32(Request.Params["IdSLA"]);
            try
            {
                #region validaciones

                int idCuenta = Convert.ToInt32(cuenta);
                var val = (from v in dbc.SLAs.Where(x => x.IdCuenta == idCuenta && x.Nombre == nombreSLA && x.Id != IdSLA) select new { IDSLA = v.Id, NOMBRE = v.Nombre }).ToList();

                if (val.Count() > 0)
                {
                    msjError = msjError + "- El nombre del SLA ya se encuentra registrado.<br/>";
                }
                int sw = 0;
                int flag = 0;
                foreach (string items in arrPrio)
                {
                    string[] item;
                    item = items.Split('|');
                    if (Convert.ToString(item[0]) == "true")
                    {
                        if (Convert.ToString(item[2]) == "")
                        {
                            sw = sw + 1;
                        }
                        flag = flag + 1;
                    }
                }
                if (flag == 0)
                {
                    msjError = msjError + "- Debe seleccionar por lo menos una prioridad.<br/>";
                }
                if (sw != 0)
                {
                    msjError = msjError + "- Debe registrar el tiempo de las prioridades seleccionadas.<br/>";
                }
                string prio5 = arrPrio[4].Split('|')[0];
                string tiempo5 = arrPrio[4].Split('|')[2];
                if (idCuenta == 4 && (prio5 != "true" || tiempo5 == ""))
                {
                    msjError = msjError + "- Debe agregar la prioridad <span style=\"font-weight:bold\">Planificado</span>";
                }
                #endregion
                if (msjError != string.Empty)
                {
                    return msjError;
                }
                else
                {
                    int IdUser = 0;
                    try
                    {
                        IdUser = Convert.ToInt32(Session["UserId"].ToString());
                    }
                    catch
                    {
                        return "ERROR";
                    }
                    SLA objsla = dbc.SLAs.Find(IdSLA);
                    objsla.IdCuenta = idCuenta;
                    objsla.Nombre = nombreSLA;
                    objsla.FechaModifica = DateTime.Now;
                    objsla.UsuarioModifica = IdUser;
                    if (estado == "1")
                    {
                        objsla.Estado = true;
                    }
                    else
                    {
                        objsla.Estado = false;
                    }

                    dbc.Entry(objsla).State = EntityState.Modified;
                    dbc.SaveChanges();
                    dbc.EditarSLA(IdSLA, "", 0, DateTime.Now, objsla.Estado);

                    //RECORRIDO DE LA LISTA

                    #region Detalle de Solicitud
                    foreach (var items in arrPrio)
                    {
                        string[] item;
                        item = items.Split('|');
                        if (Convert.ToString(item[0]) == "true")
                        {
                            if (Convert.ToString(item[2]) != "")
                            {
                                SLADetalle objslad = new SLADetalle();
                                objslad.IdSLA = IdSLA;
                                objslad.IdPrioridad = Convert.ToInt32(item[1]);
                                objslad.TiempoAtencion = Convert.ToDecimal(item[2]);
                                objslad.Estado = true;
                                dbc.SLADetalles.Add(objslad);
                                dbc.SaveChanges();
                            }
                        }
                    }
                    #endregion
                }
                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }
        public ActionResult ObtenerSLAPorProyecto(int id = 0)
        {
            int idSLA;
            try
            {
                idSLA = Convert.ToInt32(dbc.PROYECTO_SLA.Single(x => x.IdProyecto == id && x.Estado == true).IdSLA);
            }
            catch (Exception ex)
            {
                idSLA = 0;
            }

            var query = (from a in dbc.PROYECTO_SLA.Where(x => x.IdProyecto == id && x.Estado == true).ToList()
                         join b in dbc.SLAs on a.IdSLA equals b.Id
                         select new { idSLA = a.IdSLA, nombreSLA = b.Nombre }).ToList();
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public string ValidarEdicionSLA()
        {
            try
            {
                int idSLA = 0, idDocuSale = 0;
                if (Request.Params["IdSLA"] != "") idSLA = Convert.ToInt32(Request.Params["IdSLA"]);
                if (Request.Params["IdDocuSale"] != "") idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                int idUsuario = Convert.ToInt32(Session["UserId"]);
                var mensaje = dbc.ValidarEdicionSLA(idSLA, idDocuSale).Single().Mensaje;
                return mensaje;
            }
            catch
            {
                return "Error";
            }
        }


        public ActionResult ListarCboPrioridadxSLABNV(int MacroServicio, int Categoria1, int Categoria2, int TypeTicket, int idGrupo, int idSLA)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "Id")
            {
                idSLA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "Nombre")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "Id")
            {
                idSLA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "Nombre")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }


            //if (ID_ACCO == 60 && idGrupo == 82)
            //{
            //    var idSLAsResult = dbc.SlaBNV(MacroServicio, Categoria1, Categoria2, TypeTicket).ToList();


            //    if (idSLAsResult != null && idSLAsResult.Any())
            //    {
            //        idSLA = idSLAsResult.FirstOrDefault() ?? 0;
            //    }

            //}
            if (ID_ACCO == 60 && (idGrupo == 82 || idGrupo == 83 || idGrupo == 103 || idGrupo == 88))
            {
                var idSLAsResult = dbc.SlaBNV(MacroServicio, Categoria1, Categoria2, TypeTicket).ToList();


                if (idSLAsResult != null && idSLAsResult.Any())
                {
                    idSLA = idSLAsResult.FirstOrDefault() ?? 0;
                }

            }

            var result = dbc.ListarCboPrioridadxSLA(idSLA).ToList();

            var texto = Convert.ToString(Request.QueryString["text"]);
            if (texto != "" && texto != null)
            {
                var resultadoFiltrado = result.Where(text => text.Prioridad.Contains(texto.ToUpper())).ToList();
                return Json(new { Data = resultadoFiltrado, Count = resultadoFiltrado.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult ListarCboPrioridadxSLA()
        {
            int idSLA = 0;
            string txt = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "Id")
            {
                idSLA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "Nombre")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "Id")
            {
                idSLA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "Nombre")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }
            var result = dbc.ListarCboPrioridadxSLA(idSLA).ToList();
            var texto = Convert.ToString(Request.QueryString["text"]);
            if (texto != "" && texto != null)
            {
                var resultadoFiltrado = result.Where(text => text.Prioridad.Contains(texto.ToUpper())).ToList();
                return Json(new { Data = resultadoFiltrado, Count = resultadoFiltrado.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }

        }

        //CAMBIOS SLA PROYECTOS
        public ActionResult ListarCrearPrioridadSLAEdata()
        {
            var query = dbc.ListarPrioridadesEdata().ToList();

            //(from pr in dbc.PRIORITies select new { ID_PRIO = pr.ID_PRIO, NAM_PRIO = pr.NAM_PRIO, TIEMPOA = 0, Color = pr.COL_PRIO }).ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }
    }
}
