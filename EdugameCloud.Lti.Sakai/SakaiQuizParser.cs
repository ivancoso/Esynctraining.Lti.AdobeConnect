using System.Collections.Generic;
using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;

//using EdugameCloud.Lti.BlackBoard.QuizQuestionParsers;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiQuizParser
    {
        public static LmsQuestionDTO[] ParseQuestions(BBAssessmentDTO td, string jsonData)
        {
            var result = new List<LmsQuestionDTO>();
            if (td.questions == null)
                return result.ToArray();
            foreach (var question in td.questions)
            {
                if (question == null) continue;
                var questionType = GetParserByQuestionType(td, question.type);
                var parsedQuestion = questionType.ParseQuestion(question);
                result.Add(parsedQuestion);
            }

            return result.ToArray();
        }

        public static ISakaiQuestionParser GetParserByQuestionType(BBAssessmentDTO td, string questionType)
        {
            switch (questionType.ToLowerInvariant())
            {
                case "fill in the blank":
                case "numerical":
                    //return new SakaiFillInTheBlanksParser();
                    //case "fill in the blank plus":
                    return new SakaiFillInMultipleBlanksParser(td);


                case "multiple choice":
                case "multiple answer":
                    return new SakaiMultipleChoiceParser(td);
                default:
                    return new SakaiCommonQuestionParser(td);
            }
        }
    }
}
