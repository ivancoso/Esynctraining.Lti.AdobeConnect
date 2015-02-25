// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfGenericMessage.cs">
//   
// </copyright>
// <summary>
//   Generic AMF message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;

    using DotAmf.Data;

    /// <summary>
    ///     Generic AMF message.
    /// </summary>
    internal class AmfGenericMessage : AmfMessageBase
    {
        #region Fields

        /// <summary>
        ///     AMF message.
        /// </summary>
        private readonly AmfMessage _amfMessage;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfGenericMessage"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="headers">
        /// AMF headers.
        /// </param>
        /// <param name="message">
        /// AMF message.
        /// </param>
        public AmfGenericMessage(IDictionary<string, AmfHeader> headers, AmfMessage message)
            : base(headers)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this._amfMessage = message;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     AMF message.
        /// </summary>
        public AmfMessage AmfMessage
        {
            get
            {
                return this._amfMessage;
            }
        }

        #endregion
    }
}