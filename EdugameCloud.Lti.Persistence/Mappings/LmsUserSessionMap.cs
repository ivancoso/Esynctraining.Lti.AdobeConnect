namespace EdugameCloud.Lti.Persistence.Mappings
{
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Persistence.Mappings;

    /// <summary>
    /// The LMS user session mapping.
    /// </summary>
    public class LmsUserSessionMap : BaseClassMapGuid<LmsUserSession>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserSessionMap"/> class.
        /// </summary>
        public LmsUserSessionMap()
        {
            this.Map(x => x.SessionData).Length(int.MaxValue).CustomType("StringClob").Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Nullable();
            this.Map(x => x.LmsCourseId).Not.Nullable();
            this.References(x => x.CompanyLms).Not.Nullable();
            this.References(x => x.LmsUser).Nullable();
        }

        #endregion
    }
}