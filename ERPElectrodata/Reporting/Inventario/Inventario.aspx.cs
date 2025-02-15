using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Inventario
{
    public partial class Inventario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdTipo = Convert.ToString(Request.Params["tp"]);
                string IdArea = Convert.ToString(Request.Params["ar"]);
                string IdUsuario = Convert.ToString(Request.Params["us"]);
                string Palabra = Convert.ToString(Request.Params["pc"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptInventario.ServerReport.ReportServerCredentials = rvc;
                rptInventario.ProcessingMode = ProcessingMode.Remote;
                rptInventario.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptInventario.ServerReport.ReportPath = "/Inventario/Inventario";

                rptInventario.ShowPrintButton = true;
                rptInventario.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("IdTipo", IdTipo);
                param[0] = new ReportParameter("IdArea", IdArea);
                param[0] = new ReportParameter("IdUsuario", IdUsuario);
                param[0] = new ReportParameter("PalabraClave", Palabra);
                param[0] = new ReportParameter("Traza", "0");

                rptInventario.ServerReport.SetParameters(param);
                rptInventario.ServerReport.Refresh();
            }
        }
    }
}