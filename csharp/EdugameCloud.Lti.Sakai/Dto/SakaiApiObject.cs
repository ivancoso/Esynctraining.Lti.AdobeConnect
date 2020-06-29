using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Sakai.Dto
{
    [DataContract]
    public class SakaiApiObject
    {
        [DataMember(Name = "params")]
        public SakaiParams Params { get; set; }

        [DataMember(Name = "calendars")]
        public SakaiCalendar[] Calendars { get; set; }
    }
}