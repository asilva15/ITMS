using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Linq;
using System.Net;

namespace ERPElectrodata.Reporting
{
    public partial class ReporteTotalActivoMantenimiento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                CoreEntities dbc = new CoreEntities();
                //Declaramos los parámetros de entrada para el reporte
                string IdTipoActivo = "0";
                string IdActivo = "0";
                string FechaInicio = "0";
                string FechaFin = "0";
                string IdAcco = Convert.ToString(Session["ID_ACCO"]);
                //try
                //{
                //    //obtenemos los valores de entrada
                //    IdTipoActivo = Convert.ToString(Request.Params["IdTipoActivo"]);
                //    IdActivo = Convert.ToString(Request.Params["IdActivo"]);
                //}
                //catch
                //{
                //    IdTipoActivo = "0";
                //    IdActivo = "0";
                //}

                if (Request.Params["IdTipoActivo"] != "")
                {
                    IdTipoActivo = Convert.ToString(Request.Params["IdTipoActivo"]);
                }
                else
                {
                    IdTipoActivo = "0";
                }

                if (Convert.ToString(Request.Params["IdActivo"]) != "")
                {
                    IdActivo = Convert.ToString(Request.Params["IdActivo"]);
                }
                else
                {
                    IdActivo = "0";
                }

                if (Convert.ToString(Request.Params["FechaInicio"]) != "")
                {
                    FechaInicio = Convert.ToString(Request.Params["FechaInicio"]);
                }
                else
                {
                    FechaInicio = "NULL";
                }

                if (Convert.ToString(Request.Params["FechaFin"]) != "")
                {
                    FechaFin = Convert.ToString(Request.Params["FechaFin"]);
                }
                else
                {
                    FechaFin = "NULL";
                }
                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptTotalActivoMantenimiento.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rptTotalActivoMantenimiento.ProcessingMode = ProcessingMode.Remote;
                rptTotalActivoMantenimiento.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.
                rptTotalActivoMantenimiento.ServerReport.ReportPath = "/Activo/ReporteTotalActivoMantenimiento";

                rptTotalActivoMantenimiento.ShowPrintButton = true;
                rptTotalActivoMantenimiento.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[5];
                param[0] = new ReportParameter("IdTipoActivo", IdTipoActivo);
                param[1] = new ReportParameter("IdActivo", IdActivo);
                param[2] = new ReportParameter("FechaInicio", FechaInicio);
                param[3] = new ReportParameter("FechaFin", FechaFin);
                param[4] = new ReportParameter("IdAcco", IdAcco);

//#if DEBUG
//                ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");

//                rptTotalActivoMantenimiento.ServerReport.ReportServerCredentials = rvc;
//#endif

                rptTotalActivoMantenimiento.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rptTotalActivoMantenimiento.ServerReport.Refresh();
            }
        }
    }


}