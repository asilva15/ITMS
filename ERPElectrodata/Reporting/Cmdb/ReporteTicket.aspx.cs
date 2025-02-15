using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Cmdb
{
    public partial class ReporteTicket : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string CodigoTicket = null;
                string Estado = null;
                string IdCompania = null;
                string IdCompaniaFinal = null;
                string IdPersona = null;
                string IdPersonaFinal = null;
                string IdCola = null;
                string IdPersonaAsignada = null;
                string IdTipoTicket = null;
                string IdPrioridad = null;
                string IdCate1 = null;
                string IdCate2 = null;
                string IdCate3 = null;
                string IdCate4 = null;
                string IdCatalogo = null;
                string Comentario = null;
                string FechaInicio = null;
                string FechaFin = null;
                string UserId = null;
                string IdAcco = "0";
                try
                {
                    CodigoTicket = Convert.ToString(Request.Params["ct"]);
                    Estado = Convert.ToString(Request.Params["e"]);
                    IdCompania = Convert.ToString(Request.Params["c"]);
                    IdCompaniaFinal = Convert.ToString(Request.Params["cf"]);
                    IdPersona = Convert.ToString(Request.Params["p"]);
                    IdPersonaFinal = Convert.ToString(Request.Params["pf"]);
                    IdCola = Convert.ToString(Request.Params["cl"]);
                    IdPersonaAsignada = Convert.ToString(Request.Params["pa"]);
                    IdTipoTicket = Convert.ToString(Request.Params["tt"]);
                    IdPrioridad = Convert.ToString(Request.Params["pr"]);
                    IdCate1 = Convert.ToString(Request.Params["c1"]);
                    IdCate2 = Convert.ToString(Request.Params["c2"]);
                    IdCate3 = Convert.ToString(Request.Params["c3"]);
                    IdCate4 = Convert.ToString(Request.Params["c4"]);
                    IdCatalogo = Convert.ToString(Request.Params["cs"]);
                    Comentario = Convert.ToString(Request.Params["cm"]);
                    FechaInicio = Convert.ToString(Request.Params["fi"]);
                    FechaFin = Convert.ToString(Request.Params["ff"]);
                    UserId = Convert.ToString(Request.Params["u"]);
                    IdAcco = Convert.ToString(Session["ID_ACCO"]);

                    if (String.IsNullOrEmpty(Estado)) { Estado = "0"; }
                    if (String.IsNullOrEmpty(IdCompania)) { IdCompania = "0"; }
                    if (String.IsNullOrEmpty(IdPersona)) { IdPersona = "0"; }
                    if (String.IsNullOrEmpty(IdCompaniaFinal)) { IdCompaniaFinal = "0"; }
                    if (String.IsNullOrEmpty(IdPersonaFinal)) { IdPersonaFinal = "0"; }
                    if (String.IsNullOrEmpty(IdCola)) { IdCola = "0"; }
                    if (String.IsNullOrEmpty(IdPersonaAsignada)) { IdPersonaAsignada = "0"; }
                    if (String.IsNullOrEmpty(IdTipoTicket)) { IdTipoTicket = "0"; }
                    if (String.IsNullOrEmpty(IdPrioridad)) { IdPrioridad = "0"; }
                    if (String.IsNullOrEmpty(IdCate1)) { IdCate1 = "0"; }
                    if (String.IsNullOrEmpty(IdCate2)) { IdCate2 = "0"; }
                    if (String.IsNullOrEmpty(IdCate3)) { IdCate3 = "0"; }
                    if (String.IsNullOrEmpty(IdCate4)) { IdCate4 = "0"; }
                    if (String.IsNullOrEmpty(IdCatalogo)) { IdCatalogo = "0"; }
                    if (String.IsNullOrEmpty(UserId)) { UserId = "0"; }
                    if (String.IsNullOrEmpty(FechaInicio)) { FechaInicio = DateTime.MinValue.ToString(); }
                    if (String.IsNullOrEmpty(FechaFin)) { FechaFin = DateTime.Now.ToString(); }
                }
                catch
                {
                    IdAcco = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptTicket.ServerReport.ReportServerCredentials = rvc;
                rptTicket.ProcessingMode = ProcessingMode.Remote;
                rptTicket.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptTicket.ServerReport.ReportPath = "/cmdb/ReporteTicket";

                rptTicket.ShowPrintButton = true;
                rptTicket.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[20];
                param[0] = new ReportParameter("CodigoTicket", CodigoTicket);
                param[1] = new ReportParameter("Estado", Estado);
                param[2] = new ReportParameter("IdCompania", IdCompania);
                param[3] = new ReportParameter("IdCompaniaFinal", IdCompaniaFinal);
                param[4] = new ReportParameter("IdPersona", IdPersona);
                param[5] = new ReportParameter("IdPersonaFinal", IdPersonaFinal);
                param[6] = new ReportParameter("IdCola", IdCola);
                param[7] = new ReportParameter("IdPersonaAsignada", IdPersonaAsignada);
                param[8] = new ReportParameter("IdTipoTicket", IdTipoTicket);
                param[9] = new ReportParameter("IdPrioridad", IdPrioridad);
                param[10] = new ReportParameter("IdCate1", IdCate1);
                param[11] = new ReportParameter("IdCate2", IdCate2);
                param[12] = new ReportParameter("IdCate3", IdCate3);
                param[13] = new ReportParameter("IdCate4", IdCate4);
                param[14] = new ReportParameter("IdCatalogo", IdCatalogo);
                param[15] = new ReportParameter("Comentario", Comentario);
                param[16] = new ReportParameter("FechaInicio", FechaInicio);
                param[17] = new ReportParameter("FechaFin", FechaFin);
                param[18] = new ReportParameter("UserId", UserId);
                param[19] = new ReportParameter("IdAcco", IdAcco);

                rptTicket.ServerReport.SetParameters(param);
                rptTicket.ServerReport.Refresh();
            }
        }
    }
}