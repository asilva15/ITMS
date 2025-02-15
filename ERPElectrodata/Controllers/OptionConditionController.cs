using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class OptionConditionController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        //
        // GET: /OptionCondition/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /OptionCondition/ListOptionStatCond
        public ActionResult ListOptionConditionByIdCond(int id = 0)
        {
            int idcond = Convert.ToInt32(dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == id && x.DAT_END == null).ID_COND);

            var result = (from o in dbc.OPTION_CONDITION //.Where(x => x.ID_COND == idcond)
                          join c in dbc.CONDITIONs on o.ID_OPTION equals c.ID_COND
                          join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                          select new
                          {
                              c.ID_COND,
                              c.NAM_COND,
                              s.NAM_STAT_ASSE,
                              s.ID_STAT_ASSE,
                              TEXT = s.NAM_STAT_ASSE + " - " + c.NAM_COND
                          }).Distinct();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarCondicionEntrega(int id = 0)
        {
            //int idcond = Convert.ToInt32(dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == id && x.DAT_END == null).ID_COND);

            var result = (from o in dbc.OPTION_CONDITION //.Where(x => x.ID_COND == idcond)
                          join c in dbc.CONDITIONs on o.ID_COND equals c.ID_COND
                          join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                          select new
                          {
                              c.ID_COND,
                              c.NAM_COND,
                              s.NAM_STAT_ASSE,
                              s.ID_STAT_ASSE,
                              TEXT = s.NAM_STAT_ASSE + " - " + c.NAM_COND,
                              o.VIG_OPTION
                          }).Where(x=>x.VIG_OPTION == true && x.ID_STAT_ASSE == 1).Distinct().OrderBy(x => x.NAM_COND);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCondicionRecepcion(int id = 0)
        {
            //int idcond = Convert.ToInt32(dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == id && x.DAT_END == null).ID_COND);

            if ((Convert.ToInt32(Session["ID_ACCO"]) == 56 || Convert.ToInt32(Session["ID_ACCO"]) == 57 || Convert.ToInt32(Session["ID_ACCO"]) == 58) && Convert.ToInt32(Session["GESTOR_ACTIVOS_MINSUR_MARCOBRE_RAURA"]) == 0)
            {
                var result = (from o in dbc.OPTION_CONDITION //.Where(x => x.ID_COND == idcond)
                              join c in dbc.CONDITIONs on o.ID_COND equals c.ID_COND
                              join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                              select new
                              {
                                  c.ID_COND,
                                  c.NAM_COND,
                                  s.NAM_STAT_ASSE,
                                  s.ID_STAT_ASSE,
                                  TEXT = s.NAM_STAT_ASSE + " - " + c.NAM_COND,
                                  o.VIG_OPTION,
                                  c.VIG_COND
                              }).Where(x => x.VIG_OPTION == true && x.ID_STAT_ASSE != 1 && x.ID_STAT_ASSE != 3).Distinct().OrderBy(x => x.NAM_COND);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from o in dbc.OPTION_CONDITION //.Where(x => x.ID_COND == idcond)
                              join c in dbc.CONDITIONs on o.ID_COND equals c.ID_COND
                              join s in dbc.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                              select new
                              {
                                  c.ID_COND,
                                  c.NAM_COND,
                                  s.NAM_STAT_ASSE,
                                  s.ID_STAT_ASSE,
                                  TEXT = s.NAM_STAT_ASSE + " - " + c.NAM_COND,
                                  o.VIG_OPTION,
                                  c.VIG_COND
                              }).Where(x => x.VIG_OPTION == true && x.ID_STAT_ASSE != 1).Distinct().OrderBy(x => x.NAM_COND);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
