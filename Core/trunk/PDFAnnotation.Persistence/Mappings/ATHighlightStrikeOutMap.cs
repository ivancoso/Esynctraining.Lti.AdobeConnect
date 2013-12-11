namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The highlight strike out map.
    /// </summary>
    public class ATHighlightStrikeOutMap : BaseClassMap<ATHighlightStrikeOut>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ATHighlightStrikeOutMap"/> class.
        /// </summary>
        public ATHighlightStrikeOutMap()
        {
            this.Map(x => x.Color).CustomSqlType("varchar").Length(7).Not.Nullable();
            this.Map(x => x.SelectionInfo).CustomSqlType("varchar").Length(30).Not.Nullable();
            this.Map(x => x.SelectionText).CustomSqlType("ntext").Not.Nullable();
            this.Map(x => x.HasSelection).Not.Nullable();
            this.Map(x => x.Width).Not.Nullable();
            this.Map(x => x.Height).Not.Nullable();
            this.Map(x => x.PositionX).Not.Nullable();
            this.Map(x => x.PositionY).Not.Nullable();

            this.References(x => x.Mark).Not.Nullable();
        }

        #endregion
    }
}