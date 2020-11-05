namespace PDFAnnotation.Core.Domain.DTO
{
    /// <summary>
    /// Represents items drawn in rectangle boxes
    /// </summary>
    public interface IBoxedMarkDTO : IBaseMarkDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the position x.
        /// </summary>
        float positionX { get; set; }

        /// <summary>
        /// Gets or sets the position y.
        /// </summary>
        float positionY { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        float width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        float height { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        string color { get; set; }

        #endregion
    }
}
