using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleNotification.Models;
using System.Threading;
using System.Globalization;

namespace ConsoleNotification.Templates
{
    class DocumentSaleTemplate
    {    public CoreEntities dbc = new CoreEntities();
         public EntityEntities dbe = new EntityEntities();
         private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
         private TextInfo textInfo;

        public string GenerateTemplateContact(int ID_DOCU_SALE, int ID_ENTI)
        {
            textInfo = cultureInfo.TextInfo;
            string fila="";
            string productos="";
            try
            {
                var query= (from dds in dbc.DETAIL_DOCUMENT_SALE.Where(x=>x.ID_DOCU_SALE==ID_DOCU_SALE)
                            where dds.DAT_INI_SUPP != null
                            where dds.DAT_END_SUPP != null
                            select new
                            {
                                dds.DESC_DETA,
                                dds.DAT_INI_SUPP,
                                dds.DAT_END_SUPP,
                                dds.CODE,

                            }).ToList();

                int test = query.Count();

                var ce = dbe.CLASS_ENTITY.Single(x=>x.ID_ENTI==ID_ENTI);

          string body = @"<table border=""0"" width=""600"" cellpadding=""0"" cellspacing=""0"" align=""center"" style=""font-family: Verdana, Geneva, sans-serif !important; font-size: 12px; font-weight: normal;"">" +
                        @"<tr>" +
                            @"<td>" + (ce.SEX_ENTI == "M" ? @"Estimado " : ce.SEX_ENTI == "F" ? @"Estimada " : @"Estimado(a) ") + textInfo.ToTitleCase(ce.FIR_NAME.ToLower()) + @",</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +                       
                        @"<tr>" +
                              @"<td>Gracias por confiar en nuestra compañía y elegir nuestros productos y/o servicios. Esperamos cada día entregarle mayor valor y apoyo profesional para contribuir al logro de sus objetivos empresariales.</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                         @"<tr>" +
                              @"<td>" +
                                   @"<table align=""center"" border=""1"" style=""border-collapse: collapse; border-spacing: 0; font-family: Verdana, Geneva, sans-serif !important;"">" +
                                       @"<tr>" +
                                           @"<th style=""text-align: center; font-size: 13px; padding: 5px 5px; border-style: solid; border-width: 1px; overflow: hidden; word-break: normal; background-color: #3f51b5; color: #ffffff;"">Descripción</th>" +
                                           @"<th style=""text-align: center; font-size: 13px; padding: 5px 5px; border-style: solid; border-width: 1px; overflow: hidden; word-break: normal; background-color: #3f51b5; color: #ffffff;"">Código</th>" +
                                           @"<th style=""text-align: center; width:70px; font-size: 13px;  border-style: solid; border-width: 1px; overflow: hidden; word-break: normal; background-color: #3f51b5; color: #ffffff;"">Fecha de Inicio</th>" +
                                           @"<th style=""text-align: center; width:70px; font-size: 13px;  border-style: solid; border-width: 1px; overflow: hidden; word-break: normal; background-color: #3f51b5; color: #ffffff;"">Fecha de Fin</th>" +
                                        @"</tr>";

                foreach (var q in query)
                {
                                fila = @"<tr>" +
                                             @"<td style=""font-size: 12px; padding: 10px 10px 10px 10px; border-style: solid; border-width: 1px; overflow: hidden; word-break: normal; "">" + q.DESC_DETA + @"</td>" +
                                             @"<td style=""font-size: 12px; padding: 10px 10px 10px 10px; border-style: solid; border-width: 1px; overflow: hidden; word-break: normal;"">" + q.CODE + @"</td>" +
                                             @"<td style=""width:100px; text-align: center; font-size: 12px; padding: 10px 10px 10px 10px; border-style: solid; border-width: 1px; overflow: hidden; word-break: normal; "">" + String.Format("{0:dd/MM/yy}", q.DAT_INI_SUPP) + @"</td>" +
                                             @"<td style=""width:100px; text-align: center; font-size: 12px; padding: 10px 10px 10px 10px; border-style: solid; border-width: 1px; overflow: hidden; word-break: normal; "">" + String.Format("{0:dd/MM/yy}", q.DAT_END_SUPP) + @"</td>" +
                                          @"</tr>";
                    productos = productos + fila;
                }
                  string footer = @"</table>" +
                              @"</td>" +
                           @"</tr>" +
                            @"<tr>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +
                           @"<tr>" +
                               @"<td>Si tiene alguna consulta y/o sugerencia que nos pueda ayudar a mejorar, no dude en ponerse en contacto con nosotros:</td>" +
                          @"</tr>" +
                          @"<tr>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +
                         @"<tr>" +
                           @"<td>" +
                              @"<table border=""0"" cellspacing=""0"" style=""font-size: 12px;  width:100%;"">" +
                                @"<tr>" +
                                       @"<td colspan=""5""><b>Mesa de Servicio</b></td>" +
                                   @"</tr>" +
                                   @"<tr>" +
                                       @"<td colspan=""5"">&nbsp;</td>" +
                                   @"</tr>" +
                                   @"<tr>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5; width:25%;"">Ubicación</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">:</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">Av. Principal # 456, Surquillo - Lima</td>" +
                                   @"</tr>" +
                                   @"<tr>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;width:25%;"">Teléfono Directo</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">:</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">0800 15163</td>" +
                                   @"</tr>" +
                                   @"<tr>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;width:25%;"">Número de Teléfono</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">:</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">+51 1 4480727</td>" +
                                  @"</tr>" +
                                  @"<tr>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;width:25%;"">Celular</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">:</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">959482718 / #959482718</td>" +
                                   @"</tr>" +
                                   @"<tr>" +
                                       @"<td style=""border-bottom: 1px solid #2D5C88;width:25%;"">Correo Electrónico</td>" +
                                       @"<td style=""border-bottom: 1px solid #2D5C88;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #2D5C88;"">:</td>" +
                                       @"<td style=""border-bottom: 1px solid #2D5C88;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #2D5C88;"">servicedesk@electrodata.com.pe</td>" +
                                   @"</tr>" +
                                   @"<tr>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;width:25%;"">Sitio Web</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">:</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">&nbsp;</td>" +
                                       @"<td style=""border-bottom: 1px solid #3f51b5;"">http://e-data-group.net/</td>" +
                                   @"</tr>" +
                           @"</table>" +
                       @"</td>" +
                   @"</tr>" +
                   @"<tr>" +
                      @"<td>&nbsp;</td>" +
                   @"</tr>" +
                    @"<tr>" +
                      @"<td>" +
                            @"<body>" +
                                 @"<div style=""font-size: 12px;font-family: Verdana;"">Best Regards | Saludos Cordiales,</div><br/>" +
                                       @"<table style=""min-width:300px; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                           @"<tr height=""95px"" >" +
                                                @"<td height=""95px"" style=""width:123px;font-family: Verdana;padding:0px 0px;"">" +
                                                   @"<a href=""http://www.electrodata.com.pe/"" style=""text-decoration:none;"">" +
                                                       @"<table cellspacing=""0"" cellpadding=""0""><tr><td>" +
                                                           @"<font style=""color: #022A41; font-weight: bold; font-size: 18px;text-decoration:none;font-family: Verdana;"">Electro</font><font style=""color: #CB3F37; font-weight: bold; font-size: 18px;font-family: Verdana;"">data</font>" +
                                                       @"</td></tr></table>" +
                                                   @"</a>" +
                                               @"</td>" +
                                               @"<td width=""4px"" style=""padding-left:0px;border-left:1px solid #022A41;width:4px;"">&nbsp;</td>" +
                                               @"<td  style=""min-width:277px;font-family: Verdana;padding:0px 0px;"">" +
                                               @"<font style=""font-size: 12px;font-weight:bold;font-family: Verdana;"">ITMS</font><br/>" +
                                               @"<font style=""font-size: 12px;font-family: Verdana;"">IT Management System | Sistema de Gestión de TI</font><br/>" +
                                               @"<font style=""font-size: 12px;font-family: Verdana;"">itms@electrodata.com.pe</font><br/>" +
                                               @"<font style=""font-size: 12px;font-family: Verdana;"">Office. Office. +51 1 448 0727 | 0800 15163</font><br/>" +
                                               @"<font style=""font-size: 12px;font-family: Verdana;"">Mobile. +51 959482718 | #959482718</font><br/>" +
                                               @"<font style=""font-size: 12px;font-family: Verdana;"">Lima - Perú</font>" +
                                               @"</td>" +
                                           @"</tr>" +
                                       @"</table>" +
                               @"</body>" +
                             @"</td>" +
                       @"</tr>" +
                 @"</table>"; 

                return body+productos+footer;
            }
            catch
            {
                return null;
            }
           
        }
    }
}
