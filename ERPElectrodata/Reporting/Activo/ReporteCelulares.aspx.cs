using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Activo
{
    public partial class ReporteCelulares : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                RVActivoxUsuario.ServerReport.ReportServerCredentials = rvc;
                RVActivoxUsuario.ProcessingMode = ProcessingMode.Remote;
                RVActivoxUsuario.ServerReport.ReportServerUrl = new Uri(reportServer);
                RVActivoxUsuario.ServerReport.ReportPath = "/Activo/RptCelulares";

                RVActivoxUsuario.ShowPrintButton = true;
                RVActivoxUsuario.ShowParameterPrompts = false;

                RVActivoxUsuario.ServerReport.Refresh();
            }

        }
    }
}