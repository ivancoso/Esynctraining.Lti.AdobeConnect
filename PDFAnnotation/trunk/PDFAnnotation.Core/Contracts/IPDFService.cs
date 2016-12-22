using Esynctraining.Core.Domain.Entities;

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
        /// The array of ATMarkDTO.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ATMarkDTO[] GetAllMarksForFile(string fileId);

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [Obsolete("Use 'Save' method instead.")]
        void Draw(ATDrawingMarkDTO markDTO);

        /// <summary>
        /// The rectangle draw.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [Obsolete("Use 'Save' method instead.")]
        void Rectangle(ATShapeMarkDTO markDTO);

        /// <summary>
        /// The ellipse draw.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [Obsolete("Use 'Save' method instead.")]
        void Ellipse(ATShapeMarkDTO markDTO);

        /// <summary>
        /// The stamp draw.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        [OperationContract]
        [Obsolete("Use 'Save' method instead.")]
        void Stamp(ATShapeMarkDTO markDTO);

        /// <summary>
        /// The common or custom shape.
        /// </summary>
        /// <param name="markDTO">
        /// The mark DTO.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [Obsolete("Use 'Save' method instead.")]
        void Shape(ATShapeMarkDTO markDTO);

        /// <summary>
        /// Gets supported shape type names
        /// </summary>
        /// <returns>
        /// The array of types.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        string[] GetSupportedTypes();

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="dto">
        /// The text DTO.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [Obsolete("Use 'Save' method instead.")]
        void Text(ATTextItemDTO dto);

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="highlight">
        /// The highlight DTO.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [Obsolete("Use 'Save' method instead.")]
        void Highlight(ATHighlightStrikeOutMarkDTO highlight);

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="strikeOut">
        /// The strikeout DTO.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [Obsolete("Use 'Save' method instead.")]
        void StrikeOut(ATHighlightStrikeOutMarkDTO strikeOut);

        /// <summary>
        /// The delete mark by object with id set.
        /// </summary>
        /// <param name="mark">
        /// The Mark object.
        /// </param>
        /// <returns>
        /// The ATMarkDTO.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ATMarkDTO Delete(ATMarkDTO mark);

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The id.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        string DeleteById(string id);

        /// <summary>
        /// Gets all symbol positions and values from file buffer
        /// </summary>
        /// <param name="fileId">file identifier</param>
        /// <returns>The array of ATPageSymbolsDTO </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ATPageSymbolsDTO[] GetAllSymbolsFromFile(string fileId);

        /// <summary>
        /// Gets all symbol positions and values from specified page in file buffer
        /// </summary>
        /// <param name="fileId">file identifier</param>
        /// <param name="pageIndex">page index</param>
        /// <returns>The array of ATPageSymbolsDTO </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ATPageSymbolsDTO[] GetAllSymbolsFromPage(string fileId, int pageIndex);

        /// <summary>
        /// Add or update common properties.
        /// </summary>
        /// <param name="mark">
        /// The Mark object.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void Save(ATMarkDTO mark);

        /// <summary>
        /// Add or update common properties.
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="rotation">page rotation</param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void RotatePage(string fileId, int pageIndex, int rotation);
        /// <summary>
        ///  Add new page
        /// </summary>
        /// <param name="fileId"></param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void AddNewPage(string fileId);
        /// <summary>
        /// Clear unsaved marks
        /// </summary>
        /// <param name="fileId"></param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void ClearUnsavedMarks(string fileId);


        /// <summary>
        /// Delete pages
        /// </summary>
        /// <param name="fileId">The file id</param>
        /// <param name="pageIndexes">page indexes</param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void DeletePages(string fileId, int[] pageIndexes);
        #endregion
    }
}