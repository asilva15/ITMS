<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="TicketByStatus.aspx.cs" Inherits="ERPElectrodata.Reporting.TicketByStatus" %>

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

            <rsweb:ReportViewer ID="rvTicketByStatus" Width="100%" SizeToReportContent="True" runat="server"></rsweb:ReportViewer>

        </div>
    </form>
</body>
</html>
