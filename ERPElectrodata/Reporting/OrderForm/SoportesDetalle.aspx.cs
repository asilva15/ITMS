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
    public partial class SoportesDetalle : System.Web.UI.Page
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
                string dtMantenimientoInicio = Convert.ToString(Request.Params["dtMantenimientoInicio"].ToString());
                string dtMantenimientoFin = Convert.ToString(Request.Params["dtMantenimientoFin"].ToString());
                string IdCuenta = Session["ID_ACCO"].ToString();

                if (OP == "null") OP = "";
                if (cbCliente == "null") cbCliente = "0";
                if (cbClienteFinal == "null") cbClienteFinal = "0";
                if (cbMarca == "null") cbMarca = "0";
                if (dtFechaCreacionInicio == "") dtFechaCreacionInicio = "";
                if (dtFechaCreacionFin == "") dtFechaCreacionFin = "";
                if (dtFinSoporteInicio == "") dtFinSoporteInicio = "";
                if (dtFinSoporteFin == "") dtFinSoporteFin = "";
                if (dtMantenimientoInicio == "") dtMantenimientoInicio = "";
                if (dtMantenimientoFin == "") dtMantenimientoFin = "";

                string tipoProyecto = "";
                if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SERVICEDESK"] == 1) { tipoProyecto = "0"; }
                else if ((int)Session["PROJECTMANAGER"] == 1) { tipoProyecto = "1"; }
                else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1) { tipoProyecto = "2"; }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptSoporteEDDet.ServerReport.ReportServerCredentials = rvc;
                rptSoporteEDDet.ProcessingMode = ProcessingMode.Remote;
                rptSoporteEDDet.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptSoporteEDDet.ServerReport.ReportPath = "/Proyectos/RptSoportesDetEd";

                rptSoporteEDDet.ShowPrintButton = true;
                rptSoporteEDDet.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[12];
                param[0] = new ReportParameter("OP", OP);
                param[1] = new ReportParameter("Cliente", cbCliente);
                param[2] = new ReportParameter("ClienteFinal", cbClienteFinal);
                param[3] = new ReportParameter("Marca", cbMarca);
                param[4] = new ReportParameter("FechaCreacionInicio", dtFechaCreacionInicio);
                param[5] = new ReportParameter("FechaCreacionFin", dtFechaCreacionFin);
                param[6] = new ReportParameter("FechaFinSoporteInicio", dtFinSoporteInicio);
                param[7] = new ReportParameter("FechaFinSoporteFin", dtFinSoporteFin);
                param[8] = new ReportParameter("FechaMantInicio", dtMantenimientoInicio);
                param[9] = new ReportParameter("FechaMantFin", dtMantenimientoFin);
                param[10] = new ReportParameter("TipoProyecto", tipoProyecto);
                param[11] = new ReportParameter("IdCuenta", IdCuenta);

//#if DEBUG
//                ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");

//                rptSoporteEDDet.ServerReport.ReportServerCredentials = rvc;
//#endif
                rptSoporteEDDet.ServerReport.SetParameters(param);
                rptSoporteEDDet.ServerReport.Refresh();
            }
        }
    }
}