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
    public partial class ProyIndicadores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdCuenta = Session["ID_ACCO"].ToString();
                string IdPM = Request.Params["PM"].ToString();
                string dtFechaInicio = Convert.ToString(Request.Params["dtFechaInicio"].ToString());
                string dtFechaFin = Convert.ToString(Request.Params["dtFechaFin"].ToString());

                if (String.IsNullOrEmpty(IdPM)) IdPM = "0";
                if (String.IsNullOrEmpty(dtFechaInicio)) dtFechaInicio = "";
                if (String.IsNullOrEmpty(dtFechaFin)) dtFechaFin = "";

                string tipoProyecto = "";
                if ((int)Session["ADMINISTRADOR"] == 1) { tipoProyecto = "0"; }
                else if ((int)Session["PROJECTMANAGER"] == 1) { tipoProyecto = "1"; }
                else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1) { tipoProyecto = "2"; }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptProyIndicadores.ServerReport.ReportServerCredentials = rvc;
                rptProyIndicadores.ProcessingMode = ProcessingMode.Remote;
                rptProyIndicadores.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptProyIndicadores.ServerReport.ReportPath = "/Proyectos/RptIndicadores";

                rptProyIndicadores.ShowPrintButton = true;
                rptProyIndicadores.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[5];
                param[0] = new ReportParameter("IdPM", IdPM);
                param[1] = new ReportParameter("FechaInicio", dtFechaInicio);
                param[2] = new ReportParameter("FechaFin", dtFechaFin);
                param[3] = new ReportParameter("TipoProyecto", tipoProyecto);
                param[4] = new ReportParameter("IdCuenta", IdCuenta);

                rptProyIndicadores.ServerReport.SetParameters(param);
                rptProyIndicadores.ServerReport.Refresh();
            }
        }
    }
}