using System.Linq;
using EdugameCloud.Lti.DTO;
//using EdugameCloud.Lti.BlackBoard.QuizQuestionParsers;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiQuizParser
    {
        public static LmsQuestionDTO[] ParseQuestions(BBAssessmentDTO td, string jsonData)
        {
            var ret = td.questions == null
                ? new LmsQuestionDTO[] {}
                : td.questions.Select(
                    q => GetParserByQuestionType(q.type).ParseQuestion(q))
                    .ToArray();

            return ret;
        }

        public static ISakaiQuestionParser GetParserByQuestionType(string questionType)
        {
            switch (questionType.ToLowerInvariant())
            {
                //case "fill in the blank":
                //    return new BlackboardFillInTheBlanksParser();
                //case "fill in the blank plus":
                //    return new BlackboardFillInMultipleBlanksParser();
                //case "multiple answer":
                case "multiple choice":
                    return new SakaiCommonQuestionParser();
                default:
                    return new SakaiCommonQuestionParser();
            }
        }
    }
}
