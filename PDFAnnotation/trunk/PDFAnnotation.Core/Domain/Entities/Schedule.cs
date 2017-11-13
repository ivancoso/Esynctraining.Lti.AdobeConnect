namespace PDFAnnotation.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    [Serializable]
    public class Schedule : Entity
    {
        public virtual bool IsEnabled { get; set; }

        public virtual int Interval { get; set; }

        public virtual DateTime NextRun { get; set; }

        public virtual ScheduleDescriptor ScheduleDescriptor { get; set; }

        public virtual ScheduleType ScheduleType { get; set; }

    }

}