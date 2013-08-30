/*
Copyright (c) 2007-2009 Dmitry Stroganov (DmitryStroganov.info)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// MeetingItem structure
    /// </summary>
    [Serializable]
    [XmlRoot("meeting-attendee")]
    public class MeetingAttendee
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingAttendee"/> class.
        /// </summary>
        public MeetingAttendee()
        {
        }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        [XmlAttribute("principal-id")]
        public string PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the transcript id.
        /// </summary>
        [XmlAttribute("transcript-id")]
        public string TranscriptId { get; set; }

        /// <summary>
        /// Gets or sets the meeting name.
        /// </summary>
        [XmlElement("login")]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the meeting description.
        /// </summary>
        [XmlElement("session-name")]
        public string SessionName { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [XmlElement("sco-name")]
        public string ScoName { get; set; }

        /// <summary>
        /// Gets or sets the date start.
        /// </summary>
        [XmlElement("date-created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date end.
        /// </summary>
        [XmlElement("date-end")]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [XmlElement("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the url path.
        /// </summary>
        [XmlElement("participant-name")]
        public string ParticipantName { get; set; }
    }
}
