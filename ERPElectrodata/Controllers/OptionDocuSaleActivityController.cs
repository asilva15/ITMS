using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;

namespace ERPElectrodata.Controllers
{
    public class OptionDocuSaleActivityController : Controller
    {
        public CoreEntities dbc = new CoreEntities();        
        //
        // GET: /OptionDocuSaleActivity/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListOptionDocuSaleActivity(int id = 0, int id1 = 0)
        {
            int ID_STAT_TICK = 0;

            if (id == 2) { //Si el estado de la OP es assignee
                var tick = dbc.TICKETs.Single(x => x.ID_TICK_PARENT == null && x.ID_DOCU_SALE == id1);

                if (tick != null)
                {
                    ID_STAT_TICK = dbc.TICKETs.Single(x => x.ID_TICK_PARENT == null && x.ID_DOCU_SALE == id1).ID_STAT_END.Value;
                }            
            }            

            if (ID_STAT_TICK == 4 || ID_STAT_TICK == 6) //Tickets Parent esta resuelto o cerrado
            {
                var query = (from a in dbc.TYPE_ACTIVITY.Where(x=>x.ID_TYPE_ACTI == 1 || x.ID_TYPE_ACTI == 5)
                             select new
                             {
                                 a.NAM_TYPE_ACTI,
                                 a.ID_TYPE_ACTI,
                             });

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            else {
                var query = (from a in dbc.OPTION_DOCU_SALE_ACTIVITY.Where(x => x.ID_STAT_DOCU_SALE == id && x.VIG_STAT_DOCU_ACTI == true)
                             join b in dbc.TYPE_ACTIVITY on a.ID_TYPE_ACTI equals b.ID_TYPE_ACTI
                             select new
                             {
                                 b.NAM_TYPE_ACTI,
                                 b.ID_TYPE_ACTI,
                             });
                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);            
            }            
        }

        public ActionResult ListarOpcionObservacion()
        {
            var query = (from a in dbc.TYPE_ACTIVITY.Where(x => x.ID_TYPE_ACTI == 1 || x.ID_TYPE_ACTI == 7 )
                         select new
                         {
                             a.NAM_TYPE_ACTI,
                             a.ID_TYPE_ACTI,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarOpcionesActividad(string q, string page)
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

            List<ListarOpcionesActividad_Result> resultado = dbc.ListarOpcionesActividad(termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

    }
}
