namespace EdugameCloud.WCFService.Contracts
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The uploader interface.
    /// </summary>
    [ServiceContract]
    public interface IUploader
    {
        #region Public Methods and Operators

        /// <summary>
        /// The upload multipart.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "uploadMultipart?fileId={fileId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        FilesUploadedResultDTO uploadMultipart(string fileId, Stream stream);

        #endregion
    }
}