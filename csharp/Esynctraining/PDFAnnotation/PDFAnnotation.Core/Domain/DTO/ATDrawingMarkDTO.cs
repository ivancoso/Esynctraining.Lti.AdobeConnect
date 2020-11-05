using Esynctraining.Core.Extensions;

namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    [DataContract]
    public class ATDrawingMarkDTO : IBaseMarkDTO
    {
        public ATDrawingMarkDTO() { }

        public ATDrawingMarkDTO(ATDrawing item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this.displayFormat = item.Mark.DisplayFormat;
            this.pageIndex = item.Mark.PageIndex;
            this.id = item.Mark.Id.ToString();
            this.type = item.Mark.Type;
            this.fileId = item.Mark.File.Id.ToString();
            this.datechanged = item.Mark.DateChanged.With(x=> x.ConvertToUnixTimestamp());
            this.datecreated = item.Mark.DateCreated.With(x => x.ConvertToUnixTimestamp());
            this.@readonly = item.Mark.IsReadonly;
            this.rotation = item.Mark.Rotation;
            this.color = item.Color;
            this.points = item.Points;
        }
        
        #region Public Properties

        [DataMember]
        public string color { get; set; }

        [DataMember]
        public string points { get; set; }

        #endregion

        #region Base Properties

        [DataMember]
        public float? rotation { get; set; }

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

        #endregion

    }

}