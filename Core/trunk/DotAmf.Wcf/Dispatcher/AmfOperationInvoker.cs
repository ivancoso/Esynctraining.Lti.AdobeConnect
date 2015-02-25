// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfOperationInvoker.cs">
//   
// </copyright>
// <summary>
//   Abstract AMF operation invoker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel.Dispatcher;

    using DotAmf.ServiceModel.Configuration;

    /// <summary>
    ///     Abstract AMF operation invoker.
    /// </summary>
    internal abstract class AmfOperationInvoker : IOperationInvoker
    {
        #region Fields

        /// <summary>
        ///     Endpoint capabilities.
        /// </summary>
        protected readonly AmfEndpointCapabilities Capabilities;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfOperationInvoker"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="capabilities">
        /// Endpoint capabilities.
        /// </param>
        protected AmfOperationInvoker(AmfEndpointCapabilities capabilities)
        {
            this.Capabilities = capabilities;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     <c>true</c> if the dispatcher invokes the synchronous operation; otherwise, <c>false</c>.
        /// </summary>
        /// <remarks>Only synchronous AMF operations are supported.</remarks>
        public bool IsSynchronous
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns an <c>System.Array</c> of parameter objects.
        /// </summary>
        /// <returns>The parameters that are to be used as arguments to the operation.</returns>
        public abstract object[] AllocateInputs();

        /// <summary>
        /// Returns an object and a set of output objects from an instance and set of input objects.
        /// </summary>
        /// <param name="instance">
        /// The object to be invoked.
        /// </param>
        /// <param name="inputs">
        /// The inputs to the method.
        /// </param>
        /// <param name="outputs">
        /// The outputs from the method.
        /// </param>
        /// <returns>
        /// The return value.
        /// </returns>
        public abstract object Invoke(object instance, object[] inputs, out object[] outputs);

        /// <summary>
        /// The invoke begin.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="inputs">
        /// The inputs.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The invoke end.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="outputs">
        /// The outputs.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}