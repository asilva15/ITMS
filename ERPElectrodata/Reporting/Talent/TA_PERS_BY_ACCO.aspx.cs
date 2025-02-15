using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Web.Security;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Talent
{
    public partial class TA_PERS_BY_ACCO : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string usuario = Membership.GetUser().UserName;
            try
            {
                

                if (!IsPostBack)
                {

                    string[] roles = Roles.GetRolesForUser();
                    string[] IN_ROLE = { "RRHH", "ADMINISTRADOR", "SYSTEMADMINISTRATOR" };
                    bool construir = false;

                    foreach (string xc in IN_ROLE)
                    {construir = true;}

                    if(construir == true)
                    {
                        string ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                        var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                        var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                        var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                        IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                        rptPersByAcco.ServerReport.ReportServerCredentials = rvc;
                        rptPersByAcco.ProcessingMode = ProcessingMode.Remote;
                        rptPersByAcco.ServerReport.ReportServerUrl = new Uri(reportServer);
                        rptPersByAcco.ServerReport.ReportPath = "/TalentoHumano/TA_PERS_BY_ACCO";
                        rptPersByAcco.ShowPrintButton = true;
                        rptPersByAcco.ShowParameterPrompts = false;
                        ReportParameter[] param = new ReportParameter[1];
                        param[0] = new ReportParameter("ID_ACCO", ID_ACCO);
                        rptPersByAcco.ServerReport.SetParameters(param);
                        rptPersByAcco.ServerReport.Refresh();
                    }
                }
            }
            catch
            {
                //return Content("NOT PRIVILEGIES");
            }
        }
    }
}