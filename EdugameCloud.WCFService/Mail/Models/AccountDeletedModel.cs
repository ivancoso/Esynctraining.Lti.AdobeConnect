namespace EdugameCloud.Core.Providers.Mailer.Models
{
    using Esynctraining.Core.Providers;

    /// <summary>
    /// The account deleted model.
    /// </summary>
    public class AccountDeletedModel : ChangePasswordModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountDeletedModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public AccountDeletedModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        #endregion
    }
}