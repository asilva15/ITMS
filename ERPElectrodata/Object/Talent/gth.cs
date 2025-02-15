using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Models;
using ERPElectrodata.Objects;
using System.Threading;
using System.Globalization;
using ERPElectrodata.Object.Plugins;

using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;


namespace ERPElectrodata.Object.Talent
{
    public class gth
    {
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        DateTimeFormatInfo dtinfo_en = new CultureInfo("en-US", false).DateTimeFormat;
        DateTimeFormatInfo dtinfo_es = new CultureInfo("es-PE", false).DateTimeFormat;

      
        public string PostSignature(POST_SIGNATURE_Result pers)
        {
            try
            {
                textInfo = cultureInfo.TextInfo;

                DateTime fecha = DateTime.Now.AddDays(-2);
                PluginSmtp mail = new PluginSmtp();
                BodyGTH _body = new BodyGTH();
                string body_html = _body.Signature(pers);

                //mail.message.From = "itms@electrodata.com.pe";
                //mail.message.To = pers.EMA_ELEC; //"esalazar@electrodata.com.pe;";//
                ////mail.message.BCC = "esalazar@electrodata.com.pe;";
                //mail.message.HTMLBody = _body.Signature(pers);
                //mail.message.Subject = "Post Signature (Reminder)"/* + textInfo.ToTitleCase(dtinfo_en.GetMonthName(fecha.Month))*/ + " / Post Firma (Recordatorio)"; //+ textInfo.ToTitleCase(dtinfo_es.GetMonthName(fecha.Month));
                //mail.message.Send();

                var message = mail.mailMessage;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_html, Encoding.UTF8, MediaTypeNames.Text.Html);
                //message.To.Add("lsempertegui@electrodata.com.pe");
                message.To.Add(pers.EMA_ELEC); //Descomentar     
                message.AlternateViews.Add(htmlView);
                message.Subject = "Post Signature (Reminder)"/* + textInfo.ToTitleCase(dtinfo_en.GetMonthName(fecha.Month))*/ + " / Post Firma (Recordatorio)"; //+ textInfo.ToTitleCase(dtinfo_es.GetMonthName(fecha.Month));
                mail.smtp.Send(message);

                return "OK";
            }
            catch(Exception ex)
            {
                return "Error";
            }
            
        }

        public string GenerarContrasenia()
        {
            Random rdn = new Random();
            //string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
            int longitud = caracteres.Length;
            char letra;
            int longitudContrasenia = 12;
            string contraseniaAleatoria = string.Empty;

            contraseniaAleatoria += caracteres[rdn.Next(52, 62)];
            contraseniaAleatoria += caracteres[rdn.Next(52, 72)];
            contraseniaAleatoria += caracteres[rdn.Next(26, 52)];

            for (int i = 3; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }

            contraseniaAleatoria = new string(contraseniaAleatoria.ToCharArray().OrderBy(x => rdn.Next()).ToArray());

            return contraseniaAleatoria;
        }
    }
}