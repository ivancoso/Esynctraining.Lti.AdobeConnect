using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class UserSeminarLicenseScoParser
    {
        public static UserSeminarLicenseSco Parse(XmlNode xml)
        {
            if (xml == null)
            {
                return null;
            }

            return new UserSeminarLicenseSco
            {
                AccountId = xml.SelectSingleNodeValue("account-id/text()"),
                FolderId = xml.SelectSingleNodeValue("folder-id/text()"),
                Icon = xml.SelectSingleNodeValue("icon/text()"),
                LicenseQuota = xml.ParseNodeInt("license-quota/text()"),
                PrincipalId = xml.SelectSingleNodeValue("principal-id/text()"),
                Name = xml.SelectSingleNodeValue("name/text()"),
                Quota = xml.ParseNodeInt("quota/text()"),
                QuotaId = xml.SelectSingleNodeValue("quota-id/text()"),
                ScoId = xml.SelectSingleNodeValue("sco-id/text()"),
                //Type = xml.ParseAttributeEnum("type", ScoType.not_set),  TODO: EnumReflector.ReflectEnum(xml.SelectSingleNodeValue("type/text()"), ScoType.not_set),
                UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
            };
        }
    }
}