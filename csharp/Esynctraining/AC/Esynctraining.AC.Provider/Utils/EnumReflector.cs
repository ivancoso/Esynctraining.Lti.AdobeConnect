namespace Esynctraining.AC.Provider.Utils
{
    using System;

    /// <summary>
    /// The enumeration reflector.
    /// </summary>
    public static class EnumReflector
    {
        /// <summary>
        /// The reflect enumerable.
        /// </summary>
        /// <param name="enumFieldName">
        /// The enumerable field name.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <typeparam name="T">
        /// Any enumerable.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ReflectEnum<T>(string enumFieldName, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(enumFieldName))
                throw new ArgumentException("Non-empty value expected", nameof(enumFieldName));            

            try
            {
                enumFieldName = enumFieldName.Replace('-', '_');

                return (T)Enum.Parse(typeof(T), enumFieldName, true);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return defaultValue;
        }

        public static T ReflectEnum<T>(string enumFieldValue)
        {
            if (string.IsNullOrWhiteSpace(enumFieldValue))
                throw new ArgumentException("Non-empty value expected", nameof(enumFieldValue));

            try
            {
                enumFieldValue = enumFieldValue.Replace('-', '_');
                return (T)Enum.Parse(typeof(T), enumFieldValue, true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error converting {enumFieldValue} to {typeof(T).Name}.", ex);
            }
        }

    }

}
