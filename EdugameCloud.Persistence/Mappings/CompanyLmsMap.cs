namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The company LMS map.
    /// </summary>
    public class CompanyLmsMap : BaseClassMap<CompanyLms>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyLmsMap" /> class.
        /// </summary>
        public CompanyLmsMap()
        {
            this.Map(x => x.AcPassword).Not.Nullable();
            this.Map(x => x.AcServer).Not.Nullable();
            this.Map(x => x.AcUsername).Not.Nullable();
            this.Map(x => x.ACScoId).Nullable();
            this.Map(x => x.ACTemplateScoId).Nullable();
            this.Map(x => x.ConsumerKey).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Nullable();
            this.Map(x => x.SharedSecret).Not.Nullable();
            this.Map(x => x.PrimaryColor).Nullable();
            this.Map(x => x.Layout).Nullable();
            this.Map(x => x.LmsDomain).Nullable();

            this.References(x => x.CreatedBy).Not.Nullable().Column("createdBy");
            this.References(x => x.Company).Not.Nullable().Column("companyId");
            this.References(x => x.LmsProvider).Not.Nullable().Column("lmsProviderId");
            this.References(x => x.ModifiedBy).Nullable().Column("modifiedBy");
            this.References(x => x.AdminUser).Column("AdminUserId").Nullable();
        }

        #endregion
    }
}