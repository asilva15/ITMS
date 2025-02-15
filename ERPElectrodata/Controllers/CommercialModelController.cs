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
    public class CommercialModelController : Controller
    {
        public CoreEntities dbo = new CoreEntities();
        //public EntityEntities dbe = new EntityEntities();
        //
        // GET: /CommercialModel/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexMantCommercialModel()
        {
            return View();
        }

        public ActionResult CreaCommercialModel()
        {
            return View();
        }

        public ActionResult List()
        {
            int ID_MANU;
            string txt = "";
            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (FIELD == "NAM_COMM_MODE")
            {
                ID_MANU = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) { txt = ""; }
            }
            else
            {
                ID_MANU = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbo.ComboModeloComercial(ID_MANU,txt).ToList();
            //var query = dbo.COMMERCIAL_MODEL.Where(cm => cm.ID_MANU == ID_MANU && cm.NAM_COMM_MODE.Contains(txt));

            //var result = (from x in query.ToList()
            //                  // orderby x.NAM_COMM_MODE
            //              select new
            //              {
            //                  x.ID_COMM_MODE,
            //                  x.NAM_COMM_MODE,
            //                  x.Estado
            //              });//.OrderBy(x=>x.NAM_COMM_MODE);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarBuscarCommercialModelTodo()
        {
            int Marca = Convert.ToInt32(Request.Params["ID_MANU"].ToString());
            string ModeloComercial = Request.Params["NAM_COMM_MODE"].ToString();
            string Estado = Convert.ToString(Request.Params["Estado"]);
            var query = dbo.ListarBuscarCommercialModel(Marca, ModeloComercial, Estado).ToList();
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            var query2 = query.Skip(skip).Take(take);
            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreaCommercialModel(COMMERCIAL_MODEL objManComm)
        {
            int flag = 0, existe = 0;

            String Marca = Convert.ToString(Request.Params["ID_MANUCREA"]);

            if (String.IsNullOrEmpty(Marca)) { flag = 1; }
            if (String.IsNullOrEmpty(objManComm.NAM_COMM_MODE)) { flag = 2; }
            if (String.IsNullOrEmpty(objManComm.DESC_COMM_MODE)) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            objManComm.ID_MANU = Convert.ToInt32(Marca.ToString());
            String ModeloCom = objManComm.NAM_COMM_MODE;
            var result = (from at in dbo.COMMERCIAL_MODEL.Where(x => x.NAM_COMM_MODE == ModeloCom && x.ID_MANU == objManComm.ID_MANU).ToList()
                          select new { id = at.ID_MANU, marca = at.NAM_COMM_MODE }).ToList();
            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {

                int IdUser = 0; //CAMBIAR POR '0'
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
                }
                objManComm.UserIdCreacion = IdUser;
                objManComm.Estado = true;
                objManComm.FechaCreacion = DateTime.Now;
                dbo.COMMERCIAL_MODEL.Add(objManComm);
                dbo.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objManComm.ID_MANU.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }

        }

        public ActionResult EditaCommercialModel(int Id=0)
        {
            var c = dbo.COMMERCIAL_MODEL.Find(Id);
            var d = dbo.MANUFACTURERs.Find(c.ID_MANU);

            ViewBag.Marca = d.NAM_MANU;
            ViewBag.ModeloCom = c.NAM_COMM_MODE;
            ViewBag.Descripcion = c.DESC_COMM_MODE;
            ViewBag.Estado = c.Estado;
            return View(c);
        }

        [HttpPost,ValidateInput(false)]
        public ActionResult EditaCommercialModel(COMMERCIAL_MODEL objManComm)
        {
            int flag = 0;

            if (Convert.ToString(objManComm.ID_MANU) == "") { flag = 1; }

            if (Convert.ToString(objManComm.NAM_COMM_MODE) == null) { flag = 2; }

            if (Convert.ToString(objManComm.DESC_COMM_MODE) == null) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {

                int IdUser = 0; //CAMBIAR POR '0'
                try
                {
                    IdUser = Convert.ToInt32(Session["UserId"].ToString());
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
                }

                //Convert.ToInt32(objManModel.UserIdCreacion); //PROBANDO VALORES QUE SE MANTIENEN EN LA BD
                //Convert.ToDateTime(objManModel.FechaCreacion); //PROBANDO VALORES QUE SE MANTIENEN EN LA BD

                //objManComm.ID_MANU.ToString();
                //objManModel.NAM_MANU_MODE.ToString();//PROBANDO VALORES QUE SE MANTIENEN EN LA BD
                //objManModel.DESC_MANU_MODE.ToString();//PROBANDO VALORES QUE SE MANTIENEN EN LA BD

                
                objManComm.UserIdModifica = IdUser;
                objManComm.FechaModifica = DateTime.Now;
                Convert.ToBoolean(objManComm.Estado);
                dbo.COMMERCIAL_MODEL.Attach(objManComm);
                dbo.Entry(objManComm).State = EntityState.Modified;
                dbo.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objManComm.ID_COMM_MODE.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }
        public ActionResult Create(int id=0, int idGrupo = 0, int mant = 0)
        {
            ViewBag.ID_MANUFACTURER = id;
            ViewBag.IdGrupo = idGrupo;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        // POST: /CommercialModel/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, COMMERCIAL_MODEL comm)
        {
            string NAM_COMM_MODE = Convert.ToString(Request.Form["NAM_COMM_MODE"]);

            if (NAM_COMM_MODE.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneCommercial('ERROR','0','');}window.onload=init;</script>");
            }

            int ID_MANUFACTURER = Convert.ToInt32(Request.Form["ID_MANU_HF"]);
            comm.NAM_COMM_MODE = comm.NAM_COMM_MODE.ToUpper();

            string DESC_COMM_MODE = Convert.ToString(Request.Form["DESC_COMM_MODE"]);
            DESC_COMM_MODE = DESC_COMM_MODE.Replace("&nbsp;", "");
            DESC_COMM_MODE = DESC_COMM_MODE.Replace("<br />", "");
            if (DESC_COMM_MODE.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneCommercial('ERROR','0');}window.onload=init;</script>");
            }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int idGrupoAct = Convert.ToInt32(Request.Form["idGrupoAct"]);
            int userId = Convert.ToInt32(Session["UserId"].ToString());
            int id;

            if (ID_ACCO == 60 && idGrupoAct != 0)
            {
                try
                {
                    var commer = dbo.COMMERCIAL_MODEL.FirstOrDefault(x => x.NAM_COMM_MODE.ToUpper() == comm.NAM_COMM_MODE.ToUpper() && x.ID_MANU == ID_MANUFACTURER);
                    if (commer != null)
                    {
                        id = commer.ID_COMM_MODE;
                    }
                    else
                    {
                        comm.ID_MANU = ID_MANUFACTURER;
                        comm.FechaCreacion = DateTime.Now;
                        comm.UserIdCreacion = userId;
                        dbo.COMMERCIAL_MODEL.Add(comm);
                        dbo.SaveChanges();
                        id = Convert.ToInt32(comm.ID_COMM_MODE);
                    }

                    int exis = dbo.ModeloComerxGrupo.Where(x => x.IdModelo == id && x.IdGrupoActivo == idGrupoAct).Count();
                    if (exis == 0)
                    {
                        ModeloComerxGrupo modelo = new ModeloComerxGrupo();
                        modelo.IdModelo = id;
                        modelo.IdGrupoActivo = idGrupoAct;
                        modelo.FechaCreacion = DateTime.Now;
                        modelo.UsuarioCreacion = userId;
                        modelo.Estado = true;
                        dbo.ModeloComerxGrupo.Add(modelo);
                        dbo.SaveChanges();
                    }

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneCommercial('OK','" + id.ToString() + "','" + NAM_COMM_MODE + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneCommercial('ERROR','1','');}window.onload=init;</script>");
                }
            }
            else if (ID_ACCO == 55 && idGrupoAct != 0)
            {
                try
                {
                    var commer = dbo.COMMERCIAL_MODEL.FirstOrDefault(x => x.NAM_COMM_MODE.ToUpper() == comm.NAM_COMM_MODE.ToUpper() && x.ID_MANU == ID_MANUFACTURER);
                    if (commer != null)
                    {
                        id = commer.ID_COMM_MODE;
                    }
                    else
                    {
                        comm.ID_MANU = ID_MANUFACTURER;
                        comm.FechaCreacion = DateTime.Now;
                        comm.UserIdCreacion = userId;
                        dbo.COMMERCIAL_MODEL.Add(comm);
                        dbo.SaveChanges();
                        id = Convert.ToInt32(comm.ID_COMM_MODE);
                    }

                    dbo.InsertarModeloComercialPorGrupo(id, idGrupoAct, ID_ACCO, userId);
                    
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneCommercial('OK','" + id.ToString() + "','" + NAM_COMM_MODE + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneCommercial('ERROR','1','');}window.onload=init;</script>");
                }
            }
            else
            {
                int ctd = dbo.COMMERCIAL_MODEL.Where(x => x.NAM_COMM_MODE == comm.NAM_COMM_MODE && x.ID_MANU == ID_MANUFACTURER).Count();
                if (ctd > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneCommercial('ERROR','2','');}window.onload=init;</script>");
                }
                
                try
                {
                    comm.ID_MANU = ID_MANUFACTURER;
                    comm.Estado = true;
                    dbo.COMMERCIAL_MODEL.Add(comm);
                    dbo.SaveChanges();
                    id = Convert.ToInt32(comm.ID_COMM_MODE);

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneCommercial('OK','" + id.ToString() + "','" + NAM_COMM_MODE + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneCommercial('ERROR','1','');}window.onload=init;</script>");
                }
            }
        }

        public ActionResult ListarModelosBNV(int idGrupo = 0)
        {
            int IdManu;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "NAM_COMM_MODE")
            {
                IdManu = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                IdManu = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbo.ListarModeloComercialBNV(idGrupo, IdManu, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListadoModelosBNV(int IdGrupo = 0, int IdMarca = 0, int IdModelo = 0)
        {
            var response = dbo.ListadoModelosComerciales(IdGrupo, IdMarca, IdModelo).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoModeloBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var modelo = dbo.ModeloComerxGrupo.FirstOrDefault(x => x.Id == id);
                if (modelo != null)
                {
                    modelo.Estado = Convert.ToBoolean(idEstado);
                    modelo.UsuarioModifica = Convert.ToInt32(Session["UserId"].ToString());
                    modelo.FechaModifica = DateTime.Now;
                    dbo.ModeloComerxGrupo.Attach(modelo);
                    dbo.Entry(modelo).State = EntityState.Modified;
                    dbo.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ListarModeloComercialPorGrupo()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["idGrupo"]);
            int idManu;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "NAM_COMM_MODE")
            {
                idManu = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idManu = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbo.ListarModeloComercialPorGrupo(idManu, idGrupo, idAcco, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
