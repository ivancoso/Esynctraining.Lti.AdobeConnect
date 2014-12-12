namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Domain.Entities;

    /// <summary>
    /// The LMS user map.
    /// </summary>
    public class LmsUserMap : BaseClassMap<LmsUser>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LmsUserMap" /> class.
        /// </summary>
        public LmsUserMap()
        {
            this.Map(x => x.UserId).Not.Nullable();
            this.Map(x => x.Username).Nullable();
            this.Map(x => x.Password).Nullable();
            this.Map(x => x.Token).Nullable();
            this.Map(x => x.PrimaryColor).Nullable();
            this.Map(x => x.AcConnectionMode).Default("0");

            this.References(x => x.CompanyLms).Column("companyLmsId").Nullable();

            this.HasMany(x => x.LmsUserParameters).KeyColumn("lmsUserId").Cascade.Delete().Inverse();
        }

        #endregion
    }
}