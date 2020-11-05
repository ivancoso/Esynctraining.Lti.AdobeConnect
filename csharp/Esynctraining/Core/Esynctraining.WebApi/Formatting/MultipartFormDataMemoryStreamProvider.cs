using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Esynctraining.WebApi.Formatting
{
    public class MultipartFormDataMemoryStreamProvider : MultipartMemoryStreamProvider
    {
        public sealed class FileContent
        {
            public string ContentType { get; set; }

            public Stream Stream { get; set; }

        }


        private readonly Collection<bool> _isFormData = new Collection<bool>();
        private readonly NameValueCollection _formData = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FileContent> _fileStreams = new Dictionary<string, FileContent>();


        public NameValueCollection FormData
        {
            get { return _formData; }
        }

        public Dictionary<string, FileContent> FileStreams
        {
            get { return _fileStreams; }
        }


        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            var contentDisposition = headers.ContentDisposition;
            if (contentDisposition == null)
            {
                throw new InvalidOperationException("Did not find required 'Content-Disposition' header field in MIME multipart body part.");
            }

            _isFormData.Add(string.IsNullOrEmpty(contentDisposition.FileName));
            return base.GetStream(parent, headers);
        }

        public override async Task ExecutePostProcessingAsync()
        {
            for (var index = 0; index < Contents.Count; index++)
            {
                HttpContent formContent = Contents[index];
                if (_isFormData[index])
                {
                    // Field
                    string formFieldName = UnquoteToken(formContent.Headers.ContentDisposition.Name) ?? string.Empty;
                    string formFieldValue = await formContent.ReadAsStringAsync();
                    FormData.Add(formFieldName, formFieldValue);
                }
                else
                {
                    // File
                    string fileName = UnquoteToken(formContent.Headers.ContentDisposition.FileName);
                    Stream stream = await formContent.ReadAsStreamAsync();
                    FileStreams.Add(fileName, new FileContent { ContentType = formContent.Headers.ContentType.MediaType, Stream = stream });
                }
            }
        }


        private static string UnquoteToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }

    }

}
