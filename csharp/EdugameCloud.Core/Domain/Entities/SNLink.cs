namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN Link.
    /// </summary>
    public class SNLink : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string LinkName { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        public virtual SNProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public virtual string LinkValue { get; set; }

        #endregion
    }
}