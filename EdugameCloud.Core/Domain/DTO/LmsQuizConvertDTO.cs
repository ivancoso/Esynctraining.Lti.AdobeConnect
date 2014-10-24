namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The lms quiz convert dto
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
        /// Gets or sets the lms user parameters id.
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
