namespace PDFAnnotation.Persistence.Mappings
{
    using Esynctraining.Persistence.Mappings;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The Topic Fluent NHibernate mapping class.
    /// </summary>
    public class TopicMap : BaseClassMap<Topic>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TopicMap" /> class.
        /// </summary>
        public TopicMap()
        {
            this.Map(x => x.FirstName).Nullable();
            this.Map(x => x.LastName).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.References(x => x.Category).Not.Nullable();
            this.HasMany(x => x.Files).Cascade.Delete().Inverse().ExtraLazyLoad();
        }

        #endregion
    }
}