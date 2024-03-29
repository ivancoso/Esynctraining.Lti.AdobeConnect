﻿namespace DotAmf.Serialization.TypeAdapters
{
    using System;
    using System.Globalization;

    /// <summary>
    ///     The decimal type adapter.
    /// </summary>
    public sealed class DecimalTypeAdapter : BaseTypeAdapter<decimal, decimal?>
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
            if (typeof(decimal).TypeHandle.Equals(type))
            {
                decimal? convertedValue = Convert(value);
                return convertedValue.HasValue ? convertedValue.Value : value;
            }

            if (typeof(decimal?).TypeHandle.Equals(type))
            {
                return Convert(value);
            }

            if (value is int)
            {
                return Convert(value);
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
        private static decimal? Convert(object value)
        {
            if (value != null)
            {
                decimal result;
                if (decimal.TryParse(value.ToString(), NumberStyles.Currency, CustomFormat, out result))
                {
                    return result;
                }
            }

            return default(decimal);
        }

        #endregion

    }

}