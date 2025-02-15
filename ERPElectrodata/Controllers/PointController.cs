using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class PointController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        
        //
        // GET: /Point/

        public ActionResult Index()
        {
            return View();
        }

        public int ListPoints()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int point = 0;
            try
            {
                var point_all = dbc.POINTs.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI);
                point = Convert.ToInt32(point_all.Sum(x => x.VAL_POIN));
            }
            catch
            {
                point = 0;
            }

            return point;
        }
    }
}
