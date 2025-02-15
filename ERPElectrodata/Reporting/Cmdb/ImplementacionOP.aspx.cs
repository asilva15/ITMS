using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Cmdb
{
    public partial class ImplementacionOP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptImplementacionOP.ServerReport.ReportServerCredentials = rvc;
                rptImplementacionOP.ProcessingMode = ProcessingMode.Remote;
                rptImplementacionOP.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptImplementacionOP.ServerReport.ReportPath = "/Cmdb/ImplementacionOP";

                rptImplementacionOP.ShowPrintButton = true;
                rptImplementacionOP.ShowParameterPrompts = false;

                rptImplementacionOP.ServerReport.Refresh();
            }
        }
    }
}