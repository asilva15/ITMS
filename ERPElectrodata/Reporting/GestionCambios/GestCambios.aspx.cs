using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;

namespace ERPElectrodata.Reporting.GestionCambios
{
    public partial class GestCambios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idChangeType = Convert.ToString(Request.Params["idChangeType"]);
                string idApproved = Convert.ToString(Request.Params["idApproved"]);
                string FechaInicio = Convert.ToString(Request.Params["FechaInicio"]);
                string FechaFin = Convert.ToString(Request.Params["FechaFin"]);

                if (String.IsNullOrEmpty(idChangeType)) { idChangeType = "0"; }
                if (String.IsNullOrEmpty(idApproved)) { idApproved = "0"; }
                if (String.IsNullOrEmpty(FechaInicio)) { FechaInicio = "1900/01/01"; }
                if (String.IsNullOrEmpty(FechaFin)) { FechaFin = String.Format("{0:yyyy/M/d}", DateTime.Now); }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptGestCambios.ServerReport.ReportServerCredentials = rvc;
                rptGestCambios.ProcessingMode = ProcessingMode.Remote;

                rptGestCambios.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptGestCambios.ServerReport.ReportPath = "/GestionCambios/ReporteConsolidado";
                rptGestCambios.ShowPrintButton = true;
                rptGestCambios.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[4];
                param[0] = new ReportParameter("idChangeType", idChangeType);
                param[1] = new ReportParameter("idApproved", idApproved);
                param[2] = new ReportParameter("FechaInicio", FechaInicio);
                param[3] = new ReportParameter("FechaFin", FechaFin);

                rptGestCambios.ServerReport.SetParameters(param);
                rptGestCambios.ServerReport.Refresh();
            }
        }                 
    }                     
}
                          
                          