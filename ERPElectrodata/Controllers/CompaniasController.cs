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
    public class CompaniasController : Controller
    {

        //
        // GET: /Companias/
        //private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexCompania()
        {
            return View();
        }

        public ActionResult CreaCompania()
        {
            return View();
        }

        [HttpPost,ValidateInput(false) ]
        public ActionResult CreaCompania(CLASS_ENTITY objCom)
        {
            //Validar campos en blanco
            int flag = 0, existe1 = 0, existe2=0;

            if (String.IsNullOrEmpty(objCom.COM_NAME)) { flag = 1; }
            if (String.IsNullOrEmpty(objCom.NUM_TYPE_DI)) { flag = 2; }
            if (String.IsNullOrEmpty(objCom.TEL_ENTI)) { flag = 3; }
            if (String.IsNullOrEmpty(objCom.ADDRESS)) { flag = 4; }
            if (String.IsNullOrEmpty(objCom.ID_SIDIGE)) { flag = 5; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            objCom.ID_TYPE_ENTI = 1;
            objCom.ID_TYPE_DI = 4;
            objCom.VIG_ENTI = true;
            objCom.CREATED = DateTime.Now;

            String RucCom = objCom.NUM_TYPE_DI;
            String CodRuc = objCom.ID_SIDIGE;

            var result1 = (from at in dbe.CLASS_ENTITY.Where(x => x.NUM_TYPE_DI == RucCom ).ToList()
                          select new { id = at.ID_ENTI, codruc = at.NUM_TYPE_DI }).ToList();

            var result2 = (from at in dbe.CLASS_ENTITY.Where(x => x.ID_SIDIGE == CodRuc).ToList()
                           select new { id = at.ID_ENTI, codruc = at.ID_SIDIGE }).ToList();

            if (result1.Count() > 0)
            {
                existe1 = 1;
            }

            if (result2.Count() > 0)
            {
                existe2 = 1;
            }

            

            if (existe1 != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE1','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (existe2 != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE2','" + flag.ToString() + "','0');}window.onload=init;</script>");
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

                objCom.ID_USER = IdUser;
                
                ///////////////////////////////VALORES NULOS////////////////
                //objCom.ID_ENTI = 1;
                //objCom.FIR_NAME = "";
                //objCom.LAS_NAME = "";
                //objCom.MOT_NAME = "";
                //objCom.SEC_NAME = "";
                //objCom.THI_NAME = "";
                //objCom.SEX_ENTI = "";
                //objCom.CEL_ENTI = "";
                //objCom.RPM_ENTI = "";
                //objCom.EXT_ENTI = "";
                //objCom.EMA_ENTI = "";
                //objCom.UserId = 34;
                //objCom.ID_CIVI_STAT = null;
                //objCom.BIRTHDAY = null;
                //objCom.EXPIRE_DATE_DNI = null;
                //objCom.EMISSION_DATE_DNI = null;
                //objCom.CONTACT_NAME = "";
                //objCom.CONTACT_PHONE = "";
                //objCom.PHONE_HOUSE = "";
                //objCom.ID_DEGR_INST = null;
                //objCom.ID_BLOO_GROU = null;
                //objCom.ID_NATI = null;
                //objCom.ID_UBIG = null;
                //objCom.NUM_ID_SIDIGE = null;
                //objCom.LAS_ACCE = null;
                //objCom.PEN_ACCE = null;
                //objCom.ON_LINE = null;
                //objCom.ID_FOTO = null;
                //objCom.IdProfesion = 2;
                //objCom.UserIdModifica = null;
                //objCom.FechaModifica = null;
                //////////////////////////////////////////////////////
                dbe.CLASS_ENTITY.Add(objCom);
                dbe.SaveChanges();
                
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objCom.ID_USER.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult BuscarListarCompaniaTodo(int i=0)
        {
            String Compania = Request.Params["COMPANIA"].ToString();
            String Ruc = Request.Params["RUC"].ToString();
            String Vigencia=Convert.ToString(Request.Params["VIGENCIA"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbe.ListarBuscarCompania(Compania,Ruc,Vigencia).ToList();
            var query2 = query.Skip(skip).Take(take);
            return Json(new {Data= query2, Cantidad= query.Count() },JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditaCompania(int Id= 0)
        {
            var c = dbe.CLASS_ENTITY.Find(Id);
            ViewBag.Compania = c.COM_NAME;
            ViewBag.Ruc = c.NUM_TYPE_DI;
            ViewBag.CodRuc = c.ID_SIDIGE;
            ViewBag.Vigencia = c.VIG_ENTI;
            ViewBag.Telefono = c.TEL_ENTI;
            ViewBag.Direccion = c.ADDRESS;
           

            return View(c);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditaCompania(CLASS_ENTITY objComp)
        {
            //Validar campos en blanco
            int flag = 0,existe1=0,existe2=0;

            if (String.IsNullOrEmpty(objComp.COM_NAME)) { flag = 1; }
            if (String.IsNullOrEmpty(objComp.NUM_TYPE_DI)) { flag = 2; }
            if (String.IsNullOrEmpty(objComp.TEL_ENTI)) { flag = 3; }
            if (String.IsNullOrEmpty(objComp.ADDRESS)) { flag = 4; }
            if (String.IsNullOrEmpty(objComp.ID_SIDIGE)) { flag = 5; }
            
            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            String RucCom = objComp.NUM_TYPE_DI;
            String CodRuc = objComp.ID_SIDIGE;
            int idd = objComp.ID_ENTI;

            var result1 = (from at in dbe.CLASS_ENTITY.Where(x => x.NUM_TYPE_DI == RucCom && x.ID_ENTI != idd).ToList()
                           select new { id = at.ID_ENTI, codruc = at.NUM_TYPE_DI }).ToList();

            var result2 = (from at in dbe.CLASS_ENTITY.Where(x => x.ID_SIDIGE == CodRuc && x.ID_ENTI != idd).ToList()
                           select new { id = at.ID_ENTI, codruc = at.ID_SIDIGE }).ToList();
            if (result1.Count() > 0)
            {
                existe1 = 1;
            }

            if (result2.Count() > 0)
            {
                existe2 = 1;
            }



            if (existe1 != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE1','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (existe2 != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE2','" + flag.ToString() + "','0');}window.onload=init;</script>");
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

                Convert.ToBoolean(objComp.VIG_ENTI);
                objComp.UserIdModifica = IdUser;
                objComp.FechaModifica = DateTime.Now;

                dbe.CLASS_ENTITY.Attach(objComp);
                dbe.Entry(objComp).State = EntityState.Modified;
                dbe.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objComp.ID_USER.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }
    }
}
