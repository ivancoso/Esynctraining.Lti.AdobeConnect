namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot("user")]
    public class EventParticipantCompleteInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventParticipantCompleteInformation"/> class.
        /// </summary>
        public EventParticipantCompleteInformation()
        {
        }

        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        [XmlAttribute("principal_id")]
        public string PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the permission_id.
        /// </summary>
        [XmlAttribute("permission_id")]
        public string PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        [XmlElement("login")]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the attendance_status.
        /// </summary>
        [XmlElement("attendance_status")]
        public string AttendanceStatus { get; set; }

        ///// <summary>
        ///// Gets or sets the first_in_time.
        ///// </summary>
        //[XmlElement("first_in_time")]
        //public DateTime? FirstInTime { get; set; }

        ///// <summary>
        ///// Gets or sets the last_end_time.
        ///// </summary>
        //[XmlElement("last_end_time")]
        //public DateTime? LastEndTime { get; set; }

        ///// <summary>
        ///// Gets or sets the registration_time.
        ///// </summary>
        //[XmlElement("registration_time")]
        //public DateTime? RegistrationTime { get; set; }

        ///// <summary>
        ///// Gets or sets the duration.
        ///// </summary>
        //[XmlElement("duration")]
        //public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

    }

}
