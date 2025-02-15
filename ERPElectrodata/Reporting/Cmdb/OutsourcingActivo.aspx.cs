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
    public partial class OutsourcingActivo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdCuenta = Session["ID_ACCO"].ToString();
                string IdCuentaArea = Request.QueryString["idCuentaArea"].ToString();

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptOutsourcingActivo.ServerReport.ReportServerCredentials = rvc;
                rptOutsourcingActivo.ProcessingMode = ProcessingMode.Remote;
                rptOutsourcingActivo.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptOutsourcingActivo.ServerReport.ReportPath = "/Cmdb/OutsourcingActivo";

                rptOutsourcingActivo.ShowPrintButton = true;
                rptOutsourcingActivo.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("IdCuenta", IdCuenta);
                param[1] = new ReportParameter("IdCuentaArea", IdCuentaArea);

                rptOutsourcingActivo.ServerReport.SetParameters(param);
                rptOutsourcingActivo.ServerReport.Refresh();
            }
        }
    }
}