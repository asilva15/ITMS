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
    public partial class InformeAutomatico : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                string IdPlantilla = Convert.ToString(Request.Params["Id"]);
                DateTime FechaInicio = Convert.ToDateTime(Request.QueryString["FechaInicio"]);
                DateTime FechaFin = Convert.ToDateTime(Request.QueryString["FechaFin"]);
                string ID_ACCO = Convert.ToString(Session["ID_ACCO"].ToString());

                int idCliente = 0, idClienteFinal = 0, MostrarFirma = 0;
                int IdAreaResponsable = 0, IdAsignadoA = 0, IdCategoria = 0,
                    IdSubCategoria = 0, IdClase = 0, IdSubClase = 0, IdTipoTicket = 0;

                if (int.TryParse(Convert.ToString(Request.Params["Cliente"]), out idCliente) == false)
                {
                    idCliente = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["ClienteFinal"]), out idClienteFinal) == false)
                {
                    idClienteFinal = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["IdAreaResponsable"]), out IdAreaResponsable) == false)
                {
                    IdAreaResponsable = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdAsignadoA"]), out IdAsignadoA) == false)
                {
                    IdAsignadoA = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdCategoria"]), out IdCategoria) == false)
                {
                    IdCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubCategoria"]), out IdSubCategoria) == false)
                {
                    IdSubCategoria = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdClase"]), out IdClase) == false)
                {
                    IdClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdSubClase"]), out IdSubClase) == false)
                {
                    IdSubClase = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["IdTipoTicket"]), out IdTipoTicket) == false)
                {
                    IdTipoTicket = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["MostrarFirma"]), out MostrarFirma) == false)
                {
                    MostrarFirma = 0;
                }
                int SubCuenta = 0;

                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["SubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }
                string PSubCuenta = Convert.ToString(SubCuenta);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptInformeAutomatico.ServerReport.ReportServerCredentials = rvc;
                rptInformeAutomatico.ProcessingMode = ProcessingMode.Remote;
                rptInformeAutomatico.ServerReport.ReportServerUrl = new Uri(reportServer);
                rptInformeAutomatico.ServerReport.ReportPath = "/Formatoinforme/ReporteInforme";

                rptInformeAutomatico.ShowPrintButton = true;
                rptInformeAutomatico.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[16];
                param[0] = new ReportParameter("IdPlantilla", IdPlantilla);
                param[1] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[2] = new ReportParameter("ID_QUEU", "0");
                param[3] = new ReportParameter("ID_COMP", idCliente.ToString());
                param[4] = new ReportParameter("ID_COMP_END", idClienteFinal.ToString());
                param[5] = new ReportParameter("FechaInicio", Convert.ToString(FechaInicio));
                param[6] = new ReportParameter("FechaFin", Convert.ToString(FechaFin));

                param[7] = new ReportParameter("IdAreaResponsable", Convert.ToString(IdAreaResponsable));
                param[8] = new ReportParameter("IdAsignadoA", Convert.ToString(IdAsignadoA));
                param[9] = new ReportParameter("IdCategoria", Convert.ToString(IdCategoria));
                param[10] = new ReportParameter("IdSubCategoria", Convert.ToString(IdSubCategoria));
                param[11] = new ReportParameter("IdClase", Convert.ToString(IdClase));
                param[12] = new ReportParameter("IdSubClase", Convert.ToString(IdSubClase));
                param[13] = new ReportParameter("IdTipoTicket", Convert.ToString(IdTipoTicket));
                param[14] = new ReportParameter("SubCuenta", PSubCuenta);
                param[15] = new ReportParameter("MostrarFirma", Convert.ToString(MostrarFirma));

                //#if DEBUG
                //                ReportViewerCredentials rvc2 = new ReportViewerCredentials("Administrator", "C4goldP3", "");

                //                rptInformeAutomatico.ServerReport.ReportServerCredentials = rvc2;
                //#endif

                rptInformeAutomatico.ServerReport.SetParameters(param);
                rptInformeAutomatico.ServerReport.Refresh();
            }
        }
    }
}