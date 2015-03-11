// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfStreamWriter.cs">
//   
// </copyright>
// <summary>
//   AMF stream writer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.IO
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    ///     AMF stream writer.
    /// </summary>
    internal sealed class AmfStreamWriter : BinaryWriter
    {
        #region Static Fields

        /// <summary>
        ///     Delegate to use for preparing bytes before writing.
        /// </summary>
        /// <remarks>
        ///     AMF messages have a big endian (network) byte order.
        /// </remarks>
        private static readonly PrepareBytes PrepareBytes = ByteConverter.IsLittleEndian
                                                                ? (PrepareBytes)ByteConverter.ChangeEndianness
                                                                : ByteConverter.KeepEndianness;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public AmfStreamWriter(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmfStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        public AmfStreamWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposes writer, but leaves the underlying stream open.
        /// </summary>
        public new void Dispose()
        {
            this.BaseStream.Flush();
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(byte value)
        {
            base.Write(value);
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override void Write(sbyte value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(short value)
        {
            this.Write(PrepareBytes(BitConverter.GetBytes(value)));
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(ushort value)
        {
            this.Write(PrepareBytes(BitConverter.GetBytes(value)));
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(int value)
        {
            this.Write(PrepareBytes(BitConverter.GetBytes(value)));
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(uint value)
        {
            this.Write(PrepareBytes(BitConverter.GetBytes(value)));
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override void Write(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override void Write(ulong value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override void Write(float value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override void Write(decimal value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(double value)
        {
            this.Write(PrepareBytes(BitConverter.GetBytes(value)));
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override void Write(string value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}