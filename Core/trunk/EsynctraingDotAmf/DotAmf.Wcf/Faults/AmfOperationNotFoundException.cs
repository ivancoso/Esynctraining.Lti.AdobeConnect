// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfOperationNotFoundException.cs">
//   
// </copyright>
// <summary>
//   AMF operation not found.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Faults
{
    using System;

    /// <summary>
    ///     AMF operation not found.
    /// </summary>
    internal class AmfOperationNotFoundException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfOperationNotFoundException"/> class.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        public AmfOperationNotFoundException(string operationName)
            : this(null, operationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfOperationNotFoundException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public AmfOperationNotFoundException(string message, string operationName)
            : base(message)
        {
            if (string.IsNullOrEmpty(operationName))
            {
                throw new ArgumentException("Invalid operation name.", "operationName");
            }

            this.OperationName = operationName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Operation name.
        /// </summary>
        public string OperationName { get; set; }

        #endregion
    }
}