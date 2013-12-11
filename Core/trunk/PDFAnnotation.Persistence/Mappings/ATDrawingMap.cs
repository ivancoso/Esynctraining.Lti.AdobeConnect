namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The ATDrawing Fluent NHibernate mapping class.
    /// </summary>
    public class ATDrawingMap : BaseClassMap<ATDrawing>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ATDrawingMap" /> class.
        /// </summary>
        public ATDrawingMap()
        {
            this.Map(x => x.Color).CustomSqlType("varchar").Length(7).Not.Nullable();
            this.Map(x => x.Points).CustomSqlType("text").Not.Nullable();
            this.References(x => x.Mark).Not.Nullable();
        }

        #endregion
    }
}