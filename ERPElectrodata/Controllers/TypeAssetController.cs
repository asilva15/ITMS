using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;
using System.Data.Entity;
using System.Data;
using System.Web.UI.WebControls;


namespace ERPElectrodata.Controllers
{
    public class TypeAssetController : Controller
    {
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /TypeAsset/

        public ActionResult List()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = (from x in dbc.TYPE_ASSET.ToList()
                          join ata in dbc.ACCOUNT_TYPE_ASSET on x.ID_TYPE_ASSE equals ata.ID_TYPE_ASSE
                          where x.ID_TYPE_ASSE > 0
                          select new
                          {
                              x.ID_TYPE_ASSE,
                              x.NAM_TYPE_ASSE,
                              ata.ID_ACCO
                          }).Where(x => x.ID_ACCO == ID_ACCO).OrderBy(x => x.NAM_TYPE_ASSE);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListContrato()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = (from x in dbc.ASSETs.ToList()
                          join ata in dbc.Contratoes on x.IdContrato equals ata.Id
                          where x.IdContrato > 0
                          select new
                          {
                              x.IdContrato,
                              ata.Nombre,
                              ata.ID_ACCO
                          }).Where(x => x.ID_ACCO == ID_ACCO).Distinct().OrderBy(x => x.Nombre);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TypeByIdAcco(int idTipoActivo)
        {
            textInfo = cultureInfo.TextInfo;

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = (from a in dbc.ASSETs.Where(x => x.ID_ACCO == ID_ACCO)
                         join ca in dbc.CLIENT_ASSET on a.ID_ASSE equals ca.ID_ASSE
                         select new
                         {
                             a.ID_ASSE,
                             a.ID_TYPE_ASSE,
                             ca.DAT_END
                         }).Where(x => x.DAT_END == null);

            if (idTipoActivo != 0)
            {
                query = query.Where(x => x.ID_TYPE_ASSE == idTipoActivo);
            }

            var result = (from a in query.ToList()
                          join ta in dbc.TYPE_ASSET on a.ID_TYPE_ASSE equals ta.ID_TYPE_ASSE
                          group ta by new { ta.ID_TYPE_ASSE, ta.NAM_TYPE_ASSE } into g
                          select new
                          {
                              NAM_TYPE_ASSE = textInfo.ToTitleCase(g.Key.NAM_TYPE_ASSE.ToLower()),
                              COUNT = g.Count()
                          }).OrderByDescending(x => x.COUNT);

            var pie = dbc.ASSET_TOP5(ID_ACCO, idTipoActivo).ToList();

            return Json(new { Data = result.Skip(0).Take(6), Others = result.Skip(7).Sum(x => x.COUNT), pie = pie }, JsonRequestBehavior.AllowGet);

        }

        //
        // GET: /SubTypeAsset/
        public ActionResult ListSubType(int typeasset)
        {

            var result = (from x in dbc.SUBTYPE_ASSET.ToList()
                          where x.ID_TYPE_ASSE > 0 && x.ID_TYPE_ASSE == typeasset
                          select new
                          {
                              x.ID_SUBT_ASSE,
                              x.NAM_SUBT_ASSE
                          }).OrderBy(x => x.NAM_SUBT_ASSE);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(int id = 0, int mant = 0)
        {
            ViewBag.IdGrupo = id;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(TYPE_ASSET objComponente)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int userId = Convert.ToInt32(Session["UserId"]);
            int idGrupoAct = Convert.ToInt32(Request.Form["idGrupoAct"]);
            int flag = 0;

            objComponente.NAM_TYPE_ASSE = objComponente.NAM_TYPE_ASSE?.Trim() ?? string.Empty;
            objComponente.DESC_TYPE_ASSE = objComponente.DESC_TYPE_ASSE?.Trim() ?? string.Empty;

            if (String.IsNullOrEmpty(objComponente.NAM_TYPE_ASSE)) { flag = 2; }
            if (String.IsNullOrEmpty(objComponente.DESC_TYPE_ASSE)) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneTypeAsset('ERROR','0');}window.onload=init;</script>");
            }

            int id = 0;
            var tAsset = dbc.TYPE_ASSET.FirstOrDefault(x => x.NAM_TYPE_ASSE.ToUpper() == objComponente.NAM_TYPE_ASSE.ToUpper());

            if (tAsset != null)
            {
                id = tAsset.ID_TYPE_ASSE;
            }
            else
            {
                objComponente.IN_GRAP = true;
                objComponente.NAM_TYPE_ASSE = objComponente.NAM_TYPE_ASSE.ToUpper();
                Convert.ToString(objComponente.COLOR);
                objComponente.INDICE = 6;
                dbc.TYPE_ASSET.Add(objComponente);
                dbc.SaveChanges();

                id = objComponente.ID_TYPE_ASSE;
            }

            if (idAcco == 60 && idGrupoAct != 0)
            {
                int exis = dbc.TipoActivoxGrupo.Where(x => x.IdTipoActivo == id && x.IdGrupoActivo == idGrupoAct).Count();
                if (exis == 0)
                {
                    TipoActivoxGrupo tipo = new TipoActivoxGrupo();
                    tipo.IdTipoActivo = id;
                    tipo.IdGrupoActivo = idGrupoAct;
                    tipo.FechaCreacion = DateTime.Now;
                    tipo.UsuarioCreacion = userId;
                    tipo.Estado = true;
                    dbc.TipoActivoxGrupo.Add(tipo);
                    dbc.SaveChanges();
                }
            }
            else if (idAcco == 55 && idGrupoAct != 0)
            {
                dbc.InsertarTipoActivoPorGrupo(id, idGrupoAct, idAcco, userId);
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneTypeAsset('ERROR','1');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneTypeAsset('OK','" + id.ToString() + "');}window.onload=init;</script>");
        }

        public ActionResult ListarTipoActivo()
        {
            int ID_ACCO = 0;
            //string txt = "";
            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (FIELD == "ID_ACCO")
            {
                ID_ACCO = 0;
                //txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                //if (txt == null) { txt = ""; }
            }
            else
            {
                ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            }

            var query = dbc.TAcTListarTipoActivo(ID_ACCO).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoActivoPorCuenta()
        {
            int ID_ACCO;
            string txt = "";
            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (FIELD == "NAM_TYPE_ASSE")
            {
                ID_ACCO = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) { txt = ""; }
            }
            else
            {
                ID_ACCO = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ComboTipoActivoxCuenta(ID_ACCO, txt).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTodoTipoActivo()
        {
            
            var query = dbc.TAcTListarTodoTipoActivo().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoActivoProgramas()
        {
            var query = dbc.TActListarTipoActivoProgramas().ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoActivoPorCargo()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdChar = Convert.ToInt32(Request.Params["IdChar"]);

            var result = dbc.ListarTipoActivoPorCargo(IdChar, IdAcco).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoActivoxGrupoBNV()
        {
            int idGrupo;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "NAM_TYPE_ASSE")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarTipoActivoBNV(idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListadoTipoActivosBNV(int IdGrupo = 0, int IdTipoActivo = 0)
        {
            var response = dbc.ListadoTipoActivos(IdGrupo, IdTipoActivo).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoTipoActivoBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var tipoActivo = dbc.TipoActivoxGrupo.FirstOrDefault(x => x.Id == id);
                if (tipoActivo != null)
                {
                    tipoActivo.Estado = Convert.ToBoolean(idEstado);
                    tipoActivo.UsuarioModifica = Convert.ToInt32(Session["UserId"].ToString());
                    tipoActivo.FechaModifica = DateTime.Now;
                    dbc.TipoActivoxGrupo.Attach(tipoActivo);
                    dbc.Entry(tipoActivo).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ListarTipoActivoPorGrupo()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["idGrupo"]);

            var result = dbc.ListarTipoActivoPorGrupo(idGrupo, idAcco).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
