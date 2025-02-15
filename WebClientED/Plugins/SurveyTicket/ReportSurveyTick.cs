using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text;
using System.Net.Mime;
using WebClientED.Models;
using WebClientED.Plugins;
using WebClientED.Plugins.CreateMail;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace WebClientED.Plugins.SurveyTicket
{
    public class ReportSurveyTick : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();
        CoreEntities dbc = new CoreEntities();
        string reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
        string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
        DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;
        public string SurveyTick(int ID_TICK, string DES_FILE, string SEXO, string NOMBRE, string email, int ID_ACCO)
        {
            try
            {
                string file = FilePDF(ID_TICK, DES_FILE); //Genera el PDF.          

                Body _body = new Body();
                SendMail newMail = new SendMail();
                var msg = newMail.mailMessage;

                var html_msg = _body.MessageSurveyTicket(SEXO, NOMBRE);//Obtiene el Texto.          
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                msg.AlternateViews.Add(htmlView);

                msg.Subject = "Reporte de Satisfacción";
                msg.To.Add(email);
                
                if (ID_ACCO == 3)
                {
                    //msg.CC.Add("eduardo.rojas@hudbayminerals.com,edeza@electrodata.com.pe,ysotelo@electrodata.com.pe,servicedesk@electrodata.com.pe");
                    //msg.Bcc.Add("arivadeneyra@electrodata.com.pe,bacevedo@electrodata.com.pe");
                    msg.CC.Add("edeza@electrodata.com.pe");
                }
                if (ID_ACCO == 1)
                {
                    msg.CC.Add("evalverde@electrodata.com.pe,edeza@electrodata.com.pe,masencio@electrodata.com.pe,servicedesk@electrodata.com.pe,Liliana.chavez@goldfields.com");
                    msg.Bcc.Add("arivadeneyra@electrodata.com.pe");
                    //msg.CC.Add("yrocca@electrodata.com.pe");
                    //msg.Bcc.Add("yrocca@electrodata.com.pe");
             
                }
                if (ID_ACCO == 4)
                {
                    msg.CC.Add("arivadeneyra@electrodata.com.pe,dnunez@electrodata.com.pe,servicedesk@electrodata.com.pe");
                    msg.Bcc.Add("jquisper@electrodata.com.pe");
                }
                if (ID_ACCO == 19)
                {
                    msg.CC.Add("evalverde@electrodata.com.pe,edeza@electrodata.com.pe,servicedesk@electrodata.com.pe,r_delgado@elbrocal.com.pe");
                    msg.Bcc.Add("arivadeneyra@electrodata.com.pe");
                }

                Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                msg.Attachments.Add(attachment);
                newMail.smtp.Send(msg);
                
            }
            catch(Exception ex)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = ex.Message;
                exc.DES_EXCE = "WebClientED - ReportSurveyTick - SurveyTick - Send Mail";
                dbl.EXCEPTION.Add(exc);
                dbl.SaveChanges();
                return "ERROR";
            }
            return "OK";
        }

        public string FilePDF(int ID_TICK, string DES_FILE)
        {
            try
            {
                var query = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                CultureInfo ci = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = ci;               

                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                ServerReport sr = new ServerReport();
                //sr.ReportServerCredentials = new CredencialesReporting("Administrator", "Electr0D@t4", "");
                sr.ReportServerCredentials = new CredencialesReporting("Administrator", "ITMS$15DEV$", "");
                sr.ReportServerUrl = new Uri(reportServer);
                if (query.ID_ACCO == 1)
                    sr.ReportPath = "/SurveyTicket/ReportSurveyTicketGF";
                else
                    sr.ReportPath = "/SurveyTicket/ReportSurveyTicket";

                string ID_TICKs = Convert.ToString(ID_TICK);

                ReportParameter[] param = new ReportParameter[1]; // Parametros que recibe el reporte
                param[0] = new ReportParameter("ID_TICK", ID_TICKs);            

                sr.SetParameters(param);
                sr.Refresh();

                byte[] renderedBytes;

                renderedBytes = sr.Render("pdf");

                //Crea el PDF.
                bytes = sr.Render("PDF", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = "C:\\BackUp\\Reporting Services\\" + DES_FILE + ".pdf";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                return filename;
            }
            catch(Exception ex)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = ex.Message;
                exc.DES_EXCE = "WebClientED - ReportSurveyTick - FilePDF - Send Mail";
                dbl.EXCEPTION.Add(exc);
                dbl.SaveChanges();
                return "ERROR";
            }
        }

    }
}
