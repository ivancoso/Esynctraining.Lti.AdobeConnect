namespace Esynctraining.Http.Streaming
{
    using System;
    using System.Linq;

    /// <summary>
    ///     The http multipart buffer.
    /// </summary>
    internal sealed class HttpMultipartBuffer
    {
        #region Fields

        /// <summary>
        /// The boundary as bytes.
        /// </summary>
        private readonly byte[] boundaryAsBytes;

        /// <summary>
        /// The buffer.
        /// </summary>
        private readonly byte[] buffer;

        /// <summary>
        /// The closing boundary as bytes.
        /// </summary>
        private readonly byte[] closingBoundaryAsBytes;

        /// <summary>
        /// The position.
        /// </summary>
        private int position;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMultipartBuffer"/> class.
        /// </summary>
        /// <param name="boundaryAsBytes">
        /// The boundary as a byte-array.
        /// </param>
        /// <param name="closingBoundaryAsBytes">
        /// The closing boundary as byte-array
        /// </param>
        public HttpMultipartBuffer(byte[] boundaryAsBytes, byte[] closingBoundaryAsBytes)
        {
            this.boundaryAsBytes = boundaryAsBytes;
            this.closingBoundaryAsBytes = closingBoundaryAsBytes;
            this.buffer = new byte[this.boundaryAsBytes.Length];
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether the buffer contains the same values as the boundary.
        /// </summary>
        /// <value><see langword="true" /> if buffer contains the same values as the boundary; otherwise, <see langword="false" />.</value>
        public bool IsBoundary
        {
            get
            {
                return this.buffer.SequenceEqual(this.boundaryAsBytes);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is closing boundary.
        /// </summary>
        public bool IsClosingBoundary
        {
            get
            {
                return this.buffer.SequenceEqual(this.closingBoundaryAsBytes);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this buffer is full.
        /// </summary>
        /// <value><see langword="true" /> if buffer is full; otherwise, <see langword="false" />.</value>
        public bool IsFull
        {
            get
            {
                return this.position.Equals(this.buffer.Length);
            }
        }

        /// <summary>
        ///     Gets the the number of bytes that can be stored in the buffer.
        /// </summary>
        /// <value>The number of bytes that can be stored in the buffer.</value>
        public int Length
        {
            get
            {
                return this.buffer.Length;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Inserts the specified value into the buffer and advances the internal position.
        /// </summary>
        /// <param name="value">
        /// The value to insert into the buffer.
        /// </param>
        /// <remarks>
        /// This will throw an <see cref="ArgumentOutOfRangeException"/> is you attempt to call insert more times then
        ///     the <see cref="Length"/> of the buffer and <see cref="Reset"/> was not invoked.
        /// </remarks>
        public void Insert(byte value)
        {
            this.buffer[this.position++] = value;
        }

        /// <summary>
        ///     Resets the buffer so that inserts happens from the start again.
        /// </summary>
        /// <remarks>
        ///     This does not clear any previously written data, just resets the buffer position to the start. Data that is
        ///     inserted after Reset has been called will overwrite old data.
        /// </remarks>
        public void Reset()
        {
            this.position = 0;
        }

        #endregion
    }
}