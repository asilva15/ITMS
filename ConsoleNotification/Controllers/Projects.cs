using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleNotification.Models;
using ConsoleNotification.Plugins;
using ConsoleNotification.Templates;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Configuration;

namespace ConsoleNotification.Controllers
{
    //Clase utilizada para enviar reporte de avance proyectos semanal.
    class Projects
    {
        //Declaracion de Objetos para acceso a base de datos.
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();
        string reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
 
        //Función encargada de crear y enviar correo de proyectos.
        public string ReportProject()
        {
            //Generar archivo PDF.
            string file = FilePDF();

            try
            {
                //Creamos el objeto correo electrónico.
                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;

                //Creamos cuerpo de correo.
                ProjectsTemplate body = new ProjectsTemplate();
                string body_html = body.WeeklyReportProject();
                
                if (body_html != null)
                {
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                    message.To.Add("arivadeneyra@electrodata.com.pe");
                    message.CC.Add("bacevedo@electrodata.com.pe");
                    message.CC.Add("lmasias@electrodata.com.pe");
                    message.Bcc.Add("dnunez@electrodata.com.pe");
                    message.Bcc.Add("rvilchez@electrodata.com.pe");
                    message.Bcc.Add("fsandoval@electrodata.com.pe");
                    message.Bcc.Add("srivadeneyra@electrodata.com.pe");
                    message.Bcc.Add("pmo@electrodata.com.pe");
                    
                    message.AlternateViews.Add(htmlView);
                    message.Subject = "Reporte Semanal de Proyectos (29/06 - 05/07)";

                    //Adjuntamos archivo.
                    Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                    message.Attachments.Add(attachment);
                    newMail.smtp.Send(message);

                    Console.WriteLine("Envio de correo realizado correctamente" );

                    return "OK";
                }
                else
                {
                    Console.WriteLine("Error: No se pudo generar el Template para el Reporte de Proyectos");
                    return "ERROR";

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo de Reporte Semanal de Proyectos");
                Console.WriteLine("Error generado: " + ex.Message.ToString());
                return "Fallo en el elvio de correo del Reporte Semanal de Proyectos";
            }
        }

        //Función encargada de generar reporte de avance de proyectos en pdf.
        public string FilePDF()
        {
            try
            {
                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                DateTime hoy= DateTime.Now;
                string ahora=hoy.Day+"-"+hoy.Month+"-"+hoy.Year;

                //Credenciales de servidor.
                ServerReport sr = new ServerReport();
                sr.ReportServerCredentials = new CredencialesReporting("Administrator", "Electr0D@t4", "");
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/Proyectos/ReportProyectos";
                sr.Refresh();

                byte[] renderedBytes;

                renderedBytes = sr.Render("pdf");

                //Crea el PDF.
                bytes = sr.Render("PDF", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = "C:\\BackUp\\Reporting Services\\Proyectos\\Reporte_Proyectos " +ahora+ ".pdf";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                //Retorna dirección de PDF
                return filename;
            }
            catch
            {
                Console.WriteLine("Error generando archivo PDF para el Reporte Semanal de Proyectos ");
                return "ERROR";
            }
        }
    }
}
