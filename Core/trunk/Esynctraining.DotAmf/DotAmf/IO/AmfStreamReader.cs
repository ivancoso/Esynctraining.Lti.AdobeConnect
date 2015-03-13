// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfStreamReader.cs">
//   
// </copyright>
// <summary>
//   AMF stream reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.IO
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    ///     AMF stream reader.
    /// </summary>
    internal sealed class AmfStreamReader : BinaryReader
    {
        #region Static Fields

        /// <summary>
        ///     Delegate to use for preparing bytes before reading.
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
        /// Initializes a new instance of the <see cref="AmfStreamReader"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public AmfStreamReader(Stream stream)
            : base(stream, Encoding.UTF8)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The read byte.
        /// </summary>
        /// <returns>
        ///     The <see cref="byte" />.
        /// </returns>
        public override byte ReadByte()
        {
            return base.ReadByte();
        }

        /// <summary>
        ///     The read decimal.
        /// </summary>
        /// <returns>
        ///     The <see cref="decimal" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override decimal ReadDecimal()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     The read double.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public override double ReadDouble()
        {
            return BitConverter.ToDouble(PrepareBytes(this.ReadBytes(8)), 0);
        }

        /// <summary>
        ///     The read int 16.
        /// </summary>
        /// <returns>
        ///     The <see cref="short" />.
        /// </returns>
        public override short ReadInt16()
        {
            return BitConverter.ToInt16(PrepareBytes(this.ReadBytes(2)), 0);
        }

        /// <summary>
        ///     The read int 32.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int ReadInt32()
        {
            return BitConverter.ToInt32(PrepareBytes(this.ReadBytes(4)), 0);
        }

        /// <summary>
        ///     The read int 64.
        /// </summary>
        /// <returns>
        ///     The <see cref="long" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override long ReadInt64()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     The read s byte.
        /// </summary>
        /// <returns>
        ///     The <see cref="sbyte" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override sbyte ReadSByte()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     The read single.
        /// </summary>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override float ReadSingle()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     The read string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override string ReadString()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     The read u int 16.
        /// </summary>
        /// <returns>
        ///     The <see cref="ushort" />.
        /// </returns>
        public override ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(PrepareBytes(this.ReadBytes(2)), 0);
        }

        /// <summary>
        /// Reads an UTF-8 encoded String.
        /// </summary>
        /// <param name="length">Byte-length header.</param>
        /// <returns>The String value.</returns>
        public string ReadUTF(int length)
        {
            if (length == 0)
                return string.Empty;
            UTF8Encoding utf8 = new UTF8Encoding(false, true);
            byte[] encodedBytes = this.ReadBytes(length);
#if !(NET_1_1)
            string decodedString = utf8.GetString(encodedBytes, 0, encodedBytes.Length);
#else
            string decodedString = utf8.GetString(encodedBytes);
#endif
            return decodedString;
        }

        public int ReadAMF3IntegerData()
        {
            int acc = ReadByte();
            int tmp;
            if (acc < 128)
                return acc;
            else
            {
                acc = (acc & 0x7f) << 7;
                tmp = this.ReadByte();
                if (tmp < 128)
                    acc = acc | tmp;
                else
                {
                    acc = (acc | tmp & 0x7f) << 7;
                    tmp = this.ReadByte();
                    if (tmp < 128)
                        acc = acc | tmp;
                    else
                    {
                        acc = (acc | tmp & 0x7f) << 8;
                        tmp = this.ReadByte();
                        acc = acc | tmp;
                    }
                }
            }
            //To sign extend a value from some number of bits to a greater number of bits just copy the sign bit into all the additional bits in the new format.
            //convert/sign extend the 29bit two's complement number to 32 bit
            int mask = 1 << 28; // mask
            int r = -(acc & mask) | acc;
            return r;

            //The following variation is not portable, but on architectures that employ an 
            //arithmetic right-shift, maintaining the sign, it should be fast. 
            //s = 32 - 29;
            //r = (x << s) >> s;
        }

        /// <summary>
        ///     The read u int 32.
        /// </summary>
        /// <returns>
        ///     The <see cref="uint" />.
        /// </returns>
        public override uint ReadUInt32()
        {
            return BitConverter.ToUInt32(PrepareBytes(this.ReadBytes(4)), 0);
        }

        /// <summary>
        ///     The read u int 64.
        /// </summary>
        /// <returns>
        ///     The <see cref="ulong" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override ulong ReadUInt64()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}