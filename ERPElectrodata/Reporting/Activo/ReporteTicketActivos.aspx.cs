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
    public partial class ReporteTicketActivos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_ACCO = null;
                string ID_STAT_END = null;
                try
                {
                    ID_ACCO = Convert.ToString(Session["ID_ACCO"]);

                    ID_STAT_END = Convert.ToString(Request.Params["id"]);
                }
                catch
                {
                   
                    ID_ACCO = "0";
                    ID_STAT_END = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                RVReporteTicketActivos.ServerReport.ReportServerCredentials = rvc;
                RVReporteTicketActivos.ProcessingMode = ProcessingMode.Remote;
                RVReporteTicketActivos.ServerReport.ReportServerUrl = new Uri(reportServer);
                RVReporteTicketActivos.ServerReport.ReportPath = "/Activo/ReporteTicketActivos";

                RVReporteTicketActivos.ShowPrintButton = true;
                RVReporteTicketActivos.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("IdEstado", ID_STAT_END);
                param[1] = new ReportParameter("IdAcco", ID_ACCO);

                RVReporteTicketActivos.ServerReport.SetParameters(param);

                RVReporteTicketActivos.ServerReport.Refresh();
            }
        }
    }
}