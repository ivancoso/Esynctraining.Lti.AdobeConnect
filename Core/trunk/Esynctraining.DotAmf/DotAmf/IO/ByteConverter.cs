// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="ByteConverter.cs">
//   
// </copyright>
// <summary>
//   Prepare bytes after reading or before writing from/to a stream.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.IO
{
    using System;

    /// <summary>
    ///     Prepare bytes after reading or before writing from/to a stream.
    /// </summary>
    /// <param name="bytes">Bytes to convert.</param>
    /// <returns>Converted bytes.</returns>
    internal delegate byte[] PrepareBytes(byte[] bytes);

    /// <summary>
    ///     Byte converter.
    /// </summary>
    internal static class ByteConverter
    {
        #region Public Properties

        /// <summary>
        ///     Indicates the byte order in which data is stored in this computer architecture.
        /// </summary>
        public static bool IsLittleEndian
        {
            get
            {
                return BitConverter.IsLittleEndian;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Change bytes endianness to opposite.
        /// </summary>
        /// <param name="bytes">
        /// Bytes in big/little endian order.
        /// </param>
        /// <returns>
        /// Bytes in little/big endian order.
        /// </returns>
        public static byte[] ChangeEndianness(byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// A placeholder which doesn't perform any byte convertions.
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] KeepEndianness(byte[] bytes)
        {
            return bytes;
        }

        #endregion
    }
}