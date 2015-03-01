namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Extensions;

    /// <summary>
    ///     The SN profile with extra data DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(SimpleUserDTO))]
    public class SNProfileExtraDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileExtraDTO"/> class.
        /// </summary>
        public SNProfileExtraDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileExtraDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public SNProfileExtraDTO(SNProfileExtraFromStoredProcedureDTO dto)
        {
            this.snProfileId = dto.snProfileId;
            this.profileName = dto.profileName;
            this.categoryName = dto.categoryName;
            this.dateModified = dto.dateModified.ConvertToUnixTimestamp();
            this.subModuleCategoryId = dto.subModuleCategoryId;
            this.subModuleItemId = dto.subModuleItemId;
            this.createdBy = dto.createdBy;
            this.createdByFirstName = dto.createdByFirstName;
            this.createdByLastName = dto.createdByLastName;
            this.userId = dto.userId;
            this.firstName = dto.firstName;
            this.lastName = dto.lastName;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the SN profile id.
        /// </summary>
        [DataMember]
        public int snProfileId { get; set; }

        /// <summary>
        /// Gets or sets the SN profile name.
        /// </summary>
        [DataMember]
        public string profileName { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets id
        /// </summary>
        [DataMember]
        public int createdBy { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string createdByFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string createdByLastName { get; set; }

        /// <summary>
        /// Gets or sets id
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        #endregion
    }
}