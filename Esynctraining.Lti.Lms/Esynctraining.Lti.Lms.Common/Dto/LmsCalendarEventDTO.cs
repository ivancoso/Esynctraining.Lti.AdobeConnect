using System;

namespace Esynctraining.Lti.Lms.Common.Dto
{
    public class LmsCalendarEventDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Description { get; set; }

        public LmsCalendarEventDTO()
        {
            
        }

        public LmsCalendarEventDTO(DateTime startTime, DateTime endTime, string title)
        {
            StartAt = startTime;
            EndAt = endTime;
            Title = title;
        }

        public LmsCalendarEventDTO(DateTime startTime, DateTime endTime, string title, string description)
        {
            StartAt = startTime;
            EndAt = endTime;
            Title = title;
            Description = $"<a href='{description}'>Course</a>" ;
        }
    }
}
