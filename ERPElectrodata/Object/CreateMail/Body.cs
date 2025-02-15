using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Models;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.IO;

namespace ERPElectrodata.Objects.CreateMail
{
    public class Body
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        private string footer = "";
        private string SocialNetwork = "";
        private string IconPaymentBallot = "";
        private string IconCertificate5th = "";
        private string IconCertificateUtil = "";
        private string IpPublica = "";
        private string IpServer = "https://itms.electrodata.com.pe/";
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public Body()
        {
            IpPublica = ConfigurationManager.AppSettings["IpPublica"].ToString();
            IpServer = ConfigurationManager.AppSettings["IpServer"].ToString();

            textInfo = cultureInfo.TextInfo;

            footer = @"<table style=""width:100%; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr  style=""background-color:#EAEAEA"">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:12px;"" >&copy; " + Convert.ToString(DateTime.Now.Year) + @" Electrodata Todos los Derechos Reservados</span></td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                @"</table>";

            SocialNetwork = @"<table style=""width:100%;"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;""></td>" +
                        @"<td style=""width:40px;""><a title=""Facebook"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.facebook.com/pages/Grupo-e-data/767481443318728?fref=ts"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-fb.png"" /></a></td>" +
                        @"<td style=""width:40px;""><a title=""LinkedIn"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.linkedin.com/company/electrodata-s.a.c"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-in.png"" /></a></td>" +
                        @"<td style=""width:40px;""><a title=""YouTube"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.youtube.com/channel/UCMw-vM1KwrMOq7U0YkroQkg"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-yt.png"" /></a></td>" +
                        @"<td ><a style=""border:0px solid white;text-decoration:none;"" title=""Issuu"" href=""http://issuu.com/grupoe-data"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-is.png"" /></a></td>" +
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

        public string CreateTicket(int id)
        {
            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                          join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                          join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                          join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                          join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                          select new
                          {
                              t.ID_PRIO,
                              t.ID_STAT_END,
                              t.COD_TICK,
                              NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                              p.HOU_PRIO,
                              NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                              NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                              NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                              NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                              NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                              t.ID_PERS_ENTI,
                              t.ID_PERS_ENTI_END,
                              t.ID_PERS_ENTI_ASSI,
                              t.UserId,
                              DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                              CREA = t.FEC_TICK,
                              SCHE = t.FEC_INI_TICK,
                              COL_STAT = e.COL_INDV_STAT,
                              t.ID_DOCU_SALE,
                              t.ID_ACCO,
                          }).First();

            string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                   join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                   select new
                                   {
                                       User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                              (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                   }).First().User;

            string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                    join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                    select new
                                    {
                                        User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                        (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                    }).First().User;

            string CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                               select new
                               {
                                   User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                   (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                               }).First().User;

            DateTime ExpirationDate = Convert.ToDateTime(ticket.DAT_EXPI_TICK);

            //string create_ticket_en = String.Format("{0:}",ticket.CREA);
            string expiration_ticket_en = String.Format("{0:MM/dd/yyyy H:mm}", ticket.DAT_EXPI_TICK);
            string expiration_ticket_es = String.Format("{0:dd/MM/yyyy H:mm}", ticket.DAT_EXPI_TICK);
            string sla = Convert.ToString(ticket.HOU_PRIO);
            string schedule_es = "", schedule_en = "";

            if (ticket.ID_STAT_END == 5)
            {
                schedule_en = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Scheduled Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", ticket.SCHE) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";

                schedule_es = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Programaci&#243;n</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:dd/MM/yyyy H:mm}", ticket.SCHE) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";
            }

            string emp = "";
            string op = "";
            if (ticket.ID_DOCU_SALE.HasValue)
            {
                var qDS = (from a in dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == ticket.ID_DOCU_SALE)
                           join b in dbc.TYPE_DOCUMENT_SALE on a.ID_TYPE_DOCU_SALE equals b.ID_TYPE_DOCU_SALE
                           select new
                           {
                               a.NUM_DOCU_SALE,
                               b.COD_TYPE_DOCU_SALE,
                               a.OS,
                               a.ID_ENTI,
                           });

                int ID_ENTI = qDS.First().ID_ENTI.Value;

                var qCIA = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ID_ENTI);

                emp = textInfo.ToTitleCase(qCIA.COM_NAME.ToLower());

                op = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>" + qDS.First().COD_TYPE_DOCU_SALE + "</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + qDS.First().NUM_DOCU_SALE + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>OS</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + qDS.First().OS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";
            }

            string Company = "";
            if (ticket.ID_ACCO == 4)
            {
                Company = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI).ToList()
                           join ce in dbe.CLASS_ENTITY on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               CIA = textInfo.ToTitleCase(ce.COM_NAME.ToLower()),
                           }).First().CIA;

                Company = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>CIA</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + Company + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>";
            }

            if (ticket.ID_PRIO == 6)
            {
                sla = "Planning";
                expiration_ticket_en = "Planning";
                expiration_ticket_es = "Planning";
            }

            string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%;background-color:" + ticket.COL_STAT + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                    @"<tr>" +
                        @"<td style=""width:15px;""></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td></td>" +
                        @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + (emp.Length > 0 ? emp + " - " : "") + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td>&nbsp;</td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">Information Technologies Management System</div></td>" +
                        @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td>&nbsp;</td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">English Version</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    Company +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Affected User</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + AffectedUser + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Priority</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.NAM_PRIO + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Status</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_STAT + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Category</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CATE + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>SubCategory</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCAT + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Class</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CLAS + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>SubCLass</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Staff Assigned</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + StaffAssigned + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Create By</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + CreateBy + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>SLA</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + " Hours</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Creation Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", ticket.CREA) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    schedule_en +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Expiration Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + expiration_ticket_en + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    op +
                @"</table>" +
            @"</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Versi&#243;n en Espa&#241;ol</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    Company +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Usuario Afectado</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + AffectedUser + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Prioridad</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.NAM_PRIO + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Estado</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_STAT + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Categor&#237;a</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CATE + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Sub Categor&#237;a</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCAT + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Clase</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Sub Clase</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Personal Asignado</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + StaffAssigned + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Creado Por</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + CreateBy + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>SLA</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + @" Horas</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Creaci&#243;n</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:dd/MM/yyyy H:mm}", ticket.CREA) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    schedule_es +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Vencimiento</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + expiration_ticket_es + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    op +
                                @"</table>" +
                            @"</td>" +
                        @"</tr>" +

                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                        @"</tr>" +


@"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Links</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Affected User</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td><a href=""" + IpPublica + @"Ticket/View/" + Convert.ToString(id) + @""">View Ticket Details</a></td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""+width:30px;"">&nbsp;</td>" +
                        @"<td><b>Staff Assigned</b></td>" +
                        @"<td>:</td>" +
                        @"<td><a href=""" + IpPublica + @"#/DetailsTicket/Index/" + Convert.ToString(id) + @""">View Ticket Details</a></td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Social Networks</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                SocialNetwork +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">" +
                footer +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
@"</table>";
            return header;
        }

        //ERP//
        public string TicketCambioCategoria(ObtenerCategoriaAnteriorTicket_Result cateAnterior, ObtenerCategoriaAnteriorTicket_Result categoriaActual, int ID_ACCO, string NombreCabecera)
        {
            string[] categorias = NombreCabecera.Split(',');
            int categoriasLength = categorias.Length;

            string html = $@"
<style>
    .tabla-contenedor {{
        text - align: center;
        font-size: 12px;
    }}
    table {{
        margin: 0 auto;
        border-collapse: collapse;
        width: 80%;
        border-radius: 10px; /* Aplica el border-radius solo a las tablas */
    }}
    th, td {{
        padding: 5px;
        border: 1px solid #000;
        text-align: center; /* Centra el texto en las celdas */

    }}
    th {{
        background-color: #3498db;
        color: #fff;
        white-space: nowrap; /* Evita el ajuste de texto en múltiples líneas */
        min-width: 100px;
    }}
</style>
<p><strong>Actualización de categoría</strong></p>

<div class=""tabla-contenedor"">
    <table border=""2"">
        <tr>
            <th colspan=""{cateAnterior.NIV_CATE}"">Categoría Anterior</th>
        </tr>";

            if (cateAnterior.NIV_CATE == 3)
            {
                html +=
         $@"<tr>
                <th style="" min-width:  100px;;"">{categorias[0]}</th>
                <th style="" min-width:  100px;;"">{categorias[1]}</th>
                <th style="" min-width:  100px;;"">{categorias[2]}</th>
             </tr>
            <tr>
                <td>{cateAnterior.Categoria1}</td>
                <td>{cateAnterior.Categoria2}</td>
                <td>{cateAnterior.Categoria3}</td>
            </tr>";
            }
            else if (cateAnterior.NIV_CATE == 4)
            {
                html +=
         $@"<tr>
                <th style="" min-width:  100px;;"">{categorias[0]}</th>
                <th style="" min-width:  100px;;"">{categorias[1]}</th>
                <th style="" min-width:  100px;;"">{categorias[2]}</th>
                <th style="" min-width:  100px;;"">{categorias[3]}</th>

             </tr>
            <tr>
                <td>{cateAnterior.Categoria1}</td>
                <td>{cateAnterior.Categoria2}</td>
                <td>{cateAnterior.Categoria3}</td>
                <td>{cateAnterior.Categoria4}</td>
            </tr>";
            }
            else if (cateAnterior.NIV_CATE == 5)
            {
                html +=
         $@"<tr>
                <th style="" min-width:  100px;;"">{categorias[0]}</th>
                <th style="" min-width:  100px;;"">{categorias[1]}</th>
                <th style="" min-width:  100px;;"">{categorias[2]}</th>
                <th style="" min-width:  100px;;"">{categorias[3]}</th>
                <th style="" min-width:  100px;;"">{categorias[4]}</th>
             </tr>
            <tr>
                <td>{cateAnterior.Categoria1}</td>
                <td>{cateAnterior.Categoria2}</td>
                <td>{cateAnterior.Categoria3}</td>
                <td>{cateAnterior.Categoria4}</td>
                <td>{cateAnterior.Categoria5}</td>

            </tr>";
            }
            else if (cateAnterior.NIV_CATE == 6)
            {
                html +=
         $@"<tr>
                <th style="" min-width:  100px;;"">{categorias[0]}</th>
                <th style="" min-width:  100px;;"">{categorias[1]}</th>
                <th style="" min-width:  100px;;"">{categorias[2]}</th>
                <th style="" min-width:  100px;;"">{categorias[3]}</th>
                <th style="" min-width:  100px;;"">{categorias[4]}</th>
                <th style="" min-width:  100px;;"">{categorias[5]}</th>
             </tr>
            <tr>
                <td>{cateAnterior.Categoria1}</td>
                <td>{cateAnterior.Categoria2}</td>
                <td>{cateAnterior.Categoria3}</td>
                <td>{cateAnterior.Categoria4}</td>
                <td>{cateAnterior.Categoria5}</td>
                <td>{cateAnterior.Categoria6}</td>
            </tr>";
            }

            html += $@"</table>

    <br/>

    <table border=""2"">
        <tr>
            <th colspan=""{categoriaActual.NIV_CATE}"">Categoría Actual</th>
        </tr>";

            if (categoriaActual.NIV_CATE == 3)
            {
                html +=
         $@" 
                <tr>
                    <th style="" min-width:  100px;;"">{categorias[0]}</th>
                    <th style="" min-width:  100px;;"">{categorias[1]}</th>
                    <th style="" min-width:  100px;;"">{categorias[2]}</th>
                </tr>
                <tr>
                     <td>{categoriaActual.Categoria1}</td>
                    <td>{categoriaActual.Categoria2}</td>
                    <td>{categoriaActual.Categoria3}</td>
                </tr>";
            }
            else if (categoriaActual.NIV_CATE == 4)
            {
                html +=
         $@" 
                <tr>
                     <th style="" min-width:  100px;;"">{categorias[0]}</th>
                     <th style="" min-width:  100px;;"">{categorias[1]}</th>
                     <th style="" min-width:  100px;;"">{categorias[2]}</th>
                     <th style="" min-width:  100px;;"">{categorias[3]}</th>
                </tr>
                <tr>
                     <td>{categoriaActual.Categoria1}</td>
                    <td>{categoriaActual.Categoria2}</td>
                    <td>{categoriaActual.Categoria3}</td>
                    <td>{categoriaActual.Categoria4}</td>
                </tr>";
            }
            else if (categoriaActual.NIV_CATE == 5)
            {
                html +=
         $@" 
                <tr>
                     <th style="" min-width:  100px;;"">{categorias[0]}</th>
                     <th style="" min-width:  100px;;"">{categorias[1]}</th>
                     <th style="" min-width:  100px;;"">{categorias[2]}</th>
                     <th style="" min-width:  100px;;"">{categorias[3]}</th>
                     <th style="" min-width:  100px;;"">{categorias[4]}</th>
                </tr>
                <tr>
                     <td>{categoriaActual.Categoria1}</td>
                    <td>{categoriaActual.Categoria2}</td>
                    <td>{categoriaActual.Categoria3}</td>
                    <td>{categoriaActual.Categoria4}</td>
                    <td>{categoriaActual.Categoria5}</td>

                </tr>";
            }
            else if (categoriaActual.NIV_CATE == 6)
            {
                html +=
         $@" 
                <tr>
                     <th style="" min-width:  100px;;"">{categorias[0]}</th>
                     <th style="" min-width:  100px;;"">{categorias[1]}</th>
                     <th style="" min-width:  100px;;"">{categorias[2]}</th>
                     <th style="" min-width:  100px;;"">{categorias[3]}</th>
                     <th style="" min-width:  100px;;"">{categorias[4]}</th>
                     <th style="" min-width:  100px;;"">{categorias[5]}</th>
                </tr>
                <tr>
                     <td>{categoriaActual.Categoria1}</td>
                    <td>{categoriaActual.Categoria2}</td>
                    <td>{categoriaActual.Categoria3}</td>
                    <td>{categoriaActual.Categoria4}</td>
                    <td>{categoriaActual.Categoria5}</td>
                    <td>{categoriaActual.Categoria6}</td>

                </tr>";
            }


            html +=
       @" </table>
            </div>";
            return html;
        }
        public string CreateDetailsTicket(int ID_DETA_TICK)
        {

            var detail_ticket = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
            int ID_TICK = Convert.ToInt32(detail_ticket.ID_TICK);

            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == ID_TICK).ToList()
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                          join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                          join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                          join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                          join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                          select new
                          {
                              t.ID_STAT,
                              t.ID_PRIO,
                              t.ID_STAT_END,
                              t.COD_TICK,
                              NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                              p.HOU_PRIO,
                              NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                              NAM_SCLAS = scl.NAM_CATE,
                              NAM_CLAS = cl.NAM_CATE,
                              NAM_SCAT = sc.NAM_CATE,
                              NAM_CATE = c.NAM_CATE,
                              t.ID_PERS_ENTI,
                              t.ID_PERS_ENTI_END,
                              t.ID_PERS_ENTI_ASSI,
                              t.UserId,
                              DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                              CREA = t.FEC_TICK,
                              SCHE = t.FEC_INI_TICK,
                              COL_STAT = e.COL_INDV_STAT,
                              EXP_HOUR = p.HOU_PRIO - t.MINUTES / 60
                          }).First();

            string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                   join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                   select new
                                   {
                                       User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                   }).First().User;
            string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                    join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                    select new
                                    {
                                        User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                    }).First().User;

            string CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                               select new
                               {
                                   User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                               }).First().User;

            DateTime ExpirationDate = Convert.ToDateTime(ticket.DAT_EXPI_TICK);

            //string create_ticket_en = String.Format("{0:}",ticket.CREA);
            string expiration_ticket_en = String.Format("{0:MM/dd/yyyy H:mm}", ticket.DAT_EXPI_TICK);
            string expiration_ticket_es = String.Format("{0:dd/MM/yyyy H:mm}", ticket.DAT_EXPI_TICK);
            string sla = Convert.ToString(ticket.HOU_PRIO);
            string schedule_es = "", schedule_en = "", estado_anterior = "";

            //Estado Anterior
            try
            {
                var est_ant = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                    .Where(x => x.ID_DETA_TICK < ID_DETA_TICK)
                    .Where(x => x.ID_STAT != null)
                    .OrderByDescending(x => x.ID_DETA_TICK)
                    .First();

                estado_anterior = textInfo.ToTitleCase(dbc.STATUS.Single(s => s.ID_STAT == est_ant.ID_STAT).NAM_STAT.ToLower());
            }
            catch
            {
                estado_anterior = textInfo.ToTitleCase(dbc.STATUS.Single(s => s.ID_STAT == ticket.ID_STAT).NAM_STAT.ToLower());

            }
            //Fin Estado Anterior

            if (detail_ticket.ID_STAT == 5)
            {
                schedule_en = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Scheduled Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", detail_ticket.FEC_SCHE) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";

                schedule_es = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Programaci&#243;n</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:dd/MM/yyyy H:mm}", detail_ticket.FEC_SCHE) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";
            }

            if (ticket.ID_PRIO == 6)
            {
                sla = "Planning";
                expiration_ticket_en = "Planning";
                expiration_ticket_es = "Planning";
            }

            string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%;background-color:" + ticket.COL_STAT + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                    @"<tr>" +
                        @"<td style=""width:15px;""></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                        @"<td style=""text-align:right;font-size:20px;"">" + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">Information Technologies Management System</div></td>" +
                        @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td>&nbsp;</td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">English Version</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Affected User</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + AffectedUser + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Priority</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.NAM_PRIO + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Previous Status</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + estado_anterior + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Current Status</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_STAT + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Category</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CATE + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>SubCategory</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCAT + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Class</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CLAS + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>SubCLass</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Staff Assigned</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + StaffAssigned + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Create By</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + CreateBy + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>SLA</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + " Hours</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Expiration Hours</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + Convert.ToString(ticket.EXP_HOUR) + @" Hours</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Creation Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", ticket.CREA) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    schedule_en +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Expiration Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + expiration_ticket_en + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Versi&#243;n en Espa&#241;ol</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Usuario Afectado</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + AffectedUser + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Prioridad</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.NAM_PRIO + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Estado Anterior</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + estado_anterior + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Estado Actual</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_STAT + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Categor&#237;a</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CATE + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Sub Categor&#237;a</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCAT + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Clase</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Sub Clase</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Personal Asignado</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + StaffAssigned + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Creado Por</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + CreateBy + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>SLA</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + @" Horas</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Horas para Expiraci&#243n</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + Convert.ToString(ticket.EXP_HOUR) + @" Hours</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Creaci&#243;n</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:dd/MM/yyyy H:mm}", ticket.CREA) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    schedule_es +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Vencimiento</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + expiration_ticket_es + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                                @"</table>" +
                            @"</td>" +
                        @"</tr>" +

                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                        @"</tr>" +


@"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Links</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Affected User</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td><a href=""" + IpPublica + @"Ticket/View/" + Convert.ToString(ID_TICK) + @""">View Ticket Details</a></td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""+width:30px;"">&nbsp;</td>" +
                        @"<td><b>Staff Assigned</b></td>" +
                        @"<td>:</td>" +
                        @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ID_TICK) + @""">View Ticket Details</a></td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Social Networks</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                SocialNetwork +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">" +
                footer +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
@"</table>";
            return header;

        }

        public string TransferTo(int ID_DETA_TICK)
        {

            var detail_ticket = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
            int ID_TICK = Convert.ToInt32(detail_ticket.ID_TICK);

            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == ID_TICK).ToList()
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                          join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                          join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                          join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                          join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                          select new
                          {
                              t.ID_PRIO,
                              t.ID_STAT_END,
                              t.COD_TICK,
                              NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                              p.HOU_PRIO,
                              NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                              NAM_SCLAS = scl.NAM_CATE,
                              NAM_CLAS = cl.NAM_CATE,
                              NAM_SCAT = sc.NAM_CATE,
                              NAM_CATE = c.NAM_CATE,
                              t.ID_PERS_ENTI,
                              t.ID_PERS_ENTI_END,
                              t.ID_PERS_ENTI_ASSI,
                              t.UserId,
                              DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                              CREA = t.FEC_TICK,
                              SCHE = t.FEC_INI_TICK,
                              COL_STAT = e.COL_INDV_STAT,
                              EXP_HOUR = p.HOU_PRIO - t.MINUTES / 60,
                              t.ID_PERS_ENTI_ASSI_STAR,
                              t.MINUTES,
                          }).First();

            string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                   join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                   select new
                                   {
                                       User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                   }).First().User;
            string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                    join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                    select new
                                    {
                                        User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                    }).First().User;

            string CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                               select new
                               {
                                   User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                               }).First().User;

            DateTime ExpirationDate = Convert.ToDateTime(ticket.DAT_EXPI_TICK);

            //string create_ticket_en = String.Format("{0:}",ticket.CREA);
            string expiration_ticket_en = String.Format("{0:MM/dd/yyyy H:mm}", ticket.DAT_EXPI_TICK);
            string expiration_ticket_es = String.Format("{0:dd/MM/yyyy H:mm}", ticket.DAT_EXPI_TICK);
            string sla = Convert.ToString(ticket.HOU_PRIO);
            string schedule_es = "", schedule_en = "", pers_assi_name = "", ult_assi_es = "", ult_assi_en = "",
                transferBy_es = "", transferBy_en = "", exp_hou;
            int ID_ASSI_BEFORE = 0, UserId_Before = 0, minutes = 0;
            DateTime ultima_fecha = Convert.ToDateTime(ticket.CREA);

            //Persona Asignada
            try
            {
                var pers_assi = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                    .Where(x => x.ID_DETA_TICK <= ID_DETA_TICK)
                    .OrderByDescending(x => x.ID_DETA_TICK);
                //.First();

                //Sacar Timer
                if (pers_assi.Where(x => x.ID_STAT != null).Count() >= 1)
                {
                    ultima_fecha = Convert.ToDateTime(pers_assi.Where(x => x.ID_STAT != null).First().CREATE_DETA_INCI);
                    minutes = Convert.ToInt32(ticket.MINUTES);
                }
                else
                {
                    ultima_fecha = Convert.ToDateTime(ticket.CREA);
                    minutes = 0;
                }
                //Fin Timer

                if (pers_assi.Where(x => x.ID_PERS_ENTI != null).Count() <= 1)
                {
                    //pers_assi = pers_assi.First().ID_PERS_ENTI;
                    try
                    {
                        pers_assi_name = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI_STAR).ToList()
                                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                          select new
                                          {
                                              User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                          }).First().User;
                    }
                    catch
                    {

                    }
                }
                else
                {
                    ID_ASSI_BEFORE = Convert.ToInt32(pers_assi.Where(x => x.ID_DETA_TICK != ID_DETA_TICK).First().ID_PERS_ENTI);
                    pers_assi_name = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ID_ASSI_BEFORE).ToList()
                                      join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                      select new
                                      {
                                          User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                      }).First().User;
                }

                if (pers_assi_name != "")
                {
                    ult_assi_en = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Before Responsible</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + pers_assi_name + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";

                    ult_assi_es = @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Responsable Anterior</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + pers_assi_name + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>";
                }
                UserId_Before = Convert.ToInt32(pers_assi.First().UserId);

                string TransferBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == UserId_Before).ToList()
                                     select new
                                     {
                                         User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                     }).First().User;

                transferBy_en = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Transfer By</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + TransferBy + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";

                transferBy_es = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Transferido Por</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + TransferBy + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";
                //
            }
            catch
            {

            }
            //Fin Persona Asignada

            if (ticket.ID_STAT_END == 5)
            {
                var dt_aux = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK && x.ID_STAT == 5)
                    .OrderByDescending(x => x.ID_DETA_TICK)
                    .First();

                schedule_en = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Scheduled Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", dt_aux.FEC_SCHE) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";

                schedule_es = @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Programaci&#243;n</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:dd/MM/yyyy H:mm}", dt_aux.FEC_SCHE) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>";
            }

            if (ticket.ID_PRIO == 6)
            {
                sla = "Planning";
                expiration_ticket_en = "Planning";
                expiration_ticket_es = "Planning";
                exp_hou = "Planning";
            }
            else
            {
                exp_hou = Convert.ToString(Math.Round(Convert.ToDecimal(ticket.HOU_PRIO - ((DateTime.Now.Subtract(ultima_fecha).Days * 24) +
                                            (DateTime.Now.Subtract(ultima_fecha).Hours)
                                            + ((DateTime.Now.Subtract(ultima_fecha).Minutes + minutes) / 60))), 0));
            }

            string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
            @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%;background-color:" + ticket.COL_STAT + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                    @"<tr>" +
                        @"<td style=""width:15px;""></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                        @"<td style=""text-align:right;font-size:20px;"">" + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">Information Technologies Management System</div></td>" +
                        @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td>&nbsp;</td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">English Version</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Affected User</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + AffectedUser + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Priority</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.NAM_PRIO + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Status</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_STAT + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Category</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CATE + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>SubCategory</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCAT + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Class</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CLAS + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>SubCLass</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    transferBy_en +
                    ult_assi_en +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Staff Assigned</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + StaffAssigned + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Create By</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + CreateBy + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>SLA</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + " Hours</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Expiration Hours</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + exp_hou + @" Hours</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Creation Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", ticket.CREA) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    schedule_en +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Expiration Date</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + expiration_ticket_en + "</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Versi&#243;n en Espa&#241;ol</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Usuario Afectado</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + AffectedUser + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Prioridad</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.NAM_PRIO + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Estado</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_STAT + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Categor&#237;a</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CATE + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Sub Categor&#237;a</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCAT + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Clase</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_CLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Sub Clase</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + ticket.NAM_SCLAS + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    transferBy_es +
                    ult_assi_es +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Personal Asignado</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + StaffAssigned + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Creado Por</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + CreateBy + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>SLA</b></td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                        @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + @" Horas</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Horas para Expiraci&#243n</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + exp_hou + @" Hours</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Creaci&#243;n</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + String.Format("{0:dd/MM/yyyy H:mm}", ticket.CREA) + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    schedule_es +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Fecha de Vencimiento</b></td>" +
                        @"<td>:</td>" +
                        @"<td>" + expiration_ticket_es + @"</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                                @"</table>" +
                            @"</td>" +
                        @"</tr>" +

                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                        @"</tr>" +
            @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Links</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Affected User</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td><a href=""" + IpPublica + @"Ticket/View/" + Convert.ToString(ID_TICK) + @""">View Ticket Details</a></td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""+width:30px;"">&nbsp;</td>" +
                        @"<td><b>Staff Assigned</b></td>" +
                        @"<td>:</td>" +
                        @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ID_TICK) + @""">View Ticket Details</a></td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Social Networks</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                    SocialNetwork +
                @"</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""3"">" +
                    footer +
                @"</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
            @"</tr>" +
        @"</table>";

            return header;
        }

        public string TarjetaCumple(int ID_PERS_ENTI)
        {
            string Location = "REVISE LOCATION",
                cargo = "VERIFICAR CARGO",
                UEN = "VERIFICAR UEN";
            //int ID_PERS_ENTI = 0;

            var query = (from ce in dbe.CLASS_ENTITY.ToList()
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                         where pe.ID_PERS_ENTI == ID_PERS_ENTI
                         select new
                         {
                             Nombres = textInfo.ToTitleCase((ce.FIR_NAME + (ce.SEC_NAME != null ? " " + ce.SEC_NAME : "") + " " + ce.LAS_NAME + (ce.MOT_NAME != null ? " " + ce.MOT_NAME : "")).ToLower()),
                             Cargo = "Corporative Development",//textInfo.ToTitleCase(car.NAM_CARG.ToLower()),
                             UnidadNegocio = "Administration & Finances",//textInfo.ToTitleCase(ar2.NOM_AREA.ToLower()),
                             //ID_LOCA = 81,//"Of. Electrodata (San Isidro)",//pl.ID_LOCA,
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


                var uen = dbe.TA_UEN_CARGO(4).First(x => x.ID_CHAR == ID_CHAR);
                cargo = textInfo.ToTitleCase(uen.NAM_CHAR.ToLower());
                UEN = textInfo.ToTitleCase(uen.UEN.ToLower());

                try
                {
                    var PERS_LOCA = dbe.PERSON_LOCATION.Single(x => x.ID_PERS_LOCA == ID_PERS_LOCA);
                    int ID_LOCA = Convert.ToInt32(PERS_LOCA.ID_LOCA);
                    Location = textInfo.ToTitleCase((dbc.LOCATIONs.Single(x => x.ID_LOCA == ID_LOCA).NAM_LOCA).ToLower());
                }
                catch
                {

                }

            }
            catch
            {

            }
            // rojo : #DE4231
            //naranja: #EB3D00
            //Azul: #002942
            //Plomo: #F1F1F1
            DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

            string mes = textInfo.ToTitleCase(dtinfo_es.GetMonthName(query.Month));
            //Letra: #505050
            IpServer = "http://itms.electrodata.com.pe/";
            string nombre = query.Nombres;

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
                                @"<td colspan=""6"" style=""background-color:#F1F1F1;padding:20px 0px 20px 0px;"">" + nombre + "</td>" +
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
                                @"<td style=""font-weight:bold; padding:2px 0px 2px 0px;"">Unidad de Negocio</td>" +
                                @"<td style=""padding:2px 0px 2px 0px;"">:</td>" +
                                @"<td style=""padding:2px 0px 2px 0px;"">" + UEN + "</td>" +
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
                                @"<td colspan=""6"">" + query.Rot + " " + query.Nombres + @", <br>Electrodata te felicita en este d&#237;a tan especial, " +
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
                        @"<a style=""text-align:right;color:#DE4231;font-size:15px;font-weight:bolder; text-decoration:none;"" href=""http://www.zoho.com/mail/login.html"" target=""_blank"">D&eacute;jale un Mensaje !</a></td>" +
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

        public string TicketNotification(int ID_TICK, int ID_TYPE_NOTI)
        {
            try
            {

                var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == ID_TICK).ToList()
                              join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                              join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                              join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                              join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                              join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                              join st in dbc.STATUS on t.ID_STAT_END equals st.ID_STAT
                              select new
                              {
                                  t.COD_TICK,
                                  t.FEC_TICK,
                                  t.ID_AFEC_END_CLIE,
                                  t.ID_PERS_ENTI,
                                  t.ID_PERS_ENTI_END,
                                  SUB_CLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  SUB_CATE = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  NAM_STAT = textInfo.ToTitleCase(st.NAM_STAT.ToLower()),
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  t.UserId,
                                  p.HOU_PRIO,
                                  t.ID_PERS_ENTI_ASSI,
                              }).First();

                string Requester = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI).ToList()
                                    join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                    select new
                                    {
                                        User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                    }).First().User;

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                       }).First().User;

                var Assigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    NAME = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                    Estimado = ce.SEX_ENTI == null ? "Estimado (a)" : ce.SEX_ENTI == "M" ? "Estimado" : "Estimada",
                                    Sexo = ce.SEX_ENTI,
                                }).First();

                //string Estimado_en = "",Estimado_es = "";
                int hours = 0;
                string msg_en = "", msg_es = "", unidad_en = "", unidad_es = "";
                switch (ID_TYPE_NOTI)
                {
                    case 1:
                        hours = Convert.ToInt32(ticket.HOU_PRIO) / 2;
                        msg_en = "Dear " + Assigned.NAME + ", " + Convert.ToString(hours) + " hours have elapsed since the creation of the ticket " + ticket.COD_TICK + ", please follow up!";
                        msg_es = Assigned.Estimado + " " + Assigned.NAME + @", han transcurrido " + Convert.ToString(hours) + " horas desde la creación del boleto " + ticket.COD_TICK + ", favor su seguimiento";
                        unidad_en = "Hours";
                        unidad_es = "Horas";
                        break;
                    case 2:
                        hours = 1;
                        //there is 15 minutes for the ticket HFHF768 SLA to expire
                        msg_en = "Dear " + Assigned.NAME + ", there is " + Convert.ToString(hours) + " hour for the ticket " + ticket.COD_TICK + ", SLA to expire."; ;
                        msg_es = Assigned.Estimado + " " + Assigned.NAME + @", falta " + Convert.ToString(hours) + " hora para que expire el SLA del boleto " + ticket.COD_TICK + "."; ;
                        unidad_en = "Hour";
                        unidad_es = "Hora";
                        break;
                    case 3:
                        hours = 15;
                        //there is 15 minutes for the ticket HFHF768 SLA to expire
                        msg_en = "Dear " + Assigned.NAME + ", there is " + Convert.ToString(hours) + " minutes for the ticket " + ticket.COD_TICK + ", SLA to expire."; ;
                        msg_es = Assigned.Estimado + " " + Assigned.NAME + @", faltan " + Convert.ToString(hours) + " minutos para que expire el SLA del boleto " + ticket.COD_TICK + "."; ;
                        unidad_en = "Minutes";
                        unidad_es = "Minutos";
                        break;
                    case 4:
                        hours = 0;
                        msg_en = "Dear " + Assigned.NAME + ", you have the ticket " + ticket.COD_TICK + " outside SLA"; ;
                        msg_es = Assigned.Estimado + " " + Assigned.NAME + ", tienes el boleto " + ticket.COD_TICK + " fuera de SLA";
                        unidad_en = "Minutes";
                        unidad_es = "Minutos";
                        break;
                    default:
                        hours = 0;
                        msg_en = "Dear " + Assigned.NAME + ", you have the ticket " + ticket.COD_TICK + " outside SLA";
                        msg_es = Assigned.Estimado + " " + Assigned.NAME + ", tienes el ticket " + ticket.COD_TICK + " fuera de SLA";
                        unidad_en = "Minutes";
                        unidad_es = "Minutos";
                        break;
                }

                string html_body =
    @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width: 600px; font-family: 'Segoe UI'; color: #505050; font-size: 14px; "">" +
        "<tr>" +
            @"<td style=""width: 20px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:20px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""text-align:right;font-size:20px;"">" + ticket.COD_TICK + "</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""font-size: 18px;"">" + String.Format("{0:dddd MMMM dd yyyy HH:mm ss}", DateTime.Now) + @"</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""font-size: 22px;"">Information Technologies Management System</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">English Version</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #0CB7E1;"">To Expiration SLA</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background:#0CB7E1;"">&nbsp;Requester:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Category:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;SubClass:</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td rowspan=""6"" style=""font-size: 80px; text-align: center; background-color: #0CB7E1;font-weight: 300;"">" + Convert.ToString(hours) + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + Requester + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.CATE + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.SUB_CLAS + "</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Affected End User:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;SubCategory:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Priority:</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 300; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + AffectedUser + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.SUB_CATE + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.NAM_PRIO + "</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
            @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Create Date:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Class:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Status:</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 300; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #0CB7E1; "">" + unidad_en + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + String.Format("{0:MM/dd/yyyy HH:mm}", ticket.FEC_TICK) + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.CLAS + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.NAM_STAT + "</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""text-align: justify; background: #F1F1F1;font-weight:500;"">" +
                @"<div style=""margin:15px 20px 15px 20px;"">" + msg_en + "</div>" +
            @"</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
          @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Versi&#243;n en Espa&#241;ol</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #0CB7E1; color: white;"">Vencimiento SLA</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background:#0CB7E1; color:white;"">&nbsp;Solicitante:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1; color: white;"">&nbsp;Categor&#237;a:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1; color: white;"">&nbsp;SubClase:</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td rowspan=""6"" style=""font-size: 80px; text-align: center; background-color: #0CB7E1;color:white;font-weight: 300;"">" + Convert.ToString(hours) + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + Requester + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.CATE + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.SUB_CLAS + "</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Usuario Afectado:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;SubCategor&#237;a</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Prioridad:</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + AffectedUser + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.SUB_CATE + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.NAM_PRIO + "</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
            @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Fecha Creaci&#243;n:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Clase:</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;"">&nbsp;Estado:</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align:center;background-color:#0CB7E1;"">" + unidad_es + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background:#0CB7E1;padding-left:5px;"">" + String.Format("{0:dd/MM/yyyy HH:mm}", ticket.FEC_TICK) + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background:#0CB7E1;padding-left:5px;"">" + ticket.CLAS + "</td>" +
            @"<td>&nbsp;</td>" +
            @"<td style=""background: #0CB7E1;padding-left:5px;"">" + ticket.NAM_STAT + "</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""text-align: justify; background: #F1F1F1; font-weight: 500;"">" +
                @"<div style=""margin:15px 20px 15px 20px;"">" + msg_es + "</div>" +
            @"</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Social Network</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">" +
                SocialNetwork +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">" +
                footer +
            @"</td>" +
        @"</tr>" +
    @"</table>";
                return html_body;
            }
            catch
            {
                return null;
            }
        }

        public string PaymentBallotsNotification(int ID_INVO, string Person, String Mes, String Anio, int NumberMes = 1)
        {
            string MesSpanish = "Enero";
            if (NumberMes == 1) { MesSpanish = "Enero"; }
            else if (NumberMes == 2) { MesSpanish = "Febrero"; }
            else if (NumberMes == 3) { MesSpanish = "Marzo"; }
            else if (NumberMes == 4) { MesSpanish = "Abril"; }
            else if (NumberMes == 5) { MesSpanish = "Mayo"; }
            else if (NumberMes == 6) { MesSpanish = "Junio"; }
            else if (NumberMes == 7) { MesSpanish = "Julio"; }
            else if (NumberMes == 8) { MesSpanish = "Agosto"; }
            else if (NumberMes == 9) { MesSpanish = "Septiembre"; }
            else if (NumberMes == 10) { MesSpanish = "Octubre"; }
            else if (NumberMes == 11) { MesSpanish = "Noviembre"; }
            else { MesSpanish = "Diciembre"; }

            try
            {

                string html_body =
    @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width: 600px; font-family: 'Segoe UI'; color: #505050; font-size: 14px; "">" +
        "<tr>" +
            @"<td style=""width: 20px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:20px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""text-align:right;font-size:20px;"">Payment Ballot</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""font-size: 18px;"">" + String.Format("{0:dddd MMMM dd yyyy HH:mm ss}", DateTime.Now) + @"</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""font-size: 22px;"">Information Technologies Management System</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">English Version</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color:#00acec;"">" + Mes + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #00acec;"">" + IconPaymentBallot + "</td>" +
            @"<td colspan=""7"" style=""color:#808080; padding-left:5px;"">Dear " + Person + ", your payment ballot of " + Mes + " " + Anio + " has been registered successfully.</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #00acec;"">" + Anio + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
          @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Versi&#243;n en Espa&#241;ol</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #00acec;"">" + MesSpanish + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #00acec;"">" + IconPaymentBallot + "</td>" +
            @"<td colspan=""7"" style=""color:#808080; padding-left:5px;"">Estimado " + Person + ", se ha registrado su boleta de pago de " + MesSpanish + " del " + Anio + ".</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #00acec;"">" + Anio + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Social Network</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">" +
                SocialNetwork +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">" +
                footer +
            @"</td>" +
        @"</tr>" +
    @"</table>";

                return html_body;
            }
            catch
            {

                return null;
            }
        }

        public string Certicate5thNotification(int ID_INVO, string Person, String Mes, String Anio, int NumberMes = 1)
        {
            string MesSpanish = "Enero";
            if (NumberMes == 1) { MesSpanish = "Enero"; }
            else if (NumberMes == 2) { MesSpanish = "Febrero"; }
            else if (NumberMes == 3) { MesSpanish = "Marzo"; }
            else if (NumberMes == 4) { MesSpanish = "Abril"; }
            else if (NumberMes == 5) { MesSpanish = "Mayo"; }
            else if (NumberMes == 6) { MesSpanish = "Junio"; }
            else if (NumberMes == 7) { MesSpanish = "Julio"; }
            else if (NumberMes == 8) { MesSpanish = "Agosto"; }
            else if (NumberMes == 9) { MesSpanish = "Septiembre"; }
            else if (NumberMes == 10) { MesSpanish = "Octubre"; }
            else if (NumberMes == 11) { MesSpanish = "Noviembre"; }
            else { MesSpanish = "Diciembre"; }

            try
            {

                string html_body =
    @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width: 600px; font-family: 'Segoe UI'; color: #505050; font-size: 14px; "">" +
        "<tr>" +
            @"<td style=""width: 20px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:20px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""text-align:right;font-size:20px;"">5th Category Rent Certificate</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""font-size: 18px;"">" + String.Format("{0:dddd MMMM dd yyyy HH:mm ss}", DateTime.Now) + @"</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""font-size: 22px;"">Information Technologies Management System</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">English Version</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color:#7cbb00;"">" + Mes + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + IconCertificate5th + "</td>" +
            @"<td colspan=""7"" style=""color:#808080; padding-left:5px;"">Dear " + Person + ", your 5th Category Rent Certificate has been registered successfully.</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + Anio + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
          @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Versi&#243;n en Espa&#241;ol</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + MesSpanish + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + IconCertificate5th + "</td>" +
            @"<td colspan=""7"" style=""color:#808080; padding-left:5px;"">Estimado " + Person + ", se ha registrado su Certificado de Renta de 5ta Categoría.</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + Anio + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Social Network</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">" +
                SocialNetwork +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">" +
                footer +
            @"</td>" +
        @"</tr>" +
    @"</table>";

                return html_body;
            }
            catch
            {

                return null;
            }
        }

        public string CerticateUtilities(int ID_INVO, string Person, String Mes, String Anio, int NumberMes = 1)
        {
            string MesSpanish = "Enero";
            if (NumberMes == 1) { MesSpanish = "Enero"; }
            else if (NumberMes == 2) { MesSpanish = "Febrero"; }
            else if (NumberMes == 3) { MesSpanish = "Marzo"; }
            else if (NumberMes == 4) { MesSpanish = "Abril"; }
            else if (NumberMes == 5) { MesSpanish = "Mayo"; }
            else if (NumberMes == 6) { MesSpanish = "Junio"; }
            else if (NumberMes == 7) { MesSpanish = "Julio"; }
            else if (NumberMes == 8) { MesSpanish = "Agosto"; }
            else if (NumberMes == 9) { MesSpanish = "Septiembre"; }
            else if (NumberMes == 10) { MesSpanish = "Octubre"; }
            else if (NumberMes == 11) { MesSpanish = "Noviembre"; }
            else { MesSpanish = "Diciembre"; }

            try
            {

                string html_body =
    @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width: 600px; font-family: 'Segoe UI'; color: #505050; font-size: 14px; "">" +
        "<tr>" +
            @"<td style=""width: 20px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:4px;"">&nbsp;</td>" +
            @"<td style=""width:137px;"">&nbsp;</td>" +
            @"<td style=""width:20px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""text-align:right;font-size:20px;"">Certificate Utilities</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""font-size: 18px;"">" + String.Format("{0:dddd MMMM dd yyyy HH:mm ss}", DateTime.Now) + @"</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""7"" style=""font-size: 22px;"">Information Technologies Management System</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #F1F1F1;"">" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">English Version</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color:#7cbb00;"">" + Mes + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + IconCertificateUtil + "</td>" +
            @"<td colspan=""7"" style=""color:#808080; padding-left:5px;"">Dear " + Person + ", your Certificate Utilies has been registered successfully.</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + Anio + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
          @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Versi&#243;n en Espa&#241;ol</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + MesSpanish + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + IconCertificateUtil + "</td>" +
            @"<td colspan=""7"" style=""color:#808080; padding-left:5px;"">Estimado " + Person + ", se ha registrado su Certificado de Utilidades.</td>" +
        @"</tr>" +
        @"<tr style=""font-weight: 500; color: white;"">" +
            @"<td>&nbsp;</td>" +
            @"<td style=""text-align: center; background-color: #7cbb00;"">" + Anio + "</td>" +
            @"<td colspan=""7"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color: #808080;color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Social Network</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">" +
                SocialNetwork +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""9"">" +
                footer +
            @"</td>" +
        @"</tr>" +
    @"</table>";

                return html_body;
            }
            catch
            {

                return null;
            }
        }

        public string CreateDocumentSale(int ID_DOCU_SALE)
        {
            //var DOCU_SALE = dbc.DOCUMENT_SALE.Single(x => x.ID_DOCU_SALE == ID_DOCU_SALE);
            //DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
            //DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;
            CultureInfo ci = new CultureInfo("es-PE");
            string plomo = "#f0f0f0";
            string azul = "#3f51b5";
            string blanco = "white";
            string colorLetra = "#585f69";
            string bordePlomo = "1px solid #e6e9ee";
            string bordeAzul = "1px solid #3f51b5";
            string vendor = "", quedanDias = "";
            //string solorLetraStrong = "#585f69" 

            textInfo = cultureInfo.TextInfo;

            var cia = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1).ToList();

            var per = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 2).ToList();

            var query = (from ds in dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == ID_DOCU_SALE).ToList()
                         join tds in dbc.TYPE_DOCUMENT_SALE on ds.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
                         join e in cia on ds.ID_COMP equals e.ID_ENTI
                         join fc in cia on ds.ID_COMP_END equals fc.ID_ENTI
                         join c in per on ds.ID_CTTS equals c.ID_ENTI into lc
                         from x in lc.DefaultIfEmpty()
                         select new
                         {
                             NUM_DOCU_SALE = ds.NUM_DOCU_SALE,
                             COD_TYPE_DOCU_SALE = tds.COD_TYPE_DOCU_SALE,
                             OS = ds.OS,
                             EXP_DATE = ds.EXP_DATE,
                             NAM_COMP = e.COM_NAME,
                             NAM_COMP_END = fc.COM_NAME,
                             CONTACT = (x == null ? "" : x.FIR_NAME + " " + (x.LAS_NAME != null ? x.LAS_NAME : "")),
                             ID_DOCU_SALE = ds.ID_DOCU_SALE,
                             ID_PERS_ENTI_VEND = ds.ID_PERS_ENTI_VEND
                         }).First();

            try
            {
                TimeSpan ts = Convert.ToDateTime(query.EXP_DATE) - DateTime.Now;

                quedanDias = Convert.ToString(Decimal.Round(Convert.ToDecimal(ts.TotalDays), 0));

                var pers = dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == query.ID_PERS_ENTI_VEND.Value).FirstOrDefault();
                var vend = dbe.CLASS_ENTITY.Where(ce => ce.ID_ENTI == pers.ID_ENTI2).FirstOrDefault();
                vendor = textInfo.ToTitleCase(vend.FIR_NAME.ToLower() + " " + vend.LAS_NAME.ToLower());
            }
            catch
            {
                return "ERROR";
            }


            string html_body = null;

            string text_es = "", text_en = "";

            try
            {

                text_en = @"Dear Service Desk, please generate an attention on ITMS for the " + query.COD_TYPE_DOCU_SALE + " " + query.NUM_DOCU_SALE + ", the top date to accomplish this is " + String.Format("{0:MM/dd/yyyy}", query.EXP_DATE);
                text_es = @"Se acaba de cargar una nueva orden de pedido (" + query.COD_TYPE_DOCU_SALE + " " + query.NUM_DOCU_SALE.Trim() + "), " +
                    @"en el lapso de una hora se asignar&aacute; a una de nuestras lineas operativas de servicio " +
                    @"(Redes, Virtualizaci&oacute;n, Seguridad o Infraestructura) con la finalidad de iniciar el proceso" +
                    @" de entrega y/o implementaci&oacute;n.";
            }
            catch
            {
                return null;
            }

            html_body = @"<table width=""600px"" cellspacing=""0"" cellpadding=""1"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width: 600px; font-family: 'Segoe UI'; color: #505050; font-size: 14px; "">" +
    @"<tr>" +
        @"<td style=""width: 20px;"">&nbsp;</td>" +
        @"<td style=""width:52px;"">&nbsp;</td>" +
        @"<td style=""width:135px;"" >&nbsp;</td>" +
        @"<td style=""width:4px;"">&nbsp;</td>" +
        @"<td style=""width:8px;"">&nbsp;</td>" +
        @"<td style=""width:52px;"">&nbsp;</td>" +
        @"<td style=""width:135px;"">&nbsp;</td>" +
        @"<td style=""width:4px;"" >&nbsp;</td>" +
        @"<td style=""width:20px;"">&nbsp;</td>" +
    @"</tr>" +
    @"<tr style=""background-color: " + plomo + @";"">" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +
    @"<tr style=""background-color: " + plomo + @"; "">" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align:right;font-size:20px;color:" + colorLetra + @";"">" + query.COD_TYPE_DOCU_SALE + " " + query.NUM_DOCU_SALE + @"</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    @"<tr style=""background-color: " + plomo + @";"">" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +
                //@"<tr style=""background-color: " + plomo + @";"">" +
                //    @"<td>&nbsp;</td>" +
                //    //@"<td colspan=""7"" style=""font-size: 18px;"">" + String.Format("{0:dddd MMMM dd yyyy HH:mm ss}", DateTime.Now) + @"</td>" +
                //    @"<td colspan=""7"" style=""font-size: 18px;"">" + textInfo.ToTitleCase(DateTime.Now.ToString("dddd dd MMMM HH:mm",ci)) + @"</td>" +
                //    @"<td>&nbsp;</td>" +
                //@"</tr>" +
                //@"<tr style=""background-color: " + plomo + @";"">" +
                //    @"<td colspan=""9"">&nbsp;</td>" +
                //@"</tr>" +
    @"<tr style=""background-color: " + plomo + @";"">" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""font-size: 22px;color:" + colorLetra + @";"">Nueva Orden de Pedido</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    @"<tr style=""background-color:" + plomo + @";"">" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +
    @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr style=""text-align: center; background: " + azul + @"; color: white;"">" +
        @"<td colspan=""1"" style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"<td colspan=""8"" style=""padding: 4px 0px 4px 0px; text-align: left; background: " + azul + @"; color: white;"">Descripci&oacute;n</td>" +
    @"</tr>" +

    @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr style="" color: white;"">" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Document.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @";border:" + bordePlomo + @"; color: " + colorLetra + @";"">&nbsp;" + query.COD_TYPE_DOCU_SALE + " " + query.NUM_DOCU_SALE + @"<br />&nbsp;OS " + query.OS + "</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Stop-Clock.png"" />" +
        @"</td>" +
        @"<td style=""background-color: " + "#f34235;font-weight:bold;" + @";border:" + "1px solid #f34235;" + @"; color: " + "white" + @";"">&nbsp;Fecha de Entrega<br />&nbsp;" + String.Format("{0:dd/MM/yy}", query.EXP_DATE) + @" Queda (" + quedanDias + " d&iacute;as" + @")</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +

    @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr style="" color: white;"">" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Industry.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @";"">" +
        @"<div style=""margin:0px 0px 0px 4px;"">" + query.NAM_COMP + (query.NAM_COMP == query.NAM_COMP_END ? "" : "<br />" + query.NAM_COMP_END) + "</div>" +
        @"</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Customer.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @";"">&nbsp;Cliente<br />&nbsp;" + query.CONTACT + "</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +

    @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr style="" color: white;"">" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Salesman.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @";"">&nbsp;Vendedor<br />&nbsp;" + vendor + "</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Network-Earth.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @""">&nbsp;Enlace<br/>&nbsp;<a style=""font-weight:bold;text-decoration:none; color:" + colorLetra + @""" href=""http://itms.electrodata.com.pe/OrderForm/Details/" + Convert.ToString(ID_DOCU_SALE) + @""">Ver OP</a></td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +

    @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""6"" style=""text-align: justify; background: " + plomo + @"; font-size: 14px;font-weight:500;"">" +
            @"<div style=""margin:15px 5px 15px 10px;color:" + colorLetra + @""">" +
                text_es + @"<br /><br />" +
                @"<div style=""font-weight:bold;"">Criterios de Asignaci&oacute;n:</div>" +
                @"<ul style=""margin:0px 10px 0px 10px;padding:0px 10px 0px 10px;color:" + colorLetra + @""">" +
                    @"<li>OPVN (Renovaciones y Soportes)</li>" +
                    @"<li>OPVP (Productos)</li>" +
                    @"<li>OPVP (Productos + Servicios)</li>" +
                @"</ul><br />" +
                @"Para poder visualizar la orden de pedido de click en el enlace Ver OP" +
            @"</div>" +
        @"</td>" +
        @"<td style=""background: " + plomo + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +

    @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +
    @"<tr style=""background-color: " + azul + @";color:white;"">" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
            @"<td colspan=""7"" style=""padding: 4px 0px 4px 0px;"">Redes Sociales</td>" +
            @"<td style=""padding: 4px 0px 4px 0px;"">&nbsp;</td>" +
        @"</tr>" +
    @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
    @"</tr>" +
    @"<tr>" +
            @"<td colspan=""9"">" +
                SocialNetwork +
            @"</td>" +
    @"</tr>" +
    @"<tr>" +
            @"<td colspan=""9"">&nbsp;" +
            @"</td>" +
    @"</tr>" +
    @"<tr>" +
            @"<td colspan=""9"">" +
                footer +
            @"</td>" +
    @"</tr>" +
    @"</table>";


            return html_body;
        }

        public string AttendanceReport(int ID_PERS_ENTI, string DES_QUEU)
        {
            string fecha_letra = String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now);
            DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
            DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

            string nom_month_en = dtinfo_en.GetMonthName(DateTime.Now.Month);
            string nom_month_es = dtinfo_es.GetMonthName(DateTime.Now.Month);
            string num_day = Convert.ToString(DateTime.Now.Day);
            string num_year = Convert.ToString(DateTime.Now.Year);

            var query = (from ce in dbe.CLASS_ENTITY.ToList()
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                         where pe.ID_PERS_ENTI == ID_PERS_ENTI
                         select new
                         {
                             Nombres = textInfo.ToTitleCase((ce.FIR_NAME + (ce.SEC_NAME != null ? " " + ce.SEC_NAME : "") + " " + ce.LAS_NAME + (ce.MOT_NAME != null ? " " + ce.MOT_NAME : "")).ToLower()),
                             Rot = ce.SEX_ENTI == null ? "Estimado (a)" : ce.SEX_ENTI == "M" ? "Estimado" : "Estimada"
                         }).First();

            string body_html = @"<table align=""center"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width:600px;font-family:'segoe ui';color:#002942;font-size:14px;border:0px dotted gray"">" +

             @"<tr>" +
                @"<td colspan=""4"" style=""background-color: #F1F1F1; text-align: right; font-size: 20px;padding:20px 20px 0px 20px;"">Attendance Report</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""4"" style=""background-color: #F1F1F1; text-align:left; font-size: 18px;padding:20px 20px 0px 20px;"">" + textInfo.ToTitleCase(fecha_letra) + "</td>" +
            @"</tr>" +

            @"<tr>" +
               @"<td colspan=""4"" style=""background-color:#F1F1F1; text-align: left; font-size: 22px;padding:20px 20px 20px 20px;"">Information Technologies Management System</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""4"" style=""clear:both;"">&nbsp;</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""4"" style=""background-color:#505050;padding:4px 20px 4px 20px; color:white;""2>English Version</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""4"">&nbsp;</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td width=""20px"" style=""border:0px solid blue;""></td>" +
                @"<td style=""background-color: #46a4c1;"">" +
                    @"<table style=""color:white; text-align:center; width:135px;"">" +
                        @"<tr><td>" + nom_month_en + "</td></tr>" +
                        @"<tr><td style=""font-weight:500;font-size:45px"">" + num_day + "</td></tr>" +
                       @"<tr><td>" + num_year + "</td></tr>" +
                    @"</table>" +
                @"</td>" +
                @"<td style=""padding:0px 20px 0px 10px;"">" + "Dear " + query.Nombres + ",<br /> The report has been generated for assistance concerning the queue " + DES_QUEU + ". See the attached report</td>" +

            @"</tr>" +
            @"<tr>" +
                @"<td>&nbsp;</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""4"" style=""background-color: #505050;padding:4px 20px 4px 20px; color:white;"">Versión En Español</td>" +

            @"</tr>" +
            @"<tr>" +
                @"<td>&nbsp;</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td width=""20px"" style=""border:0px solid blue;""></td>" +
                @"<td style=""background-color: #46a4c1;"">" +
                    @"<table style=""color:white; text-align:center; width:135px;"">" +
                        @"<tr><td>" + nom_month_es + "</td></tr>" +
                        @"<tr><td style=""font-weight:500;font-size:45px"">" + num_day + "</td></tr>" +
                        @"<tr><td>" + num_year + "</td></tr>" +
                    @"</table>" +
                @"</td>" +
                @"<td style=""padding:0px 20px 0px 10px;"">" + query.Rot + " " + query.Nombres + ",<br /> Se ha generado el reporte de asistencia referente a " + DES_QUEU + " Ver informe en el documento adjunto</td>" +

            @"</tr>" +
            @"<tr>" +
                @"<td>&nbsp;</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""4"" style=""background-color: #505050;padding:4px 20px 4px 20px; color:white;"">Social Network</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""4"">&nbsp;</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""4"" style=""padding:0px 0px 0px 20px"">" +
                    SocialNetwork +
                @"</td>" +
            @"</tr>" +
        @"</table>";

            return body_html;
        }

        public string AttendanceReportPerson(string SEXO, string NOMBRE)
        {
            string stringattendance = @"<div style=""font-family:verdana; font-size:10pt;"">" + @"<div style=""color:#0000ff;float:left;"">Dear " + textInfo.ToTitleCase(NOMBRE) + "&nbsp;</div>/&nbsp;" +
                @"" + (SEXO == "M" ? "Estimado " : "Estimada ") + textInfo.ToTitleCase(NOMBRE) + ",<br /><br />" +
                @"<div style=""color:#0000ff""> We are attaching your work assitance report, keep in mind that in the attached report are included your markings made on the biometric reader both at our offices in San Isidro and Surquillo.</div>" +
                @"<div> Te adjuntamos tu reporte de asistencia de trabajo, tener en cuenta que en el reporte adjunto figuran tus marcaciones realizadas en el lector biométrico tanto en nuestras oficinas de San Isidro como en Surquillo.</div> <br />" +
                @"<div style=""color:#0000ff""> If your job is located at province or in the offices of our client, we will your assistance reports through your leader.</div>" +
                @"<div> En caso tu puesto de trabajo esté ubicado en provincia o en las oficinas de nuestro cliente se solicitara a través de tu líder los reportes de asistencia correspondientes.</div> <br />" +
                //@"<div> Regards / Saludos,</div> <br />" +
                //@"<div style=""color:#0000ff""> Information Tecnologies Management System</div>" +
                //@"<div> Sistema de Gestión de Tecnologias de Información</div>" +
                //@"<b style=""color:rgb(0, 0, 102);"">Electro</b><b style=""color:rgb(255, 0, 0);"">data</b>" +
            @"</div>"

            +
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
                                        @"<font style=""font-size: 12px;font-weight:bold;font-family: Verdana;"">" + "ITMS" + "</font><br/>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">" + "IT Management System" + @" | " + "Sistema de Gestión de TI" + "</font><br/>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">" + "itms@electrodata.com.pe" + "</font><br/>" +
                //@"<div style=""color: #022A41;font-size: 12px;"" ><table cellspacing=""0"" cellpadding=""0""><tr><td style=""color: #022A41;font-size: 12px;""><span style=""color: #022A41;font-size: 12px;""><a style=""color: #022A41;font-size: 12px;"">" + mail + "</a></span></td></tr></table></div>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">Office. " + "+51 1 2713350" + @" – Ext. " + "435" + "</font><br/>" +
                //@"<font style=""font-size: 12px;font-family: Verdana;"">Mobile. " + pers.MOBILE + "</font><br/>" +
                                        @"<font style=""font-size: 12px;font-family: Verdana;"">" + @"Perú</font>" +
                //@"</pre>" +
                //@"</div>" +
                //@"</div>" +
                        "</td>" +
                    @"</tr>" +
                @"</table></body>";
            return stringattendance;
        }

        public string TarjetaCumpleStaffMes(int mes)
        {
            string Location = "-",
                   cargo = "-",
                   NAM_UEN = "-",
                   NAM_AREA = "-";

            //int mess = DateTime.Now.Month;//.AddMonths(1).Month;

            DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

            string nammes = textInfo.ToTitleCase(dtinfo_es.GetMonthName(mes));

            var location = (from l in dbc.LOCATIONs
                            select new
                            {
                                l.ID_LOCA,
                                l.NAM_LOCA,

                            }).ToList();

            var query = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.BIRTHDAY != null).ToList()
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                         join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                         where pe.ID_PERS_STAT == 1
                         where ae.VIS_TALE == true
                         where ae.ID_ACCO == 4
                         where ce.BIRTHDAY.Value.Month == mes && pc.VIG_CONT == true && pe.ID_ENTI1 == 9
                         select new
                         {
                             ID_PERS_ENTI = pe.ID_PERS_ENTI,
                             Nombres = textInfo.ToTitleCase((ce.FIR_NAME + " " + ce.LAS_NAME + (ce.MOT_NAME != null ? " " + ce.MOT_NAME : "")).ToLower()),
                             Celular = pe.CEL_PERS,
                             ID_PHOTO = pe.ID_FOTO,
                             email = (pe.EMA_ELEC.ToLower()),
                             Day = ce.BIRTHDAY.Value.Day,
                             Month = ce.BIRTHDAY.Value.Month,
                             //Rot = ce.SEX_ENTI == null ? "Estimado(a)" : ce.SEX_ENTI == "M" ? "Estimado" : "Estimada",

                         }).OrderBy(a => a.Day).ToList();

            string cadena2 = null;
            string cade2 = null;

            string cadena1 = @"<table width=""700"" cellspacing=""0"" cellpadding=""0"" border=""0""  align=""center"" style=""background-color: #f1f1f1; width: 700px; font-family: 'segoe ui'; color: #505050; font-size: 14px;"">" +
                                   @"<tr>" +
                                       @"<td style=""background-color:white; width: 20px;"">&nbsp;</td>" +
                                       @"<td style=""background-color: white; width: 320px;"">&nbsp;</td>" +
                                       @"<td style=""background-color: white; width: 20px;"">&nbsp;</td>" +
                                       @"<td style=""background-color: white; width: 320px;"">&nbsp;</td>" +
                                       @"<td style=""background-color: white; width: 20px;"">&nbsp;</td>" +
                                   @"</tr>" +

                                   @"<tr style=""background-color: #f1f1f1;"">" +
                                       @"<td>&nbsp;</td>" +
                                       @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">&nbsp;</td>" +
                                       @"<td>&nbsp;</td>" +
                                    @"</tr>" +

                                   @"<tr style=""background-color: #f1f1f1;"">" +
                                        @"<td colspan=""5"">&nbsp;</td>" +
                                   @"</tr>" +

                                   @"<tr>" +
                                        @"<td>&nbsp;</td>" +
                                       @"<td colspan=""3"" style=""font-size: 22px;"">Cumpleaños del Mes (" + nammes + ") Electrodata</td>" +
                                        @"<td>&nbsp;</td>" +
                                  @"</tr>" +

                                   @"<tr style=""background-color: #f1f1f1;"">" +
                                        @"<td colspan=""5"">&nbsp;</td>" +
                                  @"</tr>" +

                                   @"<tr style=""background-color: #f1f1f1;2"">" +
                                        @"<td>&nbsp;</td>" +
                                       @"<td colspan=""3"" style=""font-size: 22px;"">Information Technologies Management System</td>" +
                                        @"<td>&nbsp;</td>" +
                                   @"</tr>" +

                                   @"<tr style=""background-color: #f1f1f1;"">" +
                                        @"<td colspan=""5"">&nbsp;</td>" +
                                    @"</tr>" +
                                @"<tr>" +
                                  @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                                @"</tr>";

            for (int i = 0; i < query.Count; i++)
            {

                try
                {
                    var datos = dbe.Obtiene_Nombre_UEN_AREA_CARGO(query[i].ID_PERS_ENTI).First();
                    NAM_UEN = textInfo.ToTitleCase(datos.NAME_UEN.ToLower());
                    NAM_AREA = textInfo.ToTitleCase(datos.NAME_AREA.ToLower());
                    cargo = textInfo.ToTitleCase(datos.CARGO.ToLower());
                }
                catch
                {
                }

                string name_mes = textInfo.ToTitleCase(dtinfo_es.GetMonthName(query[i].Month));

                cadena2 = @"<tr>" +
                            @"<td style=""background-color: white;"">&nbsp;</td>" +
                            @"<td colspan=""3"">" +

                            @"<table cellpadding=""0"" cellspacing=""0"" align=""center"" style=""width: 660px;font-family:Calibri;"">" +

                            @"<tr>" +
                                @"<td style=""width: 70px; background-color: #CB3F37; height: 4px;""></td>" +
                                @"<td style=""background-color: #f1f1f1;"" colspan=""6""></td>" +
                                @"<td style=""width: 10px; background-color: #f1f1f1;""></td>" +
                            @"</tr>" +

                            @"<tr style=""background-color: #f1f1f1;"">" +
                                @"<td rowspan=""3"">" +
                                    @"<img style=""background-position: center center;background-size: 65px 65px;border-radius: 50%; box-shadow: 0 0 1px #999 inset, 0 0 10px rgba(0, 0, 0, 0.2) inset; display: inline-block;height: 65px;vertical-align: middle; width: 65px;""" +
                                        @"src=""" + IpPublica + @"Content/Fotos/" + query[i].ID_PHOTO + @".jpg""/>" +
                                @"</td>" +
                                @"<td rowspan=""3"" style=""width: 20px;""></td>" +
                                @"<td style=""height: 15px; line-height: 1; background-color: #f1f1f1; font-weight: 100; font-size: 16px; ""><b style=""color:#022A41;"">" + query[i].Day + " " + name_mes + "</b></td>" +
                                @"<td style=""width: 20px;""></td>" +
                                @"<td style=""background-color: #f1f1f1; color: #ffffff; font-weight: 100; border-bottom: 1px solid #022A41; font-size: 14px;width: 180px; ""><a style=""text-decoration:none;"">" + query[i].email + "</a></td>" +
                                @"<td style=""width: 10px; border-bottom: 1px solid #022A41;""></td>" +
                                @"<td style=""background-color: #f1f1f1; color: black; font-weight: 100; border-bottom: 1px solid #022A41; text-align: right;""><a style=""font-size: 14px; width: 900px; "">" + query[i].Celular + "</a></td>" +
                                @"<td style=""width: 20px; background-color: #f1f1f1; border-bottom: 1px solid #022A41;""></td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td style=""height: 20px; font-size: 22px; color: #022A41; line-height: 1; background-color: #f1f1f1; font-weight: 100; width: 260px;"">" + query[i].Nombres + "</td>" +
                                @"<td colspan=""4"" rowspan=""2"" style=""background-color: #f1f1f1;"">" +
                                @"<table cellpadding=""0"" cellspacing=""0"" style=""font-weight: 100; font-size: 14px; width:100%;"">" +
                                    @"<tr><td style=""text-align:right;"">" + NAM_UEN + "</td></tr>" +
                                    @"<tr><td style=""text-align:right;"" >" + NAM_AREA + "</td></tr>" +
                                    @"<tr><td>&nbsp;</td></tr>" +
                                @"</table>" +
                                @"</td>" +
                                @"<td style=""width: 10px; background-color: #f1f1f1;""></td>" +
                                @"</tr>" +
                                @"<tr>" +
                                @"<td valign=""top"" style=""height: 15px; line-height: 1; background-color: #f1f1f1; font-weight: 100; font-size:14px;"">" + cargo + "</td>" +
                                @"<td style=""width: 10px; background-color: #f1f1f1;""></td>" +
                            @"</tr>" +
                            @"<tr>" +
                                @"<td colspan=""8"" style=""height: 4px; background-color: #CB3F37;""></td>" +
                            @"</tr>" +
                          @"</table>" +

                          @"</td>" +
                          @"<td style=""background-color: white;"">&nbsp;</td>" +
                        @"</tr>" +

                        @"<tr>" +
                           @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                        @"</tr>";

                cade2 = cade2 + cadena2;

            }

            string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +
                                @"<td> <div style=""padding:10px 20px 10px 0px; background-color: #f1f1f1; text-align:center; height:175px;"">" +
                                   @"<img  style="" width:214px; height:155px;""src=""" + IpPublica + @"/Content/Images/HappyBirthday.png"" /></div>" +
                                @"</td>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       @"Estimados colaboradores,<br /><br />" +
                                       @"Electrodata los felicita a todos ustedes en el día de sus cumpleaños, agradecemos el permitirnos crecer junto a ustedes día a día." +
                                   @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +
                               @"<td>" +
                                   @"<img src=""" + IpPublica + @"Content/Images/Logo.jpg"" /></td>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadena1 + cade2 + cadena3;

            return body;

        }

        public string CreaCuerpoObservacionDocumento(REQUEST_EXPENSE re,DOCUMENT_EXPENSE de)
        {
            
            string fecha = String.Format("{0:D}", DateTime.Now);

            PERSON_ENTITY requester = null;
            textInfo = cultureInfo.TextInfo;


            var query = (from rex in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == re.ID_REQU_EXPE).ToList()
                             //join rea in dbc.REQUEST_EXPENSE_ACTIVITY on rex.ID_REQU_EXPE equals rea.ID_REQU_EXPE                    
                         join sre in dbc.STATUS_REQUEST_EXPENSE on rex.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                         join tds in dbc.TYPE_DELI_SUST on rex.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                         select new
                         {
                             rex.ID_PERS_ENTI_REQU,
                             NAM_TYPE_DELI_SUST = tds.NAM_TYPE_DELI_SUST,
                             COD_REQU_EXPE = rex.COD_REQU_EXPE,
                             NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN

                         }).First();

            requester = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == query.ID_PERS_ENTI_REQU.Value);

            var req = (from cer in dbe.CLASS_ENTITY.Where(c => c.ID_ENTI == requester.ID_ENTI2.Value).ToList()
                       select new
                       {
                           namreque = textInfo.ToTitleCase((cer.FIR_NAME.ToLower() + " " + cer.LAS_NAME.ToLower())),
                       }).First();

            string namerequester = req.namreque;

            string prc = dbc.DELIVERY_SUSTAIN.Where(x=> x.ID_DELI_SUST == re.ID_DELI_SUST).Single().NumeroPRC;

            string mensaje = @"<div style=""font-size: 12px;font-family: Verdana;"">" +
                            @"Estimado(a) Sr / Srta " + namerequester + ":<br /><br />" +
                            @"<div>El comprobante ha sido observado por los siguientes motivos: "+ de.Observacion +"</div> <br />" +
                            @"<div>Se adjunta comprobante.</div> <br />" +
                            @"<div>Favor de subsanar a la brevedad posible, ya que no se tendra en cuenta para su respectiva rendición.</div> <br />" +
                            @"<div>Tipo: "+ query.NAM_TYPE_DELI_SUST_SPAN +"</div>" +
                            @"<div>Código: " + query.COD_REQU_EXPE + "</div>"
                            + (prc == null ? "" : @"<div>PRC: " + prc + "</div>" )
                            +
                        @"</div>" +

                        @"<body>" +
                        @"<div style=""font-size: 12px;font-family: Verdana;"">Gracias, saludos.</div><br/>" +
                        @"<table style=""min-width:300px; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"</table>" +
                        @"</body>";

            return mensaje;

        }

        public string CreaCuerpoDocumentoActualizado(REQUEST_EXPENSE re)
        {

            string fecha = String.Format("{0:D}", DateTime.Now);
            textInfo = cultureInfo.TextInfo;

            var query = (from rex in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == re.ID_REQU_EXPE).ToList()
                             //join rea in dbc.REQUEST_EXPENSE_ACTIVITY on rex.ID_REQU_EXPE equals rea.ID_REQU_EXPE                    
                         join sre in dbc.STATUS_REQUEST_EXPENSE on rex.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                         join tds in dbc.TYPE_DELI_SUST on rex.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                         select new
                         {
                             rex.ID_PERS_ENTI_REQU,
                             NAM_TYPE_DELI_SUST = tds.NAM_TYPE_DELI_SUST,
                             COD_REQU_EXPE = rex.COD_REQU_EXPE,
                             NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN

                         }).First();

            string prc = dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == re.ID_DELI_SUST).Single().NumeroPRC;

            string mensaje = @"<div style=""font-size: 12px;font-family: Verdana;"">" +
                            @"Estimados:<br /><br />" +
                            @"<div>El comprobante ha sido actualizado por el usuario.</div> <br />" +
                            @"<div>Se adjunta nuevo comprobante.</div> <br />" +
                            @"<div>Favor de revisar, para su respectiva rendición.</div> <br />" +
                            @"<div>Tipo: " + query.NAM_TYPE_DELI_SUST_SPAN + "</div>" +
                            @"<div>Código: " + query.COD_REQU_EXPE + "</div>"
                            + (prc == null ? "" : @"<div>PRC: " + prc + "</div>")
                            +
                        @"</div>" +

                        @"<body>" +
                        @"<div style=""font-size: 12px;font-family: Verdana;"">Gracias, saludos.</div><br/>" +
                        @"<table style=""min-width:300px; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"</table>" +
                        @"</body>";

            return mensaje;

        }

        public string PettyCash(REQUEST_EXPENSE re)
        {
            //try
            //{
            string color = "#7cbb00";

            string fecha = String.Format("{0:D}", DateTime.Now);
            string namepaga = "";

            PERSON_ENTITY requester = null, approval = null, paga = null;
            textInfo = cultureInfo.TextInfo;


            var query = (from rex in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == re.ID_REQU_EXPE).ToList()
                         //join rea in dbc.REQUEST_EXPENSE_ACTIVITY on rex.ID_REQU_EXPE equals rea.ID_REQU_EXPE                    
                         join sre in dbc.STATUS_REQUEST_EXPENSE on rex.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                         join tds in dbc.TYPE_DELI_SUST on rex.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                         select new
                         {
                             rex.ID_PERS_ENTI_REQU,
                             rex.ID_PERS_ENTI_APPR,
                             rex.ID_PERS_ENTI_ASSI,
                             DRequest = String.Format("{0:d}", rex.DAT_REGI),
                             //DApprove = String.Format("{0:d}", rea.DAT_STAR),
                             rex.AMOUNT,

                             NAM_STAT_REQU_EXPE = sre.NAM_STAT_REQU_EXPE,
                             NAM_STAT_REQU_EXPE_SPAN = sre.NAM_STAT_REQU_EXPE_SPAN,
                             NAM_TYPE_DELI_SUST = tds.NAM_TYPE_DELI_SUST,
                             COD_REQU_EXPE = rex.COD_REQU_EXPE,
                             NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN

                         }).First();

            var col = dbc.STATUS_REQUEST_EXPENSE.First(x => x.ID_STAT_REQU_EXPE == re.ID_STAT_REQU_EXPE);

            color = col.COLOR.ToString();

            string DateRequest = Convert.ToString(query.DRequest);
            string DateApprove = Convert.ToString(query.DRequest);
            string monto = Convert.ToString(query.AMOUNT);


            requester = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == query.ID_PERS_ENTI_REQU.Value);

            var req = (from cer in dbe.CLASS_ENTITY.Where(c => c.ID_ENTI == requester.ID_ENTI2.Value).ToList()
                       select new
                       {
                           namreque = textInfo.ToTitleCase((cer.FIR_NAME.ToLower() + " " + cer.LAS_NAME.ToLower())),
                       }).First();
            string namerequester = req.namreque;

            approval = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == query.ID_PERS_ENTI_APPR.Value);
            var apr = (from cea in dbe.CLASS_ENTITY.Where(c => c.ID_ENTI == approval.ID_ENTI2.Value).ToList()
                       select new
                       {
                           namappr = textInfo.ToTitleCase((cea.FIR_NAME.ToLower() + " " + cea.LAS_NAME.ToLower())),
                       }).First();
            string nameaproval = apr.namappr;

            try
            {
                paga = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == query.ID_PERS_ENTI_ASSI.Value);

                var pagad = (from cep in dbe.CLASS_ENTITY.Where(c => c.ID_ENTI == paga.ID_ENTI2.Value).ToList()
                             select new
                             {
                                 nampaga = textInfo.ToTitleCase((cep.FIR_NAME.ToLower() + " " + cep.LAS_NAME.ToLower())),
                             }).First();

                namepaga = pagad.nampaga;
            }
            catch
            {

            }


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
        @"<td colspan=""4"" style=""text-align: right; font-size: 20px; color: white;"">" + Convert.ToString(query.NAM_TYPE_DELI_SUST) + @" - " +
            Convert.ToString(query.NAM_STAT_REQU_EXPE) + @" " + Convert.ToString(query.COD_REQU_EXPE) + @"</td>" +
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
            @"<td style=""font-weight: bold; font-family:Calibri"">Requester</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + namerequester + "</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Date Request</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + DateRequest + "</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Approves</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"">" + nameaproval + "</td>" +
            @"<td></td>" +
         @"</tr>" +
                // @"<tr>" +
                //    @"<td></td>" +
                //     @"<td></td>" +
                //    @"<td style=""font-weight: bold; font-family: Calibri"">Date Approves</td>" +
                //    @"<td style=""text-align:center;"">:</td>" +
                //    @"<td colspan=""5"" style=""font-family:Calibri;"">" + DateApprove + "</td>" +
                //     @"<td></td>" +
                //@"</tr>" +
                (namepaga == "" ? "" :
       @"<tr>" +
             @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Payer</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + namepaga + "</td>" +
             @"<td></td>" +
     @"</tr>") +

        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @"; font-family: Calibri"">Amount</td>" +
            @"<td style=""text-align:center; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @";"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @";"">S/. " + monto + " Peruvian Nuevo Sol</td>" +
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
            @"<td style=""font-weight: bold; font-family: Calibri"">Solicitante</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + namerequester + "</td>" +
            @"<td></td>" +
        @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Fecha de la Solicitud</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + DateRequest + "</td>" +
             @"<td></td>" +
         @"</tr>" +
                //@"<tr>" +
                //     @"<td></td>" +
                //     @"<td></td>" +
                //    @"<td style=""font-weight: bold; font-family: Calibri"">Fecha de Aprobación</td>" +
                //    @"<td style=""text-align:center;"">:</td>" +
                //    @"<td colspan=""5"" style=""font-family:Calibri;"">" + DateApprove + "</td>" +
                //     @"<td></td>" +
                //@"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Aprueba</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + nameaproval + "</td>" +
             @"<td></td>" +
         @"</tr>" +
         (namepaga == "" ? "" :
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Pagador</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + namepaga + "</td>" +
             @"<td></td>" +
        @"</tr>") +
        @"<tr>" +
            @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @"; font-family: Calibri"">Monto</td>" +
            @"<td style=""text-align:center; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @";"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @";"">S/. " + monto + " Nuevos Soles</td>" +
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
            @"<td colspan=""8"">© 2014 Electrodata All Rights Reserved</td>" +
             @"<td></td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
         @"</tr>" +
    @"</table>";

            return mensaje;
            //}
            //catch
            //{
            //    return "";
            //}

        }

        public string NewDocumentControl(DOCUMENT_CONTROL odc)
        {
            //try
            //{
            string colorBody = "#022A41";
            string color = "#FFFFFF";

            CultureInfo en = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = en;

            string fecha = String.Format("{0:D}", DateTime.Now);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var listCIA = (from ce in dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1)
                           //join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               ID_ENTI = ce.ID_ENTI,
                               COM_NAME = ce.COM_NAME,
                           }).ToList();

            var query = (from dc in dbc.DOCUMENT_CONTROL.Where(x => x.ID_DOCU_CONT == odc.ID_DOCU_CONT).ToList()
                         join cia in listCIA on dc.ID_CIA equals cia.ID_ENTI
                         join td in dbc.DOCUMENT_CONTROL_TYPE on dc.ID_DOCU_CONT_TYPE equals td.ID_DOCU_CONT_TYPE
                         join sd in dbc.DOCUMENT_CONTROL_SUB_TYPE on dc.ID_DOCU_CONT_SUB_TYPE equals sd.ID_DOCU_CONT_SUB_TYPE
                         join pe in dbe.PERSON_ENTITY on dc.ID_PERS_ENTI_REMI equals pe.ID_PERS_ENTI
                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                         select new
                         {
                             NAM_CIA = cia.COM_NAME,
                             NAM_DOCU_CONT_TYPE = td.NAM_DOCU_CONT_TYPE,
                             NUM_DOCU = dc.NUM_DOCU,
                             NAM_DOCU_CONT_SUBT = sd.NAM_DOCU_CONT_SUB_TYPE,
                             DESC = dc.SUB_DOCU,
                             FIR_NAME = ce.FIR_NAME,
                             LAS_NAME = ce.LAS_NAME,
                             FOTO = pe.ID_FOTO
                         }).First();

            string mensaje = @"<table align=""center"" border=""1"" style=""border-collapse: collapse; border-color:#D3D3D3""><tr><td> " +
        @"<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 600px; background-color: white; font-family: 'segoe ui',verdana,sans-serif; font-size: 14px; border:1px"">" +
        @"<colgroup>" +
            @"<col style=""width: 20px"">" +
            @"<col style=""width: 50px"">" +
            @"<col style=""width: 50px"">" +
            @"<col style=""width: 50px"">" +
            @"<col style=""width: 80px"">" +

            @"<col style=""width: 20px"">" +
            @"<col style=""width: 30px"">" +
            @"<col style=""width: 130px"">" +
            @"<col style=""width: 100px"">" +
            @"<col style=""width: 20px"">" +
        @"</colgroup>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td></td>" +
            @"<td colspan=""8""><img src=""http://www.electrodata.com.pe/static_electrodata/img/logo-electrodata.jpg"" /></td>" +
            @"<td></td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + colorBody + @";"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"" style=""font-size:20px; color:white; font-style:bold; text-align:center;""><br>ITMS - Information Technologies Management System<br>&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
        @"</tr>" +
       @"<tr style=""background-color:" + color + @";"">" +
             @"<td></td>" +
            @"<td colspan=""6"" style=""font-size:18px; color:gray; border:1px solid; border-color:#D3D3D3"" height=""25"">&nbsp;&nbsp;&nbsp;CONTROL DE DOCUMENTOS</td>" +
            @"<td colspan=""2"" style=""border:1px solid;  border-color:#D3D3D3; text-align:right;"">" + fecha.ToUpper() + "&nbsp;&nbsp;&nbsp;</td>" +
            @"<td></td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
         @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td>&nbsp;</td>" +
            @"<td rowspan=""2""><img src=http://itms.electrodata.com.pe/Content/Fotos/" + query.FOTO + ".jpg height='100px' width='130px' /></td>" +
            @"<td colspan=""7"" style=""font-family:Calibri; border:1px solid; border-color:#D3D3D3;"">&nbsp;&nbsp;&nbsp;" + query.FIR_NAME + " " + query.LAS_NAME + "</td>" +
             @"<td></td>" +
         @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td></td>" +
            @"<td colspan=""7"" style=""font-family:Calibri; border:1px solid; border-color:#D3D3D3;"">&nbsp;&nbsp;&nbsp;" + query.NAM_CIA.ToUpper() + "</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td></td>" +
            @"<td colspan=""8"" style=""font-family:Calibri; border:1px solid; border-color:#D3D3D3;""><BR>&nbsp;&nbsp;&nbsp;" + query.NAM_DOCU_CONT_TYPE.ToUpper() + " - " + query.NAM_DOCU_CONT_SUBT.ToUpper() + "<BR>&nbsp;</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td></td>" +
            @"<td colspan=""7"" style=""font-family:Calibri; border:1px solid; border-color:#D3D3D3;""><BR>&nbsp;&nbsp;&nbsp;<span style=""font-family:Calibri; font-size:9; color:#D3D3D3;"">DESCRIPCIÓN</span><BR>&nbsp;&nbsp;&nbsp;" + query.DESC + "<BR>&nbsp;</td>" +
            @"<td colspan=""1"" style=""font-family:Calibri; text-align:center; border:1px solid; border-color:#D3D3D3;""><span style=""font-family:Calibri; font-size:9; color:#D3D3D3;"">N° DOC</span><BR>" + query.NUM_DOCU + "</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
         @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td></td>" +
            @"<td colspan=""8"" style=""font-family:Calibri; font-style:italic; color:red; text-align:center;"">Por favor validar el documento en físico.</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr style=""background-color:" + color + @";"">" +
            @"<td>&nbsp;</td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
            @"<td>&nbsp;</td>" +
         @"</tr>" +
         @"<tr style=""background-color:" + colorBody + @"; color:white"">" +
            @"<td></td>" +
            @"<td colspan=""7"">www.electrodata.com.pe</td>" +
            @"<td>" + SocialNetwork + "</td>" +
             @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td colspan=""8"">&nbsp;</td>" +
             @"<td></td>" +
        @"</tr>" +
    @"</table>" +
    @"</td></tr></table>";

            return mensaje;
        }

        public string CodigoRestablecimientoITMS(string nombreUsuario, string cuenta, string token)
        {
            string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       //@"Estimados,<br /><br />" +
                                       @"Estimado/a  " + nombreUsuario + " " + "se ha generado un codigo para reestablecer su contraseña <br /><br />" +
                                       @"Codigo: " + token + "<br /><br />" +
                                       @"Saludos Cordiales.<br /><br />" +
                                   @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +
                               @"<td>" +
                                   @"<img src=""" + IpPublica + @"Content/Images/Logo.jpg"" /></td>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadena3;

            return body;
        }
        public string RestauraUsuarioITMS(string nombreUsuario, string cuenta, string password)
        {
            string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 10px 10px 20px;"">" +
                                       //@"Estimados,<br /><br />" +
                                       @"Estimado/a  " + nombreUsuario + " " + "se ha restablecido tus accesos:<br /><br />" +
                                       @"Usuario: " + cuenta + "<br /><br />" +
                                       @"Contraseña: " + password + "<br /><br />" +
                                       @"Saludos Cordiales.<br /><br />" +
                                   @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""4"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>" +
                                   @"<img src=""" + IpPublica + @"Content/Images/Logo.jpg"" /></td>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""4"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadena3;

            return body;

        }

        public string CuentaUsuarioITMS(string nombreUsuario, string cuenta, string password, string email_personal, string cargo, string lugartrabajo)
        {
            string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       @"Estimados,<br /><br />" +
                                       @"Se ha creado la cuenta de ITMS para " + nombreUsuario + " " + "con los siguientes datos:<br /><br />" +
                                       @"Usuario: " + cuenta + "<br /><br />" +
                                       @"Contraseña: " + password + "<br /><br />" +
                                       @"<a href=""https://itms.electrodata.com.pe/"">Sistema ITMS</a> (dar click aquí para ingresar)<br />" +
                                       @"Correo Personal: " + email_personal + "<br /><br />" +
                                       @"Cargo: " + cargo + "<br /><br />" +
                                       @"Lugar de Trabajo: " + lugartrabajo + "<br /><br />" +
                                       @"<br /><br />" +
                                       @"Por favor actualizar tu contraseña la primera vez que ingreses al sistema.<br /><br />" +
                                       
                                   @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +
                               @"<td>" +
                                   @"<img src=""" + IpPublica + @"Content/Images/Logo.jpg"" /></td>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadena3;

            return body;

        }

        public string CuentaUsuarioExternoITMS(string nombreUsuario, string usuario, string password, string email_personal, string cuenta)
        {
            string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       @"Estimados,<br /><br />" +
                                       @"Se ha creado la cuenta de ITMS para " + nombreUsuario + "<br />" +
                                       "con los siguientes datos: <br /><br />" +
                                       @"Cuenta: " + cuenta + "<br /><br />" +
                                       @"Usuario: " + usuario + "<br /><br />" +
                                       @"Contraseña: " + password + "<br /><br />" +
                                       @"<a href=""https://itms.electrodata.com.pe/"">Sistema ITMS</a> (dar click aquí para ingresar)<br />" +
                                       @"Correo: " + email_personal + "<br /><br />" +
                                       @"<br />" +
                                       @"Por favor actualizar tu contraseña la primera vez que ingreses al sistema.<br />" +
                                       
                                       @"</div>" + 
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +
                               @"<td>" +
                                   @"<img src=""" + IpPublica + @"Content/Images/Logo.jpg"" /></td>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadena3;

            return body;

        }


        public string TranferenciaN3(string sumtik, string comentario, string ticket)
        {
            string cadena3 = @"<tr>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       @"Señores Analista de Infraestructura<br /><br />" +
                                       @"Se ha transferido la actividad <b>" + comentario + "<b><br /><br />" +
                                       @" TICKET: <b>" + ticket+ "<b><br /><br />" +
                                       @" Agradeceriamos su pronta atención. <br /><br />Datos del usuario:<br /><br />" +
                                       @"" + sumtik + "<br /><br />" +
                                       //@"Jefe inmediato: " + email_personal + "<br /><br />" +
                                       @"Agradecemos de antemano su gentil apoyo.<br /><br />" +
                                       @"Saludos.<br />" +
                                       @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +
                               @"<td>" +
                                   @"<img src=""" + IpPublica + @"Content/Images/Logo.jpg"" /></td>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadena3;

            return body;

        }



        //public string SolicitudAprobacionAccesoGTI(string sumtik, string comentario, string estadoHost, string comentarioAdicional, string ticket, string adjunto, int idTick)
        public string SolicitudAprobacionAccesoGTI(string sumtik, string comentario, string estadoHost, string comentarioAdicional, string ticket, List<string> adjuntos, int idTick)
        {
            //string rutaImagen = "http://localhost:22141/Adjuntos/"; // Ruta local, se cambiará por IpPublica

            string imagenesHtml = "";

            foreach (string adjunto in adjuntos)
            {
                string imagenHtml = $@"<img src=""{IpPublica}Adjuntos/{adjunto}"" /><br /><br />";
                imagenesHtml += imagenHtml;
            }

            string enlacesImagen = "";

            foreach (string adjunto in adjuntos)
            {
                string enlaceImagen = $@"<a href=""{IpPublica}Adjuntos/{adjunto}"" target=""_blank"">Haz clic aquí para ver la imagen {adjunto}</a><br /><br />";
                enlacesImagen += enlaceImagen;
            }

            string urlRedireccion = $@"{IpPublica}/PortalUsuarioBuenaventura/Portal/AprobacionGTI?id=" + idTick;
            string btnRechazar = $@"<a href=""{urlRedireccion}"" style=""text-decoration: none; margin-right: 10px;""><button id=""btnRechazar"" style=""background-color: #d92550; color: white; padding: 10px 20px; border: none; cursor: pointer;"">Rechazar</button></a>";
            string btnAprobar = $@"<a href=""{urlRedireccion}"" style=""text-decoration: none;""><button id=""btnAprobar"" style=""background-color: #3ac47d; color: white; padding: 10px 20px; border: none; cursor: pointer;"">Aprobar</button></a><br /><br />";


            string cadenaAprobacionGTI = @"<tr>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       @"Favor su VoBo para proceder con la habilitación del siguiente acceso:<br /><br />" +
                                       @" TICKET: <b>" + ticket + "<b><br /><br />" +
                                       @"Datos de la solicitud: <b>" + comentario + "<b><br /><br />" +
                                       @"Estado de salud de Host: <b>" + estadoHost + "<b><br /><br />" +
                                       @"Comentario u observación: <b>" + comentarioAdicional + "<br /><br />" +
                                       imagenesHtml +
                                       enlacesImagen +
                                       btnRechazar + btnAprobar +
                                       @"Agradecemos de antemano su gentil apoyo.<br /><br />" +
                                       @"Saludos.<br />" +
                                       @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +

                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +
                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadenaAprobacionGTI;

            return body;

        }
        public string SolcucionAccesoUSB(string sumtik, string comentario, string solucion, string comentarioAdicional, string ticket, List<string> adjuntos, int idTick)
        {
            string imagenesHtml = "";

            foreach (string adjunto in adjuntos)
            {
                string imagenHtml = $@"<img src=""{IpPublica}Adjuntos/{adjunto}"" /><br /><br />";
                imagenesHtml += imagenHtml;
            }

            string enlacesImagen = "";

            foreach (string adjunto in adjuntos)
            {
                string enlaceImagen = $@"<a href=""{IpPublica}Adjuntos/{adjunto}"" target=""_blank"">Haz clic aquí para ver la imagen {adjunto}</a><br /><br />";
                enlacesImagen += enlaceImagen;
            }

            //string urlRedireccion = "http://localhost:33269/Portal/AprobacionGTI?id=" + idTick;

            string urlRedireccionPRD = $@"https://""{IpPublica}/PortalUsuarioBuenaventura/Portal/AprobacionGTI?id=" + idTick;

            string btnRechazar = $@"<a href=""{urlRedireccionPRD}"" style=""text-decoration: none; margin-right: 10px;""><button id=""btnRechazar"" style=""background-color: #d92550; color: white; padding: 10px 20px; border: none; cursor: pointer;"">Rechazar</button></a>";
            string btnAprobar = $@"<a href=""{urlRedireccionPRD}"" style=""text-decoration: none;""><button id=""btnAprobar"" style=""background-color: #3ac47d; color: white; padding: 10px 20px; border: none; cursor: pointer;"">Aprobar</button></a><br /><br />";

            string cadenaAprobacionGTI = @"<tr>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       @"Se brindó el acceso USB solicitado:<br /><br />" +
                                       @" TICKET: <b>" + ticket + "<b><br /><br />" +
                                       @"Datos de la solicitud: <b>" + comentario + "<b><br /><br />" +
                                       @"Comentario de solución: <b>" + solucion + "<b><br /><br />" +
                                       imagenesHtml +
                                       enlacesImagen +
                                       @"Saludos.<br />" +
                                       @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +

                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +
                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadenaAprobacionGTI;

            return body;

        }

        public string CorreoChangeRequest(string nombreUsuario, string aprobador, string detalleReq, string colorSolicitudCambio, string msjRegistroSolicitud, string status, string fechaRegistro, string IdCambio)
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
                                @"                        <td><b>Requerimiento:</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + msjRegistroSolicitud + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Solicitante</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + nombreUsuario + @"</td>" +
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
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;""><b>Detalle</b></td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">:</td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">" + detalleReq + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Estado</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + status + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Fecha</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + fechaRegistro + @"</td>" +
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
                                @"                        <td><a href=""" + IpPublica + @"#/ResumenGestionCambio/" + IdCambio + @""">Ver Cambio</a></td>" +
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

        public string CorreoChangeRequestMinsur(string nombreUsuario, string aprobador, string detalleReq, string colorSolicitudCambio, string msjRegistroSolicitud, string status, string fechaRegistro, string IdCambio)
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
                                @"                        <td><b>Requerimiento:</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + msjRegistroSolicitud + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Solicitante</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + nombreUsuario + @"</td>" +
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
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;""><b>Detalle</b></td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">:</td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">" + detalleReq + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Estado</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + status + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Fecha</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + fechaRegistro + @"</td>" +
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
                                @"                        <td><a href=""" + IpPublica + @"#/ResumenGestionCambioMinsur/" + IdCambio + @""">Ver Cambio</a></td>" +
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

        public string CorreoChangeRequestBNV(string nombreUsuario, string aprobador, string detalleReq, string colorSolicitudCambio, string msjRegistroSolicitud, string status, string fechaRegistro, string IdCambio)
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
                                @"                        <td><b>Requerimiento:</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + msjRegistroSolicitud + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Solicitante</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + nombreUsuario + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Responsable</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + aprobador + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;""><b>Detalle</b></td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">:</td>" +
                                @"                        <td style=""border-top:1px solid #2b5797;border-bottom:1px solid #2b5797;"">" + detalleReq + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Estado</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + status + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Fecha</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + fechaRegistro + @"</td>" +
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
                                @"                        <td><a href=""" + IpPublica + @"#/ResumenGestionCambioBNV/" + IdCambio + @""">Ver Cambio</a></td>" +
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
        public string CorreoRejectRequest(string nombreUsuario)
        {
            string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       @"Estimado,<br /><br />" +
                                       @" " + nombreUsuario + " " + "<br /><br />" +
                                       @"Su Solicitud de Cambio ha sido Rechazada. Por favor contactese con su jefe inmediato para cualquier consulta. <br /><br />" +
                                       @"GRACIAS POR CONTRIBUIR AL SISTEMA INTEGRADO DE GESTION <br /><br />" +
                                   @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +
                               @"<td>" +
                                   @"<img src=""" + IpPublica + @"Content/Images/Logo.jpg"" /></td>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadena3;

            return body;
        }

        public string CorreoApprovedRequest(string nombreUsuario)
        {
            string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +
                                @"<td style=""background-color: white;"">&nbsp;</td>" +

                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 0px 10px 20px;"">" +
                                       @"Estimado,<br /><br />" +
                                       @" " + nombreUsuario + " " + "<br /><br />" +
                                       @"Su Solicitud de Cambio ha sido Aprobada.<br /><br />" +
                                       @"GRACIAS POR CONTRIBUIR AL SISTEMA INTEGRADO DE GESTION <br /><br />" +
                                   @"</div>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                          @"</tr>" +

                           @"<tr>" +
                               @"<td colspan=""5"" style=""background-color: white;"">&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                               @"<td>&nbsp;</td>" +
                               @"<td>" +
                                   @"<img src=""" + IpPublica + @"Content/Images/Logo.jpg"" /></td>" +
                               @"</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                               @"<td>&nbsp;</td>" +
                           @"</tr>" +

                           @"<tr style=""background-color: white;"">" +
                             @"<td colspan=""5"">&nbsp;</td>" +
                           @"</tr>" +
                   @"</table>";

            string body = cadena3;

            return body;
        }

        public string CreateDetailsActivity(int UserId, ACTIVITY_LOG act_log, string nombreUsuario)
        {
            var nombreEmpresa = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == act_log.ID_CLIE).COM_NAME;
            var tipoActividad = dbe.TYPE_ACT_LOG.Single(x => x.ID_TYPE_ACT == act_log.ID_TYPE_ACT).DES_ACT;
            string subtipoActividad = string.Empty;
            if (act_log.ID_TYPE_ACT != 8)
            {
                subtipoActividad = dbe.SUBTYPE_ACTITY.Single(x => x.ID_SUBTYPE_ACT == act_log.ID_SUBTYPE_ACT).DES_SUB_TYPE;
            }

            var detalleSubtipoActividad = act_log.NAM_SUBTYPE_ACT;
            var nombreContacto = act_log.CONTACTO;
            var fechaInicio = act_log.DATE_INIC;
            var fechaFin = act_log.DATE_END;
            var comentario = act_log.COMENTARIO;

            string Mensaje = string.Empty;
            Mensaje = @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Especialista</span></b><span style=""font-size:9.5pt"">: " + nombreUsuario + "</span><u></u><u></u></p>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Nombre de la Empresa</span></b><span style=""font-size:9.5pt"">: " + nombreEmpresa + "&nbsp;<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Nombre del contacto</span></b><span style=""font-size:9.5pt"">: " + nombreContacto + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Fecha y Hora del Inicio:</span></b><span style=""font-size:9.5pt"">&nbsp;" + fechaInicio + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Fecha y Hora del Fin</span></b><span style=""font-size:9.5pt"">: " + fechaFin + "<u></u><u></u></span></p>" +
                             @"</div>" +
                              @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Tipo de Actividad</span></b><span style=""font-size:9.5pt"">:&nbsp;" + tipoActividad + "<u></u><u></u></span></p>" +
                             @"</div>" +
                              @"<div>" +
                             @"     <p><b><span style=""font-size:9.5pt"">" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(subtipoActividad.ToLower()) + "</span></b>" +
                             @"     <span style=""font-size:9.5pt"">:&nbsp;" + detalleSubtipoActividad + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Información breve del trabajo</span></b><span style=""font-size:9.5pt"">:&nbsp;" + comentario + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><span style=""font-size:9.5pt""><u></u>&nbsp;<u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><u></u>&nbsp;<u></u></p>" +
                             @"</div>" +
                     @"</div>";
            string body = Mensaje;
            return Mensaje;
        }
        public string CreateDetailsEditActivity(int UserId, ACTIVITY_LOG act_log, string nombreUsuario, string nombreUsuarioEdit)
        {
            var nombreEmpresa = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == act_log.ID_CLIE).COM_NAME;
            var tipoActividad = dbe.TYPE_ACT_LOG.Single(x => x.ID_TYPE_ACT == act_log.ID_TYPE_ACT).DES_ACT;
            string subtipoActividad = string.Empty;
            if (act_log.ID_TYPE_ACT != 8)
            {
                subtipoActividad = dbe.SUBTYPE_ACTITY.Single(x => x.ID_SUBTYPE_ACT == act_log.ID_SUBTYPE_ACT).DES_SUB_TYPE;
            }
            var detalleSubtipoActividad = act_log.NAM_SUBTYPE_ACT;
            var nombreContacto = act_log.CONTACTO;
            var fechaInicio = act_log.DATE_INIC;
            var fechaFin = act_log.DATE_END;
            var comentario = act_log.COMENTARIO;
            var motivo = act_log.MOTIVO_EDIT;

            string Mensaje = string.Empty;
            Mensaje = @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Actualizado por:</span></b><span style=""font-size:9.5pt""> " + nombreUsuarioEdit + "</span><u></u><u></u></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Motivo de la Corrección</span></b><span style=""font-size:9.5pt"">: " + motivo + "</span><u></u><u></u></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Especialista</span></b><span style=""font-size:9.5pt"">: " + nombreUsuario + "</span><u></u><u></u></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Nombre de la Empresa</span></b><span style=""font-size:9.5pt"">: " + nombreEmpresa + "&nbsp;<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Nombre del contacto</span></b><span style=""font-size:9.5pt"">: " + nombreContacto + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Fecha y Hora del Inicio:</span></b><span style=""font-size:9.5pt"">&nbsp;" + fechaInicio + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Fecha y Hora del Fin</span></b><span style=""font-size:9.5pt"">: " + fechaFin + "<u></u><u></u></span></p>" +
                             @"</div>" +
                              @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Tipo de Actividad</span></b><span style=""font-size:9.5pt"">:&nbsp;" + tipoActividad + "<u></u><u></u></span></p>" +
                             @"</div>" +
                              @"<div>" +
                             @"     <p><b><span style=""font-size:9.5pt"">" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(subtipoActividad.ToLower()) + "</span></b>" +
                             @"     <span style=""font-size:9.5pt"">:&nbsp;" + detalleSubtipoActividad + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Información breve del trabajo</span></b><span style=""font-size:9.5pt"">:&nbsp;" + comentario + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><span style=""font-size:9.5pt""><u></u>&nbsp;<u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><u></u>&nbsp;<u></u></p>" +
                             @"</div>" +
                     @"</div>";
            string body = Mensaje;
            return Mensaje;
        }

        public string CreateDetailsCloseTicketActivity(int UserId, ACTIVITY_LOG act_log, string nombreUsuario)
        {
            var nombreEmpresa = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == act_log.ID_CLIE).COM_NAME;
            var tipoActividad = dbe.TYPE_ACT_LOG.Single(x => x.ID_TYPE_ACT == act_log.ID_TYPE_ACT).DES_ACT;
            var subtipoActividad = dbe.SUBTYPE_ACTITY.Single(x => x.ID_SUBTYPE_ACT == act_log.ID_SUBTYPE_ACT).DES_SUB_TYPE;
            var detalleSubtipoActividad = act_log.NAM_SUBTYPE_ACT;
            var nombreContacto = act_log.CONTACTO;
            var fechaInicio = act_log.DATE_INIC;
            var fechaFin = act_log.DATE_END;
            var comentario = act_log.COMENTARIO;

            string Mensaje = string.Empty;
            Mensaje = @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Datos de Ticket para realizar encuesta de Calidad:</span><u></u><u></u></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Especialista</span></b><span style=""font-size:9.5pt"">: " + nombreUsuario + "</span><u></u><u></u></p>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Nombre de la Empresa</span></b><span style=""font-size:9.5pt"">: " + nombreEmpresa + "&nbsp;<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Nombre del contacto</span></b><span style=""font-size:9.5pt"">: " + nombreContacto + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Fecha y Hora del Inicio:</span></b><span style=""font-size:9.5pt"">&nbsp;" + fechaInicio + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Fecha y Hora del Fin</span></b><span style=""font-size:9.5pt"">: " + fechaFin + "<u></u><u></u></span></p>" +
                             @"</div>" +
                              @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Tipo de Actividad</span></b><span style=""font-size:9.5pt"">:&nbsp;" + tipoActividad + "<u></u><u></u></span></p>" +
                             @"</div>" +
                              @"<div>" +
                             @"     <p><b><span style=""font-size:9.5pt"">" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(subtipoActividad.ToLower()) + "</span></b>" +
                             @"     <span style=""font-size:9.5pt"">:&nbsp;" + detalleSubtipoActividad + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Información breve del trabajo</span></b><span style=""font-size:9.5pt"">:&nbsp;" + comentario + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><span style=""font-size:9.5pt""><u></u>&nbsp;<u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><u></u>&nbsp;<u></u></p>" +
                             @"</div>" +
                     @"</div>";
            string body = Mensaje;
            return Mensaje;
        }

        public string CreateDetailsScheduledTicketActivity(int UserId, ACTIVITY_LOG act_log, string nombreUsuario, DateTime DATE_SCHEDULED)
        {
            var nombreEmpresa = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == act_log.ID_CLIE).COM_NAME;
            var tipoActividad = dbe.TYPE_ACT_LOG.Single(x => x.ID_TYPE_ACT == act_log.ID_TYPE_ACT).DES_ACT;
            var subtipoActividad = dbe.SUBTYPE_ACTITY.Single(x => x.ID_SUBTYPE_ACT == act_log.ID_SUBTYPE_ACT).DES_SUB_TYPE;
            var detalleSubtipoActividad = act_log.NAM_SUBTYPE_ACT;
            var nombreContacto = act_log.CONTACTO;
            var fechaInicio = act_log.DATE_INIC;
            var fechaFin = act_log.DATE_END;
            var comentario = act_log.COMENTARIO;
            var fechaReprogramada = DATE_SCHEDULED;

            string Mensaje = string.Empty;
            Mensaje = @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Fecha de Reprogramación:</span></b><span style=""font-size:9.5pt""> " + fechaReprogramada + "</span><u></u><u></u></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Especialista</span></b><span style=""font-size:9.5pt"">: " + nombreUsuario + "</span><u></u><u></u></p>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Nombre de la Empresa</span></b><span style=""font-size:9.5pt"">: " + nombreEmpresa + "&nbsp;<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Nombre del contacto</span></b><span style=""font-size:9.5pt"">: " + nombreContacto + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Fecha y Hora del Inicio:</span></b><span style=""font-size:9.5pt"">&nbsp;" + fechaInicio + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Fecha y Hora del Fin</span></b><span style=""font-size:9.5pt"">: " + fechaFin + "<u></u><u></u></span></p>" +
                             @"</div>" +
                              @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Tipo de Actividad</span></b><span style=""font-size:9.5pt"">:&nbsp;" + tipoActividad + "<u></u><u></u></span></p>" +
                             @"</div>" +
                              @"<div>" +
                             @"     <p><b><span style=""font-size:9.5pt"">" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(subtipoActividad.ToLower()) + "</span></b>" +
                             @"     <span style=""font-size:9.5pt"">:&nbsp;" + detalleSubtipoActividad + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><b><span style=""font-size:9.5pt"">Información breve del trabajo</span></b><span style=""font-size:9.5pt"">:&nbsp;" + comentario + "<u></u><u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><span style=""font-size:9.5pt""><u></u>&nbsp;<u></u></span></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"    <p ><u></u>&nbsp;<u></u></p>" +
                             @"</div>" +
                     @"</div>";
            string body = Mensaje;
            return Mensaje;
        }

        public string CreaCuerpoOP(string OP, string nombreUsuario)
        {
            string Mensaje = string.Empty;
            Mensaje = @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Estimado: " + nombreUsuario + "</span><u></u><u></u></p>" +
                             @"</div>" +
                             @"<div>" +
                             @"<p ><b><span style=""font-size:9.5pt"">Se le ha asignado la siguiente OP para su revisión</span></b><span style=""font-size:9.5pt"">: " + OP + "</span><u></u><u></u></p>" +
                     @"</div>";
            string body = Mensaje;
            return Mensaje;
        }

        public string CreaCuerpoEvaluacionPersonal(string mensaje, string nombreUsuario, string nombreJefe, string dirigidoA)
        {
            string Mensaje = string.Empty;
            //Mensaje = @"<div>" +
            //                 @"<p ><b><span style=""font-size:9.5pt"">Estimado: " + dirigidoA + "</span><u></u><u></u></p>" +
            //                 @"</div>" +
            //                 @"<div>" +
            //                 @"<p ><b><span style=""font-size:9.5pt""> " + mensaje +
            //                 @"</span></b><span style=""font-size:9.5pt"">" + nombreUsuario + "</span><u></u><u></u></p>" +
            //         @"</div>";

            Mensaje = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;background-color:" + "#2b5797" + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                @"                    <tr><td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                                @"                        <td style=""text-align:right;font-size:20px;""></td>" +
                                @"                        <td style=""width:15px;""></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">EVALUACIÓN DE DESEMPEÑO</div></td>" +
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
                                @"                    <tr style=""background-color:" + "#2b5797" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Detalle de la Evaluación</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Dirigido a</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + dirigidoA + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Mensaje</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + mensaje + nombreUsuario+ @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Evaluado</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + nombreUsuario + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Jefe Directo</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + nombreJefe + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                   <tr>" +
                                @"                        <td colspan=""3"">&nbsp;</td>" +
                                @"                   </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + "#2b5797" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                @"                        <td style=""width:140px;""><b>Evaluación de Desempeño</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td><a href=""" + IpPublica + @"#/Evaluacion"">Ver Evaluación</a></td>" +
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
                                @"                    <tr style=""background-color:" + "#2b5797" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:15px;"" >Gracias por contribuir al Sistema de Gestión de Desempeño</span></td>" +
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
            string body = Mensaje;
            return Mensaje;
        }

        public string CreaCuerpoSolicitudPersonal(string mensaje, string usuarioCrea, string Estado, string dirigidoA, int id)
        {
            string Mensaje = string.Empty;
            //Mensaje = @"<div>" +
            //                 @"<p ><b><span style=""font-size:9.5pt"">Estimado: " + dirigidoA + "</span><u></u><u></u></p>" +
            //                 @"</div>" +
            //                 @"<div>" +
            //                 @"<p ><b><span style=""font-size:9.5pt""> " + mensaje +
            //                 @"</span></b><span style=""font-size:9.5pt"">" + nombreUsuario + "</span><u></u><u></u></p>" +
            //         @"</div>";

            Mensaje = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;background-color:" + "#3BC0AA" + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                @"                    <tr><td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                                @"                        <td style=""text-align:right;font-size:20px;""></td>" +
                                @"                        <td style=""width:15px;""></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">SOLICITUD DE PERSONAL</div></td>" +
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
                                @"                    <tr style=""background-color:" + "#3BC0AA" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Detalle de la Solicitud</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Dirigido a</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + dirigidoA + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Mensaje</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + mensaje + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Creado por</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + usuarioCrea + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Estado</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + Estado + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                   <tr>" +
                                @"                        <td colspan=""3"">&nbsp;</td>" +
                                @"                   </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + "#3BC0AA" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                @"                        <td style=""width:140px;""><b>Solicitud de Personal</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td><a href=""" + IpPublica + @"/SeleccionSolicitudPersonal/DetalleSolicitudPersonal/" + id + @""">Ver Solicitud</a></td>" +
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
                                @"                    <tr style=""background-color:" + "#3BC0AA" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:15px;"" >Este correo electrónico fue enviado por IT Management System</span></td>" +
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
            string body = Mensaje;
            return Mensaje;
        }

        public string CreaCuerpoSolicitudVacacion(string mensaje, string usuarioCrea, string Estado, string dirigidoA, int id, string fechaInicioRetorno)
        {
            string Mensaje = string.Empty;
            //Mensaje = @"<div>" +
            //                 @"<p ><b><span style=""font-size:9.5pt"">Estimado: " + dirigidoA + "</span><u></u><u></u></p>" +
            //                 @"</div>" +
            //                 @"<div>" +
            //                 @"<p ><b><span style=""font-size:9.5pt""> " + mensaje +
            //                 @"</span></b><span style=""font-size:9.5pt"">" + nombreUsuario + "</span><u></u><u></u></p>" +
            //         @"</div>";

            string fecInicioRetorno = "";
            if(fechaInicioRetorno != "")
                fecInicioRetorno = 
                @"                    <tr>" +
                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                @"                        <td style=""width:140px;""><b>F. inicio - F. retorno</b></td>" +
                @"                        <td style=""width:15px;"">:</td>" +
                @"                        <td>" + fechaInicioRetorno + @"</td>" +
                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                @"                    </tr>";


            Mensaje = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;background-color:" + "#3BC0AA" + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                @"                    <tr><td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                                @"                        <td style=""text-align:right;font-size:20px;""></td>" +
                                @"                        <td style=""width:15px;""></td>" +
                                @"                    </tr>" +
                                @"                    <tr><td></td>" +
                                @"                        <td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">SOLICITUD DE VACACIONES</div></td>" +
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
                                @"                    <tr style=""background-color:" + "#3BC0AA" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Detalle de la Solicitud</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Dirigido a</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + dirigidoA + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Mensaje</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + mensaje + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Creado por</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + usuarioCrea + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Estado</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + Estado + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +

                                fecInicioRetorno +

                                @"                   <tr>" +
                                @"                        <td colspan=""3"">&nbsp;</td>" +
                                @"                   </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + "#3BC0AA" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                @"                        <td style=""width:140px;""><b>Solicitud de Vacaciones</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td><a href=""" + IpPublica + @"Vacaciones/MisVacaciones"">Ver Solicitud</a></td>" +
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
                                @"                    <tr style=""background-color:" + "#3BC0AA" + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:15px;"" >Este correo electrónico fue enviado por IT Management System</span></td>" +
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
            string body = Mensaje;
            return Mensaje;
        }

        public string CreaCuerpoTicketChatAsignado(string codigo, string titulo, string descripcion, string estado, string creador, string fechaasig, string asignado,int id)
        {
            

            string Estado = @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>Estado</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + estado + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>";

            string footer = @"<table style=""width:100%; color:black; "" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr  style=""background-color:#EAEAEA"">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:12px;"" >&copy; " + Convert.ToString(DateTime.Now.Year) + @" Electrodata All Rights Reserved</span></td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +
                @"</table>";

            string SocialNetwork = @"<table style=""width:100%;"">" +
                    @"<tr>" +
                        @"<td style=""width:30px;""></td>" +
                        @"<td style=""width:40px;""><a title=""Facebook"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.facebook.com/pages/Grupo-e-data/767481443318728?fref=ts"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-fb.png"" /></a></td>" +
                        @"<td style=""width:40px;""><a title=""LinkedIn"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.linkedin.com/company/electrodata-s.a.c"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-in.png"" /></a></td>" +
                        @"<td style=""width:40px;""><a title=""YouTube"" style=""border:0px solid white;text-decoration:none;"" href=""https://www.youtube.com/channel/UCMw-vM1KwrMOq7U0YkroQkg"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-yt.png"" /></a></td>" +
                        @"<td ><a style=""border:0px solid white;text-decoration:none;"" title=""Issuu"" href=""http://issuu.com/grupoe-data"" target=""_blank""><img src=""" + IpPublica + @"Content/Images/social/icon-is.png"" /></a></td>" +
                        @"<td style=""width:30px;""></td>" +
                    @"</tr>" +
                @"</table>";

            string Mensaje = string.Empty;
            Mensaje = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                    @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%;background-color:#0097a7;color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">Ticket Asignado - " + codigo + @"</td>" + @"<td style=""width:15px;""></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">Portal Usuario Electodata System</div></td>" +
                            @"<td></td>" +
                        @"</tr>" +
                        @"<tr><td>&nbsp;</td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                        @"</tr>" +
                    @"</table>" +
                @"</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:#0097a7"">" +
                            @"<td style=""width:15px;"">&nbsp;</td>" +
                            @"<td colspan=""5"">Versi&#243;n en Espa&#241;ol</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""6"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                    @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                     @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid #0097a7;border-bottom:1px solid #0097a7;""><b>Titulo</b></td>" +
                            @"<td style=""border-top:1px solid #0097a7;border-bottom:1px solid #0097a7;"">:</td>" +
                            @"<td style=""border-top:1px solid #0097a7;border-bottom:1px solid #0097a7;"">" + titulo + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>Descripción</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + descripcion + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        Estado +


                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Creado Por</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + creador + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Asignado A</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + asignado + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Fecha de Asignación</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + fechaasig + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +

                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +



                        @"<tr>" +
                            @"<td colspan=""3"">" +
                                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                    @"<tr style=""background-color:#0097a7"">" +
                                        @"<td style=""width:15px;"">&nbsp;</td>" +
                                        @"<td colspan=""5"">Links</td>" +
                                    @"</tr>" +
                                    @"<tr>" +
                                        @"<td colspan=""6"">&nbsp;</td>" +
                                    @"</tr>" +
                                @"</table>" +
                                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                    @"<tr>" +
                                        @"<td style=""width:30px;"">&nbsp;</td>" +
                                        @"<td style=""width:140px;""><b>Ver detalle del Ticket</b></td>" +
                                        @"<td style=""width:15px;"">:</td>" +
                                        @"<td><a href=""https://itms.electrodata.com.pe/PortalUsuario/Solicitud/Detalles?id=""" + id + @"> View Ticket Details</a></td>" +
                                        @"<td style=""width:30px;"">&nbsp;</td>" +
                                    @"</tr>" +

                                @"</table>" +
                            @"</td>" +
                        @"</tr>" +

                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +

                        @"<tr>" +
                            @"<td colspan=""3"">" +
                                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                    @"<tr style=""background-color:#0097a7"">" +
                                        @"<td style=""width:15px;"">&nbsp;</td>" +
                                        @"<td colspan=""5"">Social Networks</td>" +
                                    @"</tr>" +
                                    @"<tr>" +
                                        @"<td colspan=""6"">&nbsp;</td>" +
                                    @"</tr>" +
                                @"</table>" +
                                SocialNetwork +
                            @"</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""3"">" +
                                footer +
                            @"</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                        @"</tr>" +
                @"</table>";
            string body = Mensaje;
            return Mensaje;
        }

        public string CreaCuerpoCapacidadOP(string NombreIngeniero, int CantidadProyectos)
        {

            string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%;background-color:red;color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                    @"<tr>" +
                        @"<td style=""width:15px;""></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td>&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td>&nbsp;</td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td></td>" +
                        @"<td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">Information Technologies Management System</div></td>" +
                        @"<td></td>" +
                    @"</tr>" +
                    @"<tr><td>&nbsp;</td>" +
                        @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" +
                    @"</tr>" +
                @"</table>" +
            @"</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:red"">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5""></td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +

                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>Ingeniero Asignado</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + NombreIngeniero + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td style=""width:140px;""><b>N° Proyectos</b></td>" +
                        @"<td style=""width:15px;"">:</td>" +
                        @"<td>" + CantidadProyectos.ToString() + @"</td>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"<td><b>Mensaje</b></td>" +
                        @"<td>:</td>" +
                        @"<td>La capacidad promedio de 6 Proyectos asignados ha sido superado.</td>" +
                        @"<td>&nbsp;</td>" +
                    @"</tr>" +                   
                    @"</table>" +
                            @"</td>" +
                        @"</tr>" +

                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                        @"</tr>" +

        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">" +
                @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                    @"<tr style=""background-color:red"">" +
                        @"<td style=""width:15px;"">&nbsp;</td>" +
                        @"<td colspan=""5"">Social Networks</td>" +
                    @"</tr>" +
                    @"<tr>" +
                        @"<td colspan=""6"">&nbsp;</td>" +
                    @"</tr>" +
                @"</table>" +
                SocialNetwork +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">" +
                footer +
            @"</td>" +
        @"</tr>" +
        @"<tr>" +
            @"<td colspan=""3"">&nbsp;</td>" +
        @"</tr>" +
@"</table>";
            return header;
        }

        public string CreaCuerpoOPSoporteInforme(string NumeroOP, string Titulo, string cliente, string clienteFinal, string PM)
        {
            string header = "Estimados:" +
                            "<br /><br />Se adjunta el servicio de soporte técnico y mantenimiento." +
                            "<br />Cliente: " + cliente +
                            "<br />Cliente Final: " + clienteFinal +
                            "<br />OP: " + NumeroOP + " " + Titulo +
                            "<br />Project Manager: " + PM +
                            "<br /><br />Saludos.";
            return header;
        }

        public string EnvioAutomaticoInforme(string clientefinal,string periodo, string op, string producto, string fechainicio, string fechafin)
        {
            string mensaje = string.Format(
                            "Estimados:\n\n" +  
                            "Mediante la presente hacemos entrega del siguiente informe:\n\n" +
                            "  * Cliente Final: {0}\n" +
                            "  * Tipo de Informe: {1}\n" +
                            "  * OP: {2}\n" +
                            "  * Producto: {3}\n" +
                            "  * Periodo: {4} al {5}\n\n" +
                            "Gracias.",
                            clientefinal, periodo, op, producto, fechainicio, fechafin);

            return mensaje;
        }

        public string CorreoTareaBNV(string usuario, string codigoTicket, string titulo, string descripcion, string color, string estado, string fecha, string codigoTarea, int idTicket)
        {
            string cadena3 = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;background-color:" + color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
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
                                @"                        <td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">GESTIÓN DE TAREAS</div></td>" +
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
                                @"                    <tr style=""background-color:" + color + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                @"                        <td colspan=""5"">Detalle de la Tarea</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"                <table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Ticket:</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + codigoTicket + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Título:</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + titulo + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Detalle</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + descripcion + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td style=""width:140px;""><b>Responsable</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td>" + usuario + @"</td>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Estado</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + estado + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                @"                        <td><b>Fecha</b></td>" +
                                @"                        <td>:</td>" +
                                @"                        <td>" + fecha + @"</td>" +
                                @"                        <td>&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                    <tr>" +
                                @"                        <td colspan=""6"">&nbsp;</td>" +
                                @"                    </tr>" +
                                @"                </table>" +
                                @"            </td>" +
                                @"        </tr>" +
                                @"        <tr>" +
                                @"            <td colspan=""3"">" +
                                @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                @"                    <tr style=""background-color:" + color + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                @"                        <td style=""width:140px;""><b>Resumen Tareas</b></td>" +
                                @"                        <td style=""width:15px;"">:</td>" +
                                @"                        <td><a href=""" + IpPublica + @"DetailsTicket/Index/" + idTicket + @""">Ver Tarea</a></td>" +
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



        public string CrearCuerpoCertificado(string color, int IdCerti, string titulo, string nombreColaborador, string NombreCert, string sitioGerencia, string area, string marca, string mensaje, string estadoCer, DateTime FechaProgramada)
        {
            string Mensaje = string.Empty;
            Mensaje = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                 @"        <tr>" +
                                 @"            <td colspan=""3"">" +
                                 @"                 <table style=""width:100%;background-color:" + color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                 @"                    <tr><td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                                 @"                        <td style=""text-align:right;font-size:20px;""></td>" +
                                 @"                        <td style=""width:15px;""></td>" +
                                 @"                    </tr>" +
                                 @"                    <tr><td></td>" +
                                 @"                    <td><div style=""font - size:20px; color: white; ""> " + DateTime.Now.ToString("dd-MM-yyyy") + "</div></td>" +

                                 @"                        <td></td>" +
                                 @"                    </tr>" +
                                 @"                    <tr><td></td>" +
                                 @"                        <td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + titulo + "</div></td>" +
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
                                 @"                    <tr style=""background-color:" + color + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                 @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                 @"                        <td colspan=""5"">Detalle de Certificado</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td colspan=""6"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                </table>" +
                                 @"                <table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Dirigido a</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + nombreColaborador + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Certificado</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + NombreCert + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Gerencia</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + sitioGerencia + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Área</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + area + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Marca</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + marca + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Mensaje</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + mensaje + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Fecha Programada</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + FechaProgramada.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("es-ES")) + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Estado</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + estadoCer + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                   <tr>" +
                                 @"                        <td colspan=""3"">&nbsp;</td>" +
                                 @"                   </tr>" +
                                 @"                </table>" +
                                 @"            </td>" +
                                 @"        </tr>" +
                                 @"        <tr>" +
                                 @"            <td colspan=""3"">" +
                                 @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                 @"                    <tr style=""background-color:" + color + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                 @"                        <td style=""width:140px;""><b>Ver Detalles</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td><a href=""" + IpPublica + @"Certificado/DetalleCertificado/" + IdCerti + @""">Link de Detalle</a></td>" +
                                 
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
                                 @"                    <tr style=""background-color:" + color + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                 @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:15px;"" >Este correo electrónico fue enviado por IT Management System</span></td>" +
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
                                 @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:12px;"" >&copy; " + DateTime.Now.Year + " Electrodata All Rights Reserved</span></td >" +
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
            string body = Mensaje;
            return Mensaje;
        }


        public string CrearCuerpoCertificadoAprobado(string color, int IdCerti, string titulo, string nombreColaborador, string NombreCert, string sitioGerencia, string area, string marca, string mensaje, string estadoCer, DateTime FechaProgramada)
        {
            string Mensaje = string.Empty;
            Mensaje = @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                                 @"        <tr>" +
                                 @"            <td colspan=""3"">" +
                                 @"                 <table style=""width:100%;background-color:" + color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                                 @"                    <tr><td></td>" + @"<td>&nbsp;</td>" + @"<td></td>" +
                                 @"                        <td style=""text-align:right;font-size:20px;""></td>" +
                                 @"                        <td style=""width:15px;""></td>" +
                                 @"                    </tr>" +
                                 @"                    <tr><td></td>" +
                                 @"                    <td><div style=""font - size:20px; font - family:''Segoe UI'',Verdana,sans - serif; color: white; ""> " + DateTime.Now.ToString("dd-MM-yyyy") + "</div></td>" +

                                 @"                        <td></td>" +
                                 @"                    </tr>" +
                                 @"                    <tr><td></td>" +
                                 @"                        <td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + titulo + "</div></td>" +
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
                                 @"                    <tr style=""background-color:" + color + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
                                 @"                        <td style=""width:15px;"">&nbsp;</td>" +
                                 @"                        <td colspan=""5"">Detalle de Certificado</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td colspan=""6"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                </table>" +
                                 @"                <table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Dirigido a</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + nombreColaborador + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Certificado</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + NombreCert + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Gerencia</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + sitioGerencia + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Área</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + area + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Marca</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + marca + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Mensaje</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + mensaje + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                    <tr>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                        <td style=""width:140px;""><b>Estado</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td>" + estadoCer + @"</td>" +
                                 @"                        <td style=""width:30px;"">&nbsp;</td>" +
                                 @"                    </tr>" +
                                 @"                   <tr>" +
                                 @"                        <td colspan=""3"">&nbsp;</td>" +
                                 @"                   </tr>" +
                                 @"                </table>" +
                                 @"            </td>" +
                                 @"        </tr>" +
                                 @"        <tr>" +
                                 @"            <td colspan=""3"">" +
                                 @"                <table style=""width:100%;font-size:14px;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                                 @"                    <tr style=""background-color:" + color + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                 @"                        <td style=""width:140px;""><b>Ver mas Detalles</b></td>" +
                                 @"                        <td style=""width:15px;"">:</td>" +
                                 @"                        <td><a href=""" + IpPublica + @"Certificado/DetalleCertificado/" + IdCerti + @""">Link de Detalle</a></td>" +
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
                                 @"                    <tr style=""background-color:" + color + @";font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" +
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
                                 @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:15px;"" >Este correo electrónico fue enviado por IT Management System</span></td>" +
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
                                 @"                        <td><span style=""font-family:'Segoe UI',Verdana,sans-serif; font-size:12px;"" >&copy; " + DateTime.Now.Year + " Electrodata All Rights Reserved</span></td >" +
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
            string body = Mensaje;
            return Mensaje;
        }
    }
}