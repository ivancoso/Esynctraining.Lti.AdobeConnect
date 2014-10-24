﻿namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The moodle course info dto.
    /// </summary>
    [DataContract]
    public class MoodleCourseInfoDTO
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the fullname.
        /// </summary>
        [DataMember]
        public string fullname { get; set; }

        /// <summary>
        /// Gets or sets the quizzes.
        /// </summary>
        [DataMember]
        public List<MoodleQuizInfoDTO> quizzes { get; set; }
    }
}
