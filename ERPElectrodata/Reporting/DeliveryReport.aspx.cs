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
    public partial class DeliveryReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID_TICK = Convert.ToString(Request.Params["id"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                int ID_TYPE_ASSE = Convert.ToInt32(Request.Params["ID_TYPE_ASSE"]);

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rptFormDelivery.ServerReport.ReportServerCredentials = rvc;
                rptFormDelivery.ProcessingMode = ProcessingMode.Remote;
                rptFormDelivery.ServerReport.ReportServerUrl = new Uri(reportServer);

                if (ID_ACCO == 4) {
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryED";
                }
                else if(ID_ACCO == 3){
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryHB";
                }
                else if (ID_ACCO == 55) // Hudbay V1
                {
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryHB_V1";
                }
                else if (ID_ACCO == 31)
                {
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryTR";
                }
                else if (ID_ACCO == 19)
                {
                    //rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryBR";
                    switch (ID_TYPE_ASSE)
                    {
                        case 2:
                            rptFormDelivery.ServerReport.ReportPath = "/ReporteReceptionBRCel";
                            break;
                        case 14:
                            rptFormDelivery.ServerReport.ReportPath = "/ReporteReceptionBRRadio";
                            break;
                        default:
                            rptFormDelivery.ServerReport.ReportPath = "/ReporteReceptionBREquipos";
                            break;
                    }
                }
                else if (ID_ACCO == 56)
                {
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryMS";
                }
                else if (ID_ACCO == 57)
                {
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryMM";
                }
                else if (ID_ACCO == 58)
                {
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryMR";
                }
                else if (ID_ACCO == 60)
                {
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDeliveryBNV";
                }
                else
                {
                    rptFormDelivery.ServerReport.ReportPath = "/ReporteDelivery";                
                }

                rptFormDelivery.ShowPrintButton = true;
                rptFormDelivery.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[1];
                param[0] = new ReportParameter("ID_TICK", ID_TICK);

                rptFormDelivery.ServerReport.SetParameters(param);
                rptFormDelivery.ServerReport.Refresh();
            }
        }
    }
}