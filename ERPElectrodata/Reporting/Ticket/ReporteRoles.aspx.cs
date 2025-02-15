using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Ticket
{
    public partial class ReporteRoles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_ACCO = null;

                string usuario = Convert.ToString(Request.Params["usuario"]);
                string rol = Convert.ToString(Request.Params["rol"]);

                if (String.IsNullOrEmpty(usuario)) { usuario = ""; }
                if (String.IsNullOrEmpty(rol)) { rol = ""; }

                try
                {

                    ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                }
                catch
                {

                    ID_ACCO = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                RVRoles.ServerReport.ReportServerCredentials = rvc;
                RVRoles.ProcessingMode = ProcessingMode.Remote;
                RVRoles.ServerReport.ReportServerUrl = new Uri(reportServer);
                RVRoles.ServerReport.ReportPath = "/Ticket/RVRoles";

                RVRoles.ShowPrintButton = true;
                RVRoles.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("Cuenta", ID_ACCO);
                param[1] = new ReportParameter("Usuario", usuario);
                param[2] = new ReportParameter("Rol", rol);

                RVRoles.ServerReport.SetParameters(param);

                RVRoles.ServerReport.Refresh();
            }
        }
    }
}