using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Seguridad
{
    public partial class Usuarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdUsuario = Convert.ToString(Request.Params["idUsuario"]);
                string NombreUsuario = Convert.ToString(Request.Params["Nombres"]);

                if (String.IsNullOrEmpty(IdUsuario)){
                    IdUsuario = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptUsuarios.ServerReport.ReportServerCredentials = rvc;
                rptUsuarios.ProcessingMode = ProcessingMode.Remote;
                rptUsuarios.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptUsuarios.ServerReport.ReportPath = "/Seguridad/Usuarios";

                rptUsuarios.ShowPrintButton = true;
                rptUsuarios.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("IdUsuario", IdUsuario);
                param[1] = new ReportParameter("NombreUsuario", NombreUsuario);

                rptUsuarios.ServerReport.SetParameters(param);
                rptUsuarios.ServerReport.Refresh();
            }
        }
    }
}