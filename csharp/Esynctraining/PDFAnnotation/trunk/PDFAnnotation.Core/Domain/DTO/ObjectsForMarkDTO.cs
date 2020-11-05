using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PDFAnnotation.Core.Business.Models;
using PDFAnnotation.Core.Domain.Entities;

namespace PDFAnnotation.Core.Domain.DTO
{
    /// <summary>
    ///     The objects for mark DTO.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ObjectsForMarkDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectsForMarkDTO"/> class.
        /// </summary>
        public ObjectsForMarkDTO()
        {
            this.shapes = new List<ATShape>();
            this.drawings = new List<ATDrawing>();
            this.atHighlightStrikeOuts = new List<ATHighlightStrikeOut>();
            this.atTextItems = new List<ATTextItem>();
            this.atRotations = new List<ATRotation>();
            this.atPictures = new List<ATPicture>();
            this.atFormulas = new List<ATFormula>();
            this.atAnnotations = new List<ATAnnotation>();
        }


        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the shapes.
        /// </summary>
        [DataMember]
        public List<ATShape> shapes { get; set; }

        /// <summary>
        ///     Gets or sets the drawings.
        /// </summary>
        [DataMember]
        public List<ATDrawing> drawings { get; set; }

        /// <summary>
        ///     Gets or sets the atHighlightStrikeOuts.
        /// </summary>
        [DataMember]
        public List<ATHighlightStrikeOut> atHighlightStrikeOuts { get; set; }

        /// <summary>
        ///     Gets or sets the atTextItems.
        /// </summary>
        [DataMember]
        public List<ATTextItem> atTextItems { get; set; }

        /// <summary>
        ///     Gets or sets the atRotations.
        /// </summary>
        [DataMember]
        public List<ATRotation> atRotations { get; set; }

        /// <summary>
        ///     Gets or sets the atPictures.
        /// </summary>
        [DataMember]
        public List<ATPicture> atPictures { get; set; }

        /// <summary>
        ///     Gets or sets the atFormulas.
        /// </summary>
        [DataMember]
        public List<ATFormula> atFormulas { get; set; }

        /// <summary>
        ///     Gets or sets the atAnnotations.
        /// </summary>
        [DataMember]
        public List<ATAnnotation> atAnnotations { get; set; }

        #endregion
    }
}
