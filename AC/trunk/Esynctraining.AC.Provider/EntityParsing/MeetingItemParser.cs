namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;

    internal static class MeetingItemParser
    {
        public static MeetingItem Parse(XmlNode xml)
        {
            if (xml?.Attributes == null)
            {
                return null;
            }
            var iconString = xml.SelectAttributeValue("icon");
            try
            {
                return new MeetingItem
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    Type = EnumReflector.ReflectEnum(xml.SelectAttributeValue("type"), ScoType.not_set),
                    Icon = string.IsNullOrWhiteSpace(iconString) 
                        ? ScoIcon.not_set 
                        : EnumReflector.ReflectEnum(xml.SelectAttributeValue("icon"), ScoIcon.not_set),
                    PermissionId = xml.SelectAttributeValue("permission-id"),
                    ActiveParticipants = xml.ParseAttributeInt("active-participants"),
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    Description = xml.SelectSingleNodeValue("description/text()"),
                    DomainName = xml.SelectSingleNodeValue("domain-name/text()"),
                    UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                    DateBegin = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                    DateEnd = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                    Expired = xml.ParseNodeBool("description/text()"),
                    Duration = xml.ParseNodeTimeSpan("duration/text()", default(TimeSpan))
                };
                /*
if (!string.IsNullOrEmpty(item.UrlPath) && adobeConnectRoot != null)
{
item.FullUrl = adobeConnectRoot.ToString().TrimEnd('/') + item.UrlPath;
}

item.Duration = item.DateEnd.Subtract(item.DateBegin);

// if item.DateBegin is not defined and duration is 0 => then this is the folder which should be ignored
if (!item.DateBegin.Equals(default(DateTime)) || item.Duration.TotalMinutes != 0)
{
return item;
}
 */
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
