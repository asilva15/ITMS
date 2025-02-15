using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Talent
{
    public partial class ReporteContratacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                EntityEntities dbe = new EntityEntities();

                
                string Id_Solicitud = Convert.ToString((Request.Params["IdSolicitud"]));

               

                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptContratacion.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rptContratacion.ProcessingMode = ProcessingMode.Remote;
                rptContratacion.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.
                rptContratacion.ServerReport.ReportPath = "/TalentoHumano/ReporteContratacion";

                rptContratacion.ShowPrintButton = true;
                rptContratacion.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("Id_Solicitud", Id_Solicitud);
                //param[1] = new ReportParameter("FROM_DATE", FROM_DATE);
                //param[2] = new ReportParameter("TO_DATE", TO_DATE);

                rptContratacion.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rptContratacion.ServerReport.Refresh();
            }
        }
    }
}