namespace Esynctraining.Http.Streaming
{
    using System;
    using System.IO;

    /// <summary>
    ///     The upload stream.
    /// </summary>
    public class UploadStream : Stream
    {
        #region Fields

        /// <summary>
        ///     The length.
        /// </summary>
        private readonly long length;

        /// <summary>
        ///     The left to read.
        /// </summary>
        private long leftToRead;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadStream"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        public UploadStream(long length)
        {
            this.length = length;
            this.leftToRead = length;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether can read.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether can seek.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether can write.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets the length.
        /// </summary>
        public override long Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///     Not supported in readonly stream
        /// </exception>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The flush.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var toReturn = (int)Math.Min(this.leftToRead, count);
            this.leftToRead -= toReturn;
            return toReturn;
        }

        /// <summary>
        /// The seek.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Not supported in readonly stream
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The set length.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Not supported in readonly stream
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Not supported in readonly stream
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    /// <summary>
    ///     Stream extensions
    /// </summary>
    public static class StreamExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Counts bytes in stream
        /// </summary>
        /// <param name="stream">
        /// The stream to count bytes from
        /// </param>
        /// <returns>
        /// The count of bytes
        /// </returns>
        public static long CountBytes(this Stream stream)
        {
            var buffer = new byte[100000];
            int bytesRead;
            long totalBytesRead = 0;
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                totalBytesRead += bytesRead;
            }
            while (bytesRead > 0);
            return totalBytesRead;
        }

        /// <summary>
        /// The read fully.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] ReadFully(this Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        #endregion
    }
}