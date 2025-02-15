using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Ticket
{
    public partial class ReporteCategoria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_ACCO = null;
                string ID_CATE_N1 = Convert.ToString(Request.Params["c1"]);
                string ID_CATE_N2 = Convert.ToString(Request.Params["c2"]);
                string ID_CATE_N3 = Convert.ToString(Request.Params["c3"]);
                string ID_CATE_N4 = Convert.ToString(Request.Params["c4"]);
                string keyword = Convert.ToString(Request.Params["keyword"]);
                string Estado = Convert.ToString(Request.Params["estado"]);
                string ID_TYPE_TICK = Convert.ToString(Request.Params["idtype"]);


                if (String.IsNullOrEmpty(ID_CATE_N1)) { ID_CATE_N1 = "0"; }
                if (String.IsNullOrEmpty(ID_CATE_N2)) { ID_CATE_N2 = "0"; }
                if (String.IsNullOrEmpty(ID_CATE_N3)) { ID_CATE_N3 = "0"; }
                if (String.IsNullOrEmpty(ID_CATE_N4)) { ID_CATE_N4 = "0"; }
                if (String.IsNullOrEmpty(keyword)) { keyword = ""; }
                if (String.IsNullOrEmpty(Estado)) { Estado = "0"; }
                if (String.IsNullOrEmpty(ID_TYPE_TICK)) { ID_TYPE_TICK = "0"; }

                try
                {

                    ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                }
                catch
                {

                    ID_ACCO = "0";
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                RVCategorias.ServerReport.ReportServerCredentials = rvc;
                RVCategorias.ProcessingMode = ProcessingMode.Remote;
                RVCategorias.ServerReport.ReportServerUrl = new Uri(reportServer);
                RVCategorias.ServerReport.ReportPath = "/Ticket/RVCategorias";

                RVCategorias.ShowPrintButton = true;
                RVCategorias.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[8];
                param[0] = new ReportParameter("IdCuenta", ID_ACCO);
                param[1] = new ReportParameter("Categoria1", ID_CATE_N1);
                param[2] = new ReportParameter("Categoria2", ID_CATE_N2);
                param[3] = new ReportParameter("Categoria3", ID_CATE_N3);
                param[4] = new ReportParameter("Categoria4", ID_CATE_N4);
                param[5] = new ReportParameter("PalabraClave", keyword);
                param[6] = new ReportParameter("Estado", Estado);
                param[7] = new ReportParameter("IdTypeTicket", ID_TYPE_TICK);

                RVCategorias.ServerReport.SetParameters(param);

                RVCategorias.ServerReport.Refresh();
            }

        }
    }
}