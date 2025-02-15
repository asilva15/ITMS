using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Attendance
{
    public partial class ByPerson : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string SIN_DATEs = Convert.ToString(Request.Params["SIN_DATE"]);
                string TO_DATEs = Convert.ToString(Request.Params["TO_DATE"]);
                string ID_ENTI = Convert.ToString(Request.Params["ID_ENTI"]);
                DateTime SIN_DATE, TO_DATE;

                try
                {
                    SIN_DATE = Convert.ToDateTime(SIN_DATEs);
                    TO_DATE = Convert.ToDateTime(TO_DATEs);
                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptByPerson.ServerReport.ReportServerCredentials = rvc;
                    rptByPerson.ProcessingMode = ProcessingMode.Remote;
                    rptByPerson.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptByPerson.ServerReport.ReportPath = "/Asistencia/ReportByPerson";

                    rptByPerson.ShowPrintButton = true;
                    rptByPerson.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[3];
                    param[0] = new ReportParameter("DATE_START", SIN_DATE.ToString());
                    param[1] = new ReportParameter("DATE_END", TO_DATE.ToString());
                    param[2] = new ReportParameter("ID_PERS_ENTI", ID_ENTI);

                    rptByPerson.ServerReport.SetParameters(param);
                    rptByPerson.ServerReport.Refresh();

                }
                catch (Exception ex)
                {
                    //return Content(ex.Message.ToString());

                    //return st
                }


            }
        }
    }
}