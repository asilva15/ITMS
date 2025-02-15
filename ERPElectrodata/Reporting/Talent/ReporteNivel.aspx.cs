using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Reporting.Talent
{
    public partial class ReporteNivel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string textoLibre = null;
                    string gradoInstruccion = null;
                    string Profesion = null;
                    string IdGerencia = null;
                    string IdArea = null;
                    try
                    {
                        textoLibre = Convert.ToString(Request.Params["texto"]);

                        if (String.IsNullOrEmpty(Convert.ToString(Request.Params["gradoInstr"]).ToString()))
                        {
                            gradoInstruccion = "0";
                        }
                        else
                        {
                            gradoInstruccion = Convert.ToString(Request.Params["gradoInstr"]);
                        }

                        if (String.IsNullOrEmpty(Convert.ToString(Request.Params["Prof"]).ToString()))
                        {
                            Profesion = "0";
                        }
                        else
                        {
                            Profesion = Convert.ToString(Request.Params["Prof"]);
                        }
                        if (String.IsNullOrEmpty(Convert.ToString(Request.Params["Ger"]).ToString()))
                        {
                            IdGerencia = "0";
                        }
                        else
                        {
                            IdGerencia = Convert.ToString(Request.Params["Ger"]);
                        }
                        if (String.IsNullOrEmpty(Convert.ToString(Request.Params["Area"]).ToString()))
                        {
                            IdArea = "0";
                        }
                        else
                        {
                            IdArea = Convert.ToString(Request.Params["Area"]);
                        }

                    }
                    catch
                    {
                        textoLibre = "";
                        gradoInstruccion = "0";
                        Profesion = "0";
                        IdGerencia = "0";
                        IdArea = "0";
                    }

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptRRHHNivel.ServerReport.ReportServerCredentials = rvc;
                    rptRRHHNivel.ProcessingMode = ProcessingMode.Remote;
                    rptRRHHNivel.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptRRHHNivel.ServerReport.ReportPath = "/TalentoHumano/ReporteColaboradoresNivel";

                    rptRRHHNivel.ShowPrintButton = true;
                    rptRRHHNivel.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[5];
                    param[0] = new ReportParameter("Texto", textoLibre);
                    param[1] = new ReportParameter("IdProfesion", Profesion);
                    param[2] = new ReportParameter("IdGradoInstruccion", gradoInstruccion);
                    param[3] = new ReportParameter("IdGerencia", IdGerencia);
                    param[4] = new ReportParameter("IdArea", IdArea);

                    rptRRHHNivel.ServerReport.SetParameters(param);

                    rptRRHHNivel.ServerReport.Refresh();
                }
            }
            catch
            {

            }
        }
    }
}