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

namespace ConsoleNotification.Controllers
{
    //Clase utilizada pare el envío de correos referente al control documentario
    class DocumentControl
    {
        //Declaracion de Objetos para acceso a base de datos.
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();      

        //Acción para enviar por mail la creación o recepción de un Documento en DocumentControl.
        public string NewDocumentControl()
        {
            //Buscamos lista de documentos que tienen pendiente el envío de correos.
            var qdc = dbc.DOCUMENT_CONTROL.Where(x => x.SEND_MAIL == false).ToList();
            string mailTo, rpt;
            if(qdc.Count()>0)
            {
                try
                {
                    //Recorremos todos los documentos pendientes.
                    foreach (var docu in qdc)
                    {
                        mailTo = "";
                        rpt = "";

                        //Buscamos todos los destinatarios de entrega del documento.
                        var qdcr = dbc.DOCUMENT_CONTROL_RECEIVER.Where(x => x.ID_DOCU_CONT == docu.ID_DOCU_CONT).ToList();

                        //Agrupamos y generamos lista de correos destinatarios.
                        foreach (var docurece in qdcr)
                        {
                            var pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == docurece.ID_PERS_ENTI);

                            mailTo += (pe.EMA_ELEC != null ? pe.EMA_ELEC : "");
                        }

                        //Llamamos a función encargada de enviar el correo.
                        rpt=SendMailNewDocumentControl(mailTo,docu.ID_DOCU_CONT);

                        if(rpt=="OK")
                        {
                            //Si todo es conforme se ubica registro se modifica campo SEND_MAIL y se procede a guardar.
                            var udc = dbc.DOCUMENT_CONTROL.Find(docu.ID_DOCU_CONT);
                            udc.SEND_MAIL = true;
                            dbc.DOCUMENT_CONTROL.Attach(udc);
                            dbc.Entry(udc).State = EntityState.Modified;
                            dbc.SaveChanges();

                        }
                    }
                }
                catch
                {
                    return "ERROR";
                }

                
            }
            return "OK";
        }

        //Función encargada de construir y enviar el correo.
        public string SendMailNewDocumentControl(string mail_to, int iddc)//
        {
            try
            {
                //Declaramos el cuerpo del correo.
                DocumentControlTemplate body = new DocumentControlTemplate();
                string body_html = body.NewDocumentControl(iddc);

                //Declaramos un objeto correo para proceder a enviar correo.
                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;

                message.To.Add(mail_to);
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
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


    }
}
