// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfEncoderFactory.cs">
//   
// </copyright>
// <summary>
//   AMF message encoder factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.ServiceModel.Channels
{
    using System;
    using System.ServiceModel.Channels;

    using DotAmf.Data;

    /// <summary>
    ///     AMF message encoder factory.
    /// </summary>
    /// <remarks>
    ///     The factory for producing message encoders that can read messages
    ///     from a stream and write them to a stream for various types of message encoding.
    /// </remarks>
    internal sealed class AmfEncoderFactory : MessageEncoderFactory
    {
        #region Constants

        /// <summary>
        ///     The default AMF character set.
        /// </summary>
        public const string DefaultAmfCharSet = "utf-8";

        /// <summary>
        ///     The default AMF media type (MIME).
        /// </summary>
        public const string DefaultAmfMediaType = "application/x-amf";

        #endregion

        #region Fields

        /// <summary>
        ///     Message encoder instance.
        /// </summary>
        /// <remarks>
        ///     A message encoder should support concurrent calls,
        ///     so it is safe to have only a single instance of it.
        /// </remarks>
        private readonly AmfEncoder _encoder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfEncoderFactory"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="encodingOptions">
        /// AMF encoding options.
        /// </param>
        public AmfEncoderFactory(AmfEncodingOptions encodingOptions)
        {
            // Create an encoder instance for future use
            this._encoder = new AmfEncoder(encodingOptions);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the message encoder that is produced by the factory.
        /// </summary>
        public override MessageEncoder Encoder
        {
            get
            {
                return this._encoder;
            }
        } // And you call that a factory?

        /// <summary>
        ///     Gets the message version that is used by the encoders produced by the factory to encode messages.
        /// </summary>
        /// <remarks>By default, this one is <c>MessageVersion.None</c> since we don't need any SOAP/WS-* support.</remarks>
        public override MessageVersion MessageVersion
        {
            get
            {
                return MessageVersion.None;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create session encoder.
        /// </summary>
        /// <returns>
        ///     The <see cref="MessageEncoder" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override MessageEncoder CreateSessionEncoder()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}