// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfOperationUtil.cs">
//   
// </copyright>
// <summary>
//   AMF operations utility.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;

    using DotAmf.Data;
    using DotAmf.ServiceModel.Channels;
    using DotAmf.ServiceModel.Messaging;

    /// <summary>
    ///     AMF operations utility.
    /// </summary>
    internal static class AmfOperationUtil
    {
        #region Constants

        /// <summary>
        ///     Fault operation's target template.
        /// </summary>
        private const string OperationFaultTarget = "{0}/onStatus";

        /// <summary>
        ///     Result operation's target template.
        /// </summary>
        private const string OperationResultTarget = "{0}/onResult";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Build an acknowledge message.
        /// </summary>
        /// <param name="message">
        /// Incoming message.
        /// </param>
        /// <returns>
        /// The <see cref="AcknowledgeMessage"/>.
        /// </returns>
        public static AcknowledgeMessage BuildAcknowledgeMessage(AbstractMessage message)
        {
            return new AcknowledgeMessage
                       {
                           MessageId = GenerateUuid(), 
                           CorrelationId = message.MessageId, 
                           Timestamp = GenerateTimestamp()
                       };
        }

        /// <summary>
        /// Build an error message.
        /// </summary>
        /// <param name="message">
        /// Incoming message.
        /// </param>
        /// <returns>
        /// The <see cref="ErrorMessage"/>.
        /// </returns>
        public static ErrorMessage BuildErrorMessage(AbstractMessage message)
        {
            return new ErrorMessage
                       {
                           MessageId = GenerateUuid(), 
                           CorrelationId = message.MessageId, 
                           Timestamp = GenerateTimestamp()
                       };
        }

        /// <summary>
        /// Build a message reply.
        /// </summary>
        /// <param name="request">
        /// Request message.
        /// </param>
        /// <param name="body">
        /// Reply message's body.
        /// </param>
        /// <returns>
        /// The <see cref="AmfGenericMessage"/>.
        /// </returns>
        public static AmfGenericMessage BuildMessageReply(AmfGenericMessage request, object body)
        {
            var replyHeaders = new Dictionary<string, AmfHeader>();
            var replyMessage = new AmfMessage
                                   {
                                       Target = CreateResultReplyTarget(request.AmfMessage), 
                                       Response = string.Empty, 
                                       Data = body
                                   };

            return new AmfGenericMessage(replyHeaders, replyMessage);
        }

        /// <summary>
        /// Create a result message's reply target.
        /// </summary>
        /// <param name="requestMessage">
        /// Request message.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string CreateResultReplyTarget(AmfMessage requestMessage)
        {
            return string.Format(OperationResultTarget, requestMessage.Response ?? string.Empty);
        }

        /// <summary>
        /// Create a status message's reply target.
        /// </summary>
        /// <param name="requestMessage">
        /// Request message.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string CreateStatusReplyTarget(AmfMessage requestMessage)
        {
            return string.Format(OperationFaultTarget, requestMessage.Response ?? string.Empty);
        }

        /// <summary>
        ///     Generate current timestamp.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public static double GenerateTimestamp()
        {
            TimeSpan span = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return span.TotalSeconds;
        }

        /// <summary>
        ///     Generate a unique ID.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GenerateUuid()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        #endregion
    }
}