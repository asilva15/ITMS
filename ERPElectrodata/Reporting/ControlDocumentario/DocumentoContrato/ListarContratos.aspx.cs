using System;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.ControlDocumentario.DocumentoContrato
{
    public partial class ListarContratos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string Concepto = Convert.ToString(Request.Params["ConceptoContrato"]);
                string IdOP = Convert.ToString(Request.Params["NumeroOP"]);
                string Estado = Convert.ToString(Request.Params["EstadoFirmaContrato"]);              
                string IdCliente = Convert.ToString(Request.Params["IdCliente"]);
                string IdClienteFinal = Convert.ToString(Request.Params["IdClienteFinal"]);
                string FechaInicio = Convert.ToString(Request.Params["FechaIniContrato"]);
                string FechaFin = Convert.ToString(Request.Params["FechaFinContrato"]);
                
                if (String.IsNullOrEmpty(Concepto)) { Concepto = "0"; }
                if (String.IsNullOrEmpty(IdOP)) { IdOP = "0"; }
                if (String.IsNullOrEmpty(Estado)) { Estado = "0"; }
                if (String.IsNullOrEmpty(IdCliente)) { IdCliente = "0"; }
                if (String.IsNullOrEmpty(IdClienteFinal)) { IdClienteFinal = "0"; }

                DateTime FechaIniContrato = Convert.ToDateTime("1900/01/01");
                DateTime FechaFinContrato = Convert.ToDateTime("9999/12/31");

                if (!String.IsNullOrEmpty(FechaInicio))
                {
                    FechaIniContrato = Convert.ToDateTime(FechaInicio);
                }
                if (!String.IsNullOrEmpty(FechaFin))
                {
                    FechaFinContrato = Convert.ToDateTime(FechaFin);
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptListarContratos.ServerReport.ReportServerCredentials = rvc;
                rptListarContratos.ProcessingMode = ProcessingMode.Remote;
                rptListarContratos.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptListarContratos.ServerReport.ReportPath = "/ControlDocumentario/DocumentoContratos/ReporteContratos";

                rptListarContratos.ShowPrintButton = true;
                rptListarContratos.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[7];
                param[0] = new ReportParameter("IdConcepto", Concepto);
                param[1] = new ReportParameter("IdOP", IdOP);
                param[2] = new ReportParameter("EstadoFirma", Estado);
                param[3] = new ReportParameter("IdCliente", IdCliente);
                param[4] = new ReportParameter("IdClienteFinal", IdClienteFinal);
                param[5] = new ReportParameter("FechaInicio", FechaIniContrato.ToString());
                param[6] = new ReportParameter("FechaFin", FechaFinContrato.ToString());

                rptListarContratos.ServerReport.SetParameters(param);
                rptListarContratos.ServerReport.Refresh();
            }
        }
    }
}