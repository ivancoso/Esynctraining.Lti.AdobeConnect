namespace Esynctraining.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Dynamic;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The application settings provider.
    /// </summary>
    public class ApplicationSettingsProvider : DynamicObject
    {
        private readonly NameValueCollection collection;
        private readonly string defaultUILanguage = "en-US";
        private readonly string connectionString = string.Empty;


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsProvider"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public ApplicationSettingsProvider(NameValueCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            this.connectionString = ConfigurationManager.ConnectionStrings["Database"].With(x => x.ConnectionString);
            var extended = new NameValueCollection(collection);
            foreach (var pair in new[] { new KeyValuePair<string, string>("ConnectionString", this.connectionString) })
            {
                extended.Add(pair.Key, pair.Value);
            }
            this.collection = extended;
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
            // TRICK: not to break old code
            if (result == null)
                result = string.Empty;
            return true;
        }
        
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.collection.Set(binder.Name, value == null ? null : value.ToString());
            return true;
        }

        #endregion
    }

}