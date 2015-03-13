// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmfErrorHandler.cs" company="">
//   
// </copyright>
// <summary>
//   AMF error handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using AmfMessageHeader = DotAmf.ServiceModel.Messaging.MessageHeader;

namespace DotAmf.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    using DotAmf.Data;
    using DotAmf.ServiceModel.Channels;
    using DotAmf.ServiceModel.Configuration;
    using DotAmf.ServiceModel.Faults;
    using DotAmf.ServiceModel.Messaging;

    /// <summary>
    ///     AMF error handler.
    /// </summary>
    public sealed class AmfErrorHandler : IErrorHandler
    {
        #region Fields

        /// <summary>
        ///     Endpoint capabilities.
        /// </summary>
        private readonly AmfEndpointCapabilities _capabilities;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfErrorHandler"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="capabilities">
        /// Endpoint capabilities.
        /// </param>
        public AmfErrorHandler(AmfEndpointCapabilities capabilities)
        {
            this._capabilities = capabilities;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The provide fault externally.
        /// </summary>
        /// <param name="oc">
        /// The oc.
        /// </param>
        /// <param name="exceptionDetaulsInFault">
        /// The exception detauls in fault.
        /// </param>
        /// <param name="stackTrace">
        /// The stack Trace.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="fault">
        /// The fault.
        /// </param>
        public static void ProvideFaultExternally(
            OperationContext oc, 
            bool exceptionDetaulsInFault, 
            string stackTrace, 
            Exception error, 
            MessageVersion version, 
            ref Message fault)
        {
            // An internal server error occured
            if (oc == null)
            {
                return;
            }

            // An AMF operation
            if (oc.IncomingMessageProperties.ContainsKey(MessagingHeaders.InvokerMessageBody))
            {
                var replyHeaders = new Dictionary<string, AmfHeader>();
                var replyMessage = new AmfMessage { Response = string.Empty };

                var requestMessage = (AmfMessage)oc.IncomingMessageProperties[MessagingHeaders.InvokerMessageBody];

                // An RPC operation
                if (oc.IncomingMessageProperties.ContainsKey(MessagingHeaders.RemotingMessage))
                {
                    var rpcMessage = (RemotingMessage)oc.IncomingMessageProperties[MessagingHeaders.RemotingMessage];
                    ErrorMessage acknowledge = AmfOperationUtil.BuildErrorMessage(rpcMessage);
                    if (acknowledge.Headers == null)
                    {
                        acknowledge.Headers = new Dictionary<string, object>();
                    }

                    if (error is AmfOperationNotFoundException)
                    {
                        acknowledge.Headers[AmfMessageHeader.StatusCode] = (int)HttpStatusCode.NotFound;
                    }
                    else
                    {
                        acknowledge.Headers[AmfMessageHeader.StatusCode] = (int)HttpStatusCode.BadRequest;
                    }

                    acknowledge.FaultCode = ErrorMessageFaultCode.DeliveryInDoubt;

                    acknowledge.FaultString = error.Message;

                    if (exceptionDetaulsInFault)
                    {
                        acknowledge.FaultDetail = string.IsNullOrWhiteSpace(stackTrace) ? error.StackTrace : stackTrace;
                    }

                    // Get FaultException's detail object, if any
                    if (error is FaultException)
                    {
                        Type type = error.GetType();

                        if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(FaultException<>)))
                        {
                            acknowledge.ExtendedData = type.GetProperty("Detail").GetValue(error, null);
                        }
                    }

                    replyMessage.Target = AmfOperationUtil.CreateStatusReplyTarget(requestMessage);
                    replyMessage.Data = acknowledge;
                }

                fault = new AmfGenericMessage(replyHeaders, replyMessage);
            }
        }

        /// <summary>
        /// The handle error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HandleError(Exception error)
        {
            return true;
        }

        /// <summary>
        /// The provide fault.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="fault">
        /// The fault.
        /// </param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            ProvideFaultExternally(
                OperationContext.Current, 
                this._capabilities.ExceptionDetailInFaults, 
                null, 
                error, 
                version, 
                ref fault);
        }

        #endregion
    }
}