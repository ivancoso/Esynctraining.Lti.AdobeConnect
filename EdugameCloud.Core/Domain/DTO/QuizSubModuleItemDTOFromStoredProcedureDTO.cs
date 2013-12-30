namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The sub module item dto.
    /// </summary>
    [DataContract]
    public class QuizSubModuleItemDTOFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        [DataMember]
        public virtual int? createdBy { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public virtual DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public virtual DateTime dateModified { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public virtual bool? isActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is shared.
        /// </summary>
        [DataMember]
        public virtual bool? isShared { get; set; }

        /// <summary>
        ///     Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public virtual int? modifiedBy { get; set; }

        /// <summary>
        ///     Gets or sets the sub module category.
        /// </summary>
        [DataMember]
        public virtual int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module id.
        /// </summary>
        [DataMember]
        public virtual int subModuleId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public virtual int subModuleItemId { get; set; }

        #endregion
    }
}