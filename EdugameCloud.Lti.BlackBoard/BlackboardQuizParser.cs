using System.Linq;
using EdugameCloud.Lti.BlackBoard.QuizQuestionParsers;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.BlackBoard
{
    internal sealed class BlackboardQuizParser
    {
        public static LmsQuestionDTO[] ParseQuestions(BBAssessmentDTO td)
        {
            var ret = td.questions == null
                ? new LmsQuestionDTO[] {}
                : td.questions.Select(
                    q => GetParserByQuestionType(q.type).ParseQuestion(q))
                    .ToArray();

            return ret;
        }

        public static IBlackboardQuestionParser GetParserByQuestionType(string questionType)
        {
            switch (questionType.ToLowerInvariant())
            {
                case "fill in the blank":
                    return new BlackboardFillInTheBlanksParser();
                case "fill in the blank plus":
                    return new BlackboardFillInMultipleBlanksParser();
                default:
                    return new BlackboardCommonQuestionParser();
            }
        }
    }
}
