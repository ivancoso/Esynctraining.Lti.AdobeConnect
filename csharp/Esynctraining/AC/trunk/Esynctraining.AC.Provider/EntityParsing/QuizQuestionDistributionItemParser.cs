using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public class QuizQuestionDistributionItemParser
    {
        public static QuizQuestionDistributionItem Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new QuizQuestionDistributionItem
            {
                DisplaySeq = xml.ParseAttributeInt("display-seq"),
                InteractionId = xml.ParseAttributeLong("interaction-id"),

                NumCorrect = xml.ParseAttributeInt("num-correct"),
                NumIncorrect = xml.ParseAttributeInt("num-incorrect"),
                TotalResponses = xml.ParseAttributeInt("total-responses"),
                PercentageCorrect = xml.ParseAttributeInt("percentage-correct"),

                Score = xml.ParseAttributeInt("score"),
                Name = xml.SelectSingleNodeValue("name/text()"),
                Description = xml.SelectSingleNodeValue("description/text()"),
            };
        }

    }

}
