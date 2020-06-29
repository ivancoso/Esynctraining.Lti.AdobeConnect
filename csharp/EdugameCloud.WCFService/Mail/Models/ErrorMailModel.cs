namespace EdugameCloud.WCFService.Mail.Models
{
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer.Models;

    public class ErrorMailModel : BaseTemplateModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrialModel"/> class. 
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ErrorMailModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }


        #endregion


        #region Public Properties

        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }

        #endregion
    }
}