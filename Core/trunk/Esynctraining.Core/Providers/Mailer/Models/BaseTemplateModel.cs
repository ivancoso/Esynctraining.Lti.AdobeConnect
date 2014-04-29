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
        /// Gets the base url for images.
        /// </summary>
        public string BaseUrlForImages
        {
            get
            {
                return this.CEUrl + "file/get?id=";
            }
        }

        /// <summary>
        /// Gets the CE url.
        /// </summary>
        public string CEUrl
        {
            get
            {
                return this.BaseUrl.Replace("services/", string.Empty);
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