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
    public class SolucionTIController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();

        //
        // GET: /SolucionTI/

        public static int Id = 0;
        

        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult CrearSolucionTI()
        {
            return View();
        }

        public ActionResult CreaSolucion()
        {
            return View();
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult CreaSolucion(SolucionTI objTI)
        {
            //Validar campos en blanco
            int flag = 0,existe=0;

            if (String.IsNullOrEmpty(objTI.Nombre)) { flag = 1; }
            //if (Convert.ToBoolean(objSolucionTI.Estado) == true) { flag = 2; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            String nom = objTI.Nombre;
            var result = (from at in dbc.SolucionTIs.Where(x => x.Nombre == nom).ToList()
                          select new { id = at.Id, nombre = at.Nombre }).ToList();

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

                objTI.UserIdCreacion = IdUser;
                objTI.Estado = true;
                objTI.FechaCreacion = DateTime.Now;

                dbc.SolucionTIs.Add(objTI);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objTI.Id.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }
        

        //[HttpPost, ValidateInput(false)]
        //public ActionResult CrearSolucionTI(SolucionTI objSolucionTI)
        //{
        //    //Validar campos en blanco
        //    int flag = 0;

        //    if (String.IsNullOrEmpty(objSolucionTI.Nombre)) { flag = 1; }
        //    //if (Convert.ToBoolean(objSolucionTI.Estado) == true) { flag = 2; }

        //    if (flag != 0)
        //    {
        //        return Content("<script type='text/javascript'> function init() {" +
        //                "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
        //    }
           

        //    if (ModelState.IsValid)
        //    {
                
        //        int IdUser = 0;
        //        try
        //        {
        //            IdUser = Convert.ToInt32(Session["UserId"].ToString());
        //        }
        //        catch
        //        {
        //            return Content("<script type='text/javascript'> function init() {" +
        //                "if(top.uploadDone) top.uploadDone('ERROR','2','0');}window.onload=init;</script>");
        //        }

        //        objSolucionTI.UserIdCreacion = IdUser;
        //        objSolucionTI.Estado = true;
        //        objSolucionTI.FechaCreacion = DateTime.Now;
                
        //        dbc.SolucionTIs.Add(objSolucionTI);
        //        dbc.SaveChanges();

        //        return Content("<script type='text/javascript'> function init() {" +
        //            "if(top.uploadDone) top.uploadDone('OK','0','" + objSolucionTI.Id.ToString() + "');}window.onload=init;</script>");
        //    }
        //    else
        //    {
        //        return Content("<script type='text/javascript'> function init() {" +
        //                "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
        //    }
        //}

        public ActionResult ListarTipoSolucionTI()
        {
            //var query = dbc.CTAListarCuentaTipoActivo().ToList();

            var query = (from at in dbc.SolucionTIs select new { at.Id, at.Nombre }).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult BuscarListarSolucionTITodo(int i = 0)
        {
            string Nombre = Request.Params["NOMBRE"].ToString();            
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            string estado = Convert.ToString(Request.Params["ESTADO"]);

            List<ListarBuscarSolucionTI_Result> query = dbc.ListarBuscarSolucionTI(Nombre, estado).ToList();
            
            var query2 = query.Skip(skip).Take(take);
            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult EditarSol(int Id = 0)
        {

            var c = dbc.SolucionTIs.Find(Id);
            ViewBag.Nombre = c.Nombre;
            ViewBag.Estado = c.Estado;

            return View(c);

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult EditarCom(SolucionTI objComp)
        {
            //Validar campos en blanco
            int flag = 0, existe = 0;

            if (Convert.ToString(objComp.Nombre) == null) { flag = 1; }
            //objComp.Nombre.ToString();

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            String nom = objComp.Nombre;
            int idd = objComp.Id;
            var result = (from at in dbc.SolucionTIs.Where(x => x.Nombre == nom && x.Id!=idd).ToList()
                          select new { id = at.Id, nombre = at.Nombre }).ToList();

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


                Convert.ToBoolean(objComp.Estado);
                objComp.UserIdModifica = IdUser;
                objComp.FechaModifica = DateTime.Now;

                dbc.SolucionTIs.Attach(objComp);
                dbc.Entry(objComp).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objComp.Id.ToString() + "');}window.onload=init;</script>");

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }
   }
}
