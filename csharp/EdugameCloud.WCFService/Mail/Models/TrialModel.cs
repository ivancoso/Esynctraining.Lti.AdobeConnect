﻿namespace EdugameCloud.WCFService.Mail.Models
{
    using Esynctraining.Core.Providers;
    
    /// <summary>
    /// The trial model.
    /// </summary>
    public class TrialModel : ActivationInvitationModel
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
        /// Gets or sets the trial days.
        /// </summary>
        public int TrialDays { get; set; }

        #endregion
    }
}