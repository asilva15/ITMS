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
    public partial class ReporteInforme : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string IdCuenta = Session["ID_ACCO"].ToString();
                string dtFechaInicio = Convert.ToString(Request.Params["dtFechaInicio"].ToString());
                string dtFechaFin = Convert.ToString(Request.Params["dtFechaFin"].ToString());
                string cbCliente = Convert.ToString(Request.Params["cbCliente"].ToString());
                string cbClienteFinal = Convert.ToString(Request.Params["cbClienteFinal"].ToString());
                string cbMarca = Convert.ToString(Request.Params["cbMarca"].ToString());
                string cbEstadoInforme = Convert.ToString(Request.Params["cbEstadoInforme"].ToString());
                string cbResponsable = Convert.ToString(Request.Params["cbResponsable"].ToString());
                string cbTipoInforme = Convert.ToString(Request.Params["cbTipoInforme"].ToString());
                string cbTipoEntrega = Convert.ToString(Request.Params["cbTipoEntrega"].ToString());
                string OP = Convert.ToString(Request.Params["OP"].ToString());

                if (dtFechaInicio == "") dtFechaInicio = "";
                if (dtFechaFin == "") dtFechaFin = "";
                if (cbCliente == "null" || String.IsNullOrEmpty(cbCliente)) cbCliente = "0";
                if (cbClienteFinal == "null" || String.IsNullOrEmpty(cbClienteFinal)) cbClienteFinal = "0";
                if (cbMarca == "null" || String.IsNullOrEmpty(cbMarca)) cbMarca = "0";
                if (cbEstadoInforme == "null" || String.IsNullOrEmpty(cbEstadoInforme)) cbEstadoInforme = "0";
                if (cbResponsable == "null" || String.IsNullOrEmpty(cbResponsable)) cbResponsable = "0";
                if (cbTipoInforme == "null" || String.IsNullOrEmpty(cbTipoInforme)) cbTipoInforme = "0";
                if (cbTipoEntrega == "null" || String.IsNullOrEmpty(cbTipoEntrega)) cbTipoEntrega = "";
                if (OP == "null" || String.IsNullOrEmpty(OP)) OP = "";
                
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptInforme.ServerReport.ReportServerCredentials = rvc;
                rptInforme.ProcessingMode = ProcessingMode.Remote;
                rptInforme.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptInforme.ServerReport.ReportPath = "/Proyectos/RptInforme";

                rptInforme.ShowPrintButton = true;
                rptInforme.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[10];
                param[0] = new ReportParameter("FInicio", dtFechaInicio);
                param[1] = new ReportParameter("FFin", dtFechaFin);
                param[2] = new ReportParameter("Cliente", cbCliente);
                param[3] = new ReportParameter("CliFinal", cbClienteFinal);
                param[4] = new ReportParameter("Marca", cbMarca);
                param[5] = new ReportParameter("Estado", cbEstadoInforme);
                param[6] = new ReportParameter("Resp", cbResponsable);
                param[7] = new ReportParameter("TInforme", cbTipoInforme);
                param[8] = new ReportParameter("TEntrega", cbTipoEntrega);
                param[9] = new ReportParameter("OP", OP);

                rptInforme.ServerReport.SetParameters(param);
                rptInforme.ServerReport.Refresh();
            }

        }
    }
}