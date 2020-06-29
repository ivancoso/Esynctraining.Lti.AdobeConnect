namespace EdugameCloud.Core.Providers.Mailer.Models
{
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer.Models;

    /// <summary>
    /// The change password model.
    /// </summary>
    public class ChangePasswordModel : BaseTemplateModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ChangePasswordModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the firstName.
        /// </summary>
        public string FirstName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether password changed.
        /// </summary>
        public bool PasswordChanged { get; set; }

        /// <summary>
        /// Gets or sets the trial contact email.
        /// </summary>
        public string TrialContactEmail { get; set; }

        #endregion
    }
}