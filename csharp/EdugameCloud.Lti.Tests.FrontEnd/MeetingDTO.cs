﻿using System;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Tests.FrontEnd
{
    // TODO: re-use??
    [DataContract]
    public class MeetingDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ac_room_url.
        /// </summary>
        [DataMember]
        public string ac_room_url { get; set; }

        /// <summary>
        /// Gets or sets the access_level.
        /// </summary>
        [DataMember]
        public string access_level { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether can_join.
        /// </summary>
        [DataMember]
        public bool can_join { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [DataMember]
        public string duration { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is editable.
        /// </summary>
        [DataMember]
        public bool is_editable { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the start_date.
        /// </summary>
        [DataMember]
        public string start_date { get; set; }

        /// <summary>
        /// Gets or sets the start_time.
        /// </summary>
        [DataMember]
        public string start_time { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        [DataMember]
        public string summary { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        [DataMember]
        public string template { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember]
        public int type { get; set; }

        /// <summary>
        /// Gets or sets the office hours.
        /// </summary>
        [DataMember]
        public string office_hours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow_guests.
        /// </summary>
        [DataMember]
        public bool allow_guests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enabled_for_this_meeting.
        /// </summary>
        [DataMember]
        public bool is_disabled_for_this_course { get; set; }

        /// <summary>
        /// SCO-ID of this meeting is used in several meetings. 
        /// Current meeting is either created by re-using existed meeting either current meeting is reused as source for another meeting.
        /// </summary>
        [DataMember]
        public bool reused { get; set; }

        #endregion

        //public SpecialPermissionId GetPermissionId()
        //{
        //    SpecialPermissionId specialPermissionId = string.IsNullOrEmpty(access_level)
        //        ? (allow_guests ? SpecialPermissionId.remove : SpecialPermissionId.denied)
        //        : "denied".Equals(access_level, StringComparison.OrdinalIgnoreCase)
        //            ? SpecialPermissionId.denied
        //            : ("view_hidden".Equals(access_level, StringComparison.OrdinalIgnoreCase) ? SpecialPermissionId.view_hidden : SpecialPermissionId.remove);
        //    return specialPermissionId;
        //}

        //public LmsMeetingType GetMeetingType()
        //{
        //    return type > 0 ? (LmsMeetingType)type : LmsMeetingType.Meeting;
        //}

    }
}
