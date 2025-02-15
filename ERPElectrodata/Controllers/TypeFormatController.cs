using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeFormatController : Controller
    {
        private CoreEntities dbc = new CoreEntities();

        //
        // GET: /TypeFormat/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /TypeFormat/List
        public ActionResult List() {
            var result = (from tf in dbc.TYPE_FORMAT
                          select new { 
                            tf.ID_TYPE_FORM,
                            tf.NAM_TYPE_FORM
                          }).OrderBy(x=>x.NAM_TYPE_FORM).ToList();

            return Json(new{Data=result, Count = result.Count()},JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListFormatActivos()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = dbc.ListarTipoFormatoActivos(ID_ACCO).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }
    }
}
