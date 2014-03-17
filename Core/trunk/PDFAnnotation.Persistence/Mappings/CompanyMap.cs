namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The company mapping
    /// </summary>
    public class CompanyMap : BaseClassMap<Company>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyMap"/> class.
        /// </summary>
        public CompanyMap()
        {
            this.Map(x => x.OrganizationId).Nullable();
            this.Map(x => x.CompanyName).Length(500).Nullable();
            this.Map(x => x.ColorPrimary).Length(10).Nullable();
            this.Map(x => x.ColorSecondary).Length(10).Nullable();
            this.Map(x => x.ColorText).Length(10).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Nullable();
            this.Map(x => x.RBFirmId).Nullable();
            this.Map(x => x.OrderDate).Nullable();
            this.Map(x => x.NumberOfLicenses).Nullable();
            this.Map(x => x.Phone).Length(255).Nullable();
            this.References(x => x.Logo).Column("logoId").Nullable().Cascade.Delete();
            this.References(x => x.Address).Nullable().Cascade.Delete();
            this.HasMany(x => x.CompanyContacts).Cascade.Delete().Inverse().ExtraLazyLoad();
        }

        #endregion
    }
}