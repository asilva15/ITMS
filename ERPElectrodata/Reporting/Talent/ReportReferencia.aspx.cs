using ERPElectrodata.Models;
using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Talent
{
    public partial class ReportReferencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                EntityEntities dbe = new EntityEntities();

                //Establecer como parámetro la cuenta actual.
                //int ID_ACCOi = Convert.ToInt32(Session["ID_ACCO"]);
                //string ID_ACCO = null, FROM_DATE = null, TO_DATE;
                //DateTime min_date;
                string Id_Solicitud = Convert.ToString((Request.Params["IdSolicitud"]));
                
                //try
                //{
                    //Obtenemos la fecha del primer ticket perteneciente a la cuenta.
                    //min_date = Convert.ToDateTime(dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCOi).OrderBy(t => t.FEC_TICK).FirstOrDefault().FEC_TICK);
                //}
                //catch
                //{
                    //min_date = DateTime.MinValue;
                //}

                //ID_ACCO = Convert.ToString(Session["ID_ACCO"]);

                //try
                //{
                //    Id_Solicitud = Convert.ToString((Request.Params["IdSolicitud"]));
                //    //TO_DATE = Convert.ToString(Request.Params["TO_DATE"]);
                //}
                //catch
                //{
                //    //FROM_DATE = Convert.ToString(min_date);
                //}

                //try
                //{
                //    TO_DATE = Convert.ToString(Request.Params["TO_DATE"]);
                //}
                //catch
                //{
                //    TO_DATE = Convert.ToString(DateTime.Now);
                //}

                //if (String.IsNullOrEmpty(FROM_DATE))
                //{
                //    FROM_DATE = Convert.ToString(min_date);
                //}

                //if (String.IsNullOrEmpty(TO_DATE))
                //{
                //    TO_DATE = Convert.ToString(DateTime.Now);
                //}

                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptReferencias.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rptReferencias.ProcessingMode = ProcessingMode.Remote;
                rptReferencias.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.
                rptReferencias.ServerReport.ReportPath = "/TalentoHumano/ReferenciasCandidatos";

                rptReferencias.ShowPrintButton = true;
                rptReferencias.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("Id_Solicitud", Id_Solicitud);
                //param[1] = new ReportParameter("FROM_DATE", FROM_DATE);
                //param[2] = new ReportParameter("TO_DATE", TO_DATE);

                rptReferencias.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rptReferencias.ServerReport.Refresh();
            }
        }
    }
}