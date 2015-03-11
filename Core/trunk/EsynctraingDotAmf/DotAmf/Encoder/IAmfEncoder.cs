// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="IAmfEncoder.cs">
//   
// </copyright>
// <summary>
//   AMF encoder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Encoder
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using DotAmf.Data;

    /// <summary>
    ///     AMF encoder.
    /// </summary>
    public interface IAmfEncoder
    {
        #region Public Methods and Operators

        /// <summary>
        /// Encode data from AMFX format.
        /// </summary>
        /// <param name="stream">
        /// AMF stream.
        /// </param>
        /// <param name="input">
        /// AMFX input reader.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// AMF type is not supported.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Error during serialization.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Invalid AMF context.
        /// </exception>
        void Encode(Stream stream, XmlReader input);

        /// <summary>
        /// Write AMF packet body descriptor.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        void WritePacketBody(Stream stream, AmfMessageDescriptor descriptor);

        /// <summary>
        /// Write an AMF packet header descriptor.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        void WritePacketHeader(Stream stream, AmfHeaderDescriptor descriptor);

        #endregion
    }
}