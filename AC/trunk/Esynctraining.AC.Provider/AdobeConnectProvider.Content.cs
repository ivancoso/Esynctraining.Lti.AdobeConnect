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

        /// <summary>
        /// The get content by url path.
        /// </summary>
        /// <param name="urlPath">
        /// The url path.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte[] GetContentByUrlPath(string urlPath, string format, out string error)
        {
            var downloadName = urlPath.Trim('/');
            return this.requestProcessor.DownloadData(downloadName, format, out error);
        }

        public byte[] GetContentByUrlPath2(string urlPath, string fileName, out string error)
        {
            var downloadName = urlPath.Trim('/');
            return this.requestProcessor.DownloadData2(downloadName, fileName, out error);
        }

        public byte[] GetSourceContentByUrlPath(string urlPath, string fileName, out string error)
        {
            var downloadName = urlPath.Trim('/');
            return this.requestProcessor.DownloadSourceData(downloadName, fileName, out error);
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
                string.Format(CommandParams.ScoUpload, uploadScoInfo.scoId, uploadScoInfo.summary, uploadScoInfo.title), uploadScoInfo, out status);
            return status;
        }

    }

}
