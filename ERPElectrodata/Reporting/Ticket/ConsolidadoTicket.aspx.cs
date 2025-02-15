using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Ticket
{
    public partial class ConsolidadoTicket : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idCuenta = Convert.ToString(Request.Params["idCuenta"]);
                string idComp = Convert.ToString(Request.Params["idComp"]);
                string idArea = Convert.ToString(Request.Params["idArea"]);
                string idEstado = Convert.ToString(Request.Params["idEstado"]);
                string fechaIni = Convert.ToString(Request.Params["fechaIni"]);
                string fechaFin = Convert.ToString(Request.Params["fechaFin"]);

                if(idCuenta == "")
                    idCuenta = "0";
                if (idComp == "")
                    idComp = "0";
                if (idArea == "")
                    idArea = "0";
                if (idEstado == "")
                    idEstado = "0";

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptConsolidado.ServerReport.ReportServerCredentials = rvc;
                rptConsolidado.ProcessingMode = ProcessingMode.Remote;
                rptConsolidado.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptConsolidado.ServerReport.ReportPath = "/Actividad/ConsolidadoTicket";
                
                rptConsolidado.ShowPrintButton = true;
                rptConsolidado.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[6];
                param[0] = new ReportParameter("ID_ACCO", idCuenta);
                param[1] = new ReportParameter("ID_CIA", idComp);
                param[2] = new ReportParameter("ID_QUEU", idArea);
                param[3] = new ReportParameter("STATUS", idEstado);
                param[4] = new ReportParameter("FROM_DATE", fechaIni);
                param[5] = new ReportParameter("TO_DATE", fechaFin);
                
                rptConsolidado.ServerReport.SetParameters(param);
                rptConsolidado.ServerReport.Refresh();
            }
        }
    }
}