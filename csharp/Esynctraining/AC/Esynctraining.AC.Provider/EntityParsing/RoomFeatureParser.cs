using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public class RoomFeatureParser
    {
        public static RoomFeature Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new RoomFeature
            {
                FeatureId = xml.SelectAttributeValue("feature-id"),
                AccountId = xml.SelectAttributeValue("account-id"),
                DateBegin = xml.ParseNodeDateTimeOffset("date-begin/text()"),
                DateEnd = xml.ParseNodeDateTimeOffset("date-end/text()"),
                RecordCreated = xml.ParseNodeDateTimeOffset("recordcreated/text()")
            };
        }
    }

}
