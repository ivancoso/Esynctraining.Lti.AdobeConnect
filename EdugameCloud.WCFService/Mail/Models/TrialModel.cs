namespace EdugameCloud.WCFService.Mail.Models
{
    using System.EnterpriseServices.Internal;

    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer.Models;

    /// <summary>
    /// The trial model.
    /// </summary>
    public class TrialModel : BaseTemplateModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrialModel"/> class. 
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public TrialModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the trial contact email.
        /// </summary>
        public string TrialContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the trial days.
        /// </summary>
        public int TrialDays { get; set; }

        #endregion
    }
}