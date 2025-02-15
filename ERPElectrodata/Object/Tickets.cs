using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.IO;
using ERPElectrodata.Object;
using System.Data.Entity;
using System.Configuration;
using System.Globalization;
using ERPElectrodata.Objects;
using System.Threading;

namespace ERPElectrodata.Object
{
    public class Tickets
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private AppLogEntities dbl = new AppLogEntities();

        private string IpServer = "";
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        //private Encryption seg = new Encryption(); 
        private AESRijndael seg = new AESRijndael();

        public Tickets()
        {
            IpServer = ConfigurationManager.AppSettings["IpServer"].ToString();
            textInfo = cultureInfo.TextInfo;
        }

        public string GeneraEncuesta(int idEncuestaConfiguracion)
        {
            var queryEncuestas = dbe.EncuestaConfiguracions.Where(x => x.IdEncuestaConfiguracion == idEncuestaConfiguracion).SingleOrDefault();
            int top = Convert.ToInt32(queryEncuestas.NroEncuestas);
            var query = dbc.TICKETs.Where(x => x.SEND_SURVEY == null && x.ID_ACCO == queryEncuestas.IdAcco.Value)
                .Where(x => x.ID_STAT_END == 6)
                .Where(x => x.ID_TYPE_FORM == null).OrderByDescending(x => x.FEC_TICK).Take(top).ToList();
             if (query.Count() != 0)
            {
                foreach (var ti in query)
                {
                    int ID_SURV_TICK = InsertaPreguntas(ti);
                    if (ID_SURV_TICK != 0 && ID_SURV_TICK > 0)
                    {
                        EnviarEncuesta(ti, ID_SURV_TICK);
                        var fechaUltimoEnvio = dbe.ActualizarFechaUltimaEnvio(idEncuestaConfiguracion);
                    }
                    else
                        if (ID_SURV_TICK == -1) {
                            return "2";    
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

                //var tic = dbc.TICKETs.Single(x => x.ID_TICK == 84622);
                var pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == tic.ID_PERS_ENTI);
                var ce = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == pe.ID_ENTI2.Value);
                int cq = dbe.QUESTION_TICKET.Where(x => x.ID_SURV_TICK == ID_SURV_TICK).Count();

                if (!String.IsNullOrEmpty(pe.EMA_PERS))
                {
                    BodySurvey bodySurvey = new BodySurvey();
                    SendMail newMail = new SendMail();
                    var msg = newMail.mailMessage;
                    var html_msg = bodySurvey.htmlSendSurvey(pe, ce, ID_SURV_TICK, cq, tic.ID_ACCO.Value);
                    string text = "";
                    if (tic.ID_ACCO == 1)
                    {
                        text = (ce.SEX_ENTI == "M" ? "Estimado " : ce.SEX_ENTI == "F" ? "Estimada " : "") + textInfo.ToTitleCase(ce.FIR_NAME.ToLower()) + ", " +
                            "Nuestro principal objetivo es brindarle un mejor servicio, por eso solicitamos tu ayuda completando la siguiente encuesta." +
                            "Por favor copie y pege el siguiente link en su navegador web: " + IpServer + "/Survey/Index?idst=" + Convert.ToString(ID_SURV_TICK);
                    }
                    else
                    {
                        text = (ce.SEX_ENTI == "M" ? "Estimado " : ce.SEX_ENTI == "F" ? "Estimada " : "") + textInfo.ToTitleCase(ce.FIR_NAME.ToLower()) + ", " +
                            "Nuestro principal objetivo es brindarle un mejor servicio, por eso solicitamos tu ayuda completando la siguiente encuesta que consta de " + Convert.ToString(cq) + " preguntas" +
                            "Por favor copie y pege el siguiente link en su navegador web: " + IpServer + "/Survey/Index?idst=" + Convert.ToString(ID_SURV_TICK);
                    }
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html_msg, Encoding.UTF8, MediaTypeNames.Text.Html);

                    string xx = Path.GetTempPath();

                    string serv = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/");

                    //Insertar Imagenes al cuerpo del Mensaje
                    //Facebook
                    LinkedResource imgfb = new LinkedResource(serv + "\\Iconos\\icon-fb.png", MediaTypeNames.Image.Jpeg);
                    imgfb.ContentId = "icon-fb";
                    imgfb.TransferEncoding = TransferEncoding.Base64;
                    htmlView.LinkedResources.Add(imgfb);

                    //LinkedIn
                    LinkedResource imgin = new LinkedResource(serv + "\\Iconos\\icon-in.png", MediaTypeNames.Image.Jpeg);
                    imgin.ContentId = "icon-in";
                    imgin.TransferEncoding = TransferEncoding.Base64;
                    htmlView.LinkedResources.Add(imgin);

                    //Is
                    LinkedResource imgis = new LinkedResource(serv + "\\Iconos\\icon-is.png", MediaTypeNames.Image.Jpeg);
                    imgis.ContentId = "icon-is";
                    imgis.TransferEncoding = TransferEncoding.Base64;
                    htmlView.LinkedResources.Add(imgis);

                    //YouTube
                    LinkedResource imgyt = new LinkedResource(serv + "\\Iconos\\icon-yt.png", MediaTypeNames.Image.Jpeg);
                    imgyt.ContentId = "icon-yt";
                    imgyt.TransferEncoding = TransferEncoding.Base64;
                    htmlView.LinkedResources.Add(imgyt);

                    //Boton Llenar Encuesta
                    LinkedResource btnle = new LinkedResource(serv + "\\Botones\\llenarEncuesta.png", MediaTypeNames.Image.Jpeg);
                    btnle.ContentId = "btn-le";
                    btnle.TransferEncoding = TransferEncoding.Base64;
                    htmlView.LinkedResources.Add(btnle);

                    //Logo e-data-group
                    string file = serv + "\\logo-" + Convert.ToString(tic.ID_ACCO) + ".png";
                    var existefile = File.Exists(file);

                    LinkedResource btnedgw = null;// new LinkedResource(serv + "\\logo-4.png", MediaTypeNames.Image.Jpeg);
                    if (existefile)
                    {
                        btnedgw = new LinkedResource(file);
                    }
                    else
                    {
                        btnedgw = new LinkedResource(serv + "\\logo-4.png", MediaTypeNames.Image.Jpeg);
                    }

                    btnedgw.ContentId = "log-edgw";
                    btnedgw.TransferEncoding = TransferEncoding.Base64;
                    htmlView.LinkedResources.Add(btnedgw);

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

    }
}