using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Cmdb
{
    public partial class ReportePersona : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string nombres = null;
                string apellidos = null;
                string celular = null;
                string extension = null;
                string correo = null;
                string correoElectrodata = null;
                string correoPersonal = null;
                string locacion = null;
                string tipoCargo = null;
                string idNombreCargo = null;
                string cargo = null;
                string estado = null;
                string condicion = null;
                string fechaInicio = null;
                string fechaFin = null;
                string IdCatalogo = null;
                string IdCuenta = "0";
                try
                {
                    nombres = Convert.ToString(Request.Params["n"]);
                    apellidos = Convert.ToString(Request.Params["a"]);
                    celular = Convert.ToString(Request.Params["cl"]);
                    extension = Convert.ToString(Request.Params["ex"]);
                    correo = Convert.ToString(Request.Params["cr"]);
                    correoElectrodata = Convert.ToString(Request.Params["ce"]);
                    correoPersonal = Convert.ToString(Request.Params["cp"]);
                    locacion = Convert.ToString(Request.Params["l"]);
                    tipoCargo = Convert.ToString(Request.Params["tc"]);
                    idNombreCargo = Convert.ToString(Request.Params["ic"]);
                    cargo = Convert.ToString(Request.Params["c"]);
                    estado = Convert.ToString(Request.Params["e"]);
                    condicion = Convert.ToString(Request.Params["co"]);
                    fechaInicio = Convert.ToString(Request.Params["fi"]);
                    fechaFin = Convert.ToString(Request.Params["ff"]);
                    IdCatalogo = Convert.ToString(Request.Params["ct"]);
                    IdCuenta = Convert.ToString(Session["ID_ACCO"]);

                    if (String.IsNullOrEmpty(idNombreCargo)) { idNombreCargo = "0"; }
                    if (String.IsNullOrEmpty(locacion)) { locacion = "0"; }
                    if (String.IsNullOrEmpty(estado)) { estado = "0"; }
                    if (String.IsNullOrEmpty(condicion)) { condicion = "0"; }
                    if (String.IsNullOrEmpty(tipoCargo)) { tipoCargo = "0"; }
                    if (String.IsNullOrEmpty(IdCatalogo)) { IdCatalogo = "0"; }
                    if (String.IsNullOrEmpty(fechaInicio)) { fechaInicio = DateTime.MinValue.ToString(); }
                    if (String.IsNullOrEmpty(fechaFin)) { fechaFin = DateTime.Now.ToString(); }
                }
                catch
                {
                    IdCuenta = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptPersona.ServerReport.ReportServerCredentials = rvc;
                rptPersona.ProcessingMode = ProcessingMode.Remote;
                rptPersona.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptPersona.ServerReport.ReportPath = "/cmdb/ReportePersona";

                rptPersona.ShowPrintButton = true;
                rptPersona.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[17];
                param[0] = new ReportParameter("Nombres", nombres);
                param[1] = new ReportParameter("Apellidos", apellidos);
                param[2] = new ReportParameter("Celular", celular);
                param[3] = new ReportParameter("Extension", extension);
                param[4] = new ReportParameter("Correo", correo);
                param[5] = new ReportParameter("CorreoElectrodata", correoElectrodata);
                param[6] = new ReportParameter("CorreoPersonal", correoPersonal);
                param[7] = new ReportParameter("Locacion", locacion);
                param[8] = new ReportParameter("TipoCargo", tipoCargo);
                param[9] = new ReportParameter("IdNombreCargo", idNombreCargo);
                param[10] = new ReportParameter("Cargo", cargo);
                param[11] = new ReportParameter("Estado", estado);
                param[12] = new ReportParameter("Condicion", condicion);
                param[13] = new ReportParameter("FechaInicio", fechaInicio);
                param[14] = new ReportParameter("FechaFin", fechaFin);
                param[15] = new ReportParameter("IdCatalogo", IdCatalogo);
                param[16] = new ReportParameter("IdCuenta", IdCuenta);

                rptPersona.ServerReport.SetParameters(param);
                rptPersona.ServerReport.Refresh();
            }
        }
    }
}