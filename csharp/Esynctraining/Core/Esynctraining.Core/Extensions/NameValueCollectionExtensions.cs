#if NET45 || NET461

namespace Esynctraining.Core.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Dynamic;

    /// <summary>
    /// The name value collection extensions.
    /// </summary>
    public static class NameValueCollectionExtensions
    {
#region Public Methods and Operators

        /// <summary>
        /// The has key.
        /// </summary>
        /// <param name="nvc">
        /// Name value collection
        /// </param>
        /// <param name="key">
        /// Key value
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasKey(this NameValueCollection nvc, string key)
        {
            if (nvc != null && nvc.HasKeys())
            {
                foreach (var keyVar in nvc.Keys)
                {
                    if (keyVar is string && keyVar.ToString().Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }
        
        /// <summary>
        /// Expands nave value collection to a dynamic object.
        /// </summary>
        /// <param name="valueCollection">
        /// The value collection.
        /// </param>
        /// <returns>
        /// The dynamic object result.
        /// </returns>
        public static dynamic ToExpandoWithAdditionalData(this NameValueCollection valueCollection, IEnumerable<KeyValuePair<string, string>> additionalData)
        {
            var extended = new NameValueCollection(valueCollection);
            foreach (var pair in additionalData)
            {
                extended.Add(pair.Key, pair.Value);
            }

            return extended.ToExpando();
        }

        /// <summary>
        /// Expands nave value collection to a dynamic object.
        /// </summary>
        /// <param name="valueCollection">
        /// The value collection.
        /// </param>
        /// <returns>
        /// The dynamic object result.
        /// </returns>
        public static dynamic ToExpando(this Hashtable valueCollection)
        {
            dynamic resultEx = new ExpandoObject();
            resultEx.Get = new Func<string, string>(
                key =>
                {
                    var res = resultEx as IDictionary<string, object>;
                    return res != null && res.ContainsKey(key) ? res[key].ToString() : string.Empty;
                });
            var result = resultEx as IDictionary<string, object>;
            foreach (string key in valueCollection.Keys)
            {
                result.Add(key, valueCollection[key]);
            }

            return result;
        }

        /// <summary>
        /// Expands nave value collection to a dynamic object.
        /// </summary>
        /// <param name="valueCollection">
        /// The value collection.
        /// </param>
        /// <returns>
        /// The dynamic object result.
        /// </returns>
        public static dynamic ToExpando(this NameValueCollection valueCollection)
        {
            dynamic resultEx = new ExpandoObject();
            resultEx.Get = new Func<string, string>(
                key =>
                    {
                        var res = resultEx as IDictionary<string, object>;
                        return res != null && res.ContainsKey(key) ? res[key].ToString() : string.Empty;
                    });
            var result = resultEx as IDictionary<string, object>;
            foreach (string key in valueCollection.AllKeys)
            {
                result.Add(key, valueCollection[key]);
            }

            return result;
        }

        /// <summary>
        /// The to string dictionary.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
        public static IDictionary<string, string> ToStringDictionary(this object obj)
        {
            var dic = new Dictionary<string, string>();
            dic.AddValues(obj);
            return dic;
        }

#endregion

#region Methods

        /// <summary>
        /// The add values.
        /// </summary>
        /// <param name="dic">
        /// The dictionary.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        private static void AddValues(this Dictionary<string, string> dic, object values)
        {
            if (values != null)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(values);
                foreach (PropertyDescriptor prop in props)
                {
                    object value = prop.GetValue(values);
                    if (value != null)
                    {
                        dic.Add(prop.Name, value.ToString());
                    }
                }
            }
        }

#endregion
    }

}

#endif