namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The moodle quiz info DTO.
    /// </summary>
    [DataContract]
    public class MoodleQuizInfoDTO
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        [DataMember]
        public string course { get; set; }

        /// <summary>
        /// Gets or sets the course name.
        /// </summary>
        [DataMember]
        public string courseName { get; set; }

        /// <summary>
        /// Gets or sets the last modified moodle.
        /// </summary>
        [DataMember]
        public int lastModifiedMoodle { get; set; }

        /// <summary>
        /// Gets or sets the last modified EGC.
        /// </summary>
        [DataMember]
        public int lastModifiedEGC { get; set; }
    }
}
