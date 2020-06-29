using Esynctraining.Persistence.Mappings;
using Esynctraining.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    /// <summary>
    /// The LMS user session mapping.
    /// </summary>
    public sealed class LmsUserSessionMap : BaseClassMapGuid<LmsUserSession>
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
            this.References(x => x.LmsCompany).Column("companyLmsId").Not.Nullable();
            this.References(x => x.LmsUser).Nullable();
        }

        #endregion
    }
}