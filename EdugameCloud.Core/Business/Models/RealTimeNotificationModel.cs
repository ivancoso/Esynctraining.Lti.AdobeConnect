namespace EdugameCloud.Core.Business.Models
{
    using System;
    using Esynctraining.Core.Logging;

    using EdugameCloud.Core.Converters;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.RTMP;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Providers;
    using Esynctraining.NHibernate;

    using Newtonsoft.Json;

    /// <summary>
    /// The real time Notification model.
    /// </summary>
    public class RealTimeNotificationModel : IDisposable
    {
        #region Static Fields

        #endregion

        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RealTimeNotificationModel"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public RealTimeNotificationModel(ILogger logger, ApplicationSettingsProvider settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
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
            // NOTE: we don't support RTMP
            //var allowedTypes = new List<Type>
            //                       {
            //                           typeof(User), 
            //                           typeof(AppletResult), 
            //                           typeof(QuizResult), 
            //                           typeof(TestResult), 
            //                           typeof(SurveyResult), 
            //                           typeof(Company), 
            //                           typeof(SNGroupDiscussion), 
            //                           typeof(SNMember), 
            //                       };
            //if (allowedTypes.Contains(typeof(T)) && !string.IsNullOrWhiteSpace(this.settings.RTMPServerPort))
            //{
            //    this.logger.Error(
            //        string.Format("starting RTMP call: {0}, companyId={1}, id={2}", notificationType, companyId, id));
            //}
        }

        /// <summary>
        /// The notify clients about social tokens.
        /// </summary>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        public void NotifyClientsAboutSocialTokens(SocialUserTokensDTO tokens)
        {
            // NOTE: we don't support RTMP
            //if (!string.IsNullOrWhiteSpace(this.settings.RTMPServerPort))
            //{
            //    // ReSharper disable once UnusedVariable
            //    string vo = this.ConvertToJson(tokens);
            //    this.logger.Info(
            //        string.Format("starting social RTMP call: key={0}, provider={1}", tokens.key, tokens.provider));
            //}
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert to JSON.
        /// </summary>
        /// <param name="entityDto">
        /// The entity DTO.
        /// </param>
        /// <typeparam name="T">
        /// Entity DTO type
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ConvertToJson<T>(T entityDto)
        {
            return JsonConvert.SerializeObject(entityDto, new JsonDateTimeConverter());
        }

        #endregion
    }
}