namespace EdugameCloud.Core.RTMP
{
    using System;

    using Castle.Core.Logging;

    using Weborb.Messaging.Api.Service;
    using Weborb.Messaging.Net.RTMP;

    /// <summary>
    /// The client connection handler.
    /// </summary>
    public class ClientConnectionHandler : IPendingServiceCallback
    {
        #region Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly RTMPClient client;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The domain object type.
        /// </summary>
        private readonly Type domainObjectType;

        /// <summary>
        /// The id.
        /// </summary>
        private readonly int id;

        /// <summary>
        /// The companyId.
        /// </summary>
        private readonly int companyId;

        /// <summary>
        /// The type.
        /// </summary>
        private readonly NotificationType type;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectionHandler"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="domainObjectType">
        /// The domain object type.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        public ClientConnectionHandler(RTMPClient client, ILogger logger, Type domainObjectType, NotificationType type, int id, int companyId)
        {
            this.client = client;
            this.logger = logger;
            this.domainObjectType = domainObjectType;
            this.type = type;
            this.id = id;
            this.companyId = companyId;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The result received.
        /// </summary>
        /// <param name="call">
        /// The call.
        /// </param>
        public void resultReceived(IPendingServiceCall call)
        {
            this.logger.Error(string.Format("internal RTMP client connected: {0}, companyId={1}, id={2}", this.type, this.companyId, this.id));
            // client connected
            switch (this.type)
            {
                case NotificationType.Update:
                    this.client.invoke(
                        "notifyDomainObjectInserted", new[] { this.domainObjectType.Name, this.companyId, (object)this.id }, null);
                    break;
                case NotificationType.Delete:
                    this.client.invoke(
                        "notifyDomainObjectDeleted", new[] { this.domainObjectType.Name, this.companyId, (object)this.id }, null);
                    break;
            }
        }

        #endregion
    }
}