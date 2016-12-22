using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Domain.Entities;
using PDFAnnotation.Core.Contracts;
using PDFAnnotation.Core.Domain.Entities;

namespace PDFAnnotation.Core.Business.Models
{
    /// <summary>
    /// The drawing entity.
    /// </summary>
    [Serializable]
    public class ATAnnotation : Entity
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
        public virtual string Comment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is opened.
        /// </summary>
        public virtual bool IsOpened { get; set; }

        /// <summary>
        /// Gets or sets the icon name.
        /// </summary>
        public virtual string IconName { get; set; }

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
        /// Gets or sets fill opacity.
        /// </summary>
        public virtual float? FillOpacity { get; set; }

        /// <summary>
        /// Gets or sets createdBy.
        /// </summary>
        public virtual string CreatedBy { get; set; }

        #endregion

    }
}
