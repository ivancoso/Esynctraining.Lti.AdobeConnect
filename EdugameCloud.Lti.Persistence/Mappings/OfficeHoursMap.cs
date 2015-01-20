namespace EdugameCloud.Lti.Persistence.Mappings
{
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Persistence.Mappings;

    /// <summary>
    /// The office hours map.
    /// </summary>
    public class OfficeHoursMap : BaseClassMap<OfficeHours>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeHoursMap"/> class.
        /// </summary>
        public OfficeHoursMap()
        {
            this.Map(x => x.Hours).Nullable();
            this.Map(x => x.ScoId).Not.Nullable();

            this.References(x => x.LmsUser).Not.Nullable();
        }

        #endregion
    }
}
