// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AbstractAmfDecoder.cs">
//   
// </copyright>
// <summary>
//   Abstract AMF decoder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Decoder
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using DotAmf.Data;
    using DotAmf.IO;

    /// <summary>
    ///     Abstract AMF decoder.
    /// </summary>
    internal abstract class AbstractAmfDecoder : IAmfDecoder
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAmfDecoder"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="decodingOptions">
        /// AMF decoding options.
        /// </param>
        protected AbstractAmfDecoder(AmfEncodingOptions decodingOptions)
        {
            this.DecodingOptions = decodingOptions;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     AMF decoding options.
        /// </summary>
        protected AmfEncodingOptions DecodingOptions { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        public abstract void Decode(Stream stream, XmlWriter output);

        /// <summary>
        /// The read packet body.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="AmfMessageDescriptor"/>.
        /// </returns>
        public abstract AmfMessageDescriptor ReadPacketBody(Stream stream);

        /// <summary>
        /// The read packet header.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="AmfHeaderDescriptor"/>.
        /// </returns>
        public abstract AmfHeaderDescriptor ReadPacketHeader(Stream stream);

        #endregion

        #region Methods

        /// <summary>
        ///     Create default AMF decoding context.
        /// </summary>
        /// <returns>
        ///     The <see cref="AmfContext" />.
        /// </returns>
        protected AmfContext CreateDefaultContext()
        {
            // In mixed context enviroinments, 
            // AMF0 is always used by default
            AmfVersion amfVersion = this.DecodingOptions.UseContextSwitch
                                        ? AmfVersion.Amf0
                                        : this.DecodingOptions.AmfVersion;

            return new AmfContext(amfVersion);
        }

        /// <summary>
        /// Read AMF value from the current position.
        /// </summary>
        /// <param name="context">
        /// AMF context.
        /// </param>
        /// <param name="reader">
        /// AMF stream reader.
        /// </param>
        /// <param name="output">
        /// AMFX output writer.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// AMF type is not supported.
        /// </exception>
        /// <exception cref="FormatException">
        /// Invalid data format.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Error during deserialization.
        /// </exception>
        protected abstract void ReadAmfValue(AmfContext context, AmfStreamReader reader, XmlWriter output = null);

        #endregion
    }
}