namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;

    using NHibernate.Mapping;

    /// <summary>
    /// The Moodle question
    /// </summary>
    public class MoodleQuestion
    {
        /// <summary>
        /// The id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The question text
        /// </summary>
        public string QuestionText { get; set; }

        /// <summary>
        /// The answer text
        /// </summary>
        public string AnswerText { get; set; }

        /// <summary>
        /// The question type
        /// </summary>
        public string QuestionType { get; set; }

        /// <summary>
        /// Value representing if is single
        /// </summary>
        public bool? IsSingle { get; set; }

        /// <summary>
        /// The questions
        /// </summary>
        public List<MoodleQuestion> Questions { get; set; }

        /// <summary>
        /// The answers
        /// </summary>
        public List<MoodleQuestionOptionAnswer> Answers { get; set; }

        /// <summary>
        /// The datasets
        /// </summary>
        public List<MoodleDataset> Datasets { get; set; }
    }
}
