using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class PlantillaBuenaventuraController : Controller
    {
        public CoreEntities dbc = new CoreEntities();

        #region Vistas

        public ActionResult Principal()
        {
            return View();
        }
        public ActionResult Nuevo()
        {
            return View();
        }
        public ActionResult Editar(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        #endregion

        #region CRUD
        [HttpGet]
        public ActionResult ObtenerPlantilla(int IdPlantilla)
        {
            var response = dbc.ObtenerPlantillaBuenaventura(IdPlantilla);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ListarPlantillas(int? IdGrupo, int? IdTipoTicket, int? IdPrioridad)
        {
            try
            {
                var response = dbc.LeerPlantilla(IdGrupo, IdTipoTicket, IdPrioridad);
                return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                LeerPlantilla_Result response = new LeerPlantilla_Result();
                return Json(new { data = response }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CrearPlantilla(string IdGrupo, string nombrePlantilla,PlantillaBuenaventuraDTO ticket)
        {
            try
            {
                var plantillaObj = new Plantilla1();
                plantillaObj.Nombre = nombrePlantilla;
                plantillaObj.IdGrupo = Convert.ToInt32(IdGrupo);
                plantillaObj.FechaCreacion = DateTime.Now;
                plantillaObj.UsuarioCreacion = Convert.ToInt32(Session["UserId"]);
                plantillaObj.Estado = true;

                StringBuilder strBuilder = new StringBuilder();
                strBuilder
                    .Append("TITLE_TICK=" + ticket.TITLE_TICK + "|")
                    .Append("ID_TYPE_TICK=" + ticket.ID_TYPE_TICK + "|")
                    .Append("ID_PRIO=" + ticket.ID_PRIO + "|")
                    .Append("ID_SOUR=" + ticket.ID_SOUR + "|")
                    .Append("ID_STAT=" + ticket.ID_STAT + "|")
                    .Append("FEC_INI_TICK=" + ticket.FEC_INI_TICK + "|")
                    .Append("FechaRecepcionSolicitud=" + ticket.FechaRecepcionSolicitud + "|")
                    .Append("IdRazon=" + ticket.IdRazon + "|")
                    .Append("TicketExterno=" + ticket.TicketExterno + "|")
                    .Append("ID_PERS_ENTI=" + ticket.ID_PERS_ENTI + "|")
                    .Append("ID_COMP=" + ticket.ID_COMP + "|")
                    .Append("ID_PERS_ENTI_END=" + ticket.ID_PERS_ENTI_END + "|")
                    .Append("ID_QUEU=" + ticket.ID_QUEU + "|")
                    .Append("ID_PERS_ENTI_ASSI=" + ticket.ID_PERS_ENTI_ASSI + "|")
                    .Append("IdSLA=" + ticket.IdSLA + "|")
                    .Append("ID_CATE_N1=" + ticket.ID_CATE_N1 + "|")
                    .Append("ID_CATE_N2=" + ticket.ID_CATE_N2 + "|")
                    .Append("ID_CATE_N3=" + ticket.ID_CATE_N3 + "|")
                    .Append("ID_CATE=" + ticket.ID_CATE + "|")
                    .Append("REM_CTRL_TICK=" + ticket.REM_CTRL_TICK + "|")
                    .Append("IS_PARENT=" + ticket.IS_PARENT + "|")
                    .Append("ID_TICK_PARENT=" + ticket.ID_TICK_PARENT + "|")
                    .Append("IdGrupoReportador=" + ticket.IdGrupoReportador + "|")
                    .Append("IdModTrabajo=" + ticket.IdModTrabajo + "|")
                    .Append("ID_LOCA=" + ticket.ID_LOCA + "|")
                    .Append("ID_PERS_ENTI_ANLT=" + ticket.ID_PERS_ENTI_ANLT + "|")
                    .Append("IdTipoPase=" + ticket.IdTipoPase + "|")
                    .Append("Solman=" + ticket.Solman + "|")
                    .Append("SERVICE=" + ticket.SERVICE + "|")
                    .Append("FEC_INI_REAL=" + ticket.FEC_INI_REAL + "|")
                    .Append("SUM_TICK=" + ticket.SUM_TICK + "|")
                    .Append("Solucion=" + ticket.Solucion)
                    ;
                plantillaObj.Contenido = strBuilder.ToString();

                dbc.Plantilla1Set.Add(plantillaObj);
                dbc.SaveChanges();
                return Json(new { code = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = "ERROR" }, JsonRequestBehavior.AllowGet);
            }
            
        }
        [HttpPost]
        public ActionResult EditarPlantilla(PlantillaBuenaventuraDTO plantilla)
        {
            try
            {
                var plantillaObj = dbc.Plantilla1Set.Find(plantilla.Id);

                plantillaObj.Nombre = plantilla.Nombre;
                plantillaObj.IdGrupo = plantilla.IdGrupo;
                plantillaObj.FechaModifica = DateTime.Now;
                plantillaObj.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
                plantillaObj.Estado = true;

                StringBuilder strBuilder = new StringBuilder();
                strBuilder
                    .Append("TITLE_TICK=" + plantilla.TITLE_TICK + "|")
                    .Append("ID_TYPE_TICK=" + plantilla.ID_TYPE_TICK + "|")
                    .Append("ID_PRIO=" + plantilla.ID_PRIO + "|")
                    .Append("ID_SOUR=" + plantilla.ID_SOUR + "|")
                    .Append("ID_STAT=" + plantilla.ID_STAT + "|")
                    .Append("FEC_INI_TICK=" + plantilla.FEC_INI_TICK + "|")
                    .Append("FechaRecepcionSolicitud=" + plantilla.FechaRecepcionSolicitud + "|")
                    .Append("IdRazon=" + plantilla.IdRazon + "|")
                    .Append("TicketExterno=" + plantilla.TicketExterno + "|")
                    .Append("ID_PERS_ENTI=" + plantilla.ID_PERS_ENTI + "|")
                    .Append("ID_COMP=" + plantilla.ID_COMP + "|")
                    .Append("ID_PERS_ENTI_END=" + plantilla.ID_PERS_ENTI_END + "|")
                    .Append("ID_QUEU=" + plantilla.ID_QUEU + "|")
                    .Append("ID_PERS_ENTI_ASSI=" + plantilla.ID_PERS_ENTI_ASSI + "|")
                    .Append("IdSLA=" + plantilla.IdSLA + "|")
                    .Append("ID_CATE_N1=" + plantilla.ID_CATE_N1 + "|")
                    .Append("ID_CATE_N2=" + plantilla.ID_CATE_N2 + "|")
                    .Append("ID_CATE_N3=" + plantilla.ID_CATE_N3 + "|")
                    .Append("ID_CATE=" + plantilla.ID_CATE + "|")
                    .Append("REM_CTRL_TICK=" + plantilla.REM_CTRL_TICK + "|")
                    .Append("IS_PARENT=" + plantilla.IS_PARENT + "|")
                    .Append("ID_TICK_PARENT=" + plantilla.ID_TICK_PARENT + "|")
                    .Append("IdGrupoReportador=" + plantilla.IdGrupoReportador + "|")
                    .Append("IdModTrabajo=" + plantilla.IdModTrabajo + "|")
                    .Append("ID_LOCA=" + plantilla.ID_LOCA + "|")
                    .Append("ID_PERS_ENTI_ANLT=" + plantilla.ID_PERS_ENTI_ANLT + "|")
                    .Append("IdTipoPase=" + plantilla.IdTipoPase + "|")
                    .Append("Solman=" + plantilla.Solman + "|")
                    .Append("SERVICE=" + plantilla.SERVICE + "|")
                    .Append("FEC_INI_REAL=" + plantilla.FEC_INI_REAL + "|")
                    .Append("SUM_TICK=" + plantilla.SUM_TICK + "|")
                    .Append("Solucion=" + plantilla.Solucion)
                    ;
                plantillaObj.Contenido = strBuilder.ToString();

                dbc.Entry(plantillaObj).State = EntityState.Modified;
                dbc.SaveChanges();
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
                var plantillaObj = dbc.Plantilla1Set.Find(Id);

                plantillaObj.FechaModifica = DateTime.Now;
                plantillaObj.UsuarioModifica = Convert.ToInt32(Session["UserId"]);
                plantillaObj.Estado = false;
                dbc.Entry(plantillaObj).State = EntityState.Modified;
                dbc.SaveChanges();
                return Json(new { code = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = "ERROR" }, JsonRequestBehavior.AllowGet);
            }
            
        }

        #endregion


        #region Listados adicionales
        public JsonResult ListarGrupos()
        {
            var response = dbc.ListarGrupoPlantilla().ToList();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarPlantillasGrupo(int IdGrupo)
        {
            var response = dbc.ListarPlantillasxGrupo(IdGrupo).ToList();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public JsonResult ListarGruposUsuario()
        {
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

            //Caso de Monitoreo
            if(ID_QUEU == 102)
            {
                var areasMonitoreo = new int[] { 91, 93, 102 };

                var result = (from q in dbc.QUEUEs
                              where areasMonitoreo.Contains(q.ID_QUEU) && q.VIG_QUEU == true
                              select new
                              {
                                  ID_QUEU = q.ID_QUEU,
                                  DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToUpper())
                              }).OrderBy(x => x.DES_QUEU);
                return Json(new { Data = result}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from q in dbc.QUEUEs
                              where q.ID_QUEU == ID_QUEU && q.VIG_QUEU == true
                              select new
                              {
                                  ID_QUEU = q.ID_QUEU,
                                  DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToUpper())
                              }).OrderBy(x => x.DES_QUEU);
                return Json(new { Data = result}, JsonRequestBehavior.AllowGet);
            }

        }

    }
}
