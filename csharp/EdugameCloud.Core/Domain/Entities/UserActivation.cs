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


        public static UserActivation Build(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return new UserActivation
            {
                User = user,
                ActivationCode = Guid.NewGuid().ToString(),
                DateExpires = DateTime.Now.AddDays(7),
            };
        }

    }

}