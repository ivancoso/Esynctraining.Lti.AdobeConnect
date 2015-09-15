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
                return new CommonInfo()
                {
                    AccountUrl = xml.SelectSingleNodeValue("host/text()"),
                    Version = xml.SelectSingleNodeValue("version/text()"),
                    Cookie = xml.SelectSingleNodeValue("cookie/text()"),
                    Date = xml.ParseNodeDateTime("date/text()", default(DateTime)),
                    AdminHost = xml.SelectSingleNodeValue("admin-host/text()"),
                    LocalHost = xml.SelectSingleNodeValue("local-host/text()"),
                    MobileAppPackage = xml.SelectSingleNodeValue("mobile-app-package/text()"),
                    Url = xml.SelectSingleNodeValue("url/text()")
                };
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
