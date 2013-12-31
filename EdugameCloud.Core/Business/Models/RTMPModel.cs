namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Configuration;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Converters;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Keys;
    using EdugameCloud.Core.RTMP;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Providers;

    using Newtonsoft.Json;

    using Weborb.Config;
    using Weborb.Messaging;
    using Weborb.Messaging.Net.RTMP;

    /// <summary>
    ///     The RTMP model.
    /// </summary>
    public class RTMPModel : IDisposable
    {
        #region Static Fields

        /// <summary>
        /// The _locker.
        /// </summary>
        private static readonly object _locker = new object();

        #endregion

        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The RTMP client.
        /// </summary>
        private readonly RTMPClient rtmpClient;

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RTMPModel"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public RTMPModel(ILogger logger, ApplicationSettingsProvider settings)
        {
            this.logger = logger;
            this.settings = settings;
            this.CheckIfServerIsRunning();
            this.rtmpClient = new RTMPClient();
            this.rtmpClient.ConnectFailedEvent += this.OnRTMPClientConnectFailedEvent;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.rtmpClient != null)
            {
                this.rtmpClient.disconnect();
            }
        }

        /// <summary>
        /// The notify clients.
        /// </summary>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="T">
        /// Type of entity
        /// </typeparam>
        public void NotifyClientsAboutChangesInTable<T>(NotificationType notificationType, int companyId, T entity)
            where T : Entity
        {
            this.NotifyClientsAboutChangesInTable<T>(notificationType, companyId, entity.Id);
        }

        /// <summary>
        /// The notify clients.
        /// </summary>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <typeparam name="T">
        /// Type of entity
        /// </typeparam>
        public void NotifyClientsAboutChangesInTable<T>(NotificationType notificationType, int companyId, int id)
        {
            var allowedTypes = new List<Type>
                                   {
                                       typeof(User), 
                                       typeof(AppletResult), 
                                       typeof(QuizResult), 
                                       typeof(TestResult), 
                                       typeof(SurveyResult), 
                                       typeof(Company),
                                       typeof(SNGroupDiscussion),
                                       typeof(SNMember)
                                   };
            if (allowedTypes.Contains(typeof(T)) && !string.IsNullOrWhiteSpace(this.settings.RTMPServerPort))
            {
                this.logger.Error(string.Format("starting RTMP call: {0}, companyId={1}, id={2}", notificationType.ToString(), companyId, id));
                this.rtmpClient.connect(
                    "localhost", 
                    int.Parse(this.settings.RTMPServerPort), 
                    "DBChangesNotifier", 
                    new object[] { -1 }, 
                    new ClientConnectionHandler(this.rtmpClient, this.logger, typeof(T), notificationType, id, companyId));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The check if server is running.
        /// </summary>
        private void CheckIfServerIsRunning()
        {
            lock (_locker)
            {
                if (!string.IsNullOrWhiteSpace(this.settings.RTMPServerPort) && HttpContext.Current != null)
                {
                    var server = HttpContext.Current.Application[ApplicationKeys.WebOrbRTMPServerKey] as RTMPServer;
                    if (server == null || !server.IsRunning)
                    {
                        try
                        {
                            if (server != null)
                            {
                                server.shutdown();
                                server = null;
                            }
                        }
                        catch (Exception)
                        {
                        }

                        // Initialize WebORB configuration before starting messaging server
                        ORBConfig config = ORBConfig.GetInstance();
                        server = new RTMPServer(typeof(DBChangesNotifier).Name, int.Parse((string)this.settings.RTMPServerPort), 500, config);
                        // Start the messaging server
                        server.start();
                        this.logger.Error("RTMP server was restarted");
                        // Store the server instance in the Application context, so it can be cleared out when application stops     
                        HttpContext.Current.Application[ApplicationKeys.WebOrbRTMPServerKey] = server;
                    }
                }
            }
        }

        /// <summary>
        ///     The on rtmp client connect failed event.
        /// </summary>
        private void OnRTMPClientConnectFailedEvent()
        {
            this.logger.Error("RTMP connection failed");
        }

        /// <summary>
        /// The сonvert to json.
        /// </summary>
        /// <param name="entityDto">
        /// The entity dto.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string СonvertToJson<T>(T entityDto)
        {
            return JsonConvert.SerializeObject(entityDto, new JsonDateTimeConverter());
        }

        #endregion
    }
}