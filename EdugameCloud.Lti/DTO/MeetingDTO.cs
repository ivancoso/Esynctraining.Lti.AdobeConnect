namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The meeting DTO.
    /// </summary>
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
        /// Gets or sets the connect_server.
        /// </summary>
        [DataMember]
        public string connect_server { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [DataMember]
        public string duration { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public string id { get; set; }

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

        #endregion
    }
}