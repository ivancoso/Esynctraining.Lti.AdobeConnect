namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The drawing entity.
    /// </summary>
    [Serializable]
    public class ATFormula : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the mark.
        /// </summary>
        public virtual ATMark Mark { get; set; }

        /// <summary>
        /// Gets or sets PositionX
        /// </summary>
        public virtual float PositionX { get; set; }

        /// <summary>
        /// Gets or sets PositionY
        /// </summary>
        public virtual float PositionY { get; set; }

        /// <summary>
        /// Gets or sets the path to the image.
        /// </summary>
        public virtual string Equation { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public virtual float Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public virtual float Height { get; set; }


        #endregion
    }
}
