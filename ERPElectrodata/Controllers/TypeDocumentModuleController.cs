using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class TypeDocumentModuleController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /TypeDocumentModule/ListByModule/1
        public ActionResult ListByModule(int id)
        {
            var query = dbc.TYPE_DOCUMENT_MODULE
                .Where(x=>x.VIG_TYPE_DOCU_MODU == true)
                .Where(x=>x.ID_MODU == id);

            var result = (from x in query.ToList()
                          join td in dbc.TYPE_DOCUMENT_ATTACH.Where(x=>x.VIG_TYPE_DOCU_ATTA == true).ToList() on x.ID_TYPE_DOCU_ATTA equals td.ID_TYPE_DOCU_ATTA
                          select new {
                              NAM_TYPE_DOCU_ATTA = td.NAM_TYPE_DOCU_ATTA,
                              ID_TYPE_DOCU_ATTA = td.ID_TYPE_DOCU_ATTA,
                          }).OrderBy(x=>x.NAM_TYPE_DOCU_ATTA);

            return Json(new { Data = result, Total = query.Count()},JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarTipoDocumento(string q, string page,int id)
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

            var query = dbc.TYPE_DOCUMENT_MODULE
                .Where(x => x.VIG_TYPE_DOCU_MODU == true)
                .Where(x => x.ID_MODU == id);

            var result = (from x in query.ToList()
                          join td in dbc.TYPE_DOCUMENT_ATTACH.Where(x => x.VIG_TYPE_DOCU_ATTA == true).ToList() on x.ID_TYPE_DOCU_ATTA equals td.ID_TYPE_DOCU_ATTA
                          select new
                          {
                              id = td.ID_TYPE_DOCU_ATTA,
                              text = td.NAM_TYPE_DOCU_ATTA,
                          }).Where(x => x.text.ToLower().Contains(termino.ToLower())).OrderBy(x => x.text);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarTipoDocumentos()
        {

            var query = dbc.ListarTipoDocumentos();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
