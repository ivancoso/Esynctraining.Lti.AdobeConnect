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
    }
}
