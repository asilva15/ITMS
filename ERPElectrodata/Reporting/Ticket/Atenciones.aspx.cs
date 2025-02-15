using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Ticket
{
    public partial class Atenciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdCuenta = Convert.ToString(Request.Params["IdCuenta"]);
                string Anio = Convert.ToString(Request.Params["Anio"]);
                string Mes = Convert.ToString(Request.Params["Mes"]);

                if (String.IsNullOrEmpty(Anio))
                {
                    Anio = "0";
                }
                if (String.IsNullOrEmpty(Mes))
                {
                    Mes = "0";
                }
                if (String.IsNullOrEmpty(IdCuenta))
                {
                    IdCuenta = "0";
                }
                int SubCuenta = 0;

                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["rbSubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                string PSubCuenta = Convert.ToString(SubCuenta);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptAtenciones.ServerReport.ReportServerCredentials = rvc;
                rptAtenciones.ProcessingMode = ProcessingMode.Remote;
                rptAtenciones.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptAtenciones.ServerReport.ReportPath = "/Ticket/Atenciones";

                rptAtenciones.ShowPrintButton = true;
                rptAtenciones.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[4];
                param[0] = new ReportParameter("IdAcco", IdCuenta);
                param[1] = new ReportParameter("Anio", Anio);
                param[2] = new ReportParameter("Mes", Mes);
                param[3] = new ReportParameter("SubCuenta", PSubCuenta);

                rptAtenciones.ServerReport.SetParameters(param);
                rptAtenciones.ServerReport.Refresh();
            }
        }
    }
}