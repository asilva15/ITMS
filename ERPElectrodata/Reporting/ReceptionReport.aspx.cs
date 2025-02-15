using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting
{
    public partial class ReceptionReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_TICK = Convert.ToString(Request.Params["id"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int ID_TYPE_ASSE = Convert.ToInt32(Request.Params["ID_TYPE_ASSE"]);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptFormReception.ServerReport.ReportServerCredentials = rvc;
                rptFormReception.ProcessingMode = ProcessingMode.Remote;
                rptFormReception.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptFormReception.ServerReport.ReportPath = ID_ACCO == 3 ? "/ReporteReceptionHB" : "/ReporteReception";

                if (ID_ACCO == 4)
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionED";
                }
                else if (ID_ACCO == 3)
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionHB";
                }
                else if (ID_ACCO == 55) // Hudbay V1
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionHB_V1";
                }
                else if (ID_ACCO == 19)
                {                    
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionBR";
                }
                else if (ID_ACCO == 31)
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionTR";
                }
                else if (ID_ACCO == 56)
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionMS";
                }
                else if (ID_ACCO == 57)
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionMM";
                }
                else if (ID_ACCO == 58)
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionMR";
                }
                else if (ID_ACCO == 60)
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReceptionBNV";
                }
                else
                {
                    rptFormReception.ServerReport.ReportPath = "/ReporteReception";
                }

                rptFormReception.ShowPrintButton = true;
                rptFormReception.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("ID_TICK", ID_TICK);


                rptFormReception.ServerReport.SetParameters(param);
                rptFormReception.ServerReport.Refresh();
            }
        }
    }
}