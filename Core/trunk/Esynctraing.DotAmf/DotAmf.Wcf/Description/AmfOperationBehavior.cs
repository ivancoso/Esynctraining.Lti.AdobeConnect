// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfOperationBehavior.cs">
//   
// </copyright>
// <summary>
//   Enables the AMF support for an operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Description
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    using DotAmf.ServiceModel.Dispatcher;

    /// <summary>
    ///     Enables the AMF support for an operation.
    /// </summary>
    internal sealed class AmfOperationBehavior : IOperationBehavior
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add binding parameters.
        /// </summary>
        /// <param name="operationDescription">
        /// The operation description.
        /// </param>
        /// <param name="bindingParameters">
        /// The binding parameters.
        /// </param>
        public void AddBindingParameters(
            OperationDescription operationDescription, 
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// The apply client behavior.
        /// </summary>
        /// <param name="operationDescription">
        /// The operation description.
        /// </param>
        /// <param name="clientOperation">
        /// The client operation.
        /// </param>
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the service across an operation.
        /// </summary>
        /// <param name="operationDescription">
        /// The operation being examined. Use for examination only.
        ///     If the operation description is modified, the results are undefined.
        /// </param>
        /// <param name="dispatchOperation">
        /// The run-time object that exposes customization properties
        ///     for the operation described by <c>operationDescription</c>.
        /// </param>
        public void ApplyDispatchBehavior(
            OperationDescription operationDescription, 
            DispatchOperation dispatchOperation)
        {
            dispatchOperation.Formatter = new AmfGenericOperationFormatter();
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="operationDescription">
        /// The operation description.
        /// </param>
        public void Validate(OperationDescription operationDescription)
        {
        }

        #endregion
    }
}