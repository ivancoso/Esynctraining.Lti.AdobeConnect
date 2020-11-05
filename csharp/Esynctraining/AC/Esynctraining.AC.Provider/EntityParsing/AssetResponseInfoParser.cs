using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public class AssetResponseInfoParser
    {
        public static AssetResponseInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new AssetResponseInfo
            {
                InteractionId = xml.ParseAttributeLong("interaction-id"),
                DisplaySeq = xml.ParseAttributeInt("display-seq"),
                Response = xml.SelectSingleNodeValue("response/text()"),
                Description = xml.SelectSingleNodeValue("description/text()"),
            };
        }

    }

}
