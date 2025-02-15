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
    public partial class WF_ReportAccountability : System.Web.UI.Page
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
                rv_ReportAccountability.ServerReport.ReportServerCredentials = rvc;
                rv_ReportAccountability.ProcessingMode = ProcessingMode.Remote;
                rv_ReportAccountability.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportAccountability.ServerReport.ReportPath = "/ReportAccountability";

                rv_ReportAccountability.ShowPrintButton = true;
                rv_ReportAccountability.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("ID_DELI_SUST", ID_DELI_SUST);
                param[1] = new ReportParameter("IDIOMA", ResourceLanguaje.Resource.Idioma);

                rv_ReportAccountability.ServerReport.SetParameters(param);

                rv_ReportAccountability.ServerReport.Refresh();
            }
        }

    }
}