// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfBatchMessage.cs">
//   
// </copyright>
// <summary>
//   Batch AMF message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DotAmf.Data;

    /// <summary>
    ///     Batch AMF message.
    /// </summary>
    internal sealed class AmfBatchMessage : AmfMessageBase
    {
        #region Fields

        /// <summary>
        ///     AMF messages.
        /// </summary>
        private readonly IEnumerable<AmfMessage> _amfMessages;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfBatchMessage"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="headers">
        /// AMF headers.
        /// </param>
        /// <param name="messages">
        /// AMF messages.
        /// </param>
        public AmfBatchMessage(IDictionary<string, AmfHeader> headers, IEnumerable<AmfMessage> messages)
            : base(headers)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            this._amfMessages = messages;

            // AMF packet contains multimple messages, so we will need batching
            this.Properties.AllowOutputBatching = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     AMF messages.
        /// </summary>
        public IEnumerable<AmfMessage> AmfMessages
        {
            get
            {
                return this._amfMessages;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get a list of contained AMF messages.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<AmfGenericMessage> ToList()
        {
            return this.AmfMessages.Select(msg => new AmfGenericMessage(this.AmfHeaders, msg));
        }

        #endregion
    }
}