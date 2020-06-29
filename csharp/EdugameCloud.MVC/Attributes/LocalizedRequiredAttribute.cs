namespace EdugameCloud.MVC.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// The localized required attribute.
    /// </summary>
    public sealed class LocalizedRequiredAttribute : RequiredAttribute
    {
        #region Fields

        /// <summary>
        /// The resource fetcher.
        /// </summary>
        private readonly Func<string, string, string> resourceFetcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedRequiredAttribute"/> class.
        /// </summary>
        /// <param name="keyName">
        /// The key name.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        public LocalizedRequiredAttribute(string keyName, string resourceName)
            : this(keyName, resourceName, IoC.Resolve<IResourceProvider>().GetResourceString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedRequiredAttribute"/> class.
        /// </summary>
        /// <param name="keyName">
        /// The key name.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <param name="resourceFetcher">
        /// The resource fetcher.
        /// </param>
        public LocalizedRequiredAttribute(
            string keyName, string resourceName, Func<string, string, string> resourceFetcher)
        {
            this.resourceFetcher = resourceFetcher;
            this.ErrorMessageKeyName = keyName;
            this.ErrorMessageResourceName = string.IsNullOrWhiteSpace(resourceName) ? "None" : resourceName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the error message key name.
        /// </summary>
        public string ErrorMessageKeyName { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The format error message.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(this.ErrorMessageKeyName) || string.IsNullOrEmpty(this.ErrorMessageResourceName)
                || this.ErrorMessageResourceName == "None")
            {
                return base.FormatErrorMessage(name);
            }

            return this.resourceFetcher(this.ErrorMessageKeyName, this.ErrorMessageResourceName);
        }

        #endregion
    }
}