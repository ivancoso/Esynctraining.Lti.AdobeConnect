using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Sakai.Dto
{
    [DataContract]
    public class SakaiCalendarDelete
    {
        [DataMember(Name = "calendarReference")]
        public string CalendarReference { get; set; }

        [DataMember(Name = "siteId")]
        public string SiteId { get; set; }

        [DataMember(Name = "events")]
        public SakaiEventDelete[] Events { get; set; }
    }
}