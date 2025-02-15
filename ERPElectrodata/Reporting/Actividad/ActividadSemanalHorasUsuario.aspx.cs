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
    public partial class ActividadSemanalHorasUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {

                string ID_PERS_ENTI = Convert.ToString(Request.Params["ID_PERS_ENTI"]);
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"].ToString());
                string anio = Convert.ToString(Request.Params["ANIO"]);
                string meses = Convert.ToString(Request.Params["MES"]);

                string IdPersEnti1 = "0", IdPersEnti2 = "0", IdPersEnti3 = "0", IdPersEnti4 = "0", IdPersEnti5 = "0";
                int cont=1;
                //foreach (string strIdEva in ID_PERS_ENTI.Split(','))
                //{
                //    if (!String.IsNullOrEmpty(strIdEva))
                //    {
                //        if (cont == 1) { IdPersEnti1 = strIdEva; }
                //        if (cont == 2) { IdPersEnti2 = strIdEva; }
                //        if (cont == 3) { IdPersEnti3 = strIdEva; }
                //        if (cont == 4) { IdPersEnti4 = strIdEva; }
                //        if (cont == 5) { IdPersEnti5 = strIdEva; }
                //         cont++;
                //    }
                //}

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptActividadSemanalHrsUsuario.ServerReport.ReportServerCredentials = rvc;
                rptActividadSemanalHrsUsuario.ProcessingMode = ProcessingMode.Remote;
                rptActividadSemanalHrsUsuario.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptActividadSemanalHrsUsuario.ServerReport.ReportPath = "/ReporteActividadesUsuario_2";

                rptActividadSemanalHrsUsuario.ShowPrintButton = true;
                rptActividadSemanalHrsUsuario.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[9];
                param[0] = new ReportParameter("IdPersEnti1", IdPersEnti1);
                param[1] = new ReportParameter("IdPersEnti2", IdPersEnti2);
                param[2] = new ReportParameter("IdPersEnti3", IdPersEnti3);
                param[3] = new ReportParameter("IdPersEnti4", IdPersEnti4);
                param[4] = new ReportParameter("IdPersEnti5", IdPersEnti5);
                param[5] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[6] = new ReportParameter("ANIO", anio);
                param[7] = new ReportParameter("MES", meses);
                param[8] = new ReportParameter("IdPersEnti", ID_PERS_ENTI);

#if DEBUG
                ReportViewerCredentials rvc2 = new ReportViewerCredentials("administrador", "C4goldP3", "");

                rptActividadSemanalHrsUsuario.ServerReport.ReportServerCredentials = rvc2;
#endif
                rptActividadSemanalHrsUsuario.ServerReport.SetParameters(param);
                rptActividadSemanalHrsUsuario.ServerReport.Refresh();
            }
        }
    }
}