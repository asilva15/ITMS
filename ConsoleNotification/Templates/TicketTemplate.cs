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
    class TicketTemplate
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();
        private string footer = "";
        private string TicketExterno = "";
        private string resuelto = "";
        private string conformidad = "";
        private string SocialNetwork = "";
        private string IconPaymentBallot = "";
        private string IconCertificate5th = "";
        private string IconCertificateUtil = "";
        private string IpPublica = "";
        private string YesOrNo = "";
        private string IpServer = "http://itms.electrodata.com.pe/";
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;



        public TicketTemplate()
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
            try
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
                                  ID_TICK = t.ID_TICK,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  ID_DOCU_SALE = t.ID_DOCU_SALE,
                                  ID_ACCO = t.ID_ACCO,
                              }).First();

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                                  (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                       }).First().User;

                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value
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
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + ticket.NAM_STAT + "-" + (emp.Length > 0 ? emp + " - " : "") + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" +
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
                            @"<td style=""width:140px;""><b>Usuario</b></td>" +
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
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Tiempo Estimado de Atención</b></td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + @" Horas</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        schedule_es +
                        op +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +


                            (ticket.ID_ACCO.Value == 1 && ticket.ID_STAT_END.Value == 4 && tipoUser == 2 ? "" : (
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
                            @"<td style=""width:140px;""><b>Usuario Afectado</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""+width:30px;"">&nbsp;</td>" +
                            @"<td><b>Staff Asignado</b></td>" +
                            @"<td>:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                @"</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
                @"</tr>")) +

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
            catch
            {
                return null;
            }
        }

        public string CrearTicketBuenaventura(int id)
        {
            try
            {
                //Declaración de variables del ticket
                var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                              join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                              join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                              join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                              join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                              join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                              join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                              join clo in dbc.LOCATIONs on (t.ID_LOCA == null ? 0 : t.ID_LOCA) equals clo.ID_LOCA into locx
                              from xloc in locx.DefaultIfEmpty()
                              join cst in dbc.SITEs on (xloc == null ? 0 : xloc.ID_SITE) equals cst.ID_SITE into cstx
                              from xcst in cstx.DefaultIfEmpty()
                              select new
                              {
                                  ID_TICK = t.ID_TICK,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  ID_DOCU_SALE = t.ID_DOCU_SALE,
                                  ID_ACCO = t.ID_ACCO,
                                  RESUMEN_TICKET = t.SUM_TICK,
                                  NOM_Loca = (xloc == null ? "" : xloc.NAM_LOCA),
                                  NOM_CIA = (xcst == null ? "" : xcst.NAM_SITE),
                              }).First();

                //Usuario Afectado - NOMBRE Y APELLIDO
                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                                  (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                       }).First().User;

                //Tipo de Usuario - Compañia
                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value
                                }).First().User;

                //Uusario Asignado - NOMBRE APELLIDO
                string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                            User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                            (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                        }).First().User;

                //Uusario Solicitante - NOMBRE APELLIDO
                string CreateBy = "";
                try
                {
                    CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                    (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                }).First().User;
                }
                catch
                {
                    //Si no encuentra usuario solicitante - se reemplaza con el usuario afectado 
                    CreateBy = "";

                }

                DateTime ExpirationDate = Convert.ToDateTime(ticket.DAT_EXPI_TICK);

                //string create_ticket_en = String.Format("{0:}",ticket.CREA);
                string expiration_ticket_en = String.Format("{0:MM/dd/yyyy H:mm}", ticket.DAT_EXPI_TICK);
                string expiration_ticket_es = String.Format("{0:dd/MM/yyyy H:mm}", ticket.DAT_EXPI_TICK);
                string sla = Convert.ToString(ticket.HOU_PRIO);
                string schedule_es = "", schedule_en = "";

                //TICKET PROGRAMADO - FORMATO DE FECHAS PARA LA FECHA PROGRAMADA
                if (ticket.ID_STAT_END == 5)
                {
                    //FECHA DE PROGRAMACIÓN EN FORMATO INGLES
                    schedule_en = @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Scheduled Date</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", ticket.SCHE) + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>";
                    //FECHA DE PROGRAMACION EN FORMATO ESPAÑOL
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
                //DOCUMENTOS ADJUNTOS
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
                //ID_ACCO -> ELOECTRODATA
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

                //PRIORIDAD 6?
                if (ticket.ID_PRIO == 6)
                {
                    sla = "Planning";
                    expiration_ticket_en = "Planning";
                    expiration_ticket_es = "Planning";
                }

                //CUERPO DEL MENSAJE
                string header = @"<table style = ""font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" width=""600px"" cellspacing= ""0 "" cellpadding= ""1"" border= ""0""> " +
                         @"<tr>" +
                        @"<td> Estimado(a) </td> "
                        + @"<td> &nbsp;</td>"
                        + @"</tr>"
                        + @"<tr><td><b>" + AffectedUser + @"</b></td>"
                        + @"<td>&nbsp;</td>"
                        + @"</tr>"
                        + @"<tr><td> Le informamos que se ha abierto el siguiente caso.</td>"
                        + @"<td> &nbsp;</ td ></ tr > </table>"
                        + @"<table style=""width:100%;color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                        @"</table>" +
                        @"<br>" +
                        @"<table width= '600px ' cellspacing= '0 'cellpadding= '0 ' border= '0 ' align= 'center ' style= 'width:450px; border:1px solid'>" +
                        @"<td colspan = '3'>" +
                       @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>NRO. CASO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.COD_TICK + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>REGISTRO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.CREA + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>ESTADO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_STAT + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>SERVICIO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_CLAS + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>CATEGORIA</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_CATE + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>USUARIO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + AffectedUser + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>SEDE</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NOM_Loca + "</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>CIA</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NOM_CIA + "</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +

            @"</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
            @"</tr>" +
    @"</table>" +
    @"<table style = ""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" cellspacing= ""0 "" cellpadding= ""1"" border= ""0""> "
                        + "<tr><td> <b>DESCRIPCION: </b></td><td> &nbsp;</td></tr><tr><td>" + ticket.RESUMEN_TICKET + "</td><td> &nbsp;</td></tr> <tr><td></td><td> &nbsp;</td></tr>" +
                        "<tr><td> " + CreateBy + " </td></tr><tr><td>Mesa de Servicio</td></tr><tr><td>Anexo: 123</td></tr><tr><td>Email: soporte@buenaventura.pe</td></tr></table>";

                return header;
            }
            catch
            {
                return null;
            }
        }

        public string AsignarTicketBuenaventura(int id)
        {
            try
            {
                //Declaración de variables del ticket
                var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                              join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                              join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                              join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                              join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                              join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                              join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                              join clo in dbc.LOCATIONs on (t.ID_LOCA == null ? 0 : t.ID_LOCA) equals clo.ID_LOCA into locx
                              from xloc in locx.DefaultIfEmpty()
                              join cst in dbc.SITEs on (xloc == null ? 0 : xloc.ID_SITE) equals cst.ID_SITE into cstx
                              from xcst in cstx.DefaultIfEmpty()
                              select new
                              {
                                  ID_TICK = t.ID_TICK,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  ID_DOCU_SALE = t.ID_DOCU_SALE,
                                  ID_ACCO = t.ID_ACCO,
                                  RESUMEN_TICKET = t.SUM_TICK,
                                  NOM_Loca = (xloc == null ? "" : xloc.NAM_LOCA),
                                  NOM_CIA = (xcst == null ? "" : xcst.NAM_SITE),
                              }).First();

                //Usuario Afectado - NOMBRE Y APELLIDO
                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                                  (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                       }).First().User;

                //Tipo de Usuario - Compañia
                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value
                                }).First().User;

                //Uusario Asignado - NOMBRE APELLIDO
                string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                            User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                            (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                        }).First().User;

                //Uusario Solicitante - NOMBRE APELLIDO
                string CreateBy = "";
                try
                {
                    CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                    (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                }).First().User;
                }
                catch
                {
                    //Si es no encuentra usuario solicitante - se reemplaza con el usuario afectado 
                    CreateBy = "";

                }

                DateTime ExpirationDate = Convert.ToDateTime(ticket.DAT_EXPI_TICK);

                //string create_ticket_en = String.Format("{0:}",ticket.CREA);
                string expiration_ticket_en = String.Format("{0:MM/dd/yyyy H:mm}", ticket.DAT_EXPI_TICK);
                string expiration_ticket_es = String.Format("{0:dd/MM/yyyy H:mm}", ticket.DAT_EXPI_TICK);
                string sla = Convert.ToString(ticket.HOU_PRIO);
                string schedule_es = "", schedule_en = "";

                //TICKET PROGRAMADO - FORMATO DE FECHAS PARA LA FECHA PROGRAMADA
                if (ticket.ID_STAT_END == 5)
                {
                    //FECHA DE PROGRAMACIÓN EN FORMATO INGLES
                    schedule_en = @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Scheduled Date</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", ticket.SCHE) + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>";
                    //FECHA DE PROGRAMACION EN FORMATO ESPAÑOL
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
                //DOCUMENTOS ADJUNTOS
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
                //ID_ACCO -> ELOECTRODATA
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

                //PRIORIDAD 6?
                if (ticket.ID_PRIO == 6)
                {
                    sla = "Planning";
                    expiration_ticket_en = "Planning";
                    expiration_ticket_es = "Planning";
                }

                //CUERPO DEL MENSAJE
                string header = @"<table style = ""font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" width=""600px"" cellspacing= ""0 "" cellpadding= ""1"" border= ""0""> " +
                         @"<tr>" +
                        @"<td> Estimado(a) </td> "
                        + @"<td> &nbsp;</td>"
                        + @"</tr>"
                        + @"<tr><td><b>" + StaffAssigned + @"</b></td>"
                        + @"<td>&nbsp;</td>"
                        + @"</tr>"
                        + @"<tr><td> Le informamos que se ha asignado el siguiente caso.</td>"
                        + @"<td> &nbsp;</ td ></ tr > </table>"
                        + @"<table style=""width:100%;color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                        @"</table>" +
                        @"<br>" +
                        @"<table width= '600px ' cellspacing= '0 'cellpadding= '0 ' border= '0 ' align= 'center ' style= 'width:450px; border:1px solid'>" +
                        @"<td colspan = '3'>" +
                       @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>NRO. CASO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.COD_TICK + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>REGISTRO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.CREA + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>ESTADO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_STAT + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>SERVICIO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_CLAS + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>CATEGORIA</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_CATE + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>USUARIO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + AffectedUser + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>SEDE</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NOM_Loca + "</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>CIA</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NOM_CIA + "</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +

            @"</td>" +
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
            @"</tr>" +
    @"</table>" +
    @"<table style = ""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" cellspacing= ""0 "" cellpadding= ""1"" border= ""0""> "
                        + "<tr><td> <b>DESCRIPCION: </b></td><td> &nbsp;</td></tr><tr><td>" + ticket.RESUMEN_TICKET + "</td><td> &nbsp;</td></tr> <tr><td></td><td> &nbsp;</td></tr>" +

                        "<tr><td> " + CreateBy + " </td></tr><tr><td>Mesa de Servicio</td></tr><tr><td>Anexo: 123</td></tr><tr><td>Email: soporte@buenaventura.pe</td></tr></table>";

                return header;
            }
            catch
            {
                return null;
            }
        }

        public string CrearEscalamientoTicketBuenaventura(string SUM_TICK)
        {
            return
                    @"<p>Estimado(a) Buenos días, por favor su apoyo para la asignación de un soporte especializado para atender una necesidad en el Servicio de Buenaventura: </p>" +
                    @"<br>" +
                    @"<p><b>Descripción del Ticket:</b></p>" +
                    $@"<p>{SUM_TICK}</p>";
        }
        //Ticket para Goldfields - Solicitud de Renzo Cobos


        public string CreateTicketGoldFields(int id, int IdEstado)
        {
            try
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
                                  ID_TICK = t.ID_TICK,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  ID_DOCU_SALE = t.ID_DOCU_SALE,
                                  ID_ACCO = t.ID_ACCO,
                                  SUMMARY = t.SUM_TICK,
                                  ID_CATE = t.ID_CATE,
                                  TicketExterno = t.TicketExterno
                              }).First();

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                                  (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                       }).First().User;

                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value
                                }).First().User;


                string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                            User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                            (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                        }).First().User;

                //Portal Usuario
                string CreateBy = "";
                try
                {
                    CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                    (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                }).First().User;
                }
                catch
                {
                    CreateBy = AffectedUser;
                }

                DateTime ExpirationDate = Convert.ToDateTime(ticket.DAT_EXPI_TICK);

                var ticketComentario = (from dt in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == ticket.ID_TICK).ToList()
                                        select new
                                        {
                                            id_deta_tick = dt.ID_DETA_TICK,
                                            coment = dt.COM_DETA_TICK == null ? "" : dt.COM_DETA_TICK
                                        }
                                         ).OrderByDescending(dt => dt.id_deta_tick).FirstOrDefault();

                string comentario = "";

                if (ticketComentario != null && IdEstado == 4)
                    comentario = ticketComentario.coment;

                if (comentario == "" || IdEstado == 1)
                    comentario = ticket.SUMMARY;

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

                //Para Tickets resueltos
                if (ticket.ID_STAT_END == 4)
                {
                    // Ticket con categoría Gestión De Servicios-Gestión De Seguridad Informática-AMP
                    var categoria = dbc.CATEGORies.Single(x => x.ID_CATE == ticket.ID_CATE);
                    if (categoria.ID_CATE_PARE == 15302)
                    {
                        TicketExterno = @"<tr>" +
                                @"<td style=""width:30px;"">&nbsp;</td>" +
                                @"<td><b>Ticket Cybersoc</b></td>" +
                                @"<td>:</td>" +
                                @"<td>" + ticket.TicketExterno + @"</td>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>";
                    }

                    resuelto = @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Detalle</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticket.SUMMARY + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>";
                    conformidad = "Solicitamos nos brinde su conformidad en la atención brindada.Cualquier duda o consulta, comunicarse con nosotros." +
                                    @"<br><b>De no recibir conformidad al presente correo en un tiempo de 48 horas se procederá con el cierre de su solicitud entendiendo que la misma ha sido atendida de acuerdo a lo requerido." + @"</b>" +
                                    "<br>Best Regards | Saludos Cordiales.";
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

                string Color = ticket.COL_STAT;

                if (IdEstado == 1) //Solo para creación de tickets
                {
                    switch (ticket.ID_PRIO)
                    {
                        case 1:
                            Color = "#BA141A";
                            break;
                        case 2:
                            Color = "#FF8400";
                            break;
                        case 3:
                            Color = "#F2BA0C";
                            break;
                        case 4:
                            Color = "#2D5C88";
                            break;
                        case 5:
                            Color = "#333333";
                            break;
                        default:
                            break;
                    }
                }

                string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                    @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%;background-color:" + Color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + ticket.NAM_STAT + "-" + (emp.Length > 0 ? emp + " - " : "") + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" +
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
                        @"<tr style=""background-color:" + Color + @""">" +
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
                            @"<td style=""width:140px;""><b>Usuario</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + AffectedUser + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Prioridad</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + ticket.NAM_PRIO + @"</td>" +
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

                        (ticket.ID_PRIO == 5 ? "" : (
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Tiempo Estimado de Atención</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @""">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + sla + @" Horas</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>")) +

                        schedule_es +
                        TicketExterno +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Comentario</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + comentario + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        resuelto +
                        op +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +


                            (ticket.ID_ACCO.Value == 1 && ticket.ID_STAT_END.Value == 4 && tipoUser == 2 ? "" : (
            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:" + Color + @""">" +
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
                            @"<td style=""width:140px;""><b>Usuario Afectado</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""+width:30px;"">&nbsp;</td>" +
                            @"<td><b>Staff Asignado</b></td>" +
                            @"<td>:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                @"</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
                @"</tr>")) +

            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:" + Color + @""">" +
                            @"<td style=""width:15px;"">&nbsp;</td>" +
                            @"<td colspan=""5"">Social Networks</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""6"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                    SocialNetwork +
                    conformidad +
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
            catch
            {
                return null;
            }
        }

        //Ticket para Goldfields - Solicitud de Silvia y Liliana Chávez
        public string CreateTicketGoldFieldsIngles(int id, int IdEstado)
        {
            try
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
                                  ID_TICK = t.ID_TICK,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.CLA_STAT.ToLower()),
                                  NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  ID_DOCU_SALE = t.ID_DOCU_SALE,
                                  ID_ACCO = t.ID_ACCO,
                                  SUMMARY = t.SUM_TICK
                              }).First();

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                                  (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                       }).First().User;

                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value
                                }).First().User;


                string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                            User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                            (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                        }).First().User;

                string CreateBy = "";
                try
                {
                    CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                    (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                }).First().User;
                }
                catch
                {
                    CreateBy = AffectedUser;
                }

                DateTime ExpirationDate = Convert.ToDateTime(ticket.DAT_EXPI_TICK);

                var ticketComentario = (from dt in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == ticket.ID_TICK).ToList()
                                        select new
                                        {
                                            id_deta_tick = dt.ID_DETA_TICK,
                                            coment = dt.COM_DETA_TICK == null ? "" : dt.COM_DETA_TICK
                                        }
                                         ).OrderByDescending(dt => dt.id_deta_tick).FirstOrDefault();

                string comentario = "";

                if (ticketComentario != null && IdEstado == 4)
                    comentario = ticketComentario.coment;

                if (comentario == "" || IdEstado == 1)
                    comentario = ticket.SUMMARY;

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

                string Color = ticket.COL_STAT;

                if (IdEstado == 1) //Solo para creación de tickets
                {
                    switch (ticket.ID_PRIO)
                    {
                        case 1:
                            Color = "#BA141A";
                            break;
                        case 2:
                            Color = "#FF8400";
                            break;
                        case 3:
                            Color = "#F2BA0C";
                            break;
                        case 4:
                            Color = "#2D5C88";
                            break;
                        case 5:
                            Color = "#333333";
                            break;
                        default:
                            break;
                    }
                }

                string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                    @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%;background-color:" + Color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + ticket.NAM_STAT + "-" + (emp.Length > 0 ? emp + " - " : "") + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dd/MM/yyyy}", DateTime.Now) + @"</div></td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" +
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
                        @"<tr style=""background-color:" + Color + @""">" +
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
                            @"<td style=""width:140px;""><b>User</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + AffectedUser + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Priority</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + ticket.NAM_PRIO + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Class</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticket.NAM_CLAS + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Sub Class</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticket.NAM_SCLAS + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Assigned Staff</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + StaffAssigned + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +

                        (ticket.ID_PRIO == 5 ? "" : (
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Estimated Attention Time</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @""">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + sla + @" Hours</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>")) +

                        schedule_es +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Commentary</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + comentario + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        op +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +


                            (ticket.ID_ACCO.Value == 1 && ticket.ID_STAT_END.Value == 4 && tipoUser == 2 ? "" : (
            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:" + Color + @""">" +
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
                            @"<td style=""width:140px;""><b>User</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""+width:30px;"">&nbsp;</td>" +
                            @"<td><b>Assigned Staff</b></td>" +
                            @"<td>:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                @"</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
                @"</tr>")) +

            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:" + Color + @""">" +
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
            catch
            {
                return null;
            }
        }


        //----------------------
        //Ticket Publicom - Mayra Catacora
        public string CreateTicketPublicom(int id)
        {
            try
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
                                  ID_TICK = t.ID_TICK,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  ID_DOCU_SALE = t.ID_DOCU_SALE,
                                  ID_ACCO = t.ID_ACCO,
                                  SUMMARY = t.SUM_TICK
                              }).First();

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                                  (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                       }).First().User;

                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value
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

                var ticketComentario = (from dt in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == ticket.ID_TICK).ToList()
                                        select new
                                        {
                                            id_deta_tick = dt.ID_DETA_TICK,
                                            coment = dt.COM_DETA_TICK == null ? "" : dt.COM_DETA_TICK
                                        }
                                         ).OrderByDescending(dt => dt.id_deta_tick).FirstOrDefault();

                string comentario = "", descripcionTicket = "";

                if (ticketComentario != null)
                    comentario = ticketComentario.coment;

                if (comentario == "")
                    comentario = ticket.SUMMARY;

                descripcionTicket = ticket.SUMMARY;

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
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Descripción</b></td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + descripcionTicket + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Comentario</b></td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">:</td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + comentario + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                            op +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +

                            (ticket.ID_ACCO.Value == 1 && ticket.ID_STAT_END.Value == 4 && tipoUser == 2 ? "" : (
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
                            @"<td style=""width:140px;""><b>Usuario Afectado</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""+width:30px;"">&nbsp;</td>" +
                            @"<td><b>Staff Asignado</b></td>" +
                            @"<td>:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                @"</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
                @"</tr>")) +

            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                            @"<td style=""width:15px;"">&nbsp;</td>" +
                            @"<td colspan=""5"">Redes Sociales</td>" +
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
            catch
            {
                return null;
            }
        }
        //----------------------


        public string CreateTicketToro(int id)
        {
            try
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
                                  ID_TICK = t.ID_TICK,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  ID_DOCU_SALE = t.ID_DOCU_SALE,
                                  ID_ACCO = t.ID_ACCO,
                                  SUMMARY = t.SUM_TICK
                              }).First();



                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),

                                       }).First().User;

                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value
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

                var ticketComentario = (from dt in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == ticket.ID_TICK).ToList()
                                        select new
                                        {
                                            id_deta_tick = dt.ID_DETA_TICK,
                                            coment = dt.COM_DETA_TICK == null ? "" : dt.COM_DETA_TICK
                                        }
                                         ).OrderByDescending(dt => dt.id_deta_tick).FirstOrDefault();

                string comentario = "", descripcionTicket = "";

                if (ticketComentario != null)
                    comentario = ticketComentario.coment;

                if (comentario == "")
                    comentario = ticket.SUMMARY;

                descripcionTicket = ticket.SUMMARY;

                //string create_ticket_en = String.Format("{0:}",ticket.CREA);
                string expiration_ticket_en = String.Format("{0:MM/dd/yyyy H:mm}", ticket.DAT_EXPI_TICK);
                string expiration_ticket_es = String.Format("{0:dd/MM/yyyy H:mm}", ticket.DAT_EXPI_TICK);
                string sla = Convert.ToString(ticket.HOU_PRIO);
                string schedule_es = "", schedule_en = "";



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

                //Para Tickets resueltos
                if (ticket.ID_STAT_END == 4)
                {
                    resuelto = @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Detalle</b></td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.SUMMARY + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Accion Realizada</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticketComentario.coment + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>";
                    conformidad = "Solicitamos nos brinde su conformidad en la atención brindada.Cualquier duda o consulta, comunicarse con nosotros." +
                                    @"<br><b>De no recibir conformidad al presente correo en un tiempo de 48 horas se procederá con el cierre de su solicitud entendiendo que la misma ha sido atendida de acuerdo a lo requerido." + @"</b>" +
                                    "<br>Best Regards | Saludos Cordiales.";
                }


                YesOrNo = @"<table style=""width:100%;"">" +
                  @"<tr>" +
                      @"<center>" +
                      @"<td style=""width:30px;""></td>" +
                      @"<td style=""width:40px;""><a class=""btn btn-success"" title=""Yes"" style=""font-size:15px;"" href=""https://itms.electrodata.com.pe/webclient/Ticket/RespuestaPositiva/" + id + @""" target=""_blank""><i class=""fa fa-check""></i> Si</a></td>" +
                      @"<td style=""width:40px;""><a class=""btn btn-danger"" title=""No"" style=""font-size:15px;"" href=""https://itms.electrodata.com.pe/webclient/Ticket/RespuestaNegativa/" + id + @""" target=""_blank""><i class=""fa fa-close""></i> No</a></td>" +
                      @"<td style=""width:30px;""></td>" +
                      @"<br />" +
                      @"</center>" +
                  @"</tr>" +
              @"</table>";



                string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                    @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%;background-color:" + ticket.COL_STAT + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                            @"<td style=""width:15px;""></td>" + @"<td></td>" + @"<td></td>" + @"<td></td>" + @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + ticket.NAM_STAT + "-" + (emp.Length > 0 ? emp + " - " : "") + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                        //@"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + (emp.Length > 0 ? emp + " - " : "") + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
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
                            @"<td style=""width:140px;""><b>Usuario Solicitante</b></td>" +
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
                            resuelto +
                            op +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +

                            (ticket.ID_ACCO.Value == 31 && ticket.ID_STAT_END.Value == 4 && tipoUser == 2 ? "" : (
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
                            @"<td style=""width:140px;""><b>Usuario Afectado</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""+width:30px;"">&nbsp;</td>" +
                            @"<td><b>Staff Asignado</b></td>" +
                            @"<td>:</td>" +
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ticket.ID_TICK) + @""">View Ticket Details</a></td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                @"</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
                @"</tr>")) +


            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                            @"<td style=""width:15px;"">&nbsp;</td>" +
                            @"<td colspan=""5"">¿Esta conforme con su Ticket resuelto?</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""6"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                    YesOrNo +
                @"</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                            @"<td style=""width:15px;"">&nbsp;</td>" +
                            @"<td colspan=""5"">Redes Sociales</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""6"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                    SocialNetwork +
                    conformidad +
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
            catch
            {
                return null;
            }
        }

        public string CreateDetailsTicket(int ID_DETA_TICK)
        {
            try
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
                                  ID_STAT = t.ID_STAT,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = scl.NAM_CATE,
                                  NAM_CLAS = cl.NAM_CATE,
                                  NAM_SCAT = sc.NAM_CATE,
                                  NAM_CATE = c.NAM_CATE,
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  EXP_HOUR = p.HOU_PRIO - t.MINUTES / 60,
                                  ID_ACCO = t.ID_ACCO,
                                  SUMMARY = t.SUM_TICK
                              }).First();

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),

                                       }).First().User;

                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                    //join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value

                                }).First().User;

                string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                            User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                            //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                        }).First().User;
                //Portal Usuario
                string CreateBy = "";
                try
                {
                    CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                    (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                }).First().User;
                }
                catch
                {
                    CreateBy = AffectedUser;
                }

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

                //Para Tickets resueltos
                if (ticket.ID_STAT_END == 4)
                {
                    var ticketComentario = (from dt in dbc.DETAILS_TICKET.Where(dt => dt.ID_DETA_TICK == ID_DETA_TICK).ToList()
                                            select new
                                            {
                                                id_deta_tick = dt.ID_DETA_TICK,
                                                coment = dt.COM_DETA_TICK == null ? "" : dt.COM_DETA_TICK
                                            }
                                         ).OrderByDescending(dt => dt.id_deta_tick).FirstOrDefault();

                    resuelto = @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Detalle</b></td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.SUMMARY + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Accion Realizada</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticketComentario.coment + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>";
                    conformidad = "Solicitamos nos brinde su conformidad en la atención brindada.Cualquier duda o consulta, comunicarse con nosotros." +
                                    @"<br><b>De no recibir conformidad al presente correo en un tiempo de 48 horas se procederá con el cierre de su solicitud entendiendo que la misma ha sido atendida de acuerdo a lo requerido." + @"</b>" +
                                    "<br>Best Regards | Saludos Cordiales.";
                }


                string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                    @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%;background-color:" + ticket.COL_STAT + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + ticket.NAM_STAT + " - " + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" +
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
                            @"<td style=""width:140px;""><b>Usuario</b></td>" +
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
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Tiempo Estimado de Atención</b></td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + @" Horas</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        resuelto +
                        schedule_es +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +

                            (ticket.ID_ACCO.Value == 1 && detail_ticket.ID_TYPE_DETA_TICK.Value == 3 && detail_ticket.ID_STAT.Value == 4 && tipoUser == 2 ? "" : (
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
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ID_TICK) + @""">View Ticket Details</a></td>" +
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
                @"</tr>")) +

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
                    conformidad +
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
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
            @"</tr>" +
    @"</table>";
                return header;
            }
            catch
            {
                return null;
            }

        }

        public string CrearDetalleTicketBuenaventura(int ID_DETA_TICK)
        {
            try
            {
                //ID DETALLE TICKET
                var detail_ticket = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
                int ID_TICK = Convert.ToInt32(detail_ticket.ID_TICK);

                //VARIABLES DEL TICKET
                var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == ID_TICK).ToList()
                              join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                              join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                              join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                              join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                              join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                              join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                              join clo in dbc.LOCATIONs on t.ID_LOCA equals clo.ID_LOCA
                              join cst in dbc.SITEs on clo.ID_SITE equals cst.ID_SITE
                              select new
                              {
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = scl.NAM_CATE,
                                  NAM_CLAS = cl.NAM_CATE,
                                  NAM_SCAT = sc.NAM_CATE,
                                  NAM_CATE = c.NAM_CATE,
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  SUMMARY = t.SUM_TICK,
                                  NOM_Loca = clo.NAM_LOCA,
                                  NOM_CIA = cst.NAM_SITE,
                                  ID_TYPE_TICK = t.ID_TYPE_TICK,
                                  UserId = t.UserId
                              }).First();

                //NOMBRE DEL USUARIO AFECTADO
                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),

                                       }).First().User;

                var ticketComentario = (from dt in dbc.DETAILS_TICKET.Where(dt => dt.ID_DETA_TICK == ID_DETA_TICK).ToList()
                                        select new
                                        {
                                            id_deta_tick = dt.ID_DETA_TICK,
                                            coment = dt.COM_DETA_TICK == null ? "" : dt.COM_DETA_TICK
                                        }
                                     ).OrderByDescending(dt => dt.id_deta_tick).FirstOrDefault();
                //Usuario creador del ticket
                string CreateBy = "";
                try
                {
                    CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                    (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                }).First().User;
                }
                catch
                {
                    //Si es no encuentra usuario solicitante - se reemplaza con el usuario afectado 
                    CreateBy = "";

                }

                resuelto = @"<table style = ""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" cellspacing= ""4 "" cellpadding= ""2"" border= ""0""> "

                    + "<tr><td><b>DESCRIPCIÓN: </b></td><td> &nbsp;</td></tr><tr><td><b> " + ticket.SUMMARY + " </b ></td><td> &nbsp;</td></tr><tr><td>&nbsp;</td></tr>" +
                    @"<tr><td><b>SOLUCIÓN: </b></td><td> &nbsp;</td></tr><tr><td> " + ticketComentario.coment + "</td><td> &nbsp;</td></tr><tr><td>&nbsp;</td></tr>"
                    + @"</table>";

                conformidad = "Solicitamos nos brinde su conformidad en la atención brindada.Cualquier duda o consulta, comunicarse con mesa de ayuda." +
                                @"<br><b>De no recibir conformidad al presente correo en un tiempo de 48 horas se procederá con el cierre de su solicitud entendiendo que la misma ha sido atendida de acuerdo a lo requerido." + @"</b>" +
                                "<br>Best Regards | Saludos Cordiales.";
                string tipoTicket = "";
                if (ticket.ID_TYPE_TICK == 1)
                    tipoTicket = "incidente";
                else
                    tipoTicket = "requerimiento";

                string header = @"<table style = ""font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" width=""600px"" cellspacing= ""0 "" cellpadding= ""1"" border= ""0""> " +
                         @"<tr>" +
                        @"<td>Estimado(a) </td> "
                        + @"<td> &nbsp;</ td >"
                        + @"</tr>"
                        + @"<tr><td><b>" + AffectedUser + @"</b></td>"
                        + @"<td>&nbsp;</td>"
                        + @"</tr>"
                        + @"<tr><td> Le informamos que el estado de su " + tipoTicket + " ha sido atendido a la espera de su validación respectiva.</td>"
                        + @"<td> &nbsp;</ td ></ tr > </table>"
                        + @"<table style=""width:100%;color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                        @"</table>" +
                        @"<br>" +
                        @"<table width= '600px ' cellspacing= '0 'cellpadding= '0 ' border= '0 ' align= 'center ' style= 'width:600px; border:1px solid'>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>NRO. CASO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.COD_TICK + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>REGISTRO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.CREA + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>ESTADO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_STAT + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>SERVICIO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_CLAS + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>CATEGORIA</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_CATE + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>CIA</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NOM_CIA + "</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"</table>" +
                        @"</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                        @"</tr>" +

                @"</table>" +
                @"<br>"
                + resuelto + conformidad + "<br/><br/><br/>" + CreateBy + "<br/>Mesa de Servicio<br/>Anexo: 123<br/>Email: soporte@buenaventura.pe";

                return header;
            }
            catch
            {
                return null;
            }

        }

        public string EncuestaPortalUsuario(int ID_DETA_TICK)
        {

            string colorHeader = "#1e3c72";
            string colorHeaderText = "#FFFFFF";

            var detail_ticket = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
            int ID_TICK = Convert.ToInt32(detail_ticket.ID_TICK);

            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == ID_TICK).ToList()
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                          select new
                          {
                              ID_STAT = t.ID_STAT,
                              ID_PRIO = t.ID_PRIO,
                              ID_STAT_END = t.ID_STAT_END,
                              COD_TICK = t.COD_TICK,
                              NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                              HOU_PRIO = p.HOU_PRIO,
                              NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                              ID_PERS_ENTI = t.ID_PERS_ENTI,
                              ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                              ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                              UserId = t.UserId,
                              DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                              CREA = t.FEC_TICK,
                              SCHE = t.FEC_INI_TICK,
                              COL_STAT = e.COL_INDV_STAT,
                              EXP_HOUR = p.HOU_PRIO - t.MINUTES / 60,
                              ID_ACCO = t.ID_ACCO,
                              SUMMARY = t.SUM_TICK
                          }).First();

            var AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    sexo = (ce.SEX_ENTI == null ? "M" : "F"),
                                    User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),

                                }).FirstOrDefault();


            string body =
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
                                @"<img src='http://itms.electrodata.com.pe/images/logoED.png' width='250'" +
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
                            @"<td style=""text-align:justify;font-size:15px;"">" + (AffectedUser.sexo == "M" ? "Estimado " : AffectedUser.sexo == "F" ? "Estimada " : "") + textInfo.ToTitleCase(AffectedUser.User.ToLower()) + @",</td>" +
                            @"<td>&nbsp;</td>" +
                            @"<td>&nbsp;</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""5"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td>&nbsp;</td>" +
                            @"<td colspan=""3"" style=""text-align:justify;font-size:15px;"">Nuestro principal objetivo es brindarle un mejor servicio, por favor ingrese a la breve encuesta para responder su atención brindada sobre el ticket " + ticket.COD_TICK + ". </td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""5"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td>&nbsp;</td>" +
                            @"<td colspan=""3"" style=""text-align:justify;font-size:15px;"">Solicitud: " + ticket.SUMMARY + " </td>" +
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
                            @"<td colspan=""3"" style=""text-align:center;font-size:27px"" >" +
                                @"<a class=""btn btn-success"" style=""text-decoration:none;"" href=""" + IpPublica + @"Encuesta/EncuestaPortalUsuario?IdDetalleTicket=" + (Convert.ToString(ID_DETA_TICK)) + @"""" + @" target=""_blank"">" +
                                    @"Ir a Encuesta ->" +
                                @"</a>" +
                            @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""5"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""5"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr style=""background-color:#eaeaea"">" +
                            @"<td colspan=""5"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr style=""background-color:#eaeaea"">" +
                            @"<td>&nbsp;</td>" +
                            @"<td colspan=""3"" style=""text-align:justify;font-size:13px;""><strong>De no recibir respuesta a la presente encuesta de satisfacción en un tiempo de 48 horas, se procederá con el cierre de su solicitud entendiendo que la misma ha sido atendida de acuerdo a lo requerido.</strong></td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr style=""background-color:#eaeaea"">" +
                            @"<td colspan=""5"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td>&nbsp;</td>" +
                            @"<td colspan=""3""></td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                    @"</tbody>" +
                @"</table>";

            return body;
        }

        public string TransferTo(int ID_DETA_TICK)
        {
            try
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
                                           User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                           //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                       }).First().User;
                string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                            User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                            //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                        }).First().User;

                string CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                   select new
                                   {
                                       User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                       //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
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

                    try
                    {
                        DateTime fecsche;

                        var dt_aux = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK && x.ID_STAT == 5);

                        if (dt_aux.Count() > 0)
                        {
                            var detatick = dt_aux.OrderByDescending(x => x.ID_DETA_TICK).First();
                            fecsche = Convert.ToDateTime(detatick.FEC_SCHE);

                        }
                        else
                        {
                            fecsche = Convert.ToDateTime(ticket.SCHE);
                        }

                        schedule_en = @"<tr>" +
                                @"<td style=""width:30px;"">&nbsp;</td>" +
                                @"<td><b>Scheduled Date</b></td>" +
                                @"<td>:</td>" +
                                @"<td>" + String.Format("{0:MM/dd/yyyy H:mm}", fecsche) + @"</td>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>";

                        schedule_es = @"<tr>" +
                                @"<td style=""width:30px;"">&nbsp;</td>" +
                                @"<td><b>Fecha de Programaci&#243;n</b></td>" +
                                @"<td>:</td>" +
                                @"<td>" + String.Format("{0:dd/MM/yyyy H:mm}", fecsche) + @"</td>" +
                                @"<td>&nbsp;</td>" +
                            @"</tr>";
                    }
                    catch
                    {
                        schedule_en = "";
                        schedule_es = "";
                    }
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
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ID_TICK) + @""">View Ticket Details</a></td>" +
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
            catch
            {
                return null;
            }


        }

        public string CrearCorreoTransferenciaBVN(int ID_DETA_TICK)
        {
            try
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
                              join clo in dbc.LOCATIONs on (t.ID_LOCA == null ? 0 : t.ID_LOCA) equals clo.ID_LOCA into locx
                              from xloc in locx.DefaultIfEmpty()
                              join cst in dbc.SITEs on (xloc == null ? 0 : xloc.ID_SITE) equals cst.ID_SITE into cstx
                              from xcst in cstx.DefaultIfEmpty()
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
                                  t.ID_TYPE_TICK,
                                  SUMMARY = t.SUM_TICK,
                                  NOM_CIA = (xcst == null ? "" : xcst.NAM_SITE),
                              }).First();

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                           //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                       }).First().User;
                string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                            User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                            //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                        }).First().User;

                string CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                   select new
                                   {
                                       User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                       //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                   }).First().User;

                DateTime ExpirationDate = Convert.ToDateTime(ticket.DAT_EXPI_TICK);

                //string create_ticket_en = String.Format("{0:}",ticket.CREA);
                string expiration_ticket_en = String.Format("{0:MM/dd/yyyy H:mm}", ticket.DAT_EXPI_TICK);
                string expiration_ticket_es = String.Format("{0:dd/MM/yyyy H:mm}", ticket.DAT_EXPI_TICK);
                string sla = Convert.ToString(ticket.HOU_PRIO);
                string pers_assi_name = "", ult_assi_es = "", ult_assi_en = "",
                    transferBy_es = "", transferBy_en = "", comentario = "";
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

                    //if (pers_assi.Where(x => x.ID_PERS_ENTI != null).Count() <= 1)
                    //{
                    //    try
                    //    {
                    //        pers_assi_name = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI_STAR).ToList()
                    //                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                    //                          select new
                    //                          {
                    //                              User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                    //                          }).First().User;
                    //    }
                    //    catch
                    //    {

                    //    }
                    //}
                    //else
                    //{
                    //    ID_ASSI_BEFORE = Convert.ToInt32(pers_assi.Where(x => x.ID_DETA_TICK != ID_DETA_TICK).First().ID_PERS_ENTI);
                    //    pers_assi_name = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ID_ASSI_BEFORE).ToList()
                    //                      join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                    //                      select new
                    //                      {
                    //                          User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                    //                      }).First().User;
                    //}

                    //if (pers_assi_name != "")
                    //{
                    //    ult_assi_en = @"<tr>" +
                    //        @"<td style=""width:30px;"">&nbsp;</td>" +
                    //        @"<td><b>Before Responsible</b></td>" +
                    //        @"<td>:</td>" +
                    //        @"<td>" + pers_assi_name + @"</td>" +
                    //        @"<td>&nbsp;</td>" +
                    //    @"</tr>";

                    //    ult_assi_es = @"<tr>" +
                    //            @"<td style=""width:30px;"">&nbsp;</td>" +
                    //            @"<td><b>Responsable Anterior</b></td>" +
                    //            @"<td>:</td>" +
                    //            @"<td>" + pers_assi_name + @"</td>" +
                    //            @"<td>&nbsp;</td>" +
                    //        @"</tr>";
                    //}
                    //UserId_Before = Convert.ToInt32(pers_assi.First().UserId);

                    string TransferBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == UserId_Before).ToList()
                                         select new
                                         {
                                             User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower()),
                                         }).First().User;

                    //transferBy_en = @"<tr>" +
                    //        @"<td style=""width:30px;"">&nbsp;</td>" +
                    //        @"<td><b>Transfer By</b></td>" +
                    //        @"<td>:</td>" +
                    //        @"<td>" + TransferBy + @"</td>" +
                    //        @"<td>&nbsp;</td>" +
                    //    @"</tr>";

                    //transferBy_es = @"<tr>" +
                    //        @"<td style=""width:30px;"">&nbsp;</td>" +
                    //        @"<td><b>Transferido Por</b></td>" +
                    //        @"<td>:</td>" +
                    //        @"<td>" + TransferBy + @"</td>" +
                    //        @"<td>&nbsp;</td>" +
                    //    @"</tr>";
                    //
                }
                catch
                {

                }
                comentario = @"<table style = ""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" cellspacing= ""4 "" cellpadding= ""2"" border= ""0""> "
                    + "<tr><td><b>DESCRIPCIÓN: </b></td><td> &nbsp;</td></tr><tr><td><b> " + ticket.SUMMARY + " </b ></td><td> &nbsp;</td></tr><tr><td>&nbsp;</td></tr>"
                    + @"</table>";

                string header = @"<table style = ""font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" width=""600px"" cellspacing= ""0 "" cellpadding= ""1"" border= ""0""> " +
                         @"<tr>" +
                        @"<td>Estimado(a) </td> "
                        + @"<td> &nbsp;</ td >"
                        + @"</tr>"
                        + @"<tr><td><b>" + transferBy_es + @"</b></td>"
                        + @"<td>&nbsp;</td>"
                        + @"</tr>"
                        + @"<tr><td> Le informamos que se le ha transferido el siguiente caso.</td>"
                        + @"<td> &nbsp;</ td ></ tr > </table>"
                        + @"<table style=""width:100%;color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                        @"</table>" +
                        @"<br>" +
                        @"<table width= '600px ' cellspacing= '0 'cellpadding= '0 ' border= '0 ' align= 'center ' style= 'width:600px; border:1px solid'>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>NRO. CASO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.COD_TICK + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>REGISTRO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.CREA + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>ESTADO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_STAT + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>SERVICIO</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_CLAS + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>CATEGORIA</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NAM_CATE + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>CIA</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.NOM_CIA + "</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"</table>" +
                        @"</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""3"">&nbsp;</td>" +
                        @"</tr>" +

                @"</table>" +
                @"<br>"
                + comentario + "<br/><br/><br/>" + CreateBy + "<br/>Mesa de Servicio<br/>Anexo: 123<br/>Email: soporte@buenaventura.pe";

                return header;
            }
            catch
            {
                return null;
            }


        }

        public string TicketNotification(int ID_TICK, int ID_TYPE_NOTI,string tipoTicket)
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
                          join clo in dbc.LOCATIONs on t.ID_LOCA equals clo.ID_LOCA
                          join cst in dbc.SITEs on clo.ID_SITE equals cst.ID_SITE
                          select new
                          {
                              t.COD_TICK,
                              t.FEC_TICK,
                              t.ID_PERS_ENTI_END,
                              t.ID_PERS_ENTI,
                              SUB_CLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                              CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                              SUB_CATE = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                              CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                              NAM_STAT = textInfo.ToTitleCase(st.NAM_STAT.ToLower()),
                              NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                              t.UserId,
                              p.HOU_PRIO,
                              t.ID_PERS_ENTI_ASSI,
                              NOM_Loca = clo.NAM_LOCA,
                              NOM_CIA = cst.NAM_SITE,
                              t.IdSLA,
                              t.ID_PRIO,
                              RESUMEN_TICKET = t.SUM_TICK,
                          }).First();

            string Requester = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI).ToList()
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                    //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                }).First().User;

            string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                   join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                   select new
                                   {
                                       User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                       //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                   }).First().User;

            var Assigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            select new
                            {
                                NAME = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                Estimado = ce.SEX_ENTI == null ? "Estimado (a)" : ce.SEX_ENTI == "M" ? "Estimado" : "Estimada",
                                Sexo = ce.SEX_ENTI,
                            }).First();
                string CreateBy = "";
                try
                {
                    CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                    (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                }).First().User;
                }
                catch
                {
                    //Si no encuentra usuario solicitante - se reemplaza con el usuario afectado 
                    CreateBy = "";

                }
                var tiempoAtencion = (from sl in dbc.SLADetalle.Where(sld => sld.IdSLA == ticket.IdSLA && sld.IdPrioridad == ticket.ID_PRIO && sld.Estado == true).ToList()
                                     select new {
                                         sl.TiempoAtencion
                                     }).First();
            //string Estimado_en = "",Estimado_es = "";
            int hours = 0;
            string msg_en = "", msg_es = "";
            switch (ID_TYPE_NOTI)
            {
                case 1:
                    hours = Convert.ToInt32(ticket.HOU_PRIO) / 2;
                    msg_en = "Dear " + Assigned.NAME + ", " + Convert.ToString(hours) + " hours have elapsed since the creation of the ticket " + ticket.COD_TICK + ", please follow up!";
                    msg_es = Assigned.Estimado + ": </br></br> " + Assigned.NAME + @", le informamos que el caso asignado está en 50% del progreso y el SLA para este servicio es de " + tipoTicket + " " + tiempoAtencion.TiempoAtencion;
                    break;
                case 2:
                    hours = 1;
                    //there is 15 minutes for the ticket HFHF768 SLA to expire
                    msg_en = "Dear " + Assigned.NAME + ", there is " + Convert.ToString(hours) + " hour for the ticket " + ticket.COD_TICK + ", SLA to expire."; ;
                    msg_es = Assigned.Estimado + " " + Assigned.NAME + @", falta " + Convert.ToString(hours) + " hora para que expire el SLA del boleto " + ticket.COD_TICK + "."; 

                    break;
                case 3:
                    hours = 15;
                    //there is 15 minutes for the ticket HFHF768 SLA to expire
                    msg_en = "Dear " + Assigned.NAME + ", there is " + Convert.ToString(hours) + " minutes for the ticket " + ticket.COD_TICK + ", SLA to expire."; ;
                    msg_es = Assigned.Estimado + " " + Assigned.NAME + @", faltan " + Convert.ToString(hours) + " minutos para que expire el SLA del boleto " + ticket.COD_TICK + "."; 
     
                    break;
                case 4:
                    hours = 0;
                    msg_en = "Dear " + Assigned.NAME + ", you have the ticket " + ticket.COD_TICK + " outside SLA"; ;
                    msg_es = Assigned.Estimado + ": </br></br> " + Assigned.NAME + ", le informamos que el caso asignado ha excedido el SLA.";

                    break;
                case 10:
                    //hours = Convert.ToDouble(ticket.HOU_PRIO) - (Convert.ToDouble(ticket.HOU_PRIO) * 0.07);
                    msg_es = Assigned.Estimado + ": </br></br> " + Assigned.NAME + @", le informamos que el caso asignado está en 70% del progreso y el SLA para este servicio es de " + tipoTicket + " " + tiempoAtencion.TiempoAtencion;
                    break;

                default:
                    hours = 0;
                    msg_en = "Dear " + Assigned.NAME + ", you have the ticket " + ticket.COD_TICK + " outside SLA";
                    msg_es = Assigned.Estimado + ": </br></br> " + Assigned.NAME + ", tienes el ticket " + ticket.COD_TICK + " fuera de SLA";
          
                    break;
            }
            //var Resp = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);

            string html_body =
            @"<span style=""font-family: 'Segoe UI'; color: black; font-size: 14px;"">" + msg_es + "</span>" +
            @"</br>" +
            @"</br>" +
    @"<table width=""600px"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""left"" style=""width: 550px; font-family: 'Segoe UI'; color: black; font-size: 14px; border:1px solid black"">" +
    "<tr>" +
    @"<td style=""width: 20px;"">&nbsp;</td>" +
    @"<td style=""width: 400px;"">&nbsp;</td>" +
    @"</tr>" +
    @"<tr>" +
      
    @"</tr>" +
    @"<tr style=""font-weight: 500; color: black;"">" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align: justify; background: #ffffff; "">" +
            @"<div style=""margin:3px 20px 3px 20px;""><b>NRO. CASO:</b> " + ticket.COD_TICK + "</div>" +
        @"</td>" +
        @"<td>&nbsp;</td>" +

    @"</tr>" +
    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align: justify; background: #ffffff;"">" +
            @"<div style=""margin:3px 20px 3px 20px;""><b>REGISTRO:</b> " + ticket.FEC_TICK + "</div>" +
        @"</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align: justify; background: #ffffff;"">" +
            @"<div style=""margin:3px 20px 3px 20px;""><b>ESTADO:</b> " + ticket.NAM_STAT + "</div>" +
        @"</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align: justify; background: #ffffff;"">" +
            @"<div style=""margin:3px 20px 3px 20px;""><b>SERVICIO:</b> " + ticket.CLAS + "</div>" +
        @"</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align: justify; background: #ffffff;"">" +
            @"<div style=""margin:3px 20px 3px 20px;""><b>CATEGORIA:</b> " + ticket.CATE + "</div>" +
        @"</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align: justify; background: #ffffff;"">" +
            @"<div style=""margin:3px 20px 3px 20px;""><b>USUARIO:</b> " + AffectedUser + "</div>" +
        @"</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align: justify; background: #ffffff;"">" +
            @"<div style=""margin:3px 20px 3px 20px;""><b>SEDE:</b> " + ticket.NOM_Loca + "</div>" +
        @"</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
        @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""7"" style=""text-align: justify; background: #ffffff;"">" +
            @"<div style=""margin:3px 20px 15px 20px;""><b>CIA:</b> " + ticket.NOM_CIA + "</div>" +
        @"</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +

    @"</tr>" +
  @"</table>" +
  @"<table style = ""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif; "" cellspacing= ""0 "" cellpadding= ""1"" border= ""0""> "
                        + "<tr><td> <b>DESCRIPCION: </b></td><td> &nbsp;</td></tr><tr><td>" + ticket.RESUMEN_TICKET + "</td><td> &nbsp;</td></tr> <tr><td></td><td> &nbsp;</td></tr>" +
                        "<tr><td> " + CreateBy + " </td></tr><tr><td>Mesa de Servicio</td></tr><tr><td>Anexo: 123</td></tr><tr><td>Email: soporte@buenaventura.pe</td></tr></table>";
            return html_body;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string CreateDocumentSaleEnd(int ID_DOCU_SALE)
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
            string amarillo = "#ffc107";
            string bordeAmarillo = "1px solid #ffc107";
            string rojo = "#f34235";
            string bordeRojo = "1px solid #f34235";
            //string solorLetraStrong = "#585f69"

            //Cambiar Valores
            string fechaCierre = "24/06/2015";
            string liderTecnico = "Dante Diaz";
            string liderPMO = "No Requerido";
            string bordeResaltado = bordeRojo;
            string fondoResaltado = rojo;

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

            string text_es = "";//, text_en = "";

            try
            {

                //text_en = @"Dear Service Desk, please generate an attention on ITMS for the " + query.COD_TYPE_DOCU_SALE + " " + query.NUM_DOCU_SALE + ", the top date to accomplish this is " + String.Format("{0:MM/dd/yyyy}", query.EXP_DATE);
                text_es = @"Se acaba de resolver la orden de pedido (" + query.COD_TYPE_DOCU_SALE + " " + query.NUM_DOCU_SALE.Trim() + "), " +
                    @"favor proceder a realizar la facturación.";//+
                //@"El informe en el siguiente enlace: <a href=""https://drive.google.com/drive/#folders/0B8Qp-OuBeigffmtoUjgtTXdRZ1ZCODIxRU9JNU5SMVkwRUZlR1NHUVF2d3ItZ2E1cGMyVUU"">Informe</a>";// +
                //@"(Redes, Virtualizaci&oacute;n, Seguridad o Infraestructura) con la finalidad de iniciar el proceso" +
                //@" de entrega y/o implementaci&oacute;n.";
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
        @"<td colspan=""7"" style=""font-size: 22px;color:" + colorLetra + @";"">Orden de Pedido Resuelta</td>" +
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
        @"<td style=""background-color: " + "white;font-weight:bold;" + @";border:" + bordePlomo + @"; color: " + colorLetra + @";"">&nbsp;Fecha de Entrega<br />&nbsp;" + String.Format("{0:dd/MM/yy}", query.EXP_DATE) + @"</td>" +
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
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Closed.png"" />" +
        @"</td>" +
        @"<td style=""background: " + fondoResaltado + @"; color: " + "white" + @";border:" + bordeResaltado + @";"">&nbsp;Fecha de Cierre<br />&nbsp;" + fechaCierre + "</td>" +
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
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @""">&nbsp;Enlace<br/>&nbsp;<a style=""font-weight:bold;text-decoration:none; color:" + colorLetra + @""" href=""http://itms.electrodata.com.pe/DocumentSaleActivity/Index/" + Convert.ToString(ID_DOCU_SALE) + @""">Ver OP</a></td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +

     //Agregar 2 celdas de color Amarillo
     @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr style="" color: white;"">" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Employee.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @";"">&nbsp;Líder Técnico<br />&nbsp;" + liderTecnico + "</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Business.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @";"">&nbsp;Gerente de Proyecto<br />&nbsp;" + liderPMO + "</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    //Fin

    @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""6"" style=""text-align: justify; background: " + plomo + @"; font-size: 14px;font-weight:500;"">" +
            @"<div style=""margin:15px 5px 15px 10px;color:" + colorLetra + @""">" +
                text_es + @"<br /><br />" +
                @"<div style=""font-weight:bold;"">Archivos Adjuntos:</div>" +
                @"<ul style=""margin:0px 10px 0px 10px;padding:0px 10px 0px 10px;color:" + colorLetra + @""">" +
                    //@"<li>Guía de Remisión</li>" +
                    @"<li>Acta de Conformidad</li>" +
                //@"<li>Informe de Servicio</li>" +
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

        public string CreateDocumentSaleEndCook(int ID_DOCU_SALE)
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
            string vendor = "Operaciones Electrodata", quedanDias = "";
            string amarillo = "#ffc107";
            string bordeAmarillo = "1px solid #ffc107";
            string rojo = "#f34235";
            string bordeRojo = "1px solid #f34235";
            //string solorLetraStrong = "#585f69"

            //Cambiar Valores
            string fechaCierre = "30/06/15";
            string liderTecnico = "Edwin Vásquez";
            string liderPMO = "Juan Villafana";
            string bordeResaltado = bordeAzul;
            string fondoResaltado = azul;
            string html_body = null;
            string COD_TYPE_DOCU_SALE = "OPVP 1507-004";
            string NAM_COMP = "Gold Fields La Cima";
            string NAM_COMP_END = "Gold Fields La Cima";
            DateTime EXP_DATE = new DateTime(2015, 08, 05);

            textInfo = cultureInfo.TextInfo;

            //var cia = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1).ToList();

            //var per = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 2).ToList();

            //var query = (from ds in dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == ID_DOCU_SALE).ToList()
            //             join tds in dbc.TYPE_DOCUMENT_SALE on ds.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
            //             join e in cia on ds.ID_COMP equals e.ID_ENTI
            //             join fc in cia on ds.ID_COMP_END equals fc.ID_ENTI
            //             join c in per on ds.ID_CTTS equals c.ID_ENTI into lc
            //             from x in lc.DefaultIfEmpty()
            //             select new
            //             {
            //                 NUM_DOCU_SALE = ds.NUM_DOCU_SALE,
            //                 COD_TYPE_DOCU_SALE = tds.COD_TYPE_DOCU_SALE,
            //                 OS = ds.OS,
            //                 EXP_DATE = ds.EXP_DATE,
            //                 NAM_COMP = e.COM_NAME,
            //                 NAM_COMP_END = fc.COM_NAME,
            //                 CONTACT = (x == null ? "" : x.FIR_NAME + " " + (x.LAS_NAME != null ? x.LAS_NAME : "")),
            //                 ID_DOCU_SALE = ds.ID_DOCU_SALE,
            //                 ID_PERS_ENTI_VEND = ds.ID_PERS_ENTI_VEND
            //             }).First();

            //try
            //{
            //    TimeSpan ts = Convert.ToDateTime(query.EXP_DATE) - DateTime.Now;

            //    quedanDias = Convert.ToString(Decimal.Round(Convert.ToDecimal(ts.TotalDays), 0));

            //    var pers = dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == query.ID_PERS_ENTI_VEND.Value).FirstOrDefault();
            //    var vend = dbe.CLASS_ENTITY.Where(ce => ce.ID_ENTI == pers.ID_ENTI2).FirstOrDefault();
            //    vendor = textInfo.ToTitleCase(vend.FIR_NAME.ToLower() + " " + vend.LAS_NAME.ToLower());
            //}
            //catch
            //{
            //    return "ERROR";
            //}

            //quedanDias

            string text_es = "";//, text_en = "";

            try
            {

                //text_en = @"Dear Service Desk, please generate an attention on ITMS for the " + query.COD_TYPE_DOCU_SALE + " " + query.NUM_DOCU_SALE + ", the top date to accomplish this is " + String.Format("{0:MM/dd/yyyy}", query.EXP_DATE);
                text_es = @"Se acaba de resolver la orden de pedido (" + COD_TYPE_DOCU_SALE + "), " +
                    @"favor proceder a realizar la facturación.";//+
                //@"El informe en el siguiente enlace: <a href=""https://drive.google.com/drive/#folders/0B8Qp-OuBeigffmtoUjgtTXdRZ1ZCODIxRU9JNU5SMVkwRUZlR1NHUVF2d3ItZ2E1cGMyVUU"">Informe</a>";// +
                //@"(Redes, Virtualizaci&oacute;n, Seguridad o Infraestructura) con la finalidad de iniciar el proceso" +
                //@" de entrega y/o implementaci&oacute;n.";
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
        @"<td colspan=""7"" style=""text-align:right;font-size:20px;color:" + colorLetra + @";"">" + COD_TYPE_DOCU_SALE + @"</td>" +
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
        @"<td colspan=""7"" style=""font-size: 22px;color:" + colorLetra + @";"">Orden de Pedido Resuelta</td>" +
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
        @"<td style=""background: " + blanco + @";border:" + bordePlomo + @"; color: " + colorLetra + @";"">&nbsp;" + COD_TYPE_DOCU_SALE + @"<br />&nbsp;" + "</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Stop-Clock.png"" />" +
        @"</td>" +
        @"<td style=""background-color: " + "white;font-weight:bold;" + @";border:" + bordePlomo + @"; color: " + colorLetra + @";"">&nbsp;Fecha de Entrega<br />&nbsp;" + String.Format("{0:dd/MM/yy}", EXP_DATE) + @"</td>" +
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
        @"<div style=""margin:0px 0px 0px 4px;"">" + NAM_COMP + (NAM_COMP == NAM_COMP_END ? "" : "<br />" + NAM_COMP_END) + "</div>" +
        @"</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Closed.png"" />" +
        @"</td>" +
        @"<td style=""background: " + fondoResaltado + @"; color: " + "white" + @";border:" + bordeResaltado + @";"">&nbsp;Fecha de Cierre<br />&nbsp;" + fechaCierre + "</td>" +
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
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @""">&nbsp;Enlace<br/>&nbsp;<a style=""font-weight:bold;text-decoration:none; color:" + colorLetra + @""" href=""http://itms.electrodata.com.pe/DocumentSaleActivity/Index/" + Convert.ToString(ID_DOCU_SALE) + @""">Ver OP</a></td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +

     //Agregar 2 celdas de color Amarillo
     @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr style="" color: white;"">" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Employee.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @";"">&nbsp;Líder Técnico<br />&nbsp;" + liderTecnico + "</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
        @"<td style=""text-align: center; background-color: " + azul + @";border:" + bordeAzul + @"; color: white; height: 52px;"">" +
            @"<img src=""" + IpPublica + @"Content/Images/Mail/Business.png"" />" +
        @"</td>" +
        @"<td style=""background: " + blanco + @"; color: " + colorLetra + @";border:" + bordePlomo + @";"">&nbsp;Gerente de Proyecto<br />&nbsp;" + liderPMO + "</td>" +
        @"<td style=""background: " + blanco + @";"">&nbsp;</td>" +
        @"<td>&nbsp;</td>" +
    @"</tr>" +
    //Fin

    @"<tr>" +
        @"<td colspan=""9"">&nbsp;</td>" +
    @"</tr>" +

    @"<tr>" +
        @"<td>&nbsp;</td>" +
        @"<td colspan=""6"" style=""text-align: justify; background: " + plomo + @"; font-size: 14px;font-weight:500;"">" +
            @"<div style=""margin:15px 5px 15px 10px;color:" + colorLetra + @""">" +
                text_es + @"<br /><br />" +
                @"<div style=""font-weight:bold;"">Archivos Adjuntos:</div>" +
                @"<ul style=""margin:0px 10px 0px 10px;padding:0px 10px 0px 10px;color:" + colorLetra + @""">" +
                @"<li>HEA</li>" +
                //@"<li>Guía de Remisión</li>" +
                //@"<li>Acta de Conformidad</li>" +
                //@"<li>Informe Mensual</li>" +
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

        public string TicketReportTemplate(int opcion)
        {
            string Cuenta = "";
            if (opcion == 1)
            {
                Cuenta = "Electrodata";
            }
            if (opcion == 2)
            {
                Cuenta = "Brocal";
            }

            string html_body = @"<div style=""font-size: 12px;font-family: Verdana;"">" +
                            @"Estimados,<br /><br />" +
                            @"<div>Adjuntamos el reporte de tickets diarios, tener en cuenta que este reporte es solamente para la cuenta " + Cuenta + ".</div> <br />" +
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
            return html_body;
        }

        public string PlantillaRptActividades()
        {
            DateTime desde = DateTime.Now.AddDays(-7);
            DateTime hasta = DateTime.Now;
            string html_body = @"<div style=""font-size: 12px;font-family: Verdana;"">" +
                            @"Estimados,<br /><br />" +
                            @"<div>Adjuntamos el reporte semanal de actividades: " + desde.Year + "/" + desde.Month + "/" + desde.Day + " - " + hasta.Year + "/" + hasta.Month + "/" + hasta.Day + ".</div> <br />" +
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
            return html_body;
        }

        public string CreateTicketBOC(int id, int IdEstado)
        {
            try
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
                                  ID_TICK = t.ID_TICK,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.CLA_STAT.ToLower()),
                                  NAM_SCLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_CLAS = textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SCAT = textInfo.ToTitleCase(sc.NAM_CATE.ToLower()),
                                  NAM_CATE = textInfo.ToTitleCase(c.NAM_CATE.ToLower()),
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  ID_DOCU_SALE = t.ID_DOCU_SALE,
                                  ID_ACCO = t.ID_ACCO,
                                  SUMMARY = t.SUM_TICK
                              }).First();

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                                  (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                       }).First().User;

                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value
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

                var ticketComentario = (from dt in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == ticket.ID_TICK).ToList()
                                        select new
                                        {
                                            id_deta_tick = dt.ID_DETA_TICK,
                                            coment = dt.COM_DETA_TICK == null ? "" : dt.COM_DETA_TICK
                                        }
                                         ).OrderByDescending(dt => dt.id_deta_tick).FirstOrDefault();

                string comentario = "";

                if (ticketComentario != null && IdEstado == 4)
                    comentario = ticketComentario.coment;

                if (comentario == "" || IdEstado == 1)
                    comentario = ticket.SUMMARY;

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

                string Color = ticket.COL_STAT;

                if (IdEstado == 1) //Solo para creación de tickets
                {
                    switch (ticket.ID_PRIO)
                    {
                        case 1:
                            Color = "#BA141A";
                            break;
                        case 2:
                            Color = "#FF8400";
                            break;
                        case 3:
                            Color = "#F2BA0C";
                            break;
                        case 4:
                            Color = "#2D5C88";
                            break;
                        case 5:
                            Color = "#333333";
                            break;
                        default:
                            break;
                    }
                }

                string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                    @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%;background-color:" + Color + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + ticket.NAM_STAT + "-" + (emp.Length > 0 ? emp + " - " : "") + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dd/MM/yyyy}", DateTime.Now) + @"</div></td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" +
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
                        @"<tr style=""background-color:" + Color + @""">" +
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
                            @"<td style=""width:140px;""><b>User</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + AffectedUser + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Priority</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + ticket.NAM_PRIO + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Class</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticket.NAM_CLAS + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Sub Class</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticket.NAM_SCLAS + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Assigned Staff</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + StaffAssigned + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +

                        (ticket.ID_PRIO == 5 ? "" : (
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Estimated Attention Time</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @""">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + sla + @" Hours</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>")) +

                        schedule_es +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";""><b>Commentary</b></td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">:</td>" +
                            @"<td style=""border-top:1px solid " + Color + @";border-bottom:1px solid " + Color + @";"">" + comentario + @"</td>" +
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
                        @"<tr style=""background-color:" + Color + @""">" +
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
            catch
            {
                return null;
            }
        }

        public string CreateTicketChatITMS(int id)
        {
            try
            {

                var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                              select new
                              {
                                  ID = t.ID_TICK,
                                  TITLE = t.TITLE_TICK,
                                  CREA = t.FEC_TICK,
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  SUMMARY = t.SUM_TICK
                              }).First();

                string CreateBy = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI).ToList()
                                   join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                   select new
                                   {
                                       User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                              (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
                                   }).First().User;

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

                string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                    @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%;background-color:#6360A0;color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">Ticket Creado desde el Chat ITMS-" + id + @"</td>" + @"<td style=""width:15px;""></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td colspan=""3""><div style=""font-size:22px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">Chat ITMS Electodata System</div></td>" +
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
                        @"<tr style=""background-color:#6360A0"">" +
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
                            @"<td style=""border-top:1px solid #6360A0;border-bottom:1px solid #6360A0;""><b>Titulo</b></td>" +
                            @"<td style=""border-top:1px solid #6360A0;border-bottom:1px solid #6360A0;"">:</td>" +
                            @"<td style=""border-top:1px solid #6360A0;border-bottom:1px solid #6360A0;"">" + ticket.TITLE + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>Descripción</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + ticket.SUMMARY + @"</td>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
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
                            @"<td><b>Fecha del Ticket</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticket.CREA + @"</td>" +
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
                        @"<tr style=""background-color:#6360A0"">" +
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
                            @"<td style=""width:140px;""><b>Asignar Ticket</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td><a href=""" + IpPublica + @"Ticket/ListaTicketPortal"""">View Ticket Details</a></td>" +
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
                        @"<tr style=""background-color:#6360A0"">" +
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
            catch
            {
                return null;
            }
        }


        public string CreateDetailsCommentTicket(int ID_DETA_TICK)
        {
            try
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
                                  ID_STAT = t.ID_STAT,
                                  ID_PRIO = t.ID_PRIO,
                                  ID_STAT_END = t.ID_STAT_END,
                                  COD_TICK = t.COD_TICK,
                                  NAM_PRIO = textInfo.ToTitleCase(p.NAM_PRIO.ToLower()),
                                  HOU_PRIO = p.HOU_PRIO,
                                  NAM_STAT = textInfo.ToTitleCase(e.NAM_STAT.ToLower()),
                                  NAM_SCLAS = scl.NAM_CATE,
                                  NAM_CLAS = cl.NAM_CATE,
                                  NAM_SCAT = sc.NAM_CATE,
                                  NAM_CATE = c.NAM_CATE,
                                  ID_PERS_ENTI = t.ID_PERS_ENTI,
                                  ID_PERS_ENTI_END = t.ID_PERS_ENTI_END,
                                  ID_PERS_ENTI_ASSI = t.ID_PERS_ENTI_ASSI,
                                  UserId = t.UserId,
                                  DAT_EXPI_TICK = t.DAT_EXPI_TICK,
                                  CREA = t.FEC_TICK,
                                  SCHE = t.FEC_INI_TICK,
                                  COL_STAT = e.COL_INDV_STAT,
                                  EXP_HOUR = p.HOU_PRIO - t.MINUTES / 60,
                                  ID_ACCO = t.ID_ACCO,
                                  SUMMARY = t.SUM_TICK
                              }).First();

                string AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                       join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                       select new
                                       {
                                           User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),

                                       }).First().User;

                int tipoUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                    //join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    User = pe.ID_TYPE_CLIE.Value

                                }).First().User;

                string StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                            User = textInfo.ToTitleCase((ce.FIR_NAME == null ? "" : ce.FIR_NAME.ToLower()) + ' ' + (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                            //textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' + ce.LAS_NAME.ToLower()),
                                        }).First().User;
                //Portal Usuario
                string CreateBy = "";
                try
                {
                    CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
                                    (ce.LAS_NAME == null ? "" : ce.LAS_NAME.ToLower())),
                                }).First().User;
                }
                catch
                {
                    CreateBy = AffectedUser;
                }

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

                //Para Tickets resueltos
                if (ticket.ID_STAT_END == 4)
                {
                    var ticketComentario = (from dt in dbc.DETAILS_TICKET.Where(dt => dt.ID_DETA_TICK == ID_DETA_TICK).ToList()
                                            select new
                                            {
                                                id_deta_tick = dt.ID_DETA_TICK,
                                                coment = dt.COM_DETA_TICK == null ? "" : dt.COM_DETA_TICK
                                            }
                                         ).OrderByDescending(dt => dt.id_deta_tick).FirstOrDefault();
                    /// DE NO RECIBIR CONFORMIDAD
                    resuelto = @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Detalle</b></td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                            @"<td style=""border-bottom:1px solid " + ticket.COL_STAT + @";"">" + ticket.SUMMARY + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td><b>Accion Realizada</b></td>" +
                            @"<td>:</td>" +
                            @"<td>" + ticketComentario.coment + @"</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>";
                    conformidad = "Solicitamos nos brinde su conformidad en la atención brindada.Cualquier duda o consulta, comunicarse con nosotros." +
                                    @"<br><b>De no recibir conformidad al presente correo en un tiempo de 48 horas se procederá con el cierre de su solicitud entendiendo que la misma ha sido atendida de acuerdo a lo requerido." + @"</b>" +
                                    "<br>Best Regards | Saludos Cordiales.";
                }


                string header = @"<table width=""600px"" cellspacing=""0""cellpadding=""0"" border=""0"" bgcolor=""#ffffff"" align=""center"" style=""width:600px;"">" +
                    @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%;background-color:" + ticket.COL_STAT + @";color:white;font-family:'Segoe UI',Verdana,sans-serif;"">" +
                        @"<tr>" +
                            @"<td></td>" +
                            @"<td colspan=""3"" style=""text-align:right;font-size:20px;"">" + ticket.NAM_STAT + " - " + ticket.COD_TICK + @"</td>" + @"<td style=""width:15px;""></td>" +
                        @"</tr>" +
                        @"<tr><td></td>" +
                            @"<td><div style=""font-size:18px; font-family:'Segoe UI',Verdana,sans-serif;color:white;"">" + String.Format("{0:dddd MMMM dd yyyy}", DateTime.Now) + @"</div></td>" +
                            @"<td></td>" + @"<td></td>" + @"<td></td>" +
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
                            @"<td style=""width:140px;""><b>Usuario</b></td>" +
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
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";""><b>Tiempo Estimado de Atención</b></td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @""">:</td>" +
                            @"<td style=""border-top:1px solid " + ticket.COL_STAT + @";border-bottom:1px solid " + ticket.COL_STAT + @";"">" + sla + @" Horas</td>" +
                            @"<td>&nbsp;</td>" +
                        @"</tr>" +
                        resuelto +
                        schedule_es +
                                    @"</table>" +
                                @"</td>" +
                            @"</tr>" +

                            @"<tr>" +
                                @"<td colspan=""3"">&nbsp;</td>" +
                            @"</tr>" +


                            // COMENTARIO

                            @"<tr>" +
                @"<td colspan=""3"">" +
                    @"<table style=""width:100%; color:white;font-size:14px;font-family:'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr style=""background-color:" + ticket.COL_STAT + @""">" +
                            @"<td style=""width:15px;"">&nbsp;</td>" +
                            @"<td colspan=""5"">Comentario agregado</td>" +
                        @"</tr>" +
                        @"<tr>" +
                            @"<td colspan=""6"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                    @"<table style=""width:100%;font-size:14px;font-family:Calibri,'Segoe UI',Verdana,sans-serif;"" cellspacing=""0"" cellpadding=""1"" border=""0"">" +
                        @"<tr>" +
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                            @"<td style=""width:140px;""><b>Comentario</b></td>" +
                            @"<td style=""width:15px;"">:</td>" +
                            @"<td>" + detail_ticket.COM_DETA_TICK + "</td>" + // aqui comentario
                            @"<td style=""width:30px;"">&nbsp;</td>" +
                        @"</tr>" +
                    @"</table>" +
                @"</td>" +
            @"</tr>" +

            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
                @"</tr>" +

                            // COMENTARIO





                            (ticket.ID_ACCO.Value == 1 && detail_ticket.ID_TYPE_DETA_TICK.Value == 3 && detail_ticket.ID_STAT.Value == 4 && tipoUser == 2 ? "" : (
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
                            @"<td><a href=""" + IpPublica + @"DetailsTicket/Index/" + Convert.ToString(ID_TICK) + @""">View Ticket Details</a></td>" +
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
                @"</tr>")) +

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
                    conformidad +
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
            @"</tr>" +
            @"<tr>" +
                @"<td colspan=""3"">&nbsp;</td>" +
            @"</tr>" +
    @"</table>";
                return header;
            }
            catch
            {
                return null;
            }

        }

        public string TicketAltaBajaMinsur(string sumtik, string ticket, string envioflag)
        {

            if (envioflag == "A")
            {
                string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +


                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 10px 10px 10px;"">" +
                                       @"Estimados. <br /><br />" +
                                       @"Se ha iniciado el proceso de incorporación. En ese sentido, agradeceremos gestionar las tareas que se encuentran dentro del ticket de acuerdo a su posición y/o rol en la organización. De esta manera aseguraremos que cuente con lo necesario desde su primer día de trabajo.  <br /><br />" +

                                       @" TICKET: <b>" + ticket + "<b><br /><br />" +
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
            else
            {
                string cadena3 = @"<tr>" +
                                @"<td>&nbsp;</td>" +


                               @"<td style=""background-color: #f1f1f1;"">" +
                                   @"<div style="" text-align:justify; font-size: 14px; padding:10px 10px 10px 10px;"">" +
                                       @"Estimados. <br /><br />" +
                                       @"Se ha iniciado el proceso de baja. En ese sentido, agradeceremos gestionar las tareas que se encuentran dentro del ticket de acuerdo a su posición y/o rol en la organización. De esta manera nos aseguraremos de cesar correctamente al usuario.  <br /><br />" +

                                       @" TICKET: <b>" + ticket + "<b><br /><br />" +
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



        }

    }
}




