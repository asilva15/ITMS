﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Atenciones.aspx.cs" Inherits="ERPElectrodata.Reporting.Ticket.Atenciones" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="rptAtenciones" runat="server" SizeToReportContent="True"></rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>

