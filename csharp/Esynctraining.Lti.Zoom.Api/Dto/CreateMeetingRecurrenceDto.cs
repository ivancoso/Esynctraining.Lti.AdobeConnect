using System.Runtime.Serialization;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    [DataContract]
    public class CreateMeetingRecurrenceDto
    {
        // TRICK: to support JIL instead of DayOfWeek
        [DataMember]
        public int[] DaysOfWeek { get; set; }

        [DataMember]
        public int Weeks { get; set; }

    }
}