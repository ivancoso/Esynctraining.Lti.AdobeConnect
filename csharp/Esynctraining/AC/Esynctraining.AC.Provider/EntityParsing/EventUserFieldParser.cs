using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class EventUserFieldParser
    {
        public static EventUserField Parse(XmlNode xml)
        {
            if (xml?.Attributes == null)
            {
                return null;
            }

            try
            {
                var eventField = new EventUserField
                {
                    InteractionType = xml.SelectSingleNodeValue("interaction-type/text()"),
                    DisplayOrder = xml.ParseNodeInt("display-seq/text()"),
                    IsRequired = xml.ParseNodeBool("is-required/text()"),
                    InputDataType = xml.SelectSingleNodeValue("input-data-type/text()"),
                    Description = xml.SelectSingleNodeValue("description/text()"),
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    Response = xml.SelectSingleNodeValue("response/text()"),
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