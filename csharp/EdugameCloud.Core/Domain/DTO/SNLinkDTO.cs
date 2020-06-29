namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The SN Link DTO.
    /// </summary>
    [DataContract]
    public class SNLinkDTO 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNLinkDTO"/> class.
        /// </summary>
        public SNLinkDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNLinkDTO"/> class.
        /// </summary>
        /// <param name="link">
        /// The link.
        /// </param>
        public SNLinkDTO(SNLink link)
        {
            this.snLinkId = link.Id;
            this.snProfileId = link.Profile.Id;
            this.linkName = link.LinkName;
            this.linkValue = link.LinkValue;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string linkName { get; set; }

        /// <summary>
        /// Gets or sets the SN profile id.
        /// </summary>
        [DataMember]
        public int snProfileId { get; set; }

        /// <summary>
        /// Gets or sets the SN link id.
        /// </summary>
        [DataMember]
        public int snLinkId { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [DataMember]
        public string linkValue { get; set; }

        #endregion
    }
}