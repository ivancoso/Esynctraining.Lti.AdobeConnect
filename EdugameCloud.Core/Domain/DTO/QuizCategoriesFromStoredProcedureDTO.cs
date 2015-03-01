namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The categories item DTO.
    /// </summary>
    [DataContract]
    public class QuizCategoriesFromStoredProcedureDTO2
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public virtual string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public virtual int? modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public virtual DateTime dateModified { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        [DataMember]
        public virtual bool? isActive { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public virtual int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module id.
        /// </summary>
        [DataMember]
        public virtual int subModuleId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        #endregion
    }
}