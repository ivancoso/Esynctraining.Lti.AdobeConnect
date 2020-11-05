namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The note mark entity.
    /// </summary>
    [Serializable]
    public class ATShape : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the mark.
        /// </summary>
        public virtual ATMark Mark { get; set; }

        /// <summary>
        /// Gets or sets the position x.
        /// </summary>
        public virtual float PositionX { get; set; }

        /// <summary>
        /// Gets or sets the position y.
        /// </summary>
        public virtual float PositionY { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public virtual float Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public virtual float Height { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public virtual string Color { get; set; }

        /// <summary>
        /// Gets or sets the inner text.
        /// </summary>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the image encoded by Base64 encoding.
        /// </summary>
        public virtual string Image { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        public virtual string Points { get; set; }

        /// <summary>
        /// Gets or sets the fill color.
        /// </summary>
        public virtual string FillColor { get; set; }

        /// <summary>
        /// Gets or sets the stroke width.
        /// </summary>
        public virtual int StrokeWidth { get; set; }

        /// <summary>
        /// Gets or sets fill opacity.
        /// </summary>
        public virtual float FillOpacity { get; set; }

        /// <summary>
        /// Gets or sets fill opacity.
        /// </summary>
        public virtual float StrokeOpacity { get; set; }

        /// <summary>
        /// Gets or sets the labelText.
        /// </summary>
        public virtual string LabelText { get; set; }

        /// <summary>
        /// Gets or sets the stampColor.
        /// </summary>
        public virtual string StampColor { get; set; }

        /// <summary>
        /// Gets or sets the labelTextColor.
        /// </summary>
        public virtual string LabelTextColor { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        public virtual string Style { get; set; }

        /// <summary>
        /// Gets or sets the numberingTextColor.
        /// </summary>
        public virtual string NumberingTextColor { get; set; }

        #endregion
    }
}