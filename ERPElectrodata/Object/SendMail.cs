using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ERPElectrodata.Models;
using ERPElectrodata.Objects.CreateMail;
using ERPElectrodata.Object.Plugins;
using System.Data.Entity;
using System.Threading;
using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace ERPElectrodata.Objects
{
    public class SendMail
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dba = new AppLogEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
        DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

        //public CDO.Message message = new CDO.Message();
        private string fromEmail = "";
        private string pass = "";



        public MailMessage mailMessage = new MailMessage();
        public SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        //private string fromEmail = "";
        //private string pass = "";

        // PluginSmtp newMail = new PluginSmtp();

        //// GET: /SendMail/
        //public SendMail()
        //{
        //    textInfo = cultureInfo.TextInfo;

        //    //fromEmail = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 4 && x.ID_PARA == 24).VAL_ACCO_PARA.ToString();
        //    //pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 4 && x.ID_PARA == 25).VAL_ACCO_PARA.ToString();

        //    //CDO.IConfiguration configuration = message.Configuration;
        //    //ADODB.Fields fields = configuration.Fields;

        //    //ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        //    //field.Value = "smtp.zoho.com";

        //    //field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        //    //field.Value = 465;

        //    //field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        //    //field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        //    //field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        //    //field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        //    //field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        //    //field.Value = fromEmail;

        //    //field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        //    //field.Value = pass;

        //    //field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        //    //field.Value = "true";

        //    //fields.Update();

        //    fromEmail = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 4 && x.ID_PARA == 24).VAL_ACCO_PARA.ToString();
        //    pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 4 && x.ID_PARA == 25).VAL_ACCO_PARA.ToString();

        //    MailAddress fromAddress = new MailAddress(fromEmail, "IT Management System");
        //    // MailMessage mailMessage = new MailMessage("esalazar@electrodata.com.pe" "esalazar@electrodata.com.pe", "text", "Text");
        //    //mailMessage.fr;
        //    mailMessage.From = fromAddress;

        //    smtp.EnableSsl = true;
        //    //smtp.
        //    NetworkCredential basicCredential = new NetworkCredential(fromEmail, pass);
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Credentials = basicCredential;
        //    //smtp.Send(mailMessage);
        //}

        public string NewTicket(int ID_TICK)
        {
            try
            {
                int ID_ACCO = 0, ID_QUEU = 0, ID_PERS_ENTI_ASSI = 0;

                int QUE_SUPE_MAIL = 0, IdCoor;

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                ID_ACCO = Convert.ToInt32(ticket.ID_ACCO);
                ID_QUEU = Convert.ToInt32(ticket.ID_QUEU);
                ID_PERS_ENTI_ASSI = Convert.ToInt32(ticket.ID_PERS_ENTI_ASSI);
                try
                {
                    QUE_SUPE_MAIL = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 30)
                        .Where(x => x.ID_ACCO == ID_ACCO).Count();
                }
                catch { }


                var coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                        .Single(x => x.ID_ACCO == ticket.ID_ACCO);


                var qEma = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);

                string mailTo = qEma.EMA_ELEC, mail_coor = null, mail_group = null;

                if (String.IsNullOrEmpty(mailTo))
                {
                    mailTo = "itms@electrodata.com.pe";
                }
                //Enviando correo a cuenta de correo en el cliente.
                if (!String.IsNullOrEmpty(qEma.EMA_PERS))
                {
                    mailTo = mailTo + ", " + qEma.EMA_PERS;
                }


                if (QUE_SUPE_MAIL == 0)
                {
                    IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);

                }
                else
                {
                    IdCoor = dbc.ACCOUNT_QUEUE.Where(x => x.ID_QUEU == ID_QUEU).Single(x => x.ID_ACCO == ID_ACCO).ID_PERS_ENTI_CORD.Value;

                }

                mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor).EMA_ELEC;

                if (String.IsNullOrEmpty(mail_coor))
                {
                    mail_coor = "itms@electrodata.com.pe";
                }

                int caseSwitch = Convert.ToInt32(ticket.ID_STAT);
                string Subjet1 = "";
                switch (caseSwitch)
                {
                    case 1:
                        Subjet1 = "New Ticket / Nuevo Boleto - ";
                        break;
                    case 2:
                        Subjet1 = "Cancelled Ticket / Boleto Cancelado - ";
                        break;
                    case 3:
                        Subjet1 = "New Ticket / Nuevo Boleto - ";
                        break;
                    case 4:
                        Subjet1 = "Resolved Ticket / Boleto Resuelto - ";
                        break;
                    case 5:
                        Subjet1 = "Scheduled Ticket / Boleto Programado - ";
                        break;
                    case 6:
                        Subjet1 = "Closed Ticket / Boleto Cerrado - ";
                        break;
                    default:
                        Subjet1 = "New Ticket / Nuevo Boleto - ";
                        break;
                }
                Random rnd = new Random();
                decimal xx = rnd.Next(1, 1000);

                Body body = new Body();
                string body_html = body.CreateTicket(ID_TICK);

                //copiando al grupo si el ticket es por una OP
                if (ticket.ID_DOCU_SALE != null)
                {
                    var qQueue = dbc.ACCOUNT_QUEUE
                        .Where(x => x.ID_ACCO == ID_ACCO)
                        .Single(x => x.ID_QUEU == ticket.ID_QUEU);

                    if (!String.IsNullOrEmpty(qQueue.EMA_ACCO_QUEU))
                    {
                        mail_group = (mail_coor.Length > 0 ? mail_coor + "," : "") + qQueue.EMA_ACCO_QUEU;
                    }
                }

                //Correo el jefe inmediato superior
                //var bi = dbe.EMAIL_BOSS_INMSUP(ticket.ID_PERS_ENTI_ASSI).ToList();
                //if (bi.Count() > 0) {
                //    if (mail_coor.Contains(bi.First().EMA_ELEC)==false) { 
                //        mail_coor = (mail_coor.Length > 0 ? mail_coor + ";" : "") + bi.First().EMA_ELEC;
                //    }
                //}
                PluginSmtp newMail = new PluginSmtp();

                var message = newMail.mailMessage;

                // message.To.Add("lsempertegui@electrodata.com.pe");
                message.To.Add(mailTo); //Descomentar

                //string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
                //message.From = fromEmail;
                //message.To = mailTo;



                if (ID_PERS_ENTI_ASSI != IdCoor)
                {
                    if (!String.IsNullOrEmpty(mail_group))
                    {
                        mail_coor = (mail_coor.Length > 0 ? mail_coor + ", " : "") + mail_group;
                    }
                    // message.CC = mail_coor;

                    // message.Bcc.Add(mail_coor); //Descomentar esto
                }
                else
                {
                    if (!String.IsNullOrEmpty(mail_group))
                    {
                        //mail_coor = (mail_coor.Length > 0 ? mail_coor + "; " : "") + mail_group;
                        // message.CC = mail_group;
                        //message.Bcc.Add(mail_group); //Descomentar esto
                    }
                }

                //message.HTMLBody = body_html;
                //message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                //message.Send();

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                message.AlternateViews.Add(htmlView);
                message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message.ToString();

                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = e.Message.ToString();//(e.Message.ToString());
                exc.DES_EXCE = "Send Mail - New Ticket";
                dba.EXCEPTIONs.Add(exc);
                dba.SaveChanges();
            }
        }

        public string UpdateStatus(int ID_DETA_TICK)
        {

            try
            {
                var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);

                int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                //PARA EL CASO QUE TENGA CUENTA DE CORREO EN OUTSOURSING
                var mailCLient = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 32)
                    .Where(x => x.ID_ACCO == ticket.ID_ACCO.Value).Count();

                //COORDINADOR GENERAL DE LA CUENTA
                var coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                        .Single(x => x.ID_ACCO == ticket.ID_ACCO);

                int IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);
                var per_mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor);
                string mail_coor = null;
                //Verificar mail del cliente del coordinador
                if (mailCLient > 0)
                {
                    mail_coor = per_mail_coor.EMA_ELEC + (String.IsNullOrEmpty(per_mail_coor.EMA_PERS) ? "" : "," + per_mail_coor.EMA_PERS);
                }
                else
                {
                    mail_coor = per_mail_coor.EMA_ELEC;
                }
                //Fin Coordinador general

                string mail_sup_queu = null;
                //Supervisor de Queue
                try
                {
                    int id_sup_queu = dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ticket.ID_ACCO.Value)
                                                .First(x => x.ID_QUEU == ticket.ID_QUEU.Value).ID_PERS_ENTI_CORD.Value;

                    var sup_queu = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id_sup_queu);

                    if (ticket.ID_PERS_ENTI_ASSI != id_sup_queu)
                    {
                        if (mailCLient > 0)
                        {
                            mail_sup_queu = sup_queu.EMA_ELEC + (String.IsNullOrEmpty(per_mail_coor.EMA_PERS) ? "" : "," + per_mail_coor.EMA_PERS);
                        }
                        else
                        {
                            mail_sup_queu = sup_queu.EMA_ELEC;
                        }
                    }
                }
                catch
                {
                }

                //Fin Supervisor Queue

                //Responsable del Ticket
                var Resp = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI.Value);

                //Usuario que actualiza el ticket
                var User = dbe.CLASS_ENTITY.Single(x => x.UserId == deta_tick.UserId);
                int ID_ENTI = Convert.ToInt32(User.ID_ENTI);

                var UserUpdate = dbe.PERSON_ENTITY.Single(x => x.ID_ENTI2 == ID_ENTI && x.VIG_PERS_ENTI == true);

                string mailResp = null;

                //string mailUser = null;// newResp.EMA_ELEC;

                // se copia mail a correo del cliente :: responsable del ticket
                if (mailCLient > 0)
                {
                    if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
                    {
                        mailResp = Resp.EMA_ELEC;
                    }
                    if (!String.IsNullOrEmpty(Resp.EMA_PERS))
                    {
                        mailResp = mailResp + "," + Resp.EMA_PERS;
                    }

                    if (Resp.ID_PERS_ENTI != UserUpdate.ID_PERS_ENTI)
                    {
                        mailResp = mailResp + (String.IsNullOrEmpty(UserUpdate.EMA_ELEC) ? "" : "," + UserUpdate.EMA_ELEC)
                            + (String.IsNullOrEmpty(UserUpdate.EMA_PERS) ? "" : "," + UserUpdate.EMA_PERS);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
                    {
                        mailResp = Resp.EMA_ELEC;
                    }
                    if (Resp.ID_PERS_ENTI != UserUpdate.ID_PERS_ENTI)
                    {
                        mailResp = mailResp + (String.IsNullOrEmpty(UserUpdate.EMA_ELEC) ? "" : "," + UserUpdate.EMA_ELEC);
                    }
                }

                if (String.IsNullOrEmpty(mailResp))
                {
                    mailResp = "itms@electrodata.com.pe";
                }

                int caseSwitch = Convert.ToInt32(deta_tick.ID_STAT);
                string status = "", estado = "", code_ticket = ticket.COD_TICK;
                switch (caseSwitch)
                {
                    case 1:
                        status = "Active Ticket";
                        estado = "Boleto Activo";
                        break;
                    case 2:
                        status = "Cancelled Ticket";
                        estado = "Boleto Cancelado";
                        break;
                    case 3:
                        status = "Open Ticket";
                        estado = "Boleto Activo";
                        break;
                    case 4:
                        status = "Resolved Ticket";
                        estado = "Boleto Resuelto";
                        break;
                    case 5:
                        status = "Scheduled Ticket";
                        estado = "Boleto Programado";
                        break;
                    case 6:
                        status = "Closed Ticket";
                        estado = "Boleto Cerrado";
                        break;
                    default:
                        status = "Active Ticket";
                        estado = "Boleto Activo";
                        break;
                }

                Body body = new Body();
                //string body_html = body.CreateDetailsTicket(ID_DETA_TICK);

                //message.From = fromEmail;
                //message.To = mailResp;
                //message.CC = mail_coor + (String.IsNullOrEmpty(mail_sup_queu) ? "" : ";" + mail_sup_queu);
                //message.HTMLBody = body_html;
                //message.Subject = "Update Status - " + status + " " + code_ticket + " / Actualización de Estado - " + estado + " " + code_ticket;
                //message.Send();

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;

                string body_html = body.CreateDetailsTicket(ID_DETA_TICK);
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                string msgcopy = mail_coor + (String.IsNullOrEmpty(mail_sup_queu) ? "" : "," + mail_sup_queu);

                //message.To.Add("lsempertegui@electrodata.com.pe");

                message.To.Add(mailResp); //Descomentar              
                message.CC.Add(msgcopy); //Descomentar             
                message.AlternateViews.Add(htmlView);
                message.Subject = "Update Status - " + status + " " + code_ticket + " / Actualización de Estado - " + estado + " " + code_ticket;
                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        public string TransferTicket(int ID_DETA_TICK)
        {
            try
            {
                var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
                int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);
                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                //PARA EL CASO QUE TENGA CUENTA DE CORREO EN OUTSOURSING
                var mailCLient = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 32)
                    .Where(x => x.ID_ACCO == ticket.ID_ACCO.Value).Count();

                var coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                        .Single(x => x.ID_ACCO == ticket.ID_ACCO);

                int IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);

                var per_mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor);

                string mail_coor = null;
                //Verificar mail del cliente del coordinador
                if (mailCLient > 0)
                {
                    mail_coor = per_mail_coor.EMA_ELEC + (String.IsNullOrEmpty(per_mail_coor.EMA_PERS) ? "" : "," + per_mail_coor.EMA_PERS);
                }
                else
                {
                    mail_coor = per_mail_coor.EMA_ELEC;
                }
                //Fin Coordinador general

                string mailTo = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).EMA_ELEC;
                string AssignadoActual = "", realiza_transfer = "";
                int ID_ASSI_BEFORE = 0;

                string mail_sup_queu = null;
                //SUPERVISOR DE QUEUE
                try
                {
                    int id_sup_queu = dbc.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ticket.ID_ACCO.Value)
                                                .First(x => x.ID_QUEU == ticket.ID_QUEU.Value).ID_PERS_ENTI_CORD.Value;

                    var sup_queu = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id_sup_queu);

                    if (ticket.ID_PERS_ENTI_ASSI != id_sup_queu)
                    {
                        if (mailCLient > 0)
                        {
                            mail_sup_queu = sup_queu.EMA_ELEC + (String.IsNullOrEmpty(per_mail_coor.EMA_PERS) ? "" : "," + per_mail_coor.EMA_PERS);
                        }
                        else
                        {
                            mail_sup_queu = sup_queu.EMA_ELEC;
                        }
                    }
                }
                catch { }

                try
                {
                    int id_enti = dbe.CLASS_ENTITY.Single(ce => ce.UserId == Convert.ToInt32(deta_tick.UserId)).ID_ENTI;
                    realiza_transfer = dbe.PERSON_ENTITY.Single(x => x.ID_ENTI2 == id_enti && x.VIG_PERS_ENTI == true).EMA_ELEC;
                }
                catch
                {

                }

                try
                {
                    var xyz = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                        .Where(x => x.ID_DETA_TICK <= ID_DETA_TICK)
                        .OrderByDescending(x => x.ID_DETA_TICK);

                    if (xyz.Where(x => x.ID_PERS_ENTI != null).Count() <= 1)
                    {
                        try
                        {
                            AssignadoActual = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI_STAR).EMA_ELEC;
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        ID_ASSI_BEFORE = Convert.ToInt32(xyz.Where(x => x.ID_DETA_TICK != ID_DETA_TICK).First().ID_PERS_ENTI);

                        AssignadoActual = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_ASSI_BEFORE).EMA_ELEC;
                    }
                }
                catch
                {

                }

                if (String.IsNullOrEmpty(mailTo))
                {
                    mailTo = "itms@electrodata.com.pe";
                }

                if (String.IsNullOrEmpty(mail_coor))
                {
                    mail_coor = "itms@electrodata.com.pe";
                }

                if (AssignadoActual != "")
                {
                    mailTo = mailTo + "," + AssignadoActual;
                }

                if (realiza_transfer != "")
                {
                    mailTo = mailTo + "," + realiza_transfer;
                }

                int caseSwitch = Convert.ToInt32(deta_tick.ID_STAT);

                string status = "", estado = "", code_ticket = ticket.COD_TICK;
                switch (caseSwitch)
                {
                    case 1:
                        status = "Active Ticket";
                        estado = "Boleto Activo";
                        break;
                    case 2:
                        status = "Cancelled Ticket";
                        estado = "Boleto Cancelado";
                        break;
                    case 3:
                        status = "Open Ticket";
                        estado = "Boleto Activo";
                        break;
                    case 4:
                        status = "Resolved Ticket";
                        estado = "Boleto Resuelto";
                        break;
                    case 5:
                        status = "Scheduled Ticket";
                        estado = "Boleto Programado";
                        break;
                    case 6:
                        status = "Closed Ticket";
                        estado = "Boleto Cerrado";
                        break;
                    default:
                        status = "Active Ticket";
                        estado = "Boleto Activo";
                        break;
                }

                Body body = new Body();
                string body_html = body.TransferTo(ID_DETA_TICK);

                //message.From = fromEmail;
                //message.To = mailTo;
                //message.CC = mail_coor + (String.IsNullOrEmpty(mail_sup_queu) ? "" : ";" + mail_sup_queu);
                //message.HTMLBody = body_html;
                //message.Subject = "Transfer Ticket - " + code_ticket + " / Tranferencia de Boleto - " + code_ticket;
                //message.Send();

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                string msgcopy = mail_coor + (String.IsNullOrEmpty(mail_sup_queu) ? "" : "," + mail_sup_queu);

                //message.To.Add("lsempertegui@electrodata.com.pe");

                message.To.Add(mailTo); //Descomentar              
                message.CC.Add(msgcopy); //Descomentar             
                message.AlternateViews.Add(htmlView);
                message.Subject = "Transfer Ticket - " + code_ticket + " / Tranferencia de Boleto - " + code_ticket;
                newMail.smtp.Send(message);


                return "OK";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        public string Notification()
        {
            try
            {
                var ini = DateTime.Now;
                int count = 0;
                int count1 = 0, ID_TYPE_NOTI = 0;
                int[] array = new int[3];
                array[0] = 1;
                array[1] = 3;
                array[2] = 5;

                int[] arraytt = new int[2];
                arraytt[0] = 1;
                arraytt[1] = 2;

                string cod = "", html_body = null;

                Body _Body = new Body();

                var query = dbc.TICKETs.Where(t => array.Contains((int)t.ID_STAT_END))
                    .Where(t => t.ID_PRIO != 5)
                    .Where(t => arraytt.Contains((int)t.ID_TYPE_TICK))
                    .Where(t => t.ID_ACCO != null)
                    .Where(t => t.ID_ACCO > 0).ToList();

                foreach (TICKET tickets in query)
                {
                    html_body = null;
                    string cc = "";
                    int horas = 0;
                    int minutes = 0;
                    int ID_TICK = Convert.ToInt32(tickets.ID_TICK);
                    var ticket = (from t in dbc.TICKETs
                                  join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                                  where t.ID_TICK == ID_TICK
                                  select new
                                  {
                                      p.HOU_PRIO,
                                      t.COD_TICK,
                                      t.FEC_TICK,
                                      t.MINUTES,
                                      // t.TYPE_TICKET,
                                      t.ID_PERS_ENTI_ASSI,
                                      t.ID_ACCO,
                                      FEC_INI_TICK = t.FEC_INI_TICK == null ? t.FEC_TICK : t.FEC_INI_TICK
                                  }).First();

                    int ID_ACCO = Convert.ToInt32(ticket.ID_ACCO);

                    var mailCLient = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 32)
                    .Where(x => x.ID_ACCO == ticket.ID_ACCO.Value).Count();
                    var Resp = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);
                    string mailTo = null;

                    if (mailCLient > 0)
                    {

                        if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
                        {
                            mailTo = Resp.EMA_ELEC;
                        }
                        if (!String.IsNullOrEmpty(Resp.EMA_PERS))
                        {
                            mailTo = (!String.IsNullOrEmpty(Resp.EMA_PERS) ? mailTo + "," : "") + Resp.EMA_PERS;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
                        {
                            mailTo = Resp.EMA_ELEC;
                        }
                    }

                    if (String.IsNullOrEmpty(mailTo))
                    {
                        mailTo = "itms@electrodata.com.pe";
                    }

                    var deta_tick = dbc.DETAILS_TICKET
                        .Where(x => x.ID_TICK == ID_TICK)
                        .Where(x => x.ID_STAT != null)
                        .OrderByDescending(x => x.ID_DETA_TICK);

                    if (deta_tick.Count() > 0)
                    {
                        DateTime fecha_ini_2 = Convert.ToDateTime(deta_tick.First().FEC_SCHE);
                        if (fecha_ini_2 == DateTime.MinValue)
                        {
                            fecha_ini_2 = Convert.ToDateTime(deta_tick.First().CREATE_DETA_INCI);
                            horas = Convert.ToInt32(DateTime.Now.Subtract(fecha_ini_2).TotalHours);
                            minutes = Convert.ToInt32(ticket.MINUTES) + Convert.ToInt32(DateTime.Now.Subtract(fecha_ini_2).TotalMinutes);
                        }
                        else
                        {
                            //Aún no cumple Scheduled
                            if (DateTime.Now <= fecha_ini_2)
                            {
                                horas = Convert.ToInt32(Convert.ToDouble(ticket.MINUTES) / 60.00);
                                minutes = Convert.ToInt32(ticket.MINUTES);
                            }
                            else
                            {
                                horas = Convert.ToInt32((ticket.MINUTES) / 60.00) - Convert.ToInt32(DateTime.Now.Subtract(fecha_ini_2).TotalHours);
                                minutes = Convert.ToInt32(ticket.MINUTES) + Convert.ToInt32(DateTime.Now.Subtract(fecha_ini_2).TotalMinutes);
                            }

                        }


                    }
                    else
                    {
                        horas = Convert.ToInt32(DateTime.Now.Subtract(ticket.FEC_INI_TICK.Value).TotalHours);
                        minutes = Convert.ToInt32(DateTime.Now.Subtract(ticket.FEC_INI_TICK.Value).TotalMinutes);
                    }

                    if (Convert.ToInt32(ticket.HOU_PRIO) - Convert.ToInt32(horas) == ticket.HOU_PRIO / 2)
                    {
                        //Mitad del tiempo para vencimiento
                        ID_TYPE_NOTI = 1;
                        var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
                            .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();
                        if (index1 == 0)
                        {
                            html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);
                        }
                    }

                    if (ticket.HOU_PRIO * 60 - minutes <= 60 && ticket.HOU_PRIO * 60 - minutes > 15)
                    {
                        //Falta 1 HORA
                        ID_TYPE_NOTI = 2;
                        var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
                            .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();
                        if (index1 == 0)
                        {
                            html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);

                            int ID_COOR = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                            .Single(x => x.ID_ACCO == ID_ACCO).VAL_ACCO_PARA);

                            var Coor = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_COOR);
                            cc = Coor.EMA_ELEC + "," + Coor.EMA_PERS;
                        }
                    }

                    if (ticket.HOU_PRIO * 60 - minutes <= 15 && ticket.HOU_PRIO * 60 - minutes >= 0)
                    {
                        //Faltan Menos de 15 minutos
                        ID_TYPE_NOTI = 3;
                        var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
                            .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();
                        if (index1 == 0)
                        {
                            html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);


                            try
                            {
                                int ID_COOR = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                .Single(x => x.ID_ACCO == ticket.ID_ACCO).VAL_ACCO_PARA);
                                var Coor = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_COOR);
                                cc = Coor.EMA_ELEC + "," + Coor.EMA_PERS;
                            }
                            catch { }
                        }


                    }

                    if (ticket.HOU_PRIO * 60 - minutes < 0)
                    {
                        //Venció SLA
                        ID_TYPE_NOTI = 4;
                        var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
                            .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();

                        if (index1 == 0)
                        {

                            try
                            {
                                int ID_PERS_MANA = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 23).VAL_ACCO_PARA);
                                //cc = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_PERS_MANA).EMA_PERS+";jvillafana@electrodata.com.pe";
                            }
                            catch
                            {

                            }

                            int ID_COOR = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                            .Single(x => x.ID_ACCO == ID_ACCO).VAL_ACCO_PARA);

                            var Coor = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_COOR);
                            cc = Coor.EMA_ELEC + "," + Coor.EMA_PERS; // +",jvillafana@electrodata.com.pe";


                            html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);
                        }
                    }

                    if (html_body != null)
                    {
                        //message.From = fromEmail;
                        //message.To = mailTo;
                        //message.CC = cc;
                        // message.HTMLBody = html_body;
                        // message.Subject = "Alert SLA " + ticket.COD_TICK;
                        //  message.Send();
                        PluginSmtp newMail = new PluginSmtp();
                        var message = newMail.mailMessage;

                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_body, Encoding.UTF8, MediaTypeNames.Text.Html);
                        //message.To.Add("lsempertegui@electrodata.com.pe");
                        message.To.Add(mailTo); //Descomentar              
                        message.CC.Add(cc); //Descomentar             
                        message.AlternateViews.Add(htmlView);
                        message.Subject = "Alert SLA " + ticket.COD_TICK;
                        newMail.smtp.Send(message);

                        TICKET_NOTIFICATION ticket_noti = new TICKET_NOTIFICATION();
                        ticket_noti.ID_TICK = ID_TICK;
                        ticket_noti.ID_TYPE_NOTI = ID_TYPE_NOTI;
                        ticket_noti.CREATED = DateTime.Now;
                        dbc.TICKET_NOTIFICATION.Add(ticket_noti);
                        dbc.SaveChanges();

                    }

                }
                var fin = DateTime.Now;

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            //return "since: " + Convert.ToString(ini) + " to: " + Convert.ToString(fin) + "/n" + cod + " #####: " + count + "-" + count1 + "-" + query.Count() + "xxx"+html_body;
        }

        // Method
        //public string NewTicket(int ID_TICK)
        //{
        //    try
        //    {

        //        var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

        //        var coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
        //                                .Single(x => x.ID_ACCO == ticket.ID_ACCO);

        //        int IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);

        //        var qEma = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);
        //        string mailTo = qEma.EMA_ELEC;

        //        if (String.IsNullOrEmpty(mailTo))
        //        {
        //            mailTo = "itms@electrodata.com.pe";
        //        }
        //        //Enviando correo a cuenta de correo en el cliente.
        //        if (!String.IsNullOrEmpty(qEma.EMA_PERS))
        //        {
        //            mailTo = mailTo + "; " + qEma.EMA_PERS;
        //        }

        //        string mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor).EMA_ELEC;
        //        if (String.IsNullOrEmpty(mail_coor))
        //        {
        //            mail_coor = "itms@electrodata.com.pe";
        //        }


        //        int caseSwitch = Convert.ToInt32(ticket.ID_STAT);
        //        string Subjet1 = "";
        //        switch (caseSwitch)
        //        {
        //            case 1:
        //                Subjet1 = "New Ticket / Nuevo Boleto - ";
        //                break;
        //            case 2:
        //                Subjet1 = "Cancelled Ticket / Boleto Cancelado - ";
        //                break;
        //            case 3:
        //                Subjet1 = "New Ticket / Nuevo Boleto - ";
        //                break;
        //            case 4:
        //                Subjet1 = "Resolved Ticket / Boleto Resuelto - ";
        //                break;
        //            case 5:
        //                Subjet1 = "Scheduled Ticket / Boleto Programado - ";
        //                break;
        //            case 6:
        //                Subjet1 = "Closed Ticket / Boleto Cerrado - ";
        //                break;
        //            default:
        //                Subjet1 = "New Ticket / Nuevo Boleto - ";
        //                break;
        //        }
        //        Random rnd = new Random();
        //        decimal xx = rnd.Next(1, 1000);

        //        Body body = new Body();
        //        string body_html = body.CreateTicket(ID_TICK);

        //        //copiando al grupo si el ticket es por una OP
        //        if (ticket.ID_DOCU_SALE != null)
        //        {
        //            var qQueue = dbc.QUEUEs.Single(x => x.ID_QUEU == ticket.ID_QUEU);
        //            if (String.IsNullOrEmpty(qQueue.EMA_QUEU) == false)
        //            {
        //                mail_coor = (mail_coor.Length > 0 ? mail_coor + "; " : "") + qQueue.EMA_QUEU;
        //            }
        //        }

        //        //Correo el jefe inmediato superior
        //        var bi = dbe.EMAIL_BOSS_INMSUP(ticket.ID_PERS_ENTI_ASSI).ToList();
        //        if (bi.Count() > 0) {
        //            if (mail_coor.Contains(bi.First().EMA_ELEC)==false) { 
        //                mail_coor = (mail_coor.Length > 0 ? mail_coor + ";" : "") + bi.First().EMA_ELEC;
        //            }
        //        }

        //        string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
        //        message.From = fromEmail;
        //        message.To = mailTo;
        //        message.CC = mail_coor;
        //        message.HTMLBody = body_html;
        //        message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
        //        message.Send();
        //        return "OK";
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message.ToString();
        //    }
        //}

        //public string UpdateStatus(int ID_DETA_TICK)
        //{
        //    try
        //    {
        //        var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
        //        int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);
        //        var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

        //        var coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
        //                                .Single(x => x.ID_ACCO == ticket.ID_ACCO);

        //        int IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);

        //        string mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor).EMA_ELEC;

        //        string mailTo = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).EMA_ELEC;

        //        if (String.IsNullOrEmpty(mailTo))
        //        {
        //            mailTo = "itms@electrodata.com.pe";
        //        }

        //        if (String.IsNullOrEmpty(mail_coor))
        //        {
        //            mail_coor = "itms@electrodata.com.pe";
        //        }

        //        int caseSwitch = Convert.ToInt32(deta_tick.ID_STAT);

        //        string status = "", estado = "", code_ticket = ticket.COD_TICK;
        //        switch (caseSwitch)
        //        {
        //            case 1:
        //                status = "Active Ticket";
        //                estado = "Boleto Activo";
        //                break;
        //            case 2:
        //                status = "Cancelled Ticket";
        //                estado = "Boleto Cancelado";
        //                break;
        //            case 3:
        //                status = "Open Ticket";
        //                estado = "Boleto Activo";
        //                break;
        //            case 4:
        //                status = "Resolved Ticket";
        //                estado = "Boleto Resuelto";
        //                break;
        //            case 5:
        //                status = "Scheduled Ticket";
        //                estado = "Boleto Programado";
        //                break;
        //            case 6:
        //                status = "Closed Ticket";
        //                estado = "Boleto Cerrado";
        //                break;
        //            default:
        //                status = "Active Ticket";
        //                estado = "Boleto Activo";
        //                break;
        //        }

        //        Body body = new Body();
        //        string body_html = body.CreateDetailsTicket(ID_DETA_TICK);

        //        message.From = fromEmail;
        //        message.To = mailTo;
        //        message.CC = mail_coor;
        //        message.HTMLBody = body_html;
        //        message.Subject = "Update Status - " + status + " " + code_ticket + " / Actualización de Estado - " + estado + " " + code_ticket;
        //        message.Send();

        //        return "OK";
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message.ToString();
        //    }
        //}

        public string ExpirationContract(int id = 0)
        {

            //   string mailTo1 = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == 1010).EMA_ELEC;
            //   if (String.IsNullOrEmpty(mailTo1))
            //   {
            //       mailTo1 = "cristian.arevalo@electrodata.com.pe";
            //   }

            //   Random rnd = new Random();
            //   decimal xx = rnd.Next(1, 1000);

            //   string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
            // //  message.From = fromEmail;
            //  // message.To = mailTo1;
            //  // message.BodyPart.Charset = "utf-8";
            //   message.CreateMHTMLBody(strHostName + "Mail/SendMailExpirationContract?id=" + id + "&_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone);
            ////   message.Subject = "Expiration Contract / Expiración de Contrato";
            ////   message.Send();           

            //   var message = newMail.mailMessage;
            //   AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_body, Encoding.UTF8, MediaTypeNames.Text.Html);
            //   message.To.Add("lsempertegui@electrodata.com.pe");
            //   //message.To.Add(mailTo1); //Descomentar 
            //   message.AlternateViews.Add(htmlView);
            //   message.Subject = "Expiration Contract / Expiración de Contrato";
            //   newMail.smtp.Send(message);


            return "OK";
        }

        public string ManttoContract(int id = 0, int id1 = 0)
        {

            //try
            //{
            //    string mailTo1 = "";

            //    //Correo el jefe inmediato superior
            //    //var bi = dbe.EMAIL_BOSS_INMSUP(id).ToList();
            //    //mailTo1 = mailTo1 + (bi == null ? "" : (bi.First().EMA_ELEC == null ? "" : bi.First().EMA_ELEC + ";"));

            //    //Resto de correos
            //    var query = (from ed in dbe.EMAIL_DESTINATION.Where(x => x.ID_TYPE_EMAI == id1 && x.VIG_EMAI_DEST == true)
            //                 join pe in dbe.PERSON_ENTITY on ed.ID_PERS_ENTI equals pe.ID_PERS_ENTI
            //                 select new
            //                 {
            //                     pe.EMA_ELEC,
            //                 });

            //    foreach (var a in query.ToList())
            //    {
            //        if (mailTo1.Contains(a.EMA_ELEC) == false)
            //        {
            //            mailTo1 = mailTo1 + a.EMA_ELEC + ";";
            //        }
            //    }

            //    Random rnd = new Random();
            //    decimal xx = rnd.Next(1, 1000);

            //    //mailTo1 = "carevalo@electrodata.com.pe";

            //    string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
            //    message.From = fromEmail;
            //    message.To = mailTo1;
            //    message.BodyPart.Charset = "utf-8"; 
            //    message.CreateMHTMLBody(strHostName + "Mail/SendMailContract?id=" + id + "&id1=" + id1 + "&_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone);
            //    message.Subject = (id1 == 1 ? "New Contract / Nuevo Contrato" : "Ceased Employee / Empleado Cesado");
            //    message.Send();
            //    return "OK";
            //}
            //catch(Exception e)
            //{
            //    var exc = new EXCEPTION();
            //    exc.DAT_EXCE = DateTime.Now;
            //    exc.MESSAGE = "Error Correo";//(e.Message.ToString());
            //    exc.DES_EXCE = "Send Mail - Attendance Report";
            //    dba.EXCEPTIONs.Add(exc);
            //    dba.SaveChanges();

            //    return "ERROR";
            //}
            return "OK";
        }

        public string Cumple(int ID_PERS_ENTI)
        {
            //Random rnd = new Random();
            //decimal xx = rnd.Next(1, 1000);
            try
            {
                Body body = new Body();
                string body_html = body.TarjetaCumple(ID_PERS_ENTI);

                //message.From = fromEmail;
                ////message.To = "esalazar@electrodata.com.pe;";
                //  message.To = "electrodata_peru@electrodata.com.pe";//"electrodata_peru@electrodata.com.pe";
                // //message.CC = mail_coor;jvillafana@electrodata.com.pe;
                ////message.HTMLBody("");
                // message.HTMLBody = body_html;
                //message.Subject = "Feliz Cumpleaños";
                //message.Send();

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                //message.To.Add("lsempertegui@electrodata.com.pe");
                message.To.Add("electrodata_peru@electrodata.com.pe"); //Descomentar
                message.AlternateViews.Add(htmlView);
                message.Subject = "Feliz Cumpleaños";
                newMail.smtp.Send(message);
                return "OK";
            }
            catch
            {
                return "ERROR";
            }


        }

        public string RecordatorioCumple(int ID_PERS_ENTI)
        {
            string supervisor = null;

            try
            {
                var cont = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                    .Where(x => x.VIG_CONT == true)
                    .Single(x => x.LAS_CONT == true);

                int ID_CHAR = cont.ID_CHAR.Value;
                int ID_CHAR_JEF = dbe.TA_UEN_CARGO(4).Single(x => x.ID_CHAR == ID_CHAR).ID_CHAR_JEF;

                int CONT_JEF = dbe.PERSON_CONTRACT_CHART.Single(x => x.ID_CHAR == ID_CHAR_JEF).ID_PERS_CONT;

                var query = (from pc in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_CONT == CONT_JEF)
                             join pe in dbe.PERSON_ENTITY on pc.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                             select new
                             {
                                 mail = pe.EMA_ELEC
                             }).First();

                supervisor = query.mail;
            }
            catch
            {
            }
            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;

            Random rnd = new Random();
            decimal xx = rnd.Next(1, 1000);
            Body body = new Body();
            string body_html = body.TarjetaCumple(ID_PERS_ENTI);
            // message.From = fromEmail;
            //  message.To = "cgutierrez@electrodata.com.pe; tramos@electrodata.com.pe;zrivadeneyra@electrodata.com.pe";
            //message.BCC = "esalazar@electrodata.com.pe;lsempertegui@electrodata.com.pe";
            //message.To = "esalazar@electrodata.com.pe;";



            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            // message.To.Add("lsempertegui@electrodata.com.pe");

            message.To.Add("cgutierrez@electrodata.com.pe, tramos@electrodata.com.pe,zrivadeneyra@electrodata.com.pe"); //Descomentar 
            message.Bcc.Add("jquisper@electrodata.com.pe");//Descomentar

            if (supervisor != null)
            {
                //message.CC = supervisor
                MailAddress copy = new MailAddress(supervisor);
                message.CC.Add(copy);
            }

            // message.HTMLBody = body_html;
            // message.Subject = "Recordatorio de Cumpleaños (Referencial No Enviar)";
            // message.Send();


            message.AlternateViews.Add(htmlView);
            message.Subject = "Recordatorio de Cumpleaños (Referencial No Enviar)";
            newMail.smtp.Send(message);

            return "OK";
        }




        //public string Notification()
        //{
        //    try
        //    {
        //        var ini = DateTime.Now;
        //        int count = 0;
        //        int count1 = 0, ID_TYPE_NOTI = 0;
        //        int[] array = new int[3];
        //        array[0] = 1;
        //        array[1] = 3;
        //        array[2] = 5;

        //        int[] arraytt = new int[2];
        //        arraytt[0] = 1;
        //        arraytt[1] = 2;

        //        string cod = "", html_body = null;

        //        Body _Body = new Body();

        //        var query = dbc.TICKETs.Where(t => array.Contains((int)t.ID_STAT_END))
        //            .Where(t => t.ID_PRIO != 5)
        //            .Where(t => arraytt.Contains((int)t.ID_TYPE_TICK))
        //            .Where(t => t.ID_ACCO != null)
        //            .Where(t => t.ID_ACCO > 0).ToList();

        //        foreach (TICKET tickets in query)
        //        {
        //            html_body = null;
        //            string cc = "";
        //            int horas = 0;
        //            int minutes = 0;
        //            int ID_TICK = Convert.ToInt32(tickets.ID_TICK);
        //            var ticket = (from t in dbc.TICKETs
        //                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
        //                          where t.ID_TICK == ID_TICK
        //                          select new
        //                          {
        //                              p.HOU_PRIO,
        //                              t.COD_TICK,
        //                              t.FEC_TICK,
        //                              t.MINUTES,
        //                              t.TYPE_TICKET,
        //                              t.ID_PERS_ENTI_ASSI,
        //                              t.ID_ACCO,
        //                              FEC_INI_TICK = t.FEC_INI_TICK == null ? t.FEC_TICK : t.FEC_INI_TICK
        //                          }).First();

        //            int ID_ACCO = Convert.ToInt32(ticket.ID_ACCO);

        //            string mailTo = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).EMA_ELEC;

        //            if (mailTo == null)
        //            {
        //                mailTo = "itms@electrodata.com.pe";
        //            }

        //            var deta_tick = dbc.DETAILS_TICKET
        //                .Where(x => x.ID_TICK == ID_TICK)
        //                .Where(x => x.ID_STAT != null)
        //                .OrderByDescending(x => x.ID_DETA_TICK);

        //            if (deta_tick.Count() > 0)
        //            {
        //                DateTime fecha_ini_2 = Convert.ToDateTime(deta_tick.First().FEC_SCHE);
        //                if (fecha_ini_2 == DateTime.MinValue)
        //                {
        //                    fecha_ini_2 = Convert.ToDateTime(deta_tick.First().CREATE_DETA_INCI);
        //                    horas = Convert.ToInt32(DateTime.Now.Subtract(fecha_ini_2).TotalHours);
        //                    minutes = Convert.ToInt32(ticket.MINUTES) + Convert.ToInt32(DateTime.Now.Subtract(fecha_ini_2).TotalMinutes);
        //                }
        //                else
        //                {
        //                    //Aún no cumple Scheduled
        //                    if (DateTime.Now <= fecha_ini_2)
        //                    {
        //                        horas = Convert.ToInt32(Convert.ToDouble(ticket.MINUTES) / 60.00);
        //                        minutes = Convert.ToInt32(ticket.MINUTES);
        //                    }
        //                    else
        //                    {
        //                        horas = Convert.ToInt32((ticket.MINUTES) / 60.00) - Convert.ToInt32(DateTime.Now.Subtract(fecha_ini_2).TotalHours);
        //                        minutes = Convert.ToInt32(ticket.MINUTES) + Convert.ToInt32(DateTime.Now.Subtract(fecha_ini_2).TotalMinutes);
        //                    }

        //                }


        //            }
        //            else
        //            {
        //                horas = Convert.ToInt32(DateTime.Now.Subtract(ticket.FEC_INI_TICK.Value).TotalHours);
        //                minutes = Convert.ToInt32(DateTime.Now.Subtract(ticket.FEC_INI_TICK.Value).TotalMinutes);
        //            }

        //            if (Convert.ToInt32(ticket.HOU_PRIO) - Convert.ToInt32(horas) == ticket.HOU_PRIO / 2)
        //            {
        //                //Mitad del tiempo para vencimiento
        //                ID_TYPE_NOTI = 1;
        //                var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
        //                    .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();
        //                if (index1 == 0)
        //                {
        //                    html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);
        //                }
        //            }

        //            if (ticket.HOU_PRIO * 60 - minutes <= 60 && ticket.HOU_PRIO * 60 - minutes > 15)
        //            {
        //                //Falta 1 HORA
        //                ID_TYPE_NOTI = 2;
        //                var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
        //                    .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();
        //                if (index1 == 0)
        //                {
        //                    html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);

        //                    int ID_COOR = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
        //                    .Single(x => x.ID_ACCO == ID_ACCO).VAL_ACCO_PARA);
        //                    var Coor = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_COOR);
        //                    cc = Coor.EMA_ELEC + ";" + Coor.EMA_PERS;
        //                }
        //            }

        //            if (ticket.HOU_PRIO * 60 - minutes <= 15 && ticket.HOU_PRIO * 60 - minutes >= 0)
        //            {
        //                //Faltan Menos de 15 minutos
        //                ID_TYPE_NOTI = 3;
        //                var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
        //                    .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();
        //                if (index1 == 0)
        //                {
        //                    html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);


        //                    try
        //                    {
        //                        int ID_COOR = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
        //                        .Single(x => x.ID_ACCO == ticket.ID_ACCO).VAL_ACCO_PARA);
        //                        var Coor = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_COOR);
        //                        cc = Coor.EMA_ELEC + ";" + Coor.EMA_PERS;
        //                    }
        //                    catch { }
        //                }


        //            }

        //            if (ticket.HOU_PRIO * 60 - minutes < 0)
        //            {
        //                //Venció SLA
        //                ID_TYPE_NOTI = 4;
        //                var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
        //                    .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();

        //                if (index1 == 0)
        //                {

        //                    try
        //                    {
        //                        int ID_PERS_MANA = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 23).VAL_ACCO_PARA);
        //                        //cc = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_PERS_MANA).EMA_PERS+";jvillafana@electrodata.com.pe";
        //                    }
        //                    catch
        //                    {

        //                    }

        //                    int ID_COOR = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
        //                    .Single(x => x.ID_ACCO == ID_ACCO).VAL_ACCO_PARA);

        //                    var Coor = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_COOR);
        //                    cc = Coor.EMA_ELEC + ";" + Coor.EMA_PERS + ";jvillafana@electrodata.com.pe";


        //                    html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);
        //                }
        //            }

        //            if (html_body != null)
        //            {
        //                message.From = fromEmail;
        //                message.To = mailTo;
        //                message.CC = cc;
        //                message.HTMLBody = html_body;
        //                message.Subject = "Alert SLA " + ticket.COD_TICK;
        //                message.Send();

        //                TICKET_NOTIFICATION ticket_noti = new TICKET_NOTIFICATION();
        //                ticket_noti.ID_TICK = ID_TICK;
        //                ticket_noti.ID_TYPE_NOTI = ID_TYPE_NOTI;
        //                ticket_noti.CREATED = DateTime.Now;
        //                dbc.TICKET_NOTIFICATION.Add(ticket_noti);
        //                dbc.SaveChanges();

        //            }

        //        }
        //        var fin = DateTime.Now;

        //        return "OK";
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //    //return "since: " + Convert.ToString(ini) + " to: " + Convert.ToString(fin) + "/n" + cod + " #####: " + count + "-" + count1 + "-" + query.Count() + "xxx"+html_body;
        //}

        public string NotificationPaymentBallot(int id = 0)
        {
            string html_body = null, mailTo = "";
            Body _Body = new Body();
            try
            {
                var qPersInvo = dbe.PERSON_INVOICE.Find(id);

                var qTIME = dbc.ACCOUNTING_MONTH.Find(qPersInvo.ID_ACCO_MONT);

                var qPerson = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == qPersInvo.ID_PERS_ENTI)
                               join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                               select new
                               {
                                   Person = b.FIR_NAME.Substring(0, 1).ToUpper() + b.FIR_NAME.Substring(1, b.FIR_NAME.Length - 1).ToLower() + " " +
                                            b.LAS_NAME.Substring(0, 1).ToUpper() + b.LAS_NAME.Substring(1, b.LAS_NAME.Length - 1).ToLower(),
                                   EMA_ELEC = (a.EMA_ELEC == null ? "itms@electrodata.com.pe" : a.EMA_ELEC),
                               });
                string month = qTIME.NAM_ACCO_MONT.Substring(0, 1).ToUpper() + qTIME.NAM_ACCO_MONT.Substring(1, qTIME.NAM_ACCO_MONT.Length - 1).ToLower();

                string Subject = "";
                if (qPersInvo.ID_TYPE_PAYM == 1)
                {
                    Subject = "Payment Ballot / Boleta de Pago";
                    html_body = _Body.PaymentBallotsNotification(id, qPerson.First().Person, month, qTIME.ID_ACCO_YEAR.ToString(), qTIME.ACCO_MONT.Value);
                }
                else if (qPersInvo.ID_TYPE_PAYM == 3)
                {
                    Subject = "5th Category Rent Certificate / Certificado de Renta de 5ta Categoría";
                    html_body = _Body.Certicate5thNotification(id, qPerson.First().Person, month, qTIME.ID_ACCO_YEAR.ToString(), qTIME.ACCO_MONT.Value);
                }
                else if (qPersInvo.ID_TYPE_PAYM == 4)
                {
                    Subject = "Certificate Utilities/ Certificado de Utilidades";
                    html_body = _Body.CerticateUtilities(id, qPerson.First().Person, month, qTIME.ID_ACCO_YEAR.ToString(), qTIME.ACCO_MONT.Value);
                }

                if (html_body != null)
                {
                    mailTo = qPerson.First().EMA_ELEC;
                    string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
                    //string arc = qPersInvo.NAM_ATTA + "_" + qPersInvo.ID_INVO + qPersInvo.EXT_ATTA;

                    // message.From = fromEmail;
                    // message.To = mailTo;
                    //message.HTMLBody = html_body;
                    // message.Subject = Subject;
                    ////message.AddAttachment(strHostName + "Adjuntos/Talent/Invoice/" + arc);
                    //message.Send();

                    PluginSmtp newMail = new PluginSmtp();
                    var message = newMail.mailMessage;

                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_body, Encoding.UTF8, MediaTypeNames.Text.Html);
                    //message.To.Add("lsempertegui@electrodata.com.pe");
                    message.To.Add(mailTo); //Descomentar              

                    message.AlternateViews.Add(htmlView);
                    message.Subject = Subject;
                    newMail.smtp.Send(message);
                }

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public string MailIssuesEmployee(int id = 0, string arc = "")
        {
            ////id: ID_PERS_DOCU
            //var pd = dbe.PERSON_DOCUMENTS.Single(x => x.ID_PERS_DOCU == id);
            //string mailTo1 = "";

            //var user = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == pd.ID_PERS_ENTI);
            //mailTo1 = (user == null ? "" : (user.EMA_ELEC == null ? "" : user.EMA_ELEC.Trim())) + ";";

            ////Correo el jefe inmediato superior
            //var bi = dbe.EMAIL_BOSS_INMSUP(pd.ID_PERS_ENTI).ToList();
            //mailTo1 = mailTo1 + (bi == null ? "" : (bi.First().EMA_ELEC == null ? "" : bi.First().EMA_ELEC + ";"));

            ////Resto de correos
            //var query = (from ed in dbe.EMAIL_DESTINATION.Where(x => x.ID_TYPE_EMAI == 2 && x.VIG_EMAI_DEST == true)
            //             join pe in dbe.PERSON_ENTITY on ed.ID_PERS_ENTI equals pe.ID_PERS_ENTI
            //             select new
            //             {
            //                 pe.EMA_ELEC,
            //             });

            //foreach (var a in query.ToList())
            //{
            //    if (mailTo1.Contains(a.EMA_ELEC) == false)
            //    {
            //        mailTo1 = mailTo1 + a.EMA_ELEC + ";";
            //    }
            //}

            ////mailTo1 = "carevalo@electrodata.com.pe";

            //Random rnd = new Random();
            //decimal xx = rnd.Next(1, 1000);

            //string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
            //message.From = fromEmail;
            //message.To = mailTo1;
            //message.BodyPart.Charset = "utf-8";
            //message.CreateMHTMLBody(strHostName + "Mail/SendMailIssues?id=" + id + "&_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone);
            //message.Subject = "Issues Employee / Empleado Memorandum";
            //message.AddAttachment(strHostName + "Adjuntos/Talent/Documents/" + arc);
            //message.Send();

            return "OK";
        }

        public string BeneficiariesEighteenAge()
        {
            //DateTime fec = DateTime.Today;

            //fec = fec.AddYears(-18);

            //var query = (from a in dbe.BENEFICIARies.Where(x => x.DATE_BRITH == fec && x.ATT_EVID_STUD == false)
            //             select a);

            //if (query.Count() > 0)
            //{
            //    string mailTo1 = "";
            //    var qEmail = (from ed in dbe.EMAIL_DESTINATION.Where(x => x.ID_TYPE_EMAI == 3 && x.VIG_EMAI_DEST == true)
            //                  join pe in dbe.PERSON_ENTITY on ed.ID_PERS_ENTI equals pe.ID_PERS_ENTI
            //                  select new
            //                  {
            //                      pe.EMA_ELEC,
            //                  });

            //    foreach (var a in qEmail.ToList())
            //    {
            //        if (mailTo1.Contains(a.EMA_ELEC) == false)
            //        {
            //            mailTo1 = mailTo1 + a.EMA_ELEC + ";";
            //        }
            //    }

            //    foreach (var fil in query)
            //    {
            //        mailTo1 = "carevalo@electrodata.com.pe";

            //        Random rnd = new Random();
            //        decimal xx = rnd.Next(1, 1000);

            //        string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
            //        message.From = fromEmail;
            //        message.To = mailTo1;
            //        message.BodyPart.Charset = "utf-8";
            //        message.CreateMHTMLBody(strHostName + "Mail/SendBeneficiaryAgeLimit?id=" + fil.ID_BENE.ToString() + "&id1=18&_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone);
            //        message.Subject = "Eighteen Years Old / Dieciocho Años de Edad";
            //        message.Send();
            //    }
            //    return "OK";
            //}
            //else
            //{
            //    return "HOY NADIE CUMPLE 18 AÑOS";
            //}

            return "OK";

        }

        public string BeneficiariesTwentyFourAge()
        {
            //DateTime fec = DateTime.Today;

            //fec = fec.AddYears(-24);

            //var query = (from a in dbe.BENEFICIARies.Where(x => x.DATE_BRITH == fec && x.ATT_EVID_STUD == true)
            //             select a);

            //if (query.Count() > 0)
            //{
            //    string mailTo1 = "";
            //    var qEmail = (from ed in dbe.EMAIL_DESTINATION.Where(x => x.ID_TYPE_EMAI == 3 && x.VIG_EMAI_DEST == true)
            //                  join pe in dbe.PERSON_ENTITY on ed.ID_PERS_ENTI equals pe.ID_PERS_ENTI
            //                  select new
            //                  {
            //                      pe.EMA_ELEC,
            //                  });

            //    foreach (var a in qEmail.ToList())
            //    {
            //        if (mailTo1.Contains(a.EMA_ELEC) == false)
            //        {
            //            mailTo1 = mailTo1 + a.EMA_ELEC + ";";
            //        }
            //    }

            //    foreach (var fil in query)
            //    {
            //        mailTo1 = "carevalo@electrodata.com.pe";

            //        Random rnd = new Random();
            //        decimal xx = rnd.Next(1, 1000);

            //        string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
            //        message.From = fromEmail;
            //        message.To = mailTo1;
            //        message.BodyPart.Charset = "utf-8";
            //        message.CreateMHTMLBody(strHostName + "Mail/SendBeneficiaryAgeLimit?id=" + fil.ID_BENE.ToString() + "&id1=24&_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone);
            //        message.Subject = "Twenty Four Years Old / Veinticuatro Años de Edad";
            //        message.Send();
            //    }
            //    return "OK";
            //}
            //else
            //{
            //    return "HOY NADIE CUMPLE 24 AÑOS";
            //}

            return "OK";

        }

        public string Document_Sale(int ID_DOCU_SALE)
        {
            try
            {

                string vendedor = "", cia_end = "", nam_vendor = "";

                var OP = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == ID_DOCU_SALE);

                var OP_V = (from x in OP.ToList()
                            join tds in dbc.TYPE_DOCUMENT_SALE on x.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
                            select new
                            {
                                NUM_DOCU_SALE = x.NUM_DOCU_SALE,
                                NAM_TYPE_DOCU_SALE = tds.NAM_TYPE_DOCU_SALE,
                                ID_ENTI = x.ID_ENTI,
                                ID_COMP_END = x.ID_COMP_END,
                                ID_PERS_ENTI_VEND = x.ID_PERS_ENTI_VEND
                            }).First();

                int ID_ENTI = Convert.ToInt32(OP_V.ID_ENTI.Value);
                string NAM_EMP = null;

                try
                {
                    var vendor = dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == OP_V.ID_PERS_ENTI_VEND.Value).FirstOrDefault();
                    var cia_x = dbe.CLASS_ENTITY.Where(ce => ce.ID_ENTI == OP_V.ID_COMP_END.Value).FirstOrDefault();

                    var vend = dbe.CLASS_ENTITY.Where(ce => ce.ID_ENTI == vendor.ID_ENTI2).FirstOrDefault();

                    vendedor = String.IsNullOrEmpty(vendor.EMA_ELEC) == true ? "" : vendor.EMA_ELEC;
                    cia_end = String.IsNullOrEmpty(cia_x.COM_NAME) == true ? "" : cia_x.COM_NAME;
                    nam_vendor = textInfo.ToTitleCase(vend.FIR_NAME.ToLower() + " " + vend.LAS_NAME.ToLower());

                    NAM_EMP = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ID_ENTI).COM_NAME + " - ";
                }
                catch { NAM_EMP = ""; }

                Body _Body = new Body();
                string body_html = _Body.CreateDocumentSale(ID_DOCU_SALE);

                //message.From = fromEmail;
                //message.To = "servicedesk@electrodata.com.pe";
                //message.CC = "sgomez@electrodata.com.pe";

                //message.HTMLBody = body_html;
                //message.Subject = NAM_EMP + "New " + OP_V.NAM_TYPE_DOCU_SALE + " " + OP_V.NUM_DOCU_SALE + "/" + NAM_EMP + "Nueva " + OP_V.NAM_TYPE_DOCU_SALE + " " + OP_V.NUM_DOCU_SALE;
                //message.Send();

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                message.To.Add("cacosta@electrodata.com.pe," + vendedor);
                //message.To.Add("mquispe@electrodata.com.pe," + vendedor);
                message.CC.Add("eruiz@electrodata.com.pe,brosell@electrodata.com.pe,dnunez@electrodata.com.pe,servicedesk@electrodata.com.pe,pmo@electrodata.com.pe,ocarrillo@electrodata.com.pe");//,jvillafana@electrodata.com.pe"); //Descomentar
                //message.CC.Add("sgomez@electrodata.com.pe,dnunez@electrodata.com.pe,servicedesk@electrodata.com.pe,pmo@electrodata.com.pe,ocarrillo@electrodata.com.pe,lcuenca@electrodata.com.pe");//,jvillafana@electrodata.com.pe"); //Descomentar
                message.Bcc.Add("arivadeneyra@electrodata.com.pe,lmasias@electrodata.com.pe,lsolari@electrodata.com.pe");
                message.AlternateViews.Add(htmlView);
                message.Subject = "Nueva Orden de Pedido (" + OP_V.NAM_TYPE_DOCU_SALE.Trim() + " " + OP_V.NUM_DOCU_SALE.Trim() + ") - " + cia_end + " - " + nam_vendor;
                newMail.smtp.Send(message);

                return "OK";
            }
            catch
            {
                return "ERROR";
            }

        }

        public string UpdateStatusTicketScheduled()
        {
            try
            {
                DateTime ahora = DateTime.Now;
                var query = (from a in dbc.TICKETs.Where(x => x.ID_STAT_END == 5)
                             join b in dbc.DETAILS_TICKET.Where(x => x.ID_STAT == 5).Where(x => x.ID_TYPE_DETA_TICK == 3) on a.ID_TICK equals b.ID_TICK
                             group b by b.ID_TICK into g
                             select new
                             {
                                 ID_TICK = g.Key,
                                 ID_DETA_TICK = g.Max(x => x.ID_DETA_TICK),
                             }).ToList();

                for (int i = 0; i < query.Count; i++)
                {
                    int ID_DETA_TICK = query[i].ID_DETA_TICK;
                    DateTime DAT_SCHE = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK).FEC_SCHE.Value;

                    if (DAT_SCHE <= ahora)
                    {
                        int ID_TICK = Convert.ToInt32(query[i].ID_TICK);

                        var dt = new DETAILS_TICKET();
                        dt.ID_TICK = ID_TICK;
                        dt.ID_STAT = 1;
                        dt.ID_TYPE_DETA_TICK = 3;
                        dt.COM_DETA_TICK = "THE SYSTEM MANAGER UPDATED THE STATUS OF A TICKET ASSIGNED TO THE AGREED SCHEDULE.";
                        dt.UserId = 34;
                        dt.CREATE_DETA_INCI = ahora;

                        dt.SEND_MAIL = false;

                        dbc.DETAILS_TICKET.Add(dt);
                        dbc.SaveChanges();

                        var tick = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                        tick.ID_STAT_END = 1;
                        dbc.TICKETs.Attach(tick);
                        dbc.Entry(tick).State = EntityState.Modified;
                        dbc.SaveChanges();

                        //SendMail mail = new SendMail();
                        //string HB = mail.UpdateStatus(dt.ID_DETA_TICK);
                    }
                }

                //Pasando aquellos tickets que se crearon en estado scheduled y no presentan detalle en schedule
                try
                {
                    var qSche = (from a in dbc.TICKETs.Where(x => x.ID_STAT_END == 5)
                                 join b in dbc.DETAILS_TICKET.Where(x => x.ID_STAT == 5) on a.ID_TICK equals b.ID_TICK into lb
                                 from xb in lb.DefaultIfEmpty()
                                 select new
                                 {
                                     a.ID_TICK,
                                     a.FEC_INI_TICK,
                                     ID_DETA_TICK = (xb == null ? 0 : xb.ID_DETA_TICK),
                                 }).Where(x => x.ID_DETA_TICK == 0).ToList();

                    for (int i = 0; i < qSche.Count; i++)
                    {
                        DateTime FEC_INI_TICK = qSche[i].FEC_INI_TICK.Value;
                        if (FEC_INI_TICK <= ahora)
                        {
                            int ID_TICK = Convert.ToInt32(qSche[i].ID_TICK);

                            var dt = new DETAILS_TICKET();
                            dt.ID_TICK = ID_TICK;
                            dt.ID_STAT = 1;
                            dt.ID_TYPE_DETA_TICK = 3;
                            dt.COM_DETA_TICK = "THE SYSTEM MANAGER UPDATED THE STATUS OF A TICKET ASSIGNED TO THE AGREED SCHEDULE.";
                            dt.UserId = 34;
                            dt.CREATE_DETA_INCI = ahora;

                            dt.SEND_MAIL = false;

                            dbc.DETAILS_TICKET.Add(dt);
                            dbc.SaveChanges();

                            var tick = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                            tick.ID_STAT_END = 1;
                            dbc.TICKETs.Attach(tick);
                            dbc.Entry(tick).State = EntityState.Modified;
                            dbc.SaveChanges();

                            //SendMail mail = new SendMail();
                            //string HB = mail.UpdateStatus(dt.ID_DETA_TICK);
                        }
                    }

                }
                catch (Exception e)
                {
                    var exc = new EXCEPTION();
                    exc.DAT_EXCE = DateTime.Now;
                    exc.MESSAGE = e.InnerException.Message;
                    exc.DES_EXCE = "Send Mail - UpdateStatusTicketScheduled: Para aquellos tickets que no presentan detalle";
                    dba.EXCEPTIONs.Add(exc);
                    dba.SaveChanges();
                    return "ERROR";
                }
                return "OK";
            }
            catch (Exception e)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = e.InnerException.Message;
                exc.DES_EXCE = "Send Mail - UpdateStatusTicketScheduled: Para aquellos tickets que presentan detalle";
                dba.EXCEPTIONs.Add(exc);
                dba.SaveChanges();
                return "ERROR";
            }
        }

        public string NotificationPettyCashPayment(int id = 0)
        {
            //id: ID_REQU_EXPE
            var qPerson = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9)
                           select new
                           {
                               a.ID_PERS_ENTI,
                               a.EMA_ELEC,
                           });

            var query = (from a in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).ToList()
                         join b in qPerson on a.ID_PERS_ENTI_REQU equals b.ID_PERS_ENTI
                         join c in qPerson on a.ID_PERS_ENTI_APPR equals c.ID_PERS_ENTI
                         join d in qPerson on a.ID_PERS_ENTI_ASSI equals d.ID_PERS_ENTI
                         select new
                         {
                             EMA_REQU = b.EMA_ELEC,
                             EMA_APPR = c.EMA_ELEC,
                             EMA_ASSI = d.EMA_ELEC,
                         });

            string mailTo1 = query.First().EMA_REQU + "," + query.First().EMA_APPR + "," + query.First().EMA_ASSI;

            Random rnd = new Random();
            decimal xx = rnd.Next(1, 1000);

            //mailTo1 = "carevalo@electrodata.com.pe";

            string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
            //message.From = fromEmail;
            //message.To = mailTo1;
            //message.BodyPart.Charset = "utf-8";
            //message.CreateMHTMLBody(strHostName + "Mail/SendMailPettyCashPayment?id=" + id + "&_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone);
            //message.Subject = "Petty Cash Payment / Pago de Caja Chica";
            //message.Send();
            return "OK";
        }

        public string NotificationPettyCashAccountability(int id = 0)
        {
            //id: ID_REQU_EXPE
            var qPerson = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9)
                           select new
                           {
                               a.ID_PERS_ENTI,
                               a.EMA_ELEC,
                           });

            var query = (from a in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).ToList()
                         join b in qPerson on a.ID_PERS_ENTI_REQU equals b.ID_PERS_ENTI
                         join c in qPerson on a.ID_PERS_ENTI_APPR equals c.ID_PERS_ENTI
                         join d in qPerson on a.ID_PERS_ENTI_ASSI equals d.ID_PERS_ENTI
                         select new
                         {
                             EMA_REQU = b.EMA_ELEC,
                             EMA_APPR = c.EMA_ELEC,
                             EMA_ASSI = d.EMA_ELEC,
                             a.ID_TYPE_DELI_SUST,
                         });

            string mailTo1 = query.First().EMA_REQU + "," + query.First().EMA_APPR + "," + query.First().EMA_ASSI;
            string s_tipo = "", s_type = "";
            if (query.First().ID_TYPE_DELI_SUST.Value == 1)
            {
                s_tipo = "Caja Chica";
                s_type = "Petty Cash";
            }
            else
            {
                s_tipo = "Viáticos";
                s_type = "Viatical";
            }

            Random rnd = new Random();
            decimal xx = rnd.Next(1, 1000);

            //mailTo1 = "carevalo@electrodata.com.pe";

            string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();
            //message.From = fromEmail;
            //message.To = mailTo1;
            //message.BodyPart.Charset = "utf-8";
            //message.CreateMHTMLBody(strHostName + "Mail/SendMailPettyCashAccountability?id=" + id + "&_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone);
            //message.Subject = s_type + " Accountability / Rendición de " + s_tipo;
            //message.Send();

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(strHostName + "Mail/SendMailPettyCashAccountability?id=" + id + "&_=" + Convert.ToString(xx), Encoding.UTF8, MediaTypeNames.Text.Html);
            // message.To.Add("lsempertegui@electrodata.com.pe");
            message.To.Add(mailTo1); //Descomentar     
            message.AlternateViews.Add(htmlView);
            message.Subject = s_type + " Accountability / Rendición de " + s_tipo;
            newMail.smtp.Send(message);

            return "OK";
        }

        public string DocumentoObservado(DOCUMENT_EXPENSE de)
        {
            try
            {
                int id_detail_mobility = 0;
                int id_reque_expe = 0;
                int id_type_docu = 0;
               
                    id_reque_expe = Convert.ToInt32(dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).Single().ID_REQU_EXPE);
                    id_type_docu = Convert.ToInt32(de.ID_TYPE_DOCU_EXPE);

                REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Where(x=> x.ID_REQU_EXPE == id_reque_expe).FirstOrDefault();

                var query = (from x in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == re.ID_REQU_EXPE)
                             join sre in dbc.STATUS_REQUEST_EXPENSE on re.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                             join tds in dbc.TYPE_DELI_SUST on x.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                             select new
                             {
                                 NAM_STAT_REQU_EXPE = sre.NAM_STAT_REQU_EXPE,
                                 NAM_STAT_REQU_EXPE_SPAN = sre.NAM_STAT_REQU_EXPE_SPAN,
                                 NAM_TYPE_DELI_SUST = tds.NAM_TYPE_DELI_SUST,
                                 COD_REQU_EXPE = x.COD_REQU_EXPE,
                                 NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN
                             }).First();

                PERSON_ENTITY requester = null, approval = null;
                string msgTo = null, msgCC = null;

                requester = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_REQU.Value);
                approval = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_APPR.Value);

                var person = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == requester.ID_ENTI2);

                msgTo = requester.EMA_ELEC;
                msgCC = approval.EMA_ELEC;

                //Adjuntamos el documento

                string id = "";

                if (id_type_docu == 4)
                {
                    id_detail_mobility = dbc.DETAIL_MOBILITY.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).FirstOrDefault().ID_DETA_MOVI;
                    id = dbc.ATTACHEDs.Where(x => x.ID_DETA_MOVI == id_detail_mobility).Single().NAM_ATTA + "_" + dbc.ATTACHEDs.Where(x => x.ID_DETA_MOVI == id_detail_mobility).Single().ID_ATTA + dbc.ATTACHEDs.Where(x => x.ID_DETA_MOVI == id_detail_mobility).Single().EXT_ATTA;
                }
                else
                {
                    id = dbc.ATTACHEDs.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).Single().NAM_ATTA + "_" + dbc.ATTACHEDs.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).Single().ID_ATTA + dbc.ATTACHEDs.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).Single().EXT_ATTA;
                }

                string filePath = "";

                FileStream fileStream = null;
                if (re.ID_TYPE_DELI_SUST == 1)
                {
                    filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Adjuntos/CajaChica/" + id);
                    fileStream = System.IO.File.OpenRead(filePath);
                }
                else if (re.ID_TYPE_DELI_SUST == 2)
                {
                    filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Adjuntos/Viaticos/" + id);
                    fileStream = System.IO.File.OpenRead(filePath);
                }
                else
                {
                    filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Adjuntos/Reembolso/" + id);
                    fileStream = System.IO.File.OpenRead(filePath);
                }

                MemoryStream storeStream = new MemoryStream();

                storeStream.SetLength(fileStream.Length);
                fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                byte[] byteArray = storeStream.ToArray();

                Attachment pdfAttachment = new Attachment(new MemoryStream(byteArray), "DocumentoObservado.pdf", "application/pdf");

                Body body = new Body();
                string body_html = body.CreaCuerpoObservacionDocumento(re,de);
               
                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);


                message.To.Add(msgTo); //Descomentar              
                message.CC.Add(msgCC);//Descomentar
                message.AlternateViews.Add(htmlView);
                message.Subject = "¡ALERTA! Comprobante de " + query.NAM_TYPE_DELI_SUST + " con observaciones" ;
                message.Attachments.Add(pdfAttachment);

                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        public string DocumentoActualizado(DOCUMENT_EXPENSE de)
        {
            try
            {
                int id_reque_expe = 0;
                int id_type_docu = 0;

                id_reque_expe = Convert.ToInt32(dbc.DOCUMENT_EXPENSE.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).Single().ID_REQU_EXPE);
                id_type_docu = Convert.ToInt32(de.ID_TYPE_DOCU_EXPE);

                REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id_reque_expe).FirstOrDefault();

                var query = (from x in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == re.ID_REQU_EXPE)
                             join sre in dbc.STATUS_REQUEST_EXPENSE on re.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                             join tds in dbc.TYPE_DELI_SUST on x.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                             select new
                             {
                                 NAM_STAT_REQU_EXPE = sre.NAM_STAT_REQU_EXPE,
                                 NAM_STAT_REQU_EXPE_SPAN = sre.NAM_STAT_REQU_EXPE_SPAN,
                                 NAM_TYPE_DELI_SUST = tds.NAM_TYPE_DELI_SUST,
                                 COD_REQU_EXPE = x.COD_REQU_EXPE,
                                 NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN
                             }).First();

                PERSON_ENTITY requester = null, approval = null, paga = null;
                string msgTo = null, msgCC = null,msgApruebaConta = null;

                requester = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_REQU.Value);
                approval = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_APPR.Value);

                var person = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == requester.ID_ENTI2);

                try
                {

                    if (re.ID_DELI_SUST != null)
                    {
                        var ds = dbc.DELIVERY_SUSTAIN.First(x => x.ID_DELI_SUST == re.ID_DELI_SUST.Value);
                        paga = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == ds.ID_PERS_ENTI_ASSI.Value);
                    }

                    var resultConta = (from ce in dbe.CLASS_ENTITY
                                       join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                                       join pc in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true) on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI into pcGroup
                                       from pc in pcGroup.DefaultIfEmpty()
                                       join c in dbe.CHARTs on pc.ID_CHAR equals c.ID_CHAR into cGroup
                                       from c in cGroup.DefaultIfEmpty()
                                       join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR into ncGroup
                                       from nc in ncGroup.DefaultIfEmpty()
                                       where nc.NAM_CHAR.Contains("contab")
                                               && pe.VIG_PERS_ENTI == true
                                               && ce.VIG_ENTI == true
                                               && c.ID_ACCO == 4
                                       select new { ID_PERS_ENTI = pe.ID_PERS_ENTI });

                    foreach (var result in resultConta)
                    {
                        var personC = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == result.ID_PERS_ENTI);
                        msgApruebaConta += (String.IsNullOrEmpty(personC.EMA_ELEC) ? "" : personC.EMA_ELEC) + ",";
                    }

                    // Eliminar la última coma si hay al menos un correo electrónico
                    if (!String.IsNullOrEmpty(msgApruebaConta))
                    {
                        msgApruebaConta = msgApruebaConta.TrimEnd(',');
                    }

                }
                catch
                {

                }


                msgTo = msgApruebaConta;
                msgCC = requester.EMA_ELEC + "," + approval.EMA_ELEC;

                //Adjuntamos el documento

                string id = "";

                id = dbc.ATTACHEDs.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).Single().NAM_ATTA + "_" + dbc.ATTACHEDs.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).Single().ID_ATTA + dbc.ATTACHEDs.Where(x => x.ID_DOCU_EXPE == de.ID_DOCU_EXPE).Single().EXT_ATTA;
           

                string filePath = "";

                FileStream fileStream = null;
                if (re.ID_TYPE_DELI_SUST == 1)
                {
                    filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Adjuntos/CajaChica/" + id);
                    fileStream = System.IO.File.OpenRead(filePath);
                }
                else if (re.ID_TYPE_DELI_SUST == 2)
                {
                    filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Adjuntos/Viaticos/" + id);
                    fileStream = System.IO.File.OpenRead(filePath);
                }
                else
                {
                    filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Adjuntos/Reembolso/" + id);
                    fileStream = System.IO.File.OpenRead(filePath);
                }

                MemoryStream storeStream = new MemoryStream();

                storeStream.SetLength(fileStream.Length);
                fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                byte[] byteArray = storeStream.ToArray();

                Attachment pdfAttachment = new Attachment(new MemoryStream(byteArray), "DocumentoObservado.pdf", "application/pdf");

                Body body = new Body();
                string body_html = body.CreaCuerpoDocumentoActualizado(re);

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);


                message.To.Add(msgTo); //Descomentar              
                message.CC.Add(msgCC);//Descomentar
                message.AlternateViews.Add(htmlView);
                message.Subject = "Comprobante de " + query.NAM_TYPE_DELI_SUST + " subsanado por usuario";
                message.Attachments.Add(pdfAttachment);

                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        public string RequestAccountability(REQUEST_EXPENSE re)
        {
            try
            {
                var query = (from x in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == re.ID_REQU_EXPE)
                             join sre in dbc.STATUS_REQUEST_EXPENSE on re.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                             join tds in dbc.TYPE_DELI_SUST on x.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                             select new
                             {
                                 NAM_STAT_REQU_EXPE = sre.NAM_STAT_REQU_EXPE,
                                 NAM_STAT_REQU_EXPE_SPAN = sre.NAM_STAT_REQU_EXPE_SPAN,
                                 NAM_TYPE_DELI_SUST = tds.NAM_TYPE_DELI_SUST,
                                 COD_REQU_EXPE = x.COD_REQU_EXPE,
                                 NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN
                             }).First();

                PERSON_ENTITY requester = null, approval = null, paga = null;
                string msgTo = null, msgCC = null, msgTo2 = "";

                requester = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_REQU.Value);
                approval = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_APPR.Value);

                var person = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == requester.ID_ENTI2);

                try
                {

                    if (re.ID_DELI_SUST != null)
                    {
                        var ds = dbc.DELIVERY_SUSTAIN.First(x => x.ID_DELI_SUST == re.ID_DELI_SUST.Value);
                        paga = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == ds.ID_PERS_ENTI_ASSI.Value);

                    }
                }
                catch
                {

                }

                try
                {
                    if (re.ID_PERS_ENTI_APPR_VIAT != null)
                    {
                        var personV = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_APPR_VIAT.Value);
                        msgTo2 = personV.EMA_ELEC;
                    }
                }
                catch
                {

                }

                //caja chica
                //if(re.ID_TYPE_DELI_SUST == 1){
                switch (re.ID_STAT_REQU_EXPE)
                {
                    case 1:
                        msgTo = approval.EMA_ELEC;
                        msgCC = requester.EMA_ELEC;//+";mquispe@electrodata.com.pe";
                        break;
                    case 2:
                        msgTo = requester.EMA_ELEC;
                        //msgCC = approval.EMA_ELEC + ",cacosta@electrodata.com.pe";
                        if (approval.ID_PERS_ENTI != 233)
                        {
                            msgCC = approval.EMA_ELEC + ",cacosta@electrodata.com.pe";
                        }
                        else
                        {
                            msgCC = approval.EMA_ELEC + ",cacosta@electrodata.com.pe,rmeza@electrodata.com.pe";
                        }

                        if (re.ID_TYPE_DELI_SUST == 2)
                        {
                            msgTo = msgTo + ",mromero@electrodata.com.pe,lmasias@electrodata.com.pe,bacevedo@electrodata.com.pe";
                        }

                        msgTo = msgTo + "," + msgTo2;

                        break;
                    case 3:
                        msgTo = requester.EMA_ELEC;
                        msgCC = approval.EMA_ELEC + ",cacosta@electrodata.com.pe";
                        break;
                    case 4:
                        msgTo = approval.EMA_ELEC;
                        msgCC = requester.EMA_ELEC + ",cacosta@electrodata.com.pe";
                        break;
                    case 5:
                        msgTo = requester.EMA_ELEC;
                        msgCC = paga.EMA_ELEC;
                        break;
                    case 6:
                        msgTo = paga.EMA_ELEC;
                        msgCC = requester.EMA_ELEC;
                        break;
                    case 7:
                        //msgTo = requester.EMA_ELEC;

                        msgTo = requester.EMA_ELEC;
                        msgCC = approval.EMA_ELEC;
                        break;
                    case 8:
                        msgTo = requester.EMA_ELEC;
                        msgCC = approval.EMA_ELEC;
                        break;
                    default:
                        msgTo = "itms@electrodata.com.pe";
                        break;
                }

                Body body = new Body();
                string body_html = body.PettyCash(re);
                //message.From = fromEmail;
                //message.To = msgTo;
                //message.CC = msgCC;
                //message.HTMLBody = body_html;
                //message.Subject = query.NAM_TYPE_DELI_SUST + " ("+ textInfo.ToTitleCase(person.FIR_NAME.ToLower()+" "+person.LAS_NAME.ToLower())+") - "+(re.CURRENCY == "MN" ? "PEN " : "USD ") + String.Format("{0:0.00}",re.AMOUNT)+" "+ query.NAM_STAT_REQU_EXPE 
                //    + " / " + query.NAM_TYPE_DELI_SUST_SPAN + " (" + textInfo.ToTitleCase(person.FIR_NAME.ToLower() + " " + person.LAS_NAME.ToLower()) + ") - " + (re.CURRENCY == "MN" ? "PEN " : "USD ") + String.Format("{0:0.00}", re.AMOUNT) +" "+ query.NAM_STAT_REQU_EXPE_SPAN ;
                //message.Send();

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                //message.To.Add("lsempertegui@electrodata.com.pe");

                message.To.Add(msgTo); //Descomentar              
                message.CC.Add(msgCC);//Descomentar
                message.AlternateViews.Add(htmlView);
                message.Subject = query.NAM_TYPE_DELI_SUST;
                //+ " (" + person.FIR_NAME.ToLower() + " " + person.LAS_NAME.ToLower() + ") - " + (re.CURRENCY == "MN" ? "PEN " : "USD ") + String.Format("{0:0.00}", re.AMOUNT) + " " + query.NAM_STAT_REQU_EXPE_SPAN
                //    + " / " + query.NAM_TYPE_DELI_SUST_SPAN + " (" + textInfo.ToTitleCase(person.FIR_NAME.ToLower() + " " + person.LAS_NAME.ToLower()) + ") - " + (re.CURRENCY == "MN" ? "PEN " : "USD ") + String.Format("{0:0.00}", re.AMOUNT) + " " + query.NAM_STAT_REQU_EXPE_SPAN;
                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        public string CumpleStaffMes()
        {
            DateTime fecha = DateTime.Now;
            Body body = new Body();
            string body_html = body.TarjetaCumpleStaffMes(fecha.Day);
            //message.From = fromEmail;
            //message.To = "electrodata_peru@electrodata.com.pe";//"jvillafana@electrodata.com.pe, esalazar@electrodata.com.pe"; //"electrodata_peru@electrodata.com.pe";
            //// message.CC ="lsempertegui@electrodata.com.pe";
            //message.HTMLBody = body_html;
            //message.Subject = "Birthday Month (" + textInfo.ToTitleCase(dtinfo_en.GetMonthName(fecha.Month)) + ") - Staff Electrodata / Cumpleaños del Mes (" + textInfo.ToTitleCase(dtinfo_es.GetMonthName(fecha.Month)) + ") - Personal Electrodata";
            //message.Send();

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            // message.To.Add("lsempertegui@electrodata.com.pe");

            message.To.Add("electrodata_peru@electrodata.com.pe"); //Descomentar              
            message.Bcc.Add("jquisper@electrodata.com.pe");//,jvillafana@electrodata.com.pe"); //Descomentar           
            message.AlternateViews.Add(htmlView);
            message.Subject = "Birthday Month (" + textInfo.ToTitleCase(dtinfo_en.GetMonthName(fecha.Month)) + ") - Staff Electrodata / Cumpleaños del Mes (" + textInfo.ToTitleCase(dtinfo_es.GetMonthName(fecha.Month)) + ") - Personal Electrodata";
            newMail.smtp.Send(message);

            return "OK";
        }

        public string CumpleStaffMesRemember()
        {
            DateTime fecha = DateTime.Now.AddMonths(1);

            Body body = new Body();
            string body_html = body.TarjetaCumpleStaffMes(fecha.Day);

            //message.From = fromEmail;
            //message.To = "zrivadeneyra@electrodata.com.pe;tramos@electrodata.com.pe;cgutierrez@electrodata.com.pe";//"jvillafana@electrodata.com.pe, esalazar@electrodata.com.pe"; //"electrodata_peru@electrodata.com.pe";
            //message.BCC = "lsempertegui@electrodata.com.pe;esalazar@electrodata.com.pe;jvillafana@electrodata.com.pe";
            //message.HTMLBody = body_html;
            //message.Subject = "Remmeber Birthday Month for (" + textInfo.ToTitleCase(dtinfo_en.GetMonthName(fecha.Month)) + ") - Staff Electrodata / Recordatorio Cumpleaños Mes de (" + textInfo.ToTitleCase(dtinfo_es.GetMonthName(fecha.Month)) + ") - Personal Electrodata";
            //message.Send();

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            // message.To.Add("lsempertegui@electrodata.com.pe");

            message.To.Add("zrivadeneyra@electrodata.com.pe,tramos@electrodata.com.pe,cgutierrez@electrodata.com.pe"); //Descomentar              
            message.Bcc.Add("jquisper@electrodata.com.pe");//,jvillafana@electrodata.com.pe"); //Descomentar           
            message.AlternateViews.Add(htmlView);
            message.Subject = "Remmeber Birthday Month for (" + textInfo.ToTitleCase(dtinfo_en.GetMonthName(fecha.Month)) + ") - Staff Electrodata / Recordatorio Cumpleaños Mes de (" + textInfo.ToTitleCase(dtinfo_es.GetMonthName(fecha.Month)) + ") - Personal Electrodata";
            newMail.smtp.Send(message);


            return "OK";
        }

        public string NewDocumentControl(string mail_to, DOCUMENT_CONTROL dc)
        {
            try
            {
                Body body = new Body();
                string body_html = body.NewDocumentControl(dc);
                //  message.From = fromEmail;
                //  message.To = mail_to;
                //message.Subject = "New Reception Document / Nuevo Documento Recepcionado";
                //message.HTMLBody = body_html;
                //message.Send();

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                //message.To.Add("lsempertegui@electrodata.com.pe");

                message.To.Add(mail_to); //Descomentar             

                message.AlternateViews.Add(htmlView);
                message.Subject = "New Reception Document / Nuevo Documento Recepcionado";
                newMail.smtp.Send(message);

                return "OK";
            }
            catch
            {
                return "ERROR";
            }
        }

        public string EnviarCodigoReestablecimiento(string mail_to, string nomuser, string cuent, string token)
        {
            try
            {
                Body body = new Body();
                string body_html = body.CodigoRestablecimientoITMS(nomuser, cuent, token);

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                message.To.Add(mail_to); //Descomentar             

                message.AlternateViews.Add(htmlView);
                message.Subject = "Password restoration / Restauracion de contraseña";
                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }
        public string EnviaRestaurarContrasena(string mail_to, string nomuser, string cuent, string pass)
        {
            try
            {
                Body body = new Body();
                string body_html = body.RestauraUsuarioITMS(nomuser, cuent, pass);
                //  message.From = fromEmail;
                //  message.To = mail_to;
                //message.Subject = "New Reception Document / Nuevo Documento Recepcionado";
                //message.HTMLBody = body_html;
                //message.Send();

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                //message.To.Add("lsempertegui@electrodata.com.pe");

                message.To.Add(mail_to); //Descomentar             

                message.AlternateViews.Add(htmlView);
                message.Subject = "Password restoration / Restauracion de contraseña";
                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        public string Test()
        {
            try
            {

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;

                string body_html = "Test Correo Google";

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                message.To.Add("lsempertegui@electrodata.com.pe");
                message.AlternateViews.Add(htmlView);
                message.Subject = "Test: Mensaje desde Gmail(Subject)";
                newMail.smtp.Send(message);

                return "OK";
            }
            catch
            {
                return "ERROR";
            }
        }
        public bool CreacionUsuarioEData(string mail_to, string nombreUsuario, string cuenta, string password, string email_personal, string Cargo, string LugarTrabajo)
        {
            // DateTime fecha = DateTime.Now.AddMonths(1);

            Body body = new Body();
            string body_html = body.CuentaUsuarioITMS(nombreUsuario, cuenta, password, email_personal, Cargo, LugarTrabajo);

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            message.To.Add(mail_to);
            message.Bcc.Add(ConfigurationManager.AppSettings["MailAdmin"].ToString());
            //message.Bcc.Add("mcatacora@electrodata.com.pe");
            message.AlternateViews.Add(htmlView);
            message.Subject = "Creación de Usuario ITMS -" + nombreUsuario;
            newMail.smtp.Send(message);
            return true;
        }
        public bool CreacionUsuarioExterno(string nombreUsuario, string usuario, string password, string email_personal, string cuenta)
        {

            Body body = new Body();
            string body_html = body.CuentaUsuarioExternoITMS(nombreUsuario, usuario, password, email_personal, cuenta);

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            message.To.Add(email_personal);
            message.To.Add("sistemas@electrodata.com.pe");
            message.Bcc.Add("servicedesk@electrodata.com.pe");
            message.AlternateViews.Add(htmlView);
            message.Subject = "Creación de Usuario externo ITMS -" + nombreUsuario;
            newMail.smtp.Send(message);
            return true;
        }

        public bool SendMailNewActivity(string sendMail, string asunto, int UserId, ACTIVITY_LOG act_log)
        {
            var query = (from ce in dbe.CLASS_ENTITY.Where(x => x.UserId == UserId)
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         select new
                         {
                             NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                             //     (ce.MOT_NAME == null ? "" : " " + ce.MOT_NAME.ToUpper().Substring(0, 1) + "."), 
                             EMAIL_PERSON = pe.EMA_ELEC
                         }).Single();//.NAM_PERSON;
            var emailUsuario = query.EMAIL_PERSON;
            var nombreUsuario = query.NAM_PERSON;

            var nombreEmpresa = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == act_log.ID_CLIE).COM_NAME;

            Body body = new Body();
            string body_html = body.CreateDetailsActivity(UserId, act_log, nombreUsuario);
            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(sendMail))
            {
                message.To.Add(sendMail);
                if (!string.IsNullOrEmpty(emailUsuario))
                    message.CC.Add(emailUsuario);
            }
            else
            {
                if (!string.IsNullOrEmpty(emailUsuario))
                {
                    message.To.Add(emailUsuario);
                }
                else
                {
                    return false;
                }
            }
            //message.Bcc.Add("fsandoval@electrodata.com.pe");
            message.AlternateViews.Add(htmlView);
            message.Subject = "ACTIVIDAD: " + nombreUsuario + " - " + DateTime.Now.ToShortDateString() + " - " + asunto.ToUpper();
            newMail.smtp.Send(message);
            return true;
        }
        //public bool sendMailChangeRequest(int ID_PERS_ENTI, CHANGE_REQUEST changeRequest)
        //{
        //    var queryUsuario = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
        //                           join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
        //                           select new
        //                           {
        //                               NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
        //                               EMAIL_PERSON = pe.EMA_ELEC
        //                           }).Single();
        //    var query = (from cr in dbc.CHANGE_REQUEST.Where(x=>x.id==changeRequest.id)
        //                join ca in dbc.CHANGE_APPROVED on cr.id equals ca.idChangeRequest
        //                     select new{
        //                         id = ca.id,
        //                         IdCambio = cr.id,
        //                         createDate = ca.createDate,
        //                         ca.approver,
        //                         estado=ca.idTypeRequest
        //                     }).Single();

        //    var queryAprobador = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == query.approver)
        //                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
        //                          select new
        //                          {
        //                              NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
        //                              EMAIL_PERSON = pe.EMA_ELEC
        //                          }).Single();

        //    var color = dbc.CHANGE_TYPE_REQUEST.Where(x => x.id == query.estado).FirstOrDefault().Color;

        //    #region detalle del mensaje
        //    string nombreUsuario = queryUsuario.NAM_PERSON;            
        //    string detailChange = changeRequest.question2;
        //    string colorSolicitudCambio = color;
        //    string msjRegistroSolicitud = "Se registró una nueva solicitud.";
        //    string fechaRegistro = string.Format("{0:MM/dd/yy H:mm:ss}", query.createDate);
        //    string status = "Nueva Solicitud";
        //    string aprobador = queryAprobador.NAM_PERSON;
        //    string IdCambio = Convert.ToString(query.IdCambio);
        //    string CorreoJefe = queryAprobador.EMAIL_PERSON;
        //    #endregion
        //    //Envio de Correo 
        //    Body body = new Body();
        //    string body_html = body.CorreoChangeRequest(nombreUsuario, aprobador, detailChange, colorSolicitudCambio, msjRegistroSolicitud, status, fechaRegistro, IdCambio);
        //    PluginSmtp newMail = new PluginSmtp();
        //    var message = newMail.mailMessage;
        //    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
        //    if (!string.IsNullOrEmpty(CorreoJefe))
        //    {
        //        message.To.Add(queryAprobador.EMAIL_PERSON);
        //        message.CC.Add(CorreoJefe);
        //    }

        //    message.AlternateViews.Add(htmlView);
        //    message.Subject = "SOLICITUD DE CAMBIO: " + nombreUsuario + " - " + DateTime.Now.ToShortDateString();
        //    newMail.smtp.Send(message);
        //    return true;
        //}

        public bool sendMailGestionCambios(int ID_PERS_ENTI, CHANGE_APPROVED ca, string flag)
        {
            var queryUsuario = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                    EMAIL_PERSON = pe.EMA_ELEC
                                }).Single();
            var queryAprobador = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ca.approver)
                                  join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                  select new
                                  {
                                      NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                      EMAIL_PERSON = pe.EMA_ELEC
                                  }).Single();

            var queryRequest = (from apr in dbc.CHANGE_APPROVED.Where(x => x.id == ca.id)
                                join ce in dbc.CHANGE_REQUEST on apr.idChangeRequest equals ce.id
                                select new
                                {
                                    apr.id,
                                    IdCambio = ce.id,
                                    ce.question2,
                                    estado = apr.idTypeRequest
                                }).Single();

            var color = dbc.CHANGE_TYPE_REQUEST.Where(x => x.id == queryRequest.estado).FirstOrDefault().Color;

            string mailUsuario = queryUsuario.EMAIL_PERSON;
            string mailJefe = queryAprobador.EMAIL_PERSON;
            string nombreUsuario = queryUsuario.NAM_PERSON;
            string detailChange = queryRequest.question2;
            string colorSolicitudCambio = color;
            string msjSolicitud = "";
            string status = "";
            string asunto = "";
            string fechaRegistro = "";
            int idCuenta = 0;
            if (flag == "S")
            {
                msjSolicitud = "Se registró una nueva solicitud.";
                status = "Nueva Solicitud";
                asunto = "SOLICITUD DE CAMBIO: " + nombreUsuario + " - " + DateTime.Now.ToShortDateString();
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.createDate);
            }

            else if (flag == "SM")
            {
                msjSolicitud = "Se registró una nueva solicitud.";
                status = "Nueva Solicitud";
                asunto = "SOLICITUD DE CAMBIO: " + nombreUsuario + " - " + DateTime.Now.ToShortDateString();
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.createDate);
                idCuenta = 56;
            }
            else if (flag == "A")
            {
                msjSolicitud = "Su Solicitud de Cambio ha sido Aprobada. Por favor contactese con su jefe inmediato para cualquier consulta.";
                status = "Solicitud Aprobada";
                asunto = "SOLICITUD DE CAMBIO APROBADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
            }
            else if (flag == "R")
            {
                msjSolicitud = "Su Solicitud de Cambio ha sido Rechazada. Por favor contactese con su jefe inmediato para cualquier consulta.";
                status = "Solicitud Rechazada";
                asunto = "SOLICITUD DE CAMBIO RECHAZADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
            }
            else if (flag == "V")
            {
                msjSolicitud = "Se solicita la aprobación del cambio.";
                status = "Solicitud Validada.";
                asunto = "SOLICITUD DE CAMBIO VALIDADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
            }

            else if (flag == "O")
            {
                msjSolicitud = "Se solicita realizar operación de Solicitud.";
                status = "Solicitud Aprobada.";
                asunto = "SOLICITUD DE CAMBIO APROBADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
                idCuenta = 56;
            }
            else if (flag == "E")
            {
                msjSolicitud = "Se ejecutó la solicitud del cambio.";
                status = "Solicitud Ejecutada.";
                asunto = "SOLICITUD DE CAMBIO EJECUTADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
            }

            else if (flag == "EM")
            {
                msjSolicitud = "Se ejecutó la solicitud del cambio.";
                status = "Solicitud Ejecutada.";
                asunto = "SOLICITUD DE CAMBIO EJECUTADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
                idCuenta = 56;
            }
            else if (flag == "RM")
            {
                msjSolicitud = "Su Solicitud de Cambio ha sido Rechazada. Por favor contactese con su jefe inmediato para cualquier consulta.";
                status = "Solicitud Rechazada";
                asunto = "SOLICITUD DE CAMBIO RECHAZADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
                idCuenta = 56;
            }

            string aprobador = queryAprobador.NAM_PERSON;
            string IdCambio = Convert.ToString(queryRequest.IdCambio);

            //Envio de Correo 
            Body body = new Body();
            string body_html;
            if (idCuenta == 56)
            {
                 body_html = body.CorreoChangeRequestMinsur(nombreUsuario, aprobador, detailChange, colorSolicitudCambio, msjSolicitud, status, fechaRegistro, IdCambio);
            }
            else
            {
                body_html = body.CorreoChangeRequest(nombreUsuario, aprobador, detailChange, colorSolicitudCambio, msjSolicitud, status, fechaRegistro, IdCambio);
            }
            

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(mailUsuario) && !string.IsNullOrEmpty(mailJefe))
            {
                if (flag == "S" || flag == "V")
                {
                    message.To.Add(mailUsuario);
                    message.CC.Add(mailJefe);
                }
                else if (flag == "O")
                {
                    message.To.Add(mailUsuario);
                    //message.CC.Add(mailJefe);
                }
                else if (flag == "SM")
                {
                    message.To.Add(mailJefe);
                    //message.CC.Add(mailJefe);
                }
                else if (flag == "EM")
                {
                    message.To.Add(mailUsuario);
                }
                else if (flag == "RM")
                {
                    message.To.Add(mailUsuario);
                }
                else
                {
                    message.To.Add(mailUsuario);
                }
            }

            message.AlternateViews.Add(htmlView);
            message.Subject = asunto;
            newMail.smtp.Send(message);
            return true;
        }


        #region ERP

        public int sendMailUsersRequestBNV(CHANGE_DETAIL detail, CHANGE_APPROVED changeApproved, int flag)
        {
            var salida = 1;
            var queryUsuario = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == detail.ID_PERSON_ENTI)
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                    EMAIL_PERSON = pe.EMA_ELEC
                                }).Single();
            var queryAprobador = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == changeApproved.approver)
                                  join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                  select new
                                  {
                                      NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                      EMAIL_PERSON = pe.EMA_ELEC
                                  }).Single();

             var requester = dbc.CHANGE_REQUEST.Where(x=>x.id==changeApproved.idChangeRequest).FirstOrDefault().ID_PERS_ENTI;

            var queryRequester = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == requester)
                                  join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                  select new
                                  {
                                      NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                      //EMAIL_PERSON = pe.EMA_ELEC
                                  }).Single();

            var color = dbc.CHANGE_TYPE_REQUEST.Where(x => x.id == changeApproved.idTypeRequest).FirstOrDefault().Color;

            string mailUsuario = queryUsuario.EMAIL_PERSON;
            string nombreUsuario = queryRequester.NAM_PERSON;
            string detailChange = detail.DETAIL;
            string colorSolicitudCambio = color;
            string msjSolicitud = "", status = "";
            string asunto = "";
            if (flag == 1)
            {
                msjSolicitud = "Se solicita realizar la validación de la actividad en el Módulo de Gestión de Cambios.";
                status = "Nueva Solicitud";
            }
            else if (flag == 2)
            {
                msjSolicitud = "Se debe realizar la siguiente actividad.";
                status = "Solicitud Aprobada";
            }
            else if (flag == 3)
            {
                msjSolicitud = "Por favor ingresar al Módulo de Gestión de cambios a confirmar que se realizó la actividad.";
                status = "En ejecución";
                asunto = "CAMBIO EN EJECUCIÓN: " + "[" + Convert.ToString(detail.ID_CHANGE_REQUEST) + "] ";

            }
            string fechaRegistro = string.Format("{0:MM/dd/yy}", detail.FechaFinProgramada);

            string aprobador = queryAprobador.NAM_PERSON;
            var cambio = dbc.CHANGE_REQUEST.Where(x => x.id == changeApproved.idChangeRequest).FirstOrDefault().id;
            string IdCambio = Convert.ToString(cambio);

            //Envio de Correo 
            Body body = new Body();
            string body_html = body.CorreoChangeRequestBNV(nombreUsuario, aprobador, detailChange, colorSolicitudCambio, msjSolicitud, status, fechaRegistro, IdCambio);
            if (body_html != null)
            {

                string from = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                string pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                message.From = new MailAddress(from);
                message.Body = body_html;

              
                    //(ERP)
                    try
                    {
                        //var mails = dbc.sp_Email_Change_Request(Convert.ToInt32(IdCambio));
                        
                            if (mailUsuario.EndsWith("@buenaventura.pe") || mailUsuario.EndsWith("@electrodata.com.pe"))
                            {
                                if (!string.IsNullOrEmpty(mailUsuario))
                                {
                                    message.To.Add(mailUsuario);
                                }

                            }
                            else
                            {
                        message.To.Add(mailUsuario);
                        //salida = 0;
                            }

                        
                        // message.CC.Add(mailJefe);
                    }
                    catch (Exception e)
                    {
                    }
                if (salida == 1)
                {
                    message.AlternateViews.Add(htmlView);
                    message.Subject = asunto;
                    SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                    oSmtpClient.EnableSsl = true;
                    oSmtpClient.UseDefaultCredentials = false;
                    oSmtpClient.Port = 587;
                    oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    oSmtpClient.Send(message);
                    oSmtpClient.Dispose();
                }
                

            }

            return salida;

        }

        public int sendMailGestionCambiosBNV(int ID_PERS_ENTI, CHANGE_APPROVED ca, string flag)
        {
            var salida = 1;
            var queryUsuario = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                    EMAIL_PERSON = pe.EMA_ELEC
                                }).Single();
            var queryAprobador = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ca.approver)
                                  join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                  select new
                                  {
                                      NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                      EMAIL_PERSON = pe.EMA_ELEC
                                  }).Single();

            var queryRequest = (from apr in dbc.CHANGE_APPROVED.Where(x => x.id == ca.id)
                                join ce in dbc.CHANGE_REQUEST on apr.idChangeRequest equals ce.id
                                select new
                                {
                                    apr.id,
                                    IdCambio = ce.id,
                                    ce.question2,
                                    estado = apr.idTypeRequest
                                }).Single();

            var color = dbc.CHANGE_TYPE_REQUEST.Where(x => x.id == queryRequest.estado).FirstOrDefault().Color;

            string mailUsuario = queryUsuario.EMAIL_PERSON;
            string mailJefe = queryAprobador.EMAIL_PERSON;
            string nombreUsuario = queryUsuario.NAM_PERSON;
            string detailChange = queryRequest.question2;
            string colorSolicitudCambio = color;
            string msjSolicitud = "";
            string status = "";
            string asunto = "";
            string fechaRegistro = "";
            //if (flag == "S")
            //{
            //    msjSolicitud = "Se registró una nueva solicitud.";
            //    status = "Nueva Solicitud";
            //    asunto = "SOLICITUD DE CAMBIO: " + nombreUsuario + " - " + DateTime.Now.ToShortDateString();
            //    fechaRegistro = string.Format("{0:MM/dd/yy}", ca.createDate);
            //}
            if (flag == "N")
            {
                msjSolicitud = "Se registró una nueva solicitud.";
                status = "Nueva Solicitud";
                asunto = "SOLICITUD DE CAMBIO: " + "[" + Convert.ToString(queryRequest.IdCambio) + "] " ;
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.createDate);
            }
            else if (flag == "A")
            {
                msjSolicitud = "Su Solicitud de Cambio ha sido Aprobada. Por favor contactese con su jefe inmediato para cualquier consulta.";
                status = "Solicitud Aprobada";
                asunto = "SOLICITUD DE CAMBIO APROBADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
            }
            
            else if (flag == "AN")
            {
                msjSolicitud = "La Solicitud de Cambio ha sido Anulado.";
                status = "Solicitud Anulada";
                asunto = "SOLICITUD DE CAMBIO ANULADO: " + "[" + Convert.ToString(queryRequest.IdCambio) + "] ";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
            }
            else if (flag == "V")
            {
                msjSolicitud = "Se solicita la aprobación del cambio.";
                status = "Solicitud Validada.";
                asunto = "SOLICITUD DE CAMBIO VALIDADA";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
            }
            else if (flag == "E")
            {
                msjSolicitud = "Se ejecutó la solicitud del cambio.";
                status = "Solicitud Ejecutada.";
                asunto = "SOLICITUD DE CAMBIO EJECUTADA: " + "[" + Convert.ToString(queryRequest.IdCambio) + "] ";
                fechaRegistro = string.Format("{0:MM/dd/yy}", ca.modifiedDate);
            }

            string aprobador = queryAprobador.NAM_PERSON;
            string IdCambio = Convert.ToString(queryRequest.IdCambio);

            //Envio de Correo 
            Body body = new Body();
            string body_html = body.CorreoChangeRequestBNV(nombreUsuario, aprobador, detailChange, colorSolicitudCambio, msjSolicitud, status, fechaRegistro, IdCambio);

            if (body_html != null)
            {

                string from = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                string pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                message.From = new MailAddress(from);
                message.Body = body_html;

                SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                if ( flag == "N")
                {
                    message.To.Add(mailJefe);                   
                }

                else if(flag == "AN" || flag == "E")
                {
                    var mails = dbc.sp_Email_Change_Request(Convert.ToInt32(IdCambio));


                    foreach (var email in mails)
                    {
                        message.To.Add(email);
                    }
                   
                }

                else
                {
                    message.To.Add(mailUsuario);
                   
                }

                message.AlternateViews.Add(htmlView);
                message.Subject = asunto;
                // SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;
                oSmtpClient.Port = 587;
                oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                oSmtpClient.Send(message);
                oSmtpClient.Dispose();

            }
            return salida;
        }

        #endregion

        public bool sendMailUsersRequest(CHANGE_DETAIL detail, CHANGE_APPROVED changeApproved, int flag)
        {
            var queryUsuario = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == detail.ID_PERSON_ENTI)
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                    EMAIL_PERSON = pe.EMA_ELEC
                                }).Single();
            var queryAprobador = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == changeApproved.approver)
                                  join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                  select new
                                  {
                                      NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                      EMAIL_PERSON = pe.EMA_ELEC
                                  }).Single();

            var requester = dbc.CHANGE_REQUEST.Where(x => x.id == changeApproved.idChangeRequest).FirstOrDefault().ID_PERS_ENTI;
            var queryRequester = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == requester)
                                  join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                  select new
                                  {
                                      NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                      EMAIL_PERSON = pe.EMA_ELEC
                                  }).Single();

            var color = dbc.CHANGE_TYPE_REQUEST.Where(x => x.id == changeApproved.idTypeRequest).FirstOrDefault().Color;

            string mailUsuario = queryUsuario.EMAIL_PERSON;
            string nombreUsuario = queryRequester.NAM_PERSON;
            string detailChange = detail.DETAIL;
            string colorSolicitudCambio = color;
            string msjSolicitud = "", status = "";
            if (flag == 1)
            {
                msjSolicitud = "Se solicita realizar la validación de la actividad en el Módulo de Gestión de Cambios.";
                status = "Nueva Solicitud";
            }
            else if (flag == 2)
            {
                msjSolicitud = "Se debe realizar la siguiente actividad.";
                status = "Solicitud Aprobada";
            }
            else if (flag == 3)
            {
                msjSolicitud = "Por favor ingresar al Módulo de Gestión de cambios a confirmar que se realizó su actividad.";
                status = "En ejecución";
            }
            string fechaRegistro = string.Format("{0:MM/dd/yy}", detail.FechaFinProgramada);

            string aprobador = queryAprobador.NAM_PERSON;
            var cambio = dbc.CHANGE_REQUEST.Where(x => x.id == changeApproved.idChangeRequest).FirstOrDefault().id;
            string IdCambio = Convert.ToString(cambio);

            //Envio de Correo 
            Body body = new Body();
            string body_html = body.CorreoChangeRequest(nombreUsuario, aprobador, detailChange, colorSolicitudCambio, msjSolicitud, status, fechaRegistro, IdCambio);

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(mailUsuario))
            {
                message.To.Add(mailUsuario);
            }

            message.AlternateViews.Add(htmlView);
            message.Subject = "SOLICITUD DE CAMBIO - ACTIVIDADES PROGRAMADAS DESDE:" + detail.FechaInicioProgramada + " HASTA:" + detail.FechaFinProgramada;
            newMail.smtp.Send(message);
            return true;

        }
        //public bool sendMailtoPlanRequest(int ID_PERS_ENTI, CHANGE_APPROVED ca)
        //{
        //    string colorSolicitudCambio =string.Empty;
        //    string msjRechazoSolicitud = string.Empty;
        //    string status = string.Empty;

        //    var queryUsuario = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
        //                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
        //                        select new
        //                        {
        //                            NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
        //                            EMAIL_PERSON = pe.EMA_ELEC
        //                        }).Single();
        //    var queryAprobador = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ca.approver)
        //                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
        //                          select new
        //                          {
        //                              NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
        //                              EMAIL_PERSON = pe.EMA_ELEC
        //                          }).Single();

        //    var queryRequest = (from apr in dbc.CHANGE_APPROVED.Where(x => x.id == ca.id)
        //                        join ce in dbc.CHANGE_REQUEST on apr.idChangeRequest equals ce.id
        //                        select new
        //                        {
        //                            apr.id,
        //                            IdCambio = ce.id,
        //                            ce.question2
        //                        }).Single();

        //    string mailUsuario = queryUsuario.EMAIL_PERSON;
        //    string nombreUsuario = queryUsuario.NAM_PERSON;

        //    string detailChange = queryRequest.question2;
        //    string fechaRegistro = string.Format("{0:MM/dd/yy H:mm:ss}", ca.modifiedDate);
        //    string aprobador = queryAprobador.NAM_PERSON;
        //    string IdCambio = Convert.ToString(queryRequest.IdCambio);

        //    var color = dbc.CHANGE_TYPE_REQUEST.Where(x => x.id == ca.idTypeRequest).FirstOrDefault().Color;

        //    if (ca.idTypeRequest == 6)
        //    {
        //        colorSolicitudCambio = color;
        //        msjRechazoSolicitud = "Su solicitud de cambio no se ha ejecutado.";
        //        status = "Solicitud No Ejecutada";
        //    }
        //    else
        //    {
        //            colorSolicitudCambio = color;
        //         msjRechazoSolicitud  = "Su solicitud de cambio se ha ejecutado.";
        //         status  = "Solicitud Ejecutada";
        //    }


        //    //Envio de Correo 
        //    Body body = new Body();
        //    string body_html = body.CorreoChangeRequest(nombreUsuario, aprobador, detailChange, colorSolicitudCambio, msjRechazoSolicitud, status, fechaRegistro,IdCambio);

        //    PluginSmtp newMail = new PluginSmtp();
        //    var message = newMail.mailMessage;
        //    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
        //    if (!string.IsNullOrEmpty(mailUsuario))
        //    {
        //        message.To.Add(mailUsuario);
        //    }

        //    message.AlternateViews.Add(htmlView);
        //    message.Subject = "SOLICITUD DE CAMBIO Aprobada";
        //    newMail.smtp.Send(message);
        //    return true;
        //}
        public bool sendMailSummaryChange(int ID_PERS_ENTI, CHANGE_APPROVED cApproved, string CorreoJefe)
        {
            var queryUsuario = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                select new
                                {
                                    NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                    EMAIL_PERSON = pe.EMA_ELEC
                                }).Single();

            var queryRequest = (from ca in dbc.CHANGE_APPROVED.Where(x => x.id == cApproved.id)
                                join ce in dbc.CHANGE_REQUEST on ca.idChangeRequest equals ce.id
                                select new
                                {
                                    ca.id,
                                    IdCambio = ce.id,
                                    ce.question2
                                }).Single();
            var queryAprobador = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == cApproved.approver)
                                  join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                  select new
                                  {
                                      NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                      EMAIL_PERSON = pe.EMA_ELEC
                                  }).Single();
            string planAccion = cApproved.justificationChange + " " + "Programado para el dia:" + cApproved.changeDate;

            //Envio de Correo 
            string nombreUsuario = queryUsuario.NAM_PERSON;
            string detailChange = planAccion;
            string colorSolicitudCambio = "#7cbb00";
            string msjRegistroSolicitud = "Se cargo el resumen del cambio. ";
            string fechaRegistro = string.Format("{0:MM/dd/yy H:mm:ss}", cApproved.modifiedDate);
            string status = "Aprobado";
            string aprobador = queryAprobador.NAM_PERSON;
            string IdCambio = Convert.ToString(queryRequest.IdCambio);

            //Envio de Correo 
            Body body = new Body();
            string body_html = body.CorreoChangeRequest(nombreUsuario, aprobador, detailChange, colorSolicitudCambio, msjRegistroSolicitud, status, fechaRegistro, IdCambio);

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(CorreoJefe))
            {
                message.To.Add(CorreoJefe);
            }

            message.AlternateViews.Add(htmlView);
            message.Subject = "Resumen del Cambio de: " + nombreUsuario + " - " + DateTime.Now.ToShortDateString();
            newMail.smtp.Send(message);
            return true;
        }
        public bool SendMailCloseTicketActivity(string DES_ASUNTO, int UserId, ACTIVITY_LOG actlog)
        {
            var query = (from ce in dbe.CLASS_ENTITY.Where(x => x.UserId == UserId)
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         select new
                         {
                             NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                             EMAIL_PERSON = pe.EMA_ELEC
                         }).Single();
            var emailUsuario = query.EMAIL_PERSON;
            var nombreUsuario = query.NAM_PERSON;
            var nombreEmpresa = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == actlog.ID_CLIE).COM_NAME;
            Body body = new Body();
            string body_html = body.CreateDetailsCloseTicketActivity(UserId, actlog, nombreUsuario);
            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            string sendMail = "servicedesk@electrodata.com.pe";
            message.To.Add(sendMail);
            if (!string.IsNullOrEmpty(emailUsuario))
                message.Bcc.Add(emailUsuario);
            message.AlternateViews.Add(htmlView);
            message.Subject = "TICKET CERRADO: " + actlog.NAM_SUBTYPE_ACT + " - " + nombreUsuario + " - " + DateTime.Now.ToShortDateString();
            newMail.smtp.Send(message);
            return true;
        }
        public bool SendMailScheduledTicketActivity(string DES_ASUNTO, int UserId, ACTIVITY_LOG actlog, DateTime DATE_SCHEDULED)
        {
            var query = (from ce in dbe.CLASS_ENTITY.Where(x => x.UserId == UserId)
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         select new
                         {
                             NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                             EMAIL_PERSON = pe.EMA_ELEC
                         }).Single();
            var emailUsuario = query.EMAIL_PERSON;
            var nombreUsuario = query.NAM_PERSON;
            var nombreEmpresa = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == actlog.ID_CLIE).COM_NAME;
            Body body = new Body();
            string body_html = body.CreateDetailsScheduledTicketActivity(UserId, actlog, nombreUsuario, DATE_SCHEDULED);
            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            string sendMail = "servicedesk@electrodata.com.pe";
            message.To.Add(sendMail);
            if (!string.IsNullOrEmpty(emailUsuario))
                message.Bcc.Add(emailUsuario);
            message.AlternateViews.Add(htmlView);
            message.Subject = "TICKET REPROGRAMADO: " + actlog.NAM_SUBTYPE_ACT + " - " + nombreUsuario + " - " + DateTime.Now.ToShortDateString();
            newMail.smtp.Send(message);
            return true;
        }
        public bool SendMailEditActivity(string sendMail, int UserId, ACTIVITY_LOG act_log)
        {
            var query = (from ce in dbe.CLASS_ENTITY.Where(x => x.UserId == UserId)
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         select new
                         {
                             NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                             EMAIL_PERSON = pe.EMA_ELEC
                         }).Single();
            var emailUsuario = query.EMAIL_PERSON;
            var nombreUsuario = query.NAM_PERSON;

            var nombreEmpresa = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == act_log.ID_CLIE).COM_NAME;
            var query2 = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == act_log.ID_PERS_ENTI_EDIT)
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          select new
                          {
                              NAM_PERSON_EDIT = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                          }).Single();

            var nombreEdit = query2.NAM_PERSON_EDIT;

            Body body = new Body();
            string body_html = body.CreateDetailsEditActivity(UserId, act_log, nombreUsuario, nombreEdit);
            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(sendMail))
            {
                message.To.Add(sendMail);
                if (!string.IsNullOrEmpty(emailUsuario))
                    message.Bcc.Add(emailUsuario);
            }
            else
            {
                if (!string.IsNullOrEmpty(emailUsuario))
                {
                    message.To.Add(emailUsuario);
                }
                else
                {
                    return false;
                }
            }
            message.AlternateViews.Add(htmlView);
            message.Subject = "CORRECCION DE ACTIVIDAD: " + nombreUsuario + " - " + DateTime.Now.ToShortDateString();
            newMail.smtp.Send(message);
            return true;
        }

        public bool EnviarCorreoPMO(string OP, int ID_PERS_ENTI_PM, int ID_PERS_ENTI_ING, int UserId)
        {
            var queryRegistro = (from ce in dbe.CLASS_ENTITY.Where(x => x.UserId == UserId)
                                 join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                                 select new
                                 {
                                     NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                     EMAIL_PERSON = pe.EMA_ELEC
                                 }).SingleOrDefault();

            var emailUsuario = queryRegistro.EMAIL_PERSON == null ? "" : queryRegistro.EMAIL_PERSON;
            var nombreUsuario = queryRegistro.NAM_PERSON;

            var queryPM = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI_PM)
                           join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                           select new
                           {
                               NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                               EMAIL_PERSON = pe.EMA_ELEC
                           }).SingleOrDefault();

            //var queryING = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI_ING)
            //                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
            //                select new
            //                {
            //                    NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
            //                    EMAIL_PERSON = pe.EMA_ELEC
            //                }).Single();

            //var queryJefeIng = dbe.EMAIL_BOSS_INMSUP(ID_PERS_ENTI_ING);

            var queryJefePM = dbe.EMAIL_BOSS_INMSUP(ID_PERS_ENTI_PM).Single();

            if (queryJefePM.ID_PERS_ENTI != 1008) //GG
            {

            }

            Body body = new Body();
            string body_html = body.CreaCuerpoOP(OP, queryPM.NAM_PERSON);
            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            message.To.Add(queryPM.EMAIL_PERSON);
            if (Convert.ToInt32(queryJefePM.ID_PERS_ENTI) != 1008) //GG
            {
                message.CC.Add(queryJefePM.EMA_ELEC);
            }
            if (emailUsuario != "")
            {
                message.CC.Add(emailUsuario);
            }

            message.AlternateViews.Add(htmlView);
            message.Subject = "OP ASIGNADA: " + OP;
            newMail.smtp.Send(message);
            return true;
        }

        public bool EnviarCorreoCapacidadOP(int ID_PERS_ENTI_ING, int CantidadProyectos)
        {
            var queryIng = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI_ING)
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            select new
                            {
                                NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                EMAIL_PERSON = pe.EMA_ELEC
                            }).Single();

            Body body = new Body();
            string body_html = body.CreaCuerpoCapacidadOP(queryIng.NAM_PERSON, CantidadProyectos);
            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            message.To.Add("tdiaz@electrodata.com.pe,fsandoval@electrodata.com.pe,rvilchez@electrodata.com.pe");
            message.CC.Add("dnunez@electrodata.com.pe");
            //message.To.Add("yrocca@electrodata.com.pe,yrocca@electrodata.com.pe");//////
            //message.CC.Add("salmeyda@electrodata.com.pe");//////

            message.AlternateViews.Add(htmlView);
            message.Subject = "CAPACIDAD SUPERADA DE OPs ASIGNADAS: " + queryIng.NAM_PERSON;
            newMail.smtp.Send(message);
            return true;
        }

        public bool EnviarEvaluacionPersonal(int IdPersEnti, int Estado, int Flag)
        {
            var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).ToList();
            var IdPersEntiJefe = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);

            var queryEvaluado = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti)
                                 join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                 select new
                                 {
                                     NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                     EMAIL_PERSON = pe.EMA_ELEC
                                 }).Single();
            var nombreUsuario = queryEvaluado.NAM_PERSON;
            var emailUsuario = queryEvaluado.EMAIL_PERSON;

            var queryJefe = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEntiJefe)
                             join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                             select new
                             {
                                 NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                 EMAIL_PERSON = pe.EMA_ELEC
                             }).Single();
            var nombreJefe = queryJefe.NAM_PERSON;
            var emailJefe = queryJefe.EMAIL_PERSON;


            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;

            string mensaje1 = "", dirigidoA = "", mensaje2 = "";

            if (Estado == 1 && Flag == 0)
            {
                message.To.Add(emailJefe);//Jefe
                message.CC.Add(emailUsuario);//Usuario
                //message.To.Add("zrivadeneyra@electrodata.com.pe");
                mensaje1 = "Se encuentra una Evaluación pendiente de ";
                mensaje2 = "PENDIENTE JEFE DIRECTO";
                dirigidoA = nombreJefe;
            }

            if (Estado == 2 && Flag == 0)
            {
                message.To.Add("zrivadeneyra@electrodata.com.pe");//RRHH
                                                                  //message.CC.Add(emailUsuario);//Usuario

                mensaje1 = "Se ha finalizado la Evaluación de ";
                mensaje2 = "FINALIZADO";
                dirigidoA = "Área de Recursos Humanos";
            }


            Body body = new Body();
            string body_html = body.CreaCuerpoEvaluacionPersonal(mensaje1, nombreUsuario, nombreJefe, dirigidoA);
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

            //message.CC.Add(emailUsuario);

            message.AlternateViews.Add(htmlView);
            message.Subject = "EVALUACION DE PERSONAL: " + nombreUsuario + " - " + mensaje2;
            newMail.smtp.Send(message);
            return true;
        }

        public bool EnviarSolicitudPersonal(int IdPersEnti, int IdGerencia, int Estado, int id, int GTH, int Flag, string correo)
        {
            //var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).ToList();
            //var IdPersEntiJefe = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);

            //string EmailDirOperaciones = "";


            var usuarioCrea = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti)
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               select new
                               {
                                   NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                   EMAIL_PERSON = pe.EMA_ELEC
                               }).Single();
            var nombreCreador = usuarioCrea.NAM_PERSON;
            var emailCreador = usuarioCrea.EMAIL_PERSON;

            //En caso de Unidad Minera se cambiara de responsable de gerencia a Jefe habilitado para aprobar solicitud
            var GerenciaResponsable = new ResponsablexGerencia_Result();
            var IdPersEntiGerencia = 0;
            var nombreGerencia = "";
            var emailGerencia = "";
            // Id Unidad Minera es 3
            if (IdGerencia == 3)
            {
                //Obtenemos lista de jefes de Unidad Minera
                var listaJefes = (from aprobadorUM in dbe.AprobadorUM
                                  join pc in dbe.PERSON_CONTRACT on aprobadorUM.ID_PERS_ENTI equals pc.ID_PERS_ENTI into aprobadorPC
                                  from m in aprobadorPC.DefaultIfEmpty()
                                  select new
                                  {
                                      idPersEnti = aprobadorUM.ID_PERS_ENTI,
                                      chart = m.ID_CHAR,
                                      puedeAprobar = aprobadorUM.puedeAprobar,
                                      vigencia = m.VIG_CONT
                                  }).Where(x => x.vigencia == true).ToList();

                //var listaJefes = from AprobadorUM in context
                //Obtenemos el chart de inicio
                int chartActual = (int)dbe.ValidaUsuario(IdPersEnti).ToList()[0].ID_CHAR_PERS;
                bool encontrado = false;
                bool recorrioChart = false;
                while (encontrado == false && recorrioChart == false)
                {
                    int valorActual = chartActual;
                    int nuevoValor = obtenerChartParent(chartActual);
                    
                    chartActual = nuevoValor;
                    bool jefeEncontrado = listaJefes.Any(jefe => jefe.chart == chartActual);
                    if (jefeEncontrado)
                    {
                        encontrado = true;
                        IdPersEntiGerencia = (int)listaJefes.FirstOrDefault(jefe => jefe.chart == chartActual).idPersEnti;
                        
                    }
                    if (chartActual == -1)
                    {
                        recorrioChart = true;
                    }
                }
                var jefeDatos = (from pe in dbe.PERSON_ENTITY
                                 join ce in dbe.CLASS_ENTITY
                                 on pe.ID_ENTI2 equals ce.ID_ENTI into Details
                                 from m in Details.DefaultIfEmpty()
                                 where pe.ID_PERS_ENTI == IdPersEntiGerencia
                                 select new
                                 {
                                     nombre = m.FIR_NAME,
                                     apellido = m.LAS_NAME,
                                     email = pe.EMA_ELEC,
                                     idPersEnti = pe.ID_PERS_ENTI
                                 }).FirstOrDefault();
                nombreGerencia = jefeDatos.nombre + " "+ jefeDatos.apellido;
                emailGerencia = jefeDatos.email;
            }
            else
            {
                GerenciaResponsable = dbe.ResponsablexGerencia(IdGerencia).First();
                IdPersEntiGerencia = (int)GerenciaResponsable.ID_PERS_ENTI;
                nombreGerencia = GerenciaResponsable.Nombre;
                emailGerencia = GerenciaResponsable.Email;
            }
            

            var nombreEstado = dbe.EstadoSolicitud.Where(e => e.Id == Estado).FirstOrDefault().Nombre;

            var cargo = dbe.ObtenerDetalleSolicitud(id).FirstOrDefault().Cargo;

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;

            string mensaje1 = "", dirigidoA = "", mensaje2 = "";
            int ResGerencia = 0, ResOperaciones = 0;

            if (IdPersEnti == IdPersEntiGerencia)
            {
                ResGerencia = 1;
            }

            //if (IdGerencia == 2)
            //{
            //    var idArea = dbe.SolicitudPersonal.Where(e => e.Id == id).FirstOrDefault().IdArea;
            //    var IdPersOperaciones = dbe.ResponsablexArea(idArea).FirstOrDefault().ID_PERS_ENTI; 
            //    if (IdPersEnti == IdPersOperaciones)
            // {
            //  ResOperaciones = 1;
            // }
            //}

            if (Estado == 1 && Flag == 0)
            {
                message.To.Add("rrhh@electrodata.com.pe");

                message.CC.Add(emailCreador + "," + emailGerencia);
                dirigidoA = nombreGerencia;
                mensaje1 = "Se Encuentra una Solicitud Pendiente de " + nombreCreador + " para Continuar el Proceso se Requiere la Aprobación de " + nombreGerencia;
                mensaje2 = "SOLICITUD PENDIENTE";

            }

            if (Estado == 2 && Flag == 0)
            {
                message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");//rrhh@electrodata.com.pe
                if (GTH == 1 || ResGerencia == 1)
                {
                    message.CC.Add(emailCreador);
                }
                else
                {
                    message.To.Add(emailGerencia);//Jefe
                    message.CC.Add(emailCreador);//Usuario
                }


                mensaje1 = "Se Encuentra una Solicitud Pendiente de " + nombreCreador + " para Continuar el Proceso se Requiere la Aprobación de ZUMIKO RIVADENEYRA";
                mensaje2 = "SOLICITUD PENDIENTE PARA APROBACIÓN";
                dirigidoA = "ZUMIKO RIVADENEYRA";
            }

            if (Estado == 6 && Flag == 0)
            {
                message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");//rrhh@electrodata.com.pe
                if (GTH == 1 || ResGerencia == 1)
                {
                    message.CC.Add(emailCreador);
                }
                else
                {
                    message.To.Add(emailGerencia);//Jefe
                    message.CC.Add(emailCreador);//Usuario
                }
                mensaje1 = "La Solicitud de " + nombreCreador + " ha sido Aprobada, Ya se Encuentra Habilitado el Registro de Candidatos y su Calificación";
                mensaje2 = "SOLICITUD APROBADA";
                dirigidoA = "Área de Recursos Humanos , " + nombreCreador;
            }

            if (Estado == 4 && Flag == 0)
            {

                message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");
                if (GTH == 1 || ResGerencia == 1)
                {
                    message.CC.Add(emailCreador);
                }
                else
                {
                    message.To.Add(emailGerencia);//Jefe
                    message.CC.Add(emailCreador);//Usuario
                }

                //message.To.Add(emailGerencia);
                mensaje1 = "La Solicitud de " + nombreCreador + " lamentablemente ha sido Rechazada";
                mensaje2 = "SOLICITUD RECHAZADA";
                dirigidoA = nombreCreador;
            }

            if (Estado == 3 && Flag == 0)
            {

                message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");//RRHH
                if (GTH == 1 || ResGerencia == 1)
                {
                    message.CC.Add(emailCreador);
                }
                else
                {
                    message.To.Add(emailGerencia);//Jefe
                    message.CC.Add(emailCreador);//Usuario
                }

                //message.To.Add(emailGerencia);
                mensaje1 = "Los Candidatos de la Solicitud han sido Calificados y el Usuario " + nombreCreador + " ya aprobó a sus Mejores Canditados, Se espera la Aprobación de ZUMIKO RIVADENEYRA para la contratación";
                mensaje2 = "SOLICITUD CALIFICADA POR SOLICITANTE: " + nombreCreador;
                dirigidoA = "ZUMIKO RIVADENEYRA";
            }

            if (Estado == 5 && Flag == 0)
            {

                message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");//rrhh@electrodata.com.pe
                if (GTH == 1 || ResGerencia == 1)
                {
                    message.CC.Add(emailCreador);
                }
                else
                {
                    message.To.Add(emailGerencia);//Jefe
                    message.CC.Add(emailCreador);//Usuario
                }

                //message.To.Add(emailGerencia);
                mensaje1 = "Él o Los Candidatos de la Solicitud de " + nombreCreador + " han sido contratados";
                mensaje2 = "SOLICITUD COMPLETADA";
                dirigidoA = "Área de Recursos Humanos";
            }

            if (Estado == 6 && Flag == 1)
            {
                message.To.Add("rrhh@electrodata.com.pe");
                if (GTH == 1 || ResGerencia == 1)
                {

                    if (correo == null || correo == "")
                    {
                        message.CC.Add(emailCreador);
                        dirigidoA = nombreCreador;
                    }
                    else
                    {
                        message.To.Add(correo);
                        message.CC.Add(emailCreador);
                        dirigidoA = correo;
                    }

                }
                else
                {
                    if (correo == null || correo == "")
                    {
                        message.To.Add(emailGerencia);//Jefe
                        message.CC.Add(emailCreador);//Usuario
                        dirigidoA = nombreCreador;
                    }
                    else
                    {
                        message.To.Add(correo);
                        message.To.Add(emailGerencia);//Jefe
                        message.CC.Add(emailCreador);//Usuario  
                        dirigidoA = correo;
                    }


                }

                message.To.Add(emailCreador);//Jefe
                mensaje1 = "Ya se Cargaron Cv's a la Solicitud de " + nombreCreador + " , y Puede Calificarlos.";
                mensaje2 = "SOLICITUD CV'S CARGADOS";


            }


            Body body = new Body();
            string body_html = body.CreaCuerpoSolicitudPersonal(mensaje1, nombreCreador, nombreEstado, dirigidoA, id);
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

            //message.CC.Add(emailUsuario);

            message.AlternateViews.Add(htmlView);
            message.Subject = "SOLICITUD DE PERSONAL: " + cargo + " - " + mensaje2;
            newMail.smtp.Send(message);
            return true;
        }

        public bool EnviarSolicitudVacaciones(int IdVaca, int IdPersEnti, int IdPersEntiJefe, int Estado)
        {
            //var JefeInmediato = dbe.EMAIL_BOSS_INMSUP(IdPersEnti).ToList();
            //var IdPersEntiJefe = (JefeInmediato == null ? 0 : JefeInmediato.First().ID_PERS_ENTI);

            //string EmailDirOperaciones = "";


            var usuarioCrea = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti)
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               select new
                               {
                                   NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                   EMAIL_PERSON = pe.EMA_ELEC
                               }).Single();
            var nombreCreador = usuarioCrea.NAM_PERSON;
            var emailCreador = usuarioCrea.EMAIL_PERSON;

            var usuarioJefe = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEntiJefe)
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               select new
                               {
                                   NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                   EMAIL_PERSON = pe.EMA_ELEC
                               }).Single();
            var nombreJefe = usuarioJefe.NAM_PERSON;
            var emailJefe = usuarioJefe.EMAIL_PERSON;

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;

            string mensaje1 = "", dirigidoA = "", mensaje2 = "", nombreEstado = "", fecInicioFin = "";


            if (Estado == 1)
            {
                //message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");

                message.To.Add(emailCreador + "," + emailJefe);
                dirigidoA = nombreJefe;
                mensaje1 = "Se encuentra una Solicitud de vacaciones Pendiente de " + nombreCreador + ". Para continuar el proceso se requiere la Aprobación de " + nombreJefe + ".";
                mensaje2 = "SOLICITUD DE VACACIONES PENDIENTE";
                nombreEstado = "PENDIENTE";

            }

            if (Estado == 2)
            {

                message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");
                message.CC.Add(emailCreador + "," + emailJefe);
                dirigidoA = nombreCreador;
                mensaje1 = "La Solicitud de vacaciones de " + nombreCreador + " fue APROBADA correctamente por " + nombreJefe + ".";
                mensaje2 = "SOLICITUD DE VACACIONES APROBADA";
                nombreEstado = "APROBADO";

                var qVaca = dbe.ObtenerFechasVacacion(IdVaca).Single();
                fecInicioFin = qVaca.FechaInicioRetorno;
            }

            if (Estado == 5)
            {
                //message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");
                message.To.Add(emailCreador + "," + emailJefe);
                dirigidoA = nombreCreador;
                mensaje1 = "La Solicitud de vacaciones de " + nombreCreador + " fue CANCELADA por " + nombreJefe + ".";
                mensaje2 = "SOLICITUD DE VACACIONES CANCELADA";
                nombreEstado = "CANCELADO";
            }

            if (Estado == 6)
            {
                message.To.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");
                message.CC.Add(emailCreador + "," + emailJefe);
                dirigidoA = nombreCreador;
                mensaje1 = "La Solicitud de vacación de " + nombreCreador + " ha sido DECLARADA.";
                mensaje2 = "SOLICITUD DE VACACIÓN DECLARADA";
                nombreEstado = "DECLARADO";
            }


            Body body = new Body();
            string body_html = body.CreaCuerpoSolicitudVacacion(mensaje1, nombreCreador, nombreEstado, dirigidoA, IdVaca, fecInicioFin);
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

            //message.CC.Add(emailUsuario);

            message.AlternateViews.Add(htmlView);
            message.Subject = "SOLICITUD DE VACACIONES: " + nombreCreador + " - " + mensaje2;
            newMail.smtp.Send(message);
            return true;
        }

        public bool EnviarAsignadoTicketChat(int id)
        {
            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                          select new
                          {
                              ID = t.ID_TICK,
                              TITLE = t.TITLE_TICK,
                              FASIG = t.FEC_INI_TICK,
                              ID_PERS_ENTI = t.ID_PERS_ENTI,
                              SUMMARY = t.SUM_TICK,
                              CODE = t.COD_TICK,
                              STA = t.ID_STAT,
                              ASIG = t.ID_PERS_ENTI_ASSI
                          }).First();

            //string CreateBy = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI).ToList()
            //                   join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
            //                   select new
            //                   {
            //                       User = textInfo.ToTitleCase(ce.FIR_NAME.ToLower() + ' ' +
            //                              (ce.LAS_NAME == null ? "" : ce.LAS_NAME).ToLower()),
            //                   }).First().User;

            var usuarioCrea = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI)
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               select new
                               {
                                   NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                   EMAIL_PERSON = pe.EMA_ELEC
                               }).Single();
            var nombreCreador = usuarioCrea.NAM_PERSON;
            var emailCreador = usuarioCrea.EMAIL_PERSON;

            var usuarioAsig = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ticket.ASIG)
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               select new
                               {
                                   NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                   EMAIL_PERSON = pe.EMA_ELEC
                               }).Single();
            var nombreAsignado = usuarioAsig.NAM_PERSON;
            var emailAsignado = usuarioAsig.EMAIL_PERSON;

            var nombreEstado = dbc.STATUS.Where(e => e.ID_STAT == ticket.STA).FirstOrDefault().NAM_STAT;

            PluginSmtp newMail = new PluginSmtp();
            var message = newMail.mailMessage;

            var code = dbc.TICKETs.Where(p => p.ID_TICK == id).FirstOrDefault().COD_TICK;

            Body body = new Body();
            string body_html = body.CreaCuerpoTicketChatAsignado(ticket.CODE, ticket.TITLE, ticket.SUMMARY, nombreEstado, nombreCreador, Convert.ToString(ticket.FASIG), nombreAsignado, id);
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

            message.To.Add(emailCreador);
            message.CC.Add(emailAsignado);
            message.AlternateViews.Add(htmlView);
            message.Subject = "SU TICKET HA SIDO ASIGNADO - " + ticket.CODE;
            newMail.smtp.Send(message);
            return true;
        }

        public bool EnviarSoporteInforme(int IdDocuSale)
        {
            try
            {
                var OP = dbc.ObtenerDatosCorreoSoporte(IdDocuSale).Single();
                string NumeroOP = OP.OP;
                string Titulo = OP.Titulo;
                string PM = OP.PM;
                string emailPM = OP.CorreoPM;
                string cliente = OP.Cliente;
                string clienteFinal = OP.ClienteFinal;
                string destinatarios = OP.Destinatarios;
                string file = RptSoporteInformeExcel(IdDocuSale);

                Body body = new Body();
                string body_html = body.CreaCuerpoOPSoporteInforme(NumeroOP, Titulo, cliente, clienteFinal, PM);
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;

                message.AlternateViews.Add(htmlView);
                message.Subject = "Soporte Técnico y Mantenimiento - OP " + NumeroOP;
                message.To.Add(destinatarios);
                message.CC.Add(emailPM);

                Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                message.Attachments.Add(attachment);
                newMail.smtp.Send(message);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        //Creación de archivo Excel de Reporte de Soporte e informes OP
        public string RptSoporteInformeExcel(int IdDocuSale)
        {
            try
            {
                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;
                var ruta = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 1054 && x.ID_ACCO == 4 && x.VIG_ACCO_PARA == true).Single();
                string rutaArchivo = ruta.VAL_ACCO_PARA;

                ServerReport sr = new ServerReport();
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();

                sr.ReportServerCredentials = new CredencialesReporting(reportServerUser, reportServerPass, "");
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/Proyectos/ReporteProductosServicios";

                string DES_FILE = "ReporteSoporteOP";

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("IdDocuSale", Convert.ToString(IdDocuSale));

                sr.SetParameters(param);
                sr.Refresh();

                byte[] renderedBytes;
                renderedBytes = sr.Render("Excel");

                //Crea el Excel.
                bytes = sr.Render("Excel", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = rutaArchivo + DES_FILE + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
                return filename;
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        public SendMail()
        {
            //fromEmail = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 4 && x.ID_PARA == 24).VAL_ACCO_PARA.ToString();
            //pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 4 && x.ID_PARA == 25).VAL_ACCO_PARA.ToString();

            fromEmail = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_ACCO_PARA == 2796).VAL_ACCO_PARA.ToString();
            pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_ACCO_PARA == 2797).VAL_ACCO_PARA.ToString();

            MailAddress fromAddress = new MailAddress(fromEmail, "IT Management System");
            //MailMessage mailMessage = new MailMessage("esalazar@electrodata.com.pe" "esalazar@electrodata.com.pe", "text", "Text");
            //mailMessage.fr;
            mailMessage.From = fromAddress;

            smtp.EnableSsl = true;
            //smtp.
            NetworkCredential basicCredential = new NetworkCredential(fromEmail, pass);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = basicCredential;
            //smtp.Send(mailMessage);
        }

        public int obtenerChartParent(int chart)
        {
            try
            {
                var chartVal = dbe.CHARTs.Where(x => x.ID_CHAR == chart).ToList()[0].ID_CHAR_PARE.Value;
                return chartVal;
            }
            catch (Exception e)
            {
                return -1;
            }

        }

        public bool sendMailTareasBNV(int idTarea)
        {
            try
            {
                string asunto, correo;

                var tarea = dbc.ObtenerDatosTareaBNVEnvioCorreo(idTarea).FirstOrDefault();

                correo = tarea.Correo;
                asunto = "NUEVA TAREA: #" + tarea.CodigoTarea;
                int idTick = (int)tarea.IdTicket;

                Body body = new Body();
                string body_html = body.CorreoTareaBNV(tarea.Usuario, tarea.CodigoTicket, tarea.Titulo, tarea.Descripcion, "#ffbb00", tarea.Estado, tarea.FechaCrea, tarea.CodigoTarea, idTick);

                if (body_html != null)
                {

                    string from = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                    string pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                    message.From = new MailAddress(from);
                    message.Body = body_html;

                    SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                    message.To.Add(correo);

                    message.AlternateViews.Add(htmlView);
                    message.Subject = asunto;
                    oSmtpClient.EnableSsl = true;
                    oSmtpClient.UseDefaultCredentials = false;
                    oSmtpClient.Port = 587;
                    oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    oSmtpClient.Send(message);
                    oSmtpClient.Dispose();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /*Certificado*/

        public bool EnviarProgramacionCertificado(int IdCerti, int IdGerencia, int IdArea, int IdPersEnti, string NombreCert, DateTime FechaProgramada, int Estado, int jefeinmediato, string emailjefe)

        {

            try
            {

                var persona = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti)
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               select new
                               {
                                   NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper() + " " + ce.MOT_NAME.ToUpper(),
                                   EMAIL_PERSON = pe.EMA_ELEC
                               }).Single();
                var nombreColaborador = persona.NAM_PERSON;
                var emailColaborador = persona.EMAIL_PERSON;


                var certificado = (from c in dbe.Certificado.Where(x => x.Id == IdCerti)
                                   join cg in dbe.CHARTs on c.IdGerencia equals cg.ID_CHAR
                                   join ncg in dbe.NAME_CHART on cg.ID_NAM_CHAR equals ncg.ID_NAM_CHAR
                                   join ca in dbe.CHARTs on c.IdArea equals ca.ID_CHAR
                                   join nca in dbe.NAME_CHART on ca.ID_NAM_CHAR equals nca.ID_NAM_CHAR
                                   select new
                                   {
                                       c.Nombre,
                                       nombGerencia = ncg.NAM_CHAR,
                                       area = nca.NAM_CHAR,
                                       idmarca = c.IdMarca
                                   }).Single();
                var sitioGerencia = certificado.nombGerencia;
                var area = certificado.area;
                var marcaoid = certificado.idmarca;

                var marcaResult = (from m in dbc.MANUFACTURERs
                                   where m.ID_MANU == marcaoid
                                   select new
                                   {
                                       ma = m.NAM_MANU
                                   }).Single();

                var marca = marcaResult.ma;

                /*COLOR*/
                var estadoCertificado = (from e in dbe.EstadoCertificado
                                         where e.Id == Estado
                                         select new
                                         {
                                             Nombre = e.Nombre,
                                             ColorStatus = e.COL_STAT
                                         }).Single();

                var nombreEstado = estadoCertificado.Nombre;
                var colorEstado = estadoCertificado.ColorStatus;

                //En caso de Unidad Minera se cambiara de responsable de gerencia a Jefe habilitado para aprobar solicitud
                var GerenciaResponsable = new ResponsablexGerencia_Result();
                var IdPersEntiGerencia = 0;
                var nombreGerencia = "";
                var emailGerencia = "";
    

                GerenciaResponsable = dbe.ResponsablexGerencia(IdGerencia).First();
                IdPersEntiGerencia = (int)GerenciaResponsable.ID_PERS_ENTI;
                nombreGerencia = GerenciaResponsable.Nombre;
                emailGerencia = GerenciaResponsable.Email;

                //var jefeInmediato = (from Colaborador in dbe.PERSON_CONTRACT
                //                     join C in dbe.CHARTs on Colaborador.ID_CHAR equals C.ID_CHAR
                //                     join ContratoJefe in dbe.PERSON_CONTRACT on C.ID_CHAR_PARE equals ContratoJefe.ID_CHAR
                //                     join Jefe in dbe.PERSON_ENTITY on ContratoJefe.ID_PERS_ENTI equals Jefe.ID_PERS_ENTI
                //                     join ClaseJefe in dbe.CLASS_ENTITY on Jefe.ID_ENTI2 equals ClaseJefe.ID_ENTI
                //                     where Colaborador.ID_PERS_ENTI == jefeinmediato
                //                         && Colaborador.VIG_CONT == true
                //                         && Jefe.ID_PERS_STAT == 1
                //                     select new
                //                     {
                //                         nombrejefe = ClaseJefe.LAS_NAME + " " + ClaseJefe.MOT_NAME,
                //                         EmailJefe = Jefe.EMA_ELEC
                //                     }).FirstOrDefault();

              string JeEmail;

                if (emailjefe != null)
                {
                    JeEmail = emailjefe;
                }
                else
                {
                    JeEmail = "";
                }

                var JefeEmail = JeEmail;

                


                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;

                string asunto = "", estadoCer = "", titulo = "", mensaje = "", color = "";

                Body body = new Body();
                if (Estado == 1)
                {
                    message.To.Add(emailColaborador);

                    var copiaxdefecto = (from ap in dbc.ACCOUNT_PARAMETER
                                         where ap.ID_PARA == 2231 && ap.ID_ACCO == 4 &&
                                         ap.VIG_ACCO_PARA == true
                                         select new
                                         {
                                             cc = ap.VAL_ACCO_PARA
                                         }).Single();

                    var ccopia = copiaxdefecto.cc;

                    if (!string.IsNullOrEmpty(ccopia?.Trim()))
                    {
                        message.CC.Add(ccopia);
                    }
                    //message.CC.Add("rrhh@PRUEBA.COM,zrivadeneyra@PRUEBA.com");
                    if (!string.IsNullOrEmpty(emailGerencia?.Trim()))
                    {
                        message.CC.Add(emailGerencia);
                    }

                    if (!string.IsNullOrEmpty(JefeEmail?.Trim()))
                    {
                        message.CC.Add(JefeEmail);
                    }
                    asunto = "PROGRAMACIÓN DE CERTIFICADO";
                    estadoCer = nombreEstado;
                    titulo = "PROGRAMACIÓN DE CERTIFICADO";
                    mensaje = " Se ha registrado la programación del Certificado.";
                    color = colorEstado;

                    string body_html = body.CrearCuerpoCertificado((string)color, (int)IdCerti, (string)titulo, (string)nombreColaborador, (string)NombreCert, (string)sitioGerencia, (string)area, (string)marca, (string)mensaje, (string)estadoCer, (DateTime)FechaProgramada);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                    message.AlternateViews.Add(htmlView);
                    message.Subject = asunto + " - " + nombreColaborador + " - " + estadoCer;
                    newMail.smtp.Send(message);
                }


                if (Estado == 3)
                {

                    message.To.Add(emailColaborador);

                    //message.CC.Add("rrhh@electrodata.com.pe,zrivadeneyra@electrodata.com.pe");

                    var copiaxdefecto = (from ap in dbc.ACCOUNT_PARAMETER
                                         where ap.ID_PARA == 2232 && ap.ID_ACCO == 4 &&
                                         ap.VIG_ACCO_PARA == true
                                         select new
                                         {
                                             cc = ap.VAL_ACCO_PARA
                                         }).Single();

                    var ccopia = copiaxdefecto.cc;

                    if (!string.IsNullOrEmpty(ccopia?.Trim()))
                    {
                        message.CC.Add(ccopia);
                    }
                    if (!string.IsNullOrEmpty(emailGerencia?.Trim()))
                    {
                        message.CC.Add(emailGerencia);
                    }

                    if (!string.IsNullOrEmpty(JefeEmail?.Trim()))
                    {
                        message.CC.Add(JefeEmail);
                    }
                    estadoCer = nombreEstado;
                    asunto = "CERTIFICADO APROBADO";
                    titulo = "CERTIFICADO APROBADO";
                    mensaje = "	¡Felicidades por lograr la certificación! Ya se encuentra registrada en ITMS.";
                    color = colorEstado;

                    string body_html = body.CrearCuerpoCertificadoAprobado((string)color, (int)IdCerti, (string)titulo, (string)nombreColaborador, (string)NombreCert, (string)sitioGerencia, (string)area, (string)marca, (string)mensaje, (string)estadoCer, (DateTime)FechaProgramada);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                    message.AlternateViews.Add(htmlView);
                    message.Subject = asunto + " - " + nombreColaborador;
                    newMail.smtp.Send(message);
                }

                return true;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"Error al enviar el correo: {smtpEx.Message}");
                Console.WriteLine($"StackTrace: {smtpEx.StackTrace}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return false;
            }


        }

        public string ResetearContrasenaReingreso(string mailTo, string contrasena, string nomUser, string usuario)
        {
            try
            {
                Body body = new Body();
                string body_html = body.RestauraUsuarioITMS(nomUser, usuario, contrasena);

                PluginSmtp newMail = new PluginSmtp();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                message.To.Add(mailTo);

                message.AlternateViews.Add(htmlView);
                message.Subject = "Password restoration / Restauración de contraseña";
                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

    }
}
