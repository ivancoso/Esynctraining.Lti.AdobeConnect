namespace MailSender.Mail.Models
{
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer.Models;

    /// <summary>
    /// The trial model.
    /// </summary>
    public class TrialFirstWeekModel : BaseTemplateModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrialFirstWeekModel"/> class. 
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public TrialFirstWeekModel(ApplicationSettingsProvider settings)
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