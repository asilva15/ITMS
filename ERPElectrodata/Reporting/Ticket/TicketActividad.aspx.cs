using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Ticket
{
    public partial class TicketActividad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                string FechaInicio = Convert.ToString(Request.Params["FechaInicial"]);
                string FechaFin = Convert.ToString(Request.Params["FechaFinal"]);
                string IdCuenta = Convert.ToString(Session["ID_ACCO"]);
                string IdPersona = Convert.ToString(Session["ID_PERS_ENTI"]);
                int SubCuenta = 0;

                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["rbSubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                string PSubCuenta = Convert.ToString(SubCuenta);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptTicketActividad.ServerReport.ReportServerCredentials = rvc;
                rptTicketActividad.ProcessingMode = ProcessingMode.Remote;
                rptTicketActividad.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptTicketActividad.ServerReport.ReportPath = "/Ticket/TicketActividad";

                rptTicketActividad.ShowPrintButton = true;
                rptTicketActividad.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[5];
                param[0] = new ReportParameter("FechaInicio", FechaInicio);
                param[1] = new ReportParameter("FechaFinal", FechaFin);
                param[2] = new ReportParameter("IdCuenta", IdCuenta);
                param[3] = new ReportParameter("IdPersona", IdPersona);
                param[4] = new ReportParameter("SubCuenta", PSubCuenta);

                rptTicketActividad.ServerReport.SetParameters(param);
                rptTicketActividad.ServerReport.Refresh();
            }
        }
    }
}