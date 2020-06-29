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
        //[Required]
        [DataMember]
        public string StartDate { get; set; }

        //[Required]
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
    public class UpdateCourseSectionsDto
    {
        [Required]
        [DataMember]
        public int MeetingId { get; set; }

        [Required]
        [DataMember]
        public string[] SectionIds { get; set; }
    }


    [DataContract]
    public class MeetingDTOLtiBase<TSession> : MeetingDtoBase
    {
        /// <summary>
        /// Internal eSyncTraining DB meeting record ID.
        /// </summary>
        //[Required] // todo: separate models and actions for create and update
        [DataMember]
        public long? Id { get; set; }

        #region Public Properties

        /// <summary>
        /// Meeting = 1, OfficeHours = 2, StudyGroup = 3, Seminar = 4, VirtualClassroom = 5.
        /// </summary>
        [Required]
        [DataMember]
        public int Type { get; set; }
        
        [Obsolete("TRICK: to hide from Swagger only. UNIR doesn't use OfficeHours")]
        [DataMember]
        public string OfficeHours { get; set; }

        [Obsolete("TRICK: to hide from Swagger only. UNIR doesn't use OfficeHours")]
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

        [Obsolete("TRICK: to hide from Swagger only. UNIR doesn't use AudioProfile")]
        [DataMember]
        public IDictionary<string, string> TelephonyProfileFields { get; set; }

        [Obsolete("TRICK: to hide from Swagger only. UNIR doesn't use Sessions")]
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
        //..TODO: check SSO - not in use there for now.But required for amgem.
        /// <summary>
        /// Used to support Virtual Classroom.
        /// </summary>
        [DataMember]
        public string ClassRoomId { get; set; }

        [DataMember]
        public List<string> SectionIds { get; set; }

    }

}