using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFAnnotation.Core.Contracts;

namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The drawing entity.
    /// </summary>
    [Serializable]
    public class ATPicture : Entity
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
        /// Gets or sets the width.
        /// </summary>
        public virtual float Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public virtual float Height { get; set; }

        /// <summary>
        /// Gets or sets the labelText.
        /// </summary>
        public virtual string LabelText { get; set; }
        
        /// <summary>
        /// Gets or sets the label text color.
        /// </summary>
        public virtual string LabelTextColor { get; set; }

        /// <summary>
        /// Gets or sets the label font size.
        /// </summary>
        public virtual int LabelFontSize { get; set; }


        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public virtual byte[] Image { get; set; }


        /// <summary>
        /// Gets or sets the path to the image.
        /// </summary>
        public virtual string Path { get; set; }


        #endregion
    }
}
