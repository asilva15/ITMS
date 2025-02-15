<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteRoles.aspx.cs" Inherits="ERPElectrodata.Reporting.Ticket.ReporteRoles" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
        </div>
        <rsweb:ReportViewer ID="RVRoles" runat="server" Height="550px" Width="100%" >
        </rsweb:ReportViewer>
    </form>
</body>
</html>
