<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RptTransferenciaActivos_CambioGuardia.aspx.cs" Inherits="ERPElectrodata.Reporting.RptTransferenciaActivos_CambioGuardia" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <!--Objeto JavaScript para controlar peticiones-->
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="360000"></asp:ScriptManager>
            <!--Objeto Reporting Services para observar reporte-->
            <rsweb:ReportViewer ID="rptTransferenciaActivos" Width="100%" runat="server" SizeToReportContent="True"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>

