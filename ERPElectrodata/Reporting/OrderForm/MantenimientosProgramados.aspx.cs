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
    public partial class MantenimientosProgramados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string cbClienteFinal = Convert.ToString(Request.Params["cbClienteFinal"].ToString());
                string dtAnio = Convert.ToString(Request.Params["dtAnio"].ToString().Trim());
                
                if (cbClienteFinal == "null") cbClienteFinal = "0";
                if (dtAnio == "") dtAnio = "";

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptMantProgramados.ServerReport.ReportServerCredentials = rvc;
                rptMantProgramados.ProcessingMode = ProcessingMode.Remote;
                rptMantProgramados.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptMantProgramados.ServerReport.ReportPath = "/Proyectos/RptReportesMantenimientosProgramados";

                rptMantProgramados.ShowPrintButton = true;
                rptMantProgramados.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("IdClienteFinal", cbClienteFinal);
                param[1] = new ReportParameter("AnioMantenimiento", dtAnio);



//#if DEBUG
//                ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");

//                rptMantProgramados.ServerReport.ReportServerCredentials = rvc;
//#endif

                rptMantProgramados.ServerReport.SetParameters(param);
                rptMantProgramados.ServerReport.Refresh();
            }

        }
    }
}