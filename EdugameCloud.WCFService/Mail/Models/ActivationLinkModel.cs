namespace EdugameCloud.WCFService.Mail.Models
{
    using Esynctraining.Core.Providers;

    public class ActivationLinkModel : ActivationInvitationModel
    {
        
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationInvitationModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ActivationLinkModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        #endregion

    }
}