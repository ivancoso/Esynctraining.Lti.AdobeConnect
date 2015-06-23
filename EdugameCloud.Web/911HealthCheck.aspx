<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="911HealthCheck.aspx.cs" Inherits="EdugameCloud.Web._911HealthCheck" %>
<%@ OutputCache NoStore="true" Location="None" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Health Check Page</title>
</head>
<style type="text/css">
	body {
		font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif;
	}

	.inprogress, .pass, .fail, .warning {
		padding-left: 35px;
		display: block;
		min-height: 35px;
		font-size: 120%;
		vertical-align: middle;
	}

	.inprogress {
		background: transparent url("content/images/ajax-loader.gif") no-repeat;
	}

	.pass {
		background: transparent url("content/images/tick.png") no-repeat;
	}

	.fail {
		background: transparent url("content/images/cross.png") no-repeat;
	}

	.warning {
		background: transparent url("content/images/warning.png") no-repeat;
	}

	.error {
		color: Red;
	}

	.suggestion {
		color: darkred;
		font-style: italic;
		font-size: 120%;
	}

	ul {
		list-style: none;
	}
</style>
<body>
    <form id="aForm" runat="server">
    <asp:ScriptManager ID="scriptManager" runat="server" EnablePartialRendering="true" ScriptMode="Release">
    </asp:ScriptManager>
    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Timer runat="server" ID="RefreshTimer" Interval="500" Enabled="true" OnTick="RefreshTimer_Tick" />
            <h1>Self Diagnostics</h1>
            <h2>Database Configuration</h2>
            <ul>
                <li><asp:Label CssClass="inprogress" Text="Can access database using the connection string." runat="server" ID="ConnectionStringStatusLabel" /></li>
            </ul>
            <h2>ASP.NET Configuration</h2>
            <ul>
                <li><asp:Label CssClass="inprogress" ID="AppDataLabel" Text="Write permission on '/Logs' folder." runat="server" /></li>
            </ul>
            <h2>Web.config appSettings paths and urls</h2>
            <ul>
                <li><asp:Label CssClass="inprogress" Text="All File paths readable" ID="FilePathLabel" runat="server" /></li>
                <li><asp:Label CssClass="inprogress" Text="All URLs reachable" ID="URLReachableLabel" runat="server" /></li>
				<li><asp:Label CssClass="inprogress" Text="Services: All File paths readable" ID="ServicesFilePathLabel" runat="server" /></li>
				<li><asp:Label CssClass="inprogress" Text="Services: All URLs reachable" ID="ServicesURLReachableLabel" runat="server" /></li>
            </ul>
            <h2>Mail Delivery</h2>
            <ul>
                <li><asp:Label CssClass="inprogress" Text="Check SMTP settings are valid to send email. (We send a test email from a server)" runat="server" ID="SMTPLabel" /></li>
            </ul>
            <h2>Application Settings</h2>
            <ul>
                <li><asp:Label CssClass="inprogress" Text="FileStorage folder should exist and be available." ID="FileStorageLabel" runat="server" /></li>
                <li><asp:Label CssClass="inprogress" Text="BasePath matches with the address of this site." ID="BasePathLabel" runat="server" /></li>
				<li><asp:Label CssClass="inprogress" Text="PortalUrl matches with the address of this site." ID="PortalUrlLabel" runat="server" /></li>

				<li><asp:Label CssClass="inprogress" Text="Services: TrialContactEmail is valid Email Address." ID="TrialContactEmailLabel" runat="server" /></li>
				<li><asp:Label CssClass="inprogress" Text="Services: BCCActivationEmail contains only valid Email Addresses." ID="BCCActivationEmailLabel" runat="server" /></li>
				<li><asp:Label CssClass="inprogress" Text="Services: BCCNewEmail is valid Email Address." ID="BCCNewEmailLabel" runat="server" /></li>
            </ul>
            <h2>AMF Services Settings</h2>
            <ul>
				<li><asp:Label CssClass="inprogress" Text="'gateway' setting should be valid URL." ID="GatewayUrlLabel" runat="server" /></li>
                <li><asp:Label CssClass="inprogress" Text="AmfBinding should have valid http\https transport setting." ID="AmfBindingLabel" runat="server" /></li>
				<li><asp:Label CssClass="inprogress" Text="Services/UserService.svc is accessible." ID="SecurityServiceLabel" runat="server" /></li>

            </ul>
        </ContentTemplate>
    </asp:UpdatePanel>
</form>
</body>
</html>