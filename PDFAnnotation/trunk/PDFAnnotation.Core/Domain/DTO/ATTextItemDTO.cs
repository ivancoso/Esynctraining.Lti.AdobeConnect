namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    /// <summary>
    ///     The TextItem data transfer object.
    /// </summary>
    [DataContract]
    public class ATTextItemDTO : IBoxedMarkDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ATTextItemDTO" /> class.
        /// </summary>
        public ATTextItemDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATTextItemDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The textItem.
        /// </param>
        public ATTextItemDTO(ATTextItem item)
        {
            this.displayFormat = item.Mark.DisplayFormat;
            this.pageIndex = item.Mark.PageIndex;
            this.id = item.Mark.Id.ToString();
            this.type = item.Mark.Type;
            this.fileId = item.Mark.File.Id.ToString();
            this.datechanged = item.Mark.DateChanged.With(x => x.ConvertToTimestamp());
            this.datecreated = item.Mark.DateCreated.With(x => x.ConvertToTimestamp());
            this.@readonly = item.Mark.IsReadonly;
            this.rotation = item.Mark.Rotation;

            this.positionX = (float)item.PositionX;
            this.positionY = (float)item.PositionY;
            this.width = (float)item.Width;
            this.height = (float)item.Height;
            this.color = item.Color;

            this.text = item.Text;
            this.fontName = item.FontName;
            this.fontFamily = item.FontFamily;
            this.fontSize = item.FontSize;
            this.alignment = item.Alignment;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        [DataMember]
        public float? rotation { get; set; }

        /// <summary>
        ///     Gets or sets text alignment
        /// </summary>
        [DataMember]
        public string alignment { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        [DataMember]
        public string color { get; set; }

        /// <summary>
        ///     Gets or sets the date changed.
        /// </summary>
        [DataMember]
        public double datechanged { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double datecreated { get; set; }

        /// <summary>
        ///     Gets or sets the display format.
        /// </summary>
        [DataMember]
        public string displayFormat { get; set; }

        /// <summary>
        ///     Gets or sets the file id.
        /// </summary>
        [DataMember]
        public string fileId { get; set; }

        /// <summary>
        ///     Gets or sets FontFamily
        /// </summary>
        [DataMember]
        public string fontFamily { get; set; }

        /// <summary>
        ///     Gets or sets FontName
        /// </summary>
        [DataMember]
        public string fontName { get; set; }

        /// <summary>
        ///     Gets or sets FontSize
        /// </summary>
        [DataMember]
        public int fontSize { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        [DataMember]
        public float height { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        ///     Gets or sets the page index.
        /// </summary>
        [DataMember]
        public int pageIndex { get; set; }

        /// <summary>
        ///     Gets or sets the position x.
        /// </summary>
        [DataMember]
        public float positionX { get; set; }

        /// <summary>
        ///     Gets or sets the position y.
        /// </summary>
        [DataMember]
        public float positionY { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether readonly.
        /// </summary>
        [DataMember]
        public bool @readonly { get; set; }

        /// <summary>
        ///     Gets or sets Text
        /// </summary>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [DataMember]
        public string type { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        [DataMember]
        public float width { get; set; }

        #endregion
    }
}