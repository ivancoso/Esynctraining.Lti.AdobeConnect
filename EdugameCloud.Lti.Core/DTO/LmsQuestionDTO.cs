namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;
    using EdugameCloud.Lti.Domain.Entities;

    /// <summary>
    /// The quiz question DTO.
    /// </summary>
    public class LmsQuestionDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsQuestionDTO"/> class.
        /// </summary>
        public LmsQuestionDTO()
        {
            this.answers = new List<AnswerDTO>();
            this.datasets = new List<MoodleDataset>();
            this.files = new Dictionary<int, LmsQuestionFileDTO>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        public List<AnswerDTO> answers { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the question name.
        /// </summary>
        public string question_name { get; set; }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        public string question_text { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public string question_type { get; set; }

        /// <summary>
        /// Gets or sets the presentation.
        /// </summary>
        public string presentation { get; set; }

        /// <summary>
        /// Gets or sets the datasets.
        /// </summary>
        public List<MoodleDataset> datasets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is_single.
        /// </summary>
        public bool is_single { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is_mandatory.
        /// </summary>
        public bool is_mandatory { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        public Dictionary<int, LmsQuestionFileDTO> files { get; set; } 

        /// <summary>
        /// Indicates whether answers for this question should be checked for case sensitivity. Default value is false.
        /// </summary>
        public bool caseSensitive { get; set; }

        #endregion
    }
}