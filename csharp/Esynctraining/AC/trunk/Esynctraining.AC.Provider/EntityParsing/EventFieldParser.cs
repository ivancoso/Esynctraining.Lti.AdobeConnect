using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class EventFieldParser
    {
        public static EventField Parse(XmlNode xml)
        {
            if (xml?.Attributes == null)
            {
                return null;
            }

            try
            {
                var eventField = new EventField
                {
                    InteractionId = xml.SelectAttributeValue("interaction-id"),
                    InteractionType = xml.SelectAttributeValue("interaction-type"),
                    DisplayOrder = xml.ParseAttributeInt("display-seq"),
                    IsRequired = xml.ParseAttributeBool("is-required"),
                    InputDataType = xml.SelectAttributeValue("input-data-type"),
                    Description = xml.SelectSingleNodeValue("description/text()")
                };

                return eventField;
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}