namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The meeting DTO.
    /// </summary>
    [DataContract]
    public class MeetingDTO
    {
        #region Public Properties

        [DataMember]
        public string ac_room_url { get; set; }

        [DataMember]
        public string access_level { get; set; }

        [DataMember]
        public bool can_join { get; set; }

        [DataMember]
        public string duration { get; set; }

        [DataMember]
        public long id { get; set; }

        [DataMember]
        public bool is_editable { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string start_date { get; set; }

        [DataMember]
        public string start_time { get; set; }

        [DataMember]
        public long start_timestamp { get; set; }

        [DataMember]
        public string summary { get; set; }

        [DataMember]
        public string template { get; set; }

        [DataMember]
        public int type { get; set; }

        [DataMember]
        public string office_hours { get; set; }

        [DataMember]
        public bool is_disabled_for_this_course { get; set; }

        /// <summary>
        /// SCO-ID of this meeting is used in several meetings. 
        /// Current meeting is created by re-using existed meeting
        /// </summary>
        [DataMember]
        public bool reused { get; set; }

        /// <summary>
        /// Count of other meetings in LTI which uses the same SCO-ID as current meeting.
        /// </summary>
        [DataMember]
        public int reusedByAnotherMeeting { get; set; }

        [DataMember]
        public string audioProfileId { get; set; }

        [DataMember]
        public string audioProfileName { get; set; }

        [DataMember]
        public IDictionary<string, string> telephonyProfileFields { get; set; }

        [DataMember]
        public IEnumerable<MeetingSessionDTO> sessions { get; set; }

        #endregion

        public SpecialPermissionId GetPermissionId()
        {
            if (string.IsNullOrWhiteSpace(access_level))
                throw new InvalidOperationException($"Invalid access_level value '{access_level}'.");
            SpecialPermissionId value = (SpecialPermissionId)Enum.Parse(typeof(SpecialPermissionId), access_level);
            return value;
        }

        public LmsMeetingType GetMeetingType()
        {
            if (type <= 0)
                throw new InvalidOperationException($"Invalid meeting type '{type}'");
            return (LmsMeetingType)type;
        }

    }

}