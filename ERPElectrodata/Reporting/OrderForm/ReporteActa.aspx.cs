using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.OrderForm
{
    public partial class ReporteActa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdDocuSale = Convert.ToString(Request.Params["IdDocuSale"]);

                if (String.IsNullOrEmpty(IdDocuSale)) { IdDocuSale = "0"; }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptActa.ServerReport.ReportServerCredentials = rvc;
                rptActa.ProcessingMode = ProcessingMode.Remote;
                rptActa.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptActa.ServerReport.ReportPath = "/Proyectos/ReporteActa";

                rptActa.ShowPrintButton = true;
                rptActa.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("IdDocuSale", IdDocuSale);

                rptActa.ServerReport.SetParameters(param);
                rptActa.ServerReport.Refresh();
            }
        }
    }
}