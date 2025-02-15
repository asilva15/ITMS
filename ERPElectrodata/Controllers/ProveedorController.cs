using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ProveedorController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();

        //
        // GET: /Proveedor/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.PrvListar(IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Nuevo()
        {
            return View();
        }

        // POST: /Manufacturer/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Nuevo(IEnumerable<HttpPostedFileBase> files, Proveedor objProveedor)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (String.IsNullOrEmpty(objProveedor.Nombre))
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneProveedor) top.uploadDoneProveedor('ERROR','0');}window.onload=init;</script>");
            }

            var queryProveedor = dbc.Proveedors.Where(Pr => (Pr.Nombre == objProveedor.Nombre) && Pr.IdAcco == ID_ACCO).Count();
            if (queryProveedor != 0)
            {
                return Content("<script>" +
                    "top.uploadDoneProveedor('ERROR','1');" +
                    "</script>");

            }
            objProveedor.Estado = true;
            objProveedor.IdAcco = ID_ACCO;
            objProveedor.Ruc = objProveedor.Ruc;
            objProveedor.Direccion = objProveedor.Direccion;
            objProveedor.CelularContacto = objProveedor.CelularContacto;
            objProveedor.Contacto = objProveedor.Contacto;
            objProveedor.EmailContacto = objProveedor.EmailContacto;
            objProveedor.UserIdCreacion = UserId;
            objProveedor.FechaCreacion = DateTime.Now;
            string DescripcionProv = Convert.ToString(Request.Form["DescripcionProv"]);
            objProveedor.Descripcion = DescripcionProv;

            try
            {
                dbc.Proveedors.Add(objProveedor);
                dbc.SaveChanges();
                int id = Convert.ToInt32(objProveedor.Id);

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneProveedor) top.uploadDoneProveedor('OK','" + id.ToString() + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneProveedor) top.uploadDoneProveedor('ERROR','2');}window.onload=init;</script>");
            }
        }

    }
}
