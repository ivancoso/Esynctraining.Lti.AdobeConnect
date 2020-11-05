namespace Esynctraining.Http.Streaming
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The http multipart.
    /// </summary>
    public class HttpMultipart
    {
        #region Constants

        /// <summary>
        /// The lf.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private const byte LF = (byte)'\n';

        #endregion

        #region Fields

        /// <summary>
        /// The boundary as bytes.
        /// </summary>
        private readonly byte[] boundaryAsBytes;

        /// <summary>
        /// The closing boundary as bytes.
        /// </summary>
        private readonly byte[] closingBoundaryAsBytes;

        /// <summary>
        /// The read buffer.
        /// </summary>
        private readonly HttpMultipartBuffer readBuffer;

        /// <summary>
        /// The request stream.
        /// </summary>
        private readonly MemoryStream requestStream;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMultipart"/> class.
        /// </summary>
        /// <param name="requestStream">
        /// The request stream to parse.
        /// </param>
        /// <param name="boundary">
        /// The boundary marker to look for.
        /// </param>
        public HttpMultipart(Stream requestStream, string boundary)
        {
            // boundary = boundary.StartsWith("--") ? boundary : "--" + boundary;
            this.requestStream = new MemoryStream(this.ToByteArray(requestStream));
            this.boundaryAsBytes = GetBoundaryAsBytes(boundary, false);
            this.closingBoundaryAsBytes = GetBoundaryAsBytes(boundary, true);
            this.readBuffer = new HttpMultipartBuffer(this.boundaryAsBytes, this.closingBoundaryAsBytes);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the <see cref="HttpMultipartBoundary" /> instances from the request stream.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> instance, containing the found <see cref="HttpMultipartBoundary" />
        ///     instances.
        /// </returns>
        public IEnumerable<HttpMultipartBoundary> GetBoundaries()
        {
            return
                (from boundaryStream in this.GetBoundarySubStreams() select new HttpMultipartBoundary(boundaryStream))
                    .ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get boundary as bytes.
        /// </summary>
        /// <param name="boundary">
        /// The boundary.
        /// </param>
        /// <param name="closing">
        /// The closing.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private static byte[] GetBoundaryAsBytes(string boundary, bool closing)
        {
            var boundaryBuilder = new StringBuilder();

            boundaryBuilder.Append("--");
            boundaryBuilder.Append(boundary);

            if (closing)
            {
                boundaryBuilder.Append("--");
            }
            else
            {
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
            }

            byte[] bytes = Encoding.ASCII.GetBytes(boundaryBuilder.ToString());

            return bytes;
        }

        /// <summary>
        /// The check if found end of stream.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckIfFoundEndOfStream()
        {
            return this.requestStream.Position.Equals(this.requestStream.Length);
        }

        /// <summary>
        /// The get actual end of boundary.
        /// </summary>
        /// <param name="boundaryEnd">
        /// The boundary end.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long GetActualEndOfBoundary(long boundaryEnd)
        {
            if (this.CheckIfFoundEndOfStream())
            {
                return this.requestStream.Position - (this.readBuffer.Length + 2);
            }

            return boundaryEnd - (this.readBuffer.Length + 2);
        }

        /// <summary>
        /// The get boundary sub streams.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<HttpMultipartSubStream> GetBoundarySubStreams()
        {
            var boundarySubStreams = new List<HttpMultipartSubStream>();
            long boundaryStart = this.GetNextBoundaryPosition();

            while (this.MultipartIsNotCompleted(boundaryStart))
            {
                long boundaryEnd = this.GetNextBoundaryPosition();
                boundarySubStreams.Add(
                    new HttpMultipartSubStream(
                        this.requestStream, 
                        boundaryStart, 
                        this.GetActualEndOfBoundary(boundaryEnd)));

                boundaryStart = boundaryEnd;
            }

            return boundarySubStreams;
        }

        /// <summary>
        /// The get next boundary position.
        /// </summary>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long GetNextBoundaryPosition()
        {
            this.readBuffer.Reset();
            while (true)
            {
                int byteReadFromStream = this.requestStream.ReadByte();

                if (byteReadFromStream == -1)
                {
                    return -1;
                }

                this.readBuffer.Insert((byte)byteReadFromStream);

                if (this.readBuffer.IsFull && (this.readBuffer.IsBoundary || this.readBuffer.IsClosingBoundary))
                {
                    return this.requestStream.Position;
                }

                if (byteReadFromStream.Equals(LF) || this.readBuffer.IsFull)
                {
                    this.readBuffer.Reset();
                }
            }
        }

        /// <summary>
        /// The multipart is not completed.
        /// </summary>
        /// <param name="boundaryPosition">
        /// The boundary position.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool MultipartIsNotCompleted(long boundaryPosition)
        {
            return boundaryPosition > -1 && !this.readBuffer.IsClosingBoundary;
        }

        /// <summary>
        /// The to byte array.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private byte[] ToByteArray(Stream stream)
        {
            var buffer = new byte[32768];
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        return ms.ToArray();
                    }

                    ms.Write(buffer, 0, read);
                }
            }
        }

        #endregion
    }
}