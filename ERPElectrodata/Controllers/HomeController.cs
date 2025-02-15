using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Web.Security;
using ERPElectrodata.Objects.CreateMail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Configuration;
using ERPElectrodata.Objects;
using System.Threading;
using System.Globalization;
using ERPElectrodata.Object;
using ERPElectrodata.Object.Ticket;
using System.Web.Security;
using Electrodata.ERPElectrodata.Domain.Services.LeccionAprendidaService;
using Electrodata.ERPElectrodata.Infra.Reposotories.LeccionAprendidaRepositorie;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using System.Text;
namespace ERPElectrodata.Controllers
{
    public class HomeController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        public TicketIR tir = new TicketIR();

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Principal()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.Usuario = ID_ACCO;

            return View();
        }

        #region Dashboard por cliente
        public PartialViewResult _ElectrodataPeru()
        {
            return PartialView();
        }

        #region Buenaventura
        public ActionResult _Buenaventura()
        {
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            bool vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            ViewBag.PuedeVerMisTickets = "false";
            if (ID_QUEU == 100 || vis_all_queu== true)
            {
                ViewBag.PuedeVerMisTickets = "true";
            }
            return PartialView();
        }
        public ActionResult _Buenaventura_Grupos()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            var cantidadTicketsActivo = dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 1, ID_ACCO, vis_all_queu, 0).First();
            var cantidadTicketsProgramados = dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 5, ID_ACCO, vis_all_queu, 0).First();
            var cantidadTicketsResueltos = dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 4, ID_ACCO, vis_all_queu, 0).First();
            var cantidadTicketsCerrados = dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 6, ID_ACCO, vis_all_queu, 0).First();
            DateTime fechaActualizacion = (DateTime)Session["FechaActualizacion"];
            TimeSpan diferenciaFecha = DateTime.Now - fechaActualizacion;

            int diferenciaEnDias = diferenciaFecha.Days;

            ViewBag.dias = diferenciaEnDias;
            ViewBag.TAActive = cantidadTicketsActivo;
            ViewBag.TAScheduled = cantidadTicketsProgramados;
            ViewBag.TAResolved = cantidadTicketsResueltos;
            ViewBag.TAClosed = cantidadTicketsCerrados;
            ViewBag.PActive = cantidadTicketsActivo;
            ViewBag.PScheduled = cantidadTicketsProgramados;
            ViewBag.PResolved = cantidadTicketsResueltos;
            ViewBag.PClosed = cantidadTicketsCerrados;
            return PartialView();
        }
        public ActionResult _Buenaventura_MisTickets()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            var cantidadTicketsActivo = dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 1, ID_ACCO, vis_all_queu, 1).First();
            var cantidadTicketsProgramados = dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 5, ID_ACCO, vis_all_queu, 1).First();
            ViewBag.TAActive = cantidadTicketsActivo;
            ViewBag.TAScheduled = cantidadTicketsProgramados;
            ViewBag.PActive = cantidadTicketsActivo;
            ViewBag.PScheduled = cantidadTicketsProgramados;
            return PartialView();
        }
        public ActionResult _Buenaventura_MisTickets_Activos(int cantidad = 0)
        {
            List<ListarTicketGrupoPorUsuario_Result> tickets = new List<ListarTicketGrupoPorUsuario_Result>();
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        public ActionResult _Buenaventura_MisTickets_Programados(int cantidad = 0)
        {
            List<ListarTicketGrupoPorUsuario_Result> tickets = new List<ListarTicketGrupoPorUsuario_Result>();
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _Buenaventura_MisTareas()
        {
            int idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int visAllQueu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            var cantidadTareasActivo = dbc.CantidadTareasPorUsuario(idPersEnti, 1, visAllQueu).First();
            var cantidadTareasProgramado = dbc.CantidadTareasPorUsuario(idPersEnti, 2, visAllQueu).First();
            var cantidadTareasResuelto = dbc.CantidadTareasPorUsuario(idPersEnti, 3, visAllQueu).First();
            ViewBag.TaActivo = cantidadTareasActivo;
            ViewBag.TaProgramado = cantidadTareasProgramado;
            ViewBag.TaResuelto = cantidadTareasResuelto;
            return PartialView();
        }
        public ActionResult _Buenaventura_MisTareas_Activos(int cantidad = 0)
        {
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTareas = cantidadPaginas;
            return PartialView();
        }
        public ActionResult _Buenaventura_MisTareas_Programados(int cantidad = 0)
        {
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTareas = cantidadPaginas;
            return PartialView();
        }
        public ActionResult _Buenaventura_MisTareas_Resueltos(int cantidad = 0)
        {
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTareas = cantidadPaginas;
            return PartialView();
        }
        public ActionResult ActualizarMisTareasBnv(int estado, int pagina)
        {
            int idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int visAllQueu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int inicio = 0;
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            var tareas = dbc.ListarTareasPorUsuario(idPersEnti, estado, visAllQueu, inicio, 10).ToList();
            return PartialView("_Buenaventura_TareasPorUsuario", tareas);
        }

        public ActionResult _Buenaventura_Grupos_Activos(int cantidad = 0)
        {
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTickets = cantidadPaginas;
            //var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, 0, 1, ID_ACCO, 10).ToList();
            List<ListarTicketGrupoPorUsuario_Result> tickets = new List<ListarTicketGrupoPorUsuario_Result>();
            return PartialView(tickets);
        }
        public ActionResult _Buenaventura_Grupos_Programados(int cantidad = 0)
        {
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTickets = cantidadPaginas;
            //var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, 0, 5, ID_ACCO, 10).ToList();
            List<ListarTicketGrupoPorUsuario_Result> tickets = new List<ListarTicketGrupoPorUsuario_Result>();
            return PartialView(tickets);
        }
        public ActionResult _Buenaventura_Grupos_Resueltos(int cantidad = 0)
        {
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTickets = cantidadPaginas;
            //var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, 0, 4, ID_ACCO, 10).ToList();
            List<ListarTicketGrupoPorUsuario_Result> tickets = new List<ListarTicketGrupoPorUsuario_Result>();
            return PartialView(tickets);
        }
        public ActionResult _Buenaventura_Grupos_Cerrados(int cantidad = 0)
        {
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            ViewBag.CantidadTickets = cantidadPaginas;
            //var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, 0, 6, ID_ACCO, 10).ToList();
            List<ListarTicketGrupoPorUsuario_Result> tickets = new List<ListarTicketGrupoPorUsuario_Result>();
            return PartialView(tickets);
        }
        public ActionResult ActualizarGrupoBnv(int seccion, int pagina)
        {
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, inicio, seccion, ID_ACCO, 10, vis_all_queu, 0).ToList();
            return PartialView("_Buenaventura_TicketsGrupo", tickets);
        }
        public ActionResult ActualizarMisTicketsBnv(int seccion, int pagina)
        {
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, inicio, seccion, ID_ACCO, 10, vis_all_queu, 1).ToList();
            return PartialView("_Buenaventura_TicketsGrupo", tickets);
        }
        public ActionResult ActualizarTicketsPortalUsuario(int seccion, int pagina)
        {
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, inicio, seccion, ID_ACCO, 10, vis_all_queu, 2).ToList();
            return PartialView("_Buenaventura_TicketsGrupoPortal", tickets);
        }
        public ActionResult ActualizarTicketsAppTeams(int seccion, int pagina)
        {
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, inicio, seccion, ID_ACCO, 10, vis_all_queu, 4).ToList();
            return PartialView("_Buenaventura_TicketsGrupoPortal", tickets);
        }
        public ActionResult ActualizarTicketsAppItms(int seccion, int pagina)
        {
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            var tickets = dbc.ListarTicketGrupoPorUsuario(ID_PERS_ENTI, inicio, seccion, ID_ACCO, 10, vis_all_queu, 3).ToList();
            return PartialView("_Buenaventura_TicketsGrupoPortal", tickets);
        }

        public ActionResult _Buenaventura_PortalUsuario()
        {
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int cantidad = (int)dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 1, ID_ACCO, vis_all_queu, 2).First();
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadTickets = cantidadPaginas;
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            return PartialView();
        }

        public ActionResult _Buenaventura_Teams()
        {
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int cantidad = (int)dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 1, ID_ACCO, vis_all_queu, 4).First();
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadTickets = cantidadPaginas;
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            return PartialView();
        }
        public ActionResult _Buenaventura_AppItms()
        {
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int cantidad = (int)dbc.CantidadTicketGrupoPorUsuario(ID_PERS_ENTI, 1, ID_ACCO, vis_all_queu, 3).First();
            var cantidadPaginas = cantidad / 10;
            if (cantidad > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidad % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadTickets = cantidadPaginas;
            ViewBag.CantidadPintar = cantidadPintar(cantidad);
            return PartialView();
        }
        #endregion

        #region Minsur

        public ActionResult _Minsur()
        {
            return PartialView();
        }
        //Tareas Pendientes Antiguo (Todo en bloque)
        public ActionResult _Minsur_TareasPendientes()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            int cantidadTicketsActivos = Convert.ToInt32(dbc.CantidadTicketMinsur(ID_ACCO, ID_QUEU).First());
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView();
        }
        //Tareas Pendientes Nuevo (Separado por estados)
        public ActionResult _Minsur_Tareas()
        {
            //Queue
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            bool esGestorAccesos = Convert.ToBoolean(Session["GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA"]);
            
            //Account
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //Location
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_LOCA = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI).First().ID_LOCA.GetValueOrDefault(0);
            
            if (esGestorAccesos)
            {
                ID_QUEU = 0;
                ID_LOCA = 0;
            }
            CantidadTareasTotalPorEstado_Result cantidadTareasTotalPorEstado = dbc.CantidadTareasTotalPorEstado(ID_QUEU,ID_ACCO,ID_LOCA).First();

            ViewBag.Pendientes = cantidadTareasTotalPorEstado.Pendientes;
            ViewBag.SinAsignar = cantidadTareasTotalPorEstado.SinAsignar;
            ViewBag.Terminadas = cantidadTareasTotalPorEstado.Terminadas;
            ViewBag.NoProceden = cantidadTareasTotalPorEstado.NoProceden;

            return PartialView();
        }
        public ActionResult _Minsur_Tareas_Pendientes(int cantidad = 0)
        {
            ViewBag.Cantidad = cantidad;
            return PartialView();
        }
        public ActionResult _Minsur_Tareas_SinAsignar(int cantidad = 0)
        {
            ViewBag.Cantidad = cantidad;
            return PartialView();
        }
        public ActionResult _Minsur_Tareas_Terminadas(int cantidad = 0)
        {
            ViewBag.Cantidad = cantidad;
            return PartialView();
        }
        public ActionResult _Minsur_Tareas_NoProcedentes(int cantidad = 0)
        {
            ViewBag.Cantidad = cantidad;
            return PartialView();
        }
        public ActionResult ListaUsuariosPorCola(int ID_QUEU)
        {
            //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            //bool esGestorAccesos = Convert.ToBoolean(Session["GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA"]);
            //if (esGestorAccesos)
            //    ID_QUEU = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var result = dbe.UsuariosMinsurPorColaTareas(ID_ACCO, ID_QUEU).ToList();
            //return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion
        public ActionResult ListaTareasMinsurPorEstado(int estado = 0)
        {
            //Queue
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            bool esGestorAccesos = Convert.ToBoolean(Session["GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA"]);
            
            //Account
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //Location
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_LOCA = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_LOCA.GetValueOrDefault(0);

            if (esGestorAccesos)
            {
                ID_QUEU = 0;
                ID_LOCA = 0;
            }
            List<ListaTareaMinsurPorEstado_Result> tareas = dbc.ListaTareaMinsurPorEstado(ID_QUEU, ID_ACCO, ID_LOCA, estado).ToList();

            //return Json(new { data = tareas }, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(new { data = tareas }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult ActualizarListaPendientesMinsur()
        {
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            List<TicketsPorGrupo_Result> tickets = dbc.TicketsPorGrupoMinsur(ID_ACCO, inicio, 10, ID_QUEU).ToList();
            foreach (var ticketMod in tickets)
            {
                ticketMod.cronograma = NewScheduleDate(ticketMod.ID_TICK);
                ticketMod.tiempoTicket = ticketMod.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(ticketMod.ID_TICK), ticketMod.FEC_INI_TICK.Value) : "";
                ticketMod.expTime = tir.ExpirationTime(ticketMod.ID_TICK, ticketMod.HOU_PRIO.Value);
            }
            return PartialView("_General_ListaTickets", tickets);
        }

        public ActionResult ActualizarActivos(int seccion, int pagina)
        {
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            bool vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int nVisAllQueu = 0;
            ViewBag.Id_Acco = ID_ACCO;
            if (vis_all_queu == true)
            {
                nVisAllQueu = 1;
            }
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            if (ID_ACCO == 60)
            {
                List<TicketsPorGrupoBNV_Result> tickets = dbc.TicketsPorGrupoBNV(ID_ACCO, seccion, inicio, 10, ID_PERS_ENTI, nVisAllQueu).ToList();
                foreach (var ticketMod in tickets)
                {
                    ticketMod.cronograma = NewScheduleDate(ticketMod.ID_TICK);
                    ticketMod.tiempoTicket = ticketMod.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(ticketMod.ID_TICK), ticketMod.FEC_INI_TICK.Value) : "";
                    ticketMod.expTime = tir.ExpirationTime(ticketMod.ID_TICK, ticketMod.HOU_PRIO.Value);
                }
                return PartialView("_General_ListaTicketsBNV", tickets);
            }
            else
            {
                List<TicketsPorGrupo_Result> tickets = dbc.TicketsPorGrupo(ID_ACCO, seccion, inicio, 10, UserId, ID_PERS_ENTI, nVisAllQueu).ToList();
                foreach (var ticketMod in tickets)
                {
                    ticketMod.cronograma = NewScheduleDate(ticketMod.ID_TICK);
                    ticketMod.tiempoTicket = ticketMod.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(ticketMod.ID_TICK), ticketMod.FEC_INI_TICK.Value) : "";
                    ticketMod.expTime = tir.ExpirationTime(ticketMod.ID_TICK, ticketMod.HOU_PRIO.Value);
                }
                return PartialView("_General_ListaTickets", tickets);
            }
        }

        #region Todos los clientes con configuracion por defecto
        public ActionResult _General()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            DateTime fechaActualizacion = (DateTime)Session["FechaActualizacion"];
            TimeSpan diferenciaFecha = DateTime.Now - fechaActualizacion;

            int diferenciaEnDias = diferenciaFecha.Days;

            ViewBag.dias = diferenciaEnDias;
            if (ID_ACCO == 60)
            {
                CantidadTicketTotalUsuarioBNV_Result cantidadTicketsBnv = dbc.CantidadTicketTotalUsuarioBNV(ID_PERS_ENTI, ID_ACCO, vis_all_queu).First();
                decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
                ViewBag.TAActive = cantidadTicketsBnv.activos;
                ViewBag.TAScheduled = cantidadTicketsBnv.programados;
                ViewBag.TAResolved = cantidadTicketsBnv.resueltos;
                ViewBag.TAClosed = cantidadTicketsBnv.cerrados;
                ViewBag.PActive = cantidadTicketsBnv.activos;
                ViewBag.PScheduled = cantidadTicketsBnv.programados;
                ViewBag.PResolved = cantidadTicketsBnv.resueltos;
                ViewBag.PClosed = cantidadTicketsBnv.cerrados;
                ViewBag.Porcentaje = Porcentaje;
                return PartialView();
            }
            else
            {
                CantidadTicketTotalPorUsuario_Result cantidadTicketTotalPorUsuario = dbc.CantidadTicketTotalPorUsuario(ID_PERS_ENTI, ID_ACCO, vis_all_queu, UserId).First();
                decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
                ViewBag.TAActive = cantidadTicketTotalPorUsuario.activos;
                ViewBag.TAScheduled = cantidadTicketTotalPorUsuario.programados;
                ViewBag.TAResolved = cantidadTicketTotalPorUsuario.resueltos;
                ViewBag.TAClosed = cantidadTicketTotalPorUsuario.cerrados;
                ViewBag.PActive = cantidadTicketTotalPorUsuario.activos;
                ViewBag.PScheduled = cantidadTicketTotalPorUsuario.programados;
                ViewBag.PResolved = cantidadTicketTotalPorUsuario.resueltos;
                ViewBag.PClosed = cantidadTicketTotalPorUsuario.cerrados;
                ViewBag.Porcentaje = Porcentaje;
                return PartialView();
            }
        }
        public ActionResult _General_Activos(int cantidad = 0)
        {
            int cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadTickets = cantidadPaginas;
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            return PartialView(tickets);

        }
        public ActionResult _General_Programados(int cantidad = 0)
        {
            int cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        public ActionResult _General_Resueltos(int cantidad = 0)
        {
            int cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        public ActionResult _General_Cerrados(int cantidad = 0)
        {
            int cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        #endregion
        #region ElectrodataPeru
        public ActionResult ActualizarNormalElectrodataPeru(int seccion, int pagina)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            bool vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int nVisAllQueu = 0;
            if (vis_all_queu == true)
            {
                nVisAllQueu = 1;
            }
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var ticketsActivos = dbc.TicketsPorGrupo(ID_ACCO, seccion, inicio, 10, UserId, ID_PERS_ENTI, nVisAllQueu).ToList();
            tickets = ticketsActivos;
            foreach (var ticketMod in tickets)
            {
                ticketMod.cronograma = NewScheduleDate(ticketMod.ID_TICK);
                ticketMod.tiempoTicket = ticketMod.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(ticketMod.ID_TICK), ticketMod.FEC_INI_TICK.Value) : "";
                ticketMod.expTime = tir.ExpirationTime(ticketMod.ID_TICK, ticketMod.HOU_PRIO.Value);
            }
            return PartialView("_ElectrodataPeru_ListaTickets_PorGrupo", tickets);
        }
        public ActionResult ActualizarActivosElectrodataPeru(int seccion, int pagina, int esInterno)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            bool vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int nVisAllQueu = 0;
            if (vis_all_queu == true)
            {
                nVisAllQueu = 1;
            }
            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }
            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var ticketsActivos = dbc.TicketsGrupoElectrodataPeru(ID_ACCO, seccion, inicio, 10, UserId, ID_PERS_ENTI, nVisAllQueu, esInterno).ToList();
            tickets = ticketsActivos;
            foreach (var ticketMod in tickets)
            {
                ticketMod.cronograma = NewScheduleDate(ticketMod.ID_TICK);
                ticketMod.tiempoTicket = ticketMod.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(ticketMod.ID_TICK), ticketMod.FEC_INI_TICK.Value) : "";
                ticketMod.expTime = tir.ExpirationTime(ticketMod.ID_TICK, ticketMod.HOU_PRIO.Value);
            }
            return PartialView("_ElectrodataPeru_ListaTickets_PorGrupoElectrodata", tickets);
        }
        #region Electrodata Peru Interno
        public ActionResult _ElectrodataPeru_Interno()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            bool vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int IdQueu = Convert.ToInt32(Session["ID_QUEU"]);
            var cantidadTicketsActivo = 0;
            var cantidadTicketsProgramados = 0;
            var cantidadTicketsEnEspera = 0;
            var cantidadTicketsResueltos = 0;
            var cantidadTicketsCerrados = 0;
            var cantidadTicketsResueltoSinInforme = 0;
            if (vis_all_queu == true)
            {
                cantidadTicketsActivo = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "INTERNO" && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)).Count();
                cantidadTicketsProgramados = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 5).Count();
                cantidadTicketsEnEspera = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 9).Count();
                cantidadTicketsResueltoSinInforme = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 10).Count();
                cantidadTicketsResueltos = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 4).Count();
                cantidadTicketsCerrados = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 6).Count();
            }
            else
            {
                cantidadTicketsActivo = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "INTERNO" && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)).Count();
                cantidadTicketsProgramados = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 5).Count();
                cantidadTicketsEnEspera = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 9).Count();
                cantidadTicketsResueltoSinInforme = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 10).Count();
                cantidadTicketsResueltos = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 4).Count();
                cantidadTicketsCerrados = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "INTERNO" && t.ID_STAT_END == 6).Count();
            }

            decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
            ViewBag.TAActive = cantidadTicketsActivo;
            ViewBag.TAScheduled = cantidadTicketsProgramados;
            ViewBag.TAEnEspera = cantidadTicketsEnEspera;
            ViewBag.TAResueltoSinInforme = cantidadTicketsResueltoSinInforme;
            ViewBag.TAResolved = cantidadTicketsResueltos;
            ViewBag.TAClosed = cantidadTicketsCerrados;
            ViewBag.PActive = cantidadTicketsActivo;
            ViewBag.PScheduled = cantidadTicketsProgramados;
            ViewBag.PEnEspera = cantidadTicketsEnEspera;
            ViewBag.PResolved = cantidadTicketsResueltos;
            ViewBag.PResueltoSinInforme = cantidadTicketsResueltoSinInforme;
            ViewBag.PClosed = cantidadTicketsCerrados;
            ViewBag.Porcentaje = Porcentaje;
            return PartialView();
        }
        public ActionResult _ElectrodataPeru_Interno_Activos(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;

            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        public ActionResult _ElectrodataPeru_Interno_Programados(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;

            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Interno_EnEspera(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;

            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Interno_ResueltoSinInforme(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;

            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Interno_Resueltos(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;

            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Interno_Cerrados(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;

            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        #endregion

        #region Electrodata Peru Externos

        public ActionResult _ElectrodataPeru_Externo()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            bool vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int IdQueu = Convert.ToInt32(Session["ID_QUEU"]);
            var cantidadTicketsActivo = 0;
            var cantidadTicketsProgramados = 0;
            var cantidadTicketsResueltos = 0;
            var cantidadTicketsCerrados = 0;
            var cantidadTicketsEnEspera = 0;
            var cantidadTicketsResueltoSinInforme = 0;
            if (vis_all_queu == true)
            {
                cantidadTicketsActivo = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "EXTERNO" && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)).Count();
                cantidadTicketsProgramados = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 5).Count();
                cantidadTicketsEnEspera = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 9).Count();
                cantidadTicketsResueltoSinInforme = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 10).Count();
                cantidadTicketsResueltos = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 4).Count();
                cantidadTicketsCerrados = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 6).Count();
            }
            else
            {
                cantidadTicketsActivo = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "EXTERNO" && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)).Count();
                cantidadTicketsProgramados = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 5).Count();
                cantidadTicketsEnEspera = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 9).Count();
                cantidadTicketsResueltoSinInforme = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 10).Count();
                cantidadTicketsResueltos = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 4).Count();
                cantidadTicketsCerrados = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && (t.UserId == UserId || t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI /*|| (ID_ACCO == 4 && t.ID_QUEU == IdQueu)*/) && t.SubCuenta == "EXTERNO" && t.ID_STAT_END == 6).Count();
            }

            decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
            ViewBag.TAActive = cantidadTicketsActivo;
            ViewBag.TAScheduled = cantidadTicketsProgramados;
            ViewBag.TAEnEspera = cantidadTicketsEnEspera;
            ViewBag.TAResueltoSinInforme = cantidadTicketsResueltoSinInforme;
            ViewBag.TAResolved = cantidadTicketsResueltos;
            ViewBag.TAClosed = cantidadTicketsCerrados;
            ViewBag.PActive = cantidadTicketsActivo;
            ViewBag.PScheduled = cantidadTicketsProgramados;
            ViewBag.PEnEspera = cantidadTicketsEnEspera;
            ViewBag.PResueltoSinInforme = cantidadTicketsResueltoSinInforme;
            ViewBag.PResolved = cantidadTicketsResueltos;
            ViewBag.PClosed = cantidadTicketsCerrados;
            ViewBag.Porcentaje = Porcentaje;
            return PartialView();
        }


        public ActionResult _ElectrodataPeru_Externo_Activos(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        public ActionResult _ElectrodataPeru_Externo_Programados(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Externo_EnEspera(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Externo_ResueltoSinInforme(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Externo_Resueltos(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        public ActionResult _ElectrodataPeru_Externo_Cerrados(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsGrupoElectrodataPeru_Result> tickets = new List<TicketsGrupoElectrodataPeru_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        #endregion

        #region Electrodata Peru Completo
        public ActionResult _ElectrodataPeru_Completo()
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int vis_all_queu = Convert.ToInt32(Session["VIS_ALL_QUEU"]);

            dynamic cantidadTickets;

            if (ID_ACCO == 4)
            {
                // Si ID_ACCO es 4, llamamos al procedimiento CantidadTicketTotalPorUsuarioEdata
                cantidadTickets = dbc.CantidadTicketTotalPorUsuarioEdata(ID_PERS_ENTI, ID_ACCO, vis_all_queu, UserId).First();
                ViewBag.TAEnEspera = cantidadTickets.enespera;
                ViewBag.PEnEspera = cantidadTickets.enespera;
                ViewBag.TAResueltoSinInforme = cantidadTickets.resueltosininforme;
                ViewBag.PResueltoSinInforme = cantidadTickets.resueltosininforme;
            }
            else
            {
                // Si no es 4, llamamos al procedimiento original
                cantidadTickets = dbc.CantidadTicketTotalPorUsuario(ID_PERS_ENTI, ID_ACCO, vis_all_queu, UserId).First();
            }

            decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());

            DateTime fechaActualizacion = (DateTime)Session["FechaActualizacion"];
            TimeSpan diferenciaFecha = DateTime.Now - fechaActualizacion;
            int diferenciaEnDias = diferenciaFecha.Days;

            ViewBag.dias = diferenciaEnDias;

            ViewBag.TAActive = cantidadTickets.activos;
            ViewBag.TAScheduled = cantidadTickets.programados;
            ViewBag.TAResolved = cantidadTickets.resueltos;
            ViewBag.TAClosed = cantidadTickets.cerrados;
            ViewBag.PActive = cantidadTickets.activos;
            ViewBag.PScheduled = cantidadTickets.programados;
            ViewBag.PResolved = cantidadTickets.resueltos;
            ViewBag.PClosed = cantidadTickets.cerrados;
            ViewBag.Porcentaje = Porcentaje;

            return PartialView();


        }
        public ActionResult _ElectrodataPeru_Completo_Activos(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;

            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Completo_Programados(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Completo_EnEspera(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Completo_ResueltoSinInforme(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Completo_Resueltos(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }

        public ActionResult _ElectrodataPeru_Completo_Cerrados(int cantidad = 0)
        {
            var cantidadTicketsActivos = cantidad;
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 1000)
            {
                cantidadPaginas = 100;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }
            ViewBag.CantidadPintar = cantidadPintar(cantidadTicketsActivos);
            ViewBag.CantidadTickets = cantidadPaginas;
            return PartialView(tickets);
        }
        #endregion
        #endregion

        [Authorize]
        public ActionResult Init_Cuentas()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int[] numbers = new int[0];
            int iq = 0;

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            try
            {
                decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
                ViewBag.Porcentaje = Porcentaje;

                Session["MAIN"] = "mp1";

                //if (Roles.IsUserInRole("SYSTEMADMINISTRATOR"))
                if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                {
                    ViewBag.VerWork = "1";
                }
                else
                {
                    ViewBag.VerWork = "0";
                }

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null);
                //var tickUsuarioGenerico = dbe.TicketsDeUsuariosGenericos().ToList();
                ViewBag.TAActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                ViewBag.TAScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                ViewBag.TAResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                ViewBag.TAClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                //ViewBag.TAGrupos = tickUsuarioGenerico.Count();
                //ViewBag.TAGruposTK = tickUsuarioGenerico;
                if (false == Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()))
                {


                    int supQueu = 0;
                    try
                    {
                        var Queus = dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
                            .Where(x => x.ID_PERS_ENTI_CORD == ID_PERS_ENTI);

                        supQueu = Queus.Count();

                        numbers = new int[supQueu];

                        foreach (var x in Queus)
                        {
                            //var orderKeys = new int[] { 1, 12, 306, 284, 50047 };
                            numbers[iq] = (int)x.ID_QUEU.Value;

                            iq++;
                            //Customers.Rows.Add(row);
                        }
                    }
                    catch
                    {

                    }

                    if (supQueu == 0)
                    {
                        tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                    }
                    else
                    {
                        //Mostrar el ticket cuya cola es la indicada
                        tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                    }

                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                else
                {
                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                ViewBag.TimeServer = String.Format("{0:MMM d yyyy HH:mm:ss}", DateTime.Now);
                ViewBag.Message = "";

                int idIdioma = Convert.ToInt32(Session["IdIdioma"]);

                string llave = "Asignadoa|Creadopor|Creado|UltimaAct";
                string[] llaves = llave.Split('|');

                var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
                            select new
                            {
                                llave = id.Llave,
                                texto = id.Texto
                            };

                var xxx = query.ToList();
                ViewBag.Asignadoa = xxx[0].texto;
                ViewBag.Creado = xxx[1].texto;
                ViewBag.UltimaAct = xxx[2].texto;
                ViewBag.Creadopor = xxx[3].texto;
                return View();
            }
            catch (Exception e)
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Init()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int[] numbers = new int[0];
            int iq = 0;

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            try
            {
                decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
                ViewBag.Porcentaje = Porcentaje;

                Session["MAIN"] = "mp1";

                //if (Roles.IsUserInRole("SYSTEMADMINISTRATOR"))
                if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                {
                    ViewBag.VerWork = "1";
                }
                else
                {
                    ViewBag.VerWork = "0";
                }

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null);

                ViewBag.TAActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                ViewBag.TAScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                ViewBag.TAResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                ViewBag.TAClosed = tick.Where(t => t.ID_STAT_END == 6).Count();

                //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                //var QUEU = dbc.QUEUEs.Single(q=>q.ID_QUEU== ID_QUEU).VIS_ALL_QUEU;
                if (false == Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()))
                {


                    int supQueu = 0;
                    try
                    {
                        var Queus = dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
                            .Where(x => x.ID_PERS_ENTI_CORD == ID_PERS_ENTI);

                        supQueu = Queus.Count();

                        numbers = new int[supQueu];

                        foreach (var x in Queus)
                        {
                            //var orderKeys = new int[] { 1, 12, 306, 284, 50047 };
                            numbers[iq] = (int)x.ID_QUEU.Value;

                            iq++;
                            //Customers.Rows.Add(row);
                        }
                    }
                    catch
                    {

                    }

                    if (supQueu == 0)
                    {
                        tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                    }
                    else
                    {
                        //Mostrar el ticket cuya cola es la indicada
                        tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                    }

                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                else
                {
                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                ViewBag.TimeServer = String.Format("{0:MMM d yyyy HH:mm:ss}", DateTime.Now);
                ViewBag.Message = "";

                int idIdioma = Convert.ToInt32(Session["IdIdioma"]);

                string llave = "Asignadoa|Creadopor|Creado|UltimaAct";
                //llave = Convert.ToString(Request.Params["llaves"]);
                string[] llaves = llave.Split('|');

                var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
                            select new
                            {
                                //cadena = id.Texto + "|" + id.Llave
                                llave = id.Llave,
                                texto = id.Texto
                            };

                var xxx = query.ToList();
                //List<string> elementos = new List<string>();
                //string texto = "";
                //foreach (var item in xxx)
                //{
                //    texto = item.texto;
                //    elementos.Add(texto);
                //}

                //string[] arrayTexto = elementos.ToArray();
                ViewBag.Asignadoa = xxx[0].texto;
                ViewBag.Creado = xxx[1].texto;
                ViewBag.UltimaAct = xxx[2].texto;
                ViewBag.Creadopor = xxx[3].texto;
                if (Convert.ToInt32(Session["ID_ACCO"]) == 4)
                    return View();
                else
                    //return View("Init_Cuentas");
                    return RedirectToAction("Init_Cuentas", "Home");
            }
            catch (Exception e)
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Init_Total()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int[] numbers = new int[0];
            int iq = 0;

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            try
            {
                decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
                ViewBag.Porcentaje = Porcentaje;

                Session["MAIN"] = "mp1";

                //if (Roles.IsUserInRole("SYSTEMADMINISTRATOR"))
                if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                {
                    ViewBag.VerWork = "1";
                }
                else
                {
                    ViewBag.VerWork = "0";
                }

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null);
                //var tickUsuarioGenerico = dbe.TicketsDeUsuariosGenericos().ToList();
                ViewBag.TAActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                ViewBag.TAScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                ViewBag.TAResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                ViewBag.TAClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                //ViewBag.TAGrupos = tickUsuarioGenerico.Count();
                //ViewBag.TAGruposTK = tickUsuarioGenerico;
                //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                //var QUEU = dbc.QUEUEs.Single(q=>q.ID_QUEU== ID_QUEU).VIS_ALL_QUEU;
                if (false == Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()))
                {
                    int supQueu = 0;
                    try
                    {
                        var Queus = dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
                            .Where(x => x.ID_PERS_ENTI_CORD == ID_PERS_ENTI);

                        supQueu = Queus.Count();

                        numbers = new int[supQueu];

                        foreach (var x in Queus)
                        {
                            //var orderKeys = new int[] { 1, 12, 306, 284, 50047 };
                            numbers[iq] = (int)x.ID_QUEU.Value;

                            iq++;
                            //Customers.Rows.Add(row);
                        }
                    }
                    catch
                    {

                    }

                    if (supQueu == 0)
                    {
                        tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                    }
                    else
                    {
                        //Mostrar el ticket cuya cola es la indicada
                        tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                    }

                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                else
                {
                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                ViewBag.TimeServer = String.Format("{0:MMM d yyyy HH:mm:ss}", DateTime.Now);
                ViewBag.Message = "";

                int idIdioma = Convert.ToInt32(Session["IdIdioma"]);

                string llave = "Asignadoa|Creadopor|Creado|UltimaAct";
                //llave = Convert.ToString(Request.Params["llaves"]);
                string[] llaves = llave.Split('|');

                var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
                            select new
                            {
                                //cadena = id.Texto + "|" + id.Llave
                                llave = id.Llave,
                                texto = id.Texto
                            };

                var xxx = query.ToList();
                //List<string> elementos = new List<string>();
                //string texto = "";
                //foreach (var item in xxx)
                //{
                //    texto = item.texto;
                //    elementos.Add(texto);
                //}

                //string[] arrayTexto = elementos.ToArray();
                ViewBag.Asignadoa = xxx[0].texto;
                ViewBag.Creado = xxx[1].texto;
                ViewBag.UltimaAct = xxx[2].texto;
                ViewBag.Creadopor = xxx[3].texto;

                return View();
            }
            catch (Exception e)
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Init_Interno()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int[] numbers = new int[0];
            int iq = 0;

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            try
            {
                decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
                ViewBag.Porcentaje = Porcentaje;

                Session["MAIN"] = "mp1";

                //if (Roles.IsUserInRole("SYSTEMADMINISTRATOR"))
                if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                {
                    ViewBag.VerWork = "1";
                }
                else
                {
                    ViewBag.VerWork = "0";
                }

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "INTERNO");

                ViewBag.TAActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                ViewBag.TAScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                ViewBag.TAResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                ViewBag.TAClosed = tick.Where(t => t.ID_STAT_END == 6).Count();

                //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                //var QUEU = dbc.QUEUEs.Single(q=>q.ID_QUEU== ID_QUEU).VIS_ALL_QUEU;
                if (false == Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()))
                {


                    int supQueu = 0;
                    try
                    {
                        var Queus = dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
                            .Where(x => x.ID_PERS_ENTI_CORD == ID_PERS_ENTI);

                        supQueu = Queus.Count();

                        numbers = new int[supQueu];

                        foreach (var x in Queus)
                        {
                            //var orderKeys = new int[] { 1, 12, 306, 284, 50047 };
                            numbers[iq] = (int)x.ID_QUEU.Value;

                            iq++;
                            //Customers.Rows.Add(row);
                        }
                    }
                    catch
                    {

                    }

                    if (supQueu == 0)
                    {
                        tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                    }
                    else
                    {
                        //Mostrar el ticket cuya cola es la indicada
                        tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                    }

                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                else
                {
                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                ViewBag.TimeServer = String.Format("{0:MMM d yyyy HH:mm:ss}", DateTime.Now);
                ViewBag.Message = "";

                int idIdioma = Convert.ToInt32(Session["IdIdioma"]);

                string llave = "Asignadoa|Creadopor|Creado|UltimaAct";
                //llave = Convert.ToString(Request.Params["llaves"]);
                string[] llaves = llave.Split('|');

                var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
                            select new
                            {
                                //cadena = id.Texto + "|" + id.Llave
                                llave = id.Llave,
                                texto = id.Texto
                            };

                var xxx = query.ToList();
                //List<string> elementos = new List<string>();
                //string texto = "";
                //foreach (var item in xxx)
                //{
                //    texto = item.texto;
                //    elementos.Add(texto);
                //}

                //string[] arrayTexto = elementos.ToArray();
                ViewBag.Asignadoa = xxx[0].texto;
                ViewBag.Creado = xxx[1].texto;
                ViewBag.UltimaAct = xxx[2].texto;
                ViewBag.Creadopor = xxx[3].texto;

                return View();
            }
            catch (Exception e)
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Init_Externo()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int[] numbers = new int[0];
            int iq = 0;

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            try
            {
                decimal Porcentaje = Convert.ToDecimal(dbe.ActObtenerPorcentajeTrabajado(ID_PERS_ENTI).SingleOrDefault());
                ViewBag.Porcentaje = Porcentaje;

                Session["MAIN"] = "mp1";

                //if (Roles.IsUserInRole("SYSTEMADMINISTRATOR"))
                if ((int)Session["ADMINISTRADOR_SISTEMA"] == 1)
                {
                    ViewBag.VerWork = "1";
                }
                else
                {
                    ViewBag.VerWork = "0";
                }

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.SubCuenta == "EXTERNO");

                ViewBag.TAActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                ViewBag.TAScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                ViewBag.TAResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                ViewBag.TAClosed = tick.Where(t => t.ID_STAT_END == 6).Count();

                //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                //var QUEU = dbc.QUEUEs.Single(q=>q.ID_QUEU== ID_QUEU).VIS_ALL_QUEU;
                if (false == Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()))
                {


                    int supQueu = 0;
                    try
                    {
                        var Queus = dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
                            .Where(x => x.ID_PERS_ENTI_CORD == ID_PERS_ENTI);

                        supQueu = Queus.Count();

                        numbers = new int[supQueu];

                        foreach (var x in Queus)
                        {
                            //var orderKeys = new int[] { 1, 12, 306, 284, 50047 };
                            numbers[iq] = (int)x.ID_QUEU.Value;

                            iq++;
                            //Customers.Rows.Add(row);
                        }
                    }
                    catch
                    {

                    }

                    if (supQueu == 0)
                    {
                        tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                    }
                    else
                    {
                        //Mostrar el ticket cuya cola es la indicada
                        tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                    }

                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                else
                {
                    ViewBag.PActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                    ViewBag.PScheduled = tick.Where(t => t.ID_STAT_END == 5).Count();
                    ViewBag.PResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                    ViewBag.PClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
                }
                ViewBag.TimeServer = String.Format("{0:MMM d yyyy HH:mm:ss}", DateTime.Now);
                ViewBag.Message = "";

                int idIdioma = Convert.ToInt32(Session["IdIdioma"]);

                string llave = "Asignadoa|Creadopor|Creado|UltimaAct";
                //llave = Convert.ToString(Request.Params["llaves"]);
                string[] llaves = llave.Split('|');

                var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
                            select new
                            {
                                //cadena = id.Texto + "|" + id.Llave
                                llave = id.Llave,
                                texto = id.Texto
                            };

                var xxx = query.ToList();
                //List<string> elementos = new List<string>();
                //string texto = "";
                //foreach (var item in xxx)
                //{
                //    texto = item.texto;
                //    elementos.Add(texto);
                //}

                //string[] arrayTexto = elementos.ToArray();
                ViewBag.Asignadoa = xxx[0].texto;
                ViewBag.Creado = xxx[1].texto;
                ViewBag.UltimaAct = xxx[2].texto;
                ViewBag.Creadopor = xxx[3].texto;

                return View();
            }
            catch (Exception e)
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Tickets_Grupo()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult CountTicketPortal()
        {
            //var Count = 0;
            var tick = dbc.TICKETs.Where(t => t.UserId == null && t.ID_DOCU_SALE == null && t.ID_ENTI_ASSI == null && t.ID_STAT_END == null).ToList();

            var ticks = dbc.TICKETs.Where(t => t.UserId == null && t.ID_DOCU_SALE == null && t.ID_ENTI_ASSI == null && t.ID_STAT_END == null).Count();
            var coun = ticks;

            var tickport = (from di in tick
                            select new
                            {
                                count = coun.ToString(),
                            }).Take(1);

            return Json(new { tkpt = tickport }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Página de descripción de la aplicación.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Página de contacto.";

            return View();
        }

        public ActionResult Client()
        {
            return View();
        }

        public ActionResult IndexUsuario()
        {
            return RedirectToAction("IndexUsuarioExterno");
        }

        public ActionResult IndexUsuarioExterno()
        {
            return View();
        }
        public ActionResult DetailTicket(int id = 0)
        {
            try
            {
                var DETA_TICK = new DETAILS_TICKET();
                DETA_TICK.ID_TICK = id;

                ViewBag.DATE = String.Format("{0:g}", DateTime.Now);
                ViewBag.UploadFile = "M1DT" + Convert.ToString(DateTime.Now.Ticks);

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int permisoOperaciones = 0;

                var Acco = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO);

                var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                              join tt in dbc.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                              select new
                              {
                                  t.ID_TICK,
                                  t.ID_TYPE_TICK,
                                  t.COD_TICK,
                                  NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                                  t.ID_QUEU,
                                  t.AMM_SALE_OPPO,
                                  t.SUM_TICK,
                                  t.ID_COMP,
                                  t.ID_STAT_END,
                                  t.ID_CATE,
                                  t.ID_TICK_PARENT,
                                  t.IS_PARENT,
                                  t.ID_DOCU_SALE
                              }).First();
                int existeCambio = 0;
                int existeCambioInc = 0;
                int idTicketParent = Convert.ToInt32(ticket.ID_TICK_PARENT);
                if (ticket.IS_PARENT == false)
                {
                    existeCambio = dbc.CHANGE_REQUEST.Where(x => x.IdTicket == idTicketParent).Count();
                }
                try
                {
                    existeCambioInc = dbc.CHANGE_REQUEST.Where(x => x.IdTicket == id).Count();
                }
                catch
                {
                    existeCambioInc = 0;
                }
                var cate = dbc.CATEGORies.Single(x => x.ID_CATE == ticket.ID_CATE.Value);
                ViewBag.AplicaGestionActivo = Convert.ToInt32(cate.AplicaGestionActivos);
                ViewBag.time = Acco.TIME_MANUAL;
                ViewBag.NAM_TYPE_TICK = ticket.NAM_TYPE_TICK;
                ViewBag.COD_TICK = ticket.COD_TICK;
                ViewBag.ID_QUEU = ticket.ID_QUEU;
                ViewBag.ID_TICK = id;
                ViewBag.SUM_TICK = ticket.SUM_TICK;
                ViewBag.ID_COMP = ticket.ID_COMP;
                ViewBag.Estado = ticket.ID_STAT_END;
                ViewBag.ExisteCambio = existeCambio;
                ViewBag.ExisteCambioInc = existeCambioInc;
                //Tiene Soporte Electrodata
                var soporte = (dbc.TICKETs.Single(x => x.ID_TICK == id).IdMantED);
                int tieneSoporte = 0;
                if (soporte != null)
                {
                    int idDocuSale = Convert.ToInt32(dbc.TICKETs.Single(x => x.ID_TICK == id).ID_DOCU_SALE);
                    var queryOP = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == idDocuSale);
                    var queryTipoOP = dbc.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == queryOP.ID_TYPE_DOCU_SALE.Value);
                    string numOP = queryTipoOP.NAM_TYPE_DOCU_SALE + " " + queryOP.NUM_DOCU_SALE;
                    tieneSoporte = 1;
                    ViewBag.IdDocuSale = idDocuSale;
                    ViewBag.NumDocuSale = numOP;
                }
                ViewBag.TieneSoporte = tieneSoporte;
                //Aplica Gestión de Cambios
                int AplicaCambio = 0;
                if (ticket.ID_TYPE_TICK == 1 || ticket.ID_TYPE_TICK == 2)
                {
                    AplicaCambio = 1;
                }
                ViewBag.AplicaCambio = AplicaCambio;
                int IdCambio = 0;
                try
                {
                    //if (existeCambio == 1)
                    //    IdCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == idTicketParent).SingleOrDefault().id;
                    //else
                    //    IdCambio = 0; 
                    //if (existeCambioInc == 1)
                    //    IdCambio = db.CHANGE_REQUEST.Where(x => x.IdTicket == id).SingleOrDefault().id;
                    //else
                    //    IdCambio = 0;

                    if (existeCambio == 0)
                    {
                        if (existeCambioInc == 1)
                        {
                            IdCambio = dbc.CHANGE_REQUEST.Where(x => x.IdTicket == id).SingleOrDefault().id;
                        }
                        else
                        {
                            IdCambio = 0;
                        }
                    }
                    else
                    {
                        IdCambio = dbc.CHANGE_REQUEST.Where(x => x.IdTicket == idTicketParent).SingleOrDefault().id;
                    }
                }
                catch
                {
                    IdCambio = 0;
                }

                ViewBag.IdCambio = IdCambio;
                if (ticket.ID_QUEU.Value == 26)
                {
                    try
                    {
                        var xx = dbc.DETAILS_TICKET
                            .Where(x => x.ID_TICK == id)
                            .Where(x => x.ID_STAT_SALE_OPP != null)
                            .OrderByDescending(x => x.ID_DETA_TICK)
                            .First();

                        DETA_TICK.AMM_SALE_OPP = xx.AMM_SALE_OPP.Value;
                        DETA_TICK.PRO_SALE_OPP = xx.PRO_SALE_OPP.Value;
                    }
                    catch
                    {
                        DETA_TICK.AMM_SALE_OPP = ticket.AMM_SALE_OPPO;
                        DETA_TICK.PRO_SALE_OPP = 0;
                    }
                }

                string UserName = Convert.ToString(Session["UserName"]);
                string[] rolesArray = Roles.GetRolesForUser(UserName);

                foreach (string xc in rolesArray)
                {
                    int i = Array.IndexOf(rolesArray, xc);
                    if (xc == "SERVICE DESK")
                    {
                        ViewBag.ACCESO_EDIT = 1;
                    }
                    if (xc == "ADMINISTRADOR")
                    {
                        ViewBag.ACCESO_SEND_SURVEY = 1;
                    }

                }

                //ViewBag.ACCESO_EDIT = Convert.ToInt32(Session["SUPERVISOR_SERVICEDESK"]);
                //Leccion Aprendida

                var nroleccionAprendida = (from la in dbc.ComLeccionAprendidas.Where(x => x.ID_TICKET == id).ToList()
                                           select new
                                           {
                                               titulo = la.Titulo,
                                               cuenta = la.DescripcionProblema
                                           }).ToList();


                string Titulo, DescripcionProblema, SolucionAplicada, Impactonegocio, SolucionTemporal, SolucionDefinitiva,
                    Porque2 = "", Porque3 = "", Porque4 = "", Porque5 = "";
                if (nroleccionAprendida.Count() > 0)
                {
                    int idTicket = ticket.ID_TICK;
                    var leccionAprendida = dbc.ComLeccionAprendidas.Single(t => t.ID_TICKET == idTicket);
                    Titulo = leccionAprendida.Titulo;
                    DescripcionProblema = leccionAprendida.DescripcionProblema;
                    SolucionAplicada = leccionAprendida.SolucionAplicada;
                    Impactonegocio = leccionAprendida.Impactonegocio;
                    SolucionTemporal = leccionAprendida.SolucionTemporal;
                    SolucionDefinitiva = leccionAprendida.SolucionDefinitiva;
                    Porque2 = leccionAprendida.Porque2;
                    Porque3 = leccionAprendida.Porque3;
                    Porque4 = leccionAprendida.Porque4;
                    Porque5 = leccionAprendida.Porque5;
                }
                else
                {
                    Titulo = "";
                    DescripcionProblema = "";
                    SolucionAplicada = "";
                    Impactonegocio = "";
                    SolucionTemporal = "";
                    SolucionDefinitiva = "";
                }

                ViewBag.Titulo = Titulo;
                ViewBag.DescripcionProblema = DescripcionProblema;
                ViewBag.SolucionAplicada = SolucionAplicada;
                ViewBag.Impactonegocio = Impactonegocio;
                ViewBag.SolucionTemporal = SolucionTemporal;
                ViewBag.SolucionDefinitiva = SolucionDefinitiva;
                ViewBag.Porque2 = Porque2;
                ViewBag.Porque3 = Porque3;
                ViewBag.Porque4 = Porque4;
                ViewBag.Porque5 = Porque5;

                /*Gestión Problemas*/
                var tipoTicket = dbc.TICKETs.Where(x => x.FlagProblema == true && x.ID_TICK == id).FirstOrDefault();

                if (tipoTicket != null)
                    return View("IndexProblema", DETA_TICK);
                else
                    return View(DETA_TICK);

                /*Fin*/
            }
            catch
            {
                return Content("Please Close Conecction");
            }
        }

        public ActionResult PaginaInicio()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string Nombre = Session["NAM_PERS"].ToString();
            string Foto = Session["ID_FOTO"].ToString();

            tktInicioContador_Result resultado = dbc.tktInicioContador(IdPersEnti).SingleOrDefault();

            ViewBag.Nombre = Nombre;
            ViewBag.Foto = Foto;
            ViewBag.Ticket = resultado.Ticket;
            ViewBag.Proyectos = resultado.Proyectos;
            ViewBag.Finanzas = resultado.Finanzas;
            ViewBag.Cambios = resultado.Cambios;
            ViewBag.Activos = resultado.Activos;

            return View();
        }

        public ActionResult ListaTicket()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdAreaResponsable = Convert.ToInt32(Session["ID_QUEU"]);

            if (IdCuenta == 4 && IdAreaResponsable == 5) { IdPersEnti = 1007; }

            List<tktInicioOperaciones_Result> resultado = dbc.tktInicioOperaciones(IdCuenta, IdPersEnti).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaTicketProyecto()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdAreaResponsable = Convert.ToInt32(Session["ID_QUEU"]);

            if (IdCuenta == 4 && IdAreaResponsable == 5) { IdPersEnti = 1007; }

            List<tktInicioOperacionesProyecto_Result> resultado = dbc.tktInicioOperacionesProyecto(IdCuenta, IdPersEnti).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaTicketProblema()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            List<tktInicioOperacionesProblema_Result> resultado = dbc.tktInicioOperacionesProblema(IdCuenta, IdPersEnti).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaMisTicket()
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            List<tktInicioOperaciones_Result> resultado = dbc.tktInicioOperacionesMisTickets(IdCuenta, IdPersEnti).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListaTicketsUsuarioExt()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            List<tktInicioOperacionesUsuarioExt_Result> resultado = dbc.tktInicioOperacionesUsuarioExt(4, IdPersEnti).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByIdTick(int id)
        {
            int idTicket = 0;
            textInfo = cultureInfo.TextInfo;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO //&& ce.UserId != null
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                ce.UserId,
                                pe.ID_FOTO,

                            }).ToList();

            var query = dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id).ToList();

            var result = (from di in query.OrderByDescending(di => di.ID_DETA_TICK)
                          join tdt in dbc.TYPE_DETAILS_TICKET on di.ID_TYPE_DETA_TICK equals tdt.ID_TYPE_DETA_TICK
                          join p in listuser on di.UserId equals p.UserId

                          join s in dbc.STATUS on di.ID_STAT equals s.ID_STAT into ls
                          from x in ls.DefaultIfEmpty()

                          join q in dbc.QUEUEs on di.ID_QUEU equals q.ID_QUEU into lq
                          from y in lq.DefaultIfEmpty()

                          join a in listuser on di.ID_PERS_ENTI equals a.ID_PERS_ENTI into la
                          from z in la.DefaultIfEmpty()

                          join sso in dbc.STATUS_SALES_OPPORTUNITY on di.ID_STAT_SALE_OPP equals sso.ID_STAT_SALE_OPPO into lsso
                          from w in lsso.DefaultIfEmpty()

                          join al in dbe.ACTIVITY_LOG.Where(x => x.COD_SUBTYPE_ACT == id || x.ID_TICKET_OP == id) on di.ID_DETA_TICK equals al.DETAILS_TICKETS into det
                          from act in det.DefaultIfEmpty()
                              //join t in db.TICKETs on di.ID_TICK_PARENT equals t.ID_TICK into lt
                              //from k in lt.DefaultIfEmpty()

                          select new
                          {
                              di.ID_DETA_TICK,
                              PERS = p.Client.ToLower(),
                              COM_DETA_INCI = di.COM_DETA_TICK,
                              ID_TYPE_DETA_TICK = di.ID_TYPE_DETA_TICK,
                              NAM_TYPE_DETA_TICK = tdt.NAM_TYPE_DETA_TICK.ToLower(),
                              NAM_STAT = (x == null ? String.Empty : x.NAM_STAT),
                              NAM_QUEU = (y == null ? String.Empty : y.NAM_QUEU),
                              //COD_PARENT = (k == null ? String.Empty : k.COD_TICK),
                              ASSI = (z == null ? String.Empty : z.Client),
                              CREATE_DETA_INCI = String.Format("{0:G}", di.CREATE_DETA_INCI),
                              ADJ = AttaDetaTick(di.ID_DETA_TICK),
                              FEC_SCHE = String.Format("{0:G}", di.FEC_SCHE),
                              ID_PHOT = "1",//p.ID_CLIE,
                              PHOTO = p.ID_FOTO,
                              NAM_STAT_SALE_OPPO = (w == null ? String.Empty : w.NAM_STAT_SALE_OPPO),
                              AMM_SALE_OPP = string.Format("{0:0,0.00}", di.AMM_SALE_OPP == null ? 0 : di.AMM_SALE_OPP),
                              PRO_SALE_OPP = string.Format("{0:0,0.00}", di.PRO_SALE_OPP == null ? 0 : di.PRO_SALE_OPP),
                              INEX = di.ID_DETA_TICK,
                              FROM_TIME = String.Format("{0:d}", di.FROM_TIME) + " " + String.Format("{0:HH:mm}", di.FROM_TIME),
                              TO_TIME = String.Format("{0:d}", di.TO_TIME) + " " + String.Format("{0:HH:mm}", di.TO_TIME),
                              di.FEC_END_REAL,
                              //Cargar Detalle de Actividad
                              ID_ACT_LOG = act == null ? 0 : act.ID_ACTI_LOG,
                              DATE_INIC = (act == null ? "-" : string.Format("{0:G}", act.DATE_INIC)),
                              DATE_END = (act == null ? "-" : string.Format("{0:G}", act.DATE_END)),
                              ID_DET_TICK = (act == null ? 0 : act.DETAILS_TICKETS),
                              MinutosTranscurridos = di.MINUTES == null ? 0 : di.MINUTES,
                              Causa = (di.Causa == null ? "" : (di.Causa == true ? "Causas" : ""))
                          }).ToList();

            var listCIA = (from pe in dbe.PERSON_ENTITY
                           join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               pe.ID_PERS_ENTI,
                               CIA = ce.COM_NAME,
                               idCIA = ce.ID_ENTI
                           }).ToList();

            var Companias = (from c in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null)
                             select new
                             {
                                 c.ID_ENTI,
                                 c.COM_NAME
                             }).ToList();

            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                          join ct in dbc.ComTemas on (t.IdTema == null ? 0 : t.IdTema) equals ct.IdTema into ctxsd  /*Agregado para la gestión del conocimiento*/
                          from ctxs in ctxsd.DefaultIfEmpty()
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          join s in dbc.SOURCEs on t.ID_SOUR equals s.ID_SOUR
                          join st in dbc.STATUS on t.ID_STAT_END equals st.ID_STAT
                          //join aeu in listuser on t.ID_PERS_ENTI_END equals aeu.ID_PERS_ENTI
                          join tt in dbc.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in dbc.CATEGORies on t.ID_CATE equals c4.ID_CATE
                          join c3 in dbc.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in dbc.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in dbc.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE
                          join cb in listuser on t.UserId equals cb.UserId
                          join at in listuser on t.ID_PERS_ENTI_ASSI equals at.ID_PERS_ENTI
                          join c in listCIA on t.ID_PERS_ENTI equals c.ID_PERS_ENTI
                          join ds in dbc.DOCUMENT_SALE on (t.ID_DOCU_SALE == null ? 0 : t.ID_DOCU_SALE) equals ds.ID_DOCU_SALE into lds
                          from xds in lds.DefaultIfEmpty()
                          join cen in Companias on (t.ID_COMP_END == null ? 0 : t.ID_COMP_END) equals cen.ID_ENTI into cend
                          from cenx in cend.DefaultIfEmpty()
                          join tds in dbc.TYPE_DOCUMENT_SALE on (xds == null ? 0 : xds.ID_TYPE_DOCU_SALE) equals tds.ID_TYPE_DOCU_SALE into ltds
                          from xtds in ltds.DefaultIfEmpty()
                              //join di in query.OrderByDescending(di => di.ID_DETA_TICK) on t.ID_TICK equals di.ID_TICK into dit
                              //from ditk in dit.DefaultIfEmpty()
                          select new
                          {
                              t.ID_TICK,
                              Sol = ReturnRequ(Convert.ToInt32(t.ID_PERS_ENTI)),// aeu.Client.ToLower(),
                              AEU = ReturnRequ(Convert.ToInt32(t.ID_PERS_ENTI_END)),// aeu.Client.ToLower(),
                              t.ID_PERS_ENTI_END,
                              NAM_STAT = st.NAM_STAT.ToLower(),
                              ID_STAT = st.ID_STAT,
                              NAM_PRIO = p.NAM_PRIO.ToLower(),
                              REM_CONT = (t.REM_CTRL_TICK == true ? "Yes" : "No"),
                              NAM_SOUR = s.NAM_SOUR.ToLower(),
                              t.COD_TICK,
                              t.SUM_TICK,
                              TIME_USED = timeusedtick(t.ID_TICK),
                              EXP_TIME = tir.ExpirationTime(t.ID_TICK, p.HOU_PRIO.Value),//ExpTime(Convert.ToInt32(t.ID_STAT_END), Convert.ToInt32(t.ID_PRIO), Convert.ToInt32(p.HOU_PRIO), Convert.ToDateTime(t.FEC_TICK), Convert.ToInt32(t.ID_TICK)),
                              SLA = (p.HOU_PRIO == 0 ? "Planning" : Convert.ToString(p.HOU_PRIO)),
                              CREATE_TICK = String.Format("{0:G}", t.FEC_TICK),
                              MODIFIED_TICK = String.Format("{0:G}", t.MODIFIED_TICK),
                              DATE_SCHE = ScheduleDate(Convert.ToInt32(t.ID_TICK)),
                              ATTA_TOT = TotAtta(Convert.ToInt32(t.ID_TICK)),
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              ATTA = AttaTick(Convert.ToInt32(t.ID_TICK)),
                              CREA_BY = cb.Client.ToLower(),
                              ASSI_TO = at.Client.ToLower(),
                              CATE_4 = c4.NAM_CATE.ToLower(),
                              CATE_3 = c3.NAM_CATE.ToLower(),
                              CATE_2 = c2.NAM_CATE.ToLower(),
                              CATE_1 = c1.NAM_CATE.ToLower(),
                              PHOTO = cb.ID_FOTO,
                              COMP = textInfo.ToTitleCase(c.CIA.ToLower()),
                              CIA = c.CIA.ToLower().Substring(0, (c.CIA.Length > 48 ? 48 : c.CIA.Length)) + (c.CIA.Length > 48 ? "..." : ""),
                              ID_ACCO,
                              NUM_OP = (xds == null ? "" : xds.NUM_DOCU_SALE),
                              COD_TYPE_DOCU_SALE = (xtds == null ? "" : xtds.COD_TYPE_DOCU_SALE),
                              ID_DOCU_SALE = (xds == null ? 0 : xds.ID_DOCU_SALE),
                              ID_QUEU = t.ID_QUEU,
                              AMM = string.Format("{0:0,0.00}", Ammount(Convert.ToInt32(t.ID_TICK), Convert.ToInt32(t.ID_QUEU), Convert.ToDecimal(t.AMM_SALE_OPPO))),
                              PROF = string.Format("{0:0,0.00}", Profit(Convert.ToInt32(t.ID_TICK), Convert.ToInt32(t.ID_QUEU), Convert.ToDecimal(t.AMM_SALE_OPPO))),
                              SERVICE = (t.SERVICE == true ? "Yes" : "No"),
                              FEC_INI_REAL = (t.FEC_INI_REAL == null ? "" : t.FEC_INI_REAL.ToString()),
                              FEC_END_REAL = (((from di in query.OrderByDescending(di => di.ID_DETA_TICK)
                                                where t.ID_TICK == di.ID_TICK && di.FEC_END_REAL != null
                                                select di.FEC_END_REAL).FirstOrDefault())).ToString(),
                              COMPEND = (cenx == null ? "-" : cenx.COM_NAME),
                              NAME_TEMA = (ctxs == null ? "" : ctxs.Nomtema)  /*Agregado para la gestión del conocimiento*/
                          }).ToList();



            /*Gestión Problemas*/
            var dataPreLeccionAprendida = dbc.ComLeccionAprendidas.Join(dbc.CATEGORies, cl => cl.Nivel4, c4 => c4.ID_CATE, (cl, c4) => new
            {
                idLeccionAprendida = cl.IdLeccioNAprendida,
                IdTicket = cl.ID_TICKET,
                Titulo = cl.Titulo,
                Categoria = c4.NAM_CATE,
                Descripcion = cl.DescripcionProblema,
                SolucionAplicada = cl.SolucionAplicada,
                Impacto = cl.Impactonegocio,
                SolucionTemporal = cl.SolucionTemporal,
                SolucionDefinitiva = cl.SolucionDefinitiva,
                EstadoAprobacion = cl.EstadoAprobacion,
                IdCuenta = cl.ID_ACCO
            }).Where(l => l.IdCuenta == ID_ACCO);

            var listaLeccion = dataPreLeccionAprendida.Where(x => x.IdTicket == id && x.EstadoAprobacion == "A").ToList();

            if (listaLeccion.Count() != 0)
                idTicket = Convert.ToInt32(listaLeccion.FirstOrDefault().IdTicket);

            var estadoTicket = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TICK == idTicket && t.ID_STAT_END == 4).FirstOrDefault();
            /*Fin*/

            return Json(new { data = result, datadeta = ticket, listaLeccionAprendida = estadoTicket != null ? listaLeccion : null }, JsonRequestBehavior.AllowGet);
        }
        public string Profit(int ID_TICK, int ID_QUEU, decimal AMMO)
        {
            if (ID_QUEU == 26)
            {
                try
                {
                    var xx = dbc.DETAILS_TICKET
                        .Where(x => x.ID_TICK == ID_TICK)
                        .Where(x => x.ID_STAT_SALE_OPP != null)
                        .OrderByDescending(x => x.ID_DETA_TICK)
                        .First();
                    return Convert.ToString(string.Format("{0:0,0.00}", xx.PRO_SALE_OPP.Value));
                }
                catch
                {
                    return Convert.ToString(string.Format("{0:0,0.00}", 0));
                }
            }
            else
            {
                return "0";
            }
        }
        public string Ammount(int ID_TICK, int ID_QUEU, decimal AMMO)
        {
            if (ID_QUEU == 26)
            {
                try
                {
                    var xx = dbc.DETAILS_TICKET
                        .Where(x => x.ID_TICK == ID_TICK)
                        .Where(x => x.ID_STAT_SALE_OPP != null)
                        .OrderByDescending(x => x.ID_DETA_TICK)
                        .First();
                    return Convert.ToString(string.Format("{0:0,0.00}", xx.AMM_SALE_OPP.Value));
                }
                catch
                {
                    return Convert.ToString(string.Format("{0:0,0.00}", AMMO));
                }
            }
            else
            {
                return "0";
            }
        }

        public string AttaDetaTick(int id)
        {
            string Adjun = "";
            try
            {
                var query = (from a in dbc.ATTACHEDs.Where(a => a.ID_DETA_INCI == id)
                             .Where(a => a.DELETE_ATTA == null || a.DELETE_ATTA == false)
                             join tda in dbc.TYPE_DOCUMENT_ATTACH on a.ID_TYPE_DOCU_ATTA equals tda.ID_TYPE_DOCU_ATTA into ltda
                             from xtda in ltda.DefaultIfEmpty()
                             select new
                             {
                                 NAM_ATTA = a.NAM_ATTA,
                                 ID_ATTA = a.ID_ATTA,
                                 EXT_ATTA = a.EXT_ATTA,
                                 NAM_TYPE_DOCU_ATTA = (xtda == null ? "" : xtda.NAM_TYPE_DOCU_ATTA),
                             });

                foreach (var subqu in query)
                {
                    //Adjun = Adjun + "<li><a href='../../Adjuntos/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA +" ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    //Adjun = Adjun + "<li><a href='/DetailsTicket/DescargarArchivo/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    if (subqu.EXT_ATTA.ToLower() == ".pdf" || subqu.EXT_ATTA.ToLower() == ".txt" || subqu.EXT_ATTA.ToLower() == ".png" || subqu.EXT_ATTA.ToLower() == ".jpg")
                    {
                        Adjun = Adjun + "<li><a href='/DetailsTicket/VerDocumento/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' target='_blank'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                    else
                    {
                        Adjun = Adjun + "<li><a href='/DetailsTicket/DescargarArchivo/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        public string ReturnRequ(int idend = 0)
        {
            var listClient = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == idend)
                              join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                              select new
                              {
                                  Client = ce.FIR_NAME + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME),
                              });

            return listClient.First().Client.ToLower();
        }

        public string timeusedtick(int ID_TICK)
        {
            int minutos = 0, horas = 0, dias = 0;
            try
            {
                var query = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                minutos = Convert.ToInt32(query.MINUTES);
                if (minutos >= 60)
                {
                    horas = minutos / 60;
                    minutos = minutos % 60;
                }
                else
                {
                    horas = 0;

                }
            }
            catch
            {
                horas = 0;
                minutos = 0;

            }
            return horas + " Hours " + minutos + " Minutes";
        }

        public String ScheduleDate(int ID_TICK)
        {
            DateTime FechaScheduled;

            try
            {
                FechaScheduled = Convert.ToDateTime(dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                                .Where(x => x.ID_STAT == 5)
                                .Where(x => x.ID_TYPE_DETA_TICK == 3)
                                .OrderByDescending(x => x.CREATE_DETA_INCI)
                                .Take(1).Single().FEC_SCHE);
            }
            catch
            {
                FechaScheduled = Convert.ToDateTime(dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_INI_TICK);
            }

            return String.Format("{0:G}", FechaScheduled);
        }

        public String NewScheduleDate(int ID_TICK)
        {
            DateTime FechaScheduled;

            try
            {
                FechaScheduled = Convert.ToDateTime(dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                                .Where(x => x.ID_STAT == 5)
                                .Where(x => x.ID_TYPE_DETA_TICK == 3)
                                .OrderByDescending(x => x.CREATE_DETA_INCI)
                                .Take(1).Single().FEC_SCHE);
            }
            catch
            {
                FechaScheduled = Convert.ToDateTime(dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_INI_TICK);
            }

            return FechaScheduled.ToString("dd/MM/yyyy");
        }

        public int TotAtta(int id)
        {
            int atta_tick = dbc.ATTACHEDs.Where(a => a.ID_INCI == id).Count();

            int atta_all = (from x in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id)
                            join adt in dbc.ATTACHEDs on x.ID_DETA_TICK equals adt.ID_DETA_INCI
                            select new
                            {
                                adt.ID_ATTA
                            }).Count();

            return atta_tick + atta_all;
        }

        public string AttaTick(int id)
        {
            string Adjun = "";
            try
            {
                var query = (from a in dbc.ATTACHEDs.Where(a => a.ID_INCI == id)
                             .Where(a => a.DELETE_ATTA == null || a.DELETE_ATTA == false)
                             join tda in dbc.TYPE_DOCUMENT_ATTACH on a.ID_TYPE_DOCU_ATTA equals tda.ID_TYPE_DOCU_ATTA into ltda
                             from xtda in ltda.DefaultIfEmpty()
                             select new
                             {
                                 NAM_ATTA = a.NAM_ATTA,
                                 ID_ATTA = a.ID_ATTA,
                                 EXT_ATTA = a.EXT_ATTA,
                                 NAM_TYPE_DOCU_ATTA = (xtda == null ? "" : xtda.NAM_TYPE_DOCU_ATTA),
                             });

                foreach (var subqu in query)
                {
                    //Adjun = Adjun + "<li><a href='../../Adjuntos/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    if (subqu.EXT_ATTA.ToLower() == ".pdf" || subqu.EXT_ATTA.ToLower() == ".txt" || subqu.EXT_ATTA.ToLower() == ".png" || subqu.EXT_ATTA.ToLower() == ".jpg")
                    {
                        Adjun = Adjun + "<li><a href='/DetailsTicket/VerDocumento/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' target='_blank'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                    else
                    {
                        Adjun = Adjun + "<li><a href='/DetailsTicket/DescargarArchivo/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                    }
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }
        public List<TicketsPorGrupo_Result> ConvertirListaGrupoBnvAGenerico(List<TicketsPorGrupoBNV_Result> listaOriginal)
        {
            List<TicketsPorGrupo_Result> tickets = new List<TicketsPorGrupo_Result>();
            foreach (var tick in listaOriginal)
            {
                TicketsPorGrupo_Result ticketsPorGrupo = new TicketsPorGrupo_Result();
                ticketsPorGrupo.asignadoA = tick.asignadoA;
                ticketsPorGrupo.c1 = tick.c1;
                ticketsPorGrupo.c2 = tick.c2;
                ticketsPorGrupo.c3 = tick.c3;
                ticketsPorGrupo.c4 = tick.c4;
                ticketsPorGrupo.COD_TICK = tick.COD_TICK;
                ticketsPorGrupo.COM_NAME = tick.COM_NAME;
                ticketsPorGrupo.creadoPor = tick.creadoPor;
                ticketsPorGrupo.cronograma = tick.cronograma;
                ticketsPorGrupo.expTime = tick.expTime;
                ticketsPorGrupo.fechaCreacion = tick.fechaCreacion;
                ticketsPorGrupo.FEC_INI_TICK = tick.FEC_INI_TICK;
                ticketsPorGrupo.FOT_PERS = tick.FOT_PERS;
                ticketsPorGrupo.HOU_PRIO = tick.HOU_PRIO;
                ticketsPorGrupo.ID_STAT_END = tick.ID_STAT_END;
                ticketsPorGrupo.ID_TICK = tick.ID_TICK;
                ticketsPorGrupo.MODIFIED_TICK = tick.MODIFIED_TICK;
                ticketsPorGrupo.NAM_PRIO = tick.NAM_PRIO;
                ticketsPorGrupo.requeridoPor = tick.requeridoPor;
                ticketsPorGrupo.tiempoTicket = tick.tiempoTicket;
                ticketsPorGrupo.TITLE_TICK = tick.TITLE_TICK;
                tickets.Add(ticketsPorGrupo);
            }
            return tickets;
        }
        public string ScheduleTime(int ID_TICK, DateTime FEC_INI_TICK)
        {
            try
            {
                var query = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                    .Where(x => x.ID_STAT == 5);

                if (query.Count() != 0)
                {
                    var xyz = query.OrderByDescending(x => x.ID_DETA_TICK).First().FEC_SCHE;
                    return String.Format("{0:HH:mm}", Convert.ToDateTime(xyz.Value));
                }
                else
                {
                    return String.Format("{0:HH:mm}", Convert.ToDateTime(FEC_INI_TICK));

                }
            }
            catch
            {
                return "Stopped";
            }

        }
        public int cantidadPintar(int cantidad)
        {
            if (cantidad >= 10)
            {
                return 10;
            }
            else
            {
                return cantidad;
            }
        }

        public ActionResult TicketMantenimiento()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int PuedeVerTodos = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int IdQueu = Convert.ToInt32(Session["ID_QUEU"]);
            var query = dbc.ListaTicketsMantenimiento(IdAcco, IdPersEnti, UserId, PuedeVerTodos, IdQueu, 0).ToList();

            ViewBag.TAActive = query.Where(x => x.ID_STAT_END == 1 || x.ID_STAT_END == 3).Count();
            ViewBag.TAScheduled = query.Where(x => x.ID_STAT_END == 5).Count();
            ViewBag.TAResolved = query.Where(x => x.ID_STAT_END == 4).Count();
            ViewBag.TAClosed = query.Where(x => x.ID_STAT_END == 6).Count();
            return PartialView();
        }

        public ActionResult ListaTicketsMantenimiento()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int PuedeVerTodos = Convert.ToInt32(Session["VIS_ALL_QUEU"]);
            int IdQueu = Convert.ToInt32(Session["ID_QUEU"]);
            int Estado = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query = dbc.ListaTicketsMantenimiento(IdAcco, IdPersEnti, UserId, PuedeVerTodos, IdQueu, Estado).ToList();

            foreach (var tick in query)
            {
                tick.cronograma = NewScheduleDate(tick.ID_TICK);
                tick.tiempoTicket = tick.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(tick.ID_TICK), tick.FEC_INI_TICK.Value) : "";
                tick.expTime = tir.ExpirationTime(tick.ID_TICK, tick.HOU_PRIO.Value);
            }

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Grid = query2, Cantidad = total }, JsonRequestBehavior.AllowGet);
        }

    }
}
