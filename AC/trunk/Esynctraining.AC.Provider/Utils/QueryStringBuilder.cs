﻿namespace Esynctraining.AC.Provider.Utils
{
    using System;
    using System.Reflection;
    using System.Text;
    using System.Xml.Serialization;

    using Esynctraining.AC.Provider.Constants;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The query string builder.
    /// </summary>
    internal class QueryStringBuilder
    {
        /// <summary>
        /// The entity to query string.
        /// </summary>
        /// <param name="entityObject">
        /// The p setup.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string EntityToQueryString(object entityObject)
        {
            if (entityObject == null)
            {
                return null;
            }

            var cmdParams = new StringBuilder();

            foreach (var propertyInfo in entityObject.GetType().GetProperties())
            {
                if (!propertyInfo.PropertyType.IsPublic)
                {
                    continue;
                }

                var propertyValue = propertyInfo.GetValue(entityObject, null);

                if (propertyValue == null)
                {
                    continue;
                }

                if (propertyValue is bool)
                {
                    propertyValue = propertyValue.Equals(true) ? 1 : 0;
                }
                else
                    if (propertyValue is DateTime)
                    {
                        if (propertyValue.Equals(DateTime.MinValue))
                        {
                            continue;
                        }

                        // propertyValue = ((DateTime)fieldValue).ToString(@"yyyy-MM-dd\THH:mm:ss.fffzzz");
                        propertyValue = ((DateTime)propertyValue).ToString(AdobeConnectProviderConstants.DateFormat);
                    }
                    else
                        if (propertyValue is TimeSpan)
                        {
                            if (propertyValue.Equals(TimeSpan.Zero))
                            {
                                continue;
                            }

                            propertyValue = ((TimeSpan)propertyValue).TotalMinutes;
                        }
                        else
                            if (propertyValue is Enum)
                            {
                                propertyValue = ((Enum)propertyValue).ToXmlString();
                            }

                var propertyName = GetPropertyXmlName(propertyInfo);

                cmdParams.AppendFormat("&{0}={1}", propertyName, HttpUtilsInternal.UrlEncode(propertyValue.ToString()));
            }

            return cmdParams.ToString();
        }

        /// <summary>
        /// The get property xml name.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetPropertyXmlName(PropertyInfo propertyInfo)
        {
            var xmlElement = propertyInfo.GetCustomAttributes(typeof(XmlElementAttribute), false) as XmlElementAttribute[];

            if (xmlElement != null && xmlElement.Length > 0)
            {
                if (!string.IsNullOrEmpty(xmlElement[0].ElementName))
                {
                    return xmlElement[0].ElementName;
                }
            }

            var xmlAttr = propertyInfo.GetCustomAttributes(typeof(XmlAttributeAttribute), false) as XmlAttributeAttribute[];

            if (xmlAttr != null && xmlAttr.Length > 0)
            {
                if (!string.IsNullOrEmpty(xmlAttr[0].AttributeName))
                {
                    return xmlAttr[0].AttributeName;
                }
            }

            return propertyInfo.Name.Replace('_', '-').ToLower();
        }
    }
}
