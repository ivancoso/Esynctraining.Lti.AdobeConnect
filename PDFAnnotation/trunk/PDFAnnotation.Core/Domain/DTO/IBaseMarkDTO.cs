namespace PDFAnnotation.Core.Domain.DTO
{
    using System;

    /// <summary>
    /// Represents common Mark properties
    /// </summary>
    public interface IBaseMarkDTO
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        string id { get; set; }

        /// <summary>
        /// Gets or sets the page index.
        /// </summary>
        int pageIndex { get; set; }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        float? rotation { get; set; }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        string displayFormat { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        string type { get; set; }

        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        string fileId { get; set; }

        /// <summary>
        /// Gets or sets the date changed.
        /// </summary>
        double datechanged { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        double datecreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether readonly.
        /// </summary>
        bool @readonly { get; set; }

        #endregion
    }
}
