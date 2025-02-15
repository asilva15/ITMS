using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using System.Configuration;
using System.Threading;
using ERPElectrodata.Models;
using System.Text;
using System.Net;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using ERPElectrodata.Objects;
using ERPElectrodata.Object.Attendance;
using ERPElectrodata.Object.Talent;
using System.Data.Entity;
using System.Web.Helpers;

namespace ERPElectrodata
{
    // Nota: para obtener instrucciones sobre cómo habilitar el modo clásico de IIS6 o IIS7, 
    // visite http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes); 
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            //AntiForgeryConfig.RequireSsl = false;
            // Habilitar el enrutamiento
            //RegisterRoutes(RouteTable.Routes);
            //GlobalFilters.Filters.Add(new ERPElectrodata.Filters.ValidateAntiForgeryTokenAttribute());
            //GlobalFilters.Filters.Add(new ());
            //GlobalFilters.Filters.Add(new ERPElectrodata.Filters.ValidateAntiForgeryTokenAttribute());

            //CSRFConfig.RegisterCSRFProtection();
            //GlobalFilters.Filters.Add(new System.Web.Mvc.ValidateAntiForgeryTokenAttribute());

            ////no descomentar//Application["MyThread"] = new System.Threading.Timer(new System.Threading.TimerCallback(leertxt), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 1, 0, 0, 0));
            //Application["MyThread"] = new System.Threading.Timer(new System.Threading.TimerCallback(InsertAtta), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 1,0, 0, 0));
            //Application["HappyBirthday"] = new System.Threading.Timer(new System.Threading.TimerCallback(HappyBirthday), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 1, 0, 0, 0));
            //Application["MyAlertSLA"] = new System.Threading.Timer(new System.Threading.TimerCallback(AlertSLA), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 0,5, 0, 0));
            //Application["MyPreparaArchivo"] = new System.Threading.Timer(new System.Threading.TimerCallback(AttendanceReport), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(1,0, 0,0, 0));
            //Application["Temporal"] = new System.Threading.Timer(new System.Threading.TimerCallback(Post_Signature), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0,2, 0, 0, 0));

        }

        public void Post_Signature(object state)
        {
            var list = dbe.POST_SIGNATURE(4).ToList();

            foreach (POST_SIGNATURE_Result per in list)
            {
                gth _gth = new gth();
                _gth.PostSignature(per);
            }
        }

        public void AttendanceReport(object state)
        {
            try
            {
                var fecha = DateTime.Now;
                int diaLunes = Convert.ToInt32(fecha.DayOfWeek);
                //if (diaLunes == 1 && fecha.Hour == 18)
                if (fecha.Hour == 18)
                {
                    var query = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_STAT == 1)
                                                 .Where(x => x.ID_SUB_CIA == 9 || x.ID_SUB_CIA == 3975 || x.ID_SUB_CIA == 4550).ToList()
                                 join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                 join pc in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true) on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                                 where pe.ID_PERS_ENTI != 8352 && pe.ID_PERS_ENTI != 1131
                                 //where pe.ID_PERS_ENTI == 234
                                 select new
                                 {
                                     ID_PERS_ENTI = pe.ID_PERS_ENTI,
                                     FIR_NAME = ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower(),
                                     DESC_FILE = ce.NUM_TYPE_DI,
                                     EMA_ELEC = pe.EMA_ELEC,
                                     SEX_ENTI = ce.SEX_ENTI,
                                     FIR_NAME1 = ce.FIR_NAME.ToLower(),
                                 }).ToList();

                    //for (int i = 0; i < query.Count(); i++)
                    foreach (var pers in query)
                    {
                        try
                        {
                            int ID_PERS_ENTI = pers.ID_PERS_ENTI;
                            string DESC_FILE = pers.DESC_FILE;
                            try
                            {
                                var q = dbe.PERSON_ENTITY_NOTIFICATION
                                    .Where(x => x.ID_PERS_ENTI == pers.ID_PERS_ENTI && x.ID_PERS_NOTI == 5)
                                    .Where(x => x.CREATED.Value.Day == fecha.Day)
                                    .Where(x => x.CREATED.Value.Month == fecha.Month)
                                    .Where(x => x.CREATED.Value.Year == fecha.Year)
                                    .Count();

                                if (q == 0)
                                {
                                    //string msg = mail.sendreportattendance(user.EMAIL_ELECTRODATA, firmaenString);
                                    ReportAttendance rpt = new ReportAttendance();
                                    string AR = rpt.Attendance(ID_PERS_ENTI, DESC_FILE, pers.SEX_ENTI, pers.FIR_NAME1, pers.EMA_ELEC);

                                    if (AR == "OK")
                                    {
                                        //DateTime fecha = DateTime.Now;
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
                            catch
                            {
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
                        }
                    }

                }



            }
            catch
            {

            }
        }

        private void AlertSLA(object state)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            string name = cultureInfo.Name;

            if (name != "en-US")
            {
                CultureInfo ci = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }

            //CultureInfo cultureInfox = Thread.CurrentThread.CurrentCulture;
            //string namex = cultureInfox.Name;

            SendMail mail = new SendMail();
            string xx = mail.Notification();

            string yy = mail.UpdateStatusTicketScheduled();

        }
        //private void RegisterRoutes(RouteCollection routes)
        //{
        //    routes.MapPageRoute("", "DetailsTicket/Index/{id}", "~/DetailsTicket/Index.aspx");
        //}
        private void HappyBirthday(object state)
        {
            SendMail mail = new SendMail();
            int year = DateTime.Now.Year;
            int mes = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int mesR = DateTime.Now.AddDays(1).Month;
            int dayR = DateTime.Now.AddDays(1).Day;
            //int cont_x = 0;
            DateTime ultimodia = new DateTime(year, mes, 28);//.AddMonths(1).AddDays(-1);
            DateTime date_remember = DateTime.Now;//ultimodia;

            //si el último día es viernes
            if (Convert.ToInt32(date_remember.DayOfWeek) == 4)
            {
                date_remember = date_remember.AddDays(-1);
            }

            //Obtenemos el ultimo viernes para el recodatorio
            while (Convert.ToInt32(date_remember.DayOfWeek) != 2)
            {
                date_remember = date_remember.AddDays(-1);
            }

            if (DateTime.Now.Hour == 9 && DateTime.Now.Day == 28)
            {
                if (mes == date_remember.Month && day == date_remember.Day)
                {
                    try
                    {
                        int send = dbe.PERSON_ENTITY_NOTIFICATION.Where(x => x.ID_PERS_ENTI == 1007)
                                .Where(x => x.CREATED.Value.Year == year)
                                .Where(x => x.CREATED.Value.Month == mes)
                                .Where(x => x.ID_PERS_NOTI == 1).Count();

                        if (send == 0)
                        {
                            string result = mail.CumpleStaffMesRemember();

                            var perEntNot = new PERSON_ENTITY_NOTIFICATION();
                            perEntNot.ID_PERS_ENTI = 1007;
                            perEntNot.ID_PERS_NOTI = 1;
                            perEntNot.CREATED = DateTime.Now;
                            perEntNot.UserId = 29;
                            dbe.PERSON_ENTITY_NOTIFICATION.Add(perEntNot);
                            dbe.SaveChanges();
                        }

                    }
                    catch
                    {

                    }

                }
            }

            if (DateTime.Now.Hour == 9 && DateTime.Now.Day == 1)
            {
                if (mes == date_remember.Month && day == date_remember.Day)
                {
                    try
                    {
                        int send = dbe.PERSON_ENTITY_NOTIFICATION.Where(x => x.ID_PERS_ENTI == 1007)
                                .Where(x => x.CREATED.Value.Year == year)
                                .Where(x => x.CREATED.Value.Month == mes)
                                .Where(x => x.ID_PERS_NOTI == 2).Count();

                        if (send == 0)
                        {
                            //string result = mail.CumpleStaffMesRemember();

                            string result = mail.CumpleStaffMes();

                            var perEntNot = new PERSON_ENTITY_NOTIFICATION();
                            perEntNot.ID_PERS_ENTI = 1007;
                            perEntNot.ID_PERS_NOTI = 2;
                            perEntNot.CREATED = DateTime.Now;
                            perEntNot.UserId = 29;
                            dbe.PERSON_ENTITY_NOTIFICATION.Add(perEntNot);
                            dbe.SaveChanges();
                        }

                    }
                    catch
                    {

                    }

                }
            }

            if (DateTime.Now.Hour == 13)
            {
                try
                {
                    var query = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.BIRTHDAY != null).ToList()
                                 join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                                 join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                                 where ae.ID_ACCO == 4
                                 where pe.ID_PERS_STAT == 1
                                 where ae.VIS_TALE == true
                                 where ce.BIRTHDAY.Value.Month == mes && ce.BIRTHDAY.Value.Day == day
                                 select new { pe.ID_PERS_ENTI }).ToList();

                    for (int i = 0; i < query.Count; i++)
                    {
                        try
                        {
                            int ID_PERS_ENTI = Convert.ToInt32(query[i].ID_PERS_ENTI);

                            int send = dbe.PERSON_ENTITY_NOTIFICATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                .Where(x => x.CREATED.Value.Year == year)
                                .Where(x => x.ID_PERS_NOTI == 1).Count();

                            if (send == 0)
                            {

                                string HB = mail.Cumple(ID_PERS_ENTI);

                                var perEntNot = new PERSON_ENTITY_NOTIFICATION();
                                perEntNot.ID_PERS_ENTI = ID_PERS_ENTI;
                                perEntNot.ID_PERS_NOTI = 1;
                                perEntNot.CREATED = DateTime.Now;
                                perEntNot.UserId = 29;
                                dbe.PERSON_ENTITY_NOTIFICATION.Add(perEntNot);
                                dbe.SaveChanges();

                            }
                            else
                            {
                                var exc = new EXCEPTION();
                                exc.DAT_EXCE = DateTime.Now;
                                exc.MESSAGE = "Error Threading Global Asax";
                                exc.DES_EXCE = "Send Mail - HappyBirthday";
                                dbl.EXCEPTIONs.Add(exc);
                                dbl.SaveChanges();
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
                        }
                    }
                }
                catch
                {

                }
                //recordatorio de Cumpleaños
                try
                {
                    var query = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.BIRTHDAY != null).ToList()
                                 join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                                 join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                                 where ae.ID_ACCO == 4
                                 where pe.ID_PERS_STAT == 1
                                 where ae.VIS_TALE == true
                                 where ce.BIRTHDAY.Value.Month == mesR && ce.BIRTHDAY.Value.Day == dayR
                                 select new { pe.ID_PERS_ENTI }).ToList();

                    for (int i = 0; i < query.Count; i++)
                    {

                        try
                        {
                            int ID_PERS_ENTI = Convert.ToInt32(query[i].ID_PERS_ENTI);

                            int send = dbe.PERSON_ENTITY_NOTIFICATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                                .Where(x => x.CREATED.Value.Year == year)
                                .Where(x => x.ID_PERS_NOTI == 2).Count();

                            if (send == 0)
                            {
                                //SendMail mail = new SendMail();
                                string HB = mail.RecordatorioCumple(ID_PERS_ENTI);

                                var perEntNot = new PERSON_ENTITY_NOTIFICATION();
                                perEntNot.ID_PERS_ENTI = ID_PERS_ENTI;
                                perEntNot.ID_PERS_NOTI = 2;
                                perEntNot.CREATED = DateTime.Now;
                                perEntNot.UserId = 29;
                                dbe.PERSON_ENTITY_NOTIFICATION.Add(perEntNot);
                                dbe.SaveChanges();

                            }
                            else
                            {
                                var exc = new EXCEPTION();
                                exc.DAT_EXCE = DateTime.Now;
                                exc.MESSAGE = "Error Threading Global Asax";
                                exc.DES_EXCE = "Send Mail - HappyBirthday";
                                dbl.EXCEPTIONs.Add(exc);
                                dbl.SaveChanges();
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
                        }
                    }
                }
                catch
                {

                }

            }

        }

        //private void HappyBirthday(object state)
        //{
        //    int mes = DateTime.Now.Month;
        //    int day = DateTime.Now.Day;

        //    int mesR = DateTime.Now.AddDays(1).Month;
        //    int dayR = DateTime.Now.AddDays(1).Day;

        //    if (DateTime.Now.Hour == 2)
        //    {
        //        try
        //        {
        //            var query = (from ce in dbe.CLASS_ENTITY.Where(ce=>ce.BIRTHDAY != null).ToList()
        //                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
        //                         join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
        //                         where ae.ID_ACCO == 4
        //                         where pe.ID_PERS_STAT == 1
        //                         where ae.VIS_TALE == true
        //                         where ce.BIRTHDAY.Value.Month == mes && ce.BIRTHDAY.Value.Day == day
        //                         select new { pe.ID_PERS_ENTI }).ToList();

        //            for (int i = 0; i < query.Count; i++)
        //            {
        //                int xx = Convert.ToInt32(query[i].ID_PERS_ENTI);
        //                SendMail mail = new SendMail();
        //                string HB = mail.Cumple(xx);
        //            }
        //        }
        //        catch
        //        {

        //        }

        //        //recordatorio de Cumpleaños
        //        try
        //        {
        //            var query = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.BIRTHDAY != null).ToList()
        //                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
        //                         join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
        //                         where ae.ID_ACCO == 4
        //                         where pe.ID_PERS_STAT == 1
        //                         where ae.VIS_TALE == true
        //                         where ce.BIRTHDAY.Value.Month == mesR && ce.BIRTHDAY.Value.Day == dayR
        //                         select new { pe.ID_PERS_ENTI }).ToList();

        //            for (int i = 0; i < query.Count; i++)
        //            {
        //                int xx = Convert.ToInt32(query[i].ID_PERS_ENTI);
        //                SendMail mailX = new SendMail();
        //                string HB = mailX.RecordatorioCumple(xx);

        //            }
        //        }
        //        catch
        //        {

        //        }

        //    }

        //}

        private void InsertAtta(object state)
        {
            try
            {
                if (DateTime.Now.Hour == 8)
                {
                    //Sacar Todas las cuentas que tienen FTPSW
                    var A = dbc.ACCOUNT_PARAMETER
                        .Where(pc => pc.ID_PARA == 10 && pc.VIG_ACCO_PARA == true);

                    foreach (var a in A.ToList())
                    {
                        //Obtenemos los parámetros a configurar, url(ftp), usuario, clave, dominio y carpeta donde se guardarán los archivos.
                        int ID_ACCO = Convert.ToInt32(a.ID_ACCO);
                        string url = Convert.ToString(a.VAL_ACCO_PARA);
                        string user = Convert.ToString(dbc.ACCOUNT_PARAMETER.Single(ap => ap.ID_ACCO == ID_ACCO && ap.ID_PARA == 11 && ap.VIG_ACCO_PARA == true).VAL_ACCO_PARA);
                        string clave = Convert.ToString(dbc.ACCOUNT_PARAMETER.Single(ap => ap.ID_ACCO == ID_ACCO && ap.ID_PARA == 12 && ap.VIG_ACCO_PARA == true).VAL_ACCO_PARA);
                        string dominio = Convert.ToString(dbc.ACCOUNT_PARAMETER.Single(ap => ap.ID_ACCO == ID_ACCO && ap.ID_PARA == 13 && ap.VIG_ACCO_PARA == true).VAL_ACCO_PARA);
                        string carpeta = Convert.ToString(dbc.ACCOUNT_PARAMETER.Single(ap => ap.ID_ACCO == ID_ACCO && ap.ID_PARA == 14 && ap.VIG_ACCO_PARA == true).VAL_ACCO_PARA);

                        //Conseguimos todos los nombres de los archivos
                        StringBuilder result = new StringBuilder();
                        FtpWebRequest ftp = ((FtpWebRequest)FtpWebRequest.Create(url));
                        ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                        NetworkCredential cr = new NetworkCredential(user, clave, dominio);
                        ftp.Credentials = cr;
                        FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
                        Stream responseStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(responseStream);
                        string line = reader.ReadLine();

                        while (line != null)
                        {
                            result.Append(line);
                            result.Append("\n");
                            line = reader.ReadLine();
                        }

                        result.Remove(result.ToString().LastIndexOf('\n'), 1);
                        var files = result.ToString().Split('\n');
                        //Fin Nombre de todos los archivos

                        //Recorremos el listado de archivos y Descargamos cada uno.
                        foreach (string file in files)
                        {
                            try
                            {
                                string uri = url + file;
                                Uri serverUri = new Uri(uri);

                                if (serverUri.Scheme == Uri.UriSchemeFtp)
                                {
                                    //Ingresamos las credenciales a utilizar.
                                    FtpWebRequest rftp = (FtpWebRequest)WebRequest.Create(uri);
                                    rftp.Method = WebRequestMethods.Ftp.DownloadFile;
                                    rftp.Credentials = cr;
                                    FtpWebResponse responseftp = (FtpWebResponse)rftp.GetResponse();
                                    Stream responseStreamftp = responseftp.GetResponseStream();
                                    StreamReader readerftp = new StreamReader(responseStreamftp);
                                    string serv = System.Web.Hosting.HostingEnvironment.MapPath("~/Asset/");

                                    //Escribimos el archivo en la carpeta.
                                    StreamWriter writer = new StreamWriter(serv + carpeta + "\\SW\\" + file, false);
                                    writer.Write(readerftp.ReadToEnd());
                                    readerftp.Close();
                                    responseftp.Close();
                                    writer.Close();
                                }
                            }
                            catch (WebException e)
                            {
                                String status = ((FtpWebResponse)e.Response).StatusDescription;
                            }
                        }
                        //Fin Descarga de archivos
                        reader.Close();
                        response.Close();

                        int ida = Convert.ToInt32(a.ID_ACCO);

                        dbc.DEL_SOFT(ida);
                        //leer los archivos txt y guardar los registros en base de datos.
                        leertxt(ida, carpeta);
                    }
                }
            }
            catch (Exception e)
            {

            }

        }

        private void leertxt(int IdAcco, string carpeta)
        {
            //Accedemos a cadena de conexión
            string miCadenaConexion = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
            SqlConnection cnx = new SqlConnection(miCadenaConexion);
            cnx.Open();
            int i = 0;

            //Buscamos en el servidor local los archivos descargados.
            string serv = System.Web.Hosting.HostingEnvironment.MapPath("~/Asset/");
            var files = Directory.GetFiles(serv + carpeta + "\\SW\\", "*.txt");

            try
            {
                //Recorremos cada archivo encontrado.
                foreach (var file in files)
                {
                    DataTable dt = new DataTable();
                    StreamReader sr = new StreamReader(file);
                    string line = null;

                    //Recorremos el archivo en cuanto la línea no esté en blanco.
                    while ((line = sr.ReadLine()) != null)
                    {
                        //agregamos el campo ID_ACCO al final de la estructura.
                        string[] data = (line + "\t" + Convert.ToString(IdAcco)).Split('\t');
                        if (data.Length > 0)
                        {
                            //Creamos la cabecera de los archivos.
                            if (i == 0)
                            {
                                foreach (var item in data)
                                {
                                    dt.Columns.Add(new DataColumn());

                                }
                                dt.Columns.Add(new DataColumn());
                                i++;
                            }

                            //llenamomos la data tabla con la información de cada *.txt
                            DataRow row = dt.NewRow();
                            row.ItemArray = data;
                            dt.Rows.Add(row);
                            i = 0;
                        }
                    }

                    try
                    {
                        //Copiamos el resultado a la tabla.
                        using (SqlBulkCopy copyx = new SqlBulkCopy(cnx))
                        {
                            copyx.ColumnMappings.Add("Column1", "ComputerName");
                            copyx.ColumnMappings.Add("Column2", "UserName");
                            copyx.ColumnMappings.Add("Column3", "LoginDate");
                            copyx.ColumnMappings.Add("Column4", "Caption");
                            copyx.ColumnMappings.Add("Column5", "Description");
                            copyx.ColumnMappings.Add("Column6", "IdentifyingNumber");
                            copyx.ColumnMappings.Add("Column7", "InstallDate");
                            copyx.ColumnMappings.Add("Column8", "InstallLocation");
                            copyx.ColumnMappings.Add("Column9", "InstallState");
                            copyx.ColumnMappings.Add("Column10", "Name");
                            copyx.ColumnMappings.Add("Column11", "PackageCache");
                            copyx.ColumnMappings.Add("Column12", "SKUNumber");
                            copyx.ColumnMappings.Add("Column13", "Vendor");
                            copyx.ColumnMappings.Add("Column14", "Version");
                            copyx.ColumnMappings.Add("Column15", "IdAccount");
                            copyx.DestinationTableName = "TempSW";
                            copyx.WriteToServer(dt);
                        }

                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            catch (Exception e)
            {

            }
            cnx.Close();
        }

        /*
         * Este evento se dispara cuando el framework ASP.NET obtiene el estado actual (estado de la sesión) relacionada con la solicitud actual.
         * 
         * */
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            //CultureInfo ci = new CultureInfo("es-PE");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            //System.Threading.Thread.CurrentThread.CurrentCulture =
            //CultureInfo.CreateSpecificCulture(ci.Name);
            if (HttpContext.Current.Session != null)
            {
                CultureInfo ci = (CultureInfo)this.Session["Culture"];

                //Checking first if there is no value in session 
                //and set default language 
                //this can happen for first user's request
                if (ci == null)
                {
                    //Sets default culture to english invariant
                    string langName = "en-US";
                    //string langName = "es-PE";

                    //Try to get values from Accept lang HTTP header
                    if (HttpContext.Current.Request.UserLanguages != null &&
                       HttpContext.Current.Request.UserLanguages.Length != 0)
                    {
                        //Gets accepted list 
                        langName = "en-US";//HttpContext.Current.Request.UserLanguages[0].Substring(0, 2);
                        //langName = "en-PE";
                    }
                    ci = new CultureInfo(langName);
                    this.Session["Culture"] = ci;
                    //this.Session["CulturexAll"] = this.Session["Culture"];

                }
                //Session["CulturexAll"] = ci;
                //Finally setting culture for each request
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }
        }

        //Expiration Contract
        private void ExpirationContract(object state)
        {
            SendMail mail = new SendMail();
            if (CountExpirationContract(30) > 0)
            {
                mail.ExpirationContract(30);
            }
            if (CountExpirationContract(15) > 0)
            {
                mail.ExpirationContract(15);
            }
            if (CountExpirationContract(5) > 0)
            {
                mail.ExpirationContract(5);
            }
        }

        //Configuracion de Header
        //private void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    //Evita que el aplicativo sea utilizado en un iframe
        //    Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        //    //Previene ataques tipo MIME
        //    Response.Headers.Add("X-Content-Type-Options", "nosniff");
        //    //ayuda a prevenir ataques de scripting
        //    Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        //    // Obliga a que todas las solicitudes provengan de apps HTTPS
        //    Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        //    // Este encabezado ayuda a prevenir ataques de tipo Cross-Site Scripting (XSS)
        //    //Response.Headers.Add("Content-Security-Policy", "script-src 'self'");
        //}
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            AntiForgeryConfig.RequireSsl = HttpContext.Current.Request.IsSecureConnection;
            //AntiForgeryConfig.RequireSsl = false;
        }
        //Expiration Contract
        public int CountExpirationContract(int id = 0)
        {
            DateTime END_DATE = DateTime.Today;
            END_DATE = END_DATE.AddDays(id);

            var ctd = dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.END_DATE == END_DATE).Count();

            return ctd;
        }

        ////Expiration Contract
        //public int SendMailExpirationContract(int id = 0)
        //{
        //    try
        //    {

        //        string mailTo1 = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == 1010).EMA_ELEC;

        //        if (String.IsNullOrEmpty(mailTo1))
        //        {
        //            mailTo1 = "cristian.arevalo@electrodata.com.pe";
        //        }

        //        string yourEmail = "cristian.arevalo@electrodata.com.pe";

        //        CDO.Message message = new CDO.Message();
        //        CDO.IConfiguration configuration = message.Configuration;
        //        ADODB.Fields fields = configuration.Fields;

        //        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        //        field.Value = "smtp.zoho.com";

        //        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        //        field.Value = 465;

        //        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        //        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        //        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        //        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        //        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        //        field.Value = "cristian.arevalo@electrodata.com.pe";//"esalazar@electrodata.com.pe";

        //        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        //        field.Value = "41299248";

        //        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        //        field.Value = "true";

        //        fields.Update();

        //        message.From = yourEmail;
        //        message.To = mailTo1;
        //        //message.CC = mail_coor;
        //        message.Subject = "Expiration Contract / Expiración de Contracto";
        //        Random rnd = new Random();
        //        decimal xx = rnd.Next(1, 1000);

        //        string strHostName = ConfigurationManager.AppSettings["IpServer"].ToString();

        //        message.CreateMHTMLBody(strHostName + "Mail/SendMailExpirationContract?id=" + id + "&_=" + Convert.ToString(xx), CDO.CdoMHTMLFlags.cdoSuppressNone, "Administrator", "123456");
        //        message.Send();

        //        return 1;
        //    }
        //    catch (Exception e)
        //    {
        //        //try
        //        //{
        //        //    var acp = new ACCOUNT_PARAMETER();
        //        //    acp.VAL_ACCO_PARA = e.Message;
        //        //    acp.ID_ACCO = null;
        //        //    acp.ID_PARA = null;

        //        //    db.ACCOUNT_PARAMETER.Add(acp);
        //        //    db.SaveChanges();
        //        //    return 0;
        //        //}
        //        //catch
        //        //{
        //        return 0;
        //        //}
        //    }
        //}
    }
}