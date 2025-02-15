using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using db = ERPElectrodata.Models;
using ent = ERPElectrodata.Object;
using System.Text;
using System.Xml;
using System.Collections;

namespace ERPElectrodata.Controllers
{
    public class IdiomaController : Controller
    {
        ArrayList arrIds = ERPElectrodata.Controllers.AccountController.arrIds;
        ArrayList arrLlaves = ERPElectrodata.Controllers.AccountController.arrLlaves;
        ArrayList arrNombres = ERPElectrodata.Controllers.AccountController.arrNombres;
        ArrayList arrTextos = ERPElectrodata.Controllers.AccountController.arrTextos;
        ArrayList arrResultado = ERPElectrodata.Controllers.AccountController.arrResultado;

        private EntityEntities dbe = new EntityEntities();
        ent.Idioma dbIdioma = new ent.Idioma();
        //
        // GET: /Idioma/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaComboIdioma(int id)
        {
            var result = dbe.ListaComboIdioma(id).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult cambioSessionIdioma(int IdIdioma)
        {
            //int idIdioma = Convert.ToInt32(Request.Params["cbxIdioma"]);
            int idIdioma = IdIdioma;
            int idUsuario = Convert.ToInt32(Session["UserId"]);
            IEnumerable<db.CambioIdiomadeUsuario_Result> lista = dbIdioma.CambioIdiomadeUsuario(idUsuario, idIdioma);
            IEnumerable<db.CambioIdiomadeUsuario_Result> resultado = lista.ToList();
            //Cambiar el idIdioma de la sesion
            Session["IdIdioma"] = idIdioma;
            Session["Idioma"] = Convert.ToString(resultado.SingleOrDefault().Nombre);

            string lenguaje = dbe.Idioma.Find(idIdioma).Cultura;

            var cookie = new HttpCookie("_cultureInformation");
            cookie.Value = lenguaje;
            cookie.Expires = DateTime.Now.AddDays(1);

            Response.SetCookie(cookie);

            return Content(Convert.ToString(Session["IdIdioma"]));
        }

        //public string traducir(string llave, int IdIdioma)
        //{
        //    string ret = "";
        //    for (int i = 0; i < arrIds.Count; i++)
        //    {
        //        if (Convert.ToString(arrLlaves[i]) == llave && Convert.ToInt32(arrIds[i]) == IdIdioma)
        //        {
        //            ret = arrTextos[i].ToString();
        //        }
        //    }
        //    return ret;
            
        //}
    }
}
