using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;


namespace ERPElectrodata.Reporting.Actividad
{
    public partial class ConsolidadoActividades : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idCuenta = Convert.ToString(Request.Params["idCuenta"]);
                string idUsuario = Convert.ToString(Request.Params["idUsuario"]);
                string idArea = Convert.ToString(Request.Params["idArea"]);
                string fechaIni = Convert.ToString(Request.Params["fechaIni"]);
                string fechaFin = Convert.ToString(Request.Params["fechaFin"]);

                if (idCuenta == "")
                    idCuenta = "0";
                if (idUsuario == "")
                    idUsuario = Convert.ToString(Session["ID_PERS_ENTI"].ToString());
                if (idArea == "")
                    idArea = "0";

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptConsolidado.ServerReport.ReportServerCredentials = rvc;
                rptConsolidado.ProcessingMode = ProcessingMode.Remote;
                rptConsolidado.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptConsolidado.ServerReport.ReportPath = "/Actividad/ReporteActividades";

                rptConsolidado.ShowPrintButton = true;
                rptConsolidado.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[5];
                param[0] = new ReportParameter("ID_PERS_ENTI", idUsuario);
                param[1] = new ReportParameter("ID_ACCO", idCuenta);
                param[2] = new ReportParameter("INIC", fechaIni);
                param[3] = new ReportParameter("FIN", fechaFin);
                param[4] = new ReportParameter("AREAS", idArea);

                //#if DEBUG
                //ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");

                //rptConsolidado.ServerReport.ReportServerCredentials = rvc;
                //#endif

                rptConsolidado.ServerReport.SetParameters(param);
                rptConsolidado.ServerReport.Refresh();
            }
        }
    }
}