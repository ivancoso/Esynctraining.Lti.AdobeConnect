namespace Esynctraining.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    /// <summary>
    ///     The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get content type by extension.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetContentTypeByExtension(this string fileName)
        {
            var ext = Path.GetExtension(fileName).If(x => !string.IsNullOrEmpty(x), x => x.Substring(1)) ?? string.Empty;
            ext = ext.ToLower();
            switch (ext)
            {
                case "png":
                case "gif":
                case "tiff":
                case "bmp":
                case "pict":
                    return @"image/" + ext;
                case "jpg":
                case "jpe":
                case "jpeg":
                    return @"image/jpeg";
                case "swf":
                    return @"application/x-shockwave-flash";
                case "zip":
                    return @"application/zip";
                default:
                    return null;
            }
        }

        /// <summary>
        /// The replace all.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReplaceAll(this string str, string pattern)
        {
            str = pattern.Aggregate(str, (current, @char) => current.Replace(@char, ' '));
            return str;
        }

        /// <summary>
        /// The get enumeration name.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="enumType">
        /// The enumeration type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> enumeration name.
        /// </returns>
        public static string GetEnumName(this object value, Type enumType)
        {
            try
            {
                return Enum.GetName(enumType, value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// The to boolean.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{Boolean}"/>.
        /// </returns>
        public static bool? ToBoolean(this string value)
        {
            bool retVal;
            if (bool.TryParse(value, out retVal))
            {
                return retVal;
            }

            return null;
        }

        /// <summary>
        /// Truncates string if length is more then provided
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <param name="maxLength">
        /// The max length that would not be truncated.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string TruncateIfMoreThen(this string str, int maxLength)
        {
            if (!string.IsNullOrWhiteSpace(str) && str.Length > maxLength)
            {
                return str.Substring(0, maxLength - 3) + "...";
            }

            return str;
        }

    /// <summary>
        /// The to date time_ is o_8601.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ToDateTime_ISO_8601(this string value)
        {
            try
            {
                return DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// The to enumeration.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <typeparam name="TInput">
        /// Type of Enumeration
        /// </typeparam>
        /// <returns>
        /// The <see cref="TInput"/> instance.
        /// </returns>
        public static TInput ToEnum<TInput>(this string str) where TInput : struct
        {
            try
            {
                return (TInput)Enum.Parse(typeof(TInput), str, true);
            }
            catch
            {
                return default(TInput);
            }
        }

        /// <summary>
        /// The to enumeration that support null.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <typeparam name="TInput">
        /// The type of enumeration
        /// </typeparam>
        /// <returns>
        /// The <see cref="Nullable{TInput}"/> instance.
        /// </returns>
        public static TInput? ToNullableEnum<TInput>(this string str) where TInput : struct
        {
            try
            {
                return (TInput?)Enum.Parse(typeof(TInput), str, true);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// The to plain string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToPlainString(this IEnumerable<string> value)
        {
            var retVal = new StringBuilder();
            foreach (string str in value)
            {
                retVal.AppendFormat(" {0},", str);
            }

            return retVal.ToString().TrimEnd(",".ToCharArray());
        }

        /// <summary>
        /// The uppercase first later.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UppercaseFirstLater(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The from resource.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="resourceFileName">
        /// The resource file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FromResource(this string key, string resourceFileName)
        {
            return IoC.Resolve<IResourceProvider>().GetResourceString(key, resourceFileName);
        }

        #endregion
    }
}