using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Reporting.Activo
{
    public partial class ActivoTrazaGral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //string ID_ASSE = null;
                string ID_ACCO = null;
                string ID_PERS_ENTI_CREA = null, FechaInicio = null, FechaFin = null;
                try
                {
                    //ID_ASSE = Convert.ToString(Request.Params["id"]);
                    ID_ACCO = Convert.ToString(Session["ID_ACCO"]);
                    
                    if (Convert.ToString(Request.Params["ID_PERS_ENTI_CREA"]) != "")
                    {
                        ID_PERS_ENTI_CREA = Convert.ToString(Request.Params["ID_PERS_ENTI_CREA"]);
                    }
                    else
                    {
                        ID_PERS_ENTI_CREA = "0";
                    }
                    if (Convert.ToString(Request.Params["FechaInicio"]) != "")
                    {
                        FechaInicio = Convert.ToString(Request.Params["FechaInicio"]);
                    }
                    else
                    {
                        FechaInicio = null;
                    }
                    if (Convert.ToString(Request.Params["FechaFin"]) != "")
                    {
                        FechaFin = Convert.ToString(Request.Params["FechaFin"]);
                    }
                    else
                    {
                        FechaFin = null;
                    }

                }
                catch
                {
                    ID_ACCO = "0";
                    ID_PERS_ENTI_CREA = "0";
                    FechaInicio = null;
                    FechaFin = null;
                }

                var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                RVTrazabilidadGral.ServerReport.ReportServerCredentials = rvc;
                RVTrazabilidadGral.ProcessingMode = ProcessingMode.Remote;
                RVTrazabilidadGral.ServerReport.ReportServerUrl = new Uri(reportServer);
                RVTrazabilidadGral.ServerReport.ReportPath = "/Activo/TrazabilidadGral";

                RVTrazabilidadGral.ShowPrintButton = true;
                RVTrazabilidadGral.ShowParameterPrompts = false;

                ReportParameter[] param = new ReportParameter[4];
                param[0] = new ReportParameter("ID_ACCO", ID_ACCO);
                param[1] = new ReportParameter("ID_PERS_ENTI_CREA", ID_PERS_ENTI_CREA);
                param[2] = new ReportParameter("FechaInicio", FechaInicio);
                param[3] = new ReportParameter("FechaFin", FechaFin);

                RVTrazabilidadGral.ServerReport.SetParameters(param);

                RVTrazabilidadGral.ServerReport.Refresh();
            }

        }
    }
}