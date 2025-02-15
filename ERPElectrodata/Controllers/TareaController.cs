using ERPElectrodata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using OfficeOpenXml;
using ClosedXML.Excel;

namespace ERPElectrodata.Controllers
{
    public class TareaController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();

        public ActionResult Index()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            bool tienePermiso = (Convert.ToInt32(Session["ADMINISTRADOR"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_SERVICEDESK"]) == 1);

            if (idAcco == 60 || ((idAcco == 56 || idAcco == 57 || idAcco == 58) && tienePermiso))
                return View();
            else
                return RedirectToAction("Index", "Error");
        }

        public ActionResult Create()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (idAcco == 60 || idAcco == 56 || idAcco == 57 || idAcco == 58)
                return View();
            else
                return Content("");
        }

        public ActionResult Detalle(int id)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            bool tienePermiso = (Convert.ToInt32(Session["ADMINISTRADOR"]) == 1 || Convert.ToInt32(Session["SUPERVISOR_SERVICEDESK"]) == 1);

            if ((idAcco == 56 || idAcco == 57 || idAcco == 58) && tienePermiso)
            {
                var categoria = dbc.ListarCategoriasConTareas(id, idAcco).FirstOrDefault();

                if (categoria == null)
                    return Content("");

                ViewBag.NomCategoria = categoria.NomCategoria;
                ViewBag.Estado = categoria.Estado;
                ViewBag.ResuelveTick = categoria.ResuelveTicket;
                ViewBag.Id = categoria.Id;
                return View();
            }
            else if (idAcco == 60)
            {
                var categoria = dbc.ListarCategoriasConTareasBNV(id, idAcco).FirstOrDefault();

                if (categoria == null)
                    return Content("");

                ViewBag.NomCategoria = categoria.NomCategoria;
                ViewBag.Estado = categoria.Estado;
                ViewBag.ResuelveTick = categoria.ResuelveTicket;
                ViewBag.TipoTick = categoria.TipoTicket;
                ViewBag.Id = categoria.Id;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult CrearTareas(int id)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (idAcco == 56 || idAcco == 57 || idAcco == 58)
            {
                ViewBag.Id = id;
                return View();
            }
            else if (idAcco == 60)
            {
                ViewBag.Id = id;
                return View("CrearTareasBNV");
            }
            else
            {
                return Content("");
            }
        }

        public ActionResult CategoriaPorTarea(int id, int porTarea = 0)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (idAcco == 56 || idAcco == 57 || idAcco == 58)
            {
                ViewBag.ID_ACCO = idAcco;
                ViewBag.ID_CATEN2 = id;
                ViewBag.PorTarea = porTarea;
                return View();
            }
            else
            {
                return Content("");
            }
        }

        public ActionResult TransferirTarea(int id, int idQueu)
        {
            ViewBag.IdTareaDetalle = id;
            ViewBag.IdQueu = idQueu;
            return View();
        }

        public ActionResult EstadoTareaBNV(int id, string estado)
        {
            ViewBag.IdTarea = id;
            ViewBag.Estado = estado;
            return View();
        }

        public ActionResult ImportarTareasMinsur(int idCate, int idTick)
        {
            ViewBag.IdCate = idCate;
            ViewBag.IdTick = idTick;
            return View();
        }

        public ActionResult EditarTareaMinsur(int id, int idCate)
        {
            var tarea = dbc.TareaDetalle.FirstOrDefault(x => x.IdTareaDetalle == id);
            ViewBag.IdCate = idCate;
            return View(tarea);
        }

        public ActionResult ResolverTicket(int idTick, int idTareaDetalle, int idEstado)
        {
            ViewBag.IdTicket = idTick;
            ViewBag.IdTareaDetalle = idTareaDetalle;
            ViewBag.IdEstado = idEstado;
            return View();
        }

        public ActionResult ReportePorTicket(int id)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (idAcco == 60)
            {
                ViewBag.IdTick = id;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Reporte()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (idAcco == 60)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public ActionResult CrearCategoria(int idCate, bool resolverTicket)
        {
            try
            {
                int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
                int userId = Convert.ToInt32(Session["UserId"]);

                if (idAcco == 56 || idAcco == 57 || idAcco == 58)
                {
                    var result = dbc.CrearCategoriaConTareas(idCate, resolverTicket, idAcco, userId).FirstOrDefault();
                    return Json(new { success = true, message = result.Msg, id = result.Id });
                }
                else
                {
                    var result = dbc.CrearCategoriaConTareasBNV(idCate, resolverTicket, idAcco, userId).FirstOrDefault();
                    return Json(new { success = true, message = result.Msg, id = result.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ERROR", id = 0 });
            }
        }

        [HttpPost]
        public ActionResult EditarCategoria(int id, bool estadoCate, bool resuelveTick)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                var msg = dbc.EditarCategoriaConTareas(id, estadoCate, resuelveTick, userId).FirstOrDefault();

                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult CrearTareas(int id, List<Tarea> tareas)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int orden = 1;

                foreach (var tarea in tareas)
                {
                    if (tarea.IdTarea == 0)
                    {
                        var msg = dbc.CrearTareasPorCategoria(id, tarea.TituloTarea, tarea.DescripcionTarea, orden, tarea.Id_Queu, tarea.Id_Cate, tarea.IdTareaPadre, tarea.DescPendiente, tarea.TipoGestion, tarea.Estado, userId).FirstOrDefault();
                    }
                    else
                    {
                        var msg = dbc.EditarTareaPorCategoria(tarea.IdTarea, tarea.TituloTarea, tarea.DescripcionTarea, orden, tarea.Id_Queu, tarea.Id_Cate, tarea.IdTareaPadre, tarea.DescPendiente, tarea.TipoGestion, tarea.Estado, userId).FirstOrDefault();
                    }

                    if (tarea.Estado == true)
                        orden++;
                }

                return Json(new { success = true, message = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult CrearTareasBNV(int id, List<TareaBNV> tareas)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int orden = 1;

                foreach (var tarea in tareas)
                {
                    if (tarea.Id == 0)
                    {
                        var msg = dbc.CrearConfigTareaPorCategoriaBNV(id, tarea.Titulo, tarea.Descripcion, orden, tarea.IdQueu, tarea.IdAsignado, tarea.IdTareaPadre, tarea.Estado, userId).FirstOrDefault();
                    }
                    else
                    {
                        var msg = dbc.EditarConfigTareaPorCategoriaBNV(tarea.Id, tarea.Titulo, tarea.Descripcion, orden, tarea.IdQueu, tarea.IdAsignado, tarea.IdTareaPadre, tarea.Estado, userId).FirstOrDefault();
                    }

                    if (tarea.Estado == true)
                        orden++;
                }

                return Json(new { success = true, message = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult AsignarUsuarioTarea(int id, int usuario, int idQueue)
        {
            try
            {
                var tareaDetalle = dbc.TareaDetalle.Where(x => x.IdTareaDetalle == id).FirstOrDefault();

                tareaDetalle.IdEstado = 4;
                tareaDetalle.Id_Queu = idQueue;
                tareaDetalle.Id_Pers_Enti = usuario;
                dbc.TareaDetalle.Attach(tareaDetalle);
                dbc.Entry(tareaDetalle).State = EntityState.Modified;
                dbc.SaveChanges();

                return Json(new { success = true, message = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult GuardarEstadoTarea(int id, string estado)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int idEstado;

                if (estado == "No Procede")
                    idEstado = 2;
                else
                    idEstado = 1;

                var msg = dbc.GuardarEstadoTarea(id, idEstado, userId).FirstOrDefault();

                return Json(new { success = true, message = msg, idTareaDetalle = id, idEstado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GuardarComentarioTicketResuelto(int idTick, int idTareaDetalle, int idEstado, string comentario)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                var msg = dbc.ResolverTicketTareas(idTick, idTareaDetalle, idEstado, comentario, userId).FirstOrDefault();
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ActualizarEstadoTareaDetalleBNV(int id, string estado, int usuario = 0, string comentario = "", DateTime? fecha = null)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                string msg = "OK";

                if (estado == "Transferido")
                {
                    var tareaTransferida = dbc.TransferirTareaBNV(id, usuario, userId).FirstOrDefault();

                    if (tareaTransferida != null)
                    {
                        ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                        var envio = mail.sendMailTareasBNV((int)tareaTransferida.IdTarea);
                        if (envio == true)
                        {
                            dbc.EnvioCorreoTareaBNV(tareaTransferida.IdTareaDetalle, 2);
                        }
                    }
                }
                else if (estado == "En Proceso")
                {
                    dbc.IniciarTareaBNV(id, userId);
                }
                else if (estado == "Programado")
                {
                    dbc.ProgramarTareaBNV(id, comentario, fecha, userId);
                    msg = "REFRESH";
                }
                else if (estado == "Procede")
                {
                    var tareasCreadas = dbc.FinalizarTareaBNV(id, 1, comentario, userId).ToList();
                    msg = "REFRESH";

                    foreach (var tarea in tareasCreadas)
                    {
                        ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                        var envio = mail.sendMailTareasBNV(tarea.IdTarea);
                        if (envio == true)
                        {
                            dbc.EnvioCorreoTareaBNV(tarea.IdTarea, 1);
                        }
                    }
                }
                else if (estado == "No Procede")
                {
                    dbc.FinalizarTareaBNV(id, 2, comentario, userId);
                    msg = "REFRESH";
                }

                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult CrearTareasMasivas(int idTicket, List<TareaDetalle> tareas)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int idCate = (int)dbc.TICKETs.First(x => x.ID_TICK == idTicket).ID_CATE;

                foreach (var tarea in tareas)
                {
                    if (idCate == 27158 || idCate == 28529 || idCate == 29900) //MOVIL
                    {
                        dbc.CrearTareaMovil(idTicket, tarea.IdTarea, tarea.IdSedeImpu, tarea.IdMoneda, tarea.Monto, tarea.CeCoImpu, tarea.PartPresup, tarea.FechaEjecutada, userId);
                    }
                    else if (idCate == 32465 || idCate == 32466 || idCate == 32467) //CIERRE CONTABLE
                    {
                        dbc.CrearTareaContable(idTicket, tarea.IdUsuarioSap, tarea.IdGrupoAutoriza, tarea.FechaDesde, tarea.FechaEjecutada, tarea.Id_Pers_Enti, userId);
                    }
                }

                return Json(new { success = true, message = "OK" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult GuardarEstadoTareasMasivas(List<EstadoTareaMovil> tareas)
        {
            try
            {
                foreach (var tarea in tareas)
                {
                    TareaDetalle objTareaDetalle = dbc.TareaDetalle.FirstOrDefault(x => x.IdTareaDetalle == tarea.IdTareaDetalle);

                    if (objTareaDetalle != null)
                    {
                        if (tarea.Estado == "No Procede")
                        {
                            objTareaDetalle.IdEstado = 2;
                        }
                        else
                        {
                            objTareaDetalle.IdEstado = 1;
                        }

                        objTareaDetalle.FechaTareaTerminada = DateTime.Now;
                        dbc.SaveChanges();
                    }
                }

                return Json(new { success = true, message = "OK" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult CerrarTicket(int idTicket)
        {
            try
            {
                TICKET objTicket = dbc.TICKETs.Where(x => x.ID_TICK == idTicket).FirstOrDefault();

                objTicket.ID_STAT_END = 4;
                dbc.SaveChanges();
                // AGREGAR COMENTARIO DE RESOLUCION AUTOMATICA DEL TICKET
                DETAILS_TICKET objDetails = new DETAILS_TICKET();
                objDetails.ID_TICK = idTicket;
                objDetails.ID_STAT = 4;
                objDetails.ID_TYPE_DETA_TICK = 3;
                objDetails.COM_DETA_TICK = "Ticket resuelto automáticamente al término de sus tareas.";
                objDetails.UserId = 34;
                objDetails.CREATE_DETA_INCI = DateTime.Now;
                objDetails.FROM_TIME = DateTime.Now;
                objDetails.TO_TIME = DateTime.Now;
                objDetails.IdTipoResolucion = 2;
                dbc.DETAILS_TICKET.Add(objDetails);
                dbc.SaveChanges();

                return Json(new { success = true, message = "OK" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult EditarTareaCreada(int idCate, TareaDetalle tarea)
        {
            try
            {
                var objTareaDetalle = dbc.TareaDetalle.FirstOrDefault(x => x.IdTareaDetalle == tarea.IdTareaDetalle);

                if (idCate == 32465 || idCate == 32466 || idCate == 32467)
                {
                    objTareaDetalle.IdUsuarioSap = tarea.IdUsuarioSap;
                    objTareaDetalle.IdGrupoAutoriza = tarea.IdGrupoAutoriza;
                    objTareaDetalle.FechaDesde = tarea.FechaDesde;
                    objTareaDetalle.FechaEjecutada = tarea.FechaEjecutada;
                }

                dbc.TareaDetalle.Attach(objTareaDetalle);             
                dbc.Entry(objTareaDetalle).State = EntityState.Modified;
                dbc.SaveChanges();

                return Json(new { success = true, message = "OK" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "ERROR" });
            }
        }

        [HttpPost]
        public ActionResult ImportarTareas(int idCate, HttpPostedFileBase excelFile)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var listTareas = new List<TareaDetalle>();

            if (excelFile != null && excelFile.ContentLength > 0)
            {
                try
                {
                    using (var excelWorkbook = new XLWorkbook(excelFile.InputStream))
                    {
                        var excelWorksheet = excelWorkbook.Worksheet(1);

                        if (excelWorksheet.Name != "TAREAS")
                        {
                            return Json(new { data = listTareas, message = "Seleccione la plantilla" }, JsonRequestBehavior.AllowGet);
                        }

                        var rowCount = excelWorksheet.RowsUsed().Count();

                        for (int i = 2; i <= rowCount; i++)
                        {
                            var rowData = new TareaDetalle();

                            if (idCate == 32465 || idCate == 32466 || idCate == 32467)
                            {
                                var usuarioSAP = excelWorksheet.Cell(i, 1).Value.ToString().Trim();
                                var grupoAutoriza = excelWorksheet.Cell(i, 2).Value.ToString().Trim();
                                var asignado = excelWorksheet.Cell(i, 5).Value.ToString().Trim();

                                var datos = dbc.ValidarDatosTareasContable(idAcco, usuarioSAP, grupoAutoriza, asignado).FirstOrDefault();

                                rowData.IdGrupoAutoriza = datos.IdGrupoAutoriza;
                                rowData.Id_Pers_Enti = datos.IdAsignado;
                                rowData.IdUsuarioSap = datos.IdUsuarioSap;
                                rowData.FechaDesde = GetDateValue(excelWorksheet.Cell(i, 3));
                                rowData.FechaEjecutada = GetDateValue(excelWorksheet.Cell(i, 4));
                            }
                            
                            listTareas.Add(rowData);
                        }
                    }

                    return Json(new { data = listTareas, message = "OK" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { data = listTareas, message = "Ha ocurrido un error, contacte al administrador" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { data = listTareas, message = "Seleccione un Excel con información" }, JsonRequestBehavior.AllowGet);
            }
        }

        private DateTime? GetDateValue(IXLCell cell)
        {
            // Celda vacia
            if (cell.IsEmpty() || cell.GetValue<string>().Trim() == string.Empty)
            {
                return null;
            }

            // Fecha como dateTime
            if (cell.TryGetValue<DateTime>(out DateTime fecha))
            {
                return fecha;
            }

            // Convertir fecha string a dateTime
            string fechaString = cell.GetValue<string>().Trim();

            if (DateTime.TryParse(fechaString, out fecha))
            {
                return fecha;
            }

            return null;
        }

        public ActionResult DescargarPlantillaExcel(int idCate)
        {
            string rutaArchivo = "";
            string nombreArchivo = "";

            if (idCate == 32465 || idCate == 32466 || idCate == 32467)
            {
                rutaArchivo = Server.MapPath("~/Adjuntos/Plantilla Tareas Contable.xlsx");
                nombreArchivo = "Plantilla Tareas Contable.xlsx";
            }

            return File(rutaArchivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }

        public ActionResult ListarCategoriasConTareas(int id = 0)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarCategoriasConTareas(id, idAcco).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTareasPorCategoria(int id)
        {
            var query = dbc.ListarTareasPorCategoria(id).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarTareasPorTicket(int idTicket)
        {
            int idQueue = Convert.ToInt32(Session["ID_QUEU"]);
            var listaTareasPendientes = dbc.ListarTareasPorTicket(idTicket, idQueue).ToList();
            return Json(listaTareasPendientes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCategoriasConTareasBNV(int id = 0)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.ListarCategoriasConTareasBNV(id, idAcco).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTareasPorCategoriaBNV(int id)
        {
            var query = dbc.ListarConfigTareasPorCategoriaBNV(id).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarTareasPorTicketBNV(int idTicket)
        {
            int idPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var listaTareasPendientes = dbc.ListaTareasPorTicketBNV(idTicket, idPersEnti).ToList();
            return Json(new { Data = listaTareasPendientes, Count = listaTareasPendientes.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListadoReporteTareasBNV()
        {
            int idQueu = !string.IsNullOrEmpty(Request.Params["IdQueu"]) ? Convert.ToInt32(Request.Params["IdQueu"]) : 0;
            int idEstado = !string.IsNullOrEmpty(Request.Params["Estado"]) ? Convert.ToInt32(Request.Params["Estado"]) : 0;
            
            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;

            string codigo = Convert.ToString(Request.Params["Codigo"]);

            if (!string.IsNullOrEmpty(Request.Params["FechaInicio"]))
            {
                fechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["FechaFin"]))
            {
                fechaFin = Convert.ToDateTime(Request.Params["FechaFin"]);
            }

            var listaTareas = dbc.ReporteTareasBNV(fechaInicio, fechaFin, idQueu, idEstado, codigo).ToList();
            return Json(new { Data = listaTareas, Count = listaTareas.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarReporteTarea()
        {
            int idQueu = !string.IsNullOrEmpty(Request.Params["IdQueu"]) ? Convert.ToInt32(Request.Params["IdQueu"]) : 0;
            int idEstado = !string.IsNullOrEmpty(Request.Params["Estado"]) ? Convert.ToInt32(Request.Params["Estado"]) : 0;

            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;

            string codigo = Convert.ToString(Request.Params["Codigo"]);

            if (!string.IsNullOrEmpty(Request.Params["FechaInicio"]))
            {
                fechaInicio = Convert.ToDateTime(Request.Params["FechaInicio"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["FechaFin"]))
            {
                fechaFin = Convert.ToDateTime(Request.Params["FechaFin"]);
            }

            DataTable dataTable = ObtenerDatosReporteBNV(fechaInicio, fechaFin, idQueu, idEstado, codigo);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ReporteTareas");

                ws.Column(8).Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                ws.Column(9).Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                ws.Column(10).Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                ws.Column(11).Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 30;
                ws.Column(4).Width = 40;
                ws.Column(5).Width = 35;
                ws.Column(6).Width = 20;
                ws.Column(7).Width = 40;
                ws.Column(8).Width = 25;
                ws.Column(9).Width = 25;
                ws.Column(10).Width = 25;
                ws.Column(11).Width = 25;
                ws.Column(12).Width = 25;
                ws.Column(13).Width = 25;
                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);

                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=ReporteTareas.xlsx");

                ms.WriteTo(HttpContext.Response.OutputStream);
                HttpContext.Response.End();
            }

            return View();
        }

        private DataTable ObtenerDatosReporteBNV(DateTime? fechaInicio, DateTime? fechaFin, int idQueu, int idEstado, string codigo)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Buenaventura.ExportarReporteTareasBNV", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmd.Parameters.AddWithValue("@IdQueu", idQueu);
                    cmd.Parameters.AddWithValue("@IdEstado", idEstado);
                    cmd.Parameters.AddWithValue("@CodigoTarea", codigo);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public JsonResult ListarComentariosTareasBNV(int idTicket)
        {
            var listaComentarios = dbc.ListarComentariosTareasBNV(idTicket).ToList();
            return Json(new { Data = listaComentarios, Count = listaComentarios.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarUsuariosSAPContable()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var listaUsuarios = dbc.ListarUsuariosSAPContable(idAcco).ToList();
            return Json(new { Data = listaUsuarios, Count = listaUsuarios.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarGrupoAutorizaContable()
        {
            var lista = dbc.ListarGrupoAutorizaContable().ToList();
            return Json(new { Data = lista, Count = lista.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAsignadosMinsur()
        {
            string termino = "";
            int idQueu = 0;
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);

            if (NAM_PAR == "ID_QUEU")
            {
                idQueu = Convert.ToInt32(Request.QueryString[("filter[filters][0][value]")]);
            }
            else if (NAM_PAR == "ASSI")
            {
                termino = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                idQueu = Convert.ToInt32(Request.QueryString[("filter[filters][1][value]")]);
            }
            else
            {
                idQueu = Convert.ToInt32(Request.Params["AreaResp"]);
            }

            var query = dbe.ListarAsignadosTareas(idQueu, idAcco, termino).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

    }

    public class TareaBNV
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int IdQueu { get; set; }
        public int IdAsignado { get; set; }
        public int? IdTareaPadre { get; set; }
        public bool Estado { get; set; }
    }
}
