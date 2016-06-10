using Esynctraining.Core.Extensions;

namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The note mark DTO.
    /// </summary>
    [DataContract]
    public class ATShapeMarkDTO : IBoxedMarkDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATShapeMarkDTO"/> class.
        /// </summary>
        public ATShapeMarkDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATShapeMarkDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The note.
        /// </param>
        public ATShapeMarkDTO(ATShape item)
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

            this.image = item.Image;
            this.points = item.Points;
            this.positionX = item.PositionX;
            this.positionY = item.PositionY;
            this.width = item.Width;
            this.height = item.Height;

            this.color = item.Color;
            this.text = item.Text;
            this.fillColor = item.FillColor;
            this.fillOpacity = item.FillOpacity;
            this.strokeOpacity = item.StrokeOpacity;
            this.strokeWidth = item.StrokeWidth;

            this.labelText = item.LabelText;
            this.labelTextColor = item.LabelTextColor;
            this.numberingTextColor = item.NumberingTextColor;
            this.stampColor = item.StampColor;
            this.style = item.Style;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the inner text.
        /// </summary>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the image (Base64 encoded).
        /// </summary>
        [DataMember]
        public string image { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        [DataMember]
        public string points { get; set; }

        /// <summary>
        /// Gets or sets the fill color.
        /// </summary>
        [DataMember]
        public string fillColor { get; set; }

        /// <summary>
        /// Gets or sets the stroke width.
        /// </summary>
        [DataMember]
        public int strokeWidth { get; set; }

        /// <summary>
        /// Gets or sets fill opacity.
        /// </summary>
        [DataMember]
        public float fillOpacity { get; set; }

        /// <summary>
        /// Gets or sets fill opacity.
        /// </summary>
        [DataMember]
        public float strokeOpacity { get; set; }

        /// <summary>
        /// Gets or sets the labelText.
        /// </summary>
        [DataMember]
        public string labelText { get; set; }

        /// <summary>
        /// Gets or sets the stampColor.
        /// </summary>
        [DataMember]
        public string stampColor { get; set; }

        /// <summary>
        /// Gets or sets the labelTextColor.
        /// </summary>
        [DataMember]
        public string labelTextColor { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        [DataMember]
        public string style { get; set; }

        /// <summary>
        /// Gets or sets the numberingTextColor.
        /// </summary>
        [DataMember]
        public string numberingTextColor { get; set; }

        #endregion

        #region Base Properties

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public string id { get; set; }

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
        public string fileId { get; set; }

        /// <summary>
        /// Gets or sets the date changed.
        /// </summary>
        [DataMember]
        public double datechanged { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double datecreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether readonly.
        /// </summary>
        [DataMember]
        public bool @readonly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating rotation.
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