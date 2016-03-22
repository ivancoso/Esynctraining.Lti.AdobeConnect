using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Mp4Service.Tasks.Client.Dto;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class SubtitleUtility
    {
        private readonly IAdobeConnectProxy _ac;
        private readonly ILogger _logger;
        private readonly ApiController _controller;


        public SubtitleUtility(IAdobeConnectProxy ac, ILogger logger, ApiController controller)
        {
            if (ac == null)
                throw new ArgumentNullException("ac");
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (controller == null)
                throw new ArgumentNullException("controller");

            _ac = ac;
            _logger = logger;
            _controller = controller;
        }

        public OperationResultWithData<string> AccessMp4File(string scoId, 
            string acDomain,
            string principalId, 
            string breezeToken)
        {
            try
            {
                string url = AccessSco(principalId, breezeToken, acDomain, scoId, _ac, ".mp4");

                return OperationResultWithData<string>.Success(url);
            }
            catch (WarningMessageException ex)
            {
                return OperationResultWithData<string>.Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "AccessMp4File exception. sco-id:{0}. AC: {1}.", scoId, acDomain);

                return OperationResultWithData<string>.Error(ex.Message + ex.StackTrace);
            }
        }

        public OperationResultWithData<string> AccessVttFile(string scoId,
            string acDomain,
            string principalId,
            string breezeToken)
        {
            try
            {
                string url = AccessSco(principalId, breezeToken, acDomain, scoId, _ac, ".html");

                return OperationResultWithData<string>.Success(url);
            }
            catch (WarningMessageException ex)
            {
                return OperationResultWithData<string>.Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "AccessVttFile exception. sco-id:{0}. AC: {1}.", scoId, acDomain);

                return OperationResultWithData<string>.Error(ex.Message + ex.StackTrace);
            }
        }

        public HttpResponseMessage GetVttFile(string principalId, string fileScoId)
        {
            try
            {
                ScoInfo sco = DoGetSco(fileScoId, _ac, principalId);
                FileEntry file = GetOriginalFileContent(sco, _ac);
                if (file == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(file.Content);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(file.FileName));
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = file.FileName;
                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "GetVttFile exception. sco-id:{0}. PrincipalId: {1}.", fileScoId, principalId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        public Task<FileUploadResultDto> PostVttFile(string fileScoId)
        {
            // Check if the request contains multipart/form-data.
            if (!_controller.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            
            var provider = new MultipartMemoryStreamProvider();

            var task = _controller.Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<FileUploadResultDto>(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        _controller.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }

                    foreach (HttpContent file in provider.Contents)
                    {
                        Stream stream = file.ReadAsStreamAsync().Result;

                        FileDto acFile = Create(fileScoId,
                            file.Headers.ContentDisposition.FileName,
                            file.Headers.ContentType.MediaType,
                            stream.ReadFully(),
                            _ac);

                        return new FileUploadResultDto
                        {
                            IsSuccess = true,
                            Message = "OK",
                            Result = new FileDescription
                            {
                                FileId = acFile.Id,
                                FileName = acFile.Name,
                            },
                        };
                    }
                    return new FileUploadResultDto
                    {
                        IsSuccess = false,
                        Message = "No file uploaded",
                    };
                });

            return task;
        }


        private static ScoInfo DoGetSco(string scoId, IAdobeConnectProxy ac, string principalId)
        {
            // check is user already has read permission!!!
            // TODO: setup only if source recording is accessible??
            ac.UpdateScoPermissionForPrincipal(scoId, principalId, MeetingPermissionId.view);

            return ac.GetScoInfo(scoId).ScoInfo;
        }

        private static string AccessSco(string principalId,
            string breezeToken,
            string adobeConnectDomain,
            string scoId,
            IAdobeConnectProxy provider,
            string fileExtention)
        {
            ScoInfo scoInfo = DoGetSco(scoId, provider, principalId);

            var baseUrl = adobeConnectDomain + scoInfo.UrlPath;

            return GetDownloadLink(adobeConnectDomain, provider.GetScoInfo(scoId).ScoInfo.UrlPath.Replace("/", ""), fileExtention) + "&session=" + breezeToken;
        }

        private static string GetDownloadLink(string acServer, string downloadName, string fileExtention)
        {
            string fileName = downloadName.Substring(0, downloadName.Length - 3) + fileExtention;

            return string.Format(
                "{0}/{1}/output/{2}?download={2}",
                acServer.Replace(@"api/xml", string.Empty).Trim('/'),
                downloadName,
                fileName);
        }

        private static FileEntry GetOriginalFileContent(ScoInfo file, IAdobeConnectProxy provider)
        {
            string error;
            byte[] content = provider.GetContentByUrlPath(file.UrlPath, "zip", out error);

            var archive = new ZipArchive(new MemoryStream(content));
            ZipArchiveEntry fileEntry = archive.Entries[0];

            byte[] fileContent;
            using (var memoryStream = new MemoryStream())
            {
                fileEntry.Open().CopyTo(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            return new FileEntry(fileContent, fileEntry.Name);
        }

        private static FileDto Create(string fileScoId, string fileName, string fileContentType, byte[] content, IAdobeConnectProxy ac)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName can't be empty", "fileName");
            if (string.IsNullOrWhiteSpace(fileContentType))
                throw new ArgumentException("fileContentType can't be empty", "fileContentType");
            if (content == null)
                throw new ArgumentNullException("content");

            var uploadScoInfo = new UploadScoInfo
            {
                scoId = fileScoId,
                fileContentType = fileContentType,
                fileName = fileName,
                fileBytes = content,
                title = fileName,
            };

            try
            {
                string originalFileName = fileName;
                StatusInfo uploadResult = ac.UploadContent(uploadScoInfo);
            }
            catch (AdobeConnectException ex)
            {
                // Status.Code: invalid. Status.SubCode: format. Invalid Field: file
                if (ex.Status.Code == StatusCodes.invalid && ex.Status.SubCode == StatusSubCodes.format && ex.Status.InvalidField == "file")
                    throw new Exception("Invalid file format selected.");

                throw new Exception("Error occured during file uploading.", ex);
            }

            return new FileDto
            {
                Id = fileScoId,
                Name = fileName,
                Size = content.Length,
            };
        }

    }

    /// <summary>
    /// Stream extensions
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
