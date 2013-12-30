namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The sub module
    /// </summary>
    public class SubModule : Entity
    {
        #region Fields

        /// <summary>
        /// The sub module categories.
        /// </summary>
        private ISet<SubModuleCategory> subModuleCategories = new HashedSet<SubModuleCategory>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        ///     Gets or sets the module.
        /// </summary>
        public virtual Module Module { get; set; }

        /// <summary>
        /// Gets or sets the sub module categories.
        /// </summary>
        public virtual ISet<SubModuleCategory> SubModuleCategories
        {
            get
            {
                return this.subModuleCategories;
            }

            set
            {
                this.subModuleCategories = value;
            }
        }

        /// <summary>
        ///     Gets or sets the sub module name.
        /// </summary>
        public virtual string SubModuleName { get; set; }

        #endregion
    }
}