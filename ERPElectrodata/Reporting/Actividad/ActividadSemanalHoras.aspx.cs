using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting
{
    public partial class ActividadSemanalHoras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"].ToString());
                
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptActividadSemanalHoras.ServerReport.ReportServerCredentials = rvc;
                rptActividadSemanalHoras.ProcessingMode = ProcessingMode.Remote;
                rptActividadSemanalHoras.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptActividadSemanalHoras.ServerReport.ReportPath = "/ReporteActividadesSemanales";

                rptActividadSemanalHoras.ShowPrintButton = true;
                rptActividadSemanalHoras.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("ID_ACCO", ID_ACCO);
#if DEBUG
                ReportViewerCredentials rvc2 = new ReportViewerCredentials("administrador", "C4goldP3", "");

                rptActividadSemanalHoras.ServerReport.ReportServerCredentials = rvc2;
#endif

                rptActividadSemanalHoras.ServerReport.SetParameters(param);
                rptActividadSemanalHoras.ServerReport.Refresh();
            }
        }
    }
}