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
    public partial class ProyServicios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdPM = Request.Params["PM"].ToString();
                string IdCuenta = Session["ID_ACCO"].ToString();

                if (String.IsNullOrEmpty(IdPM)) IdPM = "0";

                string tipoProyecto = "";
                if ((int)Session["ADMINISTRADOR"] == 1) { tipoProyecto = "0"; }
                else if ((int)Session["PROJECTMANAGER"] == 1) { tipoProyecto = "1"; }
                else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1) { tipoProyecto = "2"; }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptProyServicios.ServerReport.ReportServerCredentials = rvc;
                rptProyServicios.ProcessingMode = ProcessingMode.Remote;
                rptProyServicios.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptProyServicios.ServerReport.ReportPath = "/Proyectos/ProyectoServicios";

                rptProyServicios.ShowPrintButton = true;
                rptProyServicios.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("IdPM", IdPM);
                param[1] = new ReportParameter("TipoProyecto", tipoProyecto);
                param[2] = new ReportParameter("IdCuenta", IdCuenta);

                rptProyServicios.ServerReport.SetParameters(param);
                rptProyServicios.ServerReport.Refresh();
            }
        }
    }
}