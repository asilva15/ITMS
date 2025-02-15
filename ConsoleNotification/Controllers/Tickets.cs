using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleNotification.Models;
using ConsoleNotification.Plugins;
using ConsoleNotification.Templates;
using System.Net.Mail;
using System.Net.Mime;
using System.Data.Entity;
using System.Threading;
using System.Globalization;
using System.Threading;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.IO;
//using System.Web.Http;

namespace ConsoleNotification.Controllers
{
    //Envío de notificaciones referentes a las actividades realizadas al crear ticket o cambiar su estado.
    class Tickets
    {
        //Declaracion de Objetos para acceso a base de datos.
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        string reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
        private string IpServer = "";
        private AESRijndael seg = new AESRijndael();
        public static int contadorEncuestas = 0;

        public Tickets()
        {
            IpServer = ConfigurationManager.AppSettings["IpServer"].ToString();
            textInfo = cultureInfo.TextInfo;
        }

        //Envío de notificacionesal momento de crear un nuevo ticket.
        public string NewTicket()
        {
            int ID_TICK = 0;

            var qtk = (from t in dbc.TICKETs.Where(x => x.SEND_MAIL == false).Where(x => x.ID_ACCO != 60)
                       select new
                       {
                           t.ID_TICK,
                           t.ID_ACCO
                       }).ToList();

            if (qtk.Count() > 0)
            {
                try
                {
                    foreach (var tk in qtk)
                    {
                        ID_TICK = tk.ID_TICK;
                        string rpt = "";
                        if (tk.ID_ACCO == 56 || tk.ID_ACCO == 57 || tk.ID_ACCO == 58) // envío con nuevo correo para MINSUR
                        {
                            //rpt = SendMailTicketMinsur(ID_TICK);
                            rpt = "OK";
                        }
                        else if (tk.ID_ACCO == 60)
                        {
                            //rpt = SendMailTicketBuenaventura(ID_TICK);
                            rpt = "OK";
                        }
                        else
                            rpt = SendMailTicket(ID_TICK);

                        if (rpt == "OK")
                        {
                            var uticket = dbc.TICKETs.Find(ID_TICK);
                            uticket.SEND_MAIL = true;
                            uticket.CREATED_MAIL = DateTime.Now;
                            dbc.TICKETs.Attach(uticket);
                            dbc.Entry(uticket).State = EntityState.Modified;
                            dbc.SaveChanges();
                            Console.WriteLine("Se actualizó correctamente la información del Ticket código: " + uticket.COD_TICK + "\n");
                        }
                    }
                }
                catch
                {
                    return "Error al enviar correo de creación de Tickets";
                }

                return "OK";
            }
            else
            {
                return "No hubo ningún nuevo ticket para enviar correo";
            }

        }


        public string NewTicketChat()
        {
            int ID_TICK = 0;

            var qtk = (from t in dbc.TICKETs.Where(x => x.SEND_MAIL == false && x.ID_SOUR == 7)
                       select new
                       {
                           t.ID_TICK,
                       }).ToList();

            if (qtk.Count() > 0)
            {
                try
                {
                    foreach (var tk in qtk)
                    {
                        ID_TICK = tk.ID_TICK;
                        string rpt = SendMailTicketChat(tk.ID_TICK);

                        if (rpt == "OK")
                        {
                            var uticket = dbc.TICKETs.Find(tk.ID_TICK);
                            uticket.SEND_MAIL = true;
                            uticket.CREATED_MAIL = DateTime.Now;
                            dbc.TICKETs.Attach(uticket);
                            dbc.Entry(uticket).State = EntityState.Modified;
                            dbc.SaveChanges();
                            Console.WriteLine("Se actualizó correctamente la información del Ticket código: " + uticket.ID_TICK + "\n");
                        }
                    }
                }
                catch
                {
                    return "Error al enviar correo de creación de Tickets";
                }

                return "OK";
            }
            else
            {
                return "No hubo ningún nuevo ticket para enviar correo";
            }

        }

        //Envío de correos para actualización de estado
        public string UpdateStatus()
        {
            int ID_DETA_TICK = 0;

            var qdtk = (from dt in dbc.DETAILS_TICKET.Where(x => x.SEND_MAIL == false).Where(x => x.ID_TICK != null)
                        join t in dbc.TICKETs.Where(t => t.ID_ACCO != 60) on dt.ID_TICK equals t.ID_TICK
                        select new
                        {
                            dt.ID_DETA_TICK,
                            dt.ID_TYPE_DETA_TICK,
                            dt.ID_TICK, //
                            //t.ID_DOCU_SALE,
                            t.ID_ACCO
                        }).Take(100).ToList();

            if (qdtk.Count() > 0)
            {
                string rpt;
                try
                {
                    foreach (var dtk in qdtk)
                    {
                        rpt = "";

                        ID_DETA_TICK = dtk.ID_DETA_TICK;
                        var qTick = dbc.TICKETs.Single(x => x.ID_TICK == dtk.ID_TICK);
                        //////////////
                        /// LOG COMMENT
                        //rpt = "OK";
                        /// 
                        //////////////
                        if (dtk.ID_ACCO == 56 || dtk.ID_ACCO == 57 || dtk.ID_ACCO == 58)// ningun correo d minsur
                        {
                            rpt = "OK";
                        }
                        else
                        {
                            if (dtk.ID_TYPE_DETA_TICK == 3)
                            {
                                if (qTick.ID_ACCO == 60)
                                {
                                    //rpt = SendMailUpdateStatusBuenaventura(ID_DETA_TICK);
                                    rpt = "OK";
                                }
                                else
                                {
                                    rpt = SendMailUpdateStatus(ID_DETA_TICK);
                                }
                            }

                            if (dtk.ID_TYPE_DETA_TICK == 2)
                            {
                                if (qTick.ID_ACCO == 60)
                                {
                                    //rpt = EnviarCorreoTransferidoBVN(ID_DETA_TICK);
                                    rpt = "OK";
                                }
                                else
                                {
                                    rpt = SendMailTransferTicket(ID_DETA_TICK);
                                }
                            }
                        }

                        if (rpt == "OK")
                        {
                            var udticket = dbc.DETAILS_TICKET.Find(ID_DETA_TICK);
                            udticket.SEND_MAIL = true;
                            udticket.CREATED_MAIL = DateTime.Now;
                            dbc.DETAILS_TICKET.Attach(udticket);
                            dbc.Entry(udticket).State = EntityState.Modified;
                            dbc.SaveChanges();

                            Console.WriteLine("Se actualizó correctamente la información del Details_Ticket ID: " + ID_DETA_TICK + "\n");
                        }
                    }
                }
                catch (Exception e)
                {
                    return "Error al enviar correo de Details_Ticket" + e.ToString();
                }

                return "OK";
            }
            else
            {
                return "No hubo ningún Details_Ticket para enviar correo";
            }

        }

        //Envío de correos para conocimientos de tareas pendientes
        public string SendMailTareasPendientes()
        {
            int ID_TICK = 0;

            //join t in dbc.TICKETs on dt.ID_TICK equals t.ID_TICK
            var qdtk = (from tk in dbc.TICKETs.Where(x => x.SEND_MAIL_TAREA == false || x.SEND_MAIL_TAREA == null).Where(x => (x.ID_ACCO == 56 || x.ID_ACCO == 57 || x.ID_ACCO == 58) & (x.ID_CATE == 20813 || x.ID_CATE == 21265 || x.ID_CATE == 21296 || x.ID_CATE == 21300))
                        select new
                        {
                            tk.ID_TICK,
                            tk.ID_ACCO,
                            tk.ID_CATE,
                            tk.ID_STAT_END

                        }).Take(80).ToList();

            if (qdtk.Count() > 0)
            {
                string rpt;
                try
                {
                    foreach (var td in qdtk)
                    {
                        rpt = "";

                        ID_TICK = td.ID_TICK;
                        var qTick = dbc.TICKETs.Single(x => x.ID_TICK == td.ID_TICK);


                        if (qTick.ID_CATE == 20813)
                        {

                            int idtick = Convert.ToInt32(qTick.ID_TICK);

                            var lst_PersonasArea = dbc.CorreoMinsurxCuenta(idtick).ToList();
                            string lst_PersonasAreatodos = dbc.CorreoMinsurxCuenta(idtick).SingleOrDefault();


                            rpt = sendMailTicketAltaBaja(qTick.ID_TICK, "A", lst_PersonasAreatodos);

                        }

                        else if (qTick.ID_CATE == 21265 || qTick.ID_CATE == 21296 || qTick.ID_CATE == 21300)
                        {

                            int idtick = Convert.ToInt32(qTick.ID_TICK);

                            var lst_PersonasArea = dbc.CorreoMinsurxCuenta(idtick).ToList();
                            string lst_PersonasAreatodos = dbc.CorreoMinsurxCuenta(idtick).SingleOrDefault();

                            rpt = sendMailTicketAltaBaja(qTick.ID_TICK, "B", lst_PersonasAreatodos);

                        }



                        if (rpt == "OK")
                        {
                            var ticket = dbc.TICKETs.Find(ID_TICK);
                            ticket.SEND_MAIL_TAREA = true;

                            dbc.SaveChanges();

                            Console.WriteLine("Se actualizó correctamente la información del Ticket ID: " + ID_TICK + "\n");
                        }
                    }
                }
                catch (Exception e)
                {
                    return "Error al enviar correo de Details_Ticket";
                }

                return "OK";
            }
            else
            {
                return "No hubo ningún Details_Ticket para enviar correo";
            }

        }

        public string sendMailTicketAltaBaja(int idticket, string flag, string correos)
        {


            TICKET obj_Ticket = dbc.TICKETs.Where(x => x.ID_TICK == idticket).FirstOrDefault();
            //Tarea obj_Tarea = dbc.Tarea.Where(x => x.IdTarea == id).FirstOrDefault();


            string ticket = obj_Ticket.COD_TICK;
            string sumtik = obj_Ticket.SUM_TICK;
            //string comentario = obj_Tarea.DescripcionTarea;
            string msjSolicitud = "";
            string status = "";
            string asunto = "";
            string fechaRegistro = "";


            if (flag == "A")
            {
                msjSolicitud = "Ticket Alta";
                status = "TICKET ALTA / GESTION DE TAREAS";
                asunto = "TICKET ALTA / GESTION DE TAREAS";
            }

            if (flag == "B")
            {
                msjSolicitud = "Ticket Baja";
                status = "TICKET BAJA / GESTION DE TAREAS";
                asunto = "TICKET BAJA / GESTION DE TAREAS";
            }

            string envioflag = flag;

            //Envio de Correo 
            TicketTemplate body = new TicketTemplate();


            string body_html = body.TicketAltaBajaMinsur(sumtik, ticket, envioflag);


            SendMail newMail = new SendMail();
            var message = newMail.mailMessage;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
            if (!string.IsNullOrEmpty(correos))
            {

                message.To.Add(correos);

            }

            message.AlternateViews.Add(htmlView);
            message.Subject = asunto;
            newMail.smtp.Send(message);
            return "OK";
        }

        //Envio de correos resueltos de EL Toro
        public string ResolvedStatusElToro()
        {
            int ID_DETA_TICK = 0;
            //int ID_TICK = 0;

            var id = (from t in dbc.TICKETs.Where(x => x.ID_ACCO == 31).Where(p => p.ID_STAT_END == 4)
                      select new
                      {
                          t.ID_TICK
                      }).Take(40).ToList();


            var qdtk = (from dt in dbc.DETAILS_TICKET.Where(x => x.SEND_MAIL == false)
                            //join t in dbc.TICKETs on dt.ID_TICK equals t.ID_TICK
                        select new
                        {
                            dt.ID_DETA_TICK,
                            dt.ID_TYPE_DETA_TICK,
                            dt.ID_TICK

                        }).Take(40).ToList();

            var tick = (from qq in qdtk
                        join t in id on qq.ID_DETA_TICK equals t.ID_TICK
                        select new
                        {
                            qq.ID_DETA_TICK,
                            qq.ID_TYPE_DETA_TICK
                        }).Take(40).ToList();


            if (tick.Count() > 0)
            {
                string rpt;
                try
                {
                    foreach (var dtk in tick)
                    {
                        rpt = "";

                        ID_DETA_TICK = dtk.ID_DETA_TICK;

                        if (dtk.ID_TYPE_DETA_TICK == 3)
                        {
                            //if(){

                            //}
                            //else{

                            rpt = SendMailUpdateStatus(ID_DETA_TICK);
                            //}


                        }

                        if (dtk.ID_TYPE_DETA_TICK == 2)
                        {
                            rpt = SendMailTransferTicket(ID_DETA_TICK);
                        }

                        if (rpt == "OK")
                        {
                            var udticket = dbc.DETAILS_TICKET.Find(ID_DETA_TICK);
                            udticket.SEND_MAIL = true;
                            udticket.CREATED_MAIL = DateTime.Now;
                            dbc.DETAILS_TICKET.Attach(udticket);
                            dbc.Entry(udticket).State = EntityState.Modified;
                            dbc.SaveChanges();

                            Console.WriteLine("Se actualizó correctamente la información del Datails_Ticket ID: " + ID_DETA_TICK + "\n");
                        }
                    }
                }
                catch
                {
                    return "Error al enviar correo de Details_Ticket";
                }

                return "OK";
            }
            else
            {
                return "No hubo ningún Details_Ticket para enviar correo";
            }

        }

        //Creación de contenido y envío de correo.
        public string SendMailTicket(int ID_TICK)
        {
            try
            {
                int ID_ACCO = 0, ID_QUEU = 0, ID_PERS_ENTI_ASSI = 0, ID_CATE;

                int QUE_SUPE_MAIL = 0, IdCoor;
                int ASM = 1;
                string mailCliente = "";
                ACCOUNT_PARAMETER coordinator;

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                ID_ACCO = Convert.ToInt32(ticket.ID_ACCO);
                ID_QUEU = Convert.ToInt32(ticket.ID_QUEU);
                ID_CATE = Convert.ToInt32(ticket.ID_CATE);
                ID_PERS_ENTI_ASSI = Convert.ToInt32(ticket.ID_PERS_ENTI_ASSI);
                Validate vmail = new Validate();

                //Implementación de copia de correo a solicitante.
                if ((ticket.ID_ACCO.Value == ASM || ticket.ID_ACCO.Value == 25) && ticket.ID_STAT_END.Value == 4)
                {
                    try
                    {
                        PERSON_ENTITY peEUA = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END.Value);
                        if (vmail.email_bien_escrito(peEUA.EMA_PERS) && peEUA.ID_TYPE_CLIE.Value == 2)
                        {
                            //Descomentar aquí para enviar correos y notificaciones a los clientes.
                            //mailCliente = "," + pe.EMA_PERS+",gintor@electrodata.com.pe";
                            mailCliente = peEUA.EMA_PERS;
                        }
                        else
                        {
                            Console.WriteLine("Revisar correo para usuario solicitante ticket: " + ticket.COD_TICK);
                        }
                    }
                    catch
                    {
                        mailCliente = "";
                    }

                }

                try
                {
                    QUE_SUPE_MAIL = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 30)
                        .Where(x => x.ID_ACCO == ID_ACCO).Count();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ticket -> Tabla Account_Parameter ID_PARA=30 (SIN SUPERVISOR COLA) para el ID_ACCO: " + ID_ACCO);
                    Console.WriteLine("Detalle generado: " + e.Message.ToString());
                }

                try
                {
                    coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                           .Single(x => x.ID_ACCO == ticket.ID_ACCO);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Ticket -> Tabla Account_Parameter ID_PARA=19 (SIN SUPERVISOR DE CUENTA) para el ID_ACCO: " + ticket.ID_ACCO);
                    Console.WriteLine("Detalle generado: " + exc.Message.ToString());
                    coordinator = null;
                }

                var qEma = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);


                string mailTo = null, mail_coor = null, mail_group = null;

                //No envía correos de notificación a José Ishikawa, Liliana Chávez y Victor Deluchi
                if (ID_ACCO == 1 && (ID_PERS_ENTI_ASSI == 38825 || ID_PERS_ENTI_ASSI == 553 || ID_PERS_ENTI_ASSI == 43602))
                {
                    mailTo = "itms@electrodata.com.pe";
                }
                else
                {
                    mailTo = qEma.EMA_ELEC;

                    if (String.IsNullOrEmpty(mailTo))
                    {
                        mailTo = "itms@electrodata.com.pe";
                    }
                    if (!String.IsNullOrEmpty(qEma.EMA_PERS))
                    {
                        mailTo = mailTo + (qEma.EMA_PERS.Length > 5 ? "" : "," + qEma.EMA_PERS);
                    }
                }

                if (QUE_SUPE_MAIL == 0 && coordinator != null)
                {
                    IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);
                }
                else
                {
                    try
                    {
                        IdCoor = dbc.ACCOUNT_QUEUE.Where(x => x.ID_QUEU == ID_QUEU).Single(x => x.ID_ACCO == ID_ACCO).ID_PERS_ENTI_CORD.Value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: No se encontro coordinador de la cola: " + ID_QUEU + " Para el ticket código: " + ticket.COD_TICK);
                        Console.WriteLine("Error generado: " + ex.Message.ToString());
                        IdCoor = 0;
                    }
                }
                try
                {
                    mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor).EMA_ELEC;
                }
                catch
                {
                    mail_coor = null;
                }

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

                TicketTemplate body = new TicketTemplate();
                string body_html = "";

                //copiando al grupo si el ticket es por una OP
                if (ticket.ID_DOCU_SALE != null)
                {
                    var qQueue = dbc.ACCOUNT_QUEUE
                        .Where(x => x.ID_ACCO == ID_ACCO)
                        .Single(x => x.ID_QUEU == ticket.ID_QUEU);

                    if (!String.IsNullOrEmpty(qQueue.EMA_ACCO_QUEU))
                    {
                        mail_group = (mail_coor.Length > 5 ? mail_coor + "," : "") + qQueue.EMA_ACCO_QUEU;
                    }
                }

                try
                {
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    message.To.Add(mailTo);

                    if (ID_PERS_ENTI_ASSI != IdCoor)
                    {
                        if (!String.IsNullOrEmpty(mail_group))
                        {
                            mail_coor = (mail_coor.Length > 0 ? mail_coor + "," : "") + mail_group;
                        }

                        message.CC.Add(mail_coor);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(mail_group))
                        {
                            message.CC.Add(mail_group);
                        }
                    }

                    //Copia a SIG
                    if (ID_ACCO == 4 && (ticket.ID_STAT == 1 || ticket.ID_STAT == 3))
                    {
                        if (ticket.ID_CATE == 18085) // Ticket con categoría Gestión De Servicios-Desarrollo de la información-Queja/Reclamo-Queja/Reclamo
                        {
                            message.CC.Add("sig@electrodata.com.pe");
                        }
                    }
                    //Solicitud del cliente - Goldfields - inicio
                    if ((ID_ACCO == 1 || ID_ACCO == 25) && (ticket.ID_STAT == 1 || ticket.ID_STAT == 3))
                    {
                        var categoria = dbc.CATEGORies.Single(x => x.ID_CATE == ticket.ID_CATE);
                        if (categoria.ID_CATE_PARE == 15302) // Ticket con categoría Gestión De Servicios-Gestión De Seguridad Informática-AMP
                        {
                            message.CC.Add("ingenieria@mcinnovatec.com,CyberSOC@mcinnovatec.com,Jose.Ishikawa@goldfields.com,juanfrancisco.burgacolchado@telefonica.com");
                            message.CC.Add("jorgeluis.saldanatorres@telefonica.com,roberto.tupacyupanqui@telefonica.com,qos@mcinnovatec.com,josero.manta@telefonica.com");
                        }
                    }

                    if ((ID_ACCO == 1 || ID_ACCO == 25) && ticket.ID_PRIO <= 2 && (ticket.ID_STAT == 1 || ticket.ID_STAT == 3))
                    {

                        message.CC.Add("Liliana.chavez@goldfields.com,Victor.Delucchi@goldfields.com");
                        //message.CC.Add("edeza@electrodata.com.pe,evalverde@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                        //message.CC.Add("edeza@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                        message.CC.Add("cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");

                        if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 41457 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 45674 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 51898) // Usuario Snode Guardian
                        {
                            if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 51898)
                            {
                                message.CC.Add("Guardian@snode.com,vulscans@snode.com");
                            }
                            body_html = body.CreateTicketGoldFieldsIngles(ID_TICK, 1);
                        }
                        else
                        {
                            body_html = body.CreateTicketGoldFields(ID_TICK, 1);
                        }
                    }
                    else if (ID_ACCO == 20)
                    {
                        message.CC.Add("publicom@electrodata.com.pe");
                        body_html = body.CreateTicketGoldFields(ID_TICK, 1);
                    }
                    else
                    {
                        if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 44547 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 47441)//BOC Francis Rodriguez
                        {
                            message.CC.Add("francis.rodriguez@pe.bocusa.com");
                            body_html = body.CreateTicketBOC(ID_TICK, 1);
                        }
                        else
                        {
                            body_html = body.CreateTicket(ID_TICK);
                        }
                    }
                    //Solicitud del cliente - Goldfields - fin

                    //temporalmente
                    if (ID_ACCO == 1 || ID_ACCO == 25)
                    {
                        //message.Bcc.Add("evalverde@electrodata.com.pe,edeza@electrodata.com.pe, destraver@electrodata.com.pe");
                        //message.Bcc.Add("edeza@electrodata.com.pe, destraver@electrodata.com.pe");
                        message.Bcc.Add("destraver@electrodata.com.pe");
                    }
                    if (ID_ACCO == 4) { message.Bcc.Add("servicedesk@electrodata.com.pe"); }//Control de envio de correos


                    if (body_html != null)
                    {
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                        message.AlternateViews.Add(htmlView);
                        if ((ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58) && ticket.ID_CATE != null)
                        {
                            if (ticket.ID_CATE == 20706 ||
                               ticket.ID_CATE == 20707 ||
                               ticket.ID_CATE == 20708 ||
                               ticket.ID_CATE == 20711 ||
                               ticket.ID_CATE == 20713 ||
                               ticket.ID_CATE == 20715 ||
                               ticket.ID_CATE == 20718 ||
                               ticket.ID_CATE == 20721 ||
                               ticket.ID_CATE == 20723)
                            {
                                //message.Bcc.Add("mesadeayuda@minsur.com");
                                //var qAnalitica = dbc.ACCOUNT_PARAMETER
                                //.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 1062 && x.VAL_ACCO_PARA == Convert.ToString(ticket.ID_CATE)).ToList().Count();
                                //if (qAnalitica > 0)
                                //{
                                message.Subject = "INCIDENCIA ANALITICA";
                                Console.WriteLine("Envío de correo INCIDENCIA ANALITICA - MINSUR");
                                //}
                                //else {
                                //    message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                                //}
                            }
                            else
                            {
                                message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                            }
                        }
                        else
                        {
                            message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                        }
                        newMail.smtp.Send(message);

                        // Se agrega validación para que no envíe correos repetidos
                        var uticket = dbc.TICKETs.Find(ID_TICK);
                        uticket.SEND_MAIL = true;
                        uticket.CREATED_MAIL = DateTime.Now;
                        dbc.TICKETs.Attach(uticket);
                        dbc.Entry(uticket).State = EntityState.Modified;
                        dbc.SaveChanges();

                        Console.WriteLine("To: " + mailTo);
                        Console.WriteLine("CC(Mail_Coor): " + mail_coor);
                        Console.WriteLine("CC(Mail_Group): " + mail_group);
                        Console.WriteLine("Se envió correo de la creación del Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");

                        //Solicitud de goldfields
                        try
                        {
                            if ((ticket.ID_ACCO.Value == ASM || ticket.ID_ACCO == 25) && ticket.ID_STAT_END.Value == 4 && mailCliente != "" && caseSwitch == 4)
                            {
                                SendMail newMailGF = new SendMail();
                                var messageGF = newMailGF.mailMessage;
                                messageGF.To.Add(mailCliente);
                                //Solicitado por el cliente
                                if (ticket.ID_PRIO <= 2)
                                {
                                    message.CC.Add("Liliana.chavez@goldfields.com,Victor.Delucchi@goldfields.com");
                                    //message.CC.Add("edeza@electrodata.com.pe,evalverde@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                                    //message.CC.Add("edeza@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                                    message.CC.Add("cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                                }
                                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 41457) // Usuario Snode Guardian
                                {
                                    body_html = body.CreateTicketGoldFieldsIngles(ID_TICK, 1);
                                }
                                else
                                {
                                    body_html = body.CreateTicketGoldFields(ID_TICK, 1);
                                }

                                htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                                messageGF.AlternateViews.Add(htmlView);
                                messageGF.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                                newMailGF.smtp.Send(messageGF);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error al enviar al USUARIO el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                            Console.WriteLine("Error generado: " + ex.Message.ToString());
                        }

                        return "OK";
                    }
                    else
                    {
                        Console.WriteLine("Error: No se pudo generar el Template del Ticket código: " + ticket.COD_TICK);
                        return "ERROR";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al enviar el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");

                    Console.WriteLine("Error generado: " + ex.Message.ToString());

                    return "ERROR";
                }
            }
            catch (Exception exx)
            {
                EXCEPTION exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = exx.Message.ToString();
                exc.DES_EXCE = "Send Mail - New Ticket";
                dbl.EXCEPTIONs.Add(exc);
                dbl.SaveChanges();

                Console.WriteLine("Hubo un error al enviar mensaje de creación de Ticket");

                Console.WriteLine("Error generado: " + exx.Message.ToString());

                return "ERROR";
            }
        }

        //Creación de contenido y envío de correo MINSUR.
        public string SendMailTicketMinsur(int ID_TICK)
        {
            try
            {
                int ID_ACCO = 0, ID_QUEU = 0, ID_PERS_ENTI_ASSI = 0, ID_CATE;

                int QUE_SUPE_MAIL = 0, IdCoor;
                int ASM = 1;
                string mailCliente = "";
                ACCOUNT_PARAMETER coordinator;

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                ID_ACCO = Convert.ToInt32(ticket.ID_ACCO);
                ID_QUEU = Convert.ToInt32(ticket.ID_QUEU);
                ID_CATE = Convert.ToInt32(ticket.ID_CATE);
                ID_PERS_ENTI_ASSI = Convert.ToInt32(ticket.ID_PERS_ENTI_ASSI);
                Validate vmail = new Validate();

                //Implementación de copia de correo a solicitante.
                if ((ticket.ID_ACCO.Value == ASM || ticket.ID_ACCO.Value == 25) && ticket.ID_STAT_END.Value == 4)
                {
                    try
                    {
                        PERSON_ENTITY peEUA = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END.Value);
                        if (vmail.email_bien_escrito(peEUA.EMA_PERS) && peEUA.ID_TYPE_CLIE.Value == 2)
                        {
                            //Descomentar aquí para enviar correos y notificaciones a los clientes.
                            //mailCliente = "," + pe.EMA_PERS+",gintor@electrodata.com.pe";
                            mailCliente = peEUA.EMA_PERS;
                        }
                        else
                        {
                            Console.WriteLine("Revisar correo para usuario solicitante ticket: " + ticket.COD_TICK);
                        }
                    }
                    catch
                    {
                        mailCliente = "";
                    }

                }

                try
                {
                    QUE_SUPE_MAIL = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 30)
                        .Where(x => x.ID_ACCO == ID_ACCO).Count();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ticket -> Tabla Account_Parameter ID_PARA=30 (SIN SUPERVISOR COLA) para el ID_ACCO: " + ID_ACCO);
                    Console.WriteLine("Detalle generado: " + e.Message.ToString());
                }

                try
                {
                    coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                           .Single(x => x.ID_ACCO == ticket.ID_ACCO);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Ticket -> Tabla Account_Parameter ID_PARA=19 (SIN SUPERVISOR DE CUENTA) para el ID_ACCO: " + ticket.ID_ACCO);
                    Console.WriteLine("Detalle generado: " + exc.Message.ToString());
                    coordinator = null;
                }

                var qEma = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);


                string mailTo = null, mail_coor = null, mail_group = null;

                //No envía correos de notificación a José Ishikawa, Liliana Chávez y Victor Deluchi
                if (ID_ACCO == 1 && (ID_PERS_ENTI_ASSI == 38825 || ID_PERS_ENTI_ASSI == 553 || ID_PERS_ENTI_ASSI == 43602))
                {
                    mailTo = "itms@electrodata.com.pe";
                }
                else
                {
                    mailTo = qEma.EMA_ELEC;

                    if (String.IsNullOrEmpty(mailTo))
                    {
                        mailTo = "itms@electrodata.com.pe";
                    }
                    if (!String.IsNullOrEmpty(qEma.EMA_PERS))
                    {
                        mailTo = mailTo + (qEma.EMA_PERS.Length > 5 ? "" : "," + qEma.EMA_PERS);
                    }
                }

                if (QUE_SUPE_MAIL == 0 && coordinator != null)
                {
                    IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);
                }
                else
                {
                    try
                    {
                        IdCoor = dbc.ACCOUNT_QUEUE.Where(x => x.ID_QUEU == ID_QUEU).Single(x => x.ID_ACCO == ID_ACCO).ID_PERS_ENTI_CORD.Value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: No se encontro coordinador de la cola: " + ID_QUEU + " Para el ticket código: " + ticket.COD_TICK);
                        Console.WriteLine("Error generado: " + ex.Message.ToString());
                        IdCoor = 0;
                    }
                }
                try
                {
                    mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor).EMA_ELEC;
                }
                catch
                {
                    mail_coor = null;
                }

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

                TicketTemplate body = new TicketTemplate();
                string body_html = "";

                //copiando al grupo si el ticket es por una OP
                if (ticket.ID_DOCU_SALE != null)
                {
                    var qQueue = dbc.ACCOUNT_QUEUE
                        .Where(x => x.ID_ACCO == ID_ACCO)
                        .Single(x => x.ID_QUEU == ticket.ID_QUEU);

                    if (!String.IsNullOrEmpty(qQueue.EMA_ACCO_QUEU))
                    {
                        mail_group = (mail_coor.Length > 5 ? mail_coor + "," : "") + qQueue.EMA_ACCO_QUEU;
                    }
                }

                try
                {
                    SendMailMinsur newMail = new SendMailMinsur();
                    var message = newMail.mailMessage;
                    message.To.Add(mailTo);

                    if (ID_PERS_ENTI_ASSI != IdCoor)
                    {
                        if (!String.IsNullOrEmpty(mail_group))
                        {
                            mail_coor = (mail_coor.Length > 0 ? mail_coor + "," : "") + mail_group;
                        }

                        message.CC.Add(mail_coor);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(mail_group))
                        {
                            message.CC.Add(mail_group);
                        }
                    }

                    //Copia a SIG
                    if (ID_ACCO == 4 && (ticket.ID_STAT == 1 || ticket.ID_STAT == 3))
                    {
                        if (ticket.ID_CATE == 18085) // Ticket con categoría Gestión De Servicios-Desarrollo de la información-Queja/Reclamo-Queja/Reclamo
                        {
                            message.CC.Add("sig@electrodata.com.pe");
                        }
                    }
                    //Solicitud del cliente - Goldfields - inicio
                    if ((ID_ACCO == 1 || ID_ACCO == 25) && (ticket.ID_STAT == 1 || ticket.ID_STAT == 3))
                    {
                        var categoria = dbc.CATEGORies.Single(x => x.ID_CATE == ticket.ID_CATE);
                        if (categoria.ID_CATE_PARE == 15302) // Ticket con categoría Gestión De Servicios-Gestión De Seguridad Informática-AMP
                        {
                            message.CC.Add("ingenieria@mcinnovatec.com,CyberSOC@mcinnovatec.com,Jose.Ishikawa@goldfields.com,juanfrancisco.burgacolchado@telefonica.com");
                            message.CC.Add("jorgeluis.saldanatorres@telefonica.com,roberto.tupacyupanqui@telefonica.com,qos@mcinnovatec.com,josero.manta@telefonica.com");
                        }
                    }

                    if ((ID_ACCO == 1 || ID_ACCO == 25) && ticket.ID_PRIO <= 2 && (ticket.ID_STAT == 1 || ticket.ID_STAT == 3))
                    {

                        message.CC.Add("Liliana.chavez@goldfields.com,Victor.Delucchi@goldfields.com");
                        //message.CC.Add("edeza@electrodata.com.pe,evalverde@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                        // message.CC.Add("edeza@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                        message.CC.Add("cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");

                        if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 41457 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 45674 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 51898) // Usuario Snode Guardian
                        {
                            if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 51898)
                            {
                                message.CC.Add("Guardian@snode.com,vulscans@snode.com");
                            }
                            body_html = body.CreateTicketGoldFieldsIngles(ID_TICK, 1);
                        }
                        else
                        {
                            body_html = body.CreateTicketGoldFields(ID_TICK, 1);
                        }
                    }
                    else if (ID_ACCO == 20)
                    {
                        message.CC.Add("publicom@electrodata.com.pe");
                        body_html = body.CreateTicketGoldFields(ID_TICK, 1);
                    }
                    else
                    {
                        if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 44547 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 47441)//BOC Francis Rodriguez
                        {
                            message.CC.Add("francis.rodriguez@pe.bocusa.com");
                            body_html = body.CreateTicketBOC(ID_TICK, 1);
                        }
                        else
                        {
                            body_html = body.CreateTicket(ID_TICK);
                        }
                    }
                    //Solicitud del cliente - Goldfields - fin

                    //temporalmente
                    if (ID_ACCO == 1 || ID_ACCO == 25)
                    {
                        //message.Bcc.Add("evalverde@electrodata.com.pe,edeza@electrodata.com.pe, destraver@electrodata.com.pe");
                        //message.Bcc.Add("edeza@electrodata.com.pe, destraver@electrodata.com.pe");
                        message.Bcc.Add("destraver@electrodata.com.pe");
                    }
                    if (ID_ACCO == 4) { message.Bcc.Add("servicedesk@electrodata.com.pe"); }//Control de envio de correos


                    if (body_html != null)
                    {
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                        message.AlternateViews.Add(htmlView);
                        if ((ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58) && ticket.ID_CATE != null)
                        {
                            if (ticket.ID_CATE == 20706 ||
                               ticket.ID_CATE == 20707 ||
                               ticket.ID_CATE == 20708 ||
                               ticket.ID_CATE == 20711 ||
                               ticket.ID_CATE == 20713 ||
                               ticket.ID_CATE == 20715 ||
                               ticket.ID_CATE == 20718 ||
                               ticket.ID_CATE == 20721 ||
                               ticket.ID_CATE == 20723)
                            {
                                //message.Bcc.Add("mesadeayuda@minsur.com");
                                //var qAnalitica = dbc.ACCOUNT_PARAMETER
                                //.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 1062 && x.VAL_ACCO_PARA == Convert.ToString(ticket.ID_CATE)).ToList().Count();
                                //if (qAnalitica > 0)
                                //{
                                message.Subject = "INCIDENCIA ANALITICA";
                                Console.WriteLine("Envío de correo INCIDENCIA ANALITICA - MINSUR");
                                //}
                                //else {
                                //    message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                                //}
                            }
                            else
                            {
                                message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                            }
                        }
                        else
                        {
                            message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                        }
                        if (ID_ACCO >= 60 || ID_ACCO <= 74)
                        {
                            message.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                        }
                        newMail.smtp.Send(message);

                        // Se agrega validación para que no envíe correos repetidos
                        var uticket = dbc.TICKETs.Find(ID_TICK);
                        uticket.SEND_MAIL = true;
                        uticket.CREATED_MAIL = DateTime.Now;
                        dbc.TICKETs.Attach(uticket);
                        dbc.Entry(uticket).State = EntityState.Modified;
                        dbc.SaveChanges();

                        Console.WriteLine("To: " + mailTo);
                        Console.WriteLine("CC(Mail_Coor): " + mail_coor);
                        Console.WriteLine("CC(Mail_Group): " + mail_group);
                        Console.WriteLine("Se envió correo de la creación del Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");

                        //Solicitud de goldfields
                        try
                        {
                            if ((ticket.ID_ACCO.Value == ASM || ticket.ID_ACCO == 25) && ticket.ID_STAT_END.Value == 4 && mailCliente != "" && caseSwitch == 4)
                            {
                                SendMail newMailGF = new SendMail();
                                var messageGF = newMailGF.mailMessage;
                                messageGF.To.Add(mailCliente);
                                //Solicitado por el cliente
                                if (ticket.ID_PRIO <= 2)
                                {
                                    message.CC.Add("Liliana.chavez@goldfields.com,Victor.Delucchi@goldfields.com");
                                    //message.CC.Add("edeza@electrodata.com.pe,evalverde@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                                    //message.CC.Add("edeza@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                                    message.CC.Add("cfreyre@electrodata.com.pe,destraver@electrodata.com.pe");
                                }
                                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 41457) // Usuario Snode Guardian
                                {
                                    body_html = body.CreateTicketGoldFieldsIngles(ID_TICK, 1);
                                }
                                else
                                {
                                    body_html = body.CreateTicketGoldFields(ID_TICK, 1);
                                }

                                htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                                messageGF.AlternateViews.Add(htmlView);
                                messageGF.Subject = Subjet1 + Convert.ToString(ticket.COD_TICK);
                                newMailGF.smtp.Send(messageGF);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error al enviar al USUARIO el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                            Console.WriteLine("Error generado: " + ex.Message.ToString());
                        }

                        return "OK";
                    }
                    else
                    {
                        Console.WriteLine("Error: No se pudo generar el Template del Ticket código: " + ticket.COD_TICK);
                        return "ERROR";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al enviar el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");

                    Console.WriteLine("Error generado: " + ex.Message.ToString());

                    return "ERROR";
                }
            }
            catch (Exception exx)
            {
                EXCEPTION exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = exx.Message.ToString();
                exc.DES_EXCE = "Send Mail - New Ticket";
                dbl.EXCEPTIONs.Add(exc);
                dbl.SaveChanges();

                Console.WriteLine("Hubo un error al enviar mensaje de creación de Ticket");

                Console.WriteLine("Error generado: " + exx.Message.ToString());

                return "ERROR";
            }
        }

        public string SendMailTicketBuenaventura(int ID_TICK)
        {
            try
            {
                string from = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                string pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                Validate vmail = new Validate();
                int estado = Convert.ToInt32(ticket.ID_STAT);
                if (estado == 1 || estado == 3)
                {
                    //ASIGNADO
                    var qEma = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);
                    //USUARIO AFECTADO
                    var qEma1 = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END);

                    string correoUsuarioAfectado = null, correoUsuarioAsignado = null;

                    try
                    {
                        correoUsuarioAfectado = qEma1.EMA_PERS;
                        if (correoUsuarioAfectado == "" || correoUsuarioAfectado == " ")
                        {
                            Console.WriteLine("Error al enviar el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                            Console.WriteLine("Usuario Afectado no tiene correo");
                            return "ERROR";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error al enviar el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                        Console.WriteLine("Usuario Afectado no tiene correo");
                        return "ERROR";
                    }
                    try
                    {
                        correoUsuarioAsignado = qEma.EMA_ELEC;
                        if (correoUsuarioAsignado == "" || correoUsuarioAsignado == " ")
                        {
                            Console.WriteLine("Error al enviar el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                            Console.WriteLine("Usuario Afectado no tiene correo");
                            return "ERROR";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error al enviar el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                        Console.WriteLine("Usuario Asignado no tiene correo");
                        return "ERROR";
                    }

                    //Correo de escalamiento IT BV
                    if (estado == 1 && ticket.ID_QUEU == 106)
                    {
                        string destinatarioCorreo = dbc.ACCOUNT_PARAMETER
                            .Single(x => x.ID_ACCO == 60 && x.ID_PARA == 1177 && x.VIG_ACCO_PARA == true)
                            .VAL_ACCO_PARA
                            .ToString();
                        string copiaCorreo = "";
                        try
                        {
                            //Especialista Asignado
                            switch (ticket.ID_PERS_ENTI_ASSI)
                            {
                                case 80922:
                                    copiaCorreo = dbc.ACCOUNT_PARAMETER
                                        .Single(x => x.ID_ACCO == 60 && x.ID_PARA == 1173 && x.VIG_ACCO_PARA == true)
                                        .VAL_ACCO_PARA
                                        .ToString();
                                    break;
                                case 80923:
                                    copiaCorreo = dbc.ACCOUNT_PARAMETER
                                        .Single(x => x.ID_ACCO == 60 && x.ID_PARA == 1174 && x.VIG_ACCO_PARA == true)
                                        .VAL_ACCO_PARA
                                        .ToString();
                                    break;
                                case 80924:
                                    copiaCorreo = dbc.ACCOUNT_PARAMETER
                                        .Single(x => x.ID_ACCO == 60 && x.ID_PARA == 1175 && x.VIG_ACCO_PARA == true)
                                        .VAL_ACCO_PARA
                                        .ToString();
                                    break;
                                case 80925:
                                    copiaCorreo = dbc.ACCOUNT_PARAMETER
                                        .Single(x => x.ID_ACCO == 60 && x.ID_PARA == 1176 && x.VIG_ACCO_PARA == true)
                                        .VAL_ACCO_PARA
                                        .ToString();
                                    break;
                            }

                            //Envio de Correo
                            try
                            {
                                TicketTemplate body = new TicketTemplate();
                                MailMessage message = new MailMessage();
                                string body_htmlEscalamiento = body.CrearEscalamientoTicketBuenaventura(ticket.SUM_TICK);

                                if (body_htmlEscalamiento != null)
                                {
                                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_htmlEscalamiento, Encoding.UTF8, MediaTypeNames.Text.Html);
                                    message.AlternateViews.Add(htmlView);
                                    message.From = new MailAddress(from);
                                    message.To.Add(destinatarioCorreo);
                                    message.CC.Add(copiaCorreo);
                                    message.Subject = "Ticket " + Convert.ToString(ticket.COD_TICK) + " - Escalamiento TI Buenaventura";
                                    message.Body = body_htmlEscalamiento;
                                    message.AlternateViews.Add(htmlView);

                                    SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                                    oSmtpClient.EnableSsl = true;
                                    oSmtpClient.UseDefaultCredentials = false;
                                    oSmtpClient.Port = 587;

                                    oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                                    oSmtpClient.Send(message);
                                    oSmtpClient.Dispose();
                                }
                                else
                                {
                                    Console.WriteLine("Error: No se pudo generar el Template del escalamiento del Ticket código: " + ticket.COD_TICK);
                                }

                                Console.WriteLine("Se envió correo del escalamiento del Ticket Buenaventura código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error al enviar el escalamiento del Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                                Console.WriteLine("Error generado: " + ex.Message.ToString());

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error al generar el correo de Escalamiento BV en el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                            Console.WriteLine("Error generado: " + ex.Message.ToString());
                        }
                    }

                    try
                    {
                        TicketTemplate body = new TicketTemplate();
                        string body_htmlCrear = "";
                        string body_htmlAsignar = "";
                        MailMessage message = new MailMessage();
                        MailMessage mensajeAsignado = new MailMessage();

                        body_htmlCrear = body.CrearTicketBuenaventura(ID_TICK);
                        body_htmlAsignar = body.AsignarTicketBuenaventura(ID_TICK);
                        if (body_htmlCrear != null)
                        {
                            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_htmlCrear, Encoding.UTF8, MediaTypeNames.Text.Html);
                            message.AlternateViews.Add(htmlView);
                            MailMessage mailMessage = new MailMessage();
                            message.From = new MailAddress(from);
                            message.Subject = "Ticket " + Convert.ToString(ticket.COD_TICK) + " ha sido Creado";
                            message.Body = body_htmlCrear;
                            message.To.Add(correoUsuarioAfectado);
                            message.AlternateViews.Add(htmlView);
                            SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                            oSmtpClient.EnableSsl = true;
                            oSmtpClient.UseDefaultCredentials = false;
                            oSmtpClient.Port = 587;

                            oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                            oSmtpClient.Send(message);
                            oSmtpClient.Dispose();
                        }
                        else
                        {
                            Console.WriteLine("Error: No se pudo generar el Template del Ticket(Creado) código: " + ticket.COD_TICK);
                        }

                        if (body_htmlAsignar != null)
                        {
                            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_htmlAsignar, Encoding.UTF8, MediaTypeNames.Text.Html);
                            mensajeAsignado.AlternateViews.Add(htmlView);
                            MailMessage mailMessage = new MailMessage();
                            mensajeAsignado.From = new MailAddress(from);
                            mensajeAsignado.Subject = "Ticket " + Convert.ToString(ticket.COD_TICK) + " ha sido Asignado";
                            mensajeAsignado.Body = body_htmlAsignar;
                            mensajeAsignado.To.Add(correoUsuarioAsignado);
                            mensajeAsignado.AlternateViews.Add(htmlView);
                            SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                            oSmtpClient.EnableSsl = true;
                            oSmtpClient.UseDefaultCredentials = false;
                            oSmtpClient.Port = 587;

                            oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                            oSmtpClient.Send(mensajeAsignado);
                            oSmtpClient.Dispose();
                        }
                        else
                        {
                            Console.WriteLine("Error: No se pudo generar el Template del Ticket(Asignado) código: " + ticket.COD_TICK);
                        }

                        Console.WriteLine("Usuario afectado: " + correoUsuarioAfectado);
                        Console.WriteLine("Usuario asignado: " + correoUsuarioAsignado);
                        Console.WriteLine("Se envió correo de la creación del Ticket Buenaventura código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al enviar el Ticket código: " + ticket.COD_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");
                        Console.WriteLine("Error generado: " + ex.Message.ToString());

                    }
                }
                // Se agrega validación para que no envíe correos repetidos
                var uticket = dbc.TICKETs.Find(ID_TICK);
                uticket.SEND_MAIL = true;
                uticket.CREATED_MAIL = DateTime.Now;
                dbc.TICKETs.Attach(uticket);
                dbc.Entry(uticket).State = EntityState.Modified;
                dbc.SaveChanges();

                return "OK";
            }
            catch (Exception exx)
            {
                EXCEPTION exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = exx.Message.ToString();
                exc.DES_EXCE = "Send Mail - New Ticket";
                dbl.EXCEPTIONs.Add(exc);
                dbl.SaveChanges();

                Console.WriteLine("Hubo un error al enviar mensaje de creación de Ticket");

                Console.WriteLine("Error generado: " + exx.Message.ToString());

                return "ERROR";
            }
        }

        public string SendMailTicketChat(int ID_TICK)
        {
            try
            {

                TicketTemplate body = new TicketTemplate();
                string body_html = "";

                //copiando al grupo si el ticket es por una OP


                try
                {
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    message.To.Add("servicedesk@electrodata.com.pe");
                    //message.CC.Add("rdiaz@agpglass.com,consultorexterno1@agpglass.com");

                    body_html = body.CreateTicketChatITMS(ID_TICK);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                    message.AlternateViews.Add(htmlView);
                    message.Subject = "Se creó un ticket a travéz del Chat ITMSITO";
                    newMail.smtp.Send(message);

                    // Se agrega validación para que no envíe correos repetidos
                    var uticket = dbc.TICKETs.Find(ID_TICK);
                    uticket.SEND_MAIL = true;
                    uticket.CREATED_MAIL = DateTime.Now;
                    dbc.TICKETs.Attach(uticket);
                    dbc.Entry(uticket).State = EntityState.Modified;
                    dbc.SaveChanges();

                    Console.WriteLine("Se envió correo de la creación del Ticket código: " + ID_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");

                    return "OK";



                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al enviar el Ticket código: " + ID_TICK + " a las " + DateTime.Now.ToString("HH:mm") + " horas");

                    Console.WriteLine("Error generado: " + ex.Message.ToString());

                    return "ERROR";
                }
            }
            catch (Exception exx)
            {
                EXCEPTION exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = exx.Message.ToString();
                exc.DES_EXCE = "Send Mail - New Ticket";
                dbl.EXCEPTIONs.Add(exc);
                dbl.SaveChanges();

                Console.WriteLine("Hubo un error al enviar mensaje de creación de Ticket");

                Console.WriteLine("Error generado: " + exx.Message.ToString());

                return "ERROR";

            }
        }



        //Creación de contenido y envío de correo en actualización de estado.
        public string SendMailUpdateStatus(int ID_DETA_TICK)
        {
            ACCOUNT_PARAMETER coordinator = null;
            int IdCoor = 0;
            int ASM = 1;
            string mailCliente = "";
            Validate vmail = new Validate();

            try
            {
                var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);

                int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                //Implementación de copia de correo a usuario solicitante.
                if ((ticket.ID_ACCO.Value == ASM || ticket.ID_ACCO.Value == 25) && deta_tick.ID_STAT.Value == 4)
                {
                    try
                    {

                        PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);
                        if (vmail.email_bien_escrito(pe.EMA_PERS) && pe.ID_TYPE_CLIE.Value == 2)
                        {
                            //Descomentar Aqui para que envíen correos electrónicos al cliente de GoldFields.
                            mailCliente = pe.EMA_PERS;
                        }
                        else
                        {
                            Console.WriteLine("Revisar correo para usuario solicitante ticket: " + ticket.COD_TICK);
                        }
                    }
                    catch
                    {

                    }

                }

                //PARA EL CASO QUE TENGA CUENTA DE CORREO EN OUTSOURSING
                var mailCLient = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 32)
                    .Where(x => x.ID_ACCO == ticket.ID_ACCO.Value).Count();

                try
                {
                    //COORDINADOR GENERAL DE LA CUENTA
                    coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                            .Single(x => x.ID_ACCO == ticket.ID_ACCO);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Details_Ticket => Tabla Account_Parameter ID_PARA=30 (SIN SUPERVISOR DE LA CUENTA) para el ID_ACCO: " + ticket.ID_ACCO);
                    Console.WriteLine("Detalle generado: " + e.Message.ToString());
                    coordinator = null;
                }

                PERSON_ENTITY per_mail_coor = null;

                if (coordinator != null)
                {
                    IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);


                    try
                    {
                        per_mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor);
                    }
                    catch
                    {
                        per_mail_coor = null;
                    }
                }
                else
                {
                    per_mail_coor = null;
                }


                string mail_coor = null;
                if (per_mail_coor != null)
                {
                    //Verificar mail del cliente del coordinador
                    if (mailCLient > 0)
                    {
                        mail_coor = per_mail_coor.EMA_ELEC + (String.IsNullOrEmpty(per_mail_coor.EMA_PERS) ? "" : "," + per_mail_coor.EMA_PERS);
                    }
                    else
                    {
                        mail_coor = per_mail_coor.EMA_ELEC;
                    }
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
                catch (Exception e)
                {
                    Console.WriteLine("Details_Ticket -> No se encontro Supervisor de Queue");
                    Console.WriteLine("Detalle generado: " + e.Message.ToString());
                }

                //Fin Supervisor Queue

                //Responsable del Ticket
                var Resp = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI.Value);

                //Usuario que actualiza el ticket
                var User = dbe.CLASS_ENTITY.Single(x => x.UserId == deta_tick.UserId);
                int ID_ENTI = Convert.ToInt32(User.ID_ENTI);

                var UserUpdate = dbe.PERSON_ENTITY.Single(x => x.ID_ENTI2 == ID_ENTI && x.VIG_PERS_ENTI == true);

                string mailResp = null;

                // se copia mail a correo del cliente :: responsable del ticket
                if (mailCLient > 0)
                {
                    //No envía correos de notificación a José Ishikawa, Liliana Chávez y Victor Deluchi
                    if (ticket.ID_PERS_ENTI_ASSI == 38825 || ticket.ID_PERS_ENTI_ASSI == 553 || ticket.ID_PERS_ENTI_ASSI == 43602)
                    {
                        mailResp = "itms@electrodata.com.pe";
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
                        {
                            mailResp = Resp.EMA_ELEC;
                        }
                        if (String.IsNullOrEmpty(mailResp))
                        {
                            mailResp = "itms@electrodata.com.pe";
                        }
                        if (!String.IsNullOrEmpty(Resp.EMA_PERS))
                        {
                            mailResp = mailResp + "," + Resp.EMA_PERS;
                        }
                    }

                    if (Resp.ID_PERS_ENTI != UserUpdate.ID_PERS_ENTI)
                    {
                        mailResp = mailResp + (String.IsNullOrEmpty(UserUpdate.EMA_ELEC) ? "" : "," + UserUpdate.EMA_ELEC)
                            + (String.IsNullOrEmpty(UserUpdate.EMA_PERS) ? "" : "," + UserUpdate.EMA_PERS);
                    }

                    if (ticket.ID_ACCO == 31)//El toro enviar correo a usuario solciitante
                    {
                        var correoCliente = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value).EMA_PERS;
                        if (!String.IsNullOrEmpty(correoCliente))
                        {
                            mailResp = mailResp + "," + correoCliente;
                        }
                    }
                }
                else
                {
                    //No envía correos de notificación a José Ishikawa, Liliana Chávez y Victor Deluchi
                    if (ticket.ID_PERS_ENTI_ASSI == 38825 || ticket.ID_PERS_ENTI_ASSI == 553 || ticket.ID_PERS_ENTI_ASSI == 43602)
                    {
                        mailResp = "itms@electrodata.com.pe";
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
                        {
                            mailResp = Resp.EMA_ELEC;
                        }
                        if (String.IsNullOrEmpty(mailResp))
                        {
                            mailResp = "itms@electrodata.com.pe";
                        }
                    }
                    if (Resp.ID_PERS_ENTI != UserUpdate.ID_PERS_ENTI)
                    {
                        mailResp = mailResp + (String.IsNullOrEmpty(UserUpdate.EMA_ELEC) ? "" : "," + UserUpdate.EMA_ELEC);
                    }
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
                try
                {
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;

                    TicketTemplate body = new TicketTemplate();

                    //Solicitud de Goldfields
                    string body_html = "";

                    if (ticket.ID_ACCO == 1 || ticket.ID_ACCO == 25)
                    {
                        //message.Bcc.Add("evalverde@electrodata.com.pe,edeza@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                        //message.Bcc.Add("edeza@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                        message.Bcc.Add("destraver@electrodata.com.pe,dperez@electrodata.com.pe");

                        // Ticket con categoría Gestión De Servicios-Gestión De Seguridad Informática-AMP
                        var categoria = dbc.CATEGORies.Single(x => x.ID_CATE == ticket.ID_CATE);
                        if (categoria.ID_CATE_PARE == 15302 && (deta_tick.ID_STAT == 1 || deta_tick.ID_STAT == 3 || deta_tick.ID_STAT == 4)) // Estado Activo y Resuelto
                        {
                            message.CC.Add("ingenieria@mcinnovatec.com,CyberSOC@mcinnovatec.com,Jose.Ishikawa@goldfields.com,juanfrancisco.burgacolchado@telefonica.com");
                            message.CC.Add("jorgeluis.saldanatorres@telefonica.com,roberto.tupacyupanqui@telefonica.com,qos@mcinnovatec.com,josero.manta@telefonica.com");
                        }

                        if (deta_tick.ID_STAT == 4)
                        {
                            if (ticket.ID_PRIO <= 2)
                            {
                                message.CC.Add("Liliana.chavez@goldfields.com,Victor.Delucchi@goldfields.com");//Jose.Ishikawa@goldfields.com
                                // message.CC.Add("edeza@electrodata.com.pe,evalverde@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                                //message.CC.Add("edeza@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                                message.CC.Add("cfreyre@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                            }
                            // Adjuntos como evidencia
                            if (categoria.ID_CATE_PARE == 15302) // Ticket con categoría Gestión De Servicios-Gestión De Seguridad Informática-AMP
                            {
                                var listaAdjuntos = dbc.ATTACHEDs.Where(x => x.ID_DETA_INCI == deta_tick.ID_DETA_TICK);

                                foreach (ATTACHED adjunto in listaAdjuntos)
                                {
                                    string file = @"E:\ITMS\Adjuntos\" + adjunto.NAM_ATTA + "_" + adjunto.ID_ATTA.ToString() + adjunto.EXT_ATTA;
                                    Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                                    message.Attachments.Add(attachment);
                                }
                            }

                            if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 41457 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 45674 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 51898) // Usuario Snode Guardian
                            {
                                body_html = body.CreateTicketGoldFieldsIngles(ID_TICK, 4);

                                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 41457)
                                {
                                    message.CC.Add("Guardian@snode.com");
                                }
                                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 45674)
                                {
                                    message.CC.Add("vulscans@snode.com");
                                }
                                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 51898)
                                {
                                    message.CC.Add("Guardian@snode.com,vulscans@snode.com");
                                }

                                var listaAdjuntos = dbc.ATTACHEDs.Where(x => x.ID_DETA_INCI == deta_tick.ID_DETA_TICK);

                                foreach (ATTACHED adjunto in listaAdjuntos)
                                {
                                    string file = @"E:\ITMS\Adjuntos\" + adjunto.NAM_ATTA + "_" + adjunto.ID_ATTA.ToString() + adjunto.EXT_ATTA;
                                    Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                                    message.Attachments.Add(attachment);
                                }
                            }
                            else
                            {
                                body_html = body.CreateTicketGoldFields(ID_TICK, 4);
                            }

                        }
                    }
                    else if (ticket.ID_ACCO == 20 && deta_tick.ID_STAT == 4)
                    {
                        //message.CC.Add("publicom@electrodata.com.pe,jquisper@electrodata.com.pe");
                        mailResp = "publicom@electrodata.com.pe";
                        body_html = body.CreateTicketPublicom(ID_TICK);
                    }
                    else
                    {
                        if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 44547 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 47441)//BOC Francis Rodriguez
                        {
                            message.CC.Add("francis.rodriguez@pe.bocusa.com");
                            body_html = body.CreateTicketBOC(ID_TICK, 4);
                        }
                        else
                        {
                            body_html = body.CreateDetailsTicket(ID_DETA_TICK);
                        }
                    }

                    if (ticket.ID_ACCO == 31 && deta_tick.ID_STAT == 4)
                    {
                        body_html = body.CreateTicketToro(ID_TICK);
                    }

                    //Encuesta de Satisfacción Portal Usuario
                    if (deta_tick.ID_STAT == 4 && ticket.ID_SOUR == 5)
                    {
                        PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);
                        if (vmail.email_bien_escrito(pe.EMA_PERS))
                        {
                            //string correoUsuarioAfectado = pe.EMA_PERS;

                            //var mensaje = newMail.mailMessage;
                            //string encuesta_html = body.EncuestaPortalUsuario(ID_DETA_TICK);
                            //AlternateView htmlVista = AlternateView.CreateAlternateViewFromString(encuesta_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                            //mensaje.To.Add(correoUsuarioAfectado);
                            //mensaje.AlternateViews.Add(htmlVista);
                            //mensaje.Subject = "Encuesta de Satisfacción - Ticket " + code_ticket + " / Satisfaction survey - Ticket " + code_ticket;
                            //newMail.smtp.Send(mensaje);

                            //var objEncuesta = new EncuestaPortalUsuario();
                            //objEncuesta.IdTicket = ID_TICK;
                            //objEncuesta.IdDetalleTicket = ID_DETA_TICK;
                            //objEncuesta.IdEstadoEncuesta = 1;
                            //objEncuesta.FechaEnvio = DateTime.Now;
                            //objEncuesta.Estado = true;
                            //dbc.EncuestaPortalUsuarios.Add(objEncuesta);
                            //dbc.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("Revisar correo para usuario solicitante - Portal Usuario ticket: " + ticket.COD_TICK);
                        }

                        //Insert Tabla Encuesta
                    }


                    if (body_html != null)
                    {
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                        string msgcopy = mail_coor + (String.IsNullOrEmpty(mail_sup_queu) ? "" : "," + mail_sup_queu) + (String.IsNullOrEmpty(mailCliente) ? "" : "," + mailCliente);
                        message.To.Add(mailResp);
                        msgcopy = msgcopy.Replace(",,", ",");
                        message.CC.Add(msgcopy);
                        message.AlternateViews.Add(htmlView);
                        message.Subject = "Update Status - " + status + " " + code_ticket + " / Actualización de Estado - " + estado + " " + code_ticket;
                        newMail.smtp.Send(message);

                        //Despues de enviar el correo actualizamos el flag de envio de correo
                        var udticket = dbc.DETAILS_TICKET.Find(ID_DETA_TICK);
                        udticket.SEND_MAIL = true;
                        udticket.CREATED_MAIL = DateTime.Now;
                        dbc.DETAILS_TICKET.Attach(udticket);
                        dbc.Entry(udticket).State = EntityState.Modified;
                        dbc.SaveChanges();

                        Console.WriteLine("To " + mailResp);
                        Console.WriteLine("CC " + msgcopy);
                        Console.WriteLine("Se envió correo del Details_Ticket ID: " + ID_DETA_TICK);

                        return "OK";
                    }
                    else
                    {
                        Console.WriteLine("Error: No se pudo generar el Template en Details_Ticket ID: " + ID_DETA_TICK);
                        return "ERROR";

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error enviando correo del Details_Ticket ID: " + ID_DETA_TICK);
                    Console.WriteLine("Error generado: " + ex.Message.ToString());
                    return "ERROR";
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine("Error generando datos para el correo (Update) del Details_Ticket ID: " + ID_DETA_TICK);
                Console.WriteLine("Error generado: " + exx.Message.ToString());
                return "ERROR";
            }
        }

        public string SendMailUpdateStatusMinsur(int ID_DETA_TICK)
        {
            ACCOUNT_PARAMETER coordinator = null;
            int IdCoor = 0;
            int ASM = 1;
            string mailCliente = "";
            Validate vmail = new Validate();

            try
            {
                var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);

                int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                //Implementación de copia de correo a usuario solicitante.
                if ((ticket.ID_ACCO.Value == ASM || ticket.ID_ACCO.Value == 25) && deta_tick.ID_STAT.Value == 4)
                {
                    try
                    {

                        PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);
                        if (vmail.email_bien_escrito(pe.EMA_PERS) && pe.ID_TYPE_CLIE.Value == 2)
                        {
                            //Descomentar Aqui para que envíen correos electrónicos al cliente de GoldFields.
                            mailCliente = pe.EMA_PERS;
                        }
                        else
                        {
                            Console.WriteLine("Revisar correo para usuario solicitante ticket: " + ticket.COD_TICK);
                        }
                    }
                    catch
                    {

                    }

                }

                //PARA EL CASO QUE TENGA CUENTA DE CORREO EN OUTSOURSING
                var mailCLient = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 32)
                    .Where(x => x.ID_ACCO == ticket.ID_ACCO.Value).Count();

                try
                {
                    //COORDINADOR GENERAL DE LA CUENTA
                    coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                            .Single(x => x.ID_ACCO == ticket.ID_ACCO);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Details_Ticket => Tabla Account_Parameter ID_PARA=30 (SIN SUPERVISOR DE LA CUENTA) para el ID_ACCO: " + ticket.ID_ACCO);
                    Console.WriteLine("Detalle generado: " + e.Message.ToString());
                    coordinator = null;
                }

                PERSON_ENTITY per_mail_coor = null;

                if (coordinator != null)
                {
                    IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);


                    try
                    {
                        per_mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor);
                    }
                    catch
                    {
                        per_mail_coor = null;
                    }
                }
                else
                {
                    per_mail_coor = null;
                }


                string mail_coor = null;
                if (per_mail_coor != null)
                {
                    //Verificar mail del cliente del coordinador
                    if (mailCLient > 0)
                    {
                        mail_coor = per_mail_coor.EMA_ELEC + (String.IsNullOrEmpty(per_mail_coor.EMA_PERS) ? "" : "," + per_mail_coor.EMA_PERS);
                    }
                    else
                    {
                        mail_coor = per_mail_coor.EMA_ELEC;
                    }
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
                catch (Exception e)
                {
                    Console.WriteLine("Details_Ticket -> No se encontro Supervisor de Queue");
                    Console.WriteLine("Detalle generado: " + e.Message.ToString());
                }

                //Fin Supervisor Queue

                //Responsable del Ticket
                var Resp = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI.Value);

                //Usuario que actualiza el ticket
                var User = dbe.CLASS_ENTITY.Single(x => x.UserId == deta_tick.UserId);
                int ID_ENTI = Convert.ToInt32(User.ID_ENTI);

                var UserUpdate = dbe.PERSON_ENTITY.Single(x => x.ID_ENTI2 == ID_ENTI && x.VIG_PERS_ENTI == true);

                string mailResp = null;

                // se copia mail a correo del cliente :: responsable del ticket
                if (mailCLient > 0)
                {
                    //No envía correos de notificación a José Ishikawa, Liliana Chávez y Victor Deluchi
                    if (ticket.ID_PERS_ENTI_ASSI == 38825 || ticket.ID_PERS_ENTI_ASSI == 553 || ticket.ID_PERS_ENTI_ASSI == 43602)
                    {
                        mailResp = "itms@electrodata.com.pe";
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
                        {
                            mailResp = Resp.EMA_ELEC;
                        }
                        if (String.IsNullOrEmpty(mailResp))
                        {
                            mailResp = "itms@electrodata.com.pe";
                        }
                        if (!String.IsNullOrEmpty(Resp.EMA_PERS))
                        {
                            mailResp = mailResp + "," + Resp.EMA_PERS;
                        }
                    }

                    if (Resp.ID_PERS_ENTI != UserUpdate.ID_PERS_ENTI)
                    {
                        mailResp = mailResp + (String.IsNullOrEmpty(UserUpdate.EMA_ELEC) ? "" : "," + UserUpdate.EMA_ELEC)
                            + (String.IsNullOrEmpty(UserUpdate.EMA_PERS) ? "" : "," + UserUpdate.EMA_PERS);
                    }

                    if (ticket.ID_ACCO == 31)//El toro enviar correo a usuario solciitante
                    {
                        var correoCliente = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value).EMA_PERS;
                        if (!String.IsNullOrEmpty(correoCliente))
                        {
                            mailResp = mailResp + "," + correoCliente;
                        }
                    }
                }
                else
                {
                    //No envía correos de notificación a José Ishikawa, Liliana Chávez y Victor Deluchi
                    if (ticket.ID_PERS_ENTI_ASSI == 38825 || ticket.ID_PERS_ENTI_ASSI == 553 || ticket.ID_PERS_ENTI_ASSI == 43602)
                    {
                        mailResp = "itms@electrodata.com.pe";
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
                        {
                            mailResp = Resp.EMA_ELEC;
                        }
                        if (String.IsNullOrEmpty(mailResp))
                        {
                            mailResp = "itms@electrodata.com.pe";
                        }
                    }
                    if (Resp.ID_PERS_ENTI != UserUpdate.ID_PERS_ENTI)
                    {
                        mailResp = mailResp + (String.IsNullOrEmpty(UserUpdate.EMA_ELEC) ? "" : "," + UserUpdate.EMA_ELEC);
                    }
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
                try
                {
                    SendMailMinsur newMail = new SendMailMinsur();
                    var message = newMail.mailMessage;

                    TicketTemplate body = new TicketTemplate();

                    //Solicitud de Goldfields
                    string body_html = "";

                    if (ticket.ID_ACCO == 1 || ticket.ID_ACCO == 25)
                    {
                        //message.Bcc.Add("evalverde@electrodata.com.pe,edeza@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                        // message.Bcc.Add("edeza@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                        message.Bcc.Add("destraver@electrodata.com.pe,dperez@electrodata.com.pe");

                        // Ticket con categoría Gestión De Servicios-Gestión De Seguridad Informática-AMP
                        var categoria = dbc.CATEGORies.Single(x => x.ID_CATE == ticket.ID_CATE);
                        if (categoria.ID_CATE_PARE == 15302 && (deta_tick.ID_STAT == 1 || deta_tick.ID_STAT == 3 || deta_tick.ID_STAT == 4)) // Estado Activo y Resuelto
                        {
                            message.CC.Add("ingenieria@mcinnovatec.com,CyberSOC@mcinnovatec.com,Jose.Ishikawa@goldfields.com,juanfrancisco.burgacolchado@telefonica.com");
                            message.CC.Add("jorgeluis.saldanatorres@telefonica.com,roberto.tupacyupanqui@telefonica.com,qos@mcinnovatec.com,josero.manta@telefonica.com");
                        }

                        if (deta_tick.ID_STAT == 4)
                        {
                            if (ticket.ID_PRIO <= 2)
                            {
                                message.CC.Add("Liliana.chavez@goldfields.com,Victor.Delucchi@goldfields.com");//Jose.Ishikawa@goldfields.com
                                //message.CC.Add("edeza@electrodata.com.pe,evalverde@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                                //message.CC.Add("edeza@electrodata.com.pe,cfreyre@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                                message.CC.Add("cfreyre@electrodata.com.pe,destraver@electrodata.com.pe,dperez@electrodata.com.pe");
                            }
                            // Adjuntos como evidencia
                            if (categoria.ID_CATE_PARE == 15302) // Ticket con categoría Gestión De Servicios-Gestión De Seguridad Informática-AMP
                            {
                                var listaAdjuntos = dbc.ATTACHEDs.Where(x => x.ID_DETA_INCI == deta_tick.ID_DETA_TICK);

                                foreach (ATTACHED adjunto in listaAdjuntos)
                                {
                                    string file = @"E:\ITMS\Adjuntos\" + adjunto.NAM_ATTA + "_" + adjunto.ID_ATTA.ToString() + adjunto.EXT_ATTA;
                                    Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                                    message.Attachments.Add(attachment);
                                }
                            }

                            if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 41457 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 45674 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 51898) // Usuario Snode Guardian
                            {
                                body_html = body.CreateTicketGoldFieldsIngles(ID_TICK, 4);

                                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 41457)
                                {
                                    message.CC.Add("Guardian@snode.com");
                                }
                                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 45674)
                                {
                                    message.CC.Add("vulscans@snode.com");
                                }
                                if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 51898)
                                {
                                    message.CC.Add("Guardian@snode.com,vulscans@snode.com");
                                }

                                var listaAdjuntos = dbc.ATTACHEDs.Where(x => x.ID_DETA_INCI == deta_tick.ID_DETA_TICK);

                                foreach (ATTACHED adjunto in listaAdjuntos)
                                {
                                    string file = @"E:\ITMS\Adjuntos\" + adjunto.NAM_ATTA + "_" + adjunto.ID_ATTA.ToString() + adjunto.EXT_ATTA;
                                    Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                                    message.Attachments.Add(attachment);
                                }
                            }
                            else
                            {
                                body_html = body.CreateTicketGoldFields(ID_TICK, 4);
                            }

                        }
                    }
                    else if (ticket.ID_ACCO == 20 && deta_tick.ID_STAT == 4)
                    {
                        //message.CC.Add("publicom@electrodata.com.pe,jquisper@electrodata.com.pe");
                        mailResp = "publicom@electrodata.com.pe";
                        body_html = body.CreateTicketPublicom(ID_TICK);
                    }
                    else
                    {
                        if (Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 44547 || Convert.ToInt32(ticket.ID_PERS_ENTI_END) == 47441)//BOC Francis Rodriguez
                        {
                            message.CC.Add("francis.rodriguez@pe.bocusa.com");
                            body_html = body.CreateTicketBOC(ID_TICK, 4);
                        }
                        else
                        {
                            body_html = body.CreateDetailsTicket(ID_DETA_TICK);
                        }
                    }

                    if (ticket.ID_ACCO == 31 && deta_tick.ID_STAT == 4)
                    {
                        body_html = body.CreateTicketToro(ID_TICK);
                    }

                    //Encuesta de Satisfacción Portal Usuario
                    if (deta_tick.ID_STAT == 4 && ticket.ID_SOUR == 5)
                    {
                        PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);
                        if (vmail.email_bien_escrito(pe.EMA_PERS))
                        {
                            //string correoUsuarioAfectado = pe.EMA_PERS;

                            //var mensaje = newMail.mailMessage;
                            //string encuesta_html = body.EncuestaPortalUsuario(ID_DETA_TICK);
                            //AlternateView htmlVista = AlternateView.CreateAlternateViewFromString(encuesta_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                            //mensaje.To.Add(correoUsuarioAfectado);
                            //mensaje.AlternateViews.Add(htmlVista);
                            //mensaje.Subject = "Encuesta de Satisfacción - Ticket " + code_ticket + " / Satisfaction survey - Ticket " + code_ticket;
                            //newMail.smtp.Send(mensaje);

                            //var objEncuesta = new EncuestaPortalUsuario();
                            //objEncuesta.IdTicket = ID_TICK;
                            //objEncuesta.IdDetalleTicket = ID_DETA_TICK;
                            //objEncuesta.IdEstadoEncuesta = 1;
                            //objEncuesta.FechaEnvio = DateTime.Now;
                            //objEncuesta.Estado = true;
                            //dbc.EncuestaPortalUsuarios.Add(objEncuesta);
                            //dbc.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("Revisar correo para usuario solicitante - Portal Usuario ticket: " + ticket.COD_TICK);
                        }

                        //Insert Tabla Encuesta
                    }


                    if (body_html != null)
                    {
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                        string msgcopy = mail_coor + (String.IsNullOrEmpty(mail_sup_queu) ? "" : "," + mail_sup_queu) + (String.IsNullOrEmpty(mailCliente) ? "" : "," + mailCliente);
                        message.To.Add(mailResp);
                        msgcopy = msgcopy.Replace(",,", ",");
                        message.CC.Add(msgcopy);
                        message.AlternateViews.Add(htmlView);
                        message.Subject = "Update Status - " + status + " " + code_ticket + " / Actualización de Estado - " + estado + " " + code_ticket;
                        newMail.smtp.Send(message);

                        //Despues de enviar el correo actualizamos el flag de envio de correo
                        var udticket = dbc.DETAILS_TICKET.Find(ID_DETA_TICK);
                        udticket.SEND_MAIL = true;
                        udticket.CREATED_MAIL = DateTime.Now;
                        dbc.DETAILS_TICKET.Attach(udticket);
                        dbc.Entry(udticket).State = EntityState.Modified;
                        dbc.SaveChanges();

                        Console.WriteLine("To " + mailResp);
                        Console.WriteLine("CC " + msgcopy);
                        Console.WriteLine("Se envió correo del Details_Ticket ID: " + ID_DETA_TICK);

                        return "OK";
                    }
                    else
                    {
                        Console.WriteLine("Error: No se pudo generar el Template en Details_Ticket ID: " + ID_DETA_TICK);
                        return "ERROR";

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error enviando correo del Details_Ticket ID: " + ID_DETA_TICK);
                    Console.WriteLine("Error generado: " + ex.Message.ToString());
                    return "ERROR";
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine("Error generando datos para el correo (Update) del Details_Ticket ID: " + ID_DETA_TICK);
                Console.WriteLine("Error generado: " + exx.Message.ToString());
                return "ERROR";
            }
        }

        public string SendMailUpdateStatusBuenaventura(int ID_DETA_TICK)
        {

            Validate vmail = new Validate();
            string from = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24).VAL_ACCO_PARA.ToString();
            string pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25).VAL_ACCO_PARA.ToString();
            try
            {
                var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);

                int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);
                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                int estadoDetalle = Convert.ToInt32(deta_tick.ID_STAT);
                string correoUsuarioAfectado = "";
                if (estadoDetalle == 4)
                {

                    string estado = "", code_ticket = ticket.COD_TICK;
                    try
                    {
                        //SendMailBuenaventura newMail = new SendMailBuenaventura();
                        MailMessage message = new MailMessage();
                        TicketTemplate body = new TicketTemplate();
                        string body_html = "";

                        body_html = body.CrearDetalleTicketBuenaventura(ID_DETA_TICK);

                        PERSON_ENTITY pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END.Value);
                        correoUsuarioAfectado = pe.EMA_PERS;

                        if (body_html != null)
                        {
                            var categoria = (from t in dbc.TICKETs.Where(t => t.ID_TICK == ID_TICK).ToList()
                                             join cat in dbc.CATEGORies on t.ID_CATE equals cat.ID_CATE
                                             select new
                                             {
                                                 NombreCategoria = cat.NAM_CATE
                                             }
                                             ).First();

                            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                            message.From = new MailAddress(from);
                            message.Body = body_html;
                            message.To.Add(correoUsuarioAfectado);
                            message.AlternateViews.Add(htmlView);
                            message.Subject = "Ticket: " + code_ticket + " / " + categoria.NombreCategoria + "/ ha sido atendido";
                            SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                            oSmtpClient.EnableSsl = true;
                            oSmtpClient.UseDefaultCredentials = false;
                            oSmtpClient.Port = 587;
                            oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                            oSmtpClient.Send(message);
                            oSmtpClient.Dispose();
                        }
                        else
                        {
                            Console.WriteLine("Error: No se pudo generar el Template en Details_Ticket ID: " + ID_DETA_TICK);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error enviando correo del Details_Ticket ID: " + ID_DETA_TICK);
                        Console.WriteLine("Error generado: " + ex.Message.ToString());
                    }
                }
                //Despues de enviar el correo actualizamos el flag de envio de correo
                var udticket = dbc.DETAILS_TICKET.Find(ID_DETA_TICK);
                udticket.SEND_MAIL = true;
                udticket.CREATED_MAIL = DateTime.Now;
                dbc.DETAILS_TICKET.Attach(udticket);
                dbc.Entry(udticket).State = EntityState.Modified;
                dbc.SaveChanges();

                Console.WriteLine("Se envió correo del Details_Ticket ID: " + ID_DETA_TICK);

                return "OK";
            }
            catch (Exception exx)
            {
                Console.WriteLine("Error generando datos para el correo (Update) del Details_Ticket ID: " + ID_DETA_TICK);
                Console.WriteLine("Error generado: " + exx.Message.ToString());
                return "ERROR";
            }
        }

        //Creación de contenido y envío de correo al transferir ticket.
        public string SendMailTransferTicket(int ID_DETA_TICK)
        {
            try
            {
                var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
                int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);
                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                //PARA EL CASO QUE TENGA CUENTA DE CORREO EN OUTSOURSING
                var mailCLient = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 32)
                    .Where(x => x.ID_ACCO == ticket.ID_ACCO.Value).Count();

                ACCOUNT_PARAMETER coordinator = null;

                try
                {
                    coordinator = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
                                            .Single(x => x.ID_ACCO == ticket.ID_ACCO);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Details_Ticket -> Tabla Account_Parameter ID_PARA=19 (SIN COORDINADOR DE CUENTA) en el ID_ACCO: " + ticket.ID_ACCO);
                    Console.WriteLine("Detalle generado: " + e.Message.ToString());
                    coordinator = null;
                }

                PERSON_ENTITY per_mail_coor = null;

                if (coordinator != null)
                {
                    int IdCoor = Convert.ToInt32(coordinator.VAL_ACCO_PARA);

                    try
                    {
                        per_mail_coor = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == IdCoor);

                    }
                    catch
                    {
                        per_mail_coor = null;
                    }
                }
                string mail_coor = null;
                if (per_mail_coor != null)
                {
                    //Verificar mail del cliente del coordinador
                    if (mailCLient > 0)
                    {
                        mail_coor = per_mail_coor.EMA_ELEC + (String.IsNullOrEmpty(per_mail_coor.EMA_PERS) ? "" : "," + per_mail_coor.EMA_PERS);
                    }
                    else
                    {
                        mail_coor = per_mail_coor.EMA_ELEC;
                    }
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
                catch (Exception e)
                {
                    Console.WriteLine("Deails_Ticket -> Sin Supervisor de Queue (NO TIENE SUPERVISOR QUEUE)");
                    Console.WriteLine("Detalle generado: " + e.Message.ToString());
                }

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
                try
                {
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    TicketTemplate body = new TicketTemplate();
                    string body_html = body.TransferTo(ID_DETA_TICK);

                    //Copia a los supervisores
                    if (ticket.ID_ACCO == 1 || ticket.ID_ACCO == 25)
                    {
                        //message.Bcc.Add("evalverde@electrodata.com.pe,edeza@electrodata.com.pe,destraver@electrodata.com.pe");
                        //message.Bcc.Add("edeza@electrodata.com.pe,destraver@electrodata.com.pe");
                        message.Bcc.Add("destraver@electrodata.com.pe");
                    }

                    if (body_html != null)
                    {

                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                        string msgcopy = mail_coor + (String.IsNullOrEmpty(mail_sup_queu) ? "" : "," + mail_sup_queu);
                        message.To.Add(mailTo);
                        message.CC.Add(msgcopy);
                        message.AlternateViews.Add(htmlView);
                        message.Subject = "Transfer Ticket - " + code_ticket + " / Tranferencia de Boleto - " + code_ticket;
                        newMail.smtp.Send(message);

                        Console.WriteLine("To " + mailTo);
                        Console.WriteLine("CC " + msgcopy);
                        Console.WriteLine("Se envió correo del Details_Ticket id: " + ID_DETA_TICK);
                        return "OK";
                    }
                    else
                    {
                        Console.WriteLine("Error: No se pudo generar el Template en Details_Ticket para ser transferiro ID: " + ID_DETA_TICK);
                        return "ERROR";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error enviando correo  del Details_Ticket ID: " + ID_DETA_TICK);
                    Console.WriteLine("Error generado: " + ex.Message.ToString());
                    return "ERROR";
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine("Error generando datos para el correo (Transfer) del Details_Ticket ID: " + ID_DETA_TICK);
                Console.WriteLine("Error generado: " + exx.Message.ToString());
                return "ERROR";
            }
        }

        //Creación de contenido y envío de correo al transferir ticket.
        public string EnviarCorreoTransferidoBVN(int ID_DETA_TICK)
        {
            string from = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
            string pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
            try
            {
                var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);
                int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);
                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);

                string mailTo = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).EMA_ELEC;

                int caseSwitch = Convert.ToInt32(deta_tick.ID_STAT);

                string code_ticket = ticket.COD_TICK;
                try
                {
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    TicketTemplate body = new TicketTemplate();
                    string body_html = body.CrearCorreoTransferenciaBVN(ID_DETA_TICK);

                    if (body_html != null)
                    {
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                        message.From = new MailAddress(from);
                        message.Body = body_html;
                        message.To.Add(mailTo);
                        message.AlternateViews.Add(htmlView);
                        message.Subject = "Transferencia de Ticket - " + code_ticket;
                        SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                        oSmtpClient.EnableSsl = true;
                        oSmtpClient.UseDefaultCredentials = false;
                        oSmtpClient.Port = 587;
                        oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                        oSmtpClient.Send(message);
                        oSmtpClient.Dispose();


                        Console.WriteLine("To " + mailTo);
                        Console.WriteLine("Se envió correo del Details_Ticket id: " + ID_DETA_TICK);
                        return "OK";
                    }
                    else
                    {
                        Console.WriteLine("Error: No se pudo generar el Template en Details_Ticket para ser transferiro ID: " + ID_DETA_TICK);
                        //return "ERROR";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error enviando correo  del Details_Ticket ID: " + ID_DETA_TICK);
                    Console.WriteLine("Error generado: " + ex.Message.ToString());
                    //return "ERROR";
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine("Error generando datos para el correo (Transfer) del Details_Ticket ID: " + ID_DETA_TICK);
                Console.WriteLine("Error generado: " + exx.Message.ToString());
                //return "ERROR";
            }
            return "OK";
        }

        //Actualización de Tickets programados.
        public string UpdateStatusTicketScheduled()
        {
            bool ticketFueraOficina = false;
            int razonFueraOficina = 0;
            //Verificacion de Tickets programados fuera de oficina Buenaventura
            //try
            //{
            //    //Busca los tickets a traves de los detalles que cumplen la condicion
            //    //Retorna el ticket, y el ultimo detalle de ticket que haya cumplido con la condicion (programado horario fuera de oficina)

            //    var ticketsProgramados = (from a in dbc.TICKETs.Where(x => x.ID_STAT_END == 5 && x.ID_ACCO == 60)
            //                 join b in dbc.DETAILS_TICKET.Where(
            //                     x => x.ID_STAT == 5 && 
            //                     x.ID_TYPE_DETA_TICK == 3 && 
            //                     x.COM_DETA_TICK == "Se programa ticket para horario laboral") on a.ID_TICK equals b.ID_TICK
            //                 group b by b.ID_TICK into g
            //                 select new
            //                 {
            //                     ID_TICK = g.Key,
            //                     ID_DETA_TICK = g.Max(x => x.ID_DETA_TICK),
            //                 }).ToList();

            //    for (int i = 0; i < ticketsProgramados.Count; i++)
            //    {
            //        int ID_TICK = Convert.ToInt32(ticketsProgramados[i].ID_TICK);
            //        int ID_DETA_TICK = Convert.ToInt32(ticketsProgramados[i].ID_DETA_TICK);

            //        //Validamos que el detalle rescatado sea el ultimo estado que tiene registrado el detalle del ticket para no cruzar informacion
            //        var ultimoDetalleTicket = dbc.DETAILS_TICKET.Last(x => x.ID_TICK == ID_TICK);

            //        if(ultimoDetalleTicket.ID_DETA_TICK == ID_DETA_TICK)
            //        {
            //            //Buscamos por cada Ticket, los detalles correspondientes
            //            var detallesTicket = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK).ToList();
            //            int nroDetalles = detallesTicket.Count;

            //            //Busca el penultimo para tener el ultimo y penultimo
            //            DETAILS_TICKET penultimoDetalleTicket = detallesTicket.ElementAt(nroDetalles-2);

            //            //Cargamos el nuevo detalle que se cargara al ticket
            //            var nuevoDetalle = new DETAILS_TICKET();
            //            nuevoDetalle.ID_TICK = penultimoDetalleTicket.ID_TICK;
            //            nuevoDetalle.ID_STAT = penultimoDetalleTicket.ID_STAT;
            //            nuevoDetalle.ID_TYPE_DETA_TICK = penultimoDetalleTicket.ID_TYPE_DETA_TICK;
            //            nuevoDetalle.COM_DETA_TICK = penultimoDetalleTicket.COM_DETA_TICK;
            //            nuevoDetalle.UserId = penultimoDetalleTicket.UserId;
            //            nuevoDetalle.CREATE_DETA_INCI = DateTime.Now;
            //            nuevoDetalle.SEND_MAIL = true;
            //            dbc.DETAILS_TICKET.Add(nuevoDetalle);
            //            dbc.SaveChanges();

            //            //Actualizamos el ticket
            //            var tick = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
            //            tick.ID_STAT_END = penultimoDetalleTicket.ID_STAT;
            //            dbc.TICKETs.Attach(tick);
            //            dbc.Entry(tick).State = EntityState.Modified;
            //            dbc.SaveChanges();
            //            Console.WriteLine("Se actualizo el ticket : " + tick.ID_TICK);
            //        }


            //    }

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Hubo un error al actualizar los tickets programados fuera de horario de oficina");
            //    Console.WriteLine("Error generado: " + e.Message.ToString());
            //    return "ERROR";
            //}

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

                             });

                var querysch = (from q in query
                                join dt in dbc.DETAILS_TICKET on q.ID_DETA_TICK equals dt.ID_DETA_TICK
                                where dt.FEC_SCHE <= ahora
                                select new
                                {
                                    q.ID_TICK,
                                    dt.ID_DETA_TICK,
                                    dt.FEC_SCHE,
                                }).ToList();

                for (int i = 0; i < querysch.Count; i++)
                {
                    int ID_DETA_TICK = querysch[i].ID_DETA_TICK;
                    int ID_TICK = Convert.ToInt32(querysch[i].ID_TICK);

                    var dt = new DETAILS_TICKET();
                    dt.ID_TICK = ID_TICK;
                    dt.ID_STAT = 1;
                    dt.ID_TYPE_DETA_TICK = 3;
                    dt.COM_DETA_TICK = "THE SYSTEM MANAGER UPDATED THE STATUS OF A TICKET ASSIGNED TO THE AGREED SCHEDULE.";
                    dt.UserId = 34;
                    dt.CREATE_DETA_INCI = ahora;
                    dt.SEND_MAIL = false;

                    //Verificacion de tickets fuera de horario laboral
                    var ticketEvaluado = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                    var ticketDetalleEvaluado = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);


                    if (ticketEvaluado.ID_ACCO == 60 &&
                        ticketDetalleEvaluado.COM_DETA_TICK == "" &&
                        ticketDetalleEvaluado.IdRazon == 11)
                    {
                        ticketFueraOficina = true;
                        //var detalles = (from details in dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK) orderby details.ID_DETA_TICK descending select details).ToList();
                        //razonFueraOficina = (detalles.Count() > 1) ? Convert.ToInt32((detalles.Take(2).First().IdRazon == 0) ? 2 : detalles.Take(2).First().IdRazon) : 2;
                        razonFueraOficina = 2;
                        dt.IdRazon = razonFueraOficina;
                    }

                    dbc.DETAILS_TICKET.Add(dt);
                    dbc.SaveChanges();

                    //Verificación antes de re-activar el ticket
                    //Buscar entre los detalle del ticket si ya se realizó una modificación adicional
                    //Comentario
                    int modificado = (int)dbc.VerificarTicketModificado(ID_TICK, ID_DETA_TICK).SingleOrDefault();
                    bool flag = Convert.ToBoolean(modificado);
                    if (!flag)
                    {
                        var tick = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                        tick.ID_STAT_END = 1;
                        tick.EsperaPorCliente = false;

                        //Verificacion de tickets fuera de horario laboral
                        if (tick.ID_ACCO == 60) tick.IdRazon = 2;

                        dbc.TICKETs.Attach(tick);
                        dbc.Entry(tick).State = EntityState.Modified;
                        dbc.SaveChanges();
                        Console.WriteLine("Se generó mensaje del Schedule del detalle con ID: " + dt.ID_DETA_TICK);
                    }
                    else
                    {
                        if (ticketFueraOficina)
                        {
                            var tick = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                            tick.ID_STAT_END = 1;
                            tick.IdRazon = razonFueraOficina;
                            tick.MODIFIED_TICK = DateTime.Now;
                            dbc.TICKETs.Attach(tick);
                            dbc.Entry(tick).State = EntityState.Modified;
                            dbc.SaveChanges();

                            Console.WriteLine("Se generó mensaje del Schedule del detalle con ID: " + dt.ID_DETA_TICK);
                        }

                    }
                }

                //Pasando aquellos tickets que se crearon en estado scheduled y no presentan detalle en schedule
                try
                {
                    var qSche = (from a in dbc.TICKETs.Where(x => x.ID_STAT_END == 5)
                                 join b in dbc.DETAILS_TICKET.Where(x => x.ID_STAT == 5).Where(x => x.ID_TYPE_DETA_TICK == 3) on a.ID_TICK equals b.ID_TICK
                                 into lb
                                 from xb in lb.DefaultIfEmpty()
                                 select new
                                 {
                                     a.ID_TICK,
                                     a.FEC_INI_TICK,
                                     ID_DETA_TICK = (xb == null ? 0 : xb.ID_DETA_TICK),
                                 }).Where(x => x.ID_DETA_TICK == 0 && x.FEC_INI_TICK <= ahora).ToList();

                    int tota3 = qSche.Count();

                    for (int i = 0; i < qSche.Count; i++)
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
                        tick.MODIFIED_TICK = DateTime.Now;
                        dbc.TICKETs.Attach(tick);
                        dbc.Entry(tick).State = EntityState.Modified;
                        dbc.SaveChanges();

                        Console.WriteLine("Se generó mensaje del Schedule del detalle con ID: " + dt.ID_DETA_TICK);
                    }

                }
                catch (Exception e)
                {
                    var exc = new EXCEPTION();
                    exc.DAT_EXCE = DateTime.Now;
                    exc.MESSAGE = e.InnerException.Message;
                    exc.DES_EXCE = "Send Mail - UpdateStatusTicketScheduled: Para aquellos tickets que no presentan detalle";
                    dbl.EXCEPTIONs.Add(exc);
                    dbl.SaveChanges();

                    Console.WriteLine("Hubo un error al generar crear un Detalle de Ticket Schedule");
                    Console.WriteLine("Error generado: " + e.Message.ToString());

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
                dbl.EXCEPTIONs.Add(exc);
                dbl.SaveChanges();

                Console.WriteLine("Hubo un error al generar Error al generar el Detalle de Ticket Schedule");
                Console.WriteLine("Error generado: " + e.Message.ToString());
                return "ERROR";
            }
        }

        //Notificaciones de tiempos de SLA.
        //public string NotificationSLA()
        //{
        //    try
        //    {
        //        var ini = DateTime.Now;
        //        int count = 0;
        //        int count1 = 0, ID_TYPE_NOTI = 0;
        //        int[] array = new int[3];
        //        array[0] = 1; //ASSIGNED - ASIGNADO
        //        array[1] = 3; //OPEN - ABIERTO
        //        array[2] = 5; //SCHEDULED - PROGRAMADO

        //        int[] arraytt = new int[2];
        //        arraytt[0] = 1;//INCIDENT - INCIDENTE 
        //        arraytt[1] = 2;//REQUEST - REQUERIMIENTO

        //        string cod = "", html_body = null;
        //        string text = "";

        //        TicketTemplate _Body = new TicketTemplate();

        //        //Seleccionamos todos los tickets con estado Assigned, Open, Schedule y del tipo Incidente o requerimiento que no sean Planning
        //        var query = dbc.TICKETs.Where(t => array.Contains((int)t.ID_STAT_END))
        //            .Where(t => t.ID_PRIO != 5) // NO ES PLANIFICADO
        //            .Where(t => arraytt.Contains((int)t.ID_TYPE_TICK)) //INCIDENTES O REQURIMIENTOS
        //            .Where(t => t.ID_ACCO != null)
        //            .Where(t => t.ID_ACCO > 0).ToList();

        //        //Recorremos la Lista de Tickets
        //        foreach (TICKET tickets in query)
        //        {
        //            html_body = null;
        //            string cc = "";
        //            int horas = 0;
        //            int minutes = 0;
        //            int ID_TICK = Convert.ToInt32(tickets.ID_TICK);

        //            int test = 0;
        //            if (ID_TICK == 100431)
        //            {
        //                test = 1;
        //            }
        //            //Seleccionamos un ticket con su prioridad

        //            var ticket = (from t in dbc.TICKETs
        //                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
        //                          where t.ID_TICK == ID_TICK
        //                          select new
        //                          {
        //                              p.HOU_PRIO,//CAMBIO -> Tiempo atencion - SLA Detalle
        //                              t.COD_TICK,
        //                              t.FEC_TICK,
        //                              t.MINUTES,
        //                              //  t.TYPE_TICKET,
        //                              t.ID_PERS_ENTI_ASSI,
        //                              t.ID_ACCO,
        //                              FEC_INI_TICK = t.FEC_INI_TICK == null ? t.FEC_TICK : t.FEC_INI_TICK
        //                          }).First();

        //            int ID_ACCO = Convert.ToInt32(ticket.ID_ACCO);

        //            int mailCLient = 0;
        //            try
        //            {
        //                //ID_PARA=32-->MAIL TICKET CLIENT
        //                mailCLient = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 32)
        //                .Where(x => x.ID_ACCO == ticket.ID_ACCO.Value).Count(); //Resultado es True
        //            }
        //            catch (Exception e)
        //            {
        //                mailCLient = 0;
        //                Console.WriteLine("SLA -> Account_Parameter ID_PARA=32 (NO EXISTE MAIL PARA CLIENTE) para el ID_ACCO: " + ticket.ID_ACCO.Value);
        //                Console.WriteLine("Detalle Generado: " + e.Message.ToString());
        //            }
        //            var Resp = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);
        //            string mailTo = null;

        //            //Si se envia correo a outsoucing

        //            if (mailCLient > 0)
        //            {

        //                if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
        //                {
        //                    mailTo = Resp.EMA_ELEC;
        //                }
        //                if (!String.IsNullOrEmpty(Resp.EMA_PERS))
        //                {
        //                    mailTo = (!String.IsNullOrEmpty(Resp.EMA_PERS) ? mailTo + "," : "") + Resp.EMA_PERS;
        //                }
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(Resp.EMA_ELEC))
        //                {
        //                    mailTo = Resp.EMA_ELEC;
        //                }
        //            }

        //            if (String.IsNullOrEmpty(mailTo))
        //            {
        //                mailTo = "itms@electrodata.com.pe"; //Si no hay correo se envia al imtms
        //            }

        //            var deta_tick = dbc.DETAILS_TICKET
        //                .Where(x => x.ID_TICK == ID_TICK)
        //                .Where(x => x.ID_STAT != null)
        //                .OrderByDescending(x => x.ID_DETA_TICK); //Seleccionamos los Details_Ticket.

        //            if (deta_tick.Count() > 0)
        //            {
        //                DateTime fecha_ini_2;

        //                if (deta_tick.Count() >= 2 && deta_tick.First().ID_STAT == 5 && deta_tick.First().ID_TYPE_DETA_TICK == 1)
        //                {
        //                    var qdtick = deta_tick.Where(x => x.ID_TYPE_DETA_TICK == 3 && x.ID_STAT == 5).OrderByDescending(x => x.ID_DETA_TICK);
        //                    fecha_ini_2 = Convert.ToDateTime(qdtick.First().FEC_SCHE);
        //                }
        //                else
        //                {

        //                    fecha_ini_2 = Convert.ToDateTime(deta_tick.First().FEC_SCHE);
        //                }

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

        //            // Hasta aca se ha seleccionado el tiempo y de aca si verifica si cumple los  datos del tiempo. 

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
        //                    try
        //                    {

        //                        int ID_COOR = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
        //                        .Single(x => x.ID_ACCO == ID_ACCO).VAL_ACCO_PARA);

        //                        var Coor = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_COOR);

        //                        if (Coor.EMA_ELEC != null)
        //                        {
        //                            cc = Coor.EMA_ELEC;

        //                        }
        //                        if (Coor.EMA_PERS != null)
        //                        {
        //                            cc = cc + "," + Coor.EMA_PERS;
        //                        }
        //                    }
        //                    catch
        //                    {

        //                    }
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

        //                        if (Coor.EMA_ELEC != null)
        //                        {
        //                            cc = Coor.EMA_ELEC;
        //                        }
        //                        if (Coor.EMA_PERS != null)
        //                        {
        //                            cc = cc + "," + Coor.EMA_PERS;
        //                        }

        //                    }
        //                    catch
        //                    {

        //                    }
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
        //                        int ID_COOR = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 19 && x.VIG_ACCO_PARA == true)
        //                        .Single(x => x.ID_ACCO == ID_ACCO).VAL_ACCO_PARA);

        //                        var Coor = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ID_COOR);

        //                        int bandera = 0;

        //                        if (Coor.EMA_ELEC != null)
        //                        {
        //                            cc = Coor.EMA_ELEC; //+ ",cfemenias@electrodata.com.pe";
        //                            bandera = 1;
        //                        }
        //                        if (Coor.EMA_PERS != null)
        //                        {
        //                            if (bandera == 1)
        //                            {
        //                                cc = cc + "," + Coor.EMA_PERS;
        //                            }
        //                            else
        //                            {
        //                                cc = Coor.EMA_PERS; //+",cfemenias@electrodata.com.pe";
        //                            }
        //                        }
        //                        if (Coor.EMA_PERS != null && Coor.EMA_ELEC != null)
        //                        {
        //                            cc = ""; // "cfemenias@electrodata.com.pe";
        //                        }
        //                    }
        //                    catch
        //                    {

        //                    }

        //                    html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);
        //                }
        //            }

        //            //if(Convert.ToInt32(ticket.HOU_PRIO) - Convert.ToInt32(horas) == ticket.HOU_PRIO * 7 /10)
        //            //{
        //            //    ID_TYPE_NOTI = 5;
        //            //    var index1 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK)
        //            //        .Where(x => x.ID_TYPE_NOTI == ID_TYPE_NOTI).Count();
        //            //    if (index1 == 0)
        //            //    {
        //            //        html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI);
        //            //    }
        //            //}

        //            if (html_body != null)
        //            {
        //                try
        //                {
        //                    //if(ID_ACCO >= 60 || ID_ACCO <= 74)
        //                    //{
        //                    //    SendMailBuenaventura newMail = new SendMailBuenaventura();
        //                    //    var message = newMail.mailMessage;
        //                    //    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_body, Encoding.UTF8, MediaTypeNames.Text.Html);

        //                    //    message.To.Add(mailTo);
        //                    //    if (!String.IsNullOrEmpty(cc))
        //                    //    {
        //                    //        //message.CC.Add(cc);
        //                    //    }
        //                    //    message.AlternateViews.Add(htmlView);
        //                    //    message.Subject = "Alert SLA " + ticket.COD_TICK;
        //                    //    newMail.smtp.Send(message);
        //                    //    TICKET_NOTIFICATION ticket_noti = new TICKET_NOTIFICATION();
        //                    //    ticket_noti.ID_TICK = ID_TICK;
        //                    //    ticket_noti.ID_TYPE_NOTI = ID_TYPE_NOTI;
        //                    //    ticket_noti.CREATED = DateTime.Now;
        //                    //    dbc.TICKET_NOTIFICATION.Add(ticket_noti);
        //                    //    dbc.SaveChanges();

        //                    //    Console.WriteLine("Add: " + mailTo);
        //                    //    Console.WriteLine("CC: " + cc);
        //                    //    Console.WriteLine("Correo de Alerta SLA enviado correctamenete para el Ticket código: " + ticket.COD_TICK);

        //                    //    string nuevotext = "";
        //                    //    nuevotext = "ID_TICK: " + Convert.ToString(ID_TICK) + " ID_TYPE_NOTI: " + Convert.ToString(ID_TYPE_NOTI) + "\r\n";

        //                    //    text = text + nuevotext;
        //                    //}
        //                    //else
        //                    //{
        //                    SendMail newMail = new SendMail();
        //                    var message = newMail.mailMessage;
        //                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_body, Encoding.UTF8, MediaTypeNames.Text.Html);

        //                    //message.To.Add(mailTo);
        //                    if (!String.IsNullOrEmpty(cc))
        //                    {
        //                        //message.CC.Add(cc);
        //                    }
        //                    //message.AlternateViews.Add(htmlView);
        //                    message.Subject = "Alert SLA " + ticket.COD_TICK;
        //                    //newMail.smtp.Send(message);

        //                    //TICKET_NOTIFICATION ticket_noti = new TICKET_NOTIFICATION();
        //                    //ticket_noti.ID_TICK = ID_TICK;
        //                    //ticket_noti.ID_TYPE_NOTI = ID_TYPE_NOTI;
        //                    //ticket_noti.CREATED = DateTime.Now;
        //                    //dbc.TICKET_NOTIFICATION.Add(ticket_noti);
        //                    //dbc.SaveChanges();

        //                    Console.WriteLine("Add: " + mailTo);
        //                    Console.WriteLine("CC: " + cc);
        //                    Console.WriteLine("Correo de Alerta SLA enviado correctamenete para el Ticket código: " + ticket.COD_TICK);

        //                    string nuevotext = "";
        //                    nuevotext = "ID_TICK: " + Convert.ToString(ID_TICK) + " ID_TYPE_NOTI: " + Convert.ToString(ID_TYPE_NOTI) + "\r\n";

        //                    text = text + nuevotext;
        //                    //}

        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine("Error enviando correo SLA para el Ticket código: " + ticket.COD_TICK);
        //                    Console.WriteLine("Error generado: " + e.Message.ToString());
        //                }
        //            }

        //        }

        //        System.IO.File.WriteAllText(@"E:\WriteText.txt", text);

        //        return "OK";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error consiguiendo data para el correo de Alerta de SLA");
        //        Console.WriteLine("Error generado: " + ex.Message.ToString());
        //        return "ERROR";
        //    }
        //}

        public string NotificationSLABuenaventura()
        {
            try
            {
                var ini = DateTime.Now;
                int ID_TYPE_NOTI = 0;


                string from = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 24 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();
                string pass = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_ACCO == 60 && x.ID_PARA == 25 && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA.ToString();


                string tipoTicket = "", html_body = null;
                string text = "";

                TicketTemplate _Body = new TicketTemplate();

                var query = dbc.ObtenerTicketParaNotificacion().ToList();

                //Recorremos la Lista de Tickets
                foreach (ObtenerTicketParaNotificacion_Result tickets in query)
                {
                    html_body = null;
                    string asunto = "";
                    decimal horas = 0;
                    int minutes = 0;
                    int ID_TICK = Convert.ToInt32(tickets.ID_TICK);


                    var ticket = (from t in dbc.TICKETs
                                      //join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                                  join s in dbc.SLA on t.IdSLA equals s.Id
                                  join sd in dbc.SLADetalle on t.IdSLA equals sd.IdSLA
                                  join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                                  join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                                  join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                                  join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                                  where t.ID_TICK == ID_TICK && sd.IdPrioridad == t.ID_PRIO
                                  select new
                                  {
                                      t.ID_TICK,
                                      sd.TiempoAtencion,//CAMBIO -> Tiempo atencion - SLA Detalle
                                      t.COD_TICK,
                                      t.FEC_TICK,
                                      t.MINUTES,
                                      //  t.TYPE_TICKET,
                                      t.ID_PERS_ENTI_ASSI,
                                      t.ID_ACCO,
                                      FEC_INI_TICK = t.FEC_INI_TICK == null ? t.FEC_TICK : t.FEC_INI_TICK,
                                      FEC_MAIL_SLA = t.CrearCorreoSLA == null ? null : t.CrearCorreoSLA,
                                      t.EnvioCorreoSLA,
                                      t.ID_TYPE_TICK,
                                      c.NAM_CATE
                                  }).First();

                    //cuenta de usuario 
                    int ID_ACCO = Convert.ToInt32(ticket.ID_ACCO);

                    if (ticket.ID_TYPE_TICK == 1)
                        tipoTicket = "Inc.";
                    else if (ticket.ID_TYPE_TICK == 2)
                        tipoTicket = "Req.";

                    var Resp = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI);
                    //var Resp = 9104;

                    string correoEspecialista = null;

                    if (!String.IsNullOrEmpty(Resp.EMA_PERS))
                    {
                        correoEspecialista = Resp.EMA_PERS;
                    }


                    if (String.IsNullOrEmpty(correoEspecialista))
                    {
                        correoEspecialista = "itms@electrodata.com.pe"; //Si no hay correo se envia al imtms
                    }

                    var deta_tick = dbc.DETAILS_TICKET
                        .Where(x => x.ID_TICK == ID_TICK)
                        .OrderByDescending(x => x.ID_DETA_TICK); //Seleccionamos los Details_Ticket.


                    horas = dbc.ObtenerTiempoTranscurridoTicket(ID_TICK).Single().HorasTicket;

                    // Hasta aca se ha seleccionado el tiempo y de aca si verifica si cumple los  datos del tiempo. 
                    if (horas >= (ticket.TiempoAtencion / 2))
                    {
                        #region Diferenciacion SLA (50/70/Vencido)
                        //VENCIO EL SLA (100%)
                        if (horas >= ticket.TiempoAtencion)
                        {
                            //Venció SLA
                            ID_TYPE_NOTI = 4;
                            asunto = "SLA VENCIDO - " + tipoTicket + " " + ticket.COD_TICK + " / " + ticket.NAM_CATE;
                        }
                        else
                        {
                            //SLA 70%
                            if (Convert.ToDouble(horas) >= Convert.ToDouble(ticket.TiempoAtencion) * 0.7)
                            {
                                //70% del tiempo para vencimiento
                                ID_TYPE_NOTI = 10;
                                asunto = "SLA POR VENCER 70% - " + tipoTicket + " " + ticket.COD_TICK + " / " + ticket.NAM_CATE;
                            }
                            //SLA 50%
                            else
                            {
                                //Mitad del tiempo para vencimiento
                                ID_TYPE_NOTI = 1;
                                asunto = "SLA POR VENCER 50% - " + tipoTicket + " " + ticket.COD_TICK + " / " + ticket.NAM_CATE;
                            }
                        }
                        #endregion

                        if (ID_TYPE_NOTI == 1 || ID_TYPE_NOTI == 10 || ID_TYPE_NOTI == 4)
                            html_body = _Body.TicketNotification(ID_TICK, ID_TYPE_NOTI, tipoTicket);
                        if (html_body != null)
                        {
                            try
                            {

                                MailMessage message = new MailMessage();

                                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_body, Encoding.UTF8, MediaTypeNames.Text.Html);

                                message.From = new MailAddress(from);
                                message.Subject = asunto;
                                message.Body = html_body;
                                message.To.Add(correoEspecialista);
                                message.AlternateViews.Add(htmlView);
                                SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
                                oSmtpClient.EnableSsl = true;
                                oSmtpClient.UseDefaultCredentials = false;
                                oSmtpClient.Port = 587;

                                oSmtpClient.Credentials = new System.Net.NetworkCredential(from, pass);
                                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                                oSmtpClient.Send(message);
                                oSmtpClient.Dispose();

                                //if (ID_TYPE_NOTI == 4)
                                //{
                                //    var ticksla = dbc.TICKETs.Find(ticket.ID_TICK);
                                //    ticksla.EnvioCorreoSLA = true;
                                //    ticksla.CrearCorreoSLA = DateTime.Now;
                                //    dbc.TICKETs.Attach(ticksla);
                                //    dbc.Entry(ticksla).State = EntityState.Modified;
                                //    dbc.SaveChanges();
                                //}

                                //if (ID_TYPE_NOTI == 4) // AGREGAR IF
                                // { // Si pasó la fecha expirada de SLA ,se registra fecha para los envios anteriores 50% y 70% para que no se detenga en el bucle - PARA REGISTROS ANTIGUOS
                                //var query50 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK && ID_TYPE_NOTI == 1).Count();
                                //if (query50 == 0)
                                //{
                                //    TICKET_NOTIFICATION ticket_noti50 = new TICKET_NOTIFICATION();
                                //    ticket_noti50.ID_TICK = ID_TICK;
                                //    ticket_noti50.ID_TYPE_NOTI = 1;
                                //    ticket_noti50.CREATED = DateTime.Now;
                                //    dbc.TICKET_NOTIFICATION.Add(ticket_noti50);
                                //    dbc.SaveChanges();
                                //}

                                //var query70 = dbc.TICKET_NOTIFICATION.Where(x => x.ID_TICK == ID_TICK && ID_TYPE_NOTI == 10).Count();
                                //if (query70 == 0)
                                //{
                                //    TICKET_NOTIFICATION ticket_noti10 = new TICKET_NOTIFICATION();
                                //    ticket_noti10.ID_TICK = ID_TICK;
                                //    ticket_noti10.ID_TYPE_NOTI = 10;
                                //    ticket_noti10.CREATED = DateTime.Now;
                                //    dbc.TICKET_NOTIFICATION.Add(ticket_noti10);
                                //    dbc.SaveChanges();
                                //}

                                //    TICKET_NOTIFICATION ticket_noti = new TICKET_NOTIFICATION();
                                //    ticket_noti.ID_TICK = ID_TICK;
                                //    ticket_noti.ID_TYPE_NOTI = ID_TYPE_NOTI;
                                //    ticket_noti.CREATED = DateTime.Now;
                                //    dbc.TICKET_NOTIFICATION.Add(ticket_noti);
                                //    dbc.SaveChanges();
                                //}
                                //else
                                //{
                                TICKET_NOTIFICATION ticket_noti = new TICKET_NOTIFICATION();
                                ticket_noti.ID_TICK = ID_TICK;
                                ticket_noti.ID_TYPE_NOTI = ID_TYPE_NOTI;
                                ticket_noti.CREATED = DateTime.Now;
                                dbc.TICKET_NOTIFICATION.Add(ticket_noti);
                                dbc.SaveChanges();
                                //}
                                Console.WriteLine("Add: " + correoEspecialista);
                                Console.WriteLine("Correo de Alerta SLA enviado correctamenete para el Ticket código: " + ticket.COD_TICK);

                                string nuevotext = "";
                                nuevotext = "ID_TICK: " + Convert.ToString(ID_TICK) + " ID_TYPE_NOTI: " + Convert.ToString(ID_TYPE_NOTI) + "\r\n";

                                text = text + nuevotext;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error enviando correo SLA para el Ticket código: " + ticket.COD_TICK);
                                Console.WriteLine("Error generado: " + e.Message.ToString());
                            }
                        }

                    }
                }
                return "OK";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error consiguiendo data para el correo de Alerta de SLA");
                Console.WriteLine("Error generado: " + ex.Message.ToString());
                return "ERROR";
            }
        }


        public string SendMailResolvedOPFromTicket(int ID_DOCU_SALE)
        {
            try
            {
                textInfo = cultureInfo.TextInfo;

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

                //Body _Body = new Body();
                TicketTemplate _Body = new TicketTemplate();
                string body_html = _Body.CreateDocumentSaleEnd(ID_DOCU_SALE);

                //message.From = fromEmail;
                //message.To = "servicedesk@electrodata.com.pe";
                //message.CC = "sgomez@electrodata.com.pe";

                Attachment attachment = new Attachment("C:\\BackUp\\Acta de Conformidad 2015-0117.pdf", MediaTypeNames.Application.Octet);
                //Attachment attachment1 = new Attachment("C:\\BackUp\\INFORME WIFI-NEWMONT.pdf", MediaTypeNames.Application.Octet);

                //message.HTMLBody = body_html;
                //message.Subject = NAM_EMP + "New " + OP_V.NAM_TYPE_DOCU_SALE + " " + OP_V.NUM_DOCU_SALE + "/" + NAM_EMP + "Nueva " + OP_V.NAM_TYPE_DOCU_SALE + " " + OP_V.NUM_DOCU_SALE;
                //message.Send();

                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                //message.To.Add("esalazar@electrodata.com.pe,jvillafana@electrodata.com.pe");
                //message.To.Add("esalazar@electrodata.com.pe");

                message.Attachments.Add(attachment);
                //message.Attachments.Add(attachment1);

                message.To.Add("facturacion@electrodata.com.pe," + vendedor + "ddiaz@electrodata.com.pe");

                message.CC.Add("jquisper@electrodata.com.pe");

                message.AlternateViews.Add(htmlView);
                message.Subject = "Orden de Pedido Resuelta (" + OP_V.NAM_TYPE_DOCU_SALE.Trim() + " " + OP_V.NUM_DOCU_SALE.Trim() + ") - " + cia_end + " - " + nam_vendor;
                newMail.smtp.Send(message);

                return "OK";
            }
            catch
            {
                return "ERROR";
            }
        }

        //Envío de notificaciones al resolver una OP Manualmente.
        public string SendMailResolvedOPFromTicketCook(int ID_DOCU_SALE)
        {
            try
            {
                textInfo = cultureInfo.TextInfo;

                string vendedor = "evasquez@electrodata.com.pe", cia_end = "Gold Fields La Cima", nam_vendor = "Operaciones Electrodata"
                , NAM_TYPE_DOCU_SALE = "OPVP 1507-004";

                //var OP = dbc.DOCUMENT_SALE.Where(x => x.ID_DOCU_SALE == ID_DOCU_SALE);

                //var OP_V = (from x in OP.ToList()
                //            join tds in dbc.TYPE_DOCUMENT_SALE on x.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
                //            select new
                //            {
                //                NUM_DOCU_SALE = x.NUM_DOCU_SALE,
                //                NAM_TYPE_DOCU_SALE = tds.NAM_TYPE_DOCU_SALE,
                //                ID_ENTI = x.ID_ENTI,
                //                ID_COMP_END = x.ID_COMP_END,
                //                ID_PERS_ENTI_VEND = x.ID_PERS_ENTI_VEND
                //            }).First();

                //int ID_ENTI = Convert.ToInt32(OP_V.ID_ENTI.Value);
                string NAM_EMP = "Gold Fields La Cima";

                //try
                //{
                //    var vendor = dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == OP_V.ID_PERS_ENTI_VEND.Value).FirstOrDefault();
                //    var cia_x = dbe.CLASS_ENTITY.Where(ce => ce.ID_ENTI == OP_V.ID_COMP_END.Value).FirstOrDefault();

                //    var vend = dbe.CLASS_ENTITY.Where(ce => ce.ID_ENTI == vendor.ID_ENTI2).FirstOrDefault();

                //    vendedor = String.IsNullOrEmpty(vendor.EMA_ELEC) == true ? "" : vendor.EMA_ELEC.Trim() + ",";
                //    cia_end = String.IsNullOrEmpty(cia_x.COM_NAME) == true ? "" : cia_x.COM_NAME;
                //    nam_vendor = textInfo.ToTitleCase(vend.FIR_NAME.ToLower() + " " + vend.LAS_NAME.ToLower());

                //    NAM_EMP = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ID_ENTI).COM_NAME + " - ";
                //}
                //catch { NAM_EMP = ""; }

                //Body _Body = new Body();
                TicketTemplate _Body = new TicketTemplate();
                //string body_html = _Body.CreateDocumentSaleEnd(ID_DOCU_SALE);
                string body_html = _Body.CreateDocumentSaleEndCook(ID_DOCU_SALE);

                //message.From = fromEmail;
                //message.To = "servicedesk@electrodata.com.pe";
                //message.CC = "sgomez@electrodata.com.pe";

                Attachment attachment = new Attachment("C:\\BackUp\\HEA OPVP 1507-004.pdf", MediaTypeNames.Application.Octet);
                //Attachment attachment1 = new Attachment("C:\\BackUp\\Report - Junio 2015_Casa Andina.xls", MediaTypeNames.Application.Octet);

                //message.HTMLBody = body_html;
                //message.Subject = NAM_EMP + "New " + OP_V.NAM_TYPE_DOCU_SALE + " " + OP_V.NUM_DOCU_SALE + "/" + NAM_EMP + "Nueva " + OP_V.NAM_TYPE_DOCU_SALE + " " + OP_V.NUM_DOCU_SALE;
                //message.Send();

                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                //message.To.Add("esalazar@electrodata.com.pe,jvillafana@electrodata.com.pe");
                //message.To.Add("esalazar@electrodata.com.pe");

                message.Attachments.Add(attachment);
                //message.Attachments.Add(attachment1);

                message.To.Add("facturacion@electrodata.com.pe," + vendedor);// + "esalazar@electrodata.com.pe");//,@electrodata.com.pe" + vendedor);
                message.CC.Add("jquisper@electrodata.com.pe");
                ///////////////////message.CC.Add("cacosta@electrodata.com.pe,kgisakawa@electrodata.com.pe,dnunez@electrodata.com.pe,servicedesk@electrodata.com.pe,pmo@electrodata.com.pe,lcuenca@electrodata.com.pe,ocarrillo@electrodata.com.pe")
                //message.CC.Add("sgomez@electrodata.com.pe,dnunez@electrodata.com.pe,servicedesk@electrodata.com.pe,pmo@electrodata.com.pe,ocarrillo@electrodata.com.pe,lcuenca@electrodata.com.pe,jvillafana@electrodata.com.pe"); //Descomentar
                ////////////////message.Bcc.Add("arivadeneyra@electrodata.com.pe,lmasias@electrodata.com.pe,bacevedo@electrodata.com.pe,zvaldiviezo@electrodata.com.pe,lsolari@electrodata.com.pe");


                message.AlternateViews.Add(htmlView);
                message.Subject = "Orden de Pedido Resuelta (" + NAM_TYPE_DOCU_SALE + ") - " + cia_end + " - " + nam_vendor;
                newMail.smtp.Send(message);

                return "OK";
            }
            catch
            {
                return "ERROR";
            }
        }

        public string SendMailNewCommentSAP(int ID_DETA_TICK)
        {
            string mailMesa = "mesadeayuda@minsur.com"; // correo mensa de ayuda Minsur
            int mailPersonEnti = 0;
            string mailResp = "";
            string mailSAP = "";
            //string mailContralor = ",danielachampa@gmail.com,rvarela@electrodata.com.pe,lestrada@electrodata.com.pe";
            string mailContralor = "";
            Validate vmail = new Validate();

            try
            {
                var deta_tick = dbc.DETAILS_TICKET.Single(x => x.ID_DETA_TICK == ID_DETA_TICK);

                if (deta_tick.UserId != 8974)
                {
                    mailMesa = mailMesa + mailContralor;
                }

                int ID_TICK = Convert.ToInt32(deta_tick.ID_TICK);

                var ticket = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                // Usuario responsable
                mailPersonEnti = Convert.ToInt32(dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).ID_PERS_ENTI_ASSI);

                // Correo usuario responsable del ticket
                try
                {
                    mailResp = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == mailPersonEnti).EMA_ELEC;
                }
                catch
                {
                    mailResp = null;
                }

                //Correo usuario que comenta Ticket (SAP MINSUR)
                var User = dbe.CLASS_ENTITY.Single(x => x.UserId == deta_tick.UserId);
                int ID_ENTI = Convert.ToInt32(User.ID_ENTI);

                var UserUpdate = dbe.PERSON_ENTITY.Single(x => x.ID_ENTI2 == ID_ENTI && x.VIG_PERS_ENTI == true);

                mailSAP = mailSAP + (String.IsNullOrEmpty(UserUpdate.EMA_ELEC) ? "" : "," + UserUpdate.EMA_ELEC)
                            + (String.IsNullOrEmpty(UserUpdate.EMA_PERS) ? "" : "," + UserUpdate.EMA_PERS);

                string code_ticket = ticket.COD_TICK;

                try
                {
                    SendMailMinsur newMail = new SendMailMinsur();
                    var message = newMail.mailMessage;

                    TicketTemplate body = new TicketTemplate();

                    string body_html = "";

                    body_html = body.CreateDetailsCommentTicket(ID_DETA_TICK);

                    if (body_html != null)
                    {
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                        string msgcopy = mailMesa + "," + mailSAP;
                        //string msgcopy = mailMesa;
                        message.To.Add(mailResp);
                        msgcopy = msgcopy.Replace(",,", ",");
                        message.CC.Add(msgcopy);
                        message.AlternateViews.Add(htmlView);
                        message.Subject = "Update Ticket: " + code_ticket + " / Actualización de Ticket: " + code_ticket;
                        newMail.smtp.Send(message);

                        //Despues de enviar el correo actualizamos el flag de envio de correo
                        var udticket = dbc.DETAILS_TICKET.Find(ID_DETA_TICK);
                        udticket.SEND_MAIL = true;
                        udticket.CREATED_MAIL = DateTime.Now;
                        dbc.DETAILS_TICKET.Attach(udticket);
                        dbc.Entry(udticket).State = EntityState.Modified;
                        dbc.SaveChanges();

                        Console.WriteLine("To " + mailResp);
                        Console.WriteLine("CC " + msgcopy);
                        Console.WriteLine("Se envió correo del Details_Ticket ID: " + ID_DETA_TICK);

                        return "OK";
                    }
                    else
                    {
                        Console.WriteLine("Error: No se pudo generar el Template en Details_Ticket ID: " + ID_DETA_TICK);
                        return "ERROR";

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error enviando correo del Details_Ticket ID: " + ID_DETA_TICK);
                    Console.WriteLine("Error generado: " + ex.Message.ToString());
                    return "ERROR";
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine("Error generando datos para el correo (Update) del Details_Ticket ID: " + ID_DETA_TICK);
                Console.WriteLine("Error generado: " + exx.Message.ToString());
                return "ERROR";
            }
        }

        //Envío de reporte de Tickets.
        public string SendTicketReport(int opcion)
        {
            try
            {
                string file = FilePDF(opcion);

                TicketTemplate _body = new TicketTemplate();
                var html_msg = _body.TicketReportTemplate(opcion);

                if (html_msg != null && file != "ERROR")
                {
                    Console.WriteLine("Enviando Correo...");
                    SendMail newMail = new SendMail();
                    var msg = newMail.mailMessage;
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                    msg.AlternateViews.Add(htmlView);

                    if (opcion == 1)
                    {
                        msg.Subject = "Reporte de Tickets Diario " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                        //msg.To.Add("mquispe@electrodata.com.pe");
                        //////////////msg.To.Add("cacosta@electrodata.com.pe");
                        msg.CC.Add("jquisper@electrodata.com.pe");
                    }
                    if (opcion == 2)
                    {
                        msg.Subject = "Reporte de Tickets Semanal - Brocal " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                        msg.To.Add("jquisper@elbrocal.com.pe");
                    }

                    Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                    msg.Attachments.Add(attachment);
                    newMail.smtp.Send(msg);

                    Console.WriteLine("Envio exitoso");
                    Console.WriteLine();
                    return "OK";
                }
                else
                {
                    Console.WriteLine("Existió un error al generar la archivo Excel o la plantilla");
                    return "ERROR";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error enviando correo de reporte de tickets diario");
                Console.WriteLine("Error generado: " + e.Message.ToString());

                return "ERROR";
            }
        }

        //Creación de archivo Excel de Reporte de Tickets.
        public string FilePDF(int opcion)
        {
            try
            {
                Console.WriteLine("Generando archivo. Por favor espere...");
                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                ServerReport sr = new ServerReport();
                sr.ReportServerCredentials = new CredencialesReporting("Administrator", "ITMS$15DEV$", "");
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/TicketReport";

                string ID_ACCO = Convert.ToString(4);
                string TO_DATE = Convert.ToString(DateTime.Today);
                string FROM_DATE = Convert.ToString(Convert.ToDateTime("2013-01-01 0:0:0.0"));

                if (opcion == 2)
                {
                    ID_ACCO = "19";
                    FROM_DATE = Convert.ToString(DateTime.Today.AddDays(-7));
                }

                DateTime ahora = DateTime.Now;

                string DES_FILE = "ReportTicket" + ahora.Year + "-" + ahora.Month + "-" + ahora.Day;

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[1] = new ReportParameter("FROM_DATE", FROM_DATE);
                param[2] = new ReportParameter("TO_DATE", TO_DATE);

                sr.SetParameters(param);
                sr.Refresh();

                byte[] renderedBytes;

                renderedBytes = sr.Render("Excel");

                //Crea el Excel.
                bytes = sr.Render("Excel", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = "C:\\BackUp\\Reporting Services\\ReportTicket\\" + DES_FILE + ".xls";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                Console.WriteLine("Se generó en archivo Excel en la ruta: " + filename);

                return filename;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generando el archivo Excel para el envio de correo del reporte de tickets diario");
                Console.WriteLine("Error generado: " + e.Message.ToString());
                return "ERROR";
            }
        }

        public ComGenerarTicketProblemas_Result GenerarTicketProblema()
        {
            return dbc.ComGenerarTicketProblemas().First();
        }

        //Funciones utilizados para el envío de encuestas de satisfacción 

        public string GeneraEncuesta(int idEncuestaConfiguracion)
        {
            var queryEncuestas = dbe.EncuestaConfiguracions.Where(x => x.IdEncuestaConfiguracion == idEncuestaConfiguracion && x.Estado == true).SingleOrDefault();
            int top = Convert.ToInt32(queryEncuestas.NroEncuestas);
            var query = dbc.TICKETs.Where(x => x.SEND_SURVEY == false && x.ID_ACCO == queryEncuestas.IdAcco.Value)
                .Where(x => x.ID_STAT_END == 6)
                .Where(x => x.ID_TYPE_FORM == null)
                .Take(top).ToList();
            if (query.Count() != 0)
            {
                foreach (var ti in query.Take(10))
                {
                    if (idEncuestaConfiguracion == 5)
                    {
                        List<int> compList = new List<int>(new[] { 67136, 71236, 71251, 71249, 71254, 71260, 71266, 71263, 71241, 71245, 71243, 71269, 71257 });
                        if (ti.ID_QUEU == 15 && compList.Contains(ti.ID_COMP_END.Value))
                        {
                            EnviarFormularioGoogle(ti);
                            //var fechaUltimoEnvio = dbe.ActualizarFechaUltimaEnvio(idEncuestaConfiguracion);
                        }

                    }
                    else
                    {
                        int ID_SURV_TICK = InsertaPreguntas(ti);
                        if (ID_SURV_TICK != 0 && ID_SURV_TICK > 0)
                        {
                            EnviarEncuesta(ti, ID_SURV_TICK);
                            var fechaUltimoEnvio = dbe.ActualizarFechaUltimaEnvio(idEncuestaConfiguracion);
                        }
                        else
                            if (ID_SURV_TICK == -1)
                        {
                            return "2";
                        }
                    }

                }

                return "";
            }
            else
            {
                return "0";
            }
        }

        private int InsertaPreguntas(TICKET tic)
        {
            //int ID_SURV = 2;
            try
            {
                //var survey = dbe.SURVEY_ACCOUNT_TYPE_TICKET.Where(x => x.ID_ACCO == tic.ID_ACCO.Value && x.ID_TYPE_TICK == tic.ID_TYPE_TICK.Value && x.DATE_TO == null).SingleOrDefault();
                var survey = dbe.SURVEY_ACCOUNT_TYPE_TICKET
                    .Where(x => x.ID_ACCO == tic.ID_ACCO.Value)
                    .Where(x => x.ID_TYPE_TICK == tic.ID_TYPE_TICK.Value).SingleOrDefault(x => x.DATE_TO == null);

                if (survey != null)
                {

                    //int c = 0;
                    var c_st = dbe.SURVEY_TICKET.Where(x => x.ID_TICK == tic.ID_TICK);
                    int c = c_st.Count();

                    if (c_st.Count() == 0)
                    {

                        //Insertamos Survey Ticket
                        SURVEY_TICKET st = new SURVEY_TICKET();
                        st.ID_TICK = tic.ID_TICK;
                        st.ID_SURV = survey.ID_SURV;
                        st.ID_SURV_STAT = 1;
                        st.SEN_REPO = false;
                        st.CREATED = DateTime.Now;

                        dbe.SURVEY_TICKET.Add(st);
                        dbe.SaveChanges();

                        //Insertar Detalles de Encuesta
                        var ques = (from q in dbe.QUESTIONs
                                    join qg in dbe.QUESTION_GROUP.Where(x => x.ID_SURVEY == survey.ID_SURV) on q.ID_QUES_GROU equals qg.ID_QUES_GROU
                                    select new
                                    {
                                        ID_QUES = q.ID_QUES
                                    }).ToList();

                        foreach (var q in ques)
                        {
                            //Insertamos preguntas de Survey
                            QUESTION_TICKET qt = new QUESTION_TICKET();
                            qt.ID_QUES = q.ID_QUES;
                            qt.ID_SURV_TICK = st.ID_SURV_TICK;
                            qt.CREATED = DateTime.Now;
                            dbe.QUESTION_TICKET.Add(qt);
                            dbe.SaveChanges();
                        }

                        //Inserta Actividad
                        SURVEY_TICKET_ACTIVITY sta = new SURVEY_TICKET_ACTIVITY();
                        sta.ID_SURV_TICK = st.ID_SURV_TICK;
                        sta.ID_SURV_STAT = 1;
                        sta.CREATED = DateTime.Now;
                        dbe.SURVEY_TICKET_ACTIVITY.Add(sta);
                        dbe.SaveChanges();

                        return st.ID_SURV_TICK;
                    }
                    else
                    {
                        return c_st.First().ID_SURV_TICK;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = ex.Message;
                exc.DES_EXCE = "Tickets - InsertaPreguntas";
                dbl.EXCEPTIONs.Add(exc);
                dbl.SaveChanges();
                return 0;
            }
        }

        public bool EnviarEncuesta(TICKET tic, int ID_SURV_TICK)
        {
            try
            {
                var pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == tic.ID_PERS_ENTI);
                var ce = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == pe.ID_ENTI2.Value);
                int cq = dbe.QUESTION_TICKET.Where(x => x.ID_SURV_TICK == ID_SURV_TICK).Count();

                if (!String.IsNullOrEmpty(pe.EMA_PERS))
                {
                    BodySurvey bodySurvey = new BodySurvey();
                    SendMail newMail = new SendMail();
                    var msg = newMail.mailMessage;
                    var html_msg = bodySurvey.htmlSendSurvey(pe, ce, ID_SURV_TICK, cq, tic.ID_ACCO.Value);

                    string text = (ce.SEX_ENTI == "M" ? "Estimado " : ce.SEX_ENTI == "F" ? "Estimada " : "") + textInfo.ToTitleCase(ce.FIR_NAME.ToLower()) + ", " +
                        "Nuestro principal objetivo es brindarle un mejor servicio, por eso solicitamos tu ayuda completando la siguiente encuesta que consta de " + Convert.ToString(cq) + " preguntas" +
                        "Por favor copie y pege el siguiente link en su navegador web: " + IpServer + "/Survey/Index?idst=" + System.Web.HttpUtility.UrlEncode(seg.Encrypt(Convert.ToString(ID_SURV_TICK)));

                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                    string xx = Path.GetTempPath();

                    msg.AlternateViews.Add(plainView);
                    msg.AlternateViews.Add(htmlView);

                    msg.Subject = "Encuesta de Satisfacción";
                    msg.To.Add(pe.EMA_PERS.Trim());

                    newMail.smtp.Send(msg);

                    //Actualiza Ticket
                    TICKET ticket = dbc.TICKETs.Single(x => x.ID_TICK == tic.ID_TICK);
                    dbc.Entry(ticket).State = EntityState.Modified;
                    ticket.SEND_SURVEY = true;
                    dbc.SaveChanges();

                    //Inserta Actividad
                    SURVEY_TICKET_ACTIVITY sta = new SURVEY_TICKET_ACTIVITY();
                    sta.ID_SURV_TICK = ID_SURV_TICK;
                    sta.ID_SURV_STAT = 2;
                    sta.CREATED = DateTime.Now;
                    dbe.SURVEY_TICKET_ACTIVITY.Add(sta);
                    dbe.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = ex.Message;
                exc.DES_EXCE = "Tickets - EnviarEncuesta";
                dbl.EXCEPTIONs.Add(exc);
                dbl.SaveChanges();
                return false;
            }
        }

        public bool EnviarFormularioGoogle(TICKET tic)
        {
            try
            {

                var pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == tic.ID_PERS_ENTI_END);
                var pes = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == tic.ID_PERS_ENTI);
                var ce = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == pe.ID_ENTI2.Value);

                if (!String.IsNullOrEmpty(pe.EMA_PERS))
                {
                    BodySurvey bodySurvey = new BodySurvey();
                    SendMail newMail = new SendMail();
                    var msg = newMail.mailMessage;
                    var html_msg = bodySurvey.htmlSendFormEncuestaSOC(ce, tic.SUM_TICK, tic.ID_ACCO.Value);

                    string text = (ce.SEX_ENTI == "M" ? "Estimado " : ce.SEX_ENTI == "F" ? "Estimada " : "") + textInfo.ToTitleCase(ce.FIR_NAME.ToLower()) + ", " +
                        "Nuestro principal objetivo es brindarle un mejor servicio, por eso, por favor agradecemos que responda el breve cuestionario de satisfacción por el cierre de atención del ticket:\n" +
                        tic.SUM_TICK +
                        "\n Por favor ingrese al siguiente formulario: https://forms.gle/QzFpsGx1cJd8ov3L6" +
                        "\n Su respuesta será para ayudarnos con el feedback del servicio para nuestros procesos de calidad de experiencia de cliente.Gracias.";

                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                    string xx = Path.GetTempPath();

                    msg.AlternateViews.Add(plainView);
                    msg.AlternateViews.Add(htmlView);

                    msg.Subject = tic.COD_TICK + " - Encuesta de Satisfacción ITMS";
                    msg.To.Add(pe.EMA_PERS.Trim());
                    //msg.CC.Add(pe.EMA_PERS.Trim());

                    newMail.smtp.Send(msg);

                    //Actualiza Ticket
                    TICKET ticket = dbc.TICKETs.Single(x => x.ID_TICK == tic.ID_TICK);
                    dbc.Entry(ticket).State = EntityState.Modified;
                    ticket.SEND_SURVEY = true;
                    dbc.SaveChanges();

                    ////Inserta Actividad
                    //SURVEY_TICKET_ACTIVITY sta = new SURVEY_TICKET_ACTIVITY();
                    //sta.ID_SURV_TICK = ID_SURV_TICK;
                    //sta.ID_SURV_STAT = 2;
                    //sta.CREATED = DateTime.Now;
                    //dbe.SURVEY_TICKET_ACTIVITY.Add(sta);
                    //dbe.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var exc = new EXCEPTION();
                exc.DAT_EXCE = DateTime.Now;
                exc.MESSAGE = ex.Message;
                exc.DES_EXCE = "Tickets - EnviarEncuesta";
                dbl.EXCEPTIONs.Add(exc);
                dbl.SaveChanges();
                return false;
            }
        }

        public string EnviarRptActividades()
        {
            try
            {
                string file = RptActividadesArchivoPDF();

                TicketTemplate _body = new TicketTemplate();
                var html_msg = _body.PlantillaRptActividades();

                if (html_msg != null && file != "ERROR")
                {
                    Console.WriteLine("Enviando Correo...");
                    SendMail newMail = new SendMail();
                    var msg = newMail.mailMessage;
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                    msg.AlternateViews.Add(htmlView);

                    msg.Subject = "Reporte Semanal de Actividades " + DateTime.Now.AddDays(-7) + " - " + DateTime.Now;
                    msg.To.Add("jquisper@electrodata.com.pe");
                    ////////////msg.To.Add("fsandoval@electrodata.com.pe");
                    ///////////msg.CC.Add("mcatacora@electrodata.com.pe");



                    Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                    msg.Attachments.Add(attachment);
                    newMail.smtp.Send(msg);

                    Console.WriteLine("Envio exitoso");
                    Console.WriteLine();
                    return "OK";
                }
                else
                {
                    Console.WriteLine("Existió un error al generar la archivo Excel o la plantilla");
                    return "ERROR";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error enviando correo de reporte semanal de actividades");
                Console.WriteLine("Error generado: " + e.Message.ToString());

                return "ERROR";
            }
        }

        //ReporteBacklogTicket
        //public string EnviarRptBacklogTicket()
        //{
        //    try
        //    {
        //        string file = RptBacklogTicketsArchivoExcel();

        //        TicketTemplate _body = new TicketTemplate();
        //        var html_msg = _body.PlantillaRptBacklog();

        //        if (html_msg != null && file != "ERROR")
        //        {
        //            Console.WriteLine("Enviando Correo...");
        //            SendMail newMail = new SendMail();
        //            var msg = newMail.mailMessage;
        //            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

        //            msg.AlternateViews.Add(htmlView);

        //            msg.Subject = "Reporte Semanal del Backlog de Tickets" + DateTime.Now.AddDays(-7) + " - " + DateTime.Now;
        //            msg.To.Add("jquisper@electrodata.com.pe");
        //            ////////////msg.To.Add("fsandoval@electrodata.com.pe");
        //            ///////////msg.CC.Add("mcatacora@electrodata.com.pe");



        //            Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
        //            msg.Attachments.Add(attachment);
        //            newMail.smtp.Send(msg);

        //            Console.WriteLine("Envio exitoso");
        //            Console.WriteLine();
        //            return "OK";
        //        }
        //        else
        //        {
        //            Console.WriteLine("Existió un error al generar la archivo Excel o la plantilla");
        //            return "ERROR";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error enviando correo de reporte semanal de actividades");
        //        Console.WriteLine("Error generado: " + e.Message.ToString());

        //        return "ERROR";
        //    }
        //}

        //Creación de archivo Excel de Reporte de Tickets.
        public string RptActividadesArchivoPDF()
        {
            try
            {
                Console.WriteLine("Generando archivo. Por favor espere...");
                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                ServerReport sr = new ServerReport();
                sr.ReportServerCredentials = new CredencialesReporting("Administrator", "ITMS$15DEV$", "");
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/Actividad/RptTipoActividad";

                string cbUsuario = "0";
                string dtFechaInicio = Convert.ToString(DateTime.Now.AddDays(-7));
                string dtFechaFin = Convert.ToString(DateTime.Now);
                string ID_PERS_ENTI = "1007";
                string ID_ACCO = "4";

                DateTime desde = DateTime.Now.AddDays(-7);
                DateTime hasta = DateTime.Now;

                string DES_FILE = "ReportSemanalActividades" + desde.Year + desde.Month + desde.Day + "-" + hasta.Year + hasta.Month + hasta.Day;

                ReportParameter[] param = new ReportParameter[15];
                param[0] = new ReportParameter("ID_PERS_ENTI", ID_PERS_ENTI);
                param[1] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[2] = new ReportParameter("IdUsuario", cbUsuario);
                param[3] = new ReportParameter("FechaInicio", desde.Year + "-" + desde.Month + "-" + desde.Day);
                param[4] = new ReportParameter("FechaFin", hasta.Year + "-" + hasta.Month + "-" + hasta.Day);
                param[5] = new ReportParameter("TA1", "0");
                param[6] = new ReportParameter("TA2", "0");
                param[7] = new ReportParameter("TA3", "0");
                param[8] = new ReportParameter("TA4", "0");
                param[9] = new ReportParameter("TA5", "0");
                param[10] = new ReportParameter("AR1", "0");
                param[11] = new ReportParameter("AR2", "0");
                param[12] = new ReportParameter("AR3", "0");
                param[13] = new ReportParameter("AR4", "0");
                param[14] = new ReportParameter("AR5", "0");

                sr.SetParameters(param);
                sr.Refresh();

                byte[] renderedBytes;

                renderedBytes = sr.Render("Excel");

                //Crea el Excel.
                bytes = sr.Render("Excel", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = "C:\\BackUp\\Reporting Services\\RptSemanalActividades\\" + DES_FILE + ".xls";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                Console.WriteLine("Se generó en archivo Excel en la ruta: " + filename);

                return filename;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generando el archivo Excel para el envio de correo del reporte semanal de actividades");
                Console.WriteLine("Error generado: " + e.Message.ToString());
                return "ERROR";
            }
        }

        //Creación de archivo Excel de Reporte Backlog de Tickets.
        public string RptBacklogTicketsArchivoExcel()
        {
            try
            {
                Console.WriteLine("Generando archivo. Por favor espere...");
                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                ServerReport sr = new ServerReport();
                sr.ReportServerCredentials = new CredencialesReporting("Administrator", "ITMS$15DEV$", "");
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/Ticket/RptBacklog";

                // falta cambiar parametros
                string IdArea = "0";
                string dtFechaInicio = Convert.ToString(DateTime.Now.AddDays(-7));
                string dtFechaFin = Convert.ToString(DateTime.Now);
                string IdTipo = "0";
                string ID_ACCO = "60";
                string IdJef = "0";

                DateTime desde = DateTime.Now.AddDays(-7);
                DateTime hasta = DateTime.Now;

                string DES_FILE = "ReportBacklog" + desde.Year + desde.Month + desde.Day + "-" + hasta.Year + hasta.Month + hasta.Day;

                // falta cambiar parametros
                ReportParameter[] param = new ReportParameter[6];
                param[0] = new ReportParameter("IdAcco", ID_ACCO);
                param[1] = new ReportParameter("FechaInicio", dtFechaInicio);
                param[2] = new ReportParameter("FechaFin", dtFechaFin);
                param[3] = new ReportParameter("IdArea", IdArea);
                param[4] = new ReportParameter("IdTypeTick", IdTipo);
                param[5] = new ReportParameter("IdJefatura", IdJef);


                sr.SetParameters(param);
                sr.Refresh();

                byte[] renderedBytes;

                renderedBytes = sr.Render("Excel");

                //Crea el Excel.
                bytes = sr.Render("Excel", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = "C:\\BackUp\\Reporting Services\\RptSemanalBacklog\\" + DES_FILE + ".xls";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                Console.WriteLine("Se generó en archivo Excel en la ruta: " + filename);

                return filename;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generando el archivo Excel para el envio de correo del reporte semanal de actividades");
                Console.WriteLine("Error generado: " + e.Message.ToString());
                return "ERROR";
            }
        }

        public string NotificacionTicketActivo()
        {
            try
            {
                string file = "";
                List<TicketActivoEntrega_Result> tickets = dbc.TicketActivoEntrega().ToList();

                var listaUsuarios = tickets.Select(s => s.IdUsuario).Distinct();
                var html_msg = "";

                foreach (var usuario in listaUsuarios)
                {
                    SendMail newMail1 = new SendMail();
                    var msg1 = newMail1.mailMessage;
                    AlternateView htmlView1 = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                    msg1.AlternateViews.Add(htmlView1);

                    var Plantilla1 = tickets.Where(x => x.IdUsuario == usuario && x.TipoPlantilla == 1).ToList();

                    foreach (var ticket in Plantilla1)
                    {
                        Attachment attachment = new Attachment(ArchivoPDFEntregaActivoFormato(ticket.ID_TICK), MediaTypeNames.Application.Octet);
                        msg1.Attachments.Add(attachment);

                        msg1.Subject = "Solicitud: Devolución de formato de asignación de activos firmados - " + ticket.Usuario;
                    }

                    html_msg = "Estimado/a:<BR><BR>" +
                            @"El motivo de la presente es para hacerle llegar el formato de asignación de los activos que se encuentra pendiente por su parte.<BR><BR>" +
                            @"Solicitamos por favor, enviar el formato adjunto firmado mediante el siguiente enlace: LINK DEL FORM<BR><BR>" +
                            @"Si tiene alguna duda o consulta, por favor comunicarse con <b>servicedesk@electrodata.com.pe<b/><BR><BR>" +
                            @"NOTA: Recuerde que este proceso se encuentra expuesto en la política de la empresa y su incumplimiento es causal de Memorándum.<BR><BR>";

                    msg1.To.Add("jquisper@electrodata.com.pe");
                    //msg.To.Add(item.CorreoUsuario);
                    //msg.CC.Add(item.CorreoJefe);
                    if (Plantilla1.Count() > 0)
                    {
                        newMail1.smtp.Send(msg1);
                        foreach (var ticket in Plantilla1)
                        {
                            TICKET_NOTIFICATION registro = new TICKET_NOTIFICATION();
                            registro.ID_TICK = ticket.ID_TICK;
                            registro.ID_TYPE_NOTI = 7;
                            registro.CREATED = DateTime.Now;
                            dbc.TICKET_NOTIFICATION.Add(registro);
                            dbc.SaveChanges();
                        }
                    }

                    /////////////////////////////////////////////////

                    SendMail newMail2 = new SendMail();
                    var msg2 = newMail2.mailMessage;
                    AlternateView htmlView2 = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                    msg2.AlternateViews.Add(htmlView2);

                    var Plantilla2 = tickets.Where(x => x.IdUsuario == usuario && x.TipoPlantilla == 2).ToList();
                    string nombre = "";
                    foreach (var ticket in Plantilla2)
                    {
                        Attachment attachment = new Attachment(ArchivoPDFEntregaActivoFormato(ticket.ID_TICK), MediaTypeNames.Application.Octet);
                        msg2.Attachments.Add(attachment);

                        msg2.Subject = "Solicitud: Devolución de formato de asignación de activos firmados - " + ticket.Usuario;
                        nombre = ticket.Usuario;
                    }

                    html_msg = "Estimado/a:<BR><BR>" +
                                @"El motivo de la presente es para comunicarle que el colaborador " + nombre + " a la fecha no ha enviado el formato de recepción de activos firmado.<BR><BR>" +
                                @"Solicitamos por favor, enviar el formato adjunto firmado mediante el siguiente enlace: LINK DEL FORM<BR><BR>" +
                                @"Si tiene alguna duda o consulta, por favor comunicarse con <b>servicedesk@electrodata.com.pe</b><BR><BR>" +
                                @"NOTA: Recuerde que este proceso se encuentra expuesto en la política de la empresa y su incumplimiento es causal de Memorándum al colaborador a cargo.<BR><BR>";

                    //msg.To.Add(item.CorreoJefe);
                    //msg.CC.Add(item.CorreoUsuario);
                    msg2.To.Add("jquisper@electrodata.com.pe");

                    if (Plantilla2.Count() > 0)
                    {
                        newMail2.smtp.Send(msg2);
                        foreach (var ticket in Plantilla2)
                        {
                            TICKET_NOTIFICATION registro = new TICKET_NOTIFICATION();
                            registro.ID_TICK = ticket.ID_TICK;
                            registro.ID_TYPE_NOTI = 7;
                            registro.CREATED = DateTime.Now;
                            dbc.TICKET_NOTIFICATION.Add(registro);
                            dbc.SaveChanges();
                        }
                    }

                }

                return "OK";
            }
            catch (Exception e)
            {
                Console.WriteLine("Error enviando correo de reporte semanal de actividades");
                Console.WriteLine("Error generado: " + e.Message.ToString());

                return "ERROR";
            }

        }

        public string ArchivoPDFEntregaActivoFormato(int idTicket)
        {
            try
            {
                Console.WriteLine("Generando archivo. Por favor espere...");
                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                ServerReport sr = new ServerReport();
                sr.ReportServerCredentials = new CredencialesReporting("Administrador", "C4goldP3", "");
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/ReportedeliveryED";

                string strFecha = DateTime.Now.ToString("yyyyMMddHHmmss");

                string DES_FILE = "TicketEntrega" + strFecha;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("ID_TICK", idTicket.ToString());

                sr.SetParameters(param);
                sr.Refresh();

                byte[] renderedBytes;

                renderedBytes = sr.Render("PDF");

                //Crea el Excel.
                bytes = sr.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                string filename = "C:\\BackUp\\ReportingServices\\ActivoEntrega\\" + DES_FILE + ".pdf";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                Console.WriteLine("Se generó en archivo PDF en la ruta: " + filename);

                return filename;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generando el archivo PDF para el envio de correo de ");
                Console.WriteLine("Error generado: " + e.Message.ToString());
                return "ERROR";
            }
        }

    }
}