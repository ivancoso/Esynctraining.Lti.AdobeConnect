namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;
    
    public class CompanyLmsMap : BaseClassMap<CompanyLms>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLmsMap"/> class.
        /// </summary>
        public CompanyLmsMap()
        {
            this.References(x => x.Company).Not.Nullable().Column("companyId");
            this.Map(x => x.AcPassword).Not.Nullable();
            this.Map(x => x.AcServer).Not.Nullable();
            this.Map(x => x.AcUsername).Not.Nullable();
            this.Map(x => x.ConsumerKey).Not.Nullable();
            this.References(x => x.CreatedBy).Not.Nullable().Column("createdBy");
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Nullable();
            this.References(x => x.LmsProvider).Not.Nullable().Column("lmsProviderId");
            this.References(x => x.ModifiedBy).Nullable().Column("modifiedBy");
            this.Map(x => x.SharedSecret).Not.Nullable();
        }

        #endregion
    }
}