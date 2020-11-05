namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The meeting Session parser.
    /// </summary>
    internal static class EventParticipantCompleteInformationParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Item.</returns>
        public static EventParticipantCompleteInformation Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            var item = new EventParticipantCompleteInformation
            {
                PrincipalId = xml.SelectAttributeValue("principal_id"),
                PermissionId = xml.SelectAttributeValue("permission_id"),
                Login = xml.SelectAttributeValue("login"),
                AttendanceStatus = xml.SelectAttributeValue("attendance_status"),
                //FirstInTime = xml.SelectAttributeValue("first_in_time").ParseDateTimeWithDefault(default(DateTime)),
                //LastEndTime = xml.SelectAttributeValue("last_end_time").ParseDateTimeWithDefault(default(DateTime)),
                //RegistrationTime = xml.SelectAttributeValue("registration_time").ParseDateTimeWithDefault(default(DateTime)),
                //Duration = xml.ParseNodeTimeSpan("duration/text()", default(TimeSpan)),
                Name = xml.SelectAttributeValue("name"),
            };

            return item;
        }
    }
}
