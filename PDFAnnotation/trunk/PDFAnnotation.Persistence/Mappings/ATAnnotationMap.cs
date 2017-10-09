using PDFAnnotation.Core.Business.Models;

namespace PDFAnnotation.Persistence.Mappings
{
    /// <summary>
    /// The formula map.
    /// </summary>
    public class ATAnnotationMap : BaseClassMap<ATAnnotation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATAnnotationMap"/> class.
        /// </summary>
        public ATAnnotationMap()
        {
            this.Map(x => x.PositionX).Not.Nullable();
            this.Map(x => x.PositionY).Not.Nullable();
            this.Map(x => x.Comment).Not.Nullable();
            this.Map(x => x.Height).Nullable();
            this.Map(x => x.Width).Nullable();
            this.Map(x => x.IconName).Nullable();
            this.Map(x => x.IsOpened).Nullable();
            this.Map(x => x.Color).Nullable();
            this.Map(x => x.FillOpacity).Nullable();
            this.Map(x => x.CreatedBy).Nullable();
            this.References(x => x.Mark).Not.Nullable();
        }

    }

}
