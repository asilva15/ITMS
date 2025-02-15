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
    public partial class WF_ReportHistoryByAsset : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_ASSE = null;
                string ID_ACCO = null;
                try
                {
                    ID_ASSE = Convert.ToString(Request.Params["id"]);
                    ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                }
                catch
                {
                    ID_ASSE = "0";
                    ID_ACCO = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rv_ReportHisotyrByAsset.ServerReport.ReportServerCredentials = rvc;
                rv_ReportHisotyrByAsset.ProcessingMode = ProcessingMode.Remote;
                rv_ReportHisotyrByAsset.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportHisotyrByAsset.ServerReport.ReportPath = "/ReportHistoryAsset";

                rv_ReportHisotyrByAsset.ShowPrintButton = true;
                rv_ReportHisotyrByAsset.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("ID_ASSE", ID_ASSE);
                param[1] = new ReportParameter("ID_ACCO", ID_ACCO);

                rv_ReportHisotyrByAsset.ServerReport.SetParameters(param);

                rv_ReportHisotyrByAsset.ServerReport.Refresh();
            }
        }
    }
}