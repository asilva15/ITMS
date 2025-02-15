using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ERPElectrodata.Models;
using System.Data.Entity;
using ERPElectrodata.AppCode;
using System.Text;

using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Controllers
{
    public class SubTipoComponenteController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();

        public static int idTipoActivo = 0;
        public static int ID_ACCO = 0;
        public static int ID_ACCO1 = 0;
        public static int IdComponente = 0;

        //
        // GET: /SubTipoComponente/

        public ActionResult CrearSTC()
        {
            return View();
        }

        public ActionResult prueba()
        {
            return View();
        }

        public ActionResult Crear(SubTipoComponente objSubTipoComp)
        {
            //Validar campos en blanco
            int flag = 0;
            if (Convert.ToString(objSubTipoComp.IdComponente) == "") { flag = 1; }
            if (Convert.ToString(objSubTipoComp.IdTipoActivo) == "") { flag = 2; }
            if (String.IsNullOrEmpty(objSubTipoComp.NomSubtComp)) { flag = 3; }
            if (Convert.ToString(objSubTipoComp.IdAcco) == "") { flag = 4; }

            String.IsNullOrEmpty(objSubTipoComp.DesSubtComp);

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
                }
                var A = dbc.ACCOUNT_TYPE_ASSET.Single(x => x.ID == objSubTipoComp.IdTipoActivo);
                objSubTipoComp.IdTipoActivo = A.ID_TYPE_ASSE;

                objSubTipoComp.UsuarioCrea = IdUser;
                objSubTipoComp.FechaCrea = DateTime.Now;
                objSubTipoComp.Estado = true;
                objSubTipoComp.Activo = false;
                dbc.SubTipoComponentes.Add(objSubTipoComp);
                dbc.SaveChanges();
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objSubTipoComp.IdTipoActivo.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult ListarComponentesPorTipoActivo(int id = 0)
        {
            string txt = "";
            int ID = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);

            txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (txt == null) { txt = ""; }
            var query = dbc.CompListarComponentesPorTipoActivo(ID,txt).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BuscarListarSubTipoComponentes(int i = 0)
        {
            // int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ID_ACCO = Convert.ToInt32(Request.Params["ID_ACCO"].ToString());

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int ID = Convert.ToInt32(Request.Params["ID_TYPE_ASSE"].ToString());
            
            if (ID==0) {
                idTipoActivo = 0;
            }
            else {
            var A = dbc.ACCOUNT_TYPE_ASSET.Single(x => x.ID == ID);
            idTipoActivo = Convert.ToInt32(A.ID_TYPE_ASSE);
            }
            IdComponente = Convert.ToInt32(Request.Params["IdComponente"].ToString());
            string nombre = Convert.ToString(Request.Params["Nombre"].ToString());
            string descripcion = Convert.ToString(Request.Params["Descripcion"].ToString());

            if (descripcion == "")
            {
                var query = dbc.SubTipoComponenteListar(ID_ACCO, idTipoActivo, IdComponente, nombre, descripcion, 1).ToList();
                var total = query.Count();

                var query2 = query.Skip(skip).Take(take);

                return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var queryD = dbc.SubTipoComponenteListar(ID_ACCO, idTipoActivo, IdComponente, nombre, descripcion, 2).ToList();
                var total = queryD.Count();

                var query2 = queryD.Skip(skip).Take(take);

                return Json(new { Data = query2, Cantidad = queryD.Count() }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult EditarSTC(int id = 0)
        {

            ViewBag.IdSubtComp = id;
            //var pe = dbc.ACCOUNT_TYPE_ASSET.Find(id,idAcco);

            var st = dbc.SubTipoComponentes.Find(id);
            var a = dbc.ACCOUNTs.Single(x => x.ID_ACCO == st.IdAcco);
            var ta = dbc.TYPE_ASSET.Single(x => x.ID_TYPE_ASSE == st.IdTipoActivo);
            var c = dbc.Componentes.Single(x => x.IdComponente == st.IdComponente);

            //var cp = dbc.ACCOUNTs.Single(x => x.ID_ACCO == pe.ID_ACCO);
            //var ta = dbc.TYPE_ASSET.Single(x => x.ID_TYPE_ASSE == pe.ID_TYPE_ASSE);
            ViewBag.NAM_ACCO = a.NAM_ACCO;
            ViewBag.NAM_TYPE_ASSE = ta.NAM_TYPE_ASSE;
            ViewBag.Nombre = c.Nombre;
            ViewBag.NomSubtComp = st.NomSubtComp;
            ViewBag.DesSubtComp = st.DesSubtComp;

            return View(st);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Editar(SubTipoComponente objComp)
        {

            //Validar campos en blanco
            int flag = 0;

            if (Convert.ToString(objComp.IdTipoActivo) == "") { flag = 1; }
            if (String.IsNullOrEmpty(objComp.NomSubtComp)) { flag = 2; }
            if (Convert.ToString(objComp.IdAcco) == "") { flag = 3; }
            if (Convert.ToString(objComp.IdComponente) == "") { flag = 4; }
            String.IsNullOrEmpty(objComp.DesSubtComp);

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                //int IdAcco = 0;
                int IdUser = 0;
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
                }

                Convert.ToInt32(objComp.UsuarioCrea);
                Convert.ToDateTime(objComp.FechaCrea);
                Convert.ToBoolean(objComp.Estado);

                objComp.Activo = false;
                objComp.UsuarioModifica = IdUser;
                objComp.FechaModifica = DateTime.Now;

                dbc.SubTipoComponentes.Attach(objComp);
                dbc.Entry(objComp).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneEditar) top.uploadDoneEditar('OK','0','" + objComp.IdTipoActivo.ToString() + "');}window.onload=init;</script>");
                //return View();
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult ListarSubTipoComponentes(int id = 0)
        {
            string SubTipoComponente = "";
            int IdComp = 0;
            try
            {
                IdComp = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
                SubTipoComponente = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
                if (SubTipoComponente == null)
                    SubTipoComponente = "";
            } catch { }


            var query = dbc.CompListarSubTipoComponente(IdComp, SubTipoComponente).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }
}
