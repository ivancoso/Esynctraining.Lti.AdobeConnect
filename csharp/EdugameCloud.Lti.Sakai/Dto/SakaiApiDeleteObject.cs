using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Sakai.Dto
{
    [DataContract]
    public class SakaiApiDeleteObject
    {
        [DataMember(Name = "params")]
        public SakaiParams Params { get; set; }

        [DataMember(Name = "calendars")]
        public SakaiCalendarDelete[] Calendars { get; set; }
    }
}