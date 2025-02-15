using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class DocumentsSalesOppController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /DocumentsSalesOpp/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DocumentByStatus()
        {
            return View();
        }



    }
}
