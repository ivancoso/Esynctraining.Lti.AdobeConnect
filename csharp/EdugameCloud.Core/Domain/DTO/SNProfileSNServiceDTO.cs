namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The SN profile SN service.
    /// </summary>
    [DataContract]
    public class SNProfileSNServiceDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileSNServiceDTO"/> class.
        /// </summary>
        public SNProfileSNServiceDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileSNServiceDTO"/> class.
        /// </summary>
        /// <param name="profileService">
        /// The profile service.
        /// </param>
        public SNProfileSNServiceDTO(SNProfileSNService profileService)
        {
            if (profileService != null)
            {
                this.snProfileSNServiceId = profileService.Id;
                this.snProfileId = profileService.Profile.Id;
                this.snServiceId = profileService.Service.Id;
                this.serviceUrl = profileService.ServiceUrl;
                this.isEnabled = profileService.IsEnabled;
            }
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is enabled.
        /// </summary>
        [DataMember]
        public bool isEnabled { get; set; }

        /// <summary>
        /// Gets or sets the SN profile.
        /// </summary>
        [DataMember]
        public int snProfileId { get; set; }

        /// <summary>
        /// Gets or sets the SN service.
        /// </summary>
        [DataMember]
        public int snServiceId { get; set; }

        /// <summary>
        /// Gets or sets the service url.
        /// </summary>
        [DataMember]
        public string serviceUrl { get; set; }

        /// <summary>
        /// Gets or sets the SN profile SN service id.
        /// </summary>
        [DataMember]
        public int snProfileSNServiceId { get; set; }

        #endregion
    }
}