using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public class QuizInteractionItemParser
    {
        public static QuizInteractionItem Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new QuizInteractionItem
            {
                DisplaySeq = xml.ParseAttributeInt("display-seq"),
                TranscriptId = xml.ParseAttributeLong("transcript-id"),
                InteractionId = xml.ParseAttributeLong("interaction-id"),
                ScoId = xml.ParseAttributeLong("sco-id"),
                Score = xml.ParseAttributeInt("score"),

                Name = xml.SelectSingleNodeValue("name/text()"),
                ScoName = xml.SelectSingleNodeValue("sco-name/text()"),

                DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now),
                Description = xml.SelectSingleNodeValue("description/text()"),
                Response = xml.SelectSingleNodeValue("response/text()"),
            };
        }

    }

}
