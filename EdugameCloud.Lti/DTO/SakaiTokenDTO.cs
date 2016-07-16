namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The Sakai token DTO.
    /// </summary>
    [DataContract]
    public class SakaiTokenDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SakaiTokenDTO"/> class.
        /// </summary>
        public SakaiTokenDTO()
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
