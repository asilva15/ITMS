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

namespace ERPElectrodata.Reporting.Actividad
{
    public partial class ActxIngeniero : System.Web.UI.Page
    {
        public EntityEntities dbe = new EntityEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                string IdTipo = Convert.ToString(Request.Params["IdTipo"]);
                string FechaInicio = Convert.ToString(Request.Params["FechaInicio"]);
                string FechaFin = Convert.ToString(Request.Params["FechaFin"]);
                string IdIngeniero = Convert.ToString(Request.Params["IdIngeniero"]);

                string IdIngeniero1 = null, IdIngeniero2 = null, IdIngeniero3 = null, IdIngeniero4 = null, IdIngeniero5 = null;
                
                if (String.IsNullOrEmpty(IdTipo)) { IdTipo = "0"; }
                if (String.IsNullOrEmpty(FechaInicio)) { FechaInicio = "1900/01/01"; }
                if (String.IsNullOrEmpty(FechaFin)) { FechaFin = String.Format("{0:yyyy/M/d}", DateTime.Now); }
                IdIngeniero1 = "0";
                IdIngeniero2 = "0";
                IdIngeniero3 = "0";
                IdIngeniero4 = "0";
                IdIngeniero5 = "0";
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptActxIngeniero.ServerReport.ReportServerCredentials = rvc;
                rptActxIngeniero.ProcessingMode = ProcessingMode.Remote;
                rptActxIngeniero.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptActxIngeniero.ServerReport.ReportPath = "/Actividad/ActxIngeniero_2";

                rptActxIngeniero.ShowPrintButton = true;
                rptActxIngeniero.ShowParameterPrompts = false;
                if (IdIngeniero == "null")
                {
                    IdIngeniero = dbe.ObtenerUsuariosACargo(Convert.ToInt32(Session["ID_PERS_ENTI"])).Single().Usuarios;
                }
                ReportParameter[] param = new ReportParameter[9];
                param[0] = new ReportParameter("IdTipo", IdTipo);
                param[1] = new ReportParameter("FechaInicio", FechaInicio);
                param[2] = new ReportParameter("FechaFin", FechaFin);
                param[3] = new ReportParameter("IdIngeniero1", IdIngeniero1);
                param[4] = new ReportParameter("IdIngeniero2", IdIngeniero2);
                param[5] = new ReportParameter("IdIngeniero3", IdIngeniero3);
                param[6] = new ReportParameter("IdIngeniero4", IdIngeniero4);
                param[7] = new ReportParameter("IdIngeniero5", IdIngeniero5);
                param[8] = new ReportParameter("Ingenieros", IdIngeniero);


                rptActxIngeniero.ServerReport.SetParameters(param);
                rptActxIngeniero.ServerReport.Refresh();
            }
        }
    }
}