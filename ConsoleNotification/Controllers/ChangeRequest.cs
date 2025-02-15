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


namespace ConsoleNotification.Controllers
{
    class ChangeRequest
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        public string pendingRequest()
        {
            try
            {
                DateTime changePendingdate = DateTime.Now.AddHours(-48);
                using (var context = new CoreEntities())
                {
                    var query = (from ca in context.CHANGE_APPROVED.Where(x => x.idTypeRequest == 1 && x.createDate < changePendingdate)
                                 select new
                                 {
                                     id = ca.id,
                                     idTypeRequest= ca.idTypeRequest,
                                     createDate = ca.createDate
                                 }).ToList();
                    if (query.Count() > 0)
                    {
                        foreach (var q in query)
                        {
                            CHANGE_APPROVED ca = new CHANGE_APPROVED();
                            ca = dbc.CHANGE_APPROVED.Find(q.id);
                            ca.modifiedDate = DateTime.Now;
                            ca.idTypeRequest = 4;
                            //context.CHANGE_APPROVED.Attach(ca);
                            dbc.Entry(ca).State = EntityState.Modified;
                            dbc.SaveChanges();
                        }
                        Console.WriteLine("Se actualizo las Solicitudes de Cambio al estado Pendiente.");
                    }
                    else
                    {
                        Console.WriteLine("No hay Solicitudes por regularizar.");
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error al actualizar los estados Pendientes");
                Console.WriteLine("Error generado: " + e.Message.ToString());

                return "ERROR";                
            }
            return "OK";

        }

        public string AlertaDiasAntes(int dia)
        {
            DateTime changeInicio = DateTime.Now;
            DateTime changeFin = DateTime.Now;

            int year = DateTime.Now.Year;
            int mesR = DateTime.Now.Month;
            int dayR = DateTime.Now.AddDays(1).Day;//Dia siguiente

            if (dia == 1)
            {
                 if (dayR==1)
                 {
                     mesR = DateTime.Now.AddMonths(1).Month;
                     if (mesR == 1)
                     {
                         year = DateTime.Now.AddYears(1).Year;
                     }
                 }
            }
            else if (dia == 3)
            {
                 dayR = DateTime.Now.AddDays(3).Day;//tres dias siguientes
                 if (dayR == 1)
                 {
                     mesR = DateTime.Now.AddMonths(3).Month;
                     if (mesR == 1)
                     {
                         year = DateTime.Now.AddYears(1).Year;
                     }
                 }

            }
            
                using (var context = new CoreEntities())
                {
                    var queryDetail = (from cd in context.CHANGE_DETAIL.Where(x => x.FechaFinProgramada.Value.Year == year &&
                                        x.FechaFinProgramada.Value.Month == mesR && x.FechaFinProgramada.Value.Day == dayR).ToList()
                                        join cr in dbc.CHANGE_REQUEST on cd.ID_CHANGE_REQUEST equals cr.id
                                        join ca in dbc.CHANGE_APPROVED.Where(x =>x.idTypeRequest==2 || x.idTypeRequest==7) on cr.id equals ca.idChangeRequest
                                        join ct in dbc.CHANGE_TYPE_TASK on cd.ID_TYPE_TASK equals ct.ID
                                        join pe in dbe.PERSON_ENTITY on cd.ID_PERSON_ENTI equals pe.ID_PERS_ENTI
                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                        select new
                                        {
                                        //Detalle
                                        idCambio = cr.id,
                                        idDetail = cd.ID,
                                        Actividad=ct.TYPE_TASK,
                                        DETAIL= cd.DETAIL,
                                        fechaInicio = cd.FechaInicioProgramada,
                                        fechaFin = cd.FechaFinProgramada,

                                        //Responsable
                                        responsable = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                        emailResponsable = pe.EMA_ELEC
                                 }).ToList();

                if (queryDetail.Count() > 0)
                {
                    //recorremos listado de personas.
                    for (int i = 0; i < queryDetail.Count; i++)
                    {
                        try
                        {
                            int idCambio = Convert.ToInt32(queryDetail[i].idCambio);
                            int idDetail = Convert.ToInt32(queryDetail[i].idDetail);
                            string responsable = Convert.ToString(queryDetail[i].responsable);
                            string actividad = Convert.ToString(queryDetail[i].Actividad);
                            string detalle = Convert.ToString(queryDetail[i].DETAIL);
                            string colorAlerta = "#2b5797";
                            string email = Convert.ToString(queryDetail[i].emailResponsable);
                            string msjAlerta = "";
                            string fechaInicio = string.Format("{0:MM/dd/yy }", queryDetail[i].fechaInicio);
                            string fechaFin = string.Format("{0:MM/dd/yy }", queryDetail[i].fechaFin);


                            //Verificamos que no se haya enviado la alerta
                            int send=0;
                            if (dia == 1)
                            {
                                msjAlerta = "La Actividad terminará en 1 día";
                                send = dbc.CambioNotificacions.Where(x => x.idDetail == idDetail)
                                    .Where(x => x.IdTipoNotificacion == 1).Count();
                            }
                            else
                            {
                                msjAlerta = "La Actividad terminará en 3 días";
                                send = dbc.CambioNotificacions.Where(x => x.idDetail == idDetail)
                                    .Where(x => x.IdTipoNotificacion == 2).Count();
                            }
                             

                            if (send == 0)
                            {
                                //crear y enviar correo.
                                string CR = SendMail(dia, idCambio, idDetail, responsable, actividad, detalle, colorAlerta, email, msjAlerta, fechaInicio, fechaFin);

                            }
                        }
                        catch (Exception e)
                        {

                            Console.WriteLine("Error al enviar el correo");
                            Console.WriteLine("Error generado: " + e.Message.ToString());
                        }

                    }
                    return "OK";
                }
                else
                {
                    return "No hay alertas para actividades pendientes";
                }
            }
        }


        public string SendMail(int dia,int idCambio,int idDetail, string responsable, string actividad, string detalle, string colorAlerta, string email, string msjAlerta,
                                string fechaInicio, string fechaFin)
        {
            try
            {

                ///////////////////////////////crear y enviar correo.
                //Creamos cuerpo de correo.
                ChangeRequestTemplate body = new ChangeRequestTemplate();
                string body_html = body.CorreoAlerta1Request(
                responsable, actividad, detalle, colorAlerta, msjAlerta, fechaInicio, fechaFin,Convert.ToString(idCambio));

                //Si el cuerpo del correo es conforme.
                if (body_html != null)
                {
                    //Creamos el objeto correo electrónico.
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    message.To.Add(email);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                    message.AlternateViews.Add(htmlView);
                    message.Subject = "ACTIVIDAD PENDIENTE: " + responsable;
                    newMail.smtp.Send(message);

                    Console.WriteLine("Se envió correctamente la Alerta id: " + idDetail);

                    //Si todo es conforme se procede a guardar registro.
                    
                        var cambioNot = new CambioNotificacion();
                        if (dia == 1)
                        {
                            cambioNot.IdTipoNotificacion = 1;
                        }
                        else if (dia == 3)
                        {
                            cambioNot.IdTipoNotificacion = 2;
                        }
                        cambioNot.idChangeRequest = idCambio;
                        cambioNot.idDetail = idDetail;
                        cambioNot.Fecha = DateTime.Now;
                        dbc.CambioNotificacions.Add(cambioNot);
                        dbc.SaveChanges();
                        Console.WriteLine("Se registró correctamente la notificación");
                    

                    return "OK";
                }
                else
                {
                    Console.WriteLine("No se ha podido generar el Template para el responsable de la Actividad: " + responsable);
                    return "ERROR";
                }
                ///////////////////////////////////////////////////////////////
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generado: " + e.Message.ToString());

                return "ERROR";
            }
        }


        public string AlertaFinActividad()
        {
            int year = DateTime.Now.Year;
            int mes = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            using (var context = new CoreEntities())
            {
                        //Listado de Fecha Maxima por Solicitud del Cambio
                        var queryDetail = dbc.GestionCambioListadoFecha().ToList();

                        var query = (from cr in dbc.CHANGE_REQUEST.ToList()
                                                 join ca in dbc.CHANGE_APPROVED.Where(x => x.idTypeRequest == 2 || x.idTypeRequest == 7) on cr.id equals ca.idChangeRequest
                                                 join pe in dbe.PERSON_ENTITY on ca.approver equals pe.ID_PERS_ENTI
                                                 join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                                 select new
                                                 {
                                                     idChange=cr.id,
                                                     //aprobador
                                                     idAprobador = ca.approver,
                                                     aprobador = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                                     emailAprobador = pe.EMA_ELEC,

                                                     //Detalle
                                                     fecha = cr.createDate,
                                                     detalle = cr.question2
                                                 }).ToList();
                    
                    

                        for (int i = 0; i < query.Count; i++)
                        {
                            int yearCreacion = query[i].fecha.Value.Year;
                            int mesCreacion = query[i].fecha.Value.AddMonths(1).Month;//Mes siguiente
                            int dayCreacion = query[i].fecha.Value.Day;

                            int idChangeRequest = Convert.ToInt32(query[i].idChange);

                            if (mesCreacion == 1)
                            {
                                yearCreacion = query[i].fecha.Value.AddYears(1).Year;
                            }

                            if (dayCreacion == day && mesCreacion == mes && yearCreacion == year && query[i].fecha.Value <= queryDetail[i].FechaFinal) 
                                {

                                //crear y enviar correo.
                                //string CR = listadoDelCambio(Convert.ToInt32(queryDetail[j].ID_CHANGE_REQUEST), Convert.ToString(queryDetail[j].FechaFinal));
                                    var idPerson = (from cr in context.CHANGE_REQUEST.Where(x => x.id == idChangeRequest)
                                                  select new
                                                  {
                                                      idChange = cr.id,
                                                      idPerson=cr.ID_PERS_ENTI
                                                  }).Single();

                                    var queryUsuario = (from pe in dbe.PERSON_ENTITY.Where(x=> x.ID_PERS_ENTI==idPerson.idPerson) 
                                                        join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                                        select new
                                                        {
                                                            Solicitante = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                                                            emailSolicitante = pe.EMA_ELEC
                                                        }).Single();


                                    try
                                    {
                                        //recorremos listado de personas.
                                        string fechaFin = string.Format("{0:MM/dd/yy}", queryDetail[i].FechaFinal);
                                        int idCambio = Convert.ToInt32(query[i].idChange);
                                        string solicitante = Convert.ToString(queryUsuario.Solicitante);
                                        string aprobador = Convert.ToString(query[i].aprobador);
                                        string detalle = Convert.ToString(query[i].detalle);
                                        string colorAlerta = "#2b5797";
                                        string emailSolicitante = Convert.ToString(queryUsuario.emailSolicitante);
                                        string emailAprobador = Convert.ToString(query[i].emailAprobador);
                                        string msjAlerta = "Ha transcurrido un mes desde la Solicitud del Cambio";
                                        string fechaCreacion = string.Format("{0:MM/dd/yy H:mm:ss}", query[i].fecha);

                                        //Verificamos que no se haya enviado la alerta
                                        int send = 0;
                                        send = dbc.CambioNotificacions.Where(x => x.idChangeRequest == idCambio)
                                            .Where(x => x.IdTipoNotificacion == 3).Count();

                                        if (send == 0)
                                        {
                                            //crear y enviar correo.
                                            string CRA = SendMailActividad(idCambio, solicitante, aprobador, detalle, colorAlerta, emailSolicitante, emailAprobador, msjAlerta, fechaCreacion, fechaFin);

                                        }


                                    }
                                    catch (Exception e)
                                    {

                                        Console.WriteLine("Error al enviar el correo");
                                        Console.WriteLine("Error generado: " + e.Message.ToString());
                                    }
                                }

                            }
                            return "OK";
                        }       
        }


        public string SendMailActividad(int idCambio, string solicitante, string aprovador, string detalle, string colorAlerta, string emailSolicitante, string emailAprobador, string msjAlerta,
                        string fechaInicio, string fechaFin)
        {
            try
            {

                ///////////////////////////////crear y enviar correo.
                //Creamos cuerpo de correo.
                ChangeRequestTemplate body = new ChangeRequestTemplate();
                string body_html = body.CorreoAlertaFinActividad(solicitante,
                aprovador, detalle, colorAlerta, msjAlerta, fechaInicio, fechaFin,Convert.ToString(idCambio));

                //Si el cuerpo del correo es conforme.
                if (body_html != null)
                {
                    //Creamos el objeto correo electrónico.
                    SendMail newMail = new SendMail();
                    var message = newMail.mailMessage;
                    message.To.Add(emailSolicitante);
                    message.To.Add(emailAprobador);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                    message.AlternateViews.Add(htmlView);
                    message.Subject = "SOLICITUD DEL CAMBIO: " + solicitante;
                    newMail.smtp.Send(message);

                    Console.WriteLine("Se envió correctamente la Alerta id del Cambio: " + idCambio);

                    //Si todo es conforme se procede a guardar registro.

                    var cambioNot = new CambioNotificacion();

                    cambioNot.IdTipoNotificacion = 3;
                    cambioNot.idChangeRequest = idCambio;
                    cambioNot.Fecha = DateTime.Now;
                    dbc.CambioNotificacions.Add(cambioNot);
                    dbc.SaveChanges();
                    Console.WriteLine("Se registró correctamente la notificación");


                    return "OK";
                }
                else
                {
                    Console.WriteLine("No se ha podido generar el Template para la Alerta: " + solicitante);
                    return "ERROR";
                }
                ///////////////////////////////////////////////////////////////
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generado: " + e.Message.ToString());

                return "ERROR";
            }
        }

        



    }
}
