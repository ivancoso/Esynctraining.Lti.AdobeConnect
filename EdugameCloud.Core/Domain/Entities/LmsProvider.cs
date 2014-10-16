namespace EdugameCloud.Core.Domain.Entities
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

        #endregion
    }
}