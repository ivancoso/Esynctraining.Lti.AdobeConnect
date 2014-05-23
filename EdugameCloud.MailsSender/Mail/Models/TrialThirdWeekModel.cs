namespace MailSender.Mail.Models
{
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer.Models;

    /// <summary>
    /// The trial model.
    /// </summary>
    public class TrialThirdWeekModel : BaseTemplateModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrialThirdWeekModel"/> class. 
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public TrialThirdWeekModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string FirstName { get; set; }

        #endregion
    }
}