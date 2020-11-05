namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using Esynctraining.NHibernate;
    using NHibernate;
    using NHibernate.Criterion;
    using PDFAnnotation.Core.Domain.Entities;

    public class ScheduleModel : BaseModel<Schedule, int>
    {
        public ScheduleModel(IRepository<Schedule, int> repository)
            : base(repository)
        {
        }


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
                    schedule.NextRun = DateTime.Now.AddHours(schedule.Interval);
                    this.Repository.RegisterSave(schedule);
                }
            }

            return result;
        }

        public bool ExecuteIfPossible(Schedule schedule, Action<DateTime> scheduledAction)
        {
            bool result = false;
            if (schedule.IsEnabled)
            {
                    // time to run the scheduled task
                    if (scheduledAction != null)
                    {
                        scheduledAction(schedule.NextRun);
                        result = true;
                    }
            }

            return result;
        }

        public IFutureValue<Schedule> GetSchedule(ScheduleDescriptor scheduleDescriptor)
        {
            QueryOver<Schedule, Schedule> queryOver = QueryOver.Of<Schedule>().Where(s => s.ScheduleDescriptor == scheduleDescriptor);

            return this.Repository.FindOne(queryOver);
        }

    }

}