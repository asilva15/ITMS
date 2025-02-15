using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Linq;
using System.Net;

namespace ERPElectrodata.Reporting
{
    public partial class RptTransferenciaActivos_CambioGuardia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                CoreEntities dbc = new CoreEntities();
                //Declaramos los parámetros de entrada para el reporte
                string IdLoca = "0";
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

                if (Request.Params["IdLoca"] != "")
                {
                    IdLoca = Convert.ToString(Request.Params["IdLoca"]);
                }
                else
                {
                    IdLoca = "0";
                }
                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptTransferenciaActivos.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rptTransferenciaActivos.ProcessingMode = ProcessingMode.Remote;
                rptTransferenciaActivos.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.
                rptTransferenciaActivos.ServerReport.ReportPath = "/Activo/RptTransferenciaActivos_CambioGuardia";

                rptTransferenciaActivos.ShowPrintButton = true;
                rptTransferenciaActivos.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("IdLoca", IdLoca);
//#if DEBUG
//                ReportViewerCredentials rvc = new ReportViewerCredentials("administrator", "ITMS$15DEV$", "");

//                rptTransferenciaActivos.ServerReport.ReportServerCredentials = rvc;
//#endif

                rptTransferenciaActivos.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rptTransferenciaActivos.ServerReport.Refresh();
            }
        }
    }


}