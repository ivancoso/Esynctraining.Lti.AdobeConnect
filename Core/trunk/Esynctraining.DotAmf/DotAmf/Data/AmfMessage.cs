// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfMessage.cs">
//   
// </copyright>
// <summary>
//   AMF message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Data
{
    /// <summary>
    ///     AMF message.
    /// </summary>
    public sealed class AmfMessage
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AmfMessage" /> class.
        /// </summary>
        public AmfMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfMessage"/> class.
        /// </summary>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        public AmfMessage(AmfMessageDescriptor descriptor)
        {
            this.Target = descriptor.Target;
            this.Response = descriptor.Response;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     A data associated with the operation.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        ///     A method on the local client that should be invoked to handle the response.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        ///     An operation, function, or method is to be remotely invoked.
        /// </summary>
        public string Target { get; set; }

        #endregion
    }

    /// <summary>
    ///     AMF message descriptor.
    /// </summary>
    public struct AmfMessageDescriptor
    {
        #region Fields

        /// <summary>
        ///     A method on the local client that should be invoked to handle the response.
        /// </summary>
        public string Response;

        /// <summary>
        ///     An operation, function, or method is to be remotely invoked.
        /// </summary>
        public string Target;

        #endregion
    }
}