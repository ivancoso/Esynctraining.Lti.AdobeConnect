namespace Esynctraining.AC.Provider
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// The http utils internal.
    /// </summary>
    internal class HttpUtilsInternal
    {
        /// <summary>
        /// Retrieves remote http contents
        /// </summary>
        /// <param name="url">Url to the source</param>
        /// <param name="accessCredentials">optional Credentials</param>
        /// <param name="proxyUrl">option proxy Url</param>
        /// <returns>Http Contents.</returns>
        public static string HttpGetContents(string url, NetworkCredential accessCredentials, string proxyUrl)
        {
            var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;

            if (httpWebRequest == null)
            {
                return null;
            }

            httpWebRequest.Credentials = accessCredentials ?? CredentialCache.DefaultCredentials;

            if (!string.IsNullOrEmpty(proxyUrl))
            {
                httpWebRequest.Proxy = new WebProxy(proxyUrl, true)
                                           {
                                               Credentials = accessCredentials ?? CredentialCache.DefaultCredentials
                                           };
            }

            httpWebRequest.Timeout = 20000 * 60;
            httpWebRequest.Accept = "*/*";
            httpWebRequest.KeepAlive = false;
            httpWebRequest.CookieContainer = new CookieContainer();

            HttpWebResponse httpWebResponse = null;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate
                {
                    return true;
                };

                httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;

                if (httpWebResponse == null)
                {
                    return null;
                }

                var receiveStream = httpWebResponse.GetResponseStream();

                if (receiveStream == null)
                {
                    return null;
                }

                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                httpWebRequest.Abort();
                TraceTool.TraceException(ex);
            }
            finally
            {
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// The url encode.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UrlEncode(string text)
        {
            return UrlEncode(text, Encoding.UTF8);
        }

        /// <summary>
        /// The url encode.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UrlEncode(string text, Encoding encoding)
        {
            if (text == null)
            {
                return null;
            }

            if (text == string.Empty)
            {
                return string.Empty;
            }

            var bytes = encoding.GetBytes(text);

            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, 0, bytes.Length));
        }

        /// <summary>
        /// The url encode to bytes.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The bytes.
        /// </returns>
        public static byte[] UrlEncodeToBytes(string text)
        {
            return UrlEncodeToBytes(text, Encoding.UTF8);
        }

        /// <summary>
        /// The url encode to bytes.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The bytes.
        /// </returns>
        public static byte[] UrlEncodeToBytes(string text, Encoding encoding)
        {
            if (text == null)
            {
                return null;
            }

            if (text == string.Empty)
            {
                return new byte[0];
            }

            var bytes = encoding.GetBytes(text);

            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// The url encode to bytes.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <returns>
        /// The encoded bytes.
        /// </returns>
        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            return bytes.Length == 0 ? new byte[0] : UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// The url encode to bytes.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The encoded bytes.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// on invalid offset or count
        /// </exception>
        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                return null;
            }

            var len = bytes.Length;

            if (len == 0)
            {
                return new byte[0];
            }

            if (offset < 0 || offset >= len)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0 || count > len - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var result = new MemoryStream(count);
            var end = offset + count;

            for (var i = offset; i < end; i++)
            {
                UrlEncodeChar((char)bytes[i], result, false);
            }

            return result.ToArray();
        }

        /// <summary>
        /// The url encode unicode.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UrlEncodeUnicode(string text)
        {
            return text == null ? null : Encoding.ASCII.GetString(UrlEncodeUnicodeToBytes(text));
        }

        /// <summary>
        /// The url encode char.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="isUnicode">
        /// The is unicode.
        /// </param>
        private static void UrlEncodeChar(char symbol, Stream result, bool isUnicode)
        {
            var hexChars = "0123456789abcdef".ToCharArray();
            const string NotEncoded = "!'()*-._";

            if (symbol > 255)
            {
                var i = (int)symbol;

                result.WriteByte((byte)'%');
                result.WriteByte((byte)'u');

                var idx = i >> 12;

                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 8) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 4) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = i & 0x0F;
                result.WriteByte((byte)hexChars[idx]);

                return;
            }

            if (symbol > ' ' && NotEncoded.IndexOf(symbol) != -1)
            {
                result.WriteByte((byte)symbol);
                return;
            }

            if (symbol == ' ')
            {
                result.WriteByte((byte)'+');
                return;
            }

            if ((symbol < '0') || (symbol < 'A' && symbol > '9') || (symbol > 'Z' && symbol < 'a') || (symbol > 'z'))
            {
                if (isUnicode && symbol > 127)
                {
                    result.WriteByte((byte)'%');
                    result.WriteByte((byte)'u');
                    result.WriteByte((byte)'0');
                    result.WriteByte((byte)'0');
                }
                else
                {
                    result.WriteByte((byte)'%');
                }

                var idx = symbol >> 4;

                result.WriteByte((byte)hexChars[idx]);
                idx = symbol & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
            }
            else
            {
                result.WriteByte((byte)symbol);
            }
        }

        /// <summary>
        /// The url encode unicode to bytes.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The bytes.
        /// </returns>
        private static byte[] UrlEncodeUnicodeToBytes(string text)
        {
            if (text == null)
            {
                return null;
            }

            if (text == string.Empty)
            {
                return new byte[0];
            }

            var result = new MemoryStream(text.Length);

            foreach (var c in text)
            {
                UrlEncodeChar(c, result, true);
            }

            return result.ToArray();
        }
    }
}
