// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AbstractAmfEncoder.cs">
//   
// </copyright>
// <summary>
//   Abstract AMF encoder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Encoder
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using DotAmf.Data;
    using DotAmf.IO;

    /// <summary>
    ///     Abstract AMF encoder.
    /// </summary>
    internal abstract class AbstractAmfEncoder : IAmfEncoder
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAmfEncoder"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="encodingOptions">
        /// AMF encoding options.
        /// </param>
        protected AbstractAmfEncoder(AmfEncodingOptions encodingOptions)
        {
            this.EncodingOptions = encodingOptions;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     AMF encoding options.
        /// </summary>
        protected AmfEncodingOptions EncodingOptions { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        public abstract void Encode(Stream stream, XmlReader input);

        /// <summary>
        /// The write packet body.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        public abstract void WritePacketBody(Stream stream, AmfMessageDescriptor descriptor);

        /// <summary>
        /// The write packet header.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        public abstract void WritePacketHeader(Stream stream, AmfHeaderDescriptor descriptor);

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
            AmfVersion amfVersion = this.EncodingOptions.UseContextSwitch
                                        ? AmfVersion.Amf0
                                        : this.EncodingOptions.AmfVersion;

            return new AmfContext(amfVersion);
        }

        /// <summary>
        /// Write AMF value from the current position.
        /// </summary>
        /// <param name="context">
        /// AMF decoding context.
        /// </param>
        /// <param name="input">
        /// AMFX input reader.
        /// </param>
        /// <param name="writer">
        /// AMF stream writer.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// AMF type is not supported.
        /// </exception>
        /// <exception cref="FormatException">
        /// Invalid data format.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Error during serialization.
        /// </exception>
        protected abstract void WriteAmfValue(AmfContext context, XmlReader input, AmfStreamWriter writer);

        #endregion
    }
}