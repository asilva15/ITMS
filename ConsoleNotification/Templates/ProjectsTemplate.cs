using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNotification.Templates
{
    class ProjectsTemplate
    {
        public string WeeklyReportProject()
        {
            string BodyReport="";
            try
            {

                BodyReport = @"<div style=""font-size: 12px;font-family: Verdana;"">" +
                                 @"<div>"+
                                     @"<div>Estimados,</div><br />"+
                                     @"<div>El reporte adjunto tiene como objetivo mostrar todos los proyectos que continúan en ejecución durante el periodo del 29/06 al 05/07 del presente.</div><br />"+
                                     @"<div>Separándolos en 2 grupos:</div>"+
                                     @"<div><ul>"+
                                         @"<li>Top 5 (Proyectos en ejecución con mayor Utilidad Bruta)</li>" +
                                         @"<li>Top 5 (Proyectos en ejecución con menor Utilidad Bruta)</li>" +
                                     @"</ul></div>"+
                                     @"<div>Lo anterior nos permite tener una vista de los proyectos urgentes a los cuales debemos de atender para evitar y/o corregir desviaciones a lo acordado con nuestros clientes.</div><br />"+
                                     @"<div>Adicionalmente en la parte superior derecha del reporte se muestra un acumulado del presente año (desde el 01/01) sobre el total de proyectos ejecutados por ingeniería y operaciones (Cantidad y Monto en Dólares). También se indica la cantidad de personal que se tiene a la fecha en Electrodata Perú.</div><br />"+
                                     @"<div>En la parte superior izquierda se muestra un pie con el top 5 de la distribución de los proyectos por empresa. Esto nos permite saber a cuál de nuestros clientes le estamos proveyendo más productos y/o servicios.</div><br />"+
                                     @"<div>La información mostrada ha sido recabada de nuestros sistemas SIDIGE e ITMS. Además de pasar por la validación de nuestra área de proyectos.</div><br />"+
                                 @"</div>"+
                              @"</div>" +

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
                                    @"<font style=""font-size: 12px;font-family: Verdana;"">Office. +51 1 2713350</font><br/>" +
                                    @"<font style=""font-size: 12px;font-family: Verdana;"">Lima - Perú</font>" +
                                    @"</td>" +
                                @"</tr>" +
                            @"</table>" +
                            @"</body>";
                
                
                return BodyReport;

            }catch
            {
                return null;
            }
           
        }
    }
}
