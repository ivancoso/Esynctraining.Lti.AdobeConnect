namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The highlight strike out mark.
    /// </summary>
    [Serializable]
    public class ATHighlightStrikeOut : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the mark.
        /// </summary>
        public virtual ATMark Mark { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public virtual string Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether item has selection.
        /// </summary>
        public virtual bool HasSelection { get; set; }

        /// <summary>
        /// Gets or sets the selection_info.
        /// </summary>
        public virtual string SelectionInfo { get; set; }

        /// <summary>
        /// Gets or sets the selection_text.
        /// </summary>
        public virtual string SelectionText { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public virtual float Width { get; set; }

        /// <summary>
        /// Gets or sets the position x.
        /// </summary>
        public virtual float PositionX { get; set; }

        /// <summary>
        /// Gets or sets the position y.
        /// </summary>
        public virtual float PositionY { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public virtual float Height { get; set; }

        #endregion
    }
}