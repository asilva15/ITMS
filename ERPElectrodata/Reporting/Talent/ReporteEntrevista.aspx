<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteEntrevista.aspx.cs" Inherits="ERPElectrodata.Reporting.Talent.ReporteEntrevista" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

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
        <rsweb:ReportViewer ID="rptEntrevista" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="562px" ProcessingMode="Remote" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="888px">
            <ServerReport ReportPath="/TalentoHumano/ReporteEntrevista" ReportServerUrl="http://10.10.131.15/reportserver" />
        </rsweb:ReportViewer>
    
    </div>
    </form>
</body>
</html>
