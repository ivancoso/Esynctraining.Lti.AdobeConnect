namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The sub module item dto.
    /// </summary>
    [DataContract]
    public class SubModuleItemFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? themeId { get; set; }

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? createdBy { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public DateTime dateModified { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is shared.
        /// </summary>
        [DataMember]
        public bool? isShared { get; set; }

        /// <summary>
        ///     Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int? modifiedBy { get; set; }

        /// <summary>
        ///     Gets or sets the sub module category.
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