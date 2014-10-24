namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The moodle token dto.
    /// </summary>
    [DataContract]
    public class MoodleTokenDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleTokenDTO"/> class.
        /// </summary>
        public MoodleTokenDTO()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets error
        /// </summary>
        [DataMember]
        public virtual string error { get; set; }

        /// <summary>
        /// Gets or sets token
        /// </summary>
        [DataMember]
        public virtual string token { get; set; }

        #endregion
    }
}
