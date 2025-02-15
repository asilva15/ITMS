using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Seguridad
{
    public partial class EventoAuditable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptEventos.ServerReport.ReportServerCredentials = rvc;
                rptEventos.ProcessingMode = ProcessingMode.Remote;
                rptEventos.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptEventos.ServerReport.ReportPath = "/Seguridad/EventoAudita";

                rptEventos.ShowPrintButton = true;
                rptEventos.ShowParameterPrompts = false;

                //ReportParameter[] param = new ReportParameter[2];
                //param[0] = new ReportParameter("IdUsuario", IdUsuario);
                //param[1] = new ReportParameter("NombreUsuario", NombreUsuario);

                //rptEventos.ServerReport.SetParameters(param);
                rptEventos.ServerReport.Refresh();
            }
        }
    }
}