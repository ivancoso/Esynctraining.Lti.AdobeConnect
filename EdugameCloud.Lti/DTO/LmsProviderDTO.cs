namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Lti.Domain.Entities;

    /// <summary>
    /// The LMS provider DTO.
    /// </summary>
    [DataContract]
    public class LmsProviderDTO
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsProviderDTO"/> class.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        public LmsProviderDTO(LmsProvider p)
        {
            this.lmsProviderId = p.Id;
            this.lmsProviderName = p.LmsProviderName;
            this.shortName = p.ShortName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the LMS provider id.
        /// </summary>
        [DataMember]
        public int lmsProviderId { get; set; }

        /// <summary>
        /// Gets or sets the LMS provider name.
        /// </summary>
        [DataMember]
        public string lmsProviderName { get; set; }

        /// <summary>
        /// Gets or sets the name without spaces.
        /// </summary>
        [DataMember]
        public string nameWithoutSpaces { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        [DataMember]
        public string shortName { get; set; }

        /// <summary>
        /// Gets or sets the config url.
        /// </summary>
        [DataMember]
        public string configUrl { get; set; }

        /// <summary>
        /// Gets or sets the instructions url.
        /// </summary>
        [DataMember]
        public string instructionsUrl { get; set; }

        #endregion
    }
}
