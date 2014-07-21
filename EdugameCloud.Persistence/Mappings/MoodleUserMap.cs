namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The moodle user map.
    /// </summary>
    public class MoodleUserMap : BaseClassMap<MoodleUser>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleUserMap"/> class.
        /// </summary>
        public MoodleUserMap()
        {
            this.Map(x => x.UserName).Length(100).Not.Nullable();
            this.Map(x => x.FirstName).Length(100).Not.Nullable();
            this.Map(x => x.LastName).Length(100).Not.Nullable();
            this.Map(x => x.Password).Length(32).Not.Nullable();
            this.Map(x => x.MoodleUserId).Not.Nullable();
            this.Map(x => x.CompanyId).Nullable();
        }

        #endregion
    }
}
