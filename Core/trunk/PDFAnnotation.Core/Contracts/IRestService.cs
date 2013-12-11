namespace PDFAnnotation.Core.Contracts
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The RestService interface.
    /// </summary>
    [ServiceContract]
    public interface IRestService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Get full file data.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetFileStream/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetFileStream(string id);

        /// <summary>
        /// Get full file data.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetSWF/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetSWF(string id);

        /// <summary>
        /// Get file data as image.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <param name="pageIndex">page index</param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetPageSWF/{id}/{pageIndex}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetPageSWF(string id, string pageIndex);

        /// <summary>
        /// Get page file data as content disposition file.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <param name="pageIndex">page index</param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetPageStream/{id}/{pageIndex}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetPageStream(string id, string pageIndex);

        /// <summary>
        /// Get updated pdf data.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetUpdatedPDF/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetUpdatedPDF(string id);

        #endregion
    }
}