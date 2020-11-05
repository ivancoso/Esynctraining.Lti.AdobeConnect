#if NET45 || NET461
namespace Esynctraining.Core.Providers
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Dynamic;
    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The flex settings provider.
    /// </summary>
    public class FlexSettingsProvider : DynamicObject
    {
        #region Fields

        /// <summary>
        /// The collection.
        /// </summary>
        private readonly dynamic collection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexSettingsProvider"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public FlexSettingsProvider(NameValueCollection collection)
        {
            if (collection != null)
            {
                this.collection = collection.ToExpando();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The read settings.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The <see cref="FlexSettingsProvider"/>.
        /// </returns>
        public static NameValueCollection ReadSettings(string filePath)
        {
            var settings = new NameValueCollection();
            if (System.IO.File.Exists(filePath))
            {
                var lines = System.IO.File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                    {
                        var setting = line.Split("=".ToCharArray());
                        if (setting.Length > 1)
                        {
                            var key = setting[0];
                            var value = setting[1];
                            if (!settings.HasKey(key))
                            {
                                settings.Add(key, value);
                            }
                            else
                            {
                                settings[key] += ";" + value;
                            }
                        }
                    }
                }
            }

            return settings;
        }

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
}

#endif