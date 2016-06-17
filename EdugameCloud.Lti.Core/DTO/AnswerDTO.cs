namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;

    /// <summary>
    /// The answer DTO.
    /// </summary>
    public class AnswerDTO
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerDTO"/> class.
        /// </summary>
        public AnswerDTO()
        {
            this.variables = new List<VariableDTO>();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        public string left { get; set; }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        public string right { get; set; }

        /// <summary>
        /// Gets or sets the answer_text.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the answer_weight.
        /// </summary>
        public int weight { get; set; }

        /// <summary>
        /// Gets or sets the exact.
        /// </summary>
        public double exact { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the blank_id.
        /// </summary>
        public string blank_id { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        public double answer { get; set; }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        public double margin { get; set; }

        /// <summary>
        /// Gets or sets the variables.
        /// </summary>
        public List<VariableDTO> variables { get; set; }

        /// <summary>
        /// Gets or sets the match_id.
        /// </summary>
        public string match_id { get; set; }

        /// <summary>
        /// Gets or sets the numerical_answer_type.
        /// </summary>
        public string numerical_answer_type { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public double start { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        public double end { get; set; }

        /// <summary>
        /// Gets or sets the question_type.
        /// </summary>
        public string question_type { get; set; }

        /// <summary>
        /// Gets or sets the question_text.
        /// </summary>
        public string question_text { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int order { get; set; }

        /// <summary>
        /// Gets or sets the html.
        /// </summary>
        public string html { get; set; }

        public string fileData { get; set; }

        /// <summary>
        /// Indicates whether answer should be checked for case sensitivity. Default value is false.
        /// </summary>
        /// todo: will be removed from answer if this is question-level property
        public bool caseSensitive { get; set; }
        
        #endregion
    }
}