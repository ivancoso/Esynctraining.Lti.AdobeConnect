namespace EdugameCloud.Lti.Domain.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// The moodle quiz
    /// </summary>
    public class MoodleQuiz
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
        /// The intro
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// The lms submodule id
        /// </summary>
        public int LmsSubmoduleId { get; set; }
        /// <summary>
        /// The lms submodule name
        /// </summary>
        public string LmsSubmoduleName { get; set; }

        /// <summary>
        /// The questions
        /// </summary>
        public List<MoodleQuestion> Questions { get; set; }
    }
}
