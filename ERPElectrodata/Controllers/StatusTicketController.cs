using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class StatusTicketController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /StatusTicket/List
        //id === ID_TICK
        public ActionResult List()
        {

            var result = (from s in dbc.STATUS.Where(x => x.VIS_NEW_TICKET == true)
                          where s.VIS_STAT == true
                          select new
                          {
                              s.ID_STAT,
                              s.NAM_STAT
                          });

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarEstadoTicket(string q, string page)
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

            var result = dbc.ListarEstadoTicket(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ListarStatusTicket()
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
            var result = (from x in dbc.ListarEstadoTicket(termino).ToList()
                          select new
                          {
                              opcion = "<option value='" + x.id + "'>" + x.text + "</option>",
                              id = x.id,
                              text = x.text
                          }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ListarEstado(string q, string page)
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
            var result = (from s in dbc.STATUS.Where(x => x.VIS_NEW_TICKET == true)
                          where s.VIS_STAT == true
                          select new
                          {
                              id = s.ID_STAT,
                              text = s.NAM_STAT
                          }).Where(x => x.text.Contains(termino));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarEstadoEdata(string q, string page)
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
            var result = dbc.ListarEstadoEdata(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StatusCondition(int id = 0)
        {
            var result = (from t in dbc.TICKETs.Where(x => x.ID_TICK == id).ToList()
                          join tsc in dbc.TICKET_STATUS_CONDITION on t.ID_STAT_END equals tsc.ID_STAT_FROM
                          join s in dbc.STATUS on tsc.ID_STAT_TO equals s.ID_STAT
                          where tsc.VIG == true
                          select new
                          {
                              ID_STAT = s.ID_STAT,
                              NAM_STAT = s.NAM_STAT
                          });

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StatusConditionEdata(int id = 0)
        {
            var result = dbc.ObtenerEstadosEdata(id).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StatusConditionBnv(int id = 0, string horaRegistrada = "")
        {
            var result = (from t in dbc.TICKETs.Where(x => x.ID_TICK == id).ToList()
                          join tsc in dbc.TICKET_STATUS_CONDITION on t.ID_STAT_END equals tsc.ID_STAT_FROM
                          join s in dbc.STATUS on tsc.ID_STAT_TO equals s.ID_STAT
                          where tsc.VIG == true
                          select new
                          {
                              ID_STAT = s.ID_STAT,
                              NAM_STAT = s.NAM_STAT
                          });
            if (horaRegistrada == "")
            {
                result = result.Where(x => x.ID_STAT != 4);
            }
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        /*Gestión de problemas*/
        public ActionResult StatusConditionProblems(int id = 0)
        {
            var result = (from t in dbc.TICKETs.Where(x => x.ID_TICK == id).ToList()
                          join tsc in dbc.TICKET_STATUS_CONDITION on t.ID_STAT_END equals tsc.ID_STAT_FROM
                          join s in dbc.STATUS on tsc.ID_STAT_TO equals s.ID_STAT
                          where tsc.VIG == true //&& tsc.ID_STAT_TO!=5
                          select new
                          {
                              ID_STAT = s.ID_STAT,
                              NAM_STAT = s.NAM_STAT,
                          });

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        /*Fin*/
        //
        // GET: /StatusTicket/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult StatusMODU()
        {
            var result = (from s in dbc.STATUS.Where(x => x.ID_STAT == 5)
                          where s.VIS_STAT == true
                          select new
                          {
                              s.ID_STAT,
                              s.NAM_STAT
                          });

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTicketStatus()
        {
            var result = dbc.ListarTicketStatus().ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarRazonxEstado(int IDESTADO, int Tipo)
        {
            if (Tipo == 1) // Para vista Crear Ticket BNV
            {
                IDESTADO = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
                //string Nom_Estado = Convert.ToString(Request.Params["filter[filters][1][value]"]);
                var result = dbc.ListarRazonxEstado(IDESTADO).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            if (Tipo == 2) //Editar Ticket BNV
            {
                var result = dbc.ListarRazonxEstado(IDESTADO).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else // Para vista Crear Ticket BNV
            {
                var result = dbc.ListarRazonxEstado(IDESTADO).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }


            //return View();
        }

        public ActionResult ListarRazon(int IDSTAT)
        {
            int IDESTADO = 0;
            IDESTADO = IDSTAT;
            var result = dbc.ListarRazonxEstado(IDESTADO).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

    }
}
