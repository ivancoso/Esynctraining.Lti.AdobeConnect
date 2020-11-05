namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Utils;

    //<field permission-id="manage" object-type="object-type-read-only" field-id="x-984625950" account-id="965886535" display-seq="5" field-type="text" is-primary="false" is-required="false">
    //    <comments>eSyncTraining MP4 Service Internal Use</comments>
    //    <name>esync-mp4-result</name>
    //</field>
    public sealed class CustomFieldParser : IEntityParser<CustomField>
    {
        public CustomField Parse(XmlNode xml)
        {
            if (xml == null)
                throw new ArgumentNullException(nameof(xml));

            return new CustomField
            {
                PermissionId = EnumReflector.ReflectEnum(xml.SelectAttributeValue("permission-id"), PermissionId.none),
                ObjectType = EnumReflector.ReflectEnum<ObjectType>(xml.SelectAttributeValue("object-type")),
                FieldId = xml.SelectAttributeValue("field-id"),
                AccountId = xml.SelectAttributeValue("account-id"),
                DisplaySeq = xml.ParseAttributeInt("display-seq"),
                FieldType = EnumReflector.ReflectEnum<CustomField.CustomFieldType>(xml.SelectAttributeValue("field-type")),
                IsPrimary = xml.ParseAttributeBool("is-primary"),
                IsRequired = xml.ParseAttributeBool("is-required"),

                Name = xml.SelectSingleNodeValue("name/text()"),
                Comments = xml.SelectSingleNodeValue("comments/text()"),
            };
        }

    }

}
