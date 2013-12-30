namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The sub module category dto.
    /// </summary>
    [DataContract]
    public class SubModuleCategoryDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleCategoryDTO"/> class.
        /// </summary>
        public SubModuleCategoryDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleCategoryDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public SubModuleCategoryDTO(SubModuleCategory result)
        {
            this.subModuleCategoryId = result.Id;
            this.userId = result.User.With(x => x.Id);
            this.subModuleId = result.SubModule.With(x => x.Id);
            this.modifiedBy = result.ModifiedBy.Return(x => x.Id, (int?)null);
            this.categoryName = result.CategoryName;
            this.dateModified = result.DateModified;
            this.isActive = result.IsActive;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public virtual string categoryName { get; set; }

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
        ///     Gets or sets the created by.
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        #endregion
    }
}