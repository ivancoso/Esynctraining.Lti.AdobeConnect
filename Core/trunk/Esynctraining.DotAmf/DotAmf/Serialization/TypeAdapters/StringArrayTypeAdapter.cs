namespace DotAmf.Serialization.TypeAdapters
{
    using System;
    using System.Collections;
    using System.Linq;

    public sealed class StringArrayTypeAdapter : BaseTypeAdapter<string[]>
    {
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

        private static string[] Convert(object value)
        {
            if (value != null)
            {
                if (value.GetType().IsArray)
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