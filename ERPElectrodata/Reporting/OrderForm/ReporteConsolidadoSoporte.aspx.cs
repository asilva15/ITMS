using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.OrderForm
{
    public partial class ReporteConsolidadoSoporte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string idOP = Convert.ToString(Request.Params["idOP"].ToString());

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptConsolidadoSop.ServerReport.ReportServerCredentials = rvc;
                rptConsolidadoSop.ProcessingMode = ProcessingMode.Remote;
                rptConsolidadoSop.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptConsolidadoSop.ServerReport.ReportPath = "/Proyectos/ReporteConsolidadoSoporte";

                rptConsolidadoSop.ShowPrintButton = true;
                rptConsolidadoSop.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("IdOP", idOP);

                rptConsolidadoSop.ServerReport.SetParameters(param);
                rptConsolidadoSop.ServerReport.Refresh();
            }

        }
    }
}