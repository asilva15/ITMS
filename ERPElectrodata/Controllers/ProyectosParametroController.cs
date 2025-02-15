using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class ProyectosParametroController : Controller
    {
        public CoreEntities dbc = new CoreEntities();

        public ActionResult ObtenerParametro(string q, string page)
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

            var result = (from x in dbc.Parametro.Where(p => p.Estado == true)
                          select new
                          {
                              id = x.Id,
                              text = x.Nombre,
                          }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerParametroDetalle(string q, string page)
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

            int IdParametro = Convert.ToInt32(Request.Params["IdParametro"]);

            var result = (from x in dbc.ParametroDetalle.Where(p => p.Estado == true && p.IdParametro == IdParametro)
                          select new
                          {
                              id = x.Id,
                              text = x.Nombre,
                          }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
