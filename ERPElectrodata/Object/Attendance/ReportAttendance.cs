using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Objects.CreateMail;
using ERPElectrodata.Objects;
using ERPElectrodata.Models;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Globalization;
using System.Threading;

using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace ERPElectrodata.Object.Attendance
{
    public class ReportAttendance
    {
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dba = new AppLogEntities();
        string reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
        string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
        DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

        

        public string Attendance(int ID_PERS_ENTI, string DES_QUEU,string SEXO, string NOMBRE,string email)
        {
            string file = FilePDF(ID_PERS_ENTI, DES_QUEU);
            DateTime fecha = DateTime.Now.AddDays(-2);
            SendMail mail = new SendMail();
            Body _body = new Body();

            //mail.message.From = "itms@electrodata.com.pe";
            //mail.message.To = email; //"esalazar@electrodata.com.pe;";//
            ////mail.message.BCC = "esalazar@electrodata.com.pe;";
            //mail.message.AddAttachment(file);
            //mail.message.HTMLBody = _body.AttendanceReportPerson(SEXO, NOMBRE);
            //mail.message.Subject = "Attendace Report By Person "+dtinfo_en.GetMonthName(fecha.Month)+" / Reporte de Asistencia por Persona "+dtinfo_es.GetMonthName(fecha.Month);
            //mail.message.Send();

            //string body_html = _body.AttendanceReportPerson(SEXO, NOMBRE);
            //var message = mail.mailMessage;
            //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            //message.To.Add("lsempertegui@electrodata.com.pe");
            ////message.To.Add(email); //Descomentar   
            //message.Attachments.Aggregate(file);
            //message.AlternateViews.Add(htmlView);
            //message.Subject = "Attendace Report By Person " + dtinfo_en.GetMonthName(fecha.Month) + " / Reporte de Asistencia por Persona " + dtinfo_es.GetMonthName(fecha.Month);
            //mail.smtp.Send(message);

            return "OK";
        }

        public string FilePDF(int ID_PERS_ENTI, string DES_QUEU)
        {
            try
            {

                CultureInfo ci = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = ci;
                //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);

                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                ServerReport sr = new ServerReport();
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/Attendance/ReportByPerson";

                string DATE_END = Convert.ToString(DateTime.Now.AddDays(-1));
                string DATE_START = Convert.ToString(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                string ID_PERS_ENTIs = Convert.ToString(ID_PERS_ENTI);

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("ID_PERS_ENTI", ID_PERS_ENTIs);
                param[1] = new ReportParameter("DATE_START", "03/01/2015");//DATE_START);
                param[2] = new ReportParameter("DATE_END", "03/22/2015");//DATE_END);

                sr.SetParameters(param);
                sr.Refresh();

                byte[] renderedBytes;

                renderedBytes = sr.Render("pdf");
                //var result = sr.Render("pdf");

                bytes = sr.Render("PDF", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = "C:\\BackUp\\Reporting Services\\" + DES_QUEU + ".pdf";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                return filename;
            }
            catch
            {
                return "ERROR";
            }
        }
    }


}