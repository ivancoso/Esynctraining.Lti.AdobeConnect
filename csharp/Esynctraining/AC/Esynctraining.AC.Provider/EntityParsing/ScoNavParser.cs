using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class ScoNavParser
    {
        public static ScoNav Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new ScoNav
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    Type = xml.SelectAttributeValue("type"),
                    Icon = xml.SelectAttributeValue("icon"),
                    Depth = xml.ParseAttributeInt("depth"),
                    Name = xml.SelectSingleNodeValue("name/text()")
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