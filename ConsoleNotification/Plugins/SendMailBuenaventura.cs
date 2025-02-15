using ConsoleNotification.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleNotification.Plugins
{
    class SendMailBuenaventura
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
        DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

        public MailMessage mailMessage = new MailMessage();
        public SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com");
        private string fromEmail = "";
        private string pass = "";

        public SendMailBuenaventura()
        {
            //fromEmail = "elvis.rivera0206@outlook.com.pe";
            //pass = "ElvisRivera0206";
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Port = 587;
            fromEmail = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24).VAL_ACCO_PARA.ToString();
            pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25).VAL_ACCO_PARA.ToString();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            //smtp.Send(mailMessage);
            //smtp.Dispose();

            //MailAddress fromAdress = new MailAddress(fromEmail, "IT Management System");

            //mailMessage.From = fromAdress;


            //smtp.Credentials = basicCredential;
        }

    }
}
