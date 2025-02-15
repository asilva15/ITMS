using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeClientController : Controller
    {
        private EntityEntities dbe = new EntityEntities();
        //
        // GET: /TypeClient/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() {
            var result = (from tc in dbe.TYPE_CLIENT
                          select new { 
                            tc.ID_TYPE_CLIE,
                            tc.NAM_TYPE_CLIE
                          });
            return Json(new { Data = result},JsonRequestBehavior.AllowGet);
        }
        public ActionResult List2()
        {
            var result = (from tc in dbe.TIPO_TRABAJADOR
                          select new
                          {
                              tc.Id,
                              tc.Nombre
                          });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarTipoCliente(string q, string page)
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

            var result = dbe.ListarTipoCliente(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}
