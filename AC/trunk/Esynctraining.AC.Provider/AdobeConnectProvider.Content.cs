using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        /// <summary>
        /// The get contents by SCO id.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte[] GetContent(string scoId, out string error, string format = "zip")
        {
            var res = this.GetScoInfo(scoId);
            if (res.Success && res.ScoInfo != null && !string.IsNullOrEmpty(res.ScoInfo.UrlPath))
            {
                return this.GetContentByUrlPath(res.ScoInfo.UrlPath, format, out error);
            }

            error = res.Status == null
                        ? "Result is null"
                        : (string.IsNullOrWhiteSpace(res.Status.InnerXml)
                               ? res.Status.UnderlyingExceptionInfo.ToString()
                               : res.Status.InnerXml);
            return null;
        }

        public byte[] GetContentByUrlPath(string urlPath, string format, out string error)
        {
            if (string.IsNullOrEmpty(urlPath))
                throw new ArgumentException("Non-empty value expected", nameof(urlPath));
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("Non-empty value expected", nameof(format));

            return this.requestProcessor.DownloadData(urlPath, format, out error);
        }

        public byte[] GetContentByUrlPath2(string urlPath, string fileName, out string error)
        {
            if (string.IsNullOrEmpty(urlPath))
                throw new ArgumentException("Non-empty value expected", nameof(urlPath));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Non-empty value expected", nameof(fileName));

            return this.requestProcessor.DownloadData2(urlPath, fileName, out error);
        }

        public byte[] GetSourceContentByUrlPath(string urlPath, string fileName, out string error)
        {
            if (string.IsNullOrEmpty(urlPath))
                throw new ArgumentException("Non-empty value expected", nameof(urlPath));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Non-empty value expected", nameof(fileName));

            return this.requestProcessor.DownloadSourceData(urlPath, fileName, out error);
        }

        public byte[] GetSourceContentByUrlPath2(string urlPath, string format, out string error)
        {
            if (string.IsNullOrEmpty(urlPath))
                throw new ArgumentException("Non-empty value expected", nameof(urlPath));
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("Non-empty value expected", nameof(format));

            return this.requestProcessor.DownloadSourceData2(urlPath, format, out error);
        }

        /// <summary>
        /// Uploads contents by SCO id.
        /// </summary>
        /// <param name="uploadScoInfo">
        /// The upload SCO Info.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public StatusInfo UploadContent(UploadScoInfo uploadScoInfo)
        {
            StatusInfo status;
            this.requestProcessor.ProcessUpload(Commands.Sco.Upload, 
                string.Format(CommandParams.ScoUpload, uploadScoInfo.scoId, UrlEncode(uploadScoInfo.summary), UrlEncode(uploadScoInfo.title)), uploadScoInfo, out status);
            return status;
        }

    }

}
