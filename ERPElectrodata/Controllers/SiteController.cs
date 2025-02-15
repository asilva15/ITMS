using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class SiteController : Controller
    {

        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /Site/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.SITEs.Where(s=>s.ID_ACCO == ID_ACCO);

            var result = (from a in query.ToList()
                          select new { 
                            a.ID_SITE,
                            a.NAM_SITE
                          });
            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSedePorGrupo()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["idGrupo"]);

            var result = dbc.ListarSedePorGrupo(idGrupo, idAcco).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearSedePorGrupo()
        {
            int id = 0;

            try
            {
                string nomSede = Request.Params["NomSede"].ToString();
                int userId = Convert.ToInt32(Session["UserId"].ToString());
                int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
                int idGrupoAct = Convert.ToInt32(Request.Params["idGrupo"].ToString());

                var sede = dbc.SITEs.FirstOrDefault(x => x.NAM_SITE.ToUpper() == nomSede.ToUpper() && x.ID_ACCO == idAcco);
                if (sede != null)
                {
                    id = sede.ID_SITE;
                }
                else
                {
                    SITE newSite = new SITE();
                    newSite.NAM_SITE = nomSede.ToUpper();
                    newSite.ID_ACCO = idAcco;
                    dbc.SITEs.Add(newSite);
                    dbc.SaveChanges();
                    id = Convert.ToInt32(newSite.ID_SITE);
                }

                dbc.InsertarSedePorGrupo(id, idGrupoAct, idAcco, userId);
            }
            catch
            {
                return Content("ERROR");
            }

            return Content(id.ToString());
        }

    }
}
