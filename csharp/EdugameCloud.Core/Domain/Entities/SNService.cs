namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN service.
    /// </summary>
    public class SNService : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the social service.
        /// </summary>
        public virtual string SocialService { get; set; }

        #endregion
    }
}