namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The note mark DTO.
    /// </summary>
    [DataContract]
    public class ATDrawingMarkDTO : IBaseMarkDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATDrawingMarkDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The drawing.
        /// </param>
        public ATDrawingMarkDTO(ATDrawing item)
        {
            this.displayFormat = item.Mark.DisplayFormat;
            this.pageIndex = item.Mark.PageIndex;
            this.id = item.Mark.Id;
            this.type = item.Mark.Type;
            this.fileId = item.Mark.File.Id;
            this.datechanged = item.Mark.DateChanged;
            this.datecreated = item.Mark.DateCreated;
            this.@readonly = item.Mark.IsReadonly;

            this.color = item.Color;
            this.points = item.Points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATDrawingMarkDTO"/> class.
        /// </summary>
        public ATDrawingMarkDTO()
        {
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        [DataMember]
        public string color { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        [DataMember]
        public string points { get; set; }

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
        public int fileId { get; set; }

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

        #endregion
    }
}