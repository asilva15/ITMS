using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPElectrodata.Controllers
{
    public class ServiceNowController : Controller
    {
        public report_hudbayEntities dbhc = new report_hudbayEntities();
        //
        // GET: /ServiceNow/
        #region LookerStudio
        public ActionResult LookerStudio()
        {
            return View();
        }
        public ActionResult AgregarLookerStudio()
        {
            TablaLookerStudio lookerStudio = new TablaLookerStudio();
            ViewBag.Abierto = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
            return View(lookerStudio);
        }

        [HttpPost]
        public ActionResult AgregarLookerStudio(TablaLookerStudio lookerStudio)
        {
            CultureInfo culture = new CultureInfo("en-US");
            lookerStudio.created_at = DateTime.Now;
            lookerStudio.NumMes = lookerStudio.Abierto.Value.Month.ToString();
            lookerStudio.Mes = culture.DateTimeFormat.GetMonthName(lookerStudio.Abierto.Value.Month);
            if (lookerStudio.ResolucionHrs == "" || lookerStudio.ResolucionHrs == null)
            {
                lookerStudio.ResolucionHrs = "00:00";
                lookerStudio.TiempoResolucion = 0;
            }
            else
            {
                lookerStudio.TiempoResolucion = Convert.ToInt32(HorasANumero(lookerStudio.ResolucionHrs) * 3600);
            }
            lookerStudio.Tipo = ObtenerPrimeraPalabra(lookerStudio.BreveDescripcion);

            //Nuevos valores en blanco
            lookerStudio.MotivoPonerEspera = "";
            //lookerStudio.FechaVencimiento = null;
            lookerStudio.Actualizado = lookerStudio.Abierto;
            lookerStudio.AbiertoPor = null;
            lookerStudio.CerradoPor = lookerStudio.Solicitante;
            lookerStudio.ActualizadoPor = lookerStudio.Solicitante;
            dbhc.TablaLookerStudio.Add(lookerStudio);
            dbhc.SaveChanges();
            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }
        public ActionResult EditarLookerStudio(int id)
        {
            TablaLookerStudio tablaLooker = dbhc.TablaLookerStudio.Find(id);
            //Obtener id de tipo de Atencion
            //TipoAtencion tipo = dbhc.TipoAtencion.Where(x => x.TipoAtencion1 == tablaLooker.TipoAtencion).FirstOrDefault();
            //Categoria categoria = dbhc.Categoria.Where(x => x.Categoria1 == tablaLooker.Categoria).FirstOrDefault();
            //SubCategoriaH subCategoria = dbhc.SubCategoriaHs.Where(x => x.SubCategoria1 == tablaLooker.SubCategoria).FirstOrDefault();
            //Equivalencia equivalencia = dbhc.Equivalencia.Where(x => x.Nombre == tablaLooker.AtribuidoA).FirstOrDefault();
            //if(tipo != null)
            //{
            //    ViewBag.TipoAtencionID = tipo.id_TipoAtencion;
            //}
            //else
            //{
            //    ViewBag.TipoAtencionID = 0;
            //}
            //if (categoria != null)
            //{
            //    ViewBag.CategoriaID = categoria.id_Categoria;
            //}
            //else
            //{
            //    ViewBag.CategoriaID = 0;
            //}
            //if (subCategoria != null)
            //{
            //    ViewBag.SubCategoriaID = subCategoria.id_SubCategoria;
            //}
            //else
            //{
            //    ViewBag.SubCategoriaID = 0;
            //}
            //if (subCategoria != null)
            //{
            //    ViewBag.AtribuidoA = equivalencia.id_Equivalencia;
            //}
            //else
            //{
            //    ViewBag.AtribuidoA = 0;
            //}

            
            //Se debe modificar el formato de todas las fechas para que funcionen en el formulario
            //if (tablaLooker.Abierto != null)
            //{
            //    ViewBag.Abierto = ((DateTime)tablaLooker.Abierto).ToString("yyyy-MM-dd HH:mm:ss");
            //}
            //else
            //{
            //    ViewBag.Abierto = null;
            //}
            //if (tablaLooker.FechaVencimiento != null)
            //{
            //    ViewBag.FechaVencimiento = ((DateTime)tablaLooker.FechaVencimiento).ToString("yyyy-MM-dd HH:mm:ss");
            //}
            //else
            //{
            //    ViewBag.FechaVencimiento = null;
            //}
            //if (tablaLooker.Actualizado != null)
            //{
            //    ViewBag.FechaActualizado = ((DateTime)tablaLooker.Actualizado).ToString("yyyy-MM-dd HH:mm:ss");
            //}
            //else
            //{
            //    ViewBag.FechaActualizado = null;
            //}
            //Los valores de selectores deben ser agregados por Viewbag
            //if (tablaLooker.Numero != null)
            //{
            //    ViewBag.Numero = tablaLooker.Numero;
            //}
            //else
            //{
            //    ViewBag.Numero = null;
            //}
            //if (tablaLooker.Categoria != null)
            //{
            //    ViewBag.categoria = tablaLooker.Categoria;
            //}
            //else
            //{
            //    ViewBag.categoria = null;
            //}
            //if (tablaLooker.TipoAtencion != null)
            //{
            //    ViewBag.TipoAtencion = tablaLooker.TipoAtencion;
            //}
            //else
            //{
            //    ViewBag.TipoAtencion = null;
            //}

            return View(tablaLooker);
        }

        [HttpPost]
        public ActionResult EditarLookerStudio(TablaLookerStudio lookerStudio)
        {
            if (ModelState.IsValid)
            {
                TablaLookerStudio looker = dbhc.TablaLookerStudio.Find(lookerStudio.id_TablaLookerStudio);
                //looker.Numero = lookerStudio.Numero;
                looker.BreveDescripcion = lookerStudio.BreveDescripcion;
                looker.Tipo = lookerStudio.Tipo;
                //looker.AtribuidoA = lookerStudio.AtribuidoA;
                //looker.TipoAtencion = lookerStudio.TipoAtencion;
                //looker.Area = lookerStudio.Area;
                //looker.Categoria = lookerStudio.Categoria;
                //looker.SubCategoria = lookerStudio.SubCategoria;
                looker.TotalHoras_estimado = lookerStudio.TotalHoras_estimado;
                looker.TotalHoras_real = lookerStudio.TotalHoras_real;
                looker.Comentario = lookerStudio.Comentario;
                //dbhc.Entry(lookerStudio).State = System.Data.Entity.EntityState.Modified;
                dbhc.SaveChanges();
            }
            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }

        #endregion

        #region ServiceNowTickets
        public ActionResult ServiceNowTickets()
        {
            return View();
        }
        public ActionResult AgregarServiceNowTickets()
        {
            ServicesNow servicesNow = new ServicesNow();
            
            return View(servicesNow);
        }

        [HttpPost]
        public ActionResult AgregarServiceNowTickets(ServicesNow servicesNow)
        {
            dbhc.ServicesNow.Add(servicesNow);
            dbhc.SaveChanges();
            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }
        public ActionResult EditarServiceNowTickets(int id)
        {
            ServicesNow servicesNow = dbhc.ServicesNow.Find(id);
            //Se debe modificar el formato de todas las fechas para que funcionen en el formulario
            if (servicesNow.Abierto != null)
            {
                ViewBag.Abierto = ((DateTime)servicesNow.Abierto).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                ViewBag.Abierto = null;
            }
            if (servicesNow.FechaVencimiento != null)
            {
                ViewBag.FechaVencimiento = ((DateTime)servicesNow.FechaVencimiento).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                ViewBag.FechaVencimiento = null;
            }
            if (servicesNow.Actualizado != null)
            {
                ViewBag.Actualizado = ((DateTime)servicesNow.Actualizado).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                ViewBag.Actualizado = null;
            }
            return View(servicesNow);
        }

        [HttpPost]
        public ActionResult EditarServiceNowTickets(ServicesNow servicesNow)
        {
            ServicesNow servicesNowFind = dbhc.ServicesNow.Find(servicesNow.id_servicesNow);
            servicesNowFind.BreveDescripcion = servicesNow.BreveDescripcion;
            dbhc.SaveChanges();
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
        }

        
        #endregion
        #region Opciones
        public ActionResult Opciones()
        {
            return View();
        }
        #endregion
        #region Categoria
        public ActionResult VerCategoria()
        {
            return View();
        }

        public ActionResult AgregarCategoria()
        {
            Categoria categoria = new Categoria();
            return View(categoria);
        }

        [HttpPost]
        public ActionResult AgregarCategoria(Categoria categoria)
        {
            categoria.created_at = DateTime.Now;
            dbhc.Categoria.Add(categoria);
            dbhc.SaveChanges();
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK',0);}window.onload=init;</script>");
        }
        public ActionResult EditarCategoria(int id)
        {
            Categoria categoria = dbhc.Categoria.Find(id);
            return View(categoria);
        }


        [HttpPost]
        public ActionResult EditarCategoria(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                dbhc.Entry(categoria).State = System.Data.Entity.EntityState.Modified;
                dbhc.SaveChanges();
            }
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK',0);}window.onload=init;</script>");
        }

        public ActionResult ObtenerListaCategorias()
        {
            var result = (from cat in dbhc.Categoria
                          join tipo in dbhc.TipoAtencion on cat.id_TipoAtencion equals tipo.id_TipoAtencion
                          select new
                          {
                              cat.id_Categoria,
                              cat.Categoria1,
                              cat.Supervision_estimado,
                              cat.Supervision_real,
                              cat.Infraestructura_estimado,
                              cat.Infraestructura_real,
                              cat.HelpDesk_estimado,
                              cat.HelpDesk_real,
                              cat.SoporteUser_estimado,
                              cat.SoporteUser_real,
                              cat.Arquitectura_estimado,
                              cat.Arquitectura_real,
                              cat.ServicioDBA_estimado,
                              cat.ServicioDBA_real,
                              cat.TotalHoras_estimado,
                              cat.TotalHoras_real,
                              tipo.TipoAtencion1,
                              cat.created_at,
                              
                          }).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Equivalencia
        public ActionResult VerEquivalencia()
        {
            return View();
        }
        public ActionResult EditarEquivalencia(int id)
        {
            Equivalencia equivalencia = dbhc.Equivalencia.Find(id);
            return View(equivalencia);
        }
        public ActionResult AgregarEquivalencia()
        {
            Equivalencia equivalencia = new Equivalencia();
            return View(equivalencia);
        }

        [HttpPost]
        public ActionResult AgregarEquivalencia(Equivalencia equivalencia)
        {
            equivalencia.created_at = DateTime.Now;
            dbhc.Equivalencia.Add(equivalencia);
            dbhc.SaveChanges();
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK',1);}window.onload=init;</script>");
        }

        [HttpPost]
        public ActionResult EditarEquivalencia(Equivalencia equivalencia)
        {
            if (ModelState.IsValid)
            {
                dbhc.Entry(equivalencia).State = System.Data.Entity.EntityState.Modified;
                dbhc.SaveChanges();
            }
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK',1);}window.onload=init;</script>");
        }

        public ActionResult ObtenerListaEquivalencia()
        {
            var equivalencia = (from e in dbhc.Equivalencia
                                               join g in dbhc.GpoAsignacion on e.id_GpoAsignacion equals g.id_GpoAsignacion
                                               select new
                                               {
                                                   id_Equivalencia = e.id_Equivalencia,
                                                   Nombre = e.Nombre,
                                                   Area = e.Area,
                                                   correoEdata = e.correoEdata,
                                                   usuarioRed = e.usuarioRed,
                                                   correoHudbay = e.correoHudbay,
                                                   id_GpoAsignacion = e.id_GpoAsignacion,
                                                   GpoAsignacion1 = g.GpoAsignacion1,
                                                   created_at = e.created_at
                                               }).ToList();
            return Json(new
            {
                data = equivalencia
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region TipoAtencion
        public ActionResult VerTipoAtencion()
        {
            return View();
        }
        public ActionResult EditarTipoAtencion(int id)
        {
            TipoAtencion tipoAtencion = dbhc.TipoAtencion.Find(id);
            return View(tipoAtencion);
        }
        public ActionResult AgregarTipoAtencion()
        {
            TipoAtencion tipoAtencion = new TipoAtencion();
            return View(tipoAtencion);
        }

        [HttpPost]
        public ActionResult AgregarTipoAtencion(TipoAtencion tipoAtencion)
        {
            tipoAtencion.created_at = DateTime.Now;
            dbhc.TipoAtencion.Add(tipoAtencion);
            dbhc.SaveChanges();
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK',2);}window.onload=init;</script>");
        }

        [HttpPost]
        public ActionResult EditarTipoAtencion(TipoAtencion tipoAtencion)
        {
            if (ModelState.IsValid)
            {
                TipoAtencion tipo = dbhc.TipoAtencion.Find(tipoAtencion.id_TipoAtencion);
                tipo.TipoAtencion1 = tipoAtencion.TipoAtencion1;
                dbhc.SaveChanges();
            }
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK',2);}window.onload=init;</script>");
        }
        public ActionResult ObtenerListaTipoAtencion()
        {
            var tipoAtencion = dbhc.TipoAtencion.Select(x=>new { 
                x.id_TipoAtencion,
                x.TipoAtencion1,
                x.created_at
            }).ToList();
            return Json(new
            {
                data = tipoAtencion
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region GrupoAsignacion
        public ActionResult VerGrupoAsignacion()
        {
            return View();
        }
        public ActionResult EditarGrupoAsignacion(int id)
        {
            GpoAsignacion gpoAsignacion = dbhc.GpoAsignacion.Find(id);
            return View(gpoAsignacion);
        }
        public ActionResult AgregarGrupoAsignacion()
        {
            GpoAsignacion gpoAsignacion = new GpoAsignacion();
            return View(gpoAsignacion);
        }

        [HttpPost]
        public ActionResult AgregarGrupoAsignacion(GpoAsignacion gpoAsignacion)
        {
            bool existe = dbhc.GpoAsignacion.Any(g => g.GpoAsignacion1 == gpoAsignacion.GpoAsignacion1);
            if (existe)
            {
                return Content("<script type='text/javascript'>" +
               "top.warningMessage('- Ese <b>Grupo de Asignación</b> ya existe.');" +
               "</script>");

            }
            else if(gpoAsignacion.GpoAsignacion1 == null)
            {
                return Content("<script type='text/javascript'>" +
               "top.warningMessage('- El campo <b>Grupo de Asignación</b> no puede estar vacío.');" +
               "</script>");
            }
            else
            {
                gpoAsignacion.created_at = DateTime.Now;
                dbhc.GpoAsignacion.Add(gpoAsignacion);
                dbhc.SaveChanges();
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('OK',3);}window.onload=init;</script>");
            }
        }

        [HttpPost]
        public ActionResult EditarGrupoAsignacion(GpoAsignacion gpoAsignacion)
        {
            bool existe = dbhc.GpoAsignacion
                            .Any(g => g.GpoAsignacion1 == gpoAsignacion.GpoAsignacion1 && g.id_GpoAsignacion != gpoAsignacion.id_GpoAsignacion);

            if (existe)
            {
                return Content("<script type='text/javascript'>" +
               "top.warningMessage('- Ese <b>Grupo de Asignación</b> ya existe.');" +
               "</script>");

            }
            else if (gpoAsignacion.GpoAsignacion1 == null)
            {
                return Content("<script type='text/javascript'>" +
               "top.warningMessage('- El campo <b>Grupo de Asignación</b> no puede estar vacío.');" +
               "</script>");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    GpoAsignacion grupo = dbhc.GpoAsignacion.Find(gpoAsignacion.id_GpoAsignacion);
                    grupo.GpoAsignacion1 = gpoAsignacion.GpoAsignacion1;
                    dbhc.SaveChanges();
                }
                return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('OK',3);}window.onload=init;</script>");
            }
        }

        public ActionResult ObtenerListaGrupoAsignacion()
        {
            var grupoAsignacion = dbhc.GpoAsignacion.Select(x => new {
                x.id_GpoAsignacion,
                x.GpoAsignacion1,
                x.created_at
            }).ToList();
            return Json(new
            {
                data = grupoAsignacion
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        public ActionResult ObtenerTicketsLookerStudio(string fechaInicio,string fechaFin,int tipo,string atribuidoA, string tipoAtencion)
        {
            var result = (from ev in dbhc.TablaLookerStudio
                            select new
                            {
                                ev.id_TablaLookerStudio,
                                ev.GrupoDeAsignacion,
                                ev.Area,
                                ev.BreveDescripcion,
                                ev.Tipo,
                                ev.TipoAtencion,
                                ev.AtribuidoA,
                                ev.Numero,
                                ev.Prioridad,
                                ev.MotivoPonerEspera,
                                ev.Categoria,
                                ev.CreadoPor,
                                ev.Abierto,
                                ev.FechaVencimiento,
                                ev.Mes,
                                ev.NumMes,
                                ev.UnidadNegocio,
                                ev.Solicitante,
                                ev.Canal,
                                ev.Actualizado,
                                ev.EstadoIncidencia,
                                ev.TiempoResolucion,
                                ev.AbiertoPor,
                                ev.SubCategoria,
                                ev.ResolucionHrs,
                                ev.CerradoPor,
                                ev.ActualizadoPor,
                                ev.Supervision_estimado,
                                ev.Supervision_real,
                                ev.Infraestructura_estimado,
                                ev.Infraestructura_real,
                                ev.HelpDesk_estimado,
                                ev.HelpDesk_real,
                                ev.SoporteUser_estimado,
                                ev.SoporteUser_real,
                                ev.Arquitectura_estimado,
                                ev.Arquitectura_real,
                                ev.ServicioDBA_estimado,
                                ev.ServicioDBA_real,
                                ev.TotalHoras_estimado,
                                ev.TotalHoras_real,
                                ev.Comentario,
                                ev.created_at
                            }).ToList();
            if(fechaInicio != "")
            {
                DateTime fechaConvertidaInicio = DateTime.Parse(fechaInicio);
                result = result.Where(x => x.created_at >= fechaConvertidaInicio).ToList();
            }
            if(fechaFin != "")
            {
                DateTime fechaConvertidaFin = DateTime.Parse(fechaFin);
                result = result.Where(x => x.created_at <= fechaConvertidaFin).ToList();
            }
            if(tipo == 1)
            {
                result = result.Where(x => x.Tipo == "").ToList();
            }
            if (tipo == 2)
            {
                result = result.Where(x => x.Tipo != "RQ" && x.Tipo != "IN" && x.Tipo != "").ToList();
            }
            if(atribuidoA != "")
            {
                result = result.Where(x => x.AtribuidoA==atribuidoA).ToList();
            }
            if (tipoAtencion != "")
            {
                result = result.Where(x => x.TipoAtencion == tipoAtencion).ToList();
            }
            result = result.OrderByDescending(ev => ev.id_TablaLookerStudio).Take(1000).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerTicketsServiceNow(string fechaInicio, string fechaFin)
        {
            if (fechaInicio == "" && fechaFin == "")
            {
                var result = (from sn in dbhc.ServicesNow
                              select new
                              {
                                  sn.id_servicesNow,
                                  sn.GrupoDeAsignacion,
                                  sn.BreveDescripcion,
                                  sn.AtribuidoA,
                                  sn.Numero,
                                  sn.Prioridad,
                                  sn.MotivoPonerEspera,
                                  sn.TiempoResolucion,
                                  sn.Categoria,
                                  sn.CreadoPor,
                                  sn.Abierto,
                                  sn.FechaVencimiento,
                                  sn.UnidadNegocio,
                                  sn.Solicitante,
                                  sn.Canal,
                                  sn.Actualizado,
                                  sn.EstadoIncidencia,
                                  sn.AbiertoPor,
                                  sn.SubCategoria,
                                  sn.CerradoPor,
                                  sn.ActualizadoPor,
                                  sn.created_at
                              })
                          .OrderByDescending(ev => ev.created_at)
                          .Take(1000)
                          .ToList();
                return Json(new
                {
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from sn in dbhc.ServicesNow
                              select new
                              {
                                  sn.id_servicesNow,
                                  sn.GrupoDeAsignacion,
                                  sn.BreveDescripcion,
                                  sn.AtribuidoA,
                                  sn.Numero,
                                  sn.Prioridad,
                                  sn.MotivoPonerEspera,
                                  sn.TiempoResolucion,
                                  sn.Categoria,
                                  sn.CreadoPor,
                                  sn.Abierto,
                                  sn.FechaVencimiento,
                                  sn.UnidadNegocio,
                                  sn.Solicitante,
                                  sn.Canal,
                                  sn.Actualizado,
                                  sn.EstadoIncidencia,
                                  sn.AbiertoPor,
                                  sn.SubCategoria,
                                  sn.CerradoPor,
                                  sn.ActualizadoPor,
                                  sn.created_at
                              })
                          .Take(1000)
                          .ToList();
                if (fechaInicio != "")
                {
                    DateTime fechaConvertidaInicio = DateTime.Parse(fechaInicio);
                    result = result.Where(x => x.created_at >= fechaConvertidaInicio).ToList();
                }
                if (fechaFin != "")
                {
                    DateTime fechaConvertidaFin = DateTime.Parse(fechaFin);
                    result = result.Where(x => x.created_at <= fechaConvertidaFin).ToList();
                }
                return Json(new
                {
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult ObtenerTicketServiceNow(string numero)
        //{
        //    //ServicesNow servicesNow = dbhc.ServicesNow.Where(x=>x.Numero.Contains(numero)).First();
        //    ServicesNow servicesNow = dbhc.ServicesNow.FirstOrDefault(x=>x.Numero.Equals(numero,StringComparison.OrdinalIgnoreCase));
        //    return Json(new
        //    {
        //        data = servicesNow
        //    }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult CargarInformacionTicket(int id)
        {
            TablaLookerStudio tablaLooker = dbhc.TablaLookerStudio.Find(id);
            return PartialView("_FormularioEditarTicket", tablaLooker);
        }

        public ActionResult ObtenerTiposAtencion()
        {
            var result = (from ta in dbhc.TipoAtencion
                          select new
                          {
                              id = ta.TipoAtencion1,
                              text = ta.TipoAtencion1,
                              idAtencion = ta.id_TipoAtencion
                          }).ToList();
            //result = result.Where(x => !x.id.Equals("Ticket")).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerTiposAtencionPorId()
        {
            var result = (from ta in dbhc.TipoAtencion
                          select new
                          {
                              id = ta.id_TipoAtencion,
                              text = ta.TipoAtencion1,

                          }).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerCategorias()
        {
            var result = (from cat in dbhc.Categoria
                          select new
                          {
                              id = cat.id_Categoria,
                              text = cat.Categoria1,
                              idTipoAtencion = cat.id_TipoAtencion,
                          }).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerServicesNowTickets()
        {
            var result = (from cat in dbhc.ServicesNow
                          select new
                          {
                              id = cat.Numero,
                              text = cat.Numero
                          }).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerSolicitantes()
        {
            var result = dbhc.Solicitante.Select(x => new
            {
                id = x.id_Solicitante,
                text = x.Solicitante1
            }).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerEquivalencias()
        {
            var result = (from eq in dbhc.Equivalencia
                          select new
                          {
                              id = eq.id_Equivalencia,
                              text = eq.Nombre,
                              area = eq.Area,
                              usuarioRed = eq.usuarioRed,
                              id_GpoAsignacion = eq.id_GpoAsignacion
                          }).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerEquivalenciasEditar(string idGrupoAsignacion)
        {
            GpoAsignacion idGrupoAsig = dbhc.GpoAsignacion.Where(x => x.GpoAsignacion1 == idGrupoAsignacion).FirstOrDefault();
            var result = (from eq in dbhc.Equivalencia
                          where eq.id_GpoAsignacion == idGrupoAsig.id_GpoAsignacion
                          select new
                          {
                              id = eq.id_Equivalencia,
                              text = eq.Nombre,
                              area = eq.Area,
                          }).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDetalleCategoria(int idCategoria)
        {
            var categoriaFiltrada = new List<object>();
            var categoria = dbhc.Categoria
                .Select(x=>new {
                    x.id_Categoria,
                    x.Categoria1,
                    x.Supervision_estimado,
                    x.Supervision_real,
                    x.Infraestructura_estimado,
                    x.Infraestructura_real,
                    x.HelpDesk_estimado,
                    x.HelpDesk_real,
                    x.SoporteUser_estimado,
                    x.SoporteUser_real,
                    x.Arquitectura_estimado,
                    x.Arquitectura_real,
                    x.ServicioDBA_estimado,
                    x.ServicioDBA_real,
                    x.TotalHoras_estimado,
                    x.TotalHoras_real,
                    x.id_TipoAtencion
                }).Where(x=>x.id_Categoria == idCategoria).ToList();
            //if(idTipoAtencion != -1)
            //{
            //    categoria = categoria.Where(x => x.id_TipoAtencion == idTipoAtencion).ToList();
            //}
            //foreach (var item in categoria)
            //{
            //    if (item.Categoria1 == categoriaNombre)
            //    {
            //        categoriaFiltrada.Add(item); // Agregar el item a la lista filtrada
            //    }
            //}
            return Json(new
            {
                data = categoria
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCanales()
        {
            var result = dbhc.Canal.Select(x => new
            {
                id = x.Canal1,
                text = x.Canal1
            }).ToList();
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDetalleEquivalencia(string equivalenciaArea)
        {
            var equivalenciaFiltrada = new List<object>();
            var equivalencia = dbhc.Equivalencia.ToList();
            foreach (var item in equivalencia)
            {
                if (item.Area == equivalenciaArea)
                {
                    equivalenciaFiltrada.Add(item);
                }
            }
            return Json(new
            {
                data = equivalenciaFiltrada
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerGrupoAsignacion()
        {
            var gpoAsignacion = dbhc.GpoAsignacion
                .Select(x=>new {
                    id=x.id_GpoAsignacion,
                    text=x.GpoAsignacion1
                }).ToList();
            return Json(new
            {
                data = gpoAsignacion
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerSubCategorias()
        {
            var subCategorias = dbhc.SubCategoriaHs
                .Select(x => new
                {
                    id = x.id_SubCategoria,
                    text = x.SubCategoria1,
                    categoriaId = x.id_Categoria
                }).ToList();
            return Json(new
            {
                data = subCategorias
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaAtribuidoA()
        {
            var lista = dbhc.Equivalencia.Select(x => new
            {
                id=x.Nombre,
                text=x.Nombre
            }).Distinct().ToList();

            return Json(new
            {
                data = lista
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TipoAtencion()
        {
            var lista = dbhc.TipoAtencion.Select(x => new
            {
                id = x.TipoAtencion1,
                text = x.TipoAtencion1
            }).ToList();

            return Json(new
            {
                data = lista
            }, JsonRequestBehavior.AllowGet);
        }

        public string ObtenerPrimeraPalabra(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return string.Empty;
            }

            int indiceGuion = texto.IndexOf('-');
            if (indiceGuion == -1)
            {
                return texto; // Si no hay guion, devolver todo el string
            }
            string primeraPalabra = texto.Substring(0, indiceGuion);
            primeraPalabra = primeraPalabra.Trim();

            return primeraPalabra;
        }
        public double HorasANumero(string hora)
        {
            // Dividir la cadena en horas y minutos
            string[] partes = hora.Split(':');

            // Convertir las partes a números
            int horas = int.TryParse(partes[0], out int horasResultado) ? horasResultado : 0; // Si no se puede convertir, asumir 0 horas
            int minutos = int.TryParse(partes[1], out int minutosResultado) ? minutosResultado : 0; // Si no se puede convertir, asumir 0 minutos

            // Calcular el total de horas (sumar las horas y convertir los minutos a horas)
            double totalHoras = horas + minutos / 60.0;

            return totalHoras;
        }
    }
}
