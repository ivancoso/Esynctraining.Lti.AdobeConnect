using System.Runtime.Serialization;

namespace Esynctraining.Lti.Lms.Common.Dto
{
    /// <summary>
    /// The LMS quiz info DTO.
    /// </summary>
    [DataContract]
    public class LmsQuizInfoDTO
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsQuizInfoDTO"/> class.
        /// </summary>
        public LmsQuizInfoDTO()
        {
            this.isPublished = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int id { get; set; }

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
        /// Gets or sets the last modified LMS.
        /// </summary>
        [DataMember]
        public int lastModifiedLMS { get; set; }

        /// <summary>
        /// Gets or sets the last modified EGC.
        /// </summary>
        [DataMember]
        public int lastModifiedEGC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is published.
        /// </summary>
        [DataMember]
        public bool isPublished { get; set; }
        
        #endregion
    }
}
