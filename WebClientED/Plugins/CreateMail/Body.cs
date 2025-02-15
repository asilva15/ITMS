using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClientED.Models;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace WebClientED.Plugins.CreateMail
{
    public class Body
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        //private string footer = "";
        //private string SocialNetwork = "";
        //private string IconPaymentBallot = "";
        //private string IconCertificate5th = "";
        //private string IconCertificateUtil = "";
        //private string IpPublica = "";
        //private string IpServer = "http://itms.electrodata.com.pe/";
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        public string MessageSurveyTicket(string SEXO, string NOMBRE)
        {
            textInfo = cultureInfo.TextInfo;

            string stringsurveyticket = @"<div style=""font-family:verdana; font-size:10pt;"">"+    
                @"" + (SEXO == "M" ? "Estimado " : SEXO == "F" ? "Estimada ":"" ) + textInfo.ToTitleCase(NOMBRE) + ",<br /><br />" +

                @"<div>Gracias por haber participado de nuestra encuesta.</div> <br />" +

            @"</div>" +

                @"<body><div style=""font-size: 12px;font-family: Verdana;"">Best Regards | Saludos Cordiales,</div><br/>" +
                @"<table style=""min-width:300px; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr height=""95px"" >" +
                        @"<td height=""95px"" style=""width:123px;font-family: Verdana;padding:0px 0px;"">" +
                            @"<a href=""http://www.electrodata.com.pe/"" style=""text-decoration:none;"" class=""Aqui"">" +
                                @"<table cellspacing=""0"" cellpadding=""0""><tr><td>" +
                                    @"<font style=""color: #022A41; font-weight: bold; font-size: 18px;text-decoration:none;font-family: Verdana;"">Electro</font><font style=""color: #CB3F37; font-weight: bold; font-size: 18px;font-family: Verdana;"">data</font>" +
                                @"</td></tr></table>" +
                            @"</a>" +
                        "</td>" +
                         @"<td width=""4px"" style=""padding-left:0px;border-left:1px solid #022A41;width:4px;"">&nbsp;</td>" +
                        @"<td  style=""min-width:277px;font-family: Verdana;padding:0px 0px;"">" +                
                            @"<font style=""font-size: 12px;font-weight:bold;font-family: Verdana;"">" + "ITMS" + "</font><br/>" +
                            @"<font style=""font-size: 12px;font-family: Verdana;"">" + "IT Management System" + @" | " + "Sistema de Gestión de TI" + "</font><br/>" +
                            @"<font style=""font-size: 12px;font-family: Verdana;"">" + "itms@electrodata.com.pe" + "</font><br/>" +               
                            @"<font style=""font-size: 12px;font-family: Verdana;"">Office. " + "+51 1 2713350" + @" – Ext. " + "435" + "</font><br/>" +               
                            @"<font style=""font-size: 12px;font-family: Verdana;"">" + @"Perú</font>" +               
                        "</td>" +
                    @"</tr>" +
                @"</table></body>";

            return stringsurveyticket;
        }


        internal object MessageSurveyTicket()
        {
            throw new NotImplementedException();
        }
    }    
}