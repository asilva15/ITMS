using ERPElectrodata.Object.Plugins;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Reporting.Ticket
{
    public partial class FormatoInformesAuto : System.Web.UI.Page
    {
        public CoreEntities dbc = new CoreEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idBotoninformeSolucion = Convert.ToString(Request.Params["linkInformeSolucion"]);
                string idBotoninforme = Convert.ToString(Request.Params["linkInformeAtencion"]);
                string idBotonAtencion = Convert.ToString(Request.Params["linkInformeTabla"]);

                string idComp = Convert.ToString(Request.Params["idComp"]);
                string idCompEnd = Convert.ToString(Request.Params["idCompEnd"]);
                string idOP = Convert.ToString(Request.Params["idOP"]);
                DateTime fechaCreacion = Convert.ToDateTime(Request.QueryString["fechaInicio"]);
                DateTime fechaFin = Convert.ToDateTime(Request.QueryString["fechaFin"]);

                //CATEGORIAS

                string cbCategoria = Convert.ToString(Request.Params["cbCategoria"]);
                string cbSubCategoria = Convert.ToString(Request.Params["cbSubCategoria"]);
                string cbClase = Convert.ToString(Request.Params["cbClase"]);
                string cbSubClase = Convert.ToString(Request.Params["cbSubClase"]);
                int IdComp = 0, IdCompEnd = 0, IdcategoN1 = 0, IdcategoN2 = 0, IdcategoN3 = 0, IdcategoN4 = 0;
                int IdMarca = 0, IdOP = 0;


                //CONVIRTIENDO VALORES



                if (int.TryParse(Convert.ToString(Request.Params["idComp"]), out IdComp) == false)
                {
                    IdComp = 0;
                }

                if (int.TryParse(Convert.ToString(Request.Params["idCompEnd"]), out IdCompEnd) == false)
                {
                    IdCompEnd = 0;
                }

                //if (int.TryParse(Convert.ToString(Request.Params["idMarca"]), out IdMarca) == false)
                //{
                //    IdMarca = 0;
                //}
                if (int.TryParse(Convert.ToString(Request.Params["idOP"]), out IdOP) == false)
                {
                    IdOP = 0;
                }


                //---------CATEGORIAS
                if (int.TryParse(Convert.ToString(Request.Params["cbCategoria"]), out IdcategoN1) == false)
                {
                    IdcategoN1 = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["cbSubCategoria"]), out IdcategoN2) == false)
                {
                    IdcategoN2 = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["cbClase"]), out IdcategoN3) == false)
                {
                    IdcategoN3 = 0;
                }
                if (int.TryParse(Convert.ToString(Request.Params["cbSubClase"]), out IdcategoN4) == false)
                {
                    IdcategoN4 = 0;
                }




                //OPTENIENDO EL VALOR PARA LOS BOTONES
                if (!string.IsNullOrEmpty(idBotoninformeSolucion))
                {

                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptFormatoInformesAuto.ServerReport.ReportServerCredentials = rvc;
                    rptFormatoInformesAuto.ProcessingMode = ProcessingMode.Remote;
                    rptFormatoInformesAuto.ServerReport.ReportServerUrl = new Uri(reportServer);


                    rptFormatoInformesAuto.ServerReport.ReportPath = "/FormatoAutomatico";

                    rptFormatoInformesAuto.ShowPrintButton = true;
                    rptFormatoInformesAuto.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[9];
                    param[0] = new ReportParameter("IdComp", idComp);
                    param[1] = new ReportParameter("IdCompEnd", Convert.ToString(IdCompEnd));
                    //param[2] = new ReportParameter("IdMarca", idMarca);
                    //param[2] = new ReportParameter("FechaCreacion", fechaCreacion.ToString());
                    //param[3] = new ReportParameter("FechaFin", fechaFin.ToString());

                    param[2] = new ReportParameter("FechaCreacion", Convert.ToString(fechaCreacion));
                    param[3] = new ReportParameter("FechaFin", Convert.ToString(fechaFin));
                    param[4] = new ReportParameter("IdOP", Convert.ToString(IdOP));
                    //CATEGORIAS

                    param[5] = new ReportParameter("CategoriaN1", Convert.ToString(IdcategoN1));
                    param[6] = new ReportParameter("CategoriaN2", Convert.ToString(IdcategoN2));
                    param[7] = new ReportParameter("CategoriaN3", Convert.ToString(IdcategoN2));
                    param[8] = new ReportParameter("CategoriaN4", Convert.ToString(IdcategoN2));



                    //Response.Redirect("/#/CrearFormato/");
                    rptFormatoInformesAuto.ServerReport.SetParameters(param);
                    rptFormatoInformesAuto.ServerReport.Refresh();

                }
                if (!string.IsNullOrEmpty(idBotoninforme))
                {


                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptFormatoInformesAuto.ServerReport.ReportServerCredentials = rvc;
                    rptFormatoInformesAuto.ProcessingMode = ProcessingMode.Remote;
                    rptFormatoInformesAuto.ServerReport.ReportServerUrl = new Uri(reportServer);

                    rptFormatoInformesAuto.ServerReport.ReportPath = "/Formatoinforme/AtencionRequerimeinto";
                    //rptFormatoInformesAuto.ServerReport.ReportPath = "/Formatoinforme/AtencionRequerimeinto";

                    rptFormatoInformesAuto.ShowPrintButton = true;
                    rptFormatoInformesAuto.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[9];
                    param[0] = new ReportParameter("IdComp", idComp);
                    param[1] = new ReportParameter("IdCompEnd", Convert.ToString(IdCompEnd));
                    //param[2] = new ReportParameter("IdMarca", idMarca);
                    //param[2] = new ReportParameter("FechaCreacion", fechaCreacion.ToString());
                    //param[3] = new ReportParameter("FechaFin", fechaFin.ToString());

                    param[2] = new ReportParameter("FechaCreacion", Convert.ToString(fechaCreacion));
                    param[3] = new ReportParameter("FechaFin", Convert.ToString(fechaFin));
                    param[4] = new ReportParameter("IdOP", Convert.ToString(IdOP));
                    //CATEGORIAS

                    param[5] = new ReportParameter("CategoriaN1", Convert.ToString(IdcategoN1));
                    param[6] = new ReportParameter("CategoriaN2", Convert.ToString(IdcategoN2));
                    param[7] = new ReportParameter("CategoriaN3", Convert.ToString(IdcategoN2));
                    param[8] = new ReportParameter("CategoriaN4", Convert.ToString(IdcategoN2));



                    //Response.Redirect("/#/CrearFormato/");
                    rptFormatoInformesAuto.ServerReport.SetParameters(param);
                    rptFormatoInformesAuto.ServerReport.Refresh();

                }
                if (!string.IsNullOrEmpty(idBotonAtencion))
                {


                    var reportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
                    var reportServerUser = ConfigurationManager.AppSettings["ReportServerUser"].ToString();
                    var reportServerPass = ConfigurationManager.AppSettings["ReportServerPass"].ToString();
                    IReportServerCredentials rvc = new CredencialesReporting(reportServerUser, reportServerPass, "");
                    rptFormatoInformesAuto.ServerReport.ReportServerCredentials = rvc;
                    rptFormatoInformesAuto.ProcessingMode = ProcessingMode.Remote;
                    rptFormatoInformesAuto.ServerReport.ReportServerUrl = new Uri(reportServer);
                    rptFormatoInformesAuto.ServerReport.ReportPath = "/TablaInformacionTabla";
                    rptFormatoInformesAuto.ShowPrintButton = true;
                    rptFormatoInformesAuto.ShowParameterPrompts = false;

                    ReportParameter[] param = new ReportParameter[9];
                    param[0] = new ReportParameter("IdComp", idComp);
                    param[1] = new ReportParameter("IdCompEnd", Convert.ToString(IdCompEnd));
                    //param[2] = new ReportParameter("IdMarca", idMarca);
                    //param[2] = new ReportParameter("FechaCreacion", fechaCreacion.ToString());
                    //param[3] = new ReportParameter("FechaFin", fechaFin.ToString());

                    param[2] = new ReportParameter("FechaCreacion", Convert.ToString(fechaCreacion));
                    param[3] = new ReportParameter("FechaFin", Convert.ToString(fechaFin));
                    param[4] = new ReportParameter("IdOP", Convert.ToString(IdOP));
                    //CATEGORIAS

                    param[5] = new ReportParameter("CategoriaN1", Convert.ToString(IdcategoN1));
                    param[6] = new ReportParameter("CategoriaN2", Convert.ToString(IdcategoN2));
                    param[7] = new ReportParameter("CategoriaN3", Convert.ToString(IdcategoN2));
                    param[8] = new ReportParameter("CategoriaN4", Convert.ToString(IdcategoN2));



                    //Response.Redirect("/#/CrearFormato/");
                    rptFormatoInformesAuto.ServerReport.SetParameters(param);
                    rptFormatoInformesAuto.ServerReport.Refresh();

                    //rptFormatoInformesAuto.ServerReport.SetParameters(param);
                    //rptFormatoInformesAuto.ServerReport.Refresh();
                }






            }
        }
    }
}