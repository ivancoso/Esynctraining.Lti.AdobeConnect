namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;

    using NHibernate.Mapping;

    public class MoodleQuiz
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }
        public int LmsSubmoduleId { get; set; }
        public string LmsSubmoduleName { get; set; }

        public List<MoodleQuestion> Questions { get; set; }
    }
}
