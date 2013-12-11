namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The mark DTO.
    /// </summary>
    [DataContract]
    public class ATMarkDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ATMarkDTO" /> class.
        /// </summary>
        public ATMarkDTO()
        {
        }

        #region From Entity

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="mark">
        /// The mark.
        /// </param>
        public ATMarkDTO(ATMark mark)
        {
            this.fileId = mark.File.Id;
            this.datechanged = mark.DateChanged;
            this.datecreated = mark.DateCreated;
            this.displayFormat = mark.DisplayFormat;
            this.id = mark.Id;
            this.pageIndex = mark.PageIndex;
            this.@readonly = mark.IsReadonly;
            this.type = mark.Type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="atDrawing">
        /// The ATDrawing.
        /// </param>
        public ATMarkDTO(ATDrawing atDrawing) : this(atDrawing.Mark)
        {
            this.color = atDrawing.Color;
            this.points = atDrawing.Points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="note">
        /// The note.
        /// </param>
        public ATMarkDTO(ATShape note) : this(note.Mark)
        {
            this.color = note.Color;
            this.positionX = note.PositionX;
            this.positionY = note.PositionY;
            this.width = note.Width;
            this.height = note.Height;
            this.points = note.Points;
            this.fillColor = note.FillColor;
            this.fillOpacity = note.FillOpacity;
            this.strokeOpacity = note.StrokeOpacity;
            this.strokeWidth = note.StrokeWidth;

            this.labelText = note.LabelText;
            this.labelTextColor = note.LabelTextColor;
            this.numberingTextColor = note.NumberingTextColor;
            this.stampColor = note.StampColor;
            this.style = note.Style;

            this.text = note.Text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The entity item.
        /// </param>
        public ATMarkDTO(ATTextItem item)
            : this(item.Mark)
        {
            this.color = item.Color;
            this.text = item.Text;
            this.positionX = (float)item.PositionX;
            this.positionY = (float)item.PositionY;
            this.width = (float)item.Width;
            this.height = (float)item.Height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="hs">
        /// The highlight or strikeout.
        /// </param>
        public ATMarkDTO(ATHighlightStrikeOut hs) 
            : this(hs.Mark)
        {
            this.color = hs.Color;
            this.has_selection = hs.HasSelection;
            this.selection_info = hs.SelectionInfo;
            this.selection_text = hs.SelectionText;
            this.width = hs.Width;
            this.height = hs.Height;
            this.positionX = hs.PositionX;
            this.positionY = hs.PositionY;
        }

        #endregion

        #region From DTO

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="mark">
        /// The mark.
        /// </param>
        public ATMarkDTO(IBaseMarkDTO mark)
        {
            this.fileId = mark.fileId;
            this.datechanged = mark.datechanged;
            this.datecreated = mark.datecreated;
            this.displayFormat = mark.displayFormat;
            this.id = mark.id;
            this.pageIndex = mark.pageIndex;
            this.@readonly = mark.@readonly;
            this.type = mark.type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public ATMarkDTO(ATShapeMarkDTO item)
            : this((IBaseMarkDTO)item)
        {
            this.color = item.color;
            this.text = item.text;
            this.positionX = item.positionX;
            this.positionY = item.positionY;
            this.width = item.width;
            this.height = item.height;
            this.points = item.points;
            this.fillColor = item.fillColor;
            this.fillOpacity = item.fillOpacity;
            this.strokeOpacity = item.strokeOpacity;
            this.strokeWidth = item.strokeWidth;

            this.labelText = item.labelText;
            this.labelTextColor = item.labelTextColor;
            this.numberingTextColor = item.numberingTextColor;
            this.stampColor = item.stampColor;
            this.style = item.style;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public ATMarkDTO(ATDrawingMarkDTO item)
            : this((IBaseMarkDTO)item)
        {
            this.color = item.color;
            this.points = item.points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public ATMarkDTO(ATTextItemDTO item)
            : this((IBaseMarkDTO)item)
        {
            this.color = item.color;
            this.text = item.text;
            this.positionX = item.positionX;
            this.positionY = item.positionY;
            this.width = item.width;
            this.height = item.height;
            this.fontSize = item.fontSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkDTO"/> class.
        /// </summary>
        /// <param name="item">
        /// The highlight or strikeout.
        /// </param>
        public ATMarkDTO(ATHighlightStrikeOutMarkDTO item)
            : this((IBaseMarkDTO)item)
        {
            this.color = item.color;
            this.positionX = item.positionX;
            this.positionY = item.positionY;
            this.width = item.width;
            this.height = item.height;

            this.has_selection = item.has_selection;
            this.selection_info = item.selection_info;
            this.selection_text = item.selection_text;
        }

        #endregion

        #endregion

        #region Public Properties

        #region BaseMarkDTO

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public Guid id { get; set; }

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
        ///     Gets or sets the date changed.
        /// </summary>
        [DataMember]
        public DateTime datechanged { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime datecreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether readonly.
        /// </summary>
        [DataMember]
        public bool @readonly { get; set; }

        #endregion

        #region BoxedMarkDTO 

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
        /// Gets or sets a value indicating whether item has selection.
        /// </summary>
        [DataMember]
        public bool has_selection { get; set; }

        /// <summary>
        /// Gets or sets the selection_text.
        /// </summary>
        [DataMember]
        public string selection_text { get; set; }

        /// <summary>
        /// Gets or sets the selection_info.
        /// </summary>
        [DataMember]
        public string selection_info { get; set; }

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
        /// Gets or sets the rotation (page or object).
        /// </summary>
        [DataMember]
        public int rotation { get; set; }

        /// <summary>
        /// Gets or sets the labelText.
        /// </summary>
        [DataMember]
        public string labelText{ get; set; }

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

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        [DataMember]
        public int fontSize { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts <see cref="ATMarkDTO"/> to requested entity type
        /// </summary>
        /// <param name="mark">the mark</param>
        /// <param name="existing">existing item</param>
        /// <returns>corresponding entity</returns>
        public ATMark ToMark(ATMarkDTO mark, ATMark existing)
        {
            var o = existing ?? new ATMark();
            o.IsReadonly = mark.@readonly;
            o.PageIndex = mark.pageIndex;
            o.Rotation = mark.rotation;
            o.DisplayFormat = mark.displayFormat;
            o.Type = mark.type;
            return o;
        }

        /// <summary>
        /// Converts <see cref="ATMarkDTO"/> to requested entity type
        /// </summary>
        /// <param name="mark">the mark</param>
        /// <param name="existing">existing item</param>
        /// <returns>corresponding entity</returns>
        public ATShape ToShape(ATMarkDTO mark, ATShape existing)
        {
            var o = existing ?? new ATShape();
            o.Mark = this.ToMark(mark, null != existing ? existing.Mark : null);
            o.Color = mark.color;
            o.FillColor = mark.fillColor;
            o.FillOpacity = mark.fillOpacity;
            o.Height = mark.height;
            o.Image = mark.image;
            o.Points = mark.points;
            o.PositionX = mark.positionX;
            o.PositionY = mark.positionY;
            o.StrokeOpacity = mark.strokeOpacity;
            o.StrokeWidth = mark.strokeWidth;
            o.Text = mark.text;
            o.Width = mark.width;

            o.LabelText = mark.labelText;
            o.LabelTextColor = mark.labelTextColor;
            o.NumberingTextColor = mark.numberingTextColor;
            o.StampColor = mark.stampColor;
            o.Style = mark.style;
            return o;
        }

        /// <summary>
        /// Converts <see cref="ATMarkDTO"/> to requested entity type
        /// </summary>
        /// <param name="mark">the mark</param>
        /// <param name="existing">existing item</param>
        /// <returns>corresponding entity</returns>
        public ATHighlightStrikeOut ToHighlightStrikeOut(ATMarkDTO mark, ATHighlightStrikeOut existing)
        {
            var o = existing ?? new ATHighlightStrikeOut();
            o.Color = mark.color;
            o.Mark = this.ToMark(mark, null != existing ? existing.Mark : null);
            o.Height = mark.height;
            o.PositionX = mark.positionX;
            o.PositionY = mark.positionY;
            o.Width = mark.width;
            o.HasSelection = mark.has_selection;
            o.SelectionInfo = mark.selection_info;
            o.SelectionText = mark.selection_text;
            return o;
        }

        /// <summary>
        /// Converts <see cref="ATMarkDTO"/> to requested entity type
        /// </summary>
        /// <param name="mark">the mark</param>
        /// <param name="existing">existing item</param>
        /// <returns>corresponding entity</returns>
        public ATTextItem ToTextItem(ATMarkDTO mark, ATTextItem existing)
        {
            var o = existing ?? new ATTextItem();
            o.Mark = this.ToMark(mark, null != existing ? existing.Mark : null);
            o.Color = mark.color;
            o.Text = mark.text;
            o.Height = mark.height;
            o.PositionX = mark.positionX;
            o.PositionY = mark.positionY;
            o.Width = mark.width;
            o.FontSize = mark.fontSize;
            return o;
        }

        #endregion
    }
}