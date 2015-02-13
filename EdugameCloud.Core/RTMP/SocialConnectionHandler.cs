namespace EdugameCloud.Core.RTMP
{
    using Castle.Core.Logging;

    using Weborb.Messaging.Api.Service;
    using Weborb.Messaging.Net.RTMP;

    /// <summary>
    /// The social connection handler.
    /// </summary>
    public class SocialConnectionHandler : IPendingServiceCallback
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
        /// The key.
        /// </summary>
        private readonly string key;

        /// <summary>
        /// The id.
        /// </summary>
        private readonly string vo;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialConnectionHandler"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="vo">
        /// The vo.
        /// </param>
        public SocialConnectionHandler(RTMPClient client, ILogger logger, string key, string vo)
        {
            this.client = client;
            this.logger = logger;
            this.key = key;
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
            this.logger.Error(string.Format("internal RTMP social client connected: key={0}", this.key));
            this.client.invoke("notifyTokenSecretObtained", new[] { this.key, (object)this.vo }, null);
        }

        #endregion
    }
}