namespace EdugameCloud.Lti.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The schedule.
    /// </summary>
    [Serializable]
    public class Schedule : Entity
    {
        /// <summary>
        /// The next run.
        /// </summary>
        private DateTime nextRun;

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether schedule is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        public virtual int Interval { get; set; }

        /// <summary>
        /// Gets or sets the next run.
        /// </summary>
        public virtual DateTime NextRun
        {
            get
            {
                return this.nextRun;
            }

            set
            {
                this.nextRun = this.AdaptToSql(value);
            }
        }

        /// <summary>
        /// Gets or sets the schedule descriptor.
        /// </summary>
        public virtual ScheduleDescriptor ScheduleDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the schedule type.
        /// </summary>
        public virtual ScheduleType ScheduleType { get; set; }

        #endregion
    }
}