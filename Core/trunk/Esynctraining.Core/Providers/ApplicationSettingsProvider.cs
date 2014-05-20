namespace Esynctraining.Core.Providers
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Dynamic;
    using System.Web.Configuration;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The application settings provider.
    /// </summary>
    public class ApplicationSettingsProvider : DynamicObject
    {
        #region Fields

        /// <summary>
        /// The collection.
        /// </summary>
        private readonly dynamic collection;

        /// <summary>
        ///     The default UI language.
        /// </summary>
        private readonly string defaultUILanguage = "en-US";

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString = string.Empty;
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsProvider"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="globalizationSection">
        /// The globalization section.
        /// </param>
        public ApplicationSettingsProvider(
            NameValueCollection collection,
            GlobalizationSection globalizationSection = null)
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["Database"].With(x => x.ConnectionString);
            if (collection != null)
            {
                this.collection = collection.ToExpandoWithAdditionalData(new[] { new KeyValuePair<string, string>("ConnectionString", this.connectionString) });
            }

            if (globalizationSection != null)
            {
                this.defaultUILanguage = globalizationSection.UICulture;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets default UI language.
        /// </summary>
        public string DefaultUILanguage
        {
            get
            {
                return this.defaultUILanguage;
            }
        }

        /// <summary>
        ///     Gets connection string.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get dynamic member names.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.collection.AllKeys;
        }

        /// <summary>
        /// The try get member.
        /// </summary>
        /// <param name="binder">
        /// The binder.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Configuration exception if requested key is not found in config
        /// </exception>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.collection.Get(binder.Name);
            if (string.IsNullOrWhiteSpace(result.ToString()))
            {
                return true;
            }

            return true;
        }

        #endregion
    }

    /*
    public class ApplicationSettingsProvider : IApplicationSettingsProvider
    {
        #region Fields

        /// <summary>
        /// The default UI language.
        /// </summary>
        private readonly string defaultUILanguage = "en-US";

        /// <summary>
        /// The file storage.
        /// </summary>
        private readonly string fileStorage = "~/FileStorage";

        /// <summary>
        /// The left menu path.
        /// </summary>
        private readonly string leftMenuPath = "~/TopMenu.xml";

        /// <summary>
        /// The page size.
        /// </summary>
        private readonly int pageSize = 20;

        /// <summary>
        /// The perm file pattern.
        /// </summary>
        private readonly string permFilePattern = "{userId}.{fileId}.bin";

        /// <summary>
        /// The temp file pattern.
        /// </summary>
        private readonly string tempFilePattern = "temp/{userId}.{fileId}.{counter}.chunk";

        /// <summary>
        /// The authentication header name.
        /// </summary>
        private string authHeaderName = "iSpye-Auth";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsProvider"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="globalizationSection">
        /// The globalization section.
        /// </param>
        /// <exception cref="ApplicationException">
        /// Patterns are corrupted
        /// </exception>
        public ApplicationSettingsProvider(NameValueCollection collection, GlobalizationSection globalizationSection)
        {
            if (collection != null)
            {
                int settingValue;

                string setting = collection[Lambda.Property<IApplicationSettingsProvider>(x => x.PageSize)];
                if (setting != null && int.TryParse(setting, out settingValue))
                {
                    this.pageSize = settingValue;
                }

                setting = collection[Lambda.Property<IApplicationSettingsProvider>(x => x.AuthHeaderName)];
                if (setting != null)
                {
                    this.authHeaderName = setting;
                }

                setting = collection[Lambda.Property<IApplicationSettingsProvider>(x => x.LeftMenuPath)];
                if (setting != null)
                {
                    this.leftMenuPath = setting;
                }

                setting = collection[Lambda.Property<IApplicationSettingsProvider>(x => x.FileStorage)];
                if (setting != null)
                {
                    this.fileStorage = setting;
                    if (HttpContext.Current != null && setting.StartsWith("~"))
                    {
                        this.fileStorage = HttpContext.Current.Server.MapPath(this.fileStorage);
                    }
                }

                setting = collection[Lambda.Property<IApplicationSettingsProvider>(x => x.TempFilePattern)];
                if (setting != null)
                {
                    this.tempFilePattern = setting;
                    if (!setting.Contains("{userId}") || !setting.Contains("{fileId}") || !setting.Contains("{counter}"))
                    {
                        throw new ApplicationException(
                            "{userId}, {fileId} and {counter} are required parts of the temp file pattern");
                    }
                }

                setting = collection[Lambda.Property<IApplicationSettingsProvider>(x => x.PermFilePattern)];
                if (setting != null)
                {
                    this.permFilePattern = setting;
                    if (!setting.Contains("{userId}") || !setting.Contains("{fileId}"))
                    {
                        throw new ApplicationException(
                            "{userId} and {fileId} are required parts of the permanent file pattern");
                    }
                }

                if (globalizationSection != null)
                {
                    this.defaultUILanguage = globalizationSection.UICulture;
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets authentication header name.
        /// </summary>
        public string AuthHeaderName
        {
            get
            {
                return this.authHeaderName;
            }
            private set
            {
                this.authHeaderName = value;
            }
        }

        /// <summary>
        /// Gets default UI language.
        /// </summary>
        public string DefaultUILanguage
        {
            get
            {
                return this.defaultUILanguage;
            }
        }

        /// <summary>
        /// Gets the file storage.
        /// </summary>
        public string FileStorage
        {
            get
            {
                return this.fileStorage;
            }
        }

        /// <summary>
        /// Gets the left menu path.
        /// </summary>
        public string LeftMenuPath
        {
            get
            {
                return this.leftMenuPath;
            }
        }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        public int PageSize
        {
            get
            {
                return this.pageSize;
            }
        }

        /// <summary>
        /// Gets the perm file pattern.
        /// </summary>
        public string PermFilePattern
        {
            get
            {
                return this.permFilePattern;
            }
        }

        /// <summary>
        /// Gets the temp file pattern.
        /// </summary>
        public string TempFilePattern
        {
            get
            {
                return this.tempFilePattern;
            }
        }

        #endregion
    }*/
}