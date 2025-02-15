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
    public partial class TicketCreaCierra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int IdPersEnti = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                string IdCuenta = Convert.ToString(Request.Params["IdCuenta"]);
                string Anio = Convert.ToString(Request.Params["Anio"]);
                string Mes = Convert.ToString(Request.Params["Mes"]);

                if (String.IsNullOrEmpty(Anio))
                {
                    Anio = "0";
                }
                if (String.IsNullOrEmpty(Mes))
                {
                    Mes = "0";
                }
                if (String.IsNullOrEmpty(IdCuenta))
                {
                    IdCuenta = "0";
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
                rptTicketCreaCierra.ServerReport.ReportServerCredentials = rvc;
                rptTicketCreaCierra.ProcessingMode = ProcessingMode.Remote;
                rptTicketCreaCierra.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptTicketCreaCierra.ServerReport.ReportPath = "/Ticket/TicketCreadoCerrado";

                rptTicketCreaCierra.ShowPrintButton = true;
                rptTicketCreaCierra.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[5];
                param[0] = new ReportParameter("IdPersEnti", IdPersEnti.ToString());
                param[1] = new ReportParameter("IdCuenta", IdCuenta);
                param[2] = new ReportParameter("Anio", Anio);
                param[3] = new ReportParameter("Mes", Mes);
                param[4] = new ReportParameter("SubCuenta", PSubCuenta);

                rptTicketCreaCierra.ServerReport.SetParameters(param);
                rptTicketCreaCierra.ServerReport.Refresh();
            }
        }
    }
}