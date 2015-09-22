namespace EdugameCloud.Core.Domain.DTO
{
    using System.Linq;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The sub module item DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(SubModuleItemThemeDTO))]
    public class SubModuleItemDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemDTO"/> class.
        /// </summary>
        public SubModuleItemDTO()
        {
            this.themeVO = new SubModuleItemThemeDTO();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public SubModuleItemDTO(SubModuleItemFromStoredProcedureDTO dto)
        {
            this.themeId = dto.themeId;
            this.createdBy = dto.createdBy;
            this.dateCreated = dto.dateCreated.ConvertToUnixTimestamp();
            this.dateModified = dto.dateModified.ConvertToUnixTimestamp();
            this.isActive = dto.isActive;
            this.isShared = dto.isShared;
            this.modifiedBy = dto.modifiedBy;
            this.subModuleCategoryId = dto.subModuleCategoryId;
            this.subModuleItemId = dto.subModuleItemId;
            this.subModuleId = dto.subModuleId;
            this.themeVO = dto.themeVO;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public SubModuleItemDTO(SubModuleItem result)
        {
            this.subModuleItemId = result.Id;
            this.subModuleId = result.SubModuleCategory.With(x => x.SubModule).With(x => x.Id);
            this.subModuleCategoryId = result.SubModuleCategory.With(x => x.Id);
            this.createdBy = result.CreatedBy.Return(x => x.Id, (int?)null);
            this.modifiedBy = result.ModifiedBy.Return(x => x.Id, (int?)null);
            this.themeId = null;
            this.isActive = result.IsActive;
            this.isShared = result.IsShared;
            this.dateCreated = result.DateCreated.ConvertToUnixTimestamp();
            this.dateModified = result.DateModified.ConvertToUnixTimestamp();
            var theme = result.Themes.FirstOrDefault();
            if (theme != null)
            {
                this.themeId = theme.Id;
                this.themeVO = new SubModuleItemThemeDTO(theme);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? themeId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? createdBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is shared.
        /// </summary>
        [DataMember]
        public bool? isShared { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int? modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the sub module category.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the sub module id.
        /// </summary>
        [DataMember]
        public int subModuleId { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        [DataMember]
        public SubModuleItemThemeDTO themeVO { get; set; }

        #endregion
    }
}