using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleNotification.Models;
using ConsoleNotification.Plugins;
using ConsoleNotification.Templates;
using System.Net.Mail;
using System.Net.Mime;
using System.Data.Entity;
using System.Threading;
using System.Globalization;
using System.Threading;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.IO;
using System.Data.Entity.Core.Objects;

namespace ConsoleNotification.Controllers
{
    //Envío de encuestas de satisfacción, enviado al usuario asignado del ticket.
    class EncuestaConfiguracion
    {
        //Declaracion de Objetos para acceso a base de datos. 
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public AppLogEntities dbl = new AppLogEntities();

        public string GenerarEncuesta()
        {
            var queryEncuestas = dbe.EncuestaConfiguracions.Where(x => x.EnvioAutomatico == true && (DbFunctions.TruncateTime(x.FechaUltimaEnvio) != DbFunctions.TruncateTime(DateTime.Today) || x.EncuestasEnviadas < x.NroEncuestas)).ToList();
            string retorna = "";
            if (queryEncuestas.Count() != 0)
            {
                foreach (var ec in queryEncuestas)
                {
                    int id = Convert.ToInt32(ec.IdEncuestaConfiguracion);
                    var encuestas = dbe.EncuestaConfiguracions.Where(x => x.IdEncuestaConfiguracion == id && x.Estado == true).SingleOrDefault();
                    //Validar si hay encuestas pendientes por enviar, si enviar encuestas de la cuenta siguiente 
                    var queryEncPendientes = dbc.TICKETs.Where(x => x.ID_ACCO == encuestas.IdAcco && x.SEND_SURVEY == false && x.ID_STAT_END == 6 && x.ID_TYPE_FORM == null).ToList();
                    if (queryEncPendientes.Count() != 0)
                    {
                        var encPendientes = dbc.TICKETs.Where(x => x.ID_ACCO == encuestas.IdAcco && x.SEND_SURVEY == false && x.ID_STAT_END == 6 && x.ID_TYPE_FORM == null).Take(1).SingleOrDefault();
                        var queryId = dbe.EncuestaConfiguracions.Where(x => x.IdAcco == encPendientes.ID_ACCO.Value && x.Estado == true).SingleOrDefault();
                        Tickets tic = new Tickets();
                        retorna = tic.GeneraEncuesta(Convert.ToInt32(queryId.IdEncuestaConfiguracion)); 
                        retorna = "4";
                    }
                    else
                    {
                        Tickets tic = new Tickets();
                        //Validar la fecha última de envío 
                        var ultimoEnvio = dbe.ValidarFechaUltimaDeEnvio(id).SingleOrDefault();

                        var query = dbe.EncuestaConfiguracionEnvioManual(id);
                        retorna = tic.GeneraEncuesta(id);
                    } 
                    
                }
                return retorna;
            }
            else
            {
                return "3";
            }
        }

    }
}
