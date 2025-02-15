using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;
//using ERPElectrodata.Models;

namespace ERPElectrodata.Reporting
{
    public partial class WF_ReportChangeSalary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ANIO = null;
                try
                {
                    ANIO = Convert.ToString(Request.Params["CB_YEAR"]);
                }
                catch
                {
                    ANIO = Convert.ToString(DateTime.Now.Year);
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rv_ReportChangeSalary.ServerReport.ReportServerCredentials = rvc;
                rv_ReportChangeSalary.ProcessingMode = ProcessingMode.Remote;
                rv_ReportChangeSalary.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportChangeSalary.ServerReport.ReportPath = "/ReportChangeSalary";

                rv_ReportChangeSalary.ShowPrintButton = true;
                rv_ReportChangeSalary.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("Year", ANIO);

                rv_ReportChangeSalary.ServerReport.SetParameters(param);

                rv_ReportChangeSalary.ServerReport.Refresh();
            }
        }

    }
}