namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The sub module
    /// </summary>
    public class SubModule : Entity
    {
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
        public virtual IList<SubModuleCategory> SubModuleCategories { get; protected set; }

        /// <summary>
        ///     Gets or sets the sub module name.
        /// </summary>
        public virtual string SubModuleName { get; set; }

        #endregion
    }
}