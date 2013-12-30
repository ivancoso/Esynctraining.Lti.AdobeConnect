namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The build version type.
    /// </summary>
    public class BuildVersionType : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the build version name.
        /// </summary>
        public virtual string BuildVersionTypeName { get; set; }

        #endregion
    }
}