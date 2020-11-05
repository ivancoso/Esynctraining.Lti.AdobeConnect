namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    internal static class EventNotificationParser
    {
        public static EventNotification Parse(XmlNode xml)
        {
            if (xml?.Attributes == null)
                return null;

            try
            {
                return new EventNotification
                {
                    TargetAclId = xml.SelectAttributeValue("target-acl-id"),
                    ActionId = xml.SelectAttributeValue("action-id"),
                    ActionTypeId = xml.SelectAttributeValue("action-type-id"),
                    Status = xml.SelectAttributeValue("status"),
                    TemplateId = xml.SelectAttributeValue("template-id"),
                    ZoneId = xml.SelectAttributeValue("zone-id"),
                    //Type = EnumReflector.ReflectEnum(xml.SelectAttributeValue("type"), ScoType.not_set),

                    DateModified = xml.ParseNodeDateTime("date-modified/text()", default(DateTime)),
                    DateScheduled = xml.ParseNodeDateTime("date-scheduled/text()", default(DateTime)),

                    NotificationSent = xml.ParseNodeBool("notification-sent/text()"),
                    CqEmailTemplate = xml.SelectSingleNodeValue("cq-email-template/text()"),
                    CqTemplateName = xml.SelectSingleNodeValue("cq-template-name/text()"),
                    CqTemplatePath = xml.SelectSingleNodeValue("cq-template-path/text()"),

                    RelativeDate = xml.SelectSingleNodeValue("relative-date/text()"),
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
