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
    //Clase utilizada pare el envío de correos referente a las OP y vencimiento de licencias.
    class DocumentSale
    {
        //Declaracion de Objetos para acceso a base de datos.
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        //Envio de Reportes basados en las fechas de expiración de las OP.
        public string SendReportDocuementSaleClient()
        {
            int contador = 0;
            
            //Se seleccionan OP que cumplan condiciones (Fecha de fin de soporte menos a la fecha actual)
            var query = (from dds in dbc.DETAIL_DOCUMENT_SALE
                         join ds in dbc.DOCUMENT_SALE on dds.ID_DOCU_SALE equals ds.ID_DOCU_SALE
                         where dds.DAT_INI_SUPP != null
                         where dds.DAT_END_SUPP!=null
                         where ds.ID_DOCU_SALE==43
                         group dds by new { dds.ID_DOCU_SALE } into gdds
                         select new
                         {
                             ID_DOCU_SALE = gdds.Key.ID_DOCU_SALE
                         }).ToList();

            Console.WriteLine("Hoy se enviarán un total de: " + query.Count()+" correos");
            Console.WriteLine();

            //Recorrer listado de OP.
            foreach (var q in query)
            {
                Console.WriteLine("ID_DOCU_SALE: " +q.ID_DOCU_SALE);
                int ID_DOCU_SALE = Convert.ToInt32(q.ID_DOCU_SALE);

                string msgsendmail = "";

                //llamamos a función para envío de correo.
                msgsendmail = SendMailContact(ID_DOCU_SALE);

                if (msgsendmail == "OK")
                {
                    var documentsale = dbc.DOCUMENT_SALE.Find(ID_DOCU_SALE);
                    try
                    {
                        contador = Convert.ToInt32(documentsale.SEND_MAIL_CONTAC);
                    }
                    catch
                    {
                        contador = 0;
                    }

                    //Actualizamos OP solo si se ha enviado correo.
                    documentsale.SEND_MAIL_CONTAC = contador + 1;
                    dbc.DOCUMENT_SALE.Attach(documentsale);
                    dbc.Entry(documentsale).State = EntityState.Modified;
                    dbc.SaveChanges();

                    Console.WriteLine("Se actualizó correctamente la información de informacíon de envio de correo para el ID_DOCU_SALE: "+ID_DOCU_SALE);
                }


            }
            
            return "OK";
        }

        //Función encargada de realizar el envío de correos.
        public string SendMailContact(int ID_DOCU_SALE)
        {
            try
            {
                var query = dbc.DOCUMENT_SALE.Single(x=>x.ID_DOCU_SALE==ID_DOCU_SALE);
                string NAM_DOCU_SALE=dbc.TYPE_DOCUMENT_SALE.Single(x=>x.ID_TYPE_DOCU_SALE==query.ID_TYPE_DOCU_SALE).NAM_TYPE_DOCU_SALE;
                string NUM_DOCU_SALE= query.NUM_DOCU_SALE.Trim();
                int ID_CONT = Convert.ToInt32(query.ID_CTTS);
                string email = "";
                email = dbe.PERSON_ENTITY.Single(x=>x.ID_ENTI2==ID_CONT).EMA_PERS;

                //Adjuntar archivo presentación de Mesa de Ayuda
                string file = "E:\\Mesa de Servicio (EData Group).pdf";

                //Declaramos un objeto correo para proceder a enviar correo.
                SendMail newMail = new SendMail();
                var message = newMail.mailMessage;

                //Declaramos el cuerpo del correo.
                DocumentSaleTemplate body = new DocumentSaleTemplate();
                string body_html = body.GenerateTemplateContact(ID_DOCU_SALE, ID_CONT);

                if (body_html != null && String.IsNullOrEmpty(email)!=true)
                {
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);

                    message.To.Add(email);
                    message.AlternateViews.Add(htmlView);
                    message.Subject = "Listado de Productos y/o Servicios Contratados (" + NAM_DOCU_SALE + " " + NUM_DOCU_SALE+")";

                    Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                    message.Attachments.Add(attachment);

                    newMail.smtp.Send(message);

                    Console.WriteLine("To ");

                    Console.WriteLine("Se envió correo de Document_Sale con ID: " + ID_DOCU_SALE);

                    return "OK";
                }
                else
                {
                    Console.WriteLine("Error: No se pudo generar el Template en Document_Sale ID: " + ID_DOCU_SALE);
                    return "ERROR";

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo del Document_Sale ID: " + ID_DOCU_SALE);
                Console.WriteLine("Error generado: " + ex.Message.ToString());
                return "ERROR";
            }
        }
    }    
}

