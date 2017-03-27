using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    // TODO: move
    [DataContract]
    public class RequestDto
    {
        //[Required]
        //[DataMember]
        //public string lmsProviderName { get; set; }
    }

    //[DataContract]
    //public class TemplatesRequestDto : RequestDto
    //{
    //    [DataMember]
    //    public int LmsMeetingType { get; set; }
    //}

    [DataContract]
    public class MeetingRequestDto : RequestDto
    {
        /// <summary>
        /// Internal eSyncTraining DB meeting record ID.
        /// </summary>
        [Required]
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
    public sealed class SearchRequestDto : RequestDto
    {
        [DataMember]
        public string SearchTerm { get; set; }

    }

}
