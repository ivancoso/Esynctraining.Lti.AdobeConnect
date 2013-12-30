namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The SN service.
    /// </summary>
    [DataContract]
    public class SNServiceDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNServiceDTO"/> class.
        /// </summary>
        public SNServiceDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNServiceDTO"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        public SNServiceDTO(SNService service)
        {
            if (service != null)
            {
                this.snServiceId = service.Id;
                this.socialService = service.SocialService;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the SN service id.
        /// </summary>
        [DataMember]
        public int snServiceId { get; set; }

        /// <summary>
        ///     Gets or sets the social service.
        /// </summary>
        [DataMember]
        public string socialService { get; set; }

        #endregion
    }
}