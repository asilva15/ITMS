using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ConditionController : Controller
    {
        public CoreEntities dbc = new CoreEntities();

        //
        // GET: /Condition/ListStatCond
        public ActionResult ListStatCond()
        {

            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            var query = dbc.CONDITIONs.Where(c => c.VIG_COND == true);

            if (!String.IsNullOrEmpty(FIELD))
            {
                query = query.Where(c => c.NAM_COND.Contains(FIELD.ToUpper()));
            }

            var result = (from c in query.ToList()
                          join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                          select new
                          {
                              c.ID_COND,
                              c.NAM_COND,
                              s.NAM_STAT_ASSE,
                              s.ID_STAT_ASSE,
                              TEXT = s.NAM_STAT_ASSE + " - " + c.NAM_COND
                          }).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List()
        {
            int ID_STAT_ASSE;
            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);
            if (FIELD == "ID_STAT_ASSE")
            {
                ID_STAT_ASSE = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }
            else
            {
                ID_STAT_ASSE = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
            }

            var query = dbc.CONDITIONs.Where(c => c.ID_STAT_ASSE == ID_STAT_ASSE);

            var result = (from c in query.ToList()
                          select new
                          {
                              c.ID_COND,
                              c.NAM_COND
                          }).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListStatCondAsset()
        {

            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            var query = dbc.CONDITIONs.Where(c => c.VIG_COND == true && c.VIS_NEW_ASSE == true);

            if (!String.IsNullOrEmpty(FIELD))
            {
                query = query.Where(c => c.NAM_COND.Contains(FIELD.ToUpper()));
            }

            var result = (from c in query.ToList()
                          join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                          select new
                          {
                              c.ID_COND,
                              c.NAM_COND,
                              s.NAM_STAT_ASSE,
                              s.ID_STAT_ASSE,
                              TEXT = s.NAM_STAT_ASSE + " - " + c.NAM_COND
                          }).OrderBy(x => x.NAM_STAT_ASSE).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Condition/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarCondicionActualizar()
        {
            var resultado = (from o in dbc.OPTION_CONDITION
                             join c in dbc.CONDITIONs on o.ID_COND equals c.ID_COND 
                             join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                             select new
                             {
                                 IdCondicion = c.ID_COND,
                                 IdEstadoActivo = c.ID_STAT_ASSE,
                                 Condicion = c.NAM_COND,
                                 EstadoActivo = s.NAM_STAT_ASSE,
                                 VigCond = c.VIG_COND
                             }).Where(x => x.IdEstadoActivo != 1 && x.VigCond == true).Distinct().OrderBy(x => x.EstadoActivo).OrderBy(x => x.Condicion);

            return Json(new { Data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCondicionesBNV(int idGrupo = 0)
        {
            int IdEstado;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "NAM_COND")
            {
                IdEstado = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                IdEstado = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarCondicionActBNV(idGrupo, IdEstado, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCondicionEntregaBNV(int id = 0, int idGrupo = 0)
        {
            var result = dbc.ListarCondicionEntregaAct(idGrupo, id).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
