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
    public partial class Assistance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_PERS_ENTI = Convert.ToString(Request.Params["id"]);
                string ID_YEAR = Convert.ToString(Request.Params["id1"]);
                string ID_MONT = Convert.ToString(Request.Params["id2"]);
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"]);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptAssistance.ServerReport.ReportServerCredentials = rvc;
                rptAssistance.ProcessingMode = ProcessingMode.Remote;
                rptAssistance.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptAssistance.ServerReport.ReportPath = "/TalentoHumano/INF_LIST_ASSI";

                rptAssistance.ShowPrintButton = true;
                rptAssistance.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[4];
                param[0] = new ReportParameter("ID_PERS_ENTI", ID_PERS_ENTI);
                param[1] = new ReportParameter("ID_YEAR", ID_YEAR);
                param[2] = new ReportParameter("ID_MONT", ID_MONT);
                param[3] = new ReportParameter("ID_ACCO", ID_ACCO);

                rptAssistance.ServerReport.SetParameters(param);
                rptAssistance.ServerReport.Refresh();
            }
        }
    }
}