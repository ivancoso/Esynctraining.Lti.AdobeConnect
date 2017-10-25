using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class RequestDto
    {
        //[Required]
        //[DataMember]
        //public string lmsProviderName { get; set; }
    }
    
    [DataContract]
    public class MeetingRequestDto : RequestDto
    {
        /// <summary>
        /// Internal eSyncTraining DB meeting record ID.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MeetingId value is not valid.")]
        [DataMember]
        public int MeetingId { get; set; }

    }


    [DataContract]
    public class RecordingRequestDto : MeetingRequestDto
    {
        /// <summary>
        /// AC sco-id of the recording.
        /// </summary>
        [Required]
        [DataMember]
        public string RecordingId { get; set; }

    }

    [DataContract]
    public class RecordingPasscodeRequestDto
    {
        /// <summary>
        /// AC sco-id of the recording.
        /// </summary>
        [Required]
        [DataMember]
        public string RecordingId { get; set; }

    }

    [DataContract]
    public sealed class SearchRequestDto : RequestDto
    {
        [Required]
        [DataMember]
        public string SearchTerm { get; set; }

    }

}
