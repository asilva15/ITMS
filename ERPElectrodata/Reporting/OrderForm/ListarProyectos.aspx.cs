using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.OrderForm
{
    public partial class ListarProyectos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdTipo = Convert.ToString(Request.Params["IdTipo"]);
                string Codigo = Convert.ToString(Request.Params["Codigo"]);
                string IdPM = Convert.ToString(Request.Params["IdPM"]);
                string IdIng = Convert.ToString(Request.Params["IdIng"]);
                string IdCliente = Convert.ToString(Request.Params["IdCliente"]);
                string IdEstado = Convert.ToString(Request.Params["IdEstado"]);
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                string FechaInicio = Convert.ToString(Request.Params["FechaInicio"]);
                string FechaFin = Convert.ToString(Request.Params["FechaFin"]);
                string TipoProyectoPM = Convert.ToString(Request.Params["IdTipoProyecto"]);

                if (String.IsNullOrEmpty(IdTipo)) { IdTipo = "0"; }
                if (String.IsNullOrEmpty(IdPM)) { IdPM = "0"; }
                if (String.IsNullOrEmpty(IdIng)) { IdIng = "0"; }
                if (String.IsNullOrEmpty(IdCliente)) { IdCliente = "0"; }
                if (String.IsNullOrEmpty(IdEstado)) { IdEstado = "0"; }
                if (String.IsNullOrEmpty(FechaInicio)) { FechaInicio = ""; }
                if (String.IsNullOrEmpty(FechaFin)) { FechaFin = ""; }
                if (String.IsNullOrEmpty(TipoProyectoPM)) { TipoProyectoPM = "0"; }

                string tipoProyecto = "";
                if ((int)Session["ADMINISTRADOR"] == 1) { tipoProyecto = "0"; }
                else if ((int)Session["PROJECTMANAGER"] == 1) { tipoProyecto = "1"; }
                else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1) { tipoProyecto = "2"; }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptListarProyectos.ServerReport.ReportServerCredentials = rvc;
                rptListarProyectos.ProcessingMode = ProcessingMode.Remote;
                rptListarProyectos.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptListarProyectos.ServerReport.ReportPath = "/Proyectos/ReporteProyecto";

                rptListarProyectos.ShowPrintButton = true;
                rptListarProyectos.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[11];
                param[0] = new ReportParameter("IdTipo", IdTipo);
                param[1] = new ReportParameter("Codigo", Codigo);
                param[2] = new ReportParameter("IdPM", IdPM);
                param[3] = new ReportParameter("IdIng", IdIng);
                param[4] = new ReportParameter("IdCliente", IdCliente);
                param[5] = new ReportParameter("IdEstado", IdEstado);
                param[6] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[7] = new ReportParameter("FechaInicio", FechaInicio);
                param[8] = new ReportParameter("FechaFin", FechaFin);
                param[9] = new ReportParameter("TipoProyecto", tipoProyecto);
                param[10] = new ReportParameter("TipoProyectoPM", TipoProyectoPM);

                rptListarProyectos.ServerReport.SetParameters(param);
                rptListarProyectos.ServerReport.Refresh();
            }
        }
    }
}