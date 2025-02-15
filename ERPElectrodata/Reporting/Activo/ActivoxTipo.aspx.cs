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
    public partial class ActivoxTipo : System.Web.UI.Page
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
                RVActivoxTipo.ServerReport.ReportServerCredentials = rvc;
                RVActivoxTipo.ProcessingMode = ProcessingMode.Remote;
                RVActivoxTipo.ServerReport.ReportServerUrl = new Uri(reportServer);
                RVActivoxTipo.ServerReport.ReportPath = "/Activo/ActivoxTipo2";

                RVActivoxTipo.ShowPrintButton = true;
                RVActivoxTipo.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[1] = new ReportParameter("ID_TYPE_ASSE", ID_TYPE_ASSE);

                RVActivoxTipo.ServerReport.SetParameters(param);
                RVActivoxTipo.ServerReport.Refresh();
            }
        }
    }
}