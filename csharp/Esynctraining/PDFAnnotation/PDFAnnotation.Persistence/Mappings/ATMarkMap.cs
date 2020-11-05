namespace PDFAnnotation.Persistence.Mappings
{
    using Core.Domain.Entities;
    using FluentNHibernate.Mapping;

    /// <summary>
    /// The mark map.
    /// </summary>
    public class ATMarkMap : ClassMap<ATMark>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATMarkMap"/> class.
        /// </summary>
        public ATMarkMap()
        {
            this.Id(x => x.Id).GeneratedBy.Assigned();
            this.Map(x => x.IsReadonly).Not.Nullable();
            this.Map(x => x.Type).Not.Nullable();
            this.Map(x => x.DisplayFormat).Not.Nullable();
            this.Map(x => x.PageIndex).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateChanged).Not.Nullable();
            this.Map(x => x.Rotation).Nullable();
            this.Map(x => x.UpdatedBy).Nullable();
            this.Map(x => x.CreatedBy).Nullable();

            this.References(x => x.File).Not.Nullable();

            this.HasMany(x => x.Shapes).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Drawings).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.HighlightStrikeOuts).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.TextItems).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Rotations).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Pictures).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Formulas).LazyLoad().Cascade.Delete().Inverse();
            this.HasMany(x => x.Annotations).LazyLoad().Cascade.Delete().Inverse();
        }

    }

}