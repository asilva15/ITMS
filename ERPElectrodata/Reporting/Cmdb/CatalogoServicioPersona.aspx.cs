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
    public partial class CatalogoServicioPersona : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdCuenta = Session["ID_ACCO"].ToString();
                string IdServicio = Request.QueryString["idServicio"].ToString();

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptCatalogoServicioPersona.ServerReport.ReportServerCredentials = rvc;
                rptCatalogoServicioPersona.ProcessingMode = ProcessingMode.Remote;
                rptCatalogoServicioPersona.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptCatalogoServicioPersona.ServerReport.ReportPath = "/Cmdb/CatalogoServicioPersona";

                rptCatalogoServicioPersona.ShowPrintButton = true;
                rptCatalogoServicioPersona.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("IdCuenta", IdCuenta);
                param[1] = new ReportParameter("IdServicio", IdServicio);

                rptCatalogoServicioPersona.ServerReport.SetParameters(param);
                rptCatalogoServicioPersona.ServerReport.Refresh();
            }
        }
    }
}