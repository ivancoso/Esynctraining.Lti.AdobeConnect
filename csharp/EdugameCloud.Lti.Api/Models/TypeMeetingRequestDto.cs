using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    // TODO: remove type - we fetch meetingitem within RecordingsService.GetRecordings
    // we can reuse that info to have type (change API)
    [DataContract]
    public class TypeMeetingRequestDto : MeetingRequestDto
    {
        [Required]
        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public string SortBy { get; set; }

        [DataMember]
        public string SortOder { get; set; }

        [DataMember]
        public string Search { get; set; }

        [DataMember]
        public long? DateFrom { get; set; }

        [DataMember]
        public long? DateTo { get; set; }

        [DataMember]
        public int Skip { get; set; } = 0;

        [DataMember]
        public int Take { get; set; } = int.MaxValue;

    }
}
