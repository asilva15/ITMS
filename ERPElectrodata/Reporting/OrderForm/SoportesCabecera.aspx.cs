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
    public partial class SoportesCabecera : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string OP = Convert.ToString(Request.Params["OP"].ToString());
                string cbCliente = Convert.ToString(Request.Params["cbCliente"].ToString());
                string cbClienteFinal = Convert.ToString(Request.Params["cbClienteFinal"].ToString());
                string cbMarca = Convert.ToString(Request.Params["cbMarca"].ToString());
                string dtFechaCreacionInicio = Convert.ToString(Request.Params["dtFechaCreacionInicio"].ToString());
                string dtFechaCreacionFin = Convert.ToString(Request.Params["dtFechaCreacionFin"].ToString());
                string dtFinSoporteInicio = Convert.ToString(Request.Params["dtFinSoporteInicio"].ToString());
                string dtFinSoporteFin = Convert.ToString(Request.Params["dtFinSoporteFin"].ToString());
                string IdCuenta = Session["ID_ACCO"].ToString();

                if (OP == "null") OP = "";
                if (cbCliente == "null") cbCliente = "0";
                if (cbClienteFinal == "null") cbClienteFinal = "0";
                if (cbMarca == "null") cbMarca = "0";
                if (dtFechaCreacionInicio == "") dtFechaCreacionInicio = "";
                if (dtFechaCreacionFin == "") dtFechaCreacionFin = "";
                if (dtFinSoporteInicio == "") dtFinSoporteInicio = "";
                if (dtFinSoporteFin == "") dtFinSoporteFin = "";

                string tipoProyecto = "";
                if ((int)Session["ADMINISTRADOR"] == 1) { tipoProyecto = "0"; }
                else if ((int)Session["PROJECTMANAGER"] == 1) { tipoProyecto = "1"; }
                else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1) { tipoProyecto = "2"; }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptSoportesCab.ServerReport.ReportServerCredentials = rvc;
                rptSoportesCab.ProcessingMode = ProcessingMode.Remote;
                rptSoportesCab.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptSoportesCab.ServerReport.ReportPath = "/Proyectos/RptSoportesCab";

                rptSoportesCab.ShowPrintButton = true;
                rptSoportesCab.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[10];
                param[0] = new ReportParameter("OP", OP);
                param[1] = new ReportParameter("Cliente", cbCliente);
                param[2] = new ReportParameter("ClienteFinal", cbClienteFinal);
                param[3] = new ReportParameter("Marca", cbMarca);
                param[4] = new ReportParameter("FechaCreacionInicio", dtFechaCreacionInicio);
                param[5] = new ReportParameter("FechaCreacionFin", dtFechaCreacionFin);
                param[6] = new ReportParameter("FechaFinSoporteInicio", dtFinSoporteInicio);
                param[7] = new ReportParameter("FechaFinSoporteFin", dtFinSoporteFin);
                param[8] = new ReportParameter("TipoProyecto", tipoProyecto);
                param[9] = new ReportParameter("IdCuenta", IdCuenta);

                rptSoportesCab.ServerReport.SetParameters(param);
                rptSoportesCab.ServerReport.Refresh();
            }

        }
    }
}