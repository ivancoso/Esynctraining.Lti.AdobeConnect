namespace EdugameCloud.WCFService.Streaming
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class HttpMultipart
    {
        private const byte LF = (byte)'\n';
        private readonly byte[] boundaryAsBytes;
        private readonly HttpMultipartBuffer readBuffer;
        private readonly MemoryStream requestStream;
        private readonly byte[] closingBoundaryAsBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMultipart"/> class.
        /// </summary>
        /// <param name="requestStream">The request stream to parse.</param>
        /// <param name="boundary">The boundary marker to look for.</param>
        public HttpMultipart(Stream requestStream, string boundary)
        {
//            boundary = boundary.StartsWith("--") ? boundary : "--" + boundary;
            this.requestStream = new MemoryStream(this.ToByteArray(requestStream));
            this.boundaryAsBytes = GetBoundaryAsBytes(boundary, false);
            this.closingBoundaryAsBytes = GetBoundaryAsBytes(boundary, true);
            this.readBuffer = new HttpMultipartBuffer(this.boundaryAsBytes, this.closingBoundaryAsBytes);
        }

        /// <summary>
        /// Gets the <see cref="HttpMultipartBoundary"/> instances from the request stream.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing the found <see cref="HttpMultipartBoundary"/> instances.</returns>
        public IEnumerable<HttpMultipartBoundary> GetBoundaries()
        {
            return
                (from boundaryStream in this.GetBoundarySubStreams()
                 select new HttpMultipartBoundary(boundaryStream)).ToList();
        }

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

            var bytes =
                Encoding.ASCII.GetBytes(boundaryBuilder.ToString());

            return bytes;
        }

        private IEnumerable<HttpMultipartSubStream> GetBoundarySubStreams()
        {
            var boundarySubStreams = new List<HttpMultipartSubStream>();
            var boundaryStart = this.GetNextBoundaryPosition();

            while (this.MultipartIsNotCompleted(boundaryStart))
            {
                var boundaryEnd = this.GetNextBoundaryPosition();
                boundarySubStreams.Add(new HttpMultipartSubStream(
                    this.requestStream,
                    boundaryStart,
                    this.GetActualEndOfBoundary(boundaryEnd)));

                boundaryStart = boundaryEnd;
            }

            return boundarySubStreams;
        }

        private bool MultipartIsNotCompleted(long boundaryPosition)
        {
            return boundaryPosition > -1 && !this.readBuffer.IsClosingBoundary;
        }

        //we add two because or the \r\n before the boundary
        private long GetActualEndOfBoundary(long boundaryEnd)
        {
            if (this.CheckIfFoundEndOfStream())
            {
                return this.requestStream.Position - (this.readBuffer.Length + 2);
            }
            return boundaryEnd - (this.readBuffer.Length + 2);
        }

        private bool CheckIfFoundEndOfStream()
        {
            return this.requestStream.Position.Equals(this.requestStream.Length);
        }

        private long GetNextBoundaryPosition()
        {
            this.readBuffer.Reset();
            while (true)
            {
                var byteReadFromStream = this.requestStream.ReadByte();

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

        private byte[] ToByteArray(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
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
    }
}