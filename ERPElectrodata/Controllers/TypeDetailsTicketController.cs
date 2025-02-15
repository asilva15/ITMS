using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeDetailsTicketController : Controller
    {

        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /TypeDetailsTicket/List
        public ActionResult List(int id = 0)
        {
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            if(ID_QUEU == 26 && id == 26){
                var status = new int[] { 1,2,3,8};
                var result = (from tdt in dbc.TYPE_DETAILS_TICKET
                              where status.Contains(tdt.ID_TYPE_DETA_TICK) 
                              select new
                              {
                                  tdt.ID_TYPE_DETA_TICK,
                                  tdt.NAM_TYPE_DETA_TICK
                              }).ToList().OrderBy(x=>x.NAM_TYPE_DETA_TICK);
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else{
            var result = (from tdt in dbc.TYPE_DETAILS_TICKET
                          where tdt.ID_TYPE_DETA_TICK <= 3
                          select new { 
                          tdt.ID_TYPE_DETA_TICK,
                          tdt.NAM_TYPE_DETA_TICK
                          }).ToList().OrderBy(x => x.NAM_TYPE_DETA_TICK);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            
        }

        //
        // GET: /TypeDetailsTicket/List
        public ActionResult ListTypeDetailsTicketByID_MODU(int id = 0, int id1 = 0)
        {
            if (id1 == 8)//Change Aprobado
            {   //Solo listamos el Log Comment y Execute
                var result = (from tdt in dbc.TYPE_DETAILS_TICKET.Where(x => x.ID_TYPE_DETA_TICK == 1 || x.ID_TYPE_DETA_TICK == 11)
                              select new
                              {
                                  tdt.ID_TYPE_DETA_TICK,
                                  tdt.NAM_TYPE_DETA_TICK
                              }).OrderBy(x => x.NAM_TYPE_DETA_TICK).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from mtdt in dbc.MODULE_TYPE_DETAILS_TICKET.Where(x => x.ID_MODU == id)
                              join tdt in dbc.TYPE_DETAILS_TICKET on mtdt.ID_TYPE_DETA_TICK equals tdt.ID_TYPE_DETA_TICK
                              select new
                              {
                                  tdt.ID_TYPE_DETA_TICK,
                                  tdt.NAM_TYPE_DETA_TICK
                              }).OrderBy(x => x.NAM_TYPE_DETA_TICK).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListTypeDetailsTicketByID_MODUID_STAT_END(int id = 0)
        {
           
                var result = (from mtdt in dbc.MODULE_TYPE_DETAILS_TICKET.Where(x => x.ID_MODU == id)
                              join tdt in dbc.TYPE_DETAILS_TICKET on mtdt.ID_TYPE_DETA_TICK equals tdt.ID_TYPE_DETA_TICK
                              where tdt.ID_TYPE_DETA_TICK==1
                              select new
                              {
                                  tdt.ID_TYPE_DETA_TICK,
                                  tdt.NAM_TYPE_DETA_TICK
                              }).OrderBy(x => x.NAM_TYPE_DETA_TICK).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            
        }
        //
        // GET: /TypeDetailsTicket/

        public ActionResult Index()
        {
            return View();
        }

    }
}
