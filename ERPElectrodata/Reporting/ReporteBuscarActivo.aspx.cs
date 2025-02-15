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
    public partial class ReporteBuscarActivo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string Codigo = null;
                string Usuario = null;
                string Contrato = null;
                string TipoActivo = null;
                string Marca = null;
                string ModeloComercial = null;
                string ModeloFabrica = null;
                string Lugar = null;
                string Locacion = null;
                string Estado = null;
                string Condicion = null;
                string Serie = null;
                string Mac = null;
                string Nombre = null;
                string ID_ACCO = null;
                string NumeroFactura = null;
                string GuiaRemision = null;
                string OrdenCompra = null;
                string UsuariosResp = null;
                string Grupo = null;
                try
                {
                    Codigo = Convert.ToString(Request.Params["ca"]);
                    Usuario = Convert.ToString(Request.Params["u"]);
                    Contrato = Convert.ToString(Request.Params["c"]);
                    TipoActivo = Convert.ToString(Request.Params["ta"]);
                    Marca = Convert.ToString(Request.Params["m"]);
                    ModeloComercial = Convert.ToString(Request.Params["mc"]);
                    ModeloFabrica = Convert.ToString(Request.Params["mf"]);
                    Lugar = Convert.ToString(Request.Params["l"]);
                    Locacion = Convert.ToString(Request.Params["lo"]);
                    Estado = Convert.ToString(Request.Params["e"]);
                    Condicion = Convert.ToString(Request.Params["cn"]);
                    Serie = Convert.ToString(Request.Params["s"]);
                    Mac = Convert.ToString(Request.Params["d"]);
                    Nombre = Convert.ToString(Request.Params["n"]);
                    NumeroFactura = Convert.ToString(Request.Params["nf"]);
                    GuiaRemision = Convert.ToString(Request.Params["gr"]);
                    OrdenCompra = Convert.ToString(Request.Params["oc"]);
                    ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                    UsuariosResp = Convert.ToString(Request.Params["urr"]);
                    Grupo = Convert.ToString(Request.Params["gru"]);

                    if (Usuario == "0," || String.IsNullOrEmpty(Usuario)) { Usuario = ""; }
                    else { Usuario = Usuario.Substring(0, Usuario.Length - 1); }
                    if (TipoActivo == "0," || String.IsNullOrEmpty(TipoActivo)) { TipoActivo = ""; }
                    else { TipoActivo = TipoActivo.Substring(0, TipoActivo.Length - 1); }
                    if (Contrato == "0," || String.IsNullOrEmpty(Contrato)) { Contrato = ""; }
                    else { Contrato = Contrato.Substring(0, Contrato.Length - 1); }
                    if (UsuariosResp == "0," || String.IsNullOrEmpty(UsuariosResp)) { UsuariosResp = ""; }
                    else { UsuariosResp = UsuariosResp.Substring(0, UsuariosResp.Length - 1); }
                }
                catch
                {
                    ID_ACCO = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rv_ReportAssetsByUser.ServerReport.ReportServerCredentials = rvc;
                rv_ReportAssetsByUser.ProcessingMode = ProcessingMode.Remote;
                rv_ReportAssetsByUser.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportAssetsByUser.ServerReport.ReportPath = "/ReporteActivos";

                rv_ReportAssetsByUser.ShowPrintButton = true;
                rv_ReportAssetsByUser.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[20];
                param[0] = new ReportParameter("Codigo", Codigo);
                param[1] = new ReportParameter("Usuario", Usuario);
                param[2] = new ReportParameter("Contrato", Contrato);
                param[3] = new ReportParameter("TipoActivo", TipoActivo);
                param[4] = new ReportParameter("Marca", Marca);
                param[5] = new ReportParameter("ModeloComercial", ModeloComercial);
                param[6] = new ReportParameter("ModeloFabrica", ModeloFabrica);
                param[7] = new ReportParameter("Lugar", Lugar);
                param[8] = new ReportParameter("Locacion", Locacion);
                param[9] = new ReportParameter("Estado", Estado);
                param[10] = new ReportParameter("Condicion", Condicion);
                param[11] = new ReportParameter("Serie", Serie);
                param[12] = new ReportParameter("Mac", Mac);
                param[13] = new ReportParameter("Nombre", Nombre);
                param[14] = new ReportParameter("NumeroFactura", NumeroFactura);
                param[15] = new ReportParameter("GuiaRemision", GuiaRemision);
                param[16] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[17] = new ReportParameter("UsuariosResp", UsuariosResp);
                param[18] = new ReportParameter("Grupo", Grupo);
                param[19] = new ReportParameter("OrdenCompra", OrdenCompra);

                rv_ReportAssetsByUser.ServerReport.SetParameters(param);

                rv_ReportAssetsByUser.ServerReport.Refresh();
            }
        }
    }
}