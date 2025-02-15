using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ConsoleNotification.Models;
using System.Globalization;
using System.Threading;

namespace ConsoleNotification.Templates
{
    class DocumentControlTemplate
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        private string footer = "";
        private string SocialNetwork = "";
        private string IconPaymentBallot = "";
        private string IconCertificate5th = "";
        private string IconCertificateUtil = "";
        private string IpPublica = "";
        private string IpServer = "http://itms.electrodata.com.pe/";
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
  
     public DocumentControlTemplate()
        {
            IpPublica = ConfigurationManager.AppSettings["IpPublica"].ToString();
            IpServer = ConfigurationManager.AppSettings["IpServer"].ToString();

            textInfo = cultureInfo.TextInfo;

            footer = @"<table style=""width:100%; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr  style=""background-color:#EAEAEA"">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:12px;"" >&copy; " + Convert.ToString(DateTime.Now.Year) + @" Electrodata All Rights Reserved</span></td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                @"</table>";

            SocialNetwork = @"<table style=""width:100%;"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;""></td>" +
                        @"<td style=""width:40px;""><img src=""" + IpPublica + @"Content/Images/social/social-facebook.png"" /></td>" +
                        @"<td style=""width:40px;""><img src=""" + IpPublica + @"Content/Images/social/social-twitter.png"" /></td>" +
                        @"<td style=""width:40px;""><img src=""" + IpPublica + @"Content/Images/social/social-linkedin.png"" /></td>" +
                        @"<td ><img src=""" + IpPublica + @"Content/Images/social/social-email.png"" /></td>" +
                        @"<td style=""width:30px;""></td>" +
                    @"</tr>" +
                @"</table>";

            IconPaymentBallot = @"<table style=""width:100%;"">" +
                    @"<tr>" +
                        @"<td style=""width:100%; text-align:center;""><img src=""" + IpPublica + @"Content/Images/PaymentBallot.png"" /></td>" +
                    @"</tr>" +
                @"</table>";

            IconCertificate5th = @"<table style=""width:100%;"">" +
                    @"<tr>" +
                        @"<td style=""width:100%; text-align:center;""><img src=""" + IpPublica + @"Content/Images/Certicate5th.png"" /></td>" +
                    @"</tr>" +
                @"</table>";

            IconCertificateUtil = @"<table style=""width:100%;"">" +
                    @"<tr>" +
                        @"<td style=""width:100%; text-align:center;""><img src=""" + IpPublica + @"Content/Images/Certicate5th.png"" /></td>" +
                    @"</tr>" +
                @"</table>";

        }

        public string NewDocumentControl(int iddc)
        {
            //try
            //{
            string color = "#022A41";

            string fecha = String.Format("{0:D}", DateTime.Now);

            var listCIA = (from ce in dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1)
                           //join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               ID_ENTI = ce.ID_ENTI,
                               COM_NAME = ce.COM_NAME,
                           }).ToList();

            var query = (from dc in dbc.DOCUMENT_CONTROL.Where(x => x.ID_DOCU_CONT == iddc).ToList()
                         join cia in listCIA on dc.ID_CIA equals cia.ID_ENTI
                         join td in dbc.DOCUMENT_CONTROL_TYPE on dc.ID_DOCU_CONT_TYPE equals td.ID_DOCU_CONT_TYPE
                         select new
                         {
                             NAM_CIA = cia.COM_NAME,
                             NAM_DOCU_CONT_TYPE = td.NAM_DOCU_CONT_TYPE,
                             NUM_DOCU = dc.NUM_DOCU,
                         }).First();



            string mensaje = @"<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 600px; background-color: white; font-family: 'segoe ui',verdana,sans-serif; font-size: 14px;"">" +
        @"<colgroup>" +
            @"<col style=""width: 20px"">" +
            @"<col style=""width: 20px"">" +
            @"<col style=""width: 150px"">" +
            @"<col style=""width: 30px"">" +
            @"<col style=""width: 80px"">" +

            @"<col style=""width: 20px"">" +
            @"<col style=""width: 30px"">" +
            @"<col style=""width: 130px"">" +
            @"<col style=""width: 100px"">" +
            @"<col style=""width: 20px"">" +
        @"</colgroup>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<th>&nbsp;</th>" +
            @"<th colspan=""8"">&nbsp;</th>" +
            @"<th>&nbsp;</th>" +
        @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td></td>" +
            @"<td colspan=""4""></td>" +
            @"<td colspan=""4"" style=""text-align: right; font-size: 20px; color: white;"">" + Convert.ToString("Document Control") + @" - " +
                Convert.ToString("New Reception") + @"</td>" +
            @"<td></td>" +
        @"</tr>" +
       @"<tr style=""background-color:" + color + @";"">" +
             @"<td></td>" +

            @"<td colspan=""4"" style=""font-size:18px; color:white;"">" + fecha + "</td>" +

            @"<td colspan=""4""></td>" +
            @"<td></td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td></td>" +
            @"<td colspan=""8"" style=""font-size:22px; color:white;"">Information Technologies Management System</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr style=""background-color: " + color + @"; color: white;"">" +
            @"<td></td>" +
            @"<td colspan=""8"">English Version</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family:Calibri"">CIA</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.NAM_CIA + "</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Type Document</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.NAM_DOCU_CONT_TYPE + "</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Number Document</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"">" + query.NUM_DOCU + "</td>" +
            @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
             @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr style=""background-color:" + color + @"; color: white;"">" +
            @"<td></td>" +
            @"<td colspan=""8"">Versión en Español</td>" +
             @"<td></td>" +
        @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">CIA</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.NAM_CIA + "</td>" +
            @"<td></td>" +
        @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Tipo Documento</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.NAM_DOCU_CONT_TYPE + "</td>" +
             @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Número Documento</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.NUM_DOCU + "</td>" +
             @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr style=""background-color: " + color + @"; color: white;"">" +
            @"<td></td>" +
            @"<td colspan=""8"">Social Network</td>" +
            @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
             @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
        @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td colspan=""7"">" + SocialNetwork + "</td>" +


     @"<td>&nbsp;</td>" +

       @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
        @"</tr>" +

        @"<tr style=""background-color: #eaeaea; font-size: 12px; "">" +
             @"<td></td>" +
            @"<td colspan=""8"">© 2015 Electrodata All Rights Reserved</td>" +
             @"<td></td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
         @"</tr>" +
    @"</table>";

            return mensaje;

        }
    }
}
