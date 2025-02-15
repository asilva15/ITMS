using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.ControlDocumentario.DocumentoVenta
{
    public partial class ListarSoluciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                string EstadoTipo = Convert.ToString(Request.Params["radio"]);
                string IdMarca = Convert.ToString(Request.Params["IdMarca"]);
                string IdSolucionTI = Convert.ToString(Request.Params["IdSolucionTI"]);
                string IdTipoOP = Convert.ToString(Request.Params["IdTipoOP"]);
                string PalabraClave = Convert.ToString(Request.Params["PalabraClave"]);
                string IdCliente = Convert.ToString(Request.Params["IdCliente"]);
                string IdClienteFinal = Convert.ToString(Request.Params["IdClienteFinal"]);
                string FechaInicio = Convert.ToString(Request.Params["FechaInicio"]);
                string FechaFin = Convert.ToString(Request.Params["FechaFin"]);
                string MontoMinimoSoles = Convert.ToString(Request.Params["MontoMinimoSoles"]);
                string MontoMaximoSoles = Convert.ToString(Request.Params["MontoMaximoSoles"]);
                string MontoMinimoDolares = Convert.ToString(Request.Params["MontoMinimoDolares"]);
                string MontoMaximoDolares = Convert.ToString(Request.Params["MontoMaximoDolares"]);
                string NroDocumento = Convert.ToString(Request.Params["NroDocumento"]);
                string inroDoc;

                if (String.IsNullOrEmpty(IdMarca)) { IdMarca = "0"; }
                if (String.IsNullOrEmpty(IdSolucionTI)) { IdSolucionTI = "0"; }
                if (String.IsNullOrEmpty(IdTipoOP)) { IdTipoOP = "0"; }
                if (String.IsNullOrEmpty(IdCliente)) { IdCliente = "0"; }
                if (String.IsNullOrEmpty(IdClienteFinal)) { IdClienteFinal = "0"; }
                if (String.IsNullOrEmpty(NroDocumento)) { inroDoc = "0"; } else inroDoc = NroDocumento;

                DateTime FechaIniContrato = Convert.ToDateTime("1900/01/01");
                DateTime FechaFinContrato = Convert.ToDateTime("9999/12/31");

                int MinimoSoles = 0;
                int MaximoSoles = 999999999;
                int MinimoDolares = 0;
                int MaximoDolares = 999999999;

                if (!String.IsNullOrEmpty(FechaInicio))
                {
                    FechaIniContrato = Convert.ToDateTime(FechaInicio);
                }
                if (!String.IsNullOrEmpty(FechaFin))
                {
                    FechaFinContrato = Convert.ToDateTime(FechaFin);
                }
                if (!String.IsNullOrEmpty(MontoMinimoSoles))
                {
                    MinimoSoles = Convert.ToInt32(MontoMinimoSoles);
                }
                if (!String.IsNullOrEmpty(MontoMaximoSoles))
                {
                    MaximoSoles = Convert.ToInt32(MontoMaximoSoles);
                }
                if (!String.IsNullOrEmpty(MontoMinimoDolares))
                {
                    MinimoDolares = Convert.ToInt32(MontoMinimoDolares);
                }
                if (!String.IsNullOrEmpty(MontoMaximoDolares))
                {
                    MaximoDolares = Convert.ToInt32(MontoMaximoDolares);
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptListarSoluciones.ServerReport.ReportServerCredentials = rvc;
                rptListarSoluciones.ProcessingMode = ProcessingMode.Remote;
                rptListarSoluciones.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptListarSoluciones.ServerReport.ReportPath = "/ControlDocumentario/DocumentoVenta/ReporteSoluciones";

                rptListarSoluciones.ShowPrintButton = true;
                rptListarSoluciones.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[14];
                param[0] = new ReportParameter("IdSolucionTI", IdSolucionTI);
                param[1] = new ReportParameter("IdTipoOP", IdTipoOP);
                param[2] = new ReportParameter("IdMarca", IdMarca);
                param[3] = new ReportParameter("IdCliente", IdCliente);
                param[4] = new ReportParameter("IdClienteFinal", IdClienteFinal);
                param[5] = new ReportParameter("FechaInicio", FechaIniContrato.ToString());
                param[6] = new ReportParameter("FechaFin", FechaFinContrato.ToString());
                param[7] = new ReportParameter("PalabraClave", PalabraClave);
                param[8] = new ReportParameter("MinimoSoles", MinimoSoles.ToString());
                param[9] = new ReportParameter("MaximoSoles", MaximoSoles.ToString());
                param[10] = new ReportParameter("MinimoDolares", MinimoDolares.ToString());
                param[11] = new ReportParameter("MaximoDolares", MaximoDolares.ToString());
                param[12] = new ReportParameter("EstadoTipo", EstadoTipo.ToString());
                param[13] = new ReportParameter("NroDocumento", inroDoc);

                rptListarSoluciones.ServerReport.SetParameters(param);
                rptListarSoluciones.ServerReport.Refresh();
            }
        }
    }
}