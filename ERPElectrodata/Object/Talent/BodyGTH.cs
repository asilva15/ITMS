using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Models;
using System.Threading;
using System.Globalization;

namespace ERPElectrodata.Object.Talent
{
    public class BodyGTH
    {
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
        DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

        public BodyGTH(){
            textInfo = cultureInfo.TextInfo;
        }

        public string Signature(POST_SIGNATURE_Result pers){

            string words = pers.EMA_ELEC.ToLower(),mail="";
            int i = 0;

            string[] split = words.Split(new Char[] { '@' });

            foreach (string s in split)
            {

                if (s.Trim() != "")
                    if(i==0){
                        mail = @"<span style=""color: #022A41;"">" + s + @"@</span>";
                    }
                    else{
                        mail = mail + @"<span style=""color: #022A41;"">" + s + @"</span>";
                    }
                i++;
                    
            }

            //string BodyHTML = 
            //    @"<div style=""font-family: Verdana;font-size: 12px;"">" +
            //    @"<div style=""font-size: 12px;"">Best Regards | Saludos Cordiales,</div><br/>" +
            //      @"<div style=""height:10px;""></div>" +
            //      @"<div style=""display: table;"">" +
            //        @"<div style=""display: table-cell; vertical-align: middle;text-decoration:none important!;"">" +
            //            @"<div style=""padding-right:10px;text-decoration:none;"">"+
            //                @"<a href=""http://www.electrodata.com.pe/"" style=""text-decoration:none;"" class=""Aqui"">"+
            //                @"<table cellspacing=""0"" cellpadding=""0""><tr><td>" +
            //                    @"<span style=""color: #022A41; font-weight: bold; font-size: 18px;text-decoration:none;"">Electro</span><span style=""color: #CB3F37; font-weight: bold; font-size: 18px;"">data</span>"+
            //                @"</td></tr></table></a></div>" +
            //            //@"<div style=""padding-right:10px;""><div style=""text-decoration:none important!;"" class="" href=""http://www.electrodata.com.pe/""><span style=""color: #022A41; font-weight: bold; font-size: 20px;"">Electro</span><span style=""color: #CB3F37; font-weight: bold; font-size: 20px;"">data</span></div></div>" +
            //         @"</div>" +
            //        @"<div style=""font-size: 12px; color: #022A41; display: table-cell; border-left:1px solid #022A41;"">" +
            //            @"<div style=""padding:0px 0px 0px 10px;"">" +
            //                @"<div><b style=""color: #022A41;"">" + textInfo.ToTitleCase(pers.EMPLOYEE.ToLower()) + @"</b></div>" +
            //                @"<div style=""color: #022A41;"">" + pers.NAM_CHAR_ENGL + @" | " + pers.NAM_CHAR_SPAN + @"</div>" +
            //                @"<div style=""color: #022A41;"" ><table cellspacing=""0"" cellpadding=""0""><tr><td style=""color: #022A41;""><span style=""color: #022A41;""><a style=""color: #022A41;font-size: 12px;"">" + mail + "</a></span></td></tr></table></div>" +
            //                @"<div style=""color: #022A41;"">Office. " + pers.OFFICE + @" – Ext. " + pers.EXT_PERS + @"</div>" +
            //                @"<div style=""color: #022A41;"">Mobile. " + pers.MOBILE + @"</div>" +
            //                @"<div style=""color: #022A41;"">" + textInfo.ToTitleCase(pers.NAM_SITE.ToLower()) + @" - Perú</div>" +
            //            @"</div>" +
            //        @"</div>" +
            //    @"</div>" +
            //@"</div>";
            string body = @"<div style=""font-size: 12px;font-family: Verdana;"">Estimad" + (pers.SEX_ENTI == "M" ? "o" : pers.SEX_ENTI == "F" ? "a" : "o(a)") + "&nbsp;" + textInfo.ToTitleCase(pers.FIR_NAME.ToLower()) + ",</div><br/>" +

                    @"<div style=""font-size: 12px;font-family: Verdana;"">Este año nuestro objetivo principal como grupo, es brindar una nueva imagen hacia nuestros " +
                    @"clientes actuales y potenciales. Por lo cual te hacemos extensiva la post firma que va alineada " +
                    @"con el cambio de nuestras páginas web en toda la región.</div><br/>" +

                    @"<div style=""font-size: 12px;font-family: Verdana;"">Tener en cuenta cambiar tu post firma por la enviada líneas abajo. Nuestra Gerencia General " +
                    @"agradece de antemano tu colaboración.</div><br/>" +

                    @"<div style=""font-weight:bold;font-size: 12px;font-family: Verdana;"">Importante: Por politica corporativa te agradecemos mantener el formato tanto en tamaño, colores y distribución.</div><br/>";
                    
            string BodyHTML = body+@"<body><div style=""font-size: 12px;font-family: Verdana;"">Best Regards | Saludos Cordiales,</div><br/>" +
                @"<table style=""min-width:300px; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr height=""95px"" >" +
                        @"<td height=""95px"" style=""width:123px;font-family: Verdana;padding:0px 0px;"">" +
                            @"<a href=""http://www.electrodata.com.pe/"" style=""text-decoration:none;"" class=""Aqui"">" +
                                @"<table cellspacing=""0"" cellpadding=""0""><tr><td>" +
                                    @"<font style=""color: #022A41; font-weight: bold; font-size: 18px;text-decoration:none;font-family: Verdana;"">Electro</font><font style=""color: #CB3F37; font-weight: bold; font-size: 18px;font-family: Verdana;"">data</font>" +
                                @"</td></tr></table>" +
                            @"</a>" +
                        "</td>" +
                         @"<td width=""4px"" style=""padding-left:0px;border-left:1px solid #022A41;width:4px;"">&nbsp;" +
                         "</td>" +
                        @"<td  style=""min-width:277px;font-family: Verdana;padding:0px 0px;"">" +
                            //@"<div style=""font-size: 12px; color: #022A41; display: table-cell; "">" +
                                //@"<div style=""padding-left:0px;"">" +
                                    //@"<pre>" +
                                    //    @"<li style=""color: #022A41;margin:0px;padding:0px;font-weight:bold;"">" + textInfo.ToTitleCase(pers.EMPLOYEE.ToLower()) + @"</li>" +
                                    //    @"<li style=""color: #022A41;"">" + pers.NAM_CHAR_ENGL + @" | " + pers.NAM_CHAR_SPAN + @"</li>" +
                                    //    @"<div style=""color: #022A41;font-size: 12px;"" ><table cellspacing=""0"" cellpadding=""0""><tr><td style=""color: #022A41;font-size: 12px;""><span style=""color: #022A41;font-size: 12px;""><a style=""color: #022A41;font-size: 12px;"">" + mail + "</a></span></td></tr></table></div>" +
                                    //    @"<div style=""color: #022A41;"">Office. " + pers.OFFICE + @" – Ext. " + pers.EXT_PERS + @"</div>" +
                                    //    @"<span style=""color: #022A41;"">Mobile. " + pers.MOBILE + @"</span>" +
                                    //    @"<div style=""color: #022A41;"">" + textInfo.ToTitleCase(pers.NAM_SITE.ToLower()) + @" - Perú</div>" +
                                    //@"</ul>" +
                                    //@"<pre style=""color: #022A41;margin:0px;padding:0px;font-weight:bold;font-family: Verdana;"">" +
                                        @"<font style=""font-size: 12px;font-weight:bold;font-family: Verdana;"">" + textInfo.ToTitleCase(pers.EMPLOYEE.ToLower()) + "</font><br/>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">" + pers.NAM_CHAR_ENGL + @" | " + pers.NAM_CHAR_SPAN + "</font><br/>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">" + pers.EMA_ELEC.ToLower() + "</font><br/>" +
                                        //@"<div style=""color: #022A41;font-size: 12px;"" ><table cellspacing=""0"" cellpadding=""0""><tr><td style=""color: #022A41;font-size: 12px;""><span style=""color: #022A41;font-size: 12px;""><a style=""color: #022A41;font-size: 12px;"">" + mail + "</a></span></td></tr></table></div>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">Office. " + pers.OFFICE + @" – Ext. " + pers.EXT_PERS + "</font><br/>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">Mobile. " + pers.MOBILE + "</font><br/>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">" + textInfo.ToTitleCase(pers.NAM_SITE.ToLower()) + @" - Perú</font>" +
                                    //@"</pre>" +
                                //@"</div>" +
                            //@"</div>" +
                        "</td>" +
                    @"</tr>" +
                @"</table></body>";

            return BodyHTML;
        }
    }
}