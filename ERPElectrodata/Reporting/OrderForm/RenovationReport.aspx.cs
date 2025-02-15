using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.OrderForm
{
    public partial class RenovationReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (!IsPostBack)
                {

                    //string[] roles = Roles.GetRolesForUser();
                    //string[] IN_ROLE = { "RRHH", "ADMINISTRADOR", "SYSTEMADMINISTRATOR" };
                    //bool construir = false;
                    //foreach (string xc in IN_ROLE)
                    //{ construir = true; }
                    //if (construir == true)
                    //{
                    //string ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptRenovation.ServerReport.ReportServerCredentials = rvc;
                    rptRenovation.ProcessingMode = ProcessingMode.Remote;
                    rptRenovation.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptRenovation.ServerReport.ReportPath = "/Proyectos/OF_RENOVATION";
                    rptRenovation.ShowPrintButton = true;
                    rptRenovation.ShowParameterPrompts = false;
                    //ReportParameter[] param = new ReportParameter[1];
                    //param[0] = new ReportParameter("ID_ACCO", ID_ACCO);
                    //rptRenovation.ServerReport.SetParameters(param);
                    rptRenovation.ServerReport.Refresh();
                }
            }
            catch
            {
                //return Content("NOT PRIVILEGIES");
            }
        }
    }
}