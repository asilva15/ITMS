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
    class TalentTemplate
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

        public TalentTemplate()
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

        public string TarjetaCumple(int ID_PERS_ENTI)
        {
            try
            {
                string Location = "REVISE LOCATION",
                       cargo = "VERIFICAR CARGO",
                       UEN = "VERIFICAR UEN";

                var query = (from ce in dbe.CLASS_ENTITY.ToList()
                             join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                             join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                             where pe.ID_PERS_ENTI == ID_PERS_ENTI
                             select new
                             {
                                 Nombre = ((ce.FIR_NAME == null ? "" : ce.FIR_NAME) + " " + (ce.SEC_NAME == null ? "" : ce.SEC_NAME) + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME) + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME)).ToLower(),
                                 Celular = pe.CEL_PERS,
                                 pe.ID_FOTO,
                                 EMA_ELEC = (pe.EMA_ELEC.ToLower()),
                                 Day = ce.BIRTHDAY.Value.Day,
                                 Month = ce.BIRTHDAY.Value.Month,
                                 Rot = ce.SEX_ENTI == null ? "Estimado (a)" : ce.SEX_ENTI == "M" ? "Estimado" : "Estimada"
                             }).First();

                try
                {
                    var contract = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                        .Where(x => x.LAS_CONT == true).First();

                    int ID_CHAR = Convert.ToInt32(contract.ID_CHAR.Value);
                    int ID_PERS_LOCA = Convert.ToInt32(contract.ID_PERS_LOCA.Value);

                    int ID_NAM_CHAR = Convert.ToInt32(dbe.CHARTs.First(x => x.ID_CHAR == ID_CHAR).ID_NAM_CHAR);

                    cargo = textInfo.ToTitleCase(dbe.NAME_CHART.First(x => x.ID_NAM_CHAR == ID_NAM_CHAR).NAM_CHAR);
                    //var uen = dbe.TA_UEN_CARGO(4).First(x => x.ID_CHAR == ID_CHAR);
                    //cargo = textInfo.ToTitleCase(uen.NAM_CHAR.ToLower());
                    //UEN = textInfo.ToTitleCase(uen.UEN.ToLower());

                    try
                    {
                        var PERS_LOCA = dbe.PERSON_LOCATION.Single(x => x.ID_PERS_LOCA == ID_PERS_LOCA);
                        int ID_LOCA = Convert.ToInt32(PERS_LOCA.ID_LOCA);
                        Location = textInfo.ToTitleCase((dbc.LOCATIONs.Single(x => x.ID_LOCA == ID_LOCA).NAM_LOCA).ToLower());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error consiguiendo Location de la persona con ID_PERS_ENTI: " + ID_PERS_ENTI);
                        Console.WriteLine("Error generado: " + e.Message.ToString());
                    }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error Obtiniedo datos (UEN, Cargo, Location) de la persona con ID_PERS_ENTI: " + ID_PERS_ENTI);
                    Console.WriteLine("Error generado: " + e.Message.ToString());
                }

                // rojo : #DE4231
                //naranja: #EB3D00
                //Azul: #002942
                //Plomo: #F1F1F1
                //Letra: #505050

                DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;
                string mes = textInfo.ToTitleCase(dtinfo_es.GetMonthName(query.Month));               
                IpServer = "http://itms.electrodata.com.pe/";
                string Nombre = textInfo.ToTitleCase(Convert.ToString(query.Nombre));

                string body = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;font-family:'Segoe UI';color:#002942;font-size:14px;"">" +
                    "<tr>" +
                        @"<td style=""width:20px;"">&nbsp;</td>" +
                        @"<td style=""width:5px;"">&nbsp;</td>" +
                        @"<td style=""min-width:145px;"">&nbsp;</td>" +
                        @"<td style=""width:20px;"" >&nbsp;</td>" +
                        @"<td style=""width:150px;"">&nbsp;</td>" +
                        @"<td style=""width:20px;"">&nbsp;</td>" +
                        @"<td style=""width:220px;"">&nbsp;</td>" +
                        @"<td style=""width:20px;"">&nbsp;</td>" +
                    "</tr>" +
                    @"<tr style=""background-color:#F1F1F1; font-size:30px;"">" +
                        @"<td style=""background-color:#F1F1F1;padding:20px 0px 20px 0px;""></td>" +
                        @"<td colspan=""6"" style=""background-color:#F1F1F1;padding:20px 0px 20px 0px;"">" + Nombre + "</td>" +
                        @"<td style=""background-color:#F1F1F1;padding:20px 0px 20px 0px;""></td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""8"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td>&nbsp;</td>" +
                        @"<td style=""background-color:#DE4231;"" rowspan=""6"">&nbsp;</td>" +
                        @"<td rowspan=""6"">" +
                            @"<img style=""background-color:#DE4231; width:145px;"" src=""" + IpServer + @"Content/Fotos/" + query.ID_FOTO + @".jpg""/>" +
                        @"</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td style=""font-weight:bold; padding:2px 0px 2px 0px;""></td>" +
                        @"<td style=""padding:2px 0px 2px 0px;""></td>" +
                        @"<td style=""padding:2px 0px 2px 0px;""></td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td style=""font-weight:bold;padding:2px 0px 2px 0px;"">Cargo</td>" +
                        @"<td style=""padding:2px 0px 2px 0px;"">:</td>" +
                        @"<td style=""padding:2px 0px 2px 0px;"">" + cargo + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td style=""font-weight:bold;padding:2px 0px 2px 0px;"">Ubicaci&#243;n</td>" +
                        @"<td style=""padding:2px 0px 2px 0px;"">:</td>" +
                        @"<td style=""padding:2px 0px 2px 0px;"">" + Location + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td style=""font-weight:bold;padding:2px 0px 2px 0px;"">Correo</td>" +
                        @"<td style=""padding:2px 0px 2px 0px;"">:</td>" +
                        @"<td style=""padding:2px 0px 2px 0px;"">" + query.EMA_ELEC + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td style=""font-weight:bold;padding:2px 0px 2px 0px;"">Celular</td>" +
                        @"<td style=""padding:2px 0px 2px 0px;"">:</td>" +
                        @"<td style=""padding:2px 0px 2px 0px;"">" + query.Celular + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""8"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td>&nbsp;</td>" +
                        @"<td colspan=""2"" style=""background-color:#DE4231;font-size:20px;text-align:center;height:140px;"">" +
                            @"<div style=""color:white;font-size:70px; font-weight:300;"">" + Convert.ToString(query.Day) + @"</div>" +
                            @"<div style=""color:white;font-weight:bold;"">" + mes + "</div></td>" +
                        @"<td style=""background-color:#F1F1F1;"">&nbsp;</td>" +
                        @"<td colspan=""3"" style=""background-color:#F1F1F1;font-size:30px;font-weight:bold;text-align: center;"">Feliz Cumplea&#241;os</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""8"">&nbsp;</td>" +
                    @"</tr>" +
                    //@"<tr style=""background-color:#F1F1F1;text-align:justify;font-size:14px;height:70px;"">" +
                    //    @"<td>&nbsp;</td>" +
                    //    @"<td colspan=""6"">Dear Teresa, <br>Electrodata congratulates you in this special " +
                    //        @"day, we thank you for allowing us to grow together with you day by day." +
                    //    @"</td>" +
                    //    @"<td>&nbsp;</td>" +
                    //@"</tr>" +
                    //@"<tr style=""background-color:#F1F1F1;text-align:justify;font-size:14px;"">" +
                    //    @"<td colspan=""8"">&nbsp;</td>" +
                    //@"</tr>" +
                    @"<tr style=""background-color:#F1F1F1;text-align:justify;font-size:14px;height:70px;"">" +
                        @"<td>&nbsp;</td>" +
                        @"<td colspan=""6"">" + query.Rot + " " + Nombre + @", <br>Electrodata te felicita en este día tan especial, " +
                            @"agradecemos el permitirnos crecer junto a ti d&#237;a a d&#237;a." +
                        @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""8"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td>&nbsp;</td>" +
                        @"<td colspan=""4"">" +
                            @"<img src=""" + IpServer + @"Content/Images/Logo.jpg"" /></td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td style=""text-align:right;color:#DE4231;font-size:15px;font-weight:bolder;"">" +
                            @"<a style=""text-align:right;color:#DE4231;font-size:15px;font-weight:bolder; text-decoration:none;"" href=""https://mail.google.com"" target=""_blank"">D&eacute;jale un Mensaje !</a></td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""8"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""8"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>";

                return body;
            }
            catch(Exception e)
            {
                Console.WriteLine("Error Generando el Template para el personal con ID_PERS_ENTI: " + ID_PERS_ENTI);
                Console.WriteLine("Error generado: " + e.Message.ToString());

                return null;
            }           
        }

        public string TemplateExperirationContract(int id = 0, int id1=0,int id2=0)
        {
            //id=ID_PERS_CONT --id1=Cantidad de días --id2=0 Experiración de contrato id2=1: Notificación tres meses

            Organization organiz = new Organization();           
            try
            {
                var qperson = (from pc in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_CONT == id)
                               join pe in dbe.PERSON_ENTITY on pc.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               join pl in dbe.PERSON_LOCATION on pc.ID_PERS_LOCA equals pl.ID_PERS_LOCA 
                               join c in dbe.CHARTs on pc.ID_CHAR equals c.ID_CHAR
                               join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                               where pl.VIG_LOCA == true
                               select new
                               {
                                   pc.ID_PERS_CONT,
                                   pe.ID_PERS_ENTI,
                                   pc.STAR_DATE,
                                   pc.END_DATE,
                                   pc.CODE,
                                   ce.LAS_NAME,
                                   ce.FIR_NAME,
                                   ce.MOT_NAME,
                                   pl.ID_LOCA,  
                                   nc.NAM_CHAR_ENGL,
                                   nc.NAM_CHAR_SPAN,
                               }).ToList();

                var qLoca = dbc.LOCATIONs
                    .Join(dbc.SITEs, x => x.ID_SITE, s => s.ID_SITE, (x, s) => new
                    {
                        x.ID_LOCA,
                        x.NAM_LOCA,
                        x.VIG_LOCA,
                        s.ID_SITE,
                        s.NAM_SITE,
                    }).ToList();

                var query = (from qp in qperson
                             join ql in qLoca on qp.ID_LOCA equals ql.ID_LOCA
                             where ql.VIG_LOCA == true
                             select new
                             {
                                 qp.ID_PERS_CONT,
                                 qp.ID_PERS_ENTI,
                                 STAR_DATE = @String.Format("{0:MM/dd/yyyy}", qp.STAR_DATE),
                                 END_DATE = @String.Format("{0:MM/dd/yyyy}", qp.END_DATE),
                                 qp.CODE,
                                 FIR_NAME = qp.FIR_NAME == null ? "" : qp.FIR_NAME.ToUpper().Substring(0, 1) + qp.FIR_NAME.ToLower().Substring(1, qp.FIR_NAME.Length - 1),
                                 LAS_NAME = qp.LAS_NAME == null ? "" : qp.LAS_NAME.ToUpper().Substring(0, 1) + qp.LAS_NAME.ToLower().Substring(1, qp.LAS_NAME.Length - 1),
                                 MOT_NAME = qp.MOT_NAME == null ? "" : qp.MOT_NAME.ToUpper().Substring(0, 1) + qp.MOT_NAME.ToLower().Substring(1, qp.MOT_NAME.Length - 1),
                                 NAM_LOCA = ql.NAM_LOCA == null ? "" : ql.NAM_LOCA,
                                 NAM_SITE = ql.NAM_SITE == null ? "" : ql.NAM_SITE,
                                 Cargo = qp.NAM_CHAR_SPAN==null? "" :qp.NAM_CHAR_SPAN,
                                 JobTitle = qp.NAM_CHAR_ENGL==null? "" :qp.NAM_CHAR_ENGL,

                             }).First();

                string namesupervisor = "", nameareaspa = "", nameuenspa = "", nameareaeng = "", nameueneng = "";

                try
                {

                    var queryOrga = organiz.chart_staff(query.ID_PERS_ENTI);

                    int ID_PERS_ENTI_SUPE = queryOrga.ID_BOSS;                  
                    int ID_CHAR_AREA = queryOrga.ID_CHAR_AREA;
                    int ID_CHAR_UEN = queryOrga.ID_CHAR_UEN;

                    var supervisor=(from pe in dbe.PERSON_ENTITY.Where(x=>x.ID_PERS_ENTI==ID_PERS_ENTI_SUPE)
                                    join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                    select new
                                    {
                                        FIR_NAME = ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToUpper().Substring(0, 1) + ce.FIR_NAME.ToLower().Substring(1, ce.FIR_NAME.Length - 1),
                                        LAS_NAME = ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToUpper().Substring(0, 1) + ce.LAS_NAME.ToLower().Substring(1, ce.LAS_NAME.Length - 1),
                                    }).First();

                    namesupervisor = supervisor.FIR_NAME + " " + supervisor.LAS_NAME;

                    var area=(from c in dbe.CHARTs.Where(x=>x.ID_CHAR==ID_CHAR_AREA)
                              join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                              select new
                              {
                                nc.NAM_CHAR_ENGL,
                                nc.NAM_CHAR_SPAN,
                              }).First();

                    nameareaspa = area.NAM_CHAR_SPAN;
                    nameareaeng = area.NAM_CHAR_ENGL;

                    var uen = (from c in dbe.CHARTs.Where(x => x.ID_CHAR == ID_CHAR_UEN)
                                join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                                select new
                                {
                                    nc.NAM_CHAR_ENGL,
                                    nc.NAM_CHAR_SPAN,
                                }).First();

                    nameuenspa=uen.NAM_CHAR_SPAN;
                    nameueneng = uen.NAM_CHAR_ENGL;

                }
                catch
                {

                }

                //Colores ITMS
                //Rojo=> #BA141A
                //Amarillo=>#F2BA0C
                //Verde=>#679700
                //Azul=> #2D5C88

                string color = "";
                if (id1 == 7)
                { color = "#BA141A"; }
                else if (id1 == 15)
                { color = "#F2BA0C"; }
                else if (id1 == 30)
                { color = "#679700"; }
                else { color = "#2D5C88"; }

                int dias = id1;
                IpServer = "http://itms.electrodata.com.pe/";

                string titlemsg = "";

                if(id2==1)
                {
                    titlemsg = "Notification Contract Three Months";
                }
                else{
                    titlemsg = "Experiration Contract";
                }

                //---------------Cierra Cabecera------------------------
                string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                @"<tr>" +
                                     @"<td>" +  
                                       @" <table cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width:100%;background-color:" + color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">"+
                                          @"<tr>"+
                                                 @"<td colspan=""2"">&nbsp;</td>"+
                                          @"</tr>"+
                                           @"<tr>" +
                                                 @"<td colspan=""2"">&nbsp;</td>" +
                                          @"</tr>" +
                                          @"<tr>"+
                                                 @"<td style=""text-align:right;font-size:20px;"">"+titlemsg+@"</td>"+
                                                 @"<td style=""width:15px;""></td>"+
                                          @"</tr>"+
                                          @"<tr>"+
                                                 @"<td>"+
                                                    @"<table cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width:100%;background-color:" + color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                                        @"<tr>" +
                                                            @"<td style=""width:85%;"">" +
                                                                @"<table cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width:100%;background-color:" + color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                                                     @"<tr>" +
                                                                        @"<td style=""width:15px;""></td>" +
                                                                        @"<td style=""text-align:left; font-size:18px;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + "</td>" +
                                                                    @"</tr>" +
                                                                    @"<tr>" +
                                                                         @"<td colspan=""2"">&nbsp;</td>" +
                                                                    @"</tr>" +
                                                                    @"<tr>" +
                                                                        @"<td></td>" +
                                                                        @"<td style=""text-align:left; font-size:20px;"">Information Technologies Management System</td>" +
                                                                    @"</tr>" +
                                                                @"</table>" +
                                                            @"</td>" +
                                                            @"<td style=""width:15%;"">" +
                                                                @"<div style=""width:100%;font-size:32px;text-align:right;"">" + dias + "</div>" +
                                                                @"<div style=""width:100%;font-size:14px;text-align:right;"">Days</div>" +
                                                             @"</td>" +
                                                        @"</tr>" +
                                                     @"</table>" +
                                                 @"</td>"+
                                                 @"<td></td>"+
                                           @"</tr>"+
                                           @"<tr>"+
                                                @"<td colspan=""2"">&nbsp;</td>"+
                                            @"</tr>"+
                                             @"<tr>" +
                                                @"<td colspan=""2"">&nbsp;</td>" +
                                            @"</tr>" +
                                        @"</table>"+  
                                 @"</td>" +
                              @"</tr>" +
                    //---------------Cierra Cabecera------------------------

                            @"<tr>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>" +

                     //-------------Version Ingles---------------------------

                            @"<tr>" +
                                @"<td>" +
                                    @"<table style=""width:100%;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                        @"<tr style=""background-color:" + color + @";color:white;"">" +
                                            @"<td style=""width:15px;"">&nbsp;</td>" +
                                            @"<td colspan=""4"">English Version</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td colspan=""5"">&nbsp;</td>" +
                                        @"</tr>" + 
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Employee</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + query.FIR_NAME + " " + query.LAS_NAME + @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Job Title</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>"+query.JobTitle+ @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                        @"</tr>" +                                      
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Boss</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + namesupervisor + @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                        @"</tr>" +
                                         @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Area</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + nameareaeng + @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                         @"</tr>" +
                                         @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Strategic Unit</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + nameueneng + @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +                                       
                                         @"</tr>" +
                                         @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td><b>Start Date</b></td>" +
                                            @"<td>:</td>" +
                                            @"<td>" + query.STAR_DATE + @"</td>" +
                                            @"<td>&nbsp;</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""border-top:1px solid " + color + @";border-bottom:1px solid " + color + @";""><b>End Date</b></td>" +
                                            @"<td style=""border-top:1px solid " + color + @";border-bottom:1px solid " + color + @";"">:</td>" +
                                            @"<td style=""border-top:1px solid " + color + @";border-bottom:1px solid " + color + @";"">" + query.END_DATE + @"</td>" +
                                            @"<td>&nbsp;</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td><b>Site - Location</b></td>" +
                                            @"<td>:</td>" +
                                            @"<td>" + query.NAM_SITE + " - " + query.NAM_LOCA + "</td>" +
                                            @"<td>&nbsp;</td>" +
                                        @"</tr>" +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>" +

                     //-----------------------------Version Español--------------------------------

                            @"<tr>" +
                                @"<td>" +
                                    @"<table style=""width:100%; font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                        @"<tr style=""color:white;background-color:" + color + @""">" +
                                            @"<td style=""width:15px;"">&nbsp;</td>" +
                                            @"<td colspan=""4"">Versi&#243;n en Espa&#241;ol</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td colspan=""5"">&nbsp;</td>" +
                                        @"</tr>" +                                  
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Empleado</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + query.FIR_NAME + " " + query.LAS_NAME + @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Cargo</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + query.Cargo + @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                        @"</tr>" +
                                        @"<tr>" +                                          
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Jefe Inmediato</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + namesupervisor + @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Área</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + nameareaspa+ @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                         @"</tr>" +
                                         @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Unidad Estratégica</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td>" + nameuenspa + @"</td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                         @"</tr>" +
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td><b>Fecha Inicio</b></td>" +
                                            @"<td>:</td>" +
                                            @"<td>" + query.STAR_DATE + @"</td>" +
                                            @"<td>&nbsp;</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""border-top:1px solid " + color + @";border-bottom:1px solid " + color + @";""><b>Fecha Fin</b></td>" +
                                            @"<td style=""border-top:1px solid " + color + @";border-bottom:1px solid " + color + @";"">:</td>" +
                                            @"<td style=""border-top:1px solid " + color + @";border-bottom:1px solid " + color + @";"">" + query.END_DATE + @"</td>" +
                                            @"<td>&nbsp;</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td><b>Sitio - Ubicación</b></td>" +
                                            @"<td>:</td>" +
                                            @"<td>" + query.NAM_SITE + " - " + query.NAM_LOCA + "</td>" +
                                            @"<td>&nbsp;</td>" +
                                        @"</tr>" +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>" +

                            //-------------------------Links-------------------------
                           @"<tr>" +
                                @"<td>" +
                                    @"<table style=""width:100%; font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                        @"<tr style=""color:white; background-color:" + color + @""">" +
                                            @"<td style=""width:15px;"">&nbsp;</td>" +
                                            @"<td colspan=""4"">Links</td>" +
                                        @"</tr>" +
                                        @"<tr>" +
                                            @"<td colspan=""5"">&nbsp;</td>" +
                                        @"</tr>" +                                   
                                        @"<tr>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                            @"<td style=""width:140px;""><b>Administrator Talent</b></td>" +
                                            @"<td style=""width:15px;"">:</td>" +
                                            @"<td><a href=""" + IpServer + @"#/Talent/Edit/"+query.ID_PERS_ENTI+@"/1"">Manage Contract</a></td>" +
                                            @"<td style=""width:30px;"">&nbsp;</td>" +
                                        @"</tr>" +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td>" +
                                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                        @"<tr style=""background-color:" + color + @""">" +
                                            @"<td style=""width:15px;"">&nbsp;</td>" +
                                            @"<td>Social Networks</td>" +
                                        @"</tr>" +

                                        @"<tr>" +
                                            @"<td colspan=""2"">&nbsp;</td>" +
                                        @"</tr>" +
                                    @"</table>" +
                                  @"</td>" +
                              @"</tr>" +

                             @"<tr>" +
                                @"<td>" +
                                    SocialNetwork +
                                @"</td>" +                             
                             @"</tr>" +
                           
                            @"<tr>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td>" +
                                    footer +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>" +

                    @"</table>";
                return header;
            }
            catch
            {
                return null;
            }
        }
  public string AttendanceReportPerson(string SEXO, string NOMBRE)
        {
            try
            {
                string stringattendance = @"<div style=""font-size: 12px;font-family: Verdana;"">" +
                            @"" + (SEXO == "M" ? "Estimado " : "Estimada ") + textInfo.ToTitleCase(NOMBRE) + ",<br /><br />" +
                            @"<div> Te adjuntamos tu reporte de asistencia de trabajo, tener en cuenta que en el reporte adjunto figuran tus marcaciones realizadas en el lector biométrico tanto en nuestras oficinas de San Isidro como en Surquillo.</div> <br />" +
                            @"<div> En caso tu puesto de trabajo esté ubicado en provincia o en las oficinas de nuestro cliente se solicitara a través de tu líder los reportes de asistencia correspondientes.</div> <br />" +
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
                                @"<font style=""font-size: 12px;font-family: Verdana;"">Office. +51 1 2713350 – Ext. 435</font><br/>" +
                                @"<font style=""font-size: 12px;font-family: Verdana;"">Lima - Perú</font>" +
                                @"</td>" +
                            @"</tr>" +
                        @"</table>" +
                        @"</body>";

                return stringattendance;
            }
            catch
            {
                return null;
            }
        }

    }
}
