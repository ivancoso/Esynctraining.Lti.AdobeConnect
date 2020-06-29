namespace EdugameCloud.WCFService.CORS
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The custom header message inspector.
    /// </summary>
    public class CustomHeaderMessageInspector : IDispatchMessageInspector
    {
        #region Fields

        /// <summary>
        ///     The required headers.
        /// </summary>
        private readonly Dictionary<string, string> requiredHeaders;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHeaderMessageInspector"/> class.
        /// </summary>
        /// <param name="headers">
        /// The headers.
        /// </param>
        public CustomHeaderMessageInspector(Dictionary<string, string> headers)
        {
            this.requiredHeaders = headers ?? new Dictionary<string, string>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The after receive request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="instanceContext">
        /// The instance context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        /// <summary>
        /// The before send reply.
        /// </summary>
        /// <param name="reply">
        /// The reply.
        /// </param>
        /// <param name="correlationState">
        /// The correlation state.
        /// </param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply.Properties.ContainsKey("httpResponse"))
            {
                var httpResponse = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
                if (httpResponse != null)
                {
                    foreach (var item in this.requiredHeaders)
                    {
                        if (!httpResponse.Headers.HasKey(item.Key))
                        {
                            httpResponse.Headers.Add(item.Key, item.Value);
                        }
                    }

                    return;
                }
            }

            WebOperationContext currentContext = WebOperationContext.Current;
            if (currentContext != null)
            {
                OutgoingWebResponseContext response = currentContext.OutgoingResponse;
                foreach (var item in this.requiredHeaders)
                {
                    if (!response.Headers.HasKey(item.Key))
                    {
                        response.Headers.Add(item.Key, item.Value);
                    }
                }
            }
        }

        #endregion
    }
}