namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN map provider.
    /// </summary>
    public class SNMapProvider : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the map provider.
        /// </summary>
        public virtual string MapProvider { get; set; }

        #endregion
    }
}