using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Globalization;
using System.Configuration;
//using ERPElectrodata.Objects;
//using WebMatrix.WebData;
//using System.Web.Security;
using System.Threading;
using ERPElectrodata.Object;
using ERPElectrodata.Object.Ticket;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI;
//using System.Net;
using app = ERPElectrodata.AppCode;
using System.Text;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using OfficeOpenXml;
//using System.IO;
using ERPElectrodata.Objects.CreateMail;
using ERPElectrodata.Object.Plugins;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
//using Microsoft.Reporting.WebForms;
using ERPElectrodata.Models.Enum;
using System.Data.SqlClient;
using ClosedXML.Excel;
//using DocumentFormat.OpenXml.Office.Excel;
//using OfficeOpenXml;

namespace ERPElectrodata.Controllers
{
    public class TicketController : Controller
    {
        public CoreEntities db = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        public int ctd = 1;
        private PointTicket pt = new PointTicket();
        public TicketIR tir = new TicketIR();

        public int estado = 0;
        [Authorize]
        public ActionResult UpdateDateTicket(int id)
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                TICKET query = db.TICKETs.Single(x => x.ID_TICK == id);
                ViewBag.IdPrioridad = query.ID_PRIO;
                ViewBag.FEC_TICK = query.FEC_TICK;
                ViewBag.FEC_INI_TICK_EDIT = query.FEC_INI_TICK;
                ViewBag.FlagProblema = Convert.ToInt32(query.FlagProblema);
                ViewBag.Habilitado = Convert.ToInt16(Session["SERVICEDESK"]);
                ViewBag.IdCompania = query.ID_COMP;
                ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
                ViewBag.FechaRecepcionSolicitud = query.FechaRecepcionSolicitud;
                ViewBag.IdPrioridad = query.ID_PRIO;
                ViewBag.SubCuenta = query.SubCuenta;
                ViewBag.IdAcco = ID_ACCO;
                ViewBag.IDCATE = query.ID_CATE;
                if (query.IdProyectoSLA == null)
                {
                    ViewBag.IdProyectoSLA = 0;
                }
                else
                {
                    ViewBag.IdProyectoSLA = query.IdProyectoSLA;
                    ViewBag.ProyectoSLA = db.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == query.IdProyectoSLA).NUM_DOCU_SALE;
                }
                if (ID_ACCO == 60)
                {
                    ViewBag.Tag = query.TAG;
                    ViewBag.ImpactoServicio = query.ImpactoServicio;
                }

                string ID_COMP_END = Convert.ToString(query.ID_COMP_END);
                string ID_COMP = Convert.ToString(query.ID_COMP);
                //string IdCompEnd;
                //if (ID_COMP_END == 0)
                //{
                //    IdCompEnd = "";
                //}
                //else
                //{
                //    IdCompEnd = Convert.ToString(ID_COMP_END);
                //}
                ViewBag.IdCompEnd = ID_COMP_END;
                ViewBag.IdComp = ID_COMP;
                ViewBag.IdPersEnti = Convert.ToInt32(query.ID_PERS_ENTI);
                ViewBag.ID_ESTADO_END = query.ID_STAT_END;
                ViewBag.ID_TYPE_TICK = query.ID_TYPE_TICK;
                if (Convert.ToInt32(Session["ID_ACCO"]) == 60)
                {
                    ViewBag.IdRazon = query.IdRazon;
                }

                var categorias = db.CategoriasxIdCateTicket(query.ID_CATE).First();
                ViewBag.Cate1 = categorias.CategoriaId_1;
                ViewBag.Cate2 = categorias.CategoriaId_2;
                ViewBag.Cate3 = categorias.CategoriaId_3;
                ViewBag.Cate4 = categorias.CategoriaId_4;
                ViewBag.IdSLA = query.IdSLA;
                bool cia = db.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).VIS_COMP.Value;
                if (cia)
                {
                    if (ID_ACCO == 4)
                    {
                        return View("EditarTicketEdata", query);
                    }
                    else
                    {
                        return View("UpdateDateTicketEX1", query);
                    }

                }
                else
                {

                    ViewBag.IdPersEnti = query.ID_PERS_ENTI;
                    return View(query);
                }
            }
            catch
            {
                return Content("Please Close Session");
            }

        }

        #region "Encuesta"

        public ActionResult ImportarDatosEncuesta()
        {
            ViewBag.IDAcco = Convert.ToString(Session["ID_ACCO"]);
            return View();
        }

        [HttpPost]
        public ActionResult PrevisualizarDatos()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];

                if (file != null && file.ContentLength > 0)
                {
                    using (XLWorkbook workbook = new XLWorkbook(file.InputStream))
                    {
                        IXLWorksheet worksheet = workbook.Worksheet(1);

                        int lastRow = worksheet.LastRowUsed().RowNumber();
                        int batchSize = 1000;

                        var previsualizacionDatos = new List<PersonasEncuesta>();

                        for (int startRow = 5; startRow <= lastRow; startRow += batchSize)
                        {
                            int endRow = Math.Min(startRow + batchSize - 1, lastRow);

                            for (int row = startRow; row <= endRow; row++)
                            {
                                string valorColumnaC = worksheet.Cell(row, 3).Value.ToString();

                                if (!string.IsNullOrEmpty(valorColumnaC))
                                {
                                    PersonasEncuesta importe = new PersonasEncuesta
                                    {
                                        NombreCompleto = worksheet.Cell(row, 1).Value.ToString(),
                                        LocalSede = worksheet.Cell(row, 2).Value.ToString(),
                                        Email = valorColumnaC,
                                        Unidad = worksheet.Cell(row, 4).Value.ToString(),
                                        Departamento = worksheet.Cell(row, 5).Value.ToString(),
                                        FechaRegistro = DateTime.Now,
                                    };

                                    previsualizacionDatos.Add(importe);
                                }
                            }
                        }

                        return Json(new { success = true, message = "Datos cargados para previsualización", previsualizacionDatos });
                    }
                }

                return Json(new { success = false, message = "No se seleccionó ningún archivo" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cargar datos para previsualización: " + ex.Message });
            }
        }



        [HttpPost]
        public ActionResult ImportarDatos()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];

                if (file != null && file.ContentLength > 0)
                {
                    using (XLWorkbook workbook = new XLWorkbook(file.InputStream))
                    {
                        IXLWorksheet worksheet = workbook.Worksheet(1);

                        int lastRow = worksheet.LastRowUsed().RowNumber();
                        int batchSize = 1000;

                        for (int startRow = 5; startRow <= lastRow; startRow += batchSize)
                        {
                            int endRow = Math.Min(startRow + batchSize - 1, lastRow);

                            for (int row = startRow; row <= endRow; row++)
                            {
                                string valorColumnaC = worksheet.Cell(row, 3).Value.ToString();

                                if (!string.IsNullOrEmpty(valorColumnaC))
                                {
                                    PersonasEncuesta importe = new PersonasEncuesta
                                    {
                                        NombreCompleto = worksheet.Cell(row, 1).Value.ToString(),
                                        Unidad = worksheet.Cell(row, 4).Value.ToString(),
                                        Departamento = worksheet.Cell(row, 5).Value.ToString(),
                                        LocalSede = worksheet.Cell(row, 2).Value.ToString(),
                                        Email = valorColumnaC,
                                        FechaRegistro = DateTime.Now
                                    };

                                    int result = db.ImportarPersonasEncuesta(
                                        importe.NombreCompleto,
                                        importe.Unidad,
                                        importe.Departamento,
                                        importe.LocalSede,
                                        importe.Email,
                                        importe.FechaRegistro
                                    );
                                }
                            }

                            dbe.SaveChanges();
                        }

                        var aniosActualizados = db.ListarPersonasEncuesta(0, 0, "0").Select(x => x.FechaRegistro).Distinct().ToList();

                        var sede = db.ListarPersonasEncuesta(0, 0, "0").Select(x => x.LocalSede).Distinct().ToList();

                        return Json(new { success = true, message = "Datos importados correctamente", aniosActualizados, sede });
                    }
                }

                return Json(new { success = false, message = "No se seleccionó ningún archivo" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al importar datos: " + ex.Message });
            }
        }





        public ActionResult ListarImporteEncuesta(int anio, int reenvio, string localSede)
        {
            if (localSede == null)
            {
                localSede = "0";
            }

            if (reenvio == 1)
            {
                var resultPorAni = anio != 0 ? db.ListarPersonasEncuesta(anio, reenvio, localSede).ToList() : null;

                return Json(new { dataPorAnio = resultPorAni }, JsonRequestBehavior.AllowGet);
            }

            var resultTodos = db.ListarPersonasEncuesta(0, 0, "0").ToList();
            var anios = resultTodos.Select(x => x.FechaRegistro).Distinct().ToList();
            var sede = resultTodos.Select(x => x.LocalSede).Distinct().ToList();
            var resultPorAnio = anio != 0 ? db.ListarPersonasEncuesta(anio, reenvio, localSede).ToList() : null;

            return Json(new { dataTodos = resultTodos, dataPorAnio = resultPorAnio, anios = anios, sede = sede }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerAnios()
        {
            var resultTodos = db.ListarPersonasEncuesta(0, 0, "0").ToList();
            var anios = resultTodos.Select(x => x.FechaRegistro).Distinct().ToList();

            return Json(new { anios = anios }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerLocalesSede()
        {
            var resultTodos = db.ListarPersonasEncuesta(0, 0, "0").ToList();
            var sede = resultTodos.Select(x => x.LocalSede).Distinct().ToList();

            return Json(new { sede = sede }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ReenviarCorreo(string email)
        {
            try
            {
                var emailAddresses = email;
                return EnviarCorreoElectronico(emailAddresses);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al reenviar el correo electrónico: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EnviarCorreoElectronico(string emailAddresses)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.office365.com"))
                {
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                    var correo = db.ACCOUNT_PARAMETER
                        .Where(account => (account.ID_PARA == 24) && account.ID_ACCO == ID_ACCO)
                        .Select(account => account.VAL_ACCO_PARA)
                        .ToList().FirstOrDefault();

                    var contraseña = db.ACCOUNT_PARAMETER
                        .Where(account => (account.ID_PARA == 25) && account.ID_ACCO == ID_ACCO)
                        .Select(account => account.VAL_ACCO_PARA)
                        .ToList().FirstOrDefault();

                    client.Port = 587;
                    client.UseDefaultCredentials = true;

                    NetworkCredential credentials = new NetworkCredential(correo, contraseña);
                    client.Credentials = credentials;

                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    client.EnableSsl = true;

                    var cuerpo = db.ACCOUNT_PARAMETER
                        .Where(account => (account.ID_PARA == 1213) && account.ID_ACCO == 60)
                        .Select(account => account.VAL_ACCO_PARA)
                        .ToList().FirstOrDefault();

                    var emailBatch = emailAddresses.Split(',').Select(email => email.Trim()).ToList();

                    foreach (var email in emailBatch)
                    {
                        using (MailMessage message = new MailMessage())
                        {
                            message.From = new MailAddress(correo);
                            message.Subject = "Encuesta Servicios TIC";

                            var idListarPersonasEncuesta = ObtenerIdListarPersonasEncuestaParaCorreo(email);
                            var enlacePersonalizado = $"{cuerpo}{idListarPersonasEncuesta}";

                            var cuerpoMensaje = $"Estimado usuario:<br><br>" +
                                                "Para nosotros es importante conocer tu experiencia sobre nuestros servicios. " +
                                                "Ayúdanos a mejorar completando esta encuesta.<br><br>" +
                                                "Si tienes dudas o consultas, comunícate con nuestra Mesa de Servicios al anexo 123<br><br>" +
                                                $"<a href='{enlacePersonalizado}'>Encuesta Servicios TIC</a>";

                            message.Body = cuerpoMensaje;
                            message.IsBodyHtml = true;
                            message.To.Add(email);

                            client.Send(message);
                        }
                    }

                }

                return Json(new { success = true, message = "Correos electrónicos enviados correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al enviar los correos electrónicos: " + ex.Message });

            }
        }




        private int ObtenerIdListarPersonasEncuestaParaCorreo(string correo)
        {

            var idListarPersonasEncuesta = db.PersonasEncuesta
                .Where(persona => persona.Email == correo)
                .Select(persona => persona.IdData)
                .FirstOrDefault();

            return idListarPersonasEncuesta;
        }




        [HttpPost]
        public ActionResult DescargarDatosEncuesta(int anio)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("[EncuestaBuenaventura].[DescargarDatosEncuesta]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@anio", anio);

                        using (var reader = command.ExecuteReader())
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine("<meta charset='utf-8'>");

                            stringBuilder.AppendLine("<table>");
                            stringBuilder.AppendLine("<tr>");

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                stringBuilder.AppendLine($"<th class='cabecera'>{reader.GetName(i)}</th>");
                            }

                            stringBuilder.AppendLine("</tr>");

                            while (reader.Read())
                            {
                                stringBuilder.AppendLine("<tr>");

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    stringBuilder.AppendLine($"<td class='contenido'>{reader[i]}</td>");
                                }

                                stringBuilder.AppendLine("</tr>");
                            }

                            stringBuilder.AppendLine("</table>");

                            Response.Clear();
                            Response.Buffer = true;
                            Response.ContentType = "application/vnd.ms-excel";
                            Response.Charset = "";
                            Response.ContentEncoding = System.Text.Encoding.UTF8;
                            Response.AddHeader("Content-Disposition", $"attachment;filename=ExcelDatosEncuesta_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls");
                            Response.Write(stringBuilder.ToString());
                            Response.Flush();
                            Response.End();
                        }
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message, "text/plain");
            }
        }




        #endregion

        [Authorize]
        public ActionResult UpdateDateTicketMinsur(int id)
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                TICKET query = db.TICKETs.Single(x => x.ID_TICK == id);
                ViewBag.IdPrioridad = query.ID_PRIO;
                ViewBag.FEC_TICK = query.FEC_TICK;
                ViewBag.FEC_INI_TICK_EDIT = query.FEC_INI_TICK;
                ViewBag.FlagProblema = Convert.ToInt32(query.FlagProblema);
                ViewBag.Habilitado = Convert.ToInt16(Session["SERVICEDESK"]);
                ViewBag.IdCompania = query.ID_COMP;
                ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
                ViewBag.FechaRecepcionSolicitud = query.FechaRecepcionSolicitud;
                ViewBag.IdPrioridad = query.ID_PRIO;
                ViewBag.SubCuenta = query.SubCuenta;
                ViewBag.IdAcco = ID_ACCO;
                ViewBag.IDCATE = query.ID_CATE;

                CategoriasxIdCateTicket_Result listaCat = db.CategoriasxIdCateTicket(query.ID_CATE).First();
                ViewBag.Cate1 = listaCat.CategoriaId_1;
                ViewBag.Cate2 = listaCat.CategoriaId_2;
                ViewBag.Cate3 = listaCat.CATEGORIA_3;
                ViewBag.Cate4 = listaCat.CATEGORIA_4;
                ViewBag.Cate5 = listaCat.CATEGORIA_5;
                ViewBag.Cate6 = listaCat.CATEGORIA_6;
                ViewBag.CateId3 = listaCat.CategoriaId_3;
                ViewBag.CateId4 = listaCat.CategoriaId_4;
                ViewBag.CateId5 = listaCat.CategoriaId_5;
                ViewBag.CateId6 = listaCat.CategoriaId_6;

                if (query.IdProyectoSLA == null)
                {
                    ViewBag.IdProyectoSLA = 0;
                }
                else
                {
                    ViewBag.IdProyectoSLA = query.IdProyectoSLA;
                    ViewBag.ProyectoSLA = db.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == query.IdProyectoSLA).NUM_DOCU_SALE;
                }


                string ID_COMP_END = Convert.ToString(query.ID_COMP_END);
                string ID_COMP = Convert.ToString(query.ID_COMP);
                //string IdCompEnd;
                //if (ID_COMP_END == 0)
                //{
                //    IdCompEnd = "";
                //}
                //else
                //{
                //    IdCompEnd = Convert.ToString(ID_COMP_END);
                //}
                ViewBag.IdCompEnd = ID_COMP_END;
                ViewBag.IdComp = ID_COMP;
                ViewBag.IdPersEnti = Convert.ToInt32(query.ID_PERS_ENTI);
                ViewBag.ID_ESTADO_END = query.ID_STAT_END;
                ViewBag.ID_TYPE_TICK = query.ID_TYPE_TICK;
                if (Convert.ToInt32(Session["ID_ACCO"]) == 60)
                {
                    ViewBag.IdRazon = query.IdRazon;
                }

                bool cia = db.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).VIS_COMP.Value;
                if (cia)
                {
                    return View("UpdateDateTicketEX1", query);
                }
                else
                {
                    ViewBag.IdSLA = query.IdSLA;
                    ViewBag.IdPersEnti = query.ID_PERS_ENTI;
                    return View(query);
                }
            }
            catch
            {
                return Content("Please Close Session");
            }

        }


        public ActionResult EditarFormatoTicket(int id = 0)
        {
            ViewBag.IdTicket = id;
            return View();
        }



        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateDateTicket(IEnumerable<HttpPostedFileBase> archivos, TICKET ticket)
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                DateTime FEC_INI_TICK = Convert.ToDateTime(Request.Params["FEC_INI_TICK_EDIT"]);
                int segundos = 0;
                int millisegundos = 0;
             
                var qTicket = db.TICKETs.Single(x => x.ID_TICK == ticket.ID_TICK);
                string idPersEnti = Convert.ToString(Request.Params["IdPersEnti"]);
                string KEY_ATTA = null;

                var impactoResult = db.ImpactoBVNComparativo(Convert.ToInt32(ticket.ID_TICK));
                string ImpactoComparativo = impactoResult.FirstOrDefault();


                var IdUserResult = Session["UserId"];
                string IdUser = IdUserResult.ToString();

                //ERP//




                var resultCateAnterior = db.ObtenerCategoriaAnteriorTicket(ticket.ID_TICK).FirstOrDefault();
                int ID_CATE_ANTERIOR = resultCateAnterior.ID_CATE;




                if (ID_ACCO == 60)
                {
                    if (Convert.ToInt32(ticket.ID_CATE) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar las Categorías.<br/>');}window.onload=init;</script>");

                    }

                    if (Convert.ToInt32(ticket.ID_PRIO) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar la Prioridad<br/>');}window.onload=init;</script>");

                    }
                    if (Convert.ToInt32(ticket.ID_SOUR) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Medio de Comunicación. <br/>');}window.onload=init;</script>");

                    }

                    if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar al Usuario Afectado<br/>');}window.onload=init;</script>");

                    }

                    if (Convert.ToInt32(ticket.IdModTrabajo) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar La modalidad de Trabajo.<br/>');}window.onload=init;</script>");

                    }
                    if (Convert.ToInt32(ticket.ID_LOCA) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar la Locación.<br/>');}window.onload=init;</script>");

                    }
                    if (Convert.ToInt32(ticket.ID_PERS_ENTI) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Usuario Solicitante.<br/>');}window.onload=init;</script>");

                    }
                }
                try
                {
                    KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);

                }
                catch
                {

                }

                if (ticket.ID_PRIO == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneEdit) top.uploadDoneEdit('ERRORBCP','Ingresar la prioridad.');}window.onload=init;</script>");
                }

                if (ticket.IdSLA == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneEdit) top.uploadDoneEdit('ERRORBCP','Ingresar el SLA.');}window.onload=init;</script>");
                }

                if (qTicket.ID_COMP == 4034) //Cliente BCP
                {
                    if (ticket.FechaRecepcionSolicitud == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneEdit) top.uploadDoneEdit('ERRORBCP','Ingresar la fecha de recepción de la solicitud.');}window.onload=init;</script>");
                    }
                }
                segundos = FEC_INI_TICK.Second;
                millisegundos = FEC_INI_TICK.Millisecond;
                FEC_INI_TICK = FEC_INI_TICK.AddSeconds(-(segundos)).AddMilliseconds(-(millisegundos));
                segundos = 0;
                millisegundos = 0;

                //if (ID_ACCO == 17 && ticket.ID_ACCO == 17)
                //{

                string TAG = "";
                string ImpactoServicio = "";
                string GarantiaProveedor = "";
                if (ID_ACCO == 60) {  TAG = Convert.ToString(ticket.TAG); ImpactoServicio = Convert.ToString(ticket.ImpactoServicio);
                    GarantiaProveedor = Convert.ToString(ticket.GarantiaProveedor);
                }
                int idCateN2 = Convert.ToInt32(Request["ID_CATE_N2"]);
                int idCateN1 = Convert.ToInt32(Request["ID_CATE_N1"]);
                int idCateN3 = Convert.ToInt32(Request["ID_CATE_N3"]);
                int idCate = Convert.ToInt32(Request["ID_CATE"]);

                var valGarantia = db.ComparativaCategoryBNV(idCateN1, idCateN2, idCateN3, idCate).FirstOrDefault();


                if (valGarantia == 0) { GarantiaProveedor = ""; }

                int ID_TICK = Convert.ToInt32(ticket.ID_TICK);
                string TIT_TICK = Convert.ToString(ticket.TITLE_TICK);
                string SUM_TICK = Convert.ToString(ticket.SUM_TICK);
                string Adicional1 = Convert.ToString(ticket.Adicional1);
                string Adicional2 = Convert.ToString(ticket.Adicional2);
                string TicketExterno = Convert.ToString(ticket.TicketExterno);
                if (String.IsNullOrEmpty(Convert.ToString(ticket.ID_SOUR)))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Ingrese el medio de comunicación');}window.onload=init;</script>");
                }
                if (String.IsNullOrEmpty(Convert.ToString(ticket.ID_COMP)))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Ingrese la compañía solicitante');}window.onload=init;</script>");
                }
                if (String.IsNullOrEmpty(SUM_TICK))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Ingrese Comentario');}window.onload=init;</script>");
                }
                if (String.IsNullOrEmpty(TIT_TICK))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Ingrese Asunto');}window.onload=init;</script>");
                }
                if (String.IsNullOrEmpty(ticket.ID_PERS_ENTI_END.ToString()) && Convert.ToInt32(Session["SUPERVISOR_SERVICEDESK"]) == 1)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Ingrese el usuario afectado.');}window.onload=init;</script>");
                }

                TICKET query = db.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                int IdUsuarioAfectado = Convert.ToInt32(query.ID_PERS_ENTI_END);
                query.TITLE_TICK = TIT_TICK;
                DateTime fecha = Convert.ToDateTime(query.FEC_INI_TICK);
                segundos = fecha.Second;
                millisegundos = fecha.Millisecond;
                fecha = fecha.AddSeconds(-(segundos)).AddMilliseconds(-(millisegundos));


                if (!String.IsNullOrEmpty(ticket.ID_PERS_ENTI_END.ToString()))
                {
                    int IdUsuarioAfectadoMod = Convert.ToInt32(ticket.ID_PERS_ENTI_END);
                    //Usuario afectado
                    if (IdUsuarioAfectadoMod != IdUsuarioAfectado)
                    {
                        var per = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdUsuarioAfectado).SingleOrDefault().ID_ENTI2;
                        var ce = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == per).SingleOrDefault();
                        string usuarioActual = (ce.FIR_NAME + (ce.LAS_NAME == null ? "" : " " + ce.LAS_NAME)).ToUpper();

                        var perM = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdUsuarioAfectadoMod).SingleOrDefault().ID_ENTI2;
                        var ceM = dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == perM).SingleOrDefault();
                        string usuarioModificado = (ceM.FIR_NAME + (ceM.LAS_NAME == null ? "" : " " + ceM.LAS_NAME)).ToUpper();

                        var detailsTicket = new DETAILS_TICKET();
                        detailsTicket.ID_TICK = ID_TICK;
                        detailsTicket.ID_TYPE_DETA_TICK = 1;
                        detailsTicket.COM_DETA_TICK = "Se modificó el usuario afectado " + usuarioActual + " por " + usuarioModificado;
                        detailsTicket.UserId = Convert.ToInt32(Session["UserId"]);
                        detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                        detailsTicket.MINUTES = 0;
                        db.DETAILS_TICKET.Add(detailsTicket);
                        db.SaveChanges();
                    }
                    query.ID_PERS_ENTI_END = IdUsuarioAfectadoMod;
                }

                if (FEC_INI_TICK != query.FEC_INI_TICK)
                {
                    query.ID_STAT = 5;
                    query.FEC_INI_TICK = FEC_INI_TICK;
                }


                if (!String.IsNullOrEmpty(ticket.ID_PRIO.ToString()))
                {
                    query.ID_PRIO = ticket.ID_PRIO;
                }
                if (!String.IsNullOrEmpty(ticket.ID_COMP.ToString()))
                {
                    query.ID_COMP = ticket.ID_COMP;
                }
                if (!String.IsNullOrEmpty(ticket.ID_COMP_END.ToString()))
                {
                    query.ID_COMP_END = ticket.ID_COMP_END;
                }


                //Ticket Problema
                int Cont = Convert.ToInt32(Request.Params["cont"]);
                if (Convert.ToInt32(query.FlagProblema) == 1)
                {
                    for (int i = 1; i <= Cont; i++)
                    {
                        SUM_TICK = SUM_TICK + "<a target='_blank' href='/DetailsTicket/Index/" + Convert.ToInt32(Request.Params["RECEIVER" + i + ""]) + "'" + " style='text-decoration:none;'>" +
                        "<span class='glyphicon glyphicon-play' aria-hidden='true'></span><span>" + Convert.ToInt32(Request.Params["RECEIVER" + i + ""]) + " </span></a>";

                    }
                }
                query.SUM_TICK = SUM_TICK;
                query.ID_SOUR = ticket.ID_SOUR;
                if (ID_ACCO == 4 || ID_ACCO == 17) /* RMA para Edata y Claro*/
                {
                    query.RMA = ticket.RMA;
                }
                query.Adicional1 = Adicional1;
                query.Adicional2 = Adicional2;
                query.TicketExterno = TicketExterno;
                query.FechaRecepcionSolicitud = ticket.FechaRecepcionSolicitud;
                query.IdSLA = ticket.IdSLA;

                if(ID_ACCO == 60)
                {
                    query.TAG = TAG;
                    query.ImpactoServicio = ImpactoServicio;
                    /* modificado por LS*/
                    query.GarantiaProveedor = GarantiaProveedor;
                    query.IdMotivoSource = ticket.IdMotivoSource;
                    query.UsuarioVIP = ticket.UsuarioVIP;
                    query.FalsoPositivo = ticket.FalsoPositivo;
                }

                query.IdProyectoSLA = ticket.IdProyectoSLA;
                query.MODIFIED_TICK = DateTime.Now;
                if (ID_ACCO == 4)
                {
                    if (ticket.ID_COMP != 9)
                    {
                        query.SubCuenta = "EXTERNO";
                    }
                }
                query.ID_PERS_ENTI = Convert.ToInt32(idPersEnti);

                if (query.ID_CATE != ticket.ID_CATE && !String.IsNullOrEmpty(ticket.ID_CATE.ToString()))
                {
                    //insertar Log
                    query.ID_CATE = ticket.ID_CATE;

                    LogTransaccione objLogTransaccion = new LogTransaccione();
                    objLogTransaccion.Tipo = "A";
                    objLogTransaccion.Tabla = "TICKET";
                    objLogTransaccion.IdRegistro = "<ID_TICK=" + query.ID_TICK.ToString() + ">";
                    objLogTransaccion.Campo = "ID_CATE";
                    objLogTransaccion.ValorOriginal = ticket.ID_CATE.ToString();
                    objLogTransaccion.ValorNuevo = query.ID_CATE.ToString();
                    objLogTransaccion.Mensaje = "";
                    objLogTransaccion.FechaRegistro = DateTime.Now;
                    objLogTransaccion.UserId = new app.Sesiones().SesionIdUsuario;
                    db.Entry(objLogTransaccion).State = EntityState.Added;
                }

                if (ID_ACCO == 60) /* Campo para creación de Ticket - BNV*/
                {

                    int ID_LOCA = Request.Form["ID_LOCA"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_LOCA"].ToString());
                    int ID_RAZON = Request.Form["ID_RAZON2"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_RAZON2"].ToString());
                    int ID_TIPOPASE = Request.Form["IdTipoPase"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdTipoPase"].ToString());  //Convert.ToInt32(Request.Form["ID_TIPOPASE"].ToString());
                    int ID_MODTRABAJO = Request.Form["IdModTrabajo"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdModTrabajo"].ToString());  //Convert.ToInt32(Request.Form["ID_MODTRABAJO"].ToString());
                    string SOLMAN = Request.Form["Solman"] == null ? null : Convert.ToString(Request.Form["Solman"]);
                    int ID_PERS_ENTI_ANLT = Request.Form["ID_PERS_ENTI_ANLT"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_PERS_ENTI_ANLT"].ToString());
                    query.ID_PERS_ENTI = Convert.ToInt32(ticket.ID_PERS_ENTI);

                    //string SOLMAN = Request.Form["SOLMAN"].ToString();

                    if (ID_LOCA == 0) { query.ID_LOCA = null; } else { query.ID_LOCA = ID_LOCA; }
                    if (ID_RAZON == 0) { query.IdRazon = null; } else { query.IdRazon = ID_RAZON; }
                    if (ID_TIPOPASE == 0) { query.IdTipoPase = null; } else { query.IdTipoPase = ID_TIPOPASE; }
                    if (ID_MODTRABAJO == 0) { query.IdModTrabajo = null; } else { query.IdModTrabajo = ID_MODTRABAJO; }
                    if (SOLMAN == "") { query.Solman = null; } else { query.Solman = SOLMAN; }
                    if (ID_PERS_ENTI_ANLT == 0) { query.ID_PERS_ENTI_ANLT = null; } else { query.ID_PERS_ENTI_ANLT = ID_PERS_ENTI_ANLT; }

                }
                query.MODIFIED_TICK = DateTime.Now;
                db.Entry(query).State = EntityState.Modified;
                db.SaveChanges();

                //Ticket Problema
                if (Convert.ToInt32(query.FlagProblema) == 1 && Convert.ToInt32(Session["SUPERVISOR_SERVICEDESK"]) == 1)
                {
                    int cate4 = Convert.ToInt32(ticket.ID_CATE);
                    int cate3 = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == cate4).SingleOrDefault().ID_CATE_PARE);
                    int cate2 = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == cate3).SingleOrDefault().ID_CATE_PARE);
                    int cate1 = Convert.ToInt32(db.CATEGORies.Where(x => x.ID_CATE == cate2).SingleOrDefault().ID_CATE_PARE);
                    int IdPrioridad = Convert.ToInt32(query.ID_PRIO);
                    for (int i = 1; i <= Cont; i++)
                    {

                        int ticketsRelacionados = Convert.ToInt32(Request.Params["RECEIVER" + i + ""]);
                        ComTicketProblema ticketProblema = new ComTicketProblema();
                        ticketProblema.IdTicketProblema = ID_TICK;
                        ticketProblema.idTicket = ticketsRelacionados;
                        ticketProblema.IdCategoria1 = cate1;
                        ticketProblema.IdCategoria2 = cate2;
                        ticketProblema.IdCategoria3 = cate3;
                        ticketProblema.IdCategoria4 = cate4;
                        ticketProblema.IdPrioridad = IdPrioridad;
                        ticketProblema.Activo = true;
                        ticketProblema.UsuarioCreacion = Convert.ToInt32(Session["UserId"]);
                        ticketProblema.Creado = DateTime.Now;
                        ticketProblema.IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

                        db.ComTicketProblemas.Add(ticketProblema);
                        db.SaveChanges();

                    }
                    db.tktTicketProblemaModificar(ID_TICK, cate1, cate2, cate3, cate4, IdPrioridad);
                }
                db.UPDATE_MINUTES_TICKET(ID_TICK);

                //ERP//
                var resultCategoriaActual = db.ObtenerCategoriaAnteriorTicket(ticket.ID_TICK).FirstOrDefault();
                int ID_CATE_ACTUAL = resultCategoriaActual.ID_CATE;
                int UserId = Convert.ToInt32(Session["UserId"].ToString());
                int ID_ACCO_CATEGORIA;
                if (ID_ACCO == 60 || ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                {
                    ID_ACCO_CATEGORIA = ID_ACCO;
                }
                else
                {
                    ID_ACCO_CATEGORIA = 4;
                }
                string CabecerasCategoria = db.ObtenerNombreCategoriasTicket(ID_ACCO_CATEGORIA).FirstOrDefault();
                Body body = new Body();

                if (ID_CATE_ACTUAL != ID_CATE_ANTERIOR)
                {
                    db.GuardarCategoríaTicket(ID_TICK, 1, UserId, body.TicketCambioCategoria(resultCateAnterior, resultCategoriaActual, ID_ACCO, CabecerasCategoria), ID_CATE_ANTERIOR, ticket.ID_COMP, ID_ACCO, Convert.ToInt32(idPersEnti));

                }

                if (KEY_ATTA != null)
                {
                    var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                        .Where(x => x.ID_INCI == null).ToList();
                    if (Attachs.Count() > 0)
                    {
                        foreach (ATTACHED attach in Attachs)
                        {
                            try
                            {
                                var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                                EObj.ID_INCI = query.ID_TICK;
                                db.Entry(EObj).State = EntityState.Modified;
                                db.SaveChanges();
                                db.Entry(EObj).State = EntityState.Detached;
                            }
                            catch
                            {

                            }

                        }
                    }
                }
                if (ID_ACCO == 60)
                {
                    if (ImpactoServicio != null && ImpactoComparativo != ImpactoServicio)
                    {
                        db.ComentarioImpactoServicioBNV(ID_TICK, 1, "Impacto del Servicio: " + ImpactoServicio, IdUser, ImpactoServicio);
                    }
                }


                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('OK','La información ha sido actualizada.');}window.onload=init;</script>");
                //}
                //else
                //{
                //    return Content("<script type='text/javascript'> function init() {" +
                //           "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','You can not make changes to this account');}window.onload=init;</script>");
                //}

            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Contácte al Administrador');}window.onload=init;</script>");

            }
        }

        [Authorize]
        public ActionResult AdjuntarArchivosNuevoTicket(string KEY_ATTA)
        {
            ViewBag.KEY_ATTA = KEY_ATTA;
            return View();
        }

        [Authorize]
        public ActionResult AgregarCategoriasTicket(int id = 0, int idAcco = 0)
        {
            ViewBag.ID_ACCO = idAcco;
            ViewBag.ID_CATEN2 = id;
            return View();
        }
        public ActionResult AgregarCategoriasTicketBuscar(int id = 0)
        {
            ViewBag.ID_CATEN2 = id;
            return View();
        }
        public ActionResult ListarCategorias(int idCateN2 = 0, int idAcco = 0)
        {
            string ID_CATE = Convert.ToString(idCateN2);
            int ID_ACCO = idAcco;

            int ID_PARA = Convert.ToInt32(Request.Params["ID_PARA"]);


            if (ID_PARA == 0)
            {
                List<ArbolCategoria_Result> query = new List<ArbolCategoria_Result>();
                query = db.ArbolCategoria(ID_ACCO, idCateN2).ToList();

                int countquery = query.Count();


                return Json(query.OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
            else
            {

                List<ArbolCategoria_Result> query = new List<ArbolCategoria_Result>();
                query = db.ArbolCategoria(ID_ACCO, ID_PARA).ToList();

                int countquery = query.Count();

                return Json(query.OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult ListarTipoTicketxCategoria(int ID_CATE)
        {
            var listaTipoTicket = db.ListaTipoTicketxIdCate(ID_CATE).ToList();

            return Json(new { Data = listaTipoTicket, Count = listaTipoTicket.Count() }, JsonRequestBehavior.AllowGet);
        }

        #region "Gestion del Conocimiento"
        public ActionResult viewDetalleUsuario(int id = 0)
        {
            ViewBag.ID_USUARIO = id;
            return View();

        }
        #endregion

        #region "Aprobacion de Accesos"
        public ActionResult viewAprobarAcceso(int id = 0)
        {
            ViewBag.ID_TICK = id;
            return View();

        }

        public ActionResult AprobarAccesoBNV()
        {
            var objTicket = new TICKET();
            var objDetalleTicket = new DETAILS_TICKET();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int ID_TICK = Convert.ToInt32(Request.Params["IdTick"].ToString());
            string Comentario = "";

            try
            {
                //TipoFianza = Convert.ToInt32(Request.Params["TipoFianzaContrato"].ToString());
                Comentario = Request.Params["Comentario"].ToString();

                if (Comentario == null || Comentario == "")
                {
                    return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            if (ID_TICK != 0)
            {
                objDetalleTicket.ID_TICK = ID_TICK;
                objDetalleTicket.COM_DETA_TICK = Comentario;
                objDetalleTicket.ID_QUEU = 95;
                objDetalleTicket.ID_TYPE_DETA_TICK = 2;
                objDetalleTicket.ID_PERS_ENTI = 79605;
                objDetalleTicket.UserId = UserId;
                objDetalleTicket.CREATE_DETA_INCI = DateTime.Now;
                objDetalleTicket.IdRazon = 12;
                db.DETAILS_TICKET.Add(objDetalleTicket);
                db.SaveChanges();

                objDetalleTicket.ID_TICK = ID_TICK;
                objDetalleTicket.COM_DETA_TICK = Comentario;
                objDetalleTicket.ID_STAT = 1;
                objDetalleTicket.ID_TYPE_DETA_TICK = 3;
                objDetalleTicket.UserId = UserId;
                objDetalleTicket.CREATE_DETA_INCI = DateTime.Now;
                objDetalleTicket.IdRazon = 2;
                db.DETAILS_TICKET.Add(objDetalleTicket);

                objTicket = db.TICKETs.Where(x => x.ID_TICK == ID_TICK).SingleOrDefault();
                objTicket.IdRazon = 2;
                db.Entry(objTicket).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                //var objDocFianza = dbc.Fianza.Where(x => x.TipoFianza == TipoFianza && x.Importe == Importe && x.Estado == true && x.Id != IdDocContratoFianza && x.IdDocumentoContrato == IdDocContrato);
                //if (objDocFianza.Count() > 0)
                //{
                return Json(new { /*IdFianza = 0,*/ Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                //}

                //objTicket = db.TICKETs.Where(x => x.ID_TICK == ID_TICK).SingleOrDefault();
                //objFianza.TipoFianza = TipoFianza;
                //objFianza.Importe = Importe;
                //objFianza.FechaInicio = FechaInicio;
                //objFianza.FechaFin = FechaFin;
                //objFianza.Descripcion = Descripcion;
                //dbc.Entry(objFianza).State = EntityState.Modified;

            }
            //db.SaveChanges();

            return Json(new { /*IdFianza = objFianza.Id, */Respuesta = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewRechazarAcceso(int id = 0)
        {
            ViewBag.ID_TICK = id;
            return View();

        }

        public ActionResult RechazarAccesoBNV()
        {
            var objTicket = new TICKET();
            var objDetalleTicket = new DETAILS_TICKET();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int ID_TICK = Convert.ToInt32(Request.Params["IdTick"].ToString());
            string Comentario = "";

            try
            {
                Comentario = Request.Params["Comentario"].ToString();

                if (Comentario == null || Comentario == "")
                {
                    return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            if (ID_TICK != 0)
            {
                objDetalleTicket.ID_TICK = ID_TICK;
                objDetalleTicket.ID_STAT = 4;
                objDetalleTicket.COM_DETA_TICK = Comentario;
                objDetalleTicket.ID_TYPE_DETA_TICK = 3;
                objDetalleTicket.UserId = UserId;
                objDetalleTicket.CREATE_DETA_INCI = DateTime.Now;
                objDetalleTicket.IdTipoResolucion = 2;
                objDetalleTicket.IdRazon = 3;
                db.DETAILS_TICKET.Add(objDetalleTicket);
                db.SaveChanges();

                objTicket = db.TICKETs.Where(x => x.ID_TICK == ID_TICK).SingleOrDefault();
                objTicket.IdRazon = 3;
                objTicket.Solucion = Comentario;
                db.Entry(objTicket).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            ////db.SaveChanges();

            return Json(new { Respuesta = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewMasInformacionAcceso(int id = 0)
        {
            ViewBag.ID_TICK = id;
            return View();

        }

        public ActionResult MasInformacionAccesoBNV()
        {
            var objTicket = new TICKET();
            var objDetalleTicket = new DETAILS_TICKET();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int ID_TICK = Convert.ToInt32(Request.Params["IdTick"].ToString());
            string Comentario = "";

            try
            {
                Comentario = Request.Params["Comentario"].ToString();

                if (Comentario == null || Comentario == "")
                {
                    return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            if (ID_TICK != 0)
            {
                objDetalleTicket.ID_TICK = ID_TICK;
                objDetalleTicket.COM_DETA_TICK = Comentario;
                objDetalleTicket.ID_QUEU = 100;
                objDetalleTicket.ID_TYPE_DETA_TICK = 2;
                objDetalleTicket.ID_PERS_ENTI = 76370;
                objDetalleTicket.UserId = UserId;
                objDetalleTicket.CREATE_DETA_INCI = DateTime.Now;
                objDetalleTicket.IdRazon = 2;
                db.DETAILS_TICKET.Add(objDetalleTicket);
                db.SaveChanges();

                objDetalleTicket.ID_TICK = ID_TICK;
                objDetalleTicket.COM_DETA_TICK = Comentario;
                objDetalleTicket.ID_STAT = 1;
                objDetalleTicket.ID_TYPE_DETA_TICK = 3;
                objDetalleTicket.UserId = UserId;
                objDetalleTicket.CREATE_DETA_INCI = DateTime.Now;
                objDetalleTicket.IdRazon = 2;
                db.DETAILS_TICKET.Add(objDetalleTicket);

                objTicket = db.TICKETs.Where(x => x.ID_TICK == ID_TICK).SingleOrDefault();
                objTicket.IdRazon = 2;
                db.Entry(objTicket).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            //db.SaveChanges();

            return Json(new { Respuesta = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewGenerarTicketRemediacionHostBuenaventura(int id = 0)
        {
            ViewBag.ID_TICK = id;
            return View();

        }

        public ActionResult GenerarTicketRemediacionHostBNV()
        {
            var objTicket = new TICKET();
            var objDetalleTicket = new DETAILS_TICKET();
            var objAdjunto = new ATTACHED();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int ID_TICK = Convert.ToInt32(Request.Params["IdTick"].ToString());
            string Comentario = "";

            int id = 0;

            try
            {
                Comentario = Request.Params["Comentario"].ToString();

                if (Comentario == null /*|| Comentario == ""*/)
                {
                    return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            if (ID_TICK != 0)
            {
                TICKET ticketPadre = db.TICKETs.FirstOrDefault(t => t.ID_TICK == ID_TICK);
                DETAILS_TICKET comentarioTicketPadre = db.DETAILS_TICKET.Where(t => t.ID_TICK == ID_TICK && t.IdRazon == 14)
                    .OrderByDescending(t => t.ID_DETA_TICK)
                   .FirstOrDefault();
                var Locacion = db.LOCATIONs.FirstOrDefault(t => t.ID_LOCA == ticketPadre.ID_LOCA);

                objTicket.ID_TICK = ticketPadre.ID_TICK;
                objTicket.ID_ACCO = ticketPadre.ID_ACCO;
                objTicket.ID_TYPE_TICK = ticketPadre.ID_TYPE_TICK;
                objTicket.ID_AREA = ticketPadre.ID_AREA;
                objTicket.ID_PRIO = ticketPadre.ID_PRIO;
                objTicket.ID_QUEU = 90;
                objTicket.ID_STAT = 1;
                objTicket.ID_STAT_END = 1;
                objTicket.ID_SOUR = ticketPadre.ID_SOUR;
                objTicket.FEC_TICK = DateTime.Now;
                objTicket.FEC_INI_TICK = DateTime.Now;
                objTicket.SUM_TICK = ticketPadre.SUM_TICK + "<br>" + comentarioTicketPadre.COM_DETA_TICK + "<br>" + Comentario;
                objTicket.REM_CTRL_TICK = ticketPadre.REM_CTRL_TICK;
                objTicket.UserId = UserId;
                objTicket.CREATE_TICK = DateTime.Now;
                objTicket.MODIFIED_TICK = DateTime.Now;
                objTicket.ID_TICK_PARENT = ticketPadre.ID_TICK;
                objTicket.IS_PARENT = true;
                objTicket.ID_COMP = ticketPadre.ID_COMP;
                objTicket.ID_COMP_END = ticketPadre.ID_COMP_END;
                objTicket.ID_CATE = ticketPadre.ID_CATE;
                objTicket.ID_PERS_ENTI = ticketPadre.ID_PERS_ENTI;
                objTicket.ID_PERS_ENTI_END = ticketPadre.ID_PERS_ENTI_END;
                objTicket.ID_PERS_ENTI_ASSI = /*60451;*/ Locacion.IdAnalistaRemediacionHost;
                objTicket.ID_LOCA = ticketPadre.ID_LOCA;
                objTicket.ID_CIA = ticketPadre.ID_CIA;
                objTicket.TITLE_TICK = "Remediación de Host" + " " + ticketPadre.COD_TICK;
                objTicket.IdTema = ticketPadre.IdTema;
                objTicket.FechaRecepcionSolicitud = DateTime.Now;
                objTicket.IdSLA = ticketPadre.IdSLA;
                objTicket.IdRazon = 2;
                objTicket.IdMarca = ticketPadre.IdMarca;
                objTicket.IdModTrabajo = ticketPadre.IdModTrabajo;
                objTicket.FlagModifica = ticketPadre.FlagModifica;
                db.TICKETs.Add(objTicket);
                //db.Database.ExecuteSqlCommand("EXEC CrearTicketAccesoRemediacionHostBNV @ID_TICK", new System.Data.SqlClient.SqlParameter("ID_TICK", ID_TICK));                  
                db.SaveChanges();

                TICKET ticketHijo = db.TICKETs.Where(t => t.ID_TICK_PARENT == ID_TICK)
                   .OrderByDescending(t => t.ID_TICK)
                   .FirstOrDefault();
                ATTACHED adjuntoTicketPadre = db.ATTACHEDs.FirstOrDefault(t => t.ID_INCI == ID_TICK);
                ATTACHED adjuntoComentarioTicketPadre = db.ATTACHEDs.Where(t => t.ID_DETA_INCI == comentarioTicketPadre.ID_DETA_TICK)
                   .OrderByDescending(t => t.ID_DETA_INCI)
                   .FirstOrDefault();

                if (adjuntoTicketPadre != null)
                {
                    objAdjunto.NAM_ATTA = adjuntoTicketPadre.NAM_ATTA;
                    objAdjunto.EXT_ATTA = adjuntoTicketPadre.EXT_ATTA;
                    objAdjunto.ID_INCI = ticketHijo.ID_TICK;
                    objAdjunto.CREATE_ATTA = DateTime.Now;
                    objAdjunto.KEY_ATTA = adjuntoTicketPadre.KEY_ATTA;
                    objAdjunto.ID_TYPE_DOCU_ATTA = adjuntoTicketPadre.ID_TYPE_DOCU_ATTA;
                    objAdjunto.DELETE_ATTA = adjuntoTicketPadre.DELETE_ATTA;
                    db.ATTACHEDs.Add(objAdjunto);
                    db.SaveChanges();
                }
                if (adjuntoComentarioTicketPadre != null)
                {
                    objAdjunto.NAM_ATTA = adjuntoComentarioTicketPadre.NAM_ATTA;
                    objAdjunto.EXT_ATTA = adjuntoComentarioTicketPadre.EXT_ATTA;
                    objAdjunto.ID_INCI = ticketHijo.ID_TICK;
                    objAdjunto.CREATE_ATTA = DateTime.Now;
                    objAdjunto.KEY_ATTA = adjuntoComentarioTicketPadre.KEY_ATTA;
                    objAdjunto.ID_TYPE_DOCU_ATTA = adjuntoComentarioTicketPadre.ID_TYPE_DOCU_ATTA;
                    objAdjunto.DELETE_ATTA = adjuntoComentarioTicketPadre.DELETE_ATTA;
                    db.ATTACHEDs.Add(objAdjunto);
                    db.SaveChanges();
                }
                id = ticketHijo.ID_TICK;
            }
            else
            {
                return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            //db.SaveChanges();

            return Json(new { id, Respuesta = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewAprobarAccesoGTI(int id = 0)
        {
            ViewBag.ID_TICK = id;
            return View();

        }

        #endregion

        //try
        //    {
        //        ConexionDatos cnx = new ConexionDatos((int)BD.BasesDatos.Core);
        //cmd = new SqlCommand("[PortalUsuarioBnv].[InsertTicketAcceso]");
        //cmd.CommandType = CommandType.StoredProcedure;
        //        //cmd.Parameters.Add("@ID_TICK", SqlDbType.Int).Value = model.ID_TICK;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.ID_ACCO;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = 2;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = null;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = 3;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = 108;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = 5;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = 5;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = 8;
        //        cmd.Parameters.Add("@", SqlDbType.DateTime).Value = DateTime.Now;
        //        cmd.Parameters.Add("@", SqlDbType.DateTime).Value = null;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = null;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.SUM_TICK;
        //        cmd.Parameters.Add("@", SqlDbType.Bit).Value = false;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.UserId;
        //        cmd.Parameters.Add("@", SqlDbType.DateTime).Value = DateTime.Now;
        //        cmd.Parameters.Add("@", SqlDbType.DateTime).Value = DateTime.Now;
        //        cmd.Parameters.Add("@", SqlDbType.Bit).Value = false;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.ID_COMP;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = null;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.ID_CATE;
        //        cmd.Parameters.Add("@", SqlDbType.Int).Value = model.ID_PERS_ENTI;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.ID_PERS_ENTI;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.ID_PERS_ENTI_ASSI;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.ID_LOCA;
        //        cmd.Parameters.Add("@MINUTES", SqlDbType.VarChar).Value = null;
        //        cmd.Parameters.Add("@DAT_EXPI_TICK", SqlDbType.DateTime).Value = null;
        //        cmd.Parameters.Add("@AMM_SALE_OPPO", SqlDbType.Money).Value = 0.00;
        //        cmd.Parameters.Add("@SEND_MAIL", SqlDbType.Bit).Value = false;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = null;
        //        cmd.Parameters.Add("@", SqlDbType.VarChar).Value = model.TITLE_TICK;
        //        cmd.Parameters.Add("@", SqlDbType.Int).Value = 0;
        //        cmd.Parameters.Add("@IdMarca", SqlDbType.Int).Value = model.IdMarca;
        //        cmd.Parameters.Add("@", SqlDbType.Int).Value = 12;
        //        cmd.Parameters.Add("@", SqlDbType.Int).Value = 158;
        //        cmd.Parameters.Add("@ID_TICK", SqlDbType.Int).Direction = ParameterDirection.Output;

        //        cnx.SqlExecuteNonQuery(cmd);

        //        model.ID_TICK = Convert.ToInt32(cmd.Parameters["@ID_TICK"].Value.ToString());
        //    }



        public ActionResult NotificationsTickets()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

            List<NotificationTicket_Result> tick = db.NotificationTicket().ToList();

            if (ID_ACCO == 4)
            {
                int pers_enti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                List<NotificacionInforme_Result> notificacionInformeAprobar = db.NotificacionInforme(pers_enti).ToList();

                // Aquí convertimos explícitamente la lista notificacionInformeAprobar a tick
                tick = notificacionInformeAprobar.Select(item => new NotificationTicket_Result
                {
                    ID_ACCO = item.ID_ACCO,
                    TITLE_TICK = item.TITLE_TICK,
                    SUM_TICK = item.SUM_TICK,
                    NAM_SOUR = item.NAM_SOUR
                }).ToList();
            }
            else if (ID_ACCO == 52)
            {
                tick = tick.Where(x => x.ID_ACCO == ID_ACCO).ToList();
            }
            else if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
            {
                var UsuDC = db.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 1210 && x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO).ToList();

                if (UsuDC.Count() > 0)
                {
                    foreach (var obj_UsuarioDC in UsuDC)
                    {
                        int ID_PERS_ENTI = Convert.ToInt32(obj_UsuarioDC.VAL_ACCO_PARA);

                        if (Convert.ToInt32(Session["ID_PERS_ENTI"]) == ID_PERS_ENTI)
                        {
                            var notis = db.ObtenerComentariosNotificacionxUsuario(Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["ID_ACCO"])).ToList();
                            return Json(new { Data = notis, Count = notis.Count() }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            tick = tick.Where(x => x.ID_ACCO == 56 || x.ID_ACCO == 57 || x.ID_ACCO == 58).ToList();
                        }
                    }
                }
                else
                {
                    tick = tick.Where(x => x.ID_ACCO == 56 || x.ID_ACCO == 57 || x.ID_ACCO == 58).ToList();
                }
            }
            else
            {
                tick = tick.Where(x => x.ID_ACCO == ID_ACCO).ToList();//tick;
            }

            var query = tick;

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CambiarEstadoNotificacion(int idTick)
        {
            try
            {
                db.CambiarEstadoNotificacionxUserTicket(Convert.ToInt32(Session["UserId"]), idTick);
                return Content("OK");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult viewListProduct(int id = 0)
        {
            ViewBag.ID_COMP = id;
            return View();
        }

        public ActionResult ProductoSerieOP()
        {
            return View();
        }

        public ActionResult verTicketPadreHijo(int id = 0)
        {
            ViewBag.ID_TICK = id;
            ViewBag.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            return View();

        }

        public JsonResult ListarTicketsPadreHijo(int idTicket)
        {
            var listaTicketsPadre = db.tktListarTicketPadreHijo(idTicket, 1).ToList();
            var listaTicketsHijo = db.tktListarTicketPadreHijo(idTicket, 2).ToList();
            var listaTicketHermano = db.tktListarTicketPadreHijo(idTicket, 3).ToList();

            return Json(new { ticketPadre = listaTicketsPadre, ticketHijo = listaTicketsHijo, ticketHermano = listaTicketHermano }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarTareasPendientes(int idTicket)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            try
            {
                DateTime fec = Convert.ToDateTime("03/15/2023");

                var TICKET = db.TICKETs.Where(x => x.ID_TICK == idTicket).FirstOrDefault();

                if (TICKET.ID_CATE == 27158 || TICKET.ID_CATE == 28529 || TICKET.ID_CATE == 29900)
                {
                    var listaTareasPendientes = db.ListarTareasMovil(idTicket, IdPersEnti).ToList();
                    return Json(new { tareaPendiente = listaTareasPendientes }, JsonRequestBehavior.AllowGet);
                }
                else if (TICKET.ID_CATE == 32465 || TICKET.ID_CATE == 32466 || TICKET.ID_CATE == 32467)
                {
                    var listaTareasPendientes = db.ListarTareasContable(idTicket, IdPersEnti).ToList();
                    return Json(new { tareaPendiente = listaTareasPendientes }, JsonRequestBehavior.AllowGet);
                }

                DateTime fecticket = Convert.ToDateTime(TICKET.FEC_INI_TICK);

                if (fecticket > fec)
                {
                    var listaTareasPendientes = db.ListaTarea2(idTicket, IdPersEnti).ToList();
                    return Json(new { tareaPendiente = listaTareasPendientes }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var listaTareasPendientes = db.ListaTarea(idTicket, IdPersEnti).ToList();
                    return Json(new { tareaPendiente = listaTareasPendientes }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                var listaTareasPendientes = db.ListaTarea(idTicket, IdPersEnti).ToList();
                return Json(new { tareaPendiente = listaTareasPendientes }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ListarTareasMinsur(int idTicket)
        {
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            try
            {
                var listaTareasPendientes = db.ListaTareasConfigurables(idTicket, IdPersEnti).ToList();
                return Json(listaTareasPendientes, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ListaEstadosTareaMinsur(int idTarea)
        {
            try
            {
                var listaEstadosTarea = db.ListaEstadosTarea(idTarea).ToList();
                return Json(listaEstadosTarea, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ListaAsignadosTareaMinsur(int idTarea)
        {
            try
            {
                int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
                var listaAsignadosTarea = dbe.ListarAsignadosTareasxCategory(idTarea, IdCuenta).ToList();
                return Json(listaAsignadosTarea, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CambiarEstadoTareaMinsur(int id, int estado)
        {
            int usuario = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            try
            {
                db.CambiarEstadoTarea(id, estado, usuario);
                return Content("REFRESH");
            }
            catch (Exception e)
            {
                return Content("ERROR");
            }
        }

        [HttpPost]
        public ActionResult AsignarTareaMinsur(int id, int asignado)
        {
            int usuario = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            try
            {
                db.AsignarTarea(id, asignado, usuario);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content("ERROR");
            }
        }

        [Authorize]
        public ActionResult ReporteTicketTarea(int id = 0)
        {
            ViewBag.ID_TICK = id;
            return View();
        }

        [Authorize]
        public ActionResult ReporteTareas()
        {
            bool esGestorTareas = Convert.ToBoolean(Session["GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA"]);
            if (esGestorTareas == true)
                ViewBag.esGestorTareas = 1;
            else
                ViewBag.esGestorTareas = 0;

            return View();
        }

        /*ERP*/
        [Authorize]
        public ActionResult ReporteTicketsBNV(string id = "")
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (ID_ACCO == 4)
            {
                return RedirectToAction("ReporteTicketsEdata", "Ticket");
            }
            else if (ID_ACCO == 1 || ID_ACCO == 25)
            {
                return RedirectToAction("ReporteTicketsGF", "Ticket");
            }
            else if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
            {
                return RedirectToAction("ReporteTicketsMINSUR", "Ticket");
            }

            int evento = 0;
            
            if (id == "Evento" || id == "EVENTO")
                evento = 1;

            ViewBag.Evento = evento;
            return View();
        }

        public ActionResult ReporteTicketsMINSUR()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (ID_ACCO == 4)
            {
                return RedirectToAction("ReporteTicketsEdata", "Ticket");
            }
            else if (ID_ACCO == 1 || ID_ACCO == 25)
            {
                return RedirectToAction("ReporteTicketsGF", "Ticket");
            }
            else if (ID_ACCO == 60)
            {
                return RedirectToAction("ReporteTicketsBNV", "Ticket");
            }
            return View();
        }
        public ActionResult ReporteTicketsEdata()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
            {
                return RedirectToAction("ReporteTicketsMINSUR", "Ticket");
            }
            else if (ID_ACCO == 1 || ID_ACCO == 25)
            {
                return RedirectToAction("ReporteTicketsGF", "Ticket");
            }
            else if (ID_ACCO == 60)
            {
                return RedirectToAction("ReporteTicketsBNV", "Ticket");
            }
            return View();
        }

        public ActionResult ReporteTicketsGF()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
            {
                return RedirectToAction("ReporteTicketsMINSUR", "Ticket");
            }
            else if (ID_ACCO == 4)
            {
                return RedirectToAction("ReporteTicketsEdata", "Ticket");
            }
            else if (ID_ACCO == 60)
            {
                return RedirectToAction("ReporteTicketsBNV", "Ticket");
            }
            return View();
        }

        public ActionResult ListaReporteTicketBNV(int estado = 0)
        {
            DateTime fechaFin = DateTime.Now.AddDays(1);  // Día de mañana
            DateTime fechaInicio = DateTime.Now.AddMonths(-2);  // Hace 2 meses atrás
            List<TicketReportBNV_Result> tareas = db.TicketReportBNV(60, fechaInicio, fechaFin, 0, 0, "", 0,"")
                  //.OrderByDescending(x => x.ID_TICK)
                  .Take(900)
                  .ToList();

            return Json(new { data = tareas }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListadoReporteTicketsBNV()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            int idqueu = 0;
            if (Request.Params["ID_QUEU"] != "")
            {
                idqueu = int.Parse(Convert.ToString(Request.Params["ID_QUEU"]));
            }
         

            string estados = Request.Params["Estados"];

            string FechaI = Convert.ToString(Request.Params["FechaInicio"]);
            string FechaF = Convert.ToString(Request.Params["FechaFin"]);


            List<TicketReportBNV_Result> resultCompleto;

            DateTime FechaInicio = DateTime.ParseExact("06/02/2000", "MM/dd/yyyy", null);
            DateTime FechaFin = DateTime.ParseExact("06/02/2000", "MM/dd/yyyy", null);

            if (Request.Params["FechaInicio"] != "" && Request.Params["FechaFin"] != "")
            {
                FechaInicio = Convert.ToDateTime(FechaI);
                FechaFin = Convert.ToDateTime(FechaF);
            }
            if (ID_ACCO == 60)
            {
                int idLoca = 0;
                if (Request.Params["ID_LOCA"] != "")
                {
                    idLoca = Convert.ToInt32(Request.Params["ID_LOCA"]);
                }
                string type_ticket = Convert.ToString(Request.Params["Type_ticket"]);

                resultCompleto = db.TicketReportBNV(60, FechaInicio, FechaFin, 0, idqueu, estados, idLoca, type_ticket).OrderByDescending(x => x.ID_TICK).Take(900).ToList();
                //return Json(new { data = resultCompleto }, JsonRequestBehavior.AllowGet);
                var jsonResult = Json(new { data = resultCompleto }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else if (ID_ACCO == 4)
            {
                string opcion = Request.Params["opcion"];
                opcion = string.IsNullOrEmpty(opcion) ? null : opcion;
                List<TicketReportEdta_Result> resultEdata;
                resultEdata = db.TicketReportEdta(4, FechaInicio, FechaFin, opcion, idqueu, estados).OrderByDescending(x => x.ID_TICK).ToList();
                return Json(new { data = resultEdata }, JsonRequestBehavior.AllowGet);

            }
            else if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
            {
                List<TicketReportMSR_Result> resultMinsur;
                resultMinsur = db.TicketReportMSR(ID_ACCO, FechaInicio, FechaFin, 0, idqueu, estados).OrderByDescending(x => x.ID_TICK).ToList();
                return Json(new { data = resultMinsur }, JsonRequestBehavior.AllowGet);
            }
            else if (ID_ACCO == 1 || ID_ACCO == 25)
            {
                List<TicketReportGF_Result> resultMinsur;
                resultMinsur = db.TicketReportGF(ID_ACCO, FechaInicio, FechaFin, 0, idqueu, estados).OrderByDescending(x => x.ID_TICK).ToList();
                return Json(new { data = resultMinsur }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = new List<object>() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListaGrupoResolutorPorCuenta()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = db.ObtenerGrupoResolutorPorCuenta(ID_ACCO).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarReporteTicketGF()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int idqueu = 0;
            if (Request.Params["ID_QUEU"] != "")
            {
                idqueu = int.Parse(Convert.ToString(Request.Params["ID_QUEU"]));
            }

            string FechaI = Convert.ToString(Request.Params["FechaInicial"]);
            string FechaF = Convert.ToString(Request.Params["FechaFinal"]);

            string estados = Request.Params["Estados"];

            DateTime FechaInicio = Convert.ToDateTime(FechaI);
            DateTime FechaFin = Convert.ToDateTime(FechaF);


            DataTable dataTable = ObtenerDatosDesdeTbTicketGF(FechaInicio, FechaFin, idqueu, ID_ACCO, estados);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Crear el paquete de Excel y llenar la hoja de cálculo
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TicketReport");
                var fechaColumn = ws.Column(3);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(4);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(18);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(19);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";

                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy HH:mm tt";
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 17;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(6).Width = 25;
                ws.Column(7).Width = 30;
                ws.Column(8).Width = 35;
                ws.Column(9).Width = 35;
                ws.Column(10).Width = 35;
                ws.Column(11).Width = 25;
                ws.Column(12).Width = 30;
                ws.Column(13).Width = 40;
                ws.Column(14).Width = 40;
                ws.Column(15).Width = 15;
                ws.Column(16).Width = 20;
                ws.Column(17).Width = 25;
                ws.Column(18).Width = 25;
                ws.Column(19).Width = 25;

                ws.Column(20).Width = 20;
                ws.Column(21).Width = 40;
                ws.Column(22).Width = 40;
                ws.Column(23).Width = 40;
                ws.Column(24).Width = 40;
                ws.Column(25).Width = 40;
                ws.Column(26).Width = 90;

                ws.Column(27).Width = 25;
                ws.Column(28).Width = 80;
                ws.Column(29).Width = 40;
                ws.Column(30).Width = 90;
                ws.Column(31).Width = 60;
                ws.Column(32).Width = 18;

                ws.Column(33).Width = 20;
                ws.Column(34).Width = 25;
                ws.Column(35).Width = 20;
                ws.Column(36).Width = 20;
                ws.Column(37).Width = 10;
                ws.Column(38).Width = 10;

                ws.Column(39).Width = 15;
                ws.Column(40).Width = 20;
                ws.Column(41).Width = 20;

                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);

                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=TicketReport.xlsx");

                ms.WriteTo(HttpContext.Response.OutputStream);
                HttpContext.Response.End();
            }

            return View();
        }
        public ActionResult ExportarReporteTicketEdata()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int idqueu = 0;
            if (Request.Params["ID_QUEU"] != "")
            {
                idqueu = int.Parse(Convert.ToString(Request.Params["ID_QUEU"]));
            }

            string FechaI = Convert.ToString(Request.Params["FechaInicial"]);
            string FechaF = Convert.ToString(Request.Params["FechaFinal"]);
            string opcion = Request.Params["opcion"];
            if (opcion == "NULL")
            {
                opcion = null;
            }

            string estados = Request.Params["Estados"];

            DateTime FechaInicio = Convert.ToDateTime(FechaI);
            DateTime FechaFin = Convert.ToDateTime(FechaF);


            DataTable dataTable = ObtenerDatosDesdeTbTicketEdata(FechaInicio, FechaFin, idqueu, opcion, estados);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Crear el paquete de Excel y llenar la hoja de cálculo
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ReporteTicket");
                var fechaColumn = ws.Column(4);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(5);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(19);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(20);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(35);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 17;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(6).Width = 25;
                ws.Column(7).Width = 30;
                ws.Column(8).Width = 35;
                ws.Column(9).Width = 35;
                ws.Column(10).Width = 35;
                ws.Column(11).Width = 25;
                ws.Column(12).Width = 30;
                ws.Column(13).Width = 40;
                ws.Column(14).Width = 40;
                ws.Column(15).Width = 15;
                ws.Column(16).Width = 20;
                ws.Column(17).Width = 25;
                ws.Column(18).Width = 25;
                ws.Column(19).Width = 25;

                ws.Column(20).Width = 20;
                ws.Column(21).Width = 40;
                ws.Column(22).Width = 40;
                ws.Column(23).Width = 40;
                ws.Column(24).Width = 40;
                ws.Column(25).Width = 40;
                ws.Column(26).Width = 90;

                ws.Column(27).Width = 25;
                ws.Column(28).Width = 100;
                ws.Column(29).Width = 50;
                ws.Column(30).Width = 60;
                ws.Column(31).Width = 25;
                ws.Column(32).Width = 15;

                ws.Column(33).Width = 15;
                ws.Column(34).Width = 25;
                ws.Column(35).Width = 20;
                ws.Column(36).Width = 18;
                ws.Column(37).Width = 15;
                ws.Column(38).Width = 12;

                ws.Column(39).Width = 14;
                ws.Column(40).Width = 20;
                ws.Column(41).Width = 20;
                ws.Column(42).Width = 18;
                ws.Column(43).Width = 18;
                ws.Column(44).Width = 60;
                ws.Column(45).Width = 60;
                ws.Column(46).Width = 60;
                ws.Column(47).Width = 60;
                ws.Column(48).Width = 60;

                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);

                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=TicketReport.xlsx");

                ms.WriteTo(HttpContext.Response.OutputStream);
                HttpContext.Response.End();
            }

            return View();
        }
        public ActionResult ExportarReporteTicketMINSUR()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int idqueu = 0;
            if (Request.Params["ID_QUEU"] != "")
            {
                idqueu = int.Parse(Convert.ToString(Request.Params["ID_QUEU"]));
            }

            string FechaI = Convert.ToString(Request.Params["FechaInicial"]);
            string FechaF = Convert.ToString(Request.Params["FechaFinal"]);

            string estados = Request.Params["Estados"];

            DateTime FechaInicio = Convert.ToDateTime(FechaI);
            DateTime FechaFin = Convert.ToDateTime(FechaF);


            DataTable dataTable = ObtenerDatosDesdeTbTicketMinsur(FechaInicio, FechaFin, idqueu, ID_ACCO, estados);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Crear el paquete de Excel y llenar la hoja de cálculo
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TicketReportMinsur");
                var fechaColumn = ws.Column(3);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(4);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(5);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(6);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(8);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(22);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 17;
                ws.Column(3).Width = 20;

                ws.Column(4).Width = 40;
                ws.Column(5).Width = 40;
                ws.Column(6).Width = 40;
                ws.Column(7).Width = 40;

                ws.Column(8).Width = 20;
                ws.Column(10).Width = 25;
                ws.Column(11).Width = 30;
                ws.Column(12).Width = 25;
                ws.Column(13).Width = 35;
                ws.Column(14).Width = 35;
                ws.Column(15).Width = 25;
                ws.Column(16).Width = 30;
                ws.Column(17).Width = 25;
                ws.Column(18).Width = 25;
                ws.Column(19).Width = 15;
                ws.Column(20).Width = 15;
                ws.Column(21).Width = 17;
                ws.Column(22).Width = 20;
                ws.Column(23).Width = 20;

                ws.Column(24).Width = 20;
                ws.Column(25).Width = 40;
                ws.Column(27).Width = 40;
                ws.Column(28).Width = 40;
                ws.Column(29).Width = 40;
                ws.Column(30).Width = 25;

                ws.Column(31).Width = 35;
                ws.Column(32).Width = 90;
                ws.Column(33).Width = 25;
                ws.Column(34).Width = 90;
                ws.Column(35).Width = 60;
                ws.Column(36).Width = 60;

                ws.Column(37).Width = 20;
                ws.Column(38).Width = 18;
                ws.Column(39).Width = 20;
                ws.Column(40).Width = 30;
                ws.Column(41).Width = 10;
                ws.Column(42).Width = 10;

                ws.Column(43).Width = 15;
                ws.Column(44).Width = 20;
                ws.Column(45).Width = 15;
                ws.Column(46).Width = 20;


                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);

                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=TicketReportMinsur.xlsx");

                ms.WriteTo(HttpContext.Response.OutputStream);
                HttpContext.Response.End();
            }

            return View();
        }

        public ActionResult ExportarReporteTicket()
        {
            int idqueu = 0;
            if (Request.Params["ID_QUEU"] != "")
            {
                idqueu = int.Parse(Convert.ToString(Request.Params["ID_QUEU"]));
            }

            int idLoca = 0;
            if (Request.Params["ID_LOCA"] != "")
            {
                idLoca = Convert.ToInt32(Request.Params["ID_LOCA"]);
            }

            string FechaI = Convert.ToString(Request.Params["FechaInicial"]);
            string FechaF = Convert.ToString(Request.Params["FechaFinal"]);

            string estados = Request.Params["Estados"];

            DateTime FechaInicio = Convert.ToDateTime(FechaI);
            DateTime FechaFin = Convert.ToDateTime(FechaF);
            string Type_ticket = Convert.ToString(Request.Params["ID_TYPE_TICKET"]);

            DataTable dataTable = ObtenerDatosDesdeTbTicketBuenaventura(FechaInicio, FechaFin, idqueu, estados, idLoca, Type_ticket);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string nombreArchivo = "ReporteTicketBuenaventura.xlsx";

            // Crear el paquete de Excel y llenar la hoja de cálculo
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ReporteTicketBuenaventura");
                var fechaColumn = ws.Column(3);

                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 17;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 20;
                ws.Column(7).Width = 30;
                ws.Column(8).Width = 35;
                ws.Column(9).Width = 35;
                ws.Column(10).Width = 35;
                ws.Column(11).Width = 25;
                ws.Column(12).Width = 15;
                ws.Column(13).Width = 30;
                ws.Column(14).Width = 40;
                ws.Column(15).Width = 40;
                ws.Column(16).Width = 15;
                ws.Column(17).Width = 20;
                ws.Column(18).Width = 25;
                ws.Column(19).Width = 25;

                ws.Column(20).Width = 25;
                ws.Column(21).Width = 25;
                ws.Column(22).Width = 25;

                ws.Column(23).Width = 20;
                ws.Column(24).Width = 40;
                ws.Column(25).Width = 40;
                ws.Column(26).Width = 40;
                ws.Column(27).Width = 40;
                ws.Column(28).Width = 40;
                ws.Column(29).Width = 25;
                ws.Column(30).Width = 25;
                ws.Column(31).Width = 30;

                ws.Column(32).Width = 90;
                ws.Column(33).Width = 80;
                ws.Column(34).Width = 40;
                ws.Column(35).Width = 90;
                ws.Column(36).Width = 60;
                ws.Column(37).Width = 60;

                ws.Column(38).Width = 10;
                ws.Column(39).Width = 25;
                ws.Column(40).Width = 20;
                ws.Column(41).Width = 20;
                ws.Column(42).Width = 10;
                ws.Column(43).Width = 10;

                ws.Column(44).Width = 15;
                ws.Column(45).Width = 20;
                ws.Column(46).Width = 15;
                ws.Column(47).Width = 20;
                ws.Column(48).Width = 20;
                ws.Column(49).Width = 20;
                ws.Column(50).Width = 20;
                ws.Column(51).Width = 20;
                ws.Column(52).Width = 20;
                ws.Column(53).Width = 80;
                ws.Column(54).Width = 25;
                ws.Column(55).Width = 25;

                ws.Column(56).Width = 20;
                ws.Column(57).Width = 20;
                ws.Column(58).Width = 40;
                ws.Column(59).Width = 20;
                ws.Column(60).Width = 40;
                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);

                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=ReporteTicketBuenaventura.xlsx");

                ms.WriteTo(HttpContext.Response.OutputStream);
                HttpContext.Response.End();
            }

            return View();
        }


        private DataTable ObtenerDatosDesdeTbTicketBuenaventura(DateTime FechaInicio, DateTime FechaFin, int idqueu, string estados, int idLoca, string type_ticket)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("TicketReportBuenaventura", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@ID_ACCO", 60);
                    cmd.Parameters.AddWithValue("@FROM_DATE", FechaInicio);
                    cmd.Parameters.AddWithValue("@TO_DATE", FechaFin);
                    cmd.Parameters.AddWithValue("@SubCuenta", 0);
                    cmd.Parameters.AddWithValue("@IdArea", idqueu);
                    cmd.Parameters.AddWithValue("@Estados", estados);
                    cmd.Parameters.AddWithValue("@IdLoca", idLoca);
                    cmd.Parameters.AddWithValue("@Type_ticket", type_ticket);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        private DataTable ObtenerDatosDesdeTbTicketMinsur(DateTime FechaInicio, DateTime FechaFin, int idqueu, int ID_ACCO, string estados)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("TicketReportMINSUR", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@ID_ACCO", ID_ACCO);
                    cmd.Parameters.AddWithValue("@FROM_DATE", FechaInicio);
                    cmd.Parameters.AddWithValue("@TO_DATE", FechaFin);
                    cmd.Parameters.AddWithValue("@SubCuenta", 0);
                    cmd.Parameters.AddWithValue("@IdArea", idqueu);
                    cmd.Parameters.AddWithValue("@Estados", estados);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        private DataTable ObtenerDatosDesdeTbTicketEdata(DateTime FechaInicio, DateTime FechaFin, int idqueu, string Subcuenta, string estados)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("TicketReportEdata", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@ID_ACCO", 4);
                    cmd.Parameters.AddWithValue("@FROM_DATE", FechaInicio);
                    cmd.Parameters.AddWithValue("@TO_DATE", FechaFin);
                    cmd.Parameters.AddWithValue("@SubCuenta", (object)Subcuenta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdArea", idqueu);
                    cmd.Parameters.AddWithValue("@Estados", estados);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        private DataTable ObtenerDatosDesdeTbTicketGF(DateTime FechaInicio, DateTime FechaFin, int idqueu, int ID_ACCO, string estados)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("TicketReportGLF", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@ID_ACCO", ID_ACCO);
                    cmd.Parameters.AddWithValue("@FROM_DATE", FechaInicio);
                    cmd.Parameters.AddWithValue("@TO_DATE", FechaFin);
                    cmd.Parameters.AddWithValue("@SubCuenta", 0);
                    cmd.Parameters.AddWithValue("@IdArea", idqueu);
                    cmd.Parameters.AddWithValue("@Estados", estados);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }



        public ActionResult ListadoReporteTareaXTicket()
        {
            int idticket = int.Parse(Convert.ToString(Request.Params["ID_TICK"]));

            var result = db.ReporteTareasxTicket(idticket).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportarReporteTarea()
        {
            int idqueu = 0;
            if (Request.Params["ID_QUEU"] != "")
            {
                idqueu = int.Parse(Convert.ToString(Request.Params["ID_QUEU"]));
            }

            int idestado = 0;
            if (Request.Params["ID_ESTADO"] != "")
            {
                idestado = int.Parse(Convert.ToString(Request.Params["ID_ESTADO"]));
            }

            int idloca = 0;

            if (Request.Params["ID_LOCA"] != "")
            {
                idloca = int.Parse(Convert.ToString(Request.Params["ID_LOCA"]));

            }

            string FechaI = Convert.ToString(Request.Params["FechaInicial"]);
            string FechaF = Convert.ToString(Request.Params["FechaFinal"]);


            DateTime FechaInicio = Convert.ToDateTime(FechaI);
            DateTime FechaFin = Convert.ToDateTime(FechaF);

            string CodTarea = Convert.ToString(Request.Params["COD_TAREA"]);
            var IdIntermeMinsur = Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMinsur"]);
            var IdIntermeMarcobre = Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMarcobre"]);
            var IdIntermeRaura = Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeRaura"]);

            var categorias = new int[]
            {
                21265, 21296, 21300, 20813, 27009, 28380, 29751,
                27011, 28382, 29753, IdIntermeMinsur, IdIntermeMarcobre,
                IdIntermeRaura, 27158, 28529, 29900, 32465, 32466, 32467
            };
            var tareasSAP = db.CATEGORies.Where(x => x.TieneTarea == true).Select(c => c.ID_CATE).ToList();
            var tareasModulo = db.CategoriaConTareas.Select(c => c.IdCate).ToList();

            //tercer cambio
            var lst_Tickets = (from t in db.TICKETs.Where(x => (categorias.Contains(x.ID_CATE ?? 0) || tareasSAP.Contains(x.ID_CATE ?? 0) || tareasModulo.Contains(x.ID_CATE ?? 0))
                               && (x.ID_ACCO == 56 || x.ID_ACCO == 57 || x.ID_ACCO == 58) && x.FEC_INI_TICK >= FechaInicio && x.FEC_INI_TICK <= FechaFin && (x.ID_PERS_ENTI_ASSI != null || x.ID_PERS_ENTI_ASSI != 0))
                               select new
                               {
                                   ID_TICK = t.ID_TICK
                               }).Distinct();

            int cant_ticket = lst_Tickets.Count();

            List<ExportarReporteTareasxTicket_Result> result1 = new List<ExportarReporteTareasxTicket_Result>();
            List<ExportarReporteTareasxTicket_Result> resultComplet = new List<ExportarReporteTareasxTicket_Result>();

            foreach (var obj_TICKETS in lst_Tickets)
            {

                result1 = db.ExportarReporteTareasxTicket(obj_TICKETS.ID_TICK).Where(x => (x.IdLoca == idloca || idloca == 0) && (x.IdQueu == idqueu || idqueu == 0) && (x.IdEstado == idestado || idestado == 0) && (x.Cod_Tarea == CodTarea || CodTarea == "")).ToList();
                resultComplet = resultComplet.Concat(result1).Distinct().ToList();

            }
            if (idqueu != 0 || idloca != 0 || idestado != 0)
            {
                var resultCompleto = resultComplet.Where(x => x.IdQueu == idqueu || x.IdLoca == idloca || x.IdEstado == idestado || x.Cod_Tarea == CodTarea).ToList();
                StringBuilder sb = new StringBuilder();
                int i = 0;
                sb.Append("<meta charset='utf-8'>");
                sb.Append("<style type='text/css'>");
                sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
                sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:100px 80px;border-style:solid red 2px;border-width:1px;overflow:hidden;word-break:normal;border-color:#0000;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".cabecera{background:rgb(232, 229, 0);color:black;font-family:Arial, sans-serif;font-size:12px;font-weight:normal;padding:40px 100px;border:red solid 2px;border-width:1px;overflow:hidden;word-break:normal;border-color:#000000;border-top-width:2px;border-bottom-width:1px;}");
                sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#000000;border-top-width:2px;border-bottom-width:2px;background:#F7FDFA;margin:5px 10px;}");
                sb.Append("</style>");
                sb.Append("<table class='tg'>");
                sb.Append("<tr>");
                sb.Append("<th class='cabecera'>Codigo Tarea</td>");
                sb.Append("<th class='cabecera'>Codigo Ticket</td>");
                sb.Append("<th class='cabecera'>Asignado a</td>");
                sb.Append("<th class='cabecera'>Tipo de Gestión</td>");
                sb.Append("<th class='cabecera'>Descripcion de Tarea</td>");
                sb.Append("<th class='cabecera'>Area Responsable</td>");
                sb.Append("<th class='cabecera'>Estado</td>");
                sb.Append("<th class='cabecera'>Sede</td>");
                sb.Append("<th class='cabecera'>Usuario afectado</td>");
                sb.Append("<th class='cabecera'>Fecha de Creación</td>");
                sb.Append("<th class='cabecera'>Realizado por</td>");
                sb.Append("<th class='cabecera'>Fecha de Realización</td>");

                sb.Append("</tr>");

                foreach (ExportarReporteTareasxTicket_Result dr in resultCompleto)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.Cod_Tarea + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Codigo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Asignado + "</td>");
                    sb.Append("<td class='contenido'>" + dr.TipoGestion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Descripcion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.AreaResponsable + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Estado + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Sede + "</td>");
                    sb.Append("<td class='contenido'>" + dr.UsuarioAfectado + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaCreacion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.UsuarioSesion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaFin + "</td>");

                    sb.Append("</tr>");
                }
                sb.Append("</table>");
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment;filename=reportetarea" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();

                return View();
            }
            else
            {
                var resultCompleto = resultComplet.ToList();

                StringBuilder sb = new StringBuilder();
                int i = 0;
                sb.Append("<meta charset='utf-8'>");
                sb.Append("<style type='text/css'>");
                sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
                sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:100px 80px;border-style:solid red 2px;border-width:1px;overflow:hidden;word-break:normal;border-color:#0000;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".cabecera{background:rgb(232, 229, 0);color:black;font-family:Arial, sans-serif;font-size:12px;font-weight:normal;padding:40px 100px;border:red solid 2px;border-width:1px;overflow:hidden;word-break:normal;border-color:#000000;border-top-width:2px;border-bottom-width:1px;}");
                sb.Append(".contenido{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#000000;border-top-width:2px;border-bottom-width:2px;background:#F7FDFA;margin:5px 10px;}");
                sb.Append("</style>");
                sb.Append("<table class='tg'>");
                sb.Append("<tr>");
                sb.Append("<th class='cabecera'>Codigo Tarea</td>");
                sb.Append("<th class='cabecera'>Codigo Ticket</td>");
                sb.Append("<th class='cabecera'>Asignado a</td>");
                sb.Append("<th class='cabecera'>Tipo de Gestión</td>");
                sb.Append("<th class='cabecera'>Descripcion de Tarea</td>");
                sb.Append("<th class='cabecera'>Area Responsable</td>");
                sb.Append("<th class='cabecera'>Estado</td>");
                sb.Append("<th class='cabecera'>Sede</td>");
                sb.Append("<th class='cabecera'>Usuario afectado</td>");
                sb.Append("<th class='cabecera'>Fecha de Creación</td>");
                sb.Append("<th class='cabecera'>Realizado por</td>");
                sb.Append("<th class='cabecera'>Fecha de Realización</td>");

                sb.Append("</tr>");

                foreach (ExportarReporteTareasxTicket_Result dr in resultCompleto)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='contenido'>" + dr.Cod_Tarea + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Codigo + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Asignado + "</td>");
                    sb.Append("<td class='contenido'>" + dr.TipoGestion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Descripcion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.AreaResponsable + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Estado + "</td>");
                    sb.Append("<td class='contenido'>" + dr.Sede + "</td>");
                    sb.Append("<td class='contenido'>" + dr.UsuarioAfectado + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaCreacion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.UsuarioSesion + "</td>");
                    sb.Append("<td class='contenido'>" + dr.FechaFin + "</td>");

                    sb.Append("</tr>");
                }
                sb.Append("</table>");
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment;filename=reportetarea" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();

                return View();
            }

        }


        //public ActionResult ExportarReporteTarea()
        //{
        //    int idqueu = 0;
        //    if (Request.Params["ID_QUEU"] != "")
        //    {
        //        idqueu = int.Parse(Convert.ToString(Request.Params["ID_QUEU"]));
        //    }

        //    int idestado = 0;
        //    if (Request.Params["ID_ESTADO"] != "")
        //    {
        //        idestado = int.Parse(Convert.ToString(Request.Params["ID_ESTADO"]));
        //    }

        //    int idloca = 0;

        //    if (Request.Params["ID_LOCA"] != "")
        //    {
        //        idloca = int.Parse(Convert.ToString(Request.Params["ID_LOCA"]));

        //    }

        //    string FechaI = Convert.ToString(Request.Params["FechaInicial"]);
        //    string FechaF = Convert.ToString(Request.Params["FechaFinal"]);


        //    DateTime FechaInicio = Convert.ToDateTime(FechaI);
        //    DateTime FechaFin = Convert.ToDateTime(FechaF);

        //    string CodTarea = Convert.ToString(Request.Params["codigotarea"]);
        //    //tercer cambio
        //    var lst_Tickets = (from t in db.TICKETs.Where(x => (x.ID_CATE == 21265 || x.ID_CATE == 21296 || x.ID_CATE == 21300 || x.ID_CATE == 20813 || x.ID_CATE == 27009 || x.ID_CATE == 28380 || x.ID_CATE == 29751 || x.ID_CATE == 27011 || x.ID_CATE == 28382 || x.ID_CATE == 29753)
        //                       && (x.ID_ACCO == 56 || x.ID_ACCO == 57 || x.ID_ACCO == 58) && x.FEC_INI_TICK >= FechaInicio && x.FEC_INI_TICK <= FechaFin && (x.ID_PERS_ENTI_ASSI != null || x.ID_PERS_ENTI_ASSI != 0))
        //                       select new
        //                       {
        //                           ID_TICK = t.ID_TICK
        //                       }).Distinct();

        //    int cant_ticket = lst_Tickets.Count();

        //    List<ReporteTareasxTicket_Result> result1 = new List<ReporteTareasxTicket_Result>();
        //    List<ReporteTareasxTicket_Result> resultComplet = new List<ReporteTareasxTicket_Result>();

        //    foreach (var obj_TICKETS in lst_Tickets)
        //    {

        //        result1 = db.ReporteTareasxTicket(obj_TICKETS.ID_TICK).Where(x => (x.IdLoca == idloca || idloca == 0) && (x.IdQueu == idqueu || idqueu == 0) && (x.IdEstado == idestado || idestado == 0) && (x.Cod_Tarea == CodTarea || CodTarea == "")).ToList();
        //        resultComplet = resultComplet.Concat(result1).Distinct().ToList();

        //    }
        //    if (idqueu != 0 || idloca != 0 || idestado != 0)
        //    {
        //        var resultCompleto = resultComplet.Where(x => x.IdQueu == idqueu || x.IdLoca == idloca || x.IdEstado == idestado || x.Cod_Tarea == CodTarea).ToList();
        //        GridView gridView = new GridView();
        //        gridView.DataSource = resultCompleto;
        //        gridView.DataBind();

        //        gridView.HeaderRow.BackColor = System.Drawing.Color.FromName("DarkBlue");
        //        gridView.HeaderRow.ForeColor = System.Drawing.Color.White;

        //        for (int i = 0; i < gridView.HeaderRow.Cells.Count; i++)
        //        {
        //            gridView.HeaderRow.Cells[i].BackColor = System.Drawing.Color.FromName("DarkBlue");
        //            gridView.HeaderRow.Cells[i].ForeColor = System.Drawing.Color.White;
        //        }

        //        foreach (GridViewRow row in gridView.Rows)
        //        {
        //            row.BackColor = System.Drawing.Color.White;
        //            row.Cells[0].Style.Add("text-align", "center");
        //            row.Cells[1].Style.Add("text-align", "center");
        //        }

        //        Response.ClearContent();
        //        Response.Buffer = true;
        //        Response.AddHeader("content-disposition", "attachment; filename=ReporteTareas.xlsx");
        //        Response.ContentType = "application/vnd.ms-excel";

        //        using (StringWriter sw = new StringWriter())
        //        {
        //            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
        //            {
        //                gridView.RenderControl(htw);
        //                Response.Output.Write(sw.ToString());
        //                Response.Flush();
        //                Response.End();
        //            }
        //        }

        //        return View();
        //    }
        //    else
        //    {
        //        var resultCompleto = resultComplet.ToList();

        //        var gridView = new GridView();
        //        gridView.DataSource = resultCompleto;
        //        gridView.DataBind();

        //        if (gridView.HeaderRow != null)
        //        {
        //            // estilo de tabla
        //            gridView.HeaderRow.BackColor = System.Drawing.Color.FromName("DarkBlue");
        //            gridView.HeaderRow.ForeColor = System.Drawing.Color.White;


        //            for (int i = 0; i < gridView.HeaderRow.Cells.Count; i++)
        //            {


        //                gridView.HeaderRow.Cells[i].BackColor = System.Drawing.Color.FromName("DarkBlue");
        //                gridView.HeaderRow.Cells[i].ForeColor = System.Drawing.Color.White;
        //            }
        //        }

        //        // estilo de celdas
        //        foreach (GridViewRow row in gridView.Rows)
        //        {
        //            row.BackColor = System.Drawing.Color.White;
        //            row.Cells[0].Style.Add("text-align", "center");
        //            row.Cells[1].Style.Add("text-align", "center");
        //        }

        //        Response.ClearContent();
        //        Response.Buffer = true;
        //        Response.AddHeader("content-disposition", "attachment; filename=ReporteTareas" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        //        Response.ContentType = "application/vnd.ms-excel";
        //        Response.Charset = "";

        //        StringWriter sw = new StringWriter();
        //        HtmlTextWriter htw = new HtmlTextWriter(sw);

        //        gridView.RenderControl(htw);
        //        Response.Output.Write(sw.ToString());
        //        Response.Flush();
        //        Response.End();

        //        return View();
        //    }

        //}


        public ActionResult ListadoReporteTarea()
        {

            int idqueu = 0;
            if (Request.Params["ID_QUEU"] != "")
            {
                idqueu = int.Parse(Convert.ToString(Request.Params["ID_QUEU"]));
            }

            int idestado = 0;
            if (Request.Params["ID_ESTADO"] != "")
            {
                idestado = int.Parse(Convert.ToString(Request.Params["ID_ESTADO"]));
            }

            int idloca = 0;

            if (Request.Params["ID_LOCA"] != "")
            {
                idloca = int.Parse(Convert.ToString(Request.Params["ID_LOCA"]));

            }

            string FechaI = Convert.ToString(Request.Params["FechaInicio"]);
            string FechaF = Convert.ToString(Request.Params["FechaFin"]);


            DateTime FechaInicio = Convert.ToDateTime(FechaI);
            DateTime FechaFin = Convert.ToDateTime(FechaF);

            string CodTarea = Convert.ToString(Request.Params["codigotarea"]);
            var IdIntermeMinsur = Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMinsur"]);
            var IdIntermeMarcobre = Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMarcobre"]);
            var IdIntermeRaura = Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeRaura"]);

            var categorias = new int[]
            {
                21265, 21296, 21300, 20813, 27009, 28380, 29751,
                27011, 28382, 29753, IdIntermeMinsur, IdIntermeMarcobre,
                IdIntermeRaura, 27158, 28529, 29900, 32465, 32466, 32467
            };
            var tareasSAP = db.CATEGORies.Where(x => x.TieneTarea == true).Select(c => c.ID_CATE).ToList();
            var tareasModulo = db.CategoriaConTareas.Select(c => c.IdCate).ToList();

            //tercer cambio
            var lst_Tickets = (from t in db.TICKETs.Where(x => (categorias.Contains(x.ID_CATE ?? 0) || tareasSAP.Contains(x.ID_CATE ?? 0) || tareasModulo.Contains(x.ID_CATE ?? 0))
                               && (x.ID_ACCO == 56 || x.ID_ACCO == 57 || x.ID_ACCO == 58) && x.FEC_INI_TICK >= FechaInicio && x.FEC_INI_TICK <= FechaFin && (x.ID_PERS_ENTI_ASSI != null || x.ID_PERS_ENTI_ASSI != 0))
                               select new
                               {
                                   ID_TICK = t.ID_TICK
                               }).Distinct();

            int cant_ticket = lst_Tickets.Count();

            List<ReporteTareasxTicket_Result> result1 = new List<ReporteTareasxTicket_Result>();
            List<ReporteTareasxTicket_Result> resultComplet = new List<ReporteTareasxTicket_Result>();

            foreach (var obj_TICKETS in lst_Tickets)
            {

                result1 = db.ReporteTareasxTicket(obj_TICKETS.ID_TICK).Where(x => (x.IdLoca == idloca || idloca == 0) && (x.IdQueu == idqueu || idqueu == 0) && (x.IdEstado == idestado || idestado == 0) && (x.Cod_Tarea == CodTarea || CodTarea == "")).ToList();
                resultComplet = resultComplet.Concat(result1).Distinct().ToList();

            }
            if (idqueu != 0 || idloca != 0 || idestado != 0)
            {
                var resultCompleto = resultComplet.Where(x => x.IdQueu == idqueu || x.IdLoca == idloca || x.IdEstado == idestado || x.Cod_Tarea == CodTarea).Take(4000).ToList();
                return Json(new { data = resultCompleto }, JsonRequestBehavior.AllowGet); ;
            }
            else
            {
                var resultCompleto = resultComplet.Take(4000).ToList();
                return Json(new { data = resultCompleto }, JsonRequestBehavior.AllowGet); ;
            }

        }

        public ActionResult ReporteTareasSelecAsignado(int id = 0)
        {
            ViewBag.IdTareaDetalle = id;

            var query = db.TareaDetalle.Where(x => x.IdTareaDetalle == id).SingleOrDefault();

            if (query != null)
            {
                ViewBag.AreaResponsable = query.Id_Queu;
                ViewBag.Asignado = query.Id_Pers_Enti;
            }

            return View();
        }


        public ActionResult ListaPorCuenta()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = db.ACCOUNT_QUEUE.Where(aq => aq.ID_ACCO == ID_ACCO && aq.VIG_ACCO_QUEU == true);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = (from aq in query.ToList()
                          join q in db.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                          join a in db.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                          select new
                          {
                              QUEU = a.ACR_ACCO + '.' + q.NAM_QUEU,
                              ID_QUEU = q.ID_QUEU,
                              ID_PERS_ENTI = ID_PERS_ENTI,// assiSelect(q.ID_QUEU),
                              ID_STAT = q.ID_STAT,
                              DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToLower()),
                              ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU,
                              ID_PERS_ENTI_ASSI_DEFA = aq.ID_PERS_ENTI_ASSI_DEFA,
                          }).Where(x => x.ID_QUEU == 78 || x.ID_QUEU == 80 || x.ID_QUEU == 1 || x.ID_QUEU == 25).OrderBy(x => x.ORD_ACCO_QUEU);

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaPorCuentaReporte()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = db.ACCOUNT_QUEUE.Where(aq => aq.ID_ACCO == ID_ACCO && aq.VIG_ACCO_QUEU == true);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = (from aq in query.ToList()
                          join q in db.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                          join a in db.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                          select new
                          {
                              QUEU = a.ACR_ACCO + '.' + q.NAM_QUEU,
                              ID_QUEU = q.ID_QUEU,
                              ID_PERS_ENTI = ID_PERS_ENTI,// assiSelect(q.ID_QUEU),
                              ID_STAT = q.ID_STAT,
                              DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToLower()),
                              ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU,
                              ID_PERS_ENTI_ASSI_DEFA = aq.ID_PERS_ENTI_ASSI_DEFA,
                          }).Where(x => x.ID_QUEU == 78 || x.ID_QUEU == 80 || x.ID_QUEU == 1 || x.ID_QUEU == 25 || x.ID_QUEU == 9).OrderBy(x => x.ORD_ACCO_QUEU);

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaPorEstado()
        {

            var result = (from e in db.EstadoTarea
                          where e.Vigencia == true
                          select new
                          {
                              ID_ESTADO = e.IdEstado,
                              ESTADO = e.Estado
                          }).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsignadoPorCola(int idQueu)
        {
            int Id_Queu = Convert.ToInt32(idQueu);
            //int Id_Queu = Convert.ToInt32(Session["ID_QUEU"]);

            var query = dbe.AsignadosPorColaMinsurMarcobreRaura(Id_Queu).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GuardarColaborador(int ID_PERS_ENTI, int IdTareaDetalle)
        {
            var objTareaDetalle = db.TareaDetalle.SingleOrDefault(x => x.IdTareaDetalle == IdTareaDetalle);

            if (objTareaDetalle != null)
            {
                objTareaDetalle.Id_Pers_Enti = ID_PERS_ENTI;
                objTareaDetalle.IdEstado = 4;
                db.SaveChanges();

                return Json(new { Respuesta = "OK" });
            }

            return Json(new { Respuesta = "Error" });
        }

        public ActionResult ListTareaLocation()
        {

            var query = db.LOCATIONs;
            var result = (from x in query.ToList()
                          join s in db.SITEs.Where(x => x.ID_ACCO == 56 || x.ID_ACCO == 57 || x.ID_ACCO == 58) on x.ID_SITE equals s.ID_SITE
                          join a in db.ACCOUNTs.Where(x => x.ID_ACCO == 56 || x.ID_ACCO == 57 || x.ID_ACCO == 58) on s.ID_ACCO equals a.ID_ACCO
                          select new
                          {
                              ID_LOCA = x.ID_LOCA,
                              NAM_LOCA = a.NAM_ACCO + " : " + s.NAM_SITE + " - " + x.NAM_LOCA
                          }).Distinct().ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CrearTareaRealizada(int id, String descripcion, int idticket, String estado)
        {
            int existeTarea = db.TareaDetalle.Where(x => x.Tarea.DescripcionTarea == descripcion & x.Id_Tick == idticket).Count();

            int id_pers_enti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string userId = Convert.ToString(Session["UserId"]);

            int id_queu = Convert.ToInt32(Session["ID_QUEU"]);
            TICKET obj_Ticket = db.TICKETs.Where(x => x.ID_TICK == idticket).FirstOrDefault();
            int idcate = Convert.ToInt32(obj_Ticket.ID_CATE);

            if (existeTarea == 1)
            {
                //if (id_queu == 1 & (descripcion == "Registro de licencia Mcafee/ EDR/ SCCM" || descripcion == "Registro de licencia Absolute" || descripcion == "Eliminacón de Licencia Mcafee / EDR / SCCM"))
                //{
                //    TareaDetalle objTareaDetalleTransferido = db.TareaDetalle.Where(x => x.Tarea.DescripcionTarea == descripcion & x.Id_Tick == idticket).FirstOrDefault();
                //    TareaDetalle objTareaDetalle = new TareaDetalle();
                //    objTareaDetalle.Id_Tick = idticket;
                //    objTareaDetalle.IdTarea = id;

                //    if (estado == "No Procede")
                //    {
                //        objTareaDetalleTransferido.IdEstado = 2;
                //        objTareaDetalle.IdEstado = 2;

                //    }
                //    else
                //    {
                //        objTareaDetalleTransferido.IdEstado = 1;
                //        objTareaDetalle.IdEstado = 3;
                //        int id_pers_enti_end = Convert.ToInt32(obj_Ticket.ID_PERS_ENTI_END);

                //        PERSON_ENTITY objPersonEntity = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id_pers_enti_end).FirstOrDefault();
                //        int idloca = Convert.ToInt32(objPersonEntity.ID_LOCA);

                //        if (idloca == 1899 || idloca == 1909 || idloca == 1912)
                //        {
                //            var lstCorreoTransferencia = (from pe in dbe.PERSON_ENTITY
                //                                          where (pe.ID_PERS_ENTI == 827 || pe.ID_PERS_ENTI == 69580)
                //                                          select new
                //                                          {
                //                                              EMA_ELEC = pe.EMA_ELEC
                //                                          }).ToList();
                //            string Correos = string.Join(",", (from row in lstCorreoTransferencia select row.EMA_ELEC));
                //            sendMailTransferencia(descripcion, idticket, "T", Correos);
                //        }

                //        else if (idloca == 1903 || idloca == 1900 || idloca == 1902 || idloca == 1906)
                //        {
                //            var lstCorreoTransferencia = (from pe in dbe.PERSON_ENTITY
                //                                          where pe.ID_PERS_ENTI == 827
                //                                          select new
                //                                          {
                //                                              EMA_ELEC = pe.EMA_ELEC
                //                                          }).ToList();
                //            string Correos = string.Join(",", (from row in lstCorreoTransferencia select row.EMA_ELEC));
                //            sendMailTransferencia(descripcion, idticket, "T", Correos);
                //        }
                //        else
                //        {
                //            var lstCorreoTransferencia = (from pe in dbe.PERSON_ENTITY
                //                                          where (pe.ID_PERS_ENTI == 79773 || pe.ID_PERS_ENTI == 66522 || pe.ID_PERS_ENTI == 72109 || pe.ID_PERS_ENTI == 66518)
                //                                          select new
                //                                          {
                //                                              EMA_ELEC = pe.EMA_ELEC
                //                                          }).ToList();
                //            string Correos = string.Join(",", (from row in lstCorreoTransferencia select row.EMA_ELEC));
                //            sendMailTransferencia(descripcion, idticket, "T", Correos);
                //        }

                //    }
                //    objTareaDetalle.FechaCreacion = DateTime.Now;
                //    objTareaDetalle.Id_Queu = 25;
                //    objTareaDetalleTransferido.Id_Pers_Enti = id_pers_enti;
                //    objTareaDetalleTransferido.FechaTareaTerminada = DateTime.Now;
                //    db.TareaDetalle.Add(objTareaDetalle);
                //    db.SaveChanges();
                //    return Content("OK");
                //}
                //primer cambio
                //else if (id_queu == 25 & (descripcion == "Registro de licencia Mcafee/ EDR/ SCCM" || descripcion == "Registro de licencia Absolute" || descripcion == "Eliminacón de Licencia Mcafee / EDR / SCCM"))
                //{
                //    TareaDetalle objTareaDetalleTransferido = db.TareaDetalle.Where(x => x.Tarea.DescripcionTarea == descripcion & x.Id_Tick == idticket & x.Id_Queu == 25).FirstOrDefault();

                //    if (estado == "No Procede")
                //    {
                //        objTareaDetalleTransferido.IdEstado = 2;
                //    }
                //    else
                //    {
                //        objTareaDetalleTransferido.IdEstado = 1;
                //    }

                //    objTareaDetalleTransferido.Id_Pers_Enti = id_pers_enti;
                //    objTareaDetalleTransferido.FechaTareaTerminada = DateTime.Now;

                //    db.SaveChanges();

                //    return Content("OK");
                //}
                //else
                //{
                TareaDetalle objTareaDetalle = db.TareaDetalle.Where(x => x.Tarea.DescripcionTarea == descripcion & x.Id_Tick == idticket).FirstOrDefault();

                if (estado == "No Procede")
                {
                    objTareaDetalle.IdEstado = 2;
                }
                else
                {
                    objTareaDetalle.IdEstado = 1;

                }

                objTareaDetalle.UsuarioCreador = userId;
                objTareaDetalle.FechaTareaTerminada = DateTime.Now;

                db.SaveChanges();
                //cuarto cambio
                int totalTareaRealizada = (from td in db.TareaDetalle
                                           where ((td.IdEstado == 1 || td.IdEstado == 2) & td.Id_Tick == idticket)
                                           select new
                                           {
                                               idTarea = td.IdTarea
                                           }).Distinct().Count();

                if (idcate == 27009 || idcate == 28380 || idcate == 29751 || idcate == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMinsur"]) || idcate == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMarcobre"]) || idcate == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeRaura"]))
                {
                    int totalTareas = db.TareaDetalle.Where(x => x.Id_Tick == idticket).Count();

                    if (totalTareaRealizada == totalTareas)
                    {
                        TICKET objTicket = db.TICKETs.Where(x => x.ID_TICK == idticket).FirstOrDefault();

                        objTicket.ID_STAT_END = 4;
                        db.SaveChanges();
                        // AGREGAR COMENTARIO DE RESOLUCION AUTOMATICA DEL TICKET
                        DETAILS_TICKET objDetails = new DETAILS_TICKET();
                        objDetails.ID_TICK = idticket;
                        objDetails.ID_STAT = 4;
                        objDetails.ID_TYPE_DETA_TICK = 3;
                        objDetails.COM_DETA_TICK = "Ticket resuelto automáticamente al término de sus tareas.";
                        objDetails.UserId = 34;
                        objDetails.CREATE_DETA_INCI = DateTime.Now;
                        objDetails.FROM_TIME = DateTime.Now;
                        objDetails.TO_TIME = DateTime.Now;
                        objDetails.IdTipoResolucion = 2;
                        db.DETAILS_TICKET.Add(objDetails);
                        db.SaveChanges();
                        ////////////////////////////////////////////////////////////
                        return Content("REFRESH");

                    }
                    else
                    {
                        return Content("OK");
                    }
                }
                else if (idcate == 27011 || idcate == 28382 || idcate == 29753)
                {
                    int totalTareas = db.TareaDetalle.Where(x => x.Id_Tick == idticket).Count();

                    if (totalTareaRealizada == totalTareas)
                    {
                        TICKET objTicket = db.TICKETs.Where(x => x.ID_TICK == idticket).FirstOrDefault();

                        objTicket.ID_STAT_END = 4;
                        db.SaveChanges();
                        // AGREGAR COMENTARIO DE RESOLUCION AUTOMATICA DEL TICKET
                        DETAILS_TICKET objDetails = new DETAILS_TICKET();
                        objDetails.ID_TICK = idticket;
                        objDetails.ID_STAT = 4;
                        objDetails.ID_TYPE_DETA_TICK = 3;
                        objDetails.COM_DETA_TICK = "Ticket resuelto automáticamente al término de sus tareas.";
                        objDetails.UserId = 34;
                        objDetails.CREATE_DETA_INCI = DateTime.Now;
                        objDetails.FROM_TIME = DateTime.Now;
                        objDetails.TO_TIME = DateTime.Now;
                        objDetails.IdTipoResolucion = 2;
                        db.DETAILS_TICKET.Add(objDetails);
                        db.SaveChanges();
                        ////////////////////////////////////////////////////////////
                        return Content("REFRESH");

                    }
                    else
                    {
                        return Content("OK");
                    }

                }

                return Content("OK");
                //}
            }
            else
            {
                //if (existeTarea > 1)
                //{
                //    //segundo cambio
                //    if (id_queu == 25 & (descripcion == "Registro de licencia Mcafee/ EDR/ SCCM" || descripcion == "Registro de licencia Absolute" || descripcion == "Eliminacón de Licencia Mcafee / EDR / SCCM"))
                //    {
                //        TareaDetalle objTareaDetalleTransferido = db.TareaDetalle.Where(x => x.Tarea.DescripcionTarea == descripcion & x.Id_Tick == idticket & x.Id_Queu == 25).FirstOrDefault();

                //        if (estado == "No Procede")
                //        {
                //            objTareaDetalleTransferido.IdEstado = 2;

                //        }
                //        else
                //        {

                //            objTareaDetalleTransferido.IdEstado = 1;

                //        }

                //        objTareaDetalleTransferido.Id_Pers_Enti = id_pers_enti;
                //        objTareaDetalleTransferido.FechaTareaTerminada = DateTime.Now;

                //        db.SaveChanges();

                //        return Content("OK");
                //    }

                //    return Content("ERROR");
                //}

                //TareaDetalle objTareaDetalle = new TareaDetalle();
                //TareaDetalle objTareaTransferida = new TareaDetalle();
                //objTareaDetalle.Id_Tick = idticket;
                //objTareaDetalle.IdTarea = id;

                //if (id_queu == 1 & (descripcion == "Registro de licencia Mcafee/ EDR/ SCCM" || descripcion == "Registro de licencia Absolute" || descripcion == "Eliminacón de Licencia Mcafee / EDR / SCCM"))
                //{

                //    objTareaDetalle.FechaCreacion = DateTime.Now;
                //    objTareaTransferida.Id_Tick = idticket;
                //    objTareaTransferida.IdTarea = id;
                //    objTareaTransferida.Id_Queu = 25;
                //    objTareaTransferida.IdEstado = 3;
                //    objTareaTransferida.FechaCreacion = DateTime.Now;

                //    objTareaDetalle.IdEstado = 1;
                //    objTareaDetalle.Id_Queu = 1;
                //    //int idacco = Convert.ToInt32(Session["ID_ACCO"]);

                //    int id_pers_enti_end = Convert.ToInt32(obj_Ticket.ID_PERS_ENTI_END);

                //    PERSON_ENTITY objPersonEntity = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id_pers_enti_end).FirstOrDefault();
                //    int idloca = Convert.ToInt32(objPersonEntity.ID_LOCA);

                //    if (idloca == 1899 || idloca == 1909 || idloca == 1912)
                //    {
                //        var lstCorreoTransferencia = (from pe in dbe.PERSON_ENTITY
                //                                      where (pe.ID_PERS_ENTI == 827 || pe.ID_PERS_ENTI == 69580)
                //                                      select new
                //                                      {
                //                                          EMA_ELEC = pe.EMA_ELEC
                //                                      }).ToList();
                //        string Correos = string.Join(",", (from row in lstCorreoTransferencia select row.EMA_ELEC));
                //        //sendMailTransferencia(descripcion, idticket, "T", Correos);
                //    }

                //    else if (idloca == 1903 || idloca == 1900 || idloca == 1902 || idloca == 1906)
                //    {
                //        var lstCorreoTransferencia = (from pe in dbe.PERSON_ENTITY
                //                                      where pe.ID_PERS_ENTI == 827
                //                                      select new
                //                                      {
                //                                          EMA_ELEC = pe.EMA_ELEC
                //                                      }).ToList();
                //        string Correos = string.Join(",", (from row in lstCorreoTransferencia select row.EMA_ELEC));
                //        //sendMailTransferencia(descripcion, idticket, "T", Correos);
                //    }
                //    else
                //    {
                //        var lstCorreoTransferencia = (from pe in dbe.PERSON_ENTITY
                //                                      where (pe.ID_PERS_ENTI == 79773 || pe.ID_PERS_ENTI == 66518)
                //                                      select new
                //                                      {
                //                                          EMA_ELEC = pe.EMA_ELEC
                //                                      }).ToList();
                //        string Correos = string.Join(",", (from row in lstCorreoTransferencia select row.EMA_ELEC));
                //        //sendMailTransferencia(descripcion, idticket, "T", Correos);
                //    }

                //    db.TareaDetalle.Add(objTareaTransferida);
                //    db.SaveChanges();

                //}
                //else if (estado == "No Procede")
                //{
                //    objTareaTransferida.Id_Tick = idticket;
                //    objTareaTransferida.IdTarea = id;
                //    objTareaTransferida.Id_Queu = 25;
                //    objTareaTransferida.IdEstado = 3;
                //    objTareaTransferida.FechaCreacion = DateTime.Now;
                //    objTareaDetalle.IdEstado = 2;
                //    objTareaTransferida.IdEstado = 2;
                //    db.TareaDetalle.Add(objTareaTransferida);
                //    db.SaveChanges();
                //}
                //else
                //{
                //    objTareaDetalle.IdEstado = 1;

                //}
                //objTareaDetalle.Id_Queu = id_queu;
                //objTareaDetalle.Id_Pers_Enti = id_pers_enti;
                //objTareaDetalle.FechaCreacion = Convert.ToDateTime(obj_Ticket.FEC_INI_TICK);
                //objTareaDetalle.FechaTareaTerminada = DateTime.Now;

                //db.TareaDetalle.Add(objTareaDetalle);
                //db.SaveChanges();
                //quinto cambio
                //int totalTareaRealizada = (from td in db.TareaDetalle
                //                           where ((td.IdEstado == 1 || td.IdEstado == 2) & td.Id_Tick == idticket)
                //                           select new
                //                           {
                //                               idTarea = td.IdTarea
                //                           }).Distinct().Count();


                //if (idcate == 27009 || idcate == 28380 || idcate == 29751)
                //{
                //    int totalTareas = db.Tarea.Where(x => (x.TipoGestion == "ALTA")).Count();

                //    if (totalTareaRealizada == totalTareas)
                //    {
                //        TICKET objTicket = db.TICKETs.Where(x => x.ID_TICK == idticket).FirstOrDefault();

                //        objTicket.ID_STAT_END = 4;
                //        db.SaveChanges();
                //        // AGREGAR COMENTARIO DE RESOLUCION AUTOMATICA DEL TICKET
                //        DETAILS_TICKET objDetails = new DETAILS_TICKET();
                //        objDetails.ID_TICK = idticket;
                //        objDetails.ID_STAT = 4;
                //        objDetails.ID_TYPE_DETA_TICK = 3;
                //        objDetails.COM_DETA_TICK = "Ticket resuelto automáticamente al término de sus tareas.";
                //        objDetails.UserId = 34;
                //        objDetails.CREATE_DETA_INCI = DateTime.Now;
                //        objDetails.FROM_TIME = DateTime.Now;
                //        objDetails.TO_TIME = DateTime.Now;
                //        objDetails.IdTipoResolucion = 2;
                //        db.DETAILS_TICKET.Add(objDetails);
                //        db.SaveChanges();
                //        ////////////////////////////////////////////////////////////
                //        return Content("REFRESH");

                //    }
                //    else
                //    {
                //        return Content("OK");
                //    }
                //}
                //else if (idcate == 27011 || idcate == 28382 || idcate == 29753)
                //{
                //    int totalTareas = db.Tarea.Where(x => (x.TipoGestion == "BAJA")).Count();

                //    if (totalTareaRealizada == totalTareas)
                //    {
                //        TICKET objTicket = db.TICKETs.Where(x => x.ID_TICK == idticket).FirstOrDefault();

                //        objTicket.ID_STAT_END = 4;
                //        db.SaveChanges();
                //        // AGREGAR COMENTARIO DE RESOLUCION AUTOMATICA DEL TICKET
                //        DETAILS_TICKET objDetails = new DETAILS_TICKET();
                //        objDetails.ID_TICK = idticket;
                //        objDetails.ID_STAT = 4;
                //        objDetails.ID_TYPE_DETA_TICK = 3;
                //        objDetails.COM_DETA_TICK = "Ticket resuelto automáticamente al término de sus tareas.";
                //        objDetails.UserId = 34;
                //        objDetails.CREATE_DETA_INCI = DateTime.Now;
                //        objDetails.FROM_TIME = DateTime.Now;
                //        objDetails.TO_TIME = DateTime.Now;
                //        objDetails.IdTipoResolucion = 2;
                //        db.DETAILS_TICKET.Add(objDetails);
                //        db.SaveChanges();
                //        ////////////////////////////////////////////////////////////
                //        return Content("REFRESH");
                //    }
                //    else
                //    {
                //        return Content("OK");
                //    }
                //}
                //    return Content("OK");
                return Content("ERROR");
            }



        }
        public ActionResult viewTransferirTarea(int id, int idqueu, int movil = 0)
        {
            ViewBag.IdTareaDetalle = id;
            ViewBag.IdQueu = idqueu;
            ViewBag.Movil = movil;
            return View();

        }

        [HttpPost]
        public ActionResult AsignarTarea(int id, int idticket, String descripcion, int usuario)
        {
            int usuarioActivo = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int id_queu = Convert.ToInt32(Session["ID_QUEU"]);

            //TareaDetalle objUltimaTareaDetalle = db.TareaDetalle.Where(
            //    x => x.IdTarea == id &&
            //    x.Id_Tick == idticket
            //    ).OrderByDescending(x => x.IdTareaDetalle).FirstOrDefault();
            TareaDetalle objTareaDetalle = db.TareaDetalle.Where(x => x.IdTareaDetalle == id).FirstOrDefault();
            var objPerson = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == usuario).FirstOrDefault();

            objTareaDetalle.IdEstado = 4;
            objTareaDetalle.Id_Pers_Enti = usuario;
            objTareaDetalle.Id_Queu = objPerson.ID_QUEU;
            db.SaveChanges();
            return Content("OK");
        }
        public bool sendMailTransferencia(String descripcion, int idticket, string flag, string correos)
        {


            TICKET obj_Ticket = db.TICKETs.Where(x => x.ID_TICK == idticket).FirstOrDefault();
            Tarea obj_Tarea = db.Tarea.Where(x => x.DescripcionTarea == descripcion).FirstOrDefault();

            string ticket = obj_Ticket.COD_TICK;
            string sumtik = obj_Ticket.SUM_TICK;
            string comentario = obj_Tarea.DescripcionTarea;
            string msjSolicitud = "";
            string status = "";
            string asunto = "";
            string fechaRegistro = "";


            if (flag == "T")
            {
                msjSolicitud = "Se ejecutó la transferencia.";
                status = "Transferencia Ejecutada.";
                asunto = "TRANSFERENCIA DE TAREA";
            }

            //Envio de Correo 
            Body body = new Body();
            string body_html = body.TranferenciaN3(sumtik, comentario, ticket);

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(correos))
            {

                message.To.Add(correos);

            }

            message.AlternateViews.Add(htmlView);
            message.Subject = asunto;
            newMail.smtp.Send(message);
            return true;
        }

        public ActionResult EnviarCorreoAprobacionGTI()
        {
            //var objTicket = new TICKET();
            //var objDetalleTicket = new DETAILS_TICKET();
            //var objAdjunto = new ATTACHED();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int ID_TICK = 0;

            if (Request.Params["IdTick"] != null)
            {
                ID_TICK = Convert.ToInt32(Request.Params["IdTick"].ToString());
            }
            else
            {
                ID_TICK = Convert.ToInt32(Request.Params["ID_TICK"].ToString());
            }

            string ComentarioAdicional = "";

            TICKET obj_Ticket = db.TICKETs.Where(x => x.ID_TICK == ID_TICK).FirstOrDefault();
            DETAILS_TICKET obj_Details_Tick = db.DETAILS_TICKET.Where(t => t.ID_TICK == ID_TICK && t.IdRazon == 13)
                    .OrderByDescending(t => t.ID_DETA_TICK)
                   .FirstOrDefault();

            //ATTACHED obj_Adjunto_Tick = db.ATTACHEDs.Where(t => t.ID_DETA_INCI == obj_Details_Tick.ID_DETA_TICK)
            //        .OrderByDescending(t => t.ID_ATTA)
            //       .FirstOrDefault();
            List<ATTACHED> adjuntos_Tick = db.ATTACHEDs.Where(t => t.ID_DETA_INCI == obj_Details_Tick.ID_DETA_TICK)
                    .OrderByDescending(t => t.ID_ATTA)
                    .ToList();

            int count_Adjuntos_Tick = adjuntos_Tick.Count;

            if (ID_TICK != 0)
            {
                ComentarioAdicional = Request.Params["Comentario"].ToString();

                if (ComentarioAdicional == null)
                {
                    return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
                }
            }

            string ticket = obj_Ticket.COD_TICK;
            string sumtik = obj_Ticket.SUM_TICK;
            string comentario = obj_Ticket.SUM_TICK;
            string estadoHost = obj_Details_Tick.COM_DETA_TICK;
            string comentarioAdicional = ComentarioAdicional;
            string msjSolicitud = "";
            string status = "";
            string asunto = "";
            string fechaRegistro = "";
            string adjunto = "";
            //string EXT_ATTA = obj_Adjunto_Tick.EXT_ATTA.ToLower();
            int idTick = obj_Ticket.ID_TICK;
            List<string> extensionesImagen = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };

            List<string> adjuntosEnCorreo = new List<string>();
            foreach (var adjuntoItem in adjuntos_Tick)
            {
                string EXT_ATTA = adjuntoItem.EXT_ATTA.ToLower();
                if (extensionesImagen.Contains(EXT_ATTA))
                {
                    string adjuntoNombre = adjuntoItem.NAM_ATTA + "_" + adjuntoItem.ID_ATTA + adjuntoItem.EXT_ATTA;
                    adjuntosEnCorreo.Add(adjuntoNombre);
                }
            }

            if (adjuntosEnCorreo.Count == 0)
            {
                return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            }
            msjSolicitud = "Se solicita aprobación para el siguiente acceso.";
            status = "Solicitud de aprobación GTI enviada.";
            asunto = "SOLICITUD DE APROBACIÓN DE ACCESOS USB";

            //if (extensionesImagen.Contains(EXT_ATTA))
            //{
            //    adjunto = obj_Adjunto_Tick.NAM_ATTA + "_" + obj_Adjunto_Tick.ID_ATTA + obj_Adjunto_Tick.EXT_ATTA;
            //    msjSolicitud = "Se solicita aprobación para el siguiente acceso.";
            //    status = "Solicitud de aprobación GTI enviada.";
            //    asunto = "SOLICITUD DE APROBACIÓN DE ACCESOS";
            //}
            //else
            //{
            //    return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            //} 


            //Envio de Correo 
            Body body = new Body();
            //string body_html = body.SolicitudAprobacionAccesoGTI(sumtik, comentario, estadoHost, comentarioAdicional, ticket, adjunto, idTick);

            //string body_html = body.SolicitudAprobacionAccesoGTI(sumtik, comentario, estadoHost, comentarioAdicional, ticket, string.Join(",", adjuntosEnCorreo), obj_Ticket.ID_TICK);
            string body_html = body.SolicitudAprobacionAccesoGTI(sumtik, comentario, estadoHost, comentarioAdicional, ticket, adjuntosEnCorreo, obj_Ticket.ID_TICK);


            PluginSmtpBNV newMail = new PluginSmtpBNV();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

            var correoGTI = db.ACCOUNT_PARAMETER
                .Where(ap => ap.ID_ACCO == 60 && ap.ID_PARA == (db.PARAMETERs
                    .Where(p => p.NAM_PARA == "Correo GTI Buenaventura"))
                    .Select(p => p.ID_PARA)
                    .FirstOrDefault())
                .Select(ap => ap.VAL_ACCO_PARA)
                .FirstOrDefault();
            message.To.Add(correoGTI);

            message.AlternateViews.Add(htmlView);
            message.Subject = asunto;
            newMail.smtp.Send(message);
            return Json(new { Respuesta = "OK" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult EnviarCorreoSolcucionAccesoUSB()
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            int ID_TICK = 0;

            if (Request.Params["IdTick"] != null)
            {
                ID_TICK = Convert.ToInt32(Request.Params["IdTick"].ToString());
            }
            else
            {
                ID_TICK = Convert.ToInt32(Request.Params["ID_TICK"].ToString());
            }

            string ComentarioAdicional = "";

            TICKET obj_Ticket = db.TICKETs.Where(x => x.ID_TICK == ID_TICK).FirstOrDefault();
            DETAILS_TICKET obj_Details_Tick = db.DETAILS_TICKET.Where(t => t.ID_TICK == ID_TICK && t.IdRazon == 3 && t.ID_STAT == 4)
                    .OrderByDescending(t => t.ID_DETA_TICK)
                   .FirstOrDefault();

            List<ATTACHED> adjuntos_Tick = db.ATTACHEDs.Where(t => t.ID_DETA_INCI == obj_Details_Tick.ID_DETA_TICK)
                    .OrderByDescending(t => t.ID_ATTA)
                    .ToList();

            int count_Adjuntos_Tick = adjuntos_Tick.Count;

            string ticket = obj_Ticket.COD_TICK;
            string sumtik = obj_Ticket.SUM_TICK;
            string comentario = obj_Ticket.SUM_TICK;
            string solucion = obj_Details_Tick.COM_DETA_TICK;
            string comentarioAdicional = ComentarioAdicional;
            string msjSolicitud = "";
            string status = "";
            string asunto = "";
            string fechaRegistro = "";
            string adjunto = "";

            int idTick = obj_Ticket.ID_TICK;
            List<string> extensionesImagen = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };

            List<string> adjuntosEnCorreo = new List<string>();
            foreach (var adjuntoItem in adjuntos_Tick)
            {
                string EXT_ATTA = adjuntoItem.EXT_ATTA.ToLower();
                if (extensionesImagen.Contains(EXT_ATTA))
                {
                    string adjuntoNombre = adjuntoItem.NAM_ATTA + "_" + adjuntoItem.ID_ATTA + adjuntoItem.EXT_ATTA;
                    adjuntosEnCorreo.Add(adjuntoNombre);
                }
                else
                    return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);

            }

            //if (adjuntosEnCorreo.Count == null)
            //{
            //    return Json(new { Respuesta = "Error Existe" }, JsonRequestBehavior.AllowGet);
            //}
            msjSolicitud = "Se activó el acceso USB solicitado.";
            status = "Solicitud de acceso USB completa.";
            asunto = "SOLICITUD DE ACCESO USB";


            //Envio de Correo 
            Body body = new Body();

            string body_html = body.SolcucionAccesoUSB(sumtik, comentario, solucion, comentarioAdicional, ticket, adjuntosEnCorreo, obj_Ticket.ID_TICK);


            PluginSmtpBNV newMail = new PluginSmtpBNV();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

            var correoOperadorSeguridad = db.ACCOUNT_PARAMETER
                .Where(ap => ap.ID_ACCO == 60 && ap.ID_PARA == (db.PARAMETERs
                    .Where(p => p.NAM_PARA == "Correo Operador Seguridad Buenaventura"))
                    .Select(p => p.ID_PARA)
                    .FirstOrDefault())
                .Select(ap => ap.VAL_ACCO_PARA)
                .FirstOrDefault();
            message.To.Add(correoOperadorSeguridad);

            message.AlternateViews.Add(htmlView);
            message.Subject = asunto;
            newMail.smtp.Send(message);
            return Json(new { Respuesta = "OK" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListProductClient(int id = 0)
        {
            using (EntityEntities context = new EntityEntities())
            {
                var query = context.CLASS_ENTITY.Where(x => x.ID_ENTI == id && x.VIG_ENTI == true).ToList();

                var listProductClient = (from ce in query
                                         join sop in db.Soportes on ce.ID_ENTI equals sop.IdCompania
                                         where ce.ID_ENTI == id && sop.FechaFinGarantia >= DateTime.Now
                                         select new
                                         {
                                             ID_COMP = ce.ID_ENTI,
                                             NAM_CLIEN = ce.COM_NAME,
                                             DES_PROD = sop.Producto,
                                             SER_PROD = sop.Serie,
                                             FEC_INIC_GAR = (sop.FechaInicioGarantia == null ? "" : string.Format("{0:d MMMM yyyy}", sop.FechaInicioGarantia)),
                                             FEC_FIN_GAR = (sop.FechaFinGarantia == null ? "" : string.Format("{0:d MMMM yyyy}", sop.FechaFinGarantia)),
                                             NRO_PREV = sop.NroPreventivos,
                                             BOL_HOR = sop.BolsaHoras,
                                             OBS_PRO = sop.Observaciones
                                         }).OrderByDescending(x => x.FEC_FIN_GAR);

                return Json(new { Data = listProductClient, Count = listProductClient.Count() }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult FindResult()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int Count = 0, UserId, ID_QUEU, ID_PERS_ENTI_ASSI, ID_TYPE_TICK, ID_PRIO, ID_SOUR, ID_PERS_ENTI, ID_PERS_ENTI_END,
                CountClose, ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_STAT_END, ID_COMP, ID_COMP_END;
            DateTime START_DATE, END_DATE;

            //Seleccion Multiple
            string estado = Convert.ToString(Request.Params["IdEstados"]).ToString();
            string[] strEstados = estado.Split(',');
            int[] Estados = new int[10];
            int contador = 0;
            if (strEstados[0] == "")
                strEstados[0] = "0";
            foreach (string strEstado in strEstados)
            {
                Estados[contador] = Convert.ToInt32(strEstado);
                contador++;
            }

            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                    SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]),
                    from = "", to = "";

            var listCate = db.CATEGORies
                .Join(db.CATEGORies, c1 => c1.ID_CATE, c2 => c2.ID_CATE_PARE, (c1, c2) => new { ID_CATE_1 = c1.ID_CATE, ID_CATE_2 = c2.ID_CATE })
                .Join(db.CATEGORies, c2 => c2.ID_CATE_2, c3 => c3.ID_CATE_PARE, (c2, c3) => new { c2.ID_CATE_1, c2.ID_CATE_2, ID_CATE_3 = c3.ID_CATE })
                .Join(db.CATEGORies, c3 => c3.ID_CATE_3, c4 => c4.ID_CATE_PARE, (c3, c4) => new { c3.ID_CATE_1, c3.ID_CATE_2, c3.ID_CATE_3, ID_CATE_4 = c4.ID_CATE });

            int xcd = listCate.Count();

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE"]), out ID_CATE) == true)
            {
                listCate = listCate.Where(t => t.ID_CATE_1 == ID_CATE);
            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CATE"]), out ID_SUB_CATE) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_2 == ID_SUB_CATE);
                }
            }
            catch
            {

            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CLAS"]), out ID_CLAS) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_3 == ID_CLAS);
                }
            }
            catch
            {

            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CLAS"]), out ID_SUB_CLAS) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_4 == ID_SUB_CLAS);
                }
            }
            catch
            {

            }

            var listClient = (from lu in dbe.ACCOUNT_ENTITY
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

            var tick = (from t in db.TICKETs
                        where t.ID_ACCO == ID_ACCO
                        join lc in listCate on t.ID_CATE equals lc.ID_CATE_4
                        join s in db.STATUS on t.ID_STAT_END equals s.ID_STAT
                        select new
                        {
                            t.ID_ACCO,
                            ID_DOCU_SALE = (t.ID_DOCU_SALE == null ? 0 : t.ID_DOCU_SALE),
                            t.FEC_TICK,
                            t.COD_TICK,
                            t.UserId,
                            t.ID_QUEU,
                            t.ID_PERS_ENTI_ASSI,
                            t.ID_TYPE_TICK,
                            t.ID_PRIO,
                            t.ID_SOUR,
                            t.ID_STAT_END,
                            t.SUM_TICK,
                            t.ID_PERS_ENTI,
                            t.ID_PERS_ENTI_END,
                            t.ID_CATE,
                            t.CREATE_TICK,
                            t.MODIFIED_TICK,
                            t.ID_TICK,
                            t.IS_PARENT,
                            t.ID_TYPE_FORM,
                            s.ORD_STAT,
                            t.ID_COMP,
                            t.ID_COMP_END,
                            t.SERVICE,
                            t.FEC_INI_TICK
                        });
            var schbjsd = tick.Count();
            //SEIS ÚLTIMOS MESES DEL TICKET DE LA CUENTA
            //var legend = "bla";
            var legendx = (from t in tick
                           group t by new { t.FEC_TICK.Value.Year, t.FEC_TICK.Value.Month } into g
                           select new
                           {
                               g.Key.Year,
                               g.Key.Month,
                               Count = g.Count(),
                           }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Take(5);

            var legend = (from t in legendx
                          join am in db.ACCOUNTING_MONTH on t.Year equals am.ID_ACCO_YEAR
                          where t.Month == am.ACCO_MONT
                          select new
                          {
                              t.Year,
                              t.Month,
                              t.Count,
                              name = am.NAM_ACCO_MONT.Substring(0, 1) + am.NAM_ACCO_MONT.Substring(1, 2).ToLower()
                          });



            if (!String.IsNullOrEmpty(COD_TICK))
            {
                tick = tick.Where(t => t.COD_TICK.ToUpper().Contains(COD_TICK.ToUpper()));
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["UserId"]), out UserId) == true)
            {
                tick = tick.Where(t => t.UserId == UserId);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == true)
            {
                tick = tick.Where(t => t.ID_QUEU == ID_QUEU);
            }
            if (estado != "0")
            {
                //tick = tick.Where(t => t.ID_STAT_END == ID_STAT_END);
                tick = tick.Where(t => Estados.Contains((int)t.ID_STAT_END));
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP"]), out ID_COMP) == true)
            {
                tick = tick.Where(t => t.ID_COMP == ID_COMP);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP_END"]), out ID_COMP_END) == true)
            {
                tick = tick.Where(t => t.ID_COMP_END == ID_COMP_END);
            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == true)
                {
                    var cantidadx = tick.Count();
                    tick = tick.Where(t => t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
                    cantidadx = tick.Count();
                }
            }
            catch { }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_TICK"]), out ID_TYPE_TICK) == true)
            {
                tick = tick.Where(t => t.ID_TYPE_TICK == ID_TYPE_TICK);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PRIO"]), out ID_PRIO) == true)
            {
                tick = tick.Where(t => t.ID_PRIO == ID_PRIO);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_SOUR"]), out ID_SOUR) == true)
            {
                tick = tick.Where(t => t.ID_SOUR == ID_SOUR);
            }

            if (!String.IsNullOrEmpty(SUM_TICK))
            {
                tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(SUM_TICK.ToUpper()));
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI == ID_PERS_ENTI);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_END"]), out ID_PERS_ENTI_END) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI_END == ID_PERS_ENTI_END);
            }

            if (Request.Params["SERVICE"].ToString() != "false")
            {
                tick = tick.Where(t => t.SERVICE == true);
            }



            var cantidad = tick.Count();

            var lineClose = (from x in legend
                             join t in tick on x.Year equals t.FEC_TICK.Value.Year
                             where x.Month == t.FEC_TICK.Value.Month && t.ID_STAT_END == 6
                             group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                             select new
                             {
                                 g.Key.ID_STAT_END,
                                 g.Key.Year,
                                 g.Key.Month,
                                 g.Key.Count,
                                 g.Key.name,
                                 RES_COUN = g.Count()

                             }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineOpen = (from x in legend
                            join t in tick on x.Year equals t.FEC_TICK.Value.Year
                            where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)
                            group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                            select new
                            {
                                g.Key.ID_STAT_END,
                                g.Key.Year,
                                g.Key.Month,
                                g.Key.Count,
                                g.Key.name,
                                RES_COUN = g.Count()

                            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineScheduled = (from x in legend
                                 join t in tick on x.Year equals t.FEC_TICK.Value.Year
                                 where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 5)
                                 group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                                 select new
                                 {
                                     g.Key.ID_STAT_END,
                                     g.Key.Year,
                                     g.Key.Month,
                                     g.Key.Count,
                                     g.Key.name,
                                     RES_COUN = g.Count()

                                 }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE"]), out START_DATE))
            {
                tick = tick.Where(t => t.FEC_TICK >= START_DATE);

            }

            if (DateTime.TryParse(Convert.ToString(Request.Params["END_DATE"]), out END_DATE))
            {
                //END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                //END_DATE = END_DATE.AddSeconds(59);
                tick = tick.Where(t => t.FEC_TICK <= END_DATE);
            }


            //Fecha Programada
            if (Convert.ToString(Request.Params["FechaProgramada"]) != "")
            {
                DateTime FechaProgramada = Convert.ToDateTime(Request.Params["FechaProgramada"]);
                //tick = tick.Where(t => t.ID_STAT_END==5 &&  (db.tktObtenerFechaProgramada(t.ID_TICK).Single()) == FechaProgramada);
                tick = tick.Where(t => t.ID_STAT_END == 5 && ((db.DETAILS_TICKET.Where(x => x.ID_TICK == t.ID_TICK && x.ID_STAT == 5 && x.ID_TYPE_DETA_TICK == 3).OrderByDescending(x => x.ID_DETA_TICK).FirstOrDefault().FEC_SCHE) == null ? t.FEC_INI_TICK :
                    (db.DETAILS_TICKET.Where(x => x.ID_TICK == t.ID_TICK && x.ID_STAT == 5 && x.ID_TYPE_DETA_TICK == 3).OrderByDescending(x => x.ID_DETA_TICK).FirstOrDefault().FEC_SCHE)) == FechaProgramada);
            }


            Count = tick.Count();
            CountClose = tick.Where(t => t.ID_STAT_END == 6).Count();

            try
            {
                from = String.Format("{0:d}", tick.OrderBy(t => t.FEC_TICK).First().FEC_TICK);
                to = String.Format("{0:d}", tick.OrderByDescending(t => t.FEC_TICK).First().FEC_TICK);
            }
            catch
            {

            }

            var resultPie = (from t in tick
                             join st in db.STATUS on t.ID_STAT_END equals st.ID_STAT
                             where t.ID_STAT_END != 6 && t.ID_STAT_END != 2
                             group st by new { st.NAM_GRAP_STAT, st.COL_GRAP_STAT } into g
                             select new
                             {
                                 name = g.Key.NAM_GRAP_STAT.Substring(0, 1) + g.Key.NAM_GRAP_STAT.Substring(1, g.Key.NAM_GRAP_STAT.Length).ToLower(),
                                 color = g.Key.COL_GRAP_STAT,
                                 y = g.Count()
                             }).ToList();

            tick = tick.OrderBy(t => t.ORD_STAT).ThenByDescending(t => t.FEC_TICK).Take(1000);//.Skip(skip).Take(take);
            var tc = tick.ToList();
            var result = (from i in tick.ToList()
                          join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          //join cr in listClient on i.UserId equals cr.UserId
                          join cr in listClient on (i.UserId == null ? 0 : i.UserId) equals cr.UserId into xx
                          from xp in xx.DefaultIfEmpty()
                          join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listClient on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI //into lcia from xcia in lcia.DefaultIfEmpty()
                          //join re in listClient on i.ID_PERS_ENTI_END equals re.ID_PERS_ENTI
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBC = c4.NAM_CATE.ToLower(),
                              NAM_CATE = c3.NAM_CATE.ToLower(),
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              //REQU = re.Client.ToLower(),
                              REQU = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                              URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                              CREATE_INCI = String.Format("{0:G}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:G}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              ICO_PRIO = pr.ICO_PRIO,
                              //CREATEBY = cr.Client.ToLower(),
                              CREATEBY = (i.UserId == null ? ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI)).ToLower() : xp.Client.ToLower()),
                              ACCO = ac.NAM_ACCO,
                              VIS_COMP = ac.VIS_COMP,
                              ASSI = asi.Client.ToLower(),
                              EXP_TIME = ExpTime(i.ID_TICK),
                              ID_STAT_END = i.ID_STAT_END,
                              //ID_PERS_ENTI = 
                              CIA_TOOL = cia.COMPANY,//(xcia == null ? "" : xcia.COMPANY),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none")
                          }
                            );

            return Json(new { Data = result, Count = Count, CountClose = CountClose, From = from, To = to, Pie = resultPie, legend = legend, lineClose = lineClose, lineScheduled = lineScheduled, lineOpen = lineOpen }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindResultGrafico()
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int Count = 0, UserId, ID_QUEU, ID_PERS_ENTI_ASSI, ID_TYPE_TICK, ID_PRIO, ID_SOUR, ID_PERS_ENTI, ID_PERS_ENTI_END,
                CountClose, ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_STAT_END, ID_COMP, ID_COMP_END;
            DateTime START_DATE, END_DATE;

            //Seleccion Multiple
            string estado = Convert.ToString(Request.Params["IdEstados"]).ToString();
            string[] strEstados = estado.Split(',');
            int[] Estados = new int[10];
            int contador = 0;
            if (strEstados[0] == "")
                strEstados[0] = "0";
            foreach (string strEstado in strEstados)
            {
                Estados[contador] = Convert.ToInt32(strEstado);
                contador++;
            }

            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                    SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]),
                    from = "", to = "";

            //var listCate = db.CATEGORies
            //    .Join(db.CATEGORies, c1 => c1.ID_CATE, c2 => c2.ID_CATE_PARE, (c1, c2) => new { ID_CATE_1 = c1.ID_CATE, ID_CATE_2 = c2.ID_CATE })
            //    .Join(db.CATEGORies, c2 => c2.ID_CATE_2, c3 => c3.ID_CATE_PARE, (c2, c3) => new { c2.ID_CATE_1, c2.ID_CATE_2, ID_CATE_3 = c3.ID_CATE })
            //    .Join(db.CATEGORies, c3 => c3.ID_CATE_3, c4 => c4.ID_CATE_PARE, (c3, c4) => new { c3.ID_CATE_1, c3.ID_CATE_2, c3.ID_CATE_3, ID_CATE_4 = c4.ID_CATE });

            //int xcd = listCate.Count();

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE"]), out ID_CATE) == true)
            //{
            //    listCate = listCate.Where(t => t.ID_CATE_1 == ID_CATE);
            //}

            //try
            //{
            //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CATE"]), out ID_SUB_CATE) == true)
            //    {
            //        listCate = listCate.Where(t => t.ID_CATE_2 == ID_SUB_CATE);
            //    }
            //}
            //catch
            //{

            //}

            //try
            //{
            //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CLAS"]), out ID_CLAS) == true)
            //    {
            //        listCate = listCate.Where(t => t.ID_CATE_3 == ID_CLAS);
            //    }
            //}
            //catch
            //{

            //}

            //try
            //{
            //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CLAS"]), out ID_SUB_CLAS) == true)
            //    {
            //        listCate = listCate.Where(t => t.ID_CATE_4 == ID_SUB_CLAS);
            //    }
            //}
            //catch
            //{

            //}

            var listClient = (from lu in dbe.ACCOUNT_ENTITY
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

            var tick = (from t in db.TICKETs
                        where t.ID_ACCO == ID_ACCO
                        //join lc in listCate on t.ID_CATE equals lc.ID_CATE_4
                        join s in db.STATUS on t.ID_STAT_END equals s.ID_STAT
                        select new
                        {
                            t.ID_ACCO,
                            ID_DOCU_SALE = (t.ID_DOCU_SALE == null ? 0 : t.ID_DOCU_SALE),
                            t.FEC_TICK,
                            t.COD_TICK,
                            t.UserId,
                            t.ID_QUEU,
                            t.ID_PERS_ENTI_ASSI,
                            t.ID_TYPE_TICK,
                            t.ID_PRIO,
                            t.ID_SOUR,
                            t.ID_STAT_END,
                            t.SUM_TICK,
                            t.ID_PERS_ENTI,
                            t.ID_PERS_ENTI_END,
                            t.ID_CATE,
                            t.CREATE_TICK,
                            t.MODIFIED_TICK,
                            t.ID_TICK,
                            t.IS_PARENT,
                            t.ID_TYPE_FORM,
                            s.ORD_STAT,
                            t.ID_COMP,
                            t.ID_COMP_END,
                            t.SERVICE,
                            t.FEC_INI_TICK
                        });

            var schbjsd = tick.Count();
            //SEIS ÚLTIMOS MESES DEL TICKET DE LA CUENTA
            //var legend = "bla";
            var legendx = (from t in tick
                           group t by new { t.FEC_TICK.Value.Year, t.FEC_TICK.Value.Month } into g
                           select new
                           {
                               g.Key.Year,
                               g.Key.Month,
                               Count = g.Count(),
                           }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Take(5);

            var legend = (from t in legendx
                          join am in db.ACCOUNTING_MONTH on t.Year equals am.ID_ACCO_YEAR
                          where t.Month == am.ACCO_MONT
                          select new
                          {
                              t.Year,
                              t.Month,
                              t.Count,
                              name = am.NAM_ACCO_MONT.Substring(0, 1) + am.NAM_ACCO_MONT.Substring(1, 2).ToLower()
                          });



            if (!String.IsNullOrEmpty(COD_TICK))
            {
                tick = tick.Where(t => t.COD_TICK.ToUpper().Contains(COD_TICK.ToUpper()));
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["UserId"]), out UserId) == true)
            {
                tick = tick.Where(t => t.UserId == UserId);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == true)
            {
                tick = tick.Where(t => t.ID_QUEU == ID_QUEU);
            }
            if (estado != "0")
            {
                //tick = tick.Where(t => t.ID_STAT_END == ID_STAT_END);
                tick = tick.Where(t => Estados.Contains((int)t.ID_STAT_END));
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP"]), out ID_COMP) == true)
            {
                tick = tick.Where(t => t.ID_COMP == ID_COMP);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP_END"]), out ID_COMP_END) == true)
            {
                tick = tick.Where(t => t.ID_COMP_END == ID_COMP_END);
            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == true)
                {
                    var cantidadx = tick.Count();
                    tick = tick.Where(t => t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
                    cantidadx = tick.Count();
                }
            }
            catch { }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_TICK"]), out ID_TYPE_TICK) == true)
            {
                tick = tick.Where(t => t.ID_TYPE_TICK == ID_TYPE_TICK);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PRIO"]), out ID_PRIO) == true)
            {
                tick = tick.Where(t => t.ID_PRIO == ID_PRIO);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_SOUR"]), out ID_SOUR) == true)
            {
                tick = tick.Where(t => t.ID_SOUR == ID_SOUR);
            }

            if (!String.IsNullOrEmpty(SUM_TICK))
            {
                tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(SUM_TICK.ToUpper()));
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI == ID_PERS_ENTI);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_END"]), out ID_PERS_ENTI_END) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI_END == ID_PERS_ENTI_END);
            }

            if (Request.Params["SERVICE"].ToString() != "false")
            {
                tick = tick.Where(t => t.SERVICE == true);
            }



            var cantidad = tick.Count();

            var lineClose = (from x in legend
                             join t in tick on x.Year equals t.FEC_TICK.Value.Year
                             where x.Month == t.FEC_TICK.Value.Month && t.ID_STAT_END == 6
                             group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                             select new
                             {
                                 g.Key.ID_STAT_END,
                                 g.Key.Year,
                                 g.Key.Month,
                                 g.Key.Count,
                                 g.Key.name,
                                 RES_COUN = g.Count()

                             }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineOpen = (from x in legend
                            join t in tick on x.Year equals t.FEC_TICK.Value.Year
                            where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)
                            group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                            select new
                            {
                                g.Key.ID_STAT_END,
                                g.Key.Year,
                                g.Key.Month,
                                g.Key.Count,
                                g.Key.name,
                                RES_COUN = g.Count()

                            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineScheduled = (from x in legend
                                 join t in tick on x.Year equals t.FEC_TICK.Value.Year
                                 where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 5)
                                 group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                                 select new
                                 {
                                     g.Key.ID_STAT_END,
                                     g.Key.Year,
                                     g.Key.Month,
                                     g.Key.Count,
                                     g.Key.name,
                                     RES_COUN = g.Count()

                                 }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE"]), out START_DATE))
            {
                tick = tick.Where(t => t.FEC_TICK >= START_DATE);

            }

            if (DateTime.TryParse(Convert.ToString(Request.Params["END_DATE"]), out END_DATE))
            {
                //END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                //END_DATE = END_DATE.AddSeconds(59);
                tick = tick.Where(t => t.FEC_TICK <= END_DATE);
            }


            //Fecha Programada
            if (Convert.ToString(Request.Params["FechaProgramada"]) != "")
            {
                DateTime FechaProgramada = Convert.ToDateTime(Request.Params["FechaProgramada"]);
                //tick = tick.Where(t => t.ID_STAT_END==5 &&  (db.tktObtenerFechaProgramada(t.ID_TICK).Single()) == FechaProgramada);
                tick = tick.Where(t => t.ID_STAT_END == 5 && ((db.DETAILS_TICKET.Where(x => x.ID_TICK == t.ID_TICK && x.ID_STAT == 5 && x.ID_TYPE_DETA_TICK == 3).OrderByDescending(x => x.ID_DETA_TICK).FirstOrDefault().FEC_SCHE) == null ? t.FEC_INI_TICK :
                    (db.DETAILS_TICKET.Where(x => x.ID_TICK == t.ID_TICK && x.ID_STAT == 5 && x.ID_TYPE_DETA_TICK == 3).OrderByDescending(x => x.ID_DETA_TICK).FirstOrDefault().FEC_SCHE)) == FechaProgramada);
            }


            Count = tick.Count();
            CountClose = tick.Where(t => t.ID_STAT_END == 6).Count();

            try
            {
                from = String.Format("{0:d}", tick.OrderBy(t => t.FEC_TICK).First().FEC_TICK);
                to = String.Format("{0:d}", tick.OrderByDescending(t => t.FEC_TICK).First().FEC_TICK);
            }
            catch
            {

            }

            var resultPie = (from t in tick
                             join st in db.STATUS on t.ID_STAT_END equals st.ID_STAT
                             where t.ID_STAT_END != 6 && t.ID_STAT_END != 2
                             group st by new { st.NAM_GRAP_STAT, st.COL_GRAP_STAT } into g
                             select new
                             {
                                 name = g.Key.NAM_GRAP_STAT.Substring(0, 1) + g.Key.NAM_GRAP_STAT.Substring(1, g.Key.NAM_GRAP_STAT.Length).ToLower(),
                                 color = g.Key.COL_GRAP_STAT,
                                 y = g.Count()
                             }).ToList();


            return Json(new { Count = Count, CountClose = CountClose, From = from, To = to, Pie = resultPie, legend = legend, lineClose = lineClose, lineScheduled = lineScheduled, lineOpen = lineOpen }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FindResultTicket()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string ids = Convert.ToString(Request.Params["ID_SUB_CATE"]);
            string FechaProgramada = Convert.ToString(Request.Params["FechaProgramada"]);
            string START_DATE = Convert.ToString(Request.Params["START_DATE"]),//DateTime.Parse(Convert.ToString(Request.Params["START_DATE"])),
                        END_DATE = Convert.ToString(Request.Params["END_DATE"]);//DateTime.Parse(Convert.ToString(Request.Params["END_DATE"]));
            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                        SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]);
            int ID_CATE = Convert.ToString(Request.Params["ID_CATE"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_CATE"]),
                        ID_SUB_CATE = Convert.ToString(Request.Params["ID_SUB_CATE"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SUB_CATE"]),
                        ID_CLAS = Convert.ToString(Request.Params["ID_CLAS"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_CLAS"]),
                        ID_SUB_CLAS = Convert.ToString(Request.Params["ID_SUB_CLAS"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SUB_CLAS"]),
                        UserId = Convert.ToString(Request.Params["UserId"]) == "" ? 0 : Convert.ToInt32(Request.Params["UserId"]),
                        ID_QUEU = Convert.ToString(Request.Params["ID_QUEU"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_QUEU"]),
                        ID_COMP = Convert.ToString(Request.Params["ID_COMP"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_COMP"]),
                        ID_COMP_END = Convert.ToString(Request.Params["ID_COMP_END"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_COMP_END"]),
                        ID_PERS_ENTI_ASSI = Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI_ASSI"]),
                        ID_TYPE_TICK = Convert.ToString(Request.Params["ID_TYPE_TICK"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_TYPE_TICK"]),
                        ID_PERS_ENTI = Convert.ToString(Request.Params["ID_PERS_ENTI"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI"]),
                        ID_PERS_ENTI_END = Convert.ToString(Request.Params["ID_PERS_ENTI_END"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI_END"]),
                        ID_SOUR = Convert.ToString(Request.Params["ID_SOUR"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SOUR"]),
                        ID_PRIO = Convert.ToString(Request.Params["ID_PRIO"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PRIO"]);
            bool SERVICE = Convert.ToBoolean(Request.Params["SERVICE"].ToString() != "false" ? true : false);

            //Multiple
            string ID_COMP_END_ = "0";
            if (ID_ACCO == 4)
            {
                ID_COMP_END_ = Convert.ToString(Request.Params["IdComps"]);

                if (ID_COMP_END_ == "")
                {
                    ID_COMP_END_ = "0";
                }
                else
                {
                    ID_COMP_END_ = ID_COMP_END_ + ",";
                }
                //
            }


            string estado = Convert.ToString(Request.Params["IdEstados"]).ToString();
            string[] strEstados = new string[0];
            int E1 = 0, E2 = 0, E3 = 0;
            if (estado == "")
            {

            }
            else
            {
                strEstados = estado.Split(',');
                E1 = Int32.Parse(strEstados[0]);
                E2 = strEstados.Count() == 2 ? Int32.Parse(strEstados[1]) : 0;
                E3 = strEstados.Count() == 3 ? Int32.Parse(strEstados[2]) : 0;
            }


            string x = FechaProgramada;
            if (ID_ACCO == (int)Minsur.MARCOBRE
               || ID_ACCO == (int)Minsur.MINSUR
               || ID_ACCO == (int)Minsur.RAURA)
            {
                int idcat3 = Convert.ToInt32(Request.Params["ID_CATEG3"]);
                int idcat4 = Convert.ToInt32(Request.Params["ID_CATEG4"]);
                int idcat5 = Convert.ToInt32(Request.Params["ID_CATEG5"]);
                int idcat6 = Convert.ToInt32(Request.Params["ID_CATEG6"]);

                var result = (from bt in db.BusquedaTicketLista2Minsur(ID_CATE, ID_SUB_CATE, idcat3, idcat4, ID_ACCO,
                                COD_TICK, UserId, ID_QUEU, ID_COMP, ID_COMP_END, ID_PERS_ENTI_ASSI, ID_TYPE_TICK,
                                ID_PRIO, ID_SOUR, SUM_TICK, ID_PERS_ENTI, ID_PERS_ENTI_END, SERVICE, START_DATE,
                                END_DATE, E1, E2, E3, FechaProgramada, idcat5, idcat6) //result
                              select new
                              {
                                  ID_INCI = bt.ID_TICK,
                                  COD_INCI = bt.COD_TICK,
                                  //SUM_INCI = StripHtml(bt.SUM_TICK),
                                  NAM_STAT = bt.NAM_STAT.ToLower(),
                                  //CLA_STAT = bt.CLA_STAT,
                                  //NAM_SOUR = bt.NAM_SOUR.ToLower(),
                                  NAM_SUBC = bt.NAM_CATE1.ToLower(),
                                  NAM_CATE = bt.NAM_CATE2.ToLower(),
                                  NAM_TYPE_TICK = bt.NAM_TYPE_TICK.ToLower(),
                                  //REQU = ReturnRequ(Convert.ToInt32(bt.ID_PERS_ENTI_END)),
                                  REQU = bt.REQUE,
                                  URL = (bt.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                                  CREATE_INCI = String.Format("{0:G}", bt.CREATE_TICK),
                                  MODIFIED_INCI = String.Format("{0:G}", bt.MODIFIED_TICK),
                                  //ID_PRIO = bt.ID_PRIO,
                                  NAM_PRIO = bt.NAM_PRIO.ToLower(),
                                  //ICO_PRIO = bt.ICO_PRIO,
                                  //CREATEBY = (bt.UserId == null ? ReturnRequ(Convert.ToInt32(bt.ID_PERS_ENTI)).ToLower() : bt.Client.ToLower()),
                                  CREATEBY = bt.CREATEBY,
                                  //ACCO = bt.NAM_ACCO,
                                  VIS_COMP = bt.VIS_COMP,
                                  ASSI = bt.Client.ToLower(),
                                  //EXP_TIME = ExpTime(Convert.ToInt32(bt.ID_TICK)),
                                  //EXP_TIME = bt.EXP_TIME,
                                  //ID_STAT_END = bt.ID_STAT_END,
                                  //CIA_TOOL = bt.COMPANY,
                                  //PARENT = (Convert.ToBoolean(bt.IS_PARENT) == true ? "expand" : "none")
                              }).ToList();

                return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                if (ID_COMP_END_ == "0")
                {
                    var result = (from bt in db.BusquedaTicketLista2(ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_ACCO,
                                    COD_TICK, UserId, ID_QUEU, ID_COMP, ID_COMP_END, ID_PERS_ENTI_ASSI, ID_TYPE_TICK,
                                    ID_PRIO, ID_SOUR, SUM_TICK, ID_PERS_ENTI, ID_PERS_ENTI_END, SERVICE, START_DATE,
                                    END_DATE, E1, E2, E3, FechaProgramada) //result
                                  select new
                                  {
                                      ID_INCI = bt.ID_TICK,
                                      COD_INCI = bt.COD_TICK,
                                      //SUM_INCI = StripHtml(bt.SUM_TICK),
                                      NAM_STAT = bt.NAM_STAT.ToLower(),
                                      //CLA_STAT = bt.CLA_STAT,
                                      //NAM_SOUR = bt.NAM_SOUR.ToLower(),
                                      NAM_SUBC = bt.NAM_CATE1.ToLower(),
                                      NAM_CATE = bt.NAM_CATE2.ToLower(),
                                      NAM_TYPE_TICK = bt.NAM_TYPE_TICK.ToLower(),
                                      //REQU = ReturnRequ(Convert.ToInt32(bt.ID_PERS_ENTI_END)),
                                      REQU = bt.REQUE,
                                      URL = (bt.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                                      CREATE_INCI = String.Format("{0:G}", bt.CREATE_TICK),
                                      MODIFIED_INCI = String.Format("{0:G}", bt.MODIFIED_TICK),
                                      //ID_PRIO = bt.ID_PRIO,
                                      NAM_PRIO = bt.NAM_PRIO.ToLower(),
                                      //ICO_PRIO = bt.ICO_PRIO,
                                      //CREATEBY = (bt.UserId == null ? ReturnRequ(Convert.ToInt32(bt.ID_PERS_ENTI)).ToLower() : bt.Client.ToLower()),
                                      CREATEBY = bt.CREATEBY,
                                      //ACCO = bt.NAM_ACCO,
                                      VIS_COMP = bt.VIS_COMP,
                                      ASSI = bt.Client.ToLower(),
                                      //EXP_TIME = ExpTime(Convert.ToInt32(bt.ID_TICK)),
                                      //EXP_TIME = bt.EXP_TIME,
                                      //ID_STAT_END = bt.ID_STAT_END,
                                      //CIA_TOOL = bt.COMPANY,
                                      //PARENT = (Convert.ToBoolean(bt.IS_PARENT) == true ? "expand" : "none")
                                      IdProyectoSLA = bt.IdProyectoSLA
                                  }).ToList();

                    var selectedIdProyectosSLA = Request.Params.GetValues("IdProyectoSLA");

                    if (selectedIdProyectosSLA != null && selectedIdProyectosSLA.Any())
                    {

                        var idProyectosSLAList = selectedIdProyectosSLA.Select(s => int.Parse(s)).ToList();

                        result = result.Where(t => idProyectosSLAList.Contains((int)t.IdProyectoSLA)).ToList();

                        return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

                    }

                    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var result = (from bt in db.BusquedaTicketListaMultiple(ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_ACCO,
                                    COD_TICK, UserId, ID_QUEU, ID_COMP, ID_COMP_END_, ID_PERS_ENTI_ASSI, ID_TYPE_TICK,
                                    ID_PRIO, ID_SOUR, SUM_TICK, ID_PERS_ENTI, ID_PERS_ENTI_END, SERVICE, START_DATE,
                                    END_DATE, E1, E2, E3, FechaProgramada) //result
                                  select new
                                  {
                                      ID_INCI = bt.ID_TICK,
                                      COD_INCI = bt.COD_TICK,
                                      //SUM_INCI = StripHtml(bt.SUM_TICK),
                                      NAM_STAT = bt.NAM_STAT.ToLower(),
                                      //CLA_STAT = bt.CLA_STAT,
                                      //NAM_SOUR = bt.NAM_SOUR.ToLower(),
                                      NAM_SUBC = bt.NAM_CATE1.ToLower(),
                                      NAM_CATE = bt.NAM_CATE2.ToLower(),
                                      NAM_TYPE_TICK = bt.NAM_TYPE_TICK.ToLower(),
                                      //REQU = ReturnRequ(Convert.ToInt32(bt.ID_PERS_ENTI_END)),
                                      REQU = bt.REQUE,
                                      URL = (bt.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                                      CREATE_INCI = String.Format("{0:G}", bt.CREATE_TICK),
                                      MODIFIED_INCI = String.Format("{0:G}", bt.MODIFIED_TICK),
                                      //ID_PRIO = bt.ID_PRIO,
                                      NAM_PRIO = bt.NAM_PRIO.ToLower(),
                                      //ICO_PRIO = bt.ICO_PRIO,
                                      //CREATEBY = (bt.UserId == null ? ReturnRequ(Convert.ToInt32(bt.ID_PERS_ENTI)).ToLower() : bt.Client.ToLower()),
                                      CREATEBY = bt.CREATEBY,
                                      //ACCO = bt.NAM_ACCO,
                                      VIS_COMP = bt.VIS_COMP,
                                      ASSI = bt.Client.ToLower(),
                                      //EXP_TIME = ExpTime(Convert.ToInt32(bt.ID_TICK)),
                                      //EXP_TIME = bt.EXP_TIME,
                                      //ID_STAT_END = bt.ID_STAT_END,
                                      //CIA_TOOL = bt.COMPANY,
                                      //PARENT = (Convert.ToBoolean(bt.IS_PARENT) == true ? "expand" : "none")
                                      IdProyectoSLA = bt.IdProyectoSLA
                                  }).ToList();

                    var selectedIdProyectosSLA = Request.Params.GetValues("IdProyectoSLA");

                    if (selectedIdProyectosSLA != null && selectedIdProyectosSLA.Any())
                    {

                        var idProyectosSLAList = selectedIdProyectosSLA.Select(s => int.Parse(s)).ToList();

                        result = result.Where(t => idProyectosSLAList.Contains((int)t.IdProyectoSLA)).ToList();

                        return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

                    }

                    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                }
            }

            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //int Count = 0, UserId, ID_QUEU, ID_PERS_ENTI_ASSI, ID_TYPE_TICK, ID_PRIO, ID_SOUR, ID_PERS_ENTI, ID_PERS_ENTI_END,
            //    CountClose, ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_STAT_END, ID_COMP, ID_COMP_END;
            //DateTime START_DATE, END_DATE;

            //return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult FindResultTicketBnv(int pagina = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int inicio = 0;
            string ids = Convert.ToString(Request.Params["ID_SUB_CATE"]);
            string FechaProgramada = Convert.ToString(Request.Params["FechaProgramada"]);
            string START_DATE = Convert.ToString(Request.Params["START_DATE"]),//DateTime.Parse(Convert.ToString(Request.Params["START_DATE"])),
                        END_DATE = Convert.ToString(Request.Params["END_DATE"]);//DateTime.Parse(Convert.ToString(Request.Params["END_DATE"]));
            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                        SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]);
            int ID_CATE = Convert.ToString(Request.Params["ID_CATE"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_CATE"]),
                        ID_SUB_CATE = Convert.ToString(Request.Params["ID_SUB_CATE"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SUB_CATE"]),
                        ID_CLAS = Convert.ToString(Request.Params["ID_CLAS"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_CLAS"]),
                        ID_SUB_CLAS = Convert.ToString(Request.Params["ID_SUB_CLAS"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SUB_CLAS"]),
                        UserId = Convert.ToString(Request.Params["UserId"]) == "" ? 0 : Convert.ToInt32(Request.Params["UserId"]),
                        ID_QUEU = Convert.ToString(Request.Params["ID_QUEU"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_QUEU"]),
                        ID_COMP = Convert.ToString(Request.Params["ID_COMP"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_COMP"]),
                        ID_COMP_END = Convert.ToString(Request.Params["ID_COMP_END"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_COMP_END"]),
                        ID_PERS_ENTI_ASSI = Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI_ASSI"]),
                        ID_TYPE_TICK = Convert.ToString(Request.Params["ID_TYPE_TICK"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_TYPE_TICK"]),
                        ID_PERS_ENTI = Convert.ToString(Request.Params["ID_PERS_ENTI"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI"]),
                        ID_PERS_ENTI_END = Convert.ToString(Request.Params["ID_PERS_ENTI_END"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI_END"]),
                        ID_SOUR = Convert.ToString(Request.Params["ID_SOUR"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SOUR"]),
                        ID_PRIO = Convert.ToString(Request.Params["ID_PRIO"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PRIO"]);
            bool SERVICE = Convert.ToBoolean(Request.Params["SERVICE"].ToString() != "false" ? true : false);

            string estado = Convert.ToString(Request.Params["IdEstados"]).ToString();
            //string[] strEstados = estado.Split(',');
            //int E1, E2, E3;
            string[] strEstados = new string[0];
            int E1 = 0, E2 = 0, E3 = 0;
            if (estado == "")
            {

            }
            else
            {
                strEstados = estado.Split(',');
                E1 = Int32.Parse(strEstados[0]);
                E2 = strEstados.Count() == 2 ? Int32.Parse(strEstados[1]) : 0;
                E3 = strEstados.Count() == 3 ? Int32.Parse(strEstados[2]) : 0;
            }
            //E1 = Int32.Parse(strEstados[0]);
            //E2 = strEstados.Count() == 2 ? Int32.Parse(strEstados[1]) : 0;
            //E3 = strEstados.Count() == 3 ? Int32.Parse(strEstados[2]) : 0;

            /*var result = db.BusquedaTicketLista4(ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_ACCO,
                                    COD_TICK, UserId, ID_QUEU, ID_COMP, ID_COMP_END, ID_PERS_ENTI_ASSI, ID_TYPE_TICK,
                                    ID_PRIO, ID_SOUR, SUM_TICK, ID_PERS_ENTI, ID_PERS_ENTI_END, SERVICE, START_DATE,
                                    END_DATE, E1, E2, E3, FechaProgramada).ToList(); //result*/
            string x = FechaProgramada;

            if (pagina > 1)
            {
                inicio = 10 * (pagina - 1);
            }

            List<BusquedaTicketListaPaginado_Result> result = db.BusquedaTicketListaPaginado(ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_ACCO,
                                COD_TICK, UserId, ID_QUEU, ID_COMP, ID_COMP_END, ID_PERS_ENTI_ASSI, ID_TYPE_TICK,
                                ID_PRIO, ID_SOUR, SUM_TICK, ID_PERS_ENTI, ID_PERS_ENTI_END, SERVICE, START_DATE,
                                END_DATE, E1, E2, E3, FechaProgramada, inicio, 10).ToList();

            return PartialView("_ListaTicketsBNV", result);

        }

        public int CantidadFindResultTicketBnv()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string ids = Convert.ToString(Request.Params["ID_SUB_CATE"]);
            string FechaProgramada = Convert.ToString(Request.Params["FechaProgramada"]);
            string START_DATE = Convert.ToString(Request.Params["START_DATE"]),//DateTime.Parse(Convert.ToString(Request.Params["START_DATE"])),
                        END_DATE = Convert.ToString(Request.Params["END_DATE"]);//DateTime.Parse(Convert.ToString(Request.Params["END_DATE"]));
            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                        SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]);
            int ID_CATE = Convert.ToString(Request.Params["ID_CATE"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_CATE"]),
                        ID_SUB_CATE = Convert.ToString(Request.Params["ID_SUB_CATE"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SUB_CATE"]),
                        ID_CLAS = Convert.ToString(Request.Params["ID_CLAS"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_CLAS"]),
                        ID_SUB_CLAS = Convert.ToString(Request.Params["ID_SUB_CLAS"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SUB_CLAS"]),
                        UserId = Convert.ToString(Request.Params["UserId"]) == "" ? 0 : Convert.ToInt32(Request.Params["UserId"]),
                        ID_QUEU = Convert.ToString(Request.Params["ID_QUEU"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_QUEU"]),
                        ID_COMP = Convert.ToString(Request.Params["ID_COMP"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_COMP"]),
                        ID_COMP_END = Convert.ToString(Request.Params["ID_COMP_END"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_COMP_END"]),
                        ID_PERS_ENTI_ASSI = Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI_ASSI"]),
                        ID_TYPE_TICK = Convert.ToString(Request.Params["ID_TYPE_TICK"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_TYPE_TICK"]),
                        ID_PERS_ENTI = Convert.ToString(Request.Params["ID_PERS_ENTI"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI"]),
                        ID_PERS_ENTI_END = Convert.ToString(Request.Params["ID_PERS_ENTI_END"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PERS_ENTI_END"]),
                        ID_SOUR = Convert.ToString(Request.Params["ID_SOUR"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_SOUR"]),
                        ID_PRIO = Convert.ToString(Request.Params["ID_PRIO"]) == "" ? 0 : Convert.ToInt32(Request.Params["ID_PRIO"]);
            bool SERVICE = Convert.ToBoolean(Request.Params["SERVICE"].ToString() != "false" ? true : false);

            string estado = Convert.ToString(Request.Params["IdEstados"]).ToString();
            string[] strEstados = new string[0];
            int E1 = 0, E2 = 0, E3 = 0;
            if (estado == "")
            {

            }
            else
            {
                strEstados = estado.Split(',');
                E1 = Int32.Parse(strEstados[0]);
                E2 = strEstados.Count() == 2 ? Int32.Parse(strEstados[1]) : 0;
                E3 = strEstados.Count() == 3 ? Int32.Parse(strEstados[2]) : 0;
            }
            //string[] strEstados = estado.Split(',');
            //int E1, E2, E3;
            //E1 = Int32.Parse(strEstados[0]);
            //E2 = strEstados.Count() == 2 ? Int32.Parse(strEstados[1]) : 0;
            //E3 = strEstados.Count() == 3 ? Int32.Parse(strEstados[2]) : 0;

            /*var result = db.BusquedaTicketLista4(ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_ACCO,
                                    COD_TICK, UserId, ID_QUEU, ID_COMP, ID_COMP_END, ID_PERS_ENTI_ASSI, ID_TYPE_TICK,
                                    ID_PRIO, ID_SOUR, SUM_TICK, ID_PERS_ENTI, ID_PERS_ENTI_END, SERVICE, START_DATE,
                                    END_DATE, E1, E2, E3, FechaProgramada).ToList(); //result*/
            string x = FechaProgramada;
            var result = db.CantidadBusquedaTicketListaPaginado(ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_ACCO,
                            COD_TICK, UserId, ID_QUEU, ID_COMP, ID_COMP_END, ID_PERS_ENTI_ASSI, ID_TYPE_TICK,
                            ID_PRIO, ID_SOUR, SUM_TICK, ID_PERS_ENTI, ID_PERS_ENTI_END, SERVICE, START_DATE,
                            END_DATE, E1, E2, E3, FechaProgramada).Count();
            int cantidadTicketsActivos = result;
            var cantidadPaginas = cantidadTicketsActivos / 10;
            if (cantidadTicketsActivos > 500)
            {
                cantidadPaginas = 500;
            }
            if (cantidadTicketsActivos % 10 > 0)
            {
                cantidadPaginas = cantidadPaginas + 1;
            }

            return cantidadPaginas;
        }

        public ActionResult Export()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            //int take = Convert.ToInt32(Request.Params["take"].ToString());

            int Count = 0, UserId, ID_QUEU, ID_PERS_ENTI_ASSI, ID_TYPE_TICK, ID_PRIO, ID_SOUR, ID_PERS_ENTI, ID_PERS_ENTI_END,
                CountClose, ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS, ID_STAT_END, ID_COMP, ID_COMP_END, IdProyectoSLA;
            DateTime START_DATE, END_DATE;

            //Seleccion Multiple
            string estado = Convert.ToString(Request.Params["HD_Estados"]).ToString();
            string[] strEstados = estado.Split(',');
            int[] Estados = new int[10];
            int contador = 0;

            if (estado != "")
            {
                foreach (string strEstado in strEstados)
                {

                    Estados[contador] = Convert.ToInt32(strEstado);
                    contador++;
                }
            }


            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                    SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]),
                    from = "", to = "";

            var listCate = db.CATEGORies
                .Join(db.CATEGORies, c1 => c1.ID_CATE, c2 => c2.ID_CATE_PARE, (c1, c2) => new { ID_CATE_1 = c1.ID_CATE, ID_CATE_2 = c2.ID_CATE })
                .Join(db.CATEGORies, c2 => c2.ID_CATE_2, c3 => c3.ID_CATE_PARE, (c2, c3) => new { c2.ID_CATE_1, c2.ID_CATE_2, ID_CATE_3 = c3.ID_CATE })
                .Join(db.CATEGORies, c3 => c3.ID_CATE_3, c4 => c4.ID_CATE_PARE, (c3, c4) => new { c3.ID_CATE_1, c3.ID_CATE_2, c3.ID_CATE_3, ID_CATE_4 = c4.ID_CATE });

            int xcd = listCate.Count();

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE"]), out ID_CATE) == true)
            {
                listCate = listCate.Where(t => t.ID_CATE_1 == ID_CATE);
            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CATE"]), out ID_SUB_CATE) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_2 == ID_SUB_CATE);
                }
            }
            catch
            {

            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CLAS"]), out ID_CLAS) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_3 == ID_CLAS);
                }
            }
            catch
            {

            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CLAS"]), out ID_SUB_CLAS) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_4 == ID_SUB_CLAS);
                }
            }
            catch
            {

            }

            var listClient = (from lu in dbe.ACCOUNT_ENTITY
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

            var tick = (from t in db.TICKETs
                        where t.ID_ACCO == ID_ACCO
                        join lc in listCate on t.ID_CATE equals lc.ID_CATE_4
                        join s in db.STATUS on t.ID_STAT_END equals s.ID_STAT
                        select new
                        {
                            t.ID_ACCO,
                            ID_DOCU_SALE = (t.ID_DOCU_SALE == null ? 0 : t.ID_DOCU_SALE),
                            t.FEC_TICK,
                            t.COD_TICK,
                            t.UserId,
                            t.ID_QUEU,
                            t.ID_PERS_ENTI_ASSI,
                            t.ID_TYPE_TICK,
                            t.ID_PRIO,
                            t.ID_SOUR,
                            t.ID_STAT_END,
                            t.SUM_TICK,
                            t.ID_PERS_ENTI,
                            t.ID_PERS_ENTI_END,
                            t.ID_CATE,
                            t.CREATE_TICK,
                            t.MODIFIED_TICK,
                            t.ID_TICK,
                            t.IS_PARENT,
                            t.ID_TYPE_FORM,
                            s.ORD_STAT,
                            t.ID_COMP,
                            t.ID_COMP_END,
                            t.IdProyectoSLA,
                            t.SERVICE,
                            t.FEC_INI_TICK
                        });
            var schbjsd = tick.Count();
            //SEIS ÚLTIMOS MESES DEL TICKET DE LA CUENTA
            //var legend = "bla";
            var legendx = (from t in tick
                           group t by new { t.FEC_TICK.Value.Year, t.FEC_TICK.Value.Month } into g
                           select new
                           {
                               g.Key.Year,
                               g.Key.Month,
                               Count = g.Count(),
                           }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Take(5);

            var legend = (from t in legendx
                          join am in db.ACCOUNTING_MONTH on t.Year equals am.ID_ACCO_YEAR
                          where t.Month == am.ACCO_MONT
                          select new
                          {
                              t.Year,
                              t.Month,
                              t.Count,
                              name = am.NAM_ACCO_MONT.Substring(0, 1) + am.NAM_ACCO_MONT.Substring(1, 2).ToLower()
                          });



            if (!String.IsNullOrEmpty(COD_TICK))
            {
                tick = tick.Where(t => t.COD_TICK.ToUpper().Contains(COD_TICK.ToUpper()));
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["UserId"]), out UserId) == true)
            {
                tick = tick.Where(t => t.UserId == UserId);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == true)
            {
                tick = tick.Where(t => t.ID_QUEU == ID_QUEU);
            }
            if (estado != "")
            {
                //tick = tick.Where(t => t.ID_STAT_END == ID_STAT_END);
                tick = tick.Where(t => Estados.Contains((int)t.ID_STAT_END));
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP"]), out ID_COMP) == true)
            {
                tick = tick.Where(t => t.ID_COMP == ID_COMP);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP_END"]), out ID_COMP_END) == true)
            {
                tick = tick.Where(t => t.ID_COMP_END == ID_COMP_END);
            }

            var selectedIdProyectosSLA = Request.Form.GetValues("IdProyectoSLA");
            if (selectedIdProyectosSLA != null && selectedIdProyectosSLA.Any())
            {
                // Puedes procesar los valores seleccionados, por ejemplo, convirtiéndolos a una lista de enteros
                var idProyectosSLAList = selectedIdProyectosSLA.Select(s => int.Parse(s)).ToList();

                // Usar los valores en tu lógica, por ejemplo, filtrar por estos valores
                tick = tick.Where(t => idProyectosSLAList.Contains((int)t.IdProyectoSLA));
            }

            //if (Int32.TryParse(Convert.ToString(Request.Params["IdProyectoSLA"]), out IdProyectoSLA) == true)
            //{
            //    tick = tick.Where(t => t.IdProyectoSLA == IdProyectoSLA);
            //}

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == true)
                {
                    tick = tick.Where(t => t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
                }
            }
            catch { }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_TICK"]), out ID_TYPE_TICK) == true)
            {
                tick = tick.Where(t => t.ID_TYPE_TICK == ID_TYPE_TICK);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PRIO"]), out ID_PRIO) == true)
            {
                tick = tick.Where(t => t.ID_PRIO == ID_PRIO);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_SOUR"]), out ID_SOUR) == true)
            {
                tick = tick.Where(t => t.ID_SOUR == ID_SOUR);
            }

            if (!String.IsNullOrEmpty(SUM_TICK))
            {
                tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(SUM_TICK.ToUpper()));
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI == ID_PERS_ENTI);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_END"]), out ID_PERS_ENTI_END) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI_END == ID_PERS_ENTI_END);
            }

            if (Request.Params["SERVICE"].ToString() != "false")
            {
                tick = tick.Where(t => t.SERVICE == true);
            }

            var lineClose = (from x in legend
                             join t in tick on x.Year equals t.FEC_TICK.Value.Year
                             where x.Month == t.FEC_TICK.Value.Month && t.ID_STAT_END == 6
                             group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                             select new
                             {
                                 g.Key.ID_STAT_END,
                                 g.Key.Year,
                                 g.Key.Month,
                                 g.Key.Count,
                                 g.Key.name,
                                 RES_COUN = g.Count()

                             }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineOpen = (from x in legend
                            join t in tick on x.Year equals t.FEC_TICK.Value.Year
                            where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)
                            group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                            select new
                            {
                                g.Key.ID_STAT_END,
                                g.Key.Year,
                                g.Key.Month,
                                g.Key.Count,
                                g.Key.name,
                                RES_COUN = g.Count()

                            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineScheduled = (from x in legend
                                 join t in tick on x.Year equals t.FEC_TICK.Value.Year
                                 where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 5)
                                 group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                                 select new
                                 {
                                     g.Key.ID_STAT_END,
                                     g.Key.Year,
                                     g.Key.Month,
                                     g.Key.Count,
                                     g.Key.name,
                                     RES_COUN = g.Count()

                                 }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE"]), out START_DATE))
            {
                tick = tick.Where(t => t.FEC_TICK >= START_DATE);

            }

            if (DateTime.TryParse(Convert.ToString(Request.Params["END_DATE"]), out END_DATE))
            {
                //END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                //END_DATE = END_DATE.AddSeconds(59);
                tick = tick.Where(t => t.FEC_TICK <= END_DATE);
            }

            //Fecha Programada
            if (Convert.ToString(Request.Params["FechaProgramada"]) != "")
            {
                DateTime FechaProgramada = Convert.ToDateTime(Request.Params["FechaProgramada"]);
                //tick = tick.Where(t => t.ID_STAT_END==5 &&  (db.tktObtenerFechaProgramada(t.ID_TICK).Single()) == FechaProgramada);
                tick = tick.Where(t => t.ID_STAT_END == 5 && ((db.DETAILS_TICKET.Where(x => x.ID_TICK == t.ID_TICK && x.ID_STAT == 5 && x.ID_TYPE_DETA_TICK == 3).OrderByDescending(x => x.ID_DETA_TICK).FirstOrDefault().FEC_SCHE) == null ? t.FEC_INI_TICK :
                    (db.DETAILS_TICKET.Where(x => x.ID_TICK == t.ID_TICK && x.ID_STAT == 5 && x.ID_TYPE_DETA_TICK == 3).OrderByDescending(x => x.ID_DETA_TICK).FirstOrDefault().FEC_SCHE)) == FechaProgramada);
            }


            Count = tick.Count();
            CountClose = tick.Where(t => t.ID_STAT_END == 6).Count();
            try
            {
                from = String.Format("{0:d}", tick.OrderBy(t => t.FEC_TICK).First().FEC_TICK);
                to = String.Format("{0:d}", tick.OrderByDescending(t => t.FEC_TICK).First().FEC_TICK);
            }
            catch
            {

            }

            var resultPie = (from t in tick
                             join st in db.STATUS on t.ID_STAT_END equals st.ID_STAT
                             where t.ID_STAT_END != 6 && t.ID_STAT_END != 2
                             group st by new { st.NAM_GRAP_STAT, st.COL_GRAP_STAT } into g
                             select new
                             {
                                 name = g.Key.NAM_GRAP_STAT.Substring(0, 1) + g.Key.NAM_GRAP_STAT.Substring(1, g.Key.NAM_GRAP_STAT.Length).ToLower(),
                                 color = g.Key.COL_GRAP_STAT,
                                 y = g.Count()
                             }).ToList();

            tick = tick.OrderBy(t => t.ORD_STAT).ThenByDescending(t => t.FEC_TICK);
            var grid = new GridView();

            var result = (from i in tick.ToList()
                          join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          //join cr in listClient on i.UserId equals cr.UserId
                          join cr in listClient on (i.UserId == null ? -1 : i.UserId) equals (cr.UserId == null ? 0 : cr.UserId) into userl
                          from usl in userl.DefaultIfEmpty()
                          join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listClient on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                          select new
                          {
                              Codigo_Ticket = i.COD_TICK,
                              Comentario = StripHtml(i.SUM_TICK),
                              Estado = s.NAM_STAT,
                              Medio_Comunicacion = so.NAM_SOUR,
                              Servicio = c4.NAM_CATE,
                              Incidente_Requerimiento = c3.NAM_CATE,
                              Tipo_Ticket = tt.NAM_TYPE_TICK,
                              //REQU = re.Client.ToLower(),
                              Usuario_Afectado = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                              URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                              Fecha_Creacion = String.Format("{0:G}", i.CREATE_TICK),
                              Fecha_Modificacion = String.Format("{0:G}", i.MODIFIED_TICK),
                              Prioridad = pr.NAM_PRIO,
                              //Usuario_Creacion = cr.Client,
                              Usuario_Creacion = i.UserId == null ? "" : usl.Client,
                              //Usuario_Creacion = (usl.UserId == null ? "" : usl.Client),
                              Cuenta = ac.NAM_ACCO,
                              Usuario_Asignado = asi.Client,
                              Tiempo_Expiracion = ExpTime(i.ID_TICK),
                              //ID_PERS_ENTI = 
                              Compania = cia.COMPANY,//(xcia == null ? "" : xcia.COMPANY),
                              Ticket_Padre = (i.IS_PARENT == true ? "SI" : "NO")
                          }).Take(2000);

            grid.DataSource = result.ToList();

            if (Convert.ToInt32(Session["ID_ACCO"]) == 4)
            {
                var resultElectrodata = (from i in tick.ToList()
                                         join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                                         join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                                         join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                                         join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                                         join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                                         join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                                         //join cr in listClient on i.UserId equals cr.UserId
                                         join cr in listClient on (i.UserId == null ? -1 : i.UserId) equals (cr.UserId == null ? 0 : cr.UserId) into userl
                                         from usl in userl.DefaultIfEmpty()
                                         join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                                         join asi in listClient on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                                         join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                                         join dsale in db.DOCUMENT_SALE on i.IdProyectoSLA equals dsale.ID_DOCU_SALE into dsa
                                         from dsale in dsa.DefaultIfEmpty()
                                         select new
                                         {
                                             Codigo_Ticket = i.COD_TICK,
                                             Comentario = StripHtml(i.SUM_TICK),
                                             Estado = s.NAM_STAT,
                                             Medio_Comunicacion = so.NAM_SOUR,
                                             Servicio = c4.NAM_CATE,
                                             Incidente_Requerimiento = c3.NAM_CATE,
                                             Tipo_Ticket = tt.NAM_TYPE_TICK,
                                             //REQU = re.Client.ToLower(),
                                             Usuario_Afectado = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                                             URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                                             Fecha_Creacion = String.Format("{0:G}", i.CREATE_TICK),
                                             Fecha_Modificacion = String.Format("{0:G}", i.MODIFIED_TICK),
                                             Prioridad = pr.NAM_PRIO,
                                             //Usuario_Creacion = cr.Client,
                                             Usuario_Creacion = i.UserId == null ? "" : usl.Client,
                                             //Usuario_Creacion = (usl.UserId == null ? "" : usl.Client),
                                             Cuenta = ac.NAM_ACCO,
                                             Usuario_Asignado = asi.Client,
                                             Tiempo_Expiracion = ExpTime(i.ID_TICK),
                                             //ID_PERS_ENTI = 
                                             Compania = cia.COMPANY,//(xcia == null ? "" : xcia.COMPANY),
                                             Ticket_Padre = (i.IS_PARENT == true ? "SI" : "NO"),
                                             OP = (Convert.ToInt32(Session["ID_ACCO"]) == 4) ?
                                      (dsale == null ? "" : (dsale.NUM_DOCU_SALE == null ? "" : ((dsale.TYPE_DOCUMENT_SALE == null ? "" : dsale.TYPE_DOCUMENT_SALE.NAM_TYPE_DOCU_SALE) + " " + dsale.NUM_DOCU_SALE))) :
                                      ""
                                         }).Take(2000);

                grid.DataSource = resultElectrodata.ToList();
            }


            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=FindTicket" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();

        }

        public ActionResult ExportFindTicketMinsur()
        {
            int compania = 0, companiaFinal = 0, solicitante = 0, usuarioFinal = 0, area = 0, asignado = 0, tipoTick = 0, prio = 0, source = 0, creador = 0, cate1 = 0, cate2 = 0, cate3 = 0, cate4 = 0, cate5 = 0, cate6 = 0;
            DateTime? fechaInicio = null, fechaFin = null, fechaProgra = null;

            int idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            string codTick = Convert.ToString(Request.Params["COD_TICK"]);
            string estados = Request.Params["Estados"];
            string comentario = Convert.ToString(Request.Params["SUM_TICK"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_COMP"]))
                compania = Convert.ToInt32(Request.Params["ID_COMP"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_COMP_END"]))
                companiaFinal = Convert.ToInt32(Request.Params["ID_COMP_END"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI"]))
                solicitante = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI_END"]))
                usuarioFinal = Convert.ToInt32(Request.Params["ID_PERS_ENTI_END"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_QUEU"]))
                area = Convert.ToInt32(Request.Params["ID_QUEU"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI_ASSI"]))
                asignado = Convert.ToInt32(Request.Params["ID_PERS_ENTI_ASSI"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_TYPE_TICK"]))
                tipoTick = Convert.ToInt32(Request.Params["ID_TYPE_TICK"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PRIO"]))
                prio = Convert.ToInt32(Request.Params["ID_PRIO"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CATE"]))
                cate1 = Convert.ToInt32(Request.Params["ID_CATE"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_SUB_CATE"]))
                cate2 = Convert.ToInt32(Request.Params["ID_SUB_CATE"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CATEG3"]))
                cate3 = Convert.ToInt32(Request.Params["ID_CATEG3"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CATEG4"]))
                cate4 = Convert.ToInt32(Request.Params["ID_CATEG4"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CATEG5"]))
                cate5 = Convert.ToInt32(Request.Params["ID_CATEG5"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CATEG6"]))
                cate6 = Convert.ToInt32(Request.Params["ID_CATEG6"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_SOUR"]))
                source = Convert.ToInt32(Request.Params["ID_SOUR"]);

            if (!String.IsNullOrEmpty(Request.Params["UserId"]))
                creador = Convert.ToInt32(Request.Params["UserId"]);

            if (!String.IsNullOrEmpty(Request.Params["START_DATE"]))
                fechaInicio = Convert.ToDateTime(Request.Params["START_DATE"]);

            if (!String.IsNullOrEmpty(Request.Params["END_DATE"]))
                fechaFin = Convert.ToDateTime(Request.Params["END_DATE"]);

            if (!String.IsNullOrEmpty(Request.Params["FechaProgramada"]))
                fechaProgra = Convert.ToDateTime(Request.Params["FechaProgramada"]);

            DataTable dataTable = ReporteFindMinsur(idCuenta, codTick, estados, compania, companiaFinal, solicitante, usuarioFinal, area, asignado, tipoTick, prio, cate1, cate2, cate3, cate4, cate5, cate6, source, comentario, fechaInicio, fechaFin, creador, fechaProgra);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Crear el paquete de Excel y llenar la hoja de cálculo
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Búsqueda");
                var fechaColumn = ws.Column(3);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(4);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(18);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                fechaColumn = ws.Column(19);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);

                ws.Column(1).Width = 15;
                ws.Column(2).Width = 17;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(6).Width = 25;
                ws.Column(7).Width = 30;
                ws.Column(8).Width = 25;
                ws.Column(9).Width = 35;
                ws.Column(10).Width = 35;
                ws.Column(11).Width = 25;
                ws.Column(12).Width = 30;
                ws.Column(13).Width = 25;
                ws.Column(14).Width = 25;
                ws.Column(15).Width = 15;
                ws.Column(16).Width = 15;
                ws.Column(17).Width = 17;
                ws.Column(18).Width = 20;
                ws.Column(19).Width = 20;

                ws.Column(20).Width = 20;
                ws.Column(21).Width = 40;
                ws.Column(22).Width = 40;
                ws.Column(23).Width = 40;
                ws.Column(24).Width = 40;
                ws.Column(25).Width = 40;
                ws.Column(26).Width = 25;

                ws.Column(27).Width = 35;
                ws.Column(28).Width = 90;
                ws.Column(29).Width = 25;
                ws.Column(30).Width = 90;
                ws.Column(31).Width = 60;
                ws.Column(32).Width = 60;

                ws.Column(33).Width = 20;
                ws.Column(34).Width = 25;
                ws.Column(35).Width = 20;
                ws.Column(36).Width = 30;
                ws.Column(37).Width = 10;
                ws.Column(38).Width = 10;

                ws.Column(39).Width = 15;
                ws.Column(40).Width = 20;
                ws.Column(41).Width = 15;
                ws.Column(42).Width = 20;


                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);

                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=FindTicket" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                ms.WriteTo(HttpContext.Response.OutputStream);
                HttpContext.Response.End();
            }

            return View();
        }

        private DataTable ReporteFindMinsur(int idCuenta, string codTick, string estados, int compania, int companiaFinal, int solicitante, int usuarioFinal, int areaRespon, int asignado, int tipoTick, int prioridad, int cate1, int cate2, int cate3, int cate4, int cate5, int cate6, int source, string comentario, DateTime? fecIni, DateTime? fecFin, int creador, DateTime? fecProgra)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ExportarBusquedaTicketMinsur", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@IdCuenta", idCuenta);
                    cmd.Parameters.AddWithValue("@CodTicket", codTick);
                    cmd.Parameters.AddWithValue("@Estados", estados);
                    cmd.Parameters.AddWithValue("@Compania", compania);
                    cmd.Parameters.AddWithValue("@CompaniaFinal", companiaFinal);
                    cmd.Parameters.AddWithValue("@Solicitante", solicitante);
                    cmd.Parameters.AddWithValue("@UsuarioFinal", usuarioFinal);
                    cmd.Parameters.AddWithValue("@AreaResponsable", areaRespon);
                    cmd.Parameters.AddWithValue("@Asignado", asignado);
                    cmd.Parameters.AddWithValue("@TipoTicket", tipoTick);
                    cmd.Parameters.AddWithValue("@Prioridad", prioridad);
                    cmd.Parameters.AddWithValue("@Cate1", cate1);
                    cmd.Parameters.AddWithValue("@Cate2", cate2);
                    cmd.Parameters.AddWithValue("@Cate3", cate3);
                    cmd.Parameters.AddWithValue("@Cate4", cate4);
                    cmd.Parameters.AddWithValue("@Cate5", cate5);
                    cmd.Parameters.AddWithValue("@Cate6", cate6);
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@Comentario", comentario);
                    if (fecIni.HasValue)
                        cmd.Parameters.AddWithValue("@FechaInicio", fecIni.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaInicio", DBNull.Value);
                    if (fecFin.HasValue)
                        cmd.Parameters.AddWithValue("@FechaFin", fecFin.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaFin", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Creador", creador);
                    if (fecProgra.HasValue)
                        cmd.Parameters.AddWithValue("@FechaProgramada", fecProgra.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaProgramada", DBNull.Value);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public ActionResult ExportFindTicketBNV()
        {
            int compania = 0, companiaFinal = 0, solicitante = 0, usuarioFinal = 0, area = 0, asignado = 0, tipoTick = 0, prio = 0, source = 0, creador = 0, cate1 = 0, cate2 = 0, cate3 = 0, cate4 = 0;
            DateTime? fechaInicio = null, fechaFin = null, fechaProgra = null;

            int idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            string codTick = Convert.ToString(Request.Params["COD_TICK"]);
            string estados = Request.Params["Estados"];
            string comentario = Convert.ToString(Request.Params["SUM_TICK"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_COMP"]))
                compania = Convert.ToInt32(Request.Params["ID_COMP"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_COMP_END"]))
                companiaFinal = Convert.ToInt32(Request.Params["ID_COMP_END"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI"]))
                solicitante = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI_END"]))
                usuarioFinal = Convert.ToInt32(Request.Params["ID_PERS_ENTI_END"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_QUEU"]))
                area = Convert.ToInt32(Request.Params["ID_QUEU"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI_ASSI"]))
                asignado = Convert.ToInt32(Request.Params["ID_PERS_ENTI_ASSI"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_TYPE_TICK"]))
                tipoTick = Convert.ToInt32(Request.Params["ID_TYPE_TICK"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PRIO"]))
                prio = Convert.ToInt32(Request.Params["ID_PRIO"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CATE"]))
                cate1 = Convert.ToInt32(Request.Params["ID_CATE"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_SUB_CATE"]))
                cate2 = Convert.ToInt32(Request.Params["ID_SUB_CATE"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CLAS"]))
                cate3 = Convert.ToInt32(Request.Params["ID_CLAS"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_SUB_CLAS"]))
                cate4 = Convert.ToInt32(Request.Params["ID_SUB_CLAS"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_SOUR"]))
                source = Convert.ToInt32(Request.Params["ID_SOUR"]);

            if (!String.IsNullOrEmpty(Request.Params["UserId"]))
                creador = Convert.ToInt32(Request.Params["UserId"]);

            if (!String.IsNullOrEmpty(Request.Params["START_DATE"]))
                fechaInicio = Convert.ToDateTime(Request.Params["START_DATE"]);

            if (!String.IsNullOrEmpty(Request.Params["END_DATE"]))
                fechaFin = Convert.ToDateTime(Request.Params["END_DATE"]);

            if (!String.IsNullOrEmpty(Request.Params["FechaProgramada"]))
                fechaProgra = Convert.ToDateTime(Request.Params["FechaProgramada"]);

            DataTable dataTable = ReporteFindBNV(idCuenta, codTick, estados, compania, companiaFinal, solicitante, usuarioFinal, area, asignado, tipoTick, prio, cate1, cate2, cate3, cate4, source, comentario, fechaInicio, fechaFin, creador, fechaProgra);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Crear el paquete de Excel y llenar la hoja de cálculo
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Búsqueda");
                var fechaColumn = ws.Column(3);
                fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 17;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 20;
                ws.Column(7).Width = 30;
                ws.Column(8).Width = 35;
                ws.Column(9).Width = 35;
                ws.Column(10).Width = 35;
                ws.Column(11).Width = 25;
                ws.Column(12).Width = 30;
                ws.Column(13).Width = 40;
                ws.Column(14).Width = 40;
                ws.Column(15).Width = 15;
                ws.Column(16).Width = 20;
                ws.Column(17).Width = 25;
                ws.Column(18).Width = 25;
                ws.Column(19).Width = 25;

                ws.Column(20).Width = 20;
                ws.Column(21).Width = 40;
                ws.Column(22).Width = 40;
                ws.Column(23).Width = 40;
                ws.Column(24).Width = 40;
                ws.Column(25).Width = 40;
                ws.Column(26).Width = 25;

                ws.Column(27).Width = 90;
                ws.Column(28).Width = 80;
                ws.Column(29).Width = 40;
                ws.Column(30).Width = 90;
                ws.Column(31).Width = 60;
                ws.Column(32).Width = 60;

                ws.Column(33).Width = 10;
                ws.Column(34).Width = 25;
                ws.Column(35).Width = 20;
                ws.Column(36).Width = 20;
                ws.Column(37).Width = 10;
                ws.Column(38).Width = 10;

                ws.Column(39).Width = 15;
                ws.Column(40).Width = 20;
                ws.Column(41).Width = 15;
                ws.Column(42).Width = 20;
                ws.Column(43).Width = 20;
                ws.Column(44).Width = 20;
                ws.Column(45).Width = 20;
                ws.Column(46).Width = 20;
                ws.Column(47).Width = 20;
                ws.Column(48).Width = 80;
                ws.Column(49).Width = 25;
                ws.Column(50).Width = 25;

                ws.Column(51).Width = 20;
                ws.Column(52).Width = 20;
                ws.Column(53).Width = 40;
                ws.Column(54).Width = 20;

                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);

                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=FindTicket" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                ms.WriteTo(HttpContext.Response.OutputStream);
                HttpContext.Response.End();
            }

            return View();
        }

        private DataTable ReporteFindBNV(int idCuenta, string codTick, string estados, int compania, int companiaFinal, int solicitante, int usuarioFinal, int areaRespon, int asignado, int tipoTick, int prioridad, int cate1, int cate2, int cate3, int cate4, int source, string comentario, DateTime? fecIni, DateTime? fecFin, int creador, DateTime? fecProgra)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ExportarBusquedaTicketBNV", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@IdCuenta", idCuenta);
                    cmd.Parameters.AddWithValue("@CodTicket", codTick);
                    cmd.Parameters.AddWithValue("@Estados", estados);
                    cmd.Parameters.AddWithValue("@Compania", compania);
                    cmd.Parameters.AddWithValue("@CompaniaFinal", companiaFinal);
                    cmd.Parameters.AddWithValue("@Solicitante", solicitante);
                    cmd.Parameters.AddWithValue("@UsuarioFinal", usuarioFinal);
                    cmd.Parameters.AddWithValue("@AreaResponsable", areaRespon);
                    cmd.Parameters.AddWithValue("@Asignado", asignado);
                    cmd.Parameters.AddWithValue("@TipoTicket", tipoTick);
                    cmd.Parameters.AddWithValue("@Prioridad", prioridad);
                    cmd.Parameters.AddWithValue("@Cate1", cate1);
                    cmd.Parameters.AddWithValue("@Cate2", cate2);
                    cmd.Parameters.AddWithValue("@Cate3", cate3);
                    cmd.Parameters.AddWithValue("@Cate4", cate4);
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@Comentario", comentario);
                    if (fecIni.HasValue)
                        cmd.Parameters.AddWithValue("@FechaInicio", fecIni.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaInicio", DBNull.Value);
                    if (fecFin.HasValue)
                        cmd.Parameters.AddWithValue("@FechaFin", fecFin.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaFin", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Creador", creador);
                    if (fecProgra.HasValue)
                        cmd.Parameters.AddWithValue("@FechaProgramada", fecProgra.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaProgramada", DBNull.Value);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public ActionResult ExportFindTicket()
        {
            int compania = 0, companiaFinal = 0, solicitante = 0, usuarioFinal = 0, area = 0, asignado = 0, tipoTick = 0, prio = 0, source = 0, creador = 0, cate1 = 0, cate2 = 0, cate3 = 0, cate4 = 0;
            DateTime? fechaInicio = null, fechaFin = null, fechaProgra = null;

            int idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            string codTick = Convert.ToString(Request.Params["COD_TICK"]);
            string estados = Request.Params["Estados"];
            string comentario = Convert.ToString(Request.Params["SUM_TICK"]);
            string estadosNum = Request.Params["HD_Estados"].ToString();

            if (!String.IsNullOrEmpty(Request.Params["ID_COMP"]))
                compania = Convert.ToInt32(Request.Params["ID_COMP"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_COMP_END"]))
                companiaFinal = Convert.ToInt32(Request.Params["ID_COMP_END"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI"]))
                solicitante = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI_END"]))
                usuarioFinal = Convert.ToInt32(Request.Params["ID_PERS_ENTI_END"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_QUEU"]))
                area = Convert.ToInt32(Request.Params["ID_QUEU"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PERS_ENTI_ASSI"]))
                asignado = Convert.ToInt32(Request.Params["ID_PERS_ENTI_ASSI"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_TYPE_TICK"]))
                tipoTick = Convert.ToInt32(Request.Params["ID_TYPE_TICK"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_PRIO"]))
                prio = Convert.ToInt32(Request.Params["ID_PRIO"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CATE"]))
                cate1 = Convert.ToInt32(Request.Params["ID_CATE"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_SUB_CATE"]))
                cate2 = Convert.ToInt32(Request.Params["ID_SUB_CATE"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_CLAS"]))
                cate3 = Convert.ToInt32(Request.Params["ID_CLAS"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_SUB_CLAS"]))
                cate4 = Convert.ToInt32(Request.Params["ID_SUB_CLAS"]);

            if (!String.IsNullOrEmpty(Request.Params["ID_SOUR"]))
                source = Convert.ToInt32(Request.Params["ID_SOUR"]);

            if (!String.IsNullOrEmpty(Request.Params["UserId"]))
                creador = Convert.ToInt32(Request.Params["UserId"]);

            if (!String.IsNullOrEmpty(Request.Params["START_DATE"]))
                fechaInicio = Convert.ToDateTime(Request.Params["START_DATE"]);

            if (!String.IsNullOrEmpty(Request.Params["END_DATE"]))
                fechaFin = Convert.ToDateTime(Request.Params["END_DATE"]);

            if (!String.IsNullOrEmpty(Request.Params["FechaProgramada"]))
                fechaProgra = Convert.ToDateTime(Request.Params["FechaProgramada"]);

            if (idCuenta == 4)
            {
                string ops = Request.Params["OPS"];
                string compsFinales = Request.Params["CompsFinal"];
                DataTable dataTable = ReporteFindEdata(idCuenta, codTick, estados, compania, compsFinales, ops, solicitante, usuarioFinal, area, asignado, tipoTick, prio, cate1, cate2, cate3, cate4, source, comentario, fechaInicio, fechaFin, creador, fechaProgra);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Crear el paquete de Excel y llenar la hoja de cálculo
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Búsqueda");
                    var fechaColumn = ws.Column(4);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(5);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(19);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(20);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(35);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                    ws.Column(1).Width = 15;
                    ws.Column(2).Width = 17;
                    ws.Column(3).Width = 20;
                    ws.Column(4).Width = 20;
                    ws.Column(6).Width = 25;
                    ws.Column(7).Width = 30;
                    ws.Column(8).Width = 35;
                    ws.Column(9).Width = 35;
                    ws.Column(10).Width = 35;
                    ws.Column(11).Width = 25;
                    ws.Column(12).Width = 30;
                    ws.Column(13).Width = 40;
                    ws.Column(14).Width = 40;
                    ws.Column(15).Width = 15;
                    ws.Column(16).Width = 20;
                    ws.Column(17).Width = 25;
                    ws.Column(18).Width = 25;
                    ws.Column(19).Width = 25;

                    ws.Column(20).Width = 20;
                    ws.Column(21).Width = 40;
                    ws.Column(22).Width = 40;
                    ws.Column(23).Width = 40;
                    ws.Column(24).Width = 40;
                    ws.Column(25).Width = 40;
                    ws.Column(26).Width = 90;

                    ws.Column(27).Width = 25;
                    ws.Column(28).Width = 100;
                    ws.Column(29).Width = 50;
                    ws.Column(30).Width = 60;
                    ws.Column(31).Width = 25;
                    ws.Column(32).Width = 15;

                    ws.Column(33).Width = 15;
                    ws.Column(34).Width = 25;
                    ws.Column(35).Width = 20;
                    ws.Column(36).Width = 30;
                    ws.Column(37).Width = 15;
                    ws.Column(38).Width = 12;

                    ws.Column(39).Width = 14;
                    ws.Column(40).Width = 20;
                    ws.Column(41).Width = 20;
                    ws.Column(42).Width = 18;
                    ws.Column(43).Width = 18;
                    ws.Column(44).Width = 60;

                    var ms = new System.IO.MemoryStream();
                    pck.SaveAs(ms);

                    HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=FindTicket" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                    ms.WriteTo(HttpContext.Response.OutputStream);
                    HttpContext.Response.End();
                }

            }
            else if (idCuenta == 1 || idCuenta == 25)
            {
                DataTable dataTable = ReporteFindGF(idCuenta, codTick, estados, compania, companiaFinal, solicitante, usuarioFinal, area, asignado, tipoTick, prio, cate1, cate2, cate3, cate4, source, comentario, fechaInicio, fechaFin, creador, fechaProgra);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Crear el paquete de Excel y llenar la hoja de cálculo
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Búsqueda");
                    var fechaColumn = ws.Column(3);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(4);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(18);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(19);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";

                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy HH:mm tt";
                    ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                    ws.Column(1).Width = 10;
                    ws.Column(2).Width = 17;
                    ws.Column(3).Width = 20;
                    ws.Column(6).Width = 20;
                    ws.Column(7).Width = 20;
                    ws.Column(11).Width = 25;
                    ws.Column(12).Width = 30;
                    ws.Column(13).Width = 40;
                    ws.Column(14).Width = 40;
                    ws.Column(15).Width = 35;
                    ws.Column(16).Width = 30;
                    ws.Column(17).Width = 25;
                    ws.Column(18).Width = 15;
                    ws.Column(19).Width = 25;

                    ws.Column(20).Width = 20;
                    ws.Column(21).Width = 40;
                    ws.Column(22).Width = 40;
                    ws.Column(23).Width = 40;
                    ws.Column(24).Width = 40;
                    ws.Column(25).Width = 15;
                    ws.Column(26).Width = 15;

                    ws.Column(27).Width = 25;
                    ws.Column(28).Width = 80;
                    ws.Column(32).Width = 20;

                    ws.Column(34).Width = 35;
                    ws.Column(35).Width = 40;
                    ws.Column(36).Width = 40;
                    ws.Column(38).Width = 20;

                    ws.Column(39).Width = 15;
                    ws.Column(40).Width = 20;
                    ws.Column(41).Width = 10;
                    ws.Column(52).Width = 20;

                    var ms = new System.IO.MemoryStream();
                    pck.SaveAs(ms);

                    HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=FindTicket" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                    ms.WriteTo(HttpContext.Response.OutputStream);
                    HttpContext.Response.End();
                }
            }
            else
            {
                DataTable dataTable = ReporteFind(idCuenta, codTick, estadosNum, compania, companiaFinal, solicitante, usuarioFinal, area, asignado, tipoTick, prio, cate1, cate2, cate3, cate4, source, comentario, fechaInicio, fechaFin, creador, fechaProgra);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Crear el paquete de Excel y llenar la hoja de cálculo
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Búsqueda");
                    var fechaColumn = ws.Column(4);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(5);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(9);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    fechaColumn = ws.Column(45);
                    fechaColumn.Style.Numberformat.Format = "dd/MM/yyyy hh:mm";

                    ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                    ws.Column(1).Width = 15;
                    ws.Column(2).Width = 17;
                    ws.Column(3).Width = 20;
                    ws.Column(4).Width = 20;
                    ws.Column(6).Width = 25;
                    ws.Column(7).Width = 30;
                    ws.Column(8).Width = 35;
                    ws.Column(9).Width = 35;
                    ws.Column(10).Width = 35;
                    ws.Column(11).Width = 25;
                    ws.Column(12).Width = 30;
                    ws.Column(13).Width = 40;
                    ws.Column(14).Width = 40;
                    ws.Column(15).Width = 15;
                    ws.Column(16).Width = 20;
                    ws.Column(17).Width = 25;
                    ws.Column(18).Width = 25;
                    ws.Column(19).Width = 25;

                    ws.Column(20).Width = 20;
                    ws.Column(21).Width = 40;
                    ws.Column(22).Width = 40;
                    ws.Column(23).Width = 40;
                    ws.Column(24).Width = 40;
                    ws.Column(25).Width = 40;
                    ws.Column(26).Width = 90;

                    ws.Column(27).Width = 25;
                    ws.Column(28).Width = 80;
                    ws.Column(29).Width = 40;
                    ws.Column(30).Width = 90;
                    ws.Column(31).Width = 60;
                    ws.Column(32).Width = 20;

                    ws.Column(33).Width = 20;
                    ws.Column(34).Width = 25;
                    ws.Column(35).Width = 20;
                    ws.Column(36).Width = 20;
                    ws.Column(37).Width = 10;
                    ws.Column(38).Width = 10;

                    ws.Column(39).Width = 15;
                    ws.Column(40).Width = 20;

                    var ms = new System.IO.MemoryStream();
                    pck.SaveAs(ms);

                    HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=FindTicket" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                    ms.WriteTo(HttpContext.Response.OutputStream);
                    HttpContext.Response.End();
                }
            }

            return View();
        }

        private DataTable ReporteFindEdata(int idCuenta, string codTick, string estados, int compania, string companiaFinal, string ops, int solicitante, int usuarioFinal, int areaRespon, int asignado, int tipoTick, int prioridad, int cate1, int cate2, int cate3, int cate4, int source, string comentario, DateTime? fecIni, DateTime? fecFin, int creador, DateTime? fecProgra)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ExportarBusquedaTicketEdata", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@IdCuenta", idCuenta);
                    cmd.Parameters.AddWithValue("@CodTicket", codTick);
                    cmd.Parameters.AddWithValue("@Estados", estados);
                    cmd.Parameters.AddWithValue("@Compania", compania);
                    cmd.Parameters.AddWithValue("@CompaniaFinal", companiaFinal);
                    cmd.Parameters.AddWithValue("@NumOP", ops);
                    cmd.Parameters.AddWithValue("@Solicitante", solicitante);
                    cmd.Parameters.AddWithValue("@UsuarioFinal", usuarioFinal);
                    cmd.Parameters.AddWithValue("@AreaResponsable", areaRespon);
                    cmd.Parameters.AddWithValue("@Asignado", asignado);
                    cmd.Parameters.AddWithValue("@TipoTicket", tipoTick);
                    cmd.Parameters.AddWithValue("@Prioridad", prioridad);
                    cmd.Parameters.AddWithValue("@Cate1", cate1);
                    cmd.Parameters.AddWithValue("@Cate2", cate2);
                    cmd.Parameters.AddWithValue("@Cate3", cate3);
                    cmd.Parameters.AddWithValue("@Cate4", cate4);
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@Comentario", comentario);
                    if (fecIni.HasValue)
                        cmd.Parameters.AddWithValue("@FechaInicio", fecIni.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaInicio", DBNull.Value);
                    if (fecFin.HasValue)
                        cmd.Parameters.AddWithValue("@FechaFin", fecFin.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaFin", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Creador", creador);
                    if (fecProgra.HasValue)
                        cmd.Parameters.AddWithValue("@FechaProgramada", fecProgra.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaProgramada", DBNull.Value);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        private DataTable ReporteFindGF(int idCuenta, string codTick, string estados, int compania, int companiaFinal, int solicitante, int usuarioFinal, int areaRespon, int asignado, int tipoTick, int prioridad, int cate1, int cate2, int cate3, int cate4, int source, string comentario, DateTime? fecIni, DateTime? fecFin, int creador, DateTime? fecProgra)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ExportarBusquedaTicketGF", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@IdCuenta", idCuenta);
                    cmd.Parameters.AddWithValue("@CodTicket", codTick);
                    cmd.Parameters.AddWithValue("@Estados", estados);
                    cmd.Parameters.AddWithValue("@Compania", compania);
                    cmd.Parameters.AddWithValue("@CompaniaFinal", companiaFinal);
                    cmd.Parameters.AddWithValue("@Solicitante", solicitante);
                    cmd.Parameters.AddWithValue("@UsuarioFinal", usuarioFinal);
                    cmd.Parameters.AddWithValue("@AreaResponsable", areaRespon);
                    cmd.Parameters.AddWithValue("@Asignado", asignado);
                    cmd.Parameters.AddWithValue("@TipoTicket", tipoTick);
                    cmd.Parameters.AddWithValue("@Prioridad", prioridad);
                    cmd.Parameters.AddWithValue("@Cate1", cate1);
                    cmd.Parameters.AddWithValue("@Cate2", cate2);
                    cmd.Parameters.AddWithValue("@Cate3", cate3);
                    cmd.Parameters.AddWithValue("@Cate4", cate4);
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@Comentario", comentario);
                    if (fecIni.HasValue)
                        cmd.Parameters.AddWithValue("@FechaInicio", fecIni.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaInicio", DBNull.Value);
                    if (fecFin.HasValue)
                        cmd.Parameters.AddWithValue("@FechaFin", fecFin.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaFin", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Creador", creador);
                    if (fecProgra.HasValue)
                        cmd.Parameters.AddWithValue("@FechaProgramada", fecProgra.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaProgramada", DBNull.Value);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        private DataTable ReporteFind(int idCuenta, string codTick, string estados, int compania, int companiaFinal, int solicitante, int usuarioFinal, int areaRespon, int asignado, int tipoTick, int prioridad, int cate1, int cate2, int cate3, int cate4, int source, string comentario, DateTime? fecIni, DateTime? fecFin, int creador, DateTime? fecProgra)
        {
            // Implementa la lógica de conexión y ejecución del procedimiento almacenado aquí.
            // ...
            string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ExportarBusquedaTicket", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Configurar parámetros si es necesario
                    cmd.Parameters.AddWithValue("@IdCuenta", idCuenta);
                    cmd.Parameters.AddWithValue("@CodTicket", codTick);
                    cmd.Parameters.AddWithValue("@Estados", estados);
                    cmd.Parameters.AddWithValue("@Compania", compania);
                    cmd.Parameters.AddWithValue("@CompaniaFinal", companiaFinal);
                    cmd.Parameters.AddWithValue("@Solicitante", solicitante);
                    cmd.Parameters.AddWithValue("@UsuarioFinal", usuarioFinal);
                    cmd.Parameters.AddWithValue("@AreaResponsable", areaRespon);
                    cmd.Parameters.AddWithValue("@Asignado", asignado);
                    cmd.Parameters.AddWithValue("@TipoTicket", tipoTick);
                    cmd.Parameters.AddWithValue("@Prioridad", prioridad);
                    cmd.Parameters.AddWithValue("@Cate1", cate1);
                    cmd.Parameters.AddWithValue("@Cate2", cate2);
                    cmd.Parameters.AddWithValue("@Cate3", cate3);
                    cmd.Parameters.AddWithValue("@Cate4", cate4);
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@Comentario", comentario);
                    if (fecIni.HasValue)
                        cmd.Parameters.AddWithValue("@FechaInicio", fecIni.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaInicio", DBNull.Value);
                    if (fecFin.HasValue)
                        cmd.Parameters.AddWithValue("@FechaFin", fecFin.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaFin", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Creador", creador);
                    if (fecProgra.HasValue)
                        cmd.Parameters.AddWithValue("@FechaProgramada", fecProgra.Value);
                    else
                        cmd.Parameters.AddWithValue("@FechaProgramada", DBNull.Value);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public string ReturnRequ(int idend = 0)
        {
            try
            {
                var listClient = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == idend)
                                  join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                  select new
                                  {
                                      Client = ce.FIR_NAME + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME),
                                  });

                return listClient.First().Client;
            }
            catch
            {
                return "";
            }

        }

        //
        // GET: /Ticket/PersonCreateTicket
        public ActionResult PersonCreateTicket()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = (from t in db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO)

                         group t by new { t.UserId } into g
                         select new
                         {
                             g.Key.UserId,
                             Count = g.Count()
                         }).ToList();

            var result = (from x in query.Where(t => t.UserId != 29)
                          join ce in dbe.CLASS_ENTITY on x.UserId equals ce.UserId
                          //join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                          select new
                          {
                              //USUARIO = ce.FIR_NAME + " " + ce.LAS_NAME,
                              USUARIO = (ce.FIR_NAME + (ce.SEC_NAME == null ? "" : " " + ce.SEC_NAME) + (ce.LAS_NAME == null ? "" : " " + ce.LAS_NAME) + (ce.MOT_NAME == null ? "" : " " + ce.MOT_NAME)),
                              ce.UserId
                          }).OrderBy(x => x.USUARIO).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SaveWorkDate(DAY_WORK day_w)
        {
            try
            {
                var xx = day_w;
                string DAT_QUERs = Convert.ToString(Request.Params["DAT_QUER"]);
                int year = Convert.ToInt32(DAT_QUERs.Substring(0, 4));
                int month = Convert.ToInt32(DAT_QUERs.Substring(4, 2));
                int day = Convert.ToInt32(DAT_QUERs.Substring(6, 2));
                int ID_PERS_ENTI = Convert.ToInt32(day_w.ID_PERS_ENTI);
                DateTime DAT_QUER = new DateTime(year, month, day);

                var query = db.DAY_WORK.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.DATE_TIME == DAT_QUER);

                if (query.Count() == 0)
                {
                    //Insertar
                    if (!String.IsNullOrEmpty(day_w.DES_DAY_WORK))
                    {
                        day_w.DATE_TIME = DAT_QUER;
                        db.DAY_WORK.Add(day_w);
                        db.SaveChanges();
                    }
                }
                else
                {
                    //editamos
                    var edit_day_work = query.First();
                    edit_day_work.DES_DAY_WORK = day_w.DES_DAY_WORK;
                    db.Entry(edit_day_work).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }

        }
        //
        // GET: /Ticket/ResultWorkUnique
        public ActionResult ResultWorkUnique()
        {
            try
            {
                string ID_PERS_ENTIs = Convert.ToString(Request.Params["ID_PERS_ENTI"]);
                string DAT_QUERs = Convert.ToString(Request.Params["DAT_QUER"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                int ID_PERS_ENTI = 0;
                int year = Convert.ToInt32(DAT_QUERs.Substring(0, 4));
                int month = Convert.ToInt32(DAT_QUERs.Substring(4, 2));
                int day = Convert.ToInt32(DAT_QUERs.Substring(6, 2));

                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                DateTime DAT_QUER = new DateTime(year, month, day);

                if (Int32.TryParse(ID_PERS_ENTIs, out ID_PERS_ENTI))
                {
                    //if(DateTime.TryParse(DAT_QUERs,out DAT_QUER))
                    //{
                    try
                    {
                        int UserId = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                            .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new { ce.UserId })
                            .First().UserId.Value;

                        var query = db.DAY_WORK.Where(x => x.DATE_TIME == DAT_QUER)
                                    .Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI);
                        //.Concat();

                        var result = db.WORK_DATE_QUEU_UNIQ(ID_PERS_ENTI, DAT_QUER, ID_ACCO, SubCuenta).ToList();

                        if (query.Count() == 1)
                        {
                            return Json(new { text = query.First().DES_DAY_WORK, tickets = result }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { text = String.Empty, tickets = result }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch
                    {
                        return Content("ERROR");
                    }

                    //}
                }

                return Content("ERROR");
            }
            catch
            {
                return Content("ERROR");
            }

        }
        //
        // GET: /Ticket/ResultWork
        public ActionResult ResultWork()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int ID_QUEU = 0;
                DateTime DAT_WORK = DateTime.Now;

                string ID_QUEUs = Convert.ToString(Request.QueryString["ID_QUEU"]);
                string DAT_WORKs = Convert.ToString(Request.QueryString["DAT_WORK"]);

                if (!String.IsNullOrEmpty(ID_QUEUs))
                {
                    ID_QUEU = Convert.ToInt32(ID_QUEUs);

                }
                else
                {
                    return Json(new { status = 0, msg = "Please Select Queue" }, JsonRequestBehavior.AllowGet);
                }

                if (!String.IsNullOrEmpty(DAT_WORKs))
                {
                    DAT_WORK = Convert.ToDateTime(DAT_WORKs);
                }
                else
                {
                    return Json(new { status = 0, msg = "Please Select Date" }, JsonRequestBehavior.AllowGet);
                }
                int SubCuenta = 0;
                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }

                var query = db.WORK_DATE_QUEU(DAT_WORK, ID_ACCO, ID_QUEU, SubCuenta).ToList();

                return Json(new { Data = query, Count = query.Count(), Date = DAT_WORKs }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Ticket/Work
        [Authorize]
        public ActionResult Work()
        {
            return View();
        }

        [Authorize]
        public ActionResult CalendarioTicketsProgramados()
        {
            return View();
        }
        //
        // GET: /Ticket/TicketParent
        public ActionResult TicketParent()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var ID_STAT = new int[] { 1, 3, 5 };

            var query = db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.IS_PARENT == true && ID_STAT.Contains((int)t.ID_STAT_END));

            var result = (from t in query.OrderBy(t => t.COD_TICK).ToList()
                          select new
                          {
                              t.ID_TICK,
                              t.COD_TICK
                          }).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTickets()
        {
            string txt = "";
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var ID_STAT = new int[] { 1, 3, 5 }; //&& ID_STAT.Contains((int)t.ID_STAT_END)

            txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            if (txt == null) { txt = ""; }

            var query = db.tktListarTickets(ID_ACCO, txt).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }




        //
        // GET: /Ticket/ListByStatus
        public ActionResult ListByStatus(int id = 0, int ID_STAT = 0)
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            Boolean vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int VIS_ALL_QUEU = 0;
            if (vis_all_queu == true)
                VIS_ALL_QUEU = 1;

            var tick = db.ListarTicketPorEstado(id, "", ID_ACCO, UserId, ID_STAT, ID_PERS_ENTI, VIS_ALL_QUEU);
            var result = (from i in tick.ToList()
                          select new
                          {
                              i.ID_INCI,
                              i.COD_INCI,
                              SUM_INCI_PLAIN = StripHtml(i.SUM_INCI),
                              i.NAM_STAT,
                              i.CLA_STAT,
                              i.NAM_SOUR,
                              i.NAM_SUBCLAS,
                              i.NAM_CLAS,
                              i.NAM_SUBC,
                              i.NAM_CATE,
                              i.NAM_TYPE_TICK,
                              i.URL,
                              i.REQU,
                              CREATE_INCI = String.Format("{0:d}", i.CREATE_TICK) + " " + String.Format("{0:HH:mm}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:d}", i.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", i.MODIFIED_TICK),
                              i.ID_PRIO,
                              i.NAM_PRIO,
                              i.HOU_PRIO,
                              i.ICO_PRIO,
                              i.CREATEBY,
                              i.ACCO,
                              i.ASSI,
                              i.ID_FOTO_ASSI,
                              EXP_TIME = tir.ExpirationTime(i.ID_INCI, ObtenerHoraSLA(i.ID_INCI)),
                              i.PARENT,
                              i.COUNTSON,
                              CIA = i.COMPANY.Substring(0, (i.COMPANY.Length > 48 ? 48 : i.COMPANY.Length)) +
                                    (i.COMPANY.Length > 48 ? "..." : ""),
                              CIA_TOOL = i.COMPANY,
                              ID_ACCO,
                              VIS_COMP = i.VIS_COMP,
                              i.ID_STAT_END,
                              i.NUM_OP,
                              i.COD_TYPE_DOCU_SALE,
                              i.ID_DOCU_SALE,
                              Seque = contador(),
                              DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_INCI), Convert.ToDateTime(i.FEC_INI_TICK), ObtenerHoraSLA(i.ID_INCI)),
                              DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_INCI), i.FEC_INI_TICK) : "",
                              HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_INCI), i.FEC_INI_TICK) : "",
                              i.Titulo
                          });
            var qr = result.ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListByStatus_SubCuenta(int id = 0, string subCuenta = "")
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
                                pe.ID_FOTO,
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
            var tick = db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_DOCU_SALE == null);

            if (subCuenta != "")
            {
                tick = tick.Where(x => x.SubCuenta == subCuenta);
            }

            // var listTicket = tick.ToList();
            //Diferenciando activos - soporte
            if (id == 0)
            {
                tick = tick.Where(x => x.ID_TYPE_FORM == null);
            }
            if (id == 1)
            {
                tick = tick.Where(x => x.ID_TYPE_FORM != null);
            }

            int Count = 0, iq = 0;
            int[] numbers = new int[0];

            if (Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()) == false) /*Los de service desk tienen el flag en true, por eso pasan sin restricciones*/
            {
                int supQueu = 0;
                try
                {

                    var Queus = db.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
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
                    tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);  /*Se toma cuando no es supervisor o usuario de Service Desk*/
                }
                else
                {
                    //Mostrar el ticket cuya cola es la indicada
                    tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value)); /*Usuario de Service D */
                }
            }

            if (ID_STAT == 0)
            {
                tick = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.DAT_EXPI_TICK).ThenBy(t => t.ID_PRIO).ThenByDescending(t => t.CREATE_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 5)
            {
                tick = tick.Where(i => i.ID_STAT_END == 5);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.MODIFIED_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 4)
            {
                tick = tick.Where(i => i.ID_STAT_END == 4);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.MODIFIED_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 6)
            {
                tick = tick.Where(i => i.ID_STAT_END == 6);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }

            var result = (from i in tick.ToList()
                          join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in db.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in db.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE
                          join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          //join cr in listuser on i.UserId equals cr.UserId
                          join cr in listuser on (i.UserId == null ? 0 : i.UserId) equals cr.UserId into xx
                          from xp in xx.DefaultIfEmpty()
                          join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listuser on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                          join ds in db.DOCUMENT_SALE on (i.ID_DOCU_SALE == null ? 0 : i.ID_DOCU_SALE) equals ds.ID_DOCU_SALE into lds
                          from xds in lds.DefaultIfEmpty()
                          join tds in db.TYPE_DOCUMENT_SALE on (xds == null ? 0 : xds.ID_TYPE_DOCU_SALE) equals tds.ID_TYPE_DOCU_SALE into ltds
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
                              //HOU_PRIO = pr.HOU_PRIO != 0 ? Convert.ToString(pr.HOU_PRIO) + "h" : "",
                              HOU_PRIO = ObtenerHoraSLA(i.ID_TICK) != 0 ? ObtenerHoraSLA(i.ID_TICK) + "h" : "",
                              ICO_PRIO = pr.ICO_PRIO,
                              //CREATEBY = cr.Client.ToLower(),
                              CREATEBY = (i.UserId == null ? ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI)).ToLower() : xp.Client.ToLower()),
                              ACCO = ac.NAM_ACCO,
                              ASSI = asi.Client.ToLower(),
                              ID_FOTO_ASSI = asi.ID_FOTO,
                              //EXP_TIME = tir.ExpirationTime(i.ID_TICK, Convert.ToInt32(pr.HOU_PRIO)),//ExpTime(i.ID_TICK),
                              EXP_TIME = tir.ExpirationTime(i.ID_TICK, ObtenerHoraSLA(i.ID_TICK)),
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
                              //DATE_INI = String.Format("{0:MMM d yyyy HH:mm:ss}", Convert.ToDateTime(i.CREATE_TICK).AddHours(Convert.ToInt32(pr.HOU_PRIO))),
                              Seque = contador(),
                              //DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_TICK), Convert.ToDateTime(i.FEC_INI_TICK), Convert.ToInt32(pr.HOU_PRIO)),  
                              DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_TICK), Convert.ToDateTime(i.FEC_INI_TICK), ObtenerHoraSLA(i.ID_TICK)),
                              DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : "",
                              HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : "",
                              Titulo = (i.TITLE_TICK == null ? "-" : i.TITLE_TICK)
                          });
            var qr = result.ToList();
            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ListByStatus_SubCuenta(int id = 0, string subCuenta = "")
        //{
        //    //textInfo = cultureInfo.TextInfo;
        //    //ctd = 1;
        //  int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
        //    int UserId = Convert.ToInt32(Session["UserId"]);
        //    //int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
        //    int ID_STAT = 1;
        //    int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //    Boolean vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
        //    int VIS_ALL_QUEU = 0;
        //    if (vis_all_queu == true)
        //        VIS_ALL_QUEU = 1;

        //    var tick = db.ListarTicketPorEstado2(id, subCuenta, ID_ACCO, UserId, ID_STAT,ID_PERS_ENTI, VIS_ALL_QUEU);
        //    var result = (from i in tick.ToList()
        //                  select new
        //                  {
        //                      i.ID_INCI,
        //                      i.COD_INCI,
        //                      i.SUM_INCI,
        //                      SUM_INCI_PLAIN = StripHtml(i.SUM_INCI),
        //                      i.NAM_STAT,
        //                      i.CLA_STAT,
        //                      i.NAM_SOUR,
        //                      i.NAM_SUBCLAS,
        //                      i.NAM_CLAS,
        //                      i.NAM_SUBC,
        //                      i.NAM_CATE,
        //                      i.NAM_TYPE_TICK,
        //                      i.URL,
        //                      i.REQU,
        //                      CREATE_INCI = String.Format("{0:d}", i.CREATE_TICK) + " " + String.Format("{0:HH:mm}", i.CREATE_TICK),
        //                      MODIFIED_INCI = String.Format("{0:d}", i.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", i.MODIFIED_TICK),
        //                      i.ID_PRIO,
        //                      i.NAM_PRIO,
        //                      i.HOU_PRIO,
        //                      i.ICO_PRIO,
        //                      i.CREATEBY,
        //                      i.ACCO,
        //                      i.ASSI,
        //                      i.ID_FOTO_ASSI,
        //                      EXP_TIME = tir.ExpirationTime(i.ID_INCI, ObtenerHoraSLA(i.ID_INCI)),
        //                      i.PARENT,
        //                      i.COUNTSON,
        //                      CIA = i.COMPANY.Substring(0, (i.COMPANY.Length > 48 ? 48 : i.COMPANY.Length)) +
        //                            (i.COMPANY.Length > 48 ? "..." : ""),
        //                      CIA_TOOL = i.COMPANY,
        //                      ID_ACCO,
        //                      VIS_COMP = i.VIS_COMP,
        //                      i.ID_STAT_END,
        //                      i.NUM_OP,
        //                      i.COD_TYPE_DOCU_SALE,
        //                      i.ID_DOCU_SALE,
        //                      Seque = contador(),
        //                      DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_INCI), Convert.ToDateTime(i.FEC_INI_TICK), ObtenerHoraSLA(i.ID_INCI)),
        //                      DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_INCI), i.FEC_INI_TICK) : "",
        //                      HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_INCI), i.FEC_INI_TICK) : "",
        //                      i.Titulo
        //                  });
        //    var r = result.ToList();

        //    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        //    //var listuser = (from lu in dbe.ACCOUNT_ENTITY
        //    //                join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
        //    //                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
        //    //                where lu.ID_ACCO == ID_ACCO
        //    //                select new
        //    //                {
        //    //                    lu.ID_PERS_ENTI,
        //    //                    Client = ce.FIR_NAME + " " + ce.LAS_NAME,
        //    //                    ce.UserId,
        //    //                    pe.ID_FOTO,
        //    //                }).ToList();

        //    //var listCIA = (from pe in dbe.PERSON_ENTITY
        //    //               join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
        //    //               select new
        //    //               {
        //    //                   pe.ID_PERS_ENTI,
        //    //                   COMPANY = ce.COM_NAME,
        //    //               }).ToList();



        //    //int skip = Convert.ToInt32(Request.Params["skip"].ToString());
        //    //int take = Convert.ToInt32(Request.Params["take"].ToString());
        //    //var tick = db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_DOCU_SALE == null);

        //    //if (subCuenta != "") //FINAL
        //    //{
        //    //    tick = tick.Where(x => x.SubCuenta == subCuenta);
        //    //}

        //    //// var listTicket = tick.ToList();
        //    ////Diferenciando activos - soporte
        //    //if (id == 0)
        //    //{
        //    //    tick = tick.Where(x => x.ID_TYPE_FORM == null);
        //    //}
        //    //if (id == 1)
        //    //{
        //    //    tick = tick.Where(x => x.ID_TYPE_FORM != null);
        //    //}

        //    //int Count = 0, iq = 0;
        //    //int[] numbers = new int[0];

        //    //if (Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()) == false) /*Los de service desk tienen el flag en true, por eso pasan sin restricciones*/
        //    //{
        //    //    int supQueu = 0;
        //    //    try
        //    //    {
        //    //        var Queus = db.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
        //    //            .Where(x => x.ID_PERS_ENTI_CORD == ID_PERS_ENTI);

        //    //        supQueu = Queus.Count();

        //    //        numbers = new int[supQueu];

        //    //        foreach (var x in Queus)
        //    //        {
        //    //            //var orderKeys = new int[] { 1, 12, 306, 284, 50047 };
        //    //            numbers[iq] = (int)x.ID_QUEU.Value;
        //    //            iq++;
        //    //            //Customers.Rows.Add(row);
        //    //        }
        //    //    }
        //    //    catch
        //    //    {

        //    //    }

        //    //    if (supQueu == 0)
        //    //    {
        //    //        tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);  /*Se toma cuando no es supervisor o usuario de Service Desk*/
        //    //    }
        //    //    else
        //    //    {
        //    //        //Mostrar el ticket cuya cola es la indicada
        //    //        tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value)); /*Usuario de Service D */
        //    //    }
        //    //}
        //    //if (ID_STAT == 0)
        //    //{
        //    //    tick = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3);
        //    //    Count = tick.Count();
        //    //    tick = tick.OrderByDescending(t => t.DAT_EXPI_TICK).ThenBy(t => t.ID_PRIO).ThenByDescending(t => t.CREATE_TICK).Skip(skip).Take(take);
        //    //}
        //    //else if (ID_STAT == 5)
        //    //{
        //    //    tick = tick.Where(i => i.ID_STAT_END == 5);
        //    //    Count = tick.Count();
        //    //    tick = tick.OrderByDescending(t => t.MODIFIED_TICK).Skip(skip).Take(take);
        //    //}
        //    //else if (ID_STAT == 4)
        //    //{
        //    //    tick = tick.Where(i => i.ID_STAT_END == 4);
        //    //    Count = tick.Count();
        //    //    tick = tick.OrderByDescending(t => t.MODIFIED_TICK).Skip(skip).Take(take);
        //    //}
        //    //else if (ID_STAT == 6)
        //    //{
        //    //    tick = tick.Where(i => i.ID_STAT_END == 6);
        //    //    Count = tick.Count();
        //    //    tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
        //    //}

        //    //var tickc2 = tick.ToList();
        //    //var result = (from i in tick.ToList()
        //    //              join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
        //    //              join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
        //    //              join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
        //    //              join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
        //    //              join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
        //    //              join c2 in db.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
        //    //              join c1 in db.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE
        //    //              join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
        //    //              //join cr in listuser on i.UserId equals cr.UserId
        //    //              join cr in listuser on (i.UserId == null ? 0 : i.UserId) equals cr.UserId into xx
        //    //              from xp in xx.DefaultIfEmpty()
        //    //              join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
        //    //              join asi in listuser on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
        //    //              join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI
        //    //              join ds in db.DOCUMENT_SALE on (i.ID_DOCU_SALE == null ? 0 : i.ID_DOCU_SALE) equals ds.ID_DOCU_SALE into lds
        //    //              from xds in lds.DefaultIfEmpty()
        //    //              join tds in db.TYPE_DOCUMENT_SALE on (xds == null ? 0 : xds.ID_TYPE_DOCU_SALE) equals tds.ID_TYPE_DOCU_SALE into ltds
        //    //              from xtds in ltds.DefaultIfEmpty()
        //    //              select new
        //    //              {
        //    //                  ID_INCI = i.ID_TICK,
        //    //                  COD_INCI = i.COD_TICK,
        //    //                  SUM_INCI = i.SUM_TICK,
        //    //                  SUM_INCI_PLAIN = StripHtml(i.SUM_TICK),
        //    //                  NAM_STAT = s.NAM_STAT.ToLower(),
        //    //                  CLA_STAT = s.CLA_STAT,
        //    //                  NAM_SOUR = so.NAM_SOUR.ToLower(),
        //    //                  NAM_SUBCLAS = c4.NAM_CATE.ToLower(),
        //    //                  NAM_CLAS = c3.NAM_CATE.ToLower(),
        //    //                  NAM_SUBC = c2.NAM_CATE.ToLower(),
        //    //                  NAM_CATE = c1.ABR_CATE,
        //    //                  NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
        //    //                  URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
        //    //                  REQU = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
        //    //                  CREATE_INCI = String.Format("{0:d}", i.CREATE_TICK) + " " + String.Format("{0:HH:mm}", i.CREATE_TICK),
        //    //                  MODIFIED_INCI = String.Format("{0:d}", i.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", i.MODIFIED_TICK),
        //    //                  ID_PRIO = pr.ID_PRIO,
        //    //                  NAM_PRIO = pr.NAM_PRIO.ToLower(),
        //    //                  //HOU_PRIO = pr.HOU_PRIO != 0 ? Convert.ToString(pr.HOU_PRIO) + "h" : "",
        //    //                  HOU_PRIO = ObtenerHoraSLA(i.ID_TICK) != 0 ? ObtenerHoraSLA(i.ID_TICK) + "h" : "",
        //    //                  ICO_PRIO = pr.ICO_PRIO,
        //    //                  //CREATEBY = cr.Client.ToLower(),
        //    //                  CREATEBY = (i.UserId == null ? ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI)).ToLower() : xp.Client.ToLower()),
        //    //                  ACCO = ac.NAM_ACCO,
        //    //                  ASSI = asi.Client.ToLower(),
        //    //                  ID_FOTO_ASSI = asi.ID_FOTO,
        //    //                  //EXP_TIME = tir.ExpirationTime(i.ID_TICK, Convert.ToInt32(pr.HOU_PRIO)),//ExpTime(i.ID_TICK),
        //    //                  EXP_TIME = tir.ExpirationTime(i.ID_TICK, ObtenerHoraSLA(i.ID_TICK)),
        //    //                  PARENT = (i.IS_PARENT == true ? "expand" : "none"),
        //    //                  COUNTSON = CountTicketSon(i.ID_TICK),
        //    //                  CIA = textInfo.ToTitleCase(cia.COMPANY.ToLower()).Substring(0, (cia.COMPANY.Length > 48 ? 48 : cia.COMPANY.Length)) +
        //    //                        (cia.COMPANY.Length > 48 ? "..." : ""),
        //    //                  CIA_TOOL = textInfo.ToTitleCase(cia.COMPANY.ToLower()),
        //    //                  ID_ACCO,
        //    //                  VIS_COMP = ac.VIS_COMP,
        //    //                  i.ID_STAT_END,
        //    //                  NUM_OP = (xds == null ? "" : xds.NUM_DOCU_SALE),
        //    //                  COD_TYPE_DOCU_SALE = (xtds == null ? "" : xtds.COD_TYPE_DOCU_SALE),
        //    //                  ID_DOCU_SALE = (xds == null ? 0 : xds.ID_DOCU_SALE),
        //    //                  //DATE_INI = String.Format("{0:MMM d yyyy HH:mm:ss}", Convert.ToDateTime(i.CREATE_TICK).AddHours(Convert.ToInt32(pr.HOU_PRIO))),
        //    //                  Seque = contador(),
        //    //                  //DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_TICK), Convert.ToDateTime(i.FEC_INI_TICK), Convert.ToInt32(pr.HOU_PRIO)),  
        //    //                  DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_TICK), Convert.ToDateTime(i.FEC_INI_TICK), ObtenerHoraSLA(i.ID_TICK)),
        //    //                  DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : "",
        //    //                  HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : "",
        //    //                  Titulo = (i.TITLE_TICK == null ? "-" : i.TITLE_TICK)
        //    //              });
        //    //var qr = result.ToList();
        //    //return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ListarTicketPorEstado(int id = 0, string subCuenta = "", int ID_STAT = 0)
        {
            //textInfo = cultureInfo.TextInfo;
            //ctd = 1;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            Boolean vis_all_queu = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            int VIS_ALL_QUEU = 0;
            if (vis_all_queu == true)
                VIS_ALL_QUEU = 1;

            var tick = db.ListarTicketPorEstado(id, subCuenta, ID_ACCO, UserId, ID_STAT, ID_PERS_ENTI, VIS_ALL_QUEU);
            var result = (from i in tick.ToList()
                          select new
                          {
                              i.ID_INCI,
                              i.COD_INCI,
                              SUM_INCI_PLAIN = StripHtml(i.SUM_INCI),
                              i.NAM_STAT,
                              i.CLA_STAT,
                              i.NAM_SOUR,
                              i.NAM_SUBCLAS,
                              i.NAM_CLAS,
                              i.NAM_SUBC,
                              i.NAM_CATE,
                              i.NAM_TYPE_TICK,
                              i.URL,
                              i.REQU,
                              CREATE_INCI = String.Format("{0:d}", i.CREATE_TICK) + " " + String.Format("{0:HH:mm}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:d}", i.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", i.MODIFIED_TICK),
                              i.ID_PRIO,
                              i.NAM_PRIO,
                              i.HOU_PRIO,
                              i.ICO_PRIO,
                              i.CREATEBY,
                              i.ACCO,
                              i.ASSI,
                              i.ID_FOTO_ASSI,
                              EXP_TIME = tir.ExpirationTime(i.ID_INCI, ObtenerHoraSLA(i.ID_INCI)),
                              i.PARENT,
                              i.COUNTSON,
                              CIA = i.COMPANY.Substring(0, (i.COMPANY.Length > 48 ? 48 : i.COMPANY.Length)) +
                                    (i.COMPANY.Length > 48 ? "..." : ""),
                              CIA_TOOL = i.COMPANY,
                              ID_ACCO,
                              VIS_COMP = i.VIS_COMP,
                              i.ID_STAT_END,
                              i.NUM_OP,
                              i.COD_TYPE_DOCU_SALE,
                              i.ID_DOCU_SALE,
                              Seque = contador(),
                              DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_INCI), Convert.ToDateTime(i.FEC_INI_TICK), ObtenerHoraSLA(i.ID_INCI)),
                              DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_INCI), i.FEC_INI_TICK) : "",
                              HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_INCI), i.FEC_INI_TICK) : "",
                              i.Titulo
                          });
            var r = result.ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
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
                var query = db.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
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
                //.OrderByDescending(x => x.ID_DETA_TICK)
                //.Where(x => x.ID_STAT == 5).First().FEC_SCHE;
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
                var query = db.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
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
            return db.TICKETs.Where(x => x.ID_TICK_PARENT == id).Count();
        }

        public int ObtenerHoraSLA(int idTicket = 0)
        {
            return db.ObtenerHoraSLA(idTicket).Single().TiempoAtencion;
        }

        //funcion para sacar expiration time
        public string ExpTime(int id)
        {
            try
            {

                var tick = db.TICKETs.Single(t => t.ID_TICK == id);
                var prio = db.PRIORITies.Single(p => p.ID_PRIO == tick.ID_PRIO);

                if (tick.ID_STAT_END != 4 && tick.ID_STAT_END != 6 && tick.ID_STAT_END != 2 && tick.ID_STAT_END != 5)
                {
                    var time = DateTime.Now.Subtract(Convert.ToDateTime(tick.CREATE_TICK)).Days * 24 +
                                                DateTime.Now.Subtract(Convert.ToDateTime(tick.CREATE_TICK)).Hours;

                    //return Convert.ToString(Convert.ToInt32(prio.HOU_PRIO) - Convert.ToInt32(time));
                    return Convert.ToString(Convert.ToInt32(ObtenerHoraSLA(tick.ID_TICK)) - Convert.ToInt32(time));
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

            var qDet = db.DETAILS_TICKET.Where(x => x.ID_TICK == IdTick //&& x.ID_STAT == 5
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

        //
        // GET: /Ticket/

        public ActionResult Index()
        {
            //var tickets = db.TICKETs.Include(t => t.CLIENT).Include(t => t.CLIENT1).Include(t => t.CLIENT2).Include(t => t.QUEUE).Include(t => t.SOURCE).Include(t => t.STATUS).Include(t => t.STATUS1).Include(t => t.TYPE_TICKET);
            return View();
        }

        //
        // GET: /Ticket/Details/5

        public ActionResult Details(int id = 0)
        {
            TICKET ticket = db.TICKETs.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //
        // GET: /Ticket/Find
        [Authorize]
        public ActionResult FindTicket()
        {
            //int idIdioma = Convert.ToInt32(Session["IdIdioma"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //string llave = "Asignadoa|Creadopor|Creado|UltimaAct";

            //string[] llaves = llave.Split('|');

            //var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
            //            select new
            //            {
            //                llave = id.Llave,
            //                texto = id.Texto
            //            };

            //var xxx = query.ToList();


            ViewBag.Asignadoa = "Asignado a";
            ViewBag.Creado = "Creado";
            ViewBag.UltimaAct = "Ultima Act";
            ViewBag.Creadopor = "Creado por";
            ViewBag.CantidadPaginas = 0;
            ViewBag.CantidadPintar = 0;
            ViewBag.ID_ACCO = ID_ACCO;
            if (ID_ACCO == (int)Minsur.MINSUR || ID_ACCO == (int)Minsur.MARCOBRE || ID_ACCO == (int)Minsur.RAURA)
            {
                return View("FindTicketMinsur");
            }
            else if (ID_ACCO == 60)
            {
                return View("FindTicketBnv");
            }
            else if (ID_ACCO == 1 || ID_ACCO == 25)
            {
                return View("BuscarTicket");
            }
            else if (ID_ACCO == 4)
            {
                return View("BuscarTicketEdata");
            }
            else
            {
                ViewBag.Account = ID_ACCO;
                return View();
            }

        }





        //
        // GET: /Ticket/Create
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var cInc = new TICKET();
                cInc.FEC_TICK = DateTime.Now;
                ViewBag.DATE = String.Format("{0:g}", cInc.FEC_TICK);
                ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
                ViewBag.ACCESO_NEWREQ = 0;
                ViewBag.ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                ViewBag.ID_PERS_ENTI_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                ViewBag.IdAcco = ID_ACCO;
                var horaServer = DateTime.Now;
                var horaServerString = horaServer.ToString("s");
                ViewBag.DATETIMESERVER = horaServerString;
                /*string UserName = Convert.ToString(Session["UserName"]);
                string[] rolesArray = Roles.GetRolesForUser(UserName);

                foreach (string xc in rolesArray)
                {
                    int i = Array.IndexOf(rolesArray, xc);
                    if (xc == "SERVICE DESK" || xc == "SYSTEMADMINISTRATOR" || xc == "ADMINISTRADOR") 
                    {
                        ViewBag.ACCESO_NEWREQ = 1;
                    }
                }*/
                //--------------Corregido
                if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1 || Convert.ToInt32((Session["SERVICEDESK"].ToString())) == 1 || Convert.ToInt32((Session["ADMINISTRADOR_SISTEMA"].ToString())) == 1)
                {
                    ViewBag.ACCESO_NEWREQ = 1;
                }
                bool cia = db.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).VIS_COMP.Value;
                if (cia)
                {
                    ViewBag.IdSLA = db.SLAs.FirstOrDefault(x => (x.IdCuenta == ID_ACCO) && x.Nombre == "SLA ESTANDAR").Id;
                    if (ID_ACCO == 4)
                    {
                        return View("CreateEdata");
                    }
                    else
                    {
                        return View("CreateEX1");
                    }

                }

                else
                {
                    if (ID_ACCO == 60)
                    {
                        return View("CreateBVN");
                    }
                    else
                    if (ID_ACCO == (int)Minsur.MARCOBRE ||
                        ID_ACCO == (int)Minsur.RAURA ||
                        ID_ACCO == (int)Minsur.MINSUR)
                    {
                        //ViewBag.IdSLA = db.SLAs.Single(x => x.IdCuenta == ID_ACCO).Id;
                        ViewBag.IdSLA = db.SLAs.Single(x => x.IdCuenta == ID_ACCO && x.Nombre == "SLA ESTANDAR").Id;
                        return View("CreateMinsur");
                    }
                    else
                    {
                        ViewBag.IdSLA = db.SLAs.Single(x => x.IdCuenta == ID_ACCO).Id;
                        return View();
                    }

                }

            }
            catch
            {
                return Content("Please Close Session");
            }
        }

        [Authorize]
        public ActionResult CreateEX1_K22()
        {
            return View();
        }

        [Authorize]
        public ActionResult CrearTicket()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var cInc = new TICKET();
                cInc.FEC_TICK = DateTime.Now;
                ViewBag.DATE = String.Format("{0:g}", cInc.FEC_TICK);
                ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
                ViewBag.ACCESO_NEWREQ = 0;
                ViewBag.ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                ViewBag.ID_PERS_ENTI_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                ViewBag.IdAcco = ID_ACCO;
                var horaServer = DateTime.Now;
                var horaServerString = horaServer.ToString("s");
                ViewBag.DATETIMESERVER = horaServerString;
                /*string UserName = Convert.ToString(Session["UserName"]);
                string[] rolesArray = Roles.GetRolesForUser(UserName);

                foreach (string xc in rolesArray)
                {
                    int i = Array.IndexOf(rolesArray, xc);
                    if (xc == "SERVICE DESK" || xc == "SYSTEMADMINISTRATOR" || xc == "ADMINISTRADOR") 
                    {
                        ViewBag.ACCESO_NEWREQ = 1;
                    }
                }*/
                //--------------Corregido
                if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1 || Convert.ToInt32((Session["SERVICEDESK"].ToString())) == 1 || Convert.ToInt32((Session["ADMINISTRADOR_SISTEMA"].ToString())) == 1)
                {
                    ViewBag.ACCESO_NEWREQ = 1;
                }
                bool cia = db.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).VIS_COMP.Value;
                if (cia)
                {
                    ViewBag.IdSLA = db.SLAs.FirstOrDefault(x => (x.IdCuenta == ID_ACCO) && x.Nombre == "SLA ESTANDAR").Id;
                    return View("CreateEX1");
                }

                else
                {
                    if (ID_ACCO == 60)
                    {
                        return View("CreateBVN");
                    }
                    else
                    if (ID_ACCO == (int)Minsur.MARCOBRE ||
                        ID_ACCO == (int)Minsur.RAURA ||
                        ID_ACCO == (int)Minsur.MINSUR)
                    {
                        ViewBag.IdSLA = db.SLAs.Single(x => x.IdCuenta == ID_ACCO).Id;
                        return View("CreateMinsur");
                    }
                    else
                    {
                        ViewBag.IdSLA = db.SLAs.Single(x => x.IdCuenta == ID_ACCO).Id;
                        return View();
                    }

                }

            }
            catch
            {
                return Content("Please Close Session");
            }
        }

        //
        // POST: /Ticket/Create

        [HttpPost, ValidateInput(false)]

        public ActionResult Create(IEnumerable<HttpPostedFileBase> file, TICKET ticket)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            if (UserId == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.perdioSesion();}window.onload=init;</script>");
            }
            try
            {
                if ((int)Session["ADMINISTRADOR"] == 0 && Convert.ToInt32(Session["ID_ACCO"]) == 3)
                {
                    int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    var cola = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti).FirstOrDefault();
                    if (cola.ID_QUEU != 1 && cola.ID_QUEU != 5)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.MensajeError) top.MensajeError('Usted no tiene permisos para crear un ticket. Por favor contacte al personal de mesa de servicio.','0');}window.onload=init;</script>");
                    }
                }
            }
            catch { }
            //SubCuenta
            string subCuenta = Request.Form["hdnSubCuenta"].ToString();
            //TicketAutomatico
            int taTicketAutomatico = Convert.ToInt32(Request.Form["hdnTkAutomatico"].ToString());
            string taTitulo = Request.Form["hdnTitulo"].ToString();
            int taFrecuencia = Convert.ToInt32(Request.Form["hdnFrecuencia"].ToString());
            string taDiaSemana = Request.Form["hdnDiaSemana"].ToString();
            string taDiaMes = Request.Form["hdnDiaMes"].ToString();
            string taDiaPersonalizado = Request.Form["hdnDiaPersonalizado"].ToString();
            int taVigencia = Convert.ToInt32(Request.Form["hdnVigencia"].ToString());
            string taVigenciaInicio = Request.Form["hdnVigenciaInicio"].ToString();
            string taVigenciaFin = Request.Form["hdnVigenciaFin"].ToString();
            string taHoraCreacion = Request.Form["hdnHoraCreacion"].ToString();


            /*GESTION DEL CONOCIMIENTO GCR161203*/
            int ID_TEMA = 0; ID_TEMA = Convert.ToInt32(Request.Form["IdTema"]);
            /*Fin*/
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string FEC_INI_TICK = Convert.ToString(Request.Form["FEC_INI_TICK"]);
            string KEY_ATTA = null;
            int ID_PERS_ENTI = 0;
            int hours = 0;

            //Solo aplica a BNV, en otros casos es 0
            int UnidadMineraBnv = 0;
            if (ID_ACCO == 60)
            {
                try
                {
                    UnidadMineraBnv = Convert.ToInt32(Request.Form["hdnUnidadMinera"].ToString());
                }
                catch (Exception ex)
                {

                }

            }

            if (ID_ACCO == 60)
            { 

                if (ticket.TITLE_TICK == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe llenar el Título de la Solicitud.<br/>');}window.onload=init;</script>");
                }

                if (Convert.ToInt32(ticket.ID_TYPE_TICK) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Tipo de Ticket.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_PRIO) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar la Prioridad<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_SOUR) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Medio de Comunicación. <br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_COMP) == 0 && UnidadMineraBnv == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar la Compañia. <br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_PERS_ENTI) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Solicitante<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar al Usuario Afectado<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_PERS_ENTI_ASSI) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar al Asignado.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_STAT) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Estado del Ticket.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_CATE) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar las Categorías.<br/>');}window.onload=init;</script>");

                }


                // Corrección en la validación del SLA -- revisado LS

                if (Convert.ToInt32(ticket.IdSLA) == 0 )
                {
                    System.Diagnostics.Debug.WriteLine("Valor de SLA recibido: " + ticket.IdSLA);
                    return Content("<script type='text/javascript'> function init() {" +
                                   "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar un SLA antes de continuar.');}window.onload=init;</script>");
                }
                if (Convert.ToInt32(ticket.IdGrupoReportador) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el grupo Repordato.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.IdModTrabajo) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar La modalidad de Trabajo.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_LOCA) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar la Locacion.<br/>');}window.onload=init;</script>");

                }

            }

            try
            {
                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);

            }
            catch
            {

            }
            try
            {
                int id_prio = Convert.ToInt32(ticket.ID_PRIO);
                //hours = Convert.ToInt32(db.PRIORITies.Single(x => x.ID_PRIO == id_prio).HOU_PRIO);
                hours = Convert.ToInt32(db.SLADetalles.Single(x => x.IdPrioridad == id_prio && x.IdSLA == ticket.IdSLA).TiempoAtencion);
                ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);

                try
                {
                    int ID_AREA = Convert.ToInt32(dbe.PERSON_ENTITY.Single(r => r.ID_PERS_ENTI == ID_PERS_ENTI).ID_AREA);
                    ticket.ID_AREA = ID_AREA;
                }
                catch
                {
                    ticket.ID_AREA = null;
                }

                if (ID_ACCO == 60)
                {
                    ticket.ID_COMP = UnidadMineraBnv;
                }
            }
            catch
            {

            }

            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            ticket.FEC_TICK = DateTime.Now;

            if (ModelState.IsValid)
            {
                // Motivo de espera
                if (!new[] { 56, 57, 58, 60 }.Contains(Convert.ToInt32(Session["ID_ACCO"])) && ticket.ID_STAT == 5 && ticket.IdMotivoEstado == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (ID_ACCO == 4)
                {
                    if (ticket.ID_QUEU == 26 && Convert.ToInt32(ticket.ID_COMP_END) == 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                    }

                    //if (ticket.ID_COMP == 4034 || ticket.ID_COMP == 44862) //Cliente BCP Y INDECOPI
                    //{
                    if (ticket.ID_SOUR == 3)
                    {
                        if (ticket.FechaRecepcionSolicitud == null)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERRORBCP','0');}window.onload=init;</script>");
                        }
                    }

                }

                if (ID_ACCO == 17)
                {
                    if (ticket.FechaRecepcionSolicitud == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERRORBCP','0');}window.onload=init;</script>");
                    }
                }

                if (UserId == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (String.IsNullOrEmpty(FEC_INI_TICK) && ticket.ID_STAT == 5)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (ticket.ID_STAT == 5 && ticket.FEC_TICK > ticket.FEC_INI_TICK)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (ticket.ID_STAT != 5)
                {
                    ticket.FEC_INI_TICK = ticket.FEC_TICK;
                }

                DateTime FechaIni;

                try
                {
                    if (ticket.ID_COMP == null)
                    {
                        ticket.ID_COMP = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI1.Value;
                    }
                }
                catch
                {

                }

                try
                {
                    DateTime.TryParse(Convert.ToString(ticket.FEC_INI_TICK), out FechaIni);
                }
                catch
                {
                    DateTime.TryParse(Convert.ToString(ticket.FEC_TICK), out FechaIni);
                }

                ticket.DAT_EXPI_TICK = FechaIni.AddHours(hours);

                ticket.UserId = UserId;
                ticket.ID_ACCO = ID_ACCO;
                if (ID_ACCO == 4 || ID_ACCO == 17) /* RMA para Edata y Claro*/
                {
                    string RMA = Request.Form["RMA"] == null ? null : Convert.ToString(Request.Form["RMA"]);
                    ticket.RMA = RMA;
                }
                if (ID_ACCO == 60) /* Campo de RAZON para creación de Ticket - BNV*/
                {

                    int ID_LOCA = Request.Form["ID_LOCA"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_LOCA"].ToString());
                    int IdGrupoReportador = Request.Form["IdGrupoReportador"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdGrupoReportador"].ToString());
                    int ID_RAZON = Request.Form["IdRazon"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdRazon"].ToString());
                    int ID_TIPOPASE = Request.Form["IdTipoPase"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdTipoPase"].ToString());  //Convert.ToInt32(Request.Form["ID_TIPOPASE"].ToString());
                    int ID_MODTRABAJO = Request.Form["IdModTrabajo"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdModTrabajo"].ToString());  //Convert.ToInt32(Request.Form["ID_MODTRABAJO"].ToString());
                    string SOLMAN = Request.Form["Solman"] == null ? null : Convert.ToString(Request.Form["Solman"]);
                    int ID_PERS_ENTI_ANLT = Request.Form["ID_PERS_ENTI_ANLT"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_PERS_ENTI_ANLT"].ToString());
                    string Solucion = Request.Form["Solucion"] == null ? null : Convert.ToString(Request.Form["Solucion"]);

                    //string SOLMAN = Request.Form["SOLMAN"].ToString();

                    if (ID_LOCA == 0) { ticket.ID_LOCA = null; } else { ticket.ID_LOCA = ID_LOCA; }
                    if (IdGrupoReportador == 0) { ticket.IdGrupoReportador = null; } else { ticket.IdGrupoReportador = IdGrupoReportador; }
                    if (ID_RAZON == 0) { ticket.IdRazon = null; } else { ticket.IdRazon = ID_RAZON; }
                    if (ID_TIPOPASE == 0) { ticket.IdTipoPase = null; } else { ticket.IdTipoPase = ID_TIPOPASE; }
                    if (ID_MODTRABAJO == 0) { ticket.IdModTrabajo = null; } else { ticket.IdModTrabajo = ID_MODTRABAJO; }
                    if (SOLMAN == "") { ticket.Solman = null; } else { ticket.Solman = SOLMAN; }
                    if (ID_PERS_ENTI_ANLT == 0) { ticket.ID_PERS_ENTI_ANLT = null; } else { ticket.ID_PERS_ENTI_ANLT = ID_PERS_ENTI_ANLT; }
                    if (Solucion == "") { ticket.Solucion = null; } else { ticket.Solucion = Solucion; }


                }
                ticket.ID_STAT_END = Convert.ToInt32(Request.Form["ID_STAT"]);
                ticket.CREATE_TICK = DateTime.Now;
                ticket.MODIFIED_TICK = DateTime.Now;
                ticket.SEND_MAIL = false;
                ticket.SEND_SURVEY = null;

                if (ticket.ID_PERS_ENTI == 63132 && ticket.ID_ACCO == 4)
                {
                    ticket.SEND_SURVEY = false;
                }

                ticket.SubCuenta = subCuenta;
                ticket.IdTema = ID_TEMA;  /*GESTION DEL CONOCIMIENTO GCR161203*/
                if (ticket.ID_ACCO == 1 && ticket.ID_CATE == 15223) /* CATEGORIA CESE DE CUENTA / GOLDFIELDS */
                    ticket.IS_PARENT = true;

                var queryConfiguracion = db.ConfiguracionCategorias.Where(x => x.IdAcco == ID_ACCO && x.Estado == true && x.EsPadre == true && x.IdCate4 == ticket.ID_CATE).ToList().Count();

                if (queryConfiguracion > 0)
                {
                    ticket.IS_PARENT = true;
                }

                int idGrupo = Request.Form["ID_QUEU"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_QUEU"].ToString());

                if (ID_ACCO == 60 && idGrupo == 82)
                {
                    int MacroServicio = Request.Form["ID_CATE_N1"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_CATE_N1"].ToString());
                    int Categoria1 = Request.Form["ID_CATE_N2"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_CATE_N2"].ToString());
                    int Categoria2 = Request.Form["ID_CATE_N3"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_CATE_N3"].ToString());
                    int TypeTicket = Request.Form["ID_TYPE_TICK"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_TYPE_TICK"].ToString());



                    var idSLAsResult = db.SlaBNV(MacroServicio, Categoria1, Categoria2, TypeTicket).ToList();


                    if (idSLAsResult != null && idSLAsResult.Any())
                    {
                        ticket.IdSLA = idSLAsResult.FirstOrDefault() ?? 0;
                    }

                }

                db.TICKETs.Add(ticket);
                db.SaveChanges();
                int id = Convert.ToInt32(ticket.ID_TICK);
                db.Entry(ticket).State = EntityState.Detached;

                if (ticket.ID_ACCO == 60 && ticket.ID_STAT_END == 4)
                {
                    DETAILS_TICKET objDetalleTicket = new DETAILS_TICKET();
                    objDetalleTicket.ID_TICK = id;
                    objDetalleTicket.ID_STAT = 4;
                    objDetalleTicket.ID_TYPE_DETA_TICK = 3;
                    objDetalleTicket.COM_DETA_TICK = ticket.Solucion;
                    objDetalleTicket.UserId = UserId;
                    objDetalleTicket.CREATE_DETA_INCI = DateTime.Now;
                    objDetalleTicket.MINUTES = 0;
                    objDetalleTicket.SEND_MAIL = false;
                    objDetalleTicket.IdTipoResolucion = 1;
                    db.DETAILS_TICKET.Add(objDetalleTicket);
                    db.SaveChanges();
                }

                //if (ticket.ID_ACCO == 1 && ticket.ID_CATE == 15223) /* CATEGORIA CESE DE CUENTA / GOLDFIELDS */
                //{
                //    var qTicketHijo = db.GenerarTicketHijoGF(ticket.ID_AREA, ticket.ID_QUEU, ticket.ID_TICK, ticket.ID_PERS_ENTI, ticket.ID_PERS_ENTI_ASSI, ticket.ID_LOCA, ticket.SUM_TICK);

                //}

                if (queryConfiguracion > 0)
                {
                    //string[] partes = ticket.SUM_TICK.Split(new string[] { "<p>" }, StringSplitOptions.None);

                    //if (partes.Length >= 3)
                    //{
                    //    // Unir las partes desde la tercera en adelante
                    //    string resultado = string.Join("<p>", partes, 2, partes.Length - 2);

                    //    // Eliminar etiquetas restantes al final
                    //    resultado = resultado.Substring(0, resultado.LastIndexOf("</p>"));

                    //    var ticketsHijos = db.CrearTicketsHijos(ticket.ID_CATE, ticket.ID_ACCO, ticket.ID_AREA, ticket.ID_QUEU, ticket.ID_TICK, ticket.ID_PERS_ENTI, ticket.ID_PERS_ENTI_ASSI, ticket.ID_LOCA, resultado);
                    //}

                    var ticketsHijos = db.CrearTicketsHijos(ticket.ID_CATE, ticket.ID_ACCO, ticket.ID_AREA, ticket.ID_QUEU, ticket.ID_TICK, ticket.ID_PERS_ENTI, ticket.ID_PERS_ENTI_ASSI, ticket.ID_LOCA, ticket.SUM_TICK);
                }

                //if ((ticket.ID_ACCO == 56 || ticket.ID_ACCO == 57 || ticket.ID_ACCO == 58) && (ticket.ID_CATE == 27011 || ticket.ID_CATE == 28382 || ticket.ID_CATE == 29753))  /* CATEGORIA CESE DE CUENTA / MINSUR MARCOBRE RAURA */
                //{

                //    var listaTareaBaja = db.Tarea.Where(x => x.TipoGestion == "BAJA" && x.Estado == true).ToList();

                //    foreach (Tarea tarea in listaTareaBaja)
                //    {
                //        TareaDetalle objTareaDetalle = new TareaDetalle();

                //        objTareaDetalle.Id_Tick = ticket.ID_TICK;
                //        objTareaDetalle.IdTarea = tarea.IdTarea;
                //        objTareaDetalle.Id_Queu = tarea.Id_Queu;
                //        objTareaDetalle.FechaCreacion = DateTime.Now;
                //        objTareaDetalle.IdEstado = 5;
                //        db.TareaDetalle.Add(objTareaDetalle);
                //        db.SaveChanges();
                //    }
                //}

                //if ((ticket.ID_ACCO == 56 || ticket.ID_ACCO == 57 || ticket.ID_ACCO == 58) && (ticket.ID_CATE == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMinsur"]) || ticket.ID_CATE == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMarcobre"]) || ticket.ID_CATE == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeRaura"])))  /* CATEGORIA INTERMEDIACION / MINSUR MARCOBRE RAURA */
                //{

                //    var listaTareaBaja = db.Tarea.Where(x => x.TipoGestion == "INTERMEDIACION" && x.Estado == true).ToList();

                //    foreach (Tarea tarea in listaTareaBaja)
                //    {
                //        TareaDetalle objTareaDetalle = new TareaDetalle();

                //        objTareaDetalle.Id_Tick = ticket.ID_TICK;
                //        objTareaDetalle.IdTarea = tarea.IdTarea;
                //        objTareaDetalle.Id_Queu = tarea.Id_Queu;
                //        objTareaDetalle.FechaCreacion = DateTime.Now;
                //        objTareaDetalle.IdEstado = 5;
                //        db.TareaDetalle.Add(objTareaDetalle);
                //        db.SaveChanges();
                //    }
                //}

                if (ticket.ID_ACCO == 56 || ticket.ID_ACCO == 57 || ticket.ID_ACCO == 58)
                {
                    //Altas (?)
                    if (ticket.ID_CATE == 27009 || ticket.ID_CATE == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMinsur"]) || ticket.ID_CATE == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeMarcobre"]) || ticket.ID_CATE == Convert.ToInt32(ConfigurationManager.AppSettings["IdIntermeRaura"]))
                    {
                        string fechaIngreso = Request.Form["FechaIngreso"].ToString();
                        DateTime fechaIngresoTime = new DateTime();
                        DateTime.TryParse(fechaIngreso, out fechaIngresoTime);
                        Gestion_Usuarios altaUsuario = new Gestion_Usuarios();
                        altaUsuario.IdModAtencion = 1;
                        altaUsuario.Fecha_Ingreso = fechaIngresoTime;
                        altaUsuario.Estado = true;
                        altaUsuario.FechaCrea = DateTime.Now;
                        altaUsuario.UsuarioCrea = UserId;
                        altaUsuario.IdTicket = id;
                        dbe.Gestion_Usuarios.Add(altaUsuario);
                        dbe.SaveChanges();
                    }
                    //Bajas (?)
                    if (ticket.ID_CATE == 27011)
                    {
                        string fechaCese = Request.Form["FechaIngreso"].ToString();
                        DateTime fechaCeseTime = new DateTime();
                        DateTime.TryParse(fechaCese, out fechaCeseTime);
                        Control_Cese bajaUsuario = new Control_Cese();
                        bajaUsuario.ID_ACCO = ID_ACCO;
                        bajaUsuario.Id_ModAtencion = 2;
                        bajaUsuario.ID_PERS_ENTI = ID_PERS_ENTI;
                        bajaUsuario.ID_PERS_ENTI_END = ticket.ID_PERS_ENTI_END;
                        bajaUsuario.Estado = true;
                        bajaUsuario.FechaCese = fechaCeseTime;
                        bajaUsuario.ID_TICKET = id;
                        bajaUsuario.FechaCrea = DateTime.Now;
                        bajaUsuario.UsuarioCrea = UserId;
                        dbe.Control_Cese.Add(bajaUsuario);
                        dbe.SaveChanges();
                    }
                }

                //TAREA
                if (ticket.ID_ACCO == 56 || ticket.ID_ACCO == 57 || ticket.ID_ACCO == 58)
                {
                    var tieneTarea = db.CATEGORies.FirstOrDefault(x => x.ID_CATE == ticket.ID_CATE).TieneTarea;

                    //SAP
                    if (tieneTarea == true)
                        db.CrearTareasxCategory(ticket.ID_CATE, ticket.ID_TICK, Convert.ToInt32(Session["ID_PERS_ENTI"]));

                    var tareasModulo = db.CategoriaConTareas.FirstOrDefault(x => x.IdCate == ticket.ID_CATE && x.Estado == true);
                    //MODULO
                    if (tareasModulo != null)
                        db.InsertarTareasPorTicket(ticket.ID_TICK, ticket.ID_CATE, ticket.ID_ACCO, UserId);
                }
                else if (ticket.ID_ACCO == 60)
                {
                    var tareasCreadas = db.InsertarTareasCrearTicketBNV(ticket.ID_TICK, ticket.ID_CATE, UserId).ToList();
                    foreach (var tarea in tareasCreadas)
                    {
                        ERPElectrodata.Objects.SendMail mail = new ERPElectrodata.Objects.SendMail();
                        var envio = mail.sendMailTareasBNV(tarea.IdTarea);
                        if (envio == true)
                        {
                            db.EnvioCorreoTareaBNV(tarea.IdTarea, 1);
                        }
                    }
                }

                int bandera = 0;

                if (KEY_ATTA != null)
                {
                    var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                        .Where(x => x.ID_INCI == null).ToList();
                    if (Attachs.Count() > 0)
                    {
                        foreach (ATTACHED attach in Attachs)
                        {
                            try
                            {
                                var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                                EObj.ID_INCI = id;
                                db.Entry(EObj).State = EntityState.Modified;
                                db.SaveChanges();
                                db.Entry(EObj).State = EntityState.Detached;
                            }
                            catch
                            {

                            }

                        }
                        bandera = 1;
                    }
                }

                string code = Convert.ToString(db.TICKETs.Single(t => t.ID_TICK == id).COD_TICK);

                int ID_PERS_ENTI_SESS = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                pt.UpdatePointsByTicket(ticket, ID_PERS_ENTI_SESS, bandera);

                //Insertar Ticket Automático
                if (taTicketAutomatico == 1)
                {
                    TicketAutomatico objTicketAutomatico = new TicketAutomatico();
                    objTicketAutomatico.IdCuenta = ID_ACCO;
                    objTicketAutomatico.IdTicket = ticket.ID_TICK;
                    objTicketAutomatico.Titulo = taTitulo;
                    objTicketAutomatico.Frecuencia = taFrecuencia;
                    if (taFrecuencia == 5)
                    {
                        objTicketAutomatico.Dia = Convert.ToInt32(taDiaPersonalizado);
                    }
                    else
                    {
                        objTicketAutomatico.Dia = Convert.ToInt32(taDiaMes);
                    }
                    objTicketAutomatico.Vigencia = taVigencia;
                    objTicketAutomatico.VigenciaFechaInicio = Convert.ToDateTime(taVigenciaInicio);
                    objTicketAutomatico.VigenciaFechaFin = Convert.ToDateTime(taVigenciaFin);
                    objTicketAutomatico.HoraCreacion = Convert.ToDateTime(taHoraCreacion);
                    objTicketAutomatico.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                    objTicketAutomatico.Estado = true;
                    objTicketAutomatico.FechaCrea = DateTime.Now;
                    db.TicketAutomaticoes.Add(objTicketAutomatico);
                    db.SaveChanges();

                    string[] Dias = taDiaSemana.Split('-');
                    foreach (string dia in Dias)
                    {
                        if (!String.IsNullOrEmpty(dia))
                        {
                            TicketAutomaticoDia objTicketAutoDia = new TicketAutomaticoDia();
                            objTicketAutoDia.IdTicketAutomatico = objTicketAutomatico.Id;
                            objTicketAutoDia.Dia = Convert.ToInt32(dia);
                            db.TicketAutomaticoDias.Add(objTicketAutoDia);
                            db.SaveChanges();
                        }
                    }
                }

                //SendMail mail = new SendMail();
                //string xx = mail.NewTicket(id);
                if (ID_ACCO == 60)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneBnv('OK','" + code + "-" + id + "','" + id + "');}window.onload=init;</script>");
                }
                else
                if (ID_ACCO == (int)Minsur.MARCOBRE ||
                    ID_ACCO == (int)Minsur.RAURA ||
                    ID_ACCO == (int)Minsur.MINSUR)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneMinsur('OK','" + code + "-" + id + "','" + id + "');}window.onload=init;</script>");
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('OK','" + code + "-" + id + "');}window.onload=init;</script>");
                }

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','0',);}window.onload=init;</script>");
            }

            // return View();
        }







        [Authorize]
        public ActionResult EditarPreticketBnv(int id = 0)
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var cInc = new TICKET();
                //ObtenerTIcket
                TICKET ticket = db.TICKETs.Find(id);
                ViewBag.ticketId = id;
                ViewBag.tituloTicket = ticket.TITLE_TICK;
                ViewBag.asignado = ticket.ID_PERS_ENTI;
                ViewBag.sumTicket = ticket.SUM_TICK;
                cInc.FEC_TICK = DateTime.Now;
                ViewBag.DATE = String.Format("{0:g}", cInc.FEC_TICK);
                ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
                ViewBag.ACCESO_NEWREQ = 0;
                ViewBag.ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                ViewBag.ID_PERS_ENTI_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                ViewBag.IdAcco = ID_ACCO;
                ViewBag.IdSource = ticket.ID_SOUR;
                ViewBag.IdLoca = ticket.ID_LOCA;
                ViewBag.ID_STAT = 1;
                ViewBag.FechaRecepcionSolicitud = ticket.FechaRecepcionSolicitud;
                ViewBag.ID_PERS_ENTI = ticket.ID_PERS_ENTI;
                ViewBag.ID_COMP = ticket.ID_COMP;
                ViewBag.ID_PERS_ENTI_END = ticket.ID_PERS_ENTI_END;
                var horaServer = DateTime.Now;
                var horaServerString = horaServer.ToString("s");
                ViewBag.DATETIMESERVER = horaServerString;
                //--------------Corregido
                if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1 || Convert.ToInt32((Session["SERVICEDESK"].ToString())) == 1 || Convert.ToInt32((Session["ADMINISTRADOR_SISTEMA"].ToString())) == 1)
                {
                    ViewBag.ACCESO_NEWREQ = 1;
                }
                return View();
            }
            catch
            {
                return Content("Please Close Session");
            }
        }

        // Editar PreTicket
        [HttpPost, ValidateInput(false)]
        public ActionResult EditarPreticketBnv(IEnumerable<HttpPostedFileBase> file, TICKET ticket)
        {
            //Crear nuevo ticket a modificar
            TICKET ticketModificar = db.TICKETs.Find(ticket.ID_TICK);

            //Rasignar el ticket obtenido al ticket a editar
            ticketModificar.TITLE_TICK = ticket.TITLE_TICK;
            ticketModificar.ID_TYPE_TICK = ticket.ID_TYPE_TICK;
            ticketModificar.ID_PRIO = ticket.ID_PRIO;
            ticketModificar.ID_SOUR = ticket.ID_SOUR;
            ticketModificar.FEC_INI_TICK = ticket.FEC_INI_TICK;
            ticketModificar.FechaRecepcionSolicitud = ticket.FechaRecepcionSolicitud;
            ticketModificar.IdRazon = ticket.IdRazon;
            ticketModificar.TicketExterno = ticket.TicketExterno;
            ticketModificar.ID_PERS_ENTI = ticket.ID_PERS_ENTI;
            ticketModificar.ID_PERS_ENTI_END = ticket.ID_PERS_ENTI_END;
            ticketModificar.ID_QUEU = ticket.ID_QUEU;
            ticketModificar.ID_PERS_ENTI_ASSI = ticket.ID_PERS_ENTI_ASSI;
            ticketModificar.ID_CATE = ticket.ID_CATE;
            ticketModificar.REM_CTRL_TICK = ticket.REM_CTRL_TICK;
            ticketModificar.IS_PARENT = ticket.IS_PARENT;
            ticketModificar.ID_TICK_PARENT = ticket.ID_TICK_PARENT;
            ticketModificar.IdGrupoReportador = ticket.IdGrupoReportador;
            ticketModificar.IdModTrabajo = ticket.IdModTrabajo;
            ticketModificar.Solman = ticket.Solman;
            ticketModificar.SERVICE = ticket.SERVICE;
            ticketModificar.FEC_INI_REAL = ticket.FEC_INI_REAL;
            ticketModificar.SUM_TICK = ticket.SUM_TICK;
            ticketModificar.Solucion = ticket.Solucion;
            ticketModificar.IdSLA = ticket.IdSLA;
            ticketModificar.FEEDBACKs = ticket.FEEDBACKs;
            ticketModificar.ATTACHEDs = ticket.ATTACHEDs;
            ticketModificar.ASSETs = ticket.ASSETs;
            ticketModificar.CHANGE_REQUEST = ticket.CHANGE_REQUEST;
            ticketModificar.CodigoSMS = ticket.CodigoSMS;
            //ticketModificar.DETAILS_TICKET = ticket.DETAILS_TICKET;
            ticketModificar.FEC_TICK = ticket.FEC_TICK;
            ticketModificar.ID_CIA = ticket.ID_CIA;
            ticketModificar.ID_COMP = ticket.ID_COMP;
            ticketModificar.ID_PERS_ENTI_ANLT = ticket.ID_PERS_ENTI_ANLT;
            ticketModificar.ID_STAT = ticket.ID_STAT;
            ticketModificar.ID_STAT_END = ticket.ID_STAT_END;
            ticketModificar.IS_PARENT = ticket.IS_PARENT;
            ticketModificar.IS_ROLE_USER = ticket.IS_ROLE_USER;
            ticketModificar.IdTipoPase = ticket.IdTipoPase;
            ticketModificar.REM_CTRL_TICK = ticket.REM_CTRL_TICK;
            ticketModificar.Solman = ticket.Solman;
            ticketModificar.TICKET_NOTIFICATION = ticket.TICKET_NOTIFICATION;
            ticketModificar.TYPE_TICKET = ticket.TYPE_TICKET;
            ticketModificar.TicketAutomaticoes = ticket.TicketAutomaticoes;
            ticketModificar.TicketExterno = ticket.TicketExterno;
            ticketModificar.UserId = ticket.UserId;

            //SubCuenta
            string subCuenta = Request.Form["hdnSubCuenta"].ToString();
            //TicketAutomatico
            int taTicketAutomatico = Convert.ToInt32(Request.Form["hdnTkAutomatico"].ToString());
            string taTitulo = Request.Form["hdnTitulo"].ToString();
            int taFrecuencia = Convert.ToInt32(Request.Form["hdnFrecuencia"].ToString());
            string taDiaSemana = Request.Form["hdnDiaSemana"].ToString();
            string taDiaMes = Request.Form["hdnDiaMes"].ToString();
            int taVigencia = Convert.ToInt32(Request.Form["hdnVigencia"].ToString());
            string taVigenciaInicio = Request.Form["hdnVigenciaInicio"].ToString();
            string taVigenciaFin = Request.Form["hdnVigenciaFin"].ToString();
            string taHoraCreacion = Request.Form["hdnHoraCreacion"].ToString();
            int UserId = Convert.ToInt32(Session["UserId"]);

            /*GESTION DEL CONOCIMIENTO GCR161203*/
            int ID_TEMA = 0; ID_TEMA = Convert.ToInt32(Request.Form["IdTema"]);
            /*Fin*/
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string FEC_INI_TICK = Convert.ToString(Request.Form["FEC_INI_TICK"]);
            string KEY_ATTA = null;
            int ID_PERS_ENTI = 0;
            int hours = 0;


            if (ID_ACCO == 60)
            {
                if (ticket.TITLE_TICK == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe llenar el Título de la Solicitud.<br/>');}window.onload=init;</script>");
                }
                if (Convert.ToInt32(ticket.ID_TYPE_TICK) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Tipo de Ticket.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_PRIO) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar la Prioridad<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_SOUR) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Medio de Comunicación. <br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_COMP) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar la Compañia. <br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_PERS_ENTI) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Solicitante<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar al Usuario Afectado<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_PERS_ENTI_ASSI) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar al Asignado.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_STAT) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el Estado del Ticket.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_CATE) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar las Categorías.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.IdGrupoReportador) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar el grupo Repordato.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.IdModTrabajo) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar La modalidad de Trabajo.<br/>');}window.onload=init;</script>");

                }
                if (Convert.ToInt32(ticket.ID_LOCA) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.Mensaje) top.Mensaje('ERROR','Debe seleccionar la Locacion.<br/>');}window.onload=init;</script>");

                }
            }

            try
            {
                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);

            }
            catch
            {

            }
            try
            {
                int id_prio = Convert.ToInt32(ticket.ID_PRIO);
                //hours = Convert.ToInt32(db.PRIORITies.Single(x => x.ID_PRIO == id_prio).HOU_PRIO);
                hours = Convert.ToInt32(db.SLADetalles.Single(x => x.IdPrioridad == id_prio && x.IdSLA == ticket.IdSLA).TiempoAtencion);
                ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);

                try
                {
                    int ID_AREA = Convert.ToInt32(dbe.PERSON_ENTITY.Single(r => r.ID_PERS_ENTI == ID_PERS_ENTI).ID_AREA);
                    ticketModificar.ID_AREA = ID_AREA;
                }
                catch
                {
                    ticketModificar.ID_AREA = null;
                }
            }
            catch
            {

            }

            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            ticketModificar.FEC_TICK = DateTime.Now;
            ticket.FEC_TICK = DateTime.Now;
            if (ModelState.IsValid)
            {

                if (UserId == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (String.IsNullOrEmpty(FEC_INI_TICK) && ticket.ID_STAT == 5)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (ticket.ID_STAT == 5 && ticket.FEC_TICK > ticket.FEC_INI_TICK)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (ticket.ID_STAT != 5)
                {
                    ticket.FEC_INI_TICK = ticket.FEC_TICK;
                    ticketModificar.FEC_INI_TICK = ticket.FEC_TICK;
                }

                DateTime FechaIni;
                try
                {
                    DateTime.TryParse(Convert.ToString(ticket.FEC_INI_TICK), out FechaIni);
                }
                catch
                {
                    DateTime.TryParse(Convert.ToString(ticket.FEC_TICK), out FechaIni);
                }

                ticketModificar.DAT_EXPI_TICK = FechaIni.AddHours(hours);

                ticketModificar.UserId = UserId;
                ticketModificar.ID_ACCO = ID_ACCO;
                if (ID_ACCO == 60) /* Campo de RAZON para creación de Ticket - BNV*/
                {

                    int ID_LOCA = Request.Form["ID_LOCA"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_LOCA"].ToString());
                    int IdGrupoReportador = Request.Form["IdGrupoReportador"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdGrupoReportador"].ToString());
                    int ID_RAZON = Request.Form["IdRazon"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdRazon"].ToString());
                    int ID_TIPOPASE = Request.Form["IdTipoPase"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdTipoPase"].ToString());  //Convert.ToInt32(Request.Form["ID_TIPOPASE"].ToString());
                    int ID_MODTRABAJO = Request.Form["IdModTrabajo"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["IdModTrabajo"].ToString());  //Convert.ToInt32(Request.Form["ID_MODTRABAJO"].ToString());
                    string SOLMAN = Request.Form["Solman"] == null ? null : Convert.ToString(Request.Form["Solman"]);
                    int ID_PERS_ENTI_ANLT = Request.Form["ID_PERS_ENTI_ANLT"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_PERS_ENTI_ANLT"].ToString());
                    string Solucion = Request.Form["Solucion"] == null ? null : Convert.ToString(Request.Form["Solucion"]);

                    //string SOLMAN = Request.Form["SOLMAN"].ToString();

                    if (ID_LOCA == 0) { ticketModificar.ID_LOCA = null; } else { ticketModificar.ID_LOCA = ID_LOCA; }
                    if (IdGrupoReportador == 0) { ticketModificar.IdGrupoReportador = null; } else { ticketModificar.IdGrupoReportador = IdGrupoReportador; }
                    if (ID_RAZON == 0) { ticketModificar.IdRazon = null; } else { ticketModificar.IdRazon = ID_RAZON; }
                    if (ID_TIPOPASE == 0) { ticketModificar.IdTipoPase = null; } else { ticketModificar.IdTipoPase = ID_TIPOPASE; }
                    if (ID_MODTRABAJO == 0) { ticketModificar.IdModTrabajo = null; } else { ticketModificar.IdModTrabajo = ID_MODTRABAJO; }
                    if (SOLMAN == "") { ticketModificar.Solman = null; } else { ticketModificar.Solman = SOLMAN; }
                    if (ID_PERS_ENTI_ANLT == 0) { ticketModificar.ID_PERS_ENTI_ANLT = null; } else { ticketModificar.ID_PERS_ENTI_ANLT = ID_PERS_ENTI_ANLT; }
                    if (Solucion == "") { ticketModificar.Solucion = null; } else { ticketModificar.Solucion = Solucion; }


                }
                ticketModificar.ID_STAT_END = Convert.ToInt32(Request.Form["ID_STAT"]);
                ticketModificar.CREATE_TICK = DateTime.Now;
                ticketModificar.MODIFIED_TICK = DateTime.Now;
                ticketModificar.SEND_MAIL = false;
                ticketModificar.SEND_SURVEY = null;
                ticketModificar.SubCuenta = subCuenta;
                ticketModificar.IdTema = ID_TEMA;  /*GESTION DEL CONOCIMIENTO GCR161203*/
                //db.TICKETs.add(ticket);
                db.Entry(ticketModificar).State = EntityState.Modified;
                db.SaveChanges();
                int id = Convert.ToInt32(ticketModificar.ID_TICK);
                db.Entry(ticketModificar).State = EntityState.Detached;

                if (ticketModificar.ID_ACCO == 60 && ticketModificar.ID_STAT_END == 4)
                {
                    DETAILS_TICKET objDetalleTicket = new DETAILS_TICKET();
                    objDetalleTicket.ID_TICK = id;
                    objDetalleTicket.ID_STAT = 4;
                    objDetalleTicket.ID_TYPE_DETA_TICK = 3;
                    objDetalleTicket.COM_DETA_TICK = ticketModificar.Solucion;
                    objDetalleTicket.UserId = UserId;
                    objDetalleTicket.CREATE_DETA_INCI = DateTime.Now;
                    objDetalleTicket.MINUTES = 0;
                    objDetalleTicket.SEND_MAIL = false;
                    objDetalleTicket.IdTipoResolucion = 1;
                    db.DETAILS_TICKET.Add(objDetalleTicket);
                    db.SaveChanges();
                }
                int bandera = 0;

                if (KEY_ATTA != null)
                {
                    var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                        .Where(x => x.ID_INCI == null).ToList();
                    if (Attachs.Count() > 0)
                    {
                        foreach (ATTACHED attach in Attachs)
                        {
                            try
                            {
                                var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                                EObj.ID_INCI = id;
                                db.Entry(EObj).State = EntityState.Modified;
                                db.SaveChanges();
                                db.Entry(EObj).State = EntityState.Detached;
                            }
                            catch
                            {

                            }

                        }
                        bandera = 1;
                    }
                }

                string code = Convert.ToString(db.TICKETs.Single(t => t.ID_TICK == id).COD_TICK);

                int ID_PERS_ENTI_SESS = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                pt.UpdatePointsByTicket(ticketModificar, ID_PERS_ENTI_SESS, bandera);

                //Insertar Ticket Automático
                if (taTicketAutomatico == 1)
                {
                    TicketAutomatico objTicketAutomatico = new TicketAutomatico();
                    objTicketAutomatico.IdCuenta = ID_ACCO;
                    objTicketAutomatico.IdTicket = ticketModificar.ID_TICK;
                    objTicketAutomatico.Titulo = taTitulo;
                    objTicketAutomatico.Frecuencia = taFrecuencia;
                    objTicketAutomatico.Dia = Convert.ToInt32(taDiaMes);
                    objTicketAutomatico.Vigencia = taVigencia;
                    objTicketAutomatico.VigenciaFechaInicio = Convert.ToDateTime(taVigenciaInicio);
                    objTicketAutomatico.VigenciaFechaFin = Convert.ToDateTime(taVigenciaFin);
                    objTicketAutomatico.HoraCreacion = Convert.ToDateTime(taHoraCreacion);
                    objTicketAutomatico.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                    objTicketAutomatico.Estado = true;
                    objTicketAutomatico.FechaCrea = DateTime.Now;
                    db.TicketAutomaticoes.Add(objTicketAutomatico);
                    db.SaveChanges();

                    string[] Dias = taDiaSemana.Split('-');
                    foreach (string dia in Dias)
                    {
                        if (!String.IsNullOrEmpty(dia))
                        {
                            TicketAutomaticoDia objTicketAutoDia = new TicketAutomaticoDia();
                            objTicketAutoDia.IdTicketAutomatico = objTicketAutomatico.Id;
                            objTicketAutoDia.Dia = Convert.ToInt32(dia);
                            db.TicketAutomaticoDias.Add(objTicketAutoDia);
                            db.SaveChanges();
                        }
                    }
                }

                //SendMail mail = new SendMail();
                //string xx = mail.NewTicket(id);
                if (ID_ACCO == 60)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDoneBnv('OK','" + code + "-" + id + "','" + id + "');}window.onload=init;</script>");
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('OK','" + code + "-" + id + "');}window.onload=init;</script>");
                }

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','0',);}window.onload=init;</script>");
            }

            // return View();
        }

        //
        // GET: /Ticket/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TICKET ticket = db.TICKETs.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", ticket.ID_CLIE);
            ViewBag.ID_AFEC_END_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", ticket.ID_AFEC_END_CLIE);
            ViewBag.ID_ASSI = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", ticket.ID_ASSI);
            ViewBag.ID_QUEU = new SelectList(db.QUEUEs, "ID_QUEU", "NAM_QUEU", ticket.ID_QUEU);
            ViewBag.ID_SOUR = new SelectList(db.SOURCEs, "ID_SOUR", "NAM_SOUR", ticket.ID_SOUR);
            ViewBag.ID_STAT = new SelectList(db.STATUS, "ID_STAT", "NAM_STAT", ticket.ID_STAT);
            ViewBag.ID_STAT_END = new SelectList(db.STATUS, "ID_STAT", "NAM_STAT", ticket.ID_STAT_END);
            ViewBag.ID_TYPE_TICK = new SelectList(db.TYPE_TICKET, "ID_TYPE_TICK", "NAM_TYPE_TICK", ticket.ID_TYPE_TICK);
            return View(ticket);
        }

        //
        // POST: /Ticket/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TICKET ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", ticket.ID_CLIE);
            ViewBag.ID_AFEC_END_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", ticket.ID_AFEC_END_CLIE);
            ViewBag.ID_ASSI = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", ticket.ID_ASSI);
            ViewBag.ID_QUEU = new SelectList(db.QUEUEs, "ID_QUEU", "NAM_QUEU", ticket.ID_QUEU);
            ViewBag.ID_SOUR = new SelectList(db.SOURCEs, "ID_SOUR", "NAM_SOUR", ticket.ID_SOUR);
            ViewBag.ID_STAT = new SelectList(db.STATUS, "ID_STAT", "NAM_STAT", ticket.ID_STAT);
            ViewBag.ID_STAT_END = new SelectList(db.STATUS, "ID_STAT", "NAM_STAT", ticket.ID_STAT_END);
            ViewBag.ID_TYPE_TICK = new SelectList(db.TYPE_TICKET, "ID_TYPE_TICK", "NAM_TYPE_TICK", ticket.ID_TYPE_TICK);
            return View(ticket);
        }

        [Authorize]
        public ActionResult EditSummary(int id)
        {
            TICKET query = db.TICKETs.Single(x => x.ID_TICK == id);
            return View(query);
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditSummary(int id, int id1 = 0)
        {
            try
            {
                int ID_TICK = Convert.ToInt32(Request.Params["ID_TICK"]);
                var SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]);
                var FEC_INI_REAL = Convert.ToDateTime(Request.Params["FEC_INI_REAL"]);

                if (String.IsNullOrEmpty(SUM_TICK.Trim()))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Summary Empty');}window.onload=init;</script>");
                }

                TICKET query = db.TICKETs.Single(x => x.ID_TICK == id);

                query.SUM_TICK = SUM_TICK;
                if (Convert.ToInt32(Session["ID_ACCO"]) == 3)
                {
                    query.FEC_INI_REAL = FEC_INI_REAL;
                }
                db.Entry(query).State = EntityState.Modified;
                db.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('OK','Register Modified');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Contact your Administrator');}window.onload=init;</script>");

            }
            //return View();
        }

        //
        // GET: /Ticket/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TICKET ticket = db.TICKETs.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //
        // POST: /Ticket/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            TICKET ticket = db.TICKETs.Find(id);
            db.TICKETs.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Ticket/FindFormat/
        public ActionResult FindFormat()
        {
            var bTick = new TICKET();
            return View(bTick);
        }

        #region "Gestión de Problemas"
        /*Ticket Problema*/
        //
        // GET: /Ticket/Find

        public ActionResult CrearTicketProblemaConfig()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearTicketProblemaConfig(TicketConfiguracion objTicket)
        {
            //Validar campos en blanco
            int flag = 0, existe = 0;

            if (Convert.ToString(objTicket.IdCuenta) == "") { flag = 1; }
            if (objTicket.TiempoMeses == null) { flag = 2; }
            if (objTicket.NroCategorias == null) { flag = 3; }

            int Cuenta = Convert.ToInt32(objTicket.IdCuenta);



            var result = (from tc in db.TicketConfiguracions.Where(x => x.IdCuenta == Cuenta && x.Activo == true).ToList()
                          select new
                          {
                              id = tc.IdTicketConfig,
                              cuenta = tc.IdCuenta
                          }).ToList();

            if (result.Count() > 0)
            {
                existe = 1;
            }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }
            if (existe != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('EXISTE','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {

                objTicket.FechaProceso = Convert.ToDateTime("01-01-2000");
                objTicket.FechaCreacion = DateTime.Now;
                objTicket.Activo = true;
                db.TicketConfiguracions.Add(objTicket);
                db.SaveChanges();


                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objTicket.IdTicketConfig.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult EditarTicketProblemaConfig(int id = 0)
        {
            ViewBag.IdTicketConfig = id;
            var tc = db.TicketConfiguracions.Find(id);
            var c = db.ACCOUNTs.Single(x => x.ID_ACCO == tc.IdCuenta);
            ViewBag.IdCuenta = tc.IdCuenta;
            ViewBag.NAM_ACCO = c.NAM_ACCO;
            ViewBag.TiempoMeses = tc.TiempoMeses;
            ViewBag.NroCategorias = tc.NroCategorias;
            ViewBag.FechaProceso = tc.FechaProceso;
            return View(tc);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditarTicketProblemaConfig(TicketConfiguracion objTicket)
        {
            int NroCategorias = (Request.Params["NroCategoriasE"] == null) ? -1 : Convert.ToInt32(Request.Params["NroCategoriasE"]);
            int TiempoMeses = (Request.Params["TiempoMesesE"] == null) ? -1 : Convert.ToInt32(Request.Params["TiempoMesesE"]);
            //Validar campos en blanco
            int flag = 0;
            if (Convert.ToString(objTicket.IdCuenta) == "") { flag = 1; }
            if (TiempoMeses == -1) { flag = 2; }
            if (NroCategorias == -1) { flag = 3; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                objTicket.NroCategorias = NroCategorias;
                objTicket.TiempoMeses = TiempoMeses;
                objTicket.FechaCreacion = DateTime.Now;
                objTicket.Activo = true;
                db.TicketConfiguracions.Attach(objTicket);
                db.Entry(objTicket).State = EntityState.Modified;
                db.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objTicket.IdTicketConfig.ToString() + "');}window.onload=init;</script>");

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult ListarTicketsProblemaConfig(int i = 0)
        {
            // int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int IdCuenta = Convert.ToInt32(Request.Params["IdCuenta"].ToString());

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int meses = Convert.ToInt32(Request.Params["TiempoMeses"].ToString());
            int categorias = Convert.ToInt32(Request.Params["NroCategorias"].ToString());

            var query = db.tktListarTicketsProblemaConfig(IdCuenta, meses, categorias).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult FindTicketProblema()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var tick = db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.ID_DOCU_SALE == null && t.FlagProblema == true);

            if (Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()) == true) /*Los de service desk tienen el flag en true, por eso pasan sin restricciones*/
            {
                ViewBag.TAActive = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3).Count();
                ViewBag.TATemporal = tick.Where(t => t.ID_STAT_END == 8).Count();
                ViewBag.TAResolved = tick.Where(t => t.ID_STAT_END == 4).Count();
                ViewBag.TAClosed = tick.Where(t => t.ID_STAT_END == 6).Count();
            }
            else
            {
                ViewBag.TAActive = 0;
                ViewBag.TATemporal = 0;
                ViewBag.TAResolved = 0;
                ViewBag.TAClosed = 0;
            }

            int idIdioma = Convert.ToInt32(Session["IdIdioma"]);

            string llave = "Asignadoa|Creadopor|Creado|UltimaAct";

            string[] llaves = llave.Split('|');

            var query = from id in dbe.IdiomaDetalle.Where(x => x.IdIdioma == idIdioma).Where(x => llaves.Contains(x.Llave))
                        select new
                        {
                            //cadena = id.Texto + "|" + id.Llave
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

        public ActionResult CrearTicketProblema()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var cInc = new TICKET();
                cInc.FEC_TICK = DateTime.Now;
                ViewBag.DATE = String.Format("{0:g}", cInc.FEC_TICK);
                ViewBag.UploadFile = "M1CT" + Convert.ToString(DateTime.Now.Ticks);
                ViewBag.ACCESO_NEWREQ = 0;
                ViewBag.ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                ViewBag.ID_PERS_ENTI_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                ViewBag.IdCuenta = ID_ACCO;


                /*string UserName = Convert.ToString(Session["UserName"]);
                string[] rolesArray = Roles.GetRolesForUser(UserName);

                foreach (string xc in rolesArray)
                {
                    int i = Array.IndexOf(rolesArray, xc);
                    if (xc == "SERVICE DESK" || xc == "SYSTEMADMINISTRATOR" || xc == "ADMINISTRADOR")
                    {
                        ViewBag.ACCESO_NEWREQ = 1;
                    }
                }*/
                //--------------Corregido
                if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1 || Convert.ToInt32((Session["SERVICEDESK"].ToString())) == 1 || Convert.ToInt32((Session["ADMINISTRADOR_SISTEMA"].ToString())) == 1)
                {
                    ViewBag.ACCESO_NEWREQ = 1;
                }
                int comp = 0;
                bool cia = db.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).VIS_COMP.Value;
                if (cia)
                {
                    comp = 1;
                }
                ViewBag.Compania = comp;
                return View();
            }
            catch
            {
                return Content("Please Close Session");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearTicketProblema(IEnumerable<HttpPostedFileBase> file, TICKET ticket, ComTicketProblema ticketProblema)
        {
            int flag = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            //Tickets Similares
            int Cont = Convert.ToInt32(Request.Params["cont"]);
            string KEY_ATTA = null;
            int ID_PERS_ENTI = 0;
            //int hours = 0;
            if (Convert.ToString(ticket.ID_QUEU) == "") { flag = 1; }
            if (Convert.ToString(ticket.ID_PERS_ENTI) == "") { flag = 2; }
            if (Convert.ToString(ticket.ID_PERS_ENTI_END) == "") { flag = 3; }
            if (Convert.ToString(ticket.ID_PERS_ENTI_ASSI) == "") { flag = 4; }
            if (Convert.ToString(ticket.ID_CATE) == "") { flag = 5; }
            if (Cont == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR1','NroTickets','0');}window.onload=init;</script>");
            }
            try
            {
                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);
            }
            catch
            {

            }
            ID_PERS_ENTI = Convert.ToInt32(ticket.ID_PERS_ENTI);

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }
            //if (ModelState.IsValid)
            //{
            try
            {
                if (ticket.ID_COMP == null)
                {
                    int comp = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI1.Value;
                    ticket.ID_COMP = comp;
                    ticket.ID_COMP_END = comp;
                    ticket.ID_CIA = comp;
                }
                else
                {
                    int IdCompañia = Convert.ToInt32(ticket.ID_COMP);
                    ticket.ID_COMP_END = IdCompañia;
                    ticket.ID_CIA = IdCompañia;
                }
                int ID_AREA = Convert.ToInt32(dbe.PERSON_ENTITY.Single(r => r.ID_PERS_ENTI == ID_PERS_ENTI).ID_AREA);
                ticket.ID_AREA = ID_AREA;
            }
            catch
            {
                ticket.ID_AREA = null;
            }

            ticket.ID_ACCO = ID_ACCO;
            ticket.ID_TYPE_TICK = 2;
            ticket.ID_PRIO = 5;
            ticket.IdSLA = 1;
            ticket.ID_STAT = 1;
            ticket.ID_STAT_END = 1;
            ticket.ID_SOUR = 4;
            ticket.FEC_TICK = DateTime.Now;
            ticket.FEC_INI_TICK = DateTime.Now;
            //ticket.COD_TICK = 1;

            string descripcion = "";
            for (int i = 1; i <= Cont; i++)
            {
                descripcion = descripcion + "<a target='_blank' href='/DetailsTicket/Index/" + Convert.ToInt32(Request.Params["RECEIVER" + i + ""]) + "'" + " style='text-decoration:none;'>" +
                "<span class='glyphicon glyphicon-play' aria-hidden='true'></span><span>" + Convert.ToInt32(Request.Params["RECEIVER" + i + ""]) + " </span></a>";
            }
            ticket.SUM_TICK = Convert.ToString(Request.Form["SUM_TICK"]) +
                "<br/>Estos Ticket se han generado en relación a  " + descripcion;
            ticket.REM_CTRL_TICK = false;
            ticket.UserId = Convert.ToInt32(Session["UserId"]);
            ticket.CREATE_TICK = DateTime.Now;
            ticket.MODIFIED_TICK = DateTime.Now;
            ticket.ID_TICK_PARENT = null;
            ticket.IS_PARENT = false;
            //ticket.ID_PERS_ENTI_ASSI = Convert.ToInt32(Request.Form["ID_PERS_ENTI_ASSI"]);
            //ticket.ID_CATE = Convert.ToInt32(Request.Form["ID_CATE"]);
            ticket.DAT_EXPI_TICK = DateTime.Now;
            ticket.AMM_SALE_OPPO = 0;
            ticket.SEND_MAIL = false;
            ticket.IdTema = 0;  /*GESTION DEL CONOCIMIENTO GCR161203*/
            ticket.FlagProblema = true;


            db.TICKETs.Add(ticket);
            db.SaveChanges();

            #region ATTACH
            int id = Convert.ToInt32(ticket.ID_TICK);
            db.Entry(ticket).State = EntityState.Detached;

            int bandera = 0;

            if (KEY_ATTA != null)
            {
                var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                    .Where(x => x.ID_INCI == null).ToList();
                if (Attachs.Count() > 0)
                {
                    foreach (ATTACHED attach in Attachs)
                    {
                        try
                        {
                            var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                            EObj.ID_INCI = id;
                            db.Entry(EObj).State = EntityState.Modified;
                            db.SaveChanges();
                            db.Entry(EObj).State = EntityState.Detached;
                        }
                        catch
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                                                                                    "if(top.uploadDone) top.uploadDone('ERROR','Error al Adjuntar archivo.');}window.onload=init;</script>");
                        }
                    }
                    bandera = 1;
                }
            }
            #endregion
            #region ComTicketProblema
            for (int i = 1; i <= Cont; i++)
            {
                int ticketsRelacionados = Convert.ToInt32(Request.Params["RECEIVER" + i + ""]);

                ticketProblema.IdTicketProblema = Convert.ToInt32(ticket.ID_TICK);
                ticketProblema.idTicket = ticketsRelacionados;
                ticketProblema.IdCategoria1 = Convert.ToInt32(Request.Form["ID_CATE_N1"]);
                ticketProblema.IdCategoria2 = Convert.ToInt32(Request.Form["ID_CATE_N2"]);
                ticketProblema.IdCategoria3 = Convert.ToInt32(Request.Form["ID_CATE_N3"]);
                ticketProblema.IdCategoria4 = Convert.ToInt32(Request.Form["ID_CATE"]);
                ticketProblema.IdPrioridad = Convert.ToInt32(ticket.ID_PRIO);
                ticketProblema.Activo = true;
                ticketProblema.UsuarioCreacion = Convert.ToInt32(Session["UserId"]);
                ticketProblema.Creado = DateTime.Now;
                ticketProblema.IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

                db.ComTicketProblemas.Add(ticketProblema);
                db.SaveChanges();

            }


            #endregion



            string code = Convert.ToString(db.TICKETs.Single(t => t.ID_TICK == id).COD_TICK);

            int ID_PERS_ENTI_SESS = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            pt.UpdatePointsByTicket(ticket, ID_PERS_ENTI_SESS, bandera);



            ////SendMail mail = new SendMail();
            ////string xx = mail.NewTicket(id);

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + code + "');}window.onload=init;</script>");
            //}
            //else
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            //}


        }

        public ActionResult FindResultProblemas()
        {
            string PalabraClave = Request.Params["BUSQUEDA_CLAVE"].ToString();
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int Count = 0, ID_QUEU, ID_PRIO, CodTicket, ID_STAT_END,
                CountClose;

            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                    SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]),
                    from = "", to = "";

            var listCate = db.CATEGORies
                .Join(db.CATEGORies, c1 => c1.ID_CATE, c2 => c2.ID_CATE_PARE, (c1, c2) => new { ID_CATE_1 = c1.ID_CATE, ID_CATE_2 = c2.ID_CATE })
                .Join(db.CATEGORies, c2 => c2.ID_CATE_2, c3 => c3.ID_CATE_PARE, (c2, c3) => new { c2.ID_CATE_1, c2.ID_CATE_2, ID_CATE_3 = c3.ID_CATE })
                .Join(db.CATEGORies, c3 => c3.ID_CATE_3, c4 => c4.ID_CATE_PARE, (c3, c4) => new { c3.ID_CATE_1, c3.ID_CATE_2, c3.ID_CATE_3, ID_CATE_4 = c4.ID_CATE });

            int xcd = listCate.Count();

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE"]), out ID_CATE) == true)
            //{
            //    listCate = listCate.Where(t => t.ID_CATE_1 == ID_CATE);
            //}

            try
            {
                //if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CATE"]), out ID_SUB_CATE) == true)
                //{
                //    listCate = listCate.Where(t => t.ID_CATE_2 == ID_SUB_CATE);
                //}
            }
            catch
            {

            }

            try
            {
                //if (Int32.TryParse(Convert.ToString(Request.Params["ID_CLAS"]), out ID_CLAS) == true)
                //{
                //    listCate = listCate.Where(t => t.ID_CATE_3 == ID_CLAS);
                //}
            }
            catch
            {

            }

            try
            {
                //if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CLAS"]), out ID_SUB_CLAS) == true)
                //{
                //    listCate = listCate.Where(t => t.ID_CATE_4 == ID_SUB_CLAS);
                //}
            }
            catch
            {

            }

            var listClient = (from lu in dbe.ACCOUNT_ENTITY
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

            var tick = (from t in db.TICKETs
                        where t.ID_ACCO == ID_ACCO
                        join lc in listCate on t.ID_CATE equals lc.ID_CATE_4
                        join s in db.STATUS on t.ID_STAT_END equals s.ID_STAT
                        where t.FlagProblema == true    /*Filtro para obtener solo tickets problema*/
                        select new
                        {
                            t.DAT_EXPI_TICK,
                            t.ID_ACCO,
                            ID_DOCU_SALE = (t.ID_DOCU_SALE == null ? 0 : t.ID_DOCU_SALE),
                            t.FEC_TICK,
                            t.COD_TICK,
                            t.UserId,
                            t.ID_QUEU,
                            t.ID_PERS_ENTI_ASSI,
                            t.ID_TYPE_TICK,
                            t.ID_PRIO,
                            t.ID_SOUR,
                            t.ID_STAT_END,
                            t.SUM_TICK,
                            t.ID_PERS_ENTI,
                            t.ID_PERS_ENTI_END,
                            t.ID_CATE,
                            t.CREATE_TICK,
                            t.MODIFIED_TICK,
                            t.ID_TICK,
                            t.IS_PARENT,
                            t.ID_TYPE_FORM,
                            s.ORD_STAT,
                            t.ID_COMP,
                            t.ID_COMP_END,
                            t.SERVICE
                        });
            var schbjsd = tick.Count();
            //SEIS ÚLTIMOS MESES DEL TICKET DE LA CUENTA
            //var legend = "bla";
            var legendx = (from t in tick
                           group t by new { t.FEC_TICK.Value.Year, t.FEC_TICK.Value.Month } into g
                           select new
                           {
                               g.Key.Year,
                               g.Key.Month,
                               Count = g.Count(),
                           }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Take(5);

            var legend = (from t in legendx
                          join am in db.ACCOUNTING_MONTH on t.Year equals am.ID_ACCO_YEAR
                          where t.Month == am.ACCO_MONT
                          select new
                          {
                              t.Year,
                              t.Month,
                              t.Count,
                              name = am.NAM_ACCO_MONT.Substring(0, 1) + am.NAM_ACCO_MONT.Substring(1, 2).ToLower()
                          });



            //if (!String.IsNullOrEmpty(COD_TICK))
            //{
            //    tick = tick.Where(t => t.COD_TICK.ToUpper().Contains(COD_TICK.ToUpper()));
            //}
            //if (Int32.TryParse(Convert.ToString(Request.Params["UserId"]), out UserId) == true)
            //{
            //    tick = tick.Where(t => t.UserId == UserId);
            //}

            /*Area responsable*/
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == true)
            {
                tick = tick.Where(t => t.ID_QUEU == ID_QUEU);
            }
            /**/
            if (Int32.TryParse(Convert.ToString(Request.Params["BUSQUEDA_CLAVE"]), out CodTicket) == true)
            {
                tick = tick.Where(t => t.ID_TICK == CodTicket);
            }
            else if (Request.Params["BUSQUEDA_CLAVE"] != "")
            {
                string Comentario = Request.Params["BUSQUEDA_CLAVE"].ToString();
                tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(Comentario.ToUpper()));
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_STAT_END"]), out ID_STAT_END) == true)
            {
                tick = tick.Where(t => t.ID_STAT_END == ID_STAT_END);
            }

            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());

            if (Int32.TryParse(Convert.ToString(ID_STAT), out ID_STAT_END) == true)
            {
                if (ID_STAT_END != 0)
                    tick = tick.Where(t => t.ID_STAT_END == ID_STAT_END);
            }
            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP"]), out ID_COMP) == true)
            //{
            //    tick = tick.Where(t => t.ID_COMP == ID_COMP);
            //}
            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP_END"]), out ID_COMP_END) == true)
            //{
            //    tick = tick.Where(t => t.ID_COMP_END == ID_COMP_END);
            //}

            try
            {
                //if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == true)
                //{
                //    tick = tick.Where(t => t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
                //}
            }
            catch { }

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_TICK"]), out ID_TYPE_TICK) == true)
            //{
            //    tick = tick.Where(t => t.ID_TYPE_TICK == ID_TYPE_TICK);
            //}

            /*Prioriad*/
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PRIO"]), out ID_PRIO) == true)
            {
                tick = tick.Where(t => t.ID_PRIO == ID_PRIO);
            }

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_SOUR"]), out ID_SOUR) == true)
            //{
            //    tick = tick.Where(t => t.ID_SOUR == ID_SOUR);
            //}

            //if (!String.IsNullOrEmpty(SUM_TICK))
            //{
            //    tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(SUM_TICK.ToUpper()));
            //}

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == true)
            //{
            //    tick = tick.Where(t => t.ID_PERS_ENTI == ID_PERS_ENTI);
            //}

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_END"]), out ID_PERS_ENTI_END) == true)
            //{
            //    tick = tick.Where(t => t.ID_PERS_ENTI_END == ID_PERS_ENTI_END);
            //}

            //if (Request.Params["SERVICE"].ToString() != "false")
            //{
            //    tick = tick.Where(t => t.SERVICE == true);
            //}

            var lineClose = (from x in legend
                             join t in tick on x.Year equals t.FEC_TICK.Value.Year
                             where x.Month == t.FEC_TICK.Value.Month && t.ID_STAT_END == 6
                             group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                             select new
                             {
                                 g.Key.ID_STAT_END,
                                 g.Key.Year,
                                 g.Key.Month,
                                 g.Key.Count,
                                 g.Key.name,
                                 RES_COUN = g.Count()

                             }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineOpen = (from x in legend
                            join t in tick on x.Year equals t.FEC_TICK.Value.Year
                            where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)
                            group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                            select new
                            {
                                g.Key.ID_STAT_END,
                                g.Key.Year,
                                g.Key.Month,
                                g.Key.Count,
                                g.Key.name,
                                RES_COUN = g.Count()

                            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineScheduled = (from x in legend
                                 join t in tick on x.Year equals t.FEC_TICK.Value.Year
                                 where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 5)
                                 group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                                 select new
                                 {
                                     g.Key.ID_STAT_END,
                                     g.Key.Year,
                                     g.Key.Month,
                                     g.Key.Count,
                                     g.Key.name,
                                     RES_COUN = g.Count()

                                 }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            //if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE"]), out START_DATE))
            //{
            //    tick = tick.Where(t => t.FEC_TICK >= START_DATE);

            //}

            //if (DateTime.TryParse(Convert.ToString(Request.Params["END_DATE"]), out END_DATE))
            //{
            //    //END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
            //    //END_DATE = END_DATE.AddSeconds(59);
            //    tick = tick.Where(t => t.FEC_TICK <= END_DATE);
            //}

            Count = tick.Count();
            CountClose = tick.Where(t => t.ID_STAT_END == 6).Count();
            try
            {
                from = String.Format("{0:d}", tick.OrderBy(t => t.FEC_TICK).First().FEC_TICK);
                to = String.Format("{0:d}", tick.OrderByDescending(t => t.FEC_TICK).First().FEC_TICK);
            }
            catch
            {

            }

            var resultPie = (from t in tick
                             join st in db.STATUS on t.ID_STAT_END equals st.ID_STAT
                             where t.ID_STAT_END != 6 && t.ID_STAT_END != 2
                             group st by new { st.NAM_GRAP_STAT, st.COL_GRAP_STAT } into g
                             select new
                             {
                                 name = g.Key.NAM_GRAP_STAT.Substring(0, 1) + g.Key.NAM_GRAP_STAT.Substring(1, g.Key.NAM_GRAP_STAT.Length).ToLower(),
                                 color = g.Key.COL_GRAP_STAT,
                                 y = g.Count()
                             }).ToList();

            tick = tick.OrderBy(t => t.ORD_STAT).ThenByDescending(t => t.FEC_TICK).Skip(skip).Take(take);

            int iq = 0;
            int[] numbers = new int[0];
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            /*Validación de permisos tomado del método ListByStatus*/
            if (Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()) == false) /*Los de service desk tienen el flag en true, por eso pasan sin restricciones*/
            {
                int supQueu = 0;
                try
                {
                    var Queus = db.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
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
                    tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);  /*Se toma cuando no es supervisor o usuario de Service Desk*/
                }
                else
                {
                    //Mostrar el ticket cuya cola es la indicada
                    tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value)); /*Usuario de Service D */
                }
            }

            var listTicket = tick.ToList();

            /**/





            var result = (from i in tick.ToList()
                          join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listClient on i.UserId equals cr.UserId
                          join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listClient on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI //into lcia from xcia in lcia.DefaultIfEmpty()
                                                                                        //join re in listClient on i.ID_PERS_ENTI_END equals re.ID_PERS_ENTI
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBC = c4.NAM_CATE.ToLower(),
                              NAM_CATE = c3.NAM_CATE.ToLower(),
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              //REQU = re.Client.ToLower(),
                              REQU = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                              URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                              CREATE_INCI = String.Format("{0:G}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:G}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client.ToLower(),
                              ACCO = ac.NAM_ACCO,
                              VIS_COMP = ac.VIS_COMP,
                              ASSI = asi.Client.ToLower(),
                              EXP_TIME = ExpTime(i.ID_TICK),
                              ID_STAT_END = i.ID_STAT_END,
                              //ID_PERS_ENTI = 
                              CIA_TOOL = cia.COMPANY,//(xcia == null ? "" : xcia.COMPANY),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none")
                          }
                            );

            return Json(new { Data = result, Count = Count, CountClose = CountClose, From = from, To = to, Pie = resultPie, legend = legend, lineClose = lineClose, lineScheduled = lineScheduled, lineOpen = lineOpen }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportProblema()
        {
            string PalabraClave = Request.Params["BUSQUEDA_CLAVE"].ToString();
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            //int take = Convert.ToInt32(Request.Params["take"].ToString());
            int Count = 0, ID_QUEU, ID_PRIO,
                CountClose, ID_STAT_END, CodTicket;


            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                    SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]),
                    from = "", to = "";

            var listCate = db.CATEGORies
                .Join(db.CATEGORies, c1 => c1.ID_CATE, c2 => c2.ID_CATE_PARE, (c1, c2) => new { ID_CATE_1 = c1.ID_CATE, ID_CATE_2 = c2.ID_CATE })
                .Join(db.CATEGORies, c2 => c2.ID_CATE_2, c3 => c3.ID_CATE_PARE, (c2, c3) => new { c2.ID_CATE_1, c2.ID_CATE_2, ID_CATE_3 = c3.ID_CATE })
                .Join(db.CATEGORies, c3 => c3.ID_CATE_3, c4 => c4.ID_CATE_PARE, (c3, c4) => new { c3.ID_CATE_1, c3.ID_CATE_2, c3.ID_CATE_3, ID_CATE_4 = c4.ID_CATE });

            int xcd = listCate.Count();

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE"]), out ID_CATE) == true)
            //{
            //    listCate = listCate.Where(t => t.ID_CATE_1 == ID_CATE);
            //}

            try
            {
                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CATE"]), out ID_SUB_CATE) == true)
                //    {
                //        listCate = listCate.Where(t => t.ID_CATE_2 == ID_SUB_CATE);
                //    }
            }
            catch
            {

            }

            try
            {
                //if (Int32.TryParse(Convert.ToString(Request.Params["ID_CLAS"]), out ID_CLAS) == true)
                //{
                //    listCate = listCate.Where(t => t.ID_CATE_3 == ID_CLAS);
                //}
            }
            catch
            {

            }

            try
            {
                //if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CLAS"]), out ID_SUB_CLAS) == true)
                //{
                //    listCate = listCate.Where(t => t.ID_CATE_4 == ID_SUB_CLAS);
                //}
            }
            catch
            {

            }

            var listClient = (from lu in dbe.ACCOUNT_ENTITY
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
                               IDCIA = ce.ID_ENTI
                           }).ToList();

            var tick = (from t in db.TICKETs
                        where t.ID_ACCO == ID_ACCO
                        join lc in listCate on t.ID_CATE equals lc.ID_CATE_4
                        join s in db.STATUS on t.ID_STAT_END equals s.ID_STAT
                        where t.FlagProblema == true    /*Filtro para obtener solo tickets problema*/
                        select new
                        {
                            t.ID_ACCO,
                            ID_DOCU_SALE = (t.ID_DOCU_SALE == null ? 0 : t.ID_DOCU_SALE),
                            t.FEC_TICK,
                            t.COD_TICK,
                            t.UserId,
                            t.ID_QUEU,
                            t.ID_PERS_ENTI_ASSI,
                            t.ID_TYPE_TICK,
                            t.ID_PRIO,
                            t.ID_SOUR,
                            t.ID_STAT_END,
                            t.SUM_TICK,
                            t.ID_PERS_ENTI,
                            t.ID_PERS_ENTI_END,
                            t.ID_CATE,
                            t.CREATE_TICK,
                            t.MODIFIED_TICK,
                            t.ID_TICK,
                            t.IS_PARENT,
                            t.ID_TYPE_FORM,
                            s.ORD_STAT,
                            t.ID_COMP,
                            t.ID_COMP_END,
                            t.SERVICE,
                            t.FEC_INI_REAL
                        });
            var schbjsd = tick.Count();
            //SEIS ÚLTIMOS MESES DEL TICKET DE LA CUENTA
            //var legend = "bla";
            var legendx = (from t in tick
                           group t by new { t.FEC_TICK.Value.Year, t.FEC_TICK.Value.Month } into g
                           select new
                           {
                               g.Key.Year,
                               g.Key.Month,
                               Count = g.Count(),
                           }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Take(5);

            var legend = (from t in legendx
                          join am in db.ACCOUNTING_MONTH on t.Year equals am.ID_ACCO_YEAR
                          where t.Month == am.ACCO_MONT
                          select new
                          {
                              t.Year,
                              t.Month,
                              t.Count,
                              name = am.NAM_ACCO_MONT.Substring(0, 1) + am.NAM_ACCO_MONT.Substring(1, 2).ToLower()
                          });



            //if (!String.IsNullOrEmpty(COD_TICK))
            //{
            //    tick = tick.Where(t => t.COD_TICK.ToUpper().Contains(COD_TICK.ToUpper()));
            //}
            //if (Int32.TryParse(Convert.ToString(Request.Params["UserId"]), out UserId) == true)
            //{
            //    tick = tick.Where(t => t.UserId == UserId);
            //}
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == true)
            {
                tick = tick.Where(t => t.ID_QUEU == ID_QUEU);
            }
            /*Area responsable*/
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == true)
            {
                tick = tick.Where(t => t.ID_QUEU == ID_QUEU);
            }
            /**/
            if (Int32.TryParse(Convert.ToString(Request.Params["BUSQUEDA_CLAVE"]), out CodTicket) == true)
            {
                tick = tick.Where(t => t.ID_TICK == CodTicket);
            }
            else if (Request.Params["BUSQUEDA_CLAVE"] != "")
            {
                string Comentario = Request.Params["BUSQUEDA_CLAVE"].ToString();
                tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(Comentario.ToUpper()));
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_STAT_END"]), out ID_STAT_END) == true)
            {
                tick = tick.Where(t => t.ID_STAT_END == ID_STAT_END);
            }
            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP"]), out ID_COMP) == true)
            //{
            //    tick = tick.Where(t => t.ID_COMP == ID_COMP);
            //}
            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_COMP_END"]), out ID_COMP_END) == true)
            //{
            //    tick = tick.Where(t => t.ID_COMP_END == ID_COMP_END);
            //}

            try
            {
                //if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == true)
                //{
                //    tick = tick.Where(t => t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
                //}
            }
            catch { }

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_TICK"]), out ID_TYPE_TICK) == true)
            //{
            //    tick = tick.Where(t => t.ID_TYPE_TICK == ID_TYPE_TICK);
            //}

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PRIO"]), out ID_PRIO) == true)
            {
                tick = tick.Where(t => t.ID_PRIO == ID_PRIO);
            }

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_SOUR"]), out ID_SOUR) == true)
            //{
            //    tick = tick.Where(t => t.ID_SOUR == ID_SOUR);
            //}

            //if (!String.IsNullOrEmpty(SUM_TICK))
            //{
            //    tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(SUM_TICK.ToUpper()));
            //}

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == true)
            //{
            //    tick = tick.Where(t => t.ID_PERS_ENTI == ID_PERS_ENTI);
            //}

            //if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_END"]), out ID_PERS_ENTI_END) == true)
            //{
            //    tick = tick.Where(t => t.ID_PERS_ENTI_END == ID_PERS_ENTI_END);
            //}

            //if (Request.Params["SERVICE"].ToString() != "false")
            //{
            //    tick = tick.Where(t => t.SERVICE == true);
            //}

            var lineClose = (from x in legend
                             join t in tick on x.Year equals t.FEC_TICK.Value.Year
                             where x.Month == t.FEC_TICK.Value.Month && t.ID_STAT_END == 6
                             group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                             select new
                             {
                                 g.Key.ID_STAT_END,
                                 g.Key.Year,
                                 g.Key.Month,
                                 g.Key.Count,
                                 g.Key.name,
                                 RES_COUN = g.Count()

                             }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineOpen = (from x in legend
                            join t in tick on x.Year equals t.FEC_TICK.Value.Year
                            where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)
                            group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                            select new
                            {
                                g.Key.ID_STAT_END,
                                g.Key.Year,
                                g.Key.Month,
                                g.Key.Count,
                                g.Key.name,
                                RES_COUN = g.Count()

                            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineScheduled = (from x in legend
                                 join t in tick on x.Year equals t.FEC_TICK.Value.Year
                                 where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 5)
                                 group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                                 select new
                                 {
                                     g.Key.ID_STAT_END,
                                     g.Key.Year,
                                     g.Key.Month,
                                     g.Key.Count,
                                     g.Key.name,
                                     RES_COUN = g.Count()

                                 }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            //if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE"]), out START_DATE))
            //{
            //    tick = tick.Where(t => t.FEC_TICK >= START_DATE);

            //}

            //if (DateTime.TryParse(Convert.ToString(Request.Params["END_DATE"]), out END_DATE))
            //{
            //    //END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
            //    //END_DATE = END_DATE.AddSeconds(59);
            //    tick = tick.Where(t => t.FEC_TICK <= END_DATE);
            //}

            Count = tick.Count();
            CountClose = tick.Where(t => t.ID_STAT_END == 6).Count();
            try
            {
                from = String.Format("{0:d}", tick.OrderBy(t => t.FEC_TICK).First().FEC_TICK);
                to = String.Format("{0:d}", tick.OrderByDescending(t => t.FEC_TICK).First().FEC_TICK);
            }
            catch
            {

            }

            var resultPie = (from t in tick
                             join st in db.STATUS on t.ID_STAT_END equals st.ID_STAT
                             where t.ID_STAT_END != 6 && t.ID_STAT_END != 2
                             group st by new { st.NAM_GRAP_STAT, st.COL_GRAP_STAT } into g
                             select new
                             {
                                 name = g.Key.NAM_GRAP_STAT.Substring(0, 1) + g.Key.NAM_GRAP_STAT.Substring(1, g.Key.NAM_GRAP_STAT.Length).ToLower(),
                                 color = g.Key.COL_GRAP_STAT,
                                 y = g.Count()
                             }).ToList();

            tick = tick.OrderBy(t => t.ORD_STAT).ThenByDescending(t => t.FEC_TICK);

            var resultProblema = (from i in tick.ToList()
                                  join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                                  join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                                  join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                                  join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                                  join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                                  join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                                  //join cr in listClient on i.UserId equals cr.UserId
                                  join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                                  //join asi in listClient on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                                  //join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI //into lcia from xcia in lcia.DefaultIfEmpty()
                                  //join ciaend in listCIA on (i.ID_COMP_END == null ? 0 : i.ID_COMP_END) equals ciaend.IDCIA into ciaends
                                  //from ciaendx in ciaends.DefaultIfEmpty()
                                  //join re in listClient on i.ID_PERS_ENTI_END equals re.ID_PERS_ENTI
                                  select new
                                  {
                                      CODE = i.COD_TICK,
                                      SUMMARY = StripHtml(Regex.Replace(i.SUM_TICK, "<.*?>", " ")),
                                      STATE = s.NAM_STAT.ToLower(),
                                      SOURCE = so.NAM_SOUR.ToLower(),
                                      SUBCATEGORY = c4.NAM_CATE.ToLower(),
                                      CATEGORY = c3.NAM_CATE.ToLower(),
                                      TYPE_TICKET = tt.NAM_TYPE_TICK.ToLower(),
                                      REQUESTER = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                                      CREATE = String.Format("{0:G}", i.CREATE_TICK),
                                      MODIFIED = String.Format("{0:G}", i.MODIFIED_TICK),
                                      PRIORITY = pr.NAM_PRIO.ToLower(),
                                      // ASSISTANCE = asi.Client.ToLower(),
                                      EXPIRATION_TIME = ExpTime(i.ID_TICK),
                                      //COMPANY = cia.COMPANY,
                                      //COMPANY_FINAL = (ciaendx == null ? "-" : ciaendx.COMPANY),
                                      DATE_START_REAL = i.FEC_INI_REAL,
                                      DATE_END_REAL = (((from di in db.DETAILS_TICKET.OrderByDescending(di => di.ID_DETA_TICK)
                                                         where i.ID_TICK == di.ID_TICK && di.FEC_END_REAL != null
                                                         select di.FEC_END_REAL).FirstOrDefault())).ToString()
                                  }
                            ).Take(1000).ToList();
            //  var count = resultProblema.Count();
            var gridProblema = new GridView();
            gridProblema.DataSource = resultProblema;
            gridProblema.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=FindTicketProblema" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            gridProblema.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();

        }

        #endregion
        /*Fin*/

        /*Ticket Portal*/
        [Authorize]
        public ActionResult ListTicketPortal()
        {
            /*string UserName = Convert.ToString(Session["UserName"]);
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

            }*/

            //--------------Corregido
            if (Convert.ToInt32((Session["SERVICEDESK"].ToString())) == 1) //Vista reemplazada por Ticket/ListaTicketPortal
            {
                ViewBag.ACCESO_EDIT = 1;
            }
            if (Convert.ToInt32((Session["ADMINISTRADOR"].ToString())) == 1)
            {
                ViewBag.ACCESO_SEND_SURVEY = 1;
            }


            if (ViewBag.ACCESO_EDIT == 1 || ViewBag.ACCESO_SEND_SURVEY == 1)
            {
                return View();
            }
            else
            {
                return Content("No tiene permisos para ingresar");
            }


        }

        public ActionResult ListaTicketPortal()
        {
            ViewBag.ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (Convert.ToInt32(Session["SERVICEDESK"]) == 1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult ListByPortal(int id = 0)
        {
            textInfo = cultureInfo.TextInfo;
            //ctd = 1;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            int Count = 0;

            var tick = db.TICKETs.Where(t => t.UserId == null && t.ID_DOCU_SALE == null && t.ID_ENTI_ASSI == null && t.ID_STAT_END == null && t.ID_PRIO == null);
            Count = tick.Count();
            tick = tick.OrderByDescending(t => t.ID_TICK);

            if (ID_ACCO == 52)
            {
                tick = tick.Where(x => x.ID_ACCO == ID_ACCO);
            }
            //var o = 1;
            var result = (from i in tick.ToList()
                          join pe in dbe.PERSON_ENTITY on i.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          join ae in dbe.ACCOUNT_ENTITY on i.ID_PERS_ENTI_ASSI equals ae.ID_PERS_ENTI into aei
                          from xeo in aei.DefaultIfEmpty()
                          join pe1 in dbe.PERSON_ENTITY on (xeo == null ? 0 : xeo.ID_PERS_ENTI) equals pe1.ID_PERS_ENTI into pee
                          from peo in pee.DefaultIfEmpty()
                          join ce1 in dbe.CLASS_ENTITY on (peo == null ? 0 : peo.ID_ENTI2) equals ce1.ID_ENTI into cep
                          from cip in cep.DefaultIfEmpty()
                          join s in db.STATUS on (i.ID_STAT_END == null ? 0 : i.ID_STAT_END) equals s.ID_STAT into lds
                          from xds in lds.DefaultIfEmpty()
                          group i by new { i.ID_ACCO, i.ID_TICK, i.TITLE_TICK, i.CREATE_TICK, i.ID_STAT_END, i.COD_TICK, ce, cip, xds, i.SUM_TICK } into g

                          select new
                          {
                              //RowNumber = o++,
                              ID_ACCO = g.Key.ID_ACCO,
                              ID_TICK = g.Key.ID_TICK,
                              ID_TITLE = g.Key.TITLE_TICK,
                              ASIG = (g.Key.cip == null ? "Sin Asignar" : textInfo.ToTitleCase(g.Key.cip.FIR_NAME.ToLower()) + " " + textInfo.ToTitleCase(g.Key.cip.LAS_NAME.ToLower())),
                              STAT = (g.Key.xds == null ? "Nuevo" : g.Key.xds.NAM_STAT),
                              FCREAT = String.Format("{0:dd/MM/yyyy}", g.Key.CREATE_TICK) + " " + String.Format("{0:HH:mm:ss}", g.Key.CREATE_TICK),
                              IDSTAT = g.Key.ID_STAT_END,
                              //CREABY = (g.Key.ce.FIR_NAME == null ? g.Key.ce.LAS_NAME.ToLower() : g.Key.ce.FIR_NAME.ToLower() + " " + g.Key.ce.LAS_NAME == null ? "" : g.Key.ce.LAS_NAME.ToLower()),
                              CREABY = (g.Key.ce.FIR_NAME == null ? "" : textInfo.ToTitleCase(g.Key.ce.FIR_NAME.ToLower())) + " " + (g.Key.ce.LAS_NAME == null ? "" : textInfo.ToTitleCase(g.Key.ce.LAS_NAME.ToLower())),
                              COD_TICK = g.Key.COD_TICK,
                              SUM_TICK_PLAIN = StripHtml(g.Key.SUM_TICK),
                              SUM_INCI = g.Key.SUM_TICK,
                              URL = "/Ticket/AsignarTicketPortal/",

                          });

            //var grupo = lis.GroupBy(x => x.ID_TICK).ToList();
            var query = result.ToList();

            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTicketsPortal()
        {
            textInfo = cultureInfo.TextInfo;
            int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = db.ListarTicketsPortal(IdPersEnti).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult TicketPortalDetalle(int id = 0)
        {
            textInfo = cultureInfo.TextInfo;
            var tick = db.TICKETs.Where(t => t.ID_TICK == id);
            var result = (from i in tick.ToList()
                          join pe in dbe.PERSON_ENTITY on i.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          select new
                          {
                              //RowNumber = o++,
                              ID_TICK = i.ID_TICK,
                              ID_TITLE = i.TITLE_TICK,
                              CREABY = (ce.FIR_NAME == null ? "" : textInfo.ToTitleCase(ce.FIR_NAME.ToLower())) + " " + (ce.LAS_NAME == null ? "" : textInfo.ToTitleCase(ce.LAS_NAME.ToLower())),
                              SUM_TICK_PLAIN = (i.SUM_TICK)
                          });

            var deta = result.ToList();

            return Json(new { datadeta = deta }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TicketPortalDeta(int id = 0)
        {
            textInfo = cultureInfo.TextInfo;
            //ctd = 1;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //id = 311316;
            var tick = db.TICKETs.Where(t => t.ID_TICK == id);
            var result = (from i in tick.ToList()
                          join pe in dbe.PERSON_ENTITY on i.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          join ae in dbe.ACCOUNT_ENTITY on i.ID_PERS_ENTI_ASSI equals ae.ID_PERS_ENTI into aei
                          from xeo in aei.DefaultIfEmpty()
                          join pe1 in dbe.PERSON_ENTITY on (xeo == null ? 0 : xeo.ID_PERS_ENTI) equals pe1.ID_PERS_ENTI into pee
                          from peo in pee.DefaultIfEmpty()
                          join ce1 in dbe.CLASS_ENTITY on (peo == null ? 0 : peo.ID_ENTI2) equals ce1.ID_ENTI into cep
                          from cip in cep.DefaultIfEmpty()
                          join s in db.STATUS on (i.ID_STAT_END == null ? 0 : i.ID_STAT_END) equals s.ID_STAT into lds
                          from xds in lds.DefaultIfEmpty()
                          group i by new { i.ID_TICK, i.TITLE_TICK, i.CREATE_TICK, i.ID_STAT_END, i.COD_TICK, ce, cip, xds, i.SUM_TICK } into g

                          select new
                          {
                              //RowNumber = o++,
                              ID_TICK = g.Key.ID_TICK,
                              ID_TITLE = g.Key.TITLE_TICK,
                              ASIG = (g.Key.cip == null ? "Sin Asignar" : textInfo.ToTitleCase(g.Key.cip.FIR_NAME.ToLower()) + " " + textInfo.ToTitleCase(g.Key.cip.LAS_NAME.ToLower())),
                              STAT = (g.Key.xds == null ? "Nuevo" : g.Key.xds.NAM_STAT),
                              FCREAT = String.Format("{0:dd/MM/yyyy}", g.Key.CREATE_TICK) + " " + String.Format("{0:HH:mm:ss}", g.Key.CREATE_TICK),
                              IDSTAT = g.Key.ID_STAT_END,
                              //CREABY = (g.Key.ce.FIR_NAME == null ? g.Key.ce.LAS_NAME.ToLower() : g.Key.ce.FIR_NAME.ToLower() + " " + g.Key.ce.LAS_NAME == null ? "" : g.Key.ce.LAS_NAME.ToLower()),
                              CREABY = (g.Key.ce.FIR_NAME == null ? "" : textInfo.ToTitleCase(g.Key.ce.FIR_NAME.ToLower())) + " " + (g.Key.ce.LAS_NAME == null ? "" : textInfo.ToTitleCase(g.Key.ce.LAS_NAME.ToLower())),
                              COD_TICK = g.Key.COD_TICK,
                              SUM_TICK_PLAIN = StripHtml(g.Key.SUM_TICK),
                              SUM_INCI = g.Key.SUM_TICK,
                              URL = "/Ticket/AsignarTicketPortal/",

                          });


            var deta = result.ToList();

            return Json(new { datadeta = deta }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AsignarTicketPortal(int id = 0)
        {
            try
            {
                ViewBag.ID_TICK = id;
                TICKET query = db.TICKETs.Single(x => x.ID_TICK == id);
                ViewBag.IdCuenta = query.ID_ACCO;
                ViewBag.SOURCE = query.ID_SOUR;
                if (query.ID_CATE != null && query.ID_CATE != 0)
                    ViewBag.FlagCategoria = 1;
                else
                    ViewBag.FlagCategoria = 0;
                return View();
                //    }
            }
            catch
            {
                return Content("Please Close Session");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarTicketPortal(TICKET ticket, int id = 0)
        {
            try
            {
                int flagCategoria = Convert.ToInt32(Request.Params["FlagCategoria"]);
                var finic = DateTime.Now;//
                int hours = 0;//
                TICKET query = db.TICKETs.Single(x => x.ID_TICK == id);
                query.ID_ACCO = query.ID_ACCO;
                query.ID_TYPE_TICK = ticket.ID_TYPE_TICK;
                query.ID_SOUR = query.ID_SOUR;
                query.ID_COMP = query.ID_COMP;
                query.ID_COMP_END = ticket.ID_COMP_END;
                if (query.ID_SOUR == 7)
                {
                    query.ID_PERS_ENTI = ticket.ID_PERS_ENTI;
                    query.ID_PERS_ENTI_END = ticket.ID_PERS_ENTI;
                }
                else
                {
                    query.ID_PERS_ENTI = query.ID_PERS_ENTI;
                    query.ID_PERS_ENTI_END = query.ID_PERS_ENTI_END;
                }
                query.ID_PRIO = ticket.ID_PRIO;
                query.ID_QUEU = ticket.ID_QUEU;
                query.ID_PERS_ENTI_ASSI = ticket.ID_PERS_ENTI_ASSI;
                query.ID_STAT = 1;
                query.IS_PARENT = true;
                query.ID_STAT_END = 1;
                if (flagCategoria == 0)
                    query.ID_CATE = ticket.ID_CATE;
                query.ID_DETA_DOCU_SALE = ticket.ID_DETA_DOCU_SALE;
                query.FEC_INI_TICK = finic;//
                int id_prio = Convert.ToInt32(ticket.ID_PRIO);//
                int idSla = Convert.ToInt32(db.SLAs.Single(x => x.IdCuenta == query.ID_ACCO && x.Nombre == "SLA ESTANDAR").Id);
                hours = Convert.ToInt32(db.SLADetalles.Single(x => x.IdPrioridad == id_prio && x.IdSLA == idSla).TiempoAtencion);//
                query.DAT_EXPI_TICK = finic.AddHours(hours);//
                query.MODIFIED_TICK = DateTime.Now;//

                query.IdSLA = db.SLAs.Where(x => x.IdCuenta == query.ID_ACCO).FirstOrDefault().Id;

                db.Entry(query).State = EntityState.Modified;
                //db.TICKETs.Attach(query);
                db.SaveChanges();
                //Categoria: ALTA DE USUARIO, para MINSUR/ MARCOBRE / RAURA se generarán 19 tickets hijos
                if (query.ID_CATE == 27009 || query.ID_CATE == 28380 || query.ID_CATE == 29751)// CATEGORIA DE ALTA
                {
                    var listaTareaBaja = db.Tarea.Where(x => x.TipoGestion == "ALTA").ToList();

                    foreach (Tarea tarea in listaTareaBaja)
                    {
                        TareaDetalle objTareaDetalle = new TareaDetalle();

                        objTareaDetalle.Id_Tick = id;
                        objTareaDetalle.IdTarea = tarea.IdTarea;
                        objTareaDetalle.Id_Queu = tarea.Id_Queu;
                        objTareaDetalle.FechaCreacion = DateTime.Now;
                        objTareaDetalle.IdEstado = 4;
                        db.TareaDetalle.Add(objTareaDetalle);
                        db.SaveChanges();
                    }
                }
                if (query.ID_CATE == 27011 || query.ID_CATE == 28382 || query.ID_CATE == 29753) /* CATEGORIA CESE DE CUENTA / MINSUR MARCOBRE RAURA */
                {

                    var listaTareaBaja = db.Tarea.Where(x => x.TipoGestion == "BAJA").ToList();

                    foreach (Tarea tarea in listaTareaBaja)
                    {
                        TareaDetalle objTareaDetalle = new TareaDetalle();

                        objTareaDetalle.Id_Tick = id;
                        objTareaDetalle.IdTarea = tarea.IdTarea;
                        objTareaDetalle.Id_Queu = tarea.Id_Queu;
                        objTareaDetalle.FechaCreacion = DateTime.Now;
                        objTareaDetalle.IdEstado = 4;
                        db.TareaDetalle.Add(objTareaDetalle);
                        db.SaveChanges();
                    }
                }

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('OK','La información ha sido actualizada.');}window.onload=init;</script>");


            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneEdit) top.uploadDoneEdit('ERROR','Falta completar información');}window.onload=init;</script>");
            }

        }


        public ActionResult FindFormatResult()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            int Count = 0, UserId, ID_QUEU, ID_PERS_ENTI_ASSI, ID_PRIO, ID_SOUR, ID_PERS_ENTI, ID_PERS_ENTI_END,
                CountClose, ID_TYPE_FORM;

            DateTime START_DATE, END_DATE;

            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                    SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]),
                    from = "", to = "";

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_FORM"]), out ID_TYPE_FORM) == false)
            {
                ID_TYPE_FORM = 1;
            }

            var listClient = (from lu in dbe.ACCOUNT_ENTITY
                              join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                              join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                              where lu.ID_ACCO == ID_ACCO
                              select new
                              {
                                  lu.ID_PERS_ENTI,
                                  Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                  ce.UserId,
                              }).ToList();

            var tick = (from t in db.TICKETs
                        where t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == ID_TYPE_FORM
                        select new
                        {
                            t.ID_ACCO,
                            t.FEC_TICK,
                            t.COD_TICK,
                            t.UserId,
                            t.ID_QUEU,
                            t.ID_PERS_ENTI_ASSI,
                            t.ID_TYPE_TICK,
                            t.ID_PRIO,
                            t.ID_SOUR,
                            t.ID_STAT_END,
                            t.SUM_TICK,
                            t.ID_PERS_ENTI,
                            t.ID_PERS_ENTI_END,
                            t.ID_CATE,
                            t.CREATE_TICK,
                            t.MODIFIED_TICK,
                            t.ID_TICK,
                            t.IS_PARENT
                        });

            var schbjsd = tick.Count();

            //SEIS ÚLTIMOS MESES DEL TICKET DE LA CUENTA
            var legendx = (from t in tick
                           group t by new { t.FEC_TICK.Value.Year, t.FEC_TICK.Value.Month } into g
                           select new
                           {
                               g.Key.Year,
                               g.Key.Month,
                               Count = g.Count(),
                           }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Take(5);

            var legend = (from t in legendx
                          join am in db.ACCOUNTING_MONTH on t.Year equals am.ID_ACCO_YEAR
                          where t.Month == am.ACCO_MONT
                          select new
                          {
                              t.Year,
                              t.Month,
                              t.Count,
                              name = am.NAM_ACCO_MONT.Substring(0, 1) + am.NAM_ACCO_MONT.Substring(1, 2).ToLower()
                          });

            if (!String.IsNullOrEmpty(COD_TICK))
            {
                tick = tick.Where(t => t.COD_TICK.ToUpper().Contains(COD_TICK.ToUpper()));
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["UserId"]), out UserId) == true)
            {
                tick = tick.Where(t => t.UserId == UserId);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == true)
            {
                tick = tick.Where(t => t.ID_QUEU == ID_QUEU);
            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == true)
                {
                    tick = tick.Where(t => t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
                }
            }
            catch { }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PRIO"]), out ID_PRIO) == true)
            {
                tick = tick.Where(t => t.ID_PRIO == ID_PRIO);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_SOUR"]), out ID_SOUR) == true)
            {
                tick = tick.Where(t => t.ID_SOUR == ID_SOUR);
            }

            if (!String.IsNullOrEmpty(SUM_TICK))
            {
                tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(SUM_TICK.ToUpper()));
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI == ID_PERS_ENTI);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_END"]), out ID_PERS_ENTI_END) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI_END == ID_PERS_ENTI_END);
            }

            var lineClose = (from x in legend
                             join t in tick on x.Year equals t.FEC_TICK.Value.Year
                             where x.Month == t.FEC_TICK.Value.Month && t.ID_STAT_END == 6
                             group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                             select new
                             {
                                 g.Key.ID_STAT_END,
                                 g.Key.Year,
                                 g.Key.Month,
                                 g.Key.Count,
                                 g.Key.name,
                                 RES_COUN = g.Count()

                             }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineOpen = (from x in legend
                            join t in tick on x.Year equals t.FEC_TICK.Value.Year
                            where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)
                            group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                            select new
                            {
                                g.Key.ID_STAT_END,
                                g.Key.Year,
                                g.Key.Month,
                                g.Key.Count,
                                g.Key.name,
                                RES_COUN = g.Count()

                            }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            var lineScheduled = (from x in legend
                                 join t in tick on x.Year equals t.FEC_TICK.Value.Year
                                 where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 5)
                                 group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
                                 select new
                                 {
                                     g.Key.ID_STAT_END,
                                     g.Key.Year,
                                     g.Key.Month,
                                     g.Key.Count,
                                     g.Key.name,
                                     RES_COUN = g.Count()

                                 }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);


            if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE"]), out START_DATE))
            {
                tick = tick.Where(t => t.FEC_TICK >= START_DATE);
            }

            if (DateTime.TryParse(Convert.ToString(Request.Params["END_DATE"]), out END_DATE))
            {
                END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                tick = tick.Where(t => t.FEC_TICK <= END_DATE);
            }

            Count = tick.Count();
            CountClose = tick.Where(t => t.ID_STAT_END == 6).Count();
            try
            {
                from = String.Format("{0:d}", tick.OrderBy(t => t.FEC_TICK).First().FEC_TICK);
                to = String.Format("{0:d}", tick.OrderByDescending(t => t.FEC_TICK).First().FEC_TICK);
            }
            catch
            {

            }

            var resultPie = (from t in tick
                             join st in db.STATUS on t.ID_STAT_END equals st.ID_STAT
                             where t.ID_STAT_END != 6 && t.ID_STAT_END != 2
                             group st by new { st.NAM_GRAP_STAT, st.COL_GRAP_STAT } into g
                             select new
                             {
                                 name = g.Key.NAM_GRAP_STAT.Substring(0, 1) + g.Key.NAM_GRAP_STAT.Substring(1, g.Key.NAM_GRAP_STAT.Length).ToLower(),
                                 color = g.Key.COL_GRAP_STAT,
                                 y = g.Count()
                             }).ToList();

            tick = tick.OrderByDescending(t => t.FEC_TICK).ThenByDescending(t => t.ID_STAT_END).Skip(skip).Take(take);

            var result = (from i in tick.ToList()
                          join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listClient on i.UserId equals cr.UserId
                          join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listClient on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join re in listClient on i.ID_PERS_ENTI_END equals re.ID_PERS_ENTI
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBC = c4.NAM_CATE.ToLower(),
                              NAM_CATE = c3.NAM_CATE.ToLower(),
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              REQU = re.Client.ToLower(),
                              CREATE_INCI = String.Format("{0:G}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:G}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client.ToLower(),
                              ACCO = ac.NAM_ACCO,
                              ASSI = asi.Client.ToLower(),
                              EXP_TIME = ExpTime(i.ID_TICK),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none")
                          });


            return Json(new { Data = result, Count = Count, CountClose = CountClose, From = from, To = to, Pie = resultPie, legend = legend, lineClose = lineClose, lineScheduled = lineScheduled, lineOpen = lineOpen }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateBarStatus(int id = 0)
        {
            var stados = db.STATUS;

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

            //var cant = db.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_STAT_END == id && i.ID_TICK_PARENT == null);
            var cant = db.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_STAT_END == id && i.ID_DOCU_SALE == null);

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
                    var Queus = db.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
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

            return cant.Count();
        }

        public int TotTicketsByStatusx(int id, int tipoTicket)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            //var cant = db.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_STAT_END == id && i.ID_TICK_PARENT == null);
            var cant = db.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_STAT_END == id && i.ID_DOCU_SALE == null);

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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult viewListTicketSon(int id = 0)
        {
            ViewBag.ID_TICK = id;
            return View();
        }

        public ActionResult ListTicketsSon(int id = 0)
        {
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

            var tick = db.TICKETs.Where(t => t.ID_TICK_PARENT == id);
            int Count = 0;

            Count = tick.Count();

            var result = (from i in tick.ToList()/*.Skip(skip).Take(take)*/
                          join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listuser on i.UserId equals cr.UserId
                          join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listuser on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBC = c4.NAM_CATE.ToLower(),
                              NAM_CATE = c3.NAM_CATE.ToLower(),
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                              REQU = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                              CREATE_INCI = String.Format("{0:G}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:G}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client.ToLower(),
                              ACCO = ac.NAM_ACCO,
                              ASSI = asi.Client.ToLower(),
                              EXP_TIME = ExpTime(i.ID_TICK),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none"),
                              TTSON = Count,
                              i.ID_TICK_PARENT,
                          });

            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListTicketsActive()
        {
            var query = db.ListTicketsActive().ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TicketsNotClosedByPerson(int id = 0)
        {
            var query = db.TICKETs
                .Where(x => x.ID_PERS_ENTI_ASSI == id)
                //.Where(x=>x.FEC_TICK >= DateTime.Now.AddMonths(-1))
                .Where(x => x.ID_STAT_END != 6 && x.ID_STAT_END != 2).ToList();

            var cia = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1).ToList();

            var result = (from x in query.OrderBy(x => x.ID_TICK)
                          join pe in cia on x.ID_COMP equals pe.ID_ENTI into lcia
                          from xpe in lcia.DefaultIfEmpty()

                          select new
                          {
                              ID_TICK = x.ID_TICK,
                              COD_TICK = x.COD_TICK,
                              CIA = xpe != null ? xpe.COM_NAME : String.Empty
                          });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendSurveyTicket()
        {
            try
            {
                int ID_TICK = Convert.ToInt32(Request.Params["ID_TICK"]);

                TICKET t = db.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                if (t.SEND_SURVEY == null)
                {
                    t.SEND_SURVEY = false;

                    db.TICKETs.Attach(t);
                    db.Entry(t).State = EntityState.Modified;
                    db.SaveChanges();

                    return Content("OK");
                }
                else
                {
                    return Content("EXISTE");
                }
            }
            catch
            {
                return Content("ERROR");
            }

        }

        public ActionResult ReporteActividadTicket()
        {
            return View();
        }

        public ActionResult TicketAutomatico()
        {
            return View();
        }

        public ActionResult ListarTicketAutomatico()
        {
            int Cuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int Frecuencia = Convert.ToInt32(Request.Params["Frecuencia"]);
            int Vigencia = Convert.ToInt32(Request.Params["Vigencia"]);
            string PalabraClave = Request.Params["PalabraClave"].ToString();
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            List<tktAutomaticoListar_Result> query = db.tktAutomaticoListar(Frecuencia, Vigencia, PalabraClave, Cuenta).ToList();

            var total = query.Count();

            var query2 = query.Skip(skip).Take(take);

            return Json(new { Data = query2, Cantidad = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteTotales()
        {
            return View();
        }
        public ActionResult ReportesPlanos()
        {
            return View();
        }
        public ActionResult ReportePlanoCategoria()
        {
            try
            {
                int anio = 0, mes = 0;
                anio = DateTime.Now.Year;
                mes = DateTime.Now.Month;

                var result = db.ACCOUNTING_MONTH.Where(x => x.ID_ACCO_YEAR == anio && x.ACCO_MONT == mes).SingleOrDefault();
                ViewBag.Año = anio;
                ViewBag.Mes = mes;
                ViewBag.NombreMes = result.NAM_ACCO_MONT;
            }
            catch
            {
                ViewBag.Año = DateTime.Now.Year;
                ViewBag.Mes = "1";
                ViewBag.NombreMes = "ENERO";
            }


            return View();
        }

        [Authorize]
        public ActionResult ReporteTicketGestionDeCambio()
        {

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.Anio = DateTime.Now.Year;
            ViewBag.Mes = DateTime.Now.Month;
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.FechaInicio = String.Format("{0:MM/dd/yyyy}", fecha);
            ViewBag.FechaActual = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
            ViewBag.IdCuenta = IdCuenta;

            return View();
        }
        public JsonResult ListarAños(string q, string page)
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
            var result = (from y in db.ACCOUNTING_YEAR.ToList()
                          select new
                          {
                              id = y.ID_ACCO_YEAR,
                              text = Convert.ToString(y.ID_ACCO_YEAR)
                          }).Where(x => x.text.Contains(termino));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAñosCbx(string q, string page)
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
            var result = (from y in db.ACCOUNTING_YEAR.ToList()
                          select new
                          {
                              id = y.ID_ACCO_YEAR,
                              text = Convert.ToString(y.ID_ACCO_YEAR)
                          }).Where(x => x.text.Contains(termino));

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarMeses(string q, string page)
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
            var result = (from m in db.ACCOUNTING_MONTH.Where(y => y.ID_ACCO_YEAR == DateTime.Now.Year)
                          select new
                          {
                              id = m.ACCO_MONT,
                              text = m.NAM_ACCO_MONT
                          }).Where(x => x.text.Contains(termino));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarMesesCbx(string q, string page)
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
            var result = (from m in db.ACCOUNTING_MONTH.Where(y => y.ID_ACCO_YEAR == DateTime.Now.Year)
                          select new
                          {
                              id = m.ACCO_MONT,
                              text = m.NAM_ACCO_MONT
                          }).Where(x => x.text.Contains(termino));

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarReportePlanoCategoria(int id = 0, int id1 = 0)
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var result = db.tktReportePlanoCategoria(id, id1, IdCuenta).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GestionCambioListarTicketsCerrados(int id = 0, int id1 = 0, int id2 = 0)
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
            DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
            var result = db.GestionCambioListarTicketsCerrados(IdCuenta, id, id1, FechaInicio, FechaFin, id2).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCalendarioTicketsProgramados(int id = 0)
        {
            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            var result = db.tktCalendarioTicketsProgramados(id, IdCuenta).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarTipoPase()
        {
            var result = db.ListarTipoPase().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarModalidadTrabajo()
        {
            var result = db.ListarModalidadTrabajo().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarGrupoReportador()
        {
            var result = db.ListarGrupoReportador().ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult RegistrarHoraReal(int ID_TICK)
        {
            TICKET query = db.TICKETs.Single(x => x.ID_TICK == ID_TICK);
            DateTime fecha = DateTime.Now;

            query.FechaInicioReal = DateTime.Now;
            db.Entry(query).State = EntityState.Modified;
            db.SaveChanges();


            var msn = "OK";


            return Content(query.FechaInicioReal.ToString());
            //return Content("<script type='text/javascript'> function init() {" +
            //"if(top.Mensaje) top.Mensaje('OK','Se registro Hora Real de Atención.');}window.onload=init;</script>");

        }

        public ActionResult ValidarFechaReal(int ID_TICK)
        {
            TICKET query = db.TICKETs.Single(x => x.ID_TICK == ID_TICK);
            var Fecha = query.FechaInicioReal;
            string FechaValida = Convert.ToString(Fecha);

            var msn = "";

            if (FechaValida == "" || FechaValida == null)
            {
                msn = "FALSE";
            }
            else
            {
                msn = "OK";
            }

            return Content(msn);

        }

        public ActionResult ObtenerSolucion(int ID_TICK)
        {
            TICKET query = db.TICKETs.Single(x => x.ID_TICK == ID_TICK);
            string solucion = query.Solucion;

            return Content(solucion);

        }

        #region "Edicion de Tickets automaticos"
        public ActionResult EditarTicketAutomatico(int id)
        {
            ViewBag.IdTicketAutomatico = id;
            return View();
        }
        public ActionResult ObtenerDias(int id = 0)
        {
            var query = db.ObtenerDias(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerDatos(int id = 0)
        {
            var query = db.ObtenerDatos(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditarTktAutomatico(int i = 0)
        {

            //Variables
            int idTicketAutomatico = Convert.ToInt32(Request.Params["IdTicketAutomatico"]);
            string titulo = Request.Params["Titulo"];
            Boolean estado = Convert.ToBoolean(Convert.ToInt32(Request.Params["Estado"]));
            int idUser = Convert.ToInt32(Session["UserId"].ToString());
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            string horaCreacion = Request.Params["HoraCreacion"];

            int frecuencia = Convert.ToInt32(Request.Params["Fre"]);
            string diasSemana = Request.Params["DiasSemana"];

            int vigencia = Convert.ToInt32(Request.Params["Vigencia"]);
            string vigenciaFechaInicio = "";
            string nuevaVigenciaFechaInicio = "";
            string vigenciaFechaFin = "";
            string nuevaVigenciaFechaFin = "";

            //Vigencia --> Indeterminado : 1 / Fechas : 2
            if (vigencia == 2)
            {
                vigenciaFechaInicio = Request.Params["VigenciaFechaInicio"];

                vigenciaFechaFin = Request.Params["VigenciaFechaFin"];
            }

            var actualiza = db.Editar(idTicketAutomatico, titulo, diasSemana, frecuencia, vigencia, estado, idUser, vigenciaFechaInicio, vigenciaFechaFin, horaCreacion, IdAcco).SingleOrDefault();
            return Content(actualiza.Mensaje);
        }
        #endregion

        #region "Consolidado de Tickets"
        public ActionResult ConsolidadoTickets()
        {
            return View();
        }
        #endregion

        #region "Autoservicio AD - desbloqueo/reseteo cuenta de red"
        public ActionResult AutoservicioAD()
        {
            return View();
        }

        public ActionResult ListarAutoservicioAD()
        {
            string fechaInicio = Convert.ToString(Request.Params["fI"]);
            string fechaFin = Convert.ToString(Request.Params["fF"]);
            var result = db.ListarAutoservicio(fechaInicio, fechaFin).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConteoAutoservicioAD()
        {
            string fechaInicio = Convert.ToString(Request.Params["fI"]);
            string fechaFin = Convert.ToString(Request.Params["fF"]);
            string tipo = Convert.ToString(Request.Params["tipo"]);
            var result = db.ConteoAutoservicio(fechaInicio, fechaFin, tipo).ToList();
            return Content(result[0].Conteo.ToString());
        }
        #endregion

        public ActionResult ListaTipoTicket()
        {

            var result = (from tt in db.TYPE_TICKET
                          select new
                          {
                              ID_TYPE_TICK = tt.ID_TYPE_TICK,
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK,
                          }).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ReporteTicketsDeActivos()
        {
            int id_acco = Convert.ToInt32(Session["ID_ACCO"]);

            if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 0)
                return RedirectToAction("Index", "Error");

            ViewBag.ID_ACCO = id_acco;
            return View();
        }

        public ActionResult ListaEstadoTicketActivo()
        {

            var result = db.ListarEstadoTicketActivo().ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }


        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult DescargarExcelTicket(int estado)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                int IdAcco = Convert.ToInt32(Session["ID_ACCO"].ToString());
                var query = db.DescargarExcelTicket(IdAcco, estado).ToList();
                var selectedData = query.Select(item => new
                {
                    Ticket = item.Ticket,
                    UsuarioFinal = item.UsuarioFinal,
                    Area = item.Area,
                    FechaInicio = item.FechaInicio,
                    Sede = item.Sede,
                    Locacion = item.Locacion,
                    Estado = item.Estado,
                    ADJUNTODETICKET = item.ADJUNTODETICKET,
                    ENTREGA_RECEPCION = item.ENTREGA_RECEPCION,
                }).ToList();

                string dato = "";

                switch (estado)
                {
                    case 1:
                        {
                            dato = "ACTIVO";
                            break;
                        }
                    case 2:
                        {
                            dato = "CANCELADO";
                            break;
                        }
                    case 6:
                        {
                            dato = "CERRADO";
                            break;
                        }
                    case 7:
                        {
                            dato = "NUEVO";
                            break;
                        }
                    case 5:
                        {
                            dato = "PROGRAMADO";
                            break;
                        }
                    case 4:
                        {
                            dato = "RESUELTO";
                            break;
                        }
                }

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("<meta charset='utf-8'>");
                stringBuilder.AppendLine("<style type='text/css'>");
                stringBuilder.AppendLine(".cabecera {background: #2a4389;color: white;font-family: Arial, sans-serif;font-size: 12px;font-weight: normal;border-width: thin;padding: 10px;border: solid;overflow: hidden;word-break: normal;border-color: #b6bbc3;}");
                stringBuilder.AppendLine(".fecha {background: #b6bbc3;color: #000000;font-size: 14px;font-weight: bold;}");
                stringBuilder.AppendLine(".contenido {font-family: Arial, sans-serif;font-size: 14px;padding: 10px;border-style: solid;overflow: hidden;border-width: thin;word-break: normal;border-color: #b6bbc3;}");

                stringBuilder.AppendLine("</style>");

                stringBuilder.AppendLine("<table>");


                stringBuilder.AppendLine("<tr>");
                stringBuilder.AppendLine($"<td class='cabecera' colspan='3'>REPORTE DE TICKETS DE {dato}</td>");
                stringBuilder.AppendLine("</tr>");


                stringBuilder.AppendLine("<tr></tr>");


                stringBuilder.AppendLine("<tr>");
                stringBuilder.AppendLine("<td class='cabecera'>FECHA:</td>");
                stringBuilder.AppendLine($"<td class='fecha' colspan='1'>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}</td>");
                stringBuilder.AppendLine($"<td class='fecha'></td>");
                stringBuilder.AppendLine("</tr>");


                stringBuilder.AppendLine("<tr></tr>");


                stringBuilder.AppendLine("<tr>");
                stringBuilder.AppendLine("<th class='cabecera'>Ticket</th>");
                stringBuilder.AppendLine("<th class='cabecera'>Usuario Final</th>");
                stringBuilder.AppendLine("<th class='cabecera'>Área</th>");
                stringBuilder.AppendLine("<th class='cabecera'>FechaInicio</th>");
                stringBuilder.AppendLine("<th class='cabecera'>Sede</th>");
                stringBuilder.AppendLine("<th class='cabecera'>Locacion</th>");
                stringBuilder.AppendLine("<th class='cabecera'>Estado</th>");
                stringBuilder.AppendLine("<th class='cabecera'>ADJUNTODETICKET</th>");
                stringBuilder.AppendLine("<th class='cabecera'>ENTREGA_RECEPCION</th>");
                stringBuilder.AppendLine("</tr>");



                foreach (var item in selectedData)
                {

                    if (item != null)
                    {

                        if (!string.IsNullOrEmpty(item.ADJUNTODETICKET))
                        {

                            string[] adjuntoValues = item.ADJUNTODETICKET.Split(';');


                            if (adjuntoValues.Length >= 3)
                            {
                                string valor1 = adjuntoValues[0].Trim();
                                string valor2 = adjuntoValues[1].Trim();
                                string valor3 = adjuntoValues[2].Trim();


                                string url = $"https://itms.electrodata.com.pe/Adjuntos/Delivery/{valor3}/{valor1}_{valor2}.pdf";


                                //string nombreContenido = adjuntoValues[0].Trim().Split(' ')[0].ToUpper();


                                stringBuilder.AppendLine("<tr>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Ticket}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.UsuarioFinal}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Area}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.FechaInicio}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Sede}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Locacion}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Estado}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'><a href='{url}' target='_blank'>{valor1.ToUpper()}</a></td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.ENTREGA_RECEPCION}</td>");
                                stringBuilder.AppendLine("</tr>");
                            }
                            else
                            {

                                stringBuilder.AppendLine("<tr>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Ticket}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.UsuarioFinal}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Area}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.FechaInicio}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Sede}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Locacion}</td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.Estado}</td>");
                                stringBuilder.AppendLine("<td class='contenido'></td>");
                                stringBuilder.AppendLine($"<td class='contenido'>{item.ENTREGA_RECEPCION}</td>");
                                stringBuilder.AppendLine("</tr>");
                            }
                        }
                        else
                        {

                            stringBuilder.AppendLine("<tr>");
                            stringBuilder.AppendLine($"<td class='contenido'>{item.Ticket}</td>");
                            stringBuilder.AppendLine($"<td class='contenido'>{item.UsuarioFinal}</td>");
                            stringBuilder.AppendLine($"<td class='contenido'>{item.Area}</td>");
                            stringBuilder.AppendLine($"<td class='contenido'>{item.FechaInicio}</td>");
                            stringBuilder.AppendLine($"<td class='contenido'>{item.Sede}</td>");
                            stringBuilder.AppendLine($"<td class='contenido'>{item.Locacion}</td>");
                            stringBuilder.AppendLine($"<td class='contenido'>{item.Estado}</td>");
                            stringBuilder.AppendLine("<td class='contenido'></td>");
                            stringBuilder.AppendLine($"<td class='contenido'>{item.ENTREGA_RECEPCION}</td>");
                            stringBuilder.AppendLine("</tr>");
                        }
                    }
                    else
                    {
                        Console.WriteLine("El elemento 'item' es null");
                    }
                }

                stringBuilder.AppendLine("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", $"attachment;filename=ExcelTicket_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls");
                Response.Write(stringBuilder.ToString());
                Response.Flush();
                Response.End();

                return View();
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message, "text/plain");
            }
        }




        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult ListarTicketPorCuenta(int estado, int draw, int start, int length)
        {
            try
            {

                if (Session["ID_ACCO"] == null || !int.TryParse(Session["ID_ACCO"].ToString(), out int IdAcco))
                {
                    return Content("Error: ID_ACCO no válido en la sesión.", "text/plain");
                }


                int pageNumber = (start / length) + 1;


                var totalRecordsResult = db.TicketDeActivoPorCuentaAct(IdAcco, estado, 0, 0, 1).FirstOrDefault();
                int totalRecords = totalRecordsResult != null ? Convert.ToInt32(totalRecordsResult.TotalRecords) : 0;


                var query = db.TicketDeActivoPorCuentaAct(IdAcco, estado, length, pageNumber, 0)?.ToList() ?? new List<TicketDeActivoPorCuentaAct_Result>();

                var result = new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = query
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message, "text/plain");
            }
        }



        public ActionResult ReporteCategorias()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.IDACCO = IdAcco;
            return View();

        }
        public ActionResult ReporteRoles()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.IDACCO = IdAcco;
            return View();

        }
        public ActionResult ReporteGrupos()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.IDACCO = IdAcco;
            return View();
        }

        [Authorize]
        public ActionResult ReporteTicketPorTipo()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.IDACCO = IdAcco;
            return View();
        }

        public int ObtenerTipoIncidenteCategoria(int id = 0)
        {
            try
            {
                var tipoIncidente = db.ListaTipoTicketxIdCate(id).First();
                return Convert.ToInt32(tipoIncidente.ID_TYPE_TICK);
            }
            catch (Exception e)
            {
                return 0;
            }


        }
        public JsonResult ObtenerSLACategoria(int id = 0)
        {
            try
            {
                var categoria = db.ListaSLATicketxIdCate(id).FirstOrDefault();
                return Json(new { Data = categoria }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public int ObtenerSLAId()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var id = db.SLAs.Single(x => x.IdCuenta == ID_ACCO && x.Nombre == "SLA ESTANDAR").Id;
                return id;
            }
            catch (Exception)
            {
                return 0;
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

        //        public ActionResult ListaUltimosTickets(int id = 0)
        //        {
        //            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
        //            int take = Convert.ToInt32(Request.Params["take"].ToString());
        //            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

        //            var query = (from t in db.TICKETs
        //                         //join dt in db.DETAILS_TICKET on t.ID_TICK equals dt.ID_DETA_TICK
        //                         join s in db.STATUS_TICKET on t.ID_STAT_END equals s.ID_STAT_TICK
        //                         where t.ID_PERS_ENTI_END == id && t.ID_ACCO == ID_ACCO
        //                         orderby t.FEC_TICK descending
        //                         select new
        //                         {
        //                             t.ID_TICK,
        //                             COD_TICK = t.COD_TICK,
        //                             TITLE_TICK = (t.TITLE_TICK == null ? "Sin título" : t.TITLE_TICK.ToLower()),
        //                             NAM_STAT_TICK = s.NAM_STAT_TICK.ToLower(),
        //                             FEC_TICK = t.FEC_TICK,
        //t                         });
        //            var result = query.Take(take).ToList();            

        //            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        //        }

        public ActionResult ListaUltimosTickets(int id = 0,string filter = null)
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (filter == null)
            {
                filter = "";
            }

            //var query = (ID_ACCO == 60)
            //            ? (from t in db.TICKETs
            //               join s in db.STATUS_TICKET on t.ID_STAT_END equals s.ID_STAT_TICK
            //               where t.ID_PERS_ENTI_END == id && t.ID_ACCO == ID_ACCO && t.ID_TYPE_FORM == null && t.COD_TICK.StartsWith(filter)
            //               orderby t.FEC_TICK descending
            //               select new
            //               {
            //                   t.ID_TICK,
            //                   COD_TICK = t.COD_TICK,
            //                   TITLE_TICK = (t.TITLE_TICK == null ? "Sin título" : t.TITLE_TICK.ToLower()),
            //                   NAM_STAT_TICK = s.NAM_STAT_TICK.ToLower(),
            //                   FEC_TICK = t.FEC_TICK
            //               })
            //               .Skip(skip)
            //               .Take(take)
            //               : (from t in db.TICKETs
            //                  join s in db.STATUS_TICKET on t.ID_STAT_END equals s.ID_STAT_TICK
            //                  where t.ID_PERS_ENTI_END == id && t.ID_ACCO == ID_ACCO
            //                  orderby t.FEC_TICK descending
            //                  select new
            //                  {
            //                      t.ID_TICK,
            //                      COD_TICK = t.COD_TICK,
            //                      TITLE_TICK = (t.TITLE_TICK == null ? "Sin título" : t.TITLE_TICK.ToLower()),
            //                      NAM_STAT_TICK = s.NAM_STAT_TICK.ToLower(),
            //                      FEC_TICK = t.FEC_TICK
            //                  })
            //               .Skip(skip)
            //               .Take(take);


            //var result = query.ToList();

            var result = db.sp_ListarUltimosTickets(id, ID_ACCO, filter, skip, take).ToList();


            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditarTareaMovil(int idTarea, TareaMovil tarea)
        {
            try
            {
                var objTareaDetalle = db.TareaDetalle.FirstOrDefault(x => x.IdTareaDetalle == idTarea);

                objTareaDetalle.IdTarea = tarea.TipoServicio;
                objTareaDetalle.IdSedeImpu = tarea.SedeImputado;
                objTareaDetalle.IdMoneda = tarea.Moneda;
                objTareaDetalle.Monto = tarea.Monto;
                objTareaDetalle.CeCoImpu = tarea.CeCoImputado;
                objTareaDetalle.PartPresup = tarea.PartidaPresupuestal;
                objTareaDetalle.FechaEjecutada = DateTime.ParseExact(tarea.FechaEjecutada, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                db.SaveChanges();

                return Content("OK");
            }
            catch (Exception)
            {
                return Content("ERROR");
            }
        }

        [HttpPost]
        public ActionResult GuardarTareasMovil(List<EstadoTareaMovil> tareas)
        {
            try
            {
                foreach (var tarea in tareas)
                {
                    TareaDetalle objTareaDetalle = db.TareaDetalle.FirstOrDefault(x => x.IdTareaDetalle == tarea.IdTareaDetalle);

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
                        db.SaveChanges();
                    }
                }

                return Content("OK");
            }
            catch (Exception)
            {
                return Content("ERROR");
            }
        }

        [HttpPost]
        public ActionResult CerrarTicketMovil(int idTicket)
        {
            try
            {
                TICKET objTicket = db.TICKETs.Where(x => x.ID_TICK == idTicket).FirstOrDefault();

                objTicket.ID_STAT_END = 4;
                db.SaveChanges();
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
                db.DETAILS_TICKET.Add(objDetails);
                db.SaveChanges();

                return Content("OK");
            }
            catch (Exception)
            {
                return Content("ERROR");
            }
        }

        public JsonResult ListarTareaMovilSedes()
        {
            var result = db.TareaMovilSede.Select(t => new { Id = t.Id, Nombre = t.Nombre }).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarTareaMovilTipo()
        {
            var result = db.Tarea.Where(x => x.TipoGestion == "MOVIL" && x.Estado == true && x.CategoriaConTareas == null)
                            .Select(t => new { Id = t.IdTarea, Nombre = t.DescripcionTarea }).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewEditarTareaMovil(int id)
        {
            var tarea = db.TareaDetalle.FirstOrDefault(x => x.IdTareaDetalle == id);

            TareaMovil tareaMovil = new TareaMovil
            {
                SedeImputado = tarea.IdSedeImpu,
                Moneda = tarea.IdMoneda,
                Monto = tarea.Monto,
                CeCoImputado = tarea.CeCoImpu ?? string.Empty,
                PartidaPresupuestal = tarea.PartPresup ?? string.Empty,
                TipoServicio = tarea.IdTarea,
                FechaEjecutada = tarea.FechaEjecutada?.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) ?? ""
            };

            ViewBag.IdTareaDetalle = id;
            ViewBag.CodTareaDetalle = tarea.Cod_Tarea;
            return View(tareaMovil);
        }

        [HttpPost]
        public ActionResult CrearTareasMovil(int idTicket, List<TareaMovil> tareas)
        {
            try
            {
                foreach (var tarea in tareas)
                {
                    var fechaEje = DateTime.ParseExact(tarea.FechaEjecutada, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    db.CrearTareaMovil(idTicket, tarea.TipoServicio, tarea.SedeImputado, tarea.Moneda, tarea.Monto, tarea.CeCoImputado, tarea.PartidaPresupuestal, fechaEje, Convert.ToInt32(Session["UserId"]));
                }

                return Content("OK");
            }
            catch (Exception)
            {
                return Content("ERROR");
            }
        }

        public ActionResult viewImportarTareasMovil()
        {
            return View();
        }

        public ActionResult DescargarPlantillaExcel()
        {
            string rutaArchivo = Server.MapPath("~/Adjuntos/Plantilla Tareas Movil.xlsx");

            return File(rutaArchivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla Tareas Movil.xlsx");
        }

        [HttpPost]
        public ActionResult ImportarTareasMovil(HttpPostedFileBase excelFile)
        {
            var listTareas = new List<TareaMovil>();

            if (excelFile != null && excelFile.ContentLength > 0)
            {
                try
                {
                    using (var excelWorkbook = new XLWorkbook(excelFile.InputStream))
                    {
                        var excelWorksheet = excelWorkbook.Worksheet(1);

                        if (excelWorksheet.Name != "Tareas")
                        {
                            return Json(new { Data = listTareas, Msg = "Seleccione la plantilla" }, JsonRequestBehavior.AllowGet);
                        }

                        var rowCount = excelWorksheet.RowsUsed().Count();

                        for (int i = 2; i <= rowCount; i++)
                        {
                            var rowData = new TareaMovil()
                            {
                                TipoServicio = excelWorksheet.Cell(i, 1).Value != null ? GetTipoServicio(excelWorksheet.Cell(i, 1).Value.ToString()) : null,
                                SedeImputado = excelWorksheet.Cell(i, 2).Value != null ? GetSedeImputado(excelWorksheet.Cell(i, 2).Value.ToString()) : null,
                                Moneda = excelWorksheet.Cell(i, 3).Value != null ? GetMoneda(excelWorksheet.Cell(i, 3).Value.ToString()) : null,
                                Monto = decimal.TryParse(excelWorksheet.Cell(i, 4).Value?.ToString().Trim(), out decimal montoDecimal) ? montoDecimal : (decimal?)null,
                                CeCoImputado = excelWorksheet.Cell(i, 5).Value != null ? excelWorksheet.Cell(i, 5).Value.ToString().Trim() : string.Empty,
                                PartidaPresupuestal = excelWorksheet.Cell(i, 6).Value != null ? excelWorksheet.Cell(i, 6).Value.ToString().Trim() : string.Empty,
                                FechaEjecutada = DateTime.TryParse(excelWorksheet.Cell(i, 7).Value?.ToString(), out DateTime fechaEjecutada) ? fechaEjecutada.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                IdPers = excelWorksheet.Cell(i, 8).Value != null ? GetPersonaAsignada(excelWorksheet.Cell(i, 8).Value.ToString()) : null
                            };

                            listTareas.Add(rowData);
                        }
                    }

                    return Json(new { Data = listTareas, Msg = "OK" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Data = listTareas, Msg = "Ha ocurrido un error, contacte al administrador" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Data = listTareas, Msg = "Seleccione un Excel" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ImpTareasMovil(int idTicket, List<TareaMovil> tareas)
        {
            try
            {
                foreach (var tarea in tareas)
                {
                    var fechaEje = DateTime.ParseExact(tarea.FechaEjecutada, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    db.CrearTareaMovil2(idTicket, tarea.TipoServicio, tarea.SedeImputado, tarea.Moneda, tarea.Monto, tarea.CeCoImputado, tarea.PartidaPresupuestal, fechaEje, tarea.IdPers, Session["UserId"].ToString());
                }

                return Content("OK");
            }
            catch (Exception)
            {
                return Content("ERROR");
            }
        }

        private Nullable<int> GetMoneda(string text)
        {
            switch (text)
            {
                case "PEN":
                    return 1;
                case "USD":
                    return 2;
                default:
                    return null;
            }
        }

        private Nullable<int> GetSedeImputado(string text)
        {
            return db.TareaMovilSede.FirstOrDefault(q => q.Nombre == text)?.Id ?? null;
        }

        private Nullable<int> GetTipoServicio(string text)
        {
            return db.Tarea.FirstOrDefault(x => x.DescripcionTarea == text && x.TipoGestion == "MOVIL" && x.Estado == true)?.IdTarea ?? null;
        }

        private Nullable<int> GetPersonaAsignada(string text)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var result = dbe.UsuariosMinsurPorColaTareas(ID_ACCO, 9).ToList();

            var personaAsignada = result
                                    .Where(item => item.ASSI.ToUpper() == text)
                                    .Select(item => item.ID_PERS_ENTI)
                                    .FirstOrDefault();

            return personaAsignada;
        }

        public ActionResult ObtenerDetalleSerie(string serie)
        {
            if (serie == "")
            {
                object[] res = new object[0];
                return Json(new { Data = res }, JsonRequestBehavior.AllowGet);
            }
            var result = db.ProductoSerie_DetalleTicket(serie).Select(x => new {
                id = x.ID_TICK,
                text = x.ID_TICK,
                ID_DOCU_SALE = x.ID_DOCU_SALE,
                ID_COMP = x.ID_COMP,
                ID_COMP_END = x.ID_COMP_END,
                ID_PERS_ENTI = x.ID_PERS_ENTI,
                ID_PERS_ENTI_END = x.ID_PERS_ENTI_END,
                ID_QUEU = x.ID_QUEU,
                ID_PERS_ENTI_ASSI = x.ID_PERS_ENTI_ASSI,
                IdProyectoSLA = x.IdProyectoSLA,
                ID_PRIO = x.ID_PRIO,
                Incidente = x.Incidente,
                Servicio = x.Servicio,
                Macroservicio = x.Macroservicio,
                UnidadNegocio = x.UnidadNegocio

            }).ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaMotivoEspera(int id = 0)
        {
            int idEstado, idCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = "";

            if (id == 1)
            {
                var result = db.ListarMotivoEspera(idCuenta, 5, txt).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

                if (field == "NombreMotivo")
                {
                    idEstado = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                    txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                    if (txt == null) txt = "";
                }
                else
                {
                    idEstado = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
                }

                var result = db.ListarMotivoEspera(idCuenta, idEstado, txt).ToList();
                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListarMotivoSource()
        {
            int idSour, idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = "";

            string field = Convert.ToString(Request.Params["filter[filters][0][field]"]);

            if (field == "Nombre")
            {
                idSour = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
                if (txt == null)
                    txt = "";
            }
            else
            {
                idSour = Convert.ToInt32(Request.Params["filter[filters][0][value]"]);
            }

            var result = db.ListarMotivoSource(idAcco, idSour, txt).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateEdata(IEnumerable<HttpPostedFileBase> file, TICKET ticket)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            if (UserId == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.perdioSesion();}window.onload=init;</script>");
            }

            //SubCuenta
            string subCuenta = Request.Form["hdnSubCuenta"].ToString();
            //TicketAutomatico
            int taTicketAutomatico = Convert.ToInt32(Request.Form["hdnTkAutomatico"].ToString());
            string taTitulo = Request.Form["hdnTitulo"].ToString();
            int taFrecuencia = Convert.ToInt32(Request.Form["hdnFrecuencia"].ToString());
            string taDiaSemana = Request.Form["hdnDiaSemana"].ToString();
            string taDiaMes = Request.Form["hdnDiaMes"].ToString();
            string taDiaPersonalizado = Request.Form["hdnDiaPersonalizado"].ToString();
            int taVigencia = Convert.ToInt32(Request.Form["hdnVigencia"].ToString());
            string taVigenciaInicio = Request.Form["hdnVigenciaInicio"].ToString();
            string taVigenciaFin = Request.Form["hdnVigenciaFin"].ToString();
            string taHoraCreacion = Request.Form["hdnHoraCreacion"].ToString();


            /*GESTION DEL CONOCIMIENTO GCR161203*/
            int ID_TEMA = 0; ID_TEMA = Convert.ToInt32(Request.Form["IdTema"]);
            /*Fin*/
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string FEC_INI_TICK = Convert.ToString(Request.Form["FEC_INI_TICK"]);
            string KEY_ATTA = null;
            int ID_PERS_ENTI = 0;
            int hours = 0;

            //Solo aplica a BNV, en otros casos es 0
            int UnidadMineraBnv = 0;


            try
            {
                KEY_ATTA = Convert.ToString(Request.Form["KEY_ATTA"]);

            }
            catch
            {

            }
            try
            {
                int id_prio = Convert.ToInt32(ticket.ID_PRIO);
                //hours = Convert.ToInt32(db.PRIORITies.Single(x => x.ID_PRIO == id_prio).HOU_PRIO);
                hours = Convert.ToInt32(db.SLADetalles.Single(x => x.IdPrioridad == id_prio && x.IdSLA == ticket.IdSLA).TiempoAtencion);
                ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);

                try
                {
                    int ID_AREA = Convert.ToInt32(dbe.PERSON_ENTITY.Single(r => r.ID_PERS_ENTI == ID_PERS_ENTI).ID_AREA);
                    ticket.ID_AREA = ID_AREA;
                }
                catch
                {
                    ticket.ID_AREA = null;
                }


            }
            catch
            {

            }

            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            ticket.FEC_TICK = DateTime.Now;

            if (ModelState.IsValid)
            {
                // Motivo de espera
                if (ticket.ID_STAT == 5 && ticket.IdMotivoEstado == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }


                if (ticket.ID_QUEU == 26 && Convert.ToInt32(ticket.ID_COMP_END) == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                //if (ticket.ID_COMP == 4034 || ticket.ID_COMP == 44862) //Cliente BCP Y INDECOPI
                //{
                //if (ticket.ID_SOUR == 3)
                //{
                if (ticket.FechaRecepcionSolicitud == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERRORBCP','0');}window.onload=init;</script>");
                }

                DateTime now = DateTime.Now;

                if (ticket.FechaRecepcionSolicitud.Value.Date > now.Date || (ticket.FechaRecepcionSolicitud.Value.Date == now.Date &&
    ticket.FechaRecepcionSolicitud.Value.TimeOfDay > now.TimeOfDay))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERRORFECHASOLICITUD','0');}window.onload=init;</script>");
                }




                if (UserId == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (String.IsNullOrEmpty(FEC_INI_TICK) && ticket.ID_STAT == 5)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (ticket.ID_STAT == 5 && ticket.FEC_TICK > ticket.FEC_INI_TICK)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }

                if (ticket.ID_STAT != 5)
                {
                    ticket.FEC_INI_TICK = ticket.FEC_TICK;
                }

                DateTime FechaIni;

                try
                {
                    if (ticket.ID_COMP == null)
                    {
                        ticket.ID_COMP = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI1.Value;
                    }
                }
                catch
                {

                }

                try
                {
                    DateTime.TryParse(Convert.ToString(ticket.FEC_INI_TICK), out FechaIni);
                }
                catch
                {
                    DateTime.TryParse(Convert.ToString(ticket.FEC_TICK), out FechaIni);
                }

                ticket.DAT_EXPI_TICK = FechaIni.AddHours(hours);

                ticket.UserId = UserId;
                ticket.ID_ACCO = ID_ACCO;

                string RMA = Request.Form["RMA"] == null ? null : Convert.ToString(Request.Form["RMA"]);
                ticket.RMA = RMA;


                ticket.ID_STAT_END = Convert.ToInt32(Request.Form["ID_STAT"]);
                ticket.CREATE_TICK = DateTime.Now;
                ticket.MODIFIED_TICK = DateTime.Now;
                ticket.SEND_MAIL = false;
                ticket.SEND_SURVEY = null;

                if (ticket.ID_PERS_ENTI == 63132)
                {
                    ticket.SEND_SURVEY = false;
                }

                ticket.SubCuenta = subCuenta;
                ticket.IdTema = ID_TEMA;  /*GESTION DEL CONOCIMIENTO GCR161203*/


                var queryConfiguracion = db.ConfiguracionCategorias.Where(x => x.IdAcco == ID_ACCO && x.Estado == true && x.EsPadre == true && x.IdCate4 == ticket.ID_CATE).ToList().Count();

                if (queryConfiguracion > 0)
                {
                    ticket.IS_PARENT = true;
                }

                int idGrupo = Request.Form["ID_QUEU"].ToString() == "" ? 0 : Convert.ToInt32(Request.Form["ID_QUEU"].ToString());



                db.TICKETs.Add(ticket);
                db.SaveChanges();
                int id = Convert.ToInt32(ticket.ID_TICK);
                db.Entry(ticket).State = EntityState.Detached;

                //REGISTRO DE TIEMPOS

                DetalleTiempoSla detalleTiempoSLA = new DetalleTiempoSla();

                detalleTiempoSLA.IdTick = ticket.ID_TICK;
                detalleTiempoSLA.IdDocuSale = ticket.IdProyectoSLA;
                detalleTiempoSLA.FechaInicio = ticket.FechaRecepcionSolicitud;
                detalleTiempoSLA.FechaFin = DateTime.Now;
                // Calcular el tiempo transcurrido en horas decimales
                TimeSpan tiempoTranscurrido = DateTime.Now - (ticket.FechaRecepcionSolicitud.HasValue ? ticket.FechaRecepcionSolicitud.Value : DateTime.Now);

                // Calcular el tiempo transcurrido en horas decimales y convertir a decimal
                detalleTiempoSLA.TiempoReal = Convert.ToInt32(tiempoTranscurrido.TotalMinutes);

                detalleTiempoSLA.IdSla = ticket.IdSLA;
                detalleTiempoSLA.IdPrioridad = ticket.ID_PRIO;
                detalleTiempoSLA.IdCategoriaSLA = 1;
                db.DetalleTiempoSla.Add(detalleTiempoSLA);
                db.SaveChanges();

                if (ticket.ID_STAT == 1)
                {
                    DetalleTiempoSla detalleTiempoSLAAtencion = new DetalleTiempoSla();

                    detalleTiempoSLAAtencion.IdTick = ticket.ID_TICK;
                    detalleTiempoSLAAtencion.IdDocuSale = ticket.IdProyectoSLA;
                    detalleTiempoSLAAtencion.FechaInicio = ticket.FechaRecepcionSolicitud;

                    detalleTiempoSLAAtencion.IdSla = ticket.IdSLA;
                    detalleTiempoSLAAtencion.IdPrioridad = ticket.ID_PRIO;
                    detalleTiempoSLAAtencion.IdCategoriaSLA = 2;
                    db.DetalleTiempoSla.Add(detalleTiempoSLAAtencion);
                    db.SaveChanges();


                    DetalleTiempoSla detalleTiempoSLAResolucion = new DetalleTiempoSla();
                    detalleTiempoSLAResolucion.IdTick = ticket.ID_TICK;
                    detalleTiempoSLAResolucion.IdDocuSale = ticket.IdProyectoSLA;
                    detalleTiempoSLAResolucion.FechaInicio = ticket.FechaRecepcionSolicitud;

                    detalleTiempoSLAResolucion.IdSla = ticket.IdSLA;
                    detalleTiempoSLAResolucion.IdPrioridad = ticket.ID_PRIO;
                    detalleTiempoSLAResolucion.IdCategoriaSLA = 3;
                    db.DetalleTiempoSla.Add(detalleTiempoSLAResolucion);
                    db.SaveChanges();

                }
                else if (ticket.ID_STAT == 5)
                {
                    DetalleTiempoSla detalleTiempoSLAAtencionprogra = new DetalleTiempoSla();

                    detalleTiempoSLAAtencionprogra.IdTick = ticket.ID_TICK;
                    detalleTiempoSLAAtencionprogra.IdDocuSale = ticket.IdProyectoSLA;
                    detalleTiempoSLAAtencionprogra.FechaInicio = ticket.FechaRecepcionSolicitud;
                    detalleTiempoSLAAtencionprogra.FechaFin = ticket.CREATE_TICK;
                    TimeSpan tiempodetalleTiempoSLAAtencionprogra = DateTime.Now - (ticket.FechaRecepcionSolicitud.HasValue ? ticket.FechaRecepcionSolicitud.Value : DateTime.Now);

                    // Calcular el tiempo transcurrido en horas decimales y convertir a decimal
                    detalleTiempoSLAAtencionprogra.TiempoReal = Convert.ToInt32(tiempodetalleTiempoSLAAtencionprogra.TotalMinutes);
                    detalleTiempoSLAAtencionprogra.IdSla = ticket.IdSLA;
                    detalleTiempoSLAAtencionprogra.IdPrioridad = ticket.ID_PRIO;
                    detalleTiempoSLAAtencionprogra.IdCategoriaSLA = 2;
                    db.DetalleTiempoSla.Add(detalleTiempoSLAAtencionprogra);
                    db.SaveChanges();

                    DetalleTiempoSla detalleTiempoSLAAtencionprogra2 = new DetalleTiempoSla();

                    detalleTiempoSLAAtencionprogra2.IdTick = ticket.ID_TICK;
                    detalleTiempoSLAAtencionprogra2.IdDocuSale = ticket.IdProyectoSLA;
                    detalleTiempoSLAAtencionprogra2.FechaInicio = ticket.FEC_INI_TICK;
                    detalleTiempoSLAAtencionprogra2.IdSla = ticket.IdSLA;
                    detalleTiempoSLAAtencionprogra2.IdPrioridad = ticket.ID_PRIO;
                    detalleTiempoSLAAtencionprogra2.IdCategoriaSLA = 2;
                    db.DetalleTiempoSla.Add(detalleTiempoSLAAtencionprogra2);
                    db.SaveChanges();

                    DetalleTiempoSla detalleTiempoSLAResolucion = new DetalleTiempoSla();
                    detalleTiempoSLAResolucion.IdTick = ticket.ID_TICK;
                    detalleTiempoSLAResolucion.IdDocuSale = ticket.IdProyectoSLA;
                    detalleTiempoSLAResolucion.FechaInicio = ticket.FechaRecepcionSolicitud;
                    detalleTiempoSLAResolucion.FechaFin = ticket.CREATE_TICK;
                    TimeSpan tiempodetalleTiempoSLAResolucion = DateTime.Now - (ticket.FechaRecepcionSolicitud.HasValue ? ticket.FechaRecepcionSolicitud.Value : DateTime.Now);

                    // Calcular el tiempo transcurrido en horas decimales y convertir a decimal
                    detalleTiempoSLAResolucion.TiempoReal = Convert.ToInt32(tiempodetalleTiempoSLAResolucion.TotalMinutes);
                    detalleTiempoSLAResolucion.IdSla = ticket.IdSLA;
                    detalleTiempoSLAResolucion.IdPrioridad = ticket.ID_PRIO;
                    detalleTiempoSLAResolucion.IdCategoriaSLA = 3;
                    db.DetalleTiempoSla.Add(detalleTiempoSLAResolucion);
                    db.SaveChanges();

                    DetalleTiempoSla detalleTiempoSLAResolucion2 = new DetalleTiempoSla();
                    detalleTiempoSLAResolucion2.IdTick = ticket.ID_TICK;
                    detalleTiempoSLAResolucion2.IdDocuSale = ticket.IdProyectoSLA;
                    detalleTiempoSLAResolucion2.FechaInicio = ticket.FEC_INI_TICK;
                    detalleTiempoSLAResolucion2.IdSla = ticket.IdSLA;
                    detalleTiempoSLAResolucion2.IdPrioridad = ticket.ID_PRIO;
                    detalleTiempoSLAResolucion2.IdCategoriaSLA = 3;
                    db.DetalleTiempoSla.Add(detalleTiempoSLAResolucion2);
                    db.SaveChanges();
                }
                else if (ticket.ID_STAT == 4)
                {
                    DetalleTiempoSla detalleTiempoSLAAtencionResuelto = new DetalleTiempoSla();
                    detalleTiempoSLAAtencionResuelto.IdTick = ticket.ID_TICK;
                    detalleTiempoSLAAtencionResuelto.IdDocuSale = ticket.IdProyectoSLA;
                    detalleTiempoSLAAtencionResuelto.FechaInicio = ticket.FechaRecepcionSolicitud;
                    detalleTiempoSLAAtencionResuelto.FechaFin = DateTime.Now;
                    TimeSpan tiempdetalleTiempoSLAAtencionResuelto = DateTime.Now - (ticket.FechaRecepcionSolicitud.HasValue ? ticket.FechaRecepcionSolicitud.Value : DateTime.Now);

                    // Calcular el tiempo transcurrido en horas decimales y convertir a decimal
                    detalleTiempoSLAAtencionResuelto.TiempoReal = Convert.ToInt32(tiempdetalleTiempoSLAAtencionResuelto.TotalMinutes);
                    detalleTiempoSLAAtencionResuelto.IdSla = ticket.IdSLA;
                    detalleTiempoSLAAtencionResuelto.IdPrioridad = ticket.ID_PRIO;
                    detalleTiempoSLAAtencionResuelto.IdCategoriaSLA = 2;
                    db.DetalleTiempoSla.Add(detalleTiempoSLAAtencionResuelto);
                    db.SaveChanges();

                    DetalleTiempoSla detalleTiempoSLAResolucion = new DetalleTiempoSla();
                    detalleTiempoSLAResolucion.IdTick = ticket.ID_TICK;
                    detalleTiempoSLAResolucion.IdDocuSale = ticket.IdProyectoSLA;
                    detalleTiempoSLAResolucion.FechaInicio = ticket.FechaRecepcionSolicitud;
                    detalleTiempoSLAResolucion.FechaFin = DateTime.Now;
                    TimeSpan tiempdetalleTiempoSLAResolucion = DateTime.Now - (ticket.FechaRecepcionSolicitud.HasValue ? ticket.FechaRecepcionSolicitud.Value : DateTime.Now);

                    // Calcular el tiempo transcurrido en horas decimales y convertir a decimal
                    detalleTiempoSLAResolucion.TiempoReal = Convert.ToInt32(tiempdetalleTiempoSLAResolucion.TotalMinutes);
                    detalleTiempoSLAResolucion.IdSla = ticket.IdSLA;
                    detalleTiempoSLAResolucion.IdPrioridad = ticket.ID_PRIO;
                    detalleTiempoSLAResolucion.IdCategoriaSLA = 3;
                    db.DetalleTiempoSla.Add(detalleTiempoSLAResolucion);
                    db.SaveChanges();

                }


                if (queryConfiguracion > 0)
                {


                    var ticketsHijos = db.CrearTicketsHijos(ticket.ID_CATE, ticket.ID_ACCO, ticket.ID_AREA, ticket.ID_QUEU, ticket.ID_TICK, ticket.ID_PERS_ENTI, ticket.ID_PERS_ENTI_ASSI, ticket.ID_LOCA, ticket.SUM_TICK);
                }






                int bandera = 0;

                if (KEY_ATTA != null)
                {
                    var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KEY_ATTA)
                        .Where(x => x.ID_INCI == null).ToList();
                    if (Attachs.Count() > 0)
                    {
                        foreach (ATTACHED attach in Attachs)
                        {
                            try
                            {
                                var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                                EObj.ID_INCI = id;
                                db.Entry(EObj).State = EntityState.Modified;
                                db.SaveChanges();
                                db.Entry(EObj).State = EntityState.Detached;
                            }
                            catch
                            {

                            }

                        }
                        bandera = 1;
                    }
                }

                string code = Convert.ToString(db.TICKETs.Single(t => t.ID_TICK == id).COD_TICK);

                int ID_PERS_ENTI_SESS = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                pt.UpdatePointsByTicket(ticket, ID_PERS_ENTI_SESS, bandera);

                //Insertar Ticket Automático
                if (taTicketAutomatico == 1)
                {
                    TicketAutomatico objTicketAutomatico = new TicketAutomatico();
                    objTicketAutomatico.IdCuenta = ID_ACCO;
                    objTicketAutomatico.IdTicket = ticket.ID_TICK;
                    objTicketAutomatico.Titulo = taTitulo;
                    objTicketAutomatico.Frecuencia = taFrecuencia;
                    if (taFrecuencia == 5)
                    {
                        objTicketAutomatico.Dia = Convert.ToInt32(taDiaPersonalizado);
                    }
                    else
                    {
                        objTicketAutomatico.Dia = Convert.ToInt32(taDiaMes);
                    }
                    objTicketAutomatico.Vigencia = taVigencia;
                    objTicketAutomatico.VigenciaFechaInicio = Convert.ToDateTime(taVigenciaInicio);
                    objTicketAutomatico.VigenciaFechaFin = Convert.ToDateTime(taVigenciaFin);
                    objTicketAutomatico.HoraCreacion = Convert.ToDateTime(taHoraCreacion);
                    objTicketAutomatico.UsuarioCrea = Convert.ToInt32(Session["UserId"]);
                    objTicketAutomatico.Estado = true;
                    objTicketAutomatico.FechaCrea = DateTime.Now;
                    db.TicketAutomaticoes.Add(objTicketAutomatico);
                    db.SaveChanges();

                    string[] Dias = taDiaSemana.Split('-');
                    foreach (string dia in Dias)
                    {
                        if (!String.IsNullOrEmpty(dia))
                        {
                            TicketAutomaticoDia objTicketAutoDia = new TicketAutomaticoDia();
                            objTicketAutoDia.IdTicketAutomatico = objTicketAutomatico.Id;
                            objTicketAutoDia.Dia = Convert.ToInt32(dia);
                            db.TicketAutomaticoDias.Add(objTicketAutoDia);
                            db.SaveChanges();
                        }
                    }
                }


                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','" + code + "-" + id + "');}window.onload=init;</script>");


            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','0',);}window.onload=init;</script>");
            }

            // return View();
        }


    }

    public class TareaMovil
    {
        public int? SedeImputado { get; set; }
        public int? Moneda { get; set; }
        public decimal? Monto { get; set; }
        public string CeCoImputado { get; set; }
        public string PartidaPresupuestal { get; set; }
        public int? TipoServicio { get; set; }
        public string FechaEjecutada { get; set; }
        public int? IdPers { get; set; }
    }

    public class EstadoTareaMovil
    {
        public int IdTareaDetalle { get; set; }
        public string Estado { get; set; }
    }
}