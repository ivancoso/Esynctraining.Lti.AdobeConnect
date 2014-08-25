namespace Esynctraining.Core.Providers.Mailer.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    ///     Contains properties from mailer configuration section
    /// </summary>
    public class MailerConfigurationSection : ConfigurationSection
    {
        #region Static Fields

        /// <summary>
        ///     The current mailer configuration.
        /// </summary>
        private static MailerConfigurationSection current;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the current mailer configuration section. if it is not yet loaded in memory, it loads it into memory
        /// </summary>
        public static MailerConfigurationSection Current
        {
            get
            {
                if (current == null)
                {
                    try
                    {
                        object settings = ConfigurationManager.GetSection("mailerSettings");
                        if (settings != null)
                        {
                            current = settings as MailerConfigurationSection;
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                }

                return current;
            }
        }

        /// <summary>
        ///     Gets or sets the templates folder path.
        /// </summary>
        /// <value>
        ///     The templates folder path.
        /// </value>
        [ConfigurationProperty("attachmentsFolderPath", IsRequired = false)]
        public string AttachmentsFolderPath
        {
            get
            {
                return (string)this["attachmentsFolderPath"];
            }

            set
            {
                this["attachmentsFolderPath"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the templates folder path.
        /// </summary>
        /// <value>
        ///     The templates folder path.
        /// </value>
        [ConfigurationProperty("templatesFolderPath", IsRequired = true)]
        public string TemplatesFolderPath
        {
            get
            {
                return (string)this["templatesFolderPath"];
            }

            set
            {
                this["templatesFolderPath"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the images folder path.
        /// </summary>
        /// <value>
        ///     The templates folder path.
        /// </value>
        [ConfigurationProperty("imagesFolderPath", IsRequired = false)]
        public string ImagesFolderPath
        {
            get
            {
                return (string)this["imagesFolderPath"];
            }

            set
            {
                this["imagesFolderPath"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the templates folder path.
        /// </summary>
        /// <value>
        ///     The templates folder path.
        /// </value>
        [ConfigurationProperty("asciiTemplatesFolderPath", IsRequired = false)]
        public string AsciiTemplatesFolderPath
        {
            get
            {
                return (string)this["asciiTemplatesFolderPath"];
            }

            set
            {
                this["asciiTemplatesFolderPath"] = value;
            }
        }

        #endregion
    }
}