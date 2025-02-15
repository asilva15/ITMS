<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteRecursosTec.aspx.cs" Inherits="ERPElectrodata.Reporting.Talent.ReporteRecursosTec" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="rtpRecursosTec" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="562px" ProcessingMode="Remote" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="888px">
                <ServerReport ReportPath="/TalentoHumano/ReporteRecursosTec" ReportServerUrl="http://10.10.131.15/reportserver" />
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
