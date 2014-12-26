namespace EdugameCloud.Lti.Persistence.Mappings
{
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Persistence.Mappings;

    /// <summary>
    /// The schedule map.
    /// </summary>
    public class ScheduleMap : BaseClassMap<Schedule>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleMap"/> class.
        /// </summary>
        public ScheduleMap()
        {
            this.Map(x => x.IsEnabled).Not.Nullable();
            this.Map(x => x.Interval).Not.Nullable();
            this.Map(x => x.NextRun).Not.Nullable();
            this.Map(x => x.ScheduleDescriptor).CustomType<ScheduleDescriptor>().Not.Nullable();
            this.Map(x => x.ScheduleType).CustomType<ScheduleType>().Not.Nullable();
        }

        #endregion
    }
}