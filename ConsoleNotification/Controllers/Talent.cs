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
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Threading;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.IO;

namespace ConsoleNotification.Controllers
{
    //Clase encargada de enviar correos referente al módulo Talent Management.
    class Talent
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();
        public AssistanceEntities dba = new AssistanceEntities();
        string plantillahtml= "";
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        string reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();   

        //Función Para enviar correo por ser el cumpleaño de cada persona.
        public string HappyBirthday()
        {
            textInfo = cultureInfo.TextInfo;
            try
            {
                int year = DateTime.Now.Year;
                int mes = DateTime.Now.Month;
                int day = DateTime.Now.Day;

                //Listado de Personas que cumplen años en la fecha actual.
                var query = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.BIRTHDAY != null).ToList()
                             join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                             join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                             join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                             where ae.ID_ACCO == 4
                             where pe.ID_PERS_STAT == 1 
                             where pc.VIG_CONT==true && pc.LAS_CONT==true
                             where DateTime.Now >= pc.STAR_DATE 
                             where ce.BIRTHDAY.Value.Month == mes && ce.BIRTHDAY.Value.Day == day
                             //where pe.ID_PERS_ENTI == 10491 // ID_PERS_ENTI de persona que se quiere enviar un mensaje en caso no se envie.
                             select new
                             {
                                 pe.ID_PERS_ENTI,
                                 NOMBRE = ((ce.FIR_NAME==null?"":ce.FIR_NAME)+" "+ (ce.LAS_NAME==null?"":ce.LAS_NAME)).ToLower(),   
                             }).ToList();

                if (query.Count() > 0)
                {
                    //recorremos listado de personas.
                    for (int i = 0; i < query.Count; i++)
                    {
                        try
                        {
                            int ID_PERS_ENTI = Convert.ToInt32(query[i].ID_PERS_ENTI);
                            string NOMBRE = textInfo.ToTitleCase(Convert.ToString(query[i].NOMBRE));

                            //verificar si ya se ha enviado notificación.
                            int send = dbe.PERSON_ENTITY_NOTIFICATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                .Where(x => x.CREATED.Value.Year == year)
                                .Where(x => x.ID_PERS_NOTI == 1).Count();

                            if (send == 0)
                            {
                                //crear y enviar correo.
                                string HB = Cumple(ID_PERS_ENTI,NOMBRE);

                                //Si todo es conforme inserta registro para dejar conformidad.
                                if (HB == "OK")
                                {
                                    var perEntNot = new PERSON_ENTITY_NOTIFICATION();
                                    perEntNot.ID_PERS_ENTI = ID_PERS_ENTI;
                                    perEntNot.ID_PERS_NOTI = 1;
                                    perEntNot.CREATED = DateTime.Now;
                                    perEntNot.UserId = 29;
                                    dbe.PERSON_ENTITY_NOTIFICATION.Add(perEntNot);
                                    dbe.SaveChanges();
                                    Console.WriteLine("Se registró correctamente la notificación del envio del mensaje de cumpleaños");
                                }  
                            }
                        }
                        catch (Exception e)
                        {
                            var exc = new EXCEPTION();
                            exc.DAT_EXCE = DateTime.Now;
                            exc.MESSAGE = e.InnerException.Message;
                            exc.DES_EXCE = "Send Mail - HappyBirthday";
                            dbl.EXCEPTIONs.Add(exc);
                            dbl.SaveChanges();

                            Console.WriteLine("Error intentado enviar el correo de cumpleaños");
                            Console.WriteLine("Error generado: " + e.Message.ToString()); 
                        }
                    }
                    return "OK";
                }
                else
                {
                    return "No hay mensajes de cumpleaños para enviar ahora";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Hubo un error al enviar el mesaje de cumpleaños");
                Console.WriteLine("Error generado: " + e.Message.ToString());
                return "Hubo un error al enviar el mesaje de cumpleaños";
            }
        }

        //Función encargada de crear mail, contenido y destinatarios.
        public string Cumple(int ID_PERS_ENTI, string NOMBRE)
        {
            try
            {
                //Creamos cuerpo de correo.
                TalentTemplate body = new TalentTemplate();
                string body_html = body.TarjetaCumple(ID_PERS_ENTI);

                //Si el cuerpo del correo es conforme.
                if (body_html != null)
                {
                    //Creamos el objeto correo electrónico.
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    message.To.Add("electrodata_peru@electrodata.com.pe");
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                    message.AlternateViews.Add(htmlView);
                    message.Subject = "Feliz Cumpleaños "+ NOMBRE;
                    newMail.smtp.Send(message);

                    Console.WriteLine("Se envió correctamente el mensaje de cumpleaños del ID_PERS_ENTI: " + ID_PERS_ENTI);
                    return "OK";
                }
                else
                {
                    Console.WriteLine("No se ha podido generar el Template para el mensaje de cumpleaños del ID_PERS_ENTI: " + ID_PERS_ENTI);
                    return "ERROR";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo de Cumpleaños del ID_PERS_ENTI: " + ID_PERS_ENTI);
                Console.WriteLine("Error generado: " + ex.Message.ToString());
                return "ERROR";
            }
        }

        //Función encargada de enviar recordatorio de cumpleaños.
        public string RecordatorioHappyBirthday()
        {            
            int year = DateTime.Now.Year;            
            int mesR = DateTime.Now.AddDays(1).Month;
            int dayR = DateTime.Now.AddDays(1).Day;
            textInfo = cultureInfo.TextInfo;
                
           try
           {
               //Obtenemos listado de personas que cumplen años un día posterior a la fecha actual.      
               var query = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.BIRTHDAY != null).ToList()
                                 join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                                 join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                                 join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                                 where ae.ID_ACCO == 4
                                 where pe.ID_PERS_STAT == 1
                                 where pc.VIG_CONT == true && pc.LAS_CONT == true
                                 where ce.BIRTHDAY.Value.Month == mesR && ce.BIRTHDAY.Value.Day == dayR
                                 //where pe.ID_PERS_ENTI == 1055 // ID_PERS_ENTI de persona que se quiere enviar un mensaje en caso no se envie.
                                 select new
                                 {
                                     pe.ID_PERS_ENTI,
                                     NOMBRE = ((ce.FIR_NAME == null ? "" : ce.FIR_NAME) + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME)).ToLower(),
                                 }).ToList();

                    if (query.Count() > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            try
                            {
                                int ID_PERS_ENTI = Convert.ToInt32(query[i].ID_PERS_ENTI);
                                string NOMBRE = textInfo.ToTitleCase(Convert.ToString(query[i].NOMBRE));

                                //Verificamos que no se haya enviado recordatorio
                                int send = dbe.PERSON_ENTITY_NOTIFICATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                    .Where(x => x.CREATED.Value.Year == year)
                                    .Where(x => x.ID_PERS_NOTI == 2).Count();

                                if (send == 0)
                                {
                                    //creación de correo y envío.
                                    string HB = RecordatorioCumple(ID_PERS_ENTI,NOMBRE);

                                    //Si todo es conforme se procede a guardar registro.
                                    if (HB == "OK")
                                    {
                                        var perEntNot = new PERSON_ENTITY_NOTIFICATION();
                                        perEntNot.ID_PERS_ENTI = ID_PERS_ENTI;
                                        perEntNot.ID_PERS_NOTI = 2;
                                        perEntNot.CREATED = DateTime.Now;
                                        perEntNot.UserId = 29;
                                        dbe.PERSON_ENTITY_NOTIFICATION.Add(perEntNot);
                                        dbe.SaveChanges();
                                        Console.WriteLine("Se registró correctamente la notificación del envio del mensaje de recordatorio de cumpleaños");
                                    }                                    
                                }
                            }
                            catch (Exception e)
                            {
                                var exc = new EXCEPTION();
                                exc.DAT_EXCE = DateTime.Now;
                                exc.MESSAGE = e.InnerException.Message;
                                exc.DES_EXCE = "Send Mail - Recordatorio HappyBirthday";
                                dbl.EXCEPTIONs.Add(exc);
                                dbl.SaveChanges();

                                Console.WriteLine("Error intentado enviar el correo de recordatorio de cumpleaños");
                                Console.WriteLine("Error generado: " + e.Message.ToString()); 
                            }
                        }
                        return "OK";
                    }
                    else
                    {
                        return "No hay mensajes de recordatorio de cumpleaños para enviar";
                    }
                }
           catch (Exception e)
           {
               Console.WriteLine("Hubo un error al enviar el mesaje de recordatorio cumpleaños");
               Console.WriteLine("Error generado: " + e.Message.ToString());
               return "Hubo un error al enviar el mesaje de Recordatorio de Cumpleaños";
           }
        }
        
        //Función encargada de crear mail
        public string RecordatorioCumple(int ID_PERS_ENTI,string NOMBRE)
        {
            string supervisor = null;

            try
            {
                //Obtenemos contrato para obtener cargo.
                var cont = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                    .Where(x => x.VIG_CONT == true)
                    .Single(x => x.LAS_CONT == true);

                int ID_CHAR = cont.ID_CHAR.Value;
                int ID_CHAR_JEF = 0;
                try { 
                    //seleccionamos jefe del empleado.
                    ID_CHAR_JEF = Convert.ToInt32(dbe.TA_UEN_CARGO(4).Single(x => x.ID_CHAR == ID_CHAR).ID_CHAR_JEF);
                }
                catch { ID_CHAR_JEF = 0; }

                int CONT_JEF = dbe.PERSON_CONTRACT.Single(x => x.ID_CHAR == ID_CHAR_JEF && x.VIG_CONT==true && x.LAS_CONT==true).ID_PERS_CONT;

                var query = (from pc in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_CONT == CONT_JEF)
                             join pe in dbe.PERSON_ENTITY on pc.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                             select new
                             {
                                 mail = pe.EMA_ELEC
                             }).First();

                supervisor = query.mail;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error obteniendo jefe para el Recordatorio de Cumpleaños de la persosana con ID_PERS_ENTI: " + ID_PERS_ENTI);
                Console.WriteLine("Error generado: " + ex.Message.ToString());
                supervisor = null;
            }
            try
            {
                //Creamos contenido de correo.
                TalentTemplate body = new TalentTemplate();
                string body_html = body.TarjetaCumple(ID_PERS_ENTI);

                if (body_html != null)
                {
                    //Creamos el objeto correo electrónico.
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    message.To.Add("tramos@electrodata.com.pe,zrivadeneyra@electrodata.com.pe,rrhh@electrodata.com.pe");
                    //message.Bcc.Add("ingresar un correo para monitoreo");

                    if (supervisor != null)
                    {
                        message.CC.Add(supervisor);
                    }

                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                    message.AlternateViews.Add(htmlView);
                    message.Subject = "Recordatorio de Cumpleaños de " + NOMBRE + " (Referencial No Enviar)";
                    newMail.smtp.Send(message);

                    Console.WriteLine("Se envió correctamente el mensaje de recordatorio de cumpleaños del ID_PERS_ENTI: " + ID_PERS_ENTI);
                    return "OK";
                }
                else
                {
                    Console.WriteLine("No se ha podido generar el Template para el mensaje de recordatorio de cumpleaños del ID_PERS_ENTI: " + ID_PERS_ENTI);
                    return "ERROR";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo de Recordatorio de Cumpleaños del ID_PERS_ENTI: " + ID_PERS_ENTI);
                Console.WriteLine("Error generado: " + ex.Message.ToString());
                return "ERROR";
            }
        }

        //Mensajes de Experiración de Contratos
        public string ExpirationContract()
        {
            try
            {
                //Crear objeto correo.
                SendMail mail = new SendMail();

                
                int contexpcont30 = 0, contexpcont15 = 0, contexpcont7 = 0;

                //Falta 30 días para que termine el contrato.
                 contexpcont30 = CountExpirationContract(30);

                 if (contexpcont30 > 0)
                 {
                     Console.WriteLine("Dentro de 30 días expirarán " + contexpcont30 + " contratos");
                     PersonExpirationContract(30);
                 }

                ////Falta 15 días para que termine el contrato.
                //contexpcont15 = CountExpirationContract(15);
                //if (contexpcont15 > 0)
                //{
                //    Console.WriteLine("Dentro de 15 días expirarán " + contexpcont15 + " contratos");
                //    PersonExpirationContract(15);
                //}

                //Falta 7 días para que termine el contrato.
                 contexpcont7 = CountExpirationContract(7);
                 if (contexpcont7 > 0)
                 {
                     Console.WriteLine("Dentro de 7 días expirarán " + contexpcont7 + " contratos");
                     PersonExpirationContract(7);
                 }

                //Mitad de contrato.
                //Console.WriteLine("Envio de correos de la mitad de tiempos de contratos");
                //HalfPersonExpirationContract();

                //Faltando tres Meses
                Console.WriteLine("Envio de correo antes de cumplir los tres meses");
                NotificationContract();

           //     System.IO.File.WriteAllText(@"E:\Htmlcontrac.txt", plantillahtml);
                
                return "OK";
            }
            catch
            {
                return "Error al enviar los mensajes de Experiracón de Contratos";
            }
        }

        //Cantidad de contactos a expirar.
        public int CountExpirationContract(int id = 0)
        {
            DateTime END_DATE = DateTime.Today;
            END_DATE = END_DATE.AddDays(id);

            var ctd = dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true && x.END_DATE == END_DATE).Count();

            return ctd;
        }

        public string PersonExpirationContract(int id = 0)
        {
            try
            {
                //id=Cantidad de dias
                string msgsmec = "";
                DateTime END_DATE = DateTime.Today;
                END_DATE = END_DATE.AddDays(id);

                //Personas que vencerá su contrato en la fecha indicada.
                var query = (from pc in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true && x.END_DATE == END_DATE)
                             select new
                             {
                                 pc.ID_PERS_CONT,
                                 pc.ID_PERS_ENTI,
                                 pc.STAR_DATE,
                                 pc.END_DATE,
                             });

                //recorrer todos los contratos a expirar.
                foreach (var q in query.ToList())
                {
                    int ID_PERS_ENTI=Convert.ToInt32(q.ID_PERS_ENTI);
                    int ID_PERS_CONT = Convert.ToInt32(q.ID_PERS_CONT);

                    int ID_PERS_NOTI = 0;
                    int caseSwitch = id;

                    //Elegir tipo de notificación.
                    switch (caseSwitch)
                    {
                        case 7:
                            ID_PERS_NOTI = 6;
                            break;
                        case 15:
                            ID_PERS_NOTI = 7;
                            break;
                        case 30:
                            ID_PERS_NOTI = 8;
                            break;
                        case 0:
                            ID_PERS_NOTI = 9;
                            break;
                        default:
                            Console.WriteLine("No es una fecha a evaluar");
                            break;
                    }

                    //Cantidad de notificaciones enviadas por persona y tipo de notificación.
                    int qpersentinoti = dbe.PERSON_ENTITY_NOTIFICATION.Where(x=>x.ID_PERS_ENTI==ID_PERS_ENTI && x.ID_PERS_NOTI==ID_PERS_NOTI && x.ID_PERS_CONT==ID_PERS_CONT).Count();
                    
                    //Si aún no se envía notificación.
                    if (qpersentinoti==0)
                    {
                        //Crear y enviar correo electrónico.
                        msgsmec=SendMailExpirationContract(Convert.ToInt32(q.ID_PERS_CONT), id,0);

                        //Si todo es conforme se procede a guardar registro.
                        if(msgsmec=="OK")
                        {
                            PERSON_ENTITY_NOTIFICATION pen = new PERSON_ENTITY_NOTIFICATION();

                            pen.ID_PERS_ENTI = ID_PERS_ENTI;
                            pen.ID_PERS_NOTI = ID_PERS_NOTI;
                            pen.ID_PERS_CONT = ID_PERS_CONT;
                            pen.CREATED = DateTime.Now;
                            pen.UserId = 29;
                            dbe.PERSON_ENTITY_NOTIFICATION.Add(pen);
                            dbe.SaveChanges();

                            Console.WriteLine("Se actualizó información para el ID_PERS_ENTI: " + ID_PERS_ENTI + " con el ID_PERS_NOTI: " + ID_PERS_NOTI);

                        }
                    }

                }
                return "OK";
            }
            catch(Exception e)
            {
                Console.WriteLine("Error actualizando información de datos en la base de Datos");
                Console.WriteLine("Error Generado =>"+e.ToString());
                return "ERROR";
            }

        }

        //Envio de Mensaje a la Mitad de tiempo de contrato.
        public string HalfPersonExpirationContract()
        {
            int ID_PERS_ENTI = 0;
            int ID_PERS_CONT = 0;
            int totaldays = 0;
            try
            {                
                string msgsmec = "";
                DateTime TODAY = DateTime.Today;
                int bandera = 0;

                var query = (from pc in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true && x.END_DATE > TODAY)
                             join pen in dbe.PERSON_ENTITY_NOTIFICATION on pc.ID_PERS_CONT equals pen.ID_PERS_CONT into lperentinoti
                             from lpen in lperentinoti.DefaultIfEmpty()                            
                             where lpen.ID_PERS_NOTI!=9                          
                             select new
                             {
                                 pc.ID_PERS_CONT,
                                 pc.ID_PERS_ENTI,
                                 pc.STAR_DATE,
                                 pc.END_DATE,
                             });               

                foreach (var q in query.ToList())
                {
                    
                    try
                    {
                         ID_PERS_ENTI = Convert.ToInt32(q.ID_PERS_ENTI);
                         ID_PERS_CONT = Convert.ToInt32(q.ID_PERS_CONT);

                        DateTime STAR_DATE = Convert.ToDateTime(q.STAR_DATE);
                        DateTime END_DATE = Convert.ToDateTime(q.END_DATE);  

                        int TotDaysCont = Convert.ToInt32((END_DATE - STAR_DATE).TotalDays);

                        int HalfTotaDays = TotDaysCont / 2;

                        DateTime HALF_DATE_CONT = STAR_DATE.AddDays(HalfTotaDays);

                        totaldays = HalfTotaDays;

                        if (HALF_DATE_CONT== DateTime.Today)
                        { 
                            bandera = 1; 
                        }
                        else { 
                            bandera = 0; 
                        }

                    }
                    catch
                    {
                        bandera=0;
                         
                    }

                    int ID_PERS_NOTI = 9;
                    if (bandera==1)
                    {
                        int qpersentinoti = dbe.PERSON_ENTITY_NOTIFICATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.ID_PERS_NOTI == ID_PERS_NOTI && x.ID_PERS_CONT == ID_PERS_CONT).Count();
                        if (qpersentinoti == 0)
                        {
                            msgsmec = SendMailExpirationContract(Convert.ToInt32(q.ID_PERS_CONT),totaldays,0);

                            if (msgsmec == "OK")
                            {
                                PERSON_ENTITY_NOTIFICATION pen = new PERSON_ENTITY_NOTIFICATION();

                                pen.ID_PERS_ENTI = ID_PERS_ENTI;
                                pen.ID_PERS_NOTI = ID_PERS_NOTI;
                                pen.ID_PERS_CONT = ID_PERS_CONT;
                                pen.CREATED = DateTime.Now;
                                pen.UserId = 29;
                                dbe.PERSON_ENTITY_NOTIFICATION.Add(pen);
                                dbe.SaveChanges();

                                Console.WriteLine("Se actualizó información para el ID_PERS_ENTI: " + ID_PERS_ENTI + " con el ID_PERS_NOTI: " + ID_PERS_NOTI);

                            }
                        }
                    }

                }
                return "OK";
            }
            catch
            {
                return "ERROR";
            }
        }

        //Envio de Mensaje Antes de los tres meses - Nuevos Ingresos.      
        public string NotificationContract()
        {
            int ID_PERS_ENTI = 0;
            int ID_PERS_CONT = 0;
            int ID_PERS_NOTI = 0;
            int totaldays = 0;
            string msgsmec = "";
            DateTime TODAY = DateTime.Today;
            int bandera = 0;

            try
            {
                 //lista de personal que cumple condición de 3 meses.
                 var query = (from pc in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true && x.END_DATE > TODAY)
                             join wp in dbe.WORK_PERIOD on pc.ID_WORK_PERI equals wp.ID_WORK_PERI
                             join pen in dbe.PERSON_ENTITY_NOTIFICATION on pc.ID_PERS_CONT equals pen.ID_PERS_CONT into lperentinoti
                             from lpen in lperentinoti.DefaultIfEmpty()
                             where lpen.ID_PERS_NOTI != 10
                             where wp.STAR_DATE.Value.Year >= 2015
                             select new
                             {
                                 pc.ID_PERS_CONT,
                                 pc.ID_PERS_ENTI,
                                 pc.STAR_DATE,
                                 pc.END_DATE,
                             });

                int TOT = query.Count();

                //recorrer listado
                foreach (var q in query.ToList())
                {

                    try
                    {
                        ID_PERS_ENTI = Convert.ToInt32(q.ID_PERS_ENTI);
                        ID_PERS_CONT = Convert.ToInt32(q.ID_PERS_CONT);

                        DateTime STAR_DATE = Convert.ToDateTime(q.STAR_DATE);
                        DateTime END_DATE = Convert.ToDateTime(q.END_DATE);

                        DateTime DATE_CONT_3M = STAR_DATE.AddMonths(3);
                        DateTime DATE_NOTI = DATE_CONT_3M.AddDays(-7);

                        if (DATE_NOTI == TODAY && END_DATE > TODAY)
                        {
                            bandera = 1;
                        }
                        else
                        {
                            bandera = 0;
                        }                   

                   
                    if (bandera == 1)
                    {
                        ID_PERS_NOTI = 10;
                        totaldays = 7;

                        int qpersentinoti = dbe.PERSON_ENTITY_NOTIFICATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.ID_PERS_NOTI == ID_PERS_NOTI && x.ID_PERS_CONT == ID_PERS_CONT).Count();
                        if (qpersentinoti == 0)
                        {
                            //crear y enviar correo.
                            msgsmec = SendMailExpirationContract(Convert.ToInt32(q.ID_PERS_CONT), totaldays,1);

                            //Si todo es conforme se procede a guardar
                            if (msgsmec == "OK") 
                            {

                                PERSON_ENTITY_NOTIFICATION pen = new PERSON_ENTITY_NOTIFICATION();

                                pen.ID_PERS_ENTI = ID_PERS_ENTI;
                                pen.ID_PERS_NOTI = ID_PERS_NOTI;
                                pen.ID_PERS_CONT = ID_PERS_CONT;
                                pen.CREATED = DateTime.Now;
                                pen.UserId = 29;
                                dbe.PERSON_ENTITY_NOTIFICATION.Add(pen);
                                dbe.SaveChanges();

                                Console.WriteLine("Se actualizó información para el ID_PERS_ENTI: " + ID_PERS_ENTI + " con el ID_PERS_NOTI: " + ID_PERS_NOTI);

                            }
                        }
                    }

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Error actualizando información de un personal que expira su contrato en tres meses");
                        Console.WriteLine("Error Generado "+e.Message.ToString());

                    }

                }
                return "OK";
            }
            catch(Exception e)
            {
                Console.WriteLine("Error actualizando información de personas que expira su contrato en tres meses");
                Console.WriteLine("Error Generado " + e.Message.ToString());
                return "ERROR";
            }

        }

        //Creación de contenido y envio de correo.
        public string SendMailExpirationContract(int id = 0, int id1=0, int id2=0)
        {
            //id=ID_PERS_CONT --id1=Cantidad de días --id2=0 Experiración de contrato id2=1: Notificación tres meses

            try
            {
              
                TalentTemplate body = new TalentTemplate();
                string body_html = body.TemplateExperirationContract(id,id1,id2);
                
                if (body_html != null)
                {
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;

                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                    message.To.Add("rrhh@electrodata.com.pe");                   
                    //message.CC.Add("lsempertegui@electrodata.com.pe");
                    message.AlternateViews.Add(htmlView);
                    message.Subject = "Expiration Contract / Expiración de Contrato";
                    newMail.smtp.Send(message);                 

                    Console.WriteLine("To: rrhh@electrodata.com.pe");                    
                    Console.WriteLine("Se envió correo de la experiación de contrato con ID: " + id);

                   // plantillahtml=plantillahtml+body_html+ "\r\n"; 

                return "OK";
                }
                else
                {
                    Console.WriteLine("Error: No se pudo generar el Template para el contrato con ID: "+id);
                    return "ERROR";
                }

                
            }
            catch(Exception exc)
            {
                Console.WriteLine("Error enviando correo de experiración de contrato con ID: " + id);
                Console.WriteLine("Error generado: " + exc.Message.ToString());
                return "ERROR";
            }
        }

        //Reporte de asistencia.
        public string AttendanceReport()
        {
            try
            {
                var fecha = DateTime.Now;            
               
                //Hora de generación de reporte
                if (fecha.Hour == 18)
                {
                    //Grupo de personas a quienes se les enviará reporte.
                    var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_STAT == 1)
                                                 .Where(x => x.ID_SUB_CIA == 9 || x.ID_SUB_CIA == 3975 || x.ID_SUB_CIA == 4550).ToList()
                                 join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                 join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                                 join pc in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT==true) on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                                 where pe.ID_PERS_ENTI != 8352 && pe.ID_PERS_ENTI != 1131 && pe.ID_HORA!=null
                                 where ae.ID_ACCO==4
                                 where pe.ID_PERS_ENTI == 10491 // Solo para prueba
                                 select new
                                 {
                                     ID_PERS_ENTI = pe.ID_PERS_ENTI,
                                     DNI = ce.NUM_TYPE_DI,
                                     FIR_NAME = ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower(),
                                     DESC_FILE = ce.NUM_TYPE_DI,
                                     EMA_ELEC = pe.EMA_ELEC,
                                     SEX_ENTI = ce.SEX_ENTI,
                                     FIR_NAME1 = ce.FIR_NAME.ToLower(),
                                 }).ToList();

                    int tests = query.Count;

                    //recorrer lista de personas para enviar correos individuales.
                    foreach (var pers in query)
                    {
                        try
                        {
                            int ID_PERS_ENTI = pers.ID_PERS_ENTI;
                            string DNI = pers.DNI;
                            string DESC_FILE = pers.DESC_FILE;
                            try
                            {
                                //Verificar si se ha enviado la notificación.
                                var q = dbe.PERSON_ENTITY_NOTIFICATION
                                    .Where(x => x.ID_PERS_ENTI == pers.ID_PERS_ENTI && x.ID_PERS_NOTI == 5)
                                    .Where(x => x.CREATED.Value.Day == fecha.Day)
                                    .Where(x => x.CREATED.Value.Month == fecha.Month)
                                    .Where(x => x.CREATED.Value.Year == fecha.Year)
                                    .Count();

                                if (q == 0)
                                {                                    
                                    //Crear contenido y enviar correo.
                                    string AR = Attendance(ID_PERS_ENTI, DNI, DESC_FILE, pers.SEX_ENTI, pers.FIR_NAME1, pers.EMA_ELEC, pers.FIR_NAME);

                                    if (AR == "OK")
                                    {
                                        //Si todo es conforme guardar registro.
                                        PERSON_ENTITY_NOTIFICATION noti_atte_report = new PERSON_ENTITY_NOTIFICATION();
                                        noti_atte_report.ID_PERS_ENTI = pers.ID_PERS_ENTI;
                                        noti_atte_report.ID_PERS_NOTI = 5;
                                        noti_atte_report.CREATED = DateTime.Now;
                                        noti_atte_report.UserId = 29;
                                        dbe.PERSON_ENTITY_NOTIFICATION.Add(noti_atte_report);
                                        dbe.SaveChanges();
                                    }
                                }
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Error Generando la notificación del envio de correo");
                                Console.WriteLine("Error Generado: "+e.Message.ToString());

                            }
                        }
                        catch (Exception e)
                        {
                            var exc = new EXCEPTION();
                            exc.DAT_EXCE = DateTime.Now;
                            exc.MESSAGE = e.InnerException.Message;
                            exc.DES_EXCE = "Send Mail - Attendance Report";
                            dbl.EXCEPTIONs.Add(exc);
                            dbl.SaveChanges();

                            Console.WriteLine("Error Consiguiendo persona para la notificación del envio de correo");
                            Console.WriteLine("Error Generado: " + e.Message.ToString());
                        }
                    }

                }

                Console.WriteLine("Se terminaron de enviar todos los mensajes");
                return "OK";

            }
            catch(Exception e)
            {
                Console.WriteLine("Error Consiguiendo persona para la notificación del envio de correo");
                Console.WriteLine("Error Generado: " + e.Message.ToString());
                return "Fallo el envio :(";
               
            }
        }

        //Función que genera contenido de correo de asistencia y envia mail.
        public string Attendance(int ID_PERS_ENTI, string DNI, string DES_FILE, string SEXO, string NOMBRE, string email, string NOMBRECOMPLETO)
        {
            try
            {
                //Generar archivo PDF
                string file = FilePDF(ID_PERS_ENTI, DES_FILE);
                DateTime fecha = DateTime.Now.AddDays(-2);
                textInfo = cultureInfo.TextInfo;

                //Generar contenido de correo.
                TalentTemplate _body = new TalentTemplate();
                var html_msg = _body.AttendanceReportPerson(SEXO, NOMBRE);

                if (html_msg!=null && file!="ERROR")
                {

                //Creamos el objeto correo electrónico.
                SendMail newMail = new SendMail();
                var msg = newMail.mailMessage;
                DateTime ahora = DateTime.Now;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                var query = dba.VER_SEND_MAIL(DNI, ahora).ToList();
                int existmail = query.Where(x => x.SUPERVISOR != null).Count();
                string emailsupervisor = "";

                msg.AlternateViews.Add(htmlView);
                msg.Subject = "Control de Asistencia Semanal - " + textInfo.ToTitleCase(NOMBRECOMPLETO) + " (01/06 - 21/06)";

                // msg.To.Add("lsempertegui@electrodata.com.pe");
                msg.To.Add(email);
                if (existmail > 0)
                {
                    emailsupervisor = query.First().SUPERVISOR;
                    msg.CC.Add(emailsupervisor);
                }
                Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                msg.Attachments.Add(attachment);
                //newMail.smtp.Send(msg);
                return "OK";
                }
                else
                {
                    return "ERROR";
                }
            }
            catch
            {
                return "ERROR";
            }
        }

        //Función que crea archivo PDF guarda archivo y retorna dirección.
        public string FilePDF(int ID_PERS_ENTI, string DES_FILE)
        {
            try
            {
                byte[] bytes = null;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                Warning[] warnings;

                ServerReport sr = new ServerReport();
                sr.ReportServerCredentials = new CredencialesReporting("Administrator", "Electr0D@t4", "");
                sr.ReportServerUrl = new Uri(reportServer);
                sr.ReportPath = "/Attendance/ReportByPerson";

                string DATE_END = Convert.ToString(DateTime.Today.AddDays(-1));
                string DATE_START = Convert.ToString(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                string ID_PERS_ENTIs = Convert.ToString(ID_PERS_ENTI);

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("ID_PERS_ENTI", ID_PERS_ENTIs);
                param[1] = new ReportParameter("DATE_START", DATE_START);
                param[2] = new ReportParameter("DATE_END", DATE_END);

                sr.SetParameters(param);
                sr.Refresh();

                byte[] renderedBytes;

                renderedBytes = sr.Render("pdf");

                //Crea el PDF.
                bytes = sr.Render("PDF", null, out mimeType,
                    out encoding, out filenameExtension, out streamids, out warnings);
                string filename = "C:\\BackUp\\Reporting Services\\Attendance\\" + DES_FILE + ".pdf";
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                return filename;
            }
            catch
            {
                return "ERROR";
            }
        }

    }
}
