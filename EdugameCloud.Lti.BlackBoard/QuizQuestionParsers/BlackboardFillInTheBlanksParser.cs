using System;
using System.Collections.Generic;
using EdugameCloud.Lti.DTO;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.BlackBoard.QuizQuestionParsers
{
    internal sealed class BlackboardFillInTheBlanksParser : BlackboardFillInMultipleBlanksParser
    {
        private const string BlankName = "x";
        private const string BlankFormat = "[{0}]";

        public override LmsQuestionDTO ParseQuestion(BBQuestionDTO dto)
        {
            var ret = base.ParseQuestion(dto);
            // inserting blank replacer in the end to handle this question type in the same way as 'Fill in multiple blanks' type
            ret.question_text += " " + String.Format(BlankFormat, BlankName);
            return ret;
        }

        protected override List<AnswerDTO> ParseAnswers(BBQuestionDTO q)
        {
            var dto = ParseFillInBlankAnswer((q.answersList as JToken).ToObject<List<FillInTheBlankAnswer>>(), BlankName, 0);
            return new List<AnswerDTO> {dto};
        }
    }
}