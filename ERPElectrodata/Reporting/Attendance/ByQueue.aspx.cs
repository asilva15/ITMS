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
    public partial class ByQueue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string SIN_DATEs = Convert.ToString(Request.Params["SIN_DATE"]);
                string TO_DATEs = Convert.ToString(Request.Params["TO_DATE"]);
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                DateTime SIN_DATE, TO_DATE;

                try
                {
                    SIN_DATE = Convert.ToDateTime(SIN_DATEs);
                    TO_DATE = Convert.ToDateTime(TO_DATEs);
                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptByQueue.ServerReport.ReportServerCredentials = rvc;
                    rptByQueue.ProcessingMode = ProcessingMode.Remote;
                    rptByQueue.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptByQueue.ServerReport.ReportPath = "/Asistencia/INF_ATTE_QUEU";

                    rptByQueue.ShowPrintButton = true;
                    rptByQueue.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[3];
                    param[0] = new ReportParameter("DATE_INI", SIN_DATE.ToString());
                    param[1] = new ReportParameter("DATE_FIN", TO_DATE.ToString());
                    param[2] = new ReportParameter("ID_ACCO", ID_ACCO);

                    rptByQueue.ServerReport.SetParameters(param);
                    rptByQueue.ServerReport.Refresh();

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