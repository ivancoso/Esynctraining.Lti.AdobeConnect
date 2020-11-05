namespace PDFAnnotation.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The schedule.
    /// </summary>
    [Serializable]
    public class Schedule : Entity
    {
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
        public virtual DateTime NextRun { get; set; }

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