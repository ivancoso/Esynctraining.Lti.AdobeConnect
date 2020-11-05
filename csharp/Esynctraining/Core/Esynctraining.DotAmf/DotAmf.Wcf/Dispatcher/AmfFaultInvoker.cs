// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfFaultInvoker.cs">
//   
// </copyright>
// <summary>
//   AMF fault invoker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    using System.ServiceModel;

    using DotAmf.ServiceModel.Configuration;
    using DotAmf.ServiceModel.Faults;
    using DotAmf.ServiceModel.Messaging;

    /// <summary>
    ///     AMF fault invoker.
    /// </summary>
    internal class AmfFaultInvoker : AmfOperationInvoker
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfFaultInvoker"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="capabilities">
        /// Endpoint capabilities.
        /// </param>
        public AmfFaultInvoker(AmfEndpointCapabilities capabilities)
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
        /// <exception cref="AmfOperationNotFoundException">
        /// </exception>
        public override object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];

            string operationName = string.Empty;

            if (OperationContext.Current.IncomingMessageProperties.ContainsKey(MessagingHeaders.RemotingMessage))
            {
                var operation =
                    (RemotingMessage)
                    OperationContext.Current.IncomingMessageProperties[MessagingHeaders.RemotingMessage];
                operationName = operation.Operation;
            }

            throw new AmfOperationNotFoundException(
                string.Format(Errors.AmfFaultInvoker_Invoke_OperationNotFound, operationName), 
                operationName);
        }

        #endregion
    }
}