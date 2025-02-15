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
    public partial class ActxCliente : System.Web.UI.Page
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
                string IdArea = Convert.ToString(Request.Params["IdArea"]);
                if (IdArea == "null") { IdArea = "0"; }
                if (IdTipo == "null") { IdTipo = "0"; }
                
                //string IdArea1 = "0", IdArea2 = "0", IdArea3 = "0", IdArea4 = "0", IdArea5 = "0";
                //int cont = 1;
                //foreach (string strIdAr in IdArea.Split(','))
                //{
                //    if (!String.IsNullOrEmpty(strIdAr))
                //    {
                //        if (cont == 1) { IdArea1 = strIdAr; }
                //        if (cont == 2) { IdArea2 = strIdAr; }
                //        if (cont == 3) { IdArea3 = strIdAr; }
                //        if (cont == 4) { IdArea4 = strIdAr; }
                //        if (cont == 5) { IdArea5 = strIdAr; }
                //        cont++;
                //    }
                //}

                if (String.IsNullOrEmpty(IdTipo)) { IdTipo = "0"; }
                if (String.IsNullOrEmpty(FechaInicio)) { FechaInicio = "1900/01/01"; }
                if (String.IsNullOrEmpty(FechaFin)) { FechaFin = String.Format("{0:yyyy/M/d}", DateTime.Now); }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptActxCliente.ServerReport.ReportServerCredentials = rvc;
                rptActxCliente.ProcessingMode = ProcessingMode.Remote;
                rptActxCliente.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptActxCliente.ServerReport.ReportPath = "/Actividad/MatrizxCliente_2";

                rptActxCliente.ShowPrintButton = true;
                rptActxCliente.ShowParameterPrompts = false;
                if (IdArea == "0") {
                    IdArea = dbe.ObtenerAreasACargo(Convert.ToInt32(Session["ID_PERS_ENTI"])).Single().Areas;
                }
                ReportParameter[] param = new ReportParameter[4];
                param[0] = new ReportParameter("IdTipo", IdTipo);
                param[1] = new ReportParameter("FechaInicio", FechaInicio);
                param[2] = new ReportParameter("FechaFin", FechaFin);
                param[3] = new ReportParameter("AREAS", IdArea);

#if DEBUG
                ReportViewerCredentials rvc2 = new ReportViewerCredentials("administrador", "C4goldP3", "");

                rptActxCliente.ServerReport.ReportServerCredentials = rvc2;
#endif
                rptActxCliente.ServerReport.SetParameters(param);
                rptActxCliente.ServerReport.Refresh();
            }
        }
    }
}