using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.EvaluacionPersonal
{
    public partial class FormatoSeguimiento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string IdPeriodo = Convert.ToString(Request.Params["cbPeriodo"]);
                string IdEvaluado = Convert.ToString(Request.Params["cbEvaluado"]);

                if (Convert.ToInt32(IdPeriodo) < 8)
                {
                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptFormato.ServerReport.ReportServerCredentials = rvc;
                    rptFormato.ProcessingMode = ProcessingMode.Remote;
                    rptFormato.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptFormato.ServerReport.ReportPath = "/TalentoHumano/EvaluacionPersonal/FormatoSeguimiento";

                    rptFormato.ShowPrintButton = true;
                    rptFormato.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[2];
                    param[0] = new ReportParameter("IdPeriodo", IdPeriodo);
                    param[1] = new ReportParameter("IdPersEnti", IdEvaluado);

                    rptFormato.ServerReport.SetParameters(param);
                    rptFormato.ServerReport.Refresh();
                }
                else
                {
                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptFormato.ServerReport.ReportServerCredentials = rvc;
                    rptFormato.ProcessingMode = ProcessingMode.Remote;
                    rptFormato.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptFormato.ServerReport.ReportPath = "/TalentoHumano/EvaluacionPersonal/FormatoSeguimientoNuevo";

                    rptFormato.ShowPrintButton = true;
                    rptFormato.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[2];
                    param[0] = new ReportParameter("IdPeriodo", IdPeriodo);
                    param[1] = new ReportParameter("IdPersEnti", IdEvaluado);

                    rptFormato.ServerReport.SetParameters(param);
                    rptFormato.ServerReport.Refresh();
                }
                
                
            }

        }
    }
}