using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting
{
    [DataContract]
    public sealed class MeetingNameInfo
    {
        [DataMember]
        public string courseId { get; set; }

        [DataMember]
        public string courseNum { get; set; }

        [DataMember]
        public string courseName { get; set; }

        [DataMember]
        public string meetingName { get; set; }

        [DataMember]
        public string date { get; set; }

        [DataMember]
        public string reusedMeetingName { get; set; }

    }

}
