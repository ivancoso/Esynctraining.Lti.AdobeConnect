namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The sub module category
    /// </summary>
    public class SubModuleCategory : Entity
    {
        #region Fields

        /// <summary>
        /// The sub module items.
        /// </summary>
        private ISet<SubModuleItem> subModuleItems = new HashedSet<SubModuleItem>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the category name.
        /// </summary>
        public virtual string CategoryName { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        ///     Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        ///     Gets or sets the sub module.
        /// </summary>
        public virtual SubModule SubModule { get; set; }

        /// <summary>
        /// Gets or sets the sub module items.
        /// </summary>
        public virtual ISet<SubModuleItem> SubModuleItems
        {
            get
            {
                return this.subModuleItems;
            }

            set
            {
                this.subModuleItems = value;
            }
        }

        /// <summary>
        ///     Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the LMS course id.
        /// </summary>
        public virtual int? LmsCourseId { get; set; }

        /// <summary>
        /// Gets or sets the company LMS.
        /// </summary>
        public virtual int? CompanyLmsId { get; set; }

        #endregion
    }
}