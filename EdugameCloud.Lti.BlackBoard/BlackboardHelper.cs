using System.Linq;

namespace EdugameCloud.Lti.BlackBoard
{
    internal static class BlackboardHelper
    {
        public const string AnswersSeparator = "$$";


        public static int GetBBId(string id)
        {
            return int.Parse(id.TrimStart('_').Split('_').First());
        }

    }

}