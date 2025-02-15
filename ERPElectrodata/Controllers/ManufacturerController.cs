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
    public class ManufacturerController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /Manufacturer/

        public ActionResult List()
        {
            //var result = (from x in dbc.MANUFACTURERs.ToList()
            //              select new { 
            //                x.ID_MANU,
            //                x.NAM_MANU
            //              }).OrderBy(x=>x.NAM_MANU);
            var result = dbc.ComboMarca().ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult IndexManufacturer()
        {

            return View();
        }

        public ActionResult ListarMarcas()
        {
            var result = dbc.ListManufacturer().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult CrearManufacturer() //agregado
        //{
        //    return View();
        //}


        //[HttpPost,ValidateInput(false)]
        //public ActionResult CrearManufacturer() //agregado
        //{
        //    return View();
        //}



        public ActionResult CreaMarca()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult CreaMarca(MANUFACTURER objMan)
        {
            int flag = 0, existe = 0;

            if (String.IsNullOrEmpty(objMan.NAM_MANU)) { flag = 1; }
            if (String.IsNullOrEmpty(objMan.DESC_MANU)) { flag = 2; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            String Marca = objMan.NAM_MANU;

            var result = (from at in dbc.MANUFACTURERs.Where(x => x.NAM_MANU == Marca).ToList()
                          select new { id = at.ID_MANU, marca = at.NAM_MANU }).ToList();

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
                objMan.UserIdCreacion = IdUser;
                objMan.FechaCreacion = DateTime.Now;
                dbc.MANUFACTURERs.Add(objMan);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objMan.ID_MANU.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }


        public ActionResult EditaMarca(int Id = 0)
        {
            var c = dbc.MANUFACTURERs.Find(Id);
            ViewBag.Marca = c.NAM_MANU;
            ViewBag.Descripcion = c.DESC_MANU;
            return View(c);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditaMarca(MANUFACTURER objMan)
        {
            //Validar campos en blanco
            int flag = 0, existe = 0;

            if (Convert.ToString(objMan.NAM_MANU) == "") { flag = 1; }
            if (String.IsNullOrEmpty(objMan.DESC_MANU)) { flag = 2; }
            //if (Convert.ToString(objMan.DESC_MANU) == null) { flag = 2; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            String Marca = objMan.NAM_MANU;
            int Codigo = objMan.ID_MANU; //1

            var result = (from at in dbc.MANUFACTURERs.Where(x => x.NAM_MANU == Marca && x.ID_MANU != Codigo).ToList()
                          select new { id = at.ID_MANU, marca = at.NAM_MANU }).ToList();

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

                Convert.ToInt32(objMan.UserIdCreacion); //PROBANDO VALORES QUE SE MANTIENEN EN LA BD
                Convert.ToDateTime(objMan.FechaCreacion); //PROBANDO VALORES QUE SE MANTIENEN EN LA BD

                objMan.UserIdModifica = IdUser;
                objMan.FechaModifica = DateTime.Now;
                objMan.NAM_MANU.ToString();
                objMan.DESC_MANU.ToString();


                dbc.MANUFACTURERs.Attach(objMan);
                dbc.Entry(objMan).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objMan.ID_MANU.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }
   

        public ActionResult BuscarListarMarcasTodo()
        {
            String Marca = Request.Params["MARCA"].ToString();
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            var query = dbc.ListarBuscarManufacturer(Marca).ToList();
            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = query.Count() },JsonRequestBehavior.AllowGet);
        }



        public ActionResult Create(int id = 0, int mant = 0)
        {
            ViewBag.IdGrupo = id;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        // POST: /Manufacturer/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, MANUFACTURER manu)
        {
            string NAM_MANU = Convert.ToString(Request.Form["NAM_MANU"]);
            if (NAM_MANU.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneManufacturer('ERROR','0');}window.onload=init;</script>");
            }
            manu.NAM_MANU = manu.NAM_MANU.ToUpper();

            string DESC_MANU = Convert.ToString(Request.Form["DESC_MANU"]);
            DESC_MANU = DESC_MANU.Replace("&nbsp;", "");
            DESC_MANU = DESC_MANU.Replace("<br />", "");
            if (DESC_MANU.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneManufacturer('ERROR','0');}window.onload=init;</script>");
            }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int userId = Convert.ToInt32(Session["UserId"].ToString());
            int idGrupoAct = Convert.ToInt32(Request.Form["idGrupoAct"]);
            int id;

            if (ID_ACCO == 60 && idGrupoAct != 0)
            {
                try
                {
                    var manufa = dbc.MANUFACTURERs.FirstOrDefault(x => x.NAM_MANU.ToUpper() == manu.NAM_MANU.ToUpper());
                    if (manufa != null)
                    {
                        id = manufa.ID_MANU;
                    }
                    else
                    {
                        manu.UserIdCreacion = userId;
                        manu.FechaCreacion = DateTime.Now;
                        dbc.MANUFACTURERs.Add(manu);
                        dbc.SaveChanges();
                        id = Convert.ToInt32(manu.ID_MANU);
                    }

                    int exis = dbc.MarcaxGrupo.Where(x => x.IdMarca == id && x.IdGrupoActivo == idGrupoAct).Count();
                    if (exis == 0)
                    {
                        MarcaxGrupo marca = new MarcaxGrupo();
                        marca.IdMarca = id;
                        marca.IdGrupoActivo = idGrupoAct;
                        marca.FechaCreacion = DateTime.Now;
                        marca.UsuarioCreacion = Convert.ToInt32(Session["UserId"].ToString());
                        marca.Estado = true;
                        dbc.MarcaxGrupo.Add(marca);
                        dbc.SaveChanges();
                    }

                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDoneManufacturer('OK','" + id.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneManufacturer('ERROR','1');}window.onload=init;</script>");
                }
            }
            else if (ID_ACCO == 55 && idGrupoAct != 0)
            {
                try
                {
                    var manufa = dbc.MANUFACTURERs.FirstOrDefault(x => x.NAM_MANU.ToUpper() == manu.NAM_MANU.ToUpper());
                    if (manufa != null)
                    {
                        id = manufa.ID_MANU;
                    }
                    else
                    {
                        manu.UserIdCreacion = userId;
                        manu.FechaCreacion = DateTime.Now;
                        dbc.MANUFACTURERs.Add(manu);
                        dbc.SaveChanges();
                        id = Convert.ToInt32(manu.ID_MANU);
                    }

                    dbc.InsertarMarcaPorGrupo(id, idGrupoAct, ID_ACCO, userId);

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneManufacturer('OK','" + id.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneManufacturer('ERROR','1');}window.onload=init;</script>");
                }
            }
            else
            {
                int ctd = dbc.MANUFACTURERs.Where(x => x.NAM_MANU == manu.NAM_MANU).Count();
                if (ctd > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneManufacturer('ERROR','2');}window.onload=init;</script>");
                }
                
                try
                {
                    dbc.MANUFACTURERs.Add(manu);
                    dbc.SaveChanges();
                    id = Convert.ToInt32(manu.ID_MANU);

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneManufacturer('OK','" + id.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneManufacturer('ERROR','1');}window.onload=init;</script>");
                }
            }

        }

        public ActionResult ListarMarcasBNV()
        {
            int idGrupo;
            string txt = "";
            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "NAM_MANU")
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) txt = "";
            }
            else
            {
                idGrupo = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = dbc.ListarMarcasBNV(idGrupo, txt).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListadoMarcasBNV(int IdGrupo = 0, int IdMarca = 0)
        {
            var response = dbc.ListadoMarcas(IdGrupo, IdMarca).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoMarcaBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var marca = dbc.MarcaxGrupo.FirstOrDefault(x => x.Id == id);
                if (marca != null)
                {
                    marca.Estado = Convert.ToBoolean(idEstado);
                    marca.UsuarioModifica = Convert.ToInt32(Session["UserId"].ToString());
                    marca.FechaModifica = DateTime.Now;
                    dbc.MarcaxGrupo.Attach(marca);
                    dbc.Entry(marca).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ListarMarcaPorGrupo()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["idGrupo"]);

            var result = dbc.ListarMarcaPorGrupo(idGrupo, idAcco).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
