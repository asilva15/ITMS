using System;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Vacaciones
{
    public partial class Formato : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                String Id = Convert.ToString(Request.Params["id"]);
                
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptFormato.ServerReport.ReportServerCredentials = rvc;
                rptFormato.ProcessingMode = ProcessingMode.Remote;
                rptFormato.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptFormato.ServerReport.ReportPath = "/TalentoHumano/FormatoVacaciones";

                rptFormato.ShowPrintButton = true;
                rptFormato.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("Id", Id);

                rptFormato.ServerReport.SetParameters(param);
                rptFormato.ServerReport.Refresh();
            }
        }
    }
}