namespace PDFAnnotation.Core.Contracts
{
    using System;
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Contracts;

    using PDFAnnotation.Core.Domain.DTO;

    /// <summary>
    ///     The AccountService interface.
    /// </summary>
    [ServiceContract]
    public interface IPDFService
    {
        #region Public Methods and Operators
        /// <summary>
        /// The get all marks for file.
        /// </summary>
        /// <param name="fileId">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ATMarkDTO> GetAllMarksForFile(Guid fileId);

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        ServiceResponse Draw(ATDrawingMarkDTO markDTO);

        /// <summary>
        /// The rectangle draw.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        ServiceResponse Rectangle(ATShapeMarkDTO markDTO);

        /// <summary>
        /// The ellipse draw.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        ServiceResponse Ellipse(ATShapeMarkDTO markDTO);

        /// <summary>
        /// The stamp draw.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        ServiceResponse Stamp(ATShapeMarkDTO markDTO);

        /// <summary>
        /// The common or custom shape.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        ServiceResponse Shape(ATShapeMarkDTO markDTO);

        /// <summary>
        /// Gets supported shape type names
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<string> GetSupportedTypes();

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="dto">
        /// The text DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        ServiceResponse Text(ATTextItemDTO dto);

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="highlight">
        /// The highlight DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        ServiceResponse Highlight(ATHighlightStrikeOutMarkDTO highlight);

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="strikeOut">
        /// The strikeout DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        ServiceResponse StrikeOut(ATHighlightStrikeOutMarkDTO strikeOut);

        /// <summary>
        /// The delete mark by object with id set.
        /// </summary>
        /// <param name="mark">
        /// The Mark object.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ATMarkDTO> Delete(ATMarkDTO mark);

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<Guid> DeleteById(Guid id);

        /// <summary>
        /// Gets all symbol positions and values from file buffer
        /// </summary>
        /// <param name="fileId">file identifier</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        [OperationContract]
        ServiceResponse<ATPageSymbolsDTO> GetAllSymbolsFromFile(Guid fileId);

        /// <summary>
        /// Gets all symbol positions and values from specified page in file buffer
        /// </summary>
        /// <param name="fileId">file identifier</param>
        /// <param name="pageIndex">page index</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        [OperationContract]
        ServiceResponse<ATPageSymbolsDTO> GetAllSymbolsFromPage(Guid fileId, int pageIndex);

        /// <summary>
        /// Add or update common properties.
        /// </summary>
        /// <param name="mark">
        /// The Mark object.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse Save(ATMarkDTO mark);

        /// <summary>
        /// Add or update common properties.
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="rotation">page rotation</param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse RotatePage(Guid fileId, int pageIndex, int rotation);

        [OperationContract]
        ServiceResponse AddNewPage(Guid fileId);

        [OperationContract]
        ServiceResponse ClearUnsavedMarks(Guid fileId);
        #endregion
    }
}