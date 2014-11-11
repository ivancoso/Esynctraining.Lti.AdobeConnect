﻿namespace EdugameCloud.WCFService
{
    using System;
    using System.Web;
    using System.Web.Configuration;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Core.Keys;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Converters;
    using EdugameCloud.Persistence;
    using EdugameCloud.WCFService.Providers;

    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using Weborb.Messaging;

    /// <summary>
    /// The global.
    /// </summary>
    public class Global : HttpApplication
    {
        #region Methods

        /// <summary>
        /// The application_ end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Application_End(object sender, EventArgs e)
        {
            IoC.Container.Dispose();
            if (WebConfigurationManager.AppSettings.HasKey("RTMPServerPort"))
            {
                var server = Application[ApplicationKeys.WebOrbRTMPServerKey] as RTMPServer;
                if (server != null)
                {
                    server.shutdown();
                }
            }
        }

        /// <summary>
        /// The application_ start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Application_Start(object sender, EventArgs e)
        {
            IoC.Initialize(new WindsorContainer());
            IoC.Container.RegisterComponents(wcf: true);
            IoC.Container.Register(Component.For<IResourceProvider>().ImplementedBy<WcfResourceProvider>().Activator<ResourceProviderActivator>());
            IoC.Container.Register(Component.For<CourseAPI>().ImplementedBy<CourseAPI>());
            IoC.Container.Register(Component.For<QuizConverter>().ImplementedBy<QuizConverter>());
            IoC.Container.Register(Component.For<QuizResultConverter>().ImplementedBy<QuizResultConverter>());
            IoC.Container.Register(Component.For<DlapAPI>().ImplementedBy<DlapAPI>());
            IoC.Container.Register(Component.For<SoapAPI>().ImplementedBy<SoapAPI>());
            try
            {
                // Initialize WebORB configuration before starting messaging server
                var config = Weborb.Config.ORBConfig.GetInstance();

                // Create Messaging server. 2037 is the port number, 500 is connection backlog
                var name = typeof(DBChangesNotifier).Name;
                if (WebConfigurationManager.AppSettings.HasKey("RTMPServerPort"))
                {
                    var port = int.Parse(WebConfigurationManager.AppSettings["RTMPServerPort"]);

                    var server = new RTMPServer(name, port, 500, config);
                    // Start the messaging server
                    server.start();

                    // Store the server instance in the Application context, so it can be cleared out when application stops     
                    Application[ApplicationKeys.WebOrbRTMPServerKey] = server;
                }
            }
            catch (Exception ex)
            {
                var logger = IoC.Resolve<Castle.Core.Logging.ILogger>();
                logger.Error("Failed to initialize RTMP server", ex);
            }  
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            Weborb.Util.ThreadContext.setCurrentHttpContext(HttpContext.Current);
            Weborb.Security.ORBSecurity.AuthenticateRequest();
        }

        #endregion
    }
}