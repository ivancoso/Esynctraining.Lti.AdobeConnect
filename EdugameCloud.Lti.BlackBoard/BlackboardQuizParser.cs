using System.Linq;
using EdugameCloud.Lti.BlackBoard.QuizQuestionParsers;
using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.BlackBoard
{
    internal sealed class BlackboardQuizParser
    {
        public static LmsQuestionDTO[] ParseQuestions(BBAssessmentDTO td, string jsonData)
        {
            var ret = td.questions == null
                ? new LmsQuestionDTO[] {}
                : td.questions.Select(
                    q => GetParserByQuestionType(td, q.type).ParseQuestion(q))
                    .ToArray();

            return ret;
        }

        public static IBlackboardQuestionParser GetParserByQuestionType(BBAssessmentDTO td, string questionType)
        {
            switch (questionType.ToLowerInvariant())
            {
                case "fill in the blank":
                    return new BlackboardFillInTheBlanksParser(td);
                case "fill in the blank plus":
                    return new BlackboardFillInMultipleBlanksParser(td);
                case "multiple answer":
                case "multiple choice":
                    return new BlackboardMultipleChoiceParser(td);
                default:
                    return new BlackboardCommonQuestionParser(td);
            }
        }
    }
}
