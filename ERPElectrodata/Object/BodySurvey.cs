using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;
using System.Configuration;
using ERPElectrodata.Object;

namespace ERPElectrodata.Object
{
    public class BodySurvey
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        private AESRijndael seg = new AESRijndael();

        private string IpServer = "";

        public BodySurvey()
        {
            IpServer = ConfigurationManager.AppSettings["IpServer"].ToString();
            textInfo = cultureInfo.TextInfo;
        }

        public string htmlSendSurvey(PERSON_ENTITY pe, CLASS_ENTITY ce, int ID_SURV_TICK, int CountQuestion, int ID_ACCO)
        {
            try

            {
                //string destinationURL = "http://www.contoso.com/default.aspx?user=test";
                //string xx = "~/Finish?url=" + //HttpContext.Current.Server.UrlEncode(destinationURL);
                //System.Web.HttpUtility.UrlEncode(destinationURL);
                string colorHeader = "#eeeeee";
                string colorHeaderText = "#022A41";
                string objetivo = "Nuestro principal objetivo es brindarle un mejor servicio, por eso solicitamos tu ayuda completando la siguiente encuesta que consta de " + Convert.ToString(CountQuestion) + @" preguntas.";

                if (ID_ACCO == 4)
                {
                    colorHeader = "#022A41";
                    colorHeaderText = "white";
                }
                if (ID_ACCO == 1)
                {
                    objetivo = "Nuestro principal objetivo es brindarle un mejor servicio, por eso solicitamos tu ayuda completando la siguiente encuesta.";
                }

                string msg =
@"<table border=""0"" width=""550"" cellpadding=""0"" cellspacing=""0"" align=""center"" style=""font-family: 'Segoe UI', Verdana, Helvetica, Sans-Serif;"">" +
    @"<tbody>" +
        @"<tr>" +
            @"<td style=""width:23px;"">&nbsp;</td>" +
            @"<td style=""min-width:313px;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""min-width:179px;"">&nbsp;</td>" +
            @"<td style=""width:23px;"">&nbsp;</td>" +
        @"</tr>" +
        //Modificar
        @"<tr style=""background-color:" + colorHeader + @";"">" +
            @"<td colspan=""5"" style=""border-top:1px solid " + colorHeader + @";border-bottom:1px solid " + colorHeader + @";"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + colorHeader + @";"">" +
            @"<td colspan=""5"" style=""border-top:1px solid " + colorHeader + @";border-bottom:1px solid " + colorHeader + @";"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + colorHeader + @"; color:white;"">" +
            @"<td style=""border:1px solid " + colorHeader + @";"">&nbsp;</td>" +
            @"<td rowspan=""2"" style=""background-color:" + colorHeader + @";border:1px solid " + colorHeader + @";"">" +
                @"<img src=""cid:log-edgw"" />" +
            @"</td>" +
            @"<td style=""border:1px solid " + colorHeader + @";"">&nbsp;</td>" +
            @"<td style=""border:1px solid " + colorHeader + @";color:" + colorHeaderText + @";"">Tu Opinión</td>" +
            @"<td style=""border:1px solid " + colorHeader + @";"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + colorHeader + @";color:white;"">" +
            @"<td style=""border:1px solid " + colorHeader + @";"">&nbsp;</td>" +
            @"<td style=""border:1px solid" + colorHeader + @";"">&nbsp;</td>" +
            @"<td style=""font-size:30px;border:1px solid " + colorHeader + @"; color:" + colorHeaderText + @";"">Importa</td>" +
            @"<td style=""font-size:30px;border:1px solid " + colorHeader + @";"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + colorHeader + @";"">" +
            @"<td colspan=""5"" style=""border-top:1px solid " + colorHeader + @";border-bottom:1px solid " + colorHeader + @";"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + colorHeader + @";"">" +
            @"<td colspan=""5"" style=""border-top:1px solid " + colorHeader + @";border-bottom:1px solid " + colorHeader + @";"">&nbsp;</td>" +
        @"</tr>" +
        //Modificar
        @"<tr>" +
            @"<td colspan=""5"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align:justify;font-size:15px;"">" + (ce.SEX_ENTI == "M" ? "Estimado " : ce.SEX_ENTI == "F" ? "Estimada " : "") + textInfo.ToTitleCase(ce.FIR_NAME.ToLower()) + @",</td>" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""5"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""3"" style=""text-align:justify;font-size:15px;"">" + objetivo + "</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""5"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""5"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""3"" style=""text-align:center;"" >" +
                @"<a style=""text-decoration:none;"" href=""" + IpServer + @"/Survey/Index?idst=" + Convert.ToString(ID_SURV_TICK) + @"""" + @" target=""_blank"">" +
                    //@"<div style=""background-color:#CB3F37;color:white;padding:0px 0px 0px 0px;margin-left:30%;width:40%;"">" +
                    @"<div style=""display:block;margin-left:auto;margin-left:auto;"">" +
                        @"<img src=""cid:btn-le"" />" +
                    @"</div>" +
                @"</a>" +
            @"</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""5"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""3""></td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""5"">" +
                @"<table width=""100%;"" cellpadding=""0"" border=""0"" cellspacing=""0"">" +
                    @"<tbody>" +
                        @"<tr >" +
                            @"<td style=""width:23px;"">&nbsp;</td>" +
                            @"<td style=""float:right;"">" +
                                @"<a title=""Facebook"" href=""https://www.facebook.com/pages/Grupo-e-data/767481443318728?fref=ts"" target=""_blank""><img style=""margin-right:5px;"" src=""cid:icon-fb"" /></a>" +
                                @"<a title=""LinkedIn"" href=""https://www.linkedin.com/company/electrodata-s.a.c"" target=""_blank""><img style=""margin-right:5px;"" src=""cid:icon-in"" /></a>" +
                                @"<a title=""Issuu"" href=""http://issuu.com/grupoe-data"" target=""_blank""><img style=""margin-right:5px;"" src=""cid:icon-is"" /></a>" +
                                @"<a title=""YouTube"" href=""https://www.youtube.com/channel/UCMw-vM1KwrMOq7U0YkroQkg"" target=""_blank""><img style=""margin-right:5px;"" src=""cid:icon-yt"" /></a>" +
                            @"</td>" +
                            @"<td style=""width:23px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""3"" style=""border-bottom:10px solid #022A41"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</tbody>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +
        @"<tr style=""background-color:#eeeeee;"">" +
            @"<td colspan=""5"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:#eeeeee;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""3"" style=""text-align:justify;font-size:10px; padding:10px 0px 10px 0px;color:#999999;"">Este mensaje se envía de acuerdo con la Ley Nº 28493 D.S. 031-2005 MTC que regula el uso del correo electrónico comercial no solicitado (SPAM), por lo que no puede ser considerado SPAM. En caso no desee recibir mensajes adicionales, por favor&nbsp;" +
            @"<a href=""" + IpServer + @"/Spam/Control?idst=" + Convert.ToString(ID_SURV_TICK) + @""" target=""_blank"" style=""text-decoration:none;color:#CB3F37;"">Click Aqui</a></td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:#eeeeee;"">" +
            @"<td colspan=""5"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
    @"</tbody>" +
@"</table>";
                return msg;
            }
            catch
            {
                return "ERROR";
            }
        }
    }
}