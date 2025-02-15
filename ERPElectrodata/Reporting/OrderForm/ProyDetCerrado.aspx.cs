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
    public partial class ProyDetCerrado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string IdPM = Request.Params["PM"].ToString();

                if (String.IsNullOrEmpty(IdPM)) IdPM = "0";

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptProyDetCerrado.ServerReport.ReportServerCredentials = rvc;
                rptProyDetCerrado.ProcessingMode = ProcessingMode.Remote;
                rptProyDetCerrado.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptProyDetCerrado.ServerReport.ReportPath = "/Proyectos/ProyectoDetalleCerrado";

                rptProyDetCerrado.ShowPrintButton = true;
                rptProyDetCerrado.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("IdPM", IdPM);

                rptProyDetCerrado.ServerReport.SetParameters(param);
                rptProyDetCerrado.ServerReport.Refresh();
            }

        }
    }
}