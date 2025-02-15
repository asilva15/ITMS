using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Actividad
{
    public partial class ActSemanalxIngeniero : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdPersEnti = Convert.ToString(Session["ID_PERS_ENTI"]);
                string FechaInicio = Convert.ToString(Request.Params["FechaInicial"]);
                string Area = Convert.ToString(Request.Params["IdArea"]);
                if (Area == "null" || Area == null)
                    Area = "";
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptActSemxIng.ServerReport.ReportServerCredentials = rvc;
                rptActSemxIng.ProcessingMode = ProcessingMode.Remote;
                rptActSemxIng.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptActSemxIng.ServerReport.ReportPath = "/Actividad/ReporteActSemanalxIng_2";

                rptActSemxIng.ShowPrintButton = true;
                rptActSemxIng.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("IdPersEnti", IdPersEnti);
                param[1] = new ReportParameter("FechaInicio", FechaInicio);
                param[2] = new ReportParameter("Area", Area);

#if DEBUG
                ReportViewerCredentials rvc2 = new ReportViewerCredentials("administrador", "C4goldP3", "");

                rptActSemxIng.ServerReport.ReportServerCredentials = rvc2;
#endif
                rptActSemxIng.ServerReport.SetParameters(param);
                rptActSemxIng.ServerReport.Refresh();
            }
        }
    }
}