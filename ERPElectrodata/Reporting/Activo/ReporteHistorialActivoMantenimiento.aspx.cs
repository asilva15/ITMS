using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Linq;
using System.Net;

namespace ERPElectrodata.Reporting
{
    public partial class ReporteHistorialActivoMantenimiento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                CoreEntities dbc = new CoreEntities();
                string IdActivo = "0";
                string IdAcco = Convert.ToString(Session["ID_ACCO"]);
                try
                {
                    //Parámetros de entrada - Id del activo 
                    IdActivo = Convert.ToString(Request.Params["IdActivo"]);
                }
                catch
                {
                }

                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptHistorialActivoMantenimiento.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rptHistorialActivoMantenimiento.ProcessingMode = ProcessingMode.Remote;
                rptHistorialActivoMantenimiento.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.
                rptHistorialActivoMantenimiento.ServerReport.ReportPath = "/Activo/ReporteHistorialActivoMantenimiento";

                rptHistorialActivoMantenimiento.ShowPrintButton = true;
                rptHistorialActivoMantenimiento.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("IdActivo", IdActivo);
                param[1] = new ReportParameter("IdAcco", IdAcco);

//#if DEBUG
//                ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");

//                rptHistorialActivoMantenimiento.ServerReport.ReportServerCredentials = rvc;
//#endif

                rptHistorialActivoMantenimiento.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rptHistorialActivoMantenimiento.ServerReport.Refresh();
            }
        }
    }


}