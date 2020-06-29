// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicenseUpgradeModel.cs" company="">
//   
// </copyright>
// <summary>
//   The trial model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.WCFService.Mail.Models
{
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer.Models;

    /// <summary>
    ///     The trial model.
    /// </summary>
    public class LicenseUpgradeModel : BaseTemplateModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseUpgradeModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public LicenseUpgradeModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        public string ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is trial.
        /// </summary>
        public bool IsTrial { get; set; }

        /// <summary>
        /// Gets or sets the primary email.
        /// </summary>
        public string PrimaryEmail { get; set; }

        /// <summary>
        /// Gets or sets the primary name.
        /// </summary>
        public string PrimaryName { get; set; }

        /// <summary>
        /// Gets or sets the seats count.
        /// </summary>
        public int SeatsCount { get; set; }

        #endregion
    }
}