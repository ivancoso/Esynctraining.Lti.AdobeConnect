namespace EdugameCloud.MVC.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// The localized range attribute.
    /// </summary>
    public class LocalizedRangeAttribute : RangeAttribute
    {
        #region Fields

        /// <summary>
        /// The resource fetcher.
        /// </summary>
        private readonly Func<string, string, string> resourceFetcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedRangeAttribute"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="minimum">
        /// The minimum.
        /// </param>
        /// <param name="maximum">
        /// The maximum.
        /// </param>
        /// <param name="keyName">
        /// The key name.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        public LocalizedRangeAttribute(Type type, string minimum, string maximum, string keyName, string resourceName)
            : this(type, minimum, maximum, keyName, resourceName, IoC.Resolve<IResourceProvider>().GetResourceString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedRangeAttribute"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="minimum">
        /// The minimum.
        /// </param>
        /// <param name="maximum">
        /// The maximum.
        /// </param>
        /// <param name="keyName">
        /// The key name.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <param name="resourceFetcher">
        /// The resource fetcher.
        /// </param>
        public LocalizedRangeAttribute(
            Type type, 
            string minimum, 
            string maximum, 
            string keyName, 
            string resourceName, 
            Func<string, string, string> resourceFetcher)
            : base(type, minimum, maximum)
        {
            this.ErrorMessageKeyName = keyName;
            this.resourceFetcher = resourceFetcher;
            this.ErrorMessageResourceName = string.IsNullOrWhiteSpace(resourceName) ? "None" : resourceName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the error message key name.
        /// </summary>
        public string ErrorMessageKeyName { get; set; }

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