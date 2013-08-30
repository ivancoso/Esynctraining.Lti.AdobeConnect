// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginResult.cs" company="eSyncTraining">
//   eSyncTraining
// </copyright>
// <summary>
//   Defines the LoginResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The login result.
    /// </summary>
    public class LoginResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public LoginResult(StatusInfo status) : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        public LoginResult(StatusInfo status, UserInfo user) : base(status)
        {
            this.User = user;
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// Gets a value indicating whether success.
        /// </summary>
        public override bool Success
        {
            get
            {
                return base.Success && this.User != null;
            }
        }
    }
}
