namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The LMS quiz convert DTO
    /// </summary>
    [DataContract]
    public class LmsQuizConvertDTO
    {
        /// <summary>
        /// The quiz ids field.
        /// </summary>
        private int[] quizIdsField = { };

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        /// <summary>
        /// Gets or sets the LMS user parameters id.
        /// </summary>
        [DataMember]
        public virtual int lmsUserParametersId { get; set; }

        /// <summary>
        /// Gets or sets the quiz id.
        /// </summary>
        [DataMember]
        public virtual int[] quizIds
        {
            get
            {
                return this.quizIdsField ?? new int[] { };
            }

            set
            {
                this.quizIdsField = value;
            }
        }
    }
}
