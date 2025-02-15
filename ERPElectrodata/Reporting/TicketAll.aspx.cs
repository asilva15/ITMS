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

namespace ERPElectrodata.Reporting
{
    public partial class TicketAll : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                CoreEntities dbc = new CoreEntities();

                //Establecer como parámetro la cuenta actual.
                int ID_ACCOi = Convert.ToInt32(Session["ID_ACCO"]);
                int IdArea = 0;
                string ID_ACCO = null, FROM_DATE = null, TO_DATE, Estados;
                DateTime min_date;
                try
                {
                    //Obtenemos la fecha del primer ticket perteneciente a la cuenta.
                    min_date = Convert.ToDateTime(dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCOi).OrderBy(t => t.FEC_TICK).FirstOrDefault().FEC_TICK);
                }
                catch
                {
                    min_date = DateTime.MinValue;
                }

                ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                
                try
                {
                    FROM_DATE = Convert.ToString(Request.Params["FROM_DATE"]);
                    TO_DATE = Convert.ToString(Request.Params["TO_DATE"]);
                }
                catch
                {
                    FROM_DATE = Convert.ToString(min_date);
                }

                try
                {
                    TO_DATE = Convert.ToString(Request.Params["TO_DATE"]);
                }
                catch
                {
                    TO_DATE = Convert.ToString(DateTime.Now);
                }

                try
                {
                    IdArea = Convert.ToInt32(Request.Params["IdAreaOperativa"]);
                }
                catch
                {
                    IdArea = 0;
                }

                if (String.IsNullOrEmpty(FROM_DATE))
                {
                    FROM_DATE = Convert.ToString(min_date);
                }

                if (String.IsNullOrEmpty(TO_DATE))
                {
                    TO_DATE = Convert.ToString(DateTime.Now);
                }
                int SubCuenta = 0;

                try
                {
                    SubCuenta = Convert.ToInt32(Request.Params["rbSubCuenta"]);
                }
                catch
                {
                    SubCuenta = 0;
                }

                try
                {
                    Estados = Request.Params["ID_ESTADO_MOD"].ToString();
                }
                catch
                {
                    Estados = "";
                }

                string PSubCuenta = Convert.ToString(SubCuenta);
                //Obtenemos dirección de Servidor de reportes
                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                rvTicketAll.ServerReport.ReportServerCredentials = rvc;
                //Obtenemos el reporte generado en diseño y configuramos sus propiedades
                rvTicketAll.ProcessingMode = ProcessingMode.Remote;
                rvTicketAll.ServerReport.ReportServerUrl = new Uri(reportServer);
                //Dirección url del reporte a ejecutar.

                if (ID_ACCOi == 56 || ID_ACCOi ==57 || ID_ACCOi == 58)
                {
                    rvTicketAll.ServerReport.ReportPath = "/TicketReportMinsur";
                }
                else
                {
                    if (ID_ACCOi == 60)
                    {
                        rvTicketAll.ServerReport.ReportPath = "/TicketReportBuenaventura";
                    }
                    else
                    {
                        rvTicketAll.ServerReport.ReportPath = "/TicketReport3";
                    }
                }

                rvTicketAll.ShowPrintButton = true;
                rvTicketAll.ShowParameterPrompts = false;

                //Establecemos lo parámetros de servidor de reportes.
                ReportParameter[] param = new ReportParameter[6];
                param[0] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[1] = new ReportParameter("FROM_DATE", FROM_DATE);
                param[2] = new ReportParameter("TO_DATE", TO_DATE);
                param[3] = new ReportParameter("SubCuenta", PSubCuenta);
                param[4] = new ReportParameter("IdArea", IdArea.ToString());
                param[5] = new ReportParameter("Estados", Estados);

                rvTicketAll.ServerReport.SetParameters(param);

                //Actualizamos Servidor de reportes para mostrar datos actualizados.
                rvTicketAll.ServerReport.Refresh();
            }
        }
    }
}