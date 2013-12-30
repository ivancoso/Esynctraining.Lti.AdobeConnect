namespace EdugameCloud.MVC.Attributes
{
    using System;
    using System.ComponentModel;

    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// Provides localizable display name meta information
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        #region Fields

        /// <summary>
        /// The resource fetcher.
        /// </summary>
        private readonly Func<string, string, string> resourceFetcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the LocalizedDisplayNameAttribute class
        /// </summary>
        /// <param name="displayNameKey">
        /// Display name key within the resource file
        /// </param>
        public LocalizedDisplayNameAttribute(string displayNameKey)
            : this(displayNameKey, IoC.Resolve<IResourceProvider>().GetResourceString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedDisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="displayNameKey">
        /// The display name key.
        /// </param>
        /// <param name="resourceFetcher">
        /// The resource fetcher.
        /// </param>
        public LocalizedDisplayNameAttribute(string displayNameKey, Func<string, string, string> resourceFetcher)
            : base(displayNameKey)
        {
            this.resourceFetcher = resourceFetcher;
            this.DisplayNameKey = displayNameKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets localized string according to key provided to constructor
        /// </summary>
        public override string DisplayName
        {
            get
            {
                string result = string.IsNullOrEmpty(this.DisplayNameKey)
                                    ? base.DisplayName
                                    : this.resourceFetcher(this.DisplayNameKey, this.ResourceName);
                return result;
            }
        }

        /// <summary>
        /// Gets or sets the display name key.
        /// </summary>
        public string DisplayNameKey { get; set; }

        /// <summary>
        /// Gets or sets resource manager type used to access resources within particular resource file
        /// </summary>
        public string ResourceName { get; set; }

        #endregion
    }
}