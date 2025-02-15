using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;

namespace ERPElectrodata.Controllers
{
    public class ParameterController : Controller
    {
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /Parameter/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarxParametro(string q, string page)
        {
            int idparametro = Convert.ToInt32(Request.QueryString["idparametro"]);

            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = (from x in dbc.ACCOUNT_PARAMETER
                          where x.ID_PARA == idparametro && 
                          x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO
                          select new
                          {
                              x.ID_ACCO_PARA,
                              x.VAL_ACCO_PARA
                          }).Where(x => x.VAL_ACCO_PARA.Contains(termino)).OrderBy(x => x.VAL_ACCO_PARA);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPorParametro(string q, string page)
        {
            int idparametro = Convert.ToInt32(Request.QueryString["idparametro"]);

            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = (from x in dbc.ACCOUNT_PARAMETER
                          where x.ID_PARA == idparametro &&
                          x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO
                          select new
                          {
                              id = x.ID_ACCO_PARA,
                              text = x.VAL_ACCO_PARA
                          }).Where(x=>x.text.Contains(termino)).OrderBy(x => x.text);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoLicencia()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarTipoLicencia(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarModoInstalacion()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarModoInstalacion(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarFechaPeriodo(string q, string page)
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

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int idParametro = Convert.ToInt32(Request.Params["idparametro"]);

            List<ListarFechaPeriodo_Result> resultado = dbc.ListarFechaPeriodo(idParametro).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarConceptoContrato()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarConceptoContrato(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarEstadoFirmaContrato()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarEstadoFirmaContrato(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoMonedaContrato()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarTipoMonedaContrato(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarFirmanteContrato()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarFirmanteContrato().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
