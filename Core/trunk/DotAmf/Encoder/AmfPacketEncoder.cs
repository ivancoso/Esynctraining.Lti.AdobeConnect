// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfPacketEncoder.cs">
//   
// </copyright>
// <summary>
//   AMF packet writer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Encoder
{
    using System;
    using System.IO;
    using System.Xml;

    using DotAmf.Data;
    using DotAmf.IO;

    /// <summary>
    ///     AMF packet writer.
    /// </summary>
    public sealed class AmfPacketEncoder
    {
        #region Fields

        /// <summary>
        ///     AMF encoding options.
        /// </summary>
        private readonly AmfEncodingOptions _options;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfPacketEncoder"/> class.
        ///     Constructor.
        /// </summary>
        /// <param name="options">
        /// Encoding options.
        /// </param>
        public AmfPacketEncoder(AmfEncodingOptions options)
        {
            this._options = options;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Encode an AMF packet from an AMFX format.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <exception cref="InvalidDataException">
        /// Error during encoding.
        /// </exception>
        public void Encode(Stream stream, XmlReader input)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException(Errors.AmfPacketWriter_Write_StreamNotWriteable, "stream");
            }

            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            try
            {
                IAmfEncoder encoder = CreateEncoder(this._options);
                var amfStreamWriter = new AmfStreamWriter(stream);

                WriteAmfVersion(amfStreamWriter, this._options.AmfVersion);

                input.MoveToContent();

                int headerCount = Convert.ToInt32(input.GetAttribute(AmfxContent.PacketHeaderCount));
                int bodyCount = Convert.ToInt32(input.GetAttribute(AmfxContent.PacketBodyCount));

                while (input.Read())
                {
                    if (input.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    if (headerCount != -1)
                    {
                        WriteHeaderCount(amfStreamWriter, headerCount);
                        headerCount = -1;
                    }

                    

                    if (input.Name == AmfxContent.PacketHeader)
                    {
                        var header = new AmfHeaderDescriptor
                                         {
                                             Name = input.GetAttribute(AmfxContent.PacketHeaderName), 
                                             MustUnderstand =
                                                 input.GetAttribute(
                                                     AmfxContent.PacketHeaderMustUnderstand)
                                                 == AmfxContent.True
                                         };

                        encoder.WritePacketHeader(stream, header);

                        while (input.Read())
                        {
                            // Skip until header content is found, if any
                            if (input.NodeType != XmlNodeType.Element || input.Name == AmfxContent.PacketHeader)
                            {
                                continue;
                            }

                            encoder.Encode(stream, input);
                            break;
                        }

                        continue;
                    }

                    

                    if (bodyCount != -1)
                    {
                        WriteMessageCount(amfStreamWriter, bodyCount);
                        bodyCount = -1;
                    }

                    #region Read packet body

                    if (input.Name == AmfxContent.PacketBody)
                    {
                        var message = new AmfMessageDescriptor
                                          {
                                              Target =
                                                  input.GetAttribute(AmfxContent.PacketBodyTarget), 
                                              Response =
                                                  input.GetAttribute(
                                                      AmfxContent.PacketBodyResponse)
                                          };

                        encoder.WritePacketBody(stream, message);

                        while (input.Read())
                        {
                            // Skip until body content is found, if any
                            if (input.NodeType != XmlNodeType.Element || input.Name == AmfxContent.PacketBody)
                            {
                                continue;
                            }

                            encoder.Encode(stream, input);
                            break;
                        }
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(Errors.AmfPacketReader_DecodingError, e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create an AMF encoder.
        /// </summary>
        /// <param name="options">
        /// Encoding options.
        /// </param>
        /// <returns>
        /// The <see cref="IAmfEncoder"/>.
        /// </returns>
        private static IAmfEncoder CreateEncoder(AmfEncodingOptions options)
        {
            switch (options.AmfVersion)
            {
                case AmfVersion.Amf0:
                    return new Amf0Encoder(options);

                case AmfVersion.Amf3:
                    return new Amf3Encoder(options);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Write AMF message version.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        private static void WriteAmfVersion(AmfStreamWriter writer, AmfVersion version)
        {
            writer.Write((ushort)version);
        }

        /// <summary>
        /// Write AMF message headers count.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        private static void WriteHeaderCount(AmfStreamWriter writer, int count)
        {
            writer.Write((ushort)count);
        }

        /// <summary>
        /// Write AMF message bodies count.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        private static void WriteMessageCount(AmfStreamWriter writer, int count)
        {
            writer.Write((ushort)count);
        }

        #endregion
    }
}