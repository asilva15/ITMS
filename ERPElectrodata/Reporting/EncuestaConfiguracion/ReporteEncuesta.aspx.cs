using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Linq;
using System.Net;

namespace ERPElectrodata.Reporting.EncuestaConfiguracion
{
    public partial class ReporteEncuesta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                EntityEntities dbc = new EntityEntities();

                //Establecer como parámetro la cuenta actual.
                int ID_ACCOi = Convert.ToInt32(Session["ID_ACCO"]);
                string ID_ACCO = null, FechaInicio = null, FechaFin = null;
                ID_ACCO = Convert.ToString(Session["ID_ACCO"]);

                try
                {
                    if (ID_ACCOi == 4) {
                        ID_ACCO = Convert.ToString(Request.Params["IdAcco"]);
                    }
                    FechaInicio = Convert.ToString(Request.Params["FechaInicio"]);
                    FechaFin = Convert.ToString(Request.Params["FechaFin"]);
                }
                catch
                {
                    //
                }
                

                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptReporteEncuesta.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rptReporteEncuesta.ProcessingMode = ProcessingMode.Remote;
                rptReporteEncuesta.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.
                rptReporteEncuesta.ServerReport.ReportPath = "/ReporteEncuestas";

                rptReporteEncuesta.ShowPrintButton = true;
                rptReporteEncuesta.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("IdAcco", ID_ACCO);
                param[1] = new ReportParameter("FechaInicio", FechaInicio);
                param[2] = new ReportParameter("FechaFin", FechaFin);

//#if DEBUG
//                ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");

//                rptReporteEncuesta.ServerReport.ReportServerCredentials = rvc;
//#endif

                rptReporteEncuesta.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rptReporteEncuesta.ServerReport.Refresh();
            }
        }
    }


}