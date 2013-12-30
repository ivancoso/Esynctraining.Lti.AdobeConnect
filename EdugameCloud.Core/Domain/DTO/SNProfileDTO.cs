namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The SN Profile.
    /// </summary>
    [DataContract]
    [KnownType(typeof(AddressDTO))]
    [KnownType(typeof(SNLinkDTO))]
    [KnownType(typeof(SNProfileSNServiceDTO))]
    [KnownType(typeof(SNMapSettingsDTO))]
    public class SNProfileDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileDTO"/> class.
        /// </summary>
        public SNProfileDTO()
        {
            this.links = new List<SNLinkDTO>();
            this.services = new List<SNProfileSNServiceDTO>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileDTO"/> class.
        /// </summary>
        /// <param name="profile">
        /// The SN profile.
        /// </param>
        public SNProfileDTO(SNProfile profile)
        {
            if (profile != null)
            {
                this.snProfileId = profile.Id;
                this.about = profile.About;
                this.addressVO = profile.Address == null ? null : new AddressDTO(profile.Address);
                this.email = profile.Email;
                this.links = profile.Links.Select(x => new SNLinkDTO(x)).ToList();
                this.services = profile.Services.Select(x => new SNProfileSNServiceDTO(x)).ToList();
                this.profileName = profile.ProfileName;
                this.userName = profile.UserName;
                this.jobTitle = profile.JobTitle;
                this.phone = profile.Phone;
                this.mapSettingsVO = profile.MapSettings == null ? null : new SNMapSettingsDTO(profile.MapSettings);
                this.subModuleItemId = profile.SubModuleItem.With(x => x.Id);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the map settings.
        /// </summary>
        [DataMember]
        public SNMapSettingsDTO mapSettingsVO { get; set; }

        /// <summary>
        ///     Gets or sets the about.
        /// </summary>
        [DataMember]
        public string about { get; set; }

        /// <summary>
        ///     Gets or sets the address.
        /// </summary>
        [DataMember]
        public AddressDTO addressVO { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        [DataMember]
        public List<SNLinkDTO> links { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string profileName { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string jobTitle { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DataMember]
        public string userName { get; set; }

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        [DataMember]
        public string phone { get; set; }

        /// <summary>
        ///     Gets or sets the services.
        /// </summary>
        [DataMember]
        public List<SNProfileSNServiceDTO> services { get; set; }

        /// <summary>
        ///     Gets or sets the SN profile id.
        /// </summary>
        [DataMember]
        public int snProfileId { get; set; }

        #endregion
    }
}