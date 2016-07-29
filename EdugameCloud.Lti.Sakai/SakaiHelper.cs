using System.Linq;

namespace EdugameCloud.Lti.Sakai
{
    public static class SakaiHelper
    {
        public const string AnswersSeparator = "$$";

        public static int GetBBId(string id)
        {
            return int.Parse(id.TrimStart('_').Split('_').First());
        }
    }
}