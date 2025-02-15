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
    public partial class Perfiles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdPerfil = Convert.ToString(Request.Params["idPerfil"]);
                string IdUsuario = Convert.ToString(Request.Params["IdUsuario"]);
                string FlagPerfil = Convert.ToString(Request.Params["FlagPerfil"]);

                if (String.IsNullOrEmpty(IdPerfil))
                {
                    IdPerfil = "0";
                }
                if (String.IsNullOrEmpty(IdUsuario))
                {
                    IdUsuario = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptPerfiles.ServerReport.ReportServerCredentials = rvc;
                rptPerfiles.ProcessingMode = ProcessingMode.Remote;
                rptPerfiles.ServerReport.ReportServerUrl = new Uri(reportServer);
                if (FlagPerfil == "on")
                {
                    rptPerfiles.ServerReport.ReportPath = "/Seguridad/Perfiles";
                }
                else
                {
                    rptPerfiles.ServerReport.ReportPath = "/Seguridad/UsuarioPerfiles";
                }
                
                rptPerfiles.ShowPrintButton = true;
                rptPerfiles.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("IdUsuario", IdUsuario);
                param[1] = new ReportParameter("IdPerfil", IdPerfil);

                rptPerfiles.ServerReport.SetParameters(param);
                rptPerfiles.ServerReport.Refresh();
            }
        }
    }
}