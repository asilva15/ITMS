using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Activo
{
    public partial class ActivoTraza : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_ASSE = null;
                string ID_ACCO = null;
                try
                {
                    ID_ASSE = Convert.ToString(Request.Params["id"]);
                    ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                }
                catch
                {
                    ID_ASSE = "0";
                    ID_ACCO = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                RVTrazabilidad.ServerReport.ReportServerCredentials = rvc;
                RVTrazabilidad.ProcessingMode = ProcessingMode.Remote;
                RVTrazabilidad.ServerReport.ReportServerUrl = new Uri(reportServer);
                RVTrazabilidad.ServerReport.ReportPath = "/Activo/Trazabilidad";

                RVTrazabilidad.ShowPrintButton = true;
                RVTrazabilidad.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("ID_ASSE", ID_ASSE);
                param[1] = new ReportParameter("ID_ACCO", ID_ACCO);

                RVTrazabilidad.ServerReport.SetParameters(param);

                RVTrazabilidad.ServerReport.Refresh();
            }
        }
    }
}