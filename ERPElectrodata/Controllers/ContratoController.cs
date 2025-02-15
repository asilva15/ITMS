using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class ContratoController : Controller
    {

        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /Contrato/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.Contratoes.Where(c => c.Estado == true && c.ID_ACCO == ID_ACCO);

            var result = (from c in query.ToList()
                          select new
                          {
                              c.Id,
                              c.Nombre
                          }).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Nuevo(int id = 0, int mant = 0)
        {
            ViewBag.IdGrupo = id;
            ViewBag.Mantenimiento = mant;
            return View();
        }

        // POST: /Manufacturer/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Nuevo(IEnumerable<HttpPostedFileBase> files, Contrato objContrato)
        {
            if (String.IsNullOrEmpty(objContrato.Nombre))
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneContrato('ERROR','0');}window.onload=init;</script>");
            }

            objContrato.Nombre = objContrato.Nombre.ToUpper();
            
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int idGrupoAct = Convert.ToInt32(Request.Form["idGrupoAct"]);
            int id;

            if (ID_ACCO == 60 && idGrupoAct != 0)
            {
                try
                {
                    var contrato = dbc.Contratoes.FirstOrDefault(x => x.Nombre == objContrato.Nombre && x.ID_ACCO == ID_ACCO);
                    if (contrato != null)
                    {
                        id = contrato.Id;
                    }
                    else
                    {
                        objContrato.Estado = true;
                        objContrato.ID_ACCO = ID_ACCO;
                        objContrato.UsuarioCrea = UserId;
                        objContrato.FechaCrea = DateTime.Now;

                        dbc.Contratoes.Add(objContrato);
                        dbc.SaveChanges();
                        id = Convert.ToInt32(objContrato.Id);
                    }

                    int exis = dbc.ContratoxGrupo.Where(x => x.IdContrato == id && x.IdGrupoActivo == idGrupoAct).Count();
                    if (exis == 0)
                    {
                        ContratoxGrupo contra = new ContratoxGrupo();
                        contra.IdContrato = id;
                        contra.IdGrupoActivo = idGrupoAct;
                        contra.FechaCreacion = DateTime.Now;
                        contra.UsuarioCreacion = UserId;
                        contra.Estado = true;
                        dbc.ContratoxGrupo.Add(contra);
                        dbc.SaveChanges();
                    }

                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDoneContrato('OK','" + id.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneContrato('ERROR','1');}window.onload=init;</script>");
                }
            }
            else if (ID_ACCO == 55 && idGrupoAct != 0)
            {
                try
                {
                    var contrato = dbc.Contratoes.FirstOrDefault(x => x.Nombre == objContrato.Nombre && x.ID_ACCO == ID_ACCO);
                    if (contrato != null)
                    {
                        id = contrato.Id;
                    }
                    else
                    {
                        objContrato.Estado = true;
                        objContrato.ID_ACCO = ID_ACCO;
                        objContrato.UsuarioCrea = UserId;
                        objContrato.FechaCrea = DateTime.Now;

                        dbc.Contratoes.Add(objContrato);
                        dbc.SaveChanges();
                        id = Convert.ToInt32(objContrato.Id);
                    }

                    dbc.InsertarContratoPorGrupo(id, idGrupoAct, ID_ACCO, UserId);

                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneContrato('OK','" + id.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneContrato('ERROR','1');}window.onload=init;</script>");
                }
            }
            else
            {
                int ctd = dbc.Contratoes.Where(x => x.Nombre == objContrato.Nombre && x.ID_ACCO == ID_ACCO).Count();
                if (ctd > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneContrato('ERROR','2');}window.onload=init;</script>");
                }
                

                objContrato.Estado = true;
                objContrato.ID_ACCO = ID_ACCO;
                objContrato.UsuarioCrea = UserId;
                objContrato.FechaCrea = DateTime.Now;

                try
                {
                    dbc.Contratoes.Add(objContrato);
                    dbc.SaveChanges();
                    id = Convert.ToInt32(objContrato.Id);

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDoneContrato('OK','" + id.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneContrato('ERROR','1');}window.onload=init;</script>");
                }
            }
        }

        public ActionResult ListadoContratosBNV(int IdGrupo = 0, int IdContrato = 0)
        {
            var response = dbc.ListadoContratos(IdGrupo, IdContrato).ToList();
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificarEstadoContratoBNV(int id = 0, int idEstado = 0)
        {
            try
            {
                var contra = dbc.ContratoxGrupo.FirstOrDefault(x => x.Id == id);
                if (contra != null)
                {
                    contra.Estado = Convert.ToBoolean(idEstado);
                    contra.UsuarioModifica = Convert.ToInt32(Session["UserId"].ToString());
                    contra.FechaModifica = DateTime.Now;
                    dbc.ContratoxGrupo.Attach(contra);
                    dbc.Entry(contra).State = EntityState.Modified;
                    dbc.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ListarContratoPorGrupo()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idGrupo = Convert.ToInt32(Request.Params["idGrupo"]);

            var result = dbc.ListarContratoPorGrupo(idGrupo, idAcco).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
