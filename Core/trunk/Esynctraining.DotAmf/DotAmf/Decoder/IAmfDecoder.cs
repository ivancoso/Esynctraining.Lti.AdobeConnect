// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="IAmfDecoder.cs">
//   
// </copyright>
// <summary>
//   AMF decoder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Decoder
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using DotAmf.Data;

    /// <summary>
    ///     AMF decoder.
    /// </summary>
    public interface IAmfDecoder
    {
        #region Public Methods and Operators

        /// <summary>
        /// Decode data to AMFX format.
        /// </summary>
        /// <param name="stream">
        /// AMF stream.
        /// </param>
        /// <param name="output">
        /// AMFX output writer.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// AMF type is not supported.
        /// </exception>
        /// <exception cref="FormatException">
        /// Unknown data format.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Error during deserialization.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Invalid AMF context.
        /// </exception>
        void Decode(Stream stream, XmlWriter output);

        /// <summary>
        /// Read AMF packet body descriptor.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <exception cref="FormatException">
        /// Data has unknown format.
        /// </exception>
        /// <returns>
        /// The <see cref="AmfMessageDescriptor"/>.
        /// </returns>
        AmfMessageDescriptor ReadPacketBody(Stream stream);

        /// <summary>
        /// Read an AMF packet header descriptor.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <exception cref="FormatException">
        /// Data has unknown format.
        /// </exception>
        /// <returns>
        /// The <see cref="AmfHeaderDescriptor"/>.
        /// </returns>
        AmfHeaderDescriptor ReadPacketHeader(Stream stream);

        #endregion
    }
}