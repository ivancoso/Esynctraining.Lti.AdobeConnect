using PDFAnnotation.Core.Domain.Entities;

namespace PDFAnnotation.Persistence.Mappings
{
    /// <summary>
    /// The picture map.
    /// </summary>
    public class ATPictureMap : BaseClassMap<ATPicture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATPictureMap"/> class.
        /// </summary>
        public ATPictureMap()
        {
            this.Map(x => x.PositionX).Not.Nullable();
            this.Map(x => x.PositionY).Not.Nullable();
            this.Map(x => x.Width).Not.Nullable();
            this.Map(x => x.Height).Not.Nullable();
            //this.Map(x => x.Image).CustomSqlType("VARBINARY(MAX)").Not.Nullable();
            this.Map(x => x.Image).CustomType("BinaryBlob").Length(int.MaxValue).Not.Nullable().LazyLoad();      
            this.Map(x => x.Path).Not.Nullable();
            this.Map(x => x.LabelText).Nullable();
            this.Map(x => x.LabelFontSize).Nullable();
            this.Map(x => x.LabelTextColor).Nullable();
            this.References(x => x.Mark).Not.Nullable();
        }

    }

}
