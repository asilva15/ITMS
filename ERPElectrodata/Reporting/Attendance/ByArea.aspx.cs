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
    public partial class ByArea : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string SIN_DATEs = Convert.ToString(Request.Params["SIN_DATE"]);
                string TO_DATEs = Convert.ToString(Request.Params["TO_DATE"]);
                string ID_CHAR_PARE = Convert.ToString(Request.Params["ID_CHAR_PARE"]);
                DateTime SIN_DATE, TO_DATE;

                try
                {
                    SIN_DATE = Convert.ToDateTime(SIN_DATEs);
                    TO_DATE = Convert.ToDateTime(TO_DATEs);
                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptByArea.ServerReport.ReportServerCredentials = rvc;
                    rptByArea.ProcessingMode = ProcessingMode.Remote;
                    rptByArea.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptByArea.ServerReport.ReportPath = "/Asistencia/ByArea";

                    rptByArea.ShowPrintButton = true;
                    rptByArea.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[3];
                    param[0] = new ReportParameter("DATE_START", SIN_DATE.ToString());
                    param[1] = new ReportParameter("DATE_END", TO_DATE.ToString());
                    param[2] = new ReportParameter("ID_CHAR_PARE", ID_CHAR_PARE);

                    rptByArea.ServerReport.SetParameters(param);
                    rptByArea.ServerReport.Refresh();

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