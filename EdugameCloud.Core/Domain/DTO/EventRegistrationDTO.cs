using System;

namespace EdugameCloud.Core.Domain.DTO
{
    public class EventRegistrationDTO
    {
        public int eventQuizMappingId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EventName { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }

    }
}