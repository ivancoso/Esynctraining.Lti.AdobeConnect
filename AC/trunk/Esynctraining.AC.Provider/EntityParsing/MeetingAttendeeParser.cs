namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The meeting attendee parser.
    /// </summary>
    internal static class MeetingAttendeeParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Meeting Item or null if it's a folder.</returns>
        public static MeetingAttendee Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var item = new MeetingAttendee
                               {
                                   ScoId = xml.SelectAttributeValue("transcript-id"),
                                   TranscriptId = xml.SelectAttributeValue("sco-id"),
                                   PrincipalId = xml.SelectAttributeValue("principal-id"),
                                   Login = xml.SelectSingleNodeValue("login/text()"),
                                   SessionName = xml.SelectSingleNodeValue("session-name/text()"),
                                   ScoName = xml.SelectSingleNodeValue("sco-name/text()"),
                                   ParticipantName = xml.SelectSingleNodeValue("participant-name/text()"),
                                   DateCreated = xml.ParseNodeDateTime("date-created/text()", default(DateTime)),
                                   DateEnd = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                               };


                item.Duration = item.DateEnd.Subtract(item.DateCreated);

                if (!item.DateCreated.Equals(default(DateTime)) && !item.DateEnd.Equals(default(DateTime)))
                {
                    return item;
                }
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
