<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportReferencia.aspx.cs" Inherits="ERPElectrodata.Reporting.Talent.ReportReferencia" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="height: 629px; width: 982px">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="rptReferencias" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="605px" ProcessingMode="Remote" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="978px">
                <ServerReport ReportPath="/TalentoHumano/ReferenciasCandidatos" ReportServerUrl="http://10.10.131.15/reportserver" />
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
