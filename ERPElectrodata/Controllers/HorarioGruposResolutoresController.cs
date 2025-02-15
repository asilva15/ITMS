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


namespace ERPElectrodata.Controllers
{
    public class HorarioGruposResolutoresController : Controller
    {
        //
        // GET: /HorarioGruposResolutores/
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        [Authorize]
        public ActionResult GrupoResolutorHorarios()
        {
            return View();
        }

        [Authorize]
        public ActionResult RegistroHorario()
        {
           
                return View();
           
        }

        [Authorize]
        public ActionResult DetalleHorario(int id = 0)
        {
            var query1 = (from s in dbe.GrupoHorario
                          select s).ToList();
            var query2 = (from c in dbc.QUEUEs
                          select c).ToList();
            var nombreGResolutor = (from sp in query1
                                    join ds in query2
                                 on sp.IdGrupo equals ds.ID_QUEU
                                    where sp.Id == id
                                    select ds).FirstOrDefault().NAM_QUEU_REPO;

                ViewBag.IdHorario = id;
                ViewBag.AreaResponsable = nombreGResolutor;
                return View();
         
        }

        // Detalle Horarios
        public ActionResult ObtenerDetalleHorarios(int id = 0)
        {
            var IdGrupo =dbe.GrupoHorario.Where(x => x.Id == id).FirstOrDefault().IdGrupo;
            var query = dbe.ActObtDetalleHorarios(IdGrupo).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarResolutoresComboBox()
        {
            
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.QUEUEs.Where(x => x.VIG_QUEU == true);
            var query1 = dbe.GrupoHorario.Where(x => x.Estado == true).ToList();
            var query2 = dbc.ACCOUNT_QUEUE.Where(x => x.VIG_ACCO_QUEU == true).ToList();
            var result = (from x in query.ToList()
                          join y in query2
                                 on x.ID_QUEU equals y.ID_QUEU
                          where !(query1.Any(item2 => item2.IdGrupo == x.ID_QUEU)) && y.ID_ACCO == IdAcco  
                          select new
                          {
                              ID_QUEU = x.ID_QUEU,
                              NAME_QUEU = (x.NAM_QUEU_REPO == null ? "" : x.NAM_QUEU_REPO.ToUpper()),
                          });
            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult CrearSolicitud(GrupoHorario objResolutores)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            bool v = Convert.ToBoolean(Request.Params["IsActive"]);
            try
            {
                Convert.ToInt32(Request.Params["cbResolutor"].ToString());
            }
            catch
            {

                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar una Área Responsable.');}window.onload=init;</script>");
            }
            try
            {
                // jornada = Convert.ToInt32(Request.Params["Jornada"]);
                DateTime.ParseExact(Request.Params["HoraInicio"], "hh:mm tt", CultureInfo.InvariantCulture);
            }
            catch
            {

                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar la Hora inicio.');}window.onload=init;</script>");
            }

            try
            {
                DateTime.ParseExact(Request.Params["HoraFin"], "hh:mm tt", CultureInfo.InvariantCulture);
            }
            catch
            {

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Debe ingresar la Hora fin.');}window.onload=init;</script>");
            }
            //var intervalString = Request.Params["HoraInicio"];
            //var intervalString1 = "9:00 AM";
            //var format = "h:mm tt";
            int ResolutorId = Convert.ToInt32(Request.Params["cbResolutor"].ToString());
            TimeSpan HoraIni = DateTime.ParseExact(Request.Params["HoraInicio"], "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            TimeSpan HoraFin = DateTime.ParseExact(Request.Params["HoraFin"], "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            //var HoraIni = TimeSpan.ParseExact(intervalString1, format, CultureInfo.InvariantCulture);
            //var HoraFin = TimeSpan.ParseExact(intervalString, format, CultureInfo.InvariantCulture);


            if (HoraFin < HoraIni)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','La hora fin debe ser mayor a la hora inicial.');}window.onload=init;</script>");
            }

            if (objResolutores.Lunes == false && objResolutores.Martes == false && objResolutores.Miercoles == false && objResolutores.Jueves == false && objResolutores.Viernes == false && objResolutores.Sabado == false && objResolutores.Domingo == false)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Debe seleccionar al menos un día.');}window.onload=init;</script>");
            }
            objResolutores.IdGrupo = ResolutorId;
            objResolutores.HoraInicio = HoraIni;
            objResolutores.HoraFin = HoraFin;
            objResolutores.Estado = true;
            objResolutores.FechaCrea = DateTime.Now;
            objResolutores.UsuarioCrea = UserId;
            //objResolutores.Lunes = Luneschk;
            
            if (v) {
                objResolutores.Feriado = true;
            }
            else
            {
                objResolutores.Feriado = false;
            }
            dbe.GrupoHorario.Add(objResolutores);
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");

        }

        public ActionResult ListarHGResolutores()
        {
            int sw = 0, Creador = 0, Estado = 0;
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            //string msjError = string.Empty;
            //DateTime? FechaInicio = null, FechaFin = null;
            //int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            //if (int.TryParse(Convert.ToString(Request.Params["IdUsuario"]), out Creador) == false)
            //{
            //    Creador = 0;
            //}
            //if (int.TryParse(Convert.ToString(Request.Params["ID_VACA_STAT"]), out Estado) == false)
            //{
            //    Estado = 0;
            //}

            //if (Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoInicio"]) == null)
            //{
            //    FechaInicio = null;
            //}
            //else
            //{
            //    CultureInfo cultures = new CultureInfo("es-ES");
            //    FechaInicio = Convert.ToDateTime(Request.Params["FechaIngresoInicio"], cultures);
            //}

            //if (Convert.ToString(Request.QueryString["FechaIngresoFin"]) == "" || Convert.ToString(Request.QueryString["FechaIngresoFin"]) == null)
            //{
            //    FechaFin = null;
            //}
            //else
            //{
            //    CultureInfo cultures = new CultureInfo("es-ES");
            //    FechaFin = Convert.ToDateTime(Request.Params["FechaIngresoFin"], cultures);
            //}

            var result1 = dbe.ListarHorariosGruposResolutores(IdAcco).ToList();
            //var result = dbe.ListarSolicitudVacacion(Creador, Estado, FechaInicio, FechaFin, IdPersEnti).ToList();

            return Json(new { data = result1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditarGrupoHorario(int? id = 0)
        {
            var query1 = (from s in dbe.GrupoHorario
                          select s).ToList();
            var query2 = (from c in dbc.QUEUEs
                          select c).ToList();
            var nombreGResolutor = (from sp in query1
                                    join ds in query2
                                 on sp.IdGrupo equals ds.ID_QUEU
                                    where sp.Id == id
                                    select ds).FirstOrDefault().NAM_QUEU_REPO;

            var IdGrupo = (from r in dbe.GrupoHorario where r.Id == id select r).First().IdGrupo;
            var HoraInicio = (from r in dbe.GrupoHorario where r.Id == id select r).First().HoraInicio;
            var HoraFin = (from r in dbe.GrupoHorario where r.Id == id select r).First().HoraFin;
            //Dias
            var Lunes = (from r in dbe.GrupoHorario where r.Id == id select r).First().Lunes;
            ViewBag.Lunes = Lunes;
            var Martes = (from r in dbe.GrupoHorario where r.Id == id select r).First().Martes;
            ViewBag.Martes = Martes;
            var Miercoles = (from r in dbe.GrupoHorario where r.Id == id select r).First().Miercoles;
            ViewBag.Miercoles = Miercoles;
            var Jueves = (from r in dbe.GrupoHorario where r.Id == id select r).First().Jueves;
            ViewBag.Jueves = Jueves;
            var Viernes = (from r in dbe.GrupoHorario where r.Id == id select r).First().Viernes;
            ViewBag.Viernes = Viernes;
            var Sabado = (from r in dbe.GrupoHorario where r.Id == id select r).First().Sabado;
            ViewBag.Sabado = Sabado;
            var Domingo = (from r in dbe.GrupoHorario where r.Id == id select r).First().Domingo;
            ViewBag.Domingo = Domingo;
            var Feriados = (from r in dbe.GrupoHorario where r.Id == id select r).First().Feriado;
            ViewBag.Feriado = Feriados;
            //
            ViewBag.GrupoResolutor = nombreGResolutor;
            ViewBag.HoraInicio = new DateTime().Add((TimeSpan)HoraInicio).ToString("hh:mm tt");
            ViewBag.HoraFin = new DateTime().Add((TimeSpan)HoraFin).ToString("hh:mm tt");
            ViewBag.Id = id;
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarGrupoHorario(GrupoHorario p)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var HorarioGResolutor = dbe.GrupoHorario.Where(i => i.Id == p.Id).Single();
            TimeSpan HoraIni = DateTime.ParseExact(Request.Params["HoraIngresoInicio"], "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            TimeSpan HoraFin = DateTime.ParseExact(Request.Params["HoraIngresoFin"], "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            if (HoraFin < HoraIni)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','La hora fin debe ser mayor a la hora inicial.');}window.onload=init;</script>");
            }
            var Luneschk = p.Lunes;
            var Marteschk = p.Martes;
            var Miercoleschk = p.Miercoles;
            var Jueveschk = p.Jueves;
            var Vierneschk = p.Viernes;
            var Sabadochk = p.Sabado;
            var Domingochk = p.Domingo;
            var Feriadochk = p.Feriado;
            HorarioGResolutor.HoraInicio = HoraIni;
            HorarioGResolutor.HoraFin = HoraFin;
            HorarioGResolutor.Lunes = Luneschk;
            HorarioGResolutor.Martes = Marteschk;
            HorarioGResolutor.Miercoles = Miercoleschk;
            HorarioGResolutor.Jueves = Jueveschk;
            HorarioGResolutor.Viernes = Vierneschk;
            HorarioGResolutor.Sabado = Sabadochk;
            HorarioGResolutor.Domingo = Domingochk;
            HorarioGResolutor.Feriado = Feriadochk;
            HorarioGResolutor.FechaActualiza = DateTime.Now;
            HorarioGResolutor.UsuarioActualiza = UserId;
            dbe.GrupoHorario.Attach(HorarioGResolutor);
            dbe.Entry(HorarioGResolutor).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneUpdate) top.uploadDoneUpdate('OK');}window.onload=init;</script>");
        }

        public ActionResult EliminarGrupoHorario(int? id = 0)
        {

            ViewBag.Id = id;
            return View();


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarGrupoHorario(GrupoHorario p)
        {
            var HorarioGResolutor = dbe.GrupoHorario.Where(i => i.Id == p.Id).Single();
            HorarioGResolutor.Estado = false;
            dbe.GrupoHorario.Attach(HorarioGResolutor);
            dbe.Entry(HorarioGResolutor).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneDelete) top.uploadDoneDelete('OK');}window.onload=init;</script>");
        }

        public ActionResult EditarHoraInicio(int? id = 0)
        {
            var HoraInicio = (from r in dbe.GrupoHorario where r.Id == id select r).First().HoraInicio;
            ViewBag.HoraInicio = new DateTime().Add((TimeSpan)HoraInicio).ToString("hh:mm tt");
            ViewBag.Id = id;
            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarHoraInicio(GrupoHorario p)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var Horario = dbe.GrupoHorario.Where(i => i.Id == p.Id).Single();
            TimeSpan HoraIni = DateTime.ParseExact(Request.Params["HoraInicio"], "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            var HoraFin = Horario.HoraFin;
            
            if (HoraFin < HoraIni)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','La hora inicial debe ser menor a la hora final.');}window.onload=init;</script>");
            }
            Horario.HoraInicio = HoraIni;
            Horario.FechaActualiza = DateTime.Now;
            Horario.UsuarioActualiza = UserId;
            dbe.GrupoHorario.Attach(Horario);
            dbe.Entry(Horario).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        public ActionResult EditarHoraFin(int? id = 0)
        {
            var HoraFin = (from r in dbe.GrupoHorario where r.Id == id select r).First().HoraFin;
            ViewBag.HoraFin = new DateTime().Add((TimeSpan)HoraFin).ToString("hh:mm tt");
            ViewBag.Id = id;
            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarHoraFin(GrupoHorario h)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var Horario = dbe.GrupoHorario.Where(i => i.Id == h.Id).Single();
            var HoraIni = Horario.HoraInicio;
             TimeSpan HoraFin = DateTime.ParseExact(Request.Params["HoraFin"], "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

            if (HoraFin < HoraIni)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','La hora final debe ser mayor a la hora inicial.');}window.onload=init;</script>");
            }
            Horario.HoraFin = HoraFin;
            Horario.FechaActualiza = DateTime.Now;
            Horario.UsuarioActualiza = UserId;
            dbe.GrupoHorario.Attach(Horario);
            dbe.Entry(Horario).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        public ActionResult EditarDias(int? id = 0)
        {
            //Dias
            var Lunes = (from r in dbe.GrupoHorario where r.Id == id select r).First().Lunes;
            ViewBag.Lunes = Lunes;
            var Martes = (from r in dbe.GrupoHorario where r.Id == id select r).First().Martes;
            ViewBag.Martes = Martes;
            var Miercoles = (from r in dbe.GrupoHorario where r.Id == id select r).First().Miercoles;
            ViewBag.Miercoles = Miercoles;
            var Jueves = (from r in dbe.GrupoHorario where r.Id == id select r).First().Jueves;
            ViewBag.Jueves = Jueves;
            var Viernes = (from r in dbe.GrupoHorario where r.Id == id select r).First().Viernes;
            ViewBag.Viernes = Viernes;
            var Sabado = (from r in dbe.GrupoHorario where r.Id == id select r).First().Sabado;
            ViewBag.Sabado = Sabado;
            var Domingo = (from r in dbe.GrupoHorario where r.Id == id select r).First().Domingo;
            ViewBag.Domingo = Domingo;
            var Feriados = (from r in dbe.GrupoHorario where r.Id == id select r).First().Feriado;
            ViewBag.Feriado = Feriados;
            //
            ViewBag.Id = id;
            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarDias(GrupoHorario h)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var Horario = dbe.GrupoHorario.Where(i => i.Id == h.Id).Single();
            if (h.Lunes == false && h.Martes == false && h.Miercoles == false && h.Jueves == false && h.Viernes == false && h.Sabado == false && h.Domingo == false)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Debe seleccionar al menos un día.');}window.onload=init;</script>");
            }
            Horario.Lunes = h.Lunes;
            Horario.Martes = h.Martes;
            Horario.Miercoles = h.Miercoles;
            Horario.Jueves = h.Jueves;
            Horario.Viernes = h.Viernes;
            Horario.Sabado = h.Sabado;
            Horario.Domingo = h.Domingo;
            Horario.FechaActualiza = DateTime.Now;
            Horario.UsuarioActualiza = UserId;
            dbe.GrupoHorario.Attach(Horario);
            dbe.Entry(Horario).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        public ActionResult EditarFeriados(int? id = 0)
        {
            var Feriado = (from r in dbe.GrupoHorario where r.Id == id select r).First().Feriado;
            
            if ((bool)Feriado)
            {
                ViewBag.Si = true;
                ViewBag.No = false;
            }
            else
            {
                ViewBag.Si = false;
                ViewBag.No = true;
            }

            ViewBag.Id = id;
            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFeriados(GrupoHorario h)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var Horario = dbe.GrupoHorario.Where(i => i.Id == h.Id).Single();
            bool v = Convert.ToBoolean(Request.Params["IsActive"]);
            if (v)
            {
                Horario.Feriado = true;
            }
            else
            {
                Horario.Feriado = false;
            }
            Horario.FechaActualiza = DateTime.Now;
            Horario.UsuarioActualiza = UserId;
            dbe.GrupoHorario.Attach(Horario);
            dbe.Entry(Horario).State = EntityState.Modified;
            dbe.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }
    }
}
