namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;

    internal static class TrainingItemParser
    {
        public static TrainingItem Parse(XmlNode xml)
        {
            if (xml?.Attributes == null)
            {
                return null;
            }
            var iconString = xml.SelectAttributeValue("icon");
            try
            {
                return new TrainingItem
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    Type = EnumReflector.ReflectEnum(xml.SelectAttributeValue("type"), ScoType.not_set),
                    Icon = string.IsNullOrWhiteSpace(iconString) 
                        ? ScoIcon.not_set 
                        : EnumReflector.ReflectEnum(xml.SelectAttributeValue("icon"), ScoIcon.not_set),
                    PermissionId = xml.SelectAttributeValue("permission-id"),
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    Description = xml.SelectSingleNodeValue("description/text()"),
                    UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                    DateBegin = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                    DateEnd = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                    DateCreated = xml.ParseNodeDateTime("date-created/text()", default(DateTime)),
                    DateModified = xml.ParseNodeDateTime("date-modified/text()", default(DateTime)),
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
