namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    internal static class TelephonyProviderParser
    {
        public static TelephonyProvider Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new TelephonyProvider
            {
                ClassName = xml.SelectSingleNodeValue("class-name/text()"),
                AdaptorId = xml.SelectSingleNodeValue("adaptor-id/text()"),
                Name = xml.SelectSingleNodeValue("name/text()"),
                ProviderStatus = xml.SelectSingleNodeValue("provider-status/text()"),

                ProviderId = xml.SelectAttributeValue("provider-id"),
                ProviderType = xml.SelectAttributeValue("provider-type"),
                AclId = xml.SelectAttributeValue("acl-id"),
            };
        }

    }

}
