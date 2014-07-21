namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;

    using NHibernate.Mapping;

    public class MoodleQuestion
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string QuestionText { get; set; }
        public string QuestionType { get; set; }

        public List<MoodleQuestionOptionAnswer> Answers { get; set; }
    }
}
