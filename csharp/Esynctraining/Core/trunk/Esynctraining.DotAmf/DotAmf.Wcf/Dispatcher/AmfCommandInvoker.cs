// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfCommandInvoker.cs">
//   
// </copyright>
// <summary>
//   AMF command invoker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;

    using DotAmf.ServiceModel.Channels;
    using DotAmf.ServiceModel.Configuration;
    using DotAmf.ServiceModel.Messaging;

    /// <summary>
    ///     AMF command invoker.
    /// </summary>
    /// <see cref="AmfCommandFormatter" />
    internal class AmfCommandInvoker : AmfOperationInvoker
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfCommandInvoker"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="capabilities">
        /// Endpoint capabilities.
        /// </param>
        public AmfCommandInvoker(AmfEndpointCapabilities capabilities)
            : base(capabilities)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The allocate inputs.
        /// </summary>
        /// <returns>
        ///     The <see cref="object[]" />.
        /// </returns>
        public override object[] AllocateInputs()
        {
            return new object[1];
        }

        // Allocate memory for an AmfGenericMessage

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="inputs">
        /// The inputs.
        /// </param>
        /// <param name="outputs">
        /// The outputs.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public override object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];

            var request = inputs[0] as AmfGenericMessage;
            if (request == null)
            {
                throw new ArgumentException(Errors.AmfGenericOperationInvoker_Invoke_MessageNotFound, "inputs");
            }

            return this.ProcessCommand(request);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process an AMF command request.
        /// </summary>
        /// <param name="request">
        /// Request message to process.
        /// </param>
        /// <returns>
        /// Service reply message.
        /// </returns>
        protected AmfGenericMessage ProcessCommand(AmfGenericMessage request)
        {
            var command = (CommandMessage)request.AmfMessage.Data;
            Func<AmfGenericMessage, CommandMessage, AmfGenericMessage> handler;

            switch (command.Operation)
            {
                case CommandMessageOperation.ClientPing:
                    handler = this.HandleClientPing;
                    break;

                default:
                    throw new NotSupportedException(
                        string.Format(Errors.AmfCommandInvoker_ProcessCommand_OperationNotSupported, command.Operation));
            }

            return handler.Invoke(request, command);
        }

        /// <summary>
        /// Handle command message: a clinet ping.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="AmfGenericMessage"/>.
        /// </returns>
        private AmfGenericMessage HandleClientPing(AmfGenericMessage request, CommandMessage message)
        {
            AcknowledgeMessage acknowledge = AmfOperationUtil.BuildAcknowledgeMessage(message);
            acknowledge.Headers = new Dictionary<string, object>
                                      {
                                          {
                                              CommandMessageHeader.MessagingVersion, 
                                              this.Capabilities.MessagingVersion
                                          }
                                      };

            return AmfOperationUtil.BuildMessageReply(request, acknowledge);
        }

        #endregion
    }
}