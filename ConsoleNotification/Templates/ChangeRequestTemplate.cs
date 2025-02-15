using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ConsoleNotification.Models;
using System.Globalization;
using System.Threading;
using ConsoleNotification.Plugins;


namespace ConsoleNotification.Templates
{
    class ChangeRequestTemplate
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


        public ChangeRequestTemplate()
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


        public string CorreoAlerta1Request(string responsable, string actividad, string detalleReq, string colorSolicitudCambio, string msjRegistroSolicitud, string fechaInicio, string fechaFin, string IdCambio)
        {
            string cadena3 = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;background-color:" + colorSolicitudCambio + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:15px;""></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                                @"                        <td style=""text-align:right;font-size:20px;""></td>" +
                                @"                        <td style=""width:15px;""></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + "</div></td>" +
                                @"                        <td></td>" + @"<td></td>" + @"<td></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">GESTIÓN DE CAMBIOS</div></td>" +
                                @"                        <td></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td>&nbsp;</td>" +
                                @"                        <td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +

                                @"        <tr>" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +

                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + colorSolicitudCambio + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Detalle de la Actividad</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Mensaje:</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + msjRegistroSolicitud + @" (Este mensaje es informativo)</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Responsable</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + responsable + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Actividad</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + actividad + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;""><b>Descripción</b></td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">:</td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">" + detalleReq + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Inicio de Actividad</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + fechaInicio + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Fin de Actividad</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + fechaFin + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + colorSolicitudCambio + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Link</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;font-size:14px;font-family:Calibri,""Segoe UI"",Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Resumen Gestión de Cambio</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td><a href=""" + IpPublica + @"/ChangeRequest/ResumenGestionCambio/" + IdCambio + @""">Ver Cambio</a></td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Lista de Solicitudes</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td><a href=""" + IpPublica + @"#/ChangeRequest"">Ver Lista</a></td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + colorSolicitudCambio + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Social Networks</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;""></td>" +
                                @"                        <td style=""width:40px;""><a title=""Facebook"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.facebook.com/pages/Grupo-e-data/767481443318728?fref=ts"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-fb.png"" /></a></td>" +
                                @"                        <td style=""width:40px;""><a title=""LinkedIn"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.linkedin.com/company/electrodata-s.a.c"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-in.png"" /></a></td>" +
                                @"                        <td style=""width:40px;""><a title=""YouTube"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.youtube.com/channel/UCMw-vM1KwrMOq7U0YkroQkg"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-yt.png"" /></a></td>" +
                                @"                        <td ><a style=""border:0px solid white;text-decoration:none;"" title=""Issuu"" href=""http://issuu.com/grupoe-data"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-is.png"" /></a></td>" +
                                @"                        <td style=""width:30px;""></td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:#EAEAEA"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:20px;"" >Gracias por contribuir al Sistema Integrado de Gestión</span></td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"       <tr style=""background-color:#EAEAEA"">" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:#EAEAEA"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:12px;"" >&copy; " + DateTime.Now.Year + " Electrodata All Rights Reserved</span></td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +
                                @"    </table>";
            string body = cadena3;
            return body;
        }

        public string CorreoAlertaFinActividad(string solicitante, string aprobador, string detalleReq, string colorSolicitudCambio, string msjRegistroSolicitud, string fechaInicio, string fechaFin, string IdCambio)
        {
            string cadena3 = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;background-color:" + colorSolicitudCambio + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:15px;""></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                                @"                        <td style=""text-align:right;font-size:20px;""></td>" +
                                @"                        <td style=""width:15px;""></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + "</div></td>" +
                                @"                        <td></td>" + @"<td></td>" + @"<td></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">GESTIÓN DE CAMBIOS</div></td>" +
                                @"                        <td></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td>&nbsp;</td>" +
                                @"                        <td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +

                                @"        <tr>" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +

                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + colorSolicitudCambio + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Detalle de Solicitud</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Mensaje:</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + msjRegistroSolicitud + @" (Este mensaje es informativo)</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Solicitante</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + solicitante + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Aprobador</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + aprobador + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;""><b>Descripción</b></td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">:</td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">" + detalleReq + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Estado</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + "Solicitud Aprobada" + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Fecha de Solicitud</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + fechaInicio + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Fecha de última actividad</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + fechaFin + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + colorSolicitudCambio + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Link</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;font-size:14px;font-family:Calibri,""Segoe UI"",Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Resumen Gestión de Cambio</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td><a href=""" + IpPublica + @"/ChangeRequest/ResumenGestionCambio/" + IdCambio + @""">Ver Cambio</a></td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" + 
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Lista de Solicitudes</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td><a href=""" + IpPublica + @"#/ChangeRequest"">Ver Lista</a></td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + colorSolicitudCambio + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Social Networks</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;""></td>" +
                                @"                        <td style=""width:40px;""><a title=""Facebook"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.facebook.com/pages/Grupo-e-data/767481443318728?fref=ts"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-fb.png"" /></a></td>" +
                                @"                        <td style=""width:40px;""><a title=""LinkedIn"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.linkedin.com/company/electrodata-s.a.c"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-in.png"" /></a></td>" +
                                @"                        <td style=""width:40px;""><a title=""YouTube"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.youtube.com/channel/UCMw-vM1KwrMOq7U0YkroQkg"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-yt.png"" /></a></td>" +
                                @"                        <td ><a style=""border:0px solid white;text-decoration:none;"" title=""Issuu"" href=""http://issuu.com/grupoe-data"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-is.png"" /></a></td>" +
                                @"                        <td style=""width:30px;""></td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:#EAEAEA"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:20px;"" >Gracias por contribuir al Sistema Integrado de Gestión</span></td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"       <tr style=""background-color:#EAEAEA"">" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:#EAEAEA"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:12px;"" >&copy; " + DateTime.Now.Year + " Electrodata All Rights Reserved</span></td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">&nbsp;</td>" +
                                @"        </tr>" +
                                @"    </table>";
            string body = cadena3;
            return body;
        }
    }
}
