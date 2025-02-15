using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ConsoleNotification.Models;
using ConsoleNotification.Plugins;
using ConsoleNotification.Controllers;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;

namespace ConsoleNotification
{
    class Program
    {
        //declaracion de variables - conexion bd  
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        static int cont = 0;
        static int contexpcont = 0;
        static int contticketreport = 0;
        static int contTicketReportSemanal = 0;

        bool flag = true;

        //Main
        static void Main(string[] args)
        {
            //Timer t = new Timer(TimerTest, null, 0, 8 * 60 * 1000);
            Timer t = new Timer(TimerCallback, null, 0, 5 * 60 * 1000);

            Console.WriteLine("                         APLICACION DE ENVIO DE CORREOS ITMS");
            Console.WriteLine("                        -------------------------------------\n");
            Console.WriteLine("********************************************************************************");
            Console.ReadLine();
        }

        private static void TimerTest(Object o)
        {
            //Tickets tk = new Tickets();
            //string xx = tk.SendMailResolvedOPFromTicketCook(0);
            string msgtk = "";
            Tickets tk = new Tickets();
            msgtk = tk.NewTicket();
            //Projects py = new Projects();
            //string xx = py.ReportProject();
        }

        private static void TimerCallback(Object o)
        {
            try
                {
                if (DateTime.Now.Hour == 7 && DateTime.Now.Minute < 15)
                {
                    /////////////////////////Change Request
                    //////////////////Solicitud de Cambios - 1dia antes por Actividad

                    ChangeRequest creq = new ChangeRequest();
                    string msgCReq = "";
                    Console.WriteLine("Enviando Correos alertando por actividad 1 dia antes...");
                    Console.WriteLine("--------------------------------------------------------------------------------");
                    //Alerta de ITMS CHANGE REQUEST E-DATA; 
                    msgCReq = creq.AlertaDiasAntes(1);

                    if (msgCReq == "OK")
                    {
                        Console.WriteLine("Se enviaron correctamente los correos alertando por Actividad un dia antes: " + DateTime.Now.ToString("HH:mm"));
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgCReq);
                        Console.WriteLine("--------------------------------------------------------------------------------");

                    }

                    //////////////////Solicitud de Cambios - 3dias antes por Actividad

                    ChangeRequest creq3 = new ChangeRequest();
                    string msgCReq3 = "";
                    Console.WriteLine("Enviando Correos alertando por actividad 3 dias antes...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgCReq3 = creq3.AlertaDiasAntes(3);

                    if (msgCReq3 == "OK")
                    {
                        Console.WriteLine("Se enviaron correctamente los correos alertando por Actividad tres dias antes: " + DateTime.Now.ToString("HH:mm"));
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    
}
                    else
                    {
                        Console.WriteLine(msgCReq3);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }

                    //////////////////Solicitud de Cambios - Alerta por fecha ultima de Actividad dias 30

                    ChangeRequest crequest = new ChangeRequest();
                    string msgCRequest = "";
                    Console.WriteLine("Enviando Correos alertando por fin de Actividad dia 30...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgCRequest = crequest.AlertaFinActividad();

                    if (msgCRequest == "OK")
                    {
                        Console.WriteLine("Se enviaron correctamente los correos alertando por Fin de Actividad dia 30: " + DateTime.Now.ToString("HH:mm"));
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgCRequest);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                }
                ////////////////////////////////////////////////////////////////

                Tickets tk = new Tickets();
                /*Ticket Problemas ---  El proceso se ejecutará a las 6 : 15 am*/
                int msgtk_ = 0;

                if (DateTime.Now.Hour == 4 && DateTime.Now.Minute < 15)
                {
                    Console.WriteLine("Generando ticket problemas");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgtk_ = Convert.ToInt32(tk.GenerarTicketProblema().Resultado);

                    if (msgtk_ == 1)
                    {
                        Console.WriteLine("Se generaron correctamente los Tickets Problema\n");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine("No se logró generar los Tickets Problema\n");
                        Console.WriteLine(msgtk_);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                }
                /*Fin*/

                //////////Creación de Tickets
                string msgtk = "";
                string msgdtk = "";

                string msgsla, msgdtsched;

                Console.WriteLine("Enviando Correos de Creación Tickets...");
                Console.WriteLine("--------------------------------------------------------------------------------");

                msgtk = tk.NewTicket();

                if (msgtk == "OK")
                {
                    Console.WriteLine("Se enviaron correctamente los correos de la creación de Tickets \n");
                    Console.WriteLine("--------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine(msgtk);
                    Console.WriteLine("--------------------------------------------------------------------------------");
                }

                //Notificación de Tickets Pendientes de Activos
                //msgtk = tk.NotificacionTicketActivo();
                //if (msgtk == "OK")
                //{
                //    Console.WriteLine("Se enviaron correctamente los correos de notificación de Tickets de Activos \n");
                //    Console.WriteLine("--------------------------------------------------------------------------------");
                //}
                //else
                //{
                //    Console.WriteLine(msgtk);
                //    Console.WriteLine("--------------------------------------------------------------------------------");
                //}

                //////Mensajes fuera de SLA

                if (cont % 1 == 0)
                {

                    Console.WriteLine("Enviando correos de Tickets fuera de SLA...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgsla = "";

                    //msgsla = tk.NotificationSLABuenaventura();

                    if (msgsla == "OK")
                    {
                        Console.WriteLine("Se enviaron correctamente los correos de SLA");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgsla);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }

                    //////Actualizando los Tickets Schedule...

                    Console.WriteLine("Actualizando Tickets Schedule...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgdtsched = "";
                    msgdtsched = tk.UpdateStatusTicketScheduled();
                    if (msgdtsched == "OK")
                    {
                        Console.WriteLine("Se actualizó correctamenete los Tickets en Schedule");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgdtsched);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }

                    //    cont = -1;
                    //}
                    //else
                    //{
                    //    cont = Math.Abs(cont) + 1;
                    //}

                    ////////Envio de Correos de detalles de Tickets

                    Console.WriteLine("Enviando Correos de Details_Ticket...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgdtk = tk.UpdateStatus();

                    if (msgdtk == "OK")
                    {
                        Console.WriteLine("Se enviaron correctamente los correos de Details_Tickets");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgdtk);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }

                    ////////Envio de Correos de Altas y Bajas en Minsur

                    string msgtp = "";

                    Console.WriteLine("Enviando Correos de Tareas de Tickets de altas y baas...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    //msgtp = tk.SendMailTareasPendientes();

                    //if (msgtp == "OK")
                    //{
                    //    Console.WriteLine("Se enviaron correctamente los correos de Tareas de tickets de altas y bajas");
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //}
                    //else
                    //{
                    //    Console.WriteLine(msgtp);
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //}


                    //////////Creación de Tickets Chat
                    string msgtkcht = "";


                    Console.WriteLine("Enviando Correos de Creación Tickets del Chat...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgtkcht = tk.NewTicketChat();

                    if (msgtkcht == "OK")
                    {
                        Console.WriteLine("Se enviaron correctamente los correos de la creación de Tickets del Chat \n");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgtkcht);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }


                    //////Document Control
                    ////DocumentControl dc = new DocumentControl();
                    ////string msgdc = "";
                    ////msgdc= dc.NewDocumentControl();

                    ////if (msgdc == "OK")
                    ////{
                    ////    Console.WriteLine("Los mensajes de creación del Document Control se han enviado correctamente: " + DateTime.Now.ToString("HH:mm"));
                    ////}
                    ////else
                    ////{
                    ////    Console.WriteLine("Error al enviar mensajes del Document Control");
                    ////}

                    ////////// TALENT
                    //////Mensaje de cumpleaños(HappyBirthday)



                    ////Reporte de Asistencia
                    //string msgtalrepasi = "";

                    //if (DateTime.Now.Hour == 18) //&& Convert.ToInt32(DateTime.Now.DayOfWeek)==1
                    //{
                    //    Console.WriteLine("Enviando Mensajes de Reporte de Asistencia... ");
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //    msgtalrepasi = tal.AttendanceReport();

                    //    if (msgtalrepasi == "OK")
                    //    {
                    //        Console.WriteLine("Se terminaron de enviar los reportes de Asistencia a las: " + DateTime.Now.ToString("HH:mm"));
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine(msgtalrepasi);
                    //    }
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //}   

                    ////Enviando información Expiración de contratos
                    //string msgtalexpcont = "";

                    //if (DateTime.Now.Hour == 5 && contexpcont == 0 && DateTime.Now.Minute <= 15)
                    //{
                    //    contexpcont = 1;
                    //    Console.WriteLine("Enviando información Expiración de contratos...");
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //    msgtalexpcont = tal.ExpirationContract();

                    //    if (msgtalexpcont == "OK")
                    //    {
                    //        Console.WriteLine("Se enviaron correctamente los mensajes de expiración de contratos");
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine(msgtalexpcont);
                    //    }

                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //}
                    //else
                    //{
                    //    if (contexpcont == 1 && DateTime.Now.Minute > 16 && DateTime.Now.Hour == 5)
                    //    {
                    //        contexpcont = 0;
                    //    }
                    //}

                    ////FINANCIAL
                    ////Caja Chica
                    Financial fin = new Financial();
                    string msgfin = "";
                    Console.WriteLine("Enviando Correos de Contabilidad...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgfin = fin.Accountability();

                    if (msgfin == "OK")
                    {
                        Console.WriteLine("Los mensajes de Contabilidad se enviaron correctamente: " + DateTime.Now.ToString("HH:mm"));
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgfin);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }

                    //Financial finPago = new Financial();
                    //string msgfinPago = "";

                    //Console.WriteLine("Enviando Correos de pago de Contabilidad...");
                    //Console.WriteLine("--------------------------------------------------------------------------------");

                    //msgfinPago = finPago.ContabilidadDeposito();

                    //if (msgfin == "OK")
                    //{
                    //    Console.WriteLine("Los mensajes de Contabilidad se enviaron correctamente: " + DateTime.Now.ToString("HH:mm"));
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //}
                    //else
                    //{
                    //    Console.WriteLine(msgfin);
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //}

                    //CORREOS CONTABILIDAD, PARA USUARIOS PENDIENTES DE DEVOLUCION A LA EMPRESA
                    Financial pendienteDevolucionEmp = new Financial();
                    string msgpendienteDev = "";

                    Console.WriteLine("Enviando Correos de Pendiente Devolución por usuario. - Contabilidad...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgpendienteDev = pendienteDevolucionEmp.ContablidadDevolucionUsuario();

                    if (msgfin == "OK")
                    {
                        Console.WriteLine("Los mensajes de Contabilidad se enviaron correctamente: " + DateTime.Now.ToString("HH:mm"));
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgfin);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }

                    ////Reporte de Proyectos

                    ////Console.WriteLine("Enviando Correos de Reporte Semanal de Proyectos...");
                    ////Console.WriteLine("--------------------------------------------------------------------------------");
                    ////Projects Proj = new Projects();
                    ////string msgProj = "";
                    ////msgProj = Proj.ReportProject();

                    ////if (msgProj == "OK")
                    ////{
                    ////    Console.WriteLine("Se envió Reporte Semanal de Proyectos Correctamente... ");
                    ////    Console.WriteLine("--------------------------------------------------------------------------------");
                    ////}
                    ////else
                    ////{
                    ////    Console.WriteLine(msgProj);
                    ////    Console.WriteLine("--------------------------------------------------------------------------------");
                    ////}

                    //Console.WriteLine("Enviando Correos de Reporte de Ventas (Docuement Sale)...");
                    //Console.WriteLine("--------------------------------------------------------------------------------");
                    //DocumentSale docusale = new DocumentSale();
                    //string msgdocusale = "";
                    //msgdocusale = docusale.SendReportDocuementSaleClient();

                    //if (msgdocusale == "OK")
                    //{
                    //    Console.WriteLine("Se enviaron correos de Reporte de Ventas Correctamente... ");
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //}
                    //else
                    //{
                    //    Console.WriteLine(msgdocusale);
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //}


                    //Envio de Correos del ticket Report
                    //if (DateTime.Now.Hour == 4 && contticketreport == 0 && DateTime.Now.Minute <= 15)
                    //{
                    //    Console.WriteLine("Enviando Correo del Reporte de Tickets...");
                    //    Console.WriteLine("--------------------------------------------------------------------------------");

                    //    string msgdticketreport = "";
                    //    msgdticketreport = tk.SendTicketReport(1);

                    //    if (msgdticketreport == "OK")
                    //    {
                    //        Console.WriteLine("Se Envió el Correo de Reporte de Tickets Correctamente");
                    //        Console.WriteLine("--------------------------------------------------------------------------------");
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Error. No se realizó en envio de reportes de tickets diario...");
                    //        Console.WriteLine("--------------------------------------------------------------------------------");
                    //    }

                    //    contticketreport = 1;
                    //}
                    //else
                    //{
                    //    if (contticketreport == 1 && DateTime.Now.Minute > 16 && DateTime.Now.Hour == 1)
                    //    {
                    //        contexpcont = 0;
                    //    }
                    //}

                    //Envio de Correos del Reporte Semanal de Actividades (Operaciones) - Domingos
                    if (DateTime.Now.Hour == 4 && contticketreport == 0 && DateTime.Now.Minute <= 15 && DateTime.Now.Day == 0)
                    {
                        Console.WriteLine("Enviando Correo del Reporte Semanal de Actividades");
                        Console.WriteLine("--------------------------------------------------------------------------------");

                        string msgRptSemAct = "";
                        msgRptSemAct = tk.EnviarRptActividades();

                        if (msgRptSemAct == "OK")
                        {
                            Console.WriteLine("Se Envió el Correo de Reporte Semanal de Actividades");
                            Console.WriteLine("--------------------------------------------------------------------------------");
                        }
                        else
                        {
                            Console.WriteLine("Error. No se realizó en envio de reportes semanal de actividades...");
                            Console.WriteLine("--------------------------------------------------------------------------------");
                        }

                        contticketreport = 1;
                    }
                    else
                    {
                        if (contticketreport == 1 && DateTime.Now.Minute > 16 && DateTime.Now.Hour == 1)
                        {
                            contexpcont = 0;
                        }
                    }

                    ///////Envío de Correos semanales de Brocal
                    //string msgtcksem = "";

                    //if (contTicketReportSemanal == 0 && DateTime.Now.Hour == 5 && DateTime.Now.Minute < 15 && Convert.ToInt32(DateTime.Now.DayOfWeek) == 1)
                    //{
                    //    Console.WriteLine("Enviando Reporte de tickets semanal Brocal... ");
                    //    Console.WriteLine("--------------------------------------------------------------------------------");

                    //    msgtcksem = tk.SendTicketReport(2);

                    //    if (msgtcksem == "OK")
                    //    {
                    //        Console.WriteLine("El reporte de ticket semanal fue enviado correctamente a las: " + DateTime.Now.ToString("HH:mm"));
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine(msgtal);
                    //    }
                    //    Console.WriteLine("--------------------------------------------------------------------------------");
                    //    contTicketReportSemanal = 1;
                    //}
                    //else
                    //{
                    //    if (contTicketReportSemanal == 1 && DateTime.Now.Minute > 16 && DateTime.Now.Hour == 5 && Convert.ToInt32(DateTime.Now.DayOfWeek) == 1)
                    //    {
                    //        contTicketReportSemanal = 0;
                    //    }
                    //}
                    ////Change Request
                    ////Solicitud de Cambios

                    ChangeRequest cr = new ChangeRequest();
                    string msgCR = "";
                    Console.WriteLine("Procediendo a Actualizar las Solicitudes a Pendientes...");
                    Console.WriteLine("--------------------------------------------------------------------------------");

                    msgCR = cr.pendingRequest();

                    if (msgCR == "OK")
                    {
                        Console.WriteLine("Las solicitudes han cambiado de Estado a Pendiente: " + DateTime.Now.ToString("HH:mm"));
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine(msgCR);
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                    ////////

                    Console.WriteLine("\n");
                    Console.WriteLine("********************************************************************************");

                    GC.Collect();

                    ////Envío de Encuestas por cuenta cada mes - Configuración Automática

                    string msgEC = "";
                    Console.WriteLine(DateTime.Now.Hour + " " + Convert.ToInt32(DateTime.Now.Day) + " " + DateTime.Now.Minute);
                    if (DateTime.Now.Hour >= 3 && DateTime.Now.Minute <= 59)
                    {
                        Controllers.EncuestaConfiguracion ec = new Controllers.EncuestaConfiguracion();
                        Console.WriteLine("Procediendo a Enviar las encuestas de satisfacción...");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                        msgEC = ec.GenerarEncuesta();
                        if (msgEC == "")
                        {
                            Console.WriteLine("Se han enviado satisfactoriamente las encuestas:.");
                            Console.WriteLine("--------------------------------------------------------------------------------");
                        }
                        else
                            if (msgEC == "0")
                        {
                            Console.WriteLine("No se han encontrado tickets para el envío de encuestas.");
                            Console.WriteLine("--------------------------------------------------------------------------------");
                        }
                        else
                                if (msgEC == "2")
                        {
                            Console.WriteLine("La cuenta no se encuentra configurada con el tipo de encuestas.");
                            Console.WriteLine("--------------------------------------------------------------------------------");
                        }
                        else
                                    if (msgEC == "3")
                        {
                            Console.WriteLine("No se han encontrado encuestas por enviar.");
                            Console.WriteLine("--------------------------------------------------------------------------------");
                        }
                        else
                                        if (msgEC == "4")
                        {
                            Console.WriteLine("Se han enviado satisfactoriamente las encuestas pendientes.");
                            Console.WriteLine("--------------------------------------------------------------------------------");
                        }
                    }

                    Console.WriteLine("\n");
                    Console.WriteLine("********************************************************************************");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("REVISAR: " + ex.ToString());
                EventLog.WriteEntry("Consola", "REVISAR: " + ex.ToString());
            }
        }


    }
}