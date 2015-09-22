namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The module.
    /// </summary>
    public class Module : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the module name.
        /// </summary>
        public virtual string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the sub modules.
        /// </summary>
        public virtual IList<SubModule> SubModules { get; protected set; }

        #endregion

        public Module()
        {
            SubModules = new List<SubModule>();
        }

    }

}