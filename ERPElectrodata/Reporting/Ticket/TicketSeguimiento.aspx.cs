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
    public partial class TicketSeguimiento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string IdCuenta = Convert.ToString(Request.Params["IdCuenta"]);
                string IdCompania = Convert.ToString(Request.Params["IdCompania"]);
                string IdUsuario = Convert.ToString(Request.Params["IdUsuario"]);
                string IdEstado = Convert.ToString(Request.Params["IdEstado"]);
                string FechaIniCreacion = Convert.ToString(Request.Params["FechaIniCreacion"]);
                string FechaFinCreacion = Convert.ToString(Request.Params["FechaFinCreacion"]);

                if (String.IsNullOrEmpty(IdUsuario))
                {
                    IdUsuario = "0";
                }
                if (String.IsNullOrEmpty(IdCompania))
                {
                    IdCompania = "0";
                }
                if (String.IsNullOrEmpty(IdCuenta))
                {
                    IdCuenta = "0";
                }
                if (String.IsNullOrEmpty(IdEstado))
                {
                    IdEstado = "0";
                }
                if (String.IsNullOrEmpty(FechaIniCreacion))
                {
                    FechaIniCreacion = null;
                }
                if (String.IsNullOrEmpty(FechaFinCreacion))
                {
                    FechaFinCreacion = null;
                }
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
                rptSeguimiento.ServerReport.ReportServerCredentials = rvc;
                rptSeguimiento.ProcessingMode = ProcessingMode.Remote;
                rptSeguimiento.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptSeguimiento.ServerReport.ReportPath = "/Ticket/TicketSeguimiento";

                rptSeguimiento.ShowPrintButton = true;
                rptSeguimiento.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[7];
                param[0] = new ReportParameter("IdAcco", IdCuenta);
                param[1] = new ReportParameter("IdCompania", IdCompania);
                param[2] = new ReportParameter("IdPersEnti", IdUsuario);
                param[3] = new ReportParameter("IdEstado", IdEstado);
                param[4] = new ReportParameter("FechaIniCreacion", FechaIniCreacion);
                param[5] = new ReportParameter("FechaFinCreacion", FechaFinCreacion);
                param[6] = new ReportParameter("SubCuenta", PSubCuenta);
                //#if DEBUG
                //                ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");
                //                rptSeguimiento.ServerReport.ReportServerCredentials = rvc;
                //#endif
                rptSeguimiento.ServerReport.SetParameters(param);
                rptSeguimiento.ServerReport.Refresh();
            }
        }
    }
}