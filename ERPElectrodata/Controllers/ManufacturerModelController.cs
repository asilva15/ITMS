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
    public class ManufacturerModelController : Controller
    {
        public CoreEntities dbo = new CoreEntities();
        //
        // GET: /ManufacturerModel/

        public ActionResult List()
        {
            int ID_COMM_MODE;
            string txt = "";
            string FIELD = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (FIELD == "NAM_MANU_MODE")
            {
                ID_COMM_MODE = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null) { txt = ""; }
            }
            else
            {
                ID_COMM_MODE = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var query = dbo.MANUFACTURER_MODEL.Where(mm => mm.ID_MANU == ID_COMM_MODE && mm.NAM_MANU_MODE.Contains(txt));

            var result = (from x in query.ToList()
                              // orderby x.NAM_MANU_MODE
                          select new
                          {
                              x.ID_MANU_MODE,
                              x.NAM_MANU_MODE
                          });//.OrderBy(x=>x.NAM_MANU_MODE);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexMantManufacturerModel()
        {
            return View();
        }

        public ActionResult ListarManufacturerCombo()
        {
            //var query = dbc.CTAListarCuentaTipoActivo().ToList();

            var query = (from at in dbo.MANUFACTURERs select new { at.ID_MANU, at.NAM_MANU }).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarBuscarManufacturerModelTodo()
        {
            int Marca=Convert.ToInt32( Request.Params["ID_MANU"].ToString());
            String Modelo=Request.Params["NAM_MANU_MODE"].ToString();
            var query = dbo.ListarBuscarManufacturerModel(Marca,Modelo).ToList();
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            var query2 = query.Skip(skip).Take(take);
            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreaManufacturerModel()
        {
            return View();
        }

        [HttpPost,ValidateInput(false)]
        public ActionResult CreaManufacturerModel(MANUFACTURER_MODEL objManMod)
        {
            int flag = 0, existe = 0;
            String Marca = Convert.ToString(Request.Params["ID_MANUCREA"]);

            if (String.IsNullOrEmpty(Marca)) { flag = 1; }
            if (String.IsNullOrEmpty(objManMod.NAM_MANU_MODE)) { flag = 2; }
            if (String.IsNullOrEmpty(objManMod.DESC_MANU_MODE)) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            objManMod.ID_MANU = Convert.ToInt32(Marca.ToString());
            String Modelo = objManMod.NAM_MANU_MODE;
            var result = (from at in dbo.MANUFACTURER_MODEL.Where(x => x.NAM_MANU_MODE == Modelo && x.ID_MANU == objManMod.ID_MANU).ToList()
                          select new { id = at.ID_MANU, marca = at.NAM_MANU_MODE }).ToList();
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
                objManMod.UserIdCreacion = IdUser;
                objManMod.FechaCreacion = DateTime.Now;
                dbo.MANUFACTURER_MODEL.Add(objManMod);
                dbo.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objManMod.ID_MANU.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }

        }


        public ActionResult EditarManufacturerModel(int Id=0)
        {
            var c = dbo.MANUFACTURER_MODEL.Find(Id);
            var d = dbo.MANUFACTURERs.Find(c.ID_MANU);
            ViewBag.Marca = d.NAM_MANU;
            ViewBag.Modelo = c.NAM_MANU_MODE;
            ViewBag.Descripcion = c.DESC_MANU_MODE;
            return View(c);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult EditarManufacturerModel(MANUFACTURER_MODEL objManModel)
        {

            int flag = 0, existe = 0;

            if (Convert.ToString(objManModel.ID_MANU) == "") { flag = 1; }

            if (Convert.ToString(objManModel.NAM_MANU_MODE) == null) { flag = 2; }

            if (Convert.ToString(objManModel.DESC_MANU_MODE) == null) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            int Codigo = objManModel.ID_MANU_MODE;
            String Modelo = objManModel.NAM_MANU_MODE;
            var result = (from at in dbo.MANUFACTURER_MODEL.Where(x => x.NAM_MANU_MODE == Modelo && x.ID_MANU == objManModel.ID_MANU
                                                                  && x.ID_MANU_MODE!=Codigo).ToList()
                          select new { id = at.ID_MANU, marca = at.NAM_MANU_MODE }).ToList();
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

                //Convert.ToInt32(objManModel.UserIdCreacion); //PROBANDO VALORES QUE SE MANTIENEN EN LA BD
                //Convert.ToDateTime(objManModel.FechaCreacion); //PROBANDO VALORES QUE SE MANTIENEN EN LA BD

                //objManModel.ID_MANU.ToString();//PROBANDO VALORES QUE SE MANTIENEN EN LA BD
                //objManModel.NAM_MANU_MODE.ToString();//PROBANDO VALORES QUE SE MANTIENEN EN LA BD
                //objManModel.DESC_MANU_MODE.ToString();//PROBANDO VALORES QUE SE MANTIENEN EN LA BD

                objManModel.UserIdModifica = IdUser;
                objManModel.FechaModifica = DateTime.Now;
                dbo.MANUFACTURER_MODEL.Attach(objManModel);
                dbo.Entry(objManModel).State = EntityState.Modified;
                dbo.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objManModel.ID_MANU_MODE.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }
        public ActionResult Create(int id = 0)
        {
            ViewBag.ID_MANUFACTURER = id;
            return View();
        }

        // POST: /CommercialModel/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(MANUFACTURER_MODEL manumo)
        {
            string NAM_MANU_MODE = Convert.ToString(Request.Form["NAM_MANU_MODE"]);

            if (NAM_MANU_MODE.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneManufacturerModel('ERROR','0','');}window.onload=init;</script>");
            }

            int ID_MANUFACTURER = Convert.ToInt32(Request.Form["ID_MANU_HF"]);
            manumo.NAM_MANU_MODE = manumo.NAM_MANU_MODE.ToUpper();
            int ctd = dbo.MANUFACTURER_MODEL.Where(x => x.NAM_MANU_MODE == manumo.NAM_MANU_MODE && x.ID_MANU == ID_MANUFACTURER).Count();
            if (ctd > 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneManufacturerModel('ERROR','2','');}window.onload=init;</script>");
            }

            string DESC_MANU_MODE = Convert.ToString(Request.Form["DESC_MANU_MODE"]);
            if (DESC_MANU_MODE.Trim().Length == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneManufacturerModel('ERROR','0');}window.onload=init;</script>");
            }

            try
            {
                manumo.ID_MANU = ID_MANUFACTURER;
                dbo.MANUFACTURER_MODEL.Add(manumo);
                dbo.SaveChanges();
                int id = Convert.ToInt32(manumo.ID_MANU_MODE);

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDoneManufacturerModel('OK','" + id.ToString() + "','" + NAM_MANU_MODE + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneManufacturerModel('ERROR','1','');}window.onload=init;</script>");
            }
        }
    }
}
