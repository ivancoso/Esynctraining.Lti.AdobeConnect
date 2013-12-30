namespace EdugameCloud.Core.Providers.Mailer.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Contains properties from mailer configuration section
    /// </summary>
    public class MailerConfigurationSection : ConfigurationSection
    {
        private static MailerConfigurationSection current;

        /// <summary>
        /// Gets or sets the templates folder path.
        /// </summary>
        /// <value>
        /// The templates folder path.
        /// </value>
        [ConfigurationProperty("templatesFolderPath", IsRequired = true )]
        public string TemplatesFolderPath
        {
            get { return (string)this["templatesFolderPath"]; }
            set { this["templatesFolderPath"] = value; }
        }

        /// <summary>
        /// Gets or sets the templates folder path.
        /// </summary>
        /// <value>
        /// The templates folder path.
        /// </value>
        [ConfigurationProperty("attachmentsFolderPath", IsRequired = false)]
        public string AttachmentsFolderPath
        {
            get { return (string)this["attachmentsFolderPath"]; }
            set { this["attachmentsFolderPath"] = value; }
        }

        /// <summary>
        /// Gets the current mailer configuration section. if it is not yet loaded in memory, it loads it into memory
        /// </summary>
        public static MailerConfigurationSection Current
        {
            get
            {
                if (current == null)
                {
                    var settings = ConfigurationManager.GetSection("mailerSettings");
                    if (settings != null)
                    {
                        current = settings as MailerConfigurationSection;
                    }
                }

                return current;
            }
        }
    }
}
