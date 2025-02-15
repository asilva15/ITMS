using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using ERPElectrodata.Objects;
using ERPElectrodata.Object.Talent;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class testController : Controller
    {
        private EntityEntities dbe = new EntityEntities();
        private CoreEntities db = new CoreEntities();

        public ActionResult TestOP(int id)
        {
            var email = new SendMail();
            email.Document_Sale(id);

            return Content("OK");
        }

        public ActionResult PostSignature()
        {
            //SendMail mail = new SendMail();
            BodyGTH _body = new BodyGTH();
            //mail.ExpirationContract(27);

            var list = dbe.POST_SIGNATURE(1).ToList();
            string xx = null;
            foreach (POST_SIGNATURE_Result per in list)
            {
                //gth _gth = new gth();
                //_gth.PostSignature(per);
                 xx =  _body.Signature(per);
            }

            return Content(xx);
        }

        public ActionResult Assistance()
        {
            return View();
        }

        public ActionResult SLA()
        {
            return View();
        }
        public ActionResult ListByStatus(int id = 0)
        {
            //textInfo = cultureInfo.TextInfo;
            var ctd = 1;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                ce.UserId,
                            }).ToList();

            var listCIA = (from pe in dbe.PERSON_ENTITY
                           join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               pe.ID_PERS_ENTI,
                               COMPANY = ce.COM_NAME,
                           }).ToList();


            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            var tick = db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO);
            int Count = 0, iq = 0;
            int[] numbers = new int[0];

            if (Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString()) == false)
            {
                int supQueu = 0;
                try
                {
                    var Queus = db.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
                        .Where(x => x.ID_PERS_ENTI_CORD == ID_PERS_ENTI);

                    supQueu = Queus.Count();

                    numbers = new int[supQueu];

                    foreach (var x in Queus)
                    {
                        //var orderKeys = new int[] { 1, 12, 306, 284, 50047 };
                        numbers[iq] = (int)x.ID_QUEU.Value;
                        iq++;
                        //Customers.Rows.Add(row);
                    }
                }
                catch
                {

                }

                if (supQueu == 0)
                {
                    tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                }
                else
                {
                    //Mostrar el ticket cuya cola es la indicada
                    tick = tick.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                }


            }

            if (ID_STAT == 0)
            {
                tick = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.IS_ROLE_USER).ThenBy(t => t.ID_PRIO).ThenByDescending(t => t.CREATE_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 5)
            {
                tick = tick.Where(i => i.ID_STAT_END == 5);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 4)
            {
                tick = tick.Where(i => i.ID_STAT_END == 4);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 6)
            {
                tick = tick.Where(i => i.ID_STAT_END == 6);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }

            var result = (from i in tick.ToList()
                          join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in db.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in db.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE
                          join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listuser on i.UserId equals cr.UserId
                          join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listuser on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                          join ds in db.DOCUMENT_SALE on (i.ID_DOCU_SALE == null ? 0 : i.ID_DOCU_SALE) equals ds.ID_DOCU_SALE into lds
                          from xds in lds.DefaultIfEmpty()
                          join tds in db.TYPE_DOCUMENT_SALE on (xds == null ? 0 : xds.ID_TYPE_DOCU_SALE) equals tds.ID_TYPE_DOCU_SALE into ltds
                          from xtds in ltds.DefaultIfEmpty()
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBCLAS = c4.NAM_CATE.ToLower(),
                              NAM_CLAS = c3.NAM_CATE.ToLower(),
                              NAM_SUBC = c2.NAM_CATE.ToLower(),
                              NAM_CATE = c1.ABR_CATE,
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                              //REQU = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                              CREATE_INCI = String.Format("{0:d}", i.CREATE_TICK) + " " + String.Format("{0:HH:mm}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:d}", i.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              HOU_PRIO = pr.HOU_PRIO != 0 ? Convert.ToString(pr.HOU_PRIO) + "h" : "",
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client.ToLower(),
                              ACCO = ac.NAM_ACCO,
                              ASSI = asi.Client.ToLower(),
                              //EXP_TIME = tir.ExpirationTime(i.ID_TICK, pr.HOU_PRIO.Value),//ExpTime(i.ID_TICK),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none"),
                              //COUNTSON = CountTicketSon(i.ID_TICK),
                              //CIA = textInfo.ToTitleCase(cia.COMPANY.ToLower()).Substring(0, (cia.COMPANY.Length > 48 ? 48 : cia.COMPANY.Length)) +
                              //      (cia.COMPANY.Length > 48 ? "..." : ""),
                              //CIA_TOOL = textInfo.ToTitleCase(cia.COMPANY.ToLower()),
                              //ID_ACCO,
                              //VIS_COMP = ac.VIS_COMP,
                              //i.ID_STAT_END,
                              //NUM_OP = (xds == null ? "" : xds.NUM_DOCU_SALE),
                              //COD_TYPE_DOCU_SALE = (xtds == null ? "" : xtds.COD_TYPE_DOCU_SALE),
                              //ID_DOCU_SALE = (xds == null ? 0 : xds.ID_DOCU_SALE),
                              ////DATE_INI = String.Format("{0:MMM d yyyy HH:mm:ss}", Convert.ToDateTime(i.CREATE_TICK).AddHours(Convert.ToInt32(pr.HOU_PRIO))),
                              //Seque = contador(),
                              //DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_TICK), Convert.ToDateTime(i.FEC_INI_TICK), Convert.ToInt32(pr.HOU_PRIO)),
                              //DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : "",
                              //HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : ""
                          });

            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Notification()
        {
            SendMail mail = new SendMail();
            mail.ExpirationContract(27);

            //SendMail mail = new SendMail();
            //string xx = mail.Notification();
            return Content("");
        }

        public ActionResult SendCumple()
        {
            SendMail mail = new SendMail();
            string xx = mail.Cumple(100);
            return Content(xx);
        }
        //
        // GET: /test/
        public int sendMail()
        {
            return 0;
        }
        public ActionResult SendmailDos()
        {
            string yourEmail = "itms@electrodata.com.pe";//"esalazar@electrodata.com.pe";

            CDO.Message message = new CDO.Message();
            CDO.IConfiguration configuration = message.Configuration;
            ADODB.Fields fields = configuration.Fields;

            Console.WriteLine(String.Format("Configuring CDO settings..."));

            // Set configuration.
            // sendusing:               cdoSendUsingPort, value 2, for sending the message using the network.
            // smtpauthenticate:     Specifies the mechanism used when authenticating to an SMTP service over the network.
            //                                  Possible values are:
            //                                  - cdoAnonymous, value 0. Do not authenticate.
            //                                  - cdoBasic, value 1. Use basic clear-text authentication. (Hint: This requires the use of "sendusername" and "sendpassword" fields)
            //                                  - cdoNTLM, value 2. The current process security context is used to authenticate with the service.

            ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
            field.Value = "smtp.zoho.com";

            field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
            field.Value = 465;

            field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
            field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

            field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
            field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

            field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
            field.Value = "itms@electrodata.com.pe";//"esalazar@electrodata.com.pe";

            field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
            field.Value = "13579it";//;"45352290";

            field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
            field.Value = "true";

            fields.Update();

            //Console.WriteLine(String.Format("Building CDO Message..."));
            
            message.From = yourEmail;
            //message.CC = "carevalo@electrodata.com.pe";
            //message.To = "esalazar@electrodata.com.pe;ronald.salazar@outlook.com";
            message.To = "jquisper@electrodata.com.pe";//"jvillafana@electrodata.com.pe";//"esalazar@electrodata.com.pe";
            //message.CC = "jvillafana@electrodata.com.pe";
            message.Subject = "New Ticket / Nuevo Boleto - INGF114000796 Assigned / Asignado P4";
//            message.HTMLBody = @"<style>
//td{
//    border-bottom:#B21E1E 1px solid;
//}
//</style>
//<table cellpadding=""2"" cellspacing=""0"">
//<tr >
//    <td colspan=""3"" style=""background-color:#B21E1E;color:white;"">INCH113000003</td>
//</tr>
//<tr style=""border-bottom:#B21E1E 1px solid;"">
//    <td style=""border-bottom:#B21E1E 1px solid;""><strong>Usuario Afectado</strong></td><td>:</td>
//    <td>Violeta Malca</td>
//</tr>
//<tr style=""border-bottom:#B21E1E 1px solid;"">
//    <td><strong>Prioridad</strong></td><td>:</td>
//    <td>Medium</td>
//</tr>
//<tr style=""border-bottom:r#B21E1E 1px solid;"">
//    <td><strong>Estado</strong></td><td>:</td>
//    <td>Assigned</td>
//</tr>
//<tr style=""border-bottom:r#B21E1E 1px solid;"">
//    <td><strong>Categoría</strong></td><td>:</td>
//    <td>ICT Infraestructure</td>
//</tr>
//<tr style=""border-bottom:r#B21E1E 1px solid;"">
//    <td><strong>Personal Asignado</strong></td><td>:</td>
//    <td>Ivan Plasencia</td>
//</tr>
//<tr style=""border-bottom:r#B21E1E 1px solid;"">
//    <td><strong>Creado Por</strong></td><td>:</td>
//    <td>System Manager</td>
//</tr>
//<tr style=""border-bottom:r#B21E1E 1px solid;"">
//    <td><strong>SLA</strong></td><td>:</td>
//    <td>24 Horas</td>
//</tr>
//<tr style=""border-bottom:r#B21E1E 1px solid;"">
//    <td><strong>Fecha Creacion</strong></td><td>:</td>
//    <td>2013-08-19 15:45:00.000</td>
//</tr>
//<tr style=""border-bottom:r#B21E1E 1px solid;"">
//    <td><strong>Fecha Vencimiento</strong></td><td>:</td>
//    <td>2013-08-20 15:45:00.000</td>
//</tr>
//</table>";
            Random rnd = new Random();
            decimal xx = rnd.Next(1, 1000);

            message.CreateMHTMLBody("http://localhost:50143/Mail/SendMail/16300?_="+Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone, "Administrator", "123456");
            //message.CreateMHTMLBody("http://190.8.136.204/", CDO.CdoMHTMLFlags.cdoSuppressNone, "Administrator", "123456");

            //Console.WriteLine(String.Format("Attempting to connect to remote server..."));

            // Send message.
            message.Send();

            //http://localhost:50143/test/SendmailDos
            return Content("OK");
        }
        public ActionResult SendEmail()
        {
            MailMessage mail = new MailMessage();
            //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            SmtpClient SmtpServer = new SmtpClient("smtp.zoho.com",465);
            
            //mail.From = new MailAddress("esalazar@electrodata.com.pe", "helpdesk@goldfields.com");
            //mail.To.Add("esalazar@electrodata.com.pe");
            //mail.Subject = "Test Send E-Mail";
            //mail.IsBodyHtml = true;
            //mail.Body = @"<div style=""background-color:#7CBB00;color:white;"">Testing<div>";
            ////Attachment attachment = new Attachment(filename);
            ////mail.Attachments.Add(attachment);
            ////SmtpServer.UseDefaultCredentials = false;
            //SmtpServer.EnableSsl = true;
            //SmtpServer.Credentials = new System.Net.NetworkCredential("ronald.salazar1988@gmail.com", "gnr889131ronald");
            //mail.From = new MailAddress("esalazar@electrodata.com.pe", "helpdesk@goldfields.com");
            //SmtpServer.Port = 25;

            //SmtpServer.Send(mail);

            mail.From = new MailAddress("jquisper@electrodata.com.pe", "helpdesk@goldfields.com");
            mail.To.Add("jquisper@electrodata.com.pe");
            mail.Subject = "Test Send E-Mail";
            mail.IsBodyHtml = true;
            mail.Body = @"<div style=""background-color:#7CBB00;color:white;"">Testing<div>";
            //Attachment attachment = new Attachment(filename);
            //mail.Attachments.Add(attachment);
            //SmtpServer.UseDefaultCredentials = false;
            SmtpServer.EnableSsl = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("jquisper@electrodata.com.pe", "45352290");
            
            SmtpServer.Port = 465;

            SmtpServer.Send(mail);

            return Content("OK");
        }


        public ActionResult TestGoogleMail()
        {
            SendMail mail = new SendMail();
            string xx = mail.Test();
            if (xx == "OK")
            {
                return Content("Mensaje enviado correctamente");
            }
            else
            {
                return Content("Error en el envio.");
            }
        }

    }
}
