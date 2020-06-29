namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN profile SN service.
    /// </summary>
    public class SNProfileSNService : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the SN profile.
        /// </summary>
        public virtual SNProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets the SN service.
        /// </summary>
        public virtual SNService Service { get; set; }

        /// <summary>
        /// Gets or sets the service url.
        /// </summary>
        public virtual string ServiceUrl { get; set; }

        #endregion
    }
}