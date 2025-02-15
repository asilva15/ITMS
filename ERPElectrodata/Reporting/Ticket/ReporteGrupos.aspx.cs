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
    public partial class ReporteGrupos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_ACCO = null;

                string usuario = Convert.ToString(Request.Params["usuario"]);
                string grupo = Convert.ToString(Request.Params["grupo"]);

                if (String.IsNullOrEmpty(usuario)) { usuario = "0"; }
                if (String.IsNullOrEmpty(grupo)) { grupo = ""; }

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
                RVGrupos.ServerReport.ReportServerCredentials = rvc;
                RVGrupos.ProcessingMode = ProcessingMode.Remote;
                RVGrupos.ServerReport.ReportServerUrl = new Uri(reportServer);
                RVGrupos.ServerReport.ReportPath = "/Ticket/RVGrupos";

                RVGrupos.ShowPrintButton = true;
                RVGrupos.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("Cuenta", ID_ACCO);
                param[1] = new ReportParameter("Usuario", usuario);
                param[2] = new ReportParameter("Grupo", grupo);

                RVGrupos.ServerReport.SetParameters(param);

                RVGrupos.ServerReport.Refresh();
            }
        }
    }
}