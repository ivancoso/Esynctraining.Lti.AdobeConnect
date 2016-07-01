using EdugameCloud.Lti.BlackBoard;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.DTO
{
    /// <summary>
    /// The bb question dto.
    /// </summary>
    public class BBQuestionDTO
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        public string answer { get; set; }

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        public object answers { get; set; }

        /// <summary>
        /// Gets or sets the answers list.
        /// </summary>
        //[JsonConverter(typeof(BbAnswerJsonConverter))]
        public object answersList { get; set; }

        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        public object choices { get; set; }

        /// <summary>
        /// Gets or sets the answer range.
        /// </summary>
        public string answerRange { get; set; }

        /// <summary>
        /// Gets or sets the answers choices.
        /// </summary>
        public string answersChoices { get; set; }

        /// <summary>
        /// Gets or sets the tolerance.
        /// </summary>
        public string tolerance { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        public string formula { get; set; }

        /// <summary>
        /// Gets or sets the variable sets.
        /// </summary>
        public object variableSets { get; set; }

        /// <summary>
        /// Gets or sets the question words list.
        /// </summary>
        public string[] questionWordsList { get; set; }

        /// <summary>
        /// Gets or sets the answer phrases list.
        /// </summary>
        public string[] answerPhrasesList { get; set; }

        /// <summary>
        /// Indicates whether answers for this question should be checked for case sensitivity. Default value is false.
        /// </summary>
        /// note: currently is not used
        public bool caseSensitive { get; set; }

        public string questionImageLink { get; set; }
        public string questionImageBinary { get; set; }

        public int rows { get; set; }

    }
}
