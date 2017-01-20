using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public class ReportBulkObjectItemParser
    {
        public static ReportBulkObjectItem Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new ReportBulkObjectItem
            {
                ScoId = xml.SelectAttributeValue("sco-id/text()"),
                Icon = xml.SelectAttributeValue("icon/text()"),
                Url = xml.SelectSingleNodeValue("url/text()"),
                Name = xml.SelectSingleNodeValue("name/text()"),
                DateModified = xml.ParseNodeDateTime("date-modified/text()", DateTime.Now),
            };
        }

    }

}
