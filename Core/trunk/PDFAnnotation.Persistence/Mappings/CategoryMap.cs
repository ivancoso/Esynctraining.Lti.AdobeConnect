namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The case mapping
    /// </summary>
    public class CategoryMap : BaseClassMap<Category>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryMap"/> class.
        /// </summary>
        public CategoryMap()
        {
            this.Map(x => x.CategoryName).Length(450).Not.Nullable();
            this.Map(x => x.Details).Nullable();
            this.Map(x => x.RBCaseId).Nullable();
            this.Map(x => x.IsFileNumbersAutoIncremented).Not.Nullable();
            this.References(x => x.Company);
            this.HasMany(x => x.Topics).Cascade.Delete().Inverse().ExtraLazyLoad();
            this.HasMany(x => x.Files).Cascade.Delete().Inverse().ExtraLazyLoad();
            this.HasManyToMany(x => x.Contacts)
                .Table("CategoryContact")
                .ParentKeyColumn("categoryId")
                .ChildKeyColumn("contactId").Cascade.Delete().Inverse();
        }

        #endregion
    }
}