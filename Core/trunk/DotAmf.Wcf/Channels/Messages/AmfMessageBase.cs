// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfMessageBase.cs">
//   
// </copyright>
// <summary>
//   Abstract AMF message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Channels;
    using System.Xml;

    using DotAmf.Data;

    /// <summary>
    ///     Abstract AMF message.
    /// </summary>
    internal abstract class AmfMessageBase : Message
    {
        #region Fields

        /// <summary>
        ///     AMF headers.
        /// </summary>
        private readonly IDictionary<string, AmfHeader> _amfHeaders;

        /// <summary>
        ///     Message headers.
        /// </summary>
        private readonly MessageHeaders _headers;

        /// <summary>
        ///     Message properties.
        /// </summary>
        private readonly MessageProperties _properties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AmfMessageBase" /> class.
        ///     Constructor.
        /// </summary>
        protected AmfMessageBase()
        {
            this._properties = new MessageProperties();

            // Make sure that there is no wrapping applied to this message
            this._headers = new MessageHeaders(MessageVersion.None);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfMessageBase"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="headers">
        /// The headers.
        /// </param>
        protected AmfMessageBase(IDictionary<string, AmfHeader> headers)
            : this()
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            this._amfHeaders = headers;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     AMF headers.
        /// </summary>
        public IDictionary<string, AmfHeader> AmfHeaders
        {
            get
            {
                return this._amfHeaders;
            }
        }

        /// <summary>
        ///     Gets the headers of the message.
        /// </summary>
        public override sealed MessageHeaders Headers
        {
            get
            {
                return this._headers;
            }
        }

        /// <summary>
        ///     Gets a set of processing-level annotations to the message.
        /// </summary>
        public override sealed MessageProperties Properties
        {
            get
            {
                return this._properties;
            }
        }

        /// <summary>
        ///     Gets the SOAP version of the message.
        /// </summary>
        /// <remarks>To make sure that there won't be any SOAP, this one always returns <c>None</c>.</remarks>
        public override sealed MessageVersion Version
        {
            get
            {
                return MessageVersion.None;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the message body is written to an XML file.
        /// </summary>
        /// <param name="writer">
        /// A XmlDictionaryWriter that is used to write this message body to an XML file.
        /// </param>
        protected override sealed void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            // We don't need this at all
        }

        #endregion
    }
}