using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Cmdb
{
    public partial class ReporteActivo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string codigoActivo = null;
                string usuario = null;
                string contrato = null;
                string tipoActivo = null;
                string marca = null;
                string modeloComercial = null;
                string modeloFabrica = null;
                string lugar = null;
                string locacion = null;
                string estado = null;
                string condicion = null;
                string numeroSerie = null;
                string direccionMAC = null;
                string nombreEquipo = null;
                string IdCatalogo = null;
                string IdAcco = "0";

                codigoActivo = Convert.ToString(Request.Params["ca"]);
                usuario = Convert.ToString(Request.Params["u"]);
                contrato = Convert.ToString(Request.Params["c"]);
                tipoActivo = Convert.ToString(Request.Params["ta"]);
                marca = Convert.ToString(Request.Params["m"]);
                modeloComercial = Convert.ToString(Request.Params["mc"]);
                modeloFabrica = Convert.ToString(Request.Params["mf"]);
                lugar = Convert.ToString(Request.Params["l"]);
                locacion = Convert.ToString(Request.Params["lo"]);
                estado = Convert.ToString(Request.Params["e"]);
                condicion = Convert.ToString(Request.Params["cn"]);
                numeroSerie = Convert.ToString(Request.Params["s"]);
                direccionMAC = Convert.ToString(Request.Params["d"]);
                nombreEquipo = Convert.ToString(Request.Params["n"]);
                IdCatalogo = Convert.ToString(Request.Params["cs"]);
                IdAcco = Convert.ToString(Session["ID_ACCO"]);

                if (String.IsNullOrEmpty(usuario)) { usuario = "0"; }
                if (String.IsNullOrEmpty(contrato)) { contrato = "0"; }
                if (String.IsNullOrEmpty(tipoActivo)) { tipoActivo = "0"; }
                if (String.IsNullOrEmpty(marca)) { marca = "0"; }
                if (String.IsNullOrEmpty(modeloComercial)) { modeloComercial = "0"; }
                if (String.IsNullOrEmpty(modeloFabrica)) { modeloFabrica = "0"; }
                if (String.IsNullOrEmpty(lugar)) { lugar = "0"; }
                if (String.IsNullOrEmpty(locacion)) { locacion = "0"; }
                if (String.IsNullOrEmpty(estado)) { estado = "0"; }
                if (String.IsNullOrEmpty(condicion)) { condicion = "0"; }
                if (String.IsNullOrEmpty(IdCatalogo)) { IdCatalogo = "0"; }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptActivo.ServerReport.ReportServerCredentials = rvc;
                rptActivo.ProcessingMode = ProcessingMode.Remote;
                rptActivo.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptActivo.ServerReport.ReportPath = "/cmdb/ReporteActivo";

                rptActivo.ShowPrintButton = true;
                rptActivo.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[16];
                param[0] = new ReportParameter("Codigo", codigoActivo);
                param[1] = new ReportParameter("Usuario", usuario);
                param[2] = new ReportParameter("Contrato", contrato);
                param[3] = new ReportParameter("TipoActivo", tipoActivo);
                param[4] = new ReportParameter("Marca", marca);
                param[5] = new ReportParameter("ModeloComercial", modeloComercial);
                param[6] = new ReportParameter("ModeloFabrica", modeloFabrica);
                param[7] = new ReportParameter("Lugar", lugar);
                param[8] = new ReportParameter("Locacion", locacion);
                param[9] = new ReportParameter("Estado", estado);
                param[10] = new ReportParameter("Condicion", condicion);
                param[11] = new ReportParameter("Serie", numeroSerie);
                param[12] = new ReportParameter("Mac", direccionMAC);
                param[13] = new ReportParameter("Nombre", nombreEquipo);
                param[14] = new ReportParameter("IdCatalogo", IdCatalogo);
                param[15] = new ReportParameter("IdCuenta", IdAcco);

                rptActivo.ServerReport.SetParameters(param);
                rptActivo.ServerReport.Refresh();
            }
        }
    }
}