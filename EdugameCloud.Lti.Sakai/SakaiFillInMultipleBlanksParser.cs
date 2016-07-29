using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.DTO;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Sakai
{
    internal class SakaiFillInMultipleBlanksParser : SakaiCommonQuestionParser
    {
        protected class FillInTheBlankAnswer
        {
            public string text { get; set; }
            public string subType { get; set; }
            public bool caseSensitive { get; set; }
        }

        protected override List<AnswerDTO> ParseAnswers(BBQuestionDTO q)
        {
            //var ret = new List<AnswerDTO>();
            //int i = 0;
            //var answersList = (JArray)q.answersList;
            //var items = answersList.ToObject<List<FillInTheBlankAnswer>>();
            ////foreach (var answer in items)
            //{
            //    var dto = ParseFillInBlankAnswer(items, "don't know", i);
            //    ret.Add(dto);
            //    //i++;
            //}

            //return ret;
            var ret = new List<AnswerDTO>();
            int i = 0;
            foreach (var answer in (JObject)q.answersList)
            {
                var dto = ParseFillInBlankAnswer(answer.Value.ToObject<List<FillInTheBlankAnswer>>(), answer.Key, i);
                ret.Add(dto);
                i++;
            }

            return ret;
        }

        protected AnswerDTO ParseFillInBlankAnswer(List<FillInTheBlankAnswer> typedAnswers, string blankName, int blankId)
        {
            var dto = new AnswerDTO()
            {
                id = blankId,
                text = String.Join(SakaiHelper.AnswersSeparator,
                    typedAnswers.Where(x => !String.IsNullOrEmpty(x.text)).Select(a => a.text)), // BB sends empty answers sometimes, we ignore them
                blank_id = blankName,
                question_type = String.Join(SakaiHelper.AnswersSeparator, typedAnswers.Select(a => a.subType)),
                caseSensitive = typedAnswers.Any(x => x.caseSensitive)
            };

            return dto;
        }
    }
}