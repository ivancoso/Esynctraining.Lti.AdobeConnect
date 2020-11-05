namespace PDFAnnotation.Core.RTMP
{
    using System;

    using global::Weborb.Messaging.Api.Service;
    using global::Weborb.Messaging.Net.RTMP;

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
        /// The domain object type.
        /// </summary>
        private readonly Type domainObjectType;

        /// <summary>
        /// The id.
        /// </summary>
        public string vo { get; set; }

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
        /// <param name="domainObjectType">
        /// The domain object type.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="vo">
        /// The vo.
        /// </param>
        public ClientConnectionHandler(RTMPClient client, Type domainObjectType, NotificationType type, string vo)
        {
            this.client = client;
            this.domainObjectType = domainObjectType;
            this.type = type;
            this.vo = vo;
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
            // client connected
            switch (this.type)
            {
                case NotificationType.Update:
                    this.client.invoke(
                        "notifyDomainObjectSaved", new object[] { this.domainObjectType.Name, this.vo }, null);
                    break;
                case NotificationType.Delete:
                    this.client.invoke(
                        "notifyDomainObjectDeleted", new object[] { this.domainObjectType.Name, this.vo }, null);
                    break;
            }
        }

        #endregion
    }
}