﻿namespace Esynctraining.Http.Streaming
{
    using System;
    using System.IO;

    /// <summary>
    ///     The http multipart sub stream.
    /// </summary>
    public class HttpMultipartSubStream : Stream
    {
        #region Fields

        /// <summary>
        /// The end.
        /// </summary>
        private readonly long end;

        /// <summary>
        /// The stream.
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// The position.
        /// </summary>
        private long position;

        /// <summary>
        /// The start.
        /// </summary>
        private long start;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMultipartSubStream"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream to create the sub-stream ontop of.
        /// </param>
        /// <param name="start">
        /// The start offset on the parent stream where the sub-stream should begin.
        /// </param>
        /// <param name="end">
        /// The end offset on the parent stream where the sub-stream should end.
        /// </param>
        public HttpMultipartSubStream(Stream stream, long start, long end)
        {
            this.stream = stream;
            this.start = start;
            this.position = start;
            this.end = end;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns><see langword="true" /> if the stream supports reading; otherwise, <see langword="false" />.</returns>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns><see langword="true" /> if the stream supports seeking; otherwise, <see langword="false" />.</returns>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns><see langword="true" /> if the stream supports writing; otherwise, <see langword="false" />.</returns>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Length
        {
            get
            {
                return this.end - this.start;
            }
        }

        /// <summary>
        ///     When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        ///     The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <filterpriority>1</filterpriority>
        public override long Position
        {
            get
            {
                return this.position - this.start;
            }

            set
            {
                this.position = this.Seek(value, SeekOrigin.Begin);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written
        ///     to the underlying device.
        /// </summary>
        /// <remarks>In the <see cref="HttpMultipartSubStream" /> type this method is implemented as no-op.</remarks>
        public override void Flush()
        {
        }

        /// <summary>
        /// The position start at current location.
        /// </summary>
        public void PositionStartAtCurrentLocation()
        {
            this.start = this.stream.Position;
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position
        ///     within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        ///     many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte array with the
        ///     values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced
        ///     by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read
        ///     from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream. 
        /// </param>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > (this.end - this.position))
            {
                count = (int)(this.end - this.position);
            }

            if (count <= 0)
            {
                return 0;
            }

            this.stream.Position = this.position;

            int bytesReadFromStream = this.stream.Read(buffer, offset, count);

            this.RepositionAfterRead(bytesReadFromStream);

            return bytesReadFromStream;
        }

        /// <summary>
        ///     Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end
        ///     of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        public override int ReadByte()
        {
            if (this.position >= this.end)
            {
                return -1;
            }

            this.stream.Position = this.position;

            int byteReadFromStream = this.stream.ReadByte();

            this.RepositionAfterRead(1);

            return byteReadFromStream;
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <param name="offset">
        /// A byte offset relative to the <paramref name="origin"/> parameter.
        /// </param>
        /// <param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new
        ///     position.
        /// </param>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long subStreamRelativePosition = this.CalculateSubStreamRelativePosition(origin, offset);

            this.ThrowExceptionIsPositionIsOutOfBounds(subStreamRelativePosition);

            this.position = this.stream.Seek(subStreamRelativePosition, SeekOrigin.Begin);

            return this.position;
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">
        /// The desired length of the current stream in bytes.
        /// </param>
        /// <remarks>
        /// This will always throw a <see cref="InvalidOperationException"/> for the
        ///     <see cref="HttpMultipartSubStream"/> type.
        /// </remarks>
        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
        ///     position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from
        ///     <paramref name="buffer"/> to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the
        ///     current stream.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be written to the current stream. 
        /// </param>
        /// <remarks>
        /// This will always throw a <see cref="InvalidOperationException"/> for the
        ///     <see cref="HttpMultipartSubStream"/> type.
        /// </remarks>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The calculate sub stream relative position.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long CalculateSubStreamRelativePosition(SeekOrigin origin, long offset)
        {
            long subStreamRelativePosition = 0L;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    subStreamRelativePosition = this.start + offset;
                    break;

                case SeekOrigin.Current:
                    subStreamRelativePosition = this.position + offset;
                    break;

                case SeekOrigin.End:
                    subStreamRelativePosition = this.end + offset;
                    break;
            }

            return subStreamRelativePosition;
        }

        /// <summary>
        /// The reposition after read.
        /// </summary>
        /// <param name="bytesReadFromStream">
        /// The bytes read from stream.
        /// </param>
        private void RepositionAfterRead(int bytesReadFromStream)
        {
            if (bytesReadFromStream == -1)
            {
                this.position = this.end;
            }
            else
            {
                this.position += bytesReadFromStream;
            }
        }

        /// <summary>
        /// The throw exception is position is out of bounds.
        /// </summary>
        /// <param name="subStreamRelativePosition">
        /// The sub stream relative position.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private void ThrowExceptionIsPositionIsOutOfBounds(long subStreamRelativePosition)
        {
            if (subStreamRelativePosition < 0 || subStreamRelativePosition > this.end)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion
    }
}