namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The contact login history.
    /// </summary>
    public class UserLoginHistory : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the from ip.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public virtual string FromIP { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public virtual string Application { get; set; }

        /// <summary>
        /// Gets or sets the contact.
        /// </summary>
        public virtual User User { get; set; }

        #endregion
    }
}