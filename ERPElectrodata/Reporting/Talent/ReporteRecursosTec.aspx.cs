using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Talent
{
    public partial class ReporteRecursosTec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                EntityEntities dbe = new EntityEntities();


                string Id_Solicitud = Convert.ToString((Request.Params["IdSolicitud"]));



                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rtpRecursosTec.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rtpRecursosTec.ProcessingMode = ProcessingMode.Remote;
                rtpRecursosTec.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.
                rtpRecursosTec.ServerReport.ReportPath = "/TalentoHumano/ReporteRecursosTec";

                rtpRecursosTec.ShowPrintButton = true;
                rtpRecursosTec.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("Id_Solicitud", Id_Solicitud);
                //param[1] = new ReportParameter("FROM_DATE", FROM_DATE);
                //param[2] = new ReportParameter("TO_DATE", TO_DATE);

                rtpRecursosTec.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rtpRecursosTec.ServerReport.Refresh();
            }
        }
    }
}