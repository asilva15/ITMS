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
    public partial class WF_ReportLessonLearnedByCategory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_CATEGORY = null;
                try
                {
                    ID_CATEGORY = Convert.ToString(Request.Params["ID_CATEGORY"]);
                }
                catch
                {
                    ID_CATEGORY = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rv_ReportLessonLearnedBycategory.ServerReport.ReportServerCredentials = rvc;
                rv_ReportLessonLearnedBycategory.ProcessingMode = ProcessingMode.Remote;
                rv_ReportLessonLearnedBycategory.ServerReport.ReportServerUrl = new Uri(reportServer);
                rv_ReportLessonLearnedBycategory.ServerReport.ReportPath = "/GestionConocimiento/LeccionesAprendidasCategorias";

                rv_ReportLessonLearnedBycategory.ShowPrintButton = true;
                rv_ReportLessonLearnedBycategory.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("idCategoria", ID_CATEGORY);


                rv_ReportLessonLearnedBycategory.ServerReport.SetParameters(param);

                rv_ReportLessonLearnedBycategory.ServerReport.Refresh();
            }
        }
    }
}