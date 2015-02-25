namespace DotAmf.Serialization.TypeAdapters
{
    using System;
    using System.Globalization;

    /// <summary>
    ///     The float null type adapter.
    /// </summary>
    public class FloatTypeAdapter : BaseTypeAdapter<float?>
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
        public override object Adapt(Type type, object value)
        {
            if (type == typeof(float?))
            {
                return this.Convert(value);
            }

            if (value is int)
            {
                return this.Convert(value);
            }

            return null;
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
        private float? Convert(object value)
        {
            if (value != null)
            {
                float result;
                if (float.TryParse(value.ToString(), NumberStyles.Any, CustomFormat, out result))
                {
                    return result;
                }
            }

            return null;
        }

        #endregion
    }
}