using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class LocationController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /Location/
        /**/
        public ActionResult List()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var query = dbc.SITEs.Where(s => s.ID_ACCO == ID_ACCO);

            var result = (from s in query.ToList()
                          join l in dbc.LOCATIONs on s.ID_SITE equals l.ID_SITE
                          select new
                          {
                              l.ID_LOCA,
                              s.ID_SITE,
                              l.NAM_LOCA,
                              s.NAM_SITE
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAsset() {

            int ID_SITE;
            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);
            if (FIELD == "ID_SITE")
            {
                ID_SITE = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }
            else {
                ID_SITE = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
            }
            var query = dbc.LOCATIONs.Where(l => l.ID_SITE == ID_SITE);
            var result = (from x in query.ToList()
                          select new { 
                            x.ID_LOCA,
                            x.NAM_LOCA
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListTalent()
        {
            //var query = dbc.SITEs.Where(s => s.ID_ACCO == 4);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var result = (from s in dbc.SITEs.Where(s => s.ID_ACCO == ID_ACCO)
                          join l in dbc.LOCATIONs on s.ID_SITE equals l.ID_SITE
                          select new
                          {
                              s.ID_SITE,
                              l.ID_LOCA,
                              l.NAM_LOCA,
                              s.NAM_SITE,
                              NAME = s.NAM_SITE + " - " + l.NAM_LOCA,
                          }).OrderBy(x => x.NAM_SITE + " - " + x.NAM_LOCA);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListChangeStatus(int id = 0)
        {
            //id: ID_PERS_ENTI
            string ID_ENTI = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id).ID_ENTI1.Value.ToString();
            //x.ID_PARA = 6 : CIA
            var ap = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 6 && x.VAL_ACCO_PARA == ID_ENTI);
            int ID_ACCO = (ap == null ? 0 : ap.ID_ACCO.Value);

            var result = (from s in dbc.SITEs.Where(s => s.ID_ACCO == ID_ACCO)
                          join l in dbc.LOCATIONs on s.ID_SITE equals l.ID_SITE
                          select new
                          {
                              s.ID_SITE,
                              l.ID_LOCA,
                              l.NAM_LOCA,
                              s.NAM_SITE,
                              NAME = s.NAM_SITE + " - " + l.NAM_LOCA,
                          }).OrderBy(x => x.NAM_SITE + " - " + x.NAM_LOCA);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarLocacion(string q, string page)
        {
            string termino = "";
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            List<ListarLocacion_Result> resultado = dbc.ListarLocacion(termino, IdAcco).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLocacionxCuenta()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = dbc.ListarLocacionxCuenta(ID_ACCO).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarLocacionesxCompania(int IdEnti = 0, int Tipo = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (ID_ACCO == 60)
            {
                var result = dbc.ListarLocacionxCuenta(ID_ACCO).ToList();

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int ID_CIA = 0;
                string CTE = "";

                if (Tipo == 1)
                {

                    ID_CIA = IdEnti;
                }
                else if (Tipo == 2)
                {
                    string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
                    if (NAM_PAR == "ID_ENTI")
                    {
                        ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
                    }
                    else if (NAM_PAR == "text")
                    {
                        CTE = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
                    }

                    NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
                    if (NAM_PAR == "ID_ENTI")
                    {
                        ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
                    }
                    else if (NAM_PAR == "text")
                    {
                        CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
                    }
                }
                else
                {

                    string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
                    if (NAM_PAR == "ID_CIA")
                    {
                        ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
                    }
                    else if (NAM_PAR == "NamLoca")
                    {
                        CTE = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
                    }

                    NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
                    if (NAM_PAR == "ID_CIA")
                    {
                        ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
                    }
                    else if (NAM_PAR == "NamLoca")
                    {
                        CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
                    }
                }

                var result = dbc.ListarLocacionesxCompania(ID_CIA);
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
            

        }
        public ActionResult ListarLocacionesxCompaniaBnv()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var result = dbc.ListarLocacionxCuenta(ID_ACCO).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CrearLocacionBNVAct(int id = 0, int idGrupo = 0, int mant = 0)
        {
            ViewBag.UMinera = id;
            ViewBag.IdGrupo = idGrupo;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearLocacionBNVAct(IEnumerable<HttpPostedFileBase> files, LOCATION loca)
        {
            string nombre = Convert.ToString(Request.Form["NAM_LOCA"]);

            if (nombre.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneLocacion('ERROR','0','');}window.onload=init;</script>");
            }

            int idUMinera = Convert.ToInt32(Request.Form["ID_UMINERA"]);
            int idGrupoAct = Convert.ToInt32(Request.Form["idGrupoAct"]);
            loca.NAM_LOCA = loca.NAM_LOCA.ToUpper();
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            if (loca.ID_SITE == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneLocacion('ERROR','0');}window.onload=init;</script>");
            }

            int id;
            try
            {
                var location = dbc.LOCATIONs.FirstOrDefault(x => x.NAM_LOCA.ToUpper() == loca.NAM_LOCA.ToUpper() && x.ID_SITE == loca.ID_SITE);
                if (location != null)
                {
                    id = location.ID_LOCA;
                }
                else
                {
                    loca.VIG_LOCA = false;
                    dbc.LOCATIONs.Add(loca);
                    dbc.SaveChanges();
                    id = Convert.ToInt32(loca.ID_LOCA);
                }

                int exis = dbc.LocacionxGrupo.Where(x => x.IdLocacion == id && x.IdGrupoActivo == idGrupoAct && x.UMinera == idUMinera).Count();
                if (exis == 0)
                {
                    LocacionxGrupo locaGru = new LocacionxGrupo();
                    locaGru.IdLocacion = id;
                    locaGru.IdGrupoActivo = idGrupoAct;
                    locaGru.UMinera = idUMinera;
                    locaGru.UsuarioCreacion = UserId;
                    locaGru.FechaCreacion = DateTime.Now;
                    locaGru.Estado = true;
                    dbc.LocacionxGrupo.Add(locaGru);
                    dbc.SaveChanges();
                }

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDoneLocacion('OK','" + id.ToString() + "','" + nombre + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneLocacion('ERROR','1','');}window.onload=init;</script>");
            }
        }

        public ActionResult ListadoLocacionesBNV(int IdGrupo = 0, int idUMinera = 0)
        {
            var response = dbc.ListadoLocaciones(IdGrupo, idUMinera).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoLocacionBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var locacion = dbc.LocacionxGrupo.FirstOrDefault(x => x.Id == id);
                if (locacion != null)
                {
                    locacion.Estado = Convert.ToBoolean(idEstado);
                    locacion.UsuarioModifica = Convert.ToInt32(Session["UserId"].ToString());
                    locacion.FechaModifica = DateTime.Now;
                    dbc.LocacionxGrupo.Attach(locacion);
                    dbc.Entry(locacion).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ListaLocacionPorGrupoSede()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["idGrupo"]);
            int idSede;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "NAM_LOCA")
            {
                idSede = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idSede = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarLocacionPorGrupo(idSede, idGrupo, idAcco, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLocacionPorGrupo()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["idGrupo"]);

            var result = dbc.ListarLocacionPorGrupo(0, idGrupo, idAcco, "").ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearLocacionPorGrupo(int idGrupo = 0, int mant = 0)
        {
            ViewBag.IdGrupo = idGrupo;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearLocacionPorGrupo(IEnumerable<HttpPostedFileBase> files, LOCATION loca)
        {
            string nombre = Convert.ToString(Request.Form["NAM_LOCA"]);

            if (nombre.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneLocacion('ERROR','0','');}window.onload=init;</script>");
            }

            loca.NAM_LOCA = loca.NAM_LOCA.ToUpper();

            int idGrupoAct = Convert.ToInt32(Request.Form["idGrupoAct"]);
            int idAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int userId = Convert.ToInt32(Session["UserId"].ToString());

            if (loca.ID_SITE == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneLocacion('ERROR','0');}window.onload=init;</script>");
            }

            int id;
            try
            {
                var location = dbc.LOCATIONs.FirstOrDefault(x => x.NAM_LOCA.ToUpper() == loca.NAM_LOCA.ToUpper() && x.ID_SITE == loca.ID_SITE);
                if (location != null)
                {
                    id = location.ID_LOCA;
                }
                else
                {
                    loca.VIG_LOCA = false;
                    dbc.LOCATIONs.Add(loca);
                    dbc.SaveChanges();
                    id = Convert.ToInt32(loca.ID_LOCA);
                }

                dbc.InsertarLocacionPorGrupo(id, idGrupoAct, idAcco, userId);

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDoneLocacion('OK','" + id.ToString() + "','" + nombre + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneLocacion('ERROR','1','');}window.onload=init;</script>");
            }
        }

    }
}
