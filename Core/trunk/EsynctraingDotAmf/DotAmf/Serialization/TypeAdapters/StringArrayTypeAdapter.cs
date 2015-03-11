namespace DotAmf.Serialization.TypeAdapters
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    ///     The float null type adapter.
    /// </summary>
    public sealed class StringArrayTypeAdapter : BaseTypeAdapter<string[]>
    {
        #region Static Fields

        /// <summary>
        /// The custom format.
        /// </summary>
        private static readonly NumberFormatInfo CustomFormat = new NumberFormatInfo
                                                                    {
                                                                        NegativeSign = "-", 
                                                                        NumberDecimalSeparator = ".", 
                                                                        NumberGroupSeparator = ",", 
                                                                        CurrencySymbol = "$", 
                                                                        CurrencyDecimalSeparator = ".", 
                                                                        CurrencyGroupSeparator = ",", 
                                                                    };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The adapt.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Adapt(RuntimeTypeHandle type, object value)
        {
            if (value is float[])
            {
                return Convert(value);
            }

            if (value is int[])
            {
                return Convert(value);
            }

            if (value is double[])
            {
                return Convert(value);
            }

            if (value is bool[])
            {
                return Convert(value);
            }

            return value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private static string[] Convert(object value)
        {
            if (value != null)
            {
                var type = value.GetType();
                if (type.IsArray)
                {
                    return ((IEnumerable)value).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();
                }
            }

            return null;
        }

        #endregion
    }
}