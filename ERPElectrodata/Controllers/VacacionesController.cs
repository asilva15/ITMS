using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using ERPElectrodata.Objects;
using ERPElectrodata.AppCode;

namespace ERPElectrodata.Controllers
{
    public class VacacionesController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        //
        // GET: /Vacaciones/
        [Authorize]
        public ActionResult MisVacaciones()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.ID_ACCO = ID_ACCO;

            Sesiones objSesion = new Sesiones();

            ViewBag.Nombre = objSesion.SesionNombre;
            ViewBag.Foto = objSesion.SesionFoto;
            string area = Session["ID_AREA"].ToString();

            return View();
            
        }

        public ActionResult MisSolicitudes()
        {
            return View();
        }

        public ActionResult CrearSolicitud(VACATION objVacaciones)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            int IdPersEntiApro = 0;
            int jornada = 0;

            if (IdPersEnti == 1007)
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.uploadDone) top.uploadDone('ERROR','Administrator no registra vacaciones.');}window.onload=init;</script>");

            }
            else
            {
                IdPersEntiApro = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).SingleOrDefault().ID_PERS_ENTI;
            }

            try
            {
                jornada = Convert.ToInt32(Request.Params["Jornada"]);
                DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {

                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar la fecha inicio.');}window.onload=init;</script>");
            }

            try
            {
                DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar la fecha fin.');}window.onload=init;</script>");
            }


            var dateIni = DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var fechaRetorno = DateTime.ParseExact(Request.Params["fechRetorno"], "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (dateFin < dateIni)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','La fecha fin debe ser mayor a la fecha inicial.');}window.onload=init;</script>");
            }

            var existe = dbe.VACATION
                        .Any(x => x.ID_PERS_ENTI == IdPersEnti && x.ID_VACA_STAT != 4 && x.ID_VACA_STAT != 5 &&
                            ((x.DAT_STAR <= dateIni && x.DAT_END >= dateIni) ||
                            (x.DAT_STAR <= dateFin && x.DAT_END >= dateFin) ||
                            (x.DAT_STAR >= dateIni && x.DAT_END <= dateFin)));

            if (existe)
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Ya existe una solicitud para las fechas ingresadas.');}window.onload=init;</script>");

            //if (objVacaciones.SUMMARY == null || objVacaciones.SUMMARY == "")
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //            "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar un comentario');}window.onload=init;</script>");
            //}
            if (jornada == 1 && (Request.Params["txtJornada"] == null || Request.Params["txtJornada"] == ""))
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar su jornada atípica.');}window.onload=init;</script>");
            }

            objVacaciones.DAT_STAR = dateIni;

            TimeSpan d = Convert.ToDateTime(dateFin) - Convert.ToDateTime(dateIni);
            int xxx = d.Days + 1;
            if (jornada == 0)
            {
                if (Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 4
                && (d.Days + 1) == 5)
                {

                    xxx = xxx - 2;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                    //dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(3)

                }
                else if (Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 5
                && (d.Days + 1) == 4)
                {
                    xxx = xxx - 2;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if (Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 5
                && (d.Days + 1) == 5)
                {
                    xxx = xxx - 2;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if (Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 3
                && (d.Days + 1) == 5)
                {
                    xxx = xxx - 2;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if (Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 3
                && Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 6 && (d.Days + 1) <= 5)
                {
                    xxx = xxx - 1;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if (Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 1
               && Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 5)
                {
                    xxx = xxx + 2;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if (Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 1
               && Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 6)
                {
                    xxx = xxx + 1;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if ((d.Days + 1) == 4 && Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_STAR"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 1)
                {
                    xxx = xxx + 2;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if ((d.Days + 1) >= 4 && Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 5)
                {
                    xxx = xxx + 2;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if ((d.Days + 1) >= 4 && Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 6)
                {
                    xxx = xxx + 1;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if ((d.Days + 1) < 4 && Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 6)
                {
                    xxx = xxx - 1;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else if ((d.Days + 1) < 4 && Convert.ToInt32(DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 0)
                {
                    xxx = xxx - 2;
                    dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objVacaciones.DAT_END = dateFin;
                }
                else
                {
                    objVacaciones.DAT_END = dateFin;
                }


            }
            else if (jornada == 1)
            {

                dateFin = DateTime.ParseExact(Request.Params["DAT_END"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                objVacaciones.DAT_END = dateFin;
            }

            TimeSpan dias = Convert.ToDateTime(objVacaciones.DAT_END) - Convert.ToDateTime(objVacaciones.DAT_STAR);

            objVacaciones.NUM_DAYS = xxx;
            objVacaciones.ID_PERS_ENTI = IdPersEnti;
            objVacaciones.DAT_REGI = DateTime.Now;
            objVacaciones.ID_PERS_ENTI_APPR_1 = IdPersEntiApro;
            objVacaciones.DAT_RETU = fechaRetorno;
            objVacaciones.UserId = UserId;
            objVacaciones.ID_VACA_STAT = 1;
            objVacaciones.JornadaAtipica = Convert.ToBoolean(jornada);
            if (jornada == 1)
            {
                objVacaciones.Jornada = Convert.ToString(Request.Params["txtJornada"]);
            }
            else
            {
                objVacaciones.Jornada = "";
            }
            objVacaciones.Reprogramacion = false;
            dbe.VACATION.Add(objVacaciones);
            dbe.SaveChanges();

            SendMail mail = new SendMail();
            mail.EnviarSolicitudVacaciones(objVacaciones.ID_VACA, (int)objVacaciones.ID_PERS_ENTI, (int)objVacaciones.ID_PERS_ENTI_APPR_1, (int)objVacaciones.ID_VACA_STAT);
            string mensaje_OK = "Se guardó correctamente su solicitud de vacaciones,";
            DateTime fecha_actual = DateTime.Now;
            if (fecha_actual.Day >= 25 && fecha_actual.Day <= 31)
                mensaje_OK = mensaje_OK + " Su solicitud será declarada el próximo mes.";

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + mensaje_OK + "');}window.onload=init;</script>");

        }


        [HttpPost]
        public ActionResult CancelarSolicitudVaciones(int id)
        {
            VACATION model = dbe.VACATION.Find(id);

            DateTime fMovimiento = DateTime.Now;

            if (model != null)
            {
                // Realiza los cambio
                model.ID_VACA_STAT = 5;
                model.DAT_REGI_DELE = fMovimiento;
                // Guarda los cambios en la base de datos
                dbe.SaveChanges();

                return Json(new { success = true, message = "Cambios realizados exitosamente" });
            }
            else
            {
                return Json(new { success = false, message = "No se encontró la solicitud de vacaciones" });
            }
        }


        //[HttpPost, ValidateInput(false)]
        [Authorize]
        public ActionResult CancelarSolicitud(int? id = 0)
        {

            ViewBag.Id = id;
            return View();


        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CancelarSolicitud(int id = 0)
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"].ToString());
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
                int jornada = 0;
                if (Request.Form["chkReprogramar"] == "on")
                {
                    try
                    {
                        jornada = Convert.ToInt32(Request.Params["JornadaC"]);
                        DateTime.ParseExact(Request.Params["FechaInicioC"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {

                        return Content("<script type='text/javascript'> function init() {" +
                                 "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar la fecha de inicio.');}window.onload=init;</script>");
                    }

                    try
                    {
                        DateTime.ParseExact(Request.Params["FechaFinC"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {

                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar la fecha fin.');}window.onload=init;</script>");
                    }
                    var dateIni = DateTime.ParseExact(Request.Params["FechaInicioC"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var dateFin = DateTime.ParseExact(Request.Params["FechaFinC"], "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (dateFin < dateIni)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','La fecha fin debe ser mayor a la fecha inicial.');}window.onload=init;</script>");
                    }

                    if (Request.Params["Justificacion"] == null || Request.Params["Justificacion"] == "")
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar una justificación.');}window.onload=init;</script>");
                    }
                    if (jornada == 1 && (Request.Params["txtJornadaC"] == null || Request.Params["txtJornadaC"] == ""))
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar su jornada atípica.');}window.onload=init;</script>");
                    }
                    VACATION objVCT = dbe.VACATION.Where(x => x.ID_VACA == id).SingleOrDefault();

                    objVCT.ID_VACA_STAT = 5;
                    dbe.VACATION.Attach(objVCT);
                    dbe.Entry(objVCT).State = EntityState.Modified;
                    dbe.SaveChanges();

                    SendMail mail = new SendMail();
                    mail.EnviarSolicitudVacaciones(objVCT.ID_VACA, (int)objVCT.ID_PERS_ENTI, (int)objVCT.ID_PERS_ENTI_APPR_1, (int)objVCT.ID_VACA_STAT);

                    //Nueva Solicitud
                    int IdSolicitante = (int)objVCT.ID_PERS_ENTI;

                    VACATION objVacaciones = new VACATION();
                    objVacaciones.DAT_STAR = dateIni;

                    TimeSpan d = Convert.ToDateTime(dateFin) - Convert.ToDateTime(dateIni);

                    //Si Fecha Fin es Viernes y los días de vacaciones es mayor igual a 5
                    if ((d.Days + 1) >= 5
                        && Convert.ToInt32(DateTime.ParseExact(Request.Params["FechaFinC"], "dd/MM/yyyy", CultureInfo.InvariantCulture).DayOfWeek) == 5)
                    {
                        dateFin = DateTime.ParseExact(Request.Params["FechaFinC"], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(2);
                        objVacaciones.DAT_END = dateFin;
                    }
                    else
                    {
                        objVacaciones.DAT_END = dateFin;
                    }

                    TimeSpan dias = Convert.ToDateTime(objVacaciones.DAT_END) - Convert.ToDateTime(objVacaciones.DAT_STAR);

                    objVacaciones.NUM_DAYS = dias.Days + 1;
                    objVacaciones.ID_PERS_ENTI = IdSolicitante;
                    objVacaciones.DAT_REGI = DateTime.Now;
                    objVacaciones.ID_PERS_ENTI_APPR_1 = dbe.EMAIL_BOSS_INMSUP(IdSolicitante).SingleOrDefault().ID_PERS_ENTI;
                    objVacaciones.DAT_RETU = dateFin;
                    objVacaciones.UserId = UserId;
                    objVacaciones.ID_VACA_STAT = 1;
                    objVacaciones.SUMMARY = Request.Params["Justificacion"];
                    objVacaciones.JornadaAtipica = Convert.ToBoolean(jornada);
                    if (jornada == 1)
                    {
                        objVacaciones.Jornada = Convert.ToString(Request.Params["txtJornadaC"]);
                    }
                    else
                    {
                        objVacaciones.Jornada = "";
                    }
                    objVacaciones.Reprogramacion = true;
                    dbe.VACATION.Add(objVacaciones);
                    dbe.SaveChanges();

                    mail.EnviarSolicitudVacaciones(objVacaciones.ID_VACA, (int)objVacaciones.ID_PERS_ENTI, (int)objVacaciones.ID_PERS_ENTI_APPR_1, (int)objVacaciones.ID_VACA_STAT);

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('Reprogramado','0','" + objVCT.ID_VACA.ToString() + "');}window.onload=init;</script>");

                }
                else
                {
                    VACATION objVCT = dbe.VACATION.Where(x => x.ID_VACA == id).SingleOrDefault();

                    objVCT.ID_VACA_STAT = 5;
                    dbe.VACATION.Attach(objVCT);
                    dbe.Entry(objVCT).State = EntityState.Modified;
                    dbe.SaveChanges();
                    try
                    {
                        SendMail mail = new SendMail();
                        mail.EnviarSolicitudVacaciones(objVCT.ID_VACA, (int)objVCT.ID_PERS_ENTI, (int)objVCT.ID_PERS_ENTI_APPR_1, (int)objVCT.ID_VACA_STAT);
                    }
                    catch
                    {

                    }
                    
                    ///
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objVCT.ID_VACA.ToString() + "');}window.onload=init;</script>");
                }
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult AprobarSolicitud(int? id = 0)
        {

            ViewBag.Id = id;
            return View();


        }

        [HttpPost]
        public ActionResult AprobarMultiplesSolicitudesVacaciones(string listaId)
        {
            if(listaId != "")
            {
                string[] elementos = listaId.Split(',');
                List<string> lista = new List<string>(elementos);
                foreach (string elemento in lista)
                {
                    string[] solicitudes = elemento.Split('_');
                    if(solicitudes[0] == "aprobar")
                    {
                        try
                        {
                            int id = Convert.ToInt32(solicitudes[1]);
                            VACATION objVCT = dbe.VACATION.Where(x => x.ID_VACA == id).SingleOrDefault();

                            objVCT.ID_VACA_STAT = 2;
                            objVCT.FechaAprobacion = DateTime.Now;
                            dbe.VACATION.Attach(objVCT);
                            dbe.Entry(objVCT).State = EntityState.Modified;
                            dbe.SaveChanges();

                            try
                            {
                                SendMail mail = new SendMail();
                                mail.EnviarSolicitudVacaciones(objVCT.ID_VACA, (int)objVCT.ID_PERS_ENTI, (int)objVCT.ID_PERS_ENTI_APPR_1, (int)objVCT.ID_VACA_STAT);
                            }
                            catch
                            {

                            }
                            
                        }
                        catch
                        {
                            return Content("ERROR");
                        }
                    }
                    if (solicitudes[0] == "desaprobar")
                    {
                        try
                        {
                            int id = Convert.ToInt32(solicitudes[1]);
                            VACATION objVCT = dbe.VACATION.Where(x => x.ID_VACA == id).SingleOrDefault();

                            objVCT.ID_VACA_STAT = 5;
                            dbe.VACATION.Attach(objVCT);
                            dbe.Entry(objVCT).State = EntityState.Modified;
                            dbe.SaveChanges();

                            try
                            {
                                SendMail mail = new SendMail();
                                mail.EnviarSolicitudVacaciones(objVCT.ID_VACA, (int)objVCT.ID_PERS_ENTI, (int)objVCT.ID_PERS_ENTI_APPR_1, (int)objVCT.ID_VACA_STAT);
                            }
                            catch
                            {

                            }
                        }
                        catch
                        {
                            return Content("ERROR");
                        }
                    }
                    Console.WriteLine(elemento);
                }
                return Content("OK");
            }
            return Content("VACIO");

            //try
            //{

            //    VACATION objVCT = dbe.VACATION.Where(x => x.ID_VACA == id).SingleOrDefault();

            //    objVCT.ID_VACA_STAT = 2;
            //    objVCT.FechaAprobacion = DateTime.Now;
            //    dbe.VACATION.Attach(objVCT);
            //    dbe.Entry(objVCT).State = EntityState.Modified;
            //    dbe.SaveChanges();

            //    SendMail mail = new SendMail();
            //    mail.EnviarSolicitudVacaciones(objVCT.ID_VACA, (int)objVCT.ID_PERS_ENTI, (int)objVCT.ID_PERS_ENTI_APPR_1, (int)objVCT.ID_VACA_STAT);

            //    return Content("<script type='text/javascript'> function init() {" +
            //    "if(top.uploadDone) top.uploadDone('OK','0','" + objVCT.ID_VACA.ToString() + "');}window.onload=init;</script>");
            //}
            //catch
            //{
            //    return Content("ERROR");
            //}
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AprobarSolicitud(int id = 0)
        {
            try
            {

                VACATION objVCT = dbe.VACATION.Where(x => x.ID_VACA == id).SingleOrDefault();

                objVCT.ID_VACA_STAT = 2;
                objVCT.FechaAprobacion = DateTime.Now;
                dbe.VACATION.Attach(objVCT);
                dbe.Entry(objVCT).State = EntityState.Modified;
                dbe.SaveChanges();

                SendMail mail = new SendMail();
                mail.EnviarSolicitudVacaciones(objVCT.ID_VACA, (int)objVCT.ID_PERS_ENTI, (int)objVCT.ID_PERS_ENTI_APPR_1, (int)objVCT.ID_VACA_STAT);

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','0','" + objVCT.ID_VACA.ToString() + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("ERROR");
            }

        }


        public ActionResult DeclararSolicitud(int? id = 0)
        {

            ViewBag.Id = id;
            return View();


        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeclararSolicitud(int id = 0)
        {
            try
            {

                VACATION objVCT = dbe.VACATION.Where(x => x.ID_VACA == id).SingleOrDefault();

                objVCT.ID_VACA_STAT = 6;
                objVCT.FechaAprobacion = DateTime.Now;
                dbe.VACATION.Attach(objVCT);
                dbe.Entry(objVCT).State = EntityState.Modified;
                dbe.SaveChanges();

                SendMail mail = new SendMail();
                mail.EnviarSolicitudVacaciones(objVCT.ID_VACA, (int)objVCT.ID_PERS_ENTI, (int)objVCT.ID_PERS_ENTI_APPR_1, (int)objVCT.ID_VACA_STAT);

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','0','" + objVCT.ID_VACA.ToString() + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult ListarMisVacaciones()
        {
            int sw = 0, Creador = 0, Estado = 0, JefeAprobador = 0, Area = 0;
            string msjError = string.Empty;
            DateTime? FechaInicio = null, FechaFin = null;
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (int.TryParse(Convert.ToString(Request.Params["IdUsuario"]), out Creador) == false)
            {
                Creador = 0;
            }
            if (int.TryParse(Convert.ToString(Request.Params["ID_VACA_STAT"]), out Estado) == false)
            {
                Estado = 0;
            }


            if (Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == null)
            {
                FechaInicio = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"], cultures);
            }

            if (Convert.ToString(Request.QueryString["FechaIngresoFin"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoFin"]) == null)
            {
                FechaFin = null;
            }
            else
            {
                CultureInfo cultures = new CultureInfo("es-ES");
                FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"], cultures);
            }

            if (int.TryParse(Convert.ToString(Request.Params["IdJefeAprobador"]), out JefeAprobador) == false)
            {
                JefeAprobador = 0;
            }

            if (int.TryParse(Convert.ToString(Request.Params["IdArea"]), out Area) == false)
            {
                Area = 0;
            }

            var result = dbe.ListarSolicitudVacacion(Creador, Estado, FechaInicio, FechaFin, JefeAprobador, Area, IdPersEnti).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAreaVacacionNuevo()
        {
            
            
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = (from lsv in dbe.ListarAreaVacacion(IdPersEnti)
             select new
             {
                 lsv.Id_Area,
                 lsv.Area
             }).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAreaVacacion(int id = 0)
        {
            int Creador = 0;
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var result = dbe.ListarAreasVacacion(Creador, 0, ID_PERS_ENTI).ToList();
            return Json(new { Data = result, Count = result.Count(), ID_PERS_ENTI = ID_PERS_ENTI }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarJefeAprobador(int id = 0)
        {
            int Creador = 0;
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var result = dbe.ListarJefeAprobador(Creador, 0, ID_PERS_ENTI).ToList();
            //var result2 = dbe.ejemplo().ToList();
            //var result = dbe.ListadoJefeAprobador(Creador, 0, ID_PERS_ENTI).ToList();
            return Json(new { Data = result, Count = result.Count(), ID_PERS_ENTI = ID_PERS_ENTI }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarCalendarioVacaciones()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = dbe.CalendarioSolicitudVacacion(IdPersEnti).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AlertasVacaciones()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = dbe.AlertaVacacionesPorAprobar(IdPersEnti).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerDocumento(string id = "")
        {
            //ViewBag.Flag = id;
            ViewBag.Documento = id;
            return View();
        }

        public ActionResult ListarEstados()
        {
            var query = dbe.ListarEstadoVacaciones().ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DescargarArchivo(string id = "")
        {
            try
            {
                string ruta = "";

                ruta = "~/Adjuntos/Talent/Vacation/";

                FileStream fileStream = System.IO.File.OpenRead(Server.MapPath(ruta + id));
                MemoryStream storeStream = new MemoryStream();

                storeStream.SetLength(fileStream.Length);
                fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                byte[] byteArray = storeStream.ToArray();

                storeStream.Flush();
                fileStream.Close();
                storeStream.Close();

                Random r = new Random();
                int aleatorio = r.Next(10000, 99999);

                Response.Clear();

                //Response.ContentType = "application/octet-stream";
                if ((id.ToLower()).Contains(".pdf"))
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".txt"))
                {
                    Response.ContentType = "text/text";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".txt");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".jpg"))
                {
                    Response.ContentType = "image/jpeg";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".jpg");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id.ToLower()).Contains(".png"))
                {
                    Response.ContentType = "image/png";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".png");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else
                {
                    string filename = ruta + id;
                    return File(filename, "application/pdf", id);
                }

            }
            catch
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

                return File(stream, "text/plain", "Error.txt");
            }

        }
    }
}
