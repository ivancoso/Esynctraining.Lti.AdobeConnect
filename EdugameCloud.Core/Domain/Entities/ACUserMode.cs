namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     AC User Mode
    /// </summary>
    public class ACUserMode : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the File.
        /// </summary>
        public virtual File Image { get; set; }

        /// <summary>
        /// Gets or sets the user mode.
        /// </summary>
        public virtual string UserMode { get; set; }

        #endregion
    }
}