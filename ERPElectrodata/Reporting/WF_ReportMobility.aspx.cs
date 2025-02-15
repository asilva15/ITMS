using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting
{
    public partial class WF_ReportMobility : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_DELI_SUST = null;
                try
                {
                    ID_DELI_SUST = Convert.ToString(Request.Params["id"]);
                }
                catch
                {
                    ID_DELI_SUST = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rv_ReportMobility.ServerReport.ReportServerCredentials = rvc;
                rv_ReportMobility.ProcessingMode = ProcessingMode.Remote;
                rv_ReportMobility.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportMobility.ServerReport.ReportPath = "/ReportMobilityPettyCash";

                rv_ReportMobility.ShowPrintButton = true;
                rv_ReportMobility.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("ID_DELI_SUST", ID_DELI_SUST);

                rv_ReportMobility.ServerReport.SetParameters(param);

                rv_ReportMobility.ServerReport.Refresh();
            }
        }
    }
}