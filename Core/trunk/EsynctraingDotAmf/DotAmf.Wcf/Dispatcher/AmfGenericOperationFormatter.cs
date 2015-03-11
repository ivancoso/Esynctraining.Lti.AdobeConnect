// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfGenericOperationFormatter.cs">
//   
// </copyright>
// <summary>
//   Generic AMF operation formatter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    using DotAmf.Data;
    using DotAmf.ServiceModel.Channels;
    using DotAmf.ServiceModel.Messaging;

    /// <summary>
    ///     Generic AMF operation formatter.
    /// </summary>
    internal sealed class AmfGenericOperationFormatter : IDispatchMessageFormatter
    {
        #region Public Methods and Operators

        /// <summary>
        /// Deserializes a message into an array of parameters.
        /// </summary>
        /// <param name="message">
        /// The incoming message.
        /// </param>
        /// <param name="parameters">
        /// The objects that are passed to the operation as parameters.
        /// </param>
        public void DeserializeRequest(Message message, object[] parameters)
        {
            var amfrequest = message as AmfGenericMessage;

            if (amfrequest == null)
            {
                throw new OperationCanceledException(
                    Errors.AmfGenericOperationFormatter_DeserializeRequest_InvalidOperation);
            }

            OperationContext.Current.IncomingMessageProperties[MessagingHeaders.InvokerMessageBody] =
                amfrequest.AmfMessage;
            OperationContext.Current.IncomingMessageProperties[MessagingHeaders.InvokerMessageHeaders] =
                amfrequest.AmfHeaders;

            var rpcMessage = amfrequest.AmfMessage.Data as RemotingMessage;

            if (rpcMessage != null)
            {
                OperationContext.Current.IncomingMessageProperties[MessagingHeaders.RemotingMessage] = rpcMessage;
            }

            IList input;

            if (rpcMessage != null)
            {
                input = rpcMessage.Body as IList;
            }
            else
            {
                input = amfrequest.AmfMessage.Data as IList;
            }

            if (input != null && input.Count == 0 || input == null)
            {
                return;
            }

            if (input.Count != parameters.Length)
            {
                throw new InvalidOperationException(
                    Errors.AmfGenericOperationFormatter_DeserializeRequest_ArgumentCountMismatch);
            }

            for (int i = 0; i < input.Count; i++)
            {
                parameters[i] = input[i];
            }
        }

        /// <summary>
        /// Serializes a reply message from a specified message version, array of parameters, and a return value.
        /// </summary>
        /// <param name="messageVersion">
        /// The SOAP message version.
        /// </param>
        /// <param name="parameters">
        /// The out parameters.
        /// </param>
        /// <param name="result">
        /// The return value.
        /// </param>
        /// <returns>
        /// The serialized reply message.
        /// </returns>
        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            // An AMF operation
            if (OperationContext.Current.IncomingMessageProperties.ContainsKey(MessagingHeaders.InvokerMessageBody))
            {
                var requestMessage =
                    (AmfMessage)OperationContext.Current.IncomingMessageProperties[MessagingHeaders.InvokerMessageBody];

                // An RPC operation
                if (OperationContext.Current.IncomingMessageProperties.ContainsKey(MessagingHeaders.RemotingMessage))
                {
                    var rpcMessage =
                        (RemotingMessage)
                        OperationContext.Current.IncomingMessageProperties[MessagingHeaders.RemotingMessage];
                    AcknowledgeMessage acknowledge = AmfOperationUtil.BuildAcknowledgeMessage(rpcMessage);
                    acknowledge.Body = result;

                    result = acknowledge;
                }

                var replyHeaders = new Dictionary<string, AmfHeader>();
                var replyMessage = new AmfMessage
                                       {
                                           Data = result, 
                                           Target = AmfOperationUtil.CreateResultReplyTarget(requestMessage), 
                                           Response = string.Empty
                                       };

                return new AmfGenericMessage(replyHeaders, replyMessage);
            }

            throw new OperationCanceledException(Errors.AmfGenericOperationFormatter_SerializeReply_InvalidOperation);
        }

        #endregion
    }
}