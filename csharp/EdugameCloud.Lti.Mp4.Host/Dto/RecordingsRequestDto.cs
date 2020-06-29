using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingsRequestDto
    {
        [Required]
        [DataMember(Name = "type")]
        public int LmsMeetingType { get; set; }

        [Required]
        [DataMember]
        public int MeetingId { get; set; }

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