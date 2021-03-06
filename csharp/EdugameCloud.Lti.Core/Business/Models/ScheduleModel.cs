﻿namespace EdugameCloud.Lti.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.NHibernate;
    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    /// The schedule model.
    /// </summary>
    public sealed class ScheduleModel : BaseModel<Schedule, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public ScheduleModel(IRepository<Schedule, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute if necessary.
        /// </summary>
        /// <param name="schedule">
        /// The schedule.
        /// </param>
        /// <param name="scheduledAction">
        /// The scheduled action.
        /// </param>
        /// <returns>
        /// The <see cref="Schedule"/>.
        /// </returns>
        public bool ExecuteIfNecessary(Schedule schedule, Action<DateTime> scheduledAction)
        {
            bool result = false;
            if (schedule.IsEnabled)
            {
                if (schedule.NextRun <= DateTime.Now)
                {
                    // time to run the scheduled task
                    if (scheduledAction != null)
                    {
                        scheduledAction(schedule.NextRun);
                        result = true;
                    }

                    // update schedule next run time
                    schedule.NextRun = DateTime.Now.AddMinutes(schedule.Interval);
                    this.Repository.RegisterSave(schedule);
                }
            }

            return result;
        }

        public bool ExecuteIfPossible(Schedule schedule, Func<IEnumerable<LmsCompany>, DateTime, int, string> scheduledAction,
            IEnumerable<LmsCompany> companies, 
            int meetingId,
            out string output)
        {
            output = null;
            bool result = false;
            if (schedule.IsEnabled)
            {
                    // time to run the scheduled task
                    if (scheduledAction != null)
                    {
                        output = scheduledAction(companies, schedule.NextRun, meetingId);
                        result = true;
                    }
            }

            return result;
        }

        /// <summary>
        /// The get schedule.
        /// </summary>
        /// <param name="scheduleDescriptor">
        /// The schedule descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Schedule}"/>.
        /// </returns>
        public IFutureValue<Schedule> GetSchedule(ScheduleDescriptor scheduleDescriptor)
        {
            QueryOver<Schedule, Schedule> queryOver = QueryOver.Of<Schedule>().Where(s => s.ScheduleDescriptor == scheduleDescriptor);

            return this.Repository.FindOne(queryOver);
        }

        #endregion

    }

}