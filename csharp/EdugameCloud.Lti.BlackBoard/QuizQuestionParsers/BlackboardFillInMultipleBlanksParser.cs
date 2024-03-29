using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.BlackBoard.QuizQuestionParsers
{
    public class BlackboardFillInMultipleBlanksParser : BlackboardCommonQuestionParser
    {
        public BlackboardFillInMultipleBlanksParser(BBAssessmentDTO td) : base(td)
        {
        }

        protected override List<AnswerDTO> ParseAnswers(BBQuestionDTO q)
        {
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
                text = String.Join(BlackboardHelper.AnswersSeparator, 
                    typedAnswers.Where(x => !String.IsNullOrEmpty(x.text)).Select(a => a.text)), // BB sends empty answers sometimes, we ignore them
                blank_id = blankName,
                question_type = String.Join(BlackboardHelper.AnswersSeparator, typedAnswers.Select(a => a.subType)),
                caseSensitive = typedAnswers.Any(x => x.caseSensitive)
            };

            return dto;
        }

        
    }
}