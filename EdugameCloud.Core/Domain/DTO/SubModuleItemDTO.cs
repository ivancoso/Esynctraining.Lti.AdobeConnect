namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The sub module item DTO.
    /// </summary>
    [DataContract]
    public class SubModuleItemDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemDTO"/> class.
        /// </summary>
        public SubModuleItemDTO()
        {
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
            this.isActive = result.IsActive;
            this.isShared = result.IsShared;
            this.dateCreated = result.DateCreated;
            this.dateModified = result.DateModified;
        }

        #endregion

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
        /// Gets or sets a value indicating whether is shared.
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
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public virtual int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the sub module id.
        /// </summary>
        [DataMember]
        public virtual int subModuleId { get; set; }

        #endregion
    }
}