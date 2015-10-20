namespace EdugameCloud.SocialSubscriptionHandler
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;

    using Esynctraining.Core.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Persistence;
    using Esynctraining.CastleLog4Net;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using NHibernate;

    using Configuration = NHibernate.Cfg.Configuration;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// The clean notes.
        /// </summary>
        /// <param name="connectionString">
        /// The connection String.
        /// </param>
        /// <param name="subscriptionId">
        /// The subscription Id.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        private static int FullClean(string connectionString, int subscriptionId, ILogger logger)
        {
            const string DeleteCommand = "delete from [EduGameCloud].[dbo].[SubscriptionUpdate] where [subscription_id] = @subscriptionId";
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();
                        try
                        {
                            using (var cmd = new SqlCommand(DeleteCommand, con))
                            {
                                cmd.Parameters.AddWithValue("@subscriptionId", subscriptionId);
                                var deleted = cmd.ExecuteNonQuery();
                                return deleted;
                            }
                        }
                        // ReSharper disable once EmptyGeneralCatchClause
                        catch (Exception ex)
                        {
                            try
                            {
                                logger.Error("Social subscription delete. Failed to delete: " + subscriptionId, ex);
                            }
                            // ReSharper disable once EmptyGeneralCatchClause
                            catch
                            {
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Social subscription delete.", ex);
            }
            return 0;
        }

        /// <summary>
        ///     The initialize container.
        /// </summary>
        private static void InitializeContainer()
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);

            container.Register(Component.For<FluentConfiguration>().LifeStyle.Singleton);
            container.Register(
                Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(
                Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<NHibernateSessionFactoryActivator>());
            container.Register(
                Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient);
            container.Register(
                Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);
            container.Register(
                Component.For<ApplicationSettingsProvider>()
                    .ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", null))
                    .LifeStyle.Singleton);

            container.Register(
                Classes.FromAssemblyNamed("EdugameCloud.Core")
                    .Pick()
                    .If(Component.IsInNamespace("EdugameCloud.Core.Business.Models"))
                    .WithService.Self()
                    .Configure(c => c.LifestyleTransient()));

            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
        }

        /// <summary>
        /// The log message.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private static void LogMessage(ILogger logger, string message)
        {
            try
            {
                Console.WriteLine(message);
                logger.Error(message);
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The main.
        /// </summary>
        private static void Main()
        {
            InitializeContainer();
            try
            {
                dynamic settings = IoC.Resolve<ApplicationSettingsProvider>();
                int hoursWithoutQuery = int.Parse(settings.HoursWithoutQuery) * -1;
                var callbackUrl = (string)settings.CallbackUrl;
                var logger = IoC.Resolve<ILogger>();
                var model = IoC.Resolve<SubscriptionHistoryLogModel>();
                var historyLog = model.GetAll().ToList();
                var webProxy = IoC.Resolve<WebProxyModel>();
                var subscriptions = webProxy.ListSubscriptions();
                if (subscriptions.data != null)
                {
                    LogMessage(logger, "Found " + subscriptions.data.Length + " subscriptions");
                    foreach (var s in subscriptions.data)
                    {
                        if (s.callback_url.Equals(callbackUrl, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var inactiveHistoryLogItem =
                                historyLog.FirstOrDefault(x => (x.SubscriptionId == s.id || x.SubscriptionTag.Equals(s.object_id, StringComparison.InvariantCultureIgnoreCase))
                                    && (!x.LastQueryTime.HasValue || x.LastQueryTime.Value <= DateTime.Now.AddHours(hoursWithoutQuery)));
                            if (inactiveHistoryLogItem != null)
                            {
                                var result = webProxy.DeleteInstagramSubscription(s.id);
                                if (result.meta.With(x => x.code == 200))
                                {
                                    var deleted = FullClean((string)settings.ConnectionString, s.id, logger);
                                    model.RegisterDelete(inactiveHistoryLogItem, true);
                                    LogMessage(logger, "Subscription for tag " + s.object_id + " suspended; items deleted count = " + deleted);
                                }
                            }
                            else
                            {
                                LogMessage(logger, "Subscription for tag " + s.object_id + " is still active");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("Error occured", ex);
            }
        }

        #endregion
    }
}