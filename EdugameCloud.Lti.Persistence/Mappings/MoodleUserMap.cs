namespace EdugameCloud.Lti.Persistence.Mappings
{
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Persistence.Mappings;

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
            this.Map(x => x.UserName).Length(100).Nullable();
            this.Map(x => x.Password).Length(100).Nullable();
            this.Map(x => x.Domain).Length(100).Not.Nullable();
            this.Map(x => x.UserId).Not.Nullable();
            this.Map(x => x.Token).Length(50).Not.Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
        }

        #endregion
    }
}
