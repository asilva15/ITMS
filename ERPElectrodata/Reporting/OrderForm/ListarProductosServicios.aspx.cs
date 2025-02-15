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
    public partial class ListarProductosServicios : System.Web.UI.Page
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
                rptListarProyectos.ServerReport.ReportServerCredentials = rvc;
                rptListarProyectos.ProcessingMode = ProcessingMode.Remote;
                rptListarProyectos.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptListarProyectos.ServerReport.ReportPath = "/Proyectos/ReporteProductosServicios";

                rptListarProyectos.ShowPrintButton = true;
                rptListarProyectos.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("IdDocuSale", IdDocuSale);

                rptListarProyectos.ServerReport.SetParameters(param);
                rptListarProyectos.ServerReport.Refresh();
            }
        }
    }
}