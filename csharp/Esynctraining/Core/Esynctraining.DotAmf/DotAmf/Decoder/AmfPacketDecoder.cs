// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfPacketDecoder.cs">
//   
// </copyright>
// <summary>
//   AMF packet decoder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Decoder
{
    using System;
    using System.IO;
    using System.Xml;

    using DotAmf.Data;
    using DotAmf.IO;

    /// <summary>
    ///     AMF packet decoder.
    /// </summary>
    internal sealed class AmfPacketDecoder
    {
        #region Fields

        /// <summary>
        ///     AMF encoding options.
        /// </summary>
        private readonly AmfEncodingOptions _options;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfPacketDecoder"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="options">
        /// Encoding options.
        /// </param>
        public AmfPacketDecoder(AmfEncodingOptions options)
        {
            if (!options.UseContextSwitch)
            {
                throw new ArgumentException(Errors.AmfPacketReader_AmfPacketReader_ContextSwitchRequired, "options");
            }

            this._options = options;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Decode an AMF packet into an AMFX format.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <exception cref="InvalidDataException">
        /// Error during decoding.
        /// </exception>
        public void Decode(Stream stream, XmlWriter output)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException(Errors.AmfPacketReader_Read_StreamClosed, "stream");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            try
            {
                var amfStreamReader = new AmfStreamReader(stream);

                AmfVersion version = ReadPacketVersion(amfStreamReader);
                IAmfDecoder decoder = CreateDecoder(version, this._options);

                output.WriteStartDocument();
                output.WriteStartElement(AmfxContent.AmfxDocument, AmfxContent.Namespace);
                output.WriteAttributeString(AmfxContent.VersionAttribute, version.ToAmfxName());
                output.Flush();

                // Read headers
                uint headerCount = ReadDataCount(amfStreamReader);

                for (int i = 0; i < headerCount; i++)
                {
                    AmfHeaderDescriptor header = decoder.ReadPacketHeader(stream);

                    output.WriteStartElement(AmfxContent.PacketHeader);
                    output.WriteAttributeString(AmfxContent.PacketHeaderName, header.Name);
                    output.WriteAttributeString(
                        AmfxContent.PacketHeaderMustUnderstand, 
                        header.MustUnderstand.ToString());
                    decoder.Decode(stream, output);
                    output.WriteEndElement();
                    output.Flush();
                }

                // Read messages
                uint messageCount = ReadDataCount(amfStreamReader);

                for (int i = 0; i < messageCount; i++)
                {
                    AmfMessageDescriptor body = decoder.ReadPacketBody(stream);

                    output.WriteStartElement(AmfxContent.PacketBody);
                    output.WriteAttributeString(AmfxContent.PacketBodyTarget, body.Target);
                    output.WriteAttributeString(AmfxContent.PacketBodyResponse, body.Response);
                    decoder.Decode(stream, output);
                    output.WriteEndElement();
                    output.Flush();
                }

                output.WriteEndElement();
                output.WriteEndDocument();
                output.Flush();
            }
            catch (Exception e)
            {
                output.Flush();
                throw new InvalidDataException(Errors.AmfPacketReader_DecodingError, e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create AMF decoder.
        /// </summary>
        /// <param name="version">
        /// AMF packet version.
        /// </param>
        /// <param name="options">
        /// Encoding options.
        /// </param>
        /// <returns>
        /// The <see cref="IAmfDecoder"/>.
        /// </returns>
        private static IAmfDecoder CreateDecoder(AmfVersion version, AmfEncodingOptions options)
        {
            switch (version)
            {
                case AmfVersion.Amf0:
                    return new Amf0Decoder(options);

                case AmfVersion.Amf3:
                    return new Amf3Decoder(options);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Read number of following headers/messages.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private static uint ReadDataCount(AmfStreamReader reader)
        {
            return reader.ReadUInt16();
        }

        /// <summary>
        /// Read AMF packet version.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <exception cref="FormatException">
        /// Data has unknown format.
        /// </exception>
        /// <returns>
        /// The <see cref="AmfVersion"/>.
        /// </returns>
        private static AmfVersion ReadPacketVersion(AmfStreamReader reader)
        {
            try
            {
                // First two bytes contain message version number
                return (AmfVersion)reader.ReadUInt16();
            }
            catch (Exception e)
            {
                throw new FormatException(Errors.AmfPacketReader_ReadPacketVersion_VersionReadError, e);
            }
        }

        #endregion
    }
}