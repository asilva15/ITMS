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
    class FinancialTemplate
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
        public FinancialTemplate()
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

        public string PettyCash(REQUEST_EXPENSE re)
        {
            //try
            //{
            string color = "#7cbb00";

            string fecha = String.Format("{0:D}", DateTime.Now);
            string namepaga = "";

            PERSON_ENTITY requester = null, approval = null, paga = null;
            textInfo = cultureInfo.TextInfo;

            //traemos el importe pendiente
            var amount = (from de in dbc.MontoRecepcion(re.ID_REQU_EXPE) select new { de.AMOUNT });
            decimal TotalRequest = Convert.ToDecimal(re.AMOUNT); //TOTAL DE SOLICITUD
            decimal TotalRecepcion = amount.Sum(x => Convert.ToDecimal(x.AMOUNT)); // TOTAL DE RENDICIÓN

            decimal MontoReembolsar =  TotalRecepcion - TotalRequest;


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
                             NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN,

                             rex.JUSTIFICATION,
                             rex.DESTINATION
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
            catch(Exception e)
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
         @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Aprueba</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + nameaproval + "</td>" +
             @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Justificación</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.JUSTIFICATION + "</td>" +
             @"<td></td>" +
         @"</tr>" +
         (query.DESTINATION == null ? "" :
         @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Destino</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.DESTINATION + "</td>" +
             @"<td></td>" +
         @"</tr>") +
         (namepaga == "" || re.ID_STAT_REQU_EXPE == 2 ? "" :
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
         (re.ID_STAT_REQU_EXPE != 9 ? "" :
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @"; font-family: Calibri"">Monto Rendido</td>" +
            @"<td style=""text-align:center; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @";"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @";"">S/. " + TotalRecepcion + " Nuevos Soles</td>" +
             @"<td></td>" +
        @"</tr>" +
        @"<tr>" +
             @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @"; font-family: Calibri"">Pendiente Devolver al Trabajador</td>" +
            @"<td style=""text-align:center; border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @";"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;border-top:1px solid " + color + @"; border-bottom:1px solid " + color + @";"">S/. " + MontoReembolsar + " Nuevos Soles</td>" +
             @"<td></td>" +
        @"</tr>") +
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

        }


        
        public string CuerpoContablidadDevolucionUsuario(REQUEST_EXPENSE rs)
        {
          
            string color = "#7cbb00";

            string fecha = String.Format("{0:D}", DateTime.Now);
            string namepaga = "";

            PERSON_ENTITY requester = null, approval = null, paga = null;
            textInfo = cultureInfo.TextInfo;

            //traemos el importe pendiente
            var amount = (from de in dbc.MontoRecepcion(rs.ID_REQU_EXPE) select new { de.AMOUNT });
            decimal TotalRequest = Convert.ToDecimal(rs.AMOUNT); //TOTAL DE SOLICITUD
            decimal TotalRecepcion = amount.Sum(x => Convert.ToDecimal(x.AMOUNT)); // TOTAL DE RENDICIÓN

            decimal MontoPendiente = TotalRequest - TotalRecepcion;


            var query = (from dsn in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == rs.ID_DELI_SUST).ToList()
                         join rex in dbc.REQUEST_EXPENSE on dsn.ID_DELI_SUST equals rex.ID_DELI_SUST
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
                             NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN,
                             FechaPago = String.Format("{0:d}", dsn.DOC_DATE),
                             rex.JUSTIFICATION,
                             rex.DESTINATION
                         }).First();

            var col = dbc.STATUS_REQUEST_EXPENSE.First(x => x.ID_STAT_REQU_EXPE == rs.ID_STAT_REQU_EXPE);

            color = col.COLOR.ToString();

            string DateRequest = Convert.ToString(query.DRequest);
            string DateApprove = Convert.ToString(query.DRequest);
            string FechaPaga = Convert.ToString(query.FechaPago);
            string monto = Convert.ToString(query.AMOUNT);
            string montorendido = Convert.ToString(TotalRecepcion);
            string montopendiente = Convert.ToString(MontoPendiente);


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
            catch (Exception e)
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
            @"<td colspan=""4"" style=""text-align: right; font-size: 20px; color: white;"">" + Convert.ToString(query.NAM_TYPE_DELI_SUST) +
                  @": " + Convert.ToString(query.COD_REQU_EXPE) + @" - Pendiente de Devolución" +@"</td>" +
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
         @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Aprueba</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + nameaproval + "</td>" +
             @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Justificación</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.JUSTIFICATION + "</td>" +
             @"<td></td>" +
         @"</tr>" +

        (rs.ID_TYPE_DELI_SUST == 1 ? "" :
        @"<tr>" +
            @"<td></td>" +
            @"<td></td>" +
            @"<td style=""font-weight: bold; font-family: Calibri"">Destino</td>" +
            @"<td style=""text-align:center;"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;"">" + query.DESTINATION + "</td>" +
             @"<td></td>" +
         @"</tr>") +
        @"<tr>" +
            @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; border-top:1px solid " + color + @"; font-family: Calibri"">Monto Solicitud</td>" +
            @"<td style=""text-align:center; border-top:1px solid " + color + @";"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;border-top:1px solid " + color + @"; "">S/. " + monto + " Nuevos Soles</td>" +
             @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; border-top:1px solid " + color + @";font-family: Calibri"">Monto Rendido</td>" +
            @"<td style=""text-align:center; border-top:1px solid " + color + @";"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;border-top:1px solid " + color + @";"">S/. " + montorendido + " Nuevos Soles</td>" +
             @"<td></td>" +
         @"</tr>" +
         @"<tr>" +
            @"<td></td>" +
             @"<td></td>" +
            @"<td style=""font-weight: bold; border-top:2px solid " + color + @"; border-bottom:2px solid " + color + @"; font-family: Calibri"">Pendiente Devolución</td>" +
            @"<td style=""text-align:center; border-top:2px solid " + color + @"; border-bottom:2px solid " + color + @";"">:</td>" +
            @"<td colspan=""5"" style=""font-family:Calibri;border-top:2px solid " + color + @"; border-bottom:2px solid " + color + @";"">S/. " + montopendiente + " Nuevos Soles</td>" +
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


        }
        
    }
}
