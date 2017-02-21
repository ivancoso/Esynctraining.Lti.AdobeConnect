namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AdobeConnect.Api.Meeting.Dto;

    [DataContract]
    public class MeetingDTOInput : MeetingDTO
    {
        [Required]
        [DataMember]
        public string StartDate { get; set; }

        [Required]
        [DataMember]
        public string StartTime { get; set; }


        public SpecialPermissionId GetPermissionId()
        {
            if (string.IsNullOrWhiteSpace(AccessLevel))
                throw new InvalidOperationException($"Invalid AccessLevel value '{AccessLevel}'.");
            SpecialPermissionId value = (SpecialPermissionId)Enum.Parse(typeof(SpecialPermissionId), AccessLevel);
            return value;
        }

    }


    [DataContract]
    public class MeetingDTOLtiBase<TSession> : MeetingDtoBase
    {
        [DataMember]
        public long Id { get; set; }

        #region Public Properties

        [DataMember]
        public int Type { get; set; }
        
        [DataMember]
        public string OfficeHours { get; set; }

        [DataMember]
        public bool IsDisabledForThisCourse { get; set; }

        /// <summary>
        /// SCO-ID of this meeting is used in several meetings. 
        /// Current meeting is created by re-using existed meeting
        /// </summary>
        [DataMember]
        public bool Reused { get; set; }

        /// <summary>
        /// Count of other meetings in LTI which uses the same SCO-ID as current meeting.
        /// </summary>
        [DataMember]
        public int ReusedByAnotherMeeting { get; set; }

        [DataMember]
        public IDictionary<string, string> TelephonyProfileFields { get; set; }

        [DataMember(Name = "sessions")]
        public TSession[] Sessions { get; set; }

        #endregion

        public LmsMeetingType GetMeetingType()
        {
            if (Type <= 0)
                throw new InvalidOperationException($"Invalid meeting type '{Type}'");
            return (LmsMeetingType)Type;
        }

    }

    /// <summary>
    /// The meeting DTO.
    /// </summary>
    [DataContract]
    public class MeetingDTO : MeetingDTOLtiBase<MeetingSessionDTO>
    {
        /// <summary>
        /// Used to support Virtual Classroom.
        /// TODO: check SSO - not in use there for now. But required for amgem.
        /// </summary>
        [DataMember]
        public string ClassRoomId { get; set; }

    }

}