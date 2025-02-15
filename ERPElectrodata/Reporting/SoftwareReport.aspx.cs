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
    public partial class SoftwareReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_TYPE_ASSE = Convert.ToString(Request.Params["id"]);
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptSoftware.ServerReport.ReportServerCredentials = rvc;
                rptSoftware.ProcessingMode = ProcessingMode.Remote;
                rptSoftware.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptSoftware.ServerReport.ReportPath = "/SOF_REPO";

                rptSoftware.ShowPrintButton = true;
                rptSoftware.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("ID_ACCO", ID_ACCO);
                //param[1] = new ReportParameter("ID_TYPE_ASSE", ID_TYPE_ASSE);

                rptSoftware.ServerReport.SetParameters(param);
                rptSoftware.ServerReport.Refresh();
            }
        }
    }
}