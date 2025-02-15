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
    public partial class WF_ReportAssetsByUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_PERS_ENTI = null;
                string ID_ACCO = null;
                try
                {
                    ID_PERS_ENTI = Convert.ToString(Request.Params["id"]);
                    ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                }
                catch
                {
                    ID_PERS_ENTI = "0";
                    ID_ACCO = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rv_ReportAssetsByUser.ServerReport.ReportServerCredentials = rvc;
                rv_ReportAssetsByUser.ProcessingMode = ProcessingMode.Remote;
                rv_ReportAssetsByUser.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportAssetsByUser.ServerReport.ReportPath = "/ReportAssetsByUser";

                rv_ReportAssetsByUser.ShowPrintButton = true;
                rv_ReportAssetsByUser.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("ID_PERS_ENTI", ID_PERS_ENTI);
                param[1] = new ReportParameter("ID_ACCO", ID_ACCO);

                rv_ReportAssetsByUser.ServerReport.SetParameters(param);

                rv_ReportAssetsByUser.ServerReport.Refresh();
            }
        }
    }
}