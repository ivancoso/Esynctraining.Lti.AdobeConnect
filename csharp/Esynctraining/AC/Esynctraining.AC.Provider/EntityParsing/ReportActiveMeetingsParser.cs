using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class ReportActiveMeetingsParser
    {
        public static ReportActiveMeetingsItem Parse(XmlNode xml)
        {
            if (xml?.Attributes == null)
            {
                return null;
            }

            try
            {
                return new ReportActiveMeetingsItem
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    ActiveParticipants = xml.ParseAttributeInt("active-participants"),
                    LengthMinutes = xml.ParseAttributeInt("length-minutes"),
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                    DateBegin = xml.ParseNodeDateTime("date-begin/text()", DateTime.MinValue)
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