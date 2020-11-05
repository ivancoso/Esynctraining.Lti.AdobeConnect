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
    public class ATHighlightStrikeOutMarkDTO : IBoxedMarkDTO
    {
        public ATHighlightStrikeOutMarkDTO() { }

        public ATHighlightStrikeOutMarkDTO(ATHighlightStrikeOut item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this.displayFormat = item.Mark.DisplayFormat;
            this.pageIndex = item.Mark.PageIndex;
            this.id = item.Mark.Id.ToString();
            this.type = item.Mark.Type;
            this.fileId = item.Mark.File.Id.ToString();
            this.datechanged = item.Mark.DateChanged.With(x => x.ConvertToUnixTimestamp()); 
            this.datecreated = item.Mark.DateCreated.With(x => x.ConvertToUnixTimestamp());
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

        [DataMember]
        public bool has_selection { get; set; }

        [DataMember]
        public string selection_info { get; set; }

        [DataMember]
        public string selection_text { get; set; }

        #endregion

        #region Base Properties

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public int pageIndex { get; set; }

        [DataMember]
        public string displayFormat { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string fileId { get; set; }

        [DataMember]
        public double datechanged { get; set; }

        [DataMember]
        public double datecreated { get; set; }

        [DataMember]
        public bool @readonly { get; set; }

        [DataMember]
        public float? rotation { get; set; }

        #endregion

        #region Boxed Properties

        [DataMember]
        public float positionX { get; set; }

        [DataMember]
        public float positionY { get; set; }

        [DataMember]
        public float width { get; set; }

        [DataMember]
        public float height { get; set; }

        [DataMember]
        public string color { get; set; }

        #endregion

    }

}