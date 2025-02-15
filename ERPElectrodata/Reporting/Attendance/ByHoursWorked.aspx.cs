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
    public partial class ByHoursWorked : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string STAR_DATEs = Convert.ToString(Request.Params["STAR_DATE"]);
                string END_DATEs = Convert.ToString(Request.Params["END_DATE"]);
             
                DateTime STAR_DATE, END_DATE;


                try
                {
                    STAR_DATE = Convert.ToDateTime(STAR_DATEs);
                    END_DATE = Convert.ToDateTime(END_DATEs);

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptByHoursWorked.ServerReport.ReportServerCredentials = rvc;
                    rptByHoursWorked.ProcessingMode = ProcessingMode.Remote;
                    rptByHoursWorked.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptByHoursWorked.ServerReport.ReportPath = "/Asistencia/ReportByHours";

                    rptByHoursWorked.ShowPrintButton = true;
                    rptByHoursWorked.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[2];
                    param[0] = new ReportParameter("DATE_STAR", STAR_DATE.ToString());
                    param[1] = new ReportParameter("DATE_END", END_DATE.ToString());
                  //  param[2] = new ReportParameter("TOLERANCIA", TOLERANCE.ToString());

                    rptByHoursWorked.ServerReport.SetParameters(param);
                    rptByHoursWorked.ServerReport.Refresh();

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