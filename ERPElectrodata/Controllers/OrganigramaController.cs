using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class OrganigramaController : Controller
    {
        //
        // GET: /Organigrama/
        EntityEntities dbe = new EntityEntities();

        [Authorize]
        public ActionResult Inicio()
        {
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [Authorize]
        public ActionResult DetalleOrganigrama(int id)
        {
            try
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Id = id;
            return View();
        }

        public JsonResult OrganigramaEmpresaGerenciaArea(int id)
        {
            List<OrganigramaEmpresaGerenciaArea_Result> resultado = dbe.OrganigramaEmpresaGerenciaArea(id).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerOrganigramaDetalle(int id)
        {
            List<OrganigramaEmpresa_Result> resultado = dbe.OrganigramaEmpresa(id).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult BuscarOrganigrama(string name = "")
        {
            if (name != "")
            {
                var searchTerms = name.ToLower().Split(' ');

                var query = (from a in dbe.CHARTs
                                     .Where(x => x.VIG_CHAR == true).ToList()
                             join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR
                             join pcch in dbe.PERSON_CONTRACT_CHART.Where(x => x.VIG_CONT_CHAR == true)
                                        on (b.ID_TYPE_CHAR == 3 ? a.ID_CHAR : 0) equals pcch.ID_CHAR into lpcch
                             from xpcch in lpcch.DefaultIfEmpty()
                             join d in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true)
                                        on (xpcch == null ? 0 : xpcch.ID_PERS_CONT) equals d.ID_PERS_CONT into ld
                             from xd in ld.DefaultIfEmpty()
                             join e in dbe.PERSON_ENTITY on (xd == null ? 0 : xd.ID_PERS_ENTI) equals e.ID_PERS_ENTI into le
                             from xe in le.DefaultIfEmpty()
                             join f in dbe.CLASS_ENTITY on (xe == null ? 0 : xe.ID_ENTI2) equals f.ID_ENTI into lf
                             from xf in lf.DefaultIfEmpty()
                             select new
                             {
                                 a.ID_CHAR_PARE,
                                 USER = (xf == null ? "-" : xf.FIR_NAME + " " + xf.LAS_NAME + " " + xf.MOT_NAME).ToLower(),
                             })
                         .Where(x => searchTerms.All(term => x.USER.Contains(term)));

                var results = query
                                    .Select(item => new
                                    {
                                        ID_PARENT = GetIdParent(Convert.ToInt32(item.ID_CHAR_PARE))
                                    }).ToList();

                var groupResults = results
                                    .GroupBy(x => x.ID_PARENT)
                                    .Select(group => group.Key)
                                    .ToList();

                ViewBag.SearchList = groupResults;
                ViewBag.SearchName = name;
                return View();
            }

            ViewBag.SearchList = null;
            ViewBag.SearchName = name;
            return View();
        }

        public JsonResult ObtenerOrganigramaBusqueda(int id, string name)
        {
            List<OrganigramaEmpresa_Result> resultado = dbe.OrganigramaEmpresa(id).ToList();
            var searchTerms = name.ToUpper().Split(' ');
            var resultadoData = resultado.Where(r => searchTerms.All(term => r.title.Contains(term)));

            List<OrganigramaEmpresa_Result> ObtenerJerarquiaCargos(int currentId)
            {
                var jerarquia = new List<OrganigramaEmpresa_Result>();
                var current = resultado.FirstOrDefault(item => item.id == currentId);

                while (current != null)
                {
                    jerarquia.Insert(0, current);
                    current = resultado.FirstOrDefault(item => item.id == current.parent);
                }

                return jerarquia;
            }

            List<OrganigramaEmpresa_Result> ObtenerCargosHijos(int currentId)
            {
                var equipo = new List<OrganigramaEmpresa_Result>();
                var hijos = resultado.Where(item => item.parent == currentId).ToList();

                foreach (var hijo in hijos)
                {
                    equipo.Add(hijo);
                    equipo.AddRange(ObtenerCargosHijos((int)hijo.id));
                }

                return equipo;
            }

            var resultadoArea = new List<OrganigramaEmpresa_Result>();
            foreach (var current in resultadoData)
            {
                var jerarquiaArea = ObtenerJerarquiaCargos((int)current.id);
                var hijosCurrent = ObtenerCargosHijos((int)current.id);
                resultadoArea.AddRange(jerarquiaArea);
                resultadoArea.AddRange(hijosCurrent);
            }

            return Json(resultadoArea, JsonRequestBehavior.AllowGet);
        }

        public int GetIdParent(int idparent)
        {
            var resultado = (from a in dbe.CHARTs
                             join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR
                             where a.VIG_CHAR == true && a.ID_CHAR == idparent
                             select new
                             {
                                 a.ID_CHAR,
                                 a.ID_CHAR_PARE,
                                 b.ID_TYPE_CHAR
                             }).SingleOrDefault();

            if (resultado != null)
            {
                if (resultado.ID_TYPE_CHAR == 2)
                {
                    return resultado.ID_CHAR;
                }
                else if (resultado.ID_CHAR_PARE.HasValue)
                {
                    return GetIdParent((int)resultado.ID_CHAR_PARE);
                }
            }

            return idparent;
        }
    }
}
