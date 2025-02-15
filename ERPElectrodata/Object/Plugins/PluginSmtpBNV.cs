using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ERPElectrodata.Models;
using ERPElectrodata.Objects.CreateMail;
using System.Data.Entity;
using System.Threading;
using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace ERPElectrodata.Object.Plugins
{
    public class PluginSmtpBNV
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dba = new AppLogEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
        DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

        public MailMessage mailMessage = new MailMessage();
        public SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
        //public SmtpClient smtp = new SmtpClient("10.10.136.96", 25);
        private string fromEmail = "";
        private string pass = "";



        // GET: /SendMail/
        public PluginSmtpBNV()
        {

            fromEmail = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24).VAL_ACCO_PARA.ToString();
            pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25).VAL_ACCO_PARA.ToString();
            MailAddress fromAddress = new MailAddress(fromEmail, "IT Management System");
            // MailMessage mailMessage = new MailMessage("esalazar@electrodata.com.pe" "esalazar@electrodata.com.pe", "text", "Text");
            //mailMessage.fr;
            mailMessage.From = fromAddress;

            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Port = 587;
            //smtp.
            NetworkCredential basicCredential = new NetworkCredential(fromEmail, pass);
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            smtp.Credentials = basicCredential;
            //smtp.Send(mailMessage);
        }

    }
}