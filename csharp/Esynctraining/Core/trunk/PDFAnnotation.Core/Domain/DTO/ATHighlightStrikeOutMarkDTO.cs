namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The note mark DTO.
    /// </summary>
    [DataContract]
    public class ATHighlightStrikeOutMarkDTO : IBoxedMarkDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATHighlightStrikeOutMarkDTO"/> class.
        /// </summary>
        public ATHighlightStrikeOutMarkDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATHighlightStrikeOutMarkDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The highlight or strikeout.
        /// </param>
        public ATHighlightStrikeOutMarkDTO(ATHighlightStrikeOut item)
        {
            this.displayFormat = item.Mark.DisplayFormat;
            this.pageIndex = item.Mark.PageIndex;
            this.id = item.Mark.Id;
            this.type = item.Mark.Type;
            this.fileId = item.Mark.File.Id;
            this.datechanged = item.Mark.DateChanged;
            this.datecreated = item.Mark.DateCreated;
            this.@readonly = item.Mark.IsReadonly;
            this.rotation = item.Mark.Rotation;
            this.positionX = (float)item.PositionX;
            this.positionY = (float)item.PositionY;
            this.width = (float)item.Width;
            this.height = (float)item.Height;
            this.color = item.Color;

            this.has_selection = item.HasSelection;
            this.selection_info = item.SelectionInfo;
            this.selection_text = item.SelectionText;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether item has selection.
        /// </summary>
        [DataMember]
        public bool has_selection { get; set; }

        /// <summary>
        /// Gets or sets the selection_info.
        /// </summary>
        [DataMember]
        public string selection_info { get; set; }

        /// <summary>
        /// Gets or sets the selection_text.
        /// </summary>
        [DataMember]
        public string selection_text { get; set; }

        #endregion

        #region Base Properties

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public System.Guid id { get; set; }

        /// <summary>
        /// Gets or sets the page index.
        /// </summary>
        [DataMember]
        public int pageIndex { get; set; }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        [DataMember]
        public string displayFormat { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember]
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        [DataMember]
        public Guid fileId { get; set; }

        /// <summary>
        /// Gets or sets the date changed.
        /// </summary>
        [DataMember]
        public System.DateTime datechanged { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public System.DateTime datecreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether readonly.
        /// </summary>
        [DataMember]
        public bool @readonly { get; set; }

        /// <summary>
        /// Gets or sets a rotation.
        /// </summary>
        [DataMember]
        public float? rotation { get; set; }
        #endregion

        #region Boxed Properties

        /// <summary>
        /// Gets or sets the position x.
        /// </summary>
        [DataMember]
        public float positionX { get; set; }

        /// <summary>
        /// Gets or sets the position y.
        /// </summary>
        [DataMember]
        public float positionY { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [DataMember]
        public float width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [DataMember]
        public float height { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        [DataMember]
        public string color { get; set; }

        #endregion
    }
}