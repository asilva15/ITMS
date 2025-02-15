using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Actividad
{
    public partial class ActividadSemanal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdCuenta = Convert.ToString(Session["ID_ACCO"]);
                string FechaInicio = Request.QueryString["SIN_DATE"].ToString();
                string FechaFin = Request.QueryString["TO_DATE"].ToString();
                string IdPersona = Request.QueryString["IdPersona"].ToString();

                if(String.IsNullOrEmpty(IdPersona)){
                    IdPersona = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptActividadSemanal.ServerReport.ReportServerCredentials = rvc;

                rptActividadSemanal.ProcessingMode = ProcessingMode.Remote;
                rptActividadSemanal.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptActividadSemanal.ServerReport.ReportPath = "/Actividad/ActividadSemanal";

                rptActividadSemanal.ShowPrintButton = true;
                rptActividadSemanal.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[3];
                param[0] = new ReportParameter("fechaini", FechaInicio);
                param[1] = new ReportParameter("fechafin", FechaFin);
                param[2] = new ReportParameter("idPersona", IdPersona);

                rptActividadSemanal.ServerReport.SetParameters(param);
                rptActividadSemanal.ServerReport.Refresh();
            }
        }
    }
}