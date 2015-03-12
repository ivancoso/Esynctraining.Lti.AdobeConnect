namespace DotAmf.Serialization.TypeAdapters
{
    using System;

    /// <summary>
    /// The decimal type adapter.
    /// </summary>
    public sealed class GuidTypeAdapter : BaseTypeAdapter<Guid, Guid?>
    {
        public override object Adapt(RuntimeTypeHandle type, object value)
        {
            if (type.Equals(typeof(Guid).TypeHandle))
            {
                var convertedValue = Convert(value);
                return convertedValue.HasValue ? convertedValue.Value : value;
            }

            if (type.Equals(typeof(Guid?).TypeHandle))
            {
                return Convert(value);
            }

            return null;
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private static Guid? Convert(object value)
        {
            if (value != null)
            {
                Guid result;
                if (Guid.TryParse(value.ToString(), out result))
                {
                    return result;
                }
            }

            return Guid.Empty;
        }
    }
}
