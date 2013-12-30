// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivationInvitationModel.cs" company="">
//   
// </copyright>
// <summary>
//   The account created model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.WCFService.Mail.Models
{
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer.Models;

    /// <summary>
    ///     The account created model.
    /// </summary>
    public class ActivationInvitationModel : BaseTemplateModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationInvitationModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ActivationInvitationModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the activation code.
        /// </summary>
        public string ActivationCode { get; set; }

        /// <summary>
        /// Gets the activation link.
        /// </summary>
        public string ActivationLink
        {
            get
            {
                return (string)this.Settings.PortalUrl + "activate?code=" + this.ActivationCode;
            }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the trial contact email.
        /// </summary>
        public string TrialContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; }

        #endregion
    }
}