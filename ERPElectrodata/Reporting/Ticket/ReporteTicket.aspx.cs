using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Ticket
{
    public partial class ReporteTicket : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdCuenta = Convert.ToString(Session["ID_ACCO"]);
                string FechaInicio = Convert.ToString(Request.Params["FechaInicial"]);
                string FechaFin = Convert.ToString(Request.Params["FechaFinal"]);

                if (String.IsNullOrEmpty(IdCuenta))
                {
                    IdCuenta = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptReporteTicket.ServerReport.ReportServerCredentials = rvc;
                rptReporteTicket.ProcessingMode = ProcessingMode.Remote;
                rptReporteTicket.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptReporteTicket.ServerReport.ReportPath = "/Ticket/ReporteTicket";

                rptReporteTicket.ShowPrintButton = true;
                rptReporteTicket.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("IdCuenta", IdCuenta);
                param[1] = new ReportParameter("FechaInicio", FechaInicio);
                param[2] = new ReportParameter("FechaFin", FechaFin);

//#if DEBUG
//                ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");

//                rptReporteTicket.ServerReport.ReportServerCredentials = rvc;
//#endif

                rptReporteTicket.ServerReport.SetParameters(param);
                rptReporteTicket.ServerReport.Refresh();
            }
        }
    }
}