using System;
using System.Linq;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using NUnit.Framework;

namespace Esynctraining.AC.Provider.Tests.Parsers
{
    public class CustomFieldParserTests
    {
        [TestCase(
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<custom-fields>" +
            "<field permission-id=\"manage\" object-type=\"object_type_hidden\" field-id=\"x-984625950\" account-id=\"965886535\" display-seq=\"5\" field-type=\"text\" is-primary=\"false\" is-required=\"false\">"
            + "<comments>eSyncTraining MP4 Service Internal Use</comments>"
            + "<name>esync-mp4-result</name>"
            + "</field>"
            + "</custom-fields>"
        )]
        public void ParseField(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            CustomField result = new CustomFieldParser().Parse(doc.SelectNodes("//field").Cast<XmlNode>().First());
            Assert.AreEqual("esync-mp4-result", result.Name);
            Assert.AreEqual(ObjectType.object_type_hidden, result.ObjectType);
            Assert.AreEqual(PermissionId.manage, result.PermissionId);
            Assert.AreEqual("x-984625950", result.FieldId);
        }
         
    }

}