namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Extensions;

    /// <summary>
    ///     The categories item DTO.
    /// </summary>
    [DataContract]
    public class SMICategoriesFromStoredProcedureDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SMICategoriesFromStoredProcedureDTO"/> class.
        /// </summary>
        public SMICategoriesFromStoredProcedureDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SMICategoriesFromStoredProcedureDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public SMICategoriesFromStoredProcedureDTO(SMICategoriesFromStoredProcedureExDTO dto)
        {
            this.categoryName = dto.categoryName;
            this.modifiedBy = dto.modifiedBy;
            this.dateModified = dto.dateModified.ConvertToUnixTimestamp();
            this.isActive = dto.isActive;
            this.subModuleCategoryId = dto.subModuleCategoryId;
            this.subModuleId = dto.subModuleId;
            this.userId = dto.userId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module id.
        /// </summary>
        [DataMember]
        public int subModuleId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        #endregion
    }
}