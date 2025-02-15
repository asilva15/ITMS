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
    public partial class TicketTotales : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdCuenta = Convert.ToString(Session["ID_ACCO"]);
                
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptTicketTotales.ServerReport.ReportServerCredentials = rvc;
                rptTicketTotales.ProcessingMode = ProcessingMode.Remote;
                rptTicketTotales.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptTicketTotales.ServerReport.ReportPath = "/Ticket/TicketTotales";

                rptTicketTotales.ShowPrintButton = true;
                rptTicketTotales.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("ID_ACCO", IdCuenta);
                param[1] = new ReportParameter("ID_COMP", "0");

                rptTicketTotales.ServerReport.SetParameters(param);
                rptTicketTotales.ServerReport.Refresh();
            }
        }
    }
}