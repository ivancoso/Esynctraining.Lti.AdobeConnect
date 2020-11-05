namespace Esynctraining.PdfProcessor.Tests
{
    using System;
    using System.IO;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The PDF processor tests.
    /// </summary>
    [TestClass]
    public class PdfProcessorTests
    {
        /// <summary>
        /// The flush resource to file.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public static void FlushResourceToFile(string resourceName, string fileName)
        {
            var ums = (UnmanagedMemoryStream)Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (ums != null)
            {
                using (ums)
                {
                    using (var fs = new FileStream(fileName, FileMode.Create))
                    {
                        var buf = new byte[ums.Length];
                        ums.Read(buf, 0, (int)ums.Length);
                        fs.Write(buf, 0, (int)ums.Length);
                    }    
                }
            }
        }

        /// <summary>
        /// The flush resource to file.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public static byte[] GetResourceBytes(string resourceName)
        {
            var ums = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (ums == null)
            {
                return null;
            }

            return ReadToEnd(ums);
        }

        /// <summary>
        /// The read to end.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}