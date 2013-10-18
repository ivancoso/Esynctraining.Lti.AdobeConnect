namespace Esynctraining.Core.Providers.Mailer.Models
{
    /// <summary>
    /// The base template model.
    /// </summary>
    public class BaseTemplateModel
    {
        #region Fields

        /// <summary>
        /// The settings.
        /// </summary>
        protected readonly dynamic Settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTemplateModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BaseTemplateModel(ApplicationSettingsProvider settings)
        {
            this.Settings = settings;
        }

        #endregion

        /// <summary>
        /// Gets application url
        /// </summary>
        public string BaseUrl
        {
            get
            {
                var url = this.Settings.BasePath.ToString();
                return (url.StartsWith("http://") || url.StartsWith("https://")) ? url : "http://" + url;
            }
        }

        /// <summary>
        /// Gets the portal url.
        /// </summary>
        public string PortalUrl
        {
            get
            {
                var url = this.Settings.PortalUrl.ToString();
                return (url.StartsWith("http://") || url.StartsWith("https://")) ? url : "http://" + url;
            }
        }

        /// <summary>
        /// Gets or sets the mail subject.
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// Gets WCF service url
        /// </summary>
        public string BaseWCFServiceUrl
        {
            get
            {
                var url = this.Settings.BaseWCFServiceUrl.ToString();
                return (url.StartsWith("http://") || url.StartsWith("https://")) ? url : "http://" + url;
            }
        }
    }
}