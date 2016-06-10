namespace PDFAnnotation.Persistence.Mappings
{
    using Esynctraining.Persistence.Mappings;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The shape map.
    /// </summary>
    public class ATShapeMap : BaseClassMap<ATShape>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ATShapeMap"/> class.
        /// </summary>
        public ATShapeMap()
        {
            this.Map(x => x.PositionX).Not.Nullable();
            this.Map(x => x.PositionY).Not.Nullable();
            this.Map(x => x.Width).Not.Nullable();
            this.Map(x => x.Height).Not.Nullable();
            this.Map(x => x.Color).Not.Nullable();
            this.Map(x => x.Text).Nullable();
            this.Map(x => x.Image).CustomType("StringClob").CustomSqlType("ntext").Nullable();
            this.Map(x => x.Points).CustomType("StringClob").CustomSqlType("ntext").Nullable();
            this.Map(x => x.FillColor).Nullable();
            this.Map(x => x.FillOpacity).Nullable();
            this.Map(x => x.StrokeOpacity).Nullable();
            this.Map(x => x.StrokeWidth).Nullable();
            this.Map(x => x.LabelText).Nullable();
            this.Map(x => x.StampColor).Nullable();
            this.Map(x => x.LabelTextColor).Nullable();
            this.Map(x => x.Style).Nullable();
            this.Map(x => x.NumberingTextColor).Nullable();
            
            this.References(x => x.Mark).Not.Nullable();
        }

        #endregion
    }
}