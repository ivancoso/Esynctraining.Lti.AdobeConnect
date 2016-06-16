using System.Runtime.Serialization;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Sakai.Dto
{
    [DataContract]
    public class SakaiCalendar
    {
        [DataMember(Name = "calendarReference")]
        public string CalendarReference { get; set; }

        [DataMember(Name = "siteId")]
        public string SiteId { get; set; }

        [DataMember(Name = "meetingId")]
        public string MeetingId { get; set; }

        [DataMember(Name = "secret")]
        public string Secret { get; set; }

        [DataMember(Name = "buttonSource")]
        public string ButtonSource { get; set; }

        [DataMember(Name = "events")]
        public SakaiEventDto[] Events { get; set; }
    }
}