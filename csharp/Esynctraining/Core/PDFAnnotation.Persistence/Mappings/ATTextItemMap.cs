namespace PDFAnnotation.Persistence.Mappings
{
    using Esynctraining.Persistence.Mappings;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The TextItem Fluent NHibernate mapping class.
    /// </summary>
    public class ATTextItemMap : BaseClassMap<ATTextItem>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ATTextItemMap" /> class.
        /// </summary>
        public ATTextItemMap()
        {
            this.Map(x => x.Text).Not.Nullable();
            this.Map(x => x.FontName).Nullable();
            this.Map(x => x.FontFamily).Nullable();
            this.Map(x => x.FontSize).Not.Nullable();
            this.Map(x => x.PositionX).Not.Nullable();
            this.Map(x => x.PositionY).Not.Nullable();
            this.Map(x => x.Color).Not.Nullable();
            this.Map(x => x.Width).Not.Nullable();
            this.Map(x => x.Height).Not.Nullable();
            this.Map(x => x.Alignment).Nullable();

            this.References(x => x.Mark).Not.Nullable();
        }

        #endregion
    }
}