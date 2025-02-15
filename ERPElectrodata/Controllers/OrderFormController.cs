using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using ERPElectrodata.Object;
using ERPElectrodata.Object.Ticket;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data.Entity.Infrastructure;  
using System.Data.Entity;
using System.IO;
using ERPElectrodata.Objects;
using System.Web.Security;
using System.Data.Entity.SqlServer;
using System.Text;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Controllers
{
    [Authorize]
    public class OrderFormController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        public int ctd = 1;
        static string op = "";
        private PointTicket pt = new PointTicket();
        public TicketIR tir = new TicketIR();

        [Authorize]
        public ActionResult Renovation()
        {
            return View();
        }
        //
        // GET: /OrderForm/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaProyectosOld()
        {
            return View();
        }
        //
        // GET: /OrderForm/
        [Authorize]
        public ActionResult ReporteGerencia()
        {
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["PMO_GERENCIA"] == 1)
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                ViewBag.IdPersEnti = IdPersEnti;
                DateTime fecha = new DateTime(DateTime.Now.Year, 1, 1);
                ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);
                ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
                return View();
            }
            else
            {
                return Content("No cuenta con permisos para acceder a esta sección. Contacte al administrador.");
            }
        }

        //
        // GET: /OrderForm/
        [Authorize]
        public ActionResult ReporteGerenciaITO()
        {
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["PMO_GERENCIA_ITO"] == 1)
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                ViewBag.IdPersEnti = IdPersEnti;
                DateTime fecha = new DateTime(DateTime.Now.Year, 1, 1);
                ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);
                ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
                return View();
            }
            else
            {
                return Content("No cuenta con permisos para acceder a esta sección. Contacte al administrador.");
            }
        }

        //
        // GET: /OrderForm/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            try
            {
                ViewBag.ID_DOCU_SALE = id;
                return View();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [Authorize]
        public ActionResult DetalleProyecto(int id)
        {
            try
            {
                if (Convert.ToInt32(Session["USUARIO_EXTERNO_TICKET"]) == 0)
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    int pmo = 0, pmoAdmin = 0, pmoSoporte = 0, informe = 0;

                    if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_PMO"] == 1 || (int)Session["SUPERVISOR_PMO_OUTSOURCING"] == 1)
                    {
                        pmoAdmin = 1;
                    }
                    if ((int)Session["PMO_SOPORTE"] == 1)
                    {
                        pmoSoporte = 1;
                    }
                    if ((int)Session["PROJECTMANAGER"] == 1 || (int)Session["PROJECTMANAGER_OUTSOURCING"] == 1 || (int)Session["FINANZAS"] == 1)
                    {
                        pmo = 1;
                    }
                    if (Convert.ToInt32(Session["ADMINISTRADOR_SISTEMA"]) == 1 || (int)Session["SUPERVISOR_INFORME"] == 1 || (int)Session["INFORME"] == 1)
                    {
                        informe = 1;
                    }

                    ViewBag.ID_DOCU_SALE = id;
                    var op = dbc.ObtenerDetalleProyecto(id).ToList().SingleOrDefault();

                    ViewBag.OP = op.OP;
                    ViewBag.Titulo = op.Titulo;
                    ViewBag.FOTO = op.FotoPM;
                    ViewBag.NOMBRE = op.PM;
                    ViewBag.ESTADO = op.IdEstado;
                    ViewBag.NombreEstado = op.Estado;
                    ViewBag.ColorEstado = op.ColorEstado;
                    ViewBag.Riesgo = op.Riesgo;
                    ViewBag.ColorRiesgo = op.ColorRiesgo;
                    ViewBag.AplicaActa = op.AplicaActa;
                    ViewBag.ADMINPMO = pmoAdmin;
                    ViewBag.PMOSoporte = pmoSoporte;
                    ViewBag.PMO = pmo;
                    ViewBag.Informe = informe;


                    ViewBag.UploadFile = "M1DS" + Convert.ToString(DateTime.Now.Ticks);

                    ViewBag.DATE = String.Format("{0:g}", DateTime.Now);

                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [Authorize]
        public ActionResult DatosProductosServicios(int id)
        {
            try
            {
                ViewBag.ID_DOCU_SALE = id;
                return View();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [Authorize]
        public ActionResult SoporteProyecto(int id)
        {
            try
            {
                if (Convert.ToInt32(Session["USUARIO_EXTERNO_TICKET"]) == 0)
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
                    int pmo = 0, pmoAdmin = 0, pmoSoporte = 0, informe = 0;

                    if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_PMO"] == 1 || (int)Session["SUPERVISOR_PMO_OUTSOURCING"] == 1)
                    {
                        pmoAdmin = 1;
                    }
                    if ((int)Session["PMO_SOPORTE"] == 1)
                    {
                        pmoSoporte = 1;
                    }
                    if ((int)Session["PROJECTMANAGER"] == 1 || (int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
                    {
                        pmo = 1;
                    }
                    if (Convert.ToInt32(Session["ADMINISTRADOR_SISTEMA"]) == 1 || (int)Session["SUPERVISOR_INFORME"] == 1 || (int)Session["INFORME"] == 1)
                    {
                        informe = 1;
                    }

                    ViewBag.ID_DOCU_SALE = id;
                    var op = dbc.ObtenerDetalleProyecto(id).ToList().SingleOrDefault();

                    ViewBag.OP = op.OP;
                    ViewBag.Titulo = op.Titulo;
                    ViewBag.FOTO = op.FotoPM;
                    ViewBag.NOMBRE = op.PM;
                    ViewBag.ESTADO = op.IdEstado;
                    ViewBag.NombreEstado = op.Estado;
                    ViewBag.ColorEstado = op.ColorEstado;
                    ViewBag.AplicaActa = op.AplicaActa;
                    ViewBag.ADMINPMO = pmoAdmin;
                    ViewBag.PMOSoporte = pmoSoporte;
                    ViewBag.PMO = pmo;
                    ViewBag.Informe = informe;

                    ViewBag.UploadFile = "M1DS" + Convert.ToString(DateTime.Now.Ticks);

                    ViewBag.DATE = String.Format("{0:g}", DateTime.Now);

                    //No aplica Soporte ED
                    var proyecto = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).SingleOrDefault();
                    string estadoED = Convert.ToString(proyecto.EstadoSoporteED);
                    string aplicaInfAveria = Convert.ToString(proyecto.AplicaInformeAveria);
                    if (aplicaInfAveria == "True")
                    {
                        aplicaInfAveria = "SI";
                    }
                    else
                    {
                        aplicaInfAveria = "NO";
                    }
                    ViewBag.EstadoSoporteED = estadoED;
                    ViewBag.AplicaInformeAveria = aplicaInfAveria;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
        //
        // GET: /OrderForm/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /OrderForm/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /OrderForm/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /OrderForm/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /OrderForm/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        public ActionResult Gantt(int id)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult CronogramaHistorico(int id)
        {
            int pmo = 0, pmoAdmin = 0;

            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_PMO"] == 1)
            {
                pmoAdmin = 1;
            }
            if ((int)Session["PROJECTMANAGER"] == 1 || (int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
            {
                pmo = 1;
            }

            var ds = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id)
                .Join(dbc.TYPE_DOCUMENT_SALE, x => x.ID_TYPE_DOCU_SALE, td => td.ID_TYPE_DOCU_SALE, (x, td) => new
                {
                    x.ID_STAT_DOCU_SALE,
                    x.NUM_DOCU_SALE,
                    td.COD_TYPE_DOCU_SALE,
                    x.Titulo,
                    x.ID_PERS_ENTI_PM
                }).FirstOrDefault();

            ViewBag.ESTADO = ds.ID_STAT_DOCU_SALE;
            ViewBag.ADMINPMO = pmoAdmin;
            ViewBag.PMO = pmo;
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }
        //
        // POST: /OrderForm/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region "Ticket OP"
        [Authorize]
        public ActionResult TicketOP()
        {
            return View();
        }

        [Authorize]
        public ActionResult ReporteMantenimientos()
        {
            return View();
        }
        [Authorize]
        public ActionResult TicketMantenimiento()
        {
            //int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            //ViewBag.Anio = DateTime.Now.Year;
            //ViewBag.Mes = DateTime.Now.Month;
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);
            ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
            return View();
        }

        [Authorize]
        public ActionResult CronogramaSoporte()
        {
            return View();
        }


        // GET: /Ticket/ListByStatus
        public ActionResult ListByStatus(int id = 0)
        {
            textInfo = cultureInfo.TextInfo;
            ctd = 1;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.SEC_NAME + " " + ce.LAS_NAME + " " + ce.MOT_NAME,
                                ce.UserId,
                            }).ToList();

            var listCIA = (from pe in dbe.PERSON_ENTITY
                           join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               pe.ID_PERS_ENTI,
                               COMPANY = ce.COM_NAME,
                           }).ToList();


            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_DOCU_SALE != null);

            int Count = 0, iq = 0;
            int[] numbers = new int[0];

            if (Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()) == false)
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
                        numbers[iq] = (int)x.ID_QUEU.Value;
                        iq++;
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
            }

            if (ID_STAT == 0)
            {
                tick = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.IS_ROLE_USER).ThenBy(t => t.ID_PRIO).ThenByDescending(t => t.CREATE_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 5)
            {
                tick = tick.Where(i => i.ID_STAT_END == 5);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 4)
            {
                tick = tick.Where(i => i.ID_STAT_END == 4);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 6)
            {
                tick = tick.Where(i => i.ID_STAT_END == 6);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }

            var result = (from i in tick.ToList()
                          join so in dbc.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in dbc.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in dbc.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in dbc.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in dbc.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in dbc.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in dbc.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE
                          join pr in dbc.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listuser on i.UserId equals cr.UserId
                          join ac in dbc.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listuser on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                          join ds in dbc.DOCUMENT_SALE on (i.ID_DOCU_SALE == null ? 0 : i.ID_DOCU_SALE) equals ds.ID_DOCU_SALE into lds
                          from xds in lds.DefaultIfEmpty()
                          join tds in dbc.TYPE_DOCUMENT_SALE on (xds == null ? 0 : xds.ID_TYPE_DOCU_SALE) equals tds.ID_TYPE_DOCU_SALE into ltds
                          from xtds in ltds.DefaultIfEmpty()
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              SUM_INCI_PLAIN = StripHtml(i.SUM_TICK),
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBCLAS = c4.NAM_CATE.ToLower(),
                              NAM_CLAS = c3.NAM_CATE.ToLower(),
                              NAM_SUBC = c2.NAM_CATE.ToLower(),
                              NAM_CATE = c1.ABR_CATE,
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                              REQU = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                              CREATE_INCI = String.Format("{0:d}", i.CREATE_TICK) + " " + String.Format("{0:HH:mm}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:d}", i.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              HOU_PRIO = pr.HOU_PRIO != 0 ? Convert.ToString(pr.HOU_PRIO) + "h" : "",
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client.ToLower(),
                              ACCO = ac.NAM_ACCO,
                              ASSI = asi.Client.ToLower(),
                              EXP_TIME = tir.ExpirationTime(i.ID_TICK, pr.HOU_PRIO.Value),//ExpTime(i.ID_TICK),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none"),
                              COUNTSON = CountTicketSon(i.ID_TICK),
                              CIA = textInfo.ToTitleCase(cia.COMPANY.ToLower()).Substring(0, (cia.COMPANY.Length > 48 ? 48 : cia.COMPANY.Length)) +
                                    (cia.COMPANY.Length > 48 ? "..." : ""),
                              CIA_TOOL = textInfo.ToTitleCase(cia.COMPANY.ToLower()),
                              ID_ACCO,
                              VIS_COMP = ac.VIS_COMP,
                              i.ID_STAT_END,
                              NUM_OP = (xds == null ? "" : xds.NUM_DOCU_SALE),
                              COD_TYPE_DOCU_SALE = (xtds == null ? "" : xtds.COD_TYPE_DOCU_SALE),
                              ID_DOCU_SALE = (xds == null ? 0 : xds.ID_DOCU_SALE),
                              Seque = contador(),
                              DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_TICK), Convert.ToDateTime(i.FEC_INI_TICK), Convert.ToInt32(pr.HOU_PRIO)),
                              DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : "",
                              HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : ""
                          });

            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        public string ReturnRequ(int idend = 0)
        {
            var listClient = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == idend)
                              join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                              select new
                              {
                                  Client = ce.FIR_NAME + " " + ce.SEC_NAME + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME) + " " + ce.MOT_NAME,
                              });

            return listClient.First().Client.ToLower();
        }

        private string StripHtml(string source)
        {
            string output;

            //get rid of HTML tags
            output = Regex.Replace(source, "<[^>]*>", string.Empty);

            //get rid of multiple blank lines
            //output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);
            output = output.Replace("&aacute;", "á");
            output = output.Replace("&eacute;", "é");
            output = output.Replace("&iacute;", "í");
            output = output.Replace("&oacute;", "ó");
            output = output.Replace("&uacute;", "ú");
            output = output.Replace("&Aacute;", "Á");
            output = output.Replace("&Eacute;", "É");
            output = output.Replace("&Iacute;", "Í");
            output = output.Replace("&Oacute;", "Ó");
            output = output.Replace("&Uacute;", "Ú");
            output = output.Replace("&ntilde;", "ñ");
            output = output.Replace("&Ntilde;", "Ñ");
            output = output.Replace("&nbsp;", " ");
            output = output.Replace("&lt;", "<");
            output = output.Replace("&gt;", ">");
            output = output.Replace("&amp;", "&");

            return output;
        }

        public string ScheduleDate(int ID_TICK, DateTime FEC_INI_TICK)
        {
            try
            {
                var query = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                    .Where(x => x.ID_STAT == 5);

                if (query.Count() != 0)
                {
                    var xyz = query.OrderByDescending(x => x.ID_DETA_TICK).First().FEC_SCHE;
                    return String.Format("{0:d}", Convert.ToDateTime(xyz.Value));
                }
                else
                {
                    return String.Format("{0:d}", Convert.ToDateTime(FEC_INI_TICK));
                }
            }
            catch
            {
                return "Stopped";
            }
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

        public int contador()
        {
            return ctd++;
        }

        public int CountTicketSon(int id = 0)
        {
            return dbc.TICKETs.Where(x => x.ID_TICK_PARENT == id).Count();
        }

        //funcion para sacar expiration time
        public string ExpTime(int id)
        {
            try
            {
                var tick = dbc.TICKETs.Single(t => t.ID_TICK == id);
                var prio = dbc.PRIORITies.Single(p => p.ID_PRIO == tick.ID_PRIO);

                if (tick.ID_STAT_END != 4 && tick.ID_STAT_END != 6 && tick.ID_STAT_END != 2 && tick.ID_STAT_END != 5)
                {
                    var time = DateTime.Now.Subtract(Convert.ToDateTime(tick.CREATE_TICK)).Days * 24 +
                                                DateTime.Now.Subtract(Convert.ToDateTime(tick.CREATE_TICK)).Hours;

                    return Convert.ToString(Convert.ToInt32(prio.HOU_PRIO) - Convert.ToInt32(time));
                }
                else //if(tick.ID_STAT_END == 6)
                {
                    return "Stopped";
                }
            }
            catch
            {
                return "0";
            }
        }

        public string DateMaxima(int IdTick, DateTime DatIni, int HH)
        {
            string FecMax = "";
            Double MinTransc = 0;

            var qDet = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == IdTick //&& x.ID_STAT == 5
                );

            if (qDet.Where(x => x.ID_STAT == 5).Where(x => x.ID_TYPE_DETA_TICK == 3).Count() > 0)
            {
                MinTransc = qDet.AsEnumerable().Sum(x => x.MINUTES).Value;
                qDet = qDet.Where(x => x.ID_TYPE_DETA_TICK == 3).OrderByDescending(x => x.ID_DETA_TICK);

                DateTime fecSche = qDet.Where(x => x.ID_STAT == 5).First().FEC_SCHE.Value;
                Double hh = Convert.ToDouble(HH) - (MinTransc / 60);
                FecMax = String.Format("{0:MMM d yyyy HH:mm:ss}", fecSche.AddHours(hh));
            }
            else
            {
                FecMax = String.Format("{0:MMM d yyyy HH:mm:ss}", DatIni.AddHours(HH));
            }
            return FecMax;
        }

        public ActionResult UpdateBarStatus(int id = 0)
        {
            var stados = dbc.STATUS;

            var result = (from s in stados.ToList()
                          where s.ID_STAT != 2
                          select new
                          {
                              s.ID_STAT,
                              s.NAM_STAT,
                              Tickets = TicketsByStatusx(s.ID_STAT, id),
                              Total = TotTicketsByStatusx(s.ID_STAT, id),

                          }).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public int TicketsByStatusx(int id, int tipoTicket)
        {
            int[] numbers = new int[0];
            int iq = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            bool VIS_ALL_QUEU = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

            var cant = dbc.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_STAT_END == id && i.ID_DOCU_SALE != null);

            if (tipoTicket == 0)
            {
                cant = cant.Where(x => x.ID_TYPE_FORM == null);
            }
            if (tipoTicket == 1)
            {
                cant = cant.Where(x => x.ID_TYPE_FORM != null);
            }

            if (VIS_ALL_QUEU == false)
            {
                int supQueu = 0;
                try
                {
                    var Queus = dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
                        .Where(x => x.ID_PERS_ENTI_CORD == ID_ASSI);

                    supQueu = Queus.Count();
                    numbers = new int[supQueu];
                    foreach (var x in Queus)
                    {
                        numbers[iq] = (int)x.ID_QUEU.Value;
                        iq++;
                    }
                }
                catch
                {

                }

                if (supQueu == 0)
                {
                    cant = cant.Where(i => i.ID_PERS_ENTI_ASSI == ID_ASSI);
                }
                else
                {
                    //Mostrar el ticket cuya cola es la indicada
                    cant = cant.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                }

            }

            return cant.Count();
        }

        public int TotTicketsByStatusx(int id, int tipoTicket)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var cant = dbc.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_STAT_END == id && i.ID_DOCU_SALE != null);

            if (tipoTicket == 0)
            {
                cant = cant.Where(x => x.ID_TYPE_FORM == null);
            }
            if (tipoTicket == 1)
            {
                cant = cant.Where(x => x.ID_TYPE_FORM != null);
            }

            return cant.Count();
        }

        #endregion

        #region "Crear OP"
        [Authorize]
        public ActionResult CrearOPSidige()
        {
            return View();
        }

        public ActionResult ResultFindOPSidige()
        {
            try
            {
                int ID_ENTI = Convert.ToInt32(Request.Params["ID_ENTI"]);
                int ID_TYPE_DOCU_SALE = Convert.ToInt32(Request.Params["ID_TYPE_DOCU_SALE"]);
                string NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"]);

                ((IObjectContextAdapter)dbc).ObjectContext.CommandTimeout = 600;
                var query = dbc.LIST_OS(ID_ENTI, ID_TYPE_DOCU_SALE, NUM_DOCU_SALE).ToList();
                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Content(e.InnerException.Message);
            }
        }

        public ActionResult OPFromSIDIGE()
        {
            var OP = new DOCUMENT_SALE();

            OP.ID_COMP = Convert.ToInt32(Request.Params["ID_ENTI"]);
            OP.ID_TYPE_DOCU_SALE = Convert.ToInt32(Request.Params["ID_TYPE_DOCU_SALE"]);
            OP.NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"]);
            OP.OS = Convert.ToString(Request.Params["OS"]);

            var query = dbc.DOCUMENT_SALE.Where(x => x.NUM_DOCU_SALE == OP.NUM_DOCU_SALE && x.ID_TYPE_DOCU_SALE == OP.ID_TYPE_DOCU_SALE).FirstOrDefault();

            string Compania = "", CompaniaFinal = "";
            int IdCompania = 0, IdCompaniaFinal = 0;

            IdCompania = query == null ? 0 : Convert.ToInt32(query.ID_COMP);
            IdCompaniaFinal = query == null ? 0 : Convert.ToInt32(query.ID_COMP_END);

            try
            {
                if (ViewBag.IdCompania != "" && ViewBag.IdCompania != 0 && ViewBag.IdCompania != null)
                {
                    Compania = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == IdCompania).SingleOrDefault().COM_NAME;
                }
                if (ViewBag.IdCompaniaFinal != "" && ViewBag.IdCompaniaFinal != 0 && ViewBag.IdCompaniaFinal != null)
                {
                    CompaniaFinal = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == IdCompaniaFinal).SingleOrDefault().COM_NAME;
                }

                ViewBag.IdCompania = IdCompania;
                ViewBag.IdCompaniaFinal = IdCompaniaFinal;
                ViewBag.Compania = Compania;
                ViewBag.CompaniaFinal = CompaniaFinal;
            }
            catch (Exception ex)
            {

            }

            return View(OP);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult OPFromSIDIGE(DOCUMENT_SALE OP, IEnumerable<HttpPostedFileBase> files)
        {
            int chkProyecto = Convert.ToInt32(Request.Params["chkProyecto"]);

            if (chkProyecto == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('Aviso','" +
                            "Seleccione el Tipo de Proyecto');}window.onload=init;</script>");
            }

            try
            {
                int ID_ENTIe = Convert.ToInt32(Request.Params["ID_COMP_END"]);
                //int ID_COMP_END = 0;
                int ID_DOCU_SALE = 0;
                int ID_USER = 0;
                DateTime EXP_DATE = DateTime.MinValue;

                int ID_ENTI = Convert.ToInt32(Request.Params["ID_COMP"]);
                int ID_TYPE_DOCU_SALE = Convert.ToInt32(Request.Params["ID_TYPE_DOCU_SALE"]);
                string NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"]);
                string OS = Convert.ToString(Request.Params["OS"]);
                string Titulo = "";
                int IdPreventa = 0;
                int IdFormaPago = 0;

                string txtObjetoServicio, txtTipoCambio, txtMontoFacturaDolar, txtMontoFacturaSol, dtFechaInicioContrato,
                dtFechaFinContrato, dtFechaInicioSoporte, dtFechaFinSoporte, ComentarioGobierno;

                DateTime FechaInicioContrato, FechaFinContrato, FechaInicioSoporte, FechaFinSoporte;

                txtObjetoServicio = Request.Params["ObjetoServicio"].ToString();
                txtTipoCambio = Request.Params["TipoCambio"];
                txtMontoFacturaDolar = Request.Params["MontoFacturaDolar"];
                txtMontoFacturaSol = Request.Params["MontoFacturaSol"];
                dtFechaInicioContrato = Request.Params["FechaInicioContrato"];
                dtFechaFinContrato = Request.Params["FechaFinContrato"];
                dtFechaInicioSoporte = Request.Params["FechaInicioSoporte"];
                dtFechaFinSoporte = Request.Params["FechaFinSoporte"];
                ComentarioGobierno = Request.Params["ComentarioGobierno"].ToString();

                if (txtMontoFacturaDolar == "") txtMontoFacturaDolar = "0";
                if (txtMontoFacturaSol == "") txtMontoFacturaSol = "0";
                //int IdComp = 0;

                int UserId = 0;

                try
                {
                    UserId = Convert.ToInt32(Session["UserId"]);
                    if (UserId == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" +
                                        "La sesión se ha terminado. Actualice la página.',0);}window.onload=init;</script>");
                    }
                    ID_USER = UserId;
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','" +
                                        "La sesión se ha terminado. Actualice la página.',0);}window.onload=init;</script>");
                }

                //try
                //{
                //    ID_COMP_END = Convert.ToInt32(Request.Params["ID_COMP_END"]);
                //    Titulo = Convert.ToString(Request.Params["TituloOP"]);

                //    try
                //    {
                //        IdPreventa = Convert.ToInt32(Request.Params["Preventa"]);
                //    }
                //    catch
                //    {
                //        IdPreventa = 0;
                //    }
                //    IdFormaPago = Convert.ToInt32(Request.Params["FormaPago"]);
                //    //IdComp = Convert.ToInt32(Request.Params["IdComp"]);



                if (OP.FechaLimitePM == null || OP.FechaLimitePM == DateTime.MinValue)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('Aviso','" +
                                "Verifique la fecha de Límite');}window.onload=init;</script>");
                }


                if (OP.ID_COMP_END == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('Aviso','" +
                                    "Ingrese un Cliente Final',0);}window.onload=init;</script>");
                }

                if (OP.Titulo.Trim() == null || OP.Titulo.Trim() == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('Aviso','" +
                                    "Ingrese un Título para la OP',0);}window.onload=init;</script>");
                }

                //}
                //catch
                //{
                //    return Content("<script type='text/javascript'> function init() {" +
                //                        "if(top.uploadDone) top.uploadDone('ERROR','" +
                //                        "Validar la información ingresada." + "',0);}window.onload=init;</script>");
                //}

                ((IObjectContextAdapter)dbc).ObjectContext.CommandTimeout = 720;
                ID_DOCU_SALE = dbc.OP_SIDIGE_TO_ITMS(ID_ENTI, ID_TYPE_DOCU_SALE, NUM_DOCU_SALE, OP.ID_COMP_END, OP.FechaLimitePM, OS, ID_USER).First().ID_DOCU_SALE;

                var tds = dbc.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == ID_TYPE_DOCU_SALE);

                try
                {
                    var ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == ID_DOCU_SALE);

                    ds.SUM_DOCU_SALE = OP.SUM_DOCU_SALE.Replace("<br /><br />", "<br />");
                    ds.DAT_REGISTER = DateTime.Now;
                    ds.ID_PERS_ENTI_PREV = OP.ID_PERS_ENTI_PREV;
                    ds.Titulo = OP.Titulo;
                    ds.FormaPago = OP.FormaPago;
                    OP.UserIdModifica = UserId;
                    OP.FECHAMODIFICA = DateTime.Now;
                    ds.EstadoTipoProyecto = chkProyecto;
                    ds.EstadoRenovacion = true;
                    ds.IdCuenta = 4;
                    dbc.Entry(ds).State = EntityState.Modified;
                    dbc.SaveChanges();

                    if (Convert.ToDecimal(txtMontoFacturaDolar) >= 40000 || Convert.ToDecimal(txtMontoFacturaSol) >= 120000)
                    {
                        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

                        DocumentoVenta objDocumentoVenta = new DocumentoVenta();
                        objDocumentoVenta.IdOP = ds.ID_DOCU_SALE;
                        objDocumentoVenta.IdTipoOP = ds.ID_TYPE_DOCU_SALE;
                        objDocumentoVenta.IdCliente = ds.ID_COMP;
                        objDocumentoVenta.IdClienteFinal = ds.ID_COMP_END;
                        objDocumentoVenta.Servicio = txtObjetoServicio;
                        objDocumentoVenta.TipoCambio = Convert.ToDecimal(txtTipoCambio);
                        objDocumentoVenta.MontoDolares = Convert.ToDecimal(txtMontoFacturaDolar);
                        objDocumentoVenta.MontoSoles = Convert.ToDecimal(txtMontoFacturaSol);
                        if (DateTime.TryParse(dtFechaInicioContrato, out FechaInicioContrato))
                        {
                            if (FechaInicioContrato != DateTime.MinValue) { objDocumentoVenta.FechaInicioContrato = FechaInicioContrato; }
                        }
                        if (DateTime.TryParse(dtFechaFinContrato, out FechaFinContrato))
                        {
                            if (FechaFinContrato != DateTime.MinValue) { objDocumentoVenta.FechaFinContrato = FechaFinContrato; }
                        }
                        if (DateTime.TryParse(dtFechaInicioSoporte, out FechaInicioSoporte))
                        {
                            if (FechaInicioSoporte != DateTime.MinValue) { objDocumentoVenta.FechaInicioSoporte = FechaInicioSoporte; }
                        }
                        if (DateTime.TryParse(dtFechaFinSoporte, out FechaFinSoporte))
                        {
                            if (FechaFinSoporte != DateTime.MinValue) { objDocumentoVenta.FechaFinSoporte = FechaFinSoporte; }
                        }
                        objDocumentoVenta.Descripcion = ComentarioGobierno;
                        objDocumentoVenta.FechaCreacion = DateTime.Now;
                        objDocumentoVenta.IdAcco = ID_ACCO;
                        objDocumentoVenta.UserIdCreacion = UserId;
                        objDocumentoVenta.Estado = true;
                        dbc.DocumentoVentas.Add(objDocumentoVenta);
                        dbc.SaveChanges();
                    }
                }
                catch
                {

                }

                try
                {
                    //var email = new SendMail();
                    //email.Document_Sale(ID_DOCU_SALE);

                    //Descomentar
                    dbc.CorreoEnvioCreacionOP(ID_DOCU_SALE);
                }
                catch
                {

                }

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + tds.NAM_TYPE_DOCU_SALE + " " + NUM_DOCU_SALE + "'," + ID_DOCU_SALE.ToString() + ");}window.onload=init;</script>");

            }
            catch (Exception e)
            {
                int ID_USER = 0;

                try
                {
                    ID_USER = Convert.ToInt32(Session["UserId"]);
                    var log = new EXCEPTION();

                    log.DAT_EXCE = DateTime.Now;
                    log.MESSAGE = e.Message;
                    log.UserId = ID_USER;
                    log.DES_EXCE = e.Message;

                    dbl.EXCEPTIONs.Add(log);
                    dbl.SaveChanges();
                }
                catch
                {

                }

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','" + "Contacte al Administrador" + "',0);}window.onload=init;</script>");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AdjuntarArchivosIniciales(IEnumerable<HttpPostedFileBase> files, int IdTipoDocumento, int IdDocuSale)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);

            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        var ATTA = new ATTACHED();
                        ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ATTA.ID_DOCU_SALE = IdDocuSale;
                        ATTA.ID_TYPE_DOCU_ATTA = IdTipoDocumento;
                        ATTA.CREATE_ATTA = DateTime.Now;
                        ATTA.UserId = UserId;
                        ATTA.Indicador = 0;

                        dbc.ATTACHEDs.Add(ATTA);
                        dbc.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/Sales/OP/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                    }
                    catch
                    {

                    }
                }
            }
            return Content("");
        }

        #endregion

        #region "Index OP"

        //Index
        public ActionResult ListarOP(int IdCliente, int IdPM, int IdIng, string OP)
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());

            //var qCIA = dbe.PERSON_ENTITY
            //            .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
            //            {
            //                x.ID_PERS_ENTI,
            //                x.ID_ENTI2,
            //                x.ID_ENTI1,
            //                ClientContact = (ce.FIR_NAME == null ? "" : ce.FIR_NAME) + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME),
            //                ClientEmail = (x.EMA_PERS == null ? "-" : x.EMA_PERS),
            //                ClientJobTitle = (x.CAR_PERS == null ? "-" : x.CAR_PERS).ToLower(),
            //                ClientTel = (x.TEL_PERS == null ? "-" : x.TEL_PERS),
            //                ClientAnexo = (x.EXT_PERS == null ? "-" : x.EXT_PERS),
            //            })
            //            .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI1, ce => ce.ID_ENTI, (x, ce) => new
            //            {
            //                ID_CTTS = x.ID_ENTI2,
            //                ID_COMP = x.ID_ENTI1,
            //                x.ID_PERS_ENTI,
            //                x.ClientContact,
            //                x.ClientEmail,
            //                x.ClientJobTitle,
            //                x.ClientTel,
            //                x.ClientAnexo,
            //                CIA = ce.COM_NAME,
            //                CIA_ADDR = (ce.ADDRESS == null ? "-" : ce.ADDRESS),
            //                CIA_TELE = (ce.TEL_ENTI == null ? "-" : ce.TEL_ENTI),
            //                CIA_RUC = (ce.NUM_TYPE_DI == null ? "" : ce.NUM_TYPE_DI),
            //            });


            if (ID_STAT == 0) { ID_STAT = 1; }

            //int tt = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT).Count();

            //var qCIA1 = dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null);

            int sw = 0;
            //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            //foreach (string rol in rolesArray)
            //{
            //    if (rol == "ASSIGNED OPs" || rol == "ADMINISTRADOR")
            //    {
            //        sw = 1;
            //    }
            //}
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                sw = 1;
            }
            else if ((int)Session["SUPERVISOR_PMO"] == 1 || (int)Session["PMO_SOPORTE"] == 1)
            {
                sw = 2;
            }
            else if ((int)Session["SUPERVISOR_PMO_OUTSOURCING"] == 1)
            {
                sw = 3;
            }

            //if (sw == 1)
            //{   //Tiene el rol y puede ver todas las OPs
            //    var query = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT)
            //                    .OrderByDescending(x => x.DAT_DOCU_SALE).Skip(skip).Take(take).ToList()
            //                 join tds in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
            //                 join b in qCIA1 on a.ID_COMP equals b.ID_ENTI
            //                 join c in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals c.ID_STAT_DOCU_SALE
            //                 join d in qCIA on a.ID_PERS_ENTI_VEND equals d.ID_PERS_ENTI
            //                 join e in qCIA on a.ID_CTTS equals e.ID_CTTS
            //                 join f in dbc.KNOWLEDGEs on a.ID_DOCU_SALE equals f.ID_DOCU_SALE into lf
            //                 from xf in lf.DefaultIfEmpty()
            //                 join g in qCIA1 on a.ID_ENTI equals g.ID_ENTI
            //                 join pm in qCIA on a.ID_PERS_ENTI_PM equals pm.ID_PERS_ENTI into lpm
            //                 from xpm in lpm.DefaultIfEmpty()
            //                 join tk in dbc.TICKETs on a.ID_DOCU_SALE equals tk.ID_DOCU_SALE into ltk
            //                 from xtk in ltk.DefaultIfEmpty()
            //                 select new
            //                 {
            //                     a.ID_DOCU_SALE,
            //                     OS = (a.OS == null ? "" : a.OS),
            //                     EXP_DATE = a.EXP_DATE == null ? "-" : String.Format("{0:d}", a.EXP_DATE),
            //                     CIA = b.COM_NAME.ToLower(),
            //                     CIA_ADDR = (b.ADDRESS == null ? "-" : b.ADDRESS.ToLower()),
            //                     CIA_TELE = (b.TEL_ENTI == null ? "-" : b.TEL_ENTI),
            //                     CIA_RUC = (b.NUM_TYPE_DI == null ? "-" : b.NUM_TYPE_DI),
            //                     NAM_CLIE_CONT = e.ClientContact.ToLower(),
            //                     CLI_EMAI = e.ClientEmail,
            //                     CLI_JOB_TITL = e.ClientJobTitle,
            //                     CLI_TELE = e.ClientTel,
            //                     CLI_PHON_EXTE = e.ClientAnexo,
            //                     tds.COD_TYPE_DOCU_SALE,
            //                     a.NUM_DOCU_SALE,
            //                     DAT_DOCU_SALE = a.DAT_DOCU_SALE == null ? "-" : String.Format("{0:d}", a.DAT_DOCU_SALE),
            //                     a.TIPO_CAMB,
            //                     a.SUM_DOCU_SALE,
            //                     NAM_STAT_DOCU_SALE = c.NAM_STAT_DOCU_SALE.ToLower(),
            //                     VENDOR = d.ClientContact.ToLower(),
            //                     a.ID_STAT_DOCU_SALE,
            //                     NAM_FILE = (xf == null ? "" : (xf.NAM_ATTA == null ? "-" : xf.NAM_ATTA.ToLower())),
            //                     NAM_ATTA = (xf == null ? "" : (xf.NAM_ATTA == null ? "-" : xf.NAM_ATTA.ToLower() + "_" + xf.ID_KNOW.ToString() + ".pdf")),
            //                     DATE_ATTA = (xf == null ? "" : (xf.DATE_ATTA == null ? "-" : String.Format("{0:d}", xf.DATE_ATTA))),
            //                     COM_NAME = g.COM_NAME.ToLower(),
            //                     TITULO = (a.Titulo == null ? a.NUM_DOCU_SALE : a.Titulo),
            //                     PM = (xpm == null ? "PENDIENTE" : xpm.ClientContact.ToUpper()),
            //                     INGENIERO = (xtk == null ? "PENDIENTE" : (qCIA.Where(x=>x.ID_PERS_ENTI == xtk.ID_PERS_ENTI_ASSI).FirstOrDefault().ClientContact.ToUpper()))
            //                 }).OrderByDescending(x => x.NUM_DOCU_SALE);

            //    return Json(new { Data = query, Count = tt }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    if (ID_STAT == 1) //no tiene el rol por lo tanto no puede ver las OPs Open
            //    {
            //        var query = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == 0) select a);
            //        return Json(new { Data = query, Count = 0 }, JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {//no tiene el rol pero puede ver las asignadas a el y a su personal
            //        int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            //        var listIPE = dbe.LIST_STAFF_BY_BOSS(ID_PERS_ENTI).ToList();

            //        var qry = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT).ToList()
            //                   join t in dbc.TICKETs on a.ID_DOCU_SALE equals t.ID_DOCU_SALE
            //                   where (from lb in listIPE select lb.ID_PERS_ENTI).Contains(Convert.ToInt32(t.ID_PERS_ENTI_ASSI))
            //                   select a);

            //        tt = qry.Count();

            //        var query = (from a in qry.Skip(skip).Take(take).ToList()
            //                     join tds in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
            //                     join b in qCIA1 on a.ID_COMP equals b.ID_ENTI
            //                     join c in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals c.ID_STAT_DOCU_SALE
            //                     join d in qCIA on a.ID_PERS_ENTI_VEND equals d.ID_PERS_ENTI
            //                     join e in qCIA on a.ID_CTTS equals e.ID_CTTS
            //                     join f in dbc.KNOWLEDGEs on a.ID_DOCU_SALE equals f.ID_DOCU_SALE into lf
            //                     from xf in lf.DefaultIfEmpty()
            //                     join g in qCIA1 on a.ID_ENTI equals g.ID_ENTI
            //                     join pm in qCIA on a.ID_PERS_ENTI_PM equals pm.ID_PERS_ENTI into lpm
            //                     from xpm in lpm.DefaultIfEmpty()
            //                     join tk in dbc.TICKETs on a.ID_DOCU_SALE equals tk.ID_DOCU_SALE into ltk
            //                     from xtk in ltk.DefaultIfEmpty()
            //                     select new
            //                     {
            //                         a.ID_DOCU_SALE,
            //                         OS = (a.OS == null ? "" : a.OS),
            //                         EXP_DATE = a.EXP_DATE == null ? "-" : String.Format("{0:d}", a.EXP_DATE),
            //                         CIA = b.COM_NAME.ToLower(),
            //                         CIA_ADDR = (b.ADDRESS == null ? "-" : b.ADDRESS.ToLower()),
            //                         CIA_TELE = (b.TEL_ENTI == null ? "-" : b.TEL_ENTI),
            //                         CIA_RUC = (b.NUM_TYPE_DI == null ? "-" : b.NUM_TYPE_DI),
            //                         NAM_CLIE_CONT = e.ClientContact.ToLower(),
            //                         CLI_EMAI = e.ClientEmail,
            //                         CLI_JOB_TITL = e.ClientJobTitle,
            //                         CLI_TELE = e.ClientTel,
            //                         CLI_PHON_EXTE = e.ClientAnexo,
            //                         tds.COD_TYPE_DOCU_SALE,
            //                         a.NUM_DOCU_SALE,
            //                         DAT_DOCU_SALE = a.DAT_DOCU_SALE == null ? "-" : String.Format("{0:d}", a.DAT_DOCU_SALE),
            //                         a.TIPO_CAMB,
            //                         a.SUM_DOCU_SALE,
            //                         NAM_STAT_DOCU_SALE = c.NAM_STAT_DOCU_SALE.ToLower(),
            //                         VENDOR = d.ClientContact.ToLower(),
            //                         a.ID_STAT_DOCU_SALE,
            //                         NAM_FILE = (xf == null ? "" : (xf.NAM_ATTA == null ? "-" : xf.NAM_ATTA.ToLower())),
            //                         NAM_ATTA = (xf == null ? "" : (xf.NAM_ATTA == null ? "-" : xf.NAM_ATTA.ToLower() + "_" + xf.ID_KNOW.ToString() + ".pdf")),
            //                         DATE_ATTA = (xf == null ? "" : (xf.DATE_ATTA == null ? "-" : String.Format("{0:d}", xf.DATE_ATTA))),
            //                         COM_NAME = g.COM_NAME.ToLower(),
            //                         TITULO = (a.Titulo == null ? a.NUM_DOCU_SALE : a.Titulo),
            //                         PM = (xpm == null ? "PENDIENTE" : xpm.ClientContact.ToUpper()),
            //                         INGENIERO = (xtk == null ? "PENDIENTE" : (qCIA.Where(x => x.ID_PERS_ENTI == xtk.ID_PERS_ENTI_ASSI).FirstOrDefault().ClientContact.ToUpper()))
            //                     }).OrderByDescending(x => x.NUM_DOCU_SALE);

            //        return Json(new { Data = query, Count = tt }, JsonRequestBehavior.AllowGet);
            //    }
            //}

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.OPListar(ID_PERS_ENTI, ID_STAT, sw, IdCliente, IdPM, IdIng, OP, IdCuenta).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewDetailArticleService(int id = 0, string proc = "")
        {
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.Procedencia = proc;
            return View();
        }

        public ActionResult ViewListTicket(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult UpdateBarStatusOP()
        {
            int clienteFinal = Convert.ToInt32(Request.QueryString["clienteFinal"]);
            int PMAsignado = Convert.ToInt32(Request.QueryString["IdPM"]);
            int Especialista = Convert.ToInt32(Request.QueryString["IdING"]);
            string NombreOP = Convert.ToString(Request.QueryString["NombreOP"]);

            var status = dbc.STATUS_DOCUMENT_SALE;

            var result = (from s in status.ToList()
                          select new
                          {
                              s.ID_STAT_DOCU_SALE,
                              s.NAM_STAT_DOCU_SALE,
                              OPs = OpsByStatus(s.ID_STAT_DOCU_SALE, clienteFinal, PMAsignado, Especialista, NombreOP),
                              Total = TTOpsByStatus(s.ID_STAT_DOCU_SALE)
                          }).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public int OpsByStatus(int ID_STAT_DOCU_SALE, int clienteFinal, int PMAsignado, int Especialista, string NombreOP)
        {
            int flag = 0;
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                flag = 1;
            }
            else if ((int)Session["SUPERVISOR_PMO"] == 1 || (int)Session["PMO_SOPORTE"] == 1)
            {
                flag = 2;
            }
            else if ((int)Session["SUPERVISOR_PMO_OUTSOURCING"] == 1)
            {
                flag = 3;
            }

            if (ID_STAT_DOCU_SALE == 0)
            {
                ID_STAT_DOCU_SALE = 1;
            }
            var cant = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.ID_PERS_ENTI_VEND != 28400);
            if (flag == 1)
            {
                cant = cant.Where(x => x.IdCuenta == IdCuenta);
            }
            else if (flag == 2)
            {
                cant = cant.Where(x => x.EstadoTipoProyecto == 1);
            }
            else if (flag == 3)
            {
                cant.Where(x => x.EstadoTipoProyecto == 2);
            }



            else
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                int pm = 0;
                if ((int)Session["PROJECTMANAGER"] == 1)
                {
                    pm = 1;
                }
                else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
                {
                    pm = 2;
                }

                var tpm = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.ID_PERS_ENTI_PM == ID_PERS_ENTI && x.IdCuenta == IdCuenta && x.EstadoTipoProyecto == pm);





                return tpm.Count();
            }

            if (clienteFinal != 0)
            {
                cant = cant.Where(i => i.ID_COMP == clienteFinal);
            }
            if (PMAsignado != 0)
            {
                cant = cant.Where(i => i.ID_PERS_ENTI_PM == PMAsignado);
            }
            if (Especialista != 0)
            {
                cant = cant.Where(i => i.ID_PERS_ENTI_PREV == Especialista);
            }
            if (NombreOP != "")
            {
                cant = cant.Where(i => i.SUM_DOCU_SALE.ToLower().Contains(NombreOP.ToLower()));
            }
            return cant.Count();
            //}
        }

        public int TTOpsByStatus(int ID_STAT_DOCU_SALE)
        {
            //int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            //int total = 0;

            //if ((int)Session["PROJECTMANAGER"] == 1)
            //{
            //    total = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.IdCuenta == IdCuenta && x.EstadoTipoProyecto == 1).Count();
            //}
            //else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
            //{
            //    total = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.IdCuenta == IdCuenta && x.EstadoTipoProyecto == 2).Count();
            //}
            //else
            //{
            //    total = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.IdCuenta == IdCuenta).Count();
            //}

            //return total;
            int flag = 0;
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                flag = 1;
            }
            else if ((int)Session["SUPERVISOR_PMO"] == 1 || (int)Session["PMO_SOPORTE"] == 1)
            {
                flag = 2;
            }
            else if ((int)Session["SUPERVISOR_PMO_OUTSOURCING"] == 1)
            {
                flag = 3;
            }

            if (ID_STAT_DOCU_SALE == 0)
            {
                ID_STAT_DOCU_SALE = 1;
            }

            if (flag == 1)
            {
                return dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.IdCuenta == IdCuenta).Count();
            }
            else if (flag == 2)
            {
                return dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.IdCuenta == IdCuenta && x.EstadoTipoProyecto == 1).Count();
            }
            else if (flag == 3)
            {
                return dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.IdCuenta == IdCuenta && x.EstadoTipoProyecto == 2).Count();
            }
            else
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                int pm = 0;
                if ((int)Session["PROJECTMANAGER"] == 1)
                {
                    pm = 1;
                }
                else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
                {
                    pm = 2;
                }

                int tpm = dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE && x.ID_PERS_ENTI_PM == ID_PERS_ENTI && x.IdCuenta == IdCuenta && x.EstadoTipoProyecto == pm).Count();

                //int ting = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE)
                //            join b in dbc.TICKETs.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI) on
                //                       a.ID_DOCU_SALE equals b.ID_DOCU_SALE
                //            select new
                //            {
                //                a.ID_DOCU_SALE,
                //            }).Count();

                int tt = tpm;

                return tt;
            }
        }

        #endregion

        #region "Detalle"
        public ActionResult ObtenerDatosComercial(int id = 0)
        {
            CultureInfo ci = (CultureInfo)this.Session["Culture"];
            var query = dbc.OPObtenerDatosComercial(id);

            var query2 = (from a in dbc.ATTACHEDs.Where(x => x.ID_DOCU_SALE == id && x.ID_TYPE_DOCU_ATTA == null)
                          select new
                          {
                              ARCHIVO = a.NAM_ATTA + a.EXT_ATTA
                          });

            return Json(new { Data = query, Data2 = query2 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OPObtenerDatosComercial(int id = 0)
        {
            var query = dbc.ObtenerDatosComercial(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosProyecto(int id = 0)
        {

            var query = dbc.ObtenerDatosProyecto(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditarTituloOP(int id = 0)
        {
            string Titulo = Request.Params["Titulo"].ToString();
            int userId = (int)Session["UserId"];

            DOCUMENT_SALE OP = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).FirstOrDefault();
            OP.Titulo = Titulo;
            OP.UserIdModifica = userId;
            OP.FECHAMODIFICA = DateTime.Now;
            dbc.Entry(OP).State = EntityState.Modified;
            dbc.SaveChanges();

            return Json(new { Data = 1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditarRiesgoOP(int id = 0, int id1 = 0)
        {
            int userId = (int)Session["UserId"];

            DOCUMENT_SALE OP = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).FirstOrDefault();
            OP.Impacto = id1;
            OP.UserIdModifica = userId;
            OP.FECHAMODIFICA = DateTime.Now;
            dbc.Entry(OP).State = EntityState.Modified;
            dbc.SaveChanges();

            return Json(new { Data = 1 }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Opciones"

        public ActionResult EditarTitulo(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            var op = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).SingleOrDefault();
            ViewBag.Titulo = op.Titulo;
            return View();
        }

        public ActionResult VerComentarios(int id = 0, int id1 = 0, int id2 = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.PerfilPMO = id1;
            ViewBag.Estado = id2;
            return View();
        }

        public ActionResult ObtenerComentarios(int id = 0)
        {
            var query = dbc.ObtenerComentariosProyecto(id, 1);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerAcciones(int id = 0, int id1 = 0, int id2 = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.PerfilPMO = id1;
            ViewBag.Estado = id2;
            return View();
        }


        public ActionResult ObtenerAcciones(int id = 0)
        {
            var query = dbc.ObtenerComentariosProyecto(id, 7);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerSoporte(int id = 0, int id1 = 0, int id2 = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.PerfilPMO = id1;
            ViewBag.Estado = id2;
            return View();
        }

        public ActionResult ObtenerSoporte(int id = 0)
        {
            var query = dbc.ObtenerComentariosProyecto(id, 8);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsignarPM(int id = 0)
        {

            string PMAsignado = "";
            string TipoProyecto = "";
            int IdPMAsignado = 0;
            int IdTipoProyecto = 0;
            ViewBag.ID_DOCU_SALE = id;
            var queryDocuSale = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).FirstOrDefault();
            try
            {
                ViewBag.IdPMAsignado = queryDocuSale.ID_PERS_ENTI_PM == null ? 0 : queryDocuSale.ID_PERS_ENTI_PM;
                if (ViewBag.IdPMAsignado != null)
                {
                    IdPMAsignado = queryDocuSale.ID_PERS_ENTI_PM.Value;
                    var queryPE = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPMAsignado).SingleOrDefault();
                    var queryPM = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == queryPE.ID_ENTI2.Value).SingleOrDefault();
                    PMAsignado = Convert.ToString(queryPM.FIR_NAME + ' ' + queryPM.LAS_NAME);
                    ViewBag.PMAsignado = PMAsignado;
                }
                ViewBag.IdTipoProyecto = queryDocuSale.IdTipoProyecto == null ? 0 : queryDocuSale.IdTipoProyecto;
                if (ViewBag.IdTipoProyecto != null)
                {
                    IdTipoProyecto = queryDocuSale.IdTipoProyecto.Value;
                    var queryTP = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 43 && x.ID_ACCO_PARA == IdTipoProyecto).SingleOrDefault();
                    TipoProyecto = queryTP.VAL_ACCO_PARA;
                    ViewBag.TipoProyecto = TipoProyecto;
                }

                //Ticket
                var ticket = dbc.TICKETs.Where(x => x.ID_DOCU_SALE == id && x.ID_QUEU != null && x.ID_PERS_ENTI_ASSI != null).FirstOrDefault();
                int ID_QUEU = 0, ID_PERS_ENTI_ASSI = 0, IdCuenta = 0;
                string area = "", areaPersona = "";
                if (ticket != null)
                {
                    ViewBag.IdQueue = ticket.ID_QUEU;
                    ViewBag.IdPersEntiAssi = ticket.ID_PERS_ENTI_ASSI;
                    ID_QUEU = Convert.ToInt32(ticket.ID_QUEU);
                    ID_PERS_ENTI_ASSI = Convert.ToInt32(ticket.ID_PERS_ENTI_ASSI);
                    IdCuenta = Convert.ToInt32(ticket.ID_ACCO);
                    var qu = dbc.QUEUEs.Where(x => x.ID_QUEU == ID_QUEU).SingleOrDefault();
                    var aq = dbc.ACCOUNT_QUEUE.Where(x => x.ID_QUEU == qu.ID_QUEU && x.ID_ACCO == IdCuenta).SingleOrDefault();
                    var ac = dbc.ACCOUNTs.Where(x => x.ID_ACCO == aq.ID_ACCO).SingleOrDefault();

                    area = ac.ACR_ACCO + '.' + qu.NAM_QUEU + " - " + (qu.DES_QUEU == null ? "" : qu.DES_QUEU.ToLower());
                    var personPE = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI_ASSI).SingleOrDefault();
                    var personPM = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == personPE.ID_ENTI2.Value).SingleOrDefault();
                    areaPersona = Convert.ToString(personPM.FIR_NAME + ' ' + personPM.LAS_NAME); ;
                }
                ViewBag.IdQueue = ID_QUEU;
                ViewBag.Area = area;
                ViewBag.IdPersEntiAssi = ID_PERS_ENTI_ASSI;
                ViewBag.AreaPersonaAsignado = areaPersona;


            }
            catch
            {
            }

            var query = dbc.TICKETs.Where(x => x.ID_DOCU_SALE == id).ToList();
            ViewBag.TieneIng = query.Count();

            PMO pmo = new PMO();
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            int flag = 0;
            if (queryDocuSale.EstadoTipoProyecto == 1)
            {
                flag = 1;
                pmo.ListarPorCola = dbc.ListarPmoAsignados(1, IdAcco, "");
            }
            else if (queryDocuSale.EstadoTipoProyecto == 2)
            {
                flag = 2;
                pmo.ListarPorCola = dbc.ListarPmoAsignados(2, IdAcco, "");
            }
            ViewBag.PmoOutsourcing = flag;
            //pmo.ListarPorCola = dbc.ListarPmoAsignados(IdPersEnti, IdAcco, "").ToList();
            pmo.ListarTipoProyecto = dbc.ListarTipoProyecto(43, "").ToList();
            pmo.idTipoProyecto = IdTipoProyecto;
            pmo.idPM = IdPMAsignado;
            return View(pmo);
        }

        //public ActionResult GenerarCronograma(int id = 0)
        //{
        //    ViewBag.ID_DOCU_SALE = id;
        //    var query = dbc.Cronogramas.Where(x => x.IdDocuSale == id);
        //    int m = 1;
        //    if (query.Count() > 0)
        //    {
        //        m = 0;
        //    }

        //    ViewBag.MostrarFechaInicio = m;
        //    Cronograma cro = new Cronograma();
        //    cro.ListarTipoActividad = dbc.ListarTipoActividad("", 50).ToList();
        //    return View(cro);
        //}

        public ActionResult ReprogramarCronograma(int id = 0, int id1 = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.IdCronograma = id1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarResponsable(PMO pmo, FormCollection collection)
        {


            int UserId = Convert.ToInt32(Session["UserId"]);
            int OP = Convert.ToInt32(Request.Params["Id"]);
            int IdPM = Convert.ToInt32(Request.Params["IdPM"]);
            string strIdTipoProyecto = Request.Params["IdTipoProyecto"].ToString();
            int sw = 0;

            //Trae los datos de la OP
            var ds = dbc.DOCUMENT_SALE.SingleOrDefault(x => x.ID_DOCU_SALE == OP);
            if (Convert.ToInt32(ds.EstadoTipoProyecto) == 2)
            {
                sw = 1;
            }

            int idTipoProyecto = 0;
            if (!String.IsNullOrEmpty(strIdTipoProyecto))
            {
                idTipoProyecto = Convert.ToInt32(strIdTipoProyecto);
            }
            //Verifica si existe el proyecto con el PM seleccionado
            var queryPM = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == OP && x.ID_PERS_ENTI_PM == IdPM);
            int contador = queryPM.Count();

            int IdAreaResponsable = 0, diasCronograma = 0;
            int IdIng = 0, flag = 0;
            string Aplica = "false", aplicaCronograma = "false"; //false 

            string Titulo = "";
            int? ID_COMP = 0;
            //Valida si ya tiene un especialista asignado al proyecto
            var query = dbc.TICKETs.Where(x => x.ID_DOCU_SALE == OP).ToList();

            try
            {
                if (sw == 0)
                {
                    //Si no tiene un especialista asignado
                    if (query.Count == 0)
                    {
                        Aplica = Convert.ToString(collection["Aplica"]); //false
                        if (Aplica == "true" && Convert.ToInt32(Request.Params["Asignado"]) != 0)
                        {
                            IdAreaResponsable = Convert.ToInt32(Request.Params["AreaResp"]);
                            IdIng = Convert.ToInt32(Request.Params["Asignado"]);
                        }
                        else
                            if (Aplica == "false")
                        {
                            IdAreaResponsable = 0;
                            IdIng = 0;
                        }
                        else
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.mostrarMensaje) top.mostrarMensaje('ERROR','1');}window.onload=init;</script>");
                        }
                    }
                }
            }
            catch { flag = 1; }

            // Valida si ya tiene los dias ingresados para generar el cronograma
            var queryCronograma = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == OP).SingleOrDefault().AplicaCronograma;

            try
            {
                //Si no tiene registrados los dias para generar el cronograma
                if (queryCronograma == true)
                {
                    aplicaCronograma = "true";
                    var diaCronograma = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == OP).SingleOrDefault().DiasCronograma;
                    diasCronograma = Convert.ToInt32(diaCronograma);
                }
                else if (queryCronograma != true)
                {
                    aplicaCronograma = Convert.ToString(collection["DiasCronograma"]);
                    if (aplicaCronograma == "true" && Request.Params["txtDias"] != "")
                    {
                        diasCronograma = Convert.ToInt32(Request.Params["txtDias"]);
                        ds.FechaRegCronograma = DateTime.Now;
                    }
                    else
                        if (aplicaCronograma == "false")
                    {
                        diasCronograma = 0;
                    }
                    else
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.mostrarMensaje) top.mostrarMensaje('ERROR','2');}window.onload=init;</script>");
                    }
                }
                else
                {
                    var diaCronograma = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == OP).SingleOrDefault().DiasCronograma;
                    diasCronograma = Convert.ToInt32(diaCronograma);
                }
            }
            catch { flag = 1; }
            //Modifica los datos de PM y Tipo del Proyecto
            ds.ID_PERS_ENTI_PM = IdPM;
            ds.IdTipoProyecto = Convert.ToInt32(idTipoProyecto);
            Titulo = (String.IsNullOrEmpty(ds.Titulo) ? "-" : ds.Titulo);
            ID_COMP = ds.ID_COMP;
            ds.AplicaCronograma = (aplicaCronograma == "false" ? false : true);
            ds.DiasCronograma = diasCronograma;
            int EstadoDS = Convert.ToInt32(ds.ID_STAT_DOCU_SALE.Value);
            if (EstadoDS == 1)
            {
                ds.ID_STAT_DOCU_SALE = 2; //Asignado
            }
            if (query.Count == 0)
            {
                ds.AplicaIng = (Aplica == "false" ? false : true);
            }
            dbc.Entry(ds).State = EntityState.Modified;
            dbc.SaveChanges();

            var tds = dbc.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == ds.ID_TYPE_DOCU_SALE);

            //Ticket

            int ID_PERS_ENTI = 0;
            if (sw == 1)
            {
                Aplica = "true";
            }

            if (Aplica == "true" && query.Count == 0 && flag == 0)
            {
                if (ds.ID_COMP_END != null)
                {
                    var PersonEntity = dbe.PERSON_ENTITY.Where(x => x.ID_ENTI2 == ds.ID_CTTS).FirstOrDefault();
                    ID_PERS_ENTI = PersonEntity.ID_PERS_ENTI;
                }

                //Genera el ticket

                int ID_CHAR = 0;
                try
                {
                    ID_CHAR = dbe.PERSON_CONTRACT.Single(x => x.LAS_CONT == true && x.ID_PERS_CONT == IdIng).ID_CHAR.Value;
                }
                catch { ID_CHAR = 0; }

                var ticket = new TICKET();
                if (query.Count() == 0)
                {
                    if (sw == 1)
                    {
                        var q = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPM).SingleOrDefault();
                        IdAreaResponsable = Convert.ToInt32(q.ID_QUEU);
                        IdIng = IdPM;
                    }
                    ticket.ID_QUEU = IdAreaResponsable;
                    ticket.ID_PERS_ENTI_ASSI = IdIng;
                }
                ticket.ID_DOCU_SALE = OP;
                ticket.ID_ACCO = 4;
                ticket.ID_TYPE_TICK = 3;
                ticket.ID_PERS_ENTI = ID_PERS_ENTI;
                ticket.ID_PERS_ENTI_END = ID_PERS_ENTI;
                ticket.ID_COMP = ID_COMP;
                ticket.ID_AREA = ID_CHAR;
                ticket.ID_PRIO = 5;
                ticket.IdSLA = 1;
                ticket.ID_STAT = 1;
                ticket.ID_STAT_END = 5;
                ticket.ID_SOUR = 4;
                ticket.FEC_TICK = DateTime.Now;
                ticket.SUM_TICK = Titulo;
                ticket.REM_CTRL_TICK = false;
                ticket.UserId = Convert.ToInt32(Session["UserId"]);
                ticket.CREATE_TICK = DateTime.Now;
                ticket.FEC_INI_TICK = ds.EXP_DATE;
                ticket.MODIFIED_TICK = DateTime.Now;
                ticket.IS_PARENT = true;
                ticket.ID_CATE = 5866; //Revisar Proyecto
                ticket.SEND_MAIL = false;
                if (IdIng != 0)
                {
                    dbc.TICKETs.Add(ticket);
                    dbc.SaveChanges();
                }

                //int id = Convert.ToInt32(ticket.ID_TICK);
                //dbc.Entry(ticket).State = EntityState.Detached;
            }

            if (contador == 0)
            {
                //Enviar Correo
                string NombreOP = tds.NAM_TYPE_DOCU_SALE + " " + ds.NUM_DOCU_SALE + " " + (ds.Titulo == null ? "" : ds.Titulo);
                //mail Descomentar
                SendMail mail = new SendMail();
                mail.EnviarCorreoPMO(NombreOP, IdPM, 0, UserId);

                //Enviar Correo de Capacidad 
                if (IdIng != 0)
                {
                    var ticketsOP = dbc.TICKETs.Where(x => x.ID_DOCU_SALE == null && (x.ID_STAT_END == 1 || x.ID_STAT_END == 3 || x.ID_STAT_END == 5) && x.ID_PERS_ENTI_ASSI == IdIng).ToList();
                    if (ticketsOP.Count() > 6)
                    {
                        //Descomentar
                        mail.EnviarCorreoCapacidadOP(IdIng, ticketsOP.Count());
                    }
                }
            }

            #region DocumentosObligatorios

            IQueryable<TYPE_DOCUMENT_ATTACH> documentos = from doc in dbc.TYPE_DOCUMENT_ATTACH
                                                          where doc.VIG_TYPE_DOCU_ATTA == true
                                                          select doc;

            TYPE_DOCUMENT_ATTACH[] documentosArray = documentos.ToArray();
            string chkDocs = "";
            foreach (TYPE_DOCUMENT_ATTACH doc in documentosArray)
            {
                if (Convert.ToString(collection[doc.NAM_TYPE_DOCU_ATTA]) != null)
                {
                    chkDocs = chkDocs + doc.ID_TYPE_DOCU_ATTA + ",";
                }

            }

            if (chkDocs.Length > 0)
            {
                chkDocs = chkDocs.Substring(0, chkDocs.Length - 1);
                string[] idsDocs = chkDocs.Split(',');

                foreach (string id in idsDocs)
                {
                    int ID = Convert.ToInt32(id);
                    #region Validar si existe el documento asignado como obligatorio al proyecto
                    var queryDocs = dbc.DocumentosObligatorios.Where(x => x.IdTypeDocuAtta == ID && x.IdDocuSale == OP && x.Estado == true);
                    var queryDocsSing = queryDocs.SingleOrDefault();
                    int cantDocs = queryDocs.Count();
                    if (queryDocs.Count() >= 1)
                    {
                        var objDocumentosObligatorios = dbc.DocumentosObligatorios.Where(x => x.IdTypeDocuAtta == ID && x.IdDocuSale == OP && x.Estado == true).SingleOrDefault();
                        queryDocsSing.FechaModifica = DateTime.Now;
                        queryDocsSing.UsuarioModifica = UserId;
                        dbc.Entry(queryDocsSing).State = EntityState.Modified;
                        dbc.SaveChanges();
                    }
                    else
                    {
                        DocumentosObligatorio objDocumentosObligatorios = new DocumentosObligatorio();
                        objDocumentosObligatorios.IdDocuSale = OP;
                        objDocumentosObligatorios.IdTypeDocuAtta = Convert.ToInt32(id);
                        objDocumentosObligatorios.FechaCrea = DateTime.Now;
                        objDocumentosObligatorios.UsuarioCrea = UserId;
                        objDocumentosObligatorios.Estado = true;
                        dbc.DocumentosObligatorios.Add(objDocumentosObligatorios);
                        dbc.SaveChanges();
                    }
                    #endregion

                }
            }

            #endregion

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeAsignarResponsable) top.MensajeAsignarResponsable('OK','1');}window.onload=init;</script>");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AgregarActividad(Cronograma objCronograma, FormCollection collection)
        {
            int flag = 0;
            //try
            //{
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
            int usuario = Convert.ToInt32(Session["UserId"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var query = dbc.Cronogramas.Where(x => x.IdDocuSale == IdDocuSale && x.Estado == true);
            var dtInicio = dbc.Cronogramas.Where(x => x.IdDocuSale == IdDocuSale && x.Estado == true && x.InicioCronograma == 1).SingleOrDefault();

            if (query.Count() == 0)
            {
                if (Convert.ToString(Request.Params["dtFechaInicio"]).ToString() == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                      "if(top.mostrarMensaje) top.mostrarMensaje('ERROR','Debe ingresar la Fecha de Inicio.');}window.onload=init;</script>");
                }
                else
                {
                    DateTime FechaInicio = Convert.ToDateTime(Request.Params["dtFechaInicio"]);
                    dbc.RegistrarActividadInicio(IdDocuSale, usuario, IdPersEnti, FechaInicio);
                    return Content("<script type='text/javascript'> function init() {" +
                                      "if(top.mostrarMensaje) top.mostrarMensaje('OK','2');}window.onload=init;</script>");
                }
            }
            else
            {
                var q = dbc.Cronogramas.Where(x => x.IdDocuSale == IdDocuSale && x.Estado == true).OrderByDescending(x => x.Peso).FirstOrDefault();

                int Peso = 0;
                if (Convert.ToString(Request.Params["txtPeso"]) != "")
                {
                    Peso = Convert.ToInt32(Request.Params["txtPeso"]);
                }
                //Validando el peso 100
                if (Convert.ToInt32(q.Peso) == 100)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.mostrarMensaje) top.mostrarMensaje('ERROR','El Cronograma ya está registrado al 100%.');}window.onload=init;</script>");
                }

                if (objCronograma.NombreActividad == null || objCronograma.IdAccoPara == null ||
                        objCronograma.FechaFin == null || Peso == 0 || Peso > 100)
                {
                    flag = 1;
                }
                if (flag == 1)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.mostrarMensaje) top.mostrarMensaje('ERROR','Verifique los datos porfavor.');}window.onload=init;</script>");
                }
                //Validando la fecha
                if (objCronograma.FechaFin < dtInicio.FechaInicio)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.mostrarMensaje) top.mostrarMensaje('ERROR','La Fecha debe ser mayor a la Fecha de Inicio (" + String.Format("{0:MM/dd/yyyy}", dtInicio.FechaInicio) + ")');}window.onload=init;</script>");
                }

                //Validando el peso
                if (Peso <= Convert.ToInt32(q.Peso))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.mostrarMensaje) top.mostrarMensaje('ERROR','El Peso debe ser mayor a " + Convert.ToInt32(q.Peso) + "');}window.onload=init;</script>");
                }

                int NumeroCronograma = 1;

                objCronograma.NombreActividad = objCronograma.NombreActividad;
                objCronograma.FechaInicio = dtInicio.FechaInicio;
                objCronograma.FechaCrea = DateTime.Now;
                objCronograma.UsuarioCrea = usuario;
                objCronograma.Estado = true;
                objCronograma.NroCronograma = NumeroCronograma;
                objCronograma.InicioCronograma = 0;
                objCronograma.Estilo = "gtaskblue";
                objCronograma.Mile = 0;
                objCronograma.IdPersEnti = IdPersEnti;
                objCronograma.Completo = 0;
                objCronograma.Grupo = 0;
                objCronograma.Padre = 1;
                objCronograma.Abierto = 1;
                objCronograma.Depende = "";
                objCronograma.Subtitulo = "";
                objCronograma.Notas = "";
                objCronograma.Gantt = "g";
                objCronograma.Peso = Peso;
                DateTime FechaInicio = Convert.ToDateTime(dtInicio.FechaInicio), FechaFin = Convert.ToDateTime(objCronograma.FechaFin);
                objCronograma.Recurso = Convert.ToInt32(dbc.ObtenerDiferenciasFechas(FechaInicio, FechaFin).SingleOrDefault());
                dbc.Cronogramas.Add(objCronograma);
                dbc.SaveChanges();
                //}
                //catch
                //{
                //    flag = 1;
                //}

            }
            if (flag == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.mostrarMensaje) top.mostrarMensaje('ERROR','Verifique los datos porfavor.');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.mostrarMensaje) top.mostrarMensaje('OK','1');}window.onload=init;</script>");
            }
        }

        public ActionResult InfoAdicionalProyecto(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult VerInfoAdicional(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult ListarInfoAdicional(int id = 0)
        {
            var query = dbc.ObtenerInfoAdicional(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerViatico(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult ListarViatico(int id = 0)
        {
            var query = dbc.OPObtenerViaticos(id);

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerHoraTrabajada(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult ListarActividades(int id = 0)
        {
            var query = dbc.OPObtenerHorasTrabajadas(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditarOP(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            DOCUMENT_SALE DS = new DOCUMENT_SALE();
            DS = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).FirstOrDefault();


            //ViewBag.PrecioVenta = Convert.ToInt32(DS.PrecioVenta);
            ViewBag.FechaCompromiso = DS.FechaCompromiso;
            ViewBag.EstadoTipoProyecto = DS.EstadoTipoProyecto;

            string FechaEntregaCliente = "";
            if (Convert.ToString(dbc.ObtenerFechaEntregaCliente(id).SingleOrDefault()) != "")
            {
                FechaEntregaCliente = Convert.ToString(dbc.ObtenerFechaEntregaCliente(id).SingleOrDefault());
            }
            int Aplica = 0;
            Aplica = Convert.ToInt32(DS.AplicaCronograma);
            if (Aplica == 0)
            {
                ViewBag.Habilitar = 1;
                ViewBag.FechaEntregaCliente = DS.FechaLimitePM;
                ViewBag.PorcentajeAvance = DS.AvanceReal;
            }
            else
            {
                ViewBag.Habilitar = 0;
                ViewBag.FechaEntregaCliente = FechaEntregaCliente;
                ViewBag.PorcentajeAvance = Convert.ToInt32(dbc.ObtenerPorcentajeAvance(id).SingleOrDefault());
            }

            return View(DS);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarOPProyecto(DOCUMENT_SALE DS)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            DateTime dtCompromiso = Convert.ToDateTime(null);
            DateTime dtFechaLimitePM = Convert.ToDateTime(null);

            DOCUMENT_SALE OP = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == DS.ID_DOCU_SALE).FirstOrDefault();

            //if (Convert.ToString(dbc.ObtenerFechaEntregaCliente(DS.ID_DOCU_SALE).SingleOrDefault()) != "")
            //{
            //    OP.FechaLimitePM = Convert.ToDateTime(dbc.ObtenerFechaEntregaCliente(DS.ID_DOCU_SALE).SingleOrDefault());
            //}
            //OP.AvanceReal = Convert.ToInt32(dbc.ObtenerPorcentajeAvance(DS.ID_DOCU_SALE).SingleOrDefault());

            OP.Impacto = DS.Impacto;
            OP.HorasProgramadas = DS.HorasProgramadas;
            OP.PrecioVenta = DS.PrecioVenta;
            OP.PCCotizado = DS.PCCotizado;
            OP.PCReal = DS.PCReal;
            OP.PCCotizadoCompra = DS.PCCotizadoCompra;
            OP.PCRealCompra = DS.PCRealCompra;
            OP.PCCotizadoServicio = DS.PCCotizadoServicio;
            OP.PCRealServicio = DS.PCRealServicio;
            OP.Facturacion = DS.Facturacion;
            OP.FechaFacturacion = DS.FechaFacturacion;
            OP.EstadoTipoProyecto = Convert.ToInt32(Request.Params["rdTipoProyecto"]);
            OP.UserIdModifica = UserId;
            OP.FECHAMODIFICA = DateTime.Now;

            int Aplica = 0;
            Aplica = Convert.ToInt32(OP.AplicaCronograma);
            if (Aplica == 0)
            {
                if (Convert.ToString(Request.Params["dtFechaCompromiso"]) != "" && Convert.ToString(Request.Params["dtFechaCompromiso"]) != null)
                {
                    dtCompromiso = Convert.ToDateTime(Request.Params["dtFechaCompromiso"]);
                    OP.FechaCompromiso = dtCompromiso;
                }
                if (Convert.ToString(Request.Params["dtFechaLimitePM"]) != "" && Convert.ToString(Request.Params["dtFechaLimitePM"]) != null)
                {
                    dtFechaLimitePM = Convert.ToDateTime(Request.Params["dtFechaLimitePM"]);
                    OP.FechaLimitePM = dtFechaLimitePM;
                }
                else
                {
                    OP.FechaLimitePM = null;
                }
                if (Convert.ToString(Request.Params["txtAvanceReal"]) == "")
                {
                    OP.AvanceReal = 0;
                }
                else
                {
                    OP.AvanceReal = Convert.ToInt32(Request.Params["txtAvanceReal"]);
                }
            }
            else
            {
                if (Convert.ToString(Request.Params["dtFechaCompromiso"]) != "" && Convert.ToString(Request.Params["dtFechaCompromiso"]) != null)
                {
                    dtCompromiso = Convert.ToDateTime(Request.Params["dtFechaCompromiso"]);
                    OP.FechaCompromiso = dtCompromiso;
                }
                if (Convert.ToString(dbc.ObtenerFechaEntregaCliente(OP.ID_DOCU_SALE).SingleOrDefault()) != "")
                {
                    OP.FechaLimitePM = Convert.ToDateTime(dbc.ObtenerFechaEntregaCliente(OP.ID_DOCU_SALE).SingleOrDefault());
                }
                OP.AvanceReal = Convert.ToInt32(dbc.ObtenerPorcentajeAvance(OP.ID_DOCU_SALE).SingleOrDefault());
            }

            dbc.Entry(OP).State = EntityState.Modified;
            dbc.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeEditarOP) top.MensajeEditarOP('OK','');}window.onload=init;</script>");

        }

        public ActionResult AsignarIngeniero(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult AsignarEspecialista()
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            int OP = Convert.ToInt32(Request.Params["Id"]);
            int IdAreaResponsable = Convert.ToInt32(Request.Params["AreaResp"]);
            int IdIng = Convert.ToInt32(Request.Params["Asignado"]);
            string NoAplica = Convert.ToString(Request.Params["Aplica"]); //false
            string Observacion = Convert.ToString(Request.Params["ObsAsignarInge"]);

            var ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == OP);
            ds.AplicaIng = (NoAplica == "false" ? true : false);
            dbc.Entry(ds).State = EntityState.Modified;
            dbc.SaveChanges();

            int ID_PERS_ENTI = 0;

            if (NoAplica == "false")
            {
                if (ds.ID_COMP_END != null)
                {
                    var PersonEntity = dbe.PERSON_ENTITY.Single(x => x.ID_ENTI2 == ds.ID_CTTS);
                    ID_PERS_ENTI = PersonEntity.ID_PERS_ENTI;
                }

                //Genera el ticket
                var ticket = new TICKET();
                int ID_CHAR = 0;
                try
                {
                    ID_CHAR = dbe.PERSON_CONTRACT.Single(x => x.LAS_CONT == true && x.ID_PERS_CONT == IdIng).ID_CHAR.Value;
                }
                catch { ID_CHAR = 0; }

                ticket.ID_QUEU = IdAreaResponsable;
                ticket.ID_DOCU_SALE = OP;
                ticket.ID_ACCO = 4;
                ticket.ID_TYPE_TICK = 3;
                ticket.ID_PERS_ENTI = ID_PERS_ENTI;
                ticket.ID_PERS_ENTI_END = ID_PERS_ENTI;
                ticket.ID_PERS_ENTI_ASSI = IdIng;
                ticket.ID_COMP = ds.ID_COMP;
                ticket.ID_AREA = ID_CHAR;
                ticket.ID_PRIO = 5;
                ticket.IdSLA = 1;
                ticket.ID_STAT = 1;
                ticket.ID_STAT_END = 5;
                ticket.ID_SOUR = 4;
                ticket.FEC_TICK = DateTime.Now;
                ticket.SUM_TICK = Observacion;
                ticket.REM_CTRL_TICK = false;
                ticket.UserId = Convert.ToInt32(Session["UserId"]);
                ticket.CREATE_TICK = DateTime.Now;
                ticket.FEC_INI_TICK = ds.EXP_DATE;
                ticket.MODIFIED_TICK = DateTime.Now;
                ticket.IS_PARENT = true;
                ticket.ID_CATE = 5866; //Revisar Proyecto
                ticket.SEND_MAIL = false;

                dbc.TICKETs.Add(ticket);
                dbc.SaveChanges();

                //int id = Convert.ToInt32(ticket.ID_TICK);
                //dbc.Entry(ticket).State = EntityState.Detached;
            }

            //Enviar Correo

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','2');}window.onload=init;</script>");
        }

        public ActionResult VerProductos(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult ListarProductos(int id = 0)
        {
            try
            {
                var result = dbc.ListarProductosxOP(id).ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ListarProductoSerieOPSIDIGE()
        {
            try
            {

                string descripcion = Request.Params["txtDescripcion"];
                int idOP = 0, idComp = 0, idCompEnd = 0, idMarca = 0;
                if (Request.Params["idOP"] != null && Request.Params["idOP"] != "")
                    idOP = Convert.ToInt32(Request.Params["idOP"]);
                if (Request.Params["idComp"] != null && Request.Params["idComp"] != "")
                    idComp = Convert.ToInt32(Request.Params["idComp"]);
                if (Request.Params["idCompEnd"] != null && Request.Params["idCompEnd"] != "")
                    idCompEnd = Convert.ToInt32(Request.Params["idCompEnd"]);
                if (Request.Params["idMarca"] != null && Request.Params["idMarca"] != "")
                    idMarca = Convert.ToInt32(Request.Params["idMarca"]);
                //var result = dbc.ListarProductoSerieOPSIDIGE(descripcion, idOP, idComp, idCompEnd, idMarca).ToList();
                var result = dbc.ListarProductoSerieOPSIDIGE(descripcion, idOP, idComp, idCompEnd).ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult EditarCamposManualesFormato()
        {
            try
            {
                bool boolafectacion;
                string Observacion = Convert.ToString(Request.Params["txtObservaciones"]);
                string AnalisisCausa = Convert.ToString(Request.Params["txtAnalisisCausa"]);
                String AfectacionServicio = Convert.ToString(Request.Params["AfectacionServicio"]);
                int IdTicket = Convert.ToInt32(Request.Params["IdTicket"]);
                if (AfectacionServicio =="Si")
                {
                    boolafectacion = true;
                }
                else
                {
                    boolafectacion = false;
                }
                var result = dbc.EditarCamposManualesFormato(IdTicket,boolafectacion,Observacion,AnalisisCausa);

                return Json(new { Data = "Actualizado" }, JsonRequestBehavior.AllowGet);
            }
            
            catch (Exception e)
           
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ListarOPFormato()
        {
            int id_comp = 0;
            string txt = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                id_comp = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "NUM_DOCU_SALE")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                id_comp = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            var result = dbc.ListarOPFormato(txt, id_comp).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormatoAutomatico()
        {

            try
            {
                string observaciones = Request.Params["txtDescripcion"];
                string analisisCausa = Request.Params["txtDescripcion"];

                DateTime fechaCreacion = DateTime.ParseExact(Request.Params["fechaInicio"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime fechaCreacion = Convert.ToDateTime(Request.Params["fechaInicio"]);

                DateTime fechaFin = DateTime.ParseExact(Request.Params["fechaFin"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime fechaFin = Convert.ToDateTime(Request.Params["fechaFin"]);
                //bool afectacionServicio = Convert.ToBoolean(Request.Params["VIS_ALL_QUEU"].ToString());
                int idOP = 0, idComp = 0, idCompEnd = 0, idMarca = 0;
                if (Request.Params["idOP"] != null && Request.Params["idOP"] != "")
                    idOP = Convert.ToInt32(Request.Params["idOP"]);
                if (Request.Params["idComp"] != null && Request.Params["idComp"] != "")
                    idComp = Convert.ToInt32(Request.Params["idComp"]);
                if (Request.Params["idCompEnd"] != null && Request.Params["idCompEnd"] != "")
                    idCompEnd = Convert.ToInt32(Request.Params["idCompEnd"]);
                //if (Request.Params["idMarca"] != null && Request.Params["idMarca"] != "")
                //    idMarca = Convert.ToInt32(Request.Params["idMarca"]);
                var result = dbc.FormatoAutomatico(idComp, idCompEnd, idOP, fechaCreacion, fechaFin, 0, 0, 0, "").ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult AgregarObservaciones(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }
        public ActionResult EditarObservaciones(int id = 0)
        {
            var query = dbc.ObtenerOPComentario(id).Single();
            ViewBag.IdComentario = id;
            ViewBag.IdTipoComentario = query.IdTipoComentario;
            ViewBag.TipoComentario = query.TipoComentario;
            ViewBag.Comentario = query.Comentario;
            ViewBag.IdParametro = query.IdParametro;
            ViewBag.Parametro = query.Parametro;
            ViewBag.IdDetalleParametro = query.IdDetalleParametro;
            ViewBag.ParametroDetalle = query.ParametroDetalle;
            ViewBag.DiasRetraso = query.DiasRetraso;
            ViewBag.AnioCierre = query.AnioCierre;
            ViewBag.MesCierre = query.MesCierre;
            ViewBag.DiaCierre = query.DiaCierre;

            return View();
        }
        public ActionResult EliminarObservaciones(int id = 0)
        {
            ViewBag.IdComentario = id;
            return View();
        }

        public ActionResult EditarRiesgo(int id = 0)
        {

            var DS = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).FirstOrDefault();
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.IdRiesgo = DS.Impacto;
            return View();
        }
        public ActionResult ProductosServicios(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AgregarObservacion(int id = 0)
        {
            try
            {
                string mensaje = "";

                int UserId = Convert.ToInt32(Session["UserId"]);
                int OP = Convert.ToInt32(Request.Params["Id"]);
                int Tipo = Convert.ToInt32(Request.Params["TipoComent"]);
                string Observacion = Convert.ToString(Request.Params["Comentario"]);

                string idTipo = Convert.ToString(Request.Params["idTipo"]);
                string idDetalle = Convert.ToString(Request.Params["idDetalle"]);
                string sfechaCierre;
                int diasRetraso;
                DateTime fechaCierre;


                DOCUMENT_SALE ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == OP);
                DOCUMENT_SALE_ACTIVITY dsa = new DOCUMENT_SALE_ACTIVITY();

                dsa.ID_DOCU_SALE = OP;
                dsa.ID_STAT_DOCU_SALE = ds.ID_STAT_DOCU_SALE;
                dsa.ID_TYPE_ACTI = Tipo;
                dsa.COM_ACTI_DOCU_SALE = Observacion;
                dsa.UserId = UserId;
                dsa.CREATE_ACTI_DOCU_SALE = DateTime.Now;
                dsa.Estado = true;

                //Validación
                if (Tipo == 7)//Acciones
                {
                    if (idTipo == "" || idTipo is null)
                    {
                        mensaje = mensaje + "Seleccionar Tipo <br>";
                    }
                    else
                    {
                        dsa.IdParametro = Convert.ToInt32(idTipo);

                        if (idTipo == "1" || idTipo == "2")
                        {
                            diasRetraso = Convert.ToInt32(Request.Params["txtDiasRetraso"]);
                            if (diasRetraso == 0)
                            {
                                mensaje = mensaje + "Ingresar Dias de Retraso <br>";
                            }
                            dsa.DiasRetraso = diasRetraso;
                        }
                        if (idTipo == "3")
                        {
                            sfechaCierre = Convert.ToString(Request.Params["dtFechaCierre"]);
                            if (sfechaCierre == "")
                            {
                                mensaje = mensaje + "Seleccionar Fecha de Cierre <br>";
                            }
                            fechaCierre = Convert.ToDateTime(sfechaCierre);
                            dsa.FechaCierre = fechaCierre;
                        }
                    }
                    if (idDetalle == "" || idDetalle is null)
                    {
                        mensaje = mensaje + "Seleccionar Detalle";
                    }
                    else
                    {
                        dsa.IdParametroDetalle = Convert.ToInt32(idDetalle);
                    }
                }

                if (mensaje != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeObservacion) top.MensajeObservacion('ERROR','" + mensaje + "');}window.onload=init;</script>");
                }

                dbc.DOCUMENT_SALE_ACTIVITY.Add(dsa);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeObservacion) top.MensajeObservacion('OK','');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeObservacion) top.MensajeObservacion('ERROR','');}window.onload=init;</script>");
            }

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditarComentario()
        {
            try
            {
                string mensaje = "";

                int UserId = Convert.ToInt32(Session["UserId"]);
                int IdComentario = Convert.ToInt32(Request.Params["IdComentario"]);
                int Tipo = Convert.ToInt32(Request.Params["TipoComent"]);
                string Observacion = Convert.ToString(Request.Params["Comentario"]);

                string idTipo = Convert.ToString(Request.Params["idTipo"]);
                string idDetalle = Convert.ToString(Request.Params["idDetalle"]);
                string sfechaCierre;
                int diasRetraso;
                DateTime fechaCierre;

                DOCUMENT_SALE_ACTIVITY dsa = dbc.DOCUMENT_SALE_ACTIVITY.Single(x => x.ID_ACTI_DOCU_SALE == IdComentario);

                dsa.ID_TYPE_ACTI = Tipo;
                dsa.COM_ACTI_DOCU_SALE = Observacion;
                dsa.UserId = UserId;
                dsa.CREATE_ACTI_DOCU_SALE = DateTime.Now;
                dsa.Estado = true;

                //Validación
                if (Tipo == 7)//Acciones
                {
                    if (idTipo == "" || idTipo is null)
                    {
                        mensaje = mensaje + "Seleccionar Tipo <br>";
                    }
                    else
                    {
                        dsa.IdParametro = Convert.ToInt32(idTipo);

                        if (idTipo == "1" || idTipo == "2")
                        {
                            diasRetraso = Convert.ToInt32(Request.Params["txtDiasRetraso"]);
                            if (diasRetraso == 0)
                            {
                                mensaje = mensaje + "Ingresar Dias de Retraso <br>";
                            }
                            dsa.DiasRetraso = diasRetraso;
                        }
                        if (idTipo == "3")
                        {
                            sfechaCierre = Convert.ToString(Request.Params["dtFechaCierre"]);
                            if (sfechaCierre == "")
                            {
                                mensaje = mensaje + "Seleccionar Fecha de Cierre <br>";
                            }
                            fechaCierre = Convert.ToDateTime(sfechaCierre);
                            dsa.FechaCierre = fechaCierre;
                        }
                    }
                    if (idDetalle == "" || idDetalle is null)
                    {
                        mensaje = mensaje + "Seleccionar Detalle";
                    }
                    else
                    {
                        dsa.IdParametroDetalle = Convert.ToInt32(idDetalle);
                    }
                }

                if (mensaje != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeObservacion) top.MensajeObservacion('ERROR','" + mensaje + "');}window.onload=init;</script>");
                }

                dsa.UserIdModifica = UserId;
                dsa.FechaModifica = DateTime.Now;
                dbc.Entry(dsa).State = EntityState.Modified;
                dbc.SaveChanges();


                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeObservacion) top.MensajeObservacion('OK','');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeObservacion) top.MensajeObservacion('ERROR','');}window.onload=init;</script>");
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EliminarComentario()
        {
            try
            {
                int IdComentario = Convert.ToInt32(Request.Params["IdComentario"]);

                DOCUMENT_SALE_ACTIVITY dsa = dbc.DOCUMENT_SALE_ACTIVITY.Where(x => x.ID_ACTI_DOCU_SALE == IdComentario).SingleOrDefault();
                dsa.Estado = false;
                dbc.Entry(dsa).State = EntityState.Modified;
                dbc.SaveChanges();


                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeObservacion) top.MensajeObservacion('OK','');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeObservacion) top.MensajeObservacion('ERROR','');}window.onload=init;</script>");
            }
        }

        [Authorize]
        public ActionResult AdjuntarArchivos(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        [Authorize]
        public ActionResult AdjuntarDocumentos(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            ViewBag.UploadFile = "M1DS" + Convert.ToString(DateTime.Now.Ticks);
            ViewBag.DATE = String.Format("{0:g}", DateTime.Now);
            return View();
        }

        [Authorize]
        public ActionResult VerFacturacion(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        [Authorize]
        public ActionResult GenerarActa(int id = 0)
        {
            string alcance = "", requisito = "";
            var acta = dbc.ActaConstitucion.Where(x => x.IdDocuSale == id).ToList();

            ViewBag.IdDocuSale = id;
            if (acta.Count() > 0)
            {
                alcance = acta.Single().Alcance;
                requisito = acta.Single().Requisito;
            }
            ViewBag.Alcance = alcance;
            ViewBag.Requisito = requisito;
            ViewBag.FlagActa = acta.Count();
            return View();
        }

        public ActionResult ObtenerArchivosAdjuntos(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        [Authorize]
        public ActionResult ListaProyectos(int id = 0)
        {
            try
            {
                if ((int)Session["UserId"] != 0)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public ActionResult ObtenerArchivosComercial(int id = 0)
        {

            
            var query2 = dbc.OPObtenerArchivosProyecto(id, 0);

            return Json(new { Data2 = query2 }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerArchivosProyecto(int id = 0)
        {
            var query = dbc.OPObtenerArchivosProyecto(id, 1);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerArchivosCronograma(int id = 0)
        {
            var query = dbc.ListarCronogramaArchivos(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CerrarOP(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            var query = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).SingleOrDefault();
            double MontoxFacturar = 0.0;
            if (query != null)
            {
                if (query.PrecioVenta == null || query.Facturacion == null)
                {
                    MontoxFacturar = 0;
                }
                else
                {
                    int Precio = 0;
                    double Porcentaje = 0.0;
                    Precio = Convert.ToInt32(query.PrecioVenta);
                    Porcentaje = 100 - Convert.ToDouble(query.Facturacion);
                    MontoxFacturar = (Porcentaje * Precio) / 100;
                }
            }
            ViewBag.MontoxFacturar = MontoxFacturar;
            return View();
        }

        public ActionResult ObtenerCerrarOP(int id = 0)
        {
            var query = dbc.OPCierre(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerTicketsProyecto(int id = 0)
        {
            var query = dbc.ObtenerTicketsProyecto(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CantidadOP(int id = 0)
        {
            String mensaje = "";
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var queryValidaCantOP = dbc.ValidarCantidadProyecto_PersAsignado(id, IdAcco).SingleOrDefault();

            mensaje = queryValidaCantOP.Mensaje;

            return Content(mensaje);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CerrandoOP()
        {

            int UserId = Convert.ToInt32(Session["UserId"]);
            int OP = Convert.ToInt32(Request.Params["Id"]);

            ///////////////////////////////
            decimal saldo = 0;
            int mantPrev = 0;

            int check = Convert.ToInt32(Request.Form["hdnChkSaldo"]);
            try
            {
                saldo = Convert.ToDecimal(Request.Form["txtSaldo"]);
            }
            catch
            {
                saldo = 0;
            }

            var proyecto = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == OP).SingleOrDefault();

            //Validando Soporte Electrodata
            //var query = dbc.SoporteEDs.Where(x => x.IdDocuSale == OP && x.Estado == true);
            //string estadoED = Convert.ToString(proyecto.EstadoSoporteED);

            //if ((estadoED == "True" && query.Count() == 0) || estadoED == "")
            //{
            //    //Devuelve mensaje de error, falta ingresar el SOPORTE ELECTRODATA 
            //    return Content("<script type='text/javascript'> function init() {" +
            //    "if(top.MensajeCerrarOP) top.MensajeCerrarOP('Soporte','SoporteED');}window.onload=init;</script>");
            //}
            //else if (estadoED == "False") { }
            //else
            //{
            //    //if (check == 1)
            //    //{
            //    //    var insert = dbc.RegistrarFacturacion(query.SingleOrDefault().IdSoporteED, saldo, UserId);
            //    //}
            //}

            //Validando Informe
            var queryInforme = dbc.InformeED.Where(x => x.IdDocuSale == OP && x.Estado == true);
            string estadoInforme = Convert.ToString(proyecto.EstadoInforme);

            if ((estadoInforme == "True" && queryInforme.Count() == 0) || estadoInforme == "")
            {
                //Devuelve mensaje de error, falta ingresar el SOPORTE ELECTRODATA 
                return Content("<script type='text/javascript'> function init() {" +
            "if(top.MensajeCerrarOP) top.MensajeCerrarOP('Soporte','Informe');}window.onload=init;</script>");
            }

            //Validando Soporte Fabricante
            var querySoporteFabricante = dbc.SoporteFabricantes.Where(x => x.IdDocuSale == OP && x.Estado == true);
            string estadoFabricante = Convert.ToString(proyecto.EstadoSoporteFabricante);

            if ((estadoFabricante == "True" && querySoporteFabricante.Count() == 0) || estadoFabricante == "")
            {
                //Devuelve mensaje de error, falta ingresar el SOPORTE ELECTRODATA 
                return Content("<script type='text/javascript'> function init() {" +
            "if(top.MensajeCerrarOP) top.MensajeCerrarOP('Soporte','SoporteFabricante');}window.onload=init;</script>");
            }


            //Validando todos los Archivos
            var msjArchivos = dbc.ValidarArchivos(OP).SingleOrDefault();
            string mensaje = Convert.ToString(msjArchivos.Mensaje);
            if (mensaje != "OK")
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeCerrarOP) top.MensajeCerrarOP('ERROR','" + mensaje + "');}window.onload=init;</script>");
            }
            //Validando todas las actividades en 100%
            string msjActividades = Convert.ToString(dbc.ValidarActividades(OP).SingleOrDefault());
            if (msjActividades != "OK")
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeCerrarOP) top.MensajeCerrarOP('ERROR','" + msjActividades + "');}window.onload=init;</script>");
            }

            string Observacion = Convert.ToString(Request.Params["ComentarioOP"]);

            var ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == OP);
            ds.ID_STAT_DOCU_SALE = 4;
            ds.SaldoPorCobrar = saldo;
            dbc.Entry(ds).State = EntityState.Modified;
            dbc.SaveChanges();

            DOCUMENT_SALE_ACTIVITY dsa = new DOCUMENT_SALE_ACTIVITY();
            dsa.ID_DOCU_SALE = OP;
            dsa.ID_STAT_DOCU_SALE = 4;
            dsa.ID_TYPE_ACTI = 1; //Log Comment
            dsa.COM_ACTI_DOCU_SALE = Observacion;
            dsa.UserId = UserId;
            dsa.CREATE_ACTI_DOCU_SALE = DateTime.Now;
            dsa.Estado = true;
            dbc.DOCUMENT_SALE_ACTIVITY.Add(dsa);
            dbc.SaveChanges();

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeCerrarOP) top.MensajeCerrarOP('OK','OK');}window.onload=init;</script>");

        }

        public ActionResult VerTraza(int id = 0)
        {
            ViewBag.ID_DOCU_SALE = id;
            return View();
        }

        public ActionResult ListarTransacciones(int id = 0)
        {
            var query = dbc.LogObtenerDatos("DOCUMENT_SALE", id.ToString());

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Buscar OP"
        [Authorize]
        public ActionResult BuscarOP()
        {
            return View();
        }

        public ActionResult FindSaleResult()
        {
            string S_NUM_DOCU_SALE = Convert.ToString(Request.Params["NUM_DOCU_SALE"].ToString()).Trim();
            string S_OS = Convert.ToString(Request.Params["OS"].ToString()).Trim();
            string S_KEYWORD = Convert.ToString(Request.Params["KEYWORD"].ToString()).Trim();

            string S_ID_STAT_DOCU_SALE = null, S_ID_ENTI = null, S_ID_PERS_ENTI = null, S_ID_PERS_ENTI_VEND = null,
                    S_ID_QUEU = null, S_ID_PERS_ENTI_ASSI = null, S_ID_CATE = null, S_ID_TYPE_DOCU_SALE = null, S_IdPM = null, S_ID_COMP_END = null;

            int ID_STAT_DOCU_SALE, ID_TYPE_DOCU_SALE, ID_ENTI, ID_PERS_ENTI, ID_PERS_ENTI_VEND, ID_QUEU, ID_PERS_ENTI_ASSI, ID_CATE, IdPM, tt = 0, ID_COMP_END;

            try { ID_STAT_DOCU_SALE = Convert.ToInt32(Request.Params["ID_STAT_DOCU_SALE"].ToString()); }
            catch { ID_STAT_DOCU_SALE = 0; }

            try { ID_TYPE_DOCU_SALE = Convert.ToInt32(Request.Params["ID_TYPE_DOCU_SALE"].ToString()); }
            catch { ID_TYPE_DOCU_SALE = 0; }

            try { ID_ENTI = Convert.ToInt32(Request.Params["ID_ENTI"].ToString()); }
            catch { ID_ENTI = 0; }

            try { ID_COMP_END = Convert.ToInt32(Request.Params["ID_COMP_END"].ToString()); }
            catch { ID_COMP_END = 0; }

            try { ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"].ToString()); }
            catch { ID_PERS_ENTI = 0; }

            try { ID_PERS_ENTI_VEND = Convert.ToInt32(Request.Params["ID_PERS_ENTI_VEND"].ToString()); }
            catch { ID_PERS_ENTI_VEND = 0; }

            try { ID_QUEU = Convert.ToInt32(Request.Params["ID_QUEU_FIND"].ToString()); }
            catch { ID_QUEU = 0; }

            try { ID_PERS_ENTI_ASSI = Convert.ToInt32(Request.Params["ID_PERS_ENTI_ASSI_FIND"].ToString()); }
            catch { ID_PERS_ENTI_ASSI = 0; }

            try { ID_CATE = Convert.ToInt32(Request.Params["ID_CATE_FIND"].ToString()); }
            catch { ID_CATE = 0; }

            try { IdPM = Convert.ToInt32(Request.Params["PM"].ToString()); }
            catch { IdPM = 0; }

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //var qClient = (from a in dbe.PERSON_ENTITY
            //               join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
            //               join c in dbe.CLASS_ENTITY on a.ID_ENTI1 equals c.ID_ENTI
            //               select new
            //               {
            //                   a.ID_PERS_ENTI,
            //                   a.ID_ENTI1,
            //                   a.ID_ENTI2,
            //                   CONTACT = b.FIR_NAME + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME),
            //                   a.EMA_PERS,
            //                   a.TEL_PERS,
            //                   a.EXT_PERS,
            //                   a.CAR_PERS,
            //                   c.COM_NAME,
            //                   RUC = (c.NUM_TYPE_DI == null ? "" : c.NUM_TYPE_DI),
            //                   c.ADDRESS,
            //                   c.TEL_ENTI,
            //               });


            //if (Int32.TryParse(S_ID_PERS_ENTI, out ID_PERS_ENTI))
            //{
            //    qClient = qClient.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI);
            //}
            //else if (Int32.TryParse(S_ID_ENTI, out ID_ENTI))
            //{
            //    qClient = qClient.Where(x => x.ID_ENTI1 == ID_ENTI);
            //}

            //var qDocuSale = (from a in dbc.DOCUMENT_SALE
            //                 join b in dbc.TICKETs on a.ID_DOCU_SALE equals b.ID_DOCU_SALE into lb
            //                 from xb in lb.DefaultIfEmpty()
            //                 select new
            //                 {
            //                     a.ID_DOCU_SALE,
            //                     a.ID_TYPE_DOCU_SALE,
            //                     a.ID_STAT_DOCU_SALE,
            //                     a.NUM_DOCU_SALE,
            //                     a.DAT_DOCU_SALE,
            //                     a.SUM_DOCU_SALE,
            //                     a.ID_PERS_ENTI_VEND,
            //                     a.UserId,
            //                     a.DAT_REGISTER,
            //                     a.ID_COMP,
            //                     a.ID_CTTS,
            //                     a.OS,
            //                     a.ID_ENTI,
            //                     a.EXP_DATE,
            //                     a.ID_PERS_ENTI_PM,
            //                     xb.ID_PERS_ENTI_END,
            //                     xb.ID_PERS_ENTI_ASSI,
            //                     xb.ID_QUEU,
            //                     xb.ID_CATE,
            //                     a.ID_COMP_END
            //                 });

            //if (!String.IsNullOrEmpty(S_NUM_DOCU_SALE))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.NUM_DOCU_SALE.ToUpper().Contains(S_NUM_DOCU_SALE.ToUpper()));
            //}
            //if (!String.IsNullOrEmpty(S_OS))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.OS.ToUpper().Contains(S_OS.ToUpper()));
            //}
            //if (Int32.TryParse(S_ID_TYPE_DOCU_SALE, out ID_TYPE_DOCU_SALE))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_TYPE_DOCU_SALE == ID_TYPE_DOCU_SALE);
            //}
            //if (Int32.TryParse(S_ID_STAT_DOCU_SALE, out ID_STAT_DOCU_SALE))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_STAT_DOCU_SALE == ID_STAT_DOCU_SALE);
            //}
            //if (Int32.TryParse(S_ID_PERS_ENTI, out ID_PERS_ENTI))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_CTTS == ID_PERS_ENTI);
            //}
            //if (Int32.TryParse(S_ID_PERS_ENTI_VEND, out ID_PERS_ENTI_VEND))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_PERS_ENTI_VEND == ID_PERS_ENTI_VEND);
            //}
            //if (Int32.TryParse(S_ID_QUEU, out ID_QUEU))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_QUEU == ID_QUEU);
            //}
            //if (Int32.TryParse(S_ID_PERS_ENTI_ASSI, out ID_PERS_ENTI_ASSI))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
            //}
            //if (Int32.TryParse(S_ID_CATE, out ID_CATE))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_CATE == ID_CATE);
            //}

            //if (Int32.TryParse(S_ID_COMP_END, out ID_COMP_END))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_COMP_END == ID_COMP_END);
            //}

            //if (!String.IsNullOrEmpty(S_KEYWORD))
            //{
            //    var qDetail = (from a in dbc.DETAIL_DOCUMENT_SALE.Where(x => x.DESC_DETA.ToUpper().Contains(S_KEYWORD.ToUpper()))
            //                   select new
            //                   {
            //                       a.ID_DOCU_SALE
            //                   }).Distinct();

            //    qDocuSale = (from a in qDocuSale
            //                 join b in qDetail on a.ID_DOCU_SALE equals b.ID_DOCU_SALE
            //                 select a);
            //}
            //if (Int32.TryParse(S_IdPM, out IdPM))
            //{
            //    qDocuSale = qDocuSale.Where(x => x.ID_PERS_ENTI_PM == IdPM);
            //}

            //var qUnion = (from a in qDocuSale.ToList()
            //              join b in qClient on a.ID_CTTS equals b.ID_ENTI2 into lb
            //              from xb in lb.DefaultIfEmpty()
            //              join c in dbe.PERSON_ENTITY on a.ID_PERS_ENTI_VEND equals c.ID_PERS_ENTI
            //              join d in dbe.CLASS_ENTITY on c.ID_ENTI2 equals d.ID_ENTI
            //              join g in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals g.ID_TYPE_DOCU_SALE
            //              join h in dbc.KNOWLEDGEs on a.ID_DOCU_SALE equals h.ID_DOCU_SALE into lh
            //              from xh in lh.DefaultIfEmpty()
            //              join i in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals i.ID_STAT_DOCU_SALE
            //              join j in dbe.CLASS_ENTITY on a.ID_ENTI equals j.ID_ENTI
            //              select new
            //              {
            //                  a.ID_DOCU_SALE,
            //                  COD_TYPE_DOCU_SALE = (g.COD_TYPE_DOCU_SALE == null ? "" : g.COD_TYPE_DOCU_SALE),
            //                  i.COLOR,
            //                  NUM_DOCU_SALE = (a.NUM_DOCU_SALE == null ? "" : a.NUM_DOCU_SALE),
            //                  DAT_DOCU_SALE = (a.DAT_DOCU_SALE == null ? "-" : String.Format("{0:d}", a.DAT_DOCU_SALE)),
            //                  EXP_DATE = (a.EXP_DATE == null ? "-" : String.Format("{0:d}", a.EXP_DATE)),
            //                  VENDOR = d.FIR_NAME + " " + (d.LAS_NAME == null ? "" : d.LAS_NAME),
            //                  a.ID_STAT_DOCU_SALE,
            //                  a.SUM_DOCU_SALE,
            //                  NAM_CLIE_CONT = (xb == null ? "-" : (xb.CONTACT == null ? "-" : xb.CONTACT)).ToLower(),
            //                  CLI_EMAI = (xb == null ? "-" : (xb.EMA_PERS == null ? "-" : xb.EMA_PERS)),
            //                  CLI_TELE = (xb == null ? "-" : (xb.TEL_PERS == null ? "-" : xb.TEL_PERS)),
            //                  CLI_PHON_EXTE = (xb == null ? "-" : (xb.EXT_PERS == null ? "-" : xb.EXT_PERS)),
            //                  CLI_JOB_TITL = (xb == null ? "-" : (xb.CAR_PERS == null ? "-" : xb.CAR_PERS.ToLower())),
            //                  CIA = (xb == null ? "-" : xb.COM_NAME.ToLower()),
            //                  CIA_RUC = (xb == null ? "-" : xb.RUC),
            //                  CIA_ADDR = (xb == null ? "-" : (xb.ADDRESS == null ? "-" : xb.ADDRESS)),
            //                  CIA_TELE = (xb == null ? "-" : (xb.TEL_ENTI == null ? "-" : xb.TEL_ENTI)),
            //                  NAM_FILE = (xh == null ? "" : (xh.NAM_ATTA == null ? "-" : xh.NAM_ATTA.ToLower())),
            //                  NAM_ATTA = (xh == null ? "" : (xh.NAM_ATTA == null ? "-" : xh.NAM_ATTA.ToLower() + "_" + xh.ID_KNOW.ToString() + ".pdf")),
            //                  DATE_ATTA = (xh == null ? "" : (xh.DATE_ATTA == null ? "-" : String.Format("{0:d}", xh.DATE_ATTA))),
            //                  a.OS,
            //                  COM_NAME = j.COM_NAME.ToLower(),
            //              });

            //tt = qUnion.Count();



            //var query = (from a in qUnion.OrderByDescending(x => x.ID_DOCU_SALE).Skip(skip).Take(take).ToList()
            //             join b in dbc.STATUS_DOCUMENT_SALE on a.ID_STAT_DOCU_SALE equals b.ID_STAT_DOCU_SALE
            //             select new
            //             {
            //                 a.ID_DOCU_SALE,
            //                 a.ID_STAT_DOCU_SALE,
            //                 a.NUM_DOCU_SALE,
            //                 a.COD_TYPE_DOCU_SALE,
            //                 a.COLOR,
            //                 a.DAT_DOCU_SALE,
            //                 a.NAM_CLIE_CONT,
            //                 a.CLI_EMAI,
            //                 a.CLI_TELE,
            //                 a.CLI_PHON_EXTE,
            //                 a.CIA,
            //                 a.CIA_RUC,
            //                 a.CLI_JOB_TITL,
            //                 a.CIA_ADDR,
            //                 a.CIA_TELE,
            //                 a.OS,
            //                 a.COM_NAME,
            //                 a.EXP_DATE,
            //                 a.SUM_DOCU_SALE,
            //                 VENDOR = a.VENDOR.ToLower(),
            //                 NAM_STAT_DOCU_SALE = b.NAM_STAT_DOCU_SALE.ToLower(),
            //                 a.NAM_FILE,
            //                 a.NAM_ATTA,
            //                 a.DATE_ATTA,
            //             }).Distinct();

            var query = dbc.OPBuscarAvanzado(ID_TYPE_DOCU_SALE, S_NUM_DOCU_SALE, S_OS, ID_STAT_DOCU_SALE, S_KEYWORD, ID_ENTI, ID_PERS_ENTI, ID_COMP_END, ID_QUEU, ID_PERS_ENTI_ASSI, IdPM, ID_CATE, ID_PERS_ENTI_VEND, 0, ID_ACCO).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Count = total }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Reporte"
        [Authorize]
        public ActionResult ReporteProyectos()
        {
            bool Acceso = false;
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["PROJECTMANAGER"] == 1 || (int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
            {
                Acceso = true;
            }

            if (Acceso == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize]
        public ActionResult ReporteGraficos()
        {
            bool Acceso = false;
            int VistaServiceDesk = 0, VistaPMO = 0;
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["PROJECTMANAGER"] == 1
                || (int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
            {
                Acceso = true;
                VistaPMO = 1;
            }


            if (Acceso == true)
            {
                ViewBag.VistaServiceDesk = VistaServiceDesk;
                ViewBag.VistaPMO = VistaPMO;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }


        public ActionResult RptProyectoRelevante()
        {
            return View();
        }

        public ActionResult RptProyectoCerrado()
        {
            return View();
        }

        public ActionResult RptProyectoDetalleCerrado()
        {
            return View();
        }

        public ActionResult RptProyectoVentas()
        {
            return View();
        }

        public ActionResult RptProyectoServicios()
        {
            return View();
        }

        public ActionResult RptGraficos()
        {
            return View();
        }

        public ActionResult RptProyectosPorFacturar()
        {
            return View();
        }

        public ActionResult RptTransferenciaOP()
        {
            return View();
        }

        public ActionResult RptSoporteDet()
        {
            return View();
        }
        public ActionResult RptMantenimientosProgramados()
        {
            return View();
        }
        public ActionResult RptProyectosPorQ()
        {
            return View();
        }
        public ActionResult RptIndicadores()
        {
            return View();
        }

        [Authorize]
        public ActionResult OPsIniciales()
        {
            bool Acceso = false;
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["RENOVACIONES"] == 1)
            {
                Acceso = true;
            }

            if (Acceso == true)
            {
                return View();
            }
            else
            {
                return Content("<h3><b>Necesitas permisos para acceder a esta sección.</b></h3>");
            }
        }

        [Authorize]
        public ActionResult ProductosRenovados()
        {
            //PERMISO PARA SERVICE DESK Y OPERACIONES
            bool Acceso = false;
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["RENOVACIONES"] == 1 || (int)Session["SUPERVISOR_SERVICEDESK"] == 1 || (int)Session["SUPERVISOR_OPERACIONES"] == 1)
            {
                Acceso = true;
            }

            if (Acceso == true)
            {
                return View();
            }
            else
            {
                return Content("<h3><b>Necesitas permisos para acceder a esta sección.</b></h3>");
            }
        }

        public ActionResult OPsRenovaciones()
        {
            bool Acceso = false;
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["RENOVACIONES"] == 1)
            {
                Acceso = true;
            }

            if (Acceso == true)
            {
                return View();
            }
            else
            {
                return Content("<h3><b>Necesitas permisos para acceder a esta sección.</b></h3>");
            }
        }

        public ActionResult ProductosOP(int id = 0)
        {
            var ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id);
            var td = dbc.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == ds.ID_TYPE_DOCU_SALE.Value);
            ViewBag.OP = ds.NUM_DOCU_SALE;
            op = ds.NUM_DOCU_SALE;
            ViewBag.TipoOP = td.COD_TYPE_DOCU_SALE;
            return View();
        }

        public ActionResult ListaProductos(int id = 0)
        {
            var ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id);
            var td = dbc.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == ds.ID_TYPE_DOCU_SALE.Value);
            ViewBag.OP = ds.NUM_DOCU_SALE;
            ViewBag.IdDocuSale = ds.ID_DOCU_SALE;
            op = ds.NUM_DOCU_SALE;
            ViewBag.TipoOP = td.COD_TYPE_DOCU_SALE;
            return View();
        }

        public ActionResult RenovacionOP(int id = 0)
        {
            var ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id);
            var td = dbc.TYPE_DOCUMENT_SALE.Single(x => x.ID_TYPE_DOCU_SALE == ds.ID_TYPE_DOCU_SALE.Value);
            ViewBag.OP = ds.NUM_DOCU_SALE;
            ViewBag.TipoOP = td.COD_TYPE_DOCU_SALE;
            ViewBag.Id = id;
            return View();
        }

        public ActionResult GrafCantidadProyectosPM()
        {
            // var query = dbc.OPGrafCantidadProyectosPM().ToList();

            // return Json(new { CantidadProy = query}, JsonRequestBehavior.AllowGet);
            return null;
        }

        #endregion

        public JsonResult ListarTipoProyecto(string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            List<ListarTipoProyecto_Result> resultado = dbc.ListarTipoProyecto(43, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarContactos(string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdDocuSale = Convert.ToInt32(Request.QueryString[("IdDocuSale")]);

            List<ListarContactos_Result> resultado = dbc.ListarContactos(IdDocuSale, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarDocumentosObligatorios(int id = 0)
        {
            try
            {

                string query = Convert.ToString(dbc.ListarDocumentosFaltantes(id).SingleOrDefault());
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListarDatosContactos(int id = 0)
        {
            var query = dbc.ListarDatosContactos(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuardarDocumento(PMO pmo)
        {
            //Variables
            int IdDocuSale = 0;
            int IdAccoPara = 0;
            int IdPersEnti = 0;
            int IdUser = 0;

            try
            {
                // Validaciones 
                IdAccoPara = Convert.ToInt32(Request.Params["IdAccoPara"]);
                IdPersEnti = Convert.ToInt32(Request.Params["IdPersEnti"]);
                IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                IdUser = Convert.ToInt32(Session["UserId"].ToString());

                // Valida que el contacto no tenga relacion con el proyecto 
                var query = dbc.Contactoes.Where(x => x.IdDocuSale == IdDocuSale && x.IdPersEnti == IdPersEnti && x.Estado == true && x.IdAccoPara == IdAccoPara);
                if (query.Count() >= 1)
                {
                    return Content("EXISTE");
                }

                Contacto objContacto = new Contacto();
                objContacto.IdDocuSale = IdDocuSale;
                objContacto.IdPersEnti = IdPersEnti;
                objContacto.IdAccoPara = IdAccoPara;
                objContacto.FechaCrea = DateTime.Now;
                objContacto.UsuarioCrea = IdUser;
                objContacto.Estado = true;
                dbc.Contactoes.Add(objContacto);
                dbc.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuardarModuloDocumento()
        {
            //Variables
            string Documento = "";
            try
            {
                // Validaciones 
                Documento = Convert.ToString(Request.Params["Documento"]);

                // Valida que el contacto no tenga relacion con el proyecto 
                var query = dbc.TYPE_DOCUMENT_ATTACH.Where(x => x.NAM_TYPE_DOCU_ATTA == Documento && x.VIG_TYPE_DOCU_ATTA == true);
                if (query.Count() >= 1)
                {
                    return Content("EXISTE");
                }

                TYPE_DOCUMENT_ATTACH objDocumento = new TYPE_DOCUMENT_ATTACH();
                objDocumento.NAM_TYPE_DOCU_ATTA = Documento;
                objDocumento.VIG_TYPE_DOCU_ATTA = true;
                dbc.TYPE_DOCUMENT_ATTACH.Add(objDocumento);
                dbc.SaveChanges();

                TYPE_DOCUMENT_MODULE objModulo = new TYPE_DOCUMENT_MODULE();
                objModulo.ID_TYPE_DOCU_ATTA = objDocumento.ID_TYPE_DOCU_ATTA;
                objModulo.VIG_TYPE_DOCU_MODU = true;
                objModulo.ID_MODU = 1;
                dbc.TYPE_DOCUMENT_MODULE.Add(objModulo);
                dbc.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ObtenerDocumentosObligatorios(int id = 0)
        {
            using (var context = new CoreEntities())
            {
                //Thread.Sleep(3000);
                var result = dbc.ObtenerDocumentoObigatorios(id).ToList();
                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ObtenerMensajeCronograma(int id = 0)
        {
            try
            {
                using (var context = new CoreEntities())
                {
                    var query = dbc.ObtenerMensajeCronograma(id).SingleOrDefault();
                    return Json(query.MensajeCronograma, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListarActividadesAgregadas(int id = 0)
        {
            //int idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"].ToString());
            var query = dbc.ListarCronogramaPorProyecto(id).ToList();

            var total = query.Count();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerNroCronogramas(int id = 0, int id1 = 0)
        {
            var query = dbc.ObtenerNroCronograma(id, id1).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDetalleCronograma(int id = 0)
        {
            ViewBag.Id = id;

            var query = dbc.Gantt(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDetalleCronogramaIniciales(int id = 0, int id1 = 0)
        {
            var query = dbc.ListarCronogramasIniciales(id, id1).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult ValidarCronograma()
        {
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"].ToString());
            var cronograma = dbc.Cronogramas.Where(x => x.IdDocuSale == IdDocuSale).ToList();
            int cantidad = Convert.ToInt32(cronograma.Count());
            if (cantidad > 0)
            {
                return Content("OK");
            }
            else
            {
                return Content("ERROR");
            }
        }
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult ReprogramarActividad(IEnumerable<HttpPostedFileBase> documento)
        {
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"].ToString());
            int IdCronograma = Convert.ToInt32(Request.Params["IdCronograma"].ToString());
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());
            int IdUser = Convert.ToInt32(Session["UserId"].ToString());
            string mensaje = "";

            int archivosAdjuntos = 0;
            //Validando Archivos
            if (documento != null)
            {
                foreach (var file in documento)
                {
                    if (file != null)
                    {
                        archivosAdjuntos = archivosAdjuntos + 1;
                    }
                }
            }
            //Fin

            if ((Convert.ToString(Request.Unvalidated["txtMotivo"])) == "")
            {
                mensaje = "Ingrese un motivo de Reprogramación de Cronograma.";
            }

            //if (archivosAdjuntos == 0)
            //{
            //    mensaje = "Debe adjuntar un archivo.";
            //}
            //if ((Convert.ToInt32(Request.Params["cbTipoDocumento"]) == 0))
            //{
            //    mensaje = "Seleccione un Tipo de Documento ";
            //}
            if (Convert.ToString(Request.Params["dtFechaReprogramada"]).ToString() == "")
            {
                mensaje = "Ingrese la Fecha Reprogramada";
            }
            if (mensaje != "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeReprogramarCronograma) top.MensajeReprogramarCronograma('ERROR','" + mensaje + "');}window.onload=init;</script>");
            }

            DateTime FechaReprogramada = Convert.ToDateTime(Request.Params["dtFechaReprogramada"]);
            string Motivo = Convert.ToString(Request.Unvalidated["txtMotivo"]);

            int IdTipoDocumento;
            if (Request.Params["cbTipoDocumento"] == null)
            {
                IdTipoDocumento = 0;
            }
            else
            {
                IdTipoDocumento = Convert.ToInt32(Request.Params["cbTipoDocumento"].ToString());
            }

            var dtInicio = dbc.Cronogramas.Where(x => x.Id == IdCronograma && x.Estado == true).SingleOrDefault();

            //Guardando reprogramación
            Boolean estado = false;
            string NombreArchivo = "", Extension = "";

            var Docs = new ArchivosActividade();
            Docs.IdDocuSale = IdDocuSale;
            Docs.IdCronograma = IdCronograma;

            Docs.UsuarioCrea = IdUser;
            Docs.FechaCrea = DateTime.Now;
            Docs.FechaInicial = dtInicio.FechaFin;
            Docs.FechaReprogramada = FechaReprogramada;
            Docs.Motivo = Motivo;

            if (archivosAdjuntos == 0)
            {
                Docs.Estado = estado;
                dbc.ArchivosActividades.Add(Docs);
                dbc.SaveChanges();
            }
            else if (documento != null)
            {
                //Adjuntando Archivos
                foreach (var file in documento)
                {
                    if (file != null)
                    {
                        try
                        {
                            estado = true;
                            NombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);
                            Extension = Path.GetExtension(file.FileName);

                            Docs.IdTipoDocumento = IdTipoDocumento;
                            Docs.NombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);
                            Docs.Extension = Path.GetExtension(file.FileName);
                            Docs.Estado = estado;
                            dbc.ArchivosActividades.Add(Docs);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/" + NombreArchivo + "_" + Docs.Id + Extension));
                        }
                        catch
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeReprogramarCronograma) top.MensajeReprogramarCronograma('ERROR','');}window.onload=init;</script>");
                        }
                    }
                }
            }

            //Reprogramando Cronograma
            dbc.ReprogramarActividad(IdCronograma, FechaReprogramada);

            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeReprogramarCronograma) top.MensajeReprogramarCronograma('OK','');}window.onload=init;</script>");
        }
        public ActionResult DetalleActividades(int id, int id1 = 0)
        {
            int pmo = 0, pmoAdmin = 0;

            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_PMO"] == 1)
            {
                pmoAdmin = 1;
            }
            if ((int)Session["PROJECTMANAGER"] == 1 || (int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
            {
                pmo = 1;
            }

            var ds = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id)
                .Join(dbc.TYPE_DOCUMENT_SALE, x => x.ID_TYPE_DOCU_SALE, td => td.ID_TYPE_DOCU_SALE, (x, td) => new
                {
                    x.ID_STAT_DOCU_SALE,
                    x.NUM_DOCU_SALE,
                    td.COD_TYPE_DOCU_SALE,
                    x.Titulo,
                    x.ID_PERS_ENTI_PM
                }).FirstOrDefault();

            ViewBag.ESTADO = ds.ID_STAT_DOCU_SALE;
            ViewBag.ADMINPMO = pmoAdmin;
            ViewBag.PMO = pmo;
            //Cronograma
            var query = dbc.Cronogramas.Where(x => x.Id == id1 && x.Estado == true).SingleOrDefault();

            ViewBag.IdDocuSale = id;
            ViewBag.IdCronograma = id1;
            ViewBag.NombreActividad = query.NombreActividad;
            ViewBag.FechaFin = String.Format("{0:MM/dd/yyyy}", query.FechaFin);
            ViewBag.Porcentaje = query.Completo;
            ViewBag.FechaActual = String.Format("{0:g}", DateTime.Now);

            //Validando Actividad Anterior(100%)
            var mostrar = dbc.ValidarActividadAnterior(id, id1).SingleOrDefault();
            ViewBag.MostrarOpciones = mostrar.Resultado;

            return View();
        }
        [Authorize]
        public ActionResult ActualizarNombreActividad()
        {
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
            int IdCronograma = Convert.ToInt32(Request.Params["IdCronograma"]);
            string Actividad = Convert.ToString(Request.Params["Actividad"]);
            if (Actividad == "")
            {
                return Content("ERROR");
            }

            dbc.ActualizarNombreActividad(IdCronograma, Actividad);

            return Content("OK");
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult AdjuntarArchivosActividades(IEnumerable<HttpPostedFileBase> archivo)
        {

            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
            int IdCronograma = Convert.ToInt32(Request.Params["IdCronograma"]);
            int Porcentaje = Convert.ToInt32(Request.Params["txtPorcentaje"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            int c = 1, archivosAdjuntos = 0;
            //Validando Actas de Conformidades
            if (archivo != null)
            {
                foreach (var file in archivo)
                {
                    if (file != null)
                    {
                        if (Convert.ToInt32(Request.Params["cbTipoDocumento" + c]) == 0)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.Mensaje) top.Mensaje('Archivos','TipoDocumento');}window.onload=init;</script>");
                        }
                        else if (Convert.ToInt32(Request.Params["cbTipoDocumento" + c]) == 1 && Convert.ToString(Request.Params["dtFechaActa" + c]) == "")
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.Mensaje) top.Mensaje('Archivos','FechaActa');}window.onload=init;</script>");
                        }
                        archivosAdjuntos = archivosAdjuntos + 1;
                    }
                    c = c + 1;
                }
            }

            //Validando que haya por lo menos 1 archivo
            var query = dbc.ListarArchivosActividades(IdCronograma).ToList();
            int cantidadArchivos = query.Count();
            //if (cantidadArchivos == 0 && archivosAdjuntos == 0 && Porcentaje == 100)
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //               "if(top.Mensaje) top.Mensaje('Archivos','MostrarArchivos');}window.onload=init;</script>");
            //}

            c = 1;

            //Validando Archivos
            if (archivo != null)
            {

                //Adjuntando Archivos
                foreach (var file in archivo)
                {
                    if (file != null)
                    {
                        try
                        {

                            string nombre = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            var Docs = new ArchivosActividade();
                            Docs.IdDocuSale = IdDocuSale;
                            Docs.IdCronograma = IdCronograma;
                            Docs.IdTipoDocumento = Convert.ToInt32(Request.Params["cbTipoDocumento" + c]);
                            DateTime DateFechaActa = DateTime.Now;
                            if (Convert.ToInt32(Request.Params["cbTipoDocumento" + c]) == 1)
                            {
                                DateFechaActa = Convert.ToDateTime(Request.Params["dtFechaActa" + c]);
                            }
                            Docs.FechaActaConformidad = DateFechaActa;
                            Docs.NombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);
                            Docs.Extension = Path.GetExtension(file.FileName); ;
                            Docs.UsuarioCrea = UserId;
                            Docs.FechaCrea = DateTime.Now;
                            Docs.Estado = true;
                            dbc.ArchivosActividades.Add(Docs);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/" + Docs.NombreArchivo + "_" + Convert.ToString(Docs.Id) + Docs.Extension));


                        }
                        catch
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.Mensaje) top.Mensaje('ERROR','Contacte al Administrador.');}window.onload=init;</script>");


                        }
                    }
                    c = c + 1;
                }
            }

            var cr = dbc.Cronogramas.Where(x => x.Id == IdCronograma).SingleOrDefault();
            if (cr.Completo < 100)
            {
                cr.Completo = Porcentaje;
                dbc.Entry(cr).State = EntityState.Modified;
                dbc.SaveChanges();
            }

            return Content("<script type='text/javascript'> function init() {" +
                                 "if(top.Mensaje) top.Mensaje('OK','ActividadGuardada');}window.onload=init;</script>");
        }

        public ActionResult ObtenerArchivosActividades(int id = 0)
        {
            var query = dbc.ListarArchivosActividades(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerReprogramaciones(int id = 0)
        {
            var query = dbc.ListarReprogramaciones(id);

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarOPsIniciales()
        {
            string anno = "", mes = "";
            if (Request.Params["anno"] != "")
                anno = Convert.ToString(Request.Params["anno"]);
            if (Request.Params["mes"] != "")
                mes = Request.Params["mes"];
            string numOP = Request.Params["op"];
            if (numOP == null)
                numOP = "";
            List<RenovacionesOPsIniciales_Result> resultado = dbc.RenovacionesOPsIniciales(anno, mes, numOP).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarOPsRenovaciones()
        {
            string anno = "", mes = "";
            if (Request.Params["anno"] != "")
                anno = Convert.ToString(Request.Params["anno"]);
            if (Request.Params["mes"] != "")
                mes = Request.Params["mes"];
            string numOP = Request.Params["op"];
            if (numOP == null)
                numOP = "";
            List<RenovacionesOPsRenovaciones_Result> resultado = dbc.RenovacionesOPsRenovaciones(anno, mes, numOP).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosOP(int id = 0)
        {
            var query = dbc.ObtenerDatosOP(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarProyectoDeRenovacion()
        {

            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
            try
            {
                ProyectosSinRenovacion ps = new ProyectosSinRenovacion();
                ps.IdDocuSale = IdDocuSale;
                ps.FechaCrea = DateTime.Now;
                ps.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                ps.Estado = false;

                dbc.ProyectosSinRenovacions.Add(ps);
                dbc.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public JsonResult ListarOPPorRenovar(string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var resultado = dbc.ListarOPPorRenovar(termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProductosOP()
        {
            List<ListarProductosOP_Result> resultado = dbc.ListarProductosOP(op).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult actualizacionOP()
        {
            try
            {
                int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                var queryOP = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == IdDocuSale);
                DateTime fechaActual = DateTime.Now;
                queryOP.FechaCaducidad = fechaActual.AddYears(1);
                queryOP.EstadoRenovacion = true;
                dbc.Entry(queryOP).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult renovacionParcial()
        {
            try
            {
                int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                int IdOPRenovacion = 0;
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (Request.Params["IdOPRenovacion"] != "null")
                {
                    var queryOP = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == IdDocuSale);
                    DateTime fechaActual = DateTime.Now;
                    queryOP.FechaCaducidad = fechaActual.AddYears(1);
                    queryOP.EstadoRenovacion = true;
                    dbc.Entry(queryOP).State = EntityState.Modified;
                    dbc.SaveChanges();

                    IdOPRenovacion = Convert.ToInt32(Request.Params["IdOPRenovacion"]);
                    OPRenovacion objOPRenovacion = new OPRenovacion();
                    objOPRenovacion.IdDocuSale = IdDocuSale;
                    objOPRenovacion.OpRenovacion1 = IdOPRenovacion;
                    objOPRenovacion.UsuarioCrea = UserId;
                    objOPRenovacion.FechaCrea = DateTime.Now;
                    dbc.OPRenovacions.Add(objOPRenovacion);
                    dbc.SaveChanges();

                    return Content("OK");
                }
                else
                {
                    return Content("ERROR");
                }
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult renovacionTotal()
        {
            try
            {
                int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
                int IdOPRenovacion = 0;
                int UserId = Convert.ToInt32(Session["UserId"]);
                if (Request.Params["IdOPRenovacion"] != "null")
                {
                    var queryOP = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == IdDocuSale);
                    queryOP.EstadoRenovacion = false;
                    string s = Request.Params["IdOPRenovacion"];
                    dbc.Entry(queryOP).State = EntityState.Modified;
                    dbc.SaveChanges();

                    IdOPRenovacion = Convert.ToInt32(Request.Params["IdOPRenovacion"]);
                    OPRenovacion objOPRenovacion = new OPRenovacion();
                    objOPRenovacion.IdDocuSale = IdDocuSale;
                    objOPRenovacion.OpRenovacion1 = IdOPRenovacion;
                    objOPRenovacion.UsuarioCrea = UserId;
                    objOPRenovacion.FechaCrea = DateTime.Now;
                    dbc.OPRenovacions.Add(objOPRenovacion);
                    dbc.SaveChanges();

                    return Content("OK");
                }
                else
                {
                    return Content("ERROR");
                }
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ListarProductosRenovados()
        {
            string idMarca = "";
            int idCliente = 0;
            int idClienteFinal = 0;
            int idTipoOP = 0;
            string op = "";
            if (Request.Params["idCliente"] != "null")
                idCliente = Convert.ToInt32(Request.Params["idCliente"]);
            if (Request.Params["idClienteFinal"] != "null")
                idClienteFinal = Convert.ToInt32(Request.Params["idClienteFinal"]);
            if (Request.Params["idTipoOP"] != "null")
                idTipoOP = Convert.ToInt32(Request.Params["idTipoOP"]);
            if (Request.Params["idMarca"] != "null")
                idMarca = Request.Params["idMarca"];
            if (Request.Params["OP"] != "null")
                op = Request.Params["OP"];
            List<ListarProductosRenovados_Result> resultado = dbc.ListarProductosRenovados(idMarca, idCliente, idClienteFinal, idTipoOP, op).ToList();

            return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarTipoOP(string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var resultado = dbc.ListarTipoOP(termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarMarcaProducto()
        {
            int id_comp = 0;
            string txt = "";
            int idMarca = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                id_comp = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "Marca")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                id_comp = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            var result = dbc.ListarMarcaProducto(txt, id_comp, idMarca).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarMarcasSidige(string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var resultado = dbc.ListarMarcasSidige(termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


        //Reportes Gerencia
        public ActionResult GrafProyectosEstado(int id = 0)
        {
            try
            {
                var query = dbc.GrafProyectosxEstado(id).ToList();

                return Json(new { ProyectosxEstado = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GrafMontoProyectos(int id = 0)
        {
            try
            {
                var query = dbc.GrafMontoProyectos(id).ToList();

                return Json(new { ProyectosxMonto = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ConsolidadoProyectosxEstado(int id = 0, int id1 = 0)
        {
            var result = dbc.ConsolidadoProyectosxEstado(id, id1).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarConsolidadoProyectos(int id = 0)
        {
            var result = dbc.ListarConsolidadoProyectos(id).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarActividadesPendientes(int id = 0, int id1 = 0, int id2 = 0)
        {
            var result = dbc.ListarActividadesPendientes(id, id1, id2).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerUltimoComentario(int id = 0)
        {
            var result = dbc.ObtenerUltimoComentario(id).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GrafProyectosPM(int id = 0)
        {
            try
            {
                DateTime FechaInicio, FechaFin;
                if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
                {
                    FechaInicio = Convert.ToDateTime(null);
                }
                else
                {
                    FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                }
                if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
                {
                    FechaFin = DateTime.Now;
                }
                else
                {
                    FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
                }

                var query = dbc.GrafProyectosPM(FechaInicio, FechaFin, id).ToList();

                return Json(new { ProyectosPM = query }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GrafProyectosEspecialista(int id = 0)
        {
            DateTime FechaInicio, FechaFin;
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }

            var query = dbc.GrafProyectosEspecialista(FechaInicio, FechaFin, id).ToList();

            return Json(new { ProyectosEspecialista = query }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult ReporteProyectosPM(int id = 0, int id1 = 0, int id2 = 0)
        {
            DateTime FechaInicio, FechaFin;
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }
            var result = dbc.ConsolidadoProjectManajer(id, FechaInicio, FechaFin, id1, id2);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GrafMontosxPM(int id = 0, int id1 = 0)
        {
            DateTime FechaInicio, FechaFin;
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }
            var result = dbc.ListarMontosxPM(id, FechaInicio, FechaFin, id1);
            return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProyectosxPM(int id = 0, int id1 = 0, int id2 = 0)
        {
            DateTime FechaInicio, FechaFin;
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }
            var result = dbc.ListarProyectosxPM(id, FechaInicio, FechaFin, id1, id2).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteProyectosEspecialista(int id = 0, int id1 = 0, int id2 = 0)
        {
            DateTime FechaInicio, FechaFin;
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }
            var result = dbc.ConsolidadoEspecialista(id, FechaInicio, FechaFin, id1, id2);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GrafMontosxEspecialista(int id = 0, int id1 = 0)
        {
            DateTime FechaInicio, FechaFin;
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }
            var result = dbc.ListarMontosxEspecialista(id, FechaInicio, FechaFin, id1);
            return Json(new { pie = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProyectosxEspecialista(int id = 0, int id1 = 0, int id2 = 0)
        {
            DateTime FechaInicio, FechaFin;
            if (Convert.ToString(Request.QueryString["FechaInicio"]) == "0")
            {
                FechaInicio = Convert.ToDateTime(null);
            }
            else
            {
                FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            }
            if (Convert.ToString(Request.QueryString["FechaFin"]) == "0")
            {
                FechaFin = DateTime.Now;
            }
            else
            {
                FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            }
            var result = dbc.ListarProyectosxEspecialista(id, FechaInicio, FechaFin, id1, id2).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerDatosPersonal(int id = 0)
        {
            try
            {
                var query = dbc.ObtenerDatosPersonal(id).SingleOrDefault();
                return Json(query.Personal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ObtenerIdPersonal(string id = "")
        {
            try
            {
                var query = dbc.ObtenerIdPersonal(id).SingleOrDefault();
                return Json(query.IdPersEnti, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ListarOPNuevoAsignado(int id = 0)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int sw = 0;
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                sw = 1;
            }
            else if ((int)Session["SUPERVISOR_PMO_LISTADO"] == 1 || (int)Session["PMO_SOPORTE"] == 1)//(int)Session["SUPERVISOR_PMO"] == 1
            {
                sw = 2;
            }
            else if ((int)Session["SUPERVISOR_PMO_OUTSOURCING"] == 1)
            {
                sw = 3;
            }
            var result = dbc.ListarOPNuevoAsignado(IdPersEnti, id, sw, IdCuenta).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCantidadOP()
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int sw = 0;
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                sw = 1;
            }
            else if ((int)Session["SUPERVISOR_PMO_LISTADO"] == 1 || (int)Session["PMO_SOPORTE"] == 1) //Session["SUPERVISOR_PMO"] == 1
            {
                sw = 2;
            }
            else if ((int)Session["SUPERVISOR_PMO_OUTSOURCING"] == 1)
            {
                sw = 3;
            }
            var query = dbc.ObtenerCantidadOP(IdPersEnti, sw, IdCuenta).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerDocumento(string id = "", string id1 = "")
        {
            ViewBag.Flag = id;
            ViewBag.Documento = id1;
            return View();
        }
        public FileResult DescargarArchivo(string id = "", string id1 = "")
        {
            try
            {
                string ruta = "";
                if (id == "0")
                {
                    ruta = "~/Adjuntos/Sales/OP/";
                }
                else if (id == "1")
                {
                    ruta = "~/Adjuntos/";
                }
                FileStream fileStream = System.IO.File.OpenRead(Server.MapPath(ruta + id1));
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
                if ((id1.ToLower()).Contains(".pdf"))
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".txt"))
                {
                    Response.ContentType = "text/text";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".txt");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".jpg"))
                {
                    Response.ContentType = "image/jpeg";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".jpg");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".png"))
                {
                    Response.ContentType = "image/png";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".png");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else
                {
                    string filename = ruta + id1;
                    return File(filename, "application/pdf", id1);
                }


                //string filename = "~/Adjuntos/" + id1;
                //return File(filename, "application/pdf", id1);
                //return File(filename, "application/pdf", Server.UrlEncode(filename));

                //return File(storeStream.GetBuffer(), "xml");
            }
            catch
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

                return File(stream, "text/plain", "Error.txt");
            }

        }

        public ActionResult ObtenerFechaDocumento(int id) //ID_DOCU_SALE
        {
            var obj = dbc.ATTACHEDs.Where(x => x.ID_DOCU_SALE == id && x.ID_TYPE_DOCU_ATTA == 1).FirstOrDefault();

            string strFecha;

            if (obj == null)
                strFecha = "";
            else
                strFecha = ((DateTime)obj.FechaActaConformidad).ToString("dd/MM/yyyy");

            //return Json(new { data = FechaActaConformidad.ToString("yyyy-MM-dd") }, JsonRequestBehavior.AllowGet);
            return Json(strFecha, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarMantenimiento(int id = 0, int id1 = 0, int id2 = 0)
        {
            int IdAsignado;
            string estado = "";
            DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            //int IdAsignado = Convert.ToInt32(Request.QueryString["IdAsignado"]);
            if (Convert.ToString(Request.QueryString["IdAsignado"]) == "")
            {
                IdAsignado = Convert.ToInt32(null);
            }
            else
            {
                IdAsignado = Convert.ToInt32(Request.QueryString["IdAsignado"]);
            }

            if (Convert.ToString(Request.QueryString["IdEstados"]) == "null")
            {
                estado = "";
            }
            else
            {
                estado = Convert.ToString(Request.QueryString["IdEstados"]);
            }

            var result = dbc.BuscarMantenimiento(id, id1, id2, IdAsignado, FechaInicio, FechaFin, estado).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCronogramaSoporteOP()
        {
            int IdCliente = 0, IdClienteFinal = 0, IdMarca = 0;
            if (Request.Params["IdCliente"].ToString() != "")
            {
                IdCliente = Convert.ToInt32(Request.Params["IdCliente"].ToString());
            }
            if (Request.Params["IdClienteFinal"].ToString() != "")
            {
                IdClienteFinal = Convert.ToInt32(Request.Params["IdClienteFinal"]);
            }
            if (Request.Params["IdMarca"].ToString() != "")
            {
                IdMarca = Convert.ToInt32(Request.Params["IdMarca"]);
            }
            string Tipo = Convert.ToString(Request.Params["Tipo"]);
            string OP = Convert.ToString(Request.Params["OP"]);
            var result = dbc.CronogramaSoporteOP(IdCliente, IdClienteFinal, IdMarca, Tipo, OP).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public string ObtenerDatoCorreoNOC(int id = 0)
        {
            var envioCorreo = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).SingleOrDefault();
            if (envioCorreo.EnvioCorreoNOC.ToString() == "1")
            {
                return "El correo ya fue enviado anteriormente.";
            }
            else
            {
                return "";
            }
        }

        public string EnviarCorreoSoporte(int id = 0)
        {
            try
            {
                SendMail mail = new SendMail();
                Boolean flag = mail.EnviarSoporteInforme(id);
                if (flag == true)
                {
                    DOCUMENT_SALE objActa = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == id);
                    objActa.EnvioCorreoNOC = 1;
                    dbc.Entry(objActa).State = EntityState.Modified;
                    dbc.SaveChanges();
                    return "OK";
                }
                else
                {
                    return "ERROR";

                }
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        #region ActaDeConstitucion

        public ActionResult ObtenerDatosActa(int id = 0)
        {
            var query = dbc.ObtenerDatosActa(id);
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosActaRiesgo(int id = 0)
        {
            var query = dbc.ObtenerDatosActaRiesgo(id);
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosActaHito(int id = 0)
        {
            var query = dbc.ObtenerDatosActaHito(id);
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosActaFacturacion(int id = 0)
        {
            var query = dbc.ObtenerDatosActaFacturacion(id);
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDatosActaInteresados(int id = 0)
        {
            var query = dbc.ObtenerDatosActaInteresados(id);
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPmoAsignados(int id, string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var queryDocuSale = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id).FirstOrDefault();
            if (queryDocuSale.EstadoTipoProyecto == 1)
            {
                List<ListarPmoAsignados_Result> resultado = dbc.ListarPmoAsignados(1, IdAcco, termino).ToList();
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<ListarPmoAsignados_Result> resultado = dbc.ListarPmoAsignados(2, IdAcco, termino).ToList();
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ListarSimboloFacturacion(string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var resultado = dbc.ListarSimboloFacturacion(termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RegistrarActa()
        {
            //Sesiones
            int UserId = Convert.ToInt32(Session["UserId"]);
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            //Declaración de variables
            int IdActa = 0;
            int IdPM = Convert.ToInt32(null), IdPMCliente = Convert.ToInt32(null);
            DateTime FechaActa = Convert.ToDateTime(null);
            int IdDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
            int FlagActa = Convert.ToInt32(Request.Params["FlagActa"]);
            int Finalizado = Convert.ToInt32(Request.Params["Finalizado"]);
            string mensaje = "";
            //Asignación de contadores
            int ContadorRiesgo = Convert.ToInt32(Request.Params["ContadorRiesgo"]);
            int ContadorHito = Convert.ToInt32(Request.Params["ContadorHito"]);
            int ContadorInteresado = Convert.ToInt32(Request.Params["ContadorInteresado"]);
            int ContadorFacturacion = Convert.ToInt32(Request.Params["ContadorFacturacion"]);

            //Variables del Acta
            string OP = Convert.ToString(Request.Params["txtOP"]);
            string Nombre = Convert.ToString(Request.Params["txtNombre"]);
            string Alcance = Convert.ToString(Request.Params["txtAlcance"]);
            string Requisito = Convert.ToString(Request.Params["txtRequisito"]);
            string Objetivo = Convert.ToString(Request.Params["txtObjetivo"]);
            string Finalidad = Convert.ToString(Request.Params["txtFinalidad"]);
            string Justificacion = Convert.ToString(Request.Params["txtJustificacion"]);
            string Exclusion = Convert.ToString(Request.Params["txtExclusion"]);
            string Restriccion = Convert.ToString(Request.Params["txtRestriccion"]);
            string Supuesto = Convert.ToString(Request.Params["txtSupuesto"]);

            string IdInteresado = "";
            int valInteresados = 0;
            //Validación Finalizado
            if (Finalizado == 1)
            {
                if (Nombre == "") { mensaje = "- Ingrese el nombre.</br>"; }
                if (Alcance == "") { mensaje = mensaje + "- Ingrese el alcance.</br>"; }
                if (Requisito == "") { mensaje = mensaje + "- Ingrese el requisito.</br>"; }
                if (Objetivo == "") { mensaje = mensaje + "- Ingrese el objetivo.</br>"; }
                if (Exclusion == "") { mensaje = mensaje + "- Ingrese la exclusión</br>"; }
                if (Restriccion == "") { mensaje = mensaje + "- Ingrese la restricción</br>"; }
                if (Supuesto == "") { mensaje = mensaje + "- Ingrese el supuesto.</br>"; }
                if (Request.Params["dtFechaActa"] == "") { mensaje = mensaje + "- Ingrese la fecha del acta.</br>"; }
                if (mensaje != "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeActa) top.MensajeActa('Validacion','" + mensaje + "');}window.onload=init;</script>");
                }
                //Hitos
                if (Request.Params["dtFechaInicio"] == "")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                         "if(top.MensajeActa) top.MensajeActa('Validacion','Ingrese la fecha de inicio del hito.');}window.onload=init;</script>");
                }
                //Projects Managers
                if (Convert.ToString(Request.Params["cbPM"]) == null || Convert.ToString(Request.Params["cbPMCliente"]) == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                         "if(top.MensajeActa) top.MensajeActa('Validacion','Seleccione a los PM del proyecto.');}window.onload=init;</script>");
                }
                //Registro de Interesados Electrodata
                for (int contInteresado = 1; contInteresado <= ContadorInteresado; contInteresado++)
                {
                    IdInteresado = Convert.ToString(Request.Params["cbInteresado" + contInteresado]);
                    if (String.IsNullOrEmpty(IdInteresado) == false)
                    {
                        valInteresados++;
                    }
                }
            }
            //Validación de Riesgos
            string riesgo = "", impacto = "", responsable = "";
            int valRiesgos = 0;
            for (int contRiesgo = 1; contRiesgo <= ContadorRiesgo; contRiesgo++)
            {
                riesgo = Convert.ToString(Request.Params["txtRiesgo" + contRiesgo]);
                impacto = Convert.ToString(Request.Params["txtImpacto" + contRiesgo]);
                responsable = Convert.ToString(Request.Params["cbResponsable" + contRiesgo]);
                if (String.IsNullOrEmpty(riesgo) == true && String.IsNullOrEmpty(impacto) == true && responsable == null) { }
                else if (String.IsNullOrEmpty(riesgo) == false && String.IsNullOrEmpty(impacto) == false && responsable != null)
                {
                    valRiesgos++;
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeActa) top.MensajeActa('Riesgo','');}window.onload=init;</script>");
                }
            }
            if (valRiesgos == 0 && Finalizado == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                     "if(top.MensajeActa) top.MensajeActa('Validacion','Ingrese como mínimo un riesgo.');}window.onload=init;</script>");
            }
            //Validación Hitos
            int maximoPeso = 0, valHitos = 0;
            string actividad = "", FechaFin = "", peso = "", aplicaHito = "", observacion = "", entregable = "";
            for (int contHito = 1; contHito <= ContadorHito; contHito++)
            {
                actividad = Convert.ToString(Request.Params["txtActividad" + contHito]);
                FechaFin = Convert.ToString(Request.Params["dtFecha" + contHito]);
                peso = Convert.ToString(Request.Params["txtPeso" + contHito]);
                if (String.IsNullOrEmpty(actividad) == true && String.IsNullOrEmpty(FechaFin) == true &&
                    String.IsNullOrEmpty(peso) == true) { }
                else
                    if (String.IsNullOrEmpty(actividad) == false && String.IsNullOrEmpty(FechaFin) == false &&
                        String.IsNullOrEmpty(peso) == false && String.IsNullOrEmpty(Request.Params["dtFechaInicio"]) == false)
                {
                    if (Convert.ToInt32(peso) > maximoPeso)
                    {
                        maximoPeso = Convert.ToInt32(peso);
                    }
                    valHitos++;
                    //Validando la fecha
                    if (Convert.ToDateTime(Request.Params["dtFechaInicio"]) > Convert.ToDateTime(FechaFin))
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.MensajeActa) top.MensajeActa('HitosFecha','Las fechas de las actividades debe ser mayor a la Fecha de Inicio (" + String.Format("{0:MM/dd/yyyy}", Request.Params["dtFechaInicio"]) + ")');}window.onload=init;</script>");
                    }
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.MensajeActa) top.MensajeActa('Hitos','');}window.onload=init;</script>");
                }
            }
            if (maximoPeso != 100 && valHitos > 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeActa) top.MensajeActa('HitosPeso','');}window.onload=init;</script>");
            }
            if (valHitos == 0 && Finalizado == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeActa) top.MensajeActa('Validacion','Ingrese como mínimo un hito.');}window.onload=init;</script>");
            }
            //Validación de Facturación
            int valFacturacion = 0;
            string item = "", monto = "", requisito = "", simbolo = "";
            for (int contFacturacion = 1; contFacturacion <= ContadorFacturacion; contFacturacion++)
            {
                item = Convert.ToString(Request.Params["txtItem" + contFacturacion]);
                monto = Convert.ToString(Request.Params["txtMonto" + contFacturacion]);
                requisito = Convert.ToString(Request.Params["txtRequisito" + contFacturacion]);
                simbolo = Convert.ToString(Request.Params["cbSimbolo" + contFacturacion]);
                if ((String.IsNullOrEmpty(item) == true && String.IsNullOrEmpty(monto) == true && String.IsNullOrEmpty(requisito) == true
                    && String.IsNullOrEmpty(simbolo) == true)) { }
                else if (String.IsNullOrEmpty(item) == false && String.IsNullOrEmpty(monto) == false && String.IsNullOrEmpty(requisito) == false
                    && String.IsNullOrEmpty(simbolo) == false)
                {
                    valFacturacion++;
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                     "if(top.MensajeActa) top.MensajeActa('Facturacion','');}window.onload=init;</script>");
                }
            }

            if (valFacturacion == 0 && Finalizado == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                     "if(top.MensajeActa) top.MensajeActa('Validacion','Ingrese como mínimo una facturación.');}window.onload=init;</script>");
            }

            if (valInteresados == 0 && Finalizado == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.MensajeActa) top.MensajeActa('Validacion','Seleccione los interesados de Electrodata.');}window.onload=init;</script>");
            }
            //Validación de Project Managers
            if (Convert.ToString(Request.Params["cbPM"]) != null)
            {
                IdPM = Convert.ToInt32(Request.Params["cbPM"]);
            }
            if (Convert.ToString(Request.Params["cbPMCliente"]) != null)
            {
                IdPMCliente = Convert.ToInt32(Request.Params["cbPMCliente"]);
            }
            //Registros
            if (FlagActa == 0) //Guardar por 1ra vez
            {
                ActaConstitucion objActa = new ActaConstitucion();
                objActa.IdDocuSale = IdDocuSale;
                objActa.IdPM = IdPM;
                objActa.IdPMCliente = IdPMCliente;
                if (Request.Params["dtFechaActa"] == "")
                {
                    objActa.FechaActa = null;
                }
                else
                {
                    objActa.FechaActa = Convert.ToDateTime(Request.Params["dtFechaActa"]);
                }
                objActa.Nombre = Nombre;
                objActa.Alcance = Alcance;
                objActa.Requisito = Requisito;
                objActa.Objetivo = Objetivo;
                objActa.Finalidad = Finalidad;
                objActa.Justificacion = Justificacion;
                objActa.Exclusion = Exclusion;
                objActa.Restriccion = Restriccion;
                objActa.Supuesto = Supuesto;
                objActa.FechaCrea = DateTime.Now;
                objActa.UsuarioCrea = UserId;
                objActa.Finalizado = Finalizado;
                objActa.Estado = true;
                dbc.ActaConstitucion.Add(objActa);
                dbc.SaveChanges();

                IdActa = objActa.Id;
            }
            else //Editar
            {
                ActaConstitucion objActa = dbc.ActaConstitucion.Single(x => x.IdDocuSale == IdDocuSale);
                objActa.IdDocuSale = IdDocuSale;
                objActa.IdPM = IdPM;
                objActa.IdPMCliente = IdPMCliente;
                if (Request.Params["dtFechaActa"] == "")
                {
                    objActa.FechaActa = null;
                }
                else
                {
                    objActa.FechaActa = Convert.ToDateTime(Request.Params["dtFechaActa"]);
                }
                objActa.Nombre = Nombre;
                objActa.Alcance = Alcance;
                objActa.Requisito = Requisito;
                objActa.Objetivo = Objetivo;
                objActa.Finalidad = Finalidad;
                objActa.Justificacion = Justificacion;
                objActa.Exclusion = Exclusion;
                objActa.Restriccion = Restriccion;
                objActa.Supuesto = Supuesto;
                objActa.FechaCrea = DateTime.Now;
                objActa.UsuarioCrea = UserId;
                objActa.Finalizado = Finalizado;
                objActa.Estado = true;
                dbc.Entry(objActa).State = EntityState.Modified;
                dbc.SaveChanges();

                IdActa = objActa.Id;
            }
            //Eliminando riesgos, facturación e interesados
            try
            {
                dbc.EliminarDatosActa(IdActa);
            }
            catch (Exception e)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeActa) top.MensajeActa('ERROR','" + e.InnerException + "');}window.onload=init;</script>");
            }
            //Registro de Riesgos
            for (int contRiesgo = 1; contRiesgo <= ContadorRiesgo; contRiesgo++)
            {
                riesgo = Convert.ToString(Request.Params["txtRiesgo" + contRiesgo]);
                impacto = Convert.ToString(Request.Params["txtImpacto" + contRiesgo]);
                responsable = Convert.ToString(Request.Params["cbResponsable" + contRiesgo]);
                if (String.IsNullOrEmpty(riesgo) == false && String.IsNullOrEmpty(impacto) == false && responsable != null)
                {
                    ActaRiesgos objRiesgo = new ActaRiesgos();
                    objRiesgo.IdActa = IdActa;
                    objRiesgo.IdResponsable = Convert.ToInt32(responsable);
                    objRiesgo.Riesgo = riesgo;
                    objRiesgo.Impacto = impacto;
                    objRiesgo.Estado = true;
                    dbc.ActaRiesgos.Add(objRiesgo);
                    dbc.SaveChanges();
                }
            }
            //Registro de Facturación
            for (int contFacturacion = 1; contFacturacion <= ContadorFacturacion; contFacturacion++)
            {
                item = Convert.ToString(Request.Params["txtItem" + contFacturacion]);
                monto = Convert.ToString(Request.Params["txtMonto" + contFacturacion]);
                requisito = Convert.ToString(Request.Params["txtRequisito" + contFacturacion]);
                simbolo = Convert.ToString(Request.Params["cbSimbolo" + contFacturacion]);
                if (String.IsNullOrEmpty(item) == false && String.IsNullOrEmpty(monto) == false
                    && String.IsNullOrEmpty(requisito) == false && String.IsNullOrEmpty(simbolo) == false)
                {
                    ActaFacturacion objFacturacion = new ActaFacturacion();
                    objFacturacion.IdActa = IdActa;
                    objFacturacion.Item = item;
                    objFacturacion.IdSimbolo = Convert.ToInt32(simbolo);
                    objFacturacion.Monto = Convert.ToDecimal(monto);
                    objFacturacion.Requisito = requisito;
                    objFacturacion.Estado = true;
                    dbc.ActaFacturacion.Add(objFacturacion);
                    dbc.SaveChanges();
                }
            }
            //Registro de cronograma
            if (Request.Params["dtFechaInicio"] != "")
            {
                DateTime FechaInicio = Convert.ToDateTime(Request.Params["dtFechaInicio"]);
                dbc.RegistrarActividadInicio(IdDocuSale, UserId, IdPersEnti, FechaInicio);

                for (int contHito = 1; contHito <= ContadorHito; contHito++)
                {
                    actividad = Convert.ToString(Request.Params["txtActividad" + contHito]);
                    FechaFin = Convert.ToString(Request.Params["dtFecha" + contHito]);
                    peso = Convert.ToString(Request.Params["txtPeso" + contHito]);
                    aplicaHito = Convert.ToString(Request.Params["chkHito" + contHito]);
                    observacion = Convert.ToString(Request.Params["txtObservacion" + contHito]);
                    entregable = Convert.ToString(Request.Params["txtEntregable" + contHito]);
                    if (String.IsNullOrEmpty(actividad) == false && String.IsNullOrEmpty(FechaFin) == false &&
                        String.IsNullOrEmpty(peso) == false)
                    {
                        Cronograma objCronograma = new Cronograma();
                        objCronograma.IdDocuSale = IdDocuSale;
                        objCronograma.NombreActividad = actividad;
                        objCronograma.IdAccoPara = 252;
                        objCronograma.FechaInicio = FechaInicio;
                        objCronograma.FechaFin = Convert.ToDateTime(FechaFin);
                        objCronograma.FechaCrea = DateTime.Now;
                        objCronograma.UsuarioCrea = UserId;
                        objCronograma.Estado = true;
                        objCronograma.NroCronograma = 1;
                        objCronograma.InicioCronograma = 0;
                        objCronograma.Estilo = "gtaskblue";
                        objCronograma.Mile = 0;
                        objCronograma.IdPersEnti = IdPersEnti;
                        objCronograma.Completo = 0;
                        objCronograma.Grupo = 0;
                        objCronograma.Padre = 1;
                        objCronograma.Abierto = 1;
                        objCronograma.Depende = "";
                        objCronograma.Subtitulo = "";
                        objCronograma.Notas = "";
                        objCronograma.Gantt = "g";
                        objCronograma.Peso = Convert.ToInt32(peso);
                        objCronograma.Recurso = Convert.ToInt32(dbc.ObtenerDiferenciasFechas(FechaInicio, Convert.ToDateTime(FechaFin)).SingleOrDefault());
                        if (aplicaHito == "1") { objCronograma.AplicaHito = true; }
                        else { objCronograma.AplicaHito = false; }
                        objCronograma.Observaciones = observacion;
                        objCronograma.Entregables = entregable;
                        dbc.Cronogramas.Add(objCronograma);
                        dbc.SaveChanges();
                    }
                }
            }
            //Registro de Interesados Electrodata
            for (int contInteresado = 1; contInteresado <= ContadorInteresado; contInteresado++)
            {
                IdInteresado = Convert.ToString(Request.Params["cbInteresado" + contInteresado]);
                if (String.IsNullOrEmpty(IdInteresado) == false)
                {
                    ActaInteresados objInteresado = new ActaInteresados();
                    objInteresado.IdActa = IdActa;
                    objInteresado.IdActaTipoInteresado = 1;
                    objInteresado.IdPersEnti = Convert.ToInt32(IdInteresado);
                    objInteresado.Estado = true;
                    dbc.ActaInteresados.Add(objInteresado);
                    dbc.SaveChanges();
                }
            }
            //Registro de Interesados Cliente
            var listaContactos = dbc.ListarContactosOP(IdDocuSale, "").ToList();
            foreach (var lista in listaContactos)
            {
                if (String.IsNullOrEmpty(Request.Params["chkContacto" + lista.id]) == false)
                {
                    ActaInteresados objInteresado = new ActaInteresados();
                    objInteresado.IdActa = IdActa;
                    objInteresado.IdActaTipoInteresado = 2;
                    objInteresado.IdPersEnti = Convert.ToInt32(lista.id);
                    objInteresado.Estado = true;
                    dbc.ActaInteresados.Add(objInteresado);
                    dbc.SaveChanges();
                }
            }

            if (Finalizado == 1)
            {
                //Guardar documento
                string archivo = RptOPActaConstitucion(IdDocuSale, OP);
                if (archivo == "ERROR")
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.MensajeActa) top.MensajeActa('ERROR','El documento no fue adjuntado');}window.onload=init;</script>");
                }
            }
            return Content("<script type='text/javascript'> function init() {" +
            "if(top.MensajeActa) top.MensajeActa('OK','');}window.onload=init;</script>");
        }

        public string RptOPActaConstitucion(int IdDocuSale, string OP)
        {
            try
            {
                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                string nombreArchivo = "ACP " + OP;
                string extension = ".pdf";
                int UserId = Convert.ToInt32(Session["UserId"]);
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                //Guardando registro
                var ATTA = new ATTACHED();
                ATTA.NAM_ATTA = nombreArchivo;
                ATTA.EXT_ATTA = extension;
                //ATTA.ID_INCI = ticket.ID_TICK;
                ATTA.ID_DOCU_SALE = IdDocuSale;
                ATTA.CREATE_ATTA = DateTime.Now;
                ATTA.KEY_ATTA = null;
                ATTA.ID_TYPE_DOCU_ATTA = 7;
                ATTA.DELETE_ATTA = false;
                ATTA.FechaActaConformidad = DateTime.Now;
                ATTA.UserId = UserId;
                ATTA.Indicador = 1;
                dbc.ATTACHEDs.Add(ATTA);
                dbc.SaveChanges();

                //Generando archivo
                var ruta = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 1054 && x.ID_ACCO == 4 && x.VIG_ACCO_PARA == true).Single();
                string rutaArchivo = ruta.VAL_ACCO_PARA;

                ServerReport sr = new ServerReport();
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();

                sr.ReportServerCredentials = new CredencialesReporting(reportServerUser, reportServerPass, "");
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/Proyectos/ReporteActa";


                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("IdDocuSale", Convert.ToString(IdDocuSale));

                sr.SetParameters(param);
                sr.Refresh();

                //Crea el Excel.
                bytes = sr.Render("PDF", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = rutaArchivo + nombreArchivo + "_" + Convert.ToString(ATTA.ID_ATTA) + extension;
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
                return filename;
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        #endregion

        #region Renovacion Email
        public ActionResult RegistroProductoEmail(FormCollection collection)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            int count = collection.AllKeys.Length;
            dbc.EliminarProductoDetalle(Convert.ToInt32(Request.Params["IdDocuSale"]));
            for (int i = 0; i < count; i++)
            {
                if (collection.AllKeys.Contains("chkProducto" + i) && collection.AllKeys.Contains("producto" + i))
                {
                    if (Request.Params["chkProducto" + i] == "on")
                    {
                        dbc.CreaProductoDetalle(Convert.ToInt32(Request.Params["IdDocuSale"]), UserId, Request.Params["Producto" + i]);
                    }
                }
            }
            if (Request.Params["chkGobierno"] == "on")
                dbc.ActualizarEmailRenovacion(Convert.ToInt32(Request.Params["IdDocuSale"]));
            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.mensajeProducto) top.mensajeProducto('OK','" +
                            "Se han registrado los productos.');}window.onload=init;</script>");
        }
        #endregion

        #region OP SLA
        public ActionResult ListarCboOP()
        {
            int id_comp = 0;
            string txt = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                string valorQueryString = Request.QueryString["filter[filters][0][value]"];
                id_comp = string.IsNullOrEmpty(valorQueryString) ? 0 : Convert.ToInt32(valorQueryString);
            }
            else if (NAM_PAR == "NUM_DOCU_SALE")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                id_comp = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            var result = dbc.ListarCboOP(txt, id_comp).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCboSLAxOP(int id = 0, int idComp = 0, string subCuenta = "")
        {
            int idDocuSale = 0;
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_DOCU_SALE")
            {
                idDocuSale = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "NUM_DOCU_SALE")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }

            var result = dbc.ListarCboSLAxOP(id, idAcco, idComp, subCuenta).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region TABLEROS
        [Authorize]
        public ActionResult ConsolidadoClientes()
        {
            return View();
        }

        public ActionResult ActualizarOP(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ListarConsolidadoClientes()
        {
            int idComp = 0, idCompEnd = 0, idStatDocuSale = 0, vigencia = 0;
            string numDocuSale = "";
            DateTime fechaIni = Convert.ToDateTime(Request.Params["FechaIni"]), fechaFin = Convert.ToDateTime(Request.Params["FechaFin"]);
            if (Request.Params["IdComp"] != "" && Request.Params["IdComp"] != null)
                idComp = Convert.ToInt32(Request.Params["IdComp"]);
            if (Request.Params["IdCompEnd"] != "" && Request.Params["IdCompEnd"] != null)
                idCompEnd = Convert.ToInt32(Request.Params["IdCompEnd"]);
            if (Request.Params["IdStatDocuSale"] != "" && Request.Params["IdStatDocuSale"] != null)
                idStatDocuSale = Convert.ToInt32(Request.Params["IdStatDocuSale"]);
            if (Request.Params["Vigencia"] != "" && Request.Params["Vigencia"] != null)
                vigencia = Convert.ToInt32(Request.Params["Vigencia"]);
            if (Request.Params["NumDocuSale"] != null || Request.Params["NumDocuSale"] != "")
                numDocuSale = Request.Params["NumDocuSale"];
            var result = dbc.ListarConsolidadoClientes(idComp, idCompEnd, idStatDocuSale, vigencia, numDocuSale, fechaIni, fechaFin).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCboActualizacionOP()
        {
            //Agregar filtrado de servidor
            string parm = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            string valor = "";
            string tabla = Request.Params["Tabla"];
            if (parm == "Nombre")
            {
                valor = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }

            var result = dbc.ListarCboActualizacionOP(valor, tabla).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActualizarProyecto(FormCollection collection)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            int idDocuSale = Convert.ToInt32(Request.Params["IdDocuSale"]);
            int idProducto = 0, idDescProd = 0, idTipoServicio = 0, idFrecuenciaFab = 0,
                idTipo = 0, idProveedor = 0, idFrecuenciaED = 0;
            try
            {
                string oc = Request.Params["txtOC"];
                //Validaciones
                if (Request.Params["cboProducto"] != "")
                    idProducto = Convert.ToInt32(Request.Params["cboProducto"]);
                if (Request.Params["cboDescripcionProd"] != "")
                    idDescProd = Convert.ToInt32(Request.Params["cboDescripcionProd"]);
                if (Request.Params["cboTipoServicio"] != "")
                    idTipoServicio = Convert.ToInt32(Request.Params["cboTipoServicio"]);
                if (Request.Params["cboFrecuenciaFab"] != "")
                    idFrecuenciaFab = Convert.ToInt32(Request.Params["cboFrecuenciaFab"]);
                if (Request.Params["cboTipo"] != "")
                    idTipo = Convert.ToInt32(Request.Params["cboTipo"]);
                if (Request.Params["cboProveedor"] != "")
                    idProveedor = Convert.ToInt32(Request.Params["cboProveedor"]);
                if (Request.Params["cboFrecuenciaED"] != "")
                    idFrecuenciaED = Convert.ToInt32(Request.Params["cboFrecuenciaED"]);
                //Trae los datos de la OP

                //Modifica los datos del Proyecto
                DOCUMENT_SALE ds = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == idDocuSale);
                ds.OC = oc;
                if (idProducto != 0)
                    ds.IdProducto = idProducto;
                if (idDescProd != 0)
                    ds.IdDescripcionProd = idDescProd;
                if (idTipoServicio != 0)
                    ds.IdTipoServicio = idTipoServicio;
                if (idFrecuenciaFab != 0)
                    ds.IdFrecuenciaFAB = idFrecuenciaFab;
                if (idTipo != 0)
                    ds.IdTipo = idTipo;
                if (idProveedor != 0)
                    ds.IdProveedor = idProveedor;
                if (idFrecuenciaED != 0)
                    ds.IdFrecuenciaED = idFrecuenciaED;
                dbc.Entry(ds).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.Mensaje) top.Mensaje('OK','Se ha actualizado la OP " + ds.NUM_DOCU_SALE + ".');}window.onload=init;</script>");
            }
            catch (Exception e)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.Mensaje) top.Mensaje('ERROR','Ha ocurrido un error, contacte al administrador.');}window.onload=init;</script>");
            }

        }

        public ActionResult ListarDesplegables()
        {
            string tabla = Request.Params["Tabla"];
            var result = dbc.ListarTbDesplegables("", tabla).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuardarMantenimiento()
        {
            //Variables
            string tabla, nombre, estado;
            int IdUser = 0;
            int iEstado = 0;

            try
            {
                // Validaciones 
                tabla = Request.Params["tabla"];
                estado = Request.Params["estado"];
                IdUser = Convert.ToInt32(Session["UserId"].ToString());
                //Validar que se ingrese el valor del campo
                if (Request.Params["nombre"] == "" || Request.Params["nombre"] == null)
                    return Content("VALOR");
                else
                    nombre = Request.Params["nombre"];
                // Validacion que no exista el mantenimiento
                var existe = dbc.ValidarMantenimientoDesplegable(0, tabla, nombre, "I").Single().Existe;
                if (existe == 1)
                {
                    return Content("EXISTE");
                }
                if (estado == "true")
                    iEstado = 1;

                var query = dbc.RegistrarMantenimientoDesplegable(tabla, nombre, iEstado, IdUser);

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult ActualizarMantenimiento()
        {
            //Variables
            string tabla, nombre, estado;
            int IdUser = 0;
            int iEstado = 0;
            int id = 0;

            try
            {
                // Validaciones 
                id = Convert.ToInt32(Request.Params["id"]);
                tabla = Request.Params["tabla"];
                estado = Request.Params["estado"];
                IdUser = Convert.ToInt32(Session["UserId"].ToString());
                //Validar que se ingrese el valor del campo
                if (Request.Params["nombre"] == "" || Request.Params["nombre"] == null)
                    return Content("VALOR");
                else
                    nombre = Request.Params["nombre"];
                // Validacion que no exista el mantenimiento
                var existe = dbc.ValidarMantenimientoDesplegable(id, tabla, nombre, "U").Single().Existe;
                if (existe == 1)
                {
                    return Content("EXISTE");
                }
                if (estado == "true")
                    iEstado = 1;

                var query = dbc.EditarMantenimientoDesplegable(id, tabla, nombre, iEstado, IdUser);

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }
        #endregion

        public decimal ObtenerHorasConsumidasTicket(int id = 0)
        {
            try {
                var resultado = dbc.ObtenerHorasConsumidasTicket(id).FirstOrDefault();
                decimal cantidadRestante;

                if (resultado != null)
                {
                    if (resultado.BolsaHoras != 0)
                    {
                        cantidadRestante = Convert.ToDecimal(resultado.HorasRestantes);
                    }
                    else
                    {
                        cantidadRestante = -9999;
                    }
                }
                else
                {
                    cantidadRestante = -9999;
                }

                return cantidadRestante;
            }
            catch(Exception ex)
            {
                return -9999;
            }

        }

        //ORDEN COMPRA
        [Authorize]
        public ActionResult OrdenCompra()
        {

            string bOP = "";

            if (Convert.ToString(Request.Params["OP"]) == null)
            { bOP = ""; }
            else {

            int id_docu_sale = Convert.ToInt32(Request.Params["OP"]);

            bOP = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == id_docu_sale).Single().NUM_DOCU_SALE.Replace(" ", "");
             
            };

            ViewBag.NumDocuSale = bOP;

            return View();
        }

        public ActionResult ListarOC(string bOP = "", string TipoOC = "", string fechaInicio = "", string fechaFin = "")
        {
            //string bOP = "",TipoOC = "", fechaInicio = "", fechaFin = "";

            if (Convert.ToString(Request.Params["OP"]) == null)
            { bOP = ""; }
            else { bOP = Convert.ToString(Request.Params["OP"]); };

            if (Convert.ToString(Request.Params["Tipo"]) == null)
            { TipoOC = ""; }
            else { TipoOC = Convert.ToString(Request.Params["Tipo"]); };

            if (Convert.ToString(Request.Params["FIni"]) == null)
            { fechaInicio = ""; }
            else { fechaInicio = Convert.ToString(Request.Params["FIni"]); };

            if (Convert.ToString(Request.Params["FFin"]) == null)
            { fechaFin = ""; }
            else { fechaFin = Convert.ToString(Request.Params["FFin"]); };

            //fechaInicio = Convert.ToString(Request.Params["FIni"]);
            //fechaFin = Convert.ToString(Request.Params["FFin"]);

            var query = dbc.ListaOrdenCompra(TipoOC, bOP, fechaInicio, fechaFin).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult VerOrdenCompraPdf()
        {
            try
            {
                string OP = Convert.ToString(Request.Params["OP"]);
                string Tipo = Convert.ToString(Request.Params["Tipo"]);
                string NumDoc = Convert.ToString(Request.Params["NumDoc"]);


                ViewBag.OP = OP;
                ViewBag.Tipo = Tipo;
                ViewBag.NumDoc = NumDoc;

                return View();


            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }

        }

        [HttpPost]
        public ActionResult VerPdf(string tip, string proyecto, string numdocu)
        {

            var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
            var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
            var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
            IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");

            // Configura el informe
            var rpOrdenCompraPdf = new ReportViewer();
            rpOrdenCompraPdf.ServerReport.ReportServerCredentials = rvc;
            rpOrdenCompraPdf.ProcessingMode = ProcessingMode.Remote;
            rpOrdenCompraPdf.ServerReport.ReportServerUrl = new Uri(reportServer);
            rpOrdenCompraPdf.ServerReport.ReportPath = "/OrdenCompraPdf";
            rpOrdenCompraPdf.ShowPrintButton = true;
            rpOrdenCompraPdf.ShowParameterPrompts = false;

            // Define parámetros
            ReportParameter[] param = new ReportParameter[3];
            param[0] = new ReportParameter("TipoOC", tip.ToString());
            param[1] = new ReportParameter("NUMCOD", numdocu.ToString());
            param[2] = new ReportParameter("Proyecto", proyecto.ToString());

            rpOrdenCompraPdf.ServerReport.SetParameters(param);
            rpOrdenCompraPdf.ServerReport.Refresh();

            byte[] pdfBytes = rpOrdenCompraPdf.ServerReport.Render("PDF");

            string pdfBase64 = Convert.ToBase64String(pdfBytes);

            return Content(pdfBase64, "application/pdf");


        }

        [Authorize]
        public ActionResult DetalleOC()
        {
            try
            {
                string Tipo = Convert.ToString(Request.Params["Tipo"]);
                string NumDoc = Convert.ToString(Request.Params["NumDoc"]);
                ViewBag.Tipo = Tipo;
                ViewBag.NumDoc = NumDoc;
            }
            catch
            {
                return Content("Por favor inicie sesión.");
            }
            return View();
        }
        public ActionResult ListarDetalleOC(string TipoOC, string NumDocOC)
        {
            string Tipo = "", NumDoc = "";

            if (TipoOC == null)
            { Tipo = ""; }
            else { Tipo = TipoOC; };

            if (NumDocOC == null)
            { NumDoc = ""; }
            else { NumDoc = NumDocOC; };

            var query = dbc.OrdenCompraDetalleITMS(TipoOC, NumDocOC).ToList();

            //var data = query.Select(item => item.ToString()).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCboOPEnFindTicket()
        {
            int id_comp = 0;
            string txt = "";
            string cargarOPCerradas = Convert.ToString(Request.QueryString["valOpCerrada"]);
            string cargarValOpInicio = Convert.ToString(Request.QueryString["valOpInicio"]);
            string cargarValOpFin = Convert.ToString(Request.QueryString["valOpFin"]);

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                string valorQueryString = Request.QueryString["filter[filters][0][value]"];
                id_comp = string.IsNullOrEmpty(valorQueryString) ? 0 : Convert.ToInt32(valorQueryString);
            }
            else if (NAM_PAR == "NUM_DOCU_SALE")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                try
                {
                    id_comp = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
                }
                catch (Exception ex)
                {
                }
            }
            var result = dbc.ListarCboOPEnFindTicket(txt, id_comp, cargarOPCerradas, cargarValOpInicio, cargarValOpFin).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
    }
}
