namespace EdugameCloud.Core.Domain.Formats.WebEx
{
    using System.Xml.Serialization;

    /// <summary>
    /// WebEx question type.
    /// </summary>
    public enum WebExQuestionType
    {
        /// <summary>
        /// A question that requires attendees to type text answers.
        /// </summary>
        [XmlEnum(Name = "text")]
        Essay,
        /// <summary>
        /// A question that requires attendees to type text answers in the blanks.
        /// </summary>
        [XmlEnum(Name = "blanks")]
        FillBlanks,
        /// <summary>
        /// Instructions that you can provide attendees in the beginning of a test.
        /// </summary>
        [XmlEnum(Name = "instr")]
        Instructions,
        /// <summary>
        /// A question that requires attendees to select one correct answer.
        /// </summary>
        [XmlEnum(Name = "mcone")]
        SingleChoice,
        /// <summary>
        /// A question that requires attendees to select more than one correct answer.
        /// </summary>
        [XmlEnum(Name = "mcmany")]
        MultipleChoice,
        /// <summary>
        /// A question that requires attendees to indicate whether the statement in the question is true or false.
        /// </summary>
        [XmlEnum(Name = "cond")]
        TrueFalse,
    }
}