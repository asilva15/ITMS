using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting
{
    public partial class ReporteTipoActividad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                string cbUsuario = Convert.ToString(Request.Params["cbUsuario"]);
                string dtFechaInicio = Convert.ToString(Request.Params["dtFechaInicio"]);
                string dtFechaFin = Convert.ToString(Request.Params["dtFechaFin"]);
                string cbTipoActividad = Convert.ToString(Request.Params["cbTipoActividad"]);
                string ID_PERS_ENTI = Convert.ToString(Session["ID_PERS_ENTI"].ToString());
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"].ToString());
                string IdArea = Convert.ToString(Request.Params["IdArea"]);

                string cbTipoActividad1 = "0", cbTipoActividad2 = "0", cbTipoActividad3 = "0", cbTipoActividad4 = "0", cbTipoActividad5 = "0";
                if (cbUsuario == "null") { cbUsuario = "0"; }
                if (cbTipoActividad == "null") { cbTipoActividad = "0"; }
                if (IdArea == "null") { IdArea = "0"; }

                //int cont = 1;
                //foreach (string strIdEva in cbTipoActividad.Split(','))
                //{
                //    if (!String.IsNullOrEmpty(strIdEva))
                //    {
                //        if (cont == 1) { cbTipoActividad1 = strIdEva; }
                //        if (cont == 2) { cbTipoActividad2 = strIdEva; }
                //        if (cont == 3) { cbTipoActividad3 = strIdEva; }
                //        if (cont == 4) { cbTipoActividad4 = strIdEva; }
                //        if (cont == 5) { cbTipoActividad5 = strIdEva; }
                //        cont++;
                //    }
                //}

                string IdArea1 = "0", IdArea2 = "0", IdArea3 = "0", IdArea4 = "0", IdArea5 = "0";
                //int contA = 1;
                //foreach (string strIdAr in IdArea.Split(','))
                //{
                //    if (!String.IsNullOrEmpty(strIdAr))
                //    {
                //        if (contA == 1) { IdArea1 = strIdAr; }
                //        if (contA == 2) { IdArea2 = strIdAr; }
                //        if (contA == 3) { IdArea3 = strIdAr; }
                //        if (contA == 4) { IdArea4 = strIdAr; }
                //        if (contA == 5) { IdArea5 = strIdAr; }
                //        contA++;
                //    }
                //}

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptTipoActividad.ServerReport.ReportServerCredentials = rvc;
                rptTipoActividad.ProcessingMode = ProcessingMode.Remote;
                rptTipoActividad.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptTipoActividad.ServerReport.ReportPath = "/Actividad/RptTipoActividad_2";

                rptTipoActividad.ShowPrintButton = true;
                rptTipoActividad.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[17];
                param[0] = new ReportParameter("ID_PERS_ENTI", ID_PERS_ENTI);
                param[1] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[2] = new ReportParameter("IdUsuario", cbUsuario);
                param[3] = new ReportParameter("FechaInicio", dtFechaInicio);
                param[4] = new ReportParameter("FechaFin", dtFechaFin);
                param[5] = new ReportParameter("TA1", cbTipoActividad1);
                param[6] = new ReportParameter("TA2", cbTipoActividad2);
                param[7] = new ReportParameter("TA3", cbTipoActividad3);
                param[8] = new ReportParameter("TA4", cbTipoActividad4);
                param[9] = new ReportParameter("TA5", cbTipoActividad5);
                param[10] = new ReportParameter("AR1", IdArea1);
                param[11] = new ReportParameter("AR2", IdArea2);
                param[12] = new ReportParameter("AR3", IdArea3);
                param[13] = new ReportParameter("AR4", IdArea4);
                param[14] = new ReportParameter("AR5", IdArea5);
                if (cbTipoActividad != "0")
                    param[15] = new ReportParameter("TipoActividad", cbTipoActividad + ",");
                else
                    param[15] = new ReportParameter("TipoActividad", cbTipoActividad);
                if (IdArea != "0")
                    param[16] = new ReportParameter("Area", IdArea + ",");
                else
                    param[16] = new ReportParameter("Area", IdArea);

#if DEBUG
                ReportViewerCredentials rvc2 = new ReportViewerCredentials("administrador", "C4goldP3", "");

                rptTipoActividad.ServerReport.ReportServerCredentials = rvc2;
#endif

                rptTipoActividad.ServerReport.SetParameters(param);
                rptTipoActividad.ServerReport.Refresh();
            }
        }
    }
}
