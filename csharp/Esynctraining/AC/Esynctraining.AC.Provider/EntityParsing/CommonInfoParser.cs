using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public static class CommonInfoParser
    {
        public static CommonInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                // NOTE: account - Information about the account the user belongs to. Returned if you are logged in to Adobe Connect or are making the call on a Adobe Connect hosted account.
                long? accountId = xml.NodeExists("//account") ? long.Parse(xml.SelectSingleNodeValue("//account/@account-id")) : default(long?);

                var result = new CommonInfo
                {
                    AccountUrl = xml.SelectSingleNodeValue("host/text()"),
                    Version = xml.SelectSingleNodeValue("version/text()"),
                    Cookie = xml.SelectSingleNodeValue("cookie/text()"),
                    Date = xml.ParseNodeDateTime("date/text()", default(DateTime)),
                    AdminHost = xml.SelectSingleNodeValue("admin-host/text()"),
                    LocalHost = xml.SelectSingleNodeValue("local-host/text()"),
                    AccountId = accountId,
                    MobileAppPackage = xml.SelectSingleNodeValue("mobile-app-package/text()"),
                    Url = xml.SelectSingleNodeValue("url/text()"),
                    TimeZoneId = int.Parse(xml.SelectSingleNodeValue("@time-zone-id")),
                    TimeZoneJavaId = xml.SelectSingleNodeValue("@time-zone-java-id"),
                    Locale = xml.SelectSingleNodeValue("@locale"),
                };

                result.OWASP_CSRF_TOKEN = OwaspParser.Parse(xml);
                result.User = UserInfoParser.Parse(xml);

                return result;
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
