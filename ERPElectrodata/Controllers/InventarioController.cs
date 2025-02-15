using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Data.Entity;
using System.Globalization;
using System.Threading;

namespace ERPElectrodata.Controllers
{
    public class InventarioController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        private TextInfo textInfo;
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        public int ctd = 1;
        //
        // GET: /Inventario/
        [Authorize]
        public ActionResult Index()
        {
            if ((int)Session["SUPERVISOR_INVENTARIO"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }

        [Authorize]
        public ActionResult Ticket()
        {
            if ((int)Session["SUPERVISOR_INVENTARIO"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }

        [Authorize]
        public ActionResult TicketDetalle(int id = 0)
        {
            if ((int)Session["SUPERVISOR_INVENTARIO"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            var detTicket = new DETAILS_TICKET();

            //string COD_TICK = Convert.ToString(dbc.TICKETs.Single(x => x.ID_TICK == id).COD_TICK);
            //string ID_TYPE_FORM = Convert.ToString(dbc.TICKETs.Single(x => x.ID_TICK == id).ID_TYPE_FORM);
            var query = (from x in dbc.TYPE_FORMAT
                         join t in dbc.TICKETs on x.ID_TYPE_FORM equals t.ID_TYPE_FORM
                         select new
                         {
                             x.NAM_TYPE_FORM,
                             t.ID_TICK,
                             t.COD_TICK,
                             t.ID_STAT_END,
                         }).Where(x => x.ID_TICK == id).First();

            ViewData["COD_TICK"] = query.COD_TICK;
            ViewData["ID_TICK"] = id;
            ViewData["NAM_TYPE_FORM"] = query.NAM_TYPE_FORM;
            detTicket.ID_TICK = id;
            ViewData["ID_STAT_END"] = query.ID_STAT_END;

            return View(detTicket);
        }

        [Authorize]
        public ActionResult Detalle(int id = 0)
        {
            if ((int)Session["SUPERVISOR_INVENTARIO"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            ViewBag.IdInventario = id;
            return View();
        }

        [Authorize]
        public ActionResult Buscar()
        {
            return View();
        }

        [Authorize]
        public ActionResult Crear()
        {
            if ((int)Session["SUPERVISOR_INVENTARIO"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }

        [Authorize]
        public ActionResult Entrega()
        {
            if ((int)Session["SUPERVISOR_INVENTARIO"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }

        [Authorize]
        public ActionResult Recepcion()
        {
            if ((int)Session["SUPERVISOR_INVENTARIO"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }

        [Authorize]
        public ActionResult Reporte()
        {
            if ((int)Session["SUPERVISOR_INVENTARIO"] == 0)
            {
                return Content("Necesitas permisos para acceder a esta sección.");
            }
            return View();
        }

        [Authorize]
        public ActionResult Grafico()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearArticulo(Inventario objInventario)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            int IdTipoInventario = Convert.ToInt32(Request.Params["IdTipoInventario"].ToString());

            objInventario.IdAcco = ID_ACCO;
            objInventario.UserIdCreacion = UserId;
            objInventario.IdTipoInventario = IdTipoInventario;
            objInventario.FechaCreacion = DateTime.Now;
            objInventario.Condicion = 0;
            objInventario.Estado = true;

            dbe.Inventarios.Add(objInventario);
            dbe.SaveChanges();

            //Registrando y asociado el activo al nuevo usuario
            var nuevoInvAsignado = new InventarioAsignado();
            nuevoInvAsignado.IdArea = 0;
            nuevoInvAsignado.Idinventario = objInventario.Id;
            nuevoInvAsignado.IdEstado = 0;
            nuevoInvAsignado.IdPersEnti = 1007;
            nuevoInvAsignado.FechaInicio = DateTime.Now;
            nuevoInvAsignado.FechaCreacion = DateTime.Now;
            nuevoInvAsignado.UserIdCreacion = UserId;
            nuevoInvAsignado.IdLocacion = 0;
            nuevoInvAsignado.IdTipoRegistro = 1;

            dbe.InventarioAsignadoes.Add(nuevoInvAsignado);
            dbe.SaveChanges();

            return Content("<script type='text/javascript'>  function init() {" +
                                       "if(top.uploadDone) top.uploadDone('OK');}window.onload=init;</script>");
        }

        public ActionResult ListarTipoInventario()
        {
            var result = (from x in dbe.InvTipoInventarios.Where(x => x.Estado == true)
                          select new
                          {
                              x.Id,
                              x.Nombre
                          }).OrderBy(x => x.Nombre);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Listar(int IdTipo, int IdArea, int IdUsuario, string PalabraClave)
        {
            //List<InvListarActivos_Result> ListaActivo = dbe.InvListarActivos(1,1,1,"").ToList();
            //int ID_STAT_INVENTARIO = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            List<InvListarActivos_Result> ListaActivo = dbe.InvListarActivos(IdTipo, IdArea, IdUsuario, PalabraClave).ToList();

            return Json(new { Grid = ListaActivo, Cantidad = ListaActivo.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarxTextoEntrega()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            if (txt == null) { txt = ""; }

            List<InvListaActivoEntrega_Result> ListaActivoEntrega = dbe.InvListaActivoEntrega(0, txt).ToList();

            return Json(new { Grid = ListaActivoEntrega, Cantidad = ListaActivoEntrega.Count() }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearEntrega(IEnumerable<HttpPostedFileBase> files, TICKET ticket)
        {
            //Validando si los Asset siguen siendo UNASSIGNED
            string idsActivo = Convert.ToString(Request.Form["idsActivo"]);

            if (idsActivo.Length > 0)
            {
                idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);
                string[] ids_asset = idsActivo.Split(',');
                string msn = "";

                var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                var ID_USER = Convert.ToInt32(Session["UserId"]);

                int ID_PER_ENTI, ID_PRIO, ID_LOCA, ID_AREA, ID_SOUR, HOUR;
                DateTime FEC_TICK;

                try
                {
                    ID_PER_ENTI = ticket.ID_PERS_ENTI.Value;
                    FEC_TICK = ticket.FEC_TICK.Value;
                    ID_AREA = ticket.ID_AREA.Value;
                    ID_LOCA = ticket.ID_LOCA.Value;
                    ID_SOUR = 3;//Por defecto
                    ID_PRIO = 5;//Planning
                    HOUR = Convert.ToInt32(dbc.PRIORITies.First(x => x.ID_PRIO == ID_PRIO).HOU_PRIO.Value);
                }
                catch
                {   //Falta informacion
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('" + msn + "','0');}window.onload=init;</script>");
                }

                string Code = null;
                try
                {
                    ticket.ID_ACCO = ID_ACCO;
                    ticket.ID_TYPE_TICK = 2;
                    ticket.ID_PERS_ENTI = ID_PER_ENTI;
                    ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
                    ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
                    ticket.ID_AREA = ID_AREA;
                    ticket.ID_QUEU = ID_QUEU;
                    ticket.ID_PRIO = 5;
                    ticket.ID_STAT = 1;
                    ticket.ID_STAT_END = 1;
                    ticket.ID_SOUR = ID_SOUR;
                    ticket.FEC_TICK = FEC_TICK;
                    //ticket.SUM_TICK = ticket.SUM_TICK;
                    ticket.REM_CTRL_TICK = false;
                    ticket.UserId = ID_USER;
                    ticket.CREATE_TICK = DateTime.Now;
                    ticket.FEC_INI_TICK = DateTime.Now;
                    ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(HOUR);
                    ticket.IS_PARENT = false;
                    ticket.ID_TYPE_FORM = 4;
                    ticket.ID_CATE = 181;
                    ticket.SEND_MAIL = false;

                    dbc.TICKETs.Add(ticket);
                    dbc.SaveChanges();

                    int id = Convert.ToInt32(ticket.ID_TICK);

                    dbc.Entry(ticket).State = EntityState.Detached;

                    Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

                    //insertando ahora el detalle
                    foreach (string dtide in ids_asset)
                    {
                        try
                        {
                            int ide = Convert.ToInt32(dtide);

                            //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
                            var query = dbe.InventarioAsignadoes.Where(x => x.Idinventario == ide && x.FechaFin == null);
                            if (query.Count() == 1)
                            {
                                var invAsignado = dbe.InventarioAsignadoes.Single(x => x.Idinventario == ide && x.FechaFin == null);
                                invAsignado.FechaFin = DateTime.Now;
                                //dbe.InventarioAsignadoes.Attach(invAsignado);
                                dbe.Entry(invAsignado).State = EntityState.Modified;
                                dbe.SaveChanges();
                            }

                            //Registrando nuevo registro del activo
                            var invTicket = new InventarioTicket();
                            invTicket.IdInventario = ide;
                            invTicket.IdTick = id;
                            invTicket.FechaInicio = DateTime.Now;

                            dbe.InventarioTickets.Add(invTicket);
                            dbc.SaveChanges();

                            //Actualizando el estado final de la fecha final del Client_Asset
                            var invAsigna = dbe.InventarioAsignadoes.Where(x => x.Idinventario == ide && x.FechaFin == null);

                            if (invAsigna.Count() > 0)
                            {
                                var invAsig = invAsigna.Single();
                                invAsig.FechaFin = DateTime.Now;
                                //dbe.InventarioAsignadoes.Attach(invAsig);
                                dbe.Entry(invAsig).State = EntityState.Modified;
                                dbe.SaveChanges();
                            }

                            //Registrando y asociado el activo al nuevo usuario
                            var nuevoInvAsignado = new InventarioAsignado();
                            nuevoInvAsignado.IdArea = ID_AREA;
                            nuevoInvAsignado.Idinventario = ide;
                            nuevoInvAsignado.IdEstado = 1;
                            nuevoInvAsignado.IdPersEnti = ID_PER_ENTI;
                            nuevoInvAsignado.FechaInicio = DateTime.Now;
                            nuevoInvAsignado.FechaCreacion = DateTime.Now;
                            nuevoInvAsignado.UserIdCreacion = ID_USER;
                            nuevoInvAsignado.IdLocacion = ID_LOCA;
                            nuevoInvAsignado.IdTipoRegistro = 1;

                            dbe.InventarioAsignadoes.Add(nuevoInvAsignado);
                            dbe.SaveChanges();
                        }
                        catch
                        {   //Falla al insertar en base de datos
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
                        }
                    }
                }
                catch
                {   //Falla al insertar en base de datos
                    //var errors = ModelState.Select(x => x.Value.Errors).ToList();                        
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
                }
                //Se registro correctamente
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + Code + "','" + ticket.ID_TICK.ToString() + "');}window.onload=init;</script>");

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
            }
        }

        public ActionResult ListarxTextoRecepcion()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            int IdPersEnti = Convert.ToInt32(Request.Params["IdPersEnti"]);

            if (txt == null) { txt = ""; }

            List<InvListaActivoEntrega_Result> ListaActivoEntrega = dbe.InvListaActivoEntrega(IdPersEnti, txt).ToList();

            return Json(new { Grid = ListaActivoEntrega, Cantidad = ListaActivoEntrega.Count() }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearRecepcion(IEnumerable<HttpPostedFileBase> files, TICKET ticket)
        {
            //Validando si los Asset siguen siendo UNASSIGNED
            string idsActivo = Convert.ToString(Request.Form["idsActivo"]);

            if (idsActivo.Length > 0)
            {
                idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);
                string[] ids_asset = idsActivo.Split(',');
                string msn = "";


                var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                var ID_USER = Convert.ToInt32(Session["UserId"]);

                int ID_PER_ENTI, ID_PRIO, ID_LOCA, ID_AREA, ID_SOUR, HOUR;
                DateTime FEC_TICK;

                try
                {
                    ID_PER_ENTI = ticket.ID_PERS_ENTI.Value;
                    FEC_TICK = ticket.FEC_TICK.Value;
                    ID_AREA = ticket.ID_AREA.Value;
                    ID_LOCA = ticket.ID_LOCA.Value;
                    ID_SOUR = 3;//Por defecto
                    ID_PRIO = 5;//Planning
                    HOUR = Convert.ToInt32(dbc.PRIORITies.First(x => x.ID_PRIO == ID_PRIO).HOU_PRIO.Value);
                }
                catch
                {   //Falta informacion
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('" + msn + "','0');}window.onload=init;</script>");
                }


                string Code = null;
                try
                {
                    ticket.ID_ACCO = ID_ACCO;
                    ticket.ID_TYPE_TICK = 2;
                    ticket.ID_PERS_ENTI = ID_PER_ENTI;
                    ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
                    ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
                    ticket.ID_AREA = ID_AREA;
                    ticket.ID_QUEU = ID_QUEU;
                    ticket.ID_PRIO = 5;
                    ticket.ID_STAT = 1;
                    ticket.ID_STAT_END = 1;
                    ticket.ID_SOUR = ID_SOUR;
                    ticket.FEC_TICK = FEC_TICK;
                    //ticket.SUM_TICK = ticket.SUM_TICK;
                    ticket.REM_CTRL_TICK = false;
                    ticket.UserId = ID_USER;
                    ticket.CREATE_TICK = DateTime.Now;
                    ticket.FEC_INI_TICK = DateTime.Now;
                    ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(HOUR);
                    ticket.IS_PARENT = false;
                    ticket.ID_TYPE_FORM = 5;
                    ticket.ID_CATE = 181;
                    ticket.SEND_MAIL = false;

                    dbc.TICKETs.Add(ticket);
                    dbc.SaveChanges();

                    int id = Convert.ToInt32(ticket.ID_TICK);

                    dbc.Entry(ticket).State = EntityState.Detached;

                    Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

                    //insertando ahora el detalle
                    foreach (string dtide in ids_asset)
                    {
                        try
                        {
                            int ide = Convert.ToInt32(dtide);

                            //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
                            var query = dbe.InventarioAsignadoes.Where(x => x.Idinventario == ide && x.FechaFin == null);
                            if (query.Count() == 1)
                            {
                                var invAsignado = dbe.InventarioAsignadoes.Single(x => x.Idinventario == ide && x.FechaFin == null);
                                invAsignado.FechaFin = DateTime.Now;
                                //dbe.InventarioAsignadoes.Attach(invAsignado);
                                dbe.Entry(invAsignado).State = EntityState.Modified;
                                dbe.SaveChanges();
                            }

                            //Registrando nuevo registro del activo
                            var invTicket = new InventarioTicket();
                            invTicket.IdInventario = ide;
                            invTicket.IdTick = id;
                            invTicket.FechaInicio = DateTime.Now;

                            dbe.InventarioTickets.Add(invTicket);
                            dbc.SaveChanges();

                            //Actualizando el estado final de la fecha final del Client_Asset
                            var invAsigna = dbe.InventarioAsignadoes.Where(x => x.Idinventario == ide && x.FechaFin == null);

                            if (invAsigna.Count() > 0)
                            {
                                var invAsig = invAsigna.Single();
                                invAsig.FechaFin = DateTime.Now;
                                //dbe.InventarioAsignadoes.Attach(invAsig);
                                dbe.Entry(invAsig).State = EntityState.Modified;
                                dbe.SaveChanges();
                            }

                            //Registrando y asociado el activo al nuevo usuario
                            var nuevoInvAsignado = new InventarioAsignado();
                            nuevoInvAsignado.IdArea = ID_AREA;
                            nuevoInvAsignado.Idinventario = ide;
                            nuevoInvAsignado.IdEstado = 0;
                            nuevoInvAsignado.IdPersEnti = 1007; //System Manager
                            nuevoInvAsignado.FechaInicio = DateTime.Now;
                            nuevoInvAsignado.FechaCreacion = DateTime.Now;
                            nuevoInvAsignado.UserIdCreacion = ID_USER;
                            nuevoInvAsignado.IdLocacion = ID_LOCA;
                            nuevoInvAsignado.IdTipoRegistro = 1;

                            dbe.InventarioAsignadoes.Add(nuevoInvAsignado);
                            dbe.SaveChanges();
                        }
                        catch
                        {   //Falla al insertar en base de datos
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
                        }
                    }
                }
                catch
                {   //Falla al insertar en base de datos
                    //var errors = ModelState.Select(x => x.Value.Errors).ToList();                        
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
                }
                //Se registro correctamente
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + Code + "','" + ticket.ID_TICK.ToString() + "');}window.onload=init;</script>");

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
            }
        }

        #region "Tickets"

        // GET: /Ticket/ListByStatus
        public ActionResult ListByStatusTicket(int id = 0)
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
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME,
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
            var tick = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO);

            //Diferenciando activos - soporte
            tick = tick.Where(x => x.ID_TYPE_FORM == 4 || x.ID_TYPE_FORM == 5);

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
            }

            if (ID_STAT == 0)
            {
                tick = tick.Where(t => (t.ID_STAT_END == 1 || t.ID_STAT_END == 3) && t.ID_TYPE_FORM == 4);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.IS_ROLE_USER).ThenBy(t => t.ID_PRIO).ThenByDescending(t => t.CREATE_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 1)
            {
                tick = tick.Where(i => (i.ID_STAT_END == 4 || i.ID_STAT_END == 6) && i.ID_TYPE_FORM == 4);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 2)
            {
                tick = tick.Where(i => (i.ID_STAT_END == 1 || i.ID_STAT_END == 3) && i.ID_TYPE_FORM == 5);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 3)
            {
                tick = tick.Where(i => (i.ID_STAT_END == 4 || i.ID_STAT_END == 6) && i.ID_TYPE_FORM == 5);
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
                              SUM_INCI_PLAIN = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBCLAS = c4.NAM_CATE.ToLower(),
                              NAM_CLAS = c3.NAM_CATE.ToLower(),
                              NAM_SUBC = c2.NAM_CATE.ToLower(),
                              NAM_CATE = c1.ABR_CATE,
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              URL = "/Inventario/TicketDetalle/",
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
                              EXP_TIME = "Planning",
                              PARENT = (i.IS_PARENT == true ? "expand" : "none"),
                              COUNTSON = 0,
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
                              DATE_MAX = "",
                              DATE_SCHE = "",
                              HOUR_SCHE = ""
                          });

            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateBarStatus(int id = 0)
        {
            var TipoFormato = dbc.TYPE_FORMAT.Where(x => x.ID_TYPE_FORM == 4 || x.ID_TYPE_FORM == 5);

            var result = (from tf in TipoFormato.ToList()
                          select new
                          {
                              tf.ID_TYPE_FORM,
                              tf.NAM_TYPE_FORM,
                              TicketsP = TicketsByStatusx(tf.ID_TYPE_FORM, 0),
                              TotalP = TotTicketsByStatusx(tf.ID_TYPE_FORM, 0),
                              TicketsC = TicketsByStatusx(tf.ID_TYPE_FORM, 1),
                              TotalC = TotTicketsByStatusx(tf.ID_TYPE_FORM, 1)
                          }).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public int TicketsByStatusx(int tipoFormato, int estado)
        {
            int[] numbers = new int[0];
            int iq = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            bool VIS_ALL_QUEU = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

            var cant = dbc.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_TYPE_FORM == tipoFormato);

            //Pendientes
            if (estado == 0)
            {
                cant = cant.Where(x => x.ID_STAT_END == 1 || x.ID_STAT_END == 3);
            }
            //Cerrados
            if (estado == 1)
            {
                cant = cant.Where(x => x.ID_STAT_END == 4 || x.ID_STAT_END == 6);
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
                    cant = cant.Where(i => i.ID_PERS_ENTI_ASSI == ID_ASSI);
                    //tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                }
                else
                {
                    //Mostrar el ticket cuya cola es la indicada
                    cant = cant.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                }

            }

            int cantidad = cant.Count();

            return cant.Count();
        }

        public int TotTicketsByStatusx(int tipoFormato, int estado)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var cant = dbc.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_TYPE_FORM == tipoFormato);

            if (estado == 0)
            {
                cant = cant.Where(x => x.ID_STAT_END == 1 || x.ID_STAT_END == 3);
            }
            if (estado == 1)
            {
                cant = cant.Where(x => x.ID_STAT_END == 4 || x.ID_STAT_END == 6);
            }

            return cant.Count();
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

        public int contador()
        {
            return ctd++;
        }

        #endregion

        #region "Detalle Ticket"

        public ActionResult ListAssetByID_TICK(int id = 0)
        {
            var query1 = dbe.InventarioTickets.Where(dtf => dtf.IdTick == id).ToList();

            var result = (from t in query1.ToList()
                          join a in dbe.Inventarios on t.IdInventario equals a.Id
                          join ta in dbe.InvTipoInventarios on a.IdTipoInventario equals ta.Id
                          select new
                          {
                              ID_TICK = t.IdTick,
                              NAM_TYPE_ASSE = (ta.Nombre == null ? string.Empty : ta.Nombre),
                              CODE = (a.Codigo == null ? string.Empty : a.Codigo),
                              NAM_EQUI = (a.Descripcion == null ? string.Empty : a.Descripcion),
                              SER_NUMB = (a.Serie == null ? string.Empty : a.Serie),
                              NAM_COMM_MODE = (a.Modelo == null ? string.Empty : a.Modelo),
                              NAM_MANU = (a.Marca == null ? string.Empty : a.Marca),
                          });
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Detalle Activo"

        public ActionResult Activo(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            List<InvActivo_Result> result = dbe.InvActivo(id).ToList();

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InventarioTicket(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            List<InvListarTicket_Result> result = dbe.InvListarTicket(id).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InventarioDetalleTicket(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            List<InvListarTicketDetalle_Result> result = dbe.InvListarTicketDetalle(id).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
