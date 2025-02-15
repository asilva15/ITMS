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
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.IO;
using System.Reflection;


namespace ConsoleNotification.Controllers
{
    //Clase utilizada pare el envío de correos referente al envío de correos para el módulo de Finanzas (Caja Chica y Viáticos)
    class Financial
    {
        //Declaracion de Objetos para acceso a base de datos.
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        public AppLogEntities dbl = new AppLogEntities();      
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        //Función para buscar Solicitude de gasto y sus actividades.
        public string Accountability()
        {
            try
            {
                string msg = "";

                //Solicitudes que tienen pendiente el envío de correo electrónico
                var qacc = (from re in dbc.REQUEST_EXPENSE.Where(x => x.SEND_MAIL == false)
                            select new
                            {
                                re.ID_REQU_EXPE,

                            }).ToList();

                if (qacc.Count() > 0)
                {
                    //Se recorre cada solicitud para enviar correo.
                    foreach (var q in qacc)
                    {
                        REQUEST_EXPENSE re = new REQUEST_EXPENSE();
                        re = dbc.REQUEST_EXPENSE.Find(q.ID_REQU_EXPE);
                        msg = RequestAccountability(re);
                   
                        if (msg == "OK")
                        {
                            re.SEND_MAIL = true;
                            re.CREATED_MAIL = DateTime.Now;
                            dbc.REQUEST_EXPENSE.Attach(re);
                            dbc.Entry(re).State = EntityState.Modified;
                            dbc.SaveChanges();
                        }
                    }

                    return "OK";
                }

                //Buscamos las actividades pendientes de envío de correo
                var actiReEx = (from re in dbc.REQUEST_EXPENSE_ACTIVITY.Where(x => x.SEND_MAIL == false)
                            select new
                            {
                                re.ID_REQU_EXPE,
                                re.ID_REQU_EXPE_ACTI
                            }).ToList();

                if (actiReEx.Count() > 0)
                {
                    //Se recorre cada actividad para enviar correos.
                    foreach (var q in actiReEx)
                    {
                        REQUEST_EXPENSE re = new REQUEST_EXPENSE();
                        re = dbc.REQUEST_EXPENSE.Find(q.ID_REQU_EXPE);

                        //Creación de cuerpo de correo y envío.
                        msg = RequestAccountability(re);

                        if (msg == "OK")
                        {
                            //Actualización de actividades después del envío saisfactorio de correo.
                            REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                            rea = dbc.REQUEST_EXPENSE_ACTIVITY.Find(q.ID_REQU_EXPE_ACTI);
                            rea.SEND_MAIL = true;
                            dbc.REQUEST_EXPENSE_ACTIVITY.Attach(rea);
                            dbc.Entry(rea).State = EntityState.Modified;
                            dbc.SaveChanges();
                        }
                    }

                    return "OK";
                }
                else
                {
                    return "No hubo ningún correo de Accountability para enviar";
                }
            }
            catch(Exception e)
            {
                return "Error al enviar los correos de Accountability";
            }         

        }

        public string ContablidadDevolucionUsuario()
        {
            try
            {
                string msg = "";

                //Solicitudes que tienen pendiente el envío de correo electrónico
                var qacc = (from ds in dbc.REQUEST_EXPENSE.Where(x => x.SEND_MAIL_USUARIO_DEVUELVE == false)
                            select new
                            {
                                ds.ID_REQU_EXPE,

                            }).ToList();

                if (qacc.Count() > 0)
                {
                    //Se recorre cada solicitud para enviar correo.
                    foreach (var q in qacc)
                    {
                        REQUEST_EXPENSE re = new REQUEST_EXPENSE();
                        re = dbc.REQUEST_EXPENSE.Find(q.ID_REQU_EXPE);
                        msg = RespuestaContablidadDevolucionUsuario(re);

                        if (msg == "OK")
                        {
                            re.SEND_MAIL_USUARIO_DEVUELVE = true;
                            re.CREATED_MAIL_USUARIO_DEVUELVE = DateTime.Now;
                            dbc.REQUEST_EXPENSE.Attach(re);
                            dbc.Entry(re).State = EntityState.Modified;
                            dbc.SaveChanges();
                        }
                    }

                    return "OK";
                }


                return "OK";
            }
            catch (Exception e)
            {
                return "Error al enviar los correos de Accountability";
            }

        }

        
        //Funcion encargada de crear y enviar correo.
        public string RequestAccountability(REQUEST_EXPENSE re)
        {
            textInfo = cultureInfo.TextInfo;

            try
            {
                //Consultmos la solicitus: Estado, Tipo, Código.
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

                //Consultamos Solicitante y Aprobador.
                PERSON_ENTITY requester = null, approval = null, paga = null;
                string msgTo = null, msgCC = null, msgTo2 = "", msgPagaViat = "", msgPagaCaja = "",msgApruebaConta="";

                //Solicitante.
                requester = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_REQU.Value);

                //Aprobador.
                approval = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_APPR.Value);


                var person = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == requester.ID_ENTI2);

                try
                {

                    if (re.ID_PERS_ENTI_ASSI != null)
                    {
                        paga = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_ASSI);
                        msgPagaCaja = (String.IsNullOrEmpty(paga.EMA_ELEC) == true ? "" : paga.EMA_ELEC);
                    }
                    else
                    {
                        int id_pers_paga = Convert.ToInt32((from ce in dbe.CLASS_ENTITY
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join pc in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true) on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI into pcGroup
                         from pc in pcGroup.DefaultIfEmpty()
                         join c in dbe.CHARTs on pc.ID_CHAR equals c.ID_CHAR into cGroup
                         from c in cGroup.DefaultIfEmpty()
                         join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR into ncGroup
                         from nc in ncGroup.DefaultIfEmpty()
                         where nc.NAM_CHAR.Contains("tesorer")
                                 && pe.VIG_PERS_ENTI == true
                                 && ce.VIG_ENTI == true
                                 && c.ID_ACCO == 4
                         select new { ID_PERS_ENTI = pe.ID_PERS_ENTI }).Single().ID_PERS_ENTI);

                        paga = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == id_pers_paga);
                        msgPagaCaja = (String.IsNullOrEmpty(paga.EMA_ELEC) == true ? "" : paga.EMA_ELEC);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Viatical -> " + e.Message.ToString());
                }

                try
                {

                        if (re.ID_PERS_ENTI_APPR_VIAT != null)
                        {
                            var personV = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == re.ID_PERS_ENTI_APPR_VIAT.Value);
                            msgTo2 = (String.IsNullOrEmpty(personV.EMA_ELEC) == true ? "" : personV.EMA_ELEC);
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
                catch (Exception e)
                {
                    Console.WriteLine("Viatical -> " + e.Message.ToString());
                }

                //En el Caso sea caja chica, verificamos su estado y configuramos destinatarios.
                switch (re.ID_STAT_REQU_EXPE)
                {
                    case 1:
                        msgTo = approval.EMA_ELEC;
                        msgCC = requester.EMA_ELEC;
                        //if (re.ID_PERS_ENTI_APPR_VIAT != null)
                        //{
                        //    msgCC = msgCC + "," + msgTo2.ToString();

                        //}
                        /* if (re.ID_PERS_ENTI_APPR == 37361 || re.ID_PERS_ENTI_APPR == 29311 || re.ID_PERS_ENTI_APPR == 233)
                         {
                             msgCC = msgCC + ",cbutrica@electrodata.com.pe";
                         }*/
                        break;
                    case 2:
                        msgTo = msgPagaCaja;
                        msgCC = requester.EMA_ELEC + (String.IsNullOrEmpty(approval.EMA_ELEC) == true ? "" : "," + approval.EMA_ELEC);

                        break;
                    case 3:
                        msgTo = requester.EMA_ELEC;
                        msgCC = approval.EMA_ELEC + "," + msgPagaCaja;
                        break;
                    case 4:
                        msgTo = approval.EMA_ELEC;
                        msgCC = requester.EMA_ELEC + (String.IsNullOrEmpty(msgPagaCaja) == true ? "" : "," + msgPagaCaja);
                        break;
                    case 5:
                        msgTo = requester.EMA_ELEC;
                        msgCC = approval.EMA_ELEC;
                        break;
                    case 6:
                        msgTo = msgApruebaConta;
                        msgCC = requester.EMA_ELEC + (String.IsNullOrEmpty(msgPagaCaja) == true ? "" : "," + msgPagaCaja);
                        break;
                    case 7:
                        msgTo = requester.EMA_ELEC;
                        msgCC = approval.EMA_ELEC;
                        break;
                    case 8:
                        msgTo = requester.EMA_ELEC;
                        msgCC = approval.EMA_ELEC;
                        break;
                    case 9:
                        msgTo = (String.IsNullOrEmpty(msgPagaCaja) == true ? "" : msgPagaCaja);
                        msgCC = requester.EMA_ELEC + "," + approval.EMA_ELEC;
                        break;
                    default:
                        msgTo = requester.EMA_ELEC;
                        msgCC = approval.EMA_ELEC + (String.IsNullOrEmpty(msgPagaViat) == true ? "" : "," + msgPagaViat);
                        break;
                }

                //Creamos cuerpo de correo.
                FinancialTemplate body = new FinancialTemplate();
                string body_html = body.PettyCash(re);

                Attachment pdfAttachment = null;

                if ((re.ID_STAT_REQU_EXPE == 5 && re.ID_TYPE_DELI_SUST == 2 && re.ID_DELI_SUST != null) || (re.ID_STAT_REQU_EXPE == 7 && re.ID_TYPE_DELI_SUST == 1 && re.ID_DELI_SUST != null))
                {
                    //Adjuntamos el documento de pago
                    int id_deli_sust = Convert.ToInt32(re.ID_DELI_SUST);

                    //int ID_TYPE_DELI_SUST = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == idRequest).Single().ID_TYPE_DELI_SUST);
                    FileStream fileStream = null;

                    string id = dbc.ATTACHEDs.Where(x => x.ID_DELI_SUST == id_deli_sust).Single().NAM_ATTA + "_" + dbc.ATTACHEDs.Where(x => x.ID_DELI_SUST == id_deli_sust).Single().ID_ATTA + dbc.ATTACHEDs.Where(x => x.ID_DELI_SUST == id_deli_sust).Single().EXT_ATTA;

                    string pdfPath = Path.Combine("..", "..", "..", "ERPElectrodata", "Adjuntos", "Depositos", id);

                    string absolutePath = Path.GetFullPath(pdfPath);

                    //string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaPDF);

                    if (File.Exists(absolutePath))
                    {
                        // Puedes abrir el archivo PDF aquí
                        // Por ejemplo, puedes usar File.OpenRead(fullPath) para abrirlo
                        fileStream = System.IO.File.OpenRead(absolutePath);
                    }
                    else
                    {
                        Console.WriteLine("El archivo PDF no se encuentra en la ubicación especificada.");
                    }

                    MemoryStream storeStream = new MemoryStream();

                    storeStream.SetLength(fileStream.Length);
                    fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                    byte[] byteArray = storeStream.ToArray();

                    pdfAttachment = new Attachment(new MemoryStream(byteArray), "ComprobantePago.pdf", "application/pdf");
                }

                //Creamos el objeto correo electrónico.
                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                //Se configuran parámetros y se envían correos.
                message.To.Add(msgTo);
                message.CC.Add(msgCC);
                if (pdfAttachment != null)
                {
                    message.Attachments.Add(pdfAttachment);
                }
                message.AlternateViews.Add(htmlView);
                message.Subject = query.NAM_TYPE_DELI_SUST + " (" + textInfo.ToTitleCase(person.FIR_NAME.ToLower() + " " + person.LAS_NAME.ToLower()) + ") - " + (re.CURRENCY == "MN" ? "PEN " : "USD ") + String.Format("{0:0.00}", re.AMOUNT) + " " + query.NAM_STAT_REQU_EXPE;
                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                Console.WriteLine("Viatical -> " + e.Message.ToString());
                return e.Message;
            }
        }

        public string RespuestaContablidadDevolucionUsuario(REQUEST_EXPENSE rs)
        {
            textInfo = cultureInfo.TextInfo;

            try
            {
                //Consultmos la solicitus: Estado, Tipo, Código.
                var query = (from xds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == rs.ID_DELI_SUST).ToList()
                             join x in dbc.REQUEST_EXPENSE on xds.ID_DELI_SUST equals x.ID_DELI_SUST
                             join sre in dbc.STATUS_REQUEST_EXPENSE on x.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                             join tds in dbc.TYPE_DELI_SUST on x.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                             select new
                             {
                                 NAM_STAT_REQU_EXPE = sre.NAM_STAT_REQU_EXPE,
                                 NAM_STAT_REQU_EXPE_SPAN = sre.NAM_STAT_REQU_EXPE_SPAN,
                                 NAM_TYPE_DELI_SUST = tds.NAM_TYPE_DELI_SUST,
                                 COD_REQU_EXPE = x.COD_REQU_EXPE,
                                 NAM_TYPE_DELI_SUST_SPAN = tds.NAM_TYPE_DELI_SUST_SPAN
                             }).First();

                //Consultamos Solicitante y Aprobador.
                PERSON_ENTITY requester = null, approval = null, paga = null;
                string msgTo = null, msgCC = null, msgTo2 = "", msgPagaViat = "", msgPagaCaja = "";

                //int ID_PERS_ENTI_SOLICITANTE = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_DELI_SUST == rs.ID_DELI_SUST).Single().ID_PERS_ENTI_REQU);
                //int ID_PERS_ENTI_APROBADOR = Convert.ToInt32(dbc.REQUEST_EXPENSE.Where(x => x.ID_DELI_SUST == ds.ID_DELI_SUST).Single().ID_PERS_ENTI_APPR);

                //Solicitante.
                requester = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == rs.ID_PERS_ENTI_REQU);

                //Aprobador.
                approval = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == rs.ID_PERS_ENTI_APPR);

                var person = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == requester.ID_ENTI2);

                try
                {

                    if (rs.ID_PERS_ENTI_ASSI != null)
                    {
                        paga = dbe.PERSON_ENTITY.First(x => x.ID_PERS_ENTI == rs.ID_PERS_ENTI_ASSI);
                        msgPagaCaja = (String.IsNullOrEmpty(paga.EMA_ELEC) == true ? "" : paga.EMA_ELEC);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Viatical -> " + e.Message.ToString());
                }

                try
                {

                    //Trabajo par el tipo de solicitud Viático.
                    if (rs.ID_TYPE_DELI_SUST == 2)
                    {
                        if (rs.ID_PERS_ENTI_ASSI != null)
                        {
                            var personV = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == rs.ID_PERS_ENTI_ASSI.Value);
                            msgTo2 = (String.IsNullOrEmpty(personV.EMA_ELEC) == true ? "" : personV.EMA_ELEC);
                        }

                        int ID_PAGA_VIAT = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.First(x => x.ID_PARA == 34).VAL_ACCO_PARA.ToString());
                        var PERS_PAGA_VIAT = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PAGA_VIAT);
                        msgPagaViat = (String.IsNullOrEmpty(PERS_PAGA_VIAT.EMA_ELEC) == true ? "" : PERS_PAGA_VIAT.EMA_ELEC);
                        //temporalmente se agrega a Elvira y Juan Campos
                        /*msgPagaViat = msgPagaViat*/ /*+ ",lmasias@electrodata.com.pe,zrivadeneyra@electrodata.com.pe,htaipe@electrodata.com.pe,cdonayre@electrodata.com.pe"*/
                        ;
                    }
                    else if (rs.ID_TYPE_DELI_SUST == 1)
                    {
                        if (rs.ID_PERS_ENTI_ASSI != null)
                        {
                            var personV = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == rs.ID_PERS_ENTI_ASSI.Value);
                            msgTo2 = (String.IsNullOrEmpty(personV.EMA_ELEC) == true ? "" : personV.EMA_ELEC);
                        }

                        int ID_PAGA_VIAT = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.First(x => x.ID_PARA == 34).VAL_ACCO_PARA.ToString());
                        var PERS_PAGA_VIAT = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PAGA_VIAT);
                        msgPagaViat = (String.IsNullOrEmpty(PERS_PAGA_VIAT.EMA_ELEC) == true ? "" : PERS_PAGA_VIAT.EMA_ELEC);
                        //temporalmente se agrega a Elvira y Juan Campos
                        /*msgPagaViat = msgPagaViat*/ /*+ ",lmasias@electrodata.com.pe,zrivadeneyra@electrodata.com.pe,htaipe@electrodata.com.pe,cdonayre@electrodata.com.pe"*/
                        ;
                    }
                    else
                    {
                        if (rs.ID_PERS_ENTI_ASSI != null)
                        {
                            var personV = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == rs.ID_PERS_ENTI_ASSI.Value);
                            msgTo2 = (String.IsNullOrEmpty(personV.EMA_ELEC) == true ? "" : personV.EMA_ELEC);
                        }

                        int ID_PAGA_VIAT = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.First(x => x.ID_PARA == 34).VAL_ACCO_PARA.ToString());
                        var PERS_PAGA_VIAT = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PAGA_VIAT);
                        msgPagaViat = (String.IsNullOrEmpty(PERS_PAGA_VIAT.EMA_ELEC) == true ? "" : PERS_PAGA_VIAT.EMA_ELEC);
                        //temporalmente se agrega a Elvira y Juan Campos
                        /*msgPagaViat = msgPagaViat*/ /*+ ",lmasias@electrodata.com.pe,zrivadeneyra@electrodata.com.pe,htaipe@electrodata.com.pe,cdonayre@electrodata.com.pe"*/
                        ;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Viatical -> " + e.Message.ToString());
                }

                //En el Caso sea caja chica, verificamos su estado y configuramos destinatarios.
                switch (rs.ID_TYPE_DELI_SUST)
                {
                    case 1:
                        msgTo = approval.EMA_ELEC;
                        msgCC = requester.EMA_ELEC;
                        if (rs.ID_PERS_ENTI_ASSI != null)
                        {
                            msgCC = msgCC + "," + msgTo2.ToString();

                        }
                        
                        break;

                    case 2:
                        msgTo = approval.EMA_ELEC;
                        msgCC = requester.EMA_ELEC;
                        if (rs.ID_PERS_ENTI_ASSI != null)
                        {
                            msgCC = msgCC + "," + msgTo2.ToString();

                        }
                        
                        break;

                    default:
                        msgTo = approval.EMA_ELEC;
                        msgCC = requester.EMA_ELEC;
                        if (rs.ID_PERS_ENTI_ASSI != null)
                        {
                            msgCC = msgCC + "," + msgTo2.ToString();

                        }
                        break;
                }

                

                //Creamos cuerpo de correo.
                FinancialTemplate body = new FinancialTemplate();
                string body_html = body.CuerpoContablidadDevolucionUsuario(rs);

                //Creamos el objeto correo electrónico.
                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                //Se configuran parámetros y se envían correos.
                message.To.Add(msgTo);
                message.CC.Add(msgCC);
                message.AlternateViews.Add(htmlView);
                //message.Attachments.Add(pdfAttachment);
                message.Subject = "¡ALERTA!: Saldo pendiente de devolución de " + query.NAM_TYPE_DELI_SUST + " (" + textInfo.ToTitleCase(person.FIR_NAME.ToLower() + " " + person.LAS_NAME.ToLower()) + ") - " + query.NAM_STAT_REQU_EXPE;
                newMail.smtp.Send(message);

                return "OK";
            }
            catch (Exception e)
            {
                Console.WriteLine("Viatical -> " + e.Message.ToString());
                return e.Message;
            }
        }


       

    }
}
