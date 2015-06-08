using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EdugameCloud.Web
{
    // TODO: test EGC Admin App login.
    // <add key="XSDProfileLocation" value="d:\Freelance\eSyncTraining\EdugameCloud\trunk\csharp\EdugameCloud.WCFService\EdugameCloud.Web\Content\xsd\vcfProfile.xsd" />
    public partial class _911HealthCheck : System.Web.UI.Page
    {
        private int CurrentTaskNo
        {
            get
            {
                return int.Parse(ViewState["CurrentTaskNo"] as string ?? "0");
            }
            set
            {
                ViewState["CurrentTaskNo"] = value.ToString();
            }
        }

        private Action[] _Tasks;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this._Tasks = new Action[] 
            {
                () => TestConnectionString(),
                () => TestWrite(),
                () => TestPaths(),
                () => TestServicesPaths(),
                () => TestUrls(),
                () => TestServicesUrls(),
                () => TestSMTP(),
                () => TestAppSettings(),
                () => TestGatewayUrl(),
                () => TestAmfBinding(),
                () => TestSecurityService(),
            };
        }

        #region Framework

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.CurrentTaskNo = 0;
            }
        }


        private void MarkAsFail(Label label, string reason, string suggestion)
        {
            label.CssClass = "fail";
            label.Text += "<br /><span class=\"error\">" + HttpUtility.HtmlEncode(reason) + "</span>" +
                "<br /><span class=\"suggestion\">" + HttpUtility.HtmlEncode(suggestion) + "</span>";
        }

        private void MarkAsWarning(Label label, string suggestion)
        {
            label.CssClass = "warning";
            label.Text += "<br /><span class=\"suggestion\">" + HttpUtility.HtmlEncode(suggestion) + "</span>";
        }

        private void MarkAsPass(Label label)
        {
            label.CssClass = "pass";
        }

        private void MarkAsPass(Label label, string message)
        {
            label.CssClass = "pass";
            label.Text += "<br /><span class=\"suggestion\">" + HttpUtility.HtmlEncode(message) + "</span>";
        }

        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            this._Tasks[this.CurrentTaskNo]();

            this.CurrentTaskNo++;
            if (this.CurrentTaskNo == this._Tasks.Length)
            {
                this.RefreshTimer.Enabled = false;
            }
        }

        #endregion

        #region Tasks

        private void TestConnectionString()
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Database"];
                if ((conn == null) || (string.IsNullOrWhiteSpace(conn.ConnectionString)))
                {
                    MarkAsFail(ConnectionStringStatusLabel,
                    "<connectionStrings><add name=\"Database\" connectionString=\"...\" /> Entry was not found",
                    "Check your <connectionstrings> section within Web.config file and Web.constrings file");
                    return;
                }

                var sw = Stopwatch.StartNew();
                using (var connection = new SqlConnection(conn.ConnectionString))
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT GETDATE()";
                        connection.Open();
                        var date = cmd.ExecuteScalar();
                    }
                }

                sw.Stop();
                var time = sw.Elapsed;
                //TODO: new DropthingsDataContext2().Test();

                MarkAsPass(ConnectionStringStatusLabel, string.Format(" It takes {0} to connect to DB and call 'SELECT GETDATE()' request.", time));
            }
            catch (Exception x)
            {
                MarkAsFail(ConnectionStringStatusLabel, x.Message,
                    "Either database startup timed out or most likely incorrect connection string or the connection string.");
            }
        }
        
        private void TestWrite()
        {
            try
            {
                File.AppendAllText(Server.MapPath("~/App_Data/" + Guid.NewGuid().ToString()), Guid.NewGuid().ToString());

                MarkAsPass(AppDataLabel);
            }
            catch (Exception x)
            {
                MarkAsFail(AppDataLabel, x.Message, "Give read, write, modify permission Your Application Pool account to App_Data folder.");
            }
        }

        private void TestPaths()
        {
            bool allPathOK = true;
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string value = ConfigurationManager.AppSettings[key];

                if (value.StartsWith("~/"))
                {
                    string fullPath = Server.MapPath(value);
                    if (!Directory.Exists(fullPath) && !File.Exists(fullPath))
                    {
                        MarkAsFail(FilePathLabel, "Invalid path: " + key + "=" + fullPath, string.Empty);
                        allPathOK = false;
                    }
                }
            }

            if (allPathOK)
                MarkAsPass(FilePathLabel);

        }

        private void TestUrls()
        {
            bool allUrlOK = true;

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string value = ConfigurationManager.AppSettings[key];
                Uri uri;
                if (Uri.TryCreate(value, UriKind.Absolute, out uri))
                {
                    // Got an URI, try hitting
                    using (var client = new WebClient())
                    {
                        try
                        {
                            client.DownloadString(uri);
                        }
                        catch (Exception x)
                        {
                            MarkAsFail(URLReachableLabel, x.Message, "Unreachable URL: " + key + "=" + uri.ToString());
                            allUrlOK = false;
                        }
                    }
                }
            }

            if (allUrlOK)
                MarkAsPass(URLReachableLabel);
        }

        private void TestSMTP()
        {
            var client = new SmtpClient();
            try
            {
                client.Send("sergeyi@esynctraining.com", "sergeyi@esynctraining.com", "EGC-LTI Test email", "Test body");
                MarkAsPass(SMTPLabel);
            }
            catch (Exception x)
            {
                MarkAsWarning(SMTPLabel, x.Message +
                    Environment.NewLine +
                    "Maybe you haven't turned on SMTP service or haven't configured Relay settings properly. You can still run application without it. But it won't be able to send emails.");
            }
        }

        private void TestAppSettings()
        {
            var appSettings = ConfigurationManager.AppSettings;

            string fileStorage = appSettings["FileStorage"];
            if (!Directory.Exists(fileStorage) && !File.Exists(fileStorage))
            {
                MarkAsFail(FileStorageLabel, "Invalid path: FileStorage=" + fileStorage, string.Empty);
            }
            else
                MarkAsPass(FileStorageLabel);

            string fullurl = Request.Url.ToString().ToLower();
            string baseUrl = fullurl.Substring(0, fullurl.IndexOf("911healthcheck.aspx")).TrimEnd('/');

            //if (ConstantHelper.DeveloperMode)
            //    MarkAsWarning(DeveloperModeLabel,
            //        "Developer mode turns off all caching and causes poor performance. It's made for developers to test changes without requiring to clear browser cache repeatedly.");
            //else
            //    MarkAsPass(DeveloperModeLabel);

            if (baseUrl != appSettings["BasePath"].TrimEnd('/'))
                MarkAsWarning(BasePathLabel, "BasePath does not match with the current URL. BasePath should be: " + baseUrl);
            else
                MarkAsPass(BasePathLabel);

            if (baseUrl != appSettings["PortalUrl"].TrimEnd('/'))
                MarkAsWarning(PortalUrlLabel, "PortalUrl does not match with the current URL. BasePath should be: " + baseUrl);
            else
                MarkAsPass(PortalUrlLabel);

            //TestPrefix(ConstantHelper.WebRoot, ConstantHelper.WebRoot.TrimEnd('/') + "/API/Proxy.svc/ajax/js", WebServiceProxyLabel,
            //    "Make sure you have installed WCF REST Starter kit Preview 2 and inside web.config <baseAddresses> inside each <service> node under <system.serviceModel> has " + ConstantHelper.WebRoot);

            KeyValueConfigurationCollection servicesAppSettings = GetServicesAppSettings();
            if (ContainsValidEmails(servicesAppSettings["TrialContactEmail"].Value))
                MarkAsPass(TrialContactEmailLabel);
            else
                MarkAsWarning(TrialContactEmailLabel, "TrialContactEmail is not valid Email Address. TrialContactEmail: " + servicesAppSettings["TrialContactEmail"].Value);

            if (ContainsValidEmails(servicesAppSettings["BCCActivationEmail"].Value))
                MarkAsPass(BCCActivationEmailLabel);
            else
                MarkAsWarning(BCCActivationEmailLabel, "BCCActivationEmail contains not valid Email Address. BCCActivationEmail: " + servicesAppSettings["BCCActivationEmail"].Value);

            if (ContainsValidEmails(servicesAppSettings["BCCNewEmail"].Value))
                MarkAsPass(BCCNewEmailLabel);
            else
                MarkAsWarning(BCCNewEmailLabel, "BCCNewEmail contains not valid Email Address. BCCNewEmail: " + servicesAppSettings["BCCNewEmail"].Value);

        }

        private void TestServicesPaths()
        {
            KeyValueConfigurationCollection appSettings = GetServicesAppSettings();
            
            bool allPathOK = true;
            foreach (string key in appSettings.AllKeys)
            {
                string value = appSettings[key].Value;

                if (value.StartsWith("~/"))
                {
                    string fullPath = Server.MapPath(value);
                    if (!Directory.Exists(fullPath) && !File.Exists(fullPath))
                    {
                        MarkAsFail(ServicesFilePathLabel, "Invalid path: " + key + "=" + fullPath, string.Empty);
                        allPathOK = false;
                    }
                }
            }

            if (allPathOK)
                MarkAsPass(ServicesFilePathLabel);

        }

        private void TestServicesUrls()
        {
            KeyValueConfigurationCollection appSettings = GetServicesAppSettings();

            bool allUrlOK = true;

            foreach (string key in appSettings.AllKeys)
            {
                string value = appSettings[key].Value;
                Uri uri;
                if (Uri.TryCreate(value, UriKind.Absolute, out uri))
                {
                    // Got an URI, try hitting
                    using (var client = new WebClient())
                    {
                        try
                        {
                            client.DownloadString(uri);
                        }
                        catch (Exception x)
                        {
                            MarkAsFail(ServicesURLReachableLabel, x.Message, "Unreachable URL: " + key + "=" + uri.ToString());
                            allUrlOK = false;
                        }
                    }
                }
            }

            if (allUrlOK)
                MarkAsPass(ServicesURLReachableLabel);
        }

        private void TestGatewayUrl()
        {
            try
            {
                string config = File.ReadAllText(Server.MapPath("~/Content/swf/config/paths.properties"));
                string[] lines = config.Split(new[] { '\r', '\n' });
                string gateway = lines.First(x => x.StartsWith("gateway")).Replace(" ", string.Empty).Replace("gateway", string.Empty).Replace("=", string.Empty);

                Uri uri;
                if (Uri.TryCreate(gateway, UriKind.Absolute, out uri))
                {
                    MarkAsPass(GatewayUrlLabel);
                }
                else
                {
                    MarkAsFail(GatewayUrlLabel, "gateway setting should be valid Absolute Url", @"Check 'gateway'settings (Content\swf\config\paths.properties file).");
                }                
            }
            catch (Exception ex)
            {
                MarkAsFail(GatewayUrlLabel, "EXCEPTION DURING TEST: " + ex.Message, null);
            }
        }

        private void TestAmfBinding()
        {
            try
            {
                Configuration servicesConfig = WebConfigurationManager.OpenWebConfiguration("~/services");
                ServiceModelSectionGroup serviceModel = ServiceModelSectionGroup.GetSectionGroup(servicesConfig);
                BindingsSection bindings = serviceModel.Bindings;

                CustomBindingElement amfBinding = bindings.CustomBinding.Bindings.Cast<CustomBindingElement>().FirstOrDefault(cb => cb.Name == "amfBinding");
                if (amfBinding == null)
                {
                    MarkAsFail(AmfBindingLabel, "Custom binding with name=\"amfBinding\" not found.", null);
                    return;
                }

                string config = File.ReadAllText(Server.MapPath("~/Content/swf/config/paths.properties"));
                bool sfwUsesHttps = config.Replace(" ", string.Empty).Contains("gateway=https:");
                var http = amfBinding.FirstOrDefault(x => x is HttpTransportElement);
                var https = amfBinding.FirstOrDefault(x => x is HttpsTransportElement);
                if (sfwUsesHttps)
                {
                    if (https == null)
                        MarkAsFail(AmfBindingLabel, "httpsTransport section not found", "Use <httpsTransport /> within <binding name=\"amfBinding\">  (services/web.config)");
                }
                else
                {
                    if (http == null)
                        MarkAsFail(AmfBindingLabel, "httpTransport section not found", "Use <httpTransport /> within <binding name=\"amfBinding\">.  (services/web.config)");
                }

                MarkAsPass(AmfBindingLabel);
            }
            catch (Exception ex)
            {
                MarkAsFail(AmfBindingLabel, "EXCEPTION DURING TEST: " + ex.Message, null);
            }
        }

        //http://dev.edugamecloud.com/services/UserService.svc
        private void TestSecurityService()
        {
            try
            {
                string config = File.ReadAllText(Server.MapPath("~/Content/swf/config/paths.properties"));
                string[] lines = config.Split(new[] { '\r', '\n' });
                string gateway = lines.First(x => x.StartsWith("gateway")).Replace(" ", string.Empty).Replace("gateway", string.Empty).Replace("=", string.Empty);

                var uri = new Uri(new Uri(gateway), @"UserService.svc");
                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadString(uri);
                    }
                    catch (Exception x)
                    {
                        MarkAsFail(SecurityServiceLabel, x.Message, "Unreachable URL: " + uri.ToString());
                    }
                }

                MarkAsPass(SecurityServiceLabel);
            }
            catch (Exception ex)
            {
                MarkAsFail(SecurityServiceLabel, "EXCEPTION DURING TEST: " + ex.Message, null);
            }
        }

        #endregion


        private static KeyValueConfigurationCollection GetServicesAppSettings()
        {
            Configuration servicesConfig = WebConfigurationManager.OpenWebConfiguration("~/services");
            return servicesConfig.AppSettings.Settings;
        }

        private static bool ContainsValidEmails(string value)
        {
            string[] emails = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (emails.Length == 0)
                return false;

            foreach (string email in emails)
            {
                try
                {
                    new MailAddress(email);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

    }

}