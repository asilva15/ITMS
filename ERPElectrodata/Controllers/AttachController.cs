using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Data.Entity;
using ERPElectrodata.AppCode;
using System.Text.RegularExpressions;

namespace ERPElectrodata.Controllers
{
    public class AttachController : Controller
    {

        Sesiones objSesiones = new Sesiones();
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /Attach/

        public ActionResult Index()
        {
            return View();
        }

        //Se recibe una transacción Post del Cliente.
        [HttpPost]
        /*[ValidateAntiForgeryToken]*/
        public ActionResult SaveAttachTicket(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                //Recibimos el tipo de archivo adjunto
                var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                //Recibimos código generado para archivo.
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);

                int ID_TYPE_DOCU_ATTA = 0;

                if(!String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                {
                    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);
                }

                //Validamos que por lo menos se hay cargado un archivo adjunto
                if (files != null)
                {
                    //Recorremos todo el listado de archivos adjuntos
                    foreach (var file in files)
                    {
                        try
                        {
                            //Obtenemos nombre de archivo
                            string name_atta = Path.GetFileNameWithoutExtension(file.FileName);
                            //Obtenemos extensión de archivo
                            string extension = Path.GetExtension(file.FileName);
                            
                            //Buscamos para identificar si el archivo enviado ya se ha guardado.
                            int repetido = dbc.ATTACHEDs
                                .Where(x=>x.DELETE_ATTA == false)
                                .Where(x=>x.KEY_ATTA == KEY_ATTA)
                                .Where(x=>x.NAM_ATTA == name_atta)
                                .Where(x => x.EXT_ATTA == extension).Count();

                            //Si el archivo no se ha repetido procedemos a guardarlo.
                            if (repetido == 0)
                            {
                                //Creamos el registro en la base de datos.
                                var ATTA = new ATTACHED();
                                ATTA.NAM_ATTA = name_atta;
                                ATTA.EXT_ATTA = extension;
                                ATTA.CREATE_ATTA = DateTime.Now;
                                ATTA.KEY_ATTA = KEY_ATTA;
                                ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                                ATTA.DELETE_ATTA = false;
                                dbc.ATTACHEDs.Add(ATTA);
                                dbc.SaveChanges();

                                //Guardamos el archivo en la ruta del servidor y se le añade el id (ID_ATTA) generado
                                file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                            }

                            else
                            {
                                return Content("Filename Exist");
                            }
                        }
                        catch
                        {
                            return Content("ERROR");
                        }
                    }
                }

                return Content(ID_TYPE_DOCU_ATTAs);
            }
            catch
            {
                var xx = "Error";
                return Content(xx);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAsyncAttachTicket(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                //var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);
                //int ID_TYPE_DOCU_ATTA = 0;

                //if (String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                //{
                //    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);
                //}
                string filenames = Convert.ToString(Request.Params["fileNames"]);

                if (filenames != null)
                {
                    //foreach (var file in files)
                    //{
                        try
                        {
                            string name_atta = Path.GetFileNameWithoutExtension(filenames);

                            var ATTA = dbc.ATTACHEDs
                                .Where(x => x.KEY_ATTA == KEY_ATTA)
                                .Where(x=>x.ID_INCI == null)
                                .Where(x=>x.DELETE_ATTA == false)
                                .Single(x => x.NAM_ATTA == name_atta);

                            ATTA.DELETE_ATTA = true;

                            try
                            {
                                //if(File.Exi)
                                if (System.IO.File.Exists(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA)))
                                {
                                    System.IO.File.Delete(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA));
                                }
                            }
                            catch
                            {

                            }

                            dbc.Entry(ATTA).State = EntityState.Modified;
                            dbc.SaveChanges();
                            //foreach(var x in Atta)
                            //{
                            
                            //}
                            //var ATTA = new ATTACHED();
                            //ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            //ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ////ATTA.ID_INCI = ticket.ID_TICK;
                            //ATTA.CREATE_ATTA = DateTime.Now;
                            //ATTA.KEY_ATTA = KEY_ATTA;
                            //ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                            //dbc.ATTACHEDs.Add(ATTA);
                            //dbc.SaveChanges();

                            //file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    //}
                }

                return Content("");
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }

        }


        /*Gestión Documentaria*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarAdjunto(IEnumerable<HttpPostedFileBase> files)
        {

              try
            {
            //Recibimos el tipo de archivo adjunto
            var Codigo = Convert.ToString(Request.Params["Codigo"]);
            //Recibimos código generado para archivo.
            var KeyAdjunto = Convert.ToString(Request.Params["KeyAdjunto"]);

            int Codigo_ = 0;

            if (!String.IsNullOrEmpty(Codigo))
            {
                Codigo_ = Convert.ToInt32(Codigo);
            }
            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        //Obtenemos nombre de archivo
                        string name_atta = Path.GetFileNameWithoutExtension(file.FileName);
                        //Obtenemos extensión de archivo
                        string extension = Path.GetExtension(file.FileName);

                        //Buscamos para identificar si el archivo enviado ya se ha guardado.
                        int repetido = dbc.Adjuntoes
                            .Where(x => x.Estado_KeyAdjunto == 0)
                            .Where(x => x.KeyAdjunto == KeyAdjunto)
                            .Where(x => x.Nombre == name_atta)
                            .Where(x => x.Extension == extension).Count();

                        //Si el archivo no se ha repetido procedemos a guardarlo.
                        if (repetido == 0)
                        {
                            //Creamos el registro en la base de datos.
                            var Adjunto = new Adjunto();
                            Adjunto.Nombre = name_atta;
                            Adjunto.Extension = extension;
                            Adjunto.Creado = DateTime.Now;
                            Adjunto.KeyAdjunto = KeyAdjunto;
                            Adjunto.IdTipoDocumento = Codigo_;
                            Adjunto.Estado_KeyAdjunto = 0;
                            Adjunto.IdCuenta = objSesiones.SesionIdCuenta;
                            Adjunto.Activo = true;
                            Adjunto.UsuarioCreacion = objSesiones.SesionIdUsuario;
                            dbc.Adjuntoes.Add(Adjunto);
                            dbc.SaveChanges();

                            //Guardamos el archivo en la ruta del servidor y se le añade el id (ID_ATTA) generado
                            file.SaveAs(Server.MapPath("~/Adjuntos/") + Adjunto.Nombre + "_" + Convert.ToString(Adjunto.Id) + Adjunto.Extension);
                        }

                        else
                        {
                            return Content("Filename Exist");
                        }
                    }
                    catch
                    {
                        return Content("ERROR");
                    }
                }

            }
            return Content(Codigo);
            }
              catch
              {
                  var xx = "Error";
                  return Content(xx);
              }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarAdjunto(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                //var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                var KeyAdjunto = Convert.ToString(Request.Params["KeyAdjunto"]);
                //int ID_TYPE_DOCU_ATTA = 0;

                //if (String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                //{
                //    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);
                //}
                string filenames = Convert.ToString(Request.Params["fileNames"]);

                if (filenames != null)
                {
                    //foreach (var file in files)
                    //{
                    try
                    {
                        string name_atta = Path.GetFileNameWithoutExtension(filenames);

                        var Adjunto = dbc.Adjuntoes
                            .Where(x => x.KeyAdjunto == KeyAdjunto)
                            .Where(x => x.IdGestionDocumento == null)
                            .Where(x => x.Estado_KeyAdjunto == 0)
                            .Single(x => x.Nombre == name_atta);

                        Adjunto.Estado_KeyAdjunto = 1;

                        try
                        {
                            //if(File.Exi)
                            if (System.IO.File.Exists(Server.MapPath("~/Adjuntos/" + Adjunto.Nombre + "_" + Convert.ToString(Adjunto.Id) + Adjunto.Extension)))
                            {
                                System.IO.File.Delete(Server.MapPath("~/Adjuntos/" + Adjunto.Nombre + "_" + Convert.ToString(Adjunto.Id) + Adjunto.Extension));
                            }
                        }
                        catch
                        {

                        }

                        dbc.Entry(Adjunto).State = EntityState.Modified;
                        dbc.SaveChanges();
                        //foreach(var x in Atta)
                        //{

                        //}
                        //var ATTA = new ATTACHED();
                        //ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        //ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ////ATTA.ID_INCI = ticket.ID_TICK;
                        //ATTA.CREATE_ATTA = DateTime.Now;
                        //ATTA.KEY_ATTA = KEY_ATTA;
                        //ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                        //dbc.ATTACHEDs.Add(ATTA);
                        //dbc.SaveChanges();

                        //file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                    }
                    catch
                    {

                    }
                    //}
                }

                return Content("");
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }

        }


        /*Fin*/





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDetailTicket(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);
                int ID_TYPE_DOCU_ATTA = 0;

                if (!String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                {
                    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            string name_atta = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            //Elimina caracteres especiales del documento
                            String doc = Regex.Replace(name_atta, @"[^\w\.@-]", "",
                                                RegexOptions.None, TimeSpan.FromSeconds(1.5));
                            int repetido = dbc.ATTACHEDs
                                .Where(x => x.DELETE_ATTA == false)
                                .Where(x => x.KEY_ATTA == KEY_ATTA)
                                .Where(x => x.NAM_ATTA == name_atta)
                                .Where(x => x.EXT_ATTA == extension).Count();

                            if (repetido == 0)
                            {
                                var ATTA = new ATTACHED();
                                ATTA.NAM_ATTA = doc;
                                ATTA.EXT_ATTA = extension;
                                //ATTA.ID_INCI = ticket.ID_TICK;
                                ATTA.CREATE_ATTA = DateTime.Now;
                                ATTA.KEY_ATTA = KEY_ATTA;
                                ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                                ATTA.DELETE_ATTA = false;
                                dbc.ATTACHEDs.Add(ATTA);
                                dbc.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                            }

                            else
                            {
                                return Content("Filename Exist");
                            }
                        }
                        catch
                        {
                            return Content("ERROR");
                        }
                    }
                }

                return Content(ID_TYPE_DOCU_ATTAs);
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveDetailTicket(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                //var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);
                //int ID_TYPE_DOCU_ATTA = 0;

                //if (String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                //{
                //    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);
                //}
                string filenames = Convert.ToString(Request.Params["fileNames"]);

                if (filenames != null)
                {
                    //foreach (var file in files)
                    //{
                    try
                    {
                        string name_atta = Path.GetFileNameWithoutExtension(filenames);

                        var ATTA = dbc.ATTACHEDs
                            .Where(x => x.KEY_ATTA == KEY_ATTA)
                            .Where(x => x.ID_DETA_INCI == null)
                            .Where(x => x.DELETE_ATTA == false)
                            .Single(x => x.NAM_ATTA == name_atta);

                        ATTA.DELETE_ATTA = true;

                        try
                        {
                            //if(File.Exi)
                            if (System.IO.File.Exists(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA)))
                            {
                                System.IO.File.Delete(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA));
                            }
                        }
                        catch
                        {

                        }

                        dbc.Entry(ATTA).State = EntityState.Modified;
                        dbc.SaveChanges();
                        //foreach(var x in Atta)
                        //{

                        //}
                        //var ATTA = new ATTACHED();
                        //ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        //ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ////ATTA.ID_INCI = ticket.ID_TICK;
                        //ATTA.CREATE_ATTA = DateTime.Now;
                        //ATTA.KEY_ATTA = KEY_ATTA;
                        //ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                        //dbc.ATTACHEDs.Add(ATTA);
                        //dbc.SaveChanges();

                        //file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                    }
                    catch
                    {

                    }
                    //}
                }

                return Content("");
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdjuntarArchivosOP(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);
                var ID_DOCU_SALE = Convert.ToInt32(Request.Params["ID_DOCU_SALE"]);
                int ID_TYPE_DOCU_ATTA = 0;

                int UserId = Convert.ToInt32(Session["UserId"]);

                var FechaActa = "";
                DateTime DateFechaActa = DateTime.Now;

                if (!String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                {
                    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);

                    if (ID_TYPE_DOCU_ATTA == 1||ID_TYPE_DOCU_ATTA == 22)
                    {
                        FechaActa = Convert.ToString(Request.Params["FechaActa"]);
                        DateFechaActa = Convert.ToDateTime(FechaActa);
                    }
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            string name_atta = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);

                            int repetido = dbc.ATTACHEDs
                                .Where(x => x.DELETE_ATTA == false)
                                .Where(x => x.KEY_ATTA == KEY_ATTA)
                                .Where(x => x.NAM_ATTA == name_atta)
                                .Where(x => x.EXT_ATTA == extension).Count();

                            if (repetido == 0)
                            {
                                var ATTA = new ATTACHED();
                                ATTA.NAM_ATTA = name_atta;
                                ATTA.EXT_ATTA = extension;
                                //ATTA.ID_INCI = ticket.ID_TICK;
                                ATTA.ID_DOCU_SALE = ID_DOCU_SALE;
                                ATTA.CREATE_ATTA = DateTime.Now;
                                ATTA.KEY_ATTA = KEY_ATTA;
                                ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                                ATTA.DELETE_ATTA = false;
                                ATTA.FechaActaConformidad = DateFechaActa;
                                ATTA.UserId = UserId;
                                ATTA.Indicador = 1;
                                dbc.ATTACHEDs.Add(ATTA);
                                dbc.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);

                                if (ID_TYPE_DOCU_ATTA == 22)
                                {
                                    dbc.AlertaDocumentosOP(ID_DOCU_SALE, ID_TYPE_DOCU_ATTA, UserId);
                                }
                            }

                            else
                            {
                                return Content("Filename Exist");
                            }
                        }
                        catch
                        {
                            return Content("ERROR");
                        }
                    }
                }

                return Content(ID_TYPE_DOCU_ATTAs);
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverArchivosOP(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);
                var ID_DOCU_SALE = Convert.ToInt32(Request.Params["ID_DOCU_SALE"]);

                string filenames = Convert.ToString(Request.Params["fileNames"]);

                if (filenames != null)
                {
                    try
                    {
                        string name_atta = Path.GetFileNameWithoutExtension(filenames);

                        var ATTA = dbc.ATTACHEDs
                            .Where(x => x.KEY_ATTA == KEY_ATTA)
                            .Where(x => x.ID_DOCU_SALE == ID_DOCU_SALE)
                            .Where(x => x.ID_DETA_INCI == null)
                            .Where(x => x.DELETE_ATTA == false)
                            .Single(x => x.NAM_ATTA == name_atta);

                        ATTA.DELETE_ATTA = true;

                        try
                        {
                            if (System.IO.File.Exists(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA)))
                            {
                                System.IO.File.Delete(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA));
                            }
                        }
                        catch
                        {

                        }

                        dbc.Entry(ATTA).State = EntityState.Modified;
                        dbc.SaveChanges();
                    }
                    catch
                    {

                    }
                }

                return Content("");
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }

        }

        #region "Edicion de Tickets"
        //Se recibe una transacción Post del Cliente.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSaveAttachTicket(IEnumerable<HttpPostedFileBase> archivos)
        {
            try
            {
                //Recibimos el tipo de archivo adjunto
                var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                //Recibimos código generado para archivo.
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);

                int ID_TYPE_DOCU_ATTA = 0;

                if (!String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                {
                    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);
                }
                //
                //Validamos que por lo menos se hay cargado un archivo adjunto
                if (archivos != null)
                {
                    //Recorremos todo el listado de archivos adjuntos
                    foreach (var file in archivos)
                    {
                        try
                        {
                            //Obtenemos nombre de archivo
                            string name_atta = Path.GetFileNameWithoutExtension(file.FileName);
                            //Obtenemos extensión de archivo
                            string extension = Path.GetExtension(file.FileName);

                            //Buscamos para identificar si el archivo enviado ya se ha guardado.
                            int repetido = dbc.ATTACHEDs
                                .Where(x => x.DELETE_ATTA == false)
                                .Where(x => x.KEY_ATTA == KEY_ATTA)
                                .Where(x => x.NAM_ATTA == name_atta)
                                .Where(x => x.EXT_ATTA == extension).Count();

                            //Si el archivo no se ha repetido procedemos a guardarlo.
                            if (repetido == 0)
                            {
                                String doc = Regex.Replace(name_atta, @"[^\w\.@-]", "",
                                                RegexOptions.None, TimeSpan.FromSeconds(1.5));
                                //Creamos el registro en la base de datos.
                                var ATTA = new ATTACHED();
                                ATTA.NAM_ATTA = doc;
                                ATTA.EXT_ATTA = extension;
                                ATTA.CREATE_ATTA = DateTime.Now;
                                ATTA.KEY_ATTA = KEY_ATTA;
                                ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                                ATTA.DELETE_ATTA = false;
                                dbc.ATTACHEDs.Add(ATTA);
                                dbc.SaveChanges();

                                //Guardamos el archivo en la ruta del servidor y se le añade el id (ID_ATTA) generado
                                file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                            }

                            else
                            {
                                return Content("Filename Exist");
                            }
                        }
                        catch
                        {
                            return Content("ERROR");
                        }
                    }
                }

                return Content(ID_TYPE_DOCU_ATTAs);
            }
            catch
            {
                var xx = "Error";
                return Content(xx);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRemoveAsyncAttachTicket(IEnumerable<HttpPostedFileBase> archivos)
        {
            try
            {
                //var ID_TYPE_DOCU_ATTAs = Convert.ToString(Request.Params["ID_TYPE_DOCU_ATTA"]);
                var KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA"]);
                //int ID_TYPE_DOCU_ATTA = 0;

                //if (String.IsNullOrEmpty(ID_TYPE_DOCU_ATTAs))
                //{
                //    ID_TYPE_DOCU_ATTA = Convert.ToInt32(ID_TYPE_DOCU_ATTAs);
                //}
                string filenames = Convert.ToString(Request.Params["fileNames"]);

                if (filenames != null)
                {
                    //foreach (var file in files)
                    //{
                    try
                    {
                        string name_atta = Path.GetFileNameWithoutExtension(filenames);

                        var ATTA = dbc.ATTACHEDs
                            .Where(x => x.KEY_ATTA == KEY_ATTA)
                            .Where(x => x.ID_INCI == null)
                            .Where(x => x.DELETE_ATTA == false)
                            .Single(x => x.NAM_ATTA == name_atta);

                        ATTA.DELETE_ATTA = true;

                        try
                        {
                            //if(File.Exi)
                            if (System.IO.File.Exists(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA)))
                            {
                                System.IO.File.Delete(Server.MapPath("~/Adjuntos/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA));
                            }
                        }
                        catch
                        {

                        }

                        dbc.Entry(ATTA).State = EntityState.Modified;
                        dbc.SaveChanges();
                        //foreach(var x in Atta)
                        //{

                        //}
                        //var ATTA = new ATTACHED();
                        //ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        //ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ////ATTA.ID_INCI = ticket.ID_TICK;
                        //ATTA.CREATE_ATTA = DateTime.Now;
                        //ATTA.KEY_ATTA = KEY_ATTA;
                        //ATTA.ID_TYPE_DOCU_ATTA = ID_TYPE_DOCU_ATTA;
                        //dbc.ATTACHEDs.Add(ATTA);
                        //dbc.SaveChanges();

                        //file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                    }
                    catch
                    {

                    }
                    //}
                }

                return Content("");
            }
            catch
            {
                var xx = "";// Convert.ToString(codeID);
                return Content(xx);
            }

        }
        #endregion
    }
}
