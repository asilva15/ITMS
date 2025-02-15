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
    public partial class WF_ReportBeneficiaries : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rv_ReportBeneficiaries.ServerReport.ReportServerCredentials = rvc;
                rv_ReportBeneficiaries.ProcessingMode = ProcessingMode.Remote;
                rv_ReportBeneficiaries.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportBeneficiaries.ServerReport.ReportPath = "/ReportBeneficiary";

                rv_ReportBeneficiaries.ShowPrintButton = true;
                rv_ReportBeneficiaries.ShowParameterPrompts = false;

                //ReportParameter[] param = new ReportParameter[1];
                //param[0] = new ReportParameter("Year", ANIO);

                //rv_ReportMemorandum.ServerReport.SetParameters(param);

                rv_ReportBeneficiaries.ServerReport.Refresh();
            }
        }
    }
}