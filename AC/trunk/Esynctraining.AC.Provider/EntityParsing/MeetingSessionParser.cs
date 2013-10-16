namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The meeting Session parser.
    /// </summary>
    internal static class MeetingSessionParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Meeting Item or null if it's a folder.</returns>
        public static MeetingSession Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var item = new MeetingSession
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    Version = xml.SelectAttributeValue("version"),
                    ParticipantsCount = xml.SelectAttributeValue("num-participants"),
                    AssetId = xml.SelectAttributeValue("asset-id"),
                    DateCreated = xml.ParseNodeDateTime("date-created/text()", default(DateTime)),
                    DateEnd = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                };


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
