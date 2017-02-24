using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class EventRegistrationDetailsParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>EventInfo object.</returns>
        public static EventRegistrationDetails Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var details = new EventRegistrationDetails();
                details.EventFields = GenericCollectionParser<EventField>.Parse(xml.SelectSingleNode("//event-fields"),
                    "field", EventFieldParser.Parse);
                details.UserFields = GenericCollectionParser<EventUserField>.Parse(xml.SelectSingleNode("//user-fields"),
                    "field", EventUserFieldParser.Parse);

                return details;
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}