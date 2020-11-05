namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Castle.Core.Logging;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Providers;

    using Newtonsoft.Json;

    using global::Weborb.Config;

    using global::Weborb.Messaging;

    using global::Weborb.Messaging.Net.RTMP;

    using PDFAnnotation.Core.Converters;
    using PDFAnnotation.Core.Domain.DTO;
    using PDFAnnotation.Core.Domain.Entities;
    using PDFAnnotation.Core.RTMP;

    /// <summary>
    ///     The RTMP model.
    /// </summary>
    public class RTMPModel : IDisposable
    {
        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The rtmp client.
        /// </summary>
        private readonly RTMPClient rtmpClient;

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        private static object _locker = new object();

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
            // Create client
            this.rtmpClient = new RTMPClient();
            this.rtmpClient.ConnectFailedEvent += this.OnRTMPClientConnectFailedEvent;
        }

        private void CheckIfServerIsRunning()
        {
            lock (_locker)
            {
                if (HttpContext.Current != null)
                {
                    var server = (RTMPServer)HttpContext.Current.Application["WebOrbRTMPServerKey"];
                    if (server == null || !server.IsRunning)
                    {
                        try
                        {
                            if (server != null)
                            {
                                server.shutdown();
                            }
                        }
                        catch (Exception)
                        {
                        }
                        // Initialize WebORB configuration before starting messaging server
                        ORBConfig config = ORBConfig.GetInstance();

                        // Create Messaging server. 2037 is the port number, 500 is connection backlog
                        server = new RTMPServer(typeof(DBChangesNotifier).Name, int.Parse((string)this.settings.RTMPServerPort), 500, config);

                        // Start the messaging server
                        server.start();
                        this.logger.Error("RTMP server was restarted");

                        // Store the server instance in the Application context, so it can be cleared out when application stops     
                        HttpContext.Current.Application["WebOrbRTMPServerKey"] = server;
                    }
                }
            }
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
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="T">
        /// Type of entity
        /// </typeparam>
        public void NotifyClientsAboutChangesInTable<T>(NotificationType notificationType, T entity) where T : Entity
        {
//            var contact = entity as Contact;
//            if (contact != null)
//            {
//                this.NotifyClientsAboutChangesInTable(notificationType, typeof(T), new ContactDTO(contact));
//            }
//
//            var dp = entity as Event;
//            if (dp != null)
//            {
//                this.NotifyClientsAboutChangesInTable(notificationType, typeof(T), new EventDTO(dp));
//            }

            var file = entity as File;
            if (file != null)
            {
                this.NotifyClientsAboutChangesInTable(notificationType, typeof(T), new FileDTO(file));
            }
        }

        /// <summary>
        /// The notify clients.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="entityType">
        /// The entity Type.
        /// </param>
        /// <param name="entityDto">
        /// The entity Dto.
        /// </param>
        public void NotifyClientsAboutChangesInTable<T>(NotificationType notificationType, Type entityType, T entityDto)
        {
            var allowedTypes = new List<Type> { typeof(File) }; 
            if (allowedTypes.Contains(entityType))
            {
                this.rtmpClient.connect(
                    "localhost", 
                    int.Parse((string)this.settings.RTMPServerPort), 
                    "DBChangesNotifier", 
                    new object[] { -1 }, 
                    new ClientConnectionHandler(this.rtmpClient, entityType, notificationType, this.СonvertToJson(entityDto)));
            }
        }

        /// <summary>
        /// The notify clients.
        /// </summary>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="T">
        /// Type of entity
        /// </typeparam>
        /// <typeparam name="TId">
        /// Type of entity id
        /// </typeparam>
        public void NotifyClientsAboutChangesInTable<T, TId>(NotificationType notificationType, T entity)
            where T : IEntity<TId>
            where TId : struct
        {
            this.NotifyClientsAboutChangesInTable<T, TId>(notificationType, entity.Id);
        }

        /// <summary>
        /// The notify clients.
        /// </summary>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <typeparam name="T">
        /// Type of entity
        /// </typeparam>
        /// <typeparam name="TId">
        /// Type of entity id
        /// </typeparam>
        public void NotifyClientsAboutChangesInTable<T, TId>(NotificationType notificationType, TId id)
        {
            var allowedTypes = new List<Type> { typeof(ATMark) };
            if (allowedTypes.Contains(typeof(T)))
            {
                this.rtmpClient.connect(
                    "localhost",
                    int.Parse(this.settings.RTMPServerPort),
                    "DBChangesNotifier",
                    new object[] { -1 },
                    new ClientConnectionHandler(this.rtmpClient, typeof(T), notificationType, id.ToString()));
            }
        }

        #endregion

        #region Methods

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