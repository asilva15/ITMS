using System;
using System.Linq;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting
{
    public partial class ReporteTicketTipo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                CoreEntities dbc = new CoreEntities();

                int IdJefatura = 0;
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                var JEFATURA = dbc.Jefatura.Where(x => x.ID_PERS_ENTI_RESP == ID_PERS_ENTI).FirstOrDefault();
                
                 
                if (ID_PERS_ENTI == 1007)
                {
                    IdJefatura = 0;
                }
                else if(JEFATURA != null)
                {
                    int IdJefaturaBVN = Convert.ToInt32(JEFATURA.IdJefatura);
                    IdJefatura = IdJefaturaBVN;
                }
                else
                {
                    IdJefatura = 0;
                }

                //Establecer como parámetro la cuenta actual.
                int ID_ACCOi = Convert.ToInt32(Session["ID_ACCO"]);
                int IdArea = 0;
                string ID_ACCO = null;
                string FechaInicio = Convert.ToString(Request.Params["FechaInicial"]);
                string FechaFin = Convert.ToString(Request.Params["FechaFinal"]);
                

                ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                
              
                try
                {
                    IdArea = Convert.ToInt32(Request.Params["IdAreaOperativaRT"]);
                }
                catch
                {
                    IdArea = 0;
                }

                
                int IdTipoTicket = 0;

                try
                {
                    IdTipoTicket = Convert.ToInt32(Request.Params["ID_TYPE_TICK"]);
                }
                catch
                {
                    IdTipoTicket = 0;
                }
                string IdTipo = Convert.ToString(IdTipoTicket);
                string IdJef = Convert.ToString(IdJefatura);
                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rvTicketTipo.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rvTicketTipo.ProcessingMode = ProcessingMode.Remote;
                rvTicketTipo.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.
                rvTicketTipo.ServerReport.ReportPath = "/Ticket/ReporteTicketPorTipo";

                rvTicketTipo.ShowPrintButton = true;
                rvTicketTipo.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[6];
                param[0] = new ReportParameter("IdAcco", ID_ACCO);
                param[1] = new ReportParameter("FechaInicio", FechaInicio);
                param[2] = new ReportParameter("FechaFin", FechaFin);
                param[3] = new ReportParameter("IdArea", IdArea.ToString());
                param[4] = new ReportParameter("IdTypeTick", IdTipo);
                param[5] = new ReportParameter("IdJefatura", IdJef);


                rvTicketTipo.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rvTicketTipo.ServerReport.Refresh();
            }
        }
    }
}