namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The user.
    /// </summary>
    public class UserActivation : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date expires.
        /// </summary>
        public virtual DateTime DateExpires { get; set; }

        /// <summary>
        /// Gets or sets the activation code.
        /// </summary>
        public virtual string ActivationCode { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }

        #endregion
    }
}