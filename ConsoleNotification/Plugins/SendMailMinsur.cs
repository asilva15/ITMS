using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleNotification.Models;
using System.Globalization;
using System.Threading;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace ConsoleNotification.Plugins
{
    public class SendMailMinsur
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
        DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

        public MailMessage mailMessage = new MailMessage();
        public SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        private string fromEmail = "";
        private string pass = "";

       public SendMailMinsur()
        {
            fromEmail = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 56 && x.ID_PARA == 24).VAL_ACCO_PARA.ToString();
            pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 56 && x.ID_PARA == 25).VAL_ACCO_PARA.ToString();

            MailAddress fromAddress = new MailAddress(fromEmail, "IT Management System");
           // MailMessage mailMessage = new MailMessage("esalazar@electrodata.com.pe" "esalazar@electrodata.com.pe", "text", "Text");
            //mailMessage.fr;
            mailMessage.From = fromAddress;

            smtp.EnableSsl = true;
            //smtp.
            NetworkCredential basicCredential = new NetworkCredential(fromEmail, pass);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = basicCredential;
            //smtp.Send(mailMessage);
        }
    }
}
