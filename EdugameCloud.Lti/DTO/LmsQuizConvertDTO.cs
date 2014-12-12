namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The LMS quiz convert DTO
    /// </summary>
    [DataContract]
    public class LmsQuizConvertDTO
    {
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
        /// Gets or sets the quiz ids.
        /// </summary>
        [DataMember]
        public virtual List<int> quizIds { get; set; }
    }
}
