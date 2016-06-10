namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// Page rotation entity
    /// </summary>
    [Serializable]
    public class ATRotation : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the mark.
        /// </summary>
        public virtual ATMark Mark { get; set; }

        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        public virtual int Angle { get; set; }

        #endregion
    }
}
