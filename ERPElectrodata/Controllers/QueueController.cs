using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class QueueController : Controller
    {
        private CoreEntities dbc = new CoreEntities();

        public ActionResult IndexCola()
        {
            return View();
        }

        public ActionResult CreaCola()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreaCola(QUEUE objQUEUE)
        {
            int flag = 0;

            if (String.IsNullOrEmpty(objQUEUE.NAM_QUEU)) { flag = 1; }
            if (String.IsNullOrEmpty(objQUEUE.NAM_QUEU_REPO)) { flag = 2; }
            if (String.IsNullOrEmpty(objQUEUE.DES_QUEU)) { flag = 3; }
            if (String.IsNullOrEmpty(objQUEUE.EMA_QUEU)) { flag = 4; }
            if (objQUEUE.LEV_QUEU == null) { flag = 5; }


            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            objQUEUE.VIG_QUEU = true;

            if (ModelState.IsValid)
            {
                dbc.QUEUEs.Add(objQUEUE);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.uploadDone) top.uploadDone('OK','0','" + objQUEUE.ID_QUEU.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult ListarBuscarColaTodo(int i = 0)
        {
            String Abreviatura = Request.Params["ABREVIATURA"].ToString();
            String Nombre = Request.Params["NOMBRE"].ToString();
            String Estado = Convert.ToString(Request.Params["ESTADO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.ListarBuscarCola(Abreviatura, Nombre, Estado).ToList();
            var query2 = query.Skip(skip).Take(take);
            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditaCola(int Id = 0)
        {
            var c = dbc.QUEUEs.Find(Id);
            ViewBag.Abreviatura = c.NAM_QUEU;
            ViewBag.Nombre = c.NAM_QUEU_REPO;
            ViewBag.Descripcion = c.DES_QUEU;
            ViewBag.Email = c.EMA_QUEU;
            ViewBag.Nivel = c.LEV_QUEU;
            ViewBag.VistaTodo = c.VIS_ALL_QUEU;
            ViewBag.Estado = c.VIG_QUEU;

            return View(c);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditaCola(QUEUE objQUEU)
        {
            //Validar campos en blanco
            int flag = 0;

            if (Convert.ToString(objQUEU.NAM_QUEU) == "") { flag = 1; }
            if (Convert.ToString(objQUEU.NAM_QUEU_REPO) == "") { flag = 2; }
            if (Convert.ToString(objQUEU.DES_QUEU) == "") { flag = 3; }
            if (Convert.ToString(objQUEU.EMA_QUEU) == "") { flag = 4; }
            if (Convert.ToString(objQUEU.LEV_QUEU) == "") { flag = 5; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                Convert.ToBoolean(objQUEU.VIS_ALL_QUEU);
                Convert.ToBoolean(objQUEU.VIG_QUEU);

                QUEUE objColas = dbc.QUEUEs.Find(objQUEU.ID_QUEU);
                objColas.NAM_QUEU = objQUEU.NAM_QUEU;
                objColas.NAM_QUEU_REPO = objQUEU.NAM_QUEU_REPO;
                objColas.DES_QUEU = objQUEU.DES_QUEU;
                objColas.EMA_QUEU = objQUEU.EMA_QUEU;
                objColas.LEV_QUEU = objQUEU.LEV_QUEU;
                objColas.VIS_ALL_QUEU = objQUEU.VIS_ALL_QUEU;
                objColas.VIG_QUEU = objQUEU.VIG_QUEU;

                dbc.Entry(objColas).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objQUEU.NAM_QUEU.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

    }
}
