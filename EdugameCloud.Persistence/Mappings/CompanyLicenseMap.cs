namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The company license mapping
    /// </summary>
    public class CompanyLicenseMap : BaseClassMap<CompanyLicense>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLicenseMap"/> class.
        /// </summary>
        public CompanyLicenseMap()
        {
            this.Map(x => x.Domain).Nullable();
            this.Map(x => x.IsTrial).Nullable();
            this.Map(x => x.ExpiryDate);
            this.Map(x => x.LicenseNumber).Length(50).Not.Nullable();
            this.Map(x => x.TotalLicensesCount).Not.Nullable().Default("(1)");
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.References(x => x.Company).Nullable();
            this.References(x => x.CreatedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<CompanyLicense>(x => x.CreatedBy)));
            this.References(x => x.ModifiedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<CompanyLicense>(x => x.ModifiedBy)));
        }

        #endregion
    }
}