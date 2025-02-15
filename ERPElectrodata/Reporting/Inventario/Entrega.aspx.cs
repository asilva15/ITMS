using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Inventario
{
    public partial class Entrega : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_TICK = Convert.ToString(Request.Params["id"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptEntregaInterna.ServerReport.ReportServerCredentials = rvc;
                rptEntregaInterna.ProcessingMode = ProcessingMode.Remote;
                rptEntregaInterna.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptEntregaInterna.ServerReport.ReportPath = "/Inventario/ReporteEntregaInterna";

                rptEntregaInterna.ShowPrintButton = true;
                rptEntregaInterna.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("ID_TICK", ID_TICK);

                rptEntregaInterna.ServerReport.SetParameters(param);
                rptEntregaInterna.ServerReport.Refresh();
            }
        }
    }
}