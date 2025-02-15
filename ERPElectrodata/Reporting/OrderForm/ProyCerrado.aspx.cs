using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.OrderForm
{
    public partial class ProyCerrado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string IdCuenta = Session["ID_ACCO"].ToString();
                string IdPM = Convert.ToString( Request.Params["cbPM"].ToString());
                string cbAño = Convert.ToString(Request.Params["cbAño"].ToString());
                string cbMes = Convert.ToString(Request.Params["cbMes"].ToString());
                string cbAñoFin = Convert.ToString(Request.Params["cbAñoFin"].ToString());
                string cbMesFin = Convert.ToString(Request.Params["cbMesFin"].ToString());

                if (IdPM=="null") IdPM = "0";
                if (cbAño == "null") cbAño = "0";
                if (cbMes == "null") cbMes = "0";
                if (cbAñoFin == "null") cbAñoFin = "0";
                if (cbMesFin == "null") cbMesFin = "0";

                string tipoProyecto = "";
                if ((int)Session["ADMINISTRADOR"] == 1) { tipoProyecto = "0"; }
                else if ((int)Session["PROJECTMANAGER"] == 1) { tipoProyecto = "1"; }
                else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1) { tipoProyecto = "2"; }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptProyCerrado.ServerReport.ReportServerCredentials = rvc;
                rptProyCerrado.ProcessingMode = ProcessingMode.Remote;
                rptProyCerrado.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptProyCerrado.ServerReport.ReportPath = "/Proyectos/ProyectoCerrado";

                rptProyCerrado.ShowPrintButton = true;
                rptProyCerrado.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[7];
                param[0] = new ReportParameter("IdPM", IdPM);
                param[1] = new ReportParameter("AñoInicio", cbAño);
                param[2] = new ReportParameter("MesInicio", cbMes);
                param[3] = new ReportParameter("AñoFin", cbAñoFin);
                param[4] = new ReportParameter("MesFin", cbMesFin);
                param[5] = new ReportParameter("TipoProyecto", tipoProyecto);
                param[6] = new ReportParameter("IdCuenta", IdCuenta);

                rptProyCerrado.ServerReport.SetParameters(param);
                rptProyCerrado.ServerReport.Refresh();
            }

        }
    }
}