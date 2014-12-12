namespace EdugameCloud.Lti.Domain.Entities
{
    /// <summary>
    /// The Moodle question option answer
    /// </summary>
    public class MoodleQuestionOptionAnswer
    {
        /// <summary>
        /// The id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The answer
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// The fraction
        /// </summary>
        public string Fraction { get; set; }
        /// <summary>
        /// The tolerance
        /// </summary>
        public double? Tolerance { get; set; }
    }
}
