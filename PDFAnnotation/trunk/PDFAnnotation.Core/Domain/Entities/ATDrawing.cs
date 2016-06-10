namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The drawing entity.
    /// </summary>
    [Serializable]
    public class ATDrawing : Entity
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
        /// Gets or sets the points.
        /// </summary>
        public virtual string Points { get; set; }

        #endregion
    }
}