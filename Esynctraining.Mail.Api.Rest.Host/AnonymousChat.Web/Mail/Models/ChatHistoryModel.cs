namespace AnonymousChat.Web.Mail.Models
{
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer.Models;

    /// <summary>
    /// The chat history model.
    /// </summary>
    public class ChatHistoryModel : BaseTemplateModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatHistoryModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ChatHistoryModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the mail body.
        /// </summary>
        public string MailBody { get; set; }
    }
}