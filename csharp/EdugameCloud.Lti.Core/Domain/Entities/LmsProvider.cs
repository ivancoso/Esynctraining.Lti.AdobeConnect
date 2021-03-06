﻿namespace EdugameCloud.Lti.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The LMS provider.
    /// </summary>
    public class LmsProvider : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the LMS provider name.
        /// </summary>
        public virtual string LmsProviderName { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public virtual string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the configuration url.
        /// </summary>
        public virtual string ConfigurationUrl { get; set; }

        /// <summary>
        /// Gets or sets the user guide file url.
        /// </summary>
        public virtual string UserGuideFileUrl { get; set; }

        #endregion
    }
}