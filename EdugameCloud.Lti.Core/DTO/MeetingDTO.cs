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
        public bool allow_guests { get; set; }

        [DataMember]
        public bool is_disabled_for_this_course { get; set; }

        /// <summary>
        /// SCO-ID of this meeting is used in several meetings. 
        /// Current meeting is either created by re-using existed meeting either current meeting is reused as source for another meeting.
        /// </summary>
        [DataMember]
        public bool reused { get; set; }

        [DataMember]
        public string audioProfileId { get; set; }

        [DataMember]
        public IDictionary<string, string> telephonyProfileFields { get; set; }

        #endregion

        public SpecialPermissionId GetPermissionId()
        {
            SpecialPermissionId specialPermissionId = string.IsNullOrEmpty(access_level)
                ? (allow_guests ? SpecialPermissionId.remove : SpecialPermissionId.denied)
                : "denied".Equals(access_level, StringComparison.OrdinalIgnoreCase)
                    ? SpecialPermissionId.denied
                    : ("view_hidden".Equals(access_level, StringComparison.OrdinalIgnoreCase) ? SpecialPermissionId.view_hidden : SpecialPermissionId.remove);
            return specialPermissionId;
        }

        public LmsMeetingType GetMeetingType()
        {
            return type > 0 ? (LmsMeetingType)type : LmsMeetingType.Meeting;
        }

    }

}