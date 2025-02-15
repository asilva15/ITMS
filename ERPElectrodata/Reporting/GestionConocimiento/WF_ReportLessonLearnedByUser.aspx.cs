using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.GestionConocimiento
{
    public partial class WF_ReportLessonLearnedByUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_USUARIO = null;
                try
                {
                    ID_USUARIO = Convert.ToString(Request.Params["id"]);
                }
                catch
                {
                    ID_USUARIO = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rv_ReportLessonLearnedByUser.ServerReport.ReportServerCredentials = rvc;
                rv_ReportLessonLearnedByUser.ProcessingMode = ProcessingMode.Remote;
                rv_ReportLessonLearnedByUser.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportLessonLearnedByUser.ServerReport.ReportPath = "/GestionConocimiento/LeccionesAprendidas";

                rv_ReportLessonLearnedByUser.ShowPrintButton = true;
                rv_ReportLessonLearnedByUser.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("idUsuario", ID_USUARIO);

                rv_ReportLessonLearnedByUser.ServerReport.SetParameters(param);
                rv_ReportLessonLearnedByUser.ServerReport.Refresh();
            }

        }
    }
}